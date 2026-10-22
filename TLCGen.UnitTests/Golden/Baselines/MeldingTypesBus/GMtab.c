/* REGEL INSTELLINGEN */
/* ------------------ */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMtab.c
      CCOL:   10.0
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
   FC_code[fc05] = "05"; TRG_max[fc05] = 0; TRG_min[fc05] = 0; TGG_max[fc05] = 0; TGG_min[fc05] = 0; TFG_max[fc05] = 0; TGL_max[fc05] = 0; TGL_min[fc05] = 0; TMGL_max[fc05] = 0; TVG_max[fc05] = 300;
   FC_code[fc41] = "41"; TRG_max[fc41] = 0; TRG_min[fc41] = 0; TGG_max[fc41] = 0; TGG_min[fc41] = 0; TFG_max[fc41] = 0; TGL_max[fc41] = 0; TGL_min[fc41] = 0; TMGL_max[fc41] = 0; TVG_max[fc41] = 300;
   FC_code[fc42] = "42"; TRG_max[fc42] = 0; TRG_min[fc42] = 0; TGG_max[fc42] = 0; TGG_min[fc42] = 0; TFG_max[fc42] = 0; TGL_max[fc42] = 0; TGL_min[fc42] = 0; TMGL_max[fc42] = 0; TVG_max[fc42] = 300;

/* conflicten */
/* ---------- */

    TIG_max[fc02][fc05] = 30;
    TIG_max[fc02][fc41] = 30;
    TIG_max[fc02][fc42] = 30;

    TIG_max[fc05][fc02] = 30;
    TIG_max[fc05][fc41] = 30;
    TIG_max[fc05][fc42] = 30;

    TIG_max[fc41][fc02] = 30;
    TIG_max[fc41][fc05] = 30;
    TIG_max[fc41][fc42] = 30;

    TIG_max[fc42][fc02] = 30;
    TIG_max[fc42][fc05] = 30;
    TIG_max[fc42][fc41] = 30;

/* overige uitgangen */
/* ----------------- */
    US_code[ussegm1]      = "segm1";                             /* Aansturing segmenten display               */
    US_code[ussegm2]      = "segm2";                             /* Aansturing segmenten display               */
    US_code[ussegm3]      = "segm3";                             /* Aansturing segmenten display               */
    US_code[ussegm4]      = "segm4";                             /* Aansturing segmenten display               */
    US_code[ussegm5]      = "segm5";                             /* Aansturing segmenten display               */
    US_code[ussegm6]      = "segm6";                             /* Aansturing segmenten display               */
    US_code[ussegm7]      = "segm7";                             /* Aansturing segmenten display               */
    US_code[usmaxwt]      = "maxwt";                             /* Verklikken maximale wachttijd overschreden */
    US_code[uskarmelding] = "karmelding";                        /* Verklikken ontvangst melding KAR           */
    US_code[uskarog]      = "karog";                             /* Verklikken ondergedrag KAR                 */
    US_code[usovinm41bus] = "ovinm41bus";                        /* Verklikken inmelding OV fase 41            */
    US_code[usovinm42bus] = "ovinm42bus";                        /* Verklikken inmelding OV fase 42            */
    US_code[usperdef]     = "perdef";                            /* Default periode actief                     */
    US_code[usper1]       = "per1";                              /* Periode actief                             */

/* detectie */
/* -------- */
    D_code[d021]     = "021";     
    D_code[d022]     = "022";     
    D_code[d051]     = "051";     
    D_code[d052]     = "052";     
    D_code[d411]     = "411";     
    D_code[d412]     = "412";     
    D_code[d413]     = "413";     
    D_code[d414]     = "414";     
    D_code[d421]     = "421";     
    D_code[d422]     = "422";     
    D_code[dsd41in]  = "sd41in";  
    D_code[dsd41uit] = "sd41uit"; 
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined PRACTICE_TEST
    D_code[ddummykarin41bus]  = "dummykarin41bus";  
    D_code[ddummykaruit41bus] = "dummykaruit41bus"; 
#endif

/* overige ingangen */
/* ---------------- */
    IS_code[isov41in]  = "ov41in";                     
    IS_code[isov41uit] = "ov41uit";                    

