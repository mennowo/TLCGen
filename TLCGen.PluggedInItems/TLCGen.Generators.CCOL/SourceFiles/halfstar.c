/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* CCOL - signaalplanstructuur bibliotheek Royal HaskoningDHV voor TPA generator V3.40    */
/*                                                                                        */
/*                                                                                        */
/* (C) Copyright 2015 Royal HaskoningDHV b.v. All rights reserved.                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* Versie :  1.0                                                                          */
/* Naam   :  rhdhv_ple.c                                                                  */
/* Datum  :  07-12-2015 kla: eerste versie                                                */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */

#include "halfstar.h"
#include "halfstar_ov.h"     /* declaratie functies                                      */

#if !defined AUTOMAAT || defined VISSIM
   #include "xyprintf.h"/* voor debug infowindow                                          */
   #include <stdio.h>      /* declaration printf()       */
#endif

void SetPlanTijden(count fc, mulv plan, mulv ta, mulv tb, mulv tc, mulv td, mulv te)
{
    TXA[plan][fc] = ta;
    TXB[plan][fc] = tb;
    TXC[plan][fc] = tc;
    TXD[plan][fc] = td;
    TXE[plan][fc] = te;
}

/*****************************************************************************/
/* vasthouden groen OV bij signaalplan */
void rhdhv_yv_ov_pl(count fc, bool bit, bool condition)
{
    RW[fc] &= ~bit;
    YV[fc] &= ~bit;

    if (condition)
    {
        YV[fc] |= (YV_PL[fc] && PR[fc] || AR[fc] && yv_ar_max_pl(fc, 0)) ? bit : 0; /* Vasthouden verlenggroen */
    }
}


/*****************************************************************************/
/* TXB_GEMIST - om te bepalen of een richting op tijd een aanvraag heeft  */
/* is altijd waar behalve wanneer het moment voor het TXB-moment optreedt */
/* wanneer de langste ontruimingstijd inclusief instelbare marge passeert */
/* ====================================================================== */
bool txb_gemist(count i, int marge)
{
    register count n, k;
    mulv to_max, to_tmp;

    if (R[i]) /* let op! i.v.m. snelheid alleen in R[] behandeld */
    {
        to_max=to_tmp= 0;

#ifndef TIGNEW
        for (n=0; n<KFC_MAX[i]; n++)
        {
            k= TO_pointer[i][n];
            if (TO[k][i])           /* zoek grootste ontruimingstijd   */
            {
                to_tmp= TGL_max[k]+TO_max[k][i]-TGL_timer[k]-TO_timer[k];
                if (to_tmp>to_max)
                    to_max= to_tmp;
            }
        }
#else
        for (n=0; n<FKFC_MAX[i]; n++)
        {
            k= TO_pointer[i][n];
            if (TIG_max[k][i]>=0)
            {
                to_tmp= TIG_max[k][i]-TIG_timer[k];
                if (to_tmp>to_max)  /* zoek grootste ontruimingstijd       */
                    to_max= to_tmp;
            }
        }
#endif
        if ((TOTXB_PL[i] > 0) && (TOTXB_PL[i] == (to_max + marge)))
            return (FALSE);
    }
    return (TRUE);
}


/**********************************************************************************/
/* Set functies voor het bepalen waar de plaats tussen TX momenten */
bool rhdhv_tussen_txa_en_txb(count fc)
{
    if (TXA_PL[fc] > TXB_PL[fc])
    {
        if ((TX_timer >= TXA_PL[fc]) || (TX_timer <  TXB_PL[fc]))
            return (TRUE);
    }
    else
    {
        if ((TX_timer >= TXA_PL[fc]) && (TX_timer <  TXB_PL[fc]))
            return (TRUE);
    }

    return (FALSE);
}

bool rhdhv_tussen_txb_en_txc(count fc)
{
    if (TXB_PL[fc]> TXC[PL][fc])
    {
        if ((TX_timer >= TXB_PL[fc]) || (TX_timer <  TXC[PL][fc]))
            return (TRUE);
    }
    else
    {
        if ((TX_timer >= TXB_PL[fc]) && (TX_timer <  TXC[PL][fc]))
            return (TRUE);
    }

    return (FALSE);
}

bool rhdhv_tussen_txb_en_txd(count fc)
{
    if (TXB_PL[fc]> TXD[PL][fc])
    {
        if ((TX_timer >= TXB_PL[fc]) || (TX_timer <  TXD[PL][fc]))
            return (TRUE);
    }
    else
    {
        if ((TX_timer >= TXB_PL[fc]) && (TX_timer <  TXD[PL][fc]))
            return (TRUE);
    }

    return (FALSE);
}

/*****************************************************************************/
void rhdhv_altcor_kop_pl(count fc_aan, count fc_af, count t_nl)
{
  /* tijdens rood van de nalooprichting kijken of er genoeg alternatieve realisatieruimte is,
     inclusief nalooptijd */
  if (R[fc_af])
  {
    if (!PAR[fc_af] || (tar_max_ple(fc_af) < (TFG_max[fc_af] + T_max[t_nl] + 10)))
      PAR[fc_aan] = FALSE;
  }
  /* tijdens groen van de nalooprichting kijken of er genoeg ruimte is voor de verlengging,
     inclusief nalooptijd */
  else
  {
    /* aanvoerende richting alleen alternatief tijdens groen van naloop als er 
       geen (fictief) conflicterende aanvraag is, of als naloop nog in vastgroen is */
    if ((!rhdhv_yv_ar_max_pl(fc_af, (mulv)(TFG_max[fc_aan] + T_max[t_nl] + 9))))
      PAR[fc_aan] = FALSE;
  }
  /* koppeling garanderen */
  if (RHDHV_IRT(t_nl))
    RR[fc_af] &= ~RHDHV_RR_ALTCOR;
}


