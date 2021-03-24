/* timingsfunc.c - gegenereerd met TLCGen 0.9.1.0 */

/* DEFINITIE FUNCTIE DEMO_MSG_FCTIMING */
/* =================================== */


/* CCOL:  version 11.0	      */
/* FILE:  demo_msg_fctiming.c */

/* DATUM: 11-10-2020 - start regelen, inschakelen, uitschakelen en fatale fout zijn toegevoegd */
/* DATUM: 05-08-2020 - maxEndTime is een verplicht veld geworden in de Dutch Profiles */
/*                   - maxEndTime voor Geel toegevoegd                                */
/*                   - Groen Knipperen opgeven als Geel voor voetgangers              */
/* DATUM: 30-04-2020          */

/* 0.9.1.0 25-01-2021  PSN CCA:      REALTIJD_min REALTIJD_max toegevoegd tbv UC4 */
/* 4.6.0    19-02-2021   Cyril       Latency correctie + define NO_WATCHOTHERTRAFFIC geactiveerd */
/* .........27-02-2021  M Scherjon    Latency als Define */
/* .........27-02-2021  M Scherjon    Bug fix   STARTTIME op 'NG' tijdens R[] en !SR[] */
/* .........03-03-2021  M Scherjon    verwisseling van array index 2 en 3 in CCOL_FC_TIMING[][][] en CCOL_FC_TIMING_old[][][] verholpen */
/* .........03-03-2021  M Scherjon    define CIF_MAX_TIMING bij CCOL_FC_TIMING[][][] en CCOL_FC_TIMING_old[][][] toegevoegd i.p.v. 8 */


/* include files */
/* ============= */
   #include "sysdef.c"
   #include "cif.inc"         /* declaratie CVN C-interface	  */
#ifndef FCPRM
   #include "fcvar.h"         /* declaratie fasecycli variabelen  */
#endif

#define LATENCY_CORRECTION_MAX_END   3   /* 0,3 seconden */
#ifndef SPAT_TIJD_ONBEKEND
   #define SPAT_TIJD_ONBEKEND           -32768   /* onbekende waarde CIF_FC_TIMING[][2] (type Time) */
#endif

/* DEFINITIE EVENTSTATE */
/* ==================== */

#ifdef EVENTSTATE_MACRODEFINITIES_CIF_INC

/* Macrodefinities status EVENTSTATE (Nederlands) */
/* ---------------------------------------------- */
#define CIF_TIMING_ONBEKEND           0    /* Unknown(0)                             */
#define CIF_TIMING_GEDOOFD            1    /* Dark(1)                                */
#define CIF_TIMING_ROOD_KNIPPEREN     2    /* stop - Then - Proceed(2)               */
#define CIF_TIMING_ROOD               3    /* stop - And - Remain(3)                 */
#define CIF_TIMING_GROEN_OVERGANG     4    /* pre - Movement(4) - not used in NL     */
#define CIF_TIMING_GROEN_DEELCONFLICT 5    /* permissive - Movement - Allowed(5)     */
#define CIF_TIMING_GROEN              6    /* protected - Movement - Allowed(6)      */
#define CIF_TIMING_GEEL_DEELCONFLICT  7    /* permissive - clearance(7)              */
#define CIF_TIMING_GEEL               8    /* protected - clearance(8)               */
#define CIF_TIMING_GEEL_KNIPPEREN     9    /* caution - Conflicting - Traffic(9)     */
#define CIF_TIMING_GROEN_KNIPPEREN_DEELCONFLICT 10    /* permissive - Movement - PreClearance - not in J2735 */
#define CIF_TIMING_GROEN_KNIPPEREN              11    /* protected -  Movement - PreClearance - not in J2735 */

#endif


/* Definitie EventState */
/* ==================== */
s_int16 CCOL_FC_EVENTSTATE[FCMAX][3];
s_int16 CCOL_FC_TIMING[FCMAX][CIF_MAX_EVENT][CIF_MAX_TIMING];
s_int16 CCOL_FC_TIMING_old[FCMAX][CIF_MAX_EVENT][CIF_MAX_TIMING];
s_int16 CCOL_FC_TIMING_CIF_TIMING_MAXENDTIME_temp;

/* DEFINITIE FCTMING FUNCTIES */
/* ========================== */

