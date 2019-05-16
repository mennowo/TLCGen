/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* CCOL - OV Prioriteit signaalplanstructuur                                              */
/*                                                                                        */
/*                                                                                        */
/* (C) Copyright 2017 Royal HaskoningDHV b.v. All rights reserved.                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* Versie :  2.0                                                                          */
/*           Integratie met uitgebreide OV module CCOL Generator                          */
/* Naam   :  ov_ple.h                                                                     */
/* Datum  :  14-06-2017                                                                   */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */

#ifndef __OV_PLE__
#define __OV_PLE__

extern mulv TXB_PL[FCMAX], TXD_PL[FCMAX];
mulv TVGPL_max[FCMAX];

bool HoofdRichting[FCMAX];             /* Array met hoofdrichtingen                       */
bool HoofdRichtingTegenhouden[FCMAX];  /* Tegenhouden hoofdrichting (TXC of minimum groen)*/
bool HoofdRichtingAfkappenYWPL[FCMAX]; /* Afkappen YW_PL hoofdrichting (na minimum groen) */
bool HoofdRichtingAfkappenYVPL[FCMAX]; /* Afkappen YV_PL hoofdrichting (na minimum groen) */

/* -------------------------------------------------------------------------------------- */
/* Gereserveerde bitwaarde tbv OV ingrepen tijdens signaalplan                            */
/* -------------------------------------------------------------------------------------- */

#define OV_PLE_BIT          BIT14  /* FM tbv OV ingrepen tijdens PL             */

void BepaalHoofdrichtingOpties(void);
int  TijdTotLaatsteRealisatieMomentConflict(int, int, int);
bool StartGroenConflictenUitstellen(count, int);
void set_pg_primair_fc_ov_ple(void);
void signaalplan_primair_ov_ple(void);

void OVHalfstarBepaalHoofdrichtingOpties(int, ...);
int  OVHalfstarBepaalPrioriteitsOpties(int);

void OVHalfstarInit(void);
void OVHalfstarSettings(void);
void OVHalfstarOnderMaximum(void);
void OVHalfstarAfkapGroen(void);
void OVHalfstarStartGroenMomenten(void);
void OVHalfstarAfkappen(void);
void OVHalfstarTerugkomGroen(void);
void OVHalfstarGroenVasthouden(void);
void OVHalfstarMeetkriterium(void);


void OVHalfstarTerugkomGroen(void)
{
	int fc;
	if (IH[hplact])
	{
		for (fc = 0; fc < FCMAX; ++fc)
			TVG_max[fc] = NG;
	}
}

void OVHalfstarOnderMaximum(void)
{
	int ov, fc;
	int iMaxResterendeGroenTijd;

	if (IH[hplact])
	{
		for (ov = 0; ov < ovOVMAX; ++ov) {
			fc = iFC_OVix[ov];

			iMaxResterendeGroenTijd = 0;
			if (G[fc])
			{
				int iLatestTXD = ((TXD_PL[fc] + iExtraGroenNaTXD[ov]) % TX_PL_max);
				if ((TOTXB_PL[fc] == 0) && (TOTXD_PL[fc] > 0))
				{
					/* primair gebied */          iMaxResterendeGroenTijd = TOTXD_PL[fc];
					if (iPrioriteitsOpties[ov] & poPLGroenVastHoudenNaTXD)
						iMaxResterendeGroenTijd += iExtraGroenNaTXD[ov];
				}
				else if (TX_between(TX_PL_timer, TXD_PL[fc], iLatestTXD, TX_PL_max) && (iPrioriteitsOpties[ov] & poPLGroenVastHoudenNaTXD))
				{
					/* ExtraGroenNaTXD gebied */
					iMaxResterendeGroenTijd = (iLatestTXD - TX_PL_timer + TX_PL_max) % TX_PL_max;
				}
				else
				{
					/* bijzondere realisatie */
					iMaxResterendeGroenTijd = iGroenBewakingsTijd[ov] - iGroenBewakingsTimer[ov];
				}
			}
			else // !G[fc]
			{
				iMaxResterendeGroenTijd = iGroenBewakingsTijd[ov];
			}

			iOnderMaximumVerstreken[ov] = iOnderMaximum[ov] >= iMaxResterendeGroenTijd;
		}
	}
}

