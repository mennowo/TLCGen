﻿using System;
using System.Collections.Generic;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class RISFaseCyclusLaneDataModel
    {
        public int RijstrookIndex { get; set; }
        public int LaneID { get; set; }
        public string SystemITF { get; set; }
        public bool UseHeading { get; set; }
        public int Heading { get; set; }
        public int HeadingMarge { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string SignalGroupName { get; set; }

        public List<RISFaseCyclusLaneSimulatedStationModel> SimulatedStations { get; set; }

        public RISFaseCyclusLaneDataModel()
        {
            SimulatedStations = new List<RISFaseCyclusLaneSimulatedStationModel>();   
        }
    }
}