/* functie voor het wegschrijven van CIF_FC_TIMING[] events */
/* -------------------------------------------------------- */
/* de functie set_fctiming() schrijft de fc_timing informatie naar de CIF_FC_TIMING buffer.
 * de functie set_fctiming() wordt aangeroepen in de functie msg_fctiming().
 */

s_int16 set_fctiming(mulv i, mulv eventnr, s_int16 mask, s_int16 eventState, s_int16 startTime, s_int16 minEndTime, s_int16 maxEndTime,
s_int16 likelyTime, s_int16 confidence, s_int16 nextTime)
{
   if ((i < FC_MAX) && (eventnr < CIF_MAX_EVENT)) {

      CIF_FC_TIMING[i][eventnr][CIF_TIMING_MASK] = mask;

      CIF_FC_TIMING[i][eventnr][CIF_TIMING_EVENTSTATE]= eventState;
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_STARTTIME] = startTime;
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_MINENDTIME]= minEndTime;
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_MAXENDTIME]= maxEndTime;
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_MAXENDTIME]= maxEndTime;
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_LIKELYTIME]= likelyTime;
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_CONFIDENCE]= confidence;
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_NEXTTIME]  = nextTime;

      CIF_FC_TIMING_WIJZ[i]= TRUE; /* zet wijzigvlag */

      return (TRUE);
   }
   else {
      return (FALSE);
   }
}
    


/* functie voor het resetten van CIF_FC_TIMING[] event */
/* --------------------------------------------------- */
/* de functie reset_fctiming() reset de fc_timing informatie in de CIF_FC_TIMING buffer.
 * de functie reset_fctiming() wordt aangeroepen in de functie msg_fctiming().
 */
s_int16 reset_fctiming(mulv i, mulv eventnr)
{
   if ((i < FC_MAX) && (eventnr < CIF_MAX_EVENT)) {

      CIF_FC_TIMING[i][eventnr][CIF_TIMING_MASK]= 0;
      
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_EVENTSTATE]= 0;
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_STARTTIME] = 0;
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_MINENDTIME]= 0;
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_MAXENDTIME]= 0;
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_LIKELYTIME]= 0;
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_CONFIDENCE]= 0;
      CIF_FC_TIMING[i][eventnr][CIF_TIMING_NEXTTIME]  = 0;

      CIF_FC_TIMING_WIJZ[i]= TRUE; /* zet wijzigvlag */
      
      return (TRUE);
   }
   else {
      return (FALSE);
   }
}



/* functie FC_TIMING[] events */
/* -------------------------- */
/* de functie msg_fctiming() bepaalt de fc_timing informatie in de CIF_FC_TIMING buffer.
 * de functie msg_fctiming() gebruikt de functies Evenstate_Definition(), set_fctiming() en reset_fctiming(),
 * de functie msg_fctiming() moet worden aangeroepen in de functie system_application2().
 * 
 */
