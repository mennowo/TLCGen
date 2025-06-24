#include "extra_func.h"

#ifdef REALFUNC
#include "realfunc.h"
#endif

mulv Knipper_1Hz = 0;
mulv Knipper_2Hz = 0;

/** ------------------------------------------------------------------------------
    MAXIMAAL MEEVERLENGEN
    ------------------------------------------------------------------------------
    Versie  Datum       Wie     Commentaar                      Vastgesteld in CO
    ------  ----------  ---     ----------                      -----------------
    1.0   12-03-2012  psn     basis                            23-10-2012
    ------------------------------------------------------------------------------
    Dit is een aangepaste versie van de standaard generator functie ym_max().
    Functie ym_max() wordt gebruikt bij de specificatie van de instructie-
    variabele YM[] (vasthouden meeverlenggroen) van de fasecyclus.
    De functie ym_max() blijft waar (TRUE) zolang de fasecyclus kan meeverlengen
    (met in achtname van ontruimingstijdverschillen).
    
    Ten opzichte van de functie uit de generator is het verschil:
    - De functie kijkt in plaats van alleen naar RA[], ook naar AAPR[]
    ------------------------------------------------------------------------------ */

bool ym_maxV1(count i, mulv to_verschil)
{
	register count n, j, k, m;
	bool ym;

	if (MG[i])
	{   /* let op! i.v.m. snelheid alleen in MG[] behandeld */
		ym = TRUE;
#ifndef NO_GGCONFLICT
		for (n = 0; n<GKFC_MAX[i]; ++n)
#else
		for (n = 0; n<KFC_MAX[i]; ++n)
#endif
		{
#if (CCOL_V >= 95)
			k = KF_pointer[i][n];
#else
			k = TO_pointer[i][n];
#endif
			if (RA[k] || AAPR[k])
			{
				ym = FALSE;
#ifndef NO_GGCONFLICT
#if (CCOL_V >= 95) && !defined NO_TIGMAX
				if (TIG_max[i][k] < GK)  break;
#else
				if (TO_max[i][k] < GK)  break;
#endif
#endif
				for (j = 0; j<KFC_MAX[k]; ++j)
				{
#if (CCOL_V >= 95)
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
				    if (CV[m] && (((TIG_max[i][k] - to_verschil) <= TIG_max[m][k])
#else
					if (CV[m] && (((TO_max[i][k] - to_verschil) <= TO_max[m][k])
#endif
						|| (to_verschil<0)))
					{
						ym = TRUE;
						break;
					}
				}
#ifndef NO_GGCONFLICT
				for (j = KFC_MAX[k]; j < GKFC_MAX[k]; ++j)
				{
#if (CCOL_V >= 95)
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
					if (CV[m] && (TIG_max[m][k] == GKL || TIG_max[i][k] <= GK))
#else
			        if (CV[m] && (TO_max[m][k] == GKL || TO_max[i][k] <= GK))
#endif
					{
						ym = TRUE;
						break;
					}
				}
#endif
			}
			if (!ym) break;
		}
	}
	else ym = FALSE;
	return ym;
}

/* Aangepaste functie ym_maxv1 met aanpassingen tbv meeverlengen met voetgangersnalopen */
bool ym_maxV2(count i, mulv to_verschil)
{
   register count n, j, k, m;
   bool ym;

   if (MG[i])
   {   /* let op! i.v.m. snelheid alleen in MG[] behandeld */
      ym = TRUE;
#ifndef NO_GGCONFLICT
      for (n = 0; n < FKFC_MAX[i]; ++n)
#else
      for (n = 0; n < KFC_MAX[i]; ++n)
#endif
      {
#if (CCOL_V >= 95)
	 k = KF_pointer[i][n];
#else
	 k = TO_pointer[i][n];
#endif
	 if (RA[k] || AAPR[k] || (CALW[k] >= PRI_CALW))
	 {
	    ym = FALSE;
#ifndef NO_GGCONFLICT
#if (CCOL_V >= 95) && !defined NO_TIGMAX
	    if (TIG_max[i][k] < GK)  break;
#else
	    if (TO_max[i][k] < GK)  break;
#endif
#endif
	    for (j = 0; j < KFC_MAX[k]; ++j)
	    {
#if (CCOL_V >= 95)
	       m = KF_pointer[k][j];
#else
	       m = TO_pointer[k][j];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
	       if (CV[m] && (!(RW[m] & BIT2) || US_type[m] == VTG_type) && (((TIG_max[i][k] - to_verschil) <= TIG_max[m][k])
#else
	       if (CV[m] && (!(RW[m] & BIT2) || US_type[m] == VTG_type) && (((TO_max[i][k] - to_verschil) <= TO_max[m][k])
#endif
		  || (to_verschil < 0) && (!(RW[m] & BIT2) || US_type[m] == VTG_type)))
	       {
		  ym = TRUE;
		  break;
	       }
	    }
#ifndef NO_GGCONFLICT
	    for (j = KFC_MAX[k]; j < GKFC_MAX[k]; ++j)
	    {
#if (CCOL_V >= 95)
	       m = KF_pointer[k][j];
#else
	       m = TO_pointer[k][j];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
	       if (CV[m] && (!(RW[m] & BIT2) || US_type[m] == VTG_type) && (TIG_max[m][k] <= GK))
#else
	       if (CV[m] && (!(RW[m] & BIT2) || US_type[m] == VTG_type) && (TO_max[m][k] <= GK))
#endif
	       {
		  ym = TRUE;
		  break;
	       }
	    }
#endif
	 }
	 if (!ym) break;
      }
   }
   else ym = FALSE;
   return ym;
}

bool ym_max_prmV1(count i, count prm, mulv to_verschil)
{
	switch (PRM[prm]) 
	{
	case 0:
		return FALSE;
	case 1:
		return ym_maxV1(i, to_verschil);
	case 2:
		return ym_max_toV1(i, to_verschil);
	case 3:
		return ym_maxV1(i, to_verschil) || MK[i] && ym_max_toV1(i, to_verschil);
	case 4:
		return ym_max_vtgV1(i);
	case 5:
		return ym_max(i, to_verschil);
	case 6:
#if (CCOL_V >= 95) && !defined NO_TIGMAX
		return ym_max_tig(i, to_verschil);
#else
		return ym_max_to(i, to_verschil);
#endif
	case 7:
#if (CCOL_V >= 95) && !defined NO_TIGMAX
		return ym_max(i, to_verschil) || MK[i] && ym_max_tig(i, to_verschil);
#else
		return ym_max(i, to_verschil) || MK[i] && ym_max_to(i, to_verschil);
#endif
#ifdef REALFUNC
    case 8:
        return !Maatgevend_Groen(i);
#endif
    case 9:
       return ym_maxV2(i, to_verschil);
    case 10:
       return ym_max_toV2(i, to_verschil);
    case 11:
		return ym_maxV2(i, to_verschil) || MK[i] && ym_max_toV2(i, to_verschil);
    case 12:
       return ym_max_vtgV2(i);
	}
	return FALSE;
}

/* MAXIMUM MEEVERLENGGROEN */
/* ======================= */
/* ym_max_to() wordt gebruikt bij de specificatie van de instructie-
 * variabele YM[] (vasthouden meeverlenggroen) van de fasecyclus.
 * ym_max_to() blijft waar (TRUE) zolang de fasecyclus kan meeverlengen
 * (met in achtname van ontruimingstijdverschillen).
 * het aanroepen van de functie ym_max_to() dient in de applicatiefunctie
 * application() te worden gespecificeerd.
 * ym_max_to() blijft ook meeverlengen met MG[], GL[] en R[]
 * zolang het ontruimingstijd verschil voordelig is!!
 * tbv deze functie is in de file fcfunc.c TGL_timer[] al op SG[] op 0 gezet.
 *
 *  32  fffffflll
 *  05  ffffffmmmmmmmmxxx
 *  08                      ffffffvvvvvxxxxx
 *
 *  YM[fc05]= ym_max_to(fc05,0) || CIF_IS[is_fix];
 *  fc05 blijft meeverlengen met ontruimingstijd van fc32 naar fc08.
 *
 *  LET OP! Er is geen speciale ym_max_tigV1 tbv integroen, ym_max_toV1
 *  is ook geschikt voor integroen
 */
bool ym_max_toV1(count i, mulv to_verschil)
{
	register count n, j, k, m;
	bool ym;

	if (MG[i]) /* let op! i.v.m. snelheid alleen in MG[] behandeld	*/
	{
		ym = TRUE;
		for (n = 0; n < GKFC_MAX[i]; ++n)
		{
#if (CCOL_V >= 95)
			k = KF_pointer[i][n];
#else
			k = TO_pointer[i][n];
#endif
			if ((RA[k] || AAPR[k]))
			{
				ym = FALSE;
#if (CCOL_V >= 95) && !defined NO_TIGMAX
				if (TIG_max[i][k] < GK)  break;
#else
				if (TO_max[i][k] < GK)  break;
#endif
				for (j = 0; j < KFC_MAX[k]; ++j)
				{
#if (CCOL_V >= 95)
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif

#if (CCOL_V >= 95) && !defined NO_TIGMAX
					if (CV[m] && (((TIG_max[i][k] - to_verschil) <=
						(TIG_max[m][k])) &&
						((TIG_max[i][k]) < (TFG_max[m] - TFG_timer[m] +
							TVG_max[m] - TVG_timer[m] +
							TIG_max[m][k] - TIG_timer[m]))
						|| (to_verschil < 0))
						|| TIG[m][k]
						&& ((TIG_max[i][k]) < (TIG_max[m][k] - TIG_timer[m])))
#else
					if (CV[m] && (((TGL_max[i] + TO_max[i][k] - to_verschil) <=
						(TGL_max[m] + TO_max[m][k])) &&
						((TGL_max[i] + TO_max[i][k]) < (TFG_max[m] - TFG_timer[m] +
							TVG_max[m] - TVG_timer[m] + TGL_max[m] - TGL_timer[m] +
							TO_max[m][k] - TO_timer[m]))
						|| (to_verschil < 0))
						|| TO[m][k]
						&& ((TGL_max[i] + TO_max[i][k]) < (TGL_max[m] + TO_max[m][k] -
							TGL_timer[m] - TO_timer[m])))
#endif
					{
						ym = TRUE;
						break;
					}
				}

				for (j = KFC_MAX[k]; j < FKFC_MAX[k]; j++)
				{
#if (CCOL_V >= 95)
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
					if (CV[m] && (TIG_max[m][k] == GKL || TIG_max[i][k] <= GK))
#else
					if (CV[m] && (TO_max[m][k] == GKL || TO_max[i][k] <= GK))
#endif
					{
						ym = TRUE;
						break;
					}
				}
			}
			if (!ym) break;
		}
	}
	else  ym = CV[i];
	return ym;
}

/* Aangepaste functie ym_max_toV1 met aanpassingen tbv meeverlengen met voetgangersnalopen */
bool ym_max_toV2(count i, mulv to_verschil)
{
   register count n, j, k, m;
   bool ym;

   if (MG[i]) /* let op! i.v.m. snelheid alleen in MG[] behandeld	*/
   {
      ym = TRUE;
      for (n = 0; n < FKFC_MAX[i]; ++n)
      {
#if (CCOL_V >= 95)
	 k = KF_pointer[i][n];
#else
	 k = TO_pointer[i][n];
#endif
	 if (RA[k] || AAPR[k] || (CALW[k] >= PRI_CALW))

	 {
	    ym = FALSE;
#if (CCOL_V >= 95) && !defined NO_TIGMAX
	    if (TIG_max[i][k] <= GK)  break;
#else
	    if (TO_max[i][k] <= GK)
	    {
#ifndef NO_GGCONFLICT 
	       for (j = 0; j < FKFC_MAX[k]; ++j)
	       {
#if (CCOL_V >= 95)
		  m = KF_pointer[k][j];
#else
		  m = TO_pointer[k][j];
#endif
		  if (CV[m] && (!(RW[m] & BIT2) || US_type[m] == VTG_type) && (TO_max[m][k] <= GK || TO_max[i][k] <= GK))
		  {
		     //ym = TRUE;
		     if (!RA[m] && AAPR[k]) { ym = TRUE; break; }
		  }
	       }
#endif
	       //                    break;
	    }
#endif
	    for (j = 0; j < KFC_MAX[k]; ++j)
	    {
#if (CCOL_V >= 95)
	       m = KF_pointer[k][j];
#else
	       m = TO_pointer[k][j];
#endif

#if (CCOL_V >= 95) && !defined NO_TIGMAX
	       if (CV[m] && (!(RW[m] & BIT2) || US_type[m] == VTG_type) && (((TIG_max[i][k] - to_verschil) <=
		  (TIG_max[m][k])) &&
		  ((TIG_max[i][k]) < (TFG_max[m] - TFG_timer[m] +
		     TVG_max[m] - TVG_timer[m] +
		     TIG_max[m][k] - TIG_timer[m]))
		  || (to_verschil < 0) && (!(RW[m] & BIT2) || US_type[m] == VTG_type)/*JDV 161221*/)
		  || TIG[m][k] && (!(RW[m] & BIT2) || US_type[m] == VTG_type)/*JDV 161221*/
		  && ((TIG_max[i][k]) < (TIG_max[m][k] - TIG_timer[m])))
#else
	       if (CV[m] && (!(RW[m] & BIT2) || US_type[m] == VTG_type) && (((TGL_max[i] + TO_max[i][k] - to_verschil) <=
		  (TGL_max[m] + TO_max[m][k])) &&
		  ((TGL_max[i] + TO_max[i][k]) < (TFG_max[m] - TFG_timer[m] +
		     TVG_max[m] - TVG_timer[m] + TGL_max[m] - TGL_timer[m] +
		     TO_max[m][k] - TO_timer[m]))
		  || (to_verschil < 0) && (!(RW[m] & BIT2) || US_type[m] == VTG_type)/*JDV 161221*/)
		  || TO[m][k] && (!(RW[m] & BIT2) || US_type[m] == VTG_type)/*JDV 161221*/
		  && ((TGL_max[i] + TO_max[i][k]) < (TGL_max[m] + TO_max[m][k] -
		     TGL_timer[m] - TO_timer[m])))
#endif
	       {
		  ym = TRUE;
		  break;
	       }
	    }

	    for (j = KFC_MAX[k]; j < FKFC_MAX[k]; j++)
	    {
#if (CCOL_V >= 95)
	       m = KF_pointer[k][j];
#else
	       m = TO_pointer[k][j];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
	       if (CV[m] && (!(RW[m] & BIT2) || US_type[m] == VTG_type) && (TIG_max[m][k] <= FK || TIG_max[i][k] <= GK))
#else
	       if (CV[m] && (!(RW[m] & BIT2) || US_type[m] == VTG_type) && (TO_max[m][k] <= FK || TO_max[i][k] <= GK))
#endif
	       {
		  if (!RA[m] && AAPR[k]) 
		  { 
		     ym = TRUE; 
		     break; 
		  }

	       }
	    }
	 }
	 if (!ym) break;
      }
   }
   else  ym = CV[i];
   return ym;
}


/** ------------------------------------------------------------------------------
MAXIMAAL MEEVERLENGEN VOOR VOETGANGERS
------------------------------------------------------------------------------
Versie  Datum       Wie     Commentaar                      Vastgesteld in CO
------  ----------  ---     ----------                      -----------------
1.0   12-03-2012  psn     basis                            23-10-2012
------------------------------------------------------------------------------
Dit is een aangepast versie van de standaard generator functie ym_max().
Functie ym_max() wordt gebruikt bij de specificatie van de instructie-
variabele YM[] (vasthouden meeverlenggroen) van de fasecyclus.
De functie ym_max() blijft waar (TRUE) zolang de fasecyclus kan meeverlengen
(met in achtname van ontruimingstijdverschillen).

De functie is aangepast voor gebruik bij voetgangers richtingen.
Ten opzichte van de functie uit de generator is het verschil:
- De functie kijkt in plaats van alleen naar RA[], ook naar AAPR[]
- De functie kijkt of de maximale groentijd voor de fase is bereikt
t.o.v. de benodigde ontruiming van autorichtingen
------------------------------------------------------------------------------ */
bool ym_max_vtgV1(count i)
{
	register count n, j, k, m;
	bool ym;

	if (MG[i]) /* let op! i.v.m. snelheid alleen in MG[] behandeld  */
	{
		ym = TRUE;
		/* nalopen of nog moet worden meeverlengd */
#ifndef NO_GGCONFLICT
		for (n = 0; n < GKFC_MAX[i]; ++n)
#else
		for (n = 0; n < KFC_MAX[i]; ++n)
#endif
		{
#if (CCOL_V >= 95)
			k = KF_pointer[i][n];
#else
			k = TO_pointer[i][n];
#endif
			if (RA[k] || AAPR[k])
			{
				ym = FALSE;
#ifndef NO_GGCONFLICT
#if (CCOL_V >= 95) && !defined NO_TIGMAX
				if (TIG_max[i][k] < GK)  break;
#else
				if (TO_max[i][k] < GK)  break;
#endif
#endif
				for (j = 0; j < KFC_MAX[k]; ++j)
				{
#if (CCOL_V >= 95)
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
					if (FC_type[m] == MVT_type)
					{
						/* bereken of max groentijd is bereikt tov benodigde ontruiming */
						if (CV[m] && PR[m] && !(VG[m] &&
							((TVG_max[m] - TVG_timer[m]) <
#if (CCOL_V >= 95) && !defined NO_TIGMAX
							(TIG_max[i][k] - TIG_max[m][k]))) &&
							 !(WG[m] && (TVG_max[m] < (TIG_max[i][k] - TIG_max[m][k]))))
#else
							(TO_max[i][k] + TGL_max[i] - TO_max[m][k] - TGL_max[m]))) &&
							 !(WG[m] && (TVG_max[m] < (TO_max[i][k] + TGL_max[i] - TO_max[m][k] - TGL_max[m]))))
#endif
						{
							ym = TRUE;
							break;
						}
					}
					else
					{
						if (CV[m] && !VG[m] && !(WG[m] && (RW[m] & BIT2)))
						{
							ym = TRUE;
							break;
						}
					}
				}
#ifndef NO_GGCONFLICT
				for (j = KFC_MAX[k]; j < GKFC_MAX[k]; ++j)
				{
#if (CCOL_V >= 95)
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
					if (CV[m] && (TIG_max[m][k] == GKL))
#else
					if (CV[m] && (TO_max[m][k] == GKL))
#endif
					{
						ym = TRUE;
						break;
					}
				}
#endif
			}
			if (!ym)
				break;
		}
	}
	else ym = FALSE;
	return ym;
}

/* Aangepaste functie ym_max_vtgV1 met aanpassingen tbv meeverlengen met voetgangersnalopen */
bool ym_max_vtgV2(count i)
{
   register count n, j, k, m;
   bool ym;

   if (MG[i]) /* let op! i.v.m. snelheid alleen in MG[] behandeld  */
   {
      ym = TRUE;
      /* nalopen of nog moet worden meeverlengd */
#ifndef NO_GGCONFLICT
      for (n = 0; n < FKFC_MAX[i]; ++n)
#else
      for (n = 0; n < KKFC_MAX[i]; ++n)
#endif
      {
#if (CCOL_V >= 95)
	 k = KF_pointer[i][n];
#else
	 k = TO_pointer[i][n];
#endif
	 if (RA[k] || AAPR[k] || (CALW[k] >= PRI_CALW))
	 {
	    ym = FALSE;
#ifndef NO_GGCONFLICT
#if (CCOL_V >= 95) && !defined NO_TIGMAX
	    if (TIG_max[i][k] <= GK)  break;
#else
	    if (TO_max[i][k] <= GK)  break;
#endif
#endif
	    for (j = 0; j < KFC_MAX[k]; ++j)
	    {
#if (CCOL_V >= 95)
	       m = KF_pointer[k][j];
#else
	       m = TO_pointer[k][j];
#endif
	       if (FC_type[m] == MVT_type)
	       {
		  /* bereken of max groentijd is bereikt tov benodigde ontruiming */
		  if (CV[m] && PR[m] && !(VG[m] &&
		     ((TVG_max[m] - TVG_timer[m]) <
#if (CCOL_V >= 95) && !defined NO_TIGMAX
		     (TIG_max[i][k] - TIG_max[m][k]))) &&
		     !(WG[m] && (TVG_max[m] < (TIG_max[i][k] - TIG_max[m][k]))))
#else
		     (TO_max[i][k] + TGL_max[i] - TO_max[m][k] - TGL_max[m]))) &&
		     !(WG[m] && (TVG_max[m] < (TO_max[i][k] + TGL_max[i] - TO_max[m][k] - TGL_max[m]))))
#endif
		  {
		     ym = TRUE;
		     break;
		  }
	       }
	       else
	       {
		  if (CV[m] && !VG[m] && !(WG[m] && (RW[m] & BIT2)))
		  {
		     ym = TRUE;
		     break;
		  }
	       }
	    }
#ifndef NO_GGCONFLICT
	    for (j = KFC_MAX[k]; j < GKFC_MAX[k]; ++j)
	    {
#if (CCOL_V >= 95)
	       m = KF_pointer[k][j];
#else
	       m = TO_pointer[k][j];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
	       if (CV[m] && (TIG_max[m][k] == GKL))
#else
	       if (CV[m] && (TO_max[m][k] == GKL))
#endif
	       {
		  if (!RA[m] && AAPR[k]) 
		  { 
		     ym = TRUE; 
		     break; 
		  }
	       }
	    }
#endif
	 }
	 if (!ym)
	    break;
      }
   }
   else ym = FALSE;
   return ym;
}

