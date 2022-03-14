#include "starvar.h"

/* Deze functie wordt gedefinieerd in bestand <sys>tab.c */
void star_instellingen();

/* Funcie declaraties voor functies in starfunc.c */
void update_cyclustimer(count cyclustijd);
bool periode(count	cyclustijd, count cyclustimer, count begin_groen, count einde_groen);
void commando_groen(count fc);
void star_reset_bits(bool star);
void star_regelen();
bool star_test_alles_rood();
void star_bepaal_omschakelen(count mgewenst, count mwerkelijk, count mprogwissel);
