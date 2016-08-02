using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models.Settings
{
    public abstract class SettingModelBase
    {
        protected string _Decription;
        protected bool _IsSet;

        public string Description
        {
            get { return _Decription; }
        }

        public SettingModelBase(string _description)
        {
            _Decription = _description;
            _IsSet = false;
        }
    }
}
