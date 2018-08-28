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
                sb.AppendLine($"{ts}#include \"to_min.c\"   /* garantie-ontruimingstijden        */");
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

        private string GenerateRegCTop(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"static int fc;");
            sb.AppendLine("mulv TDH_old[DPMAX];");
            sb.AppendLine("mulv DB_old[DPMAX];");
            sb.AppendLine();


            if (controller.Data.CCOLMulti)
            {
                sb.AppendLine($"int CCOL_SLAVE = {controller.Data.CCOLMultiSlave};");
                sb.AppendLine();
            }

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCTop])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCTop, ts));
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

        private string GenerateRegCPreApplication(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void PreApplication(void)");
            sb.AppendLine("{");

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCPreApplication])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCPreApplication, ts));
            }

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

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCKlokPerioden])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCKlokPerioden, ts));
            }

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
            sb.AppendLine();

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCAanvragen])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCAanvragen, ts));
            }

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
                    
                    foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCMaxgroen])
                    {
                        sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCMaxgroen, ts));
                    }

                    // Add file
                    sb.AppendLine($"{ts}Maxgroen_Add();");
                    sb.AppendLine("}");

                    sb.AppendLine();
                    break;

                case GroentijdenTypeEnum.VerlengGroentijden:
                    sb.AppendLine("void Verlenggroen(void)");
                    sb.AppendLine("{");

                    foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCVerlenggroen])
                    {
                        sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCVerlenggroen, ts));
                    }

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

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCWachtgroen])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCWachtgroen, ts));
            }
            sb.AppendLine($"{ts}Wachtgroen_Add();");
	        sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCMeetkriterium(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void Meetkriterium(void)");
            sb.AppendLine("{");
            
            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCMeetkriterium])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCMeetkriterium, ts));
            }
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

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCMeeverlengen])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCMeeverlengen, ts));
            }

            sb.AppendLine($"{ts}Meeverlengen_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCSynchronisaties(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void Synchronisaties(void)");
            sb.AppendLine("{");

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCSynchronisaties])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCSynchronisaties, ts));
            }

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
            sb.AppendLine();
            sb.AppendLine($"{ts}register count fc;");

            sb.AppendLine();
            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCRealisatieAfhandelingModules])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCRealisatieAfhandelingModules, ts));
            }
            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCRealisatieAfhandelingNaModules])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCRealisatieAfhandelingNaModules, ts));
            }

            sb.AppendLine();
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
            
            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCRealisatieAfhandeling])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCRealisatieAfhandeling, ts));
            }
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
            sb.AppendLine();

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCFileVerwerking])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCFileVerwerking, ts));
            }
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
            sb.AppendLine();

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCDetectieStoring])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCDetectieStoring, ts));
            }
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

            sb.AppendLine($"{ts}int i = 0;");
            sb.AppendLine($"{ts}for (; i < DPMAX; ++i)");
            sb.AppendLine($"{ts}{{");
            sb.AppendLine($"{ts}{ts}TDH_old[i] = TDH[i];");
            sb.AppendLine($"{ts}{ts}DB_old[i] = DB[i];");
            sb.AppendLine($"{ts}}}");

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCPostApplication])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCPostApplication, ts));
            }

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


            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCPreSystemApplication])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCPreSystemApplication, ts));
            }

            sb.AppendLine($"{ts}pre_system_application();");
	        if (controller.HalfstarData.IsHalfstar)
	        {
		        sb.AppendLine($"{ts}pre_system_application_halfstar();");
	        }

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCSystemApplication])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCSystemApplication, ts));
            }
            sb.AppendLine();

            if (controller.Data.GarantieOntruimingsTijden)
            {
                sb.AppendLine($"{ts}/* minimumtijden */");
                sb.AppendLine($"{ts}/* ------------- */");
                sb.AppendLine($"{ts}check_to_min();");
            }
            sb.AppendLine($"{ts}check_tgg_min();");
            sb.AppendLine($"{ts}check_tgl_min();");
            sb.AppendLine($"{ts}check_trg_min();");
            sb.AppendLine();

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCPostSystemApplication])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCPostSystemApplication, ts));
            }

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

            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCSystemApplication2])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCSystemApplication2, ts));
            }

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
            foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.RegCSpecialSignals])
            {
                sb.Append(gen.Value.GetCode(controller, CCOLCodeTypeEnum.RegCSpecialSignals, ts));
            }
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
