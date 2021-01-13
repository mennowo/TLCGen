using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class CCOLVersionChangedMessage
    {
        public CCOLVersieEnum OldVersion { get; }
        public CCOLVersieEnum NewVersion { get; }

        public CCOLVersionChangedMessage(CCOLVersieEnum oldVersion, CCOLVersieEnum newVersion)
        {
            OldVersion = oldVersion;
            NewVersion = newVersion;
        }
    }
}