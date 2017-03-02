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
            var they = obj as HDIngreepMeerealiserendeFaseCyclusModel;
            if(they != null)
            {
                return this.FaseCyclus.CompareTo(they.FaseCyclus);
            }
            throw new NotImplementedException();
        }
    }
}