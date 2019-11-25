#include "extra_func_ov.h"

bool DSIMeldingOV_V1(
	count dslus,
	count vtgtype,
	bool checkfcnmr,
	count fcnmr,
	bool checktype,
	count meldingtype,
	bool extra)
{
#if !defined (VISSIM) && DSMAX
	if (!DS_MSG || !extra) return FALSE;
#endif

	if (dslus != NG && dslus != CIF_DSI[CIF_DSI_LUS]) return FALSE;
	if (vtgtype != NG && vtgtype != CIF_DSI[CIF_DSI_VTG]) return FALSE;
	if (checkfcnmr && fcnmr != NG && fcnmr != CIF_DSI[CIF_DSI_DIR]) return FALSE;
	if (checktype && meldingtype != NG && meldingtype != CIF_DSI[CIF_DSI_TYPE]) return FALSE;

	return TRUE;
}

bool DSIMeldingOV_LijnNummer_V1(count lijnparm, count lijnmax)
{
	int index = 0;
	if (PRM[lijnparm]) return TRUE;
	if (CIF_DSI[CIF_DSI_LYN] == 0) return FALSE;
	for (index = 0; index < lijnmax; ++index)
	{
		if (PRM[lijnparm + 1 + index] != 0 && CIF_DSI[CIF_DSI_LYN] == PRM[lijnparm + 1 + index]) return TRUE;
	}
	return FALSE;
}

bool DSIMeldingOV_LijnNummerEnRitCategorie_V1(count lijnparm, count lijnmax)
{
	int index = 0;
	if (PRM[lijnparm]) return TRUE;
	if (CIF_DSI[CIF_DSI_LYN] == 0) return FALSE;
	for (index = 0; index < lijnmax; ++index)
	{
		if (PRM[lijnparm + 1 + index] != 0 && CIF_DSI[CIF_DSI_LYN] == PRM[lijnparm + 1 + index] &&
			(PRM[lijnparm + 11 + index] != 0 && CIF_DSI[CIF_DSI_RITC] == PRM[lijnparm + 11 + index] ||
				PRM[lijnparm + 11 + index] == 999)) return TRUE;
	}
	return FALSE;
}

bool DSIMelding_HD_V1(count dir,         /* 1. fc nummer of richtingnummer (201, 202, 203)  */
	                  count meldingtype, /* 2. Type melding: in of uit */
	                  bool check_sirene) /* 3. Check SIRENE */
{
	if ((CIF_DSI[CIF_DSI_VTG] == CIF_POL || 
		 CIF_DSI[CIF_DSI_VTG] == CIF_BRA || 
		 CIF_DSI[CIF_DSI_VTG] == CIF_AMB) &&  /* juiste voertuigtype? */
		(!check_sirene || (CIF_DSI[CIF_DSI_PRI] == CIF_SIR)) &&
		(CIF_DSI[CIF_DSI_DIR] == dir) &&  /* geldt deze melding voor deze richting? */
		(CIF_DSI[CIF_DSI_TYPE] == meldingtype)      /* is dit een in of een uitmelding? */
#if !defined (VISSIM) && DSMAX
		&& DS_MSG
#endif
		) return TRUE;

	return FALSE;
}

