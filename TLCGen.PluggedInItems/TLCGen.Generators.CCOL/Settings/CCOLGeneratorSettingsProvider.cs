using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.CodeGeneration;

namespace TLCGen.Generators.CCOL.Settings
{
    public class CCOLGeneratorSettingsProvider
    {
        private static CCOLGeneratorSettingsProvider _default;
        public static CCOLGeneratorSettingsProvider Default => _default ?? (_default = new CCOLGeneratorSettingsProvider());

	    public CCOLGeneratorSettingsProvider()
        {
            CCOLElementNames = new Dictionary<string, string>();
        }

        public Dictionary<string, string> CCOLElementNames { get; set; }

        private CCOLGeneratorSettingsModel _settings;
        public CCOLGeneratorSettingsModel Settings
        {
            get => _settings;
	        set
            {
                _settings = value;
                
                // Build dictionary with all available settings
                CCOLElementNames.Clear();
                foreach (var s in _settings.CodePieceGeneratorSettings)
                {
                    foreach(var ss in s.Item2.Settings)
                    {
                        CCOLElementNames.Add(Default.GetPrefix(ss.Type) + ss.Default, ss.Setting);
                    }
                }
            }
        }

        public string GetElementName(string defaultwithprefix)
        {
	        return CCOLElementNames.TryGetValue(defaultwithprefix, out var n) ? n : null;
        }

        public string GetPrefix(CCOLGeneratorSettingTypeEnum type)
        {
            switch (type)
            {
                case CCOLGeneratorSettingTypeEnum.Uitgang:
                    return GetPrefix("us");
                case CCOLGeneratorSettingTypeEnum.Ingang:
                    return GetPrefix("is");
                case CCOLGeneratorSettingTypeEnum.HulpElement:
                    return GetPrefix("h");
                case CCOLGeneratorSettingTypeEnum.Timer:
                    return GetPrefix("t");
                case CCOLGeneratorSettingTypeEnum.Counter:
                    return GetPrefix("c");
                case CCOLGeneratorSettingTypeEnum.Schakelaar:
                    return GetPrefix("sch");
                case CCOLGeneratorSettingTypeEnum.GeheugenElement:
                    return GetPrefix("m");
                case CCOLGeneratorSettingTypeEnum.Parameter:
                    return GetPrefix("prm");
                default:
                    return null;
            }
        }

        public string GetPrefix(CCOLElementTypeEnum type)
        {
            switch(type)
            {
                case CCOLElementTypeEnum.Fase:
                    return GetPrefix("fc");
                case CCOLElementTypeEnum.Detector:
                    return GetPrefix("d");
                case CCOLElementTypeEnum.Uitgang:
                    return GetPrefix("us");
                case CCOLElementTypeEnum.Ingang:
                    return GetPrefix("is");
                case CCOLElementTypeEnum.HulpElement:
                    return GetPrefix("h");
                case CCOLElementTypeEnum.Timer:
                    return GetPrefix("t");
                case CCOLElementTypeEnum.Counter:
                    return GetPrefix("c");
                case CCOLElementTypeEnum.Schakelaar:
                    return GetPrefix("sch");
                case CCOLElementTypeEnum.GeheugenElement:
                    return GetPrefix("m");
                case CCOLElementTypeEnum.Parameter:
                    return GetPrefix("prm");
                default:
                    return null;
            }
        }
        
        public string GetPrefix(string pfdefault)
        {
            if (Settings.Prefixes.Any(x => x.Default == pfdefault))
            {
                return Settings.Prefixes.First(x => x.Default == pfdefault).Setting;
            }
            return null;
        }
    }
}
