/* EXTRA_FUNC_RIS.C */
/* ================ */

/* CCOL :  versie 11.0     */
/* FILE :  extra_func.c    */
/* DATUM:  01-10-2020      */


/* #define TEST_DISPLAY_SRM_PUT_SIGNALGROUP  /* voor het tonen van de toegevoegde signalGroup in de testomgeving */


/* include files */
/* ============= */
   #include "extra_func_ris.h"
   #include "fcvar.h"      /* declaratie G[]                */
   #include "prio.h"       /* prioriteitsmodule             */
#ifdef TEST_DISPLAY_SRM_PUT_SIGNALGROUP
  #include "risdplfunc.h"     /* declaratie display_priorequest() */
#endif

   #include <stdlib.h>     /* declaratie atoi()             */
   #include <stdio.h>      /* declaratie snprintf()         */
   #include <string.h>     /* declaratie strlen()           */

#if (defined(_MSC_VER) && (_MSC_VER < 1900))
   #define snprintf sprintf_s
#endif



/* RIS INMELDING SELECTIEF */ 
/* ======================= */
/* ris_inmelding_selectief() is gebaseerd op de functie ris_detectie_selectief() uit de Toolkit CCOL. 
 * Hieraan is toegevoegd: 
 * - Controle of er al een inmelding heeft plaatsgevonden voor het betreffende voertuig (staat in RIS_PRIOREQUEST_EX_AP[r].prioControlState).
 * - het eenmalig zetten van een inmelding in RIS_PRIOREQUEST_EX_AP[r].prioControlState
 */

