using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GeneratePraticeCcolReg(ControllerModel controller)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{controller.Data.Naam}sys.h ccolreg2.txt vgs {controller.Data.CCOLVersie.GetDescription()}");
            return sb.ToString();
        }

        private string GeneratePraticeCcolReg2(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#define CCOL_IS_SPECIAL");
            sb.AppendLine("#define XTND_DIC");
            sb.AppendLine();

            if (controller.ModuleMolen.LangstWachtendeAlternatief)
            {
                sb.AppendLine("#include \"lwmlfunc.c\"");
            }
            sb.AppendLine("#include \"fbericht.c\"");
            sb.AppendLine();
            sb.AppendLine($"#include \"{controller.Data.Naam}reg.c\"");
            sb.AppendLine($"#include \"{controller.Data.Naam}tab.c\"");
            if (controller.HasPTorHD()) sb.AppendLine($"#include \"{controller.Data.Naam}prio.c\"");
            if (controller.Data.VLOGInTestOmgeving)
            {
                sb.AppendLine();
                sb.AppendLine($"#include \"ccol_mon.c\"");
            }
            sb.AppendLine();
            if (controller.InterSignaalGroep.Gelijkstarten.Any() ||
                controller.InterSignaalGroep.Voorstarten.Any())
            {
                sb.AppendLine($"#include \"syncfunc.c\"");
            }
            if (controller.HasDSI())
            {
                sb.AppendLine($"#include \"dsifunc.c\"");
            }
            sb.AppendLine($"#include \"xtnd_dic.c\"");
            return sb.ToString();
        }
    }
}