/*****************************************************************************/
void wachtstand_halfstar(count fc, bool condition_a, bool condition_ws)
{
  count j, k;
  
  /* preset */
  if (IH[h_plact])
  {
    YW[fc] &= ~RHDHV_YW_WS;
    WS[fc] = FALSE;
  }
  /* dezelfde functie als aanvraag_wachtstand_exp() van stdfunc.c, maar nu wordt ook gekeken
     naar fictieve conflicten en wordt bit RHDHV_A_WS opgezet ipv BIT2 */
  if (condition_a && !A[fc] && RV[fc] && !K[fc] && !TRG[fc] && !RR[fc] && !BL[fc]) 
  {
    for (j=0; j<FKFC_MAX[fc]; j++) 
    {
      k = TO_pointer[fc][j];
      
      /* resetten wachtstand aanvraag bij (fictief) conflicterende aanvraag */
      if (A[k])
      {
        A[fc] &= ~RHDHV_A_WS;
        if (!A[fc] && RA[fc])
        {
          RR[fc] |= TRUE;
          TFB_timer[fc] = 0;
        }
      }
      if (A[k] && !BL[k] || TRG[k] || SG[k] || GL[k])  
        return;
    }
    A[fc] |= RHDHV_A_WS;
  }

  /* wachtstand opzetten */
  WS[fc] = (bool)(WG[fc] && rhdhv_yws_groen_fk(fc) && condition_ws);
  RHDHV_BIT(YW[fc], (bool)(WS[fc]), RHDHV_YW_WS);
  
}


/*****************************************************************************/
void rhdhv_reset_PLE(void)
{
  register count i;
  /* PL=0;                /. active signalplan                    */
  /* APL=0;               /. demand of signalplan                 */
  /* SPL=FALSE;           /. start of a new signalplan            */
  RTX=FALSE;            /* restart TX -> TX_timer==1            */
  HTX=FALSE;            /* halt TX                              */
  FTX=FALSE;            /* fast TX                              */
  for(i=0; i<FC_MAX; i++)
  {
    RR_PL[i]=           /* no realisation                       */
    YS_PL[i]=           /* hold instruction advanced-greenphase */
    YW_PL[i]=           /* hold instruction waiting greenphase  */
    YV_PL[i]=           /* hold instruction veh. act.-greenphase*/
    YM_PL[i]=FALSE;     /* hold instruction parallel-greenphase */

    PR[i]=FALSE;        /* primary realisation                  */
    AR[i]=FALSE;        /* alternative realisation              */
    PG[i]=FALSE;        /* primary realized                     */
/*  AG[i]=FALSE;        /. alternative realized                 */
    PP[i]=FALSE;        /* privelege periode prim. realisation  */
    WS[i]=FALSE;        /* waiting green - phasecycle           */
  }
}


/* als een richting aan het meeverlengen is en het maximum groen nog niet is gehaald en weer verkeer op de lussen is,
   de richting terugsturen naar verlenggroen (via WG) */
void Verlengroen_na_Meeverlenggroen_PL(count fc, count prmvgmg)
{
  if (MG[fc] && !FM[fc] && !Z[fc] && PR[fc] && MK[fc] && rhdhv_tussen_txb_en_txd(fc) && 
      (TOTXD_PL[fc] >= PRM[prmvgmg]) && !kcv(fc))
  {
    RW[fc] |= RHDHV_RW_VGNAMG;
  }
}


/**********************************************************************************/
void set_ym_pl_halfstar(count fc, bool condition)
{
  if (ym_max_to(fc, NG)      &&   /* meeverlengen kan volgens ontruimingstijden      */
      ym_max_tig(fc, NG)     &&   /* meeverlengen kan volgens intergroentijdentabel  */
      rhdhv_ym_max_pl(fc, 10)&&   /* meeverlengen kan volgens signaalplan            */
      hf_wsg()               &&   /* minimaal 1 richting actief                      */
      condition             )
  {
    YM[fc] |= RHDHV_YM_PL;
  }
}


/**********************************************************************************/
/* corrigeert het meeverlengen van de aanvoerende richting van een hard gekoppelde interne richting */
void rhdhv_mgcor_pl(count fcaan, count fcnal, count t_nal)
{
  if (!rhdhv_ym_max_pl(fcnal, (mulv)(T_max[t_nal] + 9))) /* +9 vanwege afronding m.g.b. 10/sec ORT's */
    YM[fcaan] &= ~RHDHV_YM_PL;
} 


/**********************************************************************************/
/* correcties voor parallell langzaamverkeer en autoverkeer in deelconflict tijdens PL */
void rhdhv_mgcor_pl_deelc(count fc1, count fc2)
{
  if (RA[fc1])
    YM[fc2] &= ~RHDHV_YM_PL;
}


/**********************************************************************************/
void hardekoppeling_halfstar(bool period, count fc1, count fc2, count tvs, count tnldet, count tnl)
{
  /* geel- en garantieroodtimer tbv tegenhouden aanvoerrichting */
  if (GL[fc2])
    geeltimer[fc1][fc2] += TE;
  if (TRG[fc2])
    groodtimer[fc1][fc2] += TE;
  if ((R[fc2] && !TRG[fc2]) || G[fc2])
  {
    geeltimer[fc1][fc2] = 0;
    groodtimer[fc1][fc2] = 0;
  }
  
  RT[tnl] = (bool)(G[fc1] && period);
  AT[tnl] = (bool)(ERA[fc1] && SRV[fc1]);
  if (tnldet != NG)
  {
    RT[tnldet] = SG[fc1];
    AT[tnldet] = (bool)(ERA[fc1] && SRV[fc1]);
  }

  if (period && (G[fc1] || GL[fc1] || RHDHV_IRT(tnl) || ((tnldet != NG) && RHDHV_IRT(tnldet))))
  {
    if (CG[fc1] < CG_MG)
    {
      if ((YV_PL[fc2] && PR[fc2]) || (rhdhv_yv_ar_max_pl(fc2, (mulv)(T_max[tnl] + 9) && AR[fc2])))
      {
        YV[fc2] |= RHDHV_YV_KOP;
      }
    }
    
    /* naloop in verlenggroen houden, zodat naloop nog kan verlengen na koppeling. Maar als de 
       aanvoer in MG komt, dan naloop ook in MG anders kan de correctie van de koppeling
       op het meeverlengen niet worden uitgevoerd. Na einde MG van aanvoer wordt naloop weer
       VG zodat na de koppeling weer kan worden verlengd */
    RW[fc2] |= SG[fc1] ? RHDHV_RW_KOP : 0;
    
    YM[fc2] |= RHDHV_YM_KOP;
  }
  
  /* meerealisatie nalooprichting */
  rhdhv_MR(fc2, fc1, (bool)(period && ((A[fc1] &&  R[fc1]) || CV[fc1])));
  
  if(tvs != NG)
  {
    /* aanvoerende richting niet te snel realiseren */
    if (period && x_aanvoer(fc2, T_max[tvs]) && (TX_timer != TXB[PL][fc1]))
      X[fc1] |= RHDHV_X_VOOR;
    
    /* ten behoeve van verklikken */
    RT[tvs] = SG[fc1];

    /* tegenhouden aanvoerende richting rekening houden met geel en garantieroodtijd; x_aanvoer doet dit niet! */
    if (period && (GL[fc2] || TRG[fc2] && (TX_timer != TXB[PL][fc1])))
    {
      if (((TGL_max[fc2] + TRG_max[fc2]) - (geeltimer[fc1][fc2] + groodtimer[fc1][fc2])) > T_max[tvs])
        X[fc1] |= RHDHV_X_VOOR;
    }
  }
  
  /* als nalooprichting worden tegengehouden, dan ook aanvoerende richting tegenhouden */
  if (period && ((RR[fc2]) && !(TX_timer == TXB[PL][fc1])))
  {
    RR[fc1] |= RHDHV_RR_KOP;
  }
  
  /* koppeling garanderen */
  if (period && (RHDHV_IRT(tnl) || ((tnldet != NG) && RHDHV_IRT(tnldet))))
    RR[fc2] &= ~RHDHV_RR_ALTCOR;
  
}


