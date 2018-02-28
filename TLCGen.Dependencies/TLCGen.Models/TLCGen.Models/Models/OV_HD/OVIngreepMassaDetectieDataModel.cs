using System.Collections.Generic;

namespace TLCGen.Models
{
    public class OVIngreepMassaDetectieDataModel
    {
        public List<OVIngreepMassaDetectieMeldingModel> Meldingen { get; set; }

        public OVIngreepMassaDetectieDataModel()
        {
            Meldingen = new List<OVIngreepMassaDetectieMeldingModel>();
        }
    }
}
