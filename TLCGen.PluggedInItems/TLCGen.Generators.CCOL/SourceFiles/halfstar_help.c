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
void wijzig_prm(count p, mulv newvalue)
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
void wijzig_timer(count t, mulv newvalue)
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
void wijzig_hiaattijd(count d, mulv newvalue)
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
void set_prmbit(count p, mulv bitje)
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
void reset_sch(count s)
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
void print_timestamp(void)
{
  code datum[41];
  code tijd[41];
  
  datetostr(datum);                   /* standaardfunctie CCOL bericht.c */
  timetostr(tijd);                    /* standaardfunctie CCOL bericht.c */
  
  sprintf(RHDHV_TIJD,"%s %s |",datum,tijd);
}
