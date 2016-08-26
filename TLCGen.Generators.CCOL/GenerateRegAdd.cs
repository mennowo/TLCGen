using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL
{
    public partial class CCOLCodeGenerator
    {
        private string GenerateRegAdd(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* REGEL BESTAND, GEBRUIKERS TOEVOEGINGEN              */");
            sb.AppendLine("/* (gegenereerde headers niet wijzigen of verwijderen) */");
            sb.AppendLine("/* --------------------------------------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "reg.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));

            sb.AppendLine();
            sb.AppendLine("post_init_application()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("KlokPerioden_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("Aanvragen_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("Maxgroen_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("Wachtgroen_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("KlokPerioden_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("Meetkriterium_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("Meeverlengen_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("VersneldPrimair_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            if (controller.ModuleMolen.LangstWachtendeAlternatief)
            {
                sb.AppendLine("Alternatief_Add()");
                sb.AppendLine("{");
                sb.AppendLine("");
                sb.AppendLine("}");
                sb.AppendLine();
            }
            sb.AppendLine("Modules_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("RealisatieAfhandeling_Add()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("pre_system_application()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("post_system_application()");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
