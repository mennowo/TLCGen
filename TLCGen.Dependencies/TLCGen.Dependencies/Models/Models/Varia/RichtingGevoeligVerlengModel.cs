using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseCyclus", TLCGenObjectTypeEnum.Detector, "VanDetector", TLCGenObjectTypeEnum.Detector, "NaarDetector")]
    public class RichtingGevoeligVerlengModel
    {
        #region Properties

        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        [HasDefault(false)]
        public string VanDetector { get; set; }
        [HasDefault(false)]
        public string NaarDetector { get; set; }
        public int MaxTijdsVerschil { get; set; }
        public int VerlengTijd { get; set; }
        public RichtingGevoeligVerlengenTypeEnum TypeVerlengen { get; set; }

        #endregion // Properties
    }
}
