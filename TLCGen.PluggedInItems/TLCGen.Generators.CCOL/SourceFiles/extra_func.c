#include "extra_func.h"

int Knipper_1Hz = 0;
int Knipper_2Hz = 0;

/** ------------------------------------------------------------------------------
    MAXIMAAL MEEVERLENGEN
    ------------------------------------------------------------------------------
    Versie  Datum       Wie     Commentaar                      Vastgesteld in CO
    ------  ----------  ---     ----------                      -----------------
    1.0   12-03-2012  psn     basis                            23-10-2012
    ------------------------------------------------------------------------------
    Dit is een aangepaste versie van de standaard generator functie ym_max().
    Functie ym_max() wordt gebruikt bij de specificatie van de instructie-
    variabele YM[] (vasthouden meeverlenggroen) van de fasecyclus.
    De functie ym_max() blijft waar (TRUE) zolang de fasecyclus kan meeverlengen
    (met in achtname van ontruimingstijdverschillen).
    
    Ten opzichte van de functie uit de generator is het verschil:
    - De functie kijkt in plaats van alleen naar RA[], ook naar AAPR[]
    ------------------------------------------------------------------------------ */

bool ym_maxV1(count i, mulv to_verschil)
{
	register count n, j, k, m;
	bool ym;

	if (MG[i])
	{   /* let op! i.v.m. snelheid alleen in MG[] behandeld */
		ym = TRUE;
#ifndef NO_GGCONFLICT
		for (n = 0; n<GKFC_MAX[i]; ++n)
#else
		for (n = 0; n<KFC_MAX[i]; ++n)
#endif
		{
#if (CCOL_V >= 95)
			k = KF_pointer[i][n];
#else
			k = TO_pointer[i][n];
#endif
			if (RA[k] || AAPR[k])
			{
				ym = FALSE;
#ifndef NO_GGCONFLICT
#if (CCOL_V >= 95) && !defined NO_TIGMAX
				if (TIG_max[i][k] < GK)  break;
#else
				if (TO_max[i][k] < GK)  break;
#endif
#endif
				for (j = 0; j<KFC_MAX[k]; ++j)
				{
#if (CCOL_V >= 95)
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
				    if (CV[m] && (((TIG_max[i][k] - to_verschil) <= TIG_max[m][k])
#else
					if (CV[m] && (((TO_max[i][k] - to_verschil) <= TO_max[m][k])
#endif
						|| (to_verschil<0)))
					{
						ym = TRUE;
						break;
					}
				}
#ifndef NO_GGCONFLICT
				for (j = KFC_MAX[k]; j < GKFC_MAX[k]; ++j)
				{
#if (CCOL_V >= 95)
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
					if (CV[m] && (TIG_max[m][k] == GKL || TIG_max[i][k] <= GK))
#else
			        if (CV[m] && (TO_max[m][k] == GKL || TO_max[i][k] <= GK))
#endif
					{
						ym = TRUE;
						break;
					}
				}
#endif
			}
			if (!ym) break;
		}
	}
	else ym = FALSE;
	return ym;
}

bool ym_max_prmV1(count i, count prm, mulv to_verschil)
{
	switch (PRM[prm]) 
	{
	case 0:
		return FALSE;
	case 1:
		return ym_maxV1(i, to_verschil);
	case 2:
		return ym_max_toV1(i, to_verschil);
	case 3:
		return ym_maxV1(i, to_verschil) || MK[i] && ym_max_toV1(i, to_verschil);
	case 4:
		return ym_max_vtgV1(i);
	case 5:
		return ym_max(i, to_verschil);
	case 6:
#if (CCOL_V >= 95) && !defined NO_TIGMAX
		return ym_max_tig(i, to_verschil);
#else
		return ym_max_to(i, to_verschil);
#endif
	case 7:
#if (CCOL_V >= 95) && !defined NO_TIGMAX
		return ym_max(i, to_verschil) || MK[i] && ym_max_tig(i, to_verschil);
#else
		return ym_max(i, to_verschil) || MK[i] && ym_max_to(i, to_verschil);
#endif
	}
	return FALSE;
}

/* MAXIMUM MEEVERLENGGROEN */
/* ======================= */
/* ym_max_to() wordt gebruikt bij de specificatie van de instructie-
 * variabele YM[] (vasthouden meeverlenggroen) van de fasecyclus.
 * ym_max_to() blijft waar (TRUE) zolang de fasecyclus kan meeverlengen
 * (met in achtname van ontruimingstijdverschillen).
 * het aanroepen van de functie ym_max_to() dient in de applicatiefunctie
 * application() te worden gespecificeerd.
 * ym_max_to() blijft ook meeverlengen met MG[], GL[] en R[]
 * zolang het ontruimingstijd verschil voordelig is!!
 * tbv deze functie is in de file fcfunc.c TGL_timer[] al op SG[] op 0 gezet.
 *
 *  32  fffffflll
 *  05  ffffffmmmmmmmmxxx
 *  08                      ffffffvvvvvxxxxx
 *
 *  YM[fc05]= ym_max_to(fc05,0) || CIF_IS[is_fix];
 *  fc05 blijft meeverlengen met ontruimingstijd van fc32 naar fc08.
 */
bool ym_max_toV1(count i, mulv to_verschil)
{
	register count n, j, k, m;
	bool ym;

	if (MG[i]) /* let op! i.v.m. snelheid alleen in MG[] behandeld	*/
	{
		ym = TRUE;
		for (n = 0; n < GKFC_MAX[i]; ++n)
		{
#if (CCOL_V >= 95)
			k = KF_pointer[i][n];
#else
			k = TO_pointer[i][n];
#endif
			if ((RA[k] || AAPR[k]))
			{
				ym = FALSE;
#if (CCOL_V >= 95) && !defined NO_TIGMAX
				if (TIG_max[i][k] < GK)  break;
#else
				if (TO_max[i][k] < GK)  break;
#endif
				for (j = 0; j < KFC_MAX[k]; ++j)
				{
#if (CCOL_V >= 95)
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif

#if (CCOL_V >= 95) && !defined NO_TIGMAX
					if (CV[m] && (((TIG_max[i][k] - to_verschil) <=
						(TIG_max[m][k])) &&
						((TIG_max[i][k]) < (TFG_max[m] - TFG_timer[m] +
							TVG_max[m] - TVG_timer[m] +
							TIG_max[m][k] - TIG_timer[m]))
						|| (to_verschil < 0))
						|| TIG[m][k]
						&& ((TIG_max[i][k]) < (TIG_max[m][k] - TIG_timer[m])))
#else
					if (CV[m] && (((TGL_max[i] + TO_max[i][k] - to_verschil) <=
						(TGL_max[m] + TO_max[m][k])) &&
						((TGL_max[i] + TO_max[i][k]) < (TFG_max[m] - TFG_timer[m] +
							TVG_max[m] - TVG_timer[m] + TGL_max[m] - TGL_timer[m] +
							TO_max[m][k] - TO_timer[m]))
						|| (to_verschil < 0))
						|| TO[m][k]
						&& ((TGL_max[i] + TO_max[i][k]) < (TGL_max[m] + TO_max[m][k] -
							TGL_timer[m] - TO_timer[m])))
#endif
					{
						ym = TRUE;
						break;
					}
				}

				for (j = KFC_MAX[k]; j < FKFC_MAX[k]; j++)
				{
#if (CCOL_V >= 95)
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
					if (CV[m] && (TIG_max[m][k] == GKL || TIG_max[i][k] <= GK))
#else
					if (CV[m] && (TO_max[m][k] == GKL || TO_max[i][k] <= GK))
#endif
					{
						ym = TRUE;
						break;
					}
				}
			}
			if (!ym) break;
		}
	}
	else  ym = CV[i];
	return ym;
}


/** ------------------------------------------------------------------------------
MAXIMAAL MEEVERLENGEN VOOR VOETGANGERS
------------------------------------------------------------------------------
Versie  Datum       Wie     Commentaar                      Vastgesteld in CO
------  ----------  ---     ----------                      -----------------
1.0   12-03-2012  psn     basis                            23-10-2012
------------------------------------------------------------------------------
Dit is een aangepast versie van de standaard generator functie ym_max().
Functie ym_max() wordt gebruikt bij de specificatie van de instructie-
variabele YM[] (vasthouden meeverlenggroen) van de fasecyclus.
De functie ym_max() blijft waar (TRUE) zolang de fasecyclus kan meeverlengen
(met in achtname van ontruimingstijdverschillen).

De functie is aangepast voor gebruik bij voetgangers richtingen.
Ten opzichte van de functie uit de generator is het verschil:
- De functie kijkt in plaats van alleen naar RA[], ook naar AAPR[]
- De functie kijkt of de maximale groentijd voor de fase is bereikt
t.o.v. de benodigde ontruiming van autorichtingen
------------------------------------------------------------------------------ */
bool ym_max_vtgV1(count i)
{
	register count n, j, k, m;
	bool ym;

	if (MG[i]) /* let op! i.v.m. snelheid alleen in MG[] behandeld  */
	{
		ym = TRUE;
		/* nalopen of nog moet worden meeverlengd */
#ifndef NO_GGCONFLICT
		for (n = 0; n < GKFC_MAX[i]; ++n)
#else
		for (n = 0; n < KFC_MAX[i]; ++n)
#endif
		{
#if (CCOL_V >= 95)
			k = KF_pointer[i][n];
#else
			k = TO_pointer[i][n];
#endif
			if (RA[k] || AAPR[k])
			{
				ym = FALSE;
#ifndef NO_GGCONFLICT
#if (CCOL_V >= 95) && !defined NO_TIGMAX
				if (TIG_max[i][k] < GK)  break;
#else
				if (TO_max[i][k] < GK)  break;
#endif
#endif
				for (j = 0; j < KFC_MAX[k]; ++j)
				{
#if (CCOL_V >= 95)
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
					if (FC_type[m] == MVT_type)
					{
						/* bereken of max groentijd is bereikt tov benodigde ontruiming */
						if (CV[m] && PR[m] && !(VG[m] &&
							((TVG_max[m] - TVG_timer[m]) <
#if (CCOL_V >= 95) && !defined NO_TIGMAX
							(TIG_max[i][k] - TIG_max[m][k]))) &&
							 !(WG[m] && (TVG_max[m] < (TIG_max[i][k] - TIG_max[m][k]))))
#else
							(TO_max[i][k] + TGL_max[i] - TO_max[m][k] - TGL_max[m]))) &&
							 !(WG[m] && (TVG_max[m] < (TO_max[i][k] + TGL_max[i] - TO_max[m][k] - TGL_max[m]))))
#endif
						{
							ym = TRUE;
							break;
						}
					}
					else
					{
						if (CV[m] && !VG[m] && !(WG[m] && (RW[m] & BIT2)))
						{
							ym = TRUE;
							break;
						}
					}
				}
#ifndef NO_GGCONFLICT
				for (j = KFC_MAX[k]; j < GKFC_MAX[k]; ++j)
				{
#if (CCOL_V >= 95)
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
#if (CCOL_V >= 95) && !defined NO_TIGMAX
					if (CV[m] && (TIG_max[m][k] == GKL))
#else
					if (CV[m] && (TO_max[m][k] == GKL))
#endif
					{
						ym = TRUE;
						break;
					}
				}
#endif
			}
			if (!ym)
				break;
		}
	}
	else ym = FALSE;
	return ym;
}

