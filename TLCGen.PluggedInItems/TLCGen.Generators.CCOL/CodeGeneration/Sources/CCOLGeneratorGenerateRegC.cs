using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateRegC(ControllerModel controller)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* REGELPROGRAMMA */");
            sb.AppendLine("/* -------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "reg.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.AppendLine("#define REG (CIF_WPS[CIF_PROG_STATUS] == CIF_STAT_REG)");
            if(controller.InterSignaalGroep?.Nalopen?.Count > 0)
            {
                sb.AppendLine("#define NALOPEN");
            }
            sb.AppendLine();
            sb.Append(GenerateRegCIncludes(controller));
            sb.Append(GenerateRegCTop(controller));
            if (controller.Data.KWCType != KWCTypeEnum.Geen && controller.Data.KWCUitgebreid)
            {
                sb.AppendLine();
                sb.Append(GenerateRegCKwcApplication(controller));
            }
            sb.Append(GenerateRegCPreApplication(controller));
            sb.Append(GenerateRegCKlokPerioden(controller));
            sb.Append(GenerateRegCAanvragen(controller));
            sb.Append(GenerateRegCMaxOfVerlenggroen(controller));
            sb.Append(GenerateRegCWachtgroen(controller));
            sb.Append(GenerateRegCMeetkriterium(controller));
            sb.Append(GenerateRegCMeeverlengen(controller));
            sb.Append(GenerateRegCSynchronisaties(controller));
            sb.Append(GenerateRegCRealisatieAfhandeling(controller));
            sb.Append(GenerateRegCFileVerwerking(controller));
            sb.Append(GenerateRegCDetectieStoring(controller));
            sb.Append(GenerateRegCInitApplication(controller));
            sb.Append(GenerateRegCPostApplication(controller));
            sb.Append(GenerateRegCApplication(controller));
            sb.Append(GenerateRegCSystemApplication(controller));
            if(controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL9)
            {
                sb.Append(GenerateRegCSystemApplication2(controller));
            }
            sb.Append(GenerateRegCDumpApplication(controller));
            sb.Append(GenerateRegCSpecialSignals(controller));

            return sb.ToString();
        }

        private string GenerateRegCIncludes(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* include files */");
            sb.AppendLine("/* ------------- */");
            sb.AppendLine($"{ts}#include \"{controller.Data.Naam}sys.h\"");
            sb.AppendLine($"{ts}#include \"fcvar.c\"    /* fasecycli                         */");
            sb.AppendLine($"{ts}#include \"kfvar.c\"    /* conflicten                        */");
            sb.AppendLine($"{ts}#include \"usvar.c\"    /* uitgangs elementen                */");
            sb.AppendLine($"{ts}#include \"dpvar.c\"    /* detectie elementen                */");
            if (controller.Data.GarantieOntruimingsTijden)
            {
                if(controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && controller.Data.Intergroen)
                {
                    sb.AppendLine($"{ts}#include \"tig_min.c\"   /* garantie-ontruimingstijden        */");
                }
                else
                {
                    sb.AppendLine($"{ts}#include \"to_min.c\"   /* garantie-ontruimingstijden        */");
                }
            }
            sb.AppendLine($"{ts}#include \"trg_min.c\"  /* garantie-roodtijden               */");
            sb.AppendLine($"{ts}#include \"tgg_min.c\"  /* garantie-groentijden              */");
            sb.AppendLine($"{ts}#include \"tgl_min.c\"  /* garantie-geeltijden               */");
            sb.AppendLine($"{ts}#include \"isvar.c\"    /* ingangs elementen                 */");
            if (controller.HasDSI())
            {
                sb.AppendLine($"{ts}#include \"dsivar.c\"   /* selectieve detectie               */");
            }
            sb.AppendLine($"{ts}#include \"hevar.c\"    /* hulp elementen                    */");
            sb.AppendLine($"{ts}#include \"mevar.c\"    /* geheugen elementen                */");
            sb.AppendLine($"{ts}#include \"tmvar.c\"    /* tijd elementen                    */");
            sb.AppendLine($"{ts}#include \"ctvar.c\"    /* teller elementen                  */");
            sb.AppendLine($"{ts}#include \"schvar.c\"   /* software schakelaars              */");
	        if (controller.HalfstarData.IsHalfstar)
	        {
				sb.AppendLine($"{ts}#include \"tigvar.c\"   /* uitgebreide signaalplan structuur */");
				sb.AppendLine($"{ts}#include \"plevar.c\"   /* uitgebreide signaalplan structuur */");
	        }
            sb.AppendLine($"{ts}#include \"prmvar.c\"   /* parameters                        */");
            sb.AppendLine($"{ts}#include \"lwmlvar.c\"  /* langstwachtende modulen structuur */");
            if(controller.Data.VLOGType != VLOGTypeEnum.Geen)
            {
                sb.AppendLine($"{ts}#ifndef NO_VLOG");
                sb.AppendLine($"{ts}{ts}#include \"vlogvar.c\"  /* variabelen t.b.v. vlogfuncties                */");
                sb.AppendLine($"{ts}{ts}#include \"logvar.c\"   /* variabelen t.b.v. logging                     */");
                sb.AppendLine($"{ts}{ts}#include \"monvar.c\"   /* variabelen t.b.v. realtime monitoring         */");
                sb.AppendLine($"{ts}{ts}#include \"fbericht.h\"");
                sb.AppendLine($"{ts}#endif");
            }
            sb.AppendLine($"{ts}#include \"prsvar.c\"   /* parameters parser                 */");
            sb.AppendLine($"{ts}#include \"control.c\"  /* controller interface              */");
            sb.AppendLine($"{ts}#include \"rtappl.h\"   /* applicatie routines               */");
            sb.AppendLine($"{ts}#include \"stdfunc.h\"  /* standaard functies                */");
            sb.AppendLine($"{ts}#include \"extra_func.c\" /* extra standaard functies        */");
            if(controller.OVData.OVIngrepen.Count > 0 || controller.OVData.HDIngrepen.Count > 0)
            {
                sb.AppendLine($"{ts}#include \"ov.h\"       /* ov-afhandeling                    */");
                sb.AppendLine($"{ts}#include \"extra_func_ov.c\" /* extra standaard functies OV     */");
            }
            sb.AppendLine();
            sb.AppendLine("#ifndef AUTOMAAT");
            sb.AppendLine("/*    #include \"ccdump.inc\" */");
            sb.AppendLine($"{ts}#include \"keysdef.c\"     /* Definitie toetsenbord t.b.v. stuffkey  */");
            sb.AppendLine($"{ts}#if !defined (_DEBUG)");
            sb.AppendLine($"{ts}{ts}#include \"xyprintf.h\" /* Printen debuginfo                      */");
            sb.AppendLine($"{ts}#endif");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine($"{ts}#include \"detectie.c\"");
            sb.AppendLine($"{ts}#include \"ccolfunc.c\"");
            if (controller.InterSignaalGroep.Voorstarten.Any() || controller.InterSignaalGroep.Gelijkstarten.Any())
            {
                sb.AppendLine($"{ts}#include \"syncvar.c\"  /* synchronisatie functies           */");
            }

	        if (controller.HalfstarData.IsHalfstar)
	        {
		        sb.AppendLine($"{ts}#include \"{controller.Data.Naam}hst.c\"");
	        }
            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCIncludes])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCIncludes, ts));
            }
            sb.AppendLine($"{ts}#include \"{controller.Data.Naam}reg.add\"");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCTop(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine("mulv TDH_old[DPMAX];");
            sb.AppendLine("mulv DB_old[DPMAX];");
            if (c.GetAllDetectors(x => x.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit).Any())
            {
                sb.AppendLine("mulv DVG[DPMAX]; /* T.b.v. meting veiligheidsgroen */");
            }
            sb.AppendLine();


            if (c.Data.CCOLMulti)
            {
                sb.AppendLine($"s_int16 CCOL_SLAVE = {c.Data.CCOLMultiSlave};");
                sb.AppendLine();
            }

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCTop])
            {
                sb.Append(gen.Value.GetCode(c, CCOLCodeTypeEnum.RegCTop, ts));
            }
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCKwcApplication(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void KwcApplication(void)");
            sb.AppendLine("{");

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCKwcApplication])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCKwcApplication, ts));
            }
            
            sb.AppendLine($"{ts}KwcApplication_Add();");
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        //private void GetVariables(StringBuilder sb, CCOLCodeTypeEnum type, ControllerModel controller)
        //{
        //    if (CCOLElementCollector.FunctionLocalVariables.ContainsKey(type))
        //    {
        //        foreach (var i in CCOLElementCollector.FunctionLocalVariables[type])
        //        {
        //            sb.AppendLine($"{ts}{i.Item1} {i.Item2};");
        //        }
        //        sb.AppendLine();
        //    }
        //}
        //
        //private void GetVariablesAndCode(StringBuilder sb, CCOLCodeTypeEnum type, ControllerModel controller)
        //{
        //    if (CCOLElementCollector.FunctionLocalVariables.ContainsKey(type))
        //    {
        //        foreach (var i in CCOLElementCollector.FunctionLocalVariables[type])
        //        {
        //            sb.AppendLine($"{ts}{i.Item1} {i.Item2};");
        //        }
        //        sb.AppendLine();
        //    }
        //
        //    foreach (var gen in OrderedPieceGenerators[type])
        //    {
        //        sb.Append(gen.Value.GetCode(controller, type, ts));
        //    }
        //}
        //
        //
        //private void GetCode(StringBuilder sb, CCOLCodeTypeEnum type, ControllerModel controller)
        //{
        //    foreach (var gen in OrderedPieceGenerators[type])
        //    {
        //        sb.Append(gen.Value.GetCode(controller, type, ts));
        //    }
        //}

        private string GenerateRegCPreApplication(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void PreApplication(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCPreApplication, true, true, false, true);

            sb.AppendLine($"{ts}PreApplication_Add();");

	        if (controller.HalfstarData.IsHalfstar)
	        {
		        sb.AppendLine($"{ts}pre_application_halfstar();");
	        }
	        sb.AppendLine();

	        if (controller.HalfstarData.IsHalfstar || controller.Fasen.Any(x => x.WachttijdVoorspeller))
	        {
		        sb.AppendLine($"{ts}/* Genereren knippersignalen */");
		        sb.AppendLine($"{ts}UpdateKnipperSignalen();");
	        }

	        sb.AppendLine("}");
	        sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCKlokPerioden(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void KlokPerioden(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCKlokPerioden, true, true, false, true);

            sb.AppendLine($"{ts}KlokPerioden_Add();");
            
	        if (controller.HalfstarData.IsHalfstar)
	        {
		        sb.AppendLine($"{ts}KlokPerioden_halfstar();");
	        }

	        sb.AppendLine("}");
	        sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCAanvragen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void Aanvragen(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCAanvragen, true, true, false, true);

            sb.AppendLine($"{ts}Aanvragen_Add();");
			
	        if (controller.HalfstarData.IsHalfstar)
	        {
		        sb.AppendLine($"{ts}Aanvragen_halfstar();");
	        }

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCMaxOfVerlenggroen(ControllerModel controller)
        {
            var sb = new StringBuilder();

            var _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");

            switch (controller.Data.TypeGroentijden)
            {
                case GroentijdenTypeEnum.MaxGroentijden:
                    sb.AppendLine("void Maxgroen(void)");
                    sb.AppendLine("{");

                    AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCMaxgroen, true, true, false, true);

                    // Add file
                    sb.AppendLine($"{ts}Maxgroen_Add();");
                    sb.AppendLine("}");

                    sb.AppendLine();
                    break;

                case GroentijdenTypeEnum.VerlengGroentijden:
                    sb.AppendLine("void Verlenggroen(void)");
                    sb.AppendLine("{");

                    AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCVerlenggroen, true, true, false, true);

                    // Add file
                    sb.AppendLine($"{ts}Maxgroen_Add();");
                    sb.AppendLine("}");

                    sb.AppendLine();
                    break;
            }
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCWachtgroen(ControllerModel controller)
        {
            var sb = new StringBuilder();
            var _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");

            sb.AppendLine("void Wachtgroen(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCWachtgroen, true, true, false, true);

            sb.AppendLine($"{ts}Wachtgroen_Add();");
	        sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCMeetkriterium(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void Meetkriterium(void)");
            sb.AppendLine("{");
            
            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCMeetkriterium, true, true, false, true);

            sb.AppendLine($"{ts}Meetkriterium_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCMeeverlengen(ControllerModel controller)
        {
            var sb = new StringBuilder();
            var _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");

            sb.AppendLine("void Meeverlengen(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCMeeverlengen, true, true, false, true);

            sb.AppendLine($"{ts}Meeverlengen_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCSynchronisaties(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void Synchronisaties(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCSynchronisaties, true, true, false, true);

            sb.AppendLine($"{ts}Synchronisaties_Add();");
	        sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCRealisatieAfhandeling(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void RealisatieAfhandeling(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCRealisatieAfhandeling, true, false, false, true);
            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCRealisatieAfhandelingModules, true, false, false, true);
            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCRealisatieAfhandelingNaModules, true, false, false, true);
            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCRealisatieAfhandelingModules, false, true, false, true);
            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCRealisatieAfhandelingNaModules, false, true, false, true);
            
            if(controller.InterSignaalGroep.Nalopen.Any())
            {
                sb.AppendLine($"{ts}YML[ML] = yml_cv_pr_nl(PRML, ML, ML_MAX);");
            }
            else
            {
              sb.AppendLine($"{ts}YML[ML] = yml_cv_pr(PRML, ML, ML_MAX);");
            }
            sb.AppendLine();
            foreach(var mm in controller.ModuleMolen.Modules)
            {
                if(mm.Naam == controller.ModuleMolen.WachtModule)
                    sb.AppendLine($"{ts}YML[{mm.Naam}] |= yml_wml(PRML, ML_MAX);");
                else
                    sb.AppendLine($"{ts}YML[{mm.Naam}] |= FALSE;");
            }
            sb.AppendLine();
            sb.AppendLine($"{ts}Modules_Add();");
	        sb.AppendLine();
            sb.AppendLine($"{ts}SML = modules(ML_MAX, PRML, YML, &ML);");
            sb.AppendLine();
            sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
            sb.AppendLine($"{ts}" + "{");
            sb.AppendLine($"{ts}{ts}YM[fc] &= ~BIT5;");
            sb.AppendLine($"{ts}{ts}YM[fc] |= SML && PG[fc] ? BIT5 : FALSE;");
            sb.AppendLine($"{ts}" + "}");
            sb.AppendLine();

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCRealisatieAfhandeling, false, true, false, true);

            sb.AppendLine($"{ts}RealisatieAfhandeling_Add();");
	        sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCFileVerwerking(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void FileVerwerking(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCFileVerwerking, true, true, false, true);

            sb.AppendLine($"{ts}FileVerwerking_Add();");
	        sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCDetectieStoring(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void DetectieStoring(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCDetectieStoring, true, true, false, true);

            sb.AppendLine($"{ts}DetectieStoring_Add();");
	        sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCInitApplication(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void init_application(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCInitApplication, true, false, false, true);

            sb.AppendLine("#if !defined AUTOMAAT && !defined VISSIM");
            sb.AppendLine($"{ts}if (!SAPPLPROG)");
            sb.AppendLine($"{ts}{ts}stuffkey(CTRLF4KEY);");
            sb.AppendLine("#endif");
            sb.AppendLine("");
            if (controller.InterSignaalGroep.Voorstarten.Count > 0 ||
               controller.InterSignaalGroep.Gelijkstarten.Count > 0)
            {
                sb.AppendLine($"{ts}init_realisation_timers();");
                sb.AppendLine("");
            }

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCInitApplication])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCInitApplication, ts));
            }
            sb.AppendLine($"{ts}post_init_application();");
	        if (controller.HalfstarData.IsHalfstar)
	        {
		        sb.AppendLine($"{ts}post_init_application_halfstar();");
	        }
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCApplication(ControllerModel controller)
        {
            var sb = new StringBuilder();
            var _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");

            sb.AppendLine("void application(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}PreApplication();");
            sb.AppendLine();
            sb.AppendLine($"{ts}TFB_max = PRM[prmfb];");
            sb.AppendLine($"{ts}KlokPerioden();");
            sb.AppendLine($"{ts}Aanvragen();");

            var hsts = controller.HalfstarData.IsHalfstar ? ts + ts : ts;

            if (controller.HalfstarData.IsHalfstar)
            {
                sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}])");
                sb.AppendLine($"{ts}{{");
                switch (controller.Data.TypeGroentijden)
                {
                    case GroentijdenTypeEnum.MaxGroentijden:
                        sb.AppendLine($"{hsts}Maxgroen_halfstar();");
                        break;
                    case GroentijdenTypeEnum.VerlengGroentijden:
                        sb.AppendLine($"{hsts}Verlenggroen_halfstar();");
                        break;
                }
                sb.AppendLine($"{hsts}Wachtgroen_halfstar();");
                sb.AppendLine($"{hsts}Meetkriterium_halfstar();");
                sb.AppendLine($"{hsts}Meeverlengen_halfstar();");
                sb.AppendLine($"{hsts}Synchronisaties_halfstar();");
                sb.AppendLine($"{hsts}RealisatieAfhandeling_halfstar();");
                sb.AppendLine($"{hsts}Alternatief_halfstar();");
                sb.AppendLine($"{hsts}FileVerwerking_halfstar();");
                sb.AppendLine($"{hsts}DetectieStoring_halfstar();");
                sb.AppendLine($"{ts}}}");
                sb.AppendLine($"{ts}else");
                sb.AppendLine($"{ts}{{");
            }

            switch (controller.Data.TypeGroentijden)
            {
                case GroentijdenTypeEnum.MaxGroentijden:
                    sb.AppendLine($"{hsts}Maxgroen();");
                    break;
                case GroentijdenTypeEnum.VerlengGroentijden:
                    sb.AppendLine($"{hsts}Verlenggroen();");
                    break;
            }
            sb.AppendLine($"{hsts}Wachtgroen();");
            sb.AppendLine($"{hsts}Meetkriterium();");
            sb.AppendLine($"{hsts}Meeverlengen();");
            sb.AppendLine($"{hsts}Synchronisaties();");
            sb.AppendLine($"{hsts}RealisatieAfhandeling();");
            sb.AppendLine($"{hsts}FileVerwerking();");
            sb.AppendLine($"{hsts}DetectieStoring();");

            if (controller.HalfstarData.IsHalfstar)
            {
                sb.AppendLine($"{ts}}}");
            }

            if (controller.OVData.OVIngrepen.Count > 0 ||
                controller.OVData.HDIngrepen.Count > 0)
            {
                sb.AppendLine($"{ts}AfhandelingOV();");
            }
            if (controller.Data.FixatieData.FixatieMogelijk)
            {
                sb.AppendLine($"{ts}Fixatie(isfix, 0, FCMAX-1, SCH[schbmfix], PRML, ML);");
            }
            sb.AppendLine("");
            sb.AppendLine($"{ts}PostApplication();");
            if (controller.Data.KWCType != KWCTypeEnum.Geen && controller.Data.KWCUitgebreid)
            {
                sb.AppendLine($"{ts}KwcApplication();");
            }
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCPostApplication(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void PostApplication(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCPostApplication, true, false, false, true);
            
            sb.AppendLine($"{ts}int i = 0;");
            sb.AppendLine($"{ts}for (; i < DPMAX; ++i)");
            sb.AppendLine($"{ts}{{");
            sb.AppendLine($"{ts}{ts}TDH_old[i] = TDH[i];");
            sb.AppendLine($"{ts}{ts}DB_old[i] = DB[i];");
            sb.AppendLine($"{ts}}}");
            sb.AppendLine();

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCPostApplication, false, true, false, true);
            
            sb.AppendLine($"{ts}PostApplication_Add();");
	        if (controller.HalfstarData.IsHalfstar)
	        {
		        sb.AppendLine($"{ts}PostApplication_halfstar();");
	        }
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCSystemApplication(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void system_application(void)");
            sb.AppendLine("{");


            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCPreSystemApplication, true, false, false, true);
            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCPostApplication, true, false, false, true);
            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCPostSystemApplication, true, false, false, true);
            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCPreSystemApplication, false, true, false, true);
            
            sb.AppendLine($"{ts}pre_system_application();");
	        if (controller.HalfstarData.IsHalfstar)
	        {
		        sb.AppendLine($"{ts}pre_system_application_halfstar();");
	        }
            sb.AppendLine();

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCSystemApplication, false, true, false, true);

            if (controller.Data.GarantieOntruimingsTijden)
            {
                sb.AppendLine($"{ts}/* minimumtijden */");
                sb.AppendLine($"{ts}/* ------------- */");
                if (controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && controller.Data.Intergroen)
                {
                    sb.AppendLine($"{ts}check_tig_min();");
                }
                else
                {
                    sb.AppendLine($"{ts}check_to_min();");
                }
            }
            sb.AppendLine($"{ts}check_tgg_min();");
            sb.AppendLine($"{ts}check_tgl_min();");
            sb.AppendLine($"{ts}check_trg_min();");
            sb.AppendLine();

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCPostSystemApplication, false, true, false, true);

            sb.AppendLine($"{ts}post_system_application();");
	        if (controller.HalfstarData.IsHalfstar)
	        {
		        sb.AppendLine($"{ts}post_system_application_halfstar();");
	        }
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }


        private string GenerateRegCSystemApplication2(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void system_application2(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCSystemApplication2, true, true, false, true);

            sb.AppendLine();
            sb.AppendLine($"{ts}post_system_application2();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCDumpApplication(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#define ENDDUMP 21");
            sb.AppendLine("");
            sb.AppendLine("void dump_application(void)");
            sb.AppendLine("{");
            //sb.AppendLine("    if (!EXTRADUMP)");
            //sb.AppendLine("      EXTRADUMP = 1;");
            //sb.AppendLine("");
            //sb.AppendLine($"{tabspace}switch (EXTRADUMP)");
            //sb.AppendLine($"{tabspace}" + "{");
            //sb.AppendLine($"{tabspace}case 1: /* dump_realisation timers */");
            //sb.AppendLine($"{tabspace}{tabspace}if (!SYNCDUMP)");
            //sb.AppendLine($"{tabspace}{tabspace}" + "{");
            //sb.AppendLine($"{tabspace}{tabspace}{tabspace}DUMP = ENDDUMP;");
            //sb.AppendLine($"{tabspace}{tabspace}{tabspace}SYNCDUMP = 1;");
            //sb.AppendLine($"{tabspace}{tabspace}" + "}");
            //sb.AppendLine($"{tabspace}dump_realisation_timers();");
            //sb.AppendLine($"{tabspace}if (!SYNCDUMP)");
            //sb.AppendLine($"{tabspace}{tabspace}EXTRADUMP++;");
            //sb.AppendLine($"{tabspace}else");
            //sb.AppendLine($"{tabspace}" + "{");
            //sb.AppendLine($"{tabspace}{tabspace}DUMP = ENDDUMP;");
            //sb.AppendLine($"{tabspace}{tabspace}break;");
            //sb.AppendLine($"{tabspace}" + "}");
            //sb.AppendLine($"{tabspace}case 2: /* dump applicatiegegevens */");
            //sb.AppendLine($"{tabspace}{tabspace}if (!waitterm((mulv) 30)) uber_puts(\"\nPlaats: {controller.Data.Stad}\n\");");
            //sb.AppendLine($"{tabspace}{tabspace}if (!waitterm((mulv) 25)) uber_puts(\"Kruispunt: {controller.Data.Naam}\n\");");
            //sb.AppendLine($"{tabspace}{tabspace}if (!waitterm((mulv) 30)) uber_puts(\"         : {controller.Data.Straat1}\n\");");
            //sb.AppendLine($"{tabspace}{tabspace}if (!waitterm((mulv) 30)) uber_puts(\"         : {controller.Data.Straat2}\n\");");
            //sb.AppendLine($"{tabspace}{tabspace}if (!waitterm((mulv) 25)) uber_puts(\"Werkadres: {controller.Data.Naam}\n\n\");");
            //sb.AppendLine($"{tabspace}{tabspace}EXTRADUMP = 0;");
            //sb.AppendLine($"{tabspace}{tabspace}break;");
            //sb.AppendLine($"{tabspace}default:");
            //sb.AppendLine($"{tabspace}{tabspace}break;");
            //sb.AppendLine($"{tabspace}" + "}");
            sb.AppendLine("");
            sb.AppendLine($"{ts}post_dump_application();");
	        if (controller.HalfstarData.IsHalfstar)
	        {
		        sb.AppendLine($"{ts}post_dump_application_halfstar();");
	        }
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCSpecialSignals(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#ifdef CCOL_IS_SPECIAL");
            sb.AppendLine("void OVSpecialSignals();");
            sb.AppendLine("void is_special_signals(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(controller, sb, CCOLCodeTypeEnum.RegCSpecialSignals, true, true, false, true);

            if (controller.OVData.OVIngrepen.Any() ||
				controller.OVData.HDIngrepen.Any())
		    {
				sb.AppendLine($"{ts}OVSpecialSignals();");
			}
            sb.AppendLine($"{ts}SpecialSignals_Add();");
            sb.AppendLine("}");
            sb.AppendLine("#endif");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
