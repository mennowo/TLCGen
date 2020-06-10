using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class NaloopDetectorModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        [HasDefault(false)]
        public string Detector { get; set; }
        public NaloopDetectorTypeEnum Type { get; set; }
    }
}
