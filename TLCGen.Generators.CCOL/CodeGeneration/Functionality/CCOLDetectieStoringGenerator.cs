using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLDetectieStoringGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;

#pragma warning disable 0649
        private string _schdvak;
        private string _thdv;
        private string _prmperc;
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
                    if (d.AanvraagBijStoring == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ||
                        d.AanvraagBijStoring == Models.Enumerations.NooitAltijdAanUitEnum.SchUit)
                    {
                        _MyElements.Add(
                            new CCOLElement(
                                $"{_schdvak}{_dpf}{d.Naam}",
                                d.AanvraagBijStoring == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ? 1 : 0,
                                CCOLElementTimeTypeEnum.SCH_type,
                                CCOLElementTypeEnum.Schakelaar));
                    }
                    if(d.Type == Models.Enumerations.DetectorTypeEnum.Kop && fc.HiaatKoplusBijDetectieStoring && fc.VervangendHiaatKoplus.HasValue)
                    {
                        _MyElements.Add(
                            new CCOLElement(
                                $"{_thdv}{_dpf}{d.Naam}",
                                fc.VervangendHiaatKoplus.Value,
                                CCOLElementTimeTypeEnum.TE_type,
                                CCOLElementTypeEnum.Timer));
                    }
                }
                if(fc.PercentageGroenBijDetectieStoring && fc.PercentageGroen.HasValue)
                {
                    _MyElements.Add(
                            new CCOLElement(
                                $"{_prmperc}{fc.Naam}",
                                fc.PercentageGroen.Value,
                                CCOLElementTimeTypeEnum.None,
                                CCOLElementTypeEnum.Parameter));
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

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.DetectieStoring:
                    return true;
                default:
                    return false;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.DetectieStoring:
                    
                    sb.AppendLine($"{ts}/* reset MK-bits vooraf, ivm onderlinge verwijzing. */");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}MK[fc] &= ~BIT5;");
                    sb.AppendLine();

                    #region Aanvraag per detector

                    sb.AppendLine($"{ts}/* Vaste aanvraag bij detectie storing */");
                    foreach (var fc in c.Fasen)
                    {
                        foreach (var d in fc.Detectoren)
                        {
                            if(d.AanvraagBijStoring == Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                            {
                                sb.AppendLine($"{ts}{ts}A[{_fcpf}{fc.Naam}] |= (CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING);");
                            }
                            else if(d.AanvraagBijStoring == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ||
                                    d.AanvraagBijStoring == Models.Enumerations.NooitAltijdAanUitEnum.SchUit)
                            {
                                sb.AppendLine($"{ts}A[{_fcpf}{fc.Naam}] |= SCH[{_schpf}{_schdvak}{_dpf}{d.Naam}] && (CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING);");
                            }
                        }
                    }
                    sb.AppendLine();

                    #endregion // Aanvraag per detector

                    #region Aanvraag alle detectoren

                    sb.AppendLine($"{ts}/* vaste aanvraag bij detectiestoring alle aanvraaglussen */");
                    foreach (var fc in c.Fasen)
                    {
                        if (!fc.AanvraagBijDetectieStoring ||
                            fc.Detectoren.Count == 0 ||
                            !fc.Detectoren.Where(x => x.Aanvraag != Models.Enumerations.DetectorAanvraagTypeEnum.Geen).Any())
                        {
                            continue;
                        }

                        string pre = "".PadLeft($"{ts}A[{_fcpf}{fc.Naam}] |= ".Length);
                        sb.Append($"{ts}A[{_fcpf}{fc.Naam}] |= ");

                        if (fc.AantalRijstroken.HasValue)
                        {
                            for (int str = 1; str <= fc.AantalRijstroken; ++str)
                            {
                                int det = 0;
                                if (str > 1)
                                {
                                    sb.AppendLine(" ||");
                                }
                                foreach (var d in fc.Detectoren)
                                {
                                    if (d.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.Geen) continue;

                                    if (d.Rijstrook == str)
                                    {
                                        det++;
                                        if (det > 1)
                                        {
                                            sb.AppendLine(" &&");
                                            sb.Append($"{pre}((CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING) && (PRM[{_prmpf}{_prmda}{d.Naam}] > 0))");
                                        }
                                        else
                                        {
                                            if (str > 1)
                                            {
                                                sb.Append($"{pre}((CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING) && (PRM[{_prmpf}{_prmda}{d.Naam}] > 0))");
                                            }
                                            else
                                            {
                                                sb.Append($"((CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING) && (PRM[{_prmpf}{_prmda}{d.Naam}] > 0))");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            int det = 0;
                            foreach (var d in fc.Detectoren)
                            {
                                if (d.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.Geen) continue;

                                det++;
                                if (det > 1)
                                {
                                    sb.AppendLine(" &&");
                                    sb.Append($"{pre}((CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING) && (PRM[{_prmpf}{_prmda}{d.Naam}] > 0))");
                                }
                                else
                                {
                                    sb.Append($"((CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING) && (PRM[{_prmpf}{_prmda}{d.Naam}] > 0))");
                                }
                            }
                        }
                        sb.AppendLine(";");
                    }
                    sb.AppendLine();

                    #endregion // Aanvraag alle detectoren

                    #region Vervangend hiaat koplus

                    sb.AppendLine($"{ts}/* hiaattijd op koplus bij defect lange lus */");
                    sb.AppendLine($"{ts}/* ---------------------------------------- */");
                    foreach (var fc in c.Fasen)
                    {
                        if (!fc.HiaatKoplusBijDetectieStoring ||
                            !fc.VervangendHiaatKoplus.HasValue ||
                            fc.Type == Models.Enumerations.FaseTypeEnum.Voetganger ||
                            fc.Detectoren.Count == 0 ||
                            !fc.Detectoren.Where(x => x.Type != Models.Enumerations.DetectorTypeEnum.Kop || x.Type == Models.Enumerations.DetectorTypeEnum.Lang).Any())
                        {
                            continue;
                        }

                        for (int str = 1; str <= fc.AantalRijstroken; ++str)
                        {
                            var dkl = fc.Detectoren.Where(x => x.Type == Models.Enumerations.DetectorTypeEnum.Kop && x.Rijstrook == str).FirstOrDefault();
                            var dll = fc.Detectoren.Where(x => x.Type == Models.Enumerations.DetectorTypeEnum.Lang && x.Rijstrook == str).FirstOrDefault();
                            if(dkl != null && dll != null)
                            {
                                sb.AppendLine($"{ts}VervangendHiaatKoplus({_fcpf}{fc.Naam}, {_dpf}{dkl.Naam}, {_dpf}{dll.Naam}, {_tpf}{_thdv}{_dpf}{dkl.Naam});");
                            }
                        }
                    }
                    sb.AppendLine();

                    #endregion // Vervangend hiaat koplus

                    #region Percentage groen

                    sb.AppendLine($"{ts}/* percentage MG bij defect alle kop/lange lussen */");
                    sb.AppendLine($"{ts}/* ---------------------------------------------- */");
                    foreach (var fc in c.Fasen)
                    {
                        if (!fc.PercentageGroenBijDetectieStoring ||
                            !fc.PercentageGroen.HasValue || 
                            !c.GroentijdenSets.SelectMany(x => x.Groentijden).Where(x => x.FaseCyclus == fc.Naam && x.Waarde.HasValue).Any() ||
                            fc.Detectoren.Count == 0)
                        {
                            continue;
                        }

                        if (fc.AantalRijstroken.HasValue)
                        {
                            string pre = "".PadLeft($"{ts}if (".Length);
                            sb.Append($"{ts}if (");
                            for (int str = 1; str <= fc.AantalRijstroken; ++str)
                            {
                                int det = 0;
                                if (str > 1)
                                {
                                    sb.AppendLine(" ||");
                                }
                                foreach (var d in fc.Detectoren)
                                {
                                    if (d.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.Geen) continue;

                                    if (d.Rijstrook == str)
                                    {
                                        det++;
                                        if (det > 1)
                                        {
                                            sb.Append($" && (CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING)");
                                        }
                                        else
                                        {
                                            if (str > 1)
                                            {
                                                sb.Append($"{pre}(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING)");
                                            }
                                            else
                                            {
                                                sb.Append($"(CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING)");
                                            }
                                        }
                                    }
                                }
                            }
                            sb.AppendLine(")");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}MK[{_fcpf}{fc.Naam}] |= BIT5;");
                            string grfunc = "";
                            switch (c.Data.TypeGroentijden)
                            {
                                case GroentijdenTypeEnum.MaxGroentijden: grfunc = "PercentageMaxGroenTijden"; break;
                                case GroentijdenTypeEnum.VerlengGroentijden: grfunc = "PercentageVerlengGroenTijden"; break;
                            }
                            sb.AppendLine($"{ts}{ts}{grfunc}({_fcpf}{fc.Naam}, {_mpf}{_mperiod}, {_prmpf}{_prmperc}{fc.Naam}, {c.GroentijdenSets.Count}, ");
                            sb.Append("".PadLeft($"{ts}{ts}{grfunc}(".Length));
                            int i = 0;
                            foreach(var mgsm in c.GroentijdenSets)
                            {
                                foreach (GroentijdModel mgm in mgsm.Groentijden)
                                {
                                    if (mgm.FaseCyclus == fc.Naam && mgm.Waarde.HasValue)
                                    {
                                        if (i > 0) sb.Append(", "); ++i;
                                        sb.Append($"{_prmpf}{mgsm.Naam.ToLower()}{fc.Naam}");
                                    }
                                }
                            }
                            sb.AppendLine($");");
                            sb.AppendLine($"{ts}}}");
                        }
                    }
                    sb.AppendLine();

                    #endregion // Percentage groen

                    sb.AppendLine($"{ts}");

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
