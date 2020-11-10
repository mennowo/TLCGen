/* RISPRIOAPPL.H */
/* ============= */

/* CCOL :  versie 11.0        */
/* FILE :  extra_func_ris.c   */
/* DATUM:  30-10-2020         */

/* Deze ris prioriteit applicatiefuncties worden gebruikt in de programmagenerator TLCGEN in combinatie met de PrioModule van TLCGen */

/* include files */
/* ============= */
   #include "extra_func_ris.h"   /* declaratie risprio-functies   */
   #include "fcvar.h"            /* declaratie G[], FC_MAX        */
   #include "prio.h"             /* prioriteitsmodule             */
   #include <stdlib.h>           /* declaratie atoi()             */
   #include <stdio.h>            /* declaratie snprintf()         */
   #include <string.h>           /* declaratie strlen()           */

#if (defined(_MSC_VER) && (_MSC_VER < 1900))
   #define snprintf sprintf_s
#endif

/* RIS INMELDING SELECTIEF */ 
/* ======================= */
/* ris_inmelding_selectief() is gebaseerd op de functie ris_detectie_selectief() uit de Toolkit CCOL. 
 * hieraan is toegevoegd:
 * - controle op approach
 * - controle op de juiste role en subrole.
 * - controle op de juiste eta
 * - controle of er al een inmelding heeft plaatsgevonden voor het betreffende voertuig (inmelding staat in RIS_PRIOREQUEST_EX_AP[r].prioControlState).
 * - het eenmalig zetten van een inmelding in RIS_PRIOREQUEST_EX_AP[r].prioControlState=priotypefc_id.
 *
 * ris_inmelding_selectief() verzorgt een selectieve inmelding op een ontvangen prioriteitsverzoek (SRM-informatie in de RIS_ITSSTATION_AP-buffer) op basis van
 * de eta (Estimated Time to Arrival) of op basis van de positie van de ItsStation op het kruispunt (CAM-informatie in de RIS_ITSSTATION_AP-buffer).
 * ris_inmelding_selectief() controleert het SRM-bericht op de juiste signalGroup of approach en op de juiste role en subrole, en controleert het CAM-bericht op
 * de juiste stationtype. bij een selectieve inmelding geeft ris_inmelding_selectief() de return-waarde waar (TRUE) en wordt de identificatie van het prioriteitstype
 * van de fasecyclus (priotypefc_id) in RIS_PRIOREQUEST_EX_AP[r].prioControlState geplaatst; RIS_PRIOREQUEST_EX_AP[r].prioControlState = priotypefc_id.
 * bij geen inmelding geeft ris_inmelding_selectief() de return-waarde niet waar (FALSE).
 * length_start ligt dichter bij de stopstreep dan length_end. 
 * bij aanroep van de functie dienen de fasecyclusindex (fc), approach_id, intersection, lane_id, stationtype (stationtype_bits), length_start, length_end, 
 * role (role_bits), subrole (subrole_bits), eta_prm, en priotypefc_id als argument te worden meegegeven. stationtype, role en subrole worden bitgewijs gebruikt
 * (definitie in risappl.h). indien de waarde van statiotype_bits role_bits, subrole_bits of lane_id kleiner gelijk of gelijk is aan nul (0) wordt de controle
 * op de betreffende variabele buiten beschouwing gelaten.
 * ris_inmelding_selectief() kan worden aangeroepen vanuit de functie application().
 *
 * voorbeelden
 * -----------
 * IH[hinov421] = ris_inmelding_selectief (fc42,  NG,         "", ris_lane421, RIS_BUS | RIS_TRAM, 20, 100, RIS_ROLE_PUBLICTRANSPORT, NG, PRM[prmeta42ov], prio_ov42ris);  // geen test op intersection en subrole
 * IH[hinov481] = ris_inmelding_selectief (fc48,  NG, SYSTEM_ITF, ris_lane481, RIS_BUS | RIS_TRAM, 20, 100, RIS_ROLE_PUBLICTRANSPORT, NG,              NG, prio_ov48ris);  // geen test op eta en subrole
 *
 * IH[hinhd421] = ris_inmelding_selectief (NG, ris_approachid42 SYSTEM_ITF           NG, RIS_HULPDIENST, 20, 300, RIS_ROLE_EMERGENCY, RIS_SUBROLE_EMERGENCY, NG, prio_hd42ris);  // geen test op fc, lane en eta
 * IH[hinhd481] = ris_inmelding_selectief (NG, ris_approachid48,SYSTEM_ITF, ris_lane481, RIS_HULPDIENST, 20, 300, RIS_ROLE_EMERGENCY, RIS_SUBROLE_EMERGENCY, NG, prio_hdv48ris); // geen test op fc en eta
 * 
 */
