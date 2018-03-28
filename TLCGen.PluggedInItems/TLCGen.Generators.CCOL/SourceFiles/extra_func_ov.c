#include "extra_func_ov.h"

/*
  * Functie: DSI_melding
  *
  * Abstract: afhandeling DSI melding
  *
  * Argumenten:
  * lusnr              = (virtuele) lusnummer
  * fcnr               = signaalgroep
  * inuit              = type melding
  * status             = status melding (bv vertrekmelding)
  * lengte             = voertuiglengte (0 = enkele tram; 1 = dubbele tram)
  * vcu_kar            = terugmelding bij een KAR bericht
  *
  * Voor alle argumenten geldt: -1 = don’t care. Er wordt dan niet gecheckt op
  *                                  dat item.
  *
  * Andere zaken die de functie beinvloeden:
  * SCH[schdsi]        = uitschakelen DSI ingreep
  * SCH[schdsi_test]   = info dsi-buffer tonen in de CCOL-terminal als er
  *                      een match is
  *
  * Decisions: Returns TRUE  als er een match is. Anders FALSE.
  *
  */
bool DSI_melding(count lusnr,
                 count fcnr,
                 count inuit,
                 count status,
                 count lengte,
                 count vcu_kar)
{

	bool match = TRUE; /* Ga uit van een match. Als er iets niet klopt,
						  zetten we de bool op FALSE. */

	/* Helemaal geen DSI; ook niets op de CCOL-terminal. */
//	if (SCH[schdsi] == 0)
//	{
//		return FALSE;
//	}

	if (CIF_DSIWIJZ)
	{
		if (lusnr != -1)
		{
			if (lusnr != CIF_DSI[CIF_DSI_LUS]) match = FALSE;
		}
		if (fcnr != -1)
		{
			if (atoi(FC_code[fcnr]) != CIF_DSI[CIF_DSI_DIR]) match = FALSE;
		}
		if (inuit != -1)
		{
			if (inuit != CIF_DSI[CIF_DSI_TYPE]) match = FALSE;
		}
		if (status != -1)
		{
			if (status != CIF_DSI[CIF_DSI_STAT]) match = FALSE;
		}
		if (lengte != -1)
		{
			if (lengte != CIF_DSI[CIF_DSI_LEN]) match = FALSE;
		}
		if (vcu_kar != -1)
		{
			/* Maak onderscheid tussen VECOM en KAR.
			 * We houden aan dat als er een GPS-locatie beschikbaar
			 * is, het gaan om een KAR-bericht, zo niet, is het
			 * een VECOM-bericht. */
//			if ((vcu_kar == DSI_KAR) && (CIF_DSI[CIF_DSI_XGRAD] == 0))
//			{
//				match = FALSE;
//			}
//			if ((vcu_kar == DSI_VCU) && (CIF_DSI[CIF_DSI_XGRAD] != 0))
//			{
//				match = FALSE;
//			}
		}	 	
	}
	else
	{
		/* Bij geen wijziging in het DSI-buffer is er nooit een match. */
		match = FALSE;
	}

	/* Tonen van lusinfo op de CCOL-terminal.
	* Alleen als er een nieuw bericht is. */
//	if (CIF_DSIWIJZ && !cif_dsiwijz_geprint && SCH[schdsi_test])
//	{
//		_DSI_print();
//	}
//	if (CIF_DSIWIJZ && !cif_accept_geprint && match  && SCH[schdsi_test])
//	{
//      uber_puts("DSI-melding geaccepteerd door programma.\n");
//      cif_accept_geprint=1;
//    }

	/* Houd bij of deze wijziging al geprint is. Als er in 1 systeemronde
	 * meerdere keren de inhouf van het buffer wordt opgevraagd, dan printen
	 * we maar 1 keer. */
//	cif_dsiwijz_geprint=CIF_DSIWIJZ;
//
//    if (!CIF_DSIWIJZ)
//    {
//      cif_accept_geprint=0;
//    }

	return match;
}

