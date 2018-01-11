using GalaSoft.MvvmLight.Messaging;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class OVIngreepSignaalGroepParametersChangedMessage : MessageBase
    {
        public OVIngreepSignaalGroepParametersModel SignaalGroepParameters { get; }

        public OVIngreepSignaalGroepParametersChangedMessage(OVIngreepSignaalGroepParametersModel signaalgroepparameters)
        {
            SignaalGroepParameters = signaalgroepparameters;
        }
    }
}
