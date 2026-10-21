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
    iFC_PRIOix[hdFC02] = fc02;
    iFC_PRIOix[hdFC05] = fc05;
    iFC_PRIOix[hdFC08] = fc08;

    /* Index van de groenbewakingstimer */
    iT_GBix[prioFC41bus] = tgb41bus;
    iT_GBix[hdFC02] = tgbhd02;
    iT_GBix[hdFC05] = tgbhd05;
    iT_GBix[hdFC08] = tgbhd08;

    /* Index van het hulpelement voor de ingreep */
    iH_PRIOix[prioFC41bus] = hprio41bus;
    iH_PRIOix[hdFC02] = hhd02;
    iH_PRIOix[hdFC05] = hhd05;
    iH_PRIOix[hdFC08] = hhd08;

    /* Prioriteitsniveau */
    iInstPrioriteitsNiveau[prioFC41bus] = PRM[prmprio41bus]/1000L;
    iInstPrioriteitsNiveau[hdFC02] = PRM[prmpriohd02]/1000L;
    iInstPrioriteitsNiveau[hdFC05] = PRM[prmpriohd05]/1000L;
    iInstPrioriteitsNiveau[hdFC08] = PRM[prmpriohd08]/1000L;

    /* Prioriteitsopties */
    iInstPrioriteitsOpties[prioFC41bus] = BepaalPrioriteitsOpties(prmprio41bus);
    iInstPrioriteitsOpties[hdFC02] = BepaalPrioriteitsOpties(prmpriohd02);
    iInstPrioriteitsOpties[hdFC05] = BepaalPrioriteitsOpties(prmpriohd05);
    iInstPrioriteitsOpties[hdFC08] = BepaalPrioriteitsOpties(prmpriohd08);

    /* Groenbewakingstijd */
    iGroenBewakingsTijd[prioFC41bus] = T_max[tgb41bus];
    iGroenBewakingsTijd[hdFC02] = T_max[tgbhd02];
    iGroenBewakingsTijd[hdFC05] = T_max[tgbhd05];
    iGroenBewakingsTijd[hdFC08] = T_max[tgbhd08];

    /* Ongehinderde rijtijd */
    iRTSOngehinderd[prioFC41bus] = PRM[prmrto41bus];
    iRTSOngehinderd[hdFC02] = PRM[prmrtohd02];
    iRTSOngehinderd[hdFC05] = PRM[prmrtohd05];
    iRTSOngehinderd[hdFC08] = PRM[prmrtohd08];

    /* Beperkt gehinderde rijtijd */
    iRTSBeperktGehinderd[prioFC41bus] = PRM[prmrtbg41bus];
    iRTSBeperktGehinderd[hdFC02] = PRM[prmrtbghd02];
    iRTSBeperktGehinderd[hdFC05] = PRM[prmrtbghd05];
    iRTSBeperktGehinderd[hdFC08] = PRM[prmrtbghd08];

    /* Gehinderde rijtijd */
    iRTSGehinderd[prioFC41bus] = PRM[prmrtg41bus];
    iRTSGehinderd[hdFC02] = PRM[prmrtghd02];
    iRTSGehinderd[hdFC05] = PRM[prmrtghd05];
    iRTSGehinderd[hdFC08] = PRM[prmrtghd08];

    /* Ondermaximum */
    iOnderMaximum[prioFC41bus] = PRM[prmomx41bus];
    iOnderMaximum[hdFC02] = 0;
    iOnderMaximum[hdFC05] = 0;
    iOnderMaximum[hdFC08] = 0;

    /* Blokkeringstijd */
    iBlokkeringsTijd[prioFC41bus] = T_max[tblk41bus];
    iBlokkeringsTijd[hdFC02] = 0;
    iBlokkeringsTijd[hdFC05] = 0;
    iBlokkeringsTijd[hdFC08] = 0;

    /* Na aanspreken groenbewaking wordt de selectieve 
	   detectie niet langer betrouwbaar gevonden */
    iSelDetFoutNaGB[prioFC41bus] = PRM[prmupinagb41bus];
    iSelDetFoutNaGB[hdFC02] = PRM[prmupinagbhd02];
    iSelDetFoutNaGB[hdFC05] = PRM[prmupinagbhd05];
    iSelDetFoutNaGB[hdFC08] = PRM[prmupinagbhd08];

    /* =============================== */
    /* Instellingen overige richtingen */
    /* =============================== */
   
    /* Maximale wachttijd */
    iMaximumWachtTijd[fc02] = PRM[prmmwta];
    iMaximumWachtTijd[fc05] = PRM[prmmwta];
    iMaximumWachtTijd[fc08] = PRM[prmmwta];
    iMaximumWachtTijd[fc41] = PRM[prmmwta];

    /* Als een richting minder groen heeft gehad door afkappen
       dan deze instelling, dan mag de richting nog een keer
       primair realiseren (terugkomen). */
    iInstPercMaxGroenTijdTerugKomen[fc02] = PRM[prmpmgt02];
    iInstPercMaxGroenTijdTerugKomen[fc05] = PRM[prmpmgt05];
    iInstPercMaxGroenTijdTerugKomen[fc08] = PRM[prmpmgt08];

    /* De minimale groentijd die een richting krijgt als
       deze mag terugkomen. */
    iInstMinTerugKomGroenTijd[fc02] = PRM[prmognt02];
    iInstMinTerugKomGroenTijd[fc05] = PRM[prmognt05];
    iInstMinTerugKomGroenTijd[fc08] = PRM[prmognt08];

    /* Aantal malen niet afkappen */
    iInstAantalMalenNietAfkappen[fc02] = PRM[prmnofm02];
    iInstAantalMalenNietAfkappen[fc05] = PRM[prmnofm05];
    iInstAantalMalenNietAfkappen[fc08] = PRM[prmnofm08];

    /* Onder deze groentijd mag een richting niet worden
       afgekapt, tenzij zich een nooddienst heeft ingemeld. */
    iAfkapGroenTijd[fc02] = PRM[prmmgcov02];
    iAfkapGroenTijd[fc05] = PRM[prmmgcov05];
    iAfkapGroenTijd[fc08] = PRM[prmmgcov08];

    /* Als een richting minder groen heeft gehad dan
       dit percentage van de maximum groentijd, dan
       mag de richting niet worden afgekapt, tenzij
       zich een nooddienst heeft ingemeld. */
    iPercGroenTijd[fc02] = PRM[prmpmgcov02];
    iPercGroenTijd[fc05] = PRM[prmpmgcov05];
    iPercGroenTijd[fc08] = PRM[prmpmgcov08];

    /* Na te zijn afgekapt, wordt het percentage
       van de maximumgroentijd verhoogd met dit ophoogpercentage. */
    iInstOphoogPercentageMG[fc02] = PRM[prmohpmg02];
    iInstOphoogPercentageMG[fc05] = PRM[prmohpmg05];
    iInstOphoogPercentageMG[fc08] = PRM[prmohpmg08];

    /* Benodigde ruimte voor alternatieve realisatie tijdens een OV ingreep */
    iPRM_ALTP[fc02] = PRM[prmaltp02];
    iPRM_ALTP[fc05] = PRM[prmaltp05];
    iPRM_ALTP[fc08] = PRM[prmaltp08];
    iPRM_ALTP[fc41] = PRM[prmaltp41];

    /* Richting mag alternatief realiseren tijdens een OV ingreep */
    iSCH_ALTG[fc02] = SCH[schaltg02];
    iSCH_ALTG[fc05] = SCH[schaltg05];
    iSCH_ALTG[fc08] = SCH[schaltg08];
    iSCH_ALTG[fc41] = SCH[schaltg41];

    /* Alternatieve realisatie mag worden
       afgekapt tijdens een OV ingreep als
       deze groentijd is gemaakt */
    iInstAfkapGroenAlt[fc02] = PRM[prmaltg02];
    iInstAfkapGroenAlt[fc05] = PRM[prmaltg05];
    iInstAfkapGroenAlt[fc08] = PRM[prmaltg08];
    iInstAfkapGroenAlt[fc41] = PRM[prmaltg41];

    /* definitie van de meerealisaties voor de hulpdiensten */
    iPrioMeeRealisatie[fc02][0] = fc05;
    iPrioMeeRealisatie[fc05][0] = fc02;

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

    PrioRijTijdScenario(hdFC02, d021, d022, tbtovg02hd);
    PrioRijTijdScenario(hdFC05, d051, d052, tbtovg05hd);
    PrioRijTijdScenario(hdFC08, d081, d082, tbtovg08hd);



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
    PrioInmelden(prioFC41bus, SH[hprioin41bus], iInstPrioriteitsNiveau[prioFC41bus], iInstPrioriteitsOpties[prioFC41bus], 0, 0);

    /* Prioriteit-uitmeldingen */
    PrioUitmelden(prioFC41bus, SH[hpriouit41bus]);

    /* HD-inmeldingen */
    PrioInmelden(hdFC02, SH[hhdin02], iInstPrioriteitsNiveau[hdFC02], iInstPrioriteitsOpties[hdFC02], 0, 0);
    PrioInmelden(hdFC05, SH[hhdin05], iInstPrioriteitsNiveau[hdFC05], iInstPrioriteitsOpties[hdFC05], 0, 0);
    PrioInmelden(hdFC08, SH[hhdin08], iInstPrioriteitsNiveau[hdFC08], iInstPrioriteitsOpties[hdFC08], 0, 0);

    /* HD-uitmeldingen */
    PrioUitmelden(hdFC02, SH[hhduit02]);
    PrioUitmelden(hdFC05, SH[hhduit05]);
    PrioUitmelden(hdFC08, SH[hhduit08]);

    IH[hhdin02] = IH[hhduit02] = FALSE;
    IH[hhdin05] = IH[hhduit05] = FALSE;
    IH[hhdin08] = IH[hhduit08] = FALSE;


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

    /* Inmelding HD fc02 */
    IH[hhdin02kar] = RT[thdin02kar] = !T[thdin02kar] && SCH[schhdin02kar] && ((DSIMelding_HD_V1(2, CIF_DSIN, SCH[schchecksirene02]) || DSIMelding_HD_V1(5, CIF_DSIN, SCH[schchecksirene02])));
    IH[hhdin02] = IH[hhdin02kar];

    /* Inmelding HD fc05 */
    IH[hhdin05kar] = SCH[schhdin05kar] && (DSIMelding_HD_V1(5, CIF_DSIN, SCH[schchecksirene05]));
    IH[hhdin05opt] = !T[thdin05opt] && SCH[schhdinuit05opt] && !C[cvchd05] && DB[d051];
    RT[thdin05opt] = G[fc05] && C[cvchd05];
    IH[hhdin05] = IH[hhdin05kar] || IH[hhdin05opt];

    /* Inmelding HD fc08 */
