using System.ComponentModel;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Plugins.RIS.Models
{
    public class RISFaseCyclusLaneSimulatedStationModel
    {
        public RISStationTypeEnum Type { get; set; }
        public bool Prioriteit { get; set; }
        public int Flow { get; set; }
        public int Snelheid { get; set; }
        public int Afstand { get; set; }
        [RefersTo]
        public string SignalGroupName { get; set; }
        public int RijstrookIndex { get; set; }
        public int LaneID { get; set; }

        [IOElement("ris", BitmappedItemTypeEnum.Ingang, "Naam")]
        public BitmapCoordinatenDataModel StationBitmapData { get; set; }

        [Browsable(false)]
        public string Naam
        {
            get
            {
                switch (Type)
                {
                    case RISStationTypeEnum.UNKNOWN:
                        return "ris" + SignalGroupName + RijstrookIndex + "unknown";
                    case RISStationTypeEnum.PEDESTRIAN:
                        return "ris" + SignalGroupName + RijstrookIndex + "pedestrian";
                    case RISStationTypeEnum.CYCLIST:
                        return "ris" + SignalGroupName + RijstrookIndex + "cyclist";
                    case RISStationTypeEnum.MOPED:
                        return "ris" + SignalGroupName + RijstrookIndex + "modep";
                    case RISStationTypeEnum.MOTORCYCLE:
                        return "ris" + SignalGroupName + RijstrookIndex + "motorcycle";
                    case RISStationTypeEnum.PASSENGERCAR:
                        return "ris" + SignalGroupName + RijstrookIndex + "car";
                    case RISStationTypeEnum.BUS:
                        return "ris" + SignalGroupName + RijstrookIndex + "bus";
                    case RISStationTypeEnum.LIGHTTRUCK:
                        return "ris" + SignalGroupName + RijstrookIndex + "lighttruck";
                    case RISStationTypeEnum.HEAVYTRUCK:
                        return "ris" + SignalGroupName + RijstrookIndex + "heavytruck";
                    case RISStationTypeEnum.TRAILER:
                        return "ris" + SignalGroupName + RijstrookIndex + "trailer";
                    case RISStationTypeEnum.SPECIALVEHICLES:
                        return "ris" + SignalGroupName + RijstrookIndex + "special";
                    case RISStationTypeEnum.TRAM:
                        return "ris" + SignalGroupName + RijstrookIndex + "tram";
                    case RISStationTypeEnum.ROADSIDEUNIT:
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
        }
    }
}
