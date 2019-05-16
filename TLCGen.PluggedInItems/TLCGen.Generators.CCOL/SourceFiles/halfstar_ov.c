/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* CCOL - OV Prioriteit signaalplanstructuur                                              */
/*                                                                                        */
/*                                                                                        */
/* (C) Copyright 2017 Royal HaskoningDHV b.v. All rights reserved.                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* Versie :  2.1                                                                          */
/*           Integratie met uitgebreide OV module CCOL Generator                          */
/* Naam   :  ov_ple.h                                                                     */
/* Datum  :  05-12-2017                                                                   */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */

#include "fcvar.h"      /* fasecycli                                                      */
#include "kfvar.h"      /* conflicten                                                     */
#include "usvar.h"      /* uitgangs elementen                                             */
#include "dpvar.h"      /* detectie elementen                                             */
#include "isvar.h"      /* ingangs elementen                                              */
#include "hevar.h"      /* hulp elementen                                                 */
#include "tmvar.h"      /* tijd elementen                                                 */
#include "ctvar.h"      /* teller elementen                                               */
#include "schvar.h"     /* software schakelaars                                           */
#include "prmvar.h"     /* parameters                                                     */
#include "mevar.h"      /* geheugen elementen                                             */
#include "lwmlvar.h"    /* uitgebreide modulen                                            */
#include "plvar.h"      /* signaalplannen                                                 */
#include "plevar.h"     /* uitgebreide signaalplannen                                     */
#if defined CCOLTIG && !defined NO_TIGMAX
#include "trigvar.h"    /* intergroen variabelen                                          */
#else
#include "tigvar.h"     /* intergroen variabelen                                          */
#endif
#include "cif.inc"      /* CVN definitie                                                  */
#include "control.h"    /* controller interface                                           */
#include "bericht.h"    /* CVN berichten                                                  */
#include "stdfunc.h"    /* standaardfuncties                                              */
#include "rtappl.h"     /* applicatie routines                                            */
#include "ov.h"
#include "halfstar_ov.h"/* declaratie functies                                            */

#if !defined AUTOMAAT || defined VISSIM
#include "xyprintf.h"/* voor debug infowindow                                          */
#include <stdio.h>      /* declaration printf()       */
#endif
/* -------------------------------------------------------------------------------------- */

/* -------------------------------------------------------------------------------------- */
/* definitie globale variabelen                                                           */
/* -------------------------------------------------------------------------------------- */
extern bool HoofdRichting[FCMAX];             /* Array met hoofdrichtingen                       */
extern bool HoofdRichtingTegenhouden[FCMAX];  /* Tegenhouden hoofdrichting (TXC of minimum groen)*/
extern bool HoofdRichtingAfkappenYWPL[FCMAX]; /* Afkappen YW_PL hoofdrichting (na minimum groen) */
extern bool HoofdRichtingAfkappenYVPL[FCMAX]; /* Afkappen YV_PL hoofdrichting (na minimum groen) */
extern int iMinimumGroenUitgesteldeHoofdrichting[FCMAX]; /* minimum groen na uitstlelen hoofdrichting */
extern mulv test_pr_fk_totxb(count i, bool fpr);  /* standaard CCOL functie uit plefunc.c */


/* -------------------------------------------------------------------------------------- */
/* Functie: set_pg_primair_ov_ple()                                                       */
/*                                                                                        */
/* Doel:    set_pg_primair_ov_ple() is een aangepaste versie van de CCOL set_pg_primair() */
/*          functie. De originele functie houdt geen rekening met het uitstellen van      */
/*          realisaties tot het uiterste realisatiemoment voor TXD                        */
/*                                                                                        */
/* Params:  i     Fasecyclus                                                              */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
mulv set_pg_primair_ov_ple(count i)
{
	register count n, k;
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)
	char str[20];
