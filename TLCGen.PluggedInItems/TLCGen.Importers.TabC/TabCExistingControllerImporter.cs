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
using TLCGen.Settings;

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

	    public string Name => "Importeer tab.c (in geopende regeling)";

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

            var openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Title = "Selecteer tab.c file voor importeren",
                Filter = "tab.c files|*tab.c|Alle files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var lines = File.ReadAllLines(openFileDialog.FileName);

                    // Check if at least all Phases in the Controller occur in the tab.c file
                    var _Fasen = new List<string>();
                    foreach (var line in lines)
                    {
                        if (Regex.IsMatch(line, @"^\s+TO_max\["))
                        {
                            var fc1 = Regex.Replace(line, @"^\s*TO_max\s*\[\s*(fc[0-9]+).*", "$1");
                            if (!_Fasen.Contains(fc1.Replace("fc", "")))
                                _Fasen.Add(fc1.Replace("fc", ""));
                        }
                    }
                    var AllPhasesMessage = "";
                    var newfcs = new List<FaseCyclusModel>();
                    foreach (var fcm in c.Fasen)
                    {
                        if (!_Fasen.Contains(fcm.Naam))
                        {
                            AllPhasesMessage = AllPhasesMessage + fcm.Naam + "\n";
                            newfcs.Add(fcm);
                        }
                    }
                    var result = MessageBoxResult.Yes;
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
                        foreach (var cm in c.InterSignaalGroep.Conflicten)
                        {
                            if(newfcs.Any(x => x.Naam == cm.FaseVan || x.Naam == cm.FaseNaar))
                            {
                                tempcs.Add(cm);
                            }
                        }
                        foreach(var tc in tempcs)
                        {
                            c.InterSignaalGroep.Conflicten.Remove(tc);
                        }

                        // Build a list of the Phases with conflicts from the tab.c file
                        var newData = TabCImportHelper.GetNewData(lines);

                        // Copy the results into the ControllerVM
                        var NewPhasesMessage = "";

                        // Store current conflicts
                        var OldConflicts = new List<ConflictModel>();
                        foreach (var _cm in c.InterSignaalGroep.Conflicten)
                            OldConflicts.Add(_cm);

                        foreach (var newfcm in newData.Fasen)
                        {
                            // Search for existing phases
	                        var fc = c.Fasen.FirstOrDefault(x => x.Naam == newfcm.Naam);
                            if (fc == null)
                            {
                                newfcm.Type = Settings.Utilities.FaseCyclusUtilities.GetFaseTypeFromNaam(newfcm.Naam);
                                DefaultsProvider.Default.SetDefaultsOnModel(newfcm, newfcm.Type.ToString());
                                c.Fasen.Add(newfcm);
                                c.ModuleMolen.FasenModuleData.Add(new FaseCyclusModuleDataModel { FaseCyclus = newfcm.Naam });
                                NewPhasesMessage = NewPhasesMessage + newfcm.Naam + "\n";
                            }
                            else
                            {
	                            foreach (var nd in newfcm.Detectoren)
	                            {
		                            if (fc.Detectoren.All(x => x.Naam != nd.Naam))
		                            {
										fc.Detectoren.Add(nd);
		                            }
	                            }
                            }
                        }

                        foreach(var cm in newData.Conflicten)
                        {
                            var _cm = new ConflictModel
                            {
                                FaseVan = cm.FaseVan,
                                FaseNaar = cm.FaseNaar,
                                Waarde = cm.Waarde
                            };
                            var old = c.InterSignaalGroep.Conflicten.FirstOrDefault(x => x.FaseVan == cm.FaseVan && x.FaseNaar == cm.FaseNaar);
                            if (old != null)
                            {
                                c.InterSignaalGroep.Conflicten.Remove(old);
                            }

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
                    MessageBox.Show("Fout bij uitlezen tab.c.:\n" + e.Message, "Fout bij importeren tab.c");
                    return null;
                }
            }

            return null;
        }
    }
}
