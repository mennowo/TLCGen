
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class OVIngreepSignaalGroepParametersChangedMessage
    {
        public OVIngreepSignaalGroepParametersModel SignaalGroepParameters { get; }

        public OVIngreepSignaalGroepParametersChangedMessage(OVIngreepSignaalGroepParametersModel signaalgroepparameters)
        {
            SignaalGroepParameters = signaalgroepparameters;
        }
    }
}
