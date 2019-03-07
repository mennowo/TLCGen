#include <stdarg.h>

#include "ccolfunc.h"

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)
   #include <conio.h>
#endif

#include "nalopen.c"
#include "gkvar.c"    /* groengroenkonflikten              */
#include "nlvar.c"    /* nalopen                           */

#ifdef SYNC
   #include "syncfunc.c"
#endif

#include "uitstuur.c"
#include "fixatie.c"


#define TAMAX  32000      /* Maximaal aantal tijdelijke aanvragen op een fasecyclus     */

mulv TA[FCMAX];           /* Tijdelijke aanvraag (kan tijdens geel/rood worden gereset) */
mulv TMA[FCMAX][FCMAX];   /* Tijdelijke meeaanvraag van fcxx naar fcyy                  */

/**************************************************************************
 *  Functie  : aanvraag_detectie_reset_prm_va_arg
 *
 *  Functionele omschrijving :
 *    Deze functie is afgeleid van aanvraag_detectie_prm_va_arg() in
 *    STDFUNC maar reset de aanvraag als de detectielus een instelbare tijd
 *    onbezet is.
 **************************************************************************/
void aanvraag_detectie_reset_prm_va_arg(count fc, ...)
{
    va_list argpt;                       /* variabele argumentenlijst    */
    count dpnr,                          /* arraynummer detectie-element */
        tav;                           /* arraynummer tijdelement      */
    mulv  prm;                           /* aanvraag parameter           */

    va_start(argpt, fc);
    do {
        dpnr = va_arg(argpt, va_count); /* lees array-nummer detecctie */
        if (dpnr >= 0)
        {
            tav = va_arg(argpt, va_count); /* lees array-nummer tijdelement */
            if (tav >= 0)
            {
                prm = va_arg(argpt, va_mulv); /* lees waarde parameter */
                if (prm > END)
                {
                    if (prm <= 0)
                    {
                        RT[tav] = FALSE;
                    }
                    if (prm == 1)
                    {
                        RT[tav] = R[fc] && !TRG[fc] && DB[dpnr] ? TRUE : T[tav] && !G[fc] && D[dpnr];
                    }
                    if (prm == 2)
                    {
                        RT[tav] = R[fc] && DB[dpnr] ? TRUE : T[tav] && !G[fc] && D[dpnr];
                    }
                    if (prm >= 3)
                    {
                        RT[tav] = (R[fc] || GL[fc]) && DB[dpnr] ? TRUE : T[tav] && !G[fc] && D[dpnr];
                    }
                    AT[tav] = G[fc];
                    if (RT[tav] && !T[tav] && (TA[fc] < TAMAX)) TA[fc]++; /* optellen tijdelijke aanvragen bij start timer */
                    if (ET[tav] && !AT[tav] && (TA[fc] > 0))    TA[fc]--; /* aftellen tijdelijke aanvragen bij einde timer */
                    if (EG[fc])                               TA[fc] = 0; /* altijd reset bij eindgroen (net als A)        */
                }
            }
        }
    } while ((dpnr >= 0) && (tav >= 0) && (prm > END));
    if (TA[fc])
    {
        /* set tijdelijke aanvraag */
        A[fc] |= BIT8;
    }
    if (!TA[fc] && (A[fc] & BIT8))
    {     
        /* reset tijdelijke aanvraag */
        A[fc] &= ~BIT8;
        if (!A[fc])
        {
            TFB_timer[fc] = 0;
            RR[fc] |= RA[fc] ? BIT8 : 0;
        }
    }
    va_end(argpt); /* maak var. arg-lijst leeg */
}

/**************************************************************************
 *  Functie  : aanvraag_richtinggevoelig_reset
 *
 *  Functionele omschrijving :
 *    Deze functie is afgeleid van aanvraag_richtinggevoelig() in STDFUNC
 *    maar reset de aanvraag als het richtinggevoelige lussenpaar een
 *    instelbare tijd onbezet is.
 **************************************************************************/
