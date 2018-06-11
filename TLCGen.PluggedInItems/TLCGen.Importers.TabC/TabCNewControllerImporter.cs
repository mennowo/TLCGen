using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Extensions;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.Importers.TabC
{
    [TLCGenPlugin(TLCGenPluginElems.Importer)]
    public class TabCNewControllerImporter : ITLCGenImporter
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

        public bool ImportsIntoExisting { get { return false; } }
        public string Name { get { return "Importeer tab.c (nieuwe regeling)"; } }

        public string GetPluginName()
        {
            return Name;
        }

        public ControllerModel ImportController(ControllerModel c = null)
        {
            if (c != null)
            {
                throw new NullReferenceException("TabC importer: Controller parsed is not null, which it should be for importing into new.");
            }

			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				CheckFileExists = true,
				Title = "Selecteer tab.c file voor importeren",
				Filter = "Import files|*tab.c;*.ccol|Alle files|*.*"
			};

			ControllerModel newc = new ControllerModel();

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    DefaultsProvider.Default.SetDefaultsOnModel(newc.Data);
                    newc.Data.GarantieOntruimingsTijden = false;

                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    if (lines.Length <= 1)
                        throw new IndexOutOfRangeException("Het bestand heeft minder dan 2 regels.");

                    // Build a list of the Phases with conflicts from the tab.c file
                    TabCImportHelperOutcome NewData = TabCImportHelper.GetNewData(lines);
                    NewData.Fasen.BubbleSort();
                    foreach (FaseCyclusModel fcm in NewData.Fasen)
                    {
                        fcm.Type = Settings.Utilities.FaseCyclusUtilities.GetFaseTypeFromNaam(fcm.Naam);
                        DefaultsProvider.Default.SetDefaultsOnModel(fcm, fcm.Type.ToString());
                        newc.Fasen.Add(fcm);
                        var fcdm = new FaseCyclusModuleDataModel() { FaseCyclus = fcm.Naam };
                        DefaultsProvider.Default.SetDefaultsOnModel(fcdm, fcm.Type.ToString());
                        newc.ModuleMolen.FasenModuleData.Add(fcdm);
                    }
                    NewData.Conflicten.BubbleSort();
                    foreach (ConflictModel cm in NewData.Conflicten)
                    {
                        newc.InterSignaalGroep.Conflicten.Add(cm);
                    }
                    return newc;
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
