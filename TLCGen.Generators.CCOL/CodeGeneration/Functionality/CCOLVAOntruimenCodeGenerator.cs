using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLVAOntruimenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

#pragma warning disable 0649
        private string _tva;
        private string _tvamax;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            foreach(var va in c.VAOntruimenFasen)
            {
                _MyElements.Add(
                            new CCOLElement(
                                $"{_tvamax}{va.FaseCyclus}",
                                va.VAOntrTijdensRood,
                                CCOLElementTimeTypeEnum.TE_type,
                                CCOLElementTypeEnum.Timer));
                foreach (var d in va.VADetectoren)
                {
                    foreach(var cf in d.ConflicterendeFasen)
                    {
                        _MyElements.Add(
                            new CCOLElement(
                                $"{_tva}{va.FaseCyclus}{cf.FaseCyclus}_{_dpf}{d.Detector}",
                                cf.VAOntruimingsTijd,
                                CCOLElementTimeTypeEnum.TE_type,
                                CCOLElementTypeEnum.Timer));
                    }
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
                case CCOLRegCCodeTypeEnum.RealisatieAfhandeling:
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
                case CCOLRegCCodeTypeEnum.RealisatieAfhandeling:
                    sb.AppendLine($"{ts}/* VA ontruimen */");
                    sb.AppendLine($"{ts}/* ============ */");
                    sb.AppendLine();
                    foreach (var va in c.VAOntruimenFasen)
                    {
                        if (va.VADetectoren?.Count > 0)
                        {
                            sb.AppendLine($"{ts}/* herstarten maxima */");
                            sb.AppendLine($"{ts}RT[{_tpf}{_tvamax}{va.FaseCyclus}] = !R[{_fcpf}{va.FaseCyclus}];");
                        }
                    }
                    sb.AppendLine();
                    foreach (var va in c.VAOntruimenFasen)
                    {
                        if (va.VADetectoren?.Count > 0)
                        {
                            sb.AppendLine($"{ts}/* Fase {va.FaseCyclus} */");
                            foreach (var d in va.VADetectoren)
                            {
                                sb.AppendLine($"{ts}if (!(CIF_IS[{_dpf}{d.Detector}] >= CIF_DET_STORING))");
                                sb.AppendLine($"{ts}{{");
                                foreach (var cf in d.ConflicterendeFasen)
                                {
                                    sb.AppendLine($"{ts}{ts}RT[{_tpf}{_tva}{va.FaseCyclus}{cf.FaseCyclus}_{_dpf}{d.Detector}] = D[{_dpf}{d.Detector}] && T[{_tpf}{_tvamax}{va.FaseCyclus}] && T_max[{_tpf}{_tva}{va.FaseCyclus}{cf.FaseCyclus}_{_dpf}{d.Detector}];");
                                }
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"{ts}else");
                                sb.AppendLine($"{ts}{{");
                                foreach (var cf in d.ConflicterendeFasen)
                                {
                                    sb.AppendLine($"{ts}{ts}RT[{_tpf}{_tva}{va.FaseCyclus}{cf.FaseCyclus}_{_dpf}{d.Detector}] = FALSE;");
                                }
                                sb.AppendLine($"{ts}}}");
                            }
                            sb.AppendLine();
                            var fasen = va.VADetectoren.First().ConflicterendeFasen;
                            foreach (var cf in fasen)
                            {
                                sb.AppendLine($"{ts}X[{_fcpf}{cf.FaseCyclus}] &= ~BIT5;");
                            }
                            foreach (var cf in fasen)
                            { 
                                sb.Append($"{ts}if(");
                                int i = 0;
                                foreach (var d in va.VADetectoren)
                                {
                                    if (i != 0)
                                    {
                                        sb.Append(" || ");
                                    }
                                    sb.Append($"T[{_tpf}{_tva}{va.FaseCyclus}{cf.FaseCyclus}_{_dpf}{d.Detector}]");
                                    ++i;
                                }
                                sb.AppendLine($") X[{_fcpf}{cf.FaseCyclus}] |= BIT5;");
                            }
                        }
                    }
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool HasSettings()
        {
            return true;
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            if (settings == null || settings.Settings == null)
            {
                return false;
            }

            foreach (var s in settings.Settings)
            {
                if (s.Default == "va") _tva = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "vamax") _tvamax = s.Setting == null ? s.Default : s.Setting;
            }

            return base.SetSettings(settings);
        }
    }
}