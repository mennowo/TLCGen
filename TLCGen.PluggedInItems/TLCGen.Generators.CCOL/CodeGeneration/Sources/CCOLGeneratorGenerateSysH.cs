using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateSysH(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* ALGEMENE APPLICATIEFILE */");
            sb.AppendLine("/* ----------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(c.Data, "sys.h"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(c.Data));
            sb.AppendLine();
            sb.AppendLine($"#define SYSTEM \"{c.Data.Naam}\"");
            if (c.Data.AanmakenVerionSysh)
            {
                var ver = c.Data.Versies.LastOrDefault();
                if (ver!= null)
                {
                    sb.AppendLine($"#define VERSION \"{ver.Versie} {ver.Datum:yyyyMMdd}\"");
                }
            }
            sb.AppendLine();
            sb.Append(GenerateSysHFasen(c));
            sb.AppendLine();
            sb.Append(GenerateSysHUitgangen(c));
            sb.AppendLine();
            sb.Append(GenerateSysHDetectors(c));
            sb.AppendLine();
            sb.Append(GenerateSysHIngangen(c));
            sb.AppendLine();
            sb.Append(GenerateSysHHulpElementen(c));
            sb.AppendLine();
            sb.Append(GenerateSysHGeheugenElementen(c));
            sb.AppendLine();
            sb.Append(GenerateSysHTijdElementen(c));
            sb.AppendLine();
            sb.Append(GenerateSysHCounters(c));
            sb.AppendLine();
            sb.Append(GenerateSysHSchakelaars(c));
            sb.AppendLine();
            sb.Append(GenerateSysHParameters(c));
            sb.AppendLine();
            if (c.HasDSI())
            {
                sb.Append(GenerateSysHDS(c));
                sb.AppendLine();
            }
            var ov = 0;
            if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.GeneriekePrioriteit)
            {
                foreach (var ovFC in c.PrioData.PrioIngrepen)
                {
                    sb.AppendLine($"{ts}#define prioFC{ovFC.FaseCyclus}{ovFC.Naam} {ov.ToString()}");
                    ++ov;
                }
                foreach (var hdFC in c.PrioData.HDIngrepen)
                {
                    sb.AppendLine($"{ts}#define hdFC{hdFC.FaseCyclus} {ov.ToString()}");
                    ++ov;
                }
                sb.AppendLine($"{ts}#define prioFCMAX {ov.ToString()}");
                sb.AppendLine();
            }
            
            sb.AppendLine("/* modulen */");
            sb.AppendLine("/* ------- */");
            if (!c.Data.MultiModuleReeksen)
            {
                sb.AppendLine($"{ts}#define MLMAX1 {c.ModuleMolen.Modules.Count} /* aantal modulen */");
            }
            else
            {
                foreach (var r in c.MultiModuleMolens)
                {
                    sb.AppendLine($"{ts}#define {r.Reeks}MAX1 {r.Modules.Count} /* aantal modulen reeks {r.Reeks} */");
                }
            }
	        sb.AppendLine();
	        if (c.HalfstarData.IsHalfstar)
	        {
		        sb.AppendLine("/* signaalplannen */");
		        sb.AppendLine("/* -------------- */");
		        sb.AppendLine($"{ts}#define PLMAX1 {c.HalfstarData.SignaalPlannen.Count} /* aantal signaalplannen */");
		        sb.AppendLine();
	        }
            if (c.StarData.ToepassenStar)
            {
                sb.AppendLine("/* starre programma's */");
                sb.AppendLine("/* ------------------ */");
                var pr = 0;
                foreach (var programma in c.StarData.Programmas)
                {
                    sb.AppendLine($"{ts}#define STAR{pr + 1} {pr} /* programma {programma.Naam} */");
                    ++pr;
                }
                sb.AppendLine($"{ts}#define STARMAX {c.StarData.Programmas.Count} /* aantal starre programmas */");
                sb.AppendLine();
            }
	        sb.AppendLine("/* Aantal perioden voor max groen */");
            sb.AppendLine("/* ------- */");
			// Here: +1 to allow room for default period in arrays made with this value
            sb.AppendLine($"{ts}#define MPERIODMAX {c.PeriodenData.Perioden.Count(x => x.Type == PeriodeTypeEnum.Groentijden) + 1} /* aantal groenperioden */");
            sb.AppendLine();

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.SysHBeforeUserDefines])
            {
                sb.Append(gen.Value.GetCode(c, CCOLCodeTypeEnum.SysHBeforeUserDefines, ts));
            }

            sb.AppendLine("/* Gebruikers toevoegingen file includen */");
            sb.AppendLine("/* ------------------------------------- */");
            sb.AppendLine($"{ts}#include \"{c.Data.Naam}sys.add\"");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateSysHFasen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* fasecycli */");
            sb.AppendLine("/* --------- */");

            var pad1 = "FCMAX".Length;
            foreach (var fcm in controller.Fasen)
            {
                if (fcm.GetDefine().Length > pad1) pad1 = fcm.GetDefine().Length;
            }
            pad1 = pad1 + $"{ts}#define  ".Length;

            var pad2 = controller.Fasen.Count.ToString().Length;

            var index = 0;
            foreach (var fcm in controller.Fasen)
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
            var sb = new StringBuilder();

            sb.AppendLine("/* overige uitgangen */");
            sb.AppendLine("/* ----------------- */");

            sb.Append(GetAllElementsSysHLines(_uitgangen, "FCMAX"));

            return sb.ToString();
        }

        private string GenerateSysHDetectors(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* detectie */");
            sb.AppendLine("/* -------- */");

            var pad1 = "ISMAX".Length;
            if(controller.Fasen.Any() && controller.Fasen.SelectMany(x => x.Detectoren).Any())
            {
                pad1 = controller.Fasen.SelectMany(x => x.Detectoren).Max(x => x.GetDefine().Length);
            }
            if(controller.Detectoren.Any())
            {
                var _pad1 = controller.Detectoren.Max(x => x.GetDefine().Length);
                pad1 = _pad1 > pad1 ? _pad1 : pad1;
            }
            if (controller.SelectieveDetectoren.Any())
            {
                var _pad1 = controller.SelectieveDetectoren.Max(x => x.GetDefine().Length);
                pad1 = _pad1 > pad1 ? _pad1 : pad1;
            }
            var ovdummies = controller.PrioData.GetAllDummyDetectors();
            if (ovdummies.Any())
            {
                pad1 = ovdummies.Max(x => x.GetDefine().Length);
            }
            pad1 = pad1 + $"{ts}#define  ".Length;

            var pad2 = controller.Fasen.Count.ToString().Length;

            var index = 0;
            foreach (var dm in controller.GetAllDetectors())
            {
                if (dm.Dummy) continue;
                sb.Append($"{ts}#define {dm.GetDefine()} ".PadRight(pad1));
                sb.AppendLine($"{index.ToString()}".PadLeft(pad2));
                ++index;
            }

            var autom_index = index;

            /* Dummies */
            if (controller.Fasen.Any() && controller.Fasen.SelectMany(x => x.Detectoren).Any(x => x.Dummy) ||
                controller.Detectoren.Any() && controller.Detectoren.Any(x => x.Dummy) ||
                ovdummies.Any())
            {
                sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST");
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
            var sb = new StringBuilder();

            sb.AppendLine("/* overige ingangen */");
            sb.AppendLine("/* ---------------- */");

            sb.Append(GetAllElementsSysHLines(_ingangen, "DPMAX"));

            return sb.ToString();
        }

        private string GenerateSysHHulpElementen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* hulp elementen */");
            sb.AppendLine("/* -------------- */");

            sb.Append(GetAllElementsSysHLines(_hulpElementen));

            return sb.ToString();
        }

        private string GenerateSysHGeheugenElementen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* geheugen elementen */");
            sb.AppendLine("/* ------------------ */");

            sb.Append(GetAllElementsSysHLines(_geheugenElementen));

            return sb.ToString();
        }

        private string GenerateSysHTijdElementen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* tijd elementen */");
            sb.AppendLine("/* -------------- */");

            sb.Append(GetAllElementsSysHLines(_timers));

            return sb.ToString();
        }
        
        private string GenerateSysHCounters(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* teller elementen */");
            sb.AppendLine("/* ---------------- */");

            sb.Append(GetAllElementsSysHLines(_counters));

            return sb.ToString();
        }

        private string GenerateSysHSchakelaars(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* schakelaars */");
            sb.AppendLine("/* ----------- */");

            sb.Append(GetAllElementsSysHLines(_schakelaars));

            return sb.ToString();
        }

        private string GenerateSysHParameters(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* parameters */");
            sb.AppendLine("/* ---------- */");

            sb.Append(GetAllElementsSysHLines(_parameters));

            return sb.ToString();
        }

        private string GenerateSysHDS(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* Selectieve detectie */");
            sb.AppendLine("/* ------------------- */");

            var index = 0;
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
                    sb.AppendLine($"{ts}#define {(_dpf + d.Naam).ToUpper()} {index++}{(!string.IsNullOrWhiteSpace(d.Omschrijving) ? " /* " + d.Omschrijving + "*/" : "")}");
                }
            }
            sb.AppendLine($"{ts}#define DSMAX    {index}");

            return sb.ToString();
        }
    }
}
