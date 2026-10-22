/* REGEL INSTELLINGEN */
/* ------------------ */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMtab.c
      CCOL:   11.0
    TLCGEN:   
   CCOLGEN:   UnitTest
*/

/****************************** Versie commentaar ***********************************
 *
 * Versie   Datum   Ontwerper   Commentaar
 *
 ************************************************************************************/

/* include files */
/* ------------- */
    #include "GMsys.h"
    #include "fcvar.h"    /* fasecycli                         */
    #include "kfvar.h"    /* conflicten                        */
    #include "usvar.h"    /* uitgangs elementen                */
    #include "dpvar.h"    /* detectie elementen                */
    #include "trg_min.h"  /* garantie-roodtijden               */
    #include "tgg_min.h"  /* garantie-groentijden              */
    #include "tgl_min.h"  /* garantie-geeltijden               */
    #include "isvar.h"    /* ingangs elementen                 */
    #include "dsivar.h"   /* selectieve detectie               */
    #include "hevar.h"    /* hulp elementen                    */
    #include "mevar.h"    /* geheugen elementen                */
    #include "tmvar.h"    /* tijd elementen                    */
    #include "ctvar.h"    /* teller elementen                  */
    #include "schvar.h"   /* software schakelaars              */
    #include "prmvar.h"   /* parameters                        */
    #include "lwmlvar.h"  /* langstwachtende modulen structuur */
    #include "control.h"  /* controller interface              */

    mulv FC_type[FCMAX];



void control_defaults(void)
{
    TDB_defmax = NG;
    TDH_defmax = NG;
    TBG_defmax = NG;
    TOG_defmax = NG;

    TRG_defmax = NG;
    TGG_defmax = NG;
    TGL_defmax = NG;
    TFG_defmax = NG;
    TVG_defmax = NG;

    TGG_type     |= RO_type | TE_type; /* Garantiegroentijden read-only */
    TGG_min_type |= RO_type | TE_type; /* Minimale garantiegroentijden read-only */
    TGL_min_type |= RO_type | TE_type; /* Minimale garantiegeeltijden read-only */
    TRG_min_type |= RO_type | TE_type; /* Minimale garantieroodtijden read-only */

}

