using System.Text;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: meldingen of type Detector and Ingang: one or two detectors or inputs (RelatedInput1/2).
    public partial class PrioCodeGenerator
    {
        private sealed class DetectorMeldingEmitter : PrioMeldingEmitter
        {
            private readonly bool _ingang;

            public DetectorMeldingEmitter(PrioCodeGenerator generator, bool ingang) : base(generator)
            {
                _ingang = ingang;
            }

            public override string GetStoringCondition(PrioMeldingContext m)
            {
                var melding = m.Melding;
                if (_ingang || string.IsNullOrWhiteSpace(melding.RelatedInput1)) return "";

                var condition = $" && (CIF_IS[{G._dpf}{melding.RelatedInput1}] < CIF_DET_STORING)";
                if (melding.TweedeInput && !string.IsNullOrWhiteSpace(melding.RelatedInput2))
                {
                    condition += $" && (CIF_IS[{G._dpf}{melding.RelatedInput2}] < CIF_DET_STORING)";
                }
                return condition;
            }

            public override void AppendCondition(PrioMeldingContext m, StringBuilder sb)
            {
                sb.AppendLine(GetMeldingDetectieCode(m.Melding, _ingang ? G._ispf : G._dpf) + ";");
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
        }
    }
}
