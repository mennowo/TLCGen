/* ALGEMENE APPLICATIEFILE */
/* ----------------------- */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMsys.h
      CCOL:   11.0
    TLCGEN:   
   CCOLGEN:   UnitTest
*/

/****************************** Versie commentaar ***********************************
 *
 * Versie   Datum   Ontwerper   Commentaar
 *
 ************************************************************************************/

#define SYSTEM "GM"
#define RIS_GEEN_INDEXERING

/* fasecycli */
/* --------- */
    #define fc02  0
    #define fc22  1
    #define fc41  2
    #define fc42  3
    #define FCMAX1 4 /* aantal fasecycli */

/* overige uitgangen */
/* ----------------- */
    #define ussegm1       (FCMAX +  0) /* Aansturing segmenten display                            */
    #define ussegm2       (FCMAX +  1) /* Aansturing segmenten display                            */
    #define ussegm3       (FCMAX +  2) /* Aansturing segmenten display                            */
    #define ussegm4       (FCMAX +  3) /* Aansturing segmenten display                            */
    #define ussegm5       (FCMAX +  4) /* Aansturing segmenten display                            */
    #define ussegm6       (FCMAX +  5) /* Aansturing segmenten display                            */
    #define ussegm7       (FCMAX +  6) /* Aansturing segmenten display                            */
    #define usincontrol   (FCMAX +  7) /* Verklikken of applicatie daadwerkelijk de TLC aanstuurt */
    #define usnocontrol   (FCMAX +  8) /* Verklikken of applicatie niet in staat is te regelen    */
    #define usmaxwt       (FCMAX +  9) /* Verklikken maximale wachttijd overschreden              */
    #define uskarmelding  (FCMAX + 10) /* Verklikken ontvangst melding KAR                        */
    #define uskarog       (FCMAX + 11) /* Verklikken ondergedrag KAR                              */
    #define usovinm41bus  (FCMAX + 12) /* Verklikken inmelding OV fase 41                         */
    #define usovinm41tram (FCMAX + 13) /* Verklikken inmelding OV fase 41                         */
    #define usovinm42     (FCMAX + 14) /* Verklikken inmelding OV fase 42                         */
    #define usovinm22     (FCMAX + 15) /* Verklikken inmelding OV fase 22                         */
    #define usperdef      (FCMAX + 16) /* Default periode actief                                  */
    #define usper1        (FCMAX + 17) /* Periode actief                                          */
    #define USMAX1        (FCMAX + 18)

/* detectie */
/* -------- */
    #define d021               0
    #define d022               1
    #define d221               2
    #define d222               3
    #define dk22               4
    #define d411               5
    #define d412               6
    #define d421               7
    #define d422               8
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define ddummykarin41tram  9
    #define ddummykaruit41tram 10
    #define DPMAX1             11 /* aantal detectoren testomgeving */
#else
    #define DPMAX1             9 /* aantal detectoren automaat omgeving */
#endif

/* overige ingangen */
/* ---------------- */
    #define ISMAX1 (DPMAX + 0)

/* hulp elementen */
/* -------------- */
    #define hprio41bus          0 /* Bijhouden actief zijn prioriteit fase 41 */
    #define hprioin41bus        1 /* Prioriteit inmelding fase 41 Bus         */
    #define hpriouit41bus       2 /* Prioriteit uitmelding 41 Bus             */
    #define hprioin41busris     3 /* Prioriteit inmelding fase 41 Bus         */
    #define hpriouit41busris    4 /* Prioriteit uitmelding 41 Bus             */
    #define hprio41tram         5 /* Bijhouden actief zijn prioriteit fase 41 */
    #define hprioin41tram       6 /* Prioriteit inmelding fase 41 Tram        */
    #define hpriouit41tram      7 /* Prioriteit uitmelding 41 Tram            */
    #define hprioin41tramris    8 /* Prioriteit inmelding fase 41 Tram        */
    #define hprioin41tramkar    9 /* Prioriteit inmelding fase 41 Tram        */
    #define hpriouit41tramris  10 /* Prioriteit uitmelding 41 Tram            */
    #define hpriouit41tramkar  11 /* Prioriteit uitmelding 41 Tram            */
    #define hprio42            12 /* Bijhouden actief zijn prioriteit fase 42 */
    #define hprioin42          13 /* Prioriteit inmelding fase 42 Bus         */
    #define hpriouit42         14 /* Prioriteit uitmelding 42 Bus             */
    #define hprioin42ris       15 /* Prioriteit inmelding fase 42 Bus         */
    #define hpriouit42ris      16 /* Prioriteit uitmelding 42 Bus             */
    #define hprio22            17 /* Bijhouden actief zijn prioriteit fase 22 */
    #define hprioin22          18 /* Prioriteit inmelding fase 22 Fiets       */
    #define hpriouit22         19 /* Prioriteit uitmelding 22 Fiets           */
    #define hprioin22fiets221  20 /* Prioriteit inmelding fase 22 Fiets       */
    #define hpriouit22fiets221 21 /* Prioriteit uitmelding 22 Fiets           */
    #define HEMAX1             22

