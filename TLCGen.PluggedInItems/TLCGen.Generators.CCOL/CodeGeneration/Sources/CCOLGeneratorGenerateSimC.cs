using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateSimC(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* SIMULATIE APPLICATIE */");
            sb.AppendLine("/* -------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "sim.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.Append(GenerateSimCExtraDefines(controller));
            sb.AppendLine();
            sb.Append(GenerateSimCIncludes(controller));
            sb.AppendLine();
            sb.Append(GenerateSimCSimulationDefaults(controller));
            sb.AppendLine();
            sb.Append(GenerateSimCSimulationParameters(controller));

            return sb.ToString();
        }

        private string GenerateSimCIncludes(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* include files */");
            sb.AppendLine("/* ------------- */");
            sb.AppendLine($"{ts}#include \"{controller.Data.Naam}sys.h\"");
            sb.AppendLine($"{ts}#include \"simvar.c\" /* simulatie variabelen */");


            return sb.ToString();
        }

        private string GenerateSimCExtraDefines(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            var fasendets = controller.Fasen.SelectMany(x => x.Detectoren);
            var controllerdets = controller.Detectoren;
            var ovdummydets = controller.OVData.GetAllDummyDetectors();
            var alldets = fasendets.Concat(controllerdets).Concat(ovdummydets);
            int dpmax = alldets.Count();

            sb.AppendLine($"#define LNKMAX {dpmax} /* aantal links */");

            return sb.ToString();
        }

        private string GenerateSimCSimulationDefaults(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* Default waarden */");
            sb.AppendLine("/* --------------- */");
            sb.AppendLine("void simulation_defaults(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}S_defgenerator = NG;");
            sb.AppendLine($"{ts}S_defstopline  = 1800;");
            sb.AppendLine($"{ts}Q1_def         = 25;");
            sb.AppendLine($"{ts}Q2_def         = 50;");
            sb.AppendLine($"{ts}Q3_def         = 100;");
            sb.AppendLine($"{ts}Q4_def         = 200;");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateSimCSimulationParameters(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void simulation_parameters(void)");
            sb.AppendLine("{");

            sb.AppendLine(" T_subrun_max  = 3600; /* subrun tijd                 */");
            sb.AppendLine(" RANDGEN_start = 123;  /* startwaarde randomgenerator */");

            sb.AppendLine();

            sb.AppendLine($"{ts}/* Link parameters */");
            sb.AppendLine($"{ts}/* --------------- */");

            int index = 0;
            var fasendets = controller.Fasen.SelectMany(x => x.Detectoren);
            var controllerdets = controller.Detectoren;
            var ovdummydets = controller.OVData.GetAllDummyDetectors();
            var alldets = fasendets.Concat(controllerdets).Concat(ovdummydets);

            foreach (var dm in alldets)
            {
                sb.AppendLine($"{ts}LNK_code[{index}] = \"{dm.Naam}\";");
                sb.AppendLine($"{ts}IS_nr[{index}] = {dm.GetDefine()};");
                sb.AppendLine($"{ts}FC_nr[{index}] = {(dm.Simulatie.FCNr != null && dm.Simulatie.FCNr.ToUpper() != "NG" ? _fcpf + dm.Simulatie.FCNr : "NG")};");
                sb.AppendLine($"{ts}S_generator[{index}] = NG;");
                sb.AppendLine($"{ts}S_stopline[{index}] = 1800;");
                sb.AppendLine($"{ts}Q1[{index}] = {dm.Simulatie.Q1};");
                sb.AppendLine($"{ts}Q2[{index}] = {dm.Simulatie.Q2};");
                sb.AppendLine($"{ts}Q3[{index}] = {dm.Simulatie.Q3};");
                sb.AppendLine($"{ts}Q4[{index}] = {dm.Simulatie.Q4};");
                sb.AppendLine();
                ++index;
            }

            sb.AppendLine();

            sb.AppendLine($"{ts}/* Gebruikers toevoegingen file includen */");
            sb.AppendLine($"{ts}/* ------------------------------------- */");
            sb.AppendLine($"{ts}#include \"{controller.Data.Naam}sim.add\"");

            sb.AppendLine();
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
