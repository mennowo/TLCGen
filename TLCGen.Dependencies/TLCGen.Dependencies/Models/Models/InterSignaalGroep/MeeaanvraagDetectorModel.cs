using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class MeeaanvraagDetectorModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        [HasDefault(false)]
        public string MeeaanvraagDetector { get; set; }
    }
}
