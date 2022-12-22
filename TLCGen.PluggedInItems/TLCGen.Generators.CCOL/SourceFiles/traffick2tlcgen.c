/* -------------------------------------------------------------------------------------------------------- */
/* Traffick2TLCGen                                                               Versie 1.0.0 / 01 jan 2023 */
/* -------------------------------------------------------------------------------------------------------- */

/* Deze include file bevat hulp functies voor verkeerskundige Traffick functionaliteiten.                   */
/* Deze functies zijn ontwikkeld en geschreven door Marcel Fick.                                            */
/* Versie: 1.0                                                                                              */
/* Datum:  1 januari 2023                                                                                   */

#define __TRAFFICK2TLCGEN_VAR
#include "traffick2tlcgen.h"

mulv  aantal_hki_kop;                 /* aantal harde koppelingen                                           */
mulv  aantal_vtg_tgo;                 /* aantal voetgangerskoppelingen - type gescheiden oversteek          */
mulv  aantal_lvk_gst;                 /* aantal gelijk starten langzaam verkeer                             */
mulv  aantal_dcf_vst;                 /* aantal deelconflicten voorstart                                    */
mulv  aantal_pel_kop;                 /* aantal peloton koppelingen                                         */
mulv  aantal_fts_pri;                 /* aantal definities fiets voorrang module                            */
mulv  aantal_aft_123;                 /* aantal definities aftellers                                        */

mulv  REALtraffick[FCMAX];            /* REALTIJD voor richtingen die als eerstvolgende aan de beurt zijn   */
mulv  PARtraffick[FCMAX];             /* buffer PAR[] zoals bepaald door Traffick                           */
boolv AAPRprio[FCMAX];                /* AAPR[] voor prioriteitsrealisaties                                 */
mulv  AltRuimte[FCMAX];               /* realisatie ruimte voor alternatieve realisatie                     */
boolv ART[FCMAX];                     /* alternatieve realisatie toegestaan algemene schakelaar             */
mulv  ARB[FCMAX];                     /* alternatieve realisatie toegestaan verfijning per blok             */
boolv MGR[FCMAX];                     /* meeverleng groen                                                   */
boolv MMK[FCMAX];                     /* meeverleng groen alleen als MK[] waar is                           */
boolv BMC[FCMAX];                     /* beeindig meeverleng groen conflicten                               */
boolv WGR[FCMAX];                     /* wachtstand groen                                                   */
boolv FC_DVM[FCMAX];                  /* richting krijgt hogere hiaattijden toebedeeld                      */
mulv  AR_max[FCMAX];                  /* alternatief maximum                                                */
mulv  GWT[FCMAX];                     /* gewogen wachttijd tbv toekennen alternatieve realisatie            */
mulv  TEG[FCMAX];                     /* tijd tot einde groen                                               */
mulv  MTG[FCMAX];                     /* minimale tijd tot groen                                            */
mulv  mmk_old[FCMAX];                 /* buffer MM[mmk] - geheugenelement MK[] bij gescheiden hiaatmeting   */
mulv  MK_old[FCMAX];                  /* buffer MK[]                                                        */
mulv  TMPc[FCMAX][FCMAX];             /* tijdelijke conflict matrix                                         */
mulv  TMPi[FCMAX][FCMAX];             /* restant fictieve ontruimingsijd                                    */

boolv DOSEER[FCMAX];                  /* doseren aktief               (zelf te besturen in REG[]ADD)        */
mulv  MINTSG[FCMAX];                  /* minimale tijd tot startgroen (zelf te besturen in REG[]ADD)        */
mulv  PELTEG[FCMAX];                  /* tijd tot einde groen als peloton ingreep maximaal duurt            */

mulv  TVG_instelling[FCMAX];          /* buffer ingestelde waarde TVG_max[]                                 */
mulv  TGL_instelling[FCMAX];          /* buffer ingestelde waarde TGL_max[]                                 */

mulv  Waft[FCMAX];                    /* aftellerwaarde ( > 0 betekent dat 1-2-3 afteller loopt)            */
mulv  Aled[FCMAX];                    /* aantal resterende leds bij wachttijd voorspeller                   */
mulv  AanDuurLed[FCMAX];              /* tijd dat huidige aantal leds wordt uitgestuurd                     */
mulv  TijdPerLed[FCMAX];              /* tijdsduur per led voor gelijkmatige afloop wachttijd voorspeller   */
mulv  wacht_ML[FCMAX];                /* maximale wachttijd volgens de module molen                         */

mulv  ARM[FCMAX];                     /* kruispunt arm tbv HLPD prioriteit                                  */
mulv  volg_ARM[FCMAX];                /* kruispunt volgarm tbv doorzetten HLPD prioriteit                   */
boolv HD_aanwezig[FCMAX];             /* HLPD aanwezig op richting                                          */
boolv HLPD[FCMAX];                    /* HLPD prioriteit toegekend aan richting (en volgrichtingen)         */
mulv  NAL_HLPD[FCMAX];                /* nalooptijd hulpdienst ingreep op richting(en) in volgarm           */
mulv  verlos_busbaan[FCMAX];          /* buffer voor verlosmelding met prioriteit                           */
boolv iPRIO[FCMAX];                   /* prioriteit toegekend aan richting                                  */

mulv  PEL_UIT_VTG[FCMAX];             /* buffer aantal voertuig voor uitgaande peloton koppeling            */
mulv  PEL_UIT_RES[FCMAX];             /* restant minimale duur uitsturing koppelsignaal peloton koppeling   */

mulv  verklik_srm;                    /* restant duur verklikking SRM bericht                               */
mulv  duur_geen_srm = 0;              /* aantal minuten dat geen SRM bericht is ontvangen (maximum = 32000) */

boolv RAT[FCMAX];                     /* aansturing rateltikker                                             */
boolv KNIP;                           /* hulpwaarde voor knipper signaal                                    */
boolv REGEN;                          /* regensensor aktief (zelf te besturen in REG[]ADD)                  */
boolv WT_TE_HOOG;                     /* wachttijd te hoog voor toekennen prioriteit                        */
boolv GEEN_OV_PRIO;                   /* geen prioriteit OV   (zelf te besturen in REG[]ADD)                */
boolv GEEN_VW_PRIO;                   /* geen prioriteit VW   (zelf te besturen in REG[]ADD)                */
boolv GEEN_FIETS_PRIO;                /* geen fietsprioriteit (zelf te besturen in REG[]ADD)                */

boolv DF[DPMAX];                      /* detectie fout aanwezig                                             */
mulv  D_bez[DPMAX];                   /* tijdsduur detector bezet                                           */
mulv  D_onb[DPMAX];                   /* tijdsduur detector onbezet                                         */
boolv TDH_DVM[DPMAX];                 /* status TDH tijdens DVM                                             */

