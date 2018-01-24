

namespace TLCGen.Messaging.Messages
{
	public class NameChangedMessage
    {
        public string OldName { get; }
        public string NewName { get; }

        public NameChangedMessage(string oldname, string newname)
        {
            OldName = oldname;
            NewName = newname;
        }
    }
}
