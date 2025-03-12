using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class AlgemeenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private string _hplact;
        private string _mperiod;
        private List<string> _madets;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schtoon7s;
        private CCOLGeneratorCodeStringSettingModel _ussegm;
        private CCOLGeneratorCodeStringSettingModel _usML;
        private CCOLGeneratorCodeStringSettingModel _prmxx;
        private CCOLGeneratorCodeStringSettingModel _prmyy;
        private CCOLGeneratorCodeStringSettingModel _prmzz;
        private CCOLGeneratorCodeStringSettingModel _prmfb;
        private CCOLGeneratorCodeStringSettingModel _tcycl;
        private CCOLGeneratorCodeStringSettingModel _schcycl;
        private CCOLGeneratorCodeStringSettingModel _schcycl_reset;
        private CCOLGeneratorCodeStringSettingModel _mlcycl;
        private CCOLGeneratorCodeStringSettingModel _hmad;
        private CCOLGeneratorCodeStringSettingModel _usincontrol;
        private CCOLGeneratorCodeStringSettingModel _usnocontrol;
        private CCOLGeneratorCodeStringSettingModel _schtypeuswt;
#pragma warning restore 0649


        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            _myElements.Add(new CCOLElement(_mperiod, null, null, CCOLElementTypeEnum.GeheugenElement, "Onthouden actieve periode"));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmfb}", c.Data.Fasebewaking, CCOLElementTimeTypeEnum.TS_type, _prmfb));

            // Onthouden drukknop meldingen
            var madets = c.InterSignaalGroep.Meeaanvragen.Where(x => x.DetectieAfhankelijk).SelectMany(x => x.Detectoren).Select(x => x.MeeaanvraagDetector).ToList();
            if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc)
            {
                var groenSyncData = GroenSyncDataModel.ConvertSyncFuncToRealFunc(c);
                var (_, _, twoWayPedestrians) = GroenSyncDataModel.OrderSyncs(c, groenSyncData);
                foreach (var (m1, _, gelijkstart) in twoWayPedestrians)
                {
                    if (gelijkstart) continue;

                    var fc1 = c.Fasen.FirstOrDefault(x => x.Naam == m1.FaseVan);
                    var fc2 = c.Fasen.FirstOrDefault(x => x.Naam == m1.FaseNaar);
                    if (fc1 == null || fc2 == null) continue;

                    var mdr1A = fc1.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.KnopBuiten);
                    var mdr1B = fc1.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.KnopBinnen);
                    var mdr2A = fc2.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.KnopBuiten);
                    var mdr2B = fc2.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.KnopBinnen);

                    if (mdr1A != null) madets.Add(mdr1A.Naam);
                    if (mdr1B != null) madets.Add(mdr1B.Naam);
                    if (mdr2A != null) madets.Add(mdr2A.Naam);
                    if (mdr2B != null) madets.Add(mdr2B.Naam);
                }
            }

            _madets = madets.Distinct().ToList();
            _madets.Sort();
            foreach (var dm in _madets.Distinct())
            {
                var elem = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hmad}{dm}", _hmad, dm);
                if (_myElements.All(x => x.Naam != elem.Naam))
                {
                    _myElements.Add(elem);
                }
            }

            // Segment display elements
            foreach (var item in c.Data.SegmentenDisplayBitmapData)
            {
                item.BitmapData.Naam = _ussegm + item.Naam;
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_ussegm}{item.Naam}", _ussegm, item.BitmapData));
            }
            if (c.Data.SegmentDisplayType == SegmentDisplayTypeEnum.DrieCijferDisplay)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schtoon7s}", 1, CCOLElementTimeTypeEnum.SCH_type, _schtoon7s));
            }

            // Module display elements
            if (c.Data.UitgangPerModule)
            {
                foreach (var item in c.Data.ModulenDisplayBitmapData)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{item.Naam.Replace("ML", _usML.Setting)}", _usML, item.BitmapData, item.Naam.Replace("ML", _usML.Setting)));
                }
            }

            // Inputs
            foreach (var i in c.Ingangen)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(i.Naam, CCOLElementTypeEnum.Ingang, i, i.Omschrijving, null, null));
            }

            // Versie beheer
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmxx}", c.Data.HuidigeVersieMajor, CCOLElementTimeTypeEnum.None, _prmxx));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmyy}", c.Data.HuidigeVersieMinor, CCOLElementTimeTypeEnum.None, _prmyy));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmzz}", c.Data.HuidigeVersieRevision, CCOLElementTimeTypeEnum.None, _prmzz));

            // Waitsignalering
            if (c.GetAllDetectors(x => x.Wachtlicht).Any())
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schtypeuswt}", c.Data.AansturingWaitsignalen == AansturingWaitsignalenEnum.DrukknopGebruik ? 1 : 0, CCOLElementTimeTypeEnum.None, _schtypeuswt));   
            }

            // OVM
            if (c.Data.ToevoegenOVM)
            {
                foreach (var fc in c.Fasen.Where(x => x.Type == FaseTypeEnum.Auto && !(x.Naam.Length == 3 && x.Naam.StartsWith("9"))))
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"ovmextragroen_{fc.Naam}", 0, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter, "",
                            PrioCodeGeneratorHelper.CAT_Optimaliseren, PrioCodeGeneratorHelper.SUBCAT_OpenbaarVervoer));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"ovmmindergroen_{fc.Naam}", 0, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter, "",
                            PrioCodeGeneratorHelper.CAT_Optimaliseren, PrioCodeGeneratorHelper.SUBCAT_OpenbaarVervoer));
                }
            }

            // Logging TFB max
            if (c.Data.PrmLoggingTfbMax)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        "tfbfc", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "",
                        PrioCodeGeneratorHelper.CAT_TestenLoggen, PrioCodeGeneratorHelper.SUBCAT_Fasebewaking));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        "tfbmax", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "",
                        PrioCodeGeneratorHelper.CAT_TestenLoggen, PrioCodeGeneratorHelper.SUBCAT_Fasebewaking));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        "tfbtijd", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "",
                        PrioCodeGeneratorHelper.CAT_TestenLoggen, PrioCodeGeneratorHelper.SUBCAT_Fasebewaking));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        "tfbdat", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "",
                        PrioCodeGeneratorHelper.CAT_TestenLoggen, PrioCodeGeneratorHelper.SUBCAT_Fasebewaking));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        "tfbjaar", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "",
                        PrioCodeGeneratorHelper.CAT_TestenLoggen, PrioCodeGeneratorHelper.SUBCAT_Fasebewaking));
            }

            // Cyclustijdmeting
            if (c.Data.GenererenCyclustijdMeting && !c.Data.MultiModuleReeksen)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tcycl}", 999, CCOLElementTimeTypeEnum.TS_type, _tcycl));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schcycl}", 0, CCOLElementTimeTypeEnum.TE_type, _schcycl));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schcycl_reset}", 0, CCOLElementTimeTypeEnum.TE_type, _schcycl_reset));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mlcycl}", _mlcycl));
            }
            
            // Verklikken applicatie daadwerkelijk de TLC aanstuurt
            if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usincontrol}", _usincontrol, c.Data.InControlBitmapData));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usnocontrol}", _usnocontrol, c.Data.NoControlBitmapData));
            }
        }

        public override bool HasCCOLElements() => true;

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.SysHDefines => new []{20},
                CCOLCodeTypeEnum.SysHBeforeUserDefines => new []{10},
                CCOLCodeTypeEnum.RegCIncludes => new []{40},
                CCOLCodeTypeEnum.RegCAanvragen => new []{91},
                CCOLCodeTypeEnum.RegCInitApplication => new []{40},
                CCOLCodeTypeEnum.RegCPostApplication => new []{30},
                CCOLCodeTypeEnum.RegCSystemApplication => new []{200},
                CCOLCodeTypeEnum.RegCPostSystemApplication => new []{20},
                _ => null
            };
        }

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCInitApplication:
                    return c.Data.GeenDetectorGedragInAutomaatOmgeving 
                        ? new List<CCOLLocalVariable> { new CCOLLocalVariable("int", "i", "", "(defined AUTOMAAT || defined AUTOMAAT_TEST)") } 
                        : base.GetFunctionLocalVariables(c, type);
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    return c.Data.PrmLoggingTfbMax 
                        ? new List<CCOLLocalVariable> { new CCOLLocalVariable("int", "fc") } 
                        : base.GetFunctionLocalVariables(c, type);
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.SysHDefines:
                    if (c.Data.PracticeOmgeving)
                    {
                        sb.AppendLine($"#ifdef PRACTICE_TEST");
                        sb.AppendLine($"#define AMSTERDAM_PC");
                        sb.AppendLine($"#define XTND_DIC");
                        sb.AppendLine($"#define CCOL_IS_SPECIAL");
                        sb.AppendLine($"#endif");
                        sb.AppendLine($"#ifndef AUTOMAAT");
                        sb.AppendLine($"#define AMSTERDAM_PC");
                        sb.AppendLine($"#endif");
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.SysHBeforeUserDefines:
                    if (!string.IsNullOrWhiteSpace(c.Data.CCOLParserPassword))
                    {
                        sb.AppendLine($"{ts}#define PASSWORD \"{c.Data.CCOLParserPassword}\"");
                    }
                    if (c.Data.PracticeOmgeving)
                    {
                        sb.AppendLine("#if defined AMSTERDAM_PC");
                    }
                    else
                    {
                        sb.AppendLine($"#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined PRACTICE_TEST");
                    }
                    sb.AppendLine($"{ts}#define TESTOMGEVING");
                    sb.AppendLine($"#endif");

                    /* Toevoegingen AMSTERDAM_PC (vinkje Practice omgeving in TLCGen) */
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCIncludes:
                    sb.AppendLine($"#ifdef MIRMON");
                    sb.AppendLine($"{ts}#include \"MirakelMonitor.h\"");
                    sb.AppendLine($"#endif /* MIRMON */");

                    if (c.Data.PracticeOmgeving)
                    { 
                        sb.AppendLine("#ifdef XTND_DIC");
                        sb.AppendLine($"{ts}#include \"xtnd_dic.c\"");
                        sb.AppendLine("#endif");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCAanvragen:
                    if (_madets.Any())
                    {
                        sb.AppendLine($"{ts}/* Bewaar meldingen van detectie (bv. voor het zetten van een meeaanvraag) */");
                        foreach (var mad in _madets)
                        {
                            var fc = c.Fasen.FirstOrDefault(x => x.Detectoren.Any(x2 => x2.Naam == mad));
                            if (fc == null) continue;
                            sb.AppendLine($"{ts}IH[{_hpf}{_hmad}{mad}] = G[{_fcpf}{fc.Naam}] && !SG[{_fcpf}{fc.Naam}] ? FALSE : IH[{_hpf}{_hmad}{mad}] || D[{_dpf}{mad}] && A[{_fcpf}{fc.Naam}];");
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCInitApplication:
                    if (c.Data.GenererenDuurtestCode)
                    {
                        sb.AppendLine($"{ts}/* TESTOMGEVING */");
                        sb.AppendLine($"{ts}/* ============ */");
                        sb.AppendLine($"{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST && !defined VISSIM)");
                        sb.AppendLine($"{ts}{ts}if (!SAPPLPROG)");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}#ifdef DUURTEST");
                        sb.AppendLine($"{ts}{ts}{ts}//stuffkey(F5KEY);");
                        sb.AppendLine($"{ts}{ts}{ts}stuffkey(ALTF9KEY);");
                        sb.AppendLine($"{ts}{ts}{ts}stuffkey(F2KEY);");
                        if (c.Data.CCOLVersie == CCOLVersieEnum.CCOL110)
                        {
                            sb.AppendLine($"{ts}{ts}{ts}stuffkey(ALTCTRLF3KEY);");
                        }
                        else if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL120)
                        {
                            sb.AppendLine($"{ts}{ts}{ts}stuffkey(CTRLALTF3KEY);");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}{ts}{ts}stuffkey(CTRLF3KEY);");
                        }
                        sb.AppendLine($"{ts}{ts}{ts}stuffkey(F4KEY);");
                        sb.AppendLine($"{ts}{ts}{ts}//stuffkey(F10KEY);");
                        sb.AppendLine($"{ts}{ts}{ts}//stuffkey(F11KEY);");
                        sb.AppendLine($"{ts}{ts}{ts}CFB_max = 0; /* maximum aantal herstarts na fasebewaking */");
                        sb.AppendLine($"{ts}{ts}{ts}#ifndef NO_VLOG");
                        sb.AppendLine($"{ts}{ts}{ts}{ts}MONTYPE[MONTYPE_DATI] = 0;");
                        sb.AppendLine($"{ts}{ts}{ts}{ts}LOGTYPE[LOGTYPE_DATI] = 0;");
                        sb.AppendLine($"{ts}{ts}{ts}#endif");
                        sb.AppendLine($"{ts}{ts}#endif");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine($"{ts}#endif");
                    }
                    if (c.Data.MirakelMonitor)
                    {
                        sb.AppendLine($"#ifdef MIRMON");
                        sb.AppendLine($"{ts}if (SAPPLPROG) MirakelMonitor_init(SYSTEM);");
                        sb.AppendLine($"#endif /* MIRMON */");
                    }
                    if (c.Data.GeenDetectorGedragInAutomaatOmgeving)
                    {
                        sb.AppendLine("#if defined AUTOMAAT || defined AUTOMAAT_TEST");
                        sb.AppendLine($"{ts}/* verwijderen BG, OG en FL tijden in ITSAPP */");
                        sb.AppendLine($"{ts}for (i = 0; i < DPMAX; ++i) {{");
                        sb.AppendLine($"{ts}{ts}TBG_max[i]=NG;");
                        sb.AppendLine($"{ts}{ts}TOG_max[i]=NG;");
                        sb.AppendLine($"#if !defined NO_DDFLUTTER");
                        sb.AppendLine($"{ts}{ts}TFL_max[i]=NG;");
                        sb.AppendLine($"{ts}{ts}CFL_max[i]=NG;");
                        sb.AppendLine("#endif");
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine("#endif");

                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPreApplication:
                    if (c.Data.GenererenDuurtestCode)
                    {
                        sb.AppendLine($"{ts}{ts}#ifdef DUURTEST");
                        sb.AppendLine($"{ts}{ts}for (int i = 0; i < FCMAX; ++i)");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}if (TFB_timer[i] + 3 > PRM[{_prmpf}{_prmfb}])");
                        sb.AppendLine($"{ts}{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}{ts}stuffkey(F5KEY);");
                        sb.AppendLine($"{ts}{ts}{ts}}}");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine($"{ts}{ts}#endif");
                    }
                    if (c.Data.MirakelMonitor)
                    {
                        sb.AppendLine($"#ifdef MIRMON");
                        sb.AppendLine($"{ts}MirakelMonitor();");
                        sb.AppendLine($"#endif /* MIRMON */");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPostApplication:
                    if (c.Data.GenererenDuurtestCode)
                    {
                        sb.AppendLine($"{ts}/* TESTOMGEVING */");
                        sb.AppendLine($"{ts}/* ============ */");
                        sb.AppendLine($"{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST && !defined VISSIM)");
                        sb.AppendLine($"{ts}{ts}if (TS &&");
                        sb.AppendLine($"{ts}{ts}    (CIF_KLOK[CIF_JAAR] == 2099) &&");
                        sb.AppendLine($"{ts}{ts}    (CIF_KLOK[CIF_MAAND] == 1) &&");
                        sb.AppendLine($"{ts}{ts}    (CIF_KLOK[CIF_DAG] == 1) &&");
                        sb.AppendLine($"{ts}{ts}    (CIF_KLOK[CIF_UUR] == 1) &&");
                        sb.AppendLine($"{ts}{ts}    (CIF_KLOK[CIF_MINUUT] == 1) &&");
                        sb.AppendLine($"{ts}{ts}    (CIF_KLOK[CIF_SECONDE] == 1))");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}stuffkey(F3KEY);");
                        sb.AppendLine($"{ts}{ts}{ts}stuffkey(F5KEY);");
                        sb.AppendLine($"{ts}{ts}{ts}stuffkey(F4KEY);");
                        sb.AppendLine($"{ts}{ts}{ts}//stuffkey(F10KEY); ");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine($"{ts}#endif");
                    }
                    if (c.Data.ToevoegenOVM)
                    {
                        if (sb.Length > 0) sb.AppendLine();

                        sb.AppendLine($"{ts}/* OVM Rotterdam: extra/minder groen */");
                        foreach (var fc in c.Fasen.Where(x => x.Type == FaseTypeEnum.Auto && !(x.Naam.Length == 3 && x.Naam.StartsWith("9"))))
                        {
                            sb.AppendLine($"{ts}if (TVG_max[{_fcpf}{fc.Naam}] > -1) TVG_max[{_fcpf}{fc.Naam}] += PRM[{_prmpf}ovmextragroen_{fc.Naam}];");
                            sb.AppendLine($"{ts}if (TVG_max[{_fcpf}{fc.Naam}] > -1) TVG_max[{_fcpf}{fc.Naam}] -= PRM[{_prmpf}ovmmindergroen_{fc.Naam}];");
                        }
                        sb.AppendLine();
                    }

                    if (c.Data.GenererenCyclustijdMeting && !c.Data.MultiModuleReeksen)
                    {
                        if (sb.Length > 0) sb.AppendLine();

                        sb.AppendLine($"{ts}CyclustijdMeting({_tpf}{_tcycl}, {_schpf}{_schcycl}, SML && (ML == ML1), {_schpf}{_schcycl_reset}, {_mpf}{_mlcycl});");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSystemApplication:
                    if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                    {
                        sb.AppendLine($"{ts}#ifdef AUTOMAAT");
                        sb.AppendLine($"{ts}{ts}/* verklikken of applicatie daadwerkelijk de TLC aanstuurt */");
                        sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_usincontrol}] = (CIF_WPS[CIF_PROG_CONTROL] == CIF_CONTROL_INCONTROL) ? TRUE : FALSE;");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}{ts}/* verklikken of applicatie niet in staat is te regelen */");
                        sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_usnocontrol}] = (CIF_GPS[CIF_PROG_CONTROL] != CIF_CONTROL_ONGEDEF) ? TRUE : FALSE;");
                        if (c.Data.GeenControlGeenFileBasedVLOG)
                        {
                            sb.AppendLine();
                            sb.AppendLine($"{ts}{ts}/* uitschakelen vlog indien applicatie niet in control is */");
                            sb.AppendLine($"{ts}{ts}LOGPRM[LOGPRM_VLOGMODE] &= ~(BIT5);");
                            sb.AppendLine($"{ts}{ts}LOGPRM[LOGPRM_VLOGMODE] |= CIF_GUS[{_uspf}{_usincontrol}] ? 0 : BIT5;");
                        }

                        sb.AppendLine($"{ts}#endif");
                    }

                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    if (c.Data.SegmentDisplayType == SegmentDisplayTypeEnum.EnkelDisplay)
                    {
                        if (!c.Data.MultiModuleReeksen)
                        {
                            sb.AppendLine($"{ts}SegmentSturing(ML+1, {_uspf}{_ussegm}1, {_uspf}{_ussegm}2, {_uspf}{_ussegm}3, {_uspf}{_ussegm}4, {_uspf}{_ussegm}5, {_uspf}{_ussegm}6, {_uspf}{_ussegm}7);");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}{ts}/* Let op: end enkel segmenten display niet is compatible met multi module molens */");
                            sb.AppendLine($"CIF_GUS[{_uspf}{_ussegm}4] = TRUE;");
                        }
                    }
                    else
                    {

                        if (c.Data.SegmentDisplayType == SegmentDisplayTypeEnum.DrieCijferDisplay)
                        {
                            if (c.HalfstarData.IsHalfstar)
                            {
                                sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}] && SCH[{_schpf}{_schtoon7s}])");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}/* Uitsturen segmenten verklikking signaalplantijd */");
                                SingleModuleCode("TX_timer", sb, ts);
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"{ts}else if (SCH[{_schpf}{_schtoon7s}])");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}/* Uitsturen segmenten verklikking module regelen */");
                                if (!c.Data.MultiModuleReeksen)
                                {
                                    SingleModuleCode("ML + 1", sb, ts);
                                }
                                else
                                {
                                    MultiModuleCode(c, sb);
                                }
                                sb.AppendLine($"{ts}}}");
                            }
                            else
                            {
                                sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtoon7s}])");
                                sb.AppendLine($"{ts}{{");
                                // single module mill
                                if (!c.Data.MultiModuleReeksen)
                                {
                                    SingleModuleCode("ML + 1", sb, ts);
                                }
                                // multi module mill, with at least one mill with modules with signalgroups
                                else if (c.MultiModuleMolens.Any(x => x.Modules.Any(x2 => x2.Fasen.Any())))
                                {
                                    MultiModuleCode(c, sb);
                                }
                                sb.AppendLine($"{ts}}}");
                            }
                        }
                    }

                    sb.AppendLine();
                    if (c.Data.UitgangPerModule && c.Data.ModulenDisplayBitmapData.Any())
                    {
                        sb.AppendLine($"{ts}/* Uitsturen actieve module */");
                        foreach(var m in c.Data.ModulenDisplayBitmapData)
                        {
                            var mNaam = Regex.Replace(m.Naam, @"ML[A-E]+", "ML");
                            var mReeks = Regex.Replace(m.Naam, @"ML([A-E]?)[0-9]+", "ML$1");
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{m.Naam.Replace("ML", _usML.Setting)}] = {mReeks} == {mNaam};");
                        }
                    }

                     if (c.Data.PrmLoggingTfbMax)
                    {
                        if (c.Data.UitgangPerModule && c.Data.ModulenDisplayBitmapData.Any())
                            sb.AppendLine();
                        sb.AppendLine($"{ts}/* Onthouden hoogste tfb waarde + tijdstip */");
                        sb.AppendLine($"{ts}for (fc = 0; fc < FC_MAX; ++fc)");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}if (TFB_timer[fc]>PRM[{_prmpf}tfbmax])");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || (defined VISSIM)");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf( 92, 0, \"Hoogste TFB waarde\");");
                        sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf( 92, 1, \"------------------\");");
                        sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf( 92, 2, \"Fc %s TFB:%4d sec\", FC_code[fc], TFB_timer[fc]);");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf( 92, 3, \"Tijd %02d\", (CIF_KLOK[CIF_UUR]));");
                        sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf( 99, 3,     \":%02d\", (CIF_KLOK[CIF_MINUUT]));");
                        sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(102, 3,     \":%02d\", (CIF_KLOK[CIF_SECONDE]));");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf( 92, 4, \"d.d. %02d\", (CIF_KLOK[CIF_DAG]));");
                        sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf( 99, 4,     \"-%02d\", (CIF_KLOK[CIF_MAAND]));");
                        sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(102, 4,     \"-%04d\", (CIF_KLOK[CIF_JAAR]));");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}{ts}{ts}#endif");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}tfbfc]   = fc;");
                        sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}tfbmax]  = TFB_timer[fc];");
                        sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}tfbtijd] = CIF_KLOK[CIF_UUR]*100+CIF_KLOK[CIF_MINUUT];");
                        sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}tfbdat]  = CIF_KLOK[CIF_DAG]*100+CIF_KLOK[CIF_MAAND];");
                        sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}tfbjaar] = CIF_KLOK[CIF_JAAR];");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}{ts}{ts}CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine($"{ts}}}");
                    }
                    break;
            }

            return sb.ToString();
        }

        private void SingleModuleCode(string arg1, StringBuilder sb, string ts)
        {
            sb.AppendLine($"{ts}{ts}SegmentSturingDrie({arg1},");
            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}a1, {_uspf}{_ussegm}a2, {_uspf}{_ussegm}a3, {_uspf}{_ussegm}a4, {_uspf}{_ussegm}a5, {_uspf}{_ussegm}a6, {_uspf}{_ussegm}a7,");
            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}b1, {_uspf}{_ussegm}b2, {_uspf}{_ussegm}b3, {_uspf}{_ussegm}b4, {_uspf}{_ussegm}b5, {_uspf}{_ussegm}b6, {_uspf}{_ussegm}b7,");
            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}c1, {_uspf}{_ussegm}c2, {_uspf}{_ussegm}c3, {_uspf}{_ussegm}c4, {_uspf}{_ussegm}c5, {_uspf}{_ussegm}c6, {_uspf}{_ussegm}c7);");
        }

        private void MultiModuleCode(ControllerModel c, StringBuilder sb)
        {
            var reeksen = c.MultiModuleMolens.Where(x => x.Modules.Any(x2 => x2.Fasen.Any())).OrderBy(x => x.Reeks).ToList();

            // acutally single mill
            if (reeksen.Count > 0)
            {
                sb.AppendLine($"SegmentSturing({reeksen[0].Reeks} + 1, {_uspf}{_ussegm}a1, {_uspf}{_ussegm}a2, {_uspf}{_ussegm}a3, {_uspf}{_ussegm}a4, {_uspf}{_ussegm}a5, {_uspf}{_ussegm}a6, {_uspf}{_ussegm}a7);");
            }

            // two mills: dash between two ML numbers
            if (reeksen.Count == 2)
            {
                sb.AppendLine($"CIF_GUS[{_uspf}{_ussegm}b4] = TRUE;");
                sb.AppendLine($"SegmentSturing({reeksen[1].Reeks} + 1, {_uspf}{_ussegm}c1, {_uspf}{_ussegm}c2, {_uspf}{_ussegm}c3, {_uspf}{_ussegm}c4, {_uspf}{_ussegm}c5, {_uspf}{_ussegm}c6, {_uspf}{_ussegm}c7);");
            }

            // three or more mills: three ML numbers
            else if (reeksen.Count > 2)
            {
                sb.AppendLine($"SegmentSturing({reeksen[1].Reeks} + 1, {_uspf}{_ussegm}b1, {_uspf}{_ussegm}b2, {_uspf}{_ussegm}b3, {_uspf}{_ussegm}b4, {_uspf}{_ussegm}b5, {_uspf}{_ussegm}b6, {_uspf}{_ussegm}b7);");
                sb.AppendLine($"SegmentSturing({reeksen[2].Reeks} + 1, {_uspf}{_ussegm}c1, {_uspf}{_ussegm}c2, {_uspf}{_ussegm}c3, {_uspf}{_ussegm}c4, {_uspf}{_ussegm}c5, {_uspf}{_ussegm}c6, {_uspf}{_ussegm}c7);");
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");
            _mperiod = CCOLGeneratorSettingsProvider.Default.GetElementName("mperiod");

            return base.SetSettings(settings);
        }
    }
}
