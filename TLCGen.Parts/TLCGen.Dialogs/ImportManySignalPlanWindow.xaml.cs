using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.Dialogs
{
    /// <summary>
    /// Interaction logic for ImportSignalPlanWindow.xaml
    /// </summary>
    public partial class ImportManySignalPlanWindow : Window
	{
		public ImportManySignalPlanWindow(HalfstarDataModel dataModel)
		{
			InitializeComponent();
            DataModel = dataModel;
		}

		public HalfstarDataModel DataModel { get; private set; }

        private enum ImportType
        {
            FickVASTabC
        }

		private void ImportButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (DataModel == null || !File.Exists(FileTextBox.File)) this.Close();

            var plans = new List<SignaalPlanModel>();

            var type = ImportType.FickVASTabC;

			try
			{
				switch(type)
				{
					case ImportType.FickVASTabC:
						var lines = File.ReadAllLines(FileTextBox.File);
                        var prmRE = new Regex(@"\s*PRM_code\[prm(?<naam>[a-zA-Z_]+)(?<number>[0-9]+e?)\]\s*=\s*\""[a-zA-Z0-9_]+\"";\s*PRM\[prm[a-zA-Z0-9_]+\]\s*=\s*(?<value>[0-9]+);.*", RegexOptions.Compiled);
                        foreach (var l in lines)
                        {
                            var m = prmRE.Match(l);
                            if (m.Success)
                            {
                                var naam = m.Groups["naam"].Value;
                                if (!(naam == "eu" || naam == "sp" || naam == "sv" || naam == "ep" || naam == "eg" || naam == "ctijd" || naam == "insch" || naam == "omsch")) continue;
                                var number = m.Groups["number"].Value;
                                var value = m.Groups["value"].Value;
                                var ss = (number.EndsWith("e") ? number.Length - 2 : number.Length - 1);
                                var fc = number.Substring(0, ss);
                                if (fc.EndsWith("e")) fc = fc.Substring(0, 1);
                                var pl = number.Substring(ss, 1);
                                if (pl.EndsWith("e")) pl = pl.Substring(0, 1);
                                var plan = plans.FirstOrDefault(x => x.Naam.EndsWith(pl));
                                if (plan == null)
                                {
                                    plan = new SignaalPlanModel() { Naam = "PL" + pl };
                                    plans.Add(plan);
                                }
                                var plfc = plan.Fasen.FirstOrDefault(x => x.FaseCyclus == fc);
                                if(naam == "eu" || naam == "sp" || naam == "sv" || naam == "ep" || naam == "eg")
                                {
                                    if (plfc == null)
                                    {
                                        plfc = new SignaalPlanFaseModel() { FaseCyclus = fc };
                                        plan.Fasen.Add(plfc);
                                    }
                                }
                                switch (naam)
                                {
                                    case "ctijd":
                                        plan.Cyclustijd = int.Parse(value);
                                        break;
                                    case "insch":
                                        plan.StartMoment = int.Parse(value);
                                        break;
                                    case "omsch":
                                        plan.SwitchMoment = int.Parse(value);
                                        break;
                                    case "eu":
                                        
                                        if (!number.EndsWith("e")) plfc.A1 = int.Parse(value);
                                        else                       plfc.A2 = int.Parse(value);
                                        break;
                                    case "sp":
                                        if (!number.EndsWith("e")) plfc.B1 = int.Parse(value);
                                        else                       plfc.B2 = int.Parse(value);
                                        break;
                                    case "sv":
                                        if (!number.EndsWith("e")) plfc.C1 = int.Parse(value);
                                        else                       plfc.C2 = int.Parse(value);
                                        break;
                                    case "ep":
                                        if (!number.EndsWith("e")) plfc.D1 = int.Parse(value);
                                        else                       plfc.D2 = int.Parse(value);
                                        break;
                                    case "eg":
                                        if (!number.EndsWith("e")) plfc.E1 = int.Parse(value);
                                        else                       plfc.E2 = int.Parse(value);
                                        break;
                                }
                            }
                        }
						break;
					default:
						break;
				}
                DataModel.SignaalPlannen.Clear();
                foreach(var plan in plans)
                {
                    DataModel.SignaalPlannen.Add(plan);
                    foreach (var gk in DataModel.GekoppeldeKruisingen)
                    {
                        gk.PlanUitgangen.Add(new HalfstarGekoppeldeKruisingPlanUitgangModel
                        {
                            Kruising = gk.KruisingNaam,
                            Plan = plan.Naam,
                            Type = gk.Type
                        });
                        gk.PlanUitgangen.BubbleSort();
                        gk.PlanIngangen.Add(new HalfstarGekoppeldeKruisingPlanIngangModel
                        {
                            Kruising = gk.KruisingNaam,
                            Plan = plan.Naam,
                            Type = gk.Type
                        });
                        gk.PlanIngangen.BubbleSort();
                    }
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
