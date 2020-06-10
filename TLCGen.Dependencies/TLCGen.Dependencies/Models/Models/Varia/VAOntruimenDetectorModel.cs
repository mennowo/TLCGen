using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class VAOntruimenDetectorModel
    {
        [XmlArrayItem(ElementName = "ConflictendeFase")]
        public List<VAOntruimenNaarFaseModel> ConflicterendeFasen { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        [HasDefault(false)]
        public string Detector { get; set; }

        public VAOntruimenDetectorModel()
        {
            ConflicterendeFasen = new List<VAOntruimenNaarFaseModel>();
        }
    }
}
