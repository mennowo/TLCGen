using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateHstAddHeader(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine(_beginGeneratedHeader);
            sb.AppendLine("/* HALFSTAR, GEBRUIKERS TOEVOEGINGEN              */");
            sb.AppendLine("/* ---------------------------------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(c.Data, "hst.add"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(c.Data));
            sb.AppendLine(_endGeneratedHeader);

            return sb.ToString();
        }

        private string GenerateHstAdd(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine(GenerateRegAddHeader(c));
            sb.AppendLine();
            sb.AppendLine("void post_init_application_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void PreApplication_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void KlokPerioden_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Aanvragen_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Maxgroen_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Wachtgroen_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Meetkriterium_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Meeverlengen_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Synchronisaties_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Alternatief_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void RealisatieAfhandeling_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void FileVerwerking_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void DetectieStoring_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void PostApplication_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void pre_system_application_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void post_system_application_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void post_dump_application_halfstar_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("boolv application1_tig_Add()");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}return 0;");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("boolv application2_tig_Add()");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}return 0;");
            sb.AppendLine("}");
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
