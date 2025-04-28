using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.TLCCC.CodeGeneration
{
    public partial class TLCCCGenerator
    {
        private string GenerateRegC(ControllerModel controller)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* APPLICATION SETTINGS */");
            sb.AppendLine("/* -------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "tab.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.Append(GenerateRegCDefines(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCIncludes(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCDefinitions(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCApplication(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCApplicationExit(controller));

            return sb.ToString();
        }

        private string GenerateRegCDefines(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#define CIF_PUBLIC");
            sb.AppendLine("#define NO_CIF_MON");

            return sb.ToString();
        }

        private string GenerateRegCIncludes(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"/* include files */");
            sb.AppendLine($"/* ------------- */");
            sb.AppendLine($"    #include \"{controller.Data.Naam}sys.h\"");
            
            return sb.ToString();
        }

        private string GenerateRegCDefinitions(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("static char running = TRUE;");
            sb.AppendLine();
            sb.AppendLine("/* TLCCC variables */");
            sb.AppendLine("SIGNALGROUP signalgroups[SGMAX] = { 0 };");
            sb.AppendLine("OUTGOING_SIGNAL outging_signals[OSMAX] = { 0 };");
            sb.AppendLine("DETECTOR detectors[DMAX] = { 0 };");
            sb.AppendLine("TIMER timers[TMMAX] = { 0 };");
            sb.AppendLine("MODULE modules[MLMAX] = { 0 };");
            sb.AppendLine("SWITCH switches[SWMAX] = { 0 };");
            sb.AppendLine("PARAMETER parameters[PRMMAX] = { 0 };");
            sb.AppendLine("MODULEMILL modulemill = { 0 };");
            sb.AppendLine("CLOCK clock = { 0 };");
            sb.AppendLine();
            sb.AppendLine("#ifdef TLCCC_WIN32");
            sb.AppendLine($"{ts}short SignalGroups_internal_state[SGMAX];");
            sb.AppendLine($"{ts}short SignalGroups_internal_state_alt[SGMAX];");
            sb.AppendLine("#endif");

            return sb.ToString();
        }

        private string GenerateRegCApplication(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("s_int16 applicatieprogramma(s_int16 state)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}if (state == CIF_INIT)");
            sb.AppendLine($"{ts}{{");
            sb.AppendLine($"{ts}{ts}/* Initialize */");
            sb.AppendLine($"{ts}{ts}application_init(signalgroups, detectors, outging_signals, &modulemill, modules, &clock);");
            sb.AppendLine($"{ts}}}");
            sb.AppendLine($"{ts}else if(running == TRUE)");
            sb.AppendLine($"{ts}{{");
            sb.AppendLine($"{ts}{ts}/* Read data from interface */");
            sb.AppendLine($"{ts}{ts}if (CIF_ISWIJZ)");
            sb.AppendLine($"{ts}{ts}{{");
            sb.AppendLine($"{ts}{ts}{ts}CIF_ISWIJZ = FALSE;");
            sb.AppendLine($"{ts}{ts}}}");
            sb.AppendLine($"{ts}{ts}if (CIF_WUSWIJZ)");
            sb.AppendLine($"{ts}{ts}{{");
            sb.AppendLine($"{ts}{ts}{ts}CIF_WUSWIJZ = FALSE;");
            sb.AppendLine($"{ts}{ts}}}");
            sb.AppendLine($"{ts}{ts}// TODO: process other inputs");
            sb.AppendLine($"{ts}{ts}Detectors_update(detectors, DMAX);");
            sb.AppendLine($"{ts}{ts}if (CIF_PARM1WIJZPB)");
            sb.AppendLine($"{ts}{ts}{{");
            sb.AppendLine($"{ts}{ts}{ts}// TODO");
            sb.AppendLine($"{ts}{ts}}}");
            sb.AppendLine();
            sb.AppendLine($"{ts}{ts}/* Update clock and timers */");
            sb.AppendLine($"{ts}{ts}Clock_update(&clock);");
            sb.AppendLine($"{ts}{ts}SignalGroups_timers_update(signalgroups, SGMAX, &clock);");
            sb.AppendLine($"{ts}{ts}Detectors_timers_update(detectors, DMAX, &clock);");
            sb.AppendLine($"{ts}{ts}Timers_update(timers, TMMAX, &clock);");
            sb.AppendLine();
            sb.AppendLine($"{ts}{ts}/* Update state */");
            sb.AppendLine($"{ts}{ts}SignalGroups_requests(signalgroups, SGMAX);");
            sb.AppendLine($"{ts}{ts}// TODO: update waiting green (later)");
            sb.AppendLine($"{ts}{ts}SignalGroups_extending(signalgroups, SGMAX);");
            sb.AppendLine($"{ts}{ts}// TODO: update free extending");
            sb.AppendLine($"{ts}{ts}SignalGroups_update_conflicts(signalgroups, SGMAX);");
            sb.AppendLine($"{ts}{ts}");
            sb.AppendLine($"{ts}{ts}Modules_update_primary(&modulemill);");
            sb.AppendLine($"{ts}{ts}//Modules_update_alternative(&modulemill, signalgroups, FCMAX);");
            sb.AppendLine($"{ts}{ts}SignalGroups_state_update_ML(signalgroups, SGMAX, &CIF_GUSWIJZ);");
            sb.AppendLine($"{ts}{ts}Modules_move_the_mill(&modulemill, signalgroups, SGMAX);");
            sb.AppendLine();
            sb.AppendLine($"{ts}{ts}Modules_update_segment_display(&modulemill, outging_signals, ossegm1, &CIF_GUSWIJZ);");
            sb.AppendLine($"{ts}{ts}");
            sb.AppendLine($"{ts}{ts}if (CIF_GUSWIJZ)");
            sb.AppendLine($"{ts}{ts}{{");
            sb.AppendLine($"{ts}{ts}{ts}SignalGroups_state_out_update(signalgroups, SGMAX);");
            sb.AppendLine($"{ts}{ts}{ts}Set_GUS(signalgroups, SGMAX, outging_signals, OSMAX- SGMAX);");
            sb.AppendLine($"{ts}{ts}}}");
            sb.AppendLine();
            sb.AppendLine("#ifdef TLCCC_WIN32");
            sb.AppendLine($"{ts}{ts}/* Internal state to WIN32 environment */");
            sb.AppendLine($"{ts}{ts}int i;");
            sb.AppendLine($"{ts}{ts}for (i = 0; i < SGMAX; ++i)");
            sb.AppendLine($"{ts}{ts}{{");
            sb.AppendLine($"{ts}{ts}{ts}SignalGroups_internal_state[i] = signalgroups[i].CycleState;");
            sb.AppendLine($"{ts}{ts}{ts}SignalGroups_internal_state_alt[i] = signalgroups[i].ML_Alternative;");
            sb.AppendLine($"{ts}{ts}}}");
            sb.AppendLine("#endif");
            sb.AppendLine($"{ts}{ts}");
            sb.AppendLine($"{ts}{ts}// TODO: check if we need to copy params from tlccc to outside buffers");
            sb.AppendLine($"{ts}}}");
            sb.AppendLine("");
            sb.AppendLine($"{ts}return 0;");
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        private string GenerateRegCApplicationExit(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void application_exit(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}/* Set running state to false, cause otherwise the application */");
            sb.AppendLine($"{ts}/* will try to access freed memory if running at high speeds */");
            sb.AppendLine($"{ts}running = FALSE;");
            sb.AppendLine();
            sb.AppendLine($"{ts}/* Free all allocated memory; good programming practice! */");
            sb.AppendLine($"{ts}SignalGroups_free(signalgroups, SGMAX);");
            sb.AppendLine($"{ts}Detectors_free(detectors, DMAX);");
            sb.AppendLine($"{ts}ModuleMill_free(&modulemill, MLMAX);");
            sb.AppendLine($"{ts}Parameters_free(parameters, PRMMAX);");
            sb.AppendLine($"{ts}Timers_free(timers, TMMAX);");
            sb.AppendLine($"{ts}Switches_free(switches, SWMAX);");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
