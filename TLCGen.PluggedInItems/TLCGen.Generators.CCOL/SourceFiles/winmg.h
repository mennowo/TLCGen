/*
*
*/

#ifndef _ITT_WIN_MG_H_
#define _ITT_WIN_MG_H_

#include <windows.h>

#include "sysdef.c"

/*
* Voorbeeld: MG_Bars_init(TVG_basis, TVG_rgv, 10, 400, 0, 0);
*/
void MG_Bars_init(mulv* basis, mulv* actueel, mulv _bar_width, mulv height, mulv x, mulv y);

/*
* Voorbeeld: MG_Fasen_Venster_init(SYSTEM, fc01, fc05, fc11, END);
* Voorbeeld: MG_Fasen_Venster_init(SYSTEM, AUTO_FC);
*/
void MG_Fasen_Venster_init(LPCTSTR lpWindowName, ...);

/*
* ...   : lijst te tonen fasecycli, afsluiten met END
*/
void MG_Bars();

#endif
