/* WACHTTIJD-LANTAARN MET LEDS */
/* =========================== */


/* (C) Copyright 2003-2019 by A.C.M. van Grinsven. All rights reserved.	*/


/* CCOL :  versie 10.0   */
/* FILE :  wtlled.c     */
/* DATUM:  17-04-2019   */



/* include files */
/* ============= */
   #include "sysdef.c"		/* definitie typen variabelen		*/
   #include "cif.inc"           /* declaratie CVN C-interface           */
   #include "fcvar.h"           /* declaratie fasecyclusvariabelen      */
   #include "tmvar.h"           /* declaratie tijdvariabelen            */
   #include "mevar.h"           /* declaratie geheugenvariabelen        */



/* macrodefinities */
/* =============== */
#ifndef WTL_MAX_LEDS
   #define WTL_MAX_LEDS       31  /* maximum aan te sturen tijdleds                */
#endif
   #define WTL_TIJD_MAX_LEDS  60  /* ledtijd (TE) bij negatieve waarde (PG[]/OS[]) */



/* definitie wachttijdfuncties */
/* =========================== */

/* de functie wachttijd_leds() verzorgt de aansturing van de leds van de wachttijd-
 * lantaarn voor de opgegeven fasecyclus (fc) op basis van de berekende wachttijd
 * (T_wacht_ber). de functie wachttijd_leds() geeft het aantal leds dat moet branden
 * aan de procesbesturing door m.b.v. een uitgangssignaal (CIF_GUS[uswvt]).
 * een wachttijd-lantaarn wordt aangestuurd, indien de opgegeven fasecyclus in
 * rood verkeert en een aanvraag heeft. de wachttijd-lantaarn wordt niet aangestuurd
 * indien de fasecyclus alleen een mee-aanvraag speciaal heeft (BIT5), omdat deze
 * mee-aanvraag kan worden teruggezet.
 * de functie gaat uit van een wachttijdlantaarn met 31 wacht-leds en de tekst 'WACHT.
 * de functie verdeelt de berekende wachttijd evenredig over de nog brandende leds.
 * het volgende led dooft indien de actuele waarde van de dooftijd (T_timer[twtv])
 * groter of gelijk is aan de berekende wachttijd gedeeld door het aantal nog brandende
 * leds. bij het doven van een led herstart de dooftijd.
 * de functie wachttijd_leds()geeft als return-waarde het aantal leds dat moet branden.
 */

mulv wachttijd_leds(count fc, count uswtv, count twtv, mulv T_wacht_ber, mulv T_led_start)
{
   /* fc    - fasecyclusnummer                            */
   /* uswtv - uitgangsnummer wachttijdlantaarn            */
   /* twtv  - tijdelementnummer, dooftijd voor een led    */
   /* T_wacht_ber - waarde van de berekende wachttijd     */


/* #define T_LED_START       2    * 2 tiendenseconden                              */


   mulv aantal_leds;		 /* aantal brandende tijd-leds	                   */

   RT[twtv]= TRUE;               /* set instructievariabele led-tijdmeting         */

   /* actueel brandende wachttijd-leds */
   /* -------------------------------- */
   aantal_leds= CIF_GUS[uswtv];  /* actueel aantal brandende tijd-leds	           */

   if (aantal_leds>WTL_MAX_LEDS) {
      aantal_leds= WTL_MAX_LEDS;  
   }


   /* aantal wachttijd-leds */
   /* --------------------- */
   if (R[fc] && (A[fc] & (BIT0+BIT3))) /* rood en detectieaanvraag of starre aanvraag    */
   {     
      if (aantal_leds==0)		/* start aansturing wachttijd-lantaarn     */
      {
         if (T_wacht_ber>=0)
         {
            if (T_led_start>0) {
               aantal_leds= T_wacht_ber/T_led_start;
            }
         }
         else   /* negatieve waarde - b.v. PG[ ] staat op */
         {
            aantal_leds= WTL_MAX_LEDS;
         }
      }
      else                              /* tijdens aansturing */
      {
         if (T_wacht_ber>=0) 
         {
            if (T_timer[twtv]>=((T_wacht_ber+T_timer[twtv])/aantal_leds))
            {
               aantal_leds--; 
/*             RT[twtv]= TRUE;  */
            }
            else
            {
               RT[twtv]= FALSE; 
            }
         }
         else {      /* negatieve waarde - b.v. PG[ ] staat op */
            if (T_timer[twtv]>=WTL_TIJD_MAX_LEDS)
            {
               aantal_leds--; 
 /*            RT[twtv]= TRUE;   */
            }
            else
            {
               RT[twtv]= FALSE; 
            }
         }
      }
   }
   
   /* beveiliging bereik */
   /* ------------------ */
   if (aantal_leds<0)  aantal_leds= 0;
   if (aantal_leds>WTL_MAX_LEDS)  aantal_leds= WTL_MAX_LEDS;

   /* beveiliging groensturing */
   /* ------------------------ */
   if (G[fc])  aantal_leds= 0;
   else if (R[fc] && A[fc] && !aantal_leds && CIF_GUS[uswtv])  aantal_leds= 1;
   
   /* uitsturing wachttijdvoorspeller */
   /* ------------------------------- */
   CIF_GUS[uswtv]= aantal_leds;


   return (mulv) aantal_leds;
}



