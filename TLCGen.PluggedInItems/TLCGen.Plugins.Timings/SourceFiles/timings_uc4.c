/* signaalgroep stadium : */
/* 'Confidence' = Signaalgroep Stadium */
#define TIMING_CONFIDENCE_ONBEKEND              0     /* Geen informatie */
#define TIMING_CONFIDENCE_RD_GEEN_AANVRAAG      1     /* Richting is Rood, en heeft geen aanvraag */
#define TIMING_CONFIDENCE_RD_AANVRAAG           3     /* Richting is Rood, heeft een aanvraag , maar niet bekend wanneer het groen gaat worden */
#define TIMING_CONFIDENCE_RD_ANDEREN_EERST      6     /* Richting is Rood, ... , andere richtingen worden nog groen */
#define TIMING_CONFIDENCE_RD_VOLGENDE_BEURT     9     /* Richting is Rood, ... , als volgende aan de beurt */
#define TIMING_CONFIDENCERD_GROEN_KOMT          12    /* Richting is Rood, ... , wordt groen, maar (prio) ingrepen nog toegestaan */
#define TIMING_CONFIDENCE_RD_GROEN_DEFINITIEF   15    /* Richting is Rood, ... , wordt zeker groen */
#define TIMING_CONFIDENCE_RD_MAX                16    /* hoogste waarde stadium ( wordt gebruikt voor interne Array ) */

boolv NaarConfidence9(count i)
{
   /* De functie NaarConfidence9 wordt gebruik voor de overgang van de confidence stadium 6-->9. 
   * De functie bepaalt of alle primaire conflicten uit het voorgaande blokeen PG hebben.  
   */
   count hml, k, n;
   hml = ML + 1;
   if (hml == MLMAX) hml = 0;
   if (PRML[hml][i] == PRIMAIR)
   {
      for (n = 0; n < KFC_MAX[i]; ++n)
      {
         k = KF_pointer[i][n];
         if ((PRML[ML][k] == PRIMAIR) && !PG[k])
            return FALSE;
      }
      return TRUE;
   }
   else
   {
      return FALSE;
   }
}