void aanvraag_richtinggevoelig_reset(count fc, count d1, count d2, count trga, count tav, mulv schrga)
{
    if ((trga >= 0) && (schrga > 0))
    {
        RT[trga] = SD[d2];
        if (tav >= 0)
        {
            RT[tav] = R[fc] && !TRG[fc] && SD[d1] && T[trga] ? TRUE : T[tav] && !G[fc] && (D[d1] || D[d2]);
            AT[tav] = G[fc];
        }
        /* snelle variant */
        if (R[fc] && !TRG[fc] && SD[d1] && T[trga])
        { 
            if (tav < 0)
            {
                A[fc] |= BIT1;
            }
        }
    }
    else
    {
        if (tav >= 0)
        {
            RT[tav] = R[fc] && !TRG[fc] && SD[d1] && D[d2] ? TRUE : T[tav] && !G[fc] && (D[d1] || D[d2]);
            AT[tav] = G[fc];
        }
        /* zekere variant   */
        if (R[fc] && !TRG[fc] && SD[d1] && D[d2])
        {
            if (tav < 0)
            {
                A[fc] |= BIT1;
            }
        }
    }
    if (tav >= 0)
    {
        if (RT[tav] && !T[tav] && (TA[fc] < TAMAX)) TA[fc]++; /* optellen tijdelijke aanvragen bij start timer */
        if (ET[tav] && !AT[tav] && (TA[fc] > 0))    TA[fc]--; /* aftellen tijdelijke aanvragen bij einde timer */
        if (EG[fc])                               TA[fc] = 0; /* altijd reset bij eindgroen (net als A)        */
        if (TA[fc])
        {
            /* set tijdelijke aanvraag */
            A[fc] |= BIT8;
        }
        if (!TA[fc] && (A[fc] & BIT8))
        {
            /* reset tijdelijke aanvraag */
            A[fc] &= ~BIT8;
            if (!A[fc]) {
                TFB_timer[fc] = 0;
                RR[fc] |= RA[fc] ? BIT8 : 0;
            }
        }
    }
}

#ifndef INSTRUCTION_BITS8
/**************************************************************************
 *  Functie  : mee_aanvraag_reset
 *
 *  Functionele omschrijving :
 *    zet een meeaanvraag door van fcv naar fcn
 *    en reset deze als deze wordt veroorzaakt door tijdelijke aanvragen
 *    die allen ingetrokken worden.
 **************************************************************************/
void mee_aanvraag_reset(count fcn, count fcv, bool expressie)
{
    register count fc;
    bool reset_A = TRUE;

    /* 'normale' meeaanvraag */
    if (expressie && (A[fcv] & ~(BIT7 | BIT8)))
    {
        A[fcn] |= BIT4;
    }

    /* tijdelijke meeaanvraag */
    if (expressie && (A[fcv] & BIT7))
    {
        A[fcn] |= BIT8;
    }

    TMA[fcv][fcn] = TA[fcv]; /* onthoud of de aanvragende fc nog een TA heeft */

    for (fc = 0; fc < FCMAX; ++fc)
    {
        if (TMA[fc][fcn]) /* er is nog een aanvragende fc met een TA */
        {
            reset_A = FALSE;
            break;
        }
    }
    if ((reset_A) && (A[fcn] & BIT8)) /* alle TA's van aanvragende fc's zijn ingetrokken */
    {
        /* -> reset tijdelijke meeaanvraag */
        A[fcn] &= ~BIT8;
        if (!A[fcn])
        {
            TFB_timer[fcn] = 0;
            RR[fcn] |= RA[fcn] ? BIT7 : 0;
        }
    }
}

#endif

#if MLMAX
/**************************************************************************
 *  Functie  : WStandRi
 *
 *  Functionele omschrijving :
 *    Bepaalt WS[] van een wachtstand-rood richting
 *    Wordt gebruikt door de functie WachtStand
 **************************************************************************/
bool WStandRi(count fci, bool *prml[], count ml, count ml_max)
{
    register count hml, hfci;

    if (WG[fci])
    {
        for (hfci = 0; hfci < FC_MAX; ++hfci)
        {
            if (hfci != fci)
            {
                for (hml = 0; hml < ml_max; ++hml)
                {
                    if (hml != ml)
                    {
                        if ((prml[hml][hfci] & PRIMAIR) && /* Aanvraag of realisatie in ander module */
                            (R[hfci] && A[hfci] || AR[hfci] && G[hfci] || PG[hfci] & VERSNELD_PRIMAIR))
                            return (!ka(fci) && WG[fci]);
                    }
                }
                if ((prml[ml][hfci] & PRIMAIR) && PG[hfci] &&
                    R[hfci] && A[hfci]) /* Aanvraag uit zelfde module, maar al */
                {
                    /* eerder primair gerealiseerd */
                    return (!ka(fci) && WG[fci]);
                }
            }
        }
    }
    return FALSE;
}

