using System;

namespace TLCGen.Plugins.AFM.Models
{
    [Serializable]
    public class AFMFaseCyclusDataModel
    {
        public string FaseCyclus { get; set; }
        public string DummyFaseCyclus { get; set; }
        public int MinimaleGroentijd { get; set; }
        public int MaximaleGroentijd { get; set; }
    }
}
