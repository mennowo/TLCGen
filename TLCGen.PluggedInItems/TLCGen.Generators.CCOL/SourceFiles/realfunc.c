/*
BESTAND:   realfunc.c
*/

/****************************** Versie commentaar ***********************************
*
* TLCGen  Datum       Naam          Commentaar
*
* 1.0     05-08-2020  OK Geregeld:  Functies t.b.v. realisatietijd en correcties
* 2.0     07-03-2021  Cyril         Geschikt gemaakt voor UC4
*                                   - CCA/PSN    04022021: REALTIJD_min REALTIJD_max ook rekening houden met fictieve OT deelconflictrichtingen
*                                   - CCA/PSN    04022021: min max tijd toegevoegd
*                                   - CCA        10022021: kijken naar fictieve OT toegevoegd
*                                   - CCA/Ddo    04032021: (AAPR[fc1] && (AAPR[fc1]<BIT6)) aangevuld bij AAPR[fc1]
*                                   - CCA/Ddo    07032021: Alleen ophogin minend en maxend tijd toegestaan
*                                   - CCA/DDo    08032021: && !(P[fc1] || P[fc2])
* 3.0     14-04-2021                - CCA        14042021: Aanpassingen Steven van Oostendorp verwerkt:
*                                                          Bugfix REALTIJD[fc2] ivm voorstart > 0, regel 217(11112020)
*                                                          Bij realtijd<=1 geen sync als RV[fc1] , regel 204 (07122020) 
*                                                          VTG3_Real_Los gewijzigd                                                   
*                                                          Minder argumenten voetgangersfuncties + detailwijzigingen (10032021) 
* 3.1     16-10-2021                - CCA        Corr_Min_nl gemaakt waarniet naar de aanwezigheid voor A voor de naloop wordt gekeken
*                                                _temp interne variabelen verwijderd (CCA/Ddo 07032021)
* 3.2     09-11-2021                - CCA        MG && TGG toegevoegd in berekeningen
* 3.3     06-12-2021                - CCA        _temp interne variabelen wederom toegevoegd
* 3.4     09-03-2023                - CCA        MLNLTEST toegevoegd ivm onterechte PG (optioneel zelf te activeren)
* 3.5     07-04-2023                - CCA        MLNLTEST register count toegevoegd
* 3.6     04-03-2023                - CCA/AW/DDO Aanpassing Synchroniseer_FO met een controle dat beide fc's moeten in dezelfde module moeten zitten
************************************************************************************/

mulv REALTIJD[FCMAX];
mulv REALTIJD_uncorrected[FCMAX];
mulv REALTIJD_max[FCMAX];
mulv REALTIJD_min[FCMAX];
bool REAL_SYN[FCMAX][FCMAX];  /* Vlag tbv synchronisatie      obv REALTIJD */
                               /* BIT1 TRUE/FALSE                           */
                               /* BIT2 correctie gelijk (extra info)        */
                               /* BIT3 correctie plus   (extra info)        */
                               /* BIT4 correctie min    (extra info)        */
bool REAL_FOT[FCMAX][FCMAX];  /* Vlag tbv fictieve ontruiming obv REALTIJD */
mulv TIME_FOT[FCMAX][FCMAX];   /* Timer tbv fictieve ontruiming, FOT loopt, resterende tijd */

