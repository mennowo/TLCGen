namespace TLCGen.Messaging.Messages
{
    public class OVIngreepMassaDetectieObjectActionMessage
    {
        public bool Add { get; }
        public bool Remove { get; }
        public object Object { get; }

        public OVIngreepMassaDetectieObjectActionMessage(object @object, bool add, bool remove)
        {
            Object = @object;
            Add = add;
            Remove = remove;
        }
    }
}
