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
   FC_code[fc22] = "22"; TRG_max[fc22] = 0; TRG_min[fc22] = 0; TGG_max[fc22] = 0; TGG_min[fc22] = 0; TFG_max[fc22] = 0; TGL_max[fc22] = 0; TGL_min[fc22] = 0; TMGL_max[fc22] = 0; TVG_max[fc22] = 300;
   FC_code[fc41] = "41"; TRG_max[fc41] = 0; TRG_min[fc41] = 0; TGG_max[fc41] = 0; TGG_min[fc41] = 0; TFG_max[fc41] = 0; TGL_max[fc41] = 0; TGL_min[fc41] = 0; TMGL_max[fc41] = 0; TVG_max[fc41] = 300;
   FC_code[fc61a] = "61a"; TRG_max[fc61a] = 0; TRG_min[fc61a] = 0; TGG_max[fc61a] = 0; TGG_min[fc61a] = 0; TFG_max[fc61a] = 0; TGL_max[fc61a] = 0; TGL_min[fc61a] = 0; TMGL_max[fc61a] = 0; TVG_max[fc61a] = 300;

/* conflicten */
/* ---------- */

    TIG_max[fc02][fc22] = 30;
    TIG_max[fc02][fc41] = 30;
    TIG_max[fc02][fc61a] = 30;

    TIG_max[fc22][fc02] = 30;
    TIG_max[fc22][fc41] = 30;
    TIG_max[fc22][fc61a] = 30;

    TIG_max[fc41][fc02] = 30;
    TIG_max[fc41][fc22] = 30;
    TIG_max[fc41][fc61a] = 30;

    TIG_max[fc61a][fc02] = 30;
    TIG_max[fc61a][fc22] = 30;
    TIG_max[fc61a][fc41] = 30;

/* overige uitgangen */
/* ----------------- */
    US_code[ussegm1]        = "segm1";                                 /* Aansturing segmenten display               */
    US_code[ussegm2]        = "segm2";                                 /* Aansturing segmenten display               */
    US_code[ussegm3]        = "segm3";                                 /* Aansturing segmenten display               */
    US_code[ussegm4]        = "segm4";                                 /* Aansturing segmenten display               */
    US_code[ussegm5]        = "segm5";                                 /* Aansturing segmenten display               */
    US_code[ussegm6]        = "segm6";                                 /* Aansturing segmenten display               */
    US_code[ussegm7]        = "segm7";                                 /* Aansturing segmenten display               */
    US_code[usmaxwt]        = "maxwt";                                 /* Verklikken maximale wachttijd overschreden */
    US_code[uskarmelding]   = "karmelding";                            /* Verklikken ontvangst melding KAR           */
    US_code[uskarog]        = "karog";                                 /* Verklikken ondergedrag KAR                 */
    US_code[usovinm61abus]  = "ovinm61abus";                           /* Verklikken inmelding OV fase 61a           */
    US_code[usovinm61ahd]   = "ovinm61ahd";                            /* Verklikken inmelding OV fase 61a           */
    US_code[usovinm22fiets] = "ovinm22fiets";                          /* Verklikken inmelding OV fase 22            */
    US_code[usovinm41bus]   = "ovinm41bus";                            /* Verklikken inmelding OV fase 41            */
    US_code[usperdef]       = "perdef";                                /* Default periode actief                     */
    US_code[usper1]         = "per1";                                  /* Periode actief                             */

/* detectie */
/* -------- */
    D_code[d021]  = "021";  
    D_code[d022]  = "022";  
    D_code[d221]  = "221";  
    D_code[d222]  = "222";  
    D_code[dk22]  = "k22";  
    D_code[d411]  = "411";  
    D_code[d412]  = "412";  
    D_code[d61a1] = "61a1"; 
    D_code[d61a2] = "61a2"; 
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined PRACTICE_TEST
    D_code[ddummykarin61abus]  = "dummykarin61abus";  
    D_code[ddummykarin61ahd]   = "dummykarin61ahd";   
    D_code[ddummykaruit61abus] = "dummykaruit61abus"; 
    D_code[ddummykaruit61ahd]  = "dummykaruit61ahd";  
#endif

/* overige ingangen */
/* ---------------- */

