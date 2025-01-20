/* -------------------------------------------------------------------------------------------------------- */
/* Traffick2TLCGen                                                               Versie 1.0.1 / 14 sep 2023 */
/* -------------------------------------------------------------------------------------------------------- */

/* Deze include file bevat hulp functies voor verkeerskundige Traffick functionaliteiten.                   */
/* Deze functies zijn ontwikkeld en geschreven door Marcel Fick.                                            */
/* Versie: 1.0.0                                                                                            */
/* Datum:  1 januari 2023                                                                                   */

/* Versie: 1.0.1                                                                                            */
/* Datum:  14 september 2023                                                                                */
/* Bugfix:                                                                                                  */
/*         Herstarten ontruimingstijd naar langzaam verkeer bij roodlicht negatie                           */
/*         Uitstel voetganger bij einde werkingsperiode rateltikker                                         */
/*         Controle op NG bij uitsturen contacten WTV ivm mogelijke seriele aansturing                      */
/*         Controle op NG bij aanhouden MVG van richting met voorstart (afhandeling deelconflict)           */
/*                                                                                                          */
/* Wijziging:                                                                                               */
/*         Functie verklik_bewaak_SRM() kan onafhankelijk van het define NO_RIS worden aangeroepen          */
/*         Display aantal LEDs wtv in US[] en ME[]                                                          */

#ifndef __TRAFFICK2TLCGEN_H
#define __TRAFFICK2TLCGEN_H

#define MAX_HKI_KOP  FCMAX            /* maximum aantal harde koppelingen                                   */
#define MAX_VTG_KOP  FCMAX            /* maximum aantal voetgangerskoppelingen - type gescheiden oversteek  */
#define MAX_LVK_GST  FCMAX            /* maximum aantal gelijk starten langzaam verkeer                     */
#define MAX_DCF_VST  FCMAX            /* maximum aantal deelconflicten voorstart                            */
#define MAX_DCF_GST  FCMAX            /* maximum aantal deelconflicten gelijkstart                          */
#define MAX_MEE_REA     40            /* maximum aantal meerealisaties                                      */
#define MAX_PEL_KOP     10            /* maximum aantal peloton koppelingen                                 */
#define MAX_FTS_PRI     20            /* maximum aantal definities fiets voorrang module                    */
#define MAX_DVM_PRG     10            /* maximum aantal DVM netwerkprogramma's                              */
#define MAX_FILE_PRG    10            /* maximum aantal FILE programma's stroomopwaarts                     */
#define MAXDUMPSTAP    600            /* aantal seconden flight buffer in testomgeving                      */
#define MAX_INTEGER 32767L