/* geheugen elementen */
/* ------------------ */
    #define mperiod           0 /* Onthouden actieve periode                     */
    #define mftstelris22fiets 1 /* Tellen fietsers obv. RIS voor prio ingreep 22 */
    #define mar02             2 /* Alternatieve ruimte fase 02                   */
    #define mar22             3 /* Alternatieve ruimte fase 22                   */
    #define mar41             4 /* Alternatieve ruimte fase 41                   */
    #define mar42             5 /* Alternatieve ruimte fase 42                   */
    #define MEMAX1            6

/* tijd elementen */
/* -------------- */
    #define tkm02         0 /* Kop maximum voor detector 02                          */
    #define tkm22         1 /* Kop maximum voor detector 22                          */
    #define tkm41         2 /* Kop maximum voor detector 41                          */
    #define tkm42         3 /* Kop maximum voor detector 42                          */
    #define tkarmelding   4 /* Duur verklikking ontvangst melding KAR                */
    #define tkarog        5 /* Ondergedrag KAR                                       */
    #define tbtovg41bus   6 /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    #define trt41bus      7 /* Actuele rijtijd prio fase 41                          */
    #define tgb41bus      8 /* Groenbewaking prioriteit fase 41                      */
    #define tblk41bus     9 /* Blokkeertijd na prioriteitsingreep fase 41            */
    #define tbtovg41tram 10 /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    #define trt41tram    11 /* Actuele rijtijd prio fase 41                          */
    #define tgb41tram    12 /* Groenbewaking prioriteit fase 41                      */
    #define tblk41tram   13 /* Blokkeertijd na prioriteitsingreep fase 41            */
    #define tbtovg42     14 /* Timer bezettijd prioriteit gehinderde rijtijd fase 42 */
    #define trt42        15 /* Actuele rijtijd prio fase 42                          */
    #define tgb42        16 /* Groenbewaking prioriteit fase 42                      */
    #define tblk42       17 /* Blokkeertijd na prioriteitsingreep fase 42            */
    #define tbtovg22     18 /* Timer bezettijd prioriteit gehinderde rijtijd fase 22 */
    #define trt22        19 /* Actuele rijtijd prio fase 22                          */
    #define tgb22        20 /* Groenbewaking prioriteit fase 22                      */
    #define tblk22       21 /* Blokkeertijd na prioriteitsingreep fase 22            */
    #define tris41bus    22
    #define tris41tram   23
    #define tris42bus    24
    #define TMMAX1       25

/* teller elementen */
/* ---------------- */
    #define cvc41bus       0 /* Bijhouden prio inmeldingen fase 41 type Bus             */
    #define cvc41tram      1 /* Bijhouden prio inmeldingen fase 41 type Tram            */
    #define cvc42          2 /* Bijhouden prio inmeldingen fase 42 type Bus             */
    #define cftscyc22fiets 3 /* Bijhouden realisaties tbv peloton prio voor fase 22     */
    #define cftsvtg22fiets 4 /* Bijhouden aantal fietsers tbv peloton prio voor fase 22 */
    #define cvc22          5 /* Bijhouden prio inmeldingen fase 22 type Fiets           */
    #define CTMAX1         6

