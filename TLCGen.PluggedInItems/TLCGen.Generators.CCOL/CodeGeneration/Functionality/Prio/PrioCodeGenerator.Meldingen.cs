using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: in- and uitmeldingen per ingreep (KAR, selectieve detector, detector, ingang, Vecom, fiets, RIS, Opticom).
    // Hook: PrioCInUitMelden (prio.c InUitMelden()); also the element names of meldingen (GetMeldingElements),
    // used by CollectCCOLElements. The names of a melding come from GetMeldingNames (MeldingNames.cs).
    public partial class PrioCodeGenerator
    {
        private string GetInUitMeldenCode(ControllerModel c, string ts)
        {
            var sb = new StringBuilder();
            var first = false;

            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                var prioName = CCOLCodeHelper.GetPriorityName(c, prio);
                var vtgType = $"PRM[{_prmpf}{_prmvtgcat}{prioName}]";
                if (!int.TryParse(prio.FaseCyclus, out var fcNmr)) fcNmr = -1;

                // In case of no "inmeldingen" we still set the PRIO H to FALSE
                if (!prio.MeldingenData.Inmeldingen.Any())
                {
                    sb.AppendLine($"{ts}/* Inmelding {_fcpf}{prio.FaseCyclus} type {prio.Type.GetDescription()} */");
                    sb.Append($"{ts}IH[{_hpf}{_hprioin}{prioName}] = ");
                    sb.AppendLine("FALSE;");
                }
                else
                {
                    var inmHelems = new List<string>();
                    if (!first) sb.AppendLine(); first = false;
                    sb.AppendLine($"{ts}/* Inmelding {_fcpf}{prio.FaseCyclus} type {prio.Type.GetDescription()} */");

                    foreach (var melding in prio.MeldingenData.Inmeldingen.Where(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton))
                    {
                        var names = GetMeldingNames(c, prio, melding);
                        var hov = melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _hprioin.ToString() : _hpriouit.ToString();
                        // NOTE: unlike names.Hulp, this only includes the input when the loop is used (todo note 1579)
                        var he = $"{_hpf}{hov}{names.Base}";
                        if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton && melding.FietsPrioriteitGebruikLus)
                        {
                            he += melding.RelatedInput1;
                        }
                        sb.AppendLine($"{ts}fietsprio_update({_fcpf}{prio.FaseCyclus}, " +
                                      (melding.FietsPrioriteitGebruikLus ? $"{_dpf}{melding.RelatedInput1}, " : "NG, ") +
                                      (melding.FietsPrioriteitGebruikLus ? $"{_ctpf}{_cftsvtg}{names.Base}, " : "NG, ") +
                                      $"{_ctpf}{_cftscyc}{names.Base}," +
                                      $"SH[{he}], ML);");
                    }

                    var sb2 = new StringBuilder();
                    foreach (var inm in prio.MeldingenData.Inmeldingen)
                    {
                        var hulps = GetMeldingCode(c, prio, inm, sb2, vtgType, fcNmr, ts, prio.MeldingenData.AntiJutterVoorAlleInmeldingen);
                        inmHelems.AddRange(hulps);
                    }
                    sb.Append($"{ts}IH[{_hpf}{_hprioin}{prioName}] = ");
                    foreach (var i in inmHelems)
                    {
                        sb.Append($"IH[{i}] = ");
                    }
                    sb.AppendLine("FALSE;");

                    sb.Append(sb2);

                    sb.Append($"{ts}IH[{_hpf}{_hprioin}{prioName}] = ");
                    if (prio.MeldingenData.AntiJutterVoorAlleInmeldingen)
                    {
                        sb.Append($"RT[{_tpf}{_tprioin}{prioName}] = ");
                    }
                    var ifirst = true;
                    if (prio.CheckPeriode && prio.GerelateerdePerioden.Any(x => !string.IsNullOrEmpty(x.Periode)))
                    {
                        if (prio.GerelateerdePerioden.Count > 1) sb.Append("(");
                        var pfirst = true;
                        foreach (var p in prio.GerelateerdePerioden)
                        {
                            if (!pfirst) sb.Append(" || "); pfirst = false;
                            sb.Append($"IH[{_hpf}{_hperiod}{p.Periode}]");
                        }
                        if (prio.GerelateerdePerioden.Count > 1) sb.Append(")");
                        sb.Append(" && ");

                        sb.Append("(");
                    }
                    foreach (var i in inmHelems)
                    {
                        if (!ifirst) sb.Append(" || ");
                        sb.Append($"IH[{i}]");
                        ifirst = false;
                    }

                    if (prio.CheckPeriode && prio.GerelateerdePerioden.Any(x => !string.IsNullOrEmpty(x.Periode)))
                    {
                        sb.Append(")");
                    }
                    sb.AppendLine(";");
                }

                // In case of no "uitmeldingen" we still set the PRIO H to FALSE
                if (!prio.MeldingenData.Uitmeldingen.Any())
                {
                    sb.AppendLine($"{ts}/* Uitmelding {_fcpf}{prio.FaseCyclus} type {prio.Type.GetDescription()} */");
                    sb.Append($"{ts}IH[{_hpf}{_hpriouit}{prioName}] = ");
                    sb.AppendLine("FALSE;");
                }
                else
                    if (prio.MeldingenData.Uitmeldingen.Any())
                {
                    var uitmHelems = new List<string>();
                    if (!first) sb.AppendLine(); 
                    first = false;
                    sb.AppendLine($"{ts}/* Uitmelding {_fcpf}{prio.FaseCyclus} type {prio.Type.GetDescription()} */");

                    var sb2 = new StringBuilder();
                    foreach (var uitm in prio.MeldingenData.Uitmeldingen)
                    {
                        var hulps = GetMeldingCode(c, prio, uitm, sb2, vtgType, fcNmr, ts, prio.MeldingenData.AntiJutterVoorAlleUitmeldingen);
                        uitmHelems.AddRange(hulps);
                    }
                    sb.Append($"{ts}IH[{_hpf}{_hpriouit}{prioName}] = ");
                    foreach (var i in uitmHelems)
                    {
                        sb.Append($"IH[{i}] = ");
                    }
                    sb.AppendLine("FALSE;");
                    sb.Append(sb2);

                    sb.Append($"{ts}IH[{_hpf}{_hpriouit}{prioName}] = ");
                    if (prio.MeldingenData.AntiJutterVoorAlleUitmeldingen)
                    {
                        sb.Append($"RT[{_tpf}{_tpriouit}{prioName}] = ");
                    }
                    first = true;
                    foreach (var i in uitmHelems)
                    {
                        if (!first) sb.Append(" || ");
                        sb.Append($"IH[{i}]");
                        first = false;
                    }
                    sb.AppendLine($";");
                }
            }

            AppendRisDisplayCode(c, sb, ts);

            AppendHDLegacyInUitMeldenCode(c, sb, ts, ref first);

            return sb.ToString();
        }

        private IEnumerable<CCOLElement> GetMeldingElements(ControllerModel c, PrioIngreepModel prio, PrioIngreepInUitMeldingModel melding, bool addHov)
        {
            var elements = new List<CCOLElement>();
            var names = GetMeldingNames(c, prio, melding);
            var prioName = CCOLCodeHelper.GetPriorityName(c, prio);
            var he = names.Hulp;
            var ti = names.Timer;
            var sw = names.Switch;

            if (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding && 
                melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton)
            {
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cftscyc}{names.Base}", 999, CCOLElementTimeTypeEnum.None, _cftscyc, prio.FaseCyclus));
                if (melding.FietsPrioriteitGebruikLus)
                {
                    elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cftsvtg}{names.Base}", 999, CCOLElementTimeTypeEnum.None, _cftsvtg, prio.FaseCyclus));
                }
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmftsblok}{names.Base}", melding.FietsPrioriteitBlok, CCOLElementTimeTypeEnum.None, _prmftsblok, prio.FaseCyclus));
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmftsmaxpercyc}{names.Base}", melding.FietsPrioriteitAantalKeerPerCyclus, CCOLElementTimeTypeEnum.None, _prmftsmaxpercyc, prio.FaseCyclus));
                if (melding.FietsPrioriteitGebruikLus)
                {
                    elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmftsminvtg}{names.Base}", melding.FietsPrioriteitMinimumAantalVoertuigen, CCOLElementTimeTypeEnum.None, _prmftsminvtg, prio.FaseCyclus));
                }
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmftsminwt}{names.Base}", melding.FietsPrioriteitMinimumWachttijdVoorPrioriteit, CCOLElementTimeTypeEnum.TS_type, _prmftsminwt, prio.FaseCyclus));

                if (melding.FietsPrioriteitGebruikRIS)
                {
                    elements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_mftstelris}{names.Base}", _mftstelris, prioName));
                    elements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmftsminvtgris}{names.Base}", 
                            melding.FietsPrioriteitMinimumAantalVoertuigenRIS, CCOLElementTimeTypeEnum.None, _prmftsminvtgris, prio.FaseCyclus));
                }
            }

            if (addHov)
            {
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(he,
                    melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _hprioin : _hpriouit, prio.FaseCyclus, prio.Type.GetDescription()));
            }
            if (melding.AntiJutterTijdToepassen)
            {
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(ti, melding.AntiJutterTijd, CCOLElementTimeTypeEnum.TE_type,
                    melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _tprioin : _tpriouit, prio.FaseCyclus, prio.Type.GetDescription()));
            }
            elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(sw, 1, CCOLElementTimeTypeEnum.SCH_type,
                melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _schprioin : _schpriouit, prio.FaseCyclus, prio.Type.GetDescription()));

            if (melding.OpvangStoring && melding.MeldingBijstoring != null)
            {
                var elems = GetMeldingElements(c, prio, melding.MeldingBijstoring, false);
                foreach (var e in elems)
                {
                    if (_myElements.All(x => x.Naam != e.Naam))
                    {
                        _myElements.Add(e);
                    }
                }
            }

            if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom && 
                melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding &&
                !string.IsNullOrWhiteSpace(melding.RelatedInput1))
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hprioin}{names.Base}", _hprioin, prio.FaseCyclus, "Opticom"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tprioin}{names.Base}", melding.AntiJutterTijdToepassen ? melding.AntiJutterTijd : 0, CCOLElementTimeTypeEnum.TE_type, _tprioin, prio.FaseCyclus, "Opticom"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schprioin}{names.Base}", 1, CCOLElementTimeTypeEnum.SCH_type, _schprioin, prio.FaseCyclus, "Opticom"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpriouit}{names.Base}", _hpriouit, prio.FaseCyclus, "Opticom"));
            }

            return elements;
        }

        private List<string> GetMeldingCode(ControllerModel c, PrioIngreepModel prio, PrioIngreepInUitMeldingModel melding, StringBuilder sb, string vtgType, int fcNmr, string ts, bool antiJutVoorAlles, bool opvang = false, string otherHov = null)
        {
            var meldingHElems = new List<string>();
            var names = GetMeldingNames(c, prio, melding);
            var prioName = CCOLCodeHelper.GetPriorityName(c, prio);
            // anti-jutter timer prefix, also used for the timer of all in- or uitmeldingen together
            var tov = melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _tprioin.ToString() : _tpriouit.ToString();

            var he = $"{_hpf}{names.Hulp}";
            var ti = $"{_tpf}{names.Timer}";
            var sw = $"{_schpf}{names.Switch}";
            if (otherHov != null) he = otherHov;
            else meldingHElems.Add(he);

            var tts = ts;

            if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                sb.AppendLine($"#ifndef NO_RIS");
            }

            if (!opvang)
            {
                tts = ts + ts;
                sb.Append($"{ts}if (SCH[{sw}]");
            }
            else if (opvang)
            {
                tts = ts + ts;
                sb.Append($"{ts}else if (SCH[{sw}]");
            }

            var extra = "";
            if ((melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector ||
                 melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding) &&
                prio.CheckLijnNummer && prio.LijnNummers.Any())
            {
                if (!prio.CheckRitCategorie || melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector)
                {
                    extra += "DSIMeldingPRIO_LijnNummer_V1(" +
                             $"{_prmpf + _prmallelijnen + prioName}, " +
                             $"{prio.LijnNummers.Count})";
                }
                else
                {
                    extra += "DSIMeldingPRIO_LijnNummerEnRitCategorie_V1(" +
                             $"{_prmpf + _prmallelijnen + prioName}, " +
                             $"{prio.LijnNummers.Count})";
                }
            }
            if (prio.CheckWagenNummer)
            {
                extra += extra == "" ? "" : " && ";
                extra += melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? $"WDNST_check_in({_fcpf}{prio.FaseCyclus})" : $"WDNST_check_uit({_fcpf}{prio.FaseCyclus})";
            }
            if (prio.CheckOpSirene)
            {
                extra += extra == "" ? "" : " && ";
                extra += $"(!SCH[{_schpf}{_schchecksirene}{prioName}] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR)";
            }
            if (extra == "") extra = "TRUE";

            var emitter = GetMeldingEmitter(melding.Type);
            var m = new PrioMeldingContext(c, prio, melding, names, prioName)
            {
                Ts = ts, Tts = tts, VtgType = vtgType, FcNmr = fcNmr, Timer = ti, Extra = extra
            };

            if (emitter != null) sb.Append(emitter.GetStoringCondition(m));
            else switch (melding.Type)
            {
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                    if (!string.IsNullOrWhiteSpace(melding.RelatedInput1))
                    {
                        sb.Append($" && (CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING)");
                    }
                    break;
            }
            sb.AppendLine(")");
            sb.AppendLine($"{ts}{{");

            if (melding.FietsPrioriteitGebruikRIS)
            {
                var mm = $"{_mpf}{_mftstelris}{names.Base}";
                sb.AppendLine($"{tts}MM[{mm}] = 0;");
                var lanes = c.RISData.RISPelotonLanes.Where(x => x.SignalGroupName == prio.FaseCyclus).ToArray();
                if (lanes.Length > 0)
                {
                    sb.AppendLine($"#ifndef NO_RIS");
                    foreach (var l in lanes)
                    {
                        // opzoeken lane instellingen
                        var risFc = c.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == prio.FaseCyclus);
                        if (risFc == null) continue;
                        
                        var fcLane = risFc.LaneData.FirstOrDefault(x => x.RijstrookIndex == l.RijstrookIndex);
                        
                        var itf = c.RISData.HasMultipleSystemITF
                            ? c.RISData.MultiSystemITF.FindIndex(x => x.SystemITF == fcLane?.SystemITF) : -1;

                        var laneIdPrm = $"{_prmpf}{_prmrislaneid}_{l.RijstrookIndex}";
                        if (prioName != prio.FaseCyclus)
                        {
                            laneIdPrm = $"{_prmpf}{_prmrislaneid}{prioName}_{l.RijstrookIndex}";
                        }

                        sb.AppendLine($"{tts}MM[{mm}] += {(fcLane.UseHeading ? "ris_itsstations_heading" : "ris_itsstations")}(" +
                                      $"{_fcpf}{prio.FaseCyclus}, " +
                                      $"SYSTEM_ITF{(itf >= 0 ? (itf + 1).ToString() : "")}, " +
                                      $"PRM[{laneIdPrm}], " +
                                      $"RIS_{l.Type}, " +
                                      $"PRM[{_prmpf}{_prmrispstart}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], " +
                                      $"PRM[{_prmpf}{_prmrispend}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], " +
                                      $"SCH[{_schpf}{_schrisgeencheckopsg}]" +
                                      (fcLane.UseHeading 
                                          ? $", PRM[{_prmpf}{_prmrislaneheading}{l.SignalGroupName}_{l.RijstrookIndex}], " +
                                            $"PRM[{_prmpf}{_prmrislaneheadingmarge}{l.SignalGroupName}_{l.RijstrookIndex}]);"
                                          : ");"));
                    }
                    sb.AppendLine($"#endif /* NO_RIS */");
                }
            }
            
            sb.Append($"{tts}IH[{he}] = ");
            if (melding.AntiJutterTijdToepassen && melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom)
            {
                sb.Append($"RT[{ti}] = ");
            }
            if (antiJutVoorAlles && melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                sb.Append($"!T[{_tpf}{tov}{prioName}] && ");
            }
            if (melding.AntiJutterTijdToepassen && melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                sb.Append($"!T[{ti}] && ");
            }
            if (melding.AlleenIndienGeenInmelding ||
                melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding && melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton)
            {
                sb.Append($"!C[{_ctpf}{_cvc}{prioName}] && ");
            }
            if (melding.KijkNaarWisselStand)
            {
                sb.Append($"IH[{_hpf}{_hwissel}{prioName}] && ");
            }
            if (melding.AlleenIndienRood)
            {
                sb.Append($"R[{_fcpf}{prio.FaseCyclus}] && !T[{_tpf}{_tovminrood}{prioName}] && ");
            }

            if (emitter != null) emitter.AppendCondition(m, sb);
            else switch (melding.Type)
            {
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding:
                    var sgCheck = c.PrioData.KARSignaalGroepNummersInParameters
                            ? $"PRM[{_prmpf}{_prmkarsg}{prio.FaseCyclus}]"
                            : fcNmr > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers ? (fcNmr - 200).ToString() : fcNmr.ToString();
                    sb.AppendLine();
                    if (prio.Type == PrioIngreepVoertuigTypeEnum.Hulpdienst)
                    {
                        sb.Append($"{tts}{ts}DSIMelding_HD_V1({sgCheck}, " +
                                                              (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding
                                                                  ? "CIF_DSIN, "
                                                                  : "CIF_DSUIT, ") + 
                                                              $"(!SCH[{_schpf}{_schchecksirene}{prioName}] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR))");
                    }
                    else
                    {
                        sb.Append($"{tts}{ts}DSIMeldingPRIO{(melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? "_V1" : "_V2")}(" +
                                                        (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding
                                                            ? ""
                                                            : $"{_fcpf}{prio.FaseCyclus}, prioFC{prioName}, ") +
                                                        $"0, " +
                                                        $"{vtgType}, " +
                                                        "TRUE, " +
                                                        $"{(fcNmr == -1 ? "NG" : sgCheck)}," +
                                                        "TRUE, " +
                                                        (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding
                                                            ? "CIF_DSIN, "
                                                            : "CIF_DSUIT, ") +
                                                        $"{extra})");
                    }
                    if (prio.Type == PrioIngreepVoertuigTypeEnum.Hulpdienst &&
                        prio.InmeldingOokDoorToepassen && prio.InmeldingOokDoorFase > 0)
                    {
                        var hdpriofcnr = (prio.InmeldingOokDoorFase.ToString().Length < 2) ? "0" + prio.InmeldingOokDoorFase.ToString()
                                                                                                 : prio.InmeldingOokDoorFase.ToString();
                        var actualAlsoFc = c.PrioData.KARSignaalGroepNummersInParameters
                            ? $"PRM[{_prmpf}{_prmkarsg}{hdpriofcnr}]"
                            : prio.InmeldingOokDoorFase > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers
                                ? (prio.InmeldingOokDoorFase - 200).ToString() : prio.InmeldingOokDoorFase.ToString();
                        sb.AppendLine(" ||");
                        if (prio.Type == PrioIngreepVoertuigTypeEnum.Hulpdienst)
                        {
                            sb.Append($"{tts}{ts}DSIMelding_HD_V1({actualAlsoFc}, " +
                                                                  (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding
                                                                      ? "CIF_DSIN, "
                                                                      : "CIF_DSUIT, ") +
                                                                  $"(!SCH[{_schpf}{_schchecksirene}{prioName}] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR))");
                        }
                        else
                        {
                            sb.Append($"{tts}{ts}DSIMeldingPRIO{(melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? "_V1" : "_V2")}(" +
                                                    (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding
                                                        ? ""
                                                        : $"{_fcpf}{prio.FaseCyclus}, prioFC{prioName}, ") +
                                                    $"0, " +
                                                    $"{vtgType}, " +
                                                    "TRUE, " +
                                                    $"{actualAlsoFc}," +
                                                    "TRUE, " +
                                                    (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding
                                                        ? "CIF_DSIN, "
                                                        : "CIF_DSUIT, ") +
                                                    $"{extra})");
                        }
                    }
                    sb.AppendLine(";");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                    sb.AppendLine();
                    sb.AppendLine($"{tts}{ts}DSIMeldingPRIO_V1({(_dpf + melding.RelatedInput1).ToUpper()}, " +
                                                    $"{vtgType}, " +
                                                    "FALSE, " +
                                                    "NG, " +
                                                    $"SCH[{_schpf}{_schcheckdstype}], " +
                                                    (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ||
                                                     melding.CheckAltijdOpDsinBijVecom
                                                        ? "CIF_DSIN, "
                                                        : "CIF_DSUIT, ") +
                                                    $"{extra});");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                    sb.AppendLine($" SD[{_dpf}{melding.RelatedInput1}];");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton:
                    if (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{tts}{ts}fietsprio_inmelding({_fcpf}{prio.FaseCyclus}, " +
                                      (melding.FietsPrioriteitGebruikLus ? $"{_dpf}{melding.RelatedInput1}, " : "NG, ") +
                                      (melding.FietsPrioriteitGebruikLus ? $"{_ctpf}{_cftsvtg}{names.Base}, " : "NG, ") +
                                      $"{_ctpf}{_cftscyc}{names.Base}, " +
                                      $"{_prmpf}{_prmftsblok}{names.Base}, " +
                                      $"{_prmpf}{_prmftsmaxpercyc}{names.Base}, " +
                                      (melding.FietsPrioriteitGebruikLus ? $"{_prmpf}{_prmftsminvtg}{names.Base}, " : "NG, ") +
                                      $"{_prmpf}{_prmftsminwt}{names.Base}, " +
                                      $"ML, " +
                                      (melding.FietsPrioriteitGebruikRIS
                                          ? $"{_mpf}{_mftstelris}{names.Base}, {_prmpf}{_prmftsminvtgris}{names.Base}"
                                          : "NG, NG") +
                                      ");");
                    }
                    else
                    {
                        sb.AppendLine($"C[{_ctpf}{_cvc}{prioName}] && G[{_fcpf}{prio.FaseCyclus}] && (T[{_tpf}{_tgb}{prioName}] && (T_timer[{_tpf}{_tgb}{prioName}] > 1));");
                    }
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde:
                    switch (melding.InUit)
                    {
                        case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                            var risFc = c.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == prio.FaseCyclus);
                            var first = true;

                            if (risFc != null)
                            {
                                sb.AppendLine();
                                foreach (var lane in risFc.LaneData)
                                {
                                    if (!first) sb.AppendLine(" ||");
                                    var itf = c.RISData.HasMultipleSystemITF
                                        ? c.RISData.MultiSystemITF.FindIndex(x => x.SystemITF == lane.SystemITF) : -1;
                                    sb.Append($"{ts}{ts}{ts}ris_inmelding_selectief(" +
                                              $"{_fcpf}{prio.FaseCyclus}, " +
                                              $"PRM[{_prmpf}{_prmrisapproachid}{prioName}], " +
                                              $"SYSTEM_ITF{(itf >= 0 ? (itf + 1).ToString() : "")}, " +
                                              $"PRM[{_prmpf}{_prmrislaneid}{prioName}_{lane.RijstrookIndex}], " +
                                              $"PRM[{_prmpf}{_prmrisstationtype}{prioName}], " +
                                              $"PRM[{_prmpf}{_prmrisstart}{prioName}], " +
                                              $"PRM[{_prmpf}{_prmrisend}{prioName}], " +
                                              $"PRM[{_prmpf}{_prmrisrole}{prioName}], " +
                                              $"PRM[{_prmpf}{_prmrissubrole}{prioName}], " +
                                              $"PRM[{_prmpf}{_prmrisimportance}{prioName}], " +
                                              $"{(melding.RisEta.HasValue ? $"PRM[{_prmpf}{_prmriseta}{prioName}]" : "NG")}, " +
                                              $"prioFC{prioName})");
                                    first = false;
                                }
                                sb.AppendLine(";");
                            }
                            break;
                        case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                            sb.AppendLine($"({c.GetBoolV()})ris_uitmelding_selectief(prioFC{prioName});");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom:
                    switch(melding.InUit)
                    {
                        case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                            sb.AppendLine($"SCH[{_schpf}{_schprioin}{names.Base}] && !C[{_ctpf}{_cvc}{prioName}] && DB[{_dpf}{melding.RelatedInput1}];");
                            sb.AppendLine($"{ts}{ts}RT[{ti}] = G[{_fcpf}{prio.FaseCyclus}] && C[{_ctpf}{_cvc}{prioName}];");
                            break;
                        case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                            var uitMelding = prio.MeldingenData.Uitmeldingen.FirstOrDefault(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom);
                            if (uitMelding != null && uitMelding.RelatedInput1 != null)
                            { 
                                sb.AppendLine($"SCH[{_schpf}{_schprioin}{names.Base}] && !TDH[{_dpf}{uitMelding.RelatedInput1}] && TDH_old[{_dpf}{uitMelding.RelatedInput1}];");
                            }
                            break;
                    }
                    break;
            }
            if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                if (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding)
                    sb.AppendLine($"{ts}{ts}if (IH[{meldingHElems.Last()}]) iPrioriteitNooitAfkappen[prioFC{prioName}] = TRUE;");
                else
                    sb.AppendLine($"{ts}{ts}if (IH[{meldingHElems.Last()}] && iAantalInmeldingen[prioFC{prioName}] == 0) iPrioriteitNooitAfkappen[prioFC{prioName}] = FALSE;");
            }
            sb.AppendLine($"{ts}}}");
            if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                sb.AppendLine($"#endif /* NO_RIS */");
            }
            if (melding.OpvangStoring && melding.MeldingBijstoring != null && melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding)
            {
                meldingHElems.AddRange(GetMeldingCode(c, prio, melding.MeldingBijstoring, sb, vtgType, fcNmr, ts, antiJutVoorAlles, true, he));
            }
            return meldingHElems;
        }
    }
}
