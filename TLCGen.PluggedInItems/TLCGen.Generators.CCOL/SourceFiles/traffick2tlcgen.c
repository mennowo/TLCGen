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

#include "traffick2tlcgen.h"

mulv  aantal_hki_kop;                 /* aantal harde koppelingen                                           */
mulv  aantal_vtg_tgo;                 /* aantal voetgangerskoppelingen - type gescheiden oversteek          */
mulv  aantal_lvk_gst;                 /* aantal gelijk starten langzaam verkeer                             */
mulv  aantal_dcf_vst;                 /* aantal deelconflicten voorstart                                    */
mulv  aantal_dcf_gst;                 /* aantal deelconflicten gelijkstart                                  */
mulv  aantal_mee_rea;                 /* aantal meerealisaties                                              */
mulv  aantal_pel_kop;                 /* aantal peloton koppelingen                                         */
mulv  aantal_fts_pri;                 /* aantal definities fiets voorrang module                            */
mulv  aantal_aft_123;                 /* aantal definities aftellers                                        */
mulv  aantal_dvm_prg;                 /* aantal DVM netwerk programma's                                     */
mulv  aantal_file_prg;                /* aantal FILE programma's (stroomopwaarts)                           */

mulv  REALtraffick[FCMAX];            /* REALTIJD voor richtingen die als eerstvolgende aan de beurt zijn   */
bool PARtraffick[FCMAX];             /* buffer PAR[] zoals bepaald door Traffick                           */
bool AAPRprio[FCMAX];                /* AAPR[] voor prioriteitsrealisaties                                 */
mulv  PFPRtraffick[FCMAX];            /* aantal modulen dat vooruit gerealiseerd mag worden                 */
mulv  AltRuimte[FCMAX];               /* realisatie ruimte voor alternatieve realisatie                     */
bool ART[FCMAX];                     /* alternatieve realisatie toegestaan algemene schakelaar             */
mulv  ARB[FCMAX];                     /* alternatieve realisatie toegestaan verfijning per blok             */
bool MGR[FCMAX];                     /* meeverleng groen                                                   */
bool MMK[FCMAX];                     /* meeverleng groen alleen als MK[] waar is                           */
bool BMC[FCMAX];                     /* beeindig meeverleng groen conflicten                               */
bool WGR[FCMAX];                     /* wachtstand groen                                                   */
bool NAL[FCMAX];                     /* naloop als gevolg van harde koppeling actief                       */
bool FC_DVM[FCMAX];                  /* richting wordt bevoordeeld als gevolg van DVM                      */
bool FC_FILE[FCMAX];                 /* richting wordt bevoordeeld als gevolg van FILE stroomopwaarts      */
bool HerstartOntruim[FCMAX];         /* richting met LHORVA functie R herstart ontruiming vanaf conflicten */
mulv  ExtraOntruim[FCMAX];            /* extra ontruiming als gevolg van LHORVA functie R                   */
bool HOT[FCMAX];                     /* startpuls roodlichtrijder ten behoeve van LHORVA functie R         */
bool VG_mag[FCMAX];                  /* veiligheidsgroen mag worden aangehouden (er is een hiaat gemeten)  */
mulv  AR_max[FCMAX];                  /* alternatief maximum                                                */
mulv  GWT[FCMAX];                     /* gewogen wachttijd tbv toekennen alternatieve realisatie            */
mulv  TEG[FCMAX];                     /* tijd tot einde groen                                               */
mulv  MTG[FCMAX];                     /* minimale tijd tot groen                                            */
mulv  mmk_old[FCMAX];                 /* buffer MM[mmk] - geheugenelement MK[] bij gescheiden hiaatmeting   */
mulv  MK_old[FCMAX];                  /* buffer MK[]                                                        */
mulv  TMPc[FCMAX][FCMAX];             /* tijdelijke conflict matrix                                         */
mulv  TMPi[FCMAX][FCMAX];             /* restant fictieve ontruimingsijd                                    */

bool DOSEER[FCMAX];                  /* doseren aktief                                                     */
mulv  DOSMAX[FCMAX];                  /* doseer maximum                                                     */
mulv  DOS_RD[FCMAX];                  /* minimale tijd tot startgroen als gevolg van doseren                */
mulv  MINTSG[FCMAX];                  /* minimale tijd tot startgroen (zelf te besturen in REG[]ADD)        */
mulv  MINTEG[FCMAX];                  /* minimale tijd tot eindegroen (zelf te besturen in REG[]ADD)        */
mulv  PELTEG[FCMAX];                  /* tijd tot einde groen als peloton ingreep maximaal duurt            */

mulv  TVG_instelling[FCMAX];          /* buffer ingestelde waarde TVG_max[]                                 */
mulv  TGL_instelling[FCMAX];          /* buffer ingestelde waarde TGL_max[]                                 */

bool TOE123[FCMAX];                  /* toestemming 1-2-3 afteller (zelf te besturen in REG[]ADD)          */
mulv  Waft[FCMAX];                    /* aftellerwaarde ( > 0 betekent dat 1-2-3 afteller loopt)            */
mulv  Aaft[FCMAX];                    /* aftellerwaarde ( = 1, 2 of 3 )                                     */
mulv  Aled[FCMAX];                    /* aantal resterende leds bij wachttijd voorspeller                   */
mulv  AanDuurLed[FCMAX];              /* tijd dat huidige aantal leds wordt uitgestuurd                     */
mulv  TijdPerLed[FCMAX];              /* tijdsduur per led voor gelijkmatige afloop wachttijd voorspeller   */
mulv  wacht_ML[FCMAX];                /* maximale wachttijd volgens de module molen                         */

mulv  ARM[FCMAX];                     /* kruispunt arm tbv HLPD prioriteit                                  */
mulv  volg_ARM[FCMAX];                /* kruispunt volgarm tbv doorzetten HLPD prioriteit                   */
bool HD_aanwezig[FCMAX];             /* HLPD aanwezig op richting                                          */
bool HLPD[FCMAX];                    /* HLPD prioriteit toegekend aan richting (en volgrichtingen)         */
mulv  NAL_HLPD[FCMAX];                /* nalooptijd hulpdienst ingreep op richting(en) in volgarm           */
mulv  verlos_busbaan[FCMAX];          /* buffer voor verlosmelding met prioriteit                           */
bool iPRIO[FCMAX];                   /* prioriteit toegekend aan richting                                  */
bool A_DST[FCMAX];                   /* vaste aanvraag gewenst als gevolg van detectie storingen           */
bool MK_DST[FCMAX];                  /* star verlengen gewenst als gevolg van detectie storingen           */

mulv  PEL_UIT_VTG[FCMAX];             /* buffer aantal voertuig voor uitgaande peloton koppeling            */
mulv  PEL_UIT_RES[FCMAX];             /* restant minimale duur uitsturing koppelsignaal peloton koppeling   */

mulv  verklik_srm;                    /* restant duur verklikking SRM bericht                               */
mulv  duur_geen_srm = 0;              /* aantal minuten dat geen SRM bericht is ontvangen (maximum = 32000) */
mulv  tkarog_old = 0;                 /* buffer oude waarde tijdelement tkarog                              */

bool RAT[FCMAX];                     /* aansturing rateltikker                                             */
bool RAT_test[FCMAX];                /* aansturing rateltikker in testomgeving (specifiek voor Accross)    */
bool RAT_aanvraag[FCMAX];            /* aansturing rateltikker is op aanvraag                              */
bool RAT_continu[FCMAX];             /* aansturing rateltikker is continu                                  */
bool KNIP;                           /* hulpwaarde voor knipper signaal                                    */
bool REGEN;                          /* regensensor aktief (zelf te besturen in REG[]ADD)                  */
bool WT_TE_HOOG;                     /* wachttijd te hoog voor toekennen prioriteit                        */
bool PEL_WT_TE_HOOG;                 /* wachttijd te hoog voor toekennen prioriteit peloton ingreep        */
bool GEEN_OV_PRIO;                   /* geen prioriteit OV   (zelf te besturen in REG[]ADD)                */
bool GEEN_VW_PRIO;                   /* geen prioriteit VW   (zelf te besturen in REG[]ADD)                */
bool GEEN_FIETS_PRIO;                /* geen fietsprioriteit (zelf te besturen in REG[]ADD)                */

mulv  OV_stipt[FCMAX];                /* buffer stiptheid ingemelde bussen obv KAR                          */
mulv  US_OV_old[FCMAX];               /* buffer US[] uitsturing geconditioneerde prioriteit                 */

bool DF[DPMAX];                      /* detectie fout aanwezig                                             */
mulv  D_bez[DPMAX];                   /* tijdsduur detector bezet                                           */
mulv  D_onb[DPMAX];                   /* tijdsduur detector onbezet                                         */
mulv  sec_teller;                     /* actuele duur intensiteitsmeting                                    */
mulv  kwartier[DPMAX];                /* kwartier intensiteit                                               */
mulv  Iactueel[DPMAX];                /* actuele stand meting                                               */

count ML_REG_MAX;                     /* maximum aantal modulen zonder DVM maatregelen                      */
count ML_DVM_MAX;                     /* maximum aantal modulen bij inzet DVM maatregelen                   */
count ML_ACT_MAX;                     /* actueel aantal modulen                                             */
count PRML_REG[MLMAX][FCMAX];         /* module indeling zonder DVM maatregelen                             */
count PRML_DVM[MLMAX][FCMAX];         /* module indeling bij inzet DVM maatregelen                          */
bool DVM_structuur_gewenst;          /* module indeling bij inzet DVM maatregelen gewenst                  */
bool DVM_structuur_actief;           /* module indeling bij inzet DVM maatregelen actief                   */

mulv  DVM_klok;                       /* DVM programma - klok wens                                          */
mulv  DVM_parm;                       /* DVM programma - parameter instelling                               */
mulv  DVM_prog;                       /* DVM programma - actief                                             */
mulv  DVM_prog_duur;                  /* DVM programma - actuele duur instelling PRM[dvmpr] in uren         */
bool DVM_structuur;                  /* DVM module structuur gewenst (zelf te besturen in REG[]ADD)        */
bool DVM_structuur_act;              /* DVM module structuur actief                                        */
bool max_verleng_groen;              /* keuze maximumgroen of maximum verlenggroentijden                   */

mulv  FILE_set;                       /* set maximum(verleng)groentijden gewenst a.g.v. FILE stroomopwaarts */
bool FILE_nass;                      /* FILE stroomafwaarts (= na stopstreep) aanwezig                     */

mulv  SLAVE;                          /* t.b.v. Master-Slave: SLAVE  in regelen                             */
mulv  MASTER;                         /* t.b.v. Master-Slave: MASTER in regelen                             */
bool SLAVE_VRI;                      /* t.b.v. Master-Slave: VRI is SLAVE                                  */
bool MASTER_VRI;                     /* t.b.v. Master-Slave: VRI is MASTER                                 */
bool slave_fc[FCMAX];                /* t.b.v. Master-Slave: richting is gedefinieerd in de SLAVE          */