void AanvraagSnelV2(count fc1, count dp)
{
   /* richting mag gelijk realiseren indien er geen conflicten lopen */
   if ((bool)(!kcv(fc1) && !K[fc1] && R[fc1] && !TRG[fc1] &&
      D[dp] && (CIF_IS[dp] <= CIF_DET_STORING)))
   {
      A[fc1] |= BIT5;
   }
}

/** ------------------------------------------------------------------------------
AANVRAAG SNEL
------------------------------------------------------------------------------
Versie  Datum       Wie     Commentaar                      Vastgesteld in CO
------  ----------  ---     ----------                      -----------------
1.0   12-03-2012    dze     basis                            06-12-2011
2.0   23-04-2012    psn     aangepaste syntax                23-10-2012
3.0   24-05-2024    nvw     alleen opzetten A                -
------------------------------------------------------------------------------
Functie voor het direct opzetten van een aanvraag.
Voertuig hoeft bezettijd niet af te wachten indien er geen conflicten lopen.

hierbij:
- fc:       fase (vaak een fiets)
- dp:       detector

Resultaat:   zet BIT4 op van A[fc]
------------------------------------------------------------------------------ */
void AanvraagSnelV3(count fc1, count dp)
{
   /* richting mag gelijk realiseren indien er geen conflicten lopen */
   if ((bool)(!kcv(fc1) && !K[fc1] && R[fc1] && !TRG[fc1] &&
      D[dp] && (CIF_IS[dp] <= CIF_DET_STORING)))
   {
      A[fc1] |= BIT5;
   }
}

