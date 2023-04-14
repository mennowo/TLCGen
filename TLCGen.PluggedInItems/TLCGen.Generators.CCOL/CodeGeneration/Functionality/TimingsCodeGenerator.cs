using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Plugins.Timings.CodeGeneration
{
    [CCOLCodePieceGenerator]
    public class TimingsCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _prmttxconfidence15;
        private CCOLGeneratorCodeStringSettingModel _schconfidence15fix;
        private CCOLGeneratorCodeStringSettingModel _schtxconfidence15ar;
        private CCOLGeneratorCodeStringSettingModel _schspatconfidence1;
        private CCOLGeneratorCodeStringSettingModel _schspatconfidence3;
        private CCOLGeneratorCodeStringSettingModel _schspatconfidence6;
        private CCOLGeneratorCodeStringSettingModel _schspatconfidence9;
        private CCOLGeneratorCodeStringSettingModel _schspatconfidence12;
        private CCOLGeneratorCodeStringSettingModel _schspatconfidence15;
        private CCOLGeneratorCodeStringSettingModel _schtimings;
        private CCOLGeneratorCodeStringSettingModel _prmlatencyminendsg;
#pragma warning restore 0649

        public string _mrealtijd;
        public string _mrealtijdmin;
        public string _mrealtijdmax;
        public string _cvc;
        public string _cvchd;
        public string _schgs;
        public string _tfo;

        public override void CollectCCOLElements(ControllerModel c)
        {
            if (c.Data.CCOLVersie < CCOLVersieEnum.CCOL110 || !c.TimingsData.TimingsToepassen)
            {
                _myElements = new List<CCOLElement>();
                return;
            }
            
            _myElements = new List<CCOLElement>
            {
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmttxconfidence15}", 30, CCOLElementTimeTypeEnum.None, _prmttxconfidence15),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schconfidence15fix}", 0, CCOLElementTimeTypeEnum.SCH_type, _schconfidence15fix),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schtxconfidence15ar}", 1, CCOLElementTimeTypeEnum.SCH_type, _schtxconfidence15ar),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schspatconfidence1}", 0, CCOLElementTimeTypeEnum.SCH_type, _schspatconfidence1),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schspatconfidence3}", 0, CCOLElementTimeTypeEnum.SCH_type, _schspatconfidence3),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schspatconfidence6}", 0, CCOLElementTimeTypeEnum.SCH_type, _schspatconfidence6),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schspatconfidence9}", 0, CCOLElementTimeTypeEnum.SCH_type, _schspatconfidence9),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schspatconfidence12}", 0, CCOLElementTimeTypeEnum.SCH_type, _schspatconfidence12),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schspatconfidence15}", 1, CCOLElementTimeTypeEnum.SCH_type, _schspatconfidence15),
                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmlatencyminendsg}", 3, CCOLElementTimeTypeEnum.TE_type, _prmlatencyminendsg)
            };

            foreach (var fase in c.Fasen)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schtimings}{fase.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schtimings, fase.Naam));
            }
        }
        
        public override bool HasCCOLElements() => true;

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAlternatieven: return new []{50};
                case CCOLCodeTypeEnum.RegCRealisatieAfhandeling: return new []{30};
                case CCOLCodeTypeEnum.RegCIncludes: return new []{120};
                case CCOLCodeTypeEnum.RegCSystemApplication2: return new []{120};
                case CCOLCodeTypeEnum.TabCIncludes: return new []{120};
                case CCOLCodeTypeEnum.TabCControlParameters: return new []{120};
                case CCOLCodeTypeEnum.PrioCRijTijdScenario: return new []{10};
                case CCOLCodeTypeEnum.PrioCTegenhoudenConflicten: return new []{40};
                case CCOLCodeTypeEnum.PrioCAfkappen: return new []{20};
                case CCOLCodeTypeEnum.PrioCPARCorrecties: return new []{40}; 
                case CCOLCodeTypeEnum.PrioCPostAfhandelingPrio: return new[] {30};
                default:
                    return null;
            }
        }
        
        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCSystemApplication2:
                    if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110 && c.TimingsData.TimingsToepassen)
                        return new List<CCOLLocalVariable> 
                        {
                            new("int", "i", defineCondition: "(!defined NO_TIMETOX)"),
                            new("int", "fc", defineCondition: "(!defined NO_TIMETOX)")
                        };
                    return base.GetFunctionLocalVariables(c, type);
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            if (c.Data.CCOLVersie < CCOLVersieEnum.CCOL110 || !c.TimingsData.TimingsToepassen) return null;

            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:
                    sb.AppendLine($"{ts}#ifndef NO_TIMETOX");
                    sb.AppendLine($"{ts}#include \"timingsvar.c\" /* FCTiming functies */");
                    sb.AppendLine($"{ts}#include \"timingsfunc.c\" /* FCTiming functies */");
                    if (c.TimingsData.TimingsUsePredictions)
                    {
                        sb.AppendLine($"{ts}#include \"timings_uc4.c\" /* FCTiming functies */");
                    }

                    sb.AppendLine($"{ts}#include \"{c.Data.Naam}fctimings.c\" /* FCTiming functies */");
                    sb.AppendLine($"{ts}#endif /* NO_TIMETOX */");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSystemApplication2:

                    if (!c.TimingsData.TimingsToepassen || !c.TimingsData.TimingsUsePredictions) return null;

                    var fcf = c.Fasen.First().Naam;
                    sb.AppendLine($"{ts}#ifndef NO_TIMETOX");
                    sb.AppendLine($"{ts}{ts}/* UC4 */");
                    sb.AppendLine($"{ts}{ts}/* eigenlijk nog per richting een schakelaar of er altijd NG moet worden gestuurd (nu is het een algemene schakelaar) */");
                    if (c.Data.RangeerData.RangerenFasen)
                    {
                        foreach (var fc in c.Data.RangeerData.RangeerFasen.OrderBy(x => x.RangeerIndex))
                        {
                            sb.AppendLine($"{ts}{ts}timings_uc4({_fcpf}{fc.Naam}, {_mpf}{_mrealtijd}{fc.Naam}, {_mpf}{_mrealtijdmin}{fc.Naam}, {_mpf}{_mrealtijdmax}{fc.Naam}, {_prmpf}{_prmttxconfidence15}, {_schpf}{_schtxconfidence15ar}, {_schpf}{_schtimings}{fc.Naam});");
                        }
                        sb.AppendLine($"{ts}{ts}if (!SCH[{_schpf}{_schconfidence15fix}])");
                        sb.AppendLine($"{ts}{ts}{{");

                        foreach (var fc in c.Data.RangeerData.RangeerFasen.OrderBy(x => x.RangeerIndex))
                        {
                            sb.AppendLine($"{ts}{ts}{ts}P[{_fcpf}{fc.Naam}] &= ~BIT11;");
                        }
                        sb.AppendLine($"{ts}{ts}}}");
                    }
                    else
                    {
                        sb.AppendLine($"{ts}{ts}for (i = 0; i < FCMAX; ++i)");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}timings_uc4({_fcpf}{fcf} + i, {_mpf}{_mrealtijd}{fcf} + i, {_mpf}{_mrealtijdmin}{fcf} + i, {_mpf}{_mrealtijdmax}{fcf} + i, {_prmpf}{_prmttxconfidence15}, {_schpf}{_schtxconfidence15ar}, {_schpf}{_schtimings}{fcf} + i);");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine($"{ts}{ts}if (!SCH[{_schpf}{_schconfidence15fix}])");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}for (i = 0; i < FCMAX; ++i)");
                        sb.AppendLine($"{ts}{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}{ts}P[{_fcpf}{fcf} + i] &= ~BIT11;");
                        sb.AppendLine($"{ts}{ts}{ts}}}");
                        sb.AppendLine($"{ts}{ts}}}");
                    }

                    var groenSyncData = GroenSyncDataModel.ConvertSyncFuncToRealFunc(c);
                    if (groenSyncData.FictieveConflicten.Any())
                    {
                        sb.AppendLine();
                        foreach (var fo in groenSyncData.FictieveConflicten)
                        {
                            sb.AppendLine($"{ts}{ts}if (RT[{_tpf}{_tfo}{fo:vannaar}] || T[{_tpf}{_tfo}{fo:vannaar}]) {{ P[{_fcpf}{fo:van}] &= ~BIT11; P[{_fcpf}{fo:naar}] &= ~BIT11; }}");
                        }
                    }

                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}pre_msg_fctiming();");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}msg_fctiming(PRM[{_prmpf}{_prmlatencyminendsg}]);");
                    sb.AppendLine();

                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.EindeGroen))
                    {
                        sb.AppendLine($"        /* Voedende richting {_fcpf}{nl:van} alleen P als naloop een P heeft of al groen is */");
                        sb.AppendLine($"        if (!((P[{_fcpf}{nl:naar}] & BIT11) || G[{_fcpf}{nl:van}]) && (P[{_fcpf}{nl:van}] & BIT11)) P[{_fcpf}{nl:van}] &= ~BIT11;");
                        sb.AppendLine($"        /* P doorzetten */");
                        sb.AppendLine($"        if ((P[{_fcpf}{nl:van}] & BIT11) && R[{_fcpf}{nl:van}]) P[{_fcpf}{nl:naar}] |= BIT11;");    
                        sb.AppendLine();
                    }
                    sb.AppendLine($"{ts}#endif");
                    sb.AppendLine();
                    sb.AppendLine("#if !(defined NO_TIMETOX) && !defined NO_TIMINGS_PRINT && (!defined (AUTOMAAT) || defined (VISSIM)) && !defined AUTOMAAT_TEST");
                    sb.AppendLine($"{ts}if (display) {{");
                    sb.AppendLine($"{ts}{ts}xyprintf( 92, 8 + FC_MAX,\"-----\");");
                    sb.AppendLine($"{ts}{ts}xyprintf( 98, 8 + FC_MAX,\"STATE\");");
                    sb.AppendLine($"{ts}{ts}xyprintf(104, 8 + FC_MAX,\"MINEND\");");
                    sb.AppendLine($"{ts}{ts}xyprintf(111, 8 + FC_MAX,\"MAXEND\");");
                    sb.AppendLine($"{ts}{ts}xyprintf(118, 8 + FC_MAX,\"LIKELY\");");
                    sb.AppendLine($"{ts}{ts}xyprintf(125, 8 + FC_MAX,\"CONF\");");
                    sb.AppendLine($"{ts}{ts}xyprintf(130, 8 + FC_MAX,\"MASK\");");
                    sb.AppendLine($"{ts}{ts}xyprintf(135, 8 + FC_MAX,\"START\");");
                    sb.AppendLine($"{ts}{ts}for (i = 0; i < FC_MAX; i++) {{");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf( 92, 9 + FC_MAX + i, \"fc%3s\", FC_code[i]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf( 98, 9 + FC_MAX + i, \"%5d\", CIF_FC_TIMING[i][0][CIF_TIMING_EVENTSTATE]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(104, 9 + FC_MAX + i, \"%6d\", CIF_FC_TIMING[i][0][CIF_TIMING_MINENDTIME]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(111, 9 + FC_MAX + i, \"%6d\", CIF_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(118, 9 + FC_MAX + i, \"%6d\", CIF_FC_TIMING[i][0][CIF_TIMING_LIKELYTIME]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(125, 9 + FC_MAX + i, \"%4d\", CIF_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(130, 9 + FC_MAX + i, \"%4d\", CIF_FC_TIMING[i][0][CIF_TIMING_MASK]);");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(135, 9 + FC_MAX + i, \"%5d\", CIF_FC_TIMING[i][0][CIF_TIMING_STARTTIME]);");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine("#endif");

                    sb.AppendLine();
                    sb.AppendLine($"{ts}#if !defined NO_TIMETOX");
                    sb.AppendLine($"{ts}#if !defined NO_RIS");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    var tigto = c.Data.Intergroen ? "TIG" : "TO";
                    foreach (var prio in c.PrioData.PrioIngrepen)
                    {
                        var reden = prio.Type switch
                        {
                            PrioIngreepVoertuigTypeEnum.Tram => "CIF_FC_RWT_OV_INGREEP",
                            PrioIngreepVoertuigTypeEnum.Bus => "CIF_FC_RWT_OV_INGREEP",
                            PrioIngreepVoertuigTypeEnum.Fiets => "CIF_FC_RWT_FIETS_PELOTON_INGREEP",
                            PrioIngreepVoertuigTypeEnum.Vrachtwagen => "CIF_FC_RWT_VRACHTVERKEER_INGREEP",
                            PrioIngreepVoertuigTypeEnum.Auto => "CIF_FC_RWT_VOERTUIG_PELOTON_INGREEP",
                            PrioIngreepVoertuigTypeEnum.NG => "CIF_FC_RWT_ONBEKEND",
                            _ => throw new NotImplementedException(),
                        };
                        sb.AppendLine($"{ts}{ts}if (C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, prio)}] && R[fc] && {tigto}[{_fcpf}{prio.FaseCyclus}][fc]) CIF_FC_RWT[fc] |= {reden};");
                    }
                    foreach (var hd in c.PrioData.HDIngrepen)
                    {
                        sb.AppendLine($"{ts}{ts}if (C[{_ctpf}{_cvchd}{hd.FaseCyclus}] && R[fc] && {tigto}[{_fcpf}{hd.FaseCyclus}][fc]) CIF_FC_RWT[fc] |= CIF_FC_RWT_HULPDIENST_INGREEP;");
                    }
                    sb.AppendLine($"{ts}{ts}if (SG[fc]) CIF_FC_RWT[fc] = 0;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}#endif");
                    sb.AppendLine($"{ts}#endif");

                    return sb.ToString();

                case CCOLCodeTypeEnum.TabCIncludes:
                    sb.AppendLine($"{ts}void Timings_Eventstate_Definition(void);");
                    return sb.ToString();

                case CCOLCodeTypeEnum.TabCControlParameters:
                    sb.AppendLine($"{ts}#ifndef NO_TIMETOX");
                    sb.AppendLine($"{ts}Timings_Eventstate_Definition();");
                    sb.AppendLine($"{ts}#endif /* NO_TIMETOX */");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCAlternatieven:
                    if (!c.TimingsData.TimingsUsePredictions) return null;

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

                case CCOLCodeTypeEnum.RegCRealisatieAfhandeling:
                    if (!c.TimingsData.TimingsUsePredictions) return null;

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
                            sb.AppendLine($"{ts}{ts}if ({sch}G[{_fcpf}{gs:van}] && R[{_fcpf}{gs:naar}] && (P[{_fcpf}{gs:naar}] & BIT11)) YM[{_fcpf}{gs:van}] |= BIT11;");
                            sb.AppendLine($"{ts}{ts}if ({sch}G[{_fcpf}{gs:naar}] && R[{_fcpf}{gs:van}] && (P[{_fcpf}{gs:van}] & BIT11)) YM[{_fcpf}{gs:naar}] |= BIT11;");
                        }
                    }

                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        if (c.InterSignaalGroep.Gelijkstarten.Any(x => x.FaseNaar == vs.FaseVan || x.FaseVan == vs.FaseVan))
                        {
                            sb.AppendLine($"{ts}{ts}if (R[{_fcpf}{vs:naar}] && !PG[{_fcpf}{vs:naar}] && R[{_fcpf}{vs:van}] && PG[{_fcpf}{vs:van}]) PG[{_fcpf}{vs:van}] = 0;");
                        }
                        sb.AppendLine($"{ts}{ts}if (G[{_fcpf}{vs:van}] && R[{_fcpf}{vs:naar}] && (P[{_fcpf}{vs:naar}] & BIT11)) YM[{_fcpf}{vs:van}] |= BIT11;");
                    }

                    foreach (var lr in c.InterSignaalGroep.LateReleases)
                    {
                        if (c.InterSignaalGroep.Gelijkstarten.Any(x => x.FaseNaar == lr.FaseVan || x.FaseVan == lr.FaseVan))
                        {
                            sb.AppendLine($"{ts}{ts}if (R[{_fcpf}{lr:naar}] && !PG[{_fcpf}{lr:naar}] && R[{_fcpf}{lr:van}] && PG[{_fcpf}{lr:van}]) PG[{_fcpf}{lr:van}] = 0;");
                        }
                        sb.AppendLine($"{ts}{ts}if (G[{_fcpf}{lr:van}] && R[{_fcpf}{lr:naar}] && (P[{_fcpf}{lr:naar}] & BIT11)) YM[{_fcpf}{lr:van}] |= BIT11;");
                    }

                    var comment1 = false;
                    foreach (var gs1 in c.InterSignaalGroep.Gelijkstarten)
                    {
                        foreach (var gs2 in c.InterSignaalGroep.Gelijkstarten.Cast<IInterSignaalGroepElement>().Concat(c.InterSignaalGroep.Nalopen))
                        {
                            if (gs1 is NaloopModel && gs2 is NaloopModel) continue;
                            
                            string fcA = null;
                            string fcB = null;
                            if (gs1.FaseVan == gs2.FaseVan && gs1.FaseNaar != gs2.FaseNaar)
                            {
                                fcA = gs1.FaseNaar;
                                fcB = gs2.FaseNaar;
                            }
                            else if (gs1.FaseNaar == gs2.FaseVan && gs1.FaseVan != gs2.FaseNaar)
                            {
                                fcA = gs1.FaseVan;
                                fcB = gs2.FaseNaar;
                            }
                            else if (gs1.FaseNaar == gs2.FaseNaar && gs1.FaseVan != gs2.FaseVan)
                            {
                                fcA = gs1.FaseVan;
                                fcB = gs2.FaseVan;
                            }

                            if (fcA == null || fcB == null) continue;

                            var start = $"{ts}{ts}if (";
                            var and = false;
                            if (gs1 is GelijkstartModel gsM1 && gsM1.Schakelbaar != AltijdAanUitEnum.Altijd)
                            {
                                start += $"SCH[{_schpf}{_schgs}{gs1:van}{gs1:naar}]";
                                and = true;
                            }

                            if (gs2 is GelijkstartModel gsM2 && gsM2.Schakelbaar != AltijdAanUitEnum.Altijd)
                            {
                                if (and) start += " && ";
                                start += $"SCH[{_schpf}{_schgs}{gs2:van}{gs2:naar}] && ";
                                and = false;
                            }

                            if (and) start += " && ";

                            if (!comment1)
                            {
                                sb.AppendLine($"{ts}{ts}/* Correctie gelijkstart <> gelijkstart of naloop");
                                sb.AppendLine($"{ts}{ts} * Bij een gelijkstart die een fase deelt met een andere gelijsktart of naloop");
                                sb.AppendLine($"{ts}{ts} * kan de max-end tijd worden verhoogd op start-geel, daarom wordt");
                                sb.AppendLine($"{ts}{ts} * start geel uitgesteld.");
                                sb.AppendLine($"{ts}{ts} */");
                                comment1 = true;
                            }

                            sb.AppendLine($"{start}G[{_fcpf}{fcA}] && R[{_fcpf}{fcB}] && (P[{_fcpf}{fcB}] & BIT11)) YM[{_fcpf}{fcA}] |= BIT11;");
                            sb.AppendLine($"{start}G[{_fcpf}{fcB}] && R[{_fcpf}{fcA}] && (P[{_fcpf}{fcA}] & BIT11)) YM[{_fcpf}{fcB}] |= BIT11;");
                        }
                    }
                    
                    var first = true;
                    foreach (var nl in c.InterSignaalGroep.Nalopen)
                    {
                        switch (nl.Type)
                        {
                            case NaloopTypeEnum.StartGroen:
                                if (first)
                                {
                                    sb.AppendLine($"{ts}{ts} /* YM nalopen P */");
                                    first = false;
                                }
                                sb.AppendLine($"{ts}{ts}if (G[{_fcpf}{nl:van}] && R[{_fcpf}{nl:naar}] && (P[{_fcpf}{nl:naar}] & BIT11)) YM[{_fcpf}{nl:van}] |= BIT11;");
                                sb.AppendLine($"{ts}{ts}if (G[{_fcpf}{nl:naar}] && R[{_fcpf}{nl:van}] && (P[{_fcpf}{nl:van}] & BIT11)) YM[{_fcpf}{nl:naar}] |= BIT11;");
                                break;
                            case NaloopTypeEnum.EindeGroen:
                                if (first)
                                {
                                    sb.AppendLine($"{ts}{ts} /* YM nalopen P */");
                                    first = false;
                                }
                                sb.AppendLine($"{ts}{ts}if (G[{_fcpf}{nl:naar}] && R[{_fcpf}{nl:van}] && (P[{_fcpf}{nl:van}] & BIT11)) YM[{_fcpf}{nl:naar}] |= BIT11;");
                                break;
                            case NaloopTypeEnum.CyclischVerlengGroen:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCRijTijdScenario:
                    if (!c.TimingsData.TimingsUsePredictions) return null;

                    sb.AppendLine("#ifndef NO_TIMETOX");
                    foreach (var ing in c.PrioData.PrioIngrepen)
                    {
                        sb.AppendLine($"{ts}if ((P[{_fcpf}{ing.FaseCyclus}] & BIT11) && C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, ing)}] && (iRijTimer[prioFC{CCOLCodeHelper.GetPriorityName(c, ing)}] < iRijTijd[prioFC{CCOLCodeHelper.GetPriorityName(c, ing)}])) iRijTijd[prioFC{CCOLCodeHelper.GetPriorityName(c, ing)}] = 0;");
                    }

                    foreach (var ing in c.PrioData.HDIngrepen)
                    {
                        sb.AppendLine($"{ts}if ((P[{_fcpf}{ing.FaseCyclus}] & BIT11) && C[{_ctpf}{_cvchd}{ing.FaseCyclus}] && (iRijTimer[hdFC{ing.FaseCyclus}] < iRijTijd[hdFC{ing.FaseCyclus}])) iRijTijd[hdFC{ing.FaseCyclus}] = 0;");
                    }

                    sb.AppendLine("#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCTegenhoudenConflicten:
                    if (!c.TimingsData.TimingsUsePredictions) return null;

                    sb.AppendLine("#ifndef NO_TIMETOX");
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        var sch = "";
                        if (gs.Schakelbaar != AltijdAanUitEnum.Altijd) sch = $" && SCH[{_schpf}{_schgs}{gs:van}{gs:naar}]";
                        sb.AppendLine($"{ts}if (SCH[{_schpf}{_schconfidence15fix}]{sch} && (P[{_fcpf}{gs:van}] & BIT11)) {{ RR[{_fcpf}{gs:naar}] &= ~PRIO_RR_BIT; }}");
                        sb.AppendLine($"{ts}if (SCH[{_schpf}{_schconfidence15fix}]{sch} && (P[{_fcpf}{gs:naar}] & BIT11)) {{ RR[{_fcpf}{gs:van}] &= ~PRIO_RR_BIT; }}");
                    }

                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}if (SCH[{_schpf}{_schconfidence15fix}] && (P[{_fcpf}{vs:naar}] & BIT11)) {{ RR[{_fcpf}{vs:van}] &= ~PRIO_RR_BIT; }}");
                    }

                    foreach (var lr in c.InterSignaalGroep.LateReleases)
                    {
                        sb.AppendLine($"{ts}if (SCH[{_schpf}{_schconfidence15fix}] && (P[{_fcpf}{lr:naar}] & BIT11)) {{ RR[{_fcpf}{lr:van}] &= ~PRIO_RR_BIT; }}");
                    }

                    sb.AppendLine("#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCAfkappen:
                    if (!c.TimingsData.TimingsUsePredictions) return null;

                    sb.AppendLine("#ifndef NO_TIMETOX");
                    sb.AppendLine($"if (SCH[{_schpf}{_schconfidence15fix}])");
                    sb.AppendLine($"{ts}{{");

                    var tsts = ts + ts;
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        var sch = "";
                        if (gs.Schakelbaar != AltijdAanUitEnum.Altijd) sch = $"SCH[{_schpf}{_schgs}{gs:van}{gs:naar}] && ";
                        sb.AppendLine($"{tsts}if ({sch}(P[{_fcpf}{gs:van}] & BIT11)) {{ Z[{_fcpf}{gs:naar}] &= ~PRIO_Z_BIT; }}");
                        sb.AppendLine($"{tsts}if ({sch}(P[{_fcpf}{gs:naar}] & BIT11)) {{ Z[{_fcpf}{gs:van}] &= ~PRIO_Z_BIT; }}");
                    }

                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{tsts}if ((P[{_fcpf}{vs:naar}] & BIT11)) {{ Z[{_fcpf}{vs:van}] &= ~PRIO_Z_BIT; }}");
                    }

                    foreach (var lr in c.InterSignaalGroep.LateReleases)
                    {
                        sb.AppendLine($"{tsts}if ((P[{_fcpf}{lr:naar}] & BIT11)) {{ Z[{_fcpf}{lr:van}] &= ~PRIO_Z_BIT; }}");
                    }

                    var comment = false;
                    foreach (var gs1 in c.InterSignaalGroep.Gelijkstarten)
                    {
                        foreach (var gs2 in c.InterSignaalGroep.Gelijkstarten.Cast<IInterSignaalGroepElement>().Concat(c.InterSignaalGroep.Nalopen))
                        {
                            if (gs1 is NaloopModel && gs2 is NaloopModel) continue;
                            
                            string fcA = null;
                            string fcB = null;
                            if (gs1.FaseVan == gs2.FaseVan && gs1.FaseNaar != gs2.FaseNaar)
                            {
                                fcA = gs1.FaseNaar;
                                fcB = gs2.FaseNaar;
                            }
                            else if (gs1.FaseNaar == gs2.FaseVan && gs1.FaseVan != gs2.FaseNaar)
                            {
                                fcA = gs1.FaseVan;
                                fcB = gs2.FaseNaar;
                            }
                            else if (gs1.FaseNaar == gs2.FaseNaar && gs1.FaseVan != gs2.FaseVan)
                            {
                                fcA = gs1.FaseVan;
                                fcB = gs2.FaseVan;
                            }

                            if (fcA == null || fcB == null) continue;

                            var start = $"{tsts} if (";
                            var and = false;
                            if (gs1.Schakelbaar != AltijdAanUitEnum.Altijd)
                            {
                                start += $"SCH[{_schpf}{_schgs}{gs1:van}{gs1:naar}]";
                                and = true;
                            }

                            if (gs2 is GelijkstartModel gsM2 && gsM2.Schakelbaar != AltijdAanUitEnum.Altijd)
                            {
                                if (and) start += " && ";
                                start += $"SCH[{_schpf}{_schgs}{gs2:van}{gs2:naar}] && ";
                                and = false;
                            }

                            if (and) start += " && ";

                            if (!comment)
                            {
                                sb.AppendLine($"{tsts}/* Correctie gelijkstart <> gelijkstart");
                                sb.AppendLine($"{tsts} * Bij een gelijkstart die een fase deelt met een andere gelijsktart");
                                sb.AppendLine($"{tsts} * kan de max-end tijd worden verhoogd op start-geel, daarom wordt");
                                sb.AppendLine($"{tsts} * start geel uitgesteld.");
                                sb.AppendLine($"{tsts} */");
                                comment = true;
                            }

                            sb.AppendLine($"{start}G[{_fcpf}{fcA}] && R[{_fcpf}{fcB}] && (P[{_fcpf}{fcB}] & BIT11)) Z[{_fcpf}{fcA}] &= ~PRIO_Z_BIT;");
                            sb.AppendLine($"{start}G[{_fcpf}{fcB}] && R[{_fcpf}{fcA}] && (P[{_fcpf}{fcA}] & BIT11)) Z[{_fcpf}{fcB}] &= ~PRIO_Z_BIT;");
                        }
                    }
                    
                    first = true;
                    foreach (var nl in c.InterSignaalGroep.Nalopen)
                    {
                        switch (nl.Type)
                        {
                            case NaloopTypeEnum.StartGroen:
                                if (first)
                                {
                                    //sb.AppendLine($"{ts}{ts} /* YM nalopen P */");
                                    first = false;
                                }
                                sb.AppendLine($"{ts}{ts}if (G[{_fcpf}{nl:van}] && R[{_fcpf}{nl:naar}] && (P[{_fcpf}{nl:naar}] & BIT11)) Z[{_fcpf}{nl:van}] &= ~PRIO_Z_BIT;");
                                sb.AppendLine($"{ts}{ts}if (G[{_fcpf}{nl:naar}] && R[{_fcpf}{nl:van}] && (P[{_fcpf}{nl:van}] & BIT11)) Z[{_fcpf}{nl:naar}] &= ~PRIO_Z_BIT;"); 
                                break;
                            case NaloopTypeEnum.EindeGroen:
                                if (first)
                                {
                                    //sb.AppendLine($"{ts}{ts} /* YM nalopen P */");
                                    first = false;
                                }
                                sb.AppendLine($"{ts}{ts}if (G[{_fcpf}{nl:naar}] && R[{_fcpf}{nl:van}] && (P[{_fcpf}{nl:van}] & BIT11)) Z[{_fcpf}{nl:naar}] &= ~PRIO_Z_BIT;");
                                break;
                            case NaloopTypeEnum.CyclischVerlengGroen:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    sb.AppendLine($"{ts}}}");

                    sb.AppendLine("#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCPARCorrecties:
                    if (!c.TimingsData.TimingsUsePredictions) return null;

                    sb.AppendLine($"{ts}#ifndef NO_TIMETOX");
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        sb.Append($"{ts}if (");
                        if (gs.Schakelbaar != AltijdAanUitEnum.Altijd) sb.Append($"SCH[{_schpf}{_schgs}{gs:van}{gs:naar}] && ");
                        sb.AppendLine($"(P[{_fcpf}{gs:van}] & BIT11) && R[{_fcpf}{gs:naar}] && !kp({_fcpf}{gs:naar}) && A[{_fcpf}{gs:naar}]) {{ PAR[{_fcpf}{gs:naar}] |= BIT11; P[{_fcpf}{gs:naar}] |= BIT11; }}");
                        sb.Append($"{ts}if (");
                        if (gs.Schakelbaar != AltijdAanUitEnum.Altijd) sb.Append($"SCH[{_schpf}{_schgs}{gs:van}{gs:naar}] && ");
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


                case CCOLCodeTypeEnum.PrioCPostAfhandelingPrio:
                    if (!c.TimingsData.TimingsUsePredictions) return null;

                    if (c.InterSignaalGroep.Nalopen.Any())
                    {
                        sb.AppendLine($"{ts}#ifndef NO_TIMETOX");
                        sb.AppendLine($"{ts}/* Niet afkappen naloop richtingen wanneer voedende een P[]&BIT11 heeft */");
                        foreach (var nl in c.InterSignaalGroep.Nalopen)
                        {
                            sb.AppendLine($"{ts}if (P[{_fcpf}{nl:van}] & BIT11) {{");
                            sb.AppendLine($"{ts}{ts} Z[{_fcpf}{nl:naar}] &= ~BIT6;");
                            sb.AppendLine($"{ts}{ts}RR[{_fcpf}{nl:naar}] &= ~(BIT1 | BIT2 | BIT4 | BIT6);");
                            sb.AppendLine($"{ts}{ts}FM[{_fcpf}{nl:naar}] &= ~PRIO_FM_BIT;");
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine($"");
                        }

                        sb.AppendLine($"{ts}#endif // NO_TIMETOX");
                    }

                    if (c.InterSignaalGroep.Voorstarten.Any())
                    {
                        if (c.InterSignaalGroep.Nalopen.Any()) sb.AppendLine();

                        sb.AppendLine($"{ts}#ifndef NO_TIMETOX");
                        sb.AppendLine($"{ts}/* Niet afkappen voorstartende richting wanneer voedende een P[]&BIT11 heeft */");
                        foreach (var vs in c.InterSignaalGroep.Voorstarten)
                        {
                            sb.AppendLine($"{ts}if (P[{_fcpf}{vs:naar}] & BIT11) {{");
                            sb.AppendLine($"{ts}{ts} Z[{_fcpf}{vs:van}] &= ~BIT6;");
                            sb.AppendLine($"{ts}{ts}RR[{_fcpf}{vs:van}] &= ~(BIT1 | BIT2 | BIT4 | BIT6);");
                            sb.AppendLine($"{ts}{ts}FM[{_fcpf}{vs:van}] &= ~PRIO_FM_BIT;");
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine($"");
                        }

                        sb.AppendLine($"{ts}#endif // NO_TIMETOX");
                    }

                    if (c.InterSignaalGroep.LateReleases.Any())
                    {
                        if (c.InterSignaalGroep.Nalopen.Any() || c.InterSignaalGroep.Voorstarten.Any()) sb.AppendLine();

                        sb.AppendLine($"{ts}#ifndef NO_TIMETOX");
                        sb.AppendLine($"{ts}/* Niet afkappen laterelease richting wanneer voedende een P[]&BIT11 heeft */");
                        foreach (var vs in c.InterSignaalGroep.LateReleases)
                        {
                            sb.AppendLine($"{ts}if (P[{_fcpf}{vs:naar}] & BIT11) {{");
                            sb.AppendLine($"{ts}{ts} Z[{_fcpf}{vs:van}] &= ~BIT6;");
                            sb.AppendLine($"{ts}{ts}RR[{_fcpf}{vs:van}] &= ~(BIT1 | BIT2 | BIT4 | BIT6);");
                            sb.AppendLine($"{ts}{ts}FM[{_fcpf}{vs:van}] &= ~PRIO_FM_BIT;");
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine($"");
                        }

                        sb.AppendLine($"{ts}#endif // NO_TIMETOX");
                    }

                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _mrealtijd = CCOLGeneratorSettingsProvider.Default.GetElementName("mrealtijd");
            _mrealtijdmin = CCOLGeneratorSettingsProvider.Default.GetElementName("mrealtijdmin");
            _mrealtijdmax = CCOLGeneratorSettingsProvider.Default.GetElementName("mrealtijdmax");
            _cvc = CCOLGeneratorSettingsProvider.Default.GetElementName("cvc");
            _cvchd = CCOLGeneratorSettingsProvider.Default.GetElementName("cvchd");
            _schgs = CCOLGeneratorSettingsProvider.Default.GetElementName("schgs");
            _tfo = CCOLGeneratorSettingsProvider.Default.GetElementName("tfo");

            return base.SetSettings(settings);
        }
    }
}