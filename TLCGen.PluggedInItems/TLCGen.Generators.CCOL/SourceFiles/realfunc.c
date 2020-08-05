/*
   BESTAND:   realfunc.c
*/

/****************************** Versie commentaar ***********************************
 *
 * Versie  Datum       Naam          Commentaar
 *
 * 1.0     05-08-2020  OK Geregeld:  Functies t.b.v. realisatietijd en correcties
 *
 ************************************************************************************/

mulv REALTIJD[FCMAX];

boolv REAL_SYN[FCMAX][FCMAX];  /* Vlag tbv synchronisatie      obv REALTIJD */
                              /* BIT1 TRUE/FALSE                           */
                              /* BIT2 correctie gelijk (extra info)        */
                              /* BIT3 correctie plus   (extra info)        */
                              /* BIT4 correctie min    (extra info)        */
boolv REAL_FOT[FCMAX][FCMAX];  /* Vlag tbv fictieve ontruiming obv REALTIJD */
mulv TIME_FOT[FCMAX][FCMAX];  /* Timer resterende FOT  (extra info)        */

/* ========================================================================================================================================================================================================== */
/* REALISATIETIJD ALGEMEEN                                                                                                                                                                                    */
/* ========================================================================================================================================================================================================== */
void Realisatietijd(count fc, count hsignaalplan, mulv correctie_sp)
{
  register count n, k;

  mulv conflicttijd;
  mulv eigentijd;

  REALTIJD[fc] = 0;  /* realisatietijd resetten */

  /* hoogste realisatietijd berekenen */
  for (n=0; n<FKFC_MAX[fc]; n++)
  {
     conflicttijd = 0;

     k= TO_pointer[fc][n];

     /* ------------------------------------------------------------ */
     /* ALS CONFLICT actief IS DIENT CONFLICTTIJD BEPAALD TE WORDEN: */
     /* - RA[k]     rood na aanvraagafwikkeling                      */
     /* - VS[k]     voorstartgroen                                   */
     /* - TO[k][fc] ontruiming actief (vanaf groen)                  */
     /* ------------------------------------------------------------ */
     if(/*RA[k] ||*/                /* (fictief) conflict in RA of   */
        VS[k] && (RS[k] || YS[k]))  /* VS, dan hoge conflicttijd     */
     {
         conflicttijd =  3000;
     }
     else if(TO[k][fc])
     {
       /* ---------------------------------------------------------- */
       /* conflicttijd groen-   conflicten bepalen                   */
       /* ---------------------------------------------------------- */
       if(TO_max[k][fc]==GK || TO_max[k][fc]==GKL)
       {
         conflicttijd = ( G[k] && !MG[k]) ? TFG_max[k]     - TFG_timer[k] +       /* conflict G (uitgezonderd MG) */
                                            TVG_max[k]     - TVG_timer[k]   : 0;

#if PLMAX
         if(hsignaalplan!=NG && correctie_sp!=NG && IH[hsignaalplan])  /* tijdens signaalplan en groen conflicttijd aanpassen */
         {
           if(G[k] && PR[k] && !MG[k] && (conflicttijd < (TOTXD_PL[k]- correctie_sp)))
                                          conflicttijd = (TOTXD_PL[k]- correctie_sp);

           /* Voor alternatief tijdens signaalplan nog geen rekening gehouden met resterrend groen. */
           /* Voor nu mag een alernatief zsm afgebroken worden.                                     */
         }
#endif
       }

       /* ---------------------------------------------------------- */
       /* conflicttijd conflicten bepalen                            */
       /* ---------------------------------------------------------- */
       else if((TO_max[k][fc]>=0))
       {
         conflicttijd = ( G[k] && !MG[k]) ? TFG_max[k]     - TFG_timer[k] +       /* conflict G (uitgezonderd MG) */
                                            TVG_max[k]     - TVG_timer[k] +
                                            TGL_max[k]                    +
                                             TO_max[k][fc]                  :
                        (GL[k] ||  MG[k]) ? TGL_max[k]     - TGL_timer[k] +       /* conflict GL of MG             */
                                             TO_max[k][fc]                  :
                                TO[k][fc] ?  TO_max[k][fc] -  TO_timer[k]   : 0;  /* ontruimen   of MG             */

#if PLMAX
         if(hsignaalplan!=NG && correctie_sp!=NG && IH[hsignaalplan])  /* tijdens signaalplan en groen conflicttijd aanpassen */
         {
           if(G[k] && PR[k] && !MG[k] && (conflicttijd < (TOTXD_PL[k]- correctie_sp + TGL_max[k] + TO_max[k][fc])))
                                          conflicttijd = (TOTXD_PL[k]- correctie_sp + TGL_max[k] + TO_max[k][fc]);

           /* Voor alternatief tijdens signaalplan nog geen rekening gehouden met resterrend groen. */
           /* Voor nu mag een alernatief zsm afgebroken worden.                                     */
         }
#endif
       }
     }

     /* --------------------------------------------------------- */
     /* REALISATIETIJD BEPALEN ALS CONFLICTTIJD MAATGEVEND BLIJKT */
     /* --------------------------------------------------------- */
     REALTIJD[fc] = !G[fc] && conflicttijd > REALTIJD[fc] ? conflicttijd : REALTIJD[fc];
  }

  /* -------------------------------------------------------------------------- */
  /* BEPAAL EIGEN TIJD:                                                         */
  /*                                                                            */
  /* 3000 (realisatijd heel hoog zetten) als:                                   */
  /* - BL                                                                       */
  /* - RR vanaf BIT2 en BIT0, BIT1/BIT2 worden gebruikt door Synchroniseer():   */
  /*   - BIT1 tegenhouden startgroen obv realisatietijd                         */
  /*   - BIT2 tegenhouden startgroen obv fictieve ontruimingstijd               */
  /*                                                                            */
  /* - anders resterend geeltijd + garantie roodtijd berekenen (+ 1 rondje RA)  */
  /* -------------------------------------------------------------------------- */
  eigentijd =     BL[fc]                                   ? 3000                                                          :
                  RR[fc] >= BIT2 || (RR[fc] & BIT0)        ? 3000                                                          :

                  GL[fc]                                   ? TGL_max[fc] - TGL_timer[fc] + TRG_max[fc]                 + 1 :
                 TRG[fc]                                   ?                               TRG_max[fc] - TRG_timer[fc] + 1 :
                  RV[fc]                                   ?                                                             1 : 0;

  /* --------------------------------------------------------- */
  /* REALISATIETIJD BEPALEN ALS EIGENTIJD MAATGEVEND BLIJKT    */
  /* --------------------------------------------------------- */
  REALTIJD[fc] = !G[fc] && eigentijd    > REALTIJD[fc] ? eigentijd    : REALTIJD[fc];
}

/* ========================================================================================================================================================================================================== */
/* REALISATIETIJD naar Memory Element                                                                                                                                                                         */
/* ========================================================================================================================================================================================================== */
void Realisatietijd_MM(count fc, count mrt)
{
  MM[mrt] = REALTIJD[fc];
}

/* ================================================================================================================================================================================== */
/* REALISATIETIJD CORRECTIE - NEW !!! - eenzijdig gemaakt - negatieve waarden kunnen als argument worden meegegeven                                                                   */
/* ================================================================================================================================================================================== */
boolv Corr_Real(count fc1,        /* fasecyclus 1                                       */
               count fc2,        /* fasecyclus 2                                       */
               mulv  t1_t2,      /* correctiewaarde van fc1 naar fc2                   */
               boolv  period)     /* extra voorwaarde                                   */
{
  boolv result = 0;

  /* --------------------------------------------------------------------------------------------- */
  /* Bepaal synchronisatie ongewenst                                                               */
  /* --------------------------------------------------------------------------------------------- */
  /* Beveiliging tegen ongewenste synchronsiatie als:                                              */
  /* - fc1 RV blijft, na TRG                                                                       */
  /* - fc1 PG heeft                                                                                */
  /* - fc2 PR heeft, maar PG nog niet                                                              */
  /* Bovenstaande kan voorkomen als:                                                               */
  /* - fc1 alternatief realiseert en groen wordt                                                   */
  /* - fc2 alternatief realiseert en nog in RA blijft (vanwege lopend conflict)                    */
  /* - modulemolen doorschiet naar eigen blok                                                      */
  /* - fc1 dan direct een PG krijgt (groen in eigen blok)                                          */
  /* - fc2 dan direct een PR krijgt (RA    in eigen blok), maar moet nog groen worden              */
  /* - fc1 naar geel/rood gaat en weer een aanvraag heeft, voordat fc2 groen is                    */
  /* - fc1 een FK heeft die aanvraag heeft en eerder aan de beurt is                               */
  /* - FC2 MOET DAN NIET SYNCHRONISEREN MET FC1, WANT DIE IS AL GEWEEST!!!!!                       */
  /* - FC2 MOET NOG WEL PRIMAIR REALISEREN.                                                        */
  /* --------------------------------------------------------------------------------------------- */
  if(PG[fc1] && RV[fc1] && !TRG[fc1] && PR[fc2] && !PG[fc2])
  {
    REAL_SYN[fc1][fc2] = FALSE;
  }
  /* --------------------------------------------------------------------------------------------- */
  /* Bepaal synchronisatie gewenst                                                                 */
  /* --------------------------------------------------------------------------------------------- */
  else
  {
    REAL_SYN[fc1][fc2] = period;
  }

  /* --------------------------------------------------------------------------------------------- */
  /* Realisatietijd fc2 wijzigen, mits:                                                            */
  /* --------------------------------------------------------------------------------------------- */
  /* - synchronisatie gewenst                                                                      */
  /* - fc2 geen groen                                                                              */
  /* - fc1 aanvraag of geel of garantierood                                                        */
  /* - realtijd fc2 kleiner dan die van fc1 + correctie                                            */
  if(REAL_SYN[fc1][fc2] && !G[fc2] && (A[fc1] || GL[fc1] || TRG[fc1]) && (REALTIJD[fc2] < (REALTIJD[fc1] + t1_t2)))
  {
    REALTIJD[fc2] = REALTIJD[fc1]==3000 ? 3000 : REALTIJD[fc1] + t1_t2;

    result = TRUE;
  }

  /* --------------------------------- */
  /* Wijzigingen aangeven via result   */
  /* --------------------------------- */
  return result;
}

