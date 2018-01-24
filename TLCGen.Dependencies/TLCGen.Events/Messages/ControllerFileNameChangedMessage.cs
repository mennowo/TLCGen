

namespace TLCGen.Messaging.Messages
{
	public class ControllerFileNameChangedMessage
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
