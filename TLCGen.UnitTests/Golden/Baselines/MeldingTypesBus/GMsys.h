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
    #define ussegm1      (FCMAX +  0) /* Aansturing segmenten display               */
    #define ussegm2      (FCMAX +  1) /* Aansturing segmenten display               */
    #define ussegm3      (FCMAX +  2) /* Aansturing segmenten display               */
    #define ussegm4      (FCMAX +  3) /* Aansturing segmenten display               */
    #define ussegm5      (FCMAX +  4) /* Aansturing segmenten display               */
    #define ussegm6      (FCMAX +  5) /* Aansturing segmenten display               */
    #define ussegm7      (FCMAX +  6) /* Aansturing segmenten display               */
    #define usmaxwt      (FCMAX +  7) /* Verklikken maximale wachttijd overschreden */
    #define uskarmelding (FCMAX +  8) /* Verklikken ontvangst melding KAR           */
    #define uskarog      (FCMAX +  9) /* Verklikken ondergedrag KAR                 */
    #define usovinm41bus (FCMAX + 10) /* Verklikken inmelding OV fase 41            */
    #define usovinm42bus (FCMAX + 11) /* Verklikken inmelding OV fase 42            */
    #define usperdef     (FCMAX + 12) /* Default periode actief                     */
    #define usper1       (FCMAX + 13) /* Periode actief                             */
    #define USMAX1       (FCMAX + 14)

/* detectie */
/* -------- */
    #define d021              0
    #define d022              1
    #define d051              2
    #define d052              3
    #define d411              4
    #define d412              5
    #define d413              6
    #define d414              7
    #define d421              8
    #define d422              9
    #define dsd41in           10
    #define dsd41uit          11
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define ddummykarin41bus  12
    #define ddummykaruit41bus 13
    #define DPMAX1            14 /* aantal detectoren testomgeving */
#else
    #define DPMAX1            12 /* aantal detectoren automaat omgeving */
#endif

/* overige ingangen */
/* ---------------- */
    #define isov41in  (DPMAX + 0)
    #define isov41uit (DPMAX + 1)
    #define ISMAX1    (DPMAX + 2)

/* hulp elementen */
/* -------------- */
    #define hprio41bus                     0 /* Bijhouden actief zijn prioriteit fase 41 */
    #define hprioin41bus                   1 /* Prioriteit inmelding fase 41 Bus         */
    #define hpriouit41bus                  2 /* Prioriteit uitmelding 41 Bus             */
    #define hprioin41buskar                3 /* Prioriteit inmelding fase 41 Bus         */
    #define hprioin41bussdsd41in           4 /* Prioriteit inmelding fase 41 Bus         */
    #define hprioin41busdet412413          5 /* Prioriteit inmelding fase 41 Bus         */
    #define hprioin41busdet413             6 /* Prioriteit inmelding fase 41 Bus         */
    #define hprioin41businpov41in          7 /* Prioriteit inmelding fase 41 Bus         */
    #define hprioin41businpov41inov41uit   8 /* Prioriteit inmelding fase 41 Bus         */
    #define hprioin41busvecio411           9 /* Prioriteit inmelding fase 41 Bus         */
    #define hprioin41busopti              10 /* Prioriteit inmelding fase 41 Opticom     */
    #define hpriouit41busopti             11 /* Prioriteit uitmelding 41 Opticom         */
    #define hprioin41busopti414           12 /* Prioriteit inmelding fase 41 Bus         */
    #define hpriouit41buskar              13 /* Prioriteit uitmelding 41 Bus             */
    #define hpriouit41bussdsd41uit        14 /* Prioriteit uitmelding 41 Bus             */
    #define hpriouit41busdet411           15 /* Prioriteit uitmelding 41 Bus             */
    #define hpriouit41busdet412413        16 /* Prioriteit uitmelding 41 Bus             */
    #define hpriouit41businpov41in        17 /* Prioriteit uitmelding 41 Bus             */
    #define hpriouit41businpov41uitov41in 18 /* Prioriteit uitmelding 41 Bus             */
    #define hpriouit41busvecio414         19 /* Prioriteit uitmelding 41 Bus             */
    #define hpriouit41busopti414          20 /* Prioriteit uitmelding 41 Bus             */
    #define hprio42bus                    21 /* Bijhouden actief zijn prioriteit fase 42 */
    #define hprioin42bus                  22 /* Prioriteit inmelding fase 42 Bus         */
    #define hpriouit42bus                 23 /* Prioriteit uitmelding 42 Bus             */
    #define hprioin42busopti              24 /* Prioriteit inmelding fase 42 Opticom     */
    #define hpriouit42busopti             25 /* Prioriteit uitmelding 42 Opticom         */
    #define hprioin42busopti421           26 /* Prioriteit inmelding fase 42 Bus         */
    #define hpriouit42busopti422          27 /* Prioriteit uitmelding 42 Bus             */
    #define hpriouit42busopti423          28 /* Prioriteit uitmelding 42 Bus             */
    #define HEMAX1                        29

