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
    US_code[ussegm1]           = "segm1";                                       /* Aansturing segmenten display               */
    US_code[ussegm2]           = "segm2";                                       /* Aansturing segmenten display               */
    US_code[ussegm3]           = "segm3";                                       /* Aansturing segmenten display               */
    US_code[ussegm4]           = "segm4";                                       /* Aansturing segmenten display               */
    US_code[ussegm5]           = "segm5";                                       /* Aansturing segmenten display               */
    US_code[ussegm6]           = "segm6";                                       /* Aansturing segmenten display               */
    US_code[ussegm7]           = "segm7";                                       /* Aansturing segmenten display               */
    US_code[usovtevroeg41tram] = "ovtevroeg41tram";                             /* Voorste OV voertuig bij 41 te vroeg Tram   */
    US_code[usovoptijd41tram]  = "ovoptijd41tram";                              /* Voorste OV voertuig bij 41 op tijd Tram    */
    US_code[usovtelaat41tram]  = "ovtelaat41tram";                              /* Voorste OV voertuig bij 41 te laat Tram    */
    US_code[usmaxwt]           = "maxwt";                                       /* Verklikken maximale wachttijd overschreden */
    US_code[uskarmelding]      = "karmelding";                                  /* Verklikken ontvangst melding KAR           */
    US_code[uskarog]           = "karog";                                       /* Verklikken ondergedrag KAR                 */
    US_code[usovinm41tram]     = "ovinm41tram";                                 /* Verklikken inmelding OV fase 41            */
    US_code[usovinm42bus]      = "ovinm42bus";                                  /* Verklikken inmelding OV fase 42            */
    US_code[usperdef]          = "perdef";                                      /* Default periode actief                     */
    US_code[usper1]            = "per1";                                        /* Periode actief                             */

/* detectie */
/* -------- */
    D_code[d021]  = "021";  
    D_code[d022]  = "022";  
    D_code[d051]  = "051";  
    D_code[d052]  = "052";  
    D_code[d411]  = "411";  
    D_code[d412]  = "412";  
    D_code[d421]  = "421";  
    D_code[d422]  = "422";  
    D_code[dwis1] = "wis1"; 
    D_code[dsd42] = "sd42"; 
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined PRACTICE_TEST
    D_code[ddummykarin41tram]  = "dummykarin41tram";  
    D_code[ddummykaruit41tram] = "dummykaruit41tram"; 
#endif

/* overige ingangen */
/* ---------------- */
    IS_code[iswisc1] = "wisc1";                  

/* hulp elementen */
/* -------------- */
    H_code[hstp41tram]           = "stp41tram";                                          /* Geconditioneerde prio OV mogelijk bij 41 Tram */
    H_code[hprio41tram]          = "prio41tram";                                         /* Bijhouden actief zijn prioriteit fase 41      */
    H_code[hprioin41tram]        = "prioin41tram";                                       /* Prioriteit inmelding fase 41 Tram             */
    H_code[hpriouit41tram]       = "priouit41tram";                                      /* Prioriteit uitmelding 41 Tram                 */
    H_code[hprioin41tramkar]     = "prioin41tramkar";                                    /* Prioriteit inmelding fase 41 Tram             */
    H_code[hprioin41tramdet411]  = "prioin41tramdet411";                                 /* Prioriteit inmelding fase 41 Tram             */
    H_code[hpriouit41tramkar]    = "priouit41tramkar";                                   /* Prioriteit uitmelding 41 Tram                 */
    H_code[hpriouit41tramdet412] = "priouit41tramdet412";                                /* Prioriteit uitmelding 41 Tram                 */
    H_code[hwissel41tram]        = "wissel41tram";                                       /* Onthouden voorwaarde fase 41                  */
    H_code[hprio42bus]           = "prio42bus";                                          /* Bijhouden actief zijn prioriteit fase 42      */
    H_code[hprioin42bus]         = "prioin42bus";                                        /* Prioriteit inmelding fase 42 Bus              */
    H_code[hpriouit42bus]        = "priouit42bus";                                       /* Prioriteit uitmelding 42 Bus                  */
    H_code[hprioin42bussdsd42]   = "prioin42bussdsd42";                                  /* Prioriteit inmelding fase 42 Bus              */
    H_code[hpriouit42bussdsd42]  = "priouit42bussdsd42";                                 /* Prioriteit uitmelding 42 Bus                  */

