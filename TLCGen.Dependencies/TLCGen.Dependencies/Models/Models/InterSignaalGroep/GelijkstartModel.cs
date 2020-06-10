using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class GelijkstartModel : IInterSignaalGroepElement
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseVan { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseNaar { get; set; }
        public int GelijkstartOntruimingstijdFaseVan { get; set; }
        public int GelijkstartOntruimingstijdFaseNaar { get; set; }
        public bool DeelConflict { get; set; }
        public Enumerations.AltijdAanUitEnum Schakelbaar { get; set; }

        #endregion // Properties
    }
}
