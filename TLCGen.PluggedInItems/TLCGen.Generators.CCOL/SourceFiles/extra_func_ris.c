#include "extra_func_ris.h"

void ris_verstuur_ssm(int prioFcsrm) { /* @@@ CCA */

  /* SSM berichten versturen 
   * 
   * RIF_PRIORITIZATIONSTATE_UNKNOWN           = 0,   Unknown state.
   * RIF_PRIORITIZATIONSTATE_REQUESTED         = 1,   This prioritization request was detected by the traffic controller.
   * RIF_PRIORITIZATIONSTATE_PROCESSING        = 2,   Checking request (request is in queue, other requests are prior).
   * RIF_PRIORITIZATIONSTATE_WATCHOTHERTRAFFIC = 3,   Cannot give full permission, therefore watch for other traffic. Note that other requests may be present.
   * RIF_PRIORITIZATIONSTATE_GRANTED           = 4,   Intervention was successful and now prioritization is active.
   * RIF_PRIORITIZATIONSTATE_REJECTED          = 5,   The prioritization request was rejected by the traffic controller.
   * RIF_PRIORITIZATIONSTATE_MAXPRESENCE       = 6,   The request has exceeded maxPresence time.
   *                                                  Used when the controller has determined that the requester should then back off and request an alternative.
   * RIF_PRIORITIZATIONSTATE_RESERVICELOCKED   = 7    Prior conditions have resulted in a reservice locked event:
   *                                                  the controller requires the passage of time before another similar request will be accepted.
   * 
   * 
   */

   int i, prio;
   int fc;
   prio = prioFcsrm;
   fc = iFC_PRIOix[prio];
   for (i = 0; i < RIS_PRIOREQUEST_AP_NUMBER; i++)
   {
      if (iAantalInmeldingen[prio] > 0) /* prioriteitsvoertuig ingemeld */
      {
         /* prioControlState == 1 --> prioControlState = 7 */ /* TLCgen: blokkeringstijd loopt */
         if  ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 1) && iWachtOpKonflikt[prio] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0)) 
         {
            ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_RESERVICELOCKED);
            RIS_PRIOREQUEST_EX_AP[i].prioControlState = 7;
         } else
            /* prioControlState == 1 --> prioControlState = 5 */ /* TLCgen: ondermaximum */ 
            if  ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 1) && iOnderMaximumVerstreken[prio] && G[fc] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0)) 
            {
               ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
               RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
            } else
               /* prioControlState == 1 --> prioControlState = 5 */ /* TLCgen: geen iPrioriteit */
               if  ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 1) && !iPrioriteit[prio] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0)) 
               {
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
                  RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
               } else
                  /* prioControlState == 1 --> prioControlState = 2 */ 
                  if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 1) && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0)) 
                  {
                     ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_PROCESSING);
                     RIS_PRIOREQUEST_EX_AP[i].prioControlState = 2;
                  }

               /* prioControlState == 2 --> prioControlState = 5 */ /* TLCgen: ondermaximum */
               if  ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 2) && iOnderMaximumVerstreken[prio] && G[fc] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0)) 
               {
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
                  RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
               } else
                  /* prioControlState == 2 --> prioControlState = 5 */ /* TLCgen: geen iPrioriteit */
                  if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 2) && !iPrioriteit[prio] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0))
                  {
                     ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
                     RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
                  } else
                     /* prioControlState == 2 --> prioControlState = 4 */ /* TLCgen: prioriteit en groen */
                     if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 2) && iPrioriteit[prio] && G[fc] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0))
                     {
                        ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_GRANTED);
                        RIS_PRIOREQUEST_EX_AP[i].prioControlState = 4;
                     }

                  /* prioControlState == 4 --> prioControlState = 5 */
                  if  ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 4) && !iPrioriteit[prio] && G[fc] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0)) 
                  {
                     ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
                     RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
                  } 
      }
      else /* geen prioriteitsvoertuig ingemeld */
      {
         /* prioControlState == 2 --> prioControlState = 5 */ /* TLCgen: geen iPrioriteit */
         if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 2) && !iPrioriteit[prio] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0))
         {
            ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
            RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
         } else
            /* prioControlState == 2 --> prioControlState = 6 */ /* TLCgen: groenbewakingstijd actief */
            if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 2) && G[fc] && (iGroenBewakingsTimer[prio] >= iGroenBewakingsTijd[prio]) && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0))
            {
               ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_MAXPRESENCE);
               RIS_PRIOREQUEST_EX_AP[i].prioControlState = 6;
            } else
               /* prioControlState == 4 --> prioControlState = 6 */ /* TLCgen: groenbewakingstijd actief */
               if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 4) && G[fc] && (iGroenBewakingsTimer[prio] >= iGroenBewakingsTijd[prio]) && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0))
               {
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_MAXPRESENCE);
                  RIS_PRIOREQUEST_EX_AP[i].prioControlState = 6; 
               } else
                  /* prioControlState == 4 --> prioControlState = 5 */ /* TLCgen: !iPrioriteit */
                  if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 2) && G[fc] && !iPrioriteit[prio] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0))
                  {
                     ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
                     RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
                  }
      }
   }
}

/* KG */ /*CCA@@@*/
/* kg() tests G for the conflicting phasecycles.
 * kg() returns TRUE if an "G[]" is detected, otherwise FALSE.
 * kg() can be used in the function application().
 */

#if !defined (CCOLFUNC)


boolv kg(count i)
{
   register count n, j;

#ifndef NO_GGCONFLICT
   for (n=0; n<GKFC_MAX[i]; n++) {
#else
   for (n=0; n<KFC_MAX[i]; n++) {
#endif
      j=KF_pointer[i][n];
      if (G[j])  return (TRUE);
   }
   return (FALSE);
   }

#endif

void ris_ym(int prioFcsrm, count tym, count tym_max) { /* @@@ CCA */
/* Moet eigengelijk gebueren op basis van ETA. Nu op basis van rijtijd (op ongeveer 250 meter dus onnauwkeurig). Optie om uit te werken op basis van aanwezigheid voertuig x meter voor de stopstreep met granted?
 */

   int i, prio;
   int fc;
   prio = prioFcsrm;
   fc = iFC_PRIOix[prio];
   for (i = 0; i < RIS_PRIOREQUEST_AP_NUMBER; i++)
   {
      IT[tym_max] = (iStartGroen[prio]<=T_max[tym]);
      IT[tym]     = (iStartGroen[prio]<=T_max[tym]);
      if (T[tym] && T[tym_max])                       YM[fc] |= 0x40;
      else                                            YM[fc] &=~0x40;
      }
}
