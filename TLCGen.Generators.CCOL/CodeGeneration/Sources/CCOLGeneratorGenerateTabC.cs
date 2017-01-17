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
        private string GenerateTabC(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* REGEL INSTELLINGEN */");
            sb.AppendLine("/* ------------------ */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "tab.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.Append(GenerateTabCIncludes(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlDefaults(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParameters(controller));

            return sb.ToString();
        }

        private string GenerateTabCIncludes(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* include files */");
            sb.AppendLine("/* ------------- */");
            sb.AppendLine($"    #include \"{controller.Data.Naam}sys.h\"");
#warning TODO - make includes dependent on the kind of controller and its settings
            sb.AppendLine("/* include files */");
            sb.AppendLine("/* ------------- */");
            sb.AppendLine("    #include \"fcvar.h\"    /* fasecycli                         */");
            sb.AppendLine("    #include \"kfvar.h\"    /* conflicten                        */");
            sb.AppendLine("    #include \"usvar.h\"    /* uitgangs elementen                */");
            sb.AppendLine("    #include \"dpvar.h\"    /* detectie elementen                */");
            sb.AppendLine("    #include \"to_min.h\"   /* garantie-ontruimingstijden        */");
            sb.AppendLine("    #include \"trg_min.h\"  /* garantie-roodtijden               */");
            sb.AppendLine("    #include \"tgg_min.h\"  /* garantie-groentijden              */");
            sb.AppendLine("    #include \"tgl_min.h\"  /* garantie-geeltijden               */");
            sb.AppendLine("    #include \"isvar.h\"    /* ingangs elementen                 */");
            sb.AppendLine("    #include \"dsivar.h\"   /* selectieve detectie               */");
            sb.AppendLine("    #include \"hevar.h\"    /* hulp elementen                    */");
            sb.AppendLine("    #include \"mevar.h\"    /* geheugen elementen                */");
            sb.AppendLine("    #include \"tmvar.h\"    /* tijd elementen                    */");
            sb.AppendLine("    #include \"ctvar.h\"    /* teller elementen                  */");
            sb.AppendLine("    #include \"schvar.h\"   /* software schakelaars              */");
            sb.AppendLine("    #include \"prmvar.h\"   /* parameters                        */");
            sb.AppendLine("    #include \"lwmlvar.h\"  /* langstwachtende modulen structuur */");
            sb.AppendLine("    #include \"control.h\"  /* controller interface              */");
            //sb.AppendLine("#ifndef NO_VLOG");
            //sb.AppendLine("    #include \"vlogvar.h\"  /* variabelen t.b.v. vlogfuncties                */");
            //sb.AppendLine("    #include \"logvar.h\"   /* variabelen t.b.v. logging                     */");
            //sb.AppendLine("    #include \"monvar.h\"   /* variabelen t.b.v. realtime monitoring         */");
            //sb.AppendLine("#endif");

            sb.AppendLine($"    #include \"{controller.Data.Naam}tab.add\"");

            return sb.ToString();
        }

        private string GenerateTabCControlDefaults(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void control_defaults(void)");
            sb.AppendLine("{");
            sb.AppendLine("    TDB_defmax = NG;");
            sb.AppendLine("    TDH_defmax = NG;");
            sb.AppendLine("    TBG_defmax = NG;");
            sb.AppendLine("    TOG_defmax = NG;");
            sb.AppendLine();
            sb.AppendLine("    TRG_defmax = NG;");
            sb.AppendLine("    TGG_defmax = NG;");
            sb.AppendLine("    TGL_defmax = NG;");
            sb.AppendLine("    TFG_defmax = NG;");
            sb.AppendLine("    TVG_defmax = NG;");
            sb.AppendLine();
            sb.AppendLine("    TVG_deftype |= RO_type; /* Verlenggroentijden  read-only */");
            sb.AppendLine();
            //sb.AppendLine("    MON_def=1;");
            //sb.AppendLine("    LOG_def=1;");
            //sb.AppendLine();
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateTabCControlParameters(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void control_parameters(void)");
            sb.AppendLine("{");
            sb.AppendLine();

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
            sb.Append(GenerateTabCControlParametersTijdElementen(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParametersParameters(controller));
            sb.AppendLine();
            sb.Append(GenerateTabCControlParametersModulen(controller));
            sb.AppendLine();

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateTabCControlParametersFasen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* fasecycli */");
            sb.AppendLine("/* --------- */");

            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                string s = $"   FC_code[{fcm.GetDefine()}] = \"{fcm.Naam}\"; TRG_max[{fcm.GetDefine()}] = {fcm.TRG};";
                int i = s.Length;
                sb.AppendLine(s);
                sb.AppendLine($"TRG_min[{fcm.GetDefine()}] = {fcm.TRG_min};".PadLeft(i));
                sb.AppendLine($"TGG_max[{fcm.GetDefine()}] = {fcm.TGG};".PadLeft(i));
                sb.AppendLine($"TGG_min[{fcm.GetDefine()}] = {fcm.TGG_min};".PadLeft(i));
                sb.AppendLine($"TFG_max[{fcm.GetDefine()}] = {fcm.TFG};".PadLeft(i));
                sb.AppendLine($"TGL_max[{fcm.GetDefine()}] = {fcm.TGL};".PadLeft(i));
                sb.AppendLine($"TGL_min[{fcm.GetDefine()}] = {fcm.TGL_min};".PadLeft(i));
                sb.AppendLine($"TVG_max[{fcm.GetDefine()}] = NG;".PadLeft(i));
            }

            return sb.ToString();
        }

        private string GenerateTabCControlParametersConflicten(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* conflicten */");
            sb.AppendLine("/* ---------- */");
            
            if (controller.InterSignaalGroep.Conflicten?.Count > 0)
            {
                foreach (ConflictModel conflict in controller.InterSignaalGroep.Conflicten)
                {
                    sb.AppendLine($"    TO_max[{conflict.FaseVan}][{conflict.FaseNaar}] = {conflict.SerializedWaarde};");
                    sb.AppendLine();
                }
            }
            //sb.AppendLine("default_to_min(0);");

            return sb.ToString();
        }

        private string GenerateTabCControlParametersUitgangen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* overige uitgangen */");
            sb.AppendLine("/* ----------------- */");

            sb.Append(GetAllElementsTabCLines(Uitgangen));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersDetectors(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* detectie */");
            sb.AppendLine("/* -------- */");

            int defmax = 0;
            int namemax = 0;
            int? dbmax = 0;
            int? dhmax = 0;
            int? ogmax = 0;
            int? bgmax = 0;
            int? cflmax = 0;
            int? tflmax = 0;

            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.Detectoren?.Count > 0)
                {
                    foreach (DetectorModel dm in fcm.Detectoren)
                    {
                        if (dm.GetDefine()?.Length > defmax) defmax = dm.GetDefine().Length;
                        if (dm.Naam?.Length > namemax) namemax = dm.Naam.Length;
                        if (dm.TDB != null && dm.TDB > dbmax) dbmax = dm.TDB;
                        if (dm.TDH != null && dm.TDH > dhmax) dhmax = dm.TDH;
                        if (dm.TOG != null && dm.TOG > ogmax) ogmax = dm.TOG;
                        if (dm.TBG != null && dm.TBG > bgmax) bgmax = dm.TBG;
                        if (dm.TFL != null && dm.TFL > tflmax) tflmax = dm.TFL;
                        if (dm.CFL != null && dm.CFL > cflmax) cflmax = dm.CFL;
                    }
                }
            }
            dbmax = dbmax.ToString().Length;
            dhmax = dhmax.ToString().Length;
            ogmax = ogmax.ToString().Length;
            bgmax = bgmax.ToString().Length;
            tflmax = tflmax.ToString().Length;
            cflmax = cflmax.ToString().Length;

            int pad1 = "D_code[] ".Length + defmax;
            int pad2 = "= \"\"; ".Length + namemax;
            int pad3 = "TDB_max[] ".Length + defmax;
            int pad4 = "= ; ".Length + Math.Max(dbmax == null ? 0 : (int)dbmax, bgmax == null ? 0 : (int)bgmax);
            int pad5 = "TDH_max[] ".Length + defmax;
            int pad6 = pad1 + pad2;

            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.Detectoren?.Count > 0)
                {
                    foreach (DetectorModel dm in fcm.Detectoren)
                    {
                        sb.Append("    ");
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
                            sb.Append("    ");
                            sb.Append("".PadLeft(pad6));
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
                        }

                        if (dm.TFL != null || dm.CFL != null)
                        {
                            sb.Append("    ");
                            sb.Append("".PadLeft(pad6));
                            if (dm.TFL != null)
                            {
                                sb.Append($"TFL_max[{dm.GetDefine()}] ".PadRight(pad3));
                                sb.Append($"= {dm.TFL}; ".PadRight(pad4));
                            }
                            if (dm.CFL != null)
                            {
                                sb.Append($"CFL_max[{dm.GetDefine()}] ".PadRight(pad5));
                                sb.AppendLine($"= {dm.CFL};");
                            }
                        }
                    }
                }
            }

            return sb.ToString();
        }

        private string GenerateTabCControlParametersIngangen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* overige ingangen */");
            sb.AppendLine("/* ---------------- */");

            sb.Append(GetAllElementsTabCLines(Ingangen));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersHulpElementen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* hulp elementen */");
            sb.AppendLine("/* -------------- */");

            sb.Append(GetAllElementsTabCLines(HulpElementen));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersGeheugenElementen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* geheugen elementen */");
            sb.AppendLine("/* ------------------ */");
            
            sb.Append(GetAllElementsTabCLines(GeheugenElementen));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersTijdElementen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* tijd elementen */");
            sb.AppendLine("/* -------------- */");

            sb.Append(GetAllElementsTabCLines(Timers));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersCounters(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* teller elementen */");
            sb.AppendLine("/* ---------------- */");

            sb.Append(GetAllElementsTabCLines(Counters));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersSchakelaars(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* schakelaars */");
            sb.AppendLine("/* ----------- */");

            sb.Append(GetAllElementsTabCLines(Schakelaars));

            return sb.ToString();
        }


        private string GenerateTabCControlParametersParameters(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* parameters */");
            sb.AppendLine("/* ---------- */");

            sb.Append(GetAllElementsTabCLines(Parameters));

            return sb.ToString();
        }

        private string GenerateTabCControlParametersModulen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* modulen */");
            sb.AppendLine("/* ------- */");

            foreach(ModuleModel mm in controller.ModuleMolen.Modules)
            {
                foreach(ModuleFaseCyclusModel mfcm in mm.Fasen)
                {
                    sb.AppendLine($"{tabspace}PRML[{mm.Naam}][{mfcm.FaseCyclus}] = PRIMAIR;");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
