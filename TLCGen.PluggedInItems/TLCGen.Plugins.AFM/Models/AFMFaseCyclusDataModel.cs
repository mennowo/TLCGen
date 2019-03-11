using System;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Plugins.AFM.Models
{
    [Serializable]
    public class AFMFaseCyclusDataModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FaseCyclus { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string DummyFaseCyclus { get; set; }
        public int MinimaleGroentijd { get; set; }
        public int MaximaleGroentijd { get; set; }
    }
}
