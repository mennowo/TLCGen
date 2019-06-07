/* in geval ccol versie lager dan 10: */
/* bool definieren                    */
/* in geval ccol 10 of hoger:         */
/* stringv definieren                 */

#ifndef __BOOLTYPE__
  #define __BOOLTYPE__
  #ifndef BIT0
    #include "sysdef.c"
  #endif
  #if !defined CCOL_V || defined CCOL_V && CCOL_V < 100
    typedef bool boolv;
    typedef va_bool va_boolv;
    typedef string stringv;
  #else
    typedef char *stringv; 	/* pointer naar een string		*/
  #endif
#endif
