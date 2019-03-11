using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Detector, "Detector")]
    public class NaloopDetectorModel
    {
        [HasDefault(false)]
        public string Detector { get; set; }
        public NaloopDetectorTypeEnum Type { get; set; }
    }
}
