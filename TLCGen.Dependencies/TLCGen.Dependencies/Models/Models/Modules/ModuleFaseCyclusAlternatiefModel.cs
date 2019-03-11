using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseCyclus")]
    public class ModuleFaseCyclusAlternatiefModel
    {
        #region Properties

        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public int AlternatieveGroenTijd { get; set; }

        #endregion // Properties
    }
}
