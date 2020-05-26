#ifndef EXTRA_FUNC_RIS
#define EXTRA_FUNC_RIS

#include <stdio.h>
#include <stdlib.h>

#include "prio.h"

#ifdef RIS_SSM
void ris_verstuur_ssm(int prioFcsrm);
#endif
void ris_ym(int prioFcsrm, count tym, count tym_max);

#endif // EXTRA_FUNC_RIS