using System;
using System.Collections.Generic;

namespace TLCGen.Plugins.MultiSim
{
    [Serializable]
    public class MultiSimEntrySetModel
    {
        public string Description { get; set; }

        public List<MultiSimEntryModel> SimulationEntries { get; set; }

        public MultiSimEntrySetModel()
        {
            SimulationEntries = new List<MultiSimEntryModel>();
        }
    }
}
