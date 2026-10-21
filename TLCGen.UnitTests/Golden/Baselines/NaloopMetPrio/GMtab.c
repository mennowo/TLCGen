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
   FC_code[fc08] = "08"; TRG_max[fc08] = 0; TRG_min[fc08] = 0; TGG_max[fc08] = 0; TGG_min[fc08] = 0; TFG_max[fc08] = 0; TGL_max[fc08] = 0; TGL_min[fc08] = 0; TMGL_max[fc08] = 0; TVG_max[fc08] = 300;
   FC_code[fc09] = "09"; TRG_max[fc09] = 0; TRG_min[fc09] = 0; TGG_max[fc09] = 0; TGG_min[fc09] = 0; TFG_max[fc09] = 0; TGL_max[fc09] = 0; TGL_min[fc09] = 0; TMGL_max[fc09] = 0; TVG_max[fc09] = 300;
   FC_code[fc41] = "41"; TRG_max[fc41] = 0; TRG_min[fc41] = 0; TGG_max[fc41] = 0; TGG_min[fc41] = 0; TFG_max[fc41] = 0; TGL_max[fc41] = 0; TGL_min[fc41] = 0; TMGL_max[fc41] = 0; TVG_max[fc41] = 300;

/* conflicten */
/* ---------- */

    TIG_max[fc02][fc05] = FK;

    TIG_max[fc05][fc02] = FK;

    TIG_max[fc02][fc05] = 30;
    TIG_max[fc02][fc09] = 30;
    TIG_max[fc02][fc41] = 30;

    TIG_max[fc05][fc02] = 30;
    TIG_max[fc05][fc08] = 30;
    TIG_max[fc05][fc41] = 30;

    TIG_max[fc08][fc05] = 30;
    TIG_max[fc08][fc09] = 30;
    TIG_max[fc08][fc41] = 30;

    TIG_max[fc09][fc02] = 30;
    TIG_max[fc09][fc08] = 30;
    TIG_max[fc09][fc41] = 30;

    TIG_max[fc41][fc02] = 30;
    TIG_max[fc41][fc05] = 30;
    TIG_max[fc41][fc08] = 30;
    TIG_max[fc41][fc09] = 30;

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
    US_code[usperdef]     = "perdef";                            /* Default periode actief                     */
    US_code[usper1]       = "per1";                              /* Periode actief                             */

/* detectie */
/* -------- */
    D_code[d021] = "021"; 
    D_code[d022] = "022"; 
    D_code[d051] = "051"; 
    D_code[d052] = "052"; 
    D_code[d081] = "081"; 
    D_code[d082] = "082"; 
    D_code[d091] = "091"; 
    D_code[d092] = "092"; 
    D_code[d411] = "411"; 
    D_code[d412] = "412"; 
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined PRACTICE_TEST
    D_code[ddummykarin41bus]  = "dummykarin41bus";  
    D_code[ddummykaruit41bus] = "dummykaruit41bus"; 
#endif

/* overige ingangen */
/* ---------------- */

/* hulp elementen */
/* -------------- */
    H_code[hprio41bus]       = "prio41bus";                                  /* Bijhouden actief zijn prioriteit fase 41 */
    H_code[hprioin41bus]     = "prioin41bus";                                /* Prioriteit inmelding fase 41 Bus         */
    H_code[hpriouit41bus]    = "priouit41bus";                               /* Prioriteit uitmelding 41 Bus             */
    H_code[hprioin41buskar]  = "prioin41buskar";                             /* Prioriteit inmelding fase 41 Bus         */
    H_code[hpriouit41buskar] = "priouit41buskar";                            /* Prioriteit uitmelding 41 Bus             */

/* geheugen elementen */
/* ------------------ */
    MM_code[mperiod] = "period";                   /* Onthouden actieve periode   */
    MM_code[mar02]   = "ar02";                     /* Alternatieve ruimte fase 02 */
    MM_code[mar05]   = "ar05";                     /* Alternatieve ruimte fase 05 */
    MM_code[mar08]   = "ar08";                     /* Alternatieve ruimte fase 08 */
    MM_code[mar09]   = "ar09";                     /* Alternatieve ruimte fase 09 */
    MM_code[mar41]   = "ar41";                     /* Alternatieve ruimte fase 41 */