/**********************************************************************************/
void rhdhv_zachtekoppeling_pl(bool period, count fc1, count fc2, count tvs, count tnldet, count tnl)
{
  /* geel- en garantieroodtimer tbv tegenhouden aanvoerrichting */
  if (GL[fc2])
    geeltimer[fc1][fc2] += TE;
  if (TRG[fc2])
    groodtimer[fc1][fc2] += TE;
  if ((R[fc2] && !TRG[fc2]) || G[fc2])
  {
    geeltimer[fc1][fc2] = 0;
    groodtimer[fc1][fc2] = 0;
  }
  
  RT[tnl] = (bool)(G[fc1] && period && !WS[fc1]);
  AT[tnl] = (bool)((ERA[fc1] && SRV[fc1]) || WS[fc1]);
  if (tnldet != NG)
  {
    RT[tnl] = SG[fc1];
    AT[tnl] = (bool)(ERA[fc1] && SRV[fc1]);
  }

  if (period && ((G[fc1] && !WS[fc1]) || GL[fc1] || RHDHV_IRT(tnl) || ((tnldet != NG) && RHDHV_IRT(tnldet))))
  {
    if ((CG[fc1] < CG_MG))
    {
      if ((YV_PL[fc2] && PR[fc2]) || (rhdhv_yv_ar_max_pl(fc2, (mulv)(T_max[tnl] + 9) && AR[fc2])))
      {
        YV[fc2] |= RHDHV_YV_KOP;
        RW[fc2] |= SG[fc1] ? RHDHV_RW_KOP : 0;
      }
    }
  }
  
  /* ten behoeve van verklikken */
  if (tvs != NG)
    RT[tvs] = SG[fc1];

}


/*****************************************************************************/
/* PP bitje opzetten en cyclische aanvraag op TXB moment als PP waar is      */
void rhdhv_set_pp(count fc, bool condition, count value)
{
  if (!condition)
    return; 

  PP[fc] |= value;
  
  if (PP[fc])
    PG[fc] &= ~PRIMAIR_OVERSLAG;
  
  /* Vaste aanvragen (half)starre programma: */
  if (aanvraag_txb(fc) && PP[fc]) 
    A[fc] |= TRUE;
}


/*****************************************************************************/
/* 'variabel' TXC moment voor bijvoorbeeld gekoppelde richtingen */
void rhdhv_var_txc(count fc, bool condition)
{
  YW[fc] &= ~RHDHV_YW_VAR_TXC;
  RW[fc] &= ~RHDHV_RW_VAR_TXC;
  
  YW[fc] |= condition && rhdhv_tussen_txb_en_txc(fc) ? RHDHV_YW_VAR_TXC : 0;
  RW[fc] |= condition && rhdhv_tussen_txb_en_txc(fc) ? RHDHV_RW_VAR_TXC : 0;
  
}


/*****************************************************************************/
void rhdhv_pl_fc(count fc_a, /* eerste fasecyclus                    */ 
                 count fc_e) /* laatste fasecyclus                   */
{
  register count fc;            /* loopindex */
  
  /* beveiliging verkeerde invoer: */
  if (fc_a < 0)                        fc_a = 0;
  if ((fc_e < 0) || (fc_e >= FC_MAX))  fc_e = (FC_MAX - 1);

  for (fc = fc_a; fc <= fc_e; fc++)
  {
    /* Tegenhouden realisatie indien txa gedefinieerd: */
    RR[fc] |= (bool)(RR_PL[fc] && !PAR[fc]) ? RHDHV_RR_PL : 0;
    
    /* Vasthouden wachtgroen: */
    if (TXC_PL[fc]> 0)
      YW[fc] |= (bool)(PR[fc] && YW_PL[fc]) ? RHDHV_YW_PL: 0;
    
    /* Vasthouden verlenggroen: */
    YV[fc] |= (bool)(MK[fc] && ((PR[fc] && YV_PL[fc]) ||                /* primair      */
                                (AR[fc] && rhdhv_yv_ar_max_pl(fc, 9))) )  /* alternatief  */
              ? RHDHV_YV_PL: 0;
    
    /* Afbreken V.A. verlenggroen: */
    FM[fc] |= (bool)(!R[fc] && !YV[fc] && PR[fc]) ? RHDHV_FM_PL: 0;
    
  }
}


/**********************************************************************************/
/* aangepaste functie van ccol bibliotheek (plfunc.c). Naast een aanvraag wordt ook gekeken naar
   PP van een conflicterende richting. De aanvraag van een hoofdrichting wordt pas op TXB moment gezet */
bool rhdhv_ym_max_pl(count i, mulv koppeltijd)
{
  register count n, k;
  bool ym;
  if (MG[i] && (YM_PL[i] || !PR[i])) 
  {
    ym= TRUE;

    for (n=0; n<FKFC_MAX[i]; n++) 
    {
      k = TO_pointer[i][n];
      if (TIG_max[i][k] >= 0) 
      {
        if ((TOTXB_PL[k] > 0) && !PG[k] && R[k] && (A[k] || PP[k]) || ((TOTXB_PL[k] == 0) && RA[k])) 
        {
          if ((TIG_max[i][k] + koppeltijd) >= (TOTXB_PL[k] - 9))  /* -9 om rekening te houden met ORT's in tienden van seconden          */
          {                                                       /* (refresh van TOTXB_PL is maar per seconde en niet per 1/10 seconde) */
            ym= FALSE;
            break;
          }  
        }
      }
    }
  }
  else  
  {
    ym= CV[i];
  }
  return ym;
}


