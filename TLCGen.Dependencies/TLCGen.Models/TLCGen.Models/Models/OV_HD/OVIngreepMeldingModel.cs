using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    public class OVIngreepMeldingModel
    {
        [RefersTo]
        public string FaseCyclus { get; set; }
        public OVIngreepMeldingTypeEnum Type { get; set; }
        public bool Inmelding { get; set; }
        public bool Uitmelding { get; set; }
        [RefersTo]
        public string RelatedInput { get; set; }
        public int? InmeldingFilterTijd { get; set; }

        public OVIngreepMeldingModel()
        {
        }
    }
}
