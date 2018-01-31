using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.Dialogs
{
	/// <summary>
	/// Interaction logic for ImportSignalPlanWindow.xaml
	/// </summary>
	public partial class ImportSignalPlanWindow : Window
	{
		public ImportSignalPlanWindow(SignaalPlanModel plan)
		{
			InitializeComponent();
			SignaalPlan = plan;
		}

		public SignaalPlanModel SignaalPlan { get; private set; }

		private void ImportButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (SignaalPlan == null || !File.Exists(FileTextBox.File)) this.Close();

			var plcopy = DeepCloner.DeepClone(SignaalPlan);
			var error = false;

			try
			{
				var ext = Path.GetExtension(FileTextBox.File.ToLower());
				switch(ext)
				{
					case ".csv":
						var lines = File.ReadAllLines(FileTextBox.File);
						var iState = 0;
						foreach (var s in lines)
						{
							if (iState == 0)
							{
								if (s.Contains("CYCLUSTIJD")) iState = 1;
								if (s.Contains("FASENDIAGRAMSIGNAALGROEPEN")) iState = 2;
								continue;
							}

							if (iState == 1)
							{
								var parts = s.Split('"');
								SignaalPlan.Cyclustijd = int.Parse(parts[1]);
								iState = 0;
							}
							else if (iState == 2)
								iState++;
							else if (iState == 3 && !string.IsNullOrWhiteSpace(s))
							{
								string[] parts = s.Split('"');
								string strPhase = parts[1];
								bool found = false;
								foreach (var fc in SignaalPlan.Fasen)
								{
									var nfc = fc.FaseCyclus.Replace("fc", "");
									if (nfc.PadLeft(strPhase.Length, '0') == strPhase)
									{
										if (fc.B1 == 0)
										{
											fc.B1 = int.Parse(parts[5]);
											fc.D1 = int.Parse(parts[7]);
										}
										else
										{
											fc.B2 = int.Parse(parts[5]);
											fc.D2 = int.Parse(parts[7]);
										}

										found = true;
										break;
									}
								}

								if (!found)
								{
									MessageBox.Show("Fout in plan uit bestand " + FileTextBox.File + ".\nNiet alle fasen uit het bestand zitten in de regelaar.",
										"Fout in plan");
									SignaalPlan = plcopy;
									this.Close();
									return;
								}
							}
							else if (string.IsNullOrWhiteSpace(s))
								iState = 0;
						}

						if (SignaalPlan.Fasen.Any(x => x.B1 == 0 || x.D1 == 0))
						{
							MessageBox.Show("Fout in plan uit bestand " + FileTextBox.File + ".\nNiet alle fasen uit de regelaar zijn gevonden.",
								"Fout in plan");
							SignaalPlan = plcopy;
							this.Close();
							return;
						}
						break;
					case ".c":
						break;
				}
				this.Close();
			}
			catch (Exception ee)
			{
				MessageBox.Show(
					"Fout bij inlezen bestand " + FileTextBox.File + ". Is het elders open?\nOriginele error:\n" + ee.ToString(),
					"Error bij lezen bestand");
				SignaalPlan = plcopy;
				this.Close();
			}
		}
		
		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
