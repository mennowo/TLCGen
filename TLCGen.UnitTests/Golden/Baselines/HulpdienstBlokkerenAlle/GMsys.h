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
    #define fc22  3
    #define fc31  4
    #define fc41  5
    #define FCMAX1 6 /* aantal fasecycli */

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
    #define usovinm02hd  (FCMAX + 10) /* Verklikken inmelding OV fase 02            */
    #define usovinm05hd  (FCMAX + 11) /* Verklikken inmelding OV fase 05            */
    #define usovinm41bus (FCMAX + 12) /* Verklikken inmelding OV fase 41            */
    #define ushdinm08    (FCMAX + 13) /* Verklikken inmelding HD fase 08            */
    #define usperdef     (FCMAX + 14) /* Default periode actief                     */
    #define usper1       (FCMAX + 15) /* Periode actief                             */
    #define USMAX1       (FCMAX + 16)

/* detectie */
/* -------- */
    #define d021              0
    #define d022              1
    #define d051              2
    #define d052              3
    #define d081              4
    #define d082              5
    #define d221              6
    #define d222              7
    #define dk22              8
    #define d311              9
    #define d312              10
    #define d411              11
    #define d412              12
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define ddummykarin02hd   13
    #define ddummykarin05hd   14
    #define ddummykarin41bus  15
    #define ddummykaruit02hd  16
    #define ddummykaruit05hd  17
    #define ddummykaruit41bus 18
    #define ddummyhdkarin08   19
    #define ddummyhdkaruit08  20
    #define DPMAX1            21 /* aantal detectoren testomgeving */
#else
    #define DPMAX1            13 /* aantal detectoren automaat omgeving */
#endif

/* overige ingangen */
/* ---------------- */
    #define ISMAX1 (DPMAX + 0)

/* hulp elementen */
/* -------------- */
    #define hprio02hd         0 /* Bijhouden actief zijn prioriteit fase 02         */
    #define hprioin02hd       1 /* Prioriteit inmelding fase 02 Nood- en hulpdienst */
    #define hpriouit02hd      2 /* Prioriteit uitmelding 02 Nood- en hulpdienst     */
    #define hprioin02hdkar    3 /* Prioriteit inmelding fase 02 Nood- en hulpdienst */
    #define hpriouit02hdkar   4 /* Prioriteit uitmelding 02 Nood- en hulpdienst     */
    #define hprio05hd         5 /* Bijhouden actief zijn prioriteit fase 05         */
    #define hprioin05hd       6 /* Prioriteit inmelding fase 05 Nood- en hulpdienst */
    #define hpriouit05hd      7 /* Prioriteit uitmelding 05 Nood- en hulpdienst     */
    #define hprioin05hdkar    8 /* Prioriteit inmelding fase 05 Nood- en hulpdienst */
    #define hpriouit05hdkar   9 /* Prioriteit uitmelding 05 Nood- en hulpdienst     */
    #define hprio41bus       10 /* Bijhouden actief zijn prioriteit fase 41         */
    #define hprioin41bus     11 /* Prioriteit inmelding fase 41 Bus                 */
    #define hpriouit41bus    12 /* Prioriteit uitmelding 41 Bus                     */
    #define hprioin41buskar  13 /* Prioriteit inmelding fase 41 Bus                 */
    #define hpriouit41buskar 14 /* Prioriteit uitmelding 41 Bus                     */
    #define hhd08            15 /* Bijhouden aanwezigheid HD fase 08                */
    #define hhdin08          16 /* HD inmelding 08                                  */
    #define hhduit08         17 /* HD uitmelding 08                                 */
    #define hhdin08kar       18 /* HD inmelding 08                                  */
    #define hhduit08kar      19 /* HD uitmelding 08                                 */
    #define HEMAX1           20

