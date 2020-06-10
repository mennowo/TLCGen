using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class ModuleFaseCyclusAlternatiefModel
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public int AlternatieveGroenTijd { get; set; }

        #endregion // Properties
    }
}
