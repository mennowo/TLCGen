/* AFM routines */
/* ------------ */

/****************************** Versie commentaar ***********************************
*
* Versie	Datum		Naam	Commentaar
*
* 1	13-08-2014	ddo		Eerste versie.
* 2	15-02-2017  psn		Basisversie voor vissim
* 3	29-03-2017  psn		Aangepast basisversie voor vissim
* 4	04-04-2017  psn		Naamgeving aangepast
* 5	16-05-2017  psn		Diverse wijzigingen
************************************************************************************/

#include "AFMRoutines.h"

/* initialisaties tbv globale AFM variabelen */
int CIFKLOKMAX = 32767; /* Maximale waarde van CIF_KLOK tbv CIF_KLOK[CIF_SEC_TELLER] (integer (s_int16)) */
int CALGROENTIJD = 300; /* Maximale groentijd bij ontruiming (calamiteit) in seconden; default 5 minuten = 300 seconden */
count tc_start = 0, tc_einde_prev = 0, tc_tijd[3] = { 0 };

void AFMinit(void)
{
    /* Initialiseert globale AFM waarden (het aantal deelnemende fasen) */

    /* AFMinit wordt éénmalig aangeroepen in post_init_application */
    /* met 'AFMinit();' */
}

void AFM_fc_initfc(AFM_FC_STRUCT * AFM_data_fc, count fc, count prm_fc)
{
    /* Initialiseert de AFM data per deelnemende fase */

    /* AFM_fc_initfc wordt per deelnemende fase aangeroepen in post_init_application */
    /* met 'AFM_fc_initfc(&AFM_verwerken_fc##, fc##, prmAFM##_FC);' */

    AFM_data_fc->fc = fc;
    AFM_data_fc->afm_prm = prm_fc;
    AFM_data_fc->max_gmax = 20;

    /* Opslaan fasenummer van deze fase */
    PRM[prm_fc] = atoi(FC_code[AFM_data_fc->fc]);

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

        PRM[afmprm + AFM_GMAXCCOL] = AFM_data_fc->gmax_ccol;
        PRM[afmprm + AFM_GMAXACT] = AFM_data_fc->gact;
        PRM[afmprm + AFM_GMAXGEM] = AFM_data_fc->gmax_gem;
        PRM[afmprm + AFM_AFGEKAPT] = AFM_data_fc->afgekapt;
    }

    if (TS)	/* Iedere seconde wordt de AFM aansturing ingelezen */
    {
        AFM_data_fc->sturing_afm = PRM[afmprm + AFM_STURING];
        AFM_data_fc->gmax_afm = PRM[afmprm + AFM_GMAXAFM];
    }

    if (TS) /* Plaats parameterwaarden in de struct */
    {
        AFM_data_fc->min_gmax = PRM[afmprm + AFM_GMAXMIN];
        AFM_data_fc->max_gmax = PRM[afmprm + AFM_GMAXMAX];
    }

}


void AFMdata(AFM_FC_STRUCT * AFM_data_fc)
{
    /* Verzamelt data (groentijden, moduletijden, etc) die nodig is voor AFM. */

    /* AFMdata wordt per deelnemende fase aangeroepen in post_application */
    /* met 'AFMdata(&AFM_verwerken_fc##)' */




    /* Bepaal laatst gerealiseerde verlenggroentijd; alleen bij primaire realisatie */
    if (EVG[AFM_data_fc->fc] && PR[AFM_data_fc->fc])			AFM_data_fc->gact = (TVG_timer[AFM_data_fc->fc] + TFG_max[AFM_data_fc->fc]) / 10;

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
            AFM_data_fc->gmax_tijd[0] = AFM_data_fc->gmax_tijd[1];
            AFM_data_fc->gmax_tijd[1] = AFM_data_fc->gmax_tijd[2];
            AFM_data_fc->gmax_tijd[2] = AFM_data_fc->gact;

            /* Bereken gemiddelde over laatste drie geregistreerde TVG's */
            if ((AFM_data_fc->gmax_tijd[0] != 0) &&
                (AFM_data_fc->gmax_tijd[1] != 0) &&
                (AFM_data_fc->gmax_tijd[2] != 0))
            {
                AFM_data_fc->gmax_gem = (count)(AFM_data_fc->gmax_tijd[0] +
                    AFM_data_fc->gmax_tijd[1] +
                    AFM_data_fc->gmax_tijd[2]) / 3;
            }
        }

    }
}

