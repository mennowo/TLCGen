using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateOvC(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* OV FUNCTIES APPLICATIE */");
            sb.AppendLine("/* ---------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "ov.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();

            sb.Append(GenerateOvCIncludes(controller));
            sb.Append(GenerateOvCTop(controller));
            sb.Append(GenerateOvCInit(controller));
            sb.Append(GenerateOvCInstellingen(controller));
            sb.Append(GenerateOvCRijTijdScenario(controller));
            sb.Append(GenerateOvCInUitMelden(controller));
            sb.Append(GenerateOvCPrioriteitsOpties(controller));
            sb.Append(GenerateOvCPrioriteitsToekenning(controller));
            sb.Append(GenerateOvCTegenhoudenConflicten(controller));
            sb.Append(GenerateOvCPostAfhandelingOV(controller));
            sb.Append(GenerateOvCPARCorrecties(controller));
            sb.Append(GenerateOvCPARCcol(controller));
            sb.Append(GenerateOvCSpecialSignals(controller));
            sb.Append(GenerateOvCBottom(controller));

            return sb.ToString();
        }

        private string GenerateOvCIncludes(ControllerModel c)
        {
            var sb = new StringBuilder();

            if (c.InterSignaalGroep.Nalopen.Any())
            {
                sb.AppendLine("#define NALOPEN");
                sb.AppendLine();
            }

            sb.AppendLine("/*include files */");
            sb.AppendLine("/*------------- */");
            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine("#ifndef _VRIWINTEST");
            }
            sb.AppendLine($"{ts}#include \"{c.Data.Naam}sys.h\"");
            sb.AppendLine($"{ts}#include \"fcvar.h\"    /* fasecycli                         */");
            sb.AppendLine($"{ts}#include \"kfvar.h\"    /* conflicten                        */");
            sb.AppendLine($"{ts}#include \"usvar.h\"    /* uitgangs elementen                */");
            sb.AppendLine($"{ts}#include \"dpvar.h\"    /* detectie elementen                */");
            sb.AppendLine($"{ts}#include \"isvar.h\"    /* ingangs elementen                 */");
            sb.AppendLine($"{ts}#include \"hevar.h\"    /* hulp elementen                    */");
            sb.AppendLine($"{ts}#include \"mevar.h\"    /* geheugen elementen                */");
            sb.AppendLine($"{ts}#include \"tmvar.h\"    /* tijd elementen                    */");
            sb.AppendLine($"{ts}#include \"ctvar.h\"    /* teller elementen                  */");
            sb.AppendLine($"{ts}#include \"schvar.h\"   /* software schakelaars              */");
            sb.AppendLine($"{ts}#include \"prmvar.h\"   /* parameters                        */");
            sb.AppendLine($"{ts}#include \"lwmlvar.h\"  /* langstwachtende modulen structuur */");
            sb.AppendLine($"{ts}#include \"control.h\"  /* controller interface              */");
            sb.AppendLine($"{ts}#include \"rtappl.h\"   /* applicatie routines               */");
            sb.AppendLine($"{ts}#include \"stdfunc.h\"  /* standaard functies                */");
            sb.AppendLine($"{ts}#include \"cif.inc\"    /* interface                         */");
            sb.AppendLine($"{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)");
            sb.AppendLine($"{ts}{ts}#include \"xyprintf.h\" /* Printen debuginfo                 */");
            sb.AppendLine($"{ts}#endif");
            sb.AppendLine($"{ts}#include \"ccolfunc.h\"");
            sb.AppendLine($"{ts}#include \"ccol_mon.h\"");
            sb.AppendLine($"{ts}#include \"extra_func.h\"");
            sb.AppendLine($"{ts}#include \"extra_func_ov.h\"");
            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine("#endif // _VRIWINTEST");
            }

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCIncludes, true, true, true, true);

			return sb.ToString();
        }

        private string GenerateOvCTop(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#define MAX_AANTAL_INMELDINGEN           10");
            sb.AppendLine("#define DEFAULT_MAX_WACHTTIJD           120");
            sb.AppendLine("#define NO_REALISEREN_TOEGESTAAN");
            sb.AppendLine("#define OV_ADDFILE");
            if (c.InterSignaalGroep.Nalopen.Any())
            {
                sb.AppendLine("#define NALOPEN");
            }

            if (c.HalfstarData.IsHalfstar)
            {
                sb.AppendLine();
                sb.AppendLine("/* Declareren OV settings functie halfstar */");
            }

            sb.AppendLine();
            sb.AppendLine("extern mulv DB_old[];");
            sb.AppendLine("extern mulv TDH_old[];");
            sb.AppendLine();
            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine("#ifndef _VRIWINTEST");
                sb.AppendLine("#include \"ov.c\"");
                sb.AppendLine("#else");
                sb.AppendLine("#include \"ov.h\"");
                sb.AppendLine("const code *iFC_OV_code[ovOVMAX];");
                sb.AppendLine("#endif");
            }
            else
            {
                sb.AppendLine("#include \"ov.c\"");
            }
            if (c.HalfstarData.IsHalfstar)
            {
                sb.AppendLine("#include \"halfstar_ov.h\"");
            }
            sb.AppendLine();
            if (c.HasKAR())
            {
                sb.AppendLine("/* Variabele tbv start KAR ondergedrag timer bij starten regeling */");
                sb.AppendLine("static char startkarog = FALSE;");
            }

	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCTop, true, true, true, true);
	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCBottom, true, false, false, true);

            if (c.Data.CCOLVersie <= CCOLVersieEnum.CCOL8 && c.Data.VLOGType != VLOGTypeEnum.Geen ||
                c.Data.CCOLVersie > CCOLVersieEnum.CCOL8 && c.Data.VLOGSettings.VLOGToepassen)
            {
                sb.AppendLine("/* VLOG mon5 buffer: monitoring/logging OV */");
                sb.AppendLine("#ifndef NO_VLOG");
                sb.AppendLine($"{ts}#ifndef NO_VLOG_200");
                sb.AppendLine($"{ts}{ts}/* VLOG_mon5_buffer afhandeling */");
                sb.AppendLine($"{ts}{ts}/* ---------------------------- */");
                sb.AppendLine($"{ts}{ts}void VLOG_mon5_buffer(void)");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}int ifc;");
                sb.AppendLine($"{ts}{ts}{ts}for (ifc = 0; ifc < FCMAX; ++ifc)");
                sb.AppendLine($"{ts}{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}{ts}mon5_buffer(SAPPLPROG, ifc,");
                sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}VLOG_mon5[ifc].voorinov,");
                sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}VLOG_mon5[ifc].inmov,");
                sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}VLOG_mon5[ifc].uitmov,");
                sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}VLOG_mon5[ifc].uitmbewov,");
                sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}VLOG_mon5[ifc].foutuitmov,");
                sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}VLOG_mon5[ifc].uituitmov,");
                sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}VLOG_mon5[ifc].voorinhd,");
                sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}VLOG_mon5[ifc].inmhd,");
                sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}VLOG_mon5[ifc].uitmhd,");
                sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}VLOG_mon5[ifc].uitmbewhd);");
                sb.AppendLine($"{ts}{ts}{ts}{ts}VLOG_mon5[ifc].voorinov = FALSE;");
                sb.AppendLine($"{ts}{ts}{ts}{ts}VLOG_mon5[ifc].inmov = FALSE;");
                sb.AppendLine($"{ts}{ts}{ts}{ts}VLOG_mon5[ifc].uitmov = FALSE;");
                sb.AppendLine($"{ts}{ts}{ts}{ts}VLOG_mon5[ifc].uitmbewov = FALSE;");
                sb.AppendLine($"{ts}{ts}{ts}{ts}VLOG_mon5[ifc].foutuitmov = FALSE;");
                sb.AppendLine($"{ts}{ts}{ts}{ts}VLOG_mon5[ifc].uituitmov = FALSE;");
                sb.AppendLine($"{ts}{ts}{ts}{ts}VLOG_mon5[ifc].voorinhd = FALSE;");
                sb.AppendLine($"{ts}{ts}{ts}{ts}VLOG_mon5[ifc].inmhd = FALSE;");
                sb.AppendLine($"{ts}{ts}{ts}{ts}VLOG_mon5[ifc].uitmhd = FALSE;");
                sb.AppendLine($"{ts}{ts}{ts}{ts}VLOG_mon5[ifc].uitmbewhd = FALSE;");
                sb.AppendLine($"{ts}{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}#endif");
                sb.AppendLine($"#endif");
            }

			sb.AppendLine("/*-------------------------------------------------------------------------------------------");
            sb.AppendLine("   OVInstellingen voorziet alle OV-instellingen van een juiste waarde.");
            sb.AppendLine("   Het gaat om de volgende instellingen:");
            sb.AppendLine("   - iFC_OVix[ov]                        : (de index van) de fasecyclus van de OV-richting ov.");
            sb.AppendLine("   - iT_GBix[ov]                         : (de index van) de timer voor de groenbewaking");
            sb.AppendLine("                                           van OV-richting ov.");
            sb.AppendLine("   - iH_OVix[ov]                         : (de index van) het hulpelement voor de");
            sb.AppendLine("                                           prioriteitstoekenning van OV-richting ov.");
            sb.AppendLine("   - iInstPrioriteitsNiveau[ov]          : het prioriteitsniveau van de inmeldingen van de");
            sb.AppendLine("                                           OV-richting ov.");
            sb.AppendLine("   - iInstPrioriteitsOpties[ov]          : de prioriteisopties van de inmeldingen van de");
            sb.AppendLine("                                           OV-richting ov.");
            sb.AppendLine("   - iGroenBewakingsTijd[ov]             : De groenbewakingstijd van de OV-richting ov.");
            sb.AppendLine("   - iRTSOngehinderd[ov]                 : De ongehinderde rijtijd van OV-richting ov.");
            sb.AppendLine("   - iRTSBeperktGehinderd[ov]            : De beperkt gehinderde rijtijd van OV-richting ov.");
            sb.AppendLine("   - iRTSGehinderd[ov]                   : De gehinderde rijtijd van OV-richting ov.");
            sb.AppendLine("   - iOnderMaximum[ov]                   : Het ondermaximum van OV-richting ov.");
            sb.AppendLine("   - iSelDetFoutNaGB[ov]                 : De in- en uitmelddetectie van OV-richting ov");
            sb.AppendLine("                                           wordt als defect beschouwd bij het aanspreken");
            sb.AppendLine("                                           van de groenbewaking. De defectmelding wordt");
            sb.AppendLine("                                           opgeheven bij de eerstvolgende uitmelding.");
            sb.AppendLine("   - iMaximumWachtTijd[fc]               : de maximum wachttijd voor richting fc waarboven");
            sb.AppendLine("                                           geen prioriteit meer wordt toegekend voor");
            sb.AppendLine("                                           niet-nooddienst-inmeldingen op de OV-richtingen.");
            sb.AppendLine("   - iInstPercMaxGroenTijdTerugKomen[fc] : percentage van de maximumgroentijd van richting fc");
            sb.AppendLine("                                           waaronder na afkappen de richting terugkomt.");
            sb.AppendLine("   - iInstMinTerugKomGroenTijd[fc]       : minimale maximumgroentijd van richting fc bij");
            sb.AppendLine("                                           terugkomen.");
            sb.AppendLine("   - iInstAantalMalenNietAfkappen[fc]    : aantal malen niet afkappen van richting fc na te");
            sb.AppendLine("                                           zijn afgekapt, tenzij inmelding nooddienst.");
            sb.AppendLine("   - iAfkapGroenTijd[fc]                 : groentijd waaronder richting fc niet mag worden");
            sb.AppendLine("                                           afgekapt, tenzij inmelding nooddienst.");
            sb.AppendLine("   - iPercGroenTijd[fc]                  : percentage van de maximumgroentijd dat richting fc");
            sb.AppendLine("                                           moet hebben gemaakt, voordat deze mag worden");
            sb.AppendLine("                                           afgekapt.");
            sb.AppendLine("   - iInstOphoogPercentageMG[fc]         : het percentage van de maximumgroentijd dat een");
            sb.AppendLine("                                           richting minimaal mag maken alvorens te worden");
            sb.AppendLine("                                           afgekapt, tenzij er sprake is van een nooddienst,");
            sb.AppendLine("                                           wordt telkens bij afkappen verhoogd met dit");
            sb.AppendLine("                                           ophoogpercentage maximumgroentijd totdat 100% is");
            sb.AppendLine("                                           gehaald (daarna reset).");
            sb.AppendLine("   - iPRM_ALTP[fc]                       : ruimte die minimaal aanwezig moet zijn voor");
            sb.AppendLine("                                           richting fc, om alternatief te realiseren tijdens");
            sb.AppendLine("                                           niet-konflikterende OV-ingre(e)p(en).");
            sb.AppendLine("   - iSCH_ALTG[fc]                       : richting fc mag alternatief realiseren tijdens");
            sb.AppendLine("                                           niet-konflikterende OV-ingre(e)p(en).");
            sb.AppendLine("   - iInstAfkapGroenAlt[fc]              : groentijd die richting fc minimaal mag maken");
            sb.AppendLine("                                           bij een alternatieve realisatie, voordat deze");
            sb.AppendLine("                                           mag worden afgekapt.");
            sb.AppendLine("   - iLangstWachtendeAlternatief         : de regeling maakt gebruik van");
            sb.AppendLine("                                           langstwachtende alternatief");
            sb.AppendLine("   ------------------------------------------------------------------------------------------- */");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateOvCInit(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void OVInitExtra(void) ");
            sb.AppendLine("{");
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateOvCInstellingen(ControllerModel c)
        {
            var sb = new StringBuilder();

            var _tgb = CCOLGeneratorSettingsProvider.Default.GetElementName("tgb");
            var _tgbhd = CCOLGeneratorSettingsProvider.Default.GetElementName("tgbhd");
            var _prmrto = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrto");
            var _prmrtbg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtbg");
            var _prmrtg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtg");
            var _prmrtohd = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtohd");
            var _prmrtbghd = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtbghd");
            var _prmrtghd = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtghd");
            var _hov = CCOLGeneratorSettingsProvider.Default.GetElementName("hov");
            var _hhd = CCOLGeneratorSettingsProvider.Default.GetElementName("hhd");
            var _hovin = CCOLGeneratorSettingsProvider.Default.GetElementName("hovin");
            var _hhdin = CCOLGeneratorSettingsProvider.Default.GetElementName("hhdin");
            var _hovuit = CCOLGeneratorSettingsProvider.Default.GetElementName("hovuit");
            var _hhduit = CCOLGeneratorSettingsProvider.Default.GetElementName("hhduit");
            var _prmprio = CCOLGeneratorSettingsProvider.Default.GetElementName("prmprio");
            var _prmpriohd = CCOLGeneratorSettingsProvider.Default.GetElementName("prmpriohd");
            var _prmomx = CCOLGeneratorSettingsProvider.Default.GetElementName("prmomx");
            var _tblk = CCOLGeneratorSettingsProvider.Default.GetElementName("tblk");
            var _schupinagb = CCOLGeneratorSettingsProvider.Default.GetElementName("schupinagb");
            var _schupinagbhd = CCOLGeneratorSettingsProvider.Default.GetElementName("schupinagbhd");
            var _prmmwta = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmwta");
            var _prmmwtfts = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmwtfts");
            var _prmmwtvtg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmwtvtg");
            var _prmpmgt = CCOLGeneratorSettingsProvider.Default.GetElementName("prmpmgt");
            var _prmognt = CCOLGeneratorSettingsProvider.Default.GetElementName("prmognt");
            var _prmnofm = CCOLGeneratorSettingsProvider.Default.GetElementName("prmnofm");
            var _prmmgcov = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmgcov");
            var _prmpmgcov = CCOLGeneratorSettingsProvider.Default.GetElementName("prmpmgcov");
            var _prmaltp = CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltp");
            var _prmaltg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltg");
            var _schaltg = CCOLGeneratorSettingsProvider.Default.GetElementName("schaltg");
            var _prmohpmg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmohpmg");

            sb.AppendLine("void OVInstellingen(void) ");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}/* ============================= */");
            sb.AppendLine($"{ts}/* Instellingen OV-richtingen    */");
            sb.AppendLine($"{ts}/* ============================= */");
            sb.AppendLine();

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCInstellingen, true, false, true, true);

            if (c.HasPTorHD())
            {
                sb.AppendLine($"{ts}/* Fasecyclus voor OV-richtingen */");
                foreach (var ov in c.OVData.OVIngrepen)
                {
                    sb.AppendLine($"{ts}iFC_OVix[ovFC{ov.FaseCyclus}] = {_fcpf}{ov.FaseCyclus};");
                }
                foreach (var hd in c.OVData.HDIngrepen)
                {
                    sb.AppendLine($"{ts}iFC_OVix[hdFC{hd.FaseCyclus}] = {_fcpf}{hd.FaseCyclus};");
                }
                sb.AppendLine();

                if (c.Data.PracticeOmgeving)
                {
                    sb.AppendLine($"{ts}/* Code voor OV-richtingen */");
                    sb.AppendLine($"{ts}#ifdef _VRIWINTEST");
                    foreach (var ov in c.OVData.OVIngrepen)
                    {
                        sb.AppendLine($"{ts}{ts}iFC_OV_code[ovFC{ov.FaseCyclus}] = \"ov{ov.FaseCyclus}\";");
                    }
                    foreach (var hd in c.OVData.HDIngrepen)
                    {
                        sb.AppendLine($"{ts}{ts}iFC_OV_code[hdFC{hd.FaseCyclus}] = \"hd{hd.FaseCyclus}\";");
                    }
                    sb.AppendLine($"{ts}#endif // _VRIWINTEST");
                    sb.AppendLine();
                }
            }

            sb.AppendLine($"{ts}/* Index van de groenbewakingstimer */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}iT_GBix[ovFC{ov.FaseCyclus}] = {_tpf}{_tgb}{ov.FaseCyclus};");
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iT_GBix[hdFC{hd.FaseCyclus}] = {_tpf}{_tgbhd}{hd.FaseCyclus};");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Index van het hulpelement voor de ingreep */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}iH_OVix[ovFC{ov.FaseCyclus}] = {_hpf}{_hov}{ov.FaseCyclus};");
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iH_OVix[hdFC{hd.FaseCyclus}] = {_hpf}{_hhd}{hd.FaseCyclus};");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Prioriteitsniveau */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}iInstPrioriteitsNiveau[ovFC{ov.FaseCyclus}] = PRM[{_prmpf}{_prmprio}{ov.FaseCyclus}]/1000L;");
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iInstPrioriteitsNiveau[hdFC{hd.FaseCyclus}] = PRM[{_prmpf}{_prmpriohd}{hd.FaseCyclus}]/1000L;");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Prioriteitsopties */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}iInstPrioriteitsOpties[ovFC{ov.FaseCyclus}] = BepaalPrioriteitsOpties({_prmpf}{_prmprio}{ov.FaseCyclus});");
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iInstPrioriteitsOpties[hdFC{hd.FaseCyclus}] = BepaalPrioriteitsOpties({_prmpf}{_prmpriohd}{hd.FaseCyclus});");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Groenbewakingstijd */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}iGroenBewakingsTijd[ovFC{ov.FaseCyclus}] = T_max[{_tpf}{_tgb}{ov.FaseCyclus}];");
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iGroenBewakingsTijd[hdFC{hd.FaseCyclus}] = T_max[{_tpf}{_tgbhd}{hd.FaseCyclus}];");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Ongehinderde rijtijd */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}iRTSOngehinderd[ovFC{ov.FaseCyclus}] = PRM[{_prmpf}{_prmrto}{ov.FaseCyclus}];");
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iRTSOngehinderd[hdFC{hd.FaseCyclus}] = PRM[{_prmpf}{_prmrtohd}{hd.FaseCyclus}];");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Beperkt gehinderde rijtijd */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}iRTSBeperktGehinderd[ovFC{ov.FaseCyclus}] = PRM[{_prmpf}{_prmrtbg}{ov.FaseCyclus}];");
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iRTSBeperktGehinderd[hdFC{hd.FaseCyclus}] = PRM[{_prmpf}{_prmrtbghd}{hd.FaseCyclus}];");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Gehinderde rijtijd */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}iRTSGehinderd[ovFC{ov.FaseCyclus}] = PRM[{_prmpf}{_prmrtg}{ov.FaseCyclus}];");
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iRTSGehinderd[hdFC{hd.FaseCyclus}] = PRM[{_prmpf}{_prmrtghd}{hd.FaseCyclus}];");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Ondermaximum */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}iOnderMaximum[ovFC{ov.FaseCyclus}] = PRM[{_prmpf}{_prmomx}{ov.FaseCyclus}];");
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iOnderMaximum[hdFC{hd.FaseCyclus}] = 0;");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Blokkeringstijd */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}iBlokkeringsTijd[ovFC{ov.FaseCyclus}] = T_max[{_tpf}{_tblk}{ov.FaseCyclus}];");
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iBlokkeringsTijd[hdFC{hd.FaseCyclus}] = 0;");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Na aanspreken groenbewaking wordt de selectieve ");
            sb.AppendLine("	   detectie niet langer betrouwbaar gevonden */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}iSelDetFoutNaGB[ovFC{ov.FaseCyclus}] = SCH[{_schpf}{_schupinagb}{ov.FaseCyclus}];");
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iSelDetFoutNaGB[hdFC{hd.FaseCyclus}] = SCH[{_schpf}{_schupinagbhd}{hd.FaseCyclus}];");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* =============================== */");
            sb.AppendLine($"{ts}/* Instellingen overige richtingen */");
            sb.AppendLine($"{ts}/* =============================== */");
            sb.AppendLine("   ");
            sb.AppendLine($"{ts}/* Maximale wachttijd */");
            foreach (var fc in c.Fasen)
            {
                switch (fc.Type)
                {
                    case FaseTypeEnum.OV:
                    case FaseTypeEnum.Auto:
                        sb.AppendLine($"{ts}iMaximumWachtTijd[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmmwta}];");
                        break;
                    case FaseTypeEnum.Fiets:
                        sb.AppendLine($"{ts}iMaximumWachtTijd[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmmwtfts}];");
                        break;
                    case FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iMaximumWachtTijd[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmmwtvtg}];");
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Als een richting minder groen heeft gehad door afkappen");
            sb.AppendLine($"{ts}   dan deze instelling, dan mag de richting nog een keer");
            sb.AppendLine($"{ts}   primair realiseren (terugkomen). */");
            foreach (var ovfc in c.OVData.OVIngreepSignaalGroepParameters)
            {
                if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, ovfc.FaseCyclus))
                {
                    continue;
                }

                var fct = c.Fasen.First(x => x.Naam == ovfc.FaseCyclus).Type;
                switch (fct)
                {
                    case FaseTypeEnum.OV:
                    case FaseTypeEnum.Auto:
                    case FaseTypeEnum.Fiets:
                    case FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iInstPercMaxGroenTijdTerugKomen[{_fcpf}{ovfc.FaseCyclus}] = PRM[{_prmpf}{_prmpmgt}{ovfc.FaseCyclus}];");
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* De minimale groentijd die een richting krijgt als");
            sb.AppendLine($"{ts}   deze mag terugkomen. */");
            foreach (var ovfc in c.OVData.OVIngreepSignaalGroepParameters)
            {
                if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, ovfc.FaseCyclus))
                {
                    continue;
                }

                var fct = c.Fasen.First(x => x.Naam == ovfc.FaseCyclus).Type;
                switch (fct)
                {
                    case FaseTypeEnum.OV:
                    case FaseTypeEnum.Auto:
                    case FaseTypeEnum.Fiets:
                    case FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iInstMinTerugKomGroenTijd[{_fcpf}{ovfc.FaseCyclus}] = PRM[{_prmpf}{_prmognt}{ovfc.FaseCyclus}];");
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Aantal malen niet afkappen */");
            foreach (var ovfc in c.OVData.OVIngreepSignaalGroepParameters)
            {
                if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, ovfc.FaseCyclus))
                {
                    continue;
                }

                var fct = c.Fasen.First(x => x.Naam == ovfc.FaseCyclus).Type;
                switch (fct)
                {
                    case FaseTypeEnum.Voetganger:
                        continue;
                    case FaseTypeEnum.OV:
                    case FaseTypeEnum.Auto:
                    case FaseTypeEnum.Fiets:
                        sb.AppendLine($"{ts}iInstAantalMalenNietAfkappen[{_fcpf}{ovfc.FaseCyclus}] = PRM[{_prmpf}{_prmnofm}{ovfc.FaseCyclus}];");
                        break;
	                default:
		                throw new ArgumentOutOfRangeException();
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Onder deze groentijd mag een richting niet worden");
            sb.AppendLine($"{ts}   afgekapt, tenzij zich een nooddienst heeft ingemeld. */");
            foreach (var ovfc in c.OVData.OVIngreepSignaalGroepParameters)
            {
                if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, ovfc.FaseCyclus))
                {
                    continue;
                }

                var fct = c.Fasen.First(x => x.Naam == ovfc.FaseCyclus).Type;
                switch (fct)
                {
                    case FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iAfkapGroenTijd[{_fcpf}{ovfc.FaseCyclus}] = 0;");
                        continue;
                    case FaseTypeEnum.OV:
                    case FaseTypeEnum.Auto:
                    case FaseTypeEnum.Fiets:
                        sb.AppendLine($"{ts}iAfkapGroenTijd[{_fcpf}{ovfc.FaseCyclus}] = PRM[{_prmpf}{_prmmgcov}{ovfc.FaseCyclus}];");
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Als een richting minder groen heeft gehad dan");
            sb.AppendLine($"{ts}   dit percentage van de maximum groentijd, dan");
            sb.AppendLine($"{ts}   mag de richting niet worden afgekapt, tenzij");
            sb.AppendLine($"{ts}   zich een nooddienst heeft ingemeld. */");
            foreach (var ovfc in c.OVData.OVIngreepSignaalGroepParameters)
            {
                if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, ovfc.FaseCyclus))
                {
                    continue;
                }

                var fct = c.Fasen.First(x => x.Naam == ovfc.FaseCyclus).Type;
                switch (fct)
                {
                    case FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iPercGroenTijd[{_fcpf}{ovfc.FaseCyclus}] = 100; /* Voetgangers mogen niet worden afgekapt. */");
                        continue;
                    case FaseTypeEnum.OV:
                    case FaseTypeEnum.Auto:
                    case FaseTypeEnum.Fiets:
                        sb.AppendLine($"{ts}iPercGroenTijd[{_fcpf}{ovfc.FaseCyclus}] = PRM[{_prmpf}{_prmpmgcov}{ovfc.FaseCyclus}];");
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Na te zijn afgekapt, wordt het percentage");
            sb.AppendLine($"{ts}   van de maximumgroentijd verhoogd met dit ophoogpercentage. */");
            foreach (var ovfc in c.OVData.OVIngreepSignaalGroepParameters)
            {
                if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, ovfc.FaseCyclus))
                {
                    continue;
                }
                var fct = c.Fasen.First(x => x.Naam == ovfc.FaseCyclus).Type;

                switch (fct)
                {
                    case FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iInstOphoogPercentageMG[{_fcpf}{ovfc.FaseCyclus}] = 0;");
                        continue;
                    case FaseTypeEnum.OV:
                    case FaseTypeEnum.Auto:
                    case FaseTypeEnum.Fiets:
                        sb.AppendLine($"{ts}iInstOphoogPercentageMG[{_fcpf}{ovfc.FaseCyclus}] = PRM[{_prmpf}{_prmohpmg}{ovfc.FaseCyclus}];");
                        break;
                }
            }
            sb.AppendLine();

            if (c.ModuleMolen.LangstWachtendeAlternatief)
            {
                var gelijkstarttuples = CCOLCodeHelper.GetFasenWithGelijkStarts(c);

                sb.AppendLine($"{ts}/* Benodigde ruimte voor alternatieve realisatie tijdens een OV ingreep */");
                foreach (var fc in c.ModuleMolen.FasenModuleData)
                {
                    Tuple<string, List<string>> hasgs = null;
                    foreach (var gs in gelijkstarttuples)
                    {
                        if (gs.Item1 == fc.FaseCyclus && gs.Item2.Count > 1)
                        {
                            hasgs = gs;
                            break;
                        }
                    }
                    if (hasgs != null)
                    {
                        sb.Append($"{ts}iPRM_ALTP[{_fcpf}{fc.FaseCyclus}] = PRM[{_prmpf}{_prmaltp}");
                        foreach (var ofc in hasgs.Item2)
                        {
                            sb.Append(ofc);
                        }
                        sb.AppendLine("];");
                    }
                    else
                    {
                        sb.AppendLine($"{ts}iPRM_ALTP[{_fcpf}{fc.FaseCyclus}] = PRM[{_prmpf}{_prmaltp}{fc.FaseCyclus}];");
                    }

                }
                sb.AppendLine();

                sb.AppendLine($"{ts}/* Richting mag alternatief realiseren tijdens een OV ingreep*/");
                foreach (var fc in c.ModuleMolen.FasenModuleData)
                {
                    Tuple<string, List<string>> hasgs = null;
                    foreach (var gs in gelijkstarttuples)
                    {
                        if (gs.Item1 == fc.FaseCyclus && gs.Item2.Count > 1)
                        {
                            hasgs = gs;
                            break;
                        }
                    }
                    if (hasgs != null)
                    {
                        sb.Append($"{ts}iSCH_ALTG[{_fcpf}{fc.FaseCyclus}] = SCH[{_schpf}{_schaltg}");
                        foreach (var ofc in hasgs.Item2)
                        {
                            sb.Append(ofc);
                        }
                        sb.AppendLine("];");
                    }
                    else
                    {
                        sb.AppendLine($"{ts}iSCH_ALTG[{_fcpf}{fc.FaseCyclus}] = SCH[{_schpf}{_schaltg}{fc.FaseCyclus}];");
                    }
                }
                sb.AppendLine();

                sb.AppendLine($"{ts}/* Alternatieve realisatie mag worden");
                sb.AppendLine($"{ts}   afgekapt tijdens een OV ingreep als");
                sb.AppendLine($"{ts}   deze groentijd is gemaakt */");
                foreach (var fc in c.Fasen)
                {
                    sb.AppendLine($"{ts}iInstAfkapGroenAlt[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmaltg}{fc.Naam}];");
                }
                sb.AppendLine();

                sb.AppendLine($"{ts}/* De regeling maakt gebruik van langstwachtende alternatief */");
                sb.AppendLine($"{ts}iLangstWachtendeAlternatief = 1;");
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine($"{ts}/* De regeling maakt geen gebruik van langstwachtende alternatief */");
                sb.AppendLine($"{ts}iLangstWachtendeAlternatief = 0;");
                sb.AppendLine();
            }

			AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCInstellingen, false, true, false, false);

			sb.AppendLine("}");
            sb.AppendLine();

			return sb.ToString();
        }

        private string GenerateOvCRijTijdScenario(ControllerModel c)
        {
            var sb = new StringBuilder();

            var _tbtovg = CCOLGeneratorSettingsProvider.Default.GetElementName("tbtovg");
            var _cvc = CCOLGeneratorSettingsProvider.Default.GetElementName("cvc");
            var _schvi = CCOLGeneratorSettingsProvider.Default.GetElementName("schvi");
            var _schgeenwissel = CCOLGeneratorSettingsProvider.Default.GetElementName("schgeenwissel");
            var _schwisselpol = CCOLGeneratorSettingsProvider.Default.GetElementName("schwisselpol");
            var _hwissel = CCOLGeneratorSettingsProvider.Default.GetElementName("hwissel");

            sb.AppendLine("/* -----------------------------------------------------------");
            sb.AppendLine("   RijTijdScenario bepaalt het actieve rijtijdscenario");
            sb.AppendLine("   van alle OV-richtingen. Het resultaat wordt geplaatst");
            sb.AppendLine("   in de variabele iRijTijdScenario, en kan als waarde");
            sb.AppendLine("   rtsOngehinderd, rtsBeperktGehinderd of rtsGehinderd hebben.");
            sb.AppendLine("   Het rijtijdscenario is bepalend voor de gehanteerde");
            sb.AppendLine("   rijtijd.");
            sb.AppendLine("   ----------------------------------------------------------- */");
            sb.AppendLine("void RijTijdScenario(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}/* Vaststellen rijtijdscenarios */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                foreach (var fc in c.Fasen)
                {
                    if (ov.FaseCyclus == fc.Naam)
                    {
                        bool wissel = false;
                        if (ov.KoplusKijkNaarWisselstand && ov.HasOVIngreepWissel())
                        {
                            wissel = true;
                            sb.Append($"{ts}if (IH[{_hpf}{_hwissel}{ov.FaseCyclus}])");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}OVRijTijdScenario(ovFC{fc.Naam}, {_dpf}{ov.Koplus}, NG, NG);");
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine($"{ts}else");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}OVRijTijdScenario(ovFC{fc.Naam}, NG, NG, NG);");
                            sb.AppendLine($"{ts}}}");
                        }
                        else
                        {
                            var tts = wissel ? ts + ts : ts;
                            if (fc.Detectoren.Any(x => x.Type == DetectorTypeEnum.Kop || x.Type == DetectorTypeEnum.Lang))
                            {
                                var kl = fc.Detectoren.Where(x => x.Type == DetectorTypeEnum.Kop).ToList();
                                var ll = fc.Detectoren.Where(x => x.Type == DetectorTypeEnum.Lang).ToList();
                                if (ll.Count <= kl.Count)
                                {
                                    var i = 0;
                                    foreach (var d in kl)
                                    {
                                        sb.Append($"{tts}OVRijTijdScenario(ovFC{fc.Naam}, {_dpf}{d.Naam}, ");
                                        if (i < ll.Count)
                                        {
                                            sb.AppendLine($"{_dpf}{ll[i].Naam}, {_tpf}{_tbtovg}{fc.Naam});");
                                            ++i;
                                        }
                                        else
                                        {
                                            sb.AppendLine("NG, NG);");
                                        }
                                    }
                                }
                                else
                                {
                                    var i = 0;
                                    foreach (var d in ll)
                                    {
                                        sb.Append($"{tts}OVRijTijdScenario(ovFC{fc.Naam}, ");
                                        if (i < kl.Count)
                                        {
                                            sb.AppendLine($"{_dpf}{kl[i].Naam}, {_dpf}{d.Naam}, {_tpf}{_tbtovg}{fc.Naam});");
                                            ++i;
                                        }
                                        else
                                        {
                                            sb.AppendLine($"NG, {_dpf}{d.Naam}, NG);");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                sb.Append($"{tts}OVRijTijdScenario(ovFC{fc.Naam}, NG, NG, NG);");
                            }
                            sb.AppendLine();
                        }

                        break;
                    }
                }
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                foreach (var fc in c.Fasen)
                {
                    if (hd.FaseCyclus == fc.Naam)
                    {
                        if (fc.Detectoren.Count > 0)
                        {
                            var kl = fc.Detectoren.Where(x => x.Type == DetectorTypeEnum.Kop).ToList();
                            var ll = fc.Detectoren.Where(x => x.Type == DetectorTypeEnum.Lang).ToList();
                            if (ll.Count <= kl.Count)
                            {
                                var i = 0;
                                foreach (var d in kl)
                                {
                                    sb.Append($"{ts}OVRijTijdScenario(hdFC{fc.Naam}, {_dpf}{d.Naam}, ");
                                    if (i < ll.Count)
                                    {
                                        sb.AppendLine($"{_dpf}{ll[i].Naam}, {_tpf}{_tbtovg}{fc.Naam});");
                                        ++i;
                                    }
                                    else
                                    {
                                        sb.AppendLine("NG, NG);");
                                    }
                                }
                            }
                            else
                            {
                                var i = 0;
                                foreach (var d in ll)
                                {
                                    sb.Append($"{ts}OVRijTijdScenario(hdFC{fc.Naam}, ");
                                    if (i < kl.Count)
                                    {
                                        sb.AppendLine($"{_dpf}{kl[i].Naam}, {_dpf}{d.Naam}, {_tpf}{_tbtovg}{fc.Naam});");
                                        ++i;
                                    }
                                    else
                                    {
                                        sb.AppendLine($"NG, {_dpf}{d.Naam}, NG);");
                                    }
                                }
                            }
                        }
                        else
                        {
                            sb.Append($"{ts}OVRijTijdScenario(hdFC{fc.Naam}, NG, NG, NG);");
                        }
                        break;
                    }
                }
            }
            sb.AppendLine();
            foreach (var ov in c.OVData.OVIngrepen.Where(x => x.VersneldeInmeldingKoplus != NooitAltijdAanUitEnum.Nooit && !string.IsNullOrWhiteSpace(x.Koplus) && x.Koplus != "NG"))
            {
                sb.Append(ov.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.Altijd
                        ? $"{ts}if (DB[{_dpf}{ov.Koplus}] && C[{_cpf}{_cvc}{ov.FaseCyclus}] && (iRijTimer[ovFC{ov.FaseCyclus}] < iRijTijd[ovFC{ov.FaseCyclus}])"
			            : $"{ts}if (SCH[{_schpf}{_schvi}{ov.FaseCyclus}] && DB[{_dpf}{ov.Koplus}] && C[{_cpf}{_cvc}{ov.FaseCyclus}] && (iRijTimer[ovFC{ov.FaseCyclus}] < iRijTijd[ovFC{ov.FaseCyclus}])");
                if (ov.KoplusKijkNaarWisselstand && ov.HasOVIngreepWissel())
                {
                    sb.Append($" && IH[{_hpf}{_hwissel}{ov.FaseCyclus}]");
                }
                sb.AppendLine($") iRijTijd[ovFC{ov.FaseCyclus}] = 0;");
            }

			if (OrderedPieceGenerators[CCOLCodeTypeEnum.OvCRijTijdScenario].Any())
	        {
		        sb.AppendLine();
		        foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.OvCRijTijdScenario])
		        {
			        sb.Append(gen.Value.GetCode(c, CCOLCodeTypeEnum.OvCRijTijdScenario, ts));
		        }
	        }
	        sb.AppendLine();
			sb.AppendLine("}");
	        sb.AppendLine();

			return sb.ToString();
        }

        private string GenerateOvCInUitMelden(ControllerModel c)
        {
            var sb = new StringBuilder();

            var _hovin = CCOLGeneratorSettingsProvider.Default.GetElementName("hovin");
            var _hhdin = CCOLGeneratorSettingsProvider.Default.GetElementName("hhdin");
            var _hovuit = CCOLGeneratorSettingsProvider.Default.GetElementName("hovuit");
            var _hhduit = CCOLGeneratorSettingsProvider.Default.GetElementName("hhduit");
            var _schcprio = CCOLGeneratorSettingsProvider.Default.GetElementName("schcprio");
            var _prmlaatcrit = CCOLGeneratorSettingsProvider.Default.GetElementName("prmlaatcrit");
            var _prmallelijnen = CCOLGeneratorSettingsProvider.Default.GetElementName("prmallelijnen");
            var _tdhkarin = CCOLGeneratorSettingsProvider.Default.GetElementName("tdhkarin");
            var _tdhkaruit = CCOLGeneratorSettingsProvider.Default.GetElementName("tdhkaruit");
            var _tkarmelding = CCOLGeneratorSettingsProvider.Default.GetElementName("tkarmelding");
            var _tkarog = CCOLGeneratorSettingsProvider.Default.GetElementName("tkarog");
            var _cvc = CCOLGeneratorSettingsProvider.Default.GetElementName("cvc");
            
            var _hwissel = CCOLGeneratorSettingsProvider.Default.GetElementName("hwissel");
            var _schgeenwissel = CCOLGeneratorSettingsProvider.Default.GetElementName("schgeenwissel");
            var _schwisselpol = CCOLGeneratorSettingsProvider.Default.GetElementName("schwisselpol");

            sb.AppendLine("/*----------------------------------------------------------------");
            sb.AppendLine("   InUitMelden verzorgt het afhandelen van in- en uitmeldingen.");
            sb.AppendLine("   Voor het in- en uitmelden zijn hulpelementen gedefinieerd.");
            sb.AppendLine("   Bij het opkomen van het hulpelement voor de inmelding wordt");
            sb.AppendLine("   een inmelding gedaan voor OV-richting ov met prioriteitsniveau");
            sb.AppendLine("   iInstPrioriteitsNiveau[ov] en prioriteitsopties");
            sb.AppendLine("   iInstPrioriteitsOpties[ov]. Bij het opkomen van het hulpelement");
            sb.AppendLine("   voor de uitmelding wordt de eerste (oudste) inmelding uitgemeld.");
            sb.AppendLine("   De hulpelementen voor in- en uitmeldingen worden opgezet als");
            sb.AppendLine("   de bijbehorende in- resp. uitmeldlussen opstaan.");
            sb.AppendLine("   ---------------------------------------------------------------- */");
            sb.AppendLine("void InUitMelden(void)");
            sb.AppendLine("{");
            if (c.OVData.OVIngrepen.Count > 0)
            {
                AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCInUitMelden, true, false, false, true);

                sb.AppendLine($"{ts}/* OV-inmeldingen */");
                foreach (var ov in c.OVData.OVIngrepen)
                {
                    sb.AppendLine($"{ts}OVInmelden(ovFC{ov.FaseCyclus}, SH[{_hpf}{_hovin}{ov.FaseCyclus}], iInstPrioriteitsNiveau[ovFC{ov.FaseCyclus}], iInstPrioriteitsOpties[ovFC{ov.FaseCyclus}], 0, 0);");
                }
                sb.AppendLine();
                sb.AppendLine($"{ts}/* OV-uitmeldingen */");
                foreach (var ov in c.OVData.OVIngrepen)
                {
                    sb.AppendLine($"{ts}OVUitmelden(ovFC{ov.FaseCyclus}, SH[{_hpf}{_hovuit}{ov.FaseCyclus}]);");
                }
                sb.AppendLine();
            }
            if (c.OVData.HDIngrepen.Count > 0)
            {
                sb.AppendLine($"{ts}/* HD-inmeldingen */");
                foreach (var hd in c.OVData.HDIngrepen)
                {
                    sb.AppendLine($"{ts}OVInmelden(hdFC{hd.FaseCyclus}, SH[{_hpf}{_hhdin}{hd.FaseCyclus}], iInstPrioriteitsNiveau[hdFC{hd.FaseCyclus}], iInstPrioriteitsOpties[hdFC{hd.FaseCyclus}], 0, 0);");
                }
                sb.AppendLine();
                sb.AppendLine($"{ts}/* HD-uitmeldingen */");
                foreach (var hd in c.OVData.HDIngrepen)
                {
                    sb.AppendLine($"{ts}OVUitmelden(hdFC{hd.FaseCyclus}, SH[{_hpf}{_hhduit}{hd.FaseCyclus}]);");
                }
                sb.AppendLine();
            }
            
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}IH[{_hpf}{_hhdin}{hd.FaseCyclus}] = IH[{_hpf}{_hhduit}{hd.FaseCyclus}] = FALSE;");
            }
            
	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCInUitMelden, false, true, true, true);

            if (c.HasKAR())
            {
                sb.AppendLine();
                sb.AppendLine($"{ts}/* Bijhouden melding en ondergedrag KAR */");
                sb.AppendLine($"{ts}RT[{_tpf}{_tkarmelding}] = CIF_DSIWIJZ != 0;");
                sb.AppendLine($"{ts}RT[{_tpf}{_tkarog}] = T[{_tpf}{_tkarmelding}] || !startkarog;");
                sb.AppendLine($"{ts}if (!startkarog) startkarog = TRUE;");
            }

            #region HD ingrepen mee inmelden

            if (c.OVData.HDIngrepen.Any(x => x.MeerealiserendeFaseCycli.Count > 0))
            {
                sb.AppendLine($"{ts}/* Doorzetten HD inmeldingen */");
                foreach (var hd in c.OVData.HDIngrepen)
                {
                    if (hd.MeerealiserendeFaseCycli.Any())
                    {
                        foreach (var fc in hd.MeerealiserendeFaseCycli)
                        {
                            sb.AppendLine($"{ts}IH[{_hpf}{_hhdin}{fc.FaseCyclus}] |= IH[{_hpf}{_hhdin}{hd.FaseCyclus}]; IH[{_hpf}{_hhduit}{fc.FaseCyclus}] |= IH[{_hpf}{_hhduit}{hd.FaseCyclus}];");
                        }
                    }
                }
            }

            #endregion // HD ingrepen mee inmelden

            #region Noodaanvragen

            if (c.OVData.OVIngrepen.Any(x => x.NoodaanvraagKoplus && !string.IsNullOrWhiteSpace(x.Koplus) && x.Koplus != "NG"))
            {
                sb.AppendLine();
                sb.AppendLine($"{ts}/* Noodaanvragen obv koplus */");

                foreach (var ov in c.OVData.OVIngrepen.Where(x => x.NoodaanvraagKoplus && !string.IsNullOrWhiteSpace(x.Koplus) && x.Koplus != "NG"))
                {
                    sb.Append($"{ts}if (!C[{_cpf}{_cvc}{ov.FaseCyclus}] && DB[{_dpf}{ov.Koplus}] && R[{_fcpf}{ov.FaseCyclus}] && !TRG[{_fcpf}{ov.FaseCyclus}]");
                    if (ov.KoplusKijkNaarWisselstand && ov.HasOVIngreepWissel())
                    {
                        sb.Append($" && IH[{_hpf}{_hwissel}{ov.FaseCyclus}]");
                    }
                    sb.AppendLine($") A[{_fcpf}{ov.FaseCyclus}] |= BIT6;");
                }
            }

            #endregion // Noodaanvragen

            sb.AppendLine("}");
	        sb.AppendLine();

			return sb.ToString();
        }

        private string GenerateOvCPrioriteitsOpties(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* -------------------------");
            sb.AppendLine("   Prioriteitsopties voor OV");
            sb.AppendLine("   ------------------------- */");
            sb.AppendLine("void PrioriteitsOpties(void)");
            sb.AppendLine("{");
            
            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCPrioriteitsNiveau, true, false, false, true);
            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCPrioriteitsOpties, true, true, false, true);

            sb.AppendLine($"{ts}#ifdef OV_ADDFILE");
            sb.AppendLine($"{ts}{ts}PrioriteitsOpties_Add();");
            sb.AppendLine($"{ts}#endif");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCPrioriteitsNiveau, false, true, true, true);

            sb.AppendLine($"{ts}#ifdef OV_ADDFILE");
            sb.AppendLine($"{ts}{ts}PrioriteitsNiveau_Add();");
            sb.AppendLine($"{ts}#endif");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateOvCPrioriteitsToekenning(ControllerModel c)
        {
            var _hplhd = CCOLGeneratorSettingsProvider.Default.GetElementName("hplhd");
            var _schovpriople = CCOLGeneratorSettingsProvider.Default.GetElementName("schovpriople");
            var _homschtegenh = CCOLGeneratorSettingsProvider.Default.GetElementName("homschtegenh");

            var sb = new StringBuilder();
            sb.AppendLine("/* ------------------------------------");
            sb.AppendLine("   Prioriteitsopties toekenning voor OV");
            sb.AppendLine("   ------------------------------------ */");
            sb.AppendLine("void PrioriteitsToekenningExtra(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCPrioriteitsToekenning, true, true, false, false);
            
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateOvCTegenhoudenConflicten(ControllerModel c)
        {
            var _hplhd = CCOLGeneratorSettingsProvider.Default.GetElementName("hplhd");
            var _schovpriople = CCOLGeneratorSettingsProvider.Default.GetElementName("schovpriople");
            var _homschtegenh = CCOLGeneratorSettingsProvider.Default.GetElementName("homschtegenh");

            var sb = new StringBuilder();
            sb.AppendLine("/* ------------------------------------");
            sb.AppendLine("   Extra code tegenhouden conflicten OV");
            sb.AppendLine("   ------------------------------------ */");
            sb.AppendLine("void TegenhoudenConflictenExtra(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCTegenhoudenConflicten, true, true, false, false);

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateOvCPostAfhandelingOV(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* -------------------");
            sb.AppendLine("   Post afhandeling OV");
            sb.AppendLine("   ------------------- */");
            sb.AppendLine("void PostAfhandelingOV(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCPostAfhandelingOV, true, true, false, false);

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateOvCPARCorrecties(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* -------------------------------------");
            sb.AppendLine("   OVPARCorrecties corrigeert de PAR van");
            sb.AppendLine("   gesynchroniseerde richtingen.");
            sb.AppendLine("   ------------------------------------- */");
            sb.AppendLine("void OVPARCorrecties(void)");
            sb.AppendLine("{");

	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCPARCorrecties, true, true, false, false);

			sb.AppendLine("}");
            sb.AppendLine();
            return sb.ToString();
        }

        private string GenerateOvCPARCcol(ControllerModel c)
        {
            var sb = new StringBuilder();

            var _tgb = CCOLGeneratorSettingsProvider.Default.GetElementName("tgb");
            var _tgbhd = CCOLGeneratorSettingsProvider.Default.GetElementName("tgbhd");
            var _trt = CCOLGeneratorSettingsProvider.Default.GetElementName("trt");
            var _trthd = CCOLGeneratorSettingsProvider.Default.GetElementName("trthd");
            var _hovin = CCOLGeneratorSettingsProvider.Default.GetElementName("hovin");
            var _cvc = CCOLGeneratorSettingsProvider.Default.GetElementName("cvc");
            var _cvchd = CCOLGeneratorSettingsProvider.Default.GetElementName("cvchd");
            var _tblk = CCOLGeneratorSettingsProvider.Default.GetElementName("tblk");
            var _hov = CCOLGeneratorSettingsProvider.Default.GetElementName("hov");
            var _hhd = CCOLGeneratorSettingsProvider.Default.GetElementName("hhd");

            sb.AppendLine("/* -----------------------------------------------------");
            sb.AppendLine("   OVCcol zorgt voor het bijwerken van de CCOL-elementen");
            sb.AppendLine("   voor het OV.");
            sb.AppendLine("   ----------------------------------------------------- */");
            sb.AppendLine("void OVCcol(void) {");
            foreach(var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"  OVCcolElementen(ovFC{ov.FaseCyclus}, {_tpf}{_tgb}{ov.FaseCyclus}, {_tpf}{_trt}{ov.FaseCyclus}, {_hpf}{_hov}{ov.FaseCyclus}, {_cpf}{_cvc}{ov.FaseCyclus}, {_tpf}{_tblk}{ov.FaseCyclus});");
            }
            foreach(var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"  OVCcolElementen(hdFC{hd.FaseCyclus}, {_tpf}{_tgbhd}{hd.FaseCyclus}, {_tpf}{_trthd}{hd.FaseCyclus}, {_hpf}{_hhd}{hd.FaseCyclus}, {_cpf}{_cvchd}{hd.FaseCyclus}, -1);");
            }

	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCPARCcol, true, true, false, false);

			sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateOvCSpecialSignals(ControllerModel c)
        {
            var sb = new StringBuilder();

            var _prmtestdsivert = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtestdsivert");
            var _prmtestdsilyn = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtestdsilyn");
            var _prmtestdsicat = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtestdsicat");
            var _ddummykarin = CCOLGeneratorSettingsProvider.Default.GetElementName("ddummykarin");
            var _ddummykaruit = CCOLGeneratorSettingsProvider.Default.GetElementName("ddummykaruit");
            var _ddummykarhdin = CCOLGeneratorSettingsProvider.Default.GetElementName("ddummykarhdin");
            var _ddummykarhduit = CCOLGeneratorSettingsProvider.Default.GetElementName("ddummykarhduit");
            var _ddummyvecomin = CCOLGeneratorSettingsProvider.Default.GetElementName("ddummyvecomin");
            var _ddummyvecomuit = CCOLGeneratorSettingsProvider.Default.GetElementName("ddummyvecomuit");

            sb.AppendLine("/* ----------------------------------------------------------------");
            sb.AppendLine("   OVSpecialSignals wordt aangeroepen vanuit de functie ");
            sb.AppendLine("   is_special_signals. Deze wordt in de testomgeving gebruikt voor ");
            sb.AppendLine("   het opzetten van bijzondere ingangen.");
            sb.AppendLine("   ---------------------------------------------------------------- */");
            sb.AppendLine("#ifdef CCOL_IS_SPECIAL");
            sb.AppendLine("void OVSpecialSignals(void)");
            sb.AppendLine("{");

	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCSpecialSignals, false, true, false, true);

            sb.AppendLine($"{ts}/* reset oude set_DSI_message */");
            sb.AppendLine($"{ts}#if !defined VISSIM || defined SUMO");
            sb.AppendLine($"{ts}{ts}reset_DSI_message();");
            sb.AppendLine($"{ts}#endif");
            sb.AppendLine();

            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine($"{ts}#if !defined AUTOMAAT || defined _VRIWINTEST");
            }

            if (c.OVData.OVIngrepen.Any())
            {
                sb.AppendLine($"{ts}/* OV ingrepen */");
                foreach (var ov in c.OVData.OVIngrepen.Where(x => x.HasOVIngreepKAR()))
                {
	                if (int.TryParse(ov.FaseCyclus, out var ifc))
                    {
                        var m = ov.MeldingenData.Inmeldingen.FirstOrDefault(x => x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
                        var type = ov.Type == OVIngreepVoertuigTypeEnum.Bus ? "CIF_BUS" : "CIF_TRAM";
                        if (m != null)
                        {
                            sb.AppendLine($"{ts}if (SD[{_dpf}{ov.DummyKARInmelding.Naam}]) set_DSI_message(NG, {type}, {ifc}, CIF_DSIN, 1, PRM[{_prmpf}{_prmtestdsivert}] - 120, PRM[{_prmpf}{_prmtestdsilyn}], PRM[{_prmpf}{_prmtestdsicat}], 0);");
                        }
                        m = ov.MeldingenData.Uitmeldingen.FirstOrDefault(x => x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
                        if(m != null)
                        {
                            sb.AppendLine($"{ts}if (SD[{_dpf}{ov.DummyKARUitmelding.Naam}]) set_DSI_message(NG, {type}, {ifc}, CIF_DSUIT, 1, PRM[{_prmpf}{_prmtestdsivert}] - 120, PRM[{_prmpf}{_prmtestdsilyn}], PRM[{_prmpf}{_prmtestdsicat}], 0);");
                        }
                    }
                }
                var done = new List<string>();
                foreach (var ov in c.OVData.OVIngrepen.Where(x => x.HasOVIngreepVecom()))
                {
                    if (int.TryParse(ov.FaseCyclus, out var ifc))
                    {
                        var m = ov.MeldingenData.Inmeldingen.FirstOrDefault(x => x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector);
                        if (done.Any(x => x == m.RelatedInput1)) continue;
                        var type = ov.Type == OVIngreepVoertuigTypeEnum.Bus ? "CIF_BUS" : "CIF_TRAM";
                        if (m != null && !string.IsNullOrWhiteSpace(m.RelatedInput1))
                        {
                            sb.AppendLine($"{ts}if (SD[{_dpf}{m.RelatedInput1}]) set_DSI_message({(_dpf + m.RelatedInput1).ToUpper()}, {type}, {ifc}, CIF_DSIN, 1, PRM[{_prmpf}{_prmtestdsivert}] - 120, PRM[{_prmpf}{_prmtestdsilyn}], PRM[{_prmpf}{_prmtestdsicat}], NG);");
                        }
                        m = ov.MeldingenData.Uitmeldingen.FirstOrDefault(x => x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector);
                        if (m != null && !string.IsNullOrWhiteSpace(m.RelatedInput1))
                        {
                            sb.AppendLine($"{ts}if (SD[{_dpf}{m.RelatedInput1}]) set_DSI_message({(_dpf + m.RelatedInput1).ToUpper()}, {type}, {ifc}, CIF_DSUIT, 1, PRM[{_prmpf}{_prmtestdsivert}] - 120, PRM[{_prmpf}{_prmtestdsilyn}], PRM[{_prmpf}{_prmtestdsicat}], NG);");
                        }
                        if (m != null && !string.IsNullOrWhiteSpace(m.RelatedInput1))
                        {
                            done.Add(m.RelatedInput1);
                        }
                    }
                }
                sb.AppendLine();
            }

            if (c.OVData.HDIngrepen.Count > 0)
            {
                sb.AppendLine($"{ts}/* HD ingrepen */");
                foreach (var hd in c.OVData.HDIngrepen.Where(x => x.KAR))
                {
	                const string type = "CIF_POL";
	                if (int.TryParse(hd.FaseCyclus, out var ifc))
                    {
                        sb.AppendLine($"{ts}if (SD[{_dpf}{hd.DummyKARInmelding.Naam}]) set_DSI_message(0, {type}, {ifc}, CIF_DSIN, 1, 0, 0, 0, CIF_SIR);");
                        sb.AppendLine($"{ts}if (SD[{_dpf}{hd.DummyKARUitmelding.Naam}]) set_DSI_message(0, {type}, {ifc}, CIF_DSUIT, 1, 0, 0, 0, CIF_SIR);");
                    }
                }
            }

	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCSpecialSignals, false, true, false, false);

            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine($"{ts}#endif // !defined AUTOMAAT || defined _VRIWINTEST");
            }

            sb.AppendLine("}");
            sb.AppendLine("#endif");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateOvCBottom(ControllerModel c)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"#include \"{c.Data.Naam}ov.add\"");

            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine();
                sb.AppendLine("#ifdef _VRIWINTEST");
                sb.AppendLine("#include \"ov.c\"");
                sb.AppendLine("#endif");
            }

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCBottom, false, true, false, false);
			
			return sb.ToString();
        }
    }
}