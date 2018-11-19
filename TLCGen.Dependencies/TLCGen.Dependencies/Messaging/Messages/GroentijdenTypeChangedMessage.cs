
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
	public class GroentijdenTypeChangedMessage
    {
        public GroentijdenTypeEnum Type { get; }

        public GroentijdenTypeChangedMessage(GroentijdenTypeEnum type)
        {
            Type = type;
        }
    }
}
