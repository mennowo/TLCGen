using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseCyclus")]
    public class FileIngreepTeDoserenSignaalGroepModel
    {
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public int DoseerPercentage { get; set; }
    }
}
