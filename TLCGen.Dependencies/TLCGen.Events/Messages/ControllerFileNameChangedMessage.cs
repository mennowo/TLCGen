using GalaSoft.MvvmLight.Messaging;

namespace TLCGen.Messaging.Messages
{
	public class ControllerFileNameChangedMessage : MessageBase
    {
        public string OldFileName { get; private set; }
        public string NewFileName { get; private set; }

        public ControllerFileNameChangedMessage(string newname, string oldname)
        {
            OldFileName = oldname;
            NewFileName = newname;
        }
    }
}