/* hulp elementen */
/* -------------- */
    H_code[hprio61abus]            = "prio61abus";                                             /* Bijhouden actief zijn prioriteit fase 61a         */
    H_code[hprioin61abus]          = "prioin61abus";                                           /* Prioriteit inmelding fase 61a Bus                 */
    H_code[hpriouit61abus]         = "priouit61abus";                                          /* Prioriteit uitmelding 61a Bus                     */
    H_code[hprioin61abuskar]       = "prioin61abuskar";                                        /* Prioriteit inmelding fase 61a Bus                 */
    H_code[hpriouit61abuskar]      = "priouit61abuskar";                                       /* Prioriteit uitmelding 61a Bus                     */
    H_code[hprio61ahd]             = "prio61ahd";                                              /* Bijhouden actief zijn prioriteit fase 61a         */
    H_code[hprioin61ahd]           = "prioin61ahd";                                            /* Prioriteit inmelding fase 61a Nood- en hulpdienst */
    H_code[hpriouit61ahd]          = "priouit61ahd";                                           /* Prioriteit uitmelding 61a Nood- en hulpdienst     */
    H_code[hprioin61ahdkar]        = "prioin61ahdkar";                                         /* Prioriteit inmelding fase 61a Nood- en hulpdienst */
    H_code[hpriouit61ahdkar]       = "priouit61ahdkar";                                        /* Prioriteit uitmelding 61a Nood- en hulpdienst     */
    H_code[hprio22fiets]           = "prio22fiets";                                            /* Bijhouden actief zijn prioriteit fase 22          */
    H_code[hprioin22fiets]         = "prioin22fiets";                                          /* Prioriteit inmelding fase 22 Fiets                */
    H_code[hpriouit22fiets]        = "priouit22fiets";                                         /* Prioriteit uitmelding 22 Fiets                    */
    H_code[hprioin22fietsfiets221] = "prioin22fietsfiets221";                                  /* Prioriteit inmelding fase 22 Fiets                */
    H_code[hpriouit22fietsfiets]   = "priouit22fietsfiets";                                    /* Prioriteit uitmelding 22 Fiets                    */
    H_code[hprio41bus]             = "prio41bus";                                              /* Bijhouden actief zijn prioriteit fase 41          */
    H_code[hprioin41bus]           = "prioin41bus";                                            /* Prioriteit inmelding fase 41 Bus                  */
    H_code[hpriouit41bus]          = "priouit41bus";                                           /* Prioriteit uitmelding 41 Bus                      */
    H_code[hprioin41busdet411]     = "prioin41busdet411";                                      /* Prioriteit inmelding fase 41 Bus                  */
    H_code[hprioin41businpov41in]  = "prioin41businpov41in";                                   /* Prioriteit inmelding fase 41 Bus                  */
    H_code[hprioin41busvecio414]   = "prioin41busvecio414";                                    /* Prioriteit inmelding fase 41 Bus                  */
    H_code[hpriouit41busdet412]    = "priouit41busdet412";                                     /* Prioriteit uitmelding 41 Bus                      */

/* geheugen elementen */
/* ------------------ */
    MM_code[mperiod] = "period";                   /* Onthouden actieve periode    */
    MM_code[mar02]   = "ar02";                     /* Alternatieve ruimte fase 02  */
    MM_code[mar22]   = "ar22";                     /* Alternatieve ruimte fase 22  */
    MM_code[mar41]   = "ar41";                     /* Alternatieve ruimte fase 41  */
    MM_code[mar61a]  = "ar61a";                    /* Alternatieve ruimte fase 61a */