/* ================================================================================================================================================================================== */
/* REALISATIETIJD CORRECTIE - GELIJK (fc1 en fc2 tegelijk groen)                                                                                                                      */
/* ================================================================================================================================================================================== */
boolv Corr_Gel(count fc1,        /* fasecyclus 1                                       */
              count fc2,        /* fasecyclus 2                                       */
              boolv  period)     /* extra voorwaarde                                   */
{
  boolv result1 = 0;
  boolv result2 = 0;

  result2 |= Corr_Real(fc1, fc2, 0, period);
  result1 |= Corr_Real(fc2, fc1, 0, period);

  /* --------------------------------- */
  /* Wijzigingen aangeven via result   */
  /* --------------------------------- */
  REAL_SYN[fc1][fc2] |= result2 ? BIT2 : 0;
  REAL_SYN[fc2][fc1] |= result1 ? BIT2 : 0;

  return (result1 || result2);
}

/* ================================================================================================================================================================================== */
/* REALISATIETIJD CORRECTIE - PLUS (fc2 dus later groen dan fc1)                                                                                                                      */
/* ================================================================================================================================================================================== */
boolv Corr_Pls(count fc1,        /* fasecyclus 1                                       */
              count fc2,        /* fasecyclus 2                                       */
              mulv  t1_t2,      /* correctiewaarde fc1 obv fc2                        */
              boolv  period)     /* extra voorwaarde                                   */
{
  boolv result = 0;

  result |= Corr_Real(fc1, fc2, t1_t2, period);

  /* --------------------------------- */
  /* Wijzigingen aangeven via result   */
  /* --------------------------------- */
  REAL_SYN[fc1][fc2] |= result ? BIT3 : 0;

  return result;
}

/* ================================================================================================================================================================================== */
/* REALISATIETIJD CORRECTIE - MIN (fc2 dus eerder groen dan fc1)                                                                                                                      */
/* ================================================================================================================================================================================== */
boolv Corr_Min( count fc1,        /* fasecyclus 1                                       */
               count fc2,        /* fasecyclus 2                                       */
               mulv  t1_t2,      /* correctiewaarde fc1 obv fc2                        */
               boolv  period)     /* extra voorwaarde                                   */
{
  boolv result = 0;

  result |= Corr_Real(fc1, fc2, -t1_t2, period);

  /* --------------------------------- */
  /* Wijzigingen aangeven via result   */
  /* --------------------------------- */
  REAL_SYN[fc1][fc2] |= result ? BIT4 : 0;

  return result;
}

/* ================================================================================================================================================================================== */
/* REALISATIETIJD CORRECTIE DUBBELE VOETGANGERSOVERSTEEK (hulpfunctie die Corr_Real() aanroept afhankelijk van buitendrukknoppen of beide alleen mee-aanvraag)                        */
/* ================================================================================================================================================================================== */
boolv VTG2_Real(count fc1,        /* fasecyclus 1                                       */
               count fc2,        /* fasecyclus 2                                       */
               mulv  t1_t2,      /* correctiewaarde fc1 tov fc2                        */
               mulv  t2_t1,      /* correctiewaarde fc2 tov fc1                        */
               count hdk1_bu,    /* hulpelement, drukknop fc1 buitenzijde              */
               count hdk2_bu,    /* hulpelement, drukknop fc2 buitenzijde              */
               boolv  gelijk )    /* extra voorwaarde t.b.v. gelijkstart                */
{
  boolv result = 0;

  gelijk |= A[fc1] && !(A[fc1] & ~(BIT4|BIT8)) &&  /* Sowieso gelijk als beide alleen mee-aanvraag.   */
            A[fc2] && !(A[fc2] & ~(BIT4|BIT8)) ||

           !A[fc1] && !A[fc2];                     /* Bij beide geen A ook uitgaan van gelijk,         */
                                                   /* van belang tbv eventueel onderling doorzetten PG */
  /* ------------------------------------------------------------------------------------------------------------------------------------------------------ */
  /* tbv gelijkstart/inlopen                               fc1  fc2  corr1  corr2  type_corr1 type_corr2                                                    */
  /* ------------------------------------------------------------------------------------------------------------------------------------------------------ */
       if(gelijk                    )  {  result |= Corr_Gel(fc1, fc2       , TRUE);                        /* beide alleen meeanvraag, gelijkstart         */
                                       }
  else if(IH[hdk1_bu] && IH[hdk2_bu])  {  result |= Corr_Min(fc1, fc2, t1_t2, TRUE);                        /* beide buitendrukknoppen, beide mogen inlopen */
                                          result |= Corr_Min(fc2, fc1, t2_t1, TRUE);
                                       }
  else if(IH[hdk1_bu]               )  {  result |= Corr_Min(fc1, fc2, t1_t2, TRUE);                        /* dkfc1_bu               , fc1   mag   inlopen */
                                          result |= Corr_Min(fc2, fc1,     0, TRUE);                        /*                        , fc1   niet  inlopen */
                                       }
  else if(IH[hdk2_bu]               )  {  result |= Corr_Min(fc1, fc2,     0, TRUE);                        /* dkfc2_bu               , fc1   niet  inlopen */
                                          result |= Corr_Min(fc2, fc1, t2_t1, TRUE);                        /*                        , fc2   mag   inlopen */
                                       }

  /* --------------------------------- */
  /* Wijzigingen aangeven via result   */
  /* --------------------------------- */
  return result;
}

/* ================================================================================================================================================================================== */
/* REALISATIETIJD CORRECTIE DOOR FICTIEVE ONTRUIMINGSTIJD - NEW !!! - eenzijdig gemaakt                                                                                               */
/* ================================================================================================================================================================================== */
boolv Corr_FOT(count fc1,     /* fasecyclus VAN                       */
              count fc2,     /* fasecyclus NAAR                      */
              count fot1_2,  /* fictieve ontruiming VAN fc1 NAAR fc2 */
              mulv  gg1)     /* TGG_timer[fc1] > gg1, dan RT[fot1_2] */
{
  boolv result = 0;
  mulv hulp   = 0;

  /* -------------------------------------------------------------------------- */
  /* Herstarten fictieve ontruimingstijden                                      */
  /* -------------------------------------------------------------------------- */
  RT[fot1_2] = GL[fc1] || G[fc1] && (TGG_timer[fc1] > gg1 || !TGG[fc1]);

  /* -------------------------------------------------------------------------- */
  /* Realisatietijd fc2 wijzigen mits:                                          */
  /* -------------------------------------------------------------------------- */
  /* - fictieve ontruimingstijd loopt                                           */
  /* - fc1 geen groen                                                           */
  /*                                                                            */
  /* Correctie als REALTIJD[fc2] kleiner dan resterende tijd van FOT            */
  /* -------------------------------------------------------------------------- */
  if((RT[fot1_2] || T[fot1_2]) && !G[fc2])
  {
     /* hulp = resterend groen + geel + FOT                                        */
     /* tijdens groen check op RT, om correctie obv FOT vorige cyclus te voorkomen */
     hulp = RT[fot1_2] && G[fc1] && !MG[fc1] ? TFG_max[fc1]    - TFG_timer[fc1] +
                                               TVG_max[fc1]    - TVG_timer[fc1] +
                                               TGL_max[fc1]                     +
                                                 T_max[fot1_2]                  :
            RT[fot1_2] &&            MG[fc1] ? TGL_max[fc1]                     +
                                                 T_max[fot1_2]                  :
                                     GL[fc1] ? TGL_max[fc1]    - TGL_timer[fc1] +
                                                 T_max[fot1_2]                  :
                                                 T_max[fot1_2] - T_timer[fot1_2];

    if(REALTIJD[fc2] < hulp)
    {
             REALTIJD[fc2] = hulp;
                  result   = TRUE;
    }
  }

  /* -------------------------------------------------------------------------- */
  /* Vlag tbv fictieve ontruimingstijd bijwerken, toegepast van fc1 naar fc2    */
  /* -------------------------------------------------------------------------- */
  REAL_FOT[fc1][fc2] = RT[fot1_2] || T[fot1_2];

  TIME_FOT[fc1][fc2] = RT[fot1_2] || T[fot1_2] ? T_max[fot1_2] - T_timer[fot1_2] : 0;  /* timer reseterende FOT t.b.v extra info / debuggen */
  /* -------------------------------------------------------------------------- */

  /* --------------------------------- */
  /* Wijzigingen aangeven via result   */
  /* --------------------------------- */
  return result;
}

