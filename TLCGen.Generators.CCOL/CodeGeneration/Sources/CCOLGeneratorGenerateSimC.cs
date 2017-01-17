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
            sb.AppendLine($"{tabspace}#include \"{controller.Data.Naam}sys.h\"");
            sb.AppendLine($"{tabspace}#include \"simvar.c\" /* simulatie variabelen */");


            return sb.ToString();
        }

        private string GenerateSimCExtraDefines(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            int dpmax = 0;
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    ++dpmax;
                }
            }

            foreach (DetectorModel dm in controller.Detectoren)
            {
                ++dpmax;
            }

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
            sb.AppendLine($"{tabspace}S_defgenerator = NG;");
            sb.AppendLine($"{tabspace}S_defstopline  = 1800;");
            sb.AppendLine($"{tabspace}Q1_def         = 25;");
            sb.AppendLine($"{tabspace}Q2_def         = 50;");
            sb.AppendLine($"{tabspace}Q3_def         = 100;");
            sb.AppendLine($"{tabspace}Q4_def         = 200;");
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

            sb.AppendLine($"{tabspace}/* Link parameters */");
            sb.AppendLine($"{tabspace}/* --------------- */");

            int index = 0;
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    sb.AppendLine($"{tabspace}LNK_code[{index}] = \"{dm.Naam}\";");
                    sb.AppendLine($"{tabspace}IS_nr[{index}] = {dm.GetDefine()};");
                    sb.AppendLine($"{tabspace}FC_nr[{index}] = {fcm.GetDefine()};");
                    sb.AppendLine($"{tabspace}S_generator[{index}] = NG;");
                    sb.AppendLine($"{tabspace}S_stopline[{index}] = 1800;");
                    sb.AppendLine($"{tabspace}Q1[{index}] = {dm.Simulatie.Q1};");
                    sb.AppendLine($"{tabspace}Q2[{index}] = {dm.Simulatie.Q2};");
                    sb.AppendLine($"{tabspace}Q3[{index}] = {dm.Simulatie.Q3};");
                    sb.AppendLine($"{tabspace}Q4[{index}] = {dm.Simulatie.Q4};");
                    sb.AppendLine();
                    ++index;
                }
            }
            foreach (DetectorModel dm in controller.Detectoren)
            {
                sb.AppendLine($"{tabspace}LNK_code[{index}] = \"{dm.Naam}\";");
                sb.AppendLine($"{tabspace}IS_nr[{index}] = {dm.GetDefine()};");
                sb.AppendLine($"{tabspace}FC_nr[{index}] = NG;");
                sb.AppendLine($"{tabspace}S_generator[{index}] = NG;");
                sb.AppendLine($"{tabspace}S_stopline[{index}] = 1800;");
                sb.AppendLine($"{tabspace}Q1[{index}] = {dm.Simulatie.Q1};");
                sb.AppendLine($"{tabspace}Q2[{index}] = {dm.Simulatie.Q2};");
                sb.AppendLine($"{tabspace}Q3[{index}] = {dm.Simulatie.Q3};");
                sb.AppendLine($"{tabspace}Q4[{index}] = {dm.Simulatie.Q4};");
                sb.AppendLine();
                ++index;
            }

            sb.AppendLine();

            sb.AppendLine($"{tabspace}/* Gebruikers toevoegingen file includen */");
            sb.AppendLine($"{tabspace}/* ------------------------------------- */");
            sb.AppendLine($"{tabspace}#include \"{controller.Data.Naam}sim.add\"");

            sb.AppendLine();
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