/* tijd elementen */
/* -------------- */
    T_code[tkm02]       = "km02";       T_max[tkm02]       = 0;    T_type[tkm02]       = TE_type; /* Kop maximum voor detector 02                          */
    T_code[tkm05]       = "km05";       T_max[tkm05]       = 0;    T_type[tkm05]       = TE_type; /* Kop maximum voor detector 05                          */
    T_code[tkm08]       = "km08";       T_max[tkm08]       = 0;    T_type[tkm08]       = TE_type; /* Kop maximum voor detector 08                          */
    T_code[tkm09]       = "km09";       T_max[tkm09]       = 0;    T_type[tkm09]       = TE_type; /* Kop maximum voor detector 09                          */
    T_code[tkm41]       = "km41";       T_max[tkm41]       = 0;    T_type[tkm41]       = TE_type; /* Kop maximum voor detector 41                          */
    T_code[tnleg0208]   = "nleg0208";   T_max[tnleg0208]   = 50;   T_type[tnleg0208]   = TE_type; /* Naloop op einde groen van 02 naar 08                  */
    T_code[tnlcv0509]   = "nlcv0509";   T_max[tnlcv0509]   = 30;   T_type[tnlcv0509]   = TE_type; /* Naloop tijdens cyclisch verlenggroen van 05 naar 09   */
    T_code[tkarmelding] = "karmelding"; T_max[tkarmelding] = 15;   T_type[tkarmelding] = TE_type; /* Duur verklikking ontvangst melding KAR                */
    T_code[tkarog]      = "karog";      T_max[tkarog]      = 1440; T_type[tkarog]      = TM_type; /* Ondergedrag KAR                                       */
    T_code[tbtovg41bus] = "btovg41bus"; T_max[tbtovg41bus] = 30;   T_type[tbtovg41bus] = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    T_code[trt41bus]    = "rt41bus";    T_max[trt41bus]    = 0;    T_type[trt41bus]    = TE_type; /* Actuele rijtijd prio fase 41                          */
    T_code[tgb41bus]    = "gb41bus";    T_max[tgb41bus]    = 600;  T_type[tgb41bus]    = TE_type; /* Groenbewaking prioriteit fase 41                      */
    T_code[tblk41bus]   = "blk41bus";   T_max[tblk41bus]   = 200;  T_type[tblk41bus]   = TE_type; /* Blokkeertijd na prioriteitsingreep fase 41            */

/* teller elementen */
/* ---------------- */
    C_code[cvc41bus] = "vc41bus"; C_max[cvc41bus] = 999; C_type[cvc41bus] = CT_type; /* Bijhouden prio inmeldingen fase 41 type Bus */