/* ========================================================================================================================================================================================================== */
/* SYNCHRONISEER OBV SIGNAALPLAN INTREKKEN                                                                                                                                                                    */
/* ========================================================================================================================================================================================================== */
/* Tijdens signaalplan tussen TXA- en TXB-moment geen X/RR als TXB-moment daardoor in gevaar komt.                                                                                                            */
/* Dit geldt alleen voor "normale" synchronisaties, fictieve ontruimingstijden leiden wel tot X/RR.                                                                                                           */
/*                                                                                                                                                                                                            */
/* Dus dan REAL_SYN[][] resetten.                                                                                                                                                                             */
/*                                                                                                                                                                                                            */
/* Het is de zaak om de signaalplantijden op de synchronisaties af te stemmen als synchronisatie                                                                                                              */
/* gewenst is. Als dit niet is gebeurd (bewust of onbewust) kan er geen synchronisatie plaatsvinden.                                                                                                          */
/* ========================================================================================================================================================================================================== */
#if PLMAX
void Synchroniseer_SP(void)
{
  register count fc1, fc2;

  for (fc1=0; fc1<FC_MAX; fc1++)
  {
    for (fc2=0; fc2<FC_MAX; fc2++)
    {
      if(REAL_SYN[fc1][fc2] && ((TOTXA_PL[fc2]==0) && (TOTXB_PL[fc2]>0) && (REALTIJD[fc2] > TOTXB_PL[fc2]) || TX_timer==TXB_PL[fc2]))
      {
         REAL_SYN[fc1][fc2] = FALSE;
      }
    }
  }
}
#endif

/* ========================================================================================================================================================================================================== */
/* SYNCHRONISEER OBV REALTIJD (startgroenmomenten)                                                                                                                                                            */
/* ========================================================================================================================================================================================================== */
/* Als synchronisatie gewenst (van fc1 naar fc2), fc2 rood is en realtijd loopt:                                                                                                                              */
/* -  X[fc2], zolang realtijd loopt                                                                                                                                                                           */
/* - RR[fc2], fc1 nog niet aan de beurt of fc1 hele hoge realisatietijd                                                                                                                                       */
/*            laatste rondje nooit RR, want RA moet nog gemaakt worden, vandaar (REALTIJD[fc2] > 1)                                                                                                           */
/* ========================================================================================================================================================================================================== */
void Synchroniseer_SG(void)
{
  register count fc1, fc2;

  for (fc1=0; fc1<FC_MAX; fc1++)
  {
    for (fc2=0; fc2<FC_MAX; fc2++)
    {
      if(REAL_SYN[fc1][fc2] && R[fc2])
      {
         X[fc2] |= (REALTIJD[fc2] > 0)                                                                      ? BIT1 : 0;
        RR[fc2] |= (REALTIJD[fc2] > 1) && ((GL[fc1] || RV[fc1]) && kcv_primair(fc1) || REALTIJD[fc1]==3000) ? BIT1 : 0;
      }
    }
  }
}
/* ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */
void Synchroniseer_SG1_2(count fc1, count fc2)  /* Gelijk aan bovenstaande alleen niet algemeen, maar specifiek per synchronisatie                                                                            */
{
      if(REAL_SYN[fc1][fc2] && R[fc2])
      {
         X[fc2] |= (REALTIJD[fc2] > 0)                                                                      ? BIT1 : 0;
        RR[fc2] |= (REALTIJD[fc2] > 1) && ((GL[fc1] || RV[fc1]) && kcv_primair(fc1) || REALTIJD[fc1]==3000) ? BIT1 : 0;
      }
}

/* ========================================================================================================================================================================================================== */
/* SYNCHRONISEER OBV FICTIEVE ONTRUIMING                                                                                                                                                                      */
/* ========================================================================================================================================================================================================== */
/* Onderstaande acties alleen als er sprake is van fictieve ontruiming:                                                                                                                                       */
/*                                                                                                                                                                                                            */
/* Als rood fc2 en fictieve ontruiming vanaf fc1 loopt:                                                                                                                                                       */
/* -  X[fc2], altijd                                                                                                                                                                                          */
/* - RR[fc2], CV van fc1, mits RV[fc2]                                                                                                                                                                        */
/*                                                                                                                                                                                                            */
/* - RW/YM    groen beeindigen van fc1 om fictieve ontruiming te laten eindigen                                                                                                                               */
/*                                                                                                                                                                                                            */
/* - Vasthouden groen fc2 als fc1 bijna aan de beurt is. Dit mag alleen mits er geen                                                                                                                          */
/*   fictieve ontruiming van fc2 naar fc1 is. Fc1 moet immers wel op groen kunnen komen, bijv:                                                                                                                */
/*   - fc1 = fc05                                                                                                                                                                                             */
/*   - fc2 = fc22                                                                                                                                                                                             */
/*   Dan groen vasthouden fc22 als fc05 bijna aan de beurt is.                                                                                                                                                */
/*   Dit dient echter niet te gebeuren als het fc05/fc11 betreft.                                                                                                                                             */
/* ========================================================================================================================================================================================================== */
void Synchroniseer_FO(void)
{
  register count fc1, fc2;

  for (fc1=0; fc1<FC_MAX; fc1++)
  {
    for (fc2=0; fc2<FC_MAX; fc2++)
    {
      if(REAL_FOT[fc1][fc2] && R[fc2])
      {
         X[fc2] |=                                                          BIT2;
        RR[fc2] |= (RV[fc2] || GL[fc2]) && G[fc1] && !MG[fc1]             ? BIT2             : 0;

        if(A[fc2])
        {
          RW[fc1] &= ~BIT4;
          YM[fc1] &= ~BIT4;

#ifdef __HALFSTARH__
       /* TLCGEN halfstarre YM bits */
          YM[fc1] &= ~YM_HALFSTAR;
          YM[fc1] &= ~YM_KOP_HALFSTAR;
#endif
        }
      }

      if(REAL_SYN[fc2][fc1] && !REAL_FOT[fc2][fc1] && G[fc2] && !G[fc1] && A[fc1] && (RA[fc1] || AAPR[fc1] && !PG[fc1]) && !RR[fc1] && !BL[fc1] && (REALTIJD[fc1] <= (TGL_max[fc2]+TRG_max[fc2])))
      {
        RW[fc2] |= BIT1;
      }
    }
  }
}
/* ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */
void Synchroniseer_FO1_2(count fc1, count fc2)   /* Gelijk aan bovenstaande alleen niet algemeen, maar specifiek per fictieve ontrumingstijd                                                                   */
{
      if(REAL_FOT[fc1][fc2] && R[fc2])
      {
         X[fc2] |=                                                          BIT2;
        RR[fc2] |= (RV[fc2] || GL[fc2]) && G[fc1] && !MG[fc1]             ? BIT2             : 0;

        if(A[fc2])
        {
          RW[fc1] &= ~BIT4;
          YM[fc1] &= ~BIT4;

#ifdef __HALFSTARH__
       /* TLCGEN halfstarre YM bits */
          YM[fc1] &= ~YM_HALFSTAR;
          YM[fc1] &= ~YM_KOP_HALFSTAR;
#endif
        }
      }

      if(REAL_SYN[fc2][fc1] && !REAL_FOT[fc2][fc1] && G[fc2] && !G[fc1] && A[fc1] && (RA[fc1] || AAPR[fc1] && !PG[fc1]) && !RR[fc1] && !BL[fc1] && (REALTIJD[fc1] <= (TGL_max[fc2]+TRG_max[fc2])))
      {
        RW[fc2] |= BIT1;
      }
}

