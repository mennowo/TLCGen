/* ALGEMENE APPLICATIEFILE */
/* ----------------------- */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMsys.h
      CCOL:   10.0
    TLCGEN:   
   CCOLGEN:   UnitTest
*/

/****************************** Versie commentaar ***********************************
 *
 * Versie   Datum   Ontwerper   Commentaar
 *
 ************************************************************************************/

#define SYSTEM "GM"

/* fasecycli */
/* --------- */
    #define fc02  0
    #define fc05  1
    #define fc41  2
    #define fc42  3
    #define FCMAX1 4 /* aantal fasecycli */

/* overige uitgangen */
/* ----------------- */
    #define ussegm1           (FCMAX +  0) /* Aansturing segmenten display               */
    #define ussegm2           (FCMAX +  1) /* Aansturing segmenten display               */
    #define ussegm3           (FCMAX +  2) /* Aansturing segmenten display               */
    #define ussegm4           (FCMAX +  3) /* Aansturing segmenten display               */
    #define ussegm5           (FCMAX +  4) /* Aansturing segmenten display               */
    #define ussegm6           (FCMAX +  5) /* Aansturing segmenten display               */
    #define ussegm7           (FCMAX +  6) /* Aansturing segmenten display               */
    #define usovtevroeg41tram (FCMAX +  7) /* Voorste OV voertuig bij 41 te vroeg Tram   */
    #define usovoptijd41tram  (FCMAX +  8) /* Voorste OV voertuig bij 41 op tijd Tram    */
    #define usovtelaat41tram  (FCMAX +  9) /* Voorste OV voertuig bij 41 te laat Tram    */
    #define usmaxwt           (FCMAX + 10) /* Verklikken maximale wachttijd overschreden */
    #define uskarmelding      (FCMAX + 11) /* Verklikken ontvangst melding KAR           */
    #define uskarog           (FCMAX + 12) /* Verklikken ondergedrag KAR                 */
    #define usovinm41tram     (FCMAX + 13) /* Verklikken inmelding OV fase 41            */
    #define usovinm42bus      (FCMAX + 14) /* Verklikken inmelding OV fase 42            */
    #define usperdef          (FCMAX + 15) /* Default periode actief                     */
    #define usper1            (FCMAX + 16) /* Periode actief                             */
    #define USMAX1            (FCMAX + 17)

/* detectie */
/* -------- */
    #define d021               0
    #define d022               1
    #define d051               2
    #define d052               3
    #define d411               4
    #define d412               5
    #define d421               6
    #define d422               7
    #define dwis1              8
    #define dsd42              9
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define ddummykarin41tram  10
    #define ddummykaruit41tram 11
    #define DPMAX1             12 /* aantal detectoren testomgeving */
#else
    #define DPMAX1             10 /* aantal detectoren automaat omgeving */
#endif

/* overige ingangen */
/* ---------------- */
    #define iswisc1 (DPMAX + 0)
    #define ISMAX1  (DPMAX + 1)

/* hulp elementen */
/* -------------- */
    #define hstp41tram            0 /* Geconditioneerde prio OV mogelijk bij 41 Tram */
    #define hprio41tram           1 /* Bijhouden actief zijn prioriteit fase 41      */
    #define hprioin41tram         2 /* Prioriteit inmelding fase 41 Tram             */
    #define hpriouit41tram        3 /* Prioriteit uitmelding 41 Tram                 */
    #define hprioin41tramkar      4 /* Prioriteit inmelding fase 41 Tram             */
    #define hprioin41tramdet411   5 /* Prioriteit inmelding fase 41 Tram             */
    #define hpriouit41tramkar     6 /* Prioriteit uitmelding 41 Tram                 */
    #define hpriouit41tramdet412  7 /* Prioriteit uitmelding 41 Tram                 */
    #define hwissel41tram         8 /* Onthouden voorwaarde fase 41                  */
    #define hprio42bus            9 /* Bijhouden actief zijn prioriteit fase 42      */
    #define hprioin42bus         10 /* Prioriteit inmelding fase 42 Bus              */
    #define hpriouit42bus        11 /* Prioriteit uitmelding 42 Bus                  */
    #define hprioin42bussdsd42   12 /* Prioriteit inmelding fase 42 Bus              */
    #define hpriouit42bussdsd42  13 /* Prioriteit uitmelding 42 Bus                  */
    #define HEMAX1               14

