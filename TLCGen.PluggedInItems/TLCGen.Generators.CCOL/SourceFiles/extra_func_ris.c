/* EXTRA_FUNC_RIS.C */
/* ================ */

/* FILE :  extra_func_ris.c   */
/* DATUM:  09-03-2023         */

/* DATUM:  09-03-2032 - ris_inmelding_selectief */
/* DATUM:  24-11-2020 - correctie in de functie ris_ris_verstuur_ssm() */
/* DATUM:  17-11-2020 - ondersteuning van indexering SRM/CAM wordt gebruikt in de functie ris_inmelding_selectief() */
/* DATUM:  11-11-2020 - correctie in de functie ris_srm_put_signalgroup() */


/* Deze ris prioriteit applicatiefuncties worden gebruikt in de programmagenerator TLCGEN in combinatie met de PrioModule van TLCGen */

#if (CCOL_V < 120000)
  #define GEEN_CONSOLIDATIE
#endif

/* include files */
/* ============= */
   #include "extra_func_ris.h"	/* declaratie risprio-functies   */
   #include "risvar.h"        	/* declaratie risfunc-functies   */
   #include "fcvar.h"         	/* declaratie G[], FC_MAX        */
   #include "kfvar.h"         	/* declaratie GKFC_MAX           */
   #include "prmvar.h"        	/* declaratie PRM[], PRM_MAX     */
   #include "prio.h"          	/* prioriteitsmodule             */
   #include <stdlib.h>        	/* declaratie atoi()             */
   #include <stdio.h>         	/* declaratie snprintf()         */
   #include <string.h>        	/* declaratie strlen()           */

#if (defined(_MSC_VER) && (_MSC_VER < 1900))
   #define snprintf sprintf_s
#endif

/* RIS INMELDING SELECTIEF */
/* ======================= */
/* ris_inmelding_selectief() is gebaseerd op de functie ris_detectie_selectief() uit de Toolkit CCOL.
 * hieraan is toegevoegd:
 * - controle op approach
 * - controle op de juiste role, subrole en importance
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
 * role (role_bits), subrole (subrole_bits), importance (importance_bits), eta_prm, en priotypefc_id als argument te worden meegegeven. stationtype, role en subrole worden bitgewijs gebruikt
 * (definitie in risappl.h). indien de waarde van statiotype_bits role_bits, subrole_bits of lane_id kleiner gelijk of gelijk is aan nul (0) wordt de controle
 * op de betreffende variabele buiten beschouwing gelaten.
 * ris_inmelding_selectief() kan worden aangeroepen vanuit de functie application().
 *
 * voorbeelden
 * -----------
 * IH[hinov421] = ris_inmelding_selectief (fc42,  NG,         "", ris_lane421, RIS_BUS | RIS_TRAM, 20, 100, RIS_ROLE_PUBLICTRANSPORT, NG, NG, PRM[prmeta42ov], prio_ov42ris);  // geen test op intersection, subrole en importance
 * IH[hinov481] = ris_inmelding_selectief (fc48,  NG, SYSTEM_ITF, ris_lane481, RIS_BUS | RIS_TRAM, 20, 100, RIS_ROLE_PUBLICTRANSPORT, NG, NG,              NG, prio_ov48ris);  // geen test op subrole, importance en eta
 *
 * IH[hinhd421] = ris_inmelding_selectief (NG, ris_approachid42 SYSTEM_ITF           NG, RIS_HULPDIENST, 20, 300, RIS_ROLE_EMERGENCY, RIS_SUBROLE_EMERGENCY, NG, NG, prio_hd42ris);  // geen test op fc, lane, importance en eta
 * IH[hinhd481] = ris_inmelding_selectief (NG, ris_approachid48,SYSTEM_ITF, ris_lane481, RIS_HULPDIENST, 20, 300, RIS_ROLE_EMERGENCY, RIS_SUBROLE_EMERGENCY, NG, NG, prio_hdv48ris); // geen test op fc, importance en eta
 *
 */