/* ========================================================================================================================================================================================================== */
/* SYNCHRONISEER PG's                                                                                                                                                                                         */
/* ========================================================================================================================================================================================================== */
/* Als:                                                                                                                                                                                                       */
/* -    fc2 geen aanvraag heeft                                                                                                                                                                               */
/* - en fc2 rood is                                                                                                                                                                                           */
/* - en fc2 geen PG heeft                                                                                                                                                                                     */
/* - en fc1 wel  PG heeft                                                                                                                                                                                     */
/*                                                                                                                                                                                                            */
/* Dan door synchronisatie       PG[fc2]  = PG[fc1]         , mits fc1 rood  is                                                                                                                               */
/* Dan door fictieve ontruiming, PG[fc2] |= PRIMAIR_OVERSLAG, mits fc1 groen is                                                                                                                               */
/*                                                                                                                                                                                                            */
/* Verder nooit één van beide versneld primair, om te voorkomen dat PG's ongelijk geset worden. Dus:                                                                                                          */
/* - beide moeten PFPR hebben                                                                                                                                                                                 */
/* - beide moeten A    hebben                                                                                                                                                                                 */
/* zo niet, dan gaat versneld primair niet door                                                                                                                                                               */
/* ========================================================================================================================================================================================================== */
void Synchroniseer_PG(void)
{
  register count fc1, fc2;

  for (fc1=0; fc1<FC_MAX; fc1++)
  {
    for (fc2=0; fc2<FC_MAX; fc2++)
    {
      if(!A[fc2] && R[fc2] && !PG[fc2] && PG[fc1])
      {
          PG[fc2] |= REAL_SYN[fc1][fc2] && R[fc1] ? PG[fc1]          : 0;

          PG[fc2] |= REAL_FOT[fc1][fc2] && G[fc1] ? PRIMAIR_OVERSLAG : 0;
      }

      /* voorkomen dat slechts één van beide verscneld primair komt, terwijl synchronsatie gewenst */
      if(REAL_SYN[fc1][fc2] && A[fc1] && A[fc2] && (!PFPR[fc1] || !PFPR[fc2]))
      {
        PFPR[fc1] = FALSE;
        PFPR[fc2] = FALSE;
      }
    }
  }
}
/* ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */
void Synchroniseer_PG1_2(count fc1, count fc2)  /* Gelijk aan bovenstaande alleen niet algemeen, maar specifiek per synchronisatie                                                                            */
{
      if(R[fc2] && !PG[fc2] && PG[fc1])
      {
          PG[fc2] |= REAL_SYN[fc1][fc2] && R[fc1] ? PG[fc1]          : 0;

          PG[fc2] |= REAL_FOT[fc1][fc2] && G[fc1] ? PRIMAIR_OVERSLAG : 0;
      }

      /* voorkomen dat slechts één van beide verscneld primair komt, terwijl synchronsatie gewenst */
      if(REAL_SYN[fc1][fc2] && !(PFPR[fc1] && PFPR[fc2] && A[fc1] && A[fc2]))
      {
        PFPR[fc1] = FALSE;
        PFPR[fc2] = FALSE;
      }
}
/* ========================================================================================================================================================================================================== */

boolv Maatgevend_Groen(count fc)   /* fasecyclus                                      */
{
  register count n, k;

  boolv result=0;

  /* bepaal of G[fc] maatgevend is voor een (groen) conflict */
  /* ------------------------------------------------------- */
  if(G[fc])
  {
    for (n=0; n<GKFC_MAX[fc]; n++)
    {
       k= TO_pointer[fc][n];

      if(A[k] && (AAPR[k] || PAR[k] || RA[k] || AA[k]))
      {
        /* bepaal of G[fc] maatgevend is voor een groen-conflict */
        /* ----------------------------------------------------- */
        if(TO_max[fc][k]==GK || TO_max[fc][k]==GKL)
        {
          if(      MG[fc] && ((                                                                                      1) >= REALTIJD[k]))  result = TRUE;
          else if(!MG[fc] && ((TFG_max[fc] - TFG_timer[fc] + TVG_max[fc] - TVG_timer[fc]                              ) >= REALTIJD[k]))  result = TRUE;
        }
        /* bepaal of G[fc] maatgevend is voor een       conflict */
        /* ----------------------------------------------------- */
        else if(TO_max[fc][k]>=0)
        {
          if(      MG[fc] && ((                                                            TGL_max[fc] + TO_max[fc][k]) >= REALTIJD[k]))  result = TRUE;
          else if(!MG[fc] && ((TFG_max[fc] - TFG_timer[fc] + TVG_max[fc] - TVG_timer[fc] + TGL_max[fc] + TO_max[fc][k]) >= REALTIJD[k]))  result = TRUE;
        }
      }
    }
  }

  return result;
}