/* hulp elementen */
/* -------------- */
    H_code[hprio41bus]                    = "prio41bus";                                                            /* Bijhouden actief zijn prioriteit fase 41 */
    H_code[hprioin41bus]                  = "prioin41bus";                                                          /* Prioriteit inmelding fase 41 Bus         */
    H_code[hpriouit41bus]                 = "priouit41bus";                                                         /* Prioriteit uitmelding 41 Bus             */
    H_code[hprioin41buskar]               = "prioin41buskar";                                                       /* Prioriteit inmelding fase 41 Bus         */
    H_code[hprioin41bussdsd41in]          = "prioin41bussdsd41in";                                                  /* Prioriteit inmelding fase 41 Bus         */
    H_code[hprioin41busdet412413]         = "prioin41busdet412413";                                                 /* Prioriteit inmelding fase 41 Bus         */
    H_code[hprioin41busdet413]            = "prioin41busdet413";                                                    /* Prioriteit inmelding fase 41 Bus         */
    H_code[hprioin41businpov41in]         = "prioin41businpov41in";                                                 /* Prioriteit inmelding fase 41 Bus         */
    H_code[hprioin41businpov41inov41uit]  = "prioin41businpov41inov41uit";                                          /* Prioriteit inmelding fase 41 Bus         */
    H_code[hprioin41busvecio411]          = "prioin41busvecio411";                                                  /* Prioriteit inmelding fase 41 Bus         */
    H_code[hprioin41busopti414]           = "prioin41busopti414";                                                   /* Prioriteit inmelding fase 41 Bus         */
    H_code[hpriouit41buskar]              = "priouit41buskar";                                                      /* Prioriteit uitmelding 41 Bus             */
    H_code[hpriouit41bussdsd41uit]        = "priouit41bussdsd41uit";                                                /* Prioriteit uitmelding 41 Bus             */
    H_code[hpriouit41busdet411]           = "priouit41busdet411";                                                   /* Prioriteit uitmelding 41 Bus             */
    H_code[hpriouit41busdet412413]        = "priouit41busdet412413";                                                /* Prioriteit uitmelding 41 Bus             */
    H_code[hpriouit41businpov41in]        = "priouit41businpov41in";                                                /* Prioriteit uitmelding 41 Bus             */
    H_code[hpriouit41businpov41uitov41in] = "priouit41businpov41uitov41in";                                         /* Prioriteit uitmelding 41 Bus             */
    H_code[hpriouit41busvecio414]         = "priouit41busvecio414";                                                 /* Prioriteit uitmelding 41 Bus             */
    H_code[hpriouit41busopti414]          = "priouit41busopti414";                                                  /* Prioriteit uitmelding 41 Bus             */
    H_code[hprio42bus]                    = "prio42bus";                                                            /* Bijhouden actief zijn prioriteit fase 42 */
    H_code[hprioin42bus]                  = "prioin42bus";                                                          /* Prioriteit inmelding fase 42 Bus         */
    H_code[hpriouit42bus]                 = "priouit42bus";                                                         /* Prioriteit uitmelding 42 Bus             */
    H_code[hprioin42busopti421]           = "prioin42busopti421";                                                   /* Prioriteit inmelding fase 42 Bus         */
    H_code[hpriouit42busopti422]          = "priouit42busopti422";                                                  /* Prioriteit uitmelding 42 Bus             */
    H_code[hpriouit42busopti423]          = "priouit42busopti423";                                                  /* Prioriteit uitmelding 42 Bus             */

/* geheugen elementen */
/* ------------------ */
    MM_code[mperiod] = "period";                   /* Onthouden actieve periode   */
    MM_code[mar02]   = "ar02";                     /* Alternatieve ruimte fase 02 */
    MM_code[mar05]   = "ar05";                     /* Alternatieve ruimte fase 05 */
    MM_code[mar41]   = "ar41";                     /* Alternatieve ruimte fase 41 */
    MM_code[mar42]   = "ar42";                     /* Alternatieve ruimte fase 42 */

