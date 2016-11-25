using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TLCGen.CustomPropertyEditors;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.Generators.CCOL
{
    [TLCGenPlugin(TLCGenPluginElems.Generator)]
    public partial class CCOLCodeGenerator : ITLCGenGenerator
    {
        #region ITLCGenGenerator

        public string GenerateSourceFiles(ControllerModel controller, string sourcefilepath)
        {
            if (Directory.Exists(sourcefilepath))
            {
                CollectAllCCOLElements(controller);

                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}reg.c"), GenerateRegC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}tab.c"), GenerateTabC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}dpl.c"), GenerateDplC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sim.c"), GenerateSimC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sys.h"), GenerateSysH(controller));

                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}reg.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}reg.add"), GenerateRegAdd(controller));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}tab.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}tab.add"), GenerateTabAdd(controller));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}dpl.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}dpl.add"), GenerateDplAdd(controller));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sim.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sim.add"), GenerateSimAdd(controller));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sys.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sys.add"), GenerateSysAdd(controller));
                return "CCOL source code gegenereerd";
            }
            return $"Map {sourcefilepath} niet gevonden. Niets gegenereerd.";
        }

        public string GenerateProjectFiles(ControllerModel controller, string projectfilepath)
        {
            string result = "test";

            File.WriteAllText(Path.Combine(projectfilepath, $"{controller.Data.Naam}.vcxproj"), GenerateVisualStudioProjectFiles(controller));

            return result;
        }

        public string GenerateSpecification(ControllerModel controller, string specificationfilepath)
        {
            string result = "";

            return result;
        }

        public string GetGeneratorName()
        {
            return "CCOL";
        }

        public string GetGeneratorVersion()
        {
            return "0.1 (alfa)";
        }

        public string GetPluginName()
        {
            return GetGeneratorName();
        }


        #endregion // ITLCGenGenerator

        #region Fields

        private CCOLElemListData Uitgangen;
        private CCOLElemListData Ingangen;
        private CCOLElemListData HulpElementen;
        private CCOLElemListData GeheugenElementen;
        private CCOLElemListData Timers;
        private CCOLElemListData Counters;
        private CCOLElemListData Schakelaars;
        private CCOLElemListData Parameters;
        private List<DetectorModel> AlleDetectoren;

        private string tabspace = "    ";

        #endregion // Fields

        #region Setting Properties

        private string _CCOLIncludesPaden;
        [DisplayName("CCOL include paden")]
        [Description("CCOL include paden")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        public string CCOLIncludesPaden
        {
            get { return _CCOLIncludesPaden; }
            set
            {
                if(!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
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
            get { return _CCOLLibsPath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLLibsPath = value + ";";
                else
                    _CCOLLibsPath = value;
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
            get { return _CCOLResPath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLResPath = value + ";";
                else
                    _CCOLResPath = value;
            }
        }

        private string _CCOLPreprocessorDefinitions;
        [DisplayName("Preprocessor definities")]
        [Description("Preprocessor definities")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        public string CCOLPreprocessorDefinitions
        {
            get { return _CCOLPreprocessorDefinitions; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLPreprocessorDefinitions = value + ";";
                else
                    _CCOLPreprocessorDefinitions = value;
            }
        }

        #endregion // Setting Properties

    }
}