void control_parameters(void)
{
/* fasecycli */
/* --------- */
   FC_code[fc02] = "02"; TRG_max[fc02] = 0; TRG_min[fc02] = 0; TGG_max[fc02] = 0; TGG_min[fc02] = 0; TFG_max[fc02] = 0; TGL_max[fc02] = 0; TGL_min[fc02] = 0; TMGL_max[fc02] = 0; TVG_max[fc02] = 300;
   FC_code[fc22] = "22"; TRG_max[fc22] = 0; TRG_min[fc22] = 0; TGG_max[fc22] = 0; TGG_min[fc22] = 0; TFG_max[fc22] = 0; TGL_max[fc22] = 0; TGL_min[fc22] = 0; TMGL_max[fc22] = 0; TVG_max[fc22] = 300;
   FC_code[fc41] = "41"; TRG_max[fc41] = 0; TRG_min[fc41] = 0; TGG_max[fc41] = 0; TGG_min[fc41] = 0; TFG_max[fc41] = 0; TGL_max[fc41] = 0; TGL_min[fc41] = 0; TMGL_max[fc41] = 0; TVG_max[fc41] = 300;
   FC_code[fc42] = "42"; TRG_max[fc42] = 0; TRG_min[fc42] = 0; TGG_max[fc42] = 0; TGG_min[fc42] = 0; TFG_max[fc42] = 0; TGL_max[fc42] = 0; TGL_min[fc42] = 0; TMGL_max[fc42] = 0; TVG_max[fc42] = 300;

/* conflicten */
/* ---------- */

    TIG_max[fc02][fc22] = 30;
    TIG_max[fc02][fc41] = 30;
    TIG_max[fc02][fc42] = 30;

    TIG_max[fc22][fc02] = 30;
    TIG_max[fc22][fc41] = 30;
    TIG_max[fc22][fc42] = 30;

    TIG_max[fc41][fc02] = 30;
    TIG_max[fc41][fc22] = 30;
    TIG_max[fc41][fc42] = 30;

    TIG_max[fc42][fc02] = 30;
    TIG_max[fc42][fc22] = 30;
    TIG_max[fc42][fc41] = 30;

/* overige uitgangen */
/* ----------------- */
    US_code[ussegm1]       = "segm1";                               /* Aansturing segmenten display                            */
    US_code[ussegm2]       = "segm2";                               /* Aansturing segmenten display                            */
    US_code[ussegm3]       = "segm3";                               /* Aansturing segmenten display                            */
    US_code[ussegm4]       = "segm4";                               /* Aansturing segmenten display                            */
    US_code[ussegm5]       = "segm5";                               /* Aansturing segmenten display                            */
    US_code[ussegm6]       = "segm6";                               /* Aansturing segmenten display                            */
    US_code[ussegm7]       = "segm7";                               /* Aansturing segmenten display                            */
    US_code[usincontrol]   = "incontrol";                           /* Verklikken of applicatie daadwerkelijk de TLC aanstuurt */
    US_code[usnocontrol]   = "nocontrol";                           /* Verklikken of applicatie niet in staat is te regelen    */
    US_code[usmaxwt]       = "maxwt";                               /* Verklikken maximale wachttijd overschreden              */
    US_code[uskarmelding]  = "karmelding";                          /* Verklikken ontvangst melding KAR                        */
    US_code[uskarog]       = "karog";                               /* Verklikken ondergedrag KAR                              */
    US_code[usovinm41bus]  = "ovinm41bus";                          /* Verklikken inmelding OV fase 41                         */
    US_code[usovinm41tram] = "ovinm41tram";                         /* Verklikken inmelding OV fase 41                         */
    US_code[usovinm42]     = "ovinm42";                             /* Verklikken inmelding OV fase 42                         */
    US_code[usovinm22]     = "ovinm22";                             /* Verklikken inmelding OV fase 22                         */
    US_code[usperdef]      = "perdef";                              /* Default periode actief                                  */
    US_code[usper1]        = "per1";                                /* Periode actief                                          */

/* detectie */
/* -------- */
    D_code[d021] = "021"; 
    D_code[d022] = "022"; 
    D_code[d221] = "221"; 
    D_code[d222] = "222"; 
    D_code[dk22] = "k22"; 
    D_code[d411] = "411"; 
    D_code[d412] = "412"; 
    D_code[d421] = "421"; 
    D_code[d422] = "422"; 
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined PRACTICE_TEST
    D_code[ddummykarin41tram]  = "dummykarin41tram";  
    D_code[ddummykaruit41tram] = "dummykaruit41tram"; 
#endif

/* overige ingangen */
/* ---------------- */

/* hulp elementen */
/* -------------- */
    H_code[hprio41bus]         = "prio41bus";                                      /* Bijhouden actief zijn prioriteit fase 41 */
    H_code[hprioin41bus]       = "prioin41bus";                                    /* Prioriteit inmelding fase 41 Bus         */
    H_code[hpriouit41bus]      = "priouit41bus";                                   /* Prioriteit uitmelding 41 Bus             */
    H_code[hprioin41busris]    = "prioin41busris";                                 /* Prioriteit inmelding fase 41 Bus         */
    H_code[hpriouit41busris]   = "priouit41busris";                                /* Prioriteit uitmelding 41 Bus             */
    H_code[hprio41tram]        = "prio41tram";                                     /* Bijhouden actief zijn prioriteit fase 41 */
    H_code[hprioin41tram]      = "prioin41tram";                                   /* Prioriteit inmelding fase 41 Tram        */
    H_code[hpriouit41tram]     = "priouit41tram";                                  /* Prioriteit uitmelding 41 Tram            */
    H_code[hprioin41tramris]   = "prioin41tramris";                                /* Prioriteit inmelding fase 41 Tram        */
    H_code[hprioin41tramkar]   = "prioin41tramkar";                                /* Prioriteit inmelding fase 41 Tram        */
    H_code[hpriouit41tramris]  = "priouit41tramris";                               /* Prioriteit uitmelding 41 Tram            */
    H_code[hpriouit41tramkar]  = "priouit41tramkar";                               /* Prioriteit uitmelding 41 Tram            */
    H_code[hprio42]            = "prio42";                                         /* Bijhouden actief zijn prioriteit fase 42 */
    H_code[hprioin42]          = "prioin42";                                       /* Prioriteit inmelding fase 42 Bus         */
    H_code[hpriouit42]         = "priouit42";                                      /* Prioriteit uitmelding 42 Bus             */
    H_code[hprioin42ris]       = "prioin42ris";                                    /* Prioriteit inmelding fase 42 Bus         */
    H_code[hpriouit42ris]      = "priouit42ris";                                   /* Prioriteit uitmelding 42 Bus             */
    H_code[hprio22]            = "prio22";                                         /* Bijhouden actief zijn prioriteit fase 22 */
    H_code[hprioin22]          = "prioin22";                                       /* Prioriteit inmelding fase 22 Fiets       */
    H_code[hpriouit22]         = "priouit22";                                      /* Prioriteit uitmelding 22 Fiets           */
    H_code[hprioin22fiets221]  = "prioin22fiets221";                               /* Prioriteit inmelding fase 22 Fiets       */
    H_code[hpriouit22fiets221] = "priouit22fiets221";                              /* Prioriteit uitmelding 22 Fiets           */

/* geheugen elementen */
/* ------------------ */
    MM_code[mperiod]           = "period";                                       /* Onthouden actieve periode                     */
    MM_code[mftstelris22fiets] = "ftstelris22fiets";                             /* Tellen fietsers obv. RIS voor prio ingreep 22 */
    MM_code[mar02]             = "ar02";                                         /* Alternatieve ruimte fase 02                   */
    MM_code[mar22]             = "ar22";                                         /* Alternatieve ruimte fase 22                   */
    MM_code[mar41]             = "ar41";                                         /* Alternatieve ruimte fase 41                   */
    MM_code[mar42]             = "ar42";                                         /* Alternatieve ruimte fase 42                   */

/* tijd elementen */
/* -------------- */
    T_code[tkm02]        = "km02";        T_max[tkm02]        = 0;    T_type[tkm02]        = TE_type; /* Kop maximum voor detector 02                          */
    T_code[tkm22]        = "km22";        T_max[tkm22]        = 0;    T_type[tkm22]        = TE_type; /* Kop maximum voor detector 22                          */
    T_code[tkm41]        = "km41";        T_max[tkm41]        = 0;    T_type[tkm41]        = TE_type; /* Kop maximum voor detector 41                          */
    T_code[tkm42]        = "km42";        T_max[tkm42]        = 0;    T_type[tkm42]        = TE_type; /* Kop maximum voor detector 42                          */
    T_code[tkarmelding]  = "karmelding";  T_max[tkarmelding]  = 15;   T_type[tkarmelding]  = TE_type; /* Duur verklikking ontvangst melding KAR                */
    T_code[tkarog]       = "karog";       T_max[tkarog]       = 1440; T_type[tkarog]       = TM_type; /* Ondergedrag KAR                                       */
    T_code[tbtovg41bus]  = "btovg41bus";  T_max[tbtovg41bus]  = 30;   T_type[tbtovg41bus]  = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    T_code[trt41bus]     = "rt41bus";     T_max[trt41bus]     = 0;    T_type[trt41bus]     = TE_type; /* Actuele rijtijd prio fase 41                          */
    T_code[tgb41bus]     = "gb41bus";     T_max[tgb41bus]     = 600;  T_type[tgb41bus]     = TE_type; /* Groenbewaking prioriteit fase 41                      */
    T_code[tblk41bus]    = "blk41bus";    T_max[tblk41bus]    = 200;  T_type[tblk41bus]    = TE_type; /* Blokkeertijd na prioriteitsingreep fase 41            */
    T_code[tbtovg41tram] = "btovg41tram"; T_max[tbtovg41tram] = 30;   T_type[tbtovg41tram] = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    T_code[trt41tram]    = "rt41tram";    T_max[trt41tram]    = 0;    T_type[trt41tram]    = TE_type; /* Actuele rijtijd prio fase 41                          */
    T_code[tgb41tram]    = "gb41tram";    T_max[tgb41tram]    = 600;  T_type[tgb41tram]    = TE_type; /* Groenbewaking prioriteit fase 41                      */
    T_code[tblk41tram]   = "blk41tram";   T_max[tblk41tram]   = 200;  T_type[tblk41tram]   = TE_type; /* Blokkeertijd na prioriteitsingreep fase 41            */
    T_code[tbtovg42]     = "btovg42";     T_max[tbtovg42]     = 30;   T_type[tbtovg42]     = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 42 */
    T_code[trt42]        = "rt42";        T_max[trt42]        = 0;    T_type[trt42]        = TE_type; /* Actuele rijtijd prio fase 42                          */
    T_code[tgb42]        = "gb42";        T_max[tgb42]        = 600;  T_type[tgb42]        = TE_type; /* Groenbewaking prioriteit fase 42                      */
    T_code[tblk42]       = "blk42";       T_max[tblk42]       = 200;  T_type[tblk42]       = TE_type; /* Blokkeertijd na prioriteitsingreep fase 42            */
    T_code[tbtovg22]     = "btovg22";     T_max[tbtovg22]     = 30;   T_type[tbtovg22]     = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 22 */
    T_code[trt22]        = "rt22";        T_max[trt22]        = 0;    T_type[trt22]        = TE_type; /* Actuele rijtijd prio fase 22                          */
    T_code[tgb22]        = "gb22";        T_max[tgb22]        = 600;  T_type[tgb22]        = TE_type; /* Groenbewaking prioriteit fase 22                      */
    T_code[tblk22]       = "blk22";       T_max[tblk22]       = 200;  T_type[tblk22]       = TE_type; /* Blokkeertijd na prioriteitsingreep fase 22            */
    T_code[tris41bus]    = "ris41bus";    T_max[tris41bus]    = 0;    T_type[tris41bus]    = TE_type;
    T_code[tris41tram]   = "ris41tram";   T_max[tris41tram]   = 0;    T_type[tris41tram]   = TE_type;
    T_code[tris42bus]    = "ris42bus";    T_max[tris42bus]    = 0;    T_type[tris42bus]    = TE_type;

/* teller elementen */
/* ---------------- */
    C_code[cvc41bus]       = "vc41bus";       C_max[cvc41bus]       = 999; C_type[cvc41bus]       = CT_type; /* Bijhouden prio inmeldingen fase 41 type Bus             */
    C_code[cvc41tram]      = "vc41tram";      C_max[cvc41tram]      = 999; C_type[cvc41tram]      = CT_type; /* Bijhouden prio inmeldingen fase 41 type Tram            */
    C_code[cvc42]          = "vc42";          C_max[cvc42]          = 999; C_type[cvc42]          = CT_type; /* Bijhouden prio inmeldingen fase 42 type Bus             */
    C_code[cftscyc22fiets] = "ftscyc22fiets"; C_max[cftscyc22fiets] = 999;                                   /* Bijhouden realisaties tbv peloton prio voor fase 22     */
    C_code[cftsvtg22fiets] = "ftsvtg22fiets"; C_max[cftsvtg22fiets] = 999;                                   /* Bijhouden aantal fietsers tbv peloton prio voor fase 22 */
    C_code[cvc22]          = "vc22";          C_max[cvc22]          = 999; C_type[cvc22]          = CT_type; /* Bijhouden prio inmeldingen fase 22 type Fiets           */

/* schakelaars */
/* ----------- */
    SCH_code[schcheckdstype]       = "checkdstype";       SCH[schcheckdstype]       = 0;                                    /* Check type DSI bericht bij VECOM                            */
    SCH_code[schprioin41busris]    = "prioin41busris";    SCH[schprioin41busris]    = 1;                                    /* Inmelden 41 via Bus toestaan                                */
    SCH_code[schpriouit41busris]   = "priouit41busris";   SCH[schpriouit41busris]   = 1;                                    /* Uitmelden 41 via Bus toestaan                               */
    SCH_code[schprioin41tramris]   = "prioin41tramris";   SCH[schprioin41tramris]   = 1;                                    /* Inmelden 41 via Tram toestaan                               */
    SCH_code[schprioin41tramkar]   = "prioin41tramkar";   SCH[schprioin41tramkar]   = 1;                                    /* Inmelden 41 via Tram toestaan                               */
    SCH_code[schpriouit41tramris]  = "priouit41tramris";  SCH[schpriouit41tramris]  = 1;                                    /* Uitmelden 41 via Tram toestaan                              */
    SCH_code[schpriouit41tramkar]  = "priouit41tramkar";  SCH[schpriouit41tramkar]  = 1;                                    /* Uitmelden 41 via Tram toestaan                              */
    SCH_code[schprioin42ris]       = "prioin42ris";       SCH[schprioin42ris]       = 1;                                    /* Inmelden 42 via Bus toestaan                                */
    SCH_code[schpriouit42ris]      = "priouit42ris";      SCH[schpriouit42ris]      = 1;                                    /* Uitmelden 42 via Bus toestaan                               */
    SCH_code[schprioin22fiets221]  = "prioin22fiets221";  SCH[schprioin22fiets221]  = 1;                                    /* Inmelden 22 via Fiets toestaan                              */
    SCH_code[schpriouit22fiets221] = "priouit22fiets221"; SCH[schpriouit22fiets221] = 1;                                    /* Uitmelden 22 via Fiets toestaan                             */
    SCH_code[schrisgeencheckopsg]  = "risgeencheckopsg";  SCH[schrisgeencheckopsg]  = 0;                                    /* Niet checken op signaalgroep bij RIS aanvragen en verlengen */
    SCH_code[schrisaanvraag]       = "risaanvraag";       SCH[schrisaanvraag]       = 1;                                    /* Globaal in of uitschakelen aanvragen via RIS                */
    SCH_code[schrisverlengen]      = "risverlengen";      SCH[schrisverlengen]      = 1;                                    /* Globaal in of uitschakelen verlengen via RIS                */
    SCH_code[schaltg02]            = "altg02";            SCH[schaltg02]            = 0;                                    /* Alternatieve realisatie toestaan fase 02                    */
    SCH_code[schaltg22]            = "altg22";            SCH[schaltg22]            = 0;                                    /* Alternatieve realisatie toestaan fase 22                    */
    SCH_code[schaltg41]            = "altg41";            SCH[schaltg41]            = 0;                                    /* Alternatieve realisatie toestaan fase 41                    */
    SCH_code[schaltg42]            = "altg42";            SCH[schaltg42]            = 0;                                    /* Alternatieve realisatie toestaan fase 42                    */

/* parameters */
/* ---------- */
    PRM_code[prmfb]                     = "fb";                     PRM[prmfb]                     = 0;   PRM_type[prmfb]                     = TS_type; /* Instelling fasebewaking                                                        */
    PRM_code[prmxx]                     = "xx";                     PRM[prmxx]                     = 0;                                                  /* Versiebeheer xx                                                                */
    PRM_code[prmyy]                     = "yy";                     PRM[prmyy]                     = 0;                                                  /* Versiebeheer yy                                                                */
    PRM_code[prmzz]                     = "zz";                     PRM[prmzz]                     = 0;                                                  /* Versiebeheer zz                                                                */
    PRM_code[prmmk021]                  = "mk021";                  PRM[prmmk021]                  = 0;                                                  /* Meetkriterium type voor detector 021                                           */
    PRM_code[prmmk022]                  = "mk022";                  PRM[prmmk022]                  = 0;                                                  /* Meetkriterium type voor detector 022                                           */
    PRM_code[prmmk221]                  = "mk221";                  PRM[prmmk221]                  = 0;                                                  /* Meetkriterium type voor detector 221                                           */
    PRM_code[prmmk222]                  = "mk222";                  PRM[prmmk222]                  = 0;                                                  /* Meetkriterium type voor detector 222                                           */
    PRM_code[prmmkk22]                  = "mkk22";                  PRM[prmmkk22]                  = 0;                                                  /* Meetkriterium type voor detector k22                                           */
    PRM_code[prmmk411]                  = "mk411";                  PRM[prmmk411]                  = 0;                                                  /* Meetkriterium type voor detector 411                                           */
    PRM_code[prmmk412]                  = "mk412";                  PRM[prmmk412]                  = 0;                                                  /* Meetkriterium type voor detector 412                                           */
    PRM_code[prmmk421]                  = "mk421";                  PRM[prmmk421]                  = 0;                                                  /* Meetkriterium type voor detector 421                                           */
    PRM_code[prmmk422]                  = "mk422";                  PRM[prmmk422]                  = 0;                                                  /* Meetkriterium type voor detector 422                                           */
    PRM_code[prmmwta]                   = "mwta";                   PRM[prmmwta]                   = 0;   PRM_type[prmmwta]                   = TS_type; /* Maximale wachttijd autoverkeer                                                 */
    PRM_code[prmmwtfts]                 = "mwtfts";                 PRM[prmmwtfts]                 = 0;   PRM_type[prmmwtfts]                 = TS_type; /* Maximale wachttijd fiets                                                       */
    PRM_code[prmmwtvtg]                 = "mwtvtg";                 PRM[prmmwtvtg]                 = 0;   PRM_type[prmmwtvtg]                 = TS_type; /* Maximale wachttijd voetgangers                                                 */
    PRM_code[prmpmgt02]                 = "pmgt02";                 PRM[prmpmgt02]                 = 0;                                                  /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                  */
    PRM_code[prmognt02]                 = "ognt02";                 PRM[prmognt02]                 = 0;   PRM_type[prmognt02]                 = TE_type; /* Minimale groentijd bij terugkomen fase 02                                      */
    PRM_code[prmnofm02]                 = "nofm02";                 PRM[prmnofm02]                 = 0;   PRM_type[prmnofm02]                 = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 02                               */
    PRM_code[prmmgcov02]                = "mgcov02";                PRM[prmmgcov02]                = 0;   PRM_type[prmmgcov02]                = TE_type; /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov02]               = "pmgcov02";               PRM[prmpmgcov02]               = 0;                                                  /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg02]                = "ohpmg02";                PRM[prmohpmg02]                = 0;                                                  /* Percentage ophogen groentijd na afkappen fase 02                               */
    PRM_code[prmpmgt22]                 = "pmgt22";                 PRM[prmpmgt22]                 = 0;                                                  /* Minimaal percentage groentijd primair tbv. terugkomen fase 22                  */
    PRM_code[prmognt22]                 = "ognt22";                 PRM[prmognt22]                 = 0;   PRM_type[prmognt22]                 = TE_type; /* Minimale groentijd bij terugkomen fase 22                                      */
    PRM_code[prmnofm22]                 = "nofm22";                 PRM[prmnofm22]                 = 0;   PRM_type[prmnofm22]                 = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 22                               */
    PRM_code[prmmgcov22]                = "mgcov22";                PRM[prmmgcov22]                = 0;   PRM_type[prmmgcov22]                = TE_type; /* Minimum groentijd waarna fase 22 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov22]               = "pmgcov22";               PRM[prmpmgcov22]               = 0;                                                  /* Minimum percentage groentijd waarna fase 22 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg22]                = "ohpmg22";                PRM[prmohpmg22]                = 0;                                                  /* Percentage ophogen groentijd na afkappen fase 22                               */
    PRM_code[prmpmgt41]                 = "pmgt41";                 PRM[prmpmgt41]                 = 0;                                                  /* Minimaal percentage groentijd primair tbv. terugkomen fase 41                  */
    PRM_code[prmognt41]                 = "ognt41";                 PRM[prmognt41]                 = 0;   PRM_type[prmognt41]                 = TE_type; /* Minimale groentijd bij terugkomen fase 41                                      */
    PRM_code[prmnofm41]                 = "nofm41";                 PRM[prmnofm41]                 = 0;   PRM_type[prmnofm41]                 = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 41                               */
    PRM_code[prmmgcov41]                = "mgcov41";                PRM[prmmgcov41]                = 0;   PRM_type[prmmgcov41]                = TE_type; /* Minimum groentijd waarna fase 41 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov41]               = "pmgcov41";               PRM[prmpmgcov41]               = 0;                                                  /* Minimum percentage groentijd waarna fase 41 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg41]                = "ohpmg41";                PRM[prmohpmg41]                = 0;                                                  /* Percentage ophogen groentijd na afkappen fase 41                               */
    PRM_code[prmpmgt42]                 = "pmgt42";                 PRM[prmpmgt42]                 = 0;                                                  /* Minimaal percentage groentijd primair tbv. terugkomen fase 42                  */
    PRM_code[prmognt42]                 = "ognt42";                 PRM[prmognt42]                 = 0;   PRM_type[prmognt42]                 = TE_type; /* Minimale groentijd bij terugkomen fase 42                                      */
    PRM_code[prmnofm42]                 = "nofm42";                 PRM[prmnofm42]                 = 0;   PRM_type[prmnofm42]                 = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 42                               */
    PRM_code[prmmgcov42]                = "mgcov42";                PRM[prmmgcov42]                = 0;   PRM_type[prmmgcov42]                = TE_type; /* Minimum groentijd waarna fase 42 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov42]               = "pmgcov42";               PRM[prmpmgcov42]               = 0;                                                  /* Minimum percentage groentijd waarna fase 42 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg42]                = "ohpmg42";                PRM[prmohpmg42]                = 0;                                                  /* Percentage ophogen groentijd na afkappen fase 42                               */
    PRM_code[prmrto41bus]               = "rto41bus";               PRM[prmrto41bus]               = 150; PRM_type[prmrto41bus]               = TE_type; /* Ongehinderde rijtijd prioriteit fase 41                                        */
    PRM_code[prmrtbg41bus]              = "rtbg41bus";              PRM[prmrtbg41bus]              = 200; PRM_type[prmrtbg41bus]              = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    PRM_code[prmrtg41bus]               = "rtg41bus";               PRM[prmrtg41bus]               = 300; PRM_type[prmrtg41bus]               = TE_type; /* Gehinderde rijtijd prioriteit fase 41                                          */
    PRM_code[prmomx41bus]               = "omx41bus";               PRM[prmomx41bus]               = 50;  PRM_type[prmomx41bus]               = TE_type; /* Ondermaximum OV fase 41                                                        */
    PRM_code[prmupinagb41bus]           = "upinagb41bus";           PRM[prmupinagb41bus]           = 0;                                                  /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    PRM_code[prmprio41bus]              = "prio41bus";              PRM[prmprio41bus]              = 0;                                                  /* Prioriteitsinstelling OV fase 41                                               */
    PRM_code[prmrisstart41bus]          = "risstart41bus";          PRM[prmrisstart41bus]          = 0;                                                 
    PRM_code[prmrisend41bus]            = "risend41bus";            PRM[prmrisend41bus]            = 300;                                               
    PRM_code[prmriseta41bus]            = "riseta41bus";            PRM[prmriseta41bus]            = 10;  PRM_type[prmriseta41bus]            = TE_type;
    PRM_code[prmrisrole41bus]           = "risrole41bus";           PRM[prmrisrole41bus]           = 2;                                                 
    PRM_code[prmrissubrole41bus]        = "rissubrole41bus";        PRM[prmrissubrole41bus]        = 2;                                                 
    PRM_code[prmrisimportance41bus]     = "risimportance41bus";     PRM[prmrisimportance41bus]     = 2;                                                 
    PRM_code[prmrisstationtype41bus]    = "risstationtype41bus";    PRM[prmrisstationtype41bus]    = 0;                                                 
    PRM_code[prmrisgrenspriotype41bus]  = "risgrenspriotype41bus";  PRM[prmrisgrenspriotype41bus]  = 9;                                                  /* Greenswaarde type prioriteit RIS voor fase 41                                  */
    PRM_code[prmrisapproachid41bus]     = "risapproachid41bus";     PRM[prmrisapproachid41bus]     = 1;                                                 
    PRM_code[prmrislaneid41bus_1]       = "rislaneid41bus_1";       PRM[prmrislaneid41bus_1]       = 1;                                                 
    PRM_code[prmrislaneid41bus_2]       = "rislaneid41bus_2";       PRM[prmrislaneid41bus_2]       = 2;                                                 
    PRM_code[prmrto41tram]              = "rto41tram";              PRM[prmrto41tram]              = 150; PRM_type[prmrto41tram]              = TE_type; /* Ongehinderde rijtijd prioriteit fase 41                                        */
    PRM_code[prmrtbg41tram]             = "rtbg41tram";             PRM[prmrtbg41tram]             = 200; PRM_type[prmrtbg41tram]             = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    PRM_code[prmrtg41tram]              = "rtg41tram";              PRM[prmrtg41tram]              = 300; PRM_type[prmrtg41tram]              = TE_type; /* Gehinderde rijtijd prioriteit fase 41                                          */
    PRM_code[prmomx41tram]              = "omx41tram";              PRM[prmomx41tram]              = 50;  PRM_type[prmomx41tram]              = TE_type; /* Ondermaximum OV fase 41                                                        */
    PRM_code[prmupinagb41tram]          = "upinagb41tram";          PRM[prmupinagb41tram]          = 0;                                                  /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    PRM_code[prmvtgcat41tram]           = "vtgcat41tram";           PRM[prmvtgcat41tram]           = 2;                                                  /* Voertuigcategorie DSI voor prio ingreep 41tram                                 */
    PRM_code[prmprio41tram]             = "prio41tram";             PRM[prmprio41tram]             = 0;                                                  /* Prioriteitsinstelling OV fase 41                                               */
    PRM_code[prmrisstart41tram]         = "risstart41tram";         PRM[prmrisstart41tram]         = 0;                                                 
    PRM_code[prmrisend41tram]           = "risend41tram";           PRM[prmrisend41tram]           = 300;                                               
    PRM_code[prmrisrole41tram]          = "risrole41tram";          PRM[prmrisrole41tram]          = 2;                                                 
    PRM_code[prmrissubrole41tram]       = "rissubrole41tram";       PRM[prmrissubrole41tram]       = 4;                                                 
    PRM_code[prmrisimportance41tram]    = "risimportance41tram";    PRM[prmrisimportance41tram]    = 2;                                                 
    PRM_code[prmrisstationtype41tram]   = "risstationtype41tram";   PRM[prmrisstationtype41tram]   = 0;                                                 
    PRM_code[prmrisgrenspriotype41tram] = "risgrenspriotype41tram"; PRM[prmrisgrenspriotype41tram] = 9;                                                  /* Greenswaarde type prioriteit RIS voor fase 41                                  */
    PRM_code[prmrisapproachid41tram]    = "risapproachid41tram";    PRM[prmrisapproachid41tram]    = 1;                                                 
    PRM_code[prmrislaneid41tram_1]      = "rislaneid41tram_1";      PRM[prmrislaneid41tram_1]      = 1;                                                 
    PRM_code[prmrislaneid41tram_2]      = "rislaneid41tram_2";      PRM[prmrislaneid41tram_2]      = 2;                                                 
    PRM_code[prmrto42]                  = "rto42";                  PRM[prmrto42]                  = 150; PRM_type[prmrto42]                  = TE_type; /* Ongehinderde rijtijd prioriteit fase 42                                        */
    PRM_code[prmrtbg42]                 = "rtbg42";                 PRM[prmrtbg42]                 = 200; PRM_type[prmrtbg42]                 = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 42                                  */
    PRM_code[prmrtg42]                  = "rtg42";                  PRM[prmrtg42]                  = 300; PRM_type[prmrtg42]                  = TE_type; /* Gehinderde rijtijd prioriteit fase 42                                          */
    PRM_code[prmomx42]                  = "omx42";                  PRM[prmomx42]                  = 50;  PRM_type[prmomx42]                  = TE_type; /* Ondermaximum OV fase 42                                                        */
    PRM_code[prmupinagb42]              = "upinagb42";              PRM[prmupinagb42]              = 0;                                                  /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 42                  */
    PRM_code[prmprio42]                 = "prio42";                 PRM[prmprio42]                 = 0;                                                  /* Prioriteitsinstelling OV fase 42                                               */
    PRM_code[prmrisstart42]             = "risstart42";             PRM[prmrisstart42]             = 0;                                                 
    PRM_code[prmrisend42]               = "risend42";               PRM[prmrisend42]               = 300;                                               
    PRM_code[prmriseta42]               = "riseta42";               PRM[prmriseta42]               = 5;   PRM_type[prmriseta42]               = TE_type;
    PRM_code[prmrisrole42]              = "risrole42";              PRM[prmrisrole42]              = 2;                                                 
    PRM_code[prmrissubrole42]           = "rissubrole42";           PRM[prmrissubrole42]           = 2;                                                 
    PRM_code[prmrisimportance42]        = "risimportance42";        PRM[prmrisimportance42]        = 2;                                                 
    PRM_code[prmrisstationtype42]       = "risstationtype42";       PRM[prmrisstationtype42]       = 0;                                                 
    PRM_code[prmrisgrenspriotype42]     = "risgrenspriotype42";     PRM[prmrisgrenspriotype42]     = 9;                                                  /* Greenswaarde type prioriteit RIS voor fase 42                                  */
    PRM_code[prmftsblok22fiets]         = "ftsblok22fiets";         PRM[prmftsblok22fiets]         = 0;                                                  /* Blokken waarin fiets peloton prio actief mag zijn voor fase 22                 */
    PRM_code[prmftsmaxpercyc22fiets]    = "ftsmaxpercyc22fiets";    PRM[prmftsmaxpercyc22fiets]    = 0;                                                  /* Maximaal aantal keer fiets peloton prio per cyclus voor fase 22                */
    PRM_code[prmftsminvtg22fiets]       = "ftsminvtg22fiets";       PRM[prmftsminvtg22fiets]       = 3;                                                  /* Minimaal aantal gemeten fietsers tbv peloton prio voor fase 22                 */
    PRM_code[prmftsminwt22fiets]        = "ftsminwt22fiets";        PRM[prmftsminwt22fiets]        = 0;   PRM_type[prmftsminwt22fiets]        = TS_type; /* Minimale wachttijd tbv peloton prio voor fase 22                               */
    PRM_code[prmftsminvtgris22fiets]    = "ftsminvtgris22fiets";    PRM[prmftsminvtgris22fiets]    = 2;                                                  /* Minimaal aantal gemeten fietsers via RIS tbv peloton prio voor fase 22         */
    PRM_code[prmrto22]                  = "rto22";                  PRM[prmrto22]                  = 150; PRM_type[prmrto22]                  = TE_type; /* Ongehinderde rijtijd prioriteit fase 22                                        */
    PRM_code[prmrtbg22]                 = "rtbg22";                 PRM[prmrtbg22]                 = 200; PRM_type[prmrtbg22]                 = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 22                                  */
    PRM_code[prmrtg22]                  = "rtg22";                  PRM[prmrtg22]                  = 300; PRM_type[prmrtg22]                  = TE_type; /* Gehinderde rijtijd prioriteit fase 22                                          */
    PRM_code[prmomx22]                  = "omx22";                  PRM[prmomx22]                  = 50;  PRM_type[prmomx22]                  = TE_type; /* Ondermaximum OV fase 22                                                        */
    PRM_code[prmupinagb22]              = "upinagb22";              PRM[prmupinagb22]              = 0;                                                  /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 22                  */
    PRM_code[prmprio22]                 = "prio22";                 PRM[prmprio22]                 = 0;                                                  /* Prioriteitsinstelling OV fase 22                                               */
    PRM_code[prmrisstart22]             = "risstart22";             PRM[prmrisstart22]             = 0;                                                 
    PRM_code[prmrisend22]               = "risend22";               PRM[prmrisend22]               = 150;                                               
    PRM_code[prmriseta22]               = "riseta22";               PRM[prmriseta22]               = 0;   PRM_type[prmriseta22]               = TE_type;
    PRM_code[prmrisrole22]              = "risrole22";              PRM[prmrisrole22]              = 1;                                                 
    PRM_code[prmrissubrole22]           = "rissubrole22";           PRM[prmrissubrole22]           = 1;                                                 
    PRM_code[prmrisimportance22]        = "risimportance22";        PRM[prmrisimportance22]        = 0;                                                 
    PRM_code[prmrisstationtype22]       = "risstationtype22";       PRM[prmrisstationtype22]       = 0;                                                 
    PRM_code[prmrisgrenspriotype22]     = "risgrenspriotype22";     PRM[prmrisgrenspriotype22]     = 9;                                                  /* Greenswaarde type prioriteit RIS voor fase 22                                  */
    PRM_code[prmstkp1]                  = "stkp1";                  PRM[prmstkp1]                  = 600; PRM_type[prmstkp1]                  = TI_type; /* Start klokperiode ochtend                                                      */
    PRM_code[prmetkp1]                  = "etkp1";                  PRM[prmetkp1]                  = 900; PRM_type[prmetkp1]                  = TI_type; /* Einde klokperiode ochtend                                                      */
    PRM_code[prmdckp1]                  = "dckp1";                  PRM[prmdckp1]                  = 8;                                                  /* Dagsoort klokperiode ochtend                                                   */
    PRM_code[prmmg1_02]                 = "mg1_02";                 PRM[prmmg1_02]                 = 300; PRM_type[prmmg1_02]                 = TE_type; /* Maximale groentijd MG1 02                                                      */
    PRM_code[prmmg1_22]                 = "mg1_22";                 PRM[prmmg1_22]                 = 300; PRM_type[prmmg1_22]                 = TE_type; /* Maximale groentijd MG1 22                                                      */
    PRM_code[prmmg1_41]                 = "mg1_41";                 PRM[prmmg1_41]                 = 300; PRM_type[prmmg1_41]                 = TE_type; /* Maximale groentijd MG1 41                                                      */
    PRM_code[prmmg1_42]                 = "mg1_42";                 PRM[prmmg1_42]                 = 300; PRM_type[prmmg1_42]                 = TE_type; /* Maximale groentijd MG1 42                                                      */
    PRM_code[prmmg2_02]                 = "mg2_02";                 PRM[prmmg2_02]                 = 200; PRM_type[prmmg2_02]                 = TE_type; /* Maximale groentijd MG2 02                                                      */
    PRM_code[prmmg2_22]                 = "mg2_22";                 PRM[prmmg2_22]                 = 200; PRM_type[prmmg2_22]                 = TE_type; /* Maximale groentijd MG2 22                                                      */
    PRM_code[prmmg2_41]                 = "mg2_41";                 PRM[prmmg2_41]                 = 200; PRM_type[prmmg2_41]                 = TE_type; /* Maximale groentijd MG2 41                                                      */
    PRM_code[prmmg2_42]                 = "mg2_42";                 PRM[prmmg2_42]                 = 200; PRM_type[prmmg2_42]                 = TE_type; /* Maximale groentijd MG2 42                                                      */
    PRM_code[prmrisapproachid41]        = "risapproachid41";        PRM[prmrisapproachid41]        = 1;                                                  /* Approach id voor fase 41                                                       */
    PRM_code[prmrisapproachid42]        = "risapproachid42";        PRM[prmrisapproachid42]        = 2;                                                  /* Approach id voor fase 42                                                       */
    PRM_code[prmrisapproachid22]        = "risapproachid22";        PRM[prmrisapproachid22]        = 3;                                                  /* Approach id voor fase 22                                                       */
    PRM_code[prmrislaneid41_1]          = "rislaneid41_1";          PRM[prmrislaneid41_1]          = 1;                                                  /* Lane id voor lane met index 1 bij fase 41                                      */
    PRM_code[prmrislaneid41_2]          = "rislaneid41_2";          PRM[prmrislaneid41_2]          = 2;                                                  /* Lane id voor lane met index 2 bij fase 41                                      */
    PRM_code[prmrislaneid42_1]          = "rislaneid42_1";          PRM[prmrislaneid42_1]          = 3;                                                  /* Lane id voor lane met index 1 bij fase 42                                      */
    PRM_code[prmrislaneid22_1]          = "rislaneid22_1";          PRM[prmrislaneid22_1]          = 4;                                                  /* Lane id voor lane met index 1 bij fase 22                                      */
    PRM_code[prmrispstart22fts1]        = "rispstart22fts1";        PRM[prmrispstart22fts1]        = 0;                                                  /* Afstand van stopstreep tot start peloton bepaal gebied RIS fase 22             */
    PRM_code[prmrispend22fts1]          = "rispend22fts1";          PRM[prmrispend22fts1]          = 100;                                                /* Afstand van stopstreep tot einde peloton bepaal gebied RIS fase 22             */
    PRM_code[prmmlfpr02]                = "mlfpr02";                PRM[prmmlfpr02]                = 0;                                                  /* Maximaal aantal modules vooruit fase 02                                        */
    PRM_code[prmmlfpr22]                = "mlfpr22";                PRM[prmmlfpr22]                = 0;                                                  /* Maximaal aantal modules vooruit fase 22                                        */
    PRM_code[prmmlfpr41]                = "mlfpr41";                PRM[prmmlfpr41]                = 0;                                                  /* Maximaal aantal modules vooruit fase 41                                        */
    PRM_code[prmmlfpr42]                = "mlfpr42";                PRM[prmmlfpr42]                = 0;                                                  /* Maximaal aantal modules vooruit fase 42                                        */
    PRM_code[prmaltg02]                 = "altg02";                 PRM[prmaltg02]                 = 0;   PRM_type[prmaltg02]                 = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    PRM_code[prmaltp02]                 = "altp02";                 PRM[prmaltp02]                 = 0;   PRM_type[prmaltp02]                 = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    PRM_code[prmaltg22]                 = "altg22";                 PRM[prmaltg22]                 = 0;   PRM_type[prmaltg22]                 = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 22                         */
    PRM_code[prmaltp22]                 = "altp22";                 PRM[prmaltp22]                 = 0;   PRM_type[prmaltp22]                 = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 22                            */
    PRM_code[prmaltg41]                 = "altg41";                 PRM[prmaltg41]                 = 0;   PRM_type[prmaltg41]                 = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 41                         */
    PRM_code[prmaltp41]                 = "altp41";                 PRM[prmaltp41]                 = 0;   PRM_type[prmaltp41]                 = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 41                            */
    PRM_code[prmaltg42]                 = "altg42";                 PRM[prmaltg42]                 = 0;   PRM_type[prmaltg42]                 = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 42                         */
    PRM_code[prmaltp42]                 = "altp42";                 PRM[prmaltp42]                 = 0;   PRM_type[prmaltp42]                 = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 42                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    PRM_code[prmtestdsivert]            = "testdsivert";            PRM[prmtestdsivert]            = 120; PRM_type[prmtestdsivert]            = TS_type; /* Testen vertraging in DSI bericht in testomgeving                               */
    PRM_code[prmtestdsilyn]             = "testdsilyn";             PRM[prmtestdsilyn]             = 0;                                       /* Testen lijnnummer DSI bericht in testomgeving                                  */
    PRM_code[prmtestdsicat]             = "testdsicat";             PRM[prmtestdsicat]             = 10;                                      /* Testen ritcategorie DSI bericht in testomgeving                                */
