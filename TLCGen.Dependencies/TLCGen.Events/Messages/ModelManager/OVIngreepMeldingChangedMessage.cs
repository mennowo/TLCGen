using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class OVIngreepMeldingChangedMessage : ModelManagerMessageBase
    {
        public string FaseCyclus { get; }
        public OVIngreepMeldingTypeEnum MeldingType { get; }

        public OVIngreepMeldingChangedMessage(string faseCyclus, OVIngreepMeldingTypeEnum type)
        {
            FaseCyclus = faseCyclus;
            MeldingType = type;
        }
    }
}
