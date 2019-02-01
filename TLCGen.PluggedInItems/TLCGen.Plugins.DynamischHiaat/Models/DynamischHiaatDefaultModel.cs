using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Plugins.DynamischHiaat.Models
{
    [Serializable]
    [XmlRoot(ElementName = "DynamischHiaatDefaults")]
    public class DynamischHiaatDefaultsModel
    {
        [XmlArrayItem(ElementName = "DynamischHiaatDefault")]
        public List<DynamischHiaatDefaultModel> Defaults { get; set; }

        public DynamischHiaatDefaultsModel()
        {
            Defaults = new List<DynamischHiaatDefaultModel>();
        }
    }

    [Serializable]
    public class DynamischHiaatDefaultModel
    {
        public string Name { get; set; }
        public string DefaultSnelheid { get; set; }

        [XmlArrayItem(ElementName = "Snelheid")]
        public List<DynamischHiaatSpeedDefaultModel> Snelheden { get; set; }

        public DynamischHiaatDefaultModel()
        {
            Snelheden = new List<DynamischHiaatSpeedDefaultModel>();
        }
    }

    [Serializable]
    public class DynamischHiaatSpeedDefaultModel
    {
        public string Name { get; set; }

        [XmlArrayItem(ElementName = "Detector")]
        public List<DynamischHiaatDetectorModel> Detectoren { get; set; }

        public DynamischHiaatSpeedDefaultModel()
        {
            Detectoren = new List<DynamischHiaatDetectorModel>();
        }
    }
}