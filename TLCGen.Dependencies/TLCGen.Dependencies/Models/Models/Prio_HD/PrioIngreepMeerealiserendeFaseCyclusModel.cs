using System;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class PrioIngreepMeerealiserendeFaseCyclusModel : IComparable
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [XmlText]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }

        public int CompareTo(object obj)
        {
	        if (!(obj is PrioIngreepMeerealiserendeFaseCyclusModel they))
	        {
		        throw new InvalidCastException();
	        }
	        return string.Compare(FaseCyclus, they.FaseCyclus, StringComparison.Ordinal);
        }
    }
}