/**************************************************************************
 *  Functie  : WachtStand
 *
 *  Functionele omschrijving :
 *    Bepaalt WS[] van alle richtingen (wachtstand-rood)
 **************************************************************************/
void WachtStand(bool *prml[], count ml, count ml_max)
{
    register count fci;

    for (fci = 0;fci < FC_MAX; ++fci)
    {
        WS[fci]&=~BIT0;
		WS[fci] |= WStandRi(fci, prml, ml, ml_max)? BIT0 : 0;
    }
}

#endif

/* MAXIMUM REALISATIETIJD VOOR EEN ALTERNATIEVE REALISATIE */
/* ======================================================= */

/* max_tar_to() kan worden gebruikt bij de specificatie van de instructie-
 * variabele PAR[] (periode alternatieve realisatie) van de fasecyclus.
 * max_tar_to() geeft de maximum tijd in tienden van seconden, die beschikbaar
 * is voor het groen van een alternatieve realisatie van de fasecyclus.
 * het aanroepen van de functie max_tar_to() dient in de applicatiefunctie
 * application() te worden gespecificeerd.
 * de beschikbare tijd voor het groen van een alternatieve realisatie van
 * de fasecyclus wordt als volgt bepaald:
 *  . indien er geen (fictief) conficterende fasecycli met een aanvraag
 *    ( !fka(k) ) aanwezig zijn, wordt een maximale tijd voor alternatieve
 *    realisatie van de fasecyclus (300 seconden) ingesteld.
 *  . indien er een (fictief) conflicterende primaire fasecyclus met een
 *    aanvraag in een volgende module aanwezig is die primair mag worden
 *    gerealiseerd (AAPR[k] is waar), dan wordt de beschikbare tijd voor
 *    het groen van een alternatieve realisatie van de fasecyclus bepaald,
 *    afhankelijk van de resterende (verleng)groentijden in de schaduw van de actieve
 *    primaire fasecycli t.o.v. de beschouwde (fictief) conflicterende
 *    fasecyclus vermindert met de tijd van eventueel aanwezige conflictfasen. Tevens
 *    wordt rekening gehouden met het verschil in geeltijd tussen de primaire en
 *    alternatieve richting en met het verschil in ontruiming van de primaire en
 *    alternatieve richting naar de richting met de AAPR.
 *  . indien er geen (fictief) conflicterende primaire fasecyclus in een
 *    volgende module met een aanvraag aanwezig is die versneld mag worden
 *    gerealiseerd (AAPR[k]'s zijn niet waar) dan wordt de beschikbare tijd
 *    voor alternatieve realisatie van de fasecyclus bepaald door de
 *    grootste resterende (verleng)groentijd van een actieve primaire
 *    fasecyclus vermindert met de tijd van eventueel aanwezige conflictfasen.
 *
 * voorbeeld gebruik:
 *   PAR[fc01]= (max_tar_to(fc01)>=PRM[prmaltmax01]) && SCH[schar01];
 *
 */

#if !defined (CCOLFUNC) || defined (LWFUNC12)

#include "lwvar.h"
#include "fcvar.h"
#include "kfvar.h"

#if !defined CCOLTIG || defined NO_TIGMAX

