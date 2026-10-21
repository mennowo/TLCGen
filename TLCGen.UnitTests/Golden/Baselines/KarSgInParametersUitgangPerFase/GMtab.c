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
   FC_code[fc241] = "241"; TRG_max[fc241] = 0; TRG_min[fc241] = 0; TGG_max[fc241] = 0; TGG_min[fc241] = 0; TFG_max[fc241] = 0; TGL_max[fc241] = 0; TGL_min[fc241] = 0; TMGL_max[fc241] = 0; TVG_max[fc241] = 300;

/* conflicten */
/* ---------- */

    TIG_max[fc02][fc05] = 30;
    TIG_max[fc02][fc41] = 30;
    TIG_max[fc02][fc241] = 30;

    TIG_max[fc05][fc02] = 30;
    TIG_max[fc05][fc41] = 30;
    TIG_max[fc05][fc241] = 30;

    TIG_max[fc41][fc02] = 30;
    TIG_max[fc41][fc05] = 30;
    TIG_max[fc41][fc241] = 30;

    TIG_max[fc241][fc02] = 30;
    TIG_max[fc241][fc05] = 30;
    TIG_max[fc241][fc41] = 30;

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
    US_code[usovinm41]    = "ovinm41";                           /* Verklikken inmelding OV fase 41            */
    US_code[usovinm241]   = "ovinm241";                          /* Verklikken inmelding OV fase 241           */
    US_code[ushdinm02]    = "hdinm02";                           /* Verklikken inmelding HD fase 02            */
    US_code[usperdef]     = "perdef";                            /* Default periode actief                     */
    US_code[usper1]       = "per1";                              /* Periode actief                             */

/* detectie */
/* -------- */
    D_code[d021]  = "021";  
    D_code[d022]  = "022";  
    D_code[d051]  = "051";  
    D_code[d052]  = "052";  
    D_code[d411]  = "411";  
    D_code[d412]  = "412";  
    D_code[d2411] = "2411"; 
    D_code[d2412] = "2412"; 
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined PRACTICE_TEST
    D_code[ddummykarin241bus]  = "dummykarin241bus";  
    D_code[ddummykarin41bus]   = "dummykarin41bus";   
    D_code[ddummykarin05hd]    = "dummykarin05hd";    
    D_code[ddummykaruit241bus] = "dummykaruit241bus"; 
    D_code[ddummykaruit41bus]  = "dummykaruit41bus";  
    D_code[ddummykaruit05hd]   = "dummykaruit05hd";   
    D_code[ddummyhdkarin02]    = "dummyhdkarin02";    
    D_code[ddummyhdkaruit02]   = "dummyhdkaruit02";   
#endif

/* overige ingangen */
/* ---------------- */

/* hulp elementen */
/* -------------- */
    H_code[hprio241bus]       = "prio241bus";                                   /* Bijhouden actief zijn prioriteit fase 241        */
    H_code[hprioin241bus]     = "prioin241bus";                                 /* Prioriteit inmelding fase 241 Bus                */
    H_code[hpriouit241bus]    = "priouit241bus";                                /* Prioriteit uitmelding 241 Bus                    */
    H_code[hprioin241buskar]  = "prioin241buskar";                              /* Prioriteit inmelding fase 241 Bus                */
    H_code[hpriouit241buskar] = "priouit241buskar";                             /* Prioriteit uitmelding 241 Bus                    */
    H_code[hprio41bus]        = "prio41bus";                                    /* Bijhouden actief zijn prioriteit fase 41         */
    H_code[hprioin41bus]      = "prioin41bus";                                  /* Prioriteit inmelding fase 41 Bus                 */
    H_code[hpriouit41bus]     = "priouit41bus";                                 /* Prioriteit uitmelding 41 Bus                     */
    H_code[hprioin41buskar]   = "prioin41buskar";                               /* Prioriteit inmelding fase 41 Bus                 */
    H_code[hpriouit41buskar]  = "priouit41buskar";                              /* Prioriteit uitmelding 41 Bus                     */
    H_code[hprio05hd]         = "prio05hd";                                     /* Bijhouden actief zijn prioriteit fase 05         */
    H_code[hprioin05hd]       = "prioin05hd";                                   /* Prioriteit inmelding fase 05 Nood- en hulpdienst */
    H_code[hpriouit05hd]      = "priouit05hd";                                  /* Prioriteit uitmelding 05 Nood- en hulpdienst     */
    H_code[hprioin05hdkar]    = "prioin05hdkar";                                /* Prioriteit inmelding fase 05 Nood- en hulpdienst */
    H_code[hpriouit05hdkar]   = "priouit05hdkar";                               /* Prioriteit uitmelding 05 Nood- en hulpdienst     */
    H_code[hhd02]             = "hd02";                                         /* Bijhouden aanwezigheid HD fase 02                */
    H_code[hhdin02]           = "hdin02";                                       /* HD inmelding 02                                  */
    H_code[hhduit02]          = "hduit02";                                      /* HD uitmelding 02                                 */
    H_code[hhdin02kar]        = "hdin02kar";                                    /* HD inmelding 02                                  */
    H_code[hhduit02kar]       = "hduit02kar";                                   /* HD uitmelding 02                                 */

