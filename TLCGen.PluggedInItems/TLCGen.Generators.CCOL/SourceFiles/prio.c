/* OVPROGRAMMA */
/* ----------- */

#define MAX_INT                       32767L

#include "prio.h"

VLOG_MON5_STRUCT VLOG_mon5[FCMAX];

int iMaximumWachtTijd[FCMAX];
int iVerstrekenGroenTijd[FCMAX];  /*          voor bepaling afkappen */
int iVerstrekenGroenTijd2[FCMAX]; /* werner : voor bepaling ondermax */
int iGerealiseerdeGroenTijd[FCMAX];
int iAfkapGroen[FCMAX];
int iAfkapGroenTijd[FCMAX];
int iPercGroenTijd[FCMAX];
int iKonfliktTijd[FCMAX];
int iTerugKomGroenTijd[FCMAX];
int iTerugKomen[FCMAX];
int iTerugGekomen[FCMAX]; /* toegevoegd Ane 031011 ikv. niet manipuleren TVG_max[fc] */
int iInstPercMaxGroenTijdTerugKomen[FCMAX];
int iMaxGroenTijdTerugKomen[FCMAX];
int iInstMinTerugKomGroenTijd[FCMAX];
int iAantalMalenNietAfkappen[FCMAX];
int iInstAantalMalenNietAfkappen[FCMAX];
int iNietAfkappen[FCMAX];
int iMaxGroen[FCMAX];
int iPRM_ALTP[FCMAX];
int iSCH_ALTG[FCMAX];
int iInstAfkapGroenAlt[FCMAX];
int iInstOphoogPercentageMG[FCMAX];
int iOphoogPercentageMG[FCMAX];
int iPercMGOphogen[FCMAX];
int iMaximumWachtTijdOverschreden[prioFCMAX]; /* toegevoegd Ane 21-09-2011 */
int iT_GBix[prioFCMAX];
int iH_PRIOix[prioFCMAX];
int iBlokkeringsTijd[prioFCMAX];
int iBlokkeringsTimer[prioFCMAX];
int iFC_PRIOix[prioFCMAX];
int iOnderMaximum[prioFCMAX];
int iOnderMaximumVerstreken[prioFCMAX];
int iGroenBewakingsTijd[prioFCMAX];
int iGroenBewakingsTimer[prioFCMAX];
int iRijTijd[prioFCMAX];
int iRijTimer[prioFCMAX];
int iPrioriteit[prioFCMAX];
int iPrioriteitNooitAfkappen[prioFCMAX];
int iKOVPrio[prioFCMAX];
int iInstPrioriteitsNiveau[prioFCMAX];
int iInstPrioriteitsOpties[prioFCMAX];
int iPrioriteitsNiveau[prioFCMAX];
int iPrioriteitsOpties[prioFCMAX];
int iKPrioriteitsOpties[FCMAX];
int iStartGroen[prioFCMAX];
int iBijzonderRealiseren[prioFCMAX];
int iWachtOpKonflikt[prioFCMAX];
boolv bMagEerst[FCMAX];
int iAantalPrioriteitsInmeldingen[prioFCMAX];
int iRijTijdScenario[prioFCMAX];
int iRTSOngehinderd[prioFCMAX];
int iRTSBeperktGehinderd[prioFCMAX];
int iRTSGehinderd[prioFCMAX];
int iSelDetFoutNaGB[prioFCMAX];
int iSelDetFout[prioFCMAX];
int iAantalInmeldingen[prioFCMAX];
int iXPrio[prioFCMAX];

int *iRealisatieTijd[FCMAX];
int *iInPrioriteitsNiveau[prioFCMAX];
int *iInPrioriteitsOpties[prioFCMAX];
int *iInRijTimer[prioFCMAX];
int *iInGroenBewakingsTimer[prioFCMAX];
int *iInOnderMaximumVerstreken[prioFCMAX];
int *iInMaxWachtTijdOverschreden[prioFCMAX]; /*@@@ DSC*/
int *iInID[prioFCMAX];
int *iPrioMeeRealisatie[FCMAX];

int iM_RealisatieTijd[FCMAX*FCMAX];
int iM_InPrioriteitsNiveau[prioFCMAX * MAX_AANTAL_INMELDINGEN];
int iM_InPrioriteitsOpties[prioFCMAX * MAX_AANTAL_INMELDINGEN];
int iM_InRijTimer[prioFCMAX * MAX_AANTAL_INMELDINGEN];
int iM_InGroenBewakingsTimer[prioFCMAX * MAX_AANTAL_INMELDINGEN];
int iM_InOnderMaximumVerstreken[prioFCMAX * MAX_AANTAL_INMELDINGEN];
int iM_InID[prioFCMAX * MAX_AANTAL_INMELDINGEN];
int iM_InMaxWachtTijdOverschreden[prioFCMAX * MAX_AANTAL_INMELDINGEN]; /*@@@ DSC*/
int iM_PrioMeeRealisatie[FCMAX*FCMAX];

int prioKFC_MAX[prioFCMAX];
int prioGKFC_MAX[prioFCMAX];
int *prioTO_pointer[prioFCMAX];
int prioM_TO_pointer[prioFCMAX*prioFCMAX];
int iLangstWachtendeAlternatief;

void PrioInit(void)
{
    int prio1, prio2, fc1, fc2, fc;

    /* default OV-instellingen */
    for (fc = 0;fc < FCMAX; ++fc)
    {
        iMaximumWachtTijd[fc] = DEFAULT_MAX_WACHTTIJD;
        iPRM_ALTP[fc] = TFG_max[fc];
        iSCH_ALTG[fc] = TRUE;
        iRealisatieTijd[fc] = iM_RealisatieTijd + (fc*FCMAX);

    	/* Meerealisatie default uit (NG) */
	    iPrioMeeRealisatie[fc] = iM_PrioMeeRealisatie + (fc*FCMAX);
	    for (fc2 = 0; fc2 < FCMAX; ++fc2)
	    {
	        iPrioMeeRealisatie[fc][fc2] = NG;
	    }
    }
	
    /* werkelijke OV-instellingen */
    PrioInstellingen();

    /* initialisatie overige OV-variabelen */
    for (prio1=0; prio1<prioFCMAX; ++prio1)
    {
        fc1=iFC_PRIOix[prio1];
        prioTO_pointer[prio1] = prioM_TO_pointer+(prio1*prioFCMAX);
        prioKFC_MAX[prio1]=0;
        for (prio2 = 0; prio2 < prioFCMAX; ++prio2)
        {
            fc2 = iFC_PRIOix[prio2];
#if (CCOL_V >= 95) && !defined NO_TIGMAX
            if (TIG_max[fc1][fc2] >= 0)
#else
            if (TO_max[fc1][fc2] >= 0)
#endif
            {
                prioTO_pointer[prio1][prioKFC_MAX[prio1]] = prio2;
                (prioKFC_MAX[prio1])++;
            }
        }
        iBlokkeringsTimer[prio1]         = MAX_INT;
        iInPrioriteitsNiveau[prio1]      = iM_InPrioriteitsNiveau+(prio1*MAX_AANTAL_INMELDINGEN);
        iInPrioriteitsOpties[prio1]      = iM_InPrioriteitsOpties+(prio1*MAX_AANTAL_INMELDINGEN);
        iInRijTimer[prio1]               = iM_InRijTimer+(prio1*MAX_AANTAL_INMELDINGEN);
        iInGroenBewakingsTimer[prio1]    = iM_InGroenBewakingsTimer+(prio1*MAX_AANTAL_INMELDINGEN);
        iInOnderMaximumVerstreken[prio1] = iM_InOnderMaximumVerstreken+(prio1*MAX_AANTAL_INMELDINGEN);
        iInMaxWachtTijdOverschreden[prio1] = iM_InMaxWachtTijdOverschreden+(prio1*MAX_AANTAL_INMELDINGEN);/*@@@ DSC*/
        iInID[prio1]                     = iM_InID+(prio1*MAX_AANTAL_INMELDINGEN);
        iPrioriteit[prio1]               = FALSE;
        iAantalInmeldingen[prio1]        = 0;
    }
    for (prio1 = 0; prio1 < prioFCMAX; ++prio1) 
    {
        fc1 = iFC_PRIOix[prio1];
        prioGKFC_MAX[prio1] = prioKFC_MAX[prio1];
        for (prio2 = 0; prio2 < prioFCMAX; ++prio2) 
        {
            fc2 = iFC_PRIOix[prio2];
#if (CCOL_V >= 95) && !defined NO_TIGMAX
            if (TIG_max[fc1][fc2] == GK || TIG_max[fc1][fc2] == GKL) /* toegevoegd Ane 25-04-2011, GKL */
#else
            if (TO_max[fc1][fc2] == GK || TO_max[fc1][fc2] == GKL)
#endif
            {
                prioTO_pointer[prio1][prioGKFC_MAX[prio1]] = prio2;
                (prioGKFC_MAX[prio1])++;
            }
        }
    }
}

int BepaalPrioriteitsOpties(int prm_prio)
{
    int p, iReturn;

    for (iReturn = 0, p = PRM[prm_prio] % 1000L; p > 0; p /= 10L) 
    {
        switch (p % 10) 
        {
        case 1:
            iReturn |= poAanvraag;
            iReturn |= poAfkappenKonfliktRichtingen;
            break;
        case 2:
            iReturn |= poAanvraag;
            iReturn |= poGroenVastHouden;
            break;
        case 3:
            iReturn |= poAanvraag;
            iReturn |= poBijzonderRealiseren;
            break;
        case 4:
            iReturn |= poAanvraag;
            iReturn |= poAfkappenKonflikterendOV;
            iReturn |= poAfkappenKonfliktRichtingen;
            break;
        case 5:
            iReturn |= poAanvraag;
            iReturn |= poAfkappenKonflikterendOV;
            iReturn |= poAfkappenKonfliktRichtingen;
            iReturn |= poGroenVastHouden;
            iReturn |= poBijzonderRealiseren;
            iReturn |= poNoodDienst;
            break;
        }
    }
    return iReturn;
}

/* -------------------------------------------------
   KonfliktTijden bepaalt van iedere richting
   hoelang deze nog minimaal geel/rood is
   op basis van de garantiegroentijden en geeltijden
   van de lopende konflikten en de eigen
   garantieroodtijd en geeltijd.
   ------------------------------------------------- */
void KonfliktTijden(void)
{
    int fc, i, k, iKT, iRestGroen, iRestGeel, iRestTO;

    for (fc = 0; fc < FCMAX; ++fc) 
    {
        iKonfliktTijd[fc] = (GL[fc] ? (TGL_max[fc] > 0 ? TGL_max[fc] : 1) - TGL_timer[fc] : 0) +
            (GL[fc] ? TRG_max[fc] : TRG[fc] ? TRG_max[fc] - TRG_timer[fc] : 0);
        if (K[fc]) 
        {
            for (i = 0; i < GKFC_MAX[fc]; ++i) 
            {
#if (CCOL_V >= 95)
                k = KF_pointer[fc][i];
#else
                k = TO_pointer[fc][i];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
                if (TIG[k][fc])
#else
                if (TO[k][fc])
#endif
                {
                    iRestGroen = TGG[k] ? TGG_max[k] - TGG_timer[k] : 0;
                    iRestGeel = (TGL_max[k] > 0 ? TGL_max[k] : 1) - TGL_timer[k];
#if (CCOL_V >= 95) && !defined NO_TIGMAX
                    iRestTO = TIG_max[k][fc] - TIG_timer[k];
                    if (TIG_max[k][fc] == GK)
#else
                    iRestTO = TO_max[k][fc] - TO_timer[k];
                    if (TO_max[k][fc] == GK)
#endif
                    {
                        iKT = iRestGroen;
                    }
                    else
                    {
                        iKT = iRestGroen + iRestGeel + iRestTO;
                    }
                    if (iKonfliktTijd[fc] < iKT) 
                    {
                        iKonfliktTijd[fc] = iKT;
                    }
                }
            }
        }
    }
}

