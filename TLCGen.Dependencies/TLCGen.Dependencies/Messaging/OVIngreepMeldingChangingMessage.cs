using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class OVIngreepMeldingChangingMessage : ModelManagerMessageBase
    {
        public OVIngreepModel Ingreep;
        public string FaseCyclus;
        public OVIngreepInUitMeldingVoorwaardeTypeEnum MeldingType;

        public OVIngreepMeldingChangingMessage(OVIngreepModel ingreep, string faseCyclus, OVIngreepInUitMeldingVoorwaardeTypeEnum type)
        {
            FaseCyclus = faseCyclus;
            Ingreep = ingreep;
            MeldingType = type;
        }
    }
}