/* geheugen elementen */
/* ------------------ */
    MM_code[mperiod]    = "period";                         /* Onthouden actieve periode                 */
    MM_code[mstp41tram] = "stp41tram";                      /* Stiptheid voorste OV voertuig bij 41 Tram */
    MM_code[mar02]      = "ar02";                           /* Alternatieve ruimte fase 02               */
    MM_code[mar05]      = "ar05";                           /* Alternatieve ruimte fase 05               */
    MM_code[mar41]      = "ar41";                           /* Alternatieve ruimte fase 41               */
    MM_code[mar42]      = "ar42";                           /* Alternatieve ruimte fase 42               */

/* tijd elementen */
/* -------------- */
    T_code[tkm02]               = "km02";               T_max[tkm02]               = 0;    T_type[tkm02]               = TE_type; /* Kop maximum voor detector 02                                                           */
    T_code[tkm05]               = "km05";               T_max[tkm05]               = 0;    T_type[tkm05]               = TE_type; /* Kop maximum voor detector 05                                                           */
    T_code[tkm41]               = "km41";               T_max[tkm41]               = 0;    T_type[tkm41]               = TE_type; /* Kop maximum voor detector 41                                                           */
    T_code[tkm42]               = "km42";               T_max[tkm42]               = 0;    T_type[tkm42]               = TE_type; /* Kop maximum voor detector 42                                                           */
    T_code[tkarmelding]         = "karmelding";         T_max[tkarmelding]         = 15;   T_type[tkarmelding]         = TE_type; /* Duur verklikking ontvangst melding KAR                                                 */
    T_code[tkarog]              = "karog";              T_max[tkarog]              = 1440; T_type[tkarog]              = TM_type; /* Ondergedrag KAR                                                                        */
    T_code[tprioin41tramdet411] = "prioin41tramdet411"; T_max[tprioin41tramdet411] = 40;   T_type[tprioin41tramdet411] = TE_type; /* Anti jutter tijd inmelden 41 Tram                                                      */
    T_code[tprioin41tram]       = "prioin41tram";       T_max[tprioin41tram]       = 100;  T_type[tprioin41tram]       = TE_type; /* Anti jutter tijd inmelden 41 Tram                                                      */
    T_code[tpriouit41tram]      = "priouit41tram";      T_max[tpriouit41tram]      = 50;   T_type[tpriouit41tram]      = TE_type; /* Anti jutter tijd uitmelden 41                                                          */
    T_code[tbtovg41tram]        = "btovg41tram";        T_max[tbtovg41tram]        = 30;   T_type[tbtovg41tram]        = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 41                                  */
    T_code[trt41tram]           = "rt41tram";           T_max[trt41tram]           = 0;    T_type[trt41tram]           = TE_type; /* Actuele rijtijd prio fase 41                                                           */
    T_code[tgb41tram]           = "gb41tram";           T_max[tgb41tram]           = 600;  T_type[tgb41tram]           = TE_type; /* Groenbewaking prioriteit fase 41                                                       */
    T_code[tblk41tram]          = "blk41tram";          T_max[tblk41tram]          = 200;  T_type[tblk41tram]          = TE_type; /* Blokkeertijd na prioriteitsingreep fase 41                                             */
    T_code[tovminrood41tram]    = "ovminrood41tram";    T_max[tovminrood41tram]    = 30;   T_type[tovminrood41tram]    = TE_type; /* Minimale roodtijd vooraf aan inmelden mogelijk bij 41 (indien ingesteld per inmelding) */
    T_code[tprioin42bus]        = "prioin42bus";        T_max[tprioin42bus]        = 60;   T_type[tprioin42bus]        = TE_type; /* Anti jutter tijd inmelden 42 Bus                                                       */
    T_code[tbtovg42bus]         = "btovg42bus";         T_max[tbtovg42bus]         = 30;   T_type[tbtovg42bus]         = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 42                                  */
    T_code[trt42bus]            = "rt42bus";            T_max[trt42bus]            = 0;    T_type[trt42bus]            = TE_type; /* Actuele rijtijd prio fase 42                                                           */
    T_code[tgb42bus]            = "gb42bus";            T_max[tgb42bus]            = 600;  T_type[tgb42bus]            = TE_type; /* Groenbewaking prioriteit fase 42                                                       */
    T_code[tblk42bus]           = "blk42bus";           T_max[tblk42bus]           = 200;  T_type[tblk42bus]           = TE_type; /* Blokkeertijd na prioriteitsingreep fase 42                                             */

