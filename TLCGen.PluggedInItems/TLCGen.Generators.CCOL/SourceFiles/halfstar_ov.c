/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* CCOL - OV Prioriteit signaalplanstructuur                                              */
/*                                                                                        */
/*                                                                                        */
/* (C) Copyright 2015 RHDHV b.v. All rights reserved.                                     */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* Versie :  1.2                                                                          */
/* Naam   :  rhdhv_ov_ple.c                                                               */
/* Datum  :  12-10-2010                                                                   */
/*           07-12-2015 kla: punctualiteit toegevoegd                                     */
/*           28-01-2016 kla: algemene OV functies verplaatst naar rhdhv_ov.c              */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */

#include "halfstar_ov.h"     /* declaratie functies                                      */

#if !defined AUTOMAAT || defined VISSIM
   #include "xyprintf.h"/* voor debug infowindow                                          */
   #include <stdio.h>      /* declaration printf()       */
#endif
/* -------------------------------------------------------------------------------------- */

/* -------------------------------------------------------------------------------------- */
/* definitie globale variabelen                                                           */
/* -------------------------------------------------------------------------------------- */
bool HoofdRichting[FCMAX];             /* Array met hoofdrichtingen                       */
bool HoofdRichtingTegenhouden[FCMAX];  /* Tegenhouden hoofdrichting (TXC of minimum groen)*/
bool HoofdRichtingAfkappenYWPL[FCMAX]; /* Afkappen YW_PL hoofdrichting (na minimum groen) */
bool HoofdRichtingAfkappenYVPL[FCMAX]; /* Afkappen YV_PL hoofdrichting (na minimum groen) */
bool ConflictAfgekapt[FCMAX][FCMAX];   /* Conflictrichting afgekapt tgv OV prioriteit     */
int  AlternatiefSignaalplan[PLMAX];    /* Alternatief PL bij OV prio                      */

mulv tfbact[FCMAX];
mulv gr_min[FCMAX];
mulv gr_min_timer[FCMAX];
bool gr_min_einde[FCMAX];

extern mulv test_pr_fk_totxb(count i, bool fpr);  /* standaard CCOL functie uit plefunc.c */

#define max(a,b)    (((a) > (b)) ? (a) : (b)) /* vergelijk twee waarden -> return maximum */
#define min(a,b)    (((a) < (b)) ? (a) : (b)) /* vergelijk twee waarden -> return minimum */
#define ORIGINEEL_PLAN     0
#define ALTERNATIEF_PLAN   1
/* -------------------------------------------------------------------------------------- */
/* Gereserveerde waarde om in de prioriteitsparameter aan te geven of de fasenvolgorde    */
/* gewisseld mag worden ten behoeve van OV ingrepen                                       */
/* -------------------------------------------------------------------------------------- */
#define WISSEL_FASEVOLGORDE 1000


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
#ifndef AUTOMAAT
   char str[20];
