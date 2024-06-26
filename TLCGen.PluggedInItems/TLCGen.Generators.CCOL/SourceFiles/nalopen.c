#include "nalopen.h"
#include "gkvar.h"
#include "nlvar.h"
mulv TNL_TGK[TMMAX];

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
   if (RT[tnl] || T[tnl])   RW[fc2] |= BIT2;
   berekenTNL(fc2,tnl);
   if (TNL_PAR[fc2] < T_max[tnl]) TNL_PAR[fc2] = T_max[tnl]; 

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
   if (RT[tnl] || T[tnl])   RW[fc2] |= BIT2;
   berekenTNL(fc2,tnl);
   if (TNL_PAR[fc2] < T_max[tnl]) TNL_PAR[fc2] = T_max[tnl]; 
}


/**************************************************************************
 *  Functie  : NaloopEG
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 vanaf eindgroen fc1
 *    Nalooptijd wordt geherstart tijdens G aanvoerrichting
 *    Naloop wordt tevens vastgehouden tijdens RA van een voedende richting
 **************************************************************************/
void NaloopEG(count fc1, count fc2, count tnl)
{
   RT[tnl] = G[fc1];
   if (RA[fc1])             RW[fc2] |= BIT2;
   if (RT[tnl] || T[tnl])   RW[fc2] |= BIT2;
   berekenTGK_max(fc1,fc2,T_max[tnl]);
   berekenTNL(fc2,tnl);
   if (TNL_PAR[fc2] < (T_max[tnl]/* + TVG_max[fc2]*/)) TNL_PAR[fc2] = T_max[tnl] /* + TVG_max[fc2]*/;
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
		RT[tnl] |= D[dp] && G[fc1] || ED[dp] && GL[fc1];
	}
	va_end(argpt);
	if (RT[tnl] || T[tnl] || G[fc1] || GL[fc1]) RW[fc2] |= BIT2;
	if (G[fc1] || GL[fc1] || !GKC[fc1]) berekenTGK_max(fc1, fc2, T_max[tnl] + TGL_max[fc1]);
	if (EGL[fc1]) TNL_TGK[tnl] = T_max[tnl] - T_timer[tnl] + TGL_max[fc1];
	if (R[fc1] && GKC[fc1]) berekenTGK_max(fc1, fc2, TNL_TGK[tnl]);
	berekenTNL(fc2, tnl);
    if (TNL_PAR[fc2] < (TGL_max[fc1] + T_max[tnl] /* + TVG_max[fc2]*/)) TNL_PAR[fc2] = TGL_max[fc1] + T_max[tnl] /* + TVG_max[fc2]*/;

}


/**************************************************************************
 *  Functie  : NaloopCV
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 vanaf eindgroen fc1
 *    Nalooptijd wordt geherstart tijdens cyclisch verlenggroen van de aanvoerrichting
 **************************************************************************/
void NaloopCV(count fc1, count fc2, count tnl)
{
	RT[tnl] = CV[fc1] && !RA[fc1];
	if (RT[tnl] || T[tnl]) RW[fc2] |= BIT2;
	berekenTNL(fc2, tnl);
    if (TNL_PAR[fc2] < T_max[tnl]) TNL_PAR[fc2] = T_max[tnl]; 
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
	berekenTNL(fc2, tnl);
    if (TNL_PAR[fc2] < T_max[tnl]) TNL_PAR[fc2] = T_max[tnl]; 

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
	if (RT[tnl] || !GKC[fc1]) berekenTGK_max(fc1, fc2, T_max[tnl]);
	if (G[fc1] && !RT[tnl]) berekenTGK_max(fc1, fc2, (T_max[tnl] - T_timer[tnl]));
	if (EG[fc1]) TNL_TGK[tnl] = T_max[tnl] - T_timer[tnl];
	if (!G[fc1] && GKC[fc1]) berekenTGK_max(fc1, fc2, TNL_TGK[tnl]);
	berekenTNL(fc2, tnl);
   if (TNL_PAR[fc2] < (T_max[tnl] /* + TVG_max[fc2]*/)) TNL_PAR[fc2] = T_max[tnl] /* + TVG_max[fc2]*/;
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
		RT[tnl] |= (VS[fc1] || FG[fc1]) && D[dp];
	}
	va_end(argpt);
	if (RT[tnl] || T[tnl])   RW[fc2] |= BIT2;
	if (VS[fc1] || FG[fc1] || !GKC[fc1]) berekenTGK_max(fc1, fc2, T_max[tnl]);
	if (G[fc1] && !(VS[fc1] || FG[fc1])) berekenTGK_max(fc1, fc2, T_max[tnl] - T_timer[tnl]);
	if (EG[fc1]) TNL_TGK[tnl] = T_max[tnl] - T_timer[tnl];
	if (!G[fc1] && GKC[fc1]) berekenTGK_max(fc1, fc2, TNL_TGK[tnl]);
	berekenTNL(fc2, tnl);
    if (TNL_PAR[fc2] < (TGL_max[fc1] + T_max[tnl] /* + TVG_max[fc2]*/)) TNL_PAR[fc2] = TGL_max[fc1] + T_max[tnl] /* + TVG_max[fc2]*/;
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
	register count hml = ml + 1;		/* next module			*/

	if (hml >= ml_max)  hml = ML1;		/* first module			*/
	for (i = 0; i < FC_MAX; ++i)
	{
		if ((prml[ml][i] & PRIMAIR_VERSNELD) && !prml[hml][i] &&
			(PR[i] && CV[i] && !(WG[i] && WS[i] && (RW[i] || YW[i])) && !(WG[i] && (RW[i] & BIT2))))
			return (TRUE);
	}
	return (FALSE);
}

