using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseCyclus", TLCGenObjectTypeEnum.Detector, "VanDetector", TLCGenObjectTypeEnum.Detector, "NaarDetector")]
    public class RichtingGevoeligeAanvraagModel
    {
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        [HasDefault(false)]
        public string VanDetector { get; set; }
        [HasDefault(false)]
        public string NaarDetector { get; set; }
        public int MaxTijdsVerschil { get; set; }
        public bool ResetAanvraag { get; set; }
        public int ResetAanvraagTijdsduur { get; set; }
    }
}
