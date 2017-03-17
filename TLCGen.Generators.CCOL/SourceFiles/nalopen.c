#include "nalopen.h"

/**************************************************************************
 *  Functie  : NaloopVtg
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2.
 *    Nalooptijd geldt vanaf startgroen aanvoerrichting en duurt tnl lang.
 **************************************************************************/
void NaloopVtg(count fc1, count fc2, count tnl)
{
   RT[tnl] |= SG[fc1];
   if (RT[tnl])             RW[fc2] |= BIT2;
   if (RT[tnl] || T[tnl])   YV[fc2] |= BIT2;
}

/**************************************************************************
 *  Functie  : NaloopVtgDet
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 afhankelijk van gebruikte dk
 *    Nalooptijd geldt vanaf startgroen aanvoerrichting en duurt tnl lang.
 **************************************************************************/
void NaloopVtgDet(count fc1, count fc2, count dk, count hdk, count tnl)
{
   if (SG[fc1]) IH[hdk] = FALSE;
   IH[hdk] |= D[dk] && !G[fc1] && A[fc1];
   RT[tnl] = SG[fc1] && H[hdk];
   if (RT[tnl])             RW[fc2] |= BIT2;
   if (RT[tnl] || T[tnl])   YV[fc2] |= BIT2;
}


/**************************************************************************
 *  Functie  : NaloopEG
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 vanaf eindgroen fc1
 *    Nalooptijd wordt geherstart tijdens G aanvoerrichting
 **************************************************************************/
void NaloopEG(count fc1, count fc2, count tnl)
{
   RT[tnl] = G[fc1];
   if (RT[tnl] || T[tnl])   RW[fc2] |= BIT2;
}

/**************************************************************************
 *  Functie  : NaloopEGDet
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 afhankelijk van detectie dp
 *    Nalooptijd geldt groen en geel van de aanvoerrichting en wordt gedurende
 *    het bezet zijn van detector dp geherstart.
 **************************************************************************/
void NaloopEGDet(count fc1, count fc2, count dp, count tnl)
{
   RT[tnl] = D[dp] && (G[fc1]||GL[fc1]);
   if (RT[tnl] || T[tnl] || G[fc1] || GL[fc1]) RW[fc2] |= BIT2;
}
/**************************************************************************
 *  Functie  : NaloopEG
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 vanaf eindgroen fc1
 *    Nalooptijd wordt geherstart tijdens cyclisch verlenggroen van de aanvoerrichting
 **************************************************************************/
void NaloopCV(count fc1, count fc2, count tnl)
{
   RT[tnl] = CV[fc1] && !RA[fc1];
   if (RT[tnl] || T[tnl])   RW[fc2] |= BIT2;
}

/**************************************************************************
 *  Functie  : NaloopEGDet
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 afhankelijk van detectie dp
 *    Nalooptijd geldt tijdens cyclisch verlenggroen van de aanvoerrichting en wordt gedurende
 *    het bezet zijn van detector dp geherstart.
 **************************************************************************/
void NaloopCVDet(count fc1, count fc2, count dp, count tnl)
{
   RT[tnl] = D[dp] && CV[fc1] && !RA[fc1];
   if (RT[tnl] || T[tnl] || G[fc1] || GL[fc1]) RW[fc2] |= BIT2;
}

/**************************************************************************
 *  Functie  : NaloopFG
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 tijdens vastgroen fc1
 *    Nalooptijd wordt geherstart tijdens FG aanvoerrichting
 **************************************************************************/


void NaloopFG(count fc1, count fc2, count tnl)
{
   RT[tnl] = VS[fc1] || FG[fc1];
   if (RT[tnl] || T[tnl])   RW[fc2] |= BIT2;
}
/**************************************************************************
 *  Functie  : NaloopFGDET
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 tijdens vastgroen fc1
 *    Nalooptijd wordt geherstart tijdens FG aanvoerrichting en wordt gedurende
 *    het bezet zijn van detector dp geherstart.
 **************************************************************************/

void NaloopFGDet(count fc1, count fc2, count dp, count tnl)
{
   RT[tnl] = (VS[fc1] || FG[fc1]) && D[dp] ;
   if (RT[tnl] || T[tnl])   RW[fc2] |= BIT2;
}

/* YMLX[] - VASTHOUDEN MODULE AFWIKKELING */
/* ====================================== */
/* yml_cv_pr-nl() tests CV[] of the phasecycles, with PRIMARY specification
 * in the active module and no PRIMARY specification in the next module.
 * yml_cv_pr() returns TRUE if CV[] && !WS[] or !RW[fci] & BIT2 (fci is nalooprichting) is detected, otherwise FALSE.
 * yml_cv_pr() can be used in the function application() - specification
 * of YMLx[].
 */

#if !defined (CCOLFUNC) || defined (LWMLFUNC2)

bool yml_cv_pr_nl(bool *prml[], count ml, count ml_max)
{
   register count i;
   register count hml= ml+1;		/* next module			*/

   if (hml>=ml_max)  hml= ML1;		/* first module			*/
   for (i=0; i<FC_MAX; i++) {
      if ((prml[ml][i] & PRIMAIR_VERSNELD) && !prml[hml][i] &&
	      (PR[i] && CV[i] && !(WG[i] && WS[i] && (RW[i] || YW[i])) && !(WG[i] && (RW[i] & BIT2))))
	    return (TRUE);
   }
   return (FALSE);
}

#endif

