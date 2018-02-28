using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [RefersTo("Detector")]
    public class OVIngreepMassaDetectieMeldingVoorwaardeModel
    {
        public string Omschrijving { get; set; }
        public OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum Type { get; set; }
        public string Detector { get; set; }
        public string WisselContact { get; set; }

        public OVIngreepMassaDetectieMeldingVoorwaardeModel()
        {

        }
    }
}
