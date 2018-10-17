using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Plugins.RIS.Models
{

    [Serializable]
    [XmlRoot(ElementName = "RISData")]
    public class RISDataModel
    {
        public bool RISToepassen { get; set; }
        public string SystemITF { get; set; }

        [XmlArray(ElementName = "RISFaseCyclusData")]
        public List<RISFaseCyclusDataModel> RISFasen { get; set; }

        [XmlArray(ElementName = "RISLaneRequestExtendData")]
        public List<RISLaneRequestExtendDataModel> RISRequestExtendLanes { get; set; }

        public RISDataModel()
        {
            RISFasen = new List<RISFaseCyclusDataModel>();
            RISRequestExtendLanes = new List<RISLaneRequestExtendDataModel>();
        }
    }
}
