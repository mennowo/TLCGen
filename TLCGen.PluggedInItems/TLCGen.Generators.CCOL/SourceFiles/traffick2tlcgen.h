/* -------------------------------------------------------------------------------------------------------- */
/* Traffick2TLCGen                                                               Versie 1.0.0 / 01 jan 2023 */
/* -------------------------------------------------------------------------------------------------------- */

/* Deze include file bevat hulp functies voor verkeerskundige Traffick functionaliteiten.                   */
/* Deze functies zijn ontwikkeld en geschreven door Marcel Fick.                                            */
/* Versie: 1.0                                                                                              */
/* Datum:  1 januari 2023                                                                                   */

#ifndef __TRAFFICK2TLCGEN_H
#define __TRAFFICK2TLCGEN_H

#define MAX_HKI_KOP  FCMAX            /* maximum aantal harde koppelingen                                   */
#define MAX_VTG_KOP  FCMAX            /* maximum aantal voetgangerskoppelingen - type gescheiden oversteek  */
#define MAX_LVK_GST  FCMAX            /* maximum aantal gelijk starten langzaam verkeer                     */
#define MAX_DCF_VST  FCMAX            /* maximum aantal deelconflicten voorstart                            */
#define MAX_PEL_KOP     10            /* maximum aantal peloton koppelingen                                 */
#define MAX_FTS_PRI     20            /* maximum aantal definities fiets voorrang module                    */
#define MAXDUMPSTAP    600            /* aantal seconden flight buffer in testomgeving                      */

struct hki_koppeling {
  count fc1;                          /* FC    voedende richting                                            */
  count fc2;                          /* FC    volg     richting                                            */
  count tlr21;                        /* TM    late release fc2 (= inrijtijd)                               */
  count tnlfg12;                      /* TM    vaste   nalooptijd vanaf einde vastgroen      fc1            */
  count tnlfgd12;                     /* TM    det.afh.nalooptijd vanaf einde vastgroen      fc1            */
  count tnleg12;                      /* TM    vaste   nalooptijd vanaf einde (verleng)groen fc1            */
  count tnlegd12;                     /* TM    det.afh.nalooptijd vanaf einde (verleng)groen fc1            */
  boolv kop_eg;                       /* boolv koppeling vanaf einde groen (als FALSE dan vanaf EV)         */
  boolv los_fc2;                      /* boolv fc2 mag bij aanvraag fc1 los realiseren                      */
  mulv  kop_max;                      /* mulv  maximum verlenggroen na harde koppeling                      */
  mulv  status;                       /* mulv  status koppeling                                             */
};

struct vtg_koppeling {
  count fc1;                          /* FC   richting 1                                                    */
  count fc2;                          /* FC   richting 2                                                    */
  count tinl12;                       /* TM   inlooptijd fc1                                                */
  count tinl21;                       /* TM   inlooptijd fc2                                                */
  count tnlsgd12;                     /* TM   nalooptijd fc2 vanaf startgroen fc1                           */
  count tnlsgd21;                     /* TM   nalooptijd fc1 vanaf startgroen fc2                           */
  count hnla12;                       /* HE   drukknop melding koppeling vanaf fc1 aanwezig                 */
  count hnla21;                       /* HE   drukknop melding koppeling vanaf fc2 aanwezig                 */
  count hlos1;                        /* HE   los realiseren fc1 toegestaan                                 */
  count hlos2;                        /* HE   los realiseren fc2 toegestaan                                 */
  mulv  status12;                     /* mulv status koppeling fc1 -> fc2                                   */
  mulv  status21;                     /* mulv status koppeling fc2 -> fc1                                   */
};

struct lvk_gelijkstr {
  count fc1;                          /* FC   richting 1                                                    */
  count fc2;                          /* FC   richting 2                                                    */
  count fc3;                          /* FC   richting 1                                                    */
  count fc4;                          /* FC   richting 2                                                    */
};

struct dcf_voorstart {
  count fc1;                          /* FC  richting die voorstart geeft                                   */
  count fc2;                          /* FC  richting die voorstart krijgt                                  */
  count tvs21;                        /* TM  voorstart fc2                                                  */
  count to12;                         /* TM  ontruimingstijd van fc1 naar fc2                               */
  count ma21;                         /* SCH meerealisatie van fc2 met fc1                                  */
  count mv21;                         /* SCH meeverlengen  van fc2 met fc1                                  */
};

