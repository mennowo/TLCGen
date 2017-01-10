using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateDplAdd(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* DISPLAY BESTAND, GEBRUIKERS TOEVOEGINGEN            */");
            sb.AppendLine("/* (gegenereerde headers niet wijzigen of verwijderen) */");
            sb.AppendLine("/* --------------------------------------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "dpl.add"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
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
