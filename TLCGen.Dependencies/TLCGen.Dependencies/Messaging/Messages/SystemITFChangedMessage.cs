namespace TLCGen.Dependencies.Messaging.Messages
{
    public class SystemITFChangedMessage
    {
        public string OldSystemITF { get; }
        public string NewdSystemITF { get; }

        public SystemITFChangedMessage(string oldSystemITF, string newdSystemITF)
        {
            OldSystemITF = oldSystemITF;
            NewdSystemITF = newdSystemITF;
        }
    }
}
