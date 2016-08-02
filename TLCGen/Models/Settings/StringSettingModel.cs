using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models.Settings
{
    public class StringSettingModel : SettingModelBase
    {
        private string _Default;
        private string _Setting;

        public string Default { get { return _Default; } }
        public string Setting
        {
            get
            {
                if (_IsSet) return _Setting;
                else return Default;
            }
            set
            {
                _Setting = value;
                _IsSet = true;
            }
        }

        public StringSettingModel(string _default, string _description) : base(_description)
        {
            _Default = _default;
        }
    }
}
