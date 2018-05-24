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
    public class VAOntruimenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _tva;
        private CCOLGeneratorCodeStringSettingModel _tvamax;
#pragma warning restore 0649
        private string _schwisselpol;
        private string _schgeenwissel;

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

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCRealisatieAfhandeling:
                    return 10;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCRealisatieAfhandeling:
	                if (!c.VAOntruimenFasen.Any()) return "";

                    sb.AppendLine($"{ts}/* VA ontruimen */");
                    sb.AppendLine($"{ts}/* ============ */");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* herstarten maxima */");
                    foreach (var va in c.VAOntruimenFasen)
                    {
                        if (va.VADetectoren?.Count > 0)
                        {
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
                                sb.Append($"{ts}if (!(CIF_IS[{_dpf}{d.Detector}] >= CIF_DET_STORING)");
                                if (va.KijkNaarWisselstand &&
                                    (((va.Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Ingang && !string.IsNullOrWhiteSpace(va.Wissel1Input)) ||
                                       (va.Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Detector && !string.IsNullOrWhiteSpace(va.Wissel1Detector))) ||
                                     (va.Wissel2 &&
                                      ((va.Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Ingang && !string.IsNullOrWhiteSpace(va.Wissel2Input)) ||
                                       (va.Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Detector && !string.IsNullOrWhiteSpace(va.Wissel2Detector))))))
                                {
                                    if (va.KijkNaarWisselstand)
                                    {
                                        if (va.Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Ingang)
                                        {
                                            sb.Append(va.Wissel1InputVoorwaarde ?
                                                $" && ((SCH[{_schpf}{_schwisselpol}{va.Wissel1Input}] ? !IS[{_ispf}{va.Wissel1Input}] : IS[{_ispf}{va.Wissel1Input}]) || SCH[{_schpf}{_schgeenwissel}{va.Wissel1Input}])" :
                                                $" && ((SCH[{_schpf}{_schwisselpol}{va.Wissel1Input}] ? IS[{_ispf}{va.Wissel1Input}] : !IS[{_ispf}{va.Wissel1Input}]) || SCH[{_schpf}{_schgeenwissel}{va.Wissel1Input}])");
                                        }
                                        else
                                        {
                                            sb.Append($" && (D[{_dpf}{va.Wissel1Detector}] || SCH[{_schpf}{_schgeenwissel}{va.Wissel1Detector}]) &&");
                                        }
                                    }
                                    if (va.Wissel2)
                                    {
                                        if (va.Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Ingang)
                                        {
                                            sb.Append(va.Wissel2InputVoorwaarde ?
                                                $" && ((SCH[{_schpf}{_schwisselpol}{va.Wissel2Input}] ? !IS[{_ispf}{va.Wissel2Input}] : IS[{_ispf}{va.Wissel2Input}]) || SCH[{_schpf}{_schgeenwissel}{va.Wissel2Input}])" :
                                                $" && ((SCH[{_schpf}{_schwisselpol}{va.Wissel2Input}] ? IS[{_ispf}{va.Wissel2Input}] : !IS[{_ispf}{va.Wissel2Input}]) || SCH[{_schpf}{_schgeenwissel}{va.Wissel2Input}])");
                                        }
                                        else
                                        {
                                            sb.Append($" && (D[{_dpf}{va.Wissel2Detector}] || SCH[{_schpf}{_schgeenwissel}{va.Wissel2Detector}])");
                                        }
                                    }
                                }
                                sb.AppendLine(")");
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
                        }
                    }

                    sb.AppendLine($"{ts}/* afzetten X voor alle relevante fasen */");
                    var cfasen = new List<string>();
                    foreach (var va in c.VAOntruimenFasen)
                    {
                        if (va.VADetectoren?.Count > 0)
                        {
                            var fasen = va.VADetectoren.First().ConflicterendeFasen;
                            foreach (var cf in fasen)
                            {
                                if (!cfasen.Contains(cf.FaseCyclus))
                                {
                                    cfasen.Add(cf.FaseCyclus);
                                }
                            }
                        }
                    }
                    foreach(var fc in cfasen)
                    {
                        sb.AppendLine($"{ts}X[{_fcpf}{fc}] &= ~BIT8;");
                    }
                    sb.AppendLine();

                    foreach (var va in c.VAOntruimenFasen)
                    {
                        if (va.VADetectoren?.Count > 0)
                        {
                            sb.AppendLine($"{ts}/* opzetten X voor conflicten van fase {va.FaseCyclus} */");
                            var fasen = va.VADetectoren.First().ConflicterendeFasen;
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
                                sb.AppendLine($") X[{_fcpf}{cf.FaseCyclus}] |= BIT8;");
                            }
                        }
                    }
                    sb.AppendLine();
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _schwisselpol = CCOLGeneratorSettingsProvider.Default.GetElementName("schwisselpol");
            _schgeenwissel = CCOLGeneratorSettingsProvider.Default.GetElementName("schgeenwissel");

            return base.SetSettings(settings);
        }
    }
}