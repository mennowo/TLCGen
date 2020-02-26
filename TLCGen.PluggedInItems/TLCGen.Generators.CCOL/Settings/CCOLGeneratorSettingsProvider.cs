using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TLCGen.Generators.CCOL.CodeGeneration;

namespace TLCGen.Generators.CCOL.Settings
{
    public class CCOLGeneratorSettingsProvider
    {
        private static CCOLGeneratorSettingsProvider _default;
        public static CCOLGeneratorSettingsProvider Default => _default ?? (_default = new CCOLGeneratorSettingsProvider());

        private Dictionary<CCOLElementTypeEnum, string> _lastItemDescription;

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
                        CCOLElementNames.Add(GetPrefix(ss.Type) + ss.Default, ss.Setting);
                    }
                }
            }
        }

        public void Reset()
        {
            _lastItemDescription = new Dictionary<CCOLElementTypeEnum, string>();
        }

		public string GetElementDescription(string description, CCOLElementTypeEnum type, params string [] elementnames)
		{
			if (description == null) return null;
            var descr = description;
            int i = 1;
			foreach(var e in elementnames)
			{
                if (e == null) continue;
                descr = descr.Replace("_E" + i + "_", e);
				++i;
			}
            // Remove empty tags
            descr = Regex.Replace(descr, @"\s_E[0-9]_", "");
            var finalDescr = descr;

            if (!CCOLGeneratorSettingsProvider.Default.Settings.ReplaceRepeatingCommentsTextWithPeriods) return descr;

            // Check last line
            if (_lastItemDescription.ContainsKey(type))
            {
                var re = new Regex("[0-9]+");
                var re2 = new Regex(@"[a-zA-Z0-9\(\)]");
                var words = descr.Split(' ');
                var lastWords = _lastItemDescription[type].Split(' ');
                bool isSame = true;
                if (words.Length == lastWords.Length)
                {
                    for (int j = 0; j < words.Length; j++)
                    {
                        if (!(words[j] == lastWords[j] || /*words[j].Length == lastWords[j].Length &&*/ re.IsMatch(words[j])))
                        {
                            isSame = false;
                            break;
                        }
                    }
                    if (isSame)
                    {
                        descr = "";
                        for (int j = 0; j < words.Length; j++)
                        {
                            if (descr != "") descr += ".";
                            if (words[j] == lastWords[j])
                            {
                                descr += re2.Replace(words[j], ".");
                            }
                            else
                            {
                                descr += words[j];
                            }
                        }
                    }
                }
                _lastItemDescription[type] = finalDescr;
            }
            else
            {
                _lastItemDescription.Add(type, finalDescr);
            }

            return descr;	
		}

        public CCOLElement CreateElement(string name, int setting, CCOLElementTimeTypeEnum timeType, CCOLGeneratorCodeStringSettingModel element, params string [] elementnames)
        {
            var t = TranslateType(element.Type);
            return new CCOLElement(name, setting, timeType, t,
                    GetElementDescription(element.Description, t, elementnames));
        }

        public CCOLElement CreateElement(string name, CCOLGeneratorCodeStringSettingModel element, params string[] elementnames)
        {
            var t = TranslateType(element.Type);
            return new CCOLElement(name, t,
                    GetElementDescription(element.Description, t, elementnames));
        }

        public CCOLElement CreateElement(string name, CCOLElementTypeEnum type, string description)
        {
            return new CCOLElement(name, type, GetElementDescription(description, type));
        }

        public CCOLElement CreateElement(string name, int setting, CCOLElementTimeTypeEnum timeType, CCOLElementTypeEnum type, string description)
        {
            return new CCOLElement(name, setting, timeType, type, GetElementDescription(description, type));
        }

        public string GetElementName(string defaultwithprefix)
        {
            var result = CCOLElementNames.TryGetValue(defaultwithprefix, out var n);
            if (result == false) throw new KeyNotFoundException($"The default {defaultwithprefix} was not found. Code generation will be faulty.");
            return n;
        }

        private CCOLElementTypeEnum TranslateType(CCOLGeneratorSettingTypeEnum type)
        {
            switch (type)
            {
                case CCOLGeneratorSettingTypeEnum.Uitgang:
                    return CCOLElementTypeEnum.Uitgang;
                case CCOLGeneratorSettingTypeEnum.Ingang:
                    return CCOLElementTypeEnum.Ingang;
                case CCOLGeneratorSettingTypeEnum.Timer:
                    return CCOLElementTypeEnum.Timer;
                case CCOLGeneratorSettingTypeEnum.Counter:
                    return CCOLElementTypeEnum.Counter;
                case CCOLGeneratorSettingTypeEnum.Schakelaar:
                    return CCOLElementTypeEnum.Schakelaar;
                case CCOLGeneratorSettingTypeEnum.HulpElement:
                    return CCOLElementTypeEnum.HulpElement;
                case CCOLGeneratorSettingTypeEnum.GeheugenElement:
                    return CCOLElementTypeEnum.GeheugenElement;
                case CCOLGeneratorSettingTypeEnum.Parameter:
                    return CCOLElementTypeEnum.Parameter;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string GetDefaultPrefix(CCOLGeneratorSettingTypeEnum type)
        {
            switch (type)
            {
                case CCOLGeneratorSettingTypeEnum.Uitgang:
                    return GetDefaultPrefix("us");
                case CCOLGeneratorSettingTypeEnum.Ingang:
                    return GetDefaultPrefix("is");
                case CCOLGeneratorSettingTypeEnum.HulpElement:
                    return GetDefaultPrefix("h");
                case CCOLGeneratorSettingTypeEnum.Timer:
                    return GetDefaultPrefix("t");
                case CCOLGeneratorSettingTypeEnum.Counter:
                    return GetDefaultPrefix("c");
                case CCOLGeneratorSettingTypeEnum.Schakelaar:
                    return GetDefaultPrefix("sch");
                case CCOLGeneratorSettingTypeEnum.GeheugenElement:
                    return GetDefaultPrefix("m");
                case CCOLGeneratorSettingTypeEnum.Parameter:
                    return GetDefaultPrefix("prm");
                default:
                    return null;
            }
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

        public string GetDefaultPrefix(string pfdefault)
        {
            if (Settings.Prefixes.Any(x => x.Default == pfdefault))
            {
                return Settings.Prefixes.First(x => x.Default == pfdefault).Default;
            }
            return null;
        }
    }
}