#endif
   mulv to_max, to_tmp;

   if (TRG[i] && !PP[i] && !PG[i] && ((TOTXB_PL[i]>0) || (TX_PL_timer==TXB_PL[i]))) {
     if (TRG_max[i] - TRG_timer[i] > TOTXB_PL[i]) {
       PG[i]= TRUE;
       return (TRUE);
     }
   }

   if (RV[i] && (BL[i] || !A[i]) && !PP[i] && !PG[i])
   {

      to_max=to_tmp= 0;

#ifdef NO_TIG
      for (n=0; n<KFC_MAX[i]; n++)
      {
         k= TO_pointer[i][n];
         if (TO[k][i])                                   /* zoek grootste ontruimingstijd */
         {
            to_tmp= TGL_max[k]+TO_max[k][i]-TGL_timer[k]-TO_timer[k];
            if (to_tmp>to_max)
               to_max= to_tmp;
         }
      }
#else
      for (n=0; n<FKFC_MAX[i]; n++)
      {
         k= TO_pointer[i][n];
         if (TIG_max[k][i]>=0)
         {
            to_tmp= TIG_max[k][i]-TIG_timer[k];
            if (to_tmp>to_max)                           /* zoek grootste ontruimingstijd */
               to_max= to_tmp;
         }
      }
#endif

      if ((RR[i]&OV_PLE_BIT) || (BL[i]&OV_PLE_BIT)) /* uitstel tot "kort" voor TXD moment */
      {
         if ((TOTXB_PL[i]==0) && (TOTXD_PL[i]>0)  && ((TOTXD_PL[i]-to_max)<0))
         {
            if (!PG[i])
            {
               PG[i]= TRUE;     /* onvoldoende tijd tot TXD-moment of TXD-moment bereikt? */
#ifndef AUTOMAAT
               if (A[i])
               {
                  timetostr(str);
                  uber_puts("**** At ");
                  uber_puts(str);
                  uber_puts(" TX=");
                  uber_puti((mulv) (TX_PL_timer));
                  uber_puts(" PG[fc");
                  uber_puts(FC_code[i]);
                  uber_puts("] was set before TXD (");
                  uber_puti((mulv) (TXD_PL[i]));
                  uber_puts(") passes ***\n");
               }
#endif
            }
            return (TRUE);
         }
      }
      else if (((TOTXB_PL[i]>0)  || (TX_PL_timer==TXB_PL[i])) && ((TOTXB_PL[i]-to_max)<0) ||
               (TOTXB_PL[i]==0) && (TX_PL_timer!=TXB_PL[i])  && (TX_PL_timer!=TXD_PL[i]))
      {
         PG[i]= TRUE;        /* onvoldoende tijd tot TXB-moment of TXB-moment gepasseerd? */
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

   for (i=0; i<FC_MAX; i++)
   {
      set_pg_primair_ov_ple(i);
   }
}

void set_pg_fk_totxb_ov_ple(count i)
{
   register count n, k;

   /* zet PG voor overslaan fictief confl. primaire fasecycli */
   /* ------------------------------------------------------- */
   for (n=0; n<FKFC_MAX[i]; n++)
   {
      k= TO_pointer[i][n];
      if ((((TOTXB_PL[k]>0) || (TX_PL_timer==TXB_PL[k])) && (TOTXB_PL[i]>TOTXB_PL[k])))
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
   mulv result= 0;

   /* reset PR and (re)set PG */
   /* ---------------------- */
#ifdef NO_CHNG20020705
   if (RV[i] && !AA[i] || SRV[i])  PR[i]= FALSE;
#endif
/* if (PR[i] && (SG[i] || (TX_PL_timer==TXB_PL[i]) && G[i]))  PG[i]= TRUE; */
   if (PR[i] && !PG[i])
   {
      if (SG[i] && (PR[i] & VERSNELD_PRIMAIR))  PG[i] = VERSNELD_PRIMAIR;
      else if (SG[i] || G[i] && ((TX_PL_timer==TXB_PL[i]) || (TOTXB_PL[i]==0) && (TOTXD_PL[i]>TFG_max[i])))
         PG[i]= PRIMAIR;
   }

   /* overname van AR naar PR */
   /* ----------------------- */
/* if (!PR[i] && ((TX_PL_timer==TXB_PL[i]) && (RA[i] || G[i])))  {  */
   if (!PR[i] && !PG[i] && G[i] && ((TX_PL_timer==TXB_PL[i]) || (TOTXB_PL[i]==0) && (TOTXD_PL[i]>TFG_max[i])))
   {
      PR[i]= PRIMAIR;        /* AR[] -> PR[] */
      AR[i]= FALSE;
/*    PG[i]= TRUE; - eerst PR[] opzetten en volgende systeemronde pas PG[] */
             /* zorgt ervoor dat RW opstaat voordat PG waar is */
   }
   if (TX_PL_timer==TXD_PL[i])  PG[i]= FALSE;

   /* (versneld) primaire realisatie */
   /* ------------------------------ */
   if (A[i] && RV[i] && !SRV[i] && !AA[i] && !RR_PL[i] && (!RR[i] || (RR[i]&OV_PLE_BIT)) && (!BL[i] || (BL[i]&OV_PLE_BIT))
       && !PG[i] && !fkaa(i) && ((TOTXB_PL[i]>0) && !kcv(i)   /* @@ */
      || (TXB_PL[i]==TX_PL_timer)
      || ((TOTXB_PL[i] == 0 && TOTXD_PL[i]>TFG_max[i] && (RR[i]&OV_PLE_BIT)))))  /* DHV CvB toegevoegd ivm uitstellen conflicten door OV */
                     /* 12-9-1998 ivm TXA==TXB @@ */
                      /* geen test op !TRG[i] @@ */
                      /* 1-1-1999 toegevoegd !SRV[i] */
   {
      switch (result= test_pr_fk_totxb(i, fpr))  /* test fictieve conflicten */
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
      if (result)  return ((bool) TRUE);
   }
   return ((bool) FALSE);
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

   for (i=0; i<FC_MAX; i++)
   {
      set_PRPL_ov_ple(i, (bool) TRUE);  /* TRUE -> versneld primair */
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
void OV_ple_init(void)
{
   int i, j;

   for (i=0; i<FC_MAX; i++)
   {
      tfbact[i] = 0;
      gr_min[i] = TFG_max[i];
      gr_min_einde[i] = FALSE;

      for (j=0; j<FC_MAX; j++)
      {
         ConflictAfgekapt[i][j] = FALSE;
      }

      HoofdRichting[i] = FALSE;
      HoofdRichtingTegenhouden[i] = FALSE;
      HoofdRichtingAfkappenYWPL[i] = FALSE;
      HoofdRichtingAfkappenYVPL[i] = FALSE;
   }

   for (i=0; i<PL_MAX; i++)
   {
      AlternatiefSignaalplan[i] = NG;
   }

   OV_ple_settings();
}

/* -------------------------------------------------------------------------------------- */
/* Functie: AlternatievePlannen()                                                         */
/*                                                                                        */
/* Doel:    Instellingen voor alternatieve signaalplannen bepalen                         */
/*                                                                                        */
/* Params:  Lijst met variabel aantal argumenten:                                         */
/*          1. dummy element (nodig voor va_list)                                         */
/*          2. actuele signaalplan                                                        */
/*          3. instelling alternatief signaalplan met gewijzigde fasevolgorde             */
/*                                                                                        */
/*          NB: va_list altijd afsluiten met END                                          */
/*                                                                                        */
/* AlternatievePlannen(NG, (va_mulv) PL1, (va_mulv) PL11,                                 */
/*                         (va_mulv) PL2, (va_mulv) PL12,                                 */
/*                         (va_count) END);                                               */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
void AlternatievePlannen(int dummy, ...)
{
   va_list argpt;                                     /*  variabele argumentenlijst       */
   mulv plact;                                        /*  actuele signaalplan             */
   mulv plalt;                                        /*  alternatieve signaalplan        */
   int i;

   for (i=0; i<PL_MAX; i++)
   {
      AlternatiefSignaalplan[i] = NG;
   }

   va_start(argpt,dummy);                             /*  start var. argumentenlijst      */
   do
   {
      plact= va_arg(argpt, va_count);                 /*  lees actuele signaalplan        */

      if ((plact>=0) && (plact<PL_MAX))
      {
         plalt= va_arg(argpt, va_mulv);               /* lees alternatieve signaalplan    */

         if ((plalt>0) && (plalt<=PL_MAX))
         {
            AlternatiefSignaalplan[plact] = plalt-1;  /* -1 vanwege #defines PL vanaf 0   */
         }
      }
   }
   while ((plact>=0) && (plact<PL_MAX) && (plact>END) &&
          (plalt>0) && (plalt<PL_MAX) && (plalt>END));

   va_end(argpt);                                     /* maak var. arg-lijst leeg         */
}

/* -------------------------------------------------------------------------------------- */
/* Functie: minimum_groentijden_ovprio_va_arg()                                           */
/*                                                                                        */
/* Doel:    Instellen van de minimum groentijden die gelden bij het toewijzen van         */
/*          OV prioriteit. Deze groentijden worden gehanteerd bij het afkappen van de     */
/*          groenfase en bij het uitstellen van het startgroen moment. Indien geen enkele */
/*          klokperiode waar is, geldt de basis minimum groentijd (als laatste opgegeven, */
/*          zonder klokperiode).                                                          */
/*                                                                                        */
/* Params:  fc:         elementnummer fasecyclus                                          */
/*          prmming     waarde parameter minimumgroentijd klokperiode hklokper            */
/*          hklok       waarde hulpelement klokperiode                                    */
/*                                                                                        */
/* voorbeeld:                                                                             */
/* minimum_groentijden_ovprio_va_arg((count)   fc02,                                      */
/*                                   (va_mulv) PRM[prmming02ovpl1], (va_mulv) (PL==PL1),  */
/*                                   (va_mulv) PRM[prmming02ovpl2], (va_mulv) (PL==PL2),  */
/*                                   (va_mulv) PRM[prmming02ovpl3], (va_mulv) (PL==PL3),  */
/*                                   (va_mulv) PRM[prmming02ovpl4], (va_mulv) (PL==PL4),  */
/*                                   (va_mulv) PRM[prmming02ovpl4], (va_mulv) (PL==PL5),  */
/*                                   (va_mulv) TFG_max[fc02],       (va_mulv) END);       */
/* -------------------------------------------------------------------------------------- */
void minimum_groentijden_ovprio_va_arg(count fc, ...)
{
   va_list argpt;                                           /* variabele argumentenlijst  */
   mulv prmming;                                            /* minimum groentijd bij OV   */
   mulv hklok;                                              /* klokgebied                 */

   va_start(argpt,fc);                                      /* start var. argumentenlijst */
   do {
      prmming= va_arg(argpt, va_mulv);                      /* lees min. groentijd        */
      hklok  = va_arg(argpt, va_mulv);                      /* lees klokperiode           */
   } while (hklok==0);
   va_end(argpt);                                           /* maak var. arg-lijst leeg   */

   gr_min[fc]= prmming;
}

/* -------------------------------------------------------------------------------------- */
/* Functie: PasSignaalplanToe()                                                           */
/*                                                                                        */
/* Doel:    TX momenten van alternatief signaalplannen toepassen                          */
/*                                                                                        */
/* Params:  alt:        0 = Originele signaalplan, 1 = Alternatieve signaalplan           */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
bool PasSignaalplanToe(bool alt)
{
   register count fc;
   int iMultiply;
   bool l_fReturn = TRUE;

 #ifdef TX_PL_TE
   iMultiply = 10;
 #else
   iMultiply = 1;
 #endif

   /* Check of cyclustijden van originele en alternatieve plan overeenkomen               */
   if (AlternatiefSignaalplan[PL] && (TX_PL_max != TX_max[AlternatiefSignaalplan[PL]]*iMultiply))
   {
      l_fReturn = FALSE;
   }
   else
   {
      for (fc=0; fc<FC_MAX; fc++)
      {
         if (alt)
         {
            TXA_PL[fc]= TXA[AlternatiefSignaalplan[PL]][fc]*iMultiply;
            TXB_PL[fc]= TXB[AlternatiefSignaalplan[PL]][fc]*iMultiply;
            TXC_PL[fc]= TXC[AlternatiefSignaalplan[PL]][fc]*iMultiply;
            TXD_PL[fc]= TXD[AlternatiefSignaalplan[PL]][fc]*iMultiply;
            TXE_PL[fc]= TXE[AlternatiefSignaalplan[PL]][fc]*iMultiply;
         }
         else
         {
            TXA_PL[fc]= TXA[PL][fc]*iMultiply;
            TXB_PL[fc]= TXB[PL][fc]*iMultiply;
            TXC_PL[fc]= TXC[PL][fc]*iMultiply;
            TXD_PL[fc]= TXD[PL][fc]*iMultiply;
            TXE_PL[fc]= TXE[PL][fc]*iMultiply;
         }
      }
   }
   return l_fReturn;
}

/* -------------------------------------------------------------------------------------- */
/* Functie: HoofdrichtingOpties()                                                         */
/*                                                                                        */
/* Doel:    Instellingen voor hoofdrichtingen bepalen                                     */
/*                                                                                        */
/* Params:  Lijst met variabel aantal argumenten:                                         */
/*          1. dummy element (nodig voor va_list)                                         */
/*          2. index fc hoofdrichting                                                     */
/*          3. schakelaar tegenhouden hoofdrichting toegestaan                            */
/*          4. schakelaar afkappen YW_PL hoofdrichting toegestaan                         */
/*          5. schakelaar afkappen YV_PL hoofdrichting toegestaan                         */
/*                                                                                        */
/*          NB: va_list altijd afsluiten met END                                          */
/*                                                                                        */
/* HoofdrichtingOpties(NG, (va_count) fc02, TRUE, FALSE, TRUE,                            */
/*                         (va_count) fc08, TRUE, FALSE, TRUE,                            */
/*                         (va_count) END);                                               */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
void HoofdrichtingOpties(int dummy, ...)
{
   va_list argpt;                                     /*  variabele argumentenlijst       */
   count fc;                                          /*  arraynummer fc hoofdrichting    */
   mulv sch_tegen;                                    /*  bool tegenhouden hoofdrichting  */
   mulv sch_afkapywpl;                                /*  bool afkappen hoofdrichting     */
   mulv sch_afkapyvpl;                                /*  bool afkappen hoofdrichting     */
   int i;

   for (i=0; i<FC_MAX; i++)
   {
      HoofdRichting[i] = FALSE;
      HoofdRichtingTegenhouden[i] = FALSE;
      HoofdRichtingAfkappenYWPL[i] = FALSE;
      HoofdRichtingAfkappenYVPL[i] = FALSE;
   }

   va_start(argpt,dummy);                             /*  start var. argumentenlijst      */
   do
   {
      fc= va_arg(argpt, va_count);                    /*  lees array-nummer hoofdrichting */

      if (fc>=0)
      {
         sch_tegen     = va_arg(argpt, va_mulv);      /* lees waarde schakelaar           */
         sch_afkapywpl = va_arg(argpt, va_mulv);      /* lees waarde schakelaar           */
         sch_afkapyvpl = va_arg(argpt, va_mulv);      /* lees waarde schakelaar           */

         HoofdRichting[fc] = TRUE;
         HoofdRichtingTegenhouden[fc] = (sch_tegen>=END)     ? sch_tegen     : FALSE;
         HoofdRichtingAfkappenYWPL[fc]= (sch_afkapywpl>=END) ? sch_afkapywpl : FALSE;
         HoofdRichtingAfkappenYVPL[fc]= (sch_afkapyvpl>=END) ? sch_afkapyvpl : FALSE;

      }
   }
   while ((fc>=0) && (fc<FC_MAX) &&
          (sch_tegen>END) && (sch_afkapywpl>END) && (sch_afkapyvpl>END));

   va_end(argpt);                                     /* maak var. arg-lijst leeg         */
}

/* -------------------------------------------------------------------------------------- */
/* Functie: BijhoudenMinimumGroenTijden()                                                 */
/*                                                                                        */
/* Doel:    Bijhouden van de verstreken groentijd (gr_min_timer[])                        */
/*          Bepalen of de minimale groentijd (gr_min[]) voor een fasecyclus is verstreken */
/*                                                                                        */
/* Params:  geen                                                                          */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
void BijhoudenMinimumGroenTijden(void)
{
   int fc;

   for (fc=0;fc<FCMAX;fc++)
   {
      if (!G[fc])
      {
         gr_min_timer[fc] = 0;
      }
      else
      {
         gr_min_timer[fc] += TE;
      }


      if (gr_min_timer[fc] < gr_min[fc])
      {
         gr_min_einde[fc] = FALSE;
      }
      else
      {
         gr_min_einde[fc] = TRUE;
      }
   }
}

/* -------------------------------------------------------------------------------------- */
/* Functie: BijhoudenWachtTijd()                                                          */
/*                                                                                        */
/* Doel:    BijhoudenWachtTijd() meet voor alle fc de actuele wachttijd.                  */
/*          TFB_timer wordt door gebruik van de BL-optie (nooddienst) op nul              */
/*          gezet en is daarom geen juiste weergave van de werkelijke                     */
/*          wachttijd van een fasecyclus.                                                 */
/*                                                                                        */
/* Params:  Geen                                                                          */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
void BijhoudenWachtTijd(void)
{
   int fc;

   for (fc=0;fc<FC_MAX;fc++)
   {
      if (G[fc] || !A[fc] && !RA[fc])
      {
         tfbact[fc] = 0;
      }
      else
      {
         tfbact[fc] += TS;
      }
   }
}

/* -------------------------------------------------------------------------------------- */
/* Functie: WachttijdOverschrijding()                                                     */
/*                                                                                        */
/* Doel:    Bepalen of de maximale wachttijd voor een fasecyclus is overschreden          */
/*                                                                                        */
/* Params:  fc:         OV richting (doet niet mee in wachttijd)                          */
/*          pmwt:       index eerste parameter maximum wachttijd                          */
/*                                                                                        */
/* Return:  boolean wachttijd overschreden                                                */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
bool WachttijdOverschrijding(count fc, count pmwt)
{
   int i;
   bool l_fReturn = FALSE;

   for (i=0; i<FC_MAX; i++)
   {
      if (i!=fc && (tfbact[i]>PRM[pmwt+i]))
      {
         l_fReturn = TRUE;
         break;
      }
   }

   return l_fReturn;
}

/* -------------------------------------------------------------------------------------- */
/* Functie: BepaalGrootsteTO()                                                            */
/*                                                                                        */
/* Doel:    Berekenen van de grootste ontruimingstijd van conflicten naar meegegeven fc   */
/*                                                                                        */
/* Params:  fc:         fasecyclus                                                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
mulv BepaalGrootsteTO(count fc)
{
   mulv to_tmp, to_max;
   int n, k;

   to_max = 0;
#ifdef NO_TIG
   for (n=0; n < KFC_MAX[fc]; n++) {
      k= TO_pointer[fc][n];
      if (TO[k][fc]) {                              /* zoek grootste ontruimingstijd      */
         to_tmp = TGL_max[k] + TO_max[k][fc] - TGL_timer[k] - TO_timer[k];
         if (to_tmp > to_max)
            to_max = to_tmp;
      }
   }
#else
   for (n=0; n < FKFC_MAX[fc]; n++) {
      k= TO_pointer[fc][n];
      if (TIG_max[k][fc]>=0) {                      /* zoek grootste ontruimingstijd      */
        to_tmp= TIG_max[k][fc]-TIG_timer[k];
        if (to_tmp > to_max)
           to_max = to_tmp;
      }
   }
#endif
   return to_max;
}

/* -------------------------------------------------------------------------------------- */
/* Functie: BepaalRealisatieRuimte()                                                      */
/*                                                                                        */
/* Doel:    Bepalen hoeveel ruimte er nog beschikbaar is totdat een richting maximaal is  */
/*          uitgesteld en alleen zijn minimum groentijd nog kan maken tot het TXD moment  */
/*                                                                                        */
/* Params:  fc:         fasecyclus                                                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
int BepaalRealisatieRuimte(count fc)
{
   int to_max;
   int t_aa_max;

   if (R[fc])
   {
      to_max = BepaalGrootsteTO(fc);

      if (!HoofdRichting[fc] || HoofdRichtingTegenhouden[fc])
      {
         t_aa_max= TOTXD_PL[fc] - to_max;
      }
      else
      {
         if ((TOTXB_PL[fc] == 0) && (TOTXD_PL[fc] > 0))
         {
            t_aa_max= 9999;
         }
         else
         {
            t_aa_max= TOTXB_PL[fc] - to_max;
         }
      }
   }
   else
   {
      t_aa_max= NG;
   }

   return t_aa_max;
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
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
int TijdTotLaatsteRealisatieMomentConflict(int fcov, int k)
{
   int totxb_min, totxb_tmp;
   int to_max;

   totxb_min = 32767;
   totxb_tmp = 0;
   to_max = BepaalGrootsteTO(fcov);

   if (R[k] && (HoofdRichting[k] || A[k] || RA[k]))
   {
      if (!HoofdRichting[k] || HoofdRichtingTegenhouden[k])
      {
         /* Startgroen hoofdrichting mag worden uitgesteld voor OV prioriteit of de       */
         /* conflictrichting is geen hoofdrichting en mag dus altijd worden uitgesteld    */
         /* Berekenen hoeveel tijd nodig is om de OV richting te realiseren en ontruimen  */
         /* en daarna de minimale groentijd tot TXD van de conflictrichting toe te wijzen */
         if (TOTXD_PL[k] > 0 || (TX_timer == TXD_PL[k]))
         {
            totxb_min = TOTXD_PL[k] - gr_min[k] - TO_max[fcov][k] - TGL_max[fcov];
         }
      }
      else
      {
         /* Startgroen hoofdrichting mag niet worden uitgesteld voor OV prioriteit        */
         /* Berekenen hoeveel tijd nodig is om de OV richting te realiseren en ontruimen  */
         /* en daarna de minimale groentijd tot TXD van de conflictrichting toe te wijzen */
         if (TXB_PL[k] <= TXC_PL[k])
         {
            if (RA[k] && (TX_timer >= TXB_PL[k]) && (TX_timer < TXC_PL[k]))
            {
               totxb_min = 0;
            }
         }
         else
         {
            if (RA[k] && ((TX_timer >= TXB_PL[k]) || (TX_timer < TXC_PL[k])))
            {
               totxb_min = 0;
            }
         }

         if (TOTXB_PL[k] > 0 || (TX_timer == TXB_PL[k]))
         {
            totxb_tmp = TOTXB_PL[k] - TO_max[fcov][k] - TGL_max[fcov];
            if (totxb_tmp < totxb_min)             /* zoek kleinste starttijd confl. fc   */
            {
               totxb_min = totxb_tmp;
            }
         }
      }
   }

   return (totxb_min - to_max);
}

