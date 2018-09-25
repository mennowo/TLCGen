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

        [XmlArray(ElementName = "RISFaseCyclusData")]
        public List<RISFaseCyclusDataModel> RISFasen { get; set; }

        public RISDataModel()
        {
            RISFasen = new List<RISFaseCyclusDataModel>();
        }
    }
}
