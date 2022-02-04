/* DETECTIE OPTIES VOOR REGELPROGRAMMA */
/* ----------------------------------- */


/**************************************************************************
 *  Functie  : VervangendHiaatKoplus
 *
 *  Functionele omschrijving :
 *    Set BIT5 in MK bij detectiestoring langelus en nieuwe hiaattijd koplus
 *
 *  Functie aanroep: VervangendHiaatKoplus( fcnr, koplus, langelus, hitijd);
 *  Waarin: fcnr     - fasecylus
 *          koplus   - elementnr van koplus
 *          langelus - elementnr van langelus
 *          hitijd   - elementnr van timer voor vervangend hiaat
 *
 *  Voorbeeld:       VervangendHiaatKoplus( fc102, d1021, d1022, th1021dv);
 **************************************************************************/
void VervangendHiaatKoplus( count fcnr, count koplus, count langelus, count hitijd)
{
    /* reset timer voor detectievervangende hiaatmeting */
    RT[hitijd]= D[koplus];
    /* hiaattijd op koplus bij defect langelus          */
    if((CIF_IS[langelus] >= CIF_DET_STORING) &&
       (CIF_IS[koplus] < CIF_DET_STORING) && (T[hitijd] || RT[hitijd]))
    {  MK[fcnr]|= BIT5;
    }
}

/*****************************************************************************
 *  Functie : PercentageMaxGroenTijden
 *
 *  Functionele omschrijving :
 *    Bepaalt de TVG_max[fc] aan de hand van de actieve periode (MM[period])
 *    en de voor deze periode ingestelde maxgroentijd parameter. Het meegegeven
 *    percentage bepaalt de uiteindelijke TVG_max.
 *    Deze functie kan worden aangeroepen voor file of voor detectiefouten.
 *  Functie aanroep: PercentageMaxGroenTijden(fc, periode, perc, n, ...);
 ****************************************************************************/
void PercentageMaxGroenTijden(count fc, count periode, mulv percentage, count n, ...)
{
    va_list argp;
    mulv  i;
    mulv mgnr = 300;

    va_start(argp, n);

    /* eerst overbodige parameters inlezen */
    /* ----------------------------------- */
    for (i = 0; i <= MM[periode] && i < n; i++)
        mgnr = va_arg(argp, va_mulv);

    /* nu staat in mgnr de parameter index waarin je geinteresseerd bent */
    /* ----------------------------------------------------------------- */
    TVG_max[fc] = ((mulv)(((long)percentage * (long)mgnr)/100) > TFG_max[fc])
                ?  (mulv)(((long)percentage * (long)mgnr)/100) - TFG_max[fc]
                : 0;

    va_end(argp);
}

/*****************************************************************************
 *  Functie : PercentageVerlenGroenTijden
 *
 *  Functionele omschrijving :
 *    Bepaalt de TVG_max[fc] aan de hand van de actieve periode (MM[period])
 *    en de voor deze periode ingestelde verlenggroentijd parameter. Het meegegeven
 *    percentage bepaalt de uiteindelijke TVG_max.
 *    Deze functie kan worden aangeroepen voor file of voor detectiefouten.
 *  Functie aanroep: PercentageVerlengGroenTijden(fc, periode, perc, n, ...);
 ****************************************************************************/
