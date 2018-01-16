/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* CCOL - standaard bibliotheek Royal HaskoningDHV voor TPA generator V3.40               */
/*                                                                                        */
/*                                                                                        */
/* (C) Copyright 2015 Royal HaskoningDHV b.v. All rights reserved.                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* Versie :  1.0                                                                          */
/*           Eerste straatversie                                                          */
/* Naam   :  rhdhv_gen.c                                                                  */
/* Datum  :  07-12-2015                                                                   */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */

#include "halfstar_help.h"

/* tijdstip van aanroep DTV_timestamp(): */
#define RHDHV_TIJD_LNGT 82
char RHDHV_TIJD[RHDHV_TIJD_LNGT];

/*****************************************************************************/
void rhdhv_reset_MLE(void)
{
  count i;
  ML=ML1;               /* active module                        */

  for(i=0; i<ML_MAX; i++)
    YML[i]=FALSE;       /* hold module                          */

  for(i=0; i<FC_MAX; i++)
  {
    PR[i]=FALSE;        /* primary realisation                  */
    AR[i]=FALSE;        /* alternative realisation              */
    PG[i]=FALSE;        /* primary realized                     */
    AG[i]=FALSE;        /* alternative realized                 */
    PP[i]=FALSE;        /* privelege periode prim. realisation  */
    WS[i]=FALSE;        /* waiting green - phasecycle           */
    
    PFPR[i] = FALSE;
  }
}

/*****************************************************************************/
void reset_fc_halfstar(void)
{
  register count i;
  for(i=0; i<FC_MAX; i++)
  {
    YS[i] = YW[i] = YV[i] = YM[i] /*= YL[i]*/ = FALSE;
    BR[i] = PP[i] = RR[i] = BL[i] = X[i] = RS[i] = RW[i] = FW[i] = FM[i] = Z[i] /*= MK[i]*/ = FALSE;
  }
}


/**********************************************************************************/
void rhdhv_gelijkstart_va_arg(count h_x     ,
                              count h_rr    ,
                              bool  overslag,
                              ...           )
{
  count fc;
  va_list argp;
   
  IH[h_x] = FALSE;
  if (h_rr != NG)  IH[h_rr] = FALSE;
  
  va_start(argp, overslag);
  do 
  {
    fc = va_arg(argp, va_count);
    
    if (fc != END)
    {
      if ( (X[fc] && (R[fc] || GL[fc]))         ||
           RR[fc]                               ||
           (K[fc] &&  A[fc])                    ||
          TRG[fc] || GL[fc] || SGL[fc]          ||
          (PR[fc] || AR[fc]) && RV[fc] && A[fc] ||
          SRA[fc]                                )  IH[h_x] = TRUE;
      
      if ((h_rr != NG) && overslag)
      {
        if (  G[fc]           ||
             GL[fc]            )  IH[h_rr] |= TRUE;
      }
    }
  } while (fc != END);
  va_end(argp);
}


/***********************************************************************************************************************/
/* versnelde of alternatieve realisaties tegenhouden als fictief conflict aan het afwikkelen is */
void rhdhv_primcor_2kopp(count fc1, count fc2, bool voorwaarde, count h_mak1, count h_mak2)
{
  if (voorwaarde)
  {
      RR[fc1] |= R[fc1] && ((TRG[fc2] || GL[fc2] || kcv_primair(fc2) || RR[fc2] && !G[fc2])) && (IH[h_mak1] || IH[h_mak2]) ||
                 RA[fc1] && AR[fc1] && RV[fc2] && !TRG[fc2] && A[fc2] && !kcv(fc2) && !PAR[fc2] ||
                 RA[fc2] && AR[fc2] && RV[fc1] && !TRG[fc1] && A[fc1] && !kcv(fc1) && !PAR[fc1] ? RHDHV_RR_VS : 0;
      RR[fc2] |= R[fc2] && ((TRG[fc1] || GL[fc1] || kcv_primair(fc1) || RR[fc1] && !G[fc1])) && (IH[h_mak1] || IH[h_mak2]) ||
                 RA[fc1] && AR[fc1] && RV[fc2] && !TRG[fc2] && A[fc2] && !kcv(fc2) && !PAR[fc2] ||
                 RA[fc2] && AR[fc2] && RV[fc1] && !TRG[fc1] && A[fc1] && !kcv(fc1) && !PAR[fc1] ? RHDHV_RR_VS : 0;
  }
  
#ifdef HALFSTAR
  if (((TX_timer == TXB_PL[fc1]) || (TOTXB_PL[fc1] == 0)) ||
      ((TX_timer == TXB_PL[fc2]) || (TOTXB_PL[fc2] == 0)) )
  {
    RR[fc1] &= ~RHDHV_RR_VS;
    RR[fc2] &= ~RHDHV_RR_VS;
  }
#endif

}