void msg_fctiming(void)
{
   register count i;
   static s_int16 WPS_old= 0;       /* oude programmastatus		*/
   static boolv init = 0;           /* initialisatievlag                */

   for (i=0; i<FC_MAX; i++) {

      if ( (CCOL_FC_EVENTSTATE[i][CIF_ROOD] != CIF_TIMING_ONBEKEND) &&   /* geen SPAT versturen indien er geen eventstate is opgegeven. bijvoorbeeld voor verschijnborden */
           (CCOL_FC_EVENTSTATE[i][CIF_GROEN] != CIF_TIMING_ONBEKEND) &&
           (CCOL_FC_EVENTSTATE[i][CIF_GEEL] != CIF_TIMING_ONBEKEND) ) {

         switch (CIF_WPS[CIF_PROG_STATUS]) {

         case CIF_STAT_ONGEDEF:  /* Ongedefinieerd */
            if (CIF_WPS[CIF_PROG_STATUS] != WPS_old) {
               set_fctiming((mulv) i, /* fc */
                  (mulv) 0,                                /* event      */
                  (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_MINENDTIME | CIF_TIMING_MASK_MAXENDTIME), /* mask */
                  (s_int16) CIF_TIMING_ONBEKEND,           /* eventState */
                  (s_int16) SPAT_TIJD_ONBEKEND,            /* startTime  */
                  (s_int16) NG,                            /* minEndTime */
                  (s_int16) NG,                            /* maxEndTime */
                  (s_int16) NG,                            /* likelyTime */
                  (s_int16) NG,                            /* confidence */
                  (s_int16) NG);                           /* nextTime   */
               reset_fctiming((mulv) i, (mulv) 1);
            }
            break;

         case CIF_STAT_GEDOOFD: /* Gedoofd */
            if (CIF_WPS[CIF_PROG_STATUS] != WPS_old) {
               set_fctiming((mulv) i, /* fc */
                  (mulv) 0,                                /* event      */
                  (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_MINENDTIME | CIF_TIMING_MASK_MAXENDTIME), /* mask */
                  (s_int16) CIF_TIMING_GEDOOFD,            /* eventState */
                  (s_int16) SPAT_TIJD_ONBEKEND,            /* startTime  */
                  (s_int16) NG,                            /* minEndTime */
                  (s_int16) NG,                            /* maxEndTime */
                  (s_int16) NG,                            /* likelyTime */
                  (s_int16) NG,                            /* confidence */
                  (s_int16) NG );                           /* nextTime   */
               reset_fctiming((mulv) i, (mulv) 1);
            }
            break;

         case CIF_STAT_KP: /* Geel knipperen */
            if (CIF_WPS[CIF_PROG_STATUS] != WPS_old) {
               set_fctiming((mulv) i, /* fc */
                  (mulv) 0,                                /* event      */
                  (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_MINENDTIME | CIF_TIMING_MASK_MAXENDTIME), /* mask */
                  (s_int16) CIF_TIMING_GEEL_KNIPPEREN,     /* eventState */
                  (s_int16) SPAT_TIJD_ONBEKEND,            /* startTime  */
                  (s_int16) NG,                            /* minEndTime */
                  (s_int16) NG,                            /* maxEndTime */
                  (s_int16) NG,                            /* likelyTime */
                  (s_int16) NG,                            /* confidence */
                  (s_int16) NG);                           /* nextTime   */
               reset_fctiming((mulv) i, (mulv) 1);
            }
            break;

#if (CCOL_V > 100)
         case CIF_STAT_INSCHAKELEN:  /* Inschakelen */
            if (CIF_WPS[CIF_PROG_STATUS] != WPS_old) {
               set_fctiming((mulv) i, /* fc */
                  (mulv) 0,                                /* event      */
                  (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_MINENDTIME | CIF_TIMING_MASK_MAXENDTIME), /* mask */
                  (s_int16) CIF_TIMING_ONBEKEND,           /* eventState */
                  (s_int16) SPAT_TIJD_ONBEKEND,            /* startTime  */
                  (s_int16) NG,                            /* minEndTime */
                  (s_int16) NG,                            /* maxEndTime */
                  (s_int16) NG,                            /* likelyTime */
                  (s_int16) NG,                            /* confidence */
                  (s_int16) NG);                           /* nextTime   */
               reset_fctiming((mulv) i, (mulv) 1);
            }
            break;
#endif

         case CIF_STAT_AR: /* Alles rood */
            if (CIF_WPS[CIF_PROG_STATUS] != WPS_old) {
               set_fctiming((mulv) i, /* fc */
                  (mulv) 0,                                /* event      */
                  (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_MINENDTIME | CIF_TIMING_MASK_MAXENDTIME), /* mask */
                  (s_int16) CIF_TIMING_ROOD,               /* eventState */
                  (s_int16) SPAT_TIJD_ONBEKEND,            /* startTime  */
                  (s_int16) NG,                            /* minEndTime */
                  (s_int16) NG,                            /* maxEndTime */
                  (s_int16) NG,                            /* likelyTime */
                  (s_int16) NG,                            /* confidence */
                  (s_int16) NG);                           /* nextTime   */
               reset_fctiming((mulv) i, (mulv) 1);
            }
            break;

         case CIF_STAT_REG:   /* Regelen */
            if (CIF_WPS[CIF_PROG_STATUS] != WPS_old) {   /* start regelen */
               if (R[i]) { 
                  set_fctiming((mulv) i, /* fc */
                     (mulv) 0,                                     /* event      */
                     (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_MINENDTIME | CIF_TIMING_MASK_MAXENDTIME | CIF_TIMING_MASK_CONFIDENCE), /* mask */
                     (s_int16) CCOL_FC_EVENTSTATE[i][CIF_ROOD],    /* eventState */
                     (s_int16) SPAT_TIJD_ONBEKEND,                 /* startTime  */
                     (s_int16) NG,                                 /* minEndTime */
                     (s_int16) NG,                                 /* maxEndTime */
                     (s_int16) NG,                                 /* likelyTime */
                     (s_int16) NG,                                 /* confidence */
                     (s_int16) NG );                                /* nextTime   */
                  reset_fctiming((mulv) i, (mulv) 1);
               }
            }
            else { /* regelen */
               if (SR[i]) {
                  set_fctiming((mulv) i, /* fc */
                     (mulv) 0,                                     /* event      */
                     (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_STARTTIME | CIF_TIMING_MASK_MINENDTIME | CIF_TIMING_MASK_MAXENDTIME | CIF_TIMING_MASK_CONFIDENCE | CIF_TIMING_MASK_LIKELYTIME), /* mask */
                     (s_int16) CCOL_FC_EVENTSTATE[i][CIF_ROOD],    /* eventState */
                     (s_int16) 0,                                  /* startTime  */
                     (s_int16) NG,				   /* minEndTime */
                     (s_int16) NG,                                 /* maxEndTime */
                     (s_int16) NG,                                 /* likelyTime */
                     (s_int16) CCOL_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE],                   /* confidence */
                     (s_int16) NG);                                                          /* nextTime   */
                  reset_fctiming((mulv) i, (mulv) 1);
               }
               else if (R[i] && /* !SR[i] && */
                                 /* Elke tiende behandelen */
                                 TE &&
                                 /* Confidence wijzigt */
                                 (((CCOL_FC_TIMING_old[i][0][CIF_TIMING_CONFIDENCE] != CCOL_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE])) ||
                                 /* Minend tijd is niet gelijk aan elkaar of 1TE verschil */
                                 (((CCOL_FC_TIMING_old[i][0][CIF_TIMING_MINENDTIME] != CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME])) && ((CCOL_FC_TIMING_old[i][0][CIF_TIMING_MINENDTIME] - TE) != CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME])) ||
                                 /* Maxend tijd is niet gelijk aan elkaar of 1TE verschil */
                                 (((CCOL_FC_TIMING_old[i][0][CIF_TIMING_MAXENDTIME] != CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME])) && ((CCOL_FC_TIMING_old[i][0][CIF_TIMING_MAXENDTIME] - TE) != CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME]))) ) {
                                 /* Latency correctie */
                                 CCOL_FC_TIMING_CIF_TIMING_MAXENDTIME_temp = CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME];
                                 if ( CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME] != NG ) CCOL_FC_TIMING_CIF_TIMING_MAXENDTIME_temp = (CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME] + LATENCY_CORRECTION_MAX_END);
                  set_fctiming((mulv) i, /* fc */
                     (mulv) 0,                                                             /* event      */
                     (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_STARTTIME | CIF_TIMING_MASK_MINENDTIME | CIF_TIMING_MASK_MAXENDTIME | CIF_TIMING_MASK_LIKELYTIME | CIF_TIMING_MASK_CONFIDENCE), /* mask */
                     (s_int16) CCOL_FC_EVENTSTATE[i][CIF_ROOD],                            /* eventState */
                     (s_int16) CCOL_FC_TIMING[i][0][CIF_TIMING_STARTTIME],                 /* startTime  */ 
                     (s_int16) CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME],                /* minEndTime */
                     (s_int16) CCOL_FC_TIMING_CIF_TIMING_MAXENDTIME_temp,                  /* maxEndTime */
                     (s_int16) CCOL_FC_TIMING[i][0][CIF_TIMING_LIKELYTIME],                /* likelyTime */
                     (s_int16) CCOL_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE],                /* confidence */
                     (s_int16) NG );                                                        /* nextTime   */
                  reset_fctiming((mulv) i, (mulv)1);
               }
               else if (SG[i]) {
                  set_fctiming((mulv) i, /* fc */
                     (mulv) 0,                                     /* event      */
                     (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_STARTTIME | CIF_TIMING_MASK_MINENDTIME | CIF_TIMING_MASK_MAXENDTIME), /* mask */
                     (s_int16) CCOL_FC_EVENTSTATE[i][CIF_GROEN],   /* eventState */
                     (s_int16) 0,                                  /* startTime  */
                     (s_int16) TGG_max[i],                         /* minEndTime */
                     (s_int16) NG,                                 /* maxEndTime */
                     (s_int16) NG,                                 /* likelyTime */
                     (s_int16) NG,                                 /* confidence */
                     (s_int16) NG);                                /* nextTime   */
                  reset_fctiming((mulv) i, (mulv)1);
               }
               else if (SGL[i]) {
                  set_fctiming((mulv) i, /* fc */
                     (mulv) 0,                                     /* event      */
                     (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_STARTTIME | CIF_TIMING_MASK_MINENDTIME | CIF_TIMING_MASK_MAXENDTIME | CIF_TIMING_MASK_LIKELYTIME), /* mask */
                     (s_int16) CCOL_FC_EVENTSTATE[i][CIF_GEEL],    /* eventState */
                     (s_int16) 0,                                  /* startTime  */
                     (s_int16) TGL_max[i],                         /* minEndTime */
                     (s_int16) ((TMGL_max[i] > TGL_max[i]) ? (TMGL_max[i] + LATENCY_CORRECTION_MAX_END) : (TGL_max[i] + LATENCY_CORRECTION_MAX_END)), /* maxEndTime */
                     (s_int16) ((TMGL_max[i] > TGL_max[i]) ? TMGL_max[i] : TGL_max[i]), /* likelyTime */
                     (s_int16) NG,                                 /* confidence */
                     (s_int16) NG);                                /* nextTime   */
                  reset_fctiming((mulv) i, (mulv) 1);
               }
            }
            break;

