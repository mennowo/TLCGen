using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.Generators.CCOL
{
    [TLCGenPlugin(TLCGenPluginElems.Generator)]
    public class YATLCCLCCodeGenerator : ITLCGenGenerator
    {
        #region ITLCGenGenerator

        public UserControl GeneratorView
        {
            get
            {
                return null;
            }
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

        public ControllerModel Controller
        {
            get;
            set;
        }

        public void GenerateController()
        {

        }

        public bool CanGenerateController()
        {
            return false;
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