rif_bool ris_inmelding_selectief(count fc, rif_int approach_id, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_int role_bits, rif_int subrole_bits, rif_int importance_bits, rif_int eta_prm, rif_int priotypefc_id)
{
   register rif_int i, j, r = 0;
   rif_bool correct;


   while (r < RIS_PRIOREQUEST_AP_NUMBER) {   /* doorloop alle PrioRequest objecten */

      if (((RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_REQUEST) || (RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_UPDATE)) &&
         (RIS_PRIOREQUEST_EX_AP[r].prioState >= RIF_PRIORITIZATIONSTATE_REQUESTED) && (RIS_PRIOREQUEST_EX_AP[r].prioState <= RIF_PRIORITIZATIONSTATE_GRANTED)) {

         /* zet correct op FALSE */
         /* -------------------- */
         correct = FALSE;

         /* test op juiste intersection en signalgroup of approach */
         /* ------------------------------------------------------ */
         if (!strlen(intersection) || (strcmp(RIS_PRIOREQUEST_AP[r].intersection, intersection) == 0)) {                 /* test op juiste intersection */
            if (((fc >= 0) && (fc < FC_MAX)) && (strcmp(RIS_PRIOREQUEST_AP[r].signalGroup, FC_code[fc]) == 0)) {   /* test op juiste signalgroup - voor openbaar vervoer */
               correct = TRUE;    /* signalGroup is correct  */
            }
            else if (RIS_PRIOREQUEST_AP[r].approach == approach_id) {           /* test op juiste approach - voor hulpdiensten */
               correct = TRUE;    /* approach is correct */
            }
            else {
               /*  correct= FALSE; */ /* incorrecte signalGroup en incorrecte  approach -> volgende PrioRequest */
            }
         }

         /* test op inmelding */
         /* ----------------- */
         if (correct && (RIS_PRIOREQUEST_EX_AP[r].prioControlState == NG)    /* nog geen inmelding aanwezig  */
            && ((role_bits & (1 << (RIS_PRIOREQUEST_AP[r].role & 0xf))) || (role_bits <= 0))                         /* test op juiste role - bit        */
            && ((subrole_bits & (1 << (RIS_PRIOREQUEST_AP[r].subrole & 0xf))) || (subrole_bits <= 0))                /* test op juiste subrole - bit     */
            && ((importance_bits & (1 << (RIS_PRIOREQUEST_AP[r].importance & 0xf))) || (importance_bits <= 0))) {   /* test op juiste importance - bit  */

            /* test selectieve inmelding op basis van eta */
            /* ------------------------------------------ */
            if (eta_prm > 0) {                                                                      /* test op ingestelde eta_prm. (eta_prm <= 0) is uitgeschakeld */
               if ((RIS_PRIOREQUEST_AP[r].eta > RIF_UTC_TIME_PB)                                     /* test of eta in de toekomst ligt */
                  && ((RIS_PRIOREQUEST_AP[r].eta - RIF_UTC_TIME_PB) < (eta_prm * 100))) {         /* test op juiste eta_prm */

                  /*  test stationType */
                  /*  ---------------- */
                  if (stationtype_bits <= 0) {                                                        /* inmelden zonder CAM-informatie - stationType */
                     RIS_PRIOREQUEST_EX_AP[r].prioControlState = priotypefc_id;
                     return ((rif_bool)TRUE);
                  }
                  else {                                                                              /* inmelden met CAM-informatie - stationType */
#ifdef RIS_GEEN_INDEXERING
                     i = 0;
                     while (i < RIS_ITSSTATION_AP_NUMBER) {                                           /* doorloop alle ItsStation objecten */
#else
                     i = RIS_PRIOREQUEST_EX_AP[r].ItsStationIndex;
                     if ((i >= 0) && (i < RIS_ITSSTATION_AP_NUMBER)) {
#endif /* RIS_GEEN_INDEXERING */
                        if (strcmp(RIS_PRIOREQUEST_AP[r].itsStation, RIS_ITSSTATION_AP[i].id) == 0) {   /* test op zelfde ItsStation ID */
                           if ((stationtype_bits & (1 << (RIS_ITSSTATION_AP[i].stationType & 0xf))) /* || (stationtype_bits <= 0) */) {  /* test stationType - bit */
                              RIS_PRIOREQUEST_EX_AP[r].prioControlState = priotypefc_id;
                              return ((rif_bool)TRUE);
                     }
                     }
#ifdef RIS_GEEN_INDEXERING
                        i++;
#endif /* RIS_GEEN_INDEXERING */
                  }
               }
            }
         }

            /* test selectieve inmelding op basis van map (locatie) */
            /* ---------------------------------------------------- */
#ifdef RIS_GEEN_INDEXERING
            i = 0;
            while (i < RIS_ITSSTATION_AP_NUMBER) {                                                 /* doorloop alle ItsStation objecten */
#else
            i = RIS_PRIOREQUEST_EX_AP[r].ItsStationIndex;
            if ((i >= 0) && (i < RIS_ITSSTATION_AP_NUMBER)) {
#endif /* RIS_GEEN_INDEXERING */
               if (strcmp(RIS_PRIOREQUEST_AP[r].itsStation, RIS_ITSSTATION_AP[i].id) == 0) {      /* test op zelfde ItsStation ID */
                  if ((stationtype_bits & (1 << (RIS_ITSSTATION_AP[i].stationType & 0xf))) || (stationtype_bits <= 0)) {  /* test stationType - bit */
                     for (j = 0; j < RIF_MAXLANES; j++) {                                          /* doorloop alle gematchte lanes */
                        if (!strlen(RIS_ITSSTATION_AP[i].matches[j].intersection) || (strlen(intersection) && (strcmp(RIS_ITSSTATION_AP[i].matches[j].intersection, intersection) != 0))) {  /* test intersection ID */
                           break;
                        }
                        if ((lane_id == RIS_ITSSTATION_AP[i].matches[j].lane)                      /* test op juiste lane id  */
                           || (lane_id <= 0) /* && (RIS_PRIOREQUEST_AP[r].approach==approach_id) */) {  /* test op alle lanes van de approach */
                           if ((RIS_ITSSTATION_EX_AP[i].matches[j].distance > length_start) && (RIS_ITSSTATION_EX_AP[i].matches[j].distance <= length_end)) {  /* test distance */
                              RIS_PRIOREQUEST_EX_AP[r].prioControlState = priotypefc_id;
                              return ((rif_bool)TRUE);
                           }
                        }
                     }
                  }
               }
#ifdef RIS_GEEN_INDEXERING
               i++;
#endif /* RIS_GEEN_INDEXERING */
            }
            }
      }
      r++;
   }
   return ((rif_bool)FALSE);
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
         if ( (RIS_PRIOREQUEST_EX_AP[r].prioControlState == (priotypefc_id) )  ) {        /* test op juiste prioControleState */
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

                  if ( (prm_line_first < 0) || (prm_line_max <= 0) ) {  
                  
                     /* geen lijn-parameters opgegeven - don't care */
                     /* ------------------------------------------- */

                     /* plaats signalGroup in het SRM bericht */
                     /* ------------------------------------- */
                     snprintf(RIS_PRIOREQUEST_AP[r].signalGroup, RIF_STRINGLENGTH, "%s", FC_code[fc]);
                     number++;      /* SRM-bericht is aangepast - verhoog number */

                  }
                  else if ( (prm_line_first + prm_line_max) <= PRM_MAX ) {  /* test parameter range */

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
         if (!RV[kfc] || (!(RR[kfc] & BIT6)
#if (CCOL_V >= 110)
             || P[kfc]
#endif
             ) )  break;     /* er is nog een conflict die niet op rood staat of niet wordt tegenhouden met BIT6 */
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
 * ?importance? (gewicht van het prioriteitsverzoek). de prioriteitsinstelling van de PrioriteitsModule is hierbij maatgevend.
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

#define NO_WATCHOTHERTRAFFIC

#if defined prioFCMAX && (prioFCMAX > 0) /* alleen indien PRIO */

rif_int ris_verstuur_ssm(rif_int priotypefc_id, rif_int risgrenspriotype) {

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
               if ((RIS_PRIOREQUEST_AP[r].importance > 0) && (RIS_PRIOREQUEST_AP[r].importance <= risgrenspriotype)) {       /* geconditoneerde prioriteit */
                  if (iInstPrioriteitsOpties[priotypefc_id] >= 13) {
/*                   iInstPrioriteitsOpties[priotypefc_id] = 13; */   /* 13 = poAanvraag(1)+                                poGroenVastHouden(4)+poBijzonderRealiseren(8) */
                     iInstPrioriteitsOpties[priotypefc_id] &= ~(poAfkappenKonfliktRichtingen | poAfkappenKonflikterendOV); /* afkappen verwijderen */
                  }
               } 
               else if ((RIS_PRIOREQUEST_AP[r].importance > risgrenspriotype) && (RIS_PRIOREQUEST_AP[r].importance <= 14)) {   /* absolute prioriteit */
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
               if (iInstPrioriteitsOpties[priotypefc_id] > 0) { /* 1 */
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
                  if (G[fc] && !iOnderMaximumVerstreken[priotypefc_id] && !iMaximumWachtTijdOverschreden[priotypefc_id]) {/* test fasecyclus */
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

void Bepaal_Granted_Verstrekt(void)
{
   count    fc, i;
   rif_int  priotypefc_id;
   register rif_int r = 0;

   for (i = 0; i < FCMAX; ++i)
   {
      granted_verstrekt[i] = 0;
   }

   while (r < RIS_PRIOREQUEST_AP_NUMBER)
   {
      if (RIS_PRIOREQUEST_EX_AP[r].prioState == RIF_PRIORITIZATIONSTATE_GRANTED)
      {
         priotypefc_id = RIS_PRIOREQUEST_EX_AP[r].prioControlState;
         fc = (count)iFC_PRIOix[priotypefc_id];
         granted_verstrekt[fc] = 1;
         if ((RIS_PRIOREQUEST_AP[r].role == RIF_VEHICLEROLE_EMERGENCY) && (RIS_PRIOREQUEST_AP[r].subrole == RIF_VEHICLESUBROLE_EMERGENCY)) { granted_verstrekt[fc] = 2; }   /* granted verstrekt door hulpdienst ingreep */
      }
      r++;
   }
}

#endif /* prioFCMAX - alleen indien PRIO */

/* iVRI ? ISSUE TWEERICHTINGEN FIETSPADEN */
/* ===================================== */ 

/* Probleemstelling
 * ----------------
 * Op tweerichtingen fietspaden worden CAM berichten verstuurd van fietsers die naar de stopstreep rijden en van fietsers die van de stopstreep af rijden.
 * De fietsers die van de stopstreep af rijden veroorzaken nu foutieve informatie bij aanvragen en verlengen op basis van CAM-berichten van fietsers. 
 * Het splitsen van de lanes van deze fietspaden in rijrichting zou een mogelijke oplossing zijn. In het ITF-bestand is het echter verplicht om deze
 * tweerichtingen fietspaden op te geven als ?bidirectionele lanes?. Dit probleem kan worden opgelost door ook naar de heading (rijrichting) van de
 * fietsers in de CAM-berichten te kijken.
 *
 * Heading
 * -------
 * De heading in de CAM-berichten is een waarde van 0-360 graden. Tijdens stilstand van een ItsStation wordt voor de heading in de CAM-berichten de waarde 0 (nul) doorgegeven.
 * In  CCOL wordt in het uitgebreide CAM-bericht (RIS_ITSSTATION_EX_AP[ ]) ook de laatst ontvangen heading waarde groter dan 0 (nul) bewaard (RIS_ITSSTATION_EX_AP[].heading; type float).
 * Bij het kijken naar de heading dient een zekere toegestane afwijking (heading_marge) t.o.v. de rijrichting in acht te worden genomen; bijvoorbeeld 45 graden.
 * 
 * 
 * Voorbeeld bij een heading van 90 graden en een afwijking van 45 graden
 * ----------------------------------------------------------------------
 *
 *                / (ondergrens: 90-45 = 45 graden)
 *               /
 *              /  45 graden (marge)
 * ----------------------------------------<--- 90 graden rijrichting
 *              \  45 graden (marge)
 *               \
 *                \ (bovengrens: 90+45 = 135 graden)
 *
 *
 * 
 * De functie ris_check_heading() controleert de heading in het CAM bericht. ris_check_heading() geeft als return waarde waar (TRUE)
 * als de berekende afwijking van de heading binnen de opgegeven heading_marge valt, anders niet waar (FALSE).
 * ris_check_heading() wordt aangeroepen vanuit de functies ris_detectie_heading() en ris_itsstations_heading().
 *
 */

/* DEFINITIE RIS APPLICATIEFUNCTIES - HEADING */
/* ========================================== */

/* RIS CHECK HEADING */
/* ================= */
/* ris_check_heading() geeft als return waarde waar (TRUE) als de berekende afwijking van de heading binnen de opgegeven heading_marge valt,
 * anders niet waar (FALSE).
 * ItsStations die geen goede heading-informatie hebben (heading<=0) worden buiten beschouwing gelaten:
 * - bij geen heading informatie (heading = -1).
 * - bij stilstand van de ItsStation (heading = 0).
 * voor de berekening van de afwijking van de heading wordt gebruik gemaakt van een formule die Jonathan de Vries (gemeente Utrecht) heeft
 * gevonden op internet.
 * ris_check_heading() wordt aangeroepen vanuit de functies ris_detectie_heading() en ris_itsstations_heading().
 */

rif_bool ris_check_heading(rif_float itsstation_heading, mulv heading, mulv heading_marge)
{
   rif_bool heading_correct = FALSE;
   mulv heading_afwijking = 0;


  /* bereken de afwijking van de heading */
   /* ---------------------------------- */
   if (	(itsstation_heading <  0) ||      /* ItsStation heeft geen heading informatie  */
        (itsstation_heading == 0) ) {     /* ItsStation die stilstaat                  */ 
      heading_afwijking = 9999;
   }
   else {
      heading_afwijking = ( (mulv) heading - (mulv) itsstation_heading + 180 + 360) % 360 - 180; 
   }

   /* controleer de heading afwijking */
   /* ------------------------------- */
   if (	(heading_afwijking <= heading_marge) && (heading_afwijking >= -heading_marge) ) {   /* rijdt ItsStation richting de stopstreep? */       
      heading_correct = TRUE;
   }

   return (heading_correct);
}

/* RIS DETECTIE HEADING */
/* ==================== */
/* ris_detectie_heading() geeft als return waarde waar (TRUE), als er in de RIS_ITSSTATION_AP-buffer een RIS-voertuig aanwezig is van het juiste stationtype
 * in het opgegeven gebied van de rijstrook t.o.v. de stopstreep met een correcte heading, anders niet waar (FALSE). length_start ligt dichter bij de stopstreep dan length_end. 
 * indien match_signalgroup waar (TRUE) is, dient er ook een signalgroup-match te zijn en wordt er op alle gemapte lanes getest.
 * indien match_signalgroup niet waar (FALSE) is, wordt signalgroup buiten beschouwing gelaten en wordt alleen op de eerste lane met de kleinste offset getest.
 * ris_detetectie_heading() gebruikt de functie ris_check_heading() voor het controleren van de heading afwijking.
 * ris_detetectie_heading() wordt aangeroepen door de functies ris_aanvraag_heading() en ris_verlengen_heading().
 * ris_detectie_heading() kan ook worden aangeroepen vanuit de functie application().
 *
 * voorbeelden: IH[hmvtg081]= ris_detectie_heading (fc08, "", ris_lane081, RIS_MOTORVEHICLES, 0, 90, FALSE 270, 45));          // geen test op intersection
 *              IH[hmvtg281]= ris_detectie_heading (fc28, SYSTEM_ITF, ris_lane281, RIS_CYCLIST, 0, 50, FALSE 90, 45));
 */

rif_bool ris_detectie_heading(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_bool match_signalgroup, mulv heading, mulv heading_marge)
{
#ifndef GEEN_CONSOLIDATIE
   register count i = 0, j, n;
   rif_bool itsstation_signalgroup = FALSE;
#else
   register count i = 0, j;
#endif   /* GEEN_CONSOLIDATIE */

   while (i < RIS_ITSSTATION_AP_NUMBER) { /* doorloop alle ItsStation objecten */
      if (stationtype_bits & (1 << RIS_ITSSTATION_AP[i].stationType) ) {     /* test stationType - bit   */
#ifndef GEEN_CONSOLIDATIE
         if (match_signalgroup) { /* test in ItsStation op aanwezigheid van FC_code[fc] in signalGroup */
            itsstation_signalgroup= FALSE;
            for (n = 0; n < RIF_MAXSIGNALGROUPSINCAM; n++) {
               if (strlen(RIS_ITSSTATION_AP[i].signalGroup[n])) {                      /* er is een signalGroup aanwezig   */
                  if (strcmp(RIS_ITSSTATION_AP[i].signalGroup[n], FC_code[fc]) == 0) { /* test de code van de signalGroup  */
                     itsstation_signalgroup = TRUE;                                    /* signalGroup gevonden             */
                     break;   /* break signalGroup gevonden */
                  }
               }
               else {
                  break;      /* break bij geen signalGroup */
               }
            }
         }
#endif   /* GEEN_CONSOLIDATIE */
         for (j = 0; j < RIF_MAXLANES; j++) {
            if (!strlen(RIS_ITSSTATION_AP[i].matches[j].intersection) || (strlen(intersection) && (strcmp(RIS_ITSSTATION_AP[i].matches[j].intersection, intersection) != 0))) {   /* test intersection ID */
               break;
            }
            if (lane_id == RIS_ITSSTATION_AP[i].matches[j].lane) {  /* test op juiste lane id */
#ifndef GEEN_CONSOLIDATIE
               if (itsstation_signalgroup || !match_signalgroup /* || (strcmp(RIS_ITSSTATION_AP[i].matches[j].signalGroup, FC_code[fc]) == 0) */ ) { /* test op juiste signaalgroep */
#else
               if (!match_signalgroup || (strcmp(RIS_ITSSTATION_AP[i].matches[j].signalGroup, FC_code[fc]) == 0)) { /* test op juiste signaalgroep */
#endif   /* GEEN_CONSOLIDATIE */
                  if ((RIS_ITSSTATION_EX_AP[i].matches[j].distance > length_start) && (RIS_ITSSTATION_EX_AP[i].matches[j].distance <= length_end)) {  /* test op distance */
                     if (ris_check_heading(RIS_ITSSTATION_AP[i].heading, heading, heading_marge)) { /* is heading correct? */
                        return ( (rif_bool) TRUE);
                     }
                  }
               }
            }
#ifndef NO_CHNG2030210
            if (!match_signalgroup) break;   /* bij geen match_signalgroup alleen de lane met de kleinste offset testen */
#endif
         }
      }
      i++;
   }
   return ( (rif_bool) FALSE);
}

/* RIS AANVRAAG HEADING */
/* ==================== */
/* ris_aanvraag_heading() geeft als return waarde waar (TRUE), als er een aanvraag moet worden opzet, anders niet waar (FALSE).
 * een aanvraag moet worden opgezet indien tijdens de roodfase en geen garantieroodtijd er in de RIS_ITSSTATION_AP-buffer
 * een RIS-voertuig aanwezig is van het juiste stationtype in het opgegeven gebied van de rijstrook t.o.v. de stopstreep met een correcte heading.
 * length_start ligt dichter bij de stopstreep dan length_end. indien match_signalgroup waar (TRUE) is, dient er ook een signalgroup-match te zijn,
 * anders wordt signalgroup buiten beschouwing gelaten.
 * ris_aanvraag_heading() gebruikt de functie ris_detectie_heading() voor het detecteren van een RIS-voertuig in het opgegeven gebied.
 * ris_aanvraag_heading() kan worden aangeroepen vanuit de functie application().
 *
 * voorbeelden: if (ris_aanvraag_heading (fc08, "", ris_lane081, RIS_VEHICLES, 0, 90, FALSE, 270, 45))  A[fc08] |= BIT8;          // geen test op intersection
 *              if (ris_aanvraag_heading (fc28, SYSTEM_ITF, ris_lane281, RIS_CYCLIST, 0, 50, FALSE, 90, 45))  A[fc28] |= BIT8;
 */

rif_bool ris_aanvraag_heading(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_bool match_signalgroup, mulv heading, mulv heading_marge)
{
   if (R[fc] && !TRG[fc]) {  /* ivm snelheid - test op aanvraag tijdens rood en garantieroodtijd verstreken */
      if (ris_detectie_heading(fc, intersection, lane_id, stationtype_bits, length_start, length_end, match_signalgroup, heading, heading_marge) ) return ( (rif_bool) TRUE);
   }
   return ( (rif_bool) FALSE);
}

/* RIS VERLENGEN HEADING */
/* ===================== */
/* ris_verlengen_heading() geeft als return waarde waar (TRUE), als het meetkriterium moet worden opzet ,anders niet waar (FALSE).
 * het meetkriterium moet worden opgezet indien tijdens de groenfase er in de RIS_ITSSTATION_AP-buffer een RIS-voertuig aanwezig is van
 * het juiste stationtype in het opgegeven gebied van de rijstrook t.o.v. de stopstreep met een correcte heading.
 * length_start ligt dichter bij de stopstreep dan length_end.
 * indien match_signalgroup waar (TRUE) is, dient er ook een signalgroup-match te zijn, anders wordt signalgroup buiten beschouwing gelaten.
 * ris_verlengen_heading() gebruikt de functie ris_detectie_heading() voor het detecteren van een RIS-voertuig in het opgegeven gebied.
 * ris_verlengen_heading() kan worden aangeroepen vanuit de functie application().
 *
 * voorbeelden: if (ris_verlengen_heading(fc08, "", ris_lane081, RIS_VEHICLES, 10, 90, FALSE, 270, 45)))  MK[fc08] |= BIT8; else  MK[fc08] &= ~BIT8;          // geen test op intersection 
 *              if (ris_verlengen_heading(fc28, SYSTEM_ITF, ris_lane281, RIS_CYCLIST, 10, 50, FALSE, 90, 45)))  MK[fc28] |= BIT8; else  MK[fc28] &= ~BIT8; 
 */

rif_bool ris_verlengen_heading(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_bool match_signalgroup, mulv heading, mulv heading_marge)
{
   if (G[fc]) {      /* ivm snelheid - alleen afhandelen tijdens groen */
      if (ris_detectie_heading(fc, intersection, lane_id, stationtype_bits, length_start, length_end, match_signalgroup, heading, heading_marge) )  return ( (rif_bool) TRUE);
   }
   return ( (rif_bool) FALSE);
}

/* RIS ITSSTATIONS HEADING */
/* ======================= */
/* ris_itsstations_heading() berekent voor de opgegeven lane het aantal aanwezige ItsStations van het juiste stationtype
 * in het opgegeven gebied van de rijstrook t.o.v de stopstreep met een correcte heading.
 * length_start ligt dichter bij de stopstreep dan length_end.
 * indien match_signalgroup waar (TRUE) is, dient er ook een signalgroup-match te zijn en wordt er op alle gemapte lanes getest.
 * indien match_signalgroup niet waar (FALSE) is, wordt signalgroup buiten beschouwing gelaten en wordt alleen op de eerste lane met de kleinste offset getest.
																			 
 * ris_itsstations_heading() geeft als return waarde het berekende aantal ItsStations.
 * ris_itsstations_heading() gebruikt de functie ris_check_heading() voor het controleren van de heading afwijking.
 * ris_itsstations_heading() kan worden aangeroepen vanuit de functie application().
 *
 * voorbeeld: MM[m051] = ris_itsstations_heading(fc05, "", ris_lane051, RIS_MOTORVEHICLES, 50.0, 100.0, FALSE, 180, 45);          // geen test op intersection
 * voorbeeld: MM[m261] = ris_itsstations_heading(fc26, SYSTEM_ITF, ris_lane261, RIS_CYCLIST, 20.0, 80.0, FALSE, 359, 45);
 * 
 */

mulv ris_itsstations_heading(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_bool match_signalgroup, mulv heading, mulv heading_marge)
{
#ifndef GEEN_CONSOLIDATIE
   register count i = 0, j, n;
   mulv number = 0;
   rif_bool itsstation_signalgroup = FALSE;
#else
   register count i = 0, j;
   mulv number = 0;
#endif   /* GEEN_CONSOLIDATIE */

   while (i < RIS_ITSSTATION_AP_NUMBER) { /* doorloop alle ItsStation objecten */
      if (stationtype_bits & (1 << RIS_ITSSTATION_AP[i].stationType) ) {   /* test stationType - bit  */
#ifndef GEEN_CONSOLIDATIE
         if (match_signalgroup) {                     /* test in ItsStation op aanwezigheid van FC_code[fc] in signalGroup */
            for (n = 0; n < RIF_MAXSIGNALGROUPSINCAM; n++) {
               itsstation_signalgroup= FALSE;
               if (strlen(RIS_ITSSTATION_AP[i].signalGroup[n])) {                      /* er is een signalGroup aanwezig   */
                  if (strcmp(RIS_ITSSTATION_AP[i].signalGroup[n], FC_code[fc]) == 0) { /* test de code van de signalGroup  */
                     itsstation_signalgroup = TRUE;                                    /* signalGroup gevonden             */
                     break;   /* break signalGroup gevonden */
                  }
               }
               else {
                  break;      /* break bij geen signalGroup */
               }
            }
         }
#endif   /* GEEN_CONSOLIDATIE */
         for (j = 0; j < RIF_MAXLANES; j++) {
            if (!strlen(RIS_ITSSTATION_AP[i].matches[j].intersection) || (strlen(intersection) && (strcmp(RIS_ITSSTATION_AP[i].matches[j].intersection, intersection) != 0))) {    /* test intersection ID */
               break;
            }
            if (lane_id == RIS_ITSSTATION_AP[i].matches[j].lane) {  /* test op juiste lane id */
#ifndef GEEN_CONSOLIDATIE
               if (itsstation_signalgroup || !match_signalgroup /* || (strcmp(RIS_ITSSTATION_AP[i].matches[j].signalGroup, FC_code[fc]) == 0) */ ) {  /* test op juiste signaalgroep */
#else
               if ( !match_signalgroup || (strcmp(RIS_ITSSTATION_AP[i].matches[j].signalGroup, FC_code[fc]) == 0) ) {  /* test op juiste signaalgroep */
#endif   /* GEEN_CONSOLIDATIE */
                  if ((RIS_ITSSTATION_EX_AP[i].matches[j].distance > length_start) && (RIS_ITSSTATION_EX_AP[i].matches[j].distance <= length_end)) {  /* test op distance */
                     if (ris_check_heading(RIS_ITSSTATION_AP[i].heading, heading, heading_marge)) { /* is heading correct? */
                        number++;
                     }
                  }
               }
            }
#ifndef NO_CHNG2030210
            if (!match_signalgroup) break;   /* bij geen match_signalgroup alleen de lane met de kleinste offset testen */
#endif
         }
      }
      i++;
   }
   return (number);
}