/* -------------------------------------------------------
   TerugKomGroen zorgt voor een juiste waarde voor TVG_max
   van een richting, als deze richting terug komt.
   ------------------------------------------------------- */
void TerugKomGroen(void)
{
    int fc;

    for (fc = 0; fc < FCMAX; ++fc)
    {
        if (iTerugKomGroenTijd[fc] > 0 && !iTerugKomen[fc])
        {
            TVG_max[fc] = iTerugKomGroenTijd[fc] - TFG_max[fc];
            if (TVG_max[fc] < 0)
            {
                TVG_max[fc] = 0;
            }
        }
    }
}

/* ------------------------------------------------------------------
   OVTimers houdt de timers bij voor de rijtijd en de groenbewaking
   van alle inmeldingen en de rijtijd van alle OV-richtingen.
   De (verstreken) rijtijd van een OV-richting is gelijk aan de
   grootste (verstreken) rijtijd van al zijn inmeldingen.
   Daarnaast wordt:
   - de verstreken groentijd van iedere richting bijgehouden.
   - het OV-bitje van de gebruikte CCOL-instructievariabelen gereset.
   - een uitmelding gedaan bij het aanspreken van de groenbewaking
     en daarbij, indien gewenst, de detectie voor in- en uitmeldingen
     defect verklaard,
   ------------------------------------------------------------------ */
void PrioTimers(void)
{
    int fc, inm, prio;
	int sml = -1;
#ifndef MLMAX
	int ml;
#endif

    for (fc = 0; fc < FCMAX; ++fc)
    {
        Z[fc]  &= ~PRIO_Z_BIT;
        FM[fc] &= ~PRIO_FM_BIT;
        RW[fc] &= ~PRIO_RW_BIT;
        RR[fc] &= ~PRIO_RR_BIT;
        YV[fc] &= ~PRIO_YV_BIT;
	YM[fc] &= ~PRIO_YM_BIT;
	MK[fc] &= ~PRIO_MK_BIT;
        PP[fc] &= ~PRIO_PP_BIT;
        RTFB &= ~PRIO_RTFB_BIT;

        if (G[fc])
        {
            if (iVerstrekenGroenTijd[fc] + TE <= MAX_INT)
            {
                iVerstrekenGroenTijd[fc] += TE;
            }
            if (SG[fc])
            {
                iVerstrekenGroenTijd[fc] = 0;
                iVerstrekenGroenTijd2[fc] = 0;
            }
        }
        else
        {
            if (iVerstrekenGroenTijd[fc] > 0)
            {
                iGerealiseerdeGroenTijd[fc] += iVerstrekenGroenTijd[fc];
                iVerstrekenGroenTijd[fc] = -1;
                iVerstrekenGroenTijd2[fc] = -1;
            }
        }
#ifdef MLMAX
		sml = SML;
#else
#ifdef MLAMAX
		if (sml == -1) for (ml = 0; ml < MLAMAX; ++ml) if (PRMLA[ml][fc] == PRIMAIR) { sml = SMLA; break; }
#endif
#ifdef MLBMAX
		if (sml == -1) for (ml = 0; ml < MLBMAX; ++ml) if (PRMLB[ml][fc] == PRIMAIR) { sml = SMLB; break; }
#endif
#ifdef MLCMAX
		if (sml == -1) for (ml = 0; ml < MLCMAX; ++ml) if (PRMLC[ml][fc] == PRIMAIR) { sml = SMLC; break; }
#endif
#ifdef MLDMAX
		if (sml == -1) for (ml = 0; ml < MLDMAX; ++ml) if (PRMLD[ml][fc] == PRIMAIR) { sml = SMLD; break; }
#endif
#ifdef MLEMAX
		if (sml == -1) for (ml = 0; ml < MLEMAX; ++ml) if (PRMLE[ml][fc] == PRIMAIR) { sml = SMLE; break; }
#endif
#endif
        if (sml && iGerealiseerdeGroenTijd[fc] > 0 && !PG[fc])
        {
            iGerealiseerdeGroenTijd[fc] = 0;
        }
    }
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        iRijTimer[prio] = 0;
        for (inm = 0; inm < iAantalInmeldingen[prio]; ++inm)
        {
            if (iInRijTimer[prio][inm] + TE <= MAX_INT)
            {
                iInRijTimer[prio][inm] += TE;
            }
            if (iRijTimer[prio] < iInRijTimer[prio][inm])
            {
                iRijTimer[prio] = iInRijTimer[prio][inm];
            }
            if (G[fc])
            {
                if (iInGroenBewakingsTimer[prio][inm] + TE <= MAX_INT && (iInRijTimer[prio][inm] >= iRijTijd[prio])) /* PS Groenbewakingstijd niet ophogen Inrijtimer nog kleiner is als de rijtijd */
                {
                    iInGroenBewakingsTimer[prio][inm] += TE;
                }
                if (iInGroenBewakingsTimer[prio][inm] >= iGroenBewakingsTijd[prio])
                {
                    PrioUitmeldenIndex(prio, inm, 1, TRUE);
                    if (iSelDetFoutNaGB[prio])
                    {
                        iSelDetFout[prio] = TRUE;
                    }
                    inm--;
                }
            }
        }
    }
}

/* -----------------------------------------------------------------------------
   WachtTijdBewaking bepaalt of de maximumwachttijd is overschreden.
   Het resultaat wordt opgeslagen in de array iMaximumWachtTijdOverschreden[] (wijz. AW 21-09-2011).
   De maximum wachttijd is overschreden als er een richting fc een aanvraag
   heeft en zijn fasebewakingstijd TFB_timer[fc] heeft de maximum wachttijd
   iMaximumWachtTijd[fc] overschreden.
   ----------------------------------------------------------------------------- */
void WachtTijdBewaking(void)
{
    int prio, fc, i, k;

    /* Wijziging Ane iMaxWachtTijdOverschreden voor conflicten prio-richting of 
       voor richtingen die niet meegerealiseerd mogen worden; 
       bovendien wordt per prio-richting de max. wachttijdoverschrijding bepaalt */
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        iMaximumWachtTijdOverschreden[prio] = 0;
        fc = iFC_PRIOix[prio];
        for (i = 0; i < GKFC_MAX[fc] && !iMaximumWachtTijdOverschreden[prio]; ++i) /* gewijzigd Ane KFC in GKFC */
        {
#if (CCOL_V >= 95)
            k = KF_pointer[fc][i];
#else
            k = TO_pointer[fc][i];
#endif
            iMaximumWachtTijdOverschreden[prio] |= A[k] && TFB_timer[k] >= iMaximumWachtTijd[k];
        }
        for (i = 0; i < FCMAX && !iMaximumWachtTijdOverschreden[prio]; ++i)
        {
            iMaximumWachtTijdOverschreden[prio] |= A[i] && TFB_timer[i] >= iMaximumWachtTijd[i] && !iSCH_ALTG[i];
        }
    }
}

void mag_eerst(void)
{
    int prio, fc, i, k;

    for (fc = 0; fc < FCMAX; ++fc) 
    {
        if (!AAPR[fc] || G[fc])
        {
            bMagEerst[fc] = FALSE;
        }
    }

    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        if (iPrioriteit[prio] && G[fc])
        {
            for (i = 0; i < GKFC_MAX[fc]; ++i)
            {
#if (CCOL_V >= 95)
                k = KF_pointer[fc][i];
#else
                k = TO_pointer[fc][i];
#endif
                bMagEerst[k] |= AAPR[k];
            }
        }
    }
}

int moet_wachten(int ov)
{
    int fc, i, k;
    int imw = 0;

    fc = iFC_PRIOix[ov];
    for (i = 0; i < GKFC_MAX[fc]; ++i)
    {
#if (CCOL_V >= 95)
        k = KF_pointer[fc][i];
#else
        k = TO_pointer[fc][i];
#endif
        imw |= bMagEerst[k];
    }

    return imw != 0;
}

/* ------------------------------------------------------------
   BlokkeringsTijd houdt van iedere OV-richting bij
   hoelang geleden de laatste ingreep heeft plaatsgevonden.
   Het resultaat voor OV-richting prio wordt opgeslagen in de
   variabele iBlokkeringsTimer[prio].
   Tussen twee ingrepen door moeten konflikten de mogelijkheid
   hebben gehad te realiseren. Dit wordt mogelijk gemaakt door
   het bijhouden van de variabele iWachtOpKonflikt[prio], die
   aangeeft of OV-richting prio nog moet wachten op de realisatie
   van een konflikt.
   ------------------------------------------------------------ */
void BlokkeringsTijd(void)
{
    int prio, fc;

    if (!iLangstWachtendeAlternatief)
    {
        mag_eerst();

        for (prio = 0; prio < prioFCMAX; ++prio)
        {
            iWachtOpKonflikt[prio] = moet_wachten(prio);
        }
    }
    else
    {
        for (prio = 0; prio < prioFCMAX; ++prio)
        {
            fc = iFC_PRIOix[prio];
            if (iPrioriteit[prio] && G[fc])
            {
                iBlokkeringsTimer[prio] = 0;
                iWachtOpKonflikt[prio] = 1;
            }
            else
            {
                if (iBlokkeringsTimer[prio] + TE <= MAX_INT)
                {
                    iBlokkeringsTimer[prio] += TE;
                }
                if (iWachtOpKonflikt[prio] && (K[fc] || !fka(fc)) /* && !kaa(fc) waarom is dit? wijz. Ane 13-01-2012 */)
                {
                    iWachtOpKonflikt[prio] = 0;
                }
            }
        }
    }
}

/* --------------------------------------------------------------
   OnderMaximum bepaalt van alle PRIO-richtingen of het
   ondermaximum is overschreden.
   Het ondermaximum van OV-richting prio is overschreden als de
   resterende maximumgroentijd kleiner is dan zijn
   ondermaximum iOnderMaximum[prio].
   Het resultaat wordt opgeslagen in iOnderMaximumVerstreken[prio].
   -------------------------------------------------------------- */
void OnderMaximum(void)
{
    int prio,fc, iMaximumGroenTijd, iMaxResterendeGroentijd;

    for (prio=0; prio < prioFCMAX; ++prio) 
    {
        fc=iFC_PRIOix[prio];
        /* bepaal de maximum groentijd obv TFG en TVG */
        iMaximumGroenTijd = TFG_max[fc] + (TVG_max[fc] >= 0 ? TVG_max[fc] : 0);
    
        /* 15-02-2017 Werner : bereken resterende groentijd om te bepalen of er sprake is van ondermaximum */
        /*                     ipv te kijken naar de reeds verstreken groentijd                            */
        iMaxResterendeGroentijd = ((R[fc] || VS[fc]) ? (TFG_max[fc] + 1 + (TVG_max[fc] >= 0 ? TVG_max[fc] : 0) + 1)                 : 0) +
                                            (TFG[fc] ? (TFG_max[fc] + 1 + (TVG_max[fc] >= 0 ? TVG_max[fc] : 0) + 1) - TFG_timer[fc] : 0) +
                                            (WG[fc]  ? (                  (TVG_max[fc] >= 0 ? TVG_max[fc] : 0) + 1)                 : 0) +
                                            (TVG[fc] ? (                  (TVG_max[fc] >= 0 ? TVG_max[fc] : 0) + 1) - TVG_timer[fc] : 0);
    
  
        /* we kunnen op 2 manieren bepalen of het ondermaximum is verstreken : */
        /* 1) kijk naar de maximaal resterende groentijd                       */
        /* 2) kijk naar de reeds verstreken groentijd (uitgezonderd in WS)     */
        /*    -> reden : de richting kan terug gezet zijn in WG door een RW    */
        iOnderMaximumVerstreken[prio] =  iOnderMaximum[prio] > 0 ? iOnderMaximum[prio]>=iMaxResterendeGroentijd : 0;
        iOnderMaximumVerstreken[prio] |= iOnderMaximum[prio] > 0 ? iVerstrekenGroenTijd2[fc]>=iMaximumGroenTijd-iOnderMaximum[prio] : 0;
    }
}

