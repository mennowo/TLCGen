using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GeneratePtpC(ControllerModel controller)
        {
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
#warning TODO the rest...

            return sb.ToString();
        }
    }
}
