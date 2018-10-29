using System;
using System.Collections.Generic;
using TLCGen.Models;

namespace TLCGen.Plugins.RIS.Models
{
    [Serializable]
    public class RISLaneExtendDataModel
    {
        [RefersTo]
        public string SignalGroupName { get; set; }
        public int RijstrookIndex { get; set; }
        public bool RISVerlengen { get; set; }
        public int VerlengenStart { get; set; }
        public int VerlengenEnd { get; set; }
        public RISStationTypeEnum Type { get; set; }

        public RISLaneExtendDataModel()
        {
            RISVerlengen = true;
        }
    }
}