/* --------------------------------------------------------------
   OVInmeldenID doet een inmelding als iInmelding waar is, met
   prioriteitsniveau iPN, prioriteitsopties iPO en ID iID.
   Bij de inmelding wordt tevens opgeslagen of op dat moment
   het ondermaximum was verstreken.
   Daarnaast worden de timers voor de rijtijd en de groenbewaking
   gereset.
   -------------------------------------------------------------- */
void PrioInmeldenID(int prio,
                  int iInmelding,
                  int iPN,         /* prioriteitsniveau       */
                  int iPO,         /* prioriteitsopties       */
                  int iRT,         /* rijtimer                */
                  int iGBT,        /* groenbewakingstimer     */
                  int iID)         /* identificatie inmelding */
{
    int inm;
    int fc = iFC_PRIOix[prio];

    if (iInmelding && iAantalInmeldingen[prio] < MAX_AANTAL_INMELDINGEN)
    {
        inm                                = iAantalInmeldingen[prio];
        iInPrioriteitsNiveau[prio][inm]      = iPN;
        iInPrioriteitsOpties[prio][inm]      = iPO;
        iInRijTimer[prio][inm]               = iRT;
        iInGroenBewakingsTimer[prio][inm]    = iGBT;
        iInID[prio][inm]                     = iID;
        iInOnderMaximumVerstreken[prio][inm] = iOnderMaximumVerstreken[prio];
        iInMaxWachtTijdOverschreden[prio][inm] = iMaximumWachtTijdOverschreden[prio];/*@@@ DSC: bijhouden of bij inmelding wachttijdcriterium overschreden was*/
        if (iPO & poNoodDienst)
        {
            VLOG_mon5[fc].inmhd = TRUE;
        }
        else 
        {
             VLOG_mon5[fc].inmov = TRUE;
        }
        (iAantalInmeldingen[prio])++;
    }
}

/* --------------------------------------------------------------
   PrioInmelden doet een inmelding als iInmelding waar is, met
   prioriteitsniveau iPN en prioriteitsopties iPO.
   Bij de inmelding wordt tevens opgeslagen of op dat moment
   het ondermaximum was verstreken.
   Daarnaast worden de timers voor de rijtijd en de groenbewaking
   gereset.
   -------------------------------------------------------------- */
void PrioInmelden(int prio,
                int iInmelding,
                int iPN,         /* prioriteitsniveau   */
                int iPO,         /* prioriteitsopties   */
                int iRT,         /* rijtimer            */
                int iGBT)        /* groenbewakingstimer */
{
    PrioInmeldenID(prio,
                 iInmelding,
                 iPN,         /* prioriteitsniveau   */
                 iPO,         /* prioriteitsopties   */
                 iRT,         /* rijtimer            */
                 iGBT,        /* groenbewakingstimer */
                 0);          /* default ID: 0       */
}

/* ------------------------------------------------------
   PrioUitmeldenIndex meldt de inmelding met index inm uit.
   ------------------------------------------------------ */
void PrioUitmeldenIndex(int prio,
                      int inm,
                      int iUitmelding,
                      boolv bGeforceerd)
{
    int i;
    int fc = iFC_PRIOix[prio];
    if (iUitmelding && iAantalInmeldingen[prio]>0)
    {
        if (iInPrioriteitsOpties[prio][inm] & poNoodDienst)
        {
            if (bGeforceerd)
            {
                VLOG_mon5[fc].uitmbewhd = TRUE;
            }
            else
            {
                VLOG_mon5[fc].uitmhd = TRUE;
            }
        }
        else
        {
            if (bGeforceerd)
            {
                VLOG_mon5[fc].uitmbewov = TRUE;
            }
            else
            {
                VLOG_mon5[fc].uitmov = TRUE;
            }
        }
        for (i = inm; i < iAantalInmeldingen[prio] - 1; ++i)
        {
            iInPrioriteitsNiveau[prio][i]      = iInPrioriteitsNiveau[prio][i+1];
            iInPrioriteitsOpties[prio][i]      = iInPrioriteitsOpties[prio][i+1];
            iInRijTimer[prio][i]               = iInRijTimer[prio][i+1];
            iInGroenBewakingsTimer[prio][i]    = iInGroenBewakingsTimer[prio][i+1];
            iInID[prio][i]                     = iInID[prio][i+1];
            iInOnderMaximumVerstreken[prio][i] = iInOnderMaximumVerstreken[prio][i+1];
            iInMaxWachtTijdOverschreden[prio][i] = iInMaxWachtTijdOverschreden[prio][i+1]; /*@@@ DSC*/
        }
        (iAantalInmeldingen[prio])--;
        iSelDetFout[prio] = FALSE;
    }
}

/* ---------------------------------------------------------
   PrioUitmeldenID meldt de "oudste" inmelding met ID iID uit.
   --------------------------------------------------------- */
void PrioUitmeldenID(int prio, int iUitmelding, int iID)
{
    int i, inm;
    int fc = iFC_PRIOix[prio];
    if (iUitmelding)
    {
        if (iAantalInmeldingen[prio] > 0)
        {
            inm = -1;
            for (i = 0; inm == -1 && i < iAantalInmeldingen[prio]; ++i)
            {
                if (iInID[prio][i] == iID)
                {
                    inm = i;
                }
            }
            if (inm >= 0)
            {
                PrioUitmeldenIndex(prio, inm, iUitmelding, FALSE);
            }
            else
            {
                VLOG_mon5[fc].foutuitmov = TRUE;
            }
        }
        else
        {
            VLOG_mon5[fc].foutuitmov = TRUE;
        }
    }
}

void PrioUitmelden(int prio, int iUitmelding)
{
	PrioUitmeldenID(prio, iUitmelding, 0);
}

int PrioAantalInmeldingenID(int prio, int iID)
{
    int i, iReturn;

    for (i = iReturn = 0; i < iAantalInmeldingen[prio]; ++i)
    {
        if (iInID[prio][i] == iID)
        {
            iReturn++;
        }
    }
    return iReturn;
}

/* ---------------------------------------------------------------------------------
   OVRijTijdScenario bepaalt het rijtijdscenario voor een OV-richting (prio)
   op basis de status van de koplus (dkop) en de lange lus (dlang).
   Het rijtijdscenario wordt opgeslagen in de variabele iRijTijdScenario[prio]
   en kan de waarde rtsOngehinderd, rtsBeperktGehinderd of rtsGehinderd
   krijgen. In beginsel is het rijtijdscenario rtsOngehinderd. Bij het opkomen
   van de koplus of de lange lus wordt het rijtijdscenario rtsBeperktGehinderd.
   Als koplus en lange lus langer dan de bezettijd (tbezet) op staan, dan
   wordt het rijtijdscenario rtsGehinderd. Tijdens groen is het scenario
   rtsOngehinderd. Tijdens rood kan de mate van gehinderd zijn alleen maar toenemen.
   Als er geen koplus of lange lus is, dan kan hiervoor de waarde NG (-1) worden
   genomen. Is er alleen een lange lus of een koplus opgegeven, dan vervalt het
   scenario rtsGehinderd. Zijn beiden niet opgegeven, dan vervalt ook
   rtsBeperktGehinderd.
   --------------------------------------------------------------------------------- */
void PrioRijTijdScenario(int prio, int dkop, int dlang, int tbezet)
{
    int fc;

    fc = iFC_PRIOix[prio];
    if (tbezet >= 0 && dkop >= 0 && dlang >= 0)
    {
        RT[tbezet] = !D[dkop] || !D[dlang];
    }
    if (R[fc])
    {
        if (dkop >= 0 && dlang >= 0)
        {
            if (tbezet >= 0 && !T[tbezet] && !RT[tbezet])
            {
                if (iRijTijdScenario[prio] < rtsGehinderd)
                {
                    iRijTijdScenario[prio] = rtsGehinderd;
                }
            }
            else
            {
                if (tbezet < 0 && D[dkop] && D[dlang])
                {
                    if (iRijTijdScenario[prio] < rtsGehinderd)
                    {
                        iRijTijdScenario[prio] = rtsGehinderd;
                    }
                }
                if (D[dkop] || D[dlang])
                {
                    if (iRijTijdScenario[prio] < rtsBeperktGehinderd)
                    {
                        iRijTijdScenario[prio] = rtsBeperktGehinderd;
                    }
                }
            }
        }
        else
        {
            if (dkop >= 0 && D[dkop] ||
                dlang >= 0 && D[dlang])
                {
                if (iRijTijdScenario[prio] < rtsBeperktGehinderd)
                {
                    iRijTijdScenario[prio] = rtsBeperktGehinderd;
                }
            }
        }
    }
    else
    {
        iRijTijdScenario[prio] = rtsOngehinderd;
    }
    switch (iRijTijdScenario[prio])
    {
    default:
    case rtsOngehinderd:
        iRijTijd[prio] = iRTSOngehinderd[prio];
        break;
    case rtsBeperktGehinderd:
        iRijTijd[prio] = iRTSBeperktGehinderd[prio];
        break;
    case rtsGehinderd:
        iRijTijd[prio] = iRTSGehinderd[prio];
        break;
    }
}

/* ----------------------------------------------------------
   Met StelInTimer kan de actuele waarde en de instelling van
   een timer worden veranderd. De timer heeft index iIndex,
   krijgt actuele waarde iActueleWaarde en krijgt als
   instelling iInstelling.
   ---------------------------------------------------------- */
void StelInTimer(int iIndex, int iActueleWaarde, int iInstelling)
{
    if (iIndex >= 0 && iIndex < TM_MAX)
    {
        T_timer[iIndex] = iActueleWaarde;
        T_max[iIndex]   = iInstelling;
        T[iIndex]       = iActueleWaarde < iInstelling;
    }
}

/* ------------------------------------------------------------
   Met StelInCounter kan de actuele waarde en de instelling van
   een counter worden veranderd. De counter heeft index iIndex,
   krijgt actuele waarde iActueleWaarde en krijgt als
   instelling iInstelling.
   ------------------------------------------------------------ */
void StelInCounter(int iIndex, int iActueleWaarde, int iInstelling)
{
    if (iIndex >= 0 && iIndex < CT_MAX)
    {
        C_counter[iIndex] = iActueleWaarde;
        C_max[iIndex] = iInstelling;
        C[iIndex] = iActueleWaarde > 0 && iActueleWaarde < iInstelling;
    }
}

/* --------------------------------------------------------
   PrioCcolElementen zorgt voor het bijwerken van de volgende
   CCOL-elementen voor het OV:
   - de groenbewakingstimer tgb.
   - de rijtimer trt.
   - het hulpelement voor de prioriteit hprio.
   - de counter voor het aantal OV-inmeldingen cvc.
   - de blokkeringstimer tblk.
   -------------------------------------------------------- */
