using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateOvC(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

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
            sb.Append(GenerateOvCPARCorrecties(controller));
            sb.Append(GenerateOvCPARCcol(controller));
            sb.Append(GenerateOvCSpecialSignals(controller));
            sb.Append(GenerateOvCBottom(controller));

            return sb.ToString();
        }

        private string GenerateOvCIncludes(ControllerModel c)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"/*include files */");
            sb.AppendLine($"/*------------- */");
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
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateOvCTop(ControllerModel c)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("#define MAX_AANTAL_INMELDINGEN           10");
            sb.AppendLine("#define DEFAULT_MAX_WACHTTIJD           120");
            sb.AppendLine("#define NO_REALISEREN_TOEGESTAAN");
            sb.AppendLine("#define OV_ADDFILE");

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
            sb.AppendLine("#include \"syncvar.h\"");
            sb.AppendLine();
            if (c.OVData.OVIngrepen.Count > 0 && c.OVData.OVIngrepen.Where(x => x.KAR).Any() ||
               c.OVData.HDIngrepen.Count > 0 && c.OVData.HDIngrepen.Where(x => x.KAR).Any())
            {
                sb.AppendLine("/*Structs tbv bijhouden laatste DSI berichten per richting in/uit");
                sb.AppendLine("tbv voorkomen dubbele in/uit meldingen */");
                foreach (var ov in c.OVData.OVIngrepen)
                {
                    sb.AppendLine($"static prevOVkarstruct prevOVkar{ov.FaseCyclus}in, prevOVkar{ov.FaseCyclus}uit;");
                }
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

        private string GenerateOvCInstellingen(ControllerModel c)
        {
            StringBuilder sb = new StringBuilder();

            string _tgb = CCOLGeneratorSettingsProvider.Default.GetElementName("tgb");
            string _tgbhd = CCOLGeneratorSettingsProvider.Default.GetElementName("tgbhd");
            string _prmrto = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrto");
            string _prmrtbg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtbg");
            string _prmrtg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtg");
            string _prmrtohd = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtohd");
            string _prmrtbghd = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtbghd");
            string _prmrtghd = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtghd");
            string _hov = CCOLGeneratorSettingsProvider.Default.GetElementName("hov");
            string _hhd = CCOLGeneratorSettingsProvider.Default.GetElementName("hhd");
            string _hovin = CCOLGeneratorSettingsProvider.Default.GetElementName("hovin");
            string _hhdin = CCOLGeneratorSettingsProvider.Default.GetElementName("hhdin");
            string _hovuit = CCOLGeneratorSettingsProvider.Default.GetElementName("hovuit");
            string _hhduit = CCOLGeneratorSettingsProvider.Default.GetElementName("hhduit");
            string _prmprio = CCOLGeneratorSettingsProvider.Default.GetElementName("prmprio");
            string _prmpriohd = CCOLGeneratorSettingsProvider.Default.GetElementName("prmpriohd");
            string _prmomx = CCOLGeneratorSettingsProvider.Default.GetElementName("prmomx");
            string _tblk = CCOLGeneratorSettingsProvider.Default.GetElementName("tblk");
            string _schupinagb = CCOLGeneratorSettingsProvider.Default.GetElementName("schupinagb");
            string _schupinagbhd = CCOLGeneratorSettingsProvider.Default.GetElementName("schupinagbhd");
            string _prmmwta = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmwta");
            string _prmmwtfts = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmwtfts");
            string _prmmwtvtg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmwtvtg");
            string _prmpmgt = CCOLGeneratorSettingsProvider.Default.GetElementName("prmpmgt");
            string _prmognt = CCOLGeneratorSettingsProvider.Default.GetElementName("prmognt");
            string _prmnofm = CCOLGeneratorSettingsProvider.Default.GetElementName("prmnofm");
            string _prmmgcov = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmgcov");
            string _prmpmgcov = CCOLGeneratorSettingsProvider.Default.GetElementName("prmpmgcov");
            string _prmaltp = CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltp");
            string _prmaltg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltg");
            string _schaltg = CCOLGeneratorSettingsProvider.Default.GetElementName("schaltg");
            string _prmohpmg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmohpmg");

            sb.AppendLine($"void OVInstellingen(void) ");
            sb.AppendLine($"{{");
            sb.AppendLine($"{ts}/* ============================= */");
            sb.AppendLine($"{ts}/* Instellingen OV-richtingen    */");
            sb.AppendLine($"{ts}/* ============================= */");
            sb.AppendLine();

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
            sb.AppendLine($"	   detectie niet langer betrouwbaar gevonden */");
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
            sb.AppendLine($"   ");
            sb.AppendLine($"{ts}/* Maximale wachttijd */");
            foreach (var fc in c.Fasen)
            {
                switch (fc.Type)
                {
                    case Models.Enumerations.FaseTypeEnum.OV:
                    case Models.Enumerations.FaseTypeEnum.Auto:
                        sb.AppendLine($"{ts}iMaximumWachtTijd[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmmwta}];");
                        break;
                    case Models.Enumerations.FaseTypeEnum.Fiets:
                        sb.AppendLine($"{ts}iMaximumWachtTijd[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmmwtfts}];");
                        break;
                    case Models.Enumerations.FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iMaximumWachtTijd[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmmwtvtg}];");
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Als een richting minder groen heeft gehad door afkappen");
            sb.AppendLine($"{ts}   dan deze instelling, dan mag de richting nog een keer");
            sb.AppendLine($"{ts}   primair realiseren (terugkomen). */");
            foreach (var fc in c.Fasen)
            {
                bool skip = false;
                foreach (var ov in c.OVData.OVIngrepen)
                {
                    if (ov.FaseCyclus == fc.Naam)
                        skip = true;
                }
                if (skip)
                    continue;

                switch (fc.Type)
                {
                    case Models.Enumerations.FaseTypeEnum.OV:
                    case Models.Enumerations.FaseTypeEnum.Auto:
                        continue;
                    case Models.Enumerations.FaseTypeEnum.Fiets:
                    case Models.Enumerations.FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iInstPercMaxGroenTijdTerugKomen[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmpmgt}{fc.Naam}];");
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* De minimale groentijd die een richting krijgt als");
            sb.AppendLine($"{ts}   deze mag terugkomen. */");
            foreach (var fc in c.Fasen)
            {
                bool skip = false;
                foreach (var ov in c.OVData.OVIngrepen)
                {
                    if (ov.FaseCyclus == fc.Naam)
                        skip = true;
                }
                if (skip)
                    continue;

                switch (fc.Type)
                {
                    case Models.Enumerations.FaseTypeEnum.OV:
                    case Models.Enumerations.FaseTypeEnum.Auto:
                        continue;
                    case Models.Enumerations.FaseTypeEnum.Fiets:
                    case Models.Enumerations.FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iInstMinTerugKomGroenTijd[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmognt}{fc.Naam}];");
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Aantal malen niet afkappen */");
            foreach (var fc in c.Fasen)
            {
                bool skip = false;
                foreach (var ov in c.OVData.OVIngrepen)
                {
                    if (ov.FaseCyclus == fc.Naam)
                        skip = true;
                }
                if (skip)
                    continue;

                switch (fc.Type)
                {
                    case Models.Enumerations.FaseTypeEnum.Voetganger:
                        continue;
                    case Models.Enumerations.FaseTypeEnum.OV:
                    case Models.Enumerations.FaseTypeEnum.Auto:
                    case Models.Enumerations.FaseTypeEnum.Fiets:
                        sb.AppendLine($"{ts}iInstAantalMalenNietAfkappen[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmnofm}{fc.Naam}];");
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Onder deze groentijd mag een richting niet worden");
            sb.AppendLine($"{ts}   afgekapt, tenzij zich een nooddienst heeft ingemeld. */");
            foreach (var fc in c.Fasen)
            {
                bool skip = false;
                foreach (var ov in c.OVData.OVIngrepen)
                {
                    if (ov.FaseCyclus == fc.Naam)
                        skip = true;
                }
                if (skip)
                    continue;

                switch (fc.Type)
                {
                    case Models.Enumerations.FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iAfkapGroenTijd[{_fcpf}{fc.Naam}] = 0;");
                        continue;
                    case Models.Enumerations.FaseTypeEnum.OV:
                    case Models.Enumerations.FaseTypeEnum.Auto:
                    case Models.Enumerations.FaseTypeEnum.Fiets:
                        sb.AppendLine($"{ts}iAfkapGroenTijd[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmmgcov}{fc.Naam}];");
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Als een richting minder groen heeft gehad dan");
            sb.AppendLine($"{ts}   dit percentage van de maximum groentijd, dan");
            sb.AppendLine($"{ts}   mag de richting niet worden afgekapt, tenzij");
            sb.AppendLine($"{ts}   zich een nooddienst heeft ingemeld. */");
            foreach (var fc in c.Fasen)
            {
                bool skip = false;
                foreach (var ov in c.OVData.OVIngrepen)
                {
                    if (ov.FaseCyclus == fc.Naam)
                        skip = true;
                }
                if (skip)
                    continue;

                switch (fc.Type)
                {
                    case Models.Enumerations.FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iPercGroenTijd[{_fcpf}{fc.Naam}] = 100; /* Voetgangers mogen niet worden afgekapt. */");
                        continue;
                    case Models.Enumerations.FaseTypeEnum.OV:
                    case Models.Enumerations.FaseTypeEnum.Auto:
                    case Models.Enumerations.FaseTypeEnum.Fiets:
                        sb.AppendLine($"{ts}iPercGroenTijd[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmpmgcov}{fc.Naam}];");
                        break;
                }
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Na te zijn afgekapt, wordt het percentage");
            sb.AppendLine($"{ts}   van de maximumgroentijd verhoogd met dit ophoogpercentage. */");
            foreach (var fc in c.Fasen)
            {
                bool skip = false;
                foreach (var ov in c.OVData.OVIngrepen)
                {
                    if (ov.FaseCyclus == fc.Naam)
                        skip = true;
                }
                if (skip)
                    continue;

                switch (fc.Type)
                {
                    case Models.Enumerations.FaseTypeEnum.Voetganger:
                        sb.AppendLine($"{ts}iInstOphoogPercentageMG[{_fcpf}{fc.Naam}] = 0;");
                        continue;
                    case Models.Enumerations.FaseTypeEnum.OV:
                    case Models.Enumerations.FaseTypeEnum.Auto:
                    case Models.Enumerations.FaseTypeEnum.Fiets:
                        sb.AppendLine($"{ts}iInstOphoogPercentageMG[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmohpmg}{fc.Naam}];");
                        break;
                }
            }
            sb.AppendLine();

            if (c.ModuleMolen.LangstWachtendeAlternatief)
            {
                sb.AppendLine($"{ts}/* Benodigde ruimte voor alternatieve realisatie tijdens een OV ingreep */");
                foreach (var fc in c.Fasen)
                {
                    sb.AppendLine($"{ts}iPRM_ALTP[{_fcpf}{fc.Naam}] = PRM[{_prmpf}{_prmaltp}{fc.Naam}];");
                }
                sb.AppendLine();

                sb.AppendLine($"{ts}/* Richting mag alternatief realiseren tijdens een OV ingreep*/");
                foreach (var fc in c.Fasen)
                {
                    sb.AppendLine($"{ts}iSCH_ALTG[{_fcpf}{fc.Naam}] = SCH[{_schpf}{_schaltg}{fc.Naam}];");
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

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateOvCRijTijdScenario(ControllerModel c)
        {
            StringBuilder sb = new StringBuilder();

            string _tbtovg = CCOLGeneratorSettingsProvider.Default.GetElementName("tbtovg");

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
                            var kl = fc.Detectoren.Where(x => x.Type == Models.Enumerations.DetectorTypeEnum.Kop).ToList();
                            var ll = fc.Detectoren.Where(x => x.Type == Models.Enumerations.DetectorTypeEnum.Lang).ToList();
                            if (ll.Count <= kl.Count)
                            {
                                int i = 0;
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
                                int i = 0;
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
            sb.AppendLine($"}}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateOvCInUitMelden(ControllerModel c)
        {
            StringBuilder sb = new StringBuilder();

            string _hovin = CCOLGeneratorSettingsProvider.Default.GetElementName("hovin");
            string _hovuit = CCOLGeneratorSettingsProvider.Default.GetElementName("hovuit");
            string _prmovstp = CCOLGeneratorSettingsProvider.Default.GetElementName("prmovstp");
            string _schcprio = CCOLGeneratorSettingsProvider.Default.GetElementName("schcprio");
            string _prmlaatcrit = CCOLGeneratorSettingsProvider.Default.GetElementName("prmlaatcrit");
            string _prmallelijnen = CCOLGeneratorSettingsProvider.Default.GetElementName("prmallelijnen");
            string _tdhkarin = CCOLGeneratorSettingsProvider.Default.GetElementName("tdhkarin");
            string _tdhkaruit = CCOLGeneratorSettingsProvider.Default.GetElementName("tdhkaruit");
            
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
            sb.AppendLine($"void InUitMelden(void)");
            sb.AppendLine("{");
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
            sb.AppendLine($"{ts}/* Opzetten hulpelementen voor in- en uitmeldingen */");
            foreach (var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}] = IH[{_hpf}{_hovuit}{ov.FaseCyclus}] = FALSE;");
            }

            var karovfcs = c.OVData.OVIngrepen.Where(x => x.KAR);
            foreach (var ov in karovfcs)
            {
                int ifc;
                if (Int32.TryParse(ov.FaseCyclus, out ifc))
                {
                    string type = ov.Type == Models.Enumerations.OVIngreepVoertuigTypeEnum.Bus ? "CIF_BUS" : "CIF_TRAM";
                    sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}] |= OVmelding_KAR_V2({type}, {ifc}, PRM[{_prmpf}{_prmovstp}{ov.FaseCyclus}], CIF_DSIN, SCH[{_schpf}{_schcprio}], PRM[{_prmpf}{_prmlaatcrit}], {_prmpf}{_prmallelijnen}{ov.FaseCyclus}, {ov.LijnNummers.Count}, &prevOVkar{ov.FaseCyclus}in, {_tpf}{_tdhkarin}{ov.FaseCyclus});");
                }
            }
            foreach (var ov in karovfcs)
            {
                int ifc;
                if (Int32.TryParse(ov.FaseCyclus, out ifc))
                {
                    string type = ov.Type == Models.Enumerations.OVIngreepVoertuigTypeEnum.Bus ? "CIF_BUS" : "CIF_TRAM";
                    sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}] |= OVmelding_KAR_V2({type}, {ifc}, PRM[{_prmpf}{_prmovstp}{ov.FaseCyclus}], CIF_DSUIT, (bool)FALSE, 0, {_prmpf}{_prmallelijnen}{ov.FaseCyclus}, {ov.LijnNummers.Count}, &prevOVkar{ov.FaseCyclus}uit, {_tpf}{_tdhkaruit}{ov.FaseCyclus});");
                }
            }

            var vecomovfcs = c.OVData.OVIngrepen.Where(x => x.KAR);
            foreach (var ov in vecomovfcs)
            {
                string type = ov.Type == Models.Enumerations.OVIngreepVoertuigTypeEnum.Bus ? "BUS" : "TRAM";
                sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}] |= OVmelding_DSI_{type}(ds{ov.FaseCyclus}_in, NG, NG, {_prmpf}{_prmallelijnen}{ov.FaseCyclus}, {ov.LijnNummers.Count});");
            }
            foreach (var ov in vecomovfcs)
            {
                string type = ov.Type == Models.Enumerations.OVIngreepVoertuigTypeEnum.Bus ? "BUS" : "TRAM";
                sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}] |= OVmelding_DSI_{type}(ds{ov.FaseCyclus}_uit, NG, NG, {_prmpf}{_prmallelijnen}{ov.FaseCyclus}, {ov.LijnNummers.Count});");
            }

            sb.AppendLine();
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }
        
        private string GenerateOvCPARCorrecties(ControllerModel c)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* -------------------------------------");
            sb.AppendLine("   OVPARCorrecties corrigeert de PAR van");
            sb.AppendLine("   gesynchroniseerde richtingen.");
            sb.AppendLine("   ------------------------------------- */");
            sb.AppendLine("void OVPARCorrecties(void)");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine("}");
            sb.AppendLine();
            return sb.ToString();
        }

        private string GenerateOvCPARCcol(ControllerModel c)
        {
            StringBuilder sb = new StringBuilder();

            string _tgb = CCOLGeneratorSettingsProvider.Default.GetElementName("tgb");
            string _tgbhd = CCOLGeneratorSettingsProvider.Default.GetElementName("tgbhd");
            string _trt = CCOLGeneratorSettingsProvider.Default.GetElementName("trt");
            string _trthd = CCOLGeneratorSettingsProvider.Default.GetElementName("trthd");
            string _hovin = CCOLGeneratorSettingsProvider.Default.GetElementName("hovin");
            string _cvc = CCOLGeneratorSettingsProvider.Default.GetElementName("cvc");
            string _tblk = CCOLGeneratorSettingsProvider.Default.GetElementName("tblk");
            string _hov = CCOLGeneratorSettingsProvider.Default.GetElementName("hov");

            sb.AppendLine($"/*-----------------------------------------------------");
            sb.AppendLine($"   OVCcol zorgt voor het bijwerken van de CCOL-elementen");
            sb.AppendLine($"   voor het OV.");
            sb.AppendLine($"   ----------------------------------------------------- */");
            sb.AppendLine($"void OVCcol(void) {{");
            foreach(var ov in c.OVData.OVIngrepen)
            {
                sb.AppendLine($"  OVCcolElementen(ovFC{ov.FaseCyclus}, {_tpf}{_tgb}{ov.FaseCyclus}, {_tpf}{_trt}{ov.FaseCyclus}, {_hpf}{_hov}{ov.FaseCyclus}, {_cpf}{_cvc}{ov.FaseCyclus}, {_tpf}{_tblk}{ov.FaseCyclus});");
            }
            //sb.AppendLine($"  OVCcolElementen(hdFC{ov.FaseCyclus}, tgbhd{ov.FaseCyclus}, trthd{ov.FaseCyclus}, hhd{ov.FaseCyclus}, cvchd{ov.FaseCyclus}, -1);");
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateOvCSpecialSignals(ControllerModel c)
        {
            StringBuilder sb = new StringBuilder();

            string _prmtestkarvert = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtestkarvert");
            string _prmtestkarlyn = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtestkarlyn");
            string _isdummykarin = CCOLGeneratorSettingsProvider.Default.GetElementName("isdummykarin");
            string _isdummykaruit = CCOLGeneratorSettingsProvider.Default.GetElementName("isdummykaruit");
            string _isdummyvecomin = CCOLGeneratorSettingsProvider.Default.GetElementName("isdummyvecomin");
            string _isdummyvecomuit = CCOLGeneratorSettingsProvider.Default.GetElementName("isdummyvecomuit");

            sb.AppendLine("/*----------------------------------------------------------------");
            sb.AppendLine("   OVSpecialSignals wordt aangeroepen vanuit de functie ");
            sb.AppendLine("   is_special_signals. Deze wordt in de testomgeving gebruikt voor ");
            sb.AppendLine("   het opzetten van bijzondere ingangen.");
            sb.AppendLine("   ---------------------------------------------------------------- */");
            sb.AppendLine("#ifdef CCOL_IS_SPECIAL");
            sb.AppendLine($"void OVSpecialSignals(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}/* reset oude set_DSI_message */");
            sb.AppendLine($"{ts}#ifndef VISSIM");
            sb.AppendLine($"{ts}{ts}reset_DSI_message();");
            sb.AppendLine($"{ts}#endif");
            sb.AppendLine();
            foreach (var ov in c.OVData.OVIngrepen.Where(x => x.KAR))
            {
                int ifc;
                if (Int32.TryParse(ov.FaseCyclus, out ifc))
                {
                    string type = ov.Type == Models.Enumerations.OVIngreepVoertuigTypeEnum.Bus ? "CIF_BUS" : "CIF_TRAM";
                    sb.AppendLine($"{ts}if (IS[{_ispf}{_isdummykarin}{ov.FaseCyclus}] && !IS_old[{_ispf}{_isdummykarin}{ov.FaseCyclus}]) set_DSI_message_KAR({type}, {ifc}, CIF_DSIN, 1, PRM[{_prmpf}{_prmtestkarvert}], PRM[{_prmpf}{_prmtestkarlyn}], 0);");
                    sb.AppendLine($"{ts}if (IS[{_ispf}{_isdummykaruit}{ov.FaseCyclus}] && !IS_old[{_ispf}{_isdummykaruit}{ov.FaseCyclus}]) set_DSI_message_KAR({type}, {ifc}, CIF_DSUIT, 1, PRM[{_prmpf}{_prmtestkarvert}], PRM[{_prmpf}{_prmtestkarlyn}], 0);");
                }
            }
            foreach (var ov in c.OVData.OVIngrepen.Where(x => x.Vecom))
            {
                string type = ov.Type == Models.Enumerations.OVIngreepVoertuigTypeEnum.Bus ? "CIF_BUS" : "CIF_TRAM";
                sb.AppendLine($"{ts}if (IS[{_ispf}{_isdummyvecomin}{ov.FaseCyclus}] && !IS_old[{_ispf}{_isdummyvecomin}{ov.FaseCyclus}]) set_DSI_message(ds{ov.FaseCyclus}_in, {type}, CIF_DSIN, PRM[{_prmpf}{_prmtestkarlyn}], NG);");
                sb.AppendLine($"{ts}if (IS[{_ispf}{_isdummyvecomuit}{ov.FaseCyclus}] && !IS_old[{_ispf}{_isdummyvecomuit}{ov.FaseCyclus}]) set_DSI_message(ds{ov.FaseCyclus}_uit, {type}, CIF_DSUIT, PRM[{_prmpf}{_prmtestkarlyn}], NG);");
            }
            sb.AppendLine("}");
            sb.AppendLine("#endif");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateOvCBottom(ControllerModel c)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"#include \"ov.c\"");
            sb.AppendLine($"#include \"{c.Data.Naam}ov.add\"");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}