using System;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseCyclus")]
    public class HDIngreepMeerealiserendeFaseCyclusModel : IComparable
    {
        [XmlText]
        public string FaseCyclus { get; set; }

        public int CompareTo(object obj)
        {
	        if (!(obj is HDIngreepMeerealiserendeFaseCyclusModel they))
	        {
		        throw new InvalidCastException();
	        }
	        return string.Compare(FaseCyclus, they.FaseCyclus, StringComparison.Ordinal);
        }
    }
}