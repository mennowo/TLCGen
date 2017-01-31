using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateRegC(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* REGELPROGRAMMA */");
            sb.AppendLine("/* -------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "reg.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.AppendLine("#define REG (CIF_WPS[CIF_PROG_STATUS] == CIF_STAT_REG)");
            sb.AppendLine();
            sb.Append(GenerateRegCIncludes(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCTop(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCKlokPerioden(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCAanvragen(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCMaxgroen(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCWachtgroen(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCMeetkriterium(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCMeeverlengen(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCRealisatieAfhandeling(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCInitApplication(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCApplication(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCSystemApplication(controller));
            sb.AppendLine();
            sb.Append(GenerateRegCDumpApplication(controller));
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCIncludes(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* include files */");
            sb.AppendLine("/* ------------- */");
            sb.AppendLine($"{ts}#include \"{controller.Data.Naam}sys.h\"");
#warning TODO - make includes dependent on the kind of controller and its settings
            sb.AppendLine($"{ts}#include \"fcvar.c\"    /* fasecycli                         */");
            sb.AppendLine($"{ts}#include \"kfvar.c\"    /* conflicten                        */");
            sb.AppendLine($"{ts}#include \"usvar.c\"    /* uitgangs elementen                */");
            sb.AppendLine($"{ts}#include \"dpvar.c\"    /* detectie elementen                */");
            sb.AppendLine($"{ts}#include \"to_min.c\"   /* garantie-ontruimingstijden        */");
            sb.AppendLine($"{ts}#include \"trg_min.c\"  /* garantie-roodtijden               */");
            sb.AppendLine($"{ts}#include \"tgg_min.c\"  /* garantie-groentijden              */");
            sb.AppendLine($"{ts}#include \"tgl_min.c\"  /* garantie-geeltijden               */");
            sb.AppendLine($"{ts}#include \"isvar.c\"    /* ingangs elementen                 */");
            sb.AppendLine($"{ts}#include \"dsivar.c\"   /* selectieve detectie               */");
            sb.AppendLine($"{ts}#include \"hevar.c\"    /* hulp elementen                    */");
            sb.AppendLine($"{ts}#include \"mevar.c\"    /* geheugen elementen                */");
            sb.AppendLine($"{ts}#include \"tmvar.c\"    /* tijd elementen                    */");
            sb.AppendLine($"{ts}#include \"ctvar.c\"    /* teller elementen                  */");
            sb.AppendLine($"{ts}#include \"schvar.c\"   /* software schakelaars              */");
            sb.AppendLine($"{ts}#include \"prmvar.c\"   /* parameters                        */");
            sb.AppendLine($"{ts}#include \"lwmlvar.c\"  /* langstwachtende modulen structuur */");
            sb.AppendLine($"{ts}#ifndef NO_VLOG");
            sb.AppendLine($"{ts}{ts}#include \"vlogvar.c\"  /* variabelen t.b.v. vlogfuncties                */");
            sb.AppendLine($"{ts}{ts}#include \"logvar.c\"   /* variabelen t.b.v. logging                     */");
            sb.AppendLine($"{ts}{ts}#include \"monvar.c\"   /* variabelen t.b.v. realtime monitoring         */");
            sb.AppendLine($"{ts}{ts}#include \"fbericht.h\"");
            sb.AppendLine($"{ts}#endif");
            sb.AppendLine($"{ts}#include \"prsvar.c\"   /* parameters parser                 */");
            sb.AppendLine($"{ts}#include \"control.c\"  /* controller interface              */");
            sb.AppendLine($"{ts}#include \"rtappl.h\"   /* applicatie routines               */");
            sb.AppendLine($"{ts}#include \"stdfunc.h\"  /* standaard functies                */");
            sb.AppendLine($"{ts}#include \"extra_func.c\" /* extra standaard functies        */");
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
            foreach (var gen in _PieceGenerators)
            {
                if (gen.HasCode(CCOLRegCCodeTypeEnum.Includes))
                {
                    sb.Append(gen.GetCode(controller, CCOLRegCCodeTypeEnum.Includes, ts));
                }
            }
            sb.AppendLine($"{ts}#include \"{controller.Data.Naam}reg.add\"");

            return sb.ToString();
        }

        private string GenerateRegCTop(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var gen in _PieceGenerators)
            {
                if (gen.HasCode(CCOLRegCCodeTypeEnum.Top))
                {
                    sb.Append(gen.GetCode(controller, CCOLRegCCodeTypeEnum.Top, ts));
                }
            }

            return sb.ToString();
        }

        private string GenerateRegCKlokPerioden(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var gen in _PieceGenerators)
            {
                if (gen.HasCode(CCOLRegCCodeTypeEnum.KlokPerioden))
                {
                    sb.Append(gen.GetCode(controller, CCOLRegCCodeTypeEnum.KlokPerioden, ts));
                }
            }

            return sb.ToString();
        }

        private string GenerateRegCAanvragen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void Aanvragen(void)");
            sb.AppendLine("{");
            sb.AppendLine();

            // Code from funcionality code piece gens
            foreach(var gen in _PieceGenerators)
            {
                if(gen.HasCode(CCOLRegCCodeTypeEnum.Aanvragen))
                {
                    sb.Append(gen.GetCode(controller, CCOLRegCCodeTypeEnum.Aanvragen, ts));
                }
            }
            
            // Add file
            sb.AppendLine($"{ts}Aanvragen_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCMaxgroen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void Maxgroen(void)");
            sb.AppendLine("{");
            foreach (var gen in _PieceGenerators)
            {
                if (gen.HasCode(CCOLRegCCodeTypeEnum.Maxgroen))
                {
                    sb.Append(gen.GetCode(controller, CCOLRegCCodeTypeEnum.Maxgroen, ts));
                }
            }

            // Add file
            sb.AppendLine($"{ts}Maxgroen_Add();");
            sb.AppendLine("}");

            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCWachtgroen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void Wachtgroen(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}register count fc;");
            sb.AppendLine();
            foreach (var gen in _PieceGenerators)
            {
                if (gen.HasCode(CCOLRegCCodeTypeEnum.Wachtgroen))
                {
                    sb.Append(gen.GetCode(controller, CCOLRegCCodeTypeEnum.Wachtgroen, ts));
                }
            }
            sb.AppendLine($"{ts}Wachtgroen_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCMeetkriterium(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void Meetkriterium(void)");
            sb.AppendLine("{");
            sb.AppendLine();
            foreach (var gen in _PieceGenerators)
            {
                if (gen.HasCode(CCOLRegCCodeTypeEnum.Meetkriterium))
                {
                    sb.Append(gen.GetCode(controller, CCOLRegCCodeTypeEnum.Meetkriterium, ts));
                }
            }
            sb.AppendLine($"{ts}Meetkriterium_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCMeeverlengen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void Meeverlengen(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}register count fc;");

            sb.AppendLine();
            foreach (var gen in _PieceGenerators)
            {
                if (gen.HasCode(CCOLRegCCodeTypeEnum.Meeverlengen))
                {
                    sb.Append(gen.GetCode(controller, CCOLRegCCodeTypeEnum.Meeverlengen, ts));
                }
            }

            sb.AppendLine($"{ts}Meeverlengen_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCRealisatieAfhandeling(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void RealisatieAfhandeling(void)");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine($"{ts}register count fc;");

            sb.AppendLine();
            foreach (var gen in _PieceGenerators)
            {
                if (gen.HasCode(CCOLRegCCodeTypeEnum.RealisatieAfhandelingModules))
                {
                    sb.Append(gen.GetCode(controller, CCOLRegCCodeTypeEnum.RealisatieAfhandelingModules, ts));
                }
            }

            sb.AppendLine();
            sb.AppendLine($"{ts}YML[ML] = yml_cv_pr(PRML, ML, ML_MAX);");
            sb.AppendLine();
            foreach(ModuleModel mm in controller.ModuleMolen.Modules)
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
            foreach (var gen in _PieceGenerators)
            {
                if (gen.HasCode(CCOLRegCCodeTypeEnum.RealisatieAfhandeling))
                {
                    sb.Append(gen.GetCode(controller, CCOLRegCCodeTypeEnum.RealisatieAfhandeling, ts));
                }
            }
            sb.AppendLine($"{ts}RealisatieAfhandeling_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCInitApplication(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void init_application(void)");
            sb.AppendLine("{");
            sb.AppendLine("#ifndef AUTOMAAT");
            sb.AppendLine($"{ts}if (!SAPPLPROG)");
            sb.AppendLine($"{ts}{ts}stuffkey(CTRLF4KEY);");
            sb.AppendLine("#endif");
            sb.AppendLine("");
            sb.AppendLine($"{ts}post_init_application();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCApplication(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void application(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}pre_application();");
            sb.AppendLine();
            sb.AppendLine($"{ts}TFB_max = PRM[prmfb];");
            sb.AppendLine($"{ts}KlokPerioden();");
            sb.AppendLine($"{ts}Aanvragen();");
            sb.AppendLine($"{ts}Maxgroen();");
            sb.AppendLine($"{ts}Wachtgroen();");
            sb.AppendLine($"{ts}Meetkriterium();");
            sb.AppendLine($"{ts}Meeverlengen();");
            sb.AppendLine($"{ts}RealisatieAfhandeling();");
            sb.AppendLine($"{ts}Fixatie(isfix, 0, FCMAX-1, SCH[schbmfix], PRML, ML);");
            sb.AppendLine("");
            sb.AppendLine($"{ts}post_application();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCSystemApplication(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void system_application(void)");
            sb.AppendLine("{");

            foreach (var gen in _PieceGenerators)
            {
                if (gen.HasCode(CCOLRegCCodeTypeEnum.PreSystemApplication))
                {
                    sb.Append(gen.GetCode(controller, CCOLRegCCodeTypeEnum.PreSystemApplication, ts));
                }
            }

            sb.AppendLine($"{ts}pre_system_application();");

            foreach (var gen in _PieceGenerators)
            {
                if (gen.HasCode(CCOLRegCCodeTypeEnum.SystemApplication))
                {
                    sb.Append(gen.GetCode(controller, CCOLRegCCodeTypeEnum.SystemApplication, ts));
                }
            }

            sb.AppendLine();
            sb.AppendLine($"{ts}SegmentSturing(ML+1, ussegm1, ussegm2, ussegm3, ussegm4, ussegm5, ussegm6, ussegm7);");
            sb.AppendLine();

            foreach (var gen in _PieceGenerators)
            {
                if (gen.HasCode(CCOLRegCCodeTypeEnum.PostSystemApplication))
                {
                    sb.Append(gen.GetCode(controller, CCOLRegCCodeTypeEnum.PostSystemApplication, ts));
                }
            }


            sb.AppendLine($"{ts}post_system_application();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCDumpApplication(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("#define ENDDUMP   21");
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
            sb.AppendLine("}");


            return sb.ToString();
        }
    }
}
