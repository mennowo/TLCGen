using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using TLCGen.Extensions;
using TLCGen.Models;

namespace TLCGen.Dialogs
{
    /// <summary>
    /// Interaction logic for ImportSignalPlanWindow.xaml
    /// </summary>
    public partial class TLCImportSignalPlannenWindow : Window
	{
        private bool _import;

		public TLCImportSignalPlannenWindow(HalfstarDataModel dataModel, bool import)
		{
            _import = import;
            InitializeComponent();
            if (_import) ImportExportButton.Content = "Import";
            else ImportExportButton.Content = "Export";
            DataModel = dataModel;
		}

		public HalfstarDataModel DataModel { get; private set; }

		private void ImportButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (DataModel == null || _import && !File.Exists(FileTextBox.File)) this.Close();

			try
			{
                if (_import)
                {
                    var fn = FileTextBox.File;
                    var lines = File.ReadAllLines(fn);
                    foreach(var l in lines)
                    {
                        var items = l.Split(';');
                        if (items.Length <= 1) continue;
                        var plan = DataModel.SignaalPlannen.FirstOrDefault(x => x.Naam == items[0]);
                        if(plan != null)
                        {
                            if(items[1] == "PLDATA" && items.Length >= 6)
                            {
                                plan.Cyclustijd = int.Parse(items[2]);
                                plan.StartMoment = int.Parse(items[3]);
                                plan.SwitchMoment = int.Parse(items[4]);
                                plan.Commentaar = items[5];
                            }
                            else if(items.Length >= 12)
                            {
                                var fc = plan.Fasen.FirstOrDefault(x => x.FaseCyclus == items[1]);
                                if (fc != null)
                                {
                                    fc.A1 = string.IsNullOrWhiteSpace(items[2]) ? 0 : int.Parse(items[2]);
                                    fc.B1 = string.IsNullOrWhiteSpace(items[3]) ? 0 : int.Parse(items[3]);
                                    fc.C1 = string.IsNullOrWhiteSpace(items[4]) ? 0 : int.Parse(items[4]);
                                    fc.D1 = string.IsNullOrWhiteSpace(items[5]) ? 0 : int.Parse(items[5]);
                                    fc.E1 = string.IsNullOrWhiteSpace(items[6]) ? 0 : int.Parse(items[6]);
                                    fc.A2 = string.IsNullOrWhiteSpace(items[7]) ? 0 : int.Parse(items[7]);
                                    fc.B2 = string.IsNullOrWhiteSpace(items[8]) ? 0 : int.Parse(items[8]);
                                    fc.C2 = string.IsNullOrWhiteSpace(items[9]) ? 0 : int.Parse(items[9]);
                                    fc.D2 = string.IsNullOrWhiteSpace(items[10]) ? 0 : int.Parse(items[10]);
                                    fc.E2 = string.IsNullOrWhiteSpace(items[11]) ? 0 : int.Parse(items[11]);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var fn = FileTextBox.File;
                    if (!fn.ToLower().EndsWith(".plcsv")) fn += ".plcsv";
                    var sb = new StringBuilder();
                    foreach(var pl in DataModel.SignaalPlannen)
                    {
                        sb.AppendLine($"{pl.Naam};PLDATA;{pl.Cyclustijd};{pl.StartMoment};{pl.SwitchMoment};{pl.Commentaar};");
                        foreach (var fc in pl.Fasen)
                        {
                            sb.AppendLine($"{pl.Naam};{fc.FaseCyclus};{fc.A1};{fc.B1};{fc.C1};{fc.D1};{fc.E1};{fc.A2};{fc.B2};{fc.C2};{fc.D2};{fc.E2};");
                        }
                    }
                    File.WriteAllText(fn, sb.ToString());
                }
			}
			catch (Exception ee)
			{
				MessageBox.Show(
					"Fout bij inlezen bestand " + FileTextBox.File + ". Is het elders open?\nOriginele error:\n" + ee.ToString(),
					"Error bij lezen bestand");
			}
	        this.Close();
		}
		
		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