/* -------------------------------------------------------------------------------------- */
/* Functie: StartGroenConflictenUitstellen()                                              */
/*                                                                                        */
/* Doel:    Bepalen of het startgroen van conflictrichtingen nog uitgesteld kan worden    */
/*          zodat de OV richting bijzonder gerealiseerd kan worden of gedurende de        */
/*          realisatie na het TXD moment groen kan blijven                                */
/*                                                                                        */
/* Params:  fc:         fasecyclus                                                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
bool StartGroenConflictenUitstellen(count fc)
{
   int j, k;
   bool l_fReturn = TRUE;

   for (j=0; j<FKFC_MAX[fc]; j++)
   {
      k = TO_pointer[fc][j];
      if (!ConflictAfgekapt[fc][k] && (TijdTotLaatsteRealisatieMomentConflict(fc, k) <= 0))
      {
         l_fReturn = FALSE;
         break;
      }
   }

   return l_fReturn;
}

/* -------------------------------------------------------------------------------------- */
/* Functie: StartGroenHoofdRichtingenUitstellen()                                         */
/*                                                                                        */
/* Doel:    Bepalen of het startgroen van conflicterende hoofdrichtingen nog uitgesteld   */
/*          kan worden zodat de OV richting bijzonder gerealiseerd kan worden of          */
/*          gedurende de realisatie na het TXD moment groen kan blijven                   */
/*                                                                                        */
/* Params:  fc:         fasecyclus                                                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
bool StartGroenHoofdRichtingenUitstellen(count fc)
{
   int j, k;
   bool l_fReturn = TRUE;

   for (j=0; j<FKFC_MAX[fc]; j++)
   {
      k = TO_pointer[fc][j];
      if (HoofdRichting[k] && !ConflictAfgekapt[fc][k] && (TijdTotLaatsteRealisatieMomentConflict(fc, k) <= 0))
      {
         l_fReturn = FALSE;
         break;
      }
   }

   return l_fReturn;
}

/* -------------------------------------------------------------------------------------- */
/* Functie: HoofdRichtingGroen()                                                          */
/*                                                                                        */
/* Doel:    Bepaalt of er tenminste een van de hoofdrichtingen groen is                   */
/*                                                                                        */
/* Params:  geen                                                                          */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
bool HoofdRichtingGroen(void)
{
   bool l_fReturn = FALSE;
   int i;

   for (i=0; i<FC_MAX && !l_fReturn; i++)
   {
      if (HoofdRichting[i])
      {
         if (G[i])
         {
            l_fReturn = TRUE;
            break;
         }
      }
   }
   return l_fReturn;
}


