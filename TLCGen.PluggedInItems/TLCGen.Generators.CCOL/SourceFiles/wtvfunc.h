/* WACHTTIJD-VOORSPELLER */
/* ===================== */


/* (C) Copyright 2003-2019 by A.C.M. van Grinsven. All rights reserved.	*/


/* CCOL :  versie 10.0	*/
/* FILE :  wtcfunc.h	*/
/* DATUM:  17-04-2019	*/



#ifndef __WTLFUNC_H
#define __WTLFUNC_H


/* include files */
/* ============= */
   #include "sysdef.c"		 /* definitie typen variabelen		*/


                               
/* declaratie correctie functies */
/* ============================= */  
#ifdef TWACHT_EXTRA
   extern mulv tg_extra[];
#endif

     

/* declaratie wachttijdfuncties */
/* ============================ */
   mulv max_wachttijd_conflicten(count i);
   void max_wachttijd_modulen_primair(boolv *prml[], count ml, count ml_max, mulv twacht[]);
   void max_wachttijd_modulen_primair2(boolv *prml[], count ml, count ml_max, mulv twacht[], mulv t_wacht[]);
   mulv max_wachttijd_alternatief(count fc, mulv twacht[]);
   void wachttijd_correctie_gelijkstart(count fc1,count fc2, mulv t_wacht[]);
   void rr_modulen_primair(boolv *prml[], count ml, count ml_max, mulv rr_twacht[]);
   
   void max_wachttijd_modulen_primair_correctie(count ml);


   #endif   /* __WTLFUNC_H */
