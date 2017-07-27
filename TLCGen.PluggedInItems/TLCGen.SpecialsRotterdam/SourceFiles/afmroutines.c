/* AFM routines */
/* ------------ */

/****************************** Versie commentaar ***********************************
 *
 * Versie	Datum		Naam	Commentaar
 *
 * 1.0.0	13-08-2014	ddo		Eerste versie.
 *
 ************************************************************************************/

#include "AFMRoutines.h"

/* initialisaties tbv globale AFM variabelen */
int AFMfcmax = 0;   /* Aantal fasen dat mee doet met de AFM */
int CIFKLOKMAX = 32767; /* Maximale waarde van CIF_KLOK tbv CIF_KLOK[CIF_SEC_TELLER] (integer (s_int16)) */
int CALGROENTIJD = 300; /* Maximale groentijd bij ontruiming (calamiteit) in seconden; default 5 minuten = 300 seconden */
count tc_start=0,tc_einde_prev=0,tc_tijd[3]={0};

#define AFM_BIT 0x4000 /* AFM bit: bit 14 */

void AFMinit(void)
{
	/* Initialiseert globale AFM waarden (het aantal deelnemende fasen) */

	/* AFMinit wordt éénmalig aangeroepen in post_init_application */
	/* met 'AFMinit();' */
}

void AFM_fc_initfc(AFM_FC_STRUCT * AFM_data_fc, count fc, count prm_fc)
{
	/* Initialiseert de AFM data per deelnemende fase */
//	int MLi;

	/* AFM_fc_initfc wordt per deelnemende fase aangeroepen in post_init_application */
	/* met 'AFM_fc_initfc(&AFM_verwerken_fc##, fc##, prmAFM##_FC);' */

	AFM_data_fc->fc = fc;
	AFM_data_fc->afm_prm = prm_fc;
	AFM_data_fc->tvgproc = 100; 

	/* Opslaan fasenummer van deze fase */
	PRM[prm_fc] = atoi(FC_code[AFM_data_fc->fc]);

	/* Bepaal module voor deze fase */
	//for (MLi=0;MLi<MLMAX;MLi++)
	//{
	//	if (PRML[MLi][AFM_data_fc->fc] & PRIMAIR)	AFM_data_fc->ml = MLi;
	//}



}

void AFMinterface(AFM_FC_STRUCT * AFM_data_fc)
{
	/* Schrijft AFM data uit ccolwaarden naar parameters en leest  */
	/* AFM aansturing (percentage groentijd, prio doseren) uit parameters */

	/* AFMinterface wordt per deelnemende fase aangeroepen in post_application */
	/* met 'AFMinterface(&AFM_verwerken_fc##);' */

	int afmprm;

	/* afmprm = de eerste waarde uit het AFMlist array (= het adres van prmAFM##_FC) */
	afmprm = AFM_data_fc->afm_prm;

	if (TS)	/* Elke seconde worden de huidige waarden weggeschreven */
	{

		if (PRM[afmprm + AFM_TVGMAX] != AFM_data_fc->tvgmax)     PRM[afmprm + AFM_TVGMAX] = AFM_data_fc->tvgmax, AFM_CIF_changed = TRUE;
		if (PRM[afmprm + AFM_TVGACT] != AFM_data_fc->tvg)        PRM[afmprm + AFM_TVGACT] = AFM_data_fc->tvg, AFM_CIF_changed = TRUE;
		if (PRM[afmprm + AFM_TVGGEM] != AFM_data_fc->tvg_gem)    PRM[afmprm + AFM_TVGGEM] = AFM_data_fc->tvg_gem, AFM_CIF_changed = TRUE;
		if (PRM[afmprm + AFM_AFGEKAPT] != AFM_data_fc->afgekapt) PRM[afmprm + AFM_AFGEKAPT] = AFM_data_fc->afgekapt, AFM_CIF_changed = TRUE;
	}

	if (TS)	/* Iedere seconde wordt de AFM aansturing ingelezen */
	{
		AFM_data_fc->tvgproc = PRM[afmprm + AFM_GPROC];
	}

	if (TS) /* Plaats parameterwaarden in de struct */
	{
		AFM_data_fc->mintvgmax = PRM[afmprm + AFM_MINTVGMAX];
		AFM_data_fc->maxtvgmax = PRM[afmprm + AFM_MAXTVGMAX];
	}
}


