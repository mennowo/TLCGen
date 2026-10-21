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
    #define PRIO_CHECK_WAGENNMR /* check op wagendienstnummer          */
    #include "extra_func_prio.h"

boolv vertraag_kar_uitm[prioFCMAX];


extern mulv DB_old[];
extern mulv TDH_old[];

#include "prio.c"

/* Variabele tbv start KAR ondergedrag timer bij starten regeling */
static char startkarog = FALSE;

/* Variabelen tbv registreren stiptheid bij inmelding via KAR: tbv bepalen prioriteit in OV.ADD */
int iKARInSTP41tram[MAX_AANTAL_INMELDINGEN] = { 0 }; int iAantInm41tram = 0;

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
    iFC_PRIOix[prioFC42bus] = fc42;

    /* Index van de groenbewakingstimer */
    iT_GBix[prioFC41tram] = tgb41tram;
    iT_GBix[prioFC42bus] = tgb42bus;

    /* Index van het hulpelement voor de ingreep */
    iH_PRIOix[prioFC41tram] = hprio41tram;
    iH_PRIOix[prioFC42bus] = hprio42bus;

    /* Prioriteitsniveau */
    iInstPrioriteitsNiveau[prioFC41tram] = PRM[prmprio41tram]/1000L;
    iInstPrioriteitsNiveau[prioFC42bus] = PRM[prmprio42bus]/1000L;

    /* Prioriteitsopties */
    iInstPrioriteitsOpties[prioFC41tram] = BepaalPrioriteitsOpties(prmprio41tram);
    iInstPrioriteitsOpties[prioFC42bus] = BepaalPrioriteitsOpties(prmprio42bus);

    /* Groenbewakingstijd */
    iGroenBewakingsTijd[prioFC41tram] = T_max[tgb41tram];
    iGroenBewakingsTijd[prioFC42bus] = T_max[tgb42bus];

    /* Ongehinderde rijtijd */
    iRTSOngehinderd[prioFC41tram] = PRM[prmrto41tram];
    iRTSOngehinderd[prioFC42bus] = PRM[prmrto42bus];

    /* Beperkt gehinderde rijtijd */
    iRTSBeperktGehinderd[prioFC41tram] = PRM[prmrtbg41tram];
    iRTSBeperktGehinderd[prioFC42bus] = PRM[prmrtbg42bus];

    /* Gehinderde rijtijd */
    iRTSGehinderd[prioFC41tram] = PRM[prmrtg41tram];
    iRTSGehinderd[prioFC42bus] = PRM[prmrtg42bus];

    /* Ondermaximum */
    iOnderMaximum[prioFC41tram] = PRM[prmomx41tram];
    iOnderMaximum[prioFC42bus] = PRM[prmomx42bus];

    /* Blokkeringstijd */
    iBlokkeringsTijd[prioFC41tram] = T_max[tblk41tram];
    iBlokkeringsTijd[prioFC42bus] = T_max[tblk42bus];

    /* Na aanspreken groenbewaking wordt de selectieve 
	   detectie niet langer betrouwbaar gevonden */
    iSelDetFoutNaGB[prioFC41tram] = PRM[prmupinagb41tram];
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
    if (IH[hwissel41tram])    {
        PrioRijTijdScenario(prioFC41tram, d411, NG, NG);
    }
    else
    {
        PrioRijTijdScenario(prioFC41tram, NG, NG, NG);
    }
    PrioRijTijdScenario(prioFC42bus, d421, d422, tbtovg42bus);


    if (SCH[schvi41tram] && DB[d411] && C[cvc41tram] && (iRijTimer[prioFC41tram] < iRijTijd[prioFC41tram]) && IH[hwissel41tram]) iRijTijd[prioFC41tram] = 0;


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
    PrioInmelden(prioFC42bus, SH[hprioin42bus], iInstPrioriteitsNiveau[prioFC42bus], iInstPrioriteitsOpties[prioFC42bus], 0, 0);

    /* Prioriteit-uitmeldingen */
    PrioUitmelden(prioFC41tram, SH[hpriouit41tram]);
    PrioUitmelden(prioFC42bus, SH[hpriouit42bus]);



    /* Inmelding fc41 type Tram */
    IH[hprioin41tram] = IH[hprioin41tramkar] = IH[hprioin41tramdet411] = FALSE;
    if (SCH[schprioin41tramkar])
    {
        IH[hprioin41tramkar] = !T[tprioin41tram] && IH[hwissel41tram] && 
            DSIMeldingPRIO_V1(0, PRM[prmvtgcat41tram], TRUE, 41,TRUE, CIF_DSIN, DSIMeldingPRIO_LijnNummer_V1(prmallelijnen41tram, 2) && WDNST_check_in(fc41));
    }
    if (SCH[schprioin41tramdet411SD] && (CIF_IS[d411] < CIF_DET_STORING))
    {
        IH[hprioin41tramdet411] = RT[tprioin41tramdet411] = !T[tprioin41tram] && !T[tprioin41tramdet411] && IH[hwissel41tram] && R[fc41] && !T[tovminrood41tram] && SD[d411];
    }
    IH[hprioin41tram] = RT[tprioin41tram] = IH[hperiodochtend] && (IH[hprioin41tramkar] || IH[hprioin41tramdet411]);

    /* Uitmelding fc41 type Tram */
    IH[hpriouit41tram] = IH[hpriouit41tramkar] = IH[hpriouit41tramdet412] = FALSE;
    if (SCH[schpriouit41tramkar])
    {
        IH[hpriouit41tramkar] = !T[tpriouit41tram] && 
            DSIMeldingPRIO_V2(fc41, prioFC41tram, 0, PRM[prmvtgcat41tram], TRUE, 41,TRUE, CIF_DSUIT, DSIMeldingPRIO_LijnNummer_V1(prmallelijnen41tram, 2) && WDNST_check_uit(fc41));
    }
    if (SCH[schpriouit41tramdet412ED] && (CIF_IS[d412] < CIF_DET_STORING))
    {
        IH[hpriouit41tramdet412] = !T[tpriouit41tram] && ED[d412];
    }
    IH[hpriouit41tram] = RT[tpriouit41tram] = IH[hpriouit41tramkar] || IH[hpriouit41tramdet412];

    /* Inmelding fc42 type Bus */
    IH[hprioin42bus] = IH[hprioin42bussdsd42] = FALSE;
    if (SCH[schprioin42bussdsd42SD])
    {
        IH[hprioin42bussdsd42] = !T[tprioin42bus] && 
            DSIMeldingPRIO_V1(DSD42, PRM[prmvtgcat42bus], FALSE, NG, SCH[schcheckdstype], CIF_DSIN, DSIMeldingPRIO_LijnNummerEnRitCategorie_V1(prmallelijnen42bus, 1));
    }
    IH[hprioin42bus] = RT[tprioin42bus] = IH[hprioin42bussdsd42];

    /* Uitmelding fc42 type Bus */
    IH[hpriouit42bus] = IH[hpriouit42bussdsd42] = FALSE;
    if (SCH[schpriouit42bussdsd42SD])
    {
        IH[hpriouit42bussdsd42] = 
            DSIMeldingPRIO_V1(DSD42, PRM[prmvtgcat42bus], FALSE, NG, SCH[schcheckdstype], CIF_DSUIT, DSIMeldingPRIO_LijnNummerEnRitCategorie_V1(prmallelijnen42bus, 1));
    }
    IH[hpriouit42bus] = IH[hpriouit42bussdsd42];

    /* Bijhouden stiptheidsklassen ingemelde voertuigen */
    /* Bij inmelding: registeren stiptheidsklasse achterste voertuig */
    TrackStiptObvTSTP(hprioin41tram, hpriouit41tram, &iAantInm41tram, iKARInSTP41tram, hprio41tram, PRM[prmOVtstpgrensvroeg], PRM[prmOVtstpgrenslaat]);
    MM[mstp41tram] = (mulv) (iAantInm41tram > 0 ? iKARInSTP41tram[0] : 0);


    /* Bijhouden melding en ondergedrag KAR */
    RT[tkarmelding] = CIF_DSIWIJZ != 0 && CIF_DSI[CIF_DSI_LUS] == 0;
    RT[tkarog] = T[tkarmelding] || !startkarog;
    if (!startkarog) startkarog = TRUE;

    /* Noodaanvragen obv koplus */
    if (!C[cvc41tram] && DB[d411] && R[fc41] && !TRG[fc41] && !T[tovminrood41tram] && IH[hwissel41tram]) A[fc41] |= BIT6;
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
    /* Geconditioneerde prioriteit instellen */
    IH[hstp41tram] = SCH[schovstipt41tram];
    if (IH[hstp41tram] && (MM[mstp41tram] == CIF_TE_VROEG || !MM[mstp41tram])) iPrioriteitsOpties[prioFC41tram] = BepaalPrioriteitsOpties(prmovstipttevroeg41tram);
    if (IH[hstp41tram] && (MM[mstp41tram] == CIF_OP_TIJD || !MM[mstp41tram])) iPrioriteitsOpties[prioFC41tram] = BepaalPrioriteitsOpties(prmovstiptoptijd41tram);
    if (IH[hstp41tram] && (MM[mstp41tram] == CIF_TE_LAAT || !MM[mstp41tram])) iPrioriteitsOpties[prioFC41tram] = BepaalPrioriteitsOpties(prmovstipttelaat41tram);

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
    if (SD[ddummykarin41tram]) set_DSI_message(NG, CIF_TRAM, 41, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykaruit41tram] && C[cvc41tram]) set_DSI_message(NG, CIF_TRAM, 41, CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[dsd42]) set_DSI_message(DSD42, CIF_BUS, 42, CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], NG);

}
#endif

#ifdef PRIO_ADDFILE
    #include "GMprio.add"
#endif /* PRIO_ADDFILE */
