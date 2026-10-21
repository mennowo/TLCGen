/* REGEL INSTELLINGEN */
/* ------------------ */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMtab.c
      CCOL:   9.0
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
   FC_code[fc22] = "22"; TRG_max[fc22] = 0; TRG_min[fc22] = 0; TGG_max[fc22] = 0; TGG_min[fc22] = 0; TFG_max[fc22] = 0; TGL_max[fc22] = 0; TGL_min[fc22] = 0; TMGL_max[fc22] = 0; TVG_max[fc22] = 300;
   FC_code[fc41] = "41"; TRG_max[fc41] = 0; TRG_min[fc41] = 0; TGG_max[fc41] = 0; TGG_min[fc41] = 0; TFG_max[fc41] = 0; TGL_max[fc41] = 0; TGL_min[fc41] = 0; TMGL_max[fc41] = 0; TVG_max[fc41] = 300;
   FC_code[fc42] = "42"; TRG_max[fc42] = 0; TRG_min[fc42] = 0; TGG_max[fc42] = 0; TGG_min[fc42] = 0; TFG_max[fc42] = 0; TGL_max[fc42] = 0; TGL_min[fc42] = 0; TMGL_max[fc42] = 0; TVG_max[fc42] = 300;
   FC_code[fc61] = "61"; TRG_max[fc61] = 0; TRG_min[fc61] = 0; TGG_max[fc61] = 0; TGG_min[fc61] = 0; TFG_max[fc61] = 0; TGL_max[fc61] = 0; TGL_min[fc61] = 0; TMGL_max[fc61] = 0; TVG_max[fc61] = 300;

/* conflicten */
/* ---------- */

    TO_max[fc02][fc05] = 30;
    TO_max[fc02][fc22] = 30;
    TO_max[fc02][fc41] = 30;
    TO_max[fc02][fc42] = 30;
    TO_max[fc02][fc61] = 30;

    TO_max[fc05][fc02] = 30;
    TO_max[fc05][fc22] = 30;
    TO_max[fc05][fc41] = 30;
    TO_max[fc05][fc42] = 30;
    TO_max[fc05][fc61] = 30;

    TO_max[fc22][fc02] = 30;
    TO_max[fc22][fc05] = 30;
    TO_max[fc22][fc41] = 30;
    TO_max[fc22][fc42] = 30;
    TO_max[fc22][fc61] = 30;

    TO_max[fc41][fc02] = 30;
    TO_max[fc41][fc05] = 30;
    TO_max[fc41][fc22] = 30;
    TO_max[fc41][fc42] = 30;
    TO_max[fc41][fc61] = 30;

    TO_max[fc42][fc02] = 30;
    TO_max[fc42][fc05] = 30;
    TO_max[fc42][fc22] = 30;
    TO_max[fc42][fc41] = 30;
    TO_max[fc42][fc61] = 30;

    TO_max[fc61][fc02] = 30;
    TO_max[fc61][fc05] = 30;
    TO_max[fc61][fc22] = 30;
    TO_max[fc61][fc41] = 30;
    TO_max[fc61][fc42] = 30;

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
    US_code[usovinm41tram]  = "ovinm41tram";                           /* Verklikken inmelding OV fase 41            */
    US_code[usovinm41bus]   = "ovinm41bus";                            /* Verklikken inmelding OV fase 41            */
    US_code[usovinm42bus]   = "ovinm42bus";                            /* Verklikken inmelding OV fase 42            */
    US_code[usovinm22fiets] = "ovinm22fiets";                          /* Verklikken inmelding OV fase 22            */
    US_code[usovinm02vrw]   = "ovinm02vrw";                            /* Verklikken inmelding OV fase 02            */
    US_code[usovinm05auto]  = "ovinm05auto";                           /* Verklikken inmelding OV fase 05            */
    US_code[usovinm05ng]    = "ovinm05ng";                             /* Verklikken inmelding OV fase 05            */
    US_code[usovinm61hd]    = "ovinm61hd";                             /* Verklikken inmelding OV fase 61            */
    US_code[usperdef]       = "perdef";                                /* Default periode actief                     */
    US_code[usper1]         = "per1";                                  /* Periode actief                             */

/* detectie */
/* -------- */
    D_code[d021] = "021"; 
    D_code[d022] = "022"; 
    D_code[d051] = "051"; 
    D_code[d052] = "052"; 
    D_code[d221] = "221"; 
    D_code[d222] = "222"; 
    D_code[dk22] = "k22"; 
    D_code[d411] = "411"; 
    D_code[d412] = "412"; 
    D_code[d421] = "421"; 
    D_code[d422] = "422"; 
    D_code[d611] = "611"; 
    D_code[d612] = "612"; 
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined PRACTICE_TEST
    D_code[ddummykarin41tram]   = "dummykarin41tram";   
    D_code[ddummykarin41bus]    = "dummykarin41bus";    
    D_code[ddummykarin42bus]    = "dummykarin42bus";    
    D_code[ddummykarin22fiets]  = "dummykarin22fiets";  
    D_code[ddummykarin02vrw]    = "dummykarin02vrw";    
    D_code[ddummykarin05auto]   = "dummykarin05auto";   
    D_code[ddummykarin05ng]     = "dummykarin05ng";     
    D_code[ddummykarin61hd]     = "dummykarin61hd";     
    D_code[ddummykaruit41tram]  = "dummykaruit41tram";  
    D_code[ddummykaruit41bus]   = "dummykaruit41bus";   
    D_code[ddummykaruit42bus]   = "dummykaruit42bus";   
    D_code[ddummykaruit22fiets] = "dummykaruit22fiets"; 
    D_code[ddummykaruit02vrw]   = "dummykaruit02vrw";   
    D_code[ddummykaruit05auto]  = "dummykaruit05auto";  
    D_code[ddummykaruit05ng]    = "dummykaruit05ng";    
    D_code[ddummykaruit61hd]    = "dummykaruit61hd";    
#endif

/* overige ingangen */
/* ---------------- */

