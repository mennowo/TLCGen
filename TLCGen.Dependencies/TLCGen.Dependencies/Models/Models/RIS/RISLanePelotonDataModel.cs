using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class RISLanePelotonDataModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string SignalGroupName { get; set; }
        public int RijstrookIndex { get; set; }
        public bool RISPelotonBepaling { get; set; }
        public int PelotonBepalingStart { get; set; }
        public int PelotonBepalingEnd { get; set; }
        public RISStationTypeEnum Type { get; set; }

        public RISLanePelotonDataModel()
        {
            RISPelotonBepaling = true;
        }
    }
}