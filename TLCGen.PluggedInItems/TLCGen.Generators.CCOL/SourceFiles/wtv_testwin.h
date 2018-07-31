/* Test window voor visualisatie werking Wachttijdvoorspeller */
/* Versie: 1.0
   Auteur: Menno van der Woude

   Voor gebruik van deze functies moet wtv_testwin.lib worden toegevoegd aan het project
*/

/* Initialisatie functie
   - Eenmalig aanroepen bij start applicatie; bij post_init_application()
   Functie argumenten:
   - system -> geef de waarde van #define SYSTEM door
*/
void extrawin_init(char * system);

/* Fasen toevoegen
   - Aanroepen per fase die selecteerbaar moet zijn
   - Eenmalig aanroepen bij start applicatie; bij post_init_application()
   - Aanroep _na_ aanroepen van extrawin_init()
   Functie argumenten:
   - fc -> fase om toe te voegen, bv fc21
   - usbus -> NG, of een uitgang die waar wordt indien BUS in het display moet worden weergegeven
   - type -> type weergave: 0 = Leds, 1 = 3 x 8 Segmenten display
*/
void extrawin_add_fc(short fc, short usbus, short type);

/* Aansturen weergave
   - Aanroepen ergens in de applicatie
   Functie argumenten:
   - fc -> fase om te update
   - mm -> memory element om uit lezen voor resterende leds/wachttijd
*/
void extrawin_wtv(int fc, int mm);
