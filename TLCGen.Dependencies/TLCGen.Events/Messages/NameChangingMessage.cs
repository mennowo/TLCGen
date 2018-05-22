using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
	public class NameChangingMessage
    {
        public TLCGenObjectTypeEnum ObjectType { get; }
        public string OldName { get; }
        public string NewName { get; }

        public NameChangingMessage(TLCGenObjectTypeEnum objectType, string oldname, string newname)
        {
            ObjectType = objectType;
            OldName = oldname;
            NewName = newname;
        }
    }
}
