using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class PrioIngreepMeldingChangedMessage : ModelManagerMessageBase
    {
        public PrioIngreepInUitMeldingModel IngreepMelding { get; }
        public string FaseCyclus { get; }
        public bool Removing { get; }

        public PrioIngreepMeldingChangedMessage(string faseCyclus, PrioIngreepInUitMeldingModel ingreepMelding, bool removing = false)
        {
            FaseCyclus = faseCyclus;
            IngreepMelding = ingreepMelding;
            Removing = removing;
        }
    }
}