void Inlopen_Los3(count fc1,        /* fc1                                                                       */
                  count fc9,        /* fc9, middelste fc                                                         */
                  count fc2,        /* fc2                                                                       */

                  count dk1bu,      /* drukknop fc1 buitenzijde                                                  */
                  count dk1bi,      /* drukknop fc1 middenberm                                                   */
                  count dk9a,       /* drukknop fc9 A, naloop fc2                                                */
                  count dk9b,       /* drukknop fc9 B, naloop fc1                                                */
                  count dk2bi,      /* drukknop fc2 middenberm                                                   */
                  count dk2bu,      /* drukknop fc2 buitenzijde                                                  */

                  count hdk1bu,     /* hulpelement drukknop fc1 buitenzijde                                      */
                  count hdk1bi,     /* hulpelement drukknop fc1 middenberm                                       */
                  count hdk9a,      /* hulpelement drukknop fc9 A, naloop fc2                                    */
                  count hdk9b,      /* hulpelement drukknop fc9 B, naloop fc1                                    */
                  count hdk2bi,     /* hulpelement drukknop fc2 middenberm                                       */
                  count hdk2bu,     /* hulpelement drukknop fc2 buitenzijde                                      */

                  count hinl1,      /* hulpelement fc1 inlopen gewenst                                           */
                  count hinl9_1,    /* hulpelement fc9 inlopen gewenst op fc1                                    */
                  count hinl9_2,    /* hulpelement fc9 inlopen gewenst op fc2                                    */
                  count hinl2,      /* hulpelement fc2 inlopen gewenst                                           */

                  count hlos1,      /* hulpelement fc1 los toegestaan                                            */
                  count hlos9,      /* hulpelement fc9 los toegestaan                                            */
                  count hlos2,      /* hulpelement fc2 los teogestaan                                            */

                  boolv  sch1_1,     /* fc1   los, bij DK1_bi en DK1_bu                                           */
                  boolv  sch1_2,     /* fc1   los, bij DK1_bi en DK2_bu/DK9_b (A in rug tbv naloop)               */
                  boolv  sch2_1,     /* fc2   los, bij DK2_bi en DK2_bu                                           */
                  boolv  sch2_2,     /* fc2   los, bij DK2_bi en DK1_bu/DK9_a (A in rug tbv naloop)               */
                  boolv  sch9_1a,    /* fc9   los, bij DK9_a                  (DK9_a single OK    )               */
                  boolv  sch9_1b,    /* fc9   los, bij DK9_b                  (DK9_b single OK    )               */
                  boolv  sch9_2a,    /* fc9   los, bij DK9_a  en DK2_bu       (tegenligger    fc32)               */
                  boolv  sch9_2b,    /* fc9   los, bij DK9_b  en DK1_bu       (tegenligger    fc31)               */
                  boolv  sch9_3a,    /* fc9   los, bij DK9_a  en DK1_bu       (A in rug tbv naloop)               */
                  boolv  sch9_3b,    /* fc9   los, bij DK9_b  en DK2_bu       (A in rug tbv naloop)               */
                  boolv  sch9_4a,    /* fc9-2 los, bij DK9_a  en DK1_bu       (A in rug tbv naloop)               */
                  boolv  sch9_4b)    /* fc9-1 los, bij DK9_b  en DK2_bu       (A in rug tbv naloop)               */
{
  /* ------------------------------------------------------------------------------------------------------------------------------ */
  /* Bij een drie-voudige oversteek met 6 drukknoppen zijn er 18 mogelijkheden waarbij er:                                          */
  /* - alleen in het midden                                                                                                         */
  /* - of in het midden i.c.m. 1 andere drukknop is gedrukt.                                                                        */
  /*                                                                                                                                */
  /* Wanneer er op een binnendrukknop is gedrukt is het afhankelijk van de andere drukknoppen wat er moet gebeuren.                 */
  /* E.e.a. staat hieronder uitgewerkt:                                                                                             */
  /* ------------------------------------------------------------------------------------------------------------------------------ */
  /*    |      fc1      |     fc9     |      fc2      | fc1 | fc9 | fc2 |fc9-1|fc9-2|                                               */
  /*    |DK1_bu - DK1_bi|DK9_a - DK9_b|DK2_bi - DK2_bu|     |     |     |     |     |               toelichting                     */
  /* ---|---------------|-------------|---------------|-----|-----|-----|-----|-----|---------------------------------------------- */
  /*  1:|           X   |             |               | LOS |     |     |     |     | LOS = LOS                                     */
  /*  2:|   X       X   |             |               | SCH |     |     |     |     | SCH = LOS SCHAKELBAAR                         */
  /*  3:|           X   |             |   X           | LOS |     | LOS |     |     | INL = INLOPEN                                 */
  /*  4:|           X   |             |           X   | SCH |     | INL |     |     |                                               */
  /*  5:|           X   |   X         |               | LOS | SCH |     |     |     |                                               */
  /*  6:|           X   |          X  |               | SCH | SCH |     |     |     |                                               */
  /* ---|---------------|-------------|---------------|-----|-----|-----|-----|-----|---------------------------------------------- */
  /*  7:|               |             |   X           |     |     | LOS |     |     |                                               */
  /*  8:|               |             |   X       X   |     |     | SCH |     |     |                                               */
  /*  3:|           X   |             |   X           | LOS |     | LOS |     |     |                                               */
  /*  9:|   X           |             |   X           | INL |     | SCH |     |     |                                               */
  /* 10:|               |          X  |   X           |     | SCH | LOS |     |     |                                               */
  /* 11:|               |   X         |   X           |     | SCH | SCH |     |     |                                               */
  /* ---|---------------|-------------|---------------|-----|-----|-----|-----|-----|---------------------------------------------- */
  /* 12:|               |   X         |               |     | SCH |     |     |     |                                               */
  /* 13:|               |   X      X  |               |     | SCH |     |     |     |                                               */
  /* 14:|   X           |   X         |               | INL | BYZ |     |     | SCH | BYZ = BYZONDER: FC9 los? Zo nee, FC9-FC2 los? */
  /*  5:|           X   |   X         |               | LOS | SCH |     |     |     |                                               */
  /* 11:|               |   X         |   X           |     | SCH | SCH |     |     |                                               */
  /* 15:|               |   X         |           X   |     | SCH | INL |     |     |                                               */
  /* ---|---------------|-------------|---------------|-----|-----|-----|-----|-----|---------------------------------------------- */
  /* 16:|               |          X  |               |     | SCH |     |     |     |                                               */
  /* 13:|               |   X      X  |               |     | SCH |     |     |     |                                               */
  /* 17:|   X           |          X  |               | INL | SCH |     |     |     |                                               */
  /*  6:|           X   |          X  |               | SCH | SCH |     |     |     |                                               */
  /* 10:|               |          X  |   X           |     | SCH | LOS |     |     |                                               */
  /* 18:|               |          X  |           X   |     | BYZ | INL | SCH |     | BYZ = BYZONDER: FC9 los? Zo nee, FC9-FC1 los? */
  /* ---|---------------|-------------|---------------|-----|-----|-----|-----|-----|---------------------------------------------- */

  boolv SITUATIE[19]={0};

  /* ----------------------------------------------------------------------------- */
  /* Bepaal of drukknop bediend is                                                 */
  /* ----------------------------------------------------------------------------- */
  IH[hdk1bu] = G[fc1] ? FALSE : D[dk1bu] && A[fc1] ? TRUE : IH[hdk1bu];
  IH[hdk1bi] = G[fc1] ? FALSE : D[dk1bi] && A[fc1] ? TRUE : IH[hdk1bi];
  IH[hdk2bi] = G[fc2] ? FALSE : D[dk2bi] && A[fc2] ? TRUE : IH[hdk2bi];
  IH[hdk2bu] = G[fc2] ? FALSE : D[dk2bu] && A[fc2] ? TRUE : IH[hdk2bu];

  IH[hdk9a]  = G[fc9] ? FALSE : D[dk9a]  && A[fc9] ? TRUE : IH[hdk9a] ;
  IH[hdk9b]  = G[fc9] ? FALSE : D[dk9b]  && A[fc9] ? TRUE : IH[hdk9b] ;

  /* ----------------------------------------------------------------------------- */
  /* Vaststellen situaties:                                                        */
  /* - Er kunnen meerdere situaties tegelijk actief zijn, behalve 1-7-12-16        */
  /* ----------------------------------------------------------------------------- */
  SITUATIE[ 1] |= !IH[hdk1bu] &&  IH[hdk1bi] && !IH[hdk9a] && !IH[hdk9b] && !IH[hdk2bi] && !IH[hdk2bu];
  SITUATIE[ 2] |=  IH[hdk1bu] &&  IH[hdk1bi]                                                          ;
  SITUATIE[ 3] |=                 IH[hdk1bi] &&                              IH[hdk2bi]               ;
  SITUATIE[ 4] |=                 IH[hdk1bi] &&                                             IH[hdk2bu];
  SITUATIE[ 5] |=                 IH[hdk1bi] &&  IH[hdk9a]                                            ;
  SITUATIE[ 6] |=                 IH[hdk1bi] &&                IH[hdk9b]                              ;

  SITUATIE[ 7] |= !IH[hdk1bu] && !IH[hdk1bi] && !IH[hdk9a] && !IH[hdk9b] &&  IH[hdk2bi] && !IH[hdk2bu];
  SITUATIE[ 8] |=                                                            IH[hdk2bi] &&  IH[hdk2bu];
  SITUATIE[ 9] |=  IH[hdk1bu]                                            &&  IH[hdk2bi]               ;
  SITUATIE[10] |=                                              IH[hdk9b] &&  IH[hdk2bi]               ;
  SITUATIE[11] |=                                IH[hdk9a]               &&  IH[hdk2bi]               ;

  SITUATIE[12] |= !IH[hdk1bu] && !IH[hdk1bi] &&  IH[hdk9a] && !IH[hdk9b] && !IH[hdk2bi] && !IH[hdk2bu];
  SITUATIE[13] |=                                IH[hdk9a] &&  IH[hdk9b]                              ;
  SITUATIE[14] |=  IH[hdk1bu]                &&  IH[hdk9a]                                            ;
  SITUATIE[15] |=                                IH[hdk9a] &&                               IH[hdk2bu];

  SITUATIE[16] |= !IH[hdk1bu] && !IH[hdk1bi] && !IH[hdk9a] &&  IH[hdk9b] && !IH[hdk2bi] && !IH[hdk2bu];
  SITUATIE[17] |=  IH[hdk1bu]                              &&  IH[hdk9b]                              ;
  SITUATIE[18] |=                                              IH[hdk9b] &&                 IH[hdk2bu];

  /* ------------------------------------------------------------------------------------------------- */
  /* Vaststellen maatgevende acties:                                                                   */
  /* - in eerste instantie in principe "los" bij drukknop in midden en geen conflicterend CV           */
  /* - daarna los intrekken als er een beperking is                                                    */
  /* - SITUATIE 1-3-7 hebben geen beperking                                                            */
  /* ------------------------------------------------------------------------------------------------- */
  IH[hlos1] =  IH[hdk1bi]               && R[fc1] && !kcv(fc1) ? TRUE : FALSE;
  IH[hlos9] = (IH[hdk9a ] || IH[hdk9b]) && R[fc9] && !kcv(fc9) ? TRUE : FALSE;
  IH[hlos2] =  IH[hdk2bi]               && R[fc2] && !kcv(fc2) ? TRUE : FALSE;

  if(SITUATIE[ 1]            )  IH[hlos1] = IH[hlos1];  /* DK1_bi    single, geen beperkingen          */
  if(SITUATIE[ 2] && !sch1_1 )  IH[hlos1] = FALSE;      /* DK1_bi en DK1_bu                            */
  if(SITUATIE[ 3]            )  IH[hlos1] = IH[hlos1];  /* DK1_bi en DK2_b1, geen beperkingen          */
  if(SITUATIE[ 4] && !sch1_2 )  IH[hlos1] = FALSE;      /* DK1_bi en DK2_bu                            */
  if(SITUATIE[ 5] && !sch9_1a)  IH[hlos9] = FALSE;      /* DK1_bi en DK9_a                             */
  if(SITUATIE[ 6] && !sch9_1b)  IH[hlos9] = FALSE;      /* DK1_bi en DK9_b                             */
  if(SITUATIE[ 6] && !sch1_2 )  IH[hlos1] = FALSE;      /* DK1_bi en DK9_b                             */

  if(SITUATIE[ 7]            )  IH[hlos2] = IH[hlos2];  /* DK2_bi    single, geen beperkingen          */
  if(SITUATIE[ 8] && !sch2_1 )  IH[hlos2] = FALSE;      /* DK2_bi en DK2_bu                            */
  if(SITUATIE[ 9] && !sch2_2 )  IH[hlos2] = FALSE;      /* DK2_bi en DK1_bu                            */
  if(SITUATIE[10] && !sch9_1b)  IH[hlos9] = FALSE;      /* DK2_bi en DK9_b                             */
  if(SITUATIE[11] && !sch9_1a)  IH[hlos9] = FALSE;      /* DK2_bi en DK9_a                             */
  if(SITUATIE[11] && !sch2_2 )  IH[hlos2] = FALSE;      /* DK2_bi en DK9_a                             */

  if(SITUATIE[12] && !sch9_1a)  IH[hlos9] = FALSE;      /* DK9_a     single                            */
  if(SITUATIE[13] && !sch9_1a)  IH[hlos9] = FALSE;      /* DK9_a  en DK9_b                             */
  if(SITUATIE[13] && !sch9_1b)  IH[hlos9] = FALSE;      /* DK9_a  en DK9_b                             */
  if(SITUATIE[14] && !sch9_3a)  IH[hlos9] = FALSE;      /* DK9_a  en DK1_bu                            */
  if(SITUATIE[15] && !sch9_2a)  IH[hlos9] = FALSE;      /* DK9_a  en DK2_bu                            */

  if(SITUATIE[16] && !sch9_1b)  IH[hlos9] = FALSE;      /* DK9_b     single                            */
  if(SITUATIE[17] && !sch9_2b)  IH[hlos9] = FALSE;      /* DK9_b  en DK1_bu                            */
  if(SITUATIE[18] && !sch9_3b)  IH[hlos9] = FALSE;      /* DK9_b  en DK2_bu                            */

  /* ----------------------------------------------------------------------------- */
  /* Bepaal of inlopen gewenst is:                                                 */
  /* ----------------------------------------------------------------------------- */
  IH[hinl1]   = EG[fc1] ? FALSE : R[fc1] ? IH[hdk1bu] && !IH[hlos1] : IH[hinl1]  ;
  IH[hinl9_1] = EG[fc9] ? FALSE : R[fc9] ? IH[hdk9b ] && !IH[hlos9] : IH[hinl9_1];
  IH[hinl9_2] = EG[fc9] ? FALSE : R[fc9] ? IH[hdk9a ] && !IH[hlos9] : IH[hinl9_2];
  IH[hinl2]   = EG[fc2] ? FALSE : R[fc2] ? IH[hdk2bu] && !IH[hlos2] : IH[hinl2]  ;

  /* ----------------------------------------------------------------------------- */
  /* Inlopen fc1 (tijdelijk) opheffen voor SITUATIE 14 als (DK9_a  en DK1_bu):     */
  /* - fc9 niet los mag                                                            */
  /* - SCH[sch94] toestaat dat koppel fc2/fc9 los mag                              */
  /* - fc2 een kleinere REALTIJD heeft dan fc1                                     */
  /* - fc9 een kleinere REALTIJD heeft dan fc1                                     */
  /* ----------------------------------------------------------------------------- */
  if(SITUATIE[14] && !IH[hlos9] && sch9_4a && (REALTIJD[fc9] < REALTIJD[fc1])
                                           && (REALTIJD[fc2] < REALTIJD[fc1]))
  {
    IH[hinl1] = FALSE;
    IH[hlos1] = TRUE;
  }
  /* ----------------------------------------------------------------------------- */
  /* Inlopen fc2 (tijdelijk) opheffen voor SITUATIE 18 als (DK9_b  en DK2_bu):     */
  /* - fc9 niet los mag                                                            */
  /* - SCH[sch94] toestaat dat koppel fc1/fc9 los mag                              */
  /* - fc1 een kleinere REALTIJD heeft dan fc2                                     */
  /* - fc9 een kleinere REALTIJD heeft dan fc2                                     */
  /* ----------------------------------------------------------------------------- */
  if(SITUATIE[18] && !IH[hlos9] && sch9_4b && (REALTIJD[fc9] < REALTIJD[fc2])
                                           && (REALTIJD[fc1] < REALTIJD[fc2]))
  {
    IH[hinl2] = FALSE;
    IH[hlos2] = TRUE;
  }
  /* ----------------------------------------------------------------------------- */
  /* Inlopen van fc9 bezig, dan voor fc1/fc2 geen beperkingen. Er is dan voor 2    */
  /* van de 3 richtingen gerekend, fc1 of fc2 gaat dan binnenkort groen worden.    */
  /* Dus dan zijn er geen beperkingen meer nodig. De beperkingen kunnen immers ook */
  /* weer in de weg gaan zitten, waardoor inlopen van fc9 de mist in gaat.         */
  /* ----------------------------------------------------------------------------- */
  if(IH[hinl9_1] && G[fc9] && !G[fc1] ||
     IH[hinl9_2] && G[fc9] && !G[fc2])
  {
    IH[hinl1] = FALSE;
    IH[hlos1] = TRUE;

    IH[hinl2] = FALSE;
    IH[hlos2] = TRUE;
  }
}

