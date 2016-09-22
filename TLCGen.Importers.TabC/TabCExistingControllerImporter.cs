using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using TLCGen.Interfaces.Public;
using TLCGen.Models;

namespace TLCGen.Importers.TabC
{
    [TLCGenImporter]
    public class TabCExistingControllerImporter : IImporter
    {
        public bool ImportsIntoExisting { get { return true; } }
        public string Name { get { return "Importeer tab.c (in bestaande regeling)"; } }

        public ControllerModel ImportController(ControllerModel c = null)
        {
            if(c == null)
            {
                throw new NotImplementedException("TabC importer: Controller to import into cannot be null.");
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Title = "Selecteer tab.c file voor importeren";
            openFileDialog.Filter = "tab.c files|*tab.c|Alle files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string[] lines = File.ReadAllLines(openFileDialog.FileName);

                    // Check if at least all Phases in the Controller occur in the tab.c file
                    List<string> _Fasen = new List<string>();
                    foreach (string line in lines)
                    {
                        if (Regex.IsMatch(line, @"^\s+TO_max\["))
                        {
                            string fc1 = Regex.Replace(line, @"^\s*TO_max\s*\[\s*(fc[0-9]+).*", "$1");
                            if (!_Fasen.Contains(fc1))
                                _Fasen.Add(fc1);
                        }
                    }
                    string AllPhasesMessage = "";
                    List<FaseCyclusModel> newfcs = new List<FaseCyclusModel>();
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (!_Fasen.Contains(fcm.Define))
                        {
                            AllPhasesMessage = AllPhasesMessage + fcm.Define + "\n";
                            newfcs.Add(fcm);
                        }
                    }
                    MessageBoxResult result = MessageBoxResult.Yes;
                    if (!string.IsNullOrEmpty(AllPhasesMessage))
                    {
                        result = MessageBox.Show("Niet alle fasen uit de regeling komen voor in de tab.c file.\nConflicten van de volgende fasen worden verwijderd:\n\n" +
                            AllPhasesMessage + "\nDoorgaan?", "Niet alle fasen gevonden", MessageBoxButton.YesNo);
                    }

                    // Continue...
                    if (result == MessageBoxResult.Yes)
                    {
                        // Clear conflicts from Phases not in tab.c file
                        foreach (FaseCyclusModel fcm in newfcs)
                        {
                            fcm.Conflicten.Clear();
                        }

                        // Build a list of the Phases with conflicts from the tab.c file
                        List<FaseCyclusModel> NewFasen = TabCImportHelper.GetNewFasenList(lines);

                        // Copy the results into the ControllerVM
                        string NewPhasesMessage = "";
                        List<ConflictModel> OldConflicts = new List<ConflictModel>();
                        foreach (FaseCyclusModel newfcm in NewFasen)
                        {
                            // Search for existing phases
                            bool found = false;
                            foreach (FaseCyclusModel fcm in c.Fasen)
                            {
                                if (newfcm.Define == fcm.Define)
                                {
                                    found = true;

                                    // Store current conflicts
                                    OldConflicts.Clear();
                                    foreach (ConflictModel _cm in fcm.Conflicten)
                                        OldConflicts.Add(_cm);

                                    // Load new conflicts
                                    fcm.Conflicten.Clear();
                                    foreach (ConflictModel cm in newfcm.Conflicten)
                                    {
                                        ConflictModel _cm = new ConflictModel();
                                        _cm.FaseVan = fcm.Define;
                                        _cm.FaseNaar = cm.FaseNaar;
                                        _cm.Waarde = cm.Waarde;
                                        fcm.Conflicten.Add(_cm);
                                    }

                                    // Check for new conflicts
#warning TODO - At this point: check if new conflicts have been added, and act accordingly
                                }
                            }
                            if (!found)
                            {
                                c.Fasen.Add(newfcm);
                                NewPhasesMessage = NewPhasesMessage + newfcm.Define + "\n";
                            }
                        }
                        if (!string.IsNullOrEmpty(NewPhasesMessage))
                        {
                            MessageBox.Show("De volgende fasen uit de tab.c file zijn nieuw toegevoegd in de regeling:\n\n" +
                                NewPhasesMessage, "Nieuwe fasen toegevoegd", MessageBoxButton.OK);
                        }
                        return c;
                    }
                    else
                        return null;
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Fout bij uitlezen tab.c.:\n" + e.Message, "Fout bij importeren tab.c");
                    return null;
                }
            }

            return null;
        }
    }
}
