using System.ComponentModel;
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
            get => _CCOLIncludesPaden;
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
            get => _CCOLLibsPath;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLLibsPath = value + ";";
                else
                    _CCOLLibsPath = value;
            }
        }

		private string _CCOLLibsPathNoTig;
		[DisplayName("CCOL library pad NO_TIGMAX")]
		[Description("CCOL library pad NO_TIGMAX")]
		[Category("Visual project settings")]
		[TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
		[Editor(typeof(FolderEditor), typeof(FolderEditor))]
		[Browsable(false)]
		public string CCOLLibsPathNoTig
		{
			get => _CCOLLibsPathNoTig;
            set
			{
				if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
					_CCOLLibsPathNoTig = value + ";";
				else
					_CCOLLibsPathNoTig = value;
			}
		}

		private string _CCOLLibs;
        [DisplayName("CCOL extra libraries")]
        [Description("CCOL extra libraries (indien van toepassing)")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        public string CCOLLibs
        {
            get => _CCOLLibs;
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
            get => _CCOLResPath;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLResPath = value + ";";
                else
                    _CCOLResPath = value;
            }
        }

        private string _CCOLPreprocessorDefinitions;
        [DisplayName("Extra preprocessor definities")]
        [Description("Extra preprocessor definities")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        public string CCOLPreprocessorDefinitions
        {
            get => _CCOLPreprocessorDefinitions;
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