/* geheugen elementen */
/* ------------------ */
    #define mperiod 0 /* Onthouden actieve periode   */
    #define mar02   1 /* Alternatieve ruimte fase 02 */
    #define mar05   2 /* Alternatieve ruimte fase 05 */
    #define mar08   3 /* Alternatieve ruimte fase 08 */
    #define mar22   4 /* Alternatieve ruimte fase 22 */
    #define mar31   5 /* Alternatieve ruimte fase 31 */
    #define mar41   6 /* Alternatieve ruimte fase 41 */
    #define MEMAX1  7

/* tijd elementen */
/* -------------- */
    #define tkm02        0 /* Kop maximum voor detector 02                          */
    #define tkm05        1 /* Kop maximum voor detector 05                          */
    #define tkm08        2 /* Kop maximum voor detector 08                          */
    #define tkm22        3 /* Kop maximum voor detector 22                          */
    #define tkm31        4 /* Kop maximum voor detector 31                          */
    #define tkm41        5 /* Kop maximum voor detector 41                          */
    #define tkarmelding  6 /* Duur verklikking ontvangst melding KAR                */
    #define tkarog       7 /* Ondergedrag KAR                                       */
    #define tbtovg02hd   8 /* Timer bezettijd prioriteit gehinderde rijtijd fase 02 */
    #define trt02hd      9 /* Actuele rijtijd prio fase 02                          */
    #define tgb02hd     10 /* Groenbewaking prioriteit fase 02                      */
    #define tblk02hd    11 /* Blokkeertijd na prioriteitsingreep fase 02            */
    #define tbtovg05hd  12 /* Timer bezettijd prioriteit gehinderde rijtijd fase 05 */
    #define trt05hd     13 /* Actuele rijtijd prio fase 05                          */
    #define tgb05hd     14 /* Groenbewaking prioriteit fase 05                      */
    #define tblk05hd    15 /* Blokkeertijd na prioriteitsingreep fase 05            */
    #define tbtovg41bus 16 /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    #define trt41bus    17 /* Actuele rijtijd prio fase 41                          */
    #define tgb41bus    18 /* Groenbewaking prioriteit fase 41                      */
    #define tblk41bus   19 /* Blokkeertijd na prioriteitsingreep fase 41            */
    #define tgbhd08     20 /* Groenbewaking HD fase 08                              */
    #define trthd08     21 /* Actuele rijtijd HD fase 08                            */
    #define tbtovg08hd  22 /* Timer bezettijd prioriteit gehinderde rijtijd fase 08 */
    #define thdin08kar  23 /* Anti jutter tijd inmelden HD 08 KAR                   */
    #define thduit08kar 24 /* Anti jutter tijd uitmelden HD 08                      */
    #define TMMAX1      25

/* teller elementen */
/* ---------------- */
    #define cvc02hd  0 /* Bijhouden prio inmeldingen fase 02 type Nood- en hulpdienst */
    #define cvc05hd  1 /* Bijhouden prio inmeldingen fase 05 type Nood- en hulpdienst */
    #define cvc41bus 2 /* Bijhouden prio inmeldingen fase 41 type Bus                 */
    #define cvchd08  3 /* Bijhouden prio inmeldingen fase 08                          */
    #define CTMAX1   4

/* schakelaars */
/* ----------- */
    #define schcheckdstype      0 /* Check type DSI bericht bij VECOM                       */
    #define schprioin02hdkar    1 /* Inmelden 02 via Nood- en hulpdienst toestaan           */
    #define schpriouit02hdkar   2 /* Uitmelden 02 via Nood- en hulpdienst toestaan          */
    #define schprioin05hdkar    3 /* Inmelden 05 via Nood- en hulpdienst toestaan           */
    #define schpriouit05hdkar   4 /* Uitmelden 05 via Nood- en hulpdienst toestaan          */
    #define schprioin41buskar   5 /* Inmelden 41 via Bus toestaan                           */
    #define schpriouit41buskar  6 /* Uitmelden 41 via Bus toestaan                          */
    #define schhdin08kar        7 /* Inmelden 08 via KAR HD toestaan                        */
    #define schhduit08kar       8 /* Uitmelden 08 via KAR HD toestaan                       */
    #define schchecksirene08    9 /* Bij HD meldingen bij 08 via DSI controleren op CIF_SIR */
    #define schaltg02          10 /* Alternatieve realisatie toestaan fase 02               */
    #define schaltg05          11 /* Alternatieve realisatie toestaan fase 05               */
    #define schaltg08          12 /* Alternatieve realisatie toestaan fase 08               */
    #define schaltg22          13 /* Alternatieve realisatie toestaan fase 22               */
    #define schaltg31          14 /* Alternatieve realisatie toestaan fase 31               */
    #define schaltg41          15 /* Alternatieve realisatie toestaan fase 41               */
    #define SCHMAX1            16