/* Bijhouden stiptheid inkomende KAR berichten */
void TrackStiptObvTSTP(count hin, count huit, int * iAantInm, int iKARInSTP[], count hov, int grensvroeg, int grenslaat)
{
	/* reset alles */
	if (EH[hov])
	{
		int i = 0;
		*iAantInm = 0;
		for (; i < MAX_AANTAL_INMELDINGEN; ++i)
		{
			iKARInSTP[i] = 0;
		}
	}

	/* Bijhouden stiptheidsklassen ingemelde voertuigen */
	/* Bij inmelding: registeren stiptheidsklasse achterste voertuig */
	if (IH[hin] && !H[hin])
	{
		if (*iAantInm < MAX_AANTAL_INMELDINGEN)
		{
			/* Bepalen stiptheidsklasse op basis van afwijking van de dienstregeling */
			if (CIF_DSI[CIF_DSI_TSTP] > grenslaat)              iKARInSTP[*iAantInm] = CIF_TE_LAAT;
			else if (CIF_DSI[CIF_DSI_TSTP] < (-1 * grensvroeg)) iKARInSTP[*iAantInm] = CIF_TE_VROEG;
			else                                                              iKARInSTP[*iAantInm] = CIF_OP_TIJD;
			*iAantInm = (*iAantInm) + 1;
		}
	}

	/* Bij uitmelding: opschuiven resterende registraties naar voren */
	if (IH[huit] && !H[huit])
	{
		if (*iAantInm > 1)
		{
			int i = 0;
			int t1 = 0;
			for (; i < (MAX_AANTAL_INMELDINGEN - 1); ++i)
			{
				t1 = iKARInSTP[i + 1];
				iKARInSTP[i] = t1;
			}
			iKARInSTP[*iAantInm - 1] = 0;
			*iAantInm = (*iAantInm) - 1;
		}
		else if (*iAantInm == 1)
		{
			iKARInSTP[*iAantInm - 1] = 0;
			*iAantInm = (*iAantInm) - 1;
		}
		else /* failsafe */
		{
			int i = 0;
			*iAantInm = 0;
			for (; i < MAX_AANTAL_INMELDINGEN; ++i)
			{
				iKARInSTP[i] = 0;
			}
		}
	}
}

/**
 Functie : OV_teller
 Functionele omschrijving : Bij wijziging van OV_teller wordt deze in CIF_UBER geschreven.
 **/
void OV_teller(count cov, count scov)
{
	if (scov == NG || SCH[scov])
	{
		if (C_counter[cov] != C_counter_old[cov])
		{
			uber_puts(PROMPT_code);
			uber_puts(C_code[cov]);
			uber_puts("= ");
			uber_puti(C_counter[cov]);
			uber_puts("\n");
		}
	}
	/* onthouden C_counter */
	/* ------------------- */
	C_counter_old[cov] = C_counter[cov];
}

#ifdef CCOL_IS_SPECIAL
/*  de functie reset_DSI-message()
-  kan worden gebruikt voor het verwijderen van een oud DSI-bericht.
-  zet alle variabelen van de DSI-buffer op de juiste defaultwaarde.
-  dient tijdens testen te worden aangeroepen vanuit de is_special_signals().
*/
void reset_DSI_message(void)
{
	register int i;

	for (i = 0; i < CIF_AANT_DSI; ++i)      CIF_DSI[i] = (s_int16)0;

	/* set afwijkende defaultwaarden */
	CIF_DSI[CIF_DSI_TSTP] = (s_int16)CIF_DSI_TSTP_DEF;
	CIF_DSI[CIF_DSI_LSS] = (s_int16)CIF_DSI_LSS_DEF;
	CIF_DSI[CIF_DSI_TSS] = (s_int16)CIF_DSI_TSS_DEF;
}

/*  de functie set_DSI-message()
-  voor het plaatsen van een selectief detectiebericht in de DSI-buffer.
-  geeft alle variabelen de juiste waarde en zet de wijzigvlag CIF_DSIWIJZ op.
-  dient in de testomgeving te worden aangeroepen vanuit de is_special_signals().
*/
void set_DSI_message(mulv ds, s_int16 vtg, s_int16 dir, count type, s_int16 stiptheid, s_int16 aantalsecvertr, s_int16 PRM_lijnnr, s_int16 PRM_ritcat, s_int16 prio)
{
	CIF_DSI[CIF_DSI_LUS] = (s_int16)(ds < 0 ? 0 : ds);
	CIF_DSI[CIF_DSI_VTG] = (vtg < 0 ? 0 : vtg);
	CIF_DSI[CIF_DSI_DIR] = (dir < 0 ? 0 : dir);
	CIF_DSI[CIF_DSI_TYPE] = (type < 0 ? 0 : type);
	CIF_DSI[CIF_DSI_LYN] = (PRM_lijnnr < 0 ? 0 : PRM_lijnnr);
	CIF_DSI[CIF_DSI_RITC] = (PRM_ritcat < 0 ? 0 : PRM_ritcat);
	CIF_DSI[CIF_DSI_STP] = (stiptheid < 0 ? 0 : stiptheid);
	CIF_DSI[CIF_DSI_TSTP] = aantalsecvertr;
	CIF_DSI[CIF_DSI_PRI] = (prio < 0 ? 0 : prio);
	/* zet wijzig vlag
	*/
	CIF_DSIWIJZ = 1;
}