/* tijd elementen */
/* -------------- */
    T_code[tkm02]         = "km02";         T_max[tkm02]         = 0;    T_type[tkm02]         = TE_type; /* Kop maximum voor detector 02                           */
    T_code[tkm22]         = "km22";         T_max[tkm22]         = 0;    T_type[tkm22]         = TE_type; /* Kop maximum voor detector 22                           */
    T_code[tkm41]         = "km41";         T_max[tkm41]         = 0;    T_type[tkm41]         = TE_type; /* Kop maximum voor detector 41                           */
    T_code[tkm61a]        = "km61a";        T_max[tkm61a]        = 0;    T_type[tkm61a]        = TE_type; /* Kop maximum voor detector 61a                          */
    T_code[tkarmelding]   = "karmelding";   T_max[tkarmelding]   = 15;   T_type[tkarmelding]   = TE_type; /* Duur verklikking ontvangst melding KAR                 */
    T_code[tkarog]        = "karog";        T_max[tkarog]        = 1440; T_type[tkarog]        = TM_type; /* Ondergedrag KAR                                        */
    T_code[tbtovg61abus]  = "btovg61abus";  T_max[tbtovg61abus]  = 30;   T_type[tbtovg61abus]  = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 61a */
    T_code[trt61abus]     = "rt61abus";     T_max[trt61abus]     = 0;    T_type[trt61abus]     = TE_type; /* Actuele rijtijd prio fase 61a                          */
    T_code[tgb61abus]     = "gb61abus";     T_max[tgb61abus]     = 600;  T_type[tgb61abus]     = TE_type; /* Groenbewaking prioriteit fase 61a                      */
    T_code[tblk61abus]    = "blk61abus";    T_max[tblk61abus]    = 200;  T_type[tblk61abus]    = TE_type; /* Blokkeertijd na prioriteitsingreep fase 61a            */
    T_code[tbtovg61ahd]   = "btovg61ahd";   T_max[tbtovg61ahd]   = 30;   T_type[tbtovg61ahd]   = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 61a */
    T_code[trt61ahd]      = "rt61ahd";      T_max[trt61ahd]      = 0;    T_type[trt61ahd]      = TE_type; /* Actuele rijtijd prio fase 61a                          */
    T_code[tgb61ahd]      = "gb61ahd";      T_max[tgb61ahd]      = 600;  T_type[tgb61ahd]      = TE_type; /* Groenbewaking prioriteit fase 61a                      */
    T_code[tblk61ahd]     = "blk61ahd";     T_max[tblk61ahd]     = 200;  T_type[tblk61ahd]     = TE_type; /* Blokkeertijd na prioriteitsingreep fase 61a            */
    T_code[tbtovg22fiets] = "btovg22fiets"; T_max[tbtovg22fiets] = 30;   T_type[tbtovg22fiets] = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 22  */
    T_code[trt22fiets]    = "rt22fiets";    T_max[trt22fiets]    = 0;    T_type[trt22fiets]    = TE_type; /* Actuele rijtijd prio fase 22                           */
    T_code[tgb22fiets]    = "gb22fiets";    T_max[tgb22fiets]    = 600;  T_type[tgb22fiets]    = TE_type; /* Groenbewaking prioriteit fase 22                       */
    T_code[tblk22fiets]   = "blk22fiets";   T_max[tblk22fiets]   = 200;  T_type[tblk22fiets]   = TE_type; /* Blokkeertijd na prioriteitsingreep fase 22             */
    T_code[tbtovg41bus]   = "btovg41bus";   T_max[tbtovg41bus]   = 30;   T_type[tbtovg41bus]   = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 41  */
    T_code[trt41bus]      = "rt41bus";      T_max[trt41bus]      = 0;    T_type[trt41bus]      = TE_type; /* Actuele rijtijd prio fase 41                           */
    T_code[tgb41bus]      = "gb41bus";      T_max[tgb41bus]      = 600;  T_type[tgb41bus]      = TE_type; /* Groenbewaking prioriteit fase 41                       */
    T_code[tblk41bus]     = "blk41bus";     T_max[tblk41bus]     = 200;  T_type[tblk41bus]     = TE_type; /* Blokkeertijd na prioriteitsingreep fase 41             */

/* teller elementen */
/* ---------------- */
    C_code[cvc61abus]           = "vc61abus";           C_max[cvc61abus]           = 999; C_type[cvc61abus]           = CT_type; /* Bijhouden prio inmeldingen fase 61a type Bus                 */
    C_code[cvc61ahd]            = "vc61ahd";            C_max[cvc61ahd]            = 999; C_type[cvc61ahd]            = CT_type; /* Bijhouden prio inmeldingen fase 61a type Nood- en hulpdienst */
    C_code[cftscyc22fietsfiets] = "ftscyc22fietsfiets"; C_max[cftscyc22fietsfiets] = 999;                                        /* Bijhouden realisaties tbv peloton prio voor fase 22          */
    C_code[cvc22fiets]          = "vc22fiets";          C_max[cvc22fiets]          = 999; C_type[cvc22fiets]          = CT_type; /* Bijhouden prio inmeldingen fase 22 type Fiets                */
    C_code[cvc41bus]            = "vc41bus";            C_max[cvc41bus]            = 999; C_type[cvc41bus]            = CT_type; /* Bijhouden prio inmeldingen fase 41 type Bus                  */

