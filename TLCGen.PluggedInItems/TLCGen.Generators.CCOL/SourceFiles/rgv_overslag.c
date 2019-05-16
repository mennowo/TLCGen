/* DEFINITIE FUNCTIE */
/* ================= */


/* (C) Copyright 2008-2009 by A.C.M. van Grinsven. All rights reserved. */


/* CCOL:  version 7.0	   */
/* FILE:  rgv_overslag.c   */
/* DATE:  21-06-2009	   */


/* include files */
/* ============= */
   #include "fcvar.h"	      /* declaratie fasecyclus variabelen	*/
   #include "hevar.h"	      /* declaratie hulpelement variabelen	*/
   #include "lwmlvar.h"       /* declaratie modulevariabelen		*/


   extern mulv TVG_rgv[];     /* RGV-waarde - maximum verlenggroentijd  */


/* DEFINITIE FUNCTIE */
/* ================= */


/* RGV_NIET_PRIMAIR */
/* ================ */

/* de functie rgv_niet_primair() kan worden gebruikt om de verlenggroentijd van een
 * fasecyclus te verlagen, indien de fasecyclus in de cyclus niet primair is gerealiseerd.
 * rgv_niet_primair() geeft als return-waarde waar (TRUE) indien de de verlenggroentijd
 * is verlaagd anders niet waar (FALSE).
 *
 * voorbeeld:
 * rgv_niet_primair(fc03, PRML, ML, ML_MAX, hpri03, PRM[prmmintvg03], PRM[prmtvgomlaag03]);
 */

bool rgv_niet_primair (
    count fc,           /* fasecyclusnummer                             */
    bool *prml[],       /* primaire module toedeling                    */
	count ml,           /* actieve module                               */
	bool sml,          /* start module                               */
    count ml_max,       /* maxium aantal modulen                        */
    count hpri,         /* hulpelementnummer                            */
    mulv PRM_mintvg,    /* parameter minimum maximum verlenggroentijd   */
    mulv PRM_tvgomlaag, /* parameter verlaag maximum verlenggroentijd   */
    bool DD_fc)           /* status detectiestoring voor fasecyclus      */

{
   register count hml;  /* voorgaande (actieve) module */
   bool result= FALSE;

   if (PR[fc] && (prml[ml][fc] & PRIMAIR_VERSNELD)) {  /* primair (versneld) gerealiseerd     */
      IH[hpri]= TRUE;                                  /* onthou primaire realisatie          */
   }

   if (sml) {  /* module overgang? */
      if (!(prml[ml][fc] & PRIMAIR_VERSNELD)) {        /* niet primair in de actieve module   */
         hml= ml-1;     /* voorgaande module */
         if (hml<ML1) hml= ml_max-1;
         if (prml[hml][fc] & PRIMAIR_VERSNELD) {       /* wel primair in de voorgaande module */
            if (!IH[hpri] && !DD_fc) {                           /* niet primair gerealiseerd geweest   */
               if (!BL[fc] && R[fc] && !TRG[fc] && !A[fc]) {  /* niet geblokkeerd en in rood  */
                  TVG_rgv[fc] -= PRM_tvgomlaag;        /* verlaag maximum verlenggroentijd    */
                  if (TVG_rgv[fc]<PRM_mintvg) {      /* kleiner dan minimum verlenggroentijd? */
                     TVG_rgv[fc]= PRM_mintvg;     /* maak gelijk aan minimum verlenggroentijd */
                  }
                  result= TRUE;
               }
            }
            else {
               IH[hpri]= FALSE;  /* reset hpr  */
            }
         }
      }
   }
   return (result);
}