/** ------------------------------------------------------------------------------
    ACOUSTISCHE SIGNALEN:   RATELTIKKERS
    ------------------------------------------------------------------------------
    Versie  Datum       Wie     Commentaar                      Vastgesteld in CO
    ------  ----------  ---     ----------                      -----------------
      1.0   21-11-2011  dze     basis                            06-12-2011
      2.0   03-01-2011  mvw     afzetten alleen als niet         23-10-2012
                                rateltikkers continu
      3.0   22-11-2012  dze     verbetering code                 xx-xx-xxxx
      4.0   19-04-2016  mvw     loslaten ACCROSS                 xx-xx-xxxx
      5.0   11-06-2019  ddo     aanpassing voor bewaakte tikker
    ------------------------------------------------------------------------------

    De rateltikkers voor voetgangers worden niet in het regeltoestel afgehandeld,
    maar middels units in de armaturen. Het aansturen van die units gebeurt
    middels een uitgang die direct wordt aangestuurd vanuit de applicatie.

    Vanuit de applicatie worden de rateltikkers voor voetgangers aangestuurd:
    a)  van begin melding drukknop (ook indien de aanvraag dan (nog) niet ontstaat!) 
        tot na aflopen timer tnlrt, die start bij het eerstvolgend einde groen 
        van de bijbehorende richting;
    b)  van begin melding (buiten) drukknop tot na aflopen timer tnlrt, die 
        start bij het eerstvolgend einde groen mits die drukknop een groene golf 
        naar de bijbehorende richting aanvraagt;
    c)  continu tijdens een aanwezige klokperiode (H [has_aan]);
    
    Het dimsignaal voor alle rateltikkers is aanwezig:
    a)  tijdens een aanwezige klokperiode (H[has_dim]);
    OF
    b)  het volume c.q. dimnivo wordt permanent vanuit de regelapplicatie uitgestuurd 
        (bij tikkers van het nieuwe (bewaakte) type). Hierbij is tevens het signaal 
        'rateltikkers aan' geinverteerd.
    ------------------------------------------------------------------------------ */
bool Rateltikkers(   count fc,        /* fase                                     */
                       count has,      /* hulpelement rateltikkers voor deze fase  */
                       count has_aan_, /* hulpelement tikkers werking              */
                       count has_cont_,/* hulpelement tikkers continu              */
                       count tnlrt,    /* tijd na EG dat de tikkers nog moeten worden aangestuurd indien niet continu */
                       bool bewaakt,  /* rateltikker van nieuwe (bewaakte) type?  */
                       ...)            /* hulpelementen drukknoppen                */
{
	va_list argpt;
	count hdkh;
	bool hdk = FALSE;

	/* verzorgen naloop rateltikker */
	RT[tnlrt] = (G[fc] || GL[fc]) && IH[has] || EH[has_cont_];

	/* check tikkers werking */
	if (IH[has_aan_])
	{
		va_start(argpt, bewaakt);
		while ((hdkh = va_arg(argpt, va_count)) != END)
		{
			/* opzetten rateltikkers bij detectie drukknoppen */
			IH[has] |= IH[hdkh];
			if (IH[hdkh]) hdk = TRUE;
		}
		va_end(argpt);
	}

    /* afzetten rateltikker na aflopen naloop timer (indien geen nieuwe drukknop melding) */
    if (ET[tnlrt] && !hdk) {
		IH[has] = FALSE;
    }
    
    /* continue aansturing rateltikkers */
	if (has_cont_ > NG) {
		IH[has] |= IH[has_cont_];
	}

  if ((bewaakt == FALSE) || (bewaakt == NG)) {
    return (IH[has]); /* positieve uitsturing bij niet-bewaakte tikkers */
  }
  else {
    return (!IH[has]); /* geinverteerde uitsturing bij bewaakte tikkers */
  }
}

/** ------------------------------------------------------------------------------
    ACOUSTISCHE SIGNALEN:   RATELTIKKERS
    ------------------------------------------------------------------------------
    Versie  Datum       Wie     Commentaar                      Vastgesteld in CO
    ------  ----------  ---     ----------                      -----------------
    1.0   21-11-2011  dze     basis                            06-12-2011
    2.0   03-01-2011  mvw     afzetten alleen als niet         23-10-2012
    rateltikkers continu
    3.0   22-11-2012  dze     verbetering code                 xx-xx-xxxx
    ------------------------------------------------------------------------------
    
    De rateltikkers voor voetgangers worden in het regeltoestel afgehandeld via de
    Across-units. Deze units dragen zelf zorg voor dat de rateltikkers 'aan' blijven
    tijdens:
    1)  groen of groenknipperen van de aangesloten richting
    2)  lopen instelbare tijd (herstart tijdens groen en groenknipperen)
    
    Vanuit de applicatie worden de rateltikkers voor voetgangers aangestuurd:
    a)  van begin melding drukknop (ook indien de aanvraag dan (nog) niet ontstaat!)
    tot eerstvolgend begin groen van de bijbehorende richting;
    b)  van begin melding (buiten) drukknop tot eerstvolgend begin groen mits die
    drukknop een groene golf naar de bijbehorende richting aanvraagt;
    c)  continu tijdens een aanwezige klokperiode (MM [  mas_aan]);
    d)  continu tijdens een aanwezige schakelaar  (SCH[schas_aan]).
    Bij einde van een continue aanvraag wordt tijdens niet groen een aanvraag voor
    groen van de bijbehorende richting ingediend.
    
    Het dimsignaal voor alle rateltikkers is aanwezig:
    a)  tijdens een aanwezige klokperiode (MM [  mas_dim]);
    b)  tijdens een aanwezige schakelaar  (SCH[schas_dim]).
    ------------------------------------------------------------------------------ */
