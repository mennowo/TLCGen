using System;
using TLCGen.Models;

namespace TLCGen.Plugins.MultiSim
{
    [Serializable]
    public class MultiSimEntryModel
    {
        public string DetectorName { get; set; }
        public DetectorSimulatieModel SimulationModel { get; set; }

        public MultiSimEntryModel()
        {
            SimulationModel = new DetectorSimulatieModel();
        }
    }
}
