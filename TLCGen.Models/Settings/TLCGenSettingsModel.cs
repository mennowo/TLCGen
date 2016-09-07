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

        [ExpandableObject]
        [Description("Default instellingen voor regeling")]
        public TLCGenDefaultControllerSettings DefaultControllerSettings { get; set; }
        [ExpandableObject]
        [Description("Default instellingen voor fasen")]
        public TLCGenDefaultFaseCyclusSettings DefaultFaseCyclusSettings { get; set; }

        public TLCGenSettingsModel()
        {
            CustomData = new CustomDataModel();

            DefaultControllerSettings = new TLCGenDefaultControllerSettings();
            DefaultFaseCyclusSettings = new TLCGenDefaultFaseCyclusSettings();
        }
    }
}
