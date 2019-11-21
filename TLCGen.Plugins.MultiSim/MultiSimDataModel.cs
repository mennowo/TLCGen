using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Plugins.MultiSim
{
    [Serializable]
    [XmlRoot("MultiSimData")]
    public class MultiSimDataModel
    {
        public List<MultiSimEntrySetModel> SimulationEntries { get; set; }

        public MultiSimDataModel()
        {
            SimulationEntries = new List<MultiSimEntrySetModel>();
        }
    }
}
