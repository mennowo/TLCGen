using GalaSoft.MvvmLight.Messaging;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
	public class GroentijdenTypeChangedMessage : MessageBase
    {
        public GroentijdenTypeEnum Type { get; }

        public GroentijdenTypeChangedMessage(GroentijdenTypeEnum type)
        {
            Type = type;
        }
    }
}