rif_bool ris_inmelding_selectief(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_int role, rif_int subrole, rif_int priotypefc_id) 
{
   register count i, j, r = 0;

   while (r < RIS_PRIOREQUEST_AP_NUMBER) {                                                                                                                                                   /* doorloop alle PrioRequest objecten */
      if ( (RIS_PRIOREQUEST_EX_AP[r].prioControlState == RIF_PRIORITIZATIONSTATE_UNKNOWN)                                                                                                    /* nog geen inmelding aanwezig */
         && (strcmp(RIS_PRIOREQUEST_AP[r].signalGroup, FC_code[fc]) == 0)           /* test op juiste signalGroup */
         && ( (RIS_PRIOREQUEST_AP[r].role == role) || (role <= 0) )                 /* test op juiste role,        */
         && ( (RIS_PRIOREQUEST_AP[r].subrole == subrole) || (subrole <= 0) )  ) {   /* test op juiste subrole     */

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
                           if ((RIS_ITSSTATION_EX_AP[i].matches[j].distance > length_start) && (RIS_ITSSTATION_EX_AP[i].matches[j].distance <= length_end)) {                             /* test distance */
                              RIS_PRIOREQUEST_EX_AP[r].prioControlState = priotypefc_id+1;  /* 1 hoger om ook de index 0 te ondersteunen */
                              return ( (rif_bool) TRUE);
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



/* RIS INMELDING SELECTIEF_APPROACH */ 
/* ================================ */
/* ris_inmelding_selectie_approach() is gebaseerd op de functie ris_detectie_selectief() uit de Toolkit CCOL.
 * Hieraan is toegevoegd: 
 * - functie test op aproach_id i plaatst van op signalgroup_id (fc).
 * - Controle of er al een inmelding heeft plaatsgevonden voor het betreffende voertuig (staat in RIS_PRIOREQUEST_EX_AP[r].prioControlState)
 * - Het eenmalig zetten van een inmelding in RIS_PRIOREQUEST_EX_AP[r].prioControlState
 */

rif_bool ris_inmelding_selectief_approach(rif_int approach_id, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, enum Rif_VehicleRole role, enum Rif_VehicleSubRole subrole, rif_int priotypefc_id) 
{
   register count i, j, r = 0;

   while (r < RIS_PRIOREQUEST_AP_NUMBER) {                                                                                                                                                   /* doorloop alle PrioRequest objecten */
      if ( (RIS_PRIOREQUEST_EX_AP[r].prioControlState == RIF_PRIORITIZATIONSTATE_UNKNOWN)                                                                                                    /* nog geen inmelding aanwezig */
         && (RIS_PRIOREQUEST_AP[r].approach == approach_id)                         /* test op juiste approach */
         && ( (RIS_PRIOREQUEST_AP[r].role == role) || (role <= 0) )                 /* test op juiste role        */
         && ( (RIS_PRIOREQUEST_AP[r].subrole == subrole) || (subrole <= 0) )  ) {   /* test op juiste subrole     */

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
                           if ((RIS_ITSSTATION_EX_AP[i].matches[j].distance > length_start) && (RIS_ITSSTATION_EX_AP[i].matches[j].distance <= length_end)) {                             /* test distance */
                              RIS_PRIOREQUEST_EX_AP[r].prioControlState = priotypefc_id+1;  /* 1 hoger om ook de index 0 te ondersteunen */
                              return ( (rif_bool) TRUE);
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




/* RIS UITMELDING SELECTIEF  */
/* ========================= */
/* ris_uitmelding_selectief() geeft als return waarde waar (BIT0), als er voor de opgegeven priotypefc_id in de RIS_PRIOREQUEST_AP-buffer een SRM-bericht met een selectieve inmelding
 * aanwezig is en requesttype == RIF_PRIORITYREQUESTTYPE_CANCELLATION heeft. de selectieve inmelding wordt bewaard in de variabele RIS_PRIOREQUEST_EX_AP[r].prioControlState.
* ris_uitmelding_selectief() geeft als return waarde niet waar (FALSE), als er voor de opgegeven priotypefc_id in de RIS_PRIOREQUEST_AP-buffer er SRM-berichten met een selectieve 
* inmelding aanwezig zijn, maar geen requesttype == RIF_PRIORITYREQUESTTYPE_CANCELLATION hebben.
* ris_uitmelding_selectief() geeft als return waarde waar (BIT1), als er voor de opgegeven signaalgroep in de RIS_PRIOREQUEST_AP-buffer er geen SRM-berichten met een selectieve 
* inmelding meer aanwezig zijn.
* ris_uitmelding_selectief() kan worden aangeroepen vanuit de functie application().
*
* voorbeeld: IH[hovuit421] = ris_uitmelding_selectief (prioFC45ris);
*            IH[hovuit481] = ris_uitmelding_selectief (prioFC48ris);
*/


rif_bool ris_uitmelding_selectief(rif_int priotypefc_id)
{
   register count r = 0;
   rif_bool result = FALSE;

//   if (iAantalInmeldingen[priotypefc_id] > 0) {
//      result = BIT1;                                                                               /* geen SRM melding; BIT1 -> TRUE */
      while (r < RIS_PRIOREQUEST_AP_NUMBER) {                                                      /* doorloop alle PrioRequest objecten */
         if ((RIS_PRIOREQUEST_EX_AP[r].prioControlState == (priotypefc_id + 1))                    /* test op juiste prioControleState   */
            && (RIS_PRIOREQUEST_EX_AP[r].prioState >= RIF_PRIORITIZATIONSTATE_REQUESTED)           /* test op geldige prioState          */
            && (RIS_PRIOREQUEST_EX_AP[r].prioState <= RIF_PRIORITIZATIONSTATE_MAXPRESENCE)) {
            result = FALSE;                                                                        /* voor de signaalgroep is een priorequest met een selectieve inmelding aanwezig met een geldige prioState */
            if (RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_CANCELLATION) {       /* cancellation request */
               result = BIT0;                                                                      /* BIT0 -> TRUE */
               break;
            }
         }
         r++;
      }
//   }
   return ((rif_bool)result);
}



/* RIS VERSTUUR SSM */
/* ================ */
/* ris_verstuur_ssm() verstuurt de SSM berichten conform UC3 op basis van een SRM beriCht. 
 * Daarnaast verzorgt de functie in:  
 * - instellen van de prioriteitsopties op basis van importance
 * ris_verstuur_ssm() kan vanuit de regelapplicatie worden aangeroepen.
 *
 */
 
/* De volgende SSM berichten worden verstuurd middels deze functies 
 * 
 * RIF_PRIORITIZATIONSTATE_PROCESSING        = 2,   Checking request (request is in queue, other requests are prior).
 * RIF_PRIORITIZATIONSTATE_WATCHOTHERTRAFFIC = 3,   Cannot give full permission, therefore watch for other traffic. Note that other requests may be present. 
 * RIF_PRIORITIZATIONSTATE_GRANTED           = 4,   Intervention was successful and now prioritization is active.
 * RIF_PRIORITIZATIONSTATE_REJECTED          = 5,   The prioritization request was rejected by the traffic controller.
 * RIF_PRIORITIZATIONSTATE_MAXPRESENCE       = 6,   The request has exceeded maxPresence time.
 *                                                  Used when the controller has determined that the requester should then back off and request an alternative.
 */

void ris_verstuur_ssm(rif_int priotypefc_id) {

   register count i = 0;
   count fc;

   fc = iFC_PRIOix[priotypefc_id];

   while (i < RIS_PRIOREQUEST_AP_NUMBER)
   {
      if (RIS_PRIOREQUEST_EX_AP[i].prioControlState == (priotypefc_id+1) )  { /* 1 hoger om ook de index 0 te ondersteunen */

         switch (RIS_PRIOREQUEST_EX_AP[i].prioState) {

            case RIF_PRIORITIZATIONSTATE_UNKNOWN:
               break;          

            case RIF_PRIORITIZATIONSTATE_REQUESTED:

               /* Correctie Prioriteisniveau */
               /* -------------------------- */
               if ( (RIS_PRIOREQUEST_AP[i].importance >  0) && (RIS_PRIOREQUEST_AP[i].importance <= 10) ) {       /* geconditoneerde prioriteit */
                  if (iInstPrioriteitsOpties[priotypefc_id] >= 13) {
                     iInstPrioriteitsOpties[priotypefc_id] =13;      /* 13 = poAanvraag(1)+                                poGroenVastHouden(4)+poBijzonderRealiseren(8) */
                  }
               } 
               else if ((RIS_PRIOREQUEST_AP[i].importance > 10) && (RIS_PRIOREQUEST_AP[i].importance <= 14) ) {   /* absolute prioriteit */
                  if (iInstPrioriteitsOpties[priotypefc_id] >= 15) {
                     /* prioriteitsniveau niet aanpassen */
                  }
               }
               else if (RIS_PRIOREQUEST_AP[i].importance ==  -1)  {  /* geen prioriteitsniveau in het bericht */
                      /* prioriteitsniveau niet aanpassen */
               }
               else if ((RIS_PRIOREQUEST_AP[i].importance <  -1) || (RIS_PRIOREQUEST_AP[i].importance >= 15) ) {   /* out of range -> geen prioriteit */
                  iInstPrioriteitsOpties[priotypefc_id] = 0;
               }

               /* Normal Flow - [requested] -> processing */
               /* --------------------------------------- */
               if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState >  RIF_PRIORITIZATIONSTATE_UNKNOWN)   /* 1 */
                  && (iInstPrioriteitsOpties[priotypefc_id]>0)  ) { /* 1 */ 
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_PROCESSING); /* 2 */
               }
               /* Exception #3 - [requested] -> rejected */
               /* -------------------------------------- */
               else if ( iInstPrioriteitsOpties[priotypefc_id]<=0 ) {  /* 1 */   
                   ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED); /* 5 */
               }
               break;

            case RIF_PRIORITIZATIONSTATE_PROCESSING:   
               /* Normal Flow - [processing] -> granted */
               /* ------------------------------------- */
               if (G[fc] && !iOnderMaximumVerstreken[priotypefc_id])  {
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_GRANTED); /* 4 */ /*@@@@@ nog iets voor opnemen in de prio.c dat granted niet wordt afgekapt */
               }
               break;

            case RIF_PRIORITIZATIONSTATE_WATCHOTHERTRAFFIC:
               /* @@@@@ mogelijk nog invullen voor hulpdiensten */
               break;  

             case RIF_PRIORITIZATIONSTATE_GRANTED:
               /* Exception #8 - [granted] -> maxPresence -> end */
               /* ---------------------------------------------- */
            if (G[fc] && (iGroenBewakingsTimer[priotypefc_id] >= iGroenBewakingsTijd[priotypefc_id]) ) {
               ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_MAXPRESENCE); /* 6 */
            }
            break;

            case RIF_PRIORITIZATIONSTATE_REJECTED:
            case RIF_PRIORITIZATIONSTATE_MAXPRESENCE:
            case RIF_PRIORITIZATIONSTATE_RESERVICELOCKED:
               /* wacht op een SRM-cancellation bericht van de ItsStation */
               break;  
               
            default: 
               break;
         }
      }
      i++;
   }
}


/* RIS_SRM_PUT_SIGNALGROUP_PUBLICTRANSPORT() */
/* ========================================= */
/* ris_srm_put_signalgroup_publictransport() plaatst de fasecycluscodering (FC_code[fc]) in het veld signalGroup van het ontvangen SRM-bericht op basis van 
 * de approach (approachID) en de lijnnummers, indien de fasecycluscodering (nog) niet aanwezig is.
 * de lijnnummers bevinden zich in de parameterlijst (PRM) in opvolgende lijn-parameters, met als eerste lijn-parameter een waarde voor alle lijnnummers.
 * aan de functie dienen de fasecyclusindex (fc), de approach (approachID), de index van de eerste lijn-parameter (prm_line_first) en
 * het aantal lijn-parameters (prm_line_max) als argument te worden opgegeven.
 * ris_srm_put_signalgroup_publictransport() kan vanuit de regelapplicatie worden aangeroepen.
 *
 * voorbeelden
 * -----------
 * #define approachid45  2
 * #define approachid51  4
 *
 * voorbeeld: ris_srm_put_signalgroup_publictransport(fc45, approachid45, prmlyn451, 5); // openbaar vervoer op approach 2 voor fasecyclus 45 (FC_code[fc45]= "45")
 *                                                                                       // lijnnummers in PRM[prmlijn451] t/m PRM[prmlijn455] (opvolgende PRM's)
 * voorbeeld: ris_srm_put_signalgroup_publictransport(fc51, approachid51, prmlyn511, 5); // openbaar vervoer op approach 4 voor fasecyclus 51 (FC_code[fc51]= "51")
 *                                                                                       // lijnnummers in PRM[prmlyn511] t/m PRM[prmlyn515] (opvolgende PRM's) 
 */

void ris_srm_put_signalgroup_publictransport(count fc, rif_int approach_id, count prm_line_first, count prm_line_max)
{
   register count i, r = 0;

   if (RIS_NEW_PRIOREQUEST_AP_NUMBER) {   /* test of er een SRM-bericht is ontvangen */
   
      while (r < RIS_PRIOREQUEST_AP_NUMBER)
      {
         if ( !strlen(RIS_PRIOREQUEST_AP[r].signalGroup) )  {    /* test op afwezigheid fasecyclus codering */

            if ( (RIS_PRIOREQUEST_AP[r].approach == approach_id) &&                    /* test approach_id  */
                 (RIS_PRIOREQUEST_AP[r].role == RIF_VEHICLEROLE_PUBLICTRANSPORT) &&    /* test SRM-role     */            
                 (strlen(RIS_PRIOREQUEST_AP[r].routeName)) ) {                         /* test lijnnummer aanwezig */ 

               if (PRM[prm_line_first] == 999) {                    /* test parameter - alle lijnnummers */
                  /* plaats signalGroup in het SRM bericht */
                  /* ------------------------------------- */
                  snprintf(RIS_PRIOREQUEST_AP[r].signalGroup, RIF_STRINGLENGTH, "%s", FC_code[fc]); 

#ifdef TEST_DISPLAY_SRM_PUT_SIGNALGROUP
                  /* display SRM-bericht */
                  /* ------------------- */
                  ris_display_priorequest(&RIS_PRIOREQUEST_AP[r], &RIS_PRIOREQUEST_def, 0, 9);
#endif
               }
               else {
                  for (i = 0; i < prm_line_max; i++) {           /* test parameters - lijnnummers */
                     if ( ((mulv) (atoi(RIS_PRIOREQUEST_AP[r].routeName))) == PRM[prm_line_first + i] )  { /* vergelijk lijnnummer */

                        /* plaats signalGroup in het SRM bericht */
                        /* ------------------------------------- */
                        snprintf(RIS_PRIOREQUEST_AP[r].signalGroup, RIF_STRINGLENGTH, "%s", FC_code[fc]);

#ifdef TEST_DISPLAY_SRM_PUT_SIGNALGROUP
                        /* display SRM-bericht */
                        /* ------------------- */
                        ris_display_priorequest(&RIS_PRIOREQUEST_AP[r], &RIS_PRIOREQUEST_def, 0, 8);
#endif
                        break;
                     }
                  }
               }
            }
         }
         r++;
      }
   }
}

