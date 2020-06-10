using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class LateReleaseModel : IInterSignaalGroepElement
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseVan { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseNaar { get; set; }
        public int LateReleaseTijd { get; set; }
        public int LateReleaseOntruimingstijd { get; set; }

        #endregion // Properties
    }
}
