/* WACHTTIJD-VOORSPELLER */
/* ===================== */


/* (C) Copyright 2003-2019 by A.C.M. van Grinsven. All rights reserved.	*/


/* CCOL :  versie 10.0	*/
/* FILE :  wtcfunc.c	*/
/* DATUM:  17-04-2019	*/



/* include files */
/* ============= */
   #include "sysdef.c"		 /* definitie typen variabelen		*/
   #include "fcvar.h"            /* declaratie fasecyclusvariabelen     */
   #include "kfvar.h"            /* declaratie conflictvariabelen       */
   #include "lwmlvar.h"          /* declaratie variabelen lw-modulen    */



/* macrodefinities */
/* =============== */
/* #define TWACHT_EXTRA             */ /* extra groentijd t.b.v. bijvoorbeeld koppeltijden      */
/* #define TWACHT_CORRECTIE_MODULEN */ /* correctie voor bijvoorbeeld gelijkstart van fasecycli */
                                       /* correctie aanbrengen na berekening van de wachttijd   */
                                       /* van de fasecycli van de betreffende module            */
                                      

/* declaratie correctie functies */
/* ============================= */  
#ifdef TWACHT_CORRECTIE_MODULEN
  void max_wachttijd_modulen_primair_correctie(count ml);
#endif
#ifdef TWACHT_EXTRA
  extern mulv tg_extra[];
#endif

     

/* definitie wachttijdfuncties */
/* =========================== */

/* MAXIMALE WACHTTIJD VOOR AFWIKKELING CONFLICTEN */
/* ============================================== */

/* de functie max_wachttijd_conflicten() berekent voor de opgegeven faseyclus
 * de maximale wachttijd voor het afwikkelen van de conflicterende fasecycli.
 * de functie max_wachttijd_conflicten() geeft als return-waarde de berekende
 * wachttijd in tienden van seconden.
 * opmerking: deze functie is afgeleid van de functie max_tar().
 */

#ifndef NO_TIGMAX

mulv max_wachttijd_conflicten(count i)
{
   register count n, k;
   mulv twacht_max= 0;
   mulv twacht_tmp;


   if (R[i]) {    /* let op! i.v.m. snelheid alleen tijdens R[] behandeld  */
      for (n=0; n<KFC_MAX[i]; n++) {
#ifdef CCOLTIG
         k = KF_pointer[i][n];
#else
         k = TO_pointer[i][n];
#endif
	 if (TIG[k][i]) {	/* zoek grootste intergroentijd	*/
            twacht_tmp= 0;
            if (PR[k]) {
/*             if (G[kk] && !MG[kk] && !FM[kk] && !Z[kk]) { */
               if (CV[k]) {
                  twacht_tmp= TFG_max[k]+TVG_max[k]-TFG_timer[k]-TVG_timer[k]
                             + TIG_max[k][i]-TIG_timer[k];
#ifdef TWACHT_EXTRA
		  if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
               }
               else {
                  if (G[k]) {
                     twacht_tmp= TFG_max[k]-TFG_timer[k]
                                +TIG_max[k][i]-TIG_timer[k];
#ifdef TWACHT_EXTRA
		     if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
                  }
                  else { /* GL[k] || R[k] */
                     twacht_tmp= TIG_max[k][i]-TIG_timer[k];
                  }
               }
            }
            else {  /* !PR[k] */
               if (G[k]) {
                  twacht_tmp= TFG_max[k]-TFG_timer[k]
                             +TIG_max[k][i]-TIG_timer[k];
#ifdef TWACHT_EXTRA
		  if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
               }
               else {
                  twacht_tmp= TIG_max[k][i]-TIG_timer[k];
               }
            }
            if (twacht_tmp>twacht_max)  twacht_max= twacht_tmp;
         }
      }
      for (n=KFC_MAX[i]; n<GKFC_MAX[i]; n++) {
#ifdef CCOLTIG
         k = KF_pointer[i][n];
#else
         k = TO_pointer[i][n];
#endif
         if (TIG[k][i]) {	/* zoek grootste wachttijd  */
            twacht_tmp= 0;
            if (PR[k]) {
/*             if (G[kk] && !MG[kk] && !FM[kk] && !Z[kk]) { */
               if (CV[k]) {
                  twacht_tmp= TFG_max[k]+TVG_max[k]-TFG_timer[k]-TVG_timer[k];
#ifdef TWACHT_EXTRA
		  if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
               }
               else {
                  if (G[k]) {
                     twacht_tmp= TFG_max[k]-TFG_timer[k];
#ifdef TWACHT_EXTRA
		     if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
                  }
                  else { /* GL[k] || R[k] */
                     twacht_tmp= 0;
                  }
               }
            }
            else {  /* !PR[k] */
               if (G[k]) {
                  twacht_tmp= TFG_max[k]-TFG_timer[k];
#ifdef TWACHT_EXTRA
		  if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
               }
               else {
                  twacht_tmp= 0;
               }
            }
            if (twacht_tmp>twacht_max)  twacht_max= twacht_tmp;
         }
      }
   }
   return twacht_max;
}