struct pel_koppeling {
  count kop_fc;                       /* FC    koppelrichting                                               */
  count kop_toe;                      /* ME    toestemming peloton ingreep (bij NG altijd toestemming)      */
  count kop_sig;                      /* HE    koppelsignaal                                                */
  count kop_bew;                      /* TM    bewaak koppelsignaal (bij NG wordt een puls veronderstelt)   */
  count aanv_vert;                    /* TM    aanvraag vertraging  (bij NG wordt geen aanvraag opgzet)    */
  count vast_vert;                    /* TM    vasthoud vertraging  (start op binnenkomst koppelsignaal)    */
  count duur_vast;                    /* TM    duur vasthouden (bij duursign. na afvallen koppelsignaal)    */
  count duur_verl;                    /* TM    duur verlengen na ingreep (bij NG geldt TVG_max[])           */
  boolv kop_oud;                      /* boolv status koppelsignaal vorige machine slag                     */
  mulv  aanw_kop1;                    /* mulv  aanwezigheidsduur koppelsignaal 1 vanaf start puls           */
  mulv  duur_kop1;                    /* mulv  tijdsduur HOOG    koppelsignaal 1 igv duur signaal           */
  mulv  aanw_kop2;                    /* mulv  aanwezigheidsduur koppelsignaal 2 vanaf start puls           */
  mulv  duur_kop2;                    /* mulv  tijdsduur HOOG    koppelsignaal 2 igv duur signaal           */
  mulv  aanw_kop3;                    /* mulv  aanwezigheidsduur koppelsignaal 3 vanaf start puls           */
  mulv  duur_kop3;                    /* mulv  tijdsduur HOOG    koppelsignaal 3 igv duur signaal           */
  mulv  pk_status;                    /* mulv  status peloton ingreep                                       */
  boolv buffervol;                    /* boolv buffers voor peloton ingreep vol                             */
};

struct fietsvoorrang {
  count fc;                           /* FC    fietsrichting                                                */
  count drk1;                         /* DE    drukknop 1 voor aanvraag prioriteit                          */
  count drk2;                         /* DE    drukknop 2 voor aanvraag prioriteit                          */
  count de1;                          /* DE    koplus   1 voor aanvraag prioriteit                          */
  count de2;                          /* DE    koplus   2 voor aanvraag prioriteit                          */
  count inmeld;                       /* HE    hulp element voor prioriteitsmodule (in.melding prioriteit)  */
  count uitmeld;                      /* HE    hulp element voor prioriteitsmodule (uitmelding prioriteit)  */
  count ogwt_fts;                     /* TM    ondergrens wachttijd voor prioriteit                         */
  count prio_fts;                     /* PRM   prioriteitscode                                              */
  count ogwt_reg;                     /* TM    ondergrens wachttijd voor prioriteit (indien REGEN == TRUE)  */
  count prio_reg;                     /* PRM   prioriteitscode                      (indien REGEN == TRUE)  */
  count verklik;                      /* US    verklik fiets prioriteit                                     */
  boolv aanvraag;                     /* boolv fietser is op juiste wijze aangevraagd                       */
  boolv prio_vw;                      /* boolv fietser voldoet aan prioriteitsvoorwaarden                   */
  boolv prio_av;                      /* boolv fietser is met prioriteit aangevraagd                        */
};

struct prioriteit_id {
  count HD;                           /* count hulpdienst ingreep                                           */
  count OV_kar;                       /* count OV ingreep - KAR                                             */
  count OV_srm;                       /* count OV ingreep - SRM                                             */
  count OV_verlos;                    /* count OV ingreep - verlos                                          */
  count VRW;                          /* count VRW ingreep                                                  */
  count FTS;                          /* count fiets voorrang module                                        */
};

struct afteller {
  count fc;                           /* FC    richting met afteller                                        */
  count de1;                          /* DE    koplus 1                                                     */
  count de2;                          /* DE    koplus 2                                                     */
  count de3;                          /* DE    koplus 3                                                     */
  count toest;                        /* SCH   toestemming aansturing afteller                              */
  count min_duur;                     /* PRM   minimale duur tot start groen waarbij afteller mag starten   */
  count tel_duur;                     /* PRM   duur van een tel in tienden van seconden                     */
  count us_getal;                     /* US    tbv verklikking op bedienpaneel                              */
  count us_bit0;                      /* US    aansturing afteller BIT0                                     */
  count us_bit1;                      /* US    aansturing afteller BIT1                                     */
  boolv aftel_ok;                     /* boolv alle aftellers van een rijrichting zijn OK                   */
  mulv  act_duur;                     /* mulv  tijd in tienden van seconden dat TEL wordt aangestuurd       */
};

#ifndef __TRAFFICK2TLCGEN_VAR
extern mulv  aantal_hki_kop;          /* aantal harde koppelingen                                           */
extern mulv  aantal_vtg_tgo;          /* aantal voetgangerskoppelingen - type gescheiden oversteek          */
extern mulv  aantal_lvk_gst;          /* aantal gelijk starten langzaam verkeer                             */
extern mulv  aantal_dcf_vst;          /* aantal deelconflicten voorstart                                    */
extern mulv  aantal_pel_kop;          /* aantal peloton koppelingen                                         */
extern mulv  aantal_aft_123;          /* aantal definities aftellers                                        */