#if (CCOL_V > 100)
         case CIF_STAT_UITSCHAKELEN:  /* Uitschakelen */
            if (CIF_WPS[CIF_PROG_STATUS] != WPS_old) {
               set_fctiming((mulv) i, /* fc */
                  (mulv) 0,                                /* event      */
                  (s_int16) (CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_MINENDTIME | CIF_TIMING_MASK_MAXENDTIME), /* mask */
                  (s_int16) CIF_TIMING_ONBEKEND,           /* eventState */
                  (s_int16) SPAT_TIJD_ONBEKEND,            /* startTime  */
                  (s_int16) NG,                            /* minEndTime */
                  (s_int16) NG,                            /* maxEndTime */
                  (s_int16) NG,                            /* likelyTime */
                  (s_int16) NG,                            /* confidence */
                  (s_int16) NG );                           /* nextTime   */
               reset_fctiming((mulv) i, (mulv) 1);
            }
            break;

         case CIF_STAT_FATALE_FOUT:  /* Fatale fout - knippern of gedoofd */
            if (CIF_WPS[CIF_PROG_STATUS] != WPS_old) {
               set_fctiming((mulv) i, /* fc */
                  (mulv) 0,                                /* event      */
                  (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_MINENDTIME | CIF_TIMING_MASK_MAXENDTIME), /* mask */
                  (s_int16) CIF_TIMING_ONBEKEND,           /* eventState */
                  (s_int16) SPAT_TIJD_ONBEKEND,            /* startTime  */
                  (s_int16) NG,                            /* minEndTime */
                  (s_int16) NG,                            /* maxEndTime */
                  (s_int16) NG,                            /* likelyTime */
                  (s_int16) NG,                            /* confidence */
                  (s_int16) NG);                           /* nextTime   */
               reset_fctiming((mulv) i, (mulv) 1);
            }
            break;
#endif

         default:
            if (CIF_WPS[CIF_PROG_STATUS] != WPS_old) {
               set_fctiming((mulv) i, /* fc */
                  (mulv) 0,                                        /* event      */
                  (s_int16) (CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_MINENDTIME | CIF_TIMING_MASK_MAXENDTIME), /* mask */
                  (s_int16) CIF_TIMING_ONBEKEND,                   /* eventState */
                  (s_int16) SPAT_TIJD_ONBEKEND,                    /* startTime  */
                  (s_int16) NG,                                    /* minEndTime */
                  (s_int16) NG,                                    /* maxEndTime */
                  (s_int16) NG,                                    /* likelyTime */
                  (s_int16) NG,                                    /* confidence */
                  (s_int16) NG);                                  /* nextTime   */
               reset_fctiming((mulv) i, (mulv) 1);
            }
            break;
         }
      }
   }
   WPS_old= CIF_WPS[CIF_PROG_STATUS];
}