/* geheugen elementen */
/* ------------------ */
    #define mperiod    0 /* Onthouden actieve periode                 */
    #define mstp41tram 1 /* Stiptheid voorste OV voertuig bij 41 Tram */
    #define mar02      2 /* Alternatieve ruimte fase 02               */
    #define mar05      3 /* Alternatieve ruimte fase 05               */
    #define mar41      4 /* Alternatieve ruimte fase 41               */
    #define mar42      5 /* Alternatieve ruimte fase 42               */
    #define MEMAX1     6

/* tijd elementen */
/* -------------- */
    #define tkm02                0 /* Kop maximum voor detector 02                                                           */
    #define tkm05                1 /* Kop maximum voor detector 05                                                           */
    #define tkm41                2 /* Kop maximum voor detector 41                                                           */
    #define tkm42                3 /* Kop maximum voor detector 42                                                           */
    #define tkarmelding          4 /* Duur verklikking ontvangst melding KAR                                                 */
    #define tkarog               5 /* Ondergedrag KAR                                                                        */
    #define tprioin41tramdet411  6 /* Anti jutter tijd inmelden 41 Tram                                                      */
    #define tprioin41tram        7 /* Anti jutter tijd inmelden 41 Tram                                                      */
    #define tpriouit41tram       8 /* Anti jutter tijd uitmelden 41                                                          */
    #define tbtovg41tram         9 /* Timer bezettijd prioriteit gehinderde rijtijd fase 41                                  */
    #define trt41tram           10 /* Actuele rijtijd prio fase 41                                                           */
    #define tgb41tram           11 /* Groenbewaking prioriteit fase 41                                                       */
    #define tblk41tram          12 /* Blokkeertijd na prioriteitsingreep fase 41                                             */
    #define tovminrood41tram    13 /* Minimale roodtijd vooraf aan inmelden mogelijk bij 41 (indien ingesteld per inmelding) */
    #define tprioin42bus        14 /* Anti jutter tijd inmelden 42 Bus                                                       */
    #define tbtovg42bus         15 /* Timer bezettijd prioriteit gehinderde rijtijd fase 42                                  */
    #define trt42bus            16 /* Actuele rijtijd prio fase 42                                                           */
    #define tgb42bus            17 /* Groenbewaking prioriteit fase 42                                                       */
    #define tblk42bus           18 /* Blokkeertijd na prioriteitsingreep fase 42                                             */
    #define TMMAX1              19

/* teller elementen */
/* ---------------- */
    #define cvc41tram 0 /* Bijhouden prio inmeldingen fase 41 type Tram */
    #define cvc42bus  1 /* Bijhouden prio inmeldingen fase 42 type Bus  */
    #define CTMAX1    2

/* schakelaars */
/* ----------- */
    #define schovstipt41tram          0 /* Geconditioneerde prioteit voor OV bij 41 Tram */
    #define schgeenwisselwis1         1 /* Wisselstand van wis1 negeren                  */
    #define schgeenwisselwisc1        2 /* Wisselstand van wisc1 negeren                 */
    #define schwisselpolwis1          3 /* Polariteit van wissel ingang wis1             */
    #define schwisselpolwisc1         4 /* Polariteit van wissel ingang wisc1            */
    #define schcheckdstype            5 /* Check type DSI bericht bij VECOM              */
    #define schprioin41tramkar        6 /* Inmelden 41 via Tram toestaan                 */
    #define schprioin41tramdet411SD   7 /* Inmelden 41 via Tram toestaan                 */
    #define schpriouit41tramkar       8 /* Uitmelden 41 via Tram toestaan                */
    #define schpriouit41tramdet412ED  9 /* Uitmelden 41 via Tram toestaan                */
    #define schvi41tram              10 /* Versnelde inmelding fase 41                   */
    #define schprioin42bussdsd42SD   11 /* Inmelden 42 via Bus toestaan                  */
    #define schpriouit42bussdsd42SD  12 /* Uitmelden 42 via Bus toestaan                 */
    #define schaltg02                13 /* Alternatieve realisatie toestaan fase 02      */
    #define schaltg05                14 /* Alternatieve realisatie toestaan fase 05      */
    #define schaltg41                15 /* Alternatieve realisatie toestaan fase 41      */
    #define schaltg42                16 /* Alternatieve realisatie toestaan fase 42      */
    #define SCHMAX1                  17