extern mulv  REALtraffick[FCMAX];     /* REALTIJD voor richtingen die als eerstvolgende aan de beurt zijn   */
extern mulv  PARtraffick[FCMAX];      /* buffer PAR[] zoals bepaald door Traffick                           */
extern boolv AAPRprio[FCMAX];         /* AAPR[] voor prioriteitsrealisaties                                 */
extern mulv  AltRuimte[FCMAX];        /* realisatie ruimte voor alternatieve realisatie                     */
extern boolv ART[FCMAX];              /* alternatieve realisatie toegestaan algemene schakelaar             */
extern mulv  ARB[FCMAX];              /* alternatieve realisatie toegestaan verfijning per blok             */
extern boolv MGR[FCMAX];              /* meeverleng groen                                                   */
extern boolv MMK[FCMAX];              /* meeverleng groen alleen als MK[] waar is                           */
extern boolv BMC[FCMAX];              /* beeindig meeverleng groen conflicten                               */
extern boolv WGR[FCMAX];              /* wachtstand groen                                                   */
extern boolv FC_DVM[FCMAX];           /* richting krijgt hogere hiaattijden toebedeeld                      */
extern mulv  AR_max[FCMAX];           /* alternatief maximum                                                */
extern mulv  GWT[FCMAX];              /* gewogen wachttijd tbv toekennen alternatieve realisatie            */
extern mulv  TEG[FCMAX];              /* tijd tot einde groen                                               */
extern mulv  MTG[FCMAX];              /* minimale tijd tot groen                                            */
extern mulv  mmk_old[FCMAX];          /* buffer MM[mmk] - geheugenelement MK[] bij gescheiden hiaatmeting   */
extern mulv  MK_old[FCMAX];           /* buffer MK[]                                                        */
extern mulv  TMPc[FCMAX][FCMAX];      /* tijdelijke conflict matrix                                         */
extern mulv  TMPi[FCMAX][FCMAX];      /* restant fictieve ontruimingstijd                                   */

extern boolv DOSEER[FCMAX];           /* doseren aktief               (zelf te besturen in REG[]ADD)        */
extern mulv  MINTSG[FCMAX];           /* minimale tijd tot startgroen (zelf te besturen in REG[]ADD)        */
extern mulv  PELTEG[FCMAX];           /* tijd tot einde groen als peloton ingreep maximaal duurt            */

extern mulv  TVG_instelling[FCMAX];   /* buffer ingestelde waarde TVG_max[]                                 */
extern mulv  TGL_instelling[FCMAX];   /* buffer ingestelde waarde TGL_max[]                                 */

extern mulv  Waft[FCMAX];             /* aftellerwaarde ( > 0 betekent dat 1-2-3 afteller loopt)            */
extern mulv  Aled[FCMAX];             /* aantal resterende leds bij wachttijd voorspeller                   */
extern mulv  AanDuurLed[FCMAX];       /* tijd dat huidige aantal leds wordt uitgestuurd                     */
extern mulv  TijdPerLed[FCMAX];       /* tijdsduur per led voor gelijkmatige afloop wachttijd voorspeller   */
extern mulv  wacht_ML[FCMAX];         /* maximale wachttijd volgens de module molen                         */

extern mulv  ARM[FCMAX];              /* kruispunt arm tbv HLPD prioriteit                                  */
extern mulv  volg_ARM[FCMAX];         /* kruispunt volgarm tbv doorzetten HLPD prioriteit                   */
extern boolv HD_aanwezig[FCMAX];      /* HLPD aanwezig op richting                                          */
extern boolv HLPD[FCMAX];             /* HLPD prioriteit toegekend aan richting (en volgrichtingen)         */
extern mulv  NAL_HLPD[FCMAX];         /* nalooptijd hulpdienst ingreep op richting(en) in volgarm           */
extern mulv  verlos_busbaan[FCMAX];   /* buffer voor verlosmelding met prioriteit                           */
extern boolv iPRIO[FCMAX];            /* prioriteit toegekend aan richting                                  */

extern mulv  PEL_UIT_VTG[FCMAX];      /* buffer aantal voertuig voor uitgaande peloton koppeling            */
extern mulv  PEL_UIT_RES[FCMAX];      /* restant minimale duur uitsturing koppelsignaal peloton koppeling   */

extern mulv  verklik_srm;             /* restant duur verklikking SRM bericht                               */
extern mulv  duur_geen_srm;           /* aantal minuten dat geen SRM bericht is ontvangen (maximum = 32000) */

extern boolv RAT[FCMAX];              /* aansturing rateltikker                                             */
extern boolv KNIP;                    /* hulpwaarde voor knipper signaal                                    */
extern boolv REGEN;                   /* regensensor aktief (zelf te besturen in REG[]ADD)                  */
extern boolv WT_TE_HOOG;              /* wachttijd te hoog voor toekennen prioriteit                        */
extern boolv GEEN_OV_PRIO;            /* geen prioriteit OV   (zelf te besturen in REG[]ADD)                */
extern boolv GEEN_VW_PRIO;            /* geen prioriteit VW   (zelf te besturen in REG[]ADD)                */
extern boolv GEEN_FIETS_PRIO;         /* geen fietsprioriteit (zelf te besturen in REG[]ADD)                */

extern boolv DF[DPMAX];               /* detectie fout aanwezig                                             */
extern mulv  D_bez[DPMAX];            /* tijdsduur detector bezet                                           */
extern mulv  D_onb[DPMAX];            /* tijdsduur detector onbezet                                         */
extern boolv TDH_DVM[DPMAX];          /* status TDH tijdens DVM                                             */

