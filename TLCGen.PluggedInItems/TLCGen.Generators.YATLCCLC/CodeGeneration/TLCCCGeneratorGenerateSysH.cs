using System.Linq;
using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.TLCCC.CodeGeneration
{
    public partial class TLCCCGenerator
    {
        private string GenerateSysH(ControllerModel controller)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* GENREAL APPLICATIONFILE */");
            sb.AppendLine("/* ----------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "sys.h"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.AppendLine("#ifndef TLC_H_INCLUDED");
            sb.AppendLine("#define TLC_H_INCLUDED");
            sb.AppendLine();
            sb.Append(GenerateSysHSignalGroups(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHUitgangen(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHDetectors(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHInputs(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHTimers(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHSwitches(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHParameters(controller));
            sb.AppendLine();

            sb.AppendLine("/* modules */");
            sb.AppendLine("/* ------- */");
            var mli = 0;
            foreach (var ml in controller.ModuleMolen.Modules)
            {
                sb.AppendLine($"#define {ml.Naam} {mli++}");
            }
            sb.AppendLine($"#define MLMAX {mli} /* number of modules */");
            sb.AppendLine();

            sb.AppendLine("/* TLCCC includes */");
            sb.AppendLine("#include \"tlccc_main.h\"");
            sb.AppendLine("#include \"tlccc_clock.h\"");
            sb.AppendLine("#include \"tlccc_fc_func.h\"");
            sb.AppendLine("#include \"tlccc_det_func.h\"");
            sb.AppendLine("#include \"tlccc_timer_func.h\"");
            sb.AppendLine("#include \"tlccc_modules_func.h\"");
            sb.AppendLine("#include \"tlccc_prm_func.h\"");
            sb.AppendLine("#include \"tlccc_switch_func.h\"");
            sb.AppendLine("#include \"tlccc_cif.h\"");
            sb.AppendLine();

            sb.AppendLine("/* Function declarations */");
            sb.AppendLine("void application_init(PHASE phases[], DETECTOR detectors[], OUTGOING_SIGNAL os[], MODULEMILL * modulemill, MODULE modules[], CLOCK * clock);");
            sb.AppendLine("void application_exit(void);");
            sb.AppendLine();

            sb.AppendLine("#endif // TLC_H_INCLUDED");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateSysHSignalGroups(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* signalgroups */");
            sb.AppendLine("/* ------------ */");

            var pad1 = controller.Fasen.Select(fcm => (_sgpf + fcm.Naam).Length).Concat(new[] {"FCMAX".Length}).Max();
            pad1 = pad1 + "#define  ".Length;
            var pad2 = controller.Fasen.Count.ToString().Length;
            var index = 0;
            foreach (var fcm in controller.Fasen)
            {
                sb.Append($"#define {_sgpf}{fcm.Naam} ".PadRight(pad1));
                sb.AppendLine($"{index}".PadLeft(pad2));
                ++index;
            }
            sb.Append($"#define SGMAX ".PadRight(pad1));
            sb.Append($"{index} ".PadLeft(pad2));
            sb.AppendLine("/* number of signalgroups */");

            return sb.ToString();
        }

        private string GenerateSysHUitgangen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* outputs */");
            sb.AppendLine("/* ------- */");

            sb.Append(GetAllElementsSysHLines(_outputs, "SGMAX"));

            return sb.ToString();
        }

        private string GenerateSysHDetectors(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* detection */");
            sb.AppendLine("/* --------- */");

            var pad1 = "DMAX".Length;
            if(controller.Fasen.Any() && controller.Fasen.SelectMany(x => x.Detectoren).Any())
            {
                pad1 = controller.Fasen.SelectMany(x => x.Detectoren).Max(x =>(_dpf + x.Naam).Length);
            }
            if(controller.Detectoren.Any())
            {
                var _pad1 = controller.Detectoren.Max(x => (_dpf + x.Naam).Length);
                pad1 = _pad1 > pad1 ? _pad1 : pad1;
            }
            var ovdummies = controller.OVData.GetAllDummyDetectors();
            if (ovdummies.Any())
            {
                pad1 = ovdummies.Max(x => (_dpf + x.Naam).Length);
            }
            pad1 = pad1 + $"#define  ".Length;

            var pad2 = controller.Fasen.Count.ToString().Length;

            var index = 0;
            foreach (var fcm in controller.Fasen)
            {
                foreach (var dm in fcm.Detectoren)
                {
                    if (!dm.Dummy)
                    {
                        sb.Append($"#define {_dpf}{dm.Naam} ".PadRight(pad1));
                        sb.AppendLine($"{index}".PadLeft(pad2));
                        ++index;
                    }
                }
            }
            foreach (var dm in controller.Detectoren)
            {
                if (!dm.Dummy)
                {
                    sb.Append($"#define {_dpf}{dm.Naam} ".PadRight(pad1));
                    sb.AppendLine($"{index}".PadLeft(pad2));
                    ++index;
                }
            }

            var autom_index = index;

            /* Dummies */
            if (controller.Fasen.Any() && controller.Fasen.SelectMany(x => x.Detectoren).Any(x => x.Dummy) ||
                controller.Detectoren.Any() && controller.Detectoren.Any(x => x.Dummy) ||
                ovdummies.Any())
            {
                sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)");
                foreach (var fcm in controller.Fasen)
                {
                    foreach (var dm in fcm.Detectoren)
                    {
                        if (dm.Dummy)
                        {
                            sb.Append($"#define {_dpf}{dm.Naam} ".PadRight(pad1));
                            sb.AppendLine($"{index}".PadLeft(pad2));
                            ++index;
                        }
                    }
                }
                foreach (var dm in controller.Detectoren)
                {
                    if (dm.Dummy)
                    {
                        sb.Append($"#define {_dpf}{dm.Naam} ".PadRight(pad1));
                        sb.AppendLine($"{index}".PadLeft(pad2));
                        ++index;
                    }
                }
                foreach(var dm in ovdummies)
                {
                    sb.Append($"#define {_dpf}{dm.Naam} ".PadRight(pad1));
                    sb.AppendLine($"{index}".PadLeft(pad2));
                    ++index;
                }
                sb.Append($"#define DMAX ".PadRight(pad1));
                sb.Append($"{index} ".PadLeft(pad2));
                sb.AppendLine("/* number of detectors in test environment */");
                sb.AppendLine("#else");
                sb.Append($"#define DMAX ".PadRight(pad1));
                sb.Append($"{autom_index} ".PadLeft(pad2));
                sb.AppendLine("/* number of detectors in production environment */");
                sb.AppendLine("#endif");
            }
            else
            {
                sb.Append($"{ts}#define DMAX ".PadRight(pad1));
                sb.Append($"{index} ".PadLeft(pad2));
                sb.AppendLine("/* number of detectors */");
            }

            return sb.ToString();
        }

        private string GenerateSysHInputs(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* inputs */");
            sb.AppendLine("/* ------ */");

            sb.Append(GetAllElementsSysHLines(_inputs, "DMAX"));

            return sb.ToString();
        }

        private string GenerateSysHTimers(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* timers */");
            sb.AppendLine("/* ------ */");

            sb.Append(GetAllElementsSysHLines(_timers));

            return sb.ToString();
        }
        
        private string GenerateSysHSwitches(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* switches */");
            sb.AppendLine("/* -------- */");

            sb.Append(GetAllElementsSysHLines(_switches));

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
    }
}