#else

mulv max_wachttijd_conflicten(count i)
{
   register count n, k;
   mulv twacht_max= 0;
   mulv twacht_tmp;


   if (R[i]) {    /* let op! i.v.m. snelheid alleen tijdens R[] behandeld  */
      for (n=0; n<KFC_MAX[i]; n++) {
#ifdef CCOLTIG
         k = KF_pointer[i][n];
#else
         k = TO_pointer[i][n];
#endif
	 if (TO[k][i]) {	/* zoek grootste wachttijd	*/
            twacht_tmp= 0;
            if (PR[k]) {
/*             if (G[kk] && !MG[kk] && !FM[kk] && !Z[kk]) { */
               if (CV[k]) {
                  twacht_tmp= TFG_max[k]+TVG_max[k]-TFG_timer[k]-TVG_timer[k]
                             + TGL_max[k]+TO_max[k][i]-TGL_timer[k]-TO_timer[k];
#ifdef TWACHT_EXTRA
		  if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
               }
               else {
                  if (G[k]) {
                     twacht_tmp= TFG_max[k]-TFG_timer[k]
                                + TGL_max[k]+TO_max[k][i]-TGL_timer[k]-TO_timer[k];
#ifdef TWACHT_EXTRA
		     if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
                  }
                  else { /* GL[k] || R[k] */
                     twacht_tmp= TGL_max[k]+TO_max[k][i]-TGL_timer[k]-TO_timer[k];
                  }
               }
            }
            else {  /* !PR[k] */
               if (G[k]) {
                  twacht_tmp= TFG_max[k]-TFG_timer[k]
                             + TGL_max[k]+TO_max[k][i]-TGL_timer[k]-TO_timer[k];
#ifdef TWACHT_EXTRA
		  if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
               }
               else {
                  twacht_tmp= TGL_max[k]+TO_max[k][i]-TGL_timer[k]-TO_timer[k];
               }
            }
            if (twacht_tmp>twacht_max)  twacht_max= twacht_tmp;
         }
      }
      for (n=KFC_MAX[i]; n<GKFC_MAX[i]; n++) {
#ifdef CCOLTIG
         k = KF_pointer[i][n];
#else
         k = TO_pointer[i][n];
#endif
         if (TO[k][i]) {	/* zoek grootste wachttijd  */
            twacht_tmp= 0;
            if (PR[k]) {
/*             if (G[kk] && !MG[kk] && !FM[kk] && !Z[kk]) { */
               if (CV[k]) {
                  twacht_tmp= TFG_max[k]+TVG_max[k]-TFG_timer[k]-TVG_timer[k];
#ifdef TWACHT_EXTRA
		  if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
               }
               else {
                  if (G[k]) {
                     twacht_tmp= TFG_max[k]-TFG_timer[k];
#ifdef TWACHT_EXTRA
		     if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
                  }
                  else { /* GL[k] || R[k] */
                     twacht_tmp= 0;
                  }
               }
            }
            else {  /* !PR[k] */
               if (G[k]) {
                  twacht_tmp= TFG_max[k]-TFG_timer[k];
#ifdef TWACHT_EXTRA
		  if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
               }
               else {
                  twacht_tmp= 0;
               }
            }
            if (twacht_tmp>twacht_max)  twacht_max= twacht_tmp;
         }
      }
   }
   return twacht_max;
}
#endif



/* MAXIMALE WACHTTIJD VAN ALLE PRIMAIRE FASECYCLI */
/* ============================================== */

/* de functie max_wachttijd_modulen_primair() berekent de wachttijden van de primaire
 * fasecycli van de modulenreeks. eerst worden de wachttijden berekent van de primaire
 * fasecycli van de actieve module. daarna van de primaire fasecycli van de daarna 
 * volgende modulen. 
 * de functie max_wachttijd_modulen_primair() berekent de wachttijden in een systeemronde.
 * de functie max_wachttijd_conflicten() wordt gebruikt voor het berekenen van de
 * wachttijd voor het afhandelen van de lopende conflicten.
 */

