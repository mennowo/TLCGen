using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class RISCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _prmrisastart;
        private CCOLGeneratorCodeStringSettingModel _prmrisaend;
        private CCOLGeneratorCodeStringSettingModel _prmrisastartsrm0;
        private CCOLGeneratorCodeStringSettingModel _prmrisaendsrm0;
        private CCOLGeneratorCodeStringSettingModel _prmrisvstart;
        private CCOLGeneratorCodeStringSettingModel _prmrisvend;
        private CCOLGeneratorCodeStringSettingModel _prmrisvstartsrm0;
        private CCOLGeneratorCodeStringSettingModel _prmrisvendsrm0;
        private CCOLGeneratorCodeStringSettingModel _prmrispstart;
        private CCOLGeneratorCodeStringSettingModel _prmrispend;
        private CCOLGeneratorCodeStringSettingModel _prmrislaneid;
        private CCOLGeneratorCodeStringSettingModel _prmrisapproachid;
        private CCOLGeneratorCodeStringSettingModel _prmrislaneheading;
        private CCOLGeneratorCodeStringSettingModel _prmrislaneheadingmarge;
        private CCOLGeneratorCodeStringSettingModel _schrisgeencheckopsg;
        private CCOLGeneratorCodeStringSettingModel _schrisaanvraag;
        private CCOLGeneratorCodeStringSettingModel _schrisverlengen;