bool Rateltikkers_Accross(count fc,       /* fase */
	count has,      /* hulpelement rateltikkers voor deze fase */
	count has_aan_, /* hulpelement tikkers werking */
	count has_cont_,/* hulpelement tikkers continu */
	...)            /* drukknoppen */
{
	va_list argpt;
	count dkid;

	/* afzetten rateltikkers als niet continu aanvraag */
	if (G[fc])                                          IH[has] = FALSE;
	if (has_cont_ > NG)                                 IH[has] |= IH[has_cont_];

	/* check tikkers werking */
	if (IH[has_aan_])
	{
		va_start(argpt, has_cont_);
		while ((dkid = va_arg(argpt, va_count)) != END)
		{
			/* opzetten rateltikkers bij detectie drukknoppen */
			IH[has] |= IH[dkid];
		}
		va_end(argpt);
	}

	/* eenmalige aanvraag afzetten Across */
	if (has_cont_ > NG)
		if (EH[has_cont_])                              A[fc] |= !G[fc];

	return (IH[has]);
}

/* Rateltikkers Hoeflake waarbij het dimnivo vanuit de applicatie kan worden geregeld.      */
/* Code kan zowel booleaans dimsignaal als dimnivo verzorgen. Bij aansturing volume vanuit  */
/* de applicatie dient voor iedere tikker apart het dimnivo signaal uitgestuurd te worden.  */
/* aanroep:
   GUS[usrtdim32a] = RateltikkerDimming(fc32, hperiodrtdim, prmdimas32adag, prmdimas32anacht) -> dimnivo via appl.
   GUS[usrtdim32b] = RateltikkerDimming(fc32, hperiodrtdim, prmdimas32bdag, prmdimas32bnacht) -> dimnivo via appl.
   (bij twee tikker units voor fc32)
   of
   GUS[usrtdim32] = RateltikkerDimming(fc32, hperiodrtdim, NG, NG) -> volume ingesteld in tikker unit.
*/
bool Rateltikkers_HoeflakeDimming(
                         count hperasdim, /* hulpelement klokperiode gedimde uitsturing     */
                         count prmasndim, /* dimnivo periode niet dimmen (0-10, 10 = tikker uit) of NG  */ 
                         count prmasdim)  /* dimnivo periode dimmen (0-10, 10 = tikker uit) of NG  */
{
  bool uitsturing = FALSE; /* uitsturing (kan boolean dimsignaal of dimnivo blokgolf zijn) */
  int dimblokgolf = CIF_KLOK[CIF_SECONDE] - (CIF_KLOK[CIF_SECONDE] / 10 * 10 - 1);

  /* bepaal wijze van uitsturen */  
  if ((prmasndim > NG) && (prmasdim > NG)) /* dimnivo door regelapplicatie bepaald */
  {
    /* bepaal uitsturing dimnivo; 
    /* hoe HOGER de waarde van de PRM (0 .. 10), hoe LAGER het volume */
    if (IH[hperasdim]) {
      if (dimblokgolf <= PRM[prmasdim])  uitsturing = TRUE;
    }
    else {
      if (dimblokgolf <= PRM[prmasndim])    uitsturing = TRUE;
    }
  }
  else uitsturing = IH[hperasdim]; /* volume door tikker bepaald */
 
  return uitsturing;
}

/** ------------------------------------------------------------------------------
    EERLIJK DOSEREN BIJ FILE
    ------------------------------------------------------------------------------
    Versie  Datum       Wie     Commentaar                      Vastgesteld in CO
    ------  ----------  ---     ----------                      -----------------
      1.0   25-02-2013  mvw     basis                            xx-xx-xxxx
    ------------------------------------------------------------------------------
    Deze functie is bedoeld voor gebruik in combinatie met Filemelding(). Wanneer
    vanwege een filemelding wordt gedoseerd, houdt deze functie bij hoeveel is
    gedoseerd per betreffende richting. Is de file voorbij, dan wordt een
    eventuele ongelijkheid in de mate van doseren gelijk getrokken. Zodanig
    krijgt elke richting uiteindelijk een even grote dose.
    Deze functie maakt gebruik van een aantal static int/short variabelen die
    buiten de functie gedeclareerd moeten worden.
    De functie gaat uit van 4 klokperioden, waarvan de parameters via het argument
    'count fcmg[][4]' aan de functie worden doorgegeven.
    ------------------------------------------------------------------------------ */
void Eerlijk_doseren_V1(count hfile,              /* hulpelement wel/geen file                         */
                        count _prmperc,           /* indexnummer parameter % doseren                   */
                        count aantalfc,           /* aantal te doseren fasen                           */
                        count fc[],               /* pointer naar array met fasenummers                */
                        count fcmg[][MPERIODMAX], /* pointer naar array met mg parameter index nummers */
                        int nogtedoseren[],       /* pointer naar array met nog te doseren waarden     */
	                    bool *prml[],
	                    count ml,
						count _mperiod)
{
    int i, j, laatstedosering;

    /* Voor elke fase mbt dit filegebied */
    for(i = 0; i < aantalfc; ++i)
    {
        /* Als het file is, eindegroen, en het meetkriterium staat op */
        if(EG[fc[i]] && H[hfile] && MK[fc[i]])
        {
            /* Uitrekenen laatst toegepaste dosering */
            laatstedosering = 100 - (int)(100.0 * (((float)(TFG_max[fc[i]] + TVG_timer[fc[i]])) / ((float)(PRM[fcmg[i][MM[_mperiod]]]))));
            /* Voor elke fase mbt het fileveld */
            for(j = 0; j < aantalfc; ++j)
            {
                /* Die niet de fase is die nu EG heeft */
                if(i == j)continue;
                /* uitrekenen nogtedoseren waarde voor de andere richtingen:
                   (tbv programmeertechnische veiligheid van het algoritme: 
                    alleen doen wanneer de uitkomst lager is dan het file doseer percentage) */
                if((laatstedosering - nogtedoseren[i] + nogtedoseren[j]) <= (100 - PRM[_prmperc]))
                {
                    /* Nog te doseren is verschil tussen laatste dosering van huidige fase met EG en nogtedoseren waarde van die fase,
                       plus de actuele nogtedoseren waarde van fase j */
                    nogtedoseren[j] = laatstedosering - nogtedoseren[i] + nogtedoseren[j];
                }
            }
            /* Zet de nog te doseren waarde van de fase met EG op 0 */
            nogtedoseren[i] = 0;
        }
        /* Op eindegroen wanneer er geen file is, of wel file maar geen MK,
           zet actuele waarden op 0 */
        if((nogtedoseren[i] && EG[fc[i]] && PR[fc[i]] && !H[hfile]) || (EG[fc[i]] && H[hfile] && !MK[fc[i]]) || nogtedoseren[i] && !A[fc[i]] && prml[ml][fc[i]] == PRIMAIR)
        {
            nogtedoseren[i] = 0;
        }

        /* Doseren! */
        if(nogtedoseren[i] > 0 && !H[hfile])
        {
            /* Toepassen eerder opgeslagen nog te doseren percentage */
            TVG_max[fc[i]] = ((mulv)(((long)(100 - nogtedoseren[i]) * (long)PRM[fcmg[i][MM[_mperiod]]])/100) > TFG_max[fc[i]])
                        ?  (mulv)(((long)(100 - nogtedoseren[i]) * (long)PRM[fcmg[i][MM[_mperiod]]])/100) - TFG_max[fc[i]]
                        : 0;
        }
    }   
}
void Eerlijk_doseren_VerlengGroenTijden_V1(count hfile, /* hulpelement wel/geen file                         */
                        count _prmperc,                 /* indexnummer parameter % doseren                   */
                        count aantalfc,                 /* aantal te doseren fasen                           */
                        count fc[],                     /* pointer naar array met fasenummers                */
                        count fcvg[][MPERIODMAX],       /* pointer naar array met mg parameter index nummers */
                        int nogtedoseren[],             /* pointer naar array met nog te doseren waarden     */
	                    bool *prml[],
	                    count ml,
						count _mperiod)
{
    int i, j, laatstedosering;

    /* Voor elke fase mbt dit filegebied */
    for(i = 0; i < aantalfc; ++i)
    {
        /* Als het file is, eindegroen, en het meetkriterium staat op */
        if(EG[fc[i]] && H[hfile] && MK[fc[i]])
        {
            /* Uitrekenen laatst toegepaste dosering */
            laatstedosering = 100 - (int)(100.0 * (((float)(TFG_max[fc[i]] + TVG_timer[fc[i]])) / ((float)(PRM[fcvg[i][MM[_mperiod]]]+TFG_max[fc[i]]))));
            /* Voor elke fase mbt het fileveld */
            for(j = 0; j < aantalfc; ++j)
            {
                /* Die niet de fase is die nu EG heeft */
                if(i == j)continue;
                /* uitrekenen nogtedoseren waarde voor de andere richtingen:
                   (tbv programmeertechnische veiligheid van het algoritme:
                    alleen doen wanneer de uitkomst lager is dan het file doseer percentage) */
                if((laatstedosering - nogtedoseren[i] + nogtedoseren[j]) <= (100 - PRM[_prmperc]))
                {
                    /* Nog te doseren is verschil tussen laatste dosering van huidige fase met EG en nogtedoseren waarde van die fase,
                       plus de actuele nogtedoseren waarde van fase j */
                    nogtedoseren[j] = laatstedosering - nogtedoseren[i] + nogtedoseren[j];
                }
            }
            /* Zet de nog te doseren waarde van de fase met EG op 0 */
            nogtedoseren[i] = 0;
        }
        /* Op eindegroen wanneer er geen file is, of wel file maar geen MK,
           zet actuele waarden op 0 */
        if((nogtedoseren[i] && EG[fc[i]] && PR[fc[i]] && !H[hfile]) || (EG[fc[i]] && H[hfile] && !MK[fc[i]]) || nogtedoseren[i] && !A[fc[i]] && prml[ml][fc[i]] == PRIMAIR)
        {
            nogtedoseren[i] = 0;
        }

        /* Doseren! */
        if(nogtedoseren[i] > 0 && !H[hfile])
        {
            /* Toepassen eerder opgeslagen nog te doseren percentage */
            TVG_max[fc[i]] = (mulv)(((long)(100 - nogtedoseren[i]) * (long)(PRM[fcvg[i][MM[_mperiod]]]+TFG_max[fc[i]]))/100);
        }
    }   
}

