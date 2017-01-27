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
        private static CCOLGeneratorSettingsProvider _Default;
        public static CCOLGeneratorSettingsProvider Default
        {
            get
            {
                if(_Default == null)
                {
                    _Default = new CCOLGeneratorSettingsProvider();
                }
                return _Default;
            }
        }

        public CCOLGeneratorSettingsModel Settings { get; set; }

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
            if (Settings.Prefixes.Where(x => x.Default == pfdefault).Any())
            {
                return Settings.Prefixes.Where(x => x.Default == pfdefault).First().Setting;
            }
            return null;
        }
    }
}
