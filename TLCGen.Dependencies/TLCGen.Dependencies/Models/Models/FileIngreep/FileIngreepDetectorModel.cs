using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Detector, "Detector")]
    public class FileIngreepDetectorModel
    {
        [HasDefault(false)]
        public string Detector { get; set; }

        public int BezetTijd { get; set; }
        public int RijTijd { get; set; }
        public int AfvalVertraging { get; set; }
    }
}