rif_bool ris_inmelding_selectief(count fc, rif_int approach_id, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_int role_bits, rif_int subrole_bits, rif_int eta_prm, rif_int priotypefc_id)
{  
   register rif_int i, j, r = 0;
   rif_bool correct;


   while (r < RIS_PRIOREQUEST_AP_NUMBER) {   /* doorloop alle PrioRequest objecten */

      if ( (RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_REQUEST) || (RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_UPDATE) ) {

         /* zet correct op FALSE */
         /* -------------------- */
         correct= FALSE; 

         /* test op juiste signalgroup en approach */
         /* -------------------------------------- */
         if ( ( (fc >= 0 )  && ( fc < FC_MAX ) ) && (strcmp(RIS_PRIOREQUEST_AP[r].signalGroup, FC_code[fc]) == 0) ) {   /* test op juiste signalgroup - voor openbaar vervoer */
            correct= TRUE;    /* signalGroup is correct  */
         }
         else if  (RIS_PRIOREQUEST_AP[r].approach == approach_id) {           /* test op juiste approach - voor hulpdiensten */
            correct= TRUE;    /* approach is correct */
         }
         else {
             /*  correct= FALSE; */ /* incorrecte signalGroup en incorrecte  approach -> volgende PrioRequest */
         }

         /* test op inmelding */
         /* ----------------- */
         if ( correct && (RIS_PRIOREQUEST_EX_AP[r].prioControlState == NG )    /* nog geen inmelding aanwezig  */
            && ( (role_bits & (1 << (RIS_PRIOREQUEST_AP[r].role & 0xf)) ) || (role_bits <= 0) )               /* test op juiste role - bit    */
            && ( (subrole_bits & (1 << (RIS_PRIOREQUEST_AP[r].subrole & 0xf)) ) || (subrole_bits <= 0) ) ) {  /* test op juiste subrole - bit */

            /* test selectieve inmelding op basis van eta */
            /* ------------------------------------------ */
            if ( eta_prm > 0 ) {                                                                      /* test op ingestelde eta_prm. (eta_prm <= 0) is uitgeschakeld */
               if ( (RIS_PRIOREQUEST_AP[r].eta > RIF_UTC_TIME_PB)                                     /* test of eta in de toekomst ligt */
                  && ( (RIS_PRIOREQUEST_AP[r].eta - RIF_UTC_TIME_PB) < (eta_prm * 1000) ) ) {         /* test op juiste eta_prm */
                  i = 0;
                  while (i < RIS_ITSSTATION_AP_NUMBER) {                                              /* doorloop alle ItsStation objecten */
                     if (strcmp(RIS_PRIOREQUEST_AP[r].itsStation, RIS_ITSSTATION_AP[i].id) == 0 ) {   /* test op zelfde ItsStation ID */
                        if ( (stationtype_bits & (1 << (RIS_ITSSTATION_AP[i].stationType & 0xf)) ) || (stationtype_bits <= 0) ) {  /* test stationType - bit */
                           RIS_PRIOREQUEST_EX_AP[r].prioControlState = priotypefc_id;
                           return ( (rif_bool) TRUE);
                        }
                     }
                     i++;
                  }
               }
            }

            /* test selectieve inmelding op basis van map (locatie) */
            /* ---------------------------------------------------- */
            i = 0;
            while (i < RIS_ITSSTATION_AP_NUMBER) {                                                 /* doorloop alle ItsStation objecten */
               if (strcmp(RIS_PRIOREQUEST_AP[r].itsStation, RIS_ITSSTATION_AP[i].id) == 0 ) {      /* test op zelfde ItsStation ID */
                  if ( (stationtype_bits & (1 << (RIS_ITSSTATION_AP[i].stationType & 0xf) ) ) || (stationtype_bits <= 0) ) {  /* test stationType - bit */
                     for (j = 0; j < RIF_MAXLANES; j++) {                                          /* doorloop alle gematchte lanes */     
                        if (!strlen(RIS_ITSSTATION_AP[i].matches[j].intersection) || (strlen(intersection) && (strcmp(RIS_ITSSTATION_AP[i].matches[j].intersection, intersection) != 0))) {  /* test intersection ID */
                           break;
                        }
                        if ((lane_id == RIS_ITSSTATION_AP[i].matches[j].lane)                      /* test op juiste lane id  */
                           || (lane_id <= 0) /* && (RIS_PRIOREQUEST_AP[r].approach==approach_id) */ ) {  /* test op alle lanes van de approach */
                           if ((RIS_ITSSTATION_EX_AP[i].matches[j].distance > length_start) && (RIS_ITSSTATION_EX_AP[i].matches[j].distance <= length_end)) {  /* test distance */
                              RIS_PRIOREQUEST_EX_AP[r].prioControlState = priotypefc_id;
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
/* ris_uitmelding_selectief() geeft als return-waarde het aantal uitmeldingen (number) voor het prioriteitstype van de fasecyclus (priotype_id);
 * dat is aantal SRM-berichten voor priotypefc_id met requesttype == RIF_PRIORITYREQUESTTYPE_CANCELLATION in de RIS_PRIOREQUEST_AP-buffer.
 * priotypefc_id is opgeslagen in het uitgebreide SRM-bericht in de variabele RIS_PRIOREQUEST_EX_AP[r].prioControlState.
 * bij aanroep van de functie dient het prioriteitstype van de fasecyclus (priotypefc_id) als argument te worden meegegeven.
 * ris_uitmelding_selectief() kan worden aangeroepen vanuit de functie application().
 *
 * voorbeelden
 * -----------
 * MM[movuit421] = ris_uitmelding_selectief (prio_ov42ris);
 * MM[movuit481] = ris_uitmelding_selectief (prio_ov48ris);
 *
 */

rif_int ris_uitmelding_selectief(rif_int priotypefc_id)
{
   register rif_int r = 0;
   rif_int number = 0;     /* aantal uitmeldingen */

   while (r < RIS_PRIOREQUEST_AP_NUMBER) {                                                /* doorloop alle PrioRequest objecten */
      if (RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_CANCELLATION)  {   /* cancellation request */
         if ( (RIS_PRIOREQUEST_EX_AP[r].prioControlState == (priotypefc_id) )  ) {      /* test op juiste prioControleState */
            number++;      /* er is een uitmelding - verhoog number */
         }
      }
      r++;
   }
   return (number);
}




/* RIS_SRM_PUT_SIGNALGROUP() */
/* ========================= */
/* ris_srm_put_signalgroup() plaatst de fasecycluscodering (FC_code[fc]) in het veld signalGroup van het ontvangen SRM-bericht op basis van de approach (approach-id),
 * de role (role_bits), de subrole (role_bits) en het lijnnummer (routeName), indien de fasecycluscodering (nog) niet aanwezig is.
 * ris_srm_put_signalgroup() geeft als return-waarde het aantal aangepaste SRM-berichten (number).
 * de lijnnummers bevinden zich in de parameterlijst (PRM) in opvolgende lijn-parameters met als eerste lijn-parameter een parameter die voor testdoeleinden
 * ook kan worden gebruikt voor alle lijnnummers (waarde 999).
 * bij aanroep van de functie dienen de fasecyclusindex (fc), de approach (approach_id), de role (role_bits), de subrole (subrole_bits), de index van de eerste
 * lijn-parameter (prm_line_first) en het aantal gebruikte lijn-parameters (prm_line_max) als argument te worden opgegeven.
 * indien de waarde van role_bits of subrole_bits kleiner of gelijk is aan nul (0) wordt de controle op het betreffende veld buiten beschouwing gelaten.
 * ris_srm_put_signalgroup() kan vanuit de regelapplicatie worden te aangeroepen.
 *
 * voorbeelden
 * -----------
 * #define approachid45  2
 * #define approachid51  4
 *
 * openbaar vervoer (bus) op approach 2 voor fasecyclus 45 (fc45) - lijnnummers in PRM[prmlijn451] t/m PRM[prmlijn455] (5 opvolgende PRM's)
 * ----------------------------------------------------------------------------------------------------------------------------------------
 * ris_srm_put_signalgroup(fc45, approachid45, RIS_ROLE_PUBLICTRANSPORT, RIS_SUBROLE_BUS, prmlijn451, 5);
 *                                                                                         
 * openbaar vervoer op approach 4 voor fasecyclus 51 (fc51) - lijnnummers in PRM[prmlijn511] t/m PRM[prmlijn515] (5 opvolgende PRM's)
 * ----------------------------------------------------------------------------------------------------------------------------------
 * ris_srm_put_signalgroup(fc51, approachid51, RIS_ROLE_PUBLICTRANSPORT, NG, prmlijn511, 5);    // geen test op subrole
 *
 */

rif_int ris_srm_put_signalgroup(count fc, rif_int approach_id, rif_int role_bits, rif_int subrole_bits, count prm_line_first, count prm_line_max)
{
   register count   i;
   register rif_int r = 0;
   rif_int number = 0;     /* aantal aangepaste berichten */

   #define RIS_PRM_ALL_LINENUMBERS  999   /* parameter instelling voor testdoeleiden - alle lijnnummers zijn goed */


   if ( (fc >= 0) && (fc < FC_MAX) ) {    /* test op juiste fasecyclus index */

      if (RIS_NEW_PRIOREQUEST_AP_NUMBER) {   /* test of er een SRM-bericht is ontvangen */
   
         while (r < RIS_PRIOREQUEST_AP_NUMBER)  /* doorloop alle PrioRequest objecten */
         {
            if ( !strlen(RIS_PRIOREQUEST_AP[r].signalGroup) )  {    /* test op afwezigheid fasecyclus codering */

               if ( (RIS_PRIOREQUEST_AP[r].approach == approach_id)                                                   /* test juiste approach_id  */
                    && ( (role_bits & (1 << (RIS_PRIOREQUEST_AP[r].role & 0xf)) ) || (role_bits <= 0) )               /* test op juiste role - bit */
                    && ( (subrole_bits & (1 << (RIS_PRIOREQUEST_AP[r].subrole & 0xf)) ) || (subrole_bits <= 0) ) ) {  /* test op juiste subrole - bit */   

                  if ( (prm_line_first <= 0) || (prm_line_max <= 0) ) {  
                  
                     /* geen lijn-parameters opgegeven - don't care */
                     /* ------------------------------------------- */

                     /* plaats signalGroup in het SRM bericht */
                     /* ------------------------------------- */
                     snprintf(RIS_PRIOREQUEST_AP[r].signalGroup, RIF_STRINGLENGTH, "%s", FC_code[fc]);
                     number++;      /* SRM-bericht is aangepast - verhoog number */

                  }
                  else if ( (prm_line_first + prm_line_max) < PRM_MAX ) {  /* test parameter range */

                     /* test eerste lijnnumer parameter op de instelling - alle lijnnumers */
                     /* ------------------------------------------------------------------ */
                     if (PRM[prm_line_first] == RIS_PRM_ALL_LINENUMBERS) {    /* test parameter alle lijnnummers */

                        /* plaats signalGroup in het SRM bericht */
                        /* ------------------------------------- */
                        snprintf(RIS_PRIOREQUEST_AP[r].signalGroup, RIF_STRINGLENGTH, "%s", FC_code[fc]);
                        number++;      /* SRM-bericht is aangepast - verhoog number */

                     }
                     else {

                        /* test op de juiste lijnnumers */
                        /* ---------------------------- */
                        for (i = 0; i < prm_line_max; i++) {         /* doorloop de opgegeven lijnnummer parameters */

                           if ( ((mulv) (atoi(RIS_PRIOREQUEST_AP[r].routeName))) == PRM[prm_line_first + i] )  { /* vergelijk lijnnummer */

                              /* plaats signalGroup in het SRM bericht */
                              /* ------------------------------------- */
                              snprintf(RIS_PRIOREQUEST_AP[r].signalGroup, RIF_STRINGLENGTH, "%s", FC_code[fc]);
                              number++;   /* SRM-bericht is aangepast - verhoog number */
                           }
                        }
                     }
                  }
               }
            }
            r++;
         }
      }
   }
   return (number);  /* aantal aangepaste SRM-berichten */
}


/* ============================================================================================================================================= */


/* TEST CONFLICTEN FASECYCLUS HULPDIENST */
/* ===================================== */
/* test_conflicten_fasecyclus_hulpdienst() test de conflict status van de fasecylus (fc) voor de hulpdiensten of alle conflicten op rood staan en worden tegengehouden.
/* test_conflicten_fasecyclus_hulpdienst() wordt aangeroepen door de functie ris_stuur_ssm()
*/ 

static rif_bool test_conflicten_fasecyclus_hulpdienst(count fc)   
{
   register rif_int j;
   rif_bool result = 0;
   count kfc;

   if (G[fc] || (RA[fc] && !RR[fc]) ) {  /* op X[fc] kan niet worden getest; X[] staat op voor synchronisaties!  */ 
      j = 0;
      while (j < GKFC_MAX[fc]) {    /* doorloop alle conflicten */
         kfc = KF_pointer[fc][j];   /* index van de conflictrichting */
         if (!RV[kfc] || (!(RR[kfc] & BIT6) || P[kfc]) )  break;     /* er is nog een conflict die niet op rood staat of niet wordt tegenhouden met BIT6 */
         j++;
       }
       if (j >= GKFC_MAX[fc])  {  /* alle conflicten staan op rood? */
          result = TRUE;
       }
    }

   return (result);
}

   

/* RIS VERSTUUR SSM */  /* @@@@@ ook functie maken die alle SSM-berichten kan versturen? */
/* ================ */
/* ris_verstuur_ssm() bepaalt de status van het prioriteitsverzoek en informeert bij een statuswijziging het prioriteitsvoertuig door
 * het versturen van een SSM-bericht met de gewijzigde prioriteitsstatus (prioState: Processing, WatchOtherTraffic, Granted, Rejected
 * of MaxPresence). de statuswijzigingen zijn gebaseerd op de UC3-Prioriteren specificatie. ris_verstuur_ssm() gebruikt de functie
 * ris_put_activeprio() voor het verzenden van SSM-berichten.
 * ris_verstuur_ssm() verzorgt/corrigeert ook de instelling van de prioriteitsoptie(s) voor de Prioriteitsmodule op basis van de ontvangen
 * ‘importance’ (gewicht van het prioriteitsverzoek). de prioriteitsinstelling van de PrioriteitsModule is hierbij maatgevend.
 * bij de aanroep van de functie dient de identificatie van het prioriteitstype van de fasecyclus (priotypefc_id ), die wordt gedefinieerd
 * bij de PrioriteitsModule-applicatie, als argument te worden opgegeven.
 * ris_verstuur_srm() geeft als return-waarde het aantal verzonden SSM-berichten (number).
 * ris_verstuur_srm() dient vanuit de regelapplicatie worden aangeroepen.
 *
 * voorbeelden
 * -----------
 * ris_verstuur_srm(prio_ov42ris);     // openbaar vervoer op richting 48
 * ris_verstuur_srm(prio_ov48ris);     // openbaar vervoer op richting 42
 * ris_verstuur_srm(prio_hd42ris);     // hulpdiensten op richting 42
 * ris_verstuur_srm(prio_hd48ris);     // hulpdiensten op richting 48
 *
 * 
 * PrioState van de SSM berichten die worden verstuurd
 * ---------------------------------------------------
 * RIF_PRIORITIZATIONSTATE_PROCESSING        = 2,   // Checking request (request is in queue, other requests are prior).
 * RIF_PRIORITIZATIONSTATE_WATCHOTHERTRAFFIC = 3,   // Cannot give full permission, therefore watch for other traffic. 
 *                                                  // Note that other requests may be present.
 * RIF_PRIORITIZATIONSTATE_GRANTED           = 4,   // Intervention was successful and now prioritization is active.
 * RIF_PRIORITIZATIONSTATE_REJECTED          = 5,   // The prioritization request was rejected by the traffic controller.
 * RIF_PRIORITIZATIONSTATE_MAXPRESENCE       = 6,   // The request has exceeded maxPresence time.Used when the controller has
 *                                                  // determined that the requester should then back off and request an alternative.
 */

/* #define NO_WATCHOTHERTRAFFIC */

rif_int ris_verstuur_ssm(rif_int priotypefc_id) {

   register rif_int r = 0;
   rif_int number = 0, j;
   count fc;

   fc = iFC_PRIOix[priotypefc_id];

   while (r < RIS_PRIOREQUEST_AP_NUMBER)
   {
      if ( (RIS_PRIOREQUEST_EX_AP[r].prioControlState == (priotypefc_id) )
         && ( (RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_REQUEST) || (RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_UPDATE) ) ) {

         switch (RIS_PRIOREQUEST_EX_AP[r].prioState) {

            case RIF_PRIORITIZATIONSTATE_UNKNOWN:
               break;          

            case RIF_PRIORITIZATIONSTATE_REQUESTED:

               /* Correctie Prioriteitsniveau */
               /* --------------------------- */
               if ( (RIS_PRIOREQUEST_AP[r].importance >  0) && (RIS_PRIOREQUEST_AP[r].importance <= 10) ) {       /* geconditoneerde prioriteit */
                  if (iInstPrioriteitsOpties[priotypefc_id] >= 13) {
/*                   iInstPrioriteitsOpties[priotypefc_id] = 13; */   /* 13 = poAanvraag(1)+                                poGroenVastHouden(4)+poBijzonderRealiseren(8) */
                     iInstPrioriteitsOpties[priotypefc_id] &= ~(poAfkappenKonfliktRichtingen | poAfkappenKonflikterendOV); /* afkappen verwijderen */
                  }
               } 
               else if ((RIS_PRIOREQUEST_AP[r].importance > 10) && (RIS_PRIOREQUEST_AP[r].importance <= 14) ) {   /* absolute prioriteit */
                  if (iInstPrioriteitsOpties[priotypefc_id] >= 15) {
                     /* prioriteitsniveau niet aanpassen */
                  }
               }
               else if (RIS_PRIOREQUEST_AP[r].importance ==  -1)  {  /* geen prioriteitsniveau in het bericht */
                      /* prioriteitsniveau niet aanpassen */
               }
               else if ((RIS_PRIOREQUEST_AP[r].importance <  -1) || (RIS_PRIOREQUEST_AP[r].importance >= 15) ) {   /* out of range -> geen prioriteit */
                  iInstPrioriteitsOpties[priotypefc_id] = 0;
               }

               /* Normal Flow - [requested] -> processing */
               /* --------------------------------------- */
               if ((RIS_PRIOREQUEST_EX_AP[r].prioControlState >  RIF_PRIORITIZATIONSTATE_UNKNOWN)   /* 1 */
                  && (iInstPrioriteitsOpties[priotypefc_id]>0)  ) { /* 1 */ 
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[r].id), (RIS_PRIOREQUEST_AP[r].sequenceNumber), RIF_PRIORITIZATIONSTATE_PROCESSING); /* 2 */
                  number++;      /* SSM-bericht verzonden - verhoog number */
               }

               /* Exception #3 - [requested] -> rejected */
               /* -------------------------------------- */
               else if (iInstPrioriteitsOpties[priotypefc_id]<=0 ) {  
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[r].id), (RIS_PRIOREQUEST_AP[r].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED); /* 5 */
                  number++;      /* SSM-bericht verzonden - verhoog number */
               }
               break;

            case RIF_PRIORITIZATIONSTATE_PROCESSING:

               if ((RIS_PRIOREQUEST_AP[r].role == RIF_VEHICLEROLE_EMERGENCY) && (RIS_PRIOREQUEST_AP[r].subrole == RIF_VEHICLESUBROLE_EMERGENCY) ) {   /* hulpdienst? */
                             
                  /* Normal Flow - [processing] -> granted - test de realisatie en ook de meerealisaties op de approach voor de hulpdiensten */
                  /* ----------------------------------------------------------------------------------------------------------------------- */ 
                  if (G[fc] && !iOnderMaximumVerstreken[priotypefc_id]) {     /* test fasecyclus */
                     j = 0;
                     while (iPrioMeeRealisatie[fc][j] >= 0) {  /* test ook de meerealisaties van de fasecyclus op de approach  */
                        if (!G[iPrioMeeRealisatie[fc][j]] /* && !iOnderMaximumVerstreken[priotypefc_id]??? - priotypefc_id van deze fasecyclus is niet bekend */ ) { 
                           break;   /* groenstatus meerealisatie is nog niet oke */
                        }
                        else {
                           j++; /* volgende meerealisatie */
                        }
                     }

                     /* zend SSM bericht - granted */
                     /* -------------------------- */
                     if (iPrioMeeRealisatie[fc][j] < 0) {
                        ris_put_activeprio((RIS_PRIOREQUEST_AP[r].id), (RIS_PRIOREQUEST_AP[r].sequenceNumber), RIF_PRIORITIZATIONSTATE_GRANTED); /* 4 */
                        number++;      /* SSM-bericht verzonden - verhoog number */
                        break;
                     }
                  }

#ifndef NO_WATCHOTHERTRAFFIC

                  /* Normal Flow - [processing] -> WatchOtherTraffic- test de realisatie en ook de meerealisaties op de approach voor de hulpdiensten */
                  /* -------------------------------------------------------------------------------------------------------------------------------- */
                  if (test_conflicten_fasecyclus_hulpdienst(fc) ) {   /* test de fasecyclus */
                     j = 0;
                     while (iPrioMeeRealisatie[fc][j] >= 0) {  /* test ook de meerealisaties van de fasecyclus op de approach  */
                        if (!test_conflicten_fasecyclus_hulpdienst ( iPrioMeeRealisatie[fc][j] ) ) {
                           break;   /* roodstatus meerealisatie is nog niet oke */
                        }
                        else {
                           j++; /* volgende meerealisatie */
                        }
                     }

                     /* zend SSM bericht - WatchOtherTraffic */
                     /* ------------------------------------ */
                     if (iPrioMeeRealisatie[fc][j] < 0) {
                        ris_put_activeprio((RIS_PRIOREQUEST_AP[r].id), (RIS_PRIOREQUEST_AP[r].sequenceNumber), RIF_PRIORITIZATIONSTATE_WATCHOTHERTRAFFIC); /* 3 */
                        number++;      /* SSM-bericht verzonden - verhoog number */
                        break;
                     }
                  }
#endif /* NO_WATCHOTHERTRAFFIC */
               }
               
               /* Normal Flow - [processing] -> granted - (overige roles/subroles) */
               /* ---------------------------------------------------------------- */
               else if (G[fc] && !iOnderMaximumVerstreken[priotypefc_id]) {   /* test fasecyclus */
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[r].id), (RIS_PRIOREQUEST_AP[r].sequenceNumber), RIF_PRIORITIZATIONSTATE_GRANTED); /* 4 */
                  number++;      /* SSM-bericht verzonden - verhoog number */
               }
               break;

            case RIF_PRIORITIZATIONSTATE_WATCHOTHERTRAFFIC:

               if ((RIS_PRIOREQUEST_AP[r].role == RIF_VEHICLEROLE_EMERGENCY) && (RIS_PRIOREQUEST_AP[r].subrole == RIF_VEHICLESUBROLE_EMERGENCY) ) {   /* hulpdienst? */
                 
                  /* Normal Flow - [WatchOtherTraffic] -> granted - voor de realisatie en meerealisaties op de approach voor hulpdiensten */
                  /* -------------------------------------------------------------------------------------------------------------------- */  
                  if (G[fc] && !iOnderMaximumVerstreken[priotypefc_id]) {     /* test fasecyclus */
                     j = 0;
                     while (iPrioMeeRealisatie[fc][j] >= 0) {  /* test ook de meerealisaties van de fasecyclus op de approach  */
                        if (!G[iPrioMeeRealisatie[fc][j]] /* && !iOnderMaximumVerstreken[priotypefc_id]??? */ ) { 
                           break;   /* groenstatus meerealisatie is nog niet oke */
                        }
                        else {
                           j++; /* volgende meerealisatie */
                        }
                     }

                     /* zend SSM bericht - granted */
                     /* -------------------------- */
                     if (iPrioMeeRealisatie[fc][j] < 0) {
                        ris_put_activeprio((RIS_PRIOREQUEST_AP[r].id), (RIS_PRIOREQUEST_AP[r].sequenceNumber), RIF_PRIORITIZATIONSTATE_GRANTED); /* 4 */
                        number++;      /* SSM-bericht verzonden - verhoog number */
                        break;
                     }
                  }
               }

               /* Normal Flow - [WatchOtherTraffic] -> granted (overige roles/subroles)  */
               /* ---------------------------------------------------------------------- */
               else if (G[fc] && !iOnderMaximumVerstreken[priotypefc_id]) {   /* test fasecyclus */
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[r].id), (RIS_PRIOREQUEST_AP[r].sequenceNumber), RIF_PRIORITIZATIONSTATE_GRANTED); /* 4 */
                  number++;      /* SSM-bericht verzonden - verhoog number */
               }
               break;
 
             case RIF_PRIORITIZATIONSTATE_GRANTED:

               /* Exception #8 - [granted] -> maxPresence -> end */
               /* ---------------------------------------------- */
               if ( EG[fc] ) {   /* afgebroken door een hulpdienst - eigenlijk logischer om Rejected te sturen, maar dat staat niet in de UC3-Specificatie @@@@@ */
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[r].id), (RIS_PRIOREQUEST_AP[r].sequenceNumber), RIF_PRIORITIZATIONSTATE_MAXPRESENCE); /* 6 */
                  number++;      /* SSM-bericht verzonden - verhoog number */
               }
               else if (iGroenBewakingsTimer[priotypefc_id] >= iGroenBewakingsTijd[priotypefc_id]) {
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[r].id), (RIS_PRIOREQUEST_AP[r].sequenceNumber), RIF_PRIORITIZATIONSTATE_MAXPRESENCE); /* 6 */
                  number++;      /* SSM-bericht verzonden - verhoog number */
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
      r++;
   }
   return (number);
}

