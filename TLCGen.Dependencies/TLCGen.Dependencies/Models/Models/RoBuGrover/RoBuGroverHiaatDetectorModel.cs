using System;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Detector, "Detector")]
    public class RoBuGroverHiaatDetectorModel
    {
        [Browsable(false)]
        [HasDefault(false)]
        public string Detector { get; set; }
        public int HiaatTijd { get; set; }
    }
}