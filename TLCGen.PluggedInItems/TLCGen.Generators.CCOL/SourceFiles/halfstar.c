#include "halfstar.h"
#include "halfstar_ov.h"     /* declaratie functies                                      */

#if !defined AUTOMAAT || defined VISSIM
   #include "xyprintf.h"/* voor debug infowindow                                          */
   #include <stdio.h>      /* declaration printf()       */
#endif

bool COPY_2_TIG = FALSE;  /* nieuwe TX-tijden copieren naar TIG tabel             */
char HalfstarOmschakelenToegestaan = 0;

void SetPlanTijden(count fc, mulv plan, mulv ta, mulv tb, mulv tc, mulv td, mulv te)
{
    TXA[plan][fc] = ta;
    TXB[plan][fc] = tb;
    TXC[plan][fc] = tc;
    TXD[plan][fc] = td;
    TXE[plan][fc] = te;
}

/*****************************************************************************/
void reset_fc_halfstar(void)
{
	register count i;
	for (i = 0; i<FC_MAX; i++)
	{
		YS[i] = YW[i] = YV[i] = YM[i] /*= YL[i]*/ = FALSE;
		BR[i] = PP[i] = RR[i] = BL[i] = X[i] = RS[i] = RW[i] = FW[i] = FM[i] = Z[i] /*= MK[i]*/ = FALSE;
	}
}

/**********************************************************************************/
void gelijkstart_va_arg_halfstar(count h_x,
	count h_rr,
	bool  overslag,
	...)
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
			if ((X[fc] && (R[fc] || GL[fc])) ||
				RR[fc] ||
				(K[fc] && A[fc]) ||
				TRG[fc] || GL[fc] || SGL[fc] ||
				(PR[fc] || AR[fc]) && RV[fc] && A[fc] ||
				SRA[fc])  IH[h_x] = TRUE;

			if ((h_rr != NG) && overslag)
			{
				if (G[fc] ||
					GL[fc])  IH[h_rr] |= TRUE;
			}
		}
	} while (fc != END);
	va_end(argp);
}


/*****************************************************************************/
/* vasthouden groen OV bij signaalplan */
void yv_ov_pl_halfstar(count fc, bool bit, bool condition)
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
bool tussen_txa_en_txb(count fc)
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

bool tussen_txb_en_txc(count fc)
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

bool tussen_txb_en_txd(count fc)
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
void altcor_kop_halfstar(count fc_aan, count fc_af, count t_nl)
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
    if ((!yv_ar_max_halfstar(fc_af, (mulv)(TFG_max[fc_aan] + T_max[t_nl] + 9))))
      PAR[fc_aan] = FALSE;
  }
  /* koppeling garanderen */
  if (TIMER_ACTIVE(t_nl))
    RR[fc_af] &= ~RR_ALTCOR_HALFSTAR;
}


/*****************************************************************************/
void wachtstand_halfstar(count fc, bool condition_a, bool condition_ws)
{
  count j, k;
  
  /* preset */
  if (IH[h_plact])
  {
    YW[fc] &= ~YW_WS_HALFSTAR;
    WS[fc] = FALSE;
  }
  /* dezelfde functie als aanvraag_wachtstand_exp() van stdfunc.c, maar nu wordt ook gekeken
     naar fictieve conflicten en wordt bit A_WS_HALFSTAR opgezet ipv BIT2 */
  if (condition_a && !A[fc] && RV[fc] && !K[fc] && !TRG[fc] && !RR[fc] && !BL[fc]) 
  {
    for (j=0; j<FKFC_MAX[fc]; j++) 
    {
      k = TO_pointer[fc][j];
      
      /* resetten wachtstand aanvraag bij (fictief) conflicterende aanvraag */
      if (A[k])
      {
        A[fc] &= ~A_WS_HALFSTAR;
        if (!A[fc] && RA[fc])
        {
          RR[fc] |= TRUE;
          TFB_timer[fc] = 0;
        }
      }
      if (A[k] && !BL[k] || TRG[k] || SG[k] || GL[k])  
        return;
    }
    A[fc] |= A_WS_HALFSTAR;
  }

  /* wachtstand opzetten */
  WS[fc] = (bool)(WG[fc] && yws_groen_fk(fc) && condition_ws);
  BIT_ACTIVE(YW[fc], (bool)(WS[fc]), YW_WS_HALFSTAR);
  
}

