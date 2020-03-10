using System;
using GalaSoft.MvvmLight;

namespace TLCGen.Generators.CCOL.Settings
{
    [Serializable]
    public class CCOLGeneratorFolderSettingModel : ViewModelBase
    {
        private string _Setting;

        public string Default { get; set; }
        public string Setting
        {
            get => _Setting;
            set
            {
                if(value != null && value.EndsWith(";"))
                    _Setting = value;
                else if (value != null)
                    _Setting = value + ";";
                RaisePropertyChanged("Setting");
            }
        }
        public string Description { get; set; }

        public CCOLGeneratorFolderSettingModel()
        {

        }
    }
}