void Inlopen_Los2(count fc1,        /* fc1                                                                       */
                  count fc2,        /* fc2                                                                       */

                  count dk1bu,      /* drukknop fc1 buitenzijde                                                  */
                  count dk1bi,      /* drukknop fc1 middenberm                                                   */
                  count dk2bi,      /* drukknop fc2 middenberm                                                   */
                  count dk2bu,      /* drukknop fc2 buitenzijde                                                  */

                  count hdk1bu,     /* hulpelement drukknop fc1 buitenzijde                                      */
                  count hdk1bi,     /* hulpelement drukknop fc1 middenberm                                       */
                  count hdk2bi,     /* hulpelement drukknop fc2 middenberm                                       */
                  count hdk2bu,     /* hulpelement drukknop fc2 buitenzijde                                      */

                  count hinl1,      /* hulpelement fc1 inlopen gewenst                                           */
                  count hinl2,      /* hulpelement fc2 inlopen gewenst                                           */

                  count hlos1,      /* hulpelement fc1 los toegestaan                                            */
                  count hlos2,      /* hulpelement fc2 los teogestaan                                            */

                  boolv  sch1_1,     /* fc1   los, bij DK1_bi en DK1_bu                                           */
                  boolv  sch1_2,     /* fc1   los, bij DK1_bi en DK2_bu       (A in rug tbv naloop)               */
                  boolv  sch2_1,     /* fc2   los, bij DK2_bi en DK2_bu                                           */
                  boolv  sch2_2)     /* fc2   los, bij DK2_bi en DK1_bu       (A in rug tbv naloop)               */
{
  /* ------------------------------------------------------------------------------------------------------------------------------ */
  /* Bij een twee-voudige oversteek met 4 drukknoppen zijn er  7 mogelijkheden waarbij er:                                          */
  /* - alleen in het midden                                                                                                         */
  /* - of in het midden i.c.m. 1 andere drukknop is gedrukt.                                                                        */
  /*                                                                                                                                */
  /* DEZE MOGELIJKHEDEN ZIJN AFGELEID UIT DE MOGELIJKHEDEN BIJ EEN DRIE-VOUDIGE OVERSTEEK. DE NUMMERING VAN DE SITUATIES KOMT OOK   */
  /* OVEREEN MET DIE VAN EEN DRIE-VOUDIGE OVERSTEEK.                                                                                */
  /*                                                                                                                                */
  /* Wanneer er op een binnendrukknop is gedrukt is het afhankelijk van de andere drukknoppen wat er moet gebeuren.                 */
  /* E.e.a. staat hieronder uitgewerkt:                                                                                             */
  /* ------------------------------------------------------------------------------------------------------------------------------ */
  /*    |      fc1      | middenberm  |      fc2      | fc1 |     | fc2 |           |                                               */
  /*    |DK1_bu - DK1_bi|             |DK2_bi - DK2_bu|     |     |     |           |               toelichting                     */
  /* ---|---------------|-------------|---------------|-----|-----|-----|-----------|---------------------------------------------- */
  /*  1:|           X   |             |               | LOS |     |     |           | LOS = LOS                                     */
  /*  2:|   X       X   |             |               | SCH |     |     |           | SCH = LOS SCHAKELBAAR                         */
  /*  3:|           X   |             |   X           | LOS |     | LOS |           | INL = INLOPEN                                 */
  /*  4:|           X   |             |           X   | SCH |     | INL |           |                                               */
  /* ---|---------------|-------------|---------------|-----|-----|-----|-----------|---------------------------------------------- */
  /*  7:|               |             |   X           |     |     | LOS |           |                                               */
  /*  8:|               |             |   X       X   |     |     | SCH |           |                                               */
  /*  3:|           X   |             |   X           | LOS |     | LOS |           |                                               */
  /*  9:|   X           |             |   X           | INL |     | SCH |           |                                               */
  /* ---|---------------|-------------|---------------|-----|-----|-----|-----------|---------------------------------------------- */

  boolv SITUATIE[10]={0};

  /* ----------------------------------------------------------------------------- */
  /* Bepaal of drukknop bediend is                                                 */
  /* ----------------------------------------------------------------------------- */
  IH[hdk1bu] = G[fc1] ? FALSE : D[dk1bu] && A[fc1] ? TRUE : IH[hdk1bu];
  IH[hdk1bi] = G[fc1] ? FALSE : D[dk1bi] && A[fc1] ? TRUE : IH[hdk1bi];
  IH[hdk2bi] = G[fc2] ? FALSE : D[dk2bi] && A[fc2] ? TRUE : IH[hdk2bi];
  IH[hdk2bu] = G[fc2] ? FALSE : D[dk2bu] && A[fc2] ? TRUE : IH[hdk2bu];

  /* ----------------------------------------------------------------------------- */
  /* Vaststellen situaties:                                                        */
  /* - Er kunnen meerdere situaties tegelijk actief zijn, behalve 1-7-12-16        */
  /* ----------------------------------------------------------------------------- */
  SITUATIE[ 1] |= !IH[hdk1bu] &&  IH[hdk1bi] &&                             !IH[hdk2bi] && !IH[hdk2bu];
  SITUATIE[ 2] |=  IH[hdk1bu] &&  IH[hdk1bi]                                                          ;
  SITUATIE[ 3] |=                 IH[hdk1bi] &&                              IH[hdk2bi]               ;
  SITUATIE[ 4] |=                 IH[hdk1bi] &&                                             IH[hdk2bu];

  SITUATIE[ 7] |= !IH[hdk1bu] && !IH[hdk1bi] &&                              IH[hdk2bi] && !IH[hdk2bu];
  SITUATIE[ 8] |=                                                            IH[hdk2bi] &&  IH[hdk2bu];
  SITUATIE[ 9] |=  IH[hdk1bu]                                            &&  IH[hdk2bi]               ;

  /* ------------------------------------------------------------------------------------------------- */
  /* Vaststellen maatgevende acties:                                                                   */
  /* - in eerste instantie in principe "los" bij drukknop in midden en geen conflicterend CV           */
  /* - daarna los intrekken als er een beperking is                                                    */
  /* - SITUATIE 1-3-7 hebben geen beperking                                                            */
  /* ------------------------------------------------------------------------------------------------- */
  IH[hlos1] =  IH[hdk1bi]               && R[fc1] && !kcv(fc1) ? TRUE : FALSE;
  IH[hlos2] =  IH[hdk2bi]               && R[fc2] && !kcv(fc2) ? TRUE : FALSE;

  if(SITUATIE[ 1]           )  IH[hlos1] = IH[hlos1];  /* DK1_bi    single, geen beperkingen           */
  if(SITUATIE[ 2] && !sch1_1)  IH[hlos1] = FALSE;      /* DK1_bi en DK1_bu                             */
  if(SITUATIE[ 3]           )  IH[hlos1] = IH[hlos1];  /* DK1_bi en DK2_b1, geen beperkingen           */
  if(SITUATIE[ 4] && !sch1_2)  IH[hlos1] = FALSE;      /* DK1_bi en DK2_bu                             */

  if(SITUATIE[ 7]           )  IH[hlos2] = IH[hlos2];  /* DK2_bi    single, geen beperkingen           */
  if(SITUATIE[ 8] && !sch2_1)  IH[hlos2] = FALSE;      /* DK2_bi en DK2_bu                             */
  if(SITUATIE[ 9] && !sch2_2)  IH[hlos2] = FALSE;      /* DK2_bi en DK1_bu                             */

  /* ----------------------------------------------------------------------------- */
  /* Bepaal of inlopen gewenst is:                                                 */
  /* ----------------------------------------------------------------------------- */
  IH[hinl1]   = EG[fc1] ? FALSE : R[fc1] ? IH[hdk1bu] && !IH[hlos1] : IH[hinl1]  ;
  IH[hinl2]   = EG[fc2] ? FALSE : R[fc2] ? IH[hdk2bu] && !IH[hlos2] : IH[hinl2]  ;
}

