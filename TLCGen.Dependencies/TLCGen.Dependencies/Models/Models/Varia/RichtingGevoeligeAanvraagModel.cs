using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class RichtingGevoeligeAanvraagModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string VanDetector { get; set; }
        [HasDefault(false)]
        public string NaarDetector { get; set; }
        public int MaxTijdsVerschil { get; set; }
        public bool ResetAanvraag { get; set; }
        public AltijdAanUitEnum AltijdAanUit { get; set; }
        public int ResetAanvraagTijdsduur { get; set; }

        public RichtingGevoeligeAanvraagModel()
        {
            AltijdAanUit = AltijdAanUitEnum.SchAan;
        }
    }
}
