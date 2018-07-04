using System.Text;
using TLCGen.Extensions;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public static class CCOLHeaderGenerator
    {
        public static string GenerateFileHeader(ControllerDataModel data, string fileappend)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"/* KRUISPUNT: {data.Stad}");
            sb.Append(' ', 14);
            sb.AppendLine(data.Naam);
            sb.Append(' ', 14);
            sb.AppendLine(data.Straat1);
            sb.Append(' ', 14);
            sb.AppendLine(data.Straat2);
            sb.AppendLine();

            sb.AppendLine($"   BESTAND:   {data.Naam}{fileappend}");
            sb.AppendLine($"      CCOL:   {data.CCOLVersie.GetDescription()}");
            sb.AppendLine($"    TLCGEN:   {data.TLCGenVersie}");
            sb.AppendLine($"   CCOLGEN:   {CCOLCodeGeneratorPlugin.GetVersion()}");
            sb.AppendLine("*/");

            return sb.ToString();
        }

        public static string GenerateVersionHeader(ControllerDataModel data)
        {
            // Set up: variables
            var sb = new StringBuilder();
            var Distance = 3;
            const string verString = "Versie";
            const string datString = "Datum";
            const string ontString = "Ontwerper";
            const string comString = "Commentaar";
            var verLength = verString.Length + Distance;
            var datLength = datString.Length + Distance;
            var ontLength = ontString.Length + Distance;

            // Add top
            sb.AppendLine("/****************************** Versie commentaar ***********************************");
            sb.AppendLine(" *");

            // Determine 
            foreach (var vm in data.Versies)
            {
                if ((vm.Versie.Length + Distance) > verLength) verLength = vm.Versie.Length + Distance;
                if ((vm.Datum.ToShortDateString().Length + Distance) > datLength) datLength = vm.Datum.ToShortDateString().Length + Distance;
                if ((vm.Ontwerper.Length + Distance) > ontLength) ontLength = vm.Ontwerper.Length + Distance;
            }

            // Add title line
            sb.AppendFormat(" * {0}{1}{2}{3}",
                    verString.PadRight(verLength),
                    datString.PadRight(datLength),
                    ontString.PadRight(ontLength),
                    comString);
            sb.AppendLine();

            // Add version lines
            foreach (var vm in data.Versies)
            {
                sb.AppendFormat(" * {0}{1}{2}{3}",
                    vm.Versie.PadRight(verLength),
                    vm.Datum.ToShortDateString().PadRight(datLength),
                    vm.Ontwerper.PadRight(ontLength),
                    vm.Commentaar);
                sb.AppendLine();
            }

            // Add bottom
            sb.AppendLine(" *");
            sb.AppendLine(" ************************************************************************************/");

            return sb.ToString();
        }
    }

    public partial class CCOLGenerator
    {
        /// <summary>
        /// Generates a file header
        /// </summary>
        /// <param name="data">The ControllerDataModel instance that holds the info for generation</param>
        /// <param name="fileappend">The string to append to the Controller name to get the file name.</param>
        /// <returns>A string holding the file header</returns>
        private string GenerateFileHeader(ControllerDataModel data, string fileappend)
        {
            return CCOLHeaderGenerator.GenerateFileHeader(data, fileappend);
        }

        /// <summary>
        /// Generates a string with version info
        /// </summary>
        /// <param name="data">The ControllerDataModel instance that hold the version info</param>
        /// <returns>A string with version information</returns>
        private string GenerateVersionHeader(ControllerDataModel data)
        {
            return CCOLHeaderGenerator.GenerateVersionHeader(data);
        }   
    }
}
