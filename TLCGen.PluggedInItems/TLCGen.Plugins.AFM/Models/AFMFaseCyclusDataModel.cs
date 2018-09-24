using System;
using TLCGen.Models;

namespace TLCGen.Plugins.AFM.Models
{
    [Serializable]
    public class AFMFaseCyclusDataModel
    {
        [RefersTo]
        public string FaseCyclus { get; set; }
        [RefersTo]
        public string DummyFaseCyclus { get; set; }
        public int MinimaleGroentijd { get; set; }
        public int MaximaleGroentijd { get; set; }
    }
}
