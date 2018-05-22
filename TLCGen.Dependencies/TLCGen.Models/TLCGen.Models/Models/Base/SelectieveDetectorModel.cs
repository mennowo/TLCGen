using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class SelectieveDetectorModel
    {
        [ModelName(TLCGenObjectTypeEnum.SelectieveDetector)]
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        public SelectieveDetectorTypeEnum Type { get; set; }
    }
}
