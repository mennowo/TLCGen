using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class PelotonKoppelingenDataModel
    {
        [XmlArrayItem(ElementName = "PelotonKoppeling")]
        public List<PelotonKoppelingModel> PelotonKoppelingen { get; set; }

        public PelotonKoppelingenDataModel()
        {
            PelotonKoppelingen = new List<PelotonKoppelingModel>();
        }
    }
}