void max_wachttijd_modulen_primair(boolv *prml[], count ml, count ml_max, mulv twacht[])
{
   register count i, k, m, n, hml;
   mulv twacht_tmp= NG;

 
   /* reset wachttijden van alle fasecycli */
   /* ------------------------------------ */
   for (i=0; i<FC_MAX; i++)
      twacht[i]= NG;

   /* bereken wachttijden van de primaire fasecycli van de actieve module */
   /* ------------------------------------------------------------------- */
   for (i=0; i<FC_MAX; i++) {
      if ( (prml[ml][i] & PRIMAIR_VERSNELD) && !PG[i] && R[i] )         
         twacht[i]= max_wachttijd_conflicten(i);
   }
#ifdef TWACHT_CORRECTIE_MODULEN
   max_wachttijd_modulen_primair_correctie(ml);
#endif

   /* bereken wachttijden van de primaire fasecycli van de volgende modulen */
   /* --------------------------------------------------------------------- */
   hml=ml+1;
   if (hml>=ml_max)  hml= ML1;
   for (m=1; m<ml_max; m++) {
      for (i=0; i<FC_MAX; i++) {
         if ( (prml[hml][i] & PRIMAIR_VERSNELD) && !PG[i] && (twacht[i]<0)) {
            twacht[i]= max_wachttijd_conflicten(i);
            for (n=0; n<KFC_MAX[i]; n++) {
#ifdef CCOLTIG
               k = KF_pointer[i][n];
#else
               k = TO_pointer[i][n];
#endif
               if (twacht[k]>=0) {	
#ifndef NO_TIGMAX
                  twacht_tmp= twacht[k]
                            + TFG_max[k] + TVG_max[k] + TIG_max[k][i];
#else
                  twacht_tmp= twacht[k]
                            + TFG_max[k] + TVG_max[k] + TGL_max[k] + TO_max[k][i];
#endif
#ifdef TWACHT_EXTRA
		  if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
                  if (twacht_tmp>twacht[i])  twacht[i]= twacht_tmp;
               }
            }
            for (n=KFC_MAX[i]; n<GKFC_MAX[i]; n++) {
#ifdef CCOLTIG
               k = KF_pointer[i][n];
#else
               k = TO_pointer[i][n];
#endif
               if (twacht[k]>=0) {	
                  twacht_tmp= twacht[k]
                            + TFG_max[k] + TVG_max[k];
#ifdef TWACHT_EXTRA
		  if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
                  if (twacht_tmp>twacht[i])  twacht[i]= twacht_tmp;
               }
            }
            for (n=GKFC_MAX[i]; n<FKFC_MAX[i]; n++) {
#ifdef CCOLTIG
               k = KF_pointer[i][n];
#else
               k = TO_pointer[i][n];
#endif
               if (twacht[k]>=0) {	
                  twacht_tmp= twacht[k];
                  if (twacht_tmp>twacht[i])  twacht[i]= twacht_tmp;
               }
            }
         }
      }
#ifdef TWACHT_CORRECTIE_MODULEN
      max_wachttijd_modulen_primair_correctie(hml);
#endif

   /* wachttijden voor overgeslagen fasecycli */
   /* -------------------------------------- */
   for (i=0; i<FC_MAX; i++) {
      if (PG[i] && !G[i]) {
         twacht[i]= -3;
      }
   }

   /* volgende module */
   /* --------------- */
      hml++;
      if (hml>=ml_max)  hml= ML1;
   }
}



/* MAXIMALE WACHTTIJD VAN ALLE PRIMAIRE FASECYCLI */
/* ============================================== */

/* de functie max_wachttijd_modulen_primair2() berekent de wachttijden van de primaire
 * fasecycli van de modulenreeks. eerst worden de wachttijden berekent van de primaire
 * fasecycli van de actieve module. daarna van de primaire fasecycli van de daarna 
 * volgende modulen. 
 * de functie max_wachttijd_modulen_primair2() verdeelt de berekening van de wachttijden 
 * over een aantal systeemronden.
 * de functie max_wachttijd_conflicten() wordt gebruikt voor het berekenen van de
 * wachttijd voor het afhandelen van de lopende conflicten.
 */

