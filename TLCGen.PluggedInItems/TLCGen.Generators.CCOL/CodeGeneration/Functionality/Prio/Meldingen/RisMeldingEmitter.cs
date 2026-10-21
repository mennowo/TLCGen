using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: meldingen of type RISVoorwaarde: selective in- and uitmelding via RIS (ris_inmelding_selectief per lane).
    // The code is enclosed in #ifndef NO_RIS. The elements are declared in Elements.cs (RIS parameters per ingreep).
    public partial class PrioCodeGenerator
    {
        private sealed class RisMeldingEmitter : PrioMeldingEmitter
        {
            public RisMeldingEmitter(PrioCodeGenerator generator) : base(generator)
            {
            }

            public override bool WrapInNoRis => true;

            public override bool ChecksAntiJutterTimers => false;

            public override void AppendCondition(PrioMeldingContext m, StringBuilder sb)
            {
                var c = m.C;
                var prio = m.Prio;
                var ts = m.Ts;
                var prioName = m.PrioName;
                switch (m.Melding.InUit)
                {
                    case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                        var risFc = c.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == prio.FaseCyclus);
                        var first = true;

                        if (risFc != null)
                        {
                            sb.AppendLine();
                            foreach (var lane in risFc.LaneData)
                            {
                                if (!first) sb.AppendLine(" ||");
                                var itf = c.RISData.HasMultipleSystemITF
                                    ? c.RISData.MultiSystemITF.FindIndex(x => x.SystemITF == lane.SystemITF) : -1;
                                sb.Append($"{ts}{ts}{ts}ris_inmelding_selectief(" +
                                          $"{G._fcpf}{prio.FaseCyclus}, " +
                                          $"PRM[{G._prmpf}{G._prmrisapproachid}{prioName}], " +
                                          $"SYSTEM_ITF{(itf >= 0 ? (itf + 1).ToString() : "")}, " +
                                          $"PRM[{G._prmpf}{G._prmrislaneid}{prioName}_{lane.RijstrookIndex}], " +
                                          $"PRM[{G._prmpf}{G._prmrisstationtype}{prioName}], " +
                                          $"PRM[{G._prmpf}{G._prmrisstart}{prioName}], " +
                                          $"PRM[{G._prmpf}{G._prmrisend}{prioName}], " +
                                          $"PRM[{G._prmpf}{G._prmrisrole}{prioName}], " +
                                          $"PRM[{G._prmpf}{G._prmrissubrole}{prioName}], " +
                                          $"PRM[{G._prmpf}{G._prmrisimportance}{prioName}], " +
                                          $"{(m.Melding.RisEta.HasValue ? $"PRM[{G._prmpf}{G._prmriseta}{prioName}]" : "NG")}, " +
                                          $"prioFC{prioName})");
                                first = false;
                            }
                            sb.AppendLine(";");
                        }
                        break;
                    case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                        sb.AppendLine($"({c.GetBoolV()})ris_uitmelding_selectief(prioFC{prioName});");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public override void AppendAfterCondition(PrioMeldingContext m, StringBuilder sb, IReadOnlyList<string> hulpElements)
            {
                var ts = m.Ts;
                if (m.IsInmelding)
                    sb.AppendLine($"{ts}{ts}if (IH[{hulpElements.Last()}]) iPrioriteitNooitAfkappen[prioFC{m.PrioName}] = TRUE;");
                else
                    sb.AppendLine($"{ts}{ts}if (IH[{hulpElements.Last()}] && iAantalInmeldingen[prioFC{m.PrioName}] == 0) iPrioriteitNooitAfkappen[prioFC{m.PrioName}] = FALSE;");
            }
        }
    }
}
