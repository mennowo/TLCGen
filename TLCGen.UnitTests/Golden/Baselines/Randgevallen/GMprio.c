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
    iFC_PRIOix[prioFC61abus] = fc61a;
    iFC_PRIOix[prioFC61ahd] = fc61a;
    iFC_PRIOix[prioFC22fiets] = fc22;
    iFC_PRIOix[prioFC41bus] = fc41;

    /* Index van de groenbewakingstimer */
    iT_GBix[prioFC61abus] = tgb61abus;
    iT_GBix[prioFC61ahd] = tgb61ahd;
    iT_GBix[prioFC22fiets] = tgb22fiets;
    iT_GBix[prioFC41bus] = tgb41bus;

    /* Index van het hulpelement voor de ingreep */
    iH_PRIOix[prioFC61abus] = hprio61abus;
    iH_PRIOix[prioFC61ahd] = hprio61ahd;
    iH_PRIOix[prioFC22fiets] = hprio22fiets;
    iH_PRIOix[prioFC41bus] = hprio41bus;

    /* Prioriteitsniveau */
    iInstPrioriteitsNiveau[prioFC61abus] = PRM[prmprio61abus]/1000L;
    iInstPrioriteitsNiveau[prioFC61ahd] = PRM[prmprio61ahd]/1000L;
    iInstPrioriteitsNiveau[prioFC22fiets] = PRM[prmprio22fiets]/1000L;
    iInstPrioriteitsNiveau[prioFC41bus] = PRM[prmprio41bus]/1000L;

    /* Prioriteitsopties */
    iInstPrioriteitsOpties[prioFC61abus] = BepaalPrioriteitsOpties(prmprio61abus);
    iInstPrioriteitsOpties[prioFC61ahd] = BepaalPrioriteitsOpties(prmprio61ahd);
    iInstPrioriteitsOpties[prioFC22fiets] = BepaalPrioriteitsOpties(prmprio22fiets);
    iInstPrioriteitsOpties[prioFC41bus] = BepaalPrioriteitsOpties(prmprio41bus);

    /* Groenbewakingstijd */
    iGroenBewakingsTijd[prioFC61abus] = T_max[tgb61abus];
    iGroenBewakingsTijd[prioFC61ahd] = T_max[tgb61ahd];
    iGroenBewakingsTijd[prioFC22fiets] = T_max[tgb22fiets];
    iGroenBewakingsTijd[prioFC41bus] = T_max[tgb41bus];

    /* Ongehinderde rijtijd */
    iRTSOngehinderd[prioFC61abus] = PRM[prmrto61abus];
    iRTSOngehinderd[prioFC61ahd] = PRM[prmrto61ahd];
    iRTSOngehinderd[prioFC22fiets] = PRM[prmrto22fiets];
    iRTSOngehinderd[prioFC41bus] = PRM[prmrto41bus];

    /* Beperkt gehinderde rijtijd */
    iRTSBeperktGehinderd[prioFC61abus] = PRM[prmrtbg61abus];
    iRTSBeperktGehinderd[prioFC61ahd] = PRM[prmrtbg61ahd];
    iRTSBeperktGehinderd[prioFC22fiets] = PRM[prmrtbg22fiets];
    iRTSBeperktGehinderd[prioFC41bus] = PRM[prmrtbg41bus];

    /* Gehinderde rijtijd */
    iRTSGehinderd[prioFC61abus] = PRM[prmrtg61abus];
    iRTSGehinderd[prioFC61ahd] = PRM[prmrtg61ahd];
    iRTSGehinderd[prioFC22fiets] = PRM[prmrtg22fiets];
    iRTSGehinderd[prioFC41bus] = PRM[prmrtg41bus];

    /* Ondermaximum */
    iOnderMaximum[prioFC61abus] = PRM[prmomx61abus];
    iOnderMaximum[prioFC61ahd] = PRM[prmomx61ahd];
    iOnderMaximum[prioFC22fiets] = PRM[prmomx22fiets];
    iOnderMaximum[prioFC41bus] = PRM[prmomx41bus];

    /* Blokkeringstijd */
    iBlokkeringsTijd[prioFC61abus] = T_max[tblk61abus];
    iBlokkeringsTijd[prioFC61ahd] = T_max[tblk61ahd];
    iBlokkeringsTijd[prioFC22fiets] = T_max[tblk22fiets];
    iBlokkeringsTijd[prioFC41bus] = T_max[tblk41bus];

    /* Na aanspreken groenbewaking wordt de selectieve 
	   detectie niet langer betrouwbaar gevonden */
    iSelDetFoutNaGB[prioFC61abus] = PRM[prmupinagb61abus];
    iSelDetFoutNaGB[prioFC61ahd] = PRM[prmupinagb61ahd];
    iSelDetFoutNaGB[prioFC22fiets] = PRM[prmupinagb22fiets];
    iSelDetFoutNaGB[prioFC41bus] = PRM[prmupinagb41bus];

    /* =============================== */
    /* Instellingen overige richtingen */
    /* =============================== */
   
    /* Maximale wachttijd */
    iMaximumWachtTijd[fc02] = PRM[prmmwta];
    iMaximumWachtTijd[fc22] = PRM[prmmwtfts];
    iMaximumWachtTijd[fc41] = PRM[prmmwta];
    iMaximumWachtTijd[fc61a] = PRM[prmmwta];

    /* Als een richting minder groen heeft gehad door afkappen
       dan deze instelling, dan mag de richting nog een keer
       primair realiseren (terugkomen). */
    iInstPercMaxGroenTijdTerugKomen[fc02] = PRM[prmpmgt02];
    iInstPercMaxGroenTijdTerugKomen[fc22] = PRM[prmpmgt22];
    iInstPercMaxGroenTijdTerugKomen[fc41] = PRM[prmpmgt41];
    iInstPercMaxGroenTijdTerugKomen[fc61a] = PRM[prmpmgt61a];

    /* De minimale groentijd die een richting krijgt als
       deze mag terugkomen. */
    iInstMinTerugKomGroenTijd[fc02] = PRM[prmognt02];
    iInstMinTerugKomGroenTijd[fc22] = PRM[prmognt22];
    iInstMinTerugKomGroenTijd[fc41] = PRM[prmognt41];
    iInstMinTerugKomGroenTijd[fc61a] = PRM[prmognt61a];

    /* Aantal malen niet afkappen */
    iInstAantalMalenNietAfkappen[fc02] = PRM[prmnofm02];
    iInstAantalMalenNietAfkappen[fc22] = PRM[prmnofm22];
    iInstAantalMalenNietAfkappen[fc41] = PRM[prmnofm41];
    iInstAantalMalenNietAfkappen[fc61a] = PRM[prmnofm61a];

    /* Onder deze groentijd mag een richting niet worden
       afgekapt, tenzij zich een nooddienst heeft ingemeld. */
    iAfkapGroenTijd[fc02] = PRM[prmmgcov02];
    iAfkapGroenTijd[fc22] = PRM[prmmgcov22];
    iAfkapGroenTijd[fc41] = PRM[prmmgcov41];
    iAfkapGroenTijd[fc61a] = PRM[prmmgcov61a];

    /* Als een richting minder groen heeft gehad dan
       dit percentage van de maximum groentijd, dan
       mag de richting niet worden afgekapt, tenzij
       zich een nooddienst heeft ingemeld. */
    iPercGroenTijd[fc02] = PRM[prmpmgcov02];
    iPercGroenTijd[fc22] = PRM[prmpmgcov22];
    iPercGroenTijd[fc41] = PRM[prmpmgcov41];
    iPercGroenTijd[fc61a] = PRM[prmpmgcov61a];

    /* Na te zijn afgekapt, wordt het percentage
       van de maximumgroentijd verhoogd met dit ophoogpercentage. */
    iInstOphoogPercentageMG[fc02] = PRM[prmohpmg02];
    iInstOphoogPercentageMG[fc22] = PRM[prmohpmg22];
    iInstOphoogPercentageMG[fc41] = PRM[prmohpmg41];
    iInstOphoogPercentageMG[fc61a] = PRM[prmohpmg61a];

    /* Benodigde ruimte voor alternatieve realisatie tijdens een OV ingreep */
    iPRM_ALTP[fc02] = PRM[prmaltp02];
    iPRM_ALTP[fc22] = PRM[prmaltp22];
    iPRM_ALTP[fc41] = PRM[prmaltp41];
    iPRM_ALTP[fc61a] = PRM[prmaltp61a];

    /* Richting mag alternatief realiseren tijdens een OV ingreep */
    iSCH_ALTG[fc02] = SCH[schaltg02];
    iSCH_ALTG[fc22] = SCH[schaltg22];
    iSCH_ALTG[fc41] = SCH[schaltg41];
    iSCH_ALTG[fc61a] = SCH[schaltg61a];

    /* Alternatieve realisatie mag worden
       afgekapt tijdens een OV ingreep als
       deze groentijd is gemaakt */
    iInstAfkapGroenAlt[fc02] = PRM[prmaltg02];
    iInstAfkapGroenAlt[fc22] = PRM[prmaltg22];
    iInstAfkapGroenAlt[fc41] = PRM[prmaltg41];
    iInstAfkapGroenAlt[fc61a] = PRM[prmaltg61a];

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
    PrioRijTijdScenario(prioFC61abus, d61a1, d61a2, tbtovg61abus);

    PrioRijTijdScenario(prioFC61ahd, d61a1, d61a2, tbtovg61ahd);

    PrioRijTijdScenario(prioFC22fiets, d221, NG, NG);

    PrioRijTijdScenario(prioFC41bus, d411, d412, tbtovg41bus);




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
    PrioInmelden(prioFC61abus, SH[hprioin61abus], iInstPrioriteitsNiveau[prioFC61abus], iInstPrioriteitsOpties[prioFC61abus], 0, 0);
    PrioInmelden(prioFC61ahd, SH[hprioin61ahd], iInstPrioriteitsNiveau[prioFC61ahd], iInstPrioriteitsOpties[prioFC61ahd], 0, 0);
    PrioInmelden(prioFC22fiets, SH[hprioin22fiets], iInstPrioriteitsNiveau[prioFC22fiets], iInstPrioriteitsOpties[prioFC22fiets], 0, 0);
    PrioInmelden(prioFC41bus, SH[hprioin41bus], iInstPrioriteitsNiveau[prioFC41bus], iInstPrioriteitsOpties[prioFC41bus], 0, 0);

    /* Prioriteit-uitmeldingen */
    PrioUitmelden(prioFC61abus, SH[hpriouit61abus]);
    PrioUitmelden(prioFC61ahd, SH[hpriouit61ahd]);
    PrioUitmelden(prioFC22fiets, SH[hpriouit22fiets]);
    PrioUitmelden(prioFC41bus, SH[hpriouit41bus]);



    /* Inmelding fc61a type Bus */
    IH[hprioin61abus] = IH[hprioin61abuskar] = FALSE;
    if (SCH[schprioin61abuskar])
    {
        IH[hprioin61abuskar] = 
            DSIMeldingPRIO_V1(0, PRM[prmvtgcat61abus], TRUE, NG,TRUE, CIF_DSIN, TRUE);
    }
    IH[hprioin61abus] = IH[hprioin61abuskar];

    /* Uitmelding fc61a type Bus */
    IH[hpriouit61abus] = IH[hpriouit61abuskar] = FALSE;
    if (SCH[schpriouit61abuskar])
    {
        IH[hpriouit61abuskar] = 
            DSIMeldingPRIO_V2(fc61a, prioFC61abus, 0, PRM[prmvtgcat61abus], TRUE, NG,TRUE, CIF_DSUIT, TRUE);
    }
    IH[hpriouit61abus] = IH[hpriouit61abuskar];

    /* Inmelding fc61a type Nood- en hulpdienst */
    IH[hprioin61ahd] = IH[hprioin61ahdkar] = FALSE;
    if (SCH[schprioin61ahdkar])
    {
        IH[hprioin61ahdkar] = 
            DSIMelding_HD_V1(-1, CIF_DSIN, (!SCH[schchecksirene61ahd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR));
    }
    IH[hprioin61ahd] = IH[hprioin61ahdkar];

    /* Uitmelding fc61a type Nood- en hulpdienst */
    IH[hpriouit61ahd] = IH[hpriouit61ahdkar] = FALSE;
    if (SCH[schpriouit61ahdkar])
    {
        IH[hpriouit61ahdkar] = 
            DSIMelding_HD_V1(-1, CIF_DSUIT, (!SCH[schchecksirene61ahd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR));
    }
    IH[hpriouit61ahd] = IH[hpriouit61ahdkar];

    /* Inmelding fc22 type Fiets */
    fietsprio_update(fc22, NG, NG, cftscyc22fietsfiets,SH[hprioin22fietsfiets], ML);
    IH[hprioin22fiets] = IH[hprioin22fietsfiets221] = FALSE;
    if (SCH[schprioin22fietsfiets221])
    {
        IH[hprioin22fietsfiets221] = !C[cvc22fiets] && 
            fietsprio_inmelding(fc22, NG, NG, cftscyc22fietsfiets, prmftsblok22fietsfiets, prmftsmaxpercyc22fietsfiets, NG, prmftsminwt22fietsfiets, ML, NG, NG);
    }
    IH[hprioin22fiets] = IH[hprioin22fietsfiets221];

    /* Uitmelding fc22 type Fiets */
    IH[hpriouit22fiets] = IH[hpriouit22fietsfiets] = FALSE;
    if (SCH[schpriouit22fietsfiets])
    {
        IH[hpriouit22fietsfiets] = C[cvc22fiets] && G[fc22] && (T[tgb22fiets] && (T_timer[tgb22fiets] > 1));
    }
    IH[hpriouit22fiets] = IH[hpriouit22fietsfiets];

    /* Inmelding fc41 type Bus */
    IH[hprioin41bus] = IH[hprioin41busdet411] = IH[hprioin41businpov41in] = FALSE;
    if (SCH[schprioin41busdet411SD] && (CIF_IS[d411] < CIF_DET_STORING))
    {
        MM[mftstelris41busdet] = 0;
        IH[hprioin41busdet411] = SD[d411];
    }
    if (SCH[schprioin41businpov41inSIS])
    {
        IH[hprioin41businpov41in] = IS[isov41in] && !IS_old[isov41in];
    }
    else if (SCH[schprioin41busdet413SD] && (CIF_IS[d413] < CIF_DET_STORING))
    {
        IH[hprioin41businpov41in] = SD[d413];
    }
    IH[hprioin41bus] = IH[hprioin41busdet411] || IH[hprioin41businpov41in];

    /* Uitmelding fc41 type Bus */
    IH[hpriouit41bus] = IH[hpriouit41busdet412] = FALSE;
    if (SCH[schpriouit41busdet412ED] && (CIF_IS[d412] < CIF_DET_STORING))
    {
        IH[hpriouit41busdet412] = ED[d412];
    }
    IH[hpriouit41bus] = IH[hpriouit41busdet412];

    /* herstarten FB_timer bij in- of uitmelding HD of einde ingreep (door groenbewaking) */
    RTFB &= ~PRIO_RTFB_BIT;
    if (IH[hprioin61ahd]|| IH[hpriouit61ahd] || EH[hprio61ahd])
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
    PrioCcolElementen(prioFC61abus, tgb61abus, trt61abus, hprio61abus, cvc61abus, tblk61abus);
    PrioCcolElementen(prioFC61ahd, tgb61ahd, trt61ahd, hprio61ahd, cvc61ahd, tblk61ahd);
    PrioCcolElementen(prioFC22fiets, tgb22fiets, trt22fiets, hprio22fiets, cvc22fiets, tblk22fiets);
    PrioCcolElementen(prioFC41bus, tgb41bus, trt41bus, hprio41bus, cvc41bus, tblk41bus);
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

}
#endif

#ifdef PRIO_ADDFILE
    #include "GMprio.add"
#endif /* PRIO_ADDFILE */
