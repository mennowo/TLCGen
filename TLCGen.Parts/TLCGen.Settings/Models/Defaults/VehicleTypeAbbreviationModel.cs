using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    [Serializable]
    public class VehicleTypeAbbreviationModel
    {
        public PrioIngreepVoertuigTypeEnum VehicleType { get; set; }
        public string Default { get; set; }
        public string Setting { get; set; }
    }
}