mulv max_tar_to(count i)
{
    register count n, k, nn, kk;
#ifndef NO_CHNG20021126
    mulv totxb_min = 0, totxb_min_tmp = 3000, totxb_tmp;
#else
    mulv totxb_min = 0, totxb_tmp;
#endif
    mulv to_max = 0, to_tmp;
    mulv t_aa_max = 0;
    mulv aapr_tmp = 0;

    if (!kcv(i))                    /* let op! i.v.m. snelheid            */
    {
        t_aa_max = 0;                  /* alleen tijdens !kcv behandeld      */
        totxb_min = 0;
        totxb_tmp = 0;
        to_max = to_tmp = 0;

        if (!fka(i))                  /* geen (fictieve) conflictaanvragen  */
        {
            t_aa_max = 3000;             /* maximale realisatietijd            */
            return((mulv)t_aa_max);
        }
        for (n = 0; n < FKFC_MAX[i]; n++)
        {
#ifdef CCOLTIG
			k = KF_pointer[i][n];
#else
			k = TO_pointer[i][n];
#endif
            if (TO[k][i])               /* zoek grootste ontruimingstijd      */
            {
                to_tmp = TGL_max[k] + TO_max[k][i] - TGL_timer[k] - TO_timer[k];
                if (to_tmp > to_max)
                    to_max = to_tmp;
            }
            if (AAPR[k] && !PAR[k])                /* zoek maximale realisatietijd       */
            {
                aapr_tmp = TRUE;           /* t.o.v. te realiseren conflicten    */
#ifndef NO_CHNG20021126
                totxb_min = totxb_tmp = 0;
#else
                totxb_tmp = 0;
#endif
                for (nn = 0; nn < GKFC_MAX[k]; nn++)
                {
#ifdef CCOLTIG
					kk = KF_pointer[k][nn];
#else
					kk = TO_pointer[k][nn];
#endif
                    if ((kk != i) && PR[kk])/* test alleen de primaire richtingen */
                    {
                        if (G[kk] && !MG[kk] && !FM[kk] && !Z[kk])
                        {
                            totxb_tmp = TFG_max[kk] + TVG_max[kk] + TGL_max[kk] + TO_max[kk][k] - TFG_timer[kk] - TVG_timer[kk] - TGL_max[i] - TO_max[i][k];
                            if (totxb_tmp > totxb_min)
                            {
                                totxb_min = totxb_tmp;
                            }
                        }
                        else if (RA[kk] && !RR[kk] && !BL[kk])
                        {
                            totxb_tmp = TFG_max[kk] + TVG_max[kk] + TGL_max[kk] + TO_max[kk][k] - TGL_max[i] - TO_max[i][k];
                            if (totxb_tmp > totxb_min)
                            {
                                totxb_min = totxb_tmp;
                            }
                        }
                    }
                }
#ifndef NO_CHNG20021126
                if (totxb_min < totxb_min_tmp) totxb_min_tmp = totxb_min;
#endif
            }
        }
        if (t_aa_max >= 0)              /* bereken de realisatietijd          */
        {
            if (aapr_tmp)               /* primaire conflict aanvraag         */
#ifndef NO_CHNG20021126
                t_aa_max = totxb_min_tmp - to_max;
#else
                t_aa_max = totxb_min - to_max;
#endif
            else /* geen primaire conflict aanvraag */
            {
                totxb_tmp = 0;
                for (n = 0; n < FC_MAX; n++)  /* zoek langste primaire groentijd */
                {
                    if ((n != i) && (PR[n])) /* test alleen de primaire richtingen */
                    {
                        if (G[n] && !MG[n] && !FM[n] && !Z[n])
                        {
                            totxb_tmp = TFG_max[n] + TVG_max[n] + TGL_max[n]
                                - TFG_timer[n] - TVG_timer[n] - TGL_max[i];
                            if (totxb_tmp > totxb_min)
                                totxb_min = totxb_tmp;
                        }
                        else if (RA[n] && !RR[n] && !BL[n])
                        {
                            totxb_tmp = TFG_max[n] + TVG_max[n] + TGL_max[n] - TGL_max[i];
                            if (totxb_tmp > totxb_min)
                                totxb_min = totxb_tmp;
                        }
                    }
                }
                t_aa_max = totxb_min - to_max;
            }
        }
    }
    else  t_aa_max = NG;

#ifdef TEST_TAR
    if (i == fc06)
    {
        gotoxy(1, 1);
        printf("TAR_MAX01= %d    ", t_aa_max);
        gotoxy(1, 2);
        printf("totxb_min= %d    ", totxb_min);
        gotoxy(1, 3);
        printf("to_max= %d    ", to_max);
    }
#endif
    return t_aa_max;
}

#else