void AFMacties(AFM_FC_STRUCT * AFM_data_fc, mulv fc_shadow, AFM_FC_STRUCT AFM_data_fcs[AFM_fcmax])
{
    /* Voert acties uit (groentijden) in opdracht van AFM centrale. */

    /* AFMacties wordt per deelnemende fase aangeroepen in post_application */
    /* met 'AFMacties(&AFM_verwerken_fc##)' */

    int AFMfc = 0, afmprm = 0, DeltaSec = 0, DeltaProc = 100;

    /* Bewaar de oorspronkelijke TVG_max tijd */
    AFM_data_fc->gmax_ccol = (TFG_max[AFM_data_fc->fc] + TVG_max[AFM_data_fc->fc]) / 10;

    if (fc_shadow != NG)
    {
        TVG_max[fc_shadow] = TVG_max[AFM_data_fc->fc];
    }

    /* Wanneer de verlenggroentijd door AFM bekort wordt */
    if ((AFM_data_fc->gmax_afm  < AFM_data_fc->gmax_ccol) && (AFM_data_fc->gmax_afm != 0) && (AFM_data_fc->sturing_afm != 0) && T[tAFMLeven])
    {
        /* Bij knijpen van groentijd vasthouden in VG (OV) uitschakelen */
        YV[AFM_data_fc->fc] = FALSE;
        /* Bij knijpen van groentijd meeverlengen uitschakelen */
        YM[AFM_data_fc->fc] = FALSE;

        /* Verlenggroentijd verlagen met procentuele waarde */
        if (AFM_data_fc->gmax_afm > TFG_max[AFM_data_fc->fc] / 10)
        {
            TVG_max[AFM_data_fc->fc] = 10 * AFM_data_fc->gmax_afm - TFG_max[AFM_data_fc->fc];
        }
        else
        {
            TVG_max[AFM_data_fc->fc] = 0;
        }

        if (TVG_max[AFM_data_fc->fc] + TFG_max[AFM_data_fc->fc]<(AFM_data_fc->min_gmax * 10)) TVG_max[AFM_data_fc->fc] = AFM_data_fc->min_gmax * 10 - TFG_max[AFM_data_fc->fc];


        /* Op einde verlenggroentijd groen afkappen en richting rood houden */
        if (EVG[AFM_data_fc->fc])
        {
            Z[AFM_data_fc->fc] |= BIT13;
        }
    }

    /* Wanneer de verlenggroentijd door AFM (procentueel) verhoogd wordt zonder behoud cyclustijd */
    if ((AFM_data_fc->gmax_afm  > AFM_data_fc->gmax_ccol) && (AFM_data_fc->gmax_afm != 0) && (AFM_data_fc->sturing_afm != 0))
    {
        /* Verlenggroentijd verhogen met procentuele waarde */
        if (10 * AFM_data_fc->gmax_afm > TFG_max[AFM_data_fc->fc])
        {
            TVG_max[AFM_data_fc->fc] = 10 * AFM_data_fc->gmax_afm - TFG_max[AFM_data_fc->fc];
        }
        else
        {
            TVG_max[AFM_data_fc->fc] = 0;
        }
        if ((TVG_max[AFM_data_fc->fc] + TFG_max[AFM_data_fc->fc])>10 * AFM_data_fc->max_gmax) TVG_max[AFM_data_fc->fc] = AFM_data_fc->max_gmax * 10 - TFG_max[AFM_data_fc->fc];
    }

    if (!(fc_shadow == NG))
    {
        if (G[AFM_data_fc->fc] && PR[AFM_data_fc->fc] && !MG[AFM_data_fc->fc])
        {
            if (SG[AFM_data_fc->fc]) A[fc_shadow] |= BIT13;
            /* AA[fc_shadow] |= BIT13; */
            PR[fc_shadow] = PR[AFM_data_fc->fc];
            YV[fc_shadow] |= BIT13;
        }
        else
        {
            if (!R[fc_shadow] || TRG[fc_shadow])
            {
                X[AFM_data_fc->fc] |= BIT13;
            }
        }
        if (G[fc_shadow] && (AFM_data_fc->gmax_afm  < AFM_data_fc->gmax_ccol) && (AFM_data_fc->gmax_afm != 0) && PRM[prmAFM_Strikt] && (AFM_data_fc->sturing_afm != 0))
        {
            MK[fc_shadow] |= BIT13;
        }
    }

    /* Wanneer de verlenggroentijd voor een conflictrichting van deze richting
    door AFM (procentueel) verhoogd wordt MET behoud cyclustijd */

    /* Resetten AFM calamiteitenbits conflicterende richtingen (bit 14, 0x4000) */
    /* zie pre_application */

    if (PRM[prmAFM_Strikt] && (AFM_data_fc->gmax_afm == 0))
    {
        DeltaSec = 0;
        for (AFMfc = 0; AFMfc < AFM_fcmax; ++AFMfc)							/* Voor iedere richting die meedoet met AFM */
        {
            if ((AFM_data_fcs[AFMfc].fc != AFM_data_fc->fc) && (TO_max[AFM_data_fcs[AFMfc].fc][AFM_data_fc->fc] != -1)) /* Voor iedere richting die niet deze richting is en er conflicterend mee is */
            {
                afmprm = AFM_data_fcs[AFMfc].afm_prm;						/* afmprm = de eerste waarde uit het AFMlist array (= het adres van prmAFM##_FC) */

                if ((PRM[afmprm + AFM_GMAXAFM] > PRM[afmprm + AFM_GMAXCCOL]) && PRM[afmprm + AFM_STURING])
                {
                    if (MLMAX>1)
                    {

                        if (PRM[afmprm + AFM_GMAXAFM] < PRM[afmprm + AFM_GMAXMAX])
                        {
                            if (((PRM[afmprm + AFM_GMAXAFM] - PRM[afmprm + AFM_GMAXCCOL]) / (MLMAX - 1))> DeltaSec)
                            {
                                DeltaSec = (PRM[afmprm + AFM_GMAXAFM] - PRM[afmprm + AFM_GMAXCCOL]) / (MLMAX - 1);
                            }
                        }
                        else
                        {
                            if (((PRM[afmprm + AFM_GMAXMAX] - PRM[afmprm + AFM_GMAXCCOL]) / (MLMAX - 1))>DeltaSec)
                            {
                                DeltaSec = (PRM[afmprm + AFM_GMAXAFM] - PRM[afmprm + AFM_GMAXCCOL]) / (MLMAX - 1);
                            }
                        }

                    }
                    else
                    {
                        DeltaSec = 0;
                    }
                }
            }
        }
        /* Verlenggroentijd verlagen */
        if ((10 * DeltaSec) < TVG_max[AFM_data_fc->fc])
        {
            TVG_max[AFM_data_fc->fc] = TVG_max[AFM_data_fc->fc] - 10 * DeltaSec;

        }
        else
        {
            TVG_max[AFM_data_fc->fc] = 0;
        }
    }
}


