/* DEFINITIE FUNCTIE MSG_FCTIMING */
/* ============================== */

/* (C) Copyright 2016-2017 by A.C.M. van Grinsven. All rights reserved. */
/* CCOL:  version 9.0	 */

/* DATUM: 06-12-2017 - aanpassingen voor CCOL versie 9.0 - iVRI*/

/* functie voor het wegschrijven van CIF_FC_TIMING[] events */
/* -------------------------------------------------------- */
/* de functie set_fctiming() schrijft de fc_timing informatie naar de CIF_FC_TIMING buffer.
 * de functie set_fctiming() wordt aangeroepen in de fucntie msg_fctiming().
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
 * de functie reset_fctiming() wordt aangeroepen in de fucntie msg_fctiming().
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

void msg_fctiming(void)
{
   register count i;
   static s_int16 WPS_old= 0;		/* oude programmastatus		*/


   for (i=0; i<FC_MAX; i++) {
      
      switch (CIF_WPS[CIF_PROG_STATUS]) {

          case CIF_STAT_GEDOOFD: /* Gedoofd */
            if (CIF_WPS[CIF_PROG_STATUS] != WPS_old) {
               set_fctiming( (mulv) i, /* fc */
                       (mulv) 0,
                       (s_int16) (CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_MINENDTIME), /* mask */
                       (s_int16) CIF_TIMING_GEDOOFD,            /* eventState */
                       (s_int16) NG,                            /* startTime  */
                       (s_int16) NG,                            /* minEndTime */
                       (s_int16) NG,                            /* maxEndTime */
                       (s_int16) NG,                            /* likelyTime */
                       (s_int16) NG,                            /* confidence */
                       (s_int16) NG);                           /* nextTime   */
               reset_fctiming((mulv) i, (mulv) 1);
            }
            break;

         case CIF_STAT_KP: /* Geel knipperen */
            if (CIF_WPS[CIF_PROG_STATUS] != WPS_old) {
               set_fctiming( (mulv) i, /* fc */
                       (mulv) 0,
                       (s_int16) (CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_MINENDTIME), /* mask */
                       (s_int16) CIF_TIMING_GEEL_KNIPPEREN,     /* eventState */
                       (s_int16) NG,                            /* startTime  */
                       (s_int16) NG,                            /* minEndTime */
                       (s_int16) NG,                            /* maxEndTime */
                       (s_int16) NG,                            /* likelyTime */
                       (s_int16) NG,                            /* confidence */
                       (s_int16) NG);                           /* nextTime   */
               reset_fctiming((mulv) i, (mulv) 1);
            }
            break;

         case CIF_STAT_AR: /* Alles rood */
            if (CIF_WPS[CIF_PROG_STATUS] != WPS_old) {
               set_fctiming( (mulv) i, /* fc */
                       (mulv) 0,
                       (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_MINENDTIME), /* mask */
                       (s_int16) CIF_TIMING_ROOD,               /* eventState */
                       (s_int16) NG,                            /* startTime  */
                       (s_int16) NG,                            /* minEndTime */
                       (s_int16) NG,                            /* maxEndTime */
                       (s_int16) NG,                            /* likelyTime */
                       (s_int16) NG,                            /* confidence */
                       (s_int16) NG);                           /* nextTime   */
               reset_fctiming((mulv) i, (mulv) 1);
            }
            break;
         case CIF_STAT_REG:   /* Regelen */
            if (CIF_WPS[CIF_PROG_STATUS] == WPS_old) {
               if (SR[i]) {
                   set_fctiming( (mulv) i, /* fc */
                           (mulv) 0,
                           (s_int16) (CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_STARTTIME | CIF_TIMING_MASK_MINENDTIME  | CIF_TIMING_MASK_MAXENDTIME), /* mask */
                           (s_int16) CCOL_FC_EVENTSTATE[i][CIF_ROOD],   /* eventState */
                           (s_int16) 0,                                 /* startTime  */
                           (s_int16) TRG_max[i],                        /* minEndTime */
                           (s_int16) NG,                                /* maxEndTime */
                           (s_int16) NG,                                /* likelyTime */
                           (s_int16) NG,                                /* confidence */
                           (s_int16) NG);                               /* nextTime   */
                  reset_fctiming((mulv) i, (mulv) 1);
               }
               else if (SG[i]) {
                  set_fctiming( (mulv) i, /* fc */
                          (mulv) 0,
                          (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_STARTTIME | CIF_TIMING_MASK_MINENDTIME  | CIF_TIMING_MASK_MAXENDTIME), /* mask */
                          (s_int16) CCOL_FC_EVENTSTATE[i][CIF_GROEN],   /* eventState */
                          (s_int16) 0,                                  /* startTime  */
                          (s_int16) TGG_max[i],                         /* minEndTime */
                          (s_int16) NG,                                 /* maxEndTime */
                          (s_int16) NG,                                 /* likelyTime */
                          (s_int16) NG,                                 /* confidence */
                          (s_int16) NG);                                /* nextTime   */
                  reset_fctiming((mulv) i, (mulv) 1);
               }
               else if (SGL[i]) {
                  set_fctiming( (mulv) i, /* fc */
                          (mulv) 0,
                          (s_int16) (CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_STARTTIME | CIF_TIMING_MASK_MINENDTIME), /* mask */
                          (s_int16) CCOL_FC_EVENTSTATE[i][CIF_GEEL],    /* eventState */
                          (s_int16) 0,                                  /* startTime  */
                          (s_int16) TGL_max[i],                         /* minEndTime */
                          (s_int16) NG,                                 /* maxEndTime */
                          (s_int16) NG,                                 /* likelyTime */
                          (s_int16) NG,                                 /* confidence */
                          (s_int16) NG);                                /* nextTime   */
                  set_fctiming( (mulv) i, /* fc */
                          (mulv) 1,
                          (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_STARTTIME  | CIF_TIMING_MASK_MINENDTIME), /* mask */
                          (s_int16) CCOL_FC_EVENTSTATE[i][CIF_ROOD],    /* eventState */
                          (s_int16) TGL_max[i],                         /* startTime  */
                          (s_int16) NG,                                 /* minEndTime */
                          (s_int16) NG,                                 /* maxEndTime */
                          (s_int16) NG,                                 /* likelyTime */
                          (s_int16) NG,                                 /* confidence */
                          (s_int16) NG);                                /* nextTime   */
                  reset_fctiming((mulv) i, (mulv) 2);
               }
            }
            break;

         default: 
            if (CIF_WPS[CIF_PROG_STATUS] != WPS_old) {
               set_fctiming( (mulv) i, /* fc */
                       (mulv) 0,
                       (s_int16)(CIF_TIMING_MASK_EVENTSTATE | CIF_TIMING_MASK_MINENDTIME), /* mask */
                       (s_int16) CIF_TIMING_ONBEKEND,                   /* eventState */
                       (s_int16) NG,                                    /* startTime  */
                       (s_int16) NG,                                    /* minEndTime */
                       (s_int16) NG,                                    /* maxEndTime */
                       (s_int16) NG,                                    /* likelyTime */
                       (s_int16) NG,                                    /* confidence */
                       (s_int16) NG);                                   /* nextTime   */
               reset_fctiming((mulv) i, (mulv) 1);
            }
            break;
      }
   }
   WPS_old= CIF_WPS[CIF_PROG_STATUS];
}