struct hki_koppeling hki_kop[MAX_HKI_KOP];
struct vtg_koppeling vtg_tgo[MAX_VTG_KOP];
struct lvk_gelijkstr lvk_gst[MAX_LVK_GST];
struct dcf_voorstart dcf_vst[MAX_DCF_VST];
struct pel_koppeling pel_kop[MAX_PEL_KOP];
struct fietsvoorrang fts_pri[MAX_FTS_PRI];
struct prioriteit_id prio_index[FCMAX];
struct afteller      aft_123[FCMAX];

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
char  _UUR[MAXDUMPSTAP];              /* bijhouden UUR tbv flight buffer                                    */
char  _MIN[MAXDUMPSTAP];              /* bijhouden MIN tbv flight buffer                                    */
char  _SEC[MAXDUMPSTAP];              /* bijhouden SEC tbv flight buffer                                    */
mulv  _ML[MAXDUMPSTAP];               /* bijhouden ML  tbv flight buffer                                    */
char  _FC[FCMAX][MAXDUMPSTAP];        /* bijhouden fasecyclus status tbv flight buffer                      */
char  _FCA[FCMAX][MAXDUMPSTAP];       /* bijhouden aanvraag   status tbv flight buffer                      */
boolv _HA[FCMAX];                     /* hulpwaarde A[] tbv start- en einde puls aanvraag in flight buffer  */
mulv dumpstap;                        /* interne teller flight buffer                                       */
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie initialiseer variabelen Traffick2TLCGen                                                          */
/* -------------------------------------------------------------------------------------------------------- */
/* In deze functie worden alle toegevoegde variabelen geinitialiseerd.                                      */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_init_application().                                                */
/*                                                                                                          */
void init_traffick2tlcgen(void)       /* Fik230101                                                          */
{
  count i,j;

  aantal_hki_kop  = 0;                /* aantal harde koppelingen                                           */
  aantal_vtg_tgo  = 0;                /* aantal voetgangerskoppelingen - type gescheiden oversteek          */
  aantal_lvk_gst  = 0;                /* aantal gelijk starten langzaam verkeer                             */
  aantal_dcf_vst  = 0;                /* aantal deelconflicten voorstart                                    */
  aantal_pel_kop  = 0;                /* aantal peloton koppelingen                                         */
  aantal_fts_pri  = 0;                /* aantal definities fiets voorrang module                            */
  aantal_aft_123  = 0;                /* aantal definities aftellers                                        */

  KNIP            = FALSE;            /* hulpwaarde voor knipper signaal                                    */
  REGEN           = FALSE;            /* regensensor aktief (zelf te besturen in REG[]ADD)                  */
  WT_TE_HOOG      = FALSE;            /* wachttijd te hoog voor toekennen prioriteit                        */
  GEEN_OV_PRIO;                       /* geen prioriteit OV   (zelf te besturen in REG[]ADD)                */
  GEEN_VW_PRIO;                       /* geen prioriteit VW   (zelf te besturen in REG[]ADD)                */
  GEEN_FIETS_PRIO = FALSE;            /* geen fietsprioriteit (zelf te besturen in REG[]ADD)                */
  verklik_srm     = 0;                /* restant duur verklikking SRM bericht                               */

  for (i = 0; i < FCMAX; ++i)
  {
    TRG_min_type     |=  RO_type;     /* ondergrens garantie rood  read only                                */
    TGG_type         &= ~RO_type;     /* bovengrens garantie wel instelbaar                                 */
    TGG_min_type     |=  RO_type;     /* ondergrens garantie groen read only                                */

    REALtraffick[i]   = 0;            /* REALTIJD voor richtingen die als eerstvolgende aan de beurt zijn   */
    PARtraffick[i]    = 0;            /* buffer PAR[] zoals bepaald door Traffick                           */
    AAPRprio[i]       = 0;            /* AAPR[] voor prioriteitsrealisaties                                 */
    AltRuimte[i]      = 0;            /* realisatie ruimte voor alternatieve realisatie                     */
    ART[i]            = FALSE;        /* alternatieve realisatie toegestaan algemene schakelaar             */
    ARB[i]            = NG;           /* alternatieve realisatie toegestaan verfijning per blok             */
    MGR[i]            = FALSE;        /* meeverleng groen                                                   */
    MMK[i]            = FALSE;        /* meeverleng groen alleen als MK[] waar is                           */
    BMC[i]            = FALSE;        /* beeindig meeverleng groen conflicten                               */
    WGR[i]            = FALSE;        /* wachtstand groen                                                   */
    FC_DVM[i]         = FALSE;        /* richting krijgt hogere hiaattijden toebedeeld                      */
    AR_max[i]         = TFG_max[i];   /* alternatief maximum                                                */
    GWT[i]            = 0;            /* gewogen wachttijd tbv toekennen alternatieve realisatie            */
    TEG[i]            = NG;           /* tijd tot einde groen                                               */
    MTG[i]            = 0;            /* minimale tijd tot groen                                            */
    mmk_old[i]        = 0;            /* buffer MM[mmk] - geheugenelement MK[] bij gescheiden hiaatmeting   */
    MK_old[i]         = 0;            /* buffer MK[]                                                        */
    wacht_ML[i]       = NG;           /* maximale wachttijd volgens de module molen                         */

    TVG_instelling[i] = 0;            /* buffer ingestelde waarde TVG_max[]                                 */
    TGL_instelling[i] = 0;            /* buffer ingestelde waarde TGL_max[]                                 */

    DOSEER[i]         = FALSE;        /* doseren aktief               (zelf te besturen in REG[]ADD)        */
    MINTSG[i]         = 0;            /* minimale tijd tot startgroen (zelf te besturen in REG[]ADD)        */
    PELTEG[i]         = NG;           /* tijd tot einde groen als peloton ingreep maximaal duurt            */

    ARM[i]            = NG;           /* kruispunt arm tbv HLPD prioriteit                                  */
    volg_ARM[i]       = NG;           /* kruispunt volgarm tbv doorzetten HLPD prioriteit                   */
    HD_aanwezig[i]    = FALSE;        /* HLPD aanwezig op richting                                          */
    HLPD[i]           = FALSE;        /* HLPD prioriteit toegekend aan richting (en volgrichtingen)         */
    NAL_HLPD[i]       = 0;            /* nalooptijd hulpdienst ingreep op richting(en) in volgarm           */
    verlos_busbaan[i] = 0;            /* buffer voor verlosmelding met prioriteit                           */
    iPRIO[i]          = 0;            /* prioriteit toegekend aan richting                                  */

    PEL_UIT_VTG[i]    = 0;            /* buffer aantal voertuig voor uitgaande peloton koppeling obv pulsen */
    PEL_UIT_RES[i]    = 0;            /* restant duur uitsturing koppelsignaal peloton koppeling obv pulsen */

    RAT[i]            = FALSE;        /* aansturing rateltikker                                             */

    for (j = 0; j < FCMAX; ++j)
    {
      TMPc[i][j] = NG;
      TMPi[i][j] = NG;
    }
  }

  for (i = 0; i < DPMAX; ++i)
  {
    DF[i]             = FALSE;        /* detectie fout aanwezig                                             */
    D_bez[i]          = 0;            /* tijdsduur detector bezet                                           */
    D_onb[i]          = 0;            /* tijdsduur detector onbezet                                         */
    TDH_DVM[i]        = FALSE;        /* status TDH tijdens DVM                                             */
  }

  for (i = 0; i < MAX_HKI_KOP; ++i)   /* index buffer harde koppelingen                                     */
  {
    hki_kop[i].fc1      = NG;         /* FC    voedende richting                                            */
    hki_kop[i].fc2      = NG;         /* FC    volg     richting                                            */
    hki_kop[i].tlr21    = NG;         /* TM    late release fc2 (= inrijtijd)                               */
    hki_kop[i].tnlfg12  = NG;         /* TM    vaste   nalooptijd vanaf einde vastgroen      fc1            */
    hki_kop[i].tnlfgd12 = NG;         /* TM    det.afh.nalooptijd vanaf einde vastgroen      fc1            */
    hki_kop[i].tnleg12  = NG;         /* TM    vaste   nalooptijd vanaf einde (verleng)groen fc1            */
    hki_kop[i].tnlegd12 = NG;         /* TM    det.afh.nalooptijd vanaf einde (verleng)groen fc1            */
    hki_kop[i].kop_eg   = FALSE;      /* boolv koppeling vanaf einde groen (als FALSE dan vanaf EV)         */
    hki_kop[i].los_fc2  = FALSE;      /* boolv fc2 mag bij aanvraag fc1 los realiseren                      */
    hki_kop[i].kop_max  = 0;          /* mulv  maximum verlenggroen na harde koppeling                      */
    hki_kop[i].status   = NG;         /* mulv  status koppeling                                             */
  }

  for (i = 0; i < MAX_LVK_GST; ++i)   /* index buffer gelijk starten langzaam verkeer                       */
  {
    lvk_gst[i].fc1      = NG;         /* FC   richting 1                                                    */
    lvk_gst[i].fc2      = NG;         /* FC   richting 2                                                    */
    lvk_gst[i].fc3      = NG;         /* FC   richting 3                                                    */
    lvk_gst[i].fc4      = NG;         /* FC   richting 4                                                    */
  }

  for (i = 0; i < MAX_VTG_KOP; ++i)   /* index buffer voetgangerskoppelingen - type gescheiden oversteek    */
  {
    vtg_tgo[i].fc1      = NG;         /* FC   voedende richting                                             */
    vtg_tgo[i].fc2      = NG;         /* FC   volg     richting                                             */
    vtg_tgo[i].tinl12   = NG;         /* TM   inlooptijd fc1                                                */
    vtg_tgo[i].tinl21   = NG;         /* TM   inlooptijd fc2                                                */
    vtg_tgo[i].tnlsgd12 = NG;         /* TM   nalooptijd fc2 vanaf startgroen fc1                           */
    vtg_tgo[i].tnlsgd21 = NG;         /* TM   nalooptijd fc1 vanaf startgroen fc2                           */
    vtg_tgo[i].hnla12   = NG;         /* HE   drukknop melding koppeling vanaf fc1 aanwezig                 */
    vtg_tgo[i].hnla21   = NG;         /* HE   drukknop melding koppeling vanaf fc2 aanwezig                 */
    vtg_tgo[i].hlos1    = NG;         /* HE   los realiseren fc1 toegestaan                                 */
    vtg_tgo[i].hlos2    = NG;         /* HE   los realiseren fc2 toegestaan                                 */
    vtg_tgo[i].status12 = NG;         /* mulv status koppeling fc1 -> fc2                                   */
    vtg_tgo[i].status21 = NG;         /* mulv status koppeling fc2 -> fc1                                   */
  }

  for (i = 0; i < MAX_DCF_VST; ++i)   /* index buffer deelconflicten voorstart                              */
  {
    dcf_vst[i].fc1      = NG;         /* FC  richting die voorstart geeft                                   */
    dcf_vst[i].fc2      = NG;         /* FC  richting die voorstart krijgt                                  */
    dcf_vst[i].tvs21    = NG;         /* TM  voorstart fc2                                                  */
    dcf_vst[i].to12     = NG;         /* TM  ontruimingstijd van fc1 naar fc2                               */
    dcf_vst[i].ma21     = NG;         /* SCH meerealisatie van fc2 met fc1                                  */
    dcf_vst[i].mv21     = NG;         /* SCH meeverlengen  van fc2 met fc1                                  */
  }

  for (i = 0; i < MAX_PEL_KOP; ++i)   /* index buffer peloton koppelingen                                   */
  {
    pel_kop[i].kop_fc    = NG;        /* FC    koppelrichting                                               */
    pel_kop[i].kop_toe   = NG;        /* ME    toestemming peloton ingreep (bij NG altijd toestemming)      */
    pel_kop[i].kop_sig   = NG;        /* HE    koppelsignaal                                                */
    pel_kop[i].kop_bew   = NG;        /* TM    bewaak koppelsignaal (bij NG wordt een puls veronderstelt)   */
    pel_kop[i].aanv_vert = NG;        /* TM    aanvraag vertraging  (bij NG wordt geen aanvraag opgzet)     */
    pel_kop[i].vast_vert = NG;        /* TM    vasthoud vertraging  (start op binnenkomst koppelsignaal)    */
    pel_kop[i].duur_vast = NG;        /* TM    duur vasthouden (bij duursign. na afvallen koppelsignaal)    */
    pel_kop[i].duur_verl = NG;        /* TM    duur verlengen na ingreep (bij NG geldt TVG_max[])           */
    pel_kop[i].kop_oud   = FALSE;     /* boolv status koppelsignaal vorige machine slag                     */
    pel_kop[i].aanw_kop1 = NG;        /* mulv aanwezigheidsduur koppelsignaal 1 vanaf start puls            */
    pel_kop[i].duur_kop1 = NG;        /* mulv tijdsduur HOOG    koppelsignaal 1 igv duur signaal            */
    pel_kop[i].aanw_kop2 = NG;        /* mulv aanwezigheidsduur koppelsignaal 2 vanaf start puls            */
    pel_kop[i].duur_kop2 = NG;        /* mulv tijdsduur HOOG    koppelsignaal 2 igv duur signaal            */
    pel_kop[i].aanw_kop3 = NG;        /* mulv aanwezigheidsduur koppelsignaal 3 vanaf start puls            */
    pel_kop[i].duur_kop3 = NG;        /* mulv tijdsduur HOOG    koppelsignaal 3 igv duur signaal            */
    pel_kop[i].pk_status = NG;        /* mulv status peloton ingreep                                        */
    pel_kop[i].buffervol = FALSE;     /* mulv buffers voor peloton ingreep vol                              */
  }

  for (i = 0; i < MAX_FTS_PRI; ++i)   /* index buffer fiets voorrang module                                 */
  {
    fts_pri[i].fc       = NG;         /* FC    fietsrichting                                                */
    fts_pri[i].drk1     = NG;         /* DE    drukknop 1 voor aanvraag prioriteit                          */
    fts_pri[i].drk2     = NG;         /* DE    drukknop 2 voor aanvraag prioriteit                          */
    fts_pri[i].de1      = NG;         /* DE    koplus   1 voor aanvraag prioriteit                          */
    fts_pri[i].de2      = NG;         /* DE    koplus   2 voor aanvraag prioriteit                          */
    fts_pri[i].inmeld   = NG;         /* HE    hulp element voor prioriteitsmodule (in.melding prioriteit)  */
    fts_pri[i].uitmeld  = NG;         /* HE    hulp element voor prioriteitsmodule (uitmelding prioriteit)  */
    fts_pri[i].ogwt_fts = NG;         /* TM    ondergrens wachttijd voor prioriteit                         */
    fts_pri[i].prio_fts = NG;         /* PRM   prioriteitscode                                              */
    fts_pri[i].ogwt_reg = NG;         /* TM    ondergrens wachttijd voor prioriteit (indien REGEN == TRUE)  */
    fts_pri[i].prio_reg = NG;         /* PRM   prioriteitscode                      (indien REGEN == TRUE)  */
    fts_pri[i].verklik  = NG;         /* US    verklik fiets prioriteit                                     */
    fts_pri[i].aanvraag = FALSE;      /* boolv fietser is op juiste wijze aangevraagd                       */
    fts_pri[i].prio_vw  = FALSE;      /* boolv fietser voldoet aan prioriteitsvoorwaarden                   */
    fts_pri[i].prio_av  = FALSE;      /* boolv fietser is met prioriteit aangevraagd                        */
  }

  for (i = 0; i < FCMAX; ++i)         /* index buffer prioriteit                                            */
  {
    prio_index[i].HD        = NG;     /* count hulpdienst ingreep                                           */
    prio_index[i].OV_kar    = NG;     /* count OV ingreep - KAR                                             */
    prio_index[i].OV_srm    = NG;     /* count OV ingreep - SRM                                             */
    prio_index[i].OV_verlos = NG;     /* count OV ingreep - verlos                                          */
    prio_index[i].VRW       = NG;     /* count VRW ingreep                                                  */
    prio_index[i].FTS       = NG;     /* count fiets voorrang module                                        */
  }

  for (i = 0; i < FCMAX; ++i)         /* index buffer prioriteit                                            */
  {
    aft_123[i].fc        = NG;        /* FC    richting met afteller                                        */
    aft_123[i].de1       = NG;        /* DE    koplus 1                                                     */
    aft_123[i].de2       = NG;        /* DE    koplus 2                                                     */
    aft_123[i].de3       = NG;        /* DE    koplus 3                                                     */
    aft_123[i].toest     = NG;        /* SCH   toestemming aansturing afteller                              */
    aft_123[i].min_duur  = NG;        /* PRM   minimale duur tot start groen waarbij afteller mag starten   */
    aft_123[i].tel_duur  = NG;        /* PRM   duur van een tel in tienden van seconden                     */
    aft_123[i].us_getal  = NG;        /* US    tbv verklikking op bedienpaneel                              */
    aft_123[i].us_bit0   = NG;        /* US    aansturing afteller BIT0                                     */
    aft_123[i].us_bit1   = NG;        /* US    aansturing afteller BIT1                                     */
    aft_123[i].aftel_ok  = FALSE;     /* boolv alle aftellers van een rijrichting zijn OK                   */
    aft_123[i].act_duur  = FALSE;     /* mulv  tijd in tienden van seconden dat TEL wordt aangestuurd       */
  }

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
  dumpstap = 0;
  for (i = 0; i < MAXDUMPSTAP; ++i)
  {
    _UUR[i] = ' ';
    _MIN[i] = ' ';
    _SEC[i] = ' ';
    _ML[i]  = 0;
  }

  for (i = 0; i < FCMAX; ++i)
  {
    _HA[i] = FALSE;
    for (j = 0; j < MAXDUMPSTAP; ++j)
    {
      _FC[i][j]  = ' ';
      _FCA[i][j] = ' ';
    }
  }
#endif
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer harde koppeling                                                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de gegevens van harde koppelingen in een struct geplaatst.            */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_init_application().                                                */
/*                                                                                                          */
void definitie_harde_koppeling(       /* Fik230101                                                          */
count fc1,                            /* FC    voedende richting                                            */
count fc2,                            /* FC    volg     richting                                            */
count tlr21,                          /* TM    late release fc2 (= inrijtijd)                               */
count tnlfg12,                        /* TM    vaste   nalooptijd vanaf einde vastgroen      fc1            */
count tnlfgd12,                       /* TM    det.afh.nalooptijd vanaf einde vastgroen      fc1            */
count tnleg12,                        /* TM    vaste   nalooptijd vanaf einde (verleng)groen fc1            */
count tnlegd12,                       /* TM    det.afh.nalooptijd vanaf einde (verleng)groen fc1            */
boolv kop_eg,                         /* boolv koppeling vanaf einde groen (als FALSE dan vanaf EV)         */
boolv los_fc2,                        /* boolv fc2 mag bij aanvraag fc1 los realiseren                      */
mulv  kop_max)                        /* mulv  maximum verlenggroen na harde koppeling                      */
{
  if (aantal_hki_kop < MAX_HKI_KOP)
  {
    hki_kop[aantal_hki_kop].fc1      = fc1;
    hki_kop[aantal_hki_kop].fc2      = fc2;
    hki_kop[aantal_hki_kop].tlr21    = tlr21;
    hki_kop[aantal_hki_kop].tnlfg12  = tnlfg12;
    hki_kop[aantal_hki_kop].tnlfgd12 = tnlfgd12;
    hki_kop[aantal_hki_kop].tnleg12  = tnleg12;
    hki_kop[aantal_hki_kop].tnlegd12 = tnlegd12;
    hki_kop[aantal_hki_kop].kop_eg   = kop_eg;
    hki_kop[aantal_hki_kop].los_fc2  = los_fc2;
    hki_kop[aantal_hki_kop].kop_max  = kop_max;
    hki_kop[aantal_hki_kop].status   = 0;
    aantal_hki_kop++;
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer voetgangerskoppeling - type gescheiden oversteek                                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de gegevens van harde koppelingen in een struct geplaatst.            */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_init_application().                                                */
/*                                                                                                          */
void definitie_vtg_gescheiden(        /* Fik230101                                                          */
count fc1,                            /* FC richting 1                                                      */
count fc2,                            /* FC richting 2                                                      */
count tinl12,                         /* TM inlooptijd fc1                                                  */
count tinl21,                         /* TM inlooptijd fc2                                                  */
count tnlsgd12,                       /* TM nalooptijd fc2 vanaf startgroen fc1                             */
count tnlsgd21,                       /* TM nalooptijd fc1 vanaf startgroen fc2                             */
count hnla12,                         /* HE drukknop melding koppeling vanaf fc1 aanwezig                   */
count hnla21,                         /* HE drukknop melding koppeling vanaf fc2 aanwezig                   */
count hlos1,                          /* HE los realiseren fc1 toegestaan                                   */
count hlos2)                          /* HE los realiseren fc2 toegestaan                                   */
{
  if (aantal_vtg_tgo < MAX_VTG_KOP)
  {
    vtg_tgo[aantal_vtg_tgo].fc1      = fc1;
    vtg_tgo[aantal_vtg_tgo].fc2      = fc2;
    vtg_tgo[aantal_vtg_tgo].tinl12   = tinl12;
    vtg_tgo[aantal_vtg_tgo].tinl21   = tinl21;
    vtg_tgo[aantal_vtg_tgo].tnlsgd12 = tnlsgd12;
    vtg_tgo[aantal_vtg_tgo].tnlsgd21 = tnlsgd21;
    vtg_tgo[aantal_vtg_tgo].hnla12   = hnla12;
    vtg_tgo[aantal_vtg_tgo].hnla21   = hnla21;
    vtg_tgo[aantal_vtg_tgo].hlos1    = hlos1;
    vtg_tgo[aantal_vtg_tgo].hlos2    = hlos2;
    vtg_tgo[aantal_vtg_tgo].status12 = 0;
    vtg_tgo[aantal_vtg_tgo].status21 = 0;
    aantal_vtg_tgo++;
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer gelijk start voor langzaam verkeer                                                     */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de gegevens van een gelijk start voor langzaam verkeer in een struct  */
/* geplaatst.                                                                                               */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_init_application().                                                */
/*                                                                                                          */
void definitie_gelijkstart_lvk(       /* Fik230101                                                          */
count fc1,                            /* FC richting 1                                                      */
count fc2,                            /* FC richting 2                                                      */
count fc3,                            /* FC richting 3                                                      */
count fc4)                            /* FC richting 4                                                      */
{
  if (aantal_lvk_gst < MAX_LVK_GST)
  {
    lvk_gst[aantal_lvk_gst].fc1 = fc1;
    lvk_gst[aantal_lvk_gst].fc2 = fc2;
    lvk_gst[aantal_lvk_gst].fc3 = fc3;
    lvk_gst[aantal_lvk_gst].fc4 = fc4;
    aantal_lvk_gst++;
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer deelconflict voorstart                                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de gegevens van deelconflicten (voorstart) in een struct geplaatst    */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_init_application().                                                */
/*                                                                                                          */
void definitie_voorstart_dcf(         /* Fik230101                                                          */
count fc1,                            /* FC  richting die voorstart geeft                                   */
count fc2,                            /* FC  richting die voorstart krijgt                                  */
count tvs21,                          /* TM  voorstart fc2                                                  */
count to12,                           /* TM  ontruimingstijd van fc1 naar fc2                               */
count ma21,                           /* SCH meerealisatie van fc2 met fc1                                  */
count mv21)                           /* SCH meeverlengen  van fc2 met fc1                                  */
{
  if (aantal_dcf_vst < MAX_DCF_VST)
  {
    dcf_vst[aantal_dcf_vst].fc1   = fc1;
    dcf_vst[aantal_dcf_vst].fc2   = fc2;
    dcf_vst[aantal_dcf_vst].tvs21 = tvs21;
    dcf_vst[aantal_dcf_vst].to12  = to12;
    dcf_vst[aantal_dcf_vst].ma21  = ma21;
    dcf_vst[aantal_dcf_vst].mv21  = mv21;
    TMPi[fc1][fc2] = 0;
    aantal_dcf_vst++;
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer peloton koppeling                                                                      */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de gegevens van peloton koppelingen in een struct geplaatst.          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_init_application().                                                */
/*                                                                                                          */
void definitie_peloton_koppeling(     /* Fik230101                                                          */
count kop_fc,                         /* FC  koppelrichting                                                 */
count kop_toe,                        /* ME  toestemming peloton ingreep (bij NG altijd toestemming)        */
count kop_sig,                        /* HE    koppelsignaal                                                */
count kop_bew,                        /* TM  bewaak koppelsignaal (bij NG wordt een puls veronderstelt)     */
count aanv_vert,                      /* TM  aanvraag vertraging  (bij NG wordt geen aanvraag opgzet)       */
count vast_vert,                      /* TM  vasthoud vertraging  (start op binnenkomst koppelsignaal)      */
count duur_vast,                      /* TM  duur vasthouden (bij duursign. na afvallen koppelsignaal)      */
count duur_verl)                      /* TM  duur verlengen na ingreep (bij NG geldt TVG_max[])             */
{
  if (aantal_pel_kop < MAX_PEL_KOP)
  {
    pel_kop[aantal_pel_kop].kop_fc    = kop_fc;
    pel_kop[aantal_pel_kop].kop_toe   = kop_toe;
    pel_kop[aantal_pel_kop].kop_sig   = kop_sig;
    pel_kop[aantal_pel_kop].kop_bew   = kop_bew;
    pel_kop[aantal_pel_kop].aanv_vert = aanv_vert;
    pel_kop[aantal_pel_kop].vast_vert = vast_vert;
    pel_kop[aantal_pel_kop].duur_vast = duur_vast;
    pel_kop[aantal_pel_kop].duur_verl = duur_verl;
    pel_kop[aantal_pel_kop].pk_status = 0;
    pel_kop[aantal_pel_kop].buffervol = FALSE;
    aantal_pel_kop++;
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer fiets voorrang module                                                                  */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de gegevens van de fiets voorrang module in een struct geplaatst.     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_init_application().                                                */
/*                                                                                                          */
void definitie_fiets_voorrang(        /* Fik230101                                                          */
count fc,                             /* FC  fietsrichting                                                  */
count drk1,                           /* DE  drukknop 1 voor aanvraag prioriteit                            */
count drk2,                           /* DE  drukknop 2 voor aanvraag prioriteit                            */
count de1,                            /* DE  koplus   1 voor aanvraag prioriteit                            */
count de2,                            /* DE  koplus   2 voor aanvraag prioriteit                            */
count inmeld,                         /* HE  hulp element voor prioriteitsmodule (in.melding prioriteit)    */
count uitmeld,                        /* HE  hulp element voor prioriteitsmodule (uitmelding prioriteit)    */
count ogwt_fts,                       /* TM  ondergrens wachttijd voor prioriteit                           */
count prio_fts,                       /* PRM prioriteitscode                                                */
count ogwt_reg,                       /* TM  ondergrens wachttijd voor prioriteit (indien REGEN == TRUE)    */
count prio_reg,                       /* PRM prioriteitscode                      (indien REGEN == TRUE)    */
count verklik)                        /* US    verklik fiets prioriteit                                     */
{
  if (aantal_fts_pri < MAX_FTS_PRI)
  {
    fts_pri[aantal_fts_pri].fc       = fc;
    fts_pri[aantal_fts_pri].drk1     = drk1;
    fts_pri[aantal_fts_pri].drk2     = drk2;
    fts_pri[aantal_fts_pri].de1      = de1;
    fts_pri[aantal_fts_pri].de2      = de2;
    fts_pri[aantal_fts_pri].inmeld   = inmeld;
    fts_pri[aantal_fts_pri].uitmeld  = uitmeld;
    fts_pri[aantal_fts_pri].ogwt_fts = ogwt_fts;
    fts_pri[aantal_fts_pri].prio_fts = prio_fts;
    fts_pri[aantal_fts_pri].ogwt_reg = ogwt_reg;
    fts_pri[aantal_fts_pri].prio_reg = prio_reg;
    fts_pri[aantal_fts_pri].verklik  = verklik;
    fts_pri[aantal_fts_pri].aanvraag = FALSE;
    fts_pri[aantal_fts_pri].prio_vw  = FALSE;
    fts_pri[aantal_fts_pri].prio_av  = FALSE;
    aantal_fts_pri++;
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer afteller                                                                               */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de gegevens van aftellers in een struct geplaatst                     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_init_application().                                                */
/*                                                                                                          */
void definitie_afteller(              /* Fik230101                                                          */
count fc,                             /* FC    richting met afteller                                        */
count de1,                            /* DE    koplus 1                                                     */
count de2,                            /* DE    koplus 2                                                     */
count de3,                            /* DE    koplus 3                                                     */
count toest,                          /* SCH   toestemming aansturing afteller                              */
count min_duur,                       /* PRM   minimale duur tot start groen waarbij afteller mag starten   */
count tel_duur,                       /* PRM   duur van een tel in tienden van seconden                     */
count us_getal,                       /* US    tbv verklikking op bedienpaneel                              */
count us_bit0,                        /* US    aansturing afteller BIT0                                     */
count us_bit1)                        /* US    aansturing afteller BIT1                                     */
{
  if (aantal_aft_123 < FCMAX)
  {
    aft_123[aantal_aft_123].fc       = fc;
    aft_123[aantal_aft_123].de1      = de1;
    aft_123[aantal_aft_123].de2      = de2;
    aft_123[aantal_aft_123].de3      = de3;
    aft_123[aantal_aft_123].toest    = toest;
    aft_123[aantal_aft_123].min_duur = min_duur;
    aft_123[aantal_aft_123].tel_duur = tel_duur;
    aft_123[aantal_aft_123].us_getal = us_getal;
    aft_123[aantal_aft_123].us_bit0  = us_bit0;
    aft_123[aantal_aft_123].us_bit1  = us_bit1;
    aft_123[aantal_aft_123].aftel_ok = FALSE;
    aft_123[aantal_aft_123].act_duur = 0;
    aantal_aft_123++;
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bijwerken kruispunt variabelen Traffick2TLCGen                                                   */
/* -------------------------------------------------------------------------------------------------------- */
/* In deze functie worden de kruispunt variabelen bijgewerkt, te weten:                                     */
/* KNIP      : tbv knipperend aansturen leds op het BP                                                      */
/* WT_TE_HOOG: wachttijd te hoog voor prioriteit                                                            */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PreApplication_Add().                                                   */
/*                                                                                                          */
void traffick2tlcgen_kruispunt(void)  /* Fik230101                                                          */
{
  count i;

  mulv max_wt_mvt = NG;
  mulv max_wt_fts = NG;
  mulv max_wt_vtg = NG;
  mulv max_wt_def = 90;               /* default waarde indien instellingen voor voertuigtype ontbreken */

  if (TS) KNIP = !KNIP;               /* knipper signaal tbv leds op bedienpaneel */

#ifdef prmmwta
  max_wt_mvt = PRM[prmmwta];
  if (max_wt_mvt < max_wt_def) max_wt_def = max_wt_mvt;
#endif

#ifdef prmmwtfts
  max_wt_fts = PRM[prmmwtfts];
  if (max_wt_fts < max_wt_def) max_wt_def = max_wt_fts;
#endif

#ifdef prmmwtvtg
  max_wt_vtg = PRM[prmmwtvtg];
  if (max_wt_vtg < max_wt_def) max_wt_def = max_wt_vtg;
#endif

  if (max_wt_mvt == NG) max_wt_mvt = max_wt_def;
  if (max_wt_fts == NG) max_wt_fts = max_wt_def;
  if (max_wt_vtg == NG) max_wt_vtg = max_wt_def;

  WT_TE_HOOG = FALSE;

  for (i = 0; i < FCMAX; ++i)         /* bepaal of wachttijd te hoog is geworden voor toekennen prioriteit */
  {
    if ((US_type[i]&MVT_type) && (TFB_timer[i] >= max_wt_mvt)) WT_TE_HOOG = TRUE;
    if ((US_type[i]&FTS_type) && (TFB_timer[i] >= max_wt_fts)) WT_TE_HOOG = TRUE;
    if ((US_type[i]&VTG_type) && (TFB_timer[i] >= max_wt_vtg)) WT_TE_HOOG = TRUE;
    if ((US_type[i]& OV_type) && (TFB_timer[i] >= max_wt_mvt)) WT_TE_HOOG = TRUE;

    if (WT_TE_HOOG) break;
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bijwerken detectie variabelen Traffick2TLCGen                                                    */
/* -------------------------------------------------------------------------------------------------------- */
/* In deze functie worden de detectie variabelen bijgewerkt, te weten:                                      */
/* DF[]     : detectie fout geconstateerd                                                                   */
/* Dbez[]   : tijdsduur detector bezet                                                                      */
/* Donb[]   : tijdsduur detector onbezet                                                                    */
/* TDH_DVM[]: logische waarde TDH[] bij actief DVM programma                                                */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PreApplication_Add().                                                   */
/*                                                                                                          */
void traffick2tlcgen_detectie(void)   /* Fik230101                                                          */
{
  count i;

  for (i = 0; i < DPMAX; ++i)
  {

#ifndef AUTOMAAT
    if (!D[i]) BG[i] = TBG_timer[i] = FALSE;
    else       OG[i] = TOG_timer[i] = FALSE;
#endif

    DF[i] = (CIF_IS[i] >= CIF_DET_STORING) || OG[i] || BG[i] || FL[i];

    if (!D[i]) D_bez[i]  = 0;
    else       D_bez[i] += TE;

    if ( D[i]) D_onb[i]  = 0;
    else       D_onb[i] += TE;

    if (D_bez[i] > 32000) D_bez[i] = 32000;
    if (D_onb[i] > 32000) D_onb[i] = 32000;

    TDH_DVM[i] = TDH[i];

#ifdef prmtdhdvmkop
    if ((IS_type[i] == DKOP_type) && (TDH_max[i] >= 0))
    {
      if (D_onb[i] < PRM[prmtdhdvmkop]) TDH_DVM[i] = TRUE;
    }
#endif

#ifdef prmtdhdvmlang
    if ((IS_type[i] == DLNG_type) && (TDH_max[i] >= 0))
    {
      if (D_onb[i] < PRM[prmtdhdvmlang]) TDH_DVM[i] = TRUE;
    }
#endif

#ifdef prmtdhdvmver
    if ((IS_type[i] == DVER_type) && (TDH_max[i] >= 0))
    {
      if (D_onb[i] < PRM[prmtdhdvmver]) TDH_DVM[i] = TRUE;
    }
#endif

#ifndef AUTOMAAT
    if (DF[i])
    {
      if (CIF_IS[i] < CIF_DET_STORING)
      {
        if (BG[i]) CIF_IS[i] |= BIT2;
        if (OG[i]) CIF_IS[i] |= BIT3;
        if (FL[i]) CIF_IS[i] |= BIT4;
      }
      if (!FL[i]) CFL_counter[i] = 0;

      D_bez[i] = D_onb[i] = 0;
      D[i] = SD[i] = ED[i] = DB[i] = TDH[i] = TDH_DVM[i] = FALSE;
    }
#endif

  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal start puls snelheidsheidsdetectie                                                         */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt de snelheid die aan de applicatie is doorgegeven op basis van snelheidsdetectie.    */
/* De snelheid wordt precies 1 applicatie ronde aangeboden voor gebruik in andere functies.                 */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PreApplication_Add().                                                   */
/*                                                                                                          */
/* Voor snelheid en classificatie geldt de volgende codering:                                               */
/* xxxx xxxx 1111 1111                voertuigsnelheid (0-255 km/h)                                         */
/* 1xxx xxxx xxxx xxxx                rijrichting: 0 = normaal;     1 = tegenrichting                       */
/* xxx1 xxxx xxxx xxxx                status     : 0 = betrouwbaar; 1 = onbetrouwbaar                       */
/* xxxx x111 xxxx xxxx   000          voertuig   : geen voertuigpassage info                                */
/* xxxx x111 xxxx xxxx   001          voertuig   : personenauto                                             */
/* xxxx x111 xxxx xxxx   010          voertuig   : vrachtwagen                                              */
/* xxxx x111 xxxx xxxx   011          voertuig   : bus                                                      */
/* xxxx x111 xxxx xxxx   100          voertuig   : personenauto + aanhanger                                 */
/* xxxx x111 xxxx xxxx   101          voertuig   : vrachtwagen  + aanhanger                                 */
/* xxxx x111 xxxx xxxx   110          voertuig   : niet gebruikt                                            */
/* xxxx x111 xxxx xxxx   111          voertuig   : ongeldig voertuig                                        */
/*                                                                                                          */
mulv bepaal_start_puls_snelheid(      /* Fik230101                                                          */
count is)                             /* IS ingang signaal waarop snelheid wordt afgebeeld                  */
{
  mulv snelheid = 0;
                                      /* BIT12 = onbetrouwbare meting, BIT15 = voertuig in tegenrichting    */
  if (CIF_IS[is] && (CIF_IS[is] != IS_old[is]) && !(CIF_IS[is]&BIT12) && !(CIF_IS[is]&BIT15))
  {
    if (CIF_IS[is]&BIT0) snelheid += 1;
    if (CIF_IS[is]&BIT1) snelheid += 2;
    if (CIF_IS[is]&BIT2) snelheid += 4;
    if (CIF_IS[is]&BIT3) snelheid += 8;
    if (CIF_IS[is]&BIT4) snelheid += 16;
    if (CIF_IS[is]&BIT5) snelheid += 32;
    if (CIF_IS[is]&BIT6) snelheid += 64;
    if (CIF_IS[is]&BIT7) snelheid += 128;

    return snelheid;
  }

  return NG;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie fasecyclus instellingen Traffick2TLCGen                                                          */
/* -------------------------------------------------------------------------------------------------------- */
/* In deze functie worden fasecyclus instellingen naar arrays gekopieerd, zodat deze instellingen eenvoudig */
/* benaderbaar zijn voor andere functies.                                                                   */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PreApplication_Add().                                                   */
/*                                                                                                          */
void traffick2tlcgen_instel(          /* Fik230101                                                          */
count fc,                             /* FC    fasecyclus                                                   */
mulv  wg,                             /* mulv  wachtstand groen                                             */
mulv  wgav,                           /* mulv  wachtstand groen aanvraag                                    */
mulv  mg,                             /* mulv  meeverleng groen                                             */
mulv  mgmk,                           /* mulv  meeverleng groen alleen als MK[] waar is                     */
mulv  ar,                             /* mulv  alternatieve realisatie toegestaan - algemene schakelaar     */
mulv  arb,                            /* mulv  alternatieve realisatie toegestaan - verfijning per blok     */
mulv  alt_ruimte,                     /* mulv  alternatieve ruimte voor toestemming alternatieve realisatie */
mulv  alt_max,                        /* mulv  alternatief maximum                                          */
mulv  prioOV_index_kar,               /* mulv  OVFCfc - prioriteitsindex OV ingreep - KAR                   */
mulv  prioOV_index_srm,               /* mulv  OVFCfc - prioriteitsindex OV ingreep - SRM                   */
mulv  prioOV_index_verlos,            /* mulv  OVFCfc - prioriteitsindex OV ingreep - verlos                */
mulv  prioHD_index,                   /* mulv  hdFCfc - prioriteitsindex hulpdienst ingreep                 */
boolv HD_counter,                     /* boolv HD_counter (TRUE = hulpdienst aanwezig)                      */
mulv  prioVRW_index,                  /* mulv  VRWFCfc - prioriteitsindex vrachtwagen ingreep               */
mulv  prioFTS_index)                  /* mulv  ftsFCfc - prioriteitsindex fiets voorrang module             */
{
  boolv interface_wijziging = FALSE;  /* bijhouden wijziging variabele op de interface                      */

  if (TGL_max[fc] > 50)
  {
    TGL_max[fc] = 50;                 /* maximum geeltijd bedraagt altijd 5,0 seconden                      */
    interface_wijziging = TRUE;
  }

  if (TGG_max[fc] > TFG_max[fc])      /* vastgroentijd nooit lager dan de garantie groentijd                */
  {
    TFG_max[fc] = TGG_max[fc];
    interface_wijziging = TRUE;
  }

#ifndef NO_TMGLMAX
  if (TMGL_max[fc] != TGL_max[fc])
  {
    TMGL_max[fc] = TGL_max[fc];       /* nooit verlengen geel fase                                          */
    interface_wijziging = TRUE;
  }
#endif

  WGR[fc] = (boolv)wg;
  if (!G[fc] && !wgav) WGR[fc] = FALSE;

  MGR[fc] = (boolv)mg;
  MMK[fc] = (boolv)mgmk;
  ART[fc] = (boolv)ar;
  ARB[fc] = (mulv)arb;

  if (mgmk == NG)                     /* bij NG wordt ervan uitgegaan dat er voor meeverlengen een          */
  {                                   /* ... parameter beschikbaar is                                       */
    if (mg == 1) MMK[fc] = TRUE;      /* MK[] moet waar zijn voor meeverlengen                              */
    else         MMK[fc] = FALSE;
  }

  AR_max[fc] = TGG_max[fc];
  if (TFG_max[fc] > AR_max[fc]) AR_max[fc] = TFG_max[fc];
  if (alt_max     > AR_max[fc]) AR_max[fc] = alt_max;

  if (!G[fc])                         /* alt_ruimte mag wel hoger zijn dan alt_max maar nooit lager         */
  {
    if (alt_ruimte > AR_max[fc]) AR_max[fc] = alt_ruimte;
  }

  prio_index[fc].OV_kar    = prioOV_index_kar;
  prio_index[fc].OV_srm    = prioOV_index_srm;
  prio_index[fc].OV_verlos = prioOV_index_verlos;

  prio_index[fc].HD  = prioHD_index;
  HD_aanwezig[fc]    = HD_counter;
  prio_index[fc].VRW = prioVRW_index;
  prio_index[fc].FTS = prioFTS_index;

  GWT[fc] = TA_timer[fc];             /* voorbereid op gewogen wachttijd meting                             */
  RR[fc] &= ~BIT3;                    /* vroege reset is nodig ivm synchronisatie functies van TLCgen       */

  if (DOSEER[fc])
  {
    WGR[fc] = FALSE;                  /* geen wachtstand   als doseren aktief                               */
    MGR[fc] = FALSE;                  /* geen meeverlengen als doseren aktief                               */
    ART[fc] = FALSE;                  /* geen alternatieve realisatie als doseren aktief                    */
  }

  if (interface_wijziging)            /* variabele(n) gewijzigd op de interface                             */
  {
    CIF_PARM1WIJZAP = -2;
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal aanvraag fiets voorrang module                                                            */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt of de fietsrichting met prioriteit kan worden aangevraagd.                          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Aanvragen_Add().                                                        */
/*                                                                                                          */
void fiets_voorrang_aanvraag(void)    /* Fik230101                                                          */
{
  count i;

  for (i = 0; i < aantal_fts_pri; ++i)
  {
    count fc       = fts_pri[i].fc;       /* fietsrichting */
    count drk1     = fts_pri[i].drk1;     /* drukknop 1 voor aanvraag prioriteit */
    count drk2     = fts_pri[i].drk2;     /* drukknop 2 voor aanvraag prioriteit */
    count de1      = fts_pri[i].de1;      /* koplus   1 voor aanvraag prioriteit */
    count de2      = fts_pri[i].de2;      /* koplus   2 voor aanvraag prioriteit */
    count ogwt_fts = fts_pri[i].ogwt_fts; /* ondergrens wachttijd voor prioriteit */
    count prio_fts = fts_pri[i].prio_fts; /* prioriteitscode */
    count ogwt_reg = fts_pri[i].ogwt_reg; /* ondergrens wachttijd voor prioriteit */
    count prio_reg = fts_pri[i].prio_reg; /* prioriteitscode */
    boolv aanvraag = fts_pri[i].aanvraag; /* fietser is op juiste wijze aangevraagd */
    boolv prio_vw  = fts_pri[i].prio_vw;  /* fietser voldoet aan prioriteitsvoorwaarden */

    if (R[fc] && (A[fc]&BIT0))      /* detectie aanvraag aanwezig */
    {
      if ((drk1 != NG) && D[drk1] && !DF[drk1]) aanvraag = TRUE;
      if ((drk2 != NG) && D[drk2] && !DF[drk2]) aanvraag = TRUE;

      if (!TRG[fc])
      {
        if ((de1 != NG) && DB[de1] && !DF[de1]) aanvraag = TRUE;
        if ((de2 != NG) && DB[de2] && !DF[de2]) aanvraag = TRUE;
      }

      if (aanvraag)
      {
        if ((ogwt_fts != NG) && (!REGEN || (ogwt_reg == NG)))
        {
          if (!BL[fc] && !WT_TE_HOOG && (TA_timer[fc] >= T_max[ogwt_fts])) prio_vw = TRUE;
          if ((prio_fts == NG) || (PRM[prio_fts] == 0)) prio_vw = FALSE;
        }

        if ((ogwt_reg != NG) && (REGEN || (ogwt_fts == NG)))
        {
          if (!BL[fc] && !WT_TE_HOOG && (TA_timer[fc] >= T_max[ogwt_reg])) prio_vw = TRUE;
          if ((prio_reg == NG) || (PRM[prio_reg] == 0)) prio_vw = FALSE;
        }
      }
    }
    else
    {
      aanvraag = FALSE;
      prio_vw  = FALSE;
    }

    if (BL[fc] || (WT_TE_HOOG || GEEN_FIETS_PRIO) && RV[fc] && (Aled[fc] == 0)) prio_vw = FALSE;

    fts_pri[i].aanvraag = aanvraag; /* bijwerken struct */
    fts_pri[i].prio_vw  = prio_vw;
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie werk TempConflict matrix bij                                                                     */
/* -------------------------------------------------------------------------------------------------------- */
/* Richtingen die met elkaar in deelconflict zijn afhankelijk van het actuele lichtbeeld conflicterend met  */
/* elkaar. Deze functie bepaalt alle tijdelijke conflicten en stelt deze beschikbaar via matrix TMPc[][].   */
/* De matrix dient gelezen te worden als TMPc[FCvan][FCnaar].                                               */
/*                                                                                                          */
/*  NG: niet conflicterend                                                                                  */
/* >=0: conflicterend met een intergroentijd                                                                */
/*  FK: fictief conflicterend                                                                               */
/*  GK: conflicterend zonder  intergroentijd                                                                */
/* GKL: conflicterend zonder  intergroentijd waarbij richting "van" een conflicterende volgrichting heeft.  */
/*                                                                                                          */
/* De resterende duur van de fictieve ontruiming is beschikbaar via matrix TMPi[][].                        */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit RealTraffick().                                                         */
/*                                                                                                          */
void TempConflictMatrix(void)         /* Fik230101                                                          */
{
  count i,j,k;
  boolv NEST = TRUE;

  for (i = 0; i < FCMAX; ++i)         /* initialisatie TMPc matrix */
  {
    for (j = 0; j < FCMAX; j++)
    {
      TMPc[i][j] = NG;

      if (!G[i] && !G[j])             /* bijwerken fictieve ontruimingstijden */
      {
        if (TMPi[i][j] > TE) TMPi[i][j] -= TE;
        else                 TMPi[i][j]  = 0;
      }
    }
  }

  while (NEST)                        /* het vaststellen van tijdelijke conflicten is een iteratief proces  */
  {
    NEST = FALSE;
    for (i = 0; i < aantal_dcf_vst; ++i)
    {
      count fc1  = dcf_vst[i].fc1;    /* richting die voorstart geeft  */
      count fc2  = dcf_vst[i].fc2;    /* richting die voorstart krijgt */
      count to12 = dcf_vst[i].to12;   /* ontruimingstijd van fc1 naar fc2 */
      count ma21 = dcf_vst[i].ma21;   /* meerealisatie van fc2 met fc1 */

      if (!G[fc1] && !G[fc2] && (GL[fc2] || TRG[fc2] || A[fc2] || SCH[ma21]))
      {
        for (k = 0; k < FCMAX; ++k)
        {
          if ((GK_conflict(fc2,k) > NG) && (GK_conflict(fc1,k) == NG))
          {
            TMPc[fc1][k] = TMPc[k][fc1] = NEST = TRUE;

            if (volgrichting_hki(fc1)) /* fc1 is een volgrichting, ga op zoek naar de voedende richting(en) */
            {
              for (j = 0; j < aantal_hki_kop; ++j)
              {
                if (hki_kop[j].status > NG) /* koppeling is ingeschakeld */
                {
                  count fc_voedend = hki_kop[j].fc1;
                  count fc_volg    = hki_kop[j].fc2;

                  if (fc1 == fc_volg) /* koppeling gevonden, maak voedende richting ook conflicterend met k */
                  {
                    if (!G[fc_voedend] || !G[k])
                    {
                      TMPc[k][fc_voedend] = GK;
                      TMPc[fc_voedend][k] = GKL;
                    }
                  }
                }
              }
            }
          }
        }
      }

      if (G[fc1] && !G[fc2])
      {
        TMPi[fc1][fc2] = TGL[fc1] + T_max[to12];

        if (TMPc[fc1][fc2] == NG)
        {
          TMPc[fc1][fc2] = TMPc[fc2][fc1] = NEST = TRUE;

          if (volgrichting_hki(fc1))  /* fc1 is een volgrichting, ga op zoek naar de voedende richting(en)  */
          {
            for (j = 0; j < aantal_hki_kop; ++j)
            {
              if (hki_kop[j].status > NG) /* koppeling is ingeschakeld */
              {
                count fc_voedend = hki_kop[j].fc1;
                count fc_volg    = hki_kop[j].fc2;

                if (fc1 == fc_volg)   /* koppeling gevonden, maak voedende richting conflicterend met fc2   */
                {
                  TMPc[fc2][fc_voedend] = GK;
                  TMPc[fc_voedend][fc2] = GKL;
                }
              }
            }
          }
        }
      }
    }
  }

  for (i = 0; i < aantal_vtg_tgo; ++i)
  {
    count fc1 = vtg_tgo[i].fc1;     /* richting 1 */
    count fc2 = vtg_tgo[i].fc2;     /* richting 2 */

    for (j = 0; j < FCMAX; j++)
    {
      if ((TMPc[fc1][j] == NG) && (TMPc[fc2][j] != NG)) TMPc[fc1][j] = TMPc[j][fc1] = FK;
      if ((TMPc[fc2][j] == NG) && (TMPc[fc1][j] != NG)) TMPc[fc2][j] = TMPc[j][fc2] = FK;
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal actuele resterende maximale ontuimingstijd veroorzaakt door deelconflicten            */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als RETURN de resterende maximale ontruimingstijd veroorzaakt door deelconflicten.       */
/*                                                                                                          */
mulv TMP_ontruim(                     /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count i;
  mulv  max = 0;

  for (i = 0; i < FCMAX; ++i)
  {
    if (TMPi[i][fc] > max) max = TMPi[i][fc];
  }
  return max;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal status harde koppeling                                                                    */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaalt de status van de harde koppeling.                                                        */
/*                                                                                                          */
/* NG: uitgeschakeld                                                                                        */
/*  0: niet aktief                                                                                          */
/*  1: voedende richting is groen, volgrichting nog niet                                                    */
/*  2: voedende richting en volgrichting zijn allebei groen                                                 */
/*  3: voeding is niet meer groen, volgrichting groen door naloop tijd(en)                                  */
/*  4: volgrichting verlengt na afloop van harde koppeling                                                  */
/* Let op: Voeding wordt niet als groen beschouwd indien MG en koppeling vanaf EV is gedefinieerd.          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit RealTraffick().                                                         */
/*                                                                                                          */
void StatusHKI(void)                  /* Fik230101                                                          */
{
  count i,j;

  for (i = 0; i < FCMAX; ++i)
  {
    P[i]  &= ~BIT2;                   /* reset P en FM bit naloop harde koppeling */
    FM[i] &= ~BIT2;
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc1      = hki_kop[i].fc1;      /* voedende richting */
      count fc2      = hki_kop[i].fc2;      /* volg     richting */
      count tnlfg12  = hki_kop[i].tnlfg12;  /* vaste   nalooptijd vanaf einde vastgroen      fc1 */
      count tnlfgd12 = hki_kop[i].tnlfgd12; /* det.afh.nalooptijd vanaf einde vastgroen      fc1 */
      count tnleg12  = hki_kop[i].tnleg12;  /* vaste   nalooptijd vanaf einde (verleng)groen fc1 */
      count tnlegd12 = hki_kop[i].tnlegd12; /* det.afh.nalooptijd vanaf einde (verleng)groen fc1 */
      boolv kop_eg   = hki_kop[i].kop_eg;   /* koppeling vanaf einde groen (als FALSE dan vanaf EV) */
      mulv  kop_max  = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */

      boolv HGfc1 = G[fc1] && (!MG[fc1] || kop_eg);

      if ( HGfc1 && !G[fc2]) hki_kop[i].status = 1;
      if ( HGfc1 &&  G[fc2]) hki_kop[i].status = 2;
      if (!HGfc1 &&  G[fc2] && ((hki_kop[i].status == 1) || (hki_kop[i].status == 2))) hki_kop[i].status = 3;

                                            /* garandeer volgrichting */
      if ((hki_kop[i].status == 1) || RA[fc1] && P[fc1]) P[fc2] |= BIT2;

      if (!HGfc1 && (hki_kop[i].status == 3))
      {
        hki_kop[i].status = 0;
        if (SVG[fc2]) hki_kop[i].status = 3;
        if (tnlfg12  != NG) { if (T[tnlfg12]  || ET[tnlfg12] ) hki_kop[i].status = 3; }
        if (tnlfgd12 != NG) { if (T[tnlfgd12] || ET[tnlfgd12]) hki_kop[i].status = 3; }
        if (tnleg12  != NG) { if (T[tnleg12]  || ET[tnleg12] ) hki_kop[i].status = 3; }
        if (tnlegd12 != NG) { if (T[tnlegd12] || ET[tnlegd12]) hki_kop[i].status = 3; }
        if (tnlegd12 != NG) { if ((GL[fc1] || EGL[fc1]) && kop_eg) hki_kop[i].status = 3; }
      }

      if (SVG[fc2] && (hki_kop[i].status == 3)) hki_kop[i].status = 4;
      if (!VG[fc2] && (hki_kop[i].status == 4)) hki_kop[i].status = 0;
      if ( VG[fc2] && (hki_kop[i].status == 4))
      {
        if (TVG_timer[fc2] >= kop_max)
        {
#ifdef PRIO_ADDFILE
          if (!(YV[fc2]&PRIO_YV_BIT)) FM[fc2] |= BIT2;
#else
          FM[fc2] |= BIT2;
#endif
          hki_kop[i].status = 0;
        }
      }

      if (hki_kop[i].status >= 3)     /* controleer of harde koppeling op volgrichting is overgenomen door een andere voedende richting */
      {
        for (j = 0; j < aantal_hki_kop; ++j)
        {
          if ((j != i) && G[hki_kop[j].fc1] && (hki_kop[j].fc2 == fc2))
          {
            hki_kop[i].status = 0;
          }
          if (hki_kop[i].status == 0) break;
        }
      }
    }
  }

  for (i = 0; i < aantal_vtg_tgo; ++i)
  {
    count fc1      = vtg_tgo[i].fc1;      /* richting 1 */
    count fc2      = vtg_tgo[i].fc2;      /* richting 2 */
    count tnlsgd12 = vtg_tgo[i].tnlsgd12; /* nalooptijd fc2 vanaf startgroen fc1 */
    count tnlsgd21 = vtg_tgo[i].tnlsgd21; /* nalooptijd fc1 vanaf startgroen fc2 */
    count hnla12   = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
    count hnla21   = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */

    if ( G[fc1] && !G[fc2] &&  IH[hnla12]                               ) vtg_tgo[i].status12 = 1;
    if ( G[fc1] &&  G[fc2] && (IH[hnla12] || (vtg_tgo[i].status12 == 1))) vtg_tgo[i].status12 = 2;
    if (!G[fc1] &&  G[fc2] && ((vtg_tgo[i].status12 == 1) || (vtg_tgo[i].status12 == 2))) vtg_tgo[i].status12 = 3;

    if (!G[fc1] && (vtg_tgo[i].status12 == 3))
    {
      vtg_tgo[i].status12 = 0;
      if (tnlsgd12 != NG) { if (T[tnlsgd12] || ET[tnlsgd12]) vtg_tgo[i].status12 = 3; }
    }

    if ( G[fc2] && !G[fc1] &&  IH[hnla21]                               ) vtg_tgo[i].status21 = 1;
    if ( G[fc2] &&  G[fc1] && (IH[hnla21] || (vtg_tgo[i].status21 == 1))) vtg_tgo[i].status21 = 2;
    if (!G[fc2] &&  G[fc1] && ((vtg_tgo[i].status21 == 1) || (vtg_tgo[i].status21 == 2))) vtg_tgo[i].status21 = 3;

    if (!G[fc2] && (vtg_tgo[i].status21 == 3))
    {
      vtg_tgo[i].status21 = 0;
      if (tnlsgd21 != NG) { if (T[tnlsgd21] || ET[tnlsgd21]) vtg_tgo[i].status21 = 3; }
    }

    if (vtg_tgo[i].status12 == 1) P[fc2] |= BIT2; /* garandeer volgrichting */
    if (vtg_tgo[i].status21 == 1) P[fc1] |= BIT2; /* garandeer volgrichting */
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal tijd tot einde groen voor prioriteitsrealisaties                                          */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie berekent de tijd tot einde groen van een richting die met prioriteit realiseert. Uitgangspunt    */
/* bij deze berekening is dat de richting eindigt doordat de uitmeldbewaking aanspreekt. Er kan een index   */
/* worden meegegeven worden om een specifieke prioriteit door te rekenen.                                   */
/* (bij index NG worden alle prioriteiten doorgerekend)                                                     */
/*                                                                                                          */
/* Resultaat wordt weggeschreven in TEG[].                                                                  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit BepaalTEG().                                                            */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void BepaalTEG_pri(                   /* Fik230101                                                          */
count volgnr)                         /* index prioriteit                                                   */
{
  count i,prio,inm;
  boolv foutieve_index = FALSE;

  for (i = 0; i < FCMAX; ++i)         /* reset "FC heeft prioriteit" alleen bij aanroep vanuit BepaalTEG[]  */
  {
    if (volgnr == NG) iPRIO[i] = FALSE;
  }

  if ((volgnr != NG) && ((volgnr < 0) || (volgnr >= prioFCMAX))) foutieve_index = TRUE;

  if (!foutieve_index)
  {
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
      if (volgnr != NG) prio = volgnr;

      if ((iStartGroen[prio] > NG) && (iStartGroen[prio] < 9999) && (iPrioriteit[prio] > 0))
      {
        count fc = (count)iFC_PRIOix[prio];
        if (volgnr == NG) iPRIO[fc] = TRUE;       /* aan richting FC is een prioriteitsrealisatie toegekend */
        if (G[fc])
        {
          mulv GroenBewaking = 0;
          for (inm = 0; inm < iAantalInmeldingen[prio]; ++inm)
          {
            if (iInOnderMaximumVerstreken[prio][inm] == 0)
            {
              if (iInGroenBewakingsTimer[prio][inm] > 0)
              {
                if ((mulv)(iGroenBewakingsTijd[prio] - iInGroenBewakingsTimer[prio][inm]) > GroenBewaking)
                {
                  GroenBewaking = (mulv)(iGroenBewakingsTijd[prio] - iInGroenBewakingsTimer[prio][inm]);
                }
              }
              else                    /* inrijtijd is nog niet verstreken */
              {
                if ((mulv)(iRTSOngehinderd[prio] - iInRijTimer[prio][inm] + iGroenBewakingsTijd[prio]) > GroenBewaking)
                {
                  GroenBewaking = (mulv)(iRTSOngehinderd[prio] - iInRijTimer[prio][inm] + iGroenBewakingsTijd[prio]);
                }
              }
            }
          }

          if (GroenBewaking > TEG[fc])
          {
            TEG[fc] = GroenBewaking;
          }
        }
      }
      if (volgnr != NG) break;
    }
  }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal tijd tot einde groen voor harde koppelingen                                               */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie berekent de tijd tot einde groen van een volgrichting als de voedende richting volledig          */
/* uitverlengt. Er kan een index worden meegegeven worden om een specifieke koppeling door te rekenen.      */
/* (bij index NG worden alle koppelingen doorgerekend)                                                      */
/*                                                                                                          */
/* Resultaat wordt weggeschreven in TEG[].                                                                  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit BepaalTEG().                                                            */
/*                                                                                                          */
void BepaalTEG_hki(                   /* Fik230101                                                          */
count volgnr)                         /* index harde koppeling                                              */
{
  count i;
  boolv foutieve_index = FALSE;

  if ((volgnr != NG) && ((volgnr < 0) || (volgnr >= aantal_hki_kop))) foutieve_index = TRUE;

  if (!foutieve_index)
  {
    for (i = 0; i < aantal_hki_kop; ++i)
    {
      if (volgnr != NG) i = volgnr;

      if (hki_kop[i].status > NG)     /* koppeling is ingeschakeld */
      {
        count fc1      = hki_kop[i].fc1;      /* voedende richting */
        count fc2      = hki_kop[i].fc2;      /* volg     richting */
        count tnlfg12  = hki_kop[i].tnlfg12;  /* vaste   nalooptijd vanaf einde vastgroen      fc1 */
        count tnlfgd12 = hki_kop[i].tnlfgd12; /* det.afh.nalooptijd vanaf einde vastgroen      fc1 */
        count tnleg12  = hki_kop[i].tnleg12;  /* vaste   nalooptijd vanaf einde (verleng)groen fc1 */
        count tnlegd12 = hki_kop[i].tnlegd12; /* det.afh.nalooptijd vanaf einde (verleng)groen fc1 */
        boolv kop_eg   = hki_kop[i].kop_eg;   /* koppeling vanaf einde groen (als FALSE dan vanaf EV) */
        mulv  kop_max  = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */
        mulv  status   = hki_kop[i].status;   /* status koppeling */

        mulv  naloop_tijd = 0;

        if (G[fc2] && (status == 2))
        {
          naloop_tijd = 0;
          if (VS[fc1] || FG[fc1])
          {
            if ((tnlfg12  != NG)                                   ) naloop_tijd = T_max[tnlfg12];
            if ((tnlfgd12 != NG) && (naloop_tijd < T_max[tnlfgd12])) naloop_tijd = T_max[tnlfgd12];
          }
          else
          {
            if ((tnlfg12  != NG)                                                       ) naloop_tijd = T_max[tnlfg12]  - T_timer[tnlfg12];
            if ((tnlfgd12 != NG) && (naloop_tijd < T_max[tnlfgd12] - T_timer[tnlfgd12])) naloop_tijd = T_max[tnlfgd12] - T_timer[tnlfgd12];
          }

          if ((tnleg12  != NG) && (naloop_tijd < T_max[tnleg12]                  )) naloop_tijd = T_max[tnleg12];
          if (kop_eg)
          {
            if ((tnlegd12 != NG) && (naloop_tijd < T_max[tnlegd12] + TGL_max[fc1])) naloop_tijd = T_max[tnlegd12] + TGL_max[fc1];
          }
          else
          {
            if ((tnlegd12 != NG) && (naloop_tijd < T_max[tnlegd12]               )) naloop_tijd = T_max[tnlegd12];
          }
          TEG[fc2] = TEG[fc1] + naloop_tijd + kop_max;
        }

        if (G[fc2] && (status == 3))
        {
          if ((tnlfg12  != NG)                                                       ) naloop_tijd = T_max[tnlfg12]  - T_timer[tnlfg12];
          if ((tnlfgd12 != NG) && (naloop_tijd < T_max[tnlfgd12] - T_timer[tnlfgd12])) naloop_tijd = T_max[tnlfgd12] - T_timer[tnlfgd12];
          if ((tnleg12  != NG) && (naloop_tijd < T_max[tnleg12]  - T_timer[tnleg12] )) naloop_tijd = T_max[tnleg12]  - T_timer[tnleg12];

          if (tnlegd12 != NG)
          {
            if (GL[fc1] && kop_eg)
            {
              if (naloop_tijd < TGL_max[fc1] - TGL_timer[fc1] + T_max[tnlegd12]) naloop_tijd = TGL_max[fc1] - TGL_timer[fc1] + T_max[tnlegd12];
            }
            else
            {
              if (naloop_tijd < T_max[tnlegd12] - T_timer[tnlegd12]            ) naloop_tijd = T_max[tnlegd12] - T_timer[tnlegd12];
            }
          }
          TEG[fc2] = naloop_tijd + kop_max;
        }

        if (G[fc2] && (status == 4))
        {
          if (TVG_timer[fc2] >= kop_max) TEG[fc2] = 0;
          else                           TEG[fc2] = kop_max - TVG_timer[fc2];
        }
        if (PELTEG[fc2] > TEG[fc2]) TEG[fc2] = PELTEG[fc2];
      }
      if (volgnr != NG) break;
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal tijd tot einde groen voor voetgangerskoppelingen - type gescheiden oversteek              */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie berekent de tijd tot einde groen van een volgrichting bij bediening van de juiste drukknop. Er   */
/* kan een index worden meegegeven worden om een specifieke koppeling door te rekenen.                      */
/* (bij index NG worden alle koppelingen doorgerekend)                                                      */
/*                                                                                                          */
/* Resultaat wordt weggeschreven in TEG[].                                                                  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit BepaalTEG().                                                            */
/*                                                                                                          */
void BepaalTEG_vtg(                   /* Fik230101                                                          */
count volgnr)                         /* index harde koppeling                                              */
{
  count i;
  boolv foutieve_index = FALSE;

  if ((volgnr != NG) && ((volgnr < 0) || (volgnr >= aantal_vtg_tgo))) foutieve_index = TRUE;

  if (!foutieve_index)
  {
    for (i = 0; i < aantal_vtg_tgo; ++i)
    {
      if (volgnr != NG) i = volgnr;

      /* toegevoegde scope t.b.v. c-versie c89 */
      {
        count fc1      = vtg_tgo[i].fc1;      /* richting 1 */
        count fc2      = vtg_tgo[i].fc2;      /* richting 2 */
        count tnlsgd12 = vtg_tgo[i].tnlsgd12; /* nalooptijd fc2 vanaf startgroen fc1 */
        count tnlsgd21 = vtg_tgo[i].tnlsgd21; /* nalooptijd fc1 vanaf startgroen fc2 */
        count hnla12   = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
        count hnla21   = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */

        if (SG[fc1] && IH[hnla12] && G[fc2] && (TEG[fc2] < T_max[tnlsgd12])) TEG[fc2] = T_max[tnlsgd12];
        if (SG[fc2] && IH[hnla21] && G[fc1] && (TEG[fc1] < T_max[tnlsgd21])) TEG[fc1] = T_max[tnlsgd21];

        if ( G[fc1] && (TEG[fc1] < T_max[tnlsgd21] - T_timer[tnlsgd21])) TEG[fc1] = T_max[tnlsgd21] - T_timer[tnlsgd21];
        if ( G[fc2] && (TEG[fc2] < T_max[tnlsgd12] - T_timer[tnlsgd12])) TEG[fc2] = T_max[tnlsgd12] - T_timer[tnlsgd12];

        if (volgnr != NG) break;
      }
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal tijd tot einde groen                                                                      */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie berekent de tijd tot einde groen als de richting volledig uitverlengt.                           */
/* Resultaat wordt weggeschreven in TEG[]                                                                   */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit RealTraffick().                                                         */
/*                                                                                                          */
void BepaalTEG(void)                  /* Fik230101                                                          */
{
  count i;

  for (i = 0; i < FCMAX; ++i)
  {
    TEG[i] = NG;                      /* initialiseer op "onbekend" */
    if (G[i])
    {
      if (MG[i]) TEG[i] = 0;
      else
      {
        TEG[i] = TFG_max[i] - TFG_timer[i] + TVG_max[i] - TVG_timer[i];
        if (AR[i] && kaapr(i))
        {
          if (AR_max[i] > TG_timer[i]) TEG[i] = AR_max[i] - TG_timer[i];
          else                         TEG[i] = 0;
        }
      }
      if (PELTEG[i] > NG) TEG[i] = PELTEG[i];
    }
  }

#ifdef PRIO_ADDFILE
  BepaalTEG_pri(NG);
#endif
  BepaalTEG_hki(NG);
  BepaalTEG_vtg(NG);

  for (i = 0; i < aantal_dcf_vst; ++i)
  {
    count fc1  = dcf_vst[i].fc1;      /* richting die voorstart geeft  */
    count fc2  = dcf_vst[i].fc2;      /* richting die voorstart krijgt */
    count mv21 = dcf_vst[i].mv21;     /* meeverlengen  van fc2 met fc1 */

    if (G[fc1] && G[fc2] && SCH[mv21])
    {
      if (TEG[fc2] < TEG[fc1]) TEG[fc2] = TEG[fc1];
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal minimale tijd tot realisatie                                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaalt de minimale tijd tot groen voor alle richtingen.                                         */
/* Resultaat wordt weggeschreven in MTG[].                                                                  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit RealTraffick().                                                         */
/*                                                                                                          */
void BepaalMTG(void)                  /* Fik230101                                                          */
{
  count i,j,k;
  boolv door_rekenen = FALSE;         /* bepalen van MTG[] is een iteratief reken proces */
  mulv  aantal_door_reken = 0;        /* voorkom oneindig rekenen */

  for (i = 0; i < FCMAX; ++i)
  {
    mulv max     = 0;
    mulv ontruim = 0;

    BMC[i] = FALSE;                   /* reset BMC[]                                                        */

    if (G[i]) MTG[i] = 0;             /* reset MTG[]                                                        */
    else
    {
      max = 0;                        /* bepaal MTG[]                                                       */

      if  (GL[i]) max = TGL_max[i] - TGL_timer[i] + TRG_max[i];
      if (TRG[i]) max = TRG_max[i] - TRG_timer[i];

      for (j = 0; j < GKFC_MAX[i]; ++j)
      {
        k = KF_pointer[i][j];
        if (G[k])                     /* conflict is groen                                                 */
        {
          if ((TI_max(k,i) >= 0) && (TI_max(k,i) > max)) max = TI_max(k,i);
        }
        else                          /* conflict is niet groen                                            */
        {
          if ((TI_max(k,i) > 0) && TI(k,i))
          {
            ontruim = TI_max(k,i) - TI_timer(k);
            if (ontruim > max) max = ontruim;
          }
        }
      }
      ontruim = TMP_ontruim(i);
      if (ontruim > max) max = ontruim;
      MTG[i] = max;
      if (MINTSG[i] > MTG[i]) MTG[i] = MINTSG[i];
    }
  }

  door_rekenen = TRUE;
  aantal_door_reken = 0;

  while (door_rekenen && (aantal_door_reken < 5))
  {
    door_rekenen = FALSE;
    aantal_door_reken++;

    /* corrigeer MTG[] voor volgrichtingen van harde koppelingen */
    /* dit is nodig om een gedefineerde maximale inrijtijd altijd te kunnen garanderen */
    /* indien een volgrichting een deelconflict voor moet laten gaan met een voorstart */

    for (i = 0; i < aantal_dcf_vst; ++i)
    {
      count fc1   = dcf_vst[i].fc1;   /* richting die voorstart geeft  */
      count fc2   = dcf_vst[i].fc2;   /* richting die voorstart krijgt */
      count tvs21 = dcf_vst[i].tvs21; /* voorstart fc2 */
      count ma21  = dcf_vst[i].tvs21; /* meerealisatie van fc2 met fc1 */

      if (!G[fc1] && volgrichting_hki(fc1)) /* ga na of fc1 een volgrichting van een harde koppeling is */
      {
        if (!G[fc2] && (GL[fc2] || TRG[fc2] || R[fc2] && A[fc2] || SCH[ma21]) && (MTG[fc2] + T_max[tvs21] > MTG[fc1]))
        {
          door_rekenen = TRUE;
          BMC[fc1] = TRUE;
          MTG[fc1] = MTG[fc2] + T_max[tvs21];
        }
        if (G[fc2] && (TG_timer[fc2] < T_max[tvs21]))
        {
          BMC[fc1] = TRUE;
          door_rekenen = TRUE;
          if (T_max[tvs21] - TG_timer[fc2] > MTG[fc1]) MTG[fc1] = T_max[tvs21] - TG_timer[fc2];
        }
      }
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal tijd tot realisatie                                                                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeert de REALTIJD van TLCgen voor richtingen die als eerstvolgende aan de beurt zijn.       */
/* Resultaat wordt weggeschreven in REALtraffick[].                                                         */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit BepaalRealisatieTijden_Add().                                           */
/*                                                                                                          */
void RealTraffick(void)               /* Fik230101                                                          */
{
  count i,k;
  boolv fc_eerst[FCMAX];              /* richting is als eerstvolgende aan de beurt */
  boolv door_rekenen = FALSE;         /* bepalen van REALtraffick[] is een iteratief reken proces */
  mulv  aantal_door_reken = 0;        /* voorkom oneindig rekenen */

  TempConflictMatrix();               /* werk TempConflict matrix bij */
  StatusHKI();                        /* werk status harde koppelingen bij */
  BepaalTEG();                        /* bepaal tijd tot einde groen */
  BepaalMTG();                        /* bepaal minimale tijd tot groen */

  for (i = 0; i < FCMAX; ++i)         /* initialiseer hulp variabelen */
  {
    fc_eerst[i]   = FALSE;
  }

  for (i = 0; i < FCMAX; ++i)         /* bepaal welke richtingen als eerst volgende aan de beurt zijn */
  {
    fc_eerst[i] = R[i] && A[i] && AAPR[i] && (!(RR[i]&BIT6) && !(RR[i]&BIT10) || !conflict_prio_real(i)) || RA[i];
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc1    = hki_kop[i].fc1;    /* voedende richting */
      count fc2    = hki_kop[i].fc2;    /* volg     richting */
      mulv  status = hki_kop[i].status; /* status koppeling */

      if (!G[fc2] && fc_eerst[fc1] || (status == 1)) fc_eerst[fc2] = TRUE;
    }
  }

  for (i = 0; i < aantal_vtg_tgo; ++i)
  {
    count fc1      = vtg_tgo[i].fc1;      /* richting 1 */
    count fc2      = vtg_tgo[i].fc2;      /* richting 2 */
    count hnla12   = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
    count hnla21   = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
    count hlos1    = vtg_tgo[i].hlos1;    /* los realiseren fc1 toegestaan */
    count hlos2    = vtg_tgo[i].hlos2;    /* los realiseren fc2 toegestaan */
    mulv  status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
    mulv  status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

    if (fc_eerst[fc1] && (IH[hnla12] || R[fc2] && A[fc2]) || (status12 == 1))
    {
      if (!G[fc2]) fc_eerst[fc2] = TRUE;
    }
    if (fc_eerst[fc2] && (IH[hnla21] || R[fc1] && A[fc1]) || (status21 == 1))
    {
      if (!G[fc1]) fc_eerst[fc1] = TRUE;
    }
  }

  for (i = 0; i < aantal_lvk_gst; ++i)
  {
    count fc1 = lvk_gst[i].fc1;       /* richting 1 */
    count fc2 = lvk_gst[i].fc2;       /* richting 2 */
    count fc3 = lvk_gst[i].fc3;       /* richting 3 */
    count fc4 = lvk_gst[i].fc4;       /* richting 4 */

    boolv FcEerst = FALSE;
    if ((fc1 != NG) && fc_eerst[fc1]) FcEerst = TRUE;
    if ((fc2 != NG) && fc_eerst[fc2]) FcEerst = TRUE;
    if ((fc3 != NG) && fc_eerst[fc3]) FcEerst = TRUE;
    if ((fc4 != NG) && fc_eerst[fc4]) FcEerst = TRUE;

    if (FcEerst)
    {
      if ((fc1 != NG) && (GL[fc1] || TRG[fc1] || R[fc1] && A[fc1])) fc_eerst[fc1] = TRUE;
      if ((fc2 != NG) && (GL[fc2] || TRG[fc2] || R[fc2] && A[fc2])) fc_eerst[fc2] = TRUE;
      if ((fc3 != NG) && (GL[fc3] || TRG[fc3] || R[fc3] && A[fc3])) fc_eerst[fc3] = TRUE;
      if ((fc4 != NG) && (GL[fc4] || TRG[fc4] || R[fc4] && A[fc4])) fc_eerst[fc4] = TRUE;
    }
  }

  for (i = 0; i < aantal_dcf_vst; ++i)
  {
    count fc1  = dcf_vst[i].fc1;      /* richting die voorstart geeft  */
    count fc2  = dcf_vst[i].fc2;      /* richting die voorstart krijgt */
    count ma21 = dcf_vst[i].ma21;     /* meerealisatie van fc2 met fc1 */

    if (fc_eerst[fc1])
    {
      if (GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || SCH[ma21])) fc_eerst[fc2] = TRUE;
    }
  }

  for (i = 0; i < FCMAX; ++i)
  {
    if (G[i]) REALtraffick[i] = 0;
    else      REALtraffick[i] = NG;

    if (fc_eerst[i])
    {
      REALtraffick[i] = MTG[i];       /* absolute ondergrens gegeven huidige lichtbeeld                     */

      for (k = 0; k < FCMAX; ++k)
      {
        mulv ontruim = GK_conflict(k,i);
        if ((ontruim > NG) && G[k])
        {
          if (TEG[k] + ontruim > REALtraffick[i]) REALtraffick[i] = TEG[k] + ontruim;
        }
      }
    }
  }

  door_rekenen = TRUE;
  aantal_door_reken = 0;

  while (door_rekenen && (aantal_door_reken < 5))
  {
    door_rekenen = FALSE;
    aantal_door_reken++;

    for (i = 0; i < aantal_lvk_gst; ++i)
    {
      count fc1 = lvk_gst[i].fc1;     /* richting 1 */
      count fc2 = lvk_gst[i].fc2;     /* richting 2 */
      count fc3 = lvk_gst[i].fc3;     /* richting 3 */
      count fc4 = lvk_gst[i].fc4;     /* richting 4 */

      mulv max = 0;
      if ((fc1 != NG) && fc_eerst[fc1] && (REALtraffick[fc1] > max)) max = REALtraffick[fc1];
      if ((fc2 != NG) && fc_eerst[fc2] && (REALtraffick[fc2] > max)) max = REALtraffick[fc2];
      if ((fc3 != NG) && fc_eerst[fc3] && (REALtraffick[fc3] > max)) max = REALtraffick[fc3];
      if ((fc4 != NG) && fc_eerst[fc4] && (REALtraffick[fc4] > max)) max = REALtraffick[fc4];

      if ((fc1 != NG) && fc_eerst[fc1]) REALtraffick[fc1] = max;
      if ((fc2 != NG) && fc_eerst[fc2]) REALtraffick[fc2] = max;
      if ((fc3 != NG) && fc_eerst[fc3]) REALtraffick[fc3] = max;
      if ((fc4 != NG) && fc_eerst[fc4]) REALtraffick[fc4] = max;
    }

    for (i = 0; i < aantal_hki_kop; ++i)
    {
      if (hki_kop[i].status > NG)     /* koppeling is ingeschakeld */
      {
        count fc1     = hki_kop[i].fc1;     /* voedende richting */
        count fc2     = hki_kop[i].fc2;     /* volg     richting */
        count tlr21   = hki_kop[i].tlr21;   /* late release fc2 (= inrijtijd fc1) */
        boolv los_fc2 = hki_kop[i].los_fc2; /* fc2 mag bij aanvraag fc1 los realiseren */

        mulv             inrijfc1 = 0;
        if (tlr21 != NG) inrijfc1 = T_max[tlr21];

        if (fc_eerst[fc1])
        {
          if (REALtraffick[fc1] < REALtraffick[fc2] - inrijfc1)
          {
            door_rekenen = TRUE;
            REALtraffick[fc1] = REALtraffick[fc2] - inrijfc1;
          }
        }

        if (fc_eerst[fc1] && fc_eerst[fc2] && (GL[fc2] || R[fc2] && !A[fc2]))
        {
          REALtraffick[fc2] = REALtraffick[fc1] + inrijfc1;
        }

        if (fc_eerst[fc1] && fc_eerst[fc2] && !los_fc2 && (GL[fc1] || TRG[fc1] || R[fc1] && A[fc1]))
        {
          if (REALtraffick[fc2] < REALtraffick[fc1])
          {
            door_rekenen = TRUE;
            REALtraffick[fc2] = REALtraffick[fc1];
          }
        }
      }
    }

    for (i = 0; i < aantal_vtg_tgo; ++i)
    {
      count fc1      = vtg_tgo[i].fc1;      /* richting 1 */
      count fc2      = vtg_tgo[i].fc2;      /* richting 2 */
      count tinl12   = vtg_tgo[i].tinl12;   /* inlooptijd fc1 */
      count tinl21   = vtg_tgo[i].tinl21;   /* inlooptijd fc2 */
      count hnla12   = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
      count hnla21   = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
      count hlos1    = vtg_tgo[i].hlos1;    /* los realiseren fc1 toegestaan */
      count hlos2    = vtg_tgo[i].hlos2;    /* los realiseren fc2 toegestaan */

      mulv  inloop12 = 0;
      mulv  inloop21 = 0;

      if (tinl12 != NG) inloop12 = T_max[tinl12];
      if (tinl21 != NG) inloop21 = T_max[tinl21];

      if (fc_eerst[fc1] && fc_eerst[fc2])
      {
        if (IH[hnla12] && (REALtraffick[fc1] < REALtraffick[fc2] - inloop12))
        {
          door_rekenen = TRUE;
          REALtraffick[fc1] = REALtraffick[fc2] - inloop12;
        }
        if (IH[hnla21] && (REALtraffick[fc2] < REALtraffick[fc1] - inloop21))
        {
          door_rekenen = TRUE;
          REALtraffick[fc2] = REALtraffick[fc1] - inloop21;
        }

        if (!G[fc1] && !G[fc2] && (IH[hnla12] && IH[hnla21] || !IH[hnla12] && !IH[hnla21]))
        {
          if (REALtraffick[fc1] < REALtraffick[fc2])
          {
            door_rekenen = TRUE;
            REALtraffick[fc1] = REALtraffick[fc2];
          }
          if (REALtraffick[fc2] < REALtraffick[fc1])
          {
            door_rekenen = TRUE;
            REALtraffick[fc2] = REALtraffick[fc1];
          }
        }
      }
    }

    for (i = 0; i < aantal_dcf_vst; ++i)
    {
      count fc1   = dcf_vst[i].fc1;   /* richting die voorstart geeft  */
      count fc2   = dcf_vst[i].fc2;   /* richting die voorstart krijgt */
      count tvs21 = dcf_vst[i].tvs21; /* voorstart fc2 */

      if (fc_eerst[fc1] && fc_eerst[fc2])
      {
        if (REALtraffick[fc1] < REALtraffick[fc2] + T_max[tvs21])
        {
          door_rekenen = TRUE;
          REALtraffick[fc1] = REALtraffick[fc2] + T_max[tvs21];
        }
      }
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal duur naloop harde koppelingen                                                             */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCgen gaat bij de berekening van de maximale duur van de naloop uit van de koppeltijden + de ingestelde */
/* TVG_max[] van de naloop richting. Deze tijd kan te hoog zijn omdat de maximale duur van het verlenggroen */
/* na afloop van de koppeltijden lager kan zijn ingesteld dan TVG_max[]. Deze functie corrigeert de TLCgen  */
/* berekening.                                                                                              */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit BepaalAltRuimte().                                                      */
/*                                                                                                          */
#ifdef NALOPEN
void Traffick2TLCgen_NAL(void)        /* Fik230101                                                          */
{
  count i, j;

  for (i = 0; i < FCMAX; ++i)
  {
    TNL_PAR[i] = 0;
    TNL_max[i] = 0;
    TNL[i] = FALSE;
    TNL_timer[i] = 0;

    for (j = 0; j < FCMAX; ++j)
    {
      TGK_max[i][j] = 0;
    }
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc1      = hki_kop[i].fc1;      /* voedende richting */
      count fc2      = hki_kop[i].fc2;      /* volg     richting */
      count tnlfg12  = hki_kop[i].tnlfg12;  /* vaste   nalooptijd vanaf einde vastgroen      fc1 */
      count tnlfgd12 = hki_kop[i].tnlfgd12; /* det.afh.nalooptijd vanaf einde vastgroen      fc1 */
      count tnleg12  = hki_kop[i].tnleg12;  /* vaste   nalooptijd vanaf einde (verleng)groen fc1 */
      count tnlegd12 = hki_kop[i].tnlegd12; /* det.afh.nalooptijd vanaf einde (verleng)groen fc1 */
      boolv kop_eg   = hki_kop[i].kop_eg;   /* koppeling vanaf einde groen (als FALSE dan vanaf EV) */
      mulv  kop_max  = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */

      mulv  TVG_instelling = TVG_max[fc2];  /* buffer TVG_max[] volgrichting */
      mulv  TGL_instelling = TGL_max[fc2];  /* ... en TGL_max[] volgrichting */
      boolv RT_buffer      = FALSE;

      if (TVG_max[fc2] > kop_max) TVG_max[fc2] = kop_max;
      if (TGL_max[fc2] < 1) TGL_max[fc2] = 1;
      if (tnlfg12 != NG) NaloopFG(fc1, fc2, tnlfg12);
      if (tnlfgd12 != NG)
      {
        RT_buffer = RT[tnlfgd12];     /* buffer RT[], die wordt anders gereset in NaloopFGDet() */
        NaloopFGDet(fc1, fc2, tnlfgd12, END);
        RT[tnlfgd12] = RT_buffer;     /* zet juiste waarde RT[] terug */
      }
      if (tnleg12 != NG) NaloopEG(fc1, fc2, tnleg12);
      if (tnlegd12 != NG)
      {
        RT_buffer = RT[tnlegd12];     /* buffer RT[], die wordt anders gereset in NaloopEGDet() */
        NaloopEGDet(fc1, fc2, tnlegd12, END);
        RT[tnlegd12] = RT_buffer;     /* zet juiste waarde RT[] terug */
      }
      if (!kop_eg && (tnleg12 != NG)) NaloopCV(fc1, fc2, tnleg12);
      TVG_max[fc2] = TVG_instelling;  /* zet juiste waarde TVG_max[] en TGL_max[] volgrichting terug */
      TGL_max[fc2] = TGL_instelling;
    }
  }

  for (i = 0; i < aantal_vtg_tgo; ++i)
  {
    count fc1      = vtg_tgo[i].fc1;      /* richting 1 */
    count fc2      = vtg_tgo[i].fc2;      /* richting 2 */
    count tnlsgd12 = vtg_tgo[i].tnlsgd12; /* nalooptijd fc2 vanaf startgroen fc1 */
    count tnlsgd21 = vtg_tgo[i].tnlsgd21; /* nalooptijd fc1 vanaf startgroen fc2 */

    if (tnlsgd12 != NG)
    {
      mulv TVG_instelling = TVG_max[fc2]; /* buffer TVG_max[] volgrichting */
      mulv TGL_instelling = TGL_max[fc2]; /* ... en TGL_max[] volgrichting */

      TVG_max[fc2] = 0;
      if (TGL_max[fc2] < 1) TGL_max[fc2] = 1;

      berekenTNL(fc2, tnlsgd12);
      if (TNL_PAR[fc2] < T_max[tnlsgd12]) TNL_PAR[fc2] = T_max[tnlsgd12];
      TVG_max[fc2] = TVG_instelling;  /* zet juiste waarde TVG_max[] en TGL_max[] volgrichting terug */
      TGL_max[fc2] = TGL_instelling;
    }

    if (tnlsgd21 != NG)
    {
      mulv TVG_instelling = TVG_max[fc1]; /* buffer TVG_max[] volgrichting */
      mulv TGL_instelling = TGL_max[fc1]; /* ... en TGL_max[] volgrichting */

      TVG_max[fc1] = 0;
      if (TGL_max[fc1] < 1) TGL_max[fc1] = 1;

      berekenTNL(fc1, tnlsgd21);
      if (TNL_PAR[fc1] < T_max[tnlsgd21]) TNL_PAR[fc1] = T_max[tnlsgd21];
      TVG_max[fc1] = TVG_instelling;  /* zet juiste waarde TVG_max[] en TGL_max[] volgrichting terug */
      TGL_max[fc1] = TGL_instelling;
    }
  }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verwerk prioriteitsrealisaties in RealTraffick[] en TEG[]                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie verwerkt de informatie van prioriteitsrealisaties die zijn ingepland in RealTraffick[] en TEG[]  */
/* van de conflictrichtingen.                                                                               */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit BepaalAltRuimte().                                                      */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void RealTraffickPrioriteit(void)     /* Fik230101                                                          */
{
  count i,j,k,prio;

  for (i = 0; i < FCMAX; ++i)
  {
    AAPRprio[i] = FALSE;              /* reset AAPRprio[] */
  }

  for (prio = 0; prio < prioFCMAX; ++prio)
  {                                             /* richting heeft of krijgt prioriteit */
    if ((iStartGroen[prio] > NG) && (iStartGroen[prio] < 9999) && (iPrioriteit[prio] > 0))
    {
      count fc = (count)iFC_PRIOix[prio];
      if (!G[fc])
      {
        if ((iPrioriteitsOpties[prio]&poNoodDienst) || (iPrioriteitsOpties[prio]&poBijzonderRealiseren))
        {
          for (k = 0; k < FCMAX; ++k)
          {
            if (GK_conflict(fc,k) > NG)         /* corrigeer REALtraffick voor conflicten die nog rood zijn */
            {
              if (RV[k] || RA[k] && !P[k]) REALtraffick[k] = NG;
            }
          }

          if ((REALtraffick[fc] == NG) || (REALtraffick[fc] >= (mulv)iStartGroen[prio]))
          {
                                                  /* AAPRprio[] beindigt (als nodig) MG[] van de conflicten */
            if (REALtraffick[fc] == NG) AAPRprio[fc] = TRUE;

            REALtraffick[fc] = (mulv)iStartGroen[prio];
            for (j = 0; j < GKFC_MAX[fc]; ++j)
            {
              k = KF_pointer[fc][j];
              if (G[k])                                   /* corrigeer TEG[] voor conflicten die groen zijn */
              {
                if (TI_max(k,fc) >= 0)
                {
                  if (TEG[k] > REALtraffick[fc] - TI_max(k,fc))
                  {
                    TEG[k] = REALtraffick[fc] - TI_max(k,fc);
                    if (TEG[k] < 0) TEG[k] = 0;
                  }
                }
                if (TI_max(k,fc) == GK)
                {
                  if (TEG[k] > REALtraffick[fc])
                  {
                    TEG[k] = REALtraffick[fc];
                    if (TEG[k] < 0) TEG[k] = 0;
                  }
                }
#ifdef NALOPEN
                if ((TI_max(k,fc) == GKL) && TGK[k][fc])
                {
                  if (TEG[k] > REALtraffick[fc] - TGK_max[k][fc])
                  {
                    TEG[k] = REALtraffick[fc] - TGK_max[k][fc];
                    if (TEG[k] < 0) TEG[k] = 0;
                  }
                }
#endif
              }
            }
            /* for (alle tijdelijke conflicten)
            {
              ToDo -> ook iStartGroen[prio] dient gecorrigeerd te worden in geval een volgrichting deelconflicten heeft
            } */
          }
        }
      }
    }
  }

  for (i = 0; i < aantal_dcf_vst; ++i)
  {
    count fc1   = dcf_vst[i].fc1;     /* richting die voorstart geeft  */
    count fc2   = dcf_vst[i].fc2;     /* richting die voorstart krijgt */
    count tvs21 = dcf_vst[i].tvs21;   /* voorstart fc2 */
    count ma21  = dcf_vst[i].ma21;    /* meerealisatie van fc2 met fc1 */
    count mv21  = dcf_vst[i].mv21;    /* meeverlengen  van fc2 met fc1 */

    if ((REALtraffick[fc1] > NG) && R[fc1] && A[fc1] && (GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || SCH[ma21])))
    {
      if (REALtraffick[fc1] < MTG[fc2] + T_max[tvs21])
      {
        REALtraffick[fc1] = MTG[fc2] + T_max[tvs21];
      }
    }

    if (G[fc1] && G[fc2] && SCH[mv21])
    {
      if (TEG[fc2] < TEG[fc1]) TEG[fc2] = TEG[fc1];
    }

  }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal realisatie ruimte voor alternatieve realisatie                                            */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaalt de alternatieve realisatie ruimte bepaalt voor alle richtingen.                          */
/* Resultaat wordt weggeschreven in AltRuimte[].                                                            */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Maxgroen_Add().                                                         */
/*                                                                                                          */
void BepaalAltRuimte(void)            /* Fik230101                                                          */
{
  count i,k;

#ifdef NALOPEN
  Traffick2TLCgen_NAL();              /* corrigeer de nalooptijden */
#endif

#ifdef PRIO_ADDFILE
  RealTraffickPrioriteit();           /* corrigeer RealTraffick[] en TEG[] a.g.v. prioriteitsrealisaties */
#endif

  for (i = 0; i < FCMAX; ++i)
  {
    AltRuimte[i] = 9999;              /* initialiseer AltRuimte[] */

    for (k = 0; k < FCMAX; ++k)
    {
      mulv ontruim = GK_conflict(i,k);
      if (ontruim > NG)
      {
        if (RA[k] || G[k] && !MG[k])
        {
          AltRuimte[i] = NG;          /* negatief altijd voldoende om alternatieve realisatie te voorkomen */
        }
        if (R[k] && (REALtraffick[k] > NG))            /* conflict gevonden die als eerste aan de beurt is */
        {
          if (REALtraffick[k] - MTG[i] - ontruim < AltRuimte[i])
          {
            AltRuimte[i] = REALtraffick[k] - MTG[i] - ontruim;
            if (AltRuimte[i] < 0) AltRuimte[i] = NG;
          }
        }
      }
      if (AltRuimte[i] == NG) break;
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal of voor een conflicterende richting een REALtraffick[] is afgegeven                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie controleert of een fasecyclus een conflicterende richting heeft waarvan REALtraffick[]      */
/* bekend (> NG) is. Als dit het geval is wordt een wachtstand aanvraag gereset.                            */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_WGR().                                                  */
/*                                                                                                          */
boolv REALconflict(                   /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count k;

  for (k = 0; k < FCMAX; ++k)
  {
    if (GK_conflict(fc,k) > NG)
    {
      if (!G[k] && (REALtraffick[k] > NG)) return TRUE;
    }
  }
  return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal wachtstand groen                                                                          */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt of richtingen in wachtstand groen kunnen blijven. De functie bestuurt hiervoor      */
/* de bits RW BIT4 en WS BIT1 en BIT4.                                                                      */
/*                                                                                                          */
/* Het verschil met TLCgen is dat wachtstand groen niet eindigt bij een fictieve conflict aanvraag maar wel */
/* geblokkeerd wordt als een conflicterende prioriteitsaanvraag gehonoreerd is terwijl die richting nog     */
/* geen aanvraag heeft.                                                                                     */
/* (dit laatste is nodig omdat anders de afgegeven tijd tot groen niet altijd haalbaar is)                  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Wachtgroen_Add().                                                       */
/*                                                                                                          */
void Traffick2TLCgen_WGR(void)        /* Fik230101                                                          */
{
  count i;

  for (i = 0; i < FCMAX; ++i)
  {
    RW[i] &= ~BIT4;
    WS[i] &= ~BIT1;
    WS[i] &= ~BIT4;

    if (WGR[i] && yws_groen(i) && (G[i] || !fka(i)))
    {
      RW[i] |= BIT4;
      WS[i] |= BIT4;
      if (WG[i] && WGR[i] && yws_groen(i)) WS[i] |= BIT1;
    }
    if (RV[i] && REALconflict(i)) A[i] &= ~BIT2;
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc1 = hki_kop[i].fc1;     /* voedende richting */
      count fc2 = hki_kop[i].fc2;     /* volg     richting */

      if (WS[fc1] && WG[fc2])
      {
        RW[fc2] |= BIT4;
        WS[fc2] |= BIT1;
        WS[fc2] |= BIT4;
      }
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verwerk extra meetkriterium detector - geen gescheiden hiaatmeting per rijstrook                 */
/* -------------------------------------------------------------------------------------------------------- */
/* In TLCgen kan een detector geen twee meetkriteria aanhouden. Deze functie maakt dit wel mogelijk.        */
/* Het meetkriterium wordt door deze functie gecorrigeerd.                                                  */
/* De functie is afgeleid van meetkriterium1_prm_va_arg() uit het standaard CCOL pakket.                    */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Meetkriterium_Add().                                                    */
/*                                                                                                          */
void corrigeer_meetkriterium1_va_arg(count fc, count tkopmaxnr, ...)                /* TvG                  */
{                                                                                   /* wijziging Fik230101  */
/* count fc 		 * arraynummer fasecyclus			*/
/* count tkopmaxnr	 * arraynummer tijd kopmaximum			*/
   va_list argpt;	/* variabele argumentenlijst			*/
   count  dpnr;		/* arraynummer detectie-element			*/
   mulv   prm;		/* waarde verlengparameter			*/
   boolv  hmk1_2, hmk3;	/* hulpwaarden meetkriterium			*/
   boolv htdh_status;	/* hulpwaarde status hiaatmeting		*/

   va_start(argpt,tkopmaxnr);		/* start var. parameterlijst	*/
   if (G[fc]) {				/* alleen tijdens groen 	*/
      if (TFG[fc]) {                    /* test lopen vastgroentijd	*/
	 MK[fc] |= (BIT1+BIT2);		/* zet bit 1 en 2		*/
      }
      else {
	 hmk1_2=hmk3= 0;		  /* hulpwaarden worden 0	*/
	 do {
	    dpnr= va_arg(argpt, va_count);/* lees array-nummer detectie	*/
	    if (dpnr>=0) {
	       prm= va_arg(argpt, va_mulv); /* lees waarde parameter	*/
	       if (prm>END) {
                  htdh_status = TDH[dpnr] || FC_DVM[fc] && TDH_DVM[dpnr];
		  if (prm && htdh_status) { /* test waarde en hiaattijd	*/
		     if (prm==1)  {
			hmk1_2 |= BIT1;     /* set verlengbit 1		*/
		     }
		     else if (prm==2)  {
			hmk1_2 |= BIT2;     /* set verlengbit 2		*/
		     }
		     else {
			hmk3 |= BIT3;       /* set verlengbit 3		*/
		     }
		  }
	       }
	    }
	 } while (dpnr>=0 && prm>END);	/* laatste parameter?		*/
	 if (tkopmaxnr>=0) {		/* kopmaximum gebruikt?		*/
	    if (!T[tkopmaxnr])	 	/* test lopen tijd kopmaximum	*/
	       hmk1_2 &= ~BIT1;	        /* niet lopen -> reset bit1	*/
	 }

         if ((MK_old[fc] & BIT1) && (hmk1_2 & BIT1)) MK[fc] |= BIT1;
         if ((MK_old[fc] & BIT2) && (hmk1_2 & BIT2)) MK[fc] |= BIT2;

	 MK[fc] |= hmk3;		/* set verleng bit 3		*/
      }
      MK_old[fc] = MK[fc];
   }
   va_end(argpt);			/* maak var. arg-lijst leeg	*/
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verwerk extra meetkriterium detector - gescheiden hiaatmeting per rijstrook                      */
/* -------------------------------------------------------------------------------------------------------- */
/* In TLCgen kan een detector geen twee meetkriteria aanhouden. Deze functie maakt dit wel mogelijk.        */
/* Zowel het meetkriterium als het bijbehorende geheugenelement worden door deze functie gecorrigeerd.      */
/* De functie is afgeleid van meetkriterium2_prm_va_arg() uit het standaard CCOL pakket.                    */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Meetkriterium_Add().                                                    */
/*                                                                                                          */
void corrigeer_meetkriterium2_det_va_arg(count fc, count tkopmaxnr, count mmk, ...) /* TvG                  */
{                                                                                   /* wijziging Fik230101  */
/* count fc 		 * arraynummer fasecyclus			*/
/* count tkopmaxnr	 * arraynummer tijd kopmaximum			*/
/* count mmk		 * arraynummer geheugelement meetkriterium	*/
   va_list argpt;	/* variabele argumentenlijst			*/
   count  dpnr;		/* arraynummer detectie-element			*/
   mulv  prmrystr;	/* waarde rijstrookparameter 			*/
   mulv  prmmk;		/* waarde verlengparameter			*/
   mulv  hmk1_2, hmk3;	/* hulpwaarden meetkriterium			*/
   mulv  hmk1_2a;       /* hulpwaarden meetkriterium rijstrook a	*/
   mulv  hmk1_2b;       /* hulpwaarden meetkriterium rijstrook b	*/
   mulv  hmk1_2c;       /* hulpwaarden meetkriterium rijstrook c	*/
   mulv  htkopmax=TRUE; /* hulpwaarde tijdkopmaximum			*/
   mulv  htdh_status;	/* status hiaatmeting				*/

   va_start(argpt,mmk);			/* start var. parameterlijst	*/
   if (G[fc]) {				/* alleen tijdens groen 	*/
      if (tkopmaxnr>=0) {		/* kopmaximum gebruikt?		*/
	 if (!T[tkopmaxnr])	 	/* test lopen tijd kopmaximum	*/
	    htkopmax= FALSE;            /* niet lopen -> reset bit1	*/
      }
      if (TFG[fc]) {                    /* test lopen vastgroentijd	*/
	 htkopmax= TRUE;
      }
      else {
	 hmk1_2=hmk3= 0;		  /* hulpwaarden worden 0	*/
	 hmk1_2a=hmk1_2b=hmk1_2c= 0;	  /* hulpwaarden worden 0	*/
	 do {
            dpnr= va_arg(argpt, va_count);/* lees array-nummer detectie	*/
	    if (dpnr>=0) {
	       prmmk=   va_arg(argpt, va_mulv); /* lees waarde parameter  */
	       prmrystr= prmmk>>2;		/* lees waarde rijstrook  */
	       prmmk &= BIT0 | BIT1;            /* skip waarde rijstrook  */
	       if (prmmk>END) {
                  htdh_status = TDH[dpnr] || FC_DVM[fc] && TDH_DVM[dpnr];
		  if (prmmk && htdh_status) {   /* test waarde en hiaattijd    */
		     if ((prmmk==(va_mulv) 1))  {
			if (htkopmax) {
			   switch (prmrystr) {
			      case 1:
				 if (mmk_old[fc] & BIT4)
				    hmk1_2a |= BIT4 | BIT5;/* set verlengb 1+2 */
				 break;
			      case 2:
				 if (mmk_old[fc] & BIT6)
				    hmk1_2b |= BIT6 | BIT7;/* set verlengb 1+2 */
				 break;
			      case 3:
				 if (mmk_old[fc] & BIT8)
				    hmk1_2c |= BIT8 | BIT9;/* set verlengb 1+2 */
				 break;
			      default:
				 if (mmk_old[fc] & BIT1)
				    hmk1_2 |= BIT1 | BIT2; /* set verlengb 1+2 */
				 break;
			    }
			 }
		     }
		     else if (prmmk==(va_mulv) 2)  {
			switch (prmrystr) {
			   case 1:
			      hmk1_2a |= BIT5;    /* set verlengbit 2	*/
			      break;
			   case 2:
			      hmk1_2b |= BIT7;    /* set verlengbit 2	*/
			      break;
			   case 3:
			      hmk1_2c |= BIT9;    /* set verlengbit 2	*/
			      break;
			   default:
			      hmk1_2  |= BIT2;    /* set verlengbit 2	*/
			      break;
			 }
		     }
		     else {
			hmk3 |= BIT3;       /* set verlengbit 3	*/
		     }
		  }
	       }
	    }
	 } while (dpnr>=0 && prmmk>END);	/* laatste parameter?		*/

	 MM[mmk] |= hmk1_2 | hmk1_2a | hmk1_2b | hmk1_2c;

	 if (MM[mmk] & (BIT1 | BIT4 | BIT6 | BIT8))
	    MK[fc] |= BIT1;

	 if (MM[mmk] & (BIT2 | BIT5 | BIT7 | BIT9))
	    MK[fc] |= BIT2;

	 MK[fc] |= hmk3;		/* set verleng bit 3		*/
         mmk_old[fc] = MM[mmk];		/* buffer MM[mmk]		*/
      }
   }
   va_end(argpt);			/* maak var. arg-lijst leeg	*/
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal maximaal meeverlengen                                                                     */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt of richtingen nog kunnen meeverlengen op basis van "maatgevend groen". Indien dit   */
/* niet meer het geval is wordt YM[] BIT4 gereset.                                                          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Meeverlengen_Add().                                                     */
/*                                                                                                          */
void Traffick2TLCgen_MVG(void)        /* Fik230101                                                          */
{
  count i,k;

  boolv hf_mvg = FALSE;               /* hulpfunctie meeverlenggroen toegestaan */
  boolv fc_eerst[FCMAX];              /* richting is als eerstvolgende aan de beurt */
  boolv ym_reset[FCMAX];              /* meeverlenggroen moet beeindigd worden */
  boolv rw_reset[FCMAX];              /* RW[] BIT2 door TLCgen opgezet tijdens RA[] voedende richting */
  boolv fc_mv_dc[FCMAX];              /* meeverlenggroen van richting die in deelconflict mag meeverlengen */

  for (i = 0; i < FCMAX; ++i)         /* initialiseer YM[] BIT 4 en hulp variabelen */
  {
    YM[i]      &= ~BIT4;
    fc_eerst[i] = FALSE;
    ym_reset[i] = FALSE;
    rw_reset[i] = TRUE;
    fc_mv_dc[i] = FALSE;
    hf_mvg |= !G[i] && (REALtraffick[i] > NG);
  }

  for (i = 0; i < FCMAX; ++i)         /* bepaal welke richtingen als eerst volgende aan de beurt zijn */
  {
    fc_eerst[i] = R[i] && A[i] && AAPR[i] && !(AAPR[i]&BIT5) && !fkra(i) && !(RR[i]&BIT6) && !(RR[i]&BIT10) || RA[i] || AAPRprio[i];
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc1    = hki_kop[i].fc1;    /* voedende richting */
      count fc2    = hki_kop[i].fc2;    /* volg     richting */
      count status = hki_kop[i].status; /* status koppeling */

      if (!G[fc2] && fc_eerst[fc1] || (status == 1)) fc_eerst[fc2] = TRUE;
    }
  }

  for (i = 0; i < aantal_vtg_tgo; ++i)
  {
    count fc1      = vtg_tgo[i].fc1;      /* richting 1 */
    count fc2      = vtg_tgo[i].fc2;      /* richting 2 */
    count hnla12   = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
    count hnla21   = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
    count status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
    count status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

    if (fc_eerst[fc1] && IH[hnla12] || (status12 == 1))
    {
      if (!G[fc2]) fc_eerst[fc2] = TRUE;
    }
    if (fc_eerst[fc2] && IH[hnla21] || (status21 == 1))
    {
      if (!G[fc1]) fc_eerst[fc1] = TRUE;
    }
    if ((status12 > 0) && (status12 < 4)) rw_reset[fc2] = FALSE;
    if ((status21 > 0) && (status21 < 4)) rw_reset[fc1] = FALSE;
  }

  for (i = 0; i < aantal_lvk_gst; ++i)
  {
    count fc1 = lvk_gst[i].fc1;       /* richting 1 */
    count fc2 = lvk_gst[i].fc2;       /* richting 2 */
    count fc3 = lvk_gst[i].fc3;       /* richting 3 */
    count fc4 = lvk_gst[i].fc4;       /* richting 4 */

    boolv FcEerst = FALSE;
    if ((fc1 != NG) && fc_eerst[fc1]) FcEerst = TRUE;
    if ((fc2 != NG) && fc_eerst[fc2]) FcEerst = TRUE;
    if ((fc3 != NG) && fc_eerst[fc3]) FcEerst = TRUE;
    if ((fc4 != NG) && fc_eerst[fc4]) FcEerst = TRUE;

    if (FcEerst)
    {
      if ((fc1 != NG) && (GL[fc1] || TRG[fc1] || R[fc1] && A[fc1])) fc_eerst[fc1] = TRUE;
      if ((fc2 != NG) && (GL[fc2] || TRG[fc2] || R[fc2] && A[fc2])) fc_eerst[fc2] = TRUE;
      if ((fc3 != NG) && (GL[fc3] || TRG[fc3] || R[fc3] && A[fc3])) fc_eerst[fc3] = TRUE;
      if ((fc4 != NG) && (GL[fc4] || TRG[fc4] || R[fc4] && A[fc4])) fc_eerst[fc4] = TRUE;
    }
  }

  for (i = 0; i < aantal_dcf_vst; ++i)
  {
    count fc1  = dcf_vst[i].fc1;      /* richting die voorstart geeft  */
    count fc2  = dcf_vst[i].fc2;      /* richting die voorstart krijgt */
    count ma21 = dcf_vst[i].ma21;     /* meerealisatie van fc2 met fc1 */
    count mv21 = dcf_vst[i].mv21;     /* meeverlengen  van fc2 met fc1 */

    if (fc_eerst[fc1])
    {
      if (GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || SCH[ma21])) fc_eerst[fc2] = TRUE;
    }
    if (G[fc1] && G[fc2] && SCH[mv21]) fc_mv_dc[fc2] = TRUE;
  }

  for (i = 0; i < FCMAX; ++i)         /* bepaal of meeverlengen mag ogv instellingen */
  {
    if ((MGR[i] || fc_mv_dc[i]) && (MK[i] || !MMK[i] || fc_mv_dc[i]) && (hf_wsg_fcfc(0, FCMAX) || hf_mvg)) YM[i] |= BIT4;
    else
    {
      ym_reset[i] = TRUE;
    }
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc1    = hki_kop[i].fc1;    /* voedende richting */
      count fc2    = hki_kop[i].fc2;    /* volg     richting */
      boolv kop_eg = hki_kop[i].kop_eg; /* koppeling vanaf einde groen (als FALSE dan vanaf EV) */
      count status = hki_kop[i].status; /* status koppeling */

      if (kop_eg && !hf_wsg_nl_fcfc(0, FCMAX) && !hf_mvg)
      {
        YM[fc1] &= ~BIT4;
        ym_reset[fc1] = TRUE;
      }
      if ((status > 0) && (status < 4)) rw_reset[fc2] = FALSE;
    }
  }

  for (i = 0; i < FCMAX; ++i)
  {
    if (MG[i] && !ym_reset[i])        /* richting staat in MG[], ga op zoek naar "direct" conflict die aan de beurt is */
    {
      for (k = 0; k < FCMAX; ++k)
      {
        mulv ontruim = GK_conflict(i,k);
        if (ontruim > NG)
        {
          if (fc_eerst[k])            /* conflict gevonden die aan de beurt is */
          {
            if (REALtraffick[k] <= ontruim) ym_reset[i] = TRUE;
          }
        }
      }
    }
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc1      = hki_kop[i].fc1;      /* voedende richting */
      count fc2      = hki_kop[i].fc2;      /* volg     richting */
      count tnlfg12  = hki_kop[i].tnlfg12;  /* vaste   nalooptijd vanaf einde vastgroen      fc1 */
      count tnlfgd12 = hki_kop[i].tnlfgd12; /* det.afh.nalooptijd vanaf einde vastgroen      fc1 */
      count tnleg12  = hki_kop[i].tnleg12;  /* vaste   nalooptijd vanaf einde (verleng)groen fc1 */
      count tnlegd12 = hki_kop[i].tnlegd12; /* det.afh.nalooptijd vanaf einde (verleng)groen fc1 */
      boolv kop_eg   = hki_kop[i].kop_eg;   /* koppeling vanaf einde groen (als FALSE dan vanaf EV) */
      mulv  kop_max  = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */

      if (fc_eerst[fc1])              /* voedende richting harde koppeling is aan de beurt, ga op zoek naar conflicten van de volgrichting die in MG[] staan */
      {
        for (k = 0; k < FCMAX; ++k)
        {
          mulv ontruim = GK_conflict(k,fc2);
          if (ontruim > NG)
          {
            if (MG[k])                /* conflict volgrichting gevonden die in MG[] staat */
            {
              if ((REALtraffick[fc2] <= ontruim) || BMC[fc2] && RA[fc1] && !tkcv(fc1)) ym_reset[k] = TRUE;
            }
          }
        }
      }

      if (MG[fc1] && kop_eg)          /* voedende richting harde koppeling staat in MG[], ga op zoek naar een conflict van de volgrichting die aan de beurt is */
      {
        mulv naloop_tijd = 0;

        if ((tnlfg12  != NG)                                                       ) naloop_tijd = T_max[tnlfg12]  - T_timer[tnlfg12];
        if ((tnlfgd12 != NG) && (naloop_tijd < T_max[tnlfgd12] - T_timer[tnlfgd12])) naloop_tijd = T_max[tnlfgd12] - T_timer[tnlfgd12];
        if ((tnleg12  != NG) && (naloop_tijd < T_max[tnleg12]                     )) naloop_tijd = T_max[tnleg12];
        if ((tnlegd12 != NG) && (naloop_tijd < T_max[tnlegd12] + TGL_max[fc1]     )) naloop_tijd = T_max[tnlegd12] + TGL_max[fc1];

        naloop_tijd += kop_max;

        for (k = 0; k < FCMAX; ++k)
        {
          mulv ontruim = GK_conflict(fc2,k);
          if (ontruim > NG)
          {
            if (fc_eerst[k])          /* conflict volgrichting gevonden die aan de beurt is */
            {
              if (REALtraffick[k] <= ontruim + naloop_tijd) ym_reset[fc1] = TRUE;
            }
          }
          if (ym_reset[fc1]) break;
        }
      }
    }
  }

  for (i = 0; i < aantal_dcf_vst; ++i)
  {
    count fc1  = dcf_vst[i].fc1;      /* richting die voorstart geeft  */
    count fc2  = dcf_vst[i].fc2;      /* richting die voorstart krijgt */
    count mv21 = dcf_vst[i].mv21;     /* meeverlengen  van fc2 met fc1 */

    if (MG[fc1] && MG[fc2] && SCH[mv21] && ym_reset[fc2]) ym_reset[fc1] = TRUE;
  }

  for (i = 0; i < FCMAX; ++i)         /* reset YM BIT4 */
  {
    if (ym_reset[i]) YM[i] &= ~BIT4;
  }

  for (i = 0; i < FCMAX; ++i)         /* correctie TLCgen geen RW[] maar YM[] tijdens RA[] voedende richting */
  {
    if (rw_reset[i] && (RW[i]&BIT2))
    {
      RW[i] &= ~BIT2;
      YM[i] |=  BIT4;
    }
  }

  for (i = 0; i < aantal_vtg_tgo; ++i)
  {
    count fc1      = vtg_tgo[i].fc1;      /* richting 1 */
    count fc2      = vtg_tgo[i].fc2;      /* richting 2 */
    count status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
    count status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

    if (MG[fc1] && RA[fc2] && (status12 == 1)) YM[fc1] |= BIT4; /* vasthouden voeding is nodig omdat */
    if (MG[fc2] && RA[fc1] && (status21 == 1)) YM[fc2] |= BIT4; /* nog aan de overzijde gedrukt kan worden */
  }

  for (i = 0; i < aantal_lvk_gst; ++i)
  {
    count fc1 = lvk_gst[i].fc1;       /* richting 1 */
    count fc2 = lvk_gst[i].fc2;       /* richting 2 */
    count fc3 = lvk_gst[i].fc3;       /* richting 3 */
    count fc4 = lvk_gst[i].fc4;       /* richting 4 */

    boolv FcInRa = FALSE;
    if ((fc1 != NG) && RA[fc1]) FcInRa = TRUE;
    if ((fc2 != NG) && RA[fc2]) FcInRa = TRUE;
    if ((fc3 != NG) && RA[fc3]) FcInRa = TRUE;
    if ((fc4 != NG) && RA[fc4]) FcInRa = TRUE;

    if (FcInRa)
    {
      if ((fc1 != NG) && MG[fc1]) YM[fc1] |= BIT4;
      if ((fc2 != NG) && MG[fc2]) YM[fc2] |= BIT4;
      if ((fc3 != NG) && MG[fc3]) YM[fc3] |= BIT4;
      if ((fc4 != NG) && MG[fc4]) YM[fc4] |= BIT4;
    }
  }

  for (i = 0; i < aantal_dcf_vst; ++i)
  {
    count fc1  = dcf_vst[i].fc1;      /* richting die voorstart geeft  */
    count fc2  = dcf_vst[i].fc2;      /* richting die voorstart krijgt */
    count mv21 = dcf_vst[i].mv21;     /* meeverlengen  van fc2 met fc1 */

    if (RA[fc1] || G[fc1] && SCH[mv21]) YM[fc2] |= BIT4;
  }

  for (i = 0; i < aantal_pel_kop; ++i)
  {
    count kop_fc    = pel_kop[i].kop_fc;    /* koppelrichting */
    count kop_toe   = pel_kop[i].kop_toe;   /* toestemming peloton ingreep */
    count vast_vert = pel_kop[i].vast_vert; /* vasthoud vertraging  (start op binnenkomst koppelsignaal) */
    mulv  aanw_kop1 = pel_kop[i].aanw_kop1; /* aanwezigheidsduur koppelsignaal 1 vanaf start puls */
    mulv  pk_status = pel_kop[i].pk_status; /* status peloton ingreep */

    YM[kop_fc] &= ~BIT12;
    if ((pk_status > NG) && MM[kop_toe] && (aanw_kop1 > T_max[vast_vert]))
    {
      if (G[kop_fc] && !DOSEER[kop_fc])
      {
        YM[kop_fc] |= BIT12;
      }
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal uitstel                                                                                   */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt het uitstel voor deelconflicten en koppelingen. De functie maakt hiervoor gebruik   */
/* van X[] BIT3 en RR[] BIT3. Indien een richting in RA[] met P[] een voorstart verleent of onderdeel is    */
/* van een gelijkstart voor langzaam verkeer dan wordt de P[] doorgeschreven naar de gekoppelde richtingen. */
/* De functie maakt hiervoor gebruik van P[] BIT3.                                                          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Synchronisaties_Add().                                                  */
/*                                                                                                          */
void Traffick2TLCgen_uitstel(void)    /* Fik230101                                                          */
{
  count i;
  mulv  aantal_x3     = 0;            /* bepalen van uitstel is een iteratief proces */
  mulv  aantal_x3_old = NG;           /* ... tot er geen nieuwe richtingen meer een X[] BIT3 krijgen */

  for (i = 0; i < FCMAX; ++i)
  {
    P[i]  &= ~BIT3;                   /* reset P, X en RR bit synchronisatie */
    X[i]  &= ~BIT3;
    RR[i] &= ~BIT3;
    if (RA[i] && (Aled[i] > 0)) P[i] |= BIT3;
  }

  while (aantal_x3 > aantal_x3_old)
  {
    aantal_x3_old = aantal_x3;

    for (i = 0; i < aantal_hki_kop; ++i)
    {
      count fc1     = hki_kop[i].fc1;     /* voedende richting */
      count fc2     = hki_kop[i].fc2;     /* volg     richting */
      count tlr21   = hki_kop[i].tlr21;   /* late release fc2 (= inrijtijd fc1) */
      boolv los_fc2 = hki_kop[i].los_fc2; /* fc2 mag bij aanvraag fc1 los realiseren */
      mulv  status  = hki_kop[i].status;  /* status koppeling */

      mulv             inrijfc1 = 0;
      if (tlr21 != NG) inrijfc1 = T_max[tlr21];

      if (!los_fc2 && (status != 1))
      {
        if (GL[fc1] || TRG[fc1] || R[fc1] && A[fc1])
        {
          if (RA[fc1] && (X[fc1] || K[fc1])) X[fc2] |= BIT3;
          if (RV[fc1] || RA[fc1] && RR[fc1] && !P[fc1]) RR[fc2] |= BIT3;
        }
      }
      if (RA[fc1] && (inrijfc1 == 0) && (RV[fc2] || X[fc2] || K[fc2] || (TMP_ontruim(fc2) > 0) || RA[fc2] && RR[fc2] && !P[fc2])) X[fc1] |= BIT3;
      if (RA[fc1] && (inrijfc1 >  0) && ((REALtraffick[fc2] > inrijfc1) || (MTG[fc2] > inrijfc1))) X[fc1] |= BIT3;
    }

    for (i = 0; i < aantal_dcf_vst; ++i)
    {
      count fc1   = dcf_vst[i].fc1;   /* richting die voorstart geeft  */
      count fc2   = dcf_vst[i].fc2;   /* richting die voorstart krijgt */
      count tvs21 = dcf_vst[i].tvs21; /* voorstart fc2                 */
      count ma21  = dcf_vst[i].ma21;  /* meerealisatie van fc2 met fc1 */

      boolv fc2_eerst = GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || SCH[ma21]) || G[fc2] && (TG_timer[fc2] < T_max[tvs21]);

      if ( G[fc1] && (!MG[fc1] || (YM[fc1]&PRIO_YM_BIT))) RR[fc2] |= BIT3;
      if (!G[fc2] && fc2_eerst) X[fc1] |= BIT3;
      /* if (RA[fc1] && !K[fc1] && SCH[ma21]) P[fc1] |= BIT3; */
      if (RA[fc1] &&   P[fc1] && !P[fc2] ) P[fc2] |= BIT3;
      if (RA[fc1] && (VS[fc2] || FG[fc2])) P[fc1] |= BIT3;

                                       /* overrule TLCgen */
                                       /* ... als P[] opstaat kan TLCgen toch agv hoge REALtijd X[] BIT1 continue opzetten */
      if (RA[fc2] && !K[fc2] && P[fc2] && (TMP_ontruim(fc2) == 0)) X[fc2] &= ~BIT1;
    }

    for (i = 0; i < aantal_vtg_tgo; ++i)
    {
      count fc1    = vtg_tgo[i].fc1;    /* richting 1 */
      count fc2    = vtg_tgo[i].fc2;    /* richting 2 */
      count hnla12 = vtg_tgo[i].hnla12; /* drukknop melding koppeling vanaf fc1 aanwezig */
      count hnla21 = vtg_tgo[i].hnla21; /* drukknop melding koppeling vanaf fc2 aanwezig */
      count hlos1  = vtg_tgo[i].hlos1;  /* los realiseren fc1 toegestaan */
      count hlos2  = vtg_tgo[i].hlos2;  /* los realiseren fc2 toegestaan */

      if (RV[fc1] && IH[hnla12] && tkcv(fc2)) RR[fc1] |= BIT3;
      if (RV[fc2] && IH[hnla21] && tkcv(fc1)) RR[fc2] |= BIT3;

      if (RA[fc1] && RA[fc2])
      {
        if (P[fc1] && IH[hnla12]) P[fc2] |= BIT3;
        if (P[fc2] && IH[hnla21]) P[fc1] |= BIT3;

        if ((IH[hnla12] && IH[hnla21] || R[fc1] && A[fc1] && IH[hlos1] && R[fc2] && A[fc2] && IH[hlos2]) &&
            (K[fc1] || K[fc2] || (TMP_ontruim(fc1) > 0) || (TMP_ontruim(fc2) > 0)))
        {
          X[fc1] |= BIT3;
          X[fc2] |= BIT3;
        }
      }

      if (R[fc1] && R[fc2] && (IH[hnla12] || IH[hnla21]))
      {
        if (P[fc1]&BIT3) P[fc2] |= BIT3;
        if (P[fc2]&BIT3) P[fc1] |= BIT3;

        if (RR[fc1]&BIT3) RR[fc2] |= BIT3;
        if (RR[fc2]&BIT3) RR[fc1] |= BIT3;
      }

      if ((RA[fc1] || G[fc1]) && (RA[fc2] || G[fc2]) && !K[fc1] && !K[fc2] && (TMP_ontruim(fc1) == 0) && (TMP_ontruim(fc2) == 0))
      {
        X[fc1] &= ~BIT1; /* overrule TLCgen */
        X[fc2] &= ~BIT1; /* ... als P[] opstaat kan TLCgen toch agv hoge REALtijd X[] BIT1 continue opzetten */
      }
    }

    for (i = 0; i < aantal_lvk_gst; ++i)
    {
      count fc1 = lvk_gst[i].fc1;     /* richting 1 */
      count fc2 = lvk_gst[i].fc2;     /* richting 2 */
      count fc3 = lvk_gst[i].fc3;     /* richting 3 */
      count fc4 = lvk_gst[i].fc4;     /* richting 4 */

      boolv uitstel   = FALSE;
      boolv privilege = FALSE;
      if (fc1 != NG)
      {
        if (GL[fc1] || TRG[fc1] || RV[fc1]  &&  A[fc1] ||
            RA[fc1] && (P[fc1] || !RR[fc1]) && (K[fc1] || (TMP_ontruim(fc1) > 0) || X[fc1])) uitstel = TRUE;
        if (RA[fc1] &&  P[fc1]) privilege = TRUE;
      }
      if (fc2 != NG)
      {
        if (GL[fc2] || TRG[fc2] || RV[fc2]  &&  A[fc2] ||
            RA[fc2] && (P[fc2] || !RR[fc2]) && (K[fc2] || (TMP_ontruim(fc2) > 0) || X[fc2])) uitstel = TRUE;
        if (RA[fc2] &&  P[fc2]) privilege = TRUE;
      }
      if (fc3 != NG)
      {
        if (GL[fc3] || TRG[fc3] || RV[fc3]  &&  A[fc3] ||
            RA[fc3] && (P[fc3] || !RR[fc3]) && (K[fc3] || (TMP_ontruim(fc3) > 0)|| X[fc3])) uitstel = TRUE;
        if (RA[fc3] &&  P[fc3]) privilege = TRUE;
      }
      if (fc4 != NG)
      {
        if (GL[fc4] || TRG[fc4] || RV[fc4]  &&  A[fc4] ||
            RA[fc4] && (P[fc4] || !RR[fc4]) && (K[fc4] || (TMP_ontruim(fc4) > 0) || X[fc4])) uitstel = TRUE;
        if (RA[fc4] &&  P[fc4]) privilege = TRUE;
      }

      if (uitstel)
      {
        if ((fc1 != NG) && RA[fc1]) X[fc1] |= BIT3;
        if ((fc2 != NG) && RA[fc2]) X[fc2] |= BIT3;
        if ((fc3 != NG) && RA[fc3]) X[fc3] |= BIT3;
        if ((fc4 != NG) && RA[fc4]) X[fc4] |= BIT3;
      }

      if (privilege)
      {
        if ((fc1 != NG) && R[fc1] && !P[fc1]) P[fc1] |= BIT3;
        if ((fc2 != NG) && R[fc2] && !P[fc2]) P[fc2] |= BIT3;
        if ((fc3 != NG) && R[fc3] && !P[fc3]) P[fc3] |= BIT3;
        if ((fc4 != NG) && R[fc4] && !P[fc4]) P[fc4] |= BIT3;
      }
    }

    aantal_x3 = 0;
    for (i = 0; i < FCMAX; ++i)
    {
      if (X[i]&BIT3) aantal_x3++;
    }
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    count fc1 = hki_kop[i].fc1;       /* voedende richting */
    count fc2 = hki_kop[i].fc2;       /* volg     richting */
                                      /* bugfix TLCgen     */
    if (PRML[ML][fc1] != PRIMAIR) REAL_SYN[fc1][fc2] = REAL_SYN[fc2][fc1] = FALSE;
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal periode alternatieve realisatie                                                           */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt de periode waarin alternatief realiseren is toegestaan. De functie maakt gebruik    */
/* van de realisatie ruimte zoals bepaald in REALTraffick(). De functie overschrijft PAR[] zoals bepaald    */
/* door TLCgen.                                                                                             */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Alternatief_Add().                                                      */
/*                                                                                                          */
void Traffick2TLCgen_PAR(void)        /* Fik230101                                                          */
{
  count i;

  for (i = 0; i < FCMAX; ++i)         /* initialiseer PAR[] */
  {
    PAR[i] = R[i] && ART[i] && (AltRuimte[i] > AR_max[i]) && !tfkaa(i) && !tkcv(i);

    if (ARB[i] > NG)                  /* reset PAR[] als blok actief waarin alternatief niet is toegestaan */
    {
      if (!(ARB[i] & (1 << ML))) PAR[i] = FALSE;
    }
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc1      = hki_kop[i].fc1;      /* voedende richting */
      count fc2      = hki_kop[i].fc2;      /* volg     richting */
      count tlr21    = hki_kop[i].tlr21;    /* late release fc2 (= inrijtijd) */
      count tnlfg12  = hki_kop[i].tnlfg12;  /* vaste   nalooptijd vanaf einde vastgroen      fc1 */
      count tnlfgd12 = hki_kop[i].tnlfgd12; /* det.afh.nalooptijd vanaf einde vastgroen      fc1 */
      count tnleg12  = hki_kop[i].tnleg12;  /* vaste   nalooptijd vanaf einde (verleng)groen fc1 */
      count tnlegd12 = hki_kop[i].tnlegd12; /* det.afh.nalooptijd vanaf einde (verleng)groen fc1 */
      boolv kop_eg   = hki_kop[i].kop_eg;   /* koppeling vanaf einde groen (als FALSE dan vanaf EV) */
      boolv los_fc2  = hki_kop[i].los_fc2;  /* fc2 mag bij aanvraag fc1 los realiseren */
      mulv  kop_max  = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */
      mulv  status   = hki_kop[i].status;   /* status koppeling */

      mulv  tijd_tot_sg = 0;
      mulv  naloop_tijd = 0;

      mulv             inrijfc1 = 0;
      if (tlr21 != NG) inrijfc1 = T_max[tlr21];

      if (PAR[fc1] && (G[fc2] || PAR[fc2]))
      {
        tijd_tot_sg = MTG[fc1];       /* bepaal start groen moment voedende richting */
        if (MTG[fc2] - inrijfc1 > tijd_tot_sg) tijd_tot_sg = MTG[fc2] - inrijfc1;

        naloop_tijd = 0;              /* controleer of naloop vanaf einde vastgroen past */
        if ( tnlfg12  != NG) naloop_tijd = T_max[tnlfg12];
        if ((tnlfgd12 != NG) && (naloop_tijd < T_max[tnlfgd12])) naloop_tijd = T_max[tnlfgd12];
        if (MTG[fc2] + AltRuimte[fc2] <= tijd_tot_sg + TFG_max[fc1] + naloop_tijd + kop_max) PAR[fc1] = FALSE;

        naloop_tijd = 0;              /* controleer of naloop vanaf einde (verleng)groen past */
        if ( tnleg12  != NG) naloop_tijd = T_max[tnleg12];
        if (kop_eg)
        {
          if (naloop_tijd < T_max[tnlegd12] + TGL_max[fc1]) naloop_tijd = T_max[tnlegd12] + TGL_max[fc1];
        }
        else
        {
          if (naloop_tijd < T_max[tnlegd12]) naloop_tijd = T_max[tnlegd12];
        }
        if (MTG[fc2] + AltRuimte[fc2] <= tijd_tot_sg + AR_max[fc1] + naloop_tijd + kop_max) PAR[fc1] = FALSE;
      }
      else PAR[fc1] = FALSE;

      if (!PAR[fc1] && !los_fc2 && (GL[fc1] || TRG[fc1] || R[fc1] && A[fc1])) PAR[fc2] = FALSE;

      if (R[fc2])                     /* volgrichting moet altijd mee realiseren */
      {
        if (G[fc1] || (status == 1)) PAR[fc2] = TRUE;
        if (tnlfg12  != NG) { if (RT[tnlfg12]  || T[tnlfg12] ) PAR[fc2] = TRUE; }
        if (tnlfgd12 != NG) { if (RT[tnlfgd12] || T[tnlfgd12]) PAR[fc2] = TRUE; }
        if (tnleg12  != NG) { if (RT[tnleg12]  || T[tnleg12] ) PAR[fc2] = TRUE; }
        if (tnlegd12 != NG) { if (RT[tnlegd12] || T[tnlegd12]) PAR[fc2] = TRUE; }
      }
    }
  }

  for (i = 0; i < aantal_vtg_tgo; ++i)
  {
    count fc1      = vtg_tgo[i].fc1;      /* richting 1 */
    count fc2      = vtg_tgo[i].fc2;      /* richting 2 */
    count tinl12   = vtg_tgo[i].tinl12;   /* inlooptijd fc1 */
    count tinl21   = vtg_tgo[i].tinl21;   /* inlooptijd fc2 */
    count tnlsgd12 = vtg_tgo[i].tnlsgd12; /* nalooptijd fc2 vanaf startgroen fc1 */
    count tnlsgd21 = vtg_tgo[i].tnlsgd21; /* nalooptijd fc1 vanaf startgroen fc2 */
    count hnla12   = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
    count hnla21   = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
    mulv  status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
    mulv  status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

    mulv  tijd_tot_sg = 0;

    mulv              inloop12 = 0;
    mulv              inloop21 = 0;

    if (tinl12 != NG) inloop12 = T_max[tinl12];
    if (tinl21 != NG) inloop21 = T_max[tinl21];

    if (IH[hnla12])                   /* drukknop melding koppeling vanaf fc1 aanwezig */
    {
      tijd_tot_sg = MTG[fc1];         /* bepaal start groen moment richting 1 */
      if (MTG[fc2] - inloop12 > tijd_tot_sg) tijd_tot_sg = MTG[fc2] - inloop12;

      if (!PAR[fc1] || (!PAR[fc2] && !G[fc2]) || (MTG[fc2] + AltRuimte[fc2] <= tijd_tot_sg + T_max[tnlsgd12])) PAR[fc1] = PAR[fc2] = FALSE;
      if (inloop12 > 0)
      {
        if (tijd_tot_sg + T_max[tnlsgd21] >= AltRuimte[fc1]) PAR[fc1] = PAR[fc2] = FALSE;    /* tijdens het inlopen kan fc2 nog aanvragen */
      }
    }

    if (IH[hnla21])                   /* drukknop melding koppeling vanaf fc2 aanwezig */
    {
      tijd_tot_sg = MTG[fc2];         /* bepaal start groen moment richting 2 */
      if (MTG[fc1] - inloop21 > tijd_tot_sg) tijd_tot_sg = MTG[fc1] - inloop21;

      if (!PAR[fc2] || (!PAR[fc1] && !G[fc1]) || (MTG[fc1] + AltRuimte[fc1] <= tijd_tot_sg + T_max[tnlsgd21])) PAR[fc1] = PAR[fc2] = FALSE;
      if (inloop21 > 0)
      {
        if (tijd_tot_sg + T_max[tnlsgd12] >= AltRuimte[fc2]) PAR[fc1] = PAR[fc2] = FALSE;    /* tijdens het inlopen kan fc1 nog aanvragen */
      }
    }

    if (IH[hnla12] && IH[hnla21])
    {
      if (MTG[fc1] > MTG[fc2]) tijd_tot_sg = MTG[fc1];
      else                     tijd_tot_sg = MTG[fc2];

      if (!PAR[fc1] || !PAR[fc2] || (MTG[fc2] + AltRuimte[fc2] <= tijd_tot_sg + T_max[tnlsgd12]) ||
                                    (MTG[fc1] + AltRuimte[fc1] <= tijd_tot_sg + T_max[tnlsgd21])) PAR[fc1] = PAR[fc2] = FALSE;
    }

    if (R[fc2] && (RT[tnlsgd12] || T[tnlsgd12] || (status12 == 1))) PAR[fc2] = TRUE; /* volgrichting moet altijd mee realiseren */
    if (R[fc1] && (RT[tnlsgd21] || T[tnlsgd21] || (status21 == 1))) PAR[fc1] = TRUE;

    if (RA[fc1] && P[fc1] && IH[hnla12]) PAR[fc2] = TRUE;                            /* volgrichting moet altijd mee realiseren */
    if (RA[fc2] && P[fc2] && IH[hnla21]) PAR[fc1] = TRUE;
  }

  for (i = 0; i < aantal_dcf_vst; ++i)
  {
    count fc1   = dcf_vst[i].fc1;     /* richting die voorstart geeft  */
    count fc2   = dcf_vst[i].fc2;     /* richting die voorstart krijgt */
    count tvs21 = dcf_vst[i].tvs21;   /* voorstart fc2 */
    count ma21  = dcf_vst[i].ma21;    /* meerealisatie van fc2 met fc1 */

    boolv fc2_eerst   = GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || SCH[ma21]);
    mulv  tijd_tot_sg = MTG[fc1];     /* bepaal start groen moment richting 1 */

    if (fc2_eerst && (MTG[fc2]  + T_max[tvs21]  > tijd_tot_sg)) tijd_tot_sg = MTG[fc2] + T_max[tvs21];
    if (G[fc2] && (T_max[tvs21] - TG_timer[fc2] > tijd_tot_sg)) tijd_tot_sg = T_max[tvs21] - TG_timer[fc2];

    if (fc2_eerst && !PAR[fc2] || (MTG[fc1] + AltRuimte[fc1] <= tijd_tot_sg + AR_max[fc1])) PAR[fc1] = FALSE;
  }

  for (i = 0; i < aantal_dcf_vst; ++i)
  {
    count fc1 = dcf_vst[i].fc1;       /* richting die voorstart geeft  */
    count fc2 = dcf_vst[i].fc2;       /* richting die voorstart krijgt */

    if (RA[fc1] && (VS[fc2] || FG[fc2])) PAR[fc1] = TRUE;
  }

  for (i = 0; i < aantal_lvk_gst; ++i)
  {
    count fc1 = lvk_gst[i].fc1;       /* richting 1 */
    count fc2 = lvk_gst[i].fc2;       /* richting 2 */
    count fc3 = lvk_gst[i].fc3;       /* richting 3 */
    count fc4 = lvk_gst[i].fc4;       /* richting 4 */

    boolv toestemming = TRUE;
    boolv realiseer   = FALSE;

    if ((fc1 != NG) && R[fc1] && A[fc1] && !PAR[fc1]) toestemming = FALSE;
    if ((fc2 != NG) && R[fc2] && A[fc2] && !PAR[fc2]) toestemming = FALSE;
    if ((fc3 != NG) && R[fc3] && A[fc3] && !PAR[fc3]) toestemming = FALSE;
    if ((fc4 != NG) && R[fc4] && A[fc4] && !PAR[fc4]) toestemming = FALSE;

    if (!toestemming)
    {
      if ((fc1 != NG) && !G[fc1]) PAR[fc1] = FALSE;
      if ((fc2 != NG) && !G[fc2]) PAR[fc2] = FALSE;
      if ((fc3 != NG) && !G[fc3]) PAR[fc3] = FALSE;
      if ((fc4 != NG) && !G[fc4]) PAR[fc4] = FALSE;
    }

    if (fc1 != NG)
    {
      if (RA[fc1] && (P[fc1] || !RR[fc1] && PAR[fc1])) realiseer = TRUE;
    }
    if (fc2 != NG)
    {
      if (RA[fc2] && (P[fc2] || !RR[fc2] && PAR[fc2])) realiseer = TRUE;
    }
    if (fc3 != NG)
    {
      if (RA[fc3] && (P[fc3] || !RR[fc3] && PAR[fc3])) realiseer = TRUE;
    }
    if (fc4 != NG)
    {
      if (RA[fc4] && (P[fc4] || !RR[fc4] && PAR[fc4])) realiseer = TRUE;
    }

    if (realiseer)
    {
      if (fc1 != NG) { if (R[fc1]) PAR[fc1] = TRUE; }
      if (fc2 != NG) { if (R[fc2]) PAR[fc2] = TRUE; }
      if (fc3 != NG) { if (R[fc3]) PAR[fc3] = TRUE; }
      if (fc4 != NG) { if (R[fc4]) PAR[fc4] = TRUE; }
    }
  }

  for (i = 0; i < FCMAX; ++i)         /* buffer PAR[] tbv onterechte alternatieve realisaties in prio_add */
  {
    PARtraffick[i] = PAR[i];
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie beeindig alternatieve realisatie                                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie beeindigt een alternatieve realisatie indien alternatieve ruimte niet meer aanwezig is.     */
/* De functie overschrijft RR[] BIT5 en FM[] BIT5 zoals bepaald door TLCgen.                                */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Alternatief_Add().                                                      */
/*                                                                                                          */
void BeeindigAltRealisatie(void)      /* Fik230101                                                          */
{
  count i,k;
  boolv fc_eerst[FCMAX];              /* richting is als eerstvolgende aan de beurt */

  for (i = 0; i < FCMAX; ++i)
  {
    RR[i] &= ~BIT5;                   /* reset BIT sturing */
    FM[i] &= ~BIT5;
    fc_eerst[i] = FALSE;              /* initialiseer hulp variabelen */
  }

  for (i = 0; i < FCMAX; ++i)
  {
    if (AR[i] &&  R[i] && !PAR[i]                             ) RR[i] |= BIT5;
    if (AR[i] && VG[i] && (TEG[i] == 0) && (AltRuimte[i] <= 0)) FM[i] |= BIT5;
  }

  for (i = 0; i < FCMAX; ++i)         /* bepaal welke richtingen als eerst volgende aan de beurt zijn */
  {
    fc_eerst[i] = R[i] && A[i] && AAPR[i] && !(AAPR[i]&BIT5) && !fkra(i) && !(RR[i]&BIT6) && !(RR[i]&BIT10) || RA[i];
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc1    = hki_kop[i].fc1;    /* voedende richting */
      count fc2    = hki_kop[i].fc2;    /* volg     richting */
      count status = hki_kop[i].status; /* status koppeling */

      if (!G[fc2] && fc_eerst[fc1] || (status == 1)) fc_eerst[fc2] = TRUE;
    }
  }

  for (i = 0; i < aantal_dcf_vst; ++i)
  {
    count fc1  = dcf_vst[i].fc1;      /* richting die voorstart geeft  */
    count fc2  = dcf_vst[i].fc2;      /* richting die voorstart krijgt */
    count ma21 = dcf_vst[i].ma21;     /* meerealisatie van fc2 met fc1 */

    boolv fc2_eerst = GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || SCH[ma21]);

    if (fc2_eerst && fc_eerst[fc1]) fc_eerst[fc2] = TRUE;
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc1      = hki_kop[i].fc1;      /* voedende richting */
      count fc2      = hki_kop[i].fc2;      /* volg     richting */
      count tnlfg12  = hki_kop[i].tnlfg12;  /* vaste   nalooptijd vanaf einde vastgroen      fc1 */
      count tnlfgd12 = hki_kop[i].tnlfgd12; /* det.afh.nalooptijd vanaf einde vastgroen      fc1 */
      count tnleg12  = hki_kop[i].tnleg12;  /* vaste   nalooptijd vanaf einde (verleng)groen fc1 */
      count tnlegd12 = hki_kop[i].tnlegd12; /* det.afh.nalooptijd vanaf einde (verleng)groen fc1 */
      mulv  kop_max  = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */

      if (fc_eerst[fc1])              /* voedende richting harde koppeling is aan de beurt, ga op zoek naar conflicten van de volgrichting die alternatief in VG[] staan */
      {
        for (k = 0; k < FCMAX; ++k)
        {
          mulv ontruim = GK_conflict(k,fc2);
          if (ontruim > NG)
          {
            if (AR[k] && VG[k] && (TEG[k] == 0))
            if (REALtraffick[fc2] <= ontruim) FM[k] |= BIT5;
          }
        }
      }
                                      /* voedende richting harde koppeling staat in alternatief in VG[], ga op zoek naar een conflict van de volgrichting die aan de beurt is */
      if (AR[fc1] && VG[fc1] && (TEG[fc1] == 0))
      {
        mulv naloop_tijd = 0;
        if ((tnlfg12  != NG)                                                       ) naloop_tijd = T_max[tnlfg12]  - T_timer[tnlfg12];
        if ((tnlfgd12 != NG) && (naloop_tijd < T_max[tnlfgd12] - T_timer[tnlfgd12])) naloop_tijd = T_max[tnlfgd12] - T_timer[tnlfgd12];
        if ((tnleg12  != NG) && (naloop_tijd < T_max[tnleg12]                     )) naloop_tijd = T_max[tnleg12];
        if ((tnlegd12 != NG) && (naloop_tijd < T_max[tnlegd12] + TGL_max[fc1]     )) naloop_tijd = T_max[tnlegd12] + TGL_max[fc1];

        naloop_tijd += kop_max;

        for (k = 0; k < FCMAX; ++k)
        {
          mulv ontruim = GK_conflict(fc2,k);
          if (ontruim > NG)
          {
            if (fc_eerst[k])          /* conflict volgrichting gevonden die aan de beurt is */
            {
              if (REALtraffick[k] <= ontruim + naloop_tijd) FM[fc1] |= BIT5;
            }
          }
          if (FM[fc1]&BIT5) break;
        }
      }
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer prio bit retourrood bij harde koppelingen                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCgen schrijft retourrood van volgrichting terug naar de voedende richting. Hier zit een bug omdat de   */
/* voeding ook retourrood krijgt als de volgrichting een conflicterende prioriteitsaanvraag heeft die geen  */
/* prioriteitsrealisatie gaat krijgen. Deze functie verzorgt de bugfix. (reset RR[] BIT10)                  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Modules_Add().                                                          */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void BugFix_RR_bij_HKI(void)          /* Fik230101                                                          */
{
  count i;

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc1 = hki_kop[i].fc1;     /* voedende richting */
      count fc2 = hki_kop[i].fc2;     /* volg     richting */

      if (!(RR[fc2]&PRIO_RR_BIT) && !REALconflict(fc1)) RR[fc1] &= ~BIT10;
    }
  }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie forceer realisatie van richtingen met voorstart                                                  */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie forceert de richting met een voorstart naar groen indien de hoofdrichting daarop wacht.     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_REA().                                                  */
/*                                                                                                          */
void Traffick2TLCgen_REA_VST(void)    /* Fik230101                                                          */
{
  count i;

  for (i = 0; i < aantal_dcf_vst; ++i)
  {
    count fc1   = dcf_vst[i].fc1;     /* richting die voorstart geeft  */
    count fc2   = dcf_vst[i].fc2;     /* richting die voorstart krijgt */
    count tvs21 = dcf_vst[i].tvs21;   /* voorstart fc2 */
    count ma21  = dcf_vst[i].ma21;    /* meerealisatie van fc2 met fc1 */

    BL[fc2] &= ~BIT4;                 /* overslag door fc1 agv afteller */
    if (SG[fc1] && (P[fc1]&BIT4) && (TA_timer[fc2] < T_max[tvs21] + 10)) BL[fc2] |= BIT4;

    if (RA[fc1] && (X[fc1]&BIT1) && !(X[fc1]&BIT3)) X[fc1] &= ~BIT1;
    if (RA[fc1] && A[fc1] && (!RR[fc1] || P[fc1]) && !BL[fc1] && (X[fc1]&BIT3) && R[fc2] && !TRG[fc2] && (A[fc2] || SCH[ma21]))
    {
      if (SCH[ma21]) A[fc2] |= BIT4;  /* zet mee aanvraag vroeg op */
      RR[fc2] = FALSE;
      X[fc2] &= ~BIT1;
      if (RV[fc2] && !AA[fc2]) AA[fc2] = AR[fc2] = TRUE;
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer realisatie afhandeling TLCgen                                                          */
/* -------------------------------------------------------------------------------------------------------- */
/* Voor TLCgen is alleen PAR[] onvoldoende om ook daadwerkelijk te realiseren (= overgang naar RA[]). Deze  */
/* functie corrigeert dit. Daarnaast zorgt de functie ervoor dat er nooit twee conflicterende richtingen in */
/* gewenst groen (= RA[] tot en met VG[]) komen. Indien nodig wordt AA[] hiervoor gereset of RR[] opgezet.  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit RealisatieAfhandeling_Add().                                            */
/*                                                                                                          */
void Traffick2TLCgen_REA(void)        /* Fik230101                                                          */
{
  count i,k;

  Traffick2TLCgen_REA_VST();

  for (i = 0; i < FCMAX; ++i)
  {
    if (RA[i] && !A[i])
    {
      RR[i] |= BIT3;
      TFB_timer[i] = 0;
    }
    if (RA[i] && tkcv(i)) RR[i] |= BIT3;
    if (RV[i] && tkcv(i)) AA[i] = FALSE;
    if (RA[i] &&   AR[i]) AG[i] = AR[i]; /* voorkom vasthouden module als alle richtingen gerealiseerd zijn */
  }

  for (i = 0; i < aantal_pel_kop; ++i)
  {
    count kop_fc = pel_kop[i].kop_fc;   /* koppelrichting */

    if (MG[kop_fc] && (YM[kop_fc]&BIT12))
    {
      for (k = 0; k < FCMAX; ++k)
      {
        if (FK_conflict(kop_fc,k))
        {
          RR[k] |= BIT3;
        }
      }
    }
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)         /* koppeling is ingeschakeld */
    {
      count fc1    = hki_kop[i].fc1;    /* voedende richting */
      count fc2    = hki_kop[i].fc2;    /* volg     richting */
      mulv  status = hki_kop[i].status; /* status koppeling  */

      if (G[fc1] && R[fc2] && A[fc2] && !BL[fc2] && (status == 1) && !tkcv(fc2) && !tfkaa(fc2))
      {
        RR[fc2] = FALSE;
        if (RV[fc2] && !AA[fc2]) AA[fc2] = AR[fc2] = TRUE;
      }
    }
  }

  for (i = 0; i < aantal_vtg_tgo; ++i)
  {
    count fc1      = vtg_tgo[i].fc1;      /* richting 1 */
    count fc2      = vtg_tgo[i].fc2;      /* richting 2 */
    mulv  status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
    mulv  status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

    if ((RA[fc1] && P[fc1] && (X[fc1]&BIT3) || G[fc1] && (status12 == 1)) && R[fc2] && !TRG[fc2] && A[fc2] && !tkcv(fc2) && !tfkaa(fc2))
    {
      RR[fc2] = FALSE;
      if (RV[fc2] && !AA[fc2]) AA[fc2] = AR[fc2] = TRUE;
    }

    if ((RA[fc2] && P[fc2] && (X[fc2]&BIT3) || G[fc2] && (status21 == 1)) && R[fc1] && !TRG[fc1] && A[fc1] && !tkcv(fc1) && !tfkaa(fc1))
    {
      RR[fc1] = FALSE;
      if (RV[fc1] && !AA[fc1]) AA[fc1] = AR[fc1] = TRUE;
    }

    if (RA[fc1] && (P[fc1]&BIT2)) X[fc1] &= ~BIT1; /* overrule TLCgen */
    if (RA[fc2] && (P[fc2]&BIT2)) X[fc2] &= ~BIT1; /* volg richting moet altijd voeding volgen */
  }

  for (i = 0; i < aantal_lvk_gst; ++i)
  {
    count fc1 = lvk_gst[i].fc1;       /* richting 1 */
    count fc2 = lvk_gst[i].fc2;       /* richting 2 */
    count fc3 = lvk_gst[i].fc3;       /* richting 3 */
    count fc4 = lvk_gst[i].fc4;       /* richting 4 */

    boolv realiseer = FALSE;
    if ((fc1 != NG) && RA[fc1] && (P[fc1] || !RR[fc1])) realiseer = TRUE;
    if ((fc2 != NG) && RA[fc2] && (P[fc2] || !RR[fc2])) realiseer = TRUE;
    if ((fc3 != NG) && RA[fc3] && (P[fc3] || !RR[fc3])) realiseer = TRUE;
    if ((fc4 != NG) && RA[fc4] && (P[fc4] || !RR[fc4])) realiseer = TRUE;

    if (realiseer)
    {
      if ((fc1 != NG) && R[fc1] && !TRG[fc1] && A[fc1] && !tkcv(fc1) && !tfkaa(fc1))
      {
        RR[fc1] = FALSE;
        if (RV[fc1] && !AA[fc1]) AA[fc1] = AR[fc1] = TRUE;
      }
      if ((fc2 != NG) && R[fc2] && !TRG[fc2] && A[fc2] && !tkcv(fc2) && !tfkaa(fc2))
      {
        RR[fc2] = FALSE;
        if (RV[fc2] && !AA[fc2]) AA[fc2] = AR[fc2] = TRUE;
      }
      if ((fc3 != NG) && R[fc3] && !TRG[fc3] && A[fc3] && !tkcv(fc3) && !tfkaa(fc3))
      {
        RR[fc3] = FALSE;
        if (RV[fc3] && !AA[fc3]) AA[fc3] = AR[fc3] = TRUE;
      }
      if ((fc4 != NG) && R[fc4] && !TRG[fc4] && A[fc4] && !tkcv(fc4) && !tfkaa(fc4))
      {
        RR[fc4] = FALSE;
        if (RV[fc4] && !AA[fc4]) AA[fc4] = AR[fc4] = TRUE;
      }
    }
  }

  for (i = 0; i < FCMAX; ++i)
  {
    if (RV[i] && !TRG[i] && A[i] && PAR[i] && !AA[i] && !RR[i] && !BL[i] && !tkcv(i) && !tfkaa(i))
    {
      boolv langstwachtend = TRUE;

      for (k = 0; k < FCMAX; ++k)
      {
        if (FK_conflict(i,k))
        {
          if (RV[k] && !TRG[k] && A[k] && PAR[k] && !RR[k] && !BL[k] && !tkcv(k) && !tfkaa(k))
          {
            if (GWT[k] > GWT[i]) langstwachtend = FALSE;
          }
        }
        if (!langstwachtend) break;
      }

      if (langstwachtend)
      {
        AA[i] = AR[i] = TRUE;
      }
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende OV ingreep tbv wachttijd voorspeller                      */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie geeft de aanwezigheid terug van een conflicterende busingreep tbv de wachttijd voorspeller. */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit aansturing_wt_voorspeller().                                            */
/*                                                                                                          */
boolv conflict_OV(                    /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count i,prio;

  for (i = 0; i < FCMAX; ++i)
  {
    if (GK_conflict(fc,i) > NG)
    {
      prio = prio_index[i].OV_kar;
      if ((prio > NG) && G[i] && iPrioriteit[prio]) return TRUE;

      prio = prio_index[i].OV_srm;
      if ((prio > NG) && G[i] && iPrioriteit[prio]) return TRUE;

      prio = prio_index[i].OV_verlos;
      if ((prio > NG) && G[i] && iPrioriteit[prio]) return TRUE;
    }
  }
  return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bereken tijd tot startgroen tbv wachttijd voorspeller                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie berekent de minimale tijd tot groen ervan uitgaande dat conflicten die groen zijn volledig  */
/* uitverlengen. Deze berekening is nodig omdat bij een conflicterende prioriteitsingreep REALtraffick[]    */
/* de waarde NG krijgt.                                                                                     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit aansturing_wt_voorspeller().                                            */
/*                                                                                                          */
mulv REALconflictTTG(                 /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count i;
  mulv  TijdTotGroen = MTG[fc];

  for (i = 0; i < FCMAX; ++i)
  {
    boolv ontruim = GK_conflict(fc,i);
    {
      if (G[i] && (ontruim > NG))
      {
        if (TEG[i] + ontruim > TijdTotGroen) TijdTotGroen = TEG[i] + ontruim;
      }
    }
  }
  return TijdTotGroen;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie aansturing wachttijd voorspeller                                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de aansturing van de wachttijd voorspellers. De wachttijd voorspeller halteert     */
/* altijd tijdens fixatie of een conflicterende hulpdienst ingreep. Indien het bussjabloon wordt toegepast  */
/* halteert de wachttijdvoorspeller ook bij een conflicterend OV ingreep.                                   */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void aansturing_wt_voorspeller(       /* Fik230101                                                          */
count fc,                             /* FC fasecyclus                                                      */
count us0,                            /* US wachttijd voorspeller - BIT0                                    */
count us1,                            /* US wachttijd voorspeller - BIT1                                    */
count us2,                            /* US wachttijd voorspeller - BIT2                                    */
count us3,                            /* US wachttijd voorspeller - BIT3                                    */
count us4,                            /* US wachttijd voorspeller - BIT4                                    */
count usbus)                          /* US aansturing bus sjabloon                                         */
{
  boolv halteer      =  FALSE;
  boolv bus_sjb      =  FALSE;
  boolv knipper      =  FALSE;
  mulv  wachttijd    =  wacht_ML[fc];
  boolv bus_sjb_aanw = (usbus != NG);
  mulv  aantal_leds  = 0;

  if (wachttijd == -3)                /* primaire overslag geeft -3 terug */
  {
    wachttijd = 1200 - TA_timer[fc];
    if (wachttijd < 600) wachttijd = 600;
  }

#ifdef schsjabloon
  if (!SCH[schsjabloon]) bus_sjb_aanw = FALSE;
#endif

  if (conflict_OV(fc))
  {
    if (bus_sjb_aanw)
    {
      halteer = TRUE;
      bus_sjb = TRUE;
    }
    else
    {
#ifdef schhaltwtv
      if (SCH[schhaltwtv]) halteer = TRUE;
#endif
    }
  }

  if (BL[fc] || khlpd(fc))
  {
    halteer = TRUE;
    bus_sjb = FALSE;
  }
#ifdef isfix
  if (CIF_IS[isfix])
  {
    halteer = TRUE;
    bus_sjb = FALSE;
  }
#endif

  if (halteer && !bus_sjb)
  {
#ifdef schknipper
    if (SCH[schknipper]) knipper = TRUE;
#endif
  }

  if (REALtraffick[fc] > NG) wachttijd = REALtraffick[fc];
  else
  {
    if (REALconflictTTG(fc) > wachttijd) wachttijd = REALconflictTTG(fc);
  }

  if (Aled[fc]       > 0  ) AanDuurLed[fc] += TE;
  if (AanDuurLed[fc] > 600) AanDuurLed[fc] = 600;

  if ((Aled[fc] > 0) && (wachttijd > NG) && !halteer)
  {
    mulv rest      = (wachttijd + AanDuurLed[fc]) % Aled[fc];
    TijdPerLed[fc] = (wachttijd + AanDuurLed[fc]) / Aled[fc];

    if (2 * rest >= Aled[fc]) TijdPerLed[fc]++;
    if (TijdPerLed[fc] <   1) TijdPerLed[fc] = 1;
    if (TijdPerLed[fc] > 600) TijdPerLed[fc] = 600;

    if (AanDuurLed[fc] >= TijdPerLed[fc])
    {
      if (Aled[fc] > 1) Aled[fc]--;
      AanDuurLed[fc] = 0;
    }
  }

  if (G[fc] || GL[fc]) Aled[fc] = 0;                        /* er is een definitieve (rgv)detectie aanvraag */
  if (R[fc] && (Aled[fc] == 0) && ((A[fc]&BIT0) || (A[fc]&BIT1)))
  {
    if ((REALtraffick[fc] > 3) || (TA_timer[fc] > 3))       /* gedoofd houden als richting (nagenoeg)direct */
    {                                                       /* ... groen kan worden                         */
      Aled[fc]       = 31;
      AanDuurLed[fc] =  0;                                  /* actuele duur uitsturing van Aled[] leds      */
    }
  }

  aantal_leds = Aled[fc];
  if (knipper && (CIF_KLOK[CIF_TSEC_TELLER]%10 > 4) && (Aled[fc] > 1)) aantal_leds--;

  if (REG)
  {
    CIF_GUS[us4] = (boolv)((aantal_leds&BIT4) > 0);
    CIF_GUS[us3] = (boolv)((aantal_leds&BIT3) > 0);
    CIF_GUS[us2] = (boolv)((aantal_leds&BIT2) > 0);
    CIF_GUS[us1] = (boolv)((aantal_leds&BIT1) > 0);
    CIF_GUS[us0] = (boolv)((aantal_leds&BIT0) > 0);
    if (usbus != NG) CIF_GUS[usbus] = (Aled[fc] > 0) && bus_sjb;
  }
  else
  {
    CIF_GUS[us4] = FALSE;
    CIF_GUS[us3] = FALSE;
    CIF_GUS[us2] = FALSE;
    CIF_GUS[us1] = FALSE;
    CIF_GUS[us0] = FALSE;
    if (usbus != NG) CIF_GUS[usbus]= FALSE;
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie aansturing afteller                                                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de aansturing van de aftellers.                                                    */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void aansturing_aftellers(void)       /* Fik230101                                                          */
{
  count i;

  for (i = 0; i < aantal_aft_123; ++i)
  {
    count fc       = aft_123[i].fc;       /* FC    richting met afteller                                    */
    count de1      = aft_123[i].de1;      /* DE    koplus 1                                                 */
    count de2      = aft_123[i].de2;      /* DE    koplus 2                                                 */
    count de3      = aft_123[i].de3;      /* DE    koplus 3                                                 */
    count toest    = aft_123[i].toest;    /* SCH   toestemming aansturing afteller                          */
    count min_duur = aft_123[i].min_duur; /* PRM   min.duur tot start groen waarbij afteller mag starten    */
    count tel_duur = aft_123[i].tel_duur; /* PRM   duur van een tel in tienden van seconden                 */
    count us_getal = aft_123[i].us_getal; /* US    tbv verklikking op bedienpaneel                          */
    count us_bit0  = aft_123[i].us_bit0;  /* US    aansturing afteller BIT0                                 */
    count us_bit1  = aft_123[i].us_bit1;  /* US    aansturing afteller BIT1                                 */
    boolv aftel_ok = aft_123[i].aftel_ok; /* boolv alle aftellers van een rijrichting zijn OK               */
    mulv  act_duur = aft_123[i].act_duur; /* mulv  tijd in tienden van seconden dat TEL wordt aangestuurd   */

    mulv  teller_waarde = 0;              /* mulv  actuele stand afteller */

    if ((fc != NG) && (tel_duur != NG) && (us_bit0 != NG) && (us_bit1 != NG))         /* definitie is juist */
    {
      if (G[fc])                          /* richting is groen - afteller dooft */
      {
        P[fc]   &= ~BIT4;
        Waft[fc] = act_duur = 0;
        if (us_getal != NG) CIF_GUS[us_getal] = FALSE;
        CIF_GUS[us_bit0] = FALSE;
        CIF_GUS[us_bit1] = FALSE;
      }
      else
      {
        if (CIF_GUS[us_bit0]) teller_waarde  = 1;
        if (CIF_GUS[us_bit1]) teller_waarde += 2;
      }

      if (teller_waarde > 0)              /* afteller is in werking */
      {
        P[fc] |= BIT4;                    /* richting mag NOOIT retourrood */
        X[fc] |= BIT4;                    /* ... maar moet wel wachten tot aftellen gereed is */
        BL[fc] = FALSE;

        act_duur += TE;                   /* bijwerken actuele TEL duur */
        Waft[fc] -= TE;                   /* bijwerken tijd tot groen */

        if (act_duur >= PRM[tel_duur])    /* volgende TEL actief maken */
        {
          teller_waarde--;
          if (teller_waarde > 0) act_duur = 0; /* TEL duur resetten */
        }
        if ((teller_waarde == 0) && (act_duur >= PRM[tel_duur]))
        {
          X[fc] = Waft[fc] = FALSE;       /* einde uitstel - richting moet naar groen */
          if (K[fc]) 
          {
            teller_waarde = Waft[fc] = 1; /* corrigeer teller_waarde als er nog een ontruimingstijd loopt */
          }
        }
      }
      else                                /* afteller is nog niet in werking */
      {
        boolv afteller_mag = RA[fc] && !RR[fc] && !BL[fc] && aftel_ok;
        if ((toest != NG) && !SCH[toest]          ) afteller_mag = FALSE;
        if ((de1   != NG) && (DF[de1] || !DB[de1])) afteller_mag = FALSE;
        if ((de2   != NG) && (DF[de2] || !DB[de2])) afteller_mag = FALSE;
        if ((de3   != NG) && (DF[de3] || !DB[de3])) afteller_mag = FALSE;
        if (voorstart_gever(fc))
        {
          if (REALtraffick[fc] >= 3 * PRM[tel_duur] - 2) afteller_mag = FALSE;
        }
        else
        {
          if (REALtraffick[fc] >= 3 * PRM[tel_duur] - 1) afteller_mag = FALSE;
        }
        if ((min_duur != NG) && (REALtraffick[fc] < PRM[min_duur])) afteller_mag = FALSE;

        if (afteller_mag)                 /* start de afteller */
        {
          teller_waarde = 3;
          Waft[fc] = 3 * PRM[tel_duur];   /* tijd tot groen agv afteller */
          X[fc] |= BIT4;                  /* uitstellen tot aftellen gereed */
        }
      }
    }
                                          /* stuur US signalen aan */
    if (us_getal != NG) CIF_GUS[us_getal] = teller_waarde;
    CIF_GUS[us_bit0] = (boolv)((teller_waarde&BIT0) > 0);
    CIF_GUS[us_bit1] = (boolv)((teller_waarde&BIT1) > 0);

    aft_123[i].act_duur = act_duur;       /* bijwerken struct */
  }

  for (i = 0; i < aantal_dcf_vst; ++i)
  {
    count fc1 = dcf_vst[i].fc1;           /* richting die voorstart geeft  */
    count fc2 = dcf_vst[i].fc2;           /* richting die voorstart krijgt */
    count tvs21 = dcf_vst[i].tvs21;       /* voorstart fc2 */

    if (RA[fc1] && (P[fc1]&BIT4) && RA[fc2])
    {
      RR[fc2] = BL[fc2] = FALSE;
      if (Waft[fc1] <= T_max[tvs21] + 1) X[fc2] = FALSE;
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie aansturing rateltikkers vanuit applicatie                                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de aansturing van rateltikkers waarbij de uitgang van de rateltikker HOOG is       */
/* zolang de rateltikker geluid moet maken. De werking wordt geconfigureerd d.m.v. een werkingsperiode en   */
/* een werkingsparameter. De werkingsparameter kent de volgende instel mogelijkheden:                       */
/*  0 = uitgeschakeld                                                                                       */
/*  1 = continu                                                                                             */
/*  2 = op aanvraag                                                                                         */
/*  3 = tijdens ingestelde periode continu                                                                  */
/*  4 = tijdens ingestelde periode op aanvraag                                                              */
/* >= 5 tijdens ingestelde periode continu, daarbuiten op aanvraag                                          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void rateltikker_applicatie(          /* Fik230101                                                          */
count fc,                             /* FC    fasecyclus                                                   */
count hedrk1,                         /* HE    drukknop 1                                                   */
count hedrk2,                         /* HE    drukknop 2                                                   */
count usrat,                          /* US    rateltikker                                                  */
count natik,                          /* TM    natikken vanaf start rood                                    */
boolv ratel_periode,                  /* boolv werkingsperiode  rateltikkers                                */
mulv  ratel_werking)                  /* mulv  werkingsparmeter rateltikkers                                */
{
  count i;

  boolv continu  = (ratel_werking == 1) ||  ratel_periode && ((ratel_werking == 3) || (ratel_werking >= 5));
  boolv aanvraag = (ratel_werking == 2) ||  ratel_periode &&  (ratel_werking == 4)
                                        || !ratel_periode &&  (ratel_werking >= 5);
  boolv drukknop = FALSE;
  if (hedrk1 != NG) drukknop |= IH[hedrk1];
  if (hedrk2 != NG) drukknop |= IH[hedrk2];

  X[fc] &= ~BIT7;                     /* reset ratel uitstel bit */

  if (SR[fc] || !continu && !aanvraag && R[fc]    ) RAT[fc] = FALSE;
  if ( R[fc] && (continu ||  aanvraag && drukknop)) RAT[fc] = TRUE;

  if (R[fc] && !continu && !aanvraag && CIF_GUS[usrat]   ) X[fc]  |= BIT7;
  if (R[fc] && !continu && !T[natik] && RAT[fc] && !A[fc]) RAT[fc] = FALSE;

  RT[natik] = (G[fc] || GL[fc]) && RAT[fc];
  AT[natik] = R[fc] && !TRG[fc] && !continu && !aanvraag;

  if (REG)
  {
    CIF_GUS[usrat] = RAT[fc] || T[natik];
  }
  else
  {
    CIF_GUS[usrat] = FALSE;
  }

  for (i = 0; i < aantal_vtg_tgo; ++i)
  {
    count fc1      = vtg_tgo[i].fc1;      /* richting 1 */
    count fc2      = vtg_tgo[i].fc2;      /* richting 2 */
    count hnla12   = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
    count hnla21   = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
    mulv  status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
    mulv  status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

    if (fc == fc1)
    {
      if (SG[fc2] && G[fc1] && IH[hnla21] || SG[fc1] && (status21 == 1))
      {
        RAT[fc1] = TRUE;
        if (REG) CIF_GUS[usrat] = TRUE;
      }
    }

    if (fc == fc2)
    {
      if (SG[fc1] && G[fc2] && IH[hnla12] || SG[fc2] && (status12 == 1))
      {
        RAT[fc2] = TRUE;
        if (REG) CIF_GUS[usrat] = TRUE;
      }
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer wachtlicht aansturing voor fietsers                                                    */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCgen stuurt het wachtlicht alleen aan na bediening van de betreffende drukknop. In deze functie        */
/* wordt dit tijdens rood gecorrigeerd zodat het wachtlicht bij iedere (definitieve) detectie aanvraag      */
/* wordt aangestuurd.                                                                                       */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void corrigeer_wachtlicht_fiets(      /* Fik230101                                                          */
count fc,                             /* FC fasecyclus                                                      */
count uswacht)                        /* US wachtlicht uitsturing                                           */
{
  if (R[fc] && REG)
  {
    CIF_GUS[uswacht] = (boolv) (A[fc] & (BIT0 | BIT1));
    if (RV[fc] && !A[fc]) CIF_GUS[uswacht] = FALSE;     /* aanvraag kwijt geraakt */
  }
  if (!REG) CIF_GUS[uswacht] = FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie uitgaande pelotonkoppeling obv getelde voertuigen ter hoogte van de stopstreep                   */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de uitsturing van een uitgaand peloton signaal obv getelde voertuigen nabij de     */
/* stopstreep. Indien het koppelsignaal als puls is gedefinieerd wordt het bijbehorende koppelsignaal       */
/* gedurende 2,0 sec. uitgestuurd waarna de meting herstart. Bij een duursignaal wordt het koppelsignaal    */
/* uitgestuurd zolang het meetkriterium MK[] waar is tijdens de groenfase, na het afvallen van MK[] wordt   */
/* de meting herstart.                                                                                      */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_application().                                                     */
/*                                                                                                          */
void peloton_meting_uit_stopstreep(   /* Fik230101                                                          */
count fc,                             /* FC    fasecyclus                                                   */
count de1,                            /* DE    koplus 1                                                     */
count de2,                            /* DE    koplus 2                                                     */
count de3,                            /* DE    koplus 3                                                     */
count tpelmeet,                       /* TM    meetperiode peloton koppeling                                */
count tpeltdh,                        /* TM    grenshiaat  peloton meting                                   */
count pgrenswaarde,                   /* PRM   grenswaarde aantal voertuigen                                */
boolv duur_signaal,                   /* boolv FALSE = puls                                                 */
count us_ks)                          /* US    uitgaand koppelsignaal                                       */
{
  boolv ED1,ED2,ED3;

  ED1 = (de1 != NG) && ED[de1] && !DF[de1] && !R[fc]; /* bepaal uitgaande detectie pulsen */
  ED2 = (de2 != NG) && ED[de2] && !DF[de2] && !R[fc];
  ED3 = (de3 != NG) && ED[de3] && !DF[de3] && !R[fc];

  AT[tpelmeet] = FALSE;               /* reset AT[] */
  AT[tpeltdh]  = FALSE;               /* ... en start meetperiode bij vertrek van 1e voertuig */

  if (duur_signaal)
  {
    RT[tpelmeet] = RT[tpeltdh] = SG[fc] || G[fc] && !T[tpelmeet] && !T[tpeltdh] && (ED1 || ED2 || ED3) && !CIF_GUS[us_ks];
  }
  else                                /* koppelsignaal is puls */
  {
    RT[tpelmeet] = RT[tpeltdh] = SG[fc] || G[fc] && !T[tpelmeet] && !T[tpeltdh] && (ED1 || ED2 || ED3);
  }

  if ((ED1 || ED2 || ED3) && (RT[tpelmeet] || T[tpelmeet]))
  {
    if (ED1 && PEL_UIT_VTG[fc] < 10000) PEL_UIT_VTG[fc] += 1;
    if (ED2 && PEL_UIT_VTG[fc] < 10000) PEL_UIT_VTG[fc] += 1;
    if (ED3 && PEL_UIT_VTG[fc] < 10000) PEL_UIT_VTG[fc] += 1;
    RT[tpeltdh] = TRUE;               /* herstart grenshiaat er is immers een voertuig afgereden */
  }

  if (ET[tpeltdh] && !RT[tpeltdh])    /* reset meting omdat het grenshiaat is gevallen */
  {
     AT[tpelmeet]    = TRUE;
     PEL_UIT_VTG[fc] = 0;
  }

  if (PEL_UIT_RES[fc] > 0)            /* hou restant duur uitsturing koppelsignaal bij */
  {
    if (PEL_UIT_RES[fc] >= TE) PEL_UIT_RES[fc] -= TE;
    else                       PEL_UIT_RES[fc]  = 0;
  }

  if ((PRM[pgrenswaarde] > 0) && (PEL_UIT_VTG[fc] >= PRM[pgrenswaarde]))
  {
     PEL_UIT_VTG[fc] = 0;             /* peloton is gemeten */
     PEL_UIT_RES[fc] = 20;            /* ... stuur koppelsignaal uit en reset de meting */
     AT[tpelmeet]    = TRUE;
     AT[tpeltdh]     = TRUE;
     RT[tpeltdh]     = FALSE;         /* ook bij een duur signaal is de minimale uitsturing 2,0 sec. */
  }
  CIF_GUS[us_ks] = REG && ((PEL_UIT_RES[fc] > 0) || CIF_GUS[us_ks] && G[fc] && (VS[fc] || FG[fc] || MK[fc]));
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie prioriteitsafhandeling fiets voorrang module                                                     */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de prioriteitsafhandeling van de fiets voorrang module. Indien de fietsrichting    */
/* geblokkeerd wordt of indien de wachttijd te hoog oploopt kan de "uitmelding" tijdens rood plaatsvinden.  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit InUitMelden_Add().                                                      */
/*                                                                                                          */
void fiets_voorrang_module(void)      /* Fik230101                                                          */
{
  count i;

  for (i = 0; i < aantal_fts_pri; ++i)
  {
    count fc       = fts_pri[i].fc;       /* fietsrichting */
    count inmeld   = fts_pri[i].inmeld;   /* hulp element voor prioriteitsmodule (in.melding prioriteit) */
    count uitmeld  = fts_pri[i].uitmeld;  /* hulp element voor prioriteitsmodule (uitmelding prioriteit) */
    boolv aanvraag = fts_pri[i].aanvraag; /* fietser is op juiste wijze aangevraagd */
    boolv prio_vw  = fts_pri[i].prio_vw;  /* fietser voldoet aan prioriteitsvoorwaarden */
    boolv prio_av  = fts_pri[i].prio_av;  /* fietser is met prioriteit aangevraagd */

    if ((inmeld != NG) && (uitmeld != NG))
    {
      IH[inmeld] = IH[uitmeld] = FALSE;   /* reset hulp elementen */

      if (R[fc] && !TRG[fc] && prio_vw && !prio_av && !WT_TE_HOOG && !GEEN_FIETS_PRIO)
      {
        IH[inmeld] = TRUE;
        prio_av    = TRUE;
      }
      if (prio_av && (G[fc] && !SG[fc] || R[fc] && !prio_vw))
      {
        IH[uitmeld] = TRUE;
        prio_av     = FALSE;
      }

      fts_pri[i].prio_av = prio_av; /* bijwerken struct */
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verklik fiets voorrang module                                                                    */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de verklikking van de fiets voorrang module. Het led knippert tijdens rood indien  */
/* een prioriteitsaavraag aanwezig is en brandt vervolgens vast tot einde vastgroen van de fietsrichting.   */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void verklik_fiets_voorrang(void)     /* Fik230101                                                          */
{
  count i;

  for (i = 0; i < aantal_fts_pri; ++i)
  {
    count fc      = fts_pri[i].fc;      /* fietsrichting */
    boolv prio_av = fts_pri[i].prio_av; /* fietser is met prioriteit aangevraagd */
    count verklik = fts_pri[i].verklik; /* verklik fiets prioriteit */

    if (verklik > NG)
    {
      CIF_GUS[verklik] = REG && (CIF_GUS[verklik] && (VS[fc] || FG[fc]) || prio_av && (R[fc] && KNIP || SG[fc]));
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verlosmelding busbaan met prioriteit                                                             */
/* -------------------------------------------------------------------------------------------------------- */
/* Door in TLCgen een BUS prioriteit te definieren zonder in- en uitmelding kan met behulp van de detectie  */
/* nabij de stopstreep op basis van massa detectie prioriteit worden aangevraagd. Indien twee lussen in de  */
/* aanroep worden meegegeven wordt uitgegaan van een lengte gevoelige melding anders wordt prioriteit       */
/* aangevaagd op basis van bezettijd.                                                                       */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit InUitMelden_Add().                                                      */
/*                                                                                                          */
void verlos_melding_busbaan(          /* Fik230101                                                          */
count fc,                             /* FC   fasecyclus                                                    */
count de1,                            /* DE   koplus nabij de stopstreep                                    */
count de2,                            /* DE   koplus tbv lengte gevoeligheid (optioneel)                    */
count hinm,                           /* HE   puls tbv in.melding (wordt door TLCgen gegenereerd)           */
count huitm,                          /* HE   puls tbv uitmelding (wordt door TLCgen gegenereerd)           */
mulv  min_rood)                       /* mulv minimale roodtijd (TE) voor prioriteit aanvraag               */
{
  IH[hinm] = IH[huitm] = FALSE;       /* reset hulp elementen */

  if (de2 == NG)                      /* inmelding */
  {
    if (DB[de1] && (CIF_IS[de1] < CIF_DET_STORING)) verlos_busbaan[fc] = 1;
  }
  else
  {
    if (SD[de1] && (CIF_IS[de1] < CIF_DET_STORING) &&
         D[de2] && (CIF_IS[de2] < CIF_DET_STORING)) verlos_busbaan[fc] = 1;
  }

  if (ED[de1])
  {
    IH[huitm] = TRUE;                 /* uitmelding */
    verlos_busbaan[fc] = 0;           /* een uitmelding teveel is in dit geval nooit een probleem */
  }

  if (R[fc] && (TR_timer[fc] >= min_rood) && (verlos_busbaan[fc] == 1))
  {
    IH[hinm] = TRUE;                  /* uitmelding */
    verlos_busbaan[fc] = 2;           /* gelijk aan 2 maken voorkomt altijd dubbele aanmeldingen */
  }

  if (EG[fc]) verlos_busbaan[fc] = 0; /* reset verlos_baan[] bij gemiste uitmelding */
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer blokkeringstijd BUS prioriteit                                                         */
/* -------------------------------------------------------------------------------------------------------- */
/* In TLCgen heeft iedere prioriteit zijn eigen blokkeringstijd. In de iVRI kan een BUS ingreep starten op  */
/* basis van KAR en SRM. Daarnaast geldt voor busbanen ook nog vaak een mogelijkheid voor prioriteit op     */
/* basis van verlos detectie.                                                                               */
/*                                                                                                          */
/* Deze functie herstart bij een BUS ingreep alle blokkeringstijden zodat een "onterechte" herhaling van de */
/* ingreep wordt voorkomen.                                                                                 */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit BlokkeringsTijd_Add().                                                  */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void corrigeer_blokkeringstijd_OV(void) /* Fik230101                                                        */
{
  count i;

  for (i = 0; i < FCMAX; ++i)
  {
    boolv herstart = FALSE;

    if ((prio_index[i].OV_kar != NG) &&
        (iBlokkeringsTimer[prio_index[i].OV_kar] == 0) &&
        ( iBlokkeringsTijd[prio_index[i].OV_kar] >  0)) herstart = TRUE;

    if ((prio_index[i].OV_srm != NG) &&
        (iBlokkeringsTimer[prio_index[i].OV_srm] == 0) &&
        ( iBlokkeringsTijd[prio_index[i].OV_srm] >  0)) herstart = TRUE;

    if ((prio_index[i].OV_verlos != NG) &&
        (iBlokkeringsTimer[prio_index[i].OV_verlos] == 0) &&
        ( iBlokkeringsTijd[prio_index[i].OV_verlos] >  0)) herstart = TRUE;

    if (herstart)
    {
      if (prio_index[i].OV_kar    != NG) iBlokkeringsTimer[prio_index[i].OV_kar]    = 0;
      if (prio_index[i].OV_srm    != NG) iBlokkeringsTimer[prio_index[i].OV_srm]    = 0;
      if (prio_index[i].OV_verlos != NG) iBlokkeringsTimer[prio_index[i].OV_verlos] = 0;
    }
  }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer maximum wachttijd voor BUS prioriteit                                                  */
/* -------------------------------------------------------------------------------------------------------- */
/* In TLCgen wordt prioriteit geblokkeerd als een conflictrichting een te hoge wachttijd heeft. In Traffick */
/* is dit ook het geval indien een niet conflicterende richting een te hoge wachttijd heeft.                */
/*                                                                                                          */
/* Deze functie zorgt voor toepassing van de Traffick methodiek in plaats van de TLCgen methodiek.          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit WachtTijdBewaking_Add().                                                */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void corrigeer_maximum_wachttijd_OV(void) /* Fik230101                                                      */
{
  count prio;

  if (WT_TE_HOOG)
  {
    for (prio = 0; prio < FCMAX; ++prio)
    {
      iMaximumWachtTijdOverschreden[prio] = TRUE;
    }
  }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal hulpdienst ingreep                                                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de hulpdienst ingreep op alle richtingen van de gedefinieerde kruispuntarm en      */
/* eventuele gedefinieerde volgarmen.                                                                       */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_PRIO_OPTIES().                                          */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void Traffick2TLCgen_HLPD(void)       /* Fik230101                                                          */
{
  count i,j,k,x,prio;
  mulv  grw_leds = 5;                 /* grens aantal leds wt_voorspeller voor conflicterende prioriteit    */
  boolv HLPD_prio_mogelijk = FALSE;

#ifdef prmwtv
  grw_leds = PRM[prmwtv];             /* neem instelling parameter over indien die is gedefinieerd          */
#endif

  for (i = 0; i < FCMAX; ++i)
  {
    if (NAL_HLPD[i] > TE) NAL_HLPD[i] -= TE;                    /* aftellen naloop hulpdienst ingreep */
    else                  NAL_HLPD[i]  = 0;

    if (!HD_aanwezig[i] && (NAL_HLPD[i] == 0)) HLPD[i] = FALSE; /* reset hulpdienst ingreep aktief */
  }

  for (i = 0; i < FCMAX; ++i)
  {                                     /* richting heeft een hulpdienst voertuig in het traject */
    if (HD_aanwezig[i] && (ARM[i] >= 0))
    {
      HLPD_prio_mogelijk = TRUE;
      for (j = 0; j < FCMAX; ++j)       /* controleer voor alle richtingen op dezelfde arm en volgarm of HLPD prioriteit verstrekt kan worden */
      {
        if ((ARM[j] >= 0) && ((ARM[j] == ARM[i]) || (ARM[j] == volg_ARM[i])))
        {
          for (k = 0; k < FCMAX; ++k)
          {
            if (FK_conflict(j,k) && (HLPD[k] || (Aled[k] > 0) && (RA[k] || (Aled[k] < grw_leds))))
            {
              HLPD_prio_mogelijk = FALSE;
            }
            if (!HLPD_prio_mogelijk) break;
          }
                                      /* heeft een gevonden volgarm zelf ook weer een volgarm (= doorkoppeling over drie richtingen) */
          if (HLPD_prio_mogelijk && (ARM[j] == volg_ARM[i]))
          {
            for (x = 0; x < FCMAX; ++x)
            {
              if ((x != i) && (x != j) && (ARM[x] >= 0) && (ARM[x] == volg_ARM[j]))
              {
                for (k = 0; k < FCMAX; ++k)
                {
                  if (FK_conflict(x,k) && (HLPD[k] || (Aled[k] > 0) && (RA[k] || (Aled[k] < grw_leds))))
                  {
                    HLPD_prio_mogelijk = FALSE;
                  }
                  if (!HLPD_prio_mogelijk) break;
                }
              }
              if (!HLPD_prio_mogelijk) break;
            }
          }
        }
        if (!HLPD_prio_mogelijk) break;
      }

      if (HLPD_prio_mogelijk)         /* aan HD_aanwezig[i] en diens volgrichtingen kan HLPD prioriteit worden toegekend */
      {
        HLPD[i] = TRUE;
        for (j = 0; j < FCMAX; ++j)
        {
          if ((i != j) && (ARM[j] >= 0) && ((ARM[j] == ARM[i]) || (ARM[j] == volg_ARM[i])))
          {
            HLPD[j] = TRUE;
            for (x = 0; x < FCMAX; ++x) /* controleer of er een doorkoppeling is op een derde richting */
            {
              if ((ARM[x] >= 0) && (ARM[x] == volg_ARM[j])) HLPD[x] = TRUE;
            }
          }
        }
      }
    }
  }

  for (i = 0; i < FCMAX; ++i) /* zet prioriteit opties UIT indien nog geen prioriteit mogelijk */
  {
    if (!HLPD[i])             /* geen HLPD dan is hulpdienst prioriteit (nog) niet mogelijk */
    {
      count prio_hd_index = prio_index[i].HD;
      if (prio_hd_index > NG) iPrioriteitsOpties[prio_hd_index] = poGeenPrioriteit;
    }
    else                      /* wel hulpdienst prioriteit, dan geen conflicterende prioriteit mogelijk */
    {
      for (k = 0; k < FCMAX; ++k)
      {
        if (FK_conflict(i,k))
        {
          for (prio = 0; prio < prioFCMAX; ++prio)
          {
            if (iFC_PRIOix[prio] == k)
            {
              iPrioriteitsOpties[prio] = poGeenPrioriteit;
            }
          }
        }
      }
    }
  }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal naloop hulpdienst ingreep op volgarm                                                      */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de nalooptijden van een hulpdienst ingreep op alle richtingen van de gedefinieerde */
/* volgarm. De functie controleert of de volgrichting ook gedefinieerd is als volg_ARM[]. Als dit het geval */
/* is wordt de nalooptijd doorgezet op alle richtingen van de volg_ARM.                                     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PrioriteitsOpties_Add() na aanroep Traffick2TLCgen_PRIO_OPTIES().       */
/*                                                                                                          */
void Traffick2TLCgen_HLPD_nal(        /* Fik230101                                                          */
count fc1,                            /* FC   fasecyclus voedende richting                                  */
count fc2,                            /* FC   fasecyclus volg     richting                                  */
mulv  naloop)                         /* mulv nalooptijd                                                    */
{
  count i;
  boolv correct = HLPD[fc1] && (volg_ARM[fc1] == ARM[fc2]);

  if (correct)                        /* ingreep op voeding aktief en de gedefinieerde armen zijn correct */
  {
    for (i = 0; i < FCMAX; ++i)
    {
      if (ARM[i] == ARM[fc2])         /* richting gevonden op de volgarm */
      {
        if (NAL_HLPD[i] < naloop) NAL_HLPD[i] = naloop;
      }
    }
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal peloton ingreep                                                                           */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie blokkeert conflicterende prioriteitsingrepen bij een aktieve peloton ingreep.               */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_PRIO_OPTIES().                                          */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void Traffick2TLCgen_PELOTON(void)    /* Fik230101                                                          */
{
  count i,k,prio;

  for (i = 0; i < aantal_pel_kop; ++i)
  {
    count kop_fc    = pel_kop[i].kop_fc;    /* koppelrichting */
    mulv  pk_status = pel_kop[i].pk_status; /* status peloton ingreep */

    if (G[kop_fc] && ((RW[kop_fc]&BIT12) || (YV[kop_fc]&BIT12) || (YM[kop_fc]&BIT12)) && !Z[kop_fc] && (pk_status >= 3))
    {
      for (k = 0; k < FCMAX; ++k)
      {
        if (FK_conflict(kop_fc,k))
        {
          for (prio = 0; prio < prioFCMAX; ++prio)
          {
            if (iFC_PRIOix[prio] == k)
            {
              if (!(iPrioriteitsOpties[prio]&poNoodDienst)) iPrioriteitsOpties[prio] = poGeenPrioriteit;
            }
          }
        }
      }
    }
  }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal prioriteitsopties fiets voorrang module                                                   */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie zet de prioriteitsopties voor de fiets voorrang module conform de instellingen afhankelijk  */
/* van de status van de regensensor. De functie corrigeert de opties zodra een conflicterende prioriteit    */
/* is aangevraagd met als optie bijzondere realisatie. De fietsprioriteit is hieraan altijd ondergeschikt.  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_PRIO_OPTIES().                                          */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void Traffick2TLCgen_FIETS(void)      /* Fik230101                                                          */
{
  count i;

  for (i = 0; i < aantal_fts_pri; ++i)
  {
    count fc       = fts_pri[i].fc;       /* fietsrichting */
    count prio_fts = fts_pri[i].prio_fts; /* prioriteitscode */
    count prio_reg = fts_pri[i].prio_reg; /* prioriteitscode */
    boolv prio_av  = fts_pri[i].prio_av;  /* fietser is met prioriteit aangevraagd */

    count prio_fts_index = prio_index[fc].FTS;

    if (prio_av && prio_fts_index != NG)
    {
      if ((prio_fts != NG) && (!REGEN || (prio_reg == NG)))
      {
        iInstPrioriteitsOpties[prio_fts_index] = BepaalPrioriteitsOpties(prio_fts);
      }
      if ((prio_reg != NG) && (REGEN || (prio_fts == NG)))
      {
        iInstPrioriteitsOpties[prio_fts_index] = BepaalPrioriteitsOpties(prio_reg);
      }

      /* prioriteitsaanvraag nooit intrekken tijdens RA[] (bijvoorbeeld als REGEN afvalt tijdens RA) */
      if (RA[fc]) iPrioriteitsOpties[prio_fts_index] |= poBijzonderRealiseren;
      iPrioriteitsOpties[prio_fts_index] &= ~poGroenVastHouden;
    }
  }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer prioriteitsopties                                                                      */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie corrigeert prioriteitsopties als gevolg van verschillende (en onderling conflicterende)     */
/* prioriteitsingrepen en voor wachttijdvoorspellers waarvan minder dan een instelbaar aantal leds branden. */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PrioriteitsOpties_Add().                                                */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void Traffick2TLCgen_PRIO_OPTIES(void) /* Fik230101                                                         */
{
  count fc,k;
  mulv  grw_leds      = 5;             /* grens aantal leds wt_voorspeller voor conflicterende prioriteit   */
  boolv dvm_actief    = FALSE;
  int   OV_DVM_opties = poGeenPrioriteit;
  int   OV_DVM_niveau = 0;

#ifdef prmwtv
  grw_leds = PRM[prmwtv];              /* neem instelling parameter over indien die is gedefinieerd         */
#endif

#ifdef mdvmperiod
  dvm_actief = (mdvmperiod > 0);
#endif

  Traffick2TLCgen_HLPD();              /* corrigeer voor hulpdienst ingreep */
  Traffick2TLCgen_PELOTON();           /* corrigeer voor peloton ingreep (aanhouden groenfase) */
  Traffick2TLCgen_FIETS();             /* corrigeer voor fiets voorrang module */

  for (fc = 0; fc < FCMAX; ++fc)       /* corrigeer indien richting gedoseerd wordt */
  {
    count prio_OV_kar_index    = prio_index[fc].OV_kar;
    count prio_OV_srm_index    = prio_index[fc].OV_srm;
    count prio_OV_verlos_index = prio_index[fc].OV_verlos;
    count prio_vrw_index       = prio_index[fc].VRW;
    count prio_fts_index       = prio_index[fc].FTS;

    if (DOSEER[fc])
    {
      if (prio_OV_kar_index    > NG) iPrioriteitsOpties[prio_OV_kar_index]    = poGeenPrioriteit;
      if (prio_OV_srm_index    > NG) iPrioriteitsOpties[prio_OV_srm_index]    = poGeenPrioriteit;
      if (prio_OV_verlos_index > NG) iPrioriteitsOpties[prio_OV_verlos_index] = poGeenPrioriteit;
      if (prio_vrw_index       > NG) iPrioriteitsOpties[prio_vrw_index]       = poGeenPrioriteit;
      if (prio_fts_index       > NG) iPrioriteitsOpties[prio_fts_index]       = poGeenPrioriteit;
    }

    if (GEEN_OV_PRIO)
    {
      if ((prio_OV_kar_index    > NG) && !iPrioriteit[prio_OV_kar_index]   ) iPrioriteitsOpties[prio_OV_kar_index]    &= ~poBijzonderRealiseren;
      if ((prio_OV_srm_index    > NG) && !iPrioriteit[prio_OV_srm_index]   ) iPrioriteitsOpties[prio_OV_srm_index]    &= ~poBijzonderRealiseren;
      if ((prio_OV_verlos_index > NG) && !iPrioriteit[prio_OV_verlos_index]) iPrioriteitsOpties[prio_OV_verlos_index] &= ~poBijzonderRealiseren;
    }

    if (GEEN_VW_PRIO)
    {
      if ((prio_vrw_index       > NG) && !iPrioriteit[prio_vrw_index]      ) iPrioriteitsOpties[prio_vrw_index]       &= ~poBijzonderRealiseren;
    }

#ifdef TRAFFICK_DVM
    if (dvm_actief)                    /* schakel fiets- en vrachtwagen prioriteit uit */
    {
      if ((prio_vrw_index > NG) && !iPrioriteit[prio_vrw_index]) iPrioriteitsOpties[prio_vrw_index] = poGeenPrioriteit;
      if ((prio_fts_index > NG) && !iPrioriteit[prio_fts_index]) iPrioriteitsOpties[prio_fts_index] = poGeenPrioriteit;

      if (prmovdm[fc] > NG) 
      {
        OV_DVM_opties = BepaalPrioriteitsOpties(prmovdvm[fc]);
        OV_DVM_niveau = PRM[prmovdvm[fc]]/1000L;

                                       /* pas OV prioriteit KAR aan obv de ingestelde OV_DVM_opties */
        if ((prio_OV_kar_index > NG) && !iPrioriteit[prio_OV_kar_index])
        {
          if (!(OV_DVM_opties&poAanvraag                  )) iPrioriteitsOpties[prio_OV_kar_index] &= ~poAanvraag;
          if (!(OV_DVM_opties&poAfkappenKonfliktRichtingen)) iPrioriteitsOpties[prio_OV_kar_index] &= ~poAfkappenKonfliktRichtingen;
          if (!(OV_DVM_opties&poGroenVastHouden           )) iPrioriteitsOpties[prio_OV_kar_index] &= ~poGroenVastHouden;
          if (!(OV_DVM_opties&poBijzonderRealiseren       )) iPrioriteitsOpties[prio_OV_kar_index] &= ~poBijzonderRealiseren;
          if (!(OV_DVM_opties&poAfkappenKonflikterendOV   )) iPrioriteitsOpties[prio_OV_kar_index] &= ~poAfkappenKonflikterendOV;
          if (!(OV_DVM_opties&poNoodDienst                )) iPrioriteitsOpties[prio_OV_kar_index] &= ~poNoodDienst;
          if (iInstPrioriteitsNiveau[prio_OV_kar_index] > OV_DVM_niveau)
          {
            iInstPrioriteitsNiveau[prio_OV_kar_index] = OV_DVM_niveau)
          }
        }

                                       /* pas OV prioriteit SRM aan obv de ingestelde OV_DVM_opties */
        if ((prio_OV_srm_index > NG) && !iPrioriteit[prio_OV_srm_index])
        {
          if (!(OV_DVM_opties&poAanvraag                  )) iPrioriteitsOpties[prio_OV_srm_index] &= ~poAanvraag;
          if (!(OV_DVM_opties&poAfkappenKonfliktRichtingen)) iPrioriteitsOpties[prio_OV_srm_index] &= ~poAfkappenKonfliktRichtingen;
          if (!(OV_DVM_opties&poGroenVastHouden           )) iPrioriteitsOpties[prio_OV_srm_index] &= ~poGroenVastHouden;
          if (!(OV_DVM_opties&poBijzonderRealiseren       )) iPrioriteitsOpties[prio_OV_srm_index] &= ~poBijzonderRealiseren;
          if (!(OV_DVM_opties&poAfkappenKonflikterendOV   )) iPrioriteitsOpties[prio_OV_srm_index] &= ~poAfkappenKonflikterendOV;
          if (!(OV_DVM_opties&poNoodDienst                )) iPrioriteitsOpties[prio_OV_srm_index] &= ~poNoodDienst;
          if (iInstPrioriteitsNiveau[prio_OV_srm_index] > OV_DVM_niveau)
          {
            iInstPrioriteitsNiveau[prio_OV_srm_index] = OV_DVM_niveau)
          }
        }

                                       /* pas OV prioriteit verlosmelding aan obv de ingestelde OV_DVM_opties */
        if ((prio_OV_verlos_index > NG) && !iPrioriteit[prio_OV_verlos_index])
        {
          if (!(OV_DVM_opties&poAanvraag                  )) iPrioriteitsOpties[prio_OV_verlos_index] &= ~poAanvraag;
          if (!(OV_DVM_opties&poAfkappenKonfliktRichtingen)) iPrioriteitsOpties[prio_OV_verlos_index] &= ~poAfkappenKonfliktRichtingen;
          if (!(OV_DVM_opties&poGroenVastHouden           )) iPrioriteitsOpties[prio_OV_verlos_index] &= ~poGroenVastHouden;
          if (!(OV_DVM_opties&poBijzonderRealiseren       )) iPrioriteitsOpties[prio_OV_verlos_index] &= ~poBijzonderRealiseren;
          if (!(OV_DVM_opties&poAfkappenKonflikterendOV   )) iPrioriteitsOpties[prio_OV_verlos_index] &= ~poAfkappenKonflikterendOV;
          if (!(OV_DVM_opties&poNoodDienst                )) iPrioriteitsOpties[prio_OV_verlos_index] &= ~poNoodDienst;
          if (iInstPrioriteitsNiveau[prio_OV_verlos_index] > OV_DVM_niveau)
          {
            iInstPrioriteitsNiveau[prio_OV_verlos_index] = OV_DVM_niveau)
          }
        }
      }
    }
#endif
  }

  for (fc = 0; fc < FCMAX; ++fc)       /* corrigeer voor wachttijd voorspellers */
  {
    if ((Aled[fc] > 0) && (RA[fc] || (Aled[fc] < grw_leds)))
    {
      for (k = 0; k < FCMAX; ++k)
      {
        if (FK_conflict(fc, k))
        {
          count prio_OV_kar_index    = prio_index[k].OV_kar;
          count prio_OV_srm_index    = prio_index[k].OV_srm;
          count prio_OV_verlos_index = prio_index[k].OV_verlos;
          count prio_vrw_index       = prio_index[k].VRW;
          count prio_fts_index       = prio_index[k].FTS;

          if ((prio_OV_kar_index    > NG) && (!G[k] || !iPrioriteit[prio_OV_kar_index]   )) iPrioriteitsOpties[prio_OV_kar_index]    = poGeenPrioriteit;
          if ((prio_OV_srm_index    > NG) && (!G[k] || !iPrioriteit[prio_OV_srm_index]   )) iPrioriteitsOpties[prio_OV_srm_index]    = poGeenPrioriteit;
          if ((prio_OV_verlos_index > NG) && (!G[k] || !iPrioriteit[prio_OV_verlos_index])) iPrioriteitsOpties[prio_OV_verlos_index] = poGeenPrioriteit;
          if ((prio_vrw_index       > NG) && (!G[k] || !iPrioriteit[prio_vrw_index]      )) iPrioriteitsOpties[prio_vrw_index]       = poGeenPrioriteit;
          if ((prio_fts_index       > NG) && (!G[k] || !iPrioriteit[prio_fts_index]      )) iPrioriteitsOpties[prio_fts_index]       = poGeenPrioriteit;
        }
      }
    }
  }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer prioriteitstoekenning                                                                  */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCgen laat de prioriteitstoekenning AAN staan indien de prioriteitsopties (tijdelijk) worden uitgezet.  */
/* Deze functie corrigeert dit en reset in dat geval iPrioriteit[].                                         */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PrioriteitsToekenning_Add().                                            */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void Traffick2TLCgen_PRIO_TOE(void)   /* Fik230101                                                          */
{
  count prio;

  for (prio = 0; prio < prioFCMAX; ++prio)
  {
    if (iPrioriteit[prio] && (iPrioriteitsOpties[prio] == 0))
    {
      iPrioriteit[prio] = 0;
    }
  }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie manipuleer TVG_max[] naloop harde koppelingen                                                    */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCgen gaat bij de berekening van de maximale duur van de naloop uit van de koppeltijden + de ingestelde */
/* TVG_max[] van de naloop richting. Deze tijd kan te hoog zijn omdat de maximale duur van het verlenggroen */
/* na afloop van de koppeltijden lager kan zijn ingesteld dan TVG_max[]. Deze functie manipuleert de        */
/* instelling van TVG_max[] juist voor de startgroen berekening van prioriteitsrealisaties, zodat die wel   */
/* met de juiste waarde rekent. In StartGroenMomenten_Add() worden de oorspronkelijke waarden terug gezet.  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit AfkapGroen_Add().                                                       */
/*                                                                                                          */
#ifdef NALOPEN
#ifdef PRIO_ADDFILE
void Traffick2TLCpas_TVG_aan(void)    /* Fik230101                                                          */
{
  count fc, i;

  for (fc = 0; fc < FCMAX; ++fc)      /* buffer instelling TVG_max[] en TGL_max[] */
  {
    TVG_instelling[fc] = TVG_max[fc];
    TGL_instelling[fc] = TGL_max[fc];
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc1     = hki_kop[i].fc1;      /* voedende richting */
      count fc2     = hki_kop[i].fc2;      /* volg     richting */
      mulv  kop_max = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */
      mulv  status  = hki_kop[i].status;   /* status koppeling  */

      if (status >= 1)                /* voedende richting is groen of groen geweest */
      {
        if (TVG_max[fc2] > kop_max) TVG_max[fc2] = kop_max;
        if (TGL_max[fc2] < 1      ) TGL_max[fc2] = 1;
      }
    }
  }

  for (i = 0; i < aantal_vtg_tgo; ++i)
  {
    count fc1      = vtg_tgo[i].fc1;      /* richting 1 */
    count fc2      = vtg_tgo[i].fc2;      /* richting 2 */
    count tnlsgd12 = vtg_tgo[i].tnlsgd12; /* nalooptijd fc2 vanaf startgroen fc1 */
    count tnlsgd21 = vtg_tgo[i].tnlsgd21; /* nalooptijd fc1 vanaf startgroen fc2 */

    if (tnlsgd12 != NG)
    {
      TVG_max[fc2] = 0;
      if (TGL_max[fc2] < 1) TGL_max[fc2] = 1;
    }

    if (tnlsgd21 != NG)
    {
      TVG_max[fc1] = 0;
      if (TGL_max[fc1] < 1) TGL_max[fc1] = 1;
    }
  }
}
#endif
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie manipuleer TVG_max[] naloop harde koppelingen                                                    */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCgen gaat bij de berekening van de maximale duur van de naloop uit van de koppeltijden + de ingestelde */
/* TVG_max[] van de naloop richting. Deze tijd kan te hoog zijn omdat de maximale duur van het verlenggroen */
/* na afloop van de koppeltijden lager kan zijn ingesteld dan TVG_max[].                                    */
/*                                                                                                          */
/* De functie Traffick2TLCpas_TVG_aan() manipuleert de instelling van TVG_max[] juist voor de startgroen    */
/* berekening van prioriteitsrealisaties, zodat die wel met de juiste waarde rekent. Deze functie zet       */
/* juist na de startgroen berekening de oorspronkelijke waarden weer terug.                                 */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit StartGroenMomenten_Add().                                               */
/*                                                                                                          */
#ifdef NALOPEN
#ifdef PRIO_ADDFILE
void Traffick2TLCzet_TVG_terug(void)  /* Fik230101                                                          */
{
  count fc;

  for (fc = 0; fc < FCMAX; ++fc)      /* zet instelling TVG_max[] en TGL_max[] terug */
  {
    TVG_max[fc] = TVG_instelling[fc];
    TGL_max[fc] = TGL_instelling[fc];
  }
}
#endif
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer PRIO_RR_BIT[]                                                                          */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCgen stuurt een volgrichting niet RR[] indien de voedende richting wordt tegengehouden door een        */
/* prioriteitsrealisatie. Indien de volgrichting echter niet zelfstandig op aanvraag mag realiseren dan     */
/* is het wel nodig om ook het PRIO_RR_BIT[] door te zetten naar de volgrichting. Deze functie verzorgt     */
/* het doorschrijven van het PRIO_RR_BIT[].                                                                 */
/*                                                                                                          */
/* De functie schrijft daarnaast ook het PRIO_RR_BIT[] door aan richtingen die onderling een gelijkstart    */
/* hebben.                                                                                                  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PrioTegenhouden_Add() en Traffick2TLCgen_PRIO().                        */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void Traffick2TLCgen_PRIO_RR(void)    /* Fik230101                                                          */
{
  count i,prio;

  for (prio = 0; prio < prioFCMAX; ++prio)
  {
    if (iPrioriteit[prio])
    {
      if ((iPrioriteitsOpties[prio]&poNoodDienst) || (iPrioriteitsOpties[prio]&poBijzonderRealiseren))
      {
        count fc = iFC_PRIOix[prio];
        for (i = 0; i < FCMAX; ++i)
        {
          if ((TMPc[fc][i] != NG) && (TMPc[fc][i] != FK)) RR[i] |= PRIO_RR_BIT;
        }
      }
    }
  }

  for (i = 0; i < FCMAX; ++i)         /* geen PRIO_RR_BIT[] als richting altijd moet komen */
  {
    if (RA[i] && P[i]) RR[i] &= ~PRIO_RR_BIT;
  }

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc1     = hki_kop[i].fc1;     /* voedende richting */
      count fc2     = hki_kop[i].fc2;     /* volg     richting */
      boolv los_fc2 = hki_kop[i].los_fc2; /* fc2 mag bij aanvraag fc1 los realiseren */
      count status  = hki_kop[i].status;  /* status koppeling */

      if (GL[fc1] || TRG[fc1] || R[fc1] && A[fc1] && !los_fc2 && (status != 1))
      {
         if (!G[fc2] && (RR[fc1]&PRIO_RR_BIT)) RR[fc2] |= PRIO_RR_BIT;
      }
    }
  }

  for (i = 0; i < aantal_vtg_tgo; ++i)
  {
    count fc1      = vtg_tgo[i].fc1;      /* richting 1 */
    count fc2      = vtg_tgo[i].fc2;      /* richting 2 */
    count hnla12   = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
    count hnla21   = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
    mulv  status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
    mulv  status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

    if (GL[fc1] || TRG[fc1] || R[fc1] && IH[hnla12] || R[fc2] && IH[hnla21] || R[fc1] && A[fc1] && R[fc2] && A[fc2])
    {
      if (!G[fc2] && (RR[fc1]&PRIO_RR_BIT) && (status12 != 1)) RR[fc2] |= PRIO_RR_BIT;
    }

    if (GL[fc2] || TRG[fc2] || R[fc2] && IH[hnla21] || R[fc1] && IH[hnla12] || R[fc1] && A[fc1] && R[fc2] && A[fc2])
    {
      if (!G[fc1] && (RR[fc2]&PRIO_RR_BIT) && (status21 != 1)) RR[fc1] |= PRIO_RR_BIT;
    }
  }

  for (i = 0; i < aantal_lvk_gst; ++i)
  {
    count fc1 = lvk_gst[i].fc1;       /* richting 1 */
    count fc2 = lvk_gst[i].fc2;       /* richting 2 */
    count fc3 = lvk_gst[i].fc3;       /* richting 3 */
    count fc4 = lvk_gst[i].fc4;       /* richting 4 */

    boolv FcMetP  = FALSE;
    boolv FcMetRR = FALSE;

    if ((fc1 != NG) && RA[fc1] && P[fc1]) FcMetP = TRUE;
    if ((fc2 != NG) && RA[fc2] && P[fc2]) FcMetP = TRUE;
    if ((fc3 != NG) && RA[fc3] && P[fc3]) FcMetP = TRUE;
    if ((fc4 != NG) && RA[fc4] && P[fc4]) FcMetP = TRUE;

    if (!FcMetP)
    {
      if ((fc1 != NG) && (GL[fc1] || TRG[fc1] || R[fc1] && A[fc1]) && (RR[fc1]&PRIO_RR_BIT)) FcMetRR = TRUE;
      if ((fc2 != NG) && (GL[fc2] || TRG[fc2] || R[fc2] && A[fc2]) && (RR[fc2]&PRIO_RR_BIT)) FcMetRR = TRUE;
      if ((fc3 != NG) && (GL[fc3] || TRG[fc3] || R[fc3] && A[fc3]) && (RR[fc3]&PRIO_RR_BIT)) FcMetRR = TRUE;
      if ((fc4 != NG) && (GL[fc4] || TRG[fc4] || R[fc4] && A[fc4]) && (RR[fc4]&PRIO_RR_BIT)) FcMetRR = TRUE;
    }

    if (FcMetP)
    {
      if ((fc1 != NG) && !G[fc1]) RR[fc1] &= ~PRIO_RR_BIT;
      if ((fc2 != NG) && !G[fc2]) RR[fc2] &= ~PRIO_RR_BIT;
      if ((fc3 != NG) && !G[fc3]) RR[fc3] &= ~PRIO_RR_BIT;
      if ((fc4 != NG) && !G[fc4]) RR[fc4] &= ~PRIO_RR_BIT;
    }

    if (FcMetRR)
    {
      if ((fc1 != NG) && !G[fc1]) RR[fc1] |= PRIO_RR_BIT;
      if ((fc2 != NG) && !G[fc2]) RR[fc2] |= PRIO_RR_BIT;
      if ((fc3 != NG) && !G[fc3]) RR[fc3] |= PRIO_RR_BIT;
      if ((fc4 != NG) && !G[fc4]) RR[fc4] |= PRIO_RR_BIT;
    }
  }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer toestemming alternatieve realisatie tijdens een prioriteitsrealisatie                  */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCgen staat een alternatieve realisaties toe indien er voldoende ruimte is als gevolg van een           */
/* prioriteitsrealisatie van een niet conflicterende richting. Dit mag echter niet gebeuren voor richtingen */
/* die gedoseerd worden of indien Traffick heeft uitgerekend dat een alternatieve realisatie niet past.     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PrioAlternatieven_Add().                                                */
/*                                                                                                          */
void Traffick2TLCgen_PRIO_PAR(void)   /* Fik230101                                                          */
{
  count i;

  for (i = 0; i < FCMAX; ++i)
  {
    if (REALtraffick[i] == NG)
    {
      PAR[i] = PARtraffick[i];
    }
    if (DOSEER[i]) PAR[i] = FALSE;
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer prioriteitsafhandeling                                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCgen beeindigt ten onrechte het meeverlenggroen bij conflicten indien er geen prioriteitsrealisatie is */
/* gedefinieerd. Deze functie corrigeert dit. Daarnaast wordt het Z[] signaal gecorrigeerd voor richtingen  */
/* die onderling een gelijkstart hebben. Tenslotte worden op basis van HLPD[] de hulpdienst ingrepen        */
/* geaktiveerd.                                                                                             */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PostAfhandelingPrio_Add().                                              */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void Traffick2TLCgen_PRIO(void)         /* Fik230101                                                        */
{
  count i,k,prio;

  for (i = 0; i < FCMAX; ++i)
  {
    if (MG[i] && (Z[i]&PRIO_Z_BIT))
    {
      boolv gevonden = FALSE;
      for (k = 0; k < FCMAX; ++k)
      {
        if (GK_conflict(i,k) > NG)
        {
          for (prio = 0; prio < prioFCMAX; ++prio)
          {
            if ((iPrioriteit[prio]) && (iFC_PRIOix[prio] == k))
            {
               if ((iPrioriteitsOpties[prio]&poNoodDienst) || (iPrioriteitsOpties[prio]&poBijzonderRealiseren)) gevonden = TRUE;
            }
            if (gevonden) break;
          }
          if (gevonden) break;
        }
      }
      if (!gevonden) Z[i] &= ~PRIO_Z_BIT;
    }
  }

  for (i = 0; i < FCMAX; ++i)
  {
    if (HLPD[i])
    {
      RTFB |= PRIO_RTFB_BIT;
      if (R[i])
      {
        A[i] |= PRIO_A_BIT;
        if (RV[i] && !TRG[i] && !K[i] && !tkcv(i) && !AA[i] && !tfkaa(i)) set_pref_AA(i, TRUE);
      }
      if (G[i] && !MG[i]) YV[i] |= PRIO_YV_BIT;
      Z[i]   = FALSE;
      YM[i] |= PRIO_YM_BIT;

      for (k = 0; k < FCMAX; ++k)
      {
        if (GK_conflict(i,k) > NG)
        {
          if (R[k]) RR[k] |= PRIO_RR_BIT;
#ifdef __EXTRA_FUNC_RIS_H
          if (G[k] && (VG[k] || MG[k]) && !granted_verstrekt[k]) Z[k] |= PRIO_Z_BIT;
#else
          if (G[k] && (VG[k] || MG[k])) Z[k] |= PRIO_Z_BIT;
#endif
        }
      }
    }
  }

  for (i = 0; i < aantal_lvk_gst; ++i)
  {
    count fc1 = lvk_gst[i].fc1;       /* richting 1 */
    count fc2 = lvk_gst[i].fc2;       /* richting 2 */
    count fc3 = lvk_gst[i].fc3;       /* richting 3 */
    count fc4 = lvk_gst[i].fc4;       /* richting 4 */

    boolv FcInRa = FALSE;
    if ((fc1 != NG) && RA[fc1] && (P[fc1] || !RR[fc1])) FcInRa = TRUE;
    if ((fc2 != NG) && RA[fc2] && (P[fc2] || !RR[fc2])) FcInRa = TRUE;
    if ((fc3 != NG) && RA[fc3] && (P[fc3] || !RR[fc3])) FcInRa = TRUE;
    if ((fc4 != NG) && RA[fc4] && (P[fc4] || !RR[fc4])) FcInRa = TRUE;

    if (FcInRa)
    {
      if ((fc1 != NG) && MG[fc1]) Z[fc1] &= ~PRIO_Z_BIT;
      if ((fc2 != NG) && MG[fc2]) Z[fc2] &= ~PRIO_Z_BIT;
      if ((fc3 != NG) && MG[fc3]) Z[fc3] &= ~PRIO_Z_BIT;
      if ((fc4 != NG) && MG[fc4]) Z[fc4] &= ~PRIO_Z_BIT;
    }
  }

  for (i = 0; i < FCMAX; ++i)
  {
    if (G[i] && (!MG[i] && (YV[i]&PRIO_YV_BIT) || MG[i] && (YM[i]&PRIO_YM_BIT)))
    {
      for (k = 0; k < FCMAX; ++k)
      {
        if (GK_conflict(i,k) > NG)
        {
          RR[k] |= PRIO_RR_BIT;
        }
      }
    }
    else
    {
      MK[i] &= ~PRIO_MK_BIT;
    }
  }
  Traffick2TLCgen_PRIO_RR();

  for (i = 0; i < aantal_hki_kop; ++i)  /* TLCgen houdt tijdens RA[fc1] volgrichting fc2 in RW[] vast */
  {                                     /* ... Traffick niet dus resetten Z[fc2] tijdens RA[] is nodig */
    if (hki_kop[i].status > NG)         /* koppeling is ingeschakeld */
    {
      count fc1 = hki_kop[i].fc1;       /* voedende richting */
      count fc2 = hki_kop[i].fc2;       /* volg     richting */

      if (RA[fc1] && MG[fc2] || G[fc1] && G[fc2]) Z[fc2] = FALSE;
    }
  }

  for (i = 0; i < aantal_vtg_tgo; ++i)
  {
    count fc1      = vtg_tgo[i].fc1;      /* richting 1 */
    count fc2      = vtg_tgo[i].fc2;      /* richting 2 */
    count hnla12   = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
    count hnla21   = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
    count status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
    count status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

    if (MG[fc1] && RA[fc2] && (status12 == 1)) Z[fc1] = FALSE; /* vasthouden voeding is nodig omdat */
    if (MG[fc2] && RA[fc1] && (status21 == 1)) Z[fc2] = FALSE; /* nog aan de overzijde gedrukt kan worden */

    if (GL[fc1] || TRG[fc1] || R[fc1] && IH[hnla12] || R[fc2] && IH[hnla21])
    {
      if (!G[fc2] && (RR[fc1]&PRIO_RR_BIT) && (status12 != 1)) RR[fc2] |= PRIO_RR_BIT;
    }

    if (GL[fc2] || TRG[fc2] || R[fc2] && IH[hnla21] || R[fc1] && IH[hnla12])
    {
      if (!G[fc1] && (RR[fc2]&PRIO_RR_BIT) && (status21 != 1)) RR[fc1] |= PRIO_RR_BIT;
    }
  }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal TI_max[][]                                                                            */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de intergroentijd van een conflict.                                    */
/*                                                                                                          */
mulv TI_max(                          /* Fik230101                                                          */
count fc1,                            /* FC fasecyclus 1                                                    */
count fc2)                            /* FC fasecyclus 2                                                    */
{
#ifndef NO_TIGMAX                     /* intergroentijden                                                   */
  if ((fc1 < FCMAX) && (fc2 < FCMAX))
  {
    return TIG_max[fc1][fc2];
  }
  return NG;
#else                                 /* ontruimingstijden                                                  */
  if ((fc1 < FCMAX) && (fc2 < FCMAX))
  {
    if (TO_max[fc1][fc2] >= 0) return (TGL_max[fc1] + TO_max[fc1][fc2]);
    if (TO_max[fc1][fc2] <  0) return  TO_max[fc1][fc2];
  }
  return NG;
#endif
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal TI[][]                                                                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde het aflopen van de intergroentijd.                                     */
/*                                                                                                          */
boolv TI(                             /* Fik230101                                                          */
count fc1,                            /* FC fasecyclus 1                                                    */
count fc2)                            /* FC fasecyclus 2                                                    */
{
#ifndef NO_TIGMAX                     /* intergroentijden                                                   */
  if ((fc1 < FCMAX) && (fc2 < FCMAX))
  {
    if (TIG_max[fc1][fc2] >= 0) return TIG[fc1][fc2];
  }
  return FALSE;
#else                                 /* ontruimingstijden                                                  */
  if ((fc1 < FCMAX) && (fc2 < FCMAX))
  {
    if (TO_max[fc1][fc2] >= 0) return (GL[fc1] || TO[fc1][fc2]);
  }
  return FALSE;
#endif
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal TI_timer[]                                                                            */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de intergroen timer van een fasecyclus.                                */
/*                                                                                                          */
mulv TI_timer(                        /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
#ifndef NO_TIGMAX                     /* intergroentijden                                                   */
  if (fc < FCMAX)
  {
    return TIG_timer[fc];
  }
  return 0;
#else                                 /* ontruimingstijden                                                  */
  if (fc < FCMAX)
  {
    if (G[fc]) return TO_timer[fc];
    if (GL[fc])
    {
      if (TGL[fc])
      {
        return TGL_timer[fc];
      }
      else
      {
        return (TGL_max[fc] + TO_timer[fc]);
      }
    }
    if (R[fc]) return (TGL_max[fc] + TO_timer[fc]);
  }
  return 0;
#endif
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal of twee richtingen conflicterend zijn                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de intergroentijd (van fc1 naar fc2) als deze richtingen conflicterend */
/* zijn. Indien de richtingen niet conflicterend zijn is de return waarde NG.                               */
/*                                                                                                          */
mulv GK_conflict(                     /* Fik230101                                                          */
count fc1,                            /* FC fasecyclus 1                                                    */
count fc2)                            /* FC fasecyclus 2                                                    */
{
  count j,k;
  mulv  ontruim = 0;

  for (j = 0; j < GKFC_MAX[fc1]; ++j)
  {
    k = KF_pointer[fc1][j];
    if (fc2 == k)
    {
      ontruim = TI_max(fc1,fc2);
      if (ontruim >= 0) return ontruim;
      else              return 0;
    }
  }

  for (k = 0; k < FCMAX; ++k)
  {
    if ((TMPc[fc1][k] != NG) && (TMPc[fc1][k] != FK))
    {
      if (fc2 == k)
      {
        ontruim = TMPi[fc1][fc2];
        if (ontruim >= 0) return ontruim;
        else              return 0;
      }
    }
  }
  return NG;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal of twee richtingen (fictief)conflicterend zijn                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde TRUE als twee richtingen (tijdelijk) (fictief)conflicterend zijn.      */
/*                                                                                                          */
boolv FK_conflict(                    /* Fik230101                                                          */
count fc1,                            /* FC fasecyclus 1                                                    */
count fc2)                            /* FC fasecyclus 2                                                    */
{
  count j,k;

  for (j = 0; j < FKFC_MAX[fc1]; ++j)
  {
    k = KF_pointer[fc1][j];
    if (fc2 == k) return TRUE;
  }

  for (k = 0; k < FCMAX; ++k)
  {
    if (TMPc[fc1][k] != NG)
    {
      if (fc2 == k) return TRUE;
    }
  }
  return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal conflicterende richting met een aanvraag                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende richting met een aanvraag.      */
/* Het verschil met de CCOLfunctie ka() is dat ook tijdelijke conflicten worden getest op een aanvraag.     */
/*                                                                                                          */
boolv tka(                            /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count k;

  for (k = 0; k < FCMAX; ++k)
  {
    if (GK_conflict(fc,k) > NG)
    {
      if (A[k] && !G[k] && !BL[k]) return TRUE;
    }
  }
  return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid (fictief)conflicterende richting in RA[]                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een (fictief)conflicterende richting in RA[].      */
/*                                                                                                          */
boolv fkra(                           /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count k;

  for (k = 0; k < FCMAX; ++k)
  {
    if (FK_conflict(fc,k))
    {
      if (RA[k]) return TRUE;
    }
  }
  return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid (fictief)conflicterende richting in RA[] met P[]                         */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een (fictief)conflicterende ri. in RA[] met P[].   */
/*                                                                                                          */
boolv fkrap(                          /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count k;

  for (k = 0; k < FCMAX; ++k)
  {
    if (FK_conflict(fc,k))
    {
      if (RA[k] && P[k]) return TRUE;
    }
  }
  return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende richting met AAPR die (vooruit) kan realiseren            */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende richting met AAPR die (vooruit) */
/* kan realiseren. (geen AAPR[] BIT5)                                                                       */
/*                                                                                                          */
boolv kaapr(                          /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count k;

  for (k = 0; k < FCMAX; ++k)
  {
    if (GK_conflict(fc,k) > NG)
    {
      if (R[k] && A[k] && AAPR[k] && !(AAPR[k]&BIT5) && !(RR[k]&BIT6) && !(RR[k]&BIT10)) return TRUE;
    }
  }
  return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal (fictief)conflicterende richting in AA[].                                             */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een (fictief)conflicterende richting in AA[].      */
/* Het verschil met de CCOLfunctie fkaa() is dat ook tijdelijke (fictieve)conflicten worden getest op AA[]. */
/*                                                                                                          */
boolv tfkaa(                          /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count k;

  for (k = 0; k < FCMAX; ++k)
  {
    if (FK_conflict(fc,k))
    {
      if (AA[k] && (R[k] || SG[k] || GL[k])) return TRUE;
    }
  }
  return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal conflicterende richting in CV[].                                                      */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende richting in CV[]. Het verschil  */
/* met de CCOLfunctie kcv() is dat ook tijdelijke conflicten worden getest op CV[].                         */
/*                                                                                                          */
boolv tkcv(                           /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count k;

  for (k = 0; k < FCMAX; ++k)
  {
    if (GK_conflict(fc,k) > NG)
    {
      if ((R[k] || GL[k]) && AA[k] || CV[k] || G[k] && (RS[k] || RW[k])) return TRUE;
    }
  }
  return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende prioriteitsingreep welke nog niet is gerealiseerd         */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende prioriteitsingreep welke nog    */
/* niet is gerealiseerd. (nog niet groen is)                                                                */
/*                                                                                                          */
boolv conflict_prio_real(             /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count k;

  for (k = 0; k < FCMAX; ++k)
  {
    if (FK_conflict(fc,k))
    {
      if (!G[k] && (iPRIO[k] || HLPD[k])) return TRUE;
    }
  }
  return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende hulpdienst ingreep                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende hulpdienst ingreep.             */
/*                                                                                                          */
boolv khlpd(                          /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count k;

  for (k = 0; k < FCMAX; ++k)
  {
    if (FK_conflict(fc,k))
    {
      if (G[k] && HLPD[k]) return TRUE;
    }
  }
  return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal of een richting een volgrichting is binnen een gedefinieerde harde koppeling          */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als RETURN of een richting een volgrichting is binnen een gedefinieerde harde koppeling. */
/*                                                                                                          */
boolv volgrichting_hki(               /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count i;

  for (i = 0; i < aantal_hki_kop; ++i)
  {
    if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
    {
      count fc2 = hki_kop[i].fc2;     /* volg richting */
      if (fc == fc2) return TRUE;
    }
  }
  return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal of een richting een voorstart moet geven binnen een gedefinieerd deelconflict         */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als RETURN of een richting een voorstart moet geven aan een andere richting.             */
/*                                                                                                          */
boolv voorstart_gever(                /* Fik230101                                                          */
count fc)                             /* FC fasecyclus                                                      */
{
  count i;

  for (i = 0; i < aantal_dcf_vst; ++i)
  {
    count fc1 = dcf_vst[i].fc1;       /* richting die voorstart geeft  */
    if (fc == fc1) return TRUE;
  }
  return FALSE;
}


#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie stiptheid in testomgeving                                                                    */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie die op basis van de test ingangen IS[isvroeg] en IS[islaat] zorgt dat de DSI berichten in de */
/* testomgeving zorgen voor de gewenste stiptheid.                                                          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PreApplication_Add().                                                   */
/*                                                                                                          */
void test_stiptheid(                  /* Fik230101                                                          */
count ov_tevroeg,                     /* PRM ingestelde grens voor te vroeg in seconden                     */
count ov_telaat,                      /* PRM ingestelde grens voor te laat  in seconden                     */
count tst_stiptheid)                  /* PRM waarde stiptheid voor DSI berichten in de testomgeving         */
{
#if (defined isvroeg && defined islaat)

PRM[tst_stiptheid]   = 120;           /* in de testomgeving wordt hier 120 vanaf gehaald (dus 0 = op tijd)  */

if (CIF_IS[isvroeg])
{
  PRM[tst_stiptheid] = 120 - PRM[ov_tevroeg] - 1;
}
if (CIF_IS[islaat])
{
  PRM[tst_stiptheid] = 120 + PRM[ov_telaat]  + 1;
}
#endif
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie aanmaken dumpfile in testomgeving                                                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie maakt bij fase bewaking een dumpfile aan met applicatie gegevens.                                */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_dump_application().                                                */
/*                                                                                                          */
void DumpTraffick(void)               /* Fik230101                                                          */
{
  FILE *fp;
  count i,fc;

  fp = fopen("DumpTraffick.dmp", "wb");
  if (fp != NULL) {

    fprintf(fp, "FC   T2SG T2EG AltR  TFB AAPR   AR   PG  PAR HLPD");
    fprintf(fp,"\r\n");

    for (fc = 0; fc < FCMAX; ++fc)
    {
      fprintf(fp, "%s%s%5d%5d%5d%5d%5d%5d%5d%5d%5d","FC",FC_code[fc],REALtraffick[fc],TEG[fc],AltRuimte[fc],TFB_timer[fc],AAPR[fc],AR[fc],PG[fc],PAR[fc],HLPD[fc]);
      fprintf(fp,"\r\n");
    }
    fprintf(fp,"\r\n");

    i = dumpstap + 1;
    if (i >= MAXDUMPSTAP) i = 0;

    fprintf(fp, "Flight buffer: %02d-%02d-%02d\r\n", CIF_KLOK[CIF_DAG], CIF_KLOK[CIF_MAAND], CIF_KLOK[CIF_JAAR]);

    while (i != dumpstap)
    {
      if ((_UUR[i] > 0) || (_MIN[i] > 0) || (_SEC[i] > 0))
      {
        fprintf(fp,"%02d:%02d:%02d",_UUR[i],_MIN[i],_SEC[i]);
        fprintf(fp,"  %4d",_ML[i]);
        for (fc = 0; fc < FCMAX; ++fc)
        {
          if ((_FCA[fc][i] != 'A') && (_FCA[fc][i] != 'E'))
          {
            fprintf(fp,"%3c",_FC[fc][i]);
          }
          else
          {
            fprintf(fp,"%3c",_FCA[fc][i]);
          }
        }

        fprintf(fp,"\r\n");
        if ((_SEC[i] == 0) || (_SEC[i] == 30))
        {
          fprintf(fp,"              ");
          for(fc = 0; fc < FCMAX; fc++)
          {
            fprintf(fp,"%3s",FC_code[fc]);
          }
          fprintf(fp,"\r\n");
        }
      }
      i++;
      if (i >= MAXDUMPSTAP) i = 0;
    }
    fclose(fp);
  }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie update flight buffer in testomgeving                                                             */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie werkt iedere seconde het flight buffer bij tbv analyse van fase bewakingen in de testomgeving.   */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PostApplication_Add().                                                  */
/*                                                                                                          */
void FlightTraffick(void)             /* Fik230101                                                          */
{
  count i;

  if (TS)
  {
    for (i = 0; i < FCMAX; ++i)       /* initialisatie */
    {
      _ML[dumpstap] = 0;
      _FCA[i][dumpstap] = ' ';
    }

    for (i = 0; i < FCMAX; ++i)
    {
      if (A[i] && !_HA[i])            /* start aanvraag */
      {
        _FCA[i][dumpstap] = 'A';
      }
      if (!A[i] && _HA[i])
      {
        _FCA[i][dumpstap] = 'E';      /* einde aanvraag */
      }
      _HA[i] = (boolv)A[i];
    }

    for (i = 0; i < FCMAX; i++)       /* fasecyclus status */
    {
      if (CG[i] == CG_RV) _FC[i][dumpstap] = ' ';
      if (CG[i] == CG_RA) _FC[i][dumpstap] = '.';
      if (CG[i] == CG_VS) _FC[i][dumpstap] = 'S';
      if (CG[i] == CG_FG) _FC[i][dumpstap] = '#';
      if (CG[i] == CG_WG) _FC[i][dumpstap] = 'W';
      if (CG[i] == CG_VG) _FC[i][dumpstap] = '[';
      if (CG[i] == CG_MG) _FC[i][dumpstap] = 'M';
      if (CG[i] == CG_GL) _FC[i][dumpstap] = 'Z';

      if (HLPD[i])
      {
        if ( G[i]) _FC[i][dumpstap] = 'H';
        if (!G[i]) _FC[i][dumpstap] = 'h';
      }
    }

    _UUR[dumpstap] = (char)CIF_KLOK[CIF_UUR];
    _MIN[dumpstap] = (char)CIF_KLOK[CIF_MINUUT];
    _SEC[dumpstap] = (char)CIF_KLOK[CIF_SECONDE];

#ifdef MLAMAX
    _ML[dumpstap] = MLA + 1;
#else
    _ML[dumpstap] = ML  + 1;
#endif

    dumpstap++;
    if (dumpstap >= MAXDUMPSTAP) dumpstap = 0;
  }
}
#endif
