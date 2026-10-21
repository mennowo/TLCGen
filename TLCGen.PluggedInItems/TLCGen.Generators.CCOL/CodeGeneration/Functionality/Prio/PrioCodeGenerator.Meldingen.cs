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
    // used by CollectCCOLElements. NOTE: names are built in both GetMeldingElements and GetMeldingCode; keep them in sync.
    public partial class PrioCodeGenerator
    {
        private string GetInUitMeldenCode(ControllerModel c, string ts)
        {
            var sb = new StringBuilder();
            var first = false;

            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                var vtgType = $"PRM[{_prmpf}{_prmvtgcat}{CCOLCodeHelper.GetPriorityName(c, prio)}]";
                if (!int.TryParse(prio.FaseCyclus, out var fcNmr)) fcNmr = -1;

                // In case of no "inmeldingen" we still set the PRIO H to FALSE
                if (!prio.MeldingenData.Inmeldingen.Any())
                {
                    sb.AppendLine($"{ts}/* Inmelding {_fcpf}{prio.FaseCyclus} type {prio.Type.GetDescription()} */");
                    sb.Append($"{ts}IH[{_hpf}{_hprioin}{CCOLCodeHelper.GetPriorityName(c, prio)}] = ");
                    sb.AppendLine("FALSE;");
                }
                else
                {
                    var inmHelems = new List<string>();
                    if (!first) sb.AppendLine(); first = false;
                    sb.AppendLine($"{ts}/* Inmelding {_fcpf}{prio.FaseCyclus} type {prio.Type.GetDescription()} */");

                    foreach (var melding in prio.MeldingenData.Inmeldingen.Where(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton))
                    {
                        var hov = melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _hprioin.ToString() : _hpriouit.ToString();
                        var he = $"{_hpf}{hov}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
                        if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton && melding.FietsPrioriteitGebruikLus)
                        {
                            he += melding.RelatedInput1;
                        }
                        sb.AppendLine($"{ts}fietsprio_update({_fcpf}{prio.FaseCyclus}, " +
                                      (melding.FietsPrioriteitGebruikLus ? $"{_dpf}{melding.RelatedInput1}, " : "NG, ") +
                                      (melding.FietsPrioriteitGebruikLus ? $"{_ctpf}{_cftsvtg}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, " : "NG, ") +
                                      $"{_ctpf}{_cftscyc}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}," +
                                      $"SH[{he}], ML);");
                    }

                    var sb2 = new StringBuilder();
                    foreach (var inm in prio.MeldingenData.Inmeldingen)
                    {
                        var hulps = GetMeldingCode(c, prio, inm, sb2, vtgType, fcNmr, ts, prio.MeldingenData.AntiJutterVoorAlleInmeldingen);
                        inmHelems.AddRange(hulps);
                    }
                    sb.Append($"{ts}IH[{_hpf}{_hprioin}{CCOLCodeHelper.GetPriorityName(c, prio)}] = ");
                    foreach (var i in inmHelems)
                    {
                        sb.Append($"IH[{i}] = ");
                    }
                    sb.AppendLine("FALSE;");

                    sb.Append(sb2);

                    sb.Append($"{ts}IH[{_hpf}{_hprioin}{CCOLCodeHelper.GetPriorityName(c, prio)}] = ");
                    if (prio.MeldingenData.AntiJutterVoorAlleInmeldingen)
                    {
                        sb.Append($"RT[{_tpf}{_tprioin}{CCOLCodeHelper.GetPriorityName(c, prio)}] = ");
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
                    sb.Append($"{ts}IH[{_hpf}{_hpriouit}{CCOLCodeHelper.GetPriorityName(c, prio)}] = ");
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
                    sb.Append($"{ts}IH[{_hpf}{_hpriouit}{CCOLCodeHelper.GetPriorityName(c, prio)}] = ");
                    foreach (var i in uitmHelems)
                    {
                        sb.Append($"IH[{i}] = ");
                    }
                    sb.AppendLine("FALSE;");
                    sb.Append(sb2);

                    sb.Append($"{ts}IH[{_hpf}{_hpriouit}{CCOLCodeHelper.GetPriorityName(c, prio)}] = ");
                    if (prio.MeldingenData.AntiJutterVoorAlleUitmeldingen)
                    {
                        sb.Append($"RT[{_tpf}{_tpriouit}{CCOLCodeHelper.GetPriorityName(c, prio)}] = ");
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
            string hprio;
            string schprio;
            string tprio;

            // type melding
            switch (melding.InUit)
            {
                case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                    hprio = _hprioin.ToString();
                    schprio = _schprioin.ToString();
                    tprio = _tprioin.ToString();
                    break;
                case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                default:
                    hprio = _hpriouit.ToString();
                    schprio = _schpriouit.ToString();
                    tprio = _tpriouit.ToString();
                    break;
            }

            var he = $"{hprio}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
            var ti = $"{tprio}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
            var sw = $"{schprio}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
            if (melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding &&
                melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                he += melding.RelatedInput1;
                ti += melding.RelatedInput1;
                sw = sw + melding.RelatedInput1;
                if (melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton)
                {
                    sw += PrioCodeGeneratorHelper.GetDetectorTypeSCHString(melding.RelatedInput1Type);
                }
                if ((melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector || melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang) && melding.TweedeInput)
                {
                    he += melding.RelatedInput2;
                    ti += melding.RelatedInput2;
                    sw = sw + melding.RelatedInput2 + PrioCodeGeneratorHelper.GetDetectorTypeSCHString(melding.RelatedInput2Type);
                }
            }

            if (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding && 
                melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton)
            {
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cftscyc}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", 999, CCOLElementTimeTypeEnum.None, _cftscyc, prio.FaseCyclus));
                if (melding.FietsPrioriteitGebruikLus)
                {
                    elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cftsvtg}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", 999, CCOLElementTimeTypeEnum.None, _cftsvtg, prio.FaseCyclus));
                }
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmftsblok}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", melding.FietsPrioriteitBlok, CCOLElementTimeTypeEnum.None, _prmftsblok, prio.FaseCyclus));
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmftsmaxpercyc}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", melding.FietsPrioriteitAantalKeerPerCyclus, CCOLElementTimeTypeEnum.None, _prmftsmaxpercyc, prio.FaseCyclus));
                if (melding.FietsPrioriteitGebruikLus)
                {
                    elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmftsminvtg}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", melding.FietsPrioriteitMinimumAantalVoertuigen, CCOLElementTimeTypeEnum.None, _prmftsminvtg, prio.FaseCyclus));
                }
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmftsminwt}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", melding.FietsPrioriteitMinimumWachttijdVoorPrioriteit, CCOLElementTimeTypeEnum.TS_type, _prmftsminwt, prio.FaseCyclus));

                if (melding.FietsPrioriteitGebruikRIS)
                {
                    elements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_mftstelris}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", _mftstelris, CCOLCodeHelper.GetPriorityName(c, prio)));
                    elements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmftsminvtgris}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", 
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
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hprioin}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", _hprioin, prio.FaseCyclus, "Opticom"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tprioin}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", melding.AntiJutterTijdToepassen ? melding.AntiJutterTijd : 0, CCOLElementTimeTypeEnum.TE_type, _tprioin, prio.FaseCyclus, "Opticom"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schprioin}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", 1, CCOLElementTimeTypeEnum.SCH_type, _schprioin, prio.FaseCyclus, "Opticom"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpriouit}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", _hpriouit, prio.FaseCyclus, "Opticom"));
            }

            return elements;
        }

        private static string GetMeldingDetectieCode(PrioIngreepInUitMeldingModel melding, string dpf)
        {
            var sb = new StringBuilder();
            switch (melding.RelatedInput1Type)
            {
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie:
                    sb.Append($"SD[{dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp:
                    sb.Append($"D[{dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet:
                    sb.Append($"DB[{dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet:
                    sb.Append($"!DB_old[{dpf}{melding.RelatedInput1}] && DB[{dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie:
                    sb.Append($"ED[{dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat:
                    sb.Append($"TDH_old[{dpf}{melding.RelatedInput1}] && !TDH[{dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartIngang:
                    sb.Append($"IS[{dpf}{melding.RelatedInput1}] && !IS_old[{dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.IngangHoog:
                    sb.Append($"IS[{dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeIngang:
                    sb.Append($"IS_old[{dpf}{melding.RelatedInput1}] && !IS[{dpf}{melding.RelatedInput1}]");
                    break;
            }
            if (melding.TweedeInput)
            {
                switch (melding.RelatedInput2Type)
                {
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie:
                        sb.Append($" && SD[{dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp:
                        sb.Append($" && D[{dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet:
                        sb.Append($" && DB[{dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet:
                        sb.Append($" && !DB_old[{dpf}{melding.RelatedInput2}] && DB[{dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie:
                        sb.Append($" && ED[{dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat:
                        sb.Append($" && TDH_old[{dpf}{melding.RelatedInput2}] && !TDH[{dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartIngang:
                        sb.Append($" && IS[{dpf}{melding.RelatedInput2}] && !IS_old[{dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.IngangHoog:
                        sb.Append($" && IS[{dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeIngang:
                        sb.Append($" && IS_old[{dpf}{melding.RelatedInput2}] && !IS[{dpf}{melding.RelatedInput2}]");
                        break;
                }
            }
            return sb.ToString();
        }

        private List<string> GetMeldingCode(ControllerModel c, PrioIngreepModel prio, PrioIngreepInUitMeldingModel melding, StringBuilder sb, string vtgType, int fcNmr, string ts, bool antiJutVoorAlles, bool opvang = false, string otherHov = null)
        {
            var meldingHElems = new List<string>();
            string hov;
            string schov;
            string tov;

            // type melding
            switch (melding.InUit)
            {
                case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                    hov = _hprioin.ToString();
                    schov = _schprioin.ToString();
                    tov = _tprioin.ToString();
                    break;
                case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                default:
                    hov = _hpriouit.ToString();
                    schov = _schpriouit.ToString();
                    tov = _tpriouit.ToString();
                    break;
            }

            var he = $"{_hpf}{hov}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
            var ti = $"{_tpf}{tov}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
            var sw = $"{_schpf}{schov}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
            if (melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding &&
                melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                he += melding.RelatedInput1;
                ti += melding.RelatedInput1;
                sw += melding.RelatedInput1;
                if (melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton)
                {
                    sw += PrioCodeGeneratorHelper.GetDetectorTypeSCHString(melding.RelatedInput1Type);
                }
                if ((melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector || melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang) && melding.TweedeInput)
                {
                    he += melding.RelatedInput2;
                    ti += melding.RelatedInput2;
                    sw = sw + melding.RelatedInput2 + PrioCodeGeneratorHelper.GetDetectorTypeSCHString(melding.RelatedInput2Type);
                }
            }
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
            switch (melding.Type)
            {
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                    if (!string.IsNullOrWhiteSpace(melding.RelatedInput1))
                    {
                        sb.Append($" && (CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING)");
                        if (melding.TweedeInput && !string.IsNullOrWhiteSpace(melding.RelatedInput2))
                        {
                            sb.Append($" && (CIF_IS[{_dpf}{melding.RelatedInput2}] < CIF_DET_STORING)");
                        }
                    }
                    break;
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
                var mm = $"{_mpf}{_mftstelris}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
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
                        if (CCOLCodeHelper.GetPriorityName(c, prio) != prio.FaseCyclus)
                        {
                            laneIdPrm = $"{_prmpf}{_prmrislaneid}{CCOLCodeHelper.GetPriorityName(c, prio)}_{l.RijstrookIndex}";
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
                sb.Append($"!T[{_tpf}{tov}{CCOLCodeHelper.GetPriorityName(c, prio)}] && ");
            }
            if (melding.AntiJutterTijdToepassen && melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                sb.Append($"!T[{ti}] && ");
            }
            if (melding.AlleenIndienGeenInmelding ||
                melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding && melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton)
            {
                sb.Append($"!C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, prio)}] && ");
            }
            if (melding.KijkNaarWisselStand)
            {
                sb.Append($"IH[{_hpf}{_hwissel}{CCOLCodeHelper.GetPriorityName(c, prio)}] && ");
            }
            if (melding.AlleenIndienRood)
            {
                sb.Append($"R[{_fcpf}{prio.FaseCyclus}] && !T[{_tpf}{_tovminrood}{CCOLCodeHelper.GetPriorityName(c, prio)}] && ");
            }

            var extra = "";
            if ((melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector ||
                 melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding) &&
                prio.CheckLijnNummer && prio.LijnNummers.Any())
            {
                if (!prio.CheckRitCategorie || melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector)
                {
                    extra += "DSIMeldingPRIO_LijnNummer_V1(" +
                             $"{_prmpf + _prmallelijnen + CCOLCodeHelper.GetPriorityName(c, prio)}, " +
                             $"{prio.LijnNummers.Count})";
                }
                else
                {
                    extra += "DSIMeldingPRIO_LijnNummerEnRitCategorie_V1(" +
                             $"{_prmpf + _prmallelijnen + CCOLCodeHelper.GetPriorityName(c, prio)}, " +
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
                extra += $"(!SCH[{_schpf}{_schchecksirene}{CCOLCodeHelper.GetPriorityName(c, prio)}] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR)";
            }
            if (extra == "") extra = "TRUE";

            switch (melding.Type)
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
                                                              $"(!SCH[{_schpf}{_schchecksirene}{CCOLCodeHelper.GetPriorityName(c, prio)}] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR))");
                    }
                    else
                    {
                        sb.Append($"{tts}{ts}DSIMeldingPRIO{(melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? "_V1" : "_V2")}(" +
                                                        (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding
                                                            ? ""
                                                            : $"{_fcpf}{prio.FaseCyclus}, prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}, ") +
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
                                                                  $"(!SCH[{_schpf}{_schchecksirene}{CCOLCodeHelper.GetPriorityName(c, prio)}] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR))");
                        }
                        else
                        {
                            sb.Append($"{tts}{ts}DSIMeldingPRIO{(melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? "_V1" : "_V2")}(" +
                                                    (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding
                                                        ? ""
                                                        : $"{_fcpf}{prio.FaseCyclus}, prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}, ") +
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
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                    sb.AppendLine(GetMeldingDetectieCode(melding, _dpf) + ";");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang:
                    sb.AppendLine(GetMeldingDetectieCode(melding, _ispf) + ";");
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
                                      (melding.FietsPrioriteitGebruikLus ? $"{_ctpf}{_cftsvtg}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, " : "NG, ") +
                                      $"{_ctpf}{_cftscyc}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, " +
                                      $"{_prmpf}{_prmftsblok}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, " +
                                      $"{_prmpf}{_prmftsmaxpercyc}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, " +
                                      (melding.FietsPrioriteitGebruikLus ? $"{_prmpf}{_prmftsminvtg}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, " : "NG, ") +
                                      $"{_prmpf}{_prmftsminwt}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, " +
                                      $"ML, " +
                                      (melding.FietsPrioriteitGebruikRIS
                                          ? $"{_mpf}{_mftstelris}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, {_prmpf}{_prmftsminvtgris}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}"
                                          : "NG, NG") +
                                      ");");
                    }
                    else
                    {
                        sb.AppendLine($"C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, prio)}] && G[{_fcpf}{prio.FaseCyclus}] && (T[{_tpf}{_tgb}{CCOLCodeHelper.GetPriorityName(c, prio)}] && (T_timer[{_tpf}{_tgb}{CCOLCodeHelper.GetPriorityName(c, prio)}] > 1));");
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
                                              $"PRM[{_prmpf}{_prmrisapproachid}{CCOLCodeHelper.GetPriorityName(c, prio)}], " +
                                              $"SYSTEM_ITF{(itf >= 0 ? (itf + 1).ToString() : "")}, " +
                                              $"PRM[{_prmpf}{_prmrislaneid}{CCOLCodeHelper.GetPriorityName(c, prio)}_{lane.RijstrookIndex}], " +
                                              $"PRM[{_prmpf}{_prmrisstationtype}{CCOLCodeHelper.GetPriorityName(c, prio)}], " +
                                              $"PRM[{_prmpf}{_prmrisstart}{CCOLCodeHelper.GetPriorityName(c, prio)}], " +
                                              $"PRM[{_prmpf}{_prmrisend}{CCOLCodeHelper.GetPriorityName(c, prio)}], " +
                                              $"PRM[{_prmpf}{_prmrisrole}{CCOLCodeHelper.GetPriorityName(c, prio)}], " +
                                              $"PRM[{_prmpf}{_prmrissubrole}{CCOLCodeHelper.GetPriorityName(c, prio)}], " +
                                              $"PRM[{_prmpf}{_prmrisimportance}{CCOLCodeHelper.GetPriorityName(c, prio)}], " +
                                              $"{(melding.RisEta.HasValue ? $"PRM[{_prmpf}{_prmriseta}{CCOLCodeHelper.GetPriorityName(c, prio)}]" : "NG")}, " +
                                              $"prioFC{CCOLCodeHelper.GetPriorityName(c, prio)})");
                                    first = false;
                                }
                                sb.AppendLine(";");
                            }
                            break;
                        case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                            sb.AppendLine($"({c.GetBoolV()})ris_uitmelding_selectief(prioFC{CCOLCodeHelper.GetPriorityName(c, prio)});");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom:
                    switch(melding.InUit)
                    {
                        case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                            sb.AppendLine($"SCH[{_schpf}{_schprioin}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}] && !C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, prio)}] && DB[{_dpf}{melding.RelatedInput1}];");
                            sb.AppendLine($"{ts}{ts}RT[{ti}] = G[{_fcpf}{prio.FaseCyclus}] && C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, prio)}];");
                            break;
                        case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                            var uitMelding = prio.MeldingenData.Uitmeldingen.FirstOrDefault(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom);
                            if (uitMelding != null && uitMelding.RelatedInput1 != null)
                            { 
                                sb.AppendLine($"SCH[{_schpf}{_schprioin}{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}] && !TDH[{_dpf}{uitMelding.RelatedInput1}] && TDH_old[{_dpf}{uitMelding.RelatedInput1}];");
                            }
                            break;
                    }
                    break;
            }
            if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                if (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding)
                    sb.AppendLine($"{ts}{ts}if (IH[{meldingHElems.Last()}]) iPrioriteitNooitAfkappen[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = TRUE;");
                else
                    sb.AppendLine($"{ts}{ts}if (IH[{meldingHElems.Last()}] && iAantalInmeldingen[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] == 0) iPrioriteitNooitAfkappen[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = FALSE;");
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
