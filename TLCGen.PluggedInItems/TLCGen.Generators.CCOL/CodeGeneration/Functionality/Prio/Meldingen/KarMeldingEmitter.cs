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
                var hd = prio.Type == PrioIngreepVoertuigTypeEnum.Hulpdienst;
                var cif = m.IsInmelding ? "CIF_DSIN, " : "CIF_DSUIT, ";
                var extra = G.GetDsiExtraCondition(m, false);

                string GetDsiCall(string sg) => hd
                    ? $"DSIMelding_HD_V1({sg}, {cif}(!SCH[{G._schpf}{G._schchecksirene}{m.PrioName}] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR))"
                    : $"DSIMeldingPRIO{(m.IsInmelding ? "_V1" : "_V2")}(" +
                      (m.IsInmelding ? "" : $"{G._fcpf}{prio.FaseCyclus}, prioFC{m.PrioName}, ") +
                      $"0, {m.VtgType}, TRUE, {sg},TRUE, {cif}{extra})";

                var sgCheck = G.GetKarSignalGroup(c, G._prmkarsg, prio.FaseCyclus, m.FcNmr);
                sb.AppendLine();
                sb.Append($"{m.Tts}{m.Ts}{GetDsiCall(hd || m.FcNmr != -1 ? sgCheck : "NG")}");
                if (hd && prio.InmeldingOokDoorToepassen && prio.InmeldingOokDoorFase > 0)
                {
                    var alsoFc = G.GetKarSignalGroup(c, G._prmkarsg, GetOokDoorFaseName(prio.InmeldingOokDoorFase), prio.InmeldingOokDoorFase);
                    sb.AppendLine(" ||");
                    sb.Append($"{m.Tts}{m.Ts}{GetDsiCall(alsoFc)}");
                }
                sb.AppendLine(";");
            }
        }
    }
}
