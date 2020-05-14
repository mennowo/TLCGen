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
	if (vtgtype != NG && vtgtype != CIF_DSI[CIF_DSI_VTG]) return FALSE;
	if (checkfcnmr && fcnmr != NG && fcnmr != CIF_DSI[CIF_DSI_DIR]) return FALSE;
	if (checktype && meldingtype != NG && meldingtype != CIF_DSI[CIF_DSI_TYPE]) return FALSE;

	return TRUE;
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

/* RIS INMELDING SELECTIEF */
/* ======================= */
/* ris_inmelding_selectief() geeft als return waarde waar (TRUE), als er in de RIS_ITSSTATION_AP-buffer een RIS-voertuig aanwezig is van het juiste stationtype
 * in het opgegeven gebied van de rijstrook t.o.v. de stopstreep met een prioriteitsverzoek en nog geen inmelding voor het RIS-voertuig heeft, anders niet waar (FALSE).
 * de inmelding wordt bewaard in de variabele RIS_PRIOREQUEST_EX_AP[r].prioControlState.
 * length_start ligt dichter bij de stopstreep dan length_end. indien match_signalgroup waar (TRUE) is, dient er ook een signalgroup-match te zijn voor het RIS-voertuig (ItsStation),
 * anders wordt signalgroup buiten beschouwing gelaten.
 * ris_inmelding_selectief() kan worden aangeroepen vanuit de functie application().
 *
 * voorbeeld: IH[hovin421] = ris_inmelding_selectief (fc42, "", ris_lane421, RIS_BUS | RIS_TRAM, 20, 90, FALSE);          // geen test op intersection
 *            IH[hovin481] = ris_inmelding_selectief (fc48, SYSTEM_ITF, ris_lane481, RIS_BUS | RIS_TRAM, 20, 90, FALSE);
 */

rif_bool ris_inmelding_selectief(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_bool match_signalgroup)
{
   register count i, j, r = 0;

   while (r < RIS_PRIOREQUEST_AP_NUMBER) { /* doorloop alle PrioRequest objecten */
      if ( (RIS_PRIOREQUEST_EX_AP[r].prioControlState == RIF_PRIORITIZATIONSTATE_UNKNOWN)    /* nog geen inmelding aanwezig */
           && (strcmp(RIS_PRIOREQUEST_AP[r].signalGroup, FC_code[fc]) == 0) ) {              /* test op juiste signaalgroep */
         if ((RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_REQUEST) || (RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_UPDATE)) {
            i = 0;
            while (i < RIS_ITSSTATION_AP_NUMBER) {    /* doorloop alle ItsStation objecten */
               if (strcmp(RIS_PRIOREQUEST_AP[r].itsStation, RIS_ITSSTATION_AP[i].id) == 0) { /* test op zelfde ItsStation ID */
                  if (stationtype_bits & (1 << RIS_ITSSTATION_AP[i].stationType)) {          /* test stationType - bit       */
                     for (j = 0; j < RIF_MAXLANES; j++) {
                        if (!strlen(RIS_ITSSTATION_AP[i].matches[j].intersection) || (strlen(intersection) && (strcmp(RIS_ITSSTATION_AP[i].matches[j].intersection, intersection) != 0))) {   /* test intersection ID */
                           break;
                        }
                        if (lane_id == RIS_ITSSTATION_AP[i].matches[j].lane) {  /* test op juiste lane id */
                           if (!match_signalgroup || (strcmp(RIS_ITSSTATION_AP[i].matches[j].signalGroup, FC_code[fc])) == 0) {      /* test op juiste signaalgroep  */
                              if ((RIS_ITSSTATION_EX_AP[i].matches[j].distance > length_start) && (RIS_ITSSTATION_EX_AP[i].matches[j].distance <= length_end)) { /* test distance */
                                 RIS_PRIOREQUEST_EX_AP[r].prioControlState = RIF_PRIORITIZATIONSTATE_REQUESTED;    /* zet inmelding voor ID       */ /* CCA: aanpassing naar RIF_PRIORITIZATIONSTATE_REQUESTED */
                                 return ((rif_bool) TRUE);
                              }
                           }
                        }
                     }
                  }
               }
               i++;
            }
         }
      }
      r++;
   }
   return ((rif_bool) FALSE);
}