/* schakelaars */
/* ----------- */
    SCH_code[schcheckdstype]             = "checkdstype";             SCH[schcheckdstype]             = 0;                                          /* Check type DSI bericht bij VECOM               */
    SCH_code[schprioin61abuskar]         = "prioin61abuskar";         SCH[schprioin61abuskar]         = 1;                                          /* Inmelden 61a via Bus toestaan                  */
    SCH_code[schpriouit61abuskar]        = "priouit61abuskar";        SCH[schpriouit61abuskar]        = 1;                                          /* Uitmelden 61a via Bus toestaan                 */
    SCH_code[schprioin61ahdkar]          = "prioin61ahdkar";          SCH[schprioin61ahdkar]          = 1;                                          /* Inmelden 61a via Nood- en hulpdienst toestaan  */
    SCH_code[schpriouit61ahdkar]         = "priouit61ahdkar";         SCH[schpriouit61ahdkar]         = 1;                                          /* Uitmelden 61a via Nood- en hulpdienst toestaan */
    SCH_code[schprioin22fietsfiets221]   = "prioin22fietsfiets221";   SCH[schprioin22fietsfiets221]   = 1;                                          /* Inmelden 22 via Fiets toestaan                 */
    SCH_code[schpriouit22fietsfiets]     = "priouit22fietsfiets";     SCH[schpriouit22fietsfiets]     = 1;                                          /* Uitmelden 22 via Fiets toestaan                */
    SCH_code[schprioin41busdet411SD]     = "prioin41busdet411SD";     SCH[schprioin41busdet411SD]     = 1;                                          /* Inmelden 41 via Bus toestaan                   */
    SCH_code[schprioin41businpov41inSIS] = "prioin41businpov41inSIS"; SCH[schprioin41businpov41inSIS] = 1;                                          /* Inmelden 41 via Bus toestaan                   */
    SCH_code[schprioin41busvecio414SD]   = "prioin41busvecio414SD";   SCH[schprioin41busvecio414SD]   = 1;                                          /* Inmelden 41 via Bus toestaan                   */
    SCH_code[schpriouit41busdet412ED]    = "priouit41busdet412ED";    SCH[schpriouit41busdet412ED]    = 1;                                          /* Uitmelden 41 via Bus toestaan                  */
    SCH_code[schaltg02]                  = "altg02";                  SCH[schaltg02]                  = 0;                                          /* Alternatieve realisatie toestaan fase 02       */
    SCH_code[schaltg22]                  = "altg22";                  SCH[schaltg22]                  = 0;                                          /* Alternatieve realisatie toestaan fase 22       */
    SCH_code[schaltg41]                  = "altg41";                  SCH[schaltg41]                  = 0;                                          /* Alternatieve realisatie toestaan fase 41       */
    SCH_code[schaltg61a]                 = "altg61a";                 SCH[schaltg61a]                 = 0;                                          /* Alternatieve realisatie toestaan fase 61a      */

