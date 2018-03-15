using System;

namespace TLCGen.Models.Models.OV_HD
{
    [Serializable]
    [RefersTo("Detector")]
    public class ConditionDetector
    {
        public string Detector { get; set; }
    }
}