/* hulp elementen */
/* -------------- */
    H_code[hprio41tram]        = "prio41tram";                                     /* Bijhouden actief zijn prioriteit fase 41         */
    H_code[hprioin41tram]      = "prioin41tram";                                   /* Prioriteit inmelding fase 41 Tram                */
    H_code[hpriouit41tram]     = "priouit41tram";                                  /* Prioriteit uitmelding 41 Tram                    */
    H_code[hprioin41tramkar]   = "prioin41tramkar";                                /* Prioriteit inmelding fase 41 Tram                */
    H_code[hpriouit41tramkar]  = "priouit41tramkar";                               /* Prioriteit uitmelding 41 Tram                    */
    H_code[hprio41bus]         = "prio41bus";                                      /* Bijhouden actief zijn prioriteit fase 41         */
    H_code[hprioin41bus]       = "prioin41bus";                                    /* Prioriteit inmelding fase 41 Bus                 */
    H_code[hpriouit41bus]      = "priouit41bus";                                   /* Prioriteit uitmelding 41 Bus                     */
    H_code[hprioin41buskar]    = "prioin41buskar";                                 /* Prioriteit inmelding fase 41 Bus                 */
    H_code[hpriouit41buskar]   = "priouit41buskar";                                /* Prioriteit uitmelding 41 Bus                     */
    H_code[hprio42bus]         = "prio42bus";                                      /* Bijhouden actief zijn prioriteit fase 42         */
    H_code[hprioin42bus]       = "prioin42bus";                                    /* Prioriteit inmelding fase 42 Bus                 */
    H_code[hpriouit42bus]      = "priouit42bus";                                   /* Prioriteit uitmelding 42 Bus                     */
    H_code[hprioin42buskar]    = "prioin42buskar";                                 /* Prioriteit inmelding fase 42 Bus                 */
    H_code[hpriouit42buskar]   = "priouit42buskar";                                /* Prioriteit uitmelding 42 Bus                     */
    H_code[hprio22fiets]       = "prio22fiets";                                    /* Bijhouden actief zijn prioriteit fase 22         */
    H_code[hprioin22fiets]     = "prioin22fiets";                                  /* Prioriteit inmelding fase 22 Fiets               */
    H_code[hpriouit22fiets]    = "priouit22fiets";                                 /* Prioriteit uitmelding 22 Fiets                   */
    H_code[hprioin22fietskar]  = "prioin22fietskar";                               /* Prioriteit inmelding fase 22 Fiets               */
    H_code[hpriouit22fietskar] = "priouit22fietskar";                              /* Prioriteit uitmelding 22 Fiets                   */
    H_code[hprio02vrw]         = "prio02vrw";                                      /* Bijhouden actief zijn prioriteit fase 02         */
    H_code[hprioin02vrw]       = "prioin02vrw";                                    /* Prioriteit inmelding fase 02 Vrachtwagen         */
    H_code[hpriouit02vrw]      = "priouit02vrw";                                   /* Prioriteit uitmelding 02 Vrachtwagen             */
    H_code[hprioin02vrwkar]    = "prioin02vrwkar";                                 /* Prioriteit inmelding fase 02 Vrachtwagen         */
    H_code[hpriouit02vrwkar]   = "priouit02vrwkar";                                /* Prioriteit uitmelding 02 Vrachtwagen             */
    H_code[hprio05auto]        = "prio05auto";                                     /* Bijhouden actief zijn prioriteit fase 05         */
    H_code[hprioin05auto]      = "prioin05auto";                                   /* Prioriteit inmelding fase 05 Auto                */
    H_code[hpriouit05auto]     = "priouit05auto";                                  /* Prioriteit uitmelding 05 Auto                    */
    H_code[hprioin05autokar]   = "prioin05autokar";                                /* Prioriteit inmelding fase 05 Auto                */
    H_code[hpriouit05autokar]  = "priouit05autokar";                               /* Prioriteit uitmelding 05 Auto                    */
    H_code[hprio05ng]          = "prio05ng";                                       /* Bijhouden actief zijn prioriteit fase 05         */
    H_code[hprioin05ng]        = "prioin05ng";                                     /* Prioriteit inmelding fase 05 NG                  */
    H_code[hpriouit05ng]       = "priouit05ng";                                    /* Prioriteit uitmelding 05 NG                      */
    H_code[hprioin05ngkar]     = "prioin05ngkar";                                  /* Prioriteit inmelding fase 05 NG                  */
    H_code[hpriouit05ngkar]    = "priouit05ngkar";                                 /* Prioriteit uitmelding 05 NG                      */
    H_code[hprio61hd]          = "prio61hd";                                       /* Bijhouden actief zijn prioriteit fase 61         */
    H_code[hprioin61hd]        = "prioin61hd";                                     /* Prioriteit inmelding fase 61 Nood- en hulpdienst */
    H_code[hpriouit61hd]       = "priouit61hd";                                    /* Prioriteit uitmelding 61 Nood- en hulpdienst     */
    H_code[hprioin61hdkar]     = "prioin61hdkar";                                  /* Prioriteit inmelding fase 61 Nood- en hulpdienst */
    H_code[hpriouit61hdkar]    = "priouit61hdkar";                                 /* Prioriteit uitmelding 61 Nood- en hulpdienst     */

/* geheugen elementen */
/* ------------------ */
    MM_code[mperiod] = "period";                   /* Onthouden actieve periode   */
    MM_code[mar02]   = "ar02";                     /* Alternatieve ruimte fase 02 */
    MM_code[mar05]   = "ar05";                     /* Alternatieve ruimte fase 05 */
    MM_code[mar22]   = "ar22";                     /* Alternatieve ruimte fase 22 */
    MM_code[mar41]   = "ar41";                     /* Alternatieve ruimte fase 41 */
    MM_code[mar42]   = "ar42";                     /* Alternatieve ruimte fase 42 */
    MM_code[mar61]   = "ar61";                     /* Alternatieve ruimte fase 61 */