/* ========================================================================================================================================================================================================== */
/* REALISATIETIJD ALGEMEEN                                                                                                                                                                                    */
/* ========================================================================================================================================================================================================== */
void Realisatietijd(count fc, count hsignaalplan, mulv correctie_sp)    //@@ warning C4100: 'correctie_sp' : unreferenced formal parameter
{
  register count n, k;

  mulv conflicttijd;
  mulv eigentijd;
  mulv eigentijd_uncorrected;

  REALTIJD[fc] = 0;  /* realisatietijd resetten */
  REALTIJD_uncorrected[fc] = 0;  /* niet-gecorrigeerde realisatietijd resetten */

  /* hoogste realisatietijd berekenen */
  for (n=0; n<FKFC_MAX[fc]; n++)
  {
     conflicttijd = 0;

#if (CCOL_V >= 95)
     k = KF_pointer[fc][n];
#else
     k = TO_pointer[fc][n];
#endif

     /* ------------------------------------------------------------ */
     /* ALS CONFLICT actief IS DIENT CONFLICTTIJD BEPAALD TE WORDEN: */
     /* - RA[k]     rood na aanvraagafwikkeling                      */
     /* - VS[k]     voorstartgroen                                   */
     /* - TO[k][fc] ontruiming actief (vanaf groen)                  */
     /* ------------------------------------------------------------ */
     if(/*RA[k] ||*/                /* (fictief) conflict in RA of   */
        VS[k] && (RS[k] || YS[k]))  /* VS, dan hoge conflicttijd     */
     {
         conflicttijd =  9999;
     }
#if (CCOL_V >= 95) && !defined NO_TIGMAX
     else if(TIG[k][fc])
#else
     else if(TO[k][fc])
#endif
     {
       /* ---------------------------------------------------------- */
       /* conflicttijd groen-   conflicten bepalen                   */
       /* ---------------------------------------------------------- */
#if (CCOL_V >= 95) && !defined NO_TIGMAX
       if(TIG_max[k][fc]==GK || TIG_max[k][fc]==GKL)
#else
       if(TO_max[k][fc]==GK || TO_max[k][fc]==GKL)
#endif
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
#if (CCOL_V >= 95) && !defined NO_TIGMAX
       else if((TIG_max[k][fc]>=0))
#else
       else if((TO_max[k][fc]>=0))
#endif
       {
         conflicttijd = ( G[k] && !MG[k])    ? TFG_max[k] - TFG_timer[k] +                                         /* conflict G (uitgezonderd MG) */
                                               TVG_max[k] - TVG_timer[k] +
#if (CCOL_V >= 95) && !defined NO_TIGMAX
                                                TIG_max[k][fc]                               :
                          (MG[k] && !TGG[k]) ?  TIG_max[k][fc]                               :                     /* conflict MG !TGG */
                          (MG[k] &&  TGG[k]) ?  TIG_max[k][fc] + (TGG_max[k] - TGG_timer[k]) :                     /* conflict MG  TGG */
                                  TIG[k][fc] ?  TIG_max[k][fc] - TIG_timer[k]                : 0;                  /* ontruimen        */
#else
                                  TGL_max[k] +  TO_max[k][fc]                                :
                          (MG[k] && !TGG[k]) ?  TGL_max[k] +                                                       /* conflict MG !TGG */
                                                 TO_max[k][fc]                               :
                          (MG[k] &&  TGG[k]) ?  TGL_max[k] + (TGG_max[k] - TGG_timer[k]) +                         /* conflict MG TGG */
                                                 TO_max[k][fc]                               :
                                       GL[k] ?  TGL_max[k] - TGL_timer[k] +                                        /* conflict GL     */
                                                 TO_max[k][fc]                               :
                                   TO[k][fc] ?  TO_max[k][fc] - TO_timer[k]                  : 0;                  /* ontruimen       */

#endif

#if PLMAX
         if(hsignaalplan!=NG && correctie_sp!=NG && IH[hsignaalplan])  /* tijdens signaalplan en groen conflicttijd aanpassen */
         {
#if (CCOL_V >= 95) && !defined NO_TIGMAX
           if(G[k] && PR[k] && !MG[k] && (conflicttijd < (TOTXD_PL[k]- correctie_sp + TIG_max[k][fc])))
                                          conflicttijd = (TOTXD_PL[k]- correctie_sp + TIG_max[k][fc]);
#else
           if(G[k] && PR[k] && !MG[k] && (conflicttijd < (TOTXD_PL[k]- correctie_sp + TGL_max[k] + TO_max[k][fc])))
                                          conflicttijd = (TOTXD_PL[k]- correctie_sp + TGL_max[k] + TO_max[k][fc]);
#endif

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
     REALTIJD_uncorrected[fc] = !G[fc] && conflicttijd > REALTIJD_uncorrected[fc] ? conflicttijd : REALTIJD_uncorrected[fc];
  }

  /* -------------------------------------------------------------------------- */
  /* BEPAAL EIGEN TIJD:                                                         */
  /*                                                                            */
  /* 9999 (realisatijd heel hoog zetten) als:                                   */
  /* - BL                                                                       */
  /* - RR vanaf BIT2 en BIT0, BIT1/BIT2 worden gebruikt door Synchroniseer():   */
  /*   - BIT1 tegenhouden startgroen obv realisatietijd                         */
  /*   - BIT2 tegenhouden startgroen obv fictieve ontruimingstijd               */
  /*                                                                            */
  /* - anders resterend geeltijd + garantie roodtijd berekenen (+ 1 rondje RA)  */
  /* -------------------------------------------------------------------------- */
  eigentijd =     BL[fc]                                               ? 9999  :
#if CCOL_V >= 110
                  !(P[fc] & BIT11) &&
#endif 
                  (RR[fc] >= BIT2 || (RR[fc] & BIT0))        ? 9999  : 

                  GL[fc]                                   ? TGL_max[fc] - TGL_timer[fc] + TRG_max[fc]                 + 1 :
                 TRG[fc]                                   ?                               TRG_max[fc] - TRG_timer[fc] + 1 :
                  RV[fc]                                   ?                                                             1 : 0;
                  
eigentijd_uncorrected =     
                  GL[fc]                                   ? TGL_max[fc] - TGL_timer[fc] + TRG_max[fc]                 + 1 :
                 TRG[fc]                                   ?                               TRG_max[fc] - TRG_timer[fc] + 1 :
                  RV[fc]                                   ?                                                             1 : 0;

  /* --------------------------------------------------------- */
  /* REALISATIETIJD BEPALEN ALS EIGENTIJD MAATGEVEND BLIJKT    */
  /* --------------------------------------------------------- */
  REALTIJD[fc] = !G[fc] && eigentijd > REALTIJD[fc] ? eigentijd : REALTIJD[fc];
  REALTIJD_uncorrected[fc] = !G[fc] && eigentijd_uncorrected > REALTIJD_uncorrected[fc] ? eigentijd_uncorrected : REALTIJD_uncorrected[fc];
}


/* ========================================================================================================================================================================================================== */
/* REALISATIETIJD ALGEMEEN                                                                                                                                                                                    */
/* ========================================================================================================================================================================================================== */
void Realisatietijd_min(count fc, count hsignaalplan, mulv correctie_sp)      //@@ warning C4100: 'correctie_sp' : unreferenced formal parameter
{
   register count n, k;

   mulv conflicttijd;
   mulv eigentijd;

   REALTIJD_min[fc] = 0;  /* realisatietijd resetten */

                          /* hoogste realisatietijd berekenen */
   for (n=0; n<FKFC_MAX[fc]; n++)
   {
      conflicttijd = 0;

#if (CCOL_V >= 95)
      k = KF_pointer[fc][n];
#else
      k = TO_pointer[fc][n];
#endif

      /* ------------------------------------------------------------ */
      /* ALS CONFLICT actief IS DIENT CONFLICTTIJD BEPAALD TE WORDEN: */
      /* - RA[k]     rood na aanvraagafwikkeling                      */
      /* - VS[k]     voorstartgroen                                   */
      /* - TO[k][fc] ontruiming actief (vanaf groen)                  */
      /* ------------------------------------------------------------ */
      if(/*RA[k] ||*/                /* (fictief) conflict in RA of   */
         VS[k] && (RS[k] || YS[k]))  /* VS, dan hoge conflicttijd     */
      {
         conflicttijd =  9999;
      }
#if (CCOL_V >= 95) && !defined NO_TIGMAX
      else if(TIG[k][fc])
#else
      else if(TO[k][fc])
#endif
      {
         /* ---------------------------------------------------------- */
         /* conflicttijd groen-   conflicten bepalen                   */
         /* ---------------------------------------------------------- */
#if (CCOL_V >= 95) && !defined NO_TIGMAX
         if(TIG_max[k][fc]==GK || TIG_max[k][fc]==GKL)
#else
         if(TO_max[k][fc]==GK || TO_max[k][fc]==GKL)
#endif
         {
            conflicttijd = ( G[k] && !MG[k]) ? TFG_max[k] - TFG_timer[k] : 0;

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
#if (CCOL_V >= 95) && !defined NO_TIGMAX
         else if((TIG_max[k][fc]>=0))
#else
         else if((TO_max[k][fc]>=0))
#endif
         {
         conflicttijd = ( G[k] && !MG[k])    ? TFG_max[k] - TFG_timer[k] +                                         /* conflict G (uitgezonderd MG) */
#if (CCOL_V >= 95) && !defined NO_TIGMAX
                                                TIG_max[k][fc]                               :
                          (MG[k] && !TGG[k]) ?  TIG_max[k][fc]                               :                     /* conflict MG !TGG */
                          (MG[k] &&  TGG[k]) ?  TIG_max[k][fc] + (TGG_max[k] - TGG_timer[k]) :                     /* conflict MG  TGG */
                                  TIG[k][fc] ?  TIG_max[k][fc] - TIG_timer[k]                : 0;                  /* ontruimen        */
#else
                                  TGL_max[k] +  TO_max[k][fc]                                :
                          (MG[k] && !TGG[k]) ?  TGL_max[k] +                                                       /* conflict MG !TGG */
                                                 TO_max[k][fc]                               :
                          (MG[k] &&  TGG[k]) ?  TGL_max[k] + (TGG_max[k] - TGG_timer[k]) +                         /* conflict MG TGG */
                                                 TO_max[k][fc]                               :
                                       GL[k] ?  TGL_max[k] - TGL_timer[k] +                                        /* conflict GL     */
                                                 TO_max[k][fc]                               :
                                   TO[k][fc] ?  TO_max[k][fc] - TO_timer[k]                  : 0;                  /* ontruimen       */

#endif

#if PLMAX
            if(hsignaalplan!=NG && correctie_sp!=NG && IH[hsignaalplan])  /* tijdens signaalplan en groen conflicttijd aanpassen */
            {
#if (CCOL_V >= 95) && !defined NO_TIGMAX
               if(G[k] && PR[k] && !MG[k] && (conflicttijd < (TOTXD_PL[k]- correctie_sp + TIG_max[k][fc])))
                  conflicttijd = (TOTXD_PL[k]- correctie_sp + TIG_max[k][fc]);
#else
               if(G[k] && PR[k] && !MG[k] && (conflicttijd < (TOTXD_PL[k]- correctie_sp + TGL_max[k] + TO_max[k][fc])))
                  conflicttijd = (TOTXD_PL[k]- correctie_sp + TGL_max[k] + TO_max[k][fc]);
#endif

               /* Voor alternatief tijdens signaalplan nog geen rekening gehouden met resterrend groen. */
               /* Voor nu mag een alernatief zsm afgebroken worden.                                     */
            }
#endif
         }
      }

      /* --------------------------------------------------------- */
      /* REALISATIETIJD BEPALEN ALS CONFLICTTIJD MAATGEVEND BLIJKT */
      /* --------------------------------------------------------- */
      REALTIJD_min[fc] = !G[fc] && conflicttijd > REALTIJD_min[fc] ? conflicttijd : REALTIJD_min[fc];
   }

   /* -------------------------------------------------------------------------- */
   /* BEPAAL EIGEN TIJD:                                                         */
   /*                                                                            */
   /* 9999 (realisatijd heel hoog zetten) als:                                   */
   /* - BL                                                                       */
   /* - RR vanaf BIT2 en BIT0, BIT1/BIT2 worden gebruikt door Synchroniseer():   */
   /*   - BIT1 tegenhouden startgroen obv realisatietijd                         */
   /*   - BIT2 tegenhouden startgroen obv fictieve ontruimingstijd               */
   /*                                                                            */
   /* - anders resterend geeltijd + garantie roodtijd berekenen (+ 1 rondje RA)  */
   /* -------------------------------------------------------------------------- */
   eigentijd =     BL[fc]                                            ? 9999                                    :
#if CCOL_V >= 110
      !(P[fc] & BIT11) &&
#else
                          (RR[fc] >= BIT2 || (RR[fc] & BIT0))        ? 9999                                    :
#endif

      GL[fc]                                   ? TGL_max[fc] - TGL_timer[fc] + TRG_max[fc]                 + 1 :
      TRG[fc]                                  ?                               TRG_max[fc] - TRG_timer[fc] + 1 :
      RV[fc]                                   ?                                                             1 : 0;

   /* --------------------------------------------------------- */
   /* REALISATIETIJD BEPALEN ALS EIGENTIJD MAATGEVEND BLIJKT    */
   /* --------------------------------------------------------- */
   REALTIJD_min[fc]     = !G[fc] && eigentijd    > REALTIJD_min[fc]     ? eigentijd    : REALTIJD_min[fc];
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
bool Corr_Real(count fc1,        /* fasecyclus 1                                       */
               count fc2,        /* fasecyclus 2                                       */
               mulv  t1_t2,      /* correctiewaarde van fc1 naar fc2                   */
               bool  period)     /* extra voorwaarde                                   */
{
  bool result = 0;
  mulv REALTIJD_temp;      
  mulv REALTIJD_min_temp;  
  mulv REALTIJD_max_temp;  
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
  /* - Bij een realtijd van <= 1 hoeft er geen synchronisatie plaats te vinden, als RV[fc1].       */
  /*   Fc1 had op dat moment al in RA moeten staan, er kan niet langer gewacht worden op fc1.      */
  /*   Dus dan geen correcties/synchronisaties obv fc1. De regeling moet door.                     */
  /* --------------------------------------------------------------------------------------------- */
  if (PG[fc1] && RV[fc1] && !TRG[fc1] && PR[fc2] && !PG[fc2]
#if CCOL_V >= 110 
     && !((P[fc1] & BIT11) || (P[fc2] & BIT11)) 
#endif
     || RV[fc1] && (REALTIJD[fc1] <= 1))
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
  if (REAL_SYN[fc1][fc2] && !G[fc2] && (A[fc1] || GL[fc1] || TRG[fc1]) && (REALTIJD[fc2] < (REALTIJD[fc1] + t1_t2)))
  {
     REALTIJD_temp =   !G[fc1] && REALTIJD[fc1] == 9999 ? 9999 :
                       !G[fc1] ? REALTIJD[fc1] + t1_t2 :
                       TGG[fc1] && (t1_t2 > TGG_timer[fc1]) ? t1_t2 - TGG_timer[fc1] : REALTIJD[fc2];               /* Geen aanpassing als garantiegroen[fc1] verstreken is    */
                                                                                                                    /* of als voorstarttijd verstreken.                        */
     if (REALTIJD_temp > REALTIJD[fc2]) REALTIJD[fc2] = REALTIJD_temp; /* alleen maar ophogen */                    /* Voorstarttijd groter dan TGG_max wordt niet ondersteund */
     result = TRUE;                                                                                           
  }

  if (REAL_SYN[fc1][fc2] && !G[fc2] && (A[fc1] || GL[fc1] || TRG[fc1]) && (REALTIJD_min[fc2] < (REALTIJD_min[fc1] + t1_t2)))
  {
     REALTIJD_min_temp =  !G[fc1] && REALTIJD_min[fc1] == 9999 ? 9999 :
                          !G[fc1] ? REALTIJD_min[fc1] + t1_t2 :
                          TGG[fc1] && (t1_t2 > TGG_timer[fc1]) ? t1_t2 - TGG_timer[fc1] : REALTIJD_min[fc2];        /* Geen aanpassing als garantiegroen[fc1] verstreken is    */
                                                                                                                    /* of als voorstarttijd verstreken.                        */
     if (REALTIJD_min_temp > REALTIJD_min[fc2]) REALTIJD_min[fc2] = REALTIJD_min_temp; /* alleen maar ophogen */    /* Voorstarttijd groter dan TGG_max wordt niet ondersteund */
     result = TRUE;                                                                                           
  }

    if (REAL_SYN[fc1][fc2] && !G[fc2] && /* (A[fc1] || !PG[fc1] || GL[fc1] || TRG[fc1]) &&*/ (REALTIJD_max[fc2] < (REALTIJD_max[fc1] + t1_t2)) && (REALTIJD_max[fc1]!=9999))
    {  
     REALTIJD_max_temp =  !G[fc1] && REALTIJD_max[fc1] == 9999 ? 9999 :  
                          !G[fc1] ? REALTIJD_max[fc1] + t1_t2 :
                          TGG[fc1] && (t1_t2 > TGG_timer[fc1]) ? t1_t2 - TGG_timer[fc1] : REALTIJD_max[fc2];        /* Geen aanpassing als garantiegroen[fc1] verstreken is    */
    if (REALTIJD_max_temp > REALTIJD_max[fc2]) REALTIJD_max[fc2] = REALTIJD_max_temp; /* alleen maar ophogen */    /* Voorstarttijd groter dan TGG_max wordt niet ondersteund */
    result = TRUE;                                                                                           
  }

  /* --------------------------------- */
  /* Wijzigingen aangeven via result   */
  /* --------------------------------- */
  return result;
}


/* ================================================================================================================================================================================== */
/* REALISATIETIJD CORRECTIE - NEW !!! - eenzijdig gemaakt - negatieve waarden kunnen als argument worden meegegeven                                                                   */
/* ================================================================================================================================================================================== */
bool Corr_Real_nl(count fc1,        /* fasecyclus 1                                       */
               count fc2,        /* fasecyclus 2                                       */
               mulv  t1_t2,      /* correctiewaarde van fc1 naar fc2                   */
               bool  period)     /* extra voorwaarde                                   */
{
  bool result = 0;
  mulv REALTIJD_temp;      
  mulv REALTIJD_min_temp;  
  mulv REALTIJD_max_temp;  
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
  if (PG[fc1] && RV[fc1] && !TRG[fc1] && PR[fc2] && !PG[fc2]
  #if CCOL_V >= 110 
      && !((P[fc1] & BIT11) || (P[fc2] & BIT11)) 
  #endif
     )
  {
     REAL_SYN[fc1][fc2] = period;
     PG[fc1] &= ~PRIMAIR_OVERSLAG; 
  }

  /* --------------------------------------------------------------------------------------------- */
  /* - Bij een realtijd van <= 1 hoeft er geen synchronisatie plaats te vinden, als RV[fc1].       */
  /*   Fc1 had op dat moment al in RA moeten staan, er kan niet langer gewacht worden op fc1.      */
  /*   Dus dan geen correcties/synchronisaties obv fc1. De regeling moet door.                     */
  /* --------------------------------------------------------------------------------------------- */
  if (RV[fc1] && (REALTIJD[fc1] <= 1))
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
  /* - fc1 ALTIJD                                                                                  */
  /* - realtijd fc2 kleiner dan die van fc1 + correctie                                            */
  if (REAL_SYN[fc1][fc2] && !G[fc2] /*&& (A[fc1] || GL[fc1] || TRG[fc1])*/ && (REALTIJD[fc2] < (REALTIJD[fc1] + t1_t2)))
  {
    REALTIJD_temp =   !G[fc1] && REALTIJD[fc1] == 9999 ? 9999 :
                       !G[fc1] ? REALTIJD[fc1] + t1_t2 :
                       TGG[fc1] && (t1_t2 > TGG_timer[fc1]) ? t1_t2 - TGG_timer[fc1] : REALTIJD[fc2];               /* Geen aanpassing als garantiegroen[fc1] verstreken is    */
                                                                                                                    /* of als voorstarttijd verstreken.                        */
     if (REALTIJD_temp > REALTIJD[fc2]) REALTIJD[fc2] = REALTIJD_temp; /* alleen maar ophogen */                    /* Voorstarttijd groter dan TGG_max wordt niet ondersteund */
     result = TRUE;                                                                                           
   }

  if (REAL_SYN[fc1][fc2] && !G[fc2] /*&& (A[fc1] || GL[fc1] || TRG[fc1])*/ && (REALTIJD_min[fc2] < (REALTIJD_min[fc1] + t1_t2)))
  {
     REALTIJD_min_temp =  !G[fc1] && REALTIJD_min[fc1] == 9999 ? 9999 :
                          !G[fc1] ? REALTIJD_min[fc1] + t1_t2 :
                          TGG[fc1] && (t1_t2 > TGG_timer[fc1]) ? t1_t2 - TGG_timer[fc1] : REALTIJD_min[fc2];        /* Geen aanpassing als garantiegroen[fc1] verstreken is    */
                                                                                                                    /* of als voorstarttijd verstreken.                        */
     if (REALTIJD_min_temp > REALTIJD_min[fc2]) REALTIJD_min[fc2] = REALTIJD_min_temp; /* alleen maar ophogen */    /* Voorstarttijd groter dan TGG_max wordt niet ondersteund */
     result = TRUE;                                                                                           
  }

    if (REAL_SYN[fc1][fc2] && !G[fc2] && /* (A[fc1] || !PG[fc1] || GL[fc1] || TRG[fc1]) &&*/ (REALTIJD_max[fc2] < (REALTIJD_max[fc1] + t1_t2)) && (REALTIJD_max[fc1]!=9999))
    {  
     REALTIJD_max_temp =  !G[fc1] && REALTIJD_max[fc1] == 9999 ? 9999 :  
                          !G[fc1] ? REALTIJD_max[fc1] + t1_t2 :
                          TGG[fc1] && (t1_t2 > TGG_timer[fc1]) ? t1_t2 - TGG_timer[fc1] : REALTIJD_max[fc2];        /* Geen aanpassing als garantiegroen[fc1] verstreken is    */
    if (REALTIJD_max_temp > REALTIJD_max[fc2]) REALTIJD_max[fc2] = REALTIJD_max_temp; /* alleen maar ophogen */    /* Voorstarttijd groter dan TGG_max wordt niet ondersteund */
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
bool Corr_Gel(count fc1,        /* fasecyclus 1                                       */
              count fc2,        /* fasecyclus 2                                       */
              bool  period)     /* extra voorwaarde                                   */
{
  bool result1 = 0;
  bool result2 = 0;

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
bool Corr_Pls(count fc1,        /* fasecyclus 1                                       */
              count fc2,        /* fasecyclus 2                                       */
              mulv  t1_t2,      /* correctiewaarde fc1 obv fc2                        */
              bool  period)     /* extra voorwaarde                                   */
{
  bool result = 0;

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
bool Corr_Min( count fc1,        /* fasecyclus 1                                       */
               count fc2,        /* fasecyclus 2                                       */
               mulv  t1_t2,      /* correctiewaarde fc1 obv fc2                        */
               bool  period)     /* extra voorwaarde                                   */
{
  bool result = 0;

  result |= Corr_Real(fc1, fc2, -t1_t2, period);

  /* --------------------------------- */
  /* Wijzigingen aangeven via result   */
  /* --------------------------------- */
  REAL_SYN[fc1][fc2] |= result ? BIT4 : 0;

  return result;
}

/* ================================================================================================================================================================================== */
/* REALISATIETIJD CORRECTIE - MIN (fc2 dus eerder groen dan fc1)                                                                                                                      */
/* ================================================================================================================================================================================== */
bool Corr_Min_nl( count fc1,        /* fasecyclus 1                                       */
   count fc2,        /* fasecyclus 2                                       */
   mulv  t1_t2,      /* correctiewaarde fc1 obv fc2                        */
   bool  period)     /* extra voorwaarde                                   */
{
   bool result = 0;

   result |= Corr_Real_nl(fc1, fc2, -t1_t2, period);

   /* --------------------------------- */
   /* Wijzigingen aangeven via result   */
   /* --------------------------------- */
   REAL_SYN[fc1][fc2] |= result ? BIT4 : 0;

   return result;
}

/* ================================================================================================================================================================================== */
/* REALISATIETIJD CORRECTIE DUBBELE VOETGANGERSOVERSTEEK (hulpfunctie die Corr_Real() aanroept afhankelijk van buitendrukknoppen of beide alleen mee-aanvraag)                        */
/* ================================================================================================================================================================================== */
bool VTG2_Real(count fc1,        /* fasecyclus 1                                       */
               count fc2,        /* fasecyclus 2                                       */
               mulv  t1_t2,      /* correctiewaarde fc1 tov fc2                        */
               mulv  t2_t1,      /* correctiewaarde fc2 tov fc1                        */
               count hdk1_bu,    /* hulpelement, drukknop fc1 buitenzijde              */
               count hdk2_bu,    /* hulpelement, drukknop fc2 buitenzijde              */
               bool  gelijk )    /* extra voorwaarde t.b.v. gelijkstart                */
{
  bool result = 0;

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
bool Corr_FOT(count fc1,     /* fasecyclus VAN                       */
              count fc2,     /* fasecyclus NAAR                      */
              count fot1_2,  /* fictieve ontruiming VAN fc1 NAAR fc2 */
              mulv  gg1,     /* TGG_timer[fc1] > gg1, dan RT[fot1_2] */
              bool  period)  /* extra voorwaarde                     */
{
  bool result = 0;
  mulv hulp   = 0;

  /* -------------------------------------------------------------------------- */
  /* Vlag tbv fictieve ontruimingstijd, van fc1 naar fc2                        */
  /* -------------------------------------------------------------------------- */
  REAL_FOT[fc1][fc2] = period;

  /* -------------------------------------------------------------------------- */
  /* Herstarten fictieve ontruimingstijden                                      */
  /* -------------------------------------------------------------------------- */
#if (CCOL_V >= 95) && !defined NO_TIGMAX
  RT[fot1_2] = (G[fc1] && (TGG_timer[fc1] > gg1 || !TGG[fc1])) && REAL_FOT[fc1][fc2];
#else
  RT[fot1_2] = (GL[fc1] || G[fc1] && (TGG_timer[fc1] > gg1 || !TGG[fc1])) && REAL_FOT[fc1][fc2];
#endif
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
#if (CCOL_V >= 95) && !defined NO_TIGMAX
     hulp = RT[fot1_2] && G[fc1] && !MG[fc1] ? TFG_max[fc1]    - TFG_timer[fc1] +
                                               TVG_max[fc1]    - TVG_timer[fc1] +
                                               TGL_max[fc1]                     +
                                                 T_max[fot1_2]                  :
            RT[fot1_2] &&            MG[fc1] ? TGL_max[fc1]                     +
                                                 T_max[fot1_2]                  :
                                     GL[fc1] ? TGL_max[fc1]    - TGL_timer[fc1] +
                                                 T_max[fot1_2]                  :
                                                 T_max[fot1_2] - T_timer[fot1_2];
#else
     hulp = RT[fot1_2] && G[fc1] && !MG[fc1] ? TFG_max[fc1]    - TFG_timer[fc1] +
                                               TVG_max[fc1]    - TVG_timer[fc1] +
                                                 T_max[fot1_2]                  :
            RT[fot1_2] &&            MG[fc1] ? TGL_max[fc1]                     +
                                                 T_max[fot1_2]                  :
                                                 T_max[fot1_2] - T_timer[fot1_2];
#endif
    if(REALTIJD[fc2] < hulp)
    {
      REALTIJD[fc2] = hulp;

      if (REALTIJD_min[fc2] < hulp) /* CCA30012021: ook voor realtijd_min */
      {
         REALTIJD_min[fc2] = hulp;
      }
      if (REALTIJD_max[fc2] < hulp) /* CCA30012021: ook voor realtijd_max */
      {
        REALTIJD_max[fc2] = hulp;
      }

       result   = TRUE;
    }
  }

  /* -------------------------------------------------------------------------- */
  /* Reseterende FOT bijwerken, van fc1 naar fc2                                */
  /* -------------------------------------------------------------------------- */
  TIME_FOT[fc1][fc2] = RT[fot1_2] ? T_max[fot1_2]                   :
                        T[fot1_2] ? T_max[fot1_2] - T_timer[fot1_2] : 0;

																																			  
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
void Synchroniseer_SP(bool  period)
{
  register count fc1, fc2;

  for (fc1=0; fc1<FC_MAX; fc1++)
  {
    for (fc2=0; fc2<FC_MAX; fc2++)
    {
      if (period && REAL_SYN[fc1][fc2] && ((TOTXA_PL[fc2]==0) && (TOTXB_PL[fc2]>0) && (REALTIJD[fc2] > (TOTXB_PL[fc2] + 10)) || (TX_timer==TXB_PL[fc2]) && (REALTIJD[fc2] >= 10)))
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
        RR[fc2] |= (REALTIJD[fc2] > 1) && ((GL[fc1] || RV[fc1]) && kcv_primair(fc1) || REALTIJD[fc1]==9999) ? BIT1 : 0;
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
        RR[fc2] |= (REALTIJD[fc2] > 1) && ((GL[fc1] || RV[fc1]) && kcv_primair(fc1) || REALTIJD[fc1]==9999) ? BIT1 : 0;
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
/* - Vasthouden groen fc2 als fc1 bijna aan de beurt is. Dit mag alleen mits:                                                                                                                                 */
/*   - synchronisatiewens       van fc2 naar fc1                                                                                                                                                              */
/*   - fictieve ontruimingswens van fc1 naar fc2                                                                                                                                                              */
/*   - fictieve ontruimingswens van fc2 naar fc1 bestaat niet. Fc1 moet immers wel op groen kunnen komen, bijv:                                                                                               */
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
      if (TIME_FOT[fc1][fc2] && R[fc2])
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

      if (REAL_SYN[fc2][fc1] &&
         !REAL_FOT[fc2][fc1] &&
         REAL_FOT[fc1][fc2] && G[fc2] && !G[fc1] && A[fc1] && (RA[fc1] || AAPR[fc1] && !PG[fc1]) && !RR[fc1] && !BL[fc1] && !kaa(fc1) && !kaa(fc2)
#if MLMAX
         && PRML[ML][fc1] & PRIMAIR_VERSNELD && PRML[ML][fc2] & PRIMAIR_VERSNELD
#endif
#if MLAMAX
         && PRMLA[MLA][fc1] & PRIMAIR_VERSNELD && PRMLA[MLA][fc2] & PRIMAIR_VERSNELD
#endif
#if MLBMAX
         && PRMLB[MLB][fc1] & PRIMAIR_VERSNELD && PRMLB[MLB][fc2] & PRIMAIR_VERSNELD
#endif
         && (REALTIJD[fc1] <= (TGL_max[fc2] + TRG_max[fc2])))
      {
         RW[fc2] |= BIT1;
      }
    }
  }
}
/* ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */
void Synchroniseer_FO1_2(count fc1, count fc2)   /* Gelijk aan bovenstaande alleen niet algemeen, maar specifiek per fictieve ontrumingstijd                                                                   */
{
      if (TIME_FOT[fc1][fc2] && R[fc2])
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

      if (REAL_SYN[fc2][fc1] &&
         !REAL_FOT[fc2][fc1] &&
          REAL_FOT[fc1][fc2] && G[fc2] && !G[fc1] && A[fc1] && (RA[fc1] || AAPR[fc1] && !PG[fc1]) && !RR[fc1] && !BL[fc1] && !kaa(fc1) && (REALTIJD[fc1] <= (TGL_max[fc2]+TRG_max[fc2])))
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
/*                                                                                                                                                                                                            */
/* Als de #define MLNLTEST opstaat (bv opgezet in de sys.add) dan wordt alleen de PG gezet als de fc1 niet in meerdere blokken staat.                                                                         */
/* Dit voorkomt het onterecht PG zetten bijvoorbeeld bij interne koppleingen van twee voedende richtingen met een naloop of synchronisaties als de richtingen niet in hetzelfde blok staat.                   */
/* ========================================================================================================================================================================================================== */

void Synchroniseer_PG(void)
{
#ifdef MLNLTEST
  register count  mlfc1, mlfc2, ml_1=0, ml_2=0;
#endif
  register count fc1, fc2;

  for (fc1=0; fc1<FC_MAX; fc1++)
  {
#ifdef MLNLTEST
      for (mlfc1 = 0; mlfc1 < ML_MAX; ++mlfc1) {
         if (PRML[mlfc1][fc1] & PRIMAIR) {  /* zoek de eerste plek in de MLmolen waar fc1 primair komt */
            ml_1 = mlfc1;
            break;
         }
      }
#endif
      for (fc2 = 0; fc2 < FC_MAX; fc2++)
      {
#ifdef MLNLTEST
         for (mlfc2 = 0; mlfc2 < ML_MAX; ++mlfc2) {
            if (PRML[mlfc2][fc2] & PRIMAIR) {  /* zoek de laatste plek in de MLmolen waar fc2 primair komt */
               ml_2 = mlfc2;
            }
         }
#endif
         if (!A[fc2] && R[fc2] && !PG[fc2] && PG[fc1]
#ifdef MLNLTEST
            && (ml_1 > 0) && (ml_2 > 0) && !(ml_1 < ml_2) /* alleen wanneer fc2 niet ook nog in een later blok zit (dan fc1) */
#endif  
            )
         {
          PG[fc2] |= REAL_SYN[fc1][fc2] && R[fc1] ? PG[fc1]          : 0;

          PG[fc2] |= TIME_FOT[fc1][fc2] && G[fc1] ? PRIMAIR_OVERSLAG : 0;
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
      if(!A[fc2] && R[fc2] && !PG[fc2] && PG[fc1])
      {
          PG[fc2] |= REAL_SYN[fc1][fc2] && R[fc1] ? PG[fc1]          : 0;

          PG[fc2] |= TIME_FOT[fc1][fc2] && G[fc1] ? PRIMAIR_OVERSLAG : 0;
      }

      /* voorkomen dat slechts één van beide verscneld primair komt, terwijl synchronsatie gewenst */
      if(REAL_SYN[fc1][fc2] && !(PFPR[fc1] && PFPR[fc2] && A[fc1] && A[fc2]))
      {
        PFPR[fc1] = FALSE;
        PFPR[fc2] = FALSE;
      }
}
/* ========================================================================================================================================================================================================== */

bool Maatgevend_Groen(count fc)   /* fasecyclus                                      */
{
  register count n, k;

  bool result=0;

  /* bepaal of G[fc] maatgevend is voor een (groen) conflict */
  /* ------------------------------------------------------- */
  if(G[fc])
  {
    for (n=0; n<GKFC_MAX[fc]; n++)
    {
#if (CCOL_V >= 95)
      k = KF_pointer[fc][n];
#else
      k = TO_pointer[fc][n];
#endif

      if (A[k] && (AAPR[k] /* && (AAPR[k] < BIT4)*/ || PAR[k]) || RA[k] || AA[k]) /* AAPR & BIT4 = RR, AAPR & BIT5 = PFPR nog niet waar, zie extra_func.c */
      /* AAPR & BIT6 = Priomelding via set_PRIRLW , zie ccolfunc.c */
      {
        /* bepaal of G[fc] maatgevend is voor een groen-conflict */
        /* ----------------------------------------------------- */
#if (CCOL_V >= 95) && !defined NO_TIGMAX
        if(TIG_max[fc][k]==GK || TIG_max[fc][k]==GKL)
#else
        if(TO_max[fc][k]==GK || TO_max[fc][k]==GKL)
#endif
        {
          if(      MG[fc] && ((                                                                                      1) >= REALTIJD[k]))  result = TRUE;
          else if(!MG[fc] && ((TFG_max[fc] - TFG_timer[fc] + TVG_max[fc] - TVG_timer[fc]                              ) >= REALTIJD[k]))  result = TRUE;
        }
        /* bepaal of G[fc] maatgevend is voor een       conflict */
        /* ----------------------------------------------------- */
#if (CCOL_V >= 95) && !defined NO_TIGMAX
        else if(TIG_max[fc][k]>=0)
        {
          if(      MG[fc] && ((                                                            TIG_max[fc][k]) >= REALTIJD[k]))  result = TRUE;
          else if(!MG[fc] && ((TFG_max[fc] - TFG_timer[fc] + TVG_max[fc] - TVG_timer[fc] + TIG_max[fc][k]) >= REALTIJD[k]))  result = TRUE;
        }
#else
        else if(TO_max[fc][k]>=0)
        {
          if(      MG[fc] && ((                                                            TGL_max[fc] + TO_max[fc][k]) >= REALTIJD[k]))  result = TRUE;
          else if(!MG[fc] && ((TFG_max[fc] - TFG_timer[fc] + TVG_max[fc] - TVG_timer[fc] + TGL_max[fc] + TO_max[fc][k]) >= REALTIJD[k]))  result = TRUE;
        }
#endif

      }
    }
  }

  return result;
}

void Inlopen_Los3(count fc1,        /* fc1                                                                       */
                  count fc9,        /* fc9, middelste fc                                                         */
                  count fc2,        /* fc2                                                                       */

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

                  bool  sch1_1,     /* fc1   los, bij DK1_bi en DK1_bu                                           */
                  bool  sch1_2,     /* fc1   los, bij DK1_bi en DK2_bu/DK9_b (A in rug tbv naloop)               */
                  bool  sch2_1,     /* fc2   los, bij DK2_bi en DK2_bu                                           */
                  bool  sch2_2,     /* fc2   los, bij DK2_bi en DK1_bu/DK9_a (A in rug tbv naloop)               */
                  bool  sch9_1a,    /* fc9   los, bij DK9_a                  (DK9_a single OK    )               */
                  bool  sch9_1b,    /* fc9   los, bij DK9_b                  (DK9_b single OK    )               */
                  bool  sch9_2a,    /* fc9   los, bij DK9_a  en DK2_bu       (tegenligger    fc32)               */
                  bool  sch9_2b,    /* fc9   los, bij DK9_b  en DK1_bu       (tegenligger    fc31)               */
                  bool  sch9_3a,    /* fc9   los, bij DK9_a  en DK1_bu       (A in rug tbv naloop)               */
                  bool  sch9_3b,    /* fc9   los, bij DK9_b  en DK2_bu       (A in rug tbv naloop)               */
                  bool  sch9_4a,    /* fc9-2 los, bij DK9_a  en DK1_bu       (A in rug tbv naloop)               */
                  bool  sch9_4b)    /* fc9-1 los, bij DK9_b  en DK2_bu       (A in rug tbv naloop)               */
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

  bool SITUATIE[19]={0};

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

                  count hdk1bu,     /* hulpelement drukknop fc1 buitenzijde                                      */
                  count hdk1bi,     /* hulpelement drukknop fc1 middenberm                                       */
                  count hdk2bi,     /* hulpelement drukknop fc2 middenberm                                       */
                  count hdk2bu,     /* hulpelement drukknop fc2 buitenzijde                                      */

                  count hinl1,      /* hulpelement fc1 inlopen gewenst                                           */
                  count hinl2,      /* hulpelement fc2 inlopen gewenst                                           */

                  count hlos1,      /* hulpelement fc1 los toegestaan                                            */
                  count hlos2,      /* hulpelement fc2 los teogestaan                                            */

                  bool  sch1_1,     /* fc1   los, bij DK1_bi en DK1_bu                                           */
                  bool  sch1_2,     /* fc1   los, bij DK1_bi en DK2_bu       (A in rug tbv naloop)               */
                  bool  sch2_1,     /* fc2   los, bij DK2_bi en DK2_bu                                           */
                  bool  sch2_2)     /* fc2   los, bij DK2_bi en DK1_bu       (A in rug tbv naloop)               */
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

  bool SITUATIE[10]={0};

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

bool VTG3_Real_Los(count fc1,        /* fc1                                                                       */
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

                   bool  gelijk1_2,  /* extra voorwaarde t.b.v. gelijkstart                                       */
                   bool  gelijk1_9,  /* extra voorwaarde t.b.v. gelijkstart                                       */
                   bool  gelijk2_9)  /* extra voorwaarde t.b.v. gelijkstart                                       */

{
  bool result = 0;

  /* ------------------------------------------------------------------------------------------------------------------------------------------------------------ */
  /* Bepalen hulpwaarde t.b.v. beide alleen mee-aanvraag, dan gelijkstart                                                                                         */
  /* ------------------------------------------------------------------------------------------------------------------------------------------------------------ */
  bool alleen_MA_1 = A[fc1] && !(A[fc1] & ~(BIT4|BIT8));
  bool alleen_MA_2 = A[fc2] && !(A[fc2] & ~(BIT4|BIT8));
  bool alleen_MA_9 = A[fc9] && !(A[fc9] & ~(BIT4|BIT8));

  gelijk1_2 |= alleen_MA_1 && alleen_MA_2               || !A[fc1] && !A[fc2];  /* Sowieso gelijk als beide alleen mee-aanvraag.    */
  gelijk1_9 |= alleen_MA_1 && alleen_MA_9 && !IH[hinl2] || !A[fc1] && !A[fc9];  /* Bij beide geen A ook uitgaan van gelijk,         */
  gelijk2_9 |= alleen_MA_2 && alleen_MA_9 && !IH[hinl1] || !A[fc2] && !A[fc9];  /* van belang tbv eventueel onderling doorzetten PG */

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
  else if(IH[hinl9_1] &&  IH[hlos1  ])  {  result |= Corr_Min(fc9, fc1,     0, FALSE);                   /* dkfc9_bu               , fc1 los van    fc9           */
                                           result |= Corr_Min(fc1, fc9, t9_t1,  TRUE);                   /*                        , fc9 inlopen op fc1           */
                                        }
  else if(IH[hinl2  ] && !IH[hlos1  ])  {  result |= Corr_Min(fc9, fc1,     0,  TRUE);                   /* dkfc2_bu               , fc1 wacht   op fc9           */
                                           result |= Corr_Min(fc1, fc9, t9_t1,  TRUE);                   /*                        , fc9 inlopen op fc1           */
                                        }
  else if(IH[hinl2  ] &&  IH[hlos1  ])  {  result |= Corr_Min(fc9, fc1,     0, FALSE);                   /* dkfc2_bu               , fc1 los van    fc9           */
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
  else if(IH[hinl9_2] &&  IH[hlos2  ])  {  result |= Corr_Min(fc9, fc2,     0, FALSE);                   /* dkfc9_bu               , fc2 los van    fc9           */
                                           result |= Corr_Min(fc2, fc9, t9_t2,  TRUE);                   /*                        , fc9 inlopen op fc2           */
                                        }
  else if(IH[hinl1  ] && !IH[hlos2  ])  {  result |= Corr_Min(fc9, fc2,     0,  TRUE);                   /* dkfc1_bu               , fc2 wacht   op fc9           */
                                           result |= Corr_Min(fc2, fc9, t9_t2,  TRUE);                   /*                        , fc9 inlopen op fc2           */
                                        }
  else if(IH[hinl1  ] &&  IH[hlos2  ])  {  result |= Corr_Min(fc9, fc2,     0, FALSE);                   /* dkfc1_bu               , fc2 los van    fc9           */
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

bool VTG2_Real_Los(count fc1,        /* fasecyclus 1                                       */
                   count fc2,        /* fasecyclus 2                                       */
                   mulv  t1_t2,      /* inlooptijd  fc1 op fc2                             */
                   mulv  t2_t1,      /* inlooptijd  fc2 op fc1                             */
                   count hinl1,      /* hulpelement fc1 inlopen gewenst                    */
                   count hinl2,      /* hulpelement fc2 inlopen gewenst                    */
                   count hlos1,      /* hulpelement fc1 los toegestaan                     */
                   count hlos2,      /* hulpelement fc2 los teogestaan                     */
                   bool gelijk)      /* extra voorwaarde t.b.v. gelijkstart                */
{
  bool result = 0;

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
  if((AAPR[fc1] && (AAPR[fc1]<BIT4)) || PR[fc1])           result=TRUE;    /* AAPR & BIT4 betekent RR, AAPR & BIT5 betekent PFPR nog niet waar */
  /* ---------------------------------------------------------------- */
  /* niet primair, bepaal of nalooptijd past bij naloop:              */
  /* - eerst moet fc1 op groen komen, dus check REALTIJD[fc1]         */
  /* - plus de nalooptijd moet passen bij fc2                         */
  /* ---------------------------------------------------------------- */
  else if(MM[marfc2] >= (REALTIJD[fc1] + T_max[tnlsg]))  result=TRUE;
  /* ---------------------------------------------------------------- */

  return result;
}

mulv Real_Ruimte(count fc,    /* fasecyclus                                                   */
                 count mar)   /* memory element t.b.v. inzichtelijk maken alternatieve ruimte */
{
  register count n, k;

  mulv ruimte     = 3000;  /* initieel veel ruimte voor groen */
  mulv hulpruimte = 3000;  /* hulpwaarde    ruimte voor groen */


  /* ------------------------------------------ */
  /* Als conflicterend CV dan is er geen ruimte */
  /* ------------------------------------------ */
  if(kcv(fc))
  {
    ruimte = 0;
  }
  else
  {
  /* --------------------------------------------------------------------------- */
  /* Anders bepaal (hulp)ruimte t.o.v. conflicten die primair aan de beurt zijn: */
  /* --------------------------------------------------------------------------- */
  /* - REALTIJD[k] : Wanneer kan conflict realiseren ?                           */
  /*                                                                             */
  /* - REALTIJD[fc]: Wanneer kan fc       realiseren ?                           */
  /* - Intergroen  : Wat is intergroen van fc naar k (of geel/ontruimen)         */
  /*                                                                             */
  /* Een positief verschil geeft ruimte aan voor fc.                             */
  /* --------------------------------------------------------------------------- */
    for (n=0; n<GKFC_MAX[fc]; n++)
    {
#if (CCOL_V >= 95)
      k = KF_pointer[fc][n];
#else
      k = TO_pointer[fc][n];
#endif

      if(AAPR[k])
      {
#if (CCOL_V >= 95) && !defined NO_TIGMAX
        hulpruimte = (REALTIJD[k] - REALTIJD[fc] -              TIG_max[fc][k]) > 0 ?
                     (REALTIJD[k] - REALTIJD[fc] -              TIG_max[fc][k]) : 0;
#else
        hulpruimte = (REALTIJD[k] - REALTIJD[fc] - TGL_max[fc] - TO_max[fc][k]) > 0 ?
                     (REALTIJD[k] - REALTIJD[fc] - TGL_max[fc] - TO_max[fc][k]) : 0;
#endif
      }

    /* -------------------------------------------------------------------------- */
    /* Kleinste ruimte is maatgevend                                              */
    /* -------------------------------------------------------------------------- */
      ruimte = (hulpruimte < ruimte) ? hulpruimte : ruimte;
    }
  }

  /* -------------------------------------------------------------------------- */
  /* Inzichtelijk maken ruimte + returnwaarde gevven                            */
  /* -------------------------------------------------------------------------- */
  MM[mar] = ruimte;

  return ruimte;

}

bool Real_Los(count fc1,        /* fasecyclus 1                                       */
               count fc2,        /* fasecyclus 2                                       */
               mulv  t2_t1,      /* inlooptijd  fc2 op fc1                             */
               count hlos2,      /* hulpelement fc2 los toegestaan                     */
               bool gelijk)      /* extra voorwaarde t.b.v. gelijkstart               */
{
   bool result = 0;
   
   hlos2 = A[fc2] && !(A[fc2] & ~(BIT4 | BIT8));    /* aanvraag dan mag hij los                        */
   gelijk |= A[fc1] && !(A[fc1] & ~(BIT4|BIT8)) &&  /* Sowieso gelijk als beide alleen mee-aanvraag.   */
             A[fc2] && !(A[fc2] & ~(BIT4|BIT8)) ||
      !A[fc1] && !A[fc2];                           /* Bij beide geen A ook uitgaan van gelijk,         */
                                                    /* van belang tbv eventueel onderling doorzetten PG */

   /* -------------------------------------------------------------------------------------------------------------------------------------------------------- */
   /* tbv gelijkstart/inlopen                                                                                                                                  */
   /* -------------------------------------------------------------------------------------------------------------------------------------------------------- */
   if (gelijk)     result |= Corr_Gel(fc1, fc2, TRUE);         /* beide alleen meeanvraag, gelijkstart */
   else if (hlos2) result |= Corr_Min(fc1, fc2, t2_t1,  TRUE); /* fc2 inlopen op fc1 */
   
   /* --------------------------------- */
   /* Wijzigingen aangeven via result   */
   /* --------------------------------- */
   return result;
}