bool OVmelding_KAR_V2(count vtgtype,  /*  1. voertuigtype (CIF_BUS CIF_TRAM CIF_BRA) etc  */
	count dir,                  /*  2. fc nummer of richtingnummer (201, 202, 203)  */
	s_int16 stp,                /*  3. stiptheidsklasse                             */
	count meldingtype,          /*  4. type melding (CIF_DSIN / CIF_DSUIT)          */
	bool cprio,                 /*  5. alleen inmelden bij geconditioneerde prio    */
	s_int16 laatcrit,           /*  6. aantal seconden waarna bus te laat is        */
	count lijnparm,             /*  7. eerste lijnparameter (prmov##_allelijnen)    */
	count bufmax,               /*  8. max aantal lijnen in buffer                  */
struct prevovkar * prevOV,  /*  9. opslag data laatste DSI bericht              */
	count tdh)                  /* 10. hiaat timer tbv voorkomen dubbele melding    */
{
	bool check_lijn = FALSE;
	count index;
	bool isov = FALSE;

	/* Check op eerder bericht voor hetzelfde voertuig */
	if ((T[tdh] || RT[tdh]) &&
		CIF_DSI[CIF_DSI_LYN] == prevOV->prevlijn &&
		CIF_DSI[CIF_DSI_TYPE] == prevOV->prevtype &&
		CIF_DSI[CIF_DSI_VTG] == prevOV->prevvtg &&
		CIF_DSI[CIF_DSI_DIR] == prevOV->prevdir)
		return 0;
	RT[tdh] = FALSE;

	/* Kijken of lijnnummer overeenkomt met een parameter voor deze richting */
	if (CIF_DSI[CIF_DSI_LYN] != 0)
	{
		for (index = 0; index < bufmax; ++index)
		{
			check_lijn = (CIF_DSI[CIF_DSI_LYN] == PRM[lijnparm + 1 + index]);
			if (check_lijn)
				break;
		}
	}

	if ((PRM[lijnparm] == 1 || check_lijn) &&  /* lijnnummer juist voor deze richting? */
		(CIF_DSI[CIF_DSI_VTG] == vtgtype) &&  /* juiste voertuigtype? */
		(CIF_DSI[CIF_DSI_DIR] == dir) &&  /* geldt deze melding voor deze richting? */
		(!cprio ||              /* staat geconditioneerde prio uit?
								/* zo nee, behoeft het voertuig prioriteit irt ingestelde stiptheid? */
								(((stp > 0) && (CIF_DSI[CIF_DSI_STP] <= stp)) ||/* enerzijds vanwege de mate van stiptheid, */
								(CIF_DSI[CIF_DSI_TSTP] > laatcrit))) &&  /* of anders omdat de bus meer dan laatcrit seconden vertraagd? */
								(CIF_DSI[CIF_DSI_TYPE] == meldingtype)      /* is dit een in of een uitmelding? */
#if !defined (VISSIM) && DSMAX
								&& DS_MSG
#endif
								)
								isov = TRUE;

	/* Opslaan huidige waarden en herstart hiaat timer */
	if (isov)
	{
		RT[tdh] = TRUE;
		prevOV->prevlijn = CIF_DSI[CIF_DSI_LYN];
		prevOV->prevtype = CIF_DSI[CIF_DSI_TYPE];
		prevOV->prevvtg = CIF_DSI[CIF_DSI_VTG];
		prevOV->prevdir = CIF_DSI[CIF_DSI_DIR];
	}

	return isov;
}