/* geheugen elementen */
/* ------------------ */
    MM_code[mperiod] = "period";                   /* Onthouden actieve periode    */
    MM_code[mar02]   = "ar02";                     /* Alternatieve ruimte fase 02  */
    MM_code[mar05]   = "ar05";                     /* Alternatieve ruimte fase 05  */
    MM_code[mar41]   = "ar41";                     /* Alternatieve ruimte fase 41  */
    MM_code[mar241]  = "ar241";                    /* Alternatieve ruimte fase 241 */

/* tijd elementen */
/* -------------- */
    T_code[tkm02]        = "km02";        T_max[tkm02]        = 0;    T_type[tkm02]        = TE_type; /* Kop maximum voor detector 02                           */
    T_code[tkm05]        = "km05";        T_max[tkm05]        = 0;    T_type[tkm05]        = TE_type; /* Kop maximum voor detector 05                           */
    T_code[tkm41]        = "km41";        T_max[tkm41]        = 0;    T_type[tkm41]        = TE_type; /* Kop maximum voor detector 41                           */
    T_code[tkm241]       = "km241";       T_max[tkm241]       = 0;    T_type[tkm241]       = TE_type; /* Kop maximum voor detector 241                          */
    T_code[tkarmelding]  = "karmelding";  T_max[tkarmelding]  = 15;   T_type[tkarmelding]  = TE_type; /* Duur verklikking ontvangst melding KAR                 */
    T_code[tkarog]       = "karog";       T_max[tkarog]       = 1440; T_type[tkarog]       = TM_type; /* Ondergedrag KAR                                        */
    T_code[tbtovg241bus] = "btovg241bus"; T_max[tbtovg241bus] = 30;   T_type[tbtovg241bus] = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 241 */
    T_code[trt241bus]    = "rt241bus";    T_max[trt241bus]    = 0;    T_type[trt241bus]    = TE_type; /* Actuele rijtijd prio fase 241                          */
    T_code[tgb241bus]    = "gb241bus";    T_max[tgb241bus]    = 600;  T_type[tgb241bus]    = TE_type; /* Groenbewaking prioriteit fase 241                      */
    T_code[tblk241bus]   = "blk241bus";   T_max[tblk241bus]   = 200;  T_type[tblk241bus]   = TE_type; /* Blokkeertijd na prioriteitsingreep fase 241            */
    T_code[tbtovg41bus]  = "btovg41bus";  T_max[tbtovg41bus]  = 30;   T_type[tbtovg41bus]  = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 41  */
    T_code[trt41bus]     = "rt41bus";     T_max[trt41bus]     = 0;    T_type[trt41bus]     = TE_type; /* Actuele rijtijd prio fase 41                           */
    T_code[tgb41bus]     = "gb41bus";     T_max[tgb41bus]     = 600;  T_type[tgb41bus]     = TE_type; /* Groenbewaking prioriteit fase 41                       */
    T_code[tblk41bus]    = "blk41bus";    T_max[tblk41bus]    = 200;  T_type[tblk41bus]    = TE_type; /* Blokkeertijd na prioriteitsingreep fase 41             */
    T_code[tbtovg05hd]   = "btovg05hd";   T_max[tbtovg05hd]   = 30;   T_type[tbtovg05hd]   = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 05  */
    T_code[trt05hd]      = "rt05hd";      T_max[trt05hd]      = 0;    T_type[trt05hd]      = TE_type; /* Actuele rijtijd prio fase 05                           */
    T_code[tgb05hd]      = "gb05hd";      T_max[tgb05hd]      = 600;  T_type[tgb05hd]      = TE_type; /* Groenbewaking prioriteit fase 05                       */
    T_code[tblk05hd]     = "blk05hd";     T_max[tblk05hd]     = 200;  T_type[tblk05hd]     = TE_type; /* Blokkeertijd na prioriteitsingreep fase 05             */
    T_code[tgbhd02]      = "gbhd02";      T_max[tgbhd02]      = 600;  T_type[tgbhd02]      = TE_type; /* Groenbewaking HD fase 02                               */
    T_code[trthd02]      = "rthd02";      T_max[trthd02]      = 0;    T_type[trthd02]      = TE_type; /* Actuele rijtijd HD fase 02                             */
    T_code[tbtovg02hd]   = "btovg02hd";   T_max[tbtovg02hd]   = 0;    T_type[tbtovg02hd]   = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 02  */
    T_code[thdin02kar]   = "hdin02kar";   T_max[thdin02kar]   = 15;   T_type[thdin02kar]   = TE_type; /* Anti jutter tijd inmelden HD 02 KAR                    */
    T_code[thduit02kar]  = "hduit02kar";  T_max[thduit02kar]  = 15;   T_type[thduit02kar]  = TE_type; /* Anti jutter tijd uitmelden HD 02                       */

