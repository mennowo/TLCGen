using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.Dialogs
{
	/// <summary>
	/// Interaction logic for ImportDplCWindow.xaml
	/// </summary>
	public partial class ImportDplCWindow : Window
	{
		public ImportDplCWindow(ControllerModel controller)
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
				var ext = Path.GetExtension(FileTextBox.File.ToLower());
				switch(ext)
				{
					case ".c":
						var lines = File.ReadAllLines(FileTextBox.File);
						var iState = 0;
						foreach (var line in lines)
						{
							if (Regex.IsMatch(line, @"^\s*X_us"))
							{
								var match = Regex.Match(line, @"^\s*X_us\[([a-zA-Z0-9]+)\]\s*=\s*([0-9NG]+);\s*Y_us\[[a-zA-Z0-9]+\]\s*=\s*([0-9NG]+);");
								if (match.Groups.Count == 4)
								{				
									var name = match.Groups[1].Value.ToLower().Replace("fc", "");
									var x = match.Groups[2];
									var y = match.Groups[3];

									foreach (var fc in _controller.Fasen)
									{
										if (fc.Naam == name)
										{
											fc.BitmapCoordinaten.Add(new BitmapCoordinaatModel
											{
												X = x.Value == "NG" ? -1 : int.Parse(x.Value),
												Y = y.Value == "NG" ? -1 : int.Parse(y.Value)													
											});
										}
									}
								}
							}
							if (Regex.IsMatch(line, @"^\s*X_is"))
							{
								var match = Regex.Match(line, @"^\s*X_is\[([a-zA-Z0-9]+)\]\s*=\s*([0-9NG]+);\s*Y_is\[[a-zA-Z0-9]+\]\s*=\s*([0-9NG]+);");
								if (match.Groups.Count == 4)
								{				
									var name = match.Groups[1].Value.ToLower().Replace("d", "");
									var x = match.Groups[2];
									var y = match.Groups[3];

									foreach (var fc in _controller.Detectoren)
									{
										if (fc.Naam == name)
										{
											fc.BitmapCoordinaten.Add(new BitmapCoordinaatModel
											{
												X = x.Value == "NG" ? -1 : int.Parse(x.Value),
												Y = y.Value == "NG" ? -1 : int.Parse(y.Value)													
											});
										}
									}

									var dets = _controller.Fasen.SelectMany(f => f.Detectoren).Concat(_controller.Detectoren);
									foreach (var d in dets)
									{
										if (d.Naam == name)
										{
											d.BitmapCoordinaten.Add(new BitmapCoordinaatModel
											{
												X = x.Value == "NG" ? -1 : int.Parse(x.Value),
												Y = y.Value == "NG" ? -1 : int.Parse(y.Value)													
											});
										}
									}
								}
							}
						}
						break;
				}
				this.Close();
			}
			catch (Exception ee)
			{
				MessageBox.Show(
					"Fout bij inlezen bestand " + FileTextBox.File + ". Is het elders open?\nOriginele error:\n" + ee.ToString(),
					"Error bij lezen bestand");
				this.Close();
			}
		}
		
		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
