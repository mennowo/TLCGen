using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models.Settings
{
    public class TLCGenSettingsModel
    {
        [Browsable(false)]
        public CustomDataModel CustomData { get; set; }

        public TLCGenDefaultControllerSettings DefaultControllerSettings { get; set; }
        public TLCGenDefaultFaseCyclusSettings DefaultFaseCyclusSettings { get; set; }

        public TLCGenSettingsModel()
        {
            CustomData = new CustomDataModel();

            DefaultControllerSettings = new TLCGenDefaultControllerSettings();
            DefaultFaseCyclusSettings = new TLCGenDefaultFaseCyclusSettings();
        }
    }
}