/* -------------------------------------------------------------------------------------- */
/* Functie: TOTXB_AlternatiefSignaalplan()                                                */
/*                                                                                        */
/* Doel:    Bepaalt de tijd tot het TXB moment van de meegegeven fasecyclus in het        */
/*          alternatieve signaalplan                                                      */
/*                                                                                        */
/* Params:  fc:         fasecyclus                                                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
int TOTXB_AlternatiefSignaalplan(count fc)
{
   mulv totxb_tmp = -1;
   int  pl = AlternatiefSignaalplan[PL];

   if (pl >= 0)
   {
      /* set TOTXB_PL[] */
      /* -------------- */
      if (TXB[pl][fc] >= TX_PL_timer)
      {
         if (TXD[pl][fc]>=TX_PL_timer && TXD[pl][fc]>=TXB[pl][fc] || TXD[pl][fc]<TX_PL_timer)
            totxb_tmp= TXB[pl][fc]-TX_PL_timer;
         else
            totxb_tmp= 0;
      }
      else
      {
         if (TXD[pl][fc]<=TX_PL_timer && TXD[pl][fc]>=TXB[pl][fc])
            totxb_tmp= (TX_PL_max-TX_PL_timer) + TXB[pl][fc];
         else
            totxb_tmp=0;
      }

#ifndef TX_PL_TE
   totxb_tmp= totxb_tmp*10;
#endif
   }

   return totxb_tmp;
}

