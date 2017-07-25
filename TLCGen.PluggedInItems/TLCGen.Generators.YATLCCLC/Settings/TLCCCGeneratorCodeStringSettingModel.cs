using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.TLCCC.CodeGeneration;

namespace TLCGen.Generators.CCOL.Settings
{
    [Serializable]
    public class TLCCCGeneratorCodeStringSettingModel
    {
        public string Default { get; set; }

        private string _Setting;
        public string Setting
        {
            get
            {
                if (!string.IsNullOrEmpty(_Setting))
                {
                    return _Setting;
                }
                return Default;
            }
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
        public TLCCCElementTypeEnum Type { get; set; }
        public string Description { get; set; }

        public TLCCCGeneratorCodeStringSettingModel()
        {

        }
    }
}