#endif

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
	if (T[tnl]|| ET[tnl] || VG[fc2])
	{
		TNL[fc2] = TRUE;
		if ((T_max[tnl] - T_timer[tnl] /* + TVG_max[fc2]*/) > (TNL_max[fc2] - TNL_timer[fc2]))
		{
			TNL_max[fc2] = T_max[tnl] /* + TVG_max[fc2]*/;
			TNL_timer[fc2] = T_timer[tnl] + TVG_timer[fc2];
		}
	}
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

	for (fc1 = 0; fc1 < FC_MAX; fc1++)
	{
		GKC[fc1] = 0;
		TGK_timer[fc1] = 0;
		for (fc2 = 0; fc2 < FC_MAX; fc2++)
		{
			TGK_max[fc1][fc2] = 0;
			TGK[fc1][fc2] = FALSE;
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

	for (fc = 0; fc < FC_MAX; ++fc)
	{
		for (i = KFC_MAX[fc]; i < GKFC_MAX[fc]; ++i)
		{
#if (CCOL_V >= 95)
			k = KF_pointer[fc][i];
#else
			k = TO_pointer[fc][i];
#endif
			TGK_max_old[fc][k] = TGK_max[fc][k];
			TGK_max[fc][k] = 0;
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

	for (fc = 0; fc < FC_MAX; ++fc)
	{
		TNL_max[fc] = 0;
		TNL[fc] = FALSE;
		TNL_timer[fc] = 0;
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

	for (fc = 0; fc < FC_MAX; ++fc)
	{
		TNL_PAR[fc] = 0;
		TNL_max[fc] = 0;
		TNL[fc] = FALSE;
		TNL_timer[fc] = 0;
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
	count fc, k;

	for (fc = 0; fc < FC_MAX; ++fc)
	{
		if (SG[fc])
		{
			GKC[fc] = 0;
			TGK_timer[fc] = 0;
			for (i = KFC_MAX[fc]; i < GKFC_MAX[fc]; ++i)
			{
#if (CCOL_V >= 95)
				k = KF_pointer[fc][i];
#else
				k = TO_pointer[fc][i];
#endif
				if (TGK_max[fc][k] > 0)
				{
					TGK[fc][k] = TRUE;
					++GKC[fc];
				}
			}
		}
		if (!G[fc] && GKC[fc] > 0 && TE > 0)
		{
			TGK_timer[fc] += TE;
			for (i = KFC_MAX[fc]; i < GKFC_MAX[fc]; ++i)
			{
#if (CCOL_V >= 95)
				k = KF_pointer[fc][i];
#else
				k = TO_pointer[fc][i];
#endif
				if (TGK[fc][k] && TGK_timer[fc] >= TGK_max[fc][k])
				{
					TGK[fc][k] = FALSE;
					--GKC[fc];
				}
			}
		}
	}
}

void berekenTGK_max(count fc1, count fc2, mulv tnl_max)
{
	int i, k;
	for (i = KFC_MAX[fc1]; i < GKFC_MAX[fc1]; ++i)
	{
#if (CCOL_V >= 95)
		k = KF_pointer[fc1][i];
#else
		k = TO_pointer[fc1][i];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
		if (TIG_max[fc1][k] == GKL &&
			TIG_max[fc2][k] >= 0 &&
			((tnl_max /* + TVG_max[fc2]*/ + TIG_max[fc2][k]) > TGK_max[fc1][k]))
		{
			TGK_max[fc1][k] = tnl_max /* + TVG_max[fc2]*/ + TIG_max[fc2][k];
		}
		if (TIG_max[fc1][k] == GKL &&
			TIG_max[fc2][k] == GK &&
			((tnl_max /* + TVG_max[fc2]*/) > TGK_max[fc1][k]))
#else
		if (TO_max[fc1][k] == GKL &&
			TO_max[fc2][k] >= 0 &&
			((tnl_max /* + TVG_max[fc2]*/ + TGL_max[fc2] + TO_max[fc2][k]) > TGK_max[fc1][k]))
		{
			TGK_max[fc1][k] = tnl_max /* + TVG_max[fc2]*/  + TGL_max[fc2] + TO_max[fc2][k];
		}
		if (TO_max[fc1][k] == GKL &&
			TO_max[fc2][k] == GK &&
			((tnl_max /* + TVG_max[fc2]*/) > TGK_max[fc1][k]))
#endif
		{
			TGK_max[fc1][k] = tnl_max /* + TVG_max[fc2]*/;
		}
	}
}
