using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
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

    [Serializable]
    public class KruispuntArmModel
    {
        [ModelName(TLCGenObjectTypeEnum.KruispuntArm)]
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
    }

    [Serializable]
    public class KruispuntArmFaseCyclusModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.KruispuntArm)]
        [HasDefault(false)]
        public string KruispuntArm { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.KruispuntArm)]
        [HasDefault(false)]
        public string KruispuntArmVolg { get; set; }
        
        public bool HasKruispuntArmVolgTijd { get; set; }
        public int KruispuntArmVolgTijd { get; set; }
    }
}
