#include "nalopen.h"
#include "gkvar.h"
#include "nlvar.h"
count TNL_TGK[TMMAX];

/**************************************************************************
 *  Functie  : NaloopVtg
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2.
 *    Nalooptijd geldt vanaf startgroen aanvoerrichting en duurt tnl lang.
 **************************************************************************/
void NaloopVtg(count fc1, count fc2, count tnl)
{
   RT[tnl] = SG[fc1];
   if (RT[tnl])             RW[fc2] |= BIT2;
   if (RT[tnl] || T[tnl])   YV[fc2] |= BIT2;
   berekenTNL(fc2,tnl);
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
   berekenTNL(fc2,tnl);
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
   berekenTGK_max(fc1,fc2,T_max[tnl]);
   berekenTNL(fc2,tnl);
}

/**************************************************************************
 *  Functie  : NaloopEGDet
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 afhankelijk van detectie dp
 *    Nalooptijd geldt groen en geel van de aanvoerrichting en wordt gedurende
 *    het bezet zijn van detector dp geherstart.
 **************************************************************************/
void NaloopEGDet(count fc1, count fc2, count tnl, ...)
{
   va_list argpt;
   count dp;

   va_start(argpt, tnl);
   RT[tnl] = FALSE;
   while ((dp = va_arg(argpt, va_count)) != END)
   {
	   RT[tnl] |= D[dp] && (G[fc1] || GL[fc1]);
   }
   va_end(argpt);
    if (RT[tnl] || T[tnl] || G[fc1] || GL[fc1]) RW[fc2] |= BIT2;
    if (G[fc1] || GL[fc1] || !GKC[fc1]) berekenTGK_max(fc1,fc2,T_max[tnl]+TGL_max[fc1]);
	if (EGL[fc1]) TNL_TGK[tnl]= T_max[tnl]-T_timer[tnl] +TGL_max[fc1];
	if (R[fc1] && GKC[fc1]) berekenTGK_max(fc1,fc2,TNL_TGK[tnl]);
    berekenTNL(fc2,tnl);

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
   berekenTNL(fc2,tnl);
}

/**************************************************************************
 *  Functie  : NaloopCVDet
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 afhankelijk van detectie dp
 *    Nalooptijd geldt tijdens cyclisch verlenggroen van de aanvoerrichting en wordt gedurende
 *    het bezet zijn van detector dp geherstart.
 **************************************************************************/
void NaloopCVDet(count fc1, count fc2, count tnl, ...)
{
	va_list argpt;
	count dp;

	va_start(argpt, tnl);
	RT[tnl] = FALSE;
	while ((dp = va_arg(argpt, va_count)) != END)
	{
		RT[tnl] |= D[dp] && CV[fc1] && !RA[fc1];
	}
	va_end(argpt);
	if (RT[tnl] || T[tnl]) RW[fc2] |= BIT2;
    berekenTNL(fc2,tnl);
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
   if (RT[tnl]||!GKC[fc1]) berekenTGK_max(fc1,fc2,T_max[tnl]);
   if (G[fc1] && !RT[tnl]) berekenTGK_max(fc1,fc2,(T_max[tnl]-T_timer[tnl]));
   if (EG[fc1]) TNL_TGK[tnl]= T_max[tnl]-T_timer[tnl];
   if (!G[fc1] && GKC[fc1]) berekenTGK_max(fc1,fc2,TNL_TGK[tnl]);
   berekenTNL(fc2,tnl);
}
/**************************************************************************
 *  Functie  : NaloopFGDET
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 tijdens vastgroen fc1
 *    Nalooptijd wordt geherstart tijdens FG aanvoerrichting en wordt gedurende
 *    het bezet zijn van detector dp geherstart.
 **************************************************************************/

