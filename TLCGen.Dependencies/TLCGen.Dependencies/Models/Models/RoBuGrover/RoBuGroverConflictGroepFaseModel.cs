using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseCyclus")]
    public class RoBuGroverConflictGroepFaseModel
    {
        #region Properties

        [HasDefault(false)]
        public string FaseCyclus { get; set; }

        #endregion // Properties
    }
}