#pragma warning restore 0649
        
        private string _prmlijn;
        private string _prmrisrole;
        private string _prmrissubrole;

        public override bool HasSettings()
        {
            return true;
        }

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            var risModel = c.RISData;

            if (risModel.RISToepassen)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                    $"{_schrisgeencheckopsg}",
                    0,
                    CCOLElementTimeTypeEnum.SCH_type,
                    _schrisgeencheckopsg));
                
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                    $"{_schrisaanvraag}",
                    1,
                    CCOLElementTimeTypeEnum.SCH_type,
                    _schrisaanvraag));
                
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                    $"{_schrisverlengen}",
                    1,
                    CCOLElementTimeTypeEnum.SCH_type,
                    _schrisverlengen));

                foreach (var l in risModel.RISFasen.Where(l => l.LaneData.Any()))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisapproachid}{l.FaseCyclus}", l.ApproachID, CCOLElementTimeTypeEnum.None, _prmrisapproachid, l.FaseCyclus));
                }

                foreach (var l in risModel.RISFasen.SelectMany(x => x.LaneData))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}",
                        l.LaneID,
                        CCOLElementTimeTypeEnum.None,
                        _prmrislaneid, l.RijstrookIndex.ToString(), l.SignalGroupName));

                    if (l.UseHeading)
                    {
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmrislaneheading}{l.SignalGroupName}_{l.RijstrookIndex}",
                            l.Heading,
                            CCOLElementTimeTypeEnum.None,
                            _prmrislaneheading, l.RijstrookIndex.ToString(), l.SignalGroupName));

                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmrislaneheadingmarge}{l.SignalGroupName}_{l.RijstrookIndex}",
                            l.HeadingMarge,
                            CCOLElementTimeTypeEnum.None,
                            _prmrislaneheadingmarge, l.RijstrookIndex.ToString(), l.SignalGroupName));
                    }
                }
                foreach (var l in risModel.RISRequestLanes.Where(l => l.RISAanvraag))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmrisastart}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                        l.AanvraagStart,
                        CCOLElementTimeTypeEnum.None,
                        _prmrisastart, l.SignalGroupName));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmrisastartsrm0}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                        l.AanvraagStartSrm0,
                        CCOLElementTimeTypeEnum.None,
                        _prmrisastartsrm0, l.SignalGroupName));
                }
                foreach (var l in risModel.RISRequestLanes.Where(l => l.RISAanvraag))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmrisaend}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                        l.AanvraagEnd,
                        CCOLElementTimeTypeEnum.None,
                        _prmrisaend, l.SignalGroupName));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmrisaendsrm0}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                        l.AanvraagEndSrm0,
                        CCOLElementTimeTypeEnum.None,
                        _prmrisaendsrm0, l.SignalGroupName));
                }
                foreach (var l in risModel.RISExtendLanes.Where(l => l.RISVerlengen))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmrisvstart}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                        l.VerlengenStart,
                        CCOLElementTimeTypeEnum.None,
                        _prmrisvstart, l.SignalGroupName));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmrisvstartsrm0}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                        l.VerlengenStartSrm0,
                        CCOLElementTimeTypeEnum.None,
                        _prmrisvstartsrm0, l.SignalGroupName));
                }
                foreach (var l in risModel.RISExtendLanes.Where(l => l.RISVerlengen))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmrisvend}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                        l.VerlengenEnd,
                        CCOLElementTimeTypeEnum.None,
                        _prmrisvend, l.SignalGroupName));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmrisvendsrm0}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                        l.VerlengenEndSrm0,
                        CCOLElementTimeTypeEnum.None,
                        _prmrisvendsrm0, l.SignalGroupName));
                }
                foreach (var l in risModel.RISPelotonLanes.Where(l => l.RISPelotonBepaling))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmrispstart}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                        l.PelotonBepalingStart,
                        CCOLElementTimeTypeEnum.None,
                        _prmrispstart, l.SignalGroupName));
                }
                foreach (var l in risModel.RISPelotonLanes.Where(l => l.RISPelotonBepaling))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmrispend}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                        l.PelotonBepalingEnd,
                        CCOLElementTimeTypeEnum.None,
                        _prmrispend, l.SignalGroupName));
                }
            }
            var lanesSim = risModel.RISFasen.SelectMany(x => x.LaneData);
            foreach (var s in lanesSim.SelectMany(x => x.SimulatedStations))
            {
                s.StationBitmapData.Naam = s.Naam;
                var e = CCOLGeneratorSettingsProvider.Default.CreateElement(s.Naam, CCOLElementTypeEnum.Ingang, s.StationBitmapData, "", null, null);
                e.Dummy = true;
                _myElements.Add(e);
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _myElements.Where(x => x.Type == type);
        }

                public override bool HasSimulationElements(ControllerModel c)
        {
            return c.RISData.RISToepassen;
        }

        public override IEnumerable<DetectorSimulatieModel> GetSimulationElements(ControllerModel c)
        {
            return c.RISData.RISFasen.SelectMany(x => x.LaneData).SelectMany(x => x.SimulatedStations).Select(x => x.SimulationData);
        }

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            if (c.RISData?.RISToepassen != true) return base.GetFunctionLocalVariables(c, type);
            return type switch
            {
                CCOLCodeTypeEnum.RegCMeetkriterium => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.PrioCInitPrio => new List<CCOLLocalVariable>{new("int", "i")},  
                CCOLCodeTypeEnum.RegCAanvragen => new List<CCOLLocalVariable>{new("int", "fc")},  
                CCOLCodeTypeEnum.PrioCPostAfhandelingPrio =>
                    c.HasPrioRis() ? new List<CCOLLocalVariable> { new("int", "i") } : base.GetFunctionLocalVariables(c, type),  
                _ => base.GetFunctionLocalVariables(c, type)
            };
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.SysHDefines => new []{110},
                CCOLCodeTypeEnum.RegCInitApplication => new []{110},
                CCOLCodeTypeEnum.RegCAanvragen => new []{110},
                CCOLCodeTypeEnum.RegCMeetkriterium => new []{110},
                CCOLCodeTypeEnum.RegCSystemApplication2 => new []{110},
                CCOLCodeTypeEnum.RegCPostSystemApplication => new []{110},
                CCOLCodeTypeEnum.SysHBeforeUserDefines => new []{110},
                CCOLCodeTypeEnum.RegCTop => new []{110},
                CCOLCodeTypeEnum.PrioCInitPrio => new []{20},
                CCOLCodeTypeEnum.PrioCInUitMelden => new []{90},
                CCOLCodeTypeEnum.PrioCPostAfhandelingPrio => new []{20},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var risModel = c.RISData;

            if (!risModel.RISToepassen || c.Data.CCOLVersie < CCOLVersieEnum.CCOL110) return "";

            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.SysHDefines:
                    sb.AppendLine("#define RIS_GEEN_INDEXERING");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCTop:
                    if (c.RISData.RISToepassen && c.Data.CCOLVersie >= CCOLVersieEnum.CCOL120)
                    {
                        sb.AppendLine();
                        sb.AppendLine("#ifndef NO_RIS");
                        sb.AppendLine($"{ts}/* Definitie ProductInformatie ITSinfo */");
                        sb.AppendLine($"{ts}/* ----------------------------------- */");
                        sb.AppendLine($"{ts}const struct Rif_ProductInformation RIF_ITSINFO_AP = {{");
                        sb.AppendLine($"{ts}  \"Gemeente Rotterdam\",    /* manufacturerName   */");
                        sb.AppendLine($"{ts}  \"TLCGen\",                /* certifiedName      */");
                        sb.AppendLine($"{ts}  \"12.0.0\",                /* certifiedVersion   */");
                        sb.AppendLine($"{ts}  \"12.0.0\"                 /* version            */");
                        sb.AppendLine($"{ts}}};");
                        sb.AppendLine("#endif /* NO_RIS */");
                    }
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.PrioCInitPrio:
                    if (!c.HasPrioRis()) return "";
                    
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    sb.AppendLine($"{ts}/* initialisatie variabelen granted_verstrekt */");
                    sb.AppendLine($"{ts}/* ------------------------------------------ */");
                    sb.AppendLine($"{ts}for (i = 0; i < FCMAX; ++i)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}granted_verstrekt[i] = 0;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}#endif /* NO_RIS */");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.PrioCInUitMelden:
                    if (!c.HasPrioRis()) return "";
                    
                    sb.AppendLine($"#ifndef NO_RIS");
                    sb.AppendLine($"{ts}/* Bijhouden granted verstrekt */");
                    sb.AppendLine($"{ts}Bepaal_Granted_Verstrekt();");
                    if (c.PrioData.HDIngrepen.Any(x => x.MeerealiserendeFaseCycli.Any()))
                    {
                        sb.AppendLine();
                        foreach (var hd in c.PrioData.HDIngrepen)
                        {
                            if (hd.MeerealiserendeFaseCycli.Any())
                            {
                                foreach (var fc in hd.MeerealiserendeFaseCycli)
                                {
                                    sb.AppendLine($"{ts}if (granted_verstrekt[{_fcpf}{fc.FaseCyclus}] == 2) granted_verstrekt[{_fcpf}{hd.FaseCyclus}] = 2;");
                                }
                            }
                        }
                    }
                    sb.AppendLine($"#endif /* NO_RIS */");

                    return sb.ToString();
                
                case CCOLCodeTypeEnum.PrioCPostAfhandelingPrio:
                    if (!c.HasPrioRis()) return "";

                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    sb.AppendLine($"{ts}/* nooit einde groen als granted verstrekt */");
                    sb.AppendLine($"{ts}/* --------------------------------------- */");
                    sb.AppendLine($"{ts}for (i = 0; i < FCMAX; ++i)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}if (granted_verstrekt[i] > 0)     /* als granted is verstrekt dan altijd groen aanhouden */");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}if (G[i] && !MG[i]) YV[i] |= PRIO_YV_BIT;");
                    sb.AppendLine($"{ts}{ts}{ts}YM[i] |= PRIO_YM_BIT;");
                    sb.AppendLine($"{ts}{ts}{ts}Z[i] = FALSE;");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}#endif /* NO_RIS */");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.SysHBeforeUserDefines:
                    sb.AppendLine($"/* Systeem naam in het topologiebestand */");
                    sb.AppendLine($"/* ------------------------------------ */");
                    if (risModel.HasMultipleSystemITF)
                    {
                        var i = 1;
                        foreach (var sitf in risModel.MultiSystemITF)
                        {
                            sb.AppendLine($"#define SYSTEM_ITF{i} \"{sitf.SystemITF}\"");
                            ++i;
                        }
                    }
                    else
                    {
                        sb.AppendLine($"#define SYSTEM_ITF \"{risModel.SystemITF}\"");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"/* Definitie lane id in het topologiebestand */");
                    sb.AppendLine($"/* ----------------------------------------- */");
                    sb.AppendLine($"#define ris_conflict_gebied    0 /* connection tussen alle ingress lanes en egress lanes */");
                    return sb.ToString();
                    
                case CCOLCodeTypeEnum.RegCInitApplication:
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    sb.AppendLine($"{ts}{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)");
                    sb.AppendLine($"{ts}{ts}{ts}/* zet display van RIS-berichten aan in de testomgeving */");
                    sb.AppendLine($"{ts}{ts}{ts}/* ---------------------------------------------------- */");
                    sb.AppendLine($"{ts}{ts}{ts}RIS_DIPRM[RIS_DIPRM_ALL] = 1;");
                    sb.AppendLine($"{ts}{ts}#endif");
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCAanvragen:
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    sb.AppendLine($"{ts}{ts}/* RIS aanvragen */");
                    foreach (var l in risModel.RISRequestLanes.Where(x => x.RISAanvraag))
                    {
                        var risfcl = risModel.RISFasen.SelectMany(x => x.LaneData).FirstOrDefault(x => x.SignalGroupName == l.SignalGroupName && x.RijstrookIndex == l.RijstrookIndex);

                        var sitf = "SYSTEM_ITF";
                        if (risModel.HasMultipleSystemITF)
                        {
                            sitf = "SYSTEM_ITF1";
                            if (risfcl != null)
                            {
                                var msitf = risModel.MultiSystemITF.FirstOrDefault(x => x.SystemITF == risfcl.SystemITF);
                                if (msitf != null)
                                {
                                    var i = risModel.MultiSystemITF.IndexOf(msitf);
                                    sitf = $"SYSTEM_ITF{i + 1}";
                                }
                            }
                        }

                        if (risfcl is { UseHeading: true })
                        {
                            sb.AppendLine($"{ts}{ts}if (ris_aanvraag_heading({_fcpf}{l.SignalGroupName}, {sitf}, PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}], RIS_{l.Type}, PRM[{_prmpf}{_prmrisastart}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], PRM[{_prmpf}{_prmrisaend}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], SCH[{_schpf}{_schrisgeencheckopsg}], " +
                                          $"PRM[{_prmpf}{_prmrislaneheading}{l.SignalGroupName}_{l.RijstrookIndex}], " +
                                          $"PRM[{_prmpf}{_prmrislaneheadingmarge}{l.SignalGroupName}_{l.RijstrookIndex}])) A[{_fcpf}{l.SignalGroupName}] |= BIT10;");
                            sb.AppendLine($"{ts}{ts}if (ris_aanvraag_heading({_fcpf}{l.SignalGroupName}, {sitf}, PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}], RIS_{l.Type}, PRM[{_prmpf}{_prmrisastartsrm0}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], PRM[{_prmpf}{_prmrisaendsrm0}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], !SCH[{_schpf}{_schrisgeencheckopsg}], " +
                                          $"PRM[{_prmpf}{_prmrislaneheading}{l.SignalGroupName}_{l.RijstrookIndex}], " +
                                          $"PRM[{_prmpf}{_prmrislaneheadingmarge}{l.SignalGroupName}_{l.RijstrookIndex}])) A[{_fcpf}{l.SignalGroupName}] |= BIT13;");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}{ts}if (ris_aanvraag({_fcpf}{l.SignalGroupName}, {sitf}, PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}], RIS_{l.Type}, PRM[{_prmpf}{_prmrisastart}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], PRM[{_prmpf}{_prmrisaend}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], SCH[{_schpf}{_schrisgeencheckopsg}])) A[{_fcpf}{l.SignalGroupName}] |= BIT10;");
                            sb.AppendLine($"{ts}{ts}if (ris_aanvraag({_fcpf}{l.SignalGroupName}, {sitf}, PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}], RIS_{l.Type}, PRM[{_prmpf}{_prmrisastartsrm0}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], PRM[{_prmpf}{_prmrisaendsrm0}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], !SCH[{_schpf}{_schrisgeencheckopsg}])) A[{_fcpf}{l.SignalGroupName}] |= BIT13;");
                        }
                    }

                    sb.AppendLine($"{ts}/* aanvragen RIS schakelbaar, 1 schakelaar voor het schakelen van alle aanvragen */");
                    sb.AppendLine($"{ts}if (!SCH[schrisaanvraag])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}A[fc] &= ~(BIT10|BIT13);");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}}}");

                    var ovRis = c.PrioData.PrioIngrepen
                        .Where(x => x.MeldingenData.Inmeldingen.Any(x2 => 
                                        x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde) ||
                                    x.MeldingenData.Uitmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))
                        .ToList();
                    var hdRis = c.PrioData.HDIngrepen.Where(x => x.RIS).ToList();
                    if (ovRis.Any() || hdRis.Any())
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{ts}{ts}#ifdef RIS_SSM");
                        sb.AppendLine($"{ts}{ts}{ts}/* Ris PRIO: verstuur SSM */");
                        foreach (var ov in ovRis)
                        {
                            var lijncheck = ov.CheckLijnNummer && ov.LijnNummers.Any()
                                ? $"{_prmpf}{_prmlijn}{CCOLCodeHelper.GetPriorityName(c, ov)}_01, {ov.LijnNummers.Count}"
                                : "NG, NG";
                            sb.AppendLine($"{ts}{ts}{ts}ris_srm_put_signalgroup(" +
                                          $"{_fcpf}{ov.FaseCyclus}, " +
                                          $"PRM[{_prmpf}{_prmrisapproachid}{ov.FaseCyclus}], " +
                                          $"PRM[{_prmpf}{_prmrisrole}{CCOLCodeHelper.GetPriorityName(c, ov)}], " +
                                          $"PRM[{_prmpf}{_prmrissubrole}{CCOLCodeHelper.GetPriorityName(c, ov)}], " +
                                          lijncheck +
                                          ");");
                        }
                        foreach (var ov in ovRis)
                        {
                            sb.AppendLine($"{ts}{ts}{ts}ris_verstuur_ssm(prioFC{CCOLCodeHelper.GetPriorityName(c, ov)});");
                        }
                        foreach (var hd in hdRis)
                        {
                            sb.AppendLine($"{ts}{ts}{ts}ris_verstuur_ssm(hdFC{hd.FaseCyclus});");
                        }
                        sb.AppendLine($"{ts}{ts}#endif");
                    }

                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}MK[fc] &= ~(BIT10|BIT13);");
                    sb.AppendLine($"{ts}}}");
                    foreach (var l in risModel.RISExtendLanes.Where(x => x.RISVerlengen))
                    {
                        var sitf = "SYSTEM_ITF";
                        var risfcl = risModel.RISFasen.SelectMany(x => x.LaneData).FirstOrDefault(x => x.SignalGroupName == l.SignalGroupName && x.RijstrookIndex == l.RijstrookIndex);

                        if (risModel.HasMultipleSystemITF)
                        {
                            sitf = "SYSTEM_ITF1";
                            if (risfcl != null)
                            {
                                var msitf = risModel.MultiSystemITF.FirstOrDefault(x => x.SystemITF == risfcl.SystemITF);
                                if (msitf != null)
                                {
                                    var i = risModel.MultiSystemITF.IndexOf(msitf);
                                    sitf = $"SYSTEM_ITF{i + 1}";
                                }
                            }
                        }
                        if (risfcl is { UseHeading: true })
                        {
                            sb.AppendLine($"{ts}{ts}if (ris_verlengen_heading({_fcpf}{l.SignalGroupName}, {sitf}, PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}], RIS_{l.Type}, PRM[{_prmpf}{_prmrisvstart}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], PRM[{_prmpf}{_prmrisvend}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], SCH[{_schpf}{_schrisgeencheckopsg}], " +
                                          $"PRM[{_prmpf}{_prmrislaneheading}{l.SignalGroupName}_{l.RijstrookIndex}], " +
                                          $"PRM[{_prmpf}{_prmrislaneheadingmarge}{l.SignalGroupName}_{l.RijstrookIndex}])) MK[{_fcpf}{l.SignalGroupName}] |= BIT10;");
                            sb.AppendLine($"{ts}{ts}if (ris_verlengen_heading({_fcpf}{l.SignalGroupName}, {sitf}, PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}], RIS_{l.Type}, PRM[{_prmpf}{_prmrisvstartsrm0}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], PRM[{_prmpf}{_prmrisvendsrm0}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], !SCH[{_schpf}{_schrisgeencheckopsg}], " +
                                          $"PRM[{_prmpf}{_prmrislaneheading}{l.SignalGroupName}_{l.RijstrookIndex}], " +
                                          $"PRM[{_prmpf}{_prmrislaneheadingmarge}{l.SignalGroupName}_{l.RijstrookIndex}])) MK[{_fcpf}{l.SignalGroupName}] |= BIT13;");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}{ts}if (ris_verlengen({_fcpf}{l.SignalGroupName}, {sitf}, PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}], RIS_{l.Type}, PRM[{_prmpf}{_prmrisvstart}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], PRM[{_prmpf}{_prmrisvend}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], SCH[{_schpf}{_schrisgeencheckopsg}])) MK[{_fcpf}{l.SignalGroupName}] |= BIT10;");
                            sb.AppendLine($"{ts}{ts}if (ris_verlengen({_fcpf}{l.SignalGroupName}, {sitf}, PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}], RIS_{l.Type}, PRM[{_prmpf}{_prmrisvstartsrm0}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], PRM[{_prmpf}{_prmrisvendsrm0}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], !SCH[{_schpf}{_schrisgeencheckopsg}])) MK[{_fcpf}{l.SignalGroupName}] |= BIT13;");
                        }
                    }
                    sb.AppendLine($"{ts}#endif");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* verlengen RIS schakelbaar, 1 schakelaar voor het schakelen van alle verlengfuncties */");
                    sb.AppendLine($"{ts}if (!SCH[schrisverlengen])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}MK[fc] &= ~(BIT10|BIT13);");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}");
                    sb.AppendLine($"{ts}");
                    sb.AppendLine($"{ts}");
                    sb.AppendLine($"{ts}");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCSystemApplication2:
                    if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                    {
                        sb.AppendLine($"{ts}#ifdef AUTOMAAT");
                        sb.AppendLine($"{ts}{ts}/* Weggeschreven SSM (ACTIVEPRIO)-berichten ‘laten negeren’ voor de Applicatiecontainer */");
                        sb.AppendLine($"{ts}{ts}if (CIF_WPS[CIF_PROG_CONTROL] != CIF_CONTROL_INCONTROL)");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}/* zijn er SSM (ACTIVEPRIO)-berichten weggeschreven? */");
                        sb.AppendLine($"{ts}{ts}{ts}if (RIF_ACTIVEPRIO_AP_WRITE != RIF_ACTIVEPRIO_AP_READ)");
                        sb.AppendLine($"{ts}{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}{ts}/* zet de schrijfpointer terug */");
                        sb.AppendLine($"{ts}{ts}{ts}{ts}RIF_ACTIVEPRIO_AP_WRITE = RIF_ACTIVEPRIO_AP_READ;");
                        sb.AppendLine($"{ts}{ts}{ts}}}");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine($"{ts}#endif");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    sb.AppendLine($"{ts}{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)");
                    sb.AppendLine($"{ts}{ts}{ts}/* simulatie van RIS berichten */");
                    sb.AppendLine($"{ts}{ts}{ts}/* --------------------------- */");
                    sb.AppendLine($"{ts}{ts}{ts}ris_simulation(SAPPLPROG);");
                    sb.AppendLine($"{ts}{ts}#endif");
                    sb.AppendLine($"{ts}{ts}{ts}/* RIS-Controller */");
                    sb.AppendLine($"{ts}{ts}{ts}/* -------------- */");
                    sb.AppendLine($"{ts}{ts}{ts}ris_controller(SAPPLPROG, TRUE);");
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _prmlijn = CCOLGeneratorSettingsProvider.Default.GetElementName("prmlijn");
            _prmrisrole = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisrole");
            _prmrissubrole = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrissubrole");

            return base.SetSettings(settings);
        }
    }
}
