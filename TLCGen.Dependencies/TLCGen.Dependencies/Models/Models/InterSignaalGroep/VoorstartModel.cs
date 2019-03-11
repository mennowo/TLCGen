using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseVan", TLCGenObjectTypeEnum.Fase, "FaseNaar")]
    public class VoorstartModel : IInterSignaalGroepElement
    {
        #region Properties

        [HasDefault(false)]
        public string FaseVan { get; set; }
        [HasDefault(false)]
        public string FaseNaar { get; set; }
        public int VoorstartTijd { get; set; }
        public int VoorstartOntruimingstijd { get; set; }

        #endregion // Properties
    }
}
