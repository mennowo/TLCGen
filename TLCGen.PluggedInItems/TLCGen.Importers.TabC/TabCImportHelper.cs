using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.Importers.TabC
{
    public class TabCImportHelperOutcome
    {
        public List<DetectorModel> Detectoren { get; set; }
        public List<FaseCyclusModel> Fasen { get; set; }
        public List<ConflictModel> Conflicten { get; set; }

        public TabCImportHelperOutcome()
        {
            Fasen = new List<FaseCyclusModel>();
            Conflicten = new List<ConflictModel>();
	        Detectoren = new List<DetectorModel>();
        }
    }

    public static class TabCImportHelper
    {
        public static TabCImportHelperOutcome GetNewData(string[] lines)
        {
	        var dz = MessageBoxResult.None;

            TabCImportHelperOutcome outcome = new TabCImportHelperOutcome();

            if (lines.Count() <= 1)
            {
                return null;
            }

            // Compile a list of Phases with conflicts from the file
            foreach (var _line in lines)
            {
                var line = Regex.Replace(_line, @"/\*.*\*/", "");
                if (Regex.IsMatch(line, @"^\s+TO_max\["))
                {
                    string fc1 = Regex.Replace(line, @"^\s*TO_max\s*\[\s*(fc[0-9]+).*", "$1");
                    string fc2 = Regex.Replace(line, @"^\s*TO_max\s*\[\s*fc[0-9]+\s*\]\s*\[\s*(fc[0-9]+).*", "$1");
                    string _conf = Regex.Replace(line, @"^\s*TO_max\s*\[\s*fc[0-9]+\s*\]\s*\[\s*fc[0-9]+\s*\]\s*=\s*(([0-9]+|FK|GK|GKL)).*", "$1");

                    int conf = 0;
                    if (_conf == "FK" || _conf == "GK" || _conf == "GKL")
                    {
                        continue;
                    }

                    if (!Int32.TryParse(_conf, out conf))
                    {
                        throw new InvalidOperationException($"Conflict van {fc1} naar {fc2} heeft een foutieve waarde: {_conf}");
                    }

                    FaseCyclusModel _fcm1 = null;
                    foreach (var fcm in outcome.Fasen)
                    {
                        if (fcm.Naam == fc1.Replace("fc", ""))
                        {
                            _fcm1 = fcm;
                            break;
                        }
                    }
                    if (_fcm1 == null)
                    {
                        _fcm1 = new FaseCyclusModel();
                        _fcm1.Naam = fc1.Replace("fc", "");
                        outcome.Fasen.Add(_fcm1);
                    }

                    FaseCyclusModel _fcm2 = null;
                    foreach (FaseCyclusModel fcm in outcome.Fasen)
                    {
                        if (fcm.Naam == fc2.Replace("fc", ""))
                        {
                            _fcm2 = fcm;
                            break;
                        }
                    }
                    if (_fcm2 == null)
                    {
                        _fcm2 = new FaseCyclusModel();
                        _fcm2.Naam = fc2.Replace("fc", "");
                        outcome.Fasen.Add(_fcm2);
                    }
                    outcome.Conflicten.Add(new ConflictModel() { FaseVan = _fcm1.Naam, FaseNaar = _fcm2.Naam, Waarde = conf });
                }

	            if (dz != MessageBoxResult.No &&
	                Regex.IsMatch(line, @"^\s+D_code\[") || Regex.IsMatch(line, @"^\s+TBG_max\["))
	            {
		            if (dz != MessageBoxResult.Yes)
		            {
			            dz = MessageBox.Show("Detectoren zoeken in tab.c?", "Detectoren zoeken?", MessageBoxButton.YesNo);
			            if (dz == MessageBoxResult.No)
			            {
							continue;
			            }
		            }
		            string d = Regex.Replace(line, @"^\s*D_code\s*\[\s*(d[0-9a-zA-Z_]+).*", "$1");
		            string dd = Regex.Replace(d, @"^d", "");
		            if (dd.StartsWith("r"))
		            {
			            dd = "k" + dd.Substring(1);
		            }
		            var tdb = -1;
		            var tdh = -1;
		            var tog = -1;
		            var tbg = -1;
		            if (Regex.IsMatch(line, @"TDB_max"))
		            {
			            var mtdb = Regex.Match(line, @"TDB_max\[.*?\]\s*=\s*([0-9]+);");
			            if (mtdb.Groups.Count > 1)
				            int.TryParse(mtdb.Groups[1].Value, out tdb);
		            }
		            if (Regex.IsMatch(line, @"TDH_max"))
		            {
			            var mtdb = Regex.Match(line, @"TDH_max\[.*?\]\s*=\s*([0-9]+);");
			            if (mtdb.Groups.Count > 1)
				            int.TryParse(mtdb.Groups[1].Value, out tdh);
		            }
		            if (Regex.IsMatch(line, @"TBG_max"))
		            {
			            var mtdb = Regex.Match(line, @"TBG_max\[.*?\]\s*=\s*([0-9]+);");
			            if (mtdb.Groups.Count > 1)
				            int.TryParse(mtdb.Groups[1].Value, out tbg);
		            }
		            if (Regex.IsMatch(line, @"TOG_max"))
		            {
			            var mtdb = Regex.Match(line, @"TOG_max\[.*?\]\s*=\s*([0-9]+);");
			            if (mtdb.Groups.Count > 1)
				            int.TryParse(mtdb.Groups[1].Value, out tog);
		            }
					var nd = new DetectorModel
					{
						Naam = dd, Rijstrook = 1
					};
		            if (tdb != -1) nd.TDB = tdb;
		            if (tdh != -1) nd.TDH = tdh;
		            if (tbg != -1) nd.TBG = tbg;
		            if (tog != -1) nd.TOG = tog;
		            if (nd.Naam.StartsWith("k"))
		            {
			            nd.Type = DetectorTypeEnum.Knop;
			            nd.Aanvraag = DetectorAanvraagTypeEnum.RoodGeel;
			            nd.Verlengen = DetectorVerlengenTypeEnum.Geen;
		            }
		            else if (Regex.IsMatch(nd.Naam, "1[a-z]?$"))
		            {
			            nd.Type = DetectorTypeEnum.Kop;
			            nd.Aanvraag = DetectorAanvraagTypeEnum.RnietTRG;
			            nd.AanvraagDirect = true;
			            nd.Verlengen = DetectorVerlengenTypeEnum.Kopmax;
		            }
		            else if (Regex.IsMatch(nd.Naam, "2[a-z]?$"))
		            {
			            nd.Type = DetectorTypeEnum.Lang;
			            nd.Aanvraag = DetectorAanvraagTypeEnum.RnietTRG;
			            nd.Verlengen = DetectorVerlengenTypeEnum.MK2;
		            }
		            else if (Regex.IsMatch(nd.Naam, "3[a-z]?$"))
		            {
			            nd.Type = DetectorTypeEnum.Verweg;
			            nd.Aanvraag = DetectorAanvraagTypeEnum.RnietTRG;
			            nd.Verlengen = DetectorVerlengenTypeEnum.MK2;
		            }
		            else 
		            {
			            nd.Type = DetectorTypeEnum.Overig;
			            nd.Aanvraag = DetectorAanvraagTypeEnum.Uit;
		            }
		            foreach (var fc in outcome.Fasen)
		            {
			            if (fc.Naam.Length < nd.Naam.Length &&
			                Regex.IsMatch(nd.Naam, $@"^k?{fc.Naam}"))
			            {
							fc.Detectoren.Add(nd);
			            }
		            }
	            }
            }
            return outcome;
        }
    }
}