struct hki_koppeling {
    count fc1;                          /* FC    voedende richting                                            */
    count fc2;                          /* FC    volg     richting                                            */
    count tlr21;                        /* TM    late release fc2 (= inrijtijd)                               */
    count tnlfg12;                      /* TM    vaste   nalooptijd vanaf einde vastgroen      fc1            */
    count tnlfgd12;                     /* TM    det.afh.nalooptijd vanaf einde vastgroen      fc1            */
    count tnleg12;                      /* TM    vaste   nalooptijd vanaf einde (verleng)groen fc1            */
    count tnlegd12;                     /* TM    det.afh.nalooptijd vanaf einde (verleng)groen fc1            */
    bool kop_eg;                       /* bool koppeling vanaf einde groen (als FALSE dan vanaf EV)         */
    bool los_fc2;                      /* bool fc2 mag bij aanvraag fc1 los realiseren                      */
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

struct dcf_gelijkstr {
    count fc1;                          /* FC  richting 1                                                     */
    count fc2;                          /* FC  richting 2                                                     */
    count to12;                         /* TM  ontruimingstijd van fc1 naar fc2                               */
    count to21;                         /* TM  ontruimingstijd van fc2 naar fc1                               */
    count ma12;                         /* SCH meerealisatie van fc1 met fc2                                  */
    count ma21;                         /* SCH meerealisatie van fc2 met fc1                                  */
};

struct meerealisatie {
    count fc1;                          /* FC    richting die meerealisatie geeft                             */
    count fc2;                          /* FC    richting die meerealisatie krijgt                            */
    count ma21;                         /* SCH   meerealisatie van fc2 met fc1                                */
    count mv21;                         /* SCH   meeverlengen  van fc2 met fc1                                */
    bool mr2v;                         /* bool meerealisatie aan fc2 verstrekt                              */
};

struct pel_koppeling {
    count kop_fc;                       /* FC    koppelrichting                                               */
    count kop_toe;                      /* ME    toestemming peloton ingreep (bij NG altijd toestemming)      */
    count kop_sig;                      /* HE    koppelsignaal                                                */
    count kop_bew;                      /* TM    bewaak koppelsignaal (bij NG wordt een puls veronderstelt)   */
    count aanv_vert;                    /* TM    aanvraag vertraging  (bij NG wordt geen aanvraag opgzet)     */
    count vast_vert;                    /* TM    vasthoud vertraging  (start op binnenkomst koppelsignaal)    */
    count duur_vast;                    /* TM    duur vasthouden (bij duursign. na afvallen koppelsignaal)    */
    count duur_verl;                    /* TM    duur verlengen na ingreep (bij NG geldt TVG_max[])           */
    count hnaloop_1;                    /* HE    voorwaarde herstart extra nalooptijd 1 (nalooplus 1)         */
    count tnaloop_1;                    /* TM    nalooptijd 1                                                 */
    count hnaloop_2;                    /* HE    voorwaarde herstart extra nalooptijd 2 (nalooplus 2)         */
    count tnaloop_2;                    /* TM    nalooptijd 2                                                 */
    count verklik;                      /* US    verklik peloton ingreep                                      */
    bool kop_oud;                      /* bool status koppelsignaal vorige machine slag                     */
    mulv  aanw_kop1;                    /* mulv  aanwezigheidsduur koppelsignaal 1 vanaf start puls           */
    mulv  duur_kop1;                    /* mulv  tijdsduur HOOG    koppelsignaal 1 igv duur signaal           */
    mulv  aanw_kop2;                    /* mulv  aanwezigheidsduur koppelsignaal 2 vanaf start puls           */
    mulv  duur_kop2;                    /* mulv  tijdsduur HOOG    koppelsignaal 2 igv duur signaal           */
    mulv  aanw_kop3;                    /* mulv  aanwezigheidsduur koppelsignaal 3 vanaf start puls           */
    mulv  duur_kop3;                    /* mulv  tijdsduur HOOG    koppelsignaal 3 igv duur signaal           */
    mulv  pk_status;                    /* mulv  status peloton ingreep                                       */
    bool pk_afronden;                  /* bool afronden lopende peloton ingreep                             */
    bool buffervol;                    /* bool buffers voor peloton ingreep vol                             */
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
    bool aanvraag;                     /* bool fietser is op juiste wijze aangevraagd                       */
    bool prio_vw;                      /* bool fietser voldoet aan prioriteitsvoorwaarden                   */
    bool prio_av;                      /* bool fietser is met prioriteit aangevraagd                        */
};

struct prioriteit_id {
    mulv  KAR_id_OV;                    /* mulv  KAR id openbaar vervoer                                      */
    mulv  KAR_id_HD;                    /* mulv  KAR id nood- en hulpdiensten                                 */
    count HD;                           /* count hulpdienst ingreep                                           */
    count OV_kar;                       /* count OV ingreep - KAR                                             */
    count OV_srm;                       /* count OV ingreep - SRM                                             */
    count OV_verlos;                    /* count OV ingreep - verlos                                          */
    count VRW;                          /* count VRW ingreep                                                  */
    count FTS;                          /* count fiets voorrang module                                        */
    count usHD;                         /* US    verklik HD ingreep                                           */
    count usOV_kar;                     /* US    verklik OV ingreep - KAR                                     */
    count usOV_srm;                     /* US    verklik OV ingreep - SRM                                     */
    count usVRW;                        /* US    verklik VRW ingreep                                          */
};

struct afteller {
    count fc;                           /* FC    richting met afteller                                        */
    count de1;                          /* DE    koplus 1                                                     */
    count de2;                          /* DE    koplus 2                                                     */
    count de3;                          /* DE    koplus 3                                                     */
    count toest;                        /* SCH   toestemming aansturing afteller                              */
    count min_duur;                     /* PRM   minimale duur tot start groen waarbij afteller mag starten   */
    count tel_duur;                     /* PRM   duur van een tel in tienden van seconden                     */
    count is_oke_1;                     /* IS    afteller lantaarn 1 werkt correct                            */
    count is_oke_2;                     /* IS    afteller lantaarn 2 werkt correct                            */
    count is_oke_3;                     /* IS    afteller lantaarn 3 werkt correct                            */
    count is_oke_4;                     /* IS    afteller lantaarn 4 werkt correct                            */
    count us_oke;                       /* US    aftellers van alle lantaarns werken correct                  */
    count us_getal;                     /* US    tbv verklikking op bedienpaneel                              */
    count us_bit0;                      /* US    aansturing afteller BIT0                                     */
    count us_bit1;                      /* US    aansturing afteller BIT1                                     */
    bool aftel_ok;                     /* bool alle aftellers van een rijrichting zijn OK                   */
    mulv  act_tel;                      /* mulv  actuele stand afteller                                       */
    mulv  act_duur;                     /* mulv  tijd in tienden van seconden dat TEL wordt aangestuurd       */
};

struct max_groen_DVM {
    count fc;                           /* FC  fasecyclus                                                     */
    count dvm_set[MAX_DVM_PRG];         /* PRM maximum(verleng)groen tijdens DVM netwerk programma's          */
};

struct max_groen_FILE {
    count fc;                           /* FC  fasecyclus                                                     */
    count file_set[MAX_FILE_PRG];       /* PRM maximum(verleng)groen tijdens FILE programma's stroomopwaarts  */
};

extern mulv  aantal_hki_kop;          /* aantal harde koppelingen                                           */
extern mulv  aantal_vtg_tgo;          /* aantal voetgangerskoppelingen - type gescheiden oversteek          */
extern mulv  aantal_lvk_gst;          /* aantal gelijk starten langzaam verkeer                             */
extern mulv  aantal_dcf_vst;          /* aantal deelconflicten voorstart                                    */
extern mulv  aantal_dcf_gst;          /* aantal deelconflicten gelijkstart                                  */
extern mulv  aantal_mee_rea;          /* aantal meerealisaties                                              */
extern mulv  aantal_pel_kop;          /* aantal peloton koppelingen                                         */
extern mulv  aantal_aft_123;          /* aantal definities aftellers                                        */
extern mulv  aantal_dvm_prg;          /* aantal DVM netwerk programma's                                     */
extern mulv  aantal_file_prg;         /* aantal FILE programma's (stroomopwaarts)                           */

extern mulv  REALtraffick[FCMAX];     /* REALTIJD voor richtingen die als eerstvolgende aan de beurt zijn   */
extern bool PARtraffick[FCMAX];      /* buffer PAR[] zoals bepaald door Traffick                           */
extern bool AAPRprio[FCMAX];         /* AAPR[] voor prioriteitsrealisaties                                 */
extern mulv  PFPRtraffick[FCMAX];     /* aantal modulen dat vooruit gerealiseerd mag worden                 */
extern mulv  AltRuimte[FCMAX];        /* realisatie ruimte voor alternatieve realisatie                     */
extern bool ART[FCMAX];              /* alternatieve realisatie toegestaan algemene schakelaar             */
extern mulv  ARB[FCMAX];              /* alternatieve realisatie toegestaan verfijning per blok             */
extern bool MGR[FCMAX];              /* meeverleng groen                                                   */
extern bool MMK[FCMAX];              /* meeverleng groen alleen als MK[] waar is                           */
extern bool BMC[FCMAX];              /* beeindig meeverleng groen conflicten                               */
extern bool WGR[FCMAX];              /* wachtstand groen                                                   */
extern bool NAL[FCMAX];              /* naloop als gevolg van harde koppeling actief                       */
extern bool FC_DVM[FCMAX];           /* richting wordt bevoordeeld als gevolg van DVM                      */
extern bool FC_FILE[FCMAX];          /* richting wordt bevoordeeld als gevolg van FILE stroomopwaarts      */
extern bool HerstartOntruim[FCMAX];  /* richting met LHORVA functie R herstart ontruiming vanaf conflicten */
extern mulv  ExtraOntruim[FCMAX];     /* extra ontruiming als gevolg van LHORVA functie R                   */
extern bool HOT[FCMAX];              /* startpuls roodlichtrijder ten behoeve van LHORVA functie R         */
extern bool VG_mag[FCMAX];           /* veiligheidsgroen mag worden aangehouden (er is een hiaat gemeten)  */
extern mulv  AR_max[FCMAX];           /* alternatief maximum                                                */
extern mulv  GWT[FCMAX];              /* gewogen wachttijd tbv toekennen alternatieve realisatie            */
extern mulv  TEG[FCMAX];              /* tijd tot einde groen                                               */
extern mulv  MTG[FCMAX];              /* minimale tijd tot groen                                            */
extern mulv  mmk_old[FCMAX];          /* buffer MM[mmk] - geheugenelement MK[] bij gescheiden hiaatmeting   */
extern mulv  MK_old[FCMAX];           /* buffer MK[]                                                        */
extern mulv  TMPc[FCMAX][FCMAX];      /* tijdelijke conflict matrix                                         */
extern mulv  TMPi[FCMAX][FCMAX];      /* restant fictieve ontruimingstijd                                   */

extern bool DOSEER[FCMAX];           /* doseren aktief                                                     */
extern mulv  DOSMAX[FCMAX];           /* doseer maximum                                                     */
extern mulv  DOS_RD[FCMAX];           /* minimale tijd tot startgroen als gevolg van doseren                */
extern mulv  MINTSG[FCMAX];           /* minimale tijd tot startgroen (zelf te besturen in REG[]ADD)        */
extern mulv  MINTEG[FCMAX];           /* minimale tijd tot eindegroen (zelf te besturen in REG[]ADD)        */
extern mulv  PELTEG[FCMAX];           /* tijd tot einde groen als peloton ingreep maximaal duurt            */

extern mulv  TVG_instelling[FCMAX];   /* buffer ingestelde waarde TVG_max[]                                 */
extern mulv  TGL_instelling[FCMAX];   /* buffer ingestelde waarde TGL_max[]                                 */

extern bool TOE123[FCMAX];           /* toestemming 1-2-3 afteller (zelf te besturen in REG[]ADD)          */
extern mulv  Waft[FCMAX];             /* aftellerwaarde ( > 0 betekent dat 1-2-3 afteller loopt)            */
extern mulv  Aaft[FCMAX];             /* aftellerwaarde ( = 1, 2 of 3 )                                     */
extern mulv  Aled[FCMAX];             /* aantal resterende leds bij wachttijd voorspeller                   */
extern mulv  AanDuurLed[FCMAX];       /* tijd dat huidige aantal leds wordt uitgestuurd                     */
extern mulv  TijdPerLed[FCMAX];       /* tijdsduur per led voor gelijkmatige afloop wachttijd voorspeller   */
extern mulv  wacht_ML[FCMAX];         /* maximale wachttijd volgens de module molen                         */

extern mulv  ARM[FCMAX];              /* kruispunt arm tbv HLPD prioriteit                                  */
extern mulv  volg_ARM[FCMAX];         /* kruispunt volgarm tbv doorzetten HLPD prioriteit                   */
extern bool HD_aanwezig[FCMAX];      /* HLPD aanwezig op richting                                          */
extern bool HLPD[FCMAX];             /* HLPD prioriteit toegekend aan richting (en volgrichtingen)         */
extern mulv  NAL_HLPD[FCMAX];         /* nalooptijd hulpdienst ingreep op richting(en) in volgarm           */
extern mulv  verlos_busbaan[FCMAX];   /* buffer voor verlosmelding met prioriteit                           */
extern bool iPRIO[FCMAX];            /* prioriteit toegekend aan richting                                  */
extern bool A_DST[FCMAX];            /* vaste aanvraag gewenst als gevolg van detectie storingen           */
extern bool MK_DST[FCMAX];           /* star verlengen gewenst als gevolg van detectie storingen           */

extern mulv  PEL_UIT_VTG[FCMAX];      /* buffer aantal voertuig voor uitgaande peloton koppeling            */
extern mulv  PEL_UIT_RES[FCMAX];      /* restant minimale duur uitsturing koppelsignaal peloton koppeling   */

extern mulv  verklik_srm;             /* restant duur verklikking SRM bericht                               */
extern mulv  duur_geen_srm;           /* aantal minuten dat geen SRM bericht is ontvangen (maximum = 32000) */
extern mulv  tkarog_old;              /* buffer oude waarde tijdelement tkarog                              */

extern bool RAT[FCMAX];              /* aansturing rateltikker                                             */
extern bool RAT_test[FCMAX];         /* aansturing rateltikker in testomgeving (specifiek voor Accross)    */
extern bool RAT_aanvraag[FCMAX];     /* aansturing rateltikker is op aanvraag                              */
extern bool RAT_continu[FCMAX];      /* aansturing rateltikker is continu                                  */
extern bool KNIP;                    /* hulpwaarde voor knipper signaal                                    */
extern bool REGEN;                   /* regensensor aktief (zelf te besturen in REG[]ADD)                  */
extern bool WT_TE_HOOG;              /* wachttijd te hoog voor toekennen prioriteit                        */
extern bool PEL_WT_TE_HOOG;          /* wachttijd te hoog voor toekennen prioriteit peloton ingreep        */
extern bool GEEN_OV_PRIO;            /* geen prioriteit OV   (zelf te besturen in REG[]ADD)                */
extern bool GEEN_VW_PRIO;            /* geen prioriteit VW   (zelf te besturen in REG[]ADD)                */
extern bool GEEN_FIETS_PRIO;         /* geen fietsprioriteit (zelf te besturen in REG[]ADD)                */

extern mulv  OV_stipt[FCMAX];         /* buffer stiptheid ingemelde bussen obv KAR                          */
extern mulv  US_OV_old[FCMAX];        /* buffer US[] uitsturing geconditioneerde prioriteit                 */

extern bool DF[DPMAX];               /* detectie fout aanwezig                                             */
extern mulv  D_bez[DPMAX];            /* tijdsduur detector bezet                                           */
extern mulv  D_onb[DPMAX];            /* tijdsduur detector onbezet                                         */
extern mulv  sec_teller;              /* actuele duur intensiteitsmeting                                    */
extern mulv  kwartier[DPMAX];         /* kwartier intensiteit                                               */
extern mulv  Iactueel[DPMAX];         /* actuele stand meting                                               */

extern count ML_REG_MAX;              /* maximum aantal modulen zonder DVM maatregelen                      */
extern count ML_DVM_MAX;              /* maximum aantal modulen bij inzet DVM maatregelen                   */
extern count ML_ACT_MAX;              /* actueel aantal modulen                                             */
extern count PRML_REG[MLMAX][FCMAX];  /* module indeling zonder DVM maatregelen                             */
extern count PRML_DVM[MLMAX][FCMAX];  /* module indeling bij inzet DVM maatregelen                          */
extern bool DVM_structuur_gewenst;   /* module indeling bij inzet DVM maatregelen gewenst                  */
extern bool DVM_structuur_actief;    /* module indeling bij inzet DVM maatregelen actief                   */

extern mulv  DVM_klok;                /* DVM programma - klok wens                                          */
extern mulv  DVM_parm;                /* DVM programma - parameter instelling                               */
extern mulv  DVM_prog;                /* DVM programma - actief                                             */
extern mulv  DVM_prog_duur;           /* DVM programma - actuele duur instelling PRM[dvmpr] in uren         */
extern bool DVM_structuur;           /* DVM module structuur gewenst (zelf te besturen in REG[]ADD)        */
extern bool DVM_structuur_act;       /* DVM module structuur actief                                        */
extern bool max_verleng_groen;       /* keuze maximumgroen of maximum verlenggroentijden                   */

extern mulv  FILE_set;                /* set maximum(verleng)groentijden gewenst a.g.v. FILE stroomopwaarts */
extern bool FILE_nass;               /* FILE stroomafwaarts (= na stopstreep) aanwezig                     */

extern mulv  SLAVE;                   /* t.b.v. Master-Slave: SLAVE  in regelen                             */
extern mulv  MASTER;                  /* t.b.v. Master-Slave: MASTER in regelen                             */
extern bool SLAVE_VRI;               /* t.b.v. Master-Slave: VRI is SLAVE                                  */
extern bool MASTER_VRI;              /* t.b.v. Master-Slave: VRI is MASTER                                 */
extern bool slave_fc[FCMAX];         /* t.b.v. Master-Slave: richting is gedefinieerd in de SLAVE          */

extern struct hki_koppeling  hki_kop[MAX_HKI_KOP];
extern struct vtg_koppeling  vtg_tgo[MAX_VTG_KOP];
extern struct lvk_gelijkstr  lvk_gst[MAX_LVK_GST];
extern struct dcf_voorstart  dcf_vst[MAX_DCF_VST];
extern struct dcf_gelijkstr  dcf_gst[MAX_DCF_GST];
extern struct pel_koppeling  pel_kop[MAX_PEL_KOP];
extern struct fietsvoorrang  fts_pri[MAX_FTS_PRI];
extern struct prioriteit_id  prio_index[FCMAX];
extern struct afteller       aft_123[FCMAX];
extern struct max_groen_DVM  DVM_max[FCMAX];
extern struct max_groen_FILE FILE_max[FCMAX];

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
extern char  _UUR[MAXDUMPSTAP];        /* bijhouden UUR tbv flight buffer                                    */
extern char  _MIN[MAXDUMPSTAP];        /* bijhouden MIN tbv flight buffer                                    */
extern char  _SEC[MAXDUMPSTAP];        /* bijhouden SEC tbv flight buffer                                    */
extern mulv  _ML[MAXDUMPSTAP];         /* bijhouden ML  tbv flight buffer                                    */
extern char  _FC[FCMAX][MAXDUMPSTAP];  /* bijhouden fasecyclus status tbv flight buffer                      */
extern char  _FCA[FCMAX][MAXDUMPSTAP]; /* bijhouden aanvraag   status tbv flight buffer                      */
extern bool _HA[FCMAX];               /* hulpwaarde A[] tbv start- en einde puls aanvraag in flight buffer  */
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
void init_traffick2tlcgen(void);      /* Fik240224                                                          */


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
    bool kop_eg,                         /* bool koppeling vanaf einde groen (als FALSE dan vanaf EV)         */
    bool los_fc2,                        /* bool fc2 mag bij aanvraag fc1 los realiseren                      */
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
/* Met behulp van deze functie worden de gegevens van deelconflicten (voorstart) in een struct geplaatst.   */
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
/* Functie definieer deelconflict gelijkstart                                                               */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de gegevens van deelconflicten (gelijkstart) in een struct geplaatst. */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_init_application().                                                */
/*                                                                                                          */
void definitie_gelijkstart_dcf(       /* Fik230101                                                          */
    count fc1,                            /* FC  richting 1                                                     */
    count fc2,                            /* FC  richting 2                                                     */
    count to12,                           /* TM  ontruimingstijd van fc1 naar fc2                               */
    count to21,                           /* TM  ontruimingstijd van fc2 naar fc1                               */
    count ma12,                           /* SCH meerealisatie van fc1 met fc2                                  */
    count ma21);                          /* SCH meerealisatie van fc2 met fc1                                  */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer meerealisatie                                                                          */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de gegevens van meerealisaties in een struct geplaatst.               */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_init_application().                                                */
/*                                                                                                          */
void definitie_meerealisatie(         /* Fik230101                                                          */
    count fc1,                            /* FC  richting die meerealisatie geeft                               */
    count fc2,                            /* FC  richting die meerealisatie krijgt                              */
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
    count kop_fc,                         /* FC koppelrichting                                                  */
    count kop_toe,                        /* ME toestemming peloton ingreep (bij NG altijd toestemming)         */
    count kop_sig,                        /* HE koppelsignaal                                                   */
    count kop_bew,                        /* TM bewaak koppelsignaal (bij NG wordt een puls veronderstelt)      */
    count aanv_vert,                      /* TM aanvraag vertraging  (bij NG wordt geen aanvraag opgzet)        */
    count vast_vert,                      /* TM vasthoud vertraging  (start op binnenkomst koppelsignaal)       */
    count duur_vast,                      /* TM duur vasthouden (bij duursign. na afvallen koppelsignaal)       */
    count duur_verl,                      /* TM duur verlengen na ingreep (bij NG geldt TVG_max[])              */
    count hnaloop_1,                      /* HE voorwaarde herstart extra nalooptijd 1 (nalooplus 1)            */
    count tnaloop_1,                      /* TM nalooptijd 1                                                    */
    count hnaloop_2,                      /* HE voorwaarde herstart extra nalooptijd 2 (nalooplus 2)            */
    count tnaloop_2,                      /* TM nalooptijd 2                                                    */
    count verklik);                       /* US verklik peloton ingreep                                         */


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
void definitie_afteller(              /* Fik240224                                                          */
    count fc,                             /* FC    richting met afteller                                        */
    count de1,                            /* DE    koplus 1                                                     */
    count de2,                            /* DE    koplus 2                                                     */
    count de3,                            /* DE    koplus 3                                                     */
    count toest,                          /* SCH   toestemming aansturing afteller                              */
    count min_duur,                       /* PRM   minimale duur tot start groen waarbij afteller mag starten   */
    count tel_duur,                       /* PRM   duur van een tel in tienden van seconden                     */
    count is_oke_1,                       /* IS    afteller lantaarn 1 werkt correct                            */
    count is_oke_2,                       /* IS    afteller lantaarn 2 werkt correct                            */
    count is_oke_3,                       /* IS    afteller lantaarn 3 werkt correct                            */
    count is_oke_4,                       /* IS    afteller lantaarn 4 werkt correct                            */
    count us_oke,                         /* US    aftellers van alle lantaarns werken correct                  */
    count us_getal,                       /* US    tbv verklikking op bedienpaneel                              */
    count us_bit0,                        /* US    aansturing afteller BIT0                                     */
    count us_bit1);                       /* US    aansturing afteller BIT1                                     */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer maximum(verleng)groen sets voor DVM netwerk programma's                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de DVM groentijden sets in een struct geplaatst.                      */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit definitie_groentijden_traffick().                                       */
/*                                                                                                          */
void definitie_max_groen_dvm_va_arg(  /* Fik230101                                                          */
    count fc, ...);                       /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer maximum(verleng)groen sets voor FILE programma's stroomopwaarts                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de FILE stroomopwaarts groentijden sets in een struct geplaatst.      */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit definitie_groentijden_traffick().                                       */
/*                                                                                                          */
void definitie_max_groen_file_va_arg( /* Fik230101                                                          */
    count fc, ...);                       /* FC fasecyclus                                                      */


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
/* Functie detector afhandeling Traffick2TLCGen                                                             */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de detectie afhandeling van de Traffick2TLCGen functionaliteiten.                  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit traffick2tlcgen_detectie().                                             */
/*                                                                                                          */
void detector_afhandeling_va_arg(     /* Fik230726                                                          */
    count fc,                             /* FC   fasecyclus                                                    */
    count km, ...);                       /* TM   koplus maximum                                                */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bijwerken detectie variabelen Traffick2TLCGen                                                    */
/* -------------------------------------------------------------------------------------------------------- */
/* In deze functie worden de detectie variabelen bijgewerkt, te weten:                                      */
/* DF[]     : detectie fout geconstateerd                                                                   */
/* Dbez[]   : tijdsduur detector bezet                                                                      */
/* Donb[]   : tijdsduur detector onbezet                                                                    */
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
/* Functie bepaal start puls lengte detectie                                                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt de lengte die aan de applicatie is doorgegeven op basis van lengte detectie.        */
/* De lengte wordt precies 1 applicatie ronde aangeboden voor gebruik in andere functies.                   */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PreApplication_Add().                                                   */
/*                                                                                                          */
mulv bepaal_start_puls_lengte(        /* Fik240101                                                          */
    count is);                            /* IS ingang signaal waarop lengte wordt afgebeeld                    */


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
    mulv  wg_aanvraag,                    /* bool wachtstand groen aanvraag                                    */
    mulv  mgmk,                           /* mulv  meeverleng groen ook als MK[] niet waar is                   */
    mulv  mg,                             /* mulv  meeverleng groen is nooit toegestaan                         */
    mulv  ar,                             /* mulv  alternatieve realisatie toegestaan - algemene schakelaar     */
    mulv  arb,                            /* mulv  alternatieve realisatie toegestaan - verfijning per blok     */
    mulv  alt_ruimte,                     /* mulv  alternatieve ruimte voor toestemming alternatieve realisatie */
    mulv  alt_max,                        /* mulv  alternatief maximum                                          */
    count prioOV_index_kar,               /* count OVFCfc - prioriteitsindex OV ingreep - KAR                   */
    count prioOV_index_srm,               /* count OVFCfc - prioriteitsindex OV ingreep - SRM                   */
    count prioOV_index_verlos,            /* count OVFCfc - prioriteitsindex OV ingreep - verlos                */
    count prioHD_index,                   /* count hdFCfc - prioriteitsindex hulpdienst ingreep                 */
    bool HD_counter,                     /* bool HD_counter (TRUE = hulpdienst aanwezig)                      */
    count prioVRW_index,                  /* count VRWFCfc - prioriteitsindex fiets vrachtwagen ingreep         */
    count prioFTS_index);                 /* count ftsFCfc - prioriteitsindex fiets voorrang module             */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie fasecyclus instellingen Traffick2TLCGen - deel 2                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* In deze functie worden fasecyclus instellingen naar arrays gekopieerd, zodat deze instellingen eenvoudig */
