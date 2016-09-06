using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models.Settings
{
    public class TLCGenDefaultPrefixSettings
    {
        public string FaseCyclusDefinePrefix { get; set; }
        public string DetectorDefinePrefix { get; set; }

        public TLCGenDefaultPrefixSettings()
        {
            FaseCyclusDefinePrefix = "fc";
            DetectorDefinePrefix = "d";
        }
    }
}
