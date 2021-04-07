using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins.Timings.Models;

namespace TLCGen.Plugins.Timings.CodeGeneration
{
    public class TimingsCodeGenerator
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schfctiming = new CCOLGeneratorCodeStringSettingModel
        {
            Default = "fctiming", Setting = "fctiming", Type = CCOLGeneratorSettingTypeEnum.Schakelaar, Description = "Timings activeren"
        };
        private CCOLGeneratorCodeStringSettingModel _prmttxconfidence15 = new CCOLGeneratorCodeStringSettingModel
        {
            Default = "ttxconfidence15", Setting = "ttxconfidence15", Type = CCOLGeneratorSettingTypeEnum.Parameter, Description = ""
        };
        private CCOLGeneratorCodeStringSettingModel _schconfidence15fix = new CCOLGeneratorCodeStringSettingModel
        {
            Default = "confidence15fix", Setting = "confidence15fix", Type = CCOLGeneratorSettingTypeEnum.Schakelaar, Description = ""
        };
        private CCOLGeneratorCodeStringSettingModel _schtxconfidence15ar = new CCOLGeneratorCodeStringSettingModel
        {
            Default = "txconfidence15ar", Setting = "txconfidence15ar", Type = CCOLGeneratorSettingTypeEnum.Schakelaar, Description = ""
        };
        private CCOLGeneratorCodeStringSettingModel _schspatconfidence1 = new CCOLGeneratorCodeStringSettingModel
        {
            Default = "spatconfidence1", Setting = "spatconfidence1", Type = CCOLGeneratorSettingTypeEnum.Schakelaar, Description = ""
        };
        private CCOLGeneratorCodeStringSettingModel _schspatconfidence3 = new CCOLGeneratorCodeStringSettingModel
        {
            Default = "spatconfidence3", Setting = "spatconfidence3", Type = CCOLGeneratorSettingTypeEnum.Schakelaar, Description = ""
        };
        private CCOLGeneratorCodeStringSettingModel _schspatconfidence6 = new CCOLGeneratorCodeStringSettingModel
        {
            Default = "spatconfidence6", Setting = "spatconfidence6", Type = CCOLGeneratorSettingTypeEnum.Schakelaar, Description = ""
        };
        private CCOLGeneratorCodeStringSettingModel _schspatconfidence9 = new CCOLGeneratorCodeStringSettingModel
        {
            Default = "spatconfidence9", Setting = "spatconfidence9", Type = CCOLGeneratorSettingTypeEnum.Schakelaar, Description = ""
        };
        private CCOLGeneratorCodeStringSettingModel _schspatconfidence12 = new CCOLGeneratorCodeStringSettingModel
        {
            Default = "spatconfidence12", Setting = "spatconfidence12", Type = CCOLGeneratorSettingTypeEnum.Schakelaar, Description = ""
        };
        private CCOLGeneratorCodeStringSettingModel _schspatconfidence15 = new CCOLGeneratorCodeStringSettingModel
        {
            Default = "spatconfidence15", Setting = "spatconfidence15", Type = CCOLGeneratorSettingTypeEnum.Schakelaar, Description = ""
        };
#pragma warning restore 0649
        
        public string _fcpf;
        public string _schpf;
        public string _mpf;
        public string _prmpf;
        public string _ctpf;
        
        public string _mrealtijdmin;
        public string _mrealtijdmax;
        public string _cvc;
        public string _schgs;