/* parameters */
/* ---------- */
    PRM_code[prmfb]                       = "fb";                       PRM[prmfb]                       = 0;    PRM_type[prmfb]                       = TS_type; /* Instelling fasebewaking                                                         */
    PRM_code[prmxx]                       = "xx";                       PRM[prmxx]                       = 0;                                                     /* Versiebeheer xx                                                                 */
    PRM_code[prmyy]                       = "yy";                       PRM[prmyy]                       = 0;                                                     /* Versiebeheer yy                                                                 */
    PRM_code[prmzz]                       = "zz";                       PRM[prmzz]                       = 0;                                                     /* Versiebeheer zz                                                                 */
    PRM_code[prmmk021]                    = "mk021";                    PRM[prmmk021]                    = 0;                                                     /* Meetkriterium type voor detector 021                                            */
    PRM_code[prmmk022]                    = "mk022";                    PRM[prmmk022]                    = 0;                                                     /* Meetkriterium type voor detector 022                                            */
    PRM_code[prmmk221]                    = "mk221";                    PRM[prmmk221]                    = 0;                                                     /* Meetkriterium type voor detector 221                                            */
    PRM_code[prmmk222]                    = "mk222";                    PRM[prmmk222]                    = 0;                                                     /* Meetkriterium type voor detector 222                                            */
    PRM_code[prmmkk22]                    = "mkk22";                    PRM[prmmkk22]                    = 0;                                                     /* Meetkriterium type voor detector k22                                            */
    PRM_code[prmmk411]                    = "mk411";                    PRM[prmmk411]                    = 0;                                                     /* Meetkriterium type voor detector 411                                            */
    PRM_code[prmmk412]                    = "mk412";                    PRM[prmmk412]                    = 0;                                                     /* Meetkriterium type voor detector 412                                            */
    PRM_code[prmmk61a1]                   = "mk61a1";                   PRM[prmmk61a1]                   = 0;                                                     /* Meetkriterium type voor detector 61a1                                           */
    PRM_code[prmmk61a2]                   = "mk61a2";                   PRM[prmmk61a2]                   = 0;                                                     /* Meetkriterium type voor detector 61a2                                           */
    PRM_code[prmmwta]                     = "mwta";                     PRM[prmmwta]                     = 0;    PRM_type[prmmwta]                     = TS_type; /* Maximale wachttijd autoverkeer                                                  */
    PRM_code[prmmwtfts]                   = "mwtfts";                   PRM[prmmwtfts]                   = 0;    PRM_type[prmmwtfts]                   = TS_type; /* Maximale wachttijd fiets                                                        */
    PRM_code[prmmwtvtg]                   = "mwtvtg";                   PRM[prmmwtvtg]                   = 0;    PRM_type[prmmwtvtg]                   = TS_type; /* Maximale wachttijd voetgangers                                                  */
    PRM_code[prmpmgt02]                   = "pmgt02";                   PRM[prmpmgt02]                   = 0;                                                     /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                   */
    PRM_code[prmognt02]                   = "ognt02";                   PRM[prmognt02]                   = 0;    PRM_type[prmognt02]                   = TE_type; /* Minimale groentijd bij terugkomen fase 02                                       */
    PRM_code[prmnofm02]                   = "nofm02";                   PRM[prmnofm02]                   = 0;    PRM_type[prmnofm02]                   = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 02                                */
    PRM_code[prmmgcov02]                  = "mgcov02";                  PRM[prmmgcov02]                  = 0;    PRM_type[prmmgcov02]                  = TE_type; /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep             */
    PRM_code[prmpmgcov02]                 = "pmgcov02";                 PRM[prmpmgcov02]                 = 0;                                                     /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep  */
    PRM_code[prmohpmg02]                  = "ohpmg02";                  PRM[prmohpmg02]                  = 0;                                                     /* Percentage ophogen groentijd na afkappen fase 02                                */
    PRM_code[prmpmgt22]                   = "pmgt22";                   PRM[prmpmgt22]                   = 0;                                                     /* Minimaal percentage groentijd primair tbv. terugkomen fase 22                   */
    PRM_code[prmognt22]                   = "ognt22";                   PRM[prmognt22]                   = 0;    PRM_type[prmognt22]                   = TE_type; /* Minimale groentijd bij terugkomen fase 22                                       */
    PRM_code[prmnofm22]                   = "nofm22";                   PRM[prmnofm22]                   = 0;    PRM_type[prmnofm22]                   = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 22                                */
    PRM_code[prmmgcov22]                  = "mgcov22";                  PRM[prmmgcov22]                  = 0;    PRM_type[prmmgcov22]                  = TE_type; /* Minimum groentijd waarna fase 22 afgkapt mag worden door OV ingreep             */
    PRM_code[prmpmgcov22]                 = "pmgcov22";                 PRM[prmpmgcov22]                 = 0;                                                     /* Minimum percentage groentijd waarna fase 22 afgkapt mag worden door OV ingreep  */
    PRM_code[prmohpmg22]                  = "ohpmg22";                  PRM[prmohpmg22]                  = 0;                                                     /* Percentage ophogen groentijd na afkappen fase 22                                */
    PRM_code[prmpmgt41]                   = "pmgt41";                   PRM[prmpmgt41]                   = 0;                                                     /* Minimaal percentage groentijd primair tbv. terugkomen fase 41                   */
    PRM_code[prmognt41]                   = "ognt41";                   PRM[prmognt41]                   = 0;    PRM_type[prmognt41]                   = TE_type; /* Minimale groentijd bij terugkomen fase 41                                       */
    PRM_code[prmnofm41]                   = "nofm41";                   PRM[prmnofm41]                   = 0;    PRM_type[prmnofm41]                   = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 41                                */
    PRM_code[prmmgcov41]                  = "mgcov41";                  PRM[prmmgcov41]                  = 0;    PRM_type[prmmgcov41]                  = TE_type; /* Minimum groentijd waarna fase 41 afgkapt mag worden door OV ingreep             */
    PRM_code[prmpmgcov41]                 = "pmgcov41";                 PRM[prmpmgcov41]                 = 0;                                                     /* Minimum percentage groentijd waarna fase 41 afgkapt mag worden door OV ingreep  */
    PRM_code[prmohpmg41]                  = "ohpmg41";                  PRM[prmohpmg41]                  = 0;                                                     /* Percentage ophogen groentijd na afkappen fase 41                                */
    PRM_code[prmpmgt61a]                  = "pmgt61a";                  PRM[prmpmgt61a]                  = 0;                                                     /* Minimaal percentage groentijd primair tbv. terugkomen fase 61a                  */
    PRM_code[prmognt61a]                  = "ognt61a";                  PRM[prmognt61a]                  = 0;    PRM_type[prmognt61a]                  = TE_type; /* Minimale groentijd bij terugkomen fase 61a                                      */
    PRM_code[prmnofm61a]                  = "nofm61a";                  PRM[prmnofm61a]                  = 0;    PRM_type[prmnofm61a]                  = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 61a                               */
    PRM_code[prmmgcov61a]                 = "mgcov61a";                 PRM[prmmgcov61a]                 = 0;    PRM_type[prmmgcov61a]                 = TE_type; /* Minimum groentijd waarna fase 61a afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov61a]                = "pmgcov61a";                PRM[prmpmgcov61a]                = 0;                                                     /* Minimum percentage groentijd waarna fase 61a afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg61a]                 = "ohpmg61a";                 PRM[prmohpmg61a]                 = 0;                                                     /* Percentage ophogen groentijd na afkappen fase 61a                               */
    PRM_code[prmrto61abus]                = "rto61abus";                PRM[prmrto61abus]                = 150;  PRM_type[prmrto61abus]                = TE_type; /* Ongehinderde rijtijd prioriteit fase 61a                                        */
    PRM_code[prmrtbg61abus]               = "rtbg61abus";               PRM[prmrtbg61abus]               = 200;  PRM_type[prmrtbg61abus]               = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 61a                                  */
    PRM_code[prmrtg61abus]                = "rtg61abus";                PRM[prmrtg61abus]                = 300;  PRM_type[prmrtg61abus]                = TE_type; /* Gehinderde rijtijd prioriteit fase 61a                                          */
    PRM_code[prmomx61abus]                = "omx61abus";                PRM[prmomx61abus]                = 50;   PRM_type[prmomx61abus]                = TE_type; /* Ondermaximum OV fase 61a                                                        */
    PRM_code[prmupinagb61abus]            = "upinagb61abus";            PRM[prmupinagb61abus]            = 0;                                                     /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 61a                  */
    PRM_code[prmvtgcat61abus]             = "vtgcat61abus";             PRM[prmvtgcat61abus]             = 1;                                                     /* Voertuigcategorie DSI voor prio ingreep 61abus                                  */
    PRM_code[prmprio61abus]               = "prio61abus";               PRM[prmprio61abus]               = 0;                                                     /* Prioriteitsinstelling OV fase 61a                                               */
    PRM_code[prmrto61ahd]                 = "rto61ahd";                 PRM[prmrto61ahd]                 = 150;  PRM_type[prmrto61ahd]                 = TE_type; /* Ongehinderde rijtijd prioriteit fase 61a                                        */
    PRM_code[prmrtbg61ahd]                = "rtbg61ahd";                PRM[prmrtbg61ahd]                = 200;  PRM_type[prmrtbg61ahd]                = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 61a                                  */
    PRM_code[prmrtg61ahd]                 = "rtg61ahd";                 PRM[prmrtg61ahd]                 = 300;  PRM_type[prmrtg61ahd]                 = TE_type; /* Gehinderde rijtijd prioriteit fase 61a                                          */
    PRM_code[prmomx61ahd]                 = "omx61ahd";                 PRM[prmomx61ahd]                 = 50;   PRM_type[prmomx61ahd]                 = TE_type; /* Ondermaximum OV fase 61a                                                        */
    PRM_code[prmupinagb61ahd]             = "upinagb61ahd";             PRM[prmupinagb61ahd]             = 0;                                                     /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 61a                  */
    PRM_code[prmvtgcat61ahd]              = "vtgcat61ahd";              PRM[prmvtgcat61ahd]              = 0;                                                     /* Voertuigcategorie DSI voor prio ingreep 61ahd                                   */
    PRM_code[prmprio61ahd]                = "prio61ahd";                PRM[prmprio61ahd]                = 9005;                                                  /* Prioriteitsinstelling OV fase 61a                                               */
    PRM_code[prmftsblok22fietsfiets]      = "ftsblok22fietsfiets";      PRM[prmftsblok22fietsfiets]      = 0;                                                     /* Blokken waarin fiets peloton prio actief mag zijn voor fase 22                  */
    PRM_code[prmftsmaxpercyc22fietsfiets] = "ftsmaxpercyc22fietsfiets"; PRM[prmftsmaxpercyc22fietsfiets] = 0;                                                     /* Maximaal aantal keer fiets peloton prio per cyclus voor fase 22                 */
    PRM_code[prmftsminwt22fietsfiets]     = "ftsminwt22fietsfiets";     PRM[prmftsminwt22fietsfiets]     = 0;    PRM_type[prmftsminwt22fietsfiets]     = TS_type; /* Minimale wachttijd tbv peloton prio voor fase 22                                */
    PRM_code[prmrto22fiets]               = "rto22fiets";               PRM[prmrto22fiets]               = 150;  PRM_type[prmrto22fiets]               = TE_type; /* Ongehinderde rijtijd prioriteit fase 22                                         */
    PRM_code[prmrtbg22fiets]              = "rtbg22fiets";              PRM[prmrtbg22fiets]              = 200;  PRM_type[prmrtbg22fiets]              = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 22                                   */
    PRM_code[prmrtg22fiets]               = "rtg22fiets";               PRM[prmrtg22fiets]               = 300;  PRM_type[prmrtg22fiets]               = TE_type; /* Gehinderde rijtijd prioriteit fase 22                                           */
    PRM_code[prmomx22fiets]               = "omx22fiets";               PRM[prmomx22fiets]               = 50;   PRM_type[prmomx22fiets]               = TE_type; /* Ondermaximum OV fase 22                                                         */
    PRM_code[prmupinagb22fiets]           = "upinagb22fiets";           PRM[prmupinagb22fiets]           = 0;                                                     /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 22                   */
    PRM_code[prmprio22fiets]              = "prio22fiets";              PRM[prmprio22fiets]              = 0;                                                     /* Prioriteitsinstelling OV fase 22                                                */
    PRM_code[prmrto41bus]                 = "rto41bus";                 PRM[prmrto41bus]                 = 150;  PRM_type[prmrto41bus]                 = TE_type; /* Ongehinderde rijtijd prioriteit fase 41                                         */
    PRM_code[prmrtbg41bus]                = "rtbg41bus";                PRM[prmrtbg41bus]                = 200;  PRM_type[prmrtbg41bus]                = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 41                                   */
    PRM_code[prmrtg41bus]                 = "rtg41bus";                 PRM[prmrtg41bus]                 = 300;  PRM_type[prmrtg41bus]                 = TE_type; /* Gehinderde rijtijd prioriteit fase 41                                           */
    PRM_code[prmomx41bus]                 = "omx41bus";                 PRM[prmomx41bus]                 = 50;   PRM_type[prmomx41bus]                 = TE_type; /* Ondermaximum OV fase 41                                                         */
    PRM_code[prmupinagb41bus]             = "upinagb41bus";             PRM[prmupinagb41bus]             = 0;                                                     /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                   */
    PRM_code[prmprio41bus]                = "prio41bus";                PRM[prmprio41bus]                = 0;                                                     /* Prioriteitsinstelling OV fase 41                                                */
    PRM_code[prmstkp1]                    = "stkp1";                    PRM[prmstkp1]                    = 600;  PRM_type[prmstkp1]                    = TI_type; /* Start klokperiode ochtend                                                       */
    PRM_code[prmetkp1]                    = "etkp1";                    PRM[prmetkp1]                    = 900;  PRM_type[prmetkp1]                    = TI_type; /* Einde klokperiode ochtend                                                       */
    PRM_code[prmdckp1]                    = "dckp1";                    PRM[prmdckp1]                    = 8;                                                     /* Dagsoort klokperiode ochtend                                                    */
    PRM_code[prmmg1_02]                   = "mg1_02";                   PRM[prmmg1_02]                   = 300;  PRM_type[prmmg1_02]                   = TE_type; /* Maximale groentijd MG1 02                                                       */
    PRM_code[prmmg1_22]                   = "mg1_22";                   PRM[prmmg1_22]                   = 300;  PRM_type[prmmg1_22]                   = TE_type; /* Maximale groentijd MG1 22                                                       */
    PRM_code[prmmg1_41]                   = "mg1_41";                   PRM[prmmg1_41]                   = 300;  PRM_type[prmmg1_41]                   = TE_type; /* Maximale groentijd MG1 41                                                       */
    PRM_code[prmmg1_61a]                  = "mg1_61a";                  PRM[prmmg1_61a]                  = 300;  PRM_type[prmmg1_61a]                  = TE_type; /* Maximale groentijd MG1 61a                                                      */
    PRM_code[prmmg2_02]                   = "mg2_02";                   PRM[prmmg2_02]                   = 200;  PRM_type[prmmg2_02]                   = TE_type; /* Maximale groentijd MG2 02                                                       */
    PRM_code[prmmg2_22]                   = "mg2_22";                   PRM[prmmg2_22]                   = 200;  PRM_type[prmmg2_22]                   = TE_type; /* Maximale groentijd MG2 22                                                       */
    PRM_code[prmmg2_41]                   = "mg2_41";                   PRM[prmmg2_41]                   = 200;  PRM_type[prmmg2_41]                   = TE_type; /* Maximale groentijd MG2 41                                                       */
    PRM_code[prmmg2_61a]                  = "mg2_61a";                  PRM[prmmg2_61a]                  = 200;  PRM_type[prmmg2_61a]                  = TE_type; /* Maximale groentijd MG2 61a                                                      */
    PRM_code[prmmlfpr02]                  = "mlfpr02";                  PRM[prmmlfpr02]                  = 0;                                                     /* Maximaal aantal modules vooruit fase 02                                         */
    PRM_code[prmmlfpr22]                  = "mlfpr22";                  PRM[prmmlfpr22]                  = 0;                                                     /* Maximaal aantal modules vooruit fase 22                                         */
    PRM_code[prmmlfpr41]                  = "mlfpr41";                  PRM[prmmlfpr41]                  = 0;                                                     /* Maximaal aantal modules vooruit fase 41                                         */
    PRM_code[prmmlfpr61a]                 = "mlfpr61a";                 PRM[prmmlfpr61a]                 = 0;                                                     /* Maximaal aantal modules vooruit fase 61a                                        */
    PRM_code[prmaltg02]                   = "altg02";                   PRM[prmaltg02]                   = 0;    PRM_type[prmaltg02]                   = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 02                          */
    PRM_code[prmaltp02]                   = "altp02";                   PRM[prmaltp02]                   = 0;    PRM_type[prmaltp02]                   = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 02                             */
    PRM_code[prmaltg22]                   = "altg22";                   PRM[prmaltg22]                   = 0;    PRM_type[prmaltg22]                   = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 22                          */
    PRM_code[prmaltp22]                   = "altp22";                   PRM[prmaltp22]                   = 0;    PRM_type[prmaltp22]                   = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 22                             */
    PRM_code[prmaltg41]                   = "altg41";                   PRM[prmaltg41]                   = 0;    PRM_type[prmaltg41]                   = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 41                          */
    PRM_code[prmaltp41]                   = "altp41";                   PRM[prmaltp41]                   = 0;    PRM_type[prmaltp41]                   = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 41                             */
    PRM_code[prmaltg61a]                  = "altg61a";                  PRM[prmaltg61a]                  = 0;    PRM_type[prmaltg61a]                  = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 61a                         */
    PRM_code[prmaltp61a]                  = "altp61a";                  PRM[prmaltp61a]                  = 0;    PRM_type[prmaltp61a]                  = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 61a                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    PRM_code[prmtestdsivert]              = "testdsivert";              PRM[prmtestdsivert]              = 120;  PRM_type[prmtestdsivert]              = TS_type; /* Testen vertraging in DSI bericht in testomgeving                                */
    PRM_code[prmtestdsilyn]               = "testdsilyn";               PRM[prmtestdsilyn]               = 0;                                          /* Testen lijnnummer DSI bericht in testomgeving                                   */
    PRM_code[prmtestdsicat]               = "testdsicat";               PRM[prmtestdsicat]               = 10;                                         /* Testen ritcategorie DSI bericht in testomgeving                                 */
