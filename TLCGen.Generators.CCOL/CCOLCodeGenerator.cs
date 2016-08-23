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
    public partial class CCOLCodeGenerator : IGenerator
    {
        #region IGenerator

        public string GenerateSourceFiles(ControllerModel controller, string sourcefilepath)
        {
            if (Directory.Exists(sourcefilepath))
            {
                string result = "";
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}reg.c"), GenerateRegC(controller));
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
    }
}
