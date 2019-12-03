using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class PrioIngreepMeldingChangedMessage : ModelManagerMessageBase
    {
        public PrioIngreepInUitMeldingVoorwaardeTypeEnum MeldingType { get; }
        public string FaseCyclus { get; }

        public PrioIngreepMeldingChangedMessage(string faseCyclus, PrioIngreepInUitMeldingVoorwaardeTypeEnum type)
        {
            FaseCyclus = faseCyclus;
            MeldingType = type;
        }
    }
}
