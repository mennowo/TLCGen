using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using TLCGen.Dependencies.Providers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.Importers.TabC
{
    public class TabCImportHelperOutcome
    {
        public bool Intergroen { get; set; }
        public bool Garantie { get; set; }
        public List<DetectorModel> Detectoren { get; set; }
        public List<FaseCyclusModel> Fasen { get; set; }
        public List<ConflictModel> Conflicten { get; set; }
        public List<VoorstartModel> Voorstarten { get; set; }
        public List<MeeaanvraagModel> MeeAanvragen { get; set; }
        public List<GelijkstartModel> Gelijkstarten { get; set; }
        public List<LateReleaseModel> LateReleases { get; set; }
        public List<NaloopModel> Nalopen { get; set; }

        public string KruisingNaam { get; set; }
        public string KruisingStraat1 { get; set; }
        public string KruisingStraat2 { get; set; }

        public TabCImportHelperOutcome()
        {
            KruisingNaam = KruisingStraat1 = KruisingStraat2 = null;
            Intergroen = false;
            Fasen = new List<FaseCyclusModel>();
            Conflicten = new List<ConflictModel>();
	        Detectoren = new List<DetectorModel>();
            Gelijkstarten = new List<GelijkstartModel>();
            Voorstarten = new List<VoorstartModel>();
            LateReleases = new List<LateReleaseModel>();
            MeeAanvragen = new List<MeeaanvraagModel>();
            Nalopen = new List<NaloopModel>();
        }

    }

    public static class TabCImportHelper
    {
        public enum TabCType
        {
            OTTO, TPA, ATB, FICK, HUIJSKES, GC, UNKNOWN
        }

        private static Regex ReComment = new Regex(@"^\s*/\*.*", RegexOptions.Compiled);
        private static Regex ReIntergreen = new Regex(@"\s*TIG_max\s?\[.*", RegexOptions.Compiled);
        private static Regex ReGarantie = new Regex(@"\s*(TIG|TO)_min\s?\[.*", RegexOptions.Compiled);
        private static Regex ReTypeOTTO = new Regex(@"\s*/\*\s+Aangemaakt\smet:\s+OTTO.*", RegexOptions.Compiled);
        private static Regex ReTypeTPA = new Regex(@"\s*CCOLGEN:\s+V[0-9].*", RegexOptions.Compiled);
        private static Regex ReTypeATB = new Regex(@"\s*\*\s+Generator\s*:\s*Advanced\s+Traffic\s+Builder.*", RegexOptions.Compiled);
        private static Regex ReTypeFICK = new Regex(@".*DE_type.*", RegexOptions.Compiled);
        private static Regex ReTypeHUIJSKES = new Regex(@".*\*\s+Huijskes.*", RegexOptions.Compiled);
        private static Regex ReTypeGC = new Regex(@"\s*#define\sTO\(van_fc,\snaar_fc,\swaarde\).*", RegexOptions.Compiled);

        public static TabCImportHelperOutcome GetNewData(string[] lines, bool newReg)
        {
            var outcome = new TabCImportHelperOutcome();

            if (lines.Count() <= 1)
            {
                return null;
            }

            TabCType tabCType = TabCType.UNKNOWN;
            if (lines.Any(x => ReTypeOTTO.IsMatch(x))) tabCType = TabCType.OTTO;
            if (tabCType == TabCType.UNKNOWN && lines.Any(x => ReTypeTPA.IsMatch(x))) tabCType = TabCType.TPA;
            if (tabCType == TabCType.UNKNOWN && lines.Any(x => ReTypeATB.IsMatch(x))) tabCType = TabCType.ATB;
            if (tabCType == TabCType.UNKNOWN && lines.Any(x => ReTypeFICK.IsMatch(x))) tabCType = TabCType.FICK;
            if (tabCType == TabCType.UNKNOWN && lines.Any(x => ReTypeHUIJSKES.IsMatch(x))) tabCType = TabCType.HUIJSKES;
            if (tabCType == TabCType.UNKNOWN && lines.Any(x => ReTypeGC.IsMatch(x))) tabCType = TabCType.GC;
            var intergroen = lines.Any(x => ReIntergreen.IsMatch(x));
            var garantie = lines.Any(x => ReGarantie.IsMatch(x));
            outcome.Intergroen = intergroen;
            outcome.Garantie = garantie;

            var importD = false;
            var importT = false;
            var importG = false;
            var importNalopen = false;
            var importDeelconf = "";
            if (TLCGenDialogProvider.Default.ShowDialogs)
            {
                var dlg = new ChooseTabTypeWindow
                {
                    Intergroen = intergroen,
                    ImportGarantie = garantie,
                    HasIntergroen = intergroen,
                    TabType = tabCType,
                    ImportInExisting = !newReg
                };
                var res = dlg.ShowDialog();
                tabCType = dlg.TabType;
                if (res == false || tabCType == TabCType.UNKNOWN) return null;
                importD = dlg.ImportDetectoren;
                importT = dlg.ImportTijden;
                outcome.Garantie = importG = dlg.ImportGarantie;
                importDeelconf = dlg.ImportDeelconflicten;
                importNalopen = dlg.ImportNalopen;
            }
            else
            {
                importD = true;
                importT = true;
            }

            // get meta data
            if (tabCType == TabCType.OTTO)
            {
                var kNaamRegex = new Regex(@"^\s*/\*\sKruispunt\snaam:\s*(.*)\*/", RegexOptions.Compiled);
                var kLocatieRegex = new Regex(@"^\s*/\*\sKruispunt\slocatie:\s*(.*)\*/", RegexOptions.Compiled);
                foreach (var l in lines)
                {
                    var naamM = kNaamRegex.Match(l);
                    if (naamM.Success && naamM.Groups.Count > 1)
                    {
                        outcome.KruisingNaam = naamM.Groups[1].Value;
                    }
                    else
                    {
                        var locatieM = kLocatieRegex.Match(l);
                        if (locatieM.Success)
                        {
                            if (locatieM.Success && locatieM.Groups.Count > 1)
                            {
                                var streets = locatieM.Groups[1].Value.Split('-');
                                if (streets.Length >= 1)
                                {
                                    var str1 = Regex.Replace(streets[0], @"^\s*", "");
                                    outcome.KruisingStraat1 = Regex.Replace(str1, @"\s*$", "");
                                }
                                if (streets.Length >= 2)
                                {
                                    var str2 = Regex.Replace(streets[1], @"^\s*", "");
                                    outcome.KruisingStraat2 = Regex.Replace(str2, @"\s*$", "");
                                }
                            }
                        }
                    }
                }
            }

            // get phases
            Regex fasenRegex = null;
            switch (tabCType)
            {
                case TabCType.OTTO:
                    if (intergroen) fasenRegex = new Regex(@"^\s*TIG_max\s*\[\s*(?<name>fc[0-9]+).*", RegexOptions.Compiled);
                    else fasenRegex = new Regex(@"^\s*TO_max\s*\[\s*(?<name>fc[0-9]+).*", RegexOptions.Compiled);
                    
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
                    if (outcome.Fasen.All(x => x.Naam != name))
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

            // import conflicts
            Regex confRegex = null;
            Regex gconfRegex = null;
            Regex dconfRegex = null;
            Regex geelRegex = null;
            switch (tabCType)
            {
                case TabCType.OTTO:
                case TabCType.TPA:
                case TabCType.FICK:
                case TabCType.HUIJSKES:
                    if (intergroen)
                    {
                        dconfRegex = new Regex(@"^\s*\/\*\s*TIG_max\s*\[\s*fc(?<fc1>[0-9]+)\s*\]\s*\[\s*fc(?<fc2>[0-9]+)\s*\]\s*=\s*(?<conf>([0-9]+|FK|GK|GKL));\s*=\s*deelconflict\s*\*\/.*");
                        gconfRegex = new Regex(@"^\s*TIG_min\s*\[\s*fc(?<fc1>[0-9]+)\s*\]\s*\[\s*fc(?<fc2>[0-9]+)\s*\]\s*=\s*(?<conf>([0-9]+|FK|GK|GKL)).*");
                        confRegex = new Regex(@"^\s*TIG_max\s*\[\s*fc(?<fc1>[0-9]+)\s*\]\s*\[\s*fc(?<fc2>[0-9]+)\s*\]\s*=\s*(?<conf>([0-9]+|FK|GK|GKL)).*");
                        geelRegex = new Regex(@"^\s*TGL_max\s*\[\s*fc(?<fc1>[0-9]+)\s*\]\s?=\s?(?<geel>[0-9]+).*");
                    }
                    else
                    {
                        dconfRegex = new Regex(@"^\s*\/\*\s*TO_max\s*\[\s*fc(?<fc1>[0-9]+)\s*\]\s*\[\s*fc(?<fc2>[0-9]+)\s*\]\s*=\s*(?<conf>([0-9]+|FK|GK|GKL));\s*=\s*deelconflict\s*\*\/.*");
                        gconfRegex = new Regex(@"^\s*TO_min\s*\[\s*fc(?<fc1>[0-9]+)\s*\]\s*\[\s*fc(?<fc2>[0-9]+)\s*\]\s*=\s*(?<conf>([0-9]+|FK|GK|GKL)).*");
                        confRegex = new Regex(@"^\s*TO_max\s*\[\s*fc(?<fc1>[0-9]+)\s*\]\s*\[\s*fc(?<fc2>[0-9]+)\s*\]\s*=\s*(?<conf>([0-9]+|FK|GK|GKL)).*");
                    }
                    break;
                case TabCType.ATB:
                case TabCType.GC:
                    confRegex = new Regex(@"^\s*TO\(\s*fc(?<fc1>[0-9]+)\s*,\s*fc(?<fc2>[0-9]+)\s*,\s*(?<conf>([0-9]+|FK|GK|GKL)).*");
                    gconfRegex = new Regex(@"^\s*TO\(\s*fc(?<fc1>[0-9]+)\s*,\s*fc(?<fc2>[0-9]+)\s*,\s*([0-9]+|FK|GK|GKL),\s*(?<conf>([0-9]+)).*");
                    break;
            }
            var clines = lines.Where(x => !ReComment.IsMatch(x));
            if (!string.IsNullOrWhiteSpace(importDeelconf))
            {
                clines = lines;
            }

            foreach (var l in clines)
            {
                var m = confRegex.Match(l);
                if (m.Success)
                {
                    var fc1 = m.Groups["fc1"].Value;
                    var fc2 = m.Groups["fc2"].Value;
                    var conf = m.Groups["conf"].Value;
                    if (int.TryParse(conf, out var iconf))
                    {
                        var cconf = outcome.Conflicten.FirstOrDefault(x => x.FaseVan == fc1 && x.FaseNaar == fc2);
                        if (cconf != null)
                        {
                            cconf.Waarde = iconf;
                        }
                        else
                        {
                            outcome.Conflicten.Add(new ConflictModel
                            {
                                FaseVan = fc1,
                                FaseNaar = fc2,
                                Waarde = iconf
                            });
                        }
                    }
                }

                if (importG)
                {
                    m = gconfRegex.Match(l);
                    if (m.Success)
                    {
                        var fc1 = m.Groups["fc1"].Value;
                        var fc2 = m.Groups["fc2"].Value;
                        var conf = m.Groups["conf"].Value;
                        if (int.TryParse(conf, out var iconf))
                        {
                            var cconf = outcome.Conflicten.FirstOrDefault(x => x.FaseVan == fc1 && x.FaseNaar == fc2);
                            if (cconf != null)
                            {
                                cconf.GarantieWaarde = iconf;
                            }
                            else
                            {
                                outcome.Conflicten.Add(new ConflictModel
                                {
                                    FaseVan = fc1,
                                    FaseNaar = fc2,
                                    GarantieWaarde = iconf
                                });
                            }
                        }
                    }
                }

                if (intergroen)
                {
                    m = geelRegex.Match(l);
                    if (m.Success)
                    {
                        var fc1 = m.Groups["fc1"].Value;
                        var geel = m.Groups["geel"].Value;
                        if (int.TryParse(geel, out var igeel))
                        {
                            var fc = outcome.Fasen.FirstOrDefault(x => x.Naam == fc1);
                            if (fc != null)
                            {
                                fc.TGL_min = igeel;
                                fc.TGL = igeel;
                            }
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(importDeelconf))
                {
                    m = dconfRegex.Match(l);
                    if (m.Success)
                    {
                        var fc1 = m.Groups["fc1"].Value;
                        var fc2 = m.Groups["fc2"].Value;
                        var conf = m.Groups["conf"].Value;
                        if (int.TryParse(conf, out var iconf) &&
                            !outcome.Conflicten.Any(x => x.FaseVan == fc1 && x.FaseNaar == fc2))
                        {
                            var nfc1 = outcome.Fasen.FirstOrDefault(x => x.Naam == fc1);
                            var nfc2 = outcome.Fasen.FirstOrDefault(x => x.Naam == fc2);

                            var skip = false;
                            if (int.TryParse(nfc1.Naam, out var iFc1) && int.TryParse(nfc2.Naam, out var iFc2))
                            {
                                if (iFc1 > 60 && iFc1 < 80 || iFc2 > 60 && iFc2 < 80) skip = true;
                            }

                            if (!skip)
                            {
                                // deelconflicten auto verkeer: gelijkstart
                                if ((nfc1.Type == FaseTypeEnum.Auto || nfc1.Type == FaseTypeEnum.OV) &&
                                    (nfc2.Type == FaseTypeEnum.Auto || nfc2.Type == FaseTypeEnum.OV))
                                {
                                    var gs = outcome.Gelijkstarten.FirstOrDefault(x =>
                                        x.FaseNaar == fc1 && x.FaseVan == fc2);
                                    if (gs != null)
                                    {
                                        gs.GelijkstartOntruimingstijdFaseNaar = iconf;
                                    }
                                    else
                                    {
                                        outcome.Gelijkstarten.Add(new GelijkstartModel
                                        {
                                            FaseVan = fc1,
                                            FaseNaar = fc2,
                                            DeelConflict = true,
                                            GelijkstartOntruimingstijdFaseVan = iconf
                                        });
                                    }
                                }
                                // deelconflicten auto <> langzaam
                                else if ((nfc1.Type != FaseTypeEnum.Auto && nfc1.Type != FaseTypeEnum.OV) &&
                                         (nfc2.Type == FaseTypeEnum.Auto || nfc2.Type == FaseTypeEnum.OV) ||
                                         (nfc2.Type != FaseTypeEnum.Auto && nfc2.Type != FaseTypeEnum.OV) &&
                                         (nfc1.Type == FaseTypeEnum.Auto || nfc1.Type == FaseTypeEnum.OV))
                                {
                                    switch (importDeelconf)
                                    {
                                        case "Voorstarten":
                                            var vs = outcome.Voorstarten.FirstOrDefault(x =>
                                                x.FaseNaar == fc1 && x.FaseVan == fc2 ||
                                                x.FaseVan == fc1 && x.FaseNaar == fc2);
                                            // langzaam > auto
                                            if ((nfc1.Type != FaseTypeEnum.Auto && nfc1.Type != FaseTypeEnum.OV) &&
                                                (nfc2.Type == FaseTypeEnum.Auto || nfc2.Type == FaseTypeEnum.OV))
                                            {
                                                if (vs != null)
                                                {
                                                    vs.VoorstartTijd = iconf;
                                                }
                                                else
                                                {
                                                    outcome.Voorstarten.Add(new VoorstartModel
                                                    {
                                                        FaseVan = fc1,
                                                        FaseNaar = fc2,
                                                        VoorstartTijd = iconf,
                                                        VoorstartOntruimingstijd = 0
                                                    });
                                                    outcome.MeeAanvragen.Add(new MeeaanvraagModel
                                                    {
                                                        FaseVan = fc2,
                                                        FaseNaar = fc1,
                                                        Type = MeeaanvraagTypeEnum.RoodVoorAanvraag,
                                                        AanUit = AltijdAanUitEnum.SchAan
                                                    });
                                                }
                                            }
                                            // auto > langzaam
                                            else
                                            {
                                                if (vs != null)
                                                {
                                                    vs.VoorstartOntruimingstijd = iconf;
                                                }
                                                else
                                                {
                                                    outcome.Voorstarten.Add(new VoorstartModel
                                                    {
                                                        FaseVan = fc2,
                                                        FaseNaar = fc1,
                                                        VoorstartTijd = 0,
                                                        VoorstartOntruimingstijd = iconf
                                                    });
                                                    outcome.MeeAanvragen.Add(new MeeaanvraagModel
                                                    {
                                                        FaseVan = fc1,
                                                        FaseNaar = fc2,
                                                        Type = MeeaanvraagTypeEnum.RoodVoorAanvraag,
                                                        AanUit = AltijdAanUitEnum.SchAan
                                                    });
                                                }
                                            }

                                            break;
                                        case "Late release":
                                            var lr = outcome.LateReleases.FirstOrDefault(x =>
                                                x.FaseNaar == fc1 && x.FaseVan == fc2 ||
                                                x.FaseVan == fc1 && x.FaseNaar == fc2);
                                            // langzaam > auto
                                            if ((nfc1.Type != FaseTypeEnum.Auto && nfc1.Type != FaseTypeEnum.OV) &&
                                                (nfc2.Type == FaseTypeEnum.Auto || nfc2.Type == FaseTypeEnum.OV))
                                            {
                                                if (lr != null)
                                                {
                                                    lr.LateReleaseTijd = iconf;
                                                }
                                                else
                                                {
                                                    outcome.LateReleases.Add(new LateReleaseModel
                                                    {
                                                        FaseVan = fc1,
                                                        FaseNaar = fc2,
                                                        LateReleaseTijd = iconf,
                                                        LateReleaseOntruimingstijd = 0
                                                    });
                                                }
                                            }
                                            // auto > langzaam
                                            else
                                            {
                                                if (lr != null)
                                                {
                                                    lr.LateReleaseOntruimingstijd = iconf;
                                                }
                                                else
                                                {
                                                    outcome.LateReleases.Add(new LateReleaseModel
                                                    {
                                                        FaseVan = fc2,
                                                        FaseNaar = fc1,
                                                        LateReleaseTijd = 0,
                                                        LateReleaseOntruimingstijd = iconf
                                                    });
                                                }
                                            }

                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (importNalopen)
            {
                foreach (var fc in outcome.Fasen)
                {
                    if (int.TryParse(fc.Naam, out var iFc))
                    {
                        // Auto
                        if (iFc % 100 > 0 && iFc % 100 <= 12)
                        {
                            foreach (var fc2 in outcome.Fasen)
                            {
                                if (int.TryParse(fc2.Naam, out var iFc2))
                                {
                                    if (iFc2 > iFc && (iFc2 - iFc) == 60)
                                    {
                                        var nm = new NaloopModel
                                        {
                                            DetectieAfhankelijk = fc.Detectoren.Any(x => x.Type == DetectorTypeEnum.Kop),
                                            Detectoren = fc.Detectoren.Where(x => x.Type == DetectorTypeEnum.Kop).Select(x => new NaloopDetectorModel{ Detector = x.Naam, Type = NaloopDetectorTypeEnum.Hiaat }).ToList(),
                                            FaseVan = fc.Naam, 
                                            FaseNaar = fc2.Naam, 
                                            InrijdenTijdensGroen = false, 
                                            MaximaleVoorstart = null, 
                                            Type = NaloopTypeEnum.EindeGroen
                                        };
                                        SetNaloopTijden(nm);
                                        outcome.Nalopen.Add(nm);
                                    }
                                }
                            }
                        }
                        // Voetganger
                        if (iFc % 100 > 30 && iFc % 100 < 40)
                        {
                            foreach (var fc2 in outcome.Fasen)
                            {
                                if (int.TryParse(fc2.Naam, out var iFc2))
                                {
                                    if (iFc % 2 == 1 && iFc2 % 2 == 0 && iFc2 - iFc == 1 || 
                                        iFc % 2 == 0 && iFc2 % 2 == 1 && iFc2 - iFc == -1)
                                    {
                                        var nm = new NaloopModel
                                        {
                                            DetectieAfhankelijk = fc.Detectoren.Any(x => x.Type == DetectorTypeEnum.KnopBuiten),
                                            Detectoren = fc.Detectoren.Where(x => x.Type == DetectorTypeEnum.KnopBuiten).Select(x => new NaloopDetectorModel{ Detector = x.Naam, Type = NaloopDetectorTypeEnum.Hiaat }).ToList(),
                                            FaseVan = fc.Naam, 
                                            FaseNaar = fc2.Naam, 
                                            InrijdenTijdensGroen = false, 
                                            MaximaleVoorstart = null, 
                                            Type = NaloopTypeEnum.StartGroen
                                        };
                                        SetNaloopTijden(nm);
                                        outcome.Nalopen.Add(nm);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // for importing into existing controllers, do not import detectors and settings
            if (!newReg) return outcome;

            // get detectors
            if (importD)
            {
                Regex detectorenRegex = null;
                switch (tabCType)
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
                    foreach (var l in lines.Where(x => !ReComment.IsMatch(x)))
                    {
                        var m = detectorenRegex.Match(l);
                        if (m.Success)
                        {
                            var name = m.Groups["name"].Value.ToLower().Replace("d", "");
                            if (!outcome.Detectoren.Any(x => x.Naam == name))
                            {
                                outcome.Detectoren.Add(new DetectorModel
                                {
                                    Naam = name.ToLower(),
                                    Rijstrook = 1
                                });
                            }
                        }
                    }

                    // assign detectors based on d[a-zA-Z]+## in the sg name
                    var assigned = new List<DetectorModel>();
                    foreach (var d in outcome.Detectoren)
                    {
                        foreach (var fc in outcome.Fasen)
                        {
                            if (fc.Naam.Length < d.Naam.Length &&
                                Regex.IsMatch(d.Naam, $@"^[a-zA-Z]*{fc.Naam}"))
                            {
                                fc.Detectoren.Add(d);
                                assigned.Add(d);
                                break;
                            }
                        }
                    }
                    foreach(var d in assigned)
                    {
                        outcome.Detectoren.Remove(d);
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

                switch (tabCType)
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
                foreach (var l in lines.Where(x => !ReComment.IsMatch(x)))
                {
                    if(tabCType == TabCType.TPA || tabCType == TabCType.FICK || tabCType == TabCType.HUIJSKES)
                    {
                        foreach(var re in setREs)
                        {
                            var m = re.Item2.Match(l);
                            if (m.Success)
                            {
                                var named = m.Groups["name"].Value.Replace("d", "");
                                var namefc = m.Groups["name"].Value.Replace("fc", "");
                                var d = outcome.Detectoren.FirstOrDefault(x => x.Naam == named.ToLower());
                                if (d == null)
                                {
                                    d = outcome.Fasen.SelectMany(x => x.Detectoren).FirstOrDefault(x => x.Naam == named.ToLower());
                                    if (d == null)
                                    {
                                        continue;
                                    }
                                }
                                var fc = outcome.Fasen.FirstOrDefault(x => x.Naam == namefc.ToLower());
                                switch (re.Item1)
                                {
                                    case setRegex.tdh: d.TDH = m.Groups["val"].Value == "NG" ? null : new int?(int.Parse(m.Groups["val"].Value)); break;
                                    case setRegex.tdb: d.TDB = m.Groups["val"].Value == "NG" ? null : new int?(int.Parse(m.Groups["val"].Value)); break;
                                    case setRegex.tog: d.TOG = m.Groups["val"].Value == "NG" ? null : new int?(int.Parse(m.Groups["val"].Value)); break;
                                    case setRegex.tbg: d.TBG = m.Groups["val"].Value == "NG" ? null : new int?(int.Parse(m.Groups["val"].Value)); break;
                                    case setRegex.tfl: d.TFL = m.Groups["val"].Value == "NG" ? null : new int?(int.Parse(m.Groups["val"].Value)); break;
                                    case setRegex.cfl: d.CFL = m.Groups["val"].Value == "NG" ? null : new int?(int.Parse(m.Groups["val"].Value)); break;
                                    case setRegex.trg: fc.TRG = int.Parse(m.Groups["val"].Value); break;
                                    case setRegex.tgg: fc.TGG = int.Parse(m.Groups["val"].Value); break;
                                    case setRegex.tfg: fc.TFG = int.Parse(m.Groups["val"].Value); break;
                                    case setRegex.tggl:
                                        if(!intergroen)
                                            fc.TGL_min = int.Parse(m.Groups["val"].Value);
                                        break;
                                    case setRegex.tgl:
                                        if (!intergroen)
                                            fc.TGL = int.Parse(m.Groups["val"].Value);
                                        else 
                                            fc.TGL = int.Parse(m.Groups["val"].Value) > fc.TGL ? int.Parse(m.Groups["val"].Value) : fc.TGL;
                                        break;
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
                                if (tabCType == TabCType.ATB)
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
                                if (tabCType == TabCType.GC)
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
                                switch (tabCType)
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
                        DefaultsProvider.Default.SetDefaultsOnModel(d, fc.Type.ToString(), d.Type.ToString());
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

        private static void SetNaloopTijden(NaloopModel nm)
        {
            var _naloop = nm;
            _naloop.Tijden = new List<NaloopTijdModel>();
            switch (_naloop.Type)
            {
                case NaloopTypeEnum.StartGroen:
                    if(_naloop.VasteNaloop)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.StartGroen });
                    }
                    if (_naloop.DetectieAfhankelijk)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.StartGroenDetectie });
                    }
                    break;
                case NaloopTypeEnum.EindeGroen:
                    if (_naloop.VasteNaloop)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroen });
                    }
                    if (_naloop.DetectieAfhankelijk)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroenDetectie });
                    }
                    if (_naloop.VasteNaloop)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeGroen });
                    }
                    if (_naloop.DetectieAfhankelijk)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeGroenDetectie });
                    }
                    break;
                case NaloopTypeEnum.CyclischVerlengGroen:
                    if (_naloop.VasteNaloop)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroen });
                    }
                    if (_naloop.DetectieAfhankelijk)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroenDetectie });
                    }
                    if (_naloop.VasteNaloop)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeVerlengGroen });
                    }
                    if (_naloop.DetectieAfhankelijk)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeVerlengGroenDetectie });
                    }
                    break;
            }
        }
    }
}