void PrioCcolElementen(int prio, int tgb, int trt, int hprio, int cvc, int tblk)
{
    if (prio >= 0 && prio < prioFCMAX)
    {
        if (tgb >= 0 && tgb < TM_MAX)
        {
            T_max[tgb]   = iGroenBewakingsTijd[prio];
            T[tgb]       = iGroenBewakingsTimer[prio] < iGroenBewakingsTijd[prio];
            T_timer[tgb] = T[tgb] ? iGroenBewakingsTimer[prio] : T_max[tgb];
        }
        if (trt >= 0 && trt < TM_MAX)
        {
            T_max[trt]   = iRijTijd[prio];
            T[trt]       = iRijTimer[prio] < iRijTijd[prio];
            T_timer[trt] = T[trt] ? iRijTimer[prio] : T_max[trt];
        }
        if (hprio >= 0 && hprio < HE_MAX)
        {
            IH[hprio] = iPrioriteit[prio];
        }
        if (cvc >= 0 && cvc < CT_MAX)
        {
            C_counter[cvc] = iAantalInmeldingen[prio];
            C[cvc]         = iAantalInmeldingen[prio] > 0;
        }
        if (tblk >= 0 && tblk < TM_MAX)
        {
            T_max[tblk]   = iBlokkeringsTijd[prio];
            T[tblk]       = iBlokkeringsTimer[prio] < iBlokkeringsTijd[prio];
            T_timer[tblk] = T[tblk] ? iBlokkeringsTimer[prio] : T_max[tblk];
        }
    }
}

/* ---------------------------------------------------------------
   PrioriteitsToekenning zorgt voor de prioriteitstoekenning en
   voor het intrekken van prioriteiten.
   Een OV-richting met inmeldingen krijgt prioriteit als
   aan alle volgende voorwaarden is voldaan:
   - het ondermaximum is niet verstreken, of er is een
     nooddienst ingemeld..
   - er zijn geen konflikterende OV-richtingen met een hoger
     prioriteitsniveau.
   - de selectieve detectie wordt niet beschouwd als
     zijnde defect, of er is een nooddienst ingemeld.
   - de maximum wachttijd is niet overschreden, of er heeft
     zich een nooddienst ingemeld.
   - de blokkeringstijd is afgelopen, of er heeft zich een
     nooddienst ingemeld.
   - sinds de vorige ingreep heeft er een konflikt de
     mogelijkheid gehad te realiseren, of er heeft zich een
     nooddienst ingemeld.
   Van iedere OV-richting wordt het prioriteitsniveau bepaald.
   Het prioriteitsniveau van een OV-richting is gelijk aan
   het hoogste prioriteitsniveau van al zijn inmeldingen waarvan:
   - het ondermaximum bij inmelding niet was verstreken, of
   - de inmelding van een nooddienst is.
   Van iedere OV-rchting worden de prioriteitsopties bepaald.
   Een prioriteitsoptie staat op als deze opstaat voor minimaal
   ??n van bovenvermelde (bepaling prioriteitsniveau) inmeldingen.
   Van een OV-richting wordt de prioriteit ingetrokken als aan
   minimaal ??n van de volgende voorwaarden is voldaan:
   - de OV-richting is nog niet groen en er is een
     konflikterende OV-richting met een hoger prioriteitsniveau.
   - De OV-richting is groen en er is een
     konflikterende OV-richting met een hoger prioriteitsniveau
     en met de prioriteitsoptie poAfkappenKonflikterendOV.
   --------------------------------------------------------------- */
void PrioriteitsToekenning(void)
{
    int prio, inm, i, kov, fc;

    /* wijz. Ane Max.Wachttijdoverschreden kijkt alleen naar de prio-conflicten! */
    /* Bepaal prioriteitsniveau */
    /* van alle OV-richtingen   */
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        iPrioriteitsNiveau[prio] = 0;
        iPrioriteitsOpties[prio] = 0;
        iAantalPrioriteitsInmeldingen[prio] = 0;
        iGroenBewakingsTimer[prio] = iGroenBewakingsTijd[prio];
        for (inm = 0; inm < iAantalInmeldingen[prio]; ++inm)
        {
           /* wijz. Cyril Ondermaximum resetten niet op !G maar SG omdat anders als de richting bij prio= direct weer terug komt   */
           if (SG[fc] || kg(fc))
            {
              iInOnderMaximumVerstreken[prio][inm] = 0;
              iInMaxWachtTijdOverschreden[prio][inm] = 0;
           }
            iPrioriteitsOpties[prio] |= iInPrioriteitsOpties[prio][inm] & poAanvraag;
            if (!iSelDetFout[prio] && !iInOnderMaximumVerstreken[prio][inm] && !iInMaxWachtTijdOverschreden[prio][inm] ||/*@@@ DSC*/
                iInPrioriteitsOpties[prio][inm] & poNoodDienst)
            {
                (iAantalPrioriteitsInmeldingen[prio])++;
                if (iGroenBewakingsTimer[prio] > iInGroenBewakingsTimer[prio][inm])
                {
                    iGroenBewakingsTimer[prio] = iInGroenBewakingsTimer[prio][inm];
                }
                if (iPrioriteitsNiveau[prio] < iInPrioriteitsNiveau[prio][inm])
                {
                    iPrioriteitsNiveau[prio] = iInPrioriteitsNiveau[prio][inm];
                }
                iPrioriteitsOpties[prio] |= iInPrioriteitsOpties[prio][inm];
            }
        }
    }

    PrioriteitsOpties();

    /* Trek prioriteiten in */
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        if (BL[fc] ||
            iAantalPrioriteitsInmeldingen[prio] == 0
            /* Rotterdam - Modificatie 1								*/		
		    /* Aanpassing: niet terugnemen ingezette prioriteit indien	*/
		    /* wachttijd tussentijds alsnog wordt overschreden			*/
		    /* 19-01-2009 / Peter Snijders								*/
		    /*|| !G[fc] && iMaximumWachtTijdOverschreden[prio] &&
                 !(iPrioriteitsOpties[prio] & poNoodDienst)*/
            )
        {
            iPrioriteit[prio] = 0;
        }
        iKOVPrio[prio] = -1;
        for (i = 0; i < prioGKFC_MAX[prio]; ++i)
        {
            kov = prioTO_pointer[prio][i];
            if (!(iPrioriteitNooitAfkappen[prio] && G[fc]) &&
                iPrioriteitsNiveau[kov] > iPrioriteitsNiveau[prio] && !iXPrio[kov] &&
                (!G[fc] || iPrioriteitsOpties[kov] & poAfkappenKonflikterendOV))
            {
                iPrioriteit[prio] = 0;
                iKOVPrio[prio] = kov;
            }
        }
    }
    /* Deel prioriteiten uit */
	for (prio = 0; prio < prioFCMAX; prio++)
    {
        fc = iFC_PRIOix[prio];
        if (!BL[fc] &&
            !iXPrio[prio] &&
            iKOVPrio[prio] == -1 &&
            iAantalInmeldingen[prio] > 0 &&
            !iPrioriteit[prio] &&
            (iPrioriteitsOpties[prio] & poNoodDienst ||
                !iSelDetFout[prio] &&
                !(!G[fc] && iMaximumWachtTijdOverschreden[prio]) &&
                (!G[fc] || !iOnderMaximumVerstreken[prio]) &&
                /* Werner : hieronder toegevoegd als beveiliging als ondermax niet is ingevuld */
                !(G[fc] && iMaximumWachtTijdOverschreden[prio] && (iOnderMaximum[prio] <= 0)) &&
                iBlokkeringsTimer[prio] >= iBlokkeringsTijd[prio] &&
                !iWachtOpKonflikt[prio])) 
        {
            iPrioriteit[prio] = 1;
            for (i = 0; iPrioriteit[prio] && i < prioGKFC_MAX[prio]; ++i)
            {
                kov = prioTO_pointer[prio][i];
                if (iPrioriteit[kov])
                {
                    iPrioriteit[prio] = 0;
                }
            }
        }
    }


    PrioriteitsToekenningExtra();
}

/* -----------------------------------------------------------
   AfkapGroen bepaalt van iedere richting fc hoeveel groen
   deze minimaal moet hebben gehad voordat deze door een
   niet-nooddienst mag worden afgekapt.
   Het resultaat wordt opgeslagen in de variabele
   iAfkapGroen[fc].
   iAfkapGroen[fc] is minimaal gelijk aan de afkapgroentijd
   iAfkapGroenTijd[fc].
   iAfkapGroen[fc] is minimaal gelijk aan de afkapgroentijd
   berekend uit het afkapgroenpercentage
   iInstPercMaxGroenTijdTerugKomen[fc] en de maximumgroentijd.
   Daarnaast wordt van iedere richting bepaald wat de maximale
   groentijd is waarmee bij afkappen de richting terugkomt.
   Deze waarde wordt opgeslagen in de variabele
   iMaxGroenTijdTerugKomen[fc] en wordt bepaald uit het
   maximumgroenpercentage voor terugkomen
   iInstPercMaxGroenTijdTerugKomen[fc] en de maximumgroentijd.
   ----------------------------------------------------------- */
void AfkapGroen(void)
{
    int fc, iAfkapGroen2;
    for (fc = 0; fc < FCMAX; ++fc)
    {
        if (iAfkapGroenTijd[fc] < TGG_max[fc])
        {
            iAfkapGroenTijd[fc] = TGG_max[fc];
        }
        if (iAfkapGroenTijd[fc] < TFG_max[fc])
        {
            iAfkapGroenTijd[fc] = TFG_max[fc];
        }
        iMaxGroen[fc] = TFG_max[fc] + (TVG_max[fc] >= 0 ? TVG_max[fc] : 0);
        iAfkapGroen[fc] = iAfkapGroenTijd[fc] < iMaxGroen[fc] ? iAfkapGroenTijd[fc] : iMaxGroen[fc];
        iAfkapGroen2 = (iPercGroenTijd[fc] + iOphoogPercentageMG[fc])*iMaxGroen[fc] / 100L;
        if (iAfkapGroen[fc] < iAfkapGroen2)
        {
            iAfkapGroen[fc] = iAfkapGroen2;
        }
        iMaxGroenTijdTerugKomen[fc] = iInstPercMaxGroenTijdTerugKomen[fc] * iMaxGroen[fc] / 100L;
    }
}

int BepaalRestGroen(int fc, int iPrioriteitsOptiesFC)
{
    int iRestGroen;

    if (PR[fc])
    {
        if (iPrioriteitsOptiesFC & poNoodDienst)
        {
            iRestGroen = G[fc] && TGG[fc] ? TGG_max[fc] - TGG_timer[fc] : 0;
        }
        else
        {
            if (iPrioriteitsOptiesFC & poAfkappenKonfliktRichtingen && !iNietAfkappen[fc]) 
            {
                iRestGroen = G[fc] && CV[fc] && iAfkapGroen[fc] >= iVerstrekenGroenTijd[fc] ? iAfkapGroen[fc] - iVerstrekenGroenTijd[fc] : 0;
            }
            else
            {
                iRestGroen = G[fc] && CV[fc] ? TFG_max[fc] - TFG_timer[fc] + TVG_max[fc] - TVG_timer[fc] : 0;
            }
        }
    }
    else
    {
        /* Konflikt k is niet primair gerealiseerd. */
        if (AR[fc] && iInstAfkapGroenAlt[fc] > TGG_max[fc] && !(iPrioriteitsOptiesFC & poNoodDienst))
        {
            /* Konflikt k is alternatief gerealiseerd en */
            /* er is geen sprake van een nooddienst.     */
            iRestGroen = G[fc] && iInstAfkapGroenAlt[fc] >= iVerstrekenGroenTijd[fc] ?
                         iInstAfkapGroenAlt[fc] - iVerstrekenGroenTijd[fc] : 
                         0;
        }
        else
        {
            iRestGroen = G[fc] && TGG[fc] ? TGG_max[fc] - TGG_timer[fc] : 0;
        }
    }
    return iRestGroen;
}

