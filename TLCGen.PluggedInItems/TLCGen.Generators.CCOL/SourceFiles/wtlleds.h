/* WACHTTIJD-LANTAARN MET LEDS */
/* =========================== */


/* (C) Copyright 2003-2019 by A.C.M. van Grinsven. All rights reserved.	*/


/* CCOL :  versie 10.0   */
/* FILE :  wtlled.h     */
/* DATUM:  17-04-2019   */



#ifndef __WTLLEDS_H
#define __WTLLEDS_H


/* include files */
/* ============= */
   #include "sysdef.c"		/* definitie typen variabelen		*/



/* declaratie functies */
/* =================== */
   mulv wachttijd_leds(count fc, count uswtv, count twtv, mulv T_wacht_ber, mulv T_led_start);
   mulv wachttijd_leds_mm(count fc, count mmwtv, count twtv, mulv T_wacht_ber, mulv T_led_start);


#endif   /* __WTLLEDS_H */