void AFMacties_alternatieven(AFM_FC_STRUCT * AFM_data_fc)
{
    /* Voert acties uit in opdracht van AFM centrale. */
    /* Bij bekortte groentijd worden alternatieve realisaties niet toegestaan */
    /* OOK NIET van richtingen die in hetzelfde blok zitten */

    /* AFMacties_alternatieven wordt per deelnemende fase aangeroepen in Alternatief_Add */
    /* met 'AFMacties_alternatieven(&AFM_verwerken_fc##)' */
    /* Deze aanroepen dienen als allerLAATSTE in Alternatief_Add te gebeuren */



    /* Alternatieve realisaties van de richting met minder AFM groen uitschakelen */
    if ((AFM_data_fc->gmax_afm  < AFM_data_fc->gmax_ccol) && (AFM_data_fc->gmax_afm != 0) && (AFM_data_fc->sturing_afm != 0))
    {
        PAR[AFM_data_fc->fc] = FALSE;	/* opzetten gebeurt in reg.c */

    }
}




void AFMResetBits(void)
{

    /* AFMResetBits wordt éénmalig aangeroepen in pre_application met 'AFMResetBits();' */

    int fc;

    /* Resetten AFM calamiteitenbits conflicterende richtingen (bit 13, 0x2000) */
    for (fc = 0; fc<FCMAX; ++fc)
    {
        BL[fc] &= ~BIT13;
        RR[fc] &= ~BIT13;
        X[fc] &= ~BIT13;
        Z[fc] &= ~BIT13;
        /*		 A[fc] &= ~BIT13; */
        /*		AA[fc] &= ~BIT13; */
        YV[fc] &= ~BIT13;
        YM[fc] &= ~BIT13;
        MK[fc] &= ~BIT13;
    }
}




int AFMmonitor_data_pre(int x, int y)
{
    /* Toon de waarden van (interne) AFM data in xyprint venster. */

    /* AFMmonitor wordt per deelnemende fase aangeroepen in post_application */
    /* met 'AFMmonitor(&AFM_verwerken_fc##)' */
#if !(defined AUTOMAAT) || (defined VISSIM)
    int yret = y;
    xyprintf(x, yret++, "_______________________________________________________________   ");
    xyprintf(x, yret++, "  AFM monitoring data");
    xyprintf(x, yret++, "  PR fc  GMAXAFM   TVG   TVG_g    Tc   Tc_g   ML  Afgekapt  gproc");
    xyprintf(x, yret++, "________________________________________________________________   ");

    return yret;
#endif
}

int AFMmonitor_data_fc(int x, int y, AFM_FC_STRUCT * AFM_data_fc)
{
#if !(defined AUTOMAAT) || (defined VISSIM)
    int yret = y;

    xyprintf(x, yret++, "  %d %s    %3d   %3d     %3d   %3d    %3d  %3d   %3d  %3d",
        (PR[AFM_data_fc->fc] > 0), FC_code[AFM_data_fc->fc], TVG_max[AFM_data_fc->fc] + TFG_max[AFM_data_fc->fc], AFM_data_fc->gact,
        AFM_data_fc->gmax_gem, AFM_data_fc->tc, AFM_data_fc->tc_gem, /*AFM_data_fc->ml + 1,*/ AFM_data_fc->afgekapt,
        AFM_data_fc->gmax_afm);

    return yret;
#endif
}

