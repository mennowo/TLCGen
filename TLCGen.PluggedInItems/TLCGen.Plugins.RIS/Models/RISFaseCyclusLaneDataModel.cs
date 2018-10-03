using System;
using System.Collections.Generic;
using TLCGen.Models;

namespace TLCGen.Plugins.RIS.Models
{
    [Serializable]
    public class RISFaseCyclusLaneDataModel
    {
        public int RijstrookIndex { get; set; }
        public int LaneID { get; set; }
        [RefersTo]
        public string SignalGroupName { get; set; }

        public List<RISFaseCyclusLaneSimulatedStationModel> SimulatedStations { get; set; }

        public RISFaseCyclusLaneDataModel()
        {
            SimulatedStations = new List<RISFaseCyclusLaneSimulatedStationModel>();   
        }
    }
}