/**********************************************************************************/
/* aangepaste functie van ccol bibliotheek (plfunc.c). Naast een aanvraag wordt ook gekeken naar
   PP van een conflicterende richting. De aanvraag van een hoofdrichting wordt pas op TXB moment gezet.
   Tevens ook berekening uitvoeren tijdens WG en MG, want een evt nalooprichting kan door TXC moment 
   worden vastgehouden of kan meeverlengen met andere richtingen */
bool rhdhv_yv_ar_max_pl(count i, mulv koppeltijd)
{
  register count n, k;
  bool yv;
 
  if (WG[i] || VG[i] || MG[i]) 
  {
    yv= TRUE;
  
    for (n=0; n<FKFC_MAX[i]; n++) 
    {
      k= TO_pointer[i][n];
    
      if (TIG_max[i][k]>=0) 
      {
        if (TOTXB_PL[k]>0 && !PG[k] && R[k] && (A[k] || PP[k]) || TOTXB_PL[k]==0 && RA[k]) 
        {
          if ((TIG_max[i][k]+koppeltijd) >= (TOTXB_PL[k])) 
          {
            yv = FALSE;
            break;
          }
        }
      }
    }
  }
  else  
  {
    yv= FALSE;
  }
  
  return yv;
}


/*****************************************************************************/
void rhdhv_alternatief_pl(count fc, mulv altp, bool condition)
{
  PAR[fc] = FALSE;
    
  /* als een alternatieve ruimte wordt opgegeven, dan PAR opzetten bij voldoende ruimte, anders
     kijken naar vastgroentijd. Minimum is altijd vastgroen 
     Let op extra 4 vanwege aantal stappen in ccol (FG->WG->VG->MG->GL)  */
  if (altp > TFG_max[fc]) 
    PAR[fc] = (bool)((tar_max_ple(fc) >= (mulv)(altp + 11)) && condition);
  else
    PAR[fc] = (bool)((tar_max_ple(fc) >= (mulv)(TFG_max[fc] + 11)) && condition);

  /* afbreken alternatieve realisatie t.b.v. primair realiserend conflict: */
  FM[fc] = fm_ar_kpr(fc, TFG_max[fc]);

}


/*****************************************************************************/
/* alternatieve correcties voor paralelle voetgangsers en fietsers */
void rhdhv_altcor_parftsvtg_pl(count fc1, count fc2, bool voorwaarde)
{
  if (voorwaarde)
  {
    if (A[fc2] && R[fc2] && !PAR[fc2])
      PAR[fc1] = FALSE;
    if (A[fc1] && R[fc1] && !PAR[fc1])
      PAR[fc2] = FALSE;
    
    /* als 1 richting in RA staat, maar de andere nog een conflict heeft,
       dan een RR */
    /*if (kcv(fc1) && A[fc1] && !RA[fc1])
      RR[fc2] |= RHDHV_RR_ALTCOR;
    if (kcv(fc2) && A[fc2] && !RA[fc2])
      RR[fc1] |= RHDHV_RR_ALTCOR;
    */
  }  
}


/*****************************************************************************/
void rhdhv_altcor_kopvtg_pl(count fc1, count fc2, bool a_bui_fc1, bool a_bui_fc2, count tnlsg12, count tnlsg21, bool voorwaarde)
{
  if (voorwaarde)
  {
    if (a_bui_fc1 || a_bui_fc2)
    {
      if (!(PAR[fc2] && (tar_max_ple(fc2) > (T_max[tnlsg12] + 11))))
        PAR[fc1] = FALSE;
      if (!(PAR[fc1] && (tar_max_ple(fc1) > (T_max[tnlsg21] + 11))))
        PAR[fc2] = FALSE;
    }
    
    if (a_bui_fc1 && !PAR[fc1])
      PAR[fc2] = FALSE;
    if (a_bui_fc2 && !PAR[fc2])
      PAR[fc1] = FALSE;
    
    /* als PAR bitje ingetrokken, dan als in RA ook RR instructie */
    /* kla wordt in applicatie uitgevoerd */
    /*if (R[fc2])
      dtv_set_rr_par(fc1);
    if (R[fc1])
      dtv_set_rr_par(fc2);*/
  }
}


/*****************************************************************************/
/* alternatieve correcties voor tegengestelde richtingen */
void rhdhv_altcor_parfts_pl(count fc1, count fc2, bool voorwaarde)
{
  if (voorwaarde)
  {
    if (A[fc2] && R[fc2] && !PAR[fc2])
      PAR[fc1] = FALSE;
    if (A[fc1] && R[fc1] && !PAR[fc1])
      PAR[fc2] = FALSE;
  }  
}


/**********************************************************************************/
void rhdhv_getrapte_voetganger_pl(count fc1             , /* fc1 */
                                  count fc2             , /* fc2 */
                                  bool  a_bui_fc1       , /* buitendrukknopaanvraag fc1 */
                                  bool  a_bui_fc2       , /* buitendrukknopaanvraag fc2 */
                                  count tkopfc1fc2      , /* naloop (SG) fc1 -> fc2 */
                                  count tkopfc2fc1      , /* naloop (SG) fc2 -> fc1 */
                                  count voorstartfc1fc2 , /* maximale voorstart fc1 -> fc2 (mag NG) */
                                  count voorstartfc2fc1 ) /* maximale voorstart fc2 -> fc1 (mag NG) */
{
  /* nalopen */
  RT[tkopfc1fc2] = (bool)(RA[fc1] && a_bui_fc1);
  AT[tkopfc1fc2] = (bool)((ERA[fc1] && SRV[fc1]));
  if ((RT[tkopfc1fc2] || T[tkopfc1fc2]) && ((YV_PL[fc2] && PR[fc2]) || 
       (rhdhv_yv_ar_max_pl(fc2, (mulv)(T_max[tkopfc1fc2] - TFG_max[fc2])) && AR[fc2])))
  {
    YV[fc2] |= RHDHV_YV_KOP;
  }
  
  RT[tkopfc2fc1] = (bool)(RA[fc2] && a_bui_fc2);
  AT[tkopfc2fc1] = (bool)((ERA[fc2] && SRV[fc2]));
  if ((RT[tkopfc2fc1] || T[tkopfc2fc1]) && ((YV_PL[fc1] && PR[fc1]) || 
       (rhdhv_yv_ar_max_pl(fc1, (mulv)(T_max[tkopfc2fc1] - TFG_max[fc1])) && AR[fc1])))
  {
    YV[fc1] |= RHDHV_YV_KOP;
  }
    
  /* meerealisaties */
  rhdhv_MR(fc1, fc2, (bool)(a_bui_fc2 && R[fc2] && (A[fc2] != RHDHV_A_WS)));
  rhdhv_MR(fc2, fc1, (bool)(a_bui_fc1 && R[fc1] && (A[fc1] != RHDHV_A_WS)));
  
  /* aanvoerende richting niet te snel realiseren (maximale voorstarttijd gelijk aan nalooptijd) */
  if (voorstartfc1fc2 != NG)
  {
    if ((voorstartfc1fc2 > 0) && a_bui_fc1)
    {
      if (x_aanvoer(fc2, (mulv)(voorstartfc1fc2)))
      {
        X[fc1] |= RHDHV_X_VOOR;
      }
    }
  }
  if (voorstartfc2fc1 != NG)
  {
    if ((voorstartfc2fc1 > 0) && a_bui_fc2)
    {
      if (x_aanvoer(fc1, (mulv)(voorstartfc2fc1)))
      {
        X[fc2] |= RHDHV_X_VOOR;
      }
    }
  }
}