void AFMdata(AFM_FC_STRUCT * AFM_data_fc)
{
	/* Verzamelt data (groentijden, moduletijden, etc) die nodig is voor AFM. */

	/* AFMdata wordt per deelnemende fase aangeroepen in post_application */
	/* met 'AFMdata(&AFM_verwerken_fc##)' */



		
	/* Bepaal laatst gerealiseerde verlenggroentijd; alleen bij primaire realisatie */
	if (EVG[AFM_data_fc->fc] && PR[AFM_data_fc->fc])			AFM_data_fc->tvg = TVG_timer[AFM_data_fc->fc];

	/* Bepaal aanwezigheid OV ingreep */
	if (EVG[AFM_data_fc->fc] && PR[AFM_data_fc->fc])	
	{
		if (FM[AFM_data_fc->fc] || Z[AFM_data_fc->fc])				    AFM_data_fc->afgekapt = 1;
		else															AFM_data_fc->afgekapt = 0;
	}

	
	/* Bepaal gemiddelden */
	if (EVG[AFM_data_fc->fc] && PR[AFM_data_fc->fc])
	{
		/* Bepaal gemiddelde van de laatste 3 gerealiseerde verlenggroentijden van deze fase */

		if (!Z[AFM_data_fc->fc] && !FM[AFM_data_fc->fc])  /* niet meetellen wanneer richting is afgekapt door OV */
		{
			/* Schuif registers van hogere naar lagere index */
			AFM_data_fc->tvg_tijd[0] = AFM_data_fc->tvg_tijd[1];
			AFM_data_fc->tvg_tijd[1] = AFM_data_fc->tvg_tijd[2];
			AFM_data_fc->tvg_tijd[2] = AFM_data_fc->tvg;
	
			/* Bereken gemiddelde over laatste drie geregistreerde TVG's */
			if ((AFM_data_fc->tvg_tijd[0] != 0) &&
				(AFM_data_fc->tvg_tijd[1] != 0) &&
				(AFM_data_fc->tvg_tijd[2] != 0))
			{
				AFM_data_fc->tvg_gem = (count) (AFM_data_fc->tvg_tijd[0] + 
												AFM_data_fc->tvg_tijd[1] + 
												AFM_data_fc->tvg_tijd[2]) / 3;
			}
		}

	}
}