boolv NaarConfidence9_15prio(count i)
{
   /* De functie NaarConfidence9_15prio wordt gebruik voor de overgang van de confidence stadium 6-->9 en van 12-->15. 
    * De functie bepaalt of alle conflicten worden tegengehouden met RR BIT6 && !P tijdens geel of rood en of de eigen richting YV BIT6 of YM BIT6 heeft.  
    */
   register count n, j;

#ifndef NO_GGCONFLICT
   for (n = 0; n < GKFC_MAX[i]; ++n) {
#else
   for (n = 0; n < KFC_MAX[i]; ++n) {
#endif
      j = KF_pointer[i][n];
      /* eigen richting (i) heeft prioriteitsingreep */
      if (!((YV[i]&BIT6) || (YM[i]&BIT6)))  return (FALSE);
      /* conflictrichting (j) heeft geen P */
      if (P[j] & BIT11)                     return (FALSE);
      /* conflictrichting (j) heeft RR&bit6 */
      if (!((R[j]||GL[j]) && RR[j]&BIT6))   return (FALSE);
   }
   return (TRUE);
}


static void timings_uc4(count fc, count mrealtijdmin, count mrealtijdmax, count prm_ttxconfidence15, count s_conf15ar, count s_timingsfc)
{
   int min = mrealtijdmin; /* eerst MM min met daarna FCMAX x MM */
   int max = mrealtijdmax; /* eerst MM max met daarna FCMAX x MM */
   int i = fc;

   if (!TE) return;

   /* old - onthouden vorige CCOL_FC_TIMING */
   /* ----------------------------------- */
   if (R[i])
   {
      CCOL_FC_TIMING_old[i][0][CIF_TIMING_CONFIDENCE] = CCOL_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE];
      CCOL_FC_TIMING_old[i][0][CIF_TIMING_MINENDTIME] = CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME];
      CCOL_FC_TIMING_old[i][0][CIF_TIMING_MAXENDTIME] = CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME];
   }

   /* new - nieuwe CCOL_FC_TIMING */
   /* ------------------------- */

   switch (CCOL_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE])
   {

   case TIMING_CONFIDENCE_RD_GEEN_AANVRAAG:  /* 1 */
      if ((R[i] && !SR[i]) &&
         /* voorwaarde 1 --> 3 */
         ((A[i] || (YV[i] & BIT6) || (YM[i] & BIT6))))
      {
         CCOL_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE] = TIMING_CONFIDENCE_RD_AANVRAAG; /* 3 */
      }
      /* override */
      if (!SCH[schspatconfidence1] || !SCH[s_timingsfc])
      {
         CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME] = NG;
         CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME] = NG;
         CCOL_FC_TIMING[i][0][CIF_TIMING_LIKELYTIME] = NG;
      }
      break;

   case TIMING_CONFIDENCE_RD_AANVRAAG: /* 3 */
      if /* voorwaarde 3 --> 6 */
         ((kcv(i) || kg(i) || RA[i] || AAPR[i]) &&
         /* voorwaarde 1 --> 3 */
         (A[i] || YV[i] & BIT6 || YM[i] & BIT6))
         {
            CCOL_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE] = TIMING_CONFIDENCE_RD_ANDEREN_EERST; /* 6 */
         }
         /* override */
         if (!SCH[schspatconfidence3] || !SCH[s_timingsfc])
         {
            CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME] = NG;
            CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME] = NG;
            CCOL_FC_TIMING[i][0][CIF_TIMING_LIKELYTIME] = NG;
         }
         break;

   case TIMING_CONFIDENCE_RD_ANDEREN_EERST: /* 6 */
      if ((((PR[i] || (SCH[s_conf15ar] && AR[i])) && (kg(i) || RA[i]) || NaarConfidence9(i)) &&
         /* voorwaarde 3 --> 6 */
         (kcv(i) || kg(i) || RA[i] || AAPR[i])) ||
         /* prio */
         NaarConfidence9_15prio(i))
      {
         CCOL_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE] = TIMING_CONFIDENCE_RD_VOLGENDE_BEURT; /* 9 */
      }
      /* override */
      if (!SCH[schspatconfidence6] || !SCH[s_timingsfc])
      {
         CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME] = NG;
         CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME] = NG;
         CCOL_FC_TIMING[i][0][CIF_TIMING_LIKELYTIME] = NG;
      }
      break;

   case TIMING_CONFIDENCE_RD_VOLGENDE_BEURT: /* 9 */
      if (((!kg(i) || RA[i])))
      {
         CCOL_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE] = TIMING_CONFIDENCERD_GROEN_KOMT; /* 12 */
      }
      /* override */
      if (!SCH[schspatconfidence9] || !SCH[s_timingsfc])
      {
         CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME] = NG;
         CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME] = NG;
         CCOL_FC_TIMING[i][0][CIF_TIMING_LIKELYTIME] = NG;
      }
      break;

   case TIMING_CONFIDENCERD_GROEN_KOMT: /* 12 */
      if (/* geen conflicterende (of FK) richtingen met privilege (P) */
         !kp(i) && 
         /* geen conflicterende groenfase (G) */
         !kg(i) &&
         /* voorwaarde 12 --> 15 */
         (((NaarConfidence9_15prio(i) || RA[i]) && ((MM[min] <= PRM[prmttxconfidence15]) && (MM[max] <= PRM[prmttxconfidence15])))) || P[i] & BIT11)
      {
         if (P[i] & BIT11) CCOL_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE] = TIMING_CONFIDENCE_RD_GROEN_DEFINITIEF; /* 15 */ /* 1 machineslag vertragen ivm acties door P bij gelijk- en voorstart */
         P[i] |= BIT11;
      }
      CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME] = MM[min];
      CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME] = MM[max];
      CCOL_FC_TIMING[i][0][CIF_TIMING_LIKELYTIME] = (((CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME]) + (CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME]))) / 2;
      /* override */
      if (!SCH[schspatconfidence12] && (CCOL_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE] == 12) || !SCH[s_timingsfc])
      {
         CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME] = NG;
         CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME] = NG;
         CCOL_FC_TIMING[i][0][CIF_TIMING_LIKELYTIME] = NG;
      }
      break;

   case TIMING_CONFIDENCE_RD_GROEN_DEFINITIEF: /* 15 */
      CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME] = MM[min];
      CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME] = MM[max];
      CCOL_FC_TIMING[i][0][CIF_TIMING_LIKELYTIME] = (((CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME]) + (CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME]))) / 2;
      /* override */
      if (!SCH[schspatconfidence15] || !SCH[s_timingsfc])
      {
         CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME] = NG;
         CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME] = NG;
         CCOL_FC_TIMING[i][0][CIF_TIMING_LIKELYTIME] = NG;
      }
      break;
   }

   /* stadia 15 --> 1 */
   if (!R[i])
   {
      P[i] &= ~BIT11;
      CCOL_FC_TIMING[i][0][CIF_TIMING_CONFIDENCE] = 1;
   }

/* CHECKS */
/* ------ */
#ifndef AUTOMAAT
   if (R[i] && (CIF_FC_RWT[i]==0) && (CCOL_FC_TIMING_old[i][0][CIF_TIMING_MAXENDTIME] != -1) && ((CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME]) > CCOL_FC_TIMING_old[i][0][CIF_TIMING_MAXENDTIME]))
   {
      code helpstr[30];  /* help string */
      CIF_FC_RWT[i] |= CIF_FC_RWT_ONBEKEND;
      uber_puts(PROMPT_code);
      uber_puts("maxend>max_endold:");
      uber_puts(FC_code[i]);
      uber_puts(" / ");
      datetostr(helpstr);
      uber_puts(helpstr);
      uber_puts(" / ");
      timetostr(helpstr);
      uber_puts(helpstr);
      uber_puts("\n");
   }
   if (R[i] && (CIF_FC_RWT[i]==0)                                                            && ((CCOL_FC_TIMING[i][0][CIF_TIMING_MAXENDTIME]) < CCOL_FC_TIMING[i][0][CIF_TIMING_MINENDTIME]))
   {
      code helpstr[30];  /* help string */
      CIF_FC_RWT[i] |= CIF_FC_RWT_ONBEKEND;
      uber_puts(PROMPT_code);
      uber_puts("maxend<minend:"); 
      uber_puts(FC_code[i]);
      uber_puts(" / ");
      datetostr(helpstr);
      uber_puts(helpstr);
      uber_puts(" / ");
      timetostr(helpstr);
      uber_puts(helpstr);
      uber_puts("\n");
   }
#endif
}

