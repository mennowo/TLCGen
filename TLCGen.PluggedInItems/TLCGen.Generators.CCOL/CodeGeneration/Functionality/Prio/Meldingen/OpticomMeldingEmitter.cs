using System.Text;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: meldingen of type Opticom: inmelding on an occupied Opticom detector, uitmelding at the end of
    // detection of the detector of the uitmelding. Uses the regular elements of the melding only.
    public partial class PrioCodeGenerator
    {
        private sealed class OpticomMeldingEmitter : PrioMeldingEmitter
        {
            public OpticomMeldingEmitter(PrioCodeGenerator generator) : base(generator)
            {
            }

            public override bool RestartsAntiJutterTimer => false;

            public override void AppendCondition(PrioMeldingContext m, StringBuilder sb)
            {
                var prio = m.Prio;
                var ts = m.Ts;
                var input = m.Melding.RelatedInput1;
                switch (m.Melding.InUit)
                {
                    case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                        sb.AppendLine($"!C[{G._ctpf}{G._cvc}{m.PrioName}] && DB[{G._dpf}{input}];");
                        if (m.Melding.AntiJutterTijdToepassen)
                        {
                            sb.AppendLine($"{ts}{ts}RT[{m.Timer}] = G[{G._fcpf}{prio.FaseCyclus}] && C[{G._ctpf}{G._cvc}{m.PrioName}];");
                        }
                        break;
                    case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                        if (input != null)
                        {
                            sb.AppendLine($"!TDH[{G._dpf}{input}] && TDH_old[{G._dpf}{input}];");
                        }
                        break;
                }
            }
        }
    }
}
