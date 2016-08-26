using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Extensions;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL
{
    public partial class CCOLCodeGenerator
    {
        /// <summary>
        /// Generates a file header
        /// </summary>
        /// <param name="data">The ControllerDataModel instance that holds the info for generation</param>
        /// <param name="fileappend">The string to append to the Controller name to get the file name.</param>
        /// <returns>A string holding the file header</returns>
        private string GenerateFileHeader(ControllerDataModel data, string fileappend)
        {
            StringBuilder sb = new StringBuilder();

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
            sb.AppendLine($"    TLCGEN:   {data.TLCGenVersie.GetDescription()}");
            sb.AppendLine($"   COLLGEN:   {GetGeneratorVersion()}");

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
            StringBuilder sb = new StringBuilder();
            int Distance = 3;
            string VerString = "Versie";
            string DatString = "Datum";
            string OntString = "Ontwerper";
            string ComString = "Commentaar";
            int VerLength = VerString.Length + Distance;
            int DatLength = DatString.Length + Distance;
            int OntLength = OntString.Length + Distance;

            // Add top
            sb.AppendLine("/****************************** Versie commentaar ***********************************");
            sb.AppendLine(" *");

            // Determine 
            foreach (VersieModel vm in data.Versies)
            {
                if ((vm.Versie.Length + Distance) > VerLength) VerLength = vm.Versie.Length + Distance;
                if ((vm.Datum.ToShortDateString().Length + Distance) > DatLength) DatLength = vm.Datum.ToShortDateString().Length + Distance;
                if ((vm.Ontwerper.Length + Distance) > OntLength) OntLength = vm.Ontwerper.Length + Distance;
            }

            // Add title line
            sb.AppendFormat(" * {0}{1}{2}{3}",
                    VerString.PadRight(VerLength),
                    DatString.PadRight(DatLength),
                    OntString.PadRight(OntLength),
                    ComString);
            sb.AppendLine();

            // Add version lines
            foreach (VersieModel vm in data.Versies)
            {
                sb.AppendFormat(" * {0}{1}{2}{3}",
                    vm.Versie.PadRight(VerLength),
                    vm.Datum.ToShortDateString().PadRight(DatLength),
                    vm.Ontwerper.PadRight(OntLength),
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
