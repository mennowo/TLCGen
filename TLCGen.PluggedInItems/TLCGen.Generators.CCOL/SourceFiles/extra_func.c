#include "extra_func.h"

int Knipper_1Hz = 0;

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
#ifdef CCOLTIG
			k = KF_pointer[i][n];
#else
			k = TO_pointer[i][n];
#endif
			if (RA[k] || AAPR[k])
			{
				ym = FALSE;
#ifndef NO_GGCONFLICT
#if defined CCOLTIG && !defined NO_TIGMAX
				if (TIG_max[i][k]<GK)  break;
#else
				if (TO_max[i][k]<GK)  break;
#endif
#endif
				for (j = 0; j<KFC_MAX[k]; ++j)
				{
#ifdef CCOLTIG
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
#if defined CCOLTIG && !defined NO_TIGMAX
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
				for (j = KFC_MAX[k]; j<GKFC_MAX[k]; ++j)
				{
#ifdef CCOLTIG
				
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
#if defined CCOLTIG && !defined NO_TIGMAX
					if (CV[m] && (TIG_max[i][k] <= GK))
#else
					if (CV[m] && (TO_max[i][k] <= GK))
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
#ifdef CCOLTIG
			k = KF_pointer[i][n];
#else
			k = TO_pointer[i][n];
#endif
			if (RA[k] || AAPR[k])
			{
				ym = FALSE;
#ifndef NO_GGCONFLICT
				/* if (TO_max[i][k] < GK)  break; */
#endif
				for (j = 0; j < KFC_MAX[k]; ++j)
				{
#ifdef CCOLTIG
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
					if (FC_type[m] == MVT_type)
					{
						/* bereken of max groentijd is bereikt tov benodigde
						ontruiming */
						if (CV[m] && PR[m] && !(VG[m] &&
							((TVG_max[m] - TVG_timer[m]) <
#if defined CCOLTIG && !defined NO_TIGMAX
							(TIG_max[i][k] + TGL_max[i] - TIG_max[m][k] - TGL_max[m]))))
#else
								(TO_max[i][k] + TGL_max[i] - TO_max[m][k] - TGL_max[m]))))
#endif
						{
							ym = TRUE;
							break;
						}
					}
					else
					{
						if (CV[m] && !VG[m])
						{
							ym = TRUE;
							break;
						}
					}
				}
#ifndef NO_GGCONFLICT
				for (j = KFC_MAX[k]; j < GKFC_MAX[k]; ++j)
				{
#ifdef CCOLTIG
					m = KF_pointer[k][j];
#else
					m = TO_pointer[k][j];
#endif
#if defined CCOLTIG && !defined NO_TIGMAX
					if (CV[m] && (TIG_max[i][k] <= GK))
#else
					if (CV[m] && (TO_max[i][k] <= GK))
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
#if !defined (CCOLFUNC) || defined (STDFUNC32)
bool ym_max_toV1(count i, mulv to_verschil)
{
   register count n, j, k, m;
   bool ym;
   if (MG[i]) {     /* let op! i.v.m. snelheid alleen in MG[] behandeld   */
      ym= TRUE;
#ifndef NO_GGCONFLICT
      for (n=0; n<GKFC_MAX[i]; n++) {
#else
      for (n=0; n<KFC_MAX[i]; n++) {
#endif
#ifdef CCOLTIG
		  k = KF_pointer[i][n];
#else
		  k = TO_pointer[i][n];
#endif
		  if (RA[k]||AAPR[k]) {
           ym= FALSE;
#ifndef NO_GGCONFLICT
#if defined CCOLTIG && !defined NO_TIGMAX
		   if (TIG_max[i][k] < GK)  break;
#else
		   if (TO_max[i][k]<GK)  break;
#endif
#endif
           for (j=0; j<KFC_MAX[k]; j++) {
#ifdef CCOLTIG
			   m = KF_pointer[k][j];
#else
			   m= TO_pointer[k][j];
#endif
#if defined CCOLTIG && !defined NO_TIGMAX
			   if (CV[m] && (((TGL_max[i]+TIG_max[i][k]-to_verschil)
                       <= (TGL_max[m]+TIG_max[m][k]))
                && ( (TGL_max[i]+TIG_max[i][k]) < (TFG_max[m]-TFG_timer[m]
                    +TVG_max[m]-TVG_timer[m] + TGL_max[m]-TGL_timer[m]
                    +TIG_max[m][k]-TIG_timer[m]) )
                || (to_verschil<0))
                || TIG[m][k]
                && ((TGL_max[i]+TIG_max[i][k]) < (TGL_max[m]+TO_max[m][k]
                  -TGL_timer[m]-TIG_timer[m]) ) ) {
#else
			   if (CV[m] && (((TGL_max[i] + TO_max[i][k] - to_verschil)
				   <= (TGL_max[m] + TO_max[m][k]))
				   && ((TGL_max[i] + TO_max[i][k]) < (TFG_max[m] - TFG_timer[m]
					   + TVG_max[m] - TVG_timer[m] + TGL_max[m] - TGL_timer[m]
					   + TO_max[m][k] - TO_timer[m]))
				   || (to_verschil < 0))
				   || TO[m][k]
				   && ((TGL_max[i] + TO_max[i][k]) < (TGL_max[m] + TO_max[m][k]
					   - TGL_timer[m] - TO_timer[m]))) {
#endif
               ym= TRUE;
               break;
              }
           }
#ifndef NO_GGCONFLICT
           for (j=KFC_MAX[k]; j<GKFC_MAX[k]; j++) {
#ifdef CCOLTIG
			   m = KF_pointer[k][j];
#else
			   m = TO_pointer[k][j];
#endif
#if defined CCOLTIG && !defined NO_TIGMAX
				   if (CV[m] && (TIG_max[i][k]<=GK)) {
#else
			   if (CV[m] && (TO_max[i][k] <= GK)) {
#endif
               ym= TRUE;
               break;
              }
           }
#endif
       }
       if (!ym) break;
      }
   }
   else  ym= CV[i];
   return ym;
}
#endif

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

    /* verzorgen naloop rateltikker */
    RT[tnlrt] = EGL[fc] && IH[has] || EH[has_cont_];
    
    /* afzetten rateltikker na aflopen naloop timer en niet continu */
    if (ET[tnlrt])
    {
        /* Check op continue aansturing */
		if (has_cont_ == NG || !IH[has_cont_])      IH[has] = FALSE;
    }
    
    /* continue aansturing rateltikkers */
    if (has_cont_ > NG)                             IH[has] |= IH[has_cont_];
    
    /* check tikkers werking */
    if(IH[has_aan_])
    {
		va_start(argpt, tnlrt);
		while ((hdkh = va_arg(argpt, va_count)) != END)
		{
			/* opzetten rateltikkers bij detectie drukknoppen */
			IH[has] |= IH[hdkh];
		}
		va_end(argpt);
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
			IH[has] |= D[dkid];
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
                        int nogtedoseren[])       /* pointer naar array met nog te doseren waarden     */
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
        if((nogtedoseren[i] && EG[fc[i]] && PR[fc[i]] && !H[hfile]) || (EG[fc[i]] && H[hfile] && !MK[fc[i]]) || nogtedoseren[i] && !A[fc[i]] && PRML[ML][fc[i]] == PRIMAIR)
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
                        int nogtedoseren[])             /* pointer naar array met nog te doseren waarden     */
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
        if((nogtedoseren[i] && EG[fc[i]] && PR[fc[i]] && !H[hfile]) || (EG[fc[i]] && H[hfile] && !MK[fc[i]]) || nogtedoseren[i] && !A[fc[i]] && PRML[ML][fc[i]] == PRIMAIR)
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

void wachttijd_leds_knip(count fc, count mmwtv, count mmwtm, count RR_T_wacht)
{
	/* fc    - fasecyclusnummer                            */
	/* mmwtv - berekende  aantal leds wachttijdlantaarn    */
	/* mmwtm - uitsturing aantal leds wachttijdlantaarn    */

	if ((CIF_IS[isfix] || (RR_T_wacht > 0)) && MM[mmwtm])         /* fixatie of prio-ingreep terwijl wachttijdlantaarn aan staat  */
	{
		if (MM[mmwtm] > 1)       MM[mmwtm] = MM[mmwtv] - Knipper_1Hz;     /* bij aantal leds groter dan 1 = actuele ledje laten knipperen */
		else                     MM[mmwtm] = MM[mmwtv] + Knipper_1Hz;     /* bij één leds de voorgaande led, zodat de wtv niet uitgaat    */
	}
	else                        MM[mmwtm] = MM[mmwtv];            /* anders berekende aantal leds gewoon overnemen                */
}