/* benaderbaar zijn voor andere functies.                                                                   */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PreApplication_Add().                                                   */
/*                                                                                                          */
void traffick2tlcgen_instel_2(        /* Fik230301                                                          */
    count fc,                             /* FC   fasecyclus                                                    */
    mulv  KAR_id_OV,                      /* mulv KAR id openbaar vervoer                                       */
    mulv  KAR_id_HD,                      /* mulv KAR id nood- en hulpdiensten                                  */
    count usHD,                           /* US   verklik HD ingreep                                            */
    count usOV_kar,                       /* US   verklik OV ingreep - KAR                                      */
    count usOV_srm,                       /* US   verklik OV ingreep - SRM                                      */
    count usVRW,                          /* US   verklik VRW ingreep                                           */
    mulv  vooruit,                        /* mulv aantal modulen dat vooruit gerealiseerd mag worden            */
    count altw);                          /* count ophoogfactor wachttijd voor alternatieve realisatie          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal actief DVM netwerk programma                                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt het actieve DVM programma op basis van de klok of de DVM parameter. De instelling   */
/* van de DVM parameter wordt bewaakt op een maximale duur instelbaar in uren.                              */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit KlokPerioden_Add().                                                     */
/*                                                                                                          */
void bepaal_DVM_programma(void);      /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie reset wachtstand aanvraag indien niet gewenst                                                    */
/* -------------------------------------------------------------------------------------------------------- */
/* Binnen TLCGen bestaat voor wachtstand groen richtingen niet de mogelijkheid om enkel de wachtstand groen */
/* aanvraag uit te schakelen. Deze functie reset alle wachtstand groen aanvragen.                           */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Aanvragen_Add().                                                        */
/*                                                                                                          */
void reset_wachtstand_aanvraag(void); /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer wachtstand aanvraag bij harde koppelingen                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCGen zet de wachtstand aanvraag zodra alle ontruimingstijden vanaf conflictrichtingen zijn verstreken. */
/* Voor gekoppelde richtingen geldt echter dat ook alle ontruimingstijden naar de volgrichting moeten zijn  */
/* verstreken. Deze functie corrigeert de wachtstand aanvraag van gekoppelde richtingen.                    */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Aanvragen_Add().                                                        */
/*                                                                                                          */
void hki_wachtstand_aanvraag(void);   /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal aanvragen harde koppeling                                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt bij harde koppelingen het opzetten van de meeaanvraag van de volgrichtingen.       */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Aanvragen_Add().                                                        */
/*                                                                                                          */
void koppel_aanvragen(void);          /* Fik230701                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal aanvraag fiets voorrang module                                                            */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt of de fietsrichting met prioriteit kan worden aangevraagd.                          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Aanvragen_Add().                                                        */
/*                                                                                                          */
void fiets_voorrang_aanvraag(void);   /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Bijwerken peloton ingreep                                                                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verwerkt de koppelsignalen en bepaalt de status van de peloton koppeling.                   */
/*                                                                                                          */
/* NG: uitgeschakeld                                                                                        */
/*  0: niet aktief                                                                                          */
/*  1: peloton in aantocht                                                                                  */
/*  2: peloton in aantocht koppelrichting is aangevraagd                                                    */
/*  3: peloton in aantocht groen wordt vastgehouden door 1e nalooptijd                                      */
/*  4: peloton in aantocht groen wordt vastgehouden door 2e nalooptijd (= van 1e nalooplus)                 */
/*  5: peloton in aantocht groen wordt vastgehouden door 3e nalooptijd (= van 2e nalooplus)                 */
/*  6: koppelrichting verlengt na afloop van peloton ingreep                                                */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit peloton_ingreep_aanvraag().                                             */
/*                                                                                                          */
void bijwerken_peloton_ingreep(void);  /* Fik230101                                                         */


