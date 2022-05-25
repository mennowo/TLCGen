#include "extra_func_prio.h"
#include "prio.h"

bool DSIMeldingPRIO_V1(
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
	if (vtgtype > 0 && vtgtype != CIF_DSI[CIF_DSI_VTG]) return FALSE;
	if (checkfcnmr && fcnmr != NG && fcnmr != CIF_DSI[CIF_DSI_DIR]) return FALSE;
	if (checktype && meldingtype != NG && meldingtype != CIF_DSI[CIF_DSI_TYPE]) return FALSE;

	return TRUE;
}

bool DSIMeldingPRIO_V2(           /* Fik220201 */
   count fc,                       /* fasecyclus */
   count prio_fc,                  /* index prioriteit */
   count dslus,                    /* lusnummer in Kar bericht */
   count vtgtype,                  /* voertuigtype */
   bool checkfcnmr,               /* controleer fasecyclus nummer in Kar bericht */
   count fcnmr,                    /* ... -> ... fasecyclus nummer in Kar bericht */
   bool checktype,                /* controleer meldingstype in Kar bericht */
   count meldingtype,              /* ... -> ... meldingstype in Kar bericht */
   bool extra)                    /* */
{
   bool melding = TRUE;

   /* Correctie uitmelding van eerste bus fc tijdens rood */
   if (vertraag_kar_uitm[prio_fc]) PrioUitmelden(prio_fc, SG[fc]); /* vertraagde uitmelding op start groen */
   if (SG[fc]) vertraag_kar_uitm[prio_fc] = FALSE;

#if !defined (VISSIM) && DSMAX
   if (!DS_MSG || !extra) melding = FALSE;
#endif

   if (dslus != NG && dslus != CIF_DSI[CIF_DSI_LUS]) melding = FALSE;
   if (vtgtype > 0 && vtgtype != CIF_DSI[CIF_DSI_VTG]) melding = FALSE;
   if (checkfcnmr && fcnmr != NG && fcnmr != CIF_DSI[CIF_DSI_DIR]) melding = FALSE;
   if (checktype && meldingtype != NG && meldingtype != CIF_DSI[CIF_DSI_TYPE]) melding = FALSE;

   /* uitmelding eerste bus tijdens rood, tijdens 1e seconde rood gaan we ervan uit dat de bus toch doorgereden is */
#if (CCOL_V >= 110)
   if (R[fc] && TR_timer[fc] > 10 && (!vertraag_kar_uitm[prio_fc] || iAantalInmeldingen[prio_fc] == 1))
#else
   if (R[fc] && TFB_timer[fc] > 10 && (!vertraag_kar_uitm[prio_fc] || iAantalInmeldingen[prio_fc] == 1))
#endif
   {
      if (iAantalInmeldingen[prio_fc] > 0 && dslus == 0 && CIF_DSI[CIF_DSI_TYPE] == CIF_DSUIT)
      {
        if (melding)
        {
          vertraag_kar_uitm[prio_fc] = TRUE;    /* correctie, uitmelding vertragen tot start groen */
          melding = FALSE;   /* intrekken eerste uitmelding tijdens rood */
        }
      }
   }

   return melding;
}

bool DSIMeldingPRIO_LijnNummer_V1(count lijnparm, count lijnmax)
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