/* tijd elementen */
/* -------------- */
    T_code[tkm02]         = "km02";         T_max[tkm02]         = 0;    T_type[tkm02]         = TE_type; /* Kop maximum voor detector 02                          */
    T_code[tkm05]         = "km05";         T_max[tkm05]         = 0;    T_type[tkm05]         = TE_type; /* Kop maximum voor detector 05                          */
    T_code[tkm22]         = "km22";         T_max[tkm22]         = 0;    T_type[tkm22]         = TE_type; /* Kop maximum voor detector 22                          */
    T_code[tkm41]         = "km41";         T_max[tkm41]         = 0;    T_type[tkm41]         = TE_type; /* Kop maximum voor detector 41                          */
    T_code[tkm42]         = "km42";         T_max[tkm42]         = 0;    T_type[tkm42]         = TE_type; /* Kop maximum voor detector 42                          */
    T_code[tkm61]         = "km61";         T_max[tkm61]         = 0;    T_type[tkm61]         = TE_type; /* Kop maximum voor detector 61                          */
    T_code[tkarmelding]   = "karmelding";   T_max[tkarmelding]   = 15;   T_type[tkarmelding]   = TE_type; /* Duur verklikking ontvangst melding KAR                */
    T_code[tkarog]        = "karog";        T_max[tkarog]        = 1440; T_type[tkarog]        = TM_type; /* Ondergedrag KAR                                       */
    T_code[tbtovg41tram]  = "btovg41tram";  T_max[tbtovg41tram]  = 30;   T_type[tbtovg41tram]  = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    T_code[trt41tram]     = "rt41tram";     T_max[trt41tram]     = 0;    T_type[trt41tram]     = TE_type; /* Actuele rijtijd prio fase 41                          */
    T_code[tgb41tram]     = "gb41tram";     T_max[tgb41tram]     = 600;  T_type[tgb41tram]     = TE_type; /* Groenbewaking prioriteit fase 41                      */
    T_code[tblk41tram]    = "blk41tram";    T_max[tblk41tram]    = 200;  T_type[tblk41tram]    = TE_type; /* Blokkeertijd na prioriteitsingreep fase 41            */
    T_code[tbtovg41bus]   = "btovg41bus";   T_max[tbtovg41bus]   = 30;   T_type[tbtovg41bus]   = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    T_code[trt41bus]      = "rt41bus";      T_max[trt41bus]      = 0;    T_type[trt41bus]      = TE_type; /* Actuele rijtijd prio fase 41                          */
    T_code[tgb41bus]      = "gb41bus";      T_max[tgb41bus]      = 600;  T_type[tgb41bus]      = TE_type; /* Groenbewaking prioriteit fase 41                      */
    T_code[tblk41bus]     = "blk41bus";     T_max[tblk41bus]     = 200;  T_type[tblk41bus]     = TE_type; /* Blokkeertijd na prioriteitsingreep fase 41            */
    T_code[tbtovg42bus]   = "btovg42bus";   T_max[tbtovg42bus]   = 30;   T_type[tbtovg42bus]   = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 42 */
    T_code[trt42bus]      = "rt42bus";      T_max[trt42bus]      = 0;    T_type[trt42bus]      = TE_type; /* Actuele rijtijd prio fase 42                          */
    T_code[tgb42bus]      = "gb42bus";      T_max[tgb42bus]      = 600;  T_type[tgb42bus]      = TE_type; /* Groenbewaking prioriteit fase 42                      */
    T_code[tblk42bus]     = "blk42bus";     T_max[tblk42bus]     = 200;  T_type[tblk42bus]     = TE_type; /* Blokkeertijd na prioriteitsingreep fase 42            */
    T_code[tbtovg22fiets] = "btovg22fiets"; T_max[tbtovg22fiets] = 30;   T_type[tbtovg22fiets] = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 22 */
    T_code[trt22fiets]    = "rt22fiets";    T_max[trt22fiets]    = 0;    T_type[trt22fiets]    = TE_type; /* Actuele rijtijd prio fase 22                          */
    T_code[tgb22fiets]    = "gb22fiets";    T_max[tgb22fiets]    = 600;  T_type[tgb22fiets]    = TE_type; /* Groenbewaking prioriteit fase 22                      */
    T_code[tblk22fiets]   = "blk22fiets";   T_max[tblk22fiets]   = 200;  T_type[tblk22fiets]   = TE_type; /* Blokkeertijd na prioriteitsingreep fase 22            */
    T_code[tbtovg02vrw]   = "btovg02vrw";   T_max[tbtovg02vrw]   = 30;   T_type[tbtovg02vrw]   = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 02 */
    T_code[trt02vrw]      = "rt02vrw";      T_max[trt02vrw]      = 0;    T_type[trt02vrw]      = TE_type; /* Actuele rijtijd prio fase 02                          */
    T_code[tgb02vrw]      = "gb02vrw";      T_max[tgb02vrw]      = 600;  T_type[tgb02vrw]      = TE_type; /* Groenbewaking prioriteit fase 02                      */
    T_code[tblk02vrw]     = "blk02vrw";     T_max[tblk02vrw]     = 200;  T_type[tblk02vrw]     = TE_type; /* Blokkeertijd na prioriteitsingreep fase 02            */
    T_code[tbtovg05auto]  = "btovg05auto";  T_max[tbtovg05auto]  = 30;   T_type[tbtovg05auto]  = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 05 */
    T_code[trt05auto]     = "rt05auto";     T_max[trt05auto]     = 0;    T_type[trt05auto]     = TE_type; /* Actuele rijtijd prio fase 05                          */
    T_code[tgb05auto]     = "gb05auto";     T_max[tgb05auto]     = 600;  T_type[tgb05auto]     = TE_type; /* Groenbewaking prioriteit fase 05                      */
    T_code[tblk05auto]    = "blk05auto";    T_max[tblk05auto]    = 200;  T_type[tblk05auto]    = TE_type; /* Blokkeertijd na prioriteitsingreep fase 05            */
    T_code[tbtovg05ng]    = "btovg05ng";    T_max[tbtovg05ng]    = 30;   T_type[tbtovg05ng]    = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 05 */
    T_code[trt05ng]       = "rt05ng";       T_max[trt05ng]       = 0;    T_type[trt05ng]       = TE_type; /* Actuele rijtijd prio fase 05                          */
    T_code[tgb05ng]       = "gb05ng";       T_max[tgb05ng]       = 600;  T_type[tgb05ng]       = TE_type; /* Groenbewaking prioriteit fase 05                      */
    T_code[tblk05ng]      = "blk05ng";      T_max[tblk05ng]      = 200;  T_type[tblk05ng]      = TE_type; /* Blokkeertijd na prioriteitsingreep fase 05            */
    T_code[tbtovg61hd]    = "btovg61hd";    T_max[tbtovg61hd]    = 30;   T_type[tbtovg61hd]    = TE_type; /* Timer bezettijd prioriteit gehinderde rijtijd fase 61 */
    T_code[trt61hd]       = "rt61hd";       T_max[trt61hd]       = 0;    T_type[trt61hd]       = TE_type; /* Actuele rijtijd prio fase 61                          */
    T_code[tgb61hd]       = "gb61hd";       T_max[tgb61hd]       = 600;  T_type[tgb61hd]       = TE_type; /* Groenbewaking prioriteit fase 61                      */
    T_code[tblk61hd]      = "blk61hd";      T_max[tblk61hd]      = 200;  T_type[tblk61hd]      = TE_type; /* Blokkeertijd na prioriteitsingreep fase 61            */

/* teller elementen */
/* ---------------- */
    C_code[cvc41tram]  = "vc41tram";  C_max[cvc41tram]  = 999; C_type[cvc41tram]  = CT_type; /* Bijhouden prio inmeldingen fase 41 type Tram                */
    C_code[cvc41bus]   = "vc41bus";   C_max[cvc41bus]   = 999; C_type[cvc41bus]   = CT_type; /* Bijhouden prio inmeldingen fase 41 type Bus                 */
    C_code[cvc42bus]   = "vc42bus";   C_max[cvc42bus]   = 999; C_type[cvc42bus]   = CT_type; /* Bijhouden prio inmeldingen fase 42 type Bus                 */
    C_code[cvc22fiets] = "vc22fiets"; C_max[cvc22fiets] = 999; C_type[cvc22fiets] = CT_type; /* Bijhouden prio inmeldingen fase 22 type Fiets               */
    C_code[cvc02vrw]   = "vc02vrw";   C_max[cvc02vrw]   = 999; C_type[cvc02vrw]   = CT_type; /* Bijhouden prio inmeldingen fase 02 type Vrachtwagen         */
    C_code[cvc05auto]  = "vc05auto";  C_max[cvc05auto]  = 999; C_type[cvc05auto]  = CT_type; /* Bijhouden prio inmeldingen fase 05 type Auto                */
    C_code[cvc05ng]    = "vc05ng";    C_max[cvc05ng]    = 999; C_type[cvc05ng]    = CT_type; /* Bijhouden prio inmeldingen fase 05 type NG                  */
    C_code[cvc61hd]    = "vc61hd";    C_max[cvc61hd]    = 999; C_type[cvc61hd]    = CT_type; /* Bijhouden prio inmeldingen fase 61 type Nood- en hulpdienst */

