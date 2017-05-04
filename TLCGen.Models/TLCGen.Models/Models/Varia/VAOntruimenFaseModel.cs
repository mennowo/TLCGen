using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseCyclus")]
    public class VAOntruimenFaseModel
    {
        public string FaseCyclus { get; set; }
        public int VAOntrTijdensRood { get; set; }

        [XmlArrayItem(ElementName = "VADetector")]
        public List<VAOntruimenDetectorModel> VADetectoren { get; set; }

        public VAOntruimenFaseModel()
        {
            VADetectoren = new List<VAOntruimenDetectorModel>();
        }
    }
}
