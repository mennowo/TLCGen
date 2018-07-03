using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateDplAddHeader(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine(_beginGeneratedHeader);
            sb.AppendLine("/* DISPLAY BESTAND, GEBRUIKERS TOEVOEGINGEN            */");
            sb.AppendLine("/* --------------------------------------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(c.Data, "dpl.add"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(c.Data));
            sb.AppendLine(_endGeneratedHeader);

            return sb.ToString();
        }

        private string GenerateDplAdd(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.Append(GenerateDplAddHeader(c));
            sb.AppendLine();
            sb.AppendLine("/* extra fasecycli */");
            sb.AppendLine("/* --------------- */");
            sb.AppendLine();
            sb.AppendLine("/* extra overige uitgangen */");
            sb.AppendLine("/* ----------------------- */");
            sb.AppendLine();
            sb.AppendLine("/* extra detectie */");
            sb.AppendLine("/* -------------- */");
            sb.AppendLine();
            sb.AppendLine("/* extra overige ingangen */");
            sb.AppendLine("/* ---------------------- */");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
