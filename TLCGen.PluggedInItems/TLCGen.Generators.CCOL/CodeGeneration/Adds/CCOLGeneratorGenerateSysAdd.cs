using System.Linq;
using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateSysAddHeader(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine(_beginGeneratedHeader);
            sb.AppendLine("/* DEFINITIE BESTAND, GEBRUIKERS TOEVOEGINGEN          */");
            sb.AppendLine("/* --------------------------------------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(c.Data, "sys.add"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(c.Data));
            sb.AppendLine(_endGeneratedHeader);

            return sb.ToString();
        }

        private string GenerateSysAdd(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine(GenerateSysAddHeader(c));
            sb.AppendLine();
            sb.AppendLine("#define FCMAX  (FCMAX1+0)  /* Totaal aantal gebruikte fasecycli      */");
            sb.AppendLine("#define USMAX  (USMAX1+0)  /* Totaal aantal gebruikte uitgangen      */");
            sb.AppendLine("#define DPMAX  (DPMAX1+0)  /* Totaal aantal gebruikte detectoren     */");
            sb.AppendLine("#define ISMAX  (ISMAX1+0)  /* Totaal aantal gebruikte ingangen       */");
            sb.AppendLine("#define HEMAX  (HEMAX1+0)  /* Totaal aantal gebruikte hulpelementen  */");
            sb.AppendLine("#define MEMAX  (MEMAX1+0)  /* Totaal aantal gebruikte mem.elementen  */");
            sb.AppendLine("#define TMMAX  (TMMAX1+0)  /* Totaal aantal gebruikte timers         */");
            sb.AppendLine("#define CTMAX  (CTMAX1+0)  /* Totaal aantal gebruikte tellers        */");
            sb.AppendLine("#define SCHMAX (SCHMAX1+0) /* Totaal aantal gebruikte schakelaars    */");
            sb.AppendLine("#define PRMMAX (PRMMAX1+0) /* Totaal aantal gebruikte parameters     */");
            sb.AppendLine("#define MLMAX  (MLMAX1+0)  /* Totaal aantal gebruikte modulen        */");
            foreach (var r in c.MultiModuleMolens)
            {
                if (r.Modules.Any())
                {
                    sb.AppendLine($"#define {r.Reeks}MAX  ({r.Reeks}MAX1+0)  /* Totaal aantal gebruikte modulen reeks {r.Reeks} */");
                }
            }
            if (c.HalfstarData.IsHalfstar)
	        {
				sb.AppendLine("#define PLMAX  (PLMAX1+0)  /* Totaal aantal gebruikte signaalplannen */");
	        }
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
