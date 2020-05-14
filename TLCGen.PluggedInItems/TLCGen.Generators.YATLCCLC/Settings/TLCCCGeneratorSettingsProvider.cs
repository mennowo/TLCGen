using System.Collections.Generic;
using System.Linq;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Generators.TLCCC.CodeGeneration;

namespace TLCGen.Generators.TLCCC.Settings
{
    public class TLCCCGeneratorSettingsProvider
    {
        private static TLCCCGeneratorSettingsProvider _Default;
        public static TLCCCGeneratorSettingsProvider Default
        {
            get
            {
                if(_Default == null)
                {
                    _Default = new TLCCCGeneratorSettingsProvider();
                }
                return _Default;
            }
        }

        public TLCCCGeneratorSettingsProvider()
        {
            TLCCCElementNames = new Dictionary<string, string>();
            Settings = new TLCCCGeneratorSettingsModel();
        }

        public Dictionary<string, string> TLCCCElementNames { get; set; }

        private TLCCCGeneratorSettingsModel _Settings;
        public TLCCCGeneratorSettingsModel Settings
        {
            get => _Settings;
            set => _Settings = value;
        }

        public string GetPrefix(TLCCCElementTypeEnum type)
        {
            return Settings.Prefixes.Any(x => x.Type == type) ? Settings.Prefixes.First(x => x.Type == type).Setting : null;
        }
        
        public string GetPrefix(string pfdefault)
        {
            return Settings.Prefixes.Any(x => x.Default == pfdefault) ? Settings.Prefixes.First(x => x.Default == pfdefault).Setting : null;
        }
    }
}