/* tijd elementen */
/* -------------- */
    T_code[tkm02]               = "km02";               T_max[tkm02]               = 0;    T_type[tkm02]               = TE_type; /* Kop maximum voor detector 02                          */
    T_code[tkm05]               = "km05";               T_max[tkm05]               = 0;    T_type[tkm05]               = TE_type; /* Kop maximum voor detector 05                          */
    T_code[tkm41]               = "km41";               T_max[tkm41]               = 0;    T_type[tkm41]               = TE_type; /* Kop maximum voor detector 41                          */
    T_code[tkm42]               = "km42";               T_max[tkm42]               = 0;    T_type[tkm42]               = TE_type; /* Kop maximum voor detector 42                          */
    T_code[tkarmelding]         = "karmelding";         T_max[tkarmelding]         = 15;   T_type[tkarmelding]         = TE_type; /* Duur verklikking ontvangst melding KAR                */
    T_code[tkarog]              = "karog";              T_max[tkarog]              = 1440; T_type[tkarog]              = TM_type; /* Ondergedrag KAR                                       */
    T_code[tprioin41buskar]     = "prioin41buskar";     T_max[tprioin41buskar]     = 50;   T_type[tprioin41buskar]     = TE_type; /* Anti jutter tijd inmelden 41 Bus                      */
    T_code[tprioin41busdet413]  = "prioin41busdet413";  T_max[tprioin41busdet413]  = 30;   T_type[tprioin41busdet413]  = TE_type; /* Anti jutter tijd inmelden 41 Bus                      */
    T_code[tprioin41busopti414] = "prioin41busopti414"; T_max[tprioin41busopti414] = 20;   T_type[tprioin41busopti414] = TE_type; /* Anti jutter tijd inmelden 41 Bus                      */
    T_code[tbtovg41bus]         = "btovg41bus";         T_max[tbtovg41bus]         = 30;   T_type[tbtovg41bus]         = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    T_code[trt41bus]            = "rt41bus";            T_max[trt41bus]            = 0;    T_type[trt41bus]            = TE_type; /* Actuele rijtijd prio fase 41                          */
    T_code[tgb41bus]            = "gb41bus";            T_max[tgb41bus]            = 600;  T_type[tgb41bus]            = TE_type; /* Groenbewaking prioriteit fase 41                      */
    T_code[tblk41bus]           = "blk41bus";           T_max[tblk41bus]           = 200;  T_type[tblk41bus]           = TE_type; /* Blokkeertijd na prioriteitsingreep fase 41            */
    T_code[tbtovg42bus]         = "btovg42bus";         T_max[tbtovg42bus]         = 30;   T_type[tbtovg42bus]         = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 42 */
    T_code[trt42bus]            = "rt42bus";            T_max[trt42bus]            = 0;    T_type[trt42bus]            = TE_type; /* Actuele rijtijd prio fase 42                          */
    T_code[tgb42bus]            = "gb42bus";            T_max[tgb42bus]            = 600;  T_type[tgb42bus]            = TE_type; /* Groenbewaking prioriteit fase 42                      */
    T_code[tblk42bus]           = "blk42bus";           T_max[tblk42bus]           = 200;  T_type[tblk42bus]           = TE_type; /* Blokkeertijd na prioriteitsingreep fase 42            */

/* teller elementen */
/* ---------------- */
    C_code[cvc41bus] = "vc41bus"; C_max[cvc41bus] = 999; C_type[cvc41bus] = CT_type; /* Bijhouden prio inmeldingen fase 41 type Bus */
    C_code[cvc42bus] = "vc42bus"; C_max[cvc42bus] = 999; C_type[cvc42bus] = CT_type; /* Bijhouden prio inmeldingen fase 42 type Bus */

