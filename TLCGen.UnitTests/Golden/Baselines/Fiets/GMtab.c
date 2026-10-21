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
   FC_code[fc24] = "24"; TRG_max[fc24] = 0; TRG_min[fc24] = 0; TGG_max[fc24] = 0; TGG_min[fc24] = 0; TFG_max[fc24] = 0; TGL_max[fc24] = 0; TGL_min[fc24] = 0; TMGL_max[fc24] = 0; TVG_max[fc24] = 300;
   FC_code[fc31] = "31"; TRG_max[fc31] = 0; TRG_min[fc31] = 0; TGG_max[fc31] = 0; TGG_min[fc31] = 0; TFG_max[fc31] = 0; TGL_max[fc31] = 0; TGL_min[fc31] = 0; TMGL_max[fc31] = 0; TVG_max[fc31] = 300;

/* conflicten */
/* ---------- */

    TIG_max[fc02][fc22] = 30;
    TIG_max[fc02][fc24] = 30;
    TIG_max[fc02][fc31] = 30;

    TIG_max[fc22][fc02] = 30;
    TIG_max[fc22][fc24] = 30;
    TIG_max[fc22][fc31] = 30;

    TIG_max[fc24][fc02] = 30;
    TIG_max[fc24][fc22] = 30;
    TIG_max[fc24][fc31] = 30;

    TIG_max[fc31][fc02] = 30;
    TIG_max[fc31][fc22] = 30;
    TIG_max[fc31][fc24] = 30;

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
    US_code[usovinm22fiets] = "ovinm22fiets";                          /* Verklikken inmelding OV fase 22            */
    US_code[usovinm24fiets] = "ovinm24fiets";                          /* Verklikken inmelding OV fase 24            */
    US_code[usperdef]       = "perdef";                                /* Default periode actief                     */
    US_code[usper1]         = "per1";                                  /* Periode actief                             */

/* detectie */
/* -------- */
    D_code[d021] = "021"; 
    D_code[d022] = "022"; 
    D_code[d221] = "221"; 
    D_code[d222] = "222"; 
    D_code[dk22] = "k22"; 
    D_code[d241] = "241"; 
    D_code[d242] = "242"; 
    D_code[dk24] = "k24"; 
    D_code[d311] = "311"; 
    D_code[d312] = "312"; 

/* overige ingangen */
/* ---------------- */

/* hulp elementen */
/* -------------- */
    H_code[hprio22fiets]            = "prio22fiets";                                              /* Bijhouden actief zijn prioriteit fase 22 */
    H_code[hprioin22fiets]          = "prioin22fiets";                                            /* Prioriteit inmelding fase 22 Fiets       */
    H_code[hpriouit22fiets]         = "priouit22fiets";                                           /* Prioriteit uitmelding 22 Fiets           */
    H_code[hprioin22fietsfiets221]  = "prioin22fietsfiets221";                                    /* Prioriteit inmelding fase 22 Fiets       */
    H_code[hpriouit22fietsfiets221] = "priouit22fietsfiets221";                                   /* Prioriteit uitmelding 22 Fiets           */
    H_code[hprio24fiets]            = "prio24fiets";                                              /* Bijhouden actief zijn prioriteit fase 24 */
    H_code[hprioin24fiets]          = "prioin24fiets";                                            /* Prioriteit inmelding fase 24 Fiets       */
    H_code[hpriouit24fiets]         = "priouit24fiets";                                           /* Prioriteit uitmelding 24 Fiets           */
    H_code[hprioin24fietsfiets]     = "prioin24fietsfiets";                                       /* Prioriteit inmelding fase 24 Fiets       */
    H_code[hpriouit24fietsdet241]   = "priouit24fietsdet241";                                     /* Prioriteit uitmelding 24 Fiets           */

/* geheugen elementen */
/* ------------------ */
    MM_code[mperiod] = "period";                   /* Onthouden actieve periode   */
    MM_code[mar02]   = "ar02";                     /* Alternatieve ruimte fase 02 */
    MM_code[mar22]   = "ar22";                     /* Alternatieve ruimte fase 22 */
    MM_code[mar24]   = "ar24";                     /* Alternatieve ruimte fase 24 */
    MM_code[mar31]   = "ar31";                     /* Alternatieve ruimte fase 31 */

