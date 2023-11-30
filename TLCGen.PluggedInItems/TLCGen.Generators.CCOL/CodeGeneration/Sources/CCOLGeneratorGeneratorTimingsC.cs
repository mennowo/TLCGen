using System;
using System.Linq;
using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateFcTimingsC(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* DEFINITIE FCTMING FUNCTIES */");
            sb.AppendLine("/* -------------------------- */");
            sb.AppendLine();
            sb.Append(CCOLHeaderGenerator.GenerateFileHeader(c.Data, "fctimings.c"));
            sb.AppendLine();
            sb.Append(CCOLHeaderGenerator.GenerateVersionHeader(c.Data));
            sb.AppendLine();
            sb.AppendLine("/* DEFINITIE EVENTSTATE */");
            sb.AppendLine("/* ==================== */");
            sb.AppendLine();
            sb.AppendLine("#ifdef EVENTSTATE_MACRODEFINITIES_CIF_INC");
            sb.AppendLine();
            sb.AppendLine("/* Macrodefinities status EVENTSTATE (Nederlands) */");
            sb.AppendLine("/* ---------------------------------------------- */");
            sb.AppendLine("#define CIF_TIMING_ONBEKEND           0    /* Unknown(0)                             */");
            sb.AppendLine("#define CIF_TIMING_GEDOOFD            1    /* Dark(1)                                */");
            sb.AppendLine("#define CIF_TIMING_ROOD_KNIPPEREN     2    /* stop - Then - Proceed(2)               */");
            sb.AppendLine("#define CIF_TIMING_ROOD               3    /* stop - And - Remain(3)                 */");
            sb.AppendLine("#define CIF_TIMING_GROEN_OVERGANG     4    /* pre - Movement(4) - not used in NL     */");
            sb.AppendLine("#define CIF_TIMING_GROEN_DEELCONFLICT 5    /* permissive - Movement - Allowed(5)     */");
            sb.AppendLine("#define CIF_TIMING_GROEN              6    /* protected - Movement - Allowed(6)      */");
            sb.AppendLine("#define CIF_TIMING_GEEL_DEELCONFLICT  7    /* permissive - clearance(7)              */");
            sb.AppendLine("#define CIF_TIMING_GEEL               8    /* protected - clearance(8)               */");
            sb.AppendLine("#define CIF_TIMING_GEEL_KNIPPEREN     9    /* caution - Conflicting - Traffic(9)     */");
            sb.AppendLine("#define CIF_TIMING_GROEN_KNIPPEREN_DEELCONFLICT 10    /* permissive - Movement - PreClearance - not in J2735 */");
            sb.AppendLine("#define CIF_TIMING_GROEN_KNIPPEREN              11    /* protected -  Movement - PreClearance - not in J2735 */");
            sb.AppendLine();
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine($"/* De functie kr52_Eventstate_Definition() definieert de eventstate voor de fasecycli.");
            sb.AppendLine($" * De functie kr52_Eventstate_Definition() wordt aangeroepn door de functie control_parameters().");
            sb.AppendLine($" */");
            sb.AppendLine($"void Timings_Eventstate_Definition(void)");
            sb.AppendLine($"{{");
            sb.AppendLine($"{ts}register count i;");
            sb.AppendLine();
            sb.AppendLine($"{ts}/* Zet defaultwaarde */");
            sb.AppendLine($"{ts}/* ----------------- */");
            sb.AppendLine($"{ts}for (i = 0; i < FCMAX; i++)");
            sb.AppendLine($"{ts}{{");
            sb.AppendLine($"{ts}{ts}CCOL_FC_EVENTSTATE[i][CIF_ROOD]= CIF_TIMING_ONBEKEND;       /* Rood   */");
            sb.AppendLine($"{ts}{ts}CCOL_FC_EVENTSTATE[i][CIF_GROEN]= CIF_TIMING_ONBEKEND;      /* Groen  */");
            sb.AppendLine($"{ts}{ts}CCOL_FC_EVENTSTATE[i][CIF_GEEL]= CIF_TIMING_ONBEKEND;       /* Geel   */");
            sb.AppendLine($"{ts}}}");
            sb.AppendLine();
            foreach(var fc in c.TimingsData.TimingsFasen)
            {
                sb.AppendLine($"/* Fase {fc.FaseCyclus} */");
                var fcfc = c.Fasen.FirstOrDefault(x => x.Naam == fc.FaseCyclus);
                switch (fc.ConflictType)
                {
                    case TimingsFaseCyclusTypeEnum.Conflictvrij:
                        sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_ROOD]= CIF_TIMING_ROOD;       /* Rood   */");
                        sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_GROEN]= CIF_TIMING_GROEN;      /* Groen  */");
                        if (fcfc != null)
                        {
                            sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_GEEL]= CIF_TIMING_GEEL;       /* Geel   */");
                        }
                        break;
                    case TimingsFaseCyclusTypeEnum.Deelconflict:
                        sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_ROOD]= CIF_TIMING_ROOD;       /* Rood   */");
                        sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_GROEN]= CIF_TIMING_GROEN_DEELCONFLICT;      /* Groen  */");
                        if (fcfc != null)
                        {
                            sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_GEEL]= CIF_TIMING_GEEL_DEELCONFLICT;       /* Geel   */");
                        }
                        break;
                    case TimingsFaseCyclusTypeEnum.Onbekend:
                        sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_ROOD]= CIF_TIMING_ONBEKEND;       /* Rood   */");
                        sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_GROEN]= CIF_TIMING_ONBEKEND;      /* Groen  */");
                        if (fcfc != null)
                        {
                            sb.AppendLine($"{ts}CCOL_FC_EVENTSTATE[{_fcpf}{fc.FaseCyclus}][CIF_GEEL]= CIF_TIMING_ONBEKEND;       /* Geel   */");
                        }
                        break;
                }
                sb.AppendLine();
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}