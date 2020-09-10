#ifndef EXTRA_FUNC_RIS
#define EXTRA_FUNC_RIS

#include <stdio.h>
#include <stdlib.h>

#include "prio.h"

#if (CCOL_V >= 110)

	rif_bool ris_inmelding_selectief(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_bool match_signalgroup);
	rif_bool ris_uitmelding_selectief(count fc);
	void ris_verstuur_ssm(int prioFcsrm);

#endif

void ris_ym(int prioFcsrm, count tym, count tym_max);

#endif // EXTRA_FUNC_RIS