#ifndef NO_RIS
    IH[hhdin08ris] = SCH[schhdin08ris] && (
            ris_inmelding_selectief(fc08, PRM[prmrisapproachid08hd], SYSTEM_ITF, PRM[prmrislaneid08hd_1], PRM[prmrisstationtype08hd], PRM[prmrisstart08hd], PRM[prmrisend08hd], PRM[prmrisrole08hd], PRM[prmrissubrole08hd], PRM[prmrisimportance08hd], NG, hdFC08) ||
            ris_inmelding_selectief(fc08, PRM[prmrisapproachid08hd], SYSTEM_ITF, PRM[prmrislaneid08hd_2], PRM[prmrisstationtype08hd], PRM[prmrisstart08hd], PRM[prmrisend08hd], PRM[prmrisrole08hd], PRM[prmrissubrole08hd], PRM[prmrisimportance08hd], NG, hdFC08));
#endif /* NO_RIS */
    IH[hhdin08] = IH[hhdin08ris];

    /* Uitmelding HD fc02 */
    IH[hhduit02kar] = RT[thduit02kar] = !T[thduit02kar] && SCH[schhduit02kar] && (DSIMelding_HD_V1(2, CIF_DSUIT, FALSE) || DSIMelding_HD_V1(5, CIF_DSUIT, FALSE));
    IH[hhduit02] = IH[hhduit02kar];

    /* Uitmelding HD fc05 */
    IH[hhduit05kar] = SCH[schhduit05kar] && (DSIMelding_HD_V1(5, CIF_DSUIT, FALSE));
    IH[hhduit05opt] = SCH[schhdinuit05opt] && !TDH[d051] && TDH_old[d051];
    IH[hhduit05] = IH[hhduit05kar] || IH[hhduit05opt];

    /* Uitmelding HD fc08 */
