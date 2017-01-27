using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Generators.CCOL.Settings
{
    [Serializable]
    public class CCOLGeneratorCodeStringSettingModel
    {
        public string Default { get; set; }
        private string _Setting;
        public string Setting
        {
            get { return _Setting; }
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                {
                    _Setting = value;
                }
                else
                {
                    _Setting = Default;
                }
            }
        }
        public string Description { get; set; }

        public CCOLGeneratorCodeStringSettingModel()
        {

        }
    }
}
