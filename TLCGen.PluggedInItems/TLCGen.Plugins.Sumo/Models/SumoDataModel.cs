using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Plugins.Sumo
{
    [Serializable]
    [XmlRoot(ElementName = "SumoKoppeling")]
    public class SumoDataModel
    {
        public bool GenererenSumoCode { get; set; }
        public int SumoPort { get; set; }
        public int SumoOrder { get; set; }
        public int StartTijdUur { get; set; }
        public int StartTijdMinuut { get; set; }
        public string SumoKruispuntNaam { get; set; }
        public int SumoKruispuntLinkMax { get; set; }
        public bool AutoStartSumo { get; set; }
        public string SumoHomePath { get; set; }
        public string SumoConfigPath { get; set; }

        [XmlArrayItem("FaseCyclus")]
        public List<FaseCyclusSumoDataModel> FaseCycli { get; set; }
        [XmlArrayItem("Detector")]
        public List<DetectorSumoDataModel> Detectoren { get; set; }

        public SumoDataModel()
        {
            FaseCycli = new List<FaseCyclusSumoDataModel>();
            Detectoren = new List<DetectorSumoDataModel>();
            SumoKruispuntNaam = "";
            SumoHomePath = "";
            SumoConfigPath = "";

            SumoPort = 4001;
            SumoOrder = 0;
            StartTijdUur = 9;
            StartTijdMinuut = 0;
        }
    }
}