mulv max_tar_tig(count i)
{
    register count n, k, nn, kk;
#ifndef NO_CHNG20021126
    mulv totxb_min = 0, totxb_min_tmp = 3000, totxb_tmp;
#else
    mulv totxb_min = 0, totxb_tmp;
#endif
    mulv to_max = 0, to_tmp;
    mulv t_aa_max = 0;
    mulv aapr_tmp = 0;

    if (!kcv(i))                    /* let op! i.v.m. snelheid            */
    {
        t_aa_max = 0;                  /* alleen tijdens !kcv behandeld      */
        totxb_min = 0;
        totxb_tmp = 0;
        to_max = to_tmp = 0;

        if (!fka(i))                  /* geen (fictieve) conflictaanvragen  */
        {
            t_aa_max = 3000;             /* maximale realisatietijd            */
            return((mulv)t_aa_max);
        }
        for (n = 0; n < FKFC_MAX[i]; n++)
        {
            k = KF_pointer[i][n];
            if (TIG[k][i])               /* zoek grootste ontruimingstijd      */
            {
                to_tmp = TGL_max[k] + TIG_max[k][i] - TGL_timer[k] - TIG_timer[k];
                if (to_tmp > to_max)
                    to_max = to_tmp;
            }
            if (AAPR[k])                /* zoek maximale realisatietijd       */
            {
                aapr_tmp = TRUE;           /* t.o.v. te realiseren conflicten    */
#ifndef NO_CHNG20021126
                totxb_min = totxb_tmp = 0;
#else
                totxb_tmp = 0;
#endif
                for (nn = 0; nn < GKFC_MAX[k]; nn++)
                {
                    kk = KF_pointer[k][nn];
                    if ((kk != i) && PR[kk])/* test alleen de primaire richtingen */
                    {
                        if (G[kk] && !MG[kk] && !FM[kk] && !Z[kk])
                        {
                            totxb_tmp = TFG_max[kk] + TVG_max[kk] + TGL_max[kk] + TIG_max[kk][k] - TFG_timer[kk] - TVG_timer[kk] - TGL_max[i] - TIG_max[i][k];
                            if (totxb_tmp > totxb_min)
                            {
                                totxb_min = totxb_tmp;
                            }
                        }
                        else if (RA[kk] && !RR[kk] && !BL[kk])
                        {
                            totxb_tmp = TFG_max[kk] + TVG_max[kk] + TGL_max[kk] + TIG_max[kk][k] - TGL_max[i] - TIG_max[i][k];
                            if (totxb_tmp > totxb_min)
                            {
                                totxb_min = totxb_tmp;
                            }
                        }
                    }
                }
#ifndef NO_CHNG20021126
                if (totxb_min < totxb_min_tmp) totxb_min_tmp = totxb_min;
#endif
            }
            }
        if (t_aa_max >= 0)              /* bereken de realisatietijd          */
        {
            if (aapr_tmp)               /* primaire conflict aanvraag         */
#ifndef NO_CHNG20021126
                t_aa_max = totxb_min_tmp - to_max;
#else
                t_aa_max = totxb_min - to_max;
#endif
            else /* geen primaire conflict aanvraag */
            {
                totxb_tmp = 0;
                for (n = 0; n < FC_MAX; n++)  /* zoek langste primaire groentijd */
                {
                    if ((n != i) && (PR[n])) /* test alleen de primaire richtingen */
                    {
                        if (G[n] && !MG[n] && !FM[n] && !Z[n])
                        {
                            totxb_tmp = TFG_max[n] + TVG_max[n] + TGL_max[n]
                                - TFG_timer[n] - TVG_timer[n] - TGL_max[i];
                            if (totxb_tmp > totxb_min)
                                totxb_min = totxb_tmp;
                        }
                        else if (RA[n] && !RR[n] && !BL[n])
                        {
                            totxb_tmp = TFG_max[n] + TVG_max[n] + TGL_max[n] - TGL_max[i];
                            if (totxb_tmp > totxb_min)
                                totxb_min = totxb_tmp;
                        }
                    }
                }
                t_aa_max = totxb_min - to_max;
            }
        }
        }
    else  t_aa_max = NG;

#ifdef TEST_TAR
    if (i == fc06)
    {
        gotoxy(1, 1);
        printf("TAR_MAX01= %d    ", t_aa_max);
        gotoxy(1, 2);
        printf("totxb_min= %d    ", totxb_min);
        gotoxy(1, 3);
        printf("to_max= %d    ", to_max);
    }
#endif
    return t_aa_max;
    }

