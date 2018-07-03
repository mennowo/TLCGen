using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateTabAddHeader(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine(_beginGeneratedHeader);
            sb.AppendLine("/* INSTELLINGEN BESTAND, GEBRUIKERS TOEVOEGINGEN       */");
            sb.AppendLine("/* --------------------------------------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(c.Data, "tab.add"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(c.Data));
            sb.AppendLine(_endGeneratedHeader);

            return sb.ToString();
        }

        private string GenerateTabAdd(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine(GenerateTabAddHeader(c));
            sb.AppendLine();
            sb.AppendLine("/* dit is een placeholder */");
            sb.AppendLine("/* ---------------------- */");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
