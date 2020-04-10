count star_cyclustimer;

void update_cyclustimer(count cyclustijd)
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
	YM[fc] = TRUE;
	RR[fc] = FALSE;
}

void star_reset_bits()
{
	int i;
	
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
		RR[i] = TRUE;
		A[i] = TRUE;
	}

	for (i = 0; i < TMMAX; ++i)
	{
		RT[i] = FALSE;
		HT[i] = FALSE;
		AT[i] = FALSE;
	}
}
