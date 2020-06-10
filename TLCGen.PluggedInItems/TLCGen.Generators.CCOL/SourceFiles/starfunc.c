#include "starfunc.h"

static count star_cyclustimer;

static void update_cyclustimer(count cyclustijd)
{	
	star_cyclustimer -= 1;
	star_cyclustimer += TS;
	star_cyclustimer = star_cyclustimer % cyclustijd + 1;
}

boolv periode(count	cyclustijd, count cyclustimer, count begin_groen, count einde_groen)
{
	count einde = einde_groen;
	count ctk = cyclustimer;

	if (begin_groen > einde_groen)
	{
		einde += cyclustijd;
		if (cyclustimer < begin_groen)
		{
			ctk += cyclustijd;
		}
	}

	return ctk >= begin_groen && ctk < einde;
}

void commando_groen(count fc)
{
	B[fc] = (boolv)(R[fc] && A[fc]);
	YM[fc] |= BIT14;
	RR[fc] = FALSE;
}

void star_reset_bits(boolv star)
{
	int i;

	if (star)
	{
		for (i = 0; i < FCMAX; ++i)
		{
			X[i] = FALSE;
			RR[i] = FALSE;
			RS[i] = FALSE;
			RW[i] = FALSE;
			Z[i] = FALSE;
			FW[i] = FALSE;
			FM[i] = FALSE;
			YW[i] = FALSE;
			YV[i] = FALSE;
			YM[i] = FALSE;
			B[i] = FALSE;
			MK[i] = FALSE;
			RR[i] |= BIT14;
			A[i] = TRUE;
		}

		for (i = 0; i < TMMAX; ++i)
		{
			RT[i] = FALSE;
			HT[i] = FALSE;
			AT[i] = FALSE;
		}
	}
	else
	{
		for (i = 0; i < FCMAX; ++i)
		{
			B[i] = FALSE;
			YM[i] &= ~BIT14;
			RR[i] &= ~BIT14;
		}
	}
}

void star_regelen()
{
	int fc;
	int p = star_programma - 1;

	if (p < 0 || p >= STARMAX) p = 0;

	update_cyclustimer(STAR_ctijd[p]);

	for (fc = 0; fc < FCMAX; ++fc)
	{
		if (periode(STAR_ctijd[p], star_cyclustimer, STAR_start1[p][fc], STAR_eind1[p][fc])) commando_groen(fc);
		if (STAR_start2[fc] != 0 && STAR_eind2[fc] != 0)
		{
			if (periode(STAR_ctijd[p], star_cyclustimer, STAR_start2[p][fc], STAR_eind2[p][fc])) commando_groen(fc);
		}
	}
}