/* Functie om type meeaanvraag op straat instelbaar te maken */
void mee_aanvraag_prm(count i, count j, count prm, bool extra_condition)
{
    if(!extra_condition)
        return;

    switch (PRM[prm])
    {
    case 1:
        if (A[j] && R[j] && !TRG[j]) A[i] |= BIT4;
        break;
    case 2:
        if (RA[j]) A[i] |= BIT4;
        break;
    case 3:
        if (RA[j] && !K[j] || SG[j]) A[i] |= BIT4;
        break;
    case 4:
        if (SG[j]) A[i] |= BIT4;
        break;
    default:
        break;
    }
}

/*****************************************************************************
 *  Functie  : Filemelding
 *
 *  Functionele omschrijving :
 *    CCOL implementatie eenvoudig filemeetpunt (1 detector)
 *
 *  In regeling zet men een hulpelement op afhankelijk van een of meer keren
 *  deze functie aan te roepen. VB:
 *
 *    Filemelding detector df021 en df022
 *
 *    FileMelding(df021, tbz021, trij021, tafv021, hfile021, hafval021);
 *    FileMelding(df022, tbz022, trij022, tafv022, hfile022, hafval022);
 *
 *    RT[tAfvalbewaking] = (D[df021] || D[df022]);
 *    if (!(T[tAfvalbewaking] || RT[tAfvalBewaking]) && SCH[schAfvalbewaking])
 *      IH[hfile021] = IH[hfile022] = FALSE;
 *    IH[hfile] = H[hfile021] || H[hfile022];
 ****************************************************************************/
void FileMeldingV2(count det,     /* filelus                                */
                   count tbez,    /* bezettijd  als D langer bezet -> file  */
                   count trij,    /* rijtijd    als D korter bezet -> !file */
                   count tafval,  /* afvalvertraging filemelding            */
                   count hfile)   /* hulpelement filemelding                */
{
    RT[tbez]   = RT[trij] = !D[det];
    RT[tafval] = D[det] && !T[tbez] ||
		         H[hfile] && D[det] && !T[trij];

	IH[hfile] = RT[tafval] || T[tafval] || H[hfile] && D[det];

    if (CIF_IS[det] >= CIF_DET_STORING)   IH[hfile] = FALSE;
}

void UpdateKnipperSignalen()
{
	Knipper_1Hz = ((CIF_KLOK[CIF_TSEC_TELLER] % 10) > 4); /* 1 Hz */
	Knipper_2Hz = ((CIF_KLOK[CIF_TSEC_TELLER] % 5) > 2); /* 2 Hz */
}

bool hf_wsg_nl(void)
{
	register count i;

	for (i = 0; i<FC_MAX; i++) {
		if (G[i] && !MG[i] && !WS[i] && !(WG[i] && (RW[i] & BIT2)) || G[i] && MK[i] || GL[i] || TRG[i]
			|| R[i] && A[i] && !BL[i])
			return (TRUE);
	}
	return (FALSE);
}

bool hf_wsg_nl_fcfc(count fc1, count fc2)
{
	register count i;

	for (i = fc1; i < fc2; i++) {
		if (G[i] && !MG[i] && !WS[i] && !(WG[i] && (RW[i] & BIT2)) || G[i] && MK[i] || GL[i] || TRG[i]
			|| R[i] && A[i] && !BL[i])
			return (TRUE);
	}
	return (FALSE);
}

void wachttijd_leds_knip(count mmwtv, count mmwtm, count RR_T_wacht, count fix)
{
	/* mmwtv - berekende  aantal leds wachttijdlantaarn    */
	/* mmwtm - uitsturing aantal leds wachttijdlantaarn    */

	if (((fix != NG && CIF_IS[fix]) || (RR_T_wacht > 0)) && MM[mmwtm])         /* fixatie of prio-ingreep terwijl wachttijdlantaarn aan staat  */
	{
		if (MM[mmwtv] > 1)       MM[mmwtm] = MM[mmwtv] - Knipper_1Hz;     /* bij aantal leds groter dan 1 = actuele ledje laten knipperen */
		else                     MM[mmwtm] = MM[mmwtv] + Knipper_1Hz;     /* bij ??n leds de voorgaande led, zodat de wtv niet uitgaat    */
	}
	else                        MM[mmwtm] = MM[mmwtv];            /* anders berekende aantal leds gewoon overnemen                */
}

bool kcv_primair_fk_gkl(count i)
{
	register count n, j;

#ifndef NO_GGCONFLICT
	for (n = 0; n < GKFC_MAX[i]; ++n)
#else
	for (n = 0; n < KFC_MAX[i]; ++n)
#endif
	{
#if (CCOL_V >= 95)
		j = KF_pointer[i][n];
#else
		j = TO_pointer[i][n];
#endif
		if ((((R[j] || GL[j]) && AA[j] || CV[j] ||
			G[j] && (RS[j] || RW[j])) && PR[j]) &&
#if (CCOL_V >= 95) && !defined NO_TIGMAX
			!((TIG_max[i][j] == GKL) && (TIG_max[j][i] == FK)))
#else
			!((TO_max[i][j] == GKL) && (TO_max[j][i] == FK)))
#endif
			return (TRUE);
	}
	return (FALSE);
}

static bool a_pg_fkprml_fk_gkl(count i, bool *prml[], count ml)
{
   register count n,j;

   for (n = 0; n < GKFC_MAX[i]; ++n)
   {
#if (CCOL_V >= 95)
	   j = KF_pointer[i][n];
#else
	   j = TO_pointer[i][n];
#endif
      if ((A[j] && !BL[j] && (!G[j] || CV[j]) || PP[j]) &&
	      ((prml[ml][j] & PRIMAIR_VERSNELD) && !PG[j]) )
	 return (FALSE);	 /* TO_pointer[i][n]	*/
   }

   for (n = GKFC_MAX[i]; n < FKFC_MAX[i]; ++n) {
#if (CCOL_V >= 95)
	   j = KF_pointer[i][n];
#else
	   j = TO_pointer[i][n];
#endif
      if ((A[j] && !BL[j] && !G[j] || PP[j]) &&
	      ((prml[ml][j] & PRIMAIR_VERSNELD) && !PG[j]) )
	 return (FALSE);	 /* TO_pointer[i][n]	*/
   }
   return (TRUE);
 
}


static bool a_ag_fkprml_fk_gkl(count i, bool *prml[], count ml)
{
	register count n, j;

	for (n = 0; n < FKFC_MAX[i]; ++n) {
#if (CCOL_V >= 95)
		j = KF_pointer[i][n];
#else
		j = TO_pointer[i][n];
#endif
		if ((A[j] && !BL[j] && !G[j]) &&
			((prml[ml][j] & ALTERNATIEF_VERSNELD) && !AG[j]))
			return (FALSE);	 /* TO_pointer[i][n] */
	}
	return (TRUE);
}

static void set_pg_fkprml_fk_gkl(count i, bool *prml[], count ml)
{
	register count n, j;

	for (n = 0; n < FKFC_MAX[i]; ++n)
	{
#if (CCOL_V >= 95)
		j = KF_pointer[i][n];
#else
		j = TO_pointer[i][n];
#endif
		if (prml[ml][j] & PRIMAIR)  PG[j] |= PRIMAIR_OVERSLAG;
	}
}

bool kcv_fk_gkl(count i)
{
	register count n, k;

#ifndef NO_GGCONFLICT
	for (n = 0; n < GKFC_MAX[i]; ++n) {
#else
	for (n = 0; n < KFC_MAX[i]; ++n) {
#endif
#if (CCOL_V >= 95)
		k = KF_pointer[i][n];
#else
		k = TO_pointer[i][n];
#endif
		if (((R[k] || GL[k]) && AA[k] || CV[k] || G[k] && (RS[k] || RW[k])) && 
#if (CCOL_V >= 95) && !defined NO_TIGMAX
			!((TIG_max[i][k] == GKL) && (TIG_max[k][i] == FK)))
#else
			!((TO_max[i][k] == GKL) && (TO_max[k][i] == FK)))
#endif
			return (TRUE);
	}
	return (FALSE);
}

