using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    public class OVIngreepMassaDetectieMeldingVoorwaardenSetModel
    {
        public string Omschrijving { get; set; }
        [XmlArrayItem(ElementName = "Voorwaarde")]
        public List<OVIngreepMassaDetectieMeldingVoorwaardeModel> Voorwaarden { get; set; }

        public OVIngreepMassaDetectieMeldingVoorwaardenSetModel()
        {
            Voorwaarden = new List<OVIngreepMassaDetectieMeldingVoorwaardeModel>();
        }
    }
}