/* schakelaars */
/* ----------- */
    SCH_code[schcheckdstype]                       = "checkdstype";                       SCH[schcheckdstype]                       = 0;                                                    /* Check type DSI bericht bij VECOM         */
    SCH_code[schprioin41buskar]                    = "prioin41buskar";                    SCH[schprioin41buskar]                    = 1;                                                    /* Inmelden 41 via Bus toestaan             */
    SCH_code[schprioin41bussdsd41inSD]             = "prioin41bussdsd41inSD";             SCH[schprioin41bussdsd41inSD]             = 1;                                                    /* Inmelden 41 via Bus toestaan             */
    SCH_code[schprioin41busdet412D413DB]           = "prioin41busdet412D413DB";           SCH[schprioin41busdet412D413DB]           = 1;                                                    /* Inmelden 41 via Bus toestaan             */
    SCH_code[schprioin41busdet414ETDH]             = "prioin41busdet414ETDH";             SCH[schprioin41busdet414ETDH]             = 1;                                                    /* Inmelden 41 via Bus toestaan             */
    SCH_code[schprioin41busdet413SDB]              = "prioin41busdet413SDB";              SCH[schprioin41busdet413SDB]              = 1;                                                    /* Inmelden 41 via Bus toestaan             */
    SCH_code[schprioin41businpov41inSIS]           = "prioin41businpov41inSIS";           SCH[schprioin41businpov41inSIS]           = 1;                                                    /* Inmelden 41 via Bus toestaan             */
    SCH_code[schprioin41businpov41inISov41uitSIS]  = "prioin41businpov41inISov41uitSIS";  SCH[schprioin41businpov41inISov41uitSIS]  = 1;                                                    /* Inmelden 41 via Bus toestaan             */
    SCH_code[schprioin41busvecio411SD]             = "prioin41busvecio411SD";             SCH[schprioin41busvecio411SD]             = 1;                                                    /* Inmelden 41 via Bus toestaan             */
    SCH_code[schprioin41busopti414DB]              = "prioin41busopti414DB";              SCH[schprioin41busopti414DB]              = 1;                                                    /* Inmelden 41 via Bus toestaan             */
    SCH_code[schpriouit41buskar]                   = "priouit41buskar";                   SCH[schpriouit41buskar]                   = 1;                                                    /* Uitmelden 41 via Bus toestaan            */
    SCH_code[schpriouit41bussdsd41uitSD]           = "priouit41bussdsd41uitSD";           SCH[schpriouit41bussdsd41uitSD]           = 1;                                                    /* Uitmelden 41 via Bus toestaan            */
    SCH_code[schpriouit41busdet411D]               = "priouit41busdet411D";               SCH[schpriouit41busdet411D]               = 1;                                                    /* Uitmelden 41 via Bus toestaan            */
    SCH_code[schpriouit41busdet412ED413ETDH]       = "priouit41busdet412ED413ETDH";       SCH[schpriouit41busdet412ED413ETDH]       = 1;                                                    /* Uitmelden 41 via Bus toestaan            */
    SCH_code[schpriouit41businpov41inEIS]          = "priouit41businpov41inEIS";          SCH[schpriouit41businpov41inEIS]          = 1;                                                    /* Uitmelden 41 via Bus toestaan            */
    SCH_code[schpriouit41businpov41uitEISov41inIS] = "priouit41businpov41uitEISov41inIS"; SCH[schpriouit41businpov41uitEISov41inIS] = 1;                                                    /* Uitmelden 41 via Bus toestaan            */
    SCH_code[schpriouit41busvecio414SD]            = "priouit41busvecio414SD";            SCH[schpriouit41busvecio414SD]            = 1;                                                    /* Uitmelden 41 via Bus toestaan            */
    SCH_code[schpriouit41busopti414DB]             = "priouit41busopti414DB";             SCH[schpriouit41busopti414DB]             = 1;                                                    /* Uitmelden 41 via Bus toestaan            */
    SCH_code[schprioin42busopti421DB]              = "prioin42busopti421DB";              SCH[schprioin42busopti421DB]              = 1;                                                    /* Inmelden 42 via Bus toestaan             */
    SCH_code[schpriouit42busopti422DB]             = "priouit42busopti422DB";             SCH[schpriouit42busopti422DB]             = 1;                                                    /* Uitmelden 42 via Bus toestaan            */
    SCH_code[schpriouit42busopti423DB]             = "priouit42busopti423DB";             SCH[schpriouit42busopti423DB]             = 1;                                                    /* Uitmelden 42 via Bus toestaan            */
    SCH_code[schaltg02]                            = "altg02";                            SCH[schaltg02]                            = 0;                                                    /* Alternatieve realisatie toestaan fase 02 */
    SCH_code[schaltg05]                            = "altg05";                            SCH[schaltg05]                            = 0;                                                    /* Alternatieve realisatie toestaan fase 05 */
    SCH_code[schaltg41]                            = "altg41";                            SCH[schaltg41]                            = 0;                                                    /* Alternatieve realisatie toestaan fase 41 */
    SCH_code[schaltg42]                            = "altg42";                            SCH[schaltg42]                            = 0;                                                    /* Alternatieve realisatie toestaan fase 42 */