#endif
	mulv to_max, to_tmp;

	if (TRG[i] && !PP[i] && !PG[i] && ((TOTXB_PL[i] > 0) || (TX_PL_timer == TXB_PL[i]))) {
		if (TRG_max[i] - TRG_timer[i] > TOTXB_PL[i]) {
			PG[i] = TRUE;
			return (TRUE);
		}
	}

	if (RV[i] && (BL[i] || !A[i]) && !PP[i] && !PG[i])
	{

		to_max = to_tmp = 0;

#ifdef NO_TIG
		for (n = 0; n < KFC_MAX[i]; n++)
		{
#ifdef CCOLTIG
			k = KF_pointer[i][n];
#else
			k = TO_pointer[i][n];
#endif
#if defined CCOLTIG && !defined NO_TIGMAX
			if (TIG[k][i])                                   /* zoek grootste ontruimingstijd */
			{
				to_tmp = TIG_max[k][i] - TIG_timer[k];
#else
			if (TO[k][i])                                   /* zoek grootste ontruimingstijd */
			{
				to_tmp = TGL_max[k] + TO_max[k][i] - TGL_timer[k] - TO_timer[k];
#endif
				if (to_tmp > to_max)
					to_max = to_tmp;
			}
		}
#else
		for (n = 0; n < FKFC_MAX[i]; n++)
		{
#ifdef CCOLTIG
			k = KF_pointer[i][n];
			if (TRIG_max[k][i] >= 0)
			{
				to_tmp = TRIG_max[k][i] - TRIG_timer[k];
#else
			k = TO_pointer[i][n];
			if (TIG_max[k][i] >= 0)
			{
				to_tmp = TIG_max[k][i] - TIG_timer[k];
#endif
				if (to_tmp > to_max)                           /* zoek grootste ontruimingstijd */
					to_max = to_tmp;
			}
		}
#endif

		if ((RR[i] & OV_PLE_BIT) || (BL[i] & OV_PLE_BIT)) /* uitstel tot "kort" voor TXD moment */
		{
			if ((TOTXB_PL[i] == 0) && (TOTXD_PL[i] > 0) && ((TOTXD_PL[i] - to_max) < 0))
			{
				if (!PG[i])
				{
					PG[i] = TRUE;     /* onvoldoende tijd tot TXD-moment of TXD-moment bereikt? */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)
					if (A[i])
					{
						timetostr(str);
						uber_puts("**** At ");
						uber_puts(str);
						uber_puts(" TX=");
						uber_puti((mulv)(TX_PL_timer));
						uber_puts(" PG[fc");
						uber_puts(FC_code[i]);
						uber_puts("] was set before TXD (");
						uber_puti((mulv)(TXD_PL[i]));
						uber_puts(") passes ***\n");
					}
#endif
				}
				return (TRUE);
			}
		}
		else if (((TOTXB_PL[i] > 0) || (TX_PL_timer == TXB_PL[i])) && ((TOTXB_PL[i] - to_max) < 0) ||
			(TOTXB_PL[i] == 0) && (TX_PL_timer != TXB_PL[i]) && (TX_PL_timer != TXD_PL[i]))
		{
			PG[i] = TRUE;        /* onvoldoende tijd tot TXB-moment of TXB-moment gepasseerd? */
			return (TRUE);
		}
	}
	return (FALSE);
}

/* -------------------------------------------------------------------------------------- */
/* Functie: set_pg_primair_fc_ov_ple()                                                    */
/*                                                                                        */
/* Doel:    set_pg_primair_fc_ov_ple() is een aangepaste versie van de CCOL               */
/*          set_pg_primair_fc() functie. De functie set_pg_primair_fc_ov_ple() roept voor */
/*          alle fasecycli de functie set_pg_primair_ov_ple() aan.                        */
/*                                                                                        */
/* Params:  geen                                                                          */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
void set_pg_primair_fc_ov_ple(void)
{
	register count i;

	for (i = 0; i < FC_MAX; i++)
	{
		set_pg_primair_ov_ple(i);
	}
}

void set_pg_fk_totxb_ov_ple(count i)
{
	register count n, k;

	/* zet PG voor overslaan fictief confl. primaire fasecycli */
	/* ------------------------------------------------------- */
	for (n = 0; n < FKFC_MAX[i]; n++)
	{
#ifdef CCOLTIG
		k = KF_pointer[i][n];
#else
		k = TO_pointer[i][n];
#endif
		if ((((TOTXB_PL[k] > 0) || (TX_PL_timer == TXB_PL[k])) && (TOTXB_PL[i] > TOTXB_PL[k])))
		{
			PG[k] |= PRIMAIR_OVERSLAG;
		}
	}
}