struct hki_koppeling  hki_kop[MAX_HKI_KOP];
struct vtg_koppeling  vtg_tgo[MAX_VTG_KOP];
struct lvk_gelijkstr  lvk_gst[MAX_LVK_GST];
struct dcf_voorstart  dcf_vst[MAX_DCF_VST];
struct dcf_gelijkstr  dcf_gst[MAX_DCF_GST];
struct meerealisatie  mee_rea[MAX_MEE_REA];
struct pel_koppeling  pel_kop[MAX_PEL_KOP];
struct fietsvoorrang  fts_pri[MAX_FTS_PRI];
struct prioriteit_id  prio_index[FCMAX];
struct afteller       aft_123[FCMAX];
struct max_groen_DVM  DVM_max[FCMAX];
struct max_groen_FILE FILE_max[FCMAX];

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
char  _UUR[MAXDUMPSTAP];              /* bijhouden UUR tbv flight buffer                                    */
char  _MIN[MAXDUMPSTAP];              /* bijhouden MIN tbv flight buffer                                    */
char  _SEC[MAXDUMPSTAP];              /* bijhouden SEC tbv flight buffer                                    */
mulv  _ML[MAXDUMPSTAP];               /* bijhouden ML  tbv flight buffer                                    */
char  _FC[FCMAX][MAXDUMPSTAP];        /* bijhouden fasecyclus status tbv flight buffer                      */
char  _FCA[FCMAX][MAXDUMPSTAP];       /* bijhouden aanvraag   status tbv flight buffer                      */
bool _HA[FCMAX];                     /* hulpwaarde A[] tbv start- en einde puls aanvraag in flight buffer  */
mulv dumpstap;                        /* interne teller flight buffer                                       */
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie initialiseer variabelen Traffick2TLCGen                                                          */
/* -------------------------------------------------------------------------------------------------------- */
/* In deze functie worden alle toegevoegde variabelen geinitialiseerd.                                      */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_init_application().                                                */
/*                                                                                                          */
void init_traffick2tlcgen(void)       /* Fik240224                                                          */
{
    count i, j, fc, ml;

    aantal_hki_kop = 0;                /* aantal harde koppelingen                                           */
    aantal_vtg_tgo = 0;                /* aantal voetgangerskoppelingen - type gescheiden oversteek          */
    aantal_lvk_gst = 0;                /* aantal gelijk starten langzaam verkeer                             */
    aantal_dcf_vst = 0;                /* aantal deelconflicten voorstart                                    */
    aantal_dcf_gst = 0;                /* aantal deelconflicten gelijkstart                                  */
    aantal_mee_rea = 0;                /* aantal meerealisaties                                              */
    aantal_pel_kop = 0;                /* aantal peloton koppelingen                                         */
    aantal_fts_pri = 0;                /* aantal definities fiets voorrang module                            */
    aantal_aft_123 = 0;                /* aantal definities aftellers                                        */
    aantal_dvm_prg = 0;                /* aantal DVM netwerk programma's                                     */
    aantal_file_prg = 0;                /* aantal FILE programma's (stroomopwaarts)                           */

    KNIP = FALSE;            /* hulpwaarde voor knipper signaal                                    */
    REGEN = FALSE;            /* regensensor aktief (zelf te besturen in REG[]ADD)                  */
    WT_TE_HOOG = FALSE;            /* wachttijd te hoog voor toekennen prioriteit                        */
    PEL_WT_TE_HOOG = FALSE;            /* wachttijd te hoog voor toekennen prioriteit peloton ingreep        */
    GEEN_OV_PRIO = FALSE;            /* geen prioriteit OV   (zelf te besturen in REG[]ADD)                */
    GEEN_VW_PRIO = FALSE;            /* geen prioriteit VW   (zelf te besturen in REG[]ADD)                */
    GEEN_FIETS_PRIO = FALSE;            /* geen fietsprioriteit (zelf te besturen in REG[]ADD)                */
    verklik_srm = 0;                /* restant duur verklikking SRM bericht                               */
    FILE_set = 0;                /* set maximum(verleng)groentijden gewenst a.g.v. FILE stroomopwaarts */
    FILE_nass = FALSE;            /* FILE stroomafwaarts (= na stopstreep) aanwezig                     */

    for (i = 0; i < FCMAX; ++i)
    {
        TRG_min_type |= RO_type;     /* ondergrens garantie rood  read only                                */
        TGG_type &= ~RO_type;     /* bovengrens garantie wel instelbaar                                 */
        TGG_min_type |= RO_type;     /* ondergrens garantie groen read only                                */

        REALtraffick[i] = 0;            /* REALTIJD voor richtingen die als eerstvolgende aan de beurt zijn   */
        PARtraffick[i] = FALSE;        /* buffer PAR[] zoals bepaald door Traffick                           */
        AAPRprio[i] = FALSE;        /* AAPR[] voor prioriteitsrealisaties                                 */
        PFPRtraffick[i] = 0;            /* aantal modulen dat vooruit gerealiseerd mag worden                 */
        AltRuimte[i] = 0;            /* realisatie ruimte voor alternatieve realisatie                     */
        ART[i] = FALSE;        /* alternatieve realisatie toegestaan algemene schakelaar             */
        ARB[i] = NG;           /* alternatieve realisatie toegestaan verfijning per blok             */
        MGR[i] = FALSE;        /* meeverleng groen                                                   */
        MMK[i] = FALSE;        /* meeverleng groen alleen als MK[] waar is                           */
        BMC[i] = FALSE;        /* beeindig meeverleng groen conflicten                               */
        WGR[i] = FALSE;        /* wachtstand groen                                                   */
        NAL[i] = FALSE;        /* naloop als gevolg van harde koppeling actief                       */
        FC_DVM[i] = FALSE;        /* richting wordt bevoordeeld als gevolg van DVM                      */
        FC_FILE[i] = FALSE;        /* richting wordt bevoordeeld als gevolg van FILE stroomopwaarts      */
        HerstartOntruim[i] = FALSE;        /* richting met LHORVA functie R herstart ontruiming vanaf conflicten */
        ExtraOntruim[i] = 0;            /* extra ontruiming als gevolg van LHORVA functie R                   */
        HOT[i] = FALSE;        /* startpuls roodlichtrijder ten behoeve van LHORVA functie R         */
        VG_mag[i] = FALSE;        /* veiligheidsgroen mag worden aangehouden (er is een hiaat gemeten)  */
        GWT[i] = 0;            /* gewogen wachttijd tbv toekennen alternatieve realisatie            */
        TEG[i] = NG;           /* tijd tot einde groen                                               */
        MTG[i] = 0;            /* minimale tijd tot groen                                            */
        mmk_old[i] = 0;            /* buffer MM[mmk] - geheugenelement MK[] bij gescheiden hiaatmeting   */
        MK_old[i] = 0;            /* buffer MK[]                                                        */

        TVG_instelling[i] = 0;            /* buffer ingestelde waarde TVG_max[]                                 */
        TGL_instelling[i] = 0;            /* buffer ingestelde waarde TGL_max[]                                 */

        DOSEER[i] = FALSE;        /* doseren aktief                                                     */
        DOSMAX[i] = NG;           /* doseer maximum                                                     */
        DOS_RD[i] = 0;            /* minimale tijd tot startgroen als gevolg van doseren                */
        MINTSG[i] = 0;            /* minimale tijd tot startgroen (zelf te besturen in REG[]ADD)        */
        MINTEG[i] = 0;            /* minimale tijd tot eindegroen (zelf te besturen in REG[]ADD)        */
        PELTEG[i] = NG;           /* tijd tot einde groen als peloton ingreep maximaal duurt            */

        TOE123[i] = TRUE;         /* toestemming 1-2-3 afteller (zelf te besturen in REG[]ADD)          */
        Waft[i] = 0;            /* aftellerwaarde ( > 0 betekent dat 1-2-3 afteller loopt)            */
        Aaft[i] = 0;            /* aftellerwaarde ( = 1, 2 of 3 )                                     */
        Aled[i] = 0;            /* aantal resterende leds bij wachttijd voorspeller                   */
        AanDuurLed[i] = 0;            /* tijd dat huidige aantal leds wordt uitgestuurd                     */
        TijdPerLed[i] = 0;            /* tijdsduur per led voor gelijkmatige afloop wachttijd voorspeller   */
        wacht_ML[i] = NG;           /* maximale wachttijd volgens de module molen                         */

        ARM[i] = NG;           /* kruispunt arm tbv HLPD prioriteit                                  */
        volg_ARM[i] = NG;           /* kruispunt volgarm tbv doorzetten HLPD prioriteit                   */
        HD_aanwezig[i] = FALSE;        /* HLPD aanwezig op richting                                          */
        HLPD[i] = FALSE;        /* HLPD prioriteit toegekend aan richting (en volgrichtingen)         */
        NAL_HLPD[i] = 0;            /* nalooptijd hulpdienst ingreep op richting(en) in volgarm           */
        verlos_busbaan[i] = 0;            /* buffer voor verlosmelding met prioriteit                           */
        iPRIO[i] = FALSE;        /* prioriteit toegekend aan richting                                  */
        A_DST[i] = FALSE;        /* vaste aanvraag gewenst als gevolg van detectie storingen           */
        MK_DST[i] = FALSE;        /* star verlengen gewenst als gevolg van detectie storingen           */

        PEL_UIT_VTG[i] = 0;            /* buffer aantal voertuig voor uitgaande peloton koppeling obv pulsen */
        PEL_UIT_RES[i] = 0;            /* restant duur uitsturing koppelsignaal peloton koppeling obv pulsen */

        RAT[i] = FALSE;        /* aansturing rateltikker                                             */
        RAT_test[i] = FALSE;        /* aansturing rateltikker in testomgeving (specifiek voor Accross)    */
        RAT_aanvraag[i] = FALSE;        /* aansturing rateltikker is op aanvraag                              */
        RAT_continu[i] = FALSE;        /* aansturing rateltikker is continu                                  */
        OV_stipt[i] = 0;            /* buffer stiptheid ingemelde bussen obv KAR                          */
        US_OV_old[i] = NG;           /* buffer US[] uitsturing geconditioneerde prioriteit                 */

        for (j = 0; j < FCMAX; ++j)
        {
            TMPc[i][j] = NG;
            TMPi[i][j] = NG;
        }
    }

    for (i = 0; i < DPMAX; ++i)
    {
        DF[i] = FALSE;        /* detectie fout aanwezig                                             */
        D_bez[i] = 0;            /* tijdsduur detector bezet                                           */
        D_onb[i] = 0;            /* tijdsduur detector onbezet                                         */
        kwartier[i] = 0;            /* kwartier intensiteit                                               */
        Iactueel[i] = 0;            /* actuele stand meting                                               */
    }

    sec_teller = 0;            /* actuele duur intensiteitsmeting                                    */
    ML_REG_MAX = 0;            /* bepaal aantal modulen in regeling DVM maatregelen                  */
    /* ... en initialiseer PRML_REG[][] en PRML_DVM[][]                   */
    for (ml = 0; ml < MLMAX; ++ml)
    {
        for (fc = 0; fc < FCMAX; ++fc)
        {
            if (PRML[ml][fc] == PRIMAIR)
            {
                PRML_REG[ml][fc] = PRIMAIR;
                ML_REG_MAX = ml + 1;
            }
            else
            {
                PRML_REG[ml][fc] = FALSE;
            }
            PRML_DVM[ml][fc] = FALSE;
        }
    }

    ML_DVM_MAX = NG;           /* maximum aantal modulen bij inzet DVM maatregelen                   */
    ML_ACT_MAX = ML_REG_MAX;   /* actueel aantal modulen                                             */
    DVM_structuur_gewenst = FALSE;      /* module indeling bij inzet DVM maatregelen gewenst                  */
    DVM_structuur_actief = FALSE;      /* module indeling bij inzet DVM maatregelen actief                   */

    DVM_klok = 0;            /* DVM programma - klok wens                                          */
    DVM_parm = 0;            /* DVM programma - parameter instelling                               */
    DVM_prog = 0;            /* DVM programma - actief                                             */
    DVM_prog_duur = 0;            /* DVM programma - actuele duur instelling PRM[netpr] in minuten      */
    DVM_structuur = FALSE;        /* DVM module structuur gewenst                                       */
    DVM_structuur_act = FALSE;        /* DVM module structuur actief                                        */
    max_verleng_groen = FALSE;        /* keuze maximumgroen of maximum verlenggroentijden                   */

    for (i = 0; i < MAX_HKI_KOP; ++i)   /* index buffer harde koppelingen                                     */
    {
        hki_kop[i].fc1 = NG;         /* FC    voedende richting                                            */
        hki_kop[i].fc2 = NG;         /* FC    volg     richting                                            */
        hki_kop[i].tlr21 = NG;         /* TM    late release fc2 (= inrijtijd)                               */
        hki_kop[i].tnlfg12 = NG;         /* TM    vaste   nalooptijd vanaf einde vastgroen      fc1            */
        hki_kop[i].tnlfgd12 = NG;         /* TM    det.afh.nalooptijd vanaf einde vastgroen      fc1            */
        hki_kop[i].tnleg12 = NG;         /* TM    vaste   nalooptijd vanaf einde (verleng)groen fc1            */
        hki_kop[i].tnlegd12 = NG;         /* TM    det.afh.nalooptijd vanaf einde (verleng)groen fc1            */
        hki_kop[i].kop_eg = FALSE;      /* bool koppeling vanaf einde groen (als FALSE dan vanaf EV)         */
        hki_kop[i].los_fc2 = FALSE;      /* bool fc2 mag bij aanvraag fc1 los realiseren                      */
        hki_kop[i].kop_max = 0;          /* mulv  maximum verlenggroen na harde koppeling                      */
        hki_kop[i].status = NG;         /* mulv  status koppeling                                             */
    }

    for (i = 0; i < MAX_LVK_GST; ++i)   /* index buffer gelijk starten langzaam verkeer                       */
    {
        lvk_gst[i].fc1 = NG;         /* FC   richting 1                                                    */
        lvk_gst[i].fc2 = NG;         /* FC   richting 2                                                    */
        lvk_gst[i].fc3 = NG;         /* FC   richting 3                                                    */
        lvk_gst[i].fc4 = NG;         /* FC   richting 4                                                    */
    }

    for (i = 0; i < MAX_VTG_KOP; ++i)   /* index buffer voetgangerskoppelingen - type gescheiden oversteek    */
    {
        vtg_tgo[i].fc1 = NG;         /* FC   voedende richting                                             */
        vtg_tgo[i].fc2 = NG;         /* FC   volg     richting                                             */
        vtg_tgo[i].tinl12 = NG;         /* TM   inlooptijd fc1                                                */
        vtg_tgo[i].tinl21 = NG;         /* TM   inlooptijd fc2                                                */
        vtg_tgo[i].tnlsgd12 = NG;         /* TM   nalooptijd fc2 vanaf startgroen fc1                           */
        vtg_tgo[i].tnlsgd21 = NG;         /* TM   nalooptijd fc1 vanaf startgroen fc2                           */
        vtg_tgo[i].hnla12 = NG;         /* HE   drukknop melding koppeling vanaf fc1 aanwezig                 */
        vtg_tgo[i].hnla21 = NG;         /* HE   drukknop melding koppeling vanaf fc2 aanwezig                 */
        vtg_tgo[i].hlos1 = NG;         /* HE   los realiseren fc1 toegestaan                                 */
        vtg_tgo[i].hlos2 = NG;         /* HE   los realiseren fc2 toegestaan                                 */
        vtg_tgo[i].status12 = NG;         /* mulv status koppeling fc1 -> fc2                                   */
        vtg_tgo[i].status21 = NG;         /* mulv status koppeling fc2 -> fc1                                   */
    }

    for (i = 0; i < MAX_DCF_VST; ++i)   /* index buffer deelconflicten voorstart                              */
    {
        dcf_vst[i].fc1 = NG;         /* FC  richting die voorstart geeft                                   */
        dcf_vst[i].fc2 = NG;         /* FC  richting die voorstart krijgt                                  */
        dcf_vst[i].tvs21 = NG;         /* TM  voorstart fc2                                                  */
        dcf_vst[i].to12 = NG;         /* TM  ontruimingstijd van fc1 naar fc2                               */
        dcf_vst[i].ma21 = NG;         /* SCH meerealisatie van fc2 met fc1                                  */
        dcf_vst[i].mv21 = NG;         /* SCH meeverlengen  van fc2 met fc1                                  */
    }

    for (i = 0; i < MAX_DCF_GST; ++i)   /* index buffer deelconflicten gelijkstart                            */
    {
        dcf_gst[i].fc1 = NG;         /* FC  richting 1                                                     */
        dcf_gst[i].fc2 = NG;         /* FC  richting 2                                                     */
        dcf_gst[i].to12 = NG;         /* TM  ontruimingstijd van fc1 naar fc2                               */
        dcf_gst[i].to21 = NG;         /* TM  ontruimingstijd van fc2 naar fc1                               */
        dcf_gst[i].ma12 = NG;         /* SCH meerealisatie van fc1 met fc2                                  */
        dcf_gst[i].ma21 = NG;         /* SCH meerealisatie van fc2 met fc1                                  */
    }

    for (i = 0; i < MAX_MEE_REA; ++i)   /* index buffer meerealisaties                                        */
    {
        mee_rea[i].fc1 = NG;         /* FC    richting die meerealisatie geeft                             */
        mee_rea[i].fc2 = NG;         /* FC    richting die meerealisatie krijgt                            */
        mee_rea[i].ma21 = NG;         /* SCH   meerealisatie van fc2 met fc1                                */
        mee_rea[i].mv21 = NG;         /* SCH   meeverlengen  van fc2 met fc1                                */
        mee_rea[i].mr2v = FALSE;      /* bool meerealisatie aan fc2 verstrekt                              */
    }

    for (i = 0; i < MAX_PEL_KOP; ++i)   /* index buffer peloton koppelingen                                   */
    {
        pel_kop[i].kop_fc = NG;      /* FC    koppelrichting                                               */
        pel_kop[i].kop_toe = NG;      /* ME    toestemming peloton ingreep (bij NG altijd toestemming)      */
        pel_kop[i].kop_sig = NG;      /* HE    koppelsignaal                                                */
        pel_kop[i].kop_bew = NG;      /* TM    bewaak koppelsignaal (bij NG wordt een puls veronderstelt)   */
        pel_kop[i].aanv_vert = NG;      /* TM    aanvraag vertraging  (bij NG wordt geen aanvraag opgzet)     */
        pel_kop[i].vast_vert = NG;      /* TM    vasthoud vertraging  (start op binnenkomst koppelsignaal)    */
        pel_kop[i].duur_vast = NG;      /* TM    duur vasthouden (bij duursign. na afvallen koppelsignaal)    */
        pel_kop[i].duur_verl = NG;      /* TM    duur verlengen na ingreep (bij NUL geldt TVG_max[])          */
        pel_kop[i].hnaloop_1 = NG;      /* HE    voorwaarde herstart extra nalooptijd 1 (nalooplus 1)         */
        pel_kop[i].tnaloop_1 = NG;      /* TM    nalooptijd 1                                                 */
        pel_kop[i].hnaloop_2 = NG;      /* HE    voorwaarde herstart extra nalooptijd 2 (nalooplus 2)         */
        pel_kop[i].tnaloop_2 = NG;      /* TM    nalooptijd 2                                                 */
        pel_kop[i].verklik = NG;      /* US    verklik peloton ingreep                                      */
        pel_kop[i].kop_oud = FALSE;   /* bool status koppelsignaal vorige machine slag                     */
        pel_kop[i].aanw_kop1 = NG;      /* mulv  aanwezigheidsduur koppelsignaal 1 vanaf start puls           */
        pel_kop[i].duur_kop1 = NG;      /* mulv  tijdsduur HOOG    koppelsignaal 1 igv duur signaal           */
        pel_kop[i].aanw_kop2 = NG;      /* mulv  aanwezigheidsduur koppelsignaal 2 vanaf start puls           */
        pel_kop[i].duur_kop2 = NG;      /* mulv  tijdsduur HOOG    koppelsignaal 2 igv duur signaal           */
        pel_kop[i].aanw_kop3 = NG;      /* mulv  aanwezigheidsduur koppelsignaal 3 vanaf start puls           */
        pel_kop[i].duur_kop3 = NG;      /* mulv  tijdsduur HOOG    koppelsignaal 3 igv duur signaal           */
        pel_kop[i].pk_status = NG;      /* mulv  status peloton ingreep                                       */
        pel_kop[i].pk_afronden = FALSE;   /* bool afronden lopende peloton ingreep                             */
        pel_kop[i].buffervol = FALSE;   /* bool buffers voor peloton ingreep vol                             */
    }

    for (i = 0; i < MAX_FTS_PRI; ++i)   /* index buffer fiets voorrang module                                 */
    {
        fts_pri[i].fc = NG;         /* FC    fietsrichting                                                */
        fts_pri[i].drk1 = NG;         /* DE    drukknop 1 voor aanvraag prioriteit                          */
        fts_pri[i].drk2 = NG;         /* DE    drukknop 2 voor aanvraag prioriteit                          */
        fts_pri[i].de1 = NG;         /* DE    koplus   1 voor aanvraag prioriteit                          */
        fts_pri[i].de2 = NG;         /* DE    koplus   2 voor aanvraag prioriteit                          */
        fts_pri[i].inmeld = NG;         /* HE    hulp element voor prioriteitsmodule (in.melding prioriteit)  */
        fts_pri[i].uitmeld = NG;         /* HE    hulp element voor prioriteitsmodule (uitmelding prioriteit)  */
        fts_pri[i].ogwt_fts = NG;         /* TM    ondergrens wachttijd voor prioriteit                         */
        fts_pri[i].prio_fts = NG;         /* PRM   prioriteitscode                                              */
        fts_pri[i].ogwt_reg = NG;         /* TM    ondergrens wachttijd voor prioriteit (indien REGEN == TRUE)  */
        fts_pri[i].prio_reg = NG;         /* PRM   prioriteitscode                      (indien REGEN == TRUE)  */
        fts_pri[i].verklik = NG;         /* US    verklik fiets prioriteit                                     */
        fts_pri[i].aanvraag = FALSE;      /* bool fietser is op juiste wijze aangevraagd                       */
        fts_pri[i].prio_vw = FALSE;      /* bool fietser voldoet aan prioriteitsvoorwaarden                   */
        fts_pri[i].prio_av = FALSE;      /* bool fietser is met prioriteit aangevraagd                        */
    }

    for (i = 0; i < FCMAX; ++i)         /* index buffer prioriteit                                            */
    {
        prio_index[i].KAR_id_OV = NG;     /* mulv  KAR id openbaar vervoer                                      */
        prio_index[i].KAR_id_HD = NG;     /* mulv  KAR id nood- en hulpdiensten                                 */
        prio_index[i].HD = NG;     /* count hulpdienst ingreep                                           */
        prio_index[i].OV_kar = NG;     /* count OV ingreep - KAR                                             */
        prio_index[i].OV_srm = NG;     /* count OV ingreep - SRM                                             */
        prio_index[i].OV_verlos = NG;     /* count OV ingreep - verlos                                          */
        prio_index[i].VRW = NG;     /* count VRW ingreep                                                  */
        prio_index[i].FTS = NG;     /* count fiets voorrang module                                        */
        prio_index[i].usHD = NG;     /* US    verklik HD ingreep                                           */
        prio_index[i].usOV_kar = NG;     /* US    verklik OV ingreep - KAR                                     */
        prio_index[i].usOV_srm = NG;     /* US    verklik OV ingreep - SRM                                     */
        prio_index[i].usVRW = NG;     /* US    verklik VRW ingreep                                          */
    }

    for (i = 0; i < FCMAX; ++i)         /* index buffer prioriteit                                            */
    {
        aft_123[i].fc = NG;        /* FC    richting met afteller                                        */
        aft_123[i].de1 = NG;        /* DE    koplus 1                                                     */
        aft_123[i].de2 = NG;        /* DE    koplus 2                                                     */
        aft_123[i].de3 = NG;        /* DE    koplus 3                                                     */
        aft_123[i].toest = NG;        /* SCH   toestemming aansturing afteller                              */
        aft_123[i].min_duur = NG;        /* PRM   minimale duur tot start groen waarbij afteller mag starten   */
        aft_123[i].tel_duur = NG;        /* PRM   duur van een tel in tienden van seconden                     */
        aft_123[i].is_oke_1 = NG;        /* IS    afteller lantaarn 1 werkt correct                            */
        aft_123[i].is_oke_2 = NG;        /* IS    afteller lantaarn 2 werkt correct                            */
        aft_123[i].is_oke_3 = NG;        /* IS    afteller lantaarn 3 werkt correct                            */
        aft_123[i].is_oke_4 = NG;        /* IS    afteller lantaarn 4 werkt correct                            */
        aft_123[i].us_oke = NG;        /* US    aftellers van alle lantaarns werken correct                  */
        aft_123[i].us_getal = NG;        /* US    tbv verklikking op bedienpaneel                              */
        aft_123[i].us_bit0 = NG;        /* US    aansturing afteller BIT0                                     */
        aft_123[i].us_bit1 = NG;        /* US    aansturing afteller BIT1                                     */
        aft_123[i].aftel_ok = FALSE;     /* bool alle aftellers van een rijrichting zijn OK                   */
        aft_123[i].act_tel = 0;         /* mulv  actuele stand afteller                                       */
        aft_123[i].act_duur = FALSE;     /* mulv  tijd in tienden van seconden dat TEL wordt aangestuurd       */
    }

    for (i = 0; i < FCMAX; ++i)         /* index buffer maximum (verleng)groen tijdens DVM netwerkprogramma's */
    {
        DVM_max[i].fc = NG;               /* FC  fasecyclus                                                     */
        for (j = 0; j < MAX_DVM_PRG; ++j)
        {
            DVM_max[i].dvm_set[j] = NG;     /* PRM maximum(verleng)groen tijdens DVM netwerk programma's          */
        }
    }

    for (i = 0; i < FCMAX; ++i)         /* index buffer maximum (verleng)groen tijdens FILEprg stroomopwaarts */
    {
        FILE_max[i].fc = NG;              /* FC  fasecyclus                                                     */
        for (j = 0; j < MAX_FILE_PRG; ++j)
        {
            FILE_max[i].file_set[j] = NG;   /* PRM maximum(verleng)groen tijdens FILE programma's stroomopwaarts  */
        }
    }

    SLAVE = 0;                     /* t.b.v. Master-Slave: SLAVE  in regelen                             */
    MASTER = 0;                     /* t.b.v. Master-Slave: MASTER in regelen                             */
    SLAVE_VRI = FALSE;                 /* t.b.v. Master-Slave: VRI is SLAVE                                  */
    MASTER_VRI = FALSE;                 /* t.b.v. Master-Slave: VRI is MASTER                                 */
    for (i = 0; i < FCMAX; ++i)
    {
        slave_fc[i] = FALSE;              /* t.b.v. Master-Slave: richting is gedefinieerd in de SLAVE          */
    }

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    dumpstap = 0;
    for (i = 0; i < MAXDUMPSTAP; ++i)
    {
        _UUR[i] = ' ';
        _MIN[i] = ' ';
        _SEC[i] = ' ';
        _ML[i] = 0;
    }

    for (i = 0; i < FCMAX; ++i)
    {
        _HA[i] = FALSE;
        for (j = 0; j < MAXDUMPSTAP; ++j)
        {
            _FC[i][j] = ' ';
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
    bool kop_eg,                         /* bool koppeling vanaf einde groen (als FALSE dan vanaf EV)         */
    bool los_fc2,                        /* bool fc2 mag bij aanvraag fc1 los realiseren                      */
    mulv  kop_max)                        /* mulv  maximum verlenggroen na harde koppeling                      */
{
    if (aantal_hki_kop < MAX_HKI_KOP)
    {
        hki_kop[aantal_hki_kop].fc1 = fc1;
        hki_kop[aantal_hki_kop].fc2 = fc2;
        hki_kop[aantal_hki_kop].tlr21 = tlr21;
        hki_kop[aantal_hki_kop].tnlfg12 = tnlfg12;
        hki_kop[aantal_hki_kop].tnlfgd12 = tnlfgd12;
        hki_kop[aantal_hki_kop].tnleg12 = tnleg12;
        hki_kop[aantal_hki_kop].tnlegd12 = tnlegd12;
        hki_kop[aantal_hki_kop].kop_eg = kop_eg;
        hki_kop[aantal_hki_kop].los_fc2 = los_fc2;
        hki_kop[aantal_hki_kop].kop_max = kop_max;
        hki_kop[aantal_hki_kop].status = 0;
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
        vtg_tgo[aantal_vtg_tgo].fc1 = fc1;
        vtg_tgo[aantal_vtg_tgo].fc2 = fc2;
        vtg_tgo[aantal_vtg_tgo].tinl12 = tinl12;
        vtg_tgo[aantal_vtg_tgo].tinl21 = tinl21;
        vtg_tgo[aantal_vtg_tgo].tnlsgd12 = tnlsgd12;
        vtg_tgo[aantal_vtg_tgo].tnlsgd21 = tnlsgd21;
        vtg_tgo[aantal_vtg_tgo].hnla12 = hnla12;
        vtg_tgo[aantal_vtg_tgo].hnla21 = hnla21;
        vtg_tgo[aantal_vtg_tgo].hlos1 = hlos1;
        vtg_tgo[aantal_vtg_tgo].hlos2 = hlos2;
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
    count mv21)                           /* SCH meeverlengen  van fc2 met fc1                                  */
{
    if (aantal_dcf_vst < MAX_DCF_VST)
    {
        dcf_vst[aantal_dcf_vst].fc1 = fc1;
        dcf_vst[aantal_dcf_vst].fc2 = fc2;
        dcf_vst[aantal_dcf_vst].tvs21 = tvs21;
        dcf_vst[aantal_dcf_vst].to12 = to12;
        dcf_vst[aantal_dcf_vst].ma21 = ma21;
        dcf_vst[aantal_dcf_vst].mv21 = mv21;
        TMPi[fc1][fc2] = 0;
        aantal_dcf_vst++;
    }
}


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
    count ma21)                           /* SCH meerealisatie van fc2 met fc1                                  */
{
    if (aantal_dcf_gst < MAX_DCF_GST)
    {
        dcf_gst[aantal_dcf_gst].fc1 = fc1;
        dcf_gst[aantal_dcf_gst].fc2 = fc2;
        dcf_gst[aantal_dcf_gst].to12 = to12;
        dcf_gst[aantal_dcf_gst].to21 = to21;
        dcf_gst[aantal_dcf_gst].ma12 = ma12;
        dcf_gst[aantal_dcf_gst].ma21 = ma21;
        TMPi[fc1][fc2] = TMPi[fc2][fc1] = 0;
        aantal_dcf_gst++;
    }
}


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
    count mv21)                           /* SCH meeverlengen  van fc2 met fc1                                  */
{
    if (aantal_mee_rea < MAX_MEE_REA)
    {
        mee_rea[aantal_mee_rea].fc1 = fc1;
        mee_rea[aantal_mee_rea].fc2 = fc2;
        mee_rea[aantal_mee_rea].ma21 = ma21;
        mee_rea[aantal_mee_rea].mv21 = mv21;
        mee_rea[aantal_mee_rea].mr2v = FALSE;
        aantal_mee_rea++;
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
    count kop_fc,                         /* FC koppelrichting                                                  */
    count kop_toe,                        /* ME toestemming peloton ingreep (bij NG altijd toestemming)         */
    count kop_sig,                        /* HE koppelsignaal                                                   */
    count kop_bew,                        /* TM bewaak koppelsignaal (bij NG wordt een puls veronderstelt)      */
    count aanv_vert,                      /* TM aanvraag vertraging  (bij NG wordt geen aanvraag opgzet)        */
    count vast_vert,                      /* TM vasthoud vertraging  (start op binnenkomst koppelsignaal)       */
    count duur_vast,                      /* TM duur vasthouden (bij duursign. na afvallen koppelsignaal)       */
    count duur_verl,                      /* TM duur verlengen na ingreep (bij NUL geldt TVG_max[])             */
    count hnaloop_1,                      /* HE voorwaarde herstart extra nalooptijd 1 (nalooplus 1)            */
    count tnaloop_1,                      /* TM nalooptijd 1                                                    */
    count hnaloop_2,                      /* HE voorwaarde herstart extra nalooptijd 2 (nalooplus 2)            */
    count tnaloop_2,                      /* TM nalooptijd 2                                                    */
    count verklik)                        /* US verklik peloton ingreep                                         */
{
    if (aantal_pel_kop < MAX_PEL_KOP)
    {
        pel_kop[aantal_pel_kop].kop_fc = kop_fc;
        pel_kop[aantal_pel_kop].kop_toe = kop_toe;
        pel_kop[aantal_pel_kop].kop_sig = kop_sig;
        pel_kop[aantal_pel_kop].kop_bew = kop_bew;
        pel_kop[aantal_pel_kop].aanv_vert = aanv_vert;
        pel_kop[aantal_pel_kop].vast_vert = vast_vert;
        pel_kop[aantal_pel_kop].duur_vast = duur_vast;
        pel_kop[aantal_pel_kop].duur_verl = duur_verl;
        pel_kop[aantal_pel_kop].hnaloop_1 = hnaloop_1;
        pel_kop[aantal_pel_kop].tnaloop_1 = tnaloop_1;
        pel_kop[aantal_pel_kop].hnaloop_2 = hnaloop_2;
        pel_kop[aantal_pel_kop].tnaloop_2 = tnaloop_2;
        pel_kop[aantal_pel_kop].verklik = verklik;
        pel_kop[aantal_pel_kop].pk_status = 0;
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
    count verklik)                        /* US  verklik fiets prioriteit                                       */
{
    if (aantal_fts_pri < MAX_FTS_PRI)
    {
        fts_pri[aantal_fts_pri].fc = fc;
        fts_pri[aantal_fts_pri].drk1 = drk1;
        fts_pri[aantal_fts_pri].drk2 = drk2;
        fts_pri[aantal_fts_pri].de1 = de1;
        fts_pri[aantal_fts_pri].de2 = de2;
        fts_pri[aantal_fts_pri].inmeld = inmeld;
        fts_pri[aantal_fts_pri].uitmeld = uitmeld;
        fts_pri[aantal_fts_pri].ogwt_fts = ogwt_fts;
        fts_pri[aantal_fts_pri].prio_fts = prio_fts;
        fts_pri[aantal_fts_pri].ogwt_reg = ogwt_reg;
        fts_pri[aantal_fts_pri].prio_reg = prio_reg;
        fts_pri[aantal_fts_pri].verklik = verklik;
        fts_pri[aantal_fts_pri].aanvraag = FALSE;
        fts_pri[aantal_fts_pri].prio_vw = FALSE;
        fts_pri[aantal_fts_pri].prio_av = FALSE;
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
    count us_bit1)                        /* US    aansturing afteller BIT1                                     */
{
    if (aantal_aft_123 < FCMAX)
    {
        aft_123[aantal_aft_123].fc = fc;
        aft_123[aantal_aft_123].de1 = de1;
        aft_123[aantal_aft_123].de2 = de2;
        aft_123[aantal_aft_123].de3 = de3;
        aft_123[aantal_aft_123].toest = toest;
        aft_123[aantal_aft_123].min_duur = min_duur;
        aft_123[aantal_aft_123].tel_duur = tel_duur;
        aft_123[aantal_aft_123].is_oke_1 = is_oke_1;
        aft_123[aantal_aft_123].is_oke_2 = is_oke_2;
        aft_123[aantal_aft_123].is_oke_3 = is_oke_3;
        aft_123[aantal_aft_123].is_oke_4 = is_oke_4;
        aft_123[aantal_aft_123].us_oke = us_oke;
        aft_123[aantal_aft_123].us_getal = us_getal;
        aft_123[aantal_aft_123].us_bit0 = us_bit0;
        aft_123[aantal_aft_123].us_bit1 = us_bit1;
        aft_123[aantal_aft_123].aftel_ok = FALSE;
        aft_123[aantal_aft_123].act_tel = 0;
        aft_123[aantal_aft_123].act_duur = 0;
        aantal_aft_123++;
    }

#ifndef AUTOMAAT
    if (is_oke_1 != NG) IS_key[is_oke_1] = TRUE;
    if (is_oke_2 != NG) IS_key[is_oke_2] = TRUE;
    if (is_oke_3 != NG) IS_key[is_oke_3] = TRUE;
    if (is_oke_4 != NG) IS_key[is_oke_4] = TRUE;
#endif
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer maximum(verleng)groen sets voor DVM netwerk programma's                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de DVM groentijden sets in een struct geplaatst.                      */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit definitie_groentijden_traffick().                                       */
/*                                                                                                          */
void definitie_max_groen_dvm_va_arg(  /* Fik230101                                                          */
    count fc, ...)                        /* FC fasecyclus                                                      */
{
    va_list argpt;                      /* variabele argumenten lijst                                         */
    mulv  prm_max_grn_dvm;              /* mulv maximum (verleng)groentijd tijdens DVM netwerkprogramma       */
    count i = 0;

    va_start(argpt, fc);                /* start variabele argumenten lijst                                   */
    do
    {
        prm_max_grn_dvm = va_arg(argpt, va_mulv);
        if ((prm_max_grn_dvm >= 0) && (i < MAX_DVM_PRG))
        {
            DVM_max[fc].dvm_set[i] = prm_max_grn_dvm;
            i++;
        }
    } while (prm_max_grn_dvm >= 0);       /* ga door tot END gevonden in argumenten lijst                       */

    va_end(argpt);                      /* maak variable argumenten lijst leeg                                */
    /* bepaal aantal aanwezige DVM netwerkprogramma's                     */
    if (i > aantal_dvm_prg) aantal_dvm_prg = i;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie definieer maximum(verleng)groen sets voor FILE programma's stroomopwaarts                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Met behulp van deze functie worden de FILE stroomopwaarts groentijden sets in een struct geplaatst.      */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit definitie_groentijden_traffick().                                       */
/*                                                                                                          */
void definitie_max_groen_file_va_arg( /* Fik230101                                                          */
    count fc, ...)                        /* FC fasecyclus                                                      */
{
    va_list argpt;                      /* variabele argumenten lijst                                         */
    mulv  prm_max_grn_file;             /* mulv maximum (verleng)groentijd tijdens FILE prg stroomopwaarts    */
    count i = 0;

    va_start(argpt, fc);                /* start variabele argumenten lijst                                   */
    do
    {
        prm_max_grn_file = va_arg(argpt, va_mulv);
        if ((prm_max_grn_file >= 0) && (i < MAX_FILE_PRG))
        {
            FILE_max[fc].file_set[i] = prm_max_grn_file;
            i++;
        }
    } while (prm_max_grn_file >= 0);      /* ga door tot END gevonden in argumenten lijst                       */

    va_end(argpt);                      /* maak variable argumenten lijst leeg                                */
    /* bepaal aantal aanwezige DVM netwerkprogramma's                     */
    if (i > aantal_file_prg) aantal_file_prg = i;
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
    bool interface_wijziging = FALSE;  /* bijhouden wijziging variabele op de interface                      */

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
        if ((US_type[i] & MVT_type) && (TA_timer[i] / 10 >= max_wt_mvt)) WT_TE_HOOG = TRUE;
        if ((US_type[i] & FTS_type) && (TA_timer[i] / 10 >= max_wt_fts)) WT_TE_HOOG = TRUE;
        if ((US_type[i] & VTG_type) && (TA_timer[i] / 10 >= max_wt_vtg)) WT_TE_HOOG = TRUE;
        if ((US_type[i] & OV_type) && (TA_timer[i] / 10 >= max_wt_mvt)) WT_TE_HOOG = TRUE;

        if (WT_TE_HOOG) break;
    }
    /* stiptheid openbaar vervoer in testomgeving                        */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
#if (defined (prmOVtstpgrensvroeg) && defined (prmOVtstpgrenslaat) && defined (prmtestdsivert))
    test_stiptheid(prmOVtstpgrensvroeg, prmOVtstpgrenslaat, prmtestdsivert);
#endif

#if (defined (prmovtstpgrensvroeg) && defined (prmovtstpgrenslaat) && defined (prmtestdsivert))
    test_stiptheid(prmovtstpgrensvroeg, prmovtstpgrenslaat, prmtestdsivert);
#endif
#endif

#ifdef schtraffick2tlcgen
    if (!SCH[schtraffick2tlcgen])
    {
        SCH[schtraffick2tlcgen] = 1;
        interface_wijziging = TRUE;
    }
#endif

#ifdef schaltijd
    if (!SCH[schaltijd])
    {
        SCH[schaltijd] = 1;
        interface_wijziging = TRUE;
    }
#endif

#ifdef schnooit
    if (SCH[schnooit])
    {
        SCH[schnooit] = 0;
        interface_wijziging = TRUE;
    }
#endif

#ifdef tnul
    if (T_max[tnul] > 0)
    {
        T_max[tnul] = 0;
        interface_wijziging = TRUE;
    }
#endif

    if (interface_wijziging)            /* variabele(n) gewijzigd op de interface                             */
    {
        CIF_PARM1WIJZAP = -2;
    }

    PEL_WT_TE_HOOG = WT_TE_HOOG;        /* default in PEL_WT_TE_HOOG hetzelfde als WT_TE_HOOG                 */
    extra_instellingen_traffick();
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie detector afhandeling Traffick2TLCGen                                                             */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de detectie afhandeling van de Traffick2TLCGen functionaliteiten.                  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit detectie_veld_afhandeling().                                            */
/*                                                                                                          */
void detector_afhandeling_va_arg(     /* Fik230726                                                          */
    count fc,                           /* FC   fasecyclus                                                    */
    count km, ...)                      /* TM   koplus maximum                                                */
{
    va_list argpt;                      /* variabele argumenten lijst                                         */
    va_list argpt2;                     /* lijst wordt 2x doorlopen                                           */

    count fc_detector;                  /* count detector behorende bij fasecyclus                            */
    count fc_hiaat_st;                  /* count vervangende hiaattijd bij detector storing                   */

    bool DrukknopAanwezig = FALSE; /* er is tenminste een drukknop aanwezig                              */
    bool KopLusAanwezig = FALSE; /* er is tenminste een koplus aanwezig                                */
    bool LangeLusAanwezig = FALSE; /* er is tenminste een lange lus aanwezig                             */
    mulv  AantalVerwegLus = 0;     /* aantal aanwezige verweg lussen                                     */

    bool AlleDrukknopDefect = TRUE;  /* alle drukknoppen zijn defect                                       */
    bool AlleKoplusDefect = TRUE;  /* alle koplussen zijn defect                                         */
    bool AllemaalDefect = TRUE;  /* alle koplussen en de 1e lange lus zijn defect                      */
    bool EenDrukknopDefect = FALSE; /* er is tenminste een drukknop defect                                */
    bool EersteLangeLusDefect = FALSE; /* eerste lange lus is defect                                         */
    bool VerwegLusDefect = FALSE; /* er is een defecte verweg lus                                       */
    bool AlleVerwegLusDefect = TRUE;  /* alle verweg lussen (behalve de eerste) zijn defect                 */

    va_start(argpt, km);                /* start variabele argumenten lijst                                   */
    va_copy(argpt2, argpt);             /* en maak een copy van de argumenten lijst voor MVT fasecycli        */

    do                                  /* lees detector lijst en bepaal detectie storingen */
    {
        fc_detector = va_arg(argpt, va_mulv);
        if (fc_detector >= 0)
        {
            fc_hiaat_st = va_arg(argpt, va_mulv);

            if (IS_type[fc_detector] == DK_type)
            {
                if (!DF[fc_detector]) AlleDrukknopDefect = FALSE;
                else                  EenDrukknopDefect = TRUE;
                DrukknopAanwezig = TRUE;
            }

            if (IS_type[fc_detector] == DKOP_type)
            {
                if (TDH_max[fc_detector] >= 0)
                {
                    if (!DF[fc_detector]) AllemaalDefect = AlleKoplusDefect = FALSE;
                    KopLusAanwezig = TRUE;
                }

                if (R[fc] && (TR_timer[fc] <= 20) && !DF[fc_detector] && ED[fc_detector])
                {
                    if ((US_type[fc] & MVT_type) || (US_type[fc] & OV_type))
                    {
                        HOT[fc] |= TRUE;          /* voertuig door rood gereden, tbv LHOVRA functie R */
                    }
                }
            }

            if ((IS_type[fc_detector] == DLNG_type) && (TDH_max[fc_detector] >= 0))
            {
                if (!LangeLusAanwezig)
                {
                    if (!DF[fc_detector]) AllemaalDefect = FALSE;
                    else                  EersteLangeLusDefect = TRUE;
                }
                LangeLusAanwezig = TRUE;
            }

            if ((IS_type[fc_detector] == DVER_type) && (TDH_max[fc_detector] >= 0))
            {
                if (DF[fc_detector]) VerwegLusDefect = TRUE;
                if ((AantalVerwegLus > 0) && !DF[fc_detector]) AlleVerwegLusDefect = FALSE;
                AantalVerwegLus++;
            }

        }
    } while (fc_detector >= 0);           /* ga door tot END gevonden in argumenten lijst */
    va_end(argpt);                      /* maak variable argumenten lijst leeg */

    if ((US_type[fc] & MVT_type) && (KopLusAanwezig || LangeLusAanwezig) && AllemaalDefect)
    {
        A_DST[fc] = TRUE;                /* vaste aanvraag MVT conform instellingen gewenst */
        MK_DST[fc] = TRUE;                /* star verlengen gewenst als gevolg van detectie storingen */
    }

    if ((US_type[fc] & MVT_type) && EersteLangeLusDefect && !AllemaalDefect)
    {
        if (FG[fc] || G[fc] /* && !MG[fc] */ && ((MK[fc] & BIT1) || (MK[fc] & BIT2)))
        {
            if (km > NG)
            {
                T[km] = TRUE;                /* koplus maximum vervalt als gevolg van detectie storingen */
                ET[km] = FALSE;
            }
        }
    }

    if ((US_type[fc] & FTS_type) && (DrukknopAanwezig || KopLusAanwezig) && AlleDrukknopDefect && AlleKoplusDefect)
    {
        A_DST[fc] = TRUE;                 /* vaste aanvraag FTS conform instellingen gewenst */
    }

    if ((US_type[fc] & FTS_type) && KopLusAanwezig && AlleKoplusDefect)
    {
        MK_DST[fc] = TRUE;                /* star verlengen gewenst als gevolg van detectie storingen */
    }

    if ((US_type[fc] & VTG_type) && DrukknopAanwezig && EenDrukknopDefect)
    {
        A_DST[fc] = TRUE;                 /* vaste aanvraag VTG conform instellingen gewenst */
    }

    if ((US_type[fc] == OV_type) && KopLusAanwezig && AlleKoplusDefect)
    {
        A_DST[fc] = TRUE;                /* vaste aanvraag OV conform instellingen gewenst */
        MK_DST[fc] = TRUE;                /* star verlengen gewenst als gevolg van detectie storingen */
    }

    if (US_type[fc] & MVT_type)           /* bepaal welke detector met hiaat voor detectie storing verlengen */
    {                                   /* ... dit geldt alleen voor MVT fasecycli */
        bool VerwegLusGevonden = FALSE;
        bool VerwegDefectGevonden = FALSE;
        do
        {
            fc_detector = va_arg(argpt2, va_mulv);
            if (fc_detector >= 0)
            {
                fc_hiaat_st = va_arg(argpt2, va_mulv);

                if ((IS_type[fc_detector] == DKOP_type) && (TDH_max[fc_detector] >= 0) && !DF[fc_detector])
                {
                    if (EersteLangeLusDefect && (fc_hiaat_st > NG))
                    {
                        if (D_onb[fc_detector] < T_max[fc_hiaat_st]) TDH[fc_detector] = TRUE;
                    }
                }

                if ((IS_type[fc_detector] == DVER_type) && (TDH_max[fc_detector] >= 0))
                {
                    if ((AantalVerwegLus > 1) && (!VerwegLusGevonden && AlleVerwegLusDefect ||
                        VerwegDefectGevonden && VerwegLusDefect))
                    {
                        if ((fc_hiaat_st > NG) && !DF[fc_detector])
                        {
                            if (D_onb[fc_detector] < T_max[fc_hiaat_st]) TDH[fc_detector] = TRUE;
                        }
                    }
                    VerwegLusGevonden = TRUE;
                    if (DF[fc_detector]) VerwegDefectGevonden = TRUE;
                }

                if (FC_DVM[fc])               /* verhoogde hiaattijden tijdens bevordering door DVM */
                {
#ifdef prmtdhdvmkop
                    if ((IS_type[fc_detector] == DKOP_type) && (TDH_max[fc_detector] >= 0) && !DF[fc_detector])
                    {
                        if (D_onb[fc_detector] < PRM[prmtdhdvmkop]) TDH[fc_detector] = TRUE;
                    }
#endif

#ifdef prmtdhdvmlang
                    if ((IS_type[fc_detector] == DLNG_type) && (TDH_max[fc_detector] >= 0) && !DF[fc_detector])
                    {
                        if (D_onb[fc_detector] < PRM[prmtdhdvmlang]) TDH[fc_detector] = TRUE;
                    }
#endif

#ifdef prmtdhdvmver
                    if ((IS_type[fc_detector] == DVER_type) && (TDH_max[fc_detector] >= 0) && !DF[fc_detector])
                    {
                        if (D_onb[fc_detector] < PRM[prmtdhdvmver]) TDH[fc_detector] = TRUE;
                    }
#endif
                }

                if (FC_FILE[fc])               /* verhoogde hiaattijden tijdens bevordering door FILE stroomopwaarts */
                {
#ifdef prmtdhfilekop
                    if ((IS_type[fc_detector] == DKOP_type) && (TDH_max[fc_detector] >= 0) && !DF[fc_detector])
                    {
                        if (D_onb[fc_detector] < PRM[prmtdhfilekop]) TDH[fc_detector] = TRUE;
                    }
#endif

#ifdef prmtdhfilelang
                    if ((IS_type[fc_detector] == DLNG_type) && (TDH_max[fc_detector] >= 0) && !DF[fc_detector])
                    {
                        if (D_onb[fc_detector] < PRM[prmtdhfilelang]) TDH[fc_detector] = TRUE;
                    }
#endif

#ifdef prmtdhfilever
                    if ((IS_type[fc_detector] == DVER_type) && (TDH_max[fc_detector] >= 0) && !DF[fc_detector])
                    {
                        if (D_onb[fc_detector] < PRM[prmtdhfilever]) TDH[fc_detector] = TRUE;
                    }
#endif
                }
            }
        } while (fc_detector >= 0);         /* ga door tot END gevonden in argumenten lijst */
    }
    va_end(argpt2);                     /* maak variable argumenten lijst leeg */
}


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
void traffick2tlcgen_detectie(void)   /* Fik230101                                                          */
{
    count i, fc;

    /* bijhouden duur intensiteitsmeting */
    if (TS && (sec_teller < 900)) sec_teller++;

    for (i = 0; i < DPMAX; ++i)
    {

#ifndef AUTOMAAT
        if (!D[i]) BG[i] = TBG_timer[i] = FALSE;
        else       OG[i] = TOG_timer[i] = FALSE;
#endif

        DF[i] = (CIF_IS[i] >= CIF_DET_STORING) || OG[i] || BG[i] || FL[i];

        if (DF[i])                        /* lus defect reset intensiteitsmeting */
        {
            Iactueel[i] = 0;
            kwartier[i] = 0;
        }
        else                              /* tel voertuigen per detector */
        {
            if (SD[i] && (Iactueel[i] < 10000)) Iactueel[i]++;
        }

        if ((sec_teller >= 900) || (Iactueel[i] > kwartier[i]))
        {
            kwartier[i] = Iactueel[i];      /* buffer kwartier intensiteit */
        }
        if (sec_teller >= 900) Iactueel[i] = 0;

        if (!D[i]) D_bez[i] = 0;
        else       D_bez[i] += TE;

        if (D[i]) D_onb[i] = 0;
        else       D_onb[i] += TE;

        if (D_bez[i] > 32000) D_bez[i] = 32000;
        if (D_onb[i] > 32000) D_onb[i] = 32000;

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
            D[i] = SD[i] = ED[i] = DB[i] = TDH[i] = FALSE;
        }
#endif
    }
    /* reset duur intenstiteitsmeting */
    if (sec_teller >= 900) sec_teller = 0;

    for (fc = 0; fc < FCMAX; ++fc)
    {
        HOT[fc] = FALSE;               /* reset startpuls roodlichtrijder                                    */
        A_DST[fc] = FALSE;               /* reset vaste aanvraag gewenst als gevolg van detectie storingen     */
        MK_DST[fc] = FALSE;               /* reset star verlengen gewenst als gevolg van detectie storingen     */
    }

    detectie_veld_afhandeling();        /* detectie afhandeling van de Traffick2TLCGen functionaliteiten      */
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
    if (CIF_IS[is] && (CIF_IS[is] != IS_old[is]) && !(CIF_IS[is] & BIT12) && !(CIF_IS[is] & BIT15))
    {
        if (CIF_IS[is] & BIT0) snelheid += 1;
        if (CIF_IS[is] & BIT1) snelheid += 2;
        if (CIF_IS[is] & BIT2) snelheid += 4;
        if (CIF_IS[is] & BIT3) snelheid += 8;
        if (CIF_IS[is] & BIT4) snelheid += 16;
        if (CIF_IS[is] & BIT5) snelheid += 32;
        if (CIF_IS[is] & BIT6) snelheid += 64;
        if (CIF_IS[is] & BIT7) snelheid += 128;

        return snelheid;
    }

    return NG;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal start puls lengte detectie                                                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt de lengte die aan de applicatie is doorgegeven op basis van lengte detectie.        */
/* De lengte wordt precies 1 applicatie ronde aangeboden voor gebruik in andere functies.                   */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PreApplication_Add().                                                   */
/*                                                                                                          */
mulv bepaal_start_puls_lengte(        /* Fik240101                                                          */
    count is)                             /* IS ingang signaal waarop lengte wordt afgebeeld                    */
{
    mulv lengte = 0;
    /* BIT14 = onbetrouwbare meting, BIT15 = voertuig in tegenrichting    */
    if (CIF_IS[is] && (CIF_IS[is] != IS_old[is]) && !(CIF_IS[is] & BIT14) && !(CIF_IS[is] & BIT15))
    {
        if (CIF_IS[is] & BIT0) lengte += 1;
        if (CIF_IS[is] & BIT1) lengte += 2;
        if (CIF_IS[is] & BIT2) lengte += 4;
        if (CIF_IS[is] & BIT3) lengte += 8;
        if (CIF_IS[is] & BIT4) lengte += 16;
        if (CIF_IS[is] & BIT5) lengte += 32;
        if (CIF_IS[is] & BIT6) lengte += 64;
        if (CIF_IS[is] & BIT7) lengte += 128;
        if (CIF_IS[is] & BIT8) lengte += 256;
        if (CIF_IS[is] & BIT9) lengte += 512;
        if (CIF_IS[is] & BIT10) lengte += 1024;
        if (CIF_IS[is] & BIT11) lengte += 2048;
        if (CIF_IS[is] & BIT12) lengte += 4096;

        return lengte;
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
    count prioVRW_index,                  /* count VRWFCfc - prioriteitsindex vrachtwagen ingreep               */
    count prioFTS_index)                  /* count ftsFCfc - prioriteitsindex fiets voorrang module             */
{
    bool interface_wijziging = FALSE;  /* bijhouden wijziging variabele op de interface                      */

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

    WGR[fc] = (bool)wg;
    if (!G[fc] && !wgav || tka(fc)) WGR[fc] = FALSE;

    MMK[fc] = (bool)!mgmk;            /* als SCH[schmv] == 0 dan dus alleen meeverlengen als MK[] waar is    */
    MGR[fc] = (bool)!mg;              /* TLCGen genereert een FALSE op deze positie, dus MGR[] = (bool)!mg  */
    ART[fc] = (bool)ar;
    ARB[fc] = (mulv)arb;

    AR_max[fc] = TGG_max[fc];
    if (TFG_max[fc] > AR_max[fc]) AR_max[fc] = TFG_max[fc];
    if (alt_max > AR_max[fc]) AR_max[fc] = alt_max;

    if (!G[fc])                         /* alt_ruimte mag wel hoger zijn dan alt_max maar nooit lager         */
    {
        if (alt_ruimte > AR_max[fc]) AR_max[fc] = alt_ruimte;
    }

    prio_index[fc].OV_kar = prioOV_index_kar;
    prio_index[fc].OV_srm = prioOV_index_srm;
    prio_index[fc].OV_verlos = prioOV_index_verlos;

    prio_index[fc].HD = prioHD_index;
    HD_aanwezig[fc] = HD_counter;
    prio_index[fc].VRW = prioVRW_index;
    prio_index[fc].FTS = prioFTS_index;

    /* GWT[fc] = TA_timer[fc];             voorbereid op gewogen wachttijd meting                             */
    RR[fc] &= ~BIT3;                    /* vroege reset is nodig ivm synchronisatie functies van TLCGen       */

    if (DOSEER[fc])
    {
        WGR[fc] = FALSE;                  /* geen wachtstand   als doseren aktief                               */
        MGR[fc] = FALSE;                  /* geen meeverlengen als doseren aktief                               */
        ART[fc] = FALSE;                  /* geen alternatieve realisatie als doseren aktief                    */
    }

    if (R[fc] && (Aled[fc] > 0))        /* altijd niet definitieve aanvraag als wtv gestart is                */
    {
        A[fc] |= BIT0;
    }

    if (interface_wijziging)            /* variabele(n) gewijzigd op de interface                             */
    {
        CIF_PARM1WIJZAP = -2;
    }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie fasecyclus instellingen Traffick2TLCGen - deel 2                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* In deze functie worden fasecyclus instellingen naar arrays gekopieerd, zodat deze instellingen eenvoudig */
/* benaderbaar zijn voor andere functies.                                                                   */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PreApplication_Add().                                                   */
/*                                                                                                          */
void traffick2tlcgen_instel_2(        /* Fik230301                                                          */
    count fc,                             /* FC    fasecyclus                                                   */
    mulv  KAR_id_OV,                      /* mulv  KAR id openbaar vervoer                                      */
    mulv  KAR_id_HD,                      /* mulv  KAR id nood- en hulpdiensten                                 */
    count usHD,                           /* US    verklik HD ingreep                                           */
    count usOV_kar,                       /* US    verklik OV ingreep - KAR                                     */
    count usOV_srm,                       /* US    verklik OV ingreep - SRM                                     */
    count usVRW,                          /* US    verklik VRW ingreep                                          */
    mulv  vooruit,                        /* mulv  aantal modulen dat vooruit gerealiseerd mag worden           */
    count altw)                           /* count ophoogfactor wachttijd voor alternatieve realisatie          */
{
    bool interface_wijziging = FALSE;  /* bijhouden wijziging variabele op de interface                      */

    if ((altw != NG) && (PRM[altw] > 9))
    {
        PRM[altw] = 9;
        interface_wijziging = TRUE;
    }

    if (KAR_id_OV > NG) prio_index[fc].KAR_id_OV = KAR_id_OV;
    if (KAR_id_HD > NG) prio_index[fc].KAR_id_HD = KAR_id_HD;
    if (usHD > NG) prio_index[fc].usHD = usHD;
    if (usOV_kar > NG) prio_index[fc].usOV_kar = usOV_kar;
    if (usOV_srm > NG) prio_index[fc].usOV_srm = usOV_srm;
    if (usVRW > NG) prio_index[fc].usVRW = usVRW;

    if (vooruit > NG) PFPRtraffick[fc] = vooruit;

    if (R[fc] && (A[fc] & BIT0))
    {
        if (TS || (GWT[fc] == 0))
        {
            GWT[fc]++;
            if (altw != NG) GWT[fc] += PRM[altw];
        }
    }
    else
    {
        GWT[fc] = 0;
    }

    if (interface_wijziging)            /* variabele(n) gewijzigd op de interface                             */
    {
        CIF_PARM1WIJZAP = -2;
    }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal actief DVM netwerk programma                                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt het actieve DVM programma op basis van de klok of de DVM parameter. De instelling   */
/* van de DVM parameter wordt bewaakt op een maximale duur instelbaar in uren.                              */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit KlokPerioden_Add().                                                     */
/*                                                                                                          */
void bepaal_DVM_programma(void)       /* Fik230101                                                          */
{
    count fc, ml;

#ifdef prmdvmpr                       /* is DVM parameter gedefinieerd ?                                    */
    if ((PRM[prmdvmpr] != DVM_parm) || (PRM[prmdvmpr] == 0))
    {
        DVM_prog_duur = 0;
        DVM_parm = PRM[prmdvmpr];
    }
    else
    {
        if (TS && (CIF_KLOK[CIF_SECONDE] == 0) && (CIF_KLOK[CIF_MINUUT] == 0) && (DVM_prog_duur < 30500)) DVM_prog_duur++;

#ifdef prmdvmmax                      /* is bewaking op maximale duur netwerk programma aanwezig ?          */
        if ((PRM[prmdvmmax] > 0) && ((PRM[prmdvmmax] <= DVM_prog_duur) || (DVM_prog_duur >= 30500)))
        {
            DVM_parm = 0;
            DVM_prog_duur = 0;
            PRM[prmdvmpr] = 0;           /* reset DVM parameter                                                */
            CIF_PARM1WIJZAP = -2;           /* variabele(n) gewijzigd op de interface                             */
        }
#endif
    }

    if (DVM_parm > aantal_dvm_prg)      /* foutieve instelling PRM[prmdvmpr]                                  */
    {
        DVM_parm = 0;
        PRM[prmdvmpr] = 0;             /* reset DVM parameter                                                */
        CIF_PARM1WIJZAP = -2;             /* variabele(n) gewijzigd op de interface                             */
    }
#endif

    if (DVM_klok > aantal_dvm_prg)      /* foutieve waarde toegekend aan DVM_klok                             */
    {
        DVM_klok = 0;
    }

    DVM_prog = DVM_klok;                /* bepaal actief DVM programma - parameter gaat voor de klok          */
    if (DVM_parm > 0) DVM_prog = DVM_parm;

#ifdef hrgvact                        /* is RobuGrover aanwezig ?                                           */
    if (DVM_prog > 0) IH[hrgvact] = FALSE;
#endif

    ML_DVM_MAX = NG;            /* bepaal aantal modulen in regeling bij inzet DVM maatregelen        */

    for (ml = 0; ml < MLMAX; ++ml)
    {
        for (fc = 0; fc < FCMAX; ++fc)
        {
            if (PRML_DVM[ml][fc] == PRIMAIR)
            {
                ML_DVM_MAX = ml + 1;
            }
        }
    }
    /* controleer of er wel een module indeling voor DVM aanwezig is      */
    if (ML_DVM_MAX == NG) DVM_structuur_gewenst = FALSE;

    if (DVM_structuur_gewenst != DVM_structuur_actief)
    {
        if (!hlpd_aanwezig() && (ML == ML1))
        {
            DVM_structuur_actief = DVM_structuur_gewenst;

            for (ml = ML1; ml < MLMAX; ++ml)
            {
                for (fc = 0; fc < FCMAX; ++fc)
                {
                    AR[fc] = AG[fc] = PR[fc] = PG[fc] = FALSE;
                    if (DVM_structuur_actief && (PRML_DVM[ml][fc] == PRIMAIR) ||
                        !DVM_structuur_actief && (PRML_REG[ml][fc] == PRIMAIR))
                    {
                        PRML[ml][fc] = PRIMAIR;
                        if (!RV[fc]) PR[fc] = TRUE;
                        if (G[fc] || GL[fc]) PG[fc] = TRUE;
                    }
                    else
                    {
                        PRML[ml][fc] = FALSE;
                        if (!RV[fc]) AR[fc] = TRUE;
                        if (G[fc] || GL[fc]) AG[fc] = TRUE;
                    }
                }
            }
        }
    }

}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie reset wachtstand aanvraag indien niet gewenst                                                    */
/* -------------------------------------------------------------------------------------------------------- */
/* Binnen TLCGen bestaat voor wachtstand groen richtingen niet de mogelijkheid om enkel de wachtstand groen */
/* aanvraag uit te schakelen. Deze functie reset alle wachtstand groen aanvragen.                           */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Aanvragen_Add().                                                        */
/*                                                                                                          */
void reset_wachtstand_aanvraag(void)  /* Fik230101                                                          */
{
    count fc;

    for (fc = 0; fc < FCMAX; ++fc)
    {
        if (!G[fc])
        {
            A[fc] &= ~BIT2;
            WGR[fc] = FALSE;
        }
    }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie corrigeer wachtstand aanvraag bij harde koppelingen                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* TLCGen zet de wachtstand aanvraag zodra alle ontruimingstijden vanaf conflictrichtingen zijn verstreken. */
/* Bij korte ontruimingstijden kan dit moment al aanbreken voordat een conflicterende richting de "kans"    */
/* heeft gehad om opnieuw aan te vragen. (TRG[] nog niet verstreken)                                        */
/*                                                                                                          */
/* Voor gekoppelde richtingen geldt verder dat ook alle ontruimingstijden naar de volgrichting moeten zijn  */
/* verstreken.                                                                                              */
/*                                                                                                          */
/* Deze functie reset de wachtstand aanvraag als nodig.                                                     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Aanvragen_Add().                                                        */
/*                                                                                                          */
void hki_wachtstand_aanvraag(void)    /* Fik240101                                                          */
{
    count i, k;

    for (i = 0; i < FCMAX; ++i)
    {
        if (WGR[i] && R[i] && (A[i] & BIT2))
        {
            for (k = 0; k < FCMAX; ++k)
            {
                if (FK_conflict(i, k))
                {
                    if (GL[k] || TRG[k] || A[k]) A[i] &= ~BIT2;
                }
            }
        }
    }

    for (i = 0; i < aantal_hki_kop; ++i)
    {
        if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
        {
            count fc1 = hki_kop[i].fc1;     /* voedende richting */
            count fc2 = hki_kop[i].fc2;     /* volg     richting */

            if (RV[fc1] && !G[fc2] && (A[fc1] & BIT2) && !(A[fc2] & BIT2))
            {
                aanvraag_wachtstand_exp(fc2, TRUE);
                if (R[fc2] && (A[fc2] & BIT2))
                {
                    for (k = 0; k < FCMAX; ++k)
                    {
                        if (FK_conflict(fc2, k))
                        {
                            if (GL[k] || TRG[k] || A[k]) A[fc2] &= ~BIT2;
                        }
                    }
                }
                if (!RA[fc2] && !G[fc2] && !(A[fc2] & BIT2)) A[fc1] &= ~BIT2;
                A[fc2] &= ~BIT2;
            }
        }
    }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal aanvragen harde koppeling                                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt bij harde koppelingen het opzetten van de meeaanvraag van de volgrichtingen.       */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Aanvragen_Add().                                                        */
/*                                                                                                          */
void koppel_aanvragen(void)           /* Fik230701                                                          */
{
    count i;

    for (i = 0; i < aantal_hki_kop; ++i)
    {
        if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
        {
            count fc1 = hki_kop[i].fc1;     /* voedende richting */
            count fc2 = hki_kop[i].fc2;     /* volg     richting */
            bool los_fc2 = hki_kop[i].los_fc2; /* fc2 mag bij aanvraag fc1 los realiseren */
            count status = hki_kop[i].status;  /* status koppeling fc1 -> fc2 */

            if ((SG[fc1] || G[fc1] && (status == 1)) && R[fc2]) A[fc2] |= BIT4;
            if (!los_fc2)
            {
                if ((US_type[fc1] == VTG_type) && (US_type[fc2] == VTG_type))
                {
                    if (R[fc1] && A[fc1] && R[fc2]) A[fc2] |= BIT4;
                }
            }
        }
    }

    for (i = 0; i < aantal_vtg_tgo; ++i)
    {
        count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
        count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
        count hnla12 = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
        count hnla21 = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
        count status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
        count status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

        if (((!G[fc1] && A[fc1] || SG[fc1]) && IH[hnla12] || G[fc1] && (status12 == 1)) && R[fc2]) A[fc2] |= BIT4;

        /* bugfix Fik230701 - koppelaanvraag bij gescheiden oversteek is in twee richtingen */
        if (((!G[fc2] && A[fc2] || SG[fc2]) && IH[hnla21] || G[fc2] && (status21 == 1)) && R[fc1]) A[fc1] |= BIT4;
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
        count fc = fts_pri[i].fc;       /* fietsrichting */
        count drk1 = fts_pri[i].drk1;     /* drukknop 1 voor aanvraag prioriteit */
        count drk2 = fts_pri[i].drk2;     /* drukknop 2 voor aanvraag prioriteit */
        count de1 = fts_pri[i].de1;      /* koplus   1 voor aanvraag prioriteit */
        count de2 = fts_pri[i].de2;      /* koplus   2 voor aanvraag prioriteit */
        count ogwt_fts = fts_pri[i].ogwt_fts; /* ondergrens wachttijd voor prioriteit */
        count prio_fts = fts_pri[i].prio_fts; /* prioriteitscode */
        count ogwt_reg = fts_pri[i].ogwt_reg; /* ondergrens wachttijd voor prioriteit */
        count prio_reg = fts_pri[i].prio_reg; /* prioriteitscode */
        bool aanvraag = fts_pri[i].aanvraag; /* fietser is op juiste wijze aangevraagd */
        bool prio_vw = fts_pri[i].prio_vw;  /* fietser voldoet aan prioriteitsvoorwaarden */

        if (R[fc] && (A[fc] & BIT0))      /* detectie aanvraag aanwezig */
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
            prio_vw = FALSE;
        }

        if (BL[fc] || (WT_TE_HOOG || GEEN_FIETS_PRIO && (US_type[fc] == FTS_type)) && RV[fc] && (Aled[fc] == 0)) prio_vw = FALSE;

        fts_pri[i].aanvraag = aanvraag; /* bijwerken struct */
        fts_pri[i].prio_vw = prio_vw;
    }
}


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
void bijwerken_peloton_ingreep(void)  /* Fik230101                                                          */
{
    count i;

    for (i = 0; i < aantal_pel_kop; ++i)
    {
        count kop_fc = pel_kop[i].kop_fc;      /* koppelrichting */
        count kop_toe = pel_kop[i].kop_toe;     /* toestemming peloton ingreep */
        count kop_sig = pel_kop[i].kop_sig;     /* koppelsignaal */
        count kop_bew = pel_kop[i].kop_bew;     /* bewaak koppelsignaal (bij NG wordt een puls veronderstelt) */
        count vast_vert = pel_kop[i].vast_vert;   /* vasthoud vertraging  (start op binnenkomst koppelsignaal)  */
        count duur_vast = pel_kop[i].duur_vast;   /* duur vasthouden (bij duursign. na afvallen koppelsignaal)  */
        count duur_verl = pel_kop[i].duur_verl;   /* duur verlengen na ingreep (bij NUL geldt TVG_max[])  */
        count hnaloop_1 = pel_kop[i].hnaloop_1;   /* voorwaarde herstart extra nalooptijd 1 (nalooplus 1) */
        count tnaloop_1 = pel_kop[i].tnaloop_1;   /* nalooptijd 1 */
        count hnaloop_2 = pel_kop[i].hnaloop_2;   /* voorwaarde herstart extra nalooptijd 2 (nalooplus 2) */
        count tnaloop_2 = pel_kop[i].tnaloop_2;   /* nalooptijd 2 */
        bool kop_oud = pel_kop[i].kop_oud;     /* status koppelsignaal vorige machine slag */
        mulv  aanw_kop1 = pel_kop[i].aanw_kop1;   /* aanwezigheidsduur koppelsignaal 1 vanaf start puls */
        mulv  duur_kop1 = pel_kop[i].duur_kop1;   /* tijdsduur HOOG    koppelsignaal 1 igv duur signaal */
        mulv  aanw_kop2 = pel_kop[i].aanw_kop2;   /* aanwezigheidsduur koppelsignaal 2 vanaf start puls */
        mulv  duur_kop2 = pel_kop[i].duur_kop2;   /* tijdsduur HOOG    koppelsignaal 2 igv duur signaal */
        mulv  aanw_kop3 = pel_kop[i].aanw_kop3;   /* aanwezigheidsduur koppelsignaal 3 vanaf start puls */
        mulv  duur_kop3 = pel_kop[i].duur_kop3;   /* tijdsduur HOOG    koppelsignaal 3 igv duur signaal */
        mulv  pk_status = pel_kop[i].pk_status;   /* status peloton ingreep */
        bool pk_afronden = pel_kop[i].pk_afronden; /* afronden lopende peloton ingreep */
        bool buffervol = pel_kop[i].buffervol;   /* buffers voor peloton ingreep vol */

        bool puls = (kop_bew == NG);     /* bepaal of sprake is van een puls of duursignaal */
        bool doorschuiven = FALSE;       /* doorschuiven koppel buffers */

        if (TE)                           /* bijwerken duur aanwezigheid koppelsignalen */
        {
            if (aanw_kop1 > NG) aanw_kop1 += TE;
            if (aanw_kop2 > NG) aanw_kop2 += TE;
            if (aanw_kop3 > NG) aanw_kop3 += TE;

            if (!puls && IH[kop_sig] && kop_oud)
            {
                if ((duur_kop1 > NG) && (aanw_kop2 == NG)) duur_kop1 += TE;
                if ((duur_kop2 > NG) && (aanw_kop3 == NG)) duur_kop2 += TE;
                if ((duur_kop3 > NG) && !buffervol) duur_kop3 += TE;
            }
            buffervol |= !puls && (duur_kop3 > NG) && !IH[kop_sig];

            if (aanw_kop1 > 32000) aanw_kop1 = 32000;
            if (aanw_kop2 > 32000) aanw_kop2 = 32000;
            if (aanw_kop3 > 32000) aanw_kop3 = 32000;

            if (!puls && (duur_kop1 > T_max[kop_bew])) duur_kop1 = T_max[kop_bew];
            if (!puls && (duur_kop2 > T_max[kop_bew])) duur_kop2 = T_max[kop_bew];
            if (!puls && (duur_kop3 > T_max[kop_bew])) duur_kop3 = T_max[kop_bew];
        }

        if (aanw_kop1 > duur_kop1 + T_max[vast_vert] + T_max[duur_vast]) doorschuiven = TRUE;
        if (!puls && (aanw_kop1 > T_max[vast_vert]))
        {
            if (aanw_kop1 > duur_kop1 + T_max[duur_vast]) doorschuiven = TRUE;
        }
        if (!puls && (duur_kop1 < T_max[vast_vert]) && (aanw_kop2 > NG || !IH[kop_sig]))
        {
            if (aanw_kop1 > (T_max[vast_vert] - duur_kop1) + T_max[duur_vast]) doorschuiven = TRUE;
        }

        if (doorschuiven)                 /* doorschuiven koppel buffers */
        {
            aanw_kop1 = aanw_kop2;
            aanw_kop2 = aanw_kop3;
            aanw_kop3 = NG;

            duur_kop1 = duur_kop2;
            duur_kop2 = duur_kop3;
            duur_kop3 = NG;

            buffervol = FALSE;
        }

        if (puls && SH[kop_sig] || !puls && IH[kop_sig] && !kop_oud) /* nieuwe koppelpuls ontvangen */
        {
            if (aanw_kop1 == NG) aanw_kop1 = duur_kop1 = 0;
            else if (aanw_kop2 == NG) aanw_kop2 = duur_kop2 = 0;
            else if (aanw_kop3 == NG) aanw_kop3 = duur_kop3 = 0;
        }

        if ((PEL_WT_TE_HOOG || !MM[kop_toe]) && (!G[kop_fc] || doorschuiven || (pk_status != 3) || pk_afronden) || MG[kop_fc] && fkrap(kop_fc))
        {
            MM[kop_toe] = FALSE;            /* geen nieuwe ingreep meer toestaan */
            pk_afronden = TRUE;             /* lopende koppeling volledig afronden */
        }
        else pk_afronden = FALSE;

        if (pk_status > NG)               /* bijwerken status peloton ingreep */
        {
            if (R[kop_fc] && (aanw_kop1 > 0) && (pk_status == 0)) pk_status = 1;
            if (G[kop_fc] && !(RW[kop_fc] & BIT12) && !(YV[kop_fc] & BIT12) && (pk_status >= 3)) pk_status = 0;
        }
        if (pk_status < 3) pk_afronden = FALSE;

        /* groen door peloton ingreep, dus groen ook altijd aanhouden */
        if (SG[kop_fc] && (A[kop_fc] == BIT12)) pk_status = 3;

        if (pk_status >= 3)
        {
            if ((pk_status == 3) && (!doorschuiven || MM[kop_toe] && (aanw_kop1 > NG)))
            {
                mulv max_duur = 0;
                if (puls)
                {
                    max_duur = T_max[vast_vert] + T_max[duur_vast] - aanw_kop1;
                }
                else /* koppelsignaal is duur signaal */
                {
                    if (aanw_kop1 == duur_kop1)
                    {
                        max_duur = T_max[kop_bew] - duur_kop1 + T_max[duur_vast];
                    }
                    if (aanw_kop1 > duur_kop1)
                    {
                        if (duur_kop1 < T_max[vast_vert])
                        {
                            max_duur = T_max[vast_vert] + T_max[duur_vast] - aanw_kop1;
                        }
                        else
                        {
                            max_duur = duur_kop1 + T_max[duur_vast] - aanw_kop1;
                        }
                    }
                }
                if (max_duur < 0) max_duur = 0;

                if ((hnaloop_1 > NG) && (tnaloop_1 > NG))
                {
                    max_duur += T_max[tnaloop_1];
                    if ((hnaloop_2 > NG) && (tnaloop_2 > NG))
                    {
                        max_duur += T_max[tnaloop_2];
                    }
                }

                if (duur_verl != NG)
                {
                    if (T_max[duur_verl] == 0) max_duur += TVG_max[kop_fc];
                    else                       max_duur += T_max[duur_verl];
                }

                PELTEG[kop_fc] = max_duur + TE;

                if (PELTEG[kop_fc] < TFG_max[kop_fc] + TVG_max[kop_fc] - TG_timer[kop_fc])
                {
                    PELTEG[kop_fc] = TFG_max[kop_fc] + TVG_max[kop_fc] - TG_timer[kop_fc];
                }
            }

            if (pk_status == 4) /* vasthouden door 1e nalooplus */
            {
                mulv max_duur = 0;

                if ((hnaloop_1 > NG) && (tnaloop_1 > NG))
                {
                    if (T[tnaloop_1]) max_duur += T_max[tnaloop_1] - T_timer[tnaloop_1];
                    if ((hnaloop_2 > NG) && (tnaloop_2 > NG))
                    {
                        max_duur += T_max[tnaloop_2];
                    }
                }

                if (duur_verl != NG)
                {
                    if (T_max[duur_verl] == 0) max_duur += TVG_max[kop_fc];
                    else                       max_duur += T_max[duur_verl];
                }

                PELTEG[kop_fc] = max_duur + TE;

                if (PELTEG[kop_fc] < TFG_max[kop_fc] + TVG_max[kop_fc] - TG_timer[kop_fc])
                {
                    PELTEG[kop_fc] = TFG_max[kop_fc] + TVG_max[kop_fc] - TG_timer[kop_fc];
                }
            }

            if (pk_status == 5) /* vasthouden door 2e nalooplus */
            {
                mulv max_duur = 0;

                if ((hnaloop_2 > NG) && (tnaloop_2 > NG))
                {
                    if (T[tnaloop_2]) max_duur += T_max[tnaloop_2] - T_timer[tnaloop_2];
                }

                if (duur_verl != NG)
                {
                    if (T_max[duur_verl] == 0) max_duur += TVG_max[kop_fc];
                    else                       max_duur += T_max[duur_verl];
                }

                PELTEG[kop_fc] = max_duur + TE;

                if (PELTEG[kop_fc] < TFG_max[kop_fc] + TVG_max[kop_fc] - TG_timer[kop_fc])
                {
                    PELTEG[kop_fc] = TFG_max[kop_fc] + TVG_max[kop_fc] - TG_timer[kop_fc];
                }
            }

            if (pk_status == 6) /* aftellen PELTEG[] tot 0 */
            {
                if (!SVG[kop_fc])
                {
                    if (PELTEG[kop_fc] > TE) PELTEG[kop_fc] -= TE;
                    else                     PELTEG[kop_fc] = 0;
                }
            }
        }
        else                              /* pk_status niet >= 3, zet PELTEG[] terug op NG */
        {
            PELTEG[kop_fc] = NG;
        }
        /* bijwerken struct */
        pel_kop[i].kop_oud = IH[kop_sig];
        pel_kop[i].aanw_kop1 = aanw_kop1;
        pel_kop[i].duur_kop1 = duur_kop1;
        pel_kop[i].aanw_kop2 = aanw_kop2;
        pel_kop[i].duur_kop2 = duur_kop2;
        pel_kop[i].aanw_kop3 = aanw_kop3;
        pel_kop[i].duur_kop3 = duur_kop3;
        pel_kop[i].pk_status = pk_status;
        pel_kop[i].pk_afronden = pk_afronden;
        pel_kop[i].buffervol = buffervol;
    }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Peloton ingreep - aanvragen                                                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie zet de aanvraag tbv de peloton ingreep.                                                     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Aanvragen_Add().                                                        */
/*                                                                                                          */
void peloton_ingreep_aanvraag(void)   /* Fik230101                                                          */
{
    count i;

    bijwerken_peloton_ingreep();

    for (i = 0; i < aantal_pel_kop; ++i)
    {
        count kop_fc = pel_kop[i].kop_fc;    /* koppelrichting */
        count kop_toe = pel_kop[i].kop_toe;   /* toestemming peloton ingreep */
        count aanv_vert = pel_kop[i].aanv_vert; /* aanvraag vertraging (bij NG wordt geen aanvraag opgzet) */
        mulv  aanw_kop1 = pel_kop[i].aanw_kop1; /* aanwezigheidsduur koppelsignaal 1 vanaf start puls */
        mulv  pk_status = pel_kop[i].pk_status; /* status peloton ingreep */

        A[kop_fc] &= ~BIT12;
        if ((pk_status > NG) && (aanv_vert > NG) && MM[kop_toe] && (aanw_kop1 > T_max[aanv_vert]))
        {
            if (R[kop_fc] && !TRG[kop_fc] && !(RR[kop_fc] & BIT6) && !DOSEER[kop_fc] && !BL[kop_fc])
            {
                A[kop_fc] |= BIT12;
                pk_status = 2;
            }
        }
        pel_kop[i].pk_status = pk_status;
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
    count i, j, k;
    bool NEST = TRUE;

    for (i = 0; i < FCMAX; ++i)         /* initialisatie TMPc matrix */
    {
        for (j = 0; j < FCMAX; j++)
        {
            TMPc[i][j] = NG;

            if (!G[i] && !G[j])             /* bijwerken fictieve ontruimingstijden */
            {
                if (TMPi[i][j] > TE) TMPi[i][j] -= TE;
                else                 TMPi[i][j] = 0;
            }
        }
    }

    while (NEST)                        /* het vaststellen van tijdelijke conflicten is een iteratief proces  */
    {
        NEST = FALSE;
        for (i = 0; i < aantal_dcf_vst; ++i)
        {
            count fc1 = dcf_vst[i].fc1;    /* richting die voorstart geeft  */
            count fc2 = dcf_vst[i].fc2;    /* richting die voorstart krijgt */
            count to12 = dcf_vst[i].to12;   /* ontruimingstijd van fc1 naar fc2 */
            count ma21 = dcf_vst[i].ma21;   /* meerealisatie van fc2 met fc1 */

            bool meeaanv2 = FALSE;
            if ((ma21 != NG) && SCH[ma21]) meeaanv2 = TRUE;

            if (!G[fc1] && !G[fc2] && (GL[fc2] || TRG[fc2] || A[fc2] || meeaanv2))
            {
                for (k = 0; k < FCMAX; ++k)
                {
                    if ((GK_conflict(fc2, k) > NG) && (GK_conflict(fc1, k) == NG))
                    {
                        TMPc[fc1][k] = TMPc[k][fc1] = NEST = TRUE;

                        if (volgrichting_hki(fc1)) /* fc1 is een volgrichting, ga op zoek naar de voedende richting(en) */
                        {
                            for (j = 0; j < aantal_hki_kop; ++j)
                            {
                                if (hki_kop[j].status > NG) /* koppeling is ingeschakeld */
                                {
                                    count fc_voedend = hki_kop[j].fc1;
                                    count fc_volg = hki_kop[j].fc2;

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
                TMPi[fc1][fc2] = TGL_max[fc1] + T_max[to12];

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
                                count fc_volg = hki_kop[j].fc2;

                                if (fc1 == fc_volg)   /* koppeling gevonden, maak voedende richting conflicterend met fc2   */
                                {
                                    TMPc[fc2][fc_voedend] = GK;
                                    TMPc[fc_voedend][fc2] = GKL;
                                }
                            }
                        }
                    }

                    /* Fik240626 - bugfix */
                    if (volgrichting_hki(fc2))  /* fc2 is een volgrichting, ga op zoek naar de voedende richting(en)  */
                    {
                        for (j = 0; j < aantal_hki_kop; ++j)
                        {
                            if (hki_kop[j].status > NG) /* koppeling is ingeschakeld */
                            {
                                count fc_voedend = hki_kop[j].fc1;
                                count fc_volg = hki_kop[j].fc2;

                                if (fc2 == fc_volg)   /* koppeling gevonden, maak voedende richting conflicterend met fc1   */
                                {
                                    TMPc[fc1][fc_voedend] = GK;
                                    TMPc[fc_voedend][fc1] = GKL;
                                }
                            }
                        }
                    }

                }
            }
        }

        for (i = 0; i < aantal_dcf_gst; ++i)
        {
            count fc1 = dcf_gst[i].fc1;    /* richting 1 */
            count fc2 = dcf_gst[i].fc2;    /* richting 2 */
            count to12 = dcf_gst[i].to12;   /* ontruimingstijd van fc1 naar fc2 */
            count to21 = dcf_gst[i].to21;   /* ontruimingstijd van fc2 naar fc1 */
            count ma12 = dcf_gst[i].ma12;   /* meerealisatie van fc1 met fc2 */
            count ma21 = dcf_gst[i].ma21;   /* meerealisatie van fc2 met fc1 */

            bool meeaanv1 = FALSE;
            bool meeaanv2 = FALSE;

            if ((ma12 != NG) && SCH[ma12]) meeaanv1 = TRUE;
            if ((ma21 != NG) && SCH[ma21]) meeaanv2 = TRUE;

            if (!G[fc1] && (GL[fc1] || TRG[fc1] || A[fc1] || meeaanv1) && !G[fc2] && (GL[fc2] || TRG[fc2] || A[fc2] || meeaanv2))
            {
                for (k = 0; k < FCMAX; ++k)
                {
                    if ((GK_conflict(fc1, k) > NG) && (GK_conflict(fc2, k) == NG))
                    {
                        TMPc[fc2][k] = TMPc[k][fc2] = NEST = TRUE;

                        if (volgrichting_hki(fc2)) /* fc2 is een volgrichting, ga op zoek naar de voedende richting(en) */
                        {
                            for (j = 0; j < aantal_hki_kop; ++j)
                            {
                                if (hki_kop[j].status > NG) /* koppeling is ingeschakeld */
                                {
                                    count fc_voedend = hki_kop[j].fc1;
                                    count fc_volg = hki_kop[j].fc2;

                                    if (fc2 == fc_volg) /* koppeling gevonden, maak voedende richting ook conflicterend met k */
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

                    if ((GK_conflict(fc2, k) > NG) && (GK_conflict(fc1, k) == NG))
                    {
                        TMPc[fc1][k] = TMPc[k][fc1] = NEST = TRUE;

                        if (volgrichting_hki(fc1)) /* fc1 is een volgrichting, ga op zoek naar de voedende richting(en) */
                        {
                            for (j = 0; j < aantal_hki_kop; ++j)
                            {
                                if (hki_kop[j].status > NG) /* koppeling is ingeschakeld */
                                {
                                    count fc_voedend = hki_kop[j].fc1;
                                    count fc_volg = hki_kop[j].fc2;

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
                TMPi[fc1][fc2] = TGL_max[fc1] + T_max[to12];

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
                                count fc_volg = hki_kop[j].fc2;

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

            if (G[fc2] && !G[fc1])
            {
                TMPi[fc2][fc1] = TGL_max[fc2] + T_max[to21];

                if (TMPc[fc2][fc1] == NG)
                {
                    TMPc[fc2][fc1] = TMPc[fc1][fc2] = NEST = TRUE;

                    if (volgrichting_hki(fc2))  /* fc2 is een volgrichting, ga op zoek naar de voedende richting(en)  */
                    {
                        for (j = 0; j < aantal_hki_kop; ++j)
                        {
                            if (hki_kop[j].status > NG) /* koppeling is ingeschakeld */
                            {
                                count fc_voedend = hki_kop[j].fc1;
                                count fc_volg = hki_kop[j].fc2;

                                if (fc2 == fc_volg)   /* koppeling gevonden, maak voedende richting conflicterend met fc1   */
                                {
                                    TMPc[fc1][fc_voedend] = GK;
                                    TMPc[fc_voedend][fc1] = GKL;
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
    count i, j;

    for (i = 0; i < FCMAX; ++i)
    {
        P[i] &= ~BIT2;                   /* reset P en FM bit naloop harde koppeling */
        FM[i] &= ~BIT2;
        NAL[i] = FALSE;                   /* reset NAL[] -> WG[] of VG[] agv harde koppeling */
    }

    for (i = 0; i < aantal_hki_kop; ++i)
    {
        if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
        {
            count fc1 = hki_kop[i].fc1;      /* voedende richting */
            count fc2 = hki_kop[i].fc2;      /* volg     richting */
            count tnlfg12 = hki_kop[i].tnlfg12;  /* vaste   nalooptijd vanaf einde vastgroen      fc1 */
            count tnlfgd12 = hki_kop[i].tnlfgd12; /* det.afh.nalooptijd vanaf einde vastgroen      fc1 */
            count tnleg12 = hki_kop[i].tnleg12;  /* vaste   nalooptijd vanaf einde (verleng)groen fc1 */
            count tnlegd12 = hki_kop[i].tnlegd12; /* det.afh.nalooptijd vanaf einde (verleng)groen fc1 */
            bool kop_eg = hki_kop[i].kop_eg;   /* koppeling vanaf einde groen (als FALSE dan vanaf EV) */
            mulv  kop_max = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */

            bool HGfc1 = G[fc1] && (!MG[fc1] || kop_eg);

            if (HGfc1 && !G[fc2]) hki_kop[i].status = 1;
            if (HGfc1 && G[fc2]) hki_kop[i].status = 2;
            if (!HGfc1 && G[fc2] && ((hki_kop[i].status == 1) || (hki_kop[i].status == 2))) hki_kop[i].status = 3;

            /* default is de actuele TVG_max[] van de volgrichting */
            if (kop_max == NG) kop_max = TVG_max[fc2];
            hki_kop[i].kop_max = kop_max;
            /* garandeer volgrichting */
            if ((hki_kop[i].status == 1) || RA[fc1] && P[fc1]) P[fc2] |= BIT2;

            if (!HGfc1 && (hki_kop[i].status == 3))
            {
                hki_kop[i].status = 0;
                if (SVG[fc2]) hki_kop[i].status = 3;
                if (tnlfg12 != NG) { if (T[tnlfg12] || ET[tnlfg12]) hki_kop[i].status = 3; }
                if (tnlfgd12 != NG) { if (T[tnlfgd12] || ET[tnlfgd12]) hki_kop[i].status = 3; }
                if (tnleg12 != NG) { if (T[tnleg12] || ET[tnleg12]) hki_kop[i].status = 3; }
                if (tnlegd12 != NG) { if (T[tnlegd12] || ET[tnlegd12]) hki_kop[i].status = 3; }
                if (tnlegd12 != NG) { if ((GL[fc1] || EGL[fc1]) && kop_eg) hki_kop[i].status = 3; }
            }

            if (SVG[fc2] && (hki_kop[i].status == 3)) hki_kop[i].status = 4;
            if (!VG[fc2] && (hki_kop[i].status == 4)) hki_kop[i].status = 0;
            if (VG[fc2] && (hki_kop[i].status == 4))
            {
                if (TVG_timer[fc2] >= kop_max)
                {
#ifdef PRIO_ADDFILE
                    if (!(YV[fc2] & PRIO_YV_BIT)) FM[fc2] |= BIT2;
#else
                    FM[fc2] |= BIT2;
#endif
                    hki_kop[i].status = 0;
                }
            }

            if (hki_kop[i].status >= 3)     /* controleer of harde koppeling op volgrichting is overgenomen door een andere voedende richting */
            {
                if (WG[fc2] || VG[fc2]) NAL[fc2] = TRUE;

                for (j = 0; j < aantal_hki_kop; ++j)
                {
                    /* alleen actieve harde koppeling kan andere harde koppeling overnemen */
                    if (hki_kop[j].status == NG) continue;

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
        count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
        count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
        count tnlsgd12 = vtg_tgo[i].tnlsgd12; /* nalooptijd fc2 vanaf startgroen fc1 */
        count tnlsgd21 = vtg_tgo[i].tnlsgd21; /* nalooptijd fc1 vanaf startgroen fc2 */
        count hnla12 = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
        count hnla21 = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */

        if (G[fc1] && !G[fc2] && IH[hnla12]) vtg_tgo[i].status12 = 1;
        if (G[fc1] && G[fc2] && (IH[hnla12] || (vtg_tgo[i].status12 == 1))) vtg_tgo[i].status12 = 2;
        if (!G[fc1] && G[fc2] && ((vtg_tgo[i].status12 == 1) || (vtg_tgo[i].status12 == 2))) vtg_tgo[i].status12 = 3;

        if (!G[fc1] && (vtg_tgo[i].status12 == 3))
        {
            vtg_tgo[i].status12 = 0;
            if (tnlsgd12 != NG) { if (T[tnlsgd12] || ET[tnlsgd12]) vtg_tgo[i].status12 = 3; }
        }

        if (G[fc2] && !G[fc1] && IH[hnla21]) vtg_tgo[i].status21 = 1;
        if (G[fc2] && G[fc1] && (IH[hnla21] || (vtg_tgo[i].status21 == 1))) vtg_tgo[i].status21 = 2;
        if (!G[fc2] && G[fc1] && ((vtg_tgo[i].status21 == 1) || (vtg_tgo[i].status21 == 2))) vtg_tgo[i].status21 = 3;

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
    count i, prio, inm;
    bool foutieve_index = FALSE;

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
    bool foutieve_index = FALSE;

    if ((volgnr != NG) && ((volgnr < 0) || (volgnr >= aantal_hki_kop))) foutieve_index = TRUE;

    if (!foutieve_index)
    {
        for (i = 0; i < aantal_hki_kop; ++i)
        {
            if (volgnr != NG) i = volgnr;

            if (hki_kop[i].status > NG)     /* koppeling is ingeschakeld */
            {
                count fc1 = hki_kop[i].fc1;      /* voedende richting */
                count fc2 = hki_kop[i].fc2;      /* volg     richting */
                count tnlfg12 = hki_kop[i].tnlfg12;  /* vaste   nalooptijd vanaf einde vastgroen      fc1 */
                count tnlfgd12 = hki_kop[i].tnlfgd12; /* det.afh.nalooptijd vanaf einde vastgroen      fc1 */
                count tnleg12 = hki_kop[i].tnleg12;  /* vaste   nalooptijd vanaf einde (verleng)groen fc1 */
                count tnlegd12 = hki_kop[i].tnlegd12; /* det.afh.nalooptijd vanaf einde (verleng)groen fc1 */
                bool kop_eg = hki_kop[i].kop_eg;   /* koppeling vanaf einde groen (als FALSE dan vanaf EV) */
                mulv  kop_max = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */
                mulv  status = hki_kop[i].status;   /* status koppeling */

                mulv  naloop_tijd = 0;

                if (G[fc2] && (status == 2))
                {
                    naloop_tijd = 0;
                    if (VS[fc1] || FG[fc1])
                    {
                        if ((tnlfg12 != NG)) naloop_tijd = T_max[tnlfg12];
                        if ((tnlfgd12 != NG) && (naloop_tijd < T_max[tnlfgd12])) naloop_tijd = T_max[tnlfgd12];
                    }
                    else
                    {
                        if ((tnlfg12 != NG)) naloop_tijd = T_max[tnlfg12] - T_timer[tnlfg12];
                        if ((tnlfgd12 != NG) && (naloop_tijd < T_max[tnlfgd12] - T_timer[tnlfgd12])) naloop_tijd = T_max[tnlfgd12] - T_timer[tnlfgd12];
                    }

                    if ((tnleg12 != NG) && (naloop_tijd < T_max[tnleg12])) naloop_tijd = T_max[tnleg12];
                    if (kop_eg)
                    {
                        if ((tnlegd12 != NG) && (naloop_tijd < T_max[tnlegd12] + TGL_max[fc1])) naloop_tijd = T_max[tnlegd12] + TGL_max[fc1];
                    }
                    else
                    {
                        if ((tnlegd12 != NG) && (naloop_tijd < T_max[tnlegd12])) naloop_tijd = T_max[tnlegd12];
                    }
                    TEG[fc2] = TEG[fc1] + naloop_tijd + kop_max;
                }

                if (G[fc2] && (status == 3))
                {
                    if ((tnlfg12 != NG)) naloop_tijd = T_max[tnlfg12] - T_timer[tnlfg12];
                    if ((tnlfgd12 != NG) && (naloop_tijd < T_max[tnlfgd12] - T_timer[tnlfgd12])) naloop_tijd = T_max[tnlfgd12] - T_timer[tnlfgd12];
                    if ((tnleg12 != NG) && (naloop_tijd < T_max[tnleg12] - T_timer[tnleg12])) naloop_tijd = T_max[tnleg12] - T_timer[tnleg12];

                    if (tnlegd12 != NG)
                    {
                        if (GL[fc1] && kop_eg)
                        {
                            if (naloop_tijd < TGL_max[fc1] - TGL_timer[fc1] + T_max[tnlegd12]) naloop_tijd = TGL_max[fc1] - TGL_timer[fc1] + T_max[tnlegd12];
                        }
                        else
                        {
                            if (naloop_tijd < T_max[tnlegd12] - T_timer[tnlegd12]) naloop_tijd = T_max[tnlegd12] - T_timer[tnlegd12];
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
    bool foutieve_index = FALSE;

    if ((volgnr != NG) && ((volgnr < 0) || (volgnr >= aantal_vtg_tgo))) foutieve_index = TRUE;

    if (!foutieve_index)
    {
        for (i = 0; i < aantal_vtg_tgo; ++i)
        {
            if (volgnr != NG) i = volgnr;

            /* toegevoegde scope t.b.v. c-versie c89 */
            {
                count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
                count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
                count tnlsgd12 = vtg_tgo[i].tnlsgd12; /* nalooptijd fc2 vanaf startgroen fc1 */
                count tnlsgd21 = vtg_tgo[i].tnlsgd21; /* nalooptijd fc1 vanaf startgroen fc2 */
                count hnla12 = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
                count hnla21 = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */

                if (SG[fc1] && IH[hnla12] && G[fc2] && (TEG[fc2] < T_max[tnlsgd12])) TEG[fc2] = T_max[tnlsgd12];
                if (SG[fc2] && IH[hnla21] && G[fc1] && (TEG[fc1] < T_max[tnlsgd21])) TEG[fc1] = T_max[tnlsgd21];

                if (G[fc1] && (TEG[fc1] < T_max[tnlsgd21] - T_timer[tnlsgd21])) TEG[fc1] = T_max[tnlsgd21] - T_timer[tnlsgd21];
                if (G[fc2] && (TEG[fc2] < T_max[tnlsgd12] - T_timer[tnlsgd12])) TEG[fc2] = T_max[tnlsgd12] - T_timer[tnlsgd12];

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
        {                                 /* FM[] BIT2 wordt geset als VG[] volgrichting HKI beeindigt gaat worden */
            if (MG[i] || VG[i] && (FM[i] & BIT2)) TEG[i] = 0;
            else
            {
                TEG[i] = TFG_max[i] - TFG_timer[i] + TVG_max[i] - TVG_timer[i];
                if (AR[i] && kaapr(i))
                {
                    if (AR_max[i] > TG_timer[i]) TEG[i] = AR_max[i] - TG_timer[i];
                    else                         TEG[i] = 0;
                }
            }                               /* MINTEG[] wordt bestuurd vanuit de applicatie */
            if (PELTEG[i] > NG) TEG[i] = PELTEG[i];
            if (MINTEG[i] > TEG[i]) TEG[i] = MINTEG[i];
        }
    }

#ifdef PRIO_ADDFILE
    BepaalTEG_pri(NG);
#endif
    BepaalTEG_hki(NG);
    BepaalTEG_vtg(NG);

    for (i = 0; i < aantal_dcf_vst; ++i)
    {
        count fc1 = dcf_vst[i].fc1;      /* richting die voorstart geeft  */
        count fc2 = dcf_vst[i].fc2;      /* richting die voorstart krijgt */
        count mv21 = dcf_vst[i].mv21;     /* meeverlengen  van fc2 met fc1 */

        if (G[fc1] && G[fc2] && (mv21 != NG) && SCH[mv21])
        {
            if (TEG[fc2] < TEG[fc1]) TEG[fc2] = TEG[fc1];
        }
    }
}


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
void BepaalExtraOntruim(void)         /* Fik230726                                                          */
{
    count fc, i, j, k;

    for (fc = 0; fc < FCMAX; ++fc)      /* bugfix herstart ontruimingstijden, Fik230726                       */
    {
        if (ExtraOntruim[fc] > TE) ExtraOntruim[fc] -= TE;
        else                       ExtraOntruim[fc] = 0;
    }

    for (fc = 0; fc < FCMAX; ++fc)
    {
        if (HOT[fc])                      /* voertuig is door rood gereden, zie detector_afhandeling_va_arg() */
        {
            for (j = 0; j < GKFC_MAX[fc]; ++j)
            {
                k = KF_pointer[fc][j];        /* fasecylus k nog niet groen en LHOVRA functie R ingeschakeld ? */
                if (!G[k] && HerstartOntruim[k])
                {
                    if (ExtraOntruim[k] < TI_max(fc, k) - TGL_max[fc]) ExtraOntruim[k] = TI_max(fc, k) - TGL_max[fc];
                }
            }

            for (i = 0; i < aantal_dcf_vst; ++i)
            {
                count fc1 = dcf_vst[i].fc1;  /* richting die voorstart geeft  */
                count fc2 = dcf_vst[i].fc2;  /* richting die voorstart krijgt */
                count to12 = dcf_vst[i].to12; /* ontruimingstijd van fc1 naar fc2 */

                /* deelconflict nog niet groen en LHOVRA functie R ingeschakeld ? */
                if ((fc == fc1) && !G[fc2] && HerstartOntruim[fc2])
                {
                    if (ExtraOntruim[fc2] < T_max[to12]) ExtraOntruim[fc2] = T_max[to12];
                }
            }

            for (i = 0; i < aantal_dcf_gst; ++i)
            {
                count fc1 = dcf_gst[i].fc1;  /* richting 1 */
                count fc2 = dcf_gst[i].fc2;  /* richting 2 */
                count to12 = dcf_gst[i].to12; /* ontruimingstijd van fc1 naar fc2 */
                count to21 = dcf_gst[i].to21; /* ontruimingstijd van fc2 naar fc1 */

                /* deelconflict nog niet groen en LHOVRA functie R ingeschakeld ? */
                if ((fc == fc1) && !G[fc2] && HerstartOntruim[fc2])
                {
                    if (ExtraOntruim[fc2] < T_max[to12]) ExtraOntruim[fc2] = T_max[to12];
                }
                /* deelconflict nog niet groen en LHOVRA functie R ingeschakeld ? */
                if ((fc == fc2) && !G[fc1] && HerstartOntruim[fc1])
                {
                    if (ExtraOntruim[fc1] < T_max[to21]) ExtraOntruim[fc1] = T_max[to21];
                }
            }

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
    count i, j, k;
    bool door_rekenen = FALSE;         /* bepalen van MTG[] is een iteratief reken proces */
    mulv  aantal_door_reken = 0;        /* voorkom oneindig rekenen */

    for (i = 0; i < FCMAX; ++i)
    {
        mulv max = 0;
        mulv ontruim = 0;

        BMC[i] = FALSE;                   /* reset BMC[]                                                        */

        if (G[i]) MTG[i] = 0;             /* reset MTG[]                                                        */
        else
        {
            max = 0;                        /* bepaal MTG[]                                                       */

            if (GL[i]) max = TGL_max[i] - TGL_timer[i] + TRG_max[i];
            if (TRG[i]) max = TRG_max[i] - TRG_timer[i];

            for (j = 0; j < GKFC_MAX[i]; ++j)
            {
                k = KF_pointer[i][j];
                if (G[k])                     /* conflict is groen                                                 */
                {
                    if ((TI_max(k, i) >= 0) && (TI_max(k, i) > max)) max = TI_max(k, i);
                }
                else                          /* conflict is niet groen                                            */
                {
                    if ((TI_max(k, i) > 0) && TI(k, i))
                    {
                        ontruim = TI_max(k, i) - TI_timer(k);
                        if (ontruim > max) max = ontruim;
                    }
                }
            }
            ontruim = TMP_ontruim(i);
            if (ontruim > max) max = ontruim;
            MTG[i] = max;
            if (MINTSG[i] > MTG[i]) MTG[i] = MINTSG[i];       /* MINTSG[] wordt vanuit applicatie bestuurd */
            if (DOS_RD[i] > MTG[i]) MTG[i] = DOS_RD[i];       /* DOS_RD[] is minimale roodduur bij doseren */
            if (ExtraOntruim[i] > MTG[i]) MTG[i] = ExtraOntruim[i]; /* ExtraOntruim[] betreft LHOVRA functie R   */
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
            count fc1 = dcf_vst[i].fc1;   /* richting die voorstart geeft  */
            count fc2 = dcf_vst[i].fc2;   /* richting die voorstart krijgt */
            count tvs21 = dcf_vst[i].tvs21; /* voorstart fc2 */
            count ma21 = dcf_vst[i].ma21;  /* meerealisatie van fc2 met fc1 */

            bool meeaanv2 = FALSE;
            if ((ma21 != NG) && SCH[ma21]) meeaanv2 = TRUE;

            if (!G[fc1] && volgrichting_hki(fc1)) /* ga na of fc1 een volgrichting van een harde koppeling is */
            {
                if (!G[fc2] && (GL[fc2] || TRG[fc2] || R[fc2] && A[fc2] || meeaanv2) && (MTG[fc2] + T_max[tvs21] > MTG[fc1]))
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
void RealTraffick(void)               /* Fik230726                                                          */
{
    count i, k;
    bool fc_eerst[FCMAX];              /* richting is als eerstvolgende aan de beurt */
    bool door_rekenen = FALSE;         /* bepalen van REALtraffick[] is een iteratief reken proces */
    mulv  aantal_door_reken = 0;        /* voorkom oneindig rekenen */

    TempConflictMatrix();               /* werk TempConflict matrix bij */
    StatusHKI();                        /* werk status harde koppelingen bij */
    BepaalTEG();                        /* bepaal tijd tot einde groen */
    BepaalExtraOntruim();               /* LHOVRA functie R */
    BepaalMTG();                        /* bepaal minimale tijd tot groen */

    for (i = 0; i < FCMAX; ++i)         /* initialiseer hulp variabelen */
    {
        fc_eerst[i] = FALSE;
    }

    for (i = 0; i < FCMAX; ++i)         /* bepaal welke richtingen als eerst volgende aan de beurt zijn */
    {                                   /* bugfix && !fkrap(i) toegevoegd, Fik230726                    */
        fc_eerst[i] = R[i] && A[i] && AAPR[i] && !fkrap(i) && (!(RR[i] & BIT6) && !(RR[i] & BIT10) || !conflict_prio_real(i)) || RA[i];
    }

    for (i = 0; i < aantal_hki_kop; ++i)
    {
        if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
        {
            count fc1 = hki_kop[i].fc1;    /* voedende richting */
            count fc2 = hki_kop[i].fc2;    /* volg     richting */
            mulv  status = hki_kop[i].status; /* status koppeling */

            if (!G[fc2] && fc_eerst[fc1] || (status == 1)) fc_eerst[fc2] = TRUE;
        }
    }

    for (i = 0; i < aantal_vtg_tgo; ++i)
    {
        count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
        count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
        count hnla12 = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
        count hnla21 = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
        count hlos1 = vtg_tgo[i].hlos1;    /* los realiseren fc1 toegestaan */
        count hlos2 = vtg_tgo[i].hlos2;    /* los realiseren fc2 toegestaan */
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

        bool FcEerst = FALSE;
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
        count fc1 = dcf_vst[i].fc1;      /* richting die voorstart geeft  */
        count fc2 = dcf_vst[i].fc2;      /* richting die voorstart krijgt */
        count ma21 = dcf_vst[i].ma21;     /* meerealisatie van fc2 met fc1 */

        bool meeaanv2 = FALSE;
        if ((ma21 != NG) && SCH[ma21]) meeaanv2 = TRUE;

        if (fc_eerst[fc1])
        {
            if (GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || meeaanv2)) fc_eerst[fc2] = TRUE;
        }
    }

    for (i = 0; i < aantal_dcf_gst; ++i)
    {
        count fc1 = dcf_gst[i].fc1;      /* richting 1 */
        count fc2 = dcf_gst[i].fc2;      /* richting 2 */
        count ma12 = dcf_gst[i].ma12;     /* meerealisatie van fc1 met fc2 */
        count ma21 = dcf_gst[i].ma21;     /* meerealisatie van fc2 met fc1 */

        bool meeaanv1 = FALSE;
        bool meeaanv2 = FALSE;

        if ((ma12 != NG) && SCH[ma12]) meeaanv1 = TRUE;
        if ((ma21 != NG) && SCH[ma21]) meeaanv2 = TRUE;

        if (fc_eerst[fc1])
        {
            if (GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || meeaanv2)) fc_eerst[fc2] = TRUE;
        }

        if (fc_eerst[fc2])
        {
            if (GL[fc1] || TRG[fc1] || R[fc1] && (A[fc1] || meeaanv1)) fc_eerst[fc1] = TRUE;
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
                mulv ontruim = GK_conflict(k, i);
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
                count fc1 = hki_kop[i].fc1;     /* voedende richting */
                count fc2 = hki_kop[i].fc2;     /* volg     richting */
                count tlr21 = hki_kop[i].tlr21;   /* late release fc2 (= inrijtijd fc1) */
                bool los_fc2 = hki_kop[i].los_fc2; /* fc2 mag bij aanvraag fc1 los realiseren */

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
            count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
            count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
            count tinl12 = vtg_tgo[i].tinl12;   /* inlooptijd fc1 */
            count tinl21 = vtg_tgo[i].tinl21;   /* inlooptijd fc2 */
            count hnla12 = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
            count hnla21 = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
            count hlos1 = vtg_tgo[i].hlos1;    /* los realiseren fc1 toegestaan */
            count hlos2 = vtg_tgo[i].hlos2;    /* los realiseren fc2 toegestaan */

            mulv  inloop12 = 0;
            mulv  inloop21 = 0;

            if (tinl12 != NG) inloop12 = T_max[tinl12];
            if (tinl21 != NG) inloop21 = T_max[tinl21];

            if (fc_eerst[fc1] && fc_eerst[fc2])
            {
                if (IH[hnla12] && !IH[hnla21])
                {
                    if (REALtraffick[fc2] < REALtraffick[fc1])
                    {
                        door_rekenen = TRUE;
                        REALtraffick[fc2] = REALtraffick[fc1];
                    }
                    if (REALtraffick[fc1] < REALtraffick[fc2] - inloop12)
                    {
                        door_rekenen = TRUE;
                        REALtraffick[fc1] = REALtraffick[fc2] - inloop12;
                    }
                }
                if (IH[hnla21] && !IH[hnla12])
                {
                    if (REALtraffick[fc1] < REALtraffick[fc2])
                    {
                        door_rekenen = TRUE;
                        REALtraffick[fc1] = REALtraffick[fc2];
                    }
                    if (REALtraffick[fc2] < REALtraffick[fc1] - inloop21)
                    {
                        door_rekenen = TRUE;
                        REALtraffick[fc2] = REALtraffick[fc1] - inloop21;
                    }
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
            count fc1 = dcf_vst[i].fc1;   /* richting die voorstart geeft  */
            count fc2 = dcf_vst[i].fc2;   /* richting die voorstart krijgt */
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

        for (i = 0; i < aantal_dcf_gst; ++i)
        {
            count fc1 = dcf_gst[i].fc1;   /* richting 1 */
            count fc2 = dcf_gst[i].fc2;   /* richting 2 */

            if (fc_eerst[fc1] && fc_eerst[fc2])
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

#ifdef hrgvact                        /* is RobuGrover aanwezig ?                                           */
    if ((FILE_set > 0) && (FILE_set <= aantal_file_prg) || FILE_nass) IH[hrgvact] = FALSE;
    if (IH[hrgvact])
    {
        for (i = 0; i < FCMAX; ++i)
        {
            if ((US_type[i] & MVT_type) && MK_DST[i])
            {
                IH[hrgvact] = FALSE;
                break;
            }
        }
    }
#endif
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
            count fc1 = hki_kop[i].fc1;      /* voedende richting */
            count fc2 = hki_kop[i].fc2;      /* volg     richting */
            count tnlfg12 = hki_kop[i].tnlfg12;  /* vaste   nalooptijd vanaf einde vastgroen      fc1 */
            count tnlfgd12 = hki_kop[i].tnlfgd12; /* det.afh.nalooptijd vanaf einde vastgroen      fc1 */
            count tnleg12 = hki_kop[i].tnleg12;  /* vaste   nalooptijd vanaf einde (verleng)groen fc1 */
            count tnlegd12 = hki_kop[i].tnlegd12; /* det.afh.nalooptijd vanaf einde (verleng)groen fc1 */
            bool kop_eg = hki_kop[i].kop_eg;   /* koppeling vanaf einde groen (als FALSE dan vanaf EV) */
            mulv  kop_max = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */

            mulv  TVG_instelling = TVG_max[fc2];  /* buffer TVG_max[] volgrichting */
            mulv  TGL_instelling = TGL_max[fc2];  /* ... en TGL_max[] volgrichting */
            bool RT_buffer = FALSE;

            if (TVG_max[fc2] > kop_max) TVG_max[fc2] = kop_max;
            if (TGL_max[fc2] < 1) TGL_max[fc2] = 1;

            if (tnlfg12 != NG) NaloopFG(fc1, fc2, tnlfg12);
            if (tnlfgd12 != NG)
            {
                RT_buffer = RT[tnlfgd12];     /* buffer RT[], die wordt anders gereset in NaloopFGDet() */
                NaloopFGDet(fc1, fc2, tnlfgd12, END);
                RT[tnlfgd12] = RT_buffer;     /* zet juiste waarde RT[] terug */
            }
            if (kop_eg && (tnleg12 != NG)) NaloopEG(fc1, fc2, tnleg12);
            if (kop_eg && (tnlegd12 != NG))
            {
                RT_buffer = RT[tnlegd12];     /* buffer RT[], die wordt anders gereset in NaloopEGDet() */
                NaloopEGDet(fc1, fc2, tnlegd12, END);
                RT[tnlegd12] = RT_buffer;     /* zet juiste waarde RT[] terug */
            }
            if (!kop_eg && (tnleg12 != NG)) NaloopCV(fc1, fc2, tnleg12);
            if (!kop_eg && (tnlegd12 != NG))
            {
                RT_buffer = RT[tnlegd12];     /* buffer RT[], die wordt anders gereset in NaloopCVDet() */
                NaloopCVDet(fc1, fc2, tnlegd12, END);
                RT[tnlegd12] = RT_buffer;     /* zet juiste waarde RT[] terug */
            }

            TVG_max[fc2] = TVG_instelling;  /* zet juiste waarde TVG_max[] en TGL_max[] volgrichting terug */
            TGL_max[fc2] = TGL_instelling;
        }
    }

    for (i = 0; i < aantal_vtg_tgo; ++i)
    {
        count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
        count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
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
void RealTraffickPrioriteit(void)     /* Fik230830                                                          */
{
    count i, j, k, prio;

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
                if ((iPrioriteitsOpties[prio] & poNoodDienst) || (iPrioriteitsOpties[prio] & poBijzonderRealiseren))
                {
                    for (k = 0; k < FCMAX; ++k)
                    {
                        if (GK_conflict(fc, k) > NG)         /* corrigeer REALtraffick voor conflicten die nog rood zijn */
                        {
                            if (RV[k] || RA[k] && !P[k]) REALtraffick[k] = NG;
                        }
                    }
                    /* TLCGen corrigeert niet voor gelijkstartende richtingen */
                    if ((REALtraffick[fc] == NG) || (REALtraffick[fc] >= (mulv)iStartGroen[prio]) && (prio != prio_index[fc].FTS))
                    {
                        /* AAPRprio[] beindigt (als nodig) MG[] van de conflicten */
                        if (REALtraffick[fc] == NG) AAPRprio[fc] = TRUE;

                        REALtraffick[fc] = (mulv)iStartGroen[prio];
                        if (MTG[fc] > REALtraffick[fc]) REALtraffick[fc] = MTG[fc]; /* toegevoegd Fik230830 */

                        for (j = 0; j < GKFC_MAX[fc]; ++j)
                        {
                            k = KF_pointer[fc][j];
                            if (G[k])                         /* corrigeer TEG[] voor conflicten die groen zijn */
                            {
                                if (TI_max(k, fc) >= 0)
                                {
                                    if (TEG[k] > REALtraffick[fc] - TI_max(k, fc))
                                    {
                                        TEG[k] = REALtraffick[fc] - TI_max(k, fc);
                                        if (TEG[k] < 0) TEG[k] = 0;
                                    }
                                }
                                if (TI_max(k, fc) == GK)
                                {
                                    if (TEG[k] > REALtraffick[fc])
                                    {
                                        TEG[k] = REALtraffick[fc];
                                        if (TEG[k] < 0) TEG[k] = 0;
                                    }
                                }
#ifdef NALOPEN
                                if ((TI_max(k, fc) == GKL) && TGK[k][fc])
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
        count fc1 = dcf_vst[i].fc1;     /* richting die voorstart geeft  */
        count fc2 = dcf_vst[i].fc2;     /* richting die voorstart krijgt */
        count tvs21 = dcf_vst[i].tvs21;   /* voorstart fc2 */
        count ma21 = dcf_vst[i].ma21;    /* meerealisatie van fc2 met fc1 */
        count mv21 = dcf_vst[i].mv21;    /* meeverlengen  van fc2 met fc1 */

        bool meeaanv2 = FALSE;
        if ((ma21 != NG) && SCH[ma21]) meeaanv2 = TRUE;

        if ((REALtraffick[fc1] > NG) && R[fc1] && A[fc1] && (GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || meeaanv2)))
        {
            if (REALtraffick[fc1] < MTG[fc2] + T_max[tvs21])
            {
                REALtraffick[fc1] = MTG[fc2] + T_max[tvs21];
            }
        }

        if (G[fc1] && G[fc2] && (mv21 != NG) && SCH[mv21])
        {
            if (TEG[fc2] < TEG[fc1]) TEG[fc2] = TEG[fc1];
        }
    }

    for (i = 0; i < aantal_dcf_gst; ++i)
    {
        count fc1 = dcf_gst[i].fc1;     /* richting 1 */
        count fc2 = dcf_gst[i].fc2;     /* richting 2 */
        count ma12 = dcf_gst[i].ma12;    /* meerealisatie van fc1 met fc1 */
        count ma21 = dcf_gst[i].ma21;    /* meerealisatie van fc2 met fc1 */

        bool meeaanv1 = FALSE;
        bool meeaanv2 = FALSE;

        if ((ma12 != NG) && SCH[ma12]) meeaanv1 = TRUE;
        if ((ma21 != NG) && SCH[ma21]) meeaanv2 = TRUE;

        if ((REALtraffick[fc1] > NG) && R[fc1] && A[fc1] && (GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || meeaanv2)))
        {
            if (REALtraffick[fc1] < MTG[fc2])
            {
                REALtraffick[fc1] = MTG[fc2];
            }
        }

        if ((REALtraffick[fc2] > NG) && R[fc2] && A[fc2] && (GL[fc1] || TRG[fc1] || R[fc1] && (A[fc1] || meeaanv1)))
        {
            if (REALtraffick[fc2] < MTG[fc1])
            {
                REALtraffick[fc2] = MTG[fc1];
            }
        }
    }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal maximum(verleng)groentijden tijdens DVM en/of FILE stroomopwaarts.                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Tijdens DVM en/of FILE stroomopwaartw worden vooraf gedefinieerde maximum(verleng)groentijden sets van   */
/* kracht. Deze functie zorgt voor de juiste instelling van TVG_max[] en bepaalt of een richting wordt      */
/* bevoordeeld door de ingezette maatregel.                                                                 */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Maxgroen_Add().                                                         */
/*                                                                                                          */
void bepaal_maximum_groen_traffick(void) /* Fik230101                                                       */
{
    count fc;

    for (fc = 0; fc < FCMAX; ++fc)
    {
        FC_DVM[fc] = FALSE;              /* reset bevoordeling door DVM */
        FC_FILE[fc] = FALSE;              /* reset bevoordeling door FILE stroomopwaarts */
    }

    if (DVM_prog > 0)                   /* er is een DVM programma actief */
    {                                   /* controleer of waarde DVM_prog juist is */
        if ((DVM_prog <= aantal_dvm_prg) && (DVM_prog <= MAX_DVM_PRG))
        {
            for (fc = 0; fc < FCMAX; ++fc)
            {
                mulv TVG_klok = TVG_max[fc];  /* TVG_max[] volgens de klok */

                if (DVM_max[fc].dvm_set[DVM_prog - 1] != NG)
                {
                    if (PRM[DVM_max[fc].dvm_set[DVM_prog - 1]] == 0)
                    {
                        TVG_max[fc] = TVG_klok;   /* instelling "0" betekent aanhouden TVG_max[] volgens de klok */
                    }
                    else
                    {
                        if (max_verleng_groen)    /* instelling is voor maximum verlenggroen */
                        {
                            TVG_max[fc] = PRM[DVM_max[fc].dvm_set[DVM_prog - 1]];
                        }
                        else                      /* instelling is voor maximum groen */
                        {
                            if (PRM[DVM_max[fc].dvm_set[DVM_prog - 1]] > TFG_max[fc])
                            {
                                TVG_max[fc] = PRM[DVM_max[fc].dvm_set[DVM_prog - 1]] - TFG_max[fc];
                            }
                            else                    /* maximum groen is kleiner of gelijk aan TFG_max[] */
                            {
                                TVG_max[fc] = 0;      /* ... dus TVG_max[] is dan gelijk aan "0" */
                            }
                        }
                    }
                }
                /* bepaal of richting wordt bevoordeeld als gevolg van DVM */
                if (TVG_max[fc] > TVG_klok) FC_DVM[fc] = TRUE;
            }
        }
    }
    else                                /* alleen als er geen DVM programma actief is */
    {
        if (FILE_set > 0)                 /* er is een FILE set gewenst */
        {                                 /* controleer of waarde FILE_set juist is */
            if ((FILE_set <= aantal_file_prg) && (FILE_set <= MAX_FILE_PRG))
            {
                for (fc = 0; fc < FCMAX; ++fc)
                {
                    mulv TVG_klok = TVG_max[fc]; /* TVG_max[] volgens de klok */

                    if (FILE_max[fc].file_set[FILE_set - 1] != NG)
                    {
                        if (PRM[FILE_max[fc].file_set[FILE_set - 1]] == 0)
                        {
                            TVG_max[fc] = TVG_klok; /* instelling "0" betekent aanhouden TVG_max[] volgens de klok */
                        }
                        else
                        {
                            if (max_verleng_groen)  /* instelling is voor maximum verlenggroen */
                            {
                                TVG_max[fc] = PRM[FILE_max[fc].file_set[FILE_set - 1]];
                            }
                            else                    /* instelling is voor maximum groen */
                            {
                                if (PRM[FILE_max[fc].file_set[FILE_set - 1]] > TFG_max[fc])
                                {
                                    TVG_max[fc] = PRM[FILE_max[fc].file_set[FILE_set - 1]] - TFG_max[fc];
                                }
                                else                  /* maximum groen is kleiner of gelijk aan TFG_max[] */
                                {
                                    TVG_max[fc] = 0;    /* ... dus TVG_max[] is dan gelijk aan "0" */
                                }
                            }
                        }
                    }
                    /* bepaal of richting wordt bevoordeeld a.g.v. FILE stroomopwaarts */
                    if (TVG_max[fc] > TVG_klok) FC_FILE[fc] = TRUE;
                }
            }
        }
    }
}


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
    count i, k;

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
            mulv ontruim = GK_conflict(i, k);
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
bool REALconflict(                   /* Fik230101                                                          */
    count fc)                             /* FC fasecyclus                                                      */
{
    count k;

    for (k = 0; k < FCMAX; ++k)
    {
        if (GK_conflict(fc, k) > NG)
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
/* Peloton ingreep - groen vasthouden                                                                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie zorgt voor het aanhouden van het wachtgroen bij een peloton ingreep.                        */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Wachtgroen_Add().                                                       */
/*                                                                                                          */
void peloton_ingreep_wachtgroen(void) /* Fik230101                                                          */
{
    count i;

    for (i = 0; i < aantal_pel_kop; ++i)
    {
        count kop_fc = pel_kop[i].kop_fc;      /* koppelrichting */
        count kop_toe = pel_kop[i].kop_toe;     /* toestemming peloton ingreep */
        count vast_vert = pel_kop[i].vast_vert;   /* vasthoud vertraging  (start op binnenkomst koppelsignaal) */
        count duur_vast = pel_kop[i].duur_vast;   /* duur vasthouden (bij duursign. na afvallen koppelsignaal) */
        count hnaloop_1 = pel_kop[i].hnaloop_1;   /* voorwaarde herstart extra nalooptijd 1 (nalooplus 1) */
        count tnaloop_1 = pel_kop[i].tnaloop_1;   /* nalooptijd 1 */
        count hnaloop_2 = pel_kop[i].hnaloop_2;   /* voorwaarde herstart extra nalooptijd 2 (nalooplus 2) */
        count tnaloop_2 = pel_kop[i].tnaloop_2;   /* nalooptijd 2 */
        mulv  aanw_kop1 = pel_kop[i].aanw_kop1;   /* aanwezigheidsduur koppelsignaal 1 vanaf start puls */
        mulv  pk_status = pel_kop[i].pk_status;   /* status peloton ingreep */
        bool pk_afronden = pel_kop[i].pk_afronden; /* afronden lopende peloton ingreep */

        if ((pk_status > NG) && (MM[kop_toe] || (RW[kop_fc] & BIT12) && !pk_afronden) && (aanw_kop1 > T_max[vast_vert]) ||
            (pk_status == 3) && (aanw_kop1 > NG) && (aanw_kop1 <= T_max[vast_vert]) && !pk_afronden)
        {
            RW[kop_fc] &= ~BIT12;
            if (G[kop_fc] && !DOSEER[kop_fc])
            {
                if (!fkra(kop_fc))            /* geen RW[] als (fictief)conflict in RA[] staat, ivm P[] */
                {
                    RW[kop_fc] |= BIT12;
                    pk_status = 3;
                }
            }
        }
        else RW[kop_fc] &= ~BIT12;

        if ((hnaloop_1 != NG) && (tnaloop_1 != NG))   /* is er een 1e naloop lus ? */
        {
            bool naloop_lus_actief = FALSE;

            RT[tnaloop_1] = IH[hnaloop_1] && (RW[kop_fc] & BIT12);
            if (RT[tnaloop_1] || T[tnaloop_1]) naloop_lus_actief = TRUE;

            if ((hnaloop_2 != NG) && (tnaloop_2 != NG)) /* is er ook een 2e naloop lus ? */
            {
                RT[tnaloop_2] = IH[hnaloop_2] && (RT[tnaloop_1] || T[tnaloop_1]);
                if (RT[tnaloop_2] || T[tnaloop_2]) naloop_lus_actief = TRUE;
            }

            if (G[kop_fc] && !DOSEER[kop_fc] && !fkra(kop_fc) && naloop_lus_actief)
            {
                if (!(RW[kop_fc] & BIT12))
                {
                    RW[kop_fc] |= BIT12;
                    if (T[tnaloop_1]) pk_status = 4;
                    else              pk_status = 5;
                }
            }
        }
        pel_kop[i].pk_status = pk_status;
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
    bool  hmk1_2, hmk3;	/* hulpwaarden meetkriterium			*/
    bool htdh_status;	/* hulpwaarde status hiaatmeting		*/

    va_start(argpt, tkopmaxnr);		/* start var. parameterlijst	*/
    if (G[fc]) {				/* alleen tijdens groen 	*/
        if (TFG[fc]) {                    /* test lopen vastgroentijd	*/
            MK[fc] |= (BIT1 + BIT2);		/* zet bit 1 en 2		*/
        }
        else {
            hmk1_2 = hmk3 = 0;		  /* hulpwaarden worden 0	*/
            do {
                dpnr = va_arg(argpt, va_count);/* lees array-nummer detectie	*/
                if (dpnr >= 0) {
                    prm = va_arg(argpt, va_mulv); /* lees waarde parameter	*/
                    if (prm > END) {
                        htdh_status = TDH[dpnr];
                        if (prm && htdh_status) { /* test waarde en hiaattijd	*/
                            if (prm == 1) {
                                hmk1_2 |= BIT1;     /* set verlengbit 1		*/
                            }
                            else if (prm == 2) {
                                hmk1_2 |= BIT2;     /* set verlengbit 2		*/
                            }
                            else {
                                hmk3 |= BIT3;       /* set verlengbit 3		*/
                            }
                        }
                    }
                }
            } while (dpnr >= 0 && prm > END);	/* laatste parameter?		*/
            if (tkopmaxnr >= 0) {		/* kopmaximum gebruikt?		*/
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
    mulv  htkopmax = TRUE; /* hulpwaarde tijdkopmaximum			*/
    mulv  htdh_status;	/* status hiaatmeting				*/

    va_start(argpt, mmk);			/* start var. parameterlijst	*/
    if (G[fc]) {				/* alleen tijdens groen 	*/
        if (tkopmaxnr >= 0) {		/* kopmaximum gebruikt?		*/
            if (!T[tkopmaxnr])	 	/* test lopen tijd kopmaximum	*/
                htkopmax = FALSE;            /* niet lopen -> reset bit1	*/
        }
        if (TFG[fc]) {                    /* test lopen vastgroentijd	*/
            htkopmax = TRUE;
        }
        else {
            hmk1_2 = hmk3 = 0;		  /* hulpwaarden worden 0	*/
            hmk1_2a = hmk1_2b = hmk1_2c = 0;	  /* hulpwaarden worden 0	*/
            do {
                dpnr = va_arg(argpt, va_count);/* lees array-nummer detectie	*/
                if (dpnr >= 0) {
                    prmmk = va_arg(argpt, va_mulv); /* lees waarde parameter  */
                    prmrystr = prmmk >> 2;		/* lees waarde rijstrook  */
                    prmmk &= BIT0 | BIT1;            /* skip waarde rijstrook  */
                    if (prmmk > END) {
                        htdh_status = TDH[dpnr];
                        if (prmmk && htdh_status) {   /* test waarde en hiaattijd    */
                            if ((prmmk == (va_mulv)1)) {
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
                            else if (prmmk == (va_mulv)2) {
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
                                    hmk1_2 |= BIT2;    /* set verlengbit 2	*/
                                    break;
                                }
                            }
                            else {
                                hmk3 |= BIT3;       /* set verlengbit 3	*/
                            }
                        }
                    }
                }
            } while (dpnr >= 0 && prmmk > END);	/* laatste parameter?		*/

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
/* Peloton ingreep - groen verlengen                                                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie zorgt voor het verlengen na afloop van een peloton ingreep.                                 */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Meetkriterium_Add().                                                    */
/*                                                                                                          */
void peloton_ingreep_verlengen(void) /* Fik230101                                                           */
{
    count i;

    for (i = 0; i < aantal_pel_kop; ++i)
    {
        count kop_fc = pel_kop[i].kop_fc;    /* koppelrichting */
        count duur_verl = pel_kop[i].duur_verl; /* duur verlengen na ingreep (bij NUL geldt TVG_max[]) */
        mulv  pk_status = pel_kop[i].pk_status; /* status peloton ingreep */

        FM[kop_fc] &= ~BIT12;
        YV[kop_fc] &= ~BIT12;

        if (pk_status >= 3)
        {
            if (G[kop_fc] && !MG[kop_fc] && !DOSEER[kop_fc] && !(RW[kop_fc] & BIT12))
            {
                YV[kop_fc] |= BIT12;
                if (pk_status <= 5)
                {
                    if (duur_verl == NG) PELTEG[kop_fc] = 0;
                    else
                    {
                        if (T_max[duur_verl] == 0) PELTEG[kop_fc] = TVG_max[kop_fc];
                        else                       PELTEG[kop_fc] = T_max[duur_verl];
                    }

                    if (PELTEG[kop_fc] < TFG_max[kop_fc] + TVG_max[kop_fc] - TG_timer[kop_fc])
                    {
                        PELTEG[kop_fc] = TFG_max[kop_fc] + TVG_max[kop_fc] - TG_timer[kop_fc];
                    }
                    pk_status = 6;
                }
            }
        }

        if ((WG[kop_fc] || VG[kop_fc]) && (pk_status == 6))
        {
            bool einde_ingreep = (PELTEG[kop_fc] == 0);

            if (VG[kop_fc] && (TVG_timer[kop_fc] >= TVG_max[kop_fc]) || !MK[kop_fc]) einde_ingreep = TRUE;

            if (VG[kop_fc] && (duur_verl > NG) && tka(kop_fc))
            {
                if ((TVG_timer[kop_fc] >= T_max[duur_verl]) && (T_max[duur_verl] > 0) &&
                    (TG_timer[kop_fc] >= TFG_max[kop_fc] + TVG_max[kop_fc])) einde_ingreep = TRUE;
            }

            if (VG[kop_fc] && (duur_verl == NG) && tka(kop_fc))
            {
                if (TG_timer[kop_fc] >= TFG_max[kop_fc] + TVG_max[kop_fc]) einde_ingreep = TRUE;
            }

            if (einde_ingreep)
            {
                if (VG[kop_fc]) FM[kop_fc] |= BIT12;
                YV[kop_fc] &= ~BIT12;
                pk_status = 0;
            }
        }
        pel_kop[i].pk_status = pk_status;
    }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie bepaal maximaal meeverlengen                                                                     */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie bepaalt of richtingen nog kunnen meeverlengen op basis van "maatgevend groen". Indien dit   */
/* niet meer het geval is wordt YM[] BIT4 gereset.                                                          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Meeverlengen_Add().                                                     */
/*                                                                                                          */
void Traffick2TLCgen_MVG(void)        /* Fik230901                                                          */
{
    count i, k;

    bool hf_mvg = FALSE;               /* hulpfunctie meeverlenggroen toegestaan */
    bool fc_eerst[FCMAX];              /* richting is als eerstvolgende aan de beurt */
    bool ym_reset[FCMAX];              /* meeverlenggroen moet beeindigd worden */
    bool rw_reset[FCMAX];              /* RW[] BIT2 door TLCgen opgezet tijdens RA[] voedende richting */
    bool fc_mv_dc[FCMAX];              /* meeverlenggroen van richting die in deelconflict mag meeverlengen */

    for (i = 0; i < FCMAX; ++i)         /* bepaal toestemming aanhouden veiligheidsgroen */
    {
        if (!G[i]) VG_mag[i] = FALSE;
        else
        {
            if (!MK[i]) VG_mag[i] = TRUE;   /* alleen toestemming indien tijdens de groenfase hiaat is gemeten */
        }                                 /* ... geen toestemming ? dan reset veiligheidsgroenbit (YM[]BIT2) */
        if (MG[i] && !VG_mag[i]) YM[i] &= ~BIT2;
    }

    for (i = 0; i < FCMAX; ++i)         /* initialiseer YM[] BIT 4 en hulp variabelen */
    {
        YM[i] &= ~BIT4;
        fc_eerst[i] = FALSE;
        ym_reset[i] = FALSE;
        rw_reset[i] = TRUE;
        fc_mv_dc[i] = FALSE;
        hf_mvg |= !G[i] && (REALtraffick[i] > NG);
    }

    for (i = 0; i < FCMAX; ++i)         /* bepaal welke richtingen als eerst volgende aan de beurt zijn */
    {
        fc_eerst[i] = R[i] && A[i] && AAPR[i] && !(AAPR[i] & BIT5) && !fkra(i) && !(RR[i] & BIT6) && !(RR[i] & BIT10) || RA[i] || AAPRprio[i];
    }

    for (i = 0; i < aantal_hki_kop; ++i)
    {
        if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
        {
            count fc1 = hki_kop[i].fc1;    /* voedende richting */
            count fc2 = hki_kop[i].fc2;    /* volg     richting */
            count status = hki_kop[i].status; /* status koppeling */

            if (!G[fc2] && fc_eerst[fc1] || (status == 1)) fc_eerst[fc2] = TRUE;
        }
    }

    for (i = 0; i < aantal_vtg_tgo; ++i)
    {
        count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
        count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
        count hnla12 = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
        count hnla21 = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
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

        bool FcEerst = FALSE;
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
        count fc1 = dcf_vst[i].fc1;      /* richting die voorstart geeft  */
        count fc2 = dcf_vst[i].fc2;      /* richting die voorstart krijgt */
        count ma21 = dcf_vst[i].ma21;     /* meerealisatie van fc2 met fc1 */
        count mv21 = dcf_vst[i].mv21;     /* meeverlengen  van fc2 met fc1 */

        bool meeaanv2 = FALSE;
        if ((ma21 != NG) && SCH[ma21]) meeaanv2 = TRUE;

        if (fc_eerst[fc1])
        {
            if (GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || meeaanv2)) fc_eerst[fc2] = TRUE;
        }
        if (G[fc1] && G[fc2] && (mv21 != NG) && SCH[mv21] && !DOSEER[fc2]) fc_mv_dc[fc2] = TRUE;
    }

    for (i = 0; i < aantal_dcf_gst; ++i)
    {
        count fc1 = dcf_gst[i].fc1;      /* richting 1 */
        count fc2 = dcf_gst[i].fc2;      /* richting 2 */
        count ma12 = dcf_gst[i].ma12;     /* meerealisatie van fc1 met fc2 */
        count ma21 = dcf_gst[i].ma21;     /* meerealisatie van fc2 met fc1 */

        bool meeaanv1 = FALSE;
        bool meeaanv2 = FALSE;

        if ((ma12 != NG) && SCH[ma12]) meeaanv1 = TRUE;
        if ((ma21 != NG) && SCH[ma21]) meeaanv2 = TRUE;

        if (fc_eerst[fc1])
        {
            if (GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || meeaanv2)) fc_eerst[fc2] = TRUE;
        }

        if (fc_eerst[fc2])
        {
            if (GL[fc1] || TRG[fc1] || R[fc1] && (A[fc1] || meeaanv1)) fc_eerst[fc1] = TRUE;
        }
    }

    for (i = 0; i < FCMAX; ++i)         /* bepaal of meeverlengen mag ogv instellingen */
    {
        if ((MGR[i] || fc_mv_dc[i]) && (MK[i] || !MMK[i] || fc_mv_dc[i]) && (hf_wsg_fcfc(0, FCMAX) || hf_mvg)) YM[i] |= BIT4;
        else
        {
            ym_reset[i] = TRUE;
        }
    }

    for (i = 0; i < aantal_mee_rea; ++i)
    {
        count fc1 = mee_rea[i].fc1;      /* richting die meerealisatie geeft  */
        count fc2 = mee_rea[i].fc2;      /* richting die meerealisatie krijgt */
        count mv21 = mee_rea[i].mv21;     /* meeverlengen van fc2 met fc1 */

        if ((mv21 != NG) && ym_reset[fc2] && !DOSEER[fc2])
        {
            if (G[fc1] && G[fc2] && SCH[mv21] && (hf_wsg_fcfc(0, FCMAX) || hf_mvg))
            {
                YM[fc2] |= BIT4;
                ym_reset[fc2] = FALSE;
            }
        }
    }

    for (i = 0; i < aantal_hki_kop; ++i)
    {
        if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
        {
            count fc1 = hki_kop[i].fc1;    /* voedende richting */
            count fc2 = hki_kop[i].fc2;    /* volg     richting */
            bool kop_eg = hki_kop[i].kop_eg; /* koppeling vanaf einde groen (als FALSE dan vanaf EV) */
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
                mulv ontruim = GK_conflict(i, k);
                if (ontruim > NG)
                {
                    if (fc_eerst[k])            /* conflict gevonden die aan de beurt is       */
                    {                           /* (REALtraffick[k] > NG) toegevoegd Fik230901 */
                        if ((REALtraffick[k] > NG) && (REALtraffick[k] <= ontruim)) ym_reset[i] = TRUE;
                    }
                }
            }
        }
    }

    for (i = 0; i < aantal_hki_kop; ++i)
    {
        if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
        {
            count fc1 = hki_kop[i].fc1;      /* voedende richting */
            count fc2 = hki_kop[i].fc2;      /* volg     richting */
            count tnlfg12 = hki_kop[i].tnlfg12;  /* vaste   nalooptijd vanaf einde vastgroen      fc1 */
            count tnlfgd12 = hki_kop[i].tnlfgd12; /* det.afh.nalooptijd vanaf einde vastgroen      fc1 */
            count tnleg12 = hki_kop[i].tnleg12;  /* vaste   nalooptijd vanaf einde (verleng)groen fc1 */
            count tnlegd12 = hki_kop[i].tnlegd12; /* det.afh.nalooptijd vanaf einde (verleng)groen fc1 */
            bool kop_eg = hki_kop[i].kop_eg;   /* koppeling vanaf einde groen (als FALSE dan vanaf EV) */
            mulv  kop_max = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */

            if (fc_eerst[fc1])              /* voedende richting harde koppeling is aan de beurt, ga op zoek naar conflicten van de volgrichting die in MG[] staan */
            {
                for (k = 0; k < FCMAX; ++k)
                {
                    mulv ontruim = GK_conflict(k, fc2);
                    if (ontruim > NG)
                    {
                        if (MG[k])                /* conflict volgrichting gevonden die in MG[] staat */
                        {                         /* (REALtraffick[fc2] > NG) toegevoegd Fik230901    */
                            if ((REALtraffick[fc2] > NG) && (REALtraffick[fc2] <= ontruim) || BMC[fc2] && RA[fc1] && !tkcv(fc1)) ym_reset[k] = TRUE;
                        }
                    }
                }
            }

            if (MG[fc1] && kop_eg)          /* voedende richting harde koppeling staat in MG[], ga op zoek naar een conflict van de volgrichting die aan de beurt is */
            {
                mulv naloop_tijd = 0;

                if ((tnlfg12 != NG)) naloop_tijd = T_max[tnlfg12] - T_timer[tnlfg12];
                if ((tnlfgd12 != NG) && (naloop_tijd < T_max[tnlfgd12] - T_timer[tnlfgd12])) naloop_tijd = T_max[tnlfgd12] - T_timer[tnlfgd12];
                if ((tnleg12 != NG) && (naloop_tijd < T_max[tnleg12])) naloop_tijd = T_max[tnleg12];
                if ((tnlegd12 != NG) && (naloop_tijd < T_max[tnlegd12] + TGL_max[fc1])) naloop_tijd = T_max[tnlegd12] + TGL_max[fc1];

                naloop_tijd += kop_max;

                for (k = 0; k < FCMAX; ++k)
                {
                    mulv ontruim = GK_conflict(fc2, k);
                    if (ontruim > NG)
                    {
                        if (fc_eerst[k])          /* conflict volgrichting gevonden die aan de beurt is */
                        {                         /* (REALtraffick[k] > NG) toegevoegd Fik230901        */
                            if ((REALtraffick[k] > NG) && (REALtraffick[k] <= ontruim + naloop_tijd)) ym_reset[fc1] = TRUE;
                        }
                    }
                    if (ym_reset[fc1]) break;
                }
            }
        }
    }

    for (i = 0; i < aantal_dcf_vst; ++i)
    {
        count fc1 = dcf_vst[i].fc1;      /* richting die voorstart geeft  */
        count fc2 = dcf_vst[i].fc2;      /* richting die voorstart krijgt */
        count mv21 = dcf_vst[i].mv21;     /* meeverlengen  van fc2 met fc1 */

        if (MG[fc1] && MG[fc2] && (mv21 != NG) && SCH[mv21] && ym_reset[fc2]) ym_reset[fc1] = TRUE;
    }

    for (i = 0; i < FCMAX; ++i)         /* reset YM BIT4 */
    {
        if (ym_reset[i]) YM[i] &= ~BIT4;
    }

    for (i = 0; i < FCMAX; ++i)         /* correctie TLCgen geen RW[] maar YM[] tijdens RA[] voedende richting */
    {
        if (rw_reset[i] && (RW[i] & BIT2))
        {
            RW[i] &= ~BIT2;
            YM[i] |= BIT4;
        }
    }

    for (i = 0; i < aantal_vtg_tgo; ++i)
    {
        count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
        count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
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

        bool FcInRa = FALSE;
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
        count fc1 = dcf_vst[i].fc1;      /* richting die voorstart geeft  */
        count fc2 = dcf_vst[i].fc2;      /* richting die voorstart krijgt */
        count mv21 = dcf_vst[i].mv21;     /* meeverlengen van fc2 met fc1  */

        if (RA[fc1] || G[fc1] && (mv21 != NG) && SCH[mv21] && !DOSEER[fc2]) YM[fc2] |= BIT4;
    }

    for (i = 0; i < aantal_pel_kop; ++i)
    {
        count kop_fc = pel_kop[i].kop_fc;    /* koppelrichting */
        count kop_toe = pel_kop[i].kop_toe;   /* toestemming peloton ingreep */
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
void Traffick2TLCgen_uitstel(void)    /* Fik230830                                                          */
{
    count i;
    mulv  aantal_x3 = 0;            /* bepalen van uitstel is een iteratief proces */
    mulv  aantal_x3_old = NG;           /* ... tot er geen nieuwe richtingen meer een X[] BIT3 krijgen */

    for (i = 0; i < FCMAX; ++i)
    {
        P[i] &= ~BIT3;                   /* reset P, X en RR bit synchronisatie */
        X[i] &= ~BIT3;
        RR[i] &= ~BIT3;

        X[i] &= ~BIT1;                   /* reset realfunc bits agv hoge REALtijd */
        RR[i] &= ~BIT1;

        if (RA[i] && (ExtraOntruim[i] > 0)) X[i] |= BIT3; /* toegevoegd Fik230830 */
        if (RA[i] && (Aled[i] > 0)) P[i] |= BIT3;
    }

    while (aantal_x3 > aantal_x3_old)
    {
        aantal_x3_old = aantal_x3;

        for (i = 0; i < aantal_hki_kop; ++i)
        {
            count fc1 = hki_kop[i].fc1;     /* voedende richting */
            count fc2 = hki_kop[i].fc2;     /* volg     richting */
            count tlr21 = hki_kop[i].tlr21;   /* late release fc2 (= inrijtijd fc1) */
            bool los_fc2 = hki_kop[i].los_fc2; /* fc2 mag bij aanvraag fc1 los realiseren */
            mulv  status = hki_kop[i].status;  /* status koppeling */

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
            if (RA[fc1] && (inrijfc1 == 0) && (GL[fc2] || TRG[fc2] || X[fc2] || K[fc2] || (TMP_ontruim(fc2) > 0) || RA[fc2] && RR[fc2] && !P[fc2])) X[fc1] |= BIT3;
            if (RA[fc1] && (inrijfc1 > 0) && ((REALtraffick[fc2] > inrijfc1) || (MTG[fc2] > inrijfc1))) X[fc1] |= BIT3;
        }

        for (i = 0; i < aantal_dcf_vst; ++i)
        {
            count fc1 = dcf_vst[i].fc1;   /* richting die voorstart geeft  */
            count fc2 = dcf_vst[i].fc2;   /* richting die voorstart krijgt */
            count tvs21 = dcf_vst[i].tvs21; /* voorstart fc2                 */
            count ma21 = dcf_vst[i].ma21;  /* meerealisatie van fc2 met fc1 */

            bool fc2_eerst = FALSE;
            bool meeaanv2 = FALSE;
            if ((ma21 != NG) && SCH[ma21]) meeaanv2 = TRUE;

            fc2_eerst = GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || meeaanv2) || G[fc2] && (TG_timer[fc2] < T_max[tvs21]);

            if (G[fc1] && (!MG[fc1] || (YM[fc1] & PRIO_YM_BIT))) RR[fc2] |= BIT3;
            if (!G[fc1] && fc2_eerst) X[fc1] |= BIT3;
            /* if (RA[fc1] && !K[fc1] && meeaanv2) P[fc1] |= BIT3; */
            if (RA[fc1] && P[fc1] && !P[fc2]) P[fc2] |= BIT3;
            if (RA[fc1] && (VS[fc2] || FG[fc2])) P[fc1] |= BIT3;
        }

        for (i = 0; i < aantal_vtg_tgo; ++i)
        {
            count fc1 = vtg_tgo[i].fc1;    /* richting 1 */
            count fc2 = vtg_tgo[i].fc2;    /* richting 2 */
            count hnla12 = vtg_tgo[i].hnla12; /* drukknop melding koppeling vanaf fc1 aanwezig */
            count hnla21 = vtg_tgo[i].hnla21; /* drukknop melding koppeling vanaf fc2 aanwezig */
            count hlos1 = vtg_tgo[i].hlos1;  /* los realiseren fc1 toegestaan */
            count hlos2 = vtg_tgo[i].hlos2;  /* los realiseren fc2 toegestaan */

            if (RV[fc1] && (IH[hnla12] || R[fc2] && A[fc2]) && tkcv(fc2)) RR[fc1] |= BIT3;
            if (RV[fc2] && (IH[hnla21] || R[fc1] && A[fc1]) && tkcv(fc1)) RR[fc2] |= BIT3;

            if (RA[fc1] && RA[fc2])
            {
                if (P[fc1] && IH[hnla12]) P[fc2] |= BIT3;
                if (P[fc2] && IH[hnla21]) P[fc1] |= BIT3;

                if (REALtraffick[fc1] > 0) X[fc1] |= BIT3;
                if (REALtraffick[fc2] > 0) X[fc2] |= BIT3;

                if (IH[hnla12] && !IH[hnla21] && (K[fc1] || (TMP_ontruim(fc1) > 0) || (X[fc1] & BIT3))) X[fc2] |= BIT3;
                if (IH[hnla21] && !IH[hnla12] && (K[fc2] || (TMP_ontruim(fc2) > 0) || (X[fc2] & BIT3))) X[fc1] |= BIT3;

                if ((IH[hnla12] && IH[hnla21] || R[fc1] && A[fc1] && IH[hlos1] && R[fc2] && A[fc2] && IH[hlos2]) &&
                    (K[fc1] || K[fc2] || (TMP_ontruim(fc1) > 0) || (TMP_ontruim(fc2) > 0)))
                {
                    X[fc1] |= BIT3;
                    X[fc2] |= BIT3;
                }
            }

            if (R[fc1] && R[fc2] && (IH[hnla12] || IH[hnla21]))
            {
                if (P[fc1] & BIT3) P[fc2] |= BIT3;
                if (P[fc2] & BIT3) P[fc1] |= BIT3;

                if (RR[fc1] & BIT3) RR[fc2] |= BIT3;
                if (RR[fc2] & BIT3) RR[fc1] |= BIT3;
            }
        }

        for (i = 0; i < aantal_lvk_gst; ++i)
        {
            count fc1 = lvk_gst[i].fc1;     /* richting 1 */
            count fc2 = lvk_gst[i].fc2;     /* richting 2 */
            count fc3 = lvk_gst[i].fc3;     /* richting 3 */
            count fc4 = lvk_gst[i].fc4;     /* richting 4 */

            bool uitstel = FALSE;
            bool privilege = FALSE;
            bool retourrood = FALSE;

            /* nooit meer retourrood als richting wellicht meeaanvraag verstrekt heeft terwijl eigen aanvraag is kwijtgeraakt */
            if ((fc1 != NG) && RA[fc1] && !K[fc1] && (RR[fc1] & BIT9)) P[fc1] |= BIT3;
            if ((fc2 != NG) && RA[fc2] && !K[fc2] && (RR[fc2] & BIT9)) P[fc2] |= BIT3;
            if ((fc3 != NG) && RA[fc3] && !K[fc3] && (RR[fc3] & BIT9)) P[fc3] |= BIT3;
            if ((fc4 != NG) && RA[fc4] && !K[fc4] && (RR[fc4] & BIT9)) P[fc4] |= BIT3;

            if (fc1 != NG)
            {
                if (GL[fc1] || TRG[fc1] || RV[fc1] && A[fc1] && tkcv(fc1)) retourrood = TRUE;
                if (GL[fc1] || TRG[fc1] || RV[fc1] && A[fc1] ||
                    RA[fc1] && (P[fc1] || !RR[fc1]) && (K[fc1] || (TMP_ontruim(fc1) > 0) || X[fc1])) uitstel = TRUE;
                if (RA[fc1] && P[fc1]) privilege = TRUE;
            }
            if (fc2 != NG)
            {
                if (GL[fc2] || TRG[fc2] || RV[fc2] && A[fc2] && tkcv(fc2)) retourrood = TRUE;
                if (GL[fc2] || TRG[fc2] || RV[fc2] && A[fc2] ||
                    RA[fc2] && (P[fc2] || !RR[fc2]) && (K[fc2] || (TMP_ontruim(fc2) > 0) || X[fc2])) uitstel = TRUE;
                if (RA[fc2] && P[fc2]) privilege = TRUE;
            }
            if (fc3 != NG)
            {
                if (GL[fc3] || TRG[fc3] || RV[fc3] && A[fc3] && tkcv(fc3)) retourrood = TRUE;
                if (GL[fc3] || TRG[fc3] || RV[fc3] && A[fc3] ||
                    RA[fc3] && (P[fc3] || !RR[fc3]) && (K[fc3] || (TMP_ontruim(fc3) > 0) || X[fc3])) uitstel = TRUE;
                if (RA[fc3] && P[fc3]) privilege = TRUE;
            }
            if (fc4 != NG)
            {
                if (GL[fc4] || TRG[fc4] || RV[fc4] && A[fc4] && tkcv(fc4)) retourrood = TRUE;
                if (GL[fc4] || TRG[fc4] || RV[fc4] && A[fc4] ||
                    RA[fc4] && (P[fc4] || !RR[fc4]) && (K[fc4] || (TMP_ontruim(fc4) > 0) || X[fc4])) uitstel = TRUE;
                if (RA[fc4] && P[fc4]) privilege = TRUE;
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

            if (retourrood)
            {
                if ((fc1 != NG) && R[fc1]) RR[fc1] |= BIT3;
                if ((fc2 != NG) && R[fc2]) RR[fc2] |= BIT3;
                if ((fc3 != NG) && R[fc3]) RR[fc3] |= BIT3;
                if ((fc4 != NG) && R[fc4]) RR[fc4] |= BIT3;
            }
        }

        for (i = 0; i < aantal_dcf_gst; ++i)
        {
            count fc1 = dcf_gst[i].fc1;   /* richting 1 */
            count fc2 = dcf_gst[i].fc2;   /* richting 2 */
            count ma12 = dcf_gst[i].ma12;  /* meerealisatie van fc1 met fc2 */
            count ma21 = dcf_gst[i].ma21;  /* meerealisatie van fc2 met fc1 */

            bool fc1_aanvr = FALSE;
            bool fc2_aanvr = FALSE;

            bool meeaanv1 = FALSE;
            bool meeaanv2 = FALSE;

            if ((ma12 != NG) && SCH[ma12]) meeaanv1 = TRUE;
            if ((ma21 != NG) && SCH[ma21]) meeaanv2 = TRUE;

            fc1_aanvr = GL[fc1] || TRG[fc1] || R[fc1] && (A[fc1] || meeaanv1);
            fc2_aanvr = GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || meeaanv2);

            if (G[fc1] && (!MG[fc1] || (YM[fc1] & PRIO_YM_BIT))) RR[fc2] |= BIT3;
            if (G[fc2] && (!MG[fc2] || (YM[fc2] & PRIO_YM_BIT))) RR[fc1] |= BIT3;

            if (RA[fc1] && P[fc1] && !P[fc2]) P[fc2] |= BIT3;
            if (RA[fc2] && P[fc2] && !P[fc1]) P[fc1] |= BIT3;

            if (!G[fc1] && fc1_aanvr && !G[fc2] && fc2_aanvr)
            {
                if (!RA[fc1] || K[fc1] || (TMP_ontruim(fc1) > 0) || X[fc1] ||
                    !RA[fc2] || K[fc2] || (TMP_ontruim(fc2) > 0) || X[fc2])
                {
                    X[fc1] |= BIT3;
                    X[fc2] |= BIT3;
                }
            }
        }

        aantal_x3 = 0;
        for (i = 0; i < FCMAX; ++i)
        {
            if (X[i] & BIT3) aantal_x3++;
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
/* Functie bepaal periode vooruit realiseren                                                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie corrigeert de periode waarin vooruit realiseren is toegestaan. Dit is nodig als MLMAX hoger */
/* is dan het actueel aantal modulen. De functie overschrijft PFPR[] zoals bepaald door TLCGen.             */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit VersneldPrimair_Add().                                                  */
/*                                                                                                          */
void Traffick2TLCgen_PFPR(void)       /* Fik230101                                                          */
{
    count fc, ml;

    if (MLMAX > ML_ACT_MAX)             /* MLMAX is groter dan het actueel aantal modulen                     */
    {
        count MLdelta = MLMAX - ML_ACT_MAX;

        for (fc = 0; fc < FCMAX; ++fc)
        {                                 /* als meerdere realisaties dan alleen correctie laatste realisatie   */
            for (ml = MLMAX - 1; ml >= 0; --ml)
            {
                if ((PRML[ml][fc] == PRIMAIR) && (ML > ml))
                {
                    PFPR[fc] = ml_fpr(fc, (PFPRtraffick[fc] + MLdelta), PRML, ML, MLMAX);
                    break;                      /* PFPR[] gecorrigeerd break zodat volgende fc behandeld gaat worden  */
                }
            }
        }
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
void Traffick2TLCgen_PAR(void)        /* Fik230301                                                          */
{
    count i, k;
    bool PAR_verplicht[FCMAX];         /* richting moet PAR[] krijgen als gevolg van koppeling */

    for (i = 0; i < FCMAX; ++i)         /* initialiseer PAR[] en PAR_verplicht[] */
    {
        PAR[i] = R[i] && ART[i] && (AltRuimte[i] > AR_max[i]) && !tfkaa(i) && !tkcv(i);

        if (ARB[i] > NG)                  /* reset PAR[] als blok actief waarin alternatief niet is toegestaan */
        {
            if (!(ARB[i] & (1 << ML))) PAR[i] = FALSE;
        }

        PAR_verplicht[i] = FALSE;
    }

    for (i = 0; i < aantal_hki_kop; ++i)
    {
        if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
        {
            count fc1 = hki_kop[i].fc1;      /* voedende richting */
            count fc2 = hki_kop[i].fc2;      /* volg     richting */
            count tlr21 = hki_kop[i].tlr21;    /* late release fc2 (= inrijtijd) */
            count tnlfg12 = hki_kop[i].tnlfg12;  /* vaste   nalooptijd vanaf einde vastgroen      fc1 */
            count tnlfgd12 = hki_kop[i].tnlfgd12; /* det.afh.nalooptijd vanaf einde vastgroen      fc1 */
            count tnleg12 = hki_kop[i].tnleg12;  /* vaste   nalooptijd vanaf einde (verleng)groen fc1 */
            count tnlegd12 = hki_kop[i].tnlegd12; /* det.afh.nalooptijd vanaf einde (verleng)groen fc1 */
            bool kop_eg = hki_kop[i].kop_eg;   /* koppeling vanaf einde groen (als FALSE dan vanaf EV) */
            bool los_fc2 = hki_kop[i].los_fc2;  /* fc2 mag bij aanvraag fc1 los realiseren */
            mulv  kop_max = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */
            mulv  status = hki_kop[i].status;   /* status koppeling */

            mulv  tijd_tot_sg = 0;
            mulv  naloop_tijd = 0;

            mulv             inrijfc1 = 0;
            if (tlr21 != NG) inrijfc1 = T_max[tlr21];

            if (PAR[fc1] && (G[fc2] || PAR[fc2]))
            {
                tijd_tot_sg = MTG[fc1];       /* bepaal start groen moment voedende richting */
                if (MTG[fc2] - inrijfc1 > tijd_tot_sg) tijd_tot_sg = MTG[fc2] - inrijfc1;

                naloop_tijd = 0;              /* controleer of naloop vanaf einde vastgroen past */
                if (tnlfg12 != NG) naloop_tijd = T_max[tnlfg12];
                if ((tnlfgd12 != NG) && (naloop_tijd < T_max[tnlfgd12])) naloop_tijd = T_max[tnlfgd12];
                if (MTG[fc2] + AltRuimte[fc2] <= tijd_tot_sg + TFG_max[fc1] + naloop_tijd + kop_max) PAR[fc1] = FALSE;

                naloop_tijd = 0;              /* controleer of naloop vanaf einde (verleng)groen past */
                if (tnleg12 != NG) naloop_tijd = T_max[tnleg12];
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
                if (G[fc1] || (status == 1)) PAR[fc2] = PAR_verplicht[fc2] = TRUE;
                if (tnlfg12 != NG) { if (RT[tnlfg12] || T[tnlfg12]) PAR[fc2] = PAR_verplicht[fc2] = TRUE; }
                if (tnlfgd12 != NG) { if (RT[tnlfgd12] || T[tnlfgd12]) PAR[fc2] = PAR_verplicht[fc2] = TRUE; }
                if (tnleg12 != NG) { if (RT[tnleg12] || T[tnleg12]) PAR[fc2] = PAR_verplicht[fc2] = TRUE; }
                if (tnlegd12 != NG) { if (RT[tnlegd12] || T[tnlegd12]) PAR[fc2] = PAR_verplicht[fc2] = TRUE; }
            }
        }
    }

    for (i = 0; i < aantal_vtg_tgo; ++i)
    {
        count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
        count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
        count tinl12 = vtg_tgo[i].tinl12;   /* inlooptijd fc1 */
        count tinl21 = vtg_tgo[i].tinl21;   /* inlooptijd fc2 */
        count tnlsgd12 = vtg_tgo[i].tnlsgd12; /* nalooptijd fc2 vanaf startgroen fc1 */
        count tnlsgd21 = vtg_tgo[i].tnlsgd21; /* nalooptijd fc1 vanaf startgroen fc2 */
        count hnla12 = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
        count hnla21 = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
        mulv  status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
        mulv  status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

        mulv  tijd_tot_sg = 0;

        mulv  inloop12 = 0;
        mulv  inloop21 = 0;

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

        /* volgrichting moet altijd mee realiseren */
        if (R[fc2] && (RT[tnlsgd12] || T[tnlsgd12] || (status12 == 1))) PAR[fc2] = PAR_verplicht[fc2] = TRUE;
        if (R[fc1] && (RT[tnlsgd21] || T[tnlsgd21] || (status21 == 1))) PAR[fc1] = PAR_verplicht[fc1] = TRUE;

        if (RA[fc1] && P[fc1] && IH[hnla12]) PAR[fc2] = PAR_verplicht[fc2] = TRUE;
        if (RA[fc2] && P[fc2] && IH[hnla21]) PAR[fc1] = PAR_verplicht[fc1] = TRUE;

        if (!IH[hnla12] && !IH[hnla21])   /* toevoeging Fik230701 - beide voetgangers aangevraagd dan altijd samen realiseren       */
        {
            if (R[fc1] && A[fc1] && R[fc2] && A[fc2])
            {
                if (!PAR[fc1] || !PAR[fc2]) PAR[fc1] = PAR[fc2] = FALSE;
            }
        }
    }

    for (i = 0; i < aantal_dcf_vst; ++i)
    {
        count fc1 = dcf_vst[i].fc1;     /* richting die voorstart geeft  */
        count fc2 = dcf_vst[i].fc2;     /* richting die voorstart krijgt */
        count tvs21 = dcf_vst[i].tvs21;   /* voorstart fc2 */
        count ma21 = dcf_vst[i].ma21;    /* meerealisatie van fc2 met fc1 */

        bool fc2_eerst = GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || (ma21 != NG) && SCH[ma21]);
        mulv  tijd_tot_sg = MTG[fc1];     /* bepaal start groen moment richting 1 */

        if (fc2_eerst && (MTG[fc2] + T_max[tvs21] > tijd_tot_sg)) tijd_tot_sg = MTG[fc2] + T_max[tvs21];
        if (G[fc2] && (T_max[tvs21] - TG_timer[fc2] > tijd_tot_sg)) tijd_tot_sg = T_max[tvs21] - TG_timer[fc2];

        if (fc2_eerst && !PAR[fc2] || (MTG[fc1] + AltRuimte[fc1] <= tijd_tot_sg + AR_max[fc1]))
        {
            if (!G[fc1]) PAR[fc1] = FALSE;
        }
    }

    for (i = 0; i < aantal_dcf_gst; ++i)
    {
        count fc1 = dcf_gst[i].fc1;     /* richting 1  */
        count fc2 = dcf_gst[i].fc2;     /* richting 2 */
        count ma12 = dcf_gst[i].ma12;    /* meerealisatie van fc1 met fc2 */
        count ma21 = dcf_gst[i].ma21;    /* meerealisatie van fc2 met fc1 */

        bool fc1_aanvr = GL[fc1] || TRG[fc1] || R[fc1] && (A[fc1] || (ma12 != NG) && SCH[ma12]);
        bool fc2_aanvr = GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || (ma21 != NG) && SCH[ma21]);

        if (fc1_aanvr && fc2_aanvr && (!PAR[fc1] || !PAR[fc2])) PAR[fc1] = PAR[fc2] = FALSE;
    }

    for (i = 0; i < aantal_dcf_vst; ++i)
    {
        count fc1 = dcf_vst[i].fc1;       /* richting die voorstart geeft  */
        count fc2 = dcf_vst[i].fc2;       /* richting die voorstart krijgt */

        if (RA[fc1] && (VS[fc2] || FG[fc2])) PAR[fc1] = PAR_verplicht[fc1] = TRUE;
    }

    for (i = 0; i < aantal_lvk_gst; ++i)
    {
        count fc1 = lvk_gst[i].fc1;       /* richting 1 */
        count fc2 = lvk_gst[i].fc2;       /* richting 2 */
        count fc3 = lvk_gst[i].fc3;       /* richting 3 */
        count fc4 = lvk_gst[i].fc4;       /* richting 4 */

        bool toestemming = TRUE;
        bool realiseer = FALSE;

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
            if (fc1 != NG) { if (R[fc1]) PAR[fc1] = PAR_verplicht[fc1] = TRUE; }
            if (fc2 != NG) { if (R[fc2]) PAR[fc2] = PAR_verplicht[fc2] = TRUE; }
            if (fc3 != NG) { if (R[fc3]) PAR[fc3] = PAR_verplicht[fc3] = TRUE; }
            if (fc4 != NG) { if (R[fc4]) PAR[fc4] = PAR_verplicht[fc4] = TRUE; }
        }
    }

    for (i = 0; i < FCMAX; ++i)         /* corrigeer PAR[] op basis van gewogen wachttijd */
    {
        if (!PAR_verplicht[i])            /* als PAR[] niet verplicht dan is reset PAR[] toegestaan */
        {
            for (k = 0; k < FCMAX; ++k)
            {
                if ((GK_conflict(i, k) > NG) || FK_conflict(i, k))
                {
                    if (PAR[k] && (GWT[k] > GWT[i]) && !RA[i]) PAR[i] = FALSE;
                }
            }
        }
        else                             /* als PAR[] verplicht dan zonodig opzetten */
        {
            PAR[i] = TRUE;
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
    count i, k;
    bool fc_eerst[FCMAX];              /* richting is als eerstvolgende aan de beurt */

    for (i = 0; i < FCMAX; ++i)
    {
        RR[i] &= ~BIT5;                   /* reset BIT sturing */
        FM[i] &= ~BIT5;
        fc_eerst[i] = FALSE;              /* initialiseer hulp variabelen */
    }

    for (i = 0; i < FCMAX; ++i)
    {
        if (AR[i] && R[i] && !PAR[i]) RR[i] |= BIT5;
        if (AR[i] && VG[i] && (TEG[i] == 0) && (AltRuimte[i] <= 0)) FM[i] |= BIT5;
    }

    for (i = 0; i < FCMAX; ++i)         /* bepaal welke richtingen als eerst volgende aan de beurt zijn */
    {
        fc_eerst[i] = R[i] && A[i] && AAPR[i] && !(AAPR[i] & BIT5) && !fkra(i) && !(RR[i] & BIT6) && !(RR[i] & BIT10) || RA[i];
    }

    for (i = 0; i < aantal_hki_kop; ++i)
    {
        if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
        {
            count fc1 = hki_kop[i].fc1;    /* voedende richting */
            count fc2 = hki_kop[i].fc2;    /* volg     richting */
            count status = hki_kop[i].status; /* status koppeling */

            if (!G[fc2] && fc_eerst[fc1] || (status == 1)) fc_eerst[fc2] = TRUE;
        }
    }

    for (i = 0; i < aantal_dcf_vst; ++i)
    {
        count fc1 = dcf_vst[i].fc1;      /* richting die voorstart geeft  */
        count fc2 = dcf_vst[i].fc2;      /* richting die voorstart krijgt */
        count ma21 = dcf_vst[i].ma21;     /* meerealisatie van fc2 met fc1 */

        bool fc2_aanvr = GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || (ma21 != NG) && SCH[ma21]);

        if (fc2_aanvr && fc_eerst[fc1]) fc_eerst[fc2] = TRUE;
    }

    for (i = 0; i < aantal_dcf_gst; ++i)
    {
        count fc1 = dcf_gst[i].fc1;      /* richting 1 */
        count fc2 = dcf_gst[i].fc2;      /* richting 2 */
        count ma12 = dcf_gst[i].ma12;     /* meerealisatie van fc1 met fc2 */
        count ma21 = dcf_gst[i].ma21;     /* meerealisatie van fc2 met fc1 */

        bool fc1_aanvr = GL[fc1] || TRG[fc1] || R[fc1] && (A[fc1] || (ma12 != NG) && SCH[ma12]);
        bool fc2_aanvr = GL[fc2] || TRG[fc2] || R[fc2] && (A[fc2] || (ma21 != NG) && SCH[ma21]);

        if (fc1_aanvr && fc_eerst[fc2]) fc_eerst[fc1] = TRUE;
        if (fc2_aanvr && fc_eerst[fc1]) fc_eerst[fc2] = TRUE;
    }

    for (i = 0; i < aantal_hki_kop; ++i)
    {
        if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
        {
            count fc1 = hki_kop[i].fc1;      /* voedende richting */
            count fc2 = hki_kop[i].fc2;      /* volg     richting */
            count tnlfg12 = hki_kop[i].tnlfg12;  /* vaste   nalooptijd vanaf einde vastgroen      fc1 */
            count tnlfgd12 = hki_kop[i].tnlfgd12; /* det.afh.nalooptijd vanaf einde vastgroen      fc1 */
            count tnleg12 = hki_kop[i].tnleg12;  /* vaste   nalooptijd vanaf einde (verleng)groen fc1 */
            count tnlegd12 = hki_kop[i].tnlegd12; /* det.afh.nalooptijd vanaf einde (verleng)groen fc1 */
            mulv  kop_max = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */

            if (fc_eerst[fc1])              /* voedende richting harde koppeling is aan de beurt, ga op zoek naar conflicten van de volgrichting die alternatief in VG[] staan */
            {
                for (k = 0; k < FCMAX; ++k)
                {
                    mulv ontruim = GK_conflict(k, fc2);
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
                if ((tnlfg12 != NG)) naloop_tijd = T_max[tnlfg12] - T_timer[tnlfg12];
                if ((tnlfgd12 != NG) && (naloop_tijd < T_max[tnlfgd12] - T_timer[tnlfgd12])) naloop_tijd = T_max[tnlfgd12] - T_timer[tnlfgd12];
                if ((tnleg12 != NG) && (naloop_tijd < T_max[tnleg12])) naloop_tijd = T_max[tnleg12];
                if ((tnlegd12 != NG) && (naloop_tijd < T_max[tnlegd12] + TGL_max[fc1])) naloop_tijd = T_max[tnlegd12] + TGL_max[fc1];

                naloop_tijd += kop_max;

                for (k = 0; k < FCMAX; ++k)
                {
                    mulv ontruim = GK_conflict(fc2, k);
                    if (ontruim > NG)
                    {
                        if (fc_eerst[k])          /* conflict volgrichting gevonden die aan de beurt is */
                        {
                            if (REALtraffick[k] <= ontruim + naloop_tijd) FM[fc1] |= BIT5;
                        }
                    }
                    if (FM[fc1] & BIT5) break;
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
void BugFix_RR_bij_HKI(void)          /* Fik230101                                                          */
{
#ifdef PRIO_ADDFILE
    count i;

    for (i = 0; i < aantal_hki_kop; ++i)
    {
        if (hki_kop[i].status > NG)       /* koppeling is ingeschakeld */
        {
            count fc1 = hki_kop[i].fc1;     /* voedende richting */
            count fc2 = hki_kop[i].fc2;     /* volg     richting */

            if (!(RR[fc2] & PRIO_RR_BIT) && !REALconflict(fc1)) RR[fc1] &= ~BIT10;
        }
    }
#endif
}


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
        count fc1 = dcf_vst[i].fc1;     /* richting die voorstart geeft  */
        count fc2 = dcf_vst[i].fc2;     /* richting die voorstart krijgt */
        count tvs21 = dcf_vst[i].tvs21;   /* voorstart fc2 */
        count ma21 = dcf_vst[i].ma21;    /* meerealisatie van fc2 met fc1 */

        bool meeaanv2 = FALSE;
        if ((ma21 != NG) && SCH[ma21]) meeaanv2 = TRUE;

        BL[fc2] &= ~BIT4;                 /* overslag door fc1 agv afteller */
        if (SG[fc1] && (P[fc1] & BIT4) && (TA_timer[fc2] < T_max[tvs21] + 10)) BL[fc2] |= BIT4;

        if (RA[fc1] && (X[fc1] & BIT1) && !(X[fc1] & BIT3)) X[fc1] &= ~BIT1;
        if (RA[fc1] && A[fc1] && (!RR[fc1] || P[fc1]) && !BL[fc1] && (X[fc1] & BIT3) && R[fc2] && !TRG[fc2] && (A[fc2] || meeaanv2))
        {
            if (meeaanv2) A[fc2] |= BIT4;   /* zet mee aanvraag vroeg op */
            RR[fc2] = FALSE;
            X[fc2] &= ~BIT1;
            if (RV[fc2] && !AA[fc2]) AA[fc2] = AR[fc2] = TRUE;
        }
    }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie forceer realisatie van richtingen met gelijkstart                                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie forceert een richting met een gelijkstart naar groen indien daarop gewacht wordt.           */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_REA().                                                  */
/*                                                                                                          */
void Traffick2TLCgen_REA_GST(void)    /* Fik240101                                                          */
{
    count i;

    for (i = 0; i < aantal_dcf_gst; ++i)
    {
        count fc1 = dcf_gst[i].fc1;     /* richting 1 */
        count fc2 = dcf_gst[i].fc2;     /* richting 2 */
        count ma12 = dcf_gst[i].ma12;    /* meerealisatie van fc1 met fc2 */
        count ma21 = dcf_gst[i].ma21;    /* meerealisatie van fc2 met fc1 */

        bool meeaanv1 = FALSE;
        bool meeaanv2 = FALSE;

        if ((ma12 != NG) && SCH[ma12]) meeaanv1 = TRUE;
        if ((ma21 != NG) && SCH[ma21]) meeaanv2 = TRUE;

        if (RA[fc1] && (X[fc1] & BIT2) && !(X[fc1] & BIT3)) X[fc1] &= ~BIT2;
        if (RA[fc2] && (X[fc2] & BIT2) && !(X[fc2] & BIT3)) X[fc2] &= ~BIT2;

        if (RA[fc1] && A[fc1] && (!RR[fc1] || P[fc1]) && !BL[fc1] && (X[fc1] & BIT3) && R[fc2] && !TRG[fc2] && (A[fc2] || meeaanv2))
        {
            if (meeaanv2) A[fc2] |= BIT4;
            RR[fc2] = FALSE;
            X[fc2] &= ~BIT2;
            if (RV[fc2] && !AA[fc2]) AA[fc2] = AR[fc2] = TRUE;
        }

        if (RA[fc2] && A[fc2] && (!RR[fc2] || P[fc2]) && !BL[fc2] && (X[fc2] & BIT3) && R[fc1] && !TRG[fc1] && (A[fc1] || meeaanv1))
        {
            if (meeaanv1) A[fc1] |= BIT4;
            RR[fc1] = FALSE;
            X[fc1] &= ~BIT2;
            if (RV[fc1] && !AA[fc1]) AA[fc1] = AR[fc1] = TRUE;
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
    count i, k;

    Traffick2TLCgen_REA_VST();
    Traffick2TLCgen_REA_GST();

    for (i = 0; i < FCMAX; ++i)
    {
        if (RA[i] && !A[i])
        {
            RR[i] |= BIT3;
            TFB_timer[i] = 0;
        }
        if (RA[i] && tkcv(i)) RR[i] |= BIT3;
        if (RV[i] && tkcv(i)) AA[i] = FALSE;
        if (RA[i] && AR[i]) AG[i] = AR[i]; /* voorkom vasthouden module als alle richtingen gerealiseerd zijn */
    }

    for (i = 0; i < aantal_pel_kop; ++i)
    {
        count kop_fc = pel_kop[i].kop_fc;   /* koppelrichting */

        if (MG[kop_fc] && (YM[kop_fc] & BIT12))
        {
            for (k = 0; k < FCMAX; ++k)
            {
                if (FK_conflict(kop_fc, k))
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
            count fc1 = hki_kop[i].fc1;    /* voedende richting */
            count fc2 = hki_kop[i].fc2;    /* volg     richting */
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
        count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
        count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
        mulv  status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
        mulv  status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

        if ((RA[fc1] && P[fc1] && (X[fc1] & BIT3) || G[fc1] && (status12 == 1)) && R[fc2] && !TRG[fc2] && A[fc2] && !tkcv(fc2) && !tfkaa(fc2))
        {
            RR[fc2] = FALSE;
            if (RV[fc2] && !AA[fc2]) AA[fc2] = AR[fc2] = TRUE;
        }

        if ((RA[fc2] && P[fc2] && (X[fc2] & BIT3) || G[fc2] && (status21 == 1)) && R[fc1] && !TRG[fc1] && A[fc1] && !tkcv(fc1) && !tfkaa(fc1))
        {
            RR[fc1] = FALSE;
            if (RV[fc1] && !AA[fc1]) AA[fc1] = AR[fc1] = TRUE;
        }

        if (RA[fc1] && (P[fc1] & BIT2)) X[fc1] &= ~BIT1; /* overrule TLCgen */
        if (RA[fc2] && (P[fc2] & BIT2)) X[fc2] &= ~BIT1; /* volg richting moet altijd voeding volgen */
    }

    for (i = 0; i < aantal_lvk_gst; ++i)
    {
        count fc1 = lvk_gst[i].fc1;       /* richting 1 */
        count fc2 = lvk_gst[i].fc2;       /* richting 2 */
        count fc3 = lvk_gst[i].fc3;       /* richting 3 */
        count fc4 = lvk_gst[i].fc4;       /* richting 4 */

        bool realiseer = FALSE;
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

    for (i = 0; i < aantal_mee_rea; ++i)
    {
        count fc1 = mee_rea[i].fc1;      /* richting die meerealisatie geeft  */
        count fc2 = mee_rea[i].fc2;      /* richting die meerealisatie krijgt */
        count ma21 = mee_rea[i].ma21;     /* meerealisatie van fc2 met fc1 */
        count mr2v = mee_rea[i].mr2v;     /* meerealisatie aan fc2 verstrekt */

        if (G[fc2]) mr2v = FALSE;
        if ((ma21 != NG) && (SG[fc1] || FG[fc1] || EFG[fc1]))
        {
            if (R[fc2] && !TRG[fc2] && !K[fc2] && (TMP_ontruim(fc2) == 0) && !tkcv(fc2) && !tfkaa(fc2))
            {
                if (mr2v || SCH[ma21] && PAR[fc2] && !RR[fc2] && !BL[fc2] && !DOSEER[fc2] && !EFG[fc1])
                {
                    if (!AA[fc2]) AA[fc2] = AR[fc2] = mr2v = TRUE;
                    A[fc2] |= BIT4;
                    P[fc2] |= BIT3;
                    RR[fc2] = BL[fc2] = FALSE;
                }
            }
        }
        else mr2v = FALSE;

        mee_rea[i].mr2v = mr2v;           /* terugschrijven "meerealisatie fc2 verstrekt" naar struct */
    }

    for (i = 0; i < FCMAX; ++i)
    {
        TVG_instelling[i] = TVG_max[i];   /* buffer TVG_max[] voor behandeling maatregelen bij file en detectie storing */

        if (RV[i] && !TRG[i] && A[i] && PAR[i] && !AA[i] && !RR[i] && !BL[i] && !tkcv(i) && !tfkaa(i))
        {
            bool langstwachtend = TRUE;

            for (k = 0; k < FCMAX; ++k)
            {
                if (FK_conflict(i, k))
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
/* Functie reset doseer bits ten behoeve van file stroomafwaarts                                            */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie reset de doseer bits omdat deze door verschillende file ingrepen kunnen worden opgezet.     */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit FileVerwerking_Add().                                                   */
/*                                                                                                          */
void traffick_file_nass_reset(void)   /* Fik230701                                                          */
{
    count fc;

    for (fc = 0; fc < FCMAX; ++fc)
    {
        X[fc] &= ~BIT5;
        Z[fc] &= ~BIT5;
        BL[fc] &= ~BIT5;
        RR[fc] &= ~BIT12;              /* RR[] toegevoegd 12-05-2023 Jol                                     */
        DOSEER[fc] = FALSE;
        DOSMAX[fc] = NG;
        DOS_RD[fc] = 0;
    }
}


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
    count t_min_rood)                     /* T   minimum roodduur  tijdens file stroomafwaarts                  */
{
    mulv  doseer_maximum = NG;
    bool toepassen_doseer_maximum = FALSE;

    TVG_max[fc] = TVG_instelling[fc];   /* corrigeer instelling TVG_max[]                                     */

    if ((h_file != NG) && IH[h_file] && (sch_ingreep != NG) && SCH[sch_ingreep])
    {
        DOSEER[fc] = TRUE;                /* DOSEER[] verzorgt het blokkeren van wachtstandgroen, meeverlengen  */
        /* ... en het blokkeren van prioriteits- en alternatieve realisaties  */
        if (t_max_groen != NG)
        {
            doseer_maximum = T_max[t_max_groen];
        }

        if (sch_perc_gr == -2) toepassen_doseer_maximum = TRUE; /* ALTIJD = -2 */
        else
        {
            if ((sch_perc_gr != NG) && SCH[sch_perc_gr]) toepassen_doseer_maximum = TRUE;
        }

        if (toepassen_doseer_maximum)                       /* toepassen doseer maximum                         */
        {
            if (prm_perc_gr != NG)
            {
                mulv percentage = prm_perc_gr;
                mulv groentijd = NG;

                if (percentage > 100) percentage = 100;         /* bij doseren is de maximum instelling 100%        */

                if (max_verleng_groen)                          /* er worden maximum verlenggroentijden toegepast   */
                {
                    groentijd = TFG_max[fc] + (mulv)(((long)percentage * (long)TVG_max[fc]) / 100);
                }
                else                                            /* er worden maximum groentijden toegepast          */
                {
                    groentijd = (mulv)(((long)percentage * (long)TVG_max[fc]) / 100);
                }

                if ((doseer_maximum == NG) || (groentijd < doseer_maximum))
                {
                    doseer_maximum = groentijd;
                }
            }
        }

        if ((h_afkap_start != NG) && (t_afk_sfile != NG))   /* ondergrens voor afbreken bij start file          */
        {
            if (IH[h_afkap_start] && (T_max[t_afk_sfile] > doseer_maximum))
            {
                doseer_maximum = T_max[t_afk_sfile];
            }
        }

        if (doseer_maximum > NG)
        {
            if (G[fc] && (TG_timer[fc] >= doseer_maximum)) MK[fc] = FALSE;
            if ((DOSMAX[fc] == NG) || (DOSMAX[fc] >= doseer_maximum)) DOSMAX[fc] = doseer_maximum;
        }

        if (t_min_rood > NG)
        {
            if (T_max[t_min_rood] == 999) BL[fc] |= BIT5;    /* instelling 999 betekent blokkeren                 */
            else                                             /* bugfix bepaal DOS_RD[] en RR[] toegevoegd         */
            {
                if (R[fc] && (TR_timer[fc] < T_max[t_min_rood]))
                {
                    X[fc] |= BIT5;
                    RR[fc] |= BIT12;
                    if ((T_max[t_min_rood] - TR_timer[fc]) > DOS_RD[fc]) DOS_RD[fc] = T_max[t_min_rood] - TR_timer[fc];
                }
            }
        }
    }
}


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
    count pstar)                          /* PRM percentage star uitverlengen                                   */
{

    bool reset_a = TRUE;               /* reset A[]BIT11 toegestaan */

#ifndef NO_TIMETOX                    /* controleer of TLCGen A[]BIT11 al eerder heeft opgezet */
#ifdef  schconfidence15fix
    if (SCH[schconfidence15fix] && R[fc] && (P[fc] & BIT11)) reset_a = FALSE;
#endif
#endif

    MK[fc] &= ~BIT5;                    /* reset detectie storing bits */
    if (reset_a) A[fc] &= ~BIT11;

    TVG_max[fc] = TVG_instelling[fc];   /* zet juiste waarde TVG_max[] terug  */

    if (R[fc] && A_DST[fc] && (tvertraag != NG))
    {
        if (tvertraag == -2) A[fc] |= BIT11; /* ALTIJD = -2 */
        else
        {
            if ((T_max[tvertraag] > 0) && (TR_timer[fc] >= T_max[tvertraag])) A[fc] |= BIT11;
        }
    }

    if (G[fc] && MK_DST[fc] && (pstar > NG))
    {
        mulv star_groen = 0;
        mulv perc_groen = PRM[pstar];

        if (perc_groen > 100) perc_groen = 100;

        if (max_verleng_groen)
        {
            star_groen = TFG_max[fc] + (((TVG_max[fc] * 10) / 100) * perc_groen) / 10;
        }
        else
        {
            star_groen = ((((TFG_max[fc] + TVG_max[fc]) * 10) / 100) * perc_groen) / 10;
        }

        if (DOSMAX[fc] > NG)
        {
            if (G[fc] && (TG_timer[fc] < DOSMAX[fc]))
            {
                if (TG_timer[fc] < star_groen) MK[fc] |= BIT5;
            }
        }
        else
        {
            if (G[fc] && (TG_timer[fc] < star_groen)) MK[fc] |= BIT5;
        }
    }

}


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
    count us_ks)                          /* US    uitgaand koppelsignaal                                       */
{
    bool SD1 = FALSE;
    bool SD2 = FALSE;
    bool SD3 = FALSE;

    if (meting_actief)
    {
        SD1 = (de1 != NG) && SD[de1] && !DF[de1] && !G[fc];   /* bepaal uitgaande detectie pulsen */
        SD2 = (de2 != NG) && SD[de2] && !DF[de2] && !G[fc];
        SD3 = (de3 != NG) && SD[de3] && !DF[de3] && !G[fc];
    }

    if (!meting_actief)                 /* reset meting indien niet actief */
    {
        if (!G[fc]) PEL_UIT_VTG[fc] = 0;
    }
    else
    {
        if (G[fc] && !SG[fc]) PEL_UIT_VTG[fc] = 0;
        if (SD1 || SD2 || SD3)            /* tel voertuigen vanaf einde groen */
        {
            if (SD1 && PEL_UIT_VTG[fc] < 10000) PEL_UIT_VTG[fc] += 1;
            if (SD2 && PEL_UIT_VTG[fc] < 10000) PEL_UIT_VTG[fc] += 1;
            if (SD3 && PEL_UIT_VTG[fc] < 10000) PEL_UIT_VTG[fc] += 1;
        }
    }

    if (meting_actief && SG[fc] && (PRM[pgrenswaarde] > 0) && (PEL_UIT_VTG[fc] >= PRM[pgrenswaarde]))
    {
        PEL_UIT_VTG[fc] = 0;             /* peloton is gemeten */
        CIF_GUS[us_ks] = TRUE;          /* stuur koppelsignaal uit */
    }

    CIF_GUS[us_ks] = REG && G[fc] && CIF_GUS[us_ks] && ((TG_timer[fc] < 20) || duur_signaal && (VS[fc] || FG[fc] || MK[fc]));
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
    count tpeltdh)                        /* TM    grenshiaat  peloton meting                                   */
{
    bool ED1 = FALSE;
    bool ED2 = FALSE;
    bool ED3 = FALSE;

    if (meting_actief)
    {
        ED1 = (de1 != NG) && ED[de1] && !DF[de1] && G[fc];    /* bepaal uitgaande detectie pulsen */
        ED2 = (de2 != NG) && ED[de2] && !DF[de2] && G[fc];
        ED3 = (de3 != NG) && ED[de3] && !DF[de3] && G[fc];
    }

    AT[tpelmeet] = FALSE;               /* reset AT[] */
    AT[tpeltdh] = FALSE;               /* ... en start meetperiode bij vertrek van 1e voertuig */

    if (duur_signaal)
    {                                   /* SG[fc] verwijderd Fik240701 -> meetperiode starten bij afrijden 1e voertuig */
        RT[tpelmeet] = RT[tpeltdh] = /* SG[fc] || */ G[fc] && !T[tpelmeet] && !T[tpeltdh] && (ED1 || ED2 || ED3) && !CIF_GUS[us_ks];
    }
    else                                /* koppelsignaal is puls */
    {
        RT[tpelmeet] = RT[tpeltdh] = /* SG[fc] || */ G[fc] && !T[tpelmeet] && !T[tpeltdh] && (ED1 || ED2 || ED3);
    }

    if (!meting_actief)                 /* reset meting indien niet actief */
    {
        AT[tpelmeet] = AT[tpeltdh] = TRUE;
        RT[tpelmeet] = RT[tpeltdh] = FALSE;
        if (G[fc]) PEL_UIT_VTG[fc] = 0;
    }
    else
    {
        if (SG[fc]) PEL_UIT_VTG[fc] = 0;
        if ((ED1 || ED2 || ED3) && (RT[tpelmeet] || T[tpelmeet]))
        {
            if (ED1 && PEL_UIT_VTG[fc] < 10000) PEL_UIT_VTG[fc] += 1;
            if (ED2 && PEL_UIT_VTG[fc] < 10000) PEL_UIT_VTG[fc] += 1;
            if (ED3 && PEL_UIT_VTG[fc] < 10000) PEL_UIT_VTG[fc] += 1;
            RT[tpeltdh] = TRUE;             /* herstart grenshiaat er is immers een voertuig afgereden */
        }

        if (ET[tpeltdh] && !RT[tpeltdh])  /* reset meting omdat het grenshiaat is gevallen */
        {
            AT[tpelmeet] = TRUE;
            PEL_UIT_VTG[fc] = 0;
        }
    }

    if (PEL_UIT_RES[fc] > 0)            /* hou restant duur uitsturing koppelsignaal bij */
    {
        if (PEL_UIT_RES[fc] >= TE) PEL_UIT_RES[fc] -= TE;
        else                       PEL_UIT_RES[fc] = 0;
    }

    if (meting_actief && (PRM[pgrenswaarde] > 0) && (PEL_UIT_VTG[fc] >= PRM[pgrenswaarde]))
    {
        PEL_UIT_VTG[fc] = 0;             /* peloton is gemeten */
        PEL_UIT_RES[fc] = 20;            /* ... stuur koppelsignaal uit en reset de meting */
        AT[tpelmeet] = TRUE;
        AT[tpeltdh] = TRUE;
        RT[tpeltdh] = FALSE;         /* ook bij een duur signaal is de minimale uitsturing 2,0 sec. */
    }

    CIF_GUS[us_ks] = REG && ((PEL_UIT_RES[fc] > 0) || duur_signaal && CIF_GUS[us_ks] && G[fc] && (VS[fc] || FG[fc] || MK[fc]));
}


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
    count us_telaat)                      /* US 1e bus is te laat                                               */
{
    count prio_OV_kar_index = prio_index[fc].OV_kar;

    if ((prio_OV_kar_index > NG) && (ov_counter > NG) && (us_tevroeg > NG) && (us_optijd > NG) && (us_telaat > NG))
    {
        if (C[ov_counter])
        {
            if (CIF_GUS[us_tevroeg]) US_OV_old[fc] = 0;
            if (CIF_GUS[us_optijd])  US_OV_old[fc] = 1;
            if (CIF_GUS[us_telaat])  US_OV_old[fc] = 2;

            if (!CIF_GUS[us_tevroeg] && !CIF_GUS[us_optijd] && !CIF_GUS[us_telaat])
            {
                if (OV_stipt[fc] == BIT0) CIF_GUS[us_tevroeg] = TRUE;
                if (OV_stipt[fc] == BIT1) CIF_GUS[us_optijd] = TRUE;
                if (OV_stipt[fc] == BIT2) CIF_GUS[us_telaat] = TRUE;

                if (!CIF_GUS[us_tevroeg] && !CIF_GUS[us_optijd] && !CIF_GUS[us_telaat])
                {
                    if (US_OV_old[fc] == 0) CIF_GUS[us_tevroeg] = TRUE;
                    if (US_OV_old[fc] == 1) CIF_GUS[us_optijd] = TRUE;
                    if (US_OV_old[fc] == 2) CIF_GUS[us_telaat] = TRUE;
                    if (US_OV_old[fc] == NG) CIF_GUS[us_optijd] = TRUE;
                }
            }
        }
        else
        {
            US_OV_old[fc] = NG;
        }
        CIF_GUS[us_tevroeg] = REG && CIF_GUS[us_tevroeg] && (C[ov_counter] && KNIP || G[fc] && C[ov_counter] && iPrioriteit[prio_OV_kar_index]);
        CIF_GUS[us_optijd] = REG && CIF_GUS[us_optijd] && (C[ov_counter] && KNIP || G[fc] && C[ov_counter] && iPrioriteit[prio_OV_kar_index]);
        CIF_GUS[us_telaat] = REG && CIF_GUS[us_telaat] && (C[ov_counter] && KNIP || G[fc] && C[ov_counter] && iPrioriteit[prio_OV_kar_index]);
    }
    else
    {
        US_OV_old[fc] = NG;
    }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie test uitgangssignalen                                                                            */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie zet een uitgangssignaal op ten behoeve van test doeleinden.                                 */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_application().                                                     */
/*                                                                                                          */
void test_us_signalen(void)           /* Fik230101                                                          */
{
#ifdef prmtestus
    if (PRM[prmtestus] != 0)
    {
        if ((PRM[prmtestus] >= FCMAX) && (PRM[prmtestus] < USMAX))
        {
            CIF_GUS[PRM[prmtestus]] = TRUE;
        }
    }
#endif
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende OV ingreep tbv wachttijd voorspeller                      */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie geeft de aanwezigheid terug van een conflicterende busingreep tbv de wachttijd voorspeller. */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit aansturing_wt_voorspeller().                                            */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
bool conflict_OV(                    /* Fik230101                                                          */
    count fc)                             /* FC fasecyclus                                                      */
{
    count i, prio;

    for (i = 0; i < FCMAX; ++i)
    {
        if (GK_conflict(fc, i) > NG)
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
#endif


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
        bool ontruim = GK_conflict(fc, i);
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
    count mealed_uit)                     /* ME  aantal leds dat uitgestuurd wordt                              */
{
    bool halteer = FALSE;
    bool bus_sjb = FALSE;
    bool knipper = FALSE;
    mulv  wachttijd = wacht_ML[fc];
    bool bus_sjb_aanw = (usbus != NG);
    mulv  aantal_leds = 0;
    bool toestemming = TRUE;
    bool WT_REALtraffick = FALSE;

    if (wachttijd == -3)                /* primaire overslag geeft -3 terug */
    {
        wachttijd = 1200 - TA_timer[fc];
        if (wachttijd < 600) wachttijd = 600;
    }

#ifdef schsjabloon
    if (!SCH[schsjabloon]) bus_sjb_aanw = FALSE;
#endif

#ifdef PRIO_ADDFILE
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
#else
#ifdef WTV_KNIPPER
            halteer = TRUE;
#endif
#endif

        }
    }
#endif

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
#else
#ifdef WTV_KNIPPER
        knipper = TRUE;
#endif
#endif
    }

    if (schwtv != NG)
    {
        if (!SCH[schwtv] && (Aled[fc] == 0)) toestemming = FALSE;
    }

    if (hewtv != NG)
    {
        IH[hewtv] = toestemming;
    }

    WT_REALtraffick = (REALtraffick[fc] > NG) && (RA[fc] || !(AAPR[fc] & BIT5));

    if (WT_REALtraffick)
    {
        wachttijd = REALtraffick[fc];
    }
    else
    {
        if (REALconflictTTG(fc) > wachttijd) wachttijd = REALconflictTTG(fc);
    }

    /* bij bus in aantocht maar nog geen bus aanvraag conflicterende WTV halteren als wacht_ML[] == 0 */
    if ((RR[fc] & BIT6) && (wachttijd == 0)) halteer = TRUE;

    if (Aled[fc] > 0) AanDuurLed[fc] += TE;
    if (AanDuurLed[fc] > 600) AanDuurLed[fc] = 600;

    if ((Aled[fc] > 0) && (wachttijd > NG) && !halteer)
    {
        mulv rest = (wachttijd + AanDuurLed[fc]) % Aled[fc];
        TijdPerLed[fc] = (wachttijd + AanDuurLed[fc]) / Aled[fc];

        if (2 * rest >= Aled[fc]) TijdPerLed[fc]++;
        if (TijdPerLed[fc] < 1) TijdPerLed[fc] = 1;
        if (TijdPerLed[fc] > 600) TijdPerLed[fc] = 600;

        if (AanDuurLed[fc] >= TijdPerLed[fc])
        {
            if (Aled[fc] > 1) Aled[fc]--;
            AanDuurLed[fc] = 0;
        }
    }

    if (G[fc] || GL[fc]) Aled[fc] = 0;                        /* er is een definitieve (rgv)detectie aanvraag */
    if (R[fc] && toestemming && (Aled[fc] == 0) && ((A[fc] & BIT0) || (A[fc] & BIT1)))
    {
        if ((REALtraffick[fc] > 3) || (TA_timer[fc] > 3))       /* gedoofd houden als richting (nagenoeg)direct */
        {                                                       /* ... groen kan worden                         */
            Aled[fc] = 31;
            AanDuurLed[fc] = 0;                                  /* actuele duur uitsturing van Aled[] leds      */
        }
    }

    aantal_leds = Aled[fc];
    if (mealed != NG) MM[mealed] = Aled[fc];                  /* memory element TLCGen correct invullen       */

    if (knipper && (CIF_KLOK[CIF_TSEC_TELLER] % 10 > 4) && (Aled[fc] > 0) && (Aled[fc] < 31)) aantal_leds++;

    /* Aanpassing uitsturing aantal LEDs US[] en ME[] 12-05-2023 Jol */
    if (mealed_uit != NG)      MM[mealed_uit] = aantal_leds;  /* memory element TLCGen correct invullen       */
    if (usaled_uit != NG) CIF_GUS[usaled_uit] = aantal_leds;  /* display aantal leds dat uitgestuurd wordt    */

    if (REG)
    {
        if (us0 != NG) CIF_GUS[us0] = (bool)((aantal_leds & BIT0) > 0);
        if (us1 != NG) CIF_GUS[us1] = (bool)((aantal_leds & BIT1) > 0);
        if (us2 != NG) CIF_GUS[us2] = (bool)((aantal_leds & BIT2) > 0);
        if (us3 != NG) CIF_GUS[us3] = (bool)((aantal_leds & BIT3) > 0);
        if (us4 != NG) CIF_GUS[us4] = (bool)((aantal_leds & BIT4) > 0);
        if (usbus != NG) CIF_GUS[usbus] = (Aled[fc] > 0) && bus_sjb;
    }
    else
    {
        if (us0 != NG) CIF_GUS[us0] = FALSE;
        if (us1 != NG) CIF_GUS[us1] = FALSE;
        if (us2 != NG) CIF_GUS[us2] = FALSE;
        if (us3 != NG) CIF_GUS[us3] = FALSE;
        if (us4 != NG) CIF_GUS[us4] = FALSE;
        if (usbus != NG) CIF_GUS[usbus] = FALSE;
    }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie aansturing afteller                                                                              */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de aansturing van de aftellers.                                                    */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PostApplication_Add().                                                  */
/*                                                                                                          */
void aansturing_aftellers(void)       /* Fik240224                                                          */
{
    count i;

    for (i = 0; i < aantal_aft_123; ++i)
    {
        count fc = aft_123[i].fc;       /* FC    richting met afteller                                    */
        count de1 = aft_123[i].de1;      /* DE    koplus 1                                                 */
        count de2 = aft_123[i].de2;      /* DE    koplus 2                                                 */
        count de3 = aft_123[i].de3;      /* DE    koplus 3                                                 */
        count toest = aft_123[i].toest;    /* SCH   toestemming aansturing afteller                          */
        count min_duur = aft_123[i].min_duur; /* PRM   min.duur tot start groen waarbij afteller mag starten    */
        count tel_duur = aft_123[i].tel_duur; /* PRM   duur van een tel in tienden van seconden                 */
        count is_oke_1 = aft_123[i].is_oke_1; /* IS    afteller lantaarn 1 werkt correct                        */
        count is_oke_2 = aft_123[i].is_oke_2; /* IS    afteller lantaarn 2 werkt correct                        */
        count is_oke_3 = aft_123[i].is_oke_3; /* IS    afteller lantaarn 3 werkt correct                        */
        count is_oke_4 = aft_123[i].is_oke_4; /* IS    afteller lantaarn 4 werkt correct                        */
        count us_oke = aft_123[i].us_oke;   /* US    aftellers van alle lantaarns werken correct              */
        count us_getal = aft_123[i].us_getal; /* US    tbv verklikking op bedienpaneel                          */
        count us_bit0 = aft_123[i].us_bit0;  /* US    aansturing afteller BIT0                                 */
        count us_bit1 = aft_123[i].us_bit1;  /* US    aansturing afteller BIT1                                 */
        bool aftel_ok = aft_123[i].aftel_ok; /* bool alle aftellers van een rijrichting zijn OK               */
        mulv  act_tel = aft_123[i].act_tel;  /* mulv  actuele stand afteller                                   */
        mulv  act_duur = aft_123[i].act_duur; /* mulv  tijd in tienden van seconden dat TEL wordt aangestuurd   */

        mulv  teller_waarde = act_tel;      /* mulv  actuele stand afteller */
        mulv  aftel_duur_min = 6;           /* mulv  minimale duur van een tel in tienden van seconden */
        mulv  aftel_duur_max = 10;           /* mulv  maximale duur van een tel in tienden van seconden */
        mulv  aftel_min_start = 3;           /* mulv  minimale startwaarde afteller */

#ifdef prmaftmin
        aftel_duur_min = PRM[prmaftmin];
#endif

#ifdef prmaftmax
        aftel_duur_max = PRM[prmaftmax];
#endif

        if (aftel_duur_max < aftel_duur_min)  /* foutieve instelling -> terug naar Traffick2TLCGen defaults */
        {
            aftel_duur_min = 6;
            aftel_duur_max = 10;
        }

        if (tel_duur != NG)
        {
            if ((PRM[tel_duur] < aftel_duur_min) || (PRM[tel_duur] > aftel_duur_max))
            {
                if (PRM[tel_duur] < aftel_duur_min) PRM[tel_duur] = aftel_duur_min;
                if (PRM[tel_duur] > aftel_duur_max) PRM[tel_duur] = aftel_duur_max;

                CIF_PARM1WIJZAP = -2;             /* variabele(n) gewijzigd op de interface */
            }
        }

#ifdef prmaftminstart
        if ((PRM[prmaftminstart] < 1) || (PRM[prmaftminstart] > 3))
        {
            if (PRM[prmaftminstart] < 1) PRM[prmaftminstart] = 1;
            if (PRM[prmaftminstart] > 3) PRM[prmaftminstart] = 3;

            CIF_PARM1WIJZAP = -2;               /* variabele(n) gewijzigd op de interface */
        }

        aftel_min_start = PRM[prmaftminstart];
#endif

        aftel_ok = TRUE;
        if ((is_oke_1 != NG) && !CIF_IS[is_oke_1]) aftel_ok = FALSE;
        if ((is_oke_2 != NG) && !CIF_IS[is_oke_2]) aftel_ok = FALSE;
        if ((is_oke_3 != NG) && !CIF_IS[is_oke_3]) aftel_ok = FALSE;
        if ((is_oke_4 != NG) && !CIF_IS[is_oke_4]) aftel_ok = FALSE;

        if ((fc != NG) && (tel_duur != NG))   /* definitie is juist */
        {
            if (G[fc])                          /* richting is groen - afteller dooft */
            {
                P[fc] &= ~BIT4;
                Waft[fc] = act_duur = 0;
                if (us_getal != NG) CIF_GUS[us_getal] = FALSE;
                if (us_bit0 != NG) CIF_GUS[us_bit0] = FALSE;
                if (us_bit1 != NG) CIF_GUS[us_bit1] = FALSE;
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
                bool afteller_mag = REG && RA[fc] && !RR[fc] && !BL[fc] && aftel_ok && TOE123[fc];
                if ((toest != NG) && !SCH[toest]) afteller_mag = FALSE;
                if ((de1 != NG) && (DF[de1] || !DB[de1])) afteller_mag = FALSE;
                if ((de2 != NG) && (DF[de2] || !DB[de2])) afteller_mag = FALSE;
                if ((de3 != NG) && (DF[de3] || !DB[de3])) afteller_mag = FALSE;
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
                    teller_waarde = 3;              /* bepaal de startwaarde van de afteller */
                    if (REALtraffick[fc] < 2 * PRM[tel_duur]) teller_waarde = 2;
                    if (REALtraffick[fc] < PRM[tel_duur]) teller_waarde = 1;
                    if (teller_waarde < aftel_min_start) teller_waarde = aftel_min_start;

                    /* tijd tot groen agv afteller */
                    Waft[fc] = teller_waarde * PRM[tel_duur];
                    X[fc] |= BIT4;                  /* uitstellen tot aftellen gereed */
                }
            }
        }
        /* stuur US signalen aan */
        if (us_oke != NG) CIF_GUS[us_oke] = aftel_ok;

        if (REG)
        {
            if (us_getal != NG) CIF_GUS[us_getal] = teller_waarde;
            if (us_bit0 != NG) CIF_GUS[us_bit0] = (bool)((teller_waarde & BIT0) > 0);
            if (us_bit1 != NG) CIF_GUS[us_bit1] = (bool)((teller_waarde & BIT1) > 0);
        }
        else
        {
            if (us_getal != NG) CIF_GUS[us_getal] = FALSE;
            if (us_bit0 != NG) CIF_GUS[us_bit0] = FALSE;
            if (us_bit1 != NG) CIF_GUS[us_bit1] = FALSE;
        }

        Aaft[fc] = teller_waarde;             /* buffer actuele waarde afteller */

        aft_123[i].aftel_ok = aftel_ok;       /* bijwerken struct */
        aft_123[i].act_tel = teller_waarde;
        aft_123[i].act_duur = act_duur;
    }

    for (i = 0; i < aantal_dcf_vst; ++i)
    {
        count fc1 = dcf_vst[i].fc1;           /* richting die voorstart geeft  */
        count fc2 = dcf_vst[i].fc2;           /* richting die voorstart krijgt */
        count tvs21 = dcf_vst[i].tvs21;       /* voorstart fc2 */

        if (RA[fc1] && (P[fc1] & BIT4) && RA[fc2])
        {
            RR[fc2] = BL[fc2] = FALSE;
            if (Waft[fc1] <= T_max[tvs21] + 1) X[fc2] = FALSE;
        }
    }

    for (i = 0; i < aantal_dcf_gst; ++i)
    {
        count fc1 = dcf_gst[i].fc1;           /* richting 1 */
        count fc2 = dcf_gst[i].fc2;           /* richting 2 */

        if (RA[fc1] && (P[fc1] & BIT4) && RA[fc2])
        {
            RR[fc2] = BL[fc2] = FALSE;
        }

        if (RA[fc2] && (P[fc2] & BIT4) && RA[fc1])
        {
            RR[fc1] = BL[fc1] = FALSE;
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
void rateltikker_applicatie(          /* Fik230914                                                          */
    count fc,                             /* FC    fasecyclus                                                   */
    count hedrk1,                         /* HE    drukknop 1                                                   */
    count hedrk2,                         /* HE    drukknop 2                                                   */
    count usrat,                          /* US    rateltikker                                                  */
    count natik,                          /* TM    natikken vanaf start rood                                    */
    bool ratel_periode,                  /* bool werkingsperiode  rateltikkers                                */
    mulv  ratel_werking)                  /* mulv  werkingsparmeter rateltikkers                                */
{
    count i;

    bool accross = (natik == NG);     /* de ACCROSS rateltikker verzorgt zelfstandig het "natikken" */
    bool continu = (ratel_werking == 1) || ratel_periode && ((ratel_werking == 3) || (ratel_werking >= 5));
    bool aanvraag = (ratel_werking == 2) || ratel_periode && (ratel_werking == 4)
        || !ratel_periode && (ratel_werking >= 5);
    bool drukknop = FALSE;
    if (hedrk1 != NG) drukknop |= IH[hedrk1];
    if (hedrk2 != NG) drukknop |= IH[hedrk2];

    RAT_continu[fc] = continu;         /* buffer werking rateltikker voor eigen toevoegingen in REG[]ADD */
    RAT_aanvraag[fc] = aanvraag;

    X[fc] &= ~BIT7;                     /* reset ratel uitstel bit */

    if (!accross)
    {
        if (SR[fc] || !continu && !aanvraag && R[fc]) RAT[fc] = RAT_test[fc] = FALSE;
    }
    else
    {
        if (G[fc] && (TG_timer[fc] > 10)) RAT[fc] = FALSE;
        if (R[fc] && !TRG[fc] && !RAT[fc]) RAT_test[fc] = FALSE;
    }
    if (R[fc] && (continu || aanvraag && drukknop)) RAT[fc] = RAT_test[fc] = TRUE;

    if (!accross)
    {
        if (R[fc] && !continu && !aanvraag && (RAT[fc] || T[natik] || ET[natik])) X[fc] |= BIT7;
        if (R[fc] && !continu && !T[natik] && RAT[fc] && !A[fc]) RAT[fc] = FALSE;

        RT[natik] = (G[fc] || GL[fc]) && RAT[fc];
        AT[natik] = R[fc] && !TRG[fc] && !continu && !aanvraag;
    }

    if (REG)
    {
        if (!accross)
        {
            CIF_GUS[usrat] = RAT[fc] || T[natik];
        }
        else
        {
#ifdef AUTOMAAT
            CIF_GUS[usrat] = RAT[fc];
#else
            CIF_GUS[usrat] = RAT[fc] || RAT_test[fc];
#endif
        }
    }
    else
    {
        CIF_GUS[usrat] = FALSE;
    }

    if (accross)                        /* ACCROSS uitzetten kan alleen door groensturing */
    {
        if (R[fc] && !TRG[fc] && !A[fc] && !continu && !drukknop && CIF_GUS[usrat])
        {
            if (hedrk1 != NG) IH[hedrk1] = TRUE;
            if (hedrk2 != NG) IH[hedrk2] = TRUE;
        }
    }

    for (i = 0; i < aantal_vtg_tgo; ++i)
    {
        count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
        count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
        count hnla12 = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
        count hnla21 = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
        mulv  status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
        mulv  status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

        if (fc == fc1)
        {
            if (SG[fc2] && G[fc1] && IH[hnla21] || SG[fc1] && (status21 == 1) ||
                accross && G[fc2] && (TG_timer[fc2] <= 10) && (status21 == 2))
            {
                if (RAT_test[fc2])
                {
                    RAT[fc1] = RAT_test[fc1] = TRUE;
                    if (REG) CIF_GUS[usrat] = TRUE;
                }
            }
        }

        if (fc == fc2)
        {
            if (SG[fc1] && G[fc2] && IH[hnla12] || SG[fc2] && (status12 == 1) ||
                accross && G[fc1] && (TG_timer[fc1] <= 10) && (status12 == 2))
            {
                if (RAT_test[fc1])
                {
                    RAT[fc2] = RAT_test[fc2] = TRUE;
                    if (REG) CIF_GUS[usrat] = TRUE;
                }
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
        CIF_GUS[uswacht] = (bool)((A[fc] & BIT0) || (A[fc] & BIT1));
        if (RV[fc] && !A[fc]) CIF_GUS[uswacht] = FALSE;     /* aanvraag kwijt geraakt */
    }
    if (!REG) CIF_GUS[uswacht] = FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verklik peloton prioriteit                                                                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de verklikking van de peloton prioriteit. Het led knippert als er een voertuig in  */
/* het traject aanwezig is en brandt vast tijdens het groen als er prioriteit verstrekt is.                 */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void verklik_peloton_ingreep(void)    /* Fik230101                                                          */
{
    count i;

    for (i = 0; i < aantal_pel_kop; ++i)
    {
        count kop_fc = pel_kop[i].kop_fc;      /* koppelrichting */
        count kop_toe = pel_kop[i].kop_toe;     /* toestemming peloton ingreep */
        count verklik = pel_kop[i].verklik;     /* verklik peloton ingreep */
        mulv  aanw_kop1 = pel_kop[i].aanw_kop1;   /* aanwezigheidsduur koppelsignaal 1 vanaf start puls */
        bool pk_afronden = pel_kop[i].pk_afronden; /* afronden lopende peloton ingreep */

        bool koppeling_ingeschakeld = MM[kop_toe] || pk_afronden;

        if (verklik > NG)
        {
            CIF_GUS[verklik] = REG && (KNIP && koppeling_ingeschakeld && (aanw_kop1 > NG) ||
                G[kop_fc] && ((RW[kop_fc] & BIT12) || (YV[kop_fc] & BIT12)));
        }
    }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verklik prioriteit op basis van KAR en SRM                                                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de verklikking van prioriteit op basis van KAR en SRM. Het led knippert als er een */
/* voertuig met een prioriteitsaanvraag aanwezig is en brandt vast tijdens het groen als er prioriteit      */
/* verstrekt is.                                                                                            */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void verklik_prio_KAR_SRM(void)       /* Fik230101                                                          */
{
#ifdef PRIO_ADDFILE
    count fc;

    for (fc = 0; fc < FCMAX; ++fc)
    {
        count HD = prio_index[fc].HD;       /* count hulpdienst ingreep                                  */
        count OV_kar = prio_index[fc].OV_kar;   /* count OV ingreep - KAR                                    */
        count OV_srm = prio_index[fc].OV_srm;   /* count OV ingreep - SRM                                    */
        count VRW = prio_index[fc].VRW;      /* count VRW ingreep                                         */
        count usHD = prio_index[fc].usHD;     /* US    verklik HD ingreep                                  */
        count usOV_kar = prio_index[fc].usOV_kar; /* US    verklik OV ingreep - KAR                            */
        count usOV_srm = prio_index[fc].usOV_srm; /* US    verklik OV ingreep - SRM                            */
        count usVRW = prio_index[fc].usVRW;    /* US    verklik VRW ingreep                                 */

        if (usHD > NG)
        {
            CIF_GUS[usHD] = REG && HD_aanwezig[fc] && (KNIP || G[fc] && iPrioriteit[HD]);
        }

        if (usOV_kar > NG)
        {
            CIF_GUS[usOV_kar] = REG && (iAantalInmeldingen[OV_kar] > 0) && (KNIP || G[fc] && iPrioriteit[OV_kar]);
        }

        if (usOV_srm > NG)
        {
            CIF_GUS[usOV_srm] = REG && (iAantalInmeldingen[OV_srm] > 0) && (KNIP || G[fc] && iPrioriteit[OV_srm]);
        }

        if (usVRW > NG)
        {
            CIF_GUS[usVRW] = REG && (iAantalInmeldingen[VRW] > 0) && (KNIP || G[fc] && iPrioriteit[VRW]);
        }
    }
#endif

#ifdef tkarog                         /* schakel bewaking KAR op ondergedrag uit bij een instelling van "0" */
#ifdef uskarog
    if ((T_max[tkarog] == 0) || (tkarog_old == 0)) CIF_GUS[uskarog] = FALSE;
#endif
#endif
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verklik SRM bericht ontvangen en SRM ondergedrag                                                 */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verklikt de binnenkomst van SRM berichten en bewaakt SRM op ondergedrag.                    */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void verklik_bewaak_SRM(              /* Fik230830                                                          */
    count us_srm,                         /* US   verklik SRM bericht ontvangen                                 */
    mulv  duur_verklik_srm,               /* mulv duur verklik SRM bericht ontvangen in tienden van seconden    */
    count us_srm_og,                      /* US   verklik SRM ondergedrag                                       */
    mulv  srm_og)                         /* mulv duur ondergedrag SRM in minuten                               */
{
    if (verklik_srm > 0) verklik_srm -= TE;
    if (verklik_srm < 0) verklik_srm = 0;

    if (TS && (CIF_KLOK[CIF_SECONDE] == 0) && (duur_geen_srm < 32000)) duur_geen_srm++;

#ifndef NO_RIS
    if (RIS_NEW_PRIOREQUEST_AP_NUMBER)  /* test of er een SRM-bericht is ontvangen */
    {
        verklik_srm = duur_verklik_srm;
        duur_geen_srm = 0;
    }
#else                                 /* voorkom warning compiler */
    verklik_srm = duur_verklik_srm;
    verklik_srm = 0;
#endif

    if (us_srm > NG)
    {
        CIF_GUS[us_srm] = (verklik_srm > 0);
    }

    if (us_srm_og > NG)
    {
        CIF_GUS[us_srm_og] = (srm_og > 0) && (duur_geen_srm > srm_og);
    }
}
#endif


/* -------------------------------------------------------------------------------------------------------- */
/* Functie verklik fiets voorrang module                                                                    */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de verklikking van de fiets voorrang module. Het led knippert tijdens rood indien  */
/* een prioriteitsaanvraag aanwezig is en brandt vervolgens vast zolang de fietsrichting verlengt.          */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit post_system_application().                                              */
/*                                                                                                          */
void verklik_fiets_voorrang(void)     /* Fik230901                                                          */
{
    count i;

    for (i = 0; i < aantal_fts_pri; ++i)
    {
        count fc = fts_pri[i].fc;      /* fietsrichting */
        bool prio_av = fts_pri[i].prio_av; /* fietser is met prioriteit aangevraagd */
        count verklik = fts_pri[i].verklik; /* verklik fiets prioriteit */

        if (verklik > NG)
        {
            CIF_GUS[verklik] = REG && prio_av && (R[fc] && KNIP || G[fc]);
        }
    }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie buffer stiptheid ingemelde bussen op basis van KAR                                               */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie buffert de stiptheid van ingemelde bussen op basis van KAR.                                 */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit InUitMelden_Add().                                                      */
/*                                                                                                          */
void buffer_stiptheid_info(void)      /* Fik230101                                                          */
{
    count fc;

    mulv grensvroeg = NG;
    mulv grenslaat = NG;

#ifdef prmOVtstpgrensvroeg
    grensvroeg = PRM[prmOVtstpgrensvroeg];
#endif

#ifdef prmOVtstpgrenslaat
    grenslaat = PRM[prmOVtstpgrenslaat];
#endif

#ifdef prmovtstpgrensvroeg
    grensvroeg = PRM[prmovtstpgrensvroeg];
#endif

#ifdef prmovtstpgrenslaat
    grenslaat = PRM[prmovtstpgrenslaat];
#endif

    if ((grensvroeg > NG) && (grenslaat > NG))
    {
        for (fc = 0; fc < FCMAX; ++fc)
        {
            mulv  KAR_id_OV = prio_index[fc].KAR_id_OV;
            count prio_index_KAR = prio_index[fc].OV_kar;

            /* geen bussen aanwezig op fasecyclus FC */
            if (iAantalInmeldingen[prio_index_KAR] == 0) OV_stipt[fc] = 0;

            if (CIF_DSIWIJZ == CIF_GESCHREVEN)
            {
                if (((mulv)CIF_DSI[CIF_DSI_DIR] == KAR_id_OV) && ((CIF_DSI_VTG == 1) || (CIF_DSI_VTG == 71)))
                {
                    if (CIF_DSI[CIF_DSI_TYPE] == CIF_DSIN)
                    {
                        mulv  stiptheid_bus = 1;                      /* default is de bus op tijd */

                        if (CIF_DSI[CIF_DSI_TSTP] > grenslaat)              stiptheid_bus = 2;
                        else if (CIF_DSI[CIF_DSI_TSTP] < (-1 * grensvroeg)) stiptheid_bus = 0;

                        if (stiptheid_bus == 0) OV_stipt[fc] |= BIT0; /* registreer bus te vroeg aanwezig */
                        if (stiptheid_bus == 1) OV_stipt[fc] |= BIT1; /* registreer bus op tijd  aanwezig */
                        if (stiptheid_bus == 2) OV_stipt[fc] |= BIT2; /* registreer bus te laat  aanwezig */
                    }
                }
            }
        }
    }
}


/* -------------------------------------------------------------------------------------------------------- */
/* Functie prioriteitsafhandeling fiets voorrang module                                                     */
/* -------------------------------------------------------------------------------------------------------- */
/* Deze functie verzorgt de prioriteitsafhandeling van de fiets voorrang module. Indien de fietsrichting    */
/* geblokkeerd wordt of indien de wachttijd te hoog oploopt kan de "uitmelding" tijdens rood plaatsvinden.  */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit InUitMelden_Add().                                                      */
/*                                                                                                          */
void fiets_voorrang_module(void)      /* Fik230901                                                          */
{
    count i;

    for (i = 0; i < aantal_fts_pri; ++i)
    {
        count fc = fts_pri[i].fc;       /* fietsrichting */
        count inmeld = fts_pri[i].inmeld;   /* hulp element voor prioriteitsmodule (in.melding prioriteit) */
        count uitmeld = fts_pri[i].uitmeld;  /* hulp element voor prioriteitsmodule (uitmelding prioriteit) */
        bool aanvraag = fts_pri[i].aanvraag; /* fietser is op juiste wijze aangevraagd */
        bool prio_vw = fts_pri[i].prio_vw;  /* fietser voldoet aan prioriteitsvoorwaarden */
        bool prio_av = fts_pri[i].prio_av;  /* fietser is met prioriteit aangevraagd */

        if ((inmeld != NG) && (uitmeld != NG))
        {
            IH[inmeld] = IH[uitmeld] = FALSE;   /* reset hulp elementen */

            if (R[fc] && !TRG[fc] && prio_vw && !prio_av && !WT_TE_HOOG && (!GEEN_FIETS_PRIO || (US_type[fc] != FTS_type)))
            {
                IH[inmeld] = TRUE;
                prio_av = TRUE;
            }
            if (prio_av && (G[fc] && !VS[fc] && !FG[fc] && (!MK[fc] || (MK[fc] == PRIO_MK_BIT) || (TG_timer[fc] >= TFG_max[fc] + TVG_max[fc]) || MG[fc]) || R[fc] && !prio_vw))
            {
                IH[uitmeld] = TRUE;
                prio_av = FALSE;
            }

            fts_pri[i].prio_av = prio_av; /* bijwerken struct */
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
    count prioOV_index_verlos,            /* count OVFCfc - prioriteitsindex OV ingreep - verlos                */
    count de1,                            /* DE   koplus nabij de stopstreep                                    */
    count de2,                            /* DE   koplus tbv lengte gevoeligheid (optioneel)                    */
    count hinm,                           /* HE   puls tbv in.melding (wordt door TLCgen gegenereerd)           */
    count huitm,                          /* HE   puls tbv uitmelding (wordt door TLCgen gegenereerd)           */
    mulv  min_rood)                       /* mulv minimale roodtijd (TE) voor prioriteit aanvraag               */
{
    bool KAR_aanwezig = FALSE;
    bool SRM_aanwezig = FALSE;

    IH[hinm] = IH[huitm] = FALSE;       /* reset hulp elementen */

    prio_index[fc].OV_verlos = prioOV_index_verlos;
    if (TRG_max[fc] > min_rood) min_rood = TRG_max[fc];

#ifdef PRIO_ADDFILE
    if ((prio_index[fc].OV_kar != NG) && (iAantalInmeldingen[prio_index[fc].OV_kar] > 0)) KAR_aanwezig = TRUE;
    if ((prio_index[fc].OV_srm != NG) && (iAantalInmeldingen[prio_index[fc].OV_srm] > 0)) SRM_aanwezig = TRUE;
#endif

    if (de1 != NG)                      /* als de1 niet is gedefinieerd dan is de definitie ongeldig */
    {
        if (verlos_busbaan[fc] == 0)
        {
            if (de2 == NG)                  /* inmelding */
            {
                if (DB[de1] && (CIF_IS[de1] < CIF_DET_STORING)) verlos_busbaan[fc] = 1;
            }
            else
            {
                if (SD[de1] && (CIF_IS[de1] < CIF_DET_STORING) &&
                    D[de2] && (CIF_IS[de2] < CIF_DET_STORING)) verlos_busbaan[fc] = 1;
            }
        }

        if (de2 == NG)
        {
            if (ED[de1])
            {
                IH[huitm] = TRUE;             /* uitmelding */
                verlos_busbaan[fc] = 0;       /* een uitmelding teveel is in dit geval nooit een probleem */
            }
        }
        else
        {
            if (!D[de2])
            {
                IH[huitm] = TRUE;             /* uitmelding */
                verlos_busbaan[fc] = 0;       /* een uitmelding teveel is in dit geval nooit een probleem */
            }
        }

        if (R[fc] && (TR_timer[fc] >= min_rood) && !KAR_aanwezig && !SRM_aanwezig && (verlos_busbaan[fc] == 1))
        {
            IH[hinm] = TRUE;                /* inmelding */
            verlos_busbaan[fc] = 2;         /* gelijk aan 2 maken voorkomt altijd dubbele aanmeldingen */
        }
        /* reset verlos_baan[] bij gemiste uitmelding */
        if (EG[fc]) verlos_busbaan[fc] = 0;
    }
}


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
#ifdef PRIO_ADDFILE
void corrigeer_terugkomen_traffick(void) /* Fik230901                                                       */
{
    count fc;

    if (DVM_prog > 0)
    {
        for (fc = 0; fc < FCMAX; ++fc)
        {
            if (FC_DVM[fc]) iInstPercMaxGroenTijdTerugKomen[fc] = 100;
        }
    }
    else                                  /* alleen als er geen DVM programma actief is */
    {
        if (FILE_set > 0)
        {
            for (fc = 0; fc < FCMAX; ++fc)
            {
                if (FC_FILE[fc]) iInstPercMaxGroenTijdTerugKomen[fc] = 100;
            }
        }
    }

    for (fc = 0; fc < FCMAX; ++fc)
    {
        if (prio_index[fc].FTS != NG)
        {
            if (iGroenBewakingsTijd[prio_index[fc].FTS] != TFG_max[fc] + TVG_max[fc])
            {
                iGroenBewakingsTijd[prio_index[fc].FTS] = TFG_max[fc] + TVG_max[fc];
            }
        }
    }
}
#endif


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
    count fc;

    for (fc = 0; fc < FCMAX; ++fc)
    {
        bool herstart = FALSE;

        if (prio_index[fc].OV_kar != NG)
        {
            if (SG[fc]) iBlokkeringsTimer[prio_index[fc].OV_kar] = MAX_INTEGER;
            if ((iBlokkeringsTimer[prio_index[fc].OV_kar] == 0) &&
                (iBlokkeringsTijd[prio_index[fc].OV_kar] > 0)) herstart = TRUE;
        }

        if (prio_index[fc].OV_srm != NG)
        {
            if (SG[fc]) iBlokkeringsTimer[prio_index[fc].OV_srm] = MAX_INTEGER;
            if ((iBlokkeringsTimer[prio_index[fc].OV_srm] == 0) &&
                (iBlokkeringsTijd[prio_index[fc].OV_srm] > 0)) herstart = TRUE;
        }

        if (prio_index[fc].OV_verlos != NG)
        {
            if (SG[fc]) iBlokkeringsTimer[prio_index[fc].OV_verlos] = MAX_INTEGER;
            if ((iBlokkeringsTimer[prio_index[fc].OV_verlos] == 0) &&
                (iBlokkeringsTijd[prio_index[fc].OV_verlos] > 0)) herstart = TRUE;
        }

        if (herstart)
        {
            if (prio_index[fc].OV_kar != NG) iBlokkeringsTimer[prio_index[fc].OV_kar] = 0;
            if (prio_index[fc].OV_srm != NG) iBlokkeringsTimer[prio_index[fc].OV_srm] = 0;
            if (prio_index[fc].OV_verlos != NG) iBlokkeringsTimer[prio_index[fc].OV_verlos] = 0;
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
    count i, j, k, x, y, prio;
    mulv  grw_leds = 5;                 /* grens aantal leds wt_voorspeller voor conflicterende prioriteit    */
    bool HLPD_prio_mogelijk = FALSE;
    bool fixatie;                      /* toets fixatie bedient                                              */
    bool HLPD_ingreep_AAN;             /* HLPD ingreep staat is ingeschakeld                                 */

#ifdef prmwtv
    grw_leds = PRM[prmwtv];             /* neem instelling parameter over indien die is gedefinieerd          */
#endif

#ifdef isfix
    fixatie = CIF_IS[isfix];            /* toets fixatie bedient */
#else
    fixatie = FALSE;
#endif

    for (i = 0; i < FCMAX; ++i)         /* aftellen naloop hulpdienst ingreep */
    {
        if (NAL_HLPD[i] > TE) NAL_HLPD[i] -= TE;
        else                  NAL_HLPD[i] = 0;

        /* reset hulpdienst ingreep aktief */
        if (!HD_aanwezig[i] && (NAL_HLPD[i] == 0) || !G[i] && fixatie) HLPD[i] = FALSE;
    }

    for (y = 0; y < 2; ++y)             /* loop 2x doorlopen, 1e keer voor richtingen met HLPD, 2e keer voor de overige richtingen */
    {
        for (i = 0; i < FCMAX; ++i)       /* bepaal eerst of ingreep wel AAN staat */
        {
            if ((prio_index[i].HD > NG) && (iPrioriteitsOpties[prio_index[i].HD] == poGeenPrioriteit))
            {
                HLPD_ingreep_AAN = FALSE;
            }
            else
            {
                HLPD_ingreep_AAN = TRUE;
            }
            /* richting heeft een hulpdienst voertuig in het traject */
            if (HD_aanwezig[i] && (ARM[i] >= 0) && ((y == 0) && HLPD[i] || (y == 1) && !HLPD[i] && HLPD_ingreep_AAN))
            {
                HLPD_prio_mogelijk = TRUE;
                for (j = 0; j < FCMAX; ++j)   /* controleer voor alle richtingen op dezelfde arm en volgarm of HLPD prioriteit verstrekt kan worden */
                {
                    if ((ARM[j] >= 0) && ((ARM[j] == ARM[i]) || (ARM[j] == volg_ARM[i])))
                    {
                        for (k = 0; k < FCMAX; ++k)
                        {
                            if (FK_conflict(j, k) && (HLPD[k] || (Aled[k] > 0) && (RA[k] || (Aled[k] < grw_leds))))
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
                                        if (FK_conflict(x, k) && (HLPD[k] || (Aled[k] > 0) && (RA[k] || (Aled[k] < grw_leds))))
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

                if (HLPD_prio_mogelijk && !fixatie) /* aan HD_aanwezig[i] en diens volgrichtingen kan HLPD prioriteit worden toegekend */
                {
                    HLPD[i] = TRUE;
                    for (j = 0; j < FCMAX; ++j)
                    {
                        if ((i != j) && (ARM[j] >= 0) && ((ARM[j] == ARM[i]) || (ARM[j] == volg_ARM[i])))
                        {
                            HLPD[j] = TRUE;
                            for (x = 0; x < FCMAX; ++x)   /* controleer of er een doorkoppeling is op een derde richting */
                            {
                                if ((ARM[x] >= 0) && (ARM[x] == volg_ARM[j])) HLPD[x] = TRUE;
                            }
                        }
                    }
                }
            }
        }
    }

    for (i = 0; i < FCMAX; ++i) /* zet prioriteit opties UIT indien nog geen prioriteit mogelijk */
    {
        if (!HLPD[i] || krap(i))  /* geen HLPD of conflict in RA[] met P[] dan is hulpdienst prioriteit (nog) niet mogelijk */
        {
            count prio_hd_index = prio_index[i].HD;
            if (prio_hd_index > NG) iPrioriteitsOpties[prio_hd_index] = poGeenPrioriteit;
        }
        else                      /* wel hulpdienst prioriteit, dan geen conflicterende prioriteit mogelijk */
        {
            for (k = 0; k < FCMAX; ++k)
            {
                if (FK_conflict(i, k))
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
/* volgarm. De functie controleert of richting met de hulpdienst een volg_ARM heeft. Als dit het geval is   */
/* wordt de nalooptijd doorgezet op alle richtingen van de volg_ARM.                                        */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit PrioriteitsOpties_Add() na aanroep Traffick2TLCgen_PRIO_OPTIES().       */
/*                                                                                                          */
void Traffick2TLCgen_HLPD_nal(        /* Fik230101                                                          */
    count fc,                             /* FC   fasecyclus voedende richting                                  */
    mulv  naloop)                         /* mulv nalooptijd                                                    */
{
    count i;
    bool correct = HLPD[fc] && (volg_ARM[fc] > NG) && (naloop > NG);

    if (correct)                        /* ingreep aktief en er is een volgarm gedefinieerd */
    {
        for (i = 0; i < FCMAX; ++i)
        {
            if (ARM[i] == volg_ARM[fc])     /* richting gevonden op de volgarm */
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
    count i, k, prio;

    for (i = 0; i < aantal_pel_kop; ++i)
    {
        count kop_fc = pel_kop[i].kop_fc;    /* koppelrichting */
        mulv  pk_status = pel_kop[i].pk_status; /* status peloton ingreep */

        if (G[kop_fc] && ((RW[kop_fc] & BIT12) || (YV[kop_fc] & BIT12) || (YM[kop_fc] & BIT12)) && !Z[kop_fc] && (pk_status >= 3))
        {
            for (k = 0; k < FCMAX; ++k)
            {
                if (FK_conflict(kop_fc, k))
                {
                    for (prio = 0; prio < prioFCMAX; ++prio)
                    {
                        if (iFC_PRIOix[prio] == k)
                        {
                            if (!(iPrioriteitsOpties[prio] & poNoodDienst) && !(iPrioriteitsOpties[prio] & poAfkappenKonflikterendOV)) iPrioriteitsOpties[prio] = poGeenPrioriteit;
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
/* van de status van de regensensor.                                                                        */
/*                                                                                                          */
/* Functie wordt aangeroepen vanuit Traffick2TLCgen_PRIO_OPTIES().                                          */
/*                                                                                                          */
#ifdef PRIO_ADDFILE
void Traffick2TLCgen_FIETS(void)      /* Fik230101                                                          */
{
    count i;

    for (i = 0; i < aantal_fts_pri; ++i)
    {
        count fc = fts_pri[i].fc;       /* fietsrichting */
        count prio_fts = fts_pri[i].prio_fts; /* prioriteitscode */
        count prio_reg = fts_pri[i].prio_reg; /* prioriteitscode */
        bool prio_av = fts_pri[i].prio_av;  /* fietser is met prioriteit aangevraagd */

        count prio_fts_index = prio_index[fc].FTS;

        if (prio_av && (prio_fts_index != NG))
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
    count prmtelaat)                      /* PRM prioriteitsopties voor bus te laat                             */
{
    int   OV_opties_tevroeg = poGeenPrioriteit;
    int   OV_opties_optijd = poGeenPrioriteit;
    int   OV_opties_telaat = poGeenPrioriteit;
    int   OV_prioriteitsniveau = 0;

    count prio_index_KAR = prio_index[fc].OV_kar;

    if ((prio_index[fc].OV_kar > NG) && (OV_stipt[fc] > 0))
    {
        if ((prmtevroeg > NG) && (OV_stipt[fc] & BIT0))
        {
            if ((PRM[prmtevroeg] / 1000L) > OV_prioriteitsniveau) OV_prioriteitsniveau = PRM[prmtevroeg] / 1000L;
            OV_opties_tevroeg = BepaalPrioriteitsOpties(prmtevroeg);
        }

        if ((prmoptijd > NG) && (OV_stipt[fc] & BIT1))
        {
            if ((PRM[prmoptijd] / 1000L) > OV_prioriteitsniveau) OV_prioriteitsniveau = PRM[prmoptijd] / 1000L;
            OV_opties_optijd = BepaalPrioriteitsOpties(prmoptijd);
        }

        if ((prmtelaat > NG) && (OV_stipt[fc] & BIT2))
        {
            if ((PRM[prmtelaat] / 1000L) > OV_prioriteitsniveau) OV_prioriteitsniveau = PRM[prmtelaat] / 1000L;
            OV_opties_telaat = BepaalPrioriteitsOpties(prmtelaat);
        }

        iPrioriteitsNiveau[prio_index_KAR] = OV_prioriteitsniveau;
        iPrioriteitsOpties[prio_index_KAR] = poGeenPrioriteit;

        if (OV_opties_tevroeg & poAanvraag) iPrioriteitsOpties[prio_index_KAR] |= poAanvraag;
        if (OV_opties_tevroeg & poAfkappenKonfliktRichtingen) iPrioriteitsOpties[prio_index_KAR] |= poAfkappenKonfliktRichtingen;
        if (OV_opties_tevroeg & poGroenVastHouden) iPrioriteitsOpties[prio_index_KAR] |= poGroenVastHouden;
        if (OV_opties_tevroeg & poBijzonderRealiseren) iPrioriteitsOpties[prio_index_KAR] |= poBijzonderRealiseren;
        if (OV_opties_tevroeg & poAfkappenKonflikterendOV) iPrioriteitsOpties[prio_index_KAR] |= poAfkappenKonflikterendOV;
        if (OV_opties_tevroeg & poNoodDienst) iPrioriteitsOpties[prio_index_KAR] |= poNoodDienst;

        if (OV_opties_optijd & poAanvraag) iPrioriteitsOpties[prio_index_KAR] |= poAanvraag;
        if (OV_opties_optijd & poAfkappenKonfliktRichtingen) iPrioriteitsOpties[prio_index_KAR] |= poAfkappenKonfliktRichtingen;
        if (OV_opties_optijd & poGroenVastHouden) iPrioriteitsOpties[prio_index_KAR] |= poGroenVastHouden;
        if (OV_opties_optijd & poBijzonderRealiseren) iPrioriteitsOpties[prio_index_KAR] |= poBijzonderRealiseren;
        if (OV_opties_optijd & poAfkappenKonflikterendOV) iPrioriteitsOpties[prio_index_KAR] |= poAfkappenKonflikterendOV;
        if (OV_opties_optijd & poNoodDienst) iPrioriteitsOpties[prio_index_KAR] |= poNoodDienst;

        if (OV_opties_telaat & poAanvraag) iPrioriteitsOpties[prio_index_KAR] |= poAanvraag;
        if (OV_opties_telaat & poAfkappenKonfliktRichtingen) iPrioriteitsOpties[prio_index_KAR] |= poAfkappenKonfliktRichtingen;
        if (OV_opties_telaat & poGroenVastHouden) iPrioriteitsOpties[prio_index_KAR] |= poGroenVastHouden;
        if (OV_opties_telaat & poBijzonderRealiseren) iPrioriteitsOpties[prio_index_KAR] |= poBijzonderRealiseren;
        if (OV_opties_telaat & poAfkappenKonflikterendOV) iPrioriteitsOpties[prio_index_KAR] |= poAfkappenKonflikterendOV;
        if (OV_opties_telaat & poNoodDienst) iPrioriteitsOpties[prio_index_KAR] |= poNoodDienst;
    }
}


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
#ifndef NO_PRIO_OPTIES_TRAFFICK
    count fc, k, prio;
    mulv  grw_leds = 5;             /* grens aantal leds wt_voorspeller voor conflicterende prioriteit   */
    bool fixatie = FALSE;         /* toets fixatie bedient                                             */

#ifdef prmwtv
    grw_leds = PRM[prmwtv];              /* neem instelling parameter over indien die is gedefinieerd         */
#endif

#ifdef isfix
    fixatie = CIF_IS[isfix];            /* toets fixatie bedient                                             */
#endif

    corrigeer_opties_stiptheid();        /* corrigeer opties stiptheid (KAR) */
    Traffick2TLCgen_HLPD();              /* corrigeer voor hulpdienst ingreep */
    Traffick2TLCgen_PELOTON();           /* corrigeer voor peloton ingreep (aanhouden groenfase) */
    Traffick2TLCgen_FIETS();             /* corrigeer voor fiets voorrang module */

    for (fc = 0; fc < FCMAX; ++fc)
    {
        count prio_OV_kar_index = prio_index[fc].OV_kar;
        count prio_OV_srm_index = prio_index[fc].OV_srm;
        count prio_OV_verlos_index = prio_index[fc].OV_verlos;
        count prio_vrw_index = prio_index[fc].VRW;
        count prio_fts_index = prio_index[fc].FTS;

        if (DOSEER[fc] || !G[fc] && fixatie)
        {
            if (prio_OV_kar_index > NG) iPrioriteitsOpties[prio_OV_kar_index] = poGeenPrioriteit;
            if (prio_OV_srm_index > NG) iPrioriteitsOpties[prio_OV_srm_index] = poGeenPrioriteit;
            if (prio_OV_verlos_index > NG) iPrioriteitsOpties[prio_OV_verlos_index] = poGeenPrioriteit;
            if (prio_vrw_index > NG) iPrioriteitsOpties[prio_vrw_index] = poGeenPrioriteit;
            if (prio_fts_index > NG) iPrioriteitsOpties[prio_fts_index] = poGeenPrioriteit;
        }

        if (GEEN_OV_PRIO)
        {
            if ((prio_OV_kar_index > NG) && (!G[fc] || !iPrioriteit[prio_OV_kar_index])) iPrioriteitsOpties[prio_OV_kar_index] &= ~poBijzonderRealiseren;
            if ((prio_OV_srm_index > NG) && (!G[fc] || !iPrioriteit[prio_OV_srm_index])) iPrioriteitsOpties[prio_OV_srm_index] &= ~poBijzonderRealiseren;
            if ((prio_OV_verlos_index > NG) && (!G[fc] || !iPrioriteit[prio_OV_verlos_index])) iPrioriteitsOpties[prio_OV_verlos_index] &= ~poBijzonderRealiseren;
        }

        if (GEEN_VW_PRIO)
        {
            if ((prio_vrw_index > NG) && (!G[fc] || !iPrioriteit[prio_vrw_index])) iPrioriteitsOpties[prio_vrw_index] &= ~poBijzonderRealiseren;
        }

        /* DVM of FILE actief - schakel fiets- en vrachtwagen prioriteit uit */
        if ((DVM_prog > 0) && (DVM_prog <= aantal_dvm_prg) || (FILE_set > 0) && (FILE_set <= aantal_file_prg))
        {
            if ((prio_vrw_index > NG) && (!G[fc] || !iPrioriteit[prio_vrw_index])) iPrioriteitsOpties[prio_vrw_index] = poGeenPrioriteit;
            if ((prio_fts_index > NG) && (!G[fc] || !iPrioriteit[prio_fts_index])) iPrioriteitsOpties[prio_fts_index] = poGeenPrioriteit;

            if ((US_type[fc] != OV_type))   /* DVM of FILE programma actief - schakel OV prioriteit uit met uitzondering van busbanen */
            {
                if ((prio_OV_kar_index > NG) && (!G[fc] || !iPrioriteit[prio_OV_kar_index])) iPrioriteitsOpties[prio_OV_kar_index] = poGeenPrioriteit;
                if ((prio_OV_srm_index > NG) && (!G[fc] || !iPrioriteit[prio_OV_srm_index])) iPrioriteitsOpties[prio_OV_srm_index] = poGeenPrioriteit;
                if ((prio_OV_verlos_index > NG) && (!G[fc] || !iPrioriteit[prio_OV_verlos_index])) iPrioriteitsOpties[prio_OV_verlos_index] = poGeenPrioriteit;
            }
        }
    }

    for (fc = 0; fc < FCMAX; ++fc)       /* corrigeer voor wachttijd voorspellers */
    {
        if ((Aled[fc] > 0) && (RA[fc] || (Aled[fc] < grw_leds)))
        {
            for (k = 0; k < FCMAX; ++k)
            {
                if (FK_conflict(fc, k))
                {
                    count prio_OV_kar_index = prio_index[k].OV_kar;
                    count prio_OV_srm_index = prio_index[k].OV_srm;
                    count prio_OV_verlos_index = prio_index[k].OV_verlos;
                    count prio_vrw_index = prio_index[k].VRW;
                    count prio_fts_index = prio_index[k].FTS;

                    if ((prio_OV_kar_index > NG) && (!G[k] || !iPrioriteit[prio_OV_kar_index])) iPrioriteitsOpties[prio_OV_kar_index] = poGeenPrioriteit;
                    if ((prio_OV_srm_index > NG) && (!G[k] || !iPrioriteit[prio_OV_srm_index])) iPrioriteitsOpties[prio_OV_srm_index] = poGeenPrioriteit;
                    if ((prio_OV_verlos_index > NG) && (!G[k] || !iPrioriteit[prio_OV_verlos_index])) iPrioriteitsOpties[prio_OV_verlos_index] = poGeenPrioriteit;
                    if ((prio_vrw_index > NG) && (!G[k] || !iPrioriteit[prio_vrw_index])) iPrioriteitsOpties[prio_vrw_index] = poGeenPrioriteit;
                    if ((prio_fts_index > NG) && (!G[k] || !iPrioriteit[prio_fts_index])) iPrioriteitsOpties[prio_fts_index] = poGeenPrioriteit;
                }
            }
        }
    }
    /* corrigeer prioriteitsniveau */
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        if (!G[fc] && !(iPrioriteitsOpties[prio] & poBijzonderRealiseren)) iPrioriteitsNiveau[prio] = 0;
        if (G[fc] && !iPrioriteit[prio]) iPrioriteitsNiveau[prio] = 0;
    }
#endif
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
            count fc1 = hki_kop[i].fc1;      /* voedende richting */
            count fc2 = hki_kop[i].fc2;      /* volg     richting */
            mulv  kop_max = hki_kop[i].kop_max;  /* maximum verlenggroen na harde koppeling */
            mulv  status = hki_kop[i].status;   /* status koppeling  */

            if (status >= 1)                /* voedende richting is groen of groen geweest */
            {
                if (TVG_max[fc2] > kop_max) TVG_max[fc2] = kop_max;
                if (TGL_max[fc2] < 1) TGL_max[fc2] = 1;
            }
        }
    }

    for (i = 0; i < aantal_vtg_tgo; ++i)
    {
        count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
        count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
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
    count i, prio;

    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        if (iPrioriteit[prio])
        {
            if ((iPrioriteitsOpties[prio] & poNoodDienst) || (iPrioriteitsOpties[prio] & poBijzonderRealiseren))
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
            count fc1 = hki_kop[i].fc1;     /* voedende richting */
            count fc2 = hki_kop[i].fc2;     /* volg     richting */
            bool los_fc2 = hki_kop[i].los_fc2; /* fc2 mag bij aanvraag fc1 los realiseren */
            count status = hki_kop[i].status;  /* status koppeling */

            if (GL[fc1] || TRG[fc1] || R[fc1] && A[fc1] && !los_fc2 && (status != 1))
            {
                if (!G[fc2] && (RR[fc1] & PRIO_RR_BIT)) RR[fc2] |= PRIO_RR_BIT;
            }
        }
    }

    for (i = 0; i < aantal_vtg_tgo; ++i)
    {
        count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
        count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
        count hnla12 = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
        count hnla21 = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
        mulv  status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
        mulv  status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

        if (GL[fc1] || TRG[fc1] || R[fc1] && IH[hnla12] || R[fc2] && IH[hnla21] || R[fc1] && A[fc1] && R[fc2] && A[fc2])
        {
            if (!G[fc2] && (RR[fc1] & PRIO_RR_BIT) && (status12 != 1)) RR[fc2] |= PRIO_RR_BIT;
        }

        if (GL[fc2] || TRG[fc2] || R[fc2] && IH[hnla21] || R[fc1] && IH[hnla12] || R[fc1] && A[fc1] && R[fc2] && A[fc2])
        {
            if (!G[fc1] && (RR[fc2] & PRIO_RR_BIT) && (status21 != 1)) RR[fc1] |= PRIO_RR_BIT;
        }
    }

    for (i = 0; i < aantal_lvk_gst; ++i)
    {
        count fc1 = lvk_gst[i].fc1;       /* richting 1 */
        count fc2 = lvk_gst[i].fc2;       /* richting 2 */
        count fc3 = lvk_gst[i].fc3;       /* richting 3 */
        count fc4 = lvk_gst[i].fc4;       /* richting 4 */

        bool FcMetP = FALSE;
        bool FcMetRR = FALSE;

        if ((fc1 != NG) && RA[fc1] && P[fc1]) FcMetP = TRUE;
        if ((fc2 != NG) && RA[fc2] && P[fc2]) FcMetP = TRUE;
        if ((fc3 != NG) && RA[fc3] && P[fc3]) FcMetP = TRUE;
        if ((fc4 != NG) && RA[fc4] && P[fc4]) FcMetP = TRUE;

        if (!FcMetP)
        {
            if ((fc1 != NG) && (GL[fc1] || TRG[fc1] || R[fc1] && A[fc1]) && (RR[fc1] & PRIO_RR_BIT)) FcMetRR = TRUE;
            if ((fc2 != NG) && (GL[fc2] || TRG[fc2] || R[fc2] && A[fc2]) && (RR[fc2] & PRIO_RR_BIT)) FcMetRR = TRUE;
            if ((fc3 != NG) && (GL[fc3] || TRG[fc3] || R[fc3] && A[fc3]) && (RR[fc3] & PRIO_RR_BIT)) FcMetRR = TRUE;
            if ((fc4 != NG) && (GL[fc4] || TRG[fc4] || R[fc4] && A[fc4]) && (RR[fc4] & PRIO_RR_BIT)) FcMetRR = TRUE;
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
    count i, k, prio;

    for (i = 0; i < FCMAX; ++i)
    {                                     /* YM[]BIT2 is het veiligheidsgroenbit in TLCGen                    */
        if (MG[i] && (YM[i] & BIT2)) Z[i] = FALSE;
        if (MG[i] && (Z[i] & PRIO_Z_BIT))
        {
            bool gevonden = FALSE;
            for (k = 0; k < FCMAX; ++k)
            {
                if (GK_conflict(i, k) > NG)
                {
                    for (prio = 0; prio < prioFCMAX; ++prio)
                    {
                        if ((iPrioriteit[prio]) && (iFC_PRIOix[prio] == k))
                        {
                            if ((iPrioriteitsOpties[prio] & poNoodDienst) || (iPrioriteitsOpties[prio] & poBijzonderRealiseren)) gevonden = TRUE;
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
            if (!krap(i))
            {
                if (G[i] && !MG[i]) YV[i] |= PRIO_YV_BIT;
                Z[i] = FALSE;
                YM[i] |= PRIO_YM_BIT;
            }
            for (k = 0; k < FCMAX; ++k)
            {
                if (GK_conflict(i, k) > NG)
                {
                    if (R[k]) RR[k] |= PRIO_RR_BIT;
#ifdef __EXTRA_FUNC_RIS_H
                    if (G[k] && (VG[k] || MG[k]) && (!MG[i] || !(YM[k] & BIT2)) && !granted_verstrekt[k]) Z[k] |= PRIO_Z_BIT;
#else
                    if (G[k] && (VG[k] || MG[k]) && (!MG[i] || !(YM[k] & BIT2))) Z[k] |= PRIO_Z_BIT;
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

        bool FcInRa = FALSE;
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
        if (G[i] && (!MG[i] && (YV[i] & PRIO_YV_BIT) || MG[i] && (YM[i] & PRIO_YM_BIT)))
        {
            for (k = 0; k < FCMAX; ++k)
            {
                if (GK_conflict(i, k) > NG)
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
        count fc1 = vtg_tgo[i].fc1;      /* richting 1 */
        count fc2 = vtg_tgo[i].fc2;      /* richting 2 */
        count hnla12 = vtg_tgo[i].hnla12;   /* drukknop melding koppeling vanaf fc1 aanwezig */
        count hnla21 = vtg_tgo[i].hnla21;   /* drukknop melding koppeling vanaf fc2 aanwezig */
        count status12 = vtg_tgo[i].status12; /* status koppeling fc1 -> fc2 */
        count status21 = vtg_tgo[i].status21; /* status koppeling fc2 -> fc1 */

        if (MG[fc1] && RA[fc2] && (status12 == 1)) Z[fc1] = FALSE; /* vasthouden voeding is nodig omdat */
        if (MG[fc2] && RA[fc1] && (status21 == 1)) Z[fc2] = FALSE; /* nog aan de overzijde gedrukt kan worden */

        if (GL[fc1] || TRG[fc1] || R[fc1] && IH[hnla12] || R[fc2] && IH[hnla21])
        {
            if (!G[fc2] && (RR[fc1] & PRIO_RR_BIT) && (status12 != 1)) RR[fc2] |= PRIO_RR_BIT;
        }

        if (GL[fc2] || TRG[fc2] || R[fc2] && IH[hnla21] || R[fc1] && IH[hnla12])
        {
            if (!G[fc1] && (RR[fc2] & PRIO_RR_BIT) && (status21 != 1)) RR[fc1] |= PRIO_RR_BIT;
        }
    }

#ifdef tkarog                         /* schakel bewaking KAR op ondergedrag uit bij een instelling van "0" */
#ifdef uskarog
    RT[tkarog] |= ((T_max[tkarog] == 0) || (tkarog_old == 0));
    tkarog_old = T_max[tkarog];
#endif
#endif

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
        if (TO_max[fc1][fc2] < 0) return  TO_max[fc1][fc2];
    }
    return NG;
#endif
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal TI[][]                                                                                */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde het aflopen van de intergroentijd.                                     */
/*                                                                                                          */
bool TI(                             /* Fik230101                                                          */
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
    count j, k;
    mulv  ontruim = 0;

    for (j = 0; j < GKFC_MAX[fc1]; ++j)
    {
        k = KF_pointer[fc1][j];
        if (fc2 == k)
        {
            ontruim = TI_max(fc1, fc2);
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
bool FK_conflict(                    /* Fik230101                                                          */
    count fc1,                            /* FC fasecyclus 1                                                    */
    count fc2)                            /* FC fasecyclus 2                                                    */
{
    count j, k;

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
bool tka(                            /* Fik230101                                                          */
    count fc)                             /* FC fasecyclus                                                      */
{
    count k;

    for (k = 0; k < FCMAX; ++k)
    {
        if (GK_conflict(fc, k) > NG)
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
bool fkra(                           /* Fik230101                                                          */
    count fc)                             /* FC fasecyclus                                                      */
{
    count k;

    for (k = 0; k < FCMAX; ++k)
    {
        if (FK_conflict(fc, k))
        {
            if (RA[k]) return TRUE;
        }
    }
    return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende richting in RA[] met P[]                                  */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende richting in RA[] met P[].       */
/*                                                                                                          */
bool krap(                           /* Fik230101                                                          */
    count fc)                             /* FC fasecyclus                                                      */
{
    count k;

    for (k = 0; k < FCMAX; ++k)
    {
        if (GK_conflict(fc, k) > NG)
        {
            if (RA[k] && P[k]) return TRUE;
        }
    }
    return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid (fictief)conflicterende richting in RA[] met P[]                         */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een (fictief)conflicterende ri. in RA[] met P[].   */
/*                                                                                                          */
bool fkrap(                          /* Fik230101                                                          */
    count fc)                             /* FC fasecyclus                                                      */
{
    count k;

    for (k = 0; k < FCMAX; ++k)
    {
        if (FK_conflict(fc, k))
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
bool kaapr(                          /* Fik230101                                                          */
    count fc)                             /* FC fasecyclus                                                      */
{
    count k;

    for (k = 0; k < FCMAX; ++k)
    {
        if (GK_conflict(fc, k) > NG)
        {
            if (R[k] && A[k] && AAPR[k] && !(AAPR[k] & BIT5) && !(RR[k] & BIT6) && !(RR[k] & BIT10)) return TRUE;
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
bool tfkaa(                          /* Fik230101                                                          */
    count fc)                             /* FC fasecyclus                                                      */
{
    count k;

    for (k = 0; k < FCMAX; ++k)
    {
        if (FK_conflict(fc, k))
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
bool tkcv(                           /* Fik230101                                                          */
    count fc)                             /* FC fasecyclus                                                      */
{
    count k;

    for (k = 0; k < FCMAX; ++k)
    {
        if (GK_conflict(fc, k) > NG)
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
bool conflict_prio_real(             /* Fik230101                                                          */
    count fc)                             /* FC fasecyclus                                                      */
{
    count k;

    for (k = 0; k < FCMAX; ++k)
    {
        if (FK_conflict(fc, k))
        {
            if (!G[k] && (iPRIO[k] || HLPD[k])) return TRUE;
        }
    }
    return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid hulpdienst ingreep                                                       */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een hulpdienst ingreep.                            */
/*                                                                                                          */
bool hlpd_aanwezig(void)             /* Fik230101                                                          */
{
    count fc;

    for (fc = 0; fc < FCMAX; ++fc)
    {
        if (HLPD[fc]) return TRUE;
    }
    return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal aanwezigheid conflicterende hulpdienst ingreep                                        */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als return waarde de aanwezigheid van een conflicterende hulpdienst ingreep.             */
/*                                                                                                          */
bool khlpd(                          /* Fik240101                                                          */
    count fc)                             /* FC fasecyclus                                                      */
{
    count k;

    for (k = 0; k < FCMAX; ++k)
    {
        if (FK_conflict(fc, k))
        {
            /* Fik: conflicterende WTV kan tijdens HLPD[] en !G[] te snel af gaan lopen, dus direct halteren WTV  */
            if (/* G[k] && */ HLPD[k]) return TRUE;
        }
    }
    return FALSE;
}


/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie bepaal of een richting een volgrichting is binnen een gedefinieerde harde koppeling          */
/* -------------------------------------------------------------------------------------------------------- */
/* Hulpfunctie met als RETURN of een richting een volgrichting is binnen een gedefinieerde harde koppeling. */
/*                                                                                                          */
bool volgrichting_hki(               /* Fik230101                                                          */
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
bool voorstart_gever(                /* Fik230101                                                          */
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

    PRM[tst_stiptheid] = 120;           /* in de testomgeving wordt hier 120 vanaf gehaald (dus 0 = op tijd)  */

    if (CIF_IS[isvroeg])
    {
        PRM[tst_stiptheid] = 120 - PRM[ov_tevroeg] - 1;
    }
    if (CIF_IS[islaat])
    {
        PRM[tst_stiptheid] = 120 + PRM[ov_telaat] + 1;
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
    FILE* fp;
    count i, fc;

    fp = fopen("DumpTraffick.dmp", "wb");
    if (fp != NULL) {

        fprintf(fp, "FC   T2SG T2EG AltR  TFB AAPR   AR   PG  PAR HLPD");
        fprintf(fp, "\r\n");

        for (fc = 0; fc < FCMAX; ++fc)
        {
            fprintf(fp, "%s%s%5d%5d%5d%5d%5d%5d%5d%5d%5d", "FC", FC_code[fc], REALtraffick[fc], TEG[fc], AltRuimte[fc], TFB_timer[fc], AAPR[fc], AR[fc], PG[fc], PAR[fc], HLPD[fc]);
            fprintf(fp, "\r\n");
        }
        fprintf(fp, "\r\n");

        i = dumpstap + 1;
        if (i >= MAXDUMPSTAP) i = 0;

        fprintf(fp, "Flight buffer: %02d-%02d-%02d\r\n", CIF_KLOK[CIF_DAG], CIF_KLOK[CIF_MAAND], CIF_KLOK[CIF_JAAR]);

        while (i != dumpstap)
        {
            if ((_UUR[i] > 0) || (_MIN[i] > 0) || (_SEC[i] > 0))
            {
                fprintf(fp, "%02d:%02d:%02d", _UUR[i], _MIN[i], _SEC[i]);
                fprintf(fp, "  %4d", _ML[i]);
                for (fc = 0; fc < FCMAX; ++fc)
                {
                    if ((_FCA[fc][i] != 'A') && (_FCA[fc][i] != 'E'))
                    {
                        fprintf(fp, "%3c", _FC[fc][i]);
                    }
                    else
                    {
                        fprintf(fp, "%3c", _FCA[fc][i]);
                    }
                }

                fprintf(fp, "\r\n");
                if ((_SEC[i] == 0) || (_SEC[i] == 30))
                {
                    fprintf(fp, "              ");
                    for (fc = 0; fc < FCMAX; fc++)
                    {
                        fprintf(fp, "%3s", FC_code[fc]);
                    }
                    fprintf(fp, "\r\n");
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
            _HA[i] = (bool)A[i];
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
                if (G[i]) _FC[i][dumpstap] = 'H';
                if (!G[i]) _FC[i][dumpstap] = 'h';
            }
        }

        _UUR[dumpstap] = (char)CIF_KLOK[CIF_UUR];
        _MIN[dumpstap] = (char)CIF_KLOK[CIF_MINUUT];
        _SEC[dumpstap] = (char)CIF_KLOK[CIF_SECONDE];

#ifdef MLAMAX
        _ML[dumpstap] = MLA + 1;
#else
        _ML[dumpstap] = ML + 1;
#endif

        dumpstap++;
        if (dumpstap >= MAXDUMPSTAP) dumpstap = 0;
    }
}
#endif

#ifndef TRAFFICK_ADD
void extra_definities_traffick(void)
{
}

void extra_instellingen_traffick(void)
{
}

void detectie_veld_afhandeling(void)
{
}

void maatregelen_bij_detectie_storing(void)
{
}

void traffick_file_afhandeling(void)
{
}

void traffick_corrigeer_wtv(void)
{
}

void corrigeer_verklikking_stiptheid(void)
{
}

void busbaan_verlos_prioriteit(void)
{
}

void corrigeer_opties_stiptheid(void)
{
}
#endif