bool set_FPRML_fk_gkl(count i, bool *prml[], count ml, count ml_max, bool period)
{
	register count hml, m;

	if (AAPR[i] && !AA[i] && !prml[ml][i])  AAPR[i] = FALSE;

	if (A[i] && RV[i] && !TRG[i] && !AA[i] && !BL[i])
	{
		if (!prml[ml][i] && !PG[i]) /* not in active module or realized */
		{
			hml = ml;
			for (m = 0; m < ml_max; ++m)
			{
				if (!a_pg_fkprml_fk_gkl(i, prml, hml))
				{
					AAPR[i] = 0;
					return (FALSE);
				}
				if (!a_ag_fkprml_fk_gkl(i, prml, hml))
				{
					AAPR[i] |= BIT1;
				}
				if (prml[hml][i] == PRIMAIR) 
				{
					if (!period) AAPR[i] |= BIT5;
					if (RR[i])   AAPR[i] |= BIT4;
					if (fkaa_primair(i))  AAPR[i] |= BIT3;
					if (kcv_primair_fk_gkl(i))   AAPR[i] |= BIT2;
					if (fkaa(i)) 	     AAPR[i] |= BIT1;
					if (AAPR[i])  return (FALSE);
					AAPR[i] = BIT0;

					if (!kcv_fk_gkl(i))
					{
						do
						{
							--hml;
							if (hml < 0)  hml = ml_max - 1;
							set_pg_fkprml_fk_gkl(i, prml, hml);  /* set confl. pg's	*/
							prml[hml][i] |= VERSNELD_PRIMAIR; /* set_FPRML	*/
						} while (hml != ml);
						return (TRUE);
					}
					else return (FALSE);
				}
				++hml;
				if (hml >= ml_max)  hml = ML1;
			}
		}
	}
	return (FALSE);
}

/**************************************************************************
 *  Functie  : veiligheidsgroen_V1
 *
 *  Functionele omschrijving :
 *    Een verbeterde versie van de veiligheidsgroen functionaliteit uit
 *    STDFUNC.C. Er wordt alleen veiligheidsgroen gegeven wanneer
 *    zich meer dan 1 voertuig in de dilemmazone bevindt.
 *    Deze variant start meteen met meten op start MG[].
 *    Per fase dienen ALLE (schakelbaar) deelnemende lussen te worden opgegeven.
 *
 **************************************************************************/
void veiligheidsgroen_V1(count fc, count tmaxvag4, ...)
{/* tmaxvag4            * maximale tijd dat veiligheidsgroen mag worden toegekend   */

    va_list argpt;     /* variabele argumentenlijst                                 */
    count dp;          /* detector waarop veiligheidsgroen wordt bepaald            */
    count tvlg;        /* volgtijd tussen twee voertuigen                           */
    count schvag4;     /* schakelaar per lus                                        */
    count tvgh;        /* hiaattijd waarmee gerekend moet worden bij toekennen vag4 */
   
    bool vag4     = FALSE;
    bool bewaking = FALSE;
    YM[fc]       &= ~BIT2;
   
    va_start(argpt, tmaxvag4);
    dp = va_arg(argpt, va_count);

    do {
        tvlg    = va_arg(argpt, va_count);
        schvag4  = va_arg(argpt, va_count);
        tvgh     = va_arg(argpt, va_count);

        if (schvag4 == NG || SCH[schvag4]) {
            /* meten dilemma */
            if (SD[dp] && CIF_IS[dp] < CIF_DET_STORING) {  /* geen detectiestoring */
                if (DVG[dp] < 2) ++DVG[dp];         /* ophogen teller gemeten voertuigen           */
                if (DVG[dp] >= 2) bewaking = TRUE; /* bij meer dan 1 voeruig in dil.zone: bewaken */
                RT[tvlg] = TRUE;                 /* herstarten volg tijd                        */
            }
            else {
                RT[tvlg] = FALSE;
            }

            /* meten hiaat:
               bij meer dan 1 voertuig in de dilemmazone
               vag4hiaatmeting starten obv detectie */
            if (DVG[dp] >= 2) RT[tvgh] = D[dp];
            else              RT[tvgh] = FALSE;

            /* bepalen VAG4:
               indien bew.tijd en vag4hiaat beide lopen
               veiligheidsgroen op zetten */
            if (T[tmaxvag4] && T[tvgh]) vag4 = TRUE;
            if (ET[tvlg]) DVG[dp] = 0;
        }
        dp = va_arg(argpt, va_count);
    } while (dp != END);

    RT[tmaxvag4] = !T[tmaxvag4] && (MG[fc] && bewaking);     /* bewakingstijd starten       */
    if (MG[fc] && vag4)   YM[fc] |= BIT2;    /* veiligheidsgroen  activeren                 */

    va_end(argpt);
}

/* -------------------------------------------------------------------------------------------------------- */
/* Procedure inkomende pelotonkoppeling                                                                     */
/* -------------------------------------------------------------------------------------------------------- */
bool proc_pel_in_V1(                       /* Dh20130124                                                    */
	 count hfc,                            /* fasecyclus                                                   */
	 count tmeet,                          /* T meetperiode                                                 */
	 count tmaxth,                         /* T max.hiaat                                                   */
	 count grens,                          /* PRM grenswaarde                                               */
	 count mvtg,                           /* MM aantal vtg                                                 */
	 count muit,                           /* MM uitsturing aktief                                          */
	 ...)                                  /* va arg list: inkomende signalen koplussen                     */
{
	va_list argpt;     /* variabele argumentenlijst                                 */
	count hdp;
	count edpel = 0;

	/* Lezen inkomende signalen */
	va_start(argpt, muit);
	hdp = va_arg(argpt, va_count);
	do {
		if (SH[hdp] || EH[hdp]) ++edpel;
        hdp = va_arg(argpt, va_count);
	} while (hdp != END);
	va_end(argpt);

	AT[tmeet] = FALSE;
	RT[tmeet] = RT[tmaxth] = H[hfc] || !T[tmeet] && !T[tmaxth] && edpel > 0;

	if (edpel > 0 && (RT[tmeet] || T[tmeet]))
	{
		MM[mvtg] += (mulv) edpel;
		RT[tmaxth] = TRUE;
	}

	if (ET[tmaxth] && !RT[tmaxth])
	{
		AT[tmeet] = TRUE;
		MM[mvtg] = 0;
	}

	if (TS && (MM[muit] > 0)) --MM[muit];
	if (MM[mvtg] >= PRM[grens])
	{
		MM[mvtg] = 0;
		MM[muit] = 3;
		AT[tmeet] = TRUE;
	}

	return (CIF_WPS[CIF_PROG_STATUS] == CIF_STAT_REG) && (MM[muit] > 0);
}

