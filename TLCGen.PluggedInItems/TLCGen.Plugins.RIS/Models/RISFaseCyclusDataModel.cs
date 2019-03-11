using System;
using System.Collections.Generic;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Plugins.RIS.Models
{
    [Serializable]
    public class RISFaseCyclusDataModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FaseCyclus { get; set; }

        public List<RISFaseCyclusLaneDataModel> LaneData { get; set; }

        public RISFaseCyclusDataModel()
        {
            LaneData = new List<RISFaseCyclusLaneDataModel>();
        }
    }
}
