using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Detector, "MeeaanvraagDetector")]
    public class MeeaanvraagDetectorModel
    {
        [HasDefault(false)]
        public string MeeaanvraagDetector { get; set; }
    }
}
