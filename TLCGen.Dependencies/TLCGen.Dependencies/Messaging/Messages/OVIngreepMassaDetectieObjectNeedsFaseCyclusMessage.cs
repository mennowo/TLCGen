namespace TLCGen.Messaging.Messages
{
    public class PrioIngreepMeldingNeedsFaseCyclusAndIngreepMessage
    {
        public object RequestingObject { get; }
        public string FaseCyclus { get; set; }
        public string Ingreep { get; set; }

        public PrioIngreepMeldingNeedsFaseCyclusAndIngreepMessage(object requestingObject)
        {
            RequestingObject = requestingObject;
        }
    }
}
