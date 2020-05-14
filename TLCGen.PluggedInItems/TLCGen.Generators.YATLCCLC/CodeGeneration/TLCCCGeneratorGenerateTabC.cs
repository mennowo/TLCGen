using System.Linq;
using System.Text;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.TLCCC.CodeGeneration
{
    public partial class TLCCCGenerator
    {
        private string GenerateTabC(ControllerModel controller)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* APPLICATION SETTINGS */");
            sb.AppendLine("/* -------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "tab.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.Append(GenerateTabCIncludes(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCApplicationInit(controller));

            return sb.ToString();
        }

        private string GenerateTabCIncludes(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"/* include files */");
            sb.AppendLine($"/* ------------- */");
            sb.AppendLine($"#include \"{controller.Data.Naam}sys.h\"");
            
            return sb.ToString();
        }

        private string GenerateTabCApplicationInit(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void application_init(SIGNALGROUP signalgroups[], DETECTOR detectors[], OUTGOING_SIGNAL os[], MODULEMILL * modulemill, MODULE modules[], CLOCK * clock)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}int i;");
            sb.AppendLine($"{ts}short CT_max[SGMAX * SGMAX] = {{ 0 }};");
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Specify signalgroups");
            sb.AppendLine($"{ts}                 PHASE-pointer        code    index tgg tgf tge  tye trg thm */");
            foreach (var sg in controller.Fasen)
            {
                sb.AppendLine($"{ts}SignalGroup_init(&signalgroups[{_sgpf}{sg.Naam}], \"{_sgpf}{sg.Naam}\", {_sgpf}{sg.Naam}, {sg.TGG}," +
                              $" {sg.TFG}, 300, {sg.TGL}, {sg.TRG}, {sg.Kopmax});");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Specify conflicts */");
            sb.AppendLine($"{ts}for (i = 0; i < SGMAX * SGMAX; ++i)");
            sb.AppendLine($"{ts}{{");
            sb.AppendLine($"{ts}{ts}CT_max[i] = -1;");
            sb.AppendLine($"{ts}}}");
            sb.AppendLine();
            string lastIgt = null;
            foreach (var igt in controller.InterSignaalGroep.Conflicten)
            {
                if (igt.Waarde > 0 && lastIgt != null && lastIgt != igt.FaseVan)
                {
                    sb.AppendLine();
                }
                lastIgt = igt.FaseVan;
                if (igt.Waarde > 0)
                {
                    sb.AppendLine($"{ts}SignalGroup_conflict_init({_sgpf}{igt.FaseVan}, {_sgpf}{igt.FaseNaar}, CT_max, SGMAX, {igt.Waarde});");
                }
            }
            sb.AppendLine();
            sb.AppendLine($"{ts}SignalGroups_set_conflict_pointers(signalgroups, SGMAX, CT_max);");
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Specify modules */");
            foreach (var ml in controller.ModuleMolen.Modules)
            {
                sb.Append($"{ts}Module_init(&modules[{ml.Naam}], {ml.Naam}, \"{ml.Naam}\", {ml.Fasen.Count}");
                foreach (var sg in ml.Fasen)
                {
                    sb.Append($", &signalgroups[{_sgpf}{sg.FaseCyclus}]");
                }
                sb.AppendLine(");");
            }
            sb.AppendLine(
                $"{ts}ModuleMill_init(modulemill, modules, {controller.ModuleMolen.Modules.Count}, {controller.ModuleMolen.WachtModule});");
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Specify alternative conditions */");
            sb.AppendLine("{ts}Modules_set_alternative_space_default(signalgroups, SGMAX, 60);");
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Specify detectors */");
            foreach (var d in controller.Fasen.SelectMany(x => x.Detectoren))
            {
                string dtype, dreq, dext;
                switch (d.Type)
                {
                    case DetectorTypeEnum.Kop:
                        dtype = "D_HEAD";
                        break;
                    case DetectorTypeEnum.Lang:
                        dtype = "D_LONG";
                        break;
                    case DetectorTypeEnum.Verweg:
                        dtype = "D_AWAY";
                        break;
                    case DetectorTypeEnum.File:
                        dtype = "D_JAM";
                        break;
                    case DetectorTypeEnum.Knop:
                    case DetectorTypeEnum.KnopBinnen:
                    case DetectorTypeEnum.KnopBuiten:
                        dtype = "D_BUTTON";
                        break;
                    default:
                        dtype = "D_OTHER";
                        break;
                }
                switch (d.Aanvraag)
                {
                    case DetectorAanvraagTypeEnum.RnietTRG:
                        dreq = "DET_REQ_REDNONGAR";
                        break;
                    case DetectorAanvraagTypeEnum.Rood:
                        dreq = "DET_REQ_RED";
                        break;
                    case DetectorAanvraagTypeEnum.RoodGeel:
                        dreq = "DET_REQ_YELLOW";
                        break;
                    default:
                        dreq = "DET_REQ_NONE";
                        break;
                }
                switch (d.Verlengen)
                {
                    case DetectorVerlengenTypeEnum.Kopmax:
                        dext = "DET_EXT_HEADMAX";
                        break;
                    case DetectorVerlengenTypeEnum.MK1:
                    case DetectorVerlengenTypeEnum.MK2:
                        dext = "DET_EXT_MEASURE";
                        break;
                    default:
                        dext = "DET_EXT_NONE";
                        break;
                }
                sb.AppendLine(
                    $"{ts}Detector_init(&detectors[{_dpf}{d.Naam}], \"{_dpf}{d.Naam}\", &CIF_IS[{_dpf}{d.Naam}], {dtype}, {dreq}, {dext}, {d.TDB}, {d.TDH}, {d.TOG}, {d.TBG});");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Assign detectors to signalgroups */");
            foreach (var sg in controller.Fasen)
            {
                sb.Append($"{ts}SignalGroup_add_detectors(&signalgroups[{_sgpf}{sg.Naam}], {sg.Detectoren.Count}");
                foreach (var d in sg.Detectoren)
                {
                    sb.Append($", &detectors[{_dpf}{d.Naam}]");
                }
                sb.AppendLine(");");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Initiate outgoing signals */");
            foreach (var os in _outputs.Elements)
            {
                sb.AppendLine($"{ts}Outgoing_signal_init(&os[{os.Define}], \"{os.Define}\", {os.Define}+SGMAX);");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Initiate clock structure */");
            sb.AppendLine($"{ts}Clock_CIF_init(clock);");

            sb.AppendLine($"{ts}/* Register exit function */");
            sb.AppendLine($"{ts}atexit(application_exit);");
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
