/* PRIORITEIT FUNCTIES APPLICATIE */
/* ------------------------------ */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMprio.c
      CCOL:   11.0
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
    #ifndef NO_RIS
        #include "risappl.h"   /* RIS routines                     */
        #if (CCOL_V > 100)
        #include "extra_func_ris.h"   /* RIS routines              */
        #endif
    #endif /* NO_RIS */
    #include "cif.inc"    /* interface                         */
    #if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)
        #include "xyprintf.h" /* Printen debuginfo                 */
    #include "rissimvar.h"   /* RIS routines                   */
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

    #ifndef NO_RIS
    /* initialisatie variabelen granted_verstrekt */
    /* ------------------------------------------ */
    for (i = 0; i < FCMAX; ++i)
    {
        granted_verstrekt[i] = 0;
    }
    #endif /* NO_RIS */

}

void PrioInstellingen(void) 
{
    /* ======================= */
    /* Instellingen PRIORITEIT */
    /* ======================= */


    /* Fasecyclus voor richtingen met PRIO */
    iFC_PRIOix[prioFC41bus] = fc41;
    iFC_PRIOix[prioFC41tram] = fc41;
    iFC_PRIOix[prioFC42] = fc42;
    iFC_PRIOix[prioFC22] = fc22;

    /* Index van de groenbewakingstimer */
    iT_GBix[prioFC41bus] = tgb41bus;
    iT_GBix[prioFC41tram] = tgb41tram;
    iT_GBix[prioFC42] = tgb42;
    iT_GBix[prioFC22] = tgb22;

    /* Index van het hulpelement voor de ingreep */
    iH_PRIOix[prioFC41bus] = hprio41bus;
    iH_PRIOix[prioFC41tram] = hprio41tram;
    iH_PRIOix[prioFC42] = hprio42;
    iH_PRIOix[prioFC22] = hprio22;

    /* Prioriteitsniveau */
    iInstPrioriteitsNiveau[prioFC41bus] = PRM[prmprio41bus]/1000L;
    iInstPrioriteitsNiveau[prioFC41tram] = PRM[prmprio41tram]/1000L;
    iInstPrioriteitsNiveau[prioFC42] = PRM[prmprio42]/1000L;
    iInstPrioriteitsNiveau[prioFC22] = PRM[prmprio22]/1000L;

    /* Prioriteitsopties */
    iInstPrioriteitsOpties[prioFC41bus] = BepaalPrioriteitsOpties(prmprio41bus);
    iInstPrioriteitsOpties[prioFC41tram] = BepaalPrioriteitsOpties(prmprio41tram);
    iInstPrioriteitsOpties[prioFC42] = BepaalPrioriteitsOpties(prmprio42);
    iInstPrioriteitsOpties[prioFC22] = BepaalPrioriteitsOpties(prmprio22);

    /* Groenbewakingstijd */
    iGroenBewakingsTijd[prioFC41bus] = T_max[tgb41bus];
    iGroenBewakingsTijd[prioFC41tram] = T_max[tgb41tram];
    iGroenBewakingsTijd[prioFC42] = T_max[tgb42];
    iGroenBewakingsTijd[prioFC22] = T_max[tgb22];

    /* Ongehinderde rijtijd */
    iRTSOngehinderd[prioFC41bus] = PRM[prmrto41bus];
    iRTSOngehinderd[prioFC41tram] = PRM[prmrto41tram];
    iRTSOngehinderd[prioFC42] = PRM[prmrto42];
    iRTSOngehinderd[prioFC22] = PRM[prmrto22];

    /* Beperkt gehinderde rijtijd */
    iRTSBeperktGehinderd[prioFC41bus] = PRM[prmrtbg41bus];
    iRTSBeperktGehinderd[prioFC41tram] = PRM[prmrtbg41tram];
    iRTSBeperktGehinderd[prioFC42] = PRM[prmrtbg42];
    iRTSBeperktGehinderd[prioFC22] = PRM[prmrtbg22];

    /* Gehinderde rijtijd */
    iRTSGehinderd[prioFC41bus] = PRM[prmrtg41bus];
    iRTSGehinderd[prioFC41tram] = PRM[prmrtg41tram];
    iRTSGehinderd[prioFC42] = PRM[prmrtg42];
    iRTSGehinderd[prioFC22] = PRM[prmrtg22];

    /* Ondermaximum */
    iOnderMaximum[prioFC41bus] = PRM[prmomx41bus];
    iOnderMaximum[prioFC41tram] = PRM[prmomx41tram];
    iOnderMaximum[prioFC42] = PRM[prmomx42];
    iOnderMaximum[prioFC22] = PRM[prmomx22];

    /* Blokkeringstijd */
    iBlokkeringsTijd[prioFC41bus] = T_max[tblk41bus];
    iBlokkeringsTijd[prioFC41tram] = T_max[tblk41tram];
    iBlokkeringsTijd[prioFC42] = T_max[tblk42];
    iBlokkeringsTijd[prioFC22] = T_max[tblk22];

    /* Na aanspreken groenbewaking wordt de selectieve 
	   detectie niet langer betrouwbaar gevonden */
    iSelDetFoutNaGB[prioFC41bus] = PRM[prmupinagb41bus];
    iSelDetFoutNaGB[prioFC41tram] = PRM[prmupinagb41tram];
    iSelDetFoutNaGB[prioFC42] = PRM[prmupinagb42];
    iSelDetFoutNaGB[prioFC22] = PRM[prmupinagb22];

    /* =============================== */
    /* Instellingen overige richtingen */
    /* =============================== */
   
    /* Maximale wachttijd */
    iMaximumWachtTijd[fc02] = PRM[prmmwta];
    iMaximumWachtTijd[fc22] = PRM[prmmwtfts];
    iMaximumWachtTijd[fc41] = PRM[prmmwta];
    iMaximumWachtTijd[fc42] = PRM[prmmwta];

    /* Als een richting minder groen heeft gehad door afkappen
       dan deze instelling, dan mag de richting nog een keer
       primair realiseren (terugkomen). */
    iInstPercMaxGroenTijdTerugKomen[fc02] = PRM[prmpmgt02];
    iInstPercMaxGroenTijdTerugKomen[fc22] = PRM[prmpmgt22];
    iInstPercMaxGroenTijdTerugKomen[fc41] = PRM[prmpmgt41];
    iInstPercMaxGroenTijdTerugKomen[fc42] = PRM[prmpmgt42];

    /* De minimale groentijd die een richting krijgt als
       deze mag terugkomen. */
    iInstMinTerugKomGroenTijd[fc02] = PRM[prmognt02];
    iInstMinTerugKomGroenTijd[fc22] = PRM[prmognt22];
    iInstMinTerugKomGroenTijd[fc41] = PRM[prmognt41];
    iInstMinTerugKomGroenTijd[fc42] = PRM[prmognt42];

    /* Aantal malen niet afkappen */
    iInstAantalMalenNietAfkappen[fc02] = PRM[prmnofm02];
    iInstAantalMalenNietAfkappen[fc22] = PRM[prmnofm22];
    iInstAantalMalenNietAfkappen[fc41] = PRM[prmnofm41];
    iInstAantalMalenNietAfkappen[fc42] = PRM[prmnofm42];

    /* Onder deze groentijd mag een richting niet worden
       afgekapt, tenzij zich een nooddienst heeft ingemeld. */
    iAfkapGroenTijd[fc02] = PRM[prmmgcov02];
    iAfkapGroenTijd[fc22] = PRM[prmmgcov22];
    iAfkapGroenTijd[fc41] = PRM[prmmgcov41];
    iAfkapGroenTijd[fc42] = PRM[prmmgcov42];

    /* Als een richting minder groen heeft gehad dan
       dit percentage van de maximum groentijd, dan
       mag de richting niet worden afgekapt, tenzij
       zich een nooddienst heeft ingemeld. */
    iPercGroenTijd[fc02] = PRM[prmpmgcov02];
    iPercGroenTijd[fc22] = PRM[prmpmgcov22];
    iPercGroenTijd[fc41] = PRM[prmpmgcov41];
    iPercGroenTijd[fc42] = PRM[prmpmgcov42];

    /* Na te zijn afgekapt, wordt het percentage
       van de maximumgroentijd verhoogd met dit ophoogpercentage. */
    iInstOphoogPercentageMG[fc02] = PRM[prmohpmg02];
    iInstOphoogPercentageMG[fc22] = PRM[prmohpmg22];
    iInstOphoogPercentageMG[fc41] = PRM[prmohpmg41];
    iInstOphoogPercentageMG[fc42] = PRM[prmohpmg42];

    /* Benodigde ruimte voor alternatieve realisatie tijdens een OV ingreep */
    iPRM_ALTP[fc02] = PRM[prmaltp02];
    iPRM_ALTP[fc22] = PRM[prmaltp22];
    iPRM_ALTP[fc41] = PRM[prmaltp41];
    iPRM_ALTP[fc42] = PRM[prmaltp42];

    /* Richting mag alternatief realiseren tijdens een OV ingreep */
    iSCH_ALTG[fc02] = SCH[schaltg02];
    iSCH_ALTG[fc22] = SCH[schaltg22];
    iSCH_ALTG[fc41] = SCH[schaltg41];
    iSCH_ALTG[fc42] = SCH[schaltg42];

    /* Alternatieve realisatie mag worden
       afgekapt tijdens een OV ingreep als
       deze groentijd is gemaakt */
    iInstAfkapGroenAlt[fc02] = PRM[prmaltg02];
    iInstAfkapGroenAlt[fc22] = PRM[prmaltg22];
    iInstAfkapGroenAlt[fc41] = PRM[prmaltg41];
    iInstAfkapGroenAlt[fc42] = PRM[prmaltg42];

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
    PrioRijTijdScenario(prioFC41bus, d411, d412, tbtovg41bus);

    PrioRijTijdScenario(prioFC41tram, d411, d412, tbtovg41tram);

    PrioRijTijdScenario(prioFC42, d421, d422, tbtovg42);

    PrioRijTijdScenario(prioFC22, d221, NG, NG);




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
    int i = 0;

    /* Prioriteit-inmeldingen */
    PrioInmelden(prioFC41bus, SH[hprioin41bus], iInstPrioriteitsNiveau[prioFC41bus], iInstPrioriteitsOpties[prioFC41bus], 0, 0);
    PrioInmelden(prioFC41tram, SH[hprioin41tram], iInstPrioriteitsNiveau[prioFC41tram], iInstPrioriteitsOpties[prioFC41tram], 0, 0);
    PrioInmelden(prioFC42, SH[hprioin42], iInstPrioriteitsNiveau[prioFC42], iInstPrioriteitsOpties[prioFC42], 0, 0);
    PrioInmelden(prioFC22, SH[hprioin22], iInstPrioriteitsNiveau[prioFC22], iInstPrioriteitsOpties[prioFC22], 0, 0);

    /* Prioriteit-uitmeldingen */
    PrioUitmelden(prioFC41bus, SH[hpriouit41bus]);
    PrioUitmelden(prioFC41tram, SH[hpriouit41tram]);
    PrioUitmelden(prioFC42, SH[hpriouit42]);
    PrioUitmelden(prioFC22, SH[hpriouit22]);



    /* Inmelding fc41 type Bus */
    IH[hprioin41bus] = IH[hprioin41busris] = FALSE;
#ifndef NO_RIS
    if (SCH[schprioin41busris])
    {
        IH[hprioin41busris] = 
            ris_inmelding_selectief(fc41, PRM[prmrisapproachid41bus], SYSTEM_ITF, PRM[prmrislaneid41bus_1], PRM[prmrisstationtype41bus], PRM[prmrisstart41bus], PRM[prmrisend41bus], PRM[prmrisrole41bus], PRM[prmrissubrole41bus], PRM[prmrisimportance41bus], PRM[prmriseta41bus], prioFC41bus) ||
            ris_inmelding_selectief(fc41, PRM[prmrisapproachid41bus], SYSTEM_ITF, PRM[prmrislaneid41bus_2], PRM[prmrisstationtype41bus], PRM[prmrisstart41bus], PRM[prmrisend41bus], PRM[prmrisrole41bus], PRM[prmrissubrole41bus], PRM[prmrisimportance41bus], PRM[prmriseta41bus], prioFC41bus);
        if (IH[hprioin41busris]) iPrioriteitNooitAfkappen[prioFC41bus] = TRUE;
    }
#endif /* NO_RIS */
    IH[hprioin41bus] = IH[hprioin41busris];

    /* Uitmelding fc41 type Bus */
    IH[hpriouit41bus] = IH[hpriouit41busris] = FALSE;
#ifndef NO_RIS
    if (SCH[schpriouit41busris])
    {
        IH[hpriouit41busris] = (boolv)ris_uitmelding_selectief(prioFC41bus);
        if (IH[hpriouit41busris] && iAantalInmeldingen[prioFC41bus] == 0) iPrioriteitNooitAfkappen[prioFC41bus] = FALSE;
    }
#endif /* NO_RIS */
    IH[hpriouit41bus] = IH[hpriouit41busris];

    /* Inmelding fc41 type Tram */
    IH[hprioin41tram] = IH[hprioin41tramris] = IH[hprioin41tramkar] = FALSE;
#ifndef NO_RIS
    if (SCH[schprioin41tramris])
    {
        IH[hprioin41tramris] = 
            ris_inmelding_selectief(fc41, PRM[prmrisapproachid41tram], SYSTEM_ITF, PRM[prmrislaneid41tram_1], PRM[prmrisstationtype41tram], PRM[prmrisstart41tram], PRM[prmrisend41tram], PRM[prmrisrole41tram], PRM[prmrissubrole41tram], PRM[prmrisimportance41tram], NG, prioFC41tram) ||
            ris_inmelding_selectief(fc41, PRM[prmrisapproachid41tram], SYSTEM_ITF, PRM[prmrislaneid41tram_2], PRM[prmrisstationtype41tram], PRM[prmrisstart41tram], PRM[prmrisend41tram], PRM[prmrisrole41tram], PRM[prmrissubrole41tram], PRM[prmrisimportance41tram], NG, prioFC41tram);
        if (IH[hprioin41tramris]) iPrioriteitNooitAfkappen[prioFC41tram] = TRUE;
    }
#endif /* NO_RIS */
    if (SCH[schprioin41tramkar])
    {
        IH[hprioin41tramkar] = 
            DSIMeldingPRIO_V1(0, PRM[prmvtgcat41tram], TRUE, 41,TRUE, CIF_DSIN, TRUE);
    }
    IH[hprioin41tram] = IH[hprioin41tramris] || IH[hprioin41tramkar];

    /* Uitmelding fc41 type Tram */
    IH[hpriouit41tram] = IH[hpriouit41tramris] = IH[hpriouit41tramkar] = FALSE;
#ifndef NO_RIS
    if (SCH[schpriouit41tramris])
    {
        IH[hpriouit41tramris] = (boolv)ris_uitmelding_selectief(prioFC41tram);
        if (IH[hpriouit41tramris] && iAantalInmeldingen[prioFC41tram] == 0) iPrioriteitNooitAfkappen[prioFC41tram] = FALSE;
    }
#endif /* NO_RIS */
    if (SCH[schpriouit41tramkar])
    {
        IH[hpriouit41tramkar] = 
            DSIMeldingPRIO_V2(fc41, prioFC41tram, 0, PRM[prmvtgcat41tram], TRUE, 41,TRUE, CIF_DSUIT, TRUE);
    }
    IH[hpriouit41tram] = IH[hpriouit41tramris] || IH[hpriouit41tramkar];

    /* Inmelding fc42 type Bus */
    IH[hprioin42] = IH[hprioin42ris] = FALSE;
#ifndef NO_RIS
    if (SCH[schprioin42ris])
    {
        IH[hprioin42ris] = 
            ris_inmelding_selectief(fc42, PRM[prmrisapproachid42], SYSTEM_ITF, PRM[prmrislaneid42_1], PRM[prmrisstationtype42], PRM[prmrisstart42], PRM[prmrisend42], PRM[prmrisrole42], PRM[prmrissubrole42], PRM[prmrisimportance42], PRM[prmriseta42], prioFC42);
        if (IH[hprioin42ris]) iPrioriteitNooitAfkappen[prioFC42] = TRUE;
    }
#endif /* NO_RIS */
    IH[hprioin42] = IH[hprioin42ris];

    /* Uitmelding fc42 type Bus */
    IH[hpriouit42] = IH[hpriouit42ris] = FALSE;
#ifndef NO_RIS
    if (SCH[schpriouit42ris])
    {
        IH[hpriouit42ris] = (boolv)ris_uitmelding_selectief(prioFC42);
        if (IH[hpriouit42ris] && iAantalInmeldingen[prioFC42] == 0) iPrioriteitNooitAfkappen[prioFC42] = FALSE;
    }
#endif /* NO_RIS */
    IH[hpriouit42] = IH[hpriouit42ris];

    /* Inmelding fc22 type Fiets */
    fietsprio_update(fc22, d221, cftsvtg22fiets, cftscyc22fiets,SH[hprioin22fiets221], ML);
    IH[hprioin22] = IH[hprioin22fiets221] = FALSE;
    if (SCH[schprioin22fiets221])
    {
        MM[mftstelris22fiets] = 0;
#ifndef NO_RIS
        MM[mftstelris22fiets] += ris_itsstations(fc22, SYSTEM_ITF, PRM[prmrislaneid_1], RIS_CYCLIST, PRM[prmrispstart22fts1], PRM[prmrispend22fts1], SCH[schrisgeencheckopsg]);
#endif /* NO_RIS */
        IH[hprioin22fiets221] = !C[cvc22] && 
            fietsprio_inmelding(fc22, d221, cftsvtg22fiets, cftscyc22fiets, prmftsblok22fiets, prmftsmaxpercyc22fiets, prmftsminvtg22fiets, prmftsminwt22fiets, ML, mftstelris22fiets, prmftsminvtgris22fiets);
    }
    IH[hprioin22] = IH[hprioin22fiets221];

    /* Uitmelding fc22 type Fiets */
    IH[hpriouit22] = IH[hpriouit22fiets221] = FALSE;
    if (SCH[schpriouit22fiets221])
    {
        IH[hpriouit22fiets221] = C[cvc22] && G[fc22] && (T[tgb22] && (T_timer[tgb22] > 1));
    }
    IH[hpriouit22] = IH[hpriouit22fiets221];

    #if !defined AUTOMAAT && !defined AUTOMAAT_TEST && !defined NO_RIS && !defined VISSIM && !defined SUMO
    if (display) {
        /* RIS_PRIOREQUEST_AP */
        for (i = 0; i < 5; ++i) {
            xyprintf(32, i+2, "                                                ");
        }
        xyprintf(32, 0, "RIS_PRIOREQUEST_AP                              ");
        xyprintf(32, 1, "sg  id--------------- seq.Nr-req.Type--routeName");
        for (i = 0; i < (RIS_PRIOREQUEST_AP_NUMBER <= 5 ? RIS_PRIOREQUEST_AP_NUMBER : 5); ++i) {
            xyprintf(32, i+2, "%s", RIS_PRIOREQUEST_AP[i].signalGroup);
            xyprintf(36, i+2, "%s", RIS_PRIOREQUEST_AP[i].id);
            xyprintf(55, i+2, "%d", RIS_PRIOREQUEST_AP[i].sequenceNumber);
            xyprintf(62, i+2, "%d", RIS_PRIOREQUEST_AP[i].requestType);
            xyprintf(72, i+2, "%s", RIS_PRIOREQUEST_AP[i].routeName);
        }

        /* RIS_PRIOREQUEST_EX_AP */
        for (i = 0; i < 5; i++) {
            xyprintf(32, i+10, "                                                ");
        }
        xyprintf(32, 8, "RIS_PRIOREQUEST_EX_AP                           ");
        xyprintf(32, 9, "    id--------------- seq.Nr-prioState-prCtrlState");
        for (i = 0; i < (RIS_PRIOREQUEST_EX_AP_NUMBER <= 5 ? RIS_PRIOREQUEST_EX_AP_NUMBER : 5); ++i) {
            xyprintf(36, i+10, "%s", RIS_PRIOREQUEST_EX_AP[i].id);
            xyprintf(55, i+10, "%d", RIS_PRIOREQUEST_EX_AP[i].sequenceNumber);
            xyprintf(62, i+10, "%d", RIS_PRIOREQUEST_EX_AP[i].prioState);
            xyprintf(72, i+10, "%d", RIS_PRIOREQUEST_EX_AP[i].prioControlState);
        }
    }

    #endif /* #ifndef AUTOMAAT */


#ifndef NO_RIS
    /* Bijhouden granted verstrekt */
    Bepaal_Granted_Verstrekt();
#endif /* NO_RIS */


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
    int i;

    #ifndef NO_RIS
    /* nooit einde groen als granted verstrekt */
    /* --------------------------------------- */
    for (i = 0; i < FCMAX; ++i)
    {
        if (granted_verstrekt[i] > 0)     /* als granted is verstrekt dan altijd groen aanhouden */
        {
            if (G[i] && !MG[i]) YV[i] |= PRIO_YV_BIT;
            YM[i] |= PRIO_YM_BIT;
            Z[i] = FALSE;
        }
    }
    #endif /* NO_RIS */

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
    PrioCcolElementen(prioFC41bus, tgb41bus, trt41bus, hprio41bus, cvc41bus, tblk41bus);
    PrioCcolElementen(prioFC41tram, tgb41tram, trt41tram, hprio41tram, cvc41tram, tblk41tram);
    PrioCcolElementen(prioFC42, tgb42, trt42, hprio42, cvc42, tblk42);
    PrioCcolElementen(prioFC22, tgb22, trt22, hprio22, cvc22, tblk22);
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

}
#endif

#ifdef PRIO_ADDFILE
    #include "GMprio.add"
#endif /* PRIO_ADDFILE */