/* schakelaars */
/* ----------- */
    SCH_code[schcheckdstype]       = "checkdstype";       SCH[schcheckdstype]       = 0;                                    /* Check type DSI bericht bij VECOM              */
    SCH_code[schprioin41tramkar]   = "prioin41tramkar";   SCH[schprioin41tramkar]   = 1;                                    /* Inmelden 41 via Tram toestaan                 */
    SCH_code[schpriouit41tramkar]  = "priouit41tramkar";  SCH[schpriouit41tramkar]  = 1;                                    /* Uitmelden 41 via Tram toestaan                */
    SCH_code[schprioin41buskar]    = "prioin41buskar";    SCH[schprioin41buskar]    = 1;                                    /* Inmelden 41 via Bus toestaan                  */
    SCH_code[schpriouit41buskar]   = "priouit41buskar";   SCH[schpriouit41buskar]   = 1;                                    /* Uitmelden 41 via Bus toestaan                 */
    SCH_code[schprioin42buskar]    = "prioin42buskar";    SCH[schprioin42buskar]    = 1;                                    /* Inmelden 42 via Bus toestaan                  */
    SCH_code[schpriouit42buskar]   = "priouit42buskar";   SCH[schpriouit42buskar]   = 1;                                    /* Uitmelden 42 via Bus toestaan                 */
    SCH_code[schprioin22fietskar]  = "prioin22fietskar";  SCH[schprioin22fietskar]  = 1;                                    /* Inmelden 22 via Fiets toestaan                */
    SCH_code[schpriouit22fietskar] = "priouit22fietskar"; SCH[schpriouit22fietskar] = 1;                                    /* Uitmelden 22 via Fiets toestaan               */
    SCH_code[schprioin02vrwkar]    = "prioin02vrwkar";    SCH[schprioin02vrwkar]    = 1;                                    /* Inmelden 02 via Vrachtwagen toestaan          */
    SCH_code[schpriouit02vrwkar]   = "priouit02vrwkar";   SCH[schpriouit02vrwkar]   = 1;                                    /* Uitmelden 02 via Vrachtwagen toestaan         */
    SCH_code[schprioin05autokar]   = "prioin05autokar";   SCH[schprioin05autokar]   = 1;                                    /* Inmelden 05 via Auto toestaan                 */
    SCH_code[schpriouit05autokar]  = "priouit05autokar";  SCH[schpriouit05autokar]  = 1;                                    /* Uitmelden 05 via Auto toestaan                */
    SCH_code[schprioin05ngkar]     = "prioin05ngkar";     SCH[schprioin05ngkar]     = 1;                                    /* Inmelden 05 via NG toestaan                   */
    SCH_code[schpriouit05ngkar]    = "priouit05ngkar";    SCH[schpriouit05ngkar]    = 1;                                    /* Uitmelden 05 via NG toestaan                  */
    SCH_code[schprioin61hdkar]     = "prioin61hdkar";     SCH[schprioin61hdkar]     = 1;                                    /* Inmelden 61 via Nood- en hulpdienst toestaan  */
    SCH_code[schpriouit61hdkar]    = "priouit61hdkar";    SCH[schpriouit61hdkar]    = 1;                                    /* Uitmelden 61 via Nood- en hulpdienst toestaan */
    SCH_code[schaltg02]            = "altg02";            SCH[schaltg02]            = 0;                                    /* Alternatieve realisatie toestaan fase 02      */
    SCH_code[schaltg05]            = "altg05";            SCH[schaltg05]            = 0;                                    /* Alternatieve realisatie toestaan fase 05      */
    SCH_code[schaltg22]            = "altg22";            SCH[schaltg22]            = 0;                                    /* Alternatieve realisatie toestaan fase 22      */
    SCH_code[schaltg41]            = "altg41";            SCH[schaltg41]            = 0;                                    /* Alternatieve realisatie toestaan fase 41      */
    SCH_code[schaltg42]            = "altg42";            SCH[schaltg42]            = 0;                                    /* Alternatieve realisatie toestaan fase 42      */
    SCH_code[schaltg61]            = "altg61";            SCH[schaltg61]            = 0;                                    /* Alternatieve realisatie toestaan fase 61      */

