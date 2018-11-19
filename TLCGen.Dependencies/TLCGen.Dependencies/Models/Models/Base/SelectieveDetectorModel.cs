using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class SelectieveDetectorModel : DetectorModel
    {
        [HasDefault(false)]
        public string Omschrijving { get; set; }
        public SelectieveDetectorTypeEnum SdType { get; set; }
    }
}