/* RIS UITMELDING SELECTIEF * /
/* ========================= */
/* ris_uitmelding_selectief() geeft als return waarde waar (TRUE), als er voor de opgegeven signaalgroep in de RIS_PRIOREQUEST_AP-buffer een SRM-bericht
 * met requesttype == CANCELLATION aanwezig is, of er geen SRM-bericht aanwezig is voor de signaalgroep, anders niet waar (FALSE).
 * ris_uitmelding_selectief() kan worden aangeroepen vanuit de functie application().
 *
 * voorbeeld: IH[hovuit421] = ris_uitmelding_selectief (fc42);
 *            IH[hovuit481] = ris_uitmelding_selectief (fc48);
 */

rif_bool ris_uitmelding_selectief(count fc)
{
   register count r = 0;
   rif_bool result = TRUE;

   while (r < RIS_PRIOREQUEST_AP_NUMBER) { /* doorloop alle PrioRequest objecten */
      if (strcmp(RIS_PRIOREQUEST_AP[r].signalGroup, FC_code[fc]) == 0) { /* test op juiste signaalgroep */
         result = FALSE;   /* priorequest is aanwezig voor de signaalgroep */
         if ((RIS_PRIOREQUEST_AP[r].requestType == RIF_PRIORITYREQUESTTYPE_CANCELLATION)  /* cancellation request */
            && (RIS_PRIOREQUEST_EX_AP[r].prioControlState >= RIF_PRIORITIZATIONSTATE_REQUESTED) ) { /* CCA: aanpassing naar >= */
            result = TRUE;
            break;
         }
      }
      r++;
   }
   return ((rif_bool) result);
}



