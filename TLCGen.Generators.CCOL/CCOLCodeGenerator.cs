using System;
using System.Collections.Generic;
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

                string result = "";
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}reg.c"), GenerateRegC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}tab.c"), GenerateTabC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}dpl.c"), GenerateDplC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sys.h"), GenerateSysH(controller));
                return result;
            }
            return $"Map {sourcefilepath} niet gevonden. Niets gegenereerd.";
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

        private string tabspace = "    ";

        #endregion // Fields

        #region Properties

        [TLCGenGeneratorSetting("application")]
        public string Test1 { get; set; }
        [TLCGenGeneratorSetting("application")]
        public string Test2 { get; set; }
        [TLCGenGeneratorSetting("controller")]
        public string Test3 { get; set; }
        [TLCGenGeneratorSetting("controller")]
        public double Test4 { get; set; }

        #endregion // Properties

    }
}