/**********************************************************************************/
bool yws_groen_fk(count i)
{
	count n, k;

	for (n = 0; n<FKFC_MAX[i]; n++)
	{
		k = TO_pointer[i][n];
		if (A[k])  return (FALSE);
	}
	return (TRUE);
}


/* als een richting aan het meeverlengen is en het maximum groen nog niet is gehaald en weer verkeer op de lussen is,
   de richting terugsturen naar verlenggroen (via WG) */
void Verlengroen_na_Meeverlenggroen_PL(count fc, count prmvgmg)
{
  if (MG[fc] && !FM[fc] && !Z[fc] && PR[fc] && MK[fc] && tussen_txb_en_txd(fc) && 
      (TOTXD_PL[fc] >= PRM[prmvgmg]) && !kcv(fc))
  {
    RW[fc] |= RW_VGNAMG_HALFSTAR;
  }
}


/**********************************************************************************/
void set_ym_pl_halfstar(count fc, bool condition)
{
  if (ym_max_to(fc, NG)      &&   /* meeverlengen kan volgens ontruimingstijden      */
      ym_max_tig(fc, NG)     &&   /* meeverlengen kan volgens intergroentijdentabel  */
	  ym_max_halfstar(fc, 10)&&   /* meeverlengen kan volgens signaalplan            */
      hf_wsg()               &&   /* minimaal 1 richting actief                      */
      condition             )
  {
    YM[fc] |= YM_HALFSTAR;
  }
}


/**********************************************************************************/
/* corrigeert het meeverlengen van de aanvoerende richting van een hard gekoppelde interne richting */
void mgcor_halfstar(count fcaan, count fcnal, count t_nal)
{
  if (!ym_max_halfstar(fcnal, (mulv)(T_max[t_nal] + 9))) /* +9 vanwege afronding m.g.b. 10/sec ORT's */
    YM[fcaan] &= ~YM_HALFSTAR;
} 


/**********************************************************************************/
/* correcties voor parallell langzaamverkeer en autoverkeer in deelconflict tijdens PL */
void mgcor_halfstar_deelc(count fc1, count fc2)
{
  if (RA[fc1])
    YM[fc2] &= ~YM_HALFSTAR;
}


