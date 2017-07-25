using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.TLCCC.CodeGeneration
{
    public partial class TLCCCGenerator
    {
        /// <summary>
        /// Generates a file header
        /// </summary>
        /// <param name="data">The ControllerDataModel instance that holds the info for generation</param>
        /// <param name="fileappend">The string to append to the Controller name to get the file name.</param>
        /// <returns>A string holding the file header</returns>
        private string GenerateFileHeader(ControllerDataModel data, string fileappend)
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
            sb.AppendLine($"     TLCCC:   0.1");
            sb.AppendLine($"    TLCGEN:   {data.TLCGenVersie}");
            sb.AppendLine($"  TLCCCGEN:   0.1");
            sb.AppendLine("*/");

            return sb.ToString();
        }

        /// <summary>
        /// Generates a string with version info
        /// </summary>
        /// <param name="data">The ControllerDataModel instance that hold the version info</param>
        /// <returns>A string with version information</returns>
        private string GenerateVersionHeader(ControllerDataModel data)
        {
            // Set up: variables
            var sb = new StringBuilder();
            const int distance = 3;
            const string verString = "Versie";
            const string datString = "Datum";
            const string ontString = "Ontwerper";
            const string comString = "Commentaar";
            var verLength = verString.Length + distance;
            var datLength = datString.Length + distance;
            var ontLength = ontString.Length + distance;

            // Add top
            sb.AppendLine("/****************************** Versie commentaar ***********************************");
            sb.AppendLine(" *");

            // Determine 
            foreach (var vm in data.Versies)
            {
                if ((vm.Versie.Length + distance) > verLength) verLength = vm.Versie.Length + distance;
                if ((vm.Datum.ToShortDateString().Length + distance) > datLength) datLength = vm.Datum.ToShortDateString().Length + distance;
                if ((vm.Ontwerper.Length + distance) > ontLength) ontLength = vm.Ontwerper.Length + distance;
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
}