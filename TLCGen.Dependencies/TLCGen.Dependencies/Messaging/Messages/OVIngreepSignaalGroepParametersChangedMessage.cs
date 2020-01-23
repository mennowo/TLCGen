
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class PrioIngreepSignaalGroepParametersChangedMessage
    {
        public PrioIngreepSignaalGroepParametersModel SignaalGroepParameters { get; }

        public PrioIngreepSignaalGroepParametersChangedMessage(PrioIngreepSignaalGroepParametersModel signaalgroepparameters)
        {
            SignaalGroepParameters = signaalgroepparameters;
        }
    }
}
