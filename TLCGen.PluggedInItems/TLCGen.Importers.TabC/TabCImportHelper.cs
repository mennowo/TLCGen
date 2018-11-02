using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        public static TabCImportHelperOutcome GetNewData_old(string[] lines)
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
                if (Regex.IsMatch(line, @"^\s*TO_max\["))
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
	                (Regex.IsMatch(line, @"^\s*D_code\[") || Regex.IsMatch(line, @"^\s*TBG_max\[")))
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
                    // Detector: try to find, otherwise add new
                    DetectorModel nd = null;
                    foreach (var fc in outcome.Fasen)
                    {
                        var bd = fc.Detectoren.FirstOrDefault(x => x.Naam == dd);
                        if(bd != null)
                        {
                            nd = bd;
                            break;
                        }
                    }
                    if (nd == null)
                    {
                        nd = new DetectorModel()
                        {
                            Naam = dd,
                            Rijstrook = 1
                        };
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
		            if (!nd.TDB.HasValue && tdb != -1) nd.TDB = tdb;
		            if (!nd.TDH.HasValue && tdh != -1) nd.TDH = tdh;
		            if (!nd.TBG.HasValue && tbg != -1) nd.TBG = tbg;
		            if (!nd.TOG.HasValue && tog != -1) nd.TOG = tog;
	            }
            }
            return outcome;
        }

        public enum TabCType
        {
            OTTO, TPA, ATB, FICK, HUIJSKES, GC, UNKNOWN
        }

        private static Regex ReTypeOTTO = new Regex(@"\s*/\*\s+Aangemaakt\smet:\s+OTTO.*", RegexOptions.Compiled);
        private static Regex ReTypeTPA = new Regex(@"\s*CCOLGEN:\s+V[0-9].*", RegexOptions.Compiled);
        private static Regex ReTypeATB = new Regex(@"\s*\*\s+Generator\s*:\s*Advanced\s+Traffic\s+Builder.*", RegexOptions.Compiled);
        private static Regex ReTypeFICK = new Regex(@".*DE_type.*", RegexOptions.Compiled);
        private static Regex ReTypeHUIJSKES = new Regex(@".*\*\s+Huijskes.*", RegexOptions.Compiled);
        private static Regex ReTypeGC = new Regex(@"\s*#define\sTO\(van_fc,\snaar_fc,\swaarde\).*", RegexOptions.Compiled);

        public static TabCImportHelperOutcome GetNewData(string[] lines, bool newReg)
        {
            var dz = MessageBoxResult.None;

            var outcome = new TabCImportHelperOutcome();

            if (lines.Count() <= 1)
            {
                return null;
            }

            var t = TabCType.UNKNOWN;
            if (lines.Any(x => ReTypeOTTO.IsMatch(x))) t = TabCType.OTTO;
            if (t == TabCType.UNKNOWN && lines.Any(x => ReTypeTPA.IsMatch(x))) t = TabCType.TPA;
            if (t == TabCType.UNKNOWN && lines.Any(x => ReTypeATB.IsMatch(x))) t = TabCType.ATB;
            if (t == TabCType.UNKNOWN && lines.Any(x => ReTypeFICK.IsMatch(x))) t = TabCType.FICK;
            if (t == TabCType.UNKNOWN && lines.Any(x => ReTypeHUIJSKES.IsMatch(x))) t = TabCType.HUIJSKES;
            if (t == TabCType.UNKNOWN && lines.Any(x => ReTypeGC.IsMatch(x))) t = TabCType.GC;

            var dlg = new ChooseTabTypeWindow();
            dlg.TabType = t;
            dlg.ImportInExisting = !newReg;
            var res = dlg.ShowDialog();
            t = dlg.TabType;
            if (res == false || t == TabCType.UNKNOWN) return null;
            var importD = dlg.ImportDetectoren;
            var importT = dlg.ImportTijden;

            // get phases
            Regex fasenRegex = null;
            switch (t)
            {
                case TabCType.OTTO:
                    fasenRegex = new Regex(@"^\s*TO_max\s*\[\s*(?<name>fc[0-9]+).*", RegexOptions.Compiled);
                    break;
                case TabCType.TPA:
                case TabCType.FICK:
                case TabCType.HUIJSKES:
                    fasenRegex = new Regex(@"^\s*FC_code\s*\[\s*(?<name>fc[0-9]+).*", RegexOptions.Compiled);
                    break;
                case TabCType.ATB:
                case TabCType.GC:
                    fasenRegex = new Regex(@"^\s*FC\s*\(\s*(?<name>fc[0-9]+).*", RegexOptions.Compiled);
                    break;
            }
            foreach (var l in lines)
            {
                var m = fasenRegex.Match(l);
                if (m.Success)
                {
                    var name = m.Groups["name"].Value.Replace("fc", "");
                    if (!outcome.Fasen.Any(x => x.Naam == name))
                    {
                        var fcm = new FaseCyclusModel
                        {
                            Naam = name.ToLower()
                        };
                        fcm.Type = Settings.Utilities.FaseCyclusUtilities.GetFaseTypeFromNaam(fcm.Naam);
                        if (!importT)
                        {
                            DefaultsProvider.Default.SetDefaultsOnModel(fcm, fcm.Type.ToString());
                        }
                        outcome.Fasen.Add(fcm);
                    }
                }
            }

            if (!newReg) return outcome;

            // import conflicts
            Regex confRegex = null;
            switch (t)
            {
                case TabCType.OTTO:
                case TabCType.TPA:
                case TabCType.FICK:
                case TabCType.HUIJSKES:
                    confRegex = new Regex(@"^\s*TO_max\s*\[\s*fc(?<fc1>[0-9]+)\s*\]\s*\[\s*fc(?<fc2>[0-9]+)\s*\]\s*=\s*(?<conf>([0-9]+|FK|GK|GKL)).*");
                    break;
                case TabCType.ATB:
                case TabCType.GC:
                    confRegex = new Regex(@"^\s*TO\(\s*fc(?<fc1>[0-9]+)\s*,\s*fc(?<fc2>[0-9]+)\s*,\s*(?<conf>([0-9]+|FK|GK|GKL)).*");
                    break;
            }
            foreach (var l in lines)
            {
                var m = confRegex.Match(l);
                if (m.Success)
                {
                    var fc1 = m.Groups["fc1"].Value;
                    var fc2 = m.Groups["fc2"].Value;
                    var conf = m.Groups["conf"].Value;
                    if (int.TryParse(conf, out var iconf) && !outcome.Conflicten.Any(x => x.FaseVan == fc1 && x.FaseNaar == fc2))
                    {
                        outcome.Conflicten.Add(new ConflictModel
                        {
                            FaseVan = fc1,
                            FaseNaar =fc2,
                            Waarde = iconf
                        });
                    }
                }
            }

            // get detectors
            if (importD)
            {
                Regex detectorenRegex = null;
                switch (t)
                {
                    case TabCType.TPA:
                    case TabCType.FICK:
                    case TabCType.HUIJSKES:
                        detectorenRegex = new Regex(@"^\s*D_code\s*\[\s*(?<name>d[a-zA-Z0-9_]+).*", RegexOptions.Compiled);
                        break;
                    case TabCType.ATB:
                        detectorenRegex = new Regex(@"^\s*DP\s*\(\s*(?<name>d[a-zA-Z0-9_]+).*", RegexOptions.Compiled);
                        break;
                    case TabCType.GC:
                        detectorenRegex = new Regex(@"^\s*D\s*\(\s*(?<name>d[a-zA-Z0-9_]+).*", RegexOptions.Compiled);
                        break;
                }
                if (detectorenRegex != null)
                {
                    foreach (var l in lines)
                    {
                        var m = detectorenRegex.Match(l);
                        if (m.Success)
                        {
                            var name = m.Groups["name"].Value.Replace("d", "");
                            if (!outcome.Detectoren.Any(x => x.Naam == name.ToLower()))
                            {
                                outcome.Detectoren.Add(new DetectorModel
                                {
                                    Naam = name.ToLower()
                                });
                            }
                        }
                    }

                    // assign detectors to signalgroups
                    foreach (var d in outcome.Detectoren)
                    {
                        foreach (var fc in outcome.Fasen)
                        {
                            if (fc.Naam.Length < d.Naam.Length &&
                                Regex.IsMatch(d.Naam, $@"^k?{fc.Naam}"))
                            {
                                fc.Detectoren.Add(d);
                            }
                        }
                    }
                }
            }

            if (importT)
            {
                
                Regex dsetRegex1 = null;
                Regex dsetRegex2 = null;
                Regex fcsetRegex1 = null;
                var setREs = new Tuple<setRegex, Regex>[(int)setRegex.setMax];
                setREs[(int)setRegex.tdh] = new Tuple<setRegex, Regex>(setRegex.tdh, new Regex(@"\s*TDH_max\s*\[\s*(?<name>d[a-zA-Z0-9_]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));
                setREs[(int)setRegex.tdb] = new Tuple<setRegex, Regex>(setRegex.tdb, new Regex(@"\s*TDB_max\s*\[\s*(?<name>d[a-zA-Z0-9_]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));
                setREs[(int)setRegex.tog] = new Tuple<setRegex, Regex>(setRegex.tog, new Regex(@"\s*TOG_max\s*\[\s*(?<name>d[a-zA-Z0-9_]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));
                setREs[(int)setRegex.tbg] = new Tuple<setRegex, Regex>(setRegex.tbg, new Regex(@"\s*TBG_max\s*\[\s*(?<name>d[a-zA-Z0-9_]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));
                setREs[(int)setRegex.tfl] = new Tuple<setRegex, Regex>(setRegex.tfl, new Regex(@"\s*TFL_max\s*\[\s*(?<name>d[a-zA-Z0-9_]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));
                setREs[(int)setRegex.cfl] = new Tuple<setRegex, Regex>(setRegex.cfl, new Regex(@"\s*CFL_max\s*\[\s*(?<name>d[a-zA-Z0-9_]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));
                setREs[(int)setRegex.trg] = new Tuple<setRegex, Regex>(setRegex.trg, new Regex(@"\s*TRG_max\s*\[\s*(?<name>fc[0-9]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));
                setREs[(int)setRegex.trgmin] = new Tuple<setRegex, Regex>(setRegex.trgmin, new Regex(@"\s*TRG_min\s*\[\s*(?<name>fc[0-9]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));
                setREs[(int)setRegex.tgg] = new Tuple<setRegex, Regex>(setRegex.tgg, new Regex(@"\s*TGG_max\s*\[\s*(?<name>fc[0-9]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));
                setREs[(int)setRegex.tggmin] = new Tuple<setRegex, Regex>(setRegex.tggmin, new Regex(@"\s*TGG_min\s*\[\s*(?<name>fc[0-9]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));
                setREs[(int)setRegex.tfg] = new Tuple<setRegex, Regex>(setRegex.tfg, new Regex(@"\s*TFG_max\s*\[\s*(?<name>fc[0-9]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));
                setREs[(int)setRegex.tgl] = new Tuple<setRegex, Regex>(setRegex.tgl, new Regex(@"\s*TGL_max\s*\[\s*(?<name>fc[0-9]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));
                setREs[(int)setRegex.tggl] = new Tuple<setRegex, Regex>(setRegex.tggl, new Regex(@"\s*TGGL_max\s*\[\s*(?<name>fc[0-9]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));
                setREs[(int)setRegex.tglmin] = new Tuple<setRegex, Regex>(setRegex.tglmin, new Regex(@"\s*TGL_min\s*\[\s*(?<name>fc[0-9]+)\s*]\s*=\s*(?<val>[0-9NG]+)\s*;", RegexOptions.Compiled));

                switch (t)
                {
                    case TabCType.TPA:
                        dsetRegex2 = new Regex(@"IS_type\s*\[\s*(?<name>d[a-zA-Z0-9_]+)\s*]\s*=\s*(?<type>[a-zA-Z_]+).*", RegexOptions.Compiled);
                        break;
                    case TabCType.HUIJSKES:
                        dsetRegex2 = new Regex(@"IS_type\s*\[\s*(?<name>d[a-zA-Z0-9_]+)\s*]\s*=\s*(?<type>[a-zA-Z_]+).*", RegexOptions.Compiled);
                        break;
                    case TabCType.FICK:
                        dsetRegex2 = new Regex(@"DE_type\s*\[\s*(?<name>d[a-zA-Z0-9_]+)\s*]\s*=\s*(?<type>[a-zA-Z_]+).*", RegexOptions.Compiled);
                        break;
                    case TabCType.ATB:
                        dsetRegex1 = new Regex(@"^\s*DP\s*\(\s*(?<name>d[a-zA-Z0-9_]+)\s*,\s*""[a-zA-Z0-9_]+""\s*,\s*(?<tdb>[0-9NG]+)\s*,\s*(?<tdh>[0-9NG]+)\s*,\s*(?<tbg>[0-9NG]+)\s*,\s*(?<tog>[0-9NG]+).*", RegexOptions.Compiled);
                        dsetRegex2 = new Regex(@"^\s*DPT\s*\(\s*(?<name>d[a-zA-Z0-9_]+)\s*,\s*(?<type>[a-zA-Z_]+).*", RegexOptions.Compiled);
                        fcsetRegex1 = new Regex(@"^\s*FC\s*\(\s*(?<name>fc[0-9]+)\s*,\s*""[a-zA-Z0-9_]+""\s*,\s*(?<trg>[0-9NG]+)\s*,\s*(?<tgg>[0-9NG]+)\s*,\s*(?<tfg>[0-9NG]+)\s*,\s*(?<tggl>[0-9NG]+)\s*,\s*(?<tgl>[0-9NG]+).*");
                        break;
                    case TabCType.GC:
                        dsetRegex1 = new Regex(@"^\s*D\s*\(\s*(?<name>d[a-zA-Z0-9_]+)\s*,\s*""[a-zA-Z0-9_]+""\s*,\s*""[a-zA-Z0-9_]+""\s*,\s*(?<tdb>[0-9NG]+)\s*,\s*(?<tdh>[0-9NG]+)\s*,\s*(?<tbg>[0-9NG]+)\s*,\s*(?<tog>[0-9NG]+)\s*,\s*(?<type>[a-zA-Z_]+).*", RegexOptions.Compiled);
                        dsetRegex2 = new Regex(@"^\s*DFL\s*\(\s*(?<name>d[a-zA-Z0-9_]+)\s*,\s*(?<tfl>[0-9NG]+)\s*,\s*(?<cfl>[0-9NG]+).*", RegexOptions.Compiled);
                        fcsetRegex1 = new Regex(@"^\s*FC\s*\(\s*(?<name>fc[0-9]+)\s*,\s*""[a-zA-Z0-9_]+""\s*,\s*""[a-zA-Z0-9_]+""\s*,\s*(?<tgg>[0-9NG]+)\s*,\s*(?<tfg>[0-9NG]+)\s*,\s*(?<tgl>[0-9NG]+)\s*,\s*(?<trg>[0-9NG]+).*");
                        break;
                }
                foreach (var l in lines)
                {
                    if(t == TabCType.TPA || t == TabCType.FICK || t == TabCType.HUIJSKES)
                    {
                        foreach(var re in setREs)
                        {
                            var m = re.Item2.Match(l);
                            if (m.Success)
                            {
                                var named = m.Groups["name"].Value.Replace("d", "");
                                var namefc = m.Groups["name"].Value.Replace("fc", "");
                                var d = outcome.Detectoren.FirstOrDefault(x => x.Naam == named.ToLower());
                                var fc = outcome.Fasen.FirstOrDefault(x => x.Naam == namefc.ToLower());
                                switch (re.Item1)
                                {
                                    case setRegex.tdh: d.TDH = new int?(int.Parse(m.Groups["val"].Value)); break;
                                    case setRegex.tdb: d.TDB = new int?(int.Parse(m.Groups["val"].Value)); break;
                                    case setRegex.tog: d.TOG = new int?(int.Parse(m.Groups["val"].Value)); break;
                                    case setRegex.tbg: d.TBG = new int?(int.Parse(m.Groups["val"].Value)); break;
                                    case setRegex.tfl: d.TFL = new int?(int.Parse(m.Groups["val"].Value)); break;
                                    case setRegex.cfl: d.CFL = new int?(int.Parse(m.Groups["val"].Value)); break;
                                    case setRegex.trg: fc.TRG = int.Parse(m.Groups["val"].Value); break;
                                    case setRegex.tgg: fc.TGG = int.Parse(m.Groups["val"].Value); break;
                                    case setRegex.tfg: fc.TFG = int.Parse(m.Groups["val"].Value); break;
                                    case setRegex.tggl: fc.TGL_min = int.Parse(m.Groups["val"].Value); break;
                                    case setRegex.tgl: fc.TGL = int.Parse(m.Groups["val"].Value); break;
                                    case setRegex.trgmin: fc.TRG_min = int.Parse(m.Groups["val"].Value); break;
                                    case setRegex.tggmin: fc.TGG_min = int.Parse(m.Groups["val"].Value); break;
                                    case setRegex.tglmin: fc.TGL_min = int.Parse(m.Groups["val"].Value); break;
                                }
                            }
                        }
                    }
                    if (fcsetRegex1 != null)
                    {
                        var m = fcsetRegex1.Match(l);
                        if (m.Success)
                        {
                            var name = m.Groups["name"].Value.Replace("fc", "");
                            var fc = outcome.Fasen.FirstOrDefault(x => x.Naam == name.ToLower());
                            if (fc != null)
                            {
                                fc.TGG = int.Parse(m.Groups["tgg"].Value);
                                fc.TFG = int.Parse(m.Groups["tfg"].Value);
                                fc.TGL = int.Parse(m.Groups["tgl"].Value);
                                fc.TRG = int.Parse(m.Groups["trg"].Value);
                                if (t == TabCType.ATB)
                                {
                                    fc.TGL_min = int.Parse(m.Groups["tggl"].Value);
                                }
                            }
                        }
                    }
                    if (dsetRegex1 != null)
                    {
                        var m = dsetRegex1.Match(l);
                        if (m.Success)
                        {
                            var name = m.Groups["name"].Value.Replace("d", "");
                            var d = outcome.Detectoren.FirstOrDefault(x => x.Naam == name.ToLower());
                            if (d != null)
                            {
                                d.TDB = m.Groups["tdb"].Value == "NG" ? null : new int?(int.Parse(m.Groups["tdb"].Value));
                                d.TDH = m.Groups["tdh"].Value == "NG" ? null : new int?(int.Parse(m.Groups["tdh"].Value));
                                d.TOG = m.Groups["tog"].Value == "NG" ? null : new int?(int.Parse(m.Groups["tog"].Value));
                                d.TBG = m.Groups["tbg"].Value == "NG" ? null : new int?(int.Parse(m.Groups["tbg"].Value));
                                if (t == TabCType.GC)
                                {
                                    d.Type = GetDetType(m.Groups["type"].Value);
                                }
                            }
                        }
                    }
                    if (dsetRegex2 != null)
                    {
                        var m = dsetRegex2.Match(l);
                        if (m.Success)
                        {
                            var name = m.Groups["name"].Value.Replace("d", "");
                            var d = outcome.Detectoren.FirstOrDefault(x => x.Naam == name.ToLower());
                            if (d != null)
                            {
                                switch (t)
                                {
                                    case TabCType.TPA:
                                    case TabCType.HUIJSKES:
                                    case TabCType.FICK:
                                    case TabCType.ATB:
                                        d.Type = GetDetType(m.Groups["type"].Value);
                                        break;
                                    case TabCType.GC:
                                        d.TFL = int.Parse(m.Groups["tfl"].Value);
                                        d.CFL = int.Parse(m.Groups["cfl"].Value);
                                        break;
                                }
                            }
                        }
                    }
                }
                foreach (var fc in outcome.Fasen)
                {
                    foreach (var d in fc.Detectoren)
                    {
                        switch (d.Type)
                        {
                            case DetectorTypeEnum.Kop:
                                d.Aanvraag = DetectorAanvraagTypeEnum.RnietTRG;
                                d.Verlengen = DetectorVerlengenTypeEnum.Kopmax;
                                break;
                            case DetectorTypeEnum.Lang:
                                d.Aanvraag = DetectorAanvraagTypeEnum.RnietTRG;
                                d.Verlengen = DetectorVerlengenTypeEnum.MK2;
                                break;
                            case DetectorTypeEnum.Verweg:
                                d.Aanvraag = DetectorAanvraagTypeEnum.Uit;
                                d.Verlengen = DetectorVerlengenTypeEnum.MK2;
                                break;
                            case DetectorTypeEnum.Knop:
                                d.Aanvraag = DetectorAanvraagTypeEnum.RoodGeel;
                                d.Verlengen = DetectorVerlengenTypeEnum.Geen;
                                break;
                        }
                    }
                }
            }
            else
            {
                foreach (var fc in outcome.Fasen.Where(x => x.Detectoren.Any()))
                {
                    switch (fc.Type)
                    {
                        case FaseTypeEnum.Auto:
                            fc.Detectoren.ForEach(x => x.Type = DetectorTypeEnum.Lang);
                            fc.Detectoren.First().Type = DetectorTypeEnum.Kop;
                            break;
                        case FaseTypeEnum.Fiets:
                            fc.Detectoren.ForEach(x => x.Type = DetectorTypeEnum.Knop);
                            fc.Detectoren.First().Type = DetectorTypeEnum.Kop;
                            break;
                        case FaseTypeEnum.Voetganger:
                            fc.Detectoren.ForEach(x => x.Type = DetectorTypeEnum.Knop);
                            break;
                        case FaseTypeEnum.OV:
                            fc.Detectoren.ForEach(x => x.Type = DetectorTypeEnum.Lang);
                            fc.Detectoren.First().Type = DetectorTypeEnum.Kop;
                            break;
                    }
                    foreach(var d in fc.Detectoren)
                    {
                        TLCGen.Settings.DefaultsProvider.Default.SetDefaultsOnModel(d, fc.Type.ToString(), d.Type.ToString());
                    }
                }
            }

            return outcome;
        }

        enum setRegex
        {
            tdh, tdb, tog, tbg, tfl, cfl, trg, trgmin, tgg, tggmin, tfg, tggl, tglmin, tgl, setMax
        }

        private static DetectorTypeEnum GetDetType(string st)
        {
            switch (st)
            {
                case "KOPLUS":
                case "DKOP_type":
                    return DetectorTypeEnum.Kop;
                case "LANGE_LUS":
                case "DLNG_type":
                    return DetectorTypeEnum.Lang;
                case "VERWEGLUS":
                case "DVER_type":
                    return DetectorTypeEnum.Verweg;
                case "DRUKKNOP":
                case "DK_type":
                    return DetectorTypeEnum.Knop;
            }
            return DetectorTypeEnum.Overig;
        }
    }
}
