using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Generators.CCOL.Settings
{
    [Serializable]
    [XmlRoot(ElementName = "CCOLGeneratorSettings")]
    public class CCOLGeneratorSettingsModel
    {
        #region Properties

        public CCOLGeneratorVisualSettingsModel VisualSettings { get; set; }
        public CCOLGeneratorVisualSettingsModel VisualSettingsCCOL9 { get; set; }
        public CCOLGeneratorVisualSettingsModel VisualSettingsCCOL95 { get; set; }
        public CCOLGeneratorVisualSettingsModel VisualSettingsCCOL100 { get; set; }
        public CCOLGeneratorVisualSettingsModel VisualSettingsCCOL110 { get; set; }
        public CCOLGeneratorVisualSettingsModel VisualSettingsCCOL120 { get; set; }
        public CCOLGeneratorVisualSettingsModel VisualSettingsCCOL121 { get; set; }

        [XmlIgnore]
        public CCOLGeneratorVisualSettingsModel[] AllVisualSettings 
        {
            get
            {
                return new[]
                {
                    VisualSettings,
                    VisualSettingsCCOL9,
                    VisualSettingsCCOL95,
                    VisualSettingsCCOL100,
                    VisualSettingsCCOL110,
                    VisualSettingsCCOL120,
                    VisualSettingsCCOL121,
                };
            }
        }

        [XmlArrayItem(ElementName = "Prefix")]
        public List<CCOLGeneratorCodeStringSettingModel> Prefixes { get; set; }

        [XmlArrayItem(ElementName = "CodePieceGenerator")]
        public List<CodePieceSettingsTuple<string, CCOLGeneratorClassWithSettingsModel>> CodePieceGeneratorSettings { get; set; }

        public bool ReplaceRepeatingCommentsTextWithPeriods { get; set; }
        public bool AlwaysOverwriteSources { get; set; }
        public bool AlterAddFunctionsWhileGenerating { get; set; }
        public bool AlterAddHeadersWhileGenerating { get; set; }

        [XmlIgnore]
        public string TabSpace { get; set; }

        public string TabSpaceSerialized
        {
            get => TabSpace.Replace(' ', 's').Replace('\t', 't');
            set => TabSpace = value.Replace('s', ' ').Replace('t', '\t');
        }

        #endregion // Properties

        #region Constructor

        public CCOLGeneratorSettingsModel()
        {
            VisualSettings = new CCOLGeneratorVisualSettingsModel();
            VisualSettingsCCOL9 = new CCOLGeneratorVisualSettingsModel();
            VisualSettingsCCOL95 = new CCOLGeneratorVisualSettingsModel();
            VisualSettingsCCOL100 = new CCOLGeneratorVisualSettingsModel();
            VisualSettingsCCOL110 = new CCOLGeneratorVisualSettingsModel();
            VisualSettingsCCOL120 = new CCOLGeneratorVisualSettingsModel();
            VisualSettingsCCOL121 = new CCOLGeneratorVisualSettingsModel();
            Prefixes = new List<CCOLGeneratorCodeStringSettingModel>();
            CodePieceGeneratorSettings = new List<CodePieceSettingsTuple<string, CCOLGeneratorClassWithSettingsModel>>();
        }

        #endregion // Constructor
    }
}
