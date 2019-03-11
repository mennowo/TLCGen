using System;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseCyclus")]
    public class HDIngreepMeerealiserendeFaseCyclusModel : IComparable
    {
        [XmlText]
        [HasDefault(false)]
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