void PercentageVerlengGroenTijden(count fc, count periode, mulv percentage, count n, ...)
{
    va_list argp;
    mulv  i;
    mulv mgnr = 300;

    va_start(argp, n);

    /* eerst overbodige parameters inlezen */
    /* ----------------------------------- */
    for (i = 0; i <= MM[periode] && i < n; i++)
        mgnr = va_arg(argp, va_mulv);

    /* nu staat in mgnr de parameter index waarin je geinteresseerd bent */
    /* ----------------------------------------------------------------- */
    TVG_max[fc] = (mulv)(((long)percentage * (long)mgnr)/100);
    va_end(argp);
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
void FileMelding(count det,     /* filelus                                */
                 count tbez,    /* bezettijd  als D langer bezet -> file  */
                 count trij,    /* rijtijd    als D korter bezet -> !file */
                 count tafval,  /* afvalvertraging filemelding            */
                 count hfile,   /* hulpelement filemelding                */
                 count hafval)  /* hulpelement afvalvertraging            */
{
    RT[tbez]   = RT[trij] = SD[det];
    IH[hfile] |= D[det] && !T[tbez] && !RT[tbez];

    IT[tafval] = ED[det] && (T[trij] || RT[trij]) && IH[hfile] && !IH[hafval];
    AT[tafval] = D[det] && !T[trij] && !RT[trij];

    if (ET[tafval])
    {  if (!D[det])
       {  IH[hfile] = FALSE; }
       else if (T[trij] || RT[trij])
       {  IH[hafval] = TRUE; }
    }
    if (D[det] && !(T[trij] || RT[trij])) IH[hafval] = FALSE;
    if (ED[det] && IH[hafval])            IH[hfile] = IH[hafval] = FALSE;

    if (CIF_IS[det] >= CIF_DET_STORING)   IH[hfile] = FALSE;
}
/****************************************************************************
 *  Functie  : RichtingVerlengen  vanaf 2.61 Nieuwe functie
 *
 *  Functionele omschrijving :
 *    retourneert true indien richtinggevoelige verlenging van d1 naar d2
 *    anders false
 *
 *  Functie aanroep: MK[fc] |= RichtingVerlengen(fc, d1, d2, trgr, trgh, hrgh);
 ****************************************************************************/
bool RichtingVerlengen(count fc,   /* de richting die verlengd wordt                       */
                       count d1,   /* eerste aangereden detector (van)                     */
                       count d2,   /* tweede aangereden detector (naar)                    */
                       count trgr, /* timer richtingsgevoelige rijtijd                     */
                       count trgh, /* timer richtingsgevoelige hiaattijd voor beide lussen */
                       count hrgh) /* hulpelement voor bepalen richtinggevoeligheid        */

{
    RT[trgr] = D[d1];

    if (T[trgr] && SD[d2])  IH[hrgh] = TRUE;
    if (!D[d2])             IH[hrgh] = FALSE;

    RT[trgh] = IH[hrgh] &&
              (CIF_IS[d2] < CIF_DET_STORING) &&
              (CIF_IS[d1] < CIF_DET_STORING) ||
               D[d2] && (CIF_IS[d1] >= CIF_DET_STORING) ||
               D[d1] && (CIF_IS[d2] >= CIF_DET_STORING);
  /* -------------------------------------------------------------------- */
  /* voorkom nutteloos verlengen op basis van de richtingsgevoelige lussn */
  /* als de rijtijd niet meer past binnen de resterende verlenggroentijd. */
  /*                                                                      */
  /* stop met het resetten van de hiaattijd als                           */
  /* - als richting fc in verlenggroen zit ?n                             */
  /* - de resterende verlenggroentijd is korter dan de 'rijtijd',         */
  /* -------------------------------------------------------------------- */
  if (VG[fc] && ((TVG_max[fc] - TVG_timer[fc]) < T_max[trgh]))
  {
    RT[trgh] = FALSE;  /* stop met resetten van de hiaattijd */
  }
  return T[trgh];
}
/*****************************************************************************
 *  Functie  : MeetKriteriumRGprm
 *
 *  Functionele omschrijving :
 *    meetkriterium met variabel aantal argumenten en instelbaar verleng-
 *    kriterium per richtinggevoelig element.
 *
 *  Functie aanroep: MeetKriteriumRGprm(fcnr, tkopmaxnr, ...)
 *
 *    waarin:  fcnr      - elementnummer fasecyclus
 *             tkopmaxnr - elementnummer tijd kopmaximum
 *             irg       - return waarde richtinggevoelig
 *             prm       - waarde verlengparameter
 *             ...       - lijst met richtinggev. functies afgesloten door END
 *
 *  Voorbeeld:
 *    MeetKriteriumRGprm( (count) fc02, (count) tkm02, (va_mulv) RGFunc, (va_mulv) PRM[prmmkrgd021])
 *
 * instelling waarde van de verlengparameter:
 *   0      - geen verlengkriterium
 *   1      - beperkt verlengkriterium (met kopmaximum, indien kopmaximum is gedefinieerd)
 *   2      - 1e verlengkriterium      (geen kopmaximum)
 *   3      - 2e verlengkriterium
 *  overig  - 2e verlengkriterium
 *
 * Deze functie is voor een deel overgenomen uit stdfunc.c, meetkriterium_prm_va_arg()
 *********************************************************************************************/
void MeetKriteriumRGprm(count fc, count tkopmaxnr, ...)
{
    va_list argpt;         /* variabele argumentenlijst             */
    bool    iRgFunc;       /* bool uit richtinggevoelige functie    */
    mulv    prm;           /* waarde verlengparameter               */
    count   hmk4_6, hmk7;  /* hulpwaarden meetkriterium             */

    va_start(argpt,tkopmaxnr);        /* start var. parameterlijst  */

    if (G[fc])                        /* alleen tijdens groen       */
    {  if (tkopmaxnr>=0)              /* kopmaximum gebruikt?       */
       RT[tkopmaxnr]= SFG[fc];        /* herstart tijd kopmaximum   */
       if (TFG[fc])                   /* test lopen vastgroentijd   */
       {  MK[fc] |= (BIT4|BIT6);      /* zet bit 4 en 6             */
       }
       else
       {  hmk4_6 = 0xffff;                       /* alle bits in masker zetten */
          hmk4_6 &= ~BIT4;                       /* wis bits 4 en 6 in masker  */
          hmk4_6 &= ~BIT6;                       /* alle bits waar,behalve 4+6 */
          hmk7   = 0;                            /* hulpwaarden worden 0       */
          do
          {  iRgFunc= (bool) va_arg(argpt, va_mulv); /* lees return uit functie    */
             if (iRgFunc>=0)
             {  prm= (mulv) va_arg(argpt, va_mulv);  /* lees waarde parameter      */
                 if (prm>END)
                {  if (prm && iRgFunc)               /* test waarde en hiaattijd   */
                   {  if (prm==1)
                      {  hmk4_6 |= BIT4;             /* set verlengbit 4           */
                      }
                      else
                         if (prm==2)
                         {  hmk4_6 |= BIT6;          /* set verlengbit 6           */
                         }
                         else
                         {  hmk7 |= BIT7;            /* set verlengbit 7           */
                         }
                   }
                }
             }
          }  while (iRgFunc>=0 && prm>END);  /* laatste parameter?         */
          if (tkopmaxnr>=0)                  /* kopmaximum gebruikt?       */
          {  if (!T[tkopmaxnr])              /* test lopen tijd kopmaximum */
                hmk4_6 &= ~BIT4;             /* niet lopen -> reset bit4   */
          }

          /* wis bits 4 en 6 zonder andere bits aan te raken */

          MK[fc] &= hmk4_6;                  /* reset verleng bit 4 en 6   */

          /* wis verleng bit 7, en zet deze weer op */

          MK[fc] &= ~BIT7;                   /* reset verleng bit 7        */
          MK[fc] |= hmk7;                    /* set verleng bit 7          */
       }
    }
    va_end(argpt);                           /* maak var. arg-lijst leeg   */
}

