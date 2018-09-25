using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Plugins.RIS.Models
{
    public class RISFaseCyclusLaneSimulatieModel
    {
        public int LaneID { get; set; }

        [XmlArray(ElementName = "StationType")]
        public List<RISFaseCyclusLaneStationTypeSimulatieModel> StationTypes { get; set; }

        public RISFaseCyclusLaneSimulatieModel()
        {
            StationTypes = new List<RISFaseCyclusLaneStationTypeSimulatieModel>();
        }
    }
}
