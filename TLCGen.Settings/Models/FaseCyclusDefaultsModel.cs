using System;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    [Serializable]
    public class FaseCyclusDefaultsModel
    {
        public FaseTypeEnum Type { get; set; }
        public FaseCyclusModel FaseCyclus { get; set; }
        public FaseCyclusModuleDataModel FaseCyclusModuleData { get; set; }
        public GroentijdModel Groentijd { get; set; }
        public RoBuGroverFaseCyclusInstellingenModel RoBuGroverFaseCyclusInstellingen { get; set; }

        public object GetModel(string type)
        {
            switch(type)
            {
                case "FaseCyclusModel": return FaseCyclus;
                case "FaseCyclusModuleDataModel": return FaseCyclusModuleData;
                case "GroentijdModel": return Groentijd;
                case "RoBuGroverFaseCyclusInstellingenModel": return RoBuGroverFaseCyclusInstellingen;
            }
            return null;
        }
    }
}