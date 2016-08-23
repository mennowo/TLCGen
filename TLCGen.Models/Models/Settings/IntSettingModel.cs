using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models.Settings
{
    public class IntSettingModel : SettingModelBase
    {
        private int _Default;
        private int _Setting;

        public int Default { get { return _Default; } }

        public int Setting
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

        public IntSettingModel(int _default, string _description) : base(_description)
        {
            _Default = _default;
        }
    }
}
