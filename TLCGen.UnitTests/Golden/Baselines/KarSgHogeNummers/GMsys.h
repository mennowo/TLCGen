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
    #define fc241 3
    #define FCMAX1 4 /* aantal fasecycli */

/* overige uitgangen */
/* ----------------- */
    #define ussegm1       (FCMAX +  0) /* Aansturing segmenten display               */
    #define ussegm2       (FCMAX +  1) /* Aansturing segmenten display               */
    #define ussegm3       (FCMAX +  2) /* Aansturing segmenten display               */
    #define ussegm4       (FCMAX +  3) /* Aansturing segmenten display               */
    #define ussegm5       (FCMAX +  4) /* Aansturing segmenten display               */
    #define ussegm6       (FCMAX +  5) /* Aansturing segmenten display               */
    #define ussegm7       (FCMAX +  6) /* Aansturing segmenten display               */
    #define usmaxwt       (FCMAX +  7) /* Verklikken maximale wachttijd overschreden */
    #define uskarmelding  (FCMAX +  8) /* Verklikken ontvangst melding KAR           */
    #define uskarog       (FCMAX +  9) /* Verklikken ondergedrag KAR                 */
    #define usovinm241bus (FCMAX + 10) /* Verklikken inmelding OV fase 241           */
    #define usovinm41bus  (FCMAX + 11) /* Verklikken inmelding OV fase 41            */
    #define usovinm05hd   (FCMAX + 12) /* Verklikken inmelding OV fase 05            */
    #define ushdinm02     (FCMAX + 13) /* Verklikken inmelding HD fase 02            */
    #define usperdef      (FCMAX + 14) /* Default periode actief                     */
    #define usper1        (FCMAX + 15) /* Periode actief                             */
    #define USMAX1        (FCMAX + 16)

/* detectie */
/* -------- */
    #define d021               0
    #define d022               1
    #define d051               2
    #define d052               3
    #define d411               4
    #define d412               5
    #define d2411              6
    #define d2412              7
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define ddummykarin241bus  8
    #define ddummykarin41bus   9
    #define ddummykarin05hd    10
    #define ddummykaruit241bus 11
    #define ddummykaruit41bus  12
    #define ddummykaruit05hd   13
    #define ddummyhdkarin02    14
    #define ddummyhdkaruit02   15
    #define DPMAX1             16 /* aantal detectoren testomgeving */
#else
    #define DPMAX1             8 /* aantal detectoren automaat omgeving */
#endif

/* overige ingangen */
/* ---------------- */
    #define ISMAX1 (DPMAX + 0)

/* hulp elementen */
/* -------------- */
    #define hprio241bus        0 /* Bijhouden actief zijn prioriteit fase 241        */
    #define hprioin241bus      1 /* Prioriteit inmelding fase 241 Bus                */
    #define hpriouit241bus     2 /* Prioriteit uitmelding 241 Bus                    */
    #define hprioin241buskar   3 /* Prioriteit inmelding fase 241 Bus                */
    #define hpriouit241buskar  4 /* Prioriteit uitmelding 241 Bus                    */
    #define hprio41bus         5 /* Bijhouden actief zijn prioriteit fase 41         */
    #define hprioin41bus       6 /* Prioriteit inmelding fase 41 Bus                 */
    #define hpriouit41bus      7 /* Prioriteit uitmelding 41 Bus                     */
    #define hprioin41buskar    8 /* Prioriteit inmelding fase 41 Bus                 */
    #define hpriouit41buskar   9 /* Prioriteit uitmelding 41 Bus                     */
    #define hprio05hd         10 /* Bijhouden actief zijn prioriteit fase 05         */
    #define hprioin05hd       11 /* Prioriteit inmelding fase 05 Nood- en hulpdienst */
    #define hpriouit05hd      12 /* Prioriteit uitmelding 05 Nood- en hulpdienst     */
    #define hprioin05hdkar    13 /* Prioriteit inmelding fase 05 Nood- en hulpdienst */
    #define hpriouit05hdkar   14 /* Prioriteit uitmelding 05 Nood- en hulpdienst     */
    #define hhd02             15 /* Bijhouden aanwezigheid HD fase 02                */
    #define hhdin02           16 /* HD inmelding 02                                  */
    #define hhduit02          17 /* HD uitmelding 02                                 */
    #define hhdin02kar        18 /* HD inmelding 02                                  */
    #define hhduit02kar       19 /* HD uitmelding 02                                 */
    #define HEMAX1            20

