/* in geval ccol versie lager dan 10 */
/* bool definieren                   */

#ifndef __BOOLTYPE__
  #define __BOOLTYPE__
  #ifndef BIT0
    #include "sysdef.c"
  #endif
  #if !defined CCOL_V || defined CCOL_V && CCOL_V < 100
    typedef bool boolv;
    typedef va_bool va_boolv;
  #endif
#endif