/* teller elementen */
/* ---------------- */
    C_code[cvc41tram] = "vc41tram"; C_max[cvc41tram] = 999; C_type[cvc41tram] = CT_type; /* Bijhouden prio inmeldingen fase 41 type Tram */
    C_code[cvc42bus]  = "vc42bus";  C_max[cvc42bus]  = 999; C_type[cvc42bus]  = CT_type; /* Bijhouden prio inmeldingen fase 42 type Bus  */

/* schakelaars */
/* ----------- */
    SCH_code[schovstipt41tram]         = "ovstipt41tram";         SCH[schovstipt41tram]         = 1;                                        /* Geconditioneerde prioteit voor OV bij 41 Tram */
    SCH_code[schgeenwisselwis1]        = "geenwisselwis1";        SCH[schgeenwisselwis1]        = 0;                                        /* Wisselstand van wis1 negeren                  */
    SCH_code[schgeenwisselwisc1]       = "geenwisselwisc1";       SCH[schgeenwisselwisc1]       = 0;                                        /* Wisselstand van wisc1 negeren                 */
    SCH_code[schwisselpolwis1]         = "wisselpolwis1";         SCH[schwisselpolwis1]         = 0;                                        /* Polariteit van wissel ingang wis1             */
    SCH_code[schwisselpolwisc1]        = "wisselpolwisc1";        SCH[schwisselpolwisc1]        = 0;                                        /* Polariteit van wissel ingang wisc1            */
    SCH_code[schcheckdstype]           = "checkdstype";           SCH[schcheckdstype]           = 0;                                        /* Check type DSI bericht bij VECOM              */
    SCH_code[schprioin41tramkar]       = "prioin41tramkar";       SCH[schprioin41tramkar]       = 1;                                        /* Inmelden 41 via Tram toestaan                 */
    SCH_code[schprioin41tramdet411SD]  = "prioin41tramdet411SD";  SCH[schprioin41tramdet411SD]  = 1;                                        /* Inmelden 41 via Tram toestaan                 */
    SCH_code[schpriouit41tramkar]      = "priouit41tramkar";      SCH[schpriouit41tramkar]      = 1;                                        /* Uitmelden 41 via Tram toestaan                */
    SCH_code[schpriouit41tramdet412ED] = "priouit41tramdet412ED"; SCH[schpriouit41tramdet412ED] = 1;                                        /* Uitmelden 41 via Tram toestaan                */
    SCH_code[schvi41tram]              = "vi41tram";              SCH[schvi41tram]              = 1;                                        /* Versnelde inmelding fase 41                   */
    SCH_code[schprioin42bussdsd42SD]   = "prioin42bussdsd42SD";   SCH[schprioin42bussdsd42SD]   = 1;                                        /* Inmelden 42 via Bus toestaan                  */
    SCH_code[schpriouit42bussdsd42SD]  = "priouit42bussdsd42SD";  SCH[schpriouit42bussdsd42SD]  = 1;                                        /* Uitmelden 42 via Bus toestaan                 */
    SCH_code[schaltg02]                = "altg02";                SCH[schaltg02]                = 0;                                        /* Alternatieve realisatie toestaan fase 02      */
    SCH_code[schaltg05]                = "altg05";                SCH[schaltg05]                = 0;                                        /* Alternatieve realisatie toestaan fase 05      */
    SCH_code[schaltg41]                = "altg41";                SCH[schaltg41]                = 0;                                        /* Alternatieve realisatie toestaan fase 41      */
    SCH_code[schaltg42]                = "altg42";                SCH[schaltg42]                = 0;                                        /* Alternatieve realisatie toestaan fase 42      */

