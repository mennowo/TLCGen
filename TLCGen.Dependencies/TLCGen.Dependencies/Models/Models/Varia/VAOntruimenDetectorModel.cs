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

        /// <summary>
        /// Signal groups that are not (automatically added) hard conflicts of the fase, but for which
        /// VA ontruimen must still be applied - typically deelconflicten. These are user data: the
        /// automatically maintained list above is left untouched by the user. The list mirrors the
        /// automatic one (one entry per signal group and detector) so each detector has its own time.
        /// </summary>
        [XmlArrayItem(ElementName = "ManualConflict")]
        public List<VAOntruimenNaarFaseModel> ManualConflicts { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        [HasDefault(false)]
        public string Detector { get; set; }

        public VAOntruimenDetectorModel()
        {
            ConflicterendeFasen = new List<VAOntruimenNaarFaseModel>();
            ManualConflicts = new List<VAOntruimenNaarFaseModel>();
        }
    }
}
