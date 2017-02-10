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

namespace TLCGen.Importers.TabC
{
    [TLCGenPlugin(TLCGenPluginElems.Importer)]
    public class TabCNewControllerImporter : ITLCGenImporter
    {
        public ControllerModel Controller
        {
            get
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException("TabC importer: Controller parsed is not null, which it should be for importing into new.");
            }


            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Title = "Selecteer tab.c file voor importeren";
            openFileDialog.Filter = "tab.c files|*tab.c|Alle files|*.*";

            ControllerModel newc = new ControllerModel();

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    if (lines.Count() <= 1)
                        throw new NotImplementedException("Het bestand heeft minder dan 2 regels.");

                    // Build a list of the Phases with conflicts from the tab.c file
                    TabCImportHelperOutcome NewData = TabCImportHelper.GetNewData(lines);
                    NewData.Fasen.BubbleSort();
                    foreach (FaseCyclusModel fcm in NewData.Fasen)
                    {
                        newc.Fasen.Add(fcm);
                        newc.ModuleMolen.FasenModuleData.Add(new FaseCyclusModuleDataModel() { FaseCyclus = fcm.Naam });
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
