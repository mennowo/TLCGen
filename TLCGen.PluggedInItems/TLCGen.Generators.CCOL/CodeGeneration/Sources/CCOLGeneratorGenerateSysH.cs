using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateSysH(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* ALGEMENE APPLICATIEFILE */");
            sb.AppendLine("/* ----------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "sys.h"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.AppendLine($"#define SYSTEM \"{controller.Data.Naam}\"");
            sb.AppendLine();
            sb.Append(GenerateSysHFasen(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHUitgangen(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHDetectors(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHIngangen(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHHulpElementen(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHGeheugenElementen(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHTijdElementen(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHCounters(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHSchakelaars(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHParameters(controller));
            sb.AppendLine();
            if (controller.HasDSI())
            {
                sb.Append(GenerateSysHDS(controller));
            }
            
            sb.AppendLine();
            sb.AppendLine("/* modulen */");
            sb.AppendLine("/* ------- */");
            sb.AppendLine($"{ts}#define MLMAX1 {controller.ModuleMolen.Modules.Count} /* aantal modulen */");
	        sb.AppendLine();
	        if (controller.HalfstarData.IsHalfstar)
	        {
		        sb.AppendLine("/* signaalplannen*/");
		        sb.AppendLine("/* -------------- */");
		        sb.AppendLine($"{ts}#define PLMAX1 {controller.HalfstarData.SignaalPlannen.Count} /* aantal signaalplannen */");
		        sb.AppendLine();
	        }
	        sb.AppendLine("/* Aantal perioden voor max groen */");
            sb.AppendLine("/* ------- */");
			// Here: +1 to allow room for default period in arrays made with this value
            sb.AppendLine($"{ts}#define MPERIODMAX {controller.PeriodenData.Perioden.Count(x => x.Type == PeriodeTypeEnum.Groentijden) + 1} /* aantal groenperioden */");
            sb.AppendLine();
            sb.AppendLine("/* Gebruikers toevoegingen file includen */");
            sb.AppendLine("/* ------------------------------------- */");
            sb.AppendLine($"{ts}#include \"{controller.Data.Naam}sys.add\"");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateSysHFasen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* fasecycli */");
            sb.AppendLine("/* --------- */");

            int pad1 = "FCMAX".Length;
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.GetDefine().Length > pad1) pad1 = fcm.GetDefine().Length;
            }
            pad1 = pad1 + $"{ts}#define  ".Length;

            int pad2 = controller.Fasen.Count.ToString().Length;

            int index = 0;
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                sb.Append($"{ts}#define {fcm.GetDefine()} ".PadRight(pad1));
                sb.AppendLine($"{index.ToString()}".PadLeft(pad2));
                ++index;
            }
            sb.Append($"{ts}#define FCMAX1 ".PadRight(pad1));
            sb.Append($"{index.ToString()} ".PadLeft(pad2));
            sb.AppendLine("/* aantal fasecycli */");

            return sb.ToString();
        }

        private string GenerateSysHUitgangen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* overige uitgangen */");
            sb.AppendLine("/* ----------------- */");

            sb.Append(GetAllElementsSysHLines(_uitgangen, "FCMAX"));

            return sb.ToString();
        }

        private string GenerateSysHDetectors(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* detectie */");
            sb.AppendLine("/* -------- */");

            int pad1 = "ISMAX".Length;
            if(controller.Fasen.Any() && controller.Fasen.SelectMany(x => x.Detectoren).Any())
            {
                pad1 = controller.Fasen.SelectMany(x => x.Detectoren).Max(x => x.GetDefine().Length);
            }
            if(controller.Detectoren.Any())
            {
                int _pad1 = controller.Detectoren.Max(x => x.GetDefine().Length);
                pad1 = _pad1 > pad1 ? _pad1 : pad1;
            }
            if (controller.SelectieveDetectoren.Any())
            {
                int _pad1 = controller.SelectieveDetectoren.Max(x => x.GetDefine().Length);
                pad1 = _pad1 > pad1 ? _pad1 : pad1;
            }
            var ovdummies = controller.OVData.GetAllDummyDetectors();
            if (ovdummies.Any())
            {
                pad1 = ovdummies.Max(x => x.GetDefine().Length);
            }
            pad1 = pad1 + $"{ts}#define  ".Length;

            int pad2 = controller.Fasen.Count.ToString().Length;

            int index = 0;
            foreach (var dm in controller.GetAllDetectors())
            {
                if (!dm.Dummy)
                {
                    sb.Append($"{ts}#define {dm.GetDefine()} ".PadRight(pad1));
                    sb.AppendLine($"{index.ToString()}".PadLeft(pad2));
                    ++index;
                }
            }

            int autom_index = index;

            /* Dummies */
            if (controller.Fasen.Any() && controller.Fasen.SelectMany(x => x.Detectoren).Where(x => x.Dummy).Any() ||
                controller.Detectoren.Any() && controller.Detectoren.Where(x => x.Dummy).Any() ||
                ovdummies.Any())
            {
                sb.AppendLine("#ifndef AUTOMAAT");
                foreach (var dm in controller.GetAllDetectors(x => x.Dummy))
                {
                    sb.Append($"{ts}#define {dm.GetDefine()} ".PadRight(pad1));
                    sb.AppendLine($"{index.ToString()}".PadLeft(pad2));
                    ++index;
                }
                foreach(var dm in ovdummies)
                {
                    sb.Append($"{ts}#define {dm.GetDefine()} ".PadRight(pad1));
                    sb.AppendLine($"{index.ToString()}".PadLeft(pad2));
                    ++index;
                }
                sb.Append($"{ts}#define DPMAX1 ".PadRight(pad1));
                sb.Append($"{index.ToString()} ".PadLeft(pad2));
                sb.AppendLine("/* aantal detectoren testomgeving */");
                sb.AppendLine("#else");
                sb.Append($"{ts}#define DPMAX1 ".PadRight(pad1));
                sb.Append($"{autom_index.ToString()} ".PadLeft(pad2));
                sb.AppendLine("/* aantal detectoren automaat omgeving */");
                sb.AppendLine("#endif");
            }
            else
            {
                sb.Append($"{ts}#define DPMAX1 ".PadRight(pad1));
                sb.Append($"{index.ToString()} ".PadLeft(pad2));
                sb.AppendLine("/* aantal detectoren */");
            }

            return sb.ToString();
        }

        private string GenerateSysHIngangen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* overige ingangen */");
            sb.AppendLine("/* ---------------- */");

            sb.Append(GetAllElementsSysHLines(_ingangen, "DPMAX"));

            return sb.ToString();
        }

        private string GenerateSysHHulpElementen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* hulp elementen */");
            sb.AppendLine("/* -------------- */");

            sb.Append(GetAllElementsSysHLines(_hulpElementen));

            return sb.ToString();
        }

        private string GenerateSysHGeheugenElementen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* geheugen elementen */");
            sb.AppendLine("/* ------------------ */");

            sb.Append(GetAllElementsSysHLines(_geheugenElementen));

            return sb.ToString();
        }

        private string GenerateSysHTijdElementen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* tijd elementen */");
            sb.AppendLine("/* -------------- */");

            sb.Append(GetAllElementsSysHLines(_timers));

            return sb.ToString();
        }
        
        private string GenerateSysHCounters(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* teller elementen */");
            sb.AppendLine("/* ---------------- */");

            sb.Append(GetAllElementsSysHLines(_counters));

            return sb.ToString();
        }

        private string GenerateSysHSchakelaars(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* schakelaars */");
            sb.AppendLine("/* ----------- */");

            sb.Append(GetAllElementsSysHLines(_schakelaars));

            return sb.ToString();
        }

        private string GenerateSysHParameters(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* parameters */");
            sb.AppendLine("/* ---------- */");

            sb.Append(GetAllElementsSysHLines(_parameters));

            return sb.ToString();
        }

        private string GenerateSysHDS(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* Selectieve detectie */");
            sb.AppendLine("/* ------------------- */");

            int index = 0;
            var isvecom = controller.SelectieveDetectoren.Any();
            // Geen VECOM? Dan alleen een dummy lus tbv KAR
            if (!isvecom)
            {
                sb.AppendLine($"{ts}#define dsdummy 0 /* Dummy SD lus 0: tbv KAR */");
                ++index;
            }
            // Anders ook een dummy lus, voor KAR en zodat VECOM begint op 1
            else
            {
                sb.AppendLine($"{ts}#define dsdummy 0 /* Dummy SD lus 0: tbv KAR & VECOM start op 1 */");
                ++index;
                
                foreach (var d in controller.SelectieveDetectoren)
                {
                    sb.AppendLine($"{ts}#define {(_dpf + d.Naam).ToUpper()} {index++}");
                }
            }
            sb.AppendLine($"{ts}#define DSMAX    {index}");

            return sb.ToString();
        }
    }
}