int StartGroenFC(int fc, int iGewenstStartGroen, int iPrioriteitsOptiesFC)
{
    int iStartGroenFC;
    int i, k, kprio;
    int iRestGroen, iRestGeel, iRestTO;

    iStartGroenFC = (GL[fc] ? (TGL_max[fc] > 0 ? TGL_max[fc] : 1) - TGL_timer[fc] : 0) +
                    (GL[fc] ? TRG_max[fc] : TRG[fc] ? TRG_max[fc] - TRG_timer[fc] : 0);
    if (iStartGroenFC < iGewenstStartGroen)
    {
        iStartGroenFC = iGewenstStartGroen;
    }

    for (i = 0; i < GKFC_MAX[fc]; ++i)
    {
#if (CCOL_V >= 95)
        k = KF_pointer[fc][i];
#else
        k = TO_pointer[fc][i];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
        if (TIG[k][fc]
#else
        if (TO[k][fc]
#endif
#ifdef NALOPEN
            || TGK[k][fc]
#endif
            )
        {
            iKPrioriteitsOpties[k] |= iPrioriteitsOptiesFC;
            if (PR[k])
            {
                if (iPrioriteitsOptiesFC & poNoodDienst)
                {
                    iRestGroen = G[k] && TGG[k] ? TGG_max[k] - TGG_timer[k] : 0;
                }
                else
                {
                    if (iPrioriteitsOptiesFC & poAfkappenKonfliktRichtingen &&
                        !iNietAfkappen[k]) {
                        iRestGroen = G[k] && CV[k] && iAfkapGroen[k] >= iVerstrekenGroenTijd[k] ? iAfkapGroen[k] - iVerstrekenGroenTijd[k] : 0;
                    }
                    else 
                    {
                        iRestGroen = G[k] && CV[k] ? TFG_max[k] - TFG_timer[k] + TVG_max[k] - TVG_timer[k] : 0;
                    }
                }
#ifdef NALOPEN
                if (TNL[k] && iRestGroen < TNL_max[k] - TNL_timer[k])
                {
                    iRestGroen = TNL_max[k] - TNL_timer[k];
                }
#endif
            }
            else
            {
                /* Konflikt k is niet primair gerealiseerd. */
                if (AR[k] && iInstAfkapGroenAlt[k] > TGG_max[k] && !(iPrioriteitsOptiesFC & poNoodDienst))
                {
                    /* Konflikt k is alternatief gerealiseerd en */
                    /* er is geen sprake van een nooddienst.     */
                    iRestGroen = G[k] && iInstAfkapGroenAlt[k] >= iVerstrekenGroenTijd[k] ? iInstAfkapGroenAlt[k] - iVerstrekenGroenTijd[k] : 0;
                }
                else
                {
                    iRestGroen = G[k] && TGG[k] ? TGG_max[k] - TGG_timer[k] : 0;
                }
            }
            iRestGeel = G[k] ? (TGL_max[k] > 0 ? TGL_max[k] : 1) : GL[k] ? (TGL_max[k] > 0 ? TGL_max[k] : 1) - TGL_timer[k] : 0;
#if (CCOL_V >= 95) && !defined NO_TIGMAX
            iRestTO = TIG_max[k][fc] >= 0 ? TIG_max[k][fc] - TIG_timer[k] :
#else
            iRestTO = TO_max[k][fc] >= 0 ? TO_max[k][fc] - TO_timer[k] :
#endif
#ifdef NALOPEN
                TGK[k][fc] ? TGK_max[k][fc] - TGK_timer[k] :
#endif
                0;
#if (CCOL_V >= 95) && !defined NO_TIGMAX
            if (TIG_max[k][fc] >= 0 && iStartGroenFC < iRestGroen + iRestTO)
            {
                iStartGroenFC = iRestGroen + iRestTO;
            }

#else
            if (TO_max[k][fc] >= 0 && iStartGroenFC < iRestGroen + iRestGeel + iRestTO)
            {
                iStartGroenFC = iRestGroen + iRestGeel + iRestTO;
            }
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
            if (TIG_max[k][fc] <= GK && iStartGroenFC < iRestGroen + iRestTO)
#else
            if (TO_max[k][fc] <= GK && iStartGroenFC < iRestGroen + iRestTO)
#endif
            {

                iStartGroenFC = iRestGroen + iRestTO;
            }
        }
    }
    for (kprio = 0; kprio < prioFCMAX; ++kprio)
    {
        k = iFC_PRIOix[kprio];
#if (CCOL_V >= 95) && !defined NO_TIGMAX
        if (TIG[k][fc] && iPrioriteit[kprio] && G[k])
#else
        if (TO[k][fc] && iPrioriteit[kprio] && G[k])
#endif
        {
            iRestGroen = iGroenBewakingsTijd[kprio] - iGroenBewakingsTimer[kprio];
            iRestGeel = G[k] ? (TGL_max[k] > 0 ? TGL_max[k] : 1) : GL[k] ? (TGL_max[k] > 0 ? TGL_max[k] : 1) - TGL_timer[k] : 0;
#if (CCOL_V >= 95) && !defined NO_TIGMAX
            iRestTO = TIG[k][fc] ? TIG_max[k][fc] - TIG_timer[k] : 0;
            if (TIG_max[k][fc] >= 0 && iStartGroenFC < iRestGroen + iRestTO)
#else
            iRestTO = TO[k][fc] ? TO_max[k][fc] - TO_timer[k] : 0;
            if (TO_max[k][fc] >= 0 && iStartGroenFC < iRestGroen + iRestGeel + iRestTO)
#endif
            {
                iStartGroenFC = iRestGroen + iRestGeel + iRestTO;
            }
#if (CCOL_V >= 95) && !defined NO_TIGMAX
            if (TIG_max[k][fc] == GK && iStartGroenFC < iRestGroen)
#else
            if (TO_max[k][fc] == GK && iStartGroenFC < iRestGroen)
#endif
            {
                iStartGroenFC = iRestGroen;
            }
        }
    }
    return iStartGroenFC;
}

/* ---------------------------------------------------------------
   StartGroenMomenten bepaalt van iedere OV-richting prio met
   prioriteit het startgroenmoment.
   Het resultaat wordt opgeslagen in de variabele iStartGroen[prio].
   iStartGroen[prio] is minimaal gelijk aan de resterende rijtijd.
   Als er lopende konflikten zijn die verhinderen dat de
   OV-richting bij het aflopen van de rijtijd groen wordt,
   dan wordt iStartGroen[prio] hierop aangepast.
   In dat geval wordt er rekening gehouden met:
   - het resterend groen van de lopende konflikten totdat
     er mag worden afgekapt.
   - het resterende deel van de geeltijd en de ontruimingstijd
     van de lopende konflikten.
   Het resterend groen van een lopend konflikt hangt af van:
   - heeft de OV-richting de prioriteitsoptie
     poAfkappenKonfliktRichtingen.
   - heeft de OV-richting de prioriteitsoptie poNoodDienst.
   - mag het lopende konflikt worden afgekapt.
   - het restant van de afkapgroentijd van de lopende konflikten.
   - is het lopend konflikt primair, alternatief of bijzonder
     gerealiseerd.
   --------------------------------------------------------------- */
void StartGroenMomenten(void)
{
    int prio, fc, iRestRijTijd;

    for (fc = 0; fc < FC_MAX; ++fc)
    {
        iKPrioriteitsOpties[fc] = 0;
    }

    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        iStartGroen[prio] = -1;
        if (iAantalInmeldingen[prio] > 0 && iPrioriteit[prio])
        {
            fc = iFC_PRIOix[prio];
            iRestRijTijd = iRijTijd[prio] >= iRijTimer[prio] ? iRijTijd[prio] - iRijTimer[prio] : 0;
            iStartGroen[prio] = StartGroenFC(fc, iRestRijTijd, iPrioriteitsOpties[prio]);
        }
    }
}

/* ------------------------------------------------------
   OVAanvragen zet de aanvragen voor de OV-richtingen.
   Een aanvraag wordt gezet als aan de volgende
   voorwaarden wordt voldaan:
   - De OV-richting heeft de prioriteitsoptie poAanvraag.
   - De rijtijd is verstreken, of er heeft zich een
     nooddienst ingemeld.
   ------------------------------------------------------ */
void PrioAanvragen(void)
{
    int prio, fc;

    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        if (iAantalInmeldingen[prio] > 0 &&
            iPrioriteitsOpties[prio] & poAanvraag && /*!iSelDetFout[prio] && */
            (!iSelDetFout[prio] && iRijTimer[prio] >= iRijTijd[prio] ||
                iPrioriteitsOpties[prio] & poNoodDienst || 
				iPrioriteitsOpties[prio] & poAfkappenKonfliktRichtingen && !(iPrioriteitsOpties[prio] & poBijzonderRealiseren))) /* OV-richting mag niet worden overgeslagen,
																															   zodat conflictrichtingen niet nogmaals kan realiseren 
																															   als er afgekapt is */
        {
            A[fc] |= !(EG[fc] || GL[fc]) ? PRIO_A_BIT : 0; /* R[fc] vervangen door !(EG[fc] || GL[fc]) 15-2-2016 */
        }
    }
}

void RealisatieTijden(int fc, int iPrioriteitsOptiesFC)
{
    int i, k, iGroenTijd;

    for (i = 0; i < GKFC_MAX[fc]; ++i)
    {
#if (CCOL_V >= 95)
        k = KF_pointer[fc][i];
#else
        k = TO_pointer[fc][i];
#endif
        if (!G[k])
        {
            if (iPrioriteitsOptiesFC & poNoodDienst)
            {
                iGroenTijd = TGG_max[k];
            }
            else
            {
                if (iPrioriteitsOptiesFC & poAfkappenKonfliktRichtingen)
                {
                    iGroenTijd = iAfkapGroen[k];
                }
                else
                {
                    iGroenTijd = TFG_max[k] + (TVG_max[k] > 0 ? TVG_max[k] : 0);
                }
            }
#if (CCOL_V >= 95) && !defined NO_TIGMAX
            if (TIG_max[k][fc] == GK)
#else
            if (TO_max[k][fc] == GK)
#endif
            {
                iRealisatieTijd[fc][k] = iKonfliktTijd[k] + iGroenTijd;
            }
            else
            {
                iRealisatieTijd[fc][k] = iKonfliktTijd[k] + iGroenTijd + (TGL_max[k] > 0 ? TGL_max[k] : 1) +
#if (CCOL_V >= 95) && !defined NO_TIGMAX
                TIG_max[k][fc];
#else
                TO_max[k][fc];
#endif
            }
        }
        else
        {
            iRealisatieTijd[fc][k] = -1;
        }
    }
}

void TegenHoudenStartGroen(int fc, int iStartGroenFC)
{
    int i, k;
    for (i = 0; i < GKFC_MAX[fc]; ++i)
    {
#if (CCOL_V >= 95)
        k = KF_pointer[fc][i];
#else
        k = TO_pointer[fc][i];
#endif
        if (iStartGroenFC <= iRealisatieTijd[fc][k])
        {
            RR[k] |= PRIO_RR_BIT;
        }
    }
}

/* -------------------------------------------------------
   OVTegenhouden zorgt voor het tegenhouden van konflikten
   van de OV-richtingen.
   Daartoe wordt het OV-bitje van de instructievariabele
   RR[fc] gebruikt.
   Bij een nooddienstinmelding wordt tevens het OV-bitje
   van de instructievariabele RTFB opgezet.
   Een konflikt wordt tegengehouden als een realisatie
   zou veroorzaken dat het startgroenmoment van een
   OV-richting niet meer haalbaar is.
   De te realiseren groentijd van de richting is
   afhankelijk van:
   - of de konflikterende OV-richting beschikt over de
     prioriteitsopties poBijzonderRealiseren, poNoodDienst
     en/of poAfkappenKonfliktRichtingen.
   ------------------------------------------------------- */
