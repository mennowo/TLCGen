using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class KruispuntLayout
    {
        [XmlArrayItem(ElementName = "KruispuntArmen")]
        public List<KruispuntArmModel> KruispuntArmen { get; set; }

        [XmlArrayItem(ElementName = "FasenMetKruispuntArmen")]
        public List<KruispuntArmFaseCyclusModel> FasenMetKruispuntArmen { get; set; }

        public KruispuntLayout()
        {
            KruispuntArmen = new List<KruispuntArmModel>();
            FasenMetKruispuntArmen = new List<KruispuntArmFaseCyclusModel>();
        }
    }
}