/** ------------------------------------------------------------------------------
AANVRAAG SNEL
------------------------------------------------------------------------------
Versie  Datum       Wie     Commentaar                      Vastgesteld in CO
------  ----------  ---     ----------                      -----------------
1.0   12-03-2012  dze     basis                            06-12-2011
2.0   23-04-2012  psn     aangepaste syntax                23-10-2012
------------------------------------------------------------------------------
Functie voor het direct opzetten van een aanvraag.
Voertuig hoeft bezettijd niet af te wachten indien er geen conflicten lopen.

hierbij:
- fc:       fase (vaak een fiets)
- dp:       detector

Resultaat:   zet BIT4 op van A[fc]
------------------------------------------------------------------------------ */

void AanvraagSnelV2(count fc1, count dp)
{
	/* richting mag gelijk realiseren indien er geen conflicten lopen */
	mee_aanvraag_spec(fc1, (bool)(!kcv(fc1) && !K[fc1] && R[fc1] && !TRG[fc1] &&
		D[dp] && (CIF_IS[dp] <= CIF_DET_STORING)));
}

/** ------------------------------------------------------------------------------
    ACOUSTISCHE SIGNALEN:   RATELTIKKERS
    ------------------------------------------------------------------------------
    Versie  Datum       Wie     Commentaar                      Vastgesteld in CO
    ------  ----------  ---     ----------                      -----------------
      1.0   21-11-2011  dze     basis                            06-12-2011
      2.0   03-01-2011  mvw     afzetten alleen als niet         23-10-2012
                                rateltikkers continu
      3.0   22-11-2012  dze     verbetering code                 xx-xx-xxxx
      4.0   19-04-2016  mvw     loslaten ACCROSS                 xx-xx-xxxx
    ------------------------------------------------------------------------------

    De rateltikkers voor voetgangers worden niet in het regeltoestel afgehandeld,
    maar middels units in de armaturen. Het aansturen van die units gebeurt
    middels een uitgang die direct wordt aangestuurd vanuit de applicatie.

    Vanuit de applicatie worden de rateltikkers voor voetgangers aangestuurd:
    a)  van begin melding drukknop (ook indien de aanvraag dan (nog) niet ontstaat!) 
        tot na aflopen timer tnlrt, die start bij het eerstvolgend einde groen 
        van de bijbehorende richting;
    b)  van begin melding (buiten) drukknop tot na aflopen timer tnlrt, die 
        start bij het eerstvolgend einde groen mits die drukknop een groene golf 
        naar de bijbehorende richting aanvraagt;
    c)  continu tijdens een aanwezige klokperiode (MM [  mas_aan]);
    d)  continu tijdens een aanwezige schakelaar  (SCH[schas_aan]).
    
    Het dimsignaal voor alle rateltikkers is aanwezig:
    a)  tijdens een aanwezige klokperiode (MM [  mas_dim]);
    b)  tijdens een aanwezige schakelaar  (SCH[schas_dim]).
    ------------------------------------------------------------------------------ */
