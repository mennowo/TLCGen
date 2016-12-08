using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class HDIngreepModel
    {
        public string FaseCyclus { get; set; }
        public bool KAR { get; set; }
        public bool Vecom { get; set; }
        public bool Opticom { get; set; }
        public bool Sirene { get; set; }
        public int GroenBewaking { get; set; }

        [XmlArrayItem(ElementName = "MeerealiserendeFaseCyclus")]
        public List<HDIngreepMeerealiserendeFaseCyclusModel> MeerealiserendeFaseCycli { get; set; }

        public HDIngreepModel()
        {
            MeerealiserendeFaseCycli = new List<HDIngreepMeerealiserendeFaseCyclusModel>();
        }
    }
}
