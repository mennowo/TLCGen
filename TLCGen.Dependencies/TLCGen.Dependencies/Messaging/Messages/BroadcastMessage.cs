namespace TLCGen.Messaging.Messages
{
    public class BroadcastMessage(object broadcastObject)
    {
        public object BroadcastObject { get; } = broadcastObject;
    }
}
