using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.ViewModels;

namespace TLCGen.Generators.CCOL.Settings
{
    [Serializable]
    public class CCOLGeneratorFolderSettingModel : ViewModelBase
    {
        private string _Setting;

        public string Default { get; set; }
        public string Setting
        {
            get { return _Setting; }
            set
            {
                if(value != null && value.EndsWith(";"))
                    _Setting = value;
                else if (value != null)
                    _Setting = value + ";";
                OnPropertyChanged("Setting");
            }
        }
        public string Description { get; set; }

        public CCOLGeneratorFolderSettingModel()
        {

        }
    }
}
