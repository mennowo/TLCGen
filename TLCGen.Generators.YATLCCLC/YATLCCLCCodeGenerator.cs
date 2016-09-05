using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Interfaces.Public;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL
{
    [TLCGenGenerator]
    public class YATLCCLCCodeGenerator : IGenerator
    {
        #region IGenerator

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

        #endregion // IGenerator

        #region Private Methods

        #endregion // Private Methods

        #region Properties

        [DisplayName("YATLCCLC resources pad")]
        [Description("YATLCCLC resources pad")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting("application")]
        public string YATLCCLCResPath { get; set; }

        [DisplayName("Preprocessor definities")]
        [Description("Preprocessor definities")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting("application")]
        public string YATLCCLCPreprocessorDefinitions { get; set; }

        #endregion // Properties

    }
}
