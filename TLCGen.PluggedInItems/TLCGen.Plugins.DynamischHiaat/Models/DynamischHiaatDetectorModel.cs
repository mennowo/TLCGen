using System;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Plugins.DynamischHiaat.Models
{
    [Serializable]
    public class DynamischHiaatDetectorModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string SignalGroupName { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        public string DetectorName { get; set; }

        public int Moment1 { get; set; }
        public int Moment2 { get; set; }
        public int TDH1 { get; set; }
        public int TDH2 { get; set; }
        public int Maxtijd { get; set; }
        public bool SpringStart { get; set; }
        public bool VerlengNiet { get; set; }
        public bool VerlengExtra { get; set; }
        public bool DirectAftellen { get; set; }
        public bool SpringGroen { get; set; }
    }
}
