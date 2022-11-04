using System.Collections.Generic;
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
        private string _prmaltg;
        private string _cvchd;
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schtraffick2tlcgen;
#pragma warning restore 0649
        
        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if (!c.Data.TraffickCompatible) return;
            
            _myElements.Add(
                CCOLGeneratorSettingsProvider.Default.CreateElement(
                    $"{_schtraffick2tlcgen}", 1, CCOLElementTimeTypeEnum.None, _schtraffick2tlcgen));
        }

        public override bool HasCCOLElements() => true;

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCIncludes => new []{130},
                CCOLCodeTypeEnum.RegCPreApplication => new []{130},
                CCOLCodeTypeEnum.RegCBepaalRealisatieTijden => new []{20},
                CCOLCodeTypeEnum.RegCVerlenggroen => new []{80},
                CCOLCodeTypeEnum.RegCMaxgroen => new []{80},
                CCOLCodeTypeEnum.RegCWachtgroen => new []{40},
                CCOLCodeTypeEnum.RegCMeetkriterium => new []{130},
                CCOLCodeTypeEnum.RegCMeeverlengen => new []{30},
                CCOLCodeTypeEnum.RegCSynchronisaties => new []{40},
                CCOLCodeTypeEnum.RegCRealisatieAfhandelingVoorModules => new []{20},
                CCOLCodeTypeEnum.RegCRealisatieAfhandelingNaModules => new []{10},
                CCOLCodeTypeEnum.RegCRealisatieAfhandeling => new []{130},
                CCOLCodeTypeEnum.RegCInitApplication => new []{130},
                CCOLCodeTypeEnum.RegCPostApplication => new []{130},
                CCOLCodeTypeEnum.RegCDumpApplication => new []{10},
                
                CCOLCodeTypeEnum.PrioCIncludes => new []{20},
                CCOLCodeTypeEnum.PrioCInUitMelden => new []{120},
                CCOLCodeTypeEnum.PrioCAfkapGroen => new []{5},
                CCOLCodeTypeEnum.PrioCStartGroenMomenten => new []{5, 120},
                CCOLCodeTypeEnum.PrioCPrioriteitsOpties => new []{120},
                CCOLCodeTypeEnum.PrioCPrioriteitsToekenning => new []{5},
                CCOLCodeTypeEnum.PrioCTegenhoudenConflicten => new []{10},
                CCOLCodeTypeEnum.PrioCPostAfhandelingPrio => new []{120},
                CCOLCodeTypeEnum.PrioCPARCorrecties => new []{120},
                
                CCOLCodeTypeEnum.SysHDefines => new []{130},
                _ => null
            };
        }

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    return c.Data.TraffickCompatible
                        ? new List<CCOLLocalVariable> { new("count", "i", "0") }
                        : base.GetFunctionLocalVariables(c, type);
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
                        sb.AppendLine("/* Kruispunt armen definities */");
                        var k = 0;
                        foreach (var ka in c.Kruispunt.KruispuntArmen)
                        {
                            sb.AppendLine($"#define {ka.Naam} {k}");
                            ++k;
                        }
                        sb.AppendLine();
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
                    sb.AppendLine($"{ts}{ts}/* bijwerken detectie variabelen */");
                    sb.AppendLine($"{ts}{ts}/* ----------------------------- */");
                    sb.AppendLine($"{ts}{ts}traffick2tlcgen_detectie();");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/* faseyclus instellingen */");
                    sb.AppendLine($"{ts}{ts}/* ---------------------- */");
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
                                      $"SCH[{_schpf}{_schwg}{fc.Naam}], " +
                                      $"TRUE," +
                                      $"SCH[{_schpf}{_schmv}{fc.Naam}], " +
                                      $"FALSE, " +
                                      $"SCH[{_schpf}{_schaltg}{namealtg}], " +
                                      $"PRM[{_prmpf}{_prmaltp}{namealtp}], " +
                                      $"PRM[{_prmpf}{_prmaltg}{fc.Naam}], " +
                                      (kar != null ? $"prioFC{CCOLCodeHelper.GetPriorityName(c, kar)}, " : "NG, ") +
                                      (srm != null ? $"prioFC{CCOLCodeHelper.GetPriorityName(c, srm)}, " : "NG, ") +
                                      $"NG, " +
                                      (hd != null ? $"hdFC{hd.FaseCyclus}, " : "NG, ") +
                                      (hd != null ? $"C[{_ctpf}{_cvchd}{hd.FaseCyclus}], " : "FALSE, ") +
                                      (vrw != null ? $"prioFC{CCOLCodeHelper.GetPriorityName(c, vrw)}, " : "NG, ") +
                                      (fts != null ? $"prioFC{CCOLCodeHelper.GetPriorityName(c, fts)});" : "NG);"));
                    }
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();    
                
                case CCOLCodeTypeEnum.RegCBepaalRealisatieTijden:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) RealTraffick();");
                    return sb.ToString();    
                
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) BepaalAltRuimte();");
                    sb.AppendLine($"#if (!defined (AUTOMAAT) && !defined AUTOMAAT_TEST || defined (VISSIM)) && !defined NO_PRINT_REALTIJD");
                    sb.AppendLine($"{ts}if (SCH[schtraffick2tlcgen])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}if (display)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}count fc;");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(92, 6, \"      T2SG T2EG AltR  TFB  MTG\");");
                    sb.AppendLine($"{ts}{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(97, 7 + fc, \"%5d\", REALtraffick[fc]);");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(102, 7 + fc, \"%5d\", TEG[fc]);");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(107, 7 + fc, \"%5d\", AltRuimte[fc]);");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(112, 7 + fc, \"%5d\", TFB_timer[fc]);");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(117, 7 + fc, \"%5d\", MTG[fc]);");
                    sb.AppendLine($"{ts}{ts}{ts}}}");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"#endif");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCWachtgroen:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) Traffick2TLCgen_WGR();");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) Traffick2TLCgen_MVG();");
                    return sb.ToString();
                    
                case CCOLCodeTypeEnum.RegCMeeverlengen:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) Traffick2TLCgen_MVG();");
                    return sb.ToString();    
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}Traffick2TLCgen_uitstel();");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}for (i = 0; i < aantal_hki_kop; ++i)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}count fc1 = hki_kop[i].fc1;      /* voedende richting */");
                    sb.AppendLine($"{ts}{ts}count fc2 = hki_kop[i].fc2;      /* volg     richting */");
                    sb.AppendLine($"{ts}{ts}{ts}if (PRML[ML][fc1] != PRIMAIR) REAL_SYN[fc1][fc2] = REAL_SYN[fc2][fc1] = FALSE;");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();    
                
                case CCOLCodeTypeEnum.RegCRealisatieAfhandelingVoorModules:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}Traffick2TLCgen_PAR();");
                    sb.AppendLine($"{ts}{ts}BeeindigAltRealisatie();");
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
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.EindeGroen))
                    {
                        sb.AppendLine($"{ts}definitie_harde_koppeling(fc02, fc62, tlr6202, tnlfg0262, tnlfgd0262, tnleg0262, tnlegd0262, TRUE, TRUE, TVG_max[fc62]);");
                    }
                    
                    foreach (var nl in c.GetVoetgangersNalopen())
                    {
                        sb.AppendLine($"{ts}definitie_vtg_gescheiden(fc31, fc32, tinl3132, tinl3231, tnlsgd3132, tnlsgd3231, hnlak31a, hnlak32a, hlos31, hlos32);");
                    }

                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        sb.AppendLine($"{ts}definitie_gelijkstart_lvk(fc22, fc32, NG, NG);");
                    }
                    
                    foreach (var gs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}definitie_voorstart_dcf(fc05, fc22, tvs2205, tfo0522, schma0522, schhardmv2205);");
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
                    return sb.ToString();    
                
                case CCOLCodeTypeEnum.RegCPostApplication:
                    