/**********************************************************************************/
void rhdhv_getrapte_fietser_pl(count fc1             , /* fc1 */
                               count fc2             , /* fc2 */
                               bool  a_bui_fc1       , /* buitendrukknopaanvraag fc1 */
                               bool  a_bui_fc2       , /* buitendrukknopaanvraag fc2 */
                               count tkopfc1fc2      , /* naloop (SG) fc1 -> fc2 */
                               count tkopfc2fc1      , /* naloop (SG) fc2 -> fc1 */
                               count voorstartfc1fc2 , /* maximale voorstart fc1 -> fc2 (mag NG) */
                               count voorstartfc2fc1 ) /* maximale voorstart fc2 -> fc1 (mag NG) */
{
  /* nalopen */
  RT[tkopfc1fc2] = (bool)((RA[fc1] || TFG[fc1]) && a_bui_fc1);
  AT[tkopfc1fc2] = (bool)((ERA[fc1] && SRV[fc1]));
  if ((RT[tkopfc1fc2] || T[tkopfc1fc2]) && ((YV_PL[fc2] && PR[fc2]) /*|| 
       (rhdhv_yv_ar_max_pl(fc2, (mulv)(T_max[tkopfc1fc2] - TFG_max[fc2])) && AR[fc2])*/))
  {
    YV[fc2] |= RHDHV_YV_KOP;
  }
  
  RT[tkopfc2fc1] = (bool)((RA[fc2] || TFG[fc2]) && a_bui_fc2);
  AT[tkopfc2fc1] = (bool)((ERA[fc2] && SRV[fc2]));
  if ((RT[tkopfc2fc1] || T[tkopfc2fc1]) && ((YV_PL[fc1] && PR[fc1]) /*|| 
       (rhdhv_yv_ar_max_pl(fc1, (mulv)(T_max[tkopfc2fc1] - TFG_max[fc1])) && AR[fc1])*/))
  {
    YV[fc1] |= RHDHV_YV_KOP;
  }
    
  /* meerealisaties */
  rhdhv_MR(fc1, fc2, (bool)(PR[fc2] && a_bui_fc2 && R[fc2] && (A[fc2] != RHDHV_A_WS)));
  rhdhv_MR(fc2, fc1, (bool)(PR[fc1] && a_bui_fc1 && R[fc1] && (A[fc1] != RHDHV_A_WS)));
  
  /* aanvoerende richting niet te snel realiseren (maximale voorstarttijd gelijk aan nalooptijd) */
  /*if (voorstartfc1fc2 != NG)
  {
    if ((voorstartfc1fc2 > 0) && a_bui_fc1)
    {
      if (x_aanvoer(fc2, (mulv)(voorstartfc1fc2)))
      {
        X[fc1] |= RHDHV_X_VOOR;
      }
    }
  }
  if (voorstartfc2fc1 != NG)
  {
    if ((voorstartfc2fc1 > 0) && a_bui_fc2)
    {
      if (x_aanvoer(fc1, (mulv)(voorstartfc2fc1)))
      {
        X[fc2] |= RHDHV_X_VOOR;
      }
    }
  }*/
}


/**********************************************************************************/
/* 2e REALISATIE BINNEN SIGNAALPLAN */
void TweedeRealisatie(count fc_1,  /* fasecyclus 1e realisatie */
                      count fc_2)  /* fasecyclus 2e realisatie */
{
    register count tot_txb1,
                   tot_txb2,
                   txa1,
                   txb1,
                   txc1,
                   txd1,
                   txe1;

    /* berekenen tijd tot TXB-moment realisatie 1 (mits TXB fc_1 is ingevuld, anders  */
    /* tot_txb1 groter maken dan cyclustijd waardoor realisatie 1 niet wordt gekozen) */
    if (TXB[PL][fc_1] > 0)
    {
        if (TX_timer < TXB[PL][fc_1])     tot_txb1 = TXB[PL][fc_1]              - TX_timer;
        else                              tot_txb1 = TXB[PL][fc_1] + TX_max[PL] - TX_timer;
    }
    else
    {
        tot_txb1 = TX_max[PL] + 1;
    }

    /* berekenen tijd tot TXB-moment realisatie 2 (mits TXB fc_2 is ingevuld, anders  */
    /* tot_txb2 groter maken dan cyclustijd waardoor realisatie 2 niet wordt gekozen) */
    if (TXB[PL][fc_2] > 0)
    {
        if (TX_timer < TXB[PL][fc_2])     tot_txb2 = TXB[PL][fc_2]              - TX_timer;
        else                              tot_txb2 = TXB[PL][fc_2] + TX_max[PL] - TX_timer;
    }
    else
    {
        tot_txb2 = TX_max[PL] + 1;
    }

    /* omschakelen naar 2e realisatie als 2e realisatie eerstvolgende is     */
    /* en 1e realisatie voorbij het TXD-moment is (2e wordt 1e en omgekeerd) */
    /* --------------------------------------------------------------------- */
    if ((tot_txb2 < tot_txb1) && !rhdhv_tussen_txb_en_txd(fc_1) && !G[fc_1])
    {
        /* oude waarden 1e realisatie even onthouden */
        txa1 = TXA[PL][fc_1];
        txb1 = TXB[PL][fc_1];
        txc1 = TXC[PL][fc_1];
        txd1 = TXD[PL][fc_1];
        txe1 = TXE[PL][fc_1];

        /* waarden 2e realisatie kopiëren naar 1e realisatie */
        TXA[PL][fc_1] = TXA[PL][fc_2];
        TXB[PL][fc_1] = TXB[PL][fc_2];
        TXC[PL][fc_1] = TXC[PL][fc_2];
        TXD[PL][fc_1] = TXD[PL][fc_2];
        TXE[PL][fc_1] = TXE[PL][fc_2];

        /* oude waarden 1e realisatie kopiëren naar 2e realisatie */
        TXA[PL][fc_2] = txa1;
        TXB[PL][fc_2] = txb1;
        TXC[PL][fc_2] = txc1;
        TXD[PL][fc_2] = txd1;
        TXE[PL][fc_2] = txe1;

        copy_signalplan(PL);              /* kopieer de nieuwe signaalplantijden naar de werkvariabelen */
        CIF_PARM1WIJZAP = (s_int16)(-2);  /* laat weten dat de applicatie TX-tijden heeft anngepast     */

        PG[fc_1] = FALSE;
    }
}