void OVHalfstarAfkapGroen(void)
{
	int fc;
	if (IH[hplact]) 
	{
		for (fc = 0; fc < FCMAX; ++fc)
		{
			/* MaxGroenTijdTerugKomen op 0 zetten, anders loopt signaalplan uit de pas */
			iMaxGroenTijdTerugKomen[fc] = 0;
		}
	}
}

void OVHalfstarStartGroenMomenten(void)
{
	if (IH[hplact])
	{
		int ov;
		for (ov = 0; ov < ovOVMAX; ++ov) {
				{
					if (iAantalInmeldingen[ov] > 0)
					{
						if (!StartGroenConflictenUitstellen(iFC_OVix[ov], iPrioriteitsOpties[ov]))
							iStartGroen[ov] = 9999;
					}
				}

		}
	}
}

void OVHalfstarAfkappen(void)
{
	if (IH[hplact])
	{
		for (fc = 0; fc < FCMAX; ++fc)
		{
			if (HoofdRichting[fc] && G[fc] && YW_PL[fc] && !HoofdRichtingAfkappenYWPL[fc])
				iNietAfkappen[fc] |= BIT11;
			if (HoofdRichting[fc] && G[fc] && YV_PL[fc] && HoofdRichtingAfkappenYVPL[fc] && (iNietAfkappen[fc] & BIT11))
				iNietAfkappen[fc] &= ~BIT11;
		}
	}
}

void OVHalfstarGroenVasthouden(void)
{
	if (IH[hplact])
	{
		for (ov = 0;
			ov < ovOVMAX;
			ov++) {

			fc = iFC_OVix[ov];
			magUitstellen = StartGroenConflictenUitstellen(fc, iPrioriteitsOpties[ov]);
			// Reset OV_YV_BIT, will determine YV according to signalplan structure
			YV[fc] &= ~OV_YV_BIT;

			if (iPrioriteit[ov] &&
				(iPrioriteitsOpties[ov] & poGroenVastHouden) || (iPrioriteitsOpties[ov] & poPLGroenVastHoudenNaTXD)) {

				if (G[fc] && (iGroenBewakingsTimer[ov] < iGroenBewakingsTijd[ov]) && magUitstellen) {
					YV[fc] |= OV_YV_BIT;
				}
				if (!magUitstellen)
					iWachtOpKonflikt[ov] = TRUE;
			}
		}
	}
}

void OVHalfstarMeetkriterium(void)
{
	if (IH[hplact])
	{
		for (ov = 0;
			ov < ovOVMAX;
			ov++) {
			fc = iFC_OVix[ov];
			iRestGroen = 0;

			// Reset OV_MK_BIT, will determine MK according to signalplan structure
			MK[fc] &= ~OV_MK_BIT;

			if (G[fc])
			{
				if (TOTXB_PL[fc] == 0 && TOTXD_PL[fc] > 0) // primary
				{
					iRestGroen = TOTXD_PL[fc];
					if (iPrioriteitsOpties[ov] & poPLGroenVastHoudenNaTXD)
						iRestGroen += iExtraGroenNaTXD[ov];
				}
				else if (TOTXB_PL[fc] > 0 && TXD_PL[fc] > 0) // non primary
				{
					//extra groen na txd
					int iLatestTXD = ((TXD_PL[fc] + iExtraGroenNaTXD[ov]) % TX_PL_max);
					iRestGroen = (iLatestTXD - TX_PL_timer + TX_PL_max) % TX_PL_max;

					//bijzondere realisatie
					iRestGroen = iGroenBewakingsTijd[ov] - iGroenBewakingsTimer[ov];
				}

				if (iPrioriteit[ov] &&
					((iPrioriteitsOpties[ov] & poGroenVastHouden) || (iPrioriteitsOpties[ov] & poPLGroenVastHoudenNaTXD)) &&
					(iGroenBewakingsTimer[ov] < iGroenBewakingsTijd[ov]) ||
					((PR[fc] & PRIMAIR_VERSNELD) &&
					((iGroenBewakingsTimer[ov] < iGroenBewakingsTijd[ov]) || (iAantalInmeldingen[ov] > 0) && !ka(fc)) &&
						((iGroenBewakingsTijd[ov] - iGroenBewakingsTimer[ov]) <= iRestGroen)))
				{
					if (G[fc] && (StartGroenConflictenUitstellen(fc, iPrioriteitsOpties[ov])))
						MK[fc] |= OV_MK_BIT;
					else
						MK[fc] = 0;
				}
			}
		}
	}
}

#endif
