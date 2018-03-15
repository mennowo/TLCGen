using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo("Detector")]
    public class VAOntruimenDetectorModel
    {
        [XmlArrayItem(ElementName = "ConflictendeFase")]
        public List<VAOntruimenNaarFaseModel> ConflicterendeFasen { get; set; }

        public string Detector { get; set; }

        public VAOntruimenDetectorModel()
        {
            ConflicterendeFasen = new List<VAOntruimenNaarFaseModel>();
        }
    }
}
