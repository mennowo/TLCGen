﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class TraffickCompatibleCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private string _schmv;
        private string _schwg;
        private string _schaltg; 
        private string _prmaltp;
        private string _prmaltb;
        private string _prmaltg;
        private string _cvchd;
        private string _tnlfg;
        private string _tnlfgd;
        private string _tnleg;
        private string _tnlegd;
        private string _tnlsgd;
        private string _trealil;
        private string _hlos;
        private string _hnla;
        private string _tvs;
        private string _tfo;
        private string _schma;
        private string _schhardmv;
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schtraffick2tlcgen;
        private CCOLGeneratorCodeStringSettingModel _tarmvt;
#pragma warning restore 0649
        
        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if (!c.Data.TraffickCompatible) return;
            
            _myElements.Add(
                CCOLGeneratorSettingsProvider.Default.CreateElement(
                    $"{_schtraffick2tlcgen}", 1, CCOLElementTimeTypeEnum.None, _schtraffick2tlcgen));
            
            foreach (var armfc in c.Kruispunt.FasenMetKruispuntArmen.Where(x => x.HasKruispuntArmVolgTijd))
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tarmvt}{armfc.FaseCyclus}", armfc.KruispuntArmVolgTijd, CCOLElementTimeTypeEnum.TE_type, _tarmvt, armfc.FaseCyclus));
            }
        }

        public override bool HasCCOLElements() => true;

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCTop => new []{130},
                CCOLCodeTypeEnum.RegCIncludes => new []{130},
                CCOLCodeTypeEnum.RegCPreApplication => new []{130},
                CCOLCodeTypeEnum.RegCAanvragen => new []{130},
                CCOLCodeTypeEnum.RegCKlokPerioden => new []{130},
                CCOLCodeTypeEnum.RegCBepaalRealisatieTijden => new []{30},
                CCOLCodeTypeEnum.RegCVerlenggroen => new []{80},
                CCOLCodeTypeEnum.RegCMaxgroen => new []{80},
                CCOLCodeTypeEnum.RegCWachtgroen => new []{40},
                CCOLCodeTypeEnum.RegCMeetkriterium => new []{130},
                CCOLCodeTypeEnum.RegCMeeverlengen => new []{30},
                CCOLCodeTypeEnum.RegCDetectieStoring => new []{130},
                CCOLCodeTypeEnum.RegCFileVerwerking => new []{130},
                CCOLCodeTypeEnum.RegCSynchronisaties => new []{40},
                CCOLCodeTypeEnum.RegCAlternatieven => new []{130},
                CCOLCodeTypeEnum.RegCRealisatieAfhandelingVersneldPrimair => new []{130},
                CCOLCodeTypeEnum.RegCRealisatieAfhandelingNaModules => new []{130},
                CCOLCodeTypeEnum.RegCRealisatieAfhandeling => new []{130},
                CCOLCodeTypeEnum.RegCInitApplication => new []{130},
                CCOLCodeTypeEnum.RegCPostApplication => new []{130},
                CCOLCodeTypeEnum.RegCPostSystemApplication => new []{130},
                CCOLCodeTypeEnum.RegCDumpApplication => new []{10},
                
                CCOLCodeTypeEnum.PrioCIncludes => new []{20},
                CCOLCodeTypeEnum.PrioCInUitMelden => new []{120},
                CCOLCodeTypeEnum.PrioCPrioriteitsOpties => new []{120},
                
                CCOLCodeTypeEnum.SysHDefines => new []{130},
                _ => null
            };
        }

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            if (!c.Data.TraffickCompatible) return "";
            
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.SysHDefines:
                    if (c.Kruispunt.KruispuntArmen.Any())
                    {
                        sb.AppendLine();
                        sb.AppendLine("/* Kruispunt armen definities */");
                        var k = 0;
                        foreach (var ka in c.Kruispunt.KruispuntArmen)
                        {
                            sb.AppendLine($"{ts}#define {ka.Naam} {k}");
                            ++k;
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCIncludes:
                    sb.AppendLine("/* Traffick2TLCGen */");
                    sb.AppendLine("#include \"traffick2tlcgen.c\"");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPreApplication:
                    var gelijkstarttuples = CCOLCodeHelper.GetFasenWithGelijkStarts(c);
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[schtraffick2tlcgen])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}/* bijwerken kruispunt variabelen */");
                    sb.AppendLine($"{ts}{ts}/* ------------------------------ */");
                    sb.AppendLine($"{ts}{ts}traffick2tlcgen_kruispunt();");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/* bijwerken detectie variabelen */");
                    sb.AppendLine($"{ts}{ts}/* ----------------------------- */");
                    sb.AppendLine($"{ts}{ts}traffick2tlcgen_detectie();");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/* faseyclus instellingen */");
                    sb.AppendLine($"{ts}{ts}/* ---------------------- */");
                    var noPrio = c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.Geen;
                    foreach (var fc in c.Fasen)
                    {
                        var gs = gelijkstarttuples.FirstOrDefault(x => x.Item1 == fc.Naam);

                        var namealtp = fc.Naam;
                        var namealtg = fc.Naam;
                        if (gs != null)
                        {
                            namealtp = string.Join(string.Empty, gs.Item2);
                            namealtg = string.Join(string.Empty, gs.Item2);
                        }

                        var kar = c.PrioData.PrioIngrepen.FirstOrDefault(x => x.FaseCyclus == fc.Naam && x.MeldingenData.Inmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
                        var srm = c.PrioData.PrioIngrepen.FirstOrDefault(x => x.FaseCyclus == fc.Naam && x.MeldingenData.Inmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde));
                        var hd  = c.PrioData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                        var vrw = c.PrioData.PrioIngrepen.FirstOrDefault(x => x.FaseCyclus == fc.Naam && x.Type == PrioIngreepVoertuigTypeEnum.Vrachtwagen);
                        var fts = c.PrioData.PrioIngrepen.FirstOrDefault(x => x.FaseCyclus == fc.Naam && x.Type == PrioIngreepVoertuigTypeEnum.Fiets);
                        sb.AppendLine($"{ts}{ts}traffick2tlcgen_instel(" +
                                      $"{_fcpf}{fc.Naam}, " +
                                      (fc.Wachtgroen == NooitAltijdAanUitEnum.Nooit ? "FALSE, " :
                                       fc.Wachtgroen == NooitAltijdAanUitEnum.Altijd ? "TRUE, " : $"SCH[{_schpf}{_schwg}{fc.Naam}], ") +
                                      $"TRUE, " +
                                      (fc.Meeverlengen == NooitAltijdAanUitEnum.Nooit ? "FALSE, " :
                                       fc.Meeverlengen == NooitAltijdAanUitEnum.Altijd ? "TRUE, " : $"SCH[{_schpf}{_schmv}{fc.Naam}], ") +
                                      $"FALSE, " +
                                      $"SCH[{_schpf}{_schaltg}{namealtg}], " +
                                      (c.AlternatievenPerBlokData.ToepassenAlternatievenPerBlok ? $"PRM[{_prmpf}{_prmaltb}{fc.Naam}], " : "NG, ") +
                                      $"PRM[{_prmpf}{_prmaltp}{namealtp}], " +
                                      $"PRM[{_prmpf}{_prmaltg}{fc.Naam}], " +
                                      (!noPrio && kar != null ? $"prioFC{CCOLCodeHelper.GetPriorityName(c, kar)}, " : "NG, ") +
                                      (!noPrio && srm != null ? $"prioFC{CCOLCodeHelper.GetPriorityName(c, srm)}, " : "NG, ") +
                                      $"NG, " +
                                      (!noPrio && hd != null ? $"hdFC{hd.FaseCyclus}, " : "NG, ") +
                                      (!noPrio && hd != null ? $"C[{_ctpf}{_cvchd}{hd.FaseCyclus}], " : "FALSE, ") +
                                      (!noPrio && vrw != null ? $"prioFC{CCOLCodeHelper.GetPriorityName(c, vrw)}, " : "NG, ") +
                                      (!noPrio && fts != null ? $"prioFC{CCOLCodeHelper.GetPriorityName(c, fts)});" : "NG);"));
                    }
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();    
                
                case CCOLCodeTypeEnum.RegCAanvragen:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}fiets_voorrang_aanvraag();");
                    sb.AppendLine($"{ts}{ts}hki_wachtstand_aanvraag();");
                    sb.AppendLine($"{ts}{ts}koppel_aanvragen();");
                    sb.AppendLine($"{ts}{ts}peloton_ingreep_aanvraag();");
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCKlokPerioden:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen: DVM */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) bepaal_DVM_programma();");

                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCBepaalRealisatieTijden:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}RealTraffick();");
                    sb.AppendLine($"{ts}{ts}BepaalAltRuimte();");
                    sb.AppendLine($"{ts}{ts}bepaal_maximum_groen_traffick();");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"#if (!defined (AUTOMAAT) && !defined AUTOMAAT_TEST || defined (VISSIM)) && !defined NO_PRINT_REALTIJD");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}if (display)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}count fc;");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(92, 6, \"      T2SG T2EG AltR  TFB Aled  TVG\");");
                    sb.AppendLine($"{ts}{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(97, 7 + fc, \"%5d\", REALtraffick[fc]);");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(102, 7 + fc, \"%5d\", TEG[fc]);");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(107, 7 + fc, \"%5d\", AltRuimte[fc]);");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(112, 7 + fc, \"%5d\", TFB_timer[fc]);");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(117, 7 + fc, \"%5d\", Aled[fc]);");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(122, 7 + fc, \"%5d\", TVG_max[fc]);");
                    sb.AppendLine($"{ts}{ts}{ts}}}");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"#endif");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCWachtgroen:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}Traffick2TLCgen_WGR();");
                    sb.AppendLine($"{ts}{ts}peloton_ingreep_wachtgroen();");
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) peloton_ingreep_verlengen();");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCMeeverlengen:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) Traffick2TLCgen_MVG();");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCFileVerwerking:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) traffick_file_afhandeling();");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCDetectieStoring:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) maatregelen_bij_detectie_storing();");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}Traffick2TLCgen_uitstel();");
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();    
                
                case CCOLCodeTypeEnum.RegCAlternatieven:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}Traffick2TLCgen_PAR();");
                    sb.AppendLine($"{ts}{ts}BeeindigAltRealisatie();");
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();    
                
                case CCOLCodeTypeEnum.RegCRealisatieAfhandelingVersneldPrimair:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}Traffick2TLCgen_PFPR();");
                    sb.AppendLine($"{ts}}}"); 
                    return sb.ToString();    

                case CCOLCodeTypeEnum.RegCRealisatieAfhandelingNaModules:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) BugFix_RR_bij_HKI();");
                    return sb.ToString();    
                
                case CCOLCodeTypeEnum.RegCRealisatieAfhandeling:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) Traffick2TLCgen_REA();");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCInitApplication:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}init_traffick2tlcgen();");
                    
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.EindeGroen || x.Type == NaloopTypeEnum.CyclischVerlengGroen))
                    {
                        var volg = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);
                        var tnlfg = nl.VasteNaloop ? $"{_tpf}{_tnlfg}{nl:vannaar}" : "NG";
                        var tnleg = nl.VasteNaloop ? $"{_tpf}{_tnleg}{nl:vannaar}" : "NG";
                        var tnlfgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlfgd}{nl:vannaar}" : "NG";
                        var tnlegd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlegd}{nl:vannaar}" : "NG";
                        sb.AppendLine($"{ts}definitie_harde_koppeling(" +
                                      $"{_fcpf}{nl:van}, {_fcpf}{nl:naar}, {_tpf}lr{nl:naarvan}, " +
                                      $"{tnlfg}, {tnlfgd}, {tnleg}, {tnlegd}, " +
                                      $"{(nl.Type == NaloopTypeEnum.EindeGroen ? "TRUE" : "FALSE")}, " +
                                      $"{(volg?.Type == FaseTypeEnum.Auto || volg?.Type == FaseTypeEnum.OV ? "TRUE" : "FALSE")}, " +
                                      $"NG);");
                    }

                    var doneVrgs = new List<string>();
                    foreach (var nl in c.GetVoetgangersNalopen())
                    {
                        if (doneVrgs.Contains(nl.FaseVan)) continue;
                        doneVrgs.Add(nl.FaseVan);
                        doneVrgs.Add(nl.FaseNaar);
                        var other = c.InterSignaalGroep.Nalopen.FirstOrDefault(x => x.FaseVan == nl.FaseNaar);
                        if (other == null ||
                            !(nl.Type == NaloopTypeEnum.StartGroen && other.Type == NaloopTypeEnum.StartGroen)) continue;

                        var d1 = nl.Detectoren.FirstOrDefault();
                        var d2 = other.Detectoren.FirstOrDefault();
                        
                        sb.AppendLine($"{ts}definitie_vtg_gescheiden(" +
                                      $"{_fcpf}{nl:van}, {_fcpf}{nl:naar}, " +
                                      $"{_tpf}{_trealil}{nl:van}{nl:naar}, {_tpf}{_trealil}{nl:naar}{nl:van}, " +
                                      $"{_tpf}{_tnlsgd}{nl:van}{nl:naar}, {_tpf}{_tnlsgd}{nl:naar}{nl:van}, " +
                                      $"{(d1 != null ? $"{_hpf}{_hnla}{d1.Detector}" : "NG")}, " +
                                      $"{(d2 != null ? $"{_hpf}{_hnla}{d2.Detector}" : "NG")}, " +
                                      $"{_hpf}{_hlos}{nl:van}, {_hpf}{_hlos}{nl:naar});");
                    }

                    List<GelijkstartModel> otherGs = new();
                    List<string> doneGsFcs = new();
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        if (doneGsFcs.Contains(gs.FaseVan) || gs.DeelConflict) continue;
                        otherGs = c.InterSignaalGroep.Gelijkstarten
                            .Where(x => 
                                !ReferenceEquals(x, gs) && 
                                (x.FaseVan == gs.FaseNaar || x.FaseNaar == gs.FaseNaar || x.FaseVan == gs.FaseVan || x.FaseNaar == gs.FaseVan)).ToList();
                        List<GelijkstartModel> yetOtherGs = new();
                        List<GelijkstartModel> finalOtherGs = new();
                        if (otherGs.Count > 0)
                        {
                            foreach (var oGs in otherGs)
                            {
                                yetOtherGs = c.InterSignaalGroep.Gelijkstarten.Where(x => !ReferenceEquals(x, gs) && otherGs.All(x2 => !ReferenceEquals(x, x2)) && (x.FaseVan == oGs.FaseNaar || x.FaseNaar == oGs.FaseNaar || x.FaseVan == oGs.FaseVan|| x.FaseNaar == oGs.FaseVan)).ToList();
                                if (yetOtherGs.Count > 0)
                                {
                                    foreach (var foGs in otherGs)
                                    {
                                        finalOtherGs = c.InterSignaalGroep.Gelijkstarten.Where(x => 
                                            !ReferenceEquals(x, gs) && 
                                            otherGs.All(x2 => !ReferenceEquals(x, x2)) && 
                                            yetOtherGs.All(x2 => !ReferenceEquals(x, x2)) && 
                                            (x.FaseVan == foGs.FaseNaar || x.FaseNaar == foGs.FaseNaar || x.FaseVan == foGs.FaseVan|| x.FaseNaar == foGs.FaseVan)).ToList();
                                    }
                                }
                            }
                        }
                        var gsFcs = new List<GelijkstartModel> { gs }
                            .Concat(otherGs)
                            .Concat(yetOtherGs)
                            .Concat(finalOtherGs)
                            .SelectMany(x => new[] {x.FaseVan, x.FaseNaar })
                            .Distinct()
                            .Take(4)
                            .ToList();
                        sb.Append($"{ts}definitie_gelijkstart_lvk(");
                        for (int fc = 0; fc < 4; fc++)
                        {
                            if (fc > 0) sb.Append(", ");
                            if (fc < gsFcs.Count)
                            {
                                sb.Append($"{_fcpf}{gsFcs[fc]}");
                                doneGsFcs.Add(gsFcs[fc]);
                            }
                            else sb.Append($"NG");
                        }
                        sb.AppendLine(");");
                    }
                    
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        var ma = c.InterSignaalGroep.Meeaanvragen.FirstOrDefault(x => x.FaseVan == vs.FaseNaar);
                        var fcVan = c.Fasen.FirstOrDefault(x => x.Naam == vs.FaseVan);
                        sb.AppendLine($"{ts}definitie_voorstart_dcf(" +
                                      $"{_fcpf}{vs:naar}, {_fcpf}{vs:van}, " +
                                      $"{_tpf}{_tvs}{vs:vannaar}, " +
                                      $"{_tpf}{_tfo}{vs:naarvan}, " +
                                      $"{(ma != null ? $"{_schpf}{_schma}{vs:naarvan}": "NG")}, " +
                                      $"{(fcVan?.HardMeeverlengenFaseCycli.Any(x => x.FaseCyclus == vs.FaseNaar) == true ? $"{_schpf}{_schhardmv}{vs:vannaar}" : "NG")});");
                    }

                    if (c.Kruispunt.KruispuntArmen.Any() && c.Kruispunt.FasenMetKruispuntArmen.Any(x => x.KruispuntArm != "NG"))
                    {
                        sb.AppendLine($"{ts}/* Definitie kruispunt armen */");
                        foreach (var fc in c.Kruispunt.FasenMetKruispuntArmen.Where(x => x.KruispuntArm != "NG"))
                        {
                            sb.AppendLine($"{ts}ARM[{_fcpf}{fc.FaseCyclus}] = {fc.KruispuntArm};");
                        }
                        foreach (var fc in c.Kruispunt.FasenMetKruispuntArmen.Where(x => x.KruispuntArmVolg != "NG"))
                        {
                            sb.AppendLine($"{ts}volg_ARM[{_fcpf}{fc.FaseCyclus}] = {fc.KruispuntArmVolg};");
                        }
                    }

                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) extra_definities_traffick();");

                    return sb.ToString();    
                
                case CCOLCodeTypeEnum.RegCPostApplication:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) corrigeer_verklikking_stiptheid();");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST");
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[schtraffick2tlcgen])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}FlightTraffick();");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[schtraffick2tlcgen])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}traffick_corrigeer_wtv();");
                    sb.AppendLine($"{ts}verklik_fiets_voorrang();");
                    sb.AppendLine($"{ts}verklik_peloton_ingreep();");
                    sb.AppendLine($"{ts}verklik_prio_KAR_SRM();");
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCDumpApplication:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) DumpTraffick();");
                    sb.AppendLine($"#endif");
                    return sb.ToString();    
                
                case CCOLCodeTypeEnum.RegCTop:
                    sb.AppendLine("#define TRAFFICK");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.PrioCIncludes:
                    sb.AppendLine("/* Traffick2TLCGen */");
                    sb.AppendLine("#define TRAFFICK");
                    sb.AppendLine("#include \"traffick2tlcgen.h\"");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.PrioCInUitMelden:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}fiets_voorrang_module();");
                    sb.AppendLine($"{ts}{ts}buffer_stiptheid_info();");
                    sb.AppendLine($"{ts}{ts}busbaan_verlos_prioriteit();");
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.PrioCPrioriteitsOpties:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}Traffick2TLCgen_PRIO_OPTIES();");
                    if (c.Kruispunt.FasenMetKruispuntArmen.Any(x => x.HasKruispuntArmVolgTijd))
                    {
                        foreach (var kparmfc in c.Kruispunt.FasenMetKruispuntArmen.Where(x => x.HasKruispuntArmVolgTijd))
                        {
                            sb.AppendLine($"{ts}{ts}Traffick2TLCgen_HLPD_nal({_fcpf}{kparmfc.FaseCyclus}, T_max[{_tpf}{_tarmvt}{kparmfc.FaseCyclus}]);");
                        }
                    }
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();
            }

            return sb.ToString();
        }

        public override bool HasSettings() => true;

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _schmv = CCOLGeneratorSettingsProvider.Default.GetElementName("schmv");
            _schwg = CCOLGeneratorSettingsProvider.Default.GetElementName("schwg");
            _schaltg = CCOLGeneratorSettingsProvider.Default.GetElementName("schaltg");
            _prmaltb = CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltb");
            _prmaltg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltg");
            _prmaltp = CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltp");
            _cvchd = CCOLGeneratorSettingsProvider.Default.GetElementName("cvchd");
            _tnlfg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfg");
            _tnlfgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfgd");
            _tnleg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnleg");
            _tnlegd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlegd");
            _tnlsgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsgd");
            _hlos = CCOLGeneratorSettingsProvider.Default.GetElementName("hlos");
            _hnla = CCOLGeneratorSettingsProvider.Default.GetElementName("hnla");
            _trealil = CCOLGeneratorSettingsProvider.Default.GetElementName("trealil");
            _tvs = CCOLGeneratorSettingsProvider.Default.GetElementName("tvs");
            _tfo = CCOLGeneratorSettingsProvider.Default.GetElementName("tfo");
            _schma = CCOLGeneratorSettingsProvider.Default.GetElementName("schma");
            _schhardmv = CCOLGeneratorSettingsProvider.Default.GetElementName("schhardmv");

            return base.SetSettings(settings);
        }
    }
}
