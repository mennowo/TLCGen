using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class ModuleFaseCyclusModel
    {
        public string FaseCyclus { get; set; }

        [XmlArrayItem(ElementName = "Fase")]
        public List<string> Alternatieven { get; set; }

        public ModuleFaseCyclusModel()
        {
            Alternatieven = new List<string>();
        }

    }
}
