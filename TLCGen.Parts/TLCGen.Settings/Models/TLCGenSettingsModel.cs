using System;
using System.ComponentModel;
using TLCGen.Models;

namespace TLCGen.Settings
{
    [Serializable]
    public class TLCGenSettingsModel
    {
        public string DefaultsFileLocation { get; set; }
        public string TemplatesFolderLocation { get; set; }

        [Browsable(false)]
        public CustomDataModel CustomData { get; set; }

        public TLCGenSettingsModel()
        {
            CustomData = new CustomDataModel();
        }
    }
}