void ris_verstuur_ssm(int prioFcsrm) { /* @@@ CCA */

  /* SSM berichten versturen 
   * 
   * RIF_PRIORITIZATIONSTATE_UNKNOWN           = 0,   Unknown state.
   * RIF_PRIORITIZATIONSTATE_REQUESTED         = 1,   This prioritization request was detected by the traffic controller.
   * RIF_PRIORITIZATIONSTATE_PROCESSING        = 2,   Checking request (request is in queue, other requests are prior).
   * RIF_PRIORITIZATIONSTATE_WATCHOTHERTRAFFIC = 3,   Cannot give full permission, therefore watch for other traffic. Note that other requests may be present.
   * RIF_PRIORITIZATIONSTATE_GRANTED           = 4,   Intervention was successful and now prioritization is active.
   * RIF_PRIORITIZATIONSTATE_REJECTED          = 5,   The prioritization request was rejected by the traffic controller.
   * RIF_PRIORITIZATIONSTATE_MAXPRESENCE       = 6,   The request has exceeded maxPresence time.
   *                                                  Used when the controller has determined that the requester should then back off and request an alternative.
   * RIF_PRIORITIZATIONSTATE_RESERVICELOCKED   = 7    Prior conditions have resulted in a reservice locked event:
   *                                                  the controller requires the passage of time before another similar request will be accepted.
   * 
   * 
   */

   int i, prio;
   int fc;
   prio = prioFcsrm;
   fc = iFC_PRIOix[prio];
   for (i = 0; i < RIS_PRIOREQUEST_AP_NUMBER; i++)
   {
      if (iAantalInmeldingen[prio] > 0) /* prioriteitsvoertuig ingemeld */
      {
         /* prioControlState == 1 --> prioControlState = 7 */ /* TLCgen: blokkeringstijd loopt */
         if  ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 1) && iWachtOpKonflikt[prio] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0)) 
         {
            ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_RESERVICELOCKED);
            RIS_PRIOREQUEST_EX_AP[i].prioControlState = 7;
         } else
            /* prioControlState == 1 --> prioControlState = 5 */ /* TLCgen: ondermaximum */ 
            if  ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 1) && iOnderMaximumVerstreken[prio] && G[fc] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0)) 
            {
               ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
               RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
            } else
               /* prioControlState == 1 --> prioControlState = 5 */ /* TLCgen: geen iPrioriteit */
               if  ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 1) && !iPrioriteit[prio] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0)) 
               {
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
                  RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
               } else
                  /* prioControlState == 1 --> prioControlState = 2 */ 
                  if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 1) && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0)) 
                  {
                     ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_PROCESSING);
                     RIS_PRIOREQUEST_EX_AP[i].prioControlState = 2;
                  }

               /* prioControlState == 2 --> prioControlState = 5 */ /* TLCgen: ondermaximum */
               if  ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 2) && iOnderMaximumVerstreken[prio] && G[fc] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0)) 
               {
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
                  RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
               } else
                  /* prioControlState == 2 --> prioControlState = 5 */ /* TLCgen: geen iPrioriteit */
                  if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 2) && !iPrioriteit[prio] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0))
                  {
                     ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
                     RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
                  } else
                     /* prioControlState == 2 --> prioControlState = 4 */ /* TLCgen: prioriteit en groen */
                     if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 2) && iPrioriteit[prio] && G[fc] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0))
                     {
                        ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_GRANTED);
                        RIS_PRIOREQUEST_EX_AP[i].prioControlState = 4;
                     }

                  /* prioControlState == 4 --> prioControlState = 5 */
                  if  ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 4) && !iPrioriteit[prio] && G[fc] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0)) 
                  {
                     ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
                     RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
                  } 
      }
      else /* geen prioriteitsvoertuig ingemeld */
      {
         /* prioControlState == 2 --> prioControlState = 5 */ /* TLCgen: geen iPrioriteit */
         if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 2) && !iPrioriteit[prio] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0))
         {
            ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
            RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
         } else
            /* prioControlState == 2 --> prioControlState = 6 */ /* TLCgen: groenbewakingstijd actief */
            if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 2) && G[fc] && (iGroenBewakingsTimer[prio] >= iGroenBewakingsTijd[prio]) && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0))
            {
               ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_MAXPRESENCE);
               RIS_PRIOREQUEST_EX_AP[i].prioControlState = 6;
            } else
               /* prioControlState == 4 --> prioControlState = 6 */ /* TLCgen: groenbewakingstijd actief */
               if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 4) && G[fc] && (iGroenBewakingsTimer[prio] >= iGroenBewakingsTijd[prio]) && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0))
               {
                  ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_MAXPRESENCE);
                  RIS_PRIOREQUEST_EX_AP[i].prioControlState = 6; 
               } else
                  /* prioControlState == 4 --> prioControlState = 5 */ /* TLCgen: !iPrioriteit */
                  if ((RIS_PRIOREQUEST_EX_AP[i].prioControlState == 2) && G[fc] && !iPrioriteit[prio] && (strcmp(RIS_PRIOREQUEST_AP[i].signalGroup, FC_code[fc]) == 0))
                  {
                     ris_put_activeprio((RIS_PRIOREQUEST_AP[i].id), (RIS_PRIOREQUEST_AP[i].sequenceNumber), RIF_PRIORITIZATIONSTATE_REJECTED);
                     RIS_PRIOREQUEST_EX_AP[i].prioControlState = 5;
                  }
      }
   }
}

/* KG */ /*CCA@@@*/
/* kg() tests G for the conflicting phasecycles.
 * kg() returns TRUE if an "G[]" is detected, otherwise FALSE.
 * kg() can be used in the function application().
 */

#if !defined (CCOLFUNC)


boolv kg(count i)
{
   register count n, j;

#ifndef NO_GGCONFLICT
   for (n=0; n<GKFC_MAX[i]; n++) {
#else
   for (n=0; n<KFC_MAX[i]; n++) {
#endif
      j=KF_pointer[i][n];
      if (G[j])  return (TRUE);
   }
   return (FALSE);
   }

#endif

void ris_ym(int prioFcsrm, count tym, count tym_max) { /* @@@ CCA */
/* Moet eigengelijk gebueren op basis van ETA. Nu op basis van rijtijd (op ongeveer 250 meter dus onnauwkeurig). Optie om uit te werken op basis van aanwezigheid voertuig x meter voor de stopstreep met granted?
 */

   int i, prio;
   int fc;
   prio = prioFcsrm;
   fc = iFC_PRIOix[prio];
   for (i = 0; i < RIS_PRIOREQUEST_AP_NUMBER; i++)
   {
      IT[tym_max] = (iStartGroen[prio]<=T_max[tym]);
      IT[tym]     = (iStartGroen[prio]<=T_max[tym]);
      if (T[tym] && T[tym_max])                       YM[fc] |= 0x40;
      else                                            YM[fc] &=~0x40;
      }

}