/* -------------------------------------------------------------------------------------- */
/* Functie: set_PRPL_ov_ple()                                                             */
/*                                                                                        */
/* Doel:    set_PRPL_ov_ple() is een aangepaste versie van de CCOL set_PRPL() functie     */
/*          De originele functie houdt geen rekening met het uitstellen van realisaties   */
/*          tot het uiterste realisatiemoment voor TXD                                    */
/*          set_PRPL_ov_ple() bepaalt tijdens de primaire ruimte van een richting of er   */
/*          sprake is van tegehouden tbv OV prioriteit en zet de AA[] en PR[] van een     */
/*          richting die wordt tegengehouden wel op als er een aanvraag is                */
/*                                                                                        */
/* Params:  i     Fasecyclus                                                              */
/*          fpr   voorwaarde versnelde realisatie                                         */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
bool set_PRPL_ov_ple(count i, bool fpr)
{
	mulv result = 0;

	/* reset PR and (re)set PG */
	/* ---------------------- */
#ifdef NO_CHNG20020705
	if (RV[i] && !AA[i] || SRV[i])  PR[i] = FALSE;
#endif
	/* if (PR[i] && (SG[i] || (TX_PL_timer==TXB_PL[i]) && G[i]))  PG[i]= TRUE; */
	if (PR[i] && !PG[i])
	{
		if (SG[i] && (PR[i] & VERSNELD_PRIMAIR))  PG[i] = VERSNELD_PRIMAIR;
		else if (SG[i] || G[i] && ((TX_PL_timer == TXB_PL[i]) || (TOTXB_PL[i] == 0) && (TOTXD_PL[i] > TFG_max[i])))
			PG[i] = PRIMAIR;
	}

	/* overname van AR naar PR */
	/* ----------------------- */
 /* if (!PR[i] && ((TX_PL_timer==TXB_PL[i]) && (RA[i] || G[i])))  {  */
	if (!PR[i] && !PG[i] && G[i] && ((TX_PL_timer == TXB_PL[i]) || (TOTXB_PL[i] == 0) && (TOTXD_PL[i] > TFG_max[i])))
	{
		PR[i] = PRIMAIR;        /* AR[] -> PR[] */
		AR[i] = FALSE;
		/*    PG[i]= TRUE; - eerst PR[] opzetten en volgende systeemronde pas PG[] */
					 /* zorgt ervoor dat RW opstaat voordat PG waar is */
	}
	if (TX_PL_timer == TXD_PL[i])  PG[i] = FALSE;

	/* (versneld) primaire realisatie */
	/* ------------------------------ */
	if (A[i] && RV[i] && !SRV[i] && !AA[i] && !RR_PL[i] && (!RR[i] || (RR[i] & OV_PLE_BIT)) && (!BL[i] || (BL[i] & OV_PLE_BIT))
		&& !PG[i] && !fkaa_primair(i) && ((TOTXB_PL[i] > 0) && !kcv(i)   /* @@ */
			|| (TXB_PL[i] == TX_PL_timer)
			|| ((TOTXB_PL[i] == 0 && TOTXD_PL[i] > TFG_max[i]/* && (RR[i]&OV_PLE_BIT)*/))))  /* DHV CvB toegevoegd ivm uitstellen conflicten door OV */
						   /* 12-9-1998 ivm TXA==TXB @@ */
							/* geen test op !TRG[i] @@ */
							/* 1-1-1999 toegevoegd !SRV[i] */
	{
		switch (result = test_pr_fk_totxb(i, fpr))  /* test fictieve conflicten */
		{
		case PRIMAIR:
			AA[i] |= TRUE;
			PR[i] |= PRIMAIR;
			break;

		case VERSNELD_PRIMAIR:
			AA[i] |= TRUE;
			PR[i] |= VERSNELD_PRIMAIR;
			set_pg_fk_totxb_ov_ple(i);  /* set PG voor fictieve conflicten */
			break;
		}
		if (result)  return ((bool)TRUE);
	}
	return ((bool)FALSE);
}