#endif


/* extra data */
/* ---------- */
    FC_type[fc02] = MVT_type;
    FC_type[fc22] = FTS_type;
    FC_type[fc41] = OV_type;
    FC_type[fc42] = OV_type;

/* Selectieve detectie */
/* ------------------- */
    DS_code[dsdummy] = "dsdummy";

/* modulen */
/* ------- */
    PRML[ML1][fc02] = PRIMAIR;

    PRML[ML2][fc22] = PRIMAIR;

    PRML[ML3][fc41] = PRIMAIR;

    PRML[ML4][fc42] = PRIMAIR;


/* Typen ingangen */
/* -------------- */
    IS_type[d021] = DKOP_type;
    IS_type[d022] = DLNG_type;
    IS_type[d221] = DKOP_type;
    IS_type[d222] = DVER_type;
    IS_type[dk22] = DK_type;
    IS_type[d411] = DKOP_type;
    IS_type[d412] = DLNG_type;
    IS_type[d421] = DKOP_type;
    IS_type[d422] = DLNG_type;

/* Typen uitgangen */
/* --------------- */
    US_type[fc02] = MVT_type;
    US_type[fc22] = FTS_type;
    US_type[fc41] = OV_type;
    US_type[fc42] = OV_type;

#ifdef VISSIM
#endif

    #include "GMtab.add"
}