/* -------------------------------------------------------------------------------------- */
/* Functie: OV_CCOL_Elementen_ple()                                                       */
/*                                                                                        */
/* Doel:    Bijhouden van de diverse CCOL elementen die nodig zijn                        */
/*          voor de functie OVIngreep_ple()                                               */
/*                                                                                        */
/* Params:  fc:         OV richting                                                       */
/*          inm:        inmeldvoorwaarde (SH[], SD[])                                     */
/*          inm2:       tweede inmeldvoorwaarde (SH[], SD[] of NG)                        */
/*          uitm:       uitmeldvoorwaarde (SH[], SD[])                                    */
/*          uitm2:      tweede uitmeldvoorwaarde (SH[], SD[] of NG)                       */
/*          cvcov:      ov teller                                                         */
/*          cvbov:      ov buffer teller                                                  */
/*          tiv:        inmeldingsvertraging                                              */
/*          tib:        inmeldingsbewaking                                                */
/*          tblk:       blokkeringstijd                                                   */
/*          textra:     extra groentijd na TXD                                            */
/*          hprio:      hulpelement busprioriteit                                         */
/*          prio:       waarde prioriteitstype                                            */
/*          omax:       waarde ondermaximum                                               */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
void OV_CCOL_Elementen_ple(count fc,      /* OV richting                                  */
                           bool  inm,     /* inmeldvoorwaarde (SH[], SD[])                */
                           bool  inm2,    /* tweede inmeldvoorwaarde (SH[], SD[] of NG)   */
                           bool  uitm,    /* uitmeldvoorwaarde (SH[], SD[])               */
                           bool  uitm2,   /* tweede uitmeldvoorwaarde (SH[], SD[] of NG)  */
                           count cvc,     /* ov teller                                    */
                           count cvb,     /* ov buffer teller                             */
                           count tiv,     /* inmeldingsvertraging                         */
                           count tib,     /* inmeldingsbewaking                           */
                           count tgb,     /* groenbewaking                                */
                           count tblk,    /* blokkeringstijd                              */
                           count extra,   /* extra groentijd na TXD                       */
                           count hprio,   /* hulpelement busprioriteit                    */
                           mulv  prio,    /* waarde prioriteitstype                       */
                           mulv  omax,    /* waarde ondermaximum                          */
                           count pmwt,    /* parameter maximum wachttijd (1e fc)          */
                           bool  ov_mag)  /* extra voorwaarde voor toestaan OV prio       */
{
   bool in_omax;

   /* ----------------------------------------------------------------------------------- */
   /* Bepalen of bus nog binnen ondermaximum primair gerealiseerd kan worden              */
   /* ----------------------------------------------------------------------------------- */
   in_omax =  R[fc] && !TRG[fc]              /* tijdens rood altijd                       */
           || G[fc] && (CG[fc]<=CG_WG        /* tijdens groen t/m wachtgroen altijd       */
           || YV_PL[fc] && (
           prio>1                       /* als prio > 1 dan extra verlengen na TXD moment */
           ? ((TOTXD_PL[fc]+T_max[extra]) > (omax+T_max[tiv]))
           : ((TOTXD_PL[fc]) > (omax+T_max[tiv]))));

   /* ----------------------------------------------------------------------------------- */
   /* Administratie tellers                                                               */
   /* inmeldingen ov toewijzen aan actuele of bufferteller                                */
   /* ----------------------------------------------------------------------------------- */

   /* inmelding binnen ondermaximum direct aan actuele teller toewijzen                   */
   INC[cvc] = (inm || (inm2 > NG) && inm2) && in_omax;

   /* inmelding naar bufferteller als fc niet binnen ondermaximum gerealiseerd kan worden */
   INC[cvb] = (inm || (inm2 > NG) && inm2) && !INC[cvc];

   /* verplaatsen van buffer naar actuele teller als realisatie weer binnen ondermax kan  */
   if (C[cvb] && in_omax)
   {
      /* actuele teller ophogen met buffer en eventuele gelijktijdige inmelding (INC)     */
      DCL[cvc] = C_counter[cvb] + INC[cvc] + C_counter[cvc];
      /* bufferteller resetten                                                            */
      RC[cvb] = TRUE;
   }
   else
   {
      DCL[cvc] = RC[cvb] = 0;
   }

   /* ----------------------------------------------------------------------------------- */
   /* uitmeldingen verwerken                                                              */
   /* ----------------------------------------------------------------------------------- */

   /* uitmelding verwijderen uit actuele teller                                           */
   DEC[cvc] = uitm || (uitm2 > NG && uitm2);

   /* uitmelding verwijderen uit bufferteller                                             */
   DEC[cvb] = (uitm || (uitm2 > NG && uitm2)) && !C[cvc];

   /* uitmelding verwijderen uit tellers als groen- of inmeldbewaking aanspreekt          */
   RC[cvc] = ET[tgb] && !AT[tgb] || ET[tib];
   RC[cvb]|= ET[tib];


   /* ----------------------------------------------------------------------------------- */
   /* Bepalen of prioriteit toegestaan is                                                 */
   /* ----------------------------------------------------------------------------------- */
   if (EG[fc] || !C[cvc] && G[fc] || ET[tib])
   {
      IH[hprio] = FALSE;
   }

   if (ov_mag && C[cvc] && !T[tblk] && !T[tiv] && !(RR[fc]&OV_PLE_BIT) &&
      (G[fc] || (!CK[fc] && A[fc])) && !WachttijdOverschrijding(fc, pmwt))
   {
      IH[hprio] = TRUE;
   }


   /* ----------------------------------------------------------------------------------- */
   /* Administratie tijden                                                                */
   /* groenbewaking, inmeldbewaking, blokkeringstijd en inmeldvertraging                  */
   /* ----------------------------------------------------------------------------------- */

   /* bijhouden groenbewaking                                                             */
   RT[tgb] = IH[hprio] && !H[hprio] && G[fc] || /* herstart bij eerste bus tijdens groen  */
             IH[hprio] && SG[fc] ||             /*          bij eerste bus buiten groen   */
             (inm || (inm2>NG) && inm2) && G[fc] && (CG[fc]<=CG_WG /* tweede bus geen tiv */
             || YV_PL[fc] && (
             prio>1                     /* als prio > 1 dan extra verlengen na TXD moment */
             ? ((TOTXD_PL[fc]+T_max[extra]) > omax)
             : ( TOTXD_PL[fc] > omax)));

   AT[tgb] = !C[cvc] && !INC[cvc] || !IH[hprio] && H[hprio];

   /* bijhouden inmeldbewaking                                                            */
   RT[tib] = (inm || (inm2>NG) && inm2);
   AT[tib] = !C[cvc] && !INC[cvc] && !C[cvb] && !INC[cvb];

   /* bijhouden blokkeringstijd                                                           */
   RT[tblk] = EH[hprio];

   /* bijhouden inmeldvertraging                                                          */
   RT[tiv] = (inm || (inm2>NG) && inm2) && !C[cvc];
}