#endif /* NO_TIGMAX */

#endif


/* MAXIMUM REALISATIETIJD VOOR EEN ALTERNATIEVE REALISATIE ONDER OV-INGREEP */
/* ======================================================================== */

/* max_tar_ov() kan worden gebruikt bij de specificatie van de instructie-
 * variabele PAR[] (periode alternatieve realisatie) van de fasecyclus.
 * max_tar_to() geeft de maximum tijd in tienden van seconden, die beschikbaar
 * is voor het groen van een alternatieve realisatie van de fasecyclus.
 * het aanroepen van de functie max_tar_to() dient in de applicatiefunctie
 * application() te worden gespecificeerd.
 * de beschikbare tijd voor het groen van een alternatieve realisatie van
 * de fasecyclus wordt als volgt bepaald:
 *  . indien er geen (fictief) conficterende fasecycli met een aanvraag
 *    ( !fka(k) ) aanwezig zijn, wordt een maximale tijd voor alternatieve
 *    realisatie van de fasecyclus (300 seconden) ingesteld.
 *  . indien er een (fictief) conflicterende primaire fasecyclus met een
 *    aanvraag in een volgende module aanwezig is die primair mag worden
 *    gerealiseerd (AAPR[k] is waar), dan wordt de beschikbare tijd voor
 *    het groen van een alternatieve realisatie van de fasecyclus bepaald,
 *    afhankelijk van de resterende (verleng)groentijden in de schaduw van de actieve
 *    OV-fasecycli t.o.v. de beschouwde (fictief) conflicterende
 *    fasecyclus vermindert met de tijd van eventueel aanwezige conflictfasen. Tevens
 *    wordt rekening gehouden met het verschil in geeltijd tussen de OV- en
 *    alternatieve richting en met het verschil in ontruiming van de OV- en
 *    alternatieve richting naar de richting met de AAPR.
 *  . indien er geen (fictief) conflicterende primaire fasecyclus in een
 *    volgende module met een aanvraag aanwezig is die versneld mag worden
 *    gerealiseerd (AAPR[k]'s zijn niet waar) dan wordt de beschikbare tijd
 *    voor alternatieve realisatie van de fasecyclus bepaald door de
 *    grootste resterende (verleng)groentijd van een actieve OV-
 *    fasecyclus vermindert met de tijd van eventueel aanwezige conflictfasen.
 *
 * voorbeeld gebruik:
 *   PAR[fc01]= (max_tar_ov(fc01)>=PRM[prmaltmax01]) && SCH[schar01];
 *
 */
