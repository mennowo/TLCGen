
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
	public class ControllerHasOVChangedMessage
    {
        public OVIngreepTypeEnum Type { get; private set; }

        public ControllerHasOVChangedMessage(OVIngreepTypeEnum type)
        {
            Type = type;
        }
    }
}