/**********************************************************************************/
/* tar_max_ple() wordt gebruikt bij de specificatie van de instructie-
 * variabele PPA[] (period alternatieve) van de fasecyclus.
 * tar_max_ple() geeft de maximum tijd in tienden van seconden die beschik-
 * baar is voor de alternatieve realisatie van de fasecyclus.
 * het aanroepen van de functie tar_max_ple() dient in de applicatiefunctie
 * application() te worden gespecificeerd.
 */
/*   ---------xxx                                              fc_k
 *                                                             ---------xxx
 *
 *            |                          TOTXB_PL[k]          |
 *                                  | TGL_max[i]+TO_max[i][k] |
 *            | to_txb_min          |
 *            | to_max |
 *                     | tar_max_i  |
 */

mulv rhdhv_tar_max_ple(count i)
{
  register count n, k;
  mulv totxb_min, totxb_tmp;
  mulv to_max, to_tmp;
  mulv t_aa_max;
  
  if (!GL[i] && !TRG[i])
  {     /* let op! i.v.m. snelheid alleen in !GL[] && !TRG[] behandeld   */
    t_aa_max= 0;
    totxb_min= 32767;
    totxb_tmp= 0;
    to_max=to_tmp= 0;
  
#ifdef NO_TIG
    for (n=0; n<KFC_MAX[i]; n++)
    {
      k= TO_pointer[i][n];
      if (CV[k] || G[k] && (!MG[k] || RS[k] || RW[k]) || TGG[k])
      {
        t_aa_max= NG;
        break;
      }
      if (TO[k][i])
      {         /* zoek grootste ontruimingstijd   */
        to_tmp= TGL_max[k]+TO_max[k][i]-TGL_timer[k]-TO_timer[k];
        if (to_tmp>to_max)
        to_max= to_tmp;
      }
      if ((TOTXB_PL[k] || (TX_PL_timer==TXB_PL[k])) && (A[k] && R[k] || PP[k]) && !PG[k])   /* @@ 25-8-1998 */
      {
        totxb_tmp=TOTXB_PL[k] - TO_max[i][k] - TGL_max[i];/* @@ 9-8-1998 PG[i] gewijzigd in PG[k] */
        if (totxb_tmp<totxb_min) /* zoek kleinste starttijd confl. fc  */
            totxb_min= totxb_tmp;
      }
    }
    for (n=KFC_MAX[i]; n<FKFC_MAX[i]; n++)
    {
      k = TO_pointer[i][n];
      if ((TOTXB_PL[k] || (TX_PL_timer==TXB_PL[k])) && (A[k] && R[k] || PP[k]) && !PG[k])
      {
        totxb_tmp=TOTXB_PL[k];
        if (totxb_tmp<totxb_min) /*zoek kleinste starttijd confl. fc*/
            totxb_min= totxb_tmp;
       }
    }
#else
    for (n=0; n<FKFC_MAX[i]; n++)
    {
      k= TO_pointer[i][n];
      if (TIG_max[k][i]>=0)
      {
        if (CV[k] || G[k] && (!MG[k] || RS[k] || RW[k]) || TGG[k])
        {
          t_aa_max= NG;
          break;
        }
        else
          to_tmp= TIG_max[k][i]-TIG_timer[k];

        if (to_tmp>to_max)      /* zoek grootste ontruimingstijd   */
            to_max= to_tmp;
      }
      if (TIG_max[i][k]>=0)
      {
        if ((TOTXB_PL[k] || (TX_PL_timer==TXB_PL[k])) && (A[k] && R[k] || PP[k]) && !PG[k])
        {
          totxb_tmp=TOTXB_PL[k] - TIG_max[i][k];
          if (totxb_tmp<totxb_min) /*zoek kleinste starttijd confl. fc*/
              totxb_min= totxb_tmp;
        }
      }
      else if (TIG_max[i][k]==FK)
      {
        if ((TOTXB_PL[k] || (TX_PL_timer==TXB_PL[k])) && (A[k] && R[k] || PP[k]) && !PG[k])
        {
          totxb_tmp=TOTXB_PL[k];
          if (totxb_tmp<totxb_min) /*zoek kleinste starttijd confl. fc*/
              totxb_min= totxb_tmp;
        }
      }
    }
#endif
    if (t_aa_max>=0)
    {
      t_aa_max= totxb_min - to_max;
    }
  }
  else
    t_aa_max= NG;

    return t_aa_max;
}


/**********************************************************************************/
/* percentage MG bij defect alle kop/lange lussen */

void rhdhv_detstor_ple(count fc, count dkop, count dlang1, count dlang2, count prm_perc)
{
  if (dlang2 == NG)
  {
    if ((CIF_IS[dkop] >= CIF_DET_STORING) && (CIF_IS[dlang1] >= CIF_DET_STORING))
    {   
      MK[fc] |= BIT5;
      PercentageMaxGroenTijdenSP(fc, prm_perc);
    }
  }
  else
  {
    if ((CIF_IS[dkop] >= CIF_DET_STORING) && (CIF_IS[dlang1] >= CIF_DET_STORING) && (CIF_IS[dlang2] >= CIF_DET_STORING))
    {   
      MK[fc] |= BIT5;
      PercentageMaxGroenTijdenSP(fc, prm_perc);
    }
  }
  
}