void max_wachttijd_modulen_primair2(boolv *prml[], count ml, count ml_max, mulv twacht[], mulv t_wacht[])
{
   register count i, k, n;
   static count hml= NG;
   mulv twacht_tmp= NG;


   /* start een nieuwe berekeningscyclus */
   /* ---------------------------------- */
   if (hml==NG)  hml= ml;

   /* bereken iedere systeemronde de wachttijden van de primaire fasecycli van de actieve module */
   /* ------------------------------------------------------------------------------------------ */
   for (i=0; i<FC_MAX; i++) {
      if (hml==ml) twacht[i]= NG;   /* reset wachttijden van alle fasecycli */
      if ( (prml[ml][i] & PRIMAIR_VERSNELD) && !PG[i] && R[i] ) {        
         t_wacht[i]= twacht[i]= max_wachttijd_conflicten(i);
      }
   }
#ifdef TWACHT_CORRECTIE_MODULEN
   max_wachttijd_modulen_primair_correctie(ml);
#endif

   /* ga naar de volgende module indien een nieuwe berekeningscyclus */
   /* -------------------------------------------------------------- */
   if (hml==ml) {
      hml++;
      if (hml>=ml_max)  hml= ML1;
   }

   /* bereken de wachttijden van de primaire fasecycli van de volgende module */
   /* ----------------------------------------------------------------------- */
   for (i=0; i<FC_MAX; i++) {
      if ( (prml[hml][i] & PRIMAIR_VERSNELD) && !PG[i] && (twacht[i]<0)) {
         twacht[i]= max_wachttijd_conflicten(i);
         for (n=0; n<KFC_MAX[i]; n++) {
#ifdef CCOLTIG
            k = KF_pointer[i][n];
#else
            k = TO_pointer[i][n];
#endif
            if (twacht[k]>=0) {	
#ifndef NO_TIGMAX
               twacht_tmp= twacht[k]
                         + TFG_max[k] + TVG_max[k] + TIG_max[k][i];
#else
               twacht_tmp= twacht[k]
                         + TFG_max[k] + TVG_max[k] + TGL_max[k] + TO_max[k][i];
#endif
#ifdef TWACHT_EXTRA
               if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
               if (twacht_tmp>twacht[i])  twacht[i]= twacht_tmp;
            }
         }
         for (n=KFC_MAX[i]; n<GKFC_MAX[i]; n++) {
#ifdef CCOLTIG
            k = KF_pointer[i][n];
#else
            k = TO_pointer[i][n];
#endif
            if (twacht[k]>=0) {	
               twacht_tmp= twacht[k]
                         + TFG_max[k] + TVG_max[k];
#ifdef TWACHT_EXTRA
	    if (tg_extra[k]>0) twacht_tmp += tg_extra[k];
#endif
               if (twacht_tmp>twacht[i])  twacht[i]= twacht_tmp;
            }
         }
         for (n=GKFC_MAX[i]; n<FKFC_MAX[i]; n++) {
#ifdef CCOLTIG
            k = KF_pointer[i][n];
#else
            k = TO_pointer[i][n];
#endif
            if (twacht[k]>=0) {	
               twacht_tmp= twacht[k];
               if (twacht_tmp>twacht[i])  twacht[i]= twacht_tmp;
            }
         }
      }
   }
#ifdef TWACHT_CORRECTIE_MODULEN
   max_wachttijd_modulen_primair_correctie(hml);
#endif
   
   /* wachttijden voor overgeslagen fasecycli */
   /* -------------------------------------- */
   for (i=0; i<FC_MAX; i++) {
      if (PG[i] && !G[i]) {
         t_wacht[i]= twacht[i]= -3;
      }
   }
    
   /* volgende module */
   /* --------------- */
   hml++;
   if (hml>=ml_max)  hml= ML1;

   /* kopieer de wachttijden indien alle wachttijden opnieuw zijn berekend */
   /* -------------------------------------------------------------------- */
   if (hml==ml) {  
      for (i=0; i<FC_MAX; i++) {
         t_wacht[i]= twacht[i]; 
      }
      hml= NG; /* t.b.v. start een nieuwe berekeningscyclus altijd met de actieve module (i.v.m. module-overgang) */
   }
}



/* MAXIMALE WACHTTIJD BIJ ALTERNATIEVE REALISTIE */
/* ============================================= */

/* de functie max_wachttijd_alternatief() berekent de wachttijd van een fasecyclus
 * die alternatief wordt gerealiseerd (RA[] && AR[]).
 * de functie geeft als return-waarde de berekende wachttijd in tienden van seconden.
 * de functie geeft een negatieve return-waarde (-3) indien de fasecyclus niet
 * alternatief wordt gerealiseerd.
 * de functie max_wachttijd_conflicten() wordt gebruikt voor het berekenen van de
 * wachttijd voor het afhandelen van de lopende conflicten.
 */

