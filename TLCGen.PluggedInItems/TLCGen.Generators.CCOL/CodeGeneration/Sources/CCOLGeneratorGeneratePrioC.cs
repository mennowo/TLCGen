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
        private string GeneratePrioC(ControllerModel controller)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* PRIORITEIT FUNCTIES APPLICATIE */");
            sb.AppendLine("/* ------------------------------ */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "prio.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();

            sb.Append(GeneratePrioCIncludes(controller));
            sb.Append(GeneratePrioCTop(controller));
            sb.Append(GeneratePrioCInit(controller));
            sb.Append(GeneratePrioCInstellingen(controller));
            sb.Append(GeneratePrioCRijTijdScenario(controller));
            sb.Append(GeneratePrioCInUitMelden(controller));
            sb.Append(GeneratePrioCOndermaximum(controller));
            sb.Append(GeneratePrioCAfkapgroen(controller));
            sb.Append(GeneratePrioCStartGroenMomenten(controller));
            sb.Append(GeneratePrioCAfkappen(controller));
            sb.Append(GeneratePrioCTerugkomGroen(controller));
            sb.Append(GeneratePrioCGroenVasthouden(controller));
            sb.Append(GeneratePrioCMeetkriterium(controller));
            sb.Append(GeneratePrioCPrioriteitsOpties(controller));
            sb.Append(GeneratePrioCPrioriteitsToekenning(controller));
            sb.Append(GeneratePrioCTegenhoudenConflicten(controller));
            sb.Append(GeneratePrioCPostAfhandelingPrio(controller));
            sb.Append(GeneratePrioCPARCorrecties(controller));
            sb.Append(GeneratePrioCPARCcol(controller));
            sb.Append(GeneratePrioCSpecialSignals(controller));
            sb.Append(GeneratePrioCBottom(controller));

            return sb.ToString();
        }

        private string GeneratePrioCIncludes(ControllerModel c)
        {
            var sb = new StringBuilder();

            if (c.InterSignaalGroep.Nalopen.Any())
            {
                sb.AppendLine("#define NALOPEN");
            }
            sb.AppendLine("#define PRIO_ADDFILE");
            sb.AppendLine();

            sb.AppendLine("/*include files */");
            sb.AppendLine("/*------------- */");
            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine("#ifndef PRACTICE_TEST");
            }
            sb.AppendLine($"{ts}#include \"{c.Data.Naam}sys.h\"");
            sb.AppendLine($"{ts}#include \"stdfunc.h\"  /* standaard functies                */");
            sb.AppendLine($"{ts}#include \"prio.h\"     /* prio header                       */");
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
            var ris = c.RISData.RISToepassen && 
                      (c.PrioData.PrioIngrepen
                          .Any(x => 
                              x.MeldingenData.Inmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde) ||
                              x.MeldingenData.Uitmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)) ||
                       c.PrioData.HDIngrepen.Any(x => x.RIS));
            if (ris)
            {
                sb.AppendLine($"{ts}#ifndef NO_RIS");
                sb.AppendLine($"{ts}{ts}#include \"risappl.h\"   /* RIS routines                     */");
                sb.AppendLine($"{ts}{ts}#if (CCOL_V > 100)");
                sb.AppendLine($"{ts}{ts}#include \"extra_func_ris.h\"   /* RIS routines              */");
                sb.AppendLine($"{ts}{ts}#endif");
                sb.AppendLine($"{ts}#endif /* NO_RIS */");
            }
            sb.AppendLine($"{ts}#include \"cif.inc\"    /* interface                         */");
            sb.AppendLine($"{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)");
            sb.AppendLine($"{ts}{ts}#include \"xyprintf.h\" /* Printen debuginfo                 */");
            if (ris)
            {
                sb.AppendLine($"{ts}#include \"rissimvar.h\"   /* RIS routines                   */");
            }
            sb.AppendLine($"{ts}{ts}extern {c.GetBoolV()} display;");
            sb.AppendLine($"{ts}#endif");
            sb.AppendLine($"{ts}#include \"ccolfunc.h\"");
            sb.AppendLine($"{ts}#include \"ccol_mon.h\"");
            sb.AppendLine($"{ts}#include \"extra_func.h\"");
            if (c.PrioData.PrioIngrepen.Any(x => x.CheckWagenNummer))
            {
                sb.AppendLine($"{ts}#define PRIO_CHECK_WAGENNMR /* check op wagendienstnummer          */");
            }
            sb.AppendLine($"{ts}#include \"extra_func_prio.h\"");
            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine("#endif /* PRACTICE_TEST */");
            }

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCIncludes, true, true, true, true);

            return sb.ToString();
        }

        private string GeneratePrioCTop(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#define MAX_AANTAL_INMELDINGEN           10");
            sb.AppendLine("#define DEFAULT_MAX_WACHTTIJD           120");
            sb.AppendLine("#define NO_REALISEREN_TOEGESTAAN");

            if (c.HalfstarData.IsHalfstar)
            {
                sb.AppendLine();
                sb.AppendLine("/* Declareren OV settings functie halfstar */");
            }

            sb.AppendLine();
            sb.AppendLine("extern mulv DB_old[];");
            if (c.Data.CCOLVersie < CCOLVersieEnum.CCOL120)
            {
                sb.AppendLine("extern mulv TDH_old[];");
            }

            sb.AppendLine();
            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine("#ifndef PRACTICE_TEST");
                sb.AppendLine("#include \"prio.c\"");
                sb.AppendLine("#else");
                sb.AppendLine("#include \"prio.h\"");
                sb.AppendLine("const code *iFC_PRIO_code[prioFCMAX];");
                sb.AppendLine("#endif");
            }
            else
            {
                sb.AppendLine("#include \"prio.c\"");
            }
            if (c.HalfstarData.IsHalfstar && c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen && c.HasPTorHD())
            {
                sb.AppendLine("#include \"halfstar_prio.h\"");
            }
            sb.AppendLine();
            if (c.HasKAR())
            {
                sb.AppendLine("/* Variabele tbv start KAR ondergedrag timer bij starten regeling */");
                sb.AppendLine("static char startkarog = FALSE;");
            }

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCTop, true, true, true, true);
            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCBottom, true, false, false, true);

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
            sb.AppendLine("   - iFC_PRIOix[prio]                    : (de index van) de fasecyclus van de PRIO-richting prio.");
            sb.AppendLine("   - iT_GBix[prio]                       : (de index van) de timer voor de groenbewaking");
            sb.AppendLine("                                           van PRIO-richting prio.");
            sb.AppendLine("   - iH_PRIOix[prio]                     : (de index van) het hulpelement voor de");
            sb.AppendLine("                                           prioriteitstoekenning van PRIO-richting prio.");
            sb.AppendLine("   - iInstPrioriteitsNiveau[prio]        : het prioriteitsniveau van de inmeldingen van de");
            sb.AppendLine("                                           PRIO-richting prio.");
            sb.AppendLine("   - iInstPrioriteitsOpties[prio]        : de prioriteisopties van de inmeldingen van de");
            sb.AppendLine("                                           PRIO-richting prio.");
            sb.AppendLine("   - iGroenBewakingsTijd[prio]           : De groenbewakingstijd van de PRIO-richting prio.");
            sb.AppendLine("   - iRTSOngehinderd[prio]               : De ongehinderde rijtijd van PRIO-richting prio.");
            sb.AppendLine("   - iRTSBeperktGehinderd[prio]          : De beperkt gehinderde rijtijd van PRIO-richting prio.");
            sb.AppendLine("   - iRTSGehinderd[prio]                 : De gehinderde rijtijd van PRIO-richting prio.");
            sb.AppendLine("   - iOnderMaximum[prio]                 : Het ondermaximum van PRIO-richting prio.");
            sb.AppendLine("   - iSelDetFoutNaGB[prio]               : De in- en uitmelddetectie van PRIO-richting prio");
            sb.AppendLine("                                           wordt als defect beschouwd bij het aanspreken");
            sb.AppendLine("                                           van de groenbewaking. De defectmelding wordt");
            sb.AppendLine("                                           opgeheven bij de eerstvolgende uitmelding.");
            sb.AppendLine("   - iMaximumWachtTijd[fc]               : de maximum wachttijd voor richting fc waarbprioen");
            sb.AppendLine("                                           geen prioriteit meer wordt toegekend voor");
            sb.AppendLine("                                           niet-nooddienst-inmeldingen op de PRIO-richtingen.");
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
            sb.AppendLine("                                           niet-konflikterende PRIO-ingre(e)p(en).");
            sb.AppendLine("   - iSCH_ALTG[fc]                       : richting fc mag alternatief realiseren tijdens");
            sb.AppendLine("                                           niet-konflikterende PRIO-ingre(e)p(en).");
            sb.AppendLine("   - iInstAfkapGroenAlt[fc]              : groentijd die richting fc minimaal mag maken");
            sb.AppendLine("                                           bij een alternatieve realisatie, voordat deze");
            sb.AppendLine("                                           mag worden afgekapt.");
            sb.AppendLine("   - iLangstWachtendeAlternatief         : de regeling maakt gebruik van");
            sb.AppendLine("                                           langstwachtende alternatief");
            sb.AppendLine("   ------------------------------------------------------------------------------------------- */");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GeneratePrioCInit(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine("void PrioInitExtra(void) ");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCInitPrio, true, true, true, true);

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GeneratePrioCInstellingen(ControllerModel c)
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
            var _hprio = CCOLGeneratorSettingsProvider.Default.GetElementName("hprio");
            var _hhd = CCOLGeneratorSettingsProvider.Default.GetElementName("hhd");
            var _prmprio = CCOLGeneratorSettingsProvider.Default.GetElementName("prmprio");
            var _prmpriohd = CCOLGeneratorSettingsProvider.Default.GetElementName("prmpriohd");
            var _prmomx = CCOLGeneratorSettingsProvider.Default.GetElementName("prmomx");
            var _tblk = CCOLGeneratorSettingsProvider.Default.GetElementName("tblk");
            var _prmupinagb = CCOLGeneratorSettingsProvider.Default.GetElementName("prmupinagb");
            var _prmupinagbhd = CCOLGeneratorSettingsProvider.Default.GetElementName("prmupinagbhd");
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
            var _schaltghst = CCOLGeneratorSettingsProvider.Default.GetElementName("schaltghst");
            var _prmohpmg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmohpmg");
            var _hmlact = CCOLGeneratorSettingsProvider.Default.GetElementName("hmlact");
            var _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");

            sb.AppendLine("void PrioInstellingen(void) ");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}/* ======================= */");
            sb.AppendLine($"{ts}/* Instellingen PRIORITEIT */");
            sb.AppendLine($"{ts}/* ======================= */");
            sb.AppendLine();

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCInstellingen, true, false, true, true);

            if (c.HasPTorHD())
            {
                sb.AppendLine($"{ts}/* Fasecyclus voor richtingen met PRIO */");
                foreach (var prio in c.PrioData.PrioIngrepen)
                {
                    sb.AppendLine($"{ts}iFC_PRIOix[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = {_fcpf}{prio.FaseCyclus};");
                }
                foreach (var hd in c.PrioData.HDIngrepen)
                {
                    sb.AppendLine($"{ts}iFC_PRIOix[hdFC{hd.FaseCyclus}] = {_fcpf}{hd.FaseCyclus};");
                }
                sb.AppendLine();

                if (c.Data.PracticeOmgeving)
                {
                    sb.AppendLine($"{ts}/* Code voor richtingen met PRIO */");
                    sb.AppendLine($"{ts}#ifdef PRACTICE_TEST");
                    foreach (var prio in c.PrioData.PrioIngrepen)
                    {
                        sb.AppendLine($"{ts}{ts}iFC_PRIO_code[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = \"prio{CCOLCodeHelper.GetPriorityName(c, prio)}\";");
                    }
                    foreach (var hd in c.PrioData.HDIngrepen)
                    {
                        sb.AppendLine($"{ts}{ts}iFC_PRIO_code[hdFC{hd.FaseCyclus}] = \"hd{hd.FaseCyclus}\";");
                    }
                    sb.AppendLine($"{ts}#endif /* PRACTICE_TEST */");
                    sb.AppendLine();
                }
            }

            sb.AppendLine($"{ts}/* Index van de groenbewakingstimer */");
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                sb.AppendLine($"{ts}iT_GBix[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = {_tpf}{_tgb}{CCOLCodeHelper.GetPriorityName(c, prio)};");
            }
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iT_GBix[hdFC{hd.FaseCyclus}] = {_tpf}{_tgbhd}{hd.FaseCyclus};");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Index van het hulpelement voor de ingreep */");
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                sb.AppendLine($"{ts}iH_PRIOix[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = {_hpf}{_hprio}{CCOLCodeHelper.GetPriorityName(c, prio)};");
            }
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iH_PRIOix[hdFC{hd.FaseCyclus}] = {_hpf}{_hhd}{hd.FaseCyclus};");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Prioriteitsniveau */");
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                sb.AppendLine($"{ts}iInstPrioriteitsNiveau[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = PRM[{_prmpf}{_prmprio}{CCOLCodeHelper.GetPriorityName(c, prio)}]/1000L;");
            }
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iInstPrioriteitsNiveau[hdFC{hd.FaseCyclus}] = PRM[{_prmpf}{_prmpriohd}{hd.FaseCyclus}]/1000L;");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Prioriteitsopties */");
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                sb.AppendLine($"{ts}iInstPrioriteitsOpties[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = BepaalPrioriteitsOpties({_prmpf}{_prmprio}{CCOLCodeHelper.GetPriorityName(c, prio)});");
            }
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iInstPrioriteitsOpties[hdFC{hd.FaseCyclus}] = BepaalPrioriteitsOpties({_prmpf}{_prmpriohd}{hd.FaseCyclus});");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Groenbewakingstijd */");
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                sb.AppendLine($"{ts}iGroenBewakingsTijd[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = T_max[{_tpf}{_tgb}{CCOLCodeHelper.GetPriorityName(c, prio)}];");
            }
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iGroenBewakingsTijd[hdFC{hd.FaseCyclus}] = T_max[{_tpf}{_tgbhd}{hd.FaseCyclus}];");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Ongehinderde rijtijd */");
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                sb.AppendLine($"{ts}iRTSOngehinderd[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = PRM[{_prmpf}{_prmrto}{CCOLCodeHelper.GetPriorityName(c, prio)}];");
            }
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iRTSOngehinderd[hdFC{hd.FaseCyclus}] = PRM[{_prmpf}{_prmrtohd}{hd.FaseCyclus}];");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Beperkt gehinderde rijtijd */");
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                sb.AppendLine($"{ts}iRTSBeperktGehinderd[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = PRM[{_prmpf}{_prmrtbg}{CCOLCodeHelper.GetPriorityName(c, prio)}];");
            }
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iRTSBeperktGehinderd[hdFC{hd.FaseCyclus}] = PRM[{_prmpf}{_prmrtbghd}{hd.FaseCyclus}];");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Gehinderde rijtijd */");
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                sb.AppendLine($"{ts}iRTSGehinderd[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = PRM[{_prmpf}{_prmrtg}{CCOLCodeHelper.GetPriorityName(c, prio)}];");
            }
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iRTSGehinderd[hdFC{hd.FaseCyclus}] = PRM[{_prmpf}{_prmrtghd}{hd.FaseCyclus}];");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Ondermaximum */");
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                sb.AppendLine($"{ts}iOnderMaximum[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = PRM[{_prmpf}{_prmomx}{CCOLCodeHelper.GetPriorityName(c, prio)}];");
            }
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iOnderMaximum[hdFC{hd.FaseCyclus}] = 0;");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Blokkeringstijd */");
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                sb.AppendLine($"{ts}iBlokkeringsTijd[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = T_max[{_tpf}{_tblk}{CCOLCodeHelper.GetPriorityName(c, prio)}];");
            }
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iBlokkeringsTijd[hdFC{hd.FaseCyclus}] = 0;");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Na aanspreken groenbewaking wordt de selectieve ");
            sb.AppendLine("	   detectie niet langer betrouwbaar gevonden */");
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                sb.AppendLine($"{ts}iSelDetFoutNaGB[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = PRM[{_prmpf}{_prmupinagb}{CCOLCodeHelper.GetPriorityName(c, prio)}];");
            }
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iSelDetFoutNaGB[hdFC{hd.FaseCyclus}] = PRM[{_prmpf}{_prmupinagbhd}{hd.FaseCyclus}];");
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
            foreach (var priofc in c.PrioData.PrioIngreepSignaalGroepParameters)
            {
                if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, priofc.FaseCyclus))
                {
                    continue;
                }

                var fct = c.Fasen.First(x => x.Naam == priofc.FaseCyclus).Type;
                switch (fct)
                {
                    case FaseTypeEnum.OV:
                    case FaseTypeEnum.Auto:
                    case FaseTypeEnum.Fiets:
                    case FaseTypeEnum.Voetganger:
                        if (!c.PrioData.PrioIngreepSignaalGroepParametersHard)
                        {
                            sb.AppendLine($"{ts}iInstPercMaxGroenTijdTerugKomen[{_fcpf}{priofc.FaseCyclus}] = PRM[{_prmpf}{_prmpmgt}{priofc.FaseCyclus}];");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}iInstPercMaxGroenTijdTerugKomen[{_fcpf}{priofc.FaseCyclus}] = {priofc.PercMaxGroentijdVoorTerugkomen};");
                        }
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* De minimale groentijd die een richting krijgt als");
            sb.AppendLine($"{ts}   deze mag terugkomen. */");
            foreach (var priofc in c.PrioData.PrioIngreepSignaalGroepParameters)
            {
                if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, priofc.FaseCyclus))
                {
                    continue;
                }

                var fct = c.Fasen.First(x => x.Naam == priofc.FaseCyclus).Type;
                switch (fct)
                {
                    case FaseTypeEnum.OV:
                    case FaseTypeEnum.Auto:
                    case FaseTypeEnum.Fiets:
                    case FaseTypeEnum.Voetganger:
                        if (!c.PrioData.PrioIngreepSignaalGroepParametersHard)
                        {
                            sb.AppendLine($"{ts}iInstMinTerugKomGroenTijd[{_fcpf}{priofc.FaseCyclus}] = PRM[{_prmpf}{_prmognt}{priofc.FaseCyclus}];");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}iInstMinTerugKomGroenTijd[{_fcpf}{priofc.FaseCyclus}] = {priofc.OndergrensNaTerugkomen};");
                        }
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Aantal malen niet afkappen */");
            foreach (var priofc in c.PrioData.PrioIngreepSignaalGroepParameters)
            {
                if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, priofc.FaseCyclus))
                {
                    continue;
                }

                var fct = c.Fasen.First(x => x.Naam == priofc.FaseCyclus).Type;
                switch (fct)
                {
                    case FaseTypeEnum.Voetganger:
                        continue;
                    case FaseTypeEnum.OV:
                    case FaseTypeEnum.Auto:
                    case FaseTypeEnum.Fiets:
                        if (!c.PrioData.PrioIngreepSignaalGroepParametersHard)
                        {
                            sb.AppendLine($"{ts}iInstAantalMalenNietAfkappen[{_fcpf}{priofc.FaseCyclus}] = PRM[{_prmpf}{_prmnofm}{priofc.FaseCyclus}];");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}iInstAantalMalenNietAfkappen[{_fcpf}{priofc.FaseCyclus}] = {priofc.AantalKerenNietAfkappen};");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Onder deze groentijd mag een richting niet worden");
            sb.AppendLine($"{ts}   afgekapt, tenzij zich een nooddienst heeft ingemeld. */");
            foreach (var priofc in c.PrioData.PrioIngreepSignaalGroepParameters)
            {
                if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, priofc.FaseCyclus))
                {
                    continue;
                }

                var fct = c.Fasen.First(x => x.Naam == priofc.FaseCyclus).Type;
                switch (fct)
                {
                    case FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iAfkapGroenTijd[{_fcpf}{priofc.FaseCyclus}] = 0;");
                        continue;
                    case FaseTypeEnum.OV:
                    case FaseTypeEnum.Auto:
                    case FaseTypeEnum.Fiets:
                        if (!c.PrioData.PrioIngreepSignaalGroepParametersHard)
                        {
                            sb.AppendLine($"{ts}iAfkapGroenTijd[{_fcpf}{priofc.FaseCyclus}] = PRM[{_prmpf}{_prmmgcov}{priofc.FaseCyclus}];");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}iAfkapGroenTijd[{_fcpf}{priofc.FaseCyclus}] = {priofc.MinimumGroentijdConflictOVRealisatie};");
                        }
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Als een richting minder groen heeft gehad dan");
            sb.AppendLine($"{ts}   dit percentage van de maximum groentijd, dan");
            sb.AppendLine($"{ts}   mag de richting niet worden afgekapt, tenzij");
            sb.AppendLine($"{ts}   zich een nooddienst heeft ingemeld. */");
            foreach (var priofc in c.PrioData.PrioIngreepSignaalGroepParameters)
            {
                if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, priofc.FaseCyclus))
                {
                    continue;
                }

                var fct = c.Fasen.First(x => x.Naam == priofc.FaseCyclus).Type;
                switch (fct)
                {
                    case FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iPercGroenTijd[{_fcpf}{priofc.FaseCyclus}] = 100; /* Voetgangers mogen niet worden afgekapt. */");
                        continue;
                    case FaseTypeEnum.OV:
                    case FaseTypeEnum.Auto:
                    case FaseTypeEnum.Fiets:
                        if (!c.PrioData.PrioIngreepSignaalGroepParametersHard)
                        {
                            sb.AppendLine($"{ts}iPercGroenTijd[{_fcpf}{priofc.FaseCyclus}] = PRM[{_prmpf}{_prmpmgcov}{priofc.FaseCyclus}];");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}iPercGroenTijd[{_fcpf}{priofc.FaseCyclus}] = {priofc.PercMaxGroentijdConflictOVRealisatie};");
                        }
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Na te zijn afgekapt, wordt het percentage");
            sb.AppendLine($"{ts}   van de maximumgroentijd verhoogd met dit ophoogpercentage. */");
            foreach (var priofc in c.PrioData.PrioIngreepSignaalGroepParameters)
            {
                if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, priofc.FaseCyclus))
                {
                    continue;
                }
                var fct = c.Fasen.First(x => x.Naam == priofc.FaseCyclus).Type;

                switch (fct)
                {
                    case FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iInstOphoogPercentageMG[{_fcpf}{priofc.FaseCyclus}] = 0;");
                        continue;
                    case FaseTypeEnum.OV:
                    case FaseTypeEnum.Auto:
                    case FaseTypeEnum.Fiets:
                        if (!c.PrioData.PrioIngreepSignaalGroepParametersHard)
                        {
                            sb.AppendLine($"{ts}iInstOphoogPercentageMG[{_fcpf}{priofc.FaseCyclus}] = PRM[{_prmpf}{_prmohpmg}{priofc.FaseCyclus}];");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}iInstOphoogPercentageMG[{_fcpf}{priofc.FaseCyclus}] = {priofc.OphoogpercentageNaAfkappen};");
                        }
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

                sb.AppendLine($"{ts}/* Richting mag alternatief realiseren tijdens een OV ingreep */");
                var tts = ts;
                if (c.HalfstarData.IsHalfstar)
                {
                    tts += ts;
                    sb.AppendLine($"{ts}if (IH[{_hpf}{_hmlact}])");
                    sb.AppendLine($"{ts}{{");
                }
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
                        sb.Append($"{tts}iSCH_ALTG[{_fcpf}{fc.FaseCyclus}] = SCH[{_schpf}{_schaltg}");
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
                if (c.HalfstarData.IsHalfstar)
                {
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}else");
                    sb.AppendLine($"{ts}{{");
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
                            sb.Append($"{tts}iSCH_ALTG[{_fcpf}{fc.FaseCyclus}] = SCH[{_schpf}{_schaltghst}");
                            foreach (var ofc in hasgs.Item2)
                            {
                                sb.Append(ofc);
                            }
                            sb.AppendLine("];");
                        }
                        else
                        {
                            sb.AppendLine($"{tts}iSCH_ALTG[{_fcpf}{fc.FaseCyclus}] = SCH[{_schpf}{_schaltghst}{fc.FaseCyclus}];");
                        }
                    }
                    sb.AppendLine($"{ts}}}");
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

                if (c.HasHD())
                {
                    sb.AppendLine($"{ts}/* definitie van de meerealisaties voor de hulpdiensten */");
                    foreach (var hd in c.PrioData.HDIngrepen)
                    {
                        var i = 0;
                        foreach (var mr in hd.MeerealiserendeFaseCycli)
                        {
                            sb.AppendLine($"{ts}iPrioMeeRealisatie[{_fcpf}{hd.FaseCyclus}][{i}] = {_fcpf}{mr.FaseCyclus};");
                            ++i;
                        }
                    }
                    sb.AppendLine();
                }

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

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCInstellingen, false, true, false, false);

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GeneratePrioCRijTijdScenario(ControllerModel c)
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
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                foreach (var fc in c.Fasen)
                {
                    if (prio.FaseCyclus == fc.Naam)
                    {
                        var wissel = false;
                        if (prio.KoplusKijkNaarWisselstand && prio.HasOVIngreepWissel())
                        {
                            wissel = true;
                            sb.Append($"{ts}if (IH[{_hpf}{_hwissel}{CCOLCodeHelper.GetPriorityName(c, prio)}])");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}PrioRijTijdScenario(prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}, {_dpf}{prio.Koplus}, NG, NG);");
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine($"{ts}else");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}PrioRijTijdScenario(prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}, NG, NG, NG);");
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
                                        sb.Append($"{tts}PrioRijTijdScenario(prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}, {_dpf}{d.Naam}, ");
                                        if (i < ll.Count)
                                        {
                                            sb.AppendLine($"{_dpf}{ll[i].Naam}, {_tpf}{_tbtovg}{CCOLCodeHelper.GetPriorityName(c, prio)});");
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
                                        sb.Append($"{tts}PrioRijTijdScenario(prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}, ");
                                        if (i < kl.Count)
                                        {
                                            sb.AppendLine($"{_dpf}{kl[i].Naam}, {_dpf}{d.Naam}, {_tpf}{_tbtovg}{CCOLCodeHelper.GetPriorityName(c, prio)});");
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
                                sb.Append($"{tts}PrioRijTijdScenario(prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}, NG, NG, NG);");
                            }
                            sb.AppendLine();
                        }

                        break;
                    }
                }
            }
            foreach (var hd in c.PrioData.HDIngrepen)
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
                                    sb.Append($"{ts}PrioRijTijdScenario(hdFC{fc.Naam}, {_dpf}{d.Naam}, ");
                                    if (i < ll.Count)
                                    {
                                        sb.AppendLine($"{_dpf}{ll[i].Naam}, {_tpf}{_tbtovg}{fc.Naam}hd);");
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
                                    sb.Append($"{ts}PrioRijTijdScenario(hdFC{fc.Naam}, ");
                                    if (i < kl.Count)
                                    {
                                        sb.AppendLine($"{_dpf}{kl[i].Naam}, {_dpf}{d.Naam}, {_tpf}{_tbtovg}{fc.Naam}hd);");
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
                            sb.Append($"{ts}PrioRijTijdScenario(hdFC{fc.Naam}, NG, NG, NG);");
                        }
                        break;
                    }
                }
            }
            sb.AppendLine();
            foreach (var prio in c.PrioData.PrioIngrepen.Where(x => x.VersneldeInmeldingKoplus != NooitAltijdAanUitEnum.Nooit && !string.IsNullOrWhiteSpace(x.Koplus) && x.Koplus != "NG"))
            {
                sb.Append(prio.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.Altijd
                        ? $"{ts}if (DB[{_dpf}{prio.Koplus}] && C[{_cpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, prio)}] && (iRijTimer[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] < iRijTijd[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}])"
                        : $"{ts}if (SCH[{_schpf}{_schvi}{CCOLCodeHelper.GetPriorityName(c, prio)}] && DB[{_dpf}{prio.Koplus}] && C[{_cpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, prio)}] && (iRijTimer[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] < iRijTijd[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}])");
                if (prio.KoplusKijkNaarWisselstand && prio.HasOVIngreepWissel())
                {
                    sb.Append($" && IH[{_hpf}{_hwissel}{CCOLCodeHelper.GetPriorityName(c, prio)}]");
                }
                sb.AppendLine($") iRijTijd[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] = 0;");
            }

            if (OrderedPieceGenerators[CCOLCodeTypeEnum.PrioCRijTijdScenario].Any())
            {
                sb.AppendLine();
                foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.PrioCRijTijdScenario])
                {
                    sb.Append(gen.Value.GetCode(c, CCOLCodeTypeEnum.PrioCRijTijdScenario, ts, gen.Key));
                }
            }
            sb.AppendLine();
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GeneratePrioCInUitMelden(ControllerModel c)
        {
            var sb = new StringBuilder();

            var _hprioin = CCOLGeneratorSettingsProvider.Default.GetElementName("hprioin");
            var _hhdin = CCOLGeneratorSettingsProvider.Default.GetElementName("hhdin");
            var _hhd = CCOLGeneratorSettingsProvider.Default.GetElementName("hhd");
            var _hpriouit = CCOLGeneratorSettingsProvider.Default.GetElementName("hpriouit");
            var _hhduit = CCOLGeneratorSettingsProvider.Default.GetElementName("hhduit");
            var _tkarmelding = CCOLGeneratorSettingsProvider.Default.GetElementName("tkarmelding");
            var _tkarog = CCOLGeneratorSettingsProvider.Default.GetElementName("tkarog");
            var _cvc = CCOLGeneratorSettingsProvider.Default.GetElementName("cvc");
            var _tovminrood = CCOLGeneratorSettingsProvider.Default.GetElementName("tovminrood");
            var _hwissel = CCOLGeneratorSettingsProvider.Default.GetElementName("hwissel");

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
            
            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine("#if defined PRACTICE_TEST && defined CCOL_IS_SPECIAL");
                sb.AppendLine($"{ts}is_special_signals();");
                sb.AppendLine("#endif");
            }

            if (c.PrioData.PrioIngrepen.Count > 0)
            {
                AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCInUitMelden, true, false, false, true);

                sb.AppendLine($"{ts}/* Prioriteit-inmeldingen */");
                foreach (var prio in c.PrioData.PrioIngrepen)
                {
                    sb.AppendLine($"{ts}PrioInmelden(prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}, SH[{_hpf}{_hprioin}{CCOLCodeHelper.GetPriorityName(c, prio)}], iInstPrioriteitsNiveau[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}], iInstPrioriteitsOpties[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}], 0, 0);");
                }
                sb.AppendLine();
                sb.AppendLine($"{ts}/* Prioriteit-uitmeldingen */");
                foreach (var prio in c.PrioData.PrioIngrepen)
                {
                    sb.AppendLine($"{ts}PrioUitmelden(prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}, SH[{_hpf}{_hpriouit}{CCOLCodeHelper.GetPriorityName(c, prio)}]);");
                }
                sb.AppendLine();
            }
            if (c.PrioData.HDIngrepen.Count > 0)
            {
                sb.AppendLine($"{ts}/* HD-inmeldingen */");
                foreach (var hd in c.PrioData.HDIngrepen)
                {
                    sb.AppendLine($"{ts}PrioInmelden(hdFC{hd.FaseCyclus}, SH[{_hpf}{_hhdin}{hd.FaseCyclus}], iInstPrioriteitsNiveau[hdFC{hd.FaseCyclus}], iInstPrioriteitsOpties[hdFC{hd.FaseCyclus}], 0, 0);");
                }
                sb.AppendLine();
                sb.AppendLine($"{ts}/* HD-uitmeldingen */");
                foreach (var hd in c.PrioData.HDIngrepen)
                {
                    sb.AppendLine($"{ts}PrioUitmelden(hdFC{hd.FaseCyclus}, SH[{_hpf}{_hhduit}{hd.FaseCyclus}]);");
                }
                sb.AppendLine();
            }
            
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}IH[{_hpf}{_hhdin}{hd.FaseCyclus}] = IH[{_hpf}{_hhduit}{hd.FaseCyclus}] = FALSE;");
            }

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCInUitMelden, false, true, true, true);

            if (c.PrioData.HDIngrepen.Count > 0)
            {
                sb.AppendLine($"{ts}/* herstarten FB_timer bij in- of uitmelding HD of einde ingreep (door groenbewaking) */");
                sb.AppendLine($"{ts}RTFB &= ~PRIO_RTFB_BIT;");
                sb.Append($"{ts}if (");
                var tss = $"{ts}    ";
                var first = true;
                foreach (var hd in c.PrioData.HDIngrepen)
                {
                    if (!first)
                    {
                        sb.AppendLine(" ||");
                        sb.Append(tss);
                    }
                    first = false;
                    sb.Append($"IH[{_hpf}{_hhdin}{hd.FaseCyclus}]|| IH[{_hpf}{_hhduit}{hd.FaseCyclus}] || EH[{_hpf}{_hhd}{hd.FaseCyclus}]");
                }
                sb.AppendLine(")");
                sb.AppendLine($"{ts}{{");
                sb.AppendLine($"{ts}{ts}RTFB |= PRIO_RTFB_BIT;");
                sb.AppendLine($"{ts}}}");
            }

            if (c.HasKAR())
            {
                sb.AppendLine();
                sb.AppendLine($"{ts}/* Bijhouden melding en ondergedrag KAR */");
                sb.AppendLine($"{ts}RT[{_tpf}{_tkarmelding}] = CIF_DSIWIJZ != 0 && CIF_DSI[CIF_DSI_LUS] == 0;");
                sb.AppendLine($"{ts}RT[{_tpf}{_tkarog}] = T[{_tpf}{_tkarmelding}] || !startkarog;");
                sb.AppendLine($"{ts}if (!startkarog) startkarog = TRUE;");
            }

            #region HD ingrepen mee inmelden

            if (c.PrioData.HDIngrepen.Any(x => x.MeerealiserendeFaseCycli.Count > 0))
            {
                sb.AppendLine($"{ts}/* Doorzetten HD inmeldingen */");
                foreach (var hd in c.PrioData.HDIngrepen)
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

            if (c.PrioData.PrioIngrepen.Any(x => x.NoodaanvraagKoplus && !string.IsNullOrWhiteSpace(x.Koplus) && x.Koplus != "NG"))
            {
                sb.AppendLine();
                sb.AppendLine($"{ts}/* Noodaanvragen obv koplus */");

                foreach (var prio in c.PrioData.PrioIngrepen.Where(x => x.NoodaanvraagKoplus && !string.IsNullOrWhiteSpace(x.Koplus) && x.Koplus != "NG"))
                {
                    sb.Append($"{ts}if (!C[{_cpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, prio)}] && DB[{_dpf}{prio.Koplus}] && R[{_fcpf}{prio.FaseCyclus}] && !TRG[{_fcpf}{prio.FaseCyclus}] && !T[{_tpf}{_tovminrood}{CCOLCodeHelper.GetPriorityName(c, prio)}]");
                    if (prio.KoplusKijkNaarWisselstand && prio.HasOVIngreepWissel())
                    {
                        sb.Append($" && IH[{_hpf}{_hwissel}{CCOLCodeHelper.GetPriorityName(c, prio)}]");
                    }
                    sb.AppendLine($") A[{_fcpf}{prio.FaseCyclus}] |= BIT6;");
                }
            }

            #endregion // Noodaanvragen
            
            #region PRACTICE
            
            sb.AppendLine("#if defined CCOL_IS_SPECIAL && defined PRACTICE_TEST");
            sb.AppendLine($"{ts}is_special_signals();");
            sb.AppendLine("#endif");
            
            #endregion // PRACTICE

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GeneratePrioCPrioriteitsOpties(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* ------------------------------------");
            sb.AppendLine("   Prioriteitsopties voor PRIO ingrepen");
            sb.AppendLine("   ------------------------------------ */");
            sb.AppendLine("void PrioriteitsOpties(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCPrioriteitsNiveau, true, false, false, true);
            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCPrioriteitsOpties, true, true, false, true);

            sb.AppendLine($"{ts}#ifdef PRIO_ADDFILE");
            sb.AppendLine($"{ts}{ts}PrioriteitsOpties_Add();");
            sb.AppendLine($"{ts}#endif");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCPrioriteitsNiveau, false, true, true, true);

            sb.AppendLine($"{ts}#ifdef PRIO_ADDFILE");
            sb.AppendLine($"{ts}{ts}PrioriteitsNiveau_Add();");
            sb.AppendLine($"{ts}#endif");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GeneratePrioCOndermaximum(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("void OnderMaximumExtra(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCOnderMaximum, true, true, false, true);

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GeneratePrioCAfkapgroen(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("void AfkapGroenExtra(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCAfkapGroen, true, true, false, true);

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GeneratePrioCStartGroenMomenten(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("void StartGroenMomentenExtra(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCStartGroenMomenten, true, true, false, true);

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GeneratePrioCAfkappen(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("void PrioAfkappenExtra(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCAfkappen, true, true, false, true);

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GeneratePrioCTerugkomGroen(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("void PrioTerugkomGroenExtra(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCTerugkomGroen, true, true, false, true);

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GeneratePrioCGroenVasthouden(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("void PrioGroenVasthoudenExtra(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCGroenVasthouden, true, true, false, true);

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GeneratePrioCMeetkriterium(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("void PrioMeetKriteriumExtra(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCMeetkriterium, true, true, false, true);

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GeneratePrioCPrioriteitsToekenning(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* ------------------------------------");
            sb.AppendLine("   Prioriteitsopties toekenning voor OV");
            sb.AppendLine("   ------------------------------------ */");
            sb.AppendLine("void PrioriteitsToekenningExtra(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCPrioriteitsToekenning, true, true, false, false);

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GeneratePrioCTegenhoudenConflicten(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* ------------------------------------");
            sb.AppendLine("   Extra code tegenhouden conflicten OV");
            sb.AppendLine("   ------------------------------------ */");
            sb.AppendLine("void TegenhoudenConflictenExtra(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCTegenhoudenConflicten, true, true, false, false);

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GeneratePrioCPostAfhandelingPrio(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* ---------------------------");
            sb.AppendLine("   Post afhandeling prioriteit");
            sb.AppendLine("   --------------------------- */");
            sb.AppendLine("void PostAfhandelingPrio(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCPostAfhandelingPrio, true, true, false, true);

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GeneratePrioCPARCorrecties(ControllerModel c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* ---------------------------------------");
            sb.AppendLine("   PrioPARCorrecties corrigeert de PAR van");
            sb.AppendLine("   gesynchroniseerde richtingen.");
            sb.AppendLine("   --------------------------------------- */");
            sb.AppendLine("void PrioPARCorrecties(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCPARCorrecties, true, true, false, false);

            sb.AppendLine("}");
            sb.AppendLine();
            return sb.ToString();
        }

        private string GeneratePrioCPARCcol(ControllerModel c)
        {
            var sb = new StringBuilder();

            var _tgb = CCOLGeneratorSettingsProvider.Default.GetElementName("tgb");
            var _tgbhd = CCOLGeneratorSettingsProvider.Default.GetElementName("tgbhd");
            var _trt = CCOLGeneratorSettingsProvider.Default.GetElementName("trt");
            var _trthd = CCOLGeneratorSettingsProvider.Default.GetElementName("trthd");
            var _cvc = CCOLGeneratorSettingsProvider.Default.GetElementName("cvc");
            var _cvchd = CCOLGeneratorSettingsProvider.Default.GetElementName("cvchd");
            var _tblk = CCOLGeneratorSettingsProvider.Default.GetElementName("tblk");
            var _hprio = CCOLGeneratorSettingsProvider.Default.GetElementName("hprio");
            var _hhd = CCOLGeneratorSettingsProvider.Default.GetElementName("hhd");

            sb.AppendLine("/* -------------------------------------------------------");
            sb.AppendLine("   PrioCcol zorgt voor het bijwerken van de CCOL-elementen");
            sb.AppendLine("   voor het richtingen met prioriteit.");
            sb.AppendLine("   ------------------------------------------------------- */");
            sb.AppendLine("void PrioCcol(void) {");
            foreach (var ov in c.PrioData.PrioIngrepen)
            {
                sb.AppendLine($"{ts}PrioCcolElementen(" +
                    $"prioFC{CCOLCodeHelper.GetPriorityName(c, ov)}, " +
                    $"{_tpf}{_tgb}{CCOLCodeHelper.GetPriorityName(c, ov)}, " +
                    $"{_tpf}{_trt}{CCOLCodeHelper.GetPriorityName(c, ov)}, " +
                    $"{_hpf}{_hprio}{CCOLCodeHelper.GetPriorityName(c, ov)}, " +
                    $"{_cpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, ov)}, " +
                    $"{_tpf}{_tblk}{CCOLCodeHelper.GetPriorityName(c, ov)});");
            }
            foreach(var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}PrioCcolElementen(" +
                    $"hdFC{hd.FaseCyclus}, " +
                    $"{_tpf}{_tgbhd}{hd.FaseCyclus}, " +
                    $"{_tpf}{_trthd}{hd.FaseCyclus}, " +
                    $"{_hpf}{_hhd}{hd.FaseCyclus}, " +
                    $"{_cpf}{_cvchd}{hd.FaseCyclus}, " +
                    $"-1);");
            }

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCPARCcol, true, true, false, false);

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GeneratePrioCSpecialSignals(ControllerModel c)
        {
            var sb = new StringBuilder();

            var _prmtestdsivert = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtestdsivert");
            var _prmtestdsilyn = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtestdsilyn");
            var _prmtestdsicat = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtestdsicat");
            var _prmkarsg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmkarsg");
            var _prmkarsghd = CCOLGeneratorSettingsProvider.Default.GetElementName("prmkarsghd");
            var _cvc = CCOLGeneratorSettingsProvider.Default.GetElementName("cvc");
            var _cvchd = CCOLGeneratorSettingsProvider.Default.GetElementName("cvchd");

            sb.AppendLine("/* ----------------------------------------------------------------");
            sb.AppendLine("   PrioSpecialSignals wordt aangeroepen vanuit de functie ");
            sb.AppendLine("   is_special_signals. Deze wordt in de testomgeving gebruikt voor ");
            sb.AppendLine("   het opzetten van bijzondere ingangen.");
            sb.AppendLine("   ---------------------------------------------------------------- */");
            sb.AppendLine("#ifdef CCOL_IS_SPECIAL");
            sb.AppendLine("void PrioSpecialSignals(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCSpecialSignals, false, true, false, true);

            sb.AppendLine($"{ts}/* reset oude set_DSI_message */");
            sb.AppendLine($"{ts}#if !defined VISSIM_GLOBAL_DSI");
            sb.AppendLine($"{ts}{ts}reset_DSI_message();");
            sb.AppendLine($"{ts}#endif");
            sb.AppendLine();

            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine($"{ts}#if !defined AUTOMAAT || defined PRACTICE_TEST");
            }

            if (c.PrioData.PrioIngrepen.Any())
            {
                sb.AppendLine($"{ts}/* Prioriteit ingrepen */");
                foreach (var prio in c.PrioData.PrioIngrepen.Where(x => x.HasPrioIngreepKAR()))
                {
                    if (int.TryParse(prio.FaseCyclus, out var ifc))
                    {
                        var actualIfc = c.PrioData.KARSignaalGroepNummersInParameters
                            ? $"PRM[{_prmpf}{_prmkarsg}{prio.FaseCyclus}]"
                            : ifc > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers 
                                ? (ifc - 200).ToString() 
                                : ifc.ToString();
                        var type = prio.Type == PrioIngreepVoertuigTypeEnum.Bus ? "CIF_BUS" : "CIF_TRAM";
                        foreach (var m in prio.MeldingenData.Inmeldingen.Where(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding && x.DummyKARMelding != null))
                        {
                            sb.AppendLine($"{ts}if (SD[{_dpf}{m.DummyKARMelding.Naam}]) set_DSI_message(NG, {type}, {actualIfc}, CIF_DSIN, 1, PRM[{_prmpf}{_prmtestdsivert}] - 120, PRM[{_prmpf}{_prmtestdsilyn}], PRM[{_prmpf}{_prmtestdsicat}], 0);");
                        }
                        foreach (var m in prio.MeldingenData.Uitmeldingen.Where(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding && x.DummyKARMelding != null))
                        {
                            sb.AppendLine($"{ts}if (SD[{_dpf}{m.DummyKARMelding.Naam}] && C[{_cpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, prio)}]) set_DSI_message(NG, {type}, {actualIfc}, CIF_DSUIT, 1, PRM[{_prmpf}{_prmtestdsivert}] - 120, PRM[{_prmpf}{_prmtestdsilyn}], PRM[{_prmpf}{_prmtestdsicat}], 0);");
                        }
                    }
                }
                var done = new List<string>();
                foreach (var prio in c.PrioData.PrioIngrepen.Where(x => x.HasOVIngreepVecom()))
                {
                    if (int.TryParse(prio.FaseCyclus, out var ifc))
                    {
                        var actualIfc = ifc > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers ? (ifc - 200).ToString() : ifc.ToString();

                        foreach (var m in prio.MeldingenData.Inmeldingen.Where(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector))
                        {
                            if (done.Any(x => x == m.RelatedInput1)) continue;
                            var type = prio.Type == PrioIngreepVoertuigTypeEnum.Bus ? "CIF_BUS" : "CIF_TRAM";
                            if (!string.IsNullOrWhiteSpace(m.RelatedInput1))
                            {
                                sb.AppendLine($"{ts}if (SD[{_dpf}{m.RelatedInput1}]) set_DSI_message({(_dpf + m.RelatedInput1).ToUpper()}, {type}, {actualIfc}, CIF_DSIN, 1, PRM[{_prmpf}{_prmtestdsivert}] - 120, PRM[{_prmpf}{_prmtestdsilyn}], PRM[{_prmpf}{_prmtestdsicat}], NG);");
                                done.Add(m.RelatedInput1);
                            }
                        }
                        foreach (var m in prio.MeldingenData.Uitmeldingen.Where(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector))
                        {
                            if (done.Any(x => x == m.RelatedInput1)) continue;
                            var type = prio.Type == PrioIngreepVoertuigTypeEnum.Bus ? "CIF_BUS" : "CIF_TRAM";
                            if (!string.IsNullOrWhiteSpace(m.RelatedInput1))
                            {
                                sb.AppendLine($"{ts}if (SD[{_dpf}{m.RelatedInput1}] && C[{_cpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, prio)}]) set_DSI_message({(_dpf + m.RelatedInput1).ToUpper()}, {type}, {actualIfc}, {(c.PrioData.CheckOpDSIN && !m.CheckAltijdOpDsinBijVecom ? "CIF_DSUIT" : "CIF_DSIN")}, 1, PRM[{_prmpf}{_prmtestdsivert}] - 120, PRM[{_prmpf}{_prmtestdsilyn}], PRM[{_prmpf}{_prmtestdsicat}], NG);");
                                done.Add(m.RelatedInput1);
                            }
                        }
                    }
                }
                sb.AppendLine();
            }

            if (c.PrioData.HDIngrepen.Count > 0)
            {
                sb.AppendLine($"{ts}/* HD ingrepen */");
                foreach (var hd in c.PrioData.HDIngrepen.Where(x => x.KAR))
                {
	                const string type = "CIF_POL";
	                if (int.TryParse(hd.FaseCyclus, out var ifc))
                    {
                        var actualIfc = c.PrioData.KARSignaalGroepNummersInParameters 
                            ? $"PRM[{_prmpf}{_prmkarsghd}{hd.FaseCyclus}]" 
                            : ifc > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers 
                                ? (ifc - 200).ToString() 
                                : ifc.ToString();

                        sb.AppendLine($"{ts}if (SD[{_dpf}{hd.DummyKARInmelding.Naam}]) set_DSI_message(0, {type}, {actualIfc}, CIF_DSIN, 1, 0, 0, 0, CIF_SIR);");
                        sb.AppendLine($"{ts}if (SD[{_dpf}{hd.DummyKARUitmelding.Naam}] && C[{_cpf}{_cvchd}{hd.FaseCyclus}]) set_DSI_message(0, {type}, {actualIfc}, CIF_DSUIT, 1, 0, 0, 0, CIF_SIR);");
                    }
                }
            }

	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCSpecialSignals, false, true, false, false);

            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine($"{ts}#endif /* !defined AUTOMAAT || defined PRACTICE_TEST */");
            }

            sb.AppendLine("}");
            sb.AppendLine("#endif");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GeneratePrioCBottom(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#ifdef PRIO_ADDFILE");
            sb.AppendLine($"{ts}#include \"{c.Data.Naam}prio.add\"");
            sb.AppendLine("#endif /* PRIO_ADDFILE */");

            if (c.Data.PracticeOmgeving)
            {
                sb.AppendLine();
                sb.AppendLine("#ifdef PRACTICE_TEST");
                sb.AppendLine("#include \"prio.c\"");
                sb.AppendLine("#endif");
            }

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.PrioCBottom, false, true, false, false);

            return sb.ToString();
        }
    }
}