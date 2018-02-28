using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    public class OVIngreepMassaDetectieMeldingModel
    {
        public string Omschrijving { get; set; }
        public OVIngreepMassaDetectieMeldingType Type { get; set; }
        [XmlArrayItem("VoorwaardenSet")]
        public List<OVIngreepMassaDetectieMeldingVoorwaardenSetModel> VoorwaardenSets { get; set; }

        public OVIngreepMassaDetectieMeldingModel()
        {
            VoorwaardenSets = new List<OVIngreepMassaDetectieMeldingVoorwaardenSetModel>();
        }
    }
}
