using System.Text;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: meldingen of type VecomViaDetector: start of detection on the Vecom detector (RelatedInput1).
    public partial class PrioCodeGenerator
    {
        private sealed class VecomMeldingEmitter : PrioMeldingEmitter
        {
            public VecomMeldingEmitter(PrioCodeGenerator generator) : base(generator)
            {
            }

            public override string GetStoringCondition(PrioMeldingContext m)
            {
                return string.IsNullOrWhiteSpace(m.Melding.RelatedInput1)
                    ? ""
                    : $" && (CIF_IS[{G._dpf}{m.Melding.RelatedInput1}] < CIF_DET_STORING)";
            }

            public override void AppendCondition(PrioMeldingContext m, StringBuilder sb)
            {
                sb.AppendLine($" SD[{G._dpf}{m.Melding.RelatedInput1}];");
            }
        }
    }
}
