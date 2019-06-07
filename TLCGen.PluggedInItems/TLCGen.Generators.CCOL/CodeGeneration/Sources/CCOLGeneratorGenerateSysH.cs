﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateSysH(ControllerModel c)
        {
            StringBuilder sb = new StringBuilder();
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
                    sb.AppendLine($"#define VERSION \"{ver.Versie} {ver.Datum.ToString("yyyyMMdd")}\"");
                }
            }
            sb.AppendLine();
            sb.AppendLine($"#include \"booldef.h\"       /* bool typedef                 */");
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
            if (c.OVData.OVIngreepType == OVIngreepTypeEnum.Uitgebreid)
            {
                var ov = 0;
                foreach (var ovFC in c.OVData.OVIngrepen)
                {
                    sb.AppendLine($"{ts}#define ovFC{ovFC.FaseCyclus} {ov.ToString()}");
                    ++ov;
                }
                foreach (var hdFC in c.OVData.HDIngrepen)
                {
                    sb.AppendLine($"{ts}#define hdFC{hdFC.FaseCyclus} {ov.ToString()}");
                    ++ov;
                }
                sb.AppendLine($"{ts}#define ovOVMAX {ov.ToString()}");
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
		        sb.AppendLine("/* signaalplannen*/");
		        sb.AppendLine("/* -------------- */");
		        sb.AppendLine($"{ts}#define PLMAX1 {c.HalfstarData.SignaalPlannen.Count} /* aantal signaalplannen */");
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
                
                foreach (var d in controller.SelectieveDetectoren.Where(x => !x.Dummy))
                {
                    sb.AppendLine($"{ts}#define {(_dpf + d.Naam).ToUpper()} {index++}{(!string.IsNullOrWhiteSpace(d.Omschrijving) ? " /* " + d.Omschrijving + "*/" : "")}");
                }
            }
            if (controller.SelectieveDetectoren.Any(x => x.Dummy))
            {
                var rindex = index;
                sb.AppendLine($"{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM");
                foreach (var d in controller.SelectieveDetectoren.Where(x => x.Dummy))
                {
                    sb.AppendLine($"{ts}#define {(_dpf + d.Naam).ToUpper()} {index++}{(!string.IsNullOrWhiteSpace(d.Omschrijving) ? " /* " + d.Omschrijving + "*/" : "")}");
                }
                sb.AppendLine($"{ts}#else");
                sb.AppendLine($"{ts}#define DSMAX    {rindex}");
                sb.AppendLine($"{ts}#endif");
            }
            else
            {
                sb.AppendLine($"{ts}#define DSMAX    {index}");
            }

            return sb.ToString();
        }
    }
}
