using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL
{
    public partial class CCOLCodeGenerator
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
            //sb.AppendLine("    #include \"detectie.c\"");
            //sb.AppendLine("    #include \"ccolfunc.c\"");
            sb.AppendLine($"{tabspace}#include \"{controller.Data.Naam}reg.add\"");

            return sb.ToString();
        }

        private string GenerateRegCKlokPerioden(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void KlokPerioden(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{tabspace}/* default klokperiode voor max.groen */");
            sb.AppendLine($"{tabspace}/* ---------------------------------- */");
            sb.AppendLine($"{tabspace}MM[mperiod] = 0;");
            sb.AppendLine();
            //int i = 1;
            //foreach(KlokPeriodeModel kpm in controller.KlokPerioden)
            //{
            //    sb.AppendLine($"    /* klokperiode: {kpm.Omschrijving} */");
            //    sb.AppendLine("    /* ---------------- */");
            //    sb.AppendLine($"    if (klokperiode(PRM[prmstkp{i}], PRM[prmetkp{i}])");
            //    sb.AppendLine($"        && dagsoort(PRM[prmdckp1{i}]))");
            //    sb.AppendLine($"      MM[mperiod] = {i};");
            //    sb.AppendLine();
            //    ++i;
            //}
            sb.AppendLine($"{tabspace}KlokPerioden_Add();");
            sb.AppendLine("}");


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
                    sb.AppendLine($"{tabspace}aanvraag_detectie_prm_va_arg((count) {fcm.Define}, ");
                    foreach (DetectorModel dm in fcm.Detectoren)
                    {
                        sb.AppendLine($"{tabspace}{tabspace}(va_count) {dm.Define}, (va_mulv) PRM[prm{dm.Define}], ");
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
                if (fcm.VasteAanvraag != NooitAltijdAanUitEnum.Nooit)
                    sb.AppendLine($"{tabspace}if (SCH[schca{fcm.Naam}]) vaste_aanvraag({fcm.Define});");
            }
            sb.AppendLine("");

            // Wachtstand groen aanvragen
            sb.AppendLine($"{tabspace}/* Wachtstand groen aanvragen */");
            sb.AppendLine($"{tabspace}/* -------------------------- */");
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                    fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                    sb.AppendLine($"{tabspace}aanvraag_wachtstand_exp({fcm.Define}, (bool) (SCH[schwg{fcm.Naam}]));");
                else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                    sb.AppendLine($"{tabspace}aanvraag_wachtstand_exp({fcm.Define}, TRUE);");
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
                foreach(MaxGroentijdenSetModel mgsm in controller.MaxGroentijdenSets)
                {
                    foreach(MaxGroentijdModel mgm in mgsm.MaxGroentijden)
                    {
                        if(mgm.FaseCyclus == fcm.Define && mgm.Waarde != null)
                        {
                            HasMG = true;
                        }
                    }
                }

                if(HasMG)
                {
                    sb.AppendLine($"{tabspace}max_star_groentijden_va_arg((count) {fcm.Define}, (mulv) FALSE, (mulv) FALSE,");
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
                    sb.AppendLine("(va_mulv) PRM[prmmg1{fcm.Naam}], (va_mulv) NG, (va_count) END);");
                }
                else
                {
                    sb.AppendLine($"{tabspace}TVG_max[{fcm.Define}] = 0;");
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
                    sb.AppendLine($"{tabspace}RW[{fcm.Define}] |= (SCH[schwg{fcm.Naam}] && yws_groen({fcm.Define})) && !fka({fcm.Define}) ? BIT4 : 0;");
                }
                else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                {
                    sb.AppendLine($"{tabspace}RW[{fcm.Define}] |= (yws_groen({fcm.Define})) && !fka({fcm.Define}) ? BIT4 : 0;");
                }
            }
            sb.AppendLine();
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                    fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                {
                    sb.AppendLine($"{tabspace}WS[{fcm.Define}] = WG[{fcm.Define}] && SCH[schwg{fcm.Naam}] && yws_groen({fcm.Define});");
                }
                else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                {
                    sb.AppendLine($"{tabspace}WS[{fcm.Define}] = WG[{fcm.Define}] && yws_groen({fcm.Define});");
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
                    sb.AppendLine($"{tabspace}meetkriterium_prm_va_arg((count){fcm.Define}, (count)tkm{fcm.Naam}, ");
                else
                    sb.AppendLine($"{tabspace}meetkriterium_prm_va_arg((count){fcm.Define}, NG, ");
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    if (dm.Verlengen != DetectorVerlengenTypeEnum.Geen)
                    {
                        sb.Append("".PadLeft($"{tabspace}meetkriterium_prm_va_arg(".Length));
                        sb.AppendLine($"(va_count){dm.Define}_1, (va_mulv)PRM[prmmk{dm.Define}_1],");
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
                sb.AppendLine($"{tabspace}YM[{fcm.Define}] |= SCH[schmv{fcm.Naam}] && ym_max({fcm.Define}, NG) && hf_wsg() ? BIT4 : 0;");
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
                sb.AppendLine($"    PFPR[{fcm.Define}] = ml_fpr({fcm.Define}, PRM[prmmlfpr{fcm.Naam}], PRML, ML, MLMAX);");
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
                    sb.AppendLine($"{tabspace}RR[{fcm.Define}] |= R[{fcm.Define}] && AR[{fcm.Define}] && (!PAR[{fcm.Define}] || ERA[{fcm.Define}]) ? BIT5 : 0;");
                sb.AppendLine();
                foreach (FaseCyclusModel fcm in controller.Fasen)
                    sb.AppendLine($"{tabspace}FM[{fcm.Define}] |= (fm_ar_kpr({fcm.Define}, PRM[prmaltg{fcm.Naam}])) ? BIT5 : 0;");
                sb.AppendLine();
                foreach (FaseCyclusModel fcm in controller.Fasen)
                    sb.AppendLine($"{tabspace}PAR[{fcm.Define}] = (max_tar_to({fcm.Define}) >= PRM[prmaltp{fcm.Naam}]) && SCH[schaltg{fcm.Naam}];");
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
            sb.AppendLine($"{tabspace}SegmentSturing(ML+1, ussegm1, ussegm2, ussegm3, ussegm4, ussegm5, ussegm6, ussegm7);");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}post_system_application();");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
