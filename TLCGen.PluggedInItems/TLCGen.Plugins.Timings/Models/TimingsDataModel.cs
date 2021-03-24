using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Plugins.Timings.Models
{

    [Serializable]
    [XmlRoot(ElementName = "TimingsData")]
    public class TimingsDataModel
    {
        public bool TimingsToepassen { get; set; }
        
        public bool TimingsUsePredictions { get; set; }

        [XmlArray(ElementName = "TimingsFaseCyclusData")]
        public List<TimingsFaseCyclusDataModel> TimingsFasen { get; set; }

        public TimingsDataModel()
        {
            TimingsFasen = new List<TimingsFaseCyclusDataModel>();
        }
    }
}
