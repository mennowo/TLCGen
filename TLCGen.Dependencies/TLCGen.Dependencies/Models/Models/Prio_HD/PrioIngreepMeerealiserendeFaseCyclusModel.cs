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
	        if (obj is not PrioIngreepMeerealiserendeFaseCyclusModel they)
	        {
		        throw new InvalidCastException();
	        }
	        return string.Compare(FaseCyclus, they.FaseCyclus, StringComparison.Ordinal);
        }
    }

    [Serializable]
    public class PrioIngreepMeerealiserendeIngreepModel : IComparable
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [XmlText]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }

        [HasDefault(false)]
        public string PrioIngreep { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is not PrioIngreepMeerealiserendeIngreepModel they)
            {
                throw new InvalidCastException();
            }
            return string.Compare(PrioIngreep, they.PrioIngreep, StringComparison.Ordinal);
        }
    }
}