/*****************************************************************************/
/* deelconflict afwikkeling tussen auto en andere richting; 
   auto moet voorrang verlenen aan andere richting */
void rhdhv_deelconflict_auto_1r(count fc1, count fc2, count t_vs, count t_vso)
{
  /* auto tegenhouden */
  if (((rhdhv_tgroen[fc2] <= T_max[t_vs]) && G[fc2]) || ((R[fc2] || GL[fc2]) && A[fc2]))
    X[fc1] |= RHDHV_X_DEELC;
  
  /* overslag andere richting als auto eenmaal groen is */
  if (R[fc2] && G[fc1] && !MG[fc1])
    RR[fc2] |= RHDHV_RR_VS;
  
  /* als andere richting wil bijkomen, dan tegenhouden als auto nog groen is */
  RT[t_vso] = (bool)(GL[fc1]);
  if (G[fc1] || GL[fc1] || RHDHV_IRT(t_vso))
    X[fc2] |= RHDHV_X_DEELC;
  
  /* als andere richting eenmaal groen is en auto wil groen worden, 
     dan andere richting groen houden */
  if (RA[fc1] && (MG[fc2] || WG[fc2]))
    RW[fc2] |= RHDHV_RW_VS;
  
}


/**********************************************************************************/
bool rhdhv_yws_groen_fk(count i)
{
  count n, k;

  for (n=0; n<FKFC_MAX[i]; n++) 
  {
    k = TO_pointer[i][n];
    if (A[k])  return (FALSE);
  }
  return (TRUE);
}


/**********************************************************************************/
/* Deze functie is afgeleid van de ccol functie "set_MRLW()".
*/
void rhdhv_MR(count i, count j, bool condition)
{
  if (AA[j] && condition && !AA[i] && !BL[i] && !fkaa(i) && !RR[j] && !BL[j]) 
  {
      PR[i]=AR[i]=BR[i]=MR[i]= FALSE;
      AA[i]=MR[i]= TRUE;              /* set actuation                */
      A[i] |= RHDHV_A_MR;               /* set demand                 */
      if (PR[j])  PR[i]= PR[j];       /* set primary realization      */
      if (AR[j])  AR[i]= AR[j];       /* set alternative realization  */
      if (BR[j])  BR[i]= BR[j];       /* set priority realization     */
  }
}


/*****************************************************************************/
/* retour rood wanneer richting AR heeft maar geen PAR meer */
void rhdhv_reset_altreal(void)
{
  count i;

  for (i=0; i<FCMAX; i++)
    RR[i] |= R[i] && AR[i] && (!PAR[i] || ERA[i]) ? RHDHV_RR_ALTCOR : 0;
}

/*****************************************************************************/
void rhdhv_signaltimers(void)
{
  int fc;

  for (fc=0; fc<FC_MAX; fc++)
  {
    /* groentijd */
    if (G[fc] && rhdhv_tgroen[fc] < (MAX_VALUE_INT - TE))             rhdhv_tgroen[fc] +=TE;
    if (SG[fc])                                                       rhdhv_tgroen[fc] = 0;
    
    /* groentijd excusief groentijd */
    if (G[fc] && !WS[fc] && rhdhv_tgroen[fc] < (MAX_VALUE_INT - TE))  rhdhv_tgroen_ws[fc] +=TE;
    if (SG[fc])                                                       rhdhv_tgroen_ws[fc] = 0;
    
    /* geeltijd */
    if (GL[fc] && rhdhv_tgeel[fc] < (MAX_VALUE_INT - TE))             rhdhv_tgeel[fc] +=TE;
    if (SGL[fc])                                                      rhdhv_tgeel[fc] = 0;
    
    /* roodtijd */
    if (R[fc] && rhdhv_trood[fc] < (MAX_VALUE_INT - TE))              rhdhv_trood[fc] +=TE;
    if (SR[fc])                                                       rhdhv_trood[fc] = 0;
    
    /* wachttijd, alleen wanneer richting niet wordt geblokkeerd */
    if (A[fc] && !G[fc])                                              rhdhv_twacht[fc] +=TE;
    if (!A[fc] || BL[fc])                                             rhdhv_twacht[fc] = 0;
  }
}

