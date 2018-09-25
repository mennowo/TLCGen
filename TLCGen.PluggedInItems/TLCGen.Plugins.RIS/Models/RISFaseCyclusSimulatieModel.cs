using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Plugins.RIS.Models
{
    [Serializable]
    public class RISFaseCyclusSimulatieModel
    {
        public List<RISFaseCyclusLaneSimulatieModel> LaneData { get; set; }

        public RISFaseCyclusSimulatieModel()
        {
            LaneData = new List<RISFaseCyclusLaneSimulatieModel>();
        }
    }
}