bool Rateltikkers(      count fc,       /* fase */
                        count has,      /* hulpelement rateltikkers voor deze fase */
                        count has_aan_, /* hulpelement tikkers werking */
                        count has_cont_,/* hulpelement tikkers continu */
                        count tnlrt,    /* tijd na EG dat de tikkers nog moeten worden aangestuurd indien niet continu */
                        ...)            /* hulpelementen drukknoppen */
{
	va_list argpt;
	count hdkh;
	bool hdk = FALSE;

	/* verzorgen naloop rateltikker */
	RT[tnlrt] = (G[fc] || GL[fc]) && IH[has] || EH[has_cont_];

	/* check tikkers werking */
	if (IH[has_aan_])
	{
		va_start(argpt, tnlrt);
		while ((hdkh = va_arg(argpt, va_count)) != END)
		{
			/* opzetten rateltikkers bij detectie drukknoppen */
			IH[has] |= IH[hdkh];
			if (IH[hdkh]) hdk = TRUE;
		}
		va_end(argpt);
	}

    /* afzetten rateltikker na aflopen naloop timer (indien geen nieuwe drukknop melding) */
    if (ET[tnlrt] && !hdk) {
		IH[has] = FALSE;
    }
    
    /* continue aansturing rateltikkers */
	if (has_cont_ > NG) {
		IH[has] |= IH[has_cont_];
	}

    return (IH[has]);
}

/** ------------------------------------------------------------------------------
    ACOUSTISCHE SIGNALEN:   RATELTIKKERS
    ------------------------------------------------------------------------------
    Versie  Datum       Wie     Commentaar                      Vastgesteld in CO
    ------  ----------  ---     ----------                      -----------------
    1.0   21-11-2011  dze     basis                            06-12-2011
    2.0   03-01-2011  mvw     afzetten alleen als niet         23-10-2012
    rateltikkers continu
    3.0   22-11-2012  dze     verbetering code                 xx-xx-xxxx
    ------------------------------------------------------------------------------
    
    De rateltikkers voor voetgangers worden in het regeltoestel afgehandeld via de
    Across-units. Deze units dragen zelf zorg voor dat de rateltikkers 'aan' blijven
    tijdens:
    1)  groen of groenknipperen van de aangesloten richting
    2)  lopen instelbare tijd (herstart tijdens groen en groenknipperen)
    
    Vanuit de applicatie worden de rateltikkers voor voetgangers aangestuurd:
    a)  van begin melding drukknop (ook indien de aanvraag dan (nog) niet ontstaat!)
    tot eerstvolgend begin groen van de bijbehorende richting;
    b)  van begin melding (buiten) drukknop tot eerstvolgend begin groen mits die
    drukknop een groene golf naar de bijbehorende richting aanvraagt;
    c)  continu tijdens een aanwezige klokperiode (MM [  mas_aan]);
    d)  continu tijdens een aanwezige schakelaar  (SCH[schas_aan]).
    Bij einde van een continue aanvraag wordt tijdens niet groen een aanvraag voor
    groen van de bijbehorende richting ingediend.
    
    Het dimsignaal voor alle rateltikkers is aanwezig:
    a)  tijdens een aanwezige klokperiode (MM [  mas_dim]);
    b)  tijdens een aanwezige schakelaar  (SCH[schas_dim]).
    ------------------------------------------------------------------------------ */
