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
            sb.AppendLine("#define REG  (CIF_WPS[CIF_PROG_STATUS] == CIF_STAT_REG)");
            sb.AppendLine();
            sb.Append(GenerateRegCIncludes(controller));
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
            sb.AppendLine($"{tabspace}#include \"{controller.Data.Naam}sys.h\"");
#warning TODO - make includes dependent on the kind of controller and its settings
            sb.AppendLine($"{tabspace}#include \"fcvar.c\"    /* fasecycli                         */");
            sb.AppendLine($"{tabspace}#include \"kfvar.c\"    /* conflicten                        */");
            sb.AppendLine($"{tabspace}#include \"usvar.c\"    /* uitgangs elementen                */");
            sb.AppendLine($"{tabspace}#include \"dpvar.c\"    /* detectie elementen                */");
            sb.AppendLine($"{tabspace}#include \"to_min.c\"   /* garantie-ontruimingstijden        */");
            sb.AppendLine($"{tabspace}#include \"trg_min.c\"  /* garantie-roodtijden               */");
            sb.AppendLine($"{tabspace}#include \"tgg_min.c\"  /* garantie-groentijden              */");
            sb.AppendLine($"{tabspace}#include \"tgl_min.c\"  /* garantie-geeltijden               */");
            sb.AppendLine($"{tabspace}#include \"isvar.c\"    /* ingangs elementen                 */");
            sb.AppendLine($"{tabspace}#include \"dsivar.c\"   /* selectieve detectie               */");
            sb.AppendLine($"{tabspace}#include \"hevar.c\"    /* hulp elementen                    */");
            sb.AppendLine($"{tabspace}#include \"mevar.c\"    /* geheugen elementen                */");
            sb.AppendLine($"{tabspace}#include \"tmvar.c\"    /* tijd elementen                    */");
            sb.AppendLine($"{tabspace}#include \"ctvar.c\"    /* teller elementen                  */");
            sb.AppendLine($"{tabspace}#include \"schvar.c\"   /* software schakelaars              */");
            sb.AppendLine($"{tabspace}#include \"prmvar.c\"   /* parameters                        */");
            sb.AppendLine($"{tabspace}#include \"lwmlvar.c\"  /* langstwachtende modulen structuur */");
            sb.AppendLine($"{tabspace}#ifndef NO_VLOG");
            sb.AppendLine($"{tabspace}{tabspace}#include \"vlogvar.c\"  /* variabelen t.b.v. vlogfuncties                */");
            sb.AppendLine($"{tabspace}{tabspace}#include \"logvar.c\"   /* variabelen t.b.v. logging                     */");
            sb.AppendLine($"{tabspace}{tabspace}#include \"monvar.c\"   /* variabelen t.b.v. realtime monitoring         */");
            sb.AppendLine($"{tabspace}{tabspace}#include \"fbericht.h\"");
            sb.AppendLine($"{tabspace}#endif");
            sb.AppendLine($"{tabspace}#include \"prsvar.c\"   /* parameters parser                 */");
            sb.AppendLine($"{tabspace}#include \"control.c\"  /* controller interface              */");
            sb.AppendLine($"{tabspace}#include \"rtappl.h\"   /* applicatie routines               */");
            sb.AppendLine($"{tabspace}#include \"stdfunc.h\"  /* standaard functies                */");
            sb.AppendLine();
            sb.AppendLine("#ifndef AUTOMAAT");
            sb.AppendLine("/*    #include \"ccdump.inc\" */");
            sb.AppendLine($"{tabspace}#include \"keysdef.c\"     /* Definitie toetsenbord t.b.v. stuffkey  */");
            sb.AppendLine($"{tabspace}#if !defined (_DEBUG)");
            sb.AppendLine($"{tabspace}{tabspace}#include \"xyprintf.h\" /* Printen debuginfo                      */");
            sb.AppendLine($"{tabspace}#endif");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}#include \"detectie.c\"");
            sb.AppendLine($"{tabspace}#include \"ccolfunc.c\"");
            sb.AppendLine($"{tabspace}#include \"{controller.Data.Naam}reg.add\"");

            return sb.ToString();
        }

        private string GenerateRegCKlokPerioden(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_PeriodenCodeGenerator.GetCode(controller, CCOLCodeType.KlokPerioden, tabspace));

            return sb.ToString();
        }

        private string GenerateRegCAanvragen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void Aanvragen(void)");
            sb.AppendLine("{");
            sb.AppendLine();

            // Detectie aanvragen
            sb.AppendLine($"{tabspace}/* Detectie aanvragen */");
            sb.AppendLine($"{tabspace}/* ------------------ */");
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                bool HasA = false;
                if (fcm.Detectoren?.Count > 0)
                {
                    foreach (DetectorModel dm in fcm.Detectoren)
                    {
                        if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen)
                        {
                            HasA = true;
                            break;
                        }
                    }
                }
                if (HasA)
                {
                    sb.AppendLine($"{tabspace}aanvraag_detectie_prm_va_arg((count) {fcm.GetDefine()}, ");
                    foreach (DetectorModel dm in fcm.Detectoren)
                    {
                        if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen)
                            sb.AppendLine($"{tabspace}{tabspace}(va_count) {dm.GetDefine()}, (va_mulv) PRM[prm{dm.GetDefine()}], ");
                    }
                    sb.AppendLine($"{tabspace}{tabspace}(va_count) END);");
                }
            }
            sb.AppendLine("");

            // Vaste aanvragen
            sb.AppendLine($"{tabspace}/* Vaste aanvragen */");
            sb.AppendLine($"{tabspace}/* --------------- */");
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.VasteAanvraag == NooitAltijdAanUitEnum.SchAan ||
                    fcm.VasteAanvraag == NooitAltijdAanUitEnum.SchUit)
                    sb.AppendLine($"{tabspace}if (SCH[schca{fcm.Naam}]) vaste_aanvraag({fcm.GetDefine()});");
                else if (fcm.VasteAanvraag == NooitAltijdAanUitEnum.Altijd)
                    sb.AppendLine($"{tabspace}vaste_aanvraag({fcm.GetDefine()});");
            }
            sb.AppendLine("");

            // Wachtstand groen aanvragen
            sb.AppendLine($"{tabspace}/* Wachtstand groen aanvragen */");
            sb.AppendLine($"{tabspace}/* -------------------------- */");
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                    fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                    sb.AppendLine($"{tabspace}aanvraag_wachtstand_exp({fcm.GetDefine()}, (bool) (SCH[schwg{fcm.Naam}]));");
                else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                    sb.AppendLine($"{tabspace}aanvraag_wachtstand_exp({fcm.GetDefine()}, TRUE);");
            }
            sb.AppendLine("");

            // Add file
            sb.AppendLine($"{tabspace}Aanvragen_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCMaxgroen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void Maxgroen(void)");
            sb.AppendLine("{");
            sb.AppendLine();

            // Maxgroen obv periode
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                // Check if the FaseCyclus has any maxgreen times set
                bool HasMG = false;
                foreach(GroentijdenSetModel mgsm in controller.GroentijdenSets)
                {
                    foreach(GroentijdModel mgm in mgsm.Groentijden)
                    {
                        if(mgm.FaseCyclus == fcm.GetDefine() && mgm.Waarde != null)
                        {
                            HasMG = true;
                        }
                    }
                }

                if(HasMG)
                {
                    sb.AppendLine($"{tabspace}max_star_groentijden_va_arg((count) {fcm.GetDefine()}, (mulv) FALSE, (mulv) FALSE,");
                    //int per = 2, mper = 1;
                    //foreach (KlokPeriodeModel kpm in controller.KlokPeriodes)
                    //{
                    //    if(controller.DefaultKlokPeriode.Naam != kpm.Naam)
                    //    {
                    //        sb.AppendLine($"        (va_mulv) PRM[prmmg{per}{fcm.Naam}], (va_mulv) NG, (va_mulv) (MM[mperiod] == {mper}),");
                    //    }
                    //    ++per;
                    //    ++mper;
                    //}
                    sb.Append("".PadLeft(($"{tabspace}max_star_groentijden_va_arg(").Length));
                    sb.AppendLine($"(va_mulv) PRM[prmmg1{fcm.Naam}], (va_mulv) NG, (va_count) END);");
                }
                else
                {
                    sb.AppendLine($"{tabspace}TVG_max[{fcm.GetDefine()}] = 0;");
                }

            }

            // Add file
            sb.AppendLine($"{tabspace}Maxgroen_Add();");
            sb.AppendLine("}");

            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCWachtgroen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void Wachtgroen(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{tabspace}register count fc;");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}for (fc = 0; fc < FCMAX; ++fc)");
            sb.AppendLine($"{tabspace}{tabspace}RW[fc] &= ~BIT4;  /* reset BIT-sturing */");
            sb.AppendLine();
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                    fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                {
                    sb.AppendLine($"{tabspace}RW[{fcm.GetDefine()}] |= (SCH[schwg{fcm.Naam}] && yws_groen({fcm.GetDefine()})) && !fka({fcm.GetDefine()}) ? BIT4 : 0;");
                }
                else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                {
                    sb.AppendLine($"{tabspace}RW[{fcm.GetDefine()}] |= (yws_groen({fcm.GetDefine()})) && !fka({fcm.GetDefine()}) ? BIT4 : 0;");
                }
            }
            sb.AppendLine();
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                    fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                {
                    sb.AppendLine($"{tabspace}WS[{fcm.GetDefine()}] = WG[{fcm.GetDefine()}] && SCH[schwg{fcm.Naam}] && yws_groen({fcm.GetDefine()});");
                }
                else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                {
                    sb.AppendLine($"{tabspace}WS[{fcm.GetDefine()}] = WG[{fcm.GetDefine()}] && yws_groen({fcm.GetDefine()});");
                }
            }
            sb.AppendLine();
            sb.AppendLine($"{tabspace}Wachtgroen_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCMeetkriterium(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void Meetkriterium(void)");
            sb.AppendLine("{");
            sb.AppendLine();

            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                bool HasKopmax = false;
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    if (dm.Verlengen == DetectorVerlengenTypeEnum.Kopmax)
                    {
                        HasKopmax = true;
                        break;
                    }
                }
                if (HasKopmax)
                    sb.AppendLine($"{tabspace}meetkriterium_prm_va_arg((count){fcm.GetDefine()}, (count)tkm{fcm.Naam}, ");
                else
                    sb.AppendLine($"{tabspace}meetkriterium_prm_va_arg((count){fcm.GetDefine()}, NG, ");
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    if (dm.Verlengen != DetectorVerlengenTypeEnum.Geen)
                    {
                        sb.Append("".PadLeft($"{tabspace}meetkriterium_prm_va_arg(".Length));
                        sb.AppendLine($"(va_count){dm.GetDefine()}, (va_mulv)PRM[prmmk{dm.GetDefine()}],");
                    }
                }
                sb.Append("".PadLeft($"{tabspace}meetkriterium_prm_va_arg(".Length));
                sb.AppendLine($"(va_count)END);");
            }
            sb.AppendLine("");
            sb.AppendLine($"{tabspace}Meetkriterium_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCMeeverlengen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void Meeverlengen(void)");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}register count fc;");
            sb.AppendLine($"{tabspace}for (fc = 0; fc < FCMAX; ++fc)");
            sb.AppendLine($"{tabspace}" + "{");
            sb.AppendLine($"{tabspace}    YM[fc] &= ~BIT4;  /* reset BIT-sturing */");
            sb.AppendLine($"{tabspace}" + "}");
            sb.AppendLine();
            foreach (FaseCyclusModel fcm in controller.Fasen)
                sb.AppendLine($"{tabspace}YM[{fcm.GetDefine()}] |= SCH[schmv{fcm.Naam}] && ym_max({fcm.GetDefine()}, NG) && hf_wsg() ? BIT4 : 0;");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}Meeverlengen_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCRealisatieAfhandeling(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void RealisatieAfhandeling(void)");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}register count fc;");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}/* versnelde primaire realisaties */");
            sb.AppendLine($"{tabspace}/* ------------------------------ */");
            sb.AppendLine($"{tabspace}/* periode versnelde primaire realisatie - aantal modulen vooruit */");
            sb.AppendLine($"{tabspace}/* -------------------------------------------------------------- */");
            foreach (FaseCyclusModel fcm in controller.Fasen)
                sb.AppendLine($"    PFPR[{fcm.GetDefine()}] = ml_fpr({fcm.GetDefine()}, PRM[prmmlfpr{fcm.Naam}], PRML, ML, MLMAX);");
            sb.AppendLine("");
            sb.AppendLine($"{tabspace}VersneldPrimair_Add();");
            sb.AppendLine("");
            sb.AppendLine($"{tabspace}for (fc = 0; fc < FCMAX; ++fc)");
            sb.AppendLine($"{tabspace}{tabspace}set_FPRML(fc, PRML, ML, MLMAX, (bool)PFPR[fc]);");
            sb.AppendLine();
            if (controller.ModuleMolen.LangstWachtendeAlternatief)
            {
                sb.AppendLine($"{tabspace}/* langstwachtende alternatieve realisatie */");
                sb.AppendLine($"{tabspace}/* --------------------------------------- */");
                sb.AppendLine("");
                sb.AppendLine($"{tabspace}afsluiten_aanvraaggebied_pr(PRML, ML);");
                sb.AppendLine("");
                sb.AppendLine($"{tabspace}for (fc=0; fc<FCMAX; fc++)");
                sb.AppendLine($"{tabspace}" + "{");
                sb.AppendLine($"{tabspace}{tabspace}RR[fc] &= ~BIT5;");
                sb.AppendLine($"{tabspace}{tabspace}FM[fc] &= ~BIT5;");
                sb.AppendLine($"{tabspace}" + "}");
                sb.AppendLine();
                sb.AppendLine($"{tabspace}/* zet richtingen die alternatief gaan realiseren         */");
                sb.AppendLine($"{tabspace}/* terug naar RV als er geen alternatieve ruimte meer is. */");
                foreach (FaseCyclusModel fcm in controller.Fasen)
                    sb.AppendLine($"{tabspace}RR[{fcm.GetDefine()}] |= R[{fcm.GetDefine()}] && AR[{fcm.GetDefine()}] && (!PAR[{fcm.GetDefine()}] || ERA[{fcm.GetDefine()}]) ? BIT5 : 0;");
                sb.AppendLine();
                foreach (FaseCyclusModel fcm in controller.Fasen)
                    sb.AppendLine($"{tabspace}FM[{fcm.GetDefine()}] |= (fm_ar_kpr({fcm.GetDefine()}, PRM[prmaltg{fcm.Naam}])) ? BIT5 : 0;");
                sb.AppendLine();
                foreach (FaseCyclusModel fcm in controller.Fasen)
                    sb.AppendLine($"{tabspace}PAR[{fcm.GetDefine()}] = (max_tar_to({fcm.GetDefine()}) >= PRM[prmaltp{fcm.Naam}]) && SCH[schaltg{fcm.Naam}];");
                sb.AppendLine();
                sb.AppendLine($"{tabspace}Alternatief_Add();");
                sb.AppendLine();
                sb.AppendLine($"{tabspace}langstwachtende_alternatief_modulen(PRML, ML, ML_MAX);");
            }
            sb.AppendLine();
            sb.AppendLine($"{tabspace}YML[ML] = yml_cv_pr(PRML, ML, ML_MAX);");
            sb.AppendLine();
            foreach(ModuleModel mm in controller.ModuleMolen.Modules)
            {
                if(mm.Naam == controller.ModuleMolen.WachtModule)
                    sb.AppendLine($"{tabspace}YML[{mm.Naam}] |= yml_wml(PRML, ML_MAX);");
                else
                    sb.AppendLine($"{tabspace}YML[{mm.Naam}] |= FALSE;");
            }
            sb.AppendLine();
            sb.AppendLine($"{tabspace}Modules_Add();");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}SML = modules(ML_MAX, PRML, YML, &ML);");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}for (fc = 0; fc < FCMAX; ++fc)");
            sb.AppendLine($"{tabspace}" + "{");
            sb.AppendLine($"{tabspace}{tabspace}YM[fc] &= ~BIT5;");
            sb.AppendLine($"{tabspace}{tabspace}YM[fc] |= SML && PG[fc] ? BIT5 : FALSE;");
            sb.AppendLine($"{tabspace}" + "}");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}RealisatieAfhandeling_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCInitApplication(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void init_application(void)");
            sb.AppendLine("{");
            sb.AppendLine("#ifndef AUTOMAAT");
            sb.AppendLine($"{tabspace}if (!SAPPLPROG)");
            sb.AppendLine($"{tabspace}{tabspace}stuffkey(CTRLF4KEY);");
            sb.AppendLine("#endif");
            sb.AppendLine("");
            sb.AppendLine($"{tabspace}post_init_application();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCApplication(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void application(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{tabspace}pre_application();");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}TFB_max = PRM[prmfb];");
            sb.AppendLine($"{tabspace}KlokPerioden();");
            sb.AppendLine($"{tabspace}Aanvragen();");
            sb.AppendLine($"{tabspace}Maxgroen();");
            sb.AppendLine($"{tabspace}Wachtgroen();");
            sb.AppendLine($"{tabspace}Meetkriterium();");
            sb.AppendLine($"{tabspace}Meeverlengen();");
            sb.AppendLine($"{tabspace}RealisatieAfhandeling();");
            sb.AppendLine($"{tabspace}Fixatie(isfix, 0, FCMAX-1, SCH[schbmfix], PRML, ML);");
            sb.AppendLine("");
            sb.AppendLine($"{tabspace}post_application();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCSystemApplication(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void system_application(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{tabspace}pre_system_application();");
            sb.AppendLine();
            sb.Append(_PeriodenCodeGenerator.GetCode(controller, CCOLCodeType.SystemApplication, tabspace));
            sb.AppendLine();
            sb.Append(_WaitsignalenCodeGenerator.GetCode(controller, CCOLCodeType.SystemApplication, tabspace));
            sb.AppendLine();
            sb.AppendLine($"{tabspace}SegmentSturing(ML+1, ussegm1, ussegm2, ussegm3, ussegm4, ussegm5, ussegm6, ussegm7);");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}post_system_application();");
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
            sb.AppendLine($"{tabspace}post_dump_application();");
            sb.AppendLine("}");


            return sb.ToString();
        }
    }
}
