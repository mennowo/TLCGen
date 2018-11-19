using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class OVIngreepMeldingChangedMessage : ModelManagerMessageBase
    {
        public OVIngreepInUitMeldingVoorwaardeTypeEnum MeldingType { get; }
        public string FaseCyclus { get; }

        public OVIngreepMeldingChangedMessage(string faseCyclus, OVIngreepInUitMeldingVoorwaardeTypeEnum type)
        {
            FaseCyclus = faseCyclus;
            MeldingType = type;
        }
    }
}
