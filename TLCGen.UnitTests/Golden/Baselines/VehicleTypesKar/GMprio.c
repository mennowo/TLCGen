/* PRIORITEIT FUNCTIES APPLICATIE */
/* ------------------------------ */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMprio.c
      CCOL:   10.0
    TLCGEN:   
   CCOLGEN:   UnitTest
*/

/****************************** Versie commentaar ***********************************
 *
 * Versie   Datum   Ontwerper   Commentaar
 *
 ************************************************************************************/

#define PRIO_ADDFILE

/*include files */
/*------------- */
    #include "GMsys.h"
    #include "stdfunc.h"  /* standaard functies                */
    #include "prio.h"     /* prio header                       */
    #include "fcvar.h"    /* fasecycli                         */
    #include "kfvar.h"    /* conflicten                        */
    #include "usvar.h"    /* uitgangs elementen                */
    #include "dpvar.h"    /* detectie elementen                */
    #include "isvar.h"    /* ingangs elementen                 */
    #include "hevar.h"    /* hulp elementen                    */
    #include "mevar.h"    /* geheugen elementen                */
    #include "tmvar.h"    /* tijd elementen                    */
    #include "ctvar.h"    /* teller elementen                  */
    #include "schvar.h"   /* software schakelaars              */
    #include "prmvar.h"   /* parameters                        */
    #include "lwmlvar.h"  /* langstwachtende modulen structuur */
    #include "control.h"  /* controller interface              */
    #include "rtappl.h"   /* applicatie routines               */
    #include "cif.inc"    /* interface                         */
    #if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)
        #include "xyprintf.h" /* Printen debuginfo                 */
        extern boolv display;
    #endif
    #include "ccolfunc.h"
    #include "ccol_mon.h"
    #include "extra_func.h"
    #include "extra_func_prio.h"

boolv vertraag_kar_uitm[prioFCMAX];


extern mulv DB_old[];
extern mulv TDH_old[];

#include "prio.c"

/* Variabele tbv start KAR ondergedrag timer bij starten regeling */
static char startkarog = FALSE;

/*-------------------------------------------------------------------------------------------
   OVInstellingen voorziet alle OV-instellingen van een juiste waarde.
   Het gaat om de volgende instellingen:
   - iFC_PRIOix[prio]                    : (de index van) de fasecyclus van de PRIO-richting prio.
   - iT_GBix[prio]                       : (de index van) de timer voor de groenbewaking
                                           van PRIO-richting prio.
   - iH_PRIOix[prio]                     : (de index van) het hulpelement voor de
                                           prioriteitstoekenning van PRIO-richting prio.
   - iInstPrioriteitsNiveau[prio]        : het prioriteitsniveau van de inmeldingen van de
                                           PRIO-richting prio.
   - iInstPrioriteitsOpties[prio]        : de prioriteisopties van de inmeldingen van de
                                           PRIO-richting prio.
   - iGroenBewakingsTijd[prio]           : De groenbewakingstijd van de PRIO-richting prio.
   - iRTSOngehinderd[prio]               : De ongehinderde rijtijd van PRIO-richting prio.
   - iRTSBeperktGehinderd[prio]          : De beperkt gehinderde rijtijd van PRIO-richting prio.
   - iRTSGehinderd[prio]                 : De gehinderde rijtijd van PRIO-richting prio.
   - iOnderMaximum[prio]                 : Het ondermaximum van PRIO-richting prio.
   - iSelDetFoutNaGB[prio]               : De in- en uitmelddetectie van PRIO-richting prio
                                           wordt als defect beschouwd bij het aanspreken
                                           van de groenbewaking. De defectmelding wordt
                                           opgeheven bij de eerstvolgende uitmelding.
   - iMaximumWachtTijd[fc]               : de maximum wachttijd voor richting fc waarbprioen
                                           geen prioriteit meer wordt toegekend voor
                                           niet-nooddienst-inmeldingen op de PRIO-richtingen.
   - iInstPercMaxGroenTijdTerugKomen[fc] : percentage van de maximumgroentijd van richting fc
                                           waaronder na afkappen de richting terugkomt.
   - iInstMinTerugKomGroenTijd[fc]       : minimale maximumgroentijd van richting fc bij
                                           terugkomen.
   - iInstAantalMalenNietAfkappen[fc]    : aantal malen niet afkappen van richting fc na te
                                           zijn afgekapt, tenzij inmelding nooddienst.
   - iAfkapGroenTijd[fc]                 : groentijd waaronder richting fc niet mag worden
                                           afgekapt, tenzij inmelding nooddienst.
   - iPercGroenTijd[fc]                  : percentage van de maximumgroentijd dat richting fc
                                           moet hebben gemaakt, voordat deze mag worden
                                           afgekapt.
   - iInstOphoogPercentageMG[fc]         : het percentage van de maximumgroentijd dat een
                                           richting minimaal mag maken alvorens te worden
                                           afgekapt, tenzij er sprake is van een nooddienst,
                                           wordt telkens bij afkappen verhoogd met dit
                                           ophoogpercentage maximumgroentijd totdat 100% is
                                           gehaald (daarna reset).
   - iPRM_ALTP[fc]                       : ruimte die minimaal aanwezig moet zijn voor
                                           richting fc, om alternatief te realiseren tijdens
                                           niet-konflikterende PRIO-ingre(e)p(en).
   - iSCH_ALTG[fc]                       : richting fc mag alternatief realiseren tijdens
                                           niet-konflikterende PRIO-ingre(e)p(en).
   - iInstAfkapGroenAlt[fc]              : groentijd die richting fc minimaal mag maken
                                           bij een alternatieve realisatie, voordat deze
                                           mag worden afgekapt.
   - iLangstWachtendeAlternatief         : de regeling maakt gebruik van
                                           langstwachtende alternatief
   ------------------------------------------------------------------------------------------- */

