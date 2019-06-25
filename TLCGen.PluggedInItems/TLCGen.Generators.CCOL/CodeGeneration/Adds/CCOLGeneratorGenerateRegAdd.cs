using System.IO;
using System.Linq;
using System.Text;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateRegAddHeader(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine(_beginGeneratedHeader);
            sb.AppendLine("/* REGEL BESTAND, GEBRUIKERS TOEVOEGINGEN              */");
            sb.AppendLine("/* --------------------------------------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(c.Data, "reg.add"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(c.Data));
            sb.AppendLine(_endGeneratedHeader);

            return sb.ToString();
        }

        private string GenerateRegAdd(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine(GenerateRegAddHeader(c));
            sb.AppendLine();
            sb.AppendLine("#ifdef CCOL_IS_SPECIAL");
            sb.AppendLine("void SpecialSignals_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("void post_init_application()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void PreApplication_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void KlokPerioden_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Aanvragen_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Maxgroen_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Wachtgroen_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Meetkriterium_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Meeverlengen_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Synchronisaties_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void VersneldPrimair_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Alternatief_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void Modules_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void RealisatieAfhandeling_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void FileVerwerking_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void DetectieStoring_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void PostApplication_Add(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            if (c.Fasen.Any(x => x.WachttijdVoorspeller))
            {
                sb.AppendLine("void WachtijdvoorspellersWachttijd_Add(void)");
                sb.AppendLine("{");
                sb.AppendLine("");
                sb.AppendLine("}");
                sb.AppendLine();
            }
            sb.AppendLine("void pre_system_application(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void post_system_application(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL9)
            {
                sb.AppendLine("void post_system_application2(void)");
                sb.AppendLine("{");
                sb.AppendLine("");
                sb.AppendLine("}");
                sb.AppendLine();
            }
            sb.AppendLine("void post_dump_application(void)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