/* de functie wachttijd_leds_mm() werkt in principe hetzelfde als de functie wachttijd_leds().
 * het aantal nog brandende leds wordt nu naar een geheugenelement geschreven i.p.v. naar een uitgangssignaal.
 * de functie wachttijd_leds()geeft als return-waarde het aantal leds dat moet branden.
 */

mulv wachttijd_leds_mm(count fc, count mmwtv, count twtv, mulv T_wacht_ber, mulv T_led_start)
{
   /* fc    - fasecyclusnummer                            */
   /* mmwtv - uitgangsnummer wachttijdlantaarn            */
   /* twtv  - tijdelementnummer, dooftijd voor een led    */
   /* T_wacht_ber - waarde van de berekende wachttijd     */


/* #define T_LED_START        2   * 2 tiendenseconden                              */

   mulv aantal_leds;		 /* aantal brandende tijd-leds	                   */


   RT[twtv]= TRUE;               /* set instructievariabele led-tijdmeting         */


   /* actueel brandende wachttijd-leds */
   /* -------------------------------- */
   aantal_leds= MM[mmwtv];  /* actueel aantal brandende tijd-leds	           */

   if (aantal_leds>WTL_MAX_LEDS) {
      aantal_leds= WTL_MAX_LEDS;  
   }

   /* aantal wachttijd-leds */
   /* --------------------- */
   if (R[fc] && (A[fc] & (BIT0+BIT3))) /* rood en aanvraag en geen mee-aanvraag speciaal */
   {     
      if (aantal_leds==0)		/* start aansturing wachttijd-lantaarn     */
      {
         if (T_wacht_ber>=0)
         {
            if (T_led_start>0) {
               aantal_leds= T_wacht_ber/T_led_start;
            }
         }
         else   /* negatieve waarde - b.v. PG[ ] staat op */
         {
            aantal_leds= WTL_MAX_LEDS;
         }
      }
      else                              /* tijdens aansturing */
      {
         if (T_wacht_ber>=0) 
         {
            if (T_timer[twtv]>=((T_wacht_ber+T_timer[twtv])/aantal_leds))
            {
               aantal_leds--; 
/*             RT[twtv]= TRUE;  */
            }
            else
            {
               RT[twtv]= FALSE; 
            }
         }
         else {      /* negatieve waarde - b.v. PG[ ] staat op */
            if (T_timer[twtv]>=WTL_TIJD_MAX_LEDS)
            {
               aantal_leds--; 
 /*            RT[twtv]= TRUE;   */
            }
            else
            {
               RT[twtv]= FALSE; 
            }
         }
      }
   }
   
   /* beveiliging bereik */
   /* ------------------ */
   if (aantal_leds<0)  aantal_leds= 0;
   if (aantal_leds>WTL_MAX_LEDS)  aantal_leds= WTL_MAX_LEDS;

   /* beveiliging groensturing */
   /* ------------------------ */
   if (G[fc])  aantal_leds= 0;
   else if (R[fc] && A[fc] && !aantal_leds && MM[mmwtv])  aantal_leds= 1;
   
   /* uitsturing wachttijdvoorspeller */
   /* ------------------------------- */
   MM[mmwtv]= aantal_leds;


   return (mulv) aantal_leds;
}