/* -------------------------------------------------------------------------------------- */
/* Functie: set_pg_primair_fc_ov_ple()                                                    */
/*                                                                                        */
/* Doel:    set_pg_primair_fc_ov_ple() is een aangepaste versie van de CCOL functie       */
/*          set_pg_primair_fc(). De functie set_pg_primair_fc_ov_ple() roept voor         */
/*          alle fasecycli de functie set_pg_primair_ov_ple() aan.                        */
/*                                                                                        */
/* Params:  geen                                                                          */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
void signaalplan_primair_ov_ple(void)
{
	register count i;

	for (i = 0; i < FC_MAX; i++)
	{
		set_PRPL_ov_ple(i, (bool)TRUE);  /* TRUE -> versneld primair */
	}

}

/* -------------------------------------------------------------------------------------- */
/* Functie: OV_ple_init()                                                                 */
/*                                                                                        */
/* Doel:    Initialiseren instellingen t.b.v. ov prioriteit                               */
/*                                                                                        */
/* Params:  Geen                                                                          */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
void OVHalfstarInit(void)
{
	int i;

	for (i = 0; i < FC_MAX; i++)
	{
		TVGPL_max[i] = NG;
		HoofdRichting[i] = FALSE;
		HoofdRichtingTegenhouden[i] = FALSE;
		HoofdRichtingAfkappenYWPL[i] = FALSE;
		HoofdRichtingAfkappenYVPL[i] = FALSE;
	}
}


int OVHalfstarBepaalPrioriteitsOpties(int prm_prio) {
	int p, iReturn;

	for (iReturn = 0, p = PRM[prm_prio] % 10000L;
		p > 0;
		p /= 10L) {
		switch (p % 10) {
		case 1:
			iReturn |= poPLGroenVastHoudenNaTXD;
			break;
		case 2:
			iReturn |= poPLGroenVastHoudenNaTXD;
			iReturn |= poPLTegenhoudenHoofdrichting;
			break;
		case 3:
			iReturn |= poPLGroenVastHoudenNaTXD;
			iReturn |= poPLTegenhoudenHoofdrichting;
			iReturn |= poPLAbsolutePrioriteit;
			break;
		case 4:
			iReturn |= poPLGroenVastHoudenNaTXD;
			iReturn |= poPLTegenhoudenHoofdrichting;
			iReturn |= poPLAbsolutePrioriteit;
			iReturn |= poPLNoodDienst;
			break;
		}
	}
	return iReturn;
}


/* -------------------------------------------------------------------------------------- */
/* Functie: OV_ple_BepaalHoofdrichtingOpties()                                            */
/*                                                                                        */
/* Doel:    Instellingen voor hoofdrichtingen bepalen                                     */
/*                                                                                        */
/* Params:  Lijst met variabel aantal argumenten:                                         */
/*          1. dummy element (nodig voor va_list)                                         */
/*          2. index fc hoofdrichting                                                     */
/*          3. schakelaar tegenhouden hoofdrichting toegestaan                            */
/*          4. schakelaar afkappen YW_PL hoofdrichting toegestaan                         */
/*          5. schakelaar afkappen YV_PL hoofdrichting toegestaan                         */
/*          6. Tijdsduur minimum groen na uitstellen                                      */
/*                                                                                        */
/*          NB: va_list altijd afsluiten met END                                          */
/*                                                                                        */
/* HoofdrichtingOpties(NG, (va_count) fc02, TRUE, FALSE, TRUE, TFG_max[fc02],             */
/*                         (va_count) fc08, TRUE, FALSE, TRUE, TFG_max[fc08],             */
/*                         (va_count) END);                                               */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
void OVHalfstarBepaalHoofdrichtingOpties(int dummy, ...)
{
	va_list argpt;                                     /*  variabele argumentenlijst       */
	count fc;                                          /*  arraynummer fc hoofdrichting    */
	mulv sch_tegen;                                    /*  bool tegenhouden hoofdrichting  */
	mulv sch_afkapywpl;                                /*  bool afkappen hoofdrichting     */
	mulv sch_afkapyvpl;                                /*  bool afkappen hoofdrichting     */
	int t_mingroen;                                    /*  tijdsduur minimum groen uitstel */
	int i;

	for (i = 0; i < FC_MAX; i++)
	{
		HoofdRichting[i] = FALSE;
		HoofdRichtingTegenhouden[i] = FALSE;
		HoofdRichtingAfkappenYWPL[i] = FALSE;
		HoofdRichtingAfkappenYVPL[i] = FALSE;
		iMinimumGroenUitgesteldeHoofdrichting[i] = TFG_max[i];
	}

	va_start(argpt, dummy);                             /*  start var. argumentenlijst      */
	do
	{
		fc = va_arg(argpt, va_count);                    /*  lees array-nummer hoofdrichting */

		if (fc >= 0)
		{
			sch_tegen = va_arg(argpt, va_mulv);      /* lees waarde schakelaar           */
			sch_afkapywpl = va_arg(argpt, va_mulv);      /* lees waarde schakelaar           */
			sch_afkapyvpl = va_arg(argpt, va_mulv);      /* lees waarde schakelaar           */
			t_mingroen = va_arg(argpt, va_mulv);      /* lees waarde tijdsduur            */

			HoofdRichting[fc] = TRUE;
			HoofdRichtingTegenhouden[fc] = (sch_tegen >= END) ? sch_tegen : FALSE;
			HoofdRichtingAfkappenYWPL[fc] = (sch_afkapywpl >= END) ? sch_afkapywpl : FALSE;
			HoofdRichtingAfkappenYVPL[fc] = (sch_afkapyvpl >= END) ? sch_afkapyvpl : FALSE;
			iMinimumGroenUitgesteldeHoofdrichting[fc] = t_mingroen >= END ? t_mingroen : TFG_max[fc];
		}
	} while ((fc >= 0) && (fc < FC_MAX) &&
		(sch_tegen > END) && (sch_afkapywpl > END) && (sch_afkapyvpl > END) && (t_mingroen > END));

	va_end(argpt);                                     /* maak var. arg-lijst leeg         */
}


