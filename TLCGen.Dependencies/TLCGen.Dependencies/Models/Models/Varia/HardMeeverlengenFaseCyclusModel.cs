using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class HardMeeverlengenFaseCyclusModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public HardMeevelengenTypeEnum Type { get; set; }
    }
}
