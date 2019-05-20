using System;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class SelectieveDetectorModel : DetectorModel
    {
        [ModelName(TLCGenObjectTypeEnum.SelectieveDetector)]
        [Browsable(false)]
        public override string Naam { get; set; }

        [HasDefault(false)]
        public string Omschrijving { get; set; }
        public SelectieveDetectorTypeEnum SdType { get; set; }
    }
}
