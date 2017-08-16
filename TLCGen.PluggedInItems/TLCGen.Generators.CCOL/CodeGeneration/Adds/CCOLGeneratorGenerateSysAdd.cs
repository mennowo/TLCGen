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
        private string GenerateSysAdd(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* DEFINITIE BESTAND, GEBRUIKERS TOEVOEGINGEN          */");
            sb.AppendLine("/* (gegenereerde headers niet wijzigen of verwijderen) */");
            sb.AppendLine("/* --------------------------------------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "sys.add"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.AppendLine("#define FCMAX  (FCMAX1+0)  /* Totaal aantal gebruikte fasecycli     */");
            sb.AppendLine("#define USMAX  (USMAX1+0)  /* Totaal aantal gebruikte uitgangen     */");
            sb.AppendLine("#define DPMAX  (DPMAX1+0)  /* Totaal aantal gebruikte detectoren    */");
            sb.AppendLine("#define ISMAX  (ISMAX1+0)  /* Totaal aantal gebruikte ingangen      */");
            sb.AppendLine("#define HEMAX  (HEMAX1+0)  /* Totaal aantal gebruikte hulpelementen */");
            sb.AppendLine("#define MEMAX  (MEMAX1+0)  /* Totaal aantal gebruikte mem.elementen */");
            sb.AppendLine("#define TMMAX  (TMMAX1+0)  /* Totaal aantal gebruikte timers        */");
            sb.AppendLine("#define CTMAX  (CTMAX1+0)  /* Totaal aantal gebruikte tellers       */");
            sb.AppendLine("#define SCHMAX (SCHMAX1+0) /* Totaal aantal gebruikte schakelaars   */");
            sb.AppendLine("#define PRMMAX (PRMMAX1+0) /* Totaal aantal gebruikte parameters    */");
            sb.AppendLine("#define MLMAX  (MLMAX1+0)  /* Totaal aantal gebruikte modulen       */");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
