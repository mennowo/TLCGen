using System.Text;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: meldingen of type AutoMassaPeloton. NOT IMPLEMENTED: no condition is generated, which leaves an
    // incomplete statement "IH[hulp] = " in the C code (todo note 1579).
    public partial class PrioCodeGenerator
    {
        private sealed class AutoPelotonMeldingEmitter : PrioMeldingEmitter
        {
            public AutoPelotonMeldingEmitter(PrioCodeGenerator generator) : base(generator)
            {
            }

            public override void AppendCondition(PrioMeldingContext m, StringBuilder sb)
            {
            }
        }
    }
}
