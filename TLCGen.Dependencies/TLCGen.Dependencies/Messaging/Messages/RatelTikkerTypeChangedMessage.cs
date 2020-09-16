using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class RatelTikkerTypeChangedMessage
    {
        public RatelTikkerModel RatelTikker { get; }

        public RatelTikkerTypeChangedMessage(RatelTikkerModel ratelTikker)
        {
            RatelTikker = ratelTikker;
        }
    }
}