/* parameters */
/* ---------- */
    PRM_code[prmfb]             = "fb";             PRM[prmfb]             = 0;    PRM_type[prmfb]             = TS_type; /* Instelling fasebewaking                                                        */
    PRM_code[prmxx]             = "xx";             PRM[prmxx]             = 0;                                           /* Versiebeheer xx                                                                */
    PRM_code[prmyy]             = "yy";             PRM[prmyy]             = 0;                                           /* Versiebeheer yy                                                                */
    PRM_code[prmzz]             = "zz";             PRM[prmzz]             = 0;                                           /* Versiebeheer zz                                                                */
    PRM_code[prmmk021]          = "mk021";          PRM[prmmk021]          = 0;                                           /* Meetkriterium type voor detector 021                                           */
    PRM_code[prmmk022]          = "mk022";          PRM[prmmk022]          = 0;                                           /* Meetkriterium type voor detector 022                                           */
    PRM_code[prmmk051]          = "mk051";          PRM[prmmk051]          = 0;                                           /* Meetkriterium type voor detector 051                                           */
    PRM_code[prmmk052]          = "mk052";          PRM[prmmk052]          = 0;                                           /* Meetkriterium type voor detector 052                                           */
    PRM_code[prmmk221]          = "mk221";          PRM[prmmk221]          = 0;                                           /* Meetkriterium type voor detector 221                                           */
    PRM_code[prmmk222]          = "mk222";          PRM[prmmk222]          = 0;                                           /* Meetkriterium type voor detector 222                                           */
    PRM_code[prmmkk22]          = "mkk22";          PRM[prmmkk22]          = 0;                                           /* Meetkriterium type voor detector k22                                           */
    PRM_code[prmmk411]          = "mk411";          PRM[prmmk411]          = 0;                                           /* Meetkriterium type voor detector 411                                           */
    PRM_code[prmmk412]          = "mk412";          PRM[prmmk412]          = 0;                                           /* Meetkriterium type voor detector 412                                           */
    PRM_code[prmmk421]          = "mk421";          PRM[prmmk421]          = 0;                                           /* Meetkriterium type voor detector 421                                           */
    PRM_code[prmmk422]          = "mk422";          PRM[prmmk422]          = 0;                                           /* Meetkriterium type voor detector 422                                           */
    PRM_code[prmmk611]          = "mk611";          PRM[prmmk611]          = 0;                                           /* Meetkriterium type voor detector 611                                           */
    PRM_code[prmmk612]          = "mk612";          PRM[prmmk612]          = 0;                                           /* Meetkriterium type voor detector 612                                           */
    PRM_code[prmmwta]           = "mwta";           PRM[prmmwta]           = 0;    PRM_type[prmmwta]           = TS_type; /* Maximale wachttijd autoverkeer                                                 */
    PRM_code[prmmwtfts]         = "mwtfts";         PRM[prmmwtfts]         = 0;    PRM_type[prmmwtfts]         = TS_type; /* Maximale wachttijd fiets                                                       */
    PRM_code[prmmwtvtg]         = "mwtvtg";         PRM[prmmwtvtg]         = 0;    PRM_type[prmmwtvtg]         = TS_type; /* Maximale wachttijd voetgangers                                                 */
    PRM_code[prmpmgt02]         = "pmgt02";         PRM[prmpmgt02]         = 0;                                           /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                  */
    PRM_code[prmognt02]         = "ognt02";         PRM[prmognt02]         = 0;    PRM_type[prmognt02]         = TE_type; /* Minimale groentijd bij terugkomen fase 02                                      */
    PRM_code[prmnofm02]         = "nofm02";         PRM[prmnofm02]         = 0;    PRM_type[prmnofm02]         = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 02                               */
    PRM_code[prmmgcov02]        = "mgcov02";        PRM[prmmgcov02]        = 0;    PRM_type[prmmgcov02]        = TE_type; /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov02]       = "pmgcov02";       PRM[prmpmgcov02]       = 0;                                           /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg02]        = "ohpmg02";        PRM[prmohpmg02]        = 0;                                           /* Percentage ophogen groentijd na afkappen fase 02                               */
    PRM_code[prmpmgt05]         = "pmgt05";         PRM[prmpmgt05]         = 0;                                           /* Minimaal percentage groentijd primair tbv. terugkomen fase 05                  */
    PRM_code[prmognt05]         = "ognt05";         PRM[prmognt05]         = 0;    PRM_type[prmognt05]         = TE_type; /* Minimale groentijd bij terugkomen fase 05                                      */
    PRM_code[prmnofm05]         = "nofm05";         PRM[prmnofm05]         = 0;    PRM_type[prmnofm05]         = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 05                               */
    PRM_code[prmmgcov05]        = "mgcov05";        PRM[prmmgcov05]        = 0;    PRM_type[prmmgcov05]        = TE_type; /* Minimum groentijd waarna fase 05 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov05]       = "pmgcov05";       PRM[prmpmgcov05]       = 0;                                           /* Minimum percentage groentijd waarna fase 05 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg05]        = "ohpmg05";        PRM[prmohpmg05]        = 0;                                           /* Percentage ophogen groentijd na afkappen fase 05                               */
    PRM_code[prmpmgt22]         = "pmgt22";         PRM[prmpmgt22]         = 0;                                           /* Minimaal percentage groentijd primair tbv. terugkomen fase 22                  */
    PRM_code[prmognt22]         = "ognt22";         PRM[prmognt22]         = 0;    PRM_type[prmognt22]         = TE_type; /* Minimale groentijd bij terugkomen fase 22                                      */
    PRM_code[prmnofm22]         = "nofm22";         PRM[prmnofm22]         = 0;    PRM_type[prmnofm22]         = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 22                               */
    PRM_code[prmmgcov22]        = "mgcov22";        PRM[prmmgcov22]        = 0;    PRM_type[prmmgcov22]        = TE_type; /* Minimum groentijd waarna fase 22 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov22]       = "pmgcov22";       PRM[prmpmgcov22]       = 0;                                           /* Minimum percentage groentijd waarna fase 22 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg22]        = "ohpmg22";        PRM[prmohpmg22]        = 0;                                           /* Percentage ophogen groentijd na afkappen fase 22                               */
    PRM_code[prmpmgt41]         = "pmgt41";         PRM[prmpmgt41]         = 0;                                           /* Minimaal percentage groentijd primair tbv. terugkomen fase 41                  */
    PRM_code[prmognt41]         = "ognt41";         PRM[prmognt41]         = 0;    PRM_type[prmognt41]         = TE_type; /* Minimale groentijd bij terugkomen fase 41                                      */
    PRM_code[prmnofm41]         = "nofm41";         PRM[prmnofm41]         = 0;    PRM_type[prmnofm41]         = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 41                               */
    PRM_code[prmmgcov41]        = "mgcov41";        PRM[prmmgcov41]        = 0;    PRM_type[prmmgcov41]        = TE_type; /* Minimum groentijd waarna fase 41 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov41]       = "pmgcov41";       PRM[prmpmgcov41]       = 0;                                           /* Minimum percentage groentijd waarna fase 41 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg41]        = "ohpmg41";        PRM[prmohpmg41]        = 0;                                           /* Percentage ophogen groentijd na afkappen fase 41                               */
    PRM_code[prmpmgt42]         = "pmgt42";         PRM[prmpmgt42]         = 0;                                           /* Minimaal percentage groentijd primair tbv. terugkomen fase 42                  */
    PRM_code[prmognt42]         = "ognt42";         PRM[prmognt42]         = 0;    PRM_type[prmognt42]         = TE_type; /* Minimale groentijd bij terugkomen fase 42                                      */
    PRM_code[prmnofm42]         = "nofm42";         PRM[prmnofm42]         = 0;    PRM_type[prmnofm42]         = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 42                               */
    PRM_code[prmmgcov42]        = "mgcov42";        PRM[prmmgcov42]        = 0;    PRM_type[prmmgcov42]        = TE_type; /* Minimum groentijd waarna fase 42 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov42]       = "pmgcov42";       PRM[prmpmgcov42]       = 0;                                           /* Minimum percentage groentijd waarna fase 42 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg42]        = "ohpmg42";        PRM[prmohpmg42]        = 0;                                           /* Percentage ophogen groentijd na afkappen fase 42                               */
    PRM_code[prmpmgt61]         = "pmgt61";         PRM[prmpmgt61]         = 0;                                           /* Minimaal percentage groentijd primair tbv. terugkomen fase 61                  */
    PRM_code[prmognt61]         = "ognt61";         PRM[prmognt61]         = 0;    PRM_type[prmognt61]         = TE_type; /* Minimale groentijd bij terugkomen fase 61                                      */
    PRM_code[prmnofm61]         = "nofm61";         PRM[prmnofm61]         = 0;    PRM_type[prmnofm61]         = TE_type; /* Aantal malen niet afkappen na OV ingreep fase 61                               */
    PRM_code[prmmgcov61]        = "mgcov61";        PRM[prmmgcov61]        = 0;    PRM_type[prmmgcov61]        = TE_type; /* Minimum groentijd waarna fase 61 afgkapt mag worden door OV ingreep            */
    PRM_code[prmpmgcov61]       = "pmgcov61";       PRM[prmpmgcov61]       = 0;                                           /* Minimum percentage groentijd waarna fase 61 afgkapt mag worden door OV ingreep */
    PRM_code[prmohpmg61]        = "ohpmg61";        PRM[prmohpmg61]        = 0;                                           /* Percentage ophogen groentijd na afkappen fase 61                               */
    PRM_code[prmrto41tram]      = "rto41tram";      PRM[prmrto41tram]      = 150;  PRM_type[prmrto41tram]      = TE_type; /* Ongehinderde rijtijd prioriteit fase 41                                        */
    PRM_code[prmrtbg41tram]     = "rtbg41tram";     PRM[prmrtbg41tram]     = 200;  PRM_type[prmrtbg41tram]     = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    PRM_code[prmrtg41tram]      = "rtg41tram";      PRM[prmrtg41tram]      = 300;  PRM_type[prmrtg41tram]      = TE_type; /* Gehinderde rijtijd prioriteit fase 41                                          */
    PRM_code[prmomx41tram]      = "omx41tram";      PRM[prmomx41tram]      = 50;   PRM_type[prmomx41tram]      = TE_type; /* Ondermaximum OV fase 41                                                        */
    PRM_code[prmupinagb41tram]  = "upinagb41tram";  PRM[prmupinagb41tram]  = 0;                                           /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    PRM_code[prmvtgcat41tram]   = "vtgcat41tram";   PRM[prmvtgcat41tram]   = 2;                                           /* Voertuigcategorie DSI voor prio ingreep 41tram                                 */
    PRM_code[prmprio41tram]     = "prio41tram";     PRM[prmprio41tram]     = 0;                                           /* Prioriteitsinstelling OV fase 41                                               */
    PRM_code[prmrto41bus]       = "rto41bus";       PRM[prmrto41bus]       = 150;  PRM_type[prmrto41bus]       = TE_type; /* Ongehinderde rijtijd prioriteit fase 41                                        */
    PRM_code[prmrtbg41bus]      = "rtbg41bus";      PRM[prmrtbg41bus]      = 200;  PRM_type[prmrtbg41bus]      = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    PRM_code[prmrtg41bus]       = "rtg41bus";       PRM[prmrtg41bus]       = 300;  PRM_type[prmrtg41bus]       = TE_type; /* Gehinderde rijtijd prioriteit fase 41                                          */
    PRM_code[prmomx41bus]       = "omx41bus";       PRM[prmomx41bus]       = 50;   PRM_type[prmomx41bus]       = TE_type; /* Ondermaximum OV fase 41                                                        */
    PRM_code[prmupinagb41bus]   = "upinagb41bus";   PRM[prmupinagb41bus]   = 0;                                           /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    PRM_code[prmvtgcat41bus]    = "vtgcat41bus";    PRM[prmvtgcat41bus]    = 1;                                           /* Voertuigcategorie DSI voor prio ingreep 41bus                                  */
    PRM_code[prmprio41bus]      = "prio41bus";      PRM[prmprio41bus]      = 0;                                           /* Prioriteitsinstelling OV fase 41                                               */
    PRM_code[prmrto42bus]       = "rto42bus";       PRM[prmrto42bus]       = 150;  PRM_type[prmrto42bus]       = TE_type; /* Ongehinderde rijtijd prioriteit fase 42                                        */
    PRM_code[prmrtbg42bus]      = "rtbg42bus";      PRM[prmrtbg42bus]      = 200;  PRM_type[prmrtbg42bus]      = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 42                                  */
    PRM_code[prmrtg42bus]       = "rtg42bus";       PRM[prmrtg42bus]       = 300;  PRM_type[prmrtg42bus]       = TE_type; /* Gehinderde rijtijd prioriteit fase 42                                          */
    PRM_code[prmomx42bus]       = "omx42bus";       PRM[prmomx42bus]       = 50;   PRM_type[prmomx42bus]       = TE_type; /* Ondermaximum OV fase 42                                                        */
    PRM_code[prmupinagb42bus]   = "upinagb42bus";   PRM[prmupinagb42bus]   = 0;                                           /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 42                  */
    PRM_code[prmvtgcat42bus]    = "vtgcat42bus";    PRM[prmvtgcat42bus]    = 1;                                           /* Voertuigcategorie DSI voor prio ingreep 42bus                                  */
    PRM_code[prmprio42bus]      = "prio42bus";      PRM[prmprio42bus]      = 0;                                           /* Prioriteitsinstelling OV fase 42                                               */
    PRM_code[prmrto22fiets]     = "rto22fiets";     PRM[prmrto22fiets]     = 150;  PRM_type[prmrto22fiets]     = TE_type; /* Ongehinderde rijtijd prioriteit fase 22                                        */
    PRM_code[prmrtbg22fiets]    = "rtbg22fiets";    PRM[prmrtbg22fiets]    = 200;  PRM_type[prmrtbg22fiets]    = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 22                                  */
    PRM_code[prmrtg22fiets]     = "rtg22fiets";     PRM[prmrtg22fiets]     = 300;  PRM_type[prmrtg22fiets]     = TE_type; /* Gehinderde rijtijd prioriteit fase 22                                          */
    PRM_code[prmomx22fiets]     = "omx22fiets";     PRM[prmomx22fiets]     = 50;   PRM_type[prmomx22fiets]     = TE_type; /* Ondermaximum OV fase 22                                                        */
    PRM_code[prmupinagb22fiets] = "upinagb22fiets"; PRM[prmupinagb22fiets] = 0;                                           /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 22                  */
    PRM_code[prmvtgcat22fiets]  = "vtgcat22fiets";  PRM[prmvtgcat22fiets]  = 0;                                           /* Voertuigcategorie DSI voor prio ingreep 22fiets                                */
    PRM_code[prmprio22fiets]    = "prio22fiets";    PRM[prmprio22fiets]    = 0;                                           /* Prioriteitsinstelling OV fase 22                                               */
    PRM_code[prmrto02vrw]       = "rto02vrw";       PRM[prmrto02vrw]       = 150;  PRM_type[prmrto02vrw]       = TE_type; /* Ongehinderde rijtijd prioriteit fase 02                                        */
    PRM_code[prmrtbg02vrw]      = "rtbg02vrw";      PRM[prmrtbg02vrw]      = 200;  PRM_type[prmrtbg02vrw]      = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 02                                  */
    PRM_code[prmrtg02vrw]       = "rtg02vrw";       PRM[prmrtg02vrw]       = 300;  PRM_type[prmrtg02vrw]       = TE_type; /* Gehinderde rijtijd prioriteit fase 02                                          */
    PRM_code[prmomx02vrw]       = "omx02vrw";       PRM[prmomx02vrw]       = 50;   PRM_type[prmomx02vrw]       = TE_type; /* Ondermaximum OV fase 02                                                        */
    PRM_code[prmupinagb02vrw]   = "upinagb02vrw";   PRM[prmupinagb02vrw]   = 0;                                           /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 02                  */
    PRM_code[prmvtgcat02vrw]    = "vtgcat02vrw";    PRM[prmvtgcat02vrw]    = 0;                                           /* Voertuigcategorie DSI voor prio ingreep 02vrw                                  */
    PRM_code[prmprio02vrw]      = "prio02vrw";      PRM[prmprio02vrw]      = 0;                                           /* Prioriteitsinstelling OV fase 02                                               */
    PRM_code[prmrto05auto]      = "rto05auto";      PRM[prmrto05auto]      = 150;  PRM_type[prmrto05auto]      = TE_type; /* Ongehinderde rijtijd prioriteit fase 05                                        */
    PRM_code[prmrtbg05auto]     = "rtbg05auto";     PRM[prmrtbg05auto]     = 200;  PRM_type[prmrtbg05auto]     = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 05                                  */
    PRM_code[prmrtg05auto]      = "rtg05auto";      PRM[prmrtg05auto]      = 300;  PRM_type[prmrtg05auto]      = TE_type; /* Gehinderde rijtijd prioriteit fase 05                                          */
    PRM_code[prmomx05auto]      = "omx05auto";      PRM[prmomx05auto]      = 50;   PRM_type[prmomx05auto]      = TE_type; /* Ondermaximum OV fase 05                                                        */
    PRM_code[prmupinagb05auto]  = "upinagb05auto";  PRM[prmupinagb05auto]  = 0;                                           /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 05                  */
    PRM_code[prmvtgcat05auto]   = "vtgcat05auto";   PRM[prmvtgcat05auto]   = 0;                                           /* Voertuigcategorie DSI voor prio ingreep 05auto                                 */
    PRM_code[prmprio05auto]     = "prio05auto";     PRM[prmprio05auto]     = 0;                                           /* Prioriteitsinstelling OV fase 05                                               */
    PRM_code[prmrto05ng]        = "rto05ng";        PRM[prmrto05ng]        = 150;  PRM_type[prmrto05ng]        = TE_type; /* Ongehinderde rijtijd prioriteit fase 05                                        */
    PRM_code[prmrtbg05ng]       = "rtbg05ng";       PRM[prmrtbg05ng]       = 200;  PRM_type[prmrtbg05ng]       = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 05                                  */
    PRM_code[prmrtg05ng]        = "rtg05ng";        PRM[prmrtg05ng]        = 300;  PRM_type[prmrtg05ng]        = TE_type; /* Gehinderde rijtijd prioriteit fase 05                                          */
    PRM_code[prmomx05ng]        = "omx05ng";        PRM[prmomx05ng]        = 50;   PRM_type[prmomx05ng]        = TE_type; /* Ondermaximum OV fase 05                                                        */
    PRM_code[prmupinagb05ng]    = "upinagb05ng";    PRM[prmupinagb05ng]    = 0;                                           /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 05                  */
    PRM_code[prmvtgcat05ng]     = "vtgcat05ng";     PRM[prmvtgcat05ng]     = 0;                                           /* Voertuigcategorie DSI voor prio ingreep 05ng                                   */
    PRM_code[prmprio05ng]       = "prio05ng";       PRM[prmprio05ng]       = 0;                                           /* Prioriteitsinstelling OV fase 05                                               */
    PRM_code[prmrto61hd]        = "rto61hd";        PRM[prmrto61hd]        = 150;  PRM_type[prmrto61hd]        = TE_type; /* Ongehinderde rijtijd prioriteit fase 61                                        */
    PRM_code[prmrtbg61hd]       = "rtbg61hd";       PRM[prmrtbg61hd]       = 200;  PRM_type[prmrtbg61hd]       = TE_type; /* Beperkt gehinderde rijtijd prioriteit fase 61                                  */
    PRM_code[prmrtg61hd]        = "rtg61hd";        PRM[prmrtg61hd]        = 300;  PRM_type[prmrtg61hd]        = TE_type; /* Gehinderde rijtijd prioriteit fase 61                                          */
    PRM_code[prmomx61hd]        = "omx61hd";        PRM[prmomx61hd]        = 50;   PRM_type[prmomx61hd]        = TE_type; /* Ondermaximum OV fase 61                                                        */
    PRM_code[prmupinagb61hd]    = "upinagb61hd";    PRM[prmupinagb61hd]    = 0;                                           /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 61                  */
    PRM_code[prmvtgcat61hd]     = "vtgcat61hd";     PRM[prmvtgcat61hd]     = 0;                                           /* Voertuigcategorie DSI voor prio ingreep 61hd                                   */
    PRM_code[prmprio61hd]       = "prio61hd";       PRM[prmprio61hd]       = 9005;                                        /* Prioriteitsinstelling OV fase 61                                               */
    PRM_code[prmstkp1]          = "stkp1";          PRM[prmstkp1]          = 600;  PRM_type[prmstkp1]          = TI_type; /* Start klokperiode ochtend                                                      */
    PRM_code[prmetkp1]          = "etkp1";          PRM[prmetkp1]          = 900;  PRM_type[prmetkp1]          = TI_type; /* Einde klokperiode ochtend                                                      */
    PRM_code[prmdckp1]          = "dckp1";          PRM[prmdckp1]          = 8;                                           /* Dagsoort klokperiode ochtend                                                   */
    PRM_code[prmmg1_02]         = "mg1_02";         PRM[prmmg1_02]         = 300;  PRM_type[prmmg1_02]         = TE_type; /* Maximale groentijd MG1 02                                                      */
    PRM_code[prmmg1_05]         = "mg1_05";         PRM[prmmg1_05]         = 300;  PRM_type[prmmg1_05]         = TE_type; /* Maximale groentijd MG1 05                                                      */
    PRM_code[prmmg1_22]         = "mg1_22";         PRM[prmmg1_22]         = 300;  PRM_type[prmmg1_22]         = TE_type; /* Maximale groentijd MG1 22                                                      */
    PRM_code[prmmg1_41]         = "mg1_41";         PRM[prmmg1_41]         = 300;  PRM_type[prmmg1_41]         = TE_type; /* Maximale groentijd MG1 41                                                      */
    PRM_code[prmmg1_42]         = "mg1_42";         PRM[prmmg1_42]         = 300;  PRM_type[prmmg1_42]         = TE_type; /* Maximale groentijd MG1 42                                                      */
    PRM_code[prmmg1_61]         = "mg1_61";         PRM[prmmg1_61]         = 300;  PRM_type[prmmg1_61]         = TE_type; /* Maximale groentijd MG1 61                                                      */
    PRM_code[prmmg2_02]         = "mg2_02";         PRM[prmmg2_02]         = 200;  PRM_type[prmmg2_02]         = TE_type; /* Maximale groentijd MG2 02                                                      */
    PRM_code[prmmg2_05]         = "mg2_05";         PRM[prmmg2_05]         = 200;  PRM_type[prmmg2_05]         = TE_type; /* Maximale groentijd MG2 05                                                      */
    PRM_code[prmmg2_22]         = "mg2_22";         PRM[prmmg2_22]         = 200;  PRM_type[prmmg2_22]         = TE_type; /* Maximale groentijd MG2 22                                                      */
    PRM_code[prmmg2_41]         = "mg2_41";         PRM[prmmg2_41]         = 200;  PRM_type[prmmg2_41]         = TE_type; /* Maximale groentijd MG2 41                                                      */
    PRM_code[prmmg2_42]         = "mg2_42";         PRM[prmmg2_42]         = 200;  PRM_type[prmmg2_42]         = TE_type; /* Maximale groentijd MG2 42                                                      */
    PRM_code[prmmg2_61]         = "mg2_61";         PRM[prmmg2_61]         = 200;  PRM_type[prmmg2_61]         = TE_type; /* Maximale groentijd MG2 61                                                      */
    PRM_code[prmmlfpr02]        = "mlfpr02";        PRM[prmmlfpr02]        = 0;                                           /* Maximaal aantal modules vooruit fase 02                                        */
    PRM_code[prmmlfpr05]        = "mlfpr05";        PRM[prmmlfpr05]        = 0;                                           /* Maximaal aantal modules vooruit fase 05                                        */
    PRM_code[prmmlfpr22]        = "mlfpr22";        PRM[prmmlfpr22]        = 0;                                           /* Maximaal aantal modules vooruit fase 22                                        */
    PRM_code[prmmlfpr41]        = "mlfpr41";        PRM[prmmlfpr41]        = 0;                                           /* Maximaal aantal modules vooruit fase 41                                        */
    PRM_code[prmmlfpr42]        = "mlfpr42";        PRM[prmmlfpr42]        = 0;                                           /* Maximaal aantal modules vooruit fase 42                                        */
    PRM_code[prmmlfpr61]        = "mlfpr61";        PRM[prmmlfpr61]        = 0;                                           /* Maximaal aantal modules vooruit fase 61                                        */
    PRM_code[prmaltg02]         = "altg02";         PRM[prmaltg02]         = 0;    PRM_type[prmaltg02]         = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    PRM_code[prmaltp02]         = "altp02";         PRM[prmaltp02]         = 0;    PRM_type[prmaltp02]         = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    PRM_code[prmaltg05]         = "altg05";         PRM[prmaltg05]         = 0;    PRM_type[prmaltg05]         = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 05                         */
    PRM_code[prmaltp05]         = "altp05";         PRM[prmaltp05]         = 0;    PRM_type[prmaltp05]         = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 05                            */
    PRM_code[prmaltg22]         = "altg22";         PRM[prmaltg22]         = 0;    PRM_type[prmaltg22]         = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 22                         */
    PRM_code[prmaltp22]         = "altp22";         PRM[prmaltp22]         = 0;    PRM_type[prmaltp22]         = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 22                            */
    PRM_code[prmaltg41]         = "altg41";         PRM[prmaltg41]         = 0;    PRM_type[prmaltg41]         = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 41                         */
    PRM_code[prmaltp41]         = "altp41";         PRM[prmaltp41]         = 0;    PRM_type[prmaltp41]         = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 41                            */
    PRM_code[prmaltg42]         = "altg42";         PRM[prmaltg42]         = 0;    PRM_type[prmaltg42]         = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 42                         */
    PRM_code[prmaltp42]         = "altp42";         PRM[prmaltp42]         = 0;    PRM_type[prmaltp42]         = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 42                            */
    PRM_code[prmaltg61]         = "altg61";         PRM[prmaltg61]         = 0;    PRM_type[prmaltg61]         = TE_type; /* Minimale groentijd bij alternatieve realisatie fase 61                         */
    PRM_code[prmaltp61]         = "altp61";         PRM[prmaltp61]         = 0;    PRM_type[prmaltp61]         = TE_type; /* Minimale ruimte tbv alternatieve realisatie fase 61                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    PRM_code[prmtestdsivert]    = "testdsivert";    PRM[prmtestdsivert]    = 120;  PRM_type[prmtestdsivert]    = TS_type; /* Testen vertraging in DSI bericht in testomgeving                               */
    PRM_code[prmtestdsilyn]     = "testdsilyn";     PRM[prmtestdsilyn]     = 0;                                /* Testen lijnnummer DSI bericht in testomgeving                                  */
    PRM_code[prmtestdsicat]     = "testdsicat";     PRM[prmtestdsicat]     = 10;                               /* Testen ritcategorie DSI bericht in testomgeving                                */
