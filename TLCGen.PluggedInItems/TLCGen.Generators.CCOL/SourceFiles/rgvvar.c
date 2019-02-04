/* DEFINITIE FUNCTIES - ROBUUSTE GROENTIJD VERDELER */
/* ================================================ */


/* (C) Copyright 2006-2012 by A.C.M. van Grinsven. All rights reserved. */


/* CCOL:  versie 8.0   */
/* FILE:  rgvvar.c     */
/* DATE:  15-08-2012   */



/* include files */
/* ============= */
   #include <stdarg.h>	   /* gebruik variabele argumentenlijst   */
   #include "sysdef.c"	   /* definitie typen variabelen          */
  


/* declaratie globale variabelen */
/* ============================= */
   mulv TVG_basis[FCMAX];          /* basis waarde - maximum verlenggroentijd            */
   mulv TVG_rgv[FCMAX];            /* RGV-waarde - maximum verlenggroentijd              */
   mulv TVG_absoluut[FCMAX];       /* abolute verschil verlenggroentijd actueel en basis */
   mulv TVG_procentueel[FCMAX];    /* procentueel verlenggroentijd actueel en basis      */

#ifdef TO_ONTWERPTIJD
   mulv TO_ontwerp[FCMAX][FCMAX];  
#endif



/* declaratie globale functies */
/* =========================== */
#ifdef TVG_VEVG
   void rgv_verlenggroentijd( count fc, mulv PRM_mintvg, mulv PRM_maxtvg, mulv PRM_tvgomhoog, mulv PRM_tvgomlaag,
                              mulv PRM_tvgverschil, mulv PRM_maxtvg_dd, bool SCH_schrgvwtvs, bool DD_fc);

   void rgv_verlenggroentijd2(count fc, mulv PRM_mintvg, mulv PRM_maxtvg, mulv PRM_tvgomhoog, mulv PRM_tvgomlaag,
                              mulv PRM_tvgverschil, mulv PRM_maxtvg_dd, bool SCH_schrgvwtvs, bool DD_fc, bool MK_speciaal);
#else
   void rgv_verlenggroentijd( count fc, mulv PRM_mintvg, mulv PRM_maxtvg, mulv PRM_tvgomhoog, mulv PRM_tvgomlaag,
                              mulv PRM_tvgverschil, mulv PRM_maxtvg_dd, /* bool SCH_schrgvwtvs, */ bool DD_fc);
   void rgv_verlenggroentijd2(count fc, mulv PRM_mintvg, mulv PRM_maxtvg, mulv PRM_tvgomhoog, mulv PRM_tvgomlaag,
                              mulv PRM_tvgverschil, mulv PRM_maxtvg_dd, /* bool SCH_schrgvwtvs, */ bool DD_fc, bool MK_speciaal);
#endif

   bool rgv_niet_primair (count fc, count ml_max, bool *prml[], count ml, bool sml, count hpri, mulv PRM_mintvg, mulv PRM_tvgomlaag);
   mulv rgv_verlenggroentijd_correctie_va_arg(va_mulv PRM_rgv, va_mulv PRM_tcmax, ...);
   mulv berekencyclustijd_va_arg(va_count fcnr_first, ...);
   void copy_TVG_max_to_TVG_basis (void);
   void copy_TVG_rgv_to_TVG_max (void);