bool HDmelding_KAR_V1(count vtgtype,  /*  1. voertuigtype (CIF_BUS CIF_TRAM CIF_BRA) etc  */
	count prio,                 /*  2. Voert voertuig SIRENE */
	count dir,                  /*  3. fc nummer of richtingnummer (201, 202, 203)  */
	count meldingtype,          /*  5. type melding (CIF_DSIN / CIF_DSUIT)          */
struct prevovkar * prevOV,  /* 10. opslag data laatste DSI bericht              */
	count tdh)                  /* 11. hiaat timer tbv voorkomen dubbele melding    */
{
	bool check_lijn = FALSE;
	bool isov = FALSE;

	/* Check op eerder bericht voor hetzelfde voertuig */
	if ((T[tdh] || RT[tdh]) &&
		CIF_DSI[CIF_DSI_TYPE] == prevOV->prevtype &&
		CIF_DSI[CIF_DSI_VTG] == prevOV->prevvtg &&
		CIF_DSI[CIF_DSI_DIR] == prevOV->prevdir)
		return 0;
	RT[tdh] = FALSE;

	/* Kijken of lijnnummer overeenkomt met een parameter voor deze richting */

	if ((CIF_DSI[CIF_DSI_VTG] == vtgtype) &&  /* juiste voertuigtype? */
		(CIF_DSI[CIF_DSI_PRI] == prio) &&
		(CIF_DSI[CIF_DSI_DIR] == dir) &&  /* geldt deze melding voor deze richting? */
		(CIF_DSI[CIF_DSI_TYPE] == meldingtype)      /* is dit een in of een uitmelding? */
#if !defined (VISSIM) && DSMAX
		&& DS_MSG
#endif
		)                          isov = TRUE;

	/* Opslaan huidige waarden en herstart hiaat timer */
	if (isov)
	{
		RT[tdh] = TRUE;
		prevOV->prevlijn = CIF_DSI[CIF_DSI_LYN];
		prevOV->prevtype = CIF_DSI[CIF_DSI_TYPE];
		prevOV->prevvtg = CIF_DSI[CIF_DSI_VTG];
		prevOV->prevdir = CIF_DSI[CIF_DSI_DIR];
	}

	return isov;
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
void set_DSI_message_KAR(s_int16 vtg, s_int16 dir, count type, s_int16 stiptheid, s_int16 aantalsecvertr, s_int16 PRM_lijnnr, s_int16 prio)
{
	CIF_DSI[CIF_DSI_VTG] = (vtg < 0 ? 0 : vtg);
	CIF_DSI[CIF_DSI_DIR] = (dir < 0 ? 0 : dir);
	CIF_DSI[CIF_DSI_TYPE] = (type < 0 ? 0 : type);
	CIF_DSI[CIF_DSI_LYN] = (PRM_lijnnr < 0 ? 0 : PRM_lijnnr);
	CIF_DSI[CIF_DSI_STP] = (stiptheid < 0 ? 0 : stiptheid);
	CIF_DSI[CIF_DSI_TSTP] = aantalsecvertr;
	CIF_DSI[CIF_DSI_PRI] = (prio < 0 ? 0 : prio);
	/* zet wijzig vlag
	*/
	CIF_DSIWIJZ = 1;
}

/*  de functie set_DSI-message()
-   voor het plaatsen van een selectief detectiebericht in de DSI-buffer.
-   geeft alle variabelen de juiste waarde en zet de wijzigvlag CIF_DSIWIJZ op.
-   dient in de testomgeving te worden aangeroepen vanuit de is_special_signals().
*/
void set_DSI_message(mulv ds, mulv vtg, mulv melding, mulv PRM_lijnnr, mulv stiptheid)
{
	CIF_DSI[CIF_DSI_LUS] = (s_int16)(ds < 0 ? 0 : ds);
	CIF_DSI[CIF_DSI_VTG] = (s_int16)(vtg < 0 ? 0 : vtg);
	CIF_DSI[CIF_DSI_TYPE] = (s_int16)(melding < 0 ? 0 : melding);
	CIF_DSI[CIF_DSI_LYN] = (s_int16)(PRM_lijnnr < 0 ? 0 : PRM_lijnnr);
	CIF_DSI[CIF_DSI_STP] = (s_int16)(stiptheid < 0 ? 0 : stiptheid);

	/* zet wijzig vlag
	*/
	CIF_DSIWIJZ = 1;
}
#endif

bool OVmelding_DSI_BUS(
	count seldet1,              /* eerste selectieve detectielus            */
	count seldet2,              /* tweede selectieve detectielus (of NG)    */
	count seldet3,              /* derde selectieve detectielus (of NG)     */
	count lijnparm,             /* eerste lijnparameter (prmov##_allelijnen)*/
	count bufmax)               /* max aantal lijnen in buffer              */
	/*  Aanroep voor fc02:
	IH[hsd_02in]= OVmelding_DSI(ds02_1a, ds02_1b, NG, prmov02_allelijnen, 10);
	*/
{
	bool check_lijn = FALSE;
	int  index;
	if (CIF_DSI[CIF_DSI_LYN] != 0)
	{
		for (index = 0; index<bufmax; index++)    /* bufmax = aantal lijn-parameters in tab.c */
		{
			check_lijn = (CIF_DSI[CIF_DSI_LYN] == PRM[lijnparm + 1 + index]);
			if (check_lijn == TRUE) break;
		}
	}

	if (((CIF_DSI[CIF_DSI_LUS] == seldet1) ||
		(CIF_DSI[CIF_DSI_LUS] == seldet2) ||
		(CIF_DSI[CIF_DSI_LUS] == seldet3))
#ifndef VISSIM
		&& DS_MSG
#endif
		&& (((CIF_DSI[CIF_DSI_TYPE] == CIF_DSIN) || !SCH[schcheckopdsin]) && CIF_DSI[CIF_DSI_VTG] == CIF_BUS)
		&& ((PRM[lijnparm] == 1) || check_lijn))              return TRUE;
	else                                                        return FALSE;
}

bool OVmelding_DSI_TRAM(
	count seldet1,              /* eerste selectieve detectielus            */
	count seldet2,              /* tweede selectieve detectielus (of NG)    */
	count seldet3,              /* derde selectieve detectielus (of NG)     */
	count lijnparm,             /* eerste lijnparameter (prmov##_allelijnen)*/
	count bufmax)               /* max aantal lijnen in buffer              */
	/*  Aanroep voor fc02:
	IH[hsd_02in]= OVmelding_DSI(ds02_1a, ds02_1b, NG, prmov02_allelijnen, 10);
	*/
{
	bool check_lijn = FALSE;
	int  index;
	if (CIF_DSI[CIF_DSI_LYN] != 0)
	{
		for (index = 0; index<bufmax; index++)    /* bufmax = aantal lijn-parameters in tab.c */
		{
			check_lijn = (CIF_DSI[CIF_DSI_LYN] == PRM[lijnparm + 1 + index]);
			if (check_lijn == TRUE) break;
		}
	}

	if (((CIF_DSI[CIF_DSI_LUS] == seldet1) ||
		(CIF_DSI[CIF_DSI_LUS] == seldet2) ||
		(CIF_DSI[CIF_DSI_LUS] == seldet3))
#ifndef VISSIM
		&& DS_MSG
#endif
		&& (((CIF_DSI[CIF_DSI_TYPE] == CIF_DSIN) || !SCH[schcheckopdsin]) && CIF_DSI[CIF_DSI_VTG] == CIF_TRAM)
		&& ((PRM[lijnparm] == 1) || check_lijn))              return TRUE;
	else                                                        return FALSE;
}