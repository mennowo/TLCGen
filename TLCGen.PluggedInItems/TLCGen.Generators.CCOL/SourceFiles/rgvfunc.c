/* DEFINITIE FUNCTIES - ROBUUSTE GROENTIJD VERDELER */
/* ================================================ */


/* (C) Copyright 2006-2009 by A.C.M. van Grinsven. All rights reserved. */


/* CCOL:  versie 6.0 en 7.0   */
/* FILE:  rgvfunc.c       */
/* DATE:  28-01-2007          */
/****************************** Versie commentaar ***********************************
 *
 * Versie  Datum       Naam Commentaar
 *
 * 1.0.0   08-03-2005  dze  originele versie 
 *
 * 2.0.0   11-08-2005  mim  - cyclische aanvragen 02, 05, 08 en 11 ivm fasebewaking
 *              - toevoegen rateltikkers + periode dimmen
 *              - toevoegen detectoren d7_3, d55_1, d55_2, d61_1, d61_2
 *              - hernoemen fase 65 in fase 55
 *              - hernoemen fase 71 in fase 61
 *
 * 2.1.0   21-09-2006  dze  * oplossing fasebewaking
 *              * vastlegging parameters 
 * 3.0.0   13-nov-2007 ps       Robugrover definitief toegevoegd na aantal testversies 
 * 3.1.0   15-nov-2007 ps       Wijzigingen op voorstel van Ton doorgevoerd. 
 * 4.0.0   7-nov-2008  ps       Vele wijzigingen 
 * 4.1.0   19-mei-2009 TvG      Cosmetische slag
 * 5.0.0   22-nov-2010 PS      Diverse aanpassingen
 * 5.1.0   20-april-2011 PS      TVG_basis en TVG_basis_som moeten groter zijn dan 0 om te delen
 ************************************************************************************/



/* include files */
/* ============= */
   #include <stdarg.h>     /* gebruik variabele argumentenlijst   */
   #include "fcvar.h"      /* declaratie fasecyclusvariabelen     */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || (defined (VISSIM) && !defined PROMITE)
   #include "xyprintf.h"   /* declaratie xyprintf()               */
#endif


/* MACRODEFINITIE */
/* ============== */
   #define TVG_VEVG 5            /* macrodefintie verhogen verlenggroenttijd  */
                                 /* direct voor einde verlenggroen            */
  

/* DEFINITIE GLOBALE VARIABELEN */
/* ============================ */
   mulv TVG_basis[FCMAX];        /* basis waarde - maximum verlenggroentijd   */
   mulv TVG_rgv[FCMAX];          /* RGV-waarde - maximum verlenggroentijd     */



/* DEFINITIE RGV-FUNCTIES */
/* ====================== */

/* rgv_verlenggroentijd() */
/* ---------------------- */
/* de functie rgv_verlenggroentijd() verzorgt op einde verlenggroen van een primaire
 * realisatie het ophogen en verlagen van de rgv - maximum verlenggroentijd op basis
 * van de gerealiseerde verlenggroenperiode.
 *
 * voorbeeld:
 * rgv_verlenggroentijd(fc01, 50, 300 ,50, 20, 40, 200, (bool) (BG[d011] || OG[d011]));
 * 
 */