/* teller elementen */
/* ---------------- */
    C_code[cvc241bus] = "vc241bus"; C_max[cvc241bus] = 999; C_type[cvc241bus] = CT_type; /* Bijhouden prio inmeldingen fase 241 type Bus                */
    C_code[cvc41bus]  = "vc41bus";  C_max[cvc41bus]  = 999; C_type[cvc41bus]  = CT_type; /* Bijhouden prio inmeldingen fase 41 type Bus                 */
    C_code[cvc05hd]   = "vc05hd";   C_max[cvc05hd]   = 999; C_type[cvc05hd]   = CT_type; /* Bijhouden prio inmeldingen fase 05 type Nood- en hulpdienst */
    C_code[cvchd02]   = "vchd02";   C_max[cvchd02]   = 999; C_type[cvchd02]   = CT_type; /* Bijhouden prio inmeldingen fase 02                          */

/* schakelaars */
/* ----------- */
    SCH_code[schcovuber]          = "covuber";          SCH[schcovuber]          = 1;                                   /* Weergeven wijzigingen PRIO_teller via CIF_UBER         */
    SCH_code[schcheckdstype]      = "checkdstype";      SCH[schcheckdstype]      = 1;                                   /* Check type DSI bericht bij VECOM                       */
    SCH_code[schprioin241buskar]  = "prioin241buskar";  SCH[schprioin241buskar]  = 1;                                   /* Inmelden 241 via Bus toestaan                          */
    SCH_code[schpriouit241buskar] = "priouit241buskar"; SCH[schpriouit241buskar] = 1;                                   /* Uitmelden 241 via Bus toestaan                         */
    SCH_code[schprioin41buskar]   = "prioin41buskar";   SCH[schprioin41buskar]   = 1;                                   /* Inmelden 41 via Bus toestaan                           */
    SCH_code[schpriouit41buskar]  = "priouit41buskar";  SCH[schpriouit41buskar]  = 1;                                   /* Uitmelden 41 via Bus toestaan                          */
    SCH_code[schprioin05hdkar]    = "prioin05hdkar";    SCH[schprioin05hdkar]    = 1;                                   /* Inmelden 05 via Nood- en hulpdienst toestaan           */
    SCH_code[schpriouit05hdkar]   = "priouit05hdkar";   SCH[schpriouit05hdkar]   = 1;                                   /* Uitmelden 05 via Nood- en hulpdienst toestaan          */
    SCH_code[schhdin02kar]        = "hdin02kar";        SCH[schhdin02kar]        = 1;                                   /* Inmelden 02 via KAR HD toestaan                        */
    SCH_code[schhduit02kar]       = "hduit02kar";       SCH[schhduit02kar]       = 1;                                   /* Uitmelden 02 via KAR HD toestaan                       */
    SCH_code[schchecksirene02]    = "checksirene02";    SCH[schchecksirene02]    = 0;                                   /* Bij HD meldingen bij 02 via DSI controleren op CIF_SIR */
    SCH_code[schaltg02]           = "altg02";           SCH[schaltg02]           = 0;                                   /* Alternatieve realisatie toestaan fase 02               */
    SCH_code[schaltg05]           = "altg05";           SCH[schaltg05]           = 0;                                   /* Alternatieve realisatie toestaan fase 05               */
    SCH_code[schaltg41]           = "altg41";           SCH[schaltg41]           = 0;                                   /* Alternatieve realisatie toestaan fase 41               */
    SCH_code[schaltg241]          = "altg241";          SCH[schaltg241]          = 0;                                   /* Alternatieve realisatie toestaan fase 241              */