void NaloopFGDet(count fc1, count fc2, count tnl, ...)
{
	va_list argpt;
	count dp;

	va_start(argpt, tnl);
	RT[tnl] = FALSE;
	while ((dp = va_arg(argpt, va_count)) != END)
	{
		RT[tnl] |= (VS[fc1] || FG[fc1]) && D[dp] ;
	}
	va_end(argpt);
    if (RT[tnl] || T[tnl])   RW[fc2] |= BIT2;
    if (VS[fc1] || FG[fc1] || !GKC[fc1]) berekenTGK_max(fc1,fc2,T_max[tnl]);
	if (G[fc1] && !(VS[fc1] || FG[fc1])) berekenTGK_max(fc1,fc2,T_max[tnl]-T_timer[tnl]);
	if (EG[fc1]) TNL_TGK[tnl]= T_max[tnl]-T_timer[tnl];
	if (!G[fc1] && GKC[fc1]) berekenTGK_max(fc1,fc2,TNL_TGK[tnl]);
	berekenTNL(fc2,tnl);
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

/**************************************************************************
 *  Functie  : gk_NaloopDet
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 afhankelijk van detectie dp
 *    Nalooptijd geldt vanaf startgroen aanvoerrichting en wordt gedurende
 *    het bezet zijn van detector dp gereset.
 **************************************************************************/
/*void gk_NaloopDet(count fc1, count fc2, count dp, count tnl, mulv tgl_nl)
{
  RT[tnl] = D[dp] && (G[fc1] || GL[fc1] && TGL_timer[fc1] < tgl_nl);
  if (GL[fc1] && T[tnl] &&
      T_timer[tnl] < TGL_timer[fc1])
  {
    T_timer[tnl] = TGL_timer[fc1];
  }
  if (EGL[fc1] && T[tnl] &&
      T_timer[tnl] < TGL_max[fc1])
  {
    T_timer[tnl] = TGL_max[fc1];
  }
  if (RT[tnl] || T[tnl] ||
      MK[fc1] && (G[fc1] || GL[fc1] && TGL_timer[fc1]<tgl_nl)) YV[fc2] |= BIT2;

  gk_NaloopTGK(fc1,fc2,tnl,TRUE,tgl_nl);
  gk_NaloopTNL(fc1,fc2,tnl,TRUE,tgl_nl);
}

/**************************************************************************
 *  Functie  : gk_NaloopVtg
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 afhankelijk van gebruikte dk
 *    Nalooptijd geldt vanaf startgroen aanvoerrichting en duurt tnl lang.
 **************************************************************************/
/*void gk_NaloopVtg(count fc1, count fc2, count dk, count hdk, count tnl)
{
  if (EG[fc1])
  {
    IH[hdk] = FALSE;
  }
  IH[hdk] |= D[dk] && !G[fc1] && A[fc1];

  if (!RA[fc1])
  {
    RT[tnl] = FALSE;
  }

  RT[tnl] |= RA[fc1] && IH[hdk];
  AT[tnl]  = ERA[fc1] && R[fc1];

  if (RT[tnl])
  {
    RW[fc2] |= BIT2;
  }

  if (RT[tnl] || T[tnl])
  {
    YV[fc2] |= BIT2;
  }

  if (G[fc2] && !VG[fc2] && !MG[fc2])
  {
    TVGTMP_max[fc2] = T_max[tnl] - T_timer[tnl] - (TFG_max[fc2] - TFG_timer[fc2]);
  }
  else
  {
    if (!G[fc2])
    {
      TVGTMP_max[fc2] = T_max[tnl] - TFG_max[fc2];
    }
  }

  if ((RT[tnl] || T[tnl]) && (TVG_max[fc2] == 0))
  {
    TVG_max[fc2] += TVGTMP_max[fc2];
  }
  gk_NaloopTGK(fc1,fc2,tnl,FALSE,0);
  gk_NaloopTNL(fc1,fc2,tnl,FALSE,0);
}

/**************************************************************************
 *  Functie  : gk_NaloopTGK
 *
 *  Functionele omschrijving :
 *    Zorgt bij een naloop van fc1 naar fc2 voor een juiste waarde van de
 *    groengroenkonflikttimers TGK_max[fc1][k] van de toevoer fc1 naar
 *    alle konflikten k van de naloop fc2.
 *    Een groengroenkonflikttimer TGK_max[fc1][k] begint te lopen op het
 *    eindgroen van fc1 en bereikt de eindwaarde op het startgroen van k.
 *    Groengroenkonflikttimers dienen voor de bepaling van het afkapmoment
 *    van een toevoer bij een ingreep op een konflikt van de bijbehorende
 *    naloop.
 **************************************************************************/
/*
void gk_NaloopTGK(count fc1, count fc2, count tnl, bool per_herstart, count tgl_nl)
{
  int i;
  count k;
  int tnl_max;

  if (per_herstart || !GKC[fc1])
  {
    tnl_max = T_max[tnl] + tgl_nl;
    for (i=KFC_MAX[fc1];i<GKFC_MAX[fc1];i++)
    {
      k=TO_pointer[fc1][i];
      if (TO_max[fc1][k]==GKL &&
          TO_max[fc2][k]>=0 &&
          ((tnl_max + TGL_max[fc2] + TO_max[fc2][k]) > TGK_max[fc1][k]))
      {
        TGK_max[fc1][k] = tnl_max + TGL_max[fc2] + TO_max[fc2][k];
      }
    }
  }
  if (!per_herstart && (G[fc1] ||  per_herstart_old[tnl])) 
  {
	tnl_max = T_max[tnl] - T_timer[tnl] + tgl_nl;
    for (i=KFC_MAX[fc1];i<GKFC_MAX[fc1];i++)
    {
      k=TO_pointer[fc1][i];
      if (TO_max[fc1][k]==GKL &&
          TO_max[fc2][k]>=0 &&
          ((tnl_max + TGL_max[fc2] + TO_max[fc2][k]) > TGK_max[fc1][k]))
      {
        TGK_max[fc1][k] = tnl_max + TGL_max[fc2] + TO_max[fc2][k];
      }
    }
  }
  if (!per_herstart && !(G[fc1] || per_herstart_old[tnl])) 
  {
    for (i=KFC_MAX[fc1];i<GKFC_MAX[fc1];i++)
    {
      k=TO_pointer[fc1][i];
      if (TO_max[fc1][k]==GKL &&
          TO_max[fc2][k]>=0 &&
          (TGK_max_old[fc1][k] > TGK_max[fc1][k]))
      {
        TGK_max[fc1][k] = TGK_max_old[fc1][k];
      }
    }
  }
  
   
  per_herstart_old[tnl] = per_herstart;
  xyprintf(0,20,"per_herstart_old[tnlegd0363] = %2d",per_herstart_old[tnlegd0363]);
}
*/
/**************************************************************************
 *  Functie  : gk_NaloopTNL
 *
 *  Functionele omschrijving :
 *    Zorgt bij een naloop van fc1 naar fc2 voor een juiste waarde van de
 *    nalooptimer TNL_max[fc2] van de naloop fc2.
 *    Een nalooptimer TNL_max[fc2] geeft aan wat de maximale nalooptijd
 *    is die nog loopt op de naloop fc2.
 *    Nalooptimers voorkomen dat een naloop wordt afgekapt door een ingreep
 *    op een konflikt van die naloop.
 **************************************************************************/
void berekenTNL(count fc2, count tnl)
{
  if (T[tnl])
  {
    TNL[fc2]=TRUE;
    if (T_max[tnl]-T_timer[tnl]>TNL_max[fc2]-TNL_timer[fc2])
    {
      TNL_max[fc2]=T_max[tnl] + TVG_max[fc2];
      TNL_timer[fc2]=T_timer[tnl];
    }
  }
}

/**************************************************************************
 *  Functie  : gk_NaloopSG
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 vanaf startgroen fc1
 *    Nalooptijd wordt geherstart tijdens RA aanvoerrichting
 *    Nalooprichting wordt tevens tijdens groen van startrichting vastgehouden.
 **************************************************************************/
/*void gk_NaloopSG(count fc1, count fc2, count tnl)
{
  RT[tnl] = RA[fc1];
  if (RT[tnl] || T[tnl] || G[fc1]) YV[fc2] |= BIT2;

  gk_NaloopTGK(fc1,fc2,tnl,FALSE,0);
  gk_NaloopTNL(fc1,fc2,tnl,FALSE,0);
}

/**************************************************************************
 *  Functie  : gk_NaloopEG
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 vanaf eindgroen fc1
 *    Nalooptijd wordt geherstart tijdens G aanvoerrichting
 **************************************************************************/
/*void gk_NaloopEG(count fc1, count fc2, count tnl)
{
  RT[tnl] = G[fc1];
  if (RT[tnl] || T[tnl])   YV[fc2] |= BIT2;

  gk_NaloopTGK(fc1,fc2,tnl,TRUE,0);
  gk_NaloopTNL(fc1,fc2,tnl,TRUE,0);
}

/**************************************************************************
 * Functie  : gk_InitGK
 *
 * Functionele omschrijving :
 *   Het initialiseren van de groengroenkonflikttimers.
 **************************************************************************/
void gk_InitGK(void)
{
  count fc1, fc2;

  for (fc1=0;fc1<FC_MAX;fc1++)
  {
    GKC[fc1]=0;
    TGK_timer[fc1]=0;
    for (fc2=0;fc2<FC_MAX;fc2++)
    {
      TGK_max[fc1][fc2]=0;
      TGK[fc1][fc2]=FALSE;
    }
  }
}

/**************************************************************************
 * Functie  : gk_ResetGK
 *
 * Functionele omschrijving :
 *   Het resetten van de groengroenkonflikttimers.
 **************************************************************************/
void gk_ResetGK(void)
{
  int i;
  count fc, k;

  for (fc=0;fc<FC_MAX;fc++)
  {
    for (i=KFC_MAX[fc];i<GKFC_MAX[fc];i++)
    {
      k = TO_pointer[fc][i];
	  TGK_max_old[fc][k]=TGK_max[fc][k];
	  TGK_max[fc][k]=0;
    }
  }
}

/**************************************************************************
 * Functie  : gk_InitNL
 *
 * Functionele omschrijving :
 *   Het initialiseren van de nalooptimers.
 **************************************************************************/
void gk_InitNL(void)
{
  count fc;

  for (fc=0;fc<FC_MAX;fc++)
  {
    TNL_max[fc]=0;
    TNL[fc]=FALSE;
    TNL_timer[fc]=0;
  }
}

/**************************************************************************
 * Functie  : gk_ResetNL
 *
 * Functionele omschrijving :
 *   Het resetten van de nalooptimers.
 **************************************************************************/
void gk_ResetNL(void)
{
  count fc;

  for (fc=0;fc<FC_MAX;fc++)
  {
    TNL_max[fc]=0;
    TNL[fc]=FALSE;
    TNL_timer[fc]=0;
  }
}

/**************************************************************************
 * Functie  : gk_ControlGK
 *
 * Functionele omschrijving :
 *   Het bijwerken van de groengroenkonfliktvariabelen.
 **************************************************************************/
void gk_ControlGK(void)
{
  int i;
  count fc,k;

  for (fc=0;fc<FC_MAX;fc++)
  {
    if (SG[fc])
    {
      GKC[fc]=0;
      TGK_timer[fc]=0;
      for (i=KFC_MAX[fc];i<GKFC_MAX[fc];i++)
      {
        k=TO_pointer[fc][i];
        if (TGK_max[fc][k]>0)
        {
          TGK[fc][k]=TRUE;
          GKC[fc]++;
        }
      }
    }
    if (!G[fc] && GKC[fc]>0 && TE>0)
    {
      TGK_timer[fc]+=TE;
      for (i=KFC_MAX[fc];i<GKFC_MAX[fc];i++)
      {
        k=TO_pointer[fc][i];
        if (TGK[fc][k] && TGK_timer[fc]>=TGK_max[fc][k])
        {
          TGK[fc][k]=FALSE;
          GKC[fc]--;
        }
      }
    }
  }
}

void berekenTGK_max(count fc1, count fc2, count tnl_max)
{
	int i,k;
    for (i=KFC_MAX[fc1];i<GKFC_MAX[fc1];i++)
    {
      k=TO_pointer[fc1][i];
      if (TO_max[fc1][k]==GKL &&
          TO_max[fc2][k]>=0 &&
          ((tnl_max + TVG_max[fc2] + TGL_max[fc2] + TO_max[fc2][k]) > TGK_max[fc1][k]))
      {
        TGK_max[fc1][k] = tnl_max + TVG_max[fc2] + TGL_max[fc2] + TO_max[fc2][k];
      }
      if (TO_max[fc1][k]==GKL &&
          TO_max[fc2][k]==GK &&
          ((tnl_max + TVG_max[fc2]) > TGK_max[fc1][k]))
      {
        TGK_max[fc1][k] = tnl_max + TVG_max[fc2];
      }
    }
}
