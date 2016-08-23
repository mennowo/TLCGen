using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class ModuleModel
    {
        public string Naam { get; set; }

        [XmlArrayItem(ElementName = "FaseCyclus")]
        public List<string> Fasen { get; set; }

        public ModuleModel()
        {
            Fasen = new List<string>();
        }
    }
}