/**********************************************************************************/
void sync_pg(void)
{
  register count fc;
  
  for (fc=0; fc<FCMAX; fc++)
  {
    if (PG[fc] && !PRML[ML][fc] && !PRML[(ML+1==MLMAX ? ML1 : ML+1)][fc])
    {
      PG[fc] = FALSE;
    }
    if (IH[hmlact] && RA[fc] && !AAPR[fc] && !PAR[fc])
      RR[fc] |= BIT9;
  }
}


/**********************************************************************************/
/* YS_PL[] zelf opzetten bij primair startgroen, dit gebeurt nl. 
   niet als een fasecyclus voor zijn TXA-moment primair groen krijgt */
void rhdhv_set_yspl(count fc)
{
  if (SG[fc] && PR[fc] && (TXA_PL[fc]> 0))  
    YS_PL[fc]= TRUE;
}


/**********************************************************************************/
/* Voorstartgroen tijdens voorstart t.o.v. sg-plan */
void rhdhv_vs_ple(count fc, count prmtotxa, bool condition)
{
  /*RS[fc] |= YS_PL[fc] && PR[fc] || 
            ((TOTXA_PL[fc]>0) && (TOTXA_PL[fc]<PRM[prmtotxa])) ? RHDHV_RS_PLE : 0;*/
  RS[fc] |= condition /*&& YS_PL[fc]*/ && (TXA_PL[fc]> 0) && (rhdhv_tussen_txa_en_txb(fc) && PP[fc]) ? RHDHV_RS_PLE : 0;
}


/**********************************************************************************/
/* Retour wachtgroen bij wachtgroen richtingen */
void rhdhv_wg_ple(count fc, bool condition)
{
  RW[fc] |= YW[fc] || (TOTXB_PL[fc] <= (TRG_max[fc]+TGL_max[fc])) &&
            (/*(TOTXB_PL[fc]>0) ||*/ condition && !ka(fc) || (TX_timer == TXB_PL[fc])) ? RHDHV_RW_WG : 0;

}

/*****************************************************************************/
void SetPlanTijden2R(count fc, mulv plan, mulv ta  , mulv tb  , mulv tc  , mulv td  , mulv te  , 
                     count fc_2,          mulv ta_2, mulv tb_2, mulv tc_2, mulv td_2, mulv te_2)
{
    TXA[plan][fc] = ta;
    TXB[plan][fc] = tb;
    TXC[plan][fc] = tc;
    TXD[plan][fc] = td;
    TXE[plan][fc] = te;
    TXA[plan][fc_2] = ta_2;
    TXB[plan][fc_2] = tb_2;
    TXC[plan][fc_2] = tc_2;
    TXD[plan][fc_2] = td_2;
    TXE[plan][fc_2] = te_2;
}


/**********************************************************************************/
void rhdhv_tx_change(count fc        , /* signaalgroep         */
                     count pl        , /* actieve plan         */
                     count ptxa1     , /* eerste a realisatie    */
                     count ptxb1     , /* eerste b realisatie    */
                     count ptxc1     , /* eerste c realisatie    */
                     count ptxd1     , /* eerste d realisatie    */
                     count ptxe1     , /* eerste e realisatie    */
                     count ptxa2     , /* tweede a realisatie    */
                     count ptxb2     , /* tweede b realisatie    */
                     count ptxc2     , /* tweede c realisatie    */
                     count ptxd2     , /* tweede d realisatie    */
                     count ptxe2     , /* tweede e realisatie    */
                     bool  condition) /* conditie             */
{
  /* als geen wissel toegepast mag worden of parameters zijn 0 */
  if ((PRM[ptxb1] == 0) && (PRM[ptxb1] == 0) && (PRM[ptxb2] == 0) && (PRM[ptxb2] == 0))
  {
    return;
  }
  
  /* als geen tweede realisatie mag worden uitgevoerd, dan eerste realisatie instellen */
  if (!condition || (PRM[ptxb2] == 0) || (ptxb2==NG) || (pl!=PL))
  {
    if (TXA[pl][fc] != PRM[ptxa1])
    {
      TXA[pl][fc] = PRM[ptxa1];
      RHDHV_COPY_2_TIG = TRUE;
    }
    
    if (TXB[pl][fc] != PRM[ptxb1])
    {
      TXB[pl][fc] = PRM[ptxb1];
      RHDHV_COPY_2_TIG = TRUE;
    }
    if (TXC[pl][fc] != PRM[ptxc1])
    {
      TXC[pl][fc] = PRM[ptxc1];
      RHDHV_COPY_2_TIG = TRUE;
    }
    if (TXD[pl][fc] != PRM[ptxd1])
    {
      TXD[pl][fc] = PRM[ptxd1];
      RHDHV_COPY_2_TIG = TRUE;
    }
    if (TXE[pl][fc] != PRM[ptxe1])
    {
      TXE[pl][fc] = PRM[ptxe1];
      RHDHV_COPY_2_TIG = TRUE;
    }
    return;
  }
  
  if (PL==pl)
  {
    if (rhdhv_pl_gebied(NG, (mulv)(PRM[ptxd1] + 1), (mulv)(PRM[ptxd2])) || 
       (rhdhv_pl_gebied(NG, (mulv)(TXB_PL[fc]), (mulv)(TXD_PL[fc])) && EG[fc]))
    {
      if (TXA_PL[fc]!= PRM[ptxa2])
      {
        TXA[PL][fc] = PRM[ptxa2];
        TXA_PL[fc] = PRM[ptxa2];
      }
      if (TXB_PL[fc]!= PRM[ptxb2])
      {
        TXB[PL][fc] = PRM[ptxb2];
        TXB_PL[fc] = PRM[ptxb2];
      }
      if (TXC_PL[fc]!= PRM[ptxc2])
      {
        TXC[PL][fc] = PRM[ptxc2];
        TXC_PL[fc] = PRM[ptxc2];
      }
      if (TXD_PL[fc]!= PRM[ptxd2])
      {
        TXD[PL][fc] = PRM[ptxd2];
        TXD_PL[fc] = PRM[ptxd2];
      }
      //RHDHV_COPY_2_TIG = TRUE;
      check_signalplans();
    }

    if (rhdhv_pl_gebied(NG, (mulv)(PRM[ptxd2] + 1), (mulv)(PRM[ptxd1]))|| 
       (rhdhv_pl_gebied(NG, (mulv)(TXB_PL[fc]), (mulv)(TXD_PL[fc])) && EG[fc]))
    {
      if (TXA_PL[fc]!= PRM[ptxa1])
      {
        TXA[PL][fc] = PRM[ptxa1];
        TXA_PL[fc] = PRM[ptxa1];
      }
      if (TXB_PL[fc]!= PRM[ptxb1])
      {
        TXB[PL][fc] = PRM[ptxb1];
        TXB_PL[fc] = PRM[ptxb1];
      }
      if (TXC_PL[fc]!= PRM[ptxc1])
      {
        TXC[PL][fc] = PRM[ptxc1];
        TXC_PL[fc] = PRM[ptxc1];
      }
      if (TXD_PL[fc]!= PRM[ptxd1])
      {
        TXD[PL][fc] = PRM[ptxd1];
        TXD_PL[fc] = PRM[ptxd1];
      }
      //RHDHV_COPY_2_TIG = TRUE;
      check_signalplans();
    }
  }
}