void AFMakties(AFM_FC_STRUCT * AFM_data_fc, mulv fc_shadow, AFM_FC_STRUCT AFM_data_fcs[AFM_fcmax])
{
	/* Voert akties uit (groentijden) in opdracht van AFM centrale. */

	/* AFMakties wordt per deelnemende fase aangeroepen in post_application */
	/* met 'AFMakties(&AFM_verwerken_fc##)' */

	int AFMfc = 0, afmprm = 0, DeltaSec = 0, DeltaProc = 100;

	/* Bewaar de oorspronkelijke TVG_max tijd */
	AFM_data_fc->tvgmax = TVG_max[AFM_data_fc->fc];

	if (fc_shadow != NG)
	{
		TVG_max[fc_shadow] = TVG_max[AFM_data_fc->fc];
		}

	/* Resetten AFM bits (bit 13, 0x2000) */
	 X[AFM_data_fc->fc] &= ~AFM_BIT;
	 Z[AFM_data_fc->fc] &= ~AFM_BIT;
	RR[AFM_data_fc->fc] &= ~AFM_BIT;

	/* Wanneer de verlenggroentijd door AFM (procentueel) bekort wordt */
	if (AFM_data_fc->tvgproc < 100)
	{
		/* Bij knijpen van groentijd vasthouden in VG (OV) uitschakelen */
		YV[AFM_data_fc->fc] = FALSE;
		/* Bij knijpen van groentijd meeverlengen uitschakelen */
		YM[AFM_data_fc->fc] = FALSE;

		/* Verlenggroentijd verlagen met procentuele waarde */
		TVG_max[AFM_data_fc->fc] = (TVG_max[AFM_data_fc->fc] * AFM_data_fc->tvgproc / 100);
		if (TVG_max[AFM_data_fc->fc]<AFM_data_fc->mintvgmax) TVG_max[AFM_data_fc->fc]=AFM_data_fc->mintvgmax;


		/* Op einde verlenggroentijd groen afkappen en richting rood houden */
		if (EVG[AFM_data_fc->fc])
		{
			Z[AFM_data_fc->fc] |= AFM_BIT;
		}
	}
	
	/* Wanneer de verlenggroentijd door AFM (procentueel) verhoogd wordt zonder behoud cyclustijd */
	if (AFM_data_fc->tvgproc > 100)
	{
		/* Verlenggroentijd verhogen met procentuele waarde */
		TVG_max[AFM_data_fc->fc] = (TVG_max[AFM_data_fc->fc] * AFM_data_fc->tvgproc / 100);
		if (TVG_max[AFM_data_fc->fc]>AFM_data_fc->maxtvgmax) TVG_max[AFM_data_fc->fc]=AFM_data_fc->maxtvgmax;
	}

	if (!(fc_shadow==NG))
	{
		if (G[AFM_data_fc->fc] && PR[AFM_data_fc->fc] && !MG[AFM_data_fc->fc])
		{
			
            if (SG[AFM_data_fc->fc]) A[fc_shadow] |= AFM_BIT;
            PR[fc_shadow] = PR[AFM_data_fc->fc];
            YV[fc_shadow] |= AFM_BIT;                 

/*			A[fc_shadow] = TRUE;
			AA[fc_shadow] = TRUE;
			YM[fc_shadow] |= AFM_BIT; */
			if (SVG[AFM_data_fc->fc] && AFM_data_fc->tvgproc < 100 && PRM[prmAFM_Strikt])
			{
				RW[fc_shadow] |= AFM_BIT;
				MK[fc_shadow] |= AFM_BIT;
			}
			else
			{
				RW[fc_shadow] &= ~AFM_BIT;
			}
		}
		else
		{
			RW[fc_shadow] &= ~AFM_BIT;
			AA[fc_shadow] = FALSE;
			YM[fc_shadow] &= ~AFM_BIT;
			if (!G[fc_shadow]) 
			{
				MK[fc_shadow] &= ~AFM_BIT;
			}
			if (!R[fc_shadow]||TRG[fc_shadow])		
			{
				X[AFM_data_fc->fc] |= AFM_BIT;
			}
		}
	}

	/* Wanneer de verlenggroentijd voor een conflictrichting van deze richting
	   door AFM (procentueel) verhoogd wordt MET behoud cyclustijd */
	
	/* Resetten AFM calamiteitenbits conflicterende richtingen (bit 14, 0x4000) */
	/* zie pre_application */

	if (PRM[prmAFM_Strikt] && (AFM_data_fc->tvgproc == 100))
	{
		DeltaProc = 100;
		for (AFMfc = 0; AFMfc < AFM_fcmax; ++AFMfc)							/* Voor iedere richting die meedoet met AFM */
		{
			if ((AFM_data_fcs[AFMfc].fc != AFM_data_fc->fc) && (TO_max[AFM_data_fcs[AFMfc].fc][AFM_data_fc->fc] != -1)) /* Voor iedere richting die niet deze richting is en er conflicterend mee is */
			{
				afmprm = AFM_data_fcs[AFMfc].afm_prm;						/* afmprm = de eerste waarde uit het AFMlist array (= het adres van prmAFM##_FC) */
	
				if ((PRM[prmAFM_Strikt] == 1) && (PRM[afmprm + AFM_GPROC] > 100))
				{
					DeltaSec = (((PRM[afmprm + AFM_GPROC] - 100) * PRM[afmprm + AFM_TVGMAX] / 100) / (MLMAX - 1));

					if ((100 - (100 * DeltaSec / (AFM_data_fc->tvgmax)))<DeltaProc) 
					{
						DeltaProc = 100 - (100 * DeltaSec / (AFM_data_fc->tvgmax));
					}
			
				
				}
			}
		}
		/* Verlenggroentijd verlagen met procentuele waarde */
		if (DeltaProc > 0)
		{
			TVG_max[AFM_data_fc->fc] = (TVG_max[AFM_data_fc->fc] * DeltaProc / 100);
  

			
		}
		else
		{
			TVG_max[AFM_data_fc->fc] = 0;
		}
	}
}


