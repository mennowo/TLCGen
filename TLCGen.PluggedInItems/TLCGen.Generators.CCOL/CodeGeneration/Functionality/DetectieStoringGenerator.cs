using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class DetectieStoringGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schdvak;
        private CCOLGeneratorCodeStringSettingModel _thdv;
        private CCOLGeneratorCodeStringSettingModel _prmperc;
#pragma warning restore 0649
        private string _mperiod;
        private string _prmda;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            foreach (var fc in c.Fasen)
            {
                foreach (var d in fc.Detectoren)
                {
                    if (d.AanvraagBijStoring == NooitAltijdAanUitEnum.SchAan ||
                        d.AanvraagBijStoring == NooitAltijdAanUitEnum.SchUit)
                    {
                        _MyElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_schdvak}{_dpf}{d.Naam}",
                                d.AanvraagBijStoring == NooitAltijdAanUitEnum.SchAan ? 1 : 0,
                                CCOLElementTimeTypeEnum.SCH_type,
                                _schdvak, fc.Naam, d.Naam));
                    }
                    if(d.Type == DetectorTypeEnum.Kop && fc.HiaatKoplusBijDetectieStoring && fc.VervangendHiaatKoplus.HasValue)
                    {
                        _MyElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_thdv}{_dpf}{d.Naam}",
                                fc.VervangendHiaatKoplus.Value,
                                CCOLElementTimeTypeEnum.TE_type,
                                _thdv, fc.Naam, d.Naam));
                    }
                }
                if(fc.PercentageGroenBijDetectieStoring && fc.PercentageGroen.HasValue)
                {
                    _MyElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmperc}{fc.Naam}",
                            fc.PercentageGroen.Value,
                            CCOLElementTimeTypeEnum.None,
                            _prmperc, fc.Naam));
                }
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCDetectieStoring:
                    return 10;
                case CCOLCodeTypeEnum.HstCDetectieStoring:
                    return 10;
                default:
                    return 0;
            }
        }

        #region Code gen methods

        private void AanvraagPerDetector(StringBuilder sb, ControllerModel c, string ts)
        {
            sb.AppendLine($"{ts}/* Vaste aanvraag bij detectie storing */");
            foreach (var fc in c.Fasen)
            {
                foreach (var d in fc.Detectoren)
                {
                    if (d.AanvraagBijStoring == NooitAltijdAanUitEnum.Altijd)
                    {
                        sb.AppendLine($"{ts}{ts}A[{_fcpf}{fc.Naam}] |= (CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING);");
                    }
                    else if (d.AanvraagBijStoring == NooitAltijdAanUitEnum.SchAan ||
                            d.AanvraagBijStoring == NooitAltijdAanUitEnum.SchUit)
                    {
                        sb.AppendLine($"{ts}A[{_fcpf}{fc.Naam}] |= SCH[{_schpf}{_schdvak}{_dpf}{d.Naam}] && (CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING);");
                    }
                }
            }
            sb.AppendLine();
        }

        private void AanvraagAlleDetectoren(StringBuilder sb, ControllerModel c, string ts)
        {
            sb.AppendLine($"{ts}/* vaste aanvraag bij detectiestoring alle aanvraaglussen */");
            foreach (var fc in c.Fasen)
            {
                if (!fc.AanvraagBijDetectieStoring ||
                    fc.Detectoren.Count == 0 ||
                    fc.Detectoren.All(x => x.Aanvraag == DetectorAanvraagTypeEnum.Geen))
                {
                    continue;
                }

                var pre = "".PadLeft($"{ts}A[{_fcpf}{fc.Naam}] |= ".Length);
                sb.Append($"{ts}A[{_fcpf}{fc.Naam}] |= ");

                if (fc.AantalRijstroken.HasValue && fc.Type != FaseTypeEnum.Voetganger)
                {
                    for (int str = 1; str <= fc.AantalRijstroken; ++str)
                    {
                        if (fc.Detectoren.Where(x => x.Aanvraag != DetectorAanvraagTypeEnum.Geen).All(x => x.Rijstrook != str)) continue;

                        int det = 0;
                        if (str > 1)
                        {
                            sb.AppendLine(" ||");
                        }
                        var ds = new List<string>();
                        foreach (var d in fc.Detectoren)
                        {
                            if (d.Aanvraag == DetectorAanvraagTypeEnum.Geen) continue;

                            if (d.Rijstrook == str)
                            {
                                det++;
                                if (det > 1)
                                {
                                    sb.AppendLine(" &&");
                                    sb.Append($"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING || PRM[{_prmpf}{_prmda}{d.Naam}] == 0)");
                                }
                                else
                                {
                                    sb.Append(str > 1
                                        ? $"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING || PRM[{_prmpf}{_prmda}{d.Naam}] == 0)"
                                        : $"(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING || PRM[{_prmpf}{_prmda}{d.Naam}] == 0)");
                                }
                                ds.Add($"PRM[{_prmpf}{_prmda}{d.Naam}]");
                            }
                        }
                        sb.Append(" && !(");
                        det = 0;
                        foreach(var d in ds)
                        {
                            if (det > 0) sb.Append(" && ");
                            sb.Append(d + " == 0");
                            det++;
                        }
                        sb.Append(")");
                    }
                }
                else
                {
                    var det = 0;
                    var ds = new List<string>();
                    foreach (var d in fc.Detectoren)
                    {
                        if (d.Aanvraag == DetectorAanvraagTypeEnum.Geen) continue;

                        det++;
                        if (det > 1)
                        {
                            sb.AppendLine(" &&");
                            sb.Append($"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING || PRM[{_prmpf}{_prmda}{d.Naam}] == 0)");
                        }
                        else
                        {
                            sb.Append($"(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING || PRM[{_prmpf}{_prmda}{d.Naam}] == 0)");
                        }
                        ds.Add($"PRM[{_prmpf}{_prmda}{d.Naam}]");
                    }
                    sb.Append(" && !(");
                    det = 0;
                    foreach (var d in ds)
                    {
                        if (det > 0) sb.Append(" && ");
                        sb.Append(d + " == 0");
                        det++;
                    }
                    sb.AppendLine(")");
                }
                sb.AppendLine(";");
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

        private void PercentageGroen(StringBuilder sb, ControllerModel c, string ts, bool halfstar)
        {
            sb.AppendLine($"{ts}/* percentage MG bij defect alle kop/lange lussen */");
            sb.AppendLine($"{ts}/* ---------------------------------------------- */");
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
                    var pre = "".PadLeft($"{ts}if (".Length);
                    sb.Append($"{ts}if (");
                    for (var str = 1; str <= fc.AantalRijstroken; ++str)
                    {
                        if (fc.Detectoren.Where(x => !x.IsDrukKnop()).All(x => x.Rijstrook != str)) continue;

                        var det = 0;
                        if (str > 1)
                        {
                            sb.AppendLine(" ||");
                        }
                        foreach (var d in fc.Detectoren.Where(x => !x.IsDrukKnop()))
                        {
                            if (d.Verlengen == DetectorVerlengenTypeEnum.Geen) continue;

                            if (d.Rijstrook != str) continue;

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
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}MK[{_fcpf}{fc.Naam}] |= BIT5;");
                    var grfunc = "";
                    if (halfstar)
                    {
                        switch (c.Data.TypeGroentijden)
                        {
                            case GroentijdenTypeEnum.MaxGroentijden: grfunc = "PercentageMaxGroenTijdenSP"; break;
                            case GroentijdenTypeEnum.VerlengGroentijden: grfunc = "PercentageVerlengGroenTijdenSP"; break;
                        }
                    }
                    else
                    {
                        switch (c.Data.TypeGroentijden)
                        {
                            case GroentijdenTypeEnum.MaxGroentijden: grfunc = "PercentageMaxGroenTijden"; break;
                            case GroentijdenTypeEnum.VerlengGroentijden: grfunc = "PercentageVerlengGroenTijden"; break;
                        }
                    }
                    if (halfstar)
                    {
                        sb.AppendLine($"{ts}{ts}{grfunc}({_fcpf}{fc.Naam}, {_prmpf}{_prmperc}{fc.Naam});");
                    }
                    else
                    {
                        sb.AppendLine($"{ts}{ts}{grfunc}({_fcpf}{fc.Naam}, {_mpf}{_mperiod}, {_prmpf}{_prmperc}{fc.Naam}, {c.GroentijdenSets.Count}, ");
                        sb.Append("".PadLeft($"{ts}{ts}{grfunc}(".Length));

                        var defmg = c.GroentijdenSets.FirstOrDefault(
                            x => x.Naam == c.PeriodenData.DefaultPeriodeGroentijdenSet);
                        var defmgfc = defmg?.Groentijden.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                        if (defmgfc?.Waarde != null)
                        {
                            sb.Append($"{_prmpf}{c.PeriodenData.DefaultPeriodeGroentijdenSet.ToLower()}_{fc.Naam}");
                        }

                        foreach (var per in c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.Groentijden))
                        {
                            foreach (var mgsm in c.GroentijdenSets.Where(x => x.Naam == per.GroentijdenSet))
                            {
                                foreach (var unused in mgsm.Groentijden.Where(x => x.FaseCyclus == fc.Naam && x.Waarde.HasValue))
                                {
                                    sb.Append(", ");
                                    sb.Append($"{_prmpf}{mgsm.Naam.ToLower()}_{fc.Naam}");
                                }
                            }
                        }

                        sb.AppendLine(");");
                    }
                    sb.AppendLine($"{ts}}}");
                }
            }
            sb.AppendLine();

        }

        #endregion

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCDetectieStoring:

                    sb.AppendLine($"{ts}/* reset MK-bits vooraf, ivm onderlinge verwijzing. */");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}MK[fc] &= ~BIT5;");
                    sb.AppendLine();

                    AanvraagPerDetector(sb, c, ts);

                    AanvraagAlleDetectoren(sb, c, ts);

                    VervangendHiaatKoplus(sb, c, ts);

                    PercentageGroen(sb, c, ts, false);

                    return sb.ToString();

                case CCOLCodeTypeEnum.HstCDetectieStoring:
                    
                    sb.AppendLine($"{ts}/* reset MK-bits vooraf, ivm onderlinge verwijzing. */");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}MK[fc] &= ~BIT5;");
                    sb.AppendLine();

                    AanvraagPerDetector(sb, c, ts);

                    AanvraagAlleDetectoren(sb, c, ts);

                    VervangendHiaatKoplus(sb, c, ts);

                    PercentageGroen(sb, c, ts, true);

                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _prmda = CCOLGeneratorSettingsProvider.Default.GetElementName("prmda");
            _mperiod = CCOLGeneratorSettingsProvider.Default.GetElementName("mperiod");

            return base.SetSettings(settings);
        }
    }
}
