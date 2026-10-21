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
    iFC_PRIOix[prioFC241bus] = fc241;
    iFC_PRIOix[prioFC41bus] = fc41;
    iFC_PRIOix[prioFC05hd] = fc05;
    iFC_PRIOix[hdFC02] = fc02;

    /* Index van de groenbewakingstimer */
    iT_GBix[prioFC241bus] = tgb241bus;
    iT_GBix[prioFC41bus] = tgb41bus;
    iT_GBix[prioFC05hd] = tgb05hd;
    iT_GBix[hdFC02] = tgbhd02;

    /* Index van het hulpelement voor de ingreep */
    iH_PRIOix[prioFC241bus] = hprio241bus;
    iH_PRIOix[prioFC41bus] = hprio41bus;
    iH_PRIOix[prioFC05hd] = hprio05hd;
    iH_PRIOix[hdFC02] = hhd02;

    /* Prioriteitsniveau */
    iInstPrioriteitsNiveau[prioFC241bus] = PRM[prmprio241bus]/1000L;
    iInstPrioriteitsNiveau[prioFC41bus] = PRM[prmprio41bus]/1000L;
    iInstPrioriteitsNiveau[prioFC05hd] = PRM[prmprio05hd]/1000L;
    iInstPrioriteitsNiveau[hdFC02] = PRM[prmpriohd02]/1000L;

    /* Prioriteitsopties */
    iInstPrioriteitsOpties[prioFC241bus] = BepaalPrioriteitsOpties(prmprio241bus);
    iInstPrioriteitsOpties[prioFC41bus] = BepaalPrioriteitsOpties(prmprio41bus);
    iInstPrioriteitsOpties[prioFC05hd] = BepaalPrioriteitsOpties(prmprio05hd);
    iInstPrioriteitsOpties[hdFC02] = BepaalPrioriteitsOpties(prmpriohd02);

    /* Groenbewakingstijd */
    iGroenBewakingsTijd[prioFC241bus] = T_max[tgb241bus];
    iGroenBewakingsTijd[prioFC41bus] = T_max[tgb41bus];
    iGroenBewakingsTijd[prioFC05hd] = T_max[tgb05hd];
    iGroenBewakingsTijd[hdFC02] = T_max[tgbhd02];

    /* Ongehinderde rijtijd */
    iRTSOngehinderd[prioFC241bus] = PRM[prmrto241bus];
    iRTSOngehinderd[prioFC41bus] = PRM[prmrto41bus];
    iRTSOngehinderd[prioFC05hd] = PRM[prmrto05hd];
    iRTSOngehinderd[hdFC02] = PRM[prmrtohd02];

    /* Beperkt gehinderde rijtijd */
    iRTSBeperktGehinderd[prioFC241bus] = PRM[prmrtbg241bus];
    iRTSBeperktGehinderd[prioFC41bus] = PRM[prmrtbg41bus];
    iRTSBeperktGehinderd[prioFC05hd] = PRM[prmrtbg05hd];
    iRTSBeperktGehinderd[hdFC02] = PRM[prmrtbghd02];

    /* Gehinderde rijtijd */
    iRTSGehinderd[prioFC241bus] = PRM[prmrtg241bus];
    iRTSGehinderd[prioFC41bus] = PRM[prmrtg41bus];
    iRTSGehinderd[prioFC05hd] = PRM[prmrtg05hd];
    iRTSGehinderd[hdFC02] = PRM[prmrtghd02];

    /* Ondermaximum */
    iOnderMaximum[prioFC241bus] = PRM[prmomx241bus];
    iOnderMaximum[prioFC41bus] = PRM[prmomx41bus];
    iOnderMaximum[prioFC05hd] = PRM[prmomx05hd];
    iOnderMaximum[hdFC02] = 0;

    /* Blokkeringstijd */
    iBlokkeringsTijd[prioFC241bus] = T_max[tblk241bus];
    iBlokkeringsTijd[prioFC41bus] = T_max[tblk41bus];
    iBlokkeringsTijd[prioFC05hd] = T_max[tblk05hd];
    iBlokkeringsTijd[hdFC02] = 0;

    /* Na aanspreken groenbewaking wordt de selectieve 
	   detectie niet langer betrouwbaar gevonden */
    iSelDetFoutNaGB[prioFC241bus] = PRM[prmupinagb241bus];
    iSelDetFoutNaGB[prioFC41bus] = PRM[prmupinagb41bus];
    iSelDetFoutNaGB[prioFC05hd] = PRM[prmupinagb05hd];
    iSelDetFoutNaGB[hdFC02] = PRM[prmupinagbhd02];

    /* =============================== */
    /* Instellingen overige richtingen */
    /* =============================== */
   
    /* Maximale wachttijd */
    iMaximumWachtTijd[fc02] = PRM[prmmwta];
    iMaximumWachtTijd[fc05] = PRM[prmmwta];
    iMaximumWachtTijd[fc41] = PRM[prmmwta];
    iMaximumWachtTijd[fc241] = PRM[prmmwta];

    /* Als een richting minder groen heeft gehad door afkappen
       dan deze instelling, dan mag de richting nog een keer
       primair realiseren (terugkomen). */
    iInstPercMaxGroenTijdTerugKomen[fc02] = 0;
    iInstPercMaxGroenTijdTerugKomen[fc05] = 0;
    iInstPercMaxGroenTijdTerugKomen[fc41] = 0;
    iInstPercMaxGroenTijdTerugKomen[fc241] = 0;

    /* De minimale groentijd die een richting krijgt als
       deze mag terugkomen. */
    iInstMinTerugKomGroenTijd[fc02] = 0;
    iInstMinTerugKomGroenTijd[fc05] = 0;
    iInstMinTerugKomGroenTijd[fc41] = 0;
    iInstMinTerugKomGroenTijd[fc241] = 0;

    /* Aantal malen niet afkappen */
    iInstAantalMalenNietAfkappen[fc02] = 0;
    iInstAantalMalenNietAfkappen[fc05] = 0;
    iInstAantalMalenNietAfkappen[fc41] = 0;
    iInstAantalMalenNietAfkappen[fc241] = 0;

    /* Onder deze groentijd mag een richting niet worden
       afgekapt, tenzij zich een nooddienst heeft ingemeld. */
    iAfkapGroenTijd[fc02] = 0;
    iAfkapGroenTijd[fc05] = 0;
    iAfkapGroenTijd[fc41] = 0;
    iAfkapGroenTijd[fc241] = 0;

    /* Als een richting minder groen heeft gehad dan
       dit percentage van de maximum groentijd, dan
       mag de richting niet worden afgekapt, tenzij
       zich een nooddienst heeft ingemeld. */
    iPercGroenTijd[fc02] = 0;
    iPercGroenTijd[fc05] = 0;
    iPercGroenTijd[fc41] = 0;
    iPercGroenTijd[fc241] = 0;

    /* Na te zijn afgekapt, wordt het percentage
       van de maximumgroentijd verhoogd met dit ophoogpercentage. */
    iInstOphoogPercentageMG[fc02] = 0;
    iInstOphoogPercentageMG[fc05] = 0;
    iInstOphoogPercentageMG[fc41] = 0;
    iInstOphoogPercentageMG[fc241] = 0;

    /* Benodigde ruimte voor alternatieve realisatie tijdens een OV ingreep */
    iPRM_ALTP[fc02] = PRM[prmaltp02];
    iPRM_ALTP[fc05] = PRM[prmaltp05];
    iPRM_ALTP[fc41] = PRM[prmaltp41];
    iPRM_ALTP[fc241] = PRM[prmaltp241];

    /* Richting mag alternatief realiseren tijdens een OV ingreep */
    iSCH_ALTG[fc02] = SCH[schaltg02];
    iSCH_ALTG[fc05] = SCH[schaltg05];
    iSCH_ALTG[fc41] = SCH[schaltg41];
    iSCH_ALTG[fc241] = SCH[schaltg241];

    /* Alternatieve realisatie mag worden
       afgekapt tijdens een OV ingreep als
       deze groentijd is gemaakt */
    iInstAfkapGroenAlt[fc02] = PRM[prmaltg02];
    iInstAfkapGroenAlt[fc05] = PRM[prmaltg05];
    iInstAfkapGroenAlt[fc41] = PRM[prmaltg41];
    iInstAfkapGroenAlt[fc241] = PRM[prmaltg241];

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
    PrioRijTijdScenario(prioFC241bus, d2411, d2412, tbtovg241bus);

    PrioRijTijdScenario(prioFC41bus, d411, d412, tbtovg41bus);

    PrioRijTijdScenario(prioFC05hd, d051, d052, tbtovg05hd);

    PrioRijTijdScenario(hdFC02, d021, d022, tbtovg02hd);



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
    PrioInmelden(prioFC241bus, SH[hprioin241bus], iInstPrioriteitsNiveau[prioFC241bus], iInstPrioriteitsOpties[prioFC241bus], 0, 0);
    PrioInmelden(prioFC41bus, SH[hprioin41bus], iInstPrioriteitsNiveau[prioFC41bus], iInstPrioriteitsOpties[prioFC41bus], 0, 0);
    PrioInmelden(prioFC05hd, SH[hprioin05hd], iInstPrioriteitsNiveau[prioFC05hd], iInstPrioriteitsOpties[prioFC05hd], 0, 0);

    /* Prioriteit-uitmeldingen */
    PrioUitmelden(prioFC241bus, SH[hpriouit241bus]);
    PrioUitmelden(prioFC41bus, SH[hpriouit41bus]);
    PrioUitmelden(prioFC05hd, SH[hpriouit05hd]);

    /* HD-inmeldingen */
    PrioInmelden(hdFC02, SH[hhdin02], iInstPrioriteitsNiveau[hdFC02], iInstPrioriteitsOpties[hdFC02], 0, 0);

    /* HD-uitmeldingen */
    PrioUitmelden(hdFC02, SH[hhduit02]);

    IH[hhdin02] = IH[hhduit02] = FALSE;


    /* Inmelding fc241 type Bus */
    IH[hprioin241bus] = IH[hprioin241buskar] = FALSE;
    if (SCH[schprioin241buskar])
    {
        IH[hprioin241buskar] = 
            DSIMeldingPRIO_V1(0, PRM[prmvtgcat241bus], TRUE, PRM[prmkarsg241],TRUE, CIF_DSIN, TRUE);
    }
    IH[hprioin241bus] = IH[hprioin241buskar];

    /* Uitmelding fc241 type Bus */
    IH[hpriouit241bus] = IH[hpriouit241buskar] = FALSE;
    if (SCH[schpriouit241buskar])
    {
        IH[hpriouit241buskar] = 
            DSIMeldingPRIO_V2(fc241, prioFC241bus, 0, PRM[prmvtgcat241bus], TRUE, PRM[prmkarsg241],TRUE, CIF_DSUIT, TRUE);
    }
    IH[hpriouit241bus] = IH[hpriouit241buskar];

    /* Inmelding fc41 type Bus */
    IH[hprioin41bus] = IH[hprioin41buskar] = FALSE;
    if (SCH[schprioin41buskar])
    {
        IH[hprioin41buskar] = 
            DSIMeldingPRIO_V1(0, PRM[prmvtgcat41bus], TRUE, PRM[prmkarsg41],TRUE, CIF_DSIN, TRUE);
    }
    IH[hprioin41bus] = IH[hprioin41buskar];

    /* Uitmelding fc41 type Bus */
    IH[hpriouit41bus] = IH[hpriouit41buskar] = FALSE;
    if (SCH[schpriouit41buskar])
    {
        IH[hpriouit41buskar] = 
            DSIMeldingPRIO_V2(fc41, prioFC41bus, 0, PRM[prmvtgcat41bus], TRUE, PRM[prmkarsg41],TRUE, CIF_DSUIT, TRUE);
    }
    IH[hpriouit41bus] = IH[hpriouit41buskar];

    /* Inmelding fc05 type Nood- en hulpdienst */
    IH[hprioin05hd] = IH[hprioin05hdkar] = FALSE;
    if (SCH[schprioin05hdkar])
    {
        IH[hprioin05hdkar] = 
            DSIMelding_HD_V1(PRM[prmkarsg05], CIF_DSIN, (!SCH[schchecksirene05hd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR)) ||
            DSIMelding_HD_V1(PRM[prmkarsg02], CIF_DSIN, (!SCH[schchecksirene05hd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR));
    }
    IH[hprioin05hd] = IH[hprioin05hdkar];

    /* Uitmelding fc05 type Nood- en hulpdienst */
    IH[hpriouit05hd] = IH[hpriouit05hdkar] = FALSE;
    if (SCH[schpriouit05hdkar])
    {
        IH[hpriouit05hdkar] = 
            DSIMelding_HD_V1(PRM[prmkarsg05], CIF_DSUIT, (!SCH[schchecksirene05hd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR)) ||
            DSIMelding_HD_V1(PRM[prmkarsg02], CIF_DSUIT, (!SCH[schchecksirene05hd] || CIF_DSI[CIF_DSI_PRI] == CIF_SIR));
    }
    IH[hpriouit05hd] = IH[hpriouit05hdkar];

    /* Inmelding HD fc02 */
    IH[hhdin02kar] = RT[thdin02kar] = !T[thdin02kar] && SCH[schhdin02kar] && ((DSIMelding_HD_V1(PRM[prmkarsghd02], CIF_DSIN, SCH[schchecksirene02]) || DSIMelding_HD_V1(PRM[prmkarsghd05], CIF_DSIN, SCH[schchecksirene02])));
    IH[hhdin02] = IH[hhdin02kar];

    /* Uitmelding HD fc02 */
    IH[hhduit02kar] = RT[thduit02kar] = !T[thduit02kar] && SCH[schhduit02kar] && (DSIMelding_HD_V1(PRM[prmkarsghd02], CIF_DSUIT, FALSE) || DSIMelding_HD_V1(PRM[prmkarsghd05], CIF_DSUIT, FALSE));
    IH[hhduit02] = IH[hhduit02kar];

    /* herstarten FB_timer bij in- of uitmelding HD of einde ingreep (door groenbewaking) */
    RTFB &= ~PRIO_RTFB_BIT;
    if (IH[hhdin02]|| IH[hhduit02] || EH[hhd02] ||
        IH[hprioin05hd]|| IH[hpriouit05hd] || EH[hprio05hd])
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
    PrioCcolElementen(prioFC241bus, tgb241bus, trt241bus, hprio241bus, cvc241bus, tblk241bus);
    PrioCcolElementen(prioFC41bus, tgb41bus, trt41bus, hprio41bus, cvc41bus, tblk41bus);
    PrioCcolElementen(prioFC05hd, tgb05hd, trt05hd, hprio05hd, cvc05hd, tblk05hd);
    PrioCcolElementen(hdFC02, tgbhd02, trthd02, hhd02, cvchd02, -1);
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
    if (SD[ddummykarin241bus]) set_DSI_message(NG, CIF_BUS, PRM[prmkarsg241], CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykaruit241bus] && C[cvc241bus]) set_DSI_message(NG, CIF_BUS, PRM[prmkarsg241], CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykarin41bus]) set_DSI_message(NG, CIF_BUS, PRM[prmkarsg41], CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykaruit41bus] && C[cvc41bus]) set_DSI_message(NG, CIF_BUS, PRM[prmkarsg41], CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], 0);
    if (SD[ddummykarin05hd]) set_DSI_message(NG, CIF_POL, PRM[prmkarsg05], CIF_DSIN, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], CIF_SIR);
    if (SD[ddummykaruit05hd] && C[cvc05hd]) set_DSI_message(NG, CIF_POL, PRM[prmkarsg05], CIF_DSUIT, 1, PRM[prmtestdsivert] - 120, PRM[prmtestdsilyn], PRM[prmtestdsicat], CIF_SIR);

    /* HD ingrepen */
    if (SD[ddummyhdkarin02]) set_DSI_message(0, CIF_POL, PRM[prmkarsghd02], CIF_DSIN, 1, 0, 0, 0, CIF_SIR);
    if (SD[ddummyhdkaruit02] && C[cvchd02]) set_DSI_message(0, CIF_POL, PRM[prmkarsghd02], CIF_DSUIT, 1, 0, 0, 0, CIF_SIR);
}
#endif

#ifdef PRIO_ADDFILE
    #include "GMprio.add"
#endif /* PRIO_ADDFILE */