/*****************************************************************************/
/* fictieve ontruimingstijden bij deelconflicten */
void rhdhv_fictief_ort(count fc1, count fc2, count t_ort)
{
  RT[t_ort] = (bool)(GL[fc1] || SR[fc1]);
  X[fc2] |= (bool)(RHDHV_IRT(t_ort) ) ? RHDHV_X_DEELC: 0;
}


/*****************************************************************************/
void rhdhv_hd_real(count fc, bool condition)
{
  if (condition)
  {
     X[fc] = FALSE;
     Z[fc] = FALSE;
    RR[fc] = FALSE;
    FM[fc] = FALSE;
    RTFB |= RHDHV_RTFB_HD;
    AA[fc] |= RHDHV_AA_HD; 
     A[fc] |= BIT0;
    YM[fc] |= RHDHV_YM_HD;
    BR[fc] |= RHDHV_BR_HD;
  }
}

/*****************************************************************************/
void rhdhv_z_rr_fk(int fc, bool bitz, bool bitrr)
{
  count i, k;
  
  for (i=0; i<FKFC_MAX[fc]; ++i) 
  {
    k = TO_pointer[fc][i];
    
    RR[k] |= bitrr;
     Z[k] |= bitz;
  }
}


/**********************************************************************************/
/* maximale wachttijd bijhouden in geheugenelementen */
void rhdhv_max_wt(void)
{
  int fc;
  
  for (fc=0; fc<FC_MAX; fc++)
  {
    if (TFB_timer[fc] > RHDHV_MAX_WT[fc])
    {
      RHDHV_MAX_WT[fc] = TFB_timer[fc];
      #ifdef mmaxwtbase
        MM[mmaxwtbase + fc] = RHDHV_MAX_WT[fc];
      #endif
    }
  }
}


/**********************************************************************************/
/* Set functies voor het wijzigen van parameters, timers, schakelaars, hiaten en
   conflicten */
void rhdhv_wijzig_prm(count p, mulv newvalue)
{
  if (PRM[p] != newvalue)
  {
    PRM[p] = (mulv)(newvalue);

    if (CIF_PARM1WIJZAP == CIF_GEEN_PARMWIJZ)
    {
      CIF_PARM1WIJZAP = (s_int16)(&PRM[p]-CIF_PARM1);  /* laat weten dat de applicatie de parm1[]-buffer heeft aangepast */
    }                                                  /* &PRM[p] is het adres van PRM[p] */
    else
    {
      CIF_PARM1WIJZAP = (s_int16)(CIF_MEER_PARMWIJZ);  /* laat weten dat de applicatie de parm1[]-buffer heeft aangepast */
    }
  }
}


/**********************************************************************************/
void rhdhv_wijzig_timer(count t, mulv newvalue)
{
  if (T_max[t] != newvalue)
  {
    T_max[t] = (mulv)(newvalue);

    if (CIF_PARM1WIJZAP == CIF_GEEN_PARMWIJZ)
    {
      CIF_PARM1WIJZAP = (s_int16)(&T_max[t]-CIF_PARM1);
    }
    else
    {
      CIF_PARM1WIJZAP = (s_int16)(CIF_MEER_PARMWIJZ);
    }
  }
}


