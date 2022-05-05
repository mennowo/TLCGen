using System;
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
    public class DetectieStoringGenerator : CCOLCodePieceGeneratorBase
    {        
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schdvak;
        private CCOLGeneratorCodeStringSettingModel _thdv;
        private CCOLGeneratorCodeStringSettingModel _tdstvert;
        private CCOLGeneratorCodeStringSettingModel _tdstvertd;
        private CCOLGeneratorCodeStringSettingModel _prmperc;
#pragma warning restore 0649
        private string _mperiod;
        private string _hplact;
        private string _prmda;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var fc in c.Fasen)
            {
                foreach (var d in fc.Detectoren)
                {
                    if (d.AanvraagBijStoring == NooitAltijdAanUitEnum.SchAan ||
                        d.AanvraagBijStoring == NooitAltijdAanUitEnum.SchUit)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_schdvak}{_dpf}{d.Naam}",
                                d.AanvraagBijStoring == NooitAltijdAanUitEnum.SchAan ? 1 : 0,
                                CCOLElementTimeTypeEnum.SCH_type,
                                _schdvak, fc.Naam, d.Naam));
                    }
                    if(d.Type == DetectorTypeEnum.Kop && fc.HiaatKoplusBijDetectieStoring && fc.VervangendHiaatKoplus.HasValue)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_thdv}{_dpf}{d.Naam}",
                                fc.VervangendHiaatKoplus.Value,
                                CCOLElementTimeTypeEnum.TE_type,
                                _thdv, fc.Naam, d.Naam));
                    }
                }
                if (fc.PercentageGroenBijDetectieStoring && fc.PercentageGroen.HasValue)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmperc}{fc.Naam}",
                            fc.PercentageGroen.Value,
                            CCOLElementTimeTypeEnum.None,
                            _prmperc, fc.Naam));
                }
                if (fc.AanvraagBijDetectieStoringVertraagd)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tdstvert}{fc.Naam}",
                            fc.AanvraagBijDetectieStoringVertraging,
                            CCOLElementTimeTypeEnum.TE_type,
                            _tdstvert, fc.Naam));
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCDetectieStoring => new []{10},
                _ => null
            };
        }

        #region Code gen methods

        private void AanvraagAlleDetectoren(StringBuilder sb, ControllerModel c, string ts)
        {
            sb.AppendLine($"{ts}/* vaste/vertraagde aanvraag bij detectiestoring */");
            foreach (var fc in c.Fasen.Where(x => 
                (x.AanvraagBijDetectieStoring || x.Detectoren.Any(x => x.AanvraagBijStoring != NooitAltijdAanUitEnum.Nooit)) &&
                x.Detectoren.Any(x2 => x2.Aanvraag != DetectorAanvraagTypeEnum.Geen)))
            {
                var pre = "";
                if (fc.AanvraagBijDetectieStoringVertraagd && fc.AanvraagBijDetectieStoring)
                {
                    pre = "".PadLeft($"{ts}RT[{_tpf}{_tdstvert}{fc.Naam}] = ".Length);
                    sb.AppendLine($"{ts}RT[{_tpf}{_tdstvert}{fc.Naam}] = !T[{_tpf}{_tdstvert}{fc.Naam}] && R[{_fcpf}{fc.Naam}] && !A[{_fcpf}{fc.Naam}] && (");
                }
                else
                {
                    pre = "".PadLeft($"{ts}A[{_fcpf}{fc.Naam}] |= (".Length);
                    sb.AppendLine($"{ts}A[{_fcpf}{fc.Naam}] |= (");
                }

                var first = true;
                
                // aanvraag per lus
                if (fc.Detectoren.Any(x => x.AanvraagBijStoring != NooitAltijdAanUitEnum.Nooit))
                {
                    foreach (var d in fc.Detectoren)
                    {
                        if (d.AanvraagBijStoring == NooitAltijdAanUitEnum.Altijd)
                        {
                            if (!first) sb.AppendLine(" ||");
                            sb.Append($"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING)");
                        }
                        else if (d.AanvraagBijStoring == NooitAltijdAanUitEnum.SchAan ||
                                 d.AanvraagBijStoring == NooitAltijdAanUitEnum.SchUit)
                        {
                            if (!first) sb.AppendLine(" ||");
                            sb.Append($"{pre}SCH[{_schpf}{_schdvak}{_dpf}{d.Naam}] && (CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING)");
                        }
                        first = false;
                    }
                }

                // shortcut indien geen aanvraag voor fase
                if (!fc.AanvraagBijDetectieStoring)
                {
                    sb.AppendLine(") ? BIT5 : 0;");
                    continue;
                }

                // voor niet-voetgangers
                if (fc.AantalRijstroken.HasValue && fc.Type != FaseTypeEnum.Voetganger)
                {
                    for (var str = 1; str <= fc.AantalRijstroken; ++str)
                    {
                        // verzamelen relevante detectoren: op deze rijstrook, met aanvraag functie
                        var dets = fc.Detectoren.Where(x =>
                                        x.Rijstrook == str &&
                                        x.Aanvraag != DetectorAanvraagTypeEnum.Geen &&
                                        !(x.AanvraagHardOpStraat && x.Aanvraag == DetectorAanvraagTypeEnum.Uit)).ToList();

                        // skip rijstroken zonder detectie met aanvraag functie
                        if (!dets.Any()) continue;

                        // check alleen kop & knop
                        var kopKnop = false;
                        var kopLang = false;
                        if (fc.AanvraagBijDetectieStoringKoplusKnop &&
                            dets.Any(x => x.Type == DetectorTypeEnum.Kop) &&
                            dets.Any(x => x.Type == DetectorTypeEnum.Knop))
                        {
                            kopKnop = true;
                        }
                        else if (fc.AanvraagBijDetectieStoringKopLang &&
                            dets.Any(x => x.Type == DetectorTypeEnum.Kop) &&
                            dets.Any(x => x.Type == DetectorTypeEnum.Lang))
                        {
                            kopLang = true;
                        }

                        var det = 0;
                        if (str > 1 || !first)
                        {
                            sb.AppendLine(" ||");
                        }
                        var ds = new List<string>();
                        var rgv = dets.Any(x => (!kopKnop || (x.Type == DetectorTypeEnum.Kop || x.Type == DetectorTypeEnum.Knop)) && c.RichtingGevoeligeAanvragen.Any(x2 => x2.VanDetector == x.Naam || x2.NaarDetector == x.Naam));
                        foreach (var d in dets)
                        {
                            det++;
                            if (kopKnop && !(d.Type == DetectorTypeEnum.Kop || d.Type == DetectorTypeEnum.Knop)) continue;
                            if (kopLang && !(d.Type == DetectorTypeEnum.Kop || d.Type == DetectorTypeEnum.Lang)) continue;

                            if (det > 1)
                            {
                                sb.AppendLine(" &&");
                                if (!d.AanvraagHardOpStraat)
                                {
                                    sb.Append($"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING{(!rgv ? $" || PRM[{_prmpf}{_prmda}{d.Naam}] == 0" : "")})");
                                }
                                else
                                {
                                    sb.Append($"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING)");
                                }
                            }
                            else
                            {
                                if (!d.AanvraagHardOpStraat)
                                {
                                    sb.Append($"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING{(!rgv ? $" || PRM[{_prmpf}{_prmda}{d.Naam}] == 0" : "")})");
                                }
                                else
                                {
                                    sb.Append($"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING)");
                                }
                            }
                            if (!d.AanvraagHardOpStraat && !rgv)
                            {
                                ds.Add($"PRM[{_prmpf}{_prmda}{d.Naam}]");
                            }
                        }
                        if (!rgv)
                        {
                            sb.AppendLine(" &&");
                            sb.Append($"{pre}!(");
                            det = 0;
                            foreach (var d in ds)
                            {
                                if (det > 0) sb.Append(" && ");
                                sb.Append(d + " == 0");
                                det++;
                            }
                            sb.Append(")");
                        }
                    }
                }
                // voor voetgangers
                else
                {
                    var det = 0;
                    var ds = new List<string>();
                    foreach (var d in fc.Detectoren.Where(x => x.Aanvraag != DetectorAanvraagTypeEnum.Geen))
                    {
                        det++;
                        if (det > 1)
                        {
                            sb.AppendLine(" &&");
                            sb.Append(!d.AanvraagHardOpStraat
                                ? $"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING || PRM[{_prmpf}{_prmda}{d.Naam}] == 0)"
                                : $"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING)");
                        }
                        else
                        {
                            sb.Append(!d.AanvraagHardOpStraat
                                ? $"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING || PRM[{_prmpf}{_prmda}{d.Naam}] == 0)"
                                : $"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING)");
                        }
                        if (!d.AanvraagHardOpStraat)
                        {
                            ds.Add($"PRM[{_prmpf}{_prmda}{d.Naam}]");
                        }
                    }
                    if (fc.Detectoren.Any(x => !x.AanvraagHardOpStraat))
                    {
                        sb.Append(" && !(");
                        det = 0;
                        foreach (var d in ds)
                        {
                            if (det > 0) sb.Append(" && ");
                            sb.Append(d + " == 0");
                            det++;
                        }
                        sb.Append(")");
                    }
                }

                sb.AppendLine(fc.AanvraagBijDetectieStoringVertraagd ? ");" : ") ? BIT5 : 0;");
            }
            foreach (var fc in c.Fasen.Where(x =>
                x.AanvraagBijDetectieStoring &&
                x.Detectoren.Any(x2 => x2.Aanvraag != DetectorAanvraagTypeEnum.Geen &&
                x.AanvraagBijDetectieStoringVertraagd)))
            {
                sb.AppendLine($"{ts}A[{_fcpf}{fc.Naam}] |= (ET[{_tpf}{_tdstvert}{fc.Naam}] ? BIT5 : 0);");
            }
            sb.AppendLine();
        }

        private void VervangendHiaatKoplus(StringBuilder sb, ControllerModel c, string ts)
        {
            sb.AppendLine($"{ts}/* hiaattijd op koplus bij defect lange lus */");
            sb.AppendLine($"{ts}/* ---------------------------------------- */");
            foreach (var fc in c.Fasen)
            {
                if (!fc.HiaatKoplusBijDetectieStoring ||
                    !fc.VervangendHiaatKoplus.HasValue ||
                    fc.Type == FaseTypeEnum.Voetganger ||
                    fc.Detectoren.Count == 0 ||
                    !fc.Detectoren.Any(x => x.Type != DetectorTypeEnum.Kop || x.Type == DetectorTypeEnum.Lang))
                {
                    continue;
                }

                for (var str = 1; str <= fc.AantalRijstroken; ++str)
                {
                    var dkl = fc.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.Kop && x.Rijstrook == str);
                    var dll = fc.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.Lang && x.Rijstrook == str);
                    if (dkl != null && dll != null)
                    {
                        sb.AppendLine($"{ts}VervangendHiaatKoplus({_fcpf}{fc.Naam}, {_dpf}{dkl.Naam}, {_dpf}{dll.Naam}, {_tpf}{_thdv}{_dpf}{dkl.Naam});");
                    }
                }
            }
            sb.AppendLine();
        }

        private void PercentageGroen(StringBuilder sb, ControllerModel c, string ts1, string ts, bool halfstar)
        {
            var mg = c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? "MG" : "VG";
            sb.AppendLine($"{ts1}/* percentage {mg} bij defect alle kop/lange lussen */");
            sb.AppendLine($"{ts1}/* ---------------------------------------------- */");
            foreach (var fc in c.Fasen)
            {
                if (!fc.PercentageGroenBijDetectieStoring ||
                    !fc.PercentageGroen.HasValue ||
                    !c.GroentijdenSets.SelectMany(x => x.Groentijden).Any(x => x.FaseCyclus == fc.Naam && x.Waarde.HasValue) ||
                    fc.Detectoren.Count == 0 ||
                    fc.Detectoren.All(x => x.IsDrukKnop() || x.Verlengen == DetectorVerlengenTypeEnum.Geen))
                {
                    continue;
                }

                if (fc.AantalRijstroken.HasValue)
                {
                    var pre = "".PadLeft($"{ts1}if (".Length);
                    sb.Append($"{ts1}if (");
                    for (var str = 1; str <= fc.AantalRijstroken; ++str)
                    {
                        var dets = fc.Detectoren.Where(x => x.IsKopLang() && x.Verlengen != DetectorVerlengenTypeEnum.Geen && x.Rijstrook == str).ToList();
                        if (dets.Count == 0) continue;

                        var det = 0;
                        if (str > 1)
                        {
                            sb.AppendLine(" ||");
                        }
                        foreach (var d in dets)
                        {
                            det++;
                            if (det > 1)
                            {
                                sb.Append($" && (CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING)");
                            }
                            else
                            {
                                sb.Append(str > 1
                                    ? $"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING)"
                                    : $"(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING)");
                            }
                        }
                    }
                    sb.AppendLine(")");
                    sb.AppendLine($"{ts1}{{");
                    sb.AppendLine($"{ts1}{ts}MK[{_fcpf}{fc.Naam}] |= BIT5;");
                    var grfunc = "";
                    if (halfstar)
                    {
                        grfunc = c.Data.TypeGroentijden switch
                        {
                            GroentijdenTypeEnum.MaxGroentijden => "PercentageMaxGroenTijden_halfstar",
                            GroentijdenTypeEnum.VerlengGroentijden => "PercentageVerlengGroenTijden_halfstar",
                            _ => grfunc
                        };
                    }
                    else
                    {
                        grfunc = c.Data.TypeGroentijden switch
                        {
                            GroentijdenTypeEnum.MaxGroentijden => "PercentageMaxGroenTijden",
                            GroentijdenTypeEnum.VerlengGroentijden => "PercentageVerlengGroenTijden",
                            _ => grfunc
                        };
                    }
                    if (halfstar)
                    {
                        sb.AppendLine($"{ts1}{ts}{grfunc}({_fcpf}{fc.Naam}, {_prmpf}{_prmperc}{fc.Naam}, BIT5);");
                    }
                    else
                    {
                        sb.AppendLine($"{ts1}{ts}{grfunc}({_fcpf}{fc.Naam}, {_mpf}{_mperiod}, PRM[{_prmpf}{_prmperc}{fc.Naam}], ");
                        sb.Append("".PadLeft($"{ts1}{ts}{grfunc}(".Length));

                        var rest = "";
                        var irest = 1;
                        if (!c.Data.TVGAMaxAlsDefaultGroentijdSet)
                        {
                            rest += $", PRM[{_prmpf}{c.PeriodenData.DefaultPeriodeGroentijdenSet?.ToLower() ?? "NG"}_{fc.Naam}]";
                        }
                        else
                        {
                            rest += $", TVGA_max[{_fcpf}{fc.Naam}]";
                        }
                        
                        foreach (var period in c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.Groentijden))
                        {
                            var greentimeSet = c.GroentijdenSets.FirstOrDefault(x => x.Naam == period.GroentijdenSet);
                            if (greentimeSet == null) continue;

                            if (greentimeSet.Groentijden.Any(x => x.FaseCyclus == fc.Naam && x.Waarde.HasValue))
                            {
                                ++irest;
                                rest += $", PRM[{_prmpf}{greentimeSet.Naam.ToLower()}_{fc.Naam}]";
                            }
                        }

                        sb.AppendLine($"{irest}{rest});");
                    }
                    sb.AppendLine($"{ts1}}}");
                }
            }
            sb.AppendLine();
        }

        #endregion

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCDetectieStoring => new List<CCOLLocalVariable>{new CCOLLocalVariable("int", "fc")},
                _ => base.GetFunctionLocalVariables(c, type)
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCDetectieStoring:

                    sb.AppendLine($"{ts}/* reset MK-bits vooraf, ivm onderlinge verwijzing. */");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}MK[fc] &= ~BIT5;");
                    sb.AppendLine();

                    AanvraagAlleDetectoren(sb, c, ts);

                    VervangendHiaatKoplus(sb, c, ts);

                    if (c.HalfstarData.IsHalfstar)
                    {
                        sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}])");
                        sb.AppendLine($"{ts}{{");
                        PercentageGroen(sb, c, ts + ts, ts, true);
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine($"{ts}else");
                        sb.AppendLine($"{ts}{{");
                        PercentageGroen(sb, c, ts + ts, ts, false);
                        sb.AppendLine($"{ts}}}");
                    }
                    else
                    {
                        PercentageGroen(sb, c, ts, ts, false);
                    }

                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _prmda = CCOLGeneratorSettingsProvider.Default.GetElementName("prmda");
            _mperiod = CCOLGeneratorSettingsProvider.Default.GetElementName("mperiod");
            _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");

            return base.SetSettings(settings);
        }
    }
}