/* parameters */
/* ---------- */
    PRM_code[prmfb]           = "fb";           PRM[prmfb]           = 0;   PRM_type[prmfb]           = TS_type; /* Instelling fasebewaking                                                        */
    PRM_code[prmxx]           = "xx";           PRM[prmxx]           = 0;                                        /* Versiebeheer xx                                                                */
    PRM_code[prmyy]           = "yy";           PRM[prmyy]           = 0;                                        /* Versiebeheer yy                                                                */
    PRM_code[prmzz]           = "zz";           PRM[prmzz]           = 0;                                        /* Versiebeheer zz                                                                */
    PRM_code[prmmk021]        = "mk021";        PRM[prmmk021]        = 0;                                        /* Meetkriterium type voor detector 021                                           */
    PRM_code[prmmk022]        = "mk022";        PRM[prmmk022]        = 0;                                        /* Meetkriterium type voor detector 022                                           */
    PRM_code[prmmk051]        = "mk051";        PRM[prmmk051]        = 0;                                        /* Meetkriterium type voor detector 051                                           */
    PRM_code[prmmk052]        = "mk052";        PRM[prmmk052]        = 0;                                        /* Meetkriterium type voor detector 052                                           */
    PRM_code[prmmk411]        = "mk411";        PRM[prmmk411]        = 0;                                        /* Meetkriterium type voor detector 411                                           */
    PRM_code[prmmk412]        = "mk412";        PRM[prmmk412]        = 0;                                        /* Meetkriterium type voor detector 412                                           */
    PRM_code[prmmk413]        = "mk413";        PRM[prmmk413]        = 0;                                        /* Meetkriterium type voor detector 413                                           */
    PRM_code[prmmk414]        = "mk414";        PRM[prmmk414]        = 0;                                        /* Meetkriterium type voor detector 414                                           */
    PRM_code[prmmk421]        = "mk421";        PRM[prmmk421]        = 0;                                        /* Meetkriterium type voor detector 421                                           */
    PRM_code[prmmk422]        = "mk422";        PRM[prmmk422]        = 0;                                        /* Meetkriterium type voor detector 422                                           */
    PRM_code[prmmwta]         = "mwta";         PRM[prmmwta]         = 0;   PRM_type[prmmwta]         = TS_type; /* Maximale wachttijd autoverkeer                                                 */
    PRM_code[prmmwtfts]       = "mwtfts";       PRM[prmmwtfts]       = 0;   PRM_type[prmmwtfts]       = TS_type; /* Maximale wachttijd fiets                                                       */
    PRM_code[prmmwtvtg]       = "mwtvtg";       PRM[prmmwtvtg]       = 0;   PRM_type[prmmwtvtg]       = TS_type; /* Maximale wachttijd voetgangers                                                 */
    PRM_code[prmpmgt02]       = "pmgt02";       PRM[prmpmgt02]       = 0;                                        /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                  */
    PRM_code[prmognt02]       = "ognt02";       PRM[prmognt02]       = 0;   PRM_type[prmognt02]       = TE_type; /* Minimale groentijd bij terugkomen fase 02                                      */
    PRM_code[prmnofm02]       = "nofm02";       PRM[prmnofm02]       = 0;   PRM_type[prmnofm02]       = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 02                               */
    PRM_code[prmmgcov02]      = "mgcov02";      PRM[prmmgcov02]      = 0;   PRM_type[prmmgcov02]      = TE_type; /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov02]     = "pmgcov02";     PRM[prmpmgcov02]     = 0;                                        /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg02]      = "ohpmg02";      PRM[prmohpmg02]      = 0;                                        /* Percentage ophogen groentijd na afkappen fase 02                               */
    PRM_code[prmpmgt05]       = "pmgt05";       PRM[prmpmgt05]       = 0;                                        /* Minimaal percentage groentijd primair tbv. terugkomen fase 05                  */
    PRM_code[prmognt05]       = "ognt05";       PRM[prmognt05]       = 0;   PRM_type[prmognt05]       = TE_type; /* Minimale groentijd bij terugkomen fase 05                                      */
    PRM_code[prmnofm05]       = "nofm05";       PRM[prmnofm05]       = 0;   PRM_type[prmnofm05]       = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 05                               */
    PRM_code[prmmgcov05]      = "mgcov05";      PRM[prmmgcov05]      = 0;   PRM_type[prmmgcov05]      = TE_type; /* Minimum groentijd waarna fase 05 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov05]     = "pmgcov05";     PRM[prmpmgcov05]     = 0;                                        /* Minimum percentage groentijd waarna fase 05 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg05]      = "ohpmg05";      PRM[prmohpmg05]      = 0;                                        /* Percentage ophogen groentijd na afkappen fase 05                               */
    PRM_code[prmpmgt41]       = "pmgt41";       PRM[prmpmgt41]       = 0;                                        /* Minimaal percentage groentijd primair tbv. terugkomen fase 41                  */
    PRM_code[prmognt41]       = "ognt41";       PRM[prmognt41]       = 0;   PRM_type[prmognt41]       = TE_type; /* Minimale groentijd bij terugkomen fase 41                                      */
    PRM_code[prmnofm41]       = "nofm41";       PRM[prmnofm41]       = 0;   PRM_type[prmnofm41]       = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 41                               */
    PRM_code[prmmgcov41]      = "mgcov41";      PRM[prmmgcov41]      = 0;   PRM_type[prmmgcov41]      = TE_type; /* Minimum groentijd waarna fase 41 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov41]     = "pmgcov41";     PRM[prmpmgcov41]     = 0;                                        /* Minimum percentage groentijd waarna fase 41 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg41]      = "ohpmg41";      PRM[prmohpmg41]      = 0;                                        /* Percentage ophogen groentijd na afkappen fase 41                               */
    PRM_code[prmpmgt42]       = "pmgt42";       PRM[prmpmgt42]       = 0;                                        /* Minimaal percentage groentijd primair tbv. terugkomen fase 42                  */
    PRM_code[prmognt42]       = "ognt42";       PRM[prmognt42]       = 0;   PRM_type[prmognt42]       = TE_type; /* Minimale groentijd bij terugkomen fase 42                                      */
    PRM_code[prmnofm42]       = "nofm42";       PRM[prmnofm42]       = 0;   PRM_type[prmnofm42]       = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 42                               */
    PRM_code[prmmgcov42]      = "mgcov42";      PRM[prmmgcov42]      = 0;   PRM_type[prmmgcov42]      = TE_type; /* Minimum groentijd waarna fase 42 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov42]     = "pmgcov42";     PRM[prmpmgcov42]     = 0;                                        /* Minimum percentage groentijd waarna fase 42 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg42]      = "ohpmg42";      PRM[prmohpmg42]      = 0;                                        /* Percentage ophogen groentijd na afkappen fase 42                               */
    PRM_code[prmrto41bus]     = "rto41bus";     PRM[prmrto41bus]     = 150; PRM_type[prmrto41bus]     = TE_type; /* Ongehinderde rijtijd prioriteit fase 41                                        */
    PRM_code[prmrtbg41bus]    = "rtbg41bus";    PRM[prmrtbg41bus]    = 200; PRM_type[prmrtbg41bus]    = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    PRM_code[prmrtg41bus]     = "rtg41bus";     PRM[prmrtg41bus]     = 300; PRM_type[prmrtg41bus]     = TE_type; /* Gehinderde rijtijd prioriteit fase 41                                          */
    PRM_code[prmomx41bus]     = "omx41bus";     PRM[prmomx41bus]     = 50;  PRM_type[prmomx41bus]     = TE_type; /* Ondermaximum OV fase 41                                                        */
    PRM_code[prmupinagb41bus] = "upinagb41bus"; PRM[prmupinagb41bus] = 0;                                        /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    PRM_code[prmvtgcat41bus]  = "vtgcat41bus";  PRM[prmvtgcat41bus]  = 1;                                        /* Voertuigcategorie DSI voor prio ingreep 41bus                                  */
    PRM_code[prmprio41bus]    = "prio41bus";    PRM[prmprio41bus]    = 0;                                        /* Prioriteitsinstelling OV fase 41                                               */
    PRM_code[prmrto42bus]     = "rto42bus";     PRM[prmrto42bus]     = 150; PRM_type[prmrto42bus]     = TE_type; /* Ongehinderde rijtijd prioriteit fase 42                                        */
    PRM_code[prmrtbg42bus]    = "rtbg42bus";    PRM[prmrtbg42bus]    = 200; PRM_type[prmrtbg42bus]    = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 42                                  */
    PRM_code[prmrtg42bus]     = "rtg42bus";     PRM[prmrtg42bus]     = 300; PRM_type[prmrtg42bus]     = TE_type; /* Gehinderde rijtijd prioriteit fase 42                                          */
    PRM_code[prmomx42bus]     = "omx42bus";     PRM[prmomx42bus]     = 50;  PRM_type[prmomx42bus]     = TE_type; /* Ondermaximum OV fase 42                                                        */
    PRM_code[prmupinagb42bus] = "upinagb42bus"; PRM[prmupinagb42bus] = 0;                                        /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 42                  */
    PRM_code[prmprio42bus]    = "prio42bus";    PRM[prmprio42bus]    = 0;                                        /* Prioriteitsinstelling OV fase 42                                               */
    PRM_code[prmstkp1]        = "stkp1";        PRM[prmstkp1]        = 600; PRM_type[prmstkp1]        = TI_type; /* Start klokperiode ochtend                                                      */
    PRM_code[prmetkp1]        = "etkp1";        PRM[prmetkp1]        = 900; PRM_type[prmetkp1]        = TI_type; /* Einde klokperiode ochtend                                                      */
    PRM_code[prmdckp1]        = "dckp1";        PRM[prmdckp1]        = 8;                                        /* Dagsoort klokperiode ochtend                                                   */
    PRM_code[prmmg1_02]       = "mg1_02";       PRM[prmmg1_02]       = 300; PRM_type[prmmg1_02]       = TE_type; /* Maximale groentijd MG1 02                                                      */
    PRM_code[prmmg1_05]       = "mg1_05";       PRM[prmmg1_05]       = 300; PRM_type[prmmg1_05]       = TE_type; /* Maximale groentijd MG1 05                                                      */
    PRM_code[prmmg1_41]       = "mg1_41";       PRM[prmmg1_41]       = 300; PRM_type[prmmg1_41]       = TE_type; /* Maximale groentijd MG1 41                                                      */
    PRM_code[prmmg1_42]       = "mg1_42";       PRM[prmmg1_42]       = 300; PRM_type[prmmg1_42]       = TE_type; /* Maximale groentijd MG1 42                                                      */
    PRM_code[prmmg2_02]       = "mg2_02";       PRM[prmmg2_02]       = 200; PRM_type[prmmg2_02]       = TE_type; /* Maximale groentijd MG2 02                                                      */
    PRM_code[prmmg2_05]       = "mg2_05";       PRM[prmmg2_05]       = 200; PRM_type[prmmg2_05]       = TE_type; /* Maximale groentijd MG2 05                                                      */
    PRM_code[prmmg2_41]       = "mg2_41";       PRM[prmmg2_41]       = 200; PRM_type[prmmg2_41]       = TE_type; /* Maximale groentijd MG2 41                                                      */
    PRM_code[prmmg2_42]       = "mg2_42";       PRM[prmmg2_42]       = 200; PRM_type[prmmg2_42]       = TE_type; /* Maximale groentijd MG2 42                                                      */
    PRM_code[prmmlfpr02]      = "mlfpr02";      PRM[prmmlfpr02]      = 0;                                        /* Maximaal aantal modules vooruit fase 02                                        */
    PRM_code[prmmlfpr05]      = "mlfpr05";      PRM[prmmlfpr05]      = 0;                                        /* Maximaal aantal modules vooruit fase 05                                        */
    PRM_code[prmmlfpr41]      = "mlfpr41";      PRM[prmmlfpr41]      = 0;                                        /* Maximaal aantal modules vooruit fase 41                                        */
    PRM_code[prmmlfpr42]      = "mlfpr42";      PRM[prmmlfpr42]      = 0;                                        /* Maximaal aantal modules vooruit fase 42                                        */
    PRM_code[prmaltg02]       = "altg02";       PRM[prmaltg02]       = 0;   PRM_type[prmaltg02]       = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    PRM_code[prmaltp02]       = "altp02";       PRM[prmaltp02]       = 0;   PRM_type[prmaltp02]       = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    PRM_code[prmaltg05]       = "altg05";       PRM[prmaltg05]       = 0;   PRM_type[prmaltg05]       = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 05                         */
    PRM_code[prmaltp05]       = "altp05";       PRM[prmaltp05]       = 0;   PRM_type[prmaltp05]       = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 05                            */
    PRM_code[prmaltg41]       = "altg41";       PRM[prmaltg41]       = 0;   PRM_type[prmaltg41]       = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 41                         */
    PRM_code[prmaltp41]       = "altp41";       PRM[prmaltp41]       = 0;   PRM_type[prmaltp41]       = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 41                            */
    PRM_code[prmaltg42]       = "altg42";       PRM[prmaltg42]       = 0;   PRM_type[prmaltg42]       = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 42                         */
    PRM_code[prmaltp42]       = "altp42";       PRM[prmaltp42]       = 0;   PRM_type[prmaltp42]       = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 42                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    PRM_code[prmtestdsivert]  = "testdsivert";  PRM[prmtestdsivert]  = 120; PRM_type[prmtestdsivert]  = TS_type; /* Testen vertraging in DSI bericht in testomgeving                               */
    PRM_code[prmtestdsilyn]   = "testdsilyn";   PRM[prmtestdsilyn]   = 0;                             /* Testen lijnnummer DSI bericht in testomgeving                                  */
    PRM_code[prmtestdsicat]   = "testdsicat";   PRM[prmtestdsicat]   = 10;                            /* Testen ritcategorie DSI bericht in testomgeving                                */