extern struct hki_koppeling hki_kop[MAX_HKI_KOP];
extern struct vtg_koppeling vtg_tgo[MAX_VTG_KOP];
extern struct lvk_gelijkstr lvk_gst[MAX_LVK_GST];
extern struct dcf_voorstart dcf_vst[MAX_DCF_VST];
extern struct pel_koppeling pel_kop[MAX_PEL_KOP];
extern struct fietsvoorrang fts_pri[MAX_FTS_PRI];
extern struct prioriteit_id prio_index[FCMAX];
extern struct afteller      aft_123[FCMAX];

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
extern char  _UUR[MAXDUMPSTAP];        /* bijhouden UUR tbv flight buffer                                    */
extern char  _MIN[MAXDUMPSTAP];        /* bijhouden MIN tbv flight buffer                                    */
extern char  _SEC[MAXDUMPSTAP];        /* bijhouden SEC tbv flight buffer                                    */
extern mulv  _ML[MAXDUMPSTAP];         /* bijhouden ML  tbv flight buffer                                    */
extern char  _FC[FCMAX][MAXDUMPSTAP];  /* bijhouden fasecyclus status tbv flight buffer                      */
extern char  _FCA[FCMAX][MAXDUMPSTAP]; /* bijhouden aanvraag   status tbv flight buffer                      */
extern boolv _HA[FCMAX];               /* hulpwaarde A[] tbv start- en einde puls aanvraag in flight buffer  */
extern mulv  dumpstap;                 /* interne teller flight buffer                                       */
#endif
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie initialiseer variabelen Traffick2TLCGen                                                          */
/* -------------------------------------------------------------------------------------------------------- */
/* In deze functie worden alle toegevoegde variabelen geinitialiseerd.                                      */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_init_application().                                                */
/*                                                                                                          */
void init_traffick2tlcgen(void);      /* Fik230101                                                          */


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
count tlr21,                          /* TM    late release fc2 (= inrijtijd fc1)                           */
count tnlfg12,                        /* TM    vaste   nalooptijd vanaf einde vastgroen      fc1            */
count tnlfgd12,                       /* TM    det.afh.nalooptijd vanaf einde vastgroen      fc1            */
count tnleg12,                        /* TM    vaste   nalooptijd vanaf einde (verleng)groen fc1            */
count tnlegd12,                       /* TM    det.afh.nalooptijd vanaf einde (verleng)groen fc1            */
boolv kop_eg,                         /* boolv koppeling vanaf einde groen (als FALSE dan vanaf EV)         */
boolv los_fc2,                        /* boolv fc2 mag bij aanvraag fc1 los realiseren                      */
mulv  kop_max);                       /* mulv  maximum verlenggroen na harde koppeling                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer voetgangerskoppeling - type gescheiden oversteek                                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de gegevens van voetgangerskoppelingen in een struct geplaatst.       */
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
count hlos2);                         /* HE los realiseren fc2 toegestaan                                   */


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
count fc4);                           /* FC richting 4                                                      */


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
count mv21);                          /* SCH meeverlengen  van fc2 met fc1                                  */


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
count aanv_vert,                      /* TM  aanvraag vertraging  (start op binnenkomst koppelsignaal)      */
count vast_vert,                      /* TM  vasthoud vertraging  (start op binnenkomst koppelsignaal)      */
count duur_vast,                      /* TM  duur vasthouden (bij duursign. na afvallen koppelsignaal)      */
count duur_verl);                     /* TM  duur verlengen na ingreep (bij NG geldt TVG_max[])             */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer fiets voorrang module                                                                  */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de gegevens van de fiets voorrang module in een struct geplaatst.     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_init_application().                                                */
/*                                                                                                          */
void definitie_fiets_voorrang(        /* Fik230101                                                          */
count fc,                             /* FC    fietsrichting                                                */
count drk1,                           /* DE    drukknop 1 voor aanvraag prioriteit                          */
count drk2,                           /* DE    drukknop 2 voor aanvraag prioriteit                          */
count de1,                            /* DE    koplus   1 voor aanvraag prioriteit                          */
count de2,                            /* DE    koplus   2 voor aanvraag prioriteit                          */
count inmeld,                         /* HE    hulp element voor prioriteitsmodule (in.melding prioriteit)  */
count uitmeld,                        /* HE    hulp element voor prioriteitsmodule (uitmelding prioriteit)  */
count ogwt,                           /* TM    ondergrens wachttijd voor prioriteit                         */
count prio,                           /* PRM   prioriteitscode                                              */
count ogwt_reg,                       /* TM    ondergrens wachttijd voor prioriteit (indien REGEN == TRUE)  */
count prio_reg,                       /* PRM   prioriteitscode                      (indien REGEN == TRUE)  */
count verklik);                       /* US                                        */


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
count us_bit1);                       /* US    aansturing afteller BIT1                                     */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bijwerken kruispunt variabelen Traffick2TLCGen                                                   */
/* -------------------------------------------------------------------------------------------------------- */
/* In deze functie worden de kruispunt variabelen bijgewerkt, te weten:                                     */
/* KNIP      : tbv knipperend aansturen leds op het BP                                                      */
/* WT_TE_HOOG: wachttijd te hoog voor prioriteit                                                            */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PreApplication_Add().                                                   */
/*                                                                                                          */
void traffick2tlcgen_kruispunt(void); /* Fik230101                                                          */


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
void traffick2tlcgen_detectie(void);  /* Fik230101                                                          */


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
count is);                            /* IS ingang signaal waarop snelheid wordt afgebeeld                  */


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
mulv  wg_aanvraag,                    /* boolv wachtstand groen aanvraag                                    */
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
mulv  prioVRW_index,                  /* mulv  VRWFCfc - prioriteitsindex fiets vrachtwagen ingreep         */
mulv  prioFTS_index);                 /* mulv  ftsFCfc - prioriteitsindex fiets voorrang module             */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal aanvraag fiets voorrang module                                                            */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt of de fietsrichting met prioriteit kan worden aangevraagd.                          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Aanvragen_Add().                                                        */
/*                                                                                                          */
void fiets_voorrang_aanvraag(void);   /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie werk TempConflict matrix bij                                                                     */
/* -------------------------------------------------------------------------------------------------------- */
/* Richtingen die met elkaar in deelconflict zijn afhankelijk van het actuele lichtbeeld conflicterend met  */
/* elkaar. Deze functie bepaalt alle tijdelijke conflicten en stelt deze beschikbaar via matrix TMPc[][].   */
/* De matrix dient gelezen te worden als TMPc[FCvan][FCnaar].                                               */
/*                                                                                                          */
/*  NG: niet conflicterend                                                                                  */
/*   0: conflicterend met een intergroentijd                                                                */
/*  FK: fictief conflicterend                                                                               */
/*  GK: conflicterend zonder  intergroentijd                                                                */
/* GKL: conflicterend zonder  intergroentijd waarbij richting "van" een conflicterende volgrichting heeft.  */
/*  >0: niet conflicterend maar intergroentijd loopt (waarde is restant van de lopende intergroentijd)      */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit RealTraffick().                                                         */
/*                                                                                                          */
void TempConflictMatrix(void);        /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal actuele resterende maximale ontuimingstijd veroorzaakt door deelconflicten            */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als RETURN de resterende maximale ontruimingstijd veroorzaakt door deelconflicten.       */
/*                                                                                                          */
mulv TMP_ontruim(                     /* Fik230101                                                          */
count fc);                            /* FC fasecyclus                                                      */


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
void StatusHKI(void);                 /* Fik230101                                                          */


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
void BepaalTEG_pri(                   /* Fik230101                                                          */
count volgnr);                        /* index prioriteit                                                   */


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
count volgnr);                        /* index harde koppeling                                              */


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
count volgnr);                        /* index harde koppeling                                              */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal tijd tot einde groen                                                                      */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie berekent de tijd tot einde groen als de richting volledig uitverlengt.                           */
/* Resultaat wordt weggeschreven in TEG[].                                                                  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit RealTraffick().                                                         */
/*                                                                                                          */
void BepaalTEG(void);                 /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal minimale tijd tot realisatie                                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaalt de minimale tijd tot groen voor alle richtingen.                                         */
/* Resultaat wordt weggeschreven in MTG[].                                                                  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit RealTraffick().                                                         */
/*                                                                                                          */
void BepaalMTG(void);                 /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal tijd tot realisatie                                                                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie die de REALTIJD van TLCgen corrigeert voor richtingen die als eerstvolgende aan de beurt zijn.   */
/* Resultaat wordt weggeschreven in REALtraffick[].                                                         */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit BepaalRealisatieTijden_Add().                                           */
/*                                                                                                          */
void RealTraffick(void);              /* Fik230101                                                          */


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
void Traffick2TLCgen_NAL(void);       /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verwerk prioriteitsrealisaties in RealTraffick[] en TEG[]                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie verwerkt de informatie van prioriteitsrealisaties die zijn ingepland in RealTraffick[] en TEG[]  */
/* van de conflictrichtingen.                                                                               */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit BepaalAltRuimte().                                                      */
/*                                                                                                          */
void RealTraffickPrioriteit(void);    /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal realisatie ruimte voor alternatieve realisaties                                           */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaalt de alternatieve realisatie ruimte bepaalt voor alle richtingen.                          */
/* Resultaat wordt weggeschreven in AltRuimte[].                                                            */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Maxgroen_Add().                                                         */
/*                                                                                                          */
void BepaalAltRuimte(void);           /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal of voor een conflicterende richting een REALtraffick[] is afgegeven                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie controleert of een fasecyclus een conflicterende richting heeft waarvan REALtraffick[]      */
/* bekend (> NG) is. Als dit het geval is wordt een wachtstand aanvraag gereset.                            */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_WGR().                                                  */
/*                                                                                                          */
boolv REALconflict(                   /* Fik230101                                                          */
count fc);                            /* FC fasecyclus                                                      */


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
void Traffick2TLCgen_WGR(void);       /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verwerk extra meetkriterium detector - geen gescheiden hiaatmeting per rijstrook                 */
/* -------------------------------------------------------------------------------------------------------- */
/* In TLCgen kan een detector geen twee meetkriteria aanhouden. Deze functie maakt dit wel mogelijk.        */
/* Het meetkriterium wordt door deze functie gecorrigeerd.                                                  */
/* De functie is afgeleid van meetkriterium_prm_va_arg() uit het standaard CCOL pakket.                     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Meetkriterium_Add().                                                    */
/*                                                                                                          */
void corrigeer_meetkriterium_va_arg(count fc, count tkopmaxnr, ...);                 /* TvG                 */
                                                                                     /* wijziging Fik230101 */