/**********************************************************************************/
void naloopEG_CV_halfstar(bool period, count fc1, count fc2, count prmxnl, count tnldet, count tnl)
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

  if (period && (G[fc1] || GL[fc1] || TIMER_ACTIVE(tnl) || ((tnldet != NG) && TIMER_ACTIVE(tnldet))))
  {
    if (CG[fc1] < CG_MG)
    {
      if ((YV_PL[fc2] && PR[fc2]) || (yv_ar_max_halfstar(fc2, (mulv)(T_max[tnl] + 9) && AR[fc2])))
      {
        YV[fc2] |= YV_KOP_HALFSTAR;
      }
    }
    
    /* naloop in verlenggroen houden, zodat naloop nog kan verlengen na koppeling. Maar als de 
       aanvoer in MG komt, dan naloop ook in MG anders kan de correctie van de koppeling
       op het meeverlengen niet worden uitgevoerd. Na einde MG van aanvoer wordt naloop weer
       VG zodat na de koppeling weer kan worden verlengd */
    RW[fc2] |= SG[fc1] ? RW_KOP_HALFSTAR : 0;
    
    YM[fc2] |= YM_KOP_HALFSTAR;
  }
  
  /* meerealisatie nalooprichting */
  set_special_MR(fc2, fc1, (bool)(period && ((A[fc1] &&  R[fc1]) || CV[fc1])));
  
  if(prmxnl != NG)
  {
    /* aanvoerende richting niet te snel realiseren */
    if (period && x_aanvoer(fc2, PRM[prmxnl]) && (TX_timer != TXB[PL][fc1]))
      X[fc1] |= X_VOOR_HALFSTAR;

    /* tegenhouden aanvoerende richting rekening houden met geel en garantieroodtijd; x_aanvoer doet dit niet! */
    if (period && (GL[fc2] || TRG[fc2] && (TX_timer != TXB[PL][fc1])))
    {
      if (((TGL_max[fc2] + TRG_max[fc2]) - (geeltimer[fc1][fc2] + groodtimer[fc1][fc2])) > PRM[prmxnl])
        X[fc1] |= X_VOOR_HALFSTAR;
    }
  }
  
  /* als nalooprichting worden tegengehouden, dan ook aanvoerende richting tegenhouden */
  if (period && ((RR[fc2]) && !(TX_timer == TXB[PL][fc1])))
  {
    RR[fc1] |= RR_KOP_HALFSTAR;
  }
  
  /* koppeling garanderen */
  if (period && (TIMER_ACTIVE(tnl) || ((tnldet != NG) && TIMER_ACTIVE(tnldet))))
    RR[fc2] &= ~RR_ALTCOR_HALFSTAR;
  
}


/**********************************************************************************/
void zachtekoppeling_halfstar(bool period, count fc1, count fc2, count tvs, count tnldet, count tnl)
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

  if (period && ((G[fc1] && !WS[fc1]) || GL[fc1] || TIMER_ACTIVE(tnl) || ((tnldet != NG) && TIMER_ACTIVE(tnldet))))
  {
    if ((CG[fc1] < CG_MG))
    {
      if ((YV_PL[fc2] && PR[fc2]) || (yv_ar_max_halfstar(fc2, (mulv)(T_max[tnl] + 9) && AR[fc2])))
      {
        YV[fc2] |= YV_KOP_HALFSTAR;
        RW[fc2] |= SG[fc1] ? RW_KOP_HALFSTAR : 0;
      }
    }
  }
  
  /* ten behoeve van verklikken */
  if (tvs != NG)
    RT[tvs] = SG[fc1];

}


/*****************************************************************************/
/* PP bitje opzetten en cyclische aanvraag op TXB moment als PP waar is      */
void set_pp_halfstar(count fc, bool condition, count value)
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
void var_txc(count fc, bool condition)
{
  YW[fc] &= ~YW_VAR_TXC;
  RW[fc] &= ~RW_VAR_TXC;
  
  YW[fc] |= condition && tussen_txb_en_txc(fc) ? YW_VAR_TXC : 0;
  RW[fc] |= condition && tussen_txb_en_txc(fc) ? RW_VAR_TXC : 0;
  
}

/**********************************************************************************/
/* aangepaste functie van ccol bibliotheek (plfunc.c). Naast een aanvraag wordt ook gekeken naar
   PP van een conflicterende richting. De aanvraag van een hoofdrichting wordt pas op TXB moment gezet */
bool ym_max_halfstar(count i, mulv koppeltijd)
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
bool yv_ar_max_halfstar(count i, mulv koppeltijd)
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
void alternatief_halfstar(count fc, mulv altp, bool condition)
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
void altcor_parftsvtg_pl_halfstar(count fc1, count fc2, bool voorwaarde)
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
      RR[fc2] |= RR_ALTCOR_HALFSTAR;
    if (kcv(fc2) && A[fc2] && !RA[fc2])
      RR[fc1] |= RR_ALTCOR_HALFSTAR;
    */
  }  
}

/*****************************************************************************/
/* retour rood wanneer richting AR heeft maar geen PAR meer */
void reset_altreal_halfstar(void)
{
	count i;

	for (i = 0; i<FCMAX; i++)
		RR[i] |= R[i] && AR[i] && (!PAR[i] || ERA[i]) ? RR_ALTCOR_HALFSTAR : 0;
}


