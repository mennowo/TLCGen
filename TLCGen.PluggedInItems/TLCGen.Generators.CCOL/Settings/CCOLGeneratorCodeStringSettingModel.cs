﻿using System;

namespace TLCGen.Generators.CCOL.Settings
{
    [Serializable]
    public class CCOLGeneratorCodeStringSettingModel
    {
        public string Default { get; set; }

        private string _Setting;
        public string Setting
        {
            get => _Setting;
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
        public CCOLGeneratorSettingTypeEnum Type { get; set; }
        public string Description { get; set; }

        public CCOLGeneratorCodeStringSettingModel()
        {

        }

        public override string ToString()
        {
            return Setting;
        }
    }
}