void PrioInitExtra(void) 
{

    int i;

    /* initialisatie variabelen vertraag_kar_uitm */
    /* ------------------------------------------ */
    for (i = 0; i < prioFCMAX; ++i)
    {
        vertraag_kar_uitm[i] = 0;
    }

}

void PrioInstellingen(void) 
{
    /* ======================= */
    /* Instellingen PRIORITEIT */
    /* ======================= */


    /* Fasecyclus voor richtingen met PRIO */
    iFC_PRIOix[prioFC41tram] = fc41;
    iFC_PRIOix[prioFC41bus] = fc41;
    iFC_PRIOix[prioFC42bus] = fc42;
    iFC_PRIOix[prioFC22fiets] = fc22;
    iFC_PRIOix[prioFC02vrw] = fc02;
    iFC_PRIOix[prioFC05auto] = fc05;
    iFC_PRIOix[prioFC05ng] = fc05;
    iFC_PRIOix[prioFC61hd] = fc61;

    /* Index van de groenbewakingstimer */
    iT_GBix[prioFC41tram] = tgb41tram;
    iT_GBix[prioFC41bus] = tgb41bus;
    iT_GBix[prioFC42bus] = tgb42bus;
    iT_GBix[prioFC22fiets] = tgb22fiets;
    iT_GBix[prioFC02vrw] = tgb02vrw;
    iT_GBix[prioFC05auto] = tgb05auto;
    iT_GBix[prioFC05ng] = tgb05ng;
    iT_GBix[prioFC61hd] = tgb61hd;

    /* Index van het hulpelement voor de ingreep */
    iH_PRIOix[prioFC41tram] = hprio41tram;
    iH_PRIOix[prioFC41bus] = hprio41bus;
    iH_PRIOix[prioFC42bus] = hprio42bus;
    iH_PRIOix[prioFC22fiets] = hprio22fiets;
    iH_PRIOix[prioFC02vrw] = hprio02vrw;
    iH_PRIOix[prioFC05auto] = hprio05auto;
    iH_PRIOix[prioFC05ng] = hprio05ng;
    iH_PRIOix[prioFC61hd] = hprio61hd;

    /* Prioriteitsniveau */
    iInstPrioriteitsNiveau[prioFC41tram] = PRM[prmprio41tram]/1000L;
    iInstPrioriteitsNiveau[prioFC41bus] = PRM[prmprio41bus]/1000L;
    iInstPrioriteitsNiveau[prioFC42bus] = PRM[prmprio42bus]/1000L;
    iInstPrioriteitsNiveau[prioFC22fiets] = PRM[prmprio22fiets]/1000L;
    iInstPrioriteitsNiveau[prioFC02vrw] = PRM[prmprio02vrw]/1000L;
    iInstPrioriteitsNiveau[prioFC05auto] = PRM[prmprio05auto]/1000L;
    iInstPrioriteitsNiveau[prioFC05ng] = PRM[prmprio05ng]/1000L;
    iInstPrioriteitsNiveau[prioFC61hd] = PRM[prmprio61hd]/1000L;

    /* Prioriteitsopties */
    iInstPrioriteitsOpties[prioFC41tram] = BepaalPrioriteitsOpties(prmprio41tram);
    iInstPrioriteitsOpties[prioFC41bus] = BepaalPrioriteitsOpties(prmprio41bus);
    iInstPrioriteitsOpties[prioFC42bus] = BepaalPrioriteitsOpties(prmprio42bus);
    iInstPrioriteitsOpties[prioFC22fiets] = BepaalPrioriteitsOpties(prmprio22fiets);
    iInstPrioriteitsOpties[prioFC02vrw] = BepaalPrioriteitsOpties(prmprio02vrw);
    iInstPrioriteitsOpties[prioFC05auto] = BepaalPrioriteitsOpties(prmprio05auto);
    iInstPrioriteitsOpties[prioFC05ng] = BepaalPrioriteitsOpties(prmprio05ng);
    iInstPrioriteitsOpties[prioFC61hd] = BepaalPrioriteitsOpties(prmprio61hd);

    /* Groenbewakingstijd */
    iGroenBewakingsTijd[prioFC41tram] = T_max[tgb41tram];
    iGroenBewakingsTijd[prioFC41bus] = T_max[tgb41bus];
    iGroenBewakingsTijd[prioFC42bus] = T_max[tgb42bus];
    iGroenBewakingsTijd[prioFC22fiets] = T_max[tgb22fiets];
    iGroenBewakingsTijd[prioFC02vrw] = T_max[tgb02vrw];
    iGroenBewakingsTijd[prioFC05auto] = T_max[tgb05auto];
    iGroenBewakingsTijd[prioFC05ng] = T_max[tgb05ng];
    iGroenBewakingsTijd[prioFC61hd] = T_max[tgb61hd];

    /* Ongehinderde rijtijd */
    iRTSOngehinderd[prioFC41tram] = PRM[prmrto41tram];
    iRTSOngehinderd[prioFC41bus] = PRM[prmrto41bus];
    iRTSOngehinderd[prioFC42bus] = PRM[prmrto42bus];
    iRTSOngehinderd[prioFC22fiets] = PRM[prmrto22fiets];
    iRTSOngehinderd[prioFC02vrw] = PRM[prmrto02vrw];
    iRTSOngehinderd[prioFC05auto] = PRM[prmrto05auto];
    iRTSOngehinderd[prioFC05ng] = PRM[prmrto05ng];
    iRTSOngehinderd[prioFC61hd] = PRM[prmrto61hd];

    /* Beperkt gehinderde rijtijd */
    iRTSBeperktGehinderd[prioFC41tram] = PRM[prmrtbg41tram];
    iRTSBeperktGehinderd[prioFC41bus] = PRM[prmrtbg41bus];
    iRTSBeperktGehinderd[prioFC42bus] = PRM[prmrtbg42bus];
    iRTSBeperktGehinderd[prioFC22fiets] = PRM[prmrtbg22fiets];
    iRTSBeperktGehinderd[prioFC02vrw] = PRM[prmrtbg02vrw];
    iRTSBeperktGehinderd[prioFC05auto] = PRM[prmrtbg05auto];
    iRTSBeperktGehinderd[prioFC05ng] = PRM[prmrtbg05ng];
    iRTSBeperktGehinderd[prioFC61hd] = PRM[prmrtbg61hd];

    /* Gehinderde rijtijd */
    iRTSGehinderd[prioFC41tram] = PRM[prmrtg41tram];
    iRTSGehinderd[prioFC41bus] = PRM[prmrtg41bus];
    iRTSGehinderd[prioFC42bus] = PRM[prmrtg42bus];
    iRTSGehinderd[prioFC22fiets] = PRM[prmrtg22fiets];
    iRTSGehinderd[prioFC02vrw] = PRM[prmrtg02vrw];
    iRTSGehinderd[prioFC05auto] = PRM[prmrtg05auto];
    iRTSGehinderd[prioFC05ng] = PRM[prmrtg05ng];
    iRTSGehinderd[prioFC61hd] = PRM[prmrtg61hd];

    /* Ondermaximum */
    iOnderMaximum[prioFC41tram] = PRM[prmomx41tram];
    iOnderMaximum[prioFC41bus] = PRM[prmomx41bus];
    iOnderMaximum[prioFC42bus] = PRM[prmomx42bus];
    iOnderMaximum[prioFC22fiets] = PRM[prmomx22fiets];
    iOnderMaximum[prioFC02vrw] = PRM[prmomx02vrw];
    iOnderMaximum[prioFC05auto] = PRM[prmomx05auto];
    iOnderMaximum[prioFC05ng] = PRM[prmomx05ng];
    iOnderMaximum[prioFC61hd] = PRM[prmomx61hd];

    /* Blokkeringstijd */
    iBlokkeringsTijd[prioFC41tram] = T_max[tblk41tram];
    iBlokkeringsTijd[prioFC41bus] = T_max[tblk41bus];
    iBlokkeringsTijd[prioFC42bus] = T_max[tblk42bus];
    iBlokkeringsTijd[prioFC22fiets] = T_max[tblk22fiets];
    iBlokkeringsTijd[prioFC02vrw] = T_max[tblk02vrw];
    iBlokkeringsTijd[prioFC05auto] = T_max[tblk05auto];
    iBlokkeringsTijd[prioFC05ng] = T_max[tblk05ng];
    iBlokkeringsTijd[prioFC61hd] = T_max[tblk61hd];

    /* Na aanspreken groenbewaking wordt de selectieve 
	   detectie niet langer betrouwbaar gevonden */
    iSelDetFoutNaGB[prioFC41tram] = PRM[prmupinagb41tram];
    iSelDetFoutNaGB[prioFC41bus] = PRM[prmupinagb41bus];
    iSelDetFoutNaGB[prioFC42bus] = PRM[prmupinagb42bus];
    iSelDetFoutNaGB[prioFC22fiets] = PRM[prmupinagb22fiets];
    iSelDetFoutNaGB[prioFC02vrw] = PRM[prmupinagb02vrw];
    iSelDetFoutNaGB[prioFC05auto] = PRM[prmupinagb05auto];
    iSelDetFoutNaGB[prioFC05ng] = PRM[prmupinagb05ng];
    iSelDetFoutNaGB[prioFC61hd] = PRM[prmupinagb61hd];

    /* =============================== */
    /* Instellingen overige richtingen */
    /* =============================== */
   
    /* Maximale wachttijd */
    iMaximumWachtTijd[fc02] = PRM[prmmwta];
    iMaximumWachtTijd[fc05] = PRM[prmmwta];
    iMaximumWachtTijd[fc22] = PRM[prmmwtfts];
    iMaximumWachtTijd[fc41] = PRM[prmmwta];
    iMaximumWachtTijd[fc42] = PRM[prmmwta];
    iMaximumWachtTijd[fc61] = PRM[prmmwta];

    /* Als een richting minder groen heeft gehad door afkappen
       dan deze instelling, dan mag de richting nog een keer
       primair realiseren (terugkomen). */
    iInstPercMaxGroenTijdTerugKomen[fc02] = PRM[prmpmgt02];
    iInstPercMaxGroenTijdTerugKomen[fc05] = PRM[prmpmgt05];
    iInstPercMaxGroenTijdTerugKomen[fc22] = PRM[prmpmgt22];
    iInstPercMaxGroenTijdTerugKomen[fc41] = PRM[prmpmgt41];
    iInstPercMaxGroenTijdTerugKomen[fc42] = PRM[prmpmgt42];
    iInstPercMaxGroenTijdTerugKomen[fc61] = PRM[prmpmgt61];

    /* De minimale groentijd die een richting krijgt als
       deze mag terugkomen. */
    iInstMinTerugKomGroenTijd[fc02] = PRM[prmognt02];
    iInstMinTerugKomGroenTijd[fc05] = PRM[prmognt05];
    iInstMinTerugKomGroenTijd[fc22] = PRM[prmognt22];
    iInstMinTerugKomGroenTijd[fc41] = PRM[prmognt41];
    iInstMinTerugKomGroenTijd[fc42] = PRM[prmognt42];
    iInstMinTerugKomGroenTijd[fc61] = PRM[prmognt61];

    /* Aantal malen niet afkappen */
    iInstAantalMalenNietAfkappen[fc02] = PRM[prmnofm02];
    iInstAantalMalenNietAfkappen[fc05] = PRM[prmnofm05];
    iInstAantalMalenNietAfkappen[fc22] = PRM[prmnofm22];
    iInstAantalMalenNietAfkappen[fc41] = PRM[prmnofm41];
    iInstAantalMalenNietAfkappen[fc42] = PRM[prmnofm42];
    iInstAantalMalenNietAfkappen[fc61] = PRM[prmnofm61];

    /* Onder deze groentijd mag een richting niet worden
       afgekapt, tenzij zich een nooddienst heeft ingemeld. */
    iAfkapGroenTijd[fc02] = PRM[prmmgcov02];
    iAfkapGroenTijd[fc05] = PRM[prmmgcov05];
    iAfkapGroenTijd[fc22] = PRM[prmmgcov22];
    iAfkapGroenTijd[fc41] = PRM[prmmgcov41];
    iAfkapGroenTijd[fc42] = PRM[prmmgcov42];
    iAfkapGroenTijd[fc61] = PRM[prmmgcov61];

    /* Als een richting minder groen heeft gehad dan
       dit percentage van de maximum groentijd, dan
       mag de richting niet worden afgekapt, tenzij
       zich een nooddienst heeft ingemeld. */
    iPercGroenTijd[fc02] = PRM[prmpmgcov02];
    iPercGroenTijd[fc05] = PRM[prmpmgcov05];
    iPercGroenTijd[fc22] = PRM[prmpmgcov22];
    iPercGroenTijd[fc41] = PRM[prmpmgcov41];
    iPercGroenTijd[fc42] = PRM[prmpmgcov42];
    iPercGroenTijd[fc61] = PRM[prmpmgcov61];

    /* Na te zijn afgekapt, wordt het percentage
       van de maximumgroentijd verhoogd met dit ophoogpercentage. */
    iInstOphoogPercentageMG[fc02] = PRM[prmohpmg02];
    iInstOphoogPercentageMG[fc05] = PRM[prmohpmg05];
    iInstOphoogPercentageMG[fc22] = PRM[prmohpmg22];
    iInstOphoogPercentageMG[fc41] = PRM[prmohpmg41];
    iInstOphoogPercentageMG[fc42] = PRM[prmohpmg42];
    iInstOphoogPercentageMG[fc61] = PRM[prmohpmg61];

    /* Benodigde ruimte voor alternatieve realisatie tijdens een OV ingreep */
    iPRM_ALTP[fc02] = PRM[prmaltp02];
    iPRM_ALTP[fc05] = PRM[prmaltp05];
    iPRM_ALTP[fc22] = PRM[prmaltp22];
    iPRM_ALTP[fc41] = PRM[prmaltp41];
    iPRM_ALTP[fc42] = PRM[prmaltp42];
    iPRM_ALTP[fc61] = PRM[prmaltp61];

    /* Richting mag alternatief realiseren tijdens een OV ingreep */
    iSCH_ALTG[fc02] = SCH[schaltg02];
    iSCH_ALTG[fc05] = SCH[schaltg05];
    iSCH_ALTG[fc22] = SCH[schaltg22];
    iSCH_ALTG[fc41] = SCH[schaltg41];
    iSCH_ALTG[fc42] = SCH[schaltg42];
    iSCH_ALTG[fc61] = SCH[schaltg61];

    /* Alternatieve realisatie mag worden
       afgekapt tijdens een OV ingreep als
       deze groentijd is gemaakt */
    iInstAfkapGroenAlt[fc02] = PRM[prmaltg02];
    iInstAfkapGroenAlt[fc05] = PRM[prmaltg05];
    iInstAfkapGroenAlt[fc22] = PRM[prmaltg22];
    iInstAfkapGroenAlt[fc41] = PRM[prmaltg41];
    iInstAfkapGroenAlt[fc42] = PRM[prmaltg42];
    iInstAfkapGroenAlt[fc61] = PRM[prmaltg61];

    /* definitie van de meerealisaties voor de hulpdiensten */

    /* De regeling maakt gebruik van langstwachtende alternatief */
    iLangstWachtendeAlternatief = 1;

}

