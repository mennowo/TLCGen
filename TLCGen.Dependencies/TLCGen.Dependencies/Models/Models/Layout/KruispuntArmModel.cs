using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class KruispuntArmModel
    {
        [ModelName(TLCGenObjectTypeEnum.KruispuntArm)]
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
    }
}