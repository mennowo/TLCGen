using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GeneratePtpC(ControllerModel c)
        {
            var hptp = CCOLGeneratorSettingsProvider.Default.GetElementName("hptp");
            var prmptp = CCOLGeneratorSettingsProvider.Default.GetElementName("prmptp");
            var usptp = CCOLGeneratorSettingsProvider.Default.GetElementName("usptp");
            var usptperr = CCOLGeneratorSettingsProvider.Default.GetElementName("userr");
            var usptpoke = CCOLGeneratorSettingsProvider.Default.GetElementName("usoke");
            var hptpiks = CCOLGeneratorSettingsProvider.Default.GetElementName("hiks");
            var hptpuks = CCOLGeneratorSettingsProvider.Default.GetElementName("huks");
            var hptpoke = CCOLGeneratorSettingsProvider.Default.GetElementName("hoke");
            var hptperr = CCOLGeneratorSettingsProvider.Default.GetElementName("herr");
            var hptperr0 = CCOLGeneratorSettingsProvider.Default.GetElementName("herr0");
            var hptperr1 = CCOLGeneratorSettingsProvider.Default.GetElementName("herr1");
            var hptperr2 = CCOLGeneratorSettingsProvider.Default.GetElementName("herr2");
            var prmptpiks = CCOLGeneratorSettingsProvider.Default.GetElementName("prmiks");
            var prmptpuks = CCOLGeneratorSettingsProvider.Default.GetElementName("prmuks");
            var prmptpoke = CCOLGeneratorSettingsProvider.Default.GetElementName("prmoke");
            var prmptperr = CCOLGeneratorSettingsProvider.Default.GetElementName("prmerr");
            var prmptperr0 = CCOLGeneratorSettingsProvider.Default.GetElementName("prmerr0");
            var prmptperr1 = CCOLGeneratorSettingsProvider.Default.GetElementName("prmerr1");
            var prmptperr2 = CCOLGeneratorSettingsProvider.Default.GetElementName("prmerr2");
            var prmportnr = CCOLGeneratorSettingsProvider.Default.GetElementName("prmportnr");
            var prmsrc = CCOLGeneratorSettingsProvider.Default.GetElementName("prmsrc");
            var prmdest = CCOLGeneratorSettingsProvider.Default.GetElementName("prmdest");
            var prmtmsgw = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtmsgw");
            var prmtmsgs = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtmsgs");
            var prmtmsga = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtmsga");
            var prmcmsg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmcmsg");

            var sb = new StringBuilder();
            sb.AppendLine("/* APPLICATIE PTP-KOPPELINGEN */");
            sb.AppendLine("/* ========================== */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(c.Data, "ptp.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(c.Data));
            sb.AppendLine();
            if (c.Data.CCOLVersie >= Models.Enumerations.CCOLVersieEnum.CCOL110)
            {
                sb.AppendLine("#define AUTSTATUS    5 /* status automaat */");
                sb.AppendLine("#define CIFA_COMF 2048 /* communicatiefout PTP-protocol */");
                sb.AppendLine();
            }
            sb.AppendLine("/* koppelingen via PTP */");
            sb.AppendLine("/* ------------------- */");
            sb.AppendLine("/*");
            sb.AppendLine($"{ts}--------------------------------------------------");
            var i = 0;
            foreach (var k in c.PTPData.PTPKoppelingen)
            {
                i = k.TeKoppelenKruispunt.Length;
            }
            sb.Append($"{ts}VRI".PadRight(i + ts.Length + 3));
            sb.AppendLine("PTP");
            sb.AppendLine($"{ts}--------------------------------------------------");
            foreach(var k in c.PTPData.PTPKoppelingen)
            {
                sb.Append($"{ts}{k.TeKoppelenKruispunt}".PadRight(i + ts.Length + 3) + $"({k.NummerDestination})");
            }
            sb.AppendLine("*/");
            sb.AppendLine("");
            sb.AppendLine("/* macrodefinitie voor gebruik PTP-poorten */");
            sb.AppendLine("/* ======================================= */");
            foreach (var k in c.PTPData.PTPKoppelingen)
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
            foreach (var k in c.PTPData.PTPKoppelingen)
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
            foreach (var k in c.PTPData.PTPKoppelingen)
            {
                sb.AppendLine($"{ts}#ifdef PTP_{k.TeKoppelenKruispunt}PORT");
                sb.AppendLine($"{ts}{ts}/* ptp-parameters t.b.v. koppeling met PTP_{k.TeKoppelenKruispunt} */");
                sb.AppendLine($"{ts}{ts}/* ----------------------------------------- */");
                if (c.PTPData.PTPInstellingenInParameters)
                {
                    sb.AppendLine($"{ts}#if (defined AUTOMAAT)");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.PORTNR = PRM[{_prmpf}{prmportnr}{k.TeKoppelenKruispunt}];        /* poortnummer in het regeltoestel     */  /* @ door fabrikant aanpassen */");
                    sb.AppendLine($"{ts}#else");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.PORTNR = {k.PortnummerSimuatieOmgeving};        /* poortnr. testomgeving (schrijvend) */ /* @ nummer van KS-buffer */");
                    sb.AppendLine($"{ts}#endif");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.SRC  = (byte)PRM[{_prmpf}{prmsrc}{k.TeKoppelenKruispunt}];        /* nummer van source                   */ /* @ maximaal 255 */");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.DEST = (byte)PRM[{_prmpf}{prmdest}{k.TeKoppelenKruispunt}];       /* nummer van destination              */ /* @ maximaal 255 */");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.TMSGW_max= PRM[{_prmpf}{prmtmsgw}{k.TeKoppelenKruispunt}];   /* wait  time-out             */");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.TMSGS_max= PRM[{_prmpf}{prmtmsgs}{k.TeKoppelenKruispunt}];   /* send  time-out             */");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.TMSGA_max= PRM[{_prmpf}{prmtmsga}{k.TeKoppelenKruispunt}];   /* alive time-out             */");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.CMSG_max=  PRM[{_prmpf}{prmcmsg}{k.TeKoppelenKruispunt}];    /* max. berichtenteller tbv. herhaling */");
                }
                else
                {
                    sb.AppendLine($"{ts}#if (defined AUTOMAAT)");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.PORTNR = {k.PortnummerAutomaatOmgeving};        /* poortnummer in het regeltoestel     */  /* @ door fabrikant aanpassen */");
                    sb.AppendLine($"{ts}#else");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.PORTNR = {k.PortnummerSimuatieOmgeving};        /* poortnr. testomgeving (schrijvend) */ /* @ nummer van KS-buffer */");
                    sb.AppendLine($"{ts}#endif");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.SRC  = {k.NummerSource};       /* nummer van source                   */ /* @ maximaal 255 */");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.DEST = {k.NummerDestination};       /* nummer van destination              */ /* @ maximaal 255 */");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.TMSGW_max= 200;   /* wait  time-out             */");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.TMSGS_max=  10;   /* send  time-out             */");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.TMSGA_max=  10;   /* alive time-out             */");
                    sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}.CMSG_max=    3;   /* max. berichtenteller tbv. herhaling */");
                }
                sb.AppendLine();
                sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}KS.IKS_MAX = {k.AantalsignalenIn};   /* aantal inkomende koppelsignalen    */ /* @ verhogen in stappen van 8 */");
                sb.AppendLine($"{ts}{ts}PTP_{k.TeKoppelenKruispunt}KS.UKS_MAX = {k.AantalsignalenUit};   /* aantal uitgaande koppelsignalen    */ /* @ verhogen in stappen van 8 */");
                if (c.Data.CCOLVersie >= Models.Enumerations.CCOLVersieEnum.CCOL110)
                {
                    sb.AppendLine($"{ts}{ts}#if CCOL_V >= 110 && !defined NO_PTP_MULTIVALENT");
                    sb.AppendLine($"{ts}{ts}{ts}PTP_{k.TeKoppelenKruispunt}KS.IKSM_MAX = 0;");
                    sb.AppendLine($"{ts}{ts}{ts}PTP_{k.TeKoppelenKruispunt}KS.UKSM_MAX = 0;");
                    sb.AppendLine($"{ts}{ts}#endif");
                }
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
            foreach (var k in c.PTPData.PTPKoppelingen)
            {
                sb.AppendLine($"{ts}{ts}#ifdef PTP_{k.TeKoppelenKruispunt}PORT");
                if (c.Data.PracticeOmgeving) sb.AppendLine($"{ts}{ts}#ifndef PRACTICE_TEST");
                sb.AppendLine($"{ts}{ts}{ts}ptp_init(&PTP_{k.TeKoppelenKruispunt});");
                if (c.Data.PracticeOmgeving) sb.AppendLine($"{ts}{ts}#endif");
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
            if (c.Data.CCOLVersie >= Models.Enumerations.CCOLVersieEnum.CCOL110)
            {
                sb.AppendLine($"{ts}/* afzetten van statusbit in CIF_GPS[5] */");
                sb.AppendLine($"{ts}/* ------------------------------------ */");
                sb.AppendLine($"{ts}CIF_GPS[AUTSTATUS] &= ~CIFA_COMF;");
                sb.AppendLine();
            }
            foreach (var k in c.PTPData.PTPKoppelingen)
            {
                sb.AppendLine($"{ts}{ts}/* opzetten signalen van en naar {k.TeKoppelenKruispunt} */");
                sb.AppendLine($"#ifdef PTP_{k.TeKoppelenKruispunt}PORT");
                sb.AppendLine();
                sb.AppendLine($"{ts}{ts}/* nalopen in en uitgangssignalen */");
                sb.AppendLine($"{ts}{ts}");
                sb.AppendLine($"{ts}{ts}/* opzetten van uitgaande koppelsignalen PTP_{k.TeKoppelenKruispunt} */");
                sb.AppendLine($"{ts}{ts}/* ------------------------------------------------- */      ");
                sb.AppendLine($"{ts}{ts}if (CIF_WPS[CIF_PROG_STATUS] == CIF_STAT_REG) /* status regelen - set uitgaande koppelsignalen  */");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}for(i = 0; i < PTP_{k.TeKoppelenKruispunt}KS.UKS_MAX; ++i) PTP_{k.TeKoppelenKruispunt}KS.UKS[i] = IH[{_hpf}{k.TeKoppelenKruispunt}{hptpuks}01 + i] && PRM[{_prmpf}{k.TeKoppelenKruispunt}{prmptpuks}01 + i] >= 2 || PRM[{_prmpf}{k.TeKoppelenKruispunt}{prmptpuks}01 + i] == 1;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}else /* niet regelen - reset uitgaande koppelsignalen */");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}for(i = 0; i < PTP_{k.TeKoppelenKruispunt}KS.UKS_MAX; ++i) PTP_{k.TeKoppelenKruispunt}KS.UKS[i] = FALSE;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine();
                sb.AppendLine($"{ts}{ts}/* opzetten van ingaande koppelsignalen PTP_{k.TeKoppelenKruispunt} */");
                sb.AppendLine($"{ts}{ts}/* ---------------------------------------------- */");
                sb.AppendLine($"{ts}{ts}if (PTP_{k.TeKoppelenKruispunt}KS.OKE) /* goede verbinding - set ingaande koppelsignalen */");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}for(i = 0; i < PTP_{k.TeKoppelenKruispunt}KS.IKS_MAX; ++i) IH[{_hpf}{k.TeKoppelenKruispunt}{hptpiks}01 + i] = PTP_{k.TeKoppelenKruispunt}KS.IKS[i] && PRM[{_prmpf}{k.TeKoppelenKruispunt}{prmptpiks}01 + i] >= 2 || PRM[{_prmpf}{k.TeKoppelenKruispunt}{prmptpiks}01 + i] == 1;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}else /* geen goede verbinding - reset ingaande koppelsignalen */");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}    for(i = 0; i < PTP_{k.TeKoppelenKruispunt}KS.IKS_MAX; ++i) IH[{_hpf}{k.TeKoppelenKruispunt}{hptpiks}01 + i] = FALSE;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine();
                sb.AppendLine($"{ts}{ts}/* aanroep ptp-functies */");
                sb.AppendLine($"{ts}{ts}/* -------------------- */");
                if (c.Data.PracticeOmgeving) sb.AppendLine($"{ts}{ts}#ifndef PRACTICE_TEST");
                sb.AppendLine($"{ts}{ts}ptp_application_ks(&PTP_{k.TeKoppelenKruispunt}, &PTP_{k.TeKoppelenKruispunt}KS);");
                sb.AppendLine($"{ts}{ts}ptp_control(&PTP_{k.TeKoppelenKruispunt});");
                if (c.Data.PracticeOmgeving) sb.AppendLine($"{ts}{ts}#endif");
                sb.AppendLine();
                sb.AppendLine($"{ts}{ts}/* opzetten hulpelementen */");
                sb.AppendLine($"{ts}{ts}/* ---------------------- */");
                sb.AppendLine($"{ts}{ts}IH[{_hpf}{hptp}_{k.TeKoppelenKruispunt}{hptpoke}]  = PTP_{k.TeKoppelenKruispunt}KS.OKE;");
                sb.AppendLine($"{ts}{ts}IH[{_hpf}{hptp}_{k.TeKoppelenKruispunt}{hptperr}]  = PTP_{k.TeKoppelenKruispunt}.COMERROR;");
                sb.AppendLine($"{ts}{ts}IH[{_hpf}{hptp}_{k.TeKoppelenKruispunt}{hptperr0}] = PTP_{k.TeKoppelenKruispunt}.COMERROR & BIT0;");
                sb.AppendLine($"{ts}{ts}IH[{_hpf}{hptp}_{k.TeKoppelenKruispunt}{hptperr1}] = PTP_{k.TeKoppelenKruispunt}.COMERROR & BIT1;");
                sb.AppendLine($"{ts}{ts}IH[{_hpf}{hptp}_{k.TeKoppelenKruispunt}{hptperr2}] = PTP_{k.TeKoppelenKruispunt}.COMERROR & BIT2;");
                sb.AppendLine("");
                sb.AppendLine($"{ts}{ts}/* Bijhouden ptp errors   */");
                sb.AppendLine($"{ts}{ts}/* ---------------------- */");
                sb.AppendLine($"{ts}{ts}if (SH[{_hpf}{hptp}_{k.TeKoppelenKruispunt}{hptpoke}])");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}if((PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptpoke}] + 1) >= 32767)");
                sb.AppendLine($"{ts}{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}{ts}PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptpoke}] = 0;");
                sb.AppendLine($"{ts}{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptpoke}]++;");
                sb.AppendLine($"{ts}{ts}{ts}CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}if (SH[{_hpf}{hptp}_{k.TeKoppelenKruispunt}{hptperr}])");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}if((PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptperr}] + 1) >= 32767)");
                sb.AppendLine($"{ts}{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}{ts}PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptperr}] = 0;");
                sb.AppendLine($"{ts}{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptperr}]++;");
                sb.AppendLine($"{ts}{ts}{ts}CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}if (SH[{_hpf}{hptp}_{k.TeKoppelenKruispunt}{hptperr0}])");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}if((PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptperr0}] + 1) >= 32767)");
                sb.AppendLine($"{ts}{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}{ts}PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptperr0}] = 0;");
                sb.AppendLine($"{ts}{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptperr0}]++;");
                sb.AppendLine($"{ts}{ts}{ts}CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}if (SH[{_hpf}{hptp}_{k.TeKoppelenKruispunt}{hptperr1}])");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}if((PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptperr1}] + 1) >= 32767)");
                sb.AppendLine($"{ts}{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}{ts}PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptperr1}] = 0;");
                sb.AppendLine($"{ts}{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptperr1}]++;");
                sb.AppendLine($"{ts}{ts}{ts}CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}if (SH[{_hpf}{hptp}_{k.TeKoppelenKruispunt}{hptperr2}])");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}if((PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptperr2}] + 1) >= 32767)");
                sb.AppendLine($"{ts}{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}{ts}PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptperr2}] = 0;");
                sb.AppendLine($"{ts}{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}{prmptp}_{k.TeKoppelenKruispunt}{prmptperr2}]++; ");
                sb.AppendLine($"{ts}{ts}{ts}CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine();
                sb.AppendLine($"{ts}{ts}/* aansturing led op handbedieningspaneel en via IVERA op krspnt plaatje */");
                sb.AppendLine($"{ts}{ts}/* --------------------------------------------------------------------- */");
                sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{usptp}_{k.TeKoppelenKruispunt}{usptpoke}] = PTP_{k.TeKoppelenKruispunt}KS.OKE;  ");
                sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{usptp}_{k.TeKoppelenKruispunt}{usptperr}] = PTP_{k.TeKoppelenKruispunt}.COMERROR;");
                sb.AppendLine("");
                if (c.Data.CCOLVersie >= Models.Enumerations.CCOLVersieEnum.CCOL110)
                {
                    sb.AppendLine($"{ts}/* opzetten van statusbit in CIF_GPS[5] */");
                    sb.AppendLine($"{ts}/* ------------------------------------ */");
                    sb.AppendLine($"{ts}if (!PTP_{k.TeKoppelenKruispunt}KS.OKE)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}CIF_GPS[AUTSTATUS] |= CIFA_COMF;");
                    sb.AppendLine($"{ts}}}");
                }

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
            sb.AppendLine();
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) && defined (CCOL_EXIT)");
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
            foreach (var k in c.PTPData.PTPKoppelingen)
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
