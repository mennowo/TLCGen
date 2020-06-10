using System;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class RoBuGroverHiaatDetectorModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        [Browsable(false)]
        [HasDefault(false)]
        public string Detector { get; set; }
        public int HiaatTijd { get; set; }
    }
}