/**********************************************************************************/
void rhdhv_wijzig_hiaattijd(count d, mulv newvalue)
{
  if (TDH_max[d] != newvalue)
  {
    TDH_max[d] = (mulv)(newvalue);

    if (CIF_PARM1WIJZAP == CIF_GEEN_PARMWIJZ)
    {
      CIF_PARM1WIJZAP = (s_int16)(&TDH_max[d]-CIF_PARM1);
    }
    else
    {
      CIF_PARM1WIJZAP = (s_int16)(CIF_MEER_PARMWIJZ);
    }
  }
}


/**********************************************************************************/
void rhdhv_set_prmbit(count p, mulv bitje)
{
  if (!(PRM[p]&bitje))  /* als bitje niet waar is */
  {
    PRM[p] |= bitje;

    if (CIF_PARM1WIJZAP == CIF_GEEN_PARMWIJZ)
    {
      CIF_PARM1WIJZAP = (s_int16)(&PRM[p]-CIF_PARM1);
    }
    else
    {
      CIF_PARM1WIJZAP = (s_int16)(CIF_MEER_PARMWIJZ);
    }
  }
}


/**********************************************************************************/
void rhdhv_reset_sch(count s)
{
  if (SCH[s])  /* als schakelaar waar is */
  {
    SCH[s] = FALSE;

    if (CIF_PARM1WIJZAP == CIF_GEEN_PARMWIJZ)
    {
      CIF_PARM1WIJZAP = (s_int16)(&SCH[s]-CIF_PARM1);
    }
    else
    {
      CIF_PARM1WIJZAP = (s_int16)(CIF_MEER_PARMWIJZ);
    }
  }
}

/**********************************************************************************/
void rhdhv_toggle_conflict(count fc1, count fc2, count to1_2, count to2_1, bool period)
{
  if (period)  /* conflict gewenst */
  {
    if (TO_max[fc1][fc2] == NK ||
        TO_max[fc2][fc1] == NK   )
    {
      TO_max[fc1][fc2] = PRM[to1_2];
      TO_max[fc2][fc1] = PRM[to2_1];
      pointer_conflicts();  /* verander pointertabel */
      CIF_PARM1WIJZAP = (s_int16)(CIF_MEER_PARMWIJZ);  /* laat weten dat de applicatie de parm1[]-buffer heeft aangepast */
    }
  }
  else  /* geen conflict gewenst */
  {
    if (TO_max[fc1][fc2] != NK ||
        TO_max[fc2][fc1] != NK   )
    {
      TO_max[fc1][fc2] = NK;
      TO_max[fc2][fc1] = NK;
      pointer_conflicts();  /* verander pointertabel */
      CIF_PARM1WIJZAP = (s_int16)(CIF_MEER_PARMWIJZ);  /* laat weten dat de applicatie de parm1[]-buffer heeft aangepast */
    }
  }
}


/**********************************************************************************/
void rhdhv_toggle_fictive_conflict(count fc1, count fc2, bool period)
{
  if (period)  /* fictief conflict gewenst */
  {
    if (TO_max[fc1][fc2] == NK ||
        TO_max[fc2][fc1] == NK   )
    {
      TO_max[fc1][fc2] = FK;
      TO_max[fc2][fc1] = FK;
      pointer_conflicts();  /* verander pointertabel */
      CIF_PARM1WIJZAP = (s_int16)(CIF_MEER_PARMWIJZ);  /* laat weten dat de applicatie de parm1[]-buffer heeft aangepast */
    }
  }
  else  /* geen ficitief conflict gewenst */
  {
    if (TO_max[fc1][fc2] == FK ||
        TO_max[fc2][fc1] == FK   )
    {
      TO_max[fc1][fc2] = NK;
      TO_max[fc2][fc1] = NK;
      pointer_conflicts();  /* verander pointertabel */
      CIF_PARM1WIJZAP = (s_int16)(CIF_MEER_PARMWIJZ);  /* laat weten dat de applicatie de parm1[]-buffer heeft aangepast */
    }
  }
}

/**********************************************************************************/
void rhdhv_timestamp(void)
{
  code datum[41];
  code tijd[41];
  
  datetostr(datum);                   /* standaardfunctie CCOL bericht.c */
  timetostr(tijd);                    /* standaardfunctie CCOL bericht.c */
  
  sprintf(RHDHV_TIJD,"%s %s |",datum,tijd);
}
