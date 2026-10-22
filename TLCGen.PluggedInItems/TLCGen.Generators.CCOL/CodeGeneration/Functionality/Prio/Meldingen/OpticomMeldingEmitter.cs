using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: meldingen of type Opticom: inmelding on an occupied Opticom detector, uitmelding at the end of
    // detection of the detector of the Opticom uitmelding. Enabled by the regular switch of the melding only.
    // NOTE: the names used here do not match the declared elements in all cases (todo note 1576).
    public partial class PrioCodeGenerator
    {
        private sealed class OpticomMeldingEmitter : PrioMeldingEmitter
        {
            public OpticomMeldingEmitter(PrioCodeGenerator generator) : base(generator)
            {
            }

            public override bool RestartsAntiJutterTimer => false;

            public override void AddElementsBeforeMelding(PrioMeldingContext m, List<CCOLElement> elements)
            {
                var melding = m.Melding;
                if (!m.IsInmelding || string.IsNullOrWhiteSpace(melding.RelatedInput1)) return;

                var fc = m.Prio.FaseCyclus;
                var b = m.Names.Base;
                var provider = CCOLGeneratorSettingsProvider.Default;
                elements.Add(provider.CreateElement($"{G._hprioin}{b}", G._hprioin, fc, "Opticom"));
                elements.Add(provider.CreateElement($"{G._tprioin}{b}", melding.AntiJutterTijdToepassen ? melding.AntiJutterTijd : 0, CCOLElementTimeTypeEnum.TE_type, G._tprioin, fc, "Opticom"));
                elements.Add(provider.CreateElement($"{G._hpriouit}{b}", G._hpriouit, fc, "Opticom"));
            }

            public override void AppendCondition(PrioMeldingContext m, StringBuilder sb)
            {
                var prio = m.Prio;
                var ts = m.Ts;
                var b = m.Names.Base;
                switch (m.Melding.InUit)
                {
                    case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                        sb.AppendLine($"!C[{G._ctpf}{G._cvc}{m.PrioName}] && DB[{G._dpf}{m.Melding.RelatedInput1}];");
                        sb.AppendLine($"{ts}{ts}RT[{m.Timer}] = G[{G._fcpf}{prio.FaseCyclus}] && C[{G._ctpf}{G._cvc}{m.PrioName}];");
                        break;
                    case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                        var uitMelding = prio.MeldingenData.Uitmeldingen.FirstOrDefault(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom);
                        if (uitMelding != null && uitMelding.RelatedInput1 != null)
                        {
                            sb.AppendLine($"!TDH[{G._dpf}{uitMelding.RelatedInput1}] && TDH_old[{G._dpf}{uitMelding.RelatedInput1}];");
                        }
                        break;
                }
            }
        }
    }
}
