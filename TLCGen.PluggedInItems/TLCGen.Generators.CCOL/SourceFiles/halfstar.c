#include "halfstar.h"
#include "halfstar_ov.h"     /* declaratie functies                                      */

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM
   #include "xyprintf.h"/* voor debug infowindow                                          */
   #include <stdio.h>      /* declaration printf()       */
#endif

#if defined CCOLTIG
bool COPY_2_TRIG = FALSE;  /* nieuwe TX-tijden copieren naar TRIG tabel             */
#else
bool COPY_2_TIG = FALSE;  /* nieuwe TX-tijden copieren naar TIG tabel             */
#endif
char HalfstarOmschakelenToegestaan = 0;

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

/**********************************************************************************/
void getrapte_fietser_halfstar(count fc1, /* fc1 */
	count fc2, /* fc2 */
	bool  a_bui_fc1, /* buitendrukknopaanvraag fc1 */
	bool  a_bui_fc2, /* buitendrukknopaanvraag fc2 */
	count tkopfc1fc2, /* naloop (SG) fc1 -> fc2 */
	count tkopfc2fc1, /* naloop (SG) fc2 -> fc1 */
	count voorstartfc1fc2, /* maximale voorstart fc1 -> fc2 (mag NG) */
	count voorstartfc2fc1) /* maximale voorstart fc2 -> fc1 (mag NG) */
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
	set_special_MR(fc2, fc1, (bool)(period && ((A[fc1] && R[fc1]) || CV[fc1])));

	if (prmxnl != NG)
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
void naloopSG_halfstar(count fc1, /* fc1 */
	count fc2, /* fc2 */
	count dk_bui_fc1, /* buitendrukknopaanvraag fc1 */
	count hd_bui_fc1, /* onthouden buitendrukknopaanvraag fc1 */
	count tkopfc1fc2)       /* naloop (SG) fc1 -> fc2 */
{
	/* nalopen */
	if (dk_bui_fc1 != NG && hd_bui_fc1 != NG)
	{
		if (SG[fc1]) IH[hd_bui_fc1] = FALSE;
		IH[hd_bui_fc1] |= D[dk_bui_fc1] && !G[fc1] && A[fc1];
		RT[tkopfc1fc2] = (bool)(RA[fc1] && IH[hd_bui_fc1]);
	}
	else
	{
		RT[tkopfc1fc2] = (bool)(RA[fc1]);
	}
	AT[tkopfc1fc2] = (bool)((ERA[fc1] && SRV[fc1]));
	if ((RT[tkopfc1fc2] || T[tkopfc1fc2]) && ((YV_PL[fc2] && PR[fc2]) ||
		(yv_ar_max_halfstar(fc2, (mulv)(T_max[tkopfc1fc2] - TFG_max[fc2])) && AR[fc2])))
	{
		YV[fc2] |= YV_KOP_HALFSTAR;
	}

	/* meerealisaties */
	if (dk_bui_fc1 != NG && hd_bui_fc1 != NG)
	{
		set_special_MR(fc2, fc1, (bool)(IH[hd_bui_fc1] && R[fc1] && (A[fc1] != A_WS_HALFSTAR)));
	}
	else
	{
		set_special_MR(fc2, fc1, (bool)(R[fc1] && (A[fc1] != A_WS_HALFSTAR)));
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
		if ((TFG_timer[fc] + TVGA_timer[fc])>(mulv)(((long)PRM[percentage] * (long)maxg) / 100)) {
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
		return (bool)(tx >= s && tx < e);
	}
	else if (s > e)
	{
		/* realisatie ligt over de cyclustijd heen */
		/* [=====e----------------------s======] */
		return (bool)(tx >= s || tx < e);
	}
	else
	{
		return (bool)FALSE;
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
void reset_fc_halfstar(void)
{
	register count i;
	for (i = 0; i<FC_MAX; i++)
	{
		YS[i] = YW[i] = YV[i] = YM[i] /*= YL[i]*/ = FALSE;
		BR[i] = PP[i] = RR[i] = BL[i] = X[i] = RS[i] = RW[i] = FW[i] = FM[i] = Z[i] /*= MK[i]*/ = FALSE;
	}
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
void set_ym_pl_halfstar(count fc, bool condition)
{
#ifdef CCOLTIG
  if (ym_max_tig(fc, NG)      &&   /* meeverlengen kan volgens ontruimingstijden      */
      ym_max_trig(fc, NG)     &&   /* meeverlengen kan volgens intergroentijdentabel  */
#else
	if (ym_max_to(fc, NG) &&   /* meeverlengen kan volgens ontruimingstijden      */
		ym_max_tig(fc, NG) &&   /* meeverlengen kan volgens intergroentijdentabel  */
#endif
	  ym_max_halfstar(fc, 10)&&   /* meeverlengen kan volgens signaalplan            */
      hf_wsg()               &&   /* minimaal 1 richting actief                      */
      condition             )
  {
    YM[fc] |= YM_HALFSTAR;
  }
}

/**********************************************************************************/
/* YS_PL[] zelf opzetten bij primair startgroen, dit gebeurt nl.
   niet als een fasecyclus voor zijn TXA-moment primair groen krijgt */
void set_yspl(count fc)
{
	if (SG[fc] && PR[fc] && (TXA_PL[fc]> 0))
		YS_PL[fc] = TRUE;
}

/* dubbele realisatie per richting per plan. Let op, er wordt gebruik gemaakt van baseparameters:
   voor de eerste realisatie de eerste TXA parameter en voor de tweede realisatie de tweede TXA
   parameter meegeven */
void set_2real(count fc, count prm_eerste_txa, count prm_tweede_txa, mulv pl, bool condition)
{
	if (prm_tweede_txa != NG && 
		PRM[prm_tweede_txa + 1] != 0 && PRM[prm_tweede_txa + 3] != 0)
	{
		set_tx_change(fc, pl, (prm_eerste_txa + 0),
			(prm_eerste_txa + 1),
			(prm_eerste_txa + 2),
			(prm_eerste_txa + 3),
			(prm_eerste_txa + 4),
			(prm_tweede_txa + 0),
			(prm_tweede_txa + 1),
			(prm_tweede_txa + 2),
			(prm_tweede_txa + 3),
			(prm_tweede_txa + 4),
			condition);
	}
	else
	{
		set_tx_change(fc, pl, (prm_eerste_txa + 0),
			(prm_eerste_txa + 1),
			(prm_eerste_txa + 2),
			(prm_eerste_txa + 3),
			(prm_eerste_txa + 4),
			(NG),
			(NG),
			(NG),
			(NG),
			(NG),
			condition);
	}
}

void SetPlanTijden(count fc, mulv plan, mulv ta, mulv tb, mulv tc, mulv td, mulv te)
{
    TXA[plan][fc] = ta;
    TXB[plan][fc] = tb;
    TXC[plan][fc] = tc;
    TXD[plan][fc] = td;
    TXE[plan][fc] = te;
}

/*****************************************************************************/
void SetPlanTijden2R(count fc, mulv plan, mulv ta, mulv tb, mulv tc, mulv td, mulv te,
	count fc_2, mulv ta_2, mulv tb_2, mulv tc_2, mulv td_2, mulv te_2)
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
void sync_pg(void)
{
	register count fc;
	bool **prml = NULL;
	count ml = -1;
	count mlx = -1;
	count mlmax = 0;

	for (fc = 0; fc<FCMAX; fc++)
	{
#ifdef MLMAX
#else
#ifdef MLAMAX
		if (mlx == -1) for (ml = 0; ml < MLAMAX; ++ml) if (PRMLA[ml][fc]) { mlx = MLA; prml = PRMLA; mlmax = MLAMAX; break; }
#endif
#ifdef MLBMAX
		if (mlx == -1) for (ml = 0; ml < MLBMAX; ++ml) if (PRMLB[ml][fc]) { mlx = MLB; prml = PRMLB; mlmax = MLBMAX; break; }
#endif
#ifdef MLCMAX
		if (mlx == -1) for (ml = 0; ml < MLCMAX; ++ml) if (PRMLC[ml][fc]) { mlx = MLC; prml = PRMLC; mlmax = MLCMAX; break; }
#endif
#ifdef MLDMAX
		if (mlx == -1) for (ml = 0; ml < MLDMAX; ++ml) if (PRMLD[ml][fc]) { mlx = MLD; prml = PRMLD; mlmax = MLDMAX; break; }
#endif
#ifdef MLEMAX
		if (mlx == -1) for (ml = 0; ml < MLEMAX; ++ml) if (PRMLE[ml][fc]) { mlx = MLE; prml = PRMLE; mlmax = MLEMAX; break; }
#endif
#endif
		if (PG[fc] && !prml[ml][fc] && !prml[(ml + 1 == mlmax ? ML1 : ml + 1)][fc])
		{
			PG[fc] = FALSE;
		}
		if (IH[hmlact] && RA[fc] && !AAPR[fc] && !PAR[fc])
			RR[fc] |= BIT9;
	}
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

/**********************************************************************************/
void set_tx_change(count fc, /* signaalgroep         */
	count pl, /* actieve plan         */
	count ptxa1, /* eerste a realisatie    */
	count ptxb1, /* eerste b realisatie    */
	count ptxc1, /* eerste c realisatie    */
	count ptxd1, /* eerste d realisatie    */
	count ptxe1, /* eerste e realisatie    */
	count ptxa2, /* tweede a realisatie    */
	count ptxb2, /* tweede b realisatie    */
	count ptxc2, /* tweede c realisatie    */
	count ptxd2, /* tweede d realisatie    */
	count ptxe2, /* tweede e realisatie    */
	bool  condition) /* conditie             */
{
	/* als geen wissel toegepast mag worden of parameters zijn 0 */
	if ((PRM[ptxb1] == 0) && (PRM[ptxb1] == 0) && (PRM[ptxb2] == 0) && (PRM[ptxb2] == 0))
	{
		return;
	}

	/* als geen tweede realisatie mag worden uitgevoerd, dan eerste realisatie instellen */
	if (!condition || (PRM[ptxb2] == 0) || (ptxb2 == NG) || (pl != PL))
	{
		bool copy = FALSE;
		if (TXA[pl][fc] != PRM[ptxa1])
		{
			TXA[pl][fc] = PRM[ptxa1];
			copy = TRUE;
		}

		if (TXB[pl][fc] != PRM[ptxb1])
		{
			TXB[pl][fc] = PRM[ptxb1];
			copy = TRUE;
		}
		if (TXC[pl][fc] != PRM[ptxc1])
		{
			TXC[pl][fc] = PRM[ptxc1];
			copy = TRUE;
		}
		if (TXD[pl][fc] != PRM[ptxd1])
		{
			TXD[pl][fc] = PRM[ptxd1];
			copy = TRUE;
		}
		if (TXE[pl][fc] != PRM[ptxe1])
		{
			TXE[pl][fc] = PRM[ptxe1];
			copy = TRUE;
		}
		if (copy)
		{
#if defined CCOLTIG
			COPY_2_TRIG = TRUE;
#else
			COPY_2_TIG = TRUE;
#endif
		}
		return;
	}

	if (PL == pl)
	{
		if (pl_gebied(NG, (mulv)(PRM[ptxd1] + 1), (mulv)(PRM[ptxd2])) ||
			(pl_gebied(NG, (mulv)(TXB_PL[fc]), (mulv)(TXD_PL[fc])) && EG[fc]))
		{
			if (TXA_PL[fc] != PRM[ptxa2])
			{
				TXA[PL][fc] = PRM[ptxa2];
				TXA_PL[fc] = PRM[ptxa2];
			}
			if (TXB_PL[fc] != PRM[ptxb2])
			{
				TXB[PL][fc] = PRM[ptxb2];
				TXB_PL[fc] = PRM[ptxb2];
			}
			if (TXC_PL[fc] != PRM[ptxc2])
			{
				TXC[PL][fc] = PRM[ptxc2];
				TXC_PL[fc] = PRM[ptxc2];
			}
			if (TXD_PL[fc] != PRM[ptxd2])
			{
				TXD[PL][fc] = PRM[ptxd2];
				TXD_PL[fc] = PRM[ptxd2];
			}
			check_signalplans();
		}

		if (pl_gebied(NG, (mulv)(PRM[ptxd2] + 1), (mulv)(PRM[ptxd1])) ||
			(pl_gebied(NG, (mulv)(TXB_PL[fc]), (mulv)(TXD_PL[fc])) && EG[fc]))
		{
			if (TXA_PL[fc] != PRM[ptxa1])
			{
				TXA[PL][fc] = PRM[ptxa1];
				TXA_PL[fc] = PRM[ptxa1];
			}
			if (TXB_PL[fc] != PRM[ptxb1])
			{
				TXB[PL][fc] = PRM[ptxb1];
				TXB_PL[fc] = PRM[ptxb1];
			}
			if (TXC_PL[fc] != PRM[ptxc1])
			{
				TXC[PL][fc] = PRM[ptxc1];
				TXC_PL[fc] = PRM[ptxc1];
			}
			if (TXD_PL[fc] != PRM[ptxd1])
			{
				TXD[PL][fc] = PRM[ptxd1];
				TXD_PL[fc] = PRM[ptxd1];
			}
			check_signalplans();
		}
		if ((PRM[ptxe1] > 0) && (PRM[ptxe2] > 0))
		{
			if (TX_timer == PRM[ptxe1])
			{
				TXE[PL][fc] = PRM[ptxe2];
				TXE_PL[fc] = PRM[ptxe2];
			}
			if (TX_timer == PRM[ptxe2])
			{
				TXE[PL][fc] = PRM[ptxe1];
				TXE_PL[fc] = PRM[ptxe1];
			}
		}
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
#ifdef CCOLTIG
			k = KF_pointer[i][n];
#else
			k = TO_pointer[i][n];
#endif
#if defined CCOLTIG && !defined NO_TIGMAX
			if (TIG[k][i])           /* zoek grootste ontruimingstijd   */
            {
				to_tmp = TIG_max[k][i] - TIG_timer[k];
#else
			if (TO[k][i])           /* zoek grootste ontruimingstijd   */
			{
				to_tmp = TGL_max[k] + TO_max[k][i] - TGL_timer[k] - TO_timer[k];
#endif
                if (to_tmp>to_max)
                    to_max= to_tmp;
            }
        }
#else
        for (n=0; n<FKFC_MAX[i]; n++)
        {
#ifdef CCOLTIG
			k = KF_pointer[i][n];
            if (TRIG_max[k][i]>=0)
            {
                to_tmp= TRIG_max[k][i]-TRIG_timer[k];
#else
			k= TO_pointer[i][n];
			if (TIG_max[k][i] >= 0)
			{
				to_tmp = TIG_max[k][i] - TIG_timer[k];
#endif
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
void tvga_timer_halfstar(void)
{
	int i;

	for (i = 0; i<FC_MAX; i++)
	{
		if (SG[i])                   TVGA_timer[i] = 0;
		if (G[i] && (CG[i] > CG_WG)) TVGA_timer[i] += TE;
	}
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
		if (TX_timer < TXB[PL][fc_1])     tot_txb1 = TXB[PL][fc_1] - TX_timer;
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
		if (TX_timer < TXB[PL][fc_2])     tot_txb2 = TXB[PL][fc_2] - TX_timer;
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

		/* waarden 2e realisatie kopi?ren naar 1e realisatie */
		TXA[PL][fc_1] = TXA[PL][fc_2];
		TXB[PL][fc_1] = TXB[PL][fc_2];
		TXC[PL][fc_1] = TXC[PL][fc_2];
		TXD[PL][fc_1] = TXD[PL][fc_2];
		TXE[PL][fc_1] = TXE[PL][fc_2];

		/* oude waarden 1e realisatie kopi?ren naar 2e realisatie */
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
/* aangepaste functie van ccol bibliotheek (plfunc.c). Naast een aanvraag wordt ook gekeken naar
PP van een conflicterende richting. De aanvraag van een hoofdrichting wordt pas op TXB moment gezet */
bool ym_max_halfstar(count i, mulv koppeltijd)
{
	register count n, k;
	bool ym;
	if (MG[i] && (YM_PL[i] || !PR[i]))
	{
		ym = TRUE;

		for (n = 0; n<FKFC_MAX[i]; n++)
		{
#ifdef CCOLTIG
			k = KF_pointer[i][n];
			if (TRIG_max[i][k] >= 0)
#else
			k = TO_pointer[i][n];
			if (TIG_max[i][k] >= 0)
#endif
			{
				if ((TOTXB_PL[k] > 0) && !PG[k] && R[k] && (A[k] || PP[k]) || ((TOTXB_PL[k] == 0) && RA[k]))
				{
#ifdef CCOLTIG
					if ((TRIG_max[i][k] + koppeltijd) >= (TOTXB_PL[k] - 9))  /* -9 om rekening te houden met ORT's in tienden van seconden          */
#else
					if ((TIG_max[i][k] + koppeltijd) >= (TOTXB_PL[k] - 9))  /* -9 om rekening te houden met ORT's in tienden van seconden          */
#endif
					{                                                       /* (refresh van TOTXB_PL is maar per seconde en niet per 1/10 seconde) */
						ym = FALSE;
						break;
					}
				}
			}
		}
	}
	else
	{
		ym = CV[i];
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
#ifdef CCOLTIG
      k = KF_pointer[i][n];
      if (TRIG_max[i][k] >= 0)
#else
      k = TO_pointer[i][n];
      if (TIG_max[i][k] >= 0)
#endif
      {
        if (TOTXB_PL[k]>0 && !PG[k] && R[k] && (A[k] || PP[k]) || TOTXB_PL[k]==0 && RA[k]) 
        {
#ifdef CCOLTIG
			if ((TRIG_max[i][k] + koppeltijd) >= (TOTXB_PL[k]))
#else
			if ((TIG_max[i][k] + koppeltijd) >= (TOTXB_PL[k]))
#endif
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

/**********************************************************************************/
bool yws_groen_fk(count i)
{
	count n, k;

	for (n = 0; n<FKFC_MAX[i]; n++)
	{
#ifdef CCOLTIG
		k = KF_pointer[i][n];
#else
		k = TO_pointer[i][n];
#endif
		if (A[k])  return (FALSE);
	}
	return (TRUE);
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
/* Voorstartgroen tijdens voorstart t.o.v. sg-plan */
void vs_ple(count fc, count prmtotxa, bool condition)
{
	/*RS[fc] |= YS_PL[fc] && PR[fc] ||
	((TOTXA_PL[fc]>0) && (TOTXA_PL[fc]<PRM[prmtotxa])) ? RS_HALFSTAR : 0;*/
	RS[fc] |= condition /*&& YS_PL[fc]*/ && (TXA_PL[fc]> 0) && (tussen_txa_en_txb(fc) && PP[fc]) ? RS_HALFSTAR : 0;
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

/*****************************************************************************/
void wachtstand_halfstar(count fc, bool condition_hs, bool condition_a, bool condition_ws)
{
  count j, k;
  
  /* preset */
  if (condition_hs)
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
#ifdef CCOLTIG
		k = KF_pointer[fc][j];
#else
		k = TO_pointer[fc][j];
#endif
      
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
/* Retour wachtgroen bij wachtgroen richtingen */
void wg_ple(count fc, bool condition)
{
	RW[fc] |= YW[fc] || (TOTXB_PL[fc] <= (TRG_max[fc] + TGL_max[fc])) &&
		(/*(TOTXB_PL[fc]>0) ||*/ condition && !ka(fc) || (TX_timer == TXB_PL[fc])) ? RW_WG_HALFSTAR : 0;

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

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)
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
#ifdef CCOLTIG
      if (fc==fc1)
        xyprintf(3 + fc1 + fc1 + fc1 + fc1, 1 + fc + fc," X ", TRIG_max[fc][fc1]);
      else
      if (TRIG_max[fc][fc1] == NG)
        xyprintf(3 + fc1 + fc1 + fc1 + fc1, 1 + fc + fc," . ", TRIG_max[fc][fc1]);
      else
      if (TRIG_max[fc][fc1] == FK)
        xyprintf(3 + fc1 + fc1 + fc1 + fc1, 1 + fc + fc," FK", TRIG_max[fc][fc1]);
      else
        xyprintf(3 + fc1 + fc1 + fc1 + fc1, 1 + fc + fc,"%3d", TRIG_max[fc][fc1]);
#else
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
#endif
    } 
  }
}
#endif
#endif

void SignalplanPrmsToTx(count pl, count txa1)
{
	short fc;
	for (fc = 0; fc < FCMAX; ++fc)
	{
		TXA[pl][fc] = PRM[txa1 + fc * 10 + 0];
		TXB[pl][fc] = PRM[txa1 + fc * 10 + 1];
		TXC[pl][fc] = PRM[txa1 + fc * 10 + 2];
		TXD[pl][fc] = PRM[txa1 + fc * 10 + 3];
		TXE[pl][fc] = PRM[txa1 + fc * 10 + 4];
	}
}

bool CheckSignalplanPrms(count pl, count ctijd, count txa1)
{
	char temp[1024];
	short i, cfc, fc, real, tg_min;
	short txa, txb, txc, txd, txe;
	short txb_fc, txb_cfc, txd_fc, txd_cfc;
	short tx[5];
	char txS[5][1] = { 'A', 'B', 'C', 'D', 'E' };
	short txmax = PRM[ctijd];

	if (txmax < 0)
	{
		sprintf(temp, "copy PRM to TX: new signalplan %d has an invalid cycletime: %d\n", pl + 1, txmax);
		uber_puts(temp);
		return TRUE;
	}

	for (fc = 0; fc < FCMAX; ++fc)
	{
		for (real = 0; real < 2; ++real)
		{
			tx[0] = txa = PRM[txa1 + fc * 10 + real * 5 + 0];
			tx[1] = txb = txb_fc = PRM[txa1 + fc * 10 + real * 5 + 1];
			tx[2] = txc = PRM[txa1 + fc * 10 + real * 5 + 2];
			tx[3] = txd = PRM[txa1 + fc * 10 + real * 5 + 3];
			tx[4] = txe = PRM[txa1 + fc * 10 + real * 5 + 4];

			// skip 2nd realisation if empty
			if (real == 1 && txb == 0 && txd == 0) continue;

			// check between 0 and txmax
			for (i = 0; i < 5; ++i)
			{
				if (tx[i] < 0 || tx[i] >= txmax)
				{
					sprintf(temp, "copy PRM to TX: TX%c for fc%s for new signalplan %d has an invalid value: %d\n", txS[i][0], FC_code[fc], pl + 1, txa);
					uber_puts(temp);
					return TRUE;
				}
			}

			// check a, c and e
			if (txb < txd)
			{
				tg_min = (txd - txb) * 10;
				if (txa > 0 && txa > txb && txa < txd)
				{
					sprintf(temp, "copy PRM to TX: TXA for fc%s for new signalplan %d is between TXB and TXD\n", FC_code[fc], pl + 1);
					uber_puts(temp);
					return TRUE;
				}
				if (txc > 0 && (txc < txb || txc > txd))
				{
					sprintf(temp, "copy PRM to TX: TXC for fc%s for new signalplan %d is not between TXB and TXD\n", FC_code[fc], pl + 1);
					uber_puts(temp);
					return TRUE;
				}
				if (txe > 0 && txe > txb && txe < txd)
				{
					sprintf(temp, "copy PRM to TX: TXE for fc%s for new signalplan %d is between TXB and TXD\n", FC_code[fc], pl + 1);
					uber_puts(temp);
					return TRUE;
				}
			}
			else if (!(txb == 0 && txd == 0))
			{
				tg_min = (txmax - txb + txd) * 10;
				if (txa > 0 && (txa > txb || txa < txd))
				{
					sprintf(temp, "copy PRM to TX: TXA for fc%s for new signalplan %d is between TXB and TXD\n", FC_code[fc], pl + 1);
					uber_puts(temp);
					return TRUE;
				}
				if (txc > 0 && txc < txb && txc > txd)
				{
					sprintf(temp, "copy PRM to TX: TXC for fc%s for new signalplan %d is not between TXB and TXD\n", FC_code[fc], pl + 1);
					uber_puts(temp);
					return TRUE;
				}
				if (txe > 0 && (txe > txb || txe < txd))
				{
					sprintf(temp, "copy PRM to TX: TXE for fc%s for new signalplan %d is between TXB and TXD\n", FC_code[fc], pl + 1);
					uber_puts(temp);
					return TRUE;
				}
			}

			// check at least tggmax/tfgmax
			if (tg_min < TFG_max[fc])
			{
				sprintf(temp, "copy PRM to TX: fixed green time for fc%s not met in new signalplan %d\n", FC_code[fc], pl + 1);
				uber_puts(temp);
				return TRUE;
			}
			if (tg_min < TGG_max[fc])
			{
				sprintf(temp, "copy PRM to TX: minimum green time for fc%s not met in new signalplan %d\n", FC_code[fc], pl + 1);
				uber_puts(temp);
				return TRUE;
			}

			// check conflicts
			for (cfc = fc + 1; cfc < FCMAX; ++cfc)
			{
				// check if conflicting
#if defined CCOLTIG && !defined NO_TIGMAX
#ifndef NO_GGCONFLICT
				if (TIG_max[fc][cfc] >= 0 || TIG_max[fc][cfc] <= GK)
#else
				if (TIG_max[fc][cfc] >= 0)
#endif
#else
#ifndef NO_GGCONFLICT
				if (TO_max[fc][cfc] >= 0 || TO_max[fc][cfc] <= GK)
#else
				if (TO_max[fc][cfc] >= 0)
#endif
#endif
				{
					bool conflict_txb_txd = FALSE;

					// calculate TXB and TXD including amber (or not) and clearing/intergreen times
					txb_cfc = PRM[txa1 + cfc * 10 + real * 5 + 1];
#if defined CCOLTIG && !defined NO_TIGMAX
					if (TIG_max[fc][cfc] >= 0)
					{
						txd_fc = txd + ((TIG_max[fc][cfc] + 9) / 10);
						if (txd_fc > txmax)  txd_fc -= txmax;
						txd_cfc = PRM[txa1 + cfc * 10 + real * 5 + 3] + ((TIG_max[cfc][fc] + 9) / 10);
						if (txd_cfc > txmax)  txd_cfc -= txmax;
					}
#else
					if (TO_max[fc][cfc] >= 0)
					{
						txd_fc = txd + ((TGL_max[fc] + 9) / 10) + ((TO_max[fc][cfc] + 9) / 10);
						if (txd_fc > txmax)  txd_fc -= txmax;
						txd_cfc = PRM[txa1 + cfc * 10 + real * 5 + 3] + ((TGL_max[cfc] + 9) / 10) + ((TO_max[cfc][fc] + 9) / 10);
						if (txd_cfc > txmax)  txd_cfc -= txmax;
					}
#endif
#if defined CCOLTIG && !defined NO_TIGMAX
#ifndef NO_GGCONFLICT
					else /* TIG_max[fc][cfc] <= GK */
					{
						if (TIG_max[fc][cfc] == GK)
						{
							txd_fc = txd;
						}
						else if (TIG_max[fc][cfc] == GKL)
						{
							txd_fc = txd + ((TGL_max[fc] + 9) / 10);
						}
#ifdef GKL_IN_TOMAX
						else if (TIG_max[fc][cfc] < GKL) /* bijv: TIG_max[fc][cfc]= -120 */
						{
							txd_fc = txd + ((TGL_max[fc] + 9) / 10) + (((-TIG_max[fc][cfc]) + 9) / 10);
						}
#endif
						if (txd_fc > txmax) txd_fc -= txmax;

						if (TIG_max[cfc][fc] == GK)
						{
							txd_cfc = PRM[txa1 + cfc * 10 + real * 5 + 3];
						}
						else if (TIG_max[cfc][fc] == GKL)
						{
							txd_cfc = PRM[txa1 + cfc * 10 + real * 5 + 3] + ((TGL_max[cfc] + 9) / 10);
						}
#ifdef GKL_IN_TOMAX
						else if (TIG_max[cfc][fc] < GKL)   /* bijv: TIG_max[fc][cfc]= -120 */
						{
							txd_cfc = PRM[txa1 + cfc * 10 + real * 5 + 3] + ((TGL_max[cfc] + 9) / 10) + (((-TIG_max[cfc][fc]) + 9) / 10);
						}
#endif
#endif
#else
#ifndef NO_GGCONFLICT
					else /* TO_max[fc][cfc] <= GK */
					{
						if (TO_max[fc][cfc] == GK)
						{
							txd_fc = txd;
						}
						else if (TO_max[fc][cfc] == GKL)
						{
							txd_fc = txd + ((TGL_max[fc] + 9) / 10);
						}
#ifdef GKL_IN_TOMAX
						else if (TO_max[fc][cfc] < GKL) /* bijv: TO_max[fc][cfc]= -120 */
						{
							txd_fc = txd + ((TGL_max[fc] + 9) / 10) + (((-TO_max[fc][cfc]) + 9) / 10);
						}
#endif
						if (txd_fc > txmax) txd_fc -= txmax;

						if (TO_max[cfc][fc] == GK)
						{
							txd_cfc = PRM[txa1 + cfc * 10 + real * 5 + 3];
						}
						else if (TO_max[cfc][fc] == GKL)
						{
							txd_cfc = PRM[txa1 + cfc * 10 + real * 5 + 3] + ((TGL_max[cfc] + 9) / 10);
						}
#ifdef GKL_IN_TOMAX
						else if (TO_max[cfc][fc] < GKL)   /* bijv: TO_max[fc][cfc]= -120 */
						{
							txd_cfc = PRM[txa1 + cfc * 10 + real * 5 + 3] + ((TGL_max[cfc] + 9) / 10) + (((-TO_max[cfc][fc]) + 9) / 10);
						}
#endif
#endif
#endif
						if (txd_cfc > txmax) txd_cfc -= txmax;
					}
					if ((txb_fc < txd_fc) && (txb_cfc < txd_cfc))
					{
						if ((txb_fc > txb_cfc) && (txb_fc < txd_cfc))
						{
							conflict_txb_txd = TRUE;
						}
						if ((txb_cfc > txb_fc) && (txb_cfc < txd_fc))
						{
							conflict_txb_txd = TRUE;
						}
					}
					else if (txb_fc < txd_fc && txb_cfc > txd_cfc)
					{
						if ((txb_fc < txd_cfc) || (txd_fc > txb_cfc))
						{
							conflict_txb_txd = TRUE;
						}
					}
					else if (txb_fc > txd_fc && txb_cfc < txd_cfc)
					{
						if ((txb_cfc < txd_fc) || (txd_cfc > txb_fc))
						{
							conflict_txb_txd = TRUE;
						}
					}
					else if (txb_fc > txd_fc && txb_cfc > txd_cfc)
					{
						conflict_txb_txd = TRUE;
					}

					if (conflict_txb_txd)
					{
						sprintf(temp, "realisations for fc%s and fc%s are conflicting in plan %d\n", FC_code[fc], FC_code[cfc], pl + 1);
						uber_puts(temp);
						return TRUE;
					}
				}
			}
		}
	}
	return FALSE;
}

bool TX_between(int tx_value, int tx_first, int tx_second, int tx_max)
{
	bool fReturn = TRUE;

	/* check boundaries */
	if ((tx_value <= tx_max) && (tx_first <= tx_max) && (tx_second <= tx_max))
	{
		if (tx_first < tx_second) // b before d, c should be between b and d
		{
			fReturn = (bool)((tx_value >= tx_first) && (tx_value <= tx_second));
		}
		else
		{
			// b after d, ie 13 and 3
			if (tx_value >= tx_first)
			{
				fReturn = (bool)(((tx_value + tx_max) >= tx_first) && (tx_value < (tx_second + tx_max)));
			}
			else
			{
				fReturn = (bool)(((tx_value + tx_max) >= tx_first) && (tx_value < tx_second));
			}
		}
	}
	else
	{
		fReturn = FALSE;
	}
	return fReturn;
}