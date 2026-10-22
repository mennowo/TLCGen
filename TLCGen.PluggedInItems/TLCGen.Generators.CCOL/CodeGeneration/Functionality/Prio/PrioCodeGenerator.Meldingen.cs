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
    // Hook: PrioCInUitMelden (prio.c InUitMelden()); also the elements of meldingen (GetMeldingElements),
    // used by CollectCCOLElements. The names of a melding come from GetMeldingNames (MeldingNames.cs).
    // This file holds what all meldingen share; the type specific parts are in Prio/Meldingen/*MeldingEmitter.cs.
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

            GetMeldingEmitter(melding.Type).AddElements(m, elements);

            if (addHov)
            {
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(he,
                    melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _hprioin : _hpriouit, prio.FaseCyclus, prio.Type.GetDescription()));
            }
            if (HasAntiJutterPerMelding(melding))
            {
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(ti, melding.AntiJutterTijd, CCOLElementTimeTypeEnum.TE_type,
                    melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _tprioin : _tpriouit, prio.FaseCyclus, prio.Type.GetDescription()));
            }
            elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(sw, 1, CCOLElementTimeTypeEnum.SCH_type,
                melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _schprioin : _schpriouit, prio.FaseCyclus, prio.Type.GetDescription()));

            if (PrioMeldingTypes.HasValidOpvangStoring(melding))
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

            GetMeldingEmitter(melding.Type).AddElementsBeforeMelding(m, _myElements);

            return elements;
        }

        /// <summary>
        /// Anti-jutter per melding (AntiJutterTijdToepassen) is for inmeldingen only: uitmeldingen share the anti-jutter
        /// timer of all uitmeldingen (AntiJutterVoorAlleUitmeldingen). The flag on an uitmelding is ignored.
        /// The same goes for AlleenIndienGeenInmelding and AlleenIndienRood (checked inline in GetMeldingCode).
        /// </summary>
        private static bool HasAntiJutterPerMelding(PrioIngreepInUitMeldingModel melding) =>
            melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding && melding.AntiJutterTijdToepassen;

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
            var emitter = GetMeldingEmitter(melding.Type);
            var m = CreateMeldingContext(c, prio, melding);
            m = m with { Ts = ts, Tts = ts + ts, VtgType = vtgType, FcNmr = fcNmr, Timer = $"{_tpf}{m.Names.Timer}" };
            var ti = m.Timer;
            var prioName = m.PrioName;
            // anti-jutter timer prefix, also used for the timer of all in- or uitmeldingen together
            var tov = m.IsInmelding ? _tprioin.ToString() : _tpriouit.ToString();

            var he = otherHov ?? $"{_hpf}{m.Names.Hulp}";
            if (otherHov == null) meldingHElems.Add(he);

            if (emitter.WrapInNoRis)
            {
                sb.AppendLine($"#ifndef NO_RIS");
            }

            sb.Append($"{ts}{(opvang ? "else if" : "if")} (SCH[{_schpf}{m.Names.Switch}]");
            sb.Append(emitter.GetStoringCondition(m));
            sb.AppendLine(")");
            sb.AppendLine($"{ts}{{");

            // counting cyclists via RIS: only for a fiets inmelding (its elements are only declared there); the flag
            // may be left over on other melding types after a type change
            if (melding.FietsPrioriteitGebruikRIS && m.IsInmelding &&
                melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton)
            {
                FietsEmitter.AppendRisTellingCode(m, sb);
            }

            sb.Append($"{m.Tts}IH[{he}] = ");
            var antiJutterPerMelding = HasAntiJutterPerMelding(melding);
            if (antiJutterPerMelding && emitter.RestartsAntiJutterTimer)
            {
                sb.Append($"RT[{ti}] = ");
            }
            var checksAntiJutter = emitter.ChecksAntiJutterTimers;
            if (antiJutVoorAlles && checksAntiJutter)
            {
                sb.Append($"!T[{_tpf}{tov}{prioName}] && ");
            }
            if (antiJutterPerMelding && checksAntiJutter)
            {
                sb.Append($"!T[{ti}] && ");
            }
            if ((m.IsInmelding && melding.AlleenIndienGeenInmelding) || emitter.RequiresNoActiveInmelding(m))
            {
                sb.Append($"!C[{_ctpf}{_cvc}{prioName}] && ");
            }
            if (melding.KijkNaarWisselStand)
            {
                sb.Append($"IH[{_hpf}{_hwissel}{prioName}] && ");
            }
            if (m.IsInmelding && melding.AlleenIndienRood)
            {
                sb.Append($"R[{_fcpf}{prio.FaseCyclus}] && !T[{_tpf}{_tovminrood}{prioName}] && ");
            }

            emitter.AppendCondition(m, sb);
            emitter.AppendAfterCondition(m, sb, meldingHElems);
            sb.AppendLine($"{ts}}}");
            if (emitter.WrapInNoRis)
            {
                sb.AppendLine($"#endif /* NO_RIS */");
            }
            if (PrioMeldingTypes.HasValidOpvangStoring(melding))
            {
                meldingHElems.AddRange(GetMeldingCode(c, prio, melding.MeldingBijstoring, sb, vtgType, fcNmr, ts, antiJutVoorAlles, true, he));
            }
            return meldingHElems;
        }
    }
}