/* parameters */
/* ---------- */
    PRM_code[prmfb]            = "fb";            PRM[prmfb]            = 0;    PRM_type[prmfb]            = TS_type; /* Instelling fasebewaking                                        */
    PRM_code[prmxx]            = "xx";            PRM[prmxx]            = 0;                                          /* Versiebeheer xx                                                */
    PRM_code[prmyy]            = "yy";            PRM[prmyy]            = 0;                                          /* Versiebeheer yy                                                */
    PRM_code[prmzz]            = "zz";            PRM[prmzz]            = 0;                                          /* Versiebeheer zz                                                */
    PRM_code[prmmk021]         = "mk021";         PRM[prmmk021]         = 0;                                          /* Meetkriterium type voor detector 021                           */
    PRM_code[prmmk022]         = "mk022";         PRM[prmmk022]         = 0;                                          /* Meetkriterium type voor detector 022                           */
    PRM_code[prmmk051]         = "mk051";         PRM[prmmk051]         = 0;                                          /* Meetkriterium type voor detector 051                           */
    PRM_code[prmmk052]         = "mk052";         PRM[prmmk052]         = 0;                                          /* Meetkriterium type voor detector 052                           */
    PRM_code[prmmk411]         = "mk411";         PRM[prmmk411]         = 0;                                          /* Meetkriterium type voor detector 411                           */
    PRM_code[prmmk412]         = "mk412";         PRM[prmmk412]         = 0;                                          /* Meetkriterium type voor detector 412                           */
    PRM_code[prmmk2411]        = "mk2411";        PRM[prmmk2411]        = 0;                                          /* Meetkriterium type voor detector 2411                          */
    PRM_code[prmmk2412]        = "mk2412";        PRM[prmmk2412]        = 0;                                          /* Meetkriterium type voor detector 2412                          */
    PRM_code[prmmwta]          = "mwta";          PRM[prmmwta]          = 0;    PRM_type[prmmwta]          = TS_type; /* Maximale wachttijd autoverkeer                                 */
    PRM_code[prmmwtfts]        = "mwtfts";        PRM[prmmwtfts]        = 0;    PRM_type[prmmwtfts]        = TS_type; /* Maximale wachttijd fiets                                       */
    PRM_code[prmmwtvtg]        = "mwtvtg";        PRM[prmmwtvtg]        = 0;    PRM_type[prmmwtvtg]        = TS_type; /* Maximale wachttijd voetgangers                                 */
    PRM_code[prmrto241bus]     = "rto241bus";     PRM[prmrto241bus]     = 150;  PRM_type[prmrto241bus]     = TE_type; /* Ongehinderde rijtijd prioriteit fase 241                       */
    PRM_code[prmrtbg241bus]    = "rtbg241bus";    PRM[prmrtbg241bus]    = 200;  PRM_type[prmrtbg241bus]    = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 241                 */
    PRM_code[prmrtg241bus]     = "rtg241bus";     PRM[prmrtg241bus]     = 300;  PRM_type[prmrtg241bus]     = TE_type; /* Gehinderde rijtijd prioriteit fase 241                         */
    PRM_code[prmomx241bus]     = "omx241bus";     PRM[prmomx241bus]     = 50;   PRM_type[prmomx241bus]     = TE_type; /* Ondermaximum OV fase 241                                       */
    PRM_code[prmupinagb241bus] = "upinagb241bus"; PRM[prmupinagb241bus] = 0;                                          /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 241 */
    PRM_code[prmvtgcat241bus]  = "vtgcat241bus";  PRM[prmvtgcat241bus]  = 1;                                          /* Voertuigcategorie DSI voor prio ingreep 241bus                 */
    PRM_code[prmprio241bus]    = "prio241bus";    PRM[prmprio241bus]    = 0;                                          /* Prioriteitsinstelling OV fase 241                              */
    PRM_code[prmrto41bus]      = "rto41bus";      PRM[prmrto41bus]      = 150;  PRM_type[prmrto41bus]      = TE_type; /* Ongehinderde rijtijd prioriteit fase 41                        */
    PRM_code[prmrtbg41bus]     = "rtbg41bus";     PRM[prmrtbg41bus]     = 200;  PRM_type[prmrtbg41bus]     = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 41                  */
    PRM_code[prmrtg41bus]      = "rtg41bus";      PRM[prmrtg41bus]      = 300;  PRM_type[prmrtg41bus]      = TE_type; /* Gehinderde rijtijd prioriteit fase 41                          */
    PRM_code[prmomx41bus]      = "omx41bus";      PRM[prmomx41bus]      = 50;   PRM_type[prmomx41bus]      = TE_type; /* Ondermaximum OV fase 41                                        */
    PRM_code[prmupinagb41bus]  = "upinagb41bus";  PRM[prmupinagb41bus]  = 0;                                          /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41  */
    PRM_code[prmvtgcat41bus]   = "vtgcat41bus";   PRM[prmvtgcat41bus]   = 1;                                          /* Voertuigcategorie DSI voor prio ingreep 41bus                  */
    PRM_code[prmprio41bus]     = "prio41bus";     PRM[prmprio41bus]     = 0;                                          /* Prioriteitsinstelling OV fase 41                               */
    PRM_code[prmrto05hd]       = "rto05hd";       PRM[prmrto05hd]       = 150;  PRM_type[prmrto05hd]       = TE_type; /* Ongehinderde rijtijd prioriteit fase 05                        */
    PRM_code[prmrtbg05hd]      = "rtbg05hd";      PRM[prmrtbg05hd]      = 200;  PRM_type[prmrtbg05hd]      = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 05                  */
    PRM_code[prmrtg05hd]       = "rtg05hd";       PRM[prmrtg05hd]       = 300;  PRM_type[prmrtg05hd]       = TE_type; /* Gehinderde rijtijd prioriteit fase 05                          */
    PRM_code[prmomx05hd]       = "omx05hd";       PRM[prmomx05hd]       = 50;   PRM_type[prmomx05hd]       = TE_type; /* Ondermaximum OV fase 05                                        */
    PRM_code[prmupinagb05hd]   = "upinagb05hd";   PRM[prmupinagb05hd]   = 0;                                          /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 05  */
    PRM_code[prmvtgcat05hd]    = "vtgcat05hd";    PRM[prmvtgcat05hd]    = 0;                                          /* Voertuigcategorie DSI voor prio ingreep 05hd                   */
    PRM_code[prmprio05hd]      = "prio05hd";      PRM[prmprio05hd]      = 9005;                                       /* Prioriteitsinstelling OV fase 05                               */
    PRM_code[prmpriohd02]      = "priohd02";      PRM[prmpriohd02]      = 9005;                                       /* Prioriteitsinstelling HD fase 02                               */
    PRM_code[prmrtohd02]       = "rtohd02";       PRM[prmrtohd02]       = 150;  PRM_type[prmrtohd02]       = TE_type; /* Ongehinderde rijtijd HD fase 02                                */
    PRM_code[prmrtbghd02]      = "rtbghd02";      PRM[prmrtbghd02]      = 200;  PRM_type[prmrtbghd02]      = TE_type; /* Beperkt gehinderde rijtijd HD fase 02                          */
    PRM_code[prmrtghd02]       = "rtghd02";       PRM[prmrtghd02]       = 300;  PRM_type[prmrtghd02]       = TE_type; /* Gehinderde rijtijd HD fase 02                                  */
    PRM_code[prmupinagbhd02]   = "upinagbhd02";   PRM[prmupinagbhd02]   = 0;                                          /* Selectieve detectie onbetrouwbaar na groenbewaking HD fase 02  */
    PRM_code[prmkarsg05]       = "karsg05";       PRM[prmkarsg05]       = 5;                                          /* Signaalgroep nummer voor fase 05 bij inmelding via DSI         */
    PRM_code[prmkarsg41]       = "karsg41";       PRM[prmkarsg41]       = 41;                                         /* Signaalgroep nummer voor fase 41 bij inmelding via DSI         */
    PRM_code[prmkarsg241]      = "karsg241";      PRM[prmkarsg241]      = 41;                                         /* Signaalgroep nummer voor fase 241 bij inmelding via DSI        */
    PRM_code[prmkarsg02]       = "karsg02";       PRM[prmkarsg02]       = 2;                                          /* Signaalgroep nummer voor fase 02 bij inmelding via DSI         */
    PRM_code[prmkarsghd02]     = "karsghd02";     PRM[prmkarsghd02]     = 2;                                          /* Signaalgroep nummer voor fase 02 bij inmelding HD via DSI      */
    PRM_code[prmkarsghd05]     = "karsghd05";     PRM[prmkarsghd05]     = 5;                                          /* Signaalgroep nummer voor fase 05 bij inmelding HD via DSI      */
    PRM_code[prmstkp1]         = "stkp1";         PRM[prmstkp1]         = 600;  PRM_type[prmstkp1]         = TI_type; /* Start klokperiode ochtend                                      */
    PRM_code[prmetkp1]         = "etkp1";         PRM[prmetkp1]         = 900;  PRM_type[prmetkp1]         = TI_type; /* Einde klokperiode ochtend                                      */
    PRM_code[prmdckp1]         = "dckp1";         PRM[prmdckp1]         = 8;                                          /* Dagsoort klokperiode ochtend                                   */
    PRM_code[prmmg1_02]        = "mg1_02";        PRM[prmmg1_02]        = 300;  PRM_type[prmmg1_02]        = TE_type; /* Maximale groentijd MG1 02                                      */
    PRM_code[prmmg1_05]        = "mg1_05";        PRM[prmmg1_05]        = 300;  PRM_type[prmmg1_05]        = TE_type; /* Maximale groentijd MG1 05                                      */
    PRM_code[prmmg1_41]        = "mg1_41";        PRM[prmmg1_41]        = 300;  PRM_type[prmmg1_41]        = TE_type; /* Maximale groentijd MG1 41                                      */
    PRM_code[prmmg1_241]       = "mg1_241";       PRM[prmmg1_241]       = 300;  PRM_type[prmmg1_241]       = TE_type; /* Maximale groentijd MG1 241                                     */
    PRM_code[prmmg2_02]        = "mg2_02";        PRM[prmmg2_02]        = 200;  PRM_type[prmmg2_02]        = TE_type; /* Maximale groentijd MG2 02                                      */
    PRM_code[prmmg2_05]        = "mg2_05";        PRM[prmmg2_05]        = 200;  PRM_type[prmmg2_05]        = TE_type; /* Maximale groentijd MG2 05                                      */
    PRM_code[prmmg2_41]        = "mg2_41";        PRM[prmmg2_41]        = 200;  PRM_type[prmmg2_41]        = TE_type; /* Maximale groentijd MG2 41                                      */
    PRM_code[prmmg2_241]       = "mg2_241";       PRM[prmmg2_241]       = 200;  PRM_type[prmmg2_241]       = TE_type; /* Maximale groentijd MG2 241                                     */
    PRM_code[prmmlfpr02]       = "mlfpr02";       PRM[prmmlfpr02]       = 0;                                          /* Maximaal aantal modules vooruit fase 02                        */
    PRM_code[prmmlfpr05]       = "mlfpr05";       PRM[prmmlfpr05]       = 0;                                          /* Maximaal aantal modules vooruit fase 05                        */
    PRM_code[prmmlfpr41]       = "mlfpr41";       PRM[prmmlfpr41]       = 0;                                          /* Maximaal aantal modules vooruit fase 41                        */
    PRM_code[prmmlfpr241]      = "mlfpr241";      PRM[prmmlfpr241]      = 0;                                          /* Maximaal aantal modules vooruit fase 241                       */
    PRM_code[prmaltg02]        = "altg02";        PRM[prmaltg02]        = 0;    PRM_type[prmaltg02]        = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 02         */
    PRM_code[prmaltp02]        = "altp02";        PRM[prmaltp02]        = 0;    PRM_type[prmaltp02]        = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 02            */
    PRM_code[prmaltg05]        = "altg05";        PRM[prmaltg05]        = 0;    PRM_type[prmaltg05]        = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 05         */
    PRM_code[prmaltp05]        = "altp05";        PRM[prmaltp05]        = 0;    PRM_type[prmaltp05]        = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 05            */
    PRM_code[prmaltg41]        = "altg41";        PRM[prmaltg41]        = 0;    PRM_type[prmaltg41]        = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 41         */
    PRM_code[prmaltp41]        = "altp41";        PRM[prmaltp41]        = 0;    PRM_type[prmaltp41]        = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 41            */
    PRM_code[prmaltg241]       = "altg241";       PRM[prmaltg241]       = 0;    PRM_type[prmaltg241]       = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 241        */
    PRM_code[prmaltp241]       = "altp241";       PRM[prmaltp241]       = 0;    PRM_type[prmaltp241]       = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 241           */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    PRM_code[prmtestdsivert]   = "testdsivert";   PRM[prmtestdsivert]   = 120;  PRM_type[prmtestdsivert]   = TS_type; /* Testen vertraging in DSI bericht in testomgeving               */
    PRM_code[prmtestdsilyn]    = "testdsilyn";    PRM[prmtestdsilyn]    = 0;                               /* Testen lijnnummer DSI bericht in testomgeving                  */
    PRM_code[prmtestdsicat]    = "testdsicat";    PRM[prmtestdsicat]    = 10;                              /* Testen ritcategorie DSI bericht in testomgeving                */
