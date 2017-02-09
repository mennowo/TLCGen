using System;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseCyclus")]
    public class HDIngreepMeerealiserendeFaseCyclusModel
    {
        [XmlText]
        public string FaseCyclus { get; set; }
    }
}