using System;
using System.Collections.Generic;
using TLCGen.Models;

namespace TLCGen.Plugins.RIS.Models
{
    [Serializable]
    public class RISLaneRequestDataModel
    {
        [RefersTo]
        public string SignalGroupName { get; set; }
        public int RijstrookIndex { get; set; }
        public bool RISAanvraag { get; set; }
        public int AanvraagStart { get; set; }
        public int AanvraagEnd { get; set; }
        public RISStationTypeEnum Type { get; set; }

        public RISLaneRequestDataModel()
        {
            RISAanvraag = true;
        }
    }
}
