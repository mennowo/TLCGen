using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TLCGen.Models.Settings
{
    [Editor()]
    public class TLCGenSettingsModel
    {
        [Browsable(false)]
        public CustomDataModel CustomData { get; set; }
        
        public TLCGenSettingsModel()
        {
            CustomData = new CustomDataModel();
        }
    }
}