/* tijd elementen */
/* -------------- */
    T_code[tkm02]         = "km02";         T_max[tkm02]         = 0;   T_type[tkm02]         = TE_type; /* Kop maximum voor detector 02                          */
    T_code[tkm22]         = "km22";         T_max[tkm22]         = 0;   T_type[tkm22]         = TE_type; /* Kop maximum voor detector 22                          */
    T_code[tkm24]         = "km24";         T_max[tkm24]         = 0;   T_type[tkm24]         = TE_type; /* Kop maximum voor detector 24                          */
    T_code[tkm31]         = "km31";         T_max[tkm31]         = 0;   T_type[tkm31]         = TE_type; /* Kop maximum voor detector 31                          */
    T_code[tbtovg22fiets] = "btovg22fiets"; T_max[tbtovg22fiets] = 30;  T_type[tbtovg22fiets] = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 22 */
    T_code[trt22fiets]    = "rt22fiets";    T_max[trt22fiets]    = 0;   T_type[trt22fiets]    = TE_type; /* Actuele rijtijd prio fase 22                          */
    T_code[tgb22fiets]    = "gb22fiets";    T_max[tgb22fiets]    = 600; T_type[tgb22fiets]    = TE_type; /* Groenbewaking prioriteit fase 22                      */
    T_code[tblk22fiets]   = "blk22fiets";   T_max[tblk22fiets]   = 200; T_type[tblk22fiets]   = TE_type; /* Blokkeertijd na prioriteitsingreep fase 22            */
    T_code[tbtovg24fiets] = "btovg24fiets"; T_max[tbtovg24fiets] = 30;  T_type[tbtovg24fiets] = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 24 */
    T_code[trt24fiets]    = "rt24fiets";    T_max[trt24fiets]    = 0;   T_type[trt24fiets]    = TE_type; /* Actuele rijtijd prio fase 24                          */
    T_code[tgb24fiets]    = "gb24fiets";    T_max[tgb24fiets]    = 600; T_type[tgb24fiets]    = TE_type; /* Groenbewaking prioriteit fase 24                      */
    T_code[tblk24fiets]   = "blk24fiets";   T_max[tblk24fiets]   = 200; T_type[tblk24fiets]   = TE_type; /* Blokkeertijd na prioriteitsingreep fase 24            */

/* teller elementen */
/* ---------------- */
    C_code[cftscyc22fietsfiets] = "ftscyc22fietsfiets"; C_max[cftscyc22fietsfiets] = 999;                                        /* Bijhouden realisaties tbv peloton prio voor fase 22     */
    C_code[cftsvtg22fietsfiets] = "ftsvtg22fietsfiets"; C_max[cftsvtg22fietsfiets] = 999;                                        /* Bijhouden aantal fietsers tbv peloton prio voor fase 22 */
    C_code[cvc22fiets]          = "vc22fiets";          C_max[cvc22fiets]          = 999; C_type[cvc22fiets]          = CT_type; /* Bijhouden prio inmeldingen fase 22 type Fiets           */
    C_code[cftscyc24fietsfiets] = "ftscyc24fietsfiets"; C_max[cftscyc24fietsfiets] = 999;                                        /* Bijhouden realisaties tbv peloton prio voor fase 24     */
    C_code[cvc24fiets]          = "vc24fiets";          C_max[cvc24fiets]          = 999; C_type[cvc24fiets]          = CT_type; /* Bijhouden prio inmeldingen fase 24 type Fiets           */

/* schakelaars */
/* ----------- */
    SCH_code[schcheckdstype]            = "checkdstype";            SCH[schcheckdstype]            = 0;                                         /* Check type DSI bericht bij VECOM         */
    SCH_code[schprioin22fietsfiets221]  = "prioin22fietsfiets221";  SCH[schprioin22fietsfiets221]  = 1;                                         /* Inmelden 22 via Fiets toestaan           */
    SCH_code[schpriouit22fietsfiets221] = "priouit22fietsfiets221"; SCH[schpriouit22fietsfiets221] = 1;                                         /* Uitmelden 22 via Fiets toestaan          */
    SCH_code[schprioin24fietsfiets]     = "prioin24fietsfiets";     SCH[schprioin24fietsfiets]     = 1;                                         /* Inmelden 24 via Fiets toestaan           */
    SCH_code[schpriouit24fietsdet241ED] = "priouit24fietsdet241ED"; SCH[schpriouit24fietsdet241ED] = 1;                                         /* Uitmelden 24 via Fiets toestaan          */
    SCH_code[schaltg02]                 = "altg02";                 SCH[schaltg02]                 = 0;                                         /* Alternatieve realisatie toestaan fase 02 */
    SCH_code[schaltg22]                 = "altg22";                 SCH[schaltg22]                 = 0;                                         /* Alternatieve realisatie toestaan fase 22 */
    SCH_code[schaltg24]                 = "altg24";                 SCH[schaltg24]                 = 0;                                         /* Alternatieve realisatie toestaan fase 24 */
    SCH_code[schaltg31]                 = "altg31";                 SCH[schaltg31]                 = 0;                                         /* Alternatieve realisatie toestaan fase 31 */

