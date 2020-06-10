using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class StarPeriodeDataModel
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Periode)]
        [HasDefault(false)]
        public string Periode { get; set; }
        [HasDefault(false)]
        [RefersTo(TLCGenObjectTypeEnum.StarProgramma)]
        public string StarProgramma { get; set; }
        
        #endregion // Properties
    }
}