#endif


/* extra data */
/* ---------- */
    FC_type[fc02] = MVT_type;
    FC_type[fc05] = MVT_type;
    FC_type[fc22] = FTS_type;
    FC_type[fc41] = OV_type;
    FC_type[fc42] = OV_type;
    FC_type[fc61] = MVT_type;

/* Selectieve detectie */
/* ------------------- */
    DS_code[dsdummy] = "dsdummy";

/* modulen */
/* ------- */
    PRML[ML1][fc02] = PRIMAIR;

    PRML[ML2][fc05] = PRIMAIR;

    PRML[ML3][fc22] = PRIMAIR;

    PRML[ML4][fc41] = PRIMAIR;

    PRML[ML5][fc42] = PRIMAIR;

    PRML[ML6][fc61] = PRIMAIR;


/* Typen ingangen */
/* -------------- */
    IS_type[d021] = DKOP_type;
    IS_type[d022] = DLNG_type;
    IS_type[d051] = DKOP_type;
    IS_type[d052] = DLNG_type;
    IS_type[d221] = DKOP_type;
    IS_type[d222] = DVER_type;
    IS_type[dk22] = DK_type;
    IS_type[d411] = DKOP_type;
    IS_type[d412] = DLNG_type;
    IS_type[d421] = DKOP_type;
    IS_type[d422] = DLNG_type;
    IS_type[d611] = DKOP_type;
    IS_type[d612] = DLNG_type;

/* Typen uitgangen */
/* --------------- */
    US_type[fc02] = MVT_type;
    US_type[fc05] = MVT_type;
    US_type[fc22] = FTS_type;
    US_type[fc41] = OV_type;
    US_type[fc42] = OV_type;
    US_type[fc61] = MVT_type;

#ifdef VISSIM
#endif

    #include "GMtab.add"
}
