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
    #define fc08  2
    #define fc09  3
    #define fc41  4
    #define FCMAX1 5 /* aantal fasecycli */

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
    #define usperdef     (FCMAX + 11) /* Default periode actief                     */
    #define usper1       (FCMAX + 12) /* Periode actief                             */
    #define USMAX1       (FCMAX + 13)

/* detectie */
/* -------- */
    #define d021              0
    #define d022              1
    #define d051              2
    #define d052              3
    #define d081              4
    #define d082              5
    #define d091              6
    #define d092              7
    #define d411              8
    #define d412              9
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define ddummykarin41bus  10
    #define ddummykaruit41bus 11
    #define DPMAX1            12 /* aantal detectoren testomgeving */
#else
    #define DPMAX1            10 /* aantal detectoren automaat omgeving */
#endif

/* overige ingangen */
/* ---------------- */
    #define ISMAX1 (DPMAX + 0)

/* hulp elementen */
/* -------------- */
    #define hprio41bus       0 /* Bijhouden actief zijn prioriteit fase 41 */
    #define hprioin41bus     1 /* Prioriteit inmelding fase 41 Bus         */
    #define hpriouit41bus    2 /* Prioriteit uitmelding 41 Bus             */
    #define hprioin41buskar  3 /* Prioriteit inmelding fase 41 Bus         */
    #define hpriouit41buskar 4 /* Prioriteit uitmelding 41 Bus             */
    #define HEMAX1           5

/* geheugen elementen */
/* ------------------ */
    #define mperiod 0 /* Onthouden actieve periode   */
    #define mar02   1 /* Alternatieve ruimte fase 02 */
    #define mar05   2 /* Alternatieve ruimte fase 05 */
    #define mar08   3 /* Alternatieve ruimte fase 08 */
    #define mar09   4 /* Alternatieve ruimte fase 09 */
    #define mar41   5 /* Alternatieve ruimte fase 41 */
    #define MEMAX1  6

/* tijd elementen */
/* -------------- */
    #define tkm02        0 /* Kop maximum voor detector 02                          */
    #define tkm05        1 /* Kop maximum voor detector 05                          */
    #define tkm08        2 /* Kop maximum voor detector 08                          */
    #define tkm09        3 /* Kop maximum voor detector 09                          */
    #define tkm41        4 /* Kop maximum voor detector 41                          */
    #define tnleg0208    5 /* Naloop op einde groen van 02 naar 08                  */
    #define tnlcv0509    6 /* Naloop tijdens cyclisch verlenggroen van 05 naar 09   */
    #define tkarmelding  7 /* Duur verklikking ontvangst melding KAR                */
    #define tkarog       8 /* Ondergedrag KAR                                       */
    #define tbtovg41bus  9 /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    #define trt41bus    10 /* Actuele rijtijd prio fase 41                          */
    #define tgb41bus    11 /* Groenbewaking prioriteit fase 41                      */
    #define tblk41bus   12 /* Blokkeertijd na prioriteitsingreep fase 41            */
    #define TMMAX1      13

/* teller elementen */
/* ---------------- */
    #define cvc41bus 0 /* Bijhouden prio inmeldingen fase 41 type Bus */
    #define CTMAX1   1

