using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Detector, "Detector")]
    public class RatelTikkerDetectorModel
    {
        [HasDefault(false)]
        public string Detector { get; set; }
    }
}