void AFMakties_alternatieven(AFM_FC_STRUCT * AFM_data_fc)
{
	/* Voert akties uit in opdracht van AFM centrale. */
	/* Bij bekortte groentijd worden alternatieve realisaties niet toegestaan */
	/* OOK NIET van richtingen die in hetzelfde blok zitten */

	/* AFMakties_alternatieven wordt per deelnemende fase aangeroepen in Alternatief_Add */
	/* met 'AFMakties_alternatieven(&AFM_verwerken_fc##)' */
	/* Deze aanroepen dienen als allerLAATSTE in Alternatief_Add te gebeuren */
	
//	int fc;

/*	AG[AFM_data_fc->fc] &= ~BIT13;c*/

	/* Alternatieve realisaties van de richting met minder AFM groen uitschakelen */
	if (AFM_data_fc->tvgproc < 100)		
	{
		PAR[AFM_data_fc->fc]  = FALSE;	/* opzetten gebeurt in reg.c */
/*		AG[AFM_data_fc->fc] |= BIT13; */
		
		/* Alternatief Gerealiseerd opzetten */

		/* Alternatieve realisaties van richtingen in hetzelfde blok uitschakelen */
		//for (fc=0; fc<FCMAX; ++fc)
		//{
		//	if (PRML[AFM_data_fc->ml][fc])			PAR[fc] = FALSE; /* opzetten gebeurt in reg.c */
		//}
	}		
}




void AFMResetBits(void)
{

	/* AFMResetBits wordt éénmalig aangeroepen in pre_application met 'AFMResetBits();' */
	
	int fc;

	/* Resetten AFM calamiteitenbits conflicterende richtingen (bit 14, 0x4000) */
	for (fc=0; fc<FCMAX; ++fc)
	{
		BL[fc] &= ~BIT14;
		RR[fc] &= ~BIT14;
		 Z[fc] &= ~BIT14;
		 Z[fc] &= ~BIT15;
		 A[fc] &= ~BIT14;
		AA[fc] &= ~BIT14;
	}
}




int AFMmonitor_data_pre(int x, int y)
{
	/* Toon de waarden van (interne) AFM data in xyprint venster. */

	/* AFMmonitor wordt per deelnemende fase aangeroepen in post_application */
	/* met 'AFMmonitor(&AFM_verwerken_fc##)' */

	int yret = y;
	xyprintf(x, yret++, "_______________________________________________________________   ");
	xyprintf(x, yret++, "  AFM monitoring data");
	xyprintf(x, yret++, "  PR fc  TVG_m   TVG   TVG_g    Tc   Tc_g   ML  Afgekapt  gproc");
	xyprintf(x, yret++, "________________________________________________________________   ");

	return yret;
}

int AFMmonitor_data_fc(int x, int y, AFM_FC_STRUCT * AFM_data_fc)
{
	int yret = y;

	xyprintf(x, yret++,"  %d %s    %3d   %3d     %3d   %3d    %3d  %3d   %3d  %3d", 
		       (PR[AFM_data_fc->fc] > 0), FC_code[AFM_data_fc->fc], TVG_max[AFM_data_fc->fc], AFM_data_fc->tvg, 
			    AFM_data_fc->tvg_gem, AFM_data_fc->tc, AFM_data_fc->tc_gem, /*AFM_data_fc->ml + 1,*/ AFM_data_fc->afgekapt, 
				AFM_data_fc->tvgproc);

	return yret;
}

int AFMmonitor_akties_pre(int x, int y)
{
	int yret = y;

	xyprintf(x, yret++, "_______________________________________________________________   ");
	xyprintf(x, yret++, "  AFM monitoring akties");
	xyprintf(x, yret++, "  fc  TVG_max   TVG_proc   prio    timer    Gtijd     ml    Yml");
	xyprintf(x, yret++, "_______________________________________________________________   ");

	return yret;
}

int AFMmonitor_akties_fc(int x, int y, AFM_FC_STRUCT * AFM_data_fc)
{
	int yret = y;

	/* @@@@ ALLEEN voor xyprintf scherm */

	/* @@@@ einde ALLEEN voor xyprintf scherm */

	xyprintf(x, yret++, "  %s     %3d       %3d     %3d     %3d      %3d", FC_code[AFM_data_fc->fc],
		TVG_max[AFM_data_fc->fc], AFM_data_fc->tvgproc,TVG_timer[AFM_data_fc->fc], 
		TVG_max[AFM_data_fc->fc]);

	return yret;
}