/* schakelaars */
/* ----------- */
    #define schcheckdstype        0 /* Check type DSI bericht bij VECOM                            */
    #define schprioin41busris     1 /* Inmelden 41 via Bus toestaan                                */
    #define schpriouit41busris    2 /* Uitmelden 41 via Bus toestaan                               */
    #define schprioin41tramris    3 /* Inmelden 41 via Tram toestaan                               */
    #define schprioin41tramkar    4 /* Inmelden 41 via Tram toestaan                               */
    #define schpriouit41tramris   5 /* Uitmelden 41 via Tram toestaan                              */
    #define schpriouit41tramkar   6 /* Uitmelden 41 via Tram toestaan                              */
    #define schprioin42ris        7 /* Inmelden 42 via Bus toestaan                                */
    #define schpriouit42ris       8 /* Uitmelden 42 via Bus toestaan                               */
    #define schprioin22fiets221   9 /* Inmelden 22 via Fiets toestaan                              */
    #define schpriouit22fiets221 10 /* Uitmelden 22 via Fiets toestaan                             */
    #define schrisgeencheckopsg  11 /* Niet checken op signaalgroep bij RIS aanvragen en verlengen */
    #define schrisaanvraag       12 /* Globaal in of uitschakelen aanvragen via RIS                */
    #define schrisverlengen      13 /* Globaal in of uitschakelen verlengen via RIS                */
    #define schaltg02            14 /* Alternatieve realisatie toestaan fase 02                    */
    #define schaltg22            15 /* Alternatieve realisatie toestaan fase 22                    */
    #define schaltg41            16 /* Alternatieve realisatie toestaan fase 41                    */
    #define schaltg42            17 /* Alternatieve realisatie toestaan fase 42                    */
    #define SCHMAX1              18

