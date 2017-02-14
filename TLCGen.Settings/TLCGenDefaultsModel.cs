using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    [Serializable]
    public class TLCGenDefaultsModel
    {
        [XmlArrayItem(ElementName = "FaseCyclus")]
        public List<FaseCyclusDefaultsModel> Fasen { get; set; }

        [XmlArrayItem(ElementName = "Detector")]
        public List<DetectorDefaultsModel> Detectoren { get; set; }
    }

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

    [Serializable]
    public class DetectorDefaultsModel
    {
        public DetectorTypeEnum Type { get; set; }
        public DetectorModel Detector { get; set; }
        public RoBuGroverFileDetectorModel RoBuGroverFileDetector { get; set; }
        public RoBuGroverHiaatDetectorModel RoBuGroverHiaatDetector { get; set; }

        public object GetModel(string type)
        {
            switch (type)
            {
                case "DetectorModel": return Detector;
                case "RoBuGroverFileDetectorModel": return RoBuGroverFileDetector;
                case "RoBuGroverHiaatDetectorModel": return RoBuGroverHiaatDetector;
            }
            return null;
        }
    }
}
