using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateRegelingC(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* Compileerbestand */");
            sb.AppendLine("/* ---------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(c.Data, "regeling.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(c.Data));
            sb.AppendLine();
            sb.AppendLine("/* Functionaliteiten regeling:");
            sb.AppendLine($"{ts}RIS: {(c.RISData.RISToepassen ? "ja" : "nee")}");
            sb.AppendLine($"{ts}PTP: {(c.PTPData.PTPKoppelingen.Any() ? "ja" : "nee")}");
            sb.AppendLine($"{ts}Intergroen: {(c.Data.Intergroen ? "ja" : "nee")}");
            sb.AppendLine("*/");
            sb.AppendLine();
            sb.AppendLine("#ifndef AUTOMAAT");
            sb.AppendLine($"{ts}#pragma region CCOL library dependencies");
            sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"ccolreg.lib\")");
            sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"ccolsim.lib\")");
            sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"lwmlfunc.lib\")");
            sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"stdfunc.lib\")");
            if (c.Data.CCOLVersie <= CCOLVersieEnum.CCOL8 && c.Data.VLOGType != VLOGTypeEnum.Geen ||
                c.Data.CCOLVersie >= CCOLVersieEnum.CCOL9 && c.Data.VLOGSettings.VLOGToepassen)
            {
                sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"ccolvlog.lib\")");
            }
            if (c.HasDSI()) sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"dsifunc.lib\")");
            if (c.PTPData.PTPKoppelingen.Any())
            {
                sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"ccolks.lib\")");
            }
            sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"ccolmain.lib\")");
            if (c.HalfstarData.IsHalfstar)
            {
                sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"plfunc.lib\")");
                sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"plefunc.lib\")");
                sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"tx_synch.lib\")");
                sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"{(c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && c.Data.Intergroen ? "trigfunc.lib" : "tigfunc.lib")}\")");
            }
            if (c.RISData.RISToepassen)
            {
                sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"risfunc.lib\")");
                sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"rissimfunc.lib\")");
                sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"comctl32.lib\")");
                if (c.Data.CCOLVersie > CCOLVersieEnum.CCOL8)
                {
                    sb.AppendLine($"{ts}{ts}#pragma comment(lib, \"htmlhelp.lib\")");
                }
                sb.AppendLine($"{ts}{ts}/* Voor Visual 2017 en hoger: haal onderstaande regel uit het commentaar */");
                sb.AppendLine($"{ts}{ts}/* #pragma comment(lib, \"legacy_stdio_definitions.lib\") */");
            }
            sb.AppendLine($"{ts}#pragma endregion");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("#ifndef CCOLTIG     /* Intergroentijden worden toegepast */");
            sb.AppendLine($"{ts}#define CCOLTIG");
            sb.AppendLine("#endif");
            sb.AppendLine();
            if (c.PTPData.PTPKoppelingen.Any())
            {
                sb.AppendLine($"#include \"PTPWIN.C\"");
            }
            sb.AppendLine($"#include \"{c.Data.Naam}sys.h\"");
            sb.AppendLine($"#include \"{c.Data.Naam}reg.c\"");
            if (c.HasPTorHD())
            {
                sb.AppendLine($"#include \"{c.Data.Naam}prio.c\"");
            }
            sb.AppendLine($"#include \"{c.Data.Naam}tab.c\"");
            if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.SyncFunc &&
                (c.InterSignaalGroep.Gelijkstarten.Any() || c.InterSignaalGroep.Voorstarten.Any()))
            {
                sb.AppendLine("#include \"syncfunc.c\"");
            }
            sb.AppendLine("#ifndef AUTOMAAT");
            if (!c.Data.NietGebruikenBitmap)
            {
                sb.AppendLine($"{ts}#include \"{c.Data.Naam}dpl.c\"");
            }
            sb.AppendLine($"{ts}#include \"{c.Data.Naam}sim.c\"");
            if (c.RISData.RISToepassen)
            {
                sb.AppendLine($"{ts}#include \"{c.Data.Naam}rissim.c\"");
            }

            sb.AppendLine("#endif");

            return sb.ToString();
        }
    }
}
