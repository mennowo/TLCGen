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
    iFC_PRIOix[prioFC22fiets] = fc22;
    iFC_PRIOix[prioFC24fiets] = fc24;

    /* Index van de groenbewakingstimer */
    iT_GBix[prioFC22fiets] = tgb22fiets;
    iT_GBix[prioFC24fiets] = tgb24fiets;

    /* Index van het hulpelement voor de ingreep */
    iH_PRIOix[prioFC22fiets] = hprio22fiets;
    iH_PRIOix[prioFC24fiets] = hprio24fiets;

    /* Prioriteitsniveau */
    iInstPrioriteitsNiveau[prioFC22fiets] = PRM[prmprio22fiets]/1000L;
    iInstPrioriteitsNiveau[prioFC24fiets] = PRM[prmprio24fiets]/1000L;

    /* Prioriteitsopties */
    iInstPrioriteitsOpties[prioFC22fiets] = BepaalPrioriteitsOpties(prmprio22fiets);
    iInstPrioriteitsOpties[prioFC24fiets] = BepaalPrioriteitsOpties(prmprio24fiets);

    /* Groenbewakingstijd */
    iGroenBewakingsTijd[prioFC22fiets] = T_max[tgb22fiets];
    iGroenBewakingsTijd[prioFC24fiets] = T_max[tgb24fiets];

    /* Ongehinderde rijtijd */
    iRTSOngehinderd[prioFC22fiets] = PRM[prmrto22fiets];
    iRTSOngehinderd[prioFC24fiets] = PRM[prmrto24fiets];

    /* Beperkt gehinderde rijtijd */
    iRTSBeperktGehinderd[prioFC22fiets] = PRM[prmrtbg22fiets];
    iRTSBeperktGehinderd[prioFC24fiets] = PRM[prmrtbg24fiets];

    /* Gehinderde rijtijd */
    iRTSGehinderd[prioFC22fiets] = PRM[prmrtg22fiets];
    iRTSGehinderd[prioFC24fiets] = PRM[prmrtg24fiets];

    /* Ondermaximum */
    iOnderMaximum[prioFC22fiets] = PRM[prmomx22fiets];
    iOnderMaximum[prioFC24fiets] = PRM[prmomx24fiets];

    /* Blokkeringstijd */
    iBlokkeringsTijd[prioFC22fiets] = T_max[tblk22fiets];
    iBlokkeringsTijd[prioFC24fiets] = T_max[tblk24fiets];

    /* Na aanspreken groenbewaking wordt de selectieve 
	   detectie niet langer betrouwbaar gevonden */
    iSelDetFoutNaGB[prioFC22fiets] = PRM[prmupinagb22fiets];
    iSelDetFoutNaGB[prioFC24fiets] = PRM[prmupinagb24fiets];

    /* =============================== */
    /* Instellingen overige richtingen */
    /* =============================== */
   
    /* Maximale wachttijd */
    iMaximumWachtTijd[fc02] = PRM[prmmwta];
    iMaximumWachtTijd[fc22] = PRM[prmmwtfts];
    iMaximumWachtTijd[fc24] = PRM[prmmwtfts];
    iMaximumWachtTijd[fc31] = PRM[prmmwtvtg];

    /* Als een richting minder groen heeft gehad door afkappen
       dan deze instelling, dan mag de richting nog een keer
       primair realiseren (terugkomen). */
    iInstPercMaxGroenTijdTerugKomen[fc02] = PRM[prmpmgt02];
    iInstPercMaxGroenTijdTerugKomen[fc22] = PRM[prmpmgt22];
    iInstPercMaxGroenTijdTerugKomen[fc24] = PRM[prmpmgt24];
    iInstPercMaxGroenTijdTerugKomen[fc31] = PRM[prmpmgt31];

    /* De minimale groentijd die een richting krijgt als
       deze mag terugkomen. */
    iInstMinTerugKomGroenTijd[fc02] = PRM[prmognt02];
    iInstMinTerugKomGroenTijd[fc22] = PRM[prmognt22];
    iInstMinTerugKomGroenTijd[fc24] = PRM[prmognt24];
    iInstMinTerugKomGroenTijd[fc31] = PRM[prmognt31];

    /* Aantal malen niet afkappen */
    iInstAantalMalenNietAfkappen[fc02] = PRM[prmnofm02];
    iInstAantalMalenNietAfkappen[fc22] = PRM[prmnofm22];
    iInstAantalMalenNietAfkappen[fc24] = PRM[prmnofm24];

    /* Onder deze groentijd mag een richting niet worden
       afgekapt, tenzij zich een nooddienst heeft ingemeld. */
    iAfkapGroenTijd[fc02] = PRM[prmmgcov02];
    iAfkapGroenTijd[fc22] = PRM[prmmgcov22];
    iAfkapGroenTijd[fc24] = PRM[prmmgcov24];
    iAfkapGroenTijd[fc31] = 0;

    /* Als een richting minder groen heeft gehad dan
       dit percentage van de maximum groentijd, dan
       mag de richting niet worden afgekapt, tenzij
       zich een nooddienst heeft ingemeld. */
    iPercGroenTijd[fc02] = PRM[prmpmgcov02];
    iPercGroenTijd[fc22] = PRM[prmpmgcov22];
    iPercGroenTijd[fc24] = PRM[prmpmgcov24];
    iPercGroenTijd[fc31] = 100; /* Voetgangers mogen niet worden afgekapt. */

    /* Na te zijn afgekapt, wordt het percentage
       van de maximumgroentijd verhoogd met dit ophoogpercentage. */
    iInstOphoogPercentageMG[fc02] = PRM[prmohpmg02];
    iInstOphoogPercentageMG[fc22] = PRM[prmohpmg22];
    iInstOphoogPercentageMG[fc24] = PRM[prmohpmg24];
    iInstOphoogPercentageMG[fc31] = 0;

    /* Benodigde ruimte voor alternatieve realisatie tijdens een OV ingreep */
    iPRM_ALTP[fc02] = PRM[prmaltp02];
    iPRM_ALTP[fc22] = PRM[prmaltp22];
    iPRM_ALTP[fc24] = PRM[prmaltp24];
    iPRM_ALTP[fc31] = PRM[prmaltp31];

    /* Richting mag alternatief realiseren tijdens een OV ingreep */
    iSCH_ALTG[fc02] = SCH[schaltg02];
    iSCH_ALTG[fc22] = SCH[schaltg22];
    iSCH_ALTG[fc24] = SCH[schaltg24];
    iSCH_ALTG[fc31] = SCH[schaltg31];

    /* Alternatieve realisatie mag worden
       afgekapt tijdens een OV ingreep als
       deze groentijd is gemaakt */
    iInstAfkapGroenAlt[fc02] = PRM[prmaltg02];
    iInstAfkapGroenAlt[fc22] = PRM[prmaltg22];
    iInstAfkapGroenAlt[fc24] = PRM[prmaltg24];
    iInstAfkapGroenAlt[fc31] = PRM[prmaltg31];

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
    PrioRijTijdScenario(prioFC22fiets, d221, NG, NG);

    PrioRijTijdScenario(prioFC24fiets, d241, NG, NG);




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
    PrioInmelden(prioFC22fiets, SH[hprioin22fiets], iInstPrioriteitsNiveau[prioFC22fiets], iInstPrioriteitsOpties[prioFC22fiets], 0, 0);
    PrioInmelden(prioFC24fiets, SH[hprioin24fiets], iInstPrioriteitsNiveau[prioFC24fiets], iInstPrioriteitsOpties[prioFC24fiets], 0, 0);

    /* Prioriteit-uitmeldingen */
    PrioUitmelden(prioFC22fiets, SH[hpriouit22fiets]);
    PrioUitmelden(prioFC24fiets, SH[hpriouit24fiets]);



    /* Inmelding fc22 type Fiets */
    fietsprio_update(fc22, d221, cftsvtg22fietsfiets, cftscyc22fietsfiets,SH[hprioin22fietsfiets221], ML);
    IH[hprioin22fiets] = IH[hprioin22fietsfiets221] = FALSE;
    if (SCH[schprioin22fietsfiets221])
    {
        IH[hprioin22fietsfiets221] = !C[cvc22fiets] && 
            fietsprio_inmelding(fc22, d221, cftsvtg22fietsfiets, cftscyc22fietsfiets, prmftsblok22fietsfiets, prmftsmaxpercyc22fietsfiets, prmftsminvtg22fietsfiets, prmftsminwt22fietsfiets, ML, NG, NG);
    }
    IH[hprioin22fiets] = IH[hprioin22fietsfiets221];

    /* Uitmelding fc22 type Fiets */
    IH[hpriouit22fiets] = IH[hpriouit22fietsfiets221] = FALSE;
    if (SCH[schpriouit22fietsfiets221])
    {
        IH[hpriouit22fietsfiets221] = C[cvc22fiets] && G[fc22] && (T[tgb22fiets] && (T_timer[tgb22fiets] > 1));
    }
    IH[hpriouit22fiets] = IH[hpriouit22fietsfiets221];

    /* Inmelding fc24 type Fiets */
    fietsprio_update(fc24, NG, NG, cftscyc24fietsfiets,SH[hprioin24fietsfiets], ML);
    IH[hprioin24fiets] = IH[hprioin24fietsfiets] = FALSE;
    if (SCH[schprioin24fietsfiets])
    {
        IH[hprioin24fietsfiets] = !C[cvc24fiets] && 
            fietsprio_inmelding(fc24, NG, NG, cftscyc24fietsfiets, prmftsblok24fietsfiets, prmftsmaxpercyc24fietsfiets, NG, prmftsminwt24fietsfiets, ML, NG, NG);
    }
    IH[hprioin24fiets] = IH[hprioin24fietsfiets];

    /* Uitmelding fc24 type Fiets */
    IH[hpriouit24fiets] = IH[hpriouit24fietsdet241] = FALSE;
    if (SCH[schpriouit24fietsdet241ED] && (CIF_IS[d241] < CIF_DET_STORING))
    {
        IH[hpriouit24fietsdet241] = ED[d241];
    }
    IH[hpriouit24fiets] = IH[hpriouit24fietsdet241];

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
    PrioCcolElementen(prioFC22fiets, tgb22fiets, trt22fiets, hprio22fiets, cvc22fiets, tblk22fiets);
    PrioCcolElementen(prioFC24fiets, tgb24fiets, trt24fiets, hprio24fiets, cvc24fiets, tblk24fiets);
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