/* schakelaars */
/* ----------- */
    SCH_code[schcheckdstype]     = "checkdstype";     SCH[schcheckdstype]     = 0;                                  /* Check type DSI bericht bij VECOM         */
    SCH_code[schprioin41buskar]  = "prioin41buskar";  SCH[schprioin41buskar]  = 1;                                  /* Inmelden 41 via Bus toestaan             */
    SCH_code[schpriouit41buskar] = "priouit41buskar"; SCH[schpriouit41buskar] = 1;                                  /* Uitmelden 41 via Bus toestaan            */
    SCH_code[schaltg02]          = "altg02";          SCH[schaltg02]          = 0;                                  /* Alternatieve realisatie toestaan fase 02 */
    SCH_code[schaltg05]          = "altg05";          SCH[schaltg05]          = 0;                                  /* Alternatieve realisatie toestaan fase 05 */
    SCH_code[schaltg08]          = "altg08";          SCH[schaltg08]          = 0;                                  /* Alternatieve realisatie toestaan fase 08 */
    SCH_code[schaltg09]          = "altg09";          SCH[schaltg09]          = 0;                                  /* Alternatieve realisatie toestaan fase 09 */
    SCH_code[schaltg41]          = "altg41";          SCH[schaltg41]          = 0;                                  /* Alternatieve realisatie toestaan fase 41 */

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
    PRM_code[prmmk081]        = "mk081";        PRM[prmmk081]        = 0;                                        /* Meetkriterium type voor detector 081                                           */
    PRM_code[prmmk082]        = "mk082";        PRM[prmmk082]        = 0;                                        /* Meetkriterium type voor detector 082                                           */
    PRM_code[prmmk091]        = "mk091";        PRM[prmmk091]        = 0;                                        /* Meetkriterium type voor detector 091                                           */
    PRM_code[prmmk092]        = "mk092";        PRM[prmmk092]        = 0;                                        /* Meetkriterium type voor detector 092                                           */
    PRM_code[prmmk411]        = "mk411";        PRM[prmmk411]        = 0;                                        /* Meetkriterium type voor detector 411                                           */
    PRM_code[prmmk412]        = "mk412";        PRM[prmmk412]        = 0;                                        /* Meetkriterium type voor detector 412                                           */
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
    PRM_code[prmpmgt08]       = "pmgt08";       PRM[prmpmgt08]       = 0;                                        /* Minimaal percentage groentijd primair tbv. terugkomen fase 08                  */
    PRM_code[prmognt08]       = "ognt08";       PRM[prmognt08]       = 0;   PRM_type[prmognt08]       = TE_type; /* Minimale groentijd bij terugkomen fase 08                                      */
    PRM_code[prmnofm08]       = "nofm08";       PRM[prmnofm08]       = 0;   PRM_type[prmnofm08]       = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 08                               */
    PRM_code[prmmgcov08]      = "mgcov08";      PRM[prmmgcov08]      = 0;   PRM_type[prmmgcov08]      = TE_type; /* Minimum groentijd waarna fase 08 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov08]     = "pmgcov08";     PRM[prmpmgcov08]     = 0;                                        /* Minimum percentage groentijd waarna fase 08 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg08]      = "ohpmg08";      PRM[prmohpmg08]      = 0;                                        /* Percentage ophogen groentijd na afkappen fase 08                               */
    PRM_code[prmpmgt09]       = "pmgt09";       PRM[prmpmgt09]       = 0;                                        /* Minimaal percentage groentijd primair tbv. terugkomen fase 09                  */
    PRM_code[prmognt09]       = "ognt09";       PRM[prmognt09]       = 0;   PRM_type[prmognt09]       = TE_type; /* Minimale groentijd bij terugkomen fase 09                                      */
    PRM_code[prmnofm09]       = "nofm09";       PRM[prmnofm09]       = 0;   PRM_type[prmnofm09]       = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 09                               */
    PRM_code[prmmgcov09]      = "mgcov09";      PRM[prmmgcov09]      = 0;   PRM_type[prmmgcov09]      = TE_type; /* Minimum groentijd waarna fase 09 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov09]     = "pmgcov09";     PRM[prmpmgcov09]     = 0;                                        /* Minimum percentage groentijd waarna fase 09 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg09]      = "ohpmg09";      PRM[prmohpmg09]      = 0;                                        /* Percentage ophogen groentijd na afkappen fase 09                               */
    PRM_code[prmrto41bus]     = "rto41bus";     PRM[prmrto41bus]     = 150; PRM_type[prmrto41bus]     = TE_type; /* Ongehinderde rijtijd prioriteit fase 41                                        */
    PRM_code[prmrtbg41bus]    = "rtbg41bus";    PRM[prmrtbg41bus]    = 200; PRM_type[prmrtbg41bus]    = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    PRM_code[prmrtg41bus]     = "rtg41bus";     PRM[prmrtg41bus]     = 300; PRM_type[prmrtg41bus]     = TE_type; /* Gehinderde rijtijd prioriteit fase 41                                          */
    PRM_code[prmomx41bus]     = "omx41bus";     PRM[prmomx41bus]     = 50;  PRM_type[prmomx41bus]     = TE_type; /* Ondermaximum OV fase 41                                                        */
    PRM_code[prmupinagb41bus] = "upinagb41bus"; PRM[prmupinagb41bus] = 0;                                        /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    PRM_code[prmvtgcat41bus]  = "vtgcat41bus";  PRM[prmvtgcat41bus]  = 1;                                        /* Voertuigcategorie DSI voor prio ingreep 41bus                                  */
    PRM_code[prmprio41bus]    = "prio41bus";    PRM[prmprio41bus]    = 0;                                        /* Prioriteitsinstelling OV fase 41                                               */
    PRM_code[prmstkp1]        = "stkp1";        PRM[prmstkp1]        = 600; PRM_type[prmstkp1]        = TI_type; /* Start klokperiode ochtend                                                      */
    PRM_code[prmetkp1]        = "etkp1";        PRM[prmetkp1]        = 900; PRM_type[prmetkp1]        = TI_type; /* Einde klokperiode ochtend                                                      */
    PRM_code[prmdckp1]        = "dckp1";        PRM[prmdckp1]        = 8;                                        /* Dagsoort klokperiode ochtend                                                   */
    PRM_code[prmmg1_02]       = "mg1_02";       PRM[prmmg1_02]       = 300; PRM_type[prmmg1_02]       = TE_type; /* Maximale groentijd MG1 02                                                      */
    PRM_code[prmmg1_05]       = "mg1_05";       PRM[prmmg1_05]       = 300; PRM_type[prmmg1_05]       = TE_type; /* Maximale groentijd MG1 05                                                      */
    PRM_code[prmmg1_08]       = "mg1_08";       PRM[prmmg1_08]       = 300; PRM_type[prmmg1_08]       = TE_type; /* Maximale groentijd MG1 08                                                      */
    PRM_code[prmmg1_09]       = "mg1_09";       PRM[prmmg1_09]       = 300; PRM_type[prmmg1_09]       = TE_type; /* Maximale groentijd MG1 09                                                      */
    PRM_code[prmmg1_41]       = "mg1_41";       PRM[prmmg1_41]       = 300; PRM_type[prmmg1_41]       = TE_type; /* Maximale groentijd MG1 41                                                      */
    PRM_code[prmmg2_02]       = "mg2_02";       PRM[prmmg2_02]       = 200; PRM_type[prmmg2_02]       = TE_type; /* Maximale groentijd MG2 02                                                      */
    PRM_code[prmmg2_05]       = "mg2_05";       PRM[prmmg2_05]       = 200; PRM_type[prmmg2_05]       = TE_type; /* Maximale groentijd MG2 05                                                      */
    PRM_code[prmmg2_08]       = "mg2_08";       PRM[prmmg2_08]       = 200; PRM_type[prmmg2_08]       = TE_type; /* Maximale groentijd MG2 08                                                      */
    PRM_code[prmmg2_09]       = "mg2_09";       PRM[prmmg2_09]       = 200; PRM_type[prmmg2_09]       = TE_type; /* Maximale groentijd MG2 09                                                      */
    PRM_code[prmmg2_41]       = "mg2_41";       PRM[prmmg2_41]       = 200; PRM_type[prmmg2_41]       = TE_type; /* Maximale groentijd MG2 41                                                      */
    PRM_code[prmmlfpr02]      = "mlfpr02";      PRM[prmmlfpr02]      = 0;                                        /* Maximaal aantal modules vooruit fase 02                                        */
    PRM_code[prmmlfpr05]      = "mlfpr05";      PRM[prmmlfpr05]      = 0;                                        /* Maximaal aantal modules vooruit fase 05                                        */
    PRM_code[prmmlfpr08]      = "mlfpr08";      PRM[prmmlfpr08]      = 0;                                        /* Maximaal aantal modules vooruit fase 08                                        */
    PRM_code[prmmlfpr09]      = "mlfpr09";      PRM[prmmlfpr09]      = 0;                                        /* Maximaal aantal modules vooruit fase 09                                        */
    PRM_code[prmmlfpr41]      = "mlfpr41";      PRM[prmmlfpr41]      = 0;                                        /* Maximaal aantal modules vooruit fase 41                                        */
    PRM_code[prmaltg02]       = "altg02";       PRM[prmaltg02]       = 0;   PRM_type[prmaltg02]       = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    PRM_code[prmaltp02]       = "altp02";       PRM[prmaltp02]       = 0;   PRM_type[prmaltp02]       = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    PRM_code[prmaltg05]       = "altg05";       PRM[prmaltg05]       = 0;   PRM_type[prmaltg05]       = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 05                         */
    PRM_code[prmaltp05]       = "altp05";       PRM[prmaltp05]       = 0;   PRM_type[prmaltp05]       = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 05                            */
    PRM_code[prmaltg08]       = "altg08";       PRM[prmaltg08]       = 0;   PRM_type[prmaltg08]       = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 08                         */
    PRM_code[prmaltp08]       = "altp08";       PRM[prmaltp08]       = 0;   PRM_type[prmaltp08]       = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 08                            */
    PRM_code[prmaltg09]       = "altg09";       PRM[prmaltg09]       = 0;   PRM_type[prmaltg09]       = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 09                         */
    PRM_code[prmaltp09]       = "altp09";       PRM[prmaltp09]       = 0;   PRM_type[prmaltp09]       = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 09                            */
    PRM_code[prmaltg41]       = "altg41";       PRM[prmaltg41]       = 0;   PRM_type[prmaltg41]       = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 41                         */
    PRM_code[prmaltp41]       = "altp41";       PRM[prmaltp41]       = 0;   PRM_type[prmaltp41]       = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 41                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    PRM_code[prmtestdsivert]  = "testdsivert";  PRM[prmtestdsivert]  = 120; PRM_type[prmtestdsivert]  = TS_type; /* Testen vertraging in DSI bericht in testomgeving                               */
    PRM_code[prmtestdsilyn]   = "testdsilyn";   PRM[prmtestdsilyn]   = 0;                             /* Testen lijnnummer DSI bericht in testomgeving                                  */
    PRM_code[prmtestdsicat]   = "testdsicat";   PRM[prmtestdsicat]   = 10;                            /* Testen ritcategorie DSI bericht in testomgeving                                */