/* -----------------------------------------------------------
   RijTijdScenario bepaalt het actieve rijtijdscenario
   van alle OV-richtingen. Het resultaat wordt geplaatst
   in de variabele iRijTijdScenario, en kan als waarde
   rtsOngehinderd, rtsBeperktGehinderd of rtsGehinderd hebben.
   Het rijtijdscenario is bepalend voor de gehanteerde
   rijtijd.
   ----------------------------------------------------------- */
void RijTijdScenario(void)
{
    /* Vaststellen rijtijdscenarios */
    PrioRijTijdScenario(prioFC41tram, d411, d412, tbtovg41tram);

    PrioRijTijdScenario(prioFC41bus, d411, d412, tbtovg41bus);

    PrioRijTijdScenario(prioFC42bus, d421, d422, tbtovg42bus);

    PrioRijTijdScenario(prioFC22fiets, d221, NG, NG);

    PrioRijTijdScenario(prioFC02vrw, d021, d022, tbtovg02vrw);

    PrioRijTijdScenario(prioFC05auto, d051, d052, tbtovg05auto);

    PrioRijTijdScenario(prioFC05ng, d051, d052, tbtovg05ng);

    PrioRijTijdScenario(prioFC61hd, d611, d612, tbtovg61hd);




}

/*----------------------------------------------------------------
   InUitMelden verzorgt het afhandelen van in- en uitmeldingen.
   Voor het in- en uitmelden zijn hulpelementen gedefinieerd.
   Bij het opkomen van het hulpelement voor de inmelding wordt
   een inmelding gedaan voor OV-richting ov met prioriteitsniveau
   iInstPrioriteitsNiveau[ov] en prioriteitsopties
   iInstPrioriteitsOpties[ov]. Bij het opkomen van het hulpelement
   voor de uitmelding wordt de eerste (oudste) inmelding uitgemeld.
   De hulpelementen voor in- en uitmeldingen worden opgezet als
   de bijbehorende in- resp. uitmeldlussen opstaan.
   ---------------------------------------------------------------- */
