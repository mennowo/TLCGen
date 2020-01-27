using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Generators.CCOL.Settings
{
    [Serializable]
    public class CCOLGeneratorClassWithSettingsModel
    {
        public string ClassName { get; set; }
        public string Description { get; set; }
        public int ElementGenerationOrder { get; set; }

        [XmlArrayItem(ElementName = "Setting")]
        public List<CCOLGeneratorCodeStringSettingModel> Settings { get; set; }
    }
}
