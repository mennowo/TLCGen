namespace TLCGen.Messaging.Messages
{
    public class BroadcastMessage(object broadcastObject) : MessageBase
    {
        public object BroadcastObject { get; } = broadcastObject;
    }
}
