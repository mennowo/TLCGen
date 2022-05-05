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
            if (c.Data.TVGAMaxAlsDefaultGroentijdSet)
            {
                sb.AppendLine("#define TVGAMAX /* gebruik van TVGA_max[] */");
            }

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.SysHDefines, true, true, false, false);
            sb.AppendLine();
            
            sb.Append(GenerateSysHFasen(c));
            sb.AppendLine();
            sb.Append(GenerateSysHUitgangen(c));
            sb.AppendLine();
            sb.Append(GenerateSysHDetectors(c));
            sb.AppendLine();
            sb.Append(GenerateSysHIngangen(c));
            sb.AppendLine();
            sb.Append(GenerateSysHHulpElementen());
            sb.AppendLine();
            sb.Append(GenerateSysHGeheugenElementen());
            sb.AppendLine();
            sb.Append(GenerateSysHTijdElementen());
            sb.AppendLine();
            sb.Append(GenerateSysHCounters());
            sb.AppendLine();
            sb.Append(GenerateSysHSchakelaars());
            sb.AppendLine();
            sb.Append(GenerateSysHParameters());
            sb.AppendLine();
            if (c.HasDSI())
            {
                sb.Append(GenerateSysHds(c));
                sb.AppendLine();
            }
            var ov = 0;
            if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.GeneriekePrioriteit)
            {
                foreach (var ovFC in c.PrioData.PrioIngrepen)
                {
                    sb.AppendLine($"{ts}#define prioFC{CCOLCodeHelper.GetPriorityName(c, ovFC)} {ov}");
                    ++ov;
                }
                foreach (var hdFC in c.PrioData.HDIngrepen)
                {
                    sb.AppendLine($"{ts}#define hdFC{hdFC.FaseCyclus} {ov}");
                    ++ov;
                }
                    sb.AppendLine($"{ts}#define prioFCMAX {ov}");
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

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.SysHBeforeUserDefines, true, true, false, true);

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
            pad1 += $"{ts}#define  ".Length;

            var pad2 = controller.Fasen.Count.ToString().Length;

            var index = 0;
            var fasen = controller.Data.RangeerData.RangerenFasen ? controller.Fasen.OrderBy(x => x.RangeerIndex).ToList() : controller.Fasen;
            foreach (var fcm in fasen)
            {
                sb.Append($"{ts}#define {fcm.GetDefine()} ".PadRight(pad1));
                sb.AppendLine($"{index}".PadLeft(pad2));
                ++index;
            }
            sb.Append($"{ts}#define FCMAX1 ".PadRight(pad1));
            sb.Append($"{index} ".PadLeft(pad2));
            sb.AppendLine("/* aantal fasecycli */");

            return sb.ToString();
        }

        private string GenerateSysHUitgangen(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* overige uitgangen */");
            sb.AppendLine("/* ----------------- */");

            if (c.Data.RangeerData.RangerenUitgangen)
            {
                foreach (var u in _uitgangen.Elements)
                {
                    var m = c.Data.RangeerData.RangeerUitgangen.FirstOrDefault(x => x.Naam == u.Naam);
                    if (m != null) u.RangeerIndex = m.RangeerIndex;
                    else
                    {
                        int i = -0;
                        ++i;
                    }
                }
            }

            sb.Append(GetAllElementsSysHLines(_uitgangen, "FCMAX", useRangering: c.Data.RangeerData.RangerenUitgangen));

            return sb.ToString();
        }

        private string GenerateSysHDetectors(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* detectie */");
            sb.AppendLine("/* -------- */");

            var pad1 = "ISMAX".Length;
            if(c.Fasen.Any() && c.Fasen.SelectMany(x => x.Detectoren).Any())
            {
                pad1 = c.Fasen.SelectMany(x => x.Detectoren).Max(x => x.GetDefine().Length);
            }
            if(c.Detectoren.Any())
            {
                var maxPadDet = c.Detectoren.Max(x => x.GetDefine().Length);
                pad1 = maxPadDet > pad1 ? maxPadDet : pad1;
            }
            if (c.SelectieveDetectoren.Any())
            {
                var maxPadSelDet = c.SelectieveDetectoren.Max(x => x.GetDefine().Length);
                pad1 = maxPadSelDet > pad1 ? maxPadSelDet : pad1;
            }
            var ovdummies = c.PrioData.GetAllDummyDetectors();
            if (ovdummies.Any())
            {
                pad1 = ovdummies.Max(x => x.GetDefine().Length);
            }
            pad1 += $"{ts}#define  ".Length;

            var pad2 = c.Fasen.Count.ToString().Length;

            var index = 0;
            var detectors = c.Data.RangeerData.RangerenDetectoren ? c.GetAllDetectors().OrderBy(x => x is SelectieveDetectorModel ? x.RangeerIndex2 : x.RangeerIndex) : c.GetAllDetectors();
            foreach (var dm in detectors)
            {
                if (dm.Dummy) continue;
                sb.Append($"{ts}#define {dm.GetDefine()} ".PadRight(pad1));
                sb.AppendLine($"{index}".PadLeft(pad2));
                ++index;
            }

            var automIndex = index;

            /* Dummies */
            if (c.Fasen.Any() && c.Fasen.SelectMany(x => x.Detectoren).Any(x => x.Dummy) ||
                c.Detectoren.Any() && c.Detectoren.Any(x => x.Dummy) ||
                ovdummies.Any())
            {
                sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST");
                var dummyDetectors = c.Data.RangeerData.RangerenDetectoren ? c.GetAllDetectors(x => x.Dummy).OrderBy(x =>  x is SelectieveDetectorModel ? x.RangeerIndex2 : x.RangeerIndex) : c.GetAllDetectors(x => x.Dummy);
                foreach (var dm in dummyDetectors)
                {
                    sb.Append($"{ts}#define {dm.GetDefine()} ".PadRight(pad1));
                    sb.AppendLine($"{index}".PadLeft(pad2));
                    ++index;
                }
                var dummyOvDetectors = c.Data.RangeerData.RangerenDetectoren ? ovdummies.OrderBy(x => x.RangeerIndex).ToList() : ovdummies;
                foreach(var dm in dummyOvDetectors)
                {
                    sb.Append($"{ts}#define {dm.GetDefine()} ".PadRight(pad1));
                    sb.AppendLine($"{index}".PadLeft(pad2));
                    ++index;
                }
                sb.Append($"{ts}#define DPMAX1 ".PadRight(pad1));
                sb.Append($"{index} ".PadLeft(pad2));
                sb.AppendLine("/* aantal detectoren testomgeving */");
                sb.AppendLine("#else");
                sb.Append($"{ts}#define DPMAX1 ".PadRight(pad1));
                sb.Append($"{automIndex} ".PadLeft(pad2));
                sb.AppendLine("/* aantal detectoren automaat omgeving */");
                sb.AppendLine("#endif");
            }
            else
            {
                sb.Append($"{ts}#define DPMAX1 ".PadRight(pad1));
                sb.Append($"{index} ".PadLeft(pad2));
                sb.AppendLine("/* aantal detectoren */");
            }

            return sb.ToString();
        }

        private string GenerateSysHIngangen(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* overige ingangen */");
            sb.AppendLine("/* ---------------- */");

            if (c.Data.RangeerData.RangerenDetectoren)
            {
                foreach (var i in _ingangen.Elements)
                {
                    var m = c.Data.RangeerData.RangeerIngangen.FirstOrDefault(x => x.Naam == i.Naam);
                    if (m != null) i.RangeerIndex = m.RangeerIndex;
                }
            }

            sb.Append(GetAllElementsSysHLines(_ingangen, "DPMAX", useRangering: c.Data.RangeerData.RangerenDetectoren));

            return sb.ToString();
        }

        private string GenerateSysHHulpElementen()
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* hulp elementen */");
            sb.AppendLine("/* -------------- */");

            sb.Append(GetAllElementsSysHLines(_hulpElementen));

            return sb.ToString();
        }

        private string GenerateSysHGeheugenElementen()
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* geheugen elementen */");
            sb.AppendLine("/* ------------------ */");

            sb.Append(GetAllElementsSysHLines(_geheugenElementen));

            return sb.ToString();
        }

        private string GenerateSysHTijdElementen()
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* tijd elementen */");
            sb.AppendLine("/* -------------- */");

            sb.Append(GetAllElementsSysHLines(_timers));

            return sb.ToString();
        }
        
        private string GenerateSysHCounters()
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* teller elementen */");
            sb.AppendLine("/* ---------------- */");

            sb.Append(GetAllElementsSysHLines(_counters));

            return sb.ToString();
        }

        private string GenerateSysHSchakelaars()
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* schakelaars */");
            sb.AppendLine("/* ----------- */");

            sb.Append(GetAllElementsSysHLines(_schakelaars));

            return sb.ToString();
        }

        private string GenerateSysHParameters()
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* parameters */");
            sb.AppendLine("/* ---------- */");

            sb.Append(GetAllElementsSysHLines(_parameters));

            return sb.ToString();
        }

        private string GenerateSysHds(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* Selectieve detectie */");
            sb.AppendLine("/* ------------------- */");

            var index = 0;
            var isvecom = c.SelectieveDetectoren.Any();
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

                var selDets = c.Data.RangeerData.RangerenSelectieveDetectoren ? c.SelectieveDetectoren.OrderBy(x => x.RangeerIndex).ToList() : c.SelectieveDetectoren;
                foreach (var d in selDets)
                {
                    sb.AppendLine($"{ts}#define {(_dpf + d.Naam).ToUpper()} {index++}{(!string.IsNullOrWhiteSpace(d.Omschrijving) ? " /* " + d.Omschrijving + "*/" : "")}");
                }
            }
            sb.AppendLine($"{ts}#define DSMAX    {index}");

            return sb.ToString();
        }
    }
}
