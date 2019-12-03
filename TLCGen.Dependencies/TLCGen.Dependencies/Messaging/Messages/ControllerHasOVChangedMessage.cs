
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
	public class ControllerHasOVChangedMessage
    {
        public PrioIngreepTypeEnum Type { get; private set; }

        public ControllerHasOVChangedMessage(PrioIngreepTypeEnum type)
        {
            Type = type;
        }
    }
}