void AFMinterfacemonitor(AFM_FC_STRUCT afmfcstructs[AFM_fcmax])
{
	/* Toon de waarden van (externe) AFM data (parameters op de interface) in xyprint venster. */

	/* AFMinterfacemonitor wordt éénmalig aangeroepen in pre_system_application (zodat de */
	/* regelstatus ook tijdens knipperen wordt doorgegeven) met 'AFMinterfacemonitor();' */
	
	int afmprm, AFMfc;

	xyprintf(2, 6,                    "_______________________________________________________________   ");
	xyprintf(2, 7,                    "  AFM interface                                   |   Akties");
	xyprintf(2, 8,                    "  fc  TVG_m   TVG   TVG_g   Tc  Tc_g   OV  Tml_g  |   !     %%");
	xyprintf(2, 9,                    "_______________________________________________________________   ");	

	
	for (AFMfc=0; AFMfc<AFMfcmax; ++AFMfc)
	{
		afmprm = afmfcstructs[AFMfc].afm_prm;

		xyprintf(2, 10+AFMfc         , "  %3d    %3d   %3d    %3d   %3d   %3d  %3d   %3d   -   %d    %3d", 
			PRM[afmprm + AFM_FC    ], 
			PRM[afmprm + AFM_TVGMAX], 
			PRM[afmprm + AFM_MINTVGMAX], 
			PRM[afmprm + AFM_MAXTVGMAX], 
			PRM[afmprm + AFM_TVGACT], 
			PRM[afmprm + AFM_TVGGEM], 
			PRM[afmprm + AFM_AFGEKAPT], 
			PRM[afmprm + AFM_GPROC],
			PRM[afmprm + AFM_QLENGHT]);
	}	
	
	/* Doorgeven regelstatus naar Matlab via AFM connector */
#ifdef AFMCONNECTOR
	AfmSchrijfRegelStatus(AfmVriNummer, CIF_WPS[CIF_PROG_STATUS]);
#endif

#ifndef AUTOMAAT
	/* Afbeelden programmastatus in xyprintf scherm */ 
	switch (CIF_WPS[CIF_PROG_STATUS])
	{
		case 5:	xyprintf(40, 6, " REGELEN"); break;
		case 4:	xyprintf(40, 6, " ALLES ROOD"); break;
		case 3:	xyprintf(40, 6, " INSCHAKELEN"); break;
		case 2:	xyprintf(40, 6, " KNIPPEREN"); break;
		case 1:	xyprintf(40, 6, " GEDOOFD"); break;
		case 0:	xyprintf(40, 6, " UNDEFINED"); break;
 	}
#endif

	/* doorgeven aan regeltoestel dat parameters zijn gewijzigd */
	/* (is noodzakelijk bij Siemens regelautomaten) */
	CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;

}
void AFM_tc(count prm_tc,count prm_tcgem)
{
	/* Start meting cyclustijd op start ML2 */
	if (SML && (ML == ML2))
	{			
		tc_start = CIF_KLOK[CIF_SEC_TELLER];
	
		/* Bepaal laatst gerealiseerde cyclustijd; Tc groter dan of gelijk aan fasebewakingstijd wordt genegeerd */
		if ((tc_start > tc_einde_prev) && (tc_einde_prev!=0) && ((tc_start - tc_einde_prev) < PRM[prmfb])) 
		{
		PRM[prm_tc]	 = tc_start - tc_einde_prev;
					/* Schuif registers van hogere naar lagere index */
		tc_tijd[0] = tc_tijd[1];
		tc_tijd[1] = tc_tijd[2];
		tc_tijd[2] = PRM[prm_tc];
		/* Bepaal gemiddelde van de laatste 3 cyclustijden */
		if ((tc_tijd[0] != 0) &&
			 (tc_tijd[1] != 0) &&
			 (tc_tijd[2] != 0))
		{
			PRM[prm_tcgem] =  (count) (tc_tijd[0] + 
									   tc_tijd[1] + 
									   tc_tijd[2]) / 3;
		}


		}
		tc_einde_prev = tc_start;
	
	
		

	}
}
