using System;

namespace TLCGen.Models
{
    [Serializable]
    public class IOElementRangeerDataModel
    {
        public string Naam { get; set; }
        public bool HasManualNaam { get; set; }
        public string ManualNaam { get; set; }
        public int RangeerIndex { get; set; }
        public int RangeerIndex2 { get; set; } = -1;
    }
}