/* geheugen elementen */
/* ------------------ */
    #define mperiod 0 /* Onthouden actieve periode   */
    #define mar02   1 /* Alternatieve ruimte fase 02 */
    #define mar05   2 /* Alternatieve ruimte fase 05 */
    #define mar41   3 /* Alternatieve ruimte fase 41 */
    #define mar42   4 /* Alternatieve ruimte fase 42 */
    #define MEMAX1  5

/* tijd elementen */
/* -------------- */
    #define tkm02                0 /* Kop maximum voor detector 02                          */
    #define tkm05                1 /* Kop maximum voor detector 05                          */
    #define tkm41                2 /* Kop maximum voor detector 41                          */
    #define tkm42                3 /* Kop maximum voor detector 42                          */
    #define tkarmelding          4 /* Duur verklikking ontvangst melding KAR                */
    #define tkarog               5 /* Ondergedrag KAR                                       */
    #define tprioin41buskar      6 /* Anti jutter tijd inmelden 41 Bus                      */
    #define tprioin41busdet413   7 /* Anti jutter tijd inmelden 41 Bus                      */
    #define tprioin41busopti     8 /* Anti jutter tijd inmelden 41 Opticom                  */
    #define tprioin41busopti414  9 /* Anti jutter tijd inmelden 41 Bus                      */
    #define tbtovg41bus         10 /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    #define trt41bus            11 /* Actuele rijtijd prio fase 41                          */
    #define tgb41bus            12 /* Groenbewaking prioriteit fase 41                      */
    #define tblk41bus           13 /* Blokkeertijd na prioriteitsingreep fase 41            */
    #define tprioin42busopti    14 /* Anti jutter tijd inmelden 42 Opticom                  */
    #define tbtovg42bus         15 /* Timer bezettijd prioriteit gehinderde rijtijd fase 42 */
    #define trt42bus            16 /* Actuele rijtijd prio fase 42                          */
    #define tgb42bus            17 /* Groenbewaking prioriteit fase 42                      */
    #define tblk42bus           18 /* Blokkeertijd na prioriteitsingreep fase 42            */
    #define TMMAX1              19

/* teller elementen */
/* ---------------- */
    #define cvc41bus 0 /* Bijhouden prio inmeldingen fase 41 type Bus */
    #define cvc42bus 1 /* Bijhouden prio inmeldingen fase 42 type Bus */
    #define CTMAX1   2

/* schakelaars */
/* ----------- */
    #define schcheckdstype                        0 /* Check type DSI bericht bij VECOM         */
    #define schprioin41busdet411SD                1 /* Inmelden 41 via Bus toestaan             */
    #define schprioin41buskar                     2 /* Inmelden 41 via Bus toestaan             */
    #define schprioin41bussdsd41inSD              3 /* Inmelden 41 via Bus toestaan             */
    #define schprioin41busdet412D413DB            4 /* Inmelden 41 via Bus toestaan             */
    #define schprioin41busdet414ETDH              5 /* Inmelden 41 via Bus toestaan             */
    #define schprioin41busdet413SDB               6 /* Inmelden 41 via Bus toestaan             */
    #define schprioin41businpov41inSIS            7 /* Inmelden 41 via Bus toestaan             */
    #define schprioin41businpov41inISov41uitSIS   8 /* Inmelden 41 via Bus toestaan             */
    #define schprioin41busvecio411SD              9 /* Inmelden 41 via Bus toestaan             */
    #define schprioin41busopti414DB              10 /* Inmelden 41 via Bus toestaan             */
    #define schpriouit41buskar                   11 /* Uitmelden 41 via Bus toestaan            */
    #define schpriouit41bussdsd41uitSD           12 /* Uitmelden 41 via Bus toestaan            */
    #define schpriouit41busdet411D               13 /* Uitmelden 41 via Bus toestaan            */
    #define schpriouit41busdet412ED413ETDH       14 /* Uitmelden 41 via Bus toestaan            */
    #define schpriouit41businpov41inEIS          15 /* Uitmelden 41 via Bus toestaan            */
    #define schpriouit41businpov41uitEISov41inIS 16 /* Uitmelden 41 via Bus toestaan            */
    #define schpriouit41busvecio414SD            17 /* Uitmelden 41 via Bus toestaan            */
    #define schpriouit41busopti414DB             18 /* Uitmelden 41 via Bus toestaan            */
    #define schprioin42busopti421DB              19 /* Inmelden 42 via Bus toestaan             */
    #define schpriouit42busopti422DB             20 /* Uitmelden 42 via Bus toestaan            */
    #define schpriouit42busopti423DB             21 /* Uitmelden 42 via Bus toestaan            */
    #define schaltg02                            22 /* Alternatieve realisatie toestaan fase 02 */
    #define schaltg05                            23 /* Alternatieve realisatie toestaan fase 05 */
    #define schaltg41                            24 /* Alternatieve realisatie toestaan fase 41 */
    #define schaltg42                            25 /* Alternatieve realisatie toestaan fase 42 */
    #define SCHMAX1                              26

