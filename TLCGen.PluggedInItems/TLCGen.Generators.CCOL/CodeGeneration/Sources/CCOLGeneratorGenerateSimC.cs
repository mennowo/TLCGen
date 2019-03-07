using System.Linq;
using System.Text;
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
            var prms = GenerateSimCSimulationParameters(controller, out var lnkmax);
            sb.Append(GenerateSimCExtraDefines(controller, lnkmax));
            sb.AppendLine();
            sb.Append(GenerateSimCIncludes(controller));
            sb.AppendLine();
            sb.Append(GenerateSimCSimulationDefaults(controller));
            sb.AppendLine();
            sb.Append(prms);

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

        private string GenerateSimCExtraDefines(ControllerModel controller, int lnkmax)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"#define LNKMAX {lnkmax} /* aantal links */");

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

        private string GenerateSimCSimulationParameters(ControllerModel controller, out int lnkmax)
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
            var alldets = fasendets.Concat(controllerdets).Concat(ovdummydets).ToList();
            var itsstations = PieceGenerators.Where(x => x.HasSimulationElements()).SelectMany(x => x.GetSimulationElements()).ToList();

            foreach (var dm in alldets.Where(x => !x.Dummy))
            {
                sb.AppendLine($"{ts}LNK_code[{index}] = \"{dm.Naam}\";");
                sb.AppendLine($"{ts}IS_nr[{index}] = {dm.GetDefine()};");
                sb.AppendLine($"{ts}FC_nr[{index}] = {(!string.IsNullOrWhiteSpace(dm.Simulatie.FCNr) && dm.Simulatie.FCNr.ToUpper() != "NG" ? _fcpf + dm.Simulatie.FCNr : "NG")};");
                sb.AppendLine($"{ts}S_generator[{index}] = NG;");
                sb.AppendLine($"{ts}S_stopline[{index}] = {dm.Simulatie.Stopline};");
                sb.AppendLine($"{ts}Q1[{index}] = {dm.Simulatie.Q1};");
                sb.AppendLine($"{ts}Q2[{index}] = {dm.Simulatie.Q2};");
                sb.AppendLine($"{ts}Q3[{index}] = {dm.Simulatie.Q3};");
                sb.AppendLine($"{ts}Q4[{index}] = {dm.Simulatie.Q4};");
                sb.AppendLine();
                ++index;
            }
            if(alldets.Any(x => x.Dummy) || itsstations.Any())
            {
                sb.AppendLine("#if (!defined AUTOMAAT_TEST)");
                foreach (var dm in alldets.Where(x => x.Dummy))
                {
                    sb.AppendLine($"{ts}LNK_code[{index}] = \"{dm.Naam}\";");
                    sb.AppendLine($"{ts}IS_nr[{index}] = {dm.GetDefine()};");
                    sb.AppendLine($"{ts}FC_nr[{index}] = {(!string.IsNullOrWhiteSpace(dm.Simulatie.FCNr) && dm.Simulatie.FCNr.ToUpper() != "NG" ? _fcpf + dm.Simulatie.FCNr : "NG")};");
                    sb.AppendLine($"{ts}S_generator[{index}] = NG;");
                    sb.AppendLine($"{ts}S_stopline[{index}] = {dm.Simulatie.Stopline};");
                    sb.AppendLine($"{ts}Q1[{index}] = {dm.Simulatie.Q1};");
                    sb.AppendLine($"{ts}Q2[{index}] = {dm.Simulatie.Q2};");
                    sb.AppendLine($"{ts}Q3[{index}] = {dm.Simulatie.Q3};");
                    sb.AppendLine($"{ts}Q4[{index}] = {dm.Simulatie.Q4};");
                    sb.AppendLine();
                    ++index;
                }
                foreach (var e in itsstations)
                {
                    sb.AppendLine($"{ts}LNK_code[{index}] = \"{e.RelatedName}\";");
                    sb.AppendLine($"{ts}IS_nr[{index}] = {_ispf}{e.RelatedName};");
                    sb.AppendLine($"{ts}FC_nr[{index}] = {(!string.IsNullOrWhiteSpace(e.FCNr) && e.FCNr.ToUpper() != "NG" ? _fcpf + e.FCNr : "NG")};");
                    sb.AppendLine($"{ts}S_generator[{index}] = NG;");
                    sb.AppendLine($"{ts}S_stopline[{index}] = {e.Stopline};");
                    sb.AppendLine($"{ts}Q1[{index}] = {e.Q1};");
                    sb.AppendLine($"{ts}Q2[{index}] = {e.Q2};");
                    sb.AppendLine($"{ts}Q3[{index}] = {e.Q3};");
                    sb.AppendLine($"{ts}Q4[{index}] = {e.Q4};");
                    sb.AppendLine();
                    ++index;
                }
                sb.AppendLine("#endif");
            }

            sb.AppendLine();

            sb.AppendLine($"{ts}/* Gebruikers toevoegingen file includen */");
            sb.AppendLine($"{ts}/* ------------------------------------- */");
            sb.AppendLine($"{ts}#include \"{controller.Data.Naam}sim.add\"");

            sb.AppendLine();
            sb.AppendLine("}");

            lnkmax = index;

            return sb.ToString();
        }
    }
}