/* geheugen elementen */
/* ------------------ */
    #define mperiod 0 /* Onthouden actieve periode    */
    #define mar02   1 /* Alternatieve ruimte fase 02  */
    #define mar05   2 /* Alternatieve ruimte fase 05  */
    #define mar41   3 /* Alternatieve ruimte fase 41  */
    #define mar241  4 /* Alternatieve ruimte fase 241 */
    #define MEMAX1  5

/* tijd elementen */
/* -------------- */
    #define tkm02         0 /* Kop maximum voor detector 02                           */
    #define tkm05         1 /* Kop maximum voor detector 05                           */
    #define tkm41         2 /* Kop maximum voor detector 41                           */
    #define tkm241        3 /* Kop maximum voor detector 241                          */
    #define tkarmelding   4 /* Duur verklikking ontvangst melding KAR                 */
    #define tkarog        5 /* Ondergedrag KAR                                        */
    #define tbtovg241bus  6 /* Timer bezettijd prioriteit gehinderde rijtijd fase 241 */
    #define trt241bus     7 /* Actuele rijtijd prio fase 241                          */
    #define tgb241bus     8 /* Groenbewaking prioriteit fase 241                      */
    #define tblk241bus    9 /* Blokkeertijd na prioriteitsingreep fase 241            */
    #define tbtovg41bus  10 /* Timer bezettijd prioriteit gehinderde rijtijd fase 41  */
    #define trt41bus     11 /* Actuele rijtijd prio fase 41                           */
    #define tgb41bus     12 /* Groenbewaking prioriteit fase 41                       */
    #define tblk41bus    13 /* Blokkeertijd na prioriteitsingreep fase 41             */
    #define tbtovg05hd   14 /* Timer bezettijd prioriteit gehinderde rijtijd fase 05  */
    #define trt05hd      15 /* Actuele rijtijd prio fase 05                           */
    #define tgb05hd      16 /* Groenbewaking prioriteit fase 05                       */
    #define tblk05hd     17 /* Blokkeertijd na prioriteitsingreep fase 05             */
    #define tgbhd02      18 /* Groenbewaking HD fase 02                               */
    #define trthd02      19 /* Actuele rijtijd HD fase 02                             */
    #define tbtovg02hd   20 /* Timer bezettijd prioriteit gehinderde rijtijd fase 02  */
    #define thdin02kar   21 /* Anti jutter tijd inmelden HD 02 KAR                    */
    #define thduit02kar  22 /* Anti jutter tijd uitmelden HD 02                       */
    #define TMMAX1       23

/* teller elementen */
/* ---------------- */
    #define cvc241bus 0 /* Bijhouden prio inmeldingen fase 241 type Bus                */
    #define cvc41bus  1 /* Bijhouden prio inmeldingen fase 41 type Bus                 */
    #define cvc05hd   2 /* Bijhouden prio inmeldingen fase 05 type Nood- en hulpdienst */
    #define cvchd02   3 /* Bijhouden prio inmeldingen fase 02                          */
    #define CTMAX1    4

/* schakelaars */
/* ----------- */
    #define schcovuber           0 /* Weergeven wijzigingen PRIO_teller via CIF_UBER         */
    #define schcheckdstype       1 /* Check type DSI bericht bij VECOM                       */
    #define schprioin241buskar   2 /* Inmelden 241 via Bus toestaan                          */
    #define schpriouit241buskar  3 /* Uitmelden 241 via Bus toestaan                         */
    #define schprioin41buskar    4 /* Inmelden 41 via Bus toestaan                           */
    #define schpriouit41buskar   5 /* Uitmelden 41 via Bus toestaan                          */
    #define schprioin05hdkar     6 /* Inmelden 05 via Nood- en hulpdienst toestaan           */
    #define schpriouit05hdkar    7 /* Uitmelden 05 via Nood- en hulpdienst toestaan          */
    #define schhdin02kar         8 /* Inmelden 02 via KAR HD toestaan                        */
    #define schhduit02kar        9 /* Uitmelden 02 via KAR HD toestaan                       */
    #define schchecksirene02    10 /* Bij HD meldingen bij 02 via DSI controleren op CIF_SIR */
    #define schaltg02           11 /* Alternatieve realisatie toestaan fase 02               */
    #define schaltg05           12 /* Alternatieve realisatie toestaan fase 05               */
    #define schaltg41           13 /* Alternatieve realisatie toestaan fase 41               */
    #define schaltg241          14 /* Alternatieve realisatie toestaan fase 241              */
    #define SCHMAX1             15