bool Rateltikkers_Accross(count fc,       /* fase */
	count has,      /* hulpelement rateltikkers voor deze fase */
	count has_aan_, /* hulpelement tikkers werking */
	count has_cont_,/* hulpelement tikkers continu */
	...)            /* drukknoppen */
{
	va_list argpt;
	count dkid;

	/* afzetten rateltikkers als niet continu aanvraag */
	if (G[fc])                                          IH[has] = FALSE;
	if (has_cont_ > NG)                                 IH[has] |= IH[has_cont_];

	/* check tikkers werking */
	if (IH[has_aan_])
	{
		va_start(argpt, has_cont_);
		while ((dkid = va_arg(argpt, va_count)) != END)
		{
			/* opzetten rateltikkers bij detectie drukknoppen */
			IH[has] |= IH[dkid];
		}
		va_end(argpt);
	}

	/* eenmalige aanvraag afzetten Across */
	if (has_cont_ > NG)
		if (EH[has_cont_])                              A[fc] |= !G[fc];

	return (IH[has]);
}

/** ------------------------------------------------------------------------------
    EERLIJK DOSEREN BIJ FILE
    ------------------------------------------------------------------------------
    Versie  Datum       Wie     Commentaar                      Vastgesteld in CO
    ------  ----------  ---     ----------                      -----------------
      1.0   25-02-2013  mvw     basis                            xx-xx-xxxx
    ------------------------------------------------------------------------------
    Deze functie is bedoeld voor gebruik in combinatie met Filemelding(). Wanneer
    vanwege een filemelding wordt gedoseerd, houdt deze functie bij hoeveel is
    gedoseerd per betreffende richting. Is de file voorbij, dan wordt een
    eventuele ongelijkheid in de mate van doseren gelijk getrokken. Zodanig
    krijgt elke richting uiteindelijk een even grote dose.
    Deze functie maakt gebruik van een aantal static int/short variabelen die
    buiten de functie gedeclareerd moeten worden.
    De functie gaat uit van 4 klokperioden, waarvan de parameters via het argument
    'count fcmg[][4]' aan de functie worden doorgegeven.
    ------------------------------------------------------------------------------ */
void Eerlijk_doseren_V1(count hfile,              /* hulpelement wel/geen file                         */
                        count _prmperc,           /* indexnummer parameter % doseren                   */
                        count aantalfc,           /* aantal te doseren fasen                           */
                        count fc[],               /* pointer naar array met fasenummers                */
                        count fcmg[][MPERIODMAX], /* pointer naar array met mg parameter index nummers */
                        int nogtedoseren[],       /* pointer naar array met nog te doseren waarden     */
	                    bool *prml[],
	                    count ml)
{
    int i, j, laatstedosering;

    /* Voor elke fase mbt dit filegebied */
    for(i = 0; i < aantalfc; ++i)
    {
        /* Als het file is, eindegroen, en het meetkriterium staat op */
        if(EG[fc[i]] && H[hfile] && MK[fc[i]])
        {
            /* Uitrekenen laatst toegepaste dosering */
            laatstedosering = 100 - (int)(100.0 * (((float)(TFG_max[fc[i]] + TVG_timer[fc[i]])) / ((float)(PRM[fcmg[i][MM[mperiod]]]))));
            /* Voor elke fase mbt het fileveld */
            for(j = 0; j < aantalfc; ++j)
            {
                /* Die niet de fase is die nu EG heeft */
                if(i == j)continue;
                /* uitrekenen nogtedoseren waarde voor de andere richtingen:
                   (tbv programmeertechnische veiligheid van het algoritme: 
                    alleen doen wanneer de uitkomst lager is dan het file doseer percentage) */
                if((laatstedosering - nogtedoseren[i] + nogtedoseren[j]) <= (100 - PRM[_prmperc]))
                {
                    /* Nog te doseren is verschil tussen laatste dosering van huidige fase met EG en nogtedoseren waarde van die fase,
                       plus de actuele nogtedoseren waarde van fase j */
                    nogtedoseren[j] = laatstedosering - nogtedoseren[i] + nogtedoseren[j];
                }
            }
            /* Zet de nog te doseren waarde van de fase met EG op 0 */
            nogtedoseren[i] = 0;
        }
        /* Op eindegroen wanneer er geen file is, of wel file maar geen MK,
           zet actuele waarden op 0 */
        if((nogtedoseren[i] && EG[fc[i]] && PR[fc[i]] && !H[hfile]) || (EG[fc[i]] && H[hfile] && !MK[fc[i]]) || nogtedoseren[i] && !A[fc[i]] && prml[ml][fc[i]] == PRIMAIR)
        {
            nogtedoseren[i] = 0;
        }

        /* Doseren! */
        if(nogtedoseren[i] > 0 && !H[hfile])
        {
            /* Toepassen eerder opgeslagen nog te doseren percentage */
            TVG_max[fc[i]] = ((mulv)(((long)(100 - nogtedoseren[i]) * (long)PRM[fcmg[i][MM[mperiod]]])/100) > TFG_max[fc[i]])
                        ?  (mulv)(((long)(100 - nogtedoseren[i]) * (long)PRM[fcmg[i][MM[mperiod]]])/100) - TFG_max[fc[i]]
                        : 0;
        }
    }   
}
void Eerlijk_doseren_VerlengGroenTijden_V1(count hfile, /* hulpelement wel/geen file                         */
                        count _prmperc,                 /* indexnummer parameter % doseren                   */
                        count aantalfc,                 /* aantal te doseren fasen                           */
                        count fc[],                     /* pointer naar array met fasenummers                */
                        count fcvg[][MPERIODMAX],       /* pointer naar array met mg parameter index nummers */
                        int nogtedoseren[],             /* pointer naar array met nog te doseren waarden     */
	                    bool *prml[],
	                    count ml)
{
    int i, j, laatstedosering;

    /* Voor elke fase mbt dit filegebied */
    for(i = 0; i < aantalfc; ++i)
    {
        /* Als het file is, eindegroen, en het meetkriterium staat op */
        if(EG[fc[i]] && H[hfile] && MK[fc[i]])
        {
            /* Uitrekenen laatst toegepaste dosering */
            laatstedosering = 100 - (int)(100.0 * (((float)(TFG_max[fc[i]] + TVG_timer[fc[i]])) / ((float)(PRM[fcvg[i][MM[mperiod]]]+TFG_max[fc[i]]))));
            /* Voor elke fase mbt het fileveld */
            for(j = 0; j < aantalfc; ++j)
            {
                /* Die niet de fase is die nu EG heeft */
                if(i == j)continue;
                /* uitrekenen nogtedoseren waarde voor de andere richtingen:
                   (tbv programmeertechnische veiligheid van het algoritme:
                    alleen doen wanneer de uitkomst lager is dan het file doseer percentage) */
                if((laatstedosering - nogtedoseren[i] + nogtedoseren[j]) <= (100 - PRM[_prmperc]))
                {
                    /* Nog te doseren is verschil tussen laatste dosering van huidige fase met EG en nogtedoseren waarde van die fase,
                       plus de actuele nogtedoseren waarde van fase j */
                    nogtedoseren[j] = laatstedosering - nogtedoseren[i] + nogtedoseren[j];
                }
            }
            /* Zet de nog te doseren waarde van de fase met EG op 0 */
            nogtedoseren[i] = 0;
        }
        /* Op eindegroen wanneer er geen file is, of wel file maar geen MK,
           zet actuele waarden op 0 */
        if((nogtedoseren[i] && EG[fc[i]] && PR[fc[i]] && !H[hfile]) || (EG[fc[i]] && H[hfile] && !MK[fc[i]]) || nogtedoseren[i] && !A[fc[i]] && prml[ml][fc[i]] == PRIMAIR)
        {
            nogtedoseren[i] = 0;
        }

        /* Doseren! */
        if(nogtedoseren[i] > 0 && !H[hfile])
        {
            /* Toepassen eerder opgeslagen nog te doseren percentage */
            TVG_max[fc[i]] = (mulv)(((long)(100 - nogtedoseren[i]) * (long)(PRM[fcvg[i][MM[mperiod]]]+TFG_max[fc[i]]))/100);
        }
    }   
}

