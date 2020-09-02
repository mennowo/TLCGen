#include "extra_func_ris.h"

/* RIS INMELDING SELECTIEF */ /* CCA TvG */
/* ======================= */
/* ris_inmelding_selectief() is gebaseerd op de functie ris_detectie_selectief() uit de Toolkit CCOL. Hieraan is toegevoegd: 
* - Controle of er al een inmelding heeft plaatsgevonden voor het betreffende voertuig in  RIS_PRIOREQUEST_EX_AP[r].prioControlState
* - Het eenmalig zetten van een inmelding in RIS_PRIOREQUEST_EX_AP[r].prioControlState
*/

rif_bool ris_inmelding_selectief(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_bool match_signalgroup)
{
   register count i, j, r = 0;

   while (r < RIS_PRIOREQUEST_AP_NUMBER) {                                                                                                                                                   /* doorloop alle PrioRequest objecten */
      if ( (RIS_PRIOREQUEST_EX_AP[r].prioControlState == RIF_PRIORITIZATIONSTATE_UNKNOWN)                                                                                                    /* nog geen inmelding aanwezig */
         && (strcmp(RIS_PRIOREQUEST_AP[r].signalGroup, FC_code[fc]) == 0) ) {                                                                                                                /* test op juiste signaalgroep */
         if ( (RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_REQUEST) || (RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_UPDATE) ) {
            i = 0;
            while (i < RIS_ITSSTATION_AP_NUMBER) {                                                                                                                                           /* doorloop alle ItsStation objecten */
               if (strcmp(RIS_PRIOREQUEST_AP[r].itsStation, RIS_ITSSTATION_AP[i].id) == 0 ) {                                                                                                /* test op zelfde ItsStation ID */
                  if (stationtype_bits & (1 << RIS_ITSSTATION_AP[i].stationType) ) {                                                                                                         /* test stationType - bit       */
                     for (j = 0; j < RIF_MAXLANES; j++) {
                        if (!strlen(RIS_ITSSTATION_AP[i].matches[j].intersection) || (strlen(intersection) && (strcmp(RIS_ITSSTATION_AP[i].matches[j].intersection, intersection) != 0))) {  /* test intersection ID */
                           break;
                        }
                        if (lane_id == RIS_ITSSTATION_AP[i].matches[j].lane) {                                                                                                               /* test op juiste lane id */
                           if (!match_signalgroup || (strcmp(RIS_ITSSTATION_AP[i].matches[j].signalGroup, FC_code[fc])) == 0) {                                                              /* test op juiste signaalgroep  */
                              if ((RIS_ITSSTATION_EX_AP[i].matches[j].distance > length_start) && (RIS_ITSSTATION_EX_AP[i].matches[j].distance <= length_end)) {                             /* test distance */
                                 RIS_PRIOREQUEST_EX_AP[r].prioControlState = RIF_PRIORITIZATIONSTATE_REQUESTED;                                                                              /* zet inmelding voor ID       */
                                 return ( (rif_bool) TRUE);
                              }
                           }
                        }
                     }
                  }
               }
               i++;
            }
         }
      }
      r++;
   }
   return ( (rif_bool) FALSE);
}


/* RIS UITMELDING SELECTIEF * / /* CCA TvG */
/* ========================= */
/* ris_uitmelding_selectief() geeft als return waarde waar (BIT0), als er voor de opgegeven signaalgroep in de RIS_PRIOREQUEST_AP-buffer een SRM-bericht met een selectieve 
* inmelding aanwezig is en requesttype == RIF_PRIORITYREQUESTTYPE_CANCELLATION heeft. de selectieve inmelding wordt bewaard in de variabele RIS_PRIOREQUEST_EX_AP[r].prioControlState.
* ris_uitmelding_selectief() geeft als return waarde niet waar (FALSE), als er voor de opgegeven signaalgroep in de RIS_PRIOREQUEST_AP-buffer er SRM-berichten met een selectieve 
* inmelding aanwezig zijn, maar geen requesttype == RIF_PRIORITYREQUESTTYPE_CANCELLATION hebben.
* ris_uitmelding_selectief() geeft als return waarde waar (BIT1), als er voor de opgegeven signaalgroep in de RIS_PRIOREQUEST_AP-buffer er geen SRM-berichten met een selectieve 
* inmelding meer aanwezig zijn.
* ris_uitmelding_selectief() kan worden aangeroepen vanuit de functie application().
*
* voorbeeld: IH[hovuit421] = ris_uitmelding_selectief (fc42);
*            IH[hovuit481] = ris_uitmelding_selectief (fc48);
*/

rif_bool ris_uitmelding_selectief(count fc)
{
   register count r = 0;
   rif_bool result = BIT1;

   while (r < RIS_PRIOREQUEST_AP_NUMBER) {                                                        /* doorloop alle PrioRequest objecten */
      if ( (strcmp(RIS_PRIOREQUEST_AP[r].signalGroup, FC_code[fc]) == 0)                          /* test op juiste signaalgroep */
         && (RIS_PRIOREQUEST_EX_AP[r].prioControlState >= RIF_PRIORITIZATIONSTATE_REQUESTED)      /* test op aanwezige selectieve inmelding */
         && (RIS_PRIOREQUEST_EX_AP[r].prioState >= RIF_PRIORITIZATIONSTATE_REQUESTED)             /* test op geldige prioState              */
         && (RIS_PRIOREQUEST_EX_AP[r].prioState <= RIF_PRIORITIZATIONSTATE_GRANTED ) ) {          /* test op aanwezige selectieve inmelding */
         result = FALSE;                                                                          /* voor de signaalgroep is een priorequest met een selectieve inmelding aanwezig met een geldige prioState */
         if (RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_CANCELLATION)  {        /* cancellation request */
            result = BIT0;                                                                        /* BIT0 -> TRUE */
            break;
         }
      }
      r++;
   }
   return ((rif_bool) result);
}


#ifdef RIS_SSM

void ris_verstuur_ssm(int prioFcsrm) {  /* CCA */

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

#endif // RIS_SSM

void ris_ym(int prioFcsrm, count tym, count tym_max) { /* CCA */
/* @@@Moet eigengelijk gebueren op basis van ETA. Nu op basis van rijtijd (op ongeveer 250 meter dus onnauwkeurig). Optie om uit te werken op basis van aanwezigheid voertuig x meter voor de stopstreep met granted?
 * @@@Klopt het BIT?
 */

   int i, prio;
   int fc;
   prio = prioFcsrm;
   fc = iFC_PRIOix[prio];
   YM[fc] &=~BIT8;
   for (i = 0; i < RIS_PRIOREQUEST_AP_NUMBER; i++)
   {
      IT[tym_max] = (iStartGroen[prio]<=T_max[tym]);
      IT[tym]     = (iStartGroen[prio]<=T_max[tym]);
      if (T[tym] && T[tym_max])                       YM[fc] |= BIT8; 
      }
}
