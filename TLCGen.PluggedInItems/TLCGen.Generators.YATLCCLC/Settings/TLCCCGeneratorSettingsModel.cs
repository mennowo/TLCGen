using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Generators.TLCCC.CodeGeneration;

namespace TLCGen.Generators.CCOL.Settings
{
    [Serializable]
    [XmlRoot(ElementName = "CCOLGeneratorSettings")]
    public class TLCCCGeneratorSettingsModel
    {
        #region Properties

        [XmlArrayItem(ElementName = "Prefix")]
        public List<TLCCCGeneratorCodeStringSettingModel> Prefixes { get; set; }

        [XmlIgnore]
        public string TabSpace { get; set; }
        public string TabSpaceSerialized
        {
            get => TabSpace.Replace(' ', 's').Replace('\t', 't');
            set => TabSpace = value.Replace('s', ' ').Replace('t', '\t');
        }

        #endregion // Properties

        #region Constructor

        public TLCCCGeneratorSettingsModel()
        {
            Prefixes = new List<TLCCCGeneratorCodeStringSettingModel>();
            Prefixes.Add(new TLCCCGeneratorCodeStringSettingModel { Default = "sg", Type = TLCCCElementTypeEnum.SignalGroup });
            Prefixes.Add(new TLCCCGeneratorCodeStringSettingModel { Default = "os", Type = TLCCCElementTypeEnum.Output });
            Prefixes.Add(new TLCCCGeneratorCodeStringSettingModel { Default = "d", Type = TLCCCElementTypeEnum.Detector });
            Prefixes.Add(new TLCCCGeneratorCodeStringSettingModel { Default = "is", Type = TLCCCElementTypeEnum.Input });
            Prefixes.Add(new TLCCCGeneratorCodeStringSettingModel { Default = "t", Type = TLCCCElementTypeEnum.Timer });
            Prefixes.Add(new TLCCCGeneratorCodeStringSettingModel { Default = "sw", Type = TLCCCElementTypeEnum.Switch });
            Prefixes.Add(new TLCCCGeneratorCodeStringSettingModel { Default = "prm", Type = TLCCCElementTypeEnum.Parameter });
            TabSpace = "    ";
        }

        #endregion // Constructor
    }
}
