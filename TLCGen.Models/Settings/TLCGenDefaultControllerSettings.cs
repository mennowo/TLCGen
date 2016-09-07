using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TLCGen.Models.Settings
{
    public class TLCGenDefaultControllerSettings
    {
        [ExpandableObject]
        [Description("Default voorvoegsels voor code #defines")]
        public TLCGenDefaultPrefixSettings PreFixSettings { get; set; }

        public TLCGenDefaultControllerSettings()
        {
            PreFixSettings = new TLCGenDefaultPrefixSettings();
        }
    }
}