/* -------------------------------------------------------------------------------------- */
/* Functie: OVIngreep_ple()                                                               */
/*                                                                                        */
/* Doel:    Realiseren van een OVingreep binnen een signaalplan                           */
/*                                                                                        */
/* Params:  fc:         OV richting                                                       */
/*          ov_ok:      extra voorwaarde voor toekennen prioriteit                        */
/*          extra:      max. extra groentijd na TXD moment                                */
/*          pprio:      waarde prioriteitstype (1, 2, ... of PRM[])                       */
/*                         NB: deze waarde ophogen met waarde WISSEL_FASEVOLGORDE om een  */
/*                             wisseling in fasevolgorde toe te staan tbv OV prioriteit   */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
void OVIngreep_ple(count fc,     /* fc met prioriteit                                     */
                   bool  inm,    /* inmeldvoorwaarde (SH[], SD[])                         */
                   bool  inm2,   /* tweede inmeldvoorwaarde (SH[],SD[] of NG)             */
                   bool  uitm,   /* uitmeldvoorwaarde (SH[], SD[])                        */
                   bool  uitm2,  /* tweede uitmeldvoorwaarde (SH[],SD[] of NG)            */
                   count cvc,    /* ov teller                                             */
                   count cvb,    /* ov buffer teller                                      */
                   count tiv,    /* inmeldingsvertraging                                  */
                   count tib,    /* inmeldingsbewaking                                    */
                   count tgb,    /* groenbewaking                                         */
                   count tblk,   /* blokkeringstijd                                       */
                   mulv  textra, /* extra groentijd na TXD                                */
                   count hprio,  /* ov prioriteit                                         */
                   count rijtijd,/* ongehinderde rijtijd tot stopstreep                   */
                   mulv  prio,   /* prioriteitstype                                       */
                   mulv  omax,   /* waarde ondermaximum                                   */
                   count pmwt,   /* parameter maximum wachttijd (1e fc)                   */
                   count mtx,    /* geheugenelement cyclustijd bij signaalplan wisseling  */
                   count mfcpl,  /* geheugenelement actieve OV fasecyclus bij wisseling   */
                   bool  ov_mag) /* extra voorwaarde voor toestaan OV prio                */
{
  int i, j;
  mulv to_tmp, to_max;
  bool fasevolgorde_wisselen_mag = ((prio % WISSEL_FASEVOLGORDE)<prio);
  bool fasevolgorde_wisselen_gewenst = TRUE;
  bool blokkeren_mag = FALSE;   /* blokkeren van conflicten mag bij absolute prioriteit  */
  bool afkappen_mag = FALSE;    /*  afkappen van conflicten mag bij absolute prioriteit  */
  
  /* Resetten ConflictAfgekapt                                                           */
  if (EG[fc])
  {
     for (i=0; i<FC_MAX; i++)
     {
        ConflictAfgekapt[fc][i] = FALSE;
     }
  }
  
  /* Resetten aanvraag indien er geen prioriteitsreden meer is                           */
  if (!IH[hprio] && H[hprio])
  {
     A[fc] &= ~OV_PLE_BIT;
     if (!A[fc]) TFB_timer[fc] = 0;
  }
  
  /* Resetten geheugenelementen */
  if (TX_PL_timer == MM[mtx] && !G[MM[mfcpl]])
  {
    MM[mtx] = NG;
    MM[mfcpl] = NG;
    PasSignaalplanToe(ORIGINEEL_PLAN);
  }
  
  /* ----------------------------------------------------------------------------------- */
  /* Bijhouden CCOL elementen tbv tellers, timers etc                                    */
  /* ----------------------------------------------------------------------------------- */
  OV_CCOL_Elementen_ple(fc, inm, inm2, uitm, uitm2,        /* in- en uitmeldingen        */
                            cvc, cvb,                      /* OV tellers                 */
                            tiv, tib, tgb, tblk, textra,   /* timers                     */
                            hprio,                         /* hulpelement prio           */
                            prio,                          /* prioniveau                 */
                            omax,                          /* ondermaximum               */
                            pmwt,                          /* maximale wachttijd (1e fc) */
                            ov_mag);                       /* extra voorwaarde ov prio   */
  
  /* ----------------------------------------------------------------------------------- */
  /* Daadwerkelijke OV ingreep                                                           */
  /* ----------------------------------------------------------------------------------- */
  if (ov_mag)
  {
     to_max = BepaalGrootsteTO(fc);
  
     /* -------------------------------------------------------------------------------- */
     /* Ongeacht prioniveau altijd aanvraag OV richting opzetten                         */
     /* (mits er geen ontruimingstijden van conflicten naar de OV richting meer lopen)   */
     /* -------------------------------------------------------------------------------- */
     if (R[fc] && !TRG[fc] && !CK[fc] && (T[tib] && (T_timer[tib] >= (rijtijd - to_max)) || C[cvc]))
     {
        A[fc] |= OV_PLE_BIT;
     }
  
     switch (prio%WISSEL_FASEVOLGORDE)
     {
        /* -------------------------------------------------------------------------- */
        /* prio==0: geen prioriteit, behalve eerdere A[] geen extra acties            */
        /* -------------------------------------------------------------------------- */
        case 0:
           break;
  
        /* -------------------------------------------------------------------------- */
        /* prio==1: verlengen groen tot TXD (mits rijtijd nog gemaakt kan worden)     */
        /* -------------------------------------------------------------------------- */
        case 1:
  
           /* MK[] opzetten zolang restant rijtijd korter is dan tijd tot TXD         */
           if (C[cvc] && (TOTXD_PL[fc] >= (rijtijd - T_timer[tib])))
           {
              MK[fc] |= OV_PLE_BIT;
  
           /* vasthouden groen tot maximaal TXD moment                                */
           /* bepalen of (versneld) primaire realisatie kan dmv test_pr_fk_totxb      */
           /* bepalen of er nog ruimte is om te verlengen dmv yv_ar_max_pl()          */
              if (IH[hprio] && (((YV_PL[fc] && PR[fc]) || (AR[fc] && yv_ar_max_pl(fc, 0)))))
              {
                 fasevolgorde_wisselen_gewenst = FALSE;
                 YM[fc] |= OV_PLE_BIT;
                 YV[fc] |= OV_PLE_BIT;
              }
           }
           break;
  
        /* -------------------------------------------------------------------------- */
        /* prio==2: afkappen conflictrichtingen en verlengen na TXD moment            */
        /* -------------------------------------------------------------------------- */
        case 2:
  
           blokkeren_mag = FALSE; /*    blokkeren van conflicten mag niet bij prio==2 */
           afkappen_mag  = TRUE;  /*          afkappen van conflicten mag bij prio==2 */
  		   
           /* administratie extra groentijd na TXD moment                             */
           RT[textra] = G[fc] && (TX_timer==TXD_PL[fc]);  /* herstarten op TXD moment */
           AT[textra] = !G[fc];                    /* afbreken indien niet groen meer */
  
           if (IH[hprio])
           {
              /* ophogen langstwachtende teller om zodoende eerder dan conflicterende */
              /* alternatieven aan de beurt te zijn en zo prioriteit te krijgen       */
              if (CALW[fc] < PRI_CALW)
              {
                 set_CALW(fc, PRI_CALW);
              }
  
              /* bepalen of groen van conflictrichtingen beeindigd kan worden         */
              /* dit mag gebeuren als tenminste de minimum groentijd verstreken is    */
              /* voor hoofdrichtingen moet ook gelden dat deze afgekapt mogen worden  */
              /* als tenminste een van de richtingen niet afgekapt mag worden, wordt  */
              /* de bool afkappen_mag FALSE en wordt geen enkele richting afgekapt    */
              for (i=0; i<FKFC_MAX[fc] && afkappen_mag; i++)
              {
                 j = TO_pointer[fc][i];
                 if (HoofdRichting[j])
                 {
                    /* Maatregelen als conflict groen is en min. groen bereikt heeft  */
                    if (G[j])
                    {
                       if (!gr_min_einde[j])
                       {
                          /* hoofdrichting heeft zijn min.groentijd nog niet bereikt  */
                          /* afkappen van conflictrichtingen tbv prio mag nog niet    */
                          afkappen_mag  = FALSE;
                          break;
                       }
                       else if (YW_PL[j] && !HoofdRichtingAfkappenYWPL[j])
                       {
                          /* hoofdrichting wordt vastgehouden voor een peloton        */
                          /* afkappen van conflictrichtingen tbv prio mag nog niet    */
                          afkappen_mag  = FALSE;
                          break;
                       }
                       else if (!YW_PL[j] && YV_PL[j] && !HoofdRichtingAfkappenYVPL[j])
                       {
                          /* hoofdrichting wordt vastgehouden in verlenggroen         */
                          /* afkappen van conflictrichtingen tbv prio mag nog niet    */
                          afkappen_mag  = FALSE;
                          break;
                       }
                       else if (iNietAfkappen[j])
                       {
                         /* hoofdrichting mogen niet worden afgekapt                  */
                         /* volgens user defined settings                             */
                         afkappen_mag = FALSE;
                         break;
                       }
                    }
                 }
                 else if (G[j] && !gr_min_einde[j])
                 {
                    /* conflictrichting heeft zijn minimumgroentijd nog niet bereikt  */
                    /* afkappen van conflictrichtingen tbv prio mag nog niet          */
                    afkappen_mag = FALSE;
                    break;
                 }
                 else if (G[j] && iNietAfkappen[j])
                 {
                   /* richting mogen niet worden afgekapt                             */
                   /* volgens user defined settings                                   */
                   afkappen_mag = FALSE;
                   break;
                 }
              }
  
  
              /* groen conflictrichtingen beeindigen                                  */
              if (afkappen_mag)
              {
                 fasevolgorde_wisselen_gewenst = FALSE;
                 for (i=0; i<FKFC_MAX[fc]; i++)
                 {
                    j = TO_pointer[fc][i];
                    /* Afkappen wachtgroen en/of verlenggroen door FM instructie      */
                    RS[j] = YW[j] = RW[j] = YV[j] = FALSE;
                    FM[j] |= OV_PLE_BIT;
                    ConflictAfgekapt[fc][j] |= G[j];
  
                    /* Alternatieve realisatie conflictrichtingen terugzetten         */
                    if (G[fc] && (RA[j] || SRV[j]) && AR[j])
                    {
                       RR[j] |= OV_PLE_BIT;
                    }
                 }
              }
  
              /* groenfase OV richting verlengen tot (na) TXD moment                  */
              if (G[fc])
              {
                 /* meetkriterium opzetten zolang restant rijtijd < TOTXD+rijtijd     */
                 if ((TOTXD_PL[fc] + T_max[textra]) >= (rijtijd - T_timer[tib]))
                 {
                    MK[fc] |= OV_PLE_BIT;
                 }
  
                 /* zolang OV richting nog in primaire gebied zit                     */
                 if ((TOTXB_PL[fc] == 0) || YS_PL[fc] || yv_ar_max_pl(fc,0))
                 {
                    if ((TOTXD_PL[fc] + T_max[textra]) >= (rijtijd - T_timer[tib]))
                    {
                       MK[fc] |= OV_PLE_BIT;
                       YV[fc] |= OV_PLE_BIT;
                       YM[fc] |= OV_PLE_BIT;
                       if ((TOTXB_PL[fc] == 0) || YS_PL[fc])
                       {
                          RW[fc] |= OV_PLE_BIT;
                       }
                    }
                 }
                 /* indien OV richting na TXD groen is en extra groentijd loopt       */
                 else if (T[textra] || RT[textra])
                 {
                    /* maatregelen zolang extra tijd benut kan worden                 */
                    /* tevens moet startgroen conflicten uitgesteld mogen worden om   */
                    /* te voorkomen dat de conflicten geen realisatieruimte hebben    */
                    if (((T_max[textra]-T_timer[textra]) >= (rijtijd-T_timer[tib])) &&
                          StartGroenConflictenUitstellen(fc))
                    {
                       MK[fc] |= OV_PLE_BIT;
                       YV[fc] |= OV_PLE_BIT;
                       YM[fc] |= OV_PLE_BIT;
                    }
                 }
              }
           }
           break;
  
        /* -------------------------------------------------------------------------- */
        /* prio==3: absolute prioriteit                                               */
        /* -------------------------------------------------------------------------- */
        case 3:
  
           blokkeren_mag = TRUE;/* blokkeren conflicten mag bij absolute prioriteit   */
           afkappen_mag = TRUE; /*  afkappen conflicten mag bij absolute prioriteit   */
  
           /* aanvraag eerder zetten dan bij lagere prioniveaus: als bus aanwezig is  */
           if (R[fc] && !TRG[fc] && !CK[fc] && T[tib])
           {
              A[fc] |= OV_PLE_BIT;
           }
  
           if (IH[hprio])
           {
              /* ophogen langstwachtende teller om zodoende eerder dan conflicterende */
              /* alternatieven aan de beurt te zijn en zo prioriteit te krijgen       */
              if (CALW[fc] < PRI_CALW)
              {
                 set_CALW(fc, PRI_CALW);
              }
  
              /* -------------------------------------------------------------------- */
              /* bepalen of groen van conflicten geblokkeerd en/of beeindigd mag      */
              /* -------------------------------------------------------------------- */
  
              /* blokkeren mag als voor het TXB moment van de hoofdrichtingen de      */
              /* ongehinderde rijtijd + ontruiming gemaakt kan worden                 */
              /* als tenminste een van de hoofdrichtingen te snel groen wordt, wordt  */
              /* bool blokkeren_mag FALSE en wordt geen enkele richting geblokkeerd   */
  
              /* afkappen mag als tenminste de minimum groentijd verstreken is        */
              /* voor hoofdrichtingen geldt tevens dat deze afgekapt mogen worden     */
              /* als tenminste een van de richtingen niet afgekapt mag worden, wordt  */
              /* de bool afkappen_mag FALSE en wordt geen enkele richting afgekapt    */
              for (i=0; i<FKFC_MAX[fc]; i++)
              {
                 j = TO_pointer[fc][i];
                 if (HoofdRichting[j])
                 {
                    if (R[j] && !ConflictAfgekapt[fc][j] && (TijdTotLaatsteRealisatieMomentConflict(fc, j) <= 0))
                    {
                       /* hoofdrichting wordt binnen de ongehinderde rijtijd groen    */
                       /* blokkeren van conflictrichtingen tbv prio is niet mogelijk  */
                       /* afkappen van conflictrichtingen tbv prio is niet wenselijk  */
                       blokkeren_mag = FALSE;
                       afkappen_mag  = FALSE;
                       break;
                    }
                    if (YW_PL[j] && !HoofdRichtingAfkappenYWPL[j])
                    {
                       /* hoofdrichting wordt vastgehouden voor een peloton           */
                       /* blokkeren van conflictrichtingen tbv prio is niet wenselijk */
                       /* afkappen van conflictrichtingen tbv prio is niet mogelijk   */
                       blokkeren_mag = FALSE;
                       afkappen_mag  = FALSE;
                       break;
                    }
                    if (!YW_PL[j] && YV_PL[j] && !HoofdRichtingAfkappenYVPL[j])
                    {
                       /* hoofdrichting wordt vastgehouden in verlenggroen            */
                       /* afkappen van conflictrichtingen tbv prio is niet mogelijk   */
                       afkappen_mag  = FALSE;
                       break;
                    }
                 }
                 if (G[j] && iNietAfkappen[j])
                 {
                   /* richting mogen niet worden afgekapt                             */
                   /* volgens user defined settings                                   */
                   afkappen_mag = FALSE;
                   break;
                 }
              }
  
  
              /* alle conflicten mogen geblokkeerd worden, bijzondere realisatie OV   */
              if (blokkeren_mag)
              {
                 fasevolgorde_wisselen_gewenst = FALSE;
                 for (i=0; i<FKFC_MAX[fc]; i++)
                 {
                    j = TO_pointer[fc][i];
                    if (R[j])
                    {
                       RR[j] |= OV_PLE_BIT;
                       BL[j] |= OV_PLE_BIT;
                    }
                 }
                 if (R[fc] && !TRG[fc] && !CK[fc])
                 {
                    AA[fc] = TRUE;
                 }
              }
  
              /* als alle conflictrichtingen beeindigd mogen worden, afkappen groen   */
              if (afkappen_mag)
              {
                 fasevolgorde_wisselen_gewenst = FALSE;
                 for (i=0; i<FKFC_MAX[fc]; i++)
                 {
                    j = TO_pointer[fc][i];
                    if (!HoofdRichting[j] || blokkeren_mag)
                    {
                       YW[j] = RW[j] = YV[j] = FALSE;
                       FM[j] |= OV_PLE_BIT;
                       ConflictAfgekapt[fc][j] |= G[j];
                    }
  
                    /* Alternatieve realisatie conflictrichtingen terugzetten         */
                    if (G[fc] && (RA[j] || SRV[j]) && AR[j])
                    {
                       RR[j] |= OV_PLE_BIT;
                    }
                 }
              }
  
              /* groenfase OV richting verlengen tot OV voertuig verdwenen is         */
              /* dit met inachtname van de conflicterende hoofdrichtingen             */
              if (G[fc] && T[tib] && StartGroenHoofdRichtingenUitstellen(fc))
              {
                 /* meetkriterium opzetten zolang inmeldbewaking loopt                */
                 MK[fc] |= OV_PLE_BIT;
                 /* vasthouden groen                                                  */
                 YV[fc] |= OV_PLE_BIT;
                 YM[fc] |= OV_PLE_BIT;
              }
           }
           break;
  
        /* -------------------------------------------------------------------------- */
        /* prio==4: nooddienst                                                        */
        /* -------------------------------------------------------------------------- */
        case 4:
  
           fasevolgorde_wisselen_gewenst = FALSE;
  
           /* aanvraag eerder zetten dan bij lagere prioniveaus: als bus aanwezig is  */
           if (R[fc] && !TRG[fc] && T[tib])
           {
              A[fc] |= OV_PLE_BIT;
           }
  
           if (IH[hprio])
           {
              /* ophogen langstwachtende teller om zodoende eerder dan conflicterende */
              /* alternatieven aan de beurt te zijn en zo prioriteit te krijgen       */
              if (CALW[fc] < PRI_CALW)
              {
                 set_CALW(fc, PRI_CALW);
              }
              /* conflicterende hoofdrichtingen direct blokkeren en afkappen          */
              for (i=0; i<FKFC_MAX[fc]; i++)
              {
                 j = TO_pointer[fc][i];
                 if (R[j])
                 {
                    RR[j] |= OV_PLE_BIT;
                    BL[j] |= OV_PLE_BIT;
                 }
                 if (G[j])
                 {
                    FM[j] |= OV_PLE_BIT;
                    YW[j] = RW[j] = YV[j] = FALSE;
                 }
                 /* Alternatieve realisatie conflictrichtingen terugzetten            */
                 if (G[fc] && (RA[j] || SRV[j]) && AR[j])
                 {
                    RR[j] |= OV_PLE_BIT;
                 }
              }
  
              if (R[fc] && !TRG[fc])
              {
                 AA[fc] = TRUE;
              }
              if (G[fc])
              {
                 MK[fc] |= OV_PLE_BIT;
                 YM[fc] |= OV_PLE_BIT;
                 RW[fc] |= OV_PLE_BIT;
              }
           }
  
           break;
  
        default:
           break;
     } /* switch prio                                                                 */
  
     /* Als de OV richting geen groen heeft gehad, maar de ingreep is niet langer actief */
     /* dan dient de langstwachtende teller (voorkeursrealisatie) weer gereset te worden */
     if (EH[hprio])
     {
        set_CALW(fc, (mulv) (10 * TFB_timer[fc]));
     }
  
  
     /* Indien de OV richting bijzonder gerealiseerd wordt (prio>=3), kan het gebeuren   */
     /* dat een conflictrichting geen ruimte meer overhoudt voor de primaire realisatie  */
     /* In dat geval dient het overslag-bit opgezet te worden voor de conflictrichting,  */
     /* deze zal dan de volgende cyclus pas gerealiseerd worden                          */
     if (G[fc] && (prio >= 3) && (IH[hprio] || !PR[fc] && !AR[fc]))
     {
        for (i = 0;i < FKFC_MAX[fc]; i++)
        {
           to_tmp= 0;
           j = TO_pointer[fc][i];                    /* Index conflicterende fc          */
           if (!HoofdRichting[j] || (prio > 3))
           {
              to_tmp = max(TFG_max[fc]- TFG_timer[fc], TGG_max[fc]-TGG_timer[fc])
                     + TGL_max[fc] + TO_max[fc][j];
              if (to_tmp > TOTXD_PL[j])
              {
                 if (!PG[j])
                 {
                    PG[j] = PRIMAIR_OVERSLAG;
                 }
                 /* Alternatieve realisatie terugzetten                                  */
                 if ((RA[j] || SRV[j]) && (PR[j] || AR[j]))
                 {
                    RR[j] |= BIT6;
                 }
              }
           }
        } /* for (i = 0;i < FKFC_MAX[fc]; i++)                                           */
     } /* if (G[fc] && (prio >= 3) && (IH[hprio] || !PR[fc] && !AR[fc]))                 */
  
  
     /* Indien niet op de 'normale' wijze prioriteit is verleend, kan worden overgegaan  */
     /* tot het wisselen van de fasevolgorde (indien prioniveau dit toelaat, dwz > 1000) */
     if (IH[hprio] && fasevolgorde_wisselen_mag && fasevolgorde_wisselen_gewenst)
     {
        if (MM[mtx] == NG)
        {
           /* Bepalen wat de resterende tijd tot het TXB moment van de OV richting in    */
           /* het alternatieve signaalplan is                                            */
           int totxb = TOTXB_AlternatiefSignaalplan(fc);
  
           /* Als resterende tijd tot TXB kleiner is in het alternatieve plan dan in het */
           /* actuele signaalplan, dan heeft het zin om om te schakelen naar alternatief */
           if ((totxb > 0) && (totxb < TOTXB_PL[fc]))
           {
              /* Probeer TX instellingen alternatief signaalplan over te nemen           */
              if (PasSignaalplanToe(ALTERNATIEF_PLAN))
              {
                 /* onthoud actuele cyclustijd (-1 seconde)                              */
                 MM[mtx] = (TX_PL_timer > 1) ? (TX_PL_timer - 1) : TX_PL_max;
                 MM[mfcpl] = fc;
              }
              else
              {
                 /* Originele signaalplantijden terugzetten, alternatief plan lukt niet  */
                 PasSignaalplanToe(ORIGINEEL_PLAN);
                 MM[mtx] = NG;
                 MM[mfcpl] = NG;
              }
           } /* if ((totxb > 0) && (totxb < TOTXB_PL[fc]))                               */
        } /* if (MM[mtx] == NG)                                                          */
     } /* if (IH[hprio] && fasevolgorde_wisselen_mag && fasevolgorde_wisselen_gewenst)   */
  } /* if (ov_mag)                                                                       */

#ifndef AUTOMAAT
  /*xyprintf(1,0,"TX: %03d", TX_PL_timer);
  
  if (fc==fc05) xyprintf(1,2,"fc%s prio:%1d kap:%1d blok:%1d wt:%1d StartGroenConflictenUitstellen: %1d", FC_code[fc], IH[hprio], afkappen_mag, blokkeren_mag, WachttijdOverschrijding(fc, prmmwt02ple), StartGroenConflictenUitstellen(fc05));
  if (fc==fc05) xyprintf(1,4,"fc%s TOB:%04d TOD:%04d PG:%1d kap:%1d, tfb:%04d H:%1d, X:%1d, YW:%1d, YV:%1d, gr_min:%4d, gr_min_einde:%4d",FC_code[fc], TOTXB_PL[fc], TOTXD_PL[fc], (bool) (PG[fc]), ConflictAfgekapt[fc][fc], tfbact[fc], HoofdRichting[fc], HoofdRichtingTegenhouden[fc], HoofdRichtingAfkappenYWPL[fc], HoofdRichtingAfkappenYVPL[fc], gr_min[fc], gr_min_einde[fc]);
  
  for (i=0; fc==fc05 && i<FKFC_MAX[fc05]; i++)
  {
     j = TO_pointer[fc05][i];
     xyprintf(1,i+5,"fc%s TOB:%04d TOD:%04d PG:%1d kap:%1d TTR:%05d tfb:%04d H:%1d, X:%1d, YW:%1d, YV:%1d, gr_min:%4d, gr_min_einde:%4d",FC_code[j], TOTXB_PL[j], TOTXD_PL[j], (bool) (PG[j]), ConflictAfgekapt[fc][j], TijdTotLaatsteRealisatieMomentConflict(fc, j), tfbact[j], HoofdRichting[j], HoofdRichtingTegenhouden[j], HoofdRichtingAfkappenYWPL[j], HoofdRichtingAfkappenYVPL[j], gr_min[j], gr_min_einde[j]);
  }
  
  
  
  if (fc==fc11) xyprintf(1,20,"fc%s prio:%1d kap:%1d blok:%1d wt:%1d StartGroenConflictenUitstellen: %1d", FC_code[fc], IH[hprio], afkappen_mag, blokkeren_mag, WachttijdOverschrijding(fc, prmmwt02ple), StartGroenConflictenUitstellen(fc05));
  if (fc==fc11) xyprintf(1,22,"fc%s TOB:%04d TOD:%04d PG:%1d kap:%1d, tfb:%04d H:%1d, X:%1d, YW:%1d, YV:%1d, gr_min:%4d, gr_min_einde:%4d",FC_code[fc], TOTXB_PL[fc], TOTXD_PL[fc], (bool) (PG[fc]), ConflictAfgekapt[fc][fc], tfbact[fc], HoofdRichting[fc], HoofdRichtingTegenhouden[fc], HoofdRichtingAfkappenYWPL[fc], HoofdRichtingAfkappenYVPL[fc], gr_min[fc], gr_min_einde[fc]);
  for (i=0; fc==fc11 && i<FKFC_MAX[fc11]; i++)
  {
   j = TO_pointer[fc11][i];
   xyprintf(1,i+23,"fc%s TOB:%04d TOD:%04d PG:%1d kap:%1d TTR:%05d tfb:%04d H:%1d, X:%1d, YW:%1d, YV:%1d, gr_min:%4d, gr_min_einde:%4d",FC_code[j], TOTXB_PL[j], TOTXD_PL[j], (bool) (PG[j]), ConflictAfgekapt[fc][j], TijdTotLaatsteRealisatieMomentConflict(fc, j), tfbact[j], HoofdRichting[j], HoofdRichtingTegenhouden[j], HoofdRichtingAfkappenYWPL[j], HoofdRichtingAfkappenYVPL[j], gr_min[j], gr_min_einde[j]);
  }*/
#endif

}


