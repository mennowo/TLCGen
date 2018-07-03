using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GeneratePtpC(ControllerModel controller)
        {
            string _hptp = CCOLGeneratorSettingsProvider.Default.GetElementName("hptp");
            string _prmptp = CCOLGeneratorSettingsProvider.Default.GetElementName("prmptp");
            string _usptp = CCOLGeneratorSettingsProvider.Default.GetElementName("usptp");
            string _usptperr = CCOLGeneratorSettingsProvider.Default.GetElementName("userr");
            string _usptpoke = CCOLGeneratorSettingsProvider.Default.GetElementName("usoke");
            string _hptpiks = CCOLGeneratorSettingsProvider.Default.GetElementName("hiks");
            string _hptpuks = CCOLGeneratorSettingsProvider.Default.GetElementName("huks");
            string _hptpoke = CCOLGeneratorSettingsProvider.Default.GetElementName("hoke");
            string _hptperr = CCOLGeneratorSettingsProvider.Default.GetElementName("herr");
            string _hptperr0 = CCOLGeneratorSettingsProvider.Default.GetElementName("herr0");
            string _hptperr1 = CCOLGeneratorSettingsProvider.Default.GetElementName("herr1");
            string _hptperr2 = CCOLGeneratorSettingsProvider.Default.GetElementName("herr2");
            string _prmptpiks = CCOLGeneratorSettingsProvider.Default.GetElementName("prmiks");
            string _prmptpuks = CCOLGeneratorSettingsProvider.Default.GetElementName("prmuks");
            string _prmptpoke = CCOLGeneratorSettingsProvider.Default.GetElementName("prmoke");
            string _prmptperr = CCOLGeneratorSettingsProvider.Default.GetElementName("prmerr");
            string _prmptperr0 = CCOLGeneratorSettingsProvider.Default.GetElementName("prmerr0");
            string _prmptperr1 = CCOLGeneratorSettingsProvider.Default.GetElementName("prmerr1");
            string _prmptperr2 = CCOLGeneratorSettingsProvider.Default.GetElementName("prmerr2");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* APPLICATIE PTP-KOPPELINGEN */");
            sb.AppendLine("/* ========================== */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "ptp.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.AppendLine("/* koppelingen via PTP */");
            sb.AppendLine("/* ------------------- */");
            sb.AppendLine("/*");
            sb.AppendLine($"{ts}--------------------------------------------------");
            int i = 0;
            foreach (var k in controller.PTPData.PTPKoppelingen)
            {
                i = k.TeKoppelenKruispunt.Length;
            }
            sb.Append($"{ts}VRI".PadRight(i + ts.Length + 3));
            sb.AppendLine("PTP");
            sb.AppendLine($"{ts}--------------------------------------------------");
            foreach(var k in controller.PTPData.PTPKoppelingen)
            {
                sb.Append($"{ts}{k.TeKoppelenKruispunt}".PadRight(i + ts.Length + 3) + $"({k.NummerDestination})");
            }
            sb.AppendLine("*/");
            sb.AppendLine("");
            sb.AppendLine("/* macrodefinitie voor gebruik PTP-poorten */");
            sb.AppendLine("/* ======================================= */");
            foreach (var k in controller.PTPData.PTPKoppelingen)
            {
                sb.AppendLine($"#define PTP_{k.TeKoppelenKruispunt}PORT /* @ define van de te koppelen VRI */");
            }
            sb.AppendLine("");
            sb.AppendLine("/* include files */");
            sb.AppendLine("/* ============= */");
            sb.AppendLine("#include \"ptpvar.c\"     /* definitie structuur ptp-berichten    */");
            sb.AppendLine("#include \"ptpksvar.c\"   /* definitie structuur koppelsignalen   */");
            sb.AppendLine("");
            sb.AppendLine("#ifdef FABRIKANT_DRIVER");
            sb.AppendLine("/* declaraties t.b.v. aanroep fabrikant-driver */");
            sb.AppendLine("/* ------------------------------------------- */");
            sb.AppendLine($"{ts}#define INIT       1");
            sb.AppendLine($"{ts}#define GEEN_INIT  0");
            sb.AppendLine($"{ts}#define EINDE     -1");
            sb.AppendLine($"{ts}extern int communicatieprogramma(int x);");
            sb.AppendLine("#endif");
            sb.AppendLine("");
            sb.AppendLine("/* definitie structuren t.b.v. PTP-koppeling(en) */");
            sb.AppendLine("/* --------------------------------------------- */");
            foreach (var k in controller.PTPData.PTPKoppelingen)
            {
                sb.AppendLine($"#ifdef PTP_{k.TeKoppelenKruispunt}PORT");
                sb.AppendLine($"{ts}struct ptpstruct   PTP_{k.TeKoppelenKruispunt};");
                sb.AppendLine($"{ts}struct ptpksstruct PTP_{k.TeKoppelenKruispunt}KS;");
                sb.AppendLine("#endif");
            }
            sb.AppendLine("");
            sb.AppendLine("/* PTP CONTROL PARAMETERS */");
            sb.AppendLine("/* ====================== */");
            sb.AppendLine("");
            sb.AppendLine("/* ptp_control_parameters() wordt gebruikt voor de specificatie van de parameters van");
            sb.AppendLine(" * de PTP-verbinding.");
            sb.AppendLine(" * ptp_control_parameters() wordt aangeroepen door de functie ptp_system_application().");
            sb.AppendLine(" */");
            sb.AppendLine("");
            sb.AppendLine("void ptp_control_parameters (void)");
            sb.AppendLine("{");
            sb.AppendLine();
            foreach (var k in controller.PTPData.PTPKoppelingen)
            {
                sb.AppendLine($"{ts}#ifdef PTP_{k.TeKoppelenKruispunt}PORT");
                sb.AppendLine($"{ts}{ts}/* ptp-parameters t.b.v. koppeling met PTP_{k.TeKoppelenKruispunt} */");
                sb.AppendLine($"{ts}{ts}/* ----------------------------------------- */");
                sb.AppendLine($"{ts}#ifdef AUTOMAAT");
                sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.PORTNR = {k.PortnummerAutomaatOmgeving};        /* poortnummer in het regeltoestel     */  /* @ door fabrikant aanpassen */");
                sb.AppendLine($"{ts}#else");
                sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.PORTNR = {k.PortnummerSimuatieOmgeving};        /* poortnr. testomgeving (schrijvend) */ /* @ nummer van KS-buffer */");
                sb.AppendLine($"{ts}#endif");
                sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.SRC  = {k.NummerSource};       /* nummer van source                   */ /* @ maximaal laatste twee cijfers krpnr */");
                sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.DEST = {k.NummerDestination};       /* nummer van destination              */ /* @ maximaal laatste twee cijfers krpnr */");
                sb.AppendLine();
                sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.TMSGW_max= 200;   /* wait  time-out             */");
                sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.TMSGS_max=  10;   /* send  time-out             */");
                sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.TMSGA_max=  10;   /* alive time-out             */");
                sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.CMSG_max=    3;   /* max. berichtenteller tbv. herhaling */");
                sb.AppendLine();
                sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}KS.IKS_MAX=  {k.AantalsignalenIn};   /* aantal inkomende koppelsignalen    */ /* @ verhogen in stappen van 8 */");
                sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}KS.UKS_MAX=  {k.AantalsignalenUit};   /* aantal uitgaande koppelsignalen    */ /* @ verhogen in stappen van 8 */");
                sb.AppendLine($"{ts}#endif");
                sb.AppendLine();
            }
            sb.AppendLine("}");

            sb.AppendLine("/* PTP PRE SYSTEM APPLICATION */");
            sb.AppendLine("/* -------------------------- */");
            sb.AppendLine("/* ptp_pre_system_app() wordt gebruikt voor de specificatie van de afwikkeling van");
            sb.AppendLine(" * de PTP-verbindingen.");
            sb.AppendLine(" *");
            sb.AppendLine(" * ptp_pre_system_app() moet worden aangeroepen vanuit de functie pre_system_application().");
            sb.AppendLine(" */");
            sb.AppendLine();
            sb.AppendLine("void ptp_pre_system_app(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}register int i;");
            sb.AppendLine();
            sb.AppendLine($"{ts}if (SAPPLPROG) /* initialisatie bij start regelprogramma */");
            sb.AppendLine($"{ts}{{");
            sb.AppendLine($"{ts}{ts}#ifdef FABRIKANT_DRIVER");
            sb.AppendLine($"{ts}{ts}{ts}communicatieprogramma(INIT);");
            sb.AppendLine($"{ts}{ts}#endif");
            sb.AppendLine();
            sb.AppendLine($"{ts}{ts}/* opzetten PTP control parameters */");
            sb.AppendLine($"{ts}{ts}/* ------------------------------- */");
            sb.AppendLine($"{ts}{ts}ptp_control_parameters();");
            sb.AppendLine($"{ts}{ts}");
            sb.AppendLine($"{ts}{ts}/* initialisatie van PTP-koppelingen */");
            sb.AppendLine($"{ts}{ts}/* --------------------------------- */");
            foreach (var k in controller.PTPData.PTPKoppelingen)
            {
                sb.AppendLine($"{ts}{ts}#ifdef PTP_{k.TeKoppelenKruispunt}PORT");
                sb.AppendLine($"{ts}{ts}    ptp_init(&PTP_{k.TeKoppelenKruispunt});");
                sb.AppendLine($"{ts}{ts}#endif");
                sb.AppendLine();
            }
            sb.AppendLine($"{ts}}}");
            sb.AppendLine($"{ts}else");
            sb.AppendLine($"{ts}{{");
            sb.AppendLine($"{ts}{ts}#ifdef FABRIKANT_DRIVER");
            sb.AppendLine($"{ts}{ts}{ts}communicatieprogramma(GEEN_INIT);");
            sb.AppendLine($"{ts}{ts}#endif");
            sb.AppendLine();
            foreach (var k in controller.PTPData.PTPKoppelingen)
            {
                sb.AppendLine($"{ts}{ts}/* opzetten signalen van en naar {k.TeKoppelenKruispunt} */");
                sb.AppendLine($"#ifdef PTP_{k.TeKoppelenKruispunt}PORT");
                sb.AppendLine("");
                sb.AppendLine($"{ts}{ts}/* nalopen in en uitgangssignalen */");
                sb.AppendLine($"{ts}{ts}");
                sb.AppendLine($"{ts}{ts}/* opzetten van uitgaande koppelsignalen PTP_{k.TeKoppelenKruispunt} */");
                sb.AppendLine($"{ts}{ts}/* ------------------------------------------------- */      ");
                sb.AppendLine($"{ts}{ts}if (CIF_WPS[CIF_PROG_STATUS] == CIF_STAT_REG) /* status regelen - set uitgaande koppelsignalen  */");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}for(i = 0; i < PTP_{k.TeKoppelenKruispunt}KS.UKS_MAX; ++i) PTP_{k.TeKoppelenKruispunt}KS.UKS[i] = IH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptpuks}01 + i];");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}else /* niet regelen - reset uitgaande koppelsignalen */");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}for(i = 0; i < PTP_{k.TeKoppelenKruispunt}KS.UKS_MAX; ++i) PTP_{k.TeKoppelenKruispunt}KS.UKS[i] = FALSE;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}");
                sb.AppendLine($"{ts}{ts}/* opzetten van ingaande koppelsignalen PTP_{k.TeKoppelenKruispunt} */");
                sb.AppendLine($"{ts}{ts}/* ---------------------------------------------- */");
                sb.AppendLine($"{ts}{ts}if (PTP_{k.TeKoppelenKruispunt}KS.OKE) /* goede verbinding - set ingaande koppelsignalen */");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}for(i = 0; i < PTP_{k.TeKoppelenKruispunt}KS.IKS_MAX; ++i) IH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptpiks}01 + i] = PTP_{k.TeKoppelenKruispunt}KS.IKS[i];");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}else /* geen goede verbinding - reset ingaande koppelsignalen */");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}    for(i = 0; i < PTP_{k.TeKoppelenKruispunt}KS.IKS_MAX; ++i) IH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptpiks}01 + i] = FALSE;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine();
                sb.AppendLine($"{ts}{ts}/* aanroep ptp-functies */");
                sb.AppendLine($"{ts}{ts}/* -------------------- */");
                sb.AppendLine($"{ts}{ts}ptp_application_ks(&PTP_{k.TeKoppelenKruispunt}, &PTP_{k.TeKoppelenKruispunt}KS);");
                sb.AppendLine($"{ts}{ts}ptp_control(&PTP_{k.TeKoppelenKruispunt});");
                sb.AppendLine();
                sb.AppendLine($"{ts}{ts}/* opzetten hulpelementen */");
                sb.AppendLine($"{ts}{ts}/* ---------------------- */");
                sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptpoke}]  = PTP_{k.TeKoppelenKruispunt}KS.OKE;");
                sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptperr}]  = PTP_{k.TeKoppelenKruispunt}.COMERROR;");
                sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptperr0}] = PTP_{k.TeKoppelenKruispunt}.COMERROR & BIT0;");
                sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptperr1}] = PTP_{k.TeKoppelenKruispunt}.COMERROR & BIT1;");
                sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptperr2}] = PTP_{k.TeKoppelenKruispunt}.COMERROR & BIT2;");
                sb.AppendLine("");
                sb.AppendLine($"{ts}{ts}/* Bijhouden ptp errors   */");
                sb.AppendLine($"{ts}{ts}/* ---------------------- */");
                sb.AppendLine($"{ts}{ts}if (SH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptpoke}])");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}if((PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptpoke}] + 1) >= 32767)");
                sb.AppendLine($"{ts}{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}{ts}PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptpoke}] = 0;");
                sb.AppendLine($"{ts}{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptpoke}]++;");
                sb.AppendLine($"{ts}{ts}{ts}CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}if (SH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptperr}])");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}if((PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr}] + 1) >= 32767)");
                sb.AppendLine($"{ts}{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}{ts}PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr}] = 0;");
                sb.AppendLine($"{ts}{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr}]++;");
                sb.AppendLine($"{ts}{ts}{ts}CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}if (SH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptperr0}])");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}if((PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr0}] + 1) >= 32767)");
                sb.AppendLine($"{ts}{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}{ts}PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr0}] = 0;");
                sb.AppendLine($"{ts}{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr0}]++;");
                sb.AppendLine($"{ts}{ts}{ts}CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}if (SH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptperr1}])");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}if((PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr1}] + 1) >= 32767)");
                sb.AppendLine($"{ts}{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}{ts}PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr1}] = 0;");
                sb.AppendLine($"{ts}{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr1}]++;");
                sb.AppendLine($"{ts}{ts}{ts}CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}if (SH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptperr2}])");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}if((PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr2}] + 1) >= 32767)");
                sb.AppendLine($"{ts}{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}{ts}PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr2}] = 0;");
                sb.AppendLine($"{ts}{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr2}]++; ");
                sb.AppendLine($"{ts}{ts}{ts}CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine();
                sb.AppendLine($"{ts}{ts}/* aansturing led op handbedieningspaneel en via IVERA op krspnt plaatje */");
                sb.AppendLine($"{ts}{ts}/* --------------------------------------------------------------------- */");
                sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_usptp}_{k.TeKoppelenKruispunt}{_usptpoke}] = PTP_{k.TeKoppelenKruispunt}KS.OKE;  ");
                sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_usptp}_{k.TeKoppelenKruispunt}{_usptperr}] = PTP_{k.TeKoppelenKruispunt}.COMERROR;");
                sb.AppendLine("");
                sb.AppendLine("#endif");
                sb.AppendLine("");
            }
            sb.AppendLine($"{ts}}}");
            sb.AppendLine("}");

            sb.AppendLine("/* PTP POST SYSTEM APPLICATION */");
            sb.AppendLine("/* --------------------------- */");
            sb.AppendLine("/* ptp_post_system_app() wordt gebruikt voor op en afzetten van hulpelementen voor de PTP koppeling");
            sb.AppendLine(" *");
            sb.AppendLine(" * ptp_post_system_app() moet worden aangeroepen vanuit de functie post_system_application().");
            sb.AppendLine(" */");
            sb.AppendLine();
            sb.AppendLine("void ptp_post_system_app(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}int i;");
            sb.AppendLine();
            foreach (var k in controller.PTPData.PTPKoppelingen)
            {
                sb.AppendLine($"{ts}/* Hulpelementen t.b.v. in- en uitgangen seriele koppeling van {k.TeKoppelenKruispunt} */");
                sb.AppendLine($"{ts}/* ------------------------------------------------------------------ */");
                sb.AppendLine($"{ts}for(i = 0; i < PTP_{k.TeKoppelenKruispunt}KS.IKS_MAX; ++i)");
                sb.AppendLine($"{ts}{{");
                sb.AppendLine($"{ts}{ts}IH[{_hpf}{k.TeKoppelenKruispunt}{_hptpiks}01 + i] = ((IH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptpiks}01 + i] && PRM[{_prmpf}{k.TeKoppelenKruispunt}{_prmptpiks}01 + i] >= 2) || PRM[{_prmpf}{k.TeKoppelenKruispunt}{_prmptpiks}01 + i] == 1);");
                sb.AppendLine($"{ts}}}");
                sb.AppendLine("");
                sb.AppendLine($"{ts}for(i = 0; i < PTP_{k.TeKoppelenKruispunt}KS.UKS_MAX; ++i)");
                sb.AppendLine($"{ts}{{");
                sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hptp}_{k.TeKoppelenKruispunt}{_hptpuks}01 + i] = ((IH[{_hpf}{k.TeKoppelenKruispunt}{_hptpuks}01 + i] && PRM[{_prmpf}{k.TeKoppelenKruispunt}{_prmptpuks}01 + i] >= 2) || PRM[{_prmpf}{k.TeKoppelenKruispunt}{_prmptpuks}01 + i] == 1);");
                sb.AppendLine($"{ts}}}");
                sb.AppendLine();
            }
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("#if !defined (AUTOMAAT) && defined (CCOL_EXIT)");
            sb.AppendLine();
            sb.AppendLine("/* resetten koppelsignalen in testomgeving op einde programma */");
            sb.AppendLine("/* ---------------------------------------------------------- */");
            sb.AppendLine();
            sb.AppendLine("/* geen VISSIM mee opgenomen, daar in VISSM alle regelingen tegelijk starten en eindingen.");
            sb.AppendLine("*/");
            sb.AppendLine();
            sb.AppendLine("void CCOL_exit(void)");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine($"{ts}void ptp_application_reset(struct ptpstruct *PTP, struct ptpksstruct *PTPKS); /* PTPWIN.C */");
            sb.AppendLine();
            foreach (var k in controller.PTPData.PTPKoppelingen)
            {
                sb.AppendLine($"{ts}#ifdef PTP_{k.TeKoppelenKruispunt}PORT");
                sb.AppendLine($"{ts}{ts}ptp_application_reset(&PTP_{k.TeKoppelenKruispunt}, &PTP_{k.TeKoppelenKruispunt}KS);");
                sb.AppendLine($"{ts}#endif");
                sb.AppendLine();
            }
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("#endif");

            return sb.ToString();
        }
    }
}
