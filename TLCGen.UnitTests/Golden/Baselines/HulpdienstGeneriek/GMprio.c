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
    iFC_PRIOix[prioFC02hd] = fc02;
    iFC_PRIOix[prioFC05hd] = fc05;
    iFC_PRIOix[prioFC08hd] = fc08;
    iFC_PRIOix[prioFC41bus] = fc41;

    /* Index van de groenbewakingstimer */
    iT_GBix[prioFC02hd] = tgb02hd;
    iT_GBix[prioFC05hd] = tgb05hd;
    iT_GBix[prioFC08hd] = tgb08hd;
    iT_GBix[prioFC41bus] = tgb41bus;

    /* Index van het hulpelement voor de ingreep */
    iH_PRIOix[prioFC02hd] = hprio02hd;
    iH_PRIOix[prioFC05hd] = hprio05hd;
    iH_PRIOix[prioFC08hd] = hprio08hd;
    iH_PRIOix[prioFC41bus] = hprio41bus;

    /* Prioriteitsniveau */
    iInstPrioriteitsNiveau[prioFC02hd] = PRM[prmprio02hd]/1000L;
    iInstPrioriteitsNiveau[prioFC05hd] = PRM[prmprio05hd]/1000L;
    iInstPrioriteitsNiveau[prioFC08hd] = PRM[prmprio08hd]/1000L;
    iInstPrioriteitsNiveau[prioFC41bus] = PRM[prmprio41bus]/1000L;

    /* Prioriteitsopties */
    iInstPrioriteitsOpties[prioFC02hd] = BepaalPrioriteitsOpties(prmprio02hd);
    iInstPrioriteitsOpties[prioFC05hd] = BepaalPrioriteitsOpties(prmprio05hd);
    iInstPrioriteitsOpties[prioFC08hd] = BepaalPrioriteitsOpties(prmprio08hd);
    iInstPrioriteitsOpties[prioFC41bus] = BepaalPrioriteitsOpties(prmprio41bus);

    /* Groenbewakingstijd */
    iGroenBewakingsTijd[prioFC02hd] = T_max[tgb02hd];
    iGroenBewakingsTijd[prioFC05hd] = T_max[tgb05hd];
    iGroenBewakingsTijd[prioFC08hd] = T_max[tgb08hd];
    iGroenBewakingsTijd[prioFC41bus] = T_max[tgb41bus];

    /* Ongehinderde rijtijd */
    iRTSOngehinderd[prioFC02hd] = PRM[prmrto02hd];
    iRTSOngehinderd[prioFC05hd] = PRM[prmrto05hd];
    iRTSOngehinderd[prioFC08hd] = PRM[prmrto08hd];
    iRTSOngehinderd[prioFC41bus] = PRM[prmrto41bus];

    /* Beperkt gehinderde rijtijd */
    iRTSBeperktGehinderd[prioFC02hd] = PRM[prmrtbg02hd];
    iRTSBeperktGehinderd[prioFC05hd] = PRM[prmrtbg05hd];
    iRTSBeperktGehinderd[prioFC08hd] = PRM[prmrtbg08hd];
    iRTSBeperktGehinderd[prioFC41bus] = PRM[prmrtbg41bus];

    /* Gehinderde rijtijd */
    iRTSGehinderd[prioFC02hd] = PRM[prmrtg02hd];
    iRTSGehinderd[prioFC05hd] = PRM[prmrtg05hd];
    iRTSGehinderd[prioFC08hd] = PRM[prmrtg08hd];
    iRTSGehinderd[prioFC41bus] = PRM[prmrtg41bus];

    /* Ondermaximum */
    iOnderMaximum[prioFC02hd] = PRM[prmomx02hd];
    iOnderMaximum[prioFC05hd] = PRM[prmomx05hd];
    iOnderMaximum[prioFC08hd] = PRM[prmomx08hd];
    iOnderMaximum[prioFC41bus] = PRM[prmomx41bus];

    /* Blokkeringstijd */
    iBlokkeringsTijd[prioFC02hd] = T_max[tblk02hd];
    iBlokkeringsTijd[prioFC05hd] = T_max[tblk05hd];
    iBlokkeringsTijd[prioFC08hd] = T_max[tblk08hd];
    iBlokkeringsTijd[prioFC41bus] = T_max[tblk41bus];

    /* Na aanspreken groenbewaking wordt de selectieve 
	   detectie niet langer betrouwbaar gevonden */
    iSelDetFoutNaGB[prioFC02hd] = PRM[prmupinagb02hd];
    iSelDetFoutNaGB[prioFC05hd] = PRM[prmupinagb05hd];
    iSelDetFoutNaGB[prioFC08hd] = PRM[prmupinagb08hd];
    iSelDetFoutNaGB[prioFC41bus] = PRM[prmupinagb41bus];

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
    iInstPercMaxGroenTijdTerugKomen[fc41] = PRM[prmpmgt41];

    /* De minimale groentijd die een richting krijgt als
       deze mag terugkomen. */
    iInstMinTerugKomGroenTijd[fc02] = PRM[prmognt02];
    iInstMinTerugKomGroenTijd[fc05] = PRM[prmognt05];
    iInstMinTerugKomGroenTijd[fc08] = PRM[prmognt08];
    iInstMinTerugKomGroenTijd[fc41] = PRM[prmognt41];

    /* Aantal malen niet afkappen */
    iInstAantalMalenNietAfkappen[fc02] = PRM[prmnofm02];
    iInstAantalMalenNietAfkappen[fc05] = PRM[prmnofm05];
    iInstAantalMalenNietAfkappen[fc08] = PRM[prmnofm08];
    iInstAantalMalenNietAfkappen[fc41] = PRM[prmnofm41];

    /* Onder deze groentijd mag een richting niet worden
       afgekapt, tenzij zich een nooddienst heeft ingemeld. */
    iAfkapGroenTijd[fc02] = PRM[prmmgcov02];
    iAfkapGroenTijd[fc05] = PRM[prmmgcov05];
    iAfkapGroenTijd[fc08] = PRM[prmmgcov08];
    iAfkapGroenTijd[fc41] = PRM[prmmgcov41];

    /* Als een richting minder groen heeft gehad dan
       dit percentage van de maximum groentijd, dan
       mag de richting niet worden afgekapt, tenzij
       zich een nooddienst heeft ingemeld. */
    iPercGroenTijd[fc02] = PRM[prmpmgcov02];
    iPercGroenTijd[fc05] = PRM[prmpmgcov05];
    iPercGroenTijd[fc08] = PRM[prmpmgcov08];
    iPercGroenTijd[fc41] = PRM[prmpmgcov41];

    /* Na te zijn afgekapt, wordt het percentage
       van de maximumgroentijd verhoogd met dit ophoogpercentage. */
    iInstOphoogPercentageMG[fc02] = PRM[prmohpmg02];
    iInstOphoogPercentageMG[fc05] = PRM[prmohpmg05];
    iInstOphoogPercentageMG[fc08] = PRM[prmohpmg08];
    iInstOphoogPercentageMG[fc41] = PRM[prmohpmg41];

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
    PrioRijTijdScenario(prioFC02hd, d021, d022, tbtovg02hd);

    PrioRijTijdScenario(prioFC05hd, d051, d052, tbtovg05hd);

    PrioRijTijdScenario(prioFC08hd, d081, d082, tbtovg08hd);

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
    PrioInmelden(prioFC02hd, SH[hprioin02hd], iInstPrioriteitsNiveau[prioFC02hd], iInstPrioriteitsOpties[prioFC02hd], 0, 0);
    PrioInmelden(prioFC05hd, SH[hprioin05hd], iInstPrioriteitsNiveau[prioFC05hd], iInstPrioriteitsOpties[prioFC05hd], 0, 0);
    PrioInmelden(prioFC08hd, SH[hprioin08hd], iInstPrioriteitsNiveau[prioFC08hd], iInstPrioriteitsOpties[prioFC08hd], 0, 0);
    PrioInmelden(prioFC41bus, SH[hprioin41bus], iInstPrioriteitsNiveau[prioFC41bus], iInstPrioriteitsOpties[prioFC41bus], 0, 0);

    /* Prioriteit-uitmeldingen */
    PrioUitmelden(prioFC02hd, SH[hpriouit02hd]);
    PrioUitmelden(prioFC05hd, SH[hpriouit05hd]);
    PrioUitmelden(prioFC08hd, SH[hpriouit08hd]);
    PrioUitmelden(prioFC41bus, SH[hpriouit41bus]);



    /* Inmelding fc02 type Nood- en hulpdienst */
    IH[hprioin02hd] = IH[hprioin02hdkar] = FALSE;
    if (SCH[schprioin02hdkar])
    {
        IH[hprioin02hdkar] = 
            DSIMelding_HD_V1(2, CIF_DSIN, (!SCH[schchecksirene02hd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR)) ||
            DSIMelding_HD_V1(3, CIF_DSIN, (!SCH[schchecksirene02hd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR));
    }
    IH[hprioin02hd] = IH[hprioin02hdkar];

    /* Uitmelding fc02 type Nood- en hulpdienst */
    IH[hpriouit02hd] = IH[hpriouit02hdkar] = FALSE;
    if (SCH[schpriouit02hdkar])
    {
        IH[hpriouit02hdkar] = 
            DSIMelding_HD_V1(2, CIF_DSUIT, (!SCH[schchecksirene02hd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR)) ||
            DSIMelding_HD_V1(3, CIF_DSUIT, (!SCH[schchecksirene02hd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR));
    }
    IH[hpriouit02hd] = IH[hpriouit02hdkar];

    /* Inmelding fc05 type Nood- en hulpdienst */
    IH[hprioin05hd] = IH[hprioin05hdkar] = IH[hprioin05hdopti051] = FALSE;
    if (SCH[schprioin05hdkar])
    {
        IH[hprioin05hdkar] = 
            DSIMelding_HD_V1(5, CIF_DSIN, (!SCH[schchecksirene05hd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR));
    }
    if (SCH[schprioin05hdopti051DB])
    {
        IH[hprioin05hdopti051] = SCH[schprioin05hdopti] && !C[cvc05hd] && DB[d051];
        RT[tprioin05hdopti051] = G[fc05] && C[cvc05hd];
    }
    IH[hprioin05hd] = IH[hprioin05hdkar] || IH[hprioin05hdopti051];

    /* Uitmelding fc05 type Nood- en hulpdienst */
    IH[hpriouit05hd] = IH[hpriouit05hdkar] = IH[hpriouit05hdopti052] = FALSE;
    if (SCH[schpriouit05hdkar])
    {
        IH[hpriouit05hdkar] = 
            DSIMelding_HD_V1(5, CIF_DSUIT, (!SCH[schchecksirene05hd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR));
    }
    if (SCH[schpriouit05hdopti052DB])
    {
        IH[hpriouit05hdopti052] = SCH[schprioin05hdopti] && !TDH[d052] && TDH_old[d052];
    }
    IH[hpriouit05hd] = IH[hpriouit05hdkar] || IH[hpriouit05hdopti052];

    /* Inmelding fc08 type Nood- en hulpdienst */
    IH[hprioin08hd] = IH[hprioin08hddet081] = FALSE;
    if (SCH[schprioin08hddet081SD] && (CIF_IS[d081] < CIF_DET_STORING))
    {
        IH[hprioin08hddet081] = SD[d081];
    }
    IH[hprioin08hd] = IH[hprioin08hddet081];

    /* Uitmelding fc08 type Nood- en hulpdienst */
    IH[hpriouit08hd] = IH[hpriouit08hddet082] = FALSE;
    if (SCH[schpriouit08hddet082ED] && (CIF_IS[d082] < CIF_DET_STORING))
    {
        IH[hpriouit08hddet082] = ED[d082];
    }
    IH[hpriouit08hd] = IH[hpriouit08hddet082];

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

    /* herstarten FB_timer bij in- of uitmelding HD of einde ingreep (door groenbewaking) */
    RTFB &= ~PRIO_RTFB_BIT;
    if (IH[hprioin02hd]|| IH[hpriouit02hd] || EH[hprio02hd] ||
        IH[hprioin05hd]|| IH[hpriouit05hd] || EH[hprio05hd] ||
        IH[hprioin08hd]|| IH[hpriouit08hd] || EH[hprio08hd])
    {
        RTFB |= PRIO_RTFB_BIT;
    }

    /* Bijhouden melding en ondergedrag KAR */
    RT[tkarmelding] = CIF_DSIWIJZ != 0 && CIF_DSI[CIF_DSI_LUS] == 0;
    RT[tkarog] = T[tkarmelding] || !startkarog;
    if (!startkarog) startkarog = TRUE;
    /* Doorzetten HD inmeldingen */
    IH[hprioin05hd] |= IH[hprioin02hd] && SCH[schpriomr05hdmet02hd]; IH[hpriouit05hd] |= IH[hpriouit02hd] && SCH[schpriomr05hdmet02hd];
    IH[hprioin02hd] |= IH[hprioin05hd] && SCH[schpriomr02hdmet05hd]; IH[hpriouit02hd] |= IH[hpriouit05hd] && SCH[schpriomr02hdmet05hd];
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
    boolv isHD = FALSE;

    /* Bepalen of een HD ingreep actief is */
    isHD = C[cvc02hd] && !BL[fc02] || C[cvc05hd] && !BL[fc05] || C[cvc08hd] && !BL[fc08];

    /* Blokkeren alle langzaam verkeer (tevens niet-conflicten) */
    if (isHD)
    {
    }


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
    PrioCcolElementen(prioFC02hd, tgb02hd, trt02hd, hprio02hd, cvc02hd, tblk02hd);
    PrioCcolElementen(prioFC05hd, tgb05hd, trt05hd, hprio05hd, cvc05hd, tblk05hd);
    PrioCcolElementen(prioFC08hd, tgb08hd, trt08hd, hprio08hd, cvc08hd, tblk08hd);
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
    if (SD[ddummykarin02hd]) set_DSI_message(NG, CIF_POL, 2, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], CIF_SIR);
    if (SD[ddummykaruit02hd] && C[cvc02hd]) set_DSI_message(NG, CIF_POL, 2, CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], CIF_SIR);
    if (SD[ddummykarin05hd]) set_DSI_message(NG, CIF_POL, 5, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], CIF_SIR);
    if (SD[ddummykaruit05hd] && C[cvc05hd]) set_DSI_message(NG, CIF_POL, 5, CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], CIF_SIR);
    if (SD[ddummykarin41bus]) set_DSI_message(NG, CIF_BUS, 41, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykaruit41bus] && C[cvc41bus]) set_DSI_message(NG, CIF_BUS, 41, CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);

}
#endif

#ifdef PRIO_ADDFILE
    #include "GMprio.add"
#endif /* PRIO_ADDFILE */
