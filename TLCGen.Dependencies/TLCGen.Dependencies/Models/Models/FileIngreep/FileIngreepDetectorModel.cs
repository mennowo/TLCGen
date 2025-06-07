using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class FileIngreepDetectorModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        [HasDefault(false)]
        public string Detector { get; set; }

        public int BezetTijd { get; set; }
        public int RijTijd { get; set; }
        public int AfvalVertraging { get; set; }
        public bool IngreepNaamPerLus { get; set; }
    }
}
