using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class PTPDataModel
    {
        public string KruisingPTPNaam { get; set; }

        [XmlArrayItem(ElementName = "PTPKoppeling")]
        public List<PTPKoppelingModel> PTPKoppelingen { get; set; }

        public PTPDataModel()
        {
            PTPKoppelingen = new List<PTPKoppelingModel>();
        }
    }
}