/* schakelaars */
/* ----------- */
    #define schcheckdstype     0 /* Check type DSI bericht bij VECOM         */
    #define schprioin41buskar  1 /* Inmelden 41 via Bus toestaan             */
    #define schpriouit41buskar 2 /* Uitmelden 41 via Bus toestaan            */
    #define schaltg02          3 /* Alternatieve realisatie toestaan fase 02 */
    #define schaltg05          4 /* Alternatieve realisatie toestaan fase 05 */
    #define schaltg08          5 /* Alternatieve realisatie toestaan fase 08 */
    #define schaltg09          6 /* Alternatieve realisatie toestaan fase 09 */
    #define schaltg41          7 /* Alternatieve realisatie toestaan fase 41 */
    #define SCHMAX1            8

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
    #define prmmk081         8 /* Meetkriterium type voor detector 081                                           */
    #define prmmk082         9 /* Meetkriterium type voor detector 082                                           */
    #define prmmk091        10 /* Meetkriterium type voor detector 091                                           */
    #define prmmk092        11 /* Meetkriterium type voor detector 092                                           */
    #define prmmk411        12 /* Meetkriterium type voor detector 411                                           */
    #define prmmk412        13 /* Meetkriterium type voor detector 412                                           */
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
    #define prmpmgt08       29 /* Minimaal percentage groentijd primair tbv. terugkomen fase 08                  */
    #define prmognt08       30 /* Minimale groentijd bij terugkomen fase 08                                      */
    #define prmnofm08       31 /* Aantal malen niet afkappen na OV ingreep fase 08                               */
    #define prmmgcov08      32 /* Minimum groentijd waarna fase 08 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov08     33 /* Minimum percentage groentijd waarna fase 08 afgkapt mag worden door OV ingreep */
    #define prmohpmg08      34 /* Percentage ophogen groentijd na afkappen fase 08                               */
    #define prmpmgt09       35 /* Minimaal percentage groentijd primair tbv. terugkomen fase 09                  */
    #define prmognt09       36 /* Minimale groentijd bij terugkomen fase 09                                      */
    #define prmnofm09       37 /* Aantal malen niet afkappen na OV ingreep fase 09                               */
    #define prmmgcov09      38 /* Minimum groentijd waarna fase 09 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov09     39 /* Minimum percentage groentijd waarna fase 09 afgkapt mag worden door OV ingreep */
    #define prmohpmg09      40 /* Percentage ophogen groentijd na afkappen fase 09                               */
    #define prmrto41bus     41 /* Ongehinderde rijtijd prioriteit fase 41                                        */
    #define prmrtbg41bus    42 /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    #define prmrtg41bus     43 /* Gehinderde rijtijd prioriteit fase 41                                          */
    #define prmomx41bus     44 /* Ondermaximum OV fase 41                                                        */
    #define prmupinagb41bus 45 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    #define prmvtgcat41bus  46 /* Voertuigcategorie DSI voor prio ingreep 41bus                                  */
    #define prmprio41bus    47 /* Prioriteitsinstelling OV fase 41                                               */
    #define prmstkp1        48 /* Start klokperiode ochtend                                                      */
    #define prmetkp1        49 /* Einde klokperiode ochtend                                                      */
    #define prmdckp1        50 /* Dagsoort klokperiode ochtend                                                   */
    #define prmmg1_02       51 /* Maximale groentijd MG1 02                                                      */
    #define prmmg1_05       52 /* Maximale groentijd MG1 05                                                      */
    #define prmmg1_08       53 /* Maximale groentijd MG1 08                                                      */
    #define prmmg1_09       54 /* Maximale groentijd MG1 09                                                      */
    #define prmmg1_41       55 /* Maximale groentijd MG1 41                                                      */
    #define prmmg2_02       56 /* Maximale groentijd MG2 02                                                      */
    #define prmmg2_05       57 /* Maximale groentijd MG2 05                                                      */
    #define prmmg2_08       58 /* Maximale groentijd MG2 08                                                      */
    #define prmmg2_09       59 /* Maximale groentijd MG2 09                                                      */
    #define prmmg2_41       60 /* Maximale groentijd MG2 41                                                      */
    #define prmmlfpr02      61 /* Maximaal aantal modules vooruit fase 02                                        */
    #define prmmlfpr05      62 /* Maximaal aantal modules vooruit fase 05                                        */
    #define prmmlfpr08      63 /* Maximaal aantal modules vooruit fase 08                                        */
    #define prmmlfpr09      64 /* Maximaal aantal modules vooruit fase 09                                        */
    #define prmmlfpr41      65 /* Maximaal aantal modules vooruit fase 41                                        */
    #define prmaltg02       66 /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    #define prmaltp02       67 /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    #define prmaltg05       68 /* Minimale groentijd bij alternatieve realisatie fase 05                         */
    #define prmaltp05       69 /* Minimale ruimte tbv alternatieve realisatie fase 05                            */
    #define prmaltg08       70 /* Minimale groentijd bij alternatieve realisatie fase 08                         */
    #define prmaltp08       71 /* Minimale ruimte tbv alternatieve realisatie fase 08                            */
    #define prmaltg09       72 /* Minimale groentijd bij alternatieve realisatie fase 09                         */
    #define prmaltp09       73 /* Minimale ruimte tbv alternatieve realisatie fase 09                            */
    #define prmaltg41       74 /* Minimale groentijd bij alternatieve realisatie fase 41                         */
    #define prmaltp41       75 /* Minimale ruimte tbv alternatieve realisatie fase 41                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define prmtestdsivert  76 /* Testen vertraging in DSI bericht in testomgeving                               */
    #define prmtestdsilyn   77 /* Testen lijnnummer DSI bericht in testomgeving                                  */
    #define prmtestdsicat   78 /* Testen ritcategorie DSI bericht in testomgeving                                */
    #define PRMMAX1         79
#else
    #define PRMMAX1         76
#endif

/* Selectieve detectie */
/* ------------------- */
    #define dsdummy 0 /* Dummy SD lus 0: tbv KAR */
    #define DSMAX    1

    #define prioFC41bus 0
    #define prioFCMAX 1

/* modulen */
/* ------- */
    #define MLMAX1 5 /* aantal modulen */

/* Aantal perioden voor max groen */
/* ------- */
    #define MPERIODMAX 2 /* aantal groenperioden */

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined PRACTICE_TEST
    #define TESTOMGEVING
#endif

/* Gebruikers toevoegingen file includen */
/* ------------------------------------- */
    #include "GMsys.add"

