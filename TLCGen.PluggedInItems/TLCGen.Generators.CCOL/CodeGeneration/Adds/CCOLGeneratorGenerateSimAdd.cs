using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateSimAddHeader(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine(_beginGeneratedHeader);
            sb.AppendLine("/* DISPLAY BESTAND, GEBRUIKERS TOEVOEGINGEN            */");
            sb.AppendLine("/* --------------------------------------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(c.Data, "sim.add"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(c.Data));
            sb.AppendLine(_endGeneratedHeader);

            return sb.ToString();
        }

        private string GenerateSimAdd(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(GenerateSimAddHeader(controller));
            sb.AppendLine();
            sb.AppendLine("/* dit is een placeholder */");
            sb.AppendLine("/* ---------------------- */");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