/* parameters */
/* ---------- */
    PRM_code[prmfb]                   = "fb";                   PRM[prmfb]                   = 0;    PRM_type[prmfb]                   = TS_type; /* Instelling fasebewaking                                                        */
    PRM_code[prmxx]                   = "xx";                   PRM[prmxx]                   = 0;                                                 /* Versiebeheer xx                                                                */
    PRM_code[prmyy]                   = "yy";                   PRM[prmyy]                   = 0;                                                 /* Versiebeheer yy                                                                */
    PRM_code[prmzz]                   = "zz";                   PRM[prmzz]                   = 0;                                                 /* Versiebeheer zz                                                                */
    PRM_code[prmmk021]                = "mk021";                PRM[prmmk021]                = 0;                                                 /* Meetkriterium type voor detector 021                                           */
    PRM_code[prmmk022]                = "mk022";                PRM[prmmk022]                = 0;                                                 /* Meetkriterium type voor detector 022                                           */
    PRM_code[prmmk051]                = "mk051";                PRM[prmmk051]                = 0;                                                 /* Meetkriterium type voor detector 051                                           */
    PRM_code[prmmk052]                = "mk052";                PRM[prmmk052]                = 0;                                                 /* Meetkriterium type voor detector 052                                           */
    PRM_code[prmmk411]                = "mk411";                PRM[prmmk411]                = 0;                                                 /* Meetkriterium type voor detector 411                                           */
    PRM_code[prmmk412]                = "mk412";                PRM[prmmk412]                = 0;                                                 /* Meetkriterium type voor detector 412                                           */
    PRM_code[prmmk421]                = "mk421";                PRM[prmmk421]                = 0;                                                 /* Meetkriterium type voor detector 421                                           */
    PRM_code[prmmk422]                = "mk422";                PRM[prmmk422]                = 0;                                                 /* Meetkriterium type voor detector 422                                           */
    PRM_code[prmOVtstpgrensvroeg]     = "OVtstpgrensvroeg";     PRM[prmOVtstpgrensvroeg]     = 0;    PRM_type[prmOVtstpgrensvroeg]     = TS_type; /* Grens waarboven een OV voertuig als te vroeg wordt aangemerkt                  */
    PRM_code[prmOVtstpgrenslaat]      = "OVtstpgrenslaat";      PRM[prmOVtstpgrenslaat]      = 0;    PRM_type[prmOVtstpgrenslaat]      = TS_type; /* Grens waarboven een OV voertuig als te laat wordt aangemerkt                   */
    PRM_code[prmovstipttevroeg41tram] = "ovstipttevroeg41tram"; PRM[prmovstipttevroeg41tram] = 30;                                                /* Prioriteitsnveau OV te vroeg bij 41 Tram                                       */
    PRM_code[prmovstiptoptijd41tram]  = "ovstiptoptijd41tram";  PRM[prmovstiptoptijd41tram]  = 20;                                                /* Prioriteitsnveau OV op tijd bij 41 Tram                                        */
    PRM_code[prmovstipttelaat41tram]  = "ovstipttelaat41tram";  PRM[prmovstipttelaat41tram]  = 10;                                                /* Prioriteitsnveau OV te laat bij 41 Tram                                        */
    PRM_code[prmmwta]                 = "mwta";                 PRM[prmmwta]                 = 0;    PRM_type[prmmwta]                 = TS_type; /* Maximale wachttijd autoverkeer                                                 */
    PRM_code[prmmwtfts]               = "mwtfts";               PRM[prmmwtfts]               = 0;    PRM_type[prmmwtfts]               = TS_type; /* Maximale wachttijd fiets                                                       */
    PRM_code[prmmwtvtg]               = "mwtvtg";               PRM[prmmwtvtg]               = 0;    PRM_type[prmmwtvtg]               = TS_type; /* Maximale wachttijd voetgangers                                                 */
    PRM_code[prmpmgt02]               = "pmgt02";               PRM[prmpmgt02]               = 0;                                                 /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                  */
    PRM_code[prmognt02]               = "ognt02";               PRM[prmognt02]               = 0;    PRM_type[prmognt02]               = TE_type; /* Minimale groentijd bij terugkomen fase 02                                      */
    PRM_code[prmnofm02]               = "nofm02";               PRM[prmnofm02]               = 0;    PRM_type[prmnofm02]               = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 02                               */
    PRM_code[prmmgcov02]              = "mgcov02";              PRM[prmmgcov02]              = 0;    PRM_type[prmmgcov02]              = TE_type; /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov02]             = "pmgcov02";             PRM[prmpmgcov02]             = 0;                                                 /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg02]              = "ohpmg02";              PRM[prmohpmg02]              = 0;                                                 /* Percentage ophogen groentijd na afkappen fase 02                               */
    PRM_code[prmpmgt05]               = "pmgt05";               PRM[prmpmgt05]               = 0;                                                 /* Minimaal percentage groentijd primair tbv. terugkomen fase 05                  */
    PRM_code[prmognt05]               = "ognt05";               PRM[prmognt05]               = 0;    PRM_type[prmognt05]               = TE_type; /* Minimale groentijd bij terugkomen fase 05                                      */
    PRM_code[prmnofm05]               = "nofm05";               PRM[prmnofm05]               = 0;    PRM_type[prmnofm05]               = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 05                               */
    PRM_code[prmmgcov05]              = "mgcov05";              PRM[prmmgcov05]              = 0;    PRM_type[prmmgcov05]              = TE_type; /* Minimum groentijd waarna fase 05 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov05]             = "pmgcov05";             PRM[prmpmgcov05]             = 0;                                                 /* Minimum percentage groentijd waarna fase 05 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg05]              = "ohpmg05";              PRM[prmohpmg05]              = 0;                                                 /* Percentage ophogen groentijd na afkappen fase 05                               */
    PRM_code[prmpmgt41]               = "pmgt41";               PRM[prmpmgt41]               = 0;                                                 /* Minimaal percentage groentijd primair tbv. terugkomen fase 41                  */
    PRM_code[prmognt41]               = "ognt41";               PRM[prmognt41]               = 0;    PRM_type[prmognt41]               = TE_type; /* Minimale groentijd bij terugkomen fase 41                                      */
    PRM_code[prmnofm41]               = "nofm41";               PRM[prmnofm41]               = 0;    PRM_type[prmnofm41]               = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 41                               */
    PRM_code[prmmgcov41]              = "mgcov41";              PRM[prmmgcov41]              = 0;    PRM_type[prmmgcov41]              = TE_type; /* Minimum groentijd waarna fase 41 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov41]             = "pmgcov41";             PRM[prmpmgcov41]             = 0;                                                 /* Minimum percentage groentijd waarna fase 41 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg41]              = "ohpmg41";              PRM[prmohpmg41]              = 0;                                                 /* Percentage ophogen groentijd na afkappen fase 41                               */
    PRM_code[prmpmgt42]               = "pmgt42";               PRM[prmpmgt42]               = 0;                                                 /* Minimaal percentage groentijd primair tbv. terugkomen fase 42                  */
    PRM_code[prmognt42]               = "ognt42";               PRM[prmognt42]               = 0;    PRM_type[prmognt42]               = TE_type; /* Minimale groentijd bij terugkomen fase 42                                      */
    PRM_code[prmnofm42]               = "nofm42";               PRM[prmnofm42]               = 0;    PRM_type[prmnofm42]               = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 42                               */
    PRM_code[prmmgcov42]              = "mgcov42";              PRM[prmmgcov42]              = 0;    PRM_type[prmmgcov42]              = TE_type; /* Minimum groentijd waarna fase 42 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov42]             = "pmgcov42";             PRM[prmpmgcov42]             = 0;                                                 /* Minimum percentage groentijd waarna fase 42 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg42]              = "ohpmg42";              PRM[prmohpmg42]              = 0;                                                 /* Percentage ophogen groentijd na afkappen fase 42                               */
    PRM_code[prmrto41tram]            = "rto41tram";            PRM[prmrto41tram]            = 150;  PRM_type[prmrto41tram]            = TE_type; /* Ongehinderde rijtijd prioriteit fase 41                                        */
    PRM_code[prmrtbg41tram]           = "rtbg41tram";           PRM[prmrtbg41tram]           = 200;  PRM_type[prmrtbg41tram]           = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    PRM_code[prmrtg41tram]            = "rtg41tram";            PRM[prmrtg41tram]            = 300;  PRM_type[prmrtg41tram]            = TE_type; /* Gehinderde rijtijd prioriteit fase 41                                          */
    PRM_code[prmomx41tram]            = "omx41tram";            PRM[prmomx41tram]            = 50;   PRM_type[prmomx41tram]            = TE_type; /* Ondermaximum OV fase 41                                                        */
    PRM_code[prmupinagb41tram]        = "upinagb41tram";        PRM[prmupinagb41tram]        = 0;                                                 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    PRM_code[prmvtgcat41tram]         = "vtgcat41tram";         PRM[prmvtgcat41tram]         = 2;                                                 /* Voertuigcategorie DSI voor prio ingreep 41tram                                 */
    PRM_code[prmprio41tram]           = "prio41tram";           PRM[prmprio41tram]           = 2123;                                              /* Prioriteitsinstelling OV fase 41                                               */
    PRM_code[prmallelijnen41tram]     = "allelijnen41tram";     PRM[prmallelijnen41tram]     = 0;                                                 /* Prioriteit geven aan alle lijnen fase 41                                       */
    PRM_code[prmlijn41tram_01]        = "lijn41tram_01";        PRM[prmlijn41tram_01]        = 1;                                                 /* Lijnnummer 1 tbv prioriteit OV fase 41                                         */
    PRM_code[prmlijn41tram_02]        = "lijn41tram_02";        PRM[prmlijn41tram_02]        = 12;                                                /* Lijnnummer 2 tbv prioriteit OV fase 41                                         */
    PRM_code[prmritcat41tram_01]      = "ritcat41tram_01";      PRM[prmritcat41tram_01]      = 10;                                                /* Ritcategorie bij lijnnummer 1 tbv prioriteit OV fase 1                         */
    PRM_code[prmritcat41tram_02]      = "ritcat41tram_02";      PRM[prmritcat41tram_02]      = 20;                                                /* Ritcategorie bij lijnnummer 2 tbv prioriteit OV fase 2                         */
    PRM_code[prmrto42bus]             = "rto42bus";             PRM[prmrto42bus]             = 150;  PRM_type[prmrto42bus]             = TE_type; /* Ongehinderde rijtijd prioriteit fase 42                                        */
    PRM_code[prmrtbg42bus]            = "rtbg42bus";            PRM[prmrtbg42bus]            = 200;  PRM_type[prmrtbg42bus]            = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 42                                  */
    PRM_code[prmrtg42bus]             = "rtg42bus";             PRM[prmrtg42bus]             = 300;  PRM_type[prmrtg42bus]             = TE_type; /* Gehinderde rijtijd prioriteit fase 42                                          */
    PRM_code[prmomx42bus]             = "omx42bus";             PRM[prmomx42bus]             = 50;   PRM_type[prmomx42bus]             = TE_type; /* Ondermaximum OV fase 42                                                        */
    PRM_code[prmupinagb42bus]         = "upinagb42bus";         PRM[prmupinagb42bus]         = 0;                                                 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 42                  */
    PRM_code[prmvtgcat42bus]          = "vtgcat42bus";          PRM[prmvtgcat42bus]          = 1;                                                 /* Voertuigcategorie DSI voor prio ingreep 42bus                                  */
    PRM_code[prmprio42bus]            = "prio42bus";            PRM[prmprio42bus]            = 1004;                                              /* Prioriteitsinstelling OV fase 42                                               */
    PRM_code[prmallelijnen42bus]      = "allelijnen42bus";      PRM[prmallelijnen42bus]      = 1;                                                 /* Prioriteit geven aan alle lijnen fase 42                                       */
    PRM_code[prmlijn42bus_01]         = "lijn42bus_01";         PRM[prmlijn42bus_01]         = 300;                                               /* Lijnnummer 1 tbv prioriteit OV fase 42                                         */
    PRM_code[prmritcat42bus_01]       = "ritcat42bus_01";       PRM[prmritcat42bus_01]       = 10;                                                /* Ritcategorie bij lijnnummer 1 tbv prioriteit OV fase 1                         */
    PRM_code[prmstkp1]                = "stkp1";                PRM[prmstkp1]                = 600;  PRM_type[prmstkp1]                = TI_type; /* Start klokperiode ochtend                                                      */
    PRM_code[prmetkp1]                = "etkp1";                PRM[prmetkp1]                = 900;  PRM_type[prmetkp1]                = TI_type; /* Einde klokperiode ochtend                                                      */
    PRM_code[prmdckp1]                = "dckp1";                PRM[prmdckp1]                = 8;                                                 /* Dagsoort klokperiode ochtend                                                   */
    PRM_code[prmmg1_02]               = "mg1_02";               PRM[prmmg1_02]               = 300;  PRM_type[prmmg1_02]               = TE_type; /* Maximale groentijd MG1 02                                                      */
    PRM_code[prmmg1_05]               = "mg1_05";               PRM[prmmg1_05]               = 300;  PRM_type[prmmg1_05]               = TE_type; /* Maximale groentijd MG1 05                                                      */
    PRM_code[prmmg1_41]               = "mg1_41";               PRM[prmmg1_41]               = 300;  PRM_type[prmmg1_41]               = TE_type; /* Maximale groentijd MG1 41                                                      */
    PRM_code[prmmg1_42]               = "mg1_42";               PRM[prmmg1_42]               = 300;  PRM_type[prmmg1_42]               = TE_type; /* Maximale groentijd MG1 42                                                      */
    PRM_code[prmmg2_02]               = "mg2_02";               PRM[prmmg2_02]               = 200;  PRM_type[prmmg2_02]               = TE_type; /* Maximale groentijd MG2 02                                                      */
    PRM_code[prmmg2_05]               = "mg2_05";               PRM[prmmg2_05]               = 200;  PRM_type[prmmg2_05]               = TE_type; /* Maximale groentijd MG2 05                                                      */
    PRM_code[prmmg2_41]               = "mg2_41";               PRM[prmmg2_41]               = 200;  PRM_type[prmmg2_41]               = TE_type; /* Maximale groentijd MG2 41                                                      */
    PRM_code[prmmg2_42]               = "mg2_42";               PRM[prmmg2_42]               = 200;  PRM_type[prmmg2_42]               = TE_type; /* Maximale groentijd MG2 42                                                      */
    PRM_code[prmmlfpr02]              = "mlfpr02";              PRM[prmmlfpr02]              = 0;                                                 /* Maximaal aantal modules vooruit fase 02                                        */
    PRM_code[prmmlfpr05]              = "mlfpr05";              PRM[prmmlfpr05]              = 0;                                                 /* Maximaal aantal modules vooruit fase 05                                        */
    PRM_code[prmmlfpr41]              = "mlfpr41";              PRM[prmmlfpr41]              = 0;                                                 /* Maximaal aantal modules vooruit fase 41                                        */
    PRM_code[prmmlfpr42]              = "mlfpr42";              PRM[prmmlfpr42]              = 0;                                                 /* Maximaal aantal modules vooruit fase 42                                        */
    PRM_code[prmaltg02]               = "altg02";               PRM[prmaltg02]               = 0;    PRM_type[prmaltg02]               = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    PRM_code[prmaltp02]               = "altp02";               PRM[prmaltp02]               = 0;    PRM_type[prmaltp02]               = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    PRM_code[prmaltg05]               = "altg05";               PRM[prmaltg05]               = 0;    PRM_type[prmaltg05]               = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 05                         */
    PRM_code[prmaltp05]               = "altp05";               PRM[prmaltp05]               = 0;    PRM_type[prmaltp05]               = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 05                            */
    PRM_code[prmaltg41]               = "altg41";               PRM[prmaltg41]               = 0;    PRM_type[prmaltg41]               = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 41                         */
    PRM_code[prmaltp41]               = "altp41";               PRM[prmaltp41]               = 0;    PRM_type[prmaltp41]               = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 41                            */
    PRM_code[prmaltg42]               = "altg42";               PRM[prmaltg42]               = 0;    PRM_type[prmaltg42]               = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 42                         */
    PRM_code[prmaltp42]               = "altp42";               PRM[prmaltp42]               = 0;    PRM_type[prmaltp42]               = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 42                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    PRM_code[prmtestdsivert]          = "testdsivert";          PRM[prmtestdsivert]          = 120;  PRM_type[prmtestdsivert]          = TS_type; /* Testen vertraging in DSI bericht in testomgeving                               */
    PRM_code[prmtestdsilyn]           = "testdsilyn";           PRM[prmtestdsilyn]           = 0;                                      /* Testen lijnnummer DSI bericht in testomgeving                                  */
    PRM_code[prmtestdsicat]           = "testdsicat";           PRM[prmtestdsicat]           = 10;                                     /* Testen ritcategorie DSI bericht in testomgeving                                */
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
    DS_code[DSD42]  = "SD42";

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
    IS_type[d421] = DKOP_type;
    IS_type[d422] = DLNG_type;
    IS_type[dwis1] = DKOP_type;
    IS_type[dsd42] = DKOP_type;

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