void rgv_verlenggroentijd1(
        count fc,             /* fasecyclusnummer                            */
        mulv PRM_mintvg,      /* parameter minimum maximum verlenggroenttijd */
        mulv PRM_maxtvg,      /* parameter maximum maximum verlenggroenttijd */
        mulv PRM_tvgomhoog,   /* parameter verhoog maximum verlenggroenttijd */
        mulv PRM_tvgomlaag,   /* parameter verlaag maximum verlenggroenttijd */
        mulv PRM_tvgverschil, /* parameter grenswaarde voor verlagen         */
        mulv PRM_maxtvg_dd,   /* parameter maximum verlenggroenttijd bij DD  */
#ifdef TVG_VEVG
        bool SCH_schrgvwtvs,  /* schakelaar verhogen verlenggroenttijd       */
                              /* TVG_VEVG voor einde verlenggroen            */
#endif
        bool DD_fc,
        bool MK_speciaal)           /* status detectiestoring voor fasecyclus      */
{
   mulv tvg_verschil;     /* restant verlenggroentijd - tijdelijke variabele */
	if (DD_fc) 
	{                               /* detectiestoring voor fasecyclus          */
		TVG_rgv[fc]= PRM_maxtvg_dd;             /* maximum bij detectiestoring              */
    }
    else 
	{
#ifdef TVG_VEVG
		if ((EVG[fc] || !SCH_schrgvwtvs && (VG[fc] && ((TVG_rgv[fc]-TVG_timer[fc])<= TVG_VEVG))) 
			&& PR[fc] && (TVG_rgv[fc]>=0)) 
		{      /* einde verlenggroen en prim. realisatie   */
#else
		if (EVG[fc] && PR[fc] && (TVG_rgv[fc]>=0)) /* einde verlenggroen en prim. realisatie   */
		{  
#endif
			tvg_verschil= TVG_rgv[fc]-TVG_timer[fc];/* bepaal restant verlenggroentijd          */
#ifdef TVG_VEVG
			if ((tvg_verschil<=TVG_VEVG) && MK_speciaal) /* er is geen verlenggroentijd over         */
			{           
#else
            if ((tvg_verschil<=0) && MK_speciaal) /* er is geen verlenggroentijd over         */
			{               
#endif
				TVG_rgv[fc] += PRM_tvgomhoog;        /* verhoog maximum verlenggroentijd        */
				if (TVG_rgv[fc] > PRM_maxtvg)		/* groter dan maxmax verlenggroentijd?      */
				{      
					TVG_rgv[fc]= PRM_maxtvg;          /* maak gelijk aan maxmax verlenggroentijd  */
				}
			}
			else if ( (tvg_verschil>PRM_tvgverschil) /* er is veel verlenggroentijd over         */
						&& !FM[fc] && !Z[fc])		 /* en niet afgebroken en niet afgekapt      */
			{      
				TVG_rgv[fc] -= PRM_tvgomlaag;        /* verlaag maximum verlenggroentijd        */
				if (TVG_rgv[fc] < PRM_mintvg)		/* kleiner dan minimum verlenggroentijd?    */
				{									
					TVG_rgv[fc]= PRM_mintvg;          /* maak gelijk aan minimum verlenggroentijd */
				}
			}
		}
	}
}



/* rgv_verlenggroentijd2() */
/* ----------------------- */
/* de functie rgv_verlenggroentijd2() verzorgt op einde verlenggroen van een primaire
 * realisatie het ophogen en verlagen van de rgv - maximum verlenggroentijd op basis
 * van de gerealiseerde verlenggroenperiode.
 * functie rgv_verlenggroentijd2() is bedoeld voor richtingen met meerdere rijstroken.
 * deze functie heeft een extra argument (MK_speciaal) voor een speciaal meetkriterium
 * voor het verhogen van TVG_rgv[] boven de waarde van TVG_basis[].
 * het speciale meetkriterium moet daarvoor voor meerdere rijstroken nog actief zijn.
 *
 * voorbeeld:
 * rgv_verlenggroentijd(fc01, 50, 300 ,50, 20, 40, 200, (bool) (BG[d011] || OG[d011]), (bool) (H[mmk02sepc]));
 * 
 */

void rgv_verlenggroentijd2(
        count fc,             /* fasecyclusnummer                            */
        mulv PRM_mintvg,      /* parameter minimum maximum verlenggroenttijd */
        mulv PRM_maxtvg,      /* parameter maximum maximum verlenggroenttijd */
        mulv PRM_tvgomhoog,   /* parameter verhoog maximum verlenggroenttijd */
        mulv PRM_tvgomlaag,   /* parameter verlaag maximum verlenggroenttijd */
        mulv PRM_tvgverschil, /* parameter grenswaarde voor verlagen         */
        mulv PRM_maxtvg_dd,   /* parameter maximum verlenggroenttijd bij DD  */
#ifdef TVG_VEVG
        bool SCH_schrgvwtvs,  /* schakelaar verhogen verlenggroenttijd       */
                              /* TVG_VEVG voor einde verlenggroen            */
#endif
        bool DD_fc,           /* status detectiestoring voor fasecyclus      */
        bool MK_speciaal)     /* speciaal meetkriterium voor de              */
                              /* afzonderlijke rijstroken van de fasecyclus  */
{
   mulv tvg_verschil;   /* tijdelijke variabele */

	if (DD_fc) 
	{                               /* detectiestoring voor fasecyclus          */
		TVG_rgv[fc]= PRM_maxtvg_dd;             /* maximum bij detectiestoring              */
    }
	else
	{
#ifdef TVG_VEVG
		if ((EVG[fc] || !SCH_schrgvwtvs && (VG[fc] && ((TVG_rgv[fc]-TVG_timer[fc])<= TVG_VEVG))) 
			&& PR[fc] && (TVG_rgv[fc]>=0))
		{      /* einde verlenggroen en prim. realisatie   */
#else
   if (EVG[fc] && PR[fc] && (TVG_rgv[fc]>=0)) 
		{  /* einde verlenggroen en prim. realisatie   */
#endif
	     tvg_verschil= TVG_rgv[fc]-TVG_timer[fc];/* bepaal restant verlenggroentijd          */
#ifdef TVG_VEVG
         if ((tvg_verschil<=TVG_VEVG)            /* er is geen verlenggroentijd over         */
#else
         if ((tvg_verschil<=0)                   /* er is geen verlenggroentijd over         */
#endif
             && ((MK_speciaal                     /* meetkriterium afzondelijke rijstroken    */ 
                  || (TVG_rgv[fc]<TVG_basis[fc])) && MK[fc]))
			{   /* TVG_basis is nog niet bereikt  */
				TVG_rgv[fc] += PRM_tvgomhoog;        /* verhoog maximum verlenggroentijd         */
				if (TVG_rgv[fc] > PRM_maxtvg) 
				{      /* groter dan maxmax verlenggroentijd?      */
					TVG_rgv[fc]= PRM_maxtvg;          /* maak gelijk aan maxmax verlenggroentijd  */
				}
			}
			else if ( (tvg_verschil>PRM_tvgverschil)/* er is veel verlenggroentijd over         */
						&& !FM[fc] && !Z[fc]) 
			{      /* en niet afgebroken en niet afgekapt      */
				TVG_rgv[fc] -= PRM_tvgomlaag;        /* verlaag maximum verlenggroentijd         */
				if (TVG_rgv[fc] < PRM_mintvg) {      /* kleiner dan minimum verlenggroentijd?    */
					TVG_rgv[fc]= PRM_mintvg;          /* maak gelijk aan minimum verlenggroentijd */
            }
         }
      }
   }
}




/* rgv_verlenggroentijd_correctie_va_arg() */
/* --------------------------------------- */
/* de functie rgv_verlenggroentijd_correctie_va_arg() past de verlenggroentijden van een
 * maatgevende conflictgroep aan, indien de cyclustijd groter is dan de maximum opgegeven
 * cyclustijd. 
 * de functie maakt gebruik van een variabele argumentenreeks. het eerste argument is de
 * maximum gewenste cyclustijd daarna volgende fasecyclusnummers van de maatgevende
 * conflicten in volgorde van realisatie.
 * de functie moet worden afgesloten met het argument END (-3).
 * de functie rgv_verlenggroentijd_correctie_va_arg() geeft als return-waarde de 
 * berekende cyclustijd van de maatgvende conflictgroep. 
 *
 * voorbeeld: 
 * rgv_verlenggroentijd_correctie_va_arg(1200, fc02, fc09, fc12, END);
 *
 */

mulv rgv_verlenggroentijd_correctie_va_arg(va_mulv PRM_rgv, va_mulv DD_anyfc, va_mulv PRM_tcmin, va_mulv PRM_tcmax, ...)
{
   #define MAX_KGROEP 10    /* maximum aantal fasecycli in de conflictgroep  */
   #define TVG_STEP  10     /* stapgrootte voor verlagen in tienden seconden */

   va_list argpt;          /* variabele argumentenlijst     */
   count fcnr[MAX_KGROEP]; /* arraynummers fasecycli        */
   register count i, j;    /* tellers                       */
   count lastnr;           /* laatste arraynr fasecyclus    */
   mulv TFG_som;           /* som van de vastgroentijden    */
   mulv TVG_rgv_som;       /* som van de verlenggroentijden */
   mulv TVG_basis_som;     /* som van de verlenggroentijden */
   mulv TGL_som;           /* som van de geeltijden         */
   mulv TO_som;            /* som van de ontruimingstijden  */
   mulv TC_rgv_max;        /* totale rgv cyclustijd         */
   mulv TC_basis_max;      /* totale basis cyclustijd       */
/* mulv TVG_basis[FCMAX]; */     /* basis waarde van de verlenggroentijd               */
/* mulv TVG_rgv[FCMAX];   */     /* rgv (actuele) waarde van de verlenggroentijd       */
   mulv TVG_absoluut[FCMAX];     /* abolute verschil verlenggroentijd actueel en basis */
   mulv TVG_procentueel[FCMAX];  /* procentueel verlenggroentijd actueel en basis      */
   mulv fcnr_temp;               /* tijdelijke fcnr t.b.v. swap fcnr's                 */
  

   /* lees de arraynummers van de fasecycli en plaats ze in de array fcnr[] */
   /* --------------------------------------------------------------------- */
   i= 0;
   va_start(argpt, PRM_tcmax);          /* start variabele argumentenlijst           */

   do {
      fcnr[i]= va_arg(argpt, va_count); /* lees arraynummers fasecycli van conflictgroep */
      lastnr= i;
   } while ((fcnr[i++]>=0) && (i<MAX_KGROEP));
   lastnr= i-2;                         /* bepaal laatste argrument   */

   /* bereken som van de vastgroen-, verlenggroen- en geeltijden */
   /* ---------------------------------------------------------- */
   j= 0;
   TFG_som=TVG_basis_som=TVG_rgv_som=TGL_som= 0;   /* zet sommaties op 0         */

   while (fcnr[j]>=0) {
      i= fcnr[j]; 
      TFG_som += TFG_max[i];           /* sommeer vastgroentijden    */
      if (TVG_rgv[i]>0)
         TVG_rgv_som += TVG_rgv[i];    /* sommeer verlenggroentijden */
      if (TVG_basis[i]>0)
         TVG_basis_som += TVG_basis[i];/* sommeer verlenggroentijden */
      TGL_som += TGL_max[i];           /* sommeer geeltijden         */
      j++;
   }
   
   /* bereken som van de ontruimingstijden */
   /* ------------------------------------ */
   j= 1;
   if (TO_max[fcnr[lastnr]][fcnr[0]]<0)
   {
      TO_som= TO_ontwerp[fcnr[lastnr]][fcnr[0]];  /* van laatste naar eerste fasecyclus */
   }
   else
   {
      TO_som= TO_max[fcnr[lastnr]][fcnr[0]];      /* van laatste naar eerste fasecyclus */
   }
   while (fcnr[j]>=0)
   {
      if (TO_max[fcnr[j-1]][fcnr[j]]<0)
      {
         TO_som += TO_ontwerp[fcnr[j-1]][fcnr[j]]; /* naar volgende fasecyclus           */
      }
      else
      {
         TO_som += TO_max[fcnr[j-1]][fcnr[j]];     /* naar volgende fasecyclus           */
      }
      j++;
   }

   /* bereken de totale cyclustijd */
   /* ---------------------------- */
   TC_rgv_max= TFG_som + TVG_rgv_som + TGL_som + TO_som;     /* bereken totale cyclustijd     */
   TC_basis_max= TFG_som + TVG_basis_som + TGL_som + TO_som; /* bereken totale cyclustijd     */

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined (VISSIM)
   /* display resultaten in xyprintf-scherm */
   /* ------------------------------------- */
//   xyprintf (0, 15,"TC_basis_max=  %d   ", TC_basis_max);
//   xyprintf (0, 16,"TFG_som= %d  TVG_som= %d TGL_som= %d TO_som=%d lastnr= %d",
//                    TFG_som, TVG_rgv_som, TGL_som, TO_som, lastnr);
#endif

   /* correctie maximum verlenggroentijden bij een te hoge cyclustijd */
   /* --------------------------------------------------------------- */
   if (((TC_rgv_max > PRM_tcmax) || (TC_rgv_max > TC_max) && (PRM_rgv==1) || (TC_rgv_max > TC_basis_max) && (PRM_rgv==2)) && !((TC_rgv_max < PRM_tcmin) && (DD_anyfc==0))) 
            {  /* test of de berekende cyclustijd te hoog is */
      
      /* bereken verlenggroentijden - absolute verschil */
      /* ---------------------------------------------- */
      j=0;
      while (fcnr[j]>=0) {
         i= fcnr[j]; 
         if (TVG_basis[i]>=0) {
            TVG_absoluut[i]= TVG_rgv[i]-TVG_basis[i];
         }
         else {
            TVG_absoluut[i]= 0;
         }
         j++;
      }

      /* bereken verlenggroentijden - procentuele verschil */
      /* ------------------------------------------------- */
      j=0;
      while (fcnr[j]>=0) {
         i= fcnr[j]; 
         if (TVG_basis[i]>0) {
            TVG_procentueel[i]= ((long_mulv) (TVG_rgv[i]) * 100L)/TVG_basis[i];
         }
         else {
            TVG_procentueel[i]= 0;
         }
         j++;
      }

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined (VISSIM)
      /* display resultaten in xyprintf-scherm */
      /* ------------------------------------- */
      j=0;
      while (fcnr[j]>=0) {
          i= fcnr[j]; 
//          xyprintf (0, 25+j,"fc= %s TVG_rgv=%4d  TVG_basis=%4d  TVG_abs=%4d  TVG_proc=%3d ",
//                    FC_code[i], TVG_rgv[i], TVG_basis[i], TVG_absoluut[i], TVG_procentueel[i]);
          j++;
      }
#endif
      /* correctie verlenggroentijden t.o.v. maximum gewenste cyclustijd */
      /* --------------------------------------------------------------- */
      while (((TC_rgv_max > PRM_tcmax) || (TC_rgv_max > TC_max)  && (PRM_rgv==1)|| (TC_rgv_max > TC_basis_max) && (PRM_rgv==2))  && !((TC_rgv_max < PRM_tcmin) && (DD_anyfc==0))){ 

         /* sorteer de fasecyclusnummers op grootste verlenggroentijd correctie */
         /* ------------------------------------------------------------------- */
         for (j= 1; j<=lastnr; j++) { 
            for (i= j; i<=lastnr; i++) {
/*             if (TVG_absoluut[fcnr[i-1]] < TVG_absoluut[fcnr[i]] ) { */  /* abs. volgorde  */
               if (TVG_procentueel[fcnr[j-1]] < TVG_procentueel[fcnr[i]]) {/* proc. volgorde */
                  fcnr_temp= fcnr[j-1];      /* swap fcnr's  */
                  fcnr[j-1]= fcnr[i];       
                  fcnr[i]= fcnr_temp;
               }
            }
         }

         /* verlaag de verlenggroentijd van de fasecyclus zolang de hoogste waarde */
         /* ---------------------------------------------------------------------- */
         while (( (TVG_rgv[fcnr[0]]>= TVG_STEP) && ((TC_rgv_max > PRM_tcmax) || (TC_rgv_max > TC_max)  && (PRM_rgv==1)|| (TC_rgv_max > TC_basis_max) && (PRM_rgv==2)) && !((TC_rgv_max < PRM_tcmin) && (DD_anyfc==0)))
                 && (TVG_procentueel[fcnr[0]] >= TVG_procentueel[fcnr[1]]) ) {

            /* verlaag de verlenggroentijd en de cyclustijd */
            /* -------------------------------------------- */
            TVG_rgv[fcnr[0]] -= TVG_STEP; 
            TC_rgv_max -= TVG_STEP;        

            /* bereken nieuw absoluut verschil */
            /* ------------------------------- */
            if (TVG_basis[fcnr[0]]>=0) { 
               TVG_absoluut[fcnr[0]]= TVG_rgv[fcnr[0]]-TVG_basis[fcnr[0]];
            }
            else {
               TVG_absoluut[0]= 0;
            }

            /* bereken nieuw procentueel verschil */
            /* ---------------------------------- */
            if (TVG_basis[fcnr[0]]>0) {
               TVG_procentueel[fcnr[0]]=
                  ((long_mulv) (TVG_rgv[fcnr[0]]) * 100L) / TVG_basis[fcnr[0]];
            }
            else {
               TVG_procentueel[0]= 0;
            }
         }
          
         /* stop de while-loop indien de verlenggroentijd niet langer kan worden verlaagd */
         /* ----------------------------------------------------------------------------- */
         if ( TVG_rgv[fcnr[0]] < TVG_STEP) {
            break;   /* verlenggroentijd kan niet langer worden verlaagd */
         }
      }
  
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined (VISSIM)
      /* display resultaten in xyprintf-scherm */
      /* ------------------------------------- */
      j=0;
      while (fcnr[j]>=0) {
          i= fcnr[j]; 
//         xyprintf (0, 30+j,"fc= %s TVG_rgv=%4d  TVG_basis=%4d  TVG_abs=%4d  TVG_proc=%3d ",
//                    FC_code[i], TVG_rgv[i], TVG_basis[i], TVG_absoluut[i], TVG_procentueel[i]);
          j++;
      }
#endif
   }

   /* maak variabele argumentenlijst leeg */
   /* ----------------------------------- */
   va_end(argpt);           /* maak var. arg-lijst leeg  */

   return TC_rgv_max;
}



/* berekencyclustijd_va_arg() */
/* -------------------------- */
/* de functie berekencyclustijd_va_arg() berekent de cyclustijd van een maatgevende 
 * conflictgroep. 
 * de functie maakt gebruik van een variabele argumentenreeks. het eerste argument is 
 * het fasecyclusnummer van de erste fasecyclus daarna volgen de overige fasecyclusnummers
 * van de maatgevende conflicten in volgorde van realisatie.
 * de functie moet worden afgesloten met het argument END (-3).
 *
 * voorbeeld: 
 * berekencyclustijd_va_arg(fc02, fc09, fc12, END);
 *
 */

mulv berekencyclustijd_va_arg(va_count fcnr_first, ...)
{
   #define MAX_KGROEP 10    /* maximum aantal fasecycli in de conflictgroep  */

   va_list argpt;          /* variabele argumentenlijst     */
   count fcnr[MAX_KGROEP]; /* arraynummers fasecycli        */
   register count i, j;    /* tellers                       */
   count lastnr;           /* laatste arraynr fasecyclus    */
   mulv TFG_som;           /* som van de vastgroentijden    */
   mulv TVG_som;           /* som van de verlenggroentijden  */
   mulv TGL_som;           /* som van de geeltijden         */
   mulv TO_som;            /* som van de ontruimingstijden  */
   mulv TC_temp;           /* totale cyclustijd             */
  

   /* lees de arraynummers van de fasecycli en plaats ze in de array fcnr[] */
   /* --------------------------------------------------------------------- */
   i= 0;
   va_start(argpt, fcnr_first);         /* start variabele argumentenlijst           */
   fcnr[i]= fcnr_first;                 /* eerste fasecyclusnummer van de conflictgroep  */
   i++;

   do {
      fcnr[i]= va_arg(argpt, va_count); /* lees arraynummers fasecycli van conflictgroep */
      lastnr= i;
   } while ((fcnr[i++]>=0) && (i<MAX_KGROEP));
   lastnr= i-2;                         /* bepaal laatste argument   */

   /* bereken som van de vastgroen-, verlenggroen- en geeltijden */
   /* ---------------------------------------------------------- */
   j= 0;
   TFG_som=TVG_som=TGL_som= 0;   /* zet sommaties op 0         */

   while (fcnr[j]>=0) {
      i= fcnr[j]; 
      TFG_som += TFG_max[i];     /* sommeer vastgroentijden    */
      if (TVG_max[i]>0)
         TVG_som += TVG_max[i];  /* sommeer verlenggroentijden */
      TGL_som += TGL_max[i];     /* sommeer geeltijden         */
      j++;
   }
   
   /* bereken som van de ontruimingstijden */
   /* ------------------------------------ */
   j= 1;
   if (TO_max[fcnr[lastnr]][fcnr[0]] <0)
   { 
      TO_som= TO_ontwerp[fcnr[lastnr]][fcnr[0]];   /* van laatste naar eerste fasecyclus */
   }
   else
   {
      TO_som= TO_max[fcnr[lastnr]][fcnr[0]];   /* van laatste naar eerste fasecyclus */
   }

   while (fcnr[j]>=0)
   {
      if (TO_max[fcnr[j-1]][fcnr[j]]<0)
      {
     TO_som += TO_ontwerp[fcnr[j-1]][fcnr[j]]; /* naar volgende fasecyclus           */
      }
      else
      {
         TO_som += TO_max[fcnr[j-1]][fcnr[j]];     /* naar volgende fasecyclus           */
      }
      j++;
   }

   /* bereken de totale cyclustijd */
   /* ---------------------------- */
   TC_temp= TFG_som + TVG_som + TGL_som + TO_som; /* bereken totale cyclustijd     */


   /* maak variabele argumentenlijst leeg */
   /* ----------------------------------- */
   va_end(argpt);           /* maak var. arg-lijst leeg  */

   return (mulv) (TC_temp);
}




/* kopieer de verlenggroentijd TVG_rgv[] naar TVG_max[] */
/* ---------------------------------------------------- */

void copy_TVG_rgv_to_TVG_max (void)
{
   register count i;

   for (i=0; i<FC_MAX; i++) {
      TVG_max[i] =TVG_rgv[i];
   }
}
   

/* kopieer de verlenggroentijd TVG_max[] naar TVG_basis[] */
/* ------------------------------------------------------ */

void copy_TVG_max_to_TVG_basis (void)
{
   register count i;
   static mulv init= FALSE;

   if (!init) {
      for (i=0; i<FC_MAX; i++) {
         TVG_rgv[i] =TVG_max[i];
         init= TRUE;
      }
   }
 
   for (i=0; i<FC_MAX; i++) {
       TVG_basis[i]= TVG_max[i];
   }
}


/* kopieer de groentijd TVG_max[] naar TVG_rgv[] */
/* --------------------------------------------- */

/* void copy_TVG_max_to_TVG_rgv (void)
 * {
 *    register count i;
 *
 *    for (i=0; i<FC_MAX; i++) {
 *       TVG_rgv[i] =TVG_max[i];
 *    }
 * }



/* LET OP! */
/* ------- */

/* Nog geen rekening gehouden met:
 * - groen/groen conflicten
 * - gekoppelde fasecycli
 *
 * functie void rgv_verlenggroen_corrrectie_va_arg(va_mulv PRM_tcmax, ...)
 * zou ook op totale som van verlenggroentijden i.p.v. de cyclustijd kunnen werken 
 * omdat de geel- vastgroen en ontruimingstijden gelijk blijven (tenzij een parameter
 * wijziging is gedaan)
 *
 * functie maken om actuele cyclustijd te tonen.
 */



