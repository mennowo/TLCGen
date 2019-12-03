namespace TLCGen.Messaging.Messages
{
    public class PrioIngreepMassaDetectieObjectNeedsFaseCyclusMessage
    {
        public object RequestingObject { get; }
        public string FaseCyclus { get; set; }

        public PrioIngreepMassaDetectieObjectNeedsFaseCyclusMessage(object requestingObject)
        {
            RequestingObject = requestingObject;
        }
    }
}
