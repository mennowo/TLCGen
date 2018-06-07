using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo("FaseCyclus")]
    public class VAOntruimenFaseModel
    {
        public string FaseCyclus { get; set; }
        public int VAOntrMax { get; set; }
        public bool KijkNaarWisselstand { get; set; }
        public OVIngreepInUitDataWisselTypeEnum Wissel1Type { get; set; }
        public string Wissel1Input { get; set; }
        public string Wissel1Detector { get; set; }
        public bool Wissel2 { get; set; }
        public OVIngreepInUitDataWisselTypeEnum Wissel2Type { get; set; }
        public string Wissel2Input { get; set; }
        public string Wissel2Detector { get; set; }
        public bool Wissel1InputVoorwaarde { get; set; }
        public bool Wissel2InputVoorwaarde { get; set; }

        [XmlArrayItem(ElementName = "VADetector")]
        public List<VAOntruimenDetectorModel> VADetectoren { get; set; }

        public VAOntruimenFaseModel()
        {
            VADetectoren = new List<VAOntruimenDetectorModel>();
        }
    }
}