/* -------------------------------------------------------------------------------------------------------- */
/* Functie verwerk extra meetkriterium detector - gescheiden hiaatmeting per rijstrook                      */
/* -------------------------------------------------------------------------------------------------------- */
/* In TLCgen kan een detector geen twee meetkriteria aanhouden. Deze functie maakt dit wel mogelijk.        */
/* Zowel het meetkriterium als het bijbehorende geheugenelement worden door deze functie gecorrigeerd.      */
/* De functie is afgeleid van meetkriterium2_prm_va_arg() uit het standaard CCOL pakket.                    */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Meetkriterium_Add().                                                    */
/*                                                                                                          */
void corrigeer_meetkriterium2_det_va_arg(count fc, count tkopmaxnr, count mmk, ...); /* TvG                 */
                                                                                     /* wijziging Fik230101 */

/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal maximaal meeverlengen                                                                     */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt of richtingen nog kunnen meeverlengen op basis van "maatgevend groen". Indien dit   */
/* niet meer het geval is wordt YM[] BIT4 gereset.                                                          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Meeverlengen_Add().                                                     */
/*                                                                                                          */
void Traffick2TLCgen_MVG(void);       /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal uitstel                                                                                   */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt het uitstel voor deelconflicten en koppelingen. De functie maakt hiervoor gebruik   */
/* van X[] BIT3 en RR[] BIT3.                                                                               */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Synchronisaties_Add().                                                  */
/*                                                                                                          */
void Traffick2TLCgen_uitstel(void);   /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal periode alternatieve realisatie                                                           */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt de periode waarin alternatief realiseren is toegestaan. De functie maakt gebruik    */
/* van de realisatie ruimte zoals bepaald in REALTraffick(). De functie overschrijft PAR[] zoals bepaald    */
/* door TLCgen.                                                                                             */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Alternatief_Add().                                                      */
/*                                                                                                          */
void Traffick2TLCgen_PAR(void);       /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie beeindig alternatieve realisatie                                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie beeindigt een alternatieve realisatie indien alternatieve ruimte niet meer aanwezig is.     */
/* De functie overschrijft RR[] BIT5 en FM[] BIT5 zoals bepaald door TLCgen.                                */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Alternatief_Add().                                                      */
/*                                                                                                          */
void BeeindigAltRealisatie(void);     /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer prio bit retourrood bij harde koppelingen                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCgen schrijft retourrood van volgrichting terug naar de voedende richting. Hier zit een bug omdat de   */
/* voeding ook retourrood krijgt als de volgrichting een conflicterende prioriteitsaanvraag heeft die geen  */
/* prioriteitsrealisatie gaat krijgen. Deze functie verzorgt de bugfix. (reset RR[] BIT10)                  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Modules_Add().                                                          */
/*                                                                                                          */
void BugFix_RR_bij_HKI(void);         /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie forceer realisatie van richtingen met voorstart                                                  */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie forceert de richting met een voorstart naar groen indien de hoofdrichting daarop wacht.     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_REA().                                                  */
/*                                                                                                          */
void Traffick2TLCgen_REA_VST(void);   /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer realisatie afhandeling TLCgen                                                          */
/* -------------------------------------------------------------------------------------------------------- */
/* Voor TLCgen is alleen PAR[] onvoldoende om ook daadwerkelijk te realiseren (= overgang naar RA[]). Deze  */
/* functie corrigeert dit. Daarnaast zorgt de functie ervoor dat er nooit twee conflicterende richtingen in */
/* gewenst groen (= RA[] tot en met VG[]) komen. Indien nodig wordt AA[] hiervoor gereset of RR[] opgezet.  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit RealisatieAfhandeling_Add().                                            */
/*                                                                                                          */
void Traffick2TLCgen_REA(void);       /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende OV ingreep tbv wachttijd voorspeller                      */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie geeft de aanwezigheid terug van een conflicterende busingreep tbv de wachttijd voorspeller. */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit aansturing_wt_voorspeller().                                            */
/*                                                                                                          */
boolv conflict_OV(                    /* Fik230101                                                          */
count fc);                            /* FC fasecyclus                                                      */


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
count fc);                            /* FC fasecyclus                                                      */


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
count usbus);                         /* US aansturing bus sjabloon                                         */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie aansturing afteller                                                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de aansturing van de aftellers.                                                    */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void aansturing_aftellers(void);      /* Fik230101                                                          */


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
mulv  ratel_werking);                 /* mulv  werkingsparmeter rateltikkers                                */


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
count uswacht);                       /* US wachtlicht uitsturing                                           */


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
count us_ks);                         /* US    uitgaand koppelsignaal                                       */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie prioriteitsafhandeling fiets voorrang module                                                     */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de prioriteitsafhandeling van de fiets voorrang module. Indien de fietsrichting    */
/* geblokkeerd wordt of indien de wachttijd te hoog oploopt kan de "uitmelding" tijdens rood plaatsvinden.  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit InUitMelden_Add().                                                      */
/*                                                                                                          */
void fiets_voorrang_module(void);     /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verklik fiets voorrang module                                                                    */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de verklikking van de fiets voorrang module. Het led knippert tijdens rood indien  */
/* een prioriteitsaavraag aanwezig is en brandt vervolgens vast tot einde vastgroen van de fietsrichting.   */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void verklik_fiets_voorrang(void);    /* Fik230101                                                          */


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
mulv  min_rood);                      /* mulv minimale roodtijd (TE) voor prioriteit aanvraag               */


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
void corrigeer_blokkeringstijd_OV(void); /* Fik230101                                                       */


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
void corrigeer_maximum_wachttijd_OV(void); /* Fik230101                                                     */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal hulpdienst ingreep                                                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de hulpdienst ingreep op alle richtingen van de gedefinieerde kruispuntarm en      */
/* eventuele gedefinieerde volgarmen.                                                                       */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_PRIO_OPTIES().                                          */
/*                                                                                                          */
void Traffick2TLCgen_HLPD(void);      /* Fik230101                                                          */


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
mulv  naloop);                        /* mulv nalooptijd                                                    */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal peloton ingreep                                                                           */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie blokkeert conflicterende prioriteitsingrepen bij een aktieve peloton ingreep.               */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_PRIO_OPTIES().                                          */
/*                                                                                                          */
void Traffick2TLCgen_PELOTON(void);   /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal prioriteitsopties fiets voorrang module                                                   */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie zet de prioriteitsopties voor de fiets voorrang module conform de instellingen afhankelijk  */
/* van de status van de regensensor. De functie corrigeert de opties zodra een conflicterende prioriteit    */
/* is aangevraagd met als optie bijzondere realisatie. De fietsprioriteit is hieraan altijd ondergeschikt.  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_PRIO_OPTIES().                                          */
/*                                                                                                          */
void Traffick2TLCgen_FIETS(void);     /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer prioriteitsopties                                                                      */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie corrigeert prioriteitsopties als gevolg van verschillende (en onderling conflicterende)     */
/* prioriteitsingrepen en voor wachttijdvoorspellers waarvan minder dan een instelbaar aantal leds branden. */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PrioriteitsOpties_Add().                                                */
/*                                                                                                          */
void Traffick2TLCgen_PRIO_OPTIES(void); /* Fik230101                                                        */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer prioriteitstoekenning                                                                  */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCgen laat de prioriteitstoekenning AAN staan indien de prioriteitsopties (tijdelijk) worden uitgezet.  */
/* Deze functie corrigeert dit en reset in dat geval iPrioriteit[].                                         */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PrioriteitsToekenning_Add().                                            */
/*                                                                                                          */
void Traffick2TLCgen_PRIO_TOE(void);  /* Fik230101                                                          */


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
void Traffick2TLCpas_TVG_aan(void);   /* Fik230101                                                          */


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
void Traffick2TLCzet_TVG_terug(void); /* Fik230101                                                          */


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
void Traffick2TLCgen_PRIO_RR(void);   /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer toestemming alternatieve realisatie tijdens een prioriteitsrealisatie                  */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCgen staat een alternatieve realisaties toe indien er voldoende ruimte is als gevolg van een           */
/* prioriteitsrealisatie van een niet conflicterende richting. Dit mag echter niet gebeuren voor richtingen */
/* die gedoseerd worden of indien Traffick heeft uitgerekend dat een alternatieve realisatie niet past.     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PrioAlternatieven_Add().                                                */
/*                                                                                                          */
void Traffick2TLCgen_PRIO_PAR(void);  /* Fik230101                                                          */


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
void Traffick2TLCgen_PRIO(void);        /* Fik230101                                                        */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal TI_max[][]                                                                            */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de intergroentijd van een conflict.                                    */
/*                                                                                                          */
mulv TI_max(                          /* Fik230101                                                          */
count fc1,                            /* FC fasecyclus 1                                                    */
count fc2);                           /* FC fasecyclus 2                                                    */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal TI[][]                                                                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde het aflopen van de intergroentijd.                                     */
/*                                                                                                          */
boolv TI(                             /* Fik230101                                                          */
count fc1,
count fc2);


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal TI_timer[]                                                                            */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de intergroen timer van een fasecyclus.                                */
/*                                                                                                          */
mulv TI_timer(                        /* Fik230101                                                          */
count fc);


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal of twee richtingen conflicterend zijn                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de intergroentijd (van fc1 naar fc2) als deze richtingen conflicterend */
/* zijn. Indien de richtingen niet conflicterend zijn is de return waarde NG.                               */
/*                                                                                                          */
mulv GK_conflict(                     /* Fik230101                                                          */
count fc1,                            /* FC fasecyclus 1                                                    */
count fc2);                           /* FC fasecyclus 2                                                    */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal of twee richtingen (fictief)conflicterend zijn                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde TRUE als twee richtingen (tijdelijk) (fictief)conflicterend zijn.      */
/*                                                                                                          */
boolv FK_conflict(                    /* Fik230101                                                          */
count fc1,                            /* FC fasecyclus 1                                                    */
count fc2);                           /* FC fasecyclus 2                                                    */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal conflicterende richting met een aanvraag                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende richting met een aanvraag.      */
/* Het verschil met de CCOLfunctie ka() is dat ook tijdelijke conflicten worden getest op een aanvraag.     */
/*                                                                                                          */
boolv tka(                            /* Fik230101                                                          */
count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid (fictief)conflicterende richting in RA[]                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een (fictief)conflicterende richting in RA[].      */
/*                                                                                                          */
boolv fkra(                           /* Fik230101                                                          */
count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid (fictief)conflicterende richting in RA[] met P[]                         */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een (fictief)conflicterende ri. in RA[] met P[].   */
/*                                                                                                          */
boolv fkrap(                          /* Fik230101                                                          */
count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende richting met AAPR die (vooruit) kan realiseren            */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende richting met AAPR die (vooruit) */
/* kan realiseren. (geen AAPR[] BIT5)                                                                       */
/*                                                                                                          */
boolv kaapr(                          /* Fik230101                                                          */
count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal (fictief)conflicterende richting in AA[].                                             */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een (fictief)conflicterende richting in AA[].      */
/* Het verschil met de CCOLfunctie fkaa() is dat ook tijdelijke (fictieve)conflicten worden getest op AA[]. */
/*                                                                                                          */
boolv tfkaa(                          /* Fik230101                                                          */
count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal conflicterende richting in CV[].                                                      */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende richting in CV[]. Het verschil  */
/* met de CCOLfunctie kcv() is dat ook tijdelijke conflicten worden getest op CV[].                         */
/*                                                                                                          */
boolv tkcv(                           /* Fik230101                                                          */
count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende prioriteitsingreep welke nog niet is gerealiseerd         */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende prioriteitsingreep welke nog    */
/* niet is gerealiseerd. (nog niet groen is)                                                                */
/*                                                                                                          */
boolv conflict_prio_real(             /* Fik230101                                                          */
count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende hulpdienst ingreep                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende hulpdienst ingreep.             */
/*                                                                                                          */
boolv khlpd(                          /* Fik230101                                                          */
count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal of een richting een volgrichting is binnen een gedefinieerde harde koppeling          */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als RETURN of een richting een volgrichting is binnen een gedefinieerde harde koppeling. */
/*                                                                                                          */
boolv volgrichting_hki(               /* Fik230101                                                          */
count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal of een richting een voorstart moet geven binnen een gedefinieerd deelconflict         */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als RETURN of een richting een voorstart moet geven aan een andere richting.             */
/*                                                                                                          */
boolv voorstart_gever(                /* Fik230101                                                          */
count fc);                            /* FC fasecyclus                                                      */


#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
/* -------------------------------------------------------------------------------------------------------- */
/* Functie stiptheid in testomgeving                                                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie die op basis van de test ingangen IS[isvroeg] en IS[islaat] zorgt dat de DSI berichten in de */
/* testomgeving zorgen voor de gewenste stiptheid.                                                          */
/*                                                                                                          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PreApplication_Add().                                                   */
/*                                                                                                          */
void test_stiptheid(                  /* Fik230101                                                          */
count ov_tevroeg,                     /* PRM ingestelde grens voor te vroeg in seconden                     */
count ov_telaat,                      /* PRM ingestelde grens voor te laat  in seconden                     */
count tst_stiptheid);                 /* PRM waarde stiptheid voor DSI berichten in de testomgeving         */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie aanmaken dumpfile in testomgeving                                                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie maakt bij fase bewaking een dumpfile aan met applicatie gegevens.                                */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_dump_application().                                                */
/*                                                                                                          */
void DumpTraffick(void);              /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie update flight buffer in testomgeving                                                             */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie werkt iedere seconde het flight buffer bij tbv analyse van fase bewakingen in de testomgeving.   */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PostApplication_Add().                                                  */
/*                                                                                                          */
void FlightTraffick(void);            /* Fik230101                                                          */

#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie toggle FK conflicten                                                                             */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie schakelt FK conflicten, bijvoorbeeld voor het schakelbaar maken van voetgangerskoppelingen. */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PostApplication_Add().                                                  */
/* (na het schakelen van FK conflicten geen andere programma code opnemen)                                  */
/*                                                                                                          */
void Toggle_FK_Conflict(              /* Fik230101                                                          */
count fc1,                            /* FC    fasecyclus 1                                                 */
count fc2,                            /* FC    fasecyclus 2                                                 */
boolv period);                        /* boolv FK conflict gewenst                                          */

#endif
