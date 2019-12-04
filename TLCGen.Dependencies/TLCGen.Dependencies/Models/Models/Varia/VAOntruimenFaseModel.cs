using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseCyclus")]
    public class VAOntruimenFaseModel
    {
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public int VAOntrMax { get; set; }
        public bool KijkNaarWisselstand { get; set; }
        public PrioIngreepInUitDataWisselTypeEnum Wissel1Type { get; set; }
        [HasDefault(false)]
        public string Wissel1Input { get; set; }
        [HasDefault(false)]
        public string Wissel1Detector { get; set; }
        public bool Wissel2 { get; set; }
        public PrioIngreepInUitDataWisselTypeEnum Wissel2Type { get; set; }
        [HasDefault(false)]
        public string Wissel2Input { get; set; }
        [HasDefault(false)]
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
