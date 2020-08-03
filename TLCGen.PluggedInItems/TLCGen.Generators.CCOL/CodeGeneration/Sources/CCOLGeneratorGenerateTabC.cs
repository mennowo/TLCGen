using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateTabC(ControllerModel controller)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* REGEL INSTELLINGEN */");
            sb.AppendLine("/* ------------------ */");
            sb.AppendLine();
	        if (controller.HalfstarData.IsHalfstar)
	        {
				sb.AppendLine("/* Definieer functie tbv tijden halfstar */");
		        sb.AppendLine("void signaalplan_instellingen(void);");
				sb.AppendLine();
	        }
            sb.Append(GenerateFileHeader(controller.Data, "tab.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();

            sb.Append(GenerateTabCBeforeIncludes(controller));

            sb.Append(GenerateTabCIncludes(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlDefaults(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParameters(controller));
            if (controller.HalfstarData.IsHalfstar && controller.HalfstarData.SignaalPlannen.Any())
            {
                sb.AppendLine();
                sb.Append(GenerateHstCSignaalPlanInstellingen(controller));
            }
            if (controller.StarData.ToepassenStar && controller.StarData.Programmas.Any())
            {
                sb.AppendLine();
                sb.Append(GenerateTabCStarPlanInstellingen(controller));
            }

            return sb.ToString();
        }

        private string GenerateTabCBeforeIncludes(ControllerModel c)
        {
            var sb = new StringBuilder();

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.TabCBeforeIncludes, false, true, false, true);

            return sb.ToString();
        }
        private string GenerateTabCIncludes(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"/* include files */");
            sb.AppendLine($"/* ------------- */");
            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine("#ifndef PRACTICE_TEST");
            }
            sb.AppendLine($"{ts}#include \"{c.Data.Naam}sys.h\"");
            sb.AppendLine($"{ts}#include \"fcvar.h\"    /* fasecycli                         */");
            sb.AppendLine($"{ts}#include \"kfvar.h\"    /* conflicten                        */");
            sb.AppendLine($"{ts}#include \"usvar.h\"    /* uitgangs elementen                */");
            sb.AppendLine($"{ts}#include \"dpvar.h\"    /* detectie elementen                */");
            if (c.Data.GarantieOntruimingsTijden)
            {
                if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && c.Data.Intergroen)
                {
                    sb.AppendLine($"{ts}#include \"tig_min.h\"   /* garantie-ontruimingstijden        */");
                }
                else
                {
                    sb.AppendLine($"{ts}#include \"to_min.h\"   /* garantie-ontruimingstijden        */");
                }
            }
            sb.AppendLine($"{ts}#include \"trg_min.h\"  /* garantie-roodtijden               */");
            sb.AppendLine($"{ts}#include \"tgg_min.h\"  /* garantie-groentijden              */");
            sb.AppendLine($"{ts}#include \"tgl_min.h\"  /* garantie-geeltijden               */");
            sb.AppendLine($"{ts}#include \"isvar.h\"    /* ingangs elementen                 */");
            if (c.HasDSI())
            {
                sb.AppendLine($"{ts}#include \"dsivar.h\"   /* selectieve detectie               */");
            }
            sb.AppendLine($"{ts}#include \"hevar.h\"    /* hulp elementen                    */");
            sb.AppendLine($"{ts}#include \"mevar.h\"    /* geheugen elementen                */");
            sb.AppendLine($"{ts}#include \"tmvar.h\"    /* tijd elementen                    */");
            sb.AppendLine($"{ts}#include \"ctvar.h\"    /* teller elementen                  */");
            sb.AppendLine($"{ts}#include \"schvar.h\"   /* software schakelaars              */");
            sb.AppendLine($"{ts}#include \"prmvar.h\"   /* parameters                        */");
            sb.AppendLine($"{ts}#include \"lwmlvar.h\"  /* langstwachtende modulen structuur */");
            sb.AppendLine($"{ts}#include \"control.h\"  /* controller interface              */");
            if (c.Data.VLOGType != Models.Enumerations.VLOGTypeEnum.Geen)
            {
                sb.AppendLine($"{ts}#ifndef NO_VLOG");
                sb.AppendLine($"{ts}{ts}#include \"vlogvar.h\"  /* variabelen t.b.v. vlogfuncties                */");
                sb.AppendLine($"{ts}{ts}#include \"logvar.h\"   /* variabelen t.b.v. logging                     */");
                sb.AppendLine($"{ts}{ts}#include \"monvar.h\"   /* variabelen t.b.v. realtime monitoring         */");
                sb.AppendLine($"{ts}#endif");
            }

            if (c.HalfstarData.IsHalfstar)
            {
                sb.AppendLine($"{ts}#include \"tx_synch.h\"");
                sb.AppendLine($"{ts}#include \"plevar.h\"");
                sb.AppendLine($"{ts}#include \"halfstar.h\"");
            }
            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine("#endif // PRACTICE_TEST");
            }

            sb.AppendLine();
            sb.AppendLine($"{ts}mulv FC_type[FCMAX];");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.TabCIncludes, true, true, true, true);

            return sb.ToString();
        }

        private string GenerateTabCControlDefaults(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void control_defaults(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.TabCControlDefaults, true, false, false, true);

            sb.AppendLine($"{ts}TDB_defmax = NG;");
            sb.AppendLine($"{ts}TDH_defmax = NG;");
            sb.AppendLine($"{ts}TBG_defmax = NG;");
            sb.AppendLine($"{ts}TOG_defmax = NG;");
            sb.AppendLine();
            sb.AppendLine($"{ts}TRG_defmax = NG;");
            sb.AppendLine($"{ts}TGG_defmax = NG;");
            sb.AppendLine($"{ts}TGL_defmax = NG;");
            sb.AppendLine($"{ts}TFG_defmax = NG;");
            sb.AppendLine($"{ts}TVG_defmax = NG;");
            sb.AppendLine();
            sb.AppendLine($"{ts}TRG_type    |= RO_type; /* Garantieroodtijden  read-only */");
            sb.AppendLine($"{ts}TGG_type    |= RO_type; /* Garantiegroentijden read-only */");
            sb.AppendLine($"{ts}TVG_deftype |= RO_type; /* Verlenggroentijden  read-only */");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.TabCControlDefaults, false, true, true, false);
            
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateTabCControlParameters(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void control_parameters(void)");
            sb.AppendLine("{");

            if(controller.Data.CCOLVersie < CCOLVersieEnum.CCOL9)
            {
                sb.AppendLine($"{ts}#ifndef NO_VLOG");
                sb.AppendLine($"{ts}{ts}int i;");
                sb.AppendLine($"{ts}#endif");
                sb.AppendLine();
            }

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.TabCControlParameters, true, false, false, true);

            sb.Append(GenerateTabCControlParametersFasen(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParametersConflicten(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParametersUitgangen(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParametersDetectors(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParametersIngangen(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParametersHulpElementen(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParametersGeheugenElementen(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParametersTijdElementen(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParametersCounters(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParametersSchakelaars(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParametersParameters(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParametersExtraData(controller));
            sb.AppendLine();
            if (controller.HasDSI())
            {
                sb.Append(GenerateTabCControlParametersDS(controller));
                sb.AppendLine();
            }
            sb.Append(GenerateTabCControlParametersModulen(controller));
            sb.AppendLine();
            if (controller.Data.VLOGType != Models.Enumerations.VLOGTypeEnum.Geen)
            {
                sb.Append(GenerateTabCControlParametersVLOG(controller));
                sb.AppendLine();
            }
            sb.Append(GenerateTabCControlParametersIOTypes(controller));
            sb.AppendLine();

	        AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.TabCControlParameters, false, true, false, true);

	        if (controller.HalfstarData.IsHalfstar)
	        {
		        sb.AppendLine($"{ts}signaalplan_instellingen();");
                sb.AppendLine();
	        }

            sb.AppendLine($"{ts}#include \"{controller.Data.Naam}tab.add\"");

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateTabCControlParametersIOTypes(ControllerModel c)
        {
            string GetIOTypeCode(DetectorTypeEnum detectorTypeEnum, DetectorModel dm, List<string> list)
            {
                var stringBuilder = new StringBuilder();
                if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL9)
                {
                    if (detectorTypeEnum != DetectorTypeEnum.VecomDetector &&
                        (dm.Type == DetectorTypeEnum.VecomDetector || dm.Type == DetectorTypeEnum.OpticomIngang))
                    {
                        stringBuilder.AppendLine("#ifndef NO_CVN_50");
                    }

                    if ((detectorTypeEnum == DetectorTypeEnum.VecomDetector ||
                         detectorTypeEnum == DetectorTypeEnum.OpticomIngang) &&
                        dm.Type != DetectorTypeEnum.VecomDetector)
                    {
                        stringBuilder.AppendLine("#else");
                        foreach (var d in list)
                        {
                            stringBuilder.AppendLine($"{ts}IS_type[{_dpf}{d}] = DS_type;");
                        }

                        list.Clear();
                        stringBuilder.AppendLine("#endif");
                    }
                }

                stringBuilder.Append($"{ts}IS_type[{_dpf}{dm.Naam}] = ");
                switch (dm.Type)
                {
                    case DetectorTypeEnum.Knop:
                    case DetectorTypeEnum.KnopBinnen:
                    case DetectorTypeEnum.KnopBuiten:
                        stringBuilder.AppendLine("DK_type;");
                        break;
                    case DetectorTypeEnum.File:
                    case DetectorTypeEnum.Verweg:
                        stringBuilder.AppendLine("DVER_type;");
                        break;
                    case DetectorTypeEnum.Kop:
                        stringBuilder.AppendLine("DKOP_type;");
                        break;
                    case DetectorTypeEnum.Lang:
                        stringBuilder.AppendLine("DLNG_type;");
                        break;
                    case DetectorTypeEnum.OpticomIngang:
                    case DetectorTypeEnum.VecomDetector:
                        if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL9)
                        {
                            stringBuilder.AppendLine("DSI_type;");
                            list.Add(dm.Naam);
                        }
                        else
                        {
                            // TODO: it is possible to use DKOP and DVER to mark in- and uitmelding: use?
                            // #define DSUIT_type  (DS_type+KOP_type) /* inmelding selectief        */
                            // #define DSIN_type   (DS_type+VER_type) /* uitmelding selectief       */
                            stringBuilder.AppendLine("DS_type;");
                        }

                        break;
                    case DetectorTypeEnum.Overig:
                        stringBuilder.AppendLine("DL_type;");
                        break;
                    case DetectorTypeEnum.WisselStandDetector:
                    case DetectorTypeEnum.WisselDetector:
                    case DetectorTypeEnum.WisselStroomKringDetector:
                    case DetectorTypeEnum.Radar:
                        stringBuilder.AppendLine("DKOP_type;");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Unknown detector type while generating tab.c: " +
                                                              dm.Type.ToString());
                }

                return stringBuilder.ToString();
            }

            var sb = new StringBuilder();

            sb.AppendLine("/* Typen ingangen */");
            sb.AppendLine("/* -------------- */");

            var ds = new List<string>();
            var prev = DetectorTypeEnum.Kop;
            foreach (var dm in c.GetAllDetectors(x => !x.Dummy))
            {
                sb.Append(GetIOTypeCode(prev, dm, ds));
                prev = dm.Type;
            }

            var dummies = c.GetAllDetectors(x => x.Dummy).ToList();
            if (dummies.Count != 0)
            {
                sb.AppendLine("#if !defined AUTOMAAT && !defined AUTOMAAT_TEST");
                foreach (var dm in dummies)
                {
                    sb.Append(GetIOTypeCode(prev, dm, ds));
                    prev = dm.Type;
                }
                sb.AppendLine("#endif");
            }

            sb.AppendLine();
            sb.AppendLine("/* Typen uitgangen */");
            sb.AppendLine("/* --------------- */");

            foreach (var fc in c.Fasen)
            {
                sb.Append($"{ts}US_type[{_fcpf}{fc.Naam}] = ");
                switch (fc.Type)
                {
                    case Models.Enumerations.FaseTypeEnum.Auto:
                        sb.AppendLine("MVT_type;");
                        break;
                    case Models.Enumerations.FaseTypeEnum.OV:
                        sb.AppendLine("OV_type;");
                        break;
                    case Models.Enumerations.FaseTypeEnum.Fiets:
                        sb.AppendLine("FTS_type;");
                        break;
                    case Models.Enumerations.FaseTypeEnum.Voetganger:
                        sb.AppendLine("VTG_type;");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Unknown vehicle type while generating tab.c: " + fc.Type.ToString());
                }
            }

            if (c.Data.CCOLVersie > CCOLVersieEnum.CCOL8 &&
                AllCCOLInputElements.Any(x => x.Multivalent))
            {
                sb.AppendLine();
                sb.AppendLine($"{ts}/* Multivalente ingangen */");
                sb.AppendLine("#if !defined NO_VLOG_300");
                foreach (var i in AllCCOLInputElements.Where(x => x.Multivalent && !x.Dummy))
                {
                    sb.AppendLine($"{ts}IS_type[{i.Naam}] = ISM_type;");
                }
                if (AllCCOLInputElements.Any(x => x.Multivalent && x.Dummy))
                {
                    sb.AppendLine("#if !defined AUTOMAAT && !defined AUTOMAAT_TEST");
                    foreach (var i in AllCCOLInputElements.Where(x => x.Multivalent && x.Dummy))
                    {
                        sb.AppendLine($"{ts}IS_type[{i.Naam}] = ISM_type;");
                    }
                    sb.AppendLine("#endif");
                }
                sb.AppendLine("#endif /* NO_VLOG_300 */");
            }
            if (c.Data.CCOLVersie > CCOLVersieEnum.CCOL8 &&
                AllCCOLOutputElements.Any(x => x.Multivalent))
            {
                sb.AppendLine();
                sb.AppendLine($"{ts}/* Multivalente ingangen */");
                sb.AppendLine("#if !defined NO_VLOG_300");
                foreach (var i in AllCCOLOutputElements.Where(x => x.Multivalent))
                {
                    sb.AppendLine($"{ts}US_type[{i.Naam}] = USM_type;");
                }
                if (AllCCOLOutputElements.Any(x => x.Multivalent && x.Dummy))
                {
                    sb.AppendLine("#if !defined AUTOMAAT && !defined AUTOMAAT_TEST");
                    foreach (var i in AllCCOLOutputElements.Where(x => x.Multivalent && x.Dummy))
                    {
                        sb.AppendLine($"{ts}US_type[{i.Naam}] = USM_type;");
                    }
                    sb.AppendLine("#endif");
                }
                sb.AppendLine("#endif /* NO_VLOG_300 */");
            }

            return sb.ToString();
        }

        private string GenerateTabCControlParametersFasen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* fasecycli */");
            sb.AppendLine("/* --------- */");

            var gs = controller.GroentijdenSets.FirstOrDefault();
            foreach (var fcm in controller.Fasen)
            {
                var s = $"   FC_code[{fcm.GetDefine()}] = \"{fcm.Naam}\"; TRG_max[{fcm.GetDefine()}] = {fcm.TRG}; ";
                sb.Append(s);
                sb.Append($"TRG_min[{fcm.GetDefine()}] = {fcm.TRG_min}; ");
                sb.Append($"TGG_max[{fcm.GetDefine()}] = {fcm.TGG}; ");
                sb.Append($"TGG_min[{fcm.GetDefine()}] = {fcm.TGG_min}; ");
                sb.Append($"TFG_max[{fcm.GetDefine()}] = {fcm.TFG}; ");
                sb.Append($"TGL_max[{fcm.GetDefine()}] = {fcm.TGL}; ");
                sb.Append($"TGL_min[{fcm.GetDefine()}] = {fcm.TGL_min}; ");
                var tvg = "NG";
                if (gs != null)
                {
                    var fcgs = gs.Groentijden.FirstOrDefault(x => x.FaseCyclus == fcm.Naam);
                    if (fcgs != null)
                    {
                        tvg = fcgs.Waarde == null ? "NG" : fcgs.Waarde.Value.ToString();
                    }
                }
                sb.AppendLine($"TVG_max[{fcm.GetDefine()}] = {tvg};");
            }

            return sb.ToString();
        }

        private string GenerateTabCControlParametersConflicten(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* conflicten */");
            sb.AppendLine("/* ---------- */");
            
            var totigmax = controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && controller.Data.Intergroen ? "TIG_max" : "TO_max";
            var totigmin = controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && controller.Data.Intergroen ? "TIG_min" : "TO_min";

            if (controller.InterSignaalGroep.Conflicten?.Count > 0)
            {
                var prevfasefrom = "";
                foreach (var conflict in controller.InterSignaalGroep.Conflicten)
                {
                    var ff = conflict.GetFaseFromDefine();
                    var ft = conflict.GetFaseToDefine();
                    if (ff == ft) continue;

                    // Cause an empty line in between signalgroups
                    if (prevfasefrom == "")
                    {
                        prevfasefrom = ff;
                    }
                    if(prevfasefrom != ff)
                    {
                        prevfasefrom = ff;
                        sb.AppendLine();
                    }
                    sb.AppendLine($"{ts}{totigmax}[{ff}][{ft}] = {conflict.SerializedWaarde};");
                }
            }

            if(controller.Fasen.Count > 0)
            { 
                var matrix = new int[controller.Fasen.Count, controller.Fasen.Count];
                for (var i = 0; i < controller.Fasen.Count; ++i)
                {
                    for (var j = 0; j < controller.Fasen.Count; ++j)
                    {
                        matrix[i, j] = -1;
                    }
                }

                for (var i = 0; i < controller.Fasen.Count; ++i)
                {
                    for (var j = 0; j < controller.Fasen.Count; ++j)
                    {
                        foreach (var cf in controller.InterSignaalGroep.Conflicten)
                        {
                            if (cf.FaseVan == controller.Fasen[i].Naam &&
                                cf.FaseNaar == controller.Fasen[j].Naam)
                            {
                                if (cf.Waarde >= -4)
                                {
                                    switch (cf.Waarde)
                                    {
                                        case -4:
                                            matrix[i, j] = -40;
                                            break;
                                        case -3:
                                            matrix[i, j] = -30;
                                            break;
                                        case -2:
                                            matrix[i, j] = -20;
                                            break;
                                        default:
                                            matrix[i, j] = cf.Waarde;
                                            break;
                                    }
                                }
                                else
                                {
                                    matrix[i, j] = -1;
                                }
                            }
                        }
                    }
                }

                // Below: only overwrite non-user defined values

                foreach (var gs in controller.InterSignaalGroep.Gelijkstarten)
                {
                    var fc = controller.Fasen.First(x => x.Naam == gs.FaseVan);
                    var i = controller.Fasen.IndexOf(fc);
                    var fc2 = controller.Fasen.First(x => x.Naam == gs.FaseNaar);
                    var j = controller.Fasen.IndexOf(fc2);

                    for (var k = 0; k < controller.Fasen.Count; ++k)
                    {
                        if (matrix[i, k] > -1 && matrix[j, k] == -1)
                        {
                            matrix[k, j] = -2;
                            matrix[j, k] = -2;
                        }
                    }
                }
                foreach (var vs in controller.InterSignaalGroep.Voorstarten)
                {
                    var fc = controller.Fasen.First(x => x.Naam == vs.FaseVan);
                    var i = controller.Fasen.IndexOf(fc);
                    var fc2 = controller.Fasen.First(x => x.Naam == vs.FaseNaar);
                    var j = controller.Fasen.IndexOf(fc2);

                    for (var k = 0; k < controller.Fasen.Count; ++k)
                    {
                        if (matrix[i, k] > -1 && matrix[j, k] == -1)
                        {
                            matrix[k, j] = -2;
                            matrix[j, k] = -2;
                        }
                    }
                }

                foreach (var nl in controller.InterSignaalGroep.Nalopen)
                {
                    var fc = controller.Fasen.First(x => x.Naam == nl.FaseVan);
                    var i = controller.Fasen.IndexOf(fc);
                    var fc2 = controller.Fasen.First(x => x.Naam == nl.FaseNaar);
                    var j = controller.Fasen.IndexOf(fc2);

                    for (var k = 0; k < controller.Fasen.Count; ++k)
                    {
                        if (matrix[j, k] > -1 && matrix[i, k] < 0 && matrix [i, k] > -20)
                        {
                            switch (nl.Type)
                            {
                                case Models.Enumerations.NaloopTypeEnum.StartGroen:
                                    if (nl.InrijdenTijdensGroen)
                                    {
                                        if (matrix[i, k] > -2) matrix[i, k] = -2;
                                        if (matrix[k, i] > -2) matrix[k, i] = -2;
                                    }
                                    else
                                    {
                                        if (matrix[i, k] > -3) matrix[i, k] = -3;
                                        if (matrix[k, i] > -3) matrix[k, i] = -3;
                                    }
                                    break;
                                case Models.Enumerations.NaloopTypeEnum.CyclischVerlengGroen:
                                    if (matrix[i, k] > -2) matrix[i, k] = -2;
                                    if (matrix[k, i] > -2) matrix[k, i] = -2;
                                    break;
                                case Models.Enumerations.NaloopTypeEnum.EindeGroen:
                                    if (nl.InrijdenTijdensGroen)
                                    {
                                        // TODO: eerst controleren
                                        // Dit wordt gecorrigeerd in RegCInitApplication door NalopenCodeGenerator
                                        if (matrix[i, k] > -4) matrix[i, k] = -4;
                                        //if (matrix[i, k] > -4) matrix[i, k] = -2; // wordt gecorrigeerd naar -4
                                        if (matrix[k, i] > -2) matrix[k, i] = -2;
                                    }
                                    else
                                    {
                                        if (matrix[i, k] > -4) matrix[i, k] = -4;
                                        if (matrix[k, i] > -3) matrix[k, i] = -3;
                                    }
                                    break;
                            }
                        }
                    }
                }
                sb.AppendLine();
                for (var i = 0; i < controller.Fasen.Count; ++i)
                {
                    bool AppendEmptyLine;
                    AppendEmptyLine = false;
                    for (var j = 0; j < controller.Fasen.Count; ++j)
                    {
                        if (matrix[i, j] >= -1) continue;
                        var k = "FK";
                        if (matrix[i, j] == -3 || matrix[i, j] == -30) k = "GK";
                        if (matrix[i, j] == -4 || matrix[i, j] == -40) k = "GKL";
                        sb.AppendLine($"{ts}{totigmax}[{_fcpf}{controller.Fasen[i].Naam}][{_fcpf}{controller.Fasen[j].Naam}] = {k};");
                        AppendEmptyLine = true;
                    }
                    if (AppendEmptyLine) sb.AppendLine();
                }

                if (controller.Data.GarantieOntruimingsTijden)
                {
                    if (controller.InterSignaalGroep.Conflicten?.Count > 0)
                    {
                        if (controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && controller.Data.Intergroen)
                        {
                            sb.AppendLine($"{ts}default_tig_min(0);");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}default_to_min(0);");
                        }
                        sb.AppendLine();

                        var prevfasefrom = "";
                        foreach (var conflict in controller.InterSignaalGroep.Conflicten.Where(x => x.GarantieWaarde != null))
                        {
                            var ff = conflict.GetFaseFromDefine();
                            var ft = conflict.GetFaseToDefine();
                            if (ff == ft) continue;

                            // Cause an empty line in between signalgroups
                            if (prevfasefrom == "")
                            {
                                prevfasefrom = ff;
                            }
                            if (prevfasefrom != ff)
                            {
                                prevfasefrom = ff;
                                sb.AppendLine();
                            }
                            sb.AppendLine($"{ts}{totigmin}[{ff}][{ft}] = {conflict.GarantieWaarde.Value};");
                        }

                    }
                }
            }


            return sb.ToString();
        }

        private string GenerateTabCControlParametersUitgangen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* overige uitgangen */");
            sb.AppendLine("/* ----------------- */");

            sb.Append(GetAllElementsTabCLines(_uitgangen));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersDetectors(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* detectie */");
            sb.AppendLine("/* -------- */");

            var defmax = 0;
            var namemax = 0;
            int? dbmax = 0;
            int? dhmax = 0;
            int? ogmax = 0;
            int? bgmax = 0;
            int? tflmax = 0;

            var ovdummydets = controller.PrioData.GetAllDummyDetectors();
            var alldets = controller.GetAllDetectors().Concat(ovdummydets);

            var nondummydets = alldets.Where(x => !x.Dummy);
            var detectorModels = nondummydets as DetectorModel[] ?? nondummydets.ToArray();

            foreach (var dm in alldets.Where(x => !x.Dummy))
            {
                if (dm.GetDefine()?.Length > defmax) defmax = dm.GetDefine().Length;
                if (dm.Naam?.Length > namemax) namemax = dm.Naam.Length;
                if (dm.TDB != null && dm.TDB > dbmax) dbmax = dm.TDB;
                if (dm.TDH != null && dm.TDH > dhmax) dhmax = dm.TDH;
                if (dm.TOG != null && dm.TOG > ogmax) ogmax = dm.TOG;
                if (dm.TBG != null && dm.TBG > bgmax) bgmax = dm.TBG;
                if (dm.TFL != null && dm.TFL > tflmax) tflmax = dm.TFL;
            }
            dbmax = dbmax.ToString().Length;
            dhmax = dhmax.ToString().Length;
            ogmax = ogmax.ToString().Length;
            bgmax = bgmax.ToString().Length;
            tflmax = tflmax.ToString().Length;

            var pad1 = "D_code[] ".Length + defmax;
            var pad2 = "= \"\"; ".Length + namemax;
            var pad3 = "TDB_max[] ".Length + defmax;
            var pad4 = "= ; ".Length + Math.Max(dbmax ?? 0, bgmax ?? 0);
            var pad5 = "TDH_max[] ".Length + defmax;
            var pad6 = pad1 + pad2;
            var pad7 = "TFL_max[] = ;".Length + defmax + tflmax;

            foreach (var dm in alldets)
            {
                if (!dm.Dummy)
                {
                    AppendDetectorTabString(sb, dm, pad1, pad2, pad3, pad4, pad5, pad6);
                }
            }

            if (detectorModels.Any(x => x.TFL != null || x.CFL != null))
            {
                sb.AppendLine("#if !defined NO_DDFLUTTER");
                foreach (var dm in detectorModels)
                {
                    AppendDetectorFlutterTabString(sb, dm, pad7);
                }
                sb.AppendLine("#endif // !defined NO_DDFLUTTER");
            }

            /* Dummies */
            var dummydets = alldets.Where(x => x.Dummy);
            detectorModels = dummydets as DetectorModel[] ?? dummydets.ToArray();
            if (detectorModels.Any())
            {
                dbmax = dhmax = ogmax = bgmax = tflmax = defmax = namemax = 0;
                foreach (var dm in detectorModels)
                {
                    if (dm.GetDefine()?.Length > defmax) defmax = dm.GetDefine().Length;
                    if (dm.Naam?.Length > namemax) namemax = dm.Naam.Length;
                    if (dm.TDB != null && dm.TDB > dbmax) dbmax = dm.TDB;
                    if (dm.TDH != null && dm.TDH > dhmax) dhmax = dm.TDH;
                    if (dm.TOG != null && dm.TOG > ogmax) ogmax = dm.TOG;
                    if (dm.TBG != null && dm.TBG > bgmax) bgmax = dm.TBG;
                    if (dm.TFL != null && dm.TFL > tflmax) tflmax = dm.TFL;
                }
                dbmax = dbmax.ToString().Length;
                dhmax = dhmax.ToString().Length;
                ogmax = ogmax.ToString().Length;
                bgmax = bgmax.ToString().Length;
                tflmax = tflmax.ToString().Length;
                pad1 = "D_code[] ".Length + defmax;
                pad2 = "= \"\"; ".Length + namemax;
                pad3 = "TDB_max[] ".Length + defmax;
                pad4 = "= ; ".Length + Math.Max(dbmax ?? 0, bgmax ?? 0);
                pad5 = "TDH_max[] ".Length + defmax;
                pad6 = pad1 + pad2;
                pad7 = "TFL_max[] = ;".Length + defmax + tflmax;

                sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined PRACTICE_TEST");
                foreach(var dm in detectorModels)
                {
                    AppendDetectorTabString(sb, dm, pad1, pad2, pad3, pad4, pad5, pad6);
                }
                if(detectorModels.Any(x => x.TFL != null || x.CFL != null))
                {
                    sb.AppendLine("#if !defined NO_DDFLUTTER");
                    foreach (var dm in detectorModels)
                    {
                        AppendDetectorFlutterTabString(sb, dm, pad7);
                    }
                    sb.AppendLine("#endif // !defined NO_DDFLUTTER");
                }
                sb.AppendLine("#endif");
            }

            return sb.ToString();
        }

        private void AppendDetectorTabString(StringBuilder sb, DetectorModel dm, int pad1, int pad2, int pad3, int pad4, int pad5, int pad6)
        {
            sb.Append($"{ts}");
            sb.Append($"D_code[{dm.GetDefine()}] ".PadRight(pad1));
            sb.Append($"= \"{dm.Naam}\"; ".PadRight(pad2));
            if (dm.TDB != null)
            {
                sb.Append($"TDB_max[{dm.GetDefine()}] ".PadRight(pad3));
                sb.Append($"= {dm.TDB}; ".PadRight(pad4));
            }
            if (dm.TDH != null)
            {
                sb.Append($"TDH_max[{dm.GetDefine()}] ".PadRight(pad5));
                sb.AppendLine($"= {dm.TDH};");
            }

            if (dm.TBG != null || dm.TOG != null)
            {
                if (dm.TDB != null || dm.TDH != null)
                {
                    sb.Append($"{ts}");
                    sb.Append("".PadLeft(pad6));
                }
                if (dm.TBG != null)
                {
                    sb.Append($"TBG_max[{dm.GetDefine()}] ".PadRight(pad3));
                    sb.Append($"= {dm.TBG}; ".PadRight(pad4));
                }
                if (dm.TOG != null)
                {
                    sb.Append($"TOG_max[{dm.GetDefine()}] ".PadRight(pad5));
                    sb.AppendLine($"= {dm.TOG};");
                }
                else
                {
                    sb.AppendLine("");
                }
            }
            else
            {
                sb.AppendLine("");
            }
        }
        private void AppendDetectorFlutterTabString(StringBuilder sb, DetectorModel dm, int? tflmax)
        {
            if (dm.TFL != null || dm.CFL != null)
            {
                if (dm.TFL != null)
                {
                    sb.Append($"{ts}");
                    var l = $"TFL_max[{dm.GetDefine()}] = {dm.TFL};";
                    sb.Append($"TFL_max[{dm.GetDefine()}] = {dm.TFL};".PadRight(tflmax.Value));
                }
                else
                {
                    sb.Append("".PadRight(tflmax.Value));
                }
                if (dm.CFL != null)
                {
                    if (dm.TFL != null) sb.Append(" ");
                    sb.Append($"CFL_max[{dm.GetDefine()}] ");
                    sb.AppendLine($"= {dm.CFL};");
                }
                else
                {
                    sb.AppendLine("");
                }
            }
        }

        private string GenerateTabCControlParametersIngangen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* overige ingangen */");
            sb.AppendLine("/* ---------------- */");

            sb.Append(GetAllElementsTabCLines(_ingangen));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersHulpElementen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* hulp elementen */");
            sb.AppendLine("/* -------------- */");

            sb.Append(GetAllElementsTabCLines(_hulpElementen));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersGeheugenElementen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* geheugen elementen */");
            sb.AppendLine("/* ------------------ */");
            
            sb.Append(GetAllElementsTabCLines(_geheugenElementen));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersTijdElementen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* tijd elementen */");
            sb.AppendLine("/* -------------- */");

            sb.Append(GetAllElementsTabCLines(_timers));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersCounters(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* teller elementen */");
            sb.AppendLine("/* ---------------- */");

            sb.Append(GetAllElementsTabCLines(_counters));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersSchakelaars(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* schakelaars */");
            sb.AppendLine("/* ----------- */");

            sb.Append(GetAllElementsTabCLines(_schakelaars));

            return sb.ToString();
        }


        private string GenerateTabCControlParametersParameters(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* parameters */");
            sb.AppendLine("/* ---------- */");

            sb.Append(GetAllElementsTabCLines(_parameters));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersExtraData(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* extra data */");
            sb.AppendLine("/* ---------- */");

            foreach(var fc in controller.Fasen)
            {
                switch(fc.Type)
                {
                    case Models.Enumerations.FaseTypeEnum.Auto:
                        sb.AppendLine($"{ts}FC_type[{_fcpf}{fc.Naam}] = MVT_type;");
                        break;
                    case Models.Enumerations.FaseTypeEnum.Fiets:
                        sb.AppendLine($"{ts}FC_type[{_fcpf}{fc.Naam}] = FTS_type;");
                        break;
                    case Models.Enumerations.FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}FC_type[{_fcpf}{fc.Naam}] = VTG_type;");
                        break;
                    case Models.Enumerations.FaseTypeEnum.OV:
                        sb.AppendLine($"{ts}FC_type[{_fcpf}{fc.Naam}] = OV_type;");
                        break;
                }
            }

            return sb.ToString();
        }

        private string GenerateTabCControlParametersDS(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* Selectieve detectie */");
            sb.AppendLine("/* ------------------- */");
            if (!controller.SelectieveDetectoren.Any())
            {
                // dummy lus voor KAR
                sb.AppendLine($"{ts}DS_code[dsdummy] = \"dsdummy\";");
            }
            else
            {
                // dummy lus voor KAR
                sb.AppendLine($"{ts}DS_code[dsdummy] = \"dsdummy\";");
                // selectieve lussen
                foreach (var sd in controller.SelectieveDetectoren)
                {
                    sb.AppendLine($"{ts}DS_code[{(_dpf + sd.Naam).ToUpper()}]  = \"{sd.Naam.ToUpper()}\";");
                }
            }

            return sb.ToString();
        }

        private string GenerateTabCControlParametersModulen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* modulen */");
            sb.AppendLine("/* ------- */");
            var ml = false;

            if (!controller.Data.MultiModuleReeksen)
            {
                if (controller.ModuleMolen.Modules.Any(x => x.Fasen.Any()))
                {
                    ml = true;
                    foreach (var mm in controller.ModuleMolen.Modules.Where(x => x.Fasen.Any()))
                    {
                        foreach (var mfcm in mm.Fasen)
                        {
                            sb.AppendLine($"{ts}PRML[{mm.Naam}][{mfcm.GetFaseCyclusDefine()}] = PRIMAIR;");
                        }
                        sb.AppendLine();
                    }
                }
            }
            else
            {
                foreach (var r in controller.MultiModuleMolens)
                {
                    if (r.Modules.Any())
                    {
                        if (ml) sb.AppendLine();
                        ml = true;
                        sb.AppendLine($"/* modules reeks {r.Reeks} */");
                        foreach (var mm in r.Modules.Where(x => x.Fasen.Any()))
                        {
                            foreach (var mfcm in mm.Fasen)
                            {
                                var mmNaam = Regex.Replace(mm.Naam, @"ML[A-E]+", "ML");
                                sb.AppendLine($"{ts}PR{r.Reeks}[{mmNaam}][{mfcm.GetFaseCyclusDefine()}] = PRIMAIR;");
                            }
                            sb.AppendLine();
                        }
                    }
                }
            }

            return sb.ToString();
        }

        private string GenerateTabCControlParametersVLOG(ControllerModel c)
        {
            if ((c.Data.CCOLVersie <= Models.Enumerations.CCOLVersieEnum.CCOL8 &&
                 c.Data.VLOGType == Models.Enumerations.VLOGTypeEnum.Geen ||
                 c.Data.CCOLVersie > Models.Enumerations.CCOLVersieEnum.CCOL8 &&
                 c.Data.VLOGSettings?.VLOGToepassen != true))
                return "";

            var sb = new StringBuilder();

            sb.AppendLine("#ifndef NO_VLOG");
            sb.AppendLine("/* VLOG */");
            sb.AppendLine("/* ---- */");

            if (c.Data.CCOLVersie < CCOLVersieEnum.CCOL9)
            {
                sb.AppendLine();
                sb.AppendLine($"{ts}/*VLOG - logging */");
                sb.AppendLine($"{ts}/*-------------- */");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_FC] = BIT0+BIT1+BIT2+BIT3+BIT5;");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_US] = BIT0+BIT1;");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_PS] = BIT0+BIT1;");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_DS] = BIT0+BIT1;");
                if (c.Data.VLOGType == VLOGTypeEnum.Filebased)
                {
                    sb.AppendLine($"{ts}LOGPRM[LOGPRM_LOGKLOKSCH] = 1;");
                    sb.AppendLine($"{ts}LOGPRM[LOGPRM_VLOGMODE] = VLOGMODE_LOG_FILE_ASCII;");
                }

                sb.AppendLine();
                sb.AppendLine($"{ts}/* VLOG - monitoring */");
                sb.AppendLine($"{ts}/* ----------------- */");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_FC] = BIT0+BIT1+BIT2+BIT3+BIT5;");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_US] = BIT0+BIT1;");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_PS] = BIT0+BIT1;");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_DS] = BIT0+BIT1;");

                sb.AppendLine();
                sb.AppendLine($"{ts}MONPRM[MONPRM_VLOGMODE] = 1; /* 1 = ASCII */");
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine();
                sb.AppendLine($"{ts}/* VLOG - logging */");
                sb.AppendLine($"{ts}/* -------------- */");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_DATI] = {c.Data.VLOGSettings.LOGTYPE_DATI};");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_DP] = {c.Data.VLOGSettings.LOGTYPE_DP};");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_IS] = {c.Data.VLOGSettings.LOGTYPE_IS};");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_FC] = {c.Data.VLOGSettings.LOGTYPE_FC};");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_US] = {c.Data.VLOGSettings.LOGTYPE_US};");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_PS] = {c.Data.VLOGSettings.LOGTYPE_PS};");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_DS] = {c.Data.VLOGSettings.LOGTYPE_DS};");
                sb.AppendLine($"#if !defined NO_VLOG_300");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_MLX] = {c.Data.VLOGSettings.LOGTYPE_MLX};");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_OMG] = {c.Data.VLOGSettings.LOGTYPE_OMG};");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_CRC] = {c.Data.VLOGSettings.LOGTYPE_CRC};");
                sb.AppendLine($"{ts}LOGTYPE[LOGTYPE_CFG] = {c.Data.VLOGSettings.LOGTYPE_CFG};");
                sb.AppendLine($"{ts}LOGPRM[LOGPRM_EVENT] = {c.Data.VLOGSettings.LOGPRM_EVENT};");
                sb.AppendLine($"#endif");
                sb.AppendLine($"{ts}LOGPRM[LOGPRM_LOGKLOKSCH] = 1;");
                sb.AppendLine($"{ts}LOGPRM[LOGPRM_VLOGMODE] = {c.Data.VLOGSettings.LOGPRM_VLOGMODE.ToString()};");
                sb.AppendLine();
                sb.AppendLine($"{ts}/* VLOG - monitoring */");
                sb.AppendLine($"{ts}/* ----------------- */");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_DATI] = {c.Data.VLOGSettings.MONTYPE_DATI};");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_DP] = {c.Data.VLOGSettings.MONTYPE_DP};");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_IS] = {c.Data.VLOGSettings.MONTYPE_IS};");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_FC] = {c.Data.VLOGSettings.MONTYPE_FC};");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_US] = {c.Data.VLOGSettings.MONTYPE_US};");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_PS] = {c.Data.VLOGSettings.MONTYPE_PS};");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_DS] = {c.Data.VLOGSettings.MONTYPE_DS};");
                sb.AppendLine($"#if !defined NO_VLOG_300");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_MLX] = {c.Data.VLOGSettings.MONTYPE_MLX};");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_OMG] = {c.Data.VLOGSettings.MONTYPE_OMG};");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_CRC] = {c.Data.VLOGSettings.MONTYPE_CRC};");
                sb.AppendLine($"{ts}MONTYPE[MONTYPE_CFG] = {c.Data.VLOGSettings.MONTYPE_CFG};");
                sb.AppendLine($"{ts}MONPRM[MONPRM_EVENT] = {c.Data.VLOGSettings.MONPRM_EVENT};");
                sb.AppendLine($"#endif");
                sb.AppendLine($"{ts}MONPRM[MONPRM_VLOGMODE] = {(c.Data.VLOGSettings.MONPRM_VLOGMODE == VLOGMonModeEnum.Binair ? "VLOGMODE_MON_BINAIR" : "VLOGMODE_MON_ASCII")};");
            }

            if (c.Data.CCOLVersie < CCOLVersieEnum.CCOL9)
            {
                sb.AppendLine();
                sb.AppendLine($"{ts}for (i = 0; i < FCMAX; ++i) MONFC[i] = BIT0+BIT1+BIT2+BIT3;");
                sb.AppendLine($"{ts}for (i = FCMAX; i < USMAX; ++i) MONUS[i] = BIT0+BIT1;");
            }

            sb.AppendLine("#endif /* NO_VLOG */");

            return sb.ToString();
        }

        private string GenerateTabCStarPlanInstellingen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            var _prmstarstart = CCOLGeneratorSettingsProvider.Default.GetElementName("prmstarstart");
            var _prmstareind = CCOLGeneratorSettingsProvider.Default.GetElementName("prmstareind");

            sb.AppendLine("void star_instellingen(void)");
            sb.AppendLine("{");

            sb.AppendLine($"{ts}/* STARRE PROGRAMMA INSTELLINGEN */");
            sb.AppendLine($"{ts}/* ============================= */");
            sb.AppendLine();
            int pr = 1;
            foreach (var programma in controller.StarData.Programmas)
            {
                sb.AppendLine($"{ts}/* {programma.Naam} */");
                sb.AppendLine($"{ts}STAR_ctijd[STAR{pr}] = {programma.Cyclustijd};");
                foreach (var sg in programma.Fasen)
                {
                    if (controller.StarData.ProgrammaSturingViaParameter)
                    {
                        sb.AppendLine($"{ts}STAR_start1[STAR{pr}][{_fcpf}{sg.FaseCyclus}] = PRM[{_prmpf}{_prmstarstart}1{programma.Naam}{sg.FaseCyclus}]; STAR_eind1[STAR{pr}][{_fcpf}{sg.FaseCyclus}] = PRM[{_prmpf}{_prmstareind}1{programma.Naam}{sg.FaseCyclus}];");
                        if (sg.Start2.HasValue && sg.Start2 != 0 && sg.Eind2.HasValue && sg.Eind2 != 0)
                        {
                            sb.AppendLine($"{ts}STAR_start2[STAR{pr}][{_fcpf}{sg.FaseCyclus}] = PRM[{_prmpf}{_prmstarstart}2{programma.Naam}{sg.FaseCyclus}; STAR_eind2[STAR{pr}][{_fcpf}{sg.FaseCyclus}] = PRM[{_prmpf}{_prmstareind}2{programma.Naam}{sg.FaseCyclus}];");
                        }           
                    }
                    else
                    {
                        sb.AppendLine($"{ts}STAR_start1[STAR{pr}][{_fcpf}{sg.FaseCyclus}] = {sg.Start1}; STAR_eind1[STAR{pr}][{_fcpf}{sg.FaseCyclus}] = {sg.Eind1}];");
                        if (sg.Start2.HasValue && sg.Start2 != 0 && sg.Eind2.HasValue && sg.Eind2 != 0)
                        {
                            sb.AppendLine($"{ts}STAR_start2[STAR{pr}][{_fcpf}{sg.FaseCyclus}] = {sg.Start2}; STAR_eind2[STAR{pr}][{_fcpf}{sg.FaseCyclus}] = {sg.Eind2}];");
                        }
                    }
                }
                ++pr;
                sb.AppendLine();
            }
		    
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
