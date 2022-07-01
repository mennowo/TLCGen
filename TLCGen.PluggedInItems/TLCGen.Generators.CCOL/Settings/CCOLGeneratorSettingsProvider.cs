using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Models;

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

        public CCOLGeneratorSettingsModel Settings { get; set; }

        public void Reset()
        {
            _lastItemDescription = new Dictionary<CCOLElementTypeEnum, string>();
            // Build dictionary with all available settings
            CCOLElementNames.Clear();
            foreach (var s in Settings.CodePieceGeneratorSettings)
            {
                foreach(var ss in s.Item2.Settings)
                {
                    CCOLElementNames.Add(GetDefaultPrefix(ss.Type) + ss.Default, ss.Setting);
                }
            }
        }

		public string GetElementDescription(string description, CCOLElementTypeEnum type, params string [] elementnames)
		{
			if (description == null) return null;
            var descr = description;
            var i = 1;
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
                var isSame = true;
                if (words.Length == lastWords.Length)
                {
                    for (var j = 0; j < words.Length; j++)
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
                        for (var j = 0; j < words.Length; j++)
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

        internal CCOLElement CreateElement(string v, int dimmingNiveauPeriodeNietDimmen, CCOLElementTimeTypeEnum none, object prmnivndim, string faseCyclus)
        {
            throw new NotImplementedException();
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

        public CCOLElement CreateElement(string name, CCOLGeneratorCodeStringSettingModel element, IOElementModel ioElementData, params string[] elementnames)
        {
            var t = TranslateType(element.Type);
            // skim element setting from name if it already starts with that string
            name = CCOLCodeHelper.GetNameFromCombinedNameAndElementName(element, name);
            // synch names
            ioElementData.Naam = name;
            return new CCOLElement(name, t, GetElementDescription(element.Description, t, elementnames), ioElementData);
        }

        public CCOLElement CreateElement(string name, CCOLElementTypeEnum type, string description)
        {
            return new CCOLElement(name, type, GetElementDescription(description, type));
        }

        public CCOLElement CreateElement(string name, CCOLElementTypeEnum type, IOElementModel ioElementData, string description)
        {
            return new CCOLElement(name, type, GetElementDescription(description, type), ioElementData);
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
            return type switch
            {
                CCOLGeneratorSettingTypeEnum.Uitgang => CCOLElementTypeEnum.Uitgang,
                CCOLGeneratorSettingTypeEnum.Ingang => CCOLElementTypeEnum.Ingang,
                CCOLGeneratorSettingTypeEnum.Timer => CCOLElementTypeEnum.Timer,
                CCOLGeneratorSettingTypeEnum.Counter => CCOLElementTypeEnum.Counter,
                CCOLGeneratorSettingTypeEnum.Schakelaar => CCOLElementTypeEnum.Schakelaar,
                CCOLGeneratorSettingTypeEnum.HulpElement => CCOLElementTypeEnum.HulpElement,
                CCOLGeneratorSettingTypeEnum.GeheugenElement => CCOLElementTypeEnum.GeheugenElement,
                CCOLGeneratorSettingTypeEnum.Parameter => CCOLElementTypeEnum.Parameter,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public string GetDefaultPrefix(CCOLGeneratorSettingTypeEnum type)
        {
            return type switch
            {
                CCOLGeneratorSettingTypeEnum.Uitgang => GetDefaultPrefix("us"),
                CCOLGeneratorSettingTypeEnum.Ingang => GetDefaultPrefix("is"),
                CCOLGeneratorSettingTypeEnum.HulpElement => GetDefaultPrefix("h"),
                CCOLGeneratorSettingTypeEnum.Timer => GetDefaultPrefix("t"),
                CCOLGeneratorSettingTypeEnum.Counter => GetDefaultPrefix("c"),
                CCOLGeneratorSettingTypeEnum.Schakelaar => GetDefaultPrefix("sch"),
                CCOLGeneratorSettingTypeEnum.GeheugenElement => GetDefaultPrefix("m"),
                CCOLGeneratorSettingTypeEnum.Parameter => GetDefaultPrefix("prm"),
                _ => null
            };
        }

        public string GetPrefix(CCOLGeneratorSettingTypeEnum type)
        {
            return type switch
            {
                CCOLGeneratorSettingTypeEnum.Uitgang => GetPrefix("us"),
                CCOLGeneratorSettingTypeEnum.Ingang => GetPrefix("is"),
                CCOLGeneratorSettingTypeEnum.HulpElement => GetPrefix("h"),
                CCOLGeneratorSettingTypeEnum.Timer => GetPrefix("t"),
                CCOLGeneratorSettingTypeEnum.Counter => GetPrefix("c"),
                CCOLGeneratorSettingTypeEnum.Schakelaar => GetPrefix("sch"),
                CCOLGeneratorSettingTypeEnum.GeheugenElement => GetPrefix("m"),
                CCOLGeneratorSettingTypeEnum.Parameter => GetPrefix("prm"),
                _ => null
            };
        }

        public string GetPrefix(CCOLElementTypeEnum type)
        {
            return type switch
            {
                CCOLElementTypeEnum.Fase => GetPrefix("fc"),
                CCOLElementTypeEnum.Detector => GetPrefix("d"),
                CCOLElementTypeEnum.Uitgang => GetPrefix("us"),
                CCOLElementTypeEnum.Ingang => GetPrefix("is"),
                CCOLElementTypeEnum.HulpElement => GetPrefix("h"),
                CCOLElementTypeEnum.Timer => GetPrefix("t"),
                CCOLElementTypeEnum.Counter => GetPrefix("c"),
                CCOLElementTypeEnum.Schakelaar => GetPrefix("sch"),
                CCOLElementTypeEnum.GeheugenElement => GetPrefix("m"),
                CCOLElementTypeEnum.Parameter => GetPrefix("prm"),
                _ => null
            };
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