/* parameters */
/* ---------- */
    PRM_code[prmfb]                       = "fb";                       PRM[prmfb]                       = 0;   PRM_type[prmfb]                       = TS_type; /* Instelling fasebewaking                                                        */
    PRM_code[prmxx]                       = "xx";                       PRM[prmxx]                       = 0;                                                    /* Versiebeheer xx                                                                */
    PRM_code[prmyy]                       = "yy";                       PRM[prmyy]                       = 0;                                                    /* Versiebeheer yy                                                                */
    PRM_code[prmzz]                       = "zz";                       PRM[prmzz]                       = 0;                                                    /* Versiebeheer zz                                                                */
    PRM_code[prmmk021]                    = "mk021";                    PRM[prmmk021]                    = 0;                                                    /* Meetkriterium type voor detector 021                                           */
    PRM_code[prmmk022]                    = "mk022";                    PRM[prmmk022]                    = 0;                                                    /* Meetkriterium type voor detector 022                                           */
    PRM_code[prmmk221]                    = "mk221";                    PRM[prmmk221]                    = 0;                                                    /* Meetkriterium type voor detector 221                                           */
    PRM_code[prmmk222]                    = "mk222";                    PRM[prmmk222]                    = 0;                                                    /* Meetkriterium type voor detector 222                                           */
    PRM_code[prmmkk22]                    = "mkk22";                    PRM[prmmkk22]                    = 0;                                                    /* Meetkriterium type voor detector k22                                           */
    PRM_code[prmmk241]                    = "mk241";                    PRM[prmmk241]                    = 0;                                                    /* Meetkriterium type voor detector 241                                           */
    PRM_code[prmmk242]                    = "mk242";                    PRM[prmmk242]                    = 0;                                                    /* Meetkriterium type voor detector 242                                           */
    PRM_code[prmmkk24]                    = "mkk24";                    PRM[prmmkk24]                    = 0;                                                    /* Meetkriterium type voor detector k24                                           */
    PRM_code[prmmk311]                    = "mk311";                    PRM[prmmk311]                    = 0;                                                    /* Meetkriterium type voor detector 311                                           */
    PRM_code[prmmk312]                    = "mk312";                    PRM[prmmk312]                    = 0;                                                    /* Meetkriterium type voor detector 312                                           */
    PRM_code[prmmwta]                     = "mwta";                     PRM[prmmwta]                     = 0;   PRM_type[prmmwta]                     = TS_type; /* Maximale wachttijd autoverkeer                                                 */
    PRM_code[prmmwtfts]                   = "mwtfts";                   PRM[prmmwtfts]                   = 0;   PRM_type[prmmwtfts]                   = TS_type; /* Maximale wachttijd fiets                                                       */
    PRM_code[prmmwtvtg]                   = "mwtvtg";                   PRM[prmmwtvtg]                   = 0;   PRM_type[prmmwtvtg]                   = TS_type; /* Maximale wachttijd voetgangers                                                 */
    PRM_code[prmpmgt02]                   = "pmgt02";                   PRM[prmpmgt02]                   = 0;                                                    /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                  */
    PRM_code[prmognt02]                   = "ognt02";                   PRM[prmognt02]                   = 0;   PRM_type[prmognt02]                   = TE_type; /* Minimale groentijd bij terugkomen fase 02                                      */
    PRM_code[prmnofm02]                   = "nofm02";                   PRM[prmnofm02]                   = 0;   PRM_type[prmnofm02]                   = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 02                               */
    PRM_code[prmmgcov02]                  = "mgcov02";                  PRM[prmmgcov02]                  = 0;   PRM_type[prmmgcov02]                  = TE_type; /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov02]                 = "pmgcov02";                 PRM[prmpmgcov02]                 = 0;                                                    /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg02]                  = "ohpmg02";                  PRM[prmohpmg02]                  = 0;                                                    /* Percentage ophogen groentijd na afkappen fase 02                               */
    PRM_code[prmpmgt22]                   = "pmgt22";                   PRM[prmpmgt22]                   = 0;                                                    /* Minimaal percentage groentijd primair tbv. terugkomen fase 22                  */
    PRM_code[prmognt22]                   = "ognt22";                   PRM[prmognt22]                   = 0;   PRM_type[prmognt22]                   = TE_type; /* Minimale groentijd bij terugkomen fase 22                                      */
    PRM_code[prmnofm22]                   = "nofm22";                   PRM[prmnofm22]                   = 0;   PRM_type[prmnofm22]                   = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 22                               */
    PRM_code[prmmgcov22]                  = "mgcov22";                  PRM[prmmgcov22]                  = 0;   PRM_type[prmmgcov22]                  = TE_type; /* Minimum groentijd waarna fase 22 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov22]                 = "pmgcov22";                 PRM[prmpmgcov22]                 = 0;                                                    /* Minimum percentage groentijd waarna fase 22 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg22]                  = "ohpmg22";                  PRM[prmohpmg22]                  = 0;                                                    /* Percentage ophogen groentijd na afkappen fase 22                               */
    PRM_code[prmpmgt24]                   = "pmgt24";                   PRM[prmpmgt24]                   = 0;                                                    /* Minimaal percentage groentijd primair tbv. terugkomen fase 24                  */
    PRM_code[prmognt24]                   = "ognt24";                   PRM[prmognt24]                   = 0;   PRM_type[prmognt24]                   = TE_type; /* Minimale groentijd bij terugkomen fase 24                                      */
    PRM_code[prmnofm24]                   = "nofm24";                   PRM[prmnofm24]                   = 0;   PRM_type[prmnofm24]                   = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 24                               */
    PRM_code[prmmgcov24]                  = "mgcov24";                  PRM[prmmgcov24]                  = 0;   PRM_type[prmmgcov24]                  = TE_type; /* Minimum groentijd waarna fase 24 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov24]                 = "pmgcov24";                 PRM[prmpmgcov24]                 = 0;                                                    /* Minimum percentage groentijd waarna fase 24 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg24]                  = "ohpmg24";                  PRM[prmohpmg24]                  = 0;                                                    /* Percentage ophogen groentijd na afkappen fase 24                               */
    PRM_code[prmpmgt31]                   = "pmgt31";                   PRM[prmpmgt31]                   = 0;                                                    /* Minimaal percentage groentijd primair tbv. terugkomen fase 31                  */
    PRM_code[prmognt31]                   = "ognt31";                   PRM[prmognt31]                   = 0;   PRM_type[prmognt31]                   = TE_type; /* Minimale groentijd bij terugkomen fase 31                                      */
    PRM_code[prmftsblok22fietsfiets]      = "ftsblok22fietsfiets";      PRM[prmftsblok22fietsfiets]      = 1;                                                    /* Blokken waarin fiets peloton prio actief mag zijn voor fase 22                 */
    PRM_code[prmftsmaxpercyc22fietsfiets] = "ftsmaxpercyc22fietsfiets"; PRM[prmftsmaxpercyc22fietsfiets] = 1;                                                    /* Maximaal aantal keer fiets peloton prio per cyclus voor fase 22                */
    PRM_code[prmftsminvtg22fietsfiets]    = "ftsminvtg22fietsfiets";    PRM[prmftsminvtg22fietsfiets]    = 3;                                                    /* Minimaal aantal gemeten fietsers tbv peloton prio voor fase 22                 */
    PRM_code[prmftsminwt22fietsfiets]     = "ftsminwt22fietsfiets";     PRM[prmftsminwt22fietsfiets]     = 10;  PRM_type[prmftsminwt22fietsfiets]     = TS_type; /* Minimale wachttijd tbv peloton prio voor fase 22                               */
    PRM_code[prmrto22fiets]               = "rto22fiets";               PRM[prmrto22fiets]               = 150; PRM_type[prmrto22fiets]               = TE_type; /* Ongehinderde rijtijd prioriteit fase 22                                        */
    PRM_code[prmrtbg22fiets]              = "rtbg22fiets";              PRM[prmrtbg22fiets]              = 200; PRM_type[prmrtbg22fiets]              = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 22                                  */
    PRM_code[prmrtg22fiets]               = "rtg22fiets";               PRM[prmrtg22fiets]               = 300; PRM_type[prmrtg22fiets]               = TE_type; /* Gehinderde rijtijd prioriteit fase 22                                          */
    PRM_code[prmomx22fiets]               = "omx22fiets";               PRM[prmomx22fiets]               = 50;  PRM_type[prmomx22fiets]               = TE_type; /* Ondermaximum OV fase 22                                                        */
    PRM_code[prmupinagb22fiets]           = "upinagb22fiets";           PRM[prmupinagb22fiets]           = 0;                                                    /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 22                  */
    PRM_code[prmprio22fiets]              = "prio22fiets";              PRM[prmprio22fiets]              = 0;                                                    /* Prioriteitsinstelling OV fase 22                                               */
    PRM_code[prmftsblok24fietsfiets]      = "ftsblok24fietsfiets";      PRM[prmftsblok24fietsfiets]      = 2;                                                    /* Blokken waarin fiets peloton prio actief mag zijn voor fase 24                 */
    PRM_code[prmftsmaxpercyc24fietsfiets] = "ftsmaxpercyc24fietsfiets"; PRM[prmftsmaxpercyc24fietsfiets] = 2;                                                    /* Maximaal aantal keer fiets peloton prio per cyclus voor fase 24                */
    PRM_code[prmftsminwt24fietsfiets]     = "ftsminwt24fietsfiets";     PRM[prmftsminwt24fietsfiets]     = 20;  PRM_type[prmftsminwt24fietsfiets]     = TS_type; /* Minimale wachttijd tbv peloton prio voor fase 24                               */
    PRM_code[prmrto24fiets]               = "rto24fiets";               PRM[prmrto24fiets]               = 150; PRM_type[prmrto24fiets]               = TE_type; /* Ongehinderde rijtijd prioriteit fase 24                                        */
    PRM_code[prmrtbg24fiets]              = "rtbg24fiets";              PRM[prmrtbg24fiets]              = 200; PRM_type[prmrtbg24fiets]              = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 24                                  */
    PRM_code[prmrtg24fiets]               = "rtg24fiets";               PRM[prmrtg24fiets]               = 300; PRM_type[prmrtg24fiets]               = TE_type; /* Gehinderde rijtijd prioriteit fase 24                                          */
    PRM_code[prmomx24fiets]               = "omx24fiets";               PRM[prmomx24fiets]               = 50;  PRM_type[prmomx24fiets]               = TE_type; /* Ondermaximum OV fase 24                                                        */
    PRM_code[prmupinagb24fiets]           = "upinagb24fiets";           PRM[prmupinagb24fiets]           = 0;                                                    /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 24                  */
    PRM_code[prmprio24fiets]              = "prio24fiets";              PRM[prmprio24fiets]              = 0;                                                    /* Prioriteitsinstelling OV fase 24                                               */
    PRM_code[prmstkp1]                    = "stkp1";                    PRM[prmstkp1]                    = 600; PRM_type[prmstkp1]                    = TI_type; /* Start klokperiode ochtend                                                      */
    PRM_code[prmetkp1]                    = "etkp1";                    PRM[prmetkp1]                    = 900; PRM_type[prmetkp1]                    = TI_type; /* Einde klokperiode ochtend                                                      */
    PRM_code[prmdckp1]                    = "dckp1";                    PRM[prmdckp1]                    = 8;                                                    /* Dagsoort klokperiode ochtend                                                   */
    PRM_code[prmmg1_02]                   = "mg1_02";                   PRM[prmmg1_02]                   = 300; PRM_type[prmmg1_02]                   = TE_type; /* Maximale groentijd MG1 02                                                      */
    PRM_code[prmmg1_22]                   = "mg1_22";                   PRM[prmmg1_22]                   = 300; PRM_type[prmmg1_22]                   = TE_type; /* Maximale groentijd MG1 22                                                      */
    PRM_code[prmmg1_24]                   = "mg1_24";                   PRM[prmmg1_24]                   = 300; PRM_type[prmmg1_24]                   = TE_type; /* Maximale groentijd MG1 24                                                      */
    PRM_code[prmmg1_31]                   = "mg1_31";                   PRM[prmmg1_31]                   = 300; PRM_type[prmmg1_31]                   = TE_type; /* Maximale groentijd MG1 31                                                      */
    PRM_code[prmmg2_02]                   = "mg2_02";                   PRM[prmmg2_02]                   = 200; PRM_type[prmmg2_02]                   = TE_type; /* Maximale groentijd MG2 02                                                      */
    PRM_code[prmmg2_22]                   = "mg2_22";                   PRM[prmmg2_22]                   = 200; PRM_type[prmmg2_22]                   = TE_type; /* Maximale groentijd MG2 22                                                      */
    PRM_code[prmmg2_24]                   = "mg2_24";                   PRM[prmmg2_24]                   = 200; PRM_type[prmmg2_24]                   = TE_type; /* Maximale groentijd MG2 24                                                      */
    PRM_code[prmmg2_31]                   = "mg2_31";                   PRM[prmmg2_31]                   = 200; PRM_type[prmmg2_31]                   = TE_type; /* Maximale groentijd MG2 31                                                      */
    PRM_code[prmmlfpr02]                  = "mlfpr02";                  PRM[prmmlfpr02]                  = 0;                                                    /* Maximaal aantal modules vooruit fase 02                                        */
    PRM_code[prmmlfpr22]                  = "mlfpr22";                  PRM[prmmlfpr22]                  = 0;                                                    /* Maximaal aantal modules vooruit fase 22                                        */
    PRM_code[prmmlfpr24]                  = "mlfpr24";                  PRM[prmmlfpr24]                  = 0;                                                    /* Maximaal aantal modules vooruit fase 24                                        */
    PRM_code[prmmlfpr31]                  = "mlfpr31";                  PRM[prmmlfpr31]                  = 0;                                                    /* Maximaal aantal modules vooruit fase 31                                        */
    PRM_code[prmaltg02]                   = "altg02";                   PRM[prmaltg02]                   = 0;   PRM_type[prmaltg02]                   = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    PRM_code[prmaltp02]                   = "altp02";                   PRM[prmaltp02]                   = 0;   PRM_type[prmaltp02]                   = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    PRM_code[prmaltg22]                   = "altg22";                   PRM[prmaltg22]                   = 0;   PRM_type[prmaltg22]                   = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 22                         */
    PRM_code[prmaltp22]                   = "altp22";                   PRM[prmaltp22]                   = 0;   PRM_type[prmaltp22]                   = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 22                            */
    PRM_code[prmaltg24]                   = "altg24";                   PRM[prmaltg24]                   = 0;   PRM_type[prmaltg24]                   = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 24                         */
    PRM_code[prmaltp24]                   = "altp24";                   PRM[prmaltp24]                   = 0;   PRM_type[prmaltp24]                   = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 24                            */
    PRM_code[prmaltg31]                   = "altg31";                   PRM[prmaltg31]                   = 0;   PRM_type[prmaltg31]                   = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 31                         */
    PRM_code[prmaltp31]                   = "altp31";                   PRM[prmaltp31]                   = 0;   PRM_type[prmaltp31]                   = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 31                            */


/* extra data */
/* ---------- */
    FC_type[fc02] = MVT_type;
    FC_type[fc22] = FTS_type;
    FC_type[fc24] = FTS_type;
    FC_type[fc31] = VTG_type;

/* modulen */
/* ------- */
    PRML[ML1][fc02] = PRIMAIR;

    PRML[ML2][fc22] = PRIMAIR;

    PRML[ML3][fc24] = PRIMAIR;

    PRML[ML4][fc31] = PRIMAIR;


/* Typen ingangen */
/* -------------- */
    IS_type[d021] = DKOP_type;
    IS_type[d022] = DLNG_type;
    IS_type[d221] = DKOP_type;
    IS_type[d222] = DVER_type;
    IS_type[dk22] = DK_type;
    IS_type[d241] = DKOP_type;
    IS_type[d242] = DVER_type;
    IS_type[dk24] = DK_type;
    IS_type[d311] = DK_type;
    IS_type[d312] = DK_type;

/* Typen uitgangen */
/* --------------- */
    US_type[fc02] = MVT_type;
    US_type[fc22] = FTS_type;
    US_type[fc24] = FTS_type;
    US_type[fc31] = VTG_type;

#ifdef VISSIM
#endif

    #include "GMtab.add"
}
