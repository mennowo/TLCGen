#include "halfstar_wtv.h"

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM
   #include "xyprintf.h"/* voor debug infowindow                                          */
   #include <stdio.h>      /* declaration printf()       */
#endif

/* Functie voor berekenen van maximale wachttijden tijdens halfstar regelen.
   De functie wordt normaliter gebruikt bij toepassing van wachttijdvoorspellers */
void max_wachttijd_halfstar(mulv twacht[], /* wachttijd (in tienden van seconden) */
                            count h_plact, /* PL regelen actief                   */
                            count pl)      /* actieve plan                        */
{
	count fc;

	if (!H[h_plact]) return;

	/* Basis-wachttijd: Tijd tot TXB-moment. */
	if (SH[h_plact])
	{
		for (fc = 0; fc < FCMAX; ++fc)
		{
			twacht[fc] = NG;
		}
	}

	for (fc = 0; fc < FCMAX; ++fc)
	{
		twacht[fc] = TOTXB_PL[fc];

		/* als de richting al (versneld primair) is gerealiseerd of als deze een OS heeft gekregen,
		   dan de max. cyclustijd bij wachttijd optellen */
		   /* Op start TXD moment wordt PG gereset. Op TXD + 1 wordt de waarde van TOTXB_PL weer gevuld.
			  Om te voorkomen dat in die ene seconde de wachttijd 0 wordt en de leds snel aflopen, moet worden
			  gewacht totdat TOTXB_PL weer wordt gevuld */
		if (PG[fc] || !TOTXB_PL[fc])
			twacht[fc] += 10 * TX_max[pl]; /* twacht in hele seconden! */

		if (RA[fc] && !RR[fc] && !BL[fc])
		{
			twacht[fc] = max_wachttijd_conflicten(fc);
		}
		if (G[fc] || GL[fc]) twacht[fc] = 0;
	}
}
