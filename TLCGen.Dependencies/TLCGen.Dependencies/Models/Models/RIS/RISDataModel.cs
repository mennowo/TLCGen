using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    [XmlRoot(ElementName = "RISData")]
    public class RISDataModel
    {
        public bool RISToepassen { get; set; }
        public bool HasMultipleSystemITF { get; set; }
        public bool NietCheckenOpSignaalgroep { get; set; }
        public string SystemITF { get; set; }
        public List<RISSystemITFModel> MultiSystemITF { get; set; }

        [XmlArray(ElementName = "RISFaseCyclusData")]
        public List<RISFaseCyclusDataModel> RISFasen { get; set; }

        [XmlArray(ElementName = "RISLaneRequestData")]
        public List<RISLaneRequestDataModel> RISRequestLanes { get; set; }

        [XmlArray(ElementName = "RISLaneExtendData")]
        public List<RISLaneExtendDataModel> RISExtendLanes { get; set; }

        public RISDataModel()
        {
            RISFasen = new List<RISFaseCyclusDataModel>();
            RISRequestLanes = new List<RISLaneRequestDataModel>();
            RISExtendLanes = new List<RISLaneExtendDataModel>();
            MultiSystemITF = new List<RISSystemITFModel>();
        }
    }
}
