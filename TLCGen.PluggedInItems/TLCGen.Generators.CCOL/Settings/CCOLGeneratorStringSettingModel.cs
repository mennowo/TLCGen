using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Generators.CCOL.Settings
{
    [Serializable]
    public class CCOLGeneratorStringSettingModel
    {
        private string _Setting;
        public string Setting
        {
            get { return _Setting; }
            set
            {
                if(!string.IsNullOrEmpty(value))
                {
                    _Setting = value;
                }
                else
                {
                    _Setting = null;
                }
            }
        }
        public string Description { get; set; }

        public CCOLGeneratorStringSettingModel()
        {

        }
    }
}