#endif

#ifdef OV_CHECK_WAGENNMR

/* Check op wagendienstnummer openbaar vervoer */
#define WDNSTlist 10    /* maximaal 10 wagendienstnummers onthouden (IN, VOOR) */
#define WDNSTlistuit 30 /* maximaal 30 wagendienstnummers onthouden (UIT)      */
int WDNSTblock = 5;     /* wagendienstnummers maximaal 5 minuten blokkeren     */

/* Een inmelding ruimt de voormelding van hetzelfde WDNSTnr op */
/* Een uitmelding ruimt de inmelding van hetzelfde WDNSTnr op  */
/* Een uitmelding kan niet worden opgeruimd door een volgende melding, daarom een 3 x zo groot array*/

mulv WDNST_fc_in[FCMAX][WDNSTlist];          /* array met wagendienstnummer inmeldingen      */
mulv WDNST_fc_uit[FCMAX][WDNSTlistuit];      /* array met wagendienstnummer uitmeldingen     */
mulv WDNST_fc_voor[FCMAX][WDNSTlist];        /* array met wagendienstnummer voormeldingen    */
mulv WDNST_cifsect_in[FCMAX][WDNSTlist];     /* array met CIF_SEC_TELLER tijd inmeldingen    */
mulv WDNST_cifsect_uit[FCMAX][WDNSTlistuit]; /* array met CIF_SEC_TELLER tijd uitmeldingen   */
mulv WDNST_cifsect_voor[FCMAX][WDNSTlist];   /* array met CIF_SEC_TELLER tijd voormeldingen  */

/* iedere (60 * WDNSTblock) seconde oude wagendienstnummers verwijderen */
void WDNST_cleanup(void)
{
	if (TM) /* 1 x per minuut is voldoende */
	{
		count fc, listnr;
		for (fc = 0; fc < FCMAX; ++fc)
		{
			for (listnr = 0; listnr < WDNSTlist; ++listnr)
			{
				int diff;

				/* voormeldingen */
				if (CIF_KLOK[CIF_SEC_TELLER] >= WDNST_cifsect_voor[fc][listnr])
				{
					diff = CIF_KLOK[CIF_SEC_TELLER] - WDNST_cifsect_voor[fc][listnr];
				}
				else
				{ /* MAX_KLOKTELLER = 32767, zie control.c */
					diff = MAX_KLOKTELLER + CIF_KLOK[CIF_SEC_TELLER] - WDNST_cifsect_voor[fc][listnr];
				}
				if (diff > WDNSTblock * 60)
				{
					WDNST_fc_voor[fc][listnr] = 0;
					WDNST_cifsect_voor[fc][listnr] = 0;
				}

				/* inmeldingen */
				if (CIF_KLOK[CIF_SEC_TELLER] >= WDNST_cifsect_in[fc][listnr])
				{
					diff = CIF_KLOK[CIF_SEC_TELLER] - WDNST_cifsect_in[fc][listnr];
				}
				else
				{ /* MAX_KLOKTELLER = 32767, zie control.c */
					diff = MAX_KLOKTELLER + CIF_KLOK[CIF_SEC_TELLER] - WDNST_cifsect_in[fc][listnr];
				}
				if (diff > WDNSTblock * 60)
				{
					WDNST_fc_in[fc][listnr] = 0;
					WDNST_cifsect_in[fc][listnr] = 0;
				}

			}

			for (listnr = 0; listnr < WDNSTlistuit; ++listnr)
			{
				int diff;

				/* uitmeldingen */
				if (CIF_KLOK[CIF_SEC_TELLER] >= WDNST_cifsect_uit[fc][listnr])
				{
					diff = CIF_KLOK[CIF_SEC_TELLER] - WDNST_cifsect_uit[fc][listnr];
				}
				else
				{ /* MAX_KLOKTELLER = 32767, zie control.c */
					diff = MAX_KLOKTELLER + CIF_KLOK[CIF_SEC_TELLER] - WDNST_cifsect_uit[fc][listnr];
				}
				if (diff > WDNSTblock * 60)
				{
					WDNST_fc_uit[fc][listnr] = 0;
					WDNST_cifsect_uit[fc][listnr] = 0;
				}
			}
		}
	}
}

