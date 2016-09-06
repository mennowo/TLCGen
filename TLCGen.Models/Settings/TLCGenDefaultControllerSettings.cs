using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models.Settings
{
    public class TLCGenDefaultControllerSettings
    {
        public TLCGenDefaultPrefixSettings PreFixSettings { get; set; }

        public TLCGenDefaultControllerSettings()
        {
            PreFixSettings = new TLCGenDefaultPrefixSettings();
        }
    }
}
