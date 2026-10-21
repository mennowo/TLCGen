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
                        FietsEmitter.AppendUpdateCode(CreateMeldingContext(c, prio, melding) with { Ts = ts }, sb);
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
            var m = CreateMeldingContext(c, prio, melding);
            var names = m.Names;
            var he = names.Hulp;
            var ti = names.Timer;
            var sw = names.Switch;

            GetMeldingEmitter(melding.Type)?.AddElements(m, elements);

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

            GetMeldingEmitter(melding.Type)?.AddElementsBeforeMelding(m, _myElements);

            return elements;
        }

        /// <summary>
        /// Extra conditions of a DSI melding (KAR, selectieve detector): line numbers, wagennummer and sirene.
        /// Returns "TRUE" if there are none.
        /// </summary>
        private string GetDsiExtraCondition(PrioMeldingContext m, bool checkRitCategorie)
        {
            var prio = m.Prio;
            var extra = "";
            if (prio.CheckLijnNummer && prio.LijnNummers.Any())
            {
                extra += (checkRitCategorie ? "DSIMeldingPRIO_LijnNummerEnRitCategorie_V1(" : "DSIMeldingPRIO_LijnNummer_V1(") +
                         $"{_prmpf + _prmallelijnen + m.PrioName}, " +
                         $"{prio.LijnNummers.Count})";
            }
            if (prio.CheckWagenNummer)
            {
                extra += extra == "" ? "" : " && ";
                extra += m.IsInmelding ? $"WDNST_check_in({_fcpf}{prio.FaseCyclus})" : $"WDNST_check_uit({_fcpf}{prio.FaseCyclus})";
            }
            if (prio.CheckOpSirene)
            {
                extra += extra == "" ? "" : " && ";
                extra += $"(!SCH[{_schpf}{_schchecksirene}{m.PrioName}] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR)";
            }
            return extra == "" ? "TRUE" : extra;
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
            var emitter = GetMeldingEmitter(melding.Type);

            if (emitter?.WrapInNoRis == true)
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

            var m = new PrioMeldingContext(c, prio, melding, names, prioName)
            {
                Ts = ts, Tts = tts, VtgType = vtgType, FcNmr = fcNmr, Timer = ti
            };

            if (emitter != null) sb.Append(emitter.GetStoringCondition(m));
            sb.AppendLine(")");
            sb.AppendLine($"{ts}{{");

            // NOTE: checks the flag regardless of the melding type, while its elements are only declared for a
            // fiets inmelding (todo note 1579)
            if (melding.FietsPrioriteitGebruikRIS)
            {
                FietsEmitter.AppendRisTellingCode(m, sb);
            }

            sb.Append($"{tts}IH[{he}] = ");
            if (melding.AntiJutterTijdToepassen && (emitter?.RestartsAntiJutterTimer ?? true))
            {
                sb.Append($"RT[{ti}] = ");
            }
            var checksAntiJutter = emitter?.ChecksAntiJutterTimers ?? true;
            if (antiJutVoorAlles && checksAntiJutter)
            {
                sb.Append($"!T[{_tpf}{tov}{prioName}] && ");
            }
            if (melding.AntiJutterTijdToepassen && checksAntiJutter)
            {
                sb.Append($"!T[{ti}] && ");
            }
            if (melding.AlleenIndienGeenInmelding || (emitter?.RequiresNoActiveInmelding(m) ?? false))
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

            emitter?.AppendCondition(m, sb);
            emitter?.AppendAfterCondition(m, sb, meldingHElems);
            sb.AppendLine($"{ts}}}");
            if (emitter?.WrapInNoRis == true)
            {
                sb.AppendLine($"#endif /* NO_RIS */");
            }
            if (melding.OpvangStoring && melding.MeldingBijstoring != null && (emitter?.SupportsOpvangStoring ?? true))
            {
                meldingHElems.AddRange(GetMeldingCode(c, prio, melding.MeldingBijstoring, sb, vtgType, fcNmr, ts, antiJutVoorAlles, true, he));
            }
            return meldingHElems;
        }
    }
}
