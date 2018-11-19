using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo("FaseCyclus")]
    public class HardMeeverlengenFaseCyclusModel
    {
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public HardMeevelengenTypeEnum Type { get; set; }
    }
}