bool DSIMeldingPRIO_LijnNummerEnRitCategorie_V1(count lijnparm, count lijnmax)
{
	int index = 0;
	if (PRM[lijnparm]) return TRUE;
	if (CIF_DSI[CIF_DSI_LYN] == 0) return FALSE;
	for (index = 0; index < lijnmax; ++index)
	{
		if (PRM[lijnparm + 1 + index] != 0 && CIF_DSI[CIF_DSI_LYN] == PRM[lijnparm + 1 + index] &&
			(PRM[lijnparm + 1 + lijnmax + index] != 0 && CIF_DSI[CIF_DSI_RITC] == PRM[lijnparm + 1 + lijnmax + index] ||
				PRM[lijnparm + 1 + lijnmax + index] == 999)) return TRUE;
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
 Functie : PRIO_teller
 Functionele omschrijving : Bij wijziging van PRIO_teller wordt deze in CIF_UBER geschreven.
 **/
void PRIO_teller(count cov, count scov)
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

#ifdef PRIO_CHECK_WAGENNMR

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

#endif // PRIO_CHECK_WAGENNMR

void NevenMelding(count ov1,      /* OV fasecyclus 1                */
                  count ov2,      /* OV fasecyclus 2                */
                  count ov3,      /* OV fasecyclus 3                */
                  count d,        /* koplus                         */
                  count prmrtbl,  /* Bezettijd laag tbv normaal     */
                  count prmrtbh,  /* Bezettijd hoog tbv OV aanwezig */
                  count hovss1,   /* hulpelement ov bij SS     fc1  */
                  count hovss2,   /* hulpelement ov bij SS     fc2  */
                  count hovss3,   /* hulpelement ov bij SS     fc3  */
                  count hneven1,  /* hulpelement nevenmelding  fc1  */
                  count hneven2,  /* hulpelement nevenmelding  fc2  */
                  count hneven3  /* hulpelement nevenmelding  fc3  */
                  )
{
  /* ------------------------------------------------------------ */
  /* FC-indices bepalen behorende bij OV-fc's                     */
  /* ------------------------------------------------------------ */
  count fc1;
  count fc2;
  count fc3;

  fc1 =                    iFC_PRIOix[ov1];
  fc2 =                    iFC_PRIOix[ov2];
  fc3 = (ov3 == NG) ? NG : iFC_PRIOix[ov3];

  /* ------------------------------------------------------------ */
  /* Als koplus bezet, nevenmelding als geldt:                    */
  /* - rood en niet garantierood                                  */
  /* - en geen OV-voertuig aanwezig                               */
  /* - en op andere richting wel OV-voertuig aanwezig             */
  /* - en dit voertuig heeft stopstreep nog niet kunnen bereiken  */
  /* ------------------------------------------------------------ */
    IH[hneven1] |= DB[d] && iAantalInmeldingen[ov2] && !IH[hovss2] && R[fc1] && !TRG[fc1] && !iAantalInmeldingen[ov1];  /* nevenmelding fc1 van fc2 */
    IH[hneven2] |= DB[d] && iAantalInmeldingen[ov1] && !IH[hovss1] && R[fc2] && !TRG[fc2] && !iAantalInmeldingen[ov2];  /* nevenmelding fc2 van fc1 */

  if(fc3 != NG)
  {
    IH[hneven1] |= DB[d] && iAantalInmeldingen[ov3] && !IH[hovss3] && R[fc1] && !TRG[fc1] && !iAantalInmeldingen[ov1];  /* nevenmelding fc1 van fc3 */
    IH[hneven2] |= DB[d] && iAantalInmeldingen[ov3] && !IH[hovss3] && R[fc2] && !TRG[fc2] && !iAantalInmeldingen[ov2];  /* nevenmelding fc2 van fc3 */
    IH[hneven3] |= DB[d] && iAantalInmeldingen[ov1] && !IH[hovss1] && R[fc3] && !TRG[fc3] && !iAantalInmeldingen[ov3];  /* nevenmelding fc3 van fc1 */
    IH[hneven3] |= DB[d] && iAantalInmeldingen[ov2] && !IH[hovss2] && R[fc3] && !TRG[fc3] && !iAantalInmeldingen[ov3];  /* nevenmelding fc3 van fc2 */
  }

  /* ------------------------------------------------------------ */
  /* nevenmelding onthouden:                                      */
  /* - zolang hiaattijd van koplus loopt                          */
  /* - maar nooit langer onthouden dan t/m vastgroentijd          */
  /* ------------------------------------------------------------ */
    IH[hneven1] = IH[hneven1] && TDH[d] && (R[fc1] || VS[fc1] || FG[fc1]);
    IH[hneven2] = IH[hneven2] && TDH[d] && (R[fc2] || VS[fc2] || FG[fc2]);

  if(fc3 != NG)
    IH[hneven3] = IH[hneven3] && TDH[d] && (R[fc3] || VS[fc3] || FG[fc3]);

  /* ------------------------------------------------------------ */
  /* Bezettijd instellen (tbv voorkomen valse aanvraag):          */
  /* - Hoge waarde bij OV aanwezig bij stopstreep                 */
  /* - Anders lage/normale waarde bij geen OV aanwezig            */
  /* ------------------------------------------------------------ */
  TDB_max[d] =                IH[hovss1] ||
                              IH[hovss2] ||
               (fc3 != NG) && IH[hovss3] ? PRM[prmrtbh] : PRM[prmrtbl];
}

void fietsprio_update(
     count fc,          /* Fasecyclus */
     count dvw,         /* Verweg detector */
     count c_priocount, /* Counter tellen voertuigen */
     count c_priocyc,   /* Counter aantal keer prio per cyclus */
     bool prioin,      /* Hulpelement inmelding prio */
     count ml)          /* Actieve module */
{
    /* fietsprioriteit */
    /* eenmaal per cyclus */
    RC[c_priocyc] = SML && (ml==ML1);
    INC[c_priocyc] = prioin;
    /* aantal fietsers tellen */
    if (dvw != NG)
    {
        RC[c_priocount] = !R[fc];
        INC[c_priocount] = R[fc] && SD[dvw];
    }
}

bool fietsprio_inmelding(
     count fc,               /* Fasecyclus */
     count dvw,              /* Verweg detector */
     count c_priocount,      /* Counter tellen voertuigen */
     count c_priocyc,        /* Counter aantal keer prio per cyclus */
     count prm_prioblok,     /* Bitwise bepalen toegestane blokken */
     count prm_priocyc,      /* Maximum aantal keer prio per cyclus */
     count prm_priocount,    /* Minimum aantal voertuigen voor prio */
     count prm_priowt,       /* Minimum wachttijd voor prio */
     bool prioin,            /* Hulpelement inmelding prio */
     count ml,               /* Actieve module */
     count me_priocount,     /* Memory-element tellen voertuigen RIS */
     count prm_priocountris) /* Minimum aantal voertuigen voor prio RIS */
{
     /* Check juiste blok */
     if (!(PRM[prm_prioblok] & (1 << ml))) return FALSE;

     /* Check aantal keer prio per cyclus niet overschreden */
     if (C[c_priocyc] && (C_counter[c_priocyc] >= PRM[prm_priocyc]))
         return FALSE;

     /* prio actief indien: voldoende voertuigen OF voldoende wachttijd */
     return
           R[fc] && !TRG[fc] &&
           /* voldoende voertuigen massadetectie */
           (dvw != NG && C_counter[c_priocount] >= PRM[prm_priocount] ||
           /* voldoende voertuigen massadetectie */
           A[fc] && TFB_timer[fc] >= PRM[prm_priowt] ||
          /* voldoende voertuigen RIS */
          me_priocount > NG && prm_priocountris > NG && (MM[me_priocount] >= PRM[prm_priocountris]));
}