void PrioTegenhouden(void)
{
    int prio, fc;

    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        if (iPrioriteit[prio] && iPrioriteitsOpties[prio] & poBijzonderRealiseren)
        {
            fc = iFC_PRIOix[prio];
            RealisatieTijden(fc, iPrioriteitsOpties[prio]);
        }
    }
#ifdef PRIO_ADDFILE
    RealisatieTijden_Add();
#endif
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        if (iPrioriteit[prio] && iPrioriteitsOpties[prio] & poBijzonderRealiseren)
        {
            fc = iFC_PRIOix[prio];
            TegenHoudenStartGroen(fc, iStartGroen[prio]);
            if (iPrioriteitsOpties[prio] & poNoodDienst)
            {
                RTFB |= PRIO_RTFB_BIT;
            }
        }
    }
	TegenhoudenConflictenExtra();
}

void AfkappenStartGroen(int fc, int iStartGr)
{
    int i, k;

    for (i = 0; i < GKFC_MAX[fc]; ++i)
    {
#if (CCOL_V >= 95)
        k = KF_pointer[fc][i];
#else
        k = TO_pointer[fc][i];
#endif
        if (G[k] &&
#if (CCOL_V >= 95) && !defined NO_TIGMAX
            (TIG_max[k][fc] >= 0 && TIG_max[k][fc] >= iStartGr ||
#else
            (TO_max[k][fc] >= 0 && (TGL_max[k] > 0 ? TGL_max[k] : 1) + TO_max[k][fc] >= iStartGr ||
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
             TIG_max[k][fc] == GK && iStartGr <= 0 ||
             TIG_max[k][fc] == GKL 
#else
             TO_max[k][fc] == GK && iStartGr <= 0 ||
             TO_max[k][fc] == GKL 
#endif
#ifdef NALOPEN
				&& TGK_max[k][fc] >= iStartGr
#endif
            ))
        {
            if (!VS[k] && !FG[k]) Z[k] |= PRIO_Z_BIT;
            if (PR[k] && CV[k])
            {
                iAantalMalenNietAfkappen[k] = iInstAantalMalenNietAfkappen[k];
                if (iMaxGroenTijdTerugKomen[k] > iVerstrekenGroenTijd[k] + iGerealiseerdeGroenTijd[k] &&
                    iKPrioriteitsOpties[k] & poBijzonderRealiseren)
                {
                    iTerugKomen[k] = 1;
                    iTerugKomGroenTijd[k] = iMaxGroen[k] - iGerealiseerdeGroenTijd[k] - iVerstrekenGroenTijd[k];
                }
                iPercMGOphogen[k] = TRUE;
            }
        }
    }
}

void AfkappenMG(int fc, int iStartGr)
{
    int i, k;

    for (i = 0; i < GKFC_MAX[fc]; ++i)
    {
#if (CCOL_V >= 95)
        k = KF_pointer[fc][i];
#else
        k = TO_pointer[fc][i];
#endif
        if (MG[k] &&
#if (CCOL_V >= 95) && !defined NO_TIGMAX
			/* TIG_max[k][fc]==GKL toegevoegd */
            (TIG_max[k][fc] >= 0 && TIG_max[k][fc] >= iStartGr ||
             TIG_max[k][fc] == GK && iStartGr <= 0) || TIG_max[k][fc] == GKL 
#else
			/* TO_max[k][fc]==GKL toegevoegd */
            (TO_max[k][fc] >= 0 && (TGL_max[k] > 0 ? TGL_max[k] : 1) + TO_max[k][fc] >= iStartGr ||
             TO_max[k][fc] == GK && iStartGr <= 0) || TO_max[k][fc] == GKL 
#endif
#ifdef NALOPEN
			&& TGK_max[k][fc] >= iStartGr
#endif
		)
        {
            if (!VS[k] && !FG[k]) Z[k] |= PRIO_Z_BIT;
        }
    }
}

/* --------------------------------------------------------------------
   OVAfkappen zorgt voor het afkappen van de konflikten k van de
   OV-richtingen.
   Hiertoe wordt het OV-bitje van de instructievariabelen Z[k]
   en FM[k] gebruikt.
   Voor het realiseren van het startgroenmoment van een OV-richting
   wordt gebruik gemaakt van instructievariabele Z[k].
   Een richting wordt zo laat mogelijk afgekapt, maar tijdig genoeg
   om te voorkomen dat het startgroenmoment van de OV-richting in
   gevaar komt.
   Van een afgekapte richting wordt bepaald:
   - hoeveel keer deze niet mag worden afgekapt.
   - of deze terug mag komen en hoe lang.
   Daarnaast wordt het terugkomen van afgekapte richtingen afgehandeld.
   Hiertoe wordt gebruik gemaakt van de instructievariabele PP[] en
   het resetten van de PG[].
   Voor het realiseren van een verkorte cyclustijd bij
   overschrijding van de maximumwachttijd wordt gebruik gemaakt van
   de instructievariabele FM[].
   Hierbij worden richtingen die een konfliktaanvraag hebben afgekapt
   direct op het moment dat dat is toegestaan, d.w.z. als het
   afkapgroen is gerealiseerd.
   -------------------------------------------------------------------- */
void PrioAfkappen(void)
{
    int prio, fc, iTotaalAantalInmeldingen, iMaxWachtTijdOverschreden;
	int sml = -1;
#ifndef MLMAX
	int ml;
#endif

    iTotaalAantalInmeldingen = 0;
    iMaxWachtTijdOverschreden= 0;
  
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        iMaxWachtTijdOverschreden |= iMaximumWachtTijdOverschreden[prio]; /* toegevoegd AW 21-09-2011, ikv. per prio-richting */
        fc = iFC_PRIOix[prio];
        iTotaalAantalInmeldingen += !G[fc] ? iAantalInmeldingen[prio] : 0;
        if (iPrioriteit[prio] && iPrioriteitsOpties[prio] & (poAfkappenKonfliktRichtingen | poNoodDienst))
        {
            AfkappenStartGroen(fc, iStartGroen[prio]);
        }
        else
        {
            if (iPrioriteit[prio] && iPrioriteitsOpties[prio] & poBijzonderRealiseren)
            {
                AfkappenMG(fc, iStartGroen[prio]);
            }
        }
    }
    for (fc = 0; fc < FCMAX; fc++)
    {
        if (iMaxWachtTijdOverschreden && iTotaalAantalInmeldingen > 0)
        {
/* toevoeging Ane 100204 ikv. niet afbreken fc wanneer fc prioriteit moet hebben  */
            for (prio = 0; prio < prioFCMAX; ++prio) if (fc == iFC_PRIOix[prio]) break;
/* einde toevoeging */
            /* Versneld rondje                                        */
            /* Afkappen als dat mag en als er een konfliktaanvraag is */
            if (PR[fc] && G[fc] && CV[fc] &&
                iVerstrekenGroenTijd[fc] >= iAfkapGroen[fc] &&
                iTerugKomGroenTijd[fc] == 0 &&
                !iNietAfkappen[fc] &&
                ka(fc) &&
                !iPrioriteit[prio]) /* toevoeging Ane 20100204 ikv. niet afbreken fc wanneer fc prioriteit moet hebben  */
            {
                if (!VS[fc] && !FG[fc]) FM[fc] |= PRIO_FM_BIT;
            }
        }
        if (EG[fc] && !MK[fc] && !iTerugKomen[fc] || R[fc] && !TRG[fc] && !A[fc])
        {
            if (iAantalMalenNietAfkappen[fc])
            {
                iAantalMalenNietAfkappen[fc] = 0;
            }
            if (iNietAfkappen[fc])
            {
                iNietAfkappen[fc] = 0;
            }
            if (iTerugKomGroenTijd[fc])
            {
                iTerugKomGroenTijd[fc] = 0;
            }
            if (iTerugKomen[fc])
            {
                iTerugKomen[fc] = 0;
            }
        }

        /* ------------ */
        /* NietAfkappen */
        /* ------------ */
        if (EG[fc] && iNietAfkappen[fc])
        {
            (iAantalMalenNietAfkappen[fc])--;
            iNietAfkappen[fc] = 0;
        }
#ifdef MLMAX
		sml = SML;
#else
#ifdef MLAMAX
		if(sml == -1) for (ml = 0; ml < MLAMAX; ++ml) if (PRMLA[ml][fc] == PRIMAIR) { sml = SMLA; break; }
#endif
#ifdef MLBMAX
		if (sml == -1) for (ml = 0; ml < MLBMAX; ++ml) if (PRMLB[ml][fc] == PRIMAIR) { sml = SMLB; break; }
#endif
#ifdef MLCMAX
		if (sml == -1) for (ml = 0; ml < MLCMAX; ++ml) if (PRMLC[ml][fc] == PRIMAIR) { sml = SMLC; break; }
#endif
#ifdef MLDMAX
		if (sml == -1) for (ml = 0; ml < MLDMAX; ++ml) if (PRMLD[ml][fc] == PRIMAIR) { sml = SMLD; break; }
#endif
#ifdef MLEMAX
		if (sml == -1) for (ml = 0; ml < MLEMAX; ++ml) if (PRMLE[ml][fc] == PRIMAIR) { sml = SMLE; break; }
#endif
#endif
        if ((/* SG[fc] */PR[fc] && RA[fc] || sml && PG[fc] && G[fc]) && PR[fc] && iAantalMalenNietAfkappen[fc]>0 && !iNietAfkappen[fc]) 
        {
    	    /* wijz. Ane 14-11-2016 SG[fc] gewijzigd in PR[fc] && RA[fc], ivm. berekening iRealisatieTijd[fc][k], 
    	       zie ook RealisatieTijden(int fc, int iPrioriteitsOptiesFC) */
            iNietAfkappen[fc]=1;
        }
        
        /* ------------------ */
        /* OphoogPercentageMG */
        /* ------------------ */
        if (EG[fc] &&
            (!MK[fc] || iOphoogPercentageMG[fc] >= 100 - iPercGroenTijd[fc]))
		{
            iPercMGOphogen[fc] = FALSE;
            iOphoogPercentageMG[fc] = 0;
        }
        if (EG[fc] && iPercMGOphogen[fc])
        {
            iOphoogPercentageMG[fc] += iInstOphoogPercentageMG[fc];
            if (iOphoogPercentageMG[fc] >= 100 - iPercGroenTijd[fc])
            {
                iOphoogPercentageMG[fc] = 100 - iPercGroenTijd[fc];
            }
            iPercMGOphogen[fc] = FALSE;
        }
        if (R[fc] && !TRG[fc] && !A[fc])
        {
            iPercMGOphogen[fc] = FALSE;
            iOphoogPercentageMG[fc] = 0;
        }

        /* ---------- */
        /* TerugKomen */
        /* ---------- */
        if (iTerugKomGroenTijd[fc] > 0)
        {
            if (EG[fc])
            {
                if (!iTerugKomen[fc])
                {
                    iTerugKomGroenTijd[fc] = 0;
                }
                else
                {
                    /* ------------------------------------------------------------ */
                    /* Bij terugkomen geen ophoogpercentage van de maximumgroentijd */
                    /* ------------------------------------------------------------ */
                    iPercMGOphogen[fc] = FALSE;
                    iOphoogPercentageMG[fc] = 0;
                    if (iTerugKomGroenTijd[fc] < iInstMinTerugKomGroenTijd[fc])
                    {
                        iTerugKomGroenTijd[fc] = iInstMinTerugKomGroenTijd[fc];
                    }
                }
            }
            if (SG[fc] && iTerugKomen[fc]) /* wijzigen Ane 031011 ikv. niet manipuleren TVG_max[fc] */
            {
                iTerugKomen[fc] = 0;
                iTerugGekomen[fc]=1;
            }
        }
        if (iTerugKomen[fc])
        {
            if (G[fc] && !(Z[fc] & PRIO_Z_BIT) && !(FM[fc] & PRIO_FM_BIT))
            {
                /* Konflikterende prioriteit is ingetrokken
                   Er is dus niet langer reden richting fc
                   af te kappen.
                   Het resterende groen kan alsnog
                   in de huidige realisatie worden afgemaakt. */
                iPercMGOphogen[fc] = FALSE;
                iTerugKomen[fc] = FALSE;
                RW[fc] |= PRIO_RW_BIT;
            }
            if ((PG[fc] || !G[fc]) && (GL[fc] || TRG[fc] || A[fc]))
            {
                PP[fc] |= PRIO_PP_BIT;
                if (PG[fc])
                {
                    PG[fc] = 0;
                }
            }
            else
            {
                iTerugKomen[fc] = 0;
                iTerugKomGroenTijd[fc] = 0;
            }
        }
        if(iTerugGekomen[fc]) /* toegevoegd Ane 031011 ikv. niet manipuleren TVG_max[fc] */
        {        
            if(TVG_timer[fc]>=(iTerugKomGroenTijd[fc]-TFG_max[fc]))
            {
                MK[fc]&= 0;
                iTerugGekomen[fc]=0;
            }
        }
    }
}

