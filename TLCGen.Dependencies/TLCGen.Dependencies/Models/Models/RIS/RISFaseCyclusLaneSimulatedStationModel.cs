using System;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class RISFaseCyclusLaneSimulatedStationModel
    {
        public RISStationTypeSimEnum Type { get; set; }
        public RISVehicleRole VehicleRole { get; set; }
        public RISVehicleSubrole VehicleSubrole { get; set; }
        public bool Prioriteit { get; set; }
        public int Flow { get; set; }
        public int Snelheid { get; set; }
        public int Afstand { get; set; }
        public DetectorSimulatieModel SimulationData { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string SignalGroupName { get; set; }
        public int RijstrookIndex { get; set; }
        public int LaneID { get; set; }
        public int ApproachID { get; set; }
        public string SystemITF { get; set; }
        public int Importance { get; set; }

        [IOElement("ris", BitmappedItemTypeEnum.Ingang, "Naam")]
        public BitmapCoordinatenDataModel StationBitmapData { get; set; }

        [Browsable(false)]
        public string Naam
        {
            get
            {
                switch (Type)
                {
                    case RISStationTypeSimEnum.UNKNOWN:
                        return "ris" + SignalGroupName + RijstrookIndex + "unknown";
                    case RISStationTypeSimEnum.PEDESTRIAN:
                        return "ris" + SignalGroupName + RijstrookIndex + "pedestrian";
                    case RISStationTypeSimEnum.CYCLIST:
                        return "ris" + SignalGroupName + RijstrookIndex + "cyclist";
                    case RISStationTypeSimEnum.MOPED:
                        return "ris" + SignalGroupName + RijstrookIndex + "modep";
                    case RISStationTypeSimEnum.MOTORCYCLE:
                        return "ris" + SignalGroupName + RijstrookIndex + "motorcycle";
                    case RISStationTypeSimEnum.PASSENGERCAR:
                        return "ris" + SignalGroupName + RijstrookIndex + "car";
                    case RISStationTypeSimEnum.BUS:
                        return "ris" + SignalGroupName + RijstrookIndex + "bus";
                    case RISStationTypeSimEnum.LIGHTTRUCK:
                        return "ris" + SignalGroupName + RijstrookIndex + "lighttruck";
                    case RISStationTypeSimEnum.HEAVYTRUCK:
                        return "ris" + SignalGroupName + RijstrookIndex + "heavytruck";
                    case RISStationTypeSimEnum.TRAILER:
                        return "ris" + SignalGroupName + RijstrookIndex + "trailer";
                    case RISStationTypeSimEnum.SPECIALVEHICLES:
                        return "ris" + SignalGroupName + RijstrookIndex + "special";
                    case RISStationTypeSimEnum.TRAM:
                        return "ris" + SignalGroupName + RijstrookIndex + "tram";
                    case RISStationTypeSimEnum.ROADSIDEUNIT:
                        return "ris" + SignalGroupName + RijstrookIndex + "roadsideunit";
                }
                return "";
            }
            set
            {

            }
        }

        public RISFaseCyclusLaneSimulatedStationModel()
        {
            StationBitmapData = new BitmapCoordinatenDataModel();
            SimulationData = new DetectorSimulatieModel();
        }
    }
}