/*****************************************************************************/
void altcor_naloopSG_halfstar(count fc1, count fc2, bool a_bui_fc1, count tnlsg12, bool voorwaarde)
{
  if (voorwaarde)
  {
    if (a_bui_fc1)
    {
      if (!(PAR[fc2] && (tar_max_ple(fc2) > (T_max[tnlsg12] + 11))))
        PAR[fc1] = FALSE;
    }
    
    if (a_bui_fc1 && !PAR[fc1])
      PAR[fc2] = FALSE;
  }
}


/*****************************************************************************/
/* alternatieve correcties voor tegengestelde richtingen */
void altcor_parfts_pl_halfstar(count fc1, count fc2, bool voorwaarde)
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
void naloopSG_halfstar(count fc1             , /* fc1 */
                       count fc2             , /* fc2 */
                       bool  a_bui_fc1       , /* buitendrukknopaanvraag fc1 */
                       count tkopfc1fc2)       /* naloop (SG) fc1 -> fc2 */
{
  /* nalopen */
  RT[tkopfc1fc2] = (bool) (RA[fc1] && a_bui_fc1);
  AT[tkopfc1fc2] = (bool)((ERA[fc1] && SRV[fc1]));
  if ((RT[tkopfc1fc2] || T[tkopfc1fc2]) && ((YV_PL[fc2] && PR[fc2]) || 
       (yv_ar_max_halfstar(fc2, (mulv)(T_max[tkopfc1fc2] - TFG_max[fc2])) && AR[fc2])))
  {
    YV[fc2] |= YV_KOP_HALFSTAR;
  }
  
  /* meerealisaties */
  set_special_MR(fc2, fc1, (bool)(a_bui_fc1 && R[fc1] && (A[fc1] != A_WS_HALFSTAR)));
}


/**********************************************************************************/
/* Deze functie is afgeleid van de ccol functie "set_MRLW()".
*/
void set_special_MR(count i, count j, bool condition)
{
	if (AA[j] && condition && !AA[i] && !BL[i] && !fkaa(i) && !RR[j] && !BL[j])
	{
		PR[i] = AR[i] = BR[i] = MR[i] = FALSE;
		AA[i] = MR[i] = TRUE;              /* set actuation                */
		A[i] |= A_MR_HALFSTAR;               /* set demand                 */
		if (PR[j])  PR[i] = PR[j];       /* set primary realization      */
		if (AR[j])  AR[i] = AR[j];       /* set alternative realization  */
		if (BR[j])  BR[i] = BR[j];       /* set priority realization     */
	}
}


/**********************************************************************************/
void getrapte_fietser_halfstar(count fc1             , /* fc1 */
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
       (yv_ar_max_halfstar(fc2, (mulv)(T_max[tkopfc1fc2] - TFG_max[fc2])) && AR[fc2])*/))
  {
    YV[fc2] |= YV_KOP_HALFSTAR;
  }
  
  RT[tkopfc2fc1] = (bool)((RA[fc2] || TFG[fc2]) && a_bui_fc2);
  AT[tkopfc2fc1] = (bool)((ERA[fc2] && SRV[fc2]));
  if ((RT[tkopfc2fc1] || T[tkopfc2fc1]) && ((YV_PL[fc1] && PR[fc1]) /*|| 
       (yv_ar_max_halfstar(fc1, (mulv)(T_max[tkopfc2fc1] - TFG_max[fc1])) && AR[fc1])*/))
  {
    YV[fc1] |= YV_KOP_HALFSTAR;
  }
    
  /* meerealisaties */
  set_special_MR(fc1, fc2, (bool)(PR[fc2] && a_bui_fc2 && R[fc2] && (A[fc2] != A_WS_HALFSTAR)));
  set_special_MR(fc2, fc1, (bool)(PR[fc1] && a_bui_fc1 && R[fc1] && (A[fc1] != A_WS_HALFSTAR)));
  
  /* aanvoerende richting niet te snel realiseren (maximale voorstarttijd gelijk aan nalooptijd) */
  /*if (voorstartfc1fc2 != NG)
  {
    if ((voorstartfc1fc2 > 0) && a_bui_fc1)
    {
      if (x_aanvoer(fc2, (mulv)(voorstartfc1fc2)))
      {
        X[fc1] |= X_VOOR_HALFSTAR;
      }
    }
  }
  if (voorstartfc2fc1 != NG)
  {
    if ((voorstartfc2fc1 > 0) && a_bui_fc2)
    {
      if (x_aanvoer(fc1, (mulv)(voorstartfc2fc1)))
      {
        X[fc2] |= X_VOOR_HALFSTAR;
      }
    }
  }*/
}