/* --------------------------------------------------------------
   OVBijzonderRealiseren zorgt voor het bijzonder realiseren van
   OV-richtingen.
   Hiertoe wordt gebruik gemaakt van de CCOL-functies set_CALW en
   set_PRILW.
   Bij onverhoopte intrekking van de prioriteit wordt de waarde
   van CALW[] hersteld.
   Een OV-richting wordt bijzonder gerealiseerd als diens
   startgroenmoment is aangebroken en de OV-richting over de
   prioriteitsoptie poBijzonderRealiseren beschikt.
   -------------------------------------------------------------- */
void PrioBijzonderRealiseren(void)
{
    int prio, fc;

    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        if (iPrioriteit[prio] &&
            iStartGroen[prio] == 0 &&
            iPrioriteitsOpties[prio] & poBijzonderRealiseren)
        {
            iBijzonderRealiseren[prio] = 1;
            /* voorkeuraanvraag openbaar vervoer */
            if (CALW[fc] < PRI_CALW)
            {
                set_CALW(fc, PRI_CALW);
            }
            /* voorkeurrealisatie openbaar vervoer */
            if (CALW[fc] >= PRI_CALW)
            {
                set_PRIRLW(fc, TRUE); 
                /* Ane 28-02-2013: set_PRILW(fc, TRUE) in LWMLFUNC.C vervangen door set_PRIRLW(fc, TRUE) in ccolfunc.c
                   voor reden wordt verwezen naar de definitie van deze procedure in ccolfunc.c */
            }
        }
        else
        {
            if (iBijzonderRealiseren[prio])
            {
                /* voorkeurrealisatie openbaar vervoer resetten
                   indien richting nog geen groen heeft gehad,
                   maar ingreep niet langer actief is.          */
                if (CALW[fc] >= PRI_CALW)
                {
                    set_CALW(fc, (mulv)(10 * TFB_timer[fc]));
                }
            }
            iBijzonderRealiseren[prio] = 0;
        }
    }
}

/* ---------------------------------------------------------
   OVGroenVasthouden zorgt voor het vasthouden van het groen
   van OV-richtingen.
   Hiertoe wordt gebruik gemaakt van het OV-bitje van de
   instructievariabele YV[].
   Het groen van een OV-richting wordt vastgehouden als deze
   prioriteit heeft, deze beschikt over de prioriteitsoptie
   poGroenVastHouden, en de groenbewaking nog niet is
   aangesproken.
   --------------------------------------------------------- */
void PrioGroenVasthouden(void)
{
    int prio, fc;

    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        if (iPrioriteit[prio] &&
            iPrioriteitsOpties[prio] & poGroenVastHouden)
        {
            fc = iFC_PRIOix[prio];
            if (iGroenBewakingsTimer[prio] < iGroenBewakingsTijd[prio])
            {
               if (MG[fc]) YM[fc] |= PRIO_YM_BIT; /* toevoeging Ane, 2019-08-05: aangepast van RW naar YM door Menno */
               YV[fc] |= PRIO_YV_BIT;
               /* 15-02-2017 Werner : FM bit van alternatieven intrekken, om 'flipperen' te voorkomen */ 
               /*                     we willen het groen vasthouden en niet be?indigen!              */
               FM[fc] &= ~BIT5;
            }
        }
    }
}

/* ------------------------------------------------------------
   OVMeetKriterium zorgt voor het opzetten van het
   OV-bitje van de instructievariabele MK[] van de
   OV-richtingen.
   Het meetkriterium wordt opgezet als zich ??n van de volgende
   situaties voordoet:
   - De OV-richting heeft prioriteit, beschikt over de
     prioriteitsoptie poGroenVastHouden en de groenbewaking is
     niet aangesproken.
   - De OV-richting is primair gerealiseerd en het resterende
     deel van zijn groenbewaking past binnen de resterende
     maximumgroentijd.
   Daarnaast wordt het meetkriterium van de OV-richting gereset
   als de richting bijzonder is gerealiseerd, maar het groen
   niet mag vasthouden. Het overige verkeer mag het groen van
   de OV-richting in dat geval niet verlengen.
   ------------------------------------------------------------ */
void PrioMeetKriterium(void)
{
    int prio, fc, iRestGroen;

    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        if (G[fc])
        {
            iRestGroen = (TFG_max[fc] - TFG_timer[fc]) +
                (TVG_max[fc] >= TVG_timer[fc] ? TVG_max[fc] - TVG_timer[fc] : 0);
            if (iPrioriteit[prio] &&
                iPrioriteitsOpties[prio] & poGroenVastHouden &&
                iGroenBewakingsTimer[prio] < iGroenBewakingsTijd[prio] ||
                PR[fc] & PRIMAIR_VERSNELD &&
                (iGroenBewakingsTimer[prio] < iGroenBewakingsTijd[prio] ||
                 iAantalInmeldingen[prio]>0 && !ka(fc)) &&
                iGroenBewakingsTijd[prio] - iGroenBewakingsTimer[prio] <= iRestGroen)
            {
                MK[fc] |= PRIO_MK_BIT;
            }
            else
            {
                if (!(PR[fc] & PRIMAIR_VERSNELD) && !AR[fc])
                {
                    MK[fc] = 0;
                }
            }
        }
    }
}

/* ---------------------------------------------------------------
   OVAlternatieven zorgt voor het alternatief realiseren van
   richtingen die niet konflikteren met OV-richtingen met
   een ingreep.
   Hiertoe wordt o.a. gebruik gemaakt van de instructievariabele
   PAR[] en de functies max_tar_ov en langstwachtende_alternatief.
   Een richting fc die niet konflikteert met de OV-richtingen met
   prioriteit kan alternatief bijkomen als aan alle volgende
   voorwaarden is voldaan:
   - iSCH_ALTG[fc] is waar.
   - de alternatieve ruimte is minimaal gelijk aan iPRM_ALTP[fc].
   --------------------------------------------------------------- */
void PrioAlternatieven(void)
{
    int fc, prio, iLWAlt = 0;

    for (fc = 0; fc < FCMAX; ++fc)
    {
        PAR[fc] &= ~PRIO_PAR_BIT;
        FM[fc] &= ~BIT4;
        RR[fc] &= ~BIT4;
    }

    for (fc = 0; fc < FCMAX; ++fc)
    {
        if (iSCH_ALTG[fc] && !PAR[fc] && !RR[fc]) /* wijz. Ane 8-10-2015: !RR[fc] toegevoegd ivm evt. ander conflicterend prio-richting met fc */
        {
            for (prio = 0; prio < prioFCMAX; ++prio)
            {
                if (iT_GBix[prio] >= 0 && iT_GBix[prio] < TMMAX &&
                    iH_PRIOix[prio] >= 0 && iH_PRIOix[prio] < HEMAX &&
                    iPrioriteitsOpties[prio] & poGroenVastHouden &&
					iPrioriteit[prio])
                {
#if (CCOL_V >= 95) && !defined NO_TIGMAX
                    if (TIG_max[fc][iFC_PRIOix[prio]] == NG) /* voorwaarde toegevoegd Ane 17-01-2012 */
#else
                    if (TO_max[fc][iFC_PRIOix[prio]] == NG) /* voorwaarde toegevoegd Ane 17-01-2012 */
#endif
                    {
                        PAR[fc] |= (((max_tar_ov(fc, iFC_PRIOix[prio], iT_GBix[prio], iH_PRIOix[prio], END) >= iPRM_ALTP[fc])) && iSCH_ALTG[fc]) ? PRIO_PAR_BIT : 0;
                    }

                    iLWAlt |= PAR[fc];
                }

                if (iT_GBix[prio]>=0 && iT_GBix[prio]<TMMAX &&
                    iH_PRIOix[prio]>=0 && iH_PRIOix[prio]<HEMAX &&
                    iPrioriteitsOpties[prio] & poBijzonderRealiseren &&
					iPrioriteit[prio])
		        {
#if (CCOL_V >= 95) && !defined NO_TIGMAX
					if (TIG_max[fc][iFC_PRIOix[prio]] == NG) /* voorwaarde toegevoegd Ane 17-01-2012 */
#else
					if (TO_max[fc][iFC_PRIOix[prio]] == NG) /* voorwaarde toegevoegd Ane 17-01-2012 */
#endif
                    {
                        PAR[fc] |= (IH[iH_PRIOix[prio]] && R[iFC_PRIOix[prio]] && iSCH_ALTG[fc]) ? PRIO_PAR_BIT : 0;
                    }
                }
            }
        }
    }
	PrioPARCorrecties();
#ifdef PRIO_ADDFILE
	PrioAlternatieven_Add();
#endif

    for (fc = 0; fc < FCMAX; ++fc)
    { 
        /* Resetten RR Bit 5 als PAR alsnog wordt opgezet door OV */
        if ((PAR[fc] & PRIO_PAR_BIT) && R[fc] && !ERA[fc]) RR[fc] &= ~BIT5;
    }
    langstwachtende_alternatief_bit6();
    for (fc = 0; fc < FCMAX; ++fc)
    {
        if(AR[fc]&BIT6 && !PAR[fc])
        {
            RR[fc] |= (RA[fc])? BIT4 : 0; /* toegevoegd Ane 12-01-2015; resetten alternatieve realisatie, */
            FM[fc] |= G[fc]? BIT4 : 0;  /* indien OV-ingreep niet meer actief is                        */
        }
    }
}

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined (VISSIM)
int PrioriteitsOpties2PRM(int iPO)
{
    int iReturn = 0;

    if (iPO & poNoodDienst)

    {
        iReturn = 5;
    }
    else
    {
        if (iPO & poAfkappenKonfliktRichtingen &&
            !(iPO & poAfkappenKonflikterendOV)) {
            iReturn *= 10;
            iReturn += 1;
        }
        if (iPO & poGroenVastHouden)
        {
            iReturn *= 10;
            iReturn += 2;
        }
        if (iPO & poBijzonderRealiseren)
        {
            iReturn *= 10;
            iReturn += 3;
        }
        if (iPO & poAfkappenKonflikterendOV)
        {
            iReturn *= 10;
            iReturn += 4;
        }
    }
    return iReturn;
}
#endif

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined(VISSIM)

/* -------------------------------------------------------------------
   OVDebug toont in het debugscherm (F11) de volgende informatie:
   - gegevens van een OV-richting.
   - gegevens van de inmeldingen van de OV-richting.
   - gegevens van de lopende konflikten van de OV-richting.
   De te tonen OV-richting wordt als volgt bepaald:
   - Heeft geen enkele OV-richting een inmelding, dan wordt hiervoor
     prio genomen.
   - Zijn er wel OV-richtingen met inmeldingen, maar zijn er geen
     prioriteiten toegekend, dan wordt de OV-richting met de kleinste
     index gekozen.
   - Zijn er wel OV-richtingen met prioriteit, dan wordt van die groep
     die met de kleinste index gekozen.
   ------------------------------------------------------------------- */
