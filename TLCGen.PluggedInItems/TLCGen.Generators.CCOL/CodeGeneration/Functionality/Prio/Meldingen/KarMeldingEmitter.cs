using System.Text;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: meldingen of type KARMelding: a KAR DSI melding for the signal group of the ingreep. Hulpdienst
    // ingrepen may also be triggered by a melding for another signal group (InmeldingOokDoorFase).
    public partial class PrioCodeGenerator
    {
        private sealed class KarMeldingEmitter : PrioMeldingEmitter
        {
            public KarMeldingEmitter(PrioCodeGenerator generator) : base(generator)
            {
            }

            public override bool SupportsOpvangStoring => false;

            public override void AppendCondition(PrioMeldingContext m, StringBuilder sb)
            {
                var c = m.C;
                var prio = m.Prio;
                var ts = m.Ts;
                var tts = m.Tts;
                var fcNmr = m.FcNmr;
                var prioName = m.PrioName;
                var extra = G.GetDsiExtraCondition(m, false);

                var sgCheck = c.PrioData.KARSignaalGroepNummersInParameters
                        ? $"PRM[{G._prmpf}{G._prmkarsg}{prio.FaseCyclus}]"
                        : fcNmr > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers ? (fcNmr - 200).ToString() : fcNmr.ToString();
                sb.AppendLine();
                if (prio.Type == PrioIngreepVoertuigTypeEnum.Hulpdienst)
                {
                    sb.Append($"{tts}{ts}DSIMelding_HD_V1({sgCheck}, " +
                                                          (m.IsInmelding
                                                              ? "CIF_DSIN, "
                                                              : "CIF_DSUIT, ") +
                                                          $"(!SCH[{G._schpf}{G._schchecksirene}{prioName}] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR))");
                }
                else
                {
                    sb.Append($"{tts}{ts}DSIMeldingPRIO{(m.IsInmelding ? "_V1" : "_V2")}(" +
                                                    (m.IsInmelding
                                                        ? ""
                                                        : $"{G._fcpf}{prio.FaseCyclus}, prioFC{prioName}, ") +
                                                    $"0, " +
                                                    $"{m.VtgType}, " +
                                                    "TRUE, " +
                                                    $"{(fcNmr == -1 ? "NG" : sgCheck)}," +
                                                    "TRUE, " +
                                                    (m.IsInmelding
                                                        ? "CIF_DSIN, "
                                                        : "CIF_DSUIT, ") +
                                                    $"{extra})");
                }
                if (prio.Type == PrioIngreepVoertuigTypeEnum.Hulpdienst &&
                    prio.InmeldingOokDoorToepassen && prio.InmeldingOokDoorFase > 0)
                {
                    var hdpriofcnr = (prio.InmeldingOokDoorFase.ToString().Length < 2) ? "0" + prio.InmeldingOokDoorFase.ToString()
                                                                                             : prio.InmeldingOokDoorFase.ToString();
                    var actualAlsoFc = c.PrioData.KARSignaalGroepNummersInParameters
                        ? $"PRM[{G._prmpf}{G._prmkarsg}{hdpriofcnr}]"
                        : prio.InmeldingOokDoorFase > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers
                            ? (prio.InmeldingOokDoorFase - 200).ToString() : prio.InmeldingOokDoorFase.ToString();
                    sb.AppendLine(" ||");
                    if (prio.Type == PrioIngreepVoertuigTypeEnum.Hulpdienst)
                    {
                        sb.Append($"{tts}{ts}DSIMelding_HD_V1({actualAlsoFc}, " +
                                                              (m.IsInmelding
                                                                  ? "CIF_DSIN, "
                                                                  : "CIF_DSUIT, ") +
                                                              $"(!SCH[{G._schpf}{G._schchecksirene}{prioName}] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR))");
                    }
                    else
                    {
                        sb.Append($"{tts}{ts}DSIMeldingPRIO{(m.IsInmelding ? "_V1" : "_V2")}(" +
                                                (m.IsInmelding
                                                    ? ""
                                                    : $"{G._fcpf}{prio.FaseCyclus}, prioFC{prioName}, ") +
                                                $"0, " +
                                                $"{m.VtgType}, " +
                                                "TRUE, " +
                                                $"{actualAlsoFc}," +
                                                "TRUE, " +
                                                (m.IsInmelding
                                                    ? "CIF_DSIN, "
                                                    : "CIF_DSUIT, ") +
                                                $"{extra})");
                    }
                }
                sb.AppendLine(";");
            }
        }
    }
}