/* parameters */
/* ---------- */
    #define prmfb                       0 /* Instelling fasebewaking                                                        */
    #define prmxx                       1 /* Versiebeheer xx                                                                */
    #define prmyy                       2 /* Versiebeheer yy                                                                */
    #define prmzz                       3 /* Versiebeheer zz                                                                */
    #define prmmk021                    4 /* Meetkriterium type voor detector 021                                           */
    #define prmmk022                    5 /* Meetkriterium type voor detector 022                                           */
    #define prmmk221                    6 /* Meetkriterium type voor detector 221                                           */
    #define prmmk222                    7 /* Meetkriterium type voor detector 222                                           */
    #define prmmkk22                    8 /* Meetkriterium type voor detector k22                                           */
    #define prmmk411                    9 /* Meetkriterium type voor detector 411                                           */
    #define prmmk412                   10 /* Meetkriterium type voor detector 412                                           */
    #define prmmk421                   11 /* Meetkriterium type voor detector 421                                           */
    #define prmmk422                   12 /* Meetkriterium type voor detector 422                                           */
    #define prmmwta                    13 /* Maximale wachttijd autoverkeer                                                 */
    #define prmmwtfts                  14 /* Maximale wachttijd fiets                                                       */
    #define prmmwtvtg                  15 /* Maximale wachttijd voetgangers                                                 */
    #define prmpmgt02                  16 /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                  */
    #define prmognt02                  17 /* Minimale groentijd bij terugkomen fase 02                                      */
    #define prmnofm02                  18 /* Aantal malen niet afkappen na OV ingreep fase 02                               */
    #define prmmgcov02                 19 /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov02                20 /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep */
    #define prmohpmg02                 21 /* Percentage ophogen groentijd na afkappen fase 02                               */
    #define prmpmgt22                  22 /* Minimaal percentage groentijd primair tbv. terugkomen fase 22                  */
    #define prmognt22                  23 /* Minimale groentijd bij terugkomen fase 22                                      */
    #define prmnofm22                  24 /* Aantal malen niet afkappen na OV ingreep fase 22                               */
    #define prmmgcov22                 25 /* Minimum groentijd waarna fase 22 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov22                26 /* Minimum percentage groentijd waarna fase 22 afgkapt mag worden door OV ingreep */
    #define prmohpmg22                 27 /* Percentage ophogen groentijd na afkappen fase 22                               */
    #define prmpmgt41                  28 /* Minimaal percentage groentijd primair tbv. terugkomen fase 41                  */
    #define prmognt41                  29 /* Minimale groentijd bij terugkomen fase 41                                      */
    #define prmnofm41                  30 /* Aantal malen niet afkappen na OV ingreep fase 41                               */
    #define prmmgcov41                 31 /* Minimum groentijd waarna fase 41 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov41                32 /* Minimum percentage groentijd waarna fase 41 afgkapt mag worden door OV ingreep */
    #define prmohpmg41                 33 /* Percentage ophogen groentijd na afkappen fase 41                               */
    #define prmpmgt42                  34 /* Minimaal percentage groentijd primair tbv. terugkomen fase 42                  */
    #define prmognt42                  35 /* Minimale groentijd bij terugkomen fase 42                                      */
    #define prmnofm42                  36 /* Aantal malen niet afkappen na OV ingreep fase 42                               */
    #define prmmgcov42                 37 /* Minimum groentijd waarna fase 42 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov42                38 /* Minimum percentage groentijd waarna fase 42 afgkapt mag worden door OV ingreep */
    #define prmohpmg42                 39 /* Percentage ophogen groentijd na afkappen fase 42                               */
    #define prmrto41bus                40 /* Ongehinderde rijtijd prioriteit fase 41                                        */
    #define prmrtbg41bus               41 /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    #define prmrtg41bus                42 /* Gehinderde rijtijd prioriteit fase 41                                          */
    #define prmomx41bus                43 /* Ondermaximum OV fase 41                                                        */
    #define prmupinagb41bus            44 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    #define prmprio41bus               45 /* Prioriteitsinstelling OV fase 41                                               */
    #define prmrisstart41bus           46
    #define prmrisend41bus             47
    #define prmriseta41bus             48
    #define prmrisrole41bus            49
    #define prmrissubrole41bus         50
    #define prmrisimportance41bus      51
    #define prmrisstationtype41bus     52
    #define prmrisgrenspriotype41bus   53 /* Greenswaarde type prioriteit RIS voor fase 41                                  */
    #define prmrisapproachid41bus      54
    #define prmrislaneid41bus_1        55
    #define prmrislaneid41bus_2        56
    #define prmrto41tram               57 /* Ongehinderde rijtijd prioriteit fase 41                                        */
    #define prmrtbg41tram              58 /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    #define prmrtg41tram               59 /* Gehinderde rijtijd prioriteit fase 41                                          */
    #define prmomx41tram               60 /* Ondermaximum OV fase 41                                                        */
    #define prmupinagb41tram           61 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    #define prmvtgcat41tram            62 /* Voertuigcategorie DSI voor prio ingreep 41tram                                 */
    #define prmprio41tram              63 /* Prioriteitsinstelling OV fase 41                                               */
    #define prmrisstart41tram          64
    #define prmrisend41tram            65
    #define prmrisrole41tram           66
    #define prmrissubrole41tram        67
    #define prmrisimportance41tram     68
    #define prmrisstationtype41tram    69
    #define prmrisgrenspriotype41tram  70 /* Greenswaarde type prioriteit RIS voor fase 41                                  */
    #define prmrisapproachid41tram     71
    #define prmrislaneid41tram_1       72
    #define prmrislaneid41tram_2       73
    #define prmrto42                   74 /* Ongehinderde rijtijd prioriteit fase 42                                        */
    #define prmrtbg42                  75 /* Beperkt gehinderde rijtijd prioriteit fase 42                                  */
    #define prmrtg42                   76 /* Gehinderde rijtijd prioriteit fase 42                                          */
    #define prmomx42                   77 /* Ondermaximum OV fase 42                                                        */
    #define prmupinagb42               78 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 42                  */
    #define prmprio42                  79 /* Prioriteitsinstelling OV fase 42                                               */
    #define prmrisstart42              80
    #define prmrisend42                81
    #define prmriseta42                82
    #define prmrisrole42               83
    #define prmrissubrole42            84
    #define prmrisimportance42         85
    #define prmrisstationtype42        86
    #define prmrisgrenspriotype42      87 /* Greenswaarde type prioriteit RIS voor fase 42                                  */
    #define prmftsblok22fiets          88 /* Blokken waarin fiets peloton prio actief mag zijn voor fase 22                 */
    #define prmftsmaxpercyc22fiets     89 /* Maximaal aantal keer fiets peloton prio per cyclus voor fase 22                */
    #define prmftsminvtg22fiets        90 /* Minimaal aantal gemeten fietsers tbv peloton prio voor fase 22                 */
    #define prmftsminwt22fiets         91 /* Minimale wachttijd tbv peloton prio voor fase 22                               */
    #define prmftsminvtgris22fiets     92 /* Minimaal aantal gemeten fietsers via RIS tbv peloton prio voor fase 22         */
    #define prmrto22                   93 /* Ongehinderde rijtijd prioriteit fase 22                                        */
    #define prmrtbg22                  94 /* Beperkt gehinderde rijtijd prioriteit fase 22                                  */
    #define prmrtg22                   95 /* Gehinderde rijtijd prioriteit fase 22                                          */
    #define prmomx22                   96 /* Ondermaximum OV fase 22                                                        */
    #define prmupinagb22               97 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 22                  */
    #define prmprio22                  98 /* Prioriteitsinstelling OV fase 22                                               */
    #define prmrisstart22              99
    #define prmrisend22               100
    #define prmriseta22               101
    #define prmrisrole22              102
    #define prmrissubrole22           103
    #define prmrisimportance22        104
    #define prmrisstationtype22       105
    #define prmrisgrenspriotype22     106 /* Greenswaarde type prioriteit RIS voor fase 22                                  */
    #define prmstkp1                  107 /* Start klokperiode ochtend                                                      */
    #define prmetkp1                  108 /* Einde klokperiode ochtend                                                      */
    #define prmdckp1                  109 /* Dagsoort klokperiode ochtend                                                   */
    #define prmmg1_02                 110 /* Maximale groentijd MG1 02                                                      */
    #define prmmg1_22                 111 /* Maximale groentijd MG1 22                                                      */
    #define prmmg1_41                 112 /* Maximale groentijd MG1 41                                                      */
    #define prmmg1_42                 113 /* Maximale groentijd MG1 42                                                      */
    #define prmmg2_02                 114 /* Maximale groentijd MG2 02                                                      */
    #define prmmg2_22                 115 /* Maximale groentijd MG2 22                                                      */
    #define prmmg2_41                 116 /* Maximale groentijd MG2 41                                                      */
    #define prmmg2_42                 117 /* Maximale groentijd MG2 42                                                      */
    #define prmrisapproachid41        118 /* Approach id voor fase 41                                                       */
    #define prmrisapproachid42        119 /* Approach id voor fase 42                                                       */
    #define prmrisapproachid22        120 /* Approach id voor fase 22                                                       */
    #define prmrislaneid41_1          121 /* Lane id voor lane met index 1 bij fase 41                                      */
    #define prmrislaneid41_2          122 /* Lane id voor lane met index 2 bij fase 41                                      */
    #define prmrislaneid42_1          123 /* Lane id voor lane met index 1 bij fase 42                                      */
    #define prmrislaneid22_1          124 /* Lane id voor lane met index 1 bij fase 22                                      */
    #define prmrispstart22fts1        125 /* Afstand van stopstreep tot start peloton bepaal gebied RIS fase 22             */
    #define prmrispend22fts1          126 /* Afstand van stopstreep tot einde peloton bepaal gebied RIS fase 22             */
    #define prmmlfpr02                127 /* Maximaal aantal modules vooruit fase 02                                        */
    #define prmmlfpr22                128 /* Maximaal aantal modules vooruit fase 22                                        */
    #define prmmlfpr41                129 /* Maximaal aantal modules vooruit fase 41                                        */
    #define prmmlfpr42                130 /* Maximaal aantal modules vooruit fase 42                                        */
    #define prmaltg02                 131 /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    #define prmaltp02                 132 /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    #define prmaltg22                 133 /* Minimale groentijd bij alternatieve realisatie fase 22                         */
    #define prmaltp22                 134 /* Minimale ruimte tbv alternatieve realisatie fase 22                            */
    #define prmaltg41                 135 /* Minimale groentijd bij alternatieve realisatie fase 41                         */
    #define prmaltp41                 136 /* Minimale ruimte tbv alternatieve realisatie fase 41                            */
    #define prmaltg42                 137 /* Minimale groentijd bij alternatieve realisatie fase 42                         */
    #define prmaltp42                 138 /* Minimale ruimte tbv alternatieve realisatie fase 42                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define prmtestdsivert            139 /* Testen vertraging in DSI bericht in testomgeving                               */
    #define prmtestdsilyn             140 /* Testen lijnnummer DSI bericht in testomgeving                                  */
    #define prmtestdsicat             141 /* Testen ritcategorie DSI bericht in testomgeving                                */
    #define PRMMAX1                   142
#else
    #define PRMMAX1                   139
#endif

/* Selectieve detectie */
/* ------------------- */
    #define dsdummy 0 /* Dummy SD lus 0: tbv KAR */
    #define DSMAX    1

    #define prioFC41bus 0
    #define prioFC41tram 1
    #define prioFC42 2
    #define prioFC22 3
    #define prioFCMAX 4

/* modulen */
/* ------- */
    #define MLMAX1 4 /* aantal modulen */

/* Aantal perioden voor max groen */
/* ------- */
    #define MPERIODMAX 2 /* aantal groenperioden */

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined PRACTICE_TEST
    #define TESTOMGEVING
#endif

/* Systeem naam in het topologiebestand */
/* ------------------------------------ */
#define SYSTEM_ITF "ITF1"

/* Definitie lane id in het topologiebestand */
/* ----------------------------------------- */
#define ris_conflict_gebied    0 /* connection tussen alle ingress lanes en egress lanes */

/* Gebruikers toevoegingen file includen */
/* ------------------------------------- */
    #include "GMsys.add"