/**********************************************************************************/
/* 2e REALISATIE BINNEN SIGNAALPLAN */
void tweederealisatie_halfstar(count fc_1,  /* fasecyclus 1e realisatie */
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
    if ((tot_txb2 < tot_txb1) && !tussen_txb_en_txd(fc_1) && !G[fc_1])
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
void set_yspl(count fc)
{
  if (SG[fc] && PR[fc] && (TXA_PL[fc]> 0))  
    YS_PL[fc]= TRUE;
}


/**********************************************************************************/
/* Voorstartgroen tijdens voorstart t.o.v. sg-plan */
void vs_ple(count fc, count prmtotxa, bool condition)
{
  /*RS[fc] |= YS_PL[fc] && PR[fc] || 
            ((TOTXA_PL[fc]>0) && (TOTXA_PL[fc]<PRM[prmtotxa])) ? RS_HALFSTAR : 0;*/
  RS[fc] |= condition /*&& YS_PL[fc]*/ && (TXA_PL[fc]> 0) && (tussen_txa_en_txb(fc) && PP[fc]) ? RS_HALFSTAR : 0;
}


/**********************************************************************************/
/* Retour wachtgroen bij wachtgroen richtingen */
void wg_ple(count fc, bool condition)
{
  RW[fc] |= YW[fc] || (TOTXB_PL[fc] <= (TRG_max[fc]+TGL_max[fc])) &&
            (/*(TOTXB_PL[fc]>0) ||*/ condition && !ka(fc) || (TX_timer == TXB_PL[fc])) ? RW_WG_HALFSTAR : 0;

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


/*****************************************************************************/
/* Deze functie retourneert TRUE indien de actuele cyclustijd (TX_timer) zich
   tussen s (start) en e (einde) bevindt, waarbij rekening wordt gehouden met
   realisaties die over de cyclustijd heen liggen.
   Indien tx=NG dan wordt aangenomen dat de actuele TX in de cyclus moet
   worden getoetst.
*/
bool pl_gebied(mulv tx,        /* moment in de cyclus afgevraagd (mag NG) */
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


/**********************************************************************************/
void PercentageVerlengGroenTijdenSP(count fc, count percentage)
{
	mulv maxg;

#ifdef TX_PL_TE
	maxg = (TX_PL_max + TXD_PL[fc] - TXB_PL[fc]) % TX_PL_max;
#else
	maxg = ((TX_PL_max + TXD_PL[fc] - TXB_PL[fc]) % TX_PL_max) * 10;
#endif

	if (G[fc] && CV[fc] && !(MK[fc] & ~BIT5)) {
		if ((TVGA_timer[fc])>(mulv)(((long)PRM[percentage] * (long)maxg) / 100)) {
			MK[fc] &= ~BIT5;
		}
	}
}

#ifndef AUTOMAAT
/**********************************************************************************/
/* Functie dump_overslag_fc() dumpt op het moment dat een (hoofd)richting 
   geen groen heeft op zijn TXB moment een melding in de UBER_FILE */
bool txboverslag(count fc       ,  /* (hoofd)richting */
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
void print_tig(void)
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
