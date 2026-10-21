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
                var code = GetInputCode(melding.RelatedInput1Type, $"{dpf}{melding.RelatedInput1}");
                if (melding.TweedeInput)
                {
                    var code2 = GetInputCode(melding.RelatedInput2Type, $"{dpf}{melding.RelatedInput2}");
                    if (code2 != null) code += $" && {code2}";
                }
                return code;
            }

            /// <summary>
            /// Condition on a single detector or input, eg. SD[d1]; null for an unknown input type.
            /// </summary>
            private static string GetInputCode(PrioIngreepInUitMeldingVoorwaardeInputTypeEnum type, string input)
            {
                return type switch
                {
                    PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie => $"SD[{input}]",
                    PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp => $"D[{input}]",
                    PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet => $"DB[{input}]",
                    PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet => $"!DB_old[{input}] && DB[{input}]",
                    PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie => $"ED[{input}]",
                    PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat => $"TDH_old[{input}] && !TDH[{input}]",
                    PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartIngang => $"IS[{input}] && !IS_old[{input}]",
                    PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.IngangHoog => $"IS[{input}]",
                    PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeIngang => $"IS_old[{input}] && !IS[{input}]",
                    _ => null
                };
            }
        }
    }
}
