using System;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class RISLaneExtendDataModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string SignalGroupName { get; set; }
        public int RijstrookIndex { get; set; }
        public bool RISVerlengen { get; set; }
        public int VerlengenStart { get; set; }
        public int VerlengenEnd { get; set; }
        public int VerlengenStartSrm0 { get; set; }
        public int VerlengenEndSrm0 { get; set; }
        public RISStationTypeEnum Type { get; set; }

        public RISLaneExtendDataModel()
        {
            RISVerlengen = true;
        }
    }
}