#endif


/* extra data */
/* ---------- */
    FC_type[fc02] = MVT_type;
    FC_type[fc05] = MVT_type;
    FC_type[fc41] = OV_type;
    FC_type[fc42] = OV_type;

/* Selectieve detectie */
/* ------------------- */
    DS_code[dsdummy] = "dsdummy";
    DS_code[DSD41IN]  = "SD41IN";
    DS_code[DSD41UIT]  = "SD41UIT";

/* modulen */
/* ------- */
    PRML[ML1][fc02] = PRIMAIR;

    PRML[ML2][fc05] = PRIMAIR;

    PRML[ML3][fc41] = PRIMAIR;

    PRML[ML4][fc42] = PRIMAIR;


/* Typen ingangen */
/* -------------- */
    IS_type[d021] = DKOP_type;
    IS_type[d022] = DLNG_type;
    IS_type[d051] = DKOP_type;
    IS_type[d052] = DLNG_type;
    IS_type[d411] = DKOP_type;
    IS_type[d412] = DLNG_type;
    IS_type[d413] = DL_type;
    IS_type[d414] = DL_type;
    IS_type[d421] = DKOP_type;
    IS_type[d422] = DLNG_type;
    IS_type[dsd41in] = DKOP_type;
    IS_type[dsd41uit] = DKOP_type;

/* Typen uitgangen */
/* --------------- */
    US_type[fc02] = MVT_type;
    US_type[fc05] = MVT_type;
    US_type[fc41] = OV_type;
    US_type[fc42] = OV_type;

#ifdef VISSIM
#endif

    #include "GMtab.add"
}
