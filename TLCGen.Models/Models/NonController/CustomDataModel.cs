using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class CustomDataModel
    {
        [XmlArrayItem(ElementName = "GeneratorData")]
        public List<GeneratorDataModel> GeneratorsData { get; set; }

        public CustomDataModel()
        {
            GeneratorsData = new List<GeneratorDataModel>();
        }
    }
}
