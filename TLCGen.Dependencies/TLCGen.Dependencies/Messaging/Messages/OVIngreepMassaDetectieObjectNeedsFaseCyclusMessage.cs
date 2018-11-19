namespace TLCGen.Messaging.Messages
{
    public class OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage
    {
        public object RequestingObject { get; }
        public string FaseCyclus { get; set; }

        public OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage(object requestingObject)
        {
            RequestingObject = requestingObject;
        }
    }
}
