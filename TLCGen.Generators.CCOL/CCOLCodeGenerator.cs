using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Interfaces.Public;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL
{
    [TLCGenGenerator]
    public partial class CCOLCodeGenerator : IGenerator
    {
        #region IGenerator

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

        #endregion // IGenerator

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

        #region Properties

        [DisplayName("CCOL include paden")]
        [Description("CCOL include paden")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting("application")]
        public string CCOLIncludesPad { get; set; }

        [DisplayName("CCOL library pad")]
        [Description("CCOL library pad")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting("application")]
        public string CCOLLibsPath { get; set; }

        [DisplayName("CCOL resources pad")]
        [Description("CCOL resources pad")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting("application")]
        public string CCOLResPath { get; set; }

        [DisplayName("Preprocessor definities")]
        [Description("Preprocessor definities")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting("application")]
        public string CCOLPreprocessorDefinitions { get; set; }


        [DisplayName("Test set")]
        [Description("Descr")]
        [Category("Test")]
        [TLCGenCustomSetting("application")]
        public string TestSetting { get; set; }

        [DisplayName("Example setting")]
        [Description("Example of a custom generator setting per controller")]
        [Category("Controller CCOL settings")]
        //[TLCGenCustomSetting("controller")]
        public string ExampleOfGeneratorSettingPerController { get; set; }

        #endregion // Properties

    }
}
