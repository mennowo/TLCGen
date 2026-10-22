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
    iFC_PRIOix[prioFC41bus] = fc41;
    iFC_PRIOix[prioFC42bus] = fc42;

    /* Index van de groenbewakingstimer */
    iT_GBix[prioFC41bus] = tgb41bus;
    iT_GBix[prioFC42bus] = tgb42bus;

    /* Index van het hulpelement voor de ingreep */
    iH_PRIOix[prioFC41bus] = hprio41bus;
    iH_PRIOix[prioFC42bus] = hprio42bus;

    /* Prioriteitsniveau */
    iInstPrioriteitsNiveau[prioFC41bus] = PRM[prmprio41bus]/1000L;
    iInstPrioriteitsNiveau[prioFC42bus] = PRM[prmprio42bus]/1000L;

    /* Prioriteitsopties */
    iInstPrioriteitsOpties[prioFC41bus] = BepaalPrioriteitsOpties(prmprio41bus);
    iInstPrioriteitsOpties[prioFC42bus] = BepaalPrioriteitsOpties(prmprio42bus);

    /* Groenbewakingstijd */
    iGroenBewakingsTijd[prioFC41bus] = T_max[tgb41bus];
    iGroenBewakingsTijd[prioFC42bus] = T_max[tgb42bus];

    /* Ongehinderde rijtijd */
    iRTSOngehinderd[prioFC41bus] = PRM[prmrto41bus];
    iRTSOngehinderd[prioFC42bus] = PRM[prmrto42bus];

    /* Beperkt gehinderde rijtijd */
    iRTSBeperktGehinderd[prioFC41bus] = PRM[prmrtbg41bus];
    iRTSBeperktGehinderd[prioFC42bus] = PRM[prmrtbg42bus];

    /* Gehinderde rijtijd */
    iRTSGehinderd[prioFC41bus] = PRM[prmrtg41bus];
    iRTSGehinderd[prioFC42bus] = PRM[prmrtg42bus];

    /* Ondermaximum */
    iOnderMaximum[prioFC41bus] = PRM[prmomx41bus];
    iOnderMaximum[prioFC42bus] = PRM[prmomx42bus];

    /* Blokkeringstijd */
    iBlokkeringsTijd[prioFC41bus] = T_max[tblk41bus];
    iBlokkeringsTijd[prioFC42bus] = T_max[tblk42bus];

    /* Na aanspreken groenbewaking wordt de selectieve 
	   detectie niet langer betrouwbaar gevonden */
    iSelDetFoutNaGB[prioFC41bus] = PRM[prmupinagb41bus];
    iSelDetFoutNaGB[prioFC42bus] = PRM[prmupinagb42bus];

    /* =============================== */
    /* Instellingen overige richtingen */
    /* =============================== */
   
    /* Maximale wachttijd */
    iMaximumWachtTijd[fc02] = PRM[prmmwta];
    iMaximumWachtTijd[fc05] = PRM[prmmwta];
    iMaximumWachtTijd[fc41] = PRM[prmmwta];
    iMaximumWachtTijd[fc42] = PRM[prmmwta];

    /* Als een richting minder groen heeft gehad door afkappen
       dan deze instelling, dan mag de richting nog een keer
       primair realiseren (terugkomen). */
    iInstPercMaxGroenTijdTerugKomen[fc02] = PRM[prmpmgt02];
    iInstPercMaxGroenTijdTerugKomen[fc05] = PRM[prmpmgt05];
    iInstPercMaxGroenTijdTerugKomen[fc41] = PRM[prmpmgt41];
    iInstPercMaxGroenTijdTerugKomen[fc42] = PRM[prmpmgt42];

    /* De minimale groentijd die een richting krijgt als
       deze mag terugkomen. */
    iInstMinTerugKomGroenTijd[fc02] = PRM[prmognt02];
    iInstMinTerugKomGroenTijd[fc05] = PRM[prmognt05];
    iInstMinTerugKomGroenTijd[fc41] = PRM[prmognt41];
    iInstMinTerugKomGroenTijd[fc42] = PRM[prmognt42];

    /* Aantal malen niet afkappen */
    iInstAantalMalenNietAfkappen[fc02] = PRM[prmnofm02];
    iInstAantalMalenNietAfkappen[fc05] = PRM[prmnofm05];
    iInstAantalMalenNietAfkappen[fc41] = PRM[prmnofm41];
    iInstAantalMalenNietAfkappen[fc42] = PRM[prmnofm42];

    /* Onder deze groentijd mag een richting niet worden
       afgekapt, tenzij zich een nooddienst heeft ingemeld. */
    iAfkapGroenTijd[fc02] = PRM[prmmgcov02];
    iAfkapGroenTijd[fc05] = PRM[prmmgcov05];
    iAfkapGroenTijd[fc41] = PRM[prmmgcov41];
    iAfkapGroenTijd[fc42] = PRM[prmmgcov42];

    /* Als een richting minder groen heeft gehad dan
       dit percentage van de maximum groentijd, dan
       mag de richting niet worden afgekapt, tenzij
       zich een nooddienst heeft ingemeld. */
    iPercGroenTijd[fc02] = PRM[prmpmgcov02];
    iPercGroenTijd[fc05] = PRM[prmpmgcov05];
    iPercGroenTijd[fc41] = PRM[prmpmgcov41];
    iPercGroenTijd[fc42] = PRM[prmpmgcov42];

    /* Na te zijn afgekapt, wordt het percentage
       van de maximumgroentijd verhoogd met dit ophoogpercentage. */
    iInstOphoogPercentageMG[fc02] = PRM[prmohpmg02];
    iInstOphoogPercentageMG[fc05] = PRM[prmohpmg05];
    iInstOphoogPercentageMG[fc41] = PRM[prmohpmg41];
    iInstOphoogPercentageMG[fc42] = PRM[prmohpmg42];

    /* Benodigde ruimte voor alternatieve realisatie tijdens een OV ingreep */
    iPRM_ALTP[fc02] = PRM[prmaltp02];
    iPRM_ALTP[fc05] = PRM[prmaltp05];
    iPRM_ALTP[fc41] = PRM[prmaltp41];
    iPRM_ALTP[fc42] = PRM[prmaltp42];

    /* Richting mag alternatief realiseren tijdens een OV ingreep */
    iSCH_ALTG[fc02] = SCH[schaltg02];
    iSCH_ALTG[fc05] = SCH[schaltg05];
    iSCH_ALTG[fc41] = SCH[schaltg41];
    iSCH_ALTG[fc42] = SCH[schaltg42];

    /* Alternatieve realisatie mag worden
       afgekapt tijdens een OV ingreep als
       deze groentijd is gemaakt */
    iInstAfkapGroenAlt[fc02] = PRM[prmaltg02];
    iInstAfkapGroenAlt[fc05] = PRM[prmaltg05];
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

    PrioRijTijdScenario(prioFC42bus, d421, d422, tbtovg42bus);




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
    PrioInmelden(prioFC42bus, SH[hprioin42bus], iInstPrioriteitsNiveau[prioFC42bus], iInstPrioriteitsOpties[prioFC42bus], 0, 0);

    /* Prioriteit-uitmeldingen */
    PrioUitmelden(prioFC41bus, SH[hpriouit41bus]);
    PrioUitmelden(prioFC42bus, SH[hpriouit42bus]);



    /* Inmelding fc41 type Bus */
    IH[hprioin41bus] = IH[hprioin41buskar] = IH[hprioin41bussdsd41in] = IH[hprioin41busdet412413] = IH[hprioin41busdet413] = IH[hprioin41businpov41in] = IH[hprioin41businpov41inov41uit] = IH[hprioin41busvecio411] = IH[hprioin41busopti414] = FALSE;
    if (SCH[schprioin41buskar])
    {
        IH[hprioin41buskar] = RT[tprioin41buskar] = !T[tprioin41buskar] && 
            DSIMeldingPRIO_V1(0, PRM[prmvtgcat41bus], TRUE, 41,TRUE, CIF_DSIN, TRUE);
    }
    if (SCH[schprioin41bussdsd41inSD])
    {
        IH[hprioin41bussdsd41in] = 
            DSIMeldingPRIO_V1(DSD41IN, PRM[prmvtgcat41bus], FALSE, NG, SCH[schcheckdstype], CIF_DSIN, TRUE);
    }
    if (SCH[schprioin41busdet412D413DB] && (CIF_IS[d412] < CIF_DET_STORING) && (CIF_IS[d413] < CIF_DET_STORING))
    {
        IH[hprioin41busdet412413] = !C[cvc41bus] && D[d412] && DB[d413];
    }
    if (SCH[schprioin41busdet413SDB] && (CIF_IS[d413] < CIF_DET_STORING))
    {
        IH[hprioin41busdet413] = RT[tprioin41busdet413] = !T[tprioin41busdet413] && !DB_old[d413] && DB[d413];
    }
    else if (SCH[schprioin41busdet414ETDH] && (CIF_IS[d414] < CIF_DET_STORING))
    {
        IH[hprioin41busdet413] = TDH_old[d414] && !TDH[d414];
    }
    if (SCH[schprioin41businpov41inSIS])
    {
        IH[hprioin41businpov41in] = IS[isov41in] && !IS_old[isov41in];
    }
    if (SCH[schprioin41businpov41inISov41uitSIS])
    {
        IH[hprioin41businpov41inov41uit] = IS[isov41in] && IS[isov41uit] && !IS_old[isov41uit];
    }
    if (SCH[schprioin41busvecio411SD] && (CIF_IS[d411] < CIF_DET_STORING))
    {
        IH[hprioin41busvecio411] =  SD[d411];
    }
    if (SCH[schprioin41busopti414DB])
    {
        IH[hprioin41busopti414] = !T[tprioin41busopti414] && !C[cvc41bus] && DB[d414];
        RT[tprioin41busopti414] = G[fc41] && C[cvc41bus];
    }
    IH[hprioin41bus] = IH[hprioin41buskar] || IH[hprioin41bussdsd41in] || IH[hprioin41busdet412413] || IH[hprioin41busdet413] || IH[hprioin41businpov41in] || IH[hprioin41businpov41inov41uit] || IH[hprioin41busvecio411] || IH[hprioin41busopti414];

    /* Uitmelding fc41 type Bus */
    IH[hpriouit41bus] = IH[hpriouit41buskar] = IH[hpriouit41bussdsd41uit] = IH[hpriouit41busdet411] = IH[hpriouit41busdet412413] = IH[hpriouit41businpov41in] = IH[hpriouit41businpov41uitov41in] = IH[hpriouit41busvecio414] = IH[hpriouit41busopti414] = FALSE;
    if (SCH[schpriouit41buskar])
    {
        IH[hpriouit41buskar] = 
            DSIMeldingPRIO_V2(fc41, prioFC41bus, 0, PRM[prmvtgcat41bus], TRUE, 41,TRUE, CIF_DSUIT, TRUE);
    }
    if (SCH[schpriouit41bussdsd41uitSD])
    {
        IH[hpriouit41bussdsd41uit] = 
            DSIMeldingPRIO_V1(DSD41UIT, PRM[prmvtgcat41bus], FALSE, NG, SCH[schcheckdstype], CIF_DSIN, TRUE);
    }
    if (SCH[schpriouit41busdet411D] && (CIF_IS[d411] < CIF_DET_STORING))
    {
        IH[hpriouit41busdet411] = D[d411];
    }
    if (SCH[schpriouit41busdet412ED413ETDH] && (CIF_IS[d412] < CIF_DET_STORING) && (CIF_IS[d413] < CIF_DET_STORING))
    {
        IH[hpriouit41busdet412413] = ED[d412] && TDH_old[d413] && !TDH[d413];
    }
    if (SCH[schpriouit41businpov41inEIS])
    {
        IH[hpriouit41businpov41in] = IS_old[isov41in] && !IS[isov41in];
    }
    if (SCH[schpriouit41businpov41uitEISov41inIS])
    {
        IH[hpriouit41businpov41uitov41in] = IS_old[isov41uit] && !IS[isov41uit] && IS[isov41in];
    }
    if (SCH[schpriouit41busvecio414SD] && (CIF_IS[d414] < CIF_DET_STORING))
    {
        IH[hpriouit41busvecio414] =  SD[d414];
    }
    if (SCH[schpriouit41busopti414DB])
    {
        IH[hpriouit41busopti414] = !TDH[d414] && TDH_old[d414];
    }
    IH[hpriouit41bus] = IH[hpriouit41buskar] || IH[hpriouit41bussdsd41uit] || IH[hpriouit41busdet411] || IH[hpriouit41busdet412413] || IH[hpriouit41businpov41in] || IH[hpriouit41businpov41uitov41in] || IH[hpriouit41busvecio414] || IH[hpriouit41busopti414];

    /* Inmelding fc42 type Bus */
    IH[hprioin42bus] = IH[hprioin42busopti421] = FALSE;
    if (SCH[schprioin42busopti421DB])
    {
        IH[hprioin42busopti421] = !C[cvc42bus] && DB[d421];
    }
    IH[hprioin42bus] = IH[hprioin42busopti421];

    /* Uitmelding fc42 type Bus */
    IH[hpriouit42bus] = IH[hpriouit42busopti422] = IH[hpriouit42busopti423] = FALSE;
    if (SCH[schpriouit42busopti422DB])
    {
        IH[hpriouit42busopti422] = !TDH[d422] && TDH_old[d422];
    }
    if (SCH[schpriouit42busopti423DB])
    {
        IH[hpriouit42busopti423] = !TDH[d422] && TDH_old[d422];
    }
    IH[hpriouit42bus] = IH[hpriouit42busopti422] || IH[hpriouit42busopti423];


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
    PrioCcolElementen(prioFC41bus, tgb41bus, trt41bus, hprio41bus, cvc41bus, tblk41bus);
    PrioCcolElementen(prioFC42bus, tgb42bus, trt42bus, hprio42bus, cvc42bus, tblk42bus);
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
    if (SD[dsd41in]) set_DSI_message(DSD41IN, CIF_BUS, 41, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], NG);
    if (SD[dsd41uit] && C[cvc41bus]) set_DSI_message(DSD41UIT, CIF_BUS, 41, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], NG);

}
#endif

#ifdef PRIO_ADDFILE
    #include "GMprio.add"
#endif /* PRIO_ADDFILE */