//#if (!defined (AUTOMAAT) && !defined AUTOMAAT_TEST || defined (VISSIM)) && defined NO_PRINT_REALTIJD
//    /* Traffick2TLCGen */
//    if (display)
//    {
//       count fc;
//
//       xyprintf(0, 0, "FC   T2SG T2EG AltR  TFB   AR   PG  PAR   HLPD");
//       for (fc = 0; fc < FCMAX; ++fc)
//       {
//          xyprintf(0, 1 + fc, "%s%s%5d%5d%5d%5d%5d%5d%5d%5d", "FC", FC_code[fc], REALtraffick[fc], TEG[fc], AltRuimte[fc], TFB_timer[fc], AR[fc], PG[fc], PAR[fc], HLPD[fc]);
//       }
//    }
//#endif
//
//#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
//    /* Traffick2TLCGen */
//    if (SCH[schtraffick2tlcgen])
//    {
//       FlightTraffick();
//    }
//#endif
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCDumpApplication:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) DumpTraffick();");
                    return sb.ToString();    
                
                case CCOLCodeTypeEnum.PrioCIncludes:
                    sb.AppendLine("/* Traffick2TLCGen */");
                    sb.AppendLine("#include \"traffick2tlcgen.h\"");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCInUitMelden:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) fiets_voorrang_module();");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCAfkapGroen:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) Traffick2TLCgen_PRIO_TOE();");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCStartGroenMomenten:
                    if (order == 5)
                    {
                        sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                        sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) Traffick2TLCpas_TVG_aan();");
                    }
                    if (order == 120)
                    {
                        sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                        sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) Traffick2TLCzet_TVG_terug();");
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCPrioriteitsOpties:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}Traffick2TLCgen_HLPD();");
                    if (c.InterSignaalGroep.Nalopen.Any(x => x.Type == NaloopTypeEnum.EindeGroen))
                    {
                        sb.AppendLine();
                        foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.EindeGroen))
                        {
                            sb.AppendLine($"{ts}{ts}Traffick2TLCgen_HLPD_nal({_fcpf}{nl:van}, {_fcpf}{nl:naar}, 100);");
                        }
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}Traffick2TLCgen_HLPD();");
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.PrioCPrioriteitsToekenning:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) corrigeer_blokkeringstijd_OV();");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.PrioCTegenhoudenConflicten:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) Traffick2TLCgen_PRIO_RR();");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.PrioCPostAfhandelingPrio:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) Traffick2TLCgen_PRIO();");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.PrioCPARCorrecties:
                    sb.AppendLine($"{ts}/* Traffick2TLCGen */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtraffick2tlcgen}]) Traffick2TLCgen_PRIO_PAR();");
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
            _prmaltg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltg");
            _prmaltp = CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltp");
            _cvchd = CCOLGeneratorSettingsProvider.Default.GetElementName("cvchd");

            return base.SetSettings(settings);
        }
    }
}
