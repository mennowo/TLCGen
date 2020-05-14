using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using TLCGen.Models;

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
			if (_controller == null || !File.Exists(FileTextBox.File)) this.Close();

			try
			{
                var lines = File.ReadAllLines(FileTextBox.File);
                foreach (var l in lines)
                {
                    var items = l.Split(new[] { ';', ',' });
                    if (items.Length > 1 && _controller.Fasen.Any(x => x.Naam == items[0]))
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