/* Functie om type meeaanvraag op straat instelbaar te maken */
void mee_aanvraag_prm(count i, count j, count prm, bool extra_condition)
{
    if(!extra_condition)
        return;

    switch (PRM[prm])
    {
    case 1:
        if (A[j]) A[i] |= BIT4;
        break;
    case 2:
        if (RA[j]) A[i] |= BIT4;
        break;
    case 3:
        if (RA[j] && !K[j]) A[i] |= BIT4;
        break;
    case 4:
        if (SG[j]) A[i] |= BIT4;
        break;
    default:
        break;
    }
}

/*****************************************************************************
 *  Functie  : Filemelding
 *
 *  Functionele omschrijving :
 *    CCOL implementatie eenvoudig filemeetpunt (1 detector)
 *
 *  In regeling zet men een hulpelement op afhankelijk van een of meer keren
 *  deze functie aan te roepen. VB:
 *
 *    Filemelding detector df021 en df022
 *
 *    FileMelding(df021, tbz021, trij021, tafv021, hfile021, hafval021);
 *    FileMelding(df022, tbz022, trij022, tafv022, hfile022, hafval022);
 *
 *    RT[tAfvalbewaking] = (D[df021] || D[df022]);
 *    if (!(T[tAfvalbewaking] || RT[tAfvalBewaking]) && SCH[schAfvalbewaking])
 *      IH[hfile021] = IH[hfile022] = FALSE;
 *    IH[hfile] = H[hfile021] || H[hfile022];
 ****************************************************************************/
void FileMeldingV2(count det,     /* filelus                                */
                   count tbez,    /* bezettijd  als D langer bezet -> file  */
                   count trij,    /* rijtijd    als D korter bezet -> !file */
                   count tafval,  /* afvalvertraging filemelding            */
                   count hfile)   /* hulpelement filemelding                */
{
    RT[tbez]   = RT[trij] = !D[det];
    RT[tafval] = D[det] && !T[tbez] ||
		         H[hfile] && D[det] && !T[trij];

	IH[hfile] = RT[tafval] || T[tafval] || H[hfile] && D[det];

    if (CIF_IS[det] >= CIF_DET_STORING)   IH[hfile] = FALSE;
}

void UpdateKnipperSignalen()
{
	Knipper_1Hz = ((CIF_KLOK[CIF_TSEC_TELLER] % 10) > 4); /* 1 Hz */
	Knipper_2Hz = ((CIF_KLOK[CIF_TSEC_TELLER] % 5) > 2); /* 2 Hz */
}

bool hf_wsg_nl(void)
{
	register count i;

	for (i = 0; i<FC_MAX; i++) {
		if (G[i] && !MG[i] && !WS[i] && !(WG[i] && (RW[i] & BIT2)) || G[i] && MK[i] || GL[i] || TRG[i]
			|| R[i] && A[i] && !BL[i])
			return (TRUE);
	}
	return (FALSE);
}

bool hf_wsg_nl_fcfc(count fc1, count fc2)
{
	register count i;

	for (i = fc1; i < fc2; i++) {
		if (G[i] && !MG[i] && !WS[i] && !(WG[i] && (RW[i] & BIT2)) || G[i] && MK[i] || GL[i] || TRG[i]
			|| R[i] && A[i] && !BL[i])
			return (TRUE);
	}
	return (FALSE);
}