void PrioDebug(int ov)
{
    int fc, inm, i, k;
    int y = 1;
    int prio2, prio3;
    static int y_max = 0;
#define Y_MAX        65

    if (ov < 0 || iAantalInmeldingen[ov] == 0)
    {
        for (prio2 = 0, prio3 = -1; prio2 < prioFCMAX; ++prio2)
        {
            if (iAantalInmeldingen[prio2] > 0 && (prio3 == -1 || !iPrioriteit[prio3] && iPrioriteit[prio2]))
            {
                prio3 = prio2;
            }
        }
        if (prio3 == -1)
        {
            prio3 = ov;
        }
        ov = prio3;
    }

    if (y < Y_MAX)
    {
		if (y < Y_MAX) { xyprintf(1, y, "MaxWTOverschreden     =%4d ", iMaximumWachtTijdOverschreden[ov]);  y++; }
    }
    if (y < Y_MAX)
    {
        xyprintf(1, y, "___________________________");                                ++y;
    }
    if (ov >= 0 && ov < prioFCMAX)
    {
        fc = iFC_PRIOix[ov];
        if (y < Y_MAX)
        {
            xyprintf(1, y, "PRIO fc%s                     ", FC_code[fc]);                 ++y;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "%s         ",
                iRijTijdScenario[ov] == rtsOngehinderd ? "Ongehinderd" :
                iRijTijdScenario[ov] == rtsBeperktGehinderd ? "BeperktGehinderd" :
                iRijTijdScenario[ov] == rtsGehinderd ? "Gehinderd" : "?");y++;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "SelDetFout            =%4d ", iSelDetFout[ov]);                ++y;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "AantalInmeldingen     =%4d ", iAantalInmeldingen[ov]);         ++y;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "StartGroen            =%4d ", iStartGroen[ov]);                ++y;
        }

        if (!iPrioriteit[ov] && iXPrio[ov])

        {
            if (y < Y_MAX)
            {
                xyprintf(1, y, "Prioriteit            =%4s ", "X");                          ++y;
            }
        }
        else {
            if (y < Y_MAX)
            {
                xyprintf(1, y, "Prioriteit            =%4d ", iPrioriteit[ov]);              ++y;
            }
        }

        if (y < Y_MAX)

        {
            xyprintf(1, y, "XPrio                 =%4d ", iXPrio[ov]);                     ++y;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "OnderMaximumVerstreken=%4d ", iOnderMaximumVerstreken[ov]);    ++y;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "BlokkeringsTimer      =%4d ",
                iBlokkeringsTimer[ov] < iBlokkeringsTijd[ov] ?
                iBlokkeringsTimer[ov] : -1);                                  ++y;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "WachtOpKonflikt       =%4d ", iWachtOpKonflikt[ov]);           ++y;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "RijTimer              =%4d ", iRijTimer[ov]);                  ++y;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "RijTijd               =%4d ", iRijTijd[ov]);                   ++y;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "Aanvraag              =%4d ", A[fc]);                          ++y;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "PrioriteitsOpties     = %03d ",
                PrioriteitsOpties2PRM(iPrioriteitsOpties[ov]));                    ++y;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "GroenBewakingsTimer   =%4d ", iGroenBewakingsTimer[ov]);       ++y;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "GroenBewakingsTijd    =%4d ", iGroenBewakingsTijd[ov]);        ++y;
        }
        if (y < Y_MAX)
        {
            xyprintf(1, y, "___________________________");                                ++y;
        }
        for (inm = 0; inm < iAantalInmeldingen[ov]; ++inm)
        {
            if (y < Y_MAX)
            {
                xyprintf(1, y, "Inmelding %d                  ", inm);                                ++y;
            }
            if (y < Y_MAX)
            {
                xyprintf(1, y, "RijTimer              =%4d ", iInRijTimer[ov][inm]);                  ++y;
            }
            if (y < Y_MAX)
            {
                xyprintf(1, y, "PrioriteitsOpties     = %03d ",
                    PrioriteitsOpties2PRM(iInPrioriteitsOpties[ov][inm]));                    ++y;
            }
            if (y < Y_MAX)
            {
                xyprintf(1, y, "PrioriteitsNiveau     =%4d ", iInPrioriteitsNiveau[ov][inm]);         ++y;
            }
            if (y < Y_MAX)
            {
                xyprintf(1, y, "GroenBewakingsTimer   =%4d ", iInGroenBewakingsTimer[ov][inm]);       ++y;
            }
            if (y < Y_MAX)
            {
                xyprintf(1, y, "OnderMaximumVerstreken=%4d ", iInOnderMaximumVerstreken[ov][inm]);    ++y;
            }
            if (y < Y_MAX)
            {
                xyprintf(1, y, "MaxWachtTijdOverschred=%4d ", iInMaxWachtTijdOverschreden[ov][inm]);    ++y;
            }
            if (y < Y_MAX)
            {
                xyprintf(1, y, "___________________________");                                       ++y;
            }
        }
        for (i = 0; i < GKFC_MAX[fc]; ++i)
        {
#if (CCOL_V >= 95)
			k = KF_pointer[fc][i];
#else
            k = TO_pointer[fc][i];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
            if (TIG[k][fc] || iTerugKomen[k] || AAPR[k] || AR[k])
#else
            if (TO[k][fc] || iTerugKomen[k] || AAPR[k] || AR[k])
#endif
            {
                if (y < Y_MAX)
                {
                    xyprintf(1, y, "Konflikt fc%2s                 ", FC_code[k]);                        ++y;
                }
                if (y < Y_MAX)
                {
                    xyprintf(1, y, "VerstrekenGroenTijd   =%4d ", iVerstrekenGroenTijd[k]);               ++y;
                }
                if (y < Y_MAX)
                {
                    xyprintf(1, y, "MaxGroenTijdTerugKomen=%4d ", iMaxGroenTijdTerugKomen[k]);            ++y;
                }
                if (y < Y_MAX)
                {
                    xyprintf(1, y, "NietAfkappen          =%4d ", iNietAfkappen[k] ?
                        iAantalMalenNietAfkappen[k] :
                        iNietAfkappen[k]);                      ++y;
                }
                if (y < Y_MAX)
                {
                    xyprintf(1, y, "TerugKomen            =%4d ", iTerugKomen[k]);                        ++y;
                }
                if (y < Y_MAX)
                {
                    xyprintf(1, y, "TerugKomGroenTijd     =%4d ", iTerugKomGroenTijd[k]);                 ++y;
                }
                if (y < Y_MAX)
                {
                    xyprintf(1, y, "GerealiseerdeGroenTijd=%4d ", iGerealiseerdeGroenTijd[k]);            ++y;
                }
                if (y < Y_MAX)
                {
                    xyprintf(1, y, "AfkapGroen            =%4d ", iAfkapGroen[k]);                        ++y;
                }
                if (y < Y_MAX)
                {
                    xyprintf(1, y, "RealisatieTijd        =%4d ", iRealisatieTijd[fc][k]);                ++y;
                }
                if (y < Y_MAX)
                {
                    xyprintf(1, y, "KonfliktTijd          =%4d ", iKonfliktTijd[k]);                      ++y;
                }
                if (y < Y_MAX)
                {
                    xyprintf(1, y, "___________________________");                                       ++y;
                }
            }
        }
    }
    for (;y < y_max;++y)
    {
        xyprintf(1, y, "                            ");
    }
    y_max = y;
}
#endif

void AfhandelingPrio(void)
{
    static int init = 1;

    /* -------------
       Initialisatie
       ------------- */
    if (init)
    {
        PrioInit();
		PrioInitExtra();
		init = 0;
    }

    /* ------------------------------------------------------------
       Vaststellen rijtijdscenarios
       ------------------------------------------------------------ */
    RijTijdScenario();
#ifdef PRIO_ADDFILE
    RijTijdScenario_Add();
#endif

    /* --------------------
       In - en uitmeldingen
       -------------------- */
    InUitMelden();
#ifdef PRIO_ADDFILE
    InUitMelden_Add();
#endif

    /* ---------------
       OV-instellingen
       --------------- */
	PrioInstellingen();
#ifdef PRIO_ADDFILE
	PrioInstellingen_Add();
#endif

	PrioTimers();
    KonfliktTijden();
#ifdef PRIO_ADDFILE
    KonfliktTijden_Add();
#endif

    WachtTijdBewaking();
#ifdef PRIO_ADDFILE
    WachtTijdBewaking_Add();
#endif

	OnderMaximum();
	OnderMaximumExtra();
#ifdef PRIO_ADDFILE
    OnderMaximum_Add();
#endif

    BlokkeringsTijd();
#ifdef PRIO_ADDFILE
    BlokkeringsTijd_Add();
#endif

    PrioriteitsToekenning();
#ifdef PRIO_ADDFILE
    PrioriteitsToekenning_Add();
#endif

	AfkapGroen();
	AfkapGroenExtra();
#ifdef PRIO_ADDFILE
    AfkapGroen_Add();
#endif

	StartGroenMomenten();
	StartGroenMomentenExtra();
#ifdef PRIO_ADDFILE
    StartGroenMomenten_Add();
#endif

    /* ------------------------------------------------------
       Als de rijtijd verstreken is, wordt de aanvraag gezet.
       ------------------------------------------------------ */
	PrioAanvragen();
#ifdef PRIO_ADDFILE
	PrioAanvragen_Add();
#endif

    /* ------------------------------------------------
       Konflikten worden tegengehouden op basis van het
       StartGroenMoment.
       ------------------------------------------------ */
	PrioTegenhouden();
#ifdef PRIO_ADDFILE
	PrioTegenhouden_Add();
#endif

    /* -------------------------------------------
       Konflikten worden afgekapt op basis van het
       StartGroenMoment.
       ------------------------------------------- */
	PrioAfkappen();
	PrioAfkappenExtra();
#ifdef PRIO_ADDFILE
	PrioAfkappen_Add();
#endif

    /* ----------------------------------------------------------
       TVG_max wordt aangepast op basis van de TerugKomGroenTijd.
       ---------------------------------------------------------- */
	TerugKomGroen();
	PrioTerugkomGroenExtra();

    /* ---------------------------------------------------------
       Bijzonder realiseren als het StartGroenMoment is bereikt.
       --------------------------------------------------------- */
	PrioBijzonderRealiseren();

    /* ----------------------------------------------------
       Groen vasthouden tot uitmelding of aanspreken van de
       groenbewaking.
       ---------------------------------------------------- */
	PrioGroenVasthouden();
	PrioGroenVasthoudenExtra();

    /* ---------------------------------------------------------
       Meetkriterium van een bijzonder gerealiseerde richting
       afzetten zodat, bij uitmelding de richting naar rood gaat.
       --------------------------------------------------------- */
	PrioMeetKriterium();
	PrioMeetKriteriumExtra();

    /* ------------------------------------------
       Kopieer de waarden naar de Ccol-elementen.
       ------------------------------------------ */
	PrioCcol();

    /* -------------------------------------------------------------
       Alternatieve realisaties van niet konflikten tijdens ingreep,
       uitgaande van de resterende groenbewakingstijd.
       ------------------------------------------------------------- */
	PrioAlternatieven();

    PostAfhandelingPrio();
#ifdef PRIO_ADDFILE
	PostAfhandelingPrio_Add();
#endif

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined(VISSIM)
#ifdef PRIO_ADDFILE
	PrioDebug_Add();
#endif
#endif
}
