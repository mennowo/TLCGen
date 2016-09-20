using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Interfaces.Public;
using TLCGen.Models;

namespace TLCGen.Importers.TabC
{
    [TLCGenImporter]
    public class TabCNewControllerImporter : IImporter
    {
        public bool ImportsIntoExisting { get { return false; } }
        public string Name { get { return "Importeer tab.c (nieuwe regeling)"; } }

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
                    List<FaseCyclusModel> NewFasen = TabCImportHelper.GetNewFasenList(lines);
                    foreach (FaseCyclusModel fcm in NewFasen)
                    {
                        newc.Fasen.Add(fcm);
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Fout bij uitlezen tab.c.:\n" + e.Message, "Fout bij importeren tab.c");
                }
            }

            return newc;
        }
    }
}