void wachttijd_leds_knip(count fc, count mmwtv, count mmwtm, count RR_T_wacht, count fix)
{
	/* fc    - fasecyclusnummer                            */
	/* mmwtv - berekende  aantal leds wachttijdlantaarn    */
	/* mmwtm - uitsturing aantal leds wachttijdlantaarn    */

	if (((fix != NG && CIF_IS[fix]) || (RR_T_wacht > 0)) && MM[mmwtm])         /* fixatie of prio-ingreep terwijl wachttijdlantaarn aan staat  */
	{
		if (MM[mmwtm] > 1)       MM[mmwtm] = MM[mmwtv] - Knipper_1Hz;     /* bij aantal leds groter dan 1 = actuele ledje laten knipperen */
		else                     MM[mmwtm] = MM[mmwtv] + Knipper_1Hz;     /* bij ??n leds de voorgaande led, zodat de wtv niet uitgaat    */
	}
	else                        MM[mmwtm] = MM[mmwtv];            /* anders berekende aantal leds gewoon overnemen                */
}

bool kcv_primair_fk_gkl(count i)
{
	register count n, j;

#ifndef NO_GGCONFLICT
	for (n = 0; n < GKFC_MAX[i]; ++n)
#else
	for (n = 0; n < KFC_MAX[i]; ++n)
#endif
	{
#if (CCOL_V >= 95)
		j = KF_pointer[i][n];
#else
		j = TO_pointer[i][n];
#endif
		if ((((R[j] || GL[j]) && AA[j] || CV[j] ||
			G[j] && (RS[j] || RW[j])) && PR[j]) &&
#if (CCOL_V >= 95) && !defined NO_TIGMAX
			!((TIG_max[i][j] == GKL) && (TIG_max[j][i] == FK)))
#else
			!((TO_max[i][j] == GKL) && (TO_max[j][i] == FK)))
#endif
			return (TRUE);
	}
	return (FALSE);
}

static bool a_pg_fkprml_fk_gkl(count i, bool *prml[], count ml)
{
   register count n,j;

   for (n = 0; n < GKFC_MAX[i]; ++n)
   {
#if (CCOL_V >= 95)
	   j = KF_pointer[i][n];
#else
	   j = TO_pointer[i][n];
#endif
      if ((A[j] && !BL[j] && (!G[j] || CV[j]) || PP[j]) &&
	      ((prml[ml][j] & PRIMAIR_VERSNELD) && !PG[j]) )
	 return (FALSE);	 /* TO_pointer[i][n]	*/
   }

   for (n = GKFC_MAX[i]; n < FKFC_MAX[i]; ++n) {
#if (CCOL_V >= 95)
	   j = KF_pointer[i][n];
#else
	   j = TO_pointer[i][n];
#endif
      if ((A[j] && !BL[j] && !G[j] || PP[j]) &&
	      ((prml[ml][j] & PRIMAIR_VERSNELD) && !PG[j]) )
	 return (FALSE);	 /* TO_pointer[i][n]	*/
   }
   return (TRUE);
 
}


static bool a_ag_fkprml_fk_gkl(count i, bool *prml[], count ml)
{
	register count n, j;

	for (n = 0; n < FKFC_MAX[i]; ++n) {
#if (CCOL_V >= 95)
		j = KF_pointer[i][n];
#else
		j = TO_pointer[i][n];
#endif
		if ((A[j] && !BL[j] && !G[j]) &&
			((prml[ml][j] & ALTERNATIEF_VERSNELD) && !AG[j]))
			return (FALSE);	 /* TO_pointer[i][n] */
	}
	return (TRUE);
}

static void set_pg_fkprml_fk_gkl(count i, bool *prml[], count ml)
{
	register count n, j;

	for (n = 0; n < FKFC_MAX[i]; ++n)
	{
#if (CCOL_V >= 95)
		j = KF_pointer[i][n];
#else
		j = TO_pointer[i][n];
#endif
		if (prml[ml][j] & PRIMAIR)  PG[j] |= PRIMAIR_OVERSLAG;
	}
}

bool kcv_fk_gkl(count i)
{
	register count n, k;

#ifndef NO_GGCONFLICT
	for (n = 0; n < GKFC_MAX[i]; ++n) {
#else
	for (n = 0; n < KFC_MAX[i]; ++n) {
#endif
#if (CCOL_V >= 95)
		k = KF_pointer[i][n];
#else
		k = TO_pointer[i][n];
#endif
		if (((R[k] || GL[k]) && AA[k] || CV[k] || G[k] && (RS[k] || RW[k])) && 
#if (CCOL_V >= 95) && !defined NO_TIGMAX
			!((TIG_max[i][k] == GKL) && (TIG_max[k][i] == FK)))
#else
			!((TO_max[i][k] == GKL) && (TO_max[k][i] == FK)))
#endif
			return (TRUE);
	}
	return (FALSE);
}

bool set_FPRML_fk_gkl(count i, bool *prml[], count ml, count ml_max, bool period)
{
	register count hml, m;

	if (AAPR[i] && !AA[i] && !prml[ml][i])  AAPR[i] = FALSE;

	if (A[i] && RV[i] && !TRG[i] && !AA[i] && !BL[i])
	{
		if (!prml[ml][i] && !PG[i]) /* not in active module or realized */
		{
			hml = ml;
			for (m = 0; m < ml_max; ++m)
			{
				if (!a_pg_fkprml_fk_gkl(i, prml, hml))
				{
					AAPR[i] = 0;
					return (FALSE);
				}
				if (!a_ag_fkprml_fk_gkl(i, prml, hml))
				{
					AAPR[i] |= BIT1;
				}
				if (prml[hml][i] == PRIMAIR) 
				{
					if (!period) AAPR[i] |= BIT5;
					if (RR[i])   AAPR[i] |= BIT4;
					if (fkaa_primair(i))  AAPR[i] |= BIT3;
					if (kcv_primair_fk_gkl(i))   AAPR[i] |= BIT2;
					if (fkaa(i)) 	     AAPR[i] |= BIT1;
					if (AAPR[i])  return (FALSE);
					AAPR[i] = BIT0;

					if (!kcv_fk_gkl(i))
					{
						do
						{
							--hml;
							if (hml < 0)  hml = ml_max - 1;
							set_pg_fkprml_fk_gkl(i, prml, hml);  /* set confl. pg's	*/
							prml[hml][i] |= VERSNELD_PRIMAIR; /* set_FPRML	*/
						} while (hml != ml);
						return (TRUE);
					}
					else return (FALSE);
				}
				++hml;
				if (hml >= ml_max)  hml = ML1;
			}
		}
	}
	return (FALSE);
}

