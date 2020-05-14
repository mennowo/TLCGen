using TLCGen.Models;

namespace TLCGen.Dependencies.Messaging.Messages
{
    public class ControllerLoadedMessage
    {
        public ControllerModel Controller { get; }

        public ControllerLoadedMessage(ControllerModel controller)
        {
            Controller = controller;
        }
    }
}
