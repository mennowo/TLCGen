using System.Collections.Generic;
using System.Linq;
using TLCGen.Plugins;

namespace TLCGen.Models
{
    public class IOCollector
    {
        public List<IOElementModel> Outputs { get; } = new List<IOElementModel>();
        public List<IOElementModel> Inputs { get; } = new List<IOElementModel>();
        public List<IOElementModel> Detectors { get; } = new List<IOElementModel>();
        public List<IOElementModel> SignalGroups { get; } = new List<IOElementModel>();

        public void CollectItems(ControllerModel c, List<ITLCGenElementProvider> parts)
        {
            foreach (var plugin in parts)
            {
                Outputs.AddRange(plugin.GetOutputItems());
                Inputs.AddRange(plugin.GetInputItems());
            }

            foreach (var fc in c.Fasen)
            {
                SignalGroups.Add(fc);
                foreach (var d in fc.Detectoren.Where(x => !x.Dummy)) Detectors.Add(d);
            }
            foreach (var d in c.Detectoren.Where(x => !x.Dummy)) Detectors.Add(d);
            foreach (var d in c.SelectieveDetectoren.Where(x => !x.Dummy)) Detectors.Add(d);
        }
    }
}