#ifndef NO_RIS
    IH[hhduit08ris] = SCH[schhduit08ris] && (ris_uitmelding_selectief(hdFC08));
#endif /* NO_RIS */
    IH[hhduit08] = IH[hhduit08ris];

#ifndef NO_RIS
    /* Bijhouden granted verstrekt */
    Bepaal_Granted_Verstrekt();

    if (granted_verstrekt[fc05] == 2) granted_verstrekt[fc02] = 2;
    if (granted_verstrekt[fc02] == 2) granted_verstrekt[fc05] = 2;
#endif /* NO_RIS */

    /* herstarten FB_timer bij in- of uitmelding HD of einde ingreep (door groenbewaking) */
    RTFB &= ~PRIO_RTFB_BIT;
    if (IH[hhdin02]|| IH[hhduit02] || EH[hhd02] ||
        IH[hhdin05]|| IH[hhduit05] || EH[hhd05] ||
        IH[hhdin08]|| IH[hhduit08] || EH[hhd08])
    {
        RTFB |= PRIO_RTFB_BIT;
    }

    /* Bijhouden melding en ondergedrag KAR */
    RT[tkarmelding] = CIF_DSIWIJZ != 0 && CIF_DSI[CIF_DSI_LUS] == 0;
    RT[tkarog] = T[tkarmelding] || !startkarog;
    if (!startkarog) startkarog = TRUE;
    /* Doorzetten HD inmeldingen */
    IH[hhdin05] |= IH[hhdin02]; IH[hhduit05] |= IH[hhduit02];
    IH[hhdin02] |= IH[hhdin05]; IH[hhduit02] |= IH[hhduit05];
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
    PrioCcolElementen(hdFC02, tgbhd02, trthd02, hhd02, cvchd02, -1);
    PrioCcolElementen(hdFC05, tgbhd05, trthd05, hhd05, cvchd05, -1);
    PrioCcolElementen(hdFC08, tgbhd08, trthd08, hhd08, cvchd08, -1);
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
    if (SD[ddummykarin41bus]) set_DSI_message(NG, CIF_BUS, 41, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykaruit41bus] && C[cvc41bus]) set_DSI_message(NG, CIF_BUS, 41, CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);

    /* HD ingrepen */
    if (SD[ddummyhdkarin02]) set_DSI_message(0, CIF_POL, 2, CIF_DSIN, 1, 0, 0, 0, CIF_SIR);
    if (SD[ddummyhdkaruit02] && C[cvchd02]) set_DSI_message(0, CIF_POL, 2, CIF_DSUIT, 1, 0, 0, 0, CIF_SIR);
    if (SD[ddummyhdkarin05]) set_DSI_message(0, CIF_POL, 5, CIF_DSIN, 1, 0, 0, 0, CIF_SIR);
    if (SD[ddummyhdkaruit05] && C[cvchd05]) set_DSI_message(0, CIF_POL, 5, CIF_DSUIT, 1, 0, 0, 0, CIF_SIR);
}
#endif

#ifdef PRIO_ADDFILE
    #include "GMprio.add"
#endif /* PRIO_ADDFILE */