#endif


/* extra data */
/* ---------- */
    FC_type[fc02] = MVT_type;
    FC_type[fc22] = FTS_type;
    FC_type[fc41] = OV_type;
    FC_type[fc61a] = MVT_type;

/* Selectieve detectie */
/* ------------------- */
    DS_code[dsdummy] = "dsdummy";

/* modulen */
/* ------- */
    PRML[ML1][fc02] = PRIMAIR;

    PRML[ML2][fc22] = PRIMAIR;

    PRML[ML3][fc41] = PRIMAIR;

    PRML[ML4][fc61a] = PRIMAIR;


/* Typen ingangen */
/* -------------- */
    IS_type[d021] = DKOP_type;
    IS_type[d022] = DLNG_type;
    IS_type[d221] = DKOP_type;
    IS_type[d222] = DVER_type;
    IS_type[dk22] = DK_type;
    IS_type[d411] = DKOP_type;
    IS_type[d412] = DLNG_type;
    IS_type[d61a1] = DKOP_type;
    IS_type[d61a2] = DLNG_type;

/* Typen uitgangen */
/* --------------- */
    US_type[fc02] = MVT_type;
    US_type[fc22] = FTS_type;
    US_type[fc41] = OV_type;
    US_type[fc61a] = MVT_type;

#ifdef VISSIM
#endif

    #include "GMtab.add"
}
