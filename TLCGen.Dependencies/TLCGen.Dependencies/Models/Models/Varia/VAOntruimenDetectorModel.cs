using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Detector, "Detector")]
    public class VAOntruimenDetectorModel
    {
        [XmlArrayItem(ElementName = "ConflictendeFase")]
        public List<VAOntruimenNaarFaseModel> ConflicterendeFasen { get; set; }

        [HasDefault(false)]
        public string Detector { get; set; }

        public VAOntruimenDetectorModel()
        {
            ConflicterendeFasen = new List<VAOntruimenNaarFaseModel>();
        }
    }
}
