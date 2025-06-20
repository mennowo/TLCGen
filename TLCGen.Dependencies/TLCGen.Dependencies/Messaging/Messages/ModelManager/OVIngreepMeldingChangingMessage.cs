using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class OVIngreepMeldingChangingMessage : ModelManagerMessageBase
    {
        public PrioIngreepModel Ingreep;
        public string FaseCyclus;
        public PrioIngreepInUitMeldingVoorwaardeTypeEnum MeldingType;

        public OVIngreepMeldingChangingMessage(PrioIngreepModel ingreep, string faseCyclus, PrioIngreepInUitMeldingVoorwaardeTypeEnum type)
        {
            FaseCyclus = faseCyclus;
            Ingreep = ingreep;
            MeldingType = type;
        }
    }
}