        public List<CCOLElement> GetCCOLElements(ControllerModel c)
        {
            var elements = new List<CCOLElement>
            {
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schfctiming}", 1, CCOLElementTimeTypeEnum.SCH_type, _schfctiming),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmttxconfidence15}", 30, CCOLElementTimeTypeEnum.None, _prmttxconfidence15),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schconfidence15fix}", 1, CCOLElementTimeTypeEnum.SCH_type, _schconfidence15fix),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schtxconfidence15ar}", 1, CCOLElementTimeTypeEnum.SCH_type, _schtxconfidence15ar),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schspatconfidence1}", 0, CCOLElementTimeTypeEnum.SCH_type, _schspatconfidence1),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schspatconfidence3}", 0, CCOLElementTimeTypeEnum.SCH_type, _schspatconfidence3),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schspatconfidence6}", 0, CCOLElementTimeTypeEnum.SCH_type, _schspatconfidence6),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schspatconfidence9}", 0, CCOLElementTimeTypeEnum.SCH_type, _schspatconfidence9),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schspatconfidence12}", 0, CCOLElementTimeTypeEnum.SCH_type, _schspatconfidence12),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schspatconfidence15}", 1, CCOLElementTimeTypeEnum.SCH_type, _schspatconfidence15)
            };
            return elements;
        }
        
        public int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    return 40;
                case CCOLCodeTypeEnum.RegCAlternatieven:
                    return 50;
                case CCOLCodeTypeEnum.RegCRealisatieAfhandeling:
                    return 30;
                case CCOLCodeTypeEnum.RegCIncludes:
                    return 120;
                case CCOLCodeTypeEnum.RegCSystemApplication2:
                    return 120;
                case CCOLCodeTypeEnum.TabCIncludes:
                case CCOLCodeTypeEnum.TabCControlParameters:
                    return 120;
                case CCOLCodeTypeEnum.PrioCRijTijdScenario:
                    return 10;
                case CCOLCodeTypeEnum.PrioCTegenhoudenConflicten:
                    return 40;
                case CCOLCodeTypeEnum.PrioCAfkappen:
                    return 20;
                case CCOLCodeTypeEnum.PrioCPARCorrecties:
                    return 30;
                default:
                    return 0;
            }
        }

        public string GetCode(TimingsDataModel timingsModel, ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            if (c.Data.CCOLVersie <= CCOLVersieEnum.CCOL8 || !timingsModel.TimingsToepassen) return null;

            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:

                    // Generate rissim.c now
                    GenerateFcTimingsC(c, timingsModel, ts);

                    sb.AppendLine($"{ts}#include \"timingsvar.c\" /* FCTiming functies */");
                    sb.AppendLine($"{ts}#include \"timingsfunc.c\" /* FCTiming functies */");
                    if (timingsModel.TimingsUsePredictions)
                    {
                        sb.AppendLine($"{ts}#include \"timings_uc4.c\" /* FCTiming functies */");
                    }
                    sb.AppendLine($"{ts}#include \"{c.Data.Naam}fctimings.c\" /* FCTiming functies */");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCSystemApplication2:
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schfctiming}]) msg_fctiming();");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}#if !(defined NO_TIMETOX) && (!defined (AUTOMAAT) || defined (VISSIM))");
                    sb.AppendLine($"{ts}#if !defined (AUTOMAAT) || defined (VISSIM)");
                    sb.AppendLine($"{ts}{ts}for (i=0; i<FCMAX; i++)");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(63+(i*7), 44,\"fc%s\", FC_code[i]);");
                    sb.AppendLine($"{ts}");
                    sb.AppendLine($"{ts}{ts}xyprintf(40, 45, \"CIF_TIMING_MASK       : \");");
                    sb.AppendLine($"{ts}{ts}xyprintf(40, 46, \"CIF_TIMING_EVENTSTATE : \");");
                    sb.AppendLine($"{ts}{ts}xyprintf(40, 47, \"CIF_TIMING_STARTTIME  : \");");
                    sb.AppendLine($"{ts}{ts}xyprintf(40, 48, \"CIF_TIMING_MINENDTIME : \");");
                    sb.AppendLine($"{ts}{ts}xyprintf(40, 49, \"CIF_TIMING_MAXENDTIME : \");");
                    sb.AppendLine($"{ts}{ts}xyprintf(40, 50, \"CIF_TIMING_LIKELYTIME : \");");
                    sb.AppendLine($"{ts}{ts}xyprintf(40, 51, \"CIF_TIMING_CONFIDENCE : \");");
                    sb.AppendLine($"{ts}{ts}xyprintf(40, 52, \"CIF_TIMING_NEXTTIME   : \");");
                    sb.AppendLine($"{ts}{ts}for (i = 0; i < FCMAX; i++)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(63+(i*7), 45, \"%4d\", CIF_FC_TIMING[i][0][CIF_TIMING_MASK]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(63+(i*7), 46, \"%4d\", CIF_FC_TIMING[i][0][CIF_TIMING_EVENTSTATE]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(63+(i*7), 47, \"%4d\", CIF_FC_TIMING[i][0][CIF_TIMING_STARTTIME]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(63+(i*7), 48, \"%4d\", CIF_FC_TIMING[i][0][CIF_TIMING_MINENDTIME]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(63+(i*7), 49, \"%4d\", CIF_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(63+(i*7), 50, \"%4d\", CIF_FC_TIMING[i][0][CIF_TIMING_LIKELYTIME]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(63+(i*7), 51, \"%4d\", CIF_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(63+(i*7), 52, \"%4d\", CIF_FC_TIMING[i][0][CIF_TIMING_NEXTTIME]);");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}#endif");
                    sb.AppendLine($"{ts}#endif");
                    
                    return sb.ToString();
                case CCOLCodeTypeEnum.TabCIncludes:
                    sb.AppendLine($"{ts}void Timings_Eventstate_Definition(void);");
                    return sb.ToString();
                case CCOLCodeTypeEnum.TabCControlParameters:
                    sb.AppendLine($"{ts}Timings_Eventstate_Definition();");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCAlternatieven:
                    sb.AppendLine($"{ts}#ifndef NO_TIMETOX");
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        var sch = "";
                        if (gs.Schakelbaar != AltijdAanUitEnum.Altijd)
                        {
                            sch = $"SCH[{_schpf}{_schgs}{gs:van}{gs:naar}] && ";
                        }
                        sb.AppendLine($"{ts}if ({sch}(P[{_fcpf}{gs:van}] & BIT11) && R[{_fcpf}{gs:naar}] && !kp({_fcpf}{gs:naar}) && A[{_fcpf}{gs:naar}]) {{ PAR[{_fcpf}{gs:naar}] |= BIT11; P[{_fcpf}{gs:naar}] |= BIT11; }}");
                        sb.AppendLine($"{ts}if ({sch}(P[{_fcpf}{gs:naar}] & BIT11) && R[{_fcpf}{gs:van}] && !kp({_fcpf}{gs:van}) && A[{_fcpf}{gs:van}]) {{ PAR[{_fcpf}{gs:van}] |= BIT11; P[{_fcpf}{gs:van}] |= BIT11; }}");
                    }
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}if ((P[{_fcpf}{vs:naar}] & BIT11) && R[{_fcpf}{vs:van}] && !kp({_fcpf}{vs:van}) && A[{_fcpf}{vs:van}]) {{ PAR[{_fcpf}{vs:van}] |= BIT11; P[{_fcpf}{vs:van}] |= BIT11; }}");
                    }
                    foreach (var lr in c.InterSignaalGroep.LateReleases)
                    {
                        sb.AppendLine($"{ts}if ((P[{_fcpf}{lr:naar}] & BIT11) && R[{_fcpf}{lr:van}] && !kp({_fcpf}{lr:van}) && A[{_fcpf}{lr:van}]) {{ PAR[{_fcpf}{lr:van}] |= BIT11; P[{_fcpf}{lr:van}] |= BIT11; }}");
                    }
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    if (!timingsModel.TimingsUsePredictions) return null;
                    var fcf = c.Fasen.First().Naam;
                    sb.AppendLine($"{ts}#ifndef NO_TIMETOX");
                    sb.AppendLine($"{ts}{ts}/* UC4 */");
                    sb.AppendLine($"{ts}{ts}/* eigenlijk nog per richting een schakelaar of er altijd NG moet worden gestuurd (nu is het een algemene schakelaar) */");
                    sb.AppendLine($"{ts}{ts}for (i = 0; i < FCMAX; ++i)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}timings_uc4({_fcpf}{fcf} + i, {_mpf}{_mrealtijdmin}{fcf} + i, {_mpf}{_mrealtijdmax}{fcf} + i, {_prmpf}{_prmttxconfidence15}, {_schpf}{_schtxconfidence15ar});");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}#endif");  
                    return sb.ToString();
                    
                case CCOLCodeTypeEnum.RegCRealisatieAfhandeling:
                    sb.AppendLine($"{ts}#ifndef NO_TIMETOX");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schconfidence15fix}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}reset_rr_fc(BIT11);");
                    sb.AppendPerFase(c, _fcpf, $"{ts}{ts}if (R[<FC>] && (P[<FC>] & BIT11)) set_rr_gk(<FC>, BIT11);");
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        var sch = "";
                        if (gs.Schakelbaar != AltijdAanUitEnum.Altijd)
                        {
                            sch = $"SCH[{_schpf}{_schgs}{gs:van}{gs:naar}] && ";
                        }
                        sb.AppendLine($"{ts}{ts}if ({sch}R[{_fcpf}{gs:van}] && (P[{_fcpf}{gs:van}] & BIT11)) set_rr_gk({_fcpf}{gs:naar}, BIT11);");
                        sb.AppendLine($"{ts}{ts}if ({sch}R[{_fcpf}{gs:naar}] && (P[{_fcpf}{gs:naar}] & BIT11)) set_rr_gk({_fcpf}{gs:van}, BIT11);");
                    }
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}{ts}if (R[{_fcpf}{vs:naar}] && (P[{_fcpf}{vs:naar}] & BIT11)) set_rr_gk({_fcpf}{vs:van}, BIT11);");
                    }
                    foreach (var lr in c.InterSignaalGroep.LateReleases)
                    {
                        sb.AppendLine($"{ts}{ts}if (R[{_fcpf}{lr:naar}] && (P[{_fcpf}{lr:naar}] & BIT11)) set_rr_gk({_fcpf}{lr:van}, BIT11);");
                    }
                    sb.AppendPerFase(c, _fcpf, $"{ts}{ts}if (R[<FC>] && (P[<FC>] & BIT11)) A[<FC>] |= BIT11;");
                    sb.AppendPerFase(c, _fcpf, $"{ts}{ts}if (R[<FC>] && (P[<FC>] & BIT11) && !RA[<FC>] && !kaa(<FC>)) AA[<FC>] |= BIT11;");

                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc) YM[fc] &= ~BIT11;");
                    sb.AppendLine();
                    
                    // Overige zaken
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        var sch = "";
                        if (gs.Schakelbaar != AltijdAanUitEnum.Altijd)
                        {
                            sch = $"SCH[{_schpf}{_schgs}{gs:van}{gs:naar}] && ";
                        }
                        sb.AppendLine($"{ts}{ts}if ({sch}RA[{_fcpf}{gs:van}] && (P[{_fcpf}{gs:van}] & BIT11) && !kaa({_fcpf}{gs:naar}) && A[{_fcpf}{gs:naar}] && !RR[{_fcpf}{gs:naar}]) AA[{_fcpf}{gs:naar}] |= BIT11;");
                        sb.AppendLine($"{ts}{ts}if ({sch}RA[{_fcpf}{gs:naar}] && (P[{_fcpf}{gs:naar}] & BIT11) && !kaa({_fcpf}{gs:van}) && A[{_fcpf}{gs:van}] && !RR[{_fcpf}{gs:van}]) AA[{_fcpf}{gs:van}] |= BIT11;");
                        sb.AppendLine($"{ts}{ts}if ({sch}R[{_fcpf}{gs:van}] && !PG[{_fcpf}{gs:van}] && R[{_fcpf}{gs:naar}] && PG[{_fcpf}{gs:naar}]) PG[{_fcpf}{gs:van}] = 0;");
                        sb.AppendLine($"{ts}{ts}if ({sch}R[{_fcpf}{gs:naar}] && !PG[{_fcpf}{gs:naar}] && R[{_fcpf}{gs:van}] && PG[{_fcpf}{gs:van}]) PG[{_fcpf}{gs:naar}] = 0;");
                        if (!gs.DeelConflict)
                        {
                            sb.AppendLine($"{ts}{ts}if ({sch}R[{_fcpf}{gs:naar}] && (P[{_fcpf}{gs:naar}] & BIT11)) YM[{_fcpf}{gs:van}] |= BIT11;");
                            sb.AppendLine($"{ts}{ts}if ({sch}R[{_fcpf}{gs:van}] && (P[{_fcpf}{gs:van}] & BIT11)) YM[{_fcpf}{gs:naar}] |= BIT11;");
                        }
                    }

                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}{ts}if (R[{_fcpf}{vs:naar}] && (P[{_fcpf}{vs:naar}] & BIT11)) YM[{_fcpf}{vs:van}] |= BIT11;");
                        if (c.InterSignaalGroep.Gelijkstarten.Any(x => x.FaseNaar == vs.FaseVan || x.FaseVan == vs.FaseVan))
                        {
                            sb.AppendLine($"{ts}{ts}if (R[{_fcpf}{vs:naar}] && !PG[{_fcpf}{vs:naar}] && R[{_fcpf}{vs:van}] && PG[{_fcpf}{vs:van}]) PG[{_fcpf}{vs:van}] = 0;"); 
                        }
                    }
                    
                    foreach (var lr in c.InterSignaalGroep.LateReleases)
                    {
                        sb.AppendLine($"{ts}{ts}if (R[{_fcpf}{lr:naar}] && (P[{_fcpf}{lr:naar}] & BIT11)) YM[{_fcpf}{lr:van}] |= BIT11;");
                        if (c.InterSignaalGroep.Gelijkstarten.Any(x => x.FaseNaar == lr.FaseVan || x.FaseVan == lr.FaseVan))
                        {
                            sb.AppendLine($"{ts}{ts}if (R[{_fcpf}{lr:naar}] && !PG[{_fcpf}{lr:naar}] && R[{_fcpf}{lr:van}] && PG[{_fcpf}{lr:van}]) PG[{_fcpf}{lr:van}] = 0;"); 
                        }
                    }
                    
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}#endif");  
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.PrioCRijTijdScenario:
                    sb.AppendLine("#ifndef NO_TIMETOX");
                    foreach (var ing in c.PrioData.PrioIngrepen)
                    {
                        sb.AppendLine($"{ts}if ((P[{_fcpf}{ing.FaseCyclus}] & BIT11) && C[{_ctpf}{_cvc}{ing.FaseCyclus}{ing.Naam}] && (iRijTimer[prioFC{ing.FaseCyclus}{ing.Naam}] < iRijTijd[prioFC{ing.FaseCyclus}{ing.Naam}])) iRijTijd[prioFC{ing.FaseCyclus}{ing.Naam}] = 0;");
                    }
                    sb.AppendLine("#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCTegenhoudenConflicten:
                    sb.AppendLine("#ifndef NO_TIMETOX");
                    var done = new List<string>();
                    foreach (var ing in c.PrioData.PrioIngrepen)
                    {
                        if (done.Contains(ing.FaseCyclus)) continue;
                        done.Add(ing.FaseCyclus);
                        sb.AppendLine($"{ts}if (SCH[{_schpf}{_schconfidence15fix}] && (P[{_fcpf}{ing.FaseCyclus}] & BIT11)) {{");
                        sb.AppendLine($"{ts}{ts}RR[{_fcpf}{ing.FaseCyclus}] &= ~PRIO_RR_BIT;");
                        foreach (var gs in c.GetGelijkstarten(ing.FaseCyclus))
                        {
                            var otherFc = gs.FaseVan == ing.FaseCyclus ? gs.FaseNaar : gs.FaseVan;
                            if (gs.Schakelbaar != AltijdAanUitEnum.Altijd)
                            {
                                sb.Append($"SCH[{_schpf}{_schgs}{gs:van}{gs:naar}] && ");
                            }
                            sb.AppendLine($"{ts}{ts}RR[{_fcpf}{otherFc}] &= ~PRIO_RR_BIT;");
                        }
                        foreach (var vs in c.GetVoorstartenNaar(ing.FaseCyclus))
                        {
                            var otherFc = vs.FaseVan == ing.FaseCyclus ? vs.FaseNaar : vs.FaseVan;
                            sb.AppendLine($"{ts}{ts}RR[{_fcpf}{otherFc}] &= ~PRIO_RR_BIT;");
                        }
                        foreach (var lr in c.GetLateReleasesNaar(ing.FaseCyclus))
                        {
                            var otherFc = lr.FaseVan == ing.FaseCyclus ? lr.FaseNaar : lr.FaseVan;
                            sb.AppendLine($"{ts}{ts}RR[{_fcpf}{otherFc}] &= ~PRIO_RR_BIT;");
                        }
                        sb.AppendLine($"{ts}}}");
                    }

                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        sb.AppendLine($"{ts}{ts}if (SCH[{_schpf}{_schconfidence15fix}] && SCH[{_schpf}{_schgs}{gs:van}{gs:naar}] && (P[{_fcpf}{gs:van}] & BIT11)) {{ RR[{_fcpf}{gs:naar}] &= ~PRIO_RR_BIT; }}");
                        sb.AppendLine($"{ts}{ts}if (SCH[{_schpf}{_schconfidence15fix}] && SCH[{_schpf}{_schgs}{gs:van}{gs:naar}] && (P[{_fcpf}{gs:naar}] & BIT11)) {{ RR[{_fcpf}{gs:van}] &= ~PRIO_RR_BIT; }}");
                    }
                    sb.AppendLine("#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCAfkappen:
                    sb.AppendLine($"{ts}#ifndef NO_TIMETOX");
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}{ts}if (R[{_fcpf}{vs:naar}] && (P[{_fcpf}{vs:naar}] & BIT11)) Z[{_fcpf}{vs:van}] &=~BIT6;");
                    }
                    foreach (var lr in c.InterSignaalGroep.LateReleases)
                    {
                        sb.AppendLine($"{ts}{ts}if (R[{_fcpf}{lr:naar}] && (P[{_fcpf}{lr:naar}] & BIT11)) Z[{_fcpf}{lr:van}] &=~BIT6;");
                    }
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCPARCorrecties:
                    sb.AppendLine($"{ts}#ifndef NO_TIMETOX");
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        sb.Append($"{ts}if (");
                        if (gs.Schakelbaar != AltijdAanUitEnum.Altijd)
                        {
                            sb.Append($"SCH[{_schpf}{_schgs}{gs:van}{gs:naar}] && ");
                        }
                        sb.AppendLine($"(P[{_fcpf}{gs:van}] & BIT11) && R[{_fcpf}{gs:naar}] && !kp({_fcpf}{gs:naar}) && A[{_fcpf}{gs:naar}]) {{ PAR[{_fcpf}{gs:naar}] |= BIT11; P[{_fcpf}{gs:naar}] |= BIT11; }}");
                        sb.Append($"{ts}if (");
                        if (gs.Schakelbaar != AltijdAanUitEnum.Altijd)
                        {
                            sb.Append($"SCH[{_schpf}{_schgs}{gs:van}{gs:naar}] && ");
                        }
                        sb.AppendLine($"(P[{_fcpf}{gs:naar}] & BIT11) && R[{_fcpf}{gs:van}] && !kp({_fcpf}{gs:van}) && A[{_fcpf}{gs:van}]) {{ PAR[{_fcpf}{gs:van}] |= BIT11; P[{_fcpf}{gs:van}] |= BIT11; }}");
                    }
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}if ((P[{_fcpf}{vs:naar}] & BIT11) && R[{_fcpf}{vs:van}] && !kp({_fcpf}{vs:van}) && A[{_fcpf}{vs:van}]) {{ PAR[{_fcpf}{vs:van}] |= BIT11; P[{_fcpf}{vs:van}] |= BIT11; }}");
                    }
                    foreach (var lr in c.InterSignaalGroep.LateReleases)
                    {
                        sb.AppendLine($"{ts}if ((P[{_fcpf}{lr:naar}] & BIT11) && R[{_fcpf}{lr:van}] && !kp({_fcpf}{lr:van}) && A[{_fcpf}{lr:van}]) {{ PAR[{_fcpf}{lr:van}] |= BIT11; P[{_fcpf}{lr:van}] |= BIT11; }}");
                    }
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                    
                default:
                    return null;
            }
        }
        
        internal void GenerateFcTimingsC(ControllerModel c, TimingsDataModel model, string ts)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* DEFINITIE FCTMING FUNCTIES */");
            sb.AppendLine("/* -------------------------- */");
            sb.AppendLine();
            sb.Append(CCOLHeaderGenerator.GenerateFileHeader(c.Data, "fctimings.c"));
            sb.AppendLine();
            sb.Append(CCOLHeaderGenerator.GenerateVersionHeader(c.Data));
            sb.AppendLine();
            sb.AppendLine("/* DEFINITIE EVENTSTATE */");
            sb.AppendLine("/* ==================== */");
            sb.AppendLine();
            sb.AppendLine("#ifdef EVENTSTATE_MACRODEFINITIES_CIF_INC");
            sb.AppendLine();
            sb.AppendLine("/* Macrodefinities status EVENTSTATE (Nederlands) */");
            sb.AppendLine("/* ---------------------------------------------- */");
            sb.AppendLine("#define CIF_TIMING_ONBEKEND           0    /* Unknown(0)                             */");
            sb.AppendLine("#define CIF_TIMING_GEDOOFD            1    /* Dark(1)                                */");
            sb.AppendLine("#define CIF_TIMING_ROOD_KNIPPEREN     2    /* stop - Then - Proceed(2)               */");
            sb.AppendLine("#define CIF_TIMING_ROOD               3    /* stop - And - Remain(3)                 */");
            sb.AppendLine("#define CIF_TIMING_GROEN_OVERGANG     4    /* pre - Movement(4) - not used in NL     */");
            sb.AppendLine("#define CIF_TIMING_GROEN_DEELCONFLICT 5    /* permissive - Movement - Allowed(5)     */");
            sb.AppendLine("#define CIF_TIMING_GROEN              6    /* protected - Movement - Allowed(6)      */");
            sb.AppendLine("#define CIF_TIMING_GEEL_DEELCONFLICT  7    /* permissive - clearance(7)              */");
            sb.AppendLine("#define CIF_TIMING_GEEL               8    /* protected - clearance(8)               */");
            sb.AppendLine("#define CIF_TIMING_GEEL_KNIPPEREN     9    /* caution - Conflicting - Traffic(9)     */");
            sb.AppendLine("#define CIF_TIMING_GROEN_KNIPPEREN_DEELCONFLICT 10    /* permissive - Movement - PreClearance - not in J2735 */");
            sb.AppendLine("#define CIF_TIMING_GROEN_KNIPPEREN              11    /* protected -  Movement - PreClearance - not in J2735 */");
            sb.AppendLine();
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine($"/* De functie kr52_Eventstate_Definition() definieert de eventstate voor de fasecycli.");
            sb.AppendLine($" * De functie kr52_Eventstate_Definition() wordt aangeroepn door de functie control_parameters().");
            sb.AppendLine($" */");
            sb.AppendLine($"void Timings_Eventstate_Definition(void)");
            sb.AppendLine($"{{");
            sb.AppendLine($"{ts}register count i;");
            sb.AppendLine();
            sb.AppendLine($"{ts}/* Zet defaultwaarde */");
            sb.AppendLine($"{ts}/* ----------------- */");
            sb.AppendLine($"{ts}for (i = 0; i < FCMAX; i++)");
            sb.AppendLine($"{ts}{{");
            sb.AppendLine($"{ts}{ts}CCOL_FC_EVENTSTATE[i][CIF_ROOD]= CIF_TIMING_ONBEKEND;       /* Rood   */");
            sb.AppendLine($"{ts}{ts}CCOL_FC_EVENTSTATE[i][CIF_GROEN]= CIF_TIMING_ONBEKEND;      /* Groen  */");
            sb.AppendLine($"{ts}{ts}CCOL_FC_EVENTSTATE[i][CIF_GEEL]= CIF_TIMING_ONBEKEND;       /* Geel   */");
            sb.AppendLine($"{ts}}}");
            sb.AppendLine();
            foreach(var fc in model.TimingsFasen)
            {
                sb.AppendLine($"/* Fase {fc.FaseCyclus} */");
                var fcfc = c.Fasen.FirstOrDefault(x => x.Naam == fc.FaseCyclus);
                switch (fc.ConflictType)
                {
                    case TimingsFaseCyclusTypeEnum.Conflictvrij:
                        sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_ROOD]= CIF_TIMING_ROOD;       /* Rood   */");
                        sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_GROEN]= CIF_TIMING_GROEN;      /* Groen  */");
                        if (fcfc != null)
                        {
                            sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_GEEL]= CIF_TIMING_GEEL;       /* Geel   */");
                        }
                        break;
                    case TimingsFaseCyclusTypeEnum.Deelconflict:
                        sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_ROOD]= CIF_TIMING_ROOD;       /* Rood   */");
                        sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_GROEN]= CIF_TIMING_GROEN_DEELCONFLICT;      /* Groen  */");
                        if (fcfc != null)
                        {
                            sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_GEEL]= CIF_TIMING_GEEL_DEELCONFLICT;       /* Geel   */");
                        }
                        break;
                }
                sb.AppendLine();
            }
            sb.AppendLine("}");

            File.WriteAllText(Path.Combine(Path.GetDirectoryName(DataAccess.TLCGenControllerDataProvider.Default.ControllerFileName) ?? throw new NullReferenceException(), $"{c.Data.Naam}fctimings.c"), sb.ToString(), Encoding.Default);
        }
    }
}