bool IsConflict(count fc1, count fc2)
{
	count i;
	for (i = 0; i < FKFC_MAX[fc1]; ++i) { /* KFC=confl.; GKFC=KFC+groenconfl.; FKFC=GKFC+fictieve confl. */
#if (CCOL_V >= 95)
		if (KF_pointer[fc1][i] == fc2) {
#else
		if (TO_pointer[fc1][i] == fc2) {
#endif
			return (bool)TRUE;
		}
	}
	return (bool)FALSE;
}

void ModuleStructuurPRM(count prmfcml, count fcfirst, count fclast, count ml_max, bool *prml[], bool yml[], count *mlx, bool *sml)
{
	if (fcfirst < fclast)
	{
		int fc, ml, fcc;
		bool PRML_x[FCMAX];        /* bijhouden toedeling */
		mulv PRML_temp[15][FCMAX]; /* tijdelijke modulemolen */

		/* bepaal nieuwe tijdelijke modulemolen, houdt toedelen bij */
		for (fc = fcfirst; fc < fclast; ++fc)
		{
			PRML_x[fc] = FALSE;
			
			/* ML toedeling */
			for (ml = 0; ml < ml_max; ++ml)
			{
			    /* Bewust niet toegedeeld */
                if (PRM[prmfcml + fc] & BIT10)
                {
                    PRML_temp[ml][fc] = FALSE;
                    PRML_x[fc] = TRUE;
                    BL[fc] |= BIT10;
                    continue;
                }
				else
				{
				    /* Toegedeeld aan dit blok */
			    	if (PRM[prmfcml + fc] & (1 << ml))
			    	{
			    		PRML_temp[ml][fc] = PRIMAIR;
			    		PRML_x[fc] = TRUE;
			    	}
			    	/* Niet toegedeeld aan dit blok */
			    	else
			    	{
			    		PRML_temp[ml][fc] = FALSE;
			    	}
				}
			}
		}

		/* controleer:
		   - geen conflicterende fasen in dezelfde module van de tijdelijke modulemolen zitten
		   - geen fasen zonder toedeling
		*/
		for (fc = fcfirst; fc < fclast; ++fc)
		{
			/* Check toedeling */
			if (!PRML_x[fc])
			{
				char tmp[256];
				sprintf(tmp, "%s > Module indeling via PRMs: fase %s is niet toegedeeld.\n",
					PROMPT_code,
					FC_code[fc]);
				uber_puts(tmp);
				return;
			}
			/* Check conflicten */
			for (ml = 0; ml < ml_max; ++ml)
			{
				if (PRML_temp[ml][fc] == PRIMAIR)
				{
					for (fcc = fcfirst; fcc < fclast; ++fcc)
					{
						if (PRML_temp[ml][fcc] == PRIMAIR && IsConflict(fc, fcc))
						{
							char tmp[256];
							sprintf(tmp, "%s > Module indeling via PRMs: conflicterende fasen %s en %s zijn toegedeeld in hetzelfde blok.\n",
								PROMPT_code,
								FC_code[fc],
								FC_code[fcc]);
							uber_puts(tmp);
							return;
						}
					}
				}
			}
		}

		/* reset huidige modulemolen */
		for (fc = fcfirst; fc < fclast; ++fc)
		{
			for (ml = 0; ml < ml_max; ++ml)
			{
				prml[ml][fc] = FALSE;
			}
		}

		/* kopieer tijdelijke modulemolen naar nieuwe modulemolen */
		for (fc = fcfirst; fc < fclast; ++fc)
		{
			for (ml = 0; ml < ml_max; ++ml)
			{
				if (PRML_temp[ml][fc] == PRIMAIR) prml[ml][fc] = PRIMAIR;
			}
		}

		/* initialiseer nieuwe modulemolen */
		init_modules(ml_max, prml, yml, mlx, sml);

		{
			char tmp[256];
			sprintf(tmp, "%s > Module indeling via PRMs: succesvol toegepast.\n",
				PROMPT_code);
			uber_puts(tmp);
		}
	}
}

/**************************************************************************
 *  Functie  : seniorengroen
 *
 *  Functionele omschrijving :
 *    Houdt de richting na vastgroen een instelbaar percentage van dat
 *    vastgroen vast in wachtgroen zodat voetgangers die moeilijk ter been
 *    zijn meer gelegenheid hebben om de oversteek te maken.
 *    Zij dienen daartoe een instelbaar aantal seconden achtereen de
 *    aanvraagknop ingedrukt te houden.
 *    Er kunnen drie opeenvolgende oversteken bediend worden. Eventuele
 *    volgoversteken hebben ieder hun eigen instelling voor extra groen.

 *    Wanneer het seniorengroen aktief is, worden eventuele nalopen pas
 *    gestart op het einde (wacht)groen van de voedende richting, zodat
 *    men ook gelegenheid heeft om in trager tempo de naloop over te steken.
 *
 *    AANGEPASTE VERSIE 04032024:
 *    - Overgang D, !D, D leverde een ontrechte ingreep op. Dit is in deze aangepaste versie opgelost.
 *    - Nalooptijd werd tijdens de ingreep altijd herstart gedurende VS, FG en WG,
 *      deze wordt nu beperkt tot het opgegeven extra percentage.
 **************************************************************************/
void SeniorenGroen(count fc, count drk1, count drk1timer, count drk2, count drk2timer,
                   count exgperc, count verlengen, count meergroen, ...)
{
    va_list argpt;
    count tnl;
    va_start(argpt, meergroen);

    T_max[meergroen] = (TFG_max[fc] * (100 + PRM[exgperc]) / 100);

    if (drk1 != NG) {
        if (drk1timer != NG) {
            IT[drk1timer] = SD[drk1];
            AT[drk1timer] = !D[drk1];
            if (ET[drk1timer] && !IT[drk1timer] && !AT[drk1timer])  IH[verlengen] |= TRUE;
        }
    }
    if (drk2 != NG) {
        if (drk2timer != NG) {
            IT[drk2timer] = SD[drk2];
            AT[drk2timer] = !D[drk2];
            if (ET[drk2timer] && !IT[drk2timer] && !AT[drk2timer])  IH[verlengen] |= TRUE;
        }
    }

    if (G[fc] && T[meergroen])                                             RW[fc] |= BIT7;
    if (G[fc])                                                      IH[verlengen] = FALSE;

    /* extra vasthouden in FG en WG */
    RT[meergroen] = IH[verlengen];

    /* herstart naloop tijdens VS, FG en WG en beperk deze tot maximaal PRM[exgperc] extra groen */
    while ((tnl = va_arg(argpt, va_count)) != END)
    {
        RT[tnl] |= (T[meergroen] && (T_timer[meergroen] < (T_max[tnl] * PRM[exgperc] / 100)) || RT[meergroen]) && (VS[fc] || FG[fc] || WG[fc]);
    }
    va_end(argpt);
}

/*
 * Functie : CyclustijdMeting
 * Functionele omschrijving : Bepaling cyclustijd regeling o.b.v. conditie en schrijven naar CIF_UBER.
*/

void CyclustijdMeting(count tcyclus, count scyclus, count cond, count sreset, count mlcyclus)
{
    /* moeten statisch zijn om de maximale cyclustijd, de laatste 10 cyclustijden en het tijdstip van de maximale cyclustijd te onthouden. */
    static count cycmax = 0;
    static count cyca[10] = {0,0,0,0,0,0,0,0,0,0};
    static mulv dg,md,jr,ur,mt,se,tr;
    
    int i;
    count k = 0;
    count totc = 0;
    mulv cyclGem;
    
    /* Reset de maximale cyclustijd en het gemiddelde? */
    if (SCH[sreset])
    {
        cycmax = 0;
        for (i = 0; i < 10 ; i++) cyca[i] = 0;
        dg = 0;
        md = 0;
        jr = 0;
        ur = 0;
        mt = 0;
        se = 0;
        tr = 0;
    }

    RT[tcyclus]= FALSE;
    if (cond)
    {
        if (SCH[scyclus])
        {
            if ((T_timer[tcyclus] > 0) && (T_timer[tcyclus] < T_max[tcyclus]))
            {
                for (i=0; i<9; i++)
                {
                    cyca[i] = cyca[i+1];
                    if (cyca[i] > 0)
                    {
                        totc = totc + cyca[i];
                        k++;
                    }
                }
    
                cyca[9] = T_timer[tcyclus];
    
                /*if ((T_timer[tcyclus] > cycmax) && (totc > 0))*/
                if (T_timer[tcyclus] > cycmax)
                {
                    cycmax= T_timer[tcyclus];
#if !defined (AUTOMAAT) || defined (VISSIM)
                    dg = CIF_KLOK[CIF_DAG];
                    md = CIF_KLOK[CIF_MAAND];
                    jr = CIF_KLOK[CIF_JAAR];
                    ur = CIF_KLOK[CIF_UUR];
                    mt = CIF_KLOK[CIF_MINUUT];
                    se = CIF_KLOK[CIF_SECONDE];
                    tr = CIF_KLOK[CIF_TSEC_TELLER] % 10;
#endif /* AUTOMAAT || VISSIM */
                }
    
                /* Houd de laatst gemeten cyclustijd bij in een memory-element */
                MM[mlcyclus] = T_timer[tcyclus];
    		    
                /* Bereken de gemiddelde cyclustijd */
                cyclGem = ((mulv) totc + T_timer[tcyclus]) / (mulv) (k + 1);
    		    
                uber_puts(PROMPT_code);
                uber_puts("T_cyclus=");
                uber_puti(T_timer[tcyclus]);
                uber_puts(" Gem_cyc=");
                uber_puti(cyclGem);
                uber_puts(" Max_cyc=");
                uber_puti((mulv)cycmax);
#if !defined (AUTOMAAT) || defined (VISSIM)
                uber_puts(" (");
                if (dg < 10) uber_puts("0");
                uber_puti(dg);
                uber_puts("-");
                if (md < 10) uber_puts("0");
                uber_puti(md);
                uber_puts("-");
                if (jr < 10) uber_puts("0");
                uber_puti(jr);
                uber_puts(" ");
                if (ur < 10) uber_puts("0");
                uber_puti(ur);
                uber_puts(":");
                if (mt < 10) uber_puts("0");
                uber_puti(mt);
                uber_puts(":");
                if (se < 10) uber_puts("0");
                uber_puti(se);
                uber_puts(".");
                uber_puti(tr);
                uber_puts(")");
#endif /* AUTOMAAT || VISSIM */
                uber_puts("\n");
            } /* if (timer) */
        } /* SCH[scyclus] */
        RT[tcyclus]= TRUE;
    } /* if(cond) */
} /* void */

void maximumgroentijden_va_arg(count fc, ...)
{
   va_list argpt;                       /* variabele argumentenlijst	*/
   mulv prmmg;                          /* maximum verlenggroentijd	*/
   mulv hklok;				/* klokgebied			*/

   va_start(argpt, fc);			/* start var. argumentenlijst	*/
   do
   {
      prmmg= (mulv) va_arg(argpt, va_mulv);	/* lees max. verlenggroentijd	*/
      hklok= (mulv) va_arg(argpt, va_mulv);	/* lees klokperiode		*/
   }
   while (hklok == 0);

   va_end(argpt);			/* maak var. arg-lijst leeg	*/

   TVG_max[fc] = (prmmg - TFG_max[fc] < 0) ? 0 : prmmg - TFG_max[fc];
}

#if CCOL_V >= 110
bool kp(count i)
{
   register count n, j;

#ifndef NO_GGCONFLICT
   for (n = 0; n < FKFC_MAX[i]; ++n) {
#else
   for (n = 0; n < KFC_MAX[i]; ++n) {
#endif
#if (CCOL_V >= 95)
      j = KF_pointer[i][n];
#else
      j = TO_pointer[i][n];
#endif
      if (P[j] & BIT11) return TRUE;
   }
   return FALSE;
}
#endif

#ifndef AUTOMAAT
#if CCOL_V >= 110

/* Controleer of de naloop er niet eerder uit gaat dan de voedende richting.
*
* Parameters:
* voedend: de voedende richting
* volg: de volgrichting
* tnlfg: naloop na vast groen
* tnlfg: vaste naloop na groen
* tnldet: detectie afhanelijke naloop
* halt: halteer de simulatie als de naloop niet wordt gerespecteerd.
*
* Returnwaardes: TRUE als OK, FALSE als de naloop er eerder uit is dan
* de voedende richting.
*/

bool ControleerNaloopEG(count voedend, count volg, count tnlfg, count tnleg, count tnldet, bool halt)
{
   if (EG[volg] && (G[voedend] || T[tnlfg] || (TR_timer[voedend] < (T_max[tnleg] - TGL_max[voedend])) || (tnldet == NG ? FALSE : T[tnldet]) ))
   {
      /* Schrijf naar de CCOL-terminal */
      code helpstr[30]; /* help string */
      uber_puts(PROMPT_code);
      uber_puts("Ongewenste situatie: EG[");
      uber_puts(FC_code[volg]);
      uber_puts("] en G[");
      uber_puts(FC_code[voedend]);
      uber_puts("]");
      uber_puts(" / ");
      datetostr(helpstr);
      uber_puts(helpstr);
      uber_puts(" / ");
      timetostr(helpstr);
      uber_puts(helpstr);
      uber_puts("\n");

      /* Schrijf naar de debugwindow in de testomgeving. */
      xyprintf(0, 0, "Ongewenste situatie: naloop niet gerespecteerd. Zie terminal.");
      if (halt)
      {
#ifndef VISSIM
         stuffkey(F5KEY);
#endif
      }
      return FALSE;
   }
   return TRUE; /* Alles OK. */
}


/* Controleer of de voedende richting er niet eerder uit gaat dan dat de
* naloop is gerealiseerd.
*
* Parameters:
* voedend: de voedende richting
* volg: de volgrichting
* t/prm: maximale inrijtijd
* halt: halteer de simulatie als de naloop niet wordt gerespecteerd.
*
* Return waardes: TRUE als OK, FALSE als de voedende richting er eerder uit
* is dan startgroen van de naloop.
*/

bool ControleerInrijden(count voedend, count volg, bool tinr, bool halt)
{
   if (!G[volg] && G[voedend]  && (TG_timer[voedend] > (tinr == NG ? TRUE : T_max[tinr])))
   {
      /* Schrijf naar de CCOL-terminal */
      code helpstr[30]; /* help string */
      uber_puts(PROMPT_code);
      uber_puts("Ongewenste situatie: R[");
      uber_puts(FC_code[volg]);
      uber_puts("] en niet tijdig groen ");
      uber_puts(FC_code[voedend]);
      uber_puts("]");
      uber_puts(" / ");
      datetostr(helpstr);
      uber_puts(helpstr);
      uber_puts(" / ");
      timetostr(helpstr);
      uber_puts(helpstr);
      uber_puts("\n");

      /* Schrijf naar de debugwindow in de testomgeving. */
      xyprintf(31, 2, "Ongewenste situatie: naloop niet gerespecteerd. Zie terminal.");
      if (halt)
      {
#ifndef VISSIM
         stuffkey(F5KEY);
#endif
      }
      return FALSE;
   }
   return TRUE; /* Alles OK. */
}

/* Controleer gelijkstart */
bool ControleerGS(count fc1, count fc2, bool cond, bool halt)
{
   {

      if (cond)
      {
         if ((EVS[fc1] && !G[fc2]) || (EVS[fc2] && !G[fc1]))
         {
            /* Schrijf naar de CCOL-terminal */
            code helpstr[30];  /* help string */
            uber_puts(PROMPT_code);
            uber_puts("Ongewenste situatie: !GS fc");
            uber_puts(FC_code[fc1]);
            uber_puts(" en fc");
            uber_puts(FC_code[fc2]);
            uber_puts("");
            uber_puts(" / ");
            datetostr(helpstr);
            uber_puts(helpstr);
            uber_puts(" / ");
            timetostr(helpstr);
            uber_puts(helpstr);
            uber_puts("\n");

            /* Schrijf naar de debugwindow in de testomgeving. */
            xyprintf(0, 0, "Ongewenste situatie: gelijstart niet gerespecteerd. Zie terminal.");
            if (halt)
            {
#ifndef VISSIM
               stuffkey(F5KEY);
#endif
            }
            return FALSE;
         }
      }

      return TRUE; /* Alles OK. */
   }
}

/* Controleer voorstart */
bool ControleerVS(count fc1, count fc2, bool cond, bool halt)
{
   {

      if (cond)
      {
         if ((EVS[fc1] && !(G[fc2]||GL[fc2]) && !(RR[fc2]&BIT6)))
         {
            /* Schrijf naar de CCOL-terminal */
            code helpstr[30];  /* help string */
            uber_puts(PROMPT_code);
            uber_puts("Ongewenste situatie: !VS fc");
            uber_puts(FC_code[fc1]);
            uber_puts(" en fc");
            uber_puts(FC_code[fc2]);
            uber_puts("");
            uber_puts(" / ");
            datetostr(helpstr);
            uber_puts(helpstr);
            uber_puts(" / ");
            timetostr(helpstr);
            uber_puts(helpstr);
            uber_puts("\n");

            /* Schrijf naar de debugwindow in de testomgeving. */
            xyprintf(0, 0, "Ongewenste situatie: VS niet gerespecteerd. Zie terminal.");
            if (halt)
            {
#ifndef VISSIM
               stuffkey(F5KEY);
#endif
            }
            return FALSE;
         }
      }

      return TRUE; /* Alles OK. */
   }
}

#endif // #if CCOL_V >= 110
#endif // #ifndef AUTOMAAT

bool set_MRLW_nl(count i, count j, bool period)
/* meerealisatie uitgebreid */
/* Als de voedende richting niet primair komt ten gevolge , 
 * sturen wij de naloop middels een aangepast set_MRLW (zonder !fkaa) naar RA.
 * set_MRLW volstaat niet omdat hier op !fkaa wordt getest en 
 * de naloop ook moet komen als de voedende richting groen is of RR heeft. 
 * i=naloop, j=voedend, period=voorwaarde 
 */
{
#if CCOL_V >= 110
   if (AA[j] && period /* && RV[i] */ && !AA[i] && (!RR[i] || P[i]) && !BL[i] && !kaa(i) /* !fkaa */
      && (!RR[j] || G[j]) && !BL[j]) {
#else
   if (AA[j] && period /* && RV[i] */ && !AA[i] && !RR[i] && !BL[i] && !kaa(i) /* !fkaa */
      && !RR[j] && !BL[j]) {         
#endif
      PR[i] = AR[i] = BR[i] = MR[i] = FALSE;  /* reset old realizationtype   */
      AA[i] = MR[i] = TRUE;                   /* set actuation               */
      A[i] |= BIT4;                           /* set demand                  */
      if (PR[j])  PR[i] = PR[j];              /* set primary realization     */
      if (AR[j])  AR[i] = AR[j];              /* set alternative realization */
      if (BR[j])  BR[i] = BR[j];              /* set priority realization    */
      return (TRUE);
   }
   return (FALSE);
}

void set_parm1wijzap(s_int16 *parm)
{
   if (CIF_PARM1WIJZAP == CIF_GEEN_PARMWIJZ) {
   CIF_PARM1WIJZAP= (s_int16) (parm - CIF_PARM1);
   }
   else {
   CIF_PARM1WIJZAP= CIF_MEER_PARMWIJZ;
   }
}

bool set_parm1wijzpb_tvgmax (mulv periode, count startprm, mulv ifc_prm[], count ifc_prm_max)
{
	 count fci, i;
	 bool tvgmaxwijzpb; /* max. verlenggroentijd wijziging via procesbesturing */
	 
/* wanneer via procesbesturing TVG_max[] wordt gewijzigd, aanpassen in de parameter in de betreffende periode   */

/*   de index van de FASECYCLUS, waarvan de TVG_max[] is gewijzigd = 
     CIF_PARM1WIJZPB minus het aantal ontruimingstijden (= FCMAX * FCMAX)
                     minus het aantal detectortijden (= 6 (bezettijd, hiaattijd, bovengedrag, ondergedrag, fluttergedrag, fluttergedrag) * DPMAX)
                     minus het aantal signaalgroeptijden (= 4 (garantieroodtijd, garantiegroentijd, garantiegeeltijd, vastgroentijd) * FCMAX)
     fcindex_gewijzigde_maxverlenggroentijd = CIF_PARM1WIJZPB - FCMAX*FCMAX - 6 * DPMAX - 4 * FCMAX
     aan de hand hiervan kun je het indexnummer van de te wijzigen verlengroenparameter bepalen.

    dat is dan: PRM[startprm + (periode - 1) * ifc_prm_max  + fcindex_gewijzigde_maxverlenggroentijd]

*/
    tvgmaxwijzpb = FALSE;
    
    fci =  CIF_PARM1WIJZPB - (FCMAX*FCMAX) - (6 * DPMAX) - (4 * FCMAX);
    if(fci >=0 && fci < FCMAX) /* de gewijzigde parameter is gewijzgde max. verlenggroentijd */ { 
    	 for(i=0; i<ifc_prm_max; i++) if(ifc_prm[i] == fci) break;

       if((startprm + (periode - 1) * ifc_prm_max  + i) < PRMMAX) {
          PRM[startprm + (periode - 1) * ifc_prm_max  + fci] = TVG_max[fci];
          set_parm1wijzap(&PRM[startprm + (periode - 1) * ifc_prm_max  + i]);
          tvgmaxwijzpb = TRUE;
       }
    }
    
    return (tvgmaxwijzpb);
}

/* KG */
/* kg() tests G for the conflicting phasecycles.
 * kg() returns TRUE if an "G[]" is detected, otherwise FALSE.
 * kg() can be used in the function application().
 */
#if !defined (CCOLFUNC)

bool kg(count i)
{
   register count n, j;

#ifndef NO_GGCONFLICT
   for (n = 0; n < GKFC_MAX[i]; ++n) {
#else
   for (n = 0; n < KFC_MAX[i]; ++n) {
#endif
#if (CCOL_V >= 95)
      j = KF_pointer[i][n];
#else
      j = TO_pointer[i][n];
#endif
      if (G[j]) return TRUE;
   }
   return FALSE;
}

#endif
