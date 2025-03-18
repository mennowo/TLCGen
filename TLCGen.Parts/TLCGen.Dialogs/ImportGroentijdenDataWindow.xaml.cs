using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Dialogs
{
    /// <summary>
    /// Interaction logic for ImportDplCWindow.xaml
    /// </summary>
    public partial class ImportGroentijdenDataWindow : Window
	{
		public ImportGroentijdenDataWindow(ControllerModel controller)
		{
			InitializeComponent();
			_controller = controller;
		}

		private ControllerModel _controller;

		private void ImportButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (_controller == null || !File.Exists(FileTextBox.File))
			{
				Close();
				return;
			}

			try
			{
                var lines = File.ReadAllLines(FileTextBox.File);
                if (lines.Length > 0)
                {
	                if (lines[0].Contains("QIC") && lines.Length > 2)
	                {
		                var items = lines[1].Split(';');
		                var gtSets = new GroentijdenSetModel[items.Length - 1];
		                for (var i = 1; i < items.Length; i++)
		                {
			                var periode = _controller.PeriodenData.Perioden.FirstOrDefault(x => x.Naam == items[i]);
			                if (periode != null)
			                {
				                var set = _controller.GroentijdenSets.FirstOrDefault(x => x.Naam == periode.GroentijdenSet);
				                if (set != null) gtSets[i - 1] = set;
			                }
		                }
		                for (int i = 2; i < lines.Length; i++)
		                {
			                var values = lines[i].Split(';');
			                for (var j = 1; j < values.Length; j++)
			                {
								if (gtSets[j - 1] == null) continue; 
								var mgSetFc = gtSets[j - 1].Groentijden.FirstOrDefault(x => x.FaseCyclus == values[0]);
								if (mgSetFc != null)
								{
									var fc = _controller.Fasen.FirstOrDefault(x => x.Naam == mgSetFc.FaseCyclus);
								    if ((fc?.Type == FaseTypeEnum.Auto || fc?.Type == FaseTypeEnum.OV) 
								        && int.TryParse(values[j], out var mgWaarde)) mgSetFc.Waarde = mgWaarde;
								}
			                }
		                }
	                }
	                else
	                {
		                foreach (var l in lines)
		                {
		                    var items = l.Split(new[] { ';', ',' });
		                    if (items.Length > 1 && _controller != null && _controller.Fasen.Any(x => x.Naam == items[0]))
		                    {
		                        for (var img = 1; img < items.Length; ++img)
		                        {
		                            if((img - 1) < _controller.GroentijdenSets.Count && int.TryParse(items[img], out var mgWaarde))
		                            {
		                                var mgSet = _controller.GroentijdenSets[img - 1];
		                                var mgSetFc = mgSet.Groentijden.FirstOrDefault(x => x.FaseCyclus == items[0]);
		                                mgSetFc.Waarde = mgWaarde;
		                            }
		                        }
		                    }
		                }
	                }
                }
                this.DialogResult = true;
                this.Close();
			}
			catch (Exception ee)
			{
				MessageBox.Show(
					"Fout bij inlezen bestand " + FileTextBox.File + ". Is het elders open?\nOriginele error:\n" + ee.ToString(),
					"Error bij lezen bestand");
                this.DialogResult = false;
				this.Close();
			}
		}
		
		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
            this.DialogResult = false;
			this.Close();
		}
	}
}