/* parameters */
/* ---------- */
    #define prmfb                    0 /* Instelling fasebewaking                                                        */
    #define prmxx                    1 /* Versiebeheer xx                                                                */
    #define prmyy                    2 /* Versiebeheer yy                                                                */
    #define prmzz                    3 /* Versiebeheer zz                                                                */
    #define prmmk021                 4 /* Meetkriterium type voor detector 021                                           */
    #define prmmk022                 5 /* Meetkriterium type voor detector 022                                           */
    #define prmmk051                 6 /* Meetkriterium type voor detector 051                                           */
    #define prmmk052                 7 /* Meetkriterium type voor detector 052                                           */
    #define prmmk411                 8 /* Meetkriterium type voor detector 411                                           */
    #define prmmk412                 9 /* Meetkriterium type voor detector 412                                           */
    #define prmmk421                10 /* Meetkriterium type voor detector 421                                           */
    #define prmmk422                11 /* Meetkriterium type voor detector 422                                           */
    #define prmOVtstpgrensvroeg     12 /* Grens waarboven een OV voertuig als te vroeg wordt aangemerkt                  */
    #define prmOVtstpgrenslaat      13 /* Grens waarboven een OV voertuig als te laat wordt aangemerkt                   */
    #define prmovstipttevroeg41tram 14 /* Prioriteitsnveau OV te vroeg bij 41 Tram                                       */
    #define prmovstiptoptijd41tram  15 /* Prioriteitsnveau OV op tijd bij 41 Tram                                        */
    #define prmovstipttelaat41tram  16 /* Prioriteitsnveau OV te laat bij 41 Tram                                        */
    #define prmmwta                 17 /* Maximale wachttijd autoverkeer                                                 */
    #define prmmwtfts               18 /* Maximale wachttijd fiets                                                       */
    #define prmmwtvtg               19 /* Maximale wachttijd voetgangers                                                 */
    #define prmpmgt02               20 /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                  */
    #define prmognt02               21 /* Minimale groentijd bij terugkomen fase 02                                      */
    #define prmnofm02               22 /* Aantal malen niet afkappen na OV ingreep fase 02                               */
    #define prmmgcov02              23 /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov02             24 /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep */
    #define prmohpmg02              25 /* Percentage ophogen groentijd na afkappen fase 02                               */
    #define prmpmgt05               26 /* Minimaal percentage groentijd primair tbv. terugkomen fase 05                  */
    #define prmognt05               27 /* Minimale groentijd bij terugkomen fase 05                                      */
    #define prmnofm05               28 /* Aantal malen niet afkappen na OV ingreep fase 05                               */
    #define prmmgcov05              29 /* Minimum groentijd waarna fase 05 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov05             30 /* Minimum percentage groentijd waarna fase 05 afgkapt mag worden door OV ingreep */
    #define prmohpmg05              31 /* Percentage ophogen groentijd na afkappen fase 05                               */
    #define prmpmgt41               32 /* Minimaal percentage groentijd primair tbv. terugkomen fase 41                  */
    #define prmognt41               33 /* Minimale groentijd bij terugkomen fase 41                                      */
    #define prmnofm41               34 /* Aantal malen niet afkappen na OV ingreep fase 41                               */
    #define prmmgcov41              35 /* Minimum groentijd waarna fase 41 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov41             36 /* Minimum percentage groentijd waarna fase 41 afgkapt mag worden door OV ingreep */
    #define prmohpmg41              37 /* Percentage ophogen groentijd na afkappen fase 41                               */
    #define prmpmgt42               38 /* Minimaal percentage groentijd primair tbv. terugkomen fase 42                  */
    #define prmognt42               39 /* Minimale groentijd bij terugkomen fase 42                                      */
    #define prmnofm42               40 /* Aantal malen niet afkappen na OV ingreep fase 42                               */
    #define prmmgcov42              41 /* Minimum groentijd waarna fase 42 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov42             42 /* Minimum percentage groentijd waarna fase 42 afgkapt mag worden door OV ingreep */
    #define prmohpmg42              43 /* Percentage ophogen groentijd na afkappen fase 42                               */
    #define prmrto41tram            44 /* Ongehinderde rijtijd prioriteit fase 41                                        */
    #define prmrtbg41tram           45 /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    #define prmrtg41tram            46 /* Gehinderde rijtijd prioriteit fase 41                                          */
    #define prmomx41tram            47 /* Ondermaximum OV fase 41                                                        */
    #define prmupinagb41tram        48 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    #define prmvtgcat41tram         49 /* Voertuigcategorie DSI voor prio ingreep 41tram                                 */
    #define prmprio41tram           50 /* Prioriteitsinstelling OV fase 41                                               */
    #define prmallelijnen41tram     51 /* Prioriteit geven aan alle lijnen fase 41                                       */
    #define prmlijn41tram_01        52 /* Lijnnummer 1 tbv prioriteit OV fase 41                                         */
    #define prmlijn41tram_02        53 /* Lijnnummer 2 tbv prioriteit OV fase 41                                         */
    #define prmritcat41tram_01      54 /* Ritcategorie bij lijnnummer 1 tbv prioriteit OV fase 1                         */
    #define prmritcat41tram_02      55 /* Ritcategorie bij lijnnummer 2 tbv prioriteit OV fase 2                         */
    #define prmrto42bus             56 /* Ongehinderde rijtijd prioriteit fase 42                                        */
    #define prmrtbg42bus            57 /* Beperkt gehinderde rijtijd prioriteit fase 42                                  */
    #define prmrtg42bus             58 /* Gehinderde rijtijd prioriteit fase 42                                          */
    #define prmomx42bus             59 /* Ondermaximum OV fase 42                                                        */
    #define prmupinagb42bus         60 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 42                  */
    #define prmvtgcat42bus          61 /* Voertuigcategorie DSI voor prio ingreep 42bus                                  */
    #define prmprio42bus            62 /* Prioriteitsinstelling OV fase 42                                               */
    #define prmallelijnen42bus      63 /* Prioriteit geven aan alle lijnen fase 42                                       */
    #define prmlijn42bus_01         64 /* Lijnnummer 1 tbv prioriteit OV fase 42                                         */
    #define prmritcat42bus_01       65 /* Ritcategorie bij lijnnummer 1 tbv prioriteit OV fase 1                         */
    #define prmstkp1                66 /* Start klokperiode ochtend                                                      */
    #define prmetkp1                67 /* Einde klokperiode ochtend                                                      */
    #define prmdckp1                68 /* Dagsoort klokperiode ochtend                                                   */
    #define prmmg1_02               69 /* Maximale groentijd MG1 02                                                      */
    #define prmmg1_05               70 /* Maximale groentijd MG1 05                                                      */
    #define prmmg1_41               71 /* Maximale groentijd MG1 41                                                      */
    #define prmmg1_42               72 /* Maximale groentijd MG1 42                                                      */
    #define prmmg2_02               73 /* Maximale groentijd MG2 02                                                      */
    #define prmmg2_05               74 /* Maximale groentijd MG2 05                                                      */
    #define prmmg2_41               75 /* Maximale groentijd MG2 41                                                      */
    #define prmmg2_42               76 /* Maximale groentijd MG2 42                                                      */
    #define prmmlfpr02              77 /* Maximaal aantal modules vooruit fase 02                                        */
    #define prmmlfpr05              78 /* Maximaal aantal modules vooruit fase 05                                        */
    #define prmmlfpr41              79 /* Maximaal aantal modules vooruit fase 41                                        */
    #define prmmlfpr42              80 /* Maximaal aantal modules vooruit fase 42                                        */
    #define prmaltg02               81 /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    #define prmaltp02               82 /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    #define prmaltg05               83 /* Minimale groentijd bij alternatieve realisatie fase 05                         */
    #define prmaltp05               84 /* Minimale ruimte tbv alternatieve realisatie fase 05                            */
    #define prmaltg41               85 /* Minimale groentijd bij alternatieve realisatie fase 41                         */
    #define prmaltp41               86 /* Minimale ruimte tbv alternatieve realisatie fase 41                            */
    #define prmaltg42               87 /* Minimale groentijd bij alternatieve realisatie fase 42                         */
    #define prmaltp42               88 /* Minimale ruimte tbv alternatieve realisatie fase 42                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define prmtestdsivert          89 /* Testen vertraging in DSI bericht in testomgeving                               */
    #define prmtestdsilyn           90 /* Testen lijnnummer DSI bericht in testomgeving                                  */
    #define prmtestdsicat           91 /* Testen ritcategorie DSI bericht in testomgeving                                */
    #define PRMMAX1                 92
#else
    #define PRMMAX1                 89
#endif

/* Selectieve detectie */
/* ------------------- */
    #define dsdummy 0 /* Dummy SD lus 0: tbv KAR & VECOM start op 1 */
    #define DSD42 1
    #define DSMAX    2

    #define prioFC41tram 0
    #define prioFC42bus 1
    #define prioFCMAX 2

/* modulen */
/* ------- */
    #define MLMAX1 4 /* aantal modulen */

/* Aantal perioden voor max groen */
/* ------- */
    #define MPERIODMAX 2 /* aantal groenperioden */

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined PRACTICE_TEST
    #define TESTOMGEVING
#endif

/* Gebruikers toevoegingen file includen */
/* ------------------------------------- */
    #include "GMsys.add"

