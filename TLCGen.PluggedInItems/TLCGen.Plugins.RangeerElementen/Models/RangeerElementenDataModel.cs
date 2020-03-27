using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Plugins.RangeerElementen.Models
{
    [Serializable]
    [XmlRoot(ElementName = "RangeerTLCGenElementenData")]
    public class RangeerElementenDataModel
    {
        public bool RangeerElementenToepassen { get; set; }
        public bool RangeerSignaalGroepenToepassen { get; set; }

        [XmlArray(ElementName = "RangeerElementenData")]
        public List<RangeerElementModel> RangeerElementen { get; set; }

        [XmlArray(ElementName = "RangeerSignaalGroepData")]
        public List<RangeerSignaalGroepModel> RangeerSignaalGroepen { get; set; }

        public RangeerElementenDataModel()
        {
            RangeerElementen = new List<RangeerElementModel>();
            RangeerSignaalGroepen = new List<RangeerSignaalGroepModel>();
        }
    }
}