/**************************************************************************
 *  Functie  : veiligheidsgroen_V1
 *
 *  Functionele omschrijving :
 *    Een verbeterde versie van de veiligheidsgroen functionaliteit uit
 *    STDFUNC.C. Er wordt alleen veiligheidsgroen gegeven wanneer
 *    zich meer dan 1 voertuig in de dilemmazone bevindt.
 *    Deze variant start meteen met meten op start MG[].
 *    Per fase dienen ALLE (schakelbaar) deelnemende lussen te worden opgegeven.
 *
 **************************************************************************/
void veiligheidsgroen_V1(count fc, count tmaxvag4, ...)
{/* tmaxvag4            * maximale tijd dat veiligheidsgroen mag worden toegekend   */

    va_list argpt;     /* variabele argumentenlijst                                 */
    count dp;          /* detector waarop veiligheidsgroen wordt bepaald            */
    count tvlg;        /* volgtijd tussen twee voertuigen                           */
    count schvag4;     /* schakelaar per lus                                        */
    count tvgh;        /* hiaattijd waarmee gerekend moet worden bij toekennen vag4 */
   
    bool vag4     = FALSE;
    bool bewaking = FALSE;
    YM[fc]       &= ~BIT2;
   
    va_start(argpt, tmaxvag4);
    dp = va_arg(argpt, va_count);

    do {
        tvlg    = va_arg(argpt, va_count);
        schvag4  = va_arg(argpt, va_count);
        tvgh     = va_arg(argpt, va_count);

        if (schvag4 == NG || SCH[schvag4]) {
            /* meten dilemma */
            if (SD[dp] && CIF_IS[dp] < CIF_DET_STORING) {  /* geen detectiestoring */
                if (DVG[dp] < 2) ++DVG[dp];         /* ophogen teller gemeten voertuigen           */
                if (DVG[dp] >= 2) bewaking = TRUE; /* bij meer dan 1 voeruig in dil.zone: bewaken */
                RT[tvlg] = TRUE;                 /* herstarten volg tijd                        */
            }
            else {
                RT[tvlg] = FALSE;
            }

            /* meten hiaat:
               bij meer dan 1 voertuig in de dilemmazone
               vag4hiaatmeting starten obv detectie */
            if (DVG[dp] >= 2) RT[tvgh] = D[dp];
            else              RT[tvgh] = FALSE;

            /* bepalen VAG4:
               indien bew.tijd en vag4hiaat beide lopen
               veiligheidsgroen op zetten */
            if (T[tmaxvag4] && T[tvgh]) vag4 = TRUE;
            if (ET[tvlg]) DVG[dp] = 0;
        }
        dp = va_arg(argpt, va_count);
    } while (dp != END);

    RT[tmaxvag4] = !T[tmaxvag4] && (MG[fc] && bewaking);     /* bewakingstijd starten       */
    if (MG[fc] && vag4)   YM[fc] |= BIT2;    /* veiligheidsgroen  activeren                 */

    va_end(argpt);
}

/* -------------------------------------------------------------------------------------------------------- */
/* Procedure inkomende pelotonkoppeling                                                                     */
/* -------------------------------------------------------------------------------------------------------- */
bool proc_pel_in_V1(                       /* Dh20130124                                                    */
	 count hfc,                            /* fasecyclus                                                   */
	 count tmeet,                          /* T meetperiode                                                 */
	 count tmaxth,                         /* T max.hiaat                                                   */
	 count grens,                          /* PRM grenswaarde                                               */
	 count mvtg,                           /* MM aantal vtg                                                 */
	 count muit,                           /* MM uitsturing aktief                                          */
	 ...)                                  /* va arg list: inkomende signalen koplussen                     */
{
	va_list argpt;     /* variabele argumentenlijst                                 */
	count hdp;
	count edpel = 0;

	/* Lezen inkomende signalen */
	va_start(argpt, muit);
	hdp = va_arg(argpt, va_count);
	do {
		if (SH[hdp] || EH[hdp]) ++edpel;
        hdp = va_arg(argpt, va_count);
	} while (hdp != END);
	va_end(argpt);

	AT[tmeet] = FALSE;
	RT[tmeet] = RT[tmaxth] = H[hfc] || !T[tmeet] && !T[tmaxth] && edpel > 0;

	if (edpel > 0 && (RT[tmeet] || T[tmeet]))
	{
		MM[mvtg] += edpel;
		RT[tmaxth] = TRUE;
	}

	if (ET[tmaxth] && !RT[tmaxth])
	{
		AT[tmeet] = TRUE;
		MM[mvtg] = 0;
	}

	if (TS && (MM[muit] > 0)) --MM[muit];
	if (MM[mvtg] >= PRM[grens])
	{
		MM[mvtg] = 0;
		MM[muit] = 3;
		AT[tmeet] = TRUE;
	}

	return (CIF_WPS[CIF_PROG_STATUS] == CIF_STAT_REG) && (MM[muit] > 0);
}