int AFMmonitor_acties_pre(int x, int y)
{
#if !(defined AUTOMAAT) || (defined VISSIM)
    int yret = y;

    xyprintf(x, yret++, "_______________________________________________________________   ");
    xyprintf(x, yret++, "  AFM monitoring acties");
    xyprintf(x, yret++, "");
    xyprintf(x, yret++, "_______________________________________________________________   ");

    return yret;
#endif
}

int AFMmonitor_acties_fc(int x, int y, AFM_FC_STRUCT * AFM_data_fc)
{
#if !(defined AUTOMAAT) || (defined VISSIM)
    int yret = y;
    /* @@@@ ALLEEN voor xyprintf scherm */

    /* @@@@ einde ALLEEN voor xyprintf scherm */
    /*
    xyprintf(x, yret++, "", FC_code[AFM_data_fc->fc],
    TVG_max[AFM_data_fc->fc], AFM_data_fc->gmax_afm,TVG_timer[AFM_data_fc->fc],
    TVG_max[AFM_data_fc->fc]);
    */
    return yret;
#endif
}



void AFMinterfacemonitor(AFM_FC_STRUCT afmfcstructs[AFM_fcmax])
{
    /* Toon de waarden van (externe) AFM data (parameters op de interface) in xyprint venster. */

    /* AFMinterfacemonitor wordt éénmalig aangeroepen in pre_system_application (zodat de */
    /* regelstatus ook tijdens knipperen wordt doorgegeven) met 'AFMinterfacemonitor();' */

    int afmprm, AFMfc;
#if !(defined AUTOMAAT) || (defined VISSIM)

    xyprintf(2, 6, "__________________________________________________________________  ");
    xyprintf(2, 7, "  AFM interface ");
    xyprintf(2, 8, "  FC  GCCOL MING  MAXG  GACT  GGEM  AFG   GAFM  QL    RelBR RelBV ");
    xyprintf(2, 9, "__________________________________________________________________  ");

    for (AFMfc = 0; AFMfc<AFM_fcmax; ++AFMfc)
    {
        afmprm = afmfcstructs[AFMfc].afm_prm;

        xyprintf(2, 10 + AFMfc, "  %3d   %3d  %3d   %3d   %3d   %3d   %3d   %3d   %3d   %3d   %3d",
            PRM[afmprm + AFM_FC],
            PRM[afmprm + AFM_GMAXCCOL],
            PRM[afmprm + AFM_GMAXMIN],
            PRM[afmprm + AFM_GMAXMAX],
            PRM[afmprm + AFM_GMAXACT],
            PRM[afmprm + AFM_GMAXGEM],
            PRM[afmprm + AFM_AFGEKAPT],
            PRM[afmprm + AFM_GMAXAFM],
            PRM[afmprm + AFM_STURING],
            PRM[afmprm + AFM_QLENGTH],
            PRM[afmprm + AFM_ABSBUFFERRUIMTE],
            PRM[afmprm + AFM_RELBUFFERRUIMTE],
            PRM[afmprm + AFM_RELBUFFERVULLING]);
    }
#endif


#if !(defined AUTOMAAT) || (defined VISSIM)
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
void AFM_tc(count prm_tc, count prm_tcgem)
{
    /* Start meting cyclustijd op start ML2 */
    if (SML && (ML == ML2))
    {
        tc_start = CIF_KLOK[CIF_SEC_TELLER];

        /* Bepaal laatst gerealiseerde cyclustijd; Tc groter dan of gelijk aan fasebewakingstijd wordt genegeerd */
        if ((tc_start > tc_einde_prev) && (tc_einde_prev != 0) && ((tc_start - tc_einde_prev) < PRM[prmfb]))
        {
            PRM[prm_tc] = tc_start - tc_einde_prev;
            /* Schuif registers van hogere naar lagere index */
            tc_tijd[0] = tc_tijd[1];
            tc_tijd[1] = tc_tijd[2];
            tc_tijd[2] = PRM[prm_tc];
            /* Bepaal gemiddelde van de laatste 3 cyclustijden */
            if ((tc_tijd[0] != 0) &&
                (tc_tijd[1] != 0) &&
                (tc_tijd[2] != 0))
            {
                PRM[prm_tcgem] = (count)(tc_tijd[0] +
                    tc_tijd[1] +
                    tc_tijd[2]) / 3;
            }


        }
        tc_einde_prev = tc_start;



    }
}
