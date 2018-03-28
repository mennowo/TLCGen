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
            sb.Append(GenerateOvCInstellingen(controller));
            sb.Append(GenerateOvCRijTijdScenario(controller));
            sb.Append(GenerateOvCInUitMelden(controller));
            sb.Append(GenerateOvCPrioriteitsOpties(controller));
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

            sb.AppendLine("/*include files */");
            sb.AppendLine("/*------------- */");
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
            sb.AppendLine($"{ts}#ifndef AUTOMAAT");
            sb.AppendLine($"{ts}{ts}#include \"xyprintf.h\" /* Printen debuginfo                 */");
            sb.AppendLine($"{ts}#endif");
            sb.AppendLine($"{ts}#include \"ccolfunc.h\"");
            sb.AppendLine($"{ts}#include \"ccol_mon.h\"");
            sb.AppendLine($"{ts}#include \"extra_func.h\"");
            sb.AppendLine($"{ts}#include \"extra_func_ov.h\"");

			AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCIncludes, true, true);

			return sb.ToString();
        }

        private string GenerateOvCTop(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#define MAX_AANTAL_INMELDINGEN           10");
            sb.AppendLine("#define DEFAULT_MAX_WACHTTIJD           120");
            sb.AppendLine("#define NO_REALISEREN_TOEGESTAAN");
            sb.AppendLine("#define OV_ADDFILE");

            if (c.HalfstarData.IsHalfstar)
            {
                sb.AppendLine();
                sb.AppendLine("/* Declareren OV settings functie halfstar */");
            }

            sb.AppendLine();
            sb.AppendLine("typedef enum {");
            foreach (var fc in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}ovFC{fc.FaseCyclus},");
            }
            foreach (var fc in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}hdFC{fc.FaseCyclus},");
            }
            sb.AppendLine($"{ts}ovOVMAX");
            sb.AppendLine("} TOVRichtingIndex;");
            sb.AppendLine();
            sb.AppendLine("#include \"ov.h\"");
            sb.AppendLine();
            if (c.OVData.OVIngrepen.Count > 0 && c.OVData.OVIngrepen.Any(x => x.KAR) ||
                c.OVData.HDIngrepen.Count > 0 && c.OVData.HDIngrepen.Any(x => x.KAR))
            {
                var any = false;
                var done = new List<string>();
                sb.AppendLine("/* Structs tbv bijhouden laatste DSI berichten per richting in/uit");
                sb.AppendLine("   tbv voorkomen dubbele in/uit meldingen */");
                foreach (var ov in c.OVData.OVIngrepen)
                {
                    if (!any)
                    {
                        sb.AppendLine("/* Richtingen met OV ingreep */");
                        any = true;
                    }
                    sb.AppendLine($"static prevOVkarstruct prevOVkar{ov.FaseCyclus}in, prevOVkar{ov.FaseCyclus}uit;");
                    done.Add(ov.FaseCyclus);
                }
                any = false;
                foreach (var hd in c.OVData.HDIngrepen)
                {
                    if(!done.Contains(hd.FaseCyclus))
                    {
                        if (!any)
                        {
                            sb.AppendLine("/* Richtingen met HD ingreep zonder OV ingreep */");
                            any = true;
                        }
                        sb.AppendLine($"static prevOVkarstruct prevOVkar{hd.FaseCyclus}in, prevOVkar{hd.FaseCyclus}uit;");
                    }
                }
                sb.AppendLine();
                sb.AppendLine("/* Variabele tbv start KAR ondergedrag timer bij starten regeling */");
                sb.AppendLine("static char startkarog = FALSE;");
                
            }

	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCTop, true, true);

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

            if (_anyOVorHd)
            {
                sb.AppendLine($"{ts}/* Fasecyclus voor OV-richtingen */");
            }
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}iFC_OVix[ovFC{ov.FaseCyclus}] = {_fcpf}{ov.FaseCyclus};");
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}iFC_OVix[hdFC{hd.FaseCyclus}] = {_fcpf}{hd.FaseCyclus};");
            }
            sb.AppendLine();

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

			AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCInstellingen, true, true);

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
                        if (fc.Detectoren.Count > 0)
                        {
                            var kl = fc.Detectoren.Where(x => x.Type == DetectorTypeEnum.Kop).ToList();
                            var ll = fc.Detectoren.Where(x => x.Type == DetectorTypeEnum.Lang).ToList();
                            if (ll.Count <= kl.Count)
                            {
                                var i = 0;
                                foreach (var d in kl)
                                {
                                    sb.Append($"{ts}OVRijTijdScenario(ovFC{fc.Naam}, {_dpf}{d.Naam}, ");
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
                                    sb.Append($"{ts}OVRijTijdScenario(ovFC{fc.Naam}, ");
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
                        break;
                    }
                }
            }
            sb.AppendLine();
            foreach (var ov in c.OVData.OVIngrepen.Where(x => x.VersneldeInmeldingKoplus != NooitAltijdAanUitEnum.Nooit))
            {
                var fc = c.Fasen.FirstOrDefault(x => x.Naam == ov.FaseCyclus);
                if (fc != null && fc.Detectoren.Any(x => x.Type == DetectorTypeEnum.Kop))
                {
	                var d = fc.Detectoren.First(x => x.Type == DetectorTypeEnum.Kop);
	                sb.AppendLine(
		                ov.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.Altijd
			                ? $"{ts}if (DB[{_dpf}{d.Naam}] && C[{_cpf}{_cvc}{ov.FaseCyclus}] && (iRijTimer[ovFC{ov.FaseCyclus}] < iRijTijd[ovFC{ov.FaseCyclus}])) iRijTijd[ovFC{ov.FaseCyclus}] = 0;"
			                : $"{ts}if (SCH[{_schpf}{_schvi}{ov.FaseCyclus}] && DB[{_dpf}{d.Naam}] && C[{_cpf}{_cvc}{ov.FaseCyclus}] && (iRijTimer[ovFC{ov.FaseCyclus}] < iRijTijd[ovFC{ov.FaseCyclus}])) iRijTijd[ovFC{ov.FaseCyclus}] = 0;");
                }
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
            var _prmovstp = CCOLGeneratorSettingsProvider.Default.GetElementName("prmovstp");
            var _schcprio = CCOLGeneratorSettingsProvider.Default.GetElementName("schcprio");
            var _prmlaatcrit = CCOLGeneratorSettingsProvider.Default.GetElementName("prmlaatcrit");
            var _prmallelijnen = CCOLGeneratorSettingsProvider.Default.GetElementName("prmallelijnen");
            var _tdhkarin = CCOLGeneratorSettingsProvider.Default.GetElementName("tdhkarin");
            var _tdhkaruit = CCOLGeneratorSettingsProvider.Default.GetElementName("tdhkaruit");
            var _tkarmelding = CCOLGeneratorSettingsProvider.Default.GetElementName("tkarmelding");
            var _tkarog = CCOLGeneratorSettingsProvider.Default.GetElementName("tkarog");
            var _cvc = CCOLGeneratorSettingsProvider.Default.GetElementName("cvc");

            //var _tinmdsi = CCOLGeneratorSettingsProvider.Default.GetElementName("tkarog");
            //var _hinmdsi = CCOLGeneratorSettingsProvider.Default.GetElementName("hinmdsi");
            //var _tuitmdsi = CCOLGeneratorSettingsProvider.Default.GetElementName("tuitmdsi");
            //var _huitmdsi = CCOLGeneratorSettingsProvider.Default.GetElementName("huitmdsi");
            //var _schinmdsi = CCOLGeneratorSettingsProvider.Default.GetElementName("schinmdsi");
            //var _schuitmdsi = CCOLGeneratorSettingsProvider.Default.GetElementName("schuitmdsi");
            //var _tinmkar = CCOLGeneratorSettingsProvider.Default.GetElementName("tinmkar");
            //var _hinmkar = CCOLGeneratorSettingsProvider.Default.GetElementName("hinmkar");
            //var _tuitmkar = CCOLGeneratorSettingsProvider.Default.GetElementName("tuitmkar");
            //var _huitmkar = CCOLGeneratorSettingsProvider.Default.GetElementName("huitmkar");
            //var _schkarov = CCOLGeneratorSettingsProvider.Default.GetElementName("schkarov");
            //var _hinmwsk = CCOLGeneratorSettingsProvider.Default.GetElementName("hinmwsk");
            //var _huitmwsk = CCOLGeneratorSettingsProvider.Default.GetElementName("huitmwsk");
            //var _tuitmwsk = CCOLGeneratorSettingsProvider.Default.GetElementName("tuitmwsk");
            //var _schinmwsk = CCOLGeneratorSettingsProvider.Default.GetElementName("schinmwsk");
            //var _schuitmwsk = CCOLGeneratorSettingsProvider.Default.GetElementName("schuitmwsk");
            //var _hinmss = CCOLGeneratorSettingsProvider.Default.GetElementName("hinmss");
            //var _schinmss = CCOLGeneratorSettingsProvider.Default.GetElementName("schinmss");
            //var _huitmss = CCOLGeneratorSettingsProvider.Default.GetElementName("huitmss");
            //var _tuitmss = CCOLGeneratorSettingsProvider.Default.GetElementName("tuitmss");
            //var _schuitmss = CCOLGeneratorSettingsProvider.Default.GetElementName("schuitmss");
            //var _schgeenwissel = CCOLGeneratorSettingsProvider.Default.GetElementName("schgeenwissel");

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
            sb.AppendLine($"{ts}/* Opzetten hulpelementen voor in- en uitmeldingen */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}] = IH[{_hpf}{_hovuit}{ov.FaseCyclus}] = FALSE;");
            }
            foreach (var hd in c.OVData.HDIngrepen)
            {
                sb.AppendLine($"{ts}IH[{_hpf}{_hhdin}{hd.FaseCyclus}] = IH[{_hpf}{_hhduit}{hd.FaseCyclus}] = FALSE;");
            }

            #region Trams Den Haag

            //foreach (var ov in c.OVData.OVIngrepen.Where(x => x.Type == OVIngreepVoertuigTypeEnum.Tram))
            //{
            //    if (ov.Vecom)
            //    {
            //        sb.AppendLine($"{ts}IH[{_hpf}{_hinmdsi}{ov.FaseCyclus}] = RT[{_tpf}{_tinmdsi}{ov.FaseCyclus}] = SCH[{_schpf}{_schinmdsi}{ov.FaseCyclus}] && DSI_melding(ds{ov.FaseCyclus}_in, {_fcpf}{ov.FaseCyclus}, CIF_DSIN, -1, -1, -1) && !T[{_tpf}{_tinmdsi}{ov.FaseCyclus}];");
            //    }
            //}

            #endregion

            #region In- en uitmeldingen

            foreach (var ov in c.OVData.OVIngrepen)
            {
                if (ov.Meldingen.Any(x => x.Inmelding))
                {
                    var inmHelems = new List<string>();
                    sb.AppendLine($"{ts}/* Inmelding {_fcpf}{ov.FaseCyclus} */");
                    foreach (var melding in ov.Meldingen)
                    {
                        if (!melding.Inmelding) continue;
                        switch (melding.Type)
                        {
                            case OVIngreepMeldingTypeEnum.VECOM:
                                sb.AppendLine($"{ts}IH[{_hpf}in{ov.FaseCyclus}vc] = RT[{_tpf}in{ov.FaseCyclus}vc] = SCH[{_schpf}in{ov.FaseCyclus}vc] && DSI_melding(ds{ov.FaseCyclus}_in, {_fcpf}{ov.FaseCyclus}, CIF_DSIN, -1, -1, -1) && !T[{_tpf}in{ov.FaseCyclus}vc];");
                                inmHelems.Add($"{_hpf}in{ov.FaseCyclus}vc");
                                break;
                            case OVIngreepMeldingTypeEnum.KAR:
                                sb.AppendLine($"{ts}IH[{_hpf}in{ov.FaseCyclus}kar] = RT[{_tpf}in{ov.FaseCyclus}kar] = SCH[{_schpf}in{ov.FaseCyclus}kar] && DSI_melding(0, {_fcpf}{ov.FaseCyclus}, CIF_DSIN, -1, -1, -1) && !T[{_tpf}in{ov.FaseCyclus}kar];");
                                inmHelems.Add($"{_hpf}in{ov.FaseCyclus}kar");
                                break;
                            case OVIngreepMeldingTypeEnum.VerlosDetector:
                                if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                {
                                    sb.AppendLine($"{ts}IH[{_hpf}in{ov.FaseCyclus}ss] = SCH[{_schpf}in{ov.FaseCyclus}ss] && (D[{_dpf}{ov.WisselStandInput}] || SCH[{_schpf}geenwissel{ov.WisselStandInput}]) && R[{_fcpf}{ov.FaseCyclus}] && !TRG[{_fcpf}{ov.FaseCyclus}] && DB[{_dpf}{melding.RelatedInput}] && (CIF_IS[{_dpf}{melding.RelatedInput}] < CIF_DET_STORING) && (C_counter[{_cpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}IH[{_hpf}in{ov.FaseCyclus}ss] = SCH[{_schpf}in{ov.FaseCyclus}ss] && R[{_fcpf}{ov.FaseCyclus}] && !TRG[{_fcpf}{ov.FaseCyclus}] && DB[{_dpf}{melding.RelatedInput}] && (CIF_IS[{_dpf}{melding.RelatedInput}] < CIF_DET_STORING) && (C_counter[{_cpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                }
                                inmHelems.Add($"{_hpf}in{ov.FaseCyclus}ss");
                                break;
                            case OVIngreepMeldingTypeEnum.WisselStroomKringDetector:
                                if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                {
                                    sb.AppendLine($"{ts}IH[{_hpf}in{ov.FaseCyclus}wsk] = SCH[{_schpf}in{ov.FaseCyclus}wsk] && (D[{_dpf}{ov.WisselStandInput}] || SCH[{_schpf}geenwissel{ov.WisselStandInput}]) && R[{_fcpf}{ov.FaseCyclus}] && !TRG[{_fcpf}{ov.FaseCyclus}] && DB[{_dpf}{melding.RelatedInput}] && (CIF_IS[{_dpf}{melding.RelatedInput}] < CIF_DET_STORING) && (C_counter[{_cpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                    inmHelems.Add($"{_hpf}in{ov.FaseCyclus}wsk");
                                }
                                break;
                            case OVIngreepMeldingTypeEnum.WisselDetector:
                                break;
                            default:
                                throw new IndexOutOfRangeException();
                        }
                    }
                    sb.Append($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}] = ");
                    var first = true;
                    foreach(var i in inmHelems)
                    {
                        if (!first) sb.Append(" || ");
                        sb.Append($"IH[{i}]");
                        first = false;
                    }
                    sb.AppendLine(";");
                    sb.AppendLine();
                }

                if (ov.Meldingen.Any(x => x.Uitmelding))
                {
                    var uitmHelems = new List<string>();
                    sb.AppendLine($"{ts}/* Uitmelding {_fcpf}{ov.FaseCyclus} */");
                    foreach (var melding in ov.Meldingen)
                    {
                        if (!melding.Uitmelding) continue;
                        switch (melding.Type)
                        {
                            case OVIngreepMeldingTypeEnum.VECOM:
                                sb.AppendLine($"{ts}IH[{_hpf}uit{ov.FaseCyclus}vc] = SCH[{_schpf}uit{ov.FaseCyclus}vc] && DSI_melding(ds{ov.FaseCyclus}_uit, {_fcpf}{ov.FaseCyclus}, CIF_DSIN, -1, -1, -1) && !T[{_tpf}uit{ov.FaseCyclus}];");
                                uitmHelems.Add($"{_hpf}uit{ov.FaseCyclus}vc");
                                break;
                            case OVIngreepMeldingTypeEnum.KAR:
                                sb.AppendLine($"{ts}IH[{_hpf}uit{ov.FaseCyclus}kar] = SCH[{_schpf}uit{ov.FaseCyclus}kar] && DSI_melding(0, {_fcpf}{ov.FaseCyclus}, CIF_DSUIT, -1, -1, -1) && !T[{_tpf}uit{ov.FaseCyclus}];");
                                uitmHelems.Add($"{_hpf}uit{ov.FaseCyclus}kar");
                                break;
                            case OVIngreepMeldingTypeEnum.VerlosDetector:
                                if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                {
                                    sb.AppendLine($"{ts}IH[{_hpf}uit{ov.FaseCyclus}ss] = SCH[{_schpf}uit{ov.FaseCyclus}ss] && (D[{_dpf}{ov.WisselStandInput}] || SCH[{_schpf}geenwissel{ov.WisselStandInput}]) && !TDH[{_dpf}{melding.RelatedInput}] && TDH_old[{_dpf}{melding.RelatedInput}] && (CIF_IS[{_dpf}{melding.RelatedInput}] < CIF_DET_STORING) && !T[{_tpf}uit{ov.FaseCyclus}] && (C_counter[{_cpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}IH[{_hpf}uit{ov.FaseCyclus}ss] = SCH[{_schpf}uit{ov.FaseCyclus}ss] && !TDH[{_dpf}{melding.RelatedInput}] && TDH_old[{_dpf}{melding.RelatedInput}] && (CIF_IS[{_dpf}{melding.RelatedInput}] < CIF_DET_STORING) && !T[{_tpf}uit{ov.FaseCyclus}] && (C_counter[{_cpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                }
                                uitmHelems.Add($"{_hpf}uit{ov.FaseCyclus}ss");
                                break;
                            case OVIngreepMeldingTypeEnum.WisselStroomKringDetector:
                                if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                {
                                    sb.AppendLine($"{ts}IH[{_hpf}uit{ov.FaseCyclus}wsk] = SCH[{_schpf}uit{ov.FaseCyclus}wsk] && (D[{_dpf}{ov.WisselStandInput}] || SCH[{_schpf}geenwissel{ov.WisselStandInput}]) && !TDH[{_dpf}{melding.RelatedInput}] && TDH_old[{_dpf}{melding.RelatedInput}] && (CIF_IS[{_dpf}{melding.RelatedInput}] < CIF_DET_STORING) && !T[{_tpf}uit{ov.FaseCyclus}] && (C_counter[{_cpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                    uitmHelems.Add($"{_hpf}uit{ov.FaseCyclus}wsk");
                                }
                                break;
                            case OVIngreepMeldingTypeEnum.WisselDetector:
                                if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                {
                                    sb.AppendLine($"{ts}IH[{_hpf}uit{ov.FaseCyclus}wd] = SCH[{_schpf}in{ov.FaseCyclus}wd] && (D[{_dpf}{ov.WisselStandInput}] || SCH[{_schpf}geenwissel{ov.WisselStandInput}]) && R[{_fcpf}{ov.FaseCyclus}] && !TRG[{_fcpf}{ov.FaseCyclus}] && DB[{_dpf}{melding.RelatedInput}] && !DB_old[{_dpf}{melding.RelatedInput}] && (CIF_IS[{_dpf}{melding.RelatedInput}] < CIF_DET_STORING) && !T[{_tpf}uit{ov.FaseCyclus}] && (C_counter[{_cpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                    uitmHelems.Add($"{_hpf}uit{ov.FaseCyclus}wd");
                                }
                                break;
                            default:
                                throw new IndexOutOfRangeException();
                        }
                    }
                    sb.Append($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}] = RT[{_tpf}uit{ov.FaseCyclus}] = ");
                    var first = true;
                    foreach (var i in uitmHelems)
                    {
                        if (!first) sb.Append(" || ");
                        sb.Append($"IH[{i}]");
                        first = false;
                    }
                    sb.AppendLine($";");
                    sb.AppendLine();
                }
            }

            #endregion // In- en uitmeldingen

            //#region OV ingrepen KAR
            //
            //var karovfcs = c.OVData.OVIngrepen.Where(x => x.KAR);
	        //var ovIngreepModels = karovfcs as OVIngreepModel[] ?? karovfcs.ToArray();
	        //foreach (var ov in ovIngreepModels)
            //{
	        //    if (int.TryParse(ov.FaseCyclus, out var ifc))
            //    {
            //        var type = ov.Type == OVIngreepVoertuigTypeEnum.Bus ? "CIF_BUS" : "CIF_TRAM";
            //        sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}] |= OVmelding_KAR_V2({type}, {ifc}, PRM[{_prmpf}{_prmovstp}{ov.FaseCyclus}], CIF_DSIN, SCH[{_schpf}{_schcprio}], PRM[{_prmpf}{_prmlaatcrit}], {_prmpf}{_prmallelijnen}{ov.FaseCyclus}, {ov.LijnNummers.Count}, &prevOVkar{ov.FaseCyclus}in, {_tpf}{_tdhkarin}{ov.FaseCyclus});");
            //    }
            //}
            //foreach (var ov in ovIngreepModels)
            //{
	        //    if (int.TryParse(ov.FaseCyclus, out var ifc))
            //    {
            //        var type = ov.Type == OVIngreepVoertuigTypeEnum.Bus ? "CIF_BUS" : "CIF_TRAM";
            //        sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}] |= OVmelding_KAR_V2({type}, {ifc}, PRM[{_prmpf}{_prmovstp}{ov.FaseCyclus}], CIF_DSUIT, (bool)FALSE, 0, {_prmpf}{_prmallelijnen}{ov.FaseCyclus}, {ov.LijnNummers.Count}, &prevOVkar{ov.FaseCyclus}uit, {_tpf}{_tdhkaruit}{ov.FaseCyclus});");
            //    }
            //}
            //
            //#endregion // OV ingrepen KAR
            //
            //#region OV ingrepen VECOM
            //
            //var vecomovfcs = c.OVData.OVIngrepen.Where(x => x.Vecom);
            //foreach (var ov in vecomovfcs)
            //{
            //    var type = ov.Type == OVIngreepVoertuigTypeEnum.Bus ? "BUS" : "TRAM";
            //    sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}] |= OVmelding_DSI_{type}(ds{ov.FaseCyclus}_in, NG, NG, {_prmpf}{_prmallelijnen}{ov.FaseCyclus}, {ov.LijnNummers.Count});");
            //}
            //foreach (var ov in vecomovfcs)
            //{
            //    var type = ov.Type == OVIngreepVoertuigTypeEnum.Bus ? "BUS" : "TRAM";
            //    sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}] |= OVmelding_DSI_{type}(ds{ov.FaseCyclus}_uit, NG, NG, {_prmpf}{_prmallelijnen}{ov.FaseCyclus}, {ov.LijnNummers.Count});");
            //}
            //
            //#endregion // OV ingrepen VECOM

            #region HD ingrepen KAR

            var karhdfcs = c.OVData.HDIngrepen.Where(x => x.KAR);
	        var hdIngreepModels = karhdfcs as HDIngreepModel[] ?? karhdfcs.ToArray();
	        foreach (var hd in hdIngreepModels)
            {
	            if (int.TryParse(hd.FaseCyclus, out var ifc))
                {
                    var start = $"{ts}IH[{_hpf}{_hhdin}{hd.FaseCyclus}] |= ";
                    sb.AppendLine($"{start}HDmelding_KAR_V1(CIF_AMB, CIF_SIR, {ifc}, CIF_DSIN, &prevOVkar{hd.FaseCyclus}in, {_tpf}{_tdhkarin}{hd.FaseCyclus}) ||");
                    sb.Append("".PadRight(start.Length));
                    sb.AppendLine($"HDmelding_KAR_V1(CIF_POL, CIF_SIR, {ifc}, CIF_DSIN, &prevOVkar{hd.FaseCyclus}in, {_tpf}{_tdhkarin}{hd.FaseCyclus}) ||");
                    sb.Append("".PadRight(start.Length));
                    sb.AppendLine($"HDmelding_KAR_V1(CIF_BRA, CIF_SIR, {ifc}, CIF_DSIN, &prevOVkar{hd.FaseCyclus}in, {_tpf}{_tdhkarin}{hd.FaseCyclus});");
                }
            }
            foreach (var hd in hdIngreepModels)
            {
	            if (int.TryParse(hd.FaseCyclus, out var ifc))
                {
                    var start = $"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}] |= ";
                    sb.AppendLine($"{start}HDmelding_KAR_V1(CIF_AMB, CIF_SIR, {ifc}, CIF_DSUIT, &prevOVkar{hd.FaseCyclus}uit, {_tpf}{_tdhkaruit}{hd.FaseCyclus}) ||");
                    sb.Append("".PadRight(start.Length));
                    sb.AppendLine($"HDmelding_KAR_V1(CIF_POL, CIF_SIR, {ifc}, CIF_DSUIT, &prevOVkar{hd.FaseCyclus}uit, {_tpf}{_tdhkaruit}{hd.FaseCyclus}) ||");
                    sb.Append("".PadRight(start.Length));
                    sb.AppendLine($"HDmelding_KAR_V1(CIF_BRA, CIF_SIR, {ifc}, CIF_DSUIT, &prevOVkar{hd.FaseCyclus}uit, {_tpf}{_tdhkaruit}{hd.FaseCyclus});");
                }
            }

            #endregion // HD ingrepen KAR

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

            if (c.OVData.OVIngrepen.Count > 0 && c.OVData.OVIngrepen.Any(x => x.KAR) ||
                c.OVData.HDIngrepen.Count > 0 && c.OVData.HDIngrepen.Any(x => x.KAR))
            {
                sb.AppendLine();
                sb.AppendLine($"{ts}/* Bijhouden melding en ondergedrag KAR */");
                sb.AppendLine($"{ts}RT[{_tpf}{_tkarmelding}] = CIF_DSIWIJZ != 0;");
                sb.AppendLine($"{ts}RT[{_tpf}{_tkarog}] = T[{_tpf}{_tkarmelding}] || !startkarog;");
                sb.AppendLine($"{ts}if (!startkarog) startkarog = TRUE;");
            }

	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCInUitMelden, true, true);

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
            
            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCPrioriteitsOpties, true, true);

            sb.AppendLine($"{ts}PrioriteitsOpties_Add();");
            sb.AppendLine($"{ts}PrioriteitsNiveau_Add();");
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

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCPrioriteitsToekenning, true, true);
            
            sb.AppendLine("}");

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

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCPostAfhandelingOV, true, true);

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

	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCPARCorrecties, true, true);

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

            sb.AppendLine("/*-----------------------------------------------------");
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

	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCPARCcol, true, true);

			sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateOvCSpecialSignals(ControllerModel c)
        {
            var sb = new StringBuilder();

            var _prmtestkarvert = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtestkarvert");
            var _prmtestkarlyn = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtestkarlyn");
            var _ddummykarin = CCOLGeneratorSettingsProvider.Default.GetElementName("ddummykarin");
            var _ddummykaruit = CCOLGeneratorSettingsProvider.Default.GetElementName("ddummykaruit");
            var _ddummykarhdin = CCOLGeneratorSettingsProvider.Default.GetElementName("ddummykarhdin");
            var _ddummykarhduit = CCOLGeneratorSettingsProvider.Default.GetElementName("ddummykarhduit");
            var _ddummyvecomin = CCOLGeneratorSettingsProvider.Default.GetElementName("ddummyvecomin");
            var _ddummyvecomuit = CCOLGeneratorSettingsProvider.Default.GetElementName("ddummyvecomuit");

            sb.AppendLine("/*----------------------------------------------------------------");
            sb.AppendLine("   OVSpecialSignals wordt aangeroepen vanuit de functie ");
            sb.AppendLine("   is_special_signals. Deze wordt in de testomgeving gebruikt voor ");
            sb.AppendLine("   het opzetten van bijzondere ingangen.");
            sb.AppendLine("   ---------------------------------------------------------------- */");
            sb.AppendLine("#ifdef CCOL_IS_SPECIAL");
            sb.AppendLine("void OVSpecialSignals(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}/* reset oude set_DSI_message */");
            sb.AppendLine($"{ts}#ifndef VISSIM");
            sb.AppendLine($"{ts}{ts}reset_DSI_message();");
            sb.AppendLine($"{ts}#endif");
            sb.AppendLine();
            if (c.OVData.OVIngrepen.Count > 0)
            {
                sb.AppendLine($"{ts}/* OV ingrepen */");
                foreach (var ov in c.OVData.OVIngrepen.Where(x => x.KAR))
                {
	                if (int.TryParse(ov.FaseCyclus, out var ifc))
                    {
                        var type = ov.Type == OVIngreepVoertuigTypeEnum.Bus ? "CIF_BUS" : "CIF_TRAM";
                        sb.AppendLine($"{ts}if (SD[{_dpf}{ov.DummyKARInmelding.Naam}]) set_DSI_message_KAR({type}, {ifc}, CIF_DSIN, 1, PRM[{_prmpf}{_prmtestkarvert}], PRM[{_prmpf}{_prmtestkarlyn}], 0);");
                        sb.AppendLine($"{ts}if (SD[{_dpf}{ov.DummyKARUitmelding.Naam}]) set_DSI_message_KAR({type}, {ifc}, CIF_DSUIT, 1, PRM[{_prmpf}{_prmtestkarvert}], PRM[{_prmpf}{_prmtestkarlyn}], 0);");
                    }
                }
                if (c.OVData.DSI)
                {
                    foreach (var ov in c.OVData.OVIngrepen.Where(x => x.Vecom))
                    {
                        var type = ov.Type == OVIngreepVoertuigTypeEnum.Bus ? "CIF_BUS" : "CIF_TRAM";
                        sb.AppendLine($"{ts}if (SD[{_dpf}{ov.DummyVecomInmelding.Naam}]) set_DSI_message(ds{ov.FaseCyclus}_in, {type}, CIF_DSIN, PRM[{_prmpf}{_prmtestkarlyn}], NG);");
                        sb.AppendLine($"{ts}if (SD[{_dpf}{ov.DummyVecomUitmelding.Naam}]) set_DSI_message(ds{ov.FaseCyclus}_uit, {type}, CIF_DSUIT, PRM[{_prmpf}{_prmtestkarlyn}], NG);");
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
                        sb.AppendLine($"{ts}if (SD[{_dpf}{hd.DummyKARInmelding.Naam}]) set_DSI_message_KAR({type}, {ifc}, CIF_DSIN, 1, 0, 0, CIF_SIR);");
                        sb.AppendLine($"{ts}if (SD[{_dpf}{hd.DummyKARUitmelding.Naam}]) set_DSI_message_KAR({type}, {ifc}, CIF_DSUIT, 1, 0, 0, CIF_SIR);");
                    }
                }
            }

	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCSpecialSignals, true, true);
			
			sb.AppendLine("}");
            sb.AppendLine("#endif");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateOvCBottom(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#include \"ov.c\"");
            sb.AppendLine($"#include \"{c.Data.Naam}ov.add\"");

	        AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.OvCBottom, true, true);
			
			return sb.ToString();
        }
    }
}