mulv max_wachttijd_alternatief(count fc, mulv twacht[])
{
   mulv twacht_max= -3;


   if (RA[fc] && AR[fc] && !RR[fc] && !BL[fc]) {     /* let op! i.v.m. snelheid alleen tijdens RA[] behandeld		*/
      twacht[fc]=twacht_max= max_wachttijd_conflicten(fc);
   }
   return twacht_max;
}




/* WACHTTIJD CORRECTIE T.B.V. GELIJKSTART FIETSERS */
/* =============================================== */   

/* de functie wachttijd_correctie_gelijkstart() corrigeert de berekende wachttijd t.b.v. 
 * de gelijkstart van twee fietsrichtingen. De grootste waarde is maatgevend. 
 */

void wachttijd_correctie_gelijkstart(count fc1,count fc2, mulv t_wacht[])
{

   if (R[fc1] && R[fc2]) {
      if (t_wacht[fc1]>t_wacht[fc2] && t_wacht[fc1]>=0) {
         t_wacht[fc2]= t_wacht[fc1];
      }
      else if (t_wacht[fc2]>=0) {
         t_wacht[fc1]= t_wacht[fc2];
      }
   }
}




/* TEGENHOUDEN VAN PRIMAIRE FASECYCLI DOOR OPENBAAR VERVOER REALISATIE */
/* =================================================================== */

/* de functie rr_modulen_primair() kijkt of er (conflicterende) primaire fasecycli van
 * de modulenreeks worden tegengehouden door openbaar vervoer realisaties.
 * eerst worden de primaire fasecycli van de actieve module bekeken, daarna de primaire
 * fasecycli van de volgende modulen. 
 * de wachtvariabele rr_twacht[] wordt opgezet indien door een OV-realisatie de primaire
 * fasecyclus zelf wordt tegenhouden of een (fictief) conflicterende fasecyclus in een
 * voorgaande module wordt tegengehouden.
 */

void rr_modulen_primair(boolv *prml[], count ml, count ml_max, mulv rr_twacht[])
{
   register count i, k, m, n, hml;

 
   /* reset rr_twacht van alle fasecycli */
   /* ---------------------------------- */
   for (i=0; i<FC_MAX; i++)
      rr_twacht[i]= NG;

   /* bekijk RR-BIT6 voor de primaire fasecycli van de actieve module */
   /* --------------------------------------------------------------- */
   for (i=0; i<FC_MAX; i++) {
      if ( (prml[ml][i] & PRIMAIR_VERSNELD) && !PG[i] && R[i]) {
         rr_twacht[i]= FALSE;
         if (RR[i] & BIT6) {
            rr_twacht[i]= TRUE;
         }
      }
   }

   /* bekijk RR-BIT6 voor de primaire fasecycli van de volgende modulen */
   /* ----------------------------------------------------------------- */
   hml=ml+1;
   if (hml>=ml_max)  hml= ML1;
   for (m=1; m<ml_max; m++) {
      for (i=0; i<FC_MAX; i++) {
         if ( (prml[hml][i] & PRIMAIR_VERSNELD) && !PG[i] && (rr_twacht[i]<0)) {
           rr_twacht[i]= FALSE;
           if (RR[i] & BIT6) {      /* primaire fasecyclus wordt zelf tegengehouden */
              rr_twacht[i]= TRUE;
           }
           else {
              for (n=0; n<GKFC_MAX[i]; n++) { /* conflicterende primaire fasecyclus in     */
#ifdef CCOLTIG
                 k = KF_pointer[i][n];        /* een voorgaande module wordt tegengehouden */
#else
                 k = TO_pointer[i][n];
#endif
                 if (rr_twacht[k]>0) {	
                    rr_twacht[i]= TRUE;
                    break;
                 }
              }
              for (n=GKFC_MAX[i]; n<FKFC_MAX[i]; n++) {/* fictief conflicterende primaire  */
#ifdef CCOLTIG
                 k = KF_pointer[i][n];                 /* fasecyclus in een voorgaande     */
#else
                 k = TO_pointer[i][n];
#endif
                 if (rr_twacht[k]>0) {	               /* module wordt tegengehouden       */
                    rr_twacht[i]= TRUE;
                    break;
                 }
              }
            }
         }
      }

      /* volgende module */
      /* --------------- */
      hml++;
      if (hml>=ml_max)  hml= ML1;
   }
}



