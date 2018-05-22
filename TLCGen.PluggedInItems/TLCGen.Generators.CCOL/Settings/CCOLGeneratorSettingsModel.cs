using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [XmlArrayItem(ElementName = "Prefix")]
        public List<CCOLGeneratorCodeStringSettingModel> Prefixes { get; set; }

        [XmlArrayItem(ElementName = "CodePieceGenerator")]
        public List<CodePieceSettingsTuple<string, CCOLGeneratorClassWithSettingsModel>> CodePieceGeneratorSettings { get; set; }

        public bool AlwaysOverwriteSources { get; set; }
        public bool AlterAddHeadersWhileGenerating { get; set; }

        [XmlIgnore]
        public string TabSpace { get; set; }

        public string TabSpaceSerialized
        {
            get
            {
                return TabSpace.Replace(' ', 's').Replace('\t', 't');
            }
            set
            {
                TabSpace = value.Replace('s', ' ').Replace('t', '\t');
            }
        }

        #endregion // Properties

        #region Constructor

        public CCOLGeneratorSettingsModel()
        {
            VisualSettings = new CCOLGeneratorVisualSettingsModel();
            VisualSettingsCCOL9 = new CCOLGeneratorVisualSettingsModel();
            Prefixes = new List<CCOLGeneratorCodeStringSettingModel>();
            CodePieceGeneratorSettings = new List<CodePieceSettingsTuple<string, CCOLGeneratorClassWithSettingsModel>>();
        }

        #endregion // Constructor
    }
}
