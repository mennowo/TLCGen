using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.CustomPropertyEditors;
using TLCGen.Plugins;

namespace TLCGen.Generators.CCOL.Settings
{
    public class CCOLGeneratorVisualSettingsModel
    {
        private string _CCOLIncludesPaden;
        [DisplayName("CCOL include paden")]
        [Description("CCOL include paden")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        public string CCOLIncludesPaden
        {
            get { return _CCOLIncludesPaden; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLIncludesPaden = value + ";";
                else
                    _CCOLIncludesPaden = value;
            }
        }

        private string _CCOLLibsPath;
        [DisplayName("CCOL library pad")]
        [Description("CCOL library pad")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        [Editor(typeof(FolderEditor), typeof(FolderEditor))]
        public string CCOLLibsPath
        {
            get { return _CCOLLibsPath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLLibsPath = value + ";";
                else
                    _CCOLLibsPath = value;
            }
        }

        private string _CCOLLibs;
        [DisplayName("CCOL libraries")]
        [Description("CCOL libraries (indien van toepassing)")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        public string CCOLLibs
        {
            get { return _CCOLLibs; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLLibs = value + ";";
                else
                    _CCOLLibs = value;
            }
        }

        private string _CCOLResPath;
        [DisplayName("CCOL resources pad")]
        [Description("CCOL resources pad")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        [Editor(typeof(FolderEditor), typeof(FolderEditor))]
        public string CCOLResPath
        {
            get { return _CCOLResPath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLResPath = value + ";";
                else
                    _CCOLResPath = value;
            }
        }

        private string _CCOLPreprocessorDefinitions;
        [DisplayName("Preprocessor definities")]
        [Description("Preprocessor definities")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        public string CCOLPreprocessorDefinitions
        {
            get { return _CCOLPreprocessorDefinitions; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLPreprocessorDefinitions = value + ";";
                else
                    _CCOLPreprocessorDefinitions = value;
            }
        }
    }
}
