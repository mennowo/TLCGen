using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
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