bool WDNST_check_in(count fc)
{
	count listnr;
	int firstempty = 999;
	bool WDNSTbestaatniet = TRUE;
	int richting = atoi(FC_code[fc]) < 201 ? atoi(FC_code[fc]) : atoi(FC_code[fc]) - 200;

	if ((CIF_DSIWIJZ == 1) && (CIF_DSI[CIF_DSI_WDNST] != 0) && (CIF_DSI[CIF_DSI_TYPE] == CIF_DSIN) &&
		(CIF_DSI[CIF_DSI_DIR] == richting))
	{
		for (listnr = 0; listnr < WDNSTlist; ++listnr)
		{
			if (WDNST_fc_in[fc][listnr] == CIF_DSI[CIF_DSI_WDNST])
			{
				WDNSTbestaatniet = FALSE;
			}
			if (WDNST_fc_in[fc][listnr] == 0)
			{
				if (firstempty == 999) firstempty = listnr;
			}
		}
		if (WDNSTbestaatniet == TRUE)
		{
			if (firstempty != 999)
			{
				WDNST_fc_in[fc][firstempty] = CIF_DSI[CIF_DSI_WDNST];
				WDNST_cifsect_in[fc][firstempty] = CIF_KLOK[CIF_SEC_TELLER];
			}
		}
	}
	return WDNSTbestaatniet;
}

bool WDNST_check_uit(count fc)
{
	count listnr, listnr2;
	int firstempty = 999;
	bool WDNSTbestaatniet = TRUE;
	int richting = atoi(FC_code[fc]) < 201 ? atoi(FC_code[fc]) : atoi(FC_code[fc]) - 200;

	if ((CIF_DSIWIJZ == 1) && (CIF_DSI[CIF_DSI_WDNST] != 0) && (CIF_DSI[CIF_DSI_TYPE] == CIF_DSUIT) &&
		(CIF_DSI[CIF_DSI_DIR] == richting))
	{
		firstempty = 999;
		for (listnr = 0; listnr < WDNSTlistuit; ++listnr)
		{
			if (WDNST_fc_uit[fc][listnr] == CIF_DSI[CIF_DSI_WDNST])
			{
				WDNSTbestaatniet = FALSE;
			}
			if (WDNST_fc_uit[fc][listnr] == 0)
			{
				if (firstempty == 999) firstempty = listnr;
			}
		}
		if (WDNSTbestaatniet == TRUE)
		{
			if (firstempty != 999)
			{
				WDNST_fc_uit[fc][firstempty] = CIF_DSI[CIF_DSI_WDNST];
				WDNST_cifsect_uit[fc][firstempty] = CIF_KLOK[CIF_SEC_TELLER];

				/* opruimen bijbehorende inmelding */
				for (listnr2 = 0; listnr2 < WDNSTlist; ++listnr2)
				{
					if (WDNST_fc_in[fc][listnr2] == CIF_DSI[CIF_DSI_WDNST])
					{
						WDNST_fc_in[fc][listnr2] = 0;
						WDNST_cifsect_in[fc][listnr2] = 0;
					}
				}
			}
		}
	}
	return WDNSTbestaatniet;
}

#endif // OV_CHECK_WAGENNMR