using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.Generators.CCOL
{
    [TLCGenPlugin(TLCGenPluginElems.Generator)]
    public class YATLCCLCCodeGenerator : ITLCGenGenerator
    {
        #region ITLCGenGenerator

        public string GenerateSourceFiles(ControllerModel controller, string sourcefilepath)
        {
            string result = "niets gegenereerd, dit is een placeholder/demo plugin!";

            return result;
        }

        public string GenerateProjectFiles(ControllerModel controller, string projectfilepath)
        {
            string result = "";

            return result;
        }

        public string GenerateSpecification(ControllerModel controller, string specificationfilepath)
        {
            string result = "";

            return result;
        }

        public string GetGeneratorName()
        {
            return "YATLCCLC";
        }

        public string GetGeneratorVersion()
        {
            return "0";
        }

        public string GetPluginName()
        {
            return GetGeneratorName();
        }

        #endregion // ITLCGenGenerator

        #region Private Methods

        #endregion // Private Methods

        #region Properties

        [DisplayName("YATLCCLC resources pad")]
        [Description("YATLCCLC resources pad")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting]
        public string YATLCCLCResPath { get; set; }

        [DisplayName("Preprocessor definities")]
        [Description("Preprocessor definities")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting]
        public string YATLCCLCPreprocessorDefinitions { get; set; }

        #endregion // Properties

    }
}