bool IsConflict(count fc1, count fc2)
{
	count i;
	for (i = 0; i < FKFC_MAX[fc1]; ++i) { /* KFC=confl.; GKFC=KFC+groenconfl.; FKFC=GKFC+fictieve confl. */
#if (CCOL_V >= 95)
		if (KF_pointer[fc1][i] == fc2) {
#else
		if (TO_pointer[fc1][i] == fc2) {
#endif
			return (bool)TRUE;
		}
	}
	return (bool)FALSE;
}

void ModuleStructuurPRM(count prmfcml, count fcfirst, count fclast, count ml_max, bool *prml[], bool yml[], count *mlx, bool *sml)
{
	if (fcfirst < fclast)
	{
		int fc, ml, fcc;
		bool PRML_x[FCMAX];        /* bijhouden toedeling */
		mulv PRML_temp[15][FCMAX]; /* tijdelijke modulemolen */

		/* bepaal nieuwe tijdelijke modulemolen, houdt toedelen bij */
		for (fc = fcfirst; fc < fclast; ++fc)
		{
			PRML_x[fc] = FALSE;
			/* Bewust niet toegedeeld */
			if (PRM[prmfcml + fc] & BIT10)
			{
				PRML_temp[ml][fc] = FALSE;
				PRML_x[fc] = TRUE;
				BL[fc] |= BIT10;
				continue;
			}
			/* ML toedeling */
			for (ml = 0; ml < ml_max; ++ml)
			{
				/* Toegedeeld aan dit blok */
				if (PRM[prmfcml + fc] & (1 << ml))
				{
					PRML_temp[ml][fc] = PRIMAIR;
					PRML_x[fc] = TRUE;
				}
				/* Niet toegedeeld aan dit blok */
				else
				{
					PRML_temp[ml][fc] = FALSE;
				}
			}
		}

		/* controleer:
		   - geen conflicterende fasen in dezelfde module van de tijdelijke modulemolen zitten
		   - geen fasen zonder toedeling
		*/
		for (fc = fcfirst; fc < fclast; ++fc)
		{
			/* Check toedeling */
			if (!PRML_x[fc])
			{
				char tmp[256];
				sprintf(tmp, "%s > Module indeling via PRMs: fase %s is niet toegedeeld.\n",
					PROMPT_code,
					FC_code[fc]);
				uber_puts(tmp);
				return;
			}
			/* Check conflicten */
			for (ml = 0; ml < ml_max; ++ml)
			{
				if (PRML_temp[ml][fc] == PRIMAIR)
				{
					for (fcc = fcfirst; fcc < fclast; ++fcc)
					{
						if (PRML_temp[ml][fcc] == PRIMAIR && IsConflict(fc, fcc))
						{
							char tmp[256];
							sprintf(tmp, "%s > Module indeling via PRMs: conflicterende fasen %s en %s zijn toegedeeld in hetzelfde blok.\n",
								PROMPT_code,
								FC_code[fc],
								FC_code[fcc]);
							uber_puts(tmp);
							return;
						}
					}
				}
			}
		}

		/* reset huidige modulemolen */
		for (fc = fcfirst; fc < fclast; ++fc)
		{
			for (ml = 0; ml < ml_max; ++ml)
			{
				prml[ml][fc] = FALSE;
			}
		}

		/* kopieer tijdelijke modulemolen naar nieuwe modulemolen */
		for (fc = fcfirst; fc < fclast; ++fc)
		{
			for (ml = 0; ml < ml_max; ++ml)
			{
				if (PRML_temp[ml][fc] == PRIMAIR) prml[ml][fc] = PRIMAIR;
			}
		}

		/* initialiseer nieuwe modulemolen */
		init_modules(ml_max, prml, yml, mlx, sml);

		{
			char tmp[256];
			sprintf(tmp, "%s > Module indeling via PRMs: succesvol toegepast.\n",
				PROMPT_code);
			uber_puts(tmp);
		}
	}
}

/**************************************************************************
 *  Functie  : seniorengroen
 *
 *  Functionele omschrijving :
 *    Houdt de richting na vastgroen een instelbaar percentage van dat
 *    vastgroen vast in wachtgroen zodat voetgangers die moeilijk ter been
 *    zijn meer gelegenheid hebben om de oversteek te maken.
 *    Zij dienen daartoe een instelbaar aantal seconden achtereen de
 *    aanvraagknop ingedrukt te houden.
 *    Er kunnen drie opeenvolgende oversteken bediend worden. Eventuele
 *    volgoversteken hebben ieder hun eigen instelling voor extra groen.

 *    Wanneer het seniorengroen aktief is, worden eventuele nalopen pas
 *    gestart op het einde (wacht)groen van de voedende richting, zodat
 *    men ook gelegenheid heeft om in trager tempo de naloop over te steken.
 *
 **************************************************************************/
void SeniorenGroen(count fc, count drk1, count drk1timer, count drk2, count drk2timer, 
	                 count exgperc, count verlengen, count meergroen, ...) 
{
 	va_list argpt;
    count tnl;
	va_start(argpt, meergroen);

	T_max[meergroen] = (TFG_max[fc] * (100 + PRM[exgperc]) / 100); 

	if (drk1 != NG) {
		if (drk1timer != NG) {
			IT[drk1timer] = SD[drk1];
			AT[drk1timer] = ED[drk1];
			if (ET[drk1timer] && D[drk1])                 IH[verlengen] |= TRUE;
		}
	}
	if (drk2 != NG) {
		if (drk2timer != NG) {
			IT[drk2timer] = SD[drk2];
			AT[drk2timer] = ED[drk2];
			if (ET[drk2timer] && D[drk2])                 IH[verlengen] |= TRUE;
		}
	}
    
	if (G[fc] && T[meergroen])                                   RW[fc] |= BIT7; 
	if (G[fc])                                            IH[verlengen] = FALSE;
	
    /* extra vasthouden in FG en WG */
    RT[meergroen] = IH[verlengen];

    /* tegenhouden start naloop tijdens FG en WG */
    while ((tnl = va_arg(argpt, va_count)) != END)
    {
        RT[tnl] |= (T[meergroen] || RT[meergroen]) && (VS[fc] || FG[fc] || WG[fc]);
    }
    va_end(argpt);
}
