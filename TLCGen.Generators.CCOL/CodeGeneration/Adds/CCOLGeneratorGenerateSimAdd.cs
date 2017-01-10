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
        private string GenerateSimAdd(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* DISPLAY BESTAND, GEBRUIKERS TOEVOEGINGEN            */");
            sb.AppendLine("/* (gegenereerde headers niet wijzigen of verwijderen) */");
            sb.AppendLine("/* --------------------------------------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "sim.add"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.AppendLine("/* dit is een placeholder */");
            sb.AppendLine("/* ---------------------- */");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