boolv VTG3_Real_Los(count fc1,        /* fc1                                                                       */
                   count fc9,        /* fc9, middelste fc                                                         */
                   count fc2,        /* fc2                                                                       */

                   mulv  t1_t2,      /* inlooptijd  fc1 op fc2                                                    */
                   mulv  t2_t1,      /* inlooptijd  fc2 op fc1                                                    */
                   mulv  t1_t9,      /* inlooptijd  fc1 op fc9                                                    */
                   mulv  t9_t1,      /* inlooptijd  fc9 op fc1                                                    */
                   mulv  t2_t9,      /* inlooptijd  fc2 op fc9                                                    */
                   mulv  t9_t2,      /* inlooptijd  fc9 op fc2                                                    */

                   count hinl1,      /* hulpelement fc1 inlopen gewenst                                           */
                   count hinl9_1,    /* hulpelement fc9 inlopen gewenst op fc1                                    */
                   count hinl9_2,    /* hulpelement fc9 inlopen gewenst op fc2                                    */
                   count hinl2,      /* hulpelement fc2 inlopen gewenst                                           */

                   count hlos1,      /* hulpelement fc1 los toegestaan                                            */
                   count hlos9,      /* hulpelement fc9 los toegestaan                                            */
                   count hlos2,      /* hulpelement fc2 los teogestaan                                            */

                   boolv  gelijk1_2,  /* extra voorwaarde t.b.v. gelijkstart                                       */
                   boolv  gelijk1_9,  /* extra voorwaarde t.b.v. gelijkstart                                       */
                   boolv  gelijk2_9)  /* extra voorwaarde t.b.v. gelijkstart                                       */

{
  boolv result = 0;

  /* ------------------------------------------------------------------------------------------------------------------------------------------------------------ */
  /* Bepalen hulpwaarde t.b.v. beide alleen mee-aanvraag, dan gelijkstart                                                                                         */
  /* ------------------------------------------------------------------------------------------------------------------------------------------------------------ */
  boolv alleen_MA_1 = A[fc1] && !(A[fc1] & ~(BIT4|BIT8));
  boolv alleen_MA_2 = A[fc2] && !(A[fc2] & ~(BIT4|BIT8));
  boolv alleen_MA_9 = A[fc9] && !(A[fc9] & ~(BIT4|BIT8));

  gelijk1_2 |= alleen_MA_1 && alleen_MA_2 || !A[fc1] && !A[fc2];  /* Sowieso gelijk als beide alleen mee-aanvraag.    */
  gelijk1_9 |= alleen_MA_1 && alleen_MA_9 || !A[fc1] && !A[fc9];  /* Bij beide geen A ook uitgaan van gelijk,         */
  gelijk2_9 |= alleen_MA_2 && alleen_MA_9 || !A[fc2] && !A[fc9];  /* van belang tbv eventueel onderling doorzetten PG */

  /* ------------------------------------------------------------------------------------------------------------------------------------------------------------ */
  /* Bepalen REALTIJD tbv gelijkstart/inlopen:                                                                                                                    */
  /* ------------------------------------------------------------------------------------------------------------------------------------------------------------ */

  /* correcties tussen fc1 en fc2                                                                                                                                 */
       if(gelijk1_2              )      {  result |= Corr_Gel(fc1, fc2,         TRUE);                   /* beide alleen meeanvraag, gelijkstart                  */
                                        }
  else if(IH[hinl1] &&  IH[hinl2])      {  result |= Corr_Min(fc2, fc1, t1_t2,  TRUE);                   /* beide buitendrukknoppen, fc1 inlopen op fc2           */
                                           result |= Corr_Min(fc1, fc2, t2_t1,  TRUE);                   /*                          fc2 inlopen op fc1           */
                                        }
  else if(IH[hinl1] && !IH[hlos2])      {  result |= Corr_Min(fc2, fc1, t1_t2,  TRUE);                   /* dkfc1_bu               , fc1 inlopen op fc2           */
                                           result |= Corr_Min(fc1, fc2,     0,  TRUE);                   /*                        , fc2 wacht   op fc1           */
                                        }
  else if(IH[hinl1] &&  IH[hlos2])      {  result |= Corr_Min(fc2, fc1, t1_t2,  TRUE);                   /* dkfc1_bu               , fc1 inlopen op fc2           */
                                           result |= Corr_Min(fc1, fc2,     0, FALSE);                   /*                        , fc2 los van    fc1           */
                                        }
  else if(IH[hinl2] && !IH[hlos1])      {  result |= Corr_Min(fc2, fc1,     0,  TRUE);                   /* dkfc2_bu               , fc1 wacht   op fc2           */
                                           result |= Corr_Min(fc1, fc2, t2_t1,  TRUE);                   /*                        , fc2 inlopen op fc1           */
                                        }
  else if(IH[hinl2] &&  IH[hlos1])      {  result |= Corr_Min(fc2, fc1,     0, FALSE);                   /*                        , fc1 los van    fc2           */
                                           result |= Corr_Min(fc1, fc2, t2_t1,  TRUE);                   /* dkfc2_bu               , fc2 inlopen op fc1           */
                                        }
  else                                  {  result |= Corr_Gel(fc1, fc2,        FALSE);                   /* beide los van elkaar, t.b.v. reset REAL_SYN[][]       */
                                        }
  /* correcties tussen fc1 en fc9                                                                                                                                 */
       if(gelijk1_9                  )  {  result |= Corr_Gel(fc1, fc9,         TRUE);                   /* beide alleen meeanvraag, gelijkstart                  */
                                        }
  else if(IH[hinl1  ] &&  IH[hinl9_1])  {  result |= Corr_Min(fc9, fc1, t1_t9,  TRUE);                   /* beide buitendrukknoppen, fc1 inlopen op fc9           */
                                           result |= Corr_Min(fc1, fc9, t9_t1,  TRUE);                   /*                          fc9 inlopen op fc1           */
                                        }
  else if(IH[hinl1  ] && !IH[hlos9  ])  {  result |= Corr_Min(fc9, fc1, t1_t9,  TRUE);                   /* dkfc1_bu               , fc1 inlopen op fc9           */
                                           result |= Corr_Min(fc1, fc9,     0,  TRUE);                   /*                        , fc9 wacht   op fc1           */
                                        }
  else if(IH[hinl1  ] &&  IH[hlos9  ])  {  result |= Corr_Min(fc9, fc1, t1_t9,  TRUE);                   /* dkfc1_bu               , fc1 inlopen op fc9           */
                                           result |= Corr_Min(fc1, fc9,     0, FALSE);                   /*                        , fc9 los van    fc1           */
                                        }
  else if(IH[hinl9_1] && !IH[hlos1  ])  {  result |= Corr_Min(fc9, fc1,     0,  TRUE);                   /* dkfc9_bu               , fc1 wacht   op fc9           */
                                           result |= Corr_Min(fc1, fc9, t9_t1,  TRUE);                   /*                        , fc9 inlopen op fc1           */
                                        }
  else if(IH[hinl9_1] &&  IH[hlos1  ])  {  result |= Corr_Min(fc9, fc1,     0, FALSE);                   /* dkfc9_bu               , fc9 inlopen op fc1           */
                                           result |= Corr_Min(fc1, fc9, t9_t1,  TRUE);                   /*                        , fc1 los van    fc9           */
                                        }
  else if(IH[hinl2  ]                )  {  result |= Corr_Min(fc9, fc1,     0,  TRUE);                   /* dkfc2_bu               , fc1 wacht   op fc9           */
                                           result |= Corr_Min(fc1, fc9, t9_t1,  TRUE);                   /*                        , fc9 inlopen op fc1           */
                                        }
  else if(alleen_MA_1 && RA[fc9]     )  {  result |= Corr_Min(fc9, fc1,     0,  TRUE);                   /* alleen_MA_1            , fc1 wacht   op fc9           */
                                           result |= Corr_Min(fc1, fc9,     0, FALSE);                   /*                        , fc9 los van    fc1           */
                                        }
  else                                  {  result |= Corr_Gel(fc1, fc9,        FALSE);                   /* beide los van elkaar, t.b.v. reset REAL_SYN[][]       */
                                        }

  /* correcties tussen fc2 en fc9                                                                                                                                */
       if(gelijk2_9                  )  {  result |= Corr_Gel(fc2, fc9,         TRUE);                   /* beide alleen meeanvraag, gelijkstart                  */
                                        }
  else if(IH[hinl2  ] &&  IH[hinl9_2])  {  result |= Corr_Min(fc9, fc2, t2_t9,  TRUE);                   /* beide buitendrukknoppen, fc2 inlopen op fc9           */
                                           result |= Corr_Min(fc2, fc9, t9_t2,  TRUE);                   /*                          fc9 inlopen op fc2           */
                                        }
  else if(IH[hinl2  ] && !IH[hlos9  ])  {  result |= Corr_Min(fc9, fc2, t2_t9,  TRUE);                   /* dkfc2_bu               , fc2 inlopen op fc9           */
                                           result |= Corr_Min(fc2, fc9,     0,  TRUE);                   /*                        , fc9 wacht   op fc2           */
                                        }
  else if(IH[hinl2  ] &&  IH[hlos9  ])  {  result |= Corr_Min(fc9, fc2, t2_t9,  TRUE);                   /* dkfc2_bu               , fc2 inlopen op fc9           */
                                           result |= Corr_Min(fc2, fc9,     0, FALSE);                   /*                        , fc9 los van    fc2           */
                                        }
  else if(IH[hinl9_2] && !IH[hlos2  ])  {  result |= Corr_Min(fc9, fc2,     0,  TRUE);                   /* dkfc9_bu               , fc2 wacht   op fc9           */
                                           result |= Corr_Min(fc2, fc9, t9_t2,  TRUE);                   /*                        , fc9 inlopen op fc2           */
                                        }
  else if(IH[hinl9_2] &&  IH[hlos2  ])  {  result |= Corr_Min(fc9, fc2,     0, FALSE);                   /* dkfc9_bu               , fc9 inlopen op fc2           */
                                           result |= Corr_Min(fc2, fc9, t9_t2,  TRUE);                   /*                        , fc2 los van    fc9           */
                                        }
  else if(IH[hinl1  ]                )  {  result |= Corr_Min(fc9, fc2,     0,  TRUE);                   /* dkfc1_bu               , fc2 wacht   op fc9           */
                                           result |= Corr_Min(fc2, fc9, t9_t2,  TRUE);                   /*                        , fc9 inlopen op fc2           */
                                        }
  else if(alleen_MA_2 && RA[fc9]     )  {  result |= Corr_Min(fc9, fc2,     0,  TRUE);                   /* alleen_MA_2            , fc2 wacht   op fc9           */
                                           result |= Corr_Min(fc2, fc9,     0, FALSE);                   /*                        , fc9 los van    fc2           */
                                        }
  else                                  {  result |= Corr_Gel(fc2, fc9,        FALSE);                   /* beide los van elkaar, t.b.v. reset REAL_SYN[][]       */
                                        }

  /* --------------------------------- */
  /* Wijzigingen aangeven via result   */
  /* --------------------------------- */
  return result;
}

