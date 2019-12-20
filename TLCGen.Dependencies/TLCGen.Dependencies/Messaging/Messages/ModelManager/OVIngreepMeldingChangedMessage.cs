using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class PrioIngreepMeldingChangedMessage : ModelManagerMessageBase
    {
        public PrioIngreepInUitMeldingModel IngreepMelding { get; }
        public string FaseCyclus { get; }

        public PrioIngreepMeldingChangedMessage(string faseCyclus, PrioIngreepInUitMeldingModel ingreepMelding)
        {
            FaseCyclus = faseCyclus;
            IngreepMelding = ingreepMelding;
        }
    }
}