#endif


/* extra data */
/* ---------- */
    FC_type[fc02] = MVT_type;
    FC_type[fc05] = MVT_type;
    FC_type[fc41] = OV_type;
    FC_type[fc241] = OV_type;

/* Selectieve detectie */
/* ------------------- */
    DS_code[dsdummy] = "dsdummy";

/* modulen */
/* ------- */
    PRML[ML1][fc02] = PRIMAIR;

    PRML[ML2][fc05] = PRIMAIR;

    PRML[ML3][fc41] = PRIMAIR;

    PRML[ML4][fc241] = PRIMAIR;


/* Typen ingangen */
/* -------------- */
    IS_type[d021] = DKOP_type;
    IS_type[d022] = DLNG_type;
    IS_type[d051] = DKOP_type;
    IS_type[d052] = DLNG_type;
    IS_type[d411] = DKOP_type;
    IS_type[d412] = DLNG_type;
    IS_type[d2411] = DKOP_type;
    IS_type[d2412] = DLNG_type;

/* Typen uitgangen */
/* --------------- */
    US_type[fc02] = MVT_type;
    US_type[fc05] = MVT_type;
    US_type[fc41] = OV_type;
    US_type[fc241] = OV_type;

#ifdef VISSIM
#endif

    #include "GMtab.add"
}