/* parameters */
/* ---------- */
    #define prmfb            0 /* Instelling fasebewaking                                                        */
    #define prmxx            1 /* Versiebeheer xx                                                                */
    #define prmyy            2 /* Versiebeheer yy                                                                */
    #define prmzz            3 /* Versiebeheer zz                                                                */
    #define prmmk021         4 /* Meetkriterium type voor detector 021                                           */
    #define prmmk022         5 /* Meetkriterium type voor detector 022                                           */
    #define prmmk051         6 /* Meetkriterium type voor detector 051                                           */
    #define prmmk052         7 /* Meetkriterium type voor detector 052                                           */
    #define prmmk411         8 /* Meetkriterium type voor detector 411                                           */
    #define prmmk412         9 /* Meetkriterium type voor detector 412                                           */
    #define prmmk413        10 /* Meetkriterium type voor detector 413                                           */
    #define prmmk414        11 /* Meetkriterium type voor detector 414                                           */
    #define prmmk421        12 /* Meetkriterium type voor detector 421                                           */
    #define prmmk422        13 /* Meetkriterium type voor detector 422                                           */
    #define prmmwta         14 /* Maximale wachttijd autoverkeer                                                 */
    #define prmmwtfts       15 /* Maximale wachttijd fiets                                                       */
    #define prmmwtvtg       16 /* Maximale wachttijd voetgangers                                                 */
    #define prmpmgt02       17 /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                  */
    #define prmognt02       18 /* Minimale groentijd bij terugkomen fase 02                                      */
    #define prmnofm02       19 /* Aantal malen niet afkappen na OV ingreep fase 02                               */
    #define prmmgcov02      20 /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov02     21 /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep */
    #define prmohpmg02      22 /* Percentage ophogen groentijd na afkappen fase 02                               */
    #define prmpmgt05       23 /* Minimaal percentage groentijd primair tbv. terugkomen fase 05                  */
    #define prmognt05       24 /* Minimale groentijd bij terugkomen fase 05                                      */
    #define prmnofm05       25 /* Aantal malen niet afkappen na OV ingreep fase 05                               */
    #define prmmgcov05      26 /* Minimum groentijd waarna fase 05 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov05     27 /* Minimum percentage groentijd waarna fase 05 afgkapt mag worden door OV ingreep */
    #define prmohpmg05      28 /* Percentage ophogen groentijd na afkappen fase 05                               */
    #define prmpmgt41       29 /* Minimaal percentage groentijd primair tbv. terugkomen fase 41                  */
    #define prmognt41       30 /* Minimale groentijd bij terugkomen fase 41                                      */
    #define prmnofm41       31 /* Aantal malen niet afkappen na OV ingreep fase 41                               */
    #define prmmgcov41      32 /* Minimum groentijd waarna fase 41 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov41     33 /* Minimum percentage groentijd waarna fase 41 afgkapt mag worden door OV ingreep */
    #define prmohpmg41      34 /* Percentage ophogen groentijd na afkappen fase 41                               */
    #define prmpmgt42       35 /* Minimaal percentage groentijd primair tbv. terugkomen fase 42                  */
    #define prmognt42       36 /* Minimale groentijd bij terugkomen fase 42                                      */
    #define prmnofm42       37 /* Aantal malen niet afkappen na OV ingreep fase 42                               */
    #define prmmgcov42      38 /* Minimum groentijd waarna fase 42 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov42     39 /* Minimum percentage groentijd waarna fase 42 afgkapt mag worden door OV ingreep */
    #define prmohpmg42      40 /* Percentage ophogen groentijd na afkappen fase 42                               */
    #define prmrto41bus     41 /* Ongehinderde rijtijd prioriteit fase 41                                        */
    #define prmrtbg41bus    42 /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    #define prmrtg41bus     43 /* Gehinderde rijtijd prioriteit fase 41                                          */
    #define prmomx41bus     44 /* Ondermaximum OV fase 41                                                        */
    #define prmupinagb41bus 45 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    #define prmvtgcat41bus  46 /* Voertuigcategorie DSI voor prio ingreep 41bus                                  */
    #define prmprio41bus    47 /* Prioriteitsinstelling OV fase 41                                               */
    #define prmrto42bus     48 /* Ongehinderde rijtijd prioriteit fase 42                                        */
    #define prmrtbg42bus    49 /* Beperkt gehinderde rijtijd prioriteit fase 42                                  */
    #define prmrtg42bus     50 /* Gehinderde rijtijd prioriteit fase 42                                          */
    #define prmomx42bus     51 /* Ondermaximum OV fase 42                                                        */
    #define prmupinagb42bus 52 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 42                  */
    #define prmprio42bus    53 /* Prioriteitsinstelling OV fase 42                                               */
    #define prmstkp1        54 /* Start klokperiode ochtend                                                      */
    #define prmetkp1        55 /* Einde klokperiode ochtend                                                      */
    #define prmdckp1        56 /* Dagsoort klokperiode ochtend                                                   */
    #define prmmg1_02       57 /* Maximale groentijd MG1 02                                                      */
    #define prmmg1_05       58 /* Maximale groentijd MG1 05                                                      */
    #define prmmg1_41       59 /* Maximale groentijd MG1 41                                                      */
    #define prmmg1_42       60 /* Maximale groentijd MG1 42                                                      */
    #define prmmg2_02       61 /* Maximale groentijd MG2 02                                                      */
    #define prmmg2_05       62 /* Maximale groentijd MG2 05                                                      */
    #define prmmg2_41       63 /* Maximale groentijd MG2 41                                                      */
    #define prmmg2_42       64 /* Maximale groentijd MG2 42                                                      */
    #define prmmlfpr02      65 /* Maximaal aantal modules vooruit fase 02                                        */
    #define prmmlfpr05      66 /* Maximaal aantal modules vooruit fase 05                                        */
    #define prmmlfpr41      67 /* Maximaal aantal modules vooruit fase 41                                        */
    #define prmmlfpr42      68 /* Maximaal aantal modules vooruit fase 42                                        */
    #define prmaltg02       69 /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    #define prmaltp02       70 /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    #define prmaltg05       71 /* Minimale groentijd bij alternatieve realisatie fase 05                         */
    #define prmaltp05       72 /* Minimale ruimte tbv alternatieve realisatie fase 05                            */
    #define prmaltg41       73 /* Minimale groentijd bij alternatieve realisatie fase 41                         */
    #define prmaltp41       74 /* Minimale ruimte tbv alternatieve realisatie fase 41                            */
    #define prmaltg42       75 /* Minimale groentijd bij alternatieve realisatie fase 42                         */
    #define prmaltp42       76 /* Minimale ruimte tbv alternatieve realisatie fase 42                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define prmtestdsivert  77 /* Testen vertraging in DSI bericht in testomgeving                               */
    #define prmtestdsilyn   78 /* Testen lijnnummer DSI bericht in testomgeving                                  */
    #define prmtestdsicat   79 /* Testen ritcategorie DSI bericht in testomgeving                                */
    #define PRMMAX1         80
#else
    #define PRMMAX1         77
#endif

/* Selectieve detectie */
/* ------------------- */
    #define dsdummy 0 /* Dummy SD lus 0: tbv KAR & VECOM start op 1 */
    #define DSD41IN 1
    #define DSD41UIT 2
    #define DSMAX    3

    #define prioFC41bus 0
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