/* parameters */
/* ---------- */
    #define prmfb             0 /* Instelling fasebewaking                                                        */
    #define prmxx             1 /* Versiebeheer xx                                                                */
    #define prmyy             2 /* Versiebeheer yy                                                                */
    #define prmzz             3 /* Versiebeheer zz                                                                */
    #define prmmk021          4 /* Meetkriterium type voor detector 021                                           */
    #define prmmk022          5 /* Meetkriterium type voor detector 022                                           */
    #define prmmk051          6 /* Meetkriterium type voor detector 051                                           */
    #define prmmk052          7 /* Meetkriterium type voor detector 052                                           */
    #define prmmk081          8 /* Meetkriterium type voor detector 081                                           */
    #define prmmk082          9 /* Meetkriterium type voor detector 082                                           */
    #define prmmk221         10 /* Meetkriterium type voor detector 221                                           */
    #define prmmk222         11 /* Meetkriterium type voor detector 222                                           */
    #define prmmkk22         12 /* Meetkriterium type voor detector k22                                           */
    #define prmmk311         13 /* Meetkriterium type voor detector 311                                           */
    #define prmmk312         14 /* Meetkriterium type voor detector 312                                           */
    #define prmmk411         15 /* Meetkriterium type voor detector 411                                           */
    #define prmmk412         16 /* Meetkriterium type voor detector 412                                           */
    #define prmmwta          17 /* Maximale wachttijd autoverkeer                                                 */
    #define prmmwtfts        18 /* Maximale wachttijd fiets                                                       */
    #define prmmwtvtg        19 /* Maximale wachttijd voetgangers                                                 */
    #define prmpmgt02        20 /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                  */
    #define prmognt02        21 /* Minimale groentijd bij terugkomen fase 02                                      */
    #define prmnofm02        22 /* Aantal malen niet afkappen na OV ingreep fase 02                               */
    #define prmmgcov02       23 /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov02      24 /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep */
    #define prmohpmg02       25 /* Percentage ophogen groentijd na afkappen fase 02                               */
    #define prmpmgt05        26 /* Minimaal percentage groentijd primair tbv. terugkomen fase 05                  */
    #define prmognt05        27 /* Minimale groentijd bij terugkomen fase 05                                      */
    #define prmnofm05        28 /* Aantal malen niet afkappen na OV ingreep fase 05                               */
    #define prmmgcov05       29 /* Minimum groentijd waarna fase 05 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov05      30 /* Minimum percentage groentijd waarna fase 05 afgkapt mag worden door OV ingreep */
    #define prmohpmg05       31 /* Percentage ophogen groentijd na afkappen fase 05                               */
    #define prmpmgt08        32 /* Minimaal percentage groentijd primair tbv. terugkomen fase 08                  */
    #define prmognt08        33 /* Minimale groentijd bij terugkomen fase 08                                      */
    #define prmnofm08        34 /* Aantal malen niet afkappen na OV ingreep fase 08                               */
    #define prmmgcov08       35 /* Minimum groentijd waarna fase 08 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov08      36 /* Minimum percentage groentijd waarna fase 08 afgkapt mag worden door OV ingreep */
    #define prmohpmg08       37 /* Percentage ophogen groentijd na afkappen fase 08                               */
    #define prmpmgt22        38 /* Minimaal percentage groentijd primair tbv. terugkomen fase 22                  */
    #define prmognt22        39 /* Minimale groentijd bij terugkomen fase 22                                      */
    #define prmnofm22        40 /* Aantal malen niet afkappen na OV ingreep fase 22                               */
    #define prmmgcov22       41 /* Minimum groentijd waarna fase 22 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov22      42 /* Minimum percentage groentijd waarna fase 22 afgkapt mag worden door OV ingreep */
    #define prmohpmg22       43 /* Percentage ophogen groentijd na afkappen fase 22                               */
    #define prmpmgt31        44 /* Minimaal percentage groentijd primair tbv. terugkomen fase 31                  */
    #define prmognt31        45 /* Minimale groentijd bij terugkomen fase 31                                      */
    #define prmpmgt41        46 /* Minimaal percentage groentijd primair tbv. terugkomen fase 41                  */
    #define prmognt41        47 /* Minimale groentijd bij terugkomen fase 41                                      */
    #define prmnofm41        48 /* Aantal malen niet afkappen na OV ingreep fase 41                               */
    #define prmmgcov41       49 /* Minimum groentijd waarna fase 41 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov41      50 /* Minimum percentage groentijd waarna fase 41 afgkapt mag worden door OV ingreep */
    #define prmohpmg41       51 /* Percentage ophogen groentijd na afkappen fase 41                               */
    #define prmrto02hd       52 /* Ongehinderde rijtijd prioriteit fase 02                                        */
    #define prmrtbg02hd      53 /* Beperkt gehinderde rijtijd prioriteit fase 02                                  */
    #define prmrtg02hd       54 /* Gehinderde rijtijd prioriteit fase 02                                          */
    #define prmomx02hd       55 /* Ondermaximum OV fase 02                                                        */
    #define prmupinagb02hd   56 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 02                  */
    #define prmvtgcat02hd    57 /* Voertuigcategorie DSI voor prio ingreep 02hd                                   */
    #define prmprio02hd      58 /* Prioriteitsinstelling OV fase 02                                               */
    #define prmrto05hd       59 /* Ongehinderde rijtijd prioriteit fase 05                                        */
    #define prmrtbg05hd      60 /* Beperkt gehinderde rijtijd prioriteit fase 05                                  */
    #define prmrtg05hd       61 /* Gehinderde rijtijd prioriteit fase 05                                          */
    #define prmomx05hd       62 /* Ondermaximum OV fase 05                                                        */
    #define prmupinagb05hd   63 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 05                  */
    #define prmvtgcat05hd    64 /* Voertuigcategorie DSI voor prio ingreep 05hd                                   */
    #define prmprio05hd      65 /* Prioriteitsinstelling OV fase 05                                               */
    #define prmrto41bus      66 /* Ongehinderde rijtijd prioriteit fase 41                                        */
    #define prmrtbg41bus     67 /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    #define prmrtg41bus      68 /* Gehinderde rijtijd prioriteit fase 41                                          */
    #define prmomx41bus      69 /* Ondermaximum OV fase 41                                                        */
    #define prmupinagb41bus  70 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    #define prmvtgcat41bus   71 /* Voertuigcategorie DSI voor prio ingreep 41bus                                  */
    #define prmprio41bus     72 /* Prioriteitsinstelling OV fase 41                                               */
    #define prmpriohd08      73 /* Prioriteitsinstelling HD fase 08                                               */
    #define prmrtohd08       74 /* Ongehinderde rijtijd HD fase 08                                                */
    #define prmrtbghd08      75 /* Beperkt gehinderde rijtijd HD fase 08                                          */
    #define prmrtghd08       76 /* Gehinderde rijtijd HD fase 08                                                  */
    #define prmupinagbhd08   77 /* Selectieve detectie onbetrouwbaar na groenbewaking HD fase 08                  */
    #define prmstkp1         78 /* Start klokperiode ochtend                                                      */
    #define prmetkp1         79 /* Einde klokperiode ochtend                                                      */
    #define prmdckp1         80 /* Dagsoort klokperiode ochtend                                                   */
    #define prmmg1_02        81 /* Maximale groentijd MG1 02                                                      */
    #define prmmg1_05        82 /* Maximale groentijd MG1 05                                                      */
    #define prmmg1_08        83 /* Maximale groentijd MG1 08                                                      */
    #define prmmg1_22        84 /* Maximale groentijd MG1 22                                                      */
    #define prmmg1_31        85 /* Maximale groentijd MG1 31                                                      */
    #define prmmg1_41        86 /* Maximale groentijd MG1 41                                                      */
    #define prmmg2_02        87 /* Maximale groentijd MG2 02                                                      */
    #define prmmg2_05        88 /* Maximale groentijd MG2 05                                                      */
    #define prmmg2_08        89 /* Maximale groentijd MG2 08                                                      */
    #define prmmg2_22        90 /* Maximale groentijd MG2 22                                                      */
    #define prmmg2_31        91 /* Maximale groentijd MG2 31                                                      */
    #define prmmg2_41        92 /* Maximale groentijd MG2 41                                                      */
    #define prmmlfpr02       93 /* Maximaal aantal modules vooruit fase 02                                        */
    #define prmmlfpr05       94 /* Maximaal aantal modules vooruit fase 05                                        */
    #define prmmlfpr08       95 /* Maximaal aantal modules vooruit fase 08                                        */
    #define prmmlfpr22       96 /* Maximaal aantal modules vooruit fase 22                                        */
    #define prmmlfpr31       97 /* Maximaal aantal modules vooruit fase 31                                        */
    #define prmmlfpr41       98 /* Maximaal aantal modules vooruit fase 41                                        */
    #define prmaltg02        99 /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    #define prmaltp02       100 /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    #define prmaltg05       101 /* Minimale groentijd bij alternatieve realisatie fase 05                         */
    #define prmaltp05       102 /* Minimale ruimte tbv alternatieve realisatie fase 05                            */
    #define prmaltg08       103 /* Minimale groentijd bij alternatieve realisatie fase 08                         */
    #define prmaltp08       104 /* Minimale ruimte tbv alternatieve realisatie fase 08                            */
    #define prmaltg22       105 /* Minimale groentijd bij alternatieve realisatie fase 22                         */
    #define prmaltp22       106 /* Minimale ruimte tbv alternatieve realisatie fase 22                            */
    #define prmaltg31       107 /* Minimale groentijd bij alternatieve realisatie fase 31                         */
    #define prmaltp31       108 /* Minimale ruimte tbv alternatieve realisatie fase 31                            */
    #define prmaltg41       109 /* Minimale groentijd bij alternatieve realisatie fase 41                         */
    #define prmaltp41       110 /* Minimale ruimte tbv alternatieve realisatie fase 41                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define prmtestdsivert  111 /* Testen vertraging in DSI bericht in testomgeving                               */
    #define prmtestdsilyn   112 /* Testen lijnnummer DSI bericht in testomgeving                                  */
    #define prmtestdsicat   113 /* Testen ritcategorie DSI bericht in testomgeving                                */
    #define PRMMAX1         114
#else
    #define PRMMAX1         111
#endif

/* Selectieve detectie */
/* ------------------- */
    #define dsdummy 0 /* Dummy SD lus 0: tbv KAR */
    #define DSMAX    1

    #define prioFC02hd 0
    #define prioFC05hd 1
    #define prioFC41bus 2
    #define hdFC08 3
    #define prioFCMAX 4

/* modulen */
/* ------- */
    #define MLMAX1 6 /* aantal modulen */

/* Aantal perioden voor max groen */
/* ------- */
    #define MPERIODMAX 2 /* aantal groenperioden */

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined PRACTICE_TEST
    #define TESTOMGEVING
#endif

/* Gebruikers toevoegingen file includen */
/* ------------------------------------- */
    #include "GMsys.add"

