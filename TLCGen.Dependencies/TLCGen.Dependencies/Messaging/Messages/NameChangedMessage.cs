using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class SynchronisatiesTypeChangedMessage
    {
        
    }
    
	public class NameChangedMessage
    {
        public TLCGenObjectTypeEnum ObjectType { get; }
        public string OldName { get; }
        public string NewName { get; }

        public NameChangedMessage(TLCGenObjectTypeEnum objectType, string oldname, string newname)
        {
            ObjectType = objectType;
            OldName = oldname;
            NewName = newname;
        }
    }
}
