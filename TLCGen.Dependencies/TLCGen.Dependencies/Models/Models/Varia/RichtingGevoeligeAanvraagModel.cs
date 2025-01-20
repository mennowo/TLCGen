using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class RichtingGevoeligeAanvraagModel
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        [HasDefault(false)]
        public string VanDetector { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        [HasDefault(false)]
        public string NaarDetector { get; set; }
        public int MaxTijdsVerschil { get; set; }
        public bool ResetAanvraag { get; set; }
        public AltijdAanUitEnum AltijdAanUit { get; set; }
        public int ResetAanvraagTijdsduur { get; set; }

        #endregion // Properties

        #region Constructor

        public RichtingGevoeligeAanvraagModel()
        {
            AltijdAanUit = AltijdAanUitEnum.SchAan;
        }

        #endregion // Constructor
    }
}
