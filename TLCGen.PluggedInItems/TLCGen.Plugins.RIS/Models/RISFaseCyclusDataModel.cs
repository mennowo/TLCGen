using System;
using TLCGen.Models;

namespace TLCGen.Plugins.RIS.Models
{
    [Serializable]
    public class RISFaseCyclusDataModel
    {
        [RefersTo]
        public string FaseCyclus { get; set; }
        public bool RISAanvraag { get; set; }
        public int AanvraagStart { get; set; }
        public int AanvraagEnd { get; set; }
        public bool RISVerlengen { get; set; }
        public int VerlengenStart { get; set; }
        public int VerlengenEnd { get; set; }
        public RISFaseCyclusSimulatieModel SimulatieData { get; set; }

        public RISFaseCyclusDataModel()
        {
            SimulatieData = new RISFaseCyclusSimulatieModel();
        }
    }
}