/* parameters */
/* ---------- */
    #define prmfb             0 /* Instelling fasebewaking                                                         */
    #define prmxx             1 /* Versiebeheer xx                                                                 */
    #define prmyy             2 /* Versiebeheer yy                                                                 */
    #define prmzz             3 /* Versiebeheer zz                                                                 */
    #define prmmk021          4 /* Meetkriterium type voor detector 021                                            */
    #define prmmk022          5 /* Meetkriterium type voor detector 022                                            */
    #define prmmk051          6 /* Meetkriterium type voor detector 051                                            */
    #define prmmk052          7 /* Meetkriterium type voor detector 052                                            */
    #define prmmk411          8 /* Meetkriterium type voor detector 411                                            */
    #define prmmk412          9 /* Meetkriterium type voor detector 412                                            */
    #define prmmk2411        10 /* Meetkriterium type voor detector 2411                                           */
    #define prmmk2412        11 /* Meetkriterium type voor detector 2412                                           */
    #define prmmwta          12 /* Maximale wachttijd autoverkeer                                                  */
    #define prmmwtfts        13 /* Maximale wachttijd fiets                                                        */
    #define prmmwtvtg        14 /* Maximale wachttijd voetgangers                                                  */
    #define prmpmgt02        15 /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                   */
    #define prmognt02        16 /* Minimale groentijd bij terugkomen fase 02                                       */
    #define prmnofm02        17 /* Aantal malen niet afkappen na OV ingreep fase 02                                */
    #define prmmgcov02       18 /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep             */
    #define prmpmgcov02      19 /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep  */
    #define prmohpmg02       20 /* Percentage ophogen groentijd na afkappen fase 02                                */
    #define prmpmgt05        21 /* Minimaal percentage groentijd primair tbv. terugkomen fase 05                   */
    #define prmognt05        22 /* Minimale groentijd bij terugkomen fase 05                                       */
    #define prmnofm05        23 /* Aantal malen niet afkappen na OV ingreep fase 05                                */
    #define prmmgcov05       24 /* Minimum groentijd waarna fase 05 afgkapt mag worden door OV ingreep             */
    #define prmpmgcov05      25 /* Minimum percentage groentijd waarna fase 05 afgkapt mag worden door OV ingreep  */
    #define prmohpmg05       26 /* Percentage ophogen groentijd na afkappen fase 05                                */
    #define prmpmgt41        27 /* Minimaal percentage groentijd primair tbv. terugkomen fase 41                   */
    #define prmognt41        28 /* Minimale groentijd bij terugkomen fase 41                                       */
    #define prmnofm41        29 /* Aantal malen niet afkappen na OV ingreep fase 41                                */
    #define prmmgcov41       30 /* Minimum groentijd waarna fase 41 afgkapt mag worden door OV ingreep             */
    #define prmpmgcov41      31 /* Minimum percentage groentijd waarna fase 41 afgkapt mag worden door OV ingreep  */
    #define prmohpmg41       32 /* Percentage ophogen groentijd na afkappen fase 41                                */
    #define prmpmgt241       33 /* Minimaal percentage groentijd primair tbv. terugkomen fase 241                  */
    #define prmognt241       34 /* Minimale groentijd bij terugkomen fase 241                                      */
    #define prmnofm241       35 /* Aantal malen niet afkappen na OV ingreep fase 241                               */
    #define prmmgcov241      36 /* Minimum groentijd waarna fase 241 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov241     37 /* Minimum percentage groentijd waarna fase 241 afgkapt mag worden door OV ingreep */
    #define prmohpmg241      38 /* Percentage ophogen groentijd na afkappen fase 241                               */
    #define prmrto241bus     39 /* Ongehinderde rijtijd prioriteit fase 241                                        */
    #define prmrtbg241bus    40 /* Beperkt gehinderde rijtijd prioriteit fase 241                                  */
    #define prmrtg241bus     41 /* Gehinderde rijtijd prioriteit fase 241                                          */
    #define prmomx241bus     42 /* Ondermaximum OV fase 241                                                        */
    #define prmupinagb241bus 43 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 241                  */
    #define prmvtgcat241bus  44 /* Voertuigcategorie DSI voor prio ingreep 241bus                                  */
    #define prmprio241bus    45 /* Prioriteitsinstelling OV fase 241                                               */
    #define prmrto41bus      46 /* Ongehinderde rijtijd prioriteit fase 41                                         */
    #define prmrtbg41bus     47 /* Beperkt gehinderde rijtijd prioriteit fase 41                                   */
    #define prmrtg41bus      48 /* Gehinderde rijtijd prioriteit fase 41                                           */
    #define prmomx41bus      49 /* Ondermaximum OV fase 41                                                         */
    #define prmupinagb41bus  50 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                   */
    #define prmvtgcat41bus   51 /* Voertuigcategorie DSI voor prio ingreep 41bus                                   */
    #define prmprio41bus     52 /* Prioriteitsinstelling OV fase 41                                                */
    #define prmrto05hd       53 /* Ongehinderde rijtijd prioriteit fase 05                                         */
    #define prmrtbg05hd      54 /* Beperkt gehinderde rijtijd prioriteit fase 05                                   */
    #define prmrtg05hd       55 /* Gehinderde rijtijd prioriteit fase 05                                           */
    #define prmomx05hd       56 /* Ondermaximum OV fase 05                                                         */
    #define prmupinagb05hd   57 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 05                   */
    #define prmvtgcat05hd    58 /* Voertuigcategorie DSI voor prio ingreep 05hd                                    */
    #define prmprio05hd      59 /* Prioriteitsinstelling OV fase 05                                                */
    #define prmpriohd02      60 /* Prioriteitsinstelling HD fase 02                                                */
    #define prmrtohd02       61 /* Ongehinderde rijtijd HD fase 02                                                 */
    #define prmrtbghd02      62 /* Beperkt gehinderde rijtijd HD fase 02                                           */
    #define prmrtghd02       63 /* Gehinderde rijtijd HD fase 02                                                   */
    #define prmupinagbhd02   64 /* Selectieve detectie onbetrouwbaar na groenbewaking HD fase 02                   */
    #define prmstkp1         65 /* Start klokperiode ochtend                                                       */
    #define prmetkp1         66 /* Einde klokperiode ochtend                                                       */
    #define prmdckp1         67 /* Dagsoort klokperiode ochtend                                                    */
    #define prmmg1_02        68 /* Maximale groentijd MG1 02                                                       */
    #define prmmg1_05        69 /* Maximale groentijd MG1 05                                                       */
    #define prmmg1_41        70 /* Maximale groentijd MG1 41                                                       */
    #define prmmg1_241       71 /* Maximale groentijd MG1 241                                                      */
    #define prmmg2_02        72 /* Maximale groentijd MG2 02                                                       */
    #define prmmg2_05        73 /* Maximale groentijd MG2 05                                                       */
    #define prmmg2_41        74 /* Maximale groentijd MG2 41                                                       */
    #define prmmg2_241       75 /* Maximale groentijd MG2 241                                                      */
    #define prmmlfpr02       76 /* Maximaal aantal modules vooruit fase 02                                         */
    #define prmmlfpr05       77 /* Maximaal aantal modules vooruit fase 05                                         */
    #define prmmlfpr41       78 /* Maximaal aantal modules vooruit fase 41                                         */
    #define prmmlfpr241      79 /* Maximaal aantal modules vooruit fase 241                                        */
    #define prmaltg02        80 /* Minimale groentijd bij alternatieve realisatie fase 02                          */
    #define prmaltp02        81 /* Minimale ruimte tbv alternatieve realisatie fase 02                             */
    #define prmaltg05        82 /* Minimale groentijd bij alternatieve realisatie fase 05                          */
    #define prmaltp05        83 /* Minimale ruimte tbv alternatieve realisatie fase 05                             */
    #define prmaltg41        84 /* Minimale groentijd bij alternatieve realisatie fase 41                          */
    #define prmaltp41        85 /* Minimale ruimte tbv alternatieve realisatie fase 41                             */
    #define prmaltg241       86 /* Minimale groentijd bij alternatieve realisatie fase 241                         */
    #define prmaltp241       87 /* Minimale ruimte tbv alternatieve realisatie fase 241                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define prmtestdsivert   88 /* Testen vertraging in DSI bericht in testomgeving                                */
    #define prmtestdsilyn    89 /* Testen lijnnummer DSI bericht in testomgeving                                   */
    #define prmtestdsicat    90 /* Testen ritcategorie DSI bericht in testomgeving                                 */
    #define PRMMAX1          91
#else
    #define PRMMAX1          88
#endif

/* Selectieve detectie */
/* ------------------- */
    #define dsdummy 0 /* Dummy SD lus 0: tbv KAR */
    #define DSMAX    1

    #define prioFC241bus 0
    #define prioFC41bus 1
    #define prioFC05hd 2
    #define hdFC02 3
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

/* Gebruikers toevoegingen file includen */
/* ------------------------------------- */
    #include "GMsys.add"