mulv max_tar_ov(count i, ...)            /* i=alt.ri.                        */
{
    /* variabele argumentenlijst */
    va_list argpt;

    /* ov=ov-richting vb=vecombewakingstijd h=ov-hulpelement */
    register count k, n = 0, ov, vb, h;
    mulv totxb_min = 0, totxb_tmp;
    mulv to_max = 0, to_tmp;
    mulv t_aa_max = 0;
    mulv t_aa_max_max = 0;
    mulv aapr_tmp = 0;

    va_start(argpt, i); /* start var. argumentenlijst */
    do
    {
        ov = va_arg(argpt, va_count); /* lees array-nummer ov-fc */
        if (ov >= 0)
        {
            vb = va_arg(argpt, va_count); /* lees waarde vb */
            if (vb >= 0)
            {
                h = va_arg(argpt, va_count); /* lees waarde h */
#if defined CCOLTIG && !defined NO_TIGMAX
                if (h >= 0 && IH[h] && (TIG_max[ov][i] == NG) && T[vb])
#else
                if (h >= 0 && IH[h] && (TO_max[ov][i] == NG) && T[vb])
#endif
                {
                    if (!kcv(i)||TRUE) /* let op! i.v.m. snelheid alleen tijdens !kcv behandeld */
                    {
                        t_aa_max = 0;
                        totxb_min = 0;
                        totxb_tmp = 0;
                        to_max = to_tmp = 0;

                        if (!fka(i)) /* geen (fictieve) conflictaanvragen */
                        {
                            t_aa_max = 3000; /* maximale realisatietijd */
                            return((mulv)t_aa_max);
                        }
                        for (n = 0; n < FKFC_MAX[i]; ++n)
                        {
#ifdef CCOLTIG
                            k = KF_pointer[i][n];
#else
                            k = TO_pointer[i][n];
#endif
#if defined CCOLTIG && !defined NO_TIGMAX
                            if (TIG[k][i])               /* zoek grootste ontruimingstijd      */
                            {
                                to_tmp = TIG_max[k][i] - TIG_timer[k];
#else
                            if (TO[k][i])               /* zoek grootste ontruimingstijd      */
                            {
                                to_tmp = TGL_max[k] + TO_max[k][i] - TGL_timer[k] - TO_timer[k];
#endif
                                if (to_tmp > to_max)
                                    to_max = to_tmp;
                            }
                            if (AAPR[k])                /* zoek maximale realisatietijd       */
                            {
                                aapr_tmp = TRUE;           /* t.o.v. te realiseren conflicten    */
                                totxb_tmp = 0;
                                if (G[ov])
                                {
#if defined CCOLTIG && !defined NO_TIGMAX
                                    totxb_tmp = T_max[vb] + TIG_max[ov][k] - T_timer[vb] - TIG_max[i][k];
#else
                                    totxb_tmp = T_max[vb] + TGL_max[ov] + TO_max[ov][k] - T_timer[vb] - TGL_max[i] - TO_max[i][k];
#endif
                                    if (totxb_tmp > totxb_min)
                                    {
                                        totxb_min = totxb_tmp;
                                    }
                                }
                                else if (RA[ov] && !RR[ov] && !BL[ov])
                                {
#if defined CCOLTIG && !defined NO_TIGMAX
                                    totxb_tmp = T_max[vb] + TIG_max[ov][k] - TIG_max[i][k];
#else
                                    totxb_tmp = T_max[vb] + TGL_max[ov] + TO_max[ov][k] - TGL_max[i] - TO_max[i][k];
#endif
                                    if (totxb_tmp > totxb_min)
                                    {
                                        totxb_min = totxb_tmp;
                                    }
                                }
                            }
                        }

                        if (t_aa_max >= 0) /* bereken de realisatietijd */
                        {
                            if (aapr_tmp) /* primaire conflict aanvraag */
                                t_aa_max = totxb_min - to_max;
                            else /* geen primaire conflict aanvraag */
                            {
                                totxb_tmp = 0;
                                if (G[ov])
                                {
                                    totxb_tmp = T_max[vb] + TGL_max[ov] - T_timer[vb] - TGL_max[i];
                                    if (totxb_tmp > totxb_min)
                                    {
                                        totxb_min = totxb_tmp;
                                    }
                                }
                                else if (RA[ov] && !RR[ov] && !BL[ov])
                                {
                                    totxb_tmp = T_max[vb] + TGL_max[ov] - TGL_max[i];
                                    if (totxb_tmp > totxb_min)
                                    {
                                        totxb_min = totxb_tmp;
                                    }
                                }

                                t_aa_max = totxb_min - to_max;
                            }
                        }
                        else
                        {
                            t_aa_max = NG;
                        }

                        if (t_aa_max > t_aa_max_max)
                        {
                            t_aa_max_max = t_aa_max;
                        }
                    }
                }
            }
        }
    } while ((ov >= 0) && (vb >= 0) && (h >= 0));
    va_end(argpt); /* maak var. arg-lijst leeg */

    return t_aa_max_max;
}

#ifdef __BORLANDC__
#pragma argsused
#endif

/**************************************************************************
 *  Functie  : AlternatieveRuimte
 *
 *  Functionele omschrijving :
 *      Deze functie bepaalt of de alternatieve richting in ieder geval
 *      zijn minimale alternatieve groentijd kan maken tijdens het
 *      resterende (verleng)groen van de primaire richting. De bool
 *              AlternatieveRuimte moet als voorwaarde worden opgenomen
 *              in de functies set_ARML() en YML[].
 *
 *  Parameters:
 *  - fcalt     alternatieve richting
 *  - fcprim    primaire richting in wiens schaduw fcalt mag komen
 *  - paltg         minimaal gewenste alternatieve groentijd
 **************************************************************************/
bool AlternatieveRuimte(count fcalt, count fcprim, count paltg)
{
    return (TVG_max[fcprim] - TVG_timer[fcprim] >= PRM[paltg]);
}
bool no_conflict(count fc1par, count fc2ov)
{
	count l, c;
	if (fc1par == fc2ov) return (FALSE);
	else
	{
		for (l = 0; l < GKFC_MAX[fc1par]; ++l) 
		{
#ifdef CCOLTIG
			c = KF_pointer[fc2ov][l];
#else
			c = TO_pointer[fc2ov][l];
#endif
			if (c == fc1par) return(FALSE);
		}
	}
	return(TRUE);
}
#if !defined (CCOLFUNC) || defined (LWMLFUNC7)

bool testpri_gk_calw(count i)
{
    register count n, j;

    for (n = 0; n < GKFC_MAX[i]; ++n)
	{
#ifdef CCOLTIG
	   j = KF_pointer[i][n];
#else
	   j = TO_pointer[i][n];
#endif
       if ((R[j] || GL[j]) && !RR[j] && !BL[j] && (/* AA[j] && !AR[j]
					      || */ (CALW[j]>CALW[i])))
	   return FALSE;
		  /* test of !AR[j] i.v.m. het mogelijk tegenhouden  (RR[]) */
		  /* alternatieve realisaties mbv rr_ar_fkpr()              */
    }              /* de vraag is of hier wel op RR[] moet worden getest     */
		  /* indien slechts een tijdelijke volgorde van afwikkeling */
    return TRUE; /* m.b.v. RR[] moet worden beheerst			    */
}



/* SET prioriteitsrealisatie */
/* ========================= */

/* set_PRIRLW() is een variant op set_PRILW() in LWMLFUNC.C
 * in plaats van de functies testpri_fk_calw() en  fkaa() worden de functies testpri_gk_calw(i) en kcv(i) aangeroepen
 * dit is gedaan om te voorkomen dat een prioriteitsrealisatie ten onrechte wordt tegengehouden, als een fictief conflict in RA staat
 *
 * set_PRIRLW() sets AA[] and PR[] als een primaire prioriteitsrealisatie met BIT6
 * set_PRIRLW() returns TRUE, als het AAPR_bit wordt geset, anders FALSE.
 * set_PRIRLW() roept testpri_gk_calw() aan en  kcv().
 * set_PRIRLW() wordt aangeroepen in de procedure OVBijzonderRealiseren() in ov.c
 */
bool set_PRIRLW(count i, bool period)
{
   if (AAPR[i] && !AA[i])  AAPR[i] = FALSE;
   if (!AA[i] && period && (CALW[i] >= PRI_CALW) && A[i] && RV[i]
	   && !TRG[i] && !RR[i] && !BL[i] && testpri_gk_calw(i)) 
   {
	   AAPR[i] = FALSE;
	   if (kcv(i)) AAPR[i] |= BIT6; /* prioriteit! */
	   if (!AAPR[i])
	   {
		   AAPR[i] = TRUE;
		   AAPR[i] |= BIT6;
		   AA[i] = TRUE;		/* set actuation */
		   PR[i] = BIT6;
	   }
	   return  (TRUE);
   }
   return (FALSE);
}
#endif
/* SET ALTERNATIVE REALIZATION BIT6 BIJ OV-MEEREALISATIE */
/* ===================================================== */

/* set_ARLW_bit6() sets an alternative realization BIT6 verder idem lwfunc.c 
 */

#if !defined (CCOLFUNC) || defined (LWFUNC4)

bool set_ARLW_bit6 (count i)
{
   if (PAR[i] && A[i] && RV[i] && !TRG[i] && !AA[i] && !RR[i]
	   && !BL[i] && !kcv(i) && !fkaa(i) && testar_fk_calw(i))
   {
      AA[i] = TRUE;		/* set actuation		*/
      AR[i] = BIT6;
      return TRUE;
   }
   return FALSE;
}

void langstwachtende_alternatief_bit6(void)
{
   register count i;

   for (i=0; i<FC_MAX; i++)
   {
      set_ARLW_bit6(i);
   }
}

#endif