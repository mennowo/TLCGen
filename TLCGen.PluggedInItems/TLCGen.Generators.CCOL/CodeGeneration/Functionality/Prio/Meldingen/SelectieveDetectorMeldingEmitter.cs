using System.Text;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: meldingen of type SelectieveDetector: a DSI melding on a selective detector (RelatedInput1).
    public partial class PrioCodeGenerator
    {
        private sealed class SelectieveDetectorMeldingEmitter : PrioMeldingEmitter
        {
            public SelectieveDetectorMeldingEmitter(PrioCodeGenerator generator) : base(generator)
            {
            }

            public override void AppendCondition(PrioMeldingContext m, StringBuilder sb)
            {
                var melding = m.Melding;
                sb.AppendLine();
                sb.AppendLine($"{m.Tts}{m.Ts}DSIMeldingPRIO_V1({(G._dpf + melding.RelatedInput1).ToUpper()}, " +
                              $"{m.VtgType}, " +
                              "FALSE, " +
                              "NG, " +
                              $"SCH[{G._schpf}{G._schcheckdstype}], " +
                              (m.IsInmelding || melding.CheckAltijdOpDsinBijVecom
                                  ? "CIF_DSIN, "
                                  : "CIF_DSUIT, ") +
                              $"{G.GetDsiExtraCondition(m, m.Prio.CheckRitCategorie)});");
            }
        }
    }
}