#endif


/* extra data */
/* ---------- */
    FC_type[fc02] = MVT_type;
    FC_type[fc05] = MVT_type;
    FC_type[fc08] = MVT_type;
    FC_type[fc09] = MVT_type;
    FC_type[fc41] = OV_type;

/* Selectieve detectie */
/* ------------------- */
    DS_code[dsdummy] = "dsdummy";

/* modulen */
/* ------- */
    PRML[ML1][fc02] = PRIMAIR;

    PRML[ML2][fc05] = PRIMAIR;

    PRML[ML3][fc08] = PRIMAIR;

    PRML[ML4][fc09] = PRIMAIR;

    PRML[ML5][fc41] = PRIMAIR;


/* Typen ingangen */
/* -------------- */
    IS_type[d021] = DKOP_type;
    IS_type[d022] = DLNG_type;
    IS_type[d051] = DKOP_type;
    IS_type[d052] = DLNG_type;
    IS_type[d081] = DKOP_type;
    IS_type[d082] = DLNG_type;
    IS_type[d091] = DKOP_type;
    IS_type[d092] = DLNG_type;
    IS_type[d411] = DKOP_type;
    IS_type[d412] = DLNG_type;

/* Typen uitgangen */
/* --------------- */
    US_type[fc02] = MVT_type;
    US_type[fc05] = MVT_type;
    US_type[fc08] = MVT_type;
    US_type[fc09] = MVT_type;
    US_type[fc41] = OV_type;

#ifdef VISSIM
#endif

    #include "GMtab.add"
}
