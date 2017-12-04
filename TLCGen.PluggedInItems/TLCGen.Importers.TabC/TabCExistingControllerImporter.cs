using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.Importers.TabC
{
    [TLCGenPlugin(TLCGenPluginElems.Importer)]
    public class TabCExistingControllerImporter : ITLCGenImporter
    {
        public ControllerModel Controller
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                
            }
        }

        public bool ImportsIntoExisting => true;

	    public string Name => "Importeer tab.c (in bestaande regeling)";

	    public string GetPluginName()
        {
            return Name;
        }

        public ControllerModel ImportController(ControllerModel c = null)
        {
            if(c == null)
            {
                throw new NullReferenceException("TabC importer: Controller to import into cannot be null.");
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
                            if (!_Fasen.Contains(fc1.Replace("fc", "")))
                                _Fasen.Add(fc1.Replace("fc", ""));
                        }
                    }
                    string AllPhasesMessage = "";
                    List<FaseCyclusModel> newfcs = new List<FaseCyclusModel>();
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (!_Fasen.Contains(fcm.Naam))
                        {
                            AllPhasesMessage = AllPhasesMessage + fcm.Naam + "\n";
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
                        var tempcs = new List<ConflictModel>();
                        foreach (ConflictModel cm in c.InterSignaalGroep.Conflicten)
                        {
                            if(newfcs.Where(x => x.Naam == cm.FaseVan || x.Naam == cm.FaseNaar).Any())
                            {
                                tempcs.Add(cm);
                            }
                        }
                        foreach(var tc in tempcs)
                        {
                            c.InterSignaalGroep.Conflicten.Remove(tc);
                        }

                        // Build a list of the Phases with conflicts from the tab.c file
                        TabCImportHelperOutcome NewData = TabCImportHelper.GetNewData(lines);

                        // Copy the results into the ControllerVM
                        string NewPhasesMessage = "";

                        // Store current conflicts
                        List<ConflictModel> OldConflicts = new List<ConflictModel>();
                        foreach (ConflictModel _cm in c.InterSignaalGroep.Conflicten)
                            OldConflicts.Add(_cm);

                        foreach (FaseCyclusModel newfcm in NewData.Fasen)
                        {
                            // Search for existing phases
                            bool found = false;
                            foreach (FaseCyclusModel fcm in c.Fasen)
                            {
                                if (newfcm.Naam == fcm.Naam)
                                {
                                    found = true;
                                }
                            }
                            if (!found)
                            {
                                c.Fasen.Add(newfcm);
                                c.ModuleMolen.FasenModuleData.Add(new FaseCyclusModuleDataModel() { FaseCyclus = newfcm.Naam });
                                NewPhasesMessage = NewPhasesMessage + newfcm.Naam + "\n";
                            }
                        }

                        foreach(ConflictModel cm in NewData.Conflicten)
                        {

                            ConflictModel _cm = new ConflictModel();
                            _cm.FaseVan = cm.FaseVan;
                            _cm.FaseNaar = cm.FaseNaar;
                            _cm.Waarde = cm.Waarde;
                            c.InterSignaalGroep.Conflicten.Add(_cm);
                            
                            // Check for new conflicts
#warning TODO - At this point: check if new conflicts have been added, and act accordingly
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