/*****************************************************************************/
/* Deze functie retourneert TRUE indien de actuele cyclustijd (TX_timer) zich
   tussen s (start) en e (einde) bevindt, waarbij rekening wordt gehouden met
   realisaties die over de cyclustijd heen liggen.
   Indien tx=NG dan wordt aangenomen dat de actuele TX in de cyclus moet
   worden getoetst.
*/
bool rhdhv_pl_gebied(mulv tx,        /* moment in de cyclus afgevraagd (mag NG) */
                     mulv s,         /* startpunt van het gebied                */
                     mulv e)         /* eindpunt van het gebied                 */
{
  if (PL < 0)
    return FALSE;
  if (tx == NG)
    tx = TX_timer;
  if (s < e) 
  { 
    /* realisatie past binnen de cyclustijd */
    /* [--------s==========e---------------] */
    return (bool) ( tx >= s && tx < e );
  }
  else if(s > e) 
  { 
    /* realisatie ligt over de cyclustijd heen */
    /* [=====e----------------------s======] */
    return (bool) ( tx >= s || tx < e );
  }
  else
  {
    return (bool) FALSE;
  }
}


/**********************************************************************************/
#ifdef __SYNCVAR_C__
void reset_realisation_timers(void) {
   register count fc, i;
   for (fc=0; fc<FC_MAX; fc++) {
      for (i=0; i<FC_MAX; i++) {
         R_timer[fc][i] = NG;
      }
      KR[fc] = FALSE;
   }
}
#endif

/**********************************************************************************/
void tvga_timer_halfstar(void)
{
  int i;
  
  for (i=0; i<FC_MAX; i++)
  {
    if (SG[i])                   TVGA_timer[i]  = 0;
    if (G[i] && (CG[i] > CG_WG)) TVGA_timer[i] += TE;
  }
}


/**********************************************************************************/
void PercentageMaxGroenTijdenSP(count fc, count percentage)
{
   mulv maxg;

#ifdef TX_PL_TE
   maxg = (TX_PL_max + TXD_PL[fc] - TXB_PL[fc]) % TX_PL_max;
#else
   maxg = ((TX_PL_max + TXD_PL[fc] - TXB_PL[fc]) % TX_PL_max) * 10;
#endif

   if (G[fc] && CV[fc] && !(MK[fc] & ~BIT5)) {
     if ((TFG_timer[fc]+TVGA_timer[fc])>(mulv)(((long)PRM[percentage] * (long)maxg)/100)) {
        MK[fc] &= ~BIT5;
     }
   }
}


#ifndef AUTOMAAT
/**********************************************************************************/
/* Functie rhdhv_dump_overslag_fc() dumpt op het moment dat een (hoofd)richting 
   geen groen heeft op zijn TXB moment een melding in de UBER_FILE */
bool rhdhv_txboverslag(count fc       ,  /* (hoofd)richting */
                       bool  condition)  /* voorwaarde      */
{
  if (!condition)
  {
    return (bool)(FALSE);
  }
  
  if (!waitterm(1000))
  {
    char tekst[121];
                                                                                              
    if (!G[fc]                        && /* niet groen                                        */
        !SG[fc]                       && /* niet op start groen                               */
        (TX_timer == (TXB[PL][fc]+2)) && /* 2 seconde na txb moment                           */
        (APL==PL)                     && /* niet tijdens programma omschakeling               */
        TS                            && /* alleen op de hele seconde                         */
        (TXA_PL[fc]> 0)               && /* als de fc een txa moment heeft                    */
        (TXA_PL[fc]!= TXB[PL][fc])    && /* en het txa moment ongelijk is aan txb moment      */
        !BL[fc]                       && /* richting wordt niet geblokkeerd                   */
        A[fc]                         )  /* als de richting een aanvraag (aanvraag_txb) heeft */
    {
      /* tijdstempel: */
      sprintf(tekst,"Overslag TXB fc%s PL=%1d Tx=%3d \n", FC_code[fc], PL+1, TX_timer);
      uber_puts(tekst);
      
      return (bool)(TRUE);                /* bericht verzonden */
    }
  }
  else
  {
    return (bool)(FALSE);               /* bericht niet verzonden */
  }
  
  return (bool)(FALSE);               /* bericht niet verzonden */
}

/**********************************************************************************/
/* printen van de TIG tabel */
#ifdef PRINTTIG
void rhdhv_print_tig(void)
{
  int fc, fc1;
  int fc_old = 1;
  
  for (fc=0; fc<FCMAX; fc++)
  {
    /* printen Y-as */
    xyprintf(0,1+fc+fc,"%s", FC_code[fc]);
    
    /* printen X-as */
    if (fc==0) /* eerste fc */
      xyprintf(4,0,"%s", FC_code[fc]);
    else
      xyprintf(5 + fc + fc + fc + fc_old,0,"%s", FC_code[fc]);
    fc_old = fc;
    
    /* printen TIG tabel */
    for (fc1=0; fc1<FCMAX; fc1++)
    {
      if (fc==fc1)
        xyprintf(3 + fc1 + fc1 + fc1 + fc1, 1 + fc + fc," X ", TIG_max[fc][fc1]);
      else
      if (TIG_max[fc][fc1] == NG)
        xyprintf(3 + fc1 + fc1 + fc1 + fc1, 1 + fc + fc," . ", TIG_max[fc][fc1]);
      else
      if (TIG_max[fc][fc1] == FK)
        xyprintf(3 + fc1 + fc1 + fc1 + fc1, 1 + fc + fc," FK", TIG_max[fc][fc1]);
      else
        xyprintf(3 + fc1 + fc1 + fc1 + fc1, 1 + fc + fc,"%3d", TIG_max[fc][fc1]);
    } 
  }
}
#endif
#endif
