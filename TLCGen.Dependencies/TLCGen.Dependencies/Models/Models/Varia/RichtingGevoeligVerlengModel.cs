using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class RichtingGevoeligVerlengModel
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
        public int VerlengTijd { get; set; }
        public RichtingGevoeligVerlengenTypeEnum TypeVerlengen { get; set; }
        public AltijdAanUitEnum AltijdAanUit { get; set; } = AltijdAanUitEnum.SchAan;

        #endregion // Properties

        #region Constructor
        
        public RichtingGevoeligVerlengModel()
        {
            AltijdAanUit = AltijdAanUitEnum.SchAan;
        }

        #endregion // Constructor
    }
}