/* -------------------------------------------------------------------------------------------------------- */
/* Peloton ingreep - aanvragen                                                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie zet de aanvraag tbv de peloton ingreep.                                                     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Aanvragen_Add().                                                        */
/*                                                                                                          */
void peloton_ingreep_aanvraag(void);   /* Fik230101                                                         */


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
/* Functie bepaal extra ontruiming als gevolg van LHOVRA functie R                                          */
/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaalt de extra ontruiming als op een conflict richting een roodlichtrijder wordt gedetecteerd. */
/* Roodlichtrijders nadat het licht langer dan 2,0 sec. op rood staat worden genegeerd.                     */
/*                                                                                                          */
/* Resultaat wordt weggeschreven in ExtraOntruim[].                                                         */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit RealTraffick().                                                         */
/*                                                                                                          */
void BepaalExtraOntruim(void);        /* Fik230726                                                          */


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
void RealTraffick(void);              /* Fik230726                                                          */


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
void RealTraffickPrioriteit(void);    /* Fik230830                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal maximum(verleng)groentijden tijdens DVM en/of FILE stroomopwaarts.                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Tijdens DVM en/of FILE stroomopwaartw worden vooraf gedefinieerde maximum(verleng)groentijden sets van   */
/* kracht. Deze functie zorgt voor de juiste instelling van TVG_max[] en bepaalt of een richting wordt      */
/* bevoordeeld door de ingezette maatregel.                                                                 */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Maxgroen_Add().                                                         */
/*                                                                                                          */
void bepaal_maximum_groen_traffick(void); /* Fik230101                                                      */


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
bool REALconflict(                   /* Fik230101                                                          */
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
/* Peloton ingreep - groen vasthouden                                                                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie zorgt voor het aanhouden van het wachtgroen bij een peloton ingreep.                        */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Wachtgroen_Add().                                                       */
/*                                                                                                          */
void peloton_ingreep_wachtgroen(void);/* Fik230101                                                          */


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
/* Peloton ingreep - groen verlengen                                                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie zorgt voor het verlengen na afloop van een peloton ingreep.                                 */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Meetkriterium_Add().                                                    */
/*                                                                                                          */
void peloton_ingreep_verlengen(void); /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal maximaal meeverlengen                                                                     */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt of richtingen nog kunnen meeverlengen op basis van "maatgevend groen". Indien dit   */
/* niet meer het geval is wordt YM[] BIT4 gereset.                                                          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Meeverlengen_Add().                                                     */
/*                                                                                                          */
void Traffick2TLCgen_MVG(void);       /* Fik230901                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal uitstel                                                                                   */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt het uitstel voor deelconflicten en koppelingen. De functie maakt hiervoor gebruik   */
/* van X[] BIT3 en RR[] BIT3.                                                                               */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Synchronisaties_Add().                                                  */
/*                                                                                                          */
void Traffick2TLCgen_uitstel(void);   /* Fik230830                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal periode vooruit realiseren                                                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie corrigeert de periode waarin vooruit realiseren is toegestaan. Dit is nodig als MLMAX hoger */
/* is dan het actueel aantal modulen. De functie overschrijft PFPR[] zoals bepaald door TLCGen.             */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit VersneldPrimair_Add().                                                  */
/*                                                                                                          */
void Traffick2TLCgen_PFPR(void);      /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal periode alternatieve realisatie                                                           */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt de periode waarin alternatief realiseren is toegestaan. De functie maakt gebruik    */
/* van de realisatie ruimte zoals bepaald in REALTraffick(). De functie overschrijft PAR[] zoals bepaald    */
/* door TLCgen.                                                                                             */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Alternatief_Add().                                                      */
/*                                                                                                          */
void Traffick2TLCgen_PAR(void);       /* Fik230701                                                          */


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
/* Functie forceer realisatie van richtingen met gelijkstart                                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie forceert een richting met een gelijkstart naar groen indien daarop gewacht wordt.           */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_REA().                                                  */
/*                                                                                                          */
void Traffick2TLCgen_REA_GST(void);   /* Fik240101                                                          */


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
/* Functie reset doseer bits ten behoeve van file stroomafwaarts                                            */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie reset de doseer bits omdat deze door verschillende file ingrepen kunnen worden opgezet.     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit FileVerwerking_Add().                                                   */
/*                                                                                                          */
void traffick_file_nass_reset(void);  /* Fik230701                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie maatregelen bij file stroomafwaarts                                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de standaard maatregelen bij file stroomafwaarts. Voor de maximum groentijd gelden */
/* verschillende criteria namelijk een percentage van de maximum(verleng)groenduur of een absoluut maximum. */
/* De laagste waarde wordt automatisch maatgevend. Indien file stroomafwaarts ontstaat tijdens de groenfase */
/* geldt een aparte ondergrens als minimum voor de maximum groenduur. (= specifiek voor die 1e groenfase)   */
/*                                                                                                          */
void traffick_file_nass(              /* Fik230701                                                          */
    count fc,                             /* FC  fasecyclus                                                     */
    count h_file,                         /* HE  file stroomafwaarts aanwezig                                   */
    count h_afkap_start,                  /* HE  file stroomafwaarts is tijdens groen ontstaan                  */
    count sch_ingreep,                    /* SCH file ingreep toegestaan                                        */
    count sch_perc_gr,                    /* SCH percentage maximum(verleng)groen toepassen                     */
    count prm_perc_gr,                    /* PRM percentage maximum(verleng)groen                               */
    count t_afk_sfile,                    /* T   ondergrens voor afbreken als file tijdens groen ontstaat       */
    count t_max_groen,                    /* T   maximum groenduur tijdens file stroomafwaarts                  */
    count t_min_rood);                    /* T   minimum roodduur  tijdens file stroomafwaarts                  */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie maatregelen bij detectie storing                                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de standaard maatregelen bij detectie storing.                                     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit DetectieStoring_Add().                                                  */
/*                                                                                                          */
void traffick_detectie_storing(       /* Fik230101                                                          */
    count fc,                             /* FC  fasecyclus                                                     */
    count tvertraag,                      /* TM  tijdvertraging aanvraag                                        */
    count pstar);                         /* PRM percentage star uitverlengen                                   */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie uitgaande pelotonkoppeling obv getelde voertuigen in de wachtrij tijdens rood                    */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de uitsturing van een uitgaand peloton signaal obv getelde voertuigen in de        */
/* wachtrij tijdens rood. Indien het koppelsignaal als puls is gedefinieerd wordt het bijbehorende koppel-  */
/* signaal vanaf startgroen gedurende 2,0 sec. uitgestuurd. Bij een duursignaal wordt het koppelsignaal     */
/* uitgestuurd zolang het meetkriterium MK[] waar is tijdens de groenfase.                                  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_application().                                                     */
/*                                                                                                          */
void peloton_meting_uit_wachtrij(     /* Fik240701                                                          */
    count fc,                             /* FC    fasecyclus                                                   */
    count de1,                            /* DE    verweglus 1                                                  */
    count de2,                            /* DE    verweglus 2                                                  */
    count de3,                            /* DE    verweglus 3                                                  */
    count pgrenswaarde,                   /* PRM   grenswaarde aantal voertuigen                                */
    bool duur_signaal,                   /* bool FALSE = puls                                                 */
    bool meting_actief,                  /* bool TRUE  = meting actief                                        */
    count us_ks);                         /* US    uitgaand koppelsignaal                                       */


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
void peloton_meting_uit_stopstrp(     /* Fik240701                                                          */
    count fc,                             /* FC    fasecyclus                                                   */
    count de1,                            /* DE    koplus 1                                                     */
    count de2,                            /* DE    koplus 2                                                     */
    count de3,                            /* DE    koplus 3                                                     */
    count pgrenswaarde,                   /* PRM   grenswaarde aantal voertuigen                                */
    bool duur_signaal,                   /* bool FALSE = puls                                                 */
    bool meting_actief,                  /* bool TRUE  = meting actief                                        */
    count us_ks,                          /* US    uitgaand koppelsignaal                                       */
    count tpelmeet,                       /* TM    meetperiode peloton koppeling                                */
    count tpeltdh);                       /* TM    grenshiaat  peloton meting                                   */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer aansturing verklikking stiptheid openbaar vervoer op basis van KAR                     */
/* -------------------------------------------------------------------------------------------------------- */
/* In de TLCGen aansturing kan de verklikking wegvallen voordat alle bussen zijn uitgemeld. Deze functie    */
/* corrigeert de aansturing van de verklikking.                                                             */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_application().                                                     */
/*                                                                                                          */
void US_OV_stiptheid_KAR(             /* Fik230101                                                          */
    count fc,                             /* FC fasecylus                                                       */
    count ov_counter,                     /* CT aantal aanwezige bussen in het traject                          */
    count us_tevroeg,                     /* US 1e bus is te vroeg                                              */
    count us_optijd,                      /* US 1e bus is op tijd                                               */
    count us_telaat);                     /* US 1e bus is te laat                                               */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie test uitgangssignalen                                                                            */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie zet een uitgangssignaal op ten behoeve van test doeleinden.                                 */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_application().                                                     */
/*                                                                                                          */
void test_us_signalen(void);          /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende OV ingreep tbv wachttijd voorspeller                      */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie geeft de aanwezigheid terug van een conflicterende busingreep tbv de wachttijd voorspeller. */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit aansturing_wt_voorspeller().                                            */
/*                                                                                                          */
bool conflict_OV(                    /* Fik230101                                                          */
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
void aansturing_wt_voorspeller(       /* Fik230901                                                          */
    count fc,                             /* FC  fasecyclus                                                     */
    count us0,                            /* US  wachttijd voorspeller - BIT0                                   */
    count us1,                            /* US  wachttijd voorspeller - BIT1                                   */
    count us2,                            /* US  wachttijd voorspeller - BIT2                                   */
    count us3,                            /* US  wachttijd voorspeller - BIT3                                   */
    count us4,                            /* US  wachttijd voorspeller - BIT4                                   */
    count usbus,                          /* US  aansturing bus sjabloon                                        */
    count schwtv,                         /* SCH aansturing WTV toegestaan - instelling                         */
    count hewtv,                          /* HE  aansturing WTV toegestaan - actueel                            */
    count mealed,                         /* ME  aantal leds volgens berekening                                 */
    count usaled_uit,                     /* US  aantal leds dat uitgestuurd wordt                              */
    count mealed_uit);                    /* ME  aantal leds dat uitgestuurd wordt                              */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie aansturing afteller                                                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de aansturing van de aftellers.                                                    */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void aansturing_aftellers(void);      /* Fik240224                                                          */


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
void rateltikker_applicatie(          /* Fik230914                                                          */
    count fc,                             /* FC    fasecyclus                                                   */
    count hedrk1,                         /* HE    drukknop 1                                                   */
    count hedrk2,                         /* HE    drukknop 2                                                   */
    count usrat,                          /* US    rateltikker                                                  */
    count natik,                          /* TM    natikken vanaf start rood                                    */
    bool ratel_periode,                  /* bool werkingsperiode  rateltikkers                                */
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
/* Functie verklik peloton prioriteit                                                                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de verklikking van de peloton prioriteit. Het led knippert als er een voertuig in  */
/* het traject aanwezig is en brandt vast tijdens het groen als er prioriteit verstrekt is.                 */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void verklik_peloton_ingreep(void);   /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verklik prioriteit op basis van KAR en SRM                                                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de verklikking van prioriteit op basis van KAR en SRM. Het led knippert als er een */
/* voertuig met een prioriteitsaanvraag aanwezig is en brandt vast tijdens het groen als er prioriteit      */
/* verstrekt is.                                                                                            */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void verklik_prio_KAR_SRM(void);      /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verklik SRM bericht ontvangen en SRM ondergedrag                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verklikt de binnenkomst van SRM berichten en bewaakt SRM op ondergedrag.                    */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void verklik_bewaak_SRM(              /* Fik230830                                                          */
    count us_srm,                         /* US   verklik SRM bericht ontvangen                                 */
    mulv  duur_verklik_srm,               /* mulv duur verklik SRM bericht ontvangen in tienden van seconden    */
    count us_srm_og,                      /* US   verklik SRM ondergedrag                                       */
    mulv  srm_og);                        /* mulv duur ondergedrag SRM in minuten                               */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verklik fiets voorrang module                                                                    */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de verklikking van de fiets voorrang module. Het led knippert tijdens rood indien  */
/* een prioriteitsaanvraag aanwezig is en brandt vervolgens vast zolang de fietsrichting verlengt.          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void verklik_fiets_voorrang(void);    /* Fik230901                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie buffer stiptheid ingemelde bussen op basis van KAR                                               */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie buffert de stiptheid van ingemelde bussen op basis van KAR.                                 */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit InUitMelden_Add().                                                      */
/*                                                                                                          */
void buffer_stiptheid_info(void);     /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie prioriteitsafhandeling fiets voorrang module                                                     */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de prioriteitsafhandeling van de fiets voorrang module. Indien de fietsrichting    */
/* geblokkeerd wordt of indien de wachttijd te hoog oploopt kan de "uitmelding" tijdens rood plaatsvinden.  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit InUitMelden_Add().                                                      */
/*                                                                                                          */
void fiets_voorrang_module(void);     /* Fik230901                                                          */


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
    count fc,                             /* FC    fasecyclus                                                   */
    count prioOV_index_verlos,            /* count OVFCfc - prioriteitsindex OV ingreep - verlos                */
    count de1,                            /* DE    koplus nabij de stopstreep                                   */
    count de2,                            /* DE    koplus tbv lengte gevoeligheid (optioneel)                   */
    count hinm,                           /* HE    puls tbv in.melding (wordt door TLCgen gegenereerd)          */
    count huitm,                          /* HE    puls tbv uitmelding (wordt door TLCgen gegenereerd)          */
    mulv  min_rood);                      /* mulv  minimale roodtijd (TE) voor prioriteit aanvraag              */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer terugkomen na afbreken                                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Tijdens DVM en FILE stroomopwaarts worden alleen aan nood- en hulpdiensten en aan het OV op vrijliggende */
/* busbanen prioriteitsrealisaties toegekend. Richtingen die door DVM of FILE bevorderd worden mogen altijd */
/* terugkomen na afbreken.                                                                                  */
/*                                                                                                          */
/* De groenbewaking van fietsers met fietsvoorrang module wordt uitgeschakeld zodat bij prioriteit de       */
/* groenfase ook altijd kan worden verlengt. Uitmelding vindt plaats in de functie fiets_voorrang_module(). */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PrioInstellingen_Add().                                                 */
/*                                                                                                          */
void corrigeer_terugkomen_traffick(void); /* Fik230901                                                      */


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
/* volgarm. De functie controleert of richting met de hulpdienst een volg_ARM heeft. Als dit het geval is   */
/* wordt de nalooptijd doorgezet op alle richtingen van de volg_ARM.                                        */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PrioriteitsOpties_Add() na aanroep Traffick2TLCgen_PRIO_OPTIES().       */
/*                                                                                                          */
void Traffick2TLCgen_HLPD_nal(        /* Fik230101                                                          */
    count fc,                             /* FC   fasecyclus voedende richting                                  */
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
/* van de status van de regensensor.                                                                        */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_PRIO_OPTIES().                                          */
/*                                                                                                          */
void Traffick2TLCgen_FIETS(void);     /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal prioriteitsopties bij geconditioneerde prioriteit                                         */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie corrigeert de prioriteitsopties voor het openbaar vervoer afhankelijk van de stipheid.      */
/* Deze functionaliteit is alleen beschikbaar voor openbaar vervoer ingrepen op basis van KAR.              */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PrioriteitsOpties_Add().                                                */
/*                                                                                                          */
void prioriteit_opties_stiptheid(     /* Fik230101                                                          */
    count fc,                             /* FC  fasecyclus                                                     */
    count prmtevroeg,                     /* PRM prioriteitsopties voor bus te vroeg                            */
    count prmoptijd,                      /* PRM prioriteitsopties voor bus op tijd                             */
    count prmtelaat);                     /* PRM prioriteitsopties voor bus te laat                             */


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
bool TI(                             /* Fik230101                                                          */
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
bool FK_conflict(                    /* Fik230101                                                          */
    count fc1,                            /* FC fasecyclus 1                                                    */
    count fc2);                           /* FC fasecyclus 2                                                    */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal conflicterende richting met een aanvraag                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende richting met een aanvraag.      */
/* Het verschil met de CCOLfunctie ka() is dat ook tijdelijke conflicten worden getest op een aanvraag.     */
/*                                                                                                          */
bool tka(                            /* Fik230101                                                          */
    count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid (fictief)conflicterende richting in RA[]                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een (fictief)conflicterende richting in RA[].      */
/*                                                                                                          */
bool fkra(                           /* Fik230101                                                          */
    count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende richting in RA[] met P[]                                  */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende richting in RA[] met P[].       */
/*                                                                                                          */
bool krap(                           /* Fik230101                                                          */
    count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid (fictief)conflicterende richting in RA[] met P[]                         */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een (fictief)conflicterende ri. in RA[] met P[].   */
/*                                                                                                          */
bool fkrap(                          /* Fik230101                                                          */
    count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende richting met AAPR die (vooruit) kan realiseren            */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende richting met AAPR die (vooruit) */
/* kan realiseren. (geen AAPR[] BIT5)                                                                       */
/*                                                                                                          */
bool kaapr(                          /* Fik230101                                                          */
    count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal (fictief)conflicterende richting in AA[].                                             */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een (fictief)conflicterende richting in AA[].      */
/* Het verschil met de CCOLfunctie fkaa() is dat ook tijdelijke (fictieve)conflicten worden getest op AA[]. */
/*                                                                                                          */
bool tfkaa(                          /* Fik230101                                                          */
    count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal conflicterende richting in CV[].                                                      */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende richting in CV[]. Het verschil  */
/* met de CCOLfunctie kcv() is dat ook tijdelijke conflicten worden getest op CV[].                         */
/*                                                                                                          */
bool tkcv(                           /* Fik230101                                                          */
    count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende prioriteitsingreep welke nog niet is gerealiseerd         */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende prioriteitsingreep welke nog    */
/* niet is gerealiseerd. (nog niet groen is)                                                                */
/*                                                                                                          */
bool conflict_prio_real(             /* Fik230101                                                          */
    count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid hulpdienst ingreep                                                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een hulpdienst ingreep.                            */
/*                                                                                                          */
bool hlpd_aanwezig(void);            /* Fik230101                                                          */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende hulpdienst ingreep                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende hulpdienst ingreep.             */
/*                                                                                                          */
bool khlpd(                          /* Fik230101                                                          */
    count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal of een richting een volgrichting is binnen een gedefinieerde harde koppeling          */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als RETURN of een richting een volgrichting is binnen een gedefinieerde harde koppeling. */
/*                                                                                                          */
bool volgrichting_hki(               /* Fik230101                                                          */
    count fc);                            /* FC fasecyclus                                                      */


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal of een richting een voorstart moet geven binnen een gedefinieerd deelconflict         */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als RETURN of een richting een voorstart moet geven aan een andere richting.             */
/*                                                                                                          */
bool voorstart_gever(                /* Fik230101                                                          */
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

void pre_msg_fctiming(void);                 /* Fik230101                                                   */
void extra_definities_traffick(void);        /* Fik230101                                                   */
void extra_instellingen_traffick(void);      /* Fik230101                                                   */
void detectie_veld_afhandeling(void);        /* Fik230101                                                   */
void maatregelen_bij_detectie_storing(void); /* Fik230101                                                   */
void traffick_file_afhandeling(void);        /* Fik230101                                                   */
void traffick_corrigeer_wtv(void);           /* Fik230101                                                   */
void corrigeer_verklikking_stiptheid(void);  /* Fik230101                                                   */
void busbaan_verlos_prioriteit(void);        /* Fik230101                                                   */
void corrigeer_opties_stiptheid(void);       /* Fik230101                                                   */