int BepaalTO(count fcvan, count fcnaar)
{
	int to_max = 0;

#if defined CCOLTIG && !defined NO_TIGMAX
	if (TIG_max[fcvan][fcnaar] >= 0) {                      /* zoek grootste ontruimingstijd      */
		if (TIG[fcvan][fcnaar])
			to_max = TIG_max[fcvan][fcnaar] - TIG_timer[fcvan];
		else
			to_max = TIG_max[fcvan][fcnaar];
}
#else
	if (TO[fcvan][fcnaar]) {                              /* zoek grootste ontruimingstijd      */
		to_max = TGL_max[fcvan] + TO_max[fcvan][fcnaar] - TGL_timer[fcvan] - TO_timer[fcvan];
	}
#endif
	return to_max;
}

bool TXTimerTussen(int start, int eind)
{
	bool result = FALSE;

	start = start % TX_PL_max;
	eind = eind % TX_PL_max;

	if (start < eind)
		result = ((TX_timer >= start) && (TX_timer <= eind));
	else
		result = ((TX_timer < start) && (TX_timer <= eind));

	return result;
}

/* -------------------------------------------------------------------------------------- */
/* Functie: TijdTotLaatsteRealisatieMomentConflict()                                      */
/*                                                                                        */
/* Doel:    Bepalen van de resterende tijd tot het laatst mogelijke realisatiemoment van  */
/*          een conflictrichting, rekening houdend met het ontruimen van de OV richting   */
/*          en het realiseren van tenminste de minimum groentijd (gr_min[])               */
/*                                                                                        */
/* Params:  fc:         OV richting                                                       */
/*          k:          Conflictrichting                                                  */
/*          prio_opties:geldende prioriteitsopties                                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
int TijdTotLaatsteRealisatieMomentConflict(int fcov, int k, int prio_opties)
{
	int totxb_min;

	totxb_min = 30000;

	if (G[k] || (!A[k] && !HoofdRichting[k]))
	{
		/* Conflict is al groen, realisatie kan dus niet uitgesteld worden           */
		/* Of conflict heeft geen aanvraag en is geen hoofdrichting met TXB aanvraag */
		return totxb_min;
	}

	if (!HoofdRichting[k])
	{
		if (prio_opties & poPLAbsolutePrioriteit)
		{
			/* Absolute prioriteit mag niet-hoofdrichtingen altijd uitstellen cq overslaan */
			return totxb_min;
		}
		else
		{
			/* Bij niet-absolute prioriteit mogen niet-hoofdrichtingen altijd worden uitgesteld */
			/* Dit met inachtname van minimum groentijd iAfkapGroen die voor het TXD moment     */
			/* nog gerealiseerd moet worden                                                     */
			if (TOTXD_PL[k] > 0 || (TX_timer == TXD_PL[k]))
			{
				if (G[k] || (TFB_timer[k] > ((TXD_PL[k] - TXB_PL[k] + TX_PL_max) % TX_PL_max)))
					totxb_min = TOTXD_PL[k] - iAfkapGroen[k];
				else
					totxb_min = 30000;
			}
		}
	}
	else // HoofdRichting[k]
	{
		if (HoofdRichtingTegenhouden[k] && (prio_opties & poPLTegenhoudenHoofdrichting))
		{
			if (TOTXD_PL[k] > 0 || (TX_timer == TXD_PL[k]))
			{
				if (G[k] || (TFB_timer[k] > ((TXD_PL[k] - TXB_PL[k] + TX_PL_max) % TX_PL_max)))
					totxb_min = TOTXD_PL[k] - iMinimumGroenUitgesteldeHoofdrichting[k];
				else
					totxb_min = 30000;
			}
		}
		/* Startgroen hoofdrichting mag niet worden uitgesteld */
		else
		{
			if (TOTXB_PL[k] > 0 || TX_timer == TXB_PL[k]/* || TXTimerTussen(TXB_PL[k], TXB_PL[k] + (TFG_max[k]/10))*/)
			{
				totxb_min = TOTXB_PL[k]/* - TO_max[fcov][k] - TGL_max[fcov]*/;
			}
			else if ((TOTXB_PL[k] == 0) && (TOTXD_PL[k] > 0))
				return totxb_min;
		}
	}

	/* Indien OV richting nog gerealiseerd moet worden, rekening houden met groentijd en ontruimingstijd */ /* RvdS Welke TO? na de ov richting of die daarvoor? */
	if (R[fcov])
	{
		totxb_min -= TFG_max[fcov]; /* RvdS: evt aanpassen met extra tijd bovenop TFG */
		if (!CK[fcov])
			totxb_min -= BepaalTO(fcov, k); /* gaat het hier wel goed met deze functie */
	}
	else /* RvdS: (hoofd) richting is groen */
	{
		totxb_min -= (TFG_max[fcov] - TFG_timer[fcov] + BepaalTO(fcov, k)); /* RvdS:  Dit gaat niet goed en zorgt voor te kort uitverlengen */
	}
	totxb_min -= 10; /* marge voor afrondingen en afwikkeling interne fasen */

	return  totxb_min;
}