void InUitMelden(void)
{
    /* Prioriteit-inmeldingen */
    PrioInmelden(prioFC41tram, SH[hprioin41tram], iInstPrioriteitsNiveau[prioFC41tram], iInstPrioriteitsOpties[prioFC41tram], 0, 0);
    PrioInmelden(prioFC41bus, SH[hprioin41bus], iInstPrioriteitsNiveau[prioFC41bus], iInstPrioriteitsOpties[prioFC41bus], 0, 0);
    PrioInmelden(prioFC42bus, SH[hprioin42bus], iInstPrioriteitsNiveau[prioFC42bus], iInstPrioriteitsOpties[prioFC42bus], 0, 0);
    PrioInmelden(prioFC22fiets, SH[hprioin22fiets], iInstPrioriteitsNiveau[prioFC22fiets], iInstPrioriteitsOpties[prioFC22fiets], 0, 0);
    PrioInmelden(prioFC02vrw, SH[hprioin02vrw], iInstPrioriteitsNiveau[prioFC02vrw], iInstPrioriteitsOpties[prioFC02vrw], 0, 0);
    PrioInmelden(prioFC05auto, SH[hprioin05auto], iInstPrioriteitsNiveau[prioFC05auto], iInstPrioriteitsOpties[prioFC05auto], 0, 0);
    PrioInmelden(prioFC05ng, SH[hprioin05ng], iInstPrioriteitsNiveau[prioFC05ng], iInstPrioriteitsOpties[prioFC05ng], 0, 0);
    PrioInmelden(prioFC61hd, SH[hprioin61hd], iInstPrioriteitsNiveau[prioFC61hd], iInstPrioriteitsOpties[prioFC61hd], 0, 0);

    /* Prioriteit-uitmeldingen */
    PrioUitmelden(prioFC41tram, SH[hpriouit41tram]);
    PrioUitmelden(prioFC41bus, SH[hpriouit41bus]);
    PrioUitmelden(prioFC42bus, SH[hpriouit42bus]);
    PrioUitmelden(prioFC22fiets, SH[hpriouit22fiets]);
    PrioUitmelden(prioFC02vrw, SH[hpriouit02vrw]);
    PrioUitmelden(prioFC05auto, SH[hpriouit05auto]);
    PrioUitmelden(prioFC05ng, SH[hpriouit05ng]);
    PrioUitmelden(prioFC61hd, SH[hpriouit61hd]);



    /* Inmelding fc41 type Tram */
    IH[hprioin41tram] = IH[hprioin41tramkar] = FALSE;
    if (SCH[schprioin41tramkar])
    {
        IH[hprioin41tramkar] = 
            DSIMeldingPRIO_V1(0, PRM[prmvtgcat41tram], TRUE, 41,TRUE, CIF_DSIN, TRUE);
    }
    IH[hprioin41tram] = IH[hprioin41tramkar];

    /* Uitmelding fc41 type Tram */
    IH[hpriouit41tram] = IH[hpriouit41tramkar] = FALSE;
    if (SCH[schpriouit41tramkar])
    {
        IH[hpriouit41tramkar] = 
            DSIMeldingPRIO_V2(fc41, prioFC41tram, 0, PRM[prmvtgcat41tram], TRUE, 41,TRUE, CIF_DSUIT, TRUE);
    }
    IH[hpriouit41tram] = IH[hpriouit41tramkar];

    /* Inmelding fc41 type Bus */
    IH[hprioin41bus] = IH[hprioin41buskar] = FALSE;
    if (SCH[schprioin41buskar])
    {
        IH[hprioin41buskar] = 
            DSIMeldingPRIO_V1(0, PRM[prmvtgcat41bus], TRUE, 41,TRUE, CIF_DSIN, TRUE);
    }
    IH[hprioin41bus] = IH[hprioin41buskar];

    /* Uitmelding fc41 type Bus */
    IH[hpriouit41bus] = IH[hpriouit41buskar] = FALSE;
    if (SCH[schpriouit41buskar])
    {
        IH[hpriouit41buskar] = 
            DSIMeldingPRIO_V2(fc41, prioFC41bus, 0, PRM[prmvtgcat41bus], TRUE, 41,TRUE, CIF_DSUIT, TRUE);
    }
    IH[hpriouit41bus] = IH[hpriouit41buskar];

    /* Inmelding fc42 type Bus */
    IH[hprioin42bus] = IH[hprioin42buskar] = FALSE;
    if (SCH[schprioin42buskar])
    {
        IH[hprioin42buskar] = 
            DSIMeldingPRIO_V1(0, PRM[prmvtgcat42bus], TRUE, 42,TRUE, CIF_DSIN, TRUE);
    }
    IH[hprioin42bus] = IH[hprioin42buskar];

    /* Uitmelding fc42 type Bus */
    IH[hpriouit42bus] = IH[hpriouit42buskar] = FALSE;
    if (SCH[schpriouit42buskar])
    {
        IH[hpriouit42buskar] = 
            DSIMeldingPRIO_V2(fc42, prioFC42bus, 0, PRM[prmvtgcat42bus], TRUE, 42,TRUE, CIF_DSUIT, TRUE);
    }
    IH[hpriouit42bus] = IH[hpriouit42buskar];

    /* Inmelding fc22 type Fiets */
    IH[hprioin22fiets] = IH[hprioin22fietskar] = FALSE;
    if (SCH[schprioin22fietskar])
    {
        IH[hprioin22fietskar] = 
            DSIMeldingPRIO_V1(0, PRM[prmvtgcat22fiets], TRUE, 22,TRUE, CIF_DSIN, TRUE);
    }
    IH[hprioin22fiets] = IH[hprioin22fietskar];

    /* Uitmelding fc22 type Fiets */
    IH[hpriouit22fiets] = IH[hpriouit22fietskar] = FALSE;
    if (SCH[schpriouit22fietskar])
    {
        IH[hpriouit22fietskar] = 
            DSIMeldingPRIO_V2(fc22, prioFC22fiets, 0, PRM[prmvtgcat22fiets], TRUE, 22,TRUE, CIF_DSUIT, TRUE);
    }
    IH[hpriouit22fiets] = IH[hpriouit22fietskar];

    /* Inmelding fc02 type Vrachtwagen */
    IH[hprioin02vrw] = IH[hprioin02vrwkar] = FALSE;
    if (SCH[schprioin02vrwkar])
    {
        IH[hprioin02vrwkar] = 
            DSIMeldingPRIO_V1(0, PRM[prmvtgcat02vrw], TRUE, 2,TRUE, CIF_DSIN, TRUE);
    }
    IH[hprioin02vrw] = IH[hprioin02vrwkar];

    /* Uitmelding fc02 type Vrachtwagen */
    IH[hpriouit02vrw] = IH[hpriouit02vrwkar] = FALSE;
    if (SCH[schpriouit02vrwkar])
    {
        IH[hpriouit02vrwkar] = 
            DSIMeldingPRIO_V2(fc02, prioFC02vrw, 0, PRM[prmvtgcat02vrw], TRUE, 2,TRUE, CIF_DSUIT, TRUE);
    }
    IH[hpriouit02vrw] = IH[hpriouit02vrwkar];

    /* Inmelding fc05 type Auto */
    IH[hprioin05auto] = IH[hprioin05autokar] = FALSE;
    if (SCH[schprioin05autokar])
    {
        IH[hprioin05autokar] = 
            DSIMeldingPRIO_V1(0, PRM[prmvtgcat05auto], TRUE, 5,TRUE, CIF_DSIN, TRUE);
    }
    IH[hprioin05auto] = IH[hprioin05autokar];

    /* Uitmelding fc05 type Auto */
    IH[hpriouit05auto] = IH[hpriouit05autokar] = FALSE;
    if (SCH[schpriouit05autokar])
    {
        IH[hpriouit05autokar] = 
            DSIMeldingPRIO_V2(fc05, prioFC05auto, 0, PRM[prmvtgcat05auto], TRUE, 5,TRUE, CIF_DSUIT, TRUE);
    }
    IH[hpriouit05auto] = IH[hpriouit05autokar];

    /* Inmelding fc05 type NG */
    IH[hprioin05ng] = IH[hprioin05ngkar] = FALSE;
    if (SCH[schprioin05ngkar])
    {
        IH[hprioin05ngkar] = 
            DSIMeldingPRIO_V1(0, PRM[prmvtgcat05ng], TRUE, 5,TRUE, CIF_DSIN, TRUE);
    }
    IH[hprioin05ng] = IH[hprioin05ngkar];

    /* Uitmelding fc05 type NG */
    IH[hpriouit05ng] = IH[hpriouit05ngkar] = FALSE;
    if (SCH[schpriouit05ngkar])
    {
        IH[hpriouit05ngkar] = 
            DSIMeldingPRIO_V2(fc05, prioFC05ng, 0, PRM[prmvtgcat05ng], TRUE, 5,TRUE, CIF_DSUIT, TRUE);
    }
    IH[hpriouit05ng] = IH[hpriouit05ngkar];

    /* Inmelding fc61 type Nood- en hulpdienst */
    IH[hprioin61hd] = IH[hprioin61hdkar] = FALSE;
    if (SCH[schprioin61hdkar])
    {
        IH[hprioin61hdkar] = 
            DSIMelding_HD_V1(61, CIF_DSIN, (!SCH[schchecksirene61hd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR));
    }
    IH[hprioin61hd] = IH[hprioin61hdkar];

    /* Uitmelding fc61 type Nood- en hulpdienst */
    IH[hpriouit61hd] = IH[hpriouit61hdkar] = FALSE;
    if (SCH[schpriouit61hdkar])
    {
        IH[hpriouit61hdkar] = 
            DSIMelding_HD_V1(61, CIF_DSUIT, (!SCH[schchecksirene61hd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR));
    }
    IH[hpriouit61hd] = IH[hpriouit61hdkar];

    /* herstarten FB_timer bij in- of uitmelding HD of einde ingreep (door groenbewaking) */
    RTFB &= ~PRIO_RTFB_BIT;
    if (IH[hprioin61hd]|| IH[hpriouit61hd] || EH[hprio61hd])
    {
        RTFB |= PRIO_RTFB_BIT;
    }

    /* Bijhouden melding en ondergedrag KAR */
    RT[tkarmelding] = CIF_DSIWIJZ != 0 && CIF_DSI[CIF_DSI_LUS] == 0;
    RT[tkarog] = T[tkarmelding] || !startkarog;
    if (!startkarog) startkarog = TRUE;
#if defined CCOL_IS_SPECIAL && defined PRACTICE_TEST && !defined NO_PRIO
    is_special_signals();
#endif
}

void OnderMaximumExtra(void)
{
}
void AfkapGroenExtra(void)
{
}
void StartGroenMomentenExtra(void)
{
}
void PrioAfkappenExtra(void)
{
}
void PrioTerugkomGroenExtra(void)
{
}
void PrioGroenVasthoudenExtra(void)
{
}
void PrioMeetKriteriumExtra(void)
{
}
/* ------------------------------------
   Prioriteitsopties voor PRIO ingrepen
   ------------------------------------ */
void PrioriteitsOpties(void)
{
    #ifdef PRIO_ADDFILE
        PrioriteitsOpties_Add();
    #endif

    #ifdef PRIO_ADDFILE
        PrioriteitsNiveau_Add();
    #endif
}
/* ------------------------------------
   Prioriteitsopties toekenning voor OV
   ------------------------------------ */
void PrioriteitsToekenningExtra(void)
{
}
/* ------------------------------------
   Extra code tegenhouden conflicten OV
   ------------------------------------ */
void TegenhoudenConflictenExtra(void)
{
}

/* ---------------------------
   Post afhandeling prioriteit
   --------------------------- */
void PostAfhandelingPrio(void)
{
}
/* ---------------------------------------
   PrioPARCorrecties corrigeert de PAR van
   gesynchroniseerde richtingen.
   --------------------------------------- */
void PrioPARCorrecties(void)
{
}

/* -------------------------------------------------------
   PrioCcol zorgt voor het bijwerken van de CCOL-elementen
   voor het richtingen met prioriteit.
   ------------------------------------------------------- */
void PrioCcol(void) {
    PrioCcolElementen(prioFC41tram, tgb41tram, trt41tram, hprio41tram, cvc41tram, tblk41tram);
    PrioCcolElementen(prioFC41bus, tgb41bus, trt41bus, hprio41bus, cvc41bus, tblk41bus);
    PrioCcolElementen(prioFC42bus, tgb42bus, trt42bus, hprio42bus, cvc42bus, tblk42bus);
    PrioCcolElementen(prioFC22fiets, tgb22fiets, trt22fiets, hprio22fiets, cvc22fiets, tblk22fiets);
    PrioCcolElementen(prioFC02vrw, tgb02vrw, trt02vrw, hprio02vrw, cvc02vrw, tblk02vrw);
    PrioCcolElementen(prioFC05auto, tgb05auto, trt05auto, hprio05auto, cvc05auto, tblk05auto);
    PrioCcolElementen(prioFC05ng, tgb05ng, trt05ng, hprio05ng, cvc05ng, tblk05ng);
    PrioCcolElementen(prioFC61hd, tgb61hd, trt61hd, hprio61hd, cvc61hd, tblk61hd);
}

/* ----------------------------------------------------------------
   PrioSpecialSignals wordt aangeroepen vanuit de functie 
   is_special_signals. Deze wordt in de testomgeving gebruikt voor 
   het opzetten van bijzondere ingangen.
   ---------------------------------------------------------------- */
#if defined CCOL_IS_SPECIAL && !defined NO_PRIO
void PrioSpecialSignals(void)
{
    /* reset oude set_DSI_message */
    #if !defined VISSIM_GLOBAL_DSI
        reset_DSI_message();
    #endif

    /* Prioriteit ingrepen */
    if (SD[ddummykarin41tram]) set_DSI_message(NG, CIF_TRAM, 41, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykaruit41tram] && C[cvc41tram]) set_DSI_message(NG, CIF_TRAM, 41, CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykarin41bus]) set_DSI_message(NG, CIF_BUS, 41, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykaruit41bus] && C[cvc41bus]) set_DSI_message(NG, CIF_BUS, 41, CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykarin42bus]) set_DSI_message(NG, CIF_BUS, 42, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykaruit42bus] && C[cvc42bus]) set_DSI_message(NG, CIF_BUS, 42, CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykarin22fiets]) set_DSI_message(NG, CIF_BUS, 22, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykaruit22fiets] && C[cvc22fiets]) set_DSI_message(NG, CIF_BUS, 22, CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykarin02vrw]) set_DSI_message(NG, CIF_BUS, 2, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykaruit02vrw] && C[cvc02vrw]) set_DSI_message(NG, CIF_BUS, 2, CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykarin05auto]) set_DSI_message(NG, CIF_BUS, 5, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykaruit05auto] && C[cvc05auto]) set_DSI_message(NG, CIF_BUS, 5, CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykarin05ng]) set_DSI_message(NG, CIF_BUS, 5, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykaruit05ng] && C[cvc05ng]) set_DSI_message(NG, CIF_BUS, 5, CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykarin61hd]) set_DSI_message(NG, CIF_POL, 61, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], CIF_SIR);
    if (SD[ddummykaruit61hd] && C[cvc61hd]) set_DSI_message(NG, CIF_POL, 61, CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], CIF_SIR);

}
#endif

#ifdef PRIO_ADDFILE
    #include "GMprio.add"
#endif /* PRIO_ADDFILE */
