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
            sb.AppendLine($"    #include \"{controller.Data.Naam}sys.h\"");
#warning TODO - make includes dependent on the kind of controller and its settings
            sb.AppendLine("    #include \"fcvar.c\"    /* fasecycli                         */");
            sb.AppendLine("    #include \"kfvar.c\"    /* conflicten                        */");
            sb.AppendLine("    #include \"usvar.c\"    /* uitgangs elementen                */");
            sb.AppendLine("    #include \"dpvar.c\"    /* detectie elementen                */");
            sb.AppendLine("    #include \"to_min.c\"   /* garantie-ontruimingstijden        */");
            sb.AppendLine("    #include \"trg_min.c\"  /* garantie-roodtijden               */");
            sb.AppendLine("    #include \"tgg_min.c\"  /* garantie-groentijden              */");
            sb.AppendLine("    #include \"tgl_min.c\"  /* garantie-geeltijden               */");
            sb.AppendLine("    #include \"isvar.c\"    /* ingangs elementen                 */");
            sb.AppendLine("    #include \"dsivar.c\"   /* selectieve detectie               */");
            sb.AppendLine("    #include \"hevar.c\"    /* hulp elementen                    */");
            sb.AppendLine("    #include \"mevar.c\"    /* geheugen elementen                */");
            sb.AppendLine("    #include \"tmvar.c\"    /* tijd elementen                    */");
            sb.AppendLine("    #include \"ctvar.c\"    /* teller elementen                  */");
            sb.AppendLine("    #include \"schvar.c\"   /* software schakelaars              */");
            sb.AppendLine("    #include \"prmvar.c\"   /* parameters                        */");
            sb.AppendLine("    #include \"lwmlvar.c\"  /* langstwachtende modulen structuur */");
            sb.AppendLine("    #ifndef NO_VLOG");
            sb.AppendLine("       #include \"vlogvar.c\"  /* variabelen t.b.v. vlogfuncties                */");
            sb.AppendLine("       #include \"logvar.c\"   /* variabelen t.b.v. logging                     */");
            sb.AppendLine("       #include \"monvar.c\"   /* variabelen t.b.v. realtime monitoring         */");
            sb.AppendLine("       #include \"fbericht.h\"");
            sb.AppendLine("    #endif");
            sb.AppendLine("    #include \"prsvar.c\"   /* parameters parser                 */");
            sb.AppendLine("    #include \"control.c\"  /* controller interface              */");
            sb.AppendLine("    #include \"rtappl.h\"   /* applicatie routines               */");
            sb.AppendLine("    #include \"stdfunc.h\"  /* standaard functies                */");
            sb.AppendLine();
            sb.AppendLine("#ifndef AUTOMAAT");
            sb.AppendLine("/*    #include \"ccdump.inc\" */");
            sb.AppendLine("    #include \"keysdef.c\"     /* Definitie toetsenbord t.b.v. stuffkey  */");
            sb.AppendLine("    #if !defined (_DEBUG)");
            sb.AppendLine("       #include \"xyprintf.h\" /* Printen debuginfo                      */");
            sb.AppendLine("    #endif");
            sb.AppendLine("#endif");
            sb.AppendLine();
            //sb.AppendLine("    #include \"detectie.c\"");
            //sb.AppendLine("    #include \"ccolfunc.c\"");
            sb.AppendLine($"    #include \"{controller.Data.Naam}reg.add\"");

            return sb.ToString();
        }

        private string GenerateRegCKlokPerioden(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void KlokPerioden(void)");
            sb.AppendLine("{");
            sb.AppendLine("    /* default klokperiode voor max.groen */");
            sb.AppendLine("    /* ---------------------------------- */");
            sb.AppendLine("    MM[mperiod] = 0;");
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
            sb.AppendLine("    KlokPerioden_Add();");
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
            sb.AppendLine("    /* Detectie aanvragen */");
            sb.AppendLine("    /* ------------------ */");
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
                    sb.AppendLine($"    aanvraag_detectie_prm_va_arg((count) {fcm.Define}, ");
                    foreach (DetectorModel dm in fcm.Detectoren)
                    {
                        sb.AppendLine($"        (va_count) {dm.Define}, (va_mulv) PRM[prm{dm.Define}], ");
                    }
                    sb.AppendLine("        (va_count) END);");
                }
            }
            sb.AppendLine("");

            // Vaste aanvragen
            sb.AppendLine("    /* Vaste aanvragen */");
            sb.AppendLine("    /* --------------- */");
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.VasteAanvraag != NooitAltijdAanUitEnum.Nooit)
                    sb.AppendLine($"    if (SCH[schca{fcm.Naam}]) vaste_aanvraag({fcm.Define});");
            }
            sb.AppendLine("");

            // Wachtstand groen aanvragen
            sb.AppendLine("    /* Wachtstand groen aanvragen */");
            sb.AppendLine("    /* -------------------------- */");
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                    fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                    sb.AppendLine($"    aanvraag_wachtstand_exp({fcm.Define}, (bool) (SCH[schwg{fcm.Naam}]));");
                else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                    sb.AppendLine($"    aanvraag_wachtstand_exp({fcm.Define}, TRUE);");
            }
            sb.AppendLine("");

            // Add file
            sb.AppendLine("    Aanvragen_Add();");
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
                    sb.AppendLine($"    max_star_groentijden_va_arg((count) {fcm.Define}, (mulv) FALSE, (mulv) FALSE,");
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
                    sb.AppendLine($"        (va_mulv) PRM[prmmg1{fcm.Naam}], (va_mulv) NG, (va_count) END);");
                }
                else
                {
                    sb.AppendLine($"    TVG_max[{fcm.Define}] = 0;");
                }

            }

            // Add file
            sb.AppendLine("    Maxgroen_Add();");
            sb.AppendLine("}");

            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateRegCWachtgroen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void Wachtgroen(void)");
            sb.AppendLine("{");
            sb.AppendLine("    register count fc;");
            sb.AppendLine();
            sb.AppendLine("    for (fc=0; fc<FCMAX; fc++)");
            sb.AppendLine("        RW[fc] &= ~BIT4;  /* reset BIT-sturing */");
            sb.AppendLine();
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                    fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                {
                    sb.AppendLine($"    RW[{fcm.Define}] |= (SCH[schwg{fcm.Naam}] && yws_groen({fcm.Define})) && !fka({fcm.Define}) ? BIT4 : 0;");
                }
                else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                {
                    sb.AppendLine($"    RW[{fcm.Define}] |= (yws_groen({fcm.Define})) && !fka({fcm.Define}) ? BIT4 : 0;");
                }
            }
            sb.AppendLine();
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                    fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                {
                    sb.AppendLine($"    WS[{fcm.Define}] = WG[{fcm.Define}] && SCH[schwg{fcm.Naam}] && yws_groen({fcm.Define});");
                }
                else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                {
                    sb.AppendLine($"    WS[{fcm.Define}] = WG[{fcm.Define}] && yws_groen({fcm.Define});");
                }
            }
            sb.AppendLine();
            sb.AppendLine("    Wachtgroen_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCMeetkriterium(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void Meetkriterium(void)");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine("    Meetkriterium_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCMeeverlengen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void Meeverlengen(void)");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine("    Meeverlengen_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCRealisatieAfhandeling(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void RealisatieAfhandeling(void)");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine("    Alternatief_Add();");
            sb.AppendLine();
            sb.AppendLine("    langstwachtende_alternatief_modulen(PRML, ML, ML_MAX);");
            sb.AppendLine();
            sb.AppendLine("    Modules_Add();");
            sb.AppendLine();
            sb.AppendLine("    RealisatieAfhandeling_Add();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCInitApplication(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void init_application(void)");
            sb.AppendLine("{");
            sb.AppendLine("#ifndef AUTOMAAT");
            sb.AppendLine("   if (!SAPPLPROG)");
            sb.AppendLine("      stuffkey(CTRLF4KEY);");
            sb.AppendLine("#endif");
            sb.AppendLine("    EXTRADUMP = 0;");
            sb.AppendLine("    init_realisation_timers();");
            sb.AppendLine("");
            sb.AppendLine("    post_init_application();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCApplication(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void application(void)");
            sb.AppendLine("{");
            sb.AppendLine("    pre_application();");
            sb.AppendLine("");
            sb.AppendLine("#ifndef AUTOMAAT");
            sb.AppendLine("/*    WriteDump(0); */");
            sb.AppendLine("#endif");
            sb.AppendLine("");
            sb.AppendLine("    TFB_max = PRM[prmfb];");
            sb.AppendLine("    KlokPerioden();");
            sb.AppendLine("    Aanvragen();");
            sb.AppendLine("    Maxgroen();");
            sb.AppendLine("    Wachtgroen();");
            sb.AppendLine("    Meetkriterium();");
            sb.AppendLine("    FileVerwerking();");
            sb.AppendLine("    DetectieStoring();");
            sb.AppendLine("    Meeverlengen();");
            sb.AppendLine("    Synchronisaties();");
            sb.AppendLine("    RealisatieAfhandeling();");
            sb.AppendLine("    AfhandelingOV();");
            sb.AppendLine("    Fixatie(isfix, 0, FCMAX-1, SCH[schbmfix], PRML, ML);");
            sb.AppendLine("");
            sb.AppendLine("    post_application();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRegCSystemApplication(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void system_application(void)");
            sb.AppendLine("{");
            sb.AppendLine("    pre_system_application();");
            sb.AppendLine("");
            sb.AppendLine("    post_system_application();");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
