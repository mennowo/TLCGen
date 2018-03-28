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
        public string RelatedInput1 { get; set; }
        [RefersTo]
        public string RelatedInput2 { get; set; }
        [ModelName]
        public string Input1 { get; set; }
        [ModelName]
        public string Input2 { get; set; }
        public int? InmeldingFilterTijd { get; set; }

        public OVIngreepMeldingModel()
        {
        }
    }
}