/* -------------------------------------------------------------------------------------- */
/* Functie: StartGroenConflictenUitstellen()                                              */
/*                                                                                        */
/* Doel:    Bepalen of het startgroen van conflictrichtingen nog uitgesteld kan worden    */
/*          zodat de OV richting bijzonder gerealiseerd kan worden of gedurende de        */
/*          realisatie na het TXD moment groen kan blijven                                */
/*                                                                                        */
/* Params:  fc:         fasecyclus                                                        */
/*          prio_opties:geldende prioriteitsopties                                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
bool StartGroenConflictenUitstellen(count fcov, int prio_opties)
{
	int j, k;
	bool l_fReturn = TRUE;

	for (j = 0; j < FKFC_MAX[fcov]; j++)
	{
#ifdef CCOLTIG
		k = KF_pointer[fcov][j];
#else
		k = TO_pointer[fcov][j];
#endif

		if (TijdTotLaatsteRealisatieMomentConflict(fcov, k, prio_opties) <= 0)
		{
			l_fReturn = FALSE;
			break;
		}
	}

	return l_fReturn;
}

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

	if (IH[hplact])
	{
		int ov, fc, iMaxResterendeGroenTijd;
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
	if (IH[hplact])
	{
		int fc;
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
		int fc;
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
		int ov, fc;
		bool magUitstellen;
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

void OVHalfstarMeetKriterium(void)
{
	if (IH[hplact])
	{
		int ov, fc, iRestGroen;
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