boolv VTG2_Real_Los(count fc1,        /* fasecyclus 1                                       */
                   count fc2,        /* fasecyclus 2                                       */
                   mulv  t1_t2,      /* inlooptijd  fc1 op fc2                             */
                   mulv  t2_t1,      /* inlooptijd  fc2 op fc1                             */
                   count hinl1,      /* hulpelement fc1 inlopen gewenst                    */
                   count hinl2,      /* hulpelement fc2 inlopen gewenst                    */
                   count hlos1,      /* hulpelement fc1 los toegestaan                     */
                   count hlos2,      /* hulpelement fc2 los teogestaan                     */
                   boolv  gelijk)     /* extra voorwaarde t.b.v. gelijkstart                */
{
  boolv result = 0;

  gelijk |= A[fc1] && !(A[fc1] & ~(BIT4|BIT8)) &&  /* Sowieso gelijk als beide alleen mee-aanvraag.   */
            A[fc2] && !(A[fc2] & ~(BIT4|BIT8)) ||

           !A[fc1] && !A[fc2];                     /* Bij beide geen A ook uitgaan van gelijk,         */
                                                   /* van belang tbv eventueel onderling doorzetten PG */
  /* -------------------------------------------------------------------------------------------------------------------------------------------------------- */
  /* tbv gelijkstart/inlopen                                                                                                                                  */
  /* -------------------------------------------------------------------------------------------------------------------------------------------------------- */
       if(gelijk                 )  {  result |= Corr_Gel(fc1, fc2,         TRUE);                       /* beide alleen meeanvraag, gelijkstart              */
                                    }
  else if(IH[hinl1] &&  IH[hinl2])  {  result |= Corr_Min(fc2, fc1, t1_t2,  TRUE);                       /* beide buitendrukknoppen, fc1 inlopen op fc2       */
                                       result |= Corr_Min(fc1, fc2, t2_t1,  TRUE);                       /*                          fc2 inlopen op fc1       */
                                    }
  else if(IH[hinl1] && !IH[hlos2])  {  result |= Corr_Min(fc2, fc1, t1_t2,  TRUE);                       /* dkfc1_bu               , fc1 inlopen op fc2       */
                                       result |= Corr_Min(fc1, fc2,     0,  TRUE);                       /*                        , fc2 wacht   op fc1       */
                                    }
  else if(IH[hinl1] &&  IH[hlos2])  {  result |= Corr_Min(fc2, fc1, t1_t2,  TRUE);                       /* dkfc1_bu               , fc1 inlopen op fc2       */
                                       result |= Corr_Min(fc1, fc2,     0, FALSE);                       /*                        , fc2 los van    fc1       */
                                    }
  else if(IH[hinl2] && !IH[hlos1])  {  result |= Corr_Min(fc2, fc1,     0,  TRUE);                       /* dkfc2_bu               , fc1 wacht   op fc2       */
                                       result |= Corr_Min(fc1, fc2, t2_t1,  TRUE);                       /*                        , fc2 inlopen op fc1       */
                                    }
  else if(IH[hinl2] &&  IH[hlos1])  {  result |= Corr_Min(fc2, fc1,     0, FALSE);                       /*                        , fc1 los van    fc2       */
                                       result |= Corr_Min(fc1, fc2, t2_t1,  TRUE);                       /* dkfc2_bu               , fc2 inlopen op fc1       */
                                    }
  else                              {  result |= Corr_Gel(fc1, fc2,        FALSE);                       /* beide los van elkaar, t.b.v. reset REAL_SYN[][]   */
                                    }

  /* --------------------------------- */
  /* Wijzigingen aangeven via result   */
  /* --------------------------------- */
  return result;
}

bool Naloop_OK(count fc1,     /* fc1  voedende                                                      */
               count marfc2,  /* memory element fc2, alternatieve ruimte (max_tar_to / tar_max_ple) */
               count tnlsg)   /* nalooptijd                                                         */
{
  bool result=0;

  /* ---------------------------------------------------------------- */
  /* primair, dus nalooptijd toegestaan                               */
  /* ---------------------------------------------------------------- */
/*if(AAPR[fc1] &&   !RR[fc1]       || PR[fc1])           result=TRUE;*/
  if(AAPR[fc1] && (AAPR[fc1]<BIT4) || PR[fc1])           result=TRUE;    /* AAPR & BIT4 betekent RR, AAPR & BIT5 betekent PFPR nog niet waar */
  /* ---------------------------------------------------------------- */
  /* niet primair, bepaal of nalooptijd past bij naloop:              */
  /* - eerst moet fc1 op groen komen, dus check REALTIJD[fc1]         */
  /* - plus de nalooptijd moet passen bij fc2                         */
  /* ---------------------------------------------------------------- */
  else if(MM[marfc2] >= (REALTIJD[fc1] + T_max[tnlsg]))  result=TRUE;
  /* ---------------------------------------------------------------- */

  return result;
}
