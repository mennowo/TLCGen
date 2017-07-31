/* OVPROGRAMMA */
/* ----------- */

#define MAX_INT                       32767L


typedef enum {
  poGeenPrioriteit             =0,
  poAanvraag                   =1,
  poAfkappenKonfliktRichtingen =2,
  poGroenVastHouden            =4,
  poBijzonderRealiseren        =8,
  poAfkappenKonflikterendOV    =16,
  poNoodDienst                 =32,
} TPrioriteitsOpties;

typedef enum {
  rtsOngehinderd,
  rtsBeperktGehinderd,
  rtsGehinderd,
} TRijTijdScenario;

VLOG_MON5 VLOG_mon5[FCMAX];

int iMaximumWachtTijdOverschreden;
int iMaximumWachtTijd[FCMAX];
int iVerstrekenGroenTijd[FCMAX];
int iGerealiseerdeGroenTijd[FCMAX];
int iAfkapGroen[FCMAX];
int iAfkapGroenTijd[FCMAX];
int iPercGroenTijd[FCMAX];
int iKonfliktTijd[FCMAX];
int iTerugKomGroenTijd[FCMAX];
int iTerugKomen[FCMAX];
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
int iT_GBix[ovOVMAX];
int iH_OVix[ovOVMAX];
int iBlokkeringsTijd[ovOVMAX];
int iBlokkeringsTimer[ovOVMAX];
int iFC_OVix[ovOVMAX];
int iOnderMaximum[ovOVMAX];
int iOnderMaximumVerstreken[ovOVMAX];
int iGroenBewakingsTijd[ovOVMAX];
int iGroenBewakingsTimer[ovOVMAX];
int iRijTijd[ovOVMAX];
int iRijTimer[ovOVMAX];
int iPrioriteit[ovOVMAX];
int iKOVPrio[ovOVMAX];
int iInstPrioriteitsNiveau[ovOVMAX];
int iInstPrioriteitsOpties[ovOVMAX];
int iPrioriteitsNiveau[ovOVMAX];
int iPrioriteitsOpties[ovOVMAX];
int iKPrioriteitsOpties[FCMAX];
int iStartGroen[ovOVMAX];
int iBijzonderRealiseren[ovOVMAX];
int iWachtOpKonflikt[ovOVMAX];
bool bMagEerst[FCMAX];
int iAantalPrioriteitsInmeldingen[ovOVMAX];
int iRijTijdScenario[ovOVMAX];
int iRTSOngehinderd[ovOVMAX];
int iRTSBeperktGehinderd[ovOVMAX];
int iRTSGehinderd[ovOVMAX];
int iSelDetFoutNaGB[ovOVMAX];
int iSelDetFout[ovOVMAX];
int iAantalInmeldingen[ovOVMAX];
int iXPrio[ovOVMAX];

int *iRealisatieTijd[FCMAX];
int *iInPrioriteitsNiveau[ovOVMAX];
int *iInPrioriteitsOpties[ovOVMAX];
int *iInRijTimer[ovOVMAX];
int *iInGroenBewakingsTimer[ovOVMAX];
int *iInOnderMaximumVerstreken[ovOVMAX];
int *iInID[ovOVMAX];

int iM_RealisatieTijd[FCMAX*FCMAX];
int iM_InPrioriteitsNiveau[ovOVMAX * MAX_AANTAL_INMELDINGEN];
int iM_InPrioriteitsOpties[ovOVMAX * MAX_AANTAL_INMELDINGEN];
int iM_InRijTimer[ovOVMAX * MAX_AANTAL_INMELDINGEN];
int iM_InGroenBewakingsTimer[ovOVMAX * MAX_AANTAL_INMELDINGEN];
int iM_InOnderMaximumVerstreken[ovOVMAX * MAX_AANTAL_INMELDINGEN];
int iM_InID[ovOVMAX * MAX_AANTAL_INMELDINGEN];

int ovKFC_MAX[ovOVMAX];
int ovGKFC_MAX[ovOVMAX];
int *ovTO_pointer[ovOVMAX];
int ovM_TO_pointer[ovOVMAX*ovOVMAX];
int iLangstWachtendeAlternatief;

void OVInit(void) {
  int ov1, ov2,
      fc1, fc2,
      fc;

  /* default OV-instellingen */
  for (fc=0;fc<FCMAX;fc++) {
    iMaximumWachtTijd[fc]=DEFAULT_MAX_WACHTTIJD;
    iPRM_ALTP[fc]=TFG_max[fc];
    iSCH_ALTG[fc]=TRUE;
    iRealisatieTijd[fc] = iM_RealisatieTijd+(fc*FCMAX);
  }
  /* werkelijke OV-instellingen */
  OVInstellingen();
  /* initialisatie overige OV-variabelen */
  for (ov1=0;
       ov1<ovOVMAX;
       ov1++) {
    fc1=iFC_OVix[ov1];
    ovTO_pointer[ov1] = ovM_TO_pointer+(ov1*ovOVMAX);
    ovKFC_MAX[ov1]=0;
    for (ov2=0;
         ov2<ovOVMAX;
         ov2++) {
      fc2=iFC_OVix[ov2];
      if (TO_max[fc1][fc2]>=0) {
        ovTO_pointer[ov1][ovKFC_MAX[ov1]]=ov2;
        (ovKFC_MAX[ov1])++;
      }
    }
    iBlokkeringsTimer[ov1]         = MAX_INT;
    iInPrioriteitsNiveau[ov1]      = iM_InPrioriteitsNiveau+(ov1*MAX_AANTAL_INMELDINGEN);
    iInPrioriteitsOpties[ov1]      = iM_InPrioriteitsOpties+(ov1*MAX_AANTAL_INMELDINGEN);
    iInRijTimer[ov1]               = iM_InRijTimer+(ov1*MAX_AANTAL_INMELDINGEN);
    iInGroenBewakingsTimer[ov1]    = iM_InGroenBewakingsTimer+(ov1*MAX_AANTAL_INMELDINGEN);
    iInOnderMaximumVerstreken[ov1] = iM_InOnderMaximumVerstreken+(ov1*MAX_AANTAL_INMELDINGEN);
    iInID[ov1]                     = iM_InID+(ov1*MAX_AANTAL_INMELDINGEN);
    iPrioriteit[ov1]               = FALSE;
    iAantalInmeldingen[ov1]        = 0;
  }
  for (ov1=0;
       ov1<ovOVMAX;
       ov1++) {
    fc1=iFC_OVix[ov1];
    ovGKFC_MAX[ov1]=ovKFC_MAX[ov1];
    for (ov2=0;
         ov2<ovOVMAX;
         ov2++) {
      fc2=iFC_OVix[ov2];
      if (TO_max[fc1][fc2]==GK) {
        ovTO_pointer[ov1][ovGKFC_MAX[ov1]]=ov2;
        (ovGKFC_MAX[ov1])++;
      }
    }
  }
}

int BepaalPrioriteitsOpties(int prm_prio) {
  int p, iReturn;

  for (iReturn=0, p = PRM[prm_prio] % 1000L;
       p>0;
       p/=10L) {
    switch(p%10) {
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
void KonfliktTijden(void) {
  int fc, i, k, iKT, iRestGroen, iRestGeel, iRestTO;

  for (fc=0;
       fc<FCMAX;
       fc++) {
    iKonfliktTijd[fc]=(GL[fc] ? (TGL_max[fc]>0 ? TGL_max[fc] : 1) - TGL_timer[fc] : 0) +
                      (GL[fc] ? TRG_max[fc] : TRG[fc] ? TRG_max[fc] - TRG_timer[fc] : 0);
    if (K[fc]) {
      for (i=0;
           i<GKFC_MAX[fc];
           i++) {
        k = TO_pointer[fc][i];
        if (TO[k][fc]) {
          iRestGroen = TGG[k] ? TGG_max[k] - TGG_timer[k] : 0;
          iRestGeel = (TGL_max[k]>0 ? TGL_max[k] : 1) - TGL_timer[k];
          iRestTO = TO_max[k][fc] - TO_timer[k];
          if (TO_max[k][fc] == GK) {
            iKT = iRestGroen;
          } else {
            iKT = iRestGroen + iRestGeel + iRestTO;
          }
          if (iKonfliktTijd[fc] < iKT) {
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
void TerugKomGroen(void) {
  int fc;

  for (fc=0;fc<FCMAX;fc++) {
    if (iTerugKomGroenTijd[fc]>0 &&
        !iTerugKomen[fc]) {
      TVG_max[fc]=iTerugKomGroenTijd[fc]-TFG_max[fc];
      if (TVG_max[fc]<0) {
        TVG_max[fc]=0;
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
void OVTimers(void) {
  int fc, inm, ov;

  for (fc=0;fc<FCMAX;fc++) {
     Z[fc]&=~OV_Z_BIT;
    FM[fc]&=~OV_FM_BIT;
    RW[fc]&=~OV_RW_BIT;
    RR[fc]&=~OV_RR_BIT;
    YV[fc]&=~OV_YV_BIT;
    MK[fc]&=~OV_MK_BIT;
    PP[fc]&=~OV_PP_BIT;
    RTFB&=~OV_RTFB_BIT;
    if (G[fc]){
      if (iVerstrekenGroenTijd[fc]+TE<=MAX_INT) {
        iVerstrekenGroenTijd[fc]+=TE;
      }
      if (SG[fc]) {
        iVerstrekenGroenTijd[fc]=0;
      }
    } else {
      if (iVerstrekenGroenTijd[fc]>0) {
        iGerealiseerdeGroenTijd[fc]+=iVerstrekenGroenTijd[fc];
        iVerstrekenGroenTijd[fc]=-1;
      }
    }
    if (SML && iGerealiseerdeGroenTijd[fc]>0 && !PG[fc]) {
      iGerealiseerdeGroenTijd[fc]=0;
    }
  }
  for (ov=0;
       ov<ovOVMAX;
       ov++) {
    fc=iFC_OVix[ov];
    iRijTimer[ov]=0;
    for (inm=0;
         inm<iAantalInmeldingen[ov];
         inm++) {
      if (iInRijTimer[ov][inm]+TE<=MAX_INT) {
        iInRijTimer[ov][inm]+=TE;
      }
      if (iRijTimer[ov]<iInRijTimer[ov][inm]) {
        iRijTimer[ov]=iInRijTimer[ov][inm];
      }
      if (G[fc]) {
        if (iInGroenBewakingsTimer[ov][inm]+TE<=MAX_INT) {
          iInGroenBewakingsTimer[ov][inm]+=TE;
        }
        if (iInGroenBewakingsTimer[ov][inm]>=iGroenBewakingsTijd[ov]) {
          OVUitmeldenIndex(ov,inm,1,TRUE);
          if (iSelDetFoutNaGB[ov]) {
            iSelDetFout[ov] = TRUE;
          }
          inm--;
        }
      }
    }
  }
}

/* -----------------------------------------------------------------------------
   WachtTijdBewaking bepaalt of de maximumwachttijd is overschreden.
   Het resultaat wordt opgeslagen in de variabele iMaximumWachtTijdOverschreden.
   De maximum wachttijd is overschreden als er een richting fc een aanvraag
   heeft en zijn fasebewakingstijd TFB_timer[fc] heeft de maximum wachttijd
   iMaximumWachtTijd[fc] overschreden.
   ----------------------------------------------------------------------------- */
void WachtTijdBewaking(void) {
  int fc;

  iMaximumWachtTijdOverschreden=0;
  for (fc=0;
       !iMaximumWachtTijdOverschreden&&
       fc<FCMAX;
       fc++) {
    iMaximumWachtTijdOverschreden |= A[fc] && TFB_timer[fc]>=iMaximumWachtTijd[fc];
  }
}

void mag_eerst(void) {
  int ov, fc, i, k;

  for (fc=0; fc<FCMAX; fc++) {
    if (!AAPR[fc] || G[fc]) {
      bMagEerst[fc]=FALSE;
    }
  }

  for (ov=0;ov<ovOVMAX;ov++) {
    fc=iFC_OVix[ov];
    if (iPrioriteit[ov] && G[fc]) {
      for (i=0; i<GKFC_MAX[fc]; i++) {
        k = TO_pointer[fc][i];
        bMagEerst[k]|=AAPR[k];
      }
    }
  }
}

int moet_wachten(int ov) {
  int fc, i, k;
  int imw=0;

  fc=iFC_OVix[ov];
  for (i=0; i<GKFC_MAX[fc]; i++) {
    k = TO_pointer[fc][i];
    imw|= bMagEerst[k];
  }

  return imw!=0;
}

/* ------------------------------------------------------------
   BlokkeringsTijd houdt van iedere OV-richting bij
   hoelang geleden de laatste ingreep heeft plaatsgevonden.
   Het resultaat voor OV-richting ov wordt opgeslagen in de
   variabele iBlokkeringsTimer[ov].
   Tussen twee ingrepen door moeten konflikten de mogelijkheid
   hebben gehad te realiseren. Dit wordt mogelijk gemaakt door
   het bijhouden van de variabele iWachtOpKonflikt[ov], die
   aangeeft of OV-richting ov nog moet wachten op de realisatie
   van een konflikt.
   ------------------------------------------------------------ */
void BlokkeringsTijd(void) {
  int ov,fc;

  if (!iLangstWachtendeAlternatief) {
    mag_eerst();

    for (ov=0;ov<ovOVMAX;ov++) {
      iWachtOpKonflikt[ov]=moet_wachten(ov);
    }
  } else {
    for (ov=0;ov<ovOVMAX;ov++) {
      fc=iFC_OVix[ov];
      if (iPrioriteit[ov] && G[fc]) {
        iBlokkeringsTimer[ov]=0;
        if(iBlokkeringsTijd[ov]>0){
          iWachtOpKonflikt[ov]=1;
        }
      } else {
        if (iBlokkeringsTimer[ov]+TE<=MAX_INT) {
          iBlokkeringsTimer[ov]+=TE;
        }
        if (iWachtOpKonflikt[ov] && (K[fc] || !fka(fc)) && !kaa(fc)) {
          iWachtOpKonflikt[ov]=0;
        }
      }
    }
  }
}

/* --------------------------------------------------------------
   OnderMaximum bepaalt van alle OV-richtingen of het
   ondermaximum is overschreden.
   Het ondermaximum van OV-richting ov is overschreden als de
   resterende maximumgroentijd kleiner is dan zijn
   ondermaximum iOnderMaximum[ov].
   Het resultaat wordt opgeslagen in iOnderMaximumVerstreken[ov].
   -------------------------------------------------------------- */
void OnderMaximum(void) {
  int ov,fc; 
  int iMaxResterendeGroentijd;

  for (ov=0;ov<ovOVMAX;ov++) {
    fc=iFC_OVix[ov];
    iMaxResterendeGroentijd = ((R[fc] || VS[fc]) ? (TFG_max[fc] + 1 + (TVG_max[fc] >= 0 ? TVG_max[fc] : 0) + 1)                 : 0) +
                                        (TFG[fc] ? (TFG_max[fc] + 1 + (TVG_max[fc] >= 0 ? TVG_max[fc] : 0) + 1) - TFG_timer[fc] : 0) +
                                        (WG[fc]  ? (                  (TVG_max[fc] >= 0 ? TVG_max[fc] : 0) + 1)                 : 0) +
                                        (TVG[fc] ? (                  (TVG_max[fc] >= 0 ? TVG_max[fc] : 0) + 1) - TVG_timer[fc] : 0);
    iOnderMaximumVerstreken[ov] =  iOnderMaximum[ov]>=iMaxResterendeGroentijd;
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
void OVInmeldenID(int ov,
                  int iInmelding,
                  int iPN,         /* prioriteitsniveau       */
                  int iPO,         /* prioriteitsopties       */
                  int iRT,         /* rijtimer                */
                  int iGBT,        /* groenbewakingstimer     */
                  int iID) {       /* identificatie inmelding */
  int inm;
  int fc = iFC_OVix[ov];
  if (iInmelding && iAantalInmeldingen[ov]<MAX_AANTAL_INMELDINGEN) {
    inm                                = iAantalInmeldingen[ov];
    iInPrioriteitsNiveau[ov][inm]      = iPN;
    iInPrioriteitsOpties[ov][inm]      = iPO;
    iInRijTimer[ov][inm]               = iRT;
    iInGroenBewakingsTimer[ov][inm]    = iGBT;
    iInID[ov][inm]                     = iID;
    iInOnderMaximumVerstreken[ov][inm] = iOnderMaximumVerstreken[ov];
    if (iPO & poNoodDienst) {
      VLOG_mon5[fc].inmhd = TRUE;
    } else {
       VLOG_mon5[fc].inmov = TRUE;
    }

    (iAantalInmeldingen[ov])++;
  }
}

/* --------------------------------------------------------------
   OVInmelden doet een inmelding als iInmelding waar is, met
   prioriteitsniveau iPN en prioriteitsopties iPO.
   Bij de inmelding wordt tevens opgeslagen of op dat moment
   het ondermaximum was verstreken.
   Daarnaast worden de timers voor de rijtijd en de groenbewaking
   gereset.
   -------------------------------------------------------------- */
void OVInmelden(int ov,
                int iInmelding,
                int iPN,         /* prioriteitsniveau   */
                int iPO,         /* prioriteitsopties   */
                int iRT,         /* rijtimer            */
                int iGBT) {      /* groenbewakingstimer */
  OVInmeldenID(ov,
               iInmelding,
               iPN,         /* prioriteitsniveau   */
               iPO,         /* prioriteitsopties   */
               iRT,         /* rijtimer            */
               iGBT,        /* groenbewakingstimer */
               0);          /* default ID: 0       */
}

/* ------------------------------------------------------
   OVUitmeldenIndex meldt de inmelding met index inm uit.
   ------------------------------------------------------ */
void OVUitmeldenIndex(int ov,
                      int inm,
                      int iUitmelding,
                      bool bGeforceerd) {
  int i;
  int fc = iFC_OVix[ov];
  if (iUitmelding && iAantalInmeldingen[ov]>0) {
    if (iInPrioriteitsOpties[ov][inm] & poNoodDienst) {
      if (bGeforceerd) {
        VLOG_mon5[fc].uitmbewhd = TRUE;
      } else {
        VLOG_mon5[fc].uitmhd = TRUE;
      }
    } else {
      if (bGeforceerd) {
        VLOG_mon5[fc].uitmbewov = TRUE;
      } else {
        VLOG_mon5[fc].uitmov = TRUE;
      }
    }
    for (i=inm;i<iAantalInmeldingen[ov]-1;i++) {
      iInPrioriteitsNiveau[ov][i]      = iInPrioriteitsNiveau[ov][i+1];
      iInPrioriteitsOpties[ov][i]      = iInPrioriteitsOpties[ov][i+1];
      iInRijTimer[ov][i]               = iInRijTimer[ov][i+1];
      iInGroenBewakingsTimer[ov][i]    = iInGroenBewakingsTimer[ov][i+1];
      iInID[ov][i]                     = iInID[ov][i+1];
      iInOnderMaximumVerstreken[ov][i] = iInOnderMaximumVerstreken[ov][i+1];
    }
    (iAantalInmeldingen[ov])--;
    iSelDetFout[ov] = FALSE;
  }
}

/* ---------------------------------------------------------
   OVUitmeldenID meldt de "oudste" inmelding met ID iID uit.
   --------------------------------------------------------- */
void OVUitmeldenID(int ov,
                   int iUitmelding,
                   int iID) {
  int i, inm;
  int fc = iFC_OVix[ov];
  if (iUitmelding) {
    if (iAantalInmeldingen[ov]>0) {
      inm = -1;
      for (i=0;inm == -1 && i<iAantalInmeldingen[ov];i++) {
        if (iInID[ov][i]==iID) {
          inm = i;
        }
      }
      if (inm>=0) {
        OVUitmeldenIndex(ov, inm, iUitmelding, FALSE);
      } else {
        VLOG_mon5[fc].foutuitmov = TRUE;
      }
    } else {
      VLOG_mon5[fc].foutuitmov = TRUE;
    }
  }
}

void OVUitmelden(int ov,
                 int iUitmelding) {
  OVUitmeldenID(ov, iUitmelding, 0);
}

int OVAantalInmeldingenID(int ov,
                          int iID) {
  int i, iReturn;

  for (i=iReturn=0; i<iAantalInmeldingen[ov]; i++) {
    if (iInID[ov][i]==iID) {
      iReturn++;
    }
  }
  return iReturn;
}

/* ---------------------------------------------------------------------------------
   OVRijTijdScenario bepaalt het rijtijdscenario voor een OV-richting (ov)
   op basis de status van de koplus (dkop) en de lange lus (dlang).
   Het rijtijdscenario wordt opgeslagen in de variabele iRijTijdScenario[ov]
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
void OVRijTijdScenario(int ov,
                       int dkop,
                       int dlang,
                       int tbezet) {
  int fc;

  fc=iFC_OVix[ov];
  if (tbezet>=0 && dkop>=0 && dlang>=0) {
    RT[tbezet] = !D[dkop] || !D[dlang];
  }
  if (R[fc]) {
    if (dkop>=0 && dlang>=0) {
      if (tbezet>=0 && !T[tbezet] && !RT[tbezet]) {
        if (iRijTijdScenario[ov]<rtsGehinderd) {
          iRijTijdScenario[ov]=rtsGehinderd;
        }
      } else {
        if (tbezet<0 && D[dkop] && D[dlang]) {
          if (iRijTijdScenario[ov]<rtsGehinderd) {
            iRijTijdScenario[ov]=rtsGehinderd;
          }
        }
        if (D[dkop] || D[dlang]) {
          if (iRijTijdScenario[ov]<rtsBeperktGehinderd) {
            iRijTijdScenario[ov]=rtsBeperktGehinderd;
          }
        }
      }
    } else {
      if (dkop>=0 && D[dkop] ||
          dlang>=0 && D[dlang]) {
        if (iRijTijdScenario[ov]<rtsBeperktGehinderd) {
          iRijTijdScenario[ov]=rtsBeperktGehinderd;
        }
      }
    }
  } else {
    iRijTijdScenario[ov]=rtsOngehinderd;
  }
  switch (iRijTijdScenario[ov]) {
    default:
    case rtsOngehinderd:
      iRijTijd[ov] = iRTSOngehinderd[ov];
    break;
    case rtsBeperktGehinderd:
      iRijTijd[ov] = iRTSBeperktGehinderd[ov];
    break;
    case rtsGehinderd:
      iRijTijd[ov] = iRTSGehinderd[ov];
    break;
  }
}

/* ----------------------------------------------------------
   Met StelInTimer kan de actuele waarde en de instelling van
   een timer worden veranderd. De timer heeft index iIndex,
   krijgt actuele waarde iActueleWaarde en krijgt als
   instelling iInstelling.
   ---------------------------------------------------------- */
void StelInTimer(int iIndex, int iActueleWaarde, int iInstelling) {
  if (iIndex>=0 && iIndex<TM_MAX) {
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
void StelInCounter(int iIndex, int iActueleWaarde, int iInstelling) {
  if (iIndex>=0 && iIndex<CT_MAX) {
    C_counter[iIndex] = iActueleWaarde;
    C_max[iIndex]     = iInstelling;
    C[iIndex]         = iActueleWaarde>0 && iActueleWaarde < iInstelling;
  }
}

/* --------------------------------------------------------
   OVCcolElementen zorgt voor het bijwerken van de volgende
   CCOL-elementen voor het OV:
   - de groenbewakingstimer tgb.
   - de rijtimer trt.
   - het hulpelement voor de prioriteit hprio.
   - de counter voor het aantal OV-inmeldingen cvc.
   - de blokkeringstimer tblk.
   -------------------------------------------------------- */
void OVCcolElementen(int ov, int tgb, int trt, int hprio, int cvc, int tblk) {
  if (ov>=0 && ov<ovOVMAX) {
    if (tgb>=0 && tgb<TM_MAX) {
      T_max[tgb]   = iGroenBewakingsTijd[ov];
      T[tgb]       = iGroenBewakingsTimer[ov] < iGroenBewakingsTijd[ov];
      T_timer[tgb] = T[tgb] ? iGroenBewakingsTimer[ov] : T_max[tgb];
    }
    if (trt>=0 && trt<TM_MAX) {
      T_max[trt]   = iRijTijd[ov];
      T[trt]       = iRijTimer[ov] < iRijTijd[ov];
      T_timer[trt] = T[trt] ? iRijTimer[ov] : T_max[trt];
    }
    if (hprio>=0 && hprio<HE_MAX) {
      IH[hprio] = iPrioriteit[ov];
    }
    if (cvc>=0 && cvc<CT_MAX) {
      C_counter[cvc] = iAantalInmeldingen[ov];
      C[cvc]         = iAantalInmeldingen[ov] > 0;
    }
    if (tblk>=0 && tblk<TM_MAX) {
      T_max[tblk]   = iBlokkeringsTijd[ov];
      T[tblk]       = iBlokkeringsTimer[ov] < iBlokkeringsTijd[ov];
      T_timer[tblk] = T[tblk] ? iBlokkeringsTimer[ov] : T_max[tblk];
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
   ��n van bovenvermelde (bepaling prioriteitsniveau) inmeldingen.
   Van een OV-richting wordt de prioriteit ingetrokken als aan
   minimaal ��n van de volgende voorwaarden is voldaan:
   - de OV-richting is nog niet groen en er is een
     konflikterende OV-richting met een hoger prioriteitsniveau.
   - De OV-richting is groen en er is een
     konflikterende OV-richting met een hoger prioriteitsniveau
     en met de prioriteitsoptie poAfkappenKonflikterendOV.
   --------------------------------------------------------------- */
void PrioriteitsToekenning(void) {
  int ov, inm, i, kov, fc;
  /* Bepaal prioriteitsniveau */
  /* van alle OV-richtingen   */
  for (ov=0;
       ov<ovOVMAX;
       ov++) {
    fc = iFC_OVix[ov];
    iPrioriteitsNiveau[ov]=0;
    iPrioriteitsOpties[ov]=0;
    iAantalPrioriteitsInmeldingen[ov]=0;
    iGroenBewakingsTimer[ov]=iGroenBewakingsTijd[ov];
    for (inm=0;
         inm<iAantalInmeldingen[ov];
         inm++) {
      if (!G[fc]) {
        iInOnderMaximumVerstreken[ov][inm]=0;
      }
      iPrioriteitsOpties[ov]|= iInPrioriteitsOpties[ov][inm] & poAanvraag;
      if (!iSelDetFout[ov] &&
          !iInOnderMaximumVerstreken[ov][inm] ||
          iInPrioriteitsOpties[ov][inm] & poNoodDienst) {
        (iAantalPrioriteitsInmeldingen[ov])++;
        if (iGroenBewakingsTimer[ov]>iInGroenBewakingsTimer[ov][inm]) {
          iGroenBewakingsTimer[ov]=iInGroenBewakingsTimer[ov][inm];
        }
        if (iPrioriteitsNiveau[ov]<iInPrioriteitsNiveau[ov][inm]) {
          iPrioriteitsNiveau[ov] = iInPrioriteitsNiveau[ov][inm];
        }
        iPrioriteitsOpties[ov]|= iInPrioriteitsOpties[ov][inm];
      }
    }
  }
#ifdef OV_ADDFILE
  PrioriteitsOpties_Add();
  PrioriteitsNiveau_Add();
#endif
  /* Trek prioriteiten in */
  for (ov=0;
       ov<ovOVMAX;
       ov++) {
    fc = iFC_OVix[ov];
    if (BL[fc] ||
        iAantalPrioriteitsInmeldingen[ov]==0 ||
        !G[fc] && iMaximumWachtTijdOverschreden &&
        !(iPrioriteitsOpties[ov] & poNoodDienst)) {
      iPrioriteit[ov]=0;
    }
    iKOVPrio[ov] = -1;
    for (i=0;
         i<ovGKFC_MAX[ov];
         i++) {
      kov=ovTO_pointer[ov][i];
      if (iPrioriteitsNiveau[kov] > iPrioriteitsNiveau[ov] && !iXPrio[kov] &&
          (!G[fc] ||
           iPrioriteitsOpties[kov] & poAfkappenKonflikterendOV)) {
        iPrioriteit[ov] = 0;
        iKOVPrio[ov] = kov;
      }
    }
  }
  /* Deel prioriteiten uit */
  for (ov=0;
       ov<ovOVMAX;
       ov++) {
    fc = iFC_OVix[ov];
    if (!BL[fc] &&
        !iXPrio[ov] &&
        iKOVPrio[ov]==-1 &&
        iAantalInmeldingen[ov]>0 &&
        !iPrioriteit[ov] &&
        (iPrioriteitsOpties[ov] & poNoodDienst ||
         !iSelDetFout[ov] &&
         !iMaximumWachtTijdOverschreden &&
         (!G[fc] || !iOnderMaximumVerstreken[ov]) &&
         iBlokkeringsTimer[ov]>=iBlokkeringsTijd[ov] &&
         !iWachtOpKonflikt[ov])) {
      iPrioriteit[ov] = 1;
      for (i=0;
           iPrioriteit[ov] &&
           i<ovGKFC_MAX[ov];
           i++) {
        kov = ovTO_pointer[ov][i];
        if (iPrioriteit[kov]) {
          iPrioriteit[ov] = 0;
        }
      }
    }
  }
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
void AfkapGroen(void) {
  int fc,
      iAfkapGroen2;
  for (fc=0;
       fc<FCMAX;
       fc++) {
    if (iAfkapGroenTijd[fc]<TGG_max[fc]) {
      iAfkapGroenTijd[fc]=TGG_max[fc];
    }
    if (iAfkapGroenTijd[fc]<TFG_max[fc]) {
      iAfkapGroenTijd[fc]=TFG_max[fc];
    }
    iMaxGroen[fc]=TFG_max[fc]+(TVG_max[fc]>=0?TVG_max[fc]:0);
    iAfkapGroen[fc]=iAfkapGroenTijd[fc]<iMaxGroen[fc]?iAfkapGroenTijd[fc]:iMaxGroen[fc];
    iAfkapGroen2=(iPercGroenTijd[fc]+iOphoogPercentageMG[fc])*iMaxGroen[fc]/100L;
    if (iAfkapGroen[fc]<iAfkapGroen2) {
      iAfkapGroen[fc]=iAfkapGroen2;
    }
    iMaxGroenTijdTerugKomen[fc]=iInstPercMaxGroenTijdTerugKomen[fc]*iMaxGroen[fc]/100L;
  }
}

int BepaalRestGroen(int fc, int iPrioriteitsOptiesFC) {
  int iRestGroen;

  if (PR[fc]) {
    if (iPrioriteitsOptiesFC & poNoodDienst) {
      iRestGroen = G[fc] && TGG[fc] ? TGG_max[fc] - TGG_timer[fc] : 0;
    } else {
      if (iPrioriteitsOptiesFC & poAfkappenKonfliktRichtingen &&
          !iNietAfkappen[fc]) {
        iRestGroen = G[fc] && CV[fc] && iAfkapGroen[fc] >= iVerstrekenGroenTijd[fc] ? iAfkapGroen[fc] - iVerstrekenGroenTijd[fc] : 0;
      } else {
        iRestGroen = G[fc] && CV[fc] ? TFG_max[fc] - TFG_timer[fc] + TVG_max[fc] - TVG_timer[fc] : 0;
      }
    }
  } else {
    /* Konflikt k is niet primair gerealiseerd. */
    if (AR[fc] && iInstAfkapGroenAlt[fc] > TGG_max[fc] &&
        !(iPrioriteitsOptiesFC & poNoodDienst)) {
      /* Konflikt k is alternatief gerealiseerd en */
      /* er is geen sprake van een nooddienst.     */
      iRestGroen = G[fc] && iInstAfkapGroenAlt[fc] >= iVerstrekenGroenTijd[fc] ?
                   iInstAfkapGroenAlt[fc] - iVerstrekenGroenTijd[fc] : 0;
    } else {
      iRestGroen = G[fc] && TGG[fc] ? TGG_max[fc] - TGG_timer[fc] : 0;
    }
  }
  return iRestGroen;
}

int StartGroenFC(int fc, int iGewenstStartGroen, int iPrioriteitsOptiesFC) {
  int iStartGroenFC;
  int i, k, kov;
  int iRestGroen,iRestGeel,iRestTO;

  iStartGroenFC = (GL[fc] ? (TGL_max[fc]>0 ? TGL_max[fc] : 1) - TGL_timer[fc] : 0) +
                  (GL[fc] ? TRG_max[fc] : TRG[fc] ? TRG_max[fc] - TRG_timer[fc] : 0);
  if (iStartGroenFC<iGewenstStartGroen) {
    iStartGroenFC = iGewenstStartGroen;
  }

  for (i=0;
       i<GKFC_MAX[fc];
       i++) {
    k = TO_pointer[fc][i];
    if (TO[k][fc]
        #ifdef NALOOPGK
        || TGK[k][fc]
        #endif
       ) {
      iKPrioriteitsOpties[k]|=iPrioriteitsOptiesFC;
      if (PR[k]) {
        if (iPrioriteitsOptiesFC & poNoodDienst) {
          iRestGroen = G[k] && TGG[k] ? TGG_max[k] - TGG_timer[k] : 0;
        } else {
          if (iPrioriteitsOptiesFC & poAfkappenKonfliktRichtingen &&
              !iNietAfkappen[k]) {
            iRestGroen = G[k] && CV[k] && iAfkapGroen[k] >= iVerstrekenGroenTijd[k] ? iAfkapGroen[k] - iVerstrekenGroenTijd[k] : 0;
          } else {
            iRestGroen = G[k] && CV[k] ? TFG_max[k] - TFG_timer[k] + TVG_max[k] - TVG_timer[k] : 0;
          }
        }
        #ifdef NALOOPGK
        if (TNL[k] && iRestGroen < TNL_max[k] - TNL_timer[k])
        {
          iRestGroen = TNL_max[k]-TNL_timer[k];
        }
        #endif
      } else {
        /* Konflikt k is niet primair gerealiseerd. */
        if (AR[k] && iInstAfkapGroenAlt[k] > TGG_max[k] &&
            !(iPrioriteitsOptiesFC & poNoodDienst)) {
          /* Konflikt k is alternatief gerealiseerd en */
          /* er is geen sprake van een nooddienst.     */
          iRestGroen = G[k] && iInstAfkapGroenAlt[k] >= iVerstrekenGroenTijd[k] ?
                       iInstAfkapGroenAlt[k] - iVerstrekenGroenTijd[k] : 0;
        } else {
          iRestGroen = G[k] && TGG[k] ? TGG_max[k] - TGG_timer[k] : 0;
        }
      }
      iRestGeel = G[k] ? (TGL_max[k]>0 ? TGL_max[k] : 1) : GL[k] ? (TGL_max[k]>0 ? TGL_max[k] : 1) - TGL_timer[k] : 0;
      iRestTO = TO_max[k][fc]>=0 ? TO_max[k][fc] - TO_timer[k] :
                #ifdef NALOOPGK
                TGK[k][fc] ? TGK_max[k][fc]-TGK_timer[k] :
                #endif
                0;
      if (TO_max[k][fc]>=0 && iStartGroenFC < iRestGroen + iRestGeel + iRestTO) {
        iStartGroenFC = iRestGroen + iRestGeel + iRestTO;
      }
      if (TO_max[k][fc]<=GK && iStartGroenFC < iRestGroen + iRestTO) {
        iStartGroenFC = iRestGroen + iRestTO;
      }
    }
  }
  for (kov=0;
       kov<ovOVMAX;
       kov++) {
    k = iFC_OVix[kov];
    if (TO[k][fc] && iPrioriteit[kov] && G[k]) {
      iRestGroen = iGroenBewakingsTijd[kov] - iGroenBewakingsTimer[kov];
      iRestGeel = G[k] ? (TGL_max[k]>0 ? TGL_max[k] : 1) : GL[k] ? (TGL_max[k]>0 ? TGL_max[k] : 1) - TGL_timer[k] : 0;
      iRestTO = TO[k][fc] ? TO_max[k][fc] - TO_timer[k] : 0;
      if (TO_max[k][fc]>=0 && iStartGroenFC < iRestGroen + iRestGeel + iRestTO) {
        iStartGroenFC = iRestGroen + iRestGeel + iRestTO;
      }
      if (TO_max[k][fc]==GK && iStartGroenFC < iRestGroen) {
        iStartGroenFC = iRestGroen;
      }
    }
  }
  return iStartGroenFC;
}

/* ---------------------------------------------------------------
   StartGroenMomenten bepaalt van iedere OV-richting ov met
   prioriteit het startgroenmoment.
   Het resultaat wordt opgeslagen in de variabele iStartGroen[ov].
   iStartGroen[ov] is minimaal gelijk aan de resterende rijtijd.
   Als er lopende konflikten zijn die verhinderen dat de
   OV-richting bij het aflopen van de rijtijd groen wordt,
   dan wordt iStartGroen[ov] hierop aangepast.
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
void StartGroenMomenten(void) {
  int ov, fc, iRestRijTijd;

  for (fc=0;
       fc<FC_MAX;
       fc++) {
    iKPrioriteitsOpties[fc]=0;
  }

  for (ov=0;
       ov<ovOVMAX;
       ov++) {
    iStartGroen[ov] = -1;
    if (iAantalInmeldingen[ov]>0 &&
        iPrioriteit[ov]) {
      fc = iFC_OVix[ov];

      iRestRijTijd = iRijTijd[ov] >= iRijTimer[ov] ? iRijTijd[ov] - iRijTimer[ov] : 0;
      iStartGroen[ov] = StartGroenFC(fc, iRestRijTijd, iPrioriteitsOpties[ov]);
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
void OVAanvragen(void) {
  int ov, fc;

  for (ov=0;
       ov<ovOVMAX;
       ov++) {
    fc = iFC_OVix[ov];
    if (iAantalInmeldingen[ov]>0 &&
        (iPrioriteitsOpties[ov] & poAanvraag && /*!iSelDetFout[ov] && */
         (!iSelDetFout[ov] && iRijTimer[ov]>=iRijTijd[ov] ||
          iPrioriteitsOpties[ov] & poNoodDienst))) {
      A[fc]|=OV_A_BIT;
    }
  }
}

void RealisatieTijden(int fc, int iPrioriteitsOptiesFC) {
  int i, k, iGroenTijd;

  for (i=0;
       i<GKFC_MAX[fc];
       i++) {
    k = TO_pointer[fc][i];
    if (!G[k]) {
      if (iPrioriteitsOptiesFC & poNoodDienst) {
        iGroenTijd = TGG_max[k];
      } else {
        if (iPrioriteitsOptiesFC & poAfkappenKonfliktRichtingen) {
          iGroenTijd = iAfkapGroen[k];
        } else {
          iGroenTijd = TFG_max[k] + (TVG_max[k]>0?TVG_max[k]:0);
        }
      }
      if (TO_max[k][fc]==GK) {
        iRealisatieTijd[fc][k] = iKonfliktTijd[k] +
                                 iGroenTijd;
      } else {
        iRealisatieTijd[fc][k] = iKonfliktTijd[k] +
                                 iGroenTijd +
                                 (TGL_max[k]>0 ? TGL_max[k] : 1) +
                                 TO_max[k][fc];
      }
    } else {
      iRealisatieTijd[fc][k] = -1;
    }
  }
}

void TegenHoudenStartGroen(int fc, int iStartGroenFC) {
  int i, k;
  for (i=0;
       i<GKFC_MAX[fc];
       i++) {
    k = TO_pointer[fc][i];
    if (iStartGroenFC<=iRealisatieTijd[fc][k]) {
      RR[k]|=OV_RR_BIT;
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
void OVTegenhouden(void) {
  int ov, fc;

  for (ov=0;
       ov<ovOVMAX;
       ov++) {
    if (iPrioriteit[ov] &&
        iPrioriteitsOpties[ov] & poBijzonderRealiseren) {
      fc=iFC_OVix[ov];
      RealisatieTijden(fc, iPrioriteitsOpties[ov]);
    }
  }
#ifdef OV_ADDFILE
  RealisatieTijden_Add();
#endif
  for (ov=0;
       ov<ovOVMAX;
       ov++) {
    if (iPrioriteit[ov] &&
        iPrioriteitsOpties[ov] & poBijzonderRealiseren) {
      fc=iFC_OVix[ov];
      TegenHoudenStartGroen(fc, iStartGroen[ov]);
      if (iPrioriteitsOpties[ov] & poNoodDienst) {
        RTFB |= OV_RTFB_BIT;
      }
    }
  }
}

void AfkappenStartGroen(int fc, int iStartGr) {
  int i, k;

  for (i=0;
       i<GKFC_MAX[fc];
       i++) {
    k = TO_pointer[fc][i];
    if (G[k] &&
        (TO_max[k][fc]>= 0 && (TGL_max[k]>0 ? TGL_max[k] : 1) + TO_max[k][fc] >= iStartGr||
         #ifdef NALOOPGK
         TO_max[k][fc]==GK && iStartGr<=0 ||
         TO_max[k][fc]==GKL && TGK_max[k][fc] >= iStartGr
         #else
         TO_max[k][fc]<=GK && iStartGr<=0 ||
		 TO_max[k][fc]==GKL	/* PS Gooit voedende richting er direct uit */  
         #endif
        ))
    {
      Z[k]|=OV_Z_BIT;
      if (PR[k] && CV[k]) {
        iAantalMalenNietAfkappen[k] = iInstAantalMalenNietAfkappen[k];
        if (iMaxGroenTijdTerugKomen[k]>iVerstrekenGroenTijd[k]+iGerealiseerdeGroenTijd[k] &&
            iKPrioriteitsOpties[k]&poBijzonderRealiseren) {
          iTerugKomen[k] = 1;
          iTerugKomGroenTijd[k] = iMaxGroen[k] - iGerealiseerdeGroenTijd[k] -
                                  iVerstrekenGroenTijd[k];
        }
        iPercMGOphogen[k] = TRUE;
      }
    }
  }
}

void AfkappenMG(int fc, int iStartGr) {
  int i, k;

  for (i=0;
       i<GKFC_MAX[fc];
       i++) {
    k = TO_pointer[fc][i];
    if (MG[k] &&
        (TO_max[k][fc]>= 0 && (TGL_max[k]>0 ? TGL_max[k] : 1) + TO_max[k][fc] >= iStartGr||
         TO_max[k][fc]==GK && iStartGr<=0) ||TO_max[k][fc]==GKL) {							/* TO_max[k][fc]==GKL toegevoegd */
      Z[k]|=OV_Z_BIT;
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
void OVAfkappen(void) {
  int ov, fc, iTotaalAantalInmeldingen;

  iTotaalAantalInmeldingen = 0;
  for (ov=0;
       ov<ovOVMAX;
       ov++) {
    fc=iFC_OVix[ov];
    iTotaalAantalInmeldingen += !G[fc] ? iAantalInmeldingen[ov] : 0;
    if (iPrioriteit[ov] &&
        iPrioriteitsOpties[ov] & (poAfkappenKonfliktRichtingen|poNoodDienst)) {
      AfkappenStartGroen(fc, iStartGroen[ov]);
    } else {
      if (iPrioriteit[ov] &&
          iPrioriteitsOpties[ov] & poBijzonderRealiseren) {
        AfkappenMG(fc, iStartGroen[ov]);
      }
    }
  }
  for (fc=0;
       fc<FCMAX;
       fc++) {
    if (iMaximumWachtTijdOverschreden &&
        iTotaalAantalInmeldingen>0) {
      /* Versneld rondje                                        */
      /* Afkappen als dat mag en als er een konfliktaanvraag is */
      if (PR[fc] && G[fc] && CV[fc] &&
          iVerstrekenGroenTijd[fc]>=iAfkapGroen[fc] &&
          iTerugKomGroenTijd[fc]==0 &&
          !iNietAfkappen[fc] &&
          ka(fc)) {
        FM[fc]|=OV_FM_BIT;
      }
    }
    if (EG[fc] && !MK[fc] && !iTerugKomen[fc] || R[fc] && !TRG[fc] && !A[fc]) {
      if (iAantalMalenNietAfkappen[fc]) {
        iAantalMalenNietAfkappen[fc]=0;
      }
      if (iNietAfkappen[fc]) {
        iNietAfkappen[fc]=0;
      }
      if (iTerugKomGroenTijd[fc]) {
        iTerugKomGroenTijd[fc]=0;
      }
      if (iTerugKomen[fc]) {
        iTerugKomen[fc]=0;
      }
    }
    /* ------------ */
    /* NietAfkappen */
    /* ------------ */
    if (EG[fc] && iNietAfkappen[fc]) {
      (iAantalMalenNietAfkappen[fc])--;
      iNietAfkappen[fc]=0;
    }
    if (G[fc] && (SG[fc] || SML && PG[fc]) && PR[fc] && iAantalMalenNietAfkappen[fc]>0 && !iNietAfkappen[fc]) {
      iNietAfkappen[fc]=1;
    }
    /* ------------------ */
    /* OphoogPercentageMG */
    /* ------------------ */
    if (EG[fc] &&
        (!MK[fc] || iOphoogPercentageMG[fc]>=100-iPercGroenTijd[fc])) {
      iPercMGOphogen[fc] = FALSE;
      iOphoogPercentageMG[fc]=0;
    }
    if (EG[fc] && iPercMGOphogen[fc]) {
      iOphoogPercentageMG[fc]+=iInstOphoogPercentageMG[fc];
      if (iOphoogPercentageMG[fc]>=100-iPercGroenTijd[fc]) {
        iOphoogPercentageMG[fc]=100-iPercGroenTijd[fc];
      }
      iPercMGOphogen[fc] = FALSE;
    }
    if (R[fc] && !TRG[fc] && !A[fc]) {
      iPercMGOphogen[fc] = FALSE;
      iOphoogPercentageMG[fc] = 0;
    }
    /* ---------- */
    /* TerugKomen */
    /* ---------- */
    if (iTerugKomGroenTijd[fc]>0) {
      if (EG[fc]) {
        if (!iTerugKomen[fc]) {
          iTerugKomGroenTijd[fc]=0;
        } else {
          /* ------------------------------------------------------------ */
          /* Bij terugkomen geen ophoogpercentage van de maximumgroentijd */
          /* ------------------------------------------------------------ */
          iPercMGOphogen[fc] = FALSE;
          iOphoogPercentageMG[fc] = 0;
          if (iTerugKomGroenTijd[fc]<iInstMinTerugKomGroenTijd[fc]) {
            iTerugKomGroenTijd[fc] = iInstMinTerugKomGroenTijd[fc];
          }
        }
      }
      if (SG[fc]) {
        iTerugKomen[fc]=0;
      }
    }
    if (iTerugKomen[fc]) {
      if (G[fc] && !(Z[fc] & OV_Z_BIT) && !(FM[fc] & OV_FM_BIT)) {
        /* Konflikterende prioriteit is ingetrokken
           Er is dus niet langer reden richting fc
           af te kappen.
           Het resterende groen kan alsnog
           in de huidige realisatie worden afgemaakt. */
        iPercMGOphogen[fc] = FALSE;
        iTerugKomen[fc] = FALSE;
        RW[fc] |= OV_RW_BIT;
      }
      if ((PG[fc] || !G[fc]) &&(GL[fc]||TRG[fc]||A[fc])) {
        PP[fc]|=OV_PP_BIT;
        if (PG[fc]) {
          PG[fc]=0;
        }
      } else {
        iTerugKomen[fc]=0;
        iTerugKomGroenTijd[fc]=0;
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
void OVBijzonderRealiseren(void) {
  int ov, fc;

  for (ov=0;
       ov<ovOVMAX;
       ov++) {
    fc=iFC_OVix[ov];
    if (iPrioriteit[ov] &&
        iStartGroen[ov]==0 &&
        iPrioriteitsOpties[ov] & poBijzonderRealiseren) {
      iBijzonderRealiseren[ov]=1;
      /* voorkeuraanvraag openbaar vervoer */
      if (CALW[fc]<PRI_CALW) {
        set_CALW(fc, PRI_CALW);
      }
      /* voorkeurrealisatie openbaar vervoer */
      if (CALW[fc]>=PRI_CALW) {
        set_PRILW(fc, TRUE);
      }
    } else {
      if (iBijzonderRealiseren[ov]) {
        /* voorkeurrealisatie openbaar vervoer resetten
           indien richting nog geen groen heeft gehad,
           maar ingreep niet langer actief is.          */
        if (CALW[fc]>=PRI_CALW) {
          set_CALW(fc, (mulv) (10 * TFB_timer[fc]));
        }
      }
      iBijzonderRealiseren[ov]=0;
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
void OVGroenVasthouden(void) {
  int ov, fc;

  for (ov=0;
       ov<ovOVMAX;
       ov++) {
    if (iPrioriteit[ov] &&
        iPrioriteitsOpties[ov] & poGroenVastHouden) {
      fc = iFC_OVix[ov];
      if (iGroenBewakingsTimer[ov]<iGroenBewakingsTijd[ov]) {
        YV[fc]|=OV_YV_BIT;
      }
    }
  }
}

/* ------------------------------------------------------------
   OVMeetKriterium zorgt voor het opzetten van het
   OV-bitje van de instructievariabele MK[] van de
   OV-richtingen.
   Het meetkriterium wordt opgezet als zich ��n van de volgende
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
void OVMeetKriterium(void) {
  int ov, fc, iRestGroen;

  for (ov=0;
       ov<ovOVMAX;
       ov++) {
    fc = iFC_OVix[ov];
    if (G[fc]) {
      iRestGroen = (TFG_max[fc] - TFG_timer[fc]) +
                   (TVG_max[fc]>=TVG_timer[fc] ? TVG_max[fc] - TVG_timer[fc] : 0);
      if (iPrioriteit[ov] &&
          iPrioriteitsOpties[ov] & poGroenVastHouden &&
          iGroenBewakingsTimer[ov]<iGroenBewakingsTijd[ov] ||
          PR[fc] & PRIMAIR_VERSNELD &&
          (iGroenBewakingsTimer[ov]<iGroenBewakingsTijd[ov] ||
           iAantalInmeldingen[ov]>0 && !ka(fc)) &&
          iGroenBewakingsTijd[ov]-iGroenBewakingsTimer[ov]<=iRestGroen
          ) {
        MK[fc]|=OV_MK_BIT;
      } else {
        if (!(PR[fc]&PRIMAIR_VERSNELD) && !AR[fc]) {
          MK[fc]=0;
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
void OVAlternatieven(void) {
  int fc, ov, iLWAlt=0;

  for (fc=0; fc<FCMAX; fc++) {
    PAR[fc]&=~OV_PAR_BIT;
  }

  for (fc = 0; fc < FCMAX; fc++) {
    if (iSCH_ALTG[fc] && !PAR[fc]) {
      for (ov = 0; ov < ovOVMAX; ov++) {
        if (iT_GBix[ov]>=0 && iT_GBix[ov]<TMMAX &&
            iH_OVix[ov]>=0 && iH_OVix[ov]<HEMAX) {
          PAR[fc] |= ((max_tar_ov(fc, iFC_OVix[ov], iT_GBix[ov], iH_OVix[ov], END) >= iPRM_ALTP[fc]) || 
			          IH[iH_OVix[ov]] && R[iFC_OVix[ov]] && (TO_max[fc][iFC_OVix[ov]]==NG) && (iPrioriteitsOpties[ov] & poBijzonderRealiseren)) ? OV_PAR_BIT : 0;
          iLWAlt|=PAR[fc];
        }
      }
    }
  }
  OVPARCorrecties();
    #ifdef OV_ADDFILE
      OVAlternatieven_Add();
    #endif

  if (iLWAlt) {
    for (fc=0; fc<FCMAX; fc++) { /* Resetten RR Bit 5 als PAR alsnog wordt opgezet door OV */
      if (PAR[fc] && R[fc] && !ERA[fc]) RR[fc] &= ~BIT5;
    }
    langstwachtende_alternatief();
    for (fc=0; fc<FCMAX; fc++) { /* Alternatieve toedeling ook zetten bij alternatieve realisatie */
      if (AR[fc]) PRML[ML][fc] |= ALTERNATIEF;
    }
  }
}

#if !defined(AUTOMAAT) || defined (VISSIM)
int PrioriteitsOpties2PRM(int iPO) {
  int iReturn=0;

  if (iPO & poNoodDienst) {
    iReturn = 5;
  } else {
    if (iPO & poAfkappenKonfliktRichtingen &&
        !(iPO & poAfkappenKonflikterendOV)) {
      iReturn*=10;
      iReturn+=1;
    }
    if (iPO & poGroenVastHouden) {
      iReturn*=10;
      iReturn+=2;
    }
    if (iPO & poBijzonderRealiseren) {
      iReturn*=10;
      iReturn+=3;
    }
    if (iPO & poAfkappenKonflikterendOV) {
      iReturn*=10;
      iReturn+=4;
    }
  }
  return iReturn;
}
#endif

#if !defined(AUTOMAAT) || defined(VISSIM)

/* -------------------------------------------------------------------
   OVDebug toont in het debugscherm (F11) de volgende informatie:
   - gegevens van een OV-richting.
   - gegevens van de inmeldingen van de OV-richting.
   - gegevens van de lopende konflikten van de OV-richting.
   De te tonen OV-richting wordt als volgt bepaald:
   - Heeft geen enkele OV-richting een inmelding, dan wordt hiervoor
     ov genomen.
   - Zijn er wel OV-richtingen met inmeldingen, maar zijn er geen
     prioriteiten toegekend, dan wordt de OV-richting met de kleinste
     index gekozen.
   - Zijn er wel OV-richtingen met prioriteit, dan wordt van die groep
     die met de kleinste index gekozen.
   ------------------------------------------------------------------- */
void OVDebug(int ov) {
  int fc, inm, i, k;
  int y=1;
  int ov2, ov3;
  static int y_max=0;
  #define Y_MAX        65

  if (ov < 0 || iAantalInmeldingen[ov]==0) {
    for (ov2=0,ov3=-1;ov2<ovOVMAX;ov2++) {
      if (iAantalInmeldingen[ov2]>0 &&
          (ov3==-1 || !iPrioriteit[ov3] && iPrioriteit[ov2])) {
        ov3=ov2;
      }
    }
    if (ov3==-1) {
      ov3=ov;
    }
    ov=ov3;
  }

  if (y<Y_MAX) {xyprintf(1,y,"MaxWTOverschreden     =%4d ",iMaximumWachtTijdOverschreden);  y++;}
  if (y<Y_MAX) {xyprintf(1,y,"___________________________");                                y++;}
  if (ov>=0 && ov<ovOVMAX) {
    fc=iFC_OVix[ov];
    if (y<Y_MAX) {xyprintf(1,y,"OV fc%s                       ",FC_code[fc]);                 y++;}
    if (y<Y_MAX) {xyprintf(1,y,"%s         ",
                               iRijTijdScenario[ov]==rtsOngehinderd      ? "Ongehinderd" :
                               iRijTijdScenario[ov]==rtsBeperktGehinderd ? "BeperktGehinderd" :
                               iRijTijdScenario[ov]==rtsGehinderd        ? "Gehinderd" : "?");y++;}
    if (y<Y_MAX) {xyprintf(1,y,"SelDetFout            =%4d ",iSelDetFout[ov]);                y++;}
    if (y<Y_MAX) {xyprintf(1,y,"AantalInmeldingen     =%4d ",iAantalInmeldingen[ov]);         y++;}
    if (y<Y_MAX) {xyprintf(1,y,"StartGroen            =%4d ",iStartGroen[ov]);                y++;}

    if (!iPrioriteit[ov] && iXPrio[ov]) {
      if (y<Y_MAX) {xyprintf(1,y,"Prioriteit            =%4s ","X");                          y++;}
    } else {
      if (y<Y_MAX) {xyprintf(1,y,"Prioriteit            =%4d ",iPrioriteit[ov]);              y++;}
    }

    if (y<Y_MAX) {xyprintf(1,y,"XPrio                 =%4d ",iXPrio[ov]);                     y++;}
    if (y<Y_MAX) {xyprintf(1,y,"OnderMaximumVerstreken=%4d ",iOnderMaximumVerstreken[ov]);    y++;}
    if (y<Y_MAX) {xyprintf(1,y,"BlokkeringsTimer      =%4d ",
                                iBlokkeringsTimer[ov]<iBlokkeringsTijd[ov]?
                                iBlokkeringsTimer[ov] : -1);                                  y++;}
    if (y<Y_MAX) {xyprintf(1,y,"WachtOpKonflikt       =%4d ",iWachtOpKonflikt[ov]);           y++;}
    if (y<Y_MAX) {xyprintf(1,y,"RijTimer              =%4d ",iRijTimer[ov]);                  y++;}
    if (y<Y_MAX) {xyprintf(1,y,"RijTijd               =%4d ",iRijTijd[ov]);                   y++;}
    if (y<Y_MAX) {xyprintf(1,y,"Aanvraag              =%4d ",A[fc]);                          y++;}
    if (y<Y_MAX) {xyprintf(1,y,"PrioriteitsOpties     = %03d ",
                           PrioriteitsOpties2PRM(iPrioriteitsOpties[ov]));                    y++;}
    if (y<Y_MAX) {xyprintf(1,y,"GroenBewakingsTimer   =%4d ",iGroenBewakingsTimer[ov]);       y++;}
    if (y<Y_MAX) {xyprintf(1,y,"GroenBewakingsTijd    =%4d ",iGroenBewakingsTijd[ov]);        y++;}
    if (y<Y_MAX) {xyprintf(1,y,"___________________________");                                y++;}
    for (inm=0;
         inm<iAantalInmeldingen[ov];
         inm++) {
      if (y<Y_MAX) {xyprintf(1,y,"Inmelding %d                  ",inm);                                y++;}
      if (y<Y_MAX) {xyprintf(1,y,"RijTimer              =%4d ",iInRijTimer[ov][inm]);                  y++;}
      if (y<Y_MAX) {xyprintf(1,y,"PrioriteitsOpties     = %03d ",
                             PrioriteitsOpties2PRM(iInPrioriteitsOpties[ov][inm]));                    y++;}
      if (y<Y_MAX) {xyprintf(1,y,"PrioriteitsNiveau     =%4d ",iInPrioriteitsNiveau[ov][inm]);         y++;}
      if (y<Y_MAX) {xyprintf(1,y,"GroenBewakingsTimer   =%4d ",iInGroenBewakingsTimer[ov][inm]);       y++;}
      if (y<Y_MAX) {xyprintf(1,y,"OnderMaximumVerstreken=%4d ",iInOnderMaximumVerstreken[ov][inm]);    y++;}
      if (y<Y_MAX) {xyprintf(1,y,"___________________________");                                       y++;}
    }
    for (i=0;i<GKFC_MAX[fc];i++) {
      k=TO_pointer[fc][i];
      if (TO[k][fc] || iTerugKomen[k] || AAPR[k] || AR[k]) {
        if (y<Y_MAX) {xyprintf(1,y,"Konflikt fc%2s                 ",FC_code[k]);                        y++;}
        if (y<Y_MAX) {xyprintf(1,y,"VerstrekenGroenTijd   =%4d ",iVerstrekenGroenTijd[k]);               y++;}
        if (y<Y_MAX) {xyprintf(1,y,"MaxGroenTijdTerugKomen=%4d ",iMaxGroenTijdTerugKomen[k]);            y++;}
        if (y<Y_MAX) {xyprintf(1,y,"NietAfkappen          =%4d ",iNietAfkappen[k]?
                                                                 iAantalMalenNietAfkappen[k]:
                                                                 iNietAfkappen[k]);                      y++;}
        if (y<Y_MAX) {xyprintf(1,y,"TerugKomen            =%4d ",iTerugKomen[k]);                        y++;}
        if (y<Y_MAX) {xyprintf(1,y,"TerugKomGroenTijd     =%4d ",iTerugKomGroenTijd[k]);                 y++;}
        if (y<Y_MAX) {xyprintf(1,y,"GerealiseerdeGroenTijd=%4d ",iGerealiseerdeGroenTijd[k]);            y++;}
        if (y<Y_MAX) {xyprintf(1,y,"AfkapGroen            =%4d ",iAfkapGroen[k]);                        y++;}
        if (y<Y_MAX) {xyprintf(1,y,"RealisatieTijd        =%4d ",iRealisatieTijd[fc][k]);                y++;}
        if (y<Y_MAX) {xyprintf(1,y,"KonfliktTijd          =%4d ",iKonfliktTijd[k]);                      y++;}
        if (y<Y_MAX) {xyprintf(1,y,"___________________________");                                       y++;}
      }
    }
  }
  for (;y<y_max;y++) {
    xyprintf(1,y,"                            ");
  }
  y_max=y;
}
#endif

void AfhandelingOV(void) {
  static int init=1;

  /* -------------
     Initialisatie
     ------------- */
  if (init) {
    OVInit();
    init=0;
  }

  /* ------------------------------------------------------------
     Vaststellen rijtijdscenarios
     ------------------------------------------------------------ */
  RijTijdScenario();
#ifdef OV_ADDFILE
  RijTijdScenario_Add();
#endif

  /* --------------------
     In - en uitmeldingen
     -------------------- */
  InUitMelden();
#ifdef OV_ADDFILE
  InUitMelden_Add();
#endif

  /* ---------------
     OV-instellingen
     --------------- */
  OVInstellingen();
#ifdef OV_ADDFILE
  OVInstellingen_Add();
#endif

  OVTimers();
  KonfliktTijden();
#ifdef OV_ADDFILE
  KonfliktTijden_Add();
#endif

  WachtTijdBewaking();
#ifdef OV_ADDFILE
  WachtTijdBewaking_Add();
#endif

  OnderMaximum();
#ifdef OV_ADDFILE
  OnderMaximum_Add();
#endif

  BlokkeringsTijd();
#ifdef OV_ADDFILE
  BlokkeringsTijd_Add();
#endif

  PrioriteitsToekenning();
#ifdef OV_ADDFILE
  PrioriteitsToekenning_Add();
#endif

  AfkapGroen();
#ifdef OV_ADDFILE
  AfkapGroen_Add();
#endif

  StartGroenMomenten();
#ifdef OV_ADDFILE
  StartGroenMomenten_Add();
#endif

  /* ------------------------------------------------------
     Als de rijtijd verstreken is, wordt de aanvraag gezet.
     ------------------------------------------------------ */
  OVAanvragen();
#ifdef OV_ADDFILE
  OVAanvragen_Add();
#endif

  /* ------------------------------------------------
     Konflikten worden tegengehouden op basis van het
     StartGroenMoment.
     ------------------------------------------------ */
  OVTegenhouden();
#ifdef OV_ADDFILE
  OVTegenhouden_Add();
#endif

  /* -------------------------------------------
     Konflikten worden afgekapt op basis van het
     StartGroenMoment.
     ------------------------------------------- */
  OVAfkappen();
#ifdef OV_ADDFILE
  OVAfkappen_Add();
#endif

  /* ----------------------------------------------------------
     TVG_max wordt aangepast op basis van de TerugKomGroenTijd.
     ---------------------------------------------------------- */
  TerugKomGroen();

  /* ---------------------------------------------------------
     Bijzonder realiseren als het StartGroenMoment is bereikt.
     --------------------------------------------------------- */
  OVBijzonderRealiseren();

  /* ----------------------------------------------------
     Groen vasthouden tot uitmelding of aanspreken van de
     groenbewaking.
     ---------------------------------------------------- */
  OVGroenVasthouden();

  /* ---------------------------------------------------------
     Meetkriterium van een bijzonder gerealiseerde richting
     afzetten zodat, bij uitmelding de richting naar rood gaat.
     --------------------------------------------------------- */
  OVMeetKriterium();

  /* ------------------------------------------
     Kopieer de waarden naar de Ccol-elementen.
     ------------------------------------------ */
  OVCcol();

  /* -------------------------------------------------------------
     Alternatieve realisaties van niet konflikten tijdens ingreep,
     uitgaande van de resterende groenbewakingstijd.
     ------------------------------------------------------------- */
  OVAlternatieven();

#ifdef OV_ADDFILE
  post_AfhandelingOV();
#endif

#if !defined(AUTOMAAT) || defined(VISSIM)
#ifdef OV_ADDFILE
  OVDebug_Add();
#endif
#endif
}
