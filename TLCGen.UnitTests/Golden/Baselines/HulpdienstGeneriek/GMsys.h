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
    #define fc41  3
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
    #define usovinm02hd  (FCMAX + 10) /* Verklikken inmelding OV fase 02            */
    #define usovinm05hd  (FCMAX + 11) /* Verklikken inmelding OV fase 05            */
    #define usovinm08hd  (FCMAX + 12) /* Verklikken inmelding OV fase 08            */
    #define usovinm41bus (FCMAX + 13) /* Verklikken inmelding OV fase 41            */
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
    #define d411              6
    #define d412              7
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define ddummykarin02hd   8
    #define ddummykarin05hd   9
    #define ddummykarin41bus  10
    #define ddummykaruit02hd  11
    #define ddummykaruit05hd  12
    #define ddummykaruit41bus 13
    #define DPMAX1            14 /* aantal detectoren testomgeving */
#else
    #define DPMAX1            8 /* aantal detectoren automaat omgeving */
#endif

/* overige ingangen */
/* ---------------- */
    #define ISMAX1 (DPMAX + 0)

/* hulp elementen */
/* -------------- */
    #define hprio02hd            0 /* Bijhouden actief zijn prioriteit fase 02         */
    #define hprioin02hd          1 /* Prioriteit inmelding fase 02 Nood- en hulpdienst */
    #define hpriouit02hd         2 /* Prioriteit uitmelding 02 Nood- en hulpdienst     */
    #define hprioin02hdkar       3 /* Prioriteit inmelding fase 02 Nood- en hulpdienst */
    #define hpriouit02hdkar      4 /* Prioriteit uitmelding 02 Nood- en hulpdienst     */
    #define hprio05hd            5 /* Bijhouden actief zijn prioriteit fase 05         */
    #define hprioin05hd          6 /* Prioriteit inmelding fase 05 Nood- en hulpdienst */
    #define hpriouit05hd         7 /* Prioriteit uitmelding 05 Nood- en hulpdienst     */
    #define hprioin05hdkar       8 /* Prioriteit inmelding fase 05 Nood- en hulpdienst */
    #define hprioin05hdopti      9 /* Prioriteit inmelding fase 05 Opticom             */
    #define hpriouit05hdopti    10 /* Prioriteit uitmelding 05 Opticom                 */
    #define hprioin05hdopti051  11 /* Prioriteit inmelding fase 05 Nood- en hulpdienst */
    #define hpriouit05hdkar     12 /* Prioriteit uitmelding 05 Nood- en hulpdienst     */
    #define hpriouit05hdopti052 13 /* Prioriteit uitmelding 05 Nood- en hulpdienst     */
    #define hprio08hd           14 /* Bijhouden actief zijn prioriteit fase 08         */
    #define hprioin08hd         15 /* Prioriteit inmelding fase 08 Nood- en hulpdienst */
    #define hpriouit08hd        16 /* Prioriteit uitmelding 08 Nood- en hulpdienst     */
    #define hprioin08hddet081   17 /* Prioriteit inmelding fase 08 Nood- en hulpdienst */
    #define hpriouit08hddet082  18 /* Prioriteit uitmelding 08 Nood- en hulpdienst     */
    #define hprio41bus          19 /* Bijhouden actief zijn prioriteit fase 41         */
    #define hprioin41bus        20 /* Prioriteit inmelding fase 41 Bus                 */
    #define hpriouit41bus       21 /* Prioriteit uitmelding 41 Bus                     */
    #define hprioin41buskar     22 /* Prioriteit inmelding fase 41 Bus                 */
    #define hpriouit41buskar    23 /* Prioriteit uitmelding 41 Bus                     */
    #define HEMAX1              24

/* geheugen elementen */
/* ------------------ */
    #define mperiod 0 /* Onthouden actieve periode   */
    #define mar02   1 /* Alternatieve ruimte fase 02 */
    #define mar05   2 /* Alternatieve ruimte fase 05 */
    #define mar08   3 /* Alternatieve ruimte fase 08 */
    #define mar41   4 /* Alternatieve ruimte fase 41 */
    #define MEMAX1  5

/* tijd elementen */
/* -------------- */
    #define tkm02            0 /* Kop maximum voor detector 02                          */
    #define tkm05            1 /* Kop maximum voor detector 05                          */
    #define tkm08            2 /* Kop maximum voor detector 08                          */
    #define tkm41            3 /* Kop maximum voor detector 41                          */
    #define tkarmelding      4 /* Duur verklikking ontvangst melding KAR                */
    #define tkarog           5 /* Ondergedrag KAR                                       */
    #define tbtovg02hd       6 /* Timer bezettijd prioriteit gehinderde rijtijd fase 02 */
    #define trt02hd          7 /* Actuele rijtijd prio fase 02                          */
    #define tgb02hd          8 /* Groenbewaking prioriteit fase 02                      */
    #define tblk02hd         9 /* Blokkeertijd na prioriteitsingreep fase 02            */
    #define tprioin05hdopti 10 /* Anti jutter tijd inmelden 05 Opticom                  */
    #define tbtovg05hd      11 /* Timer bezettijd prioriteit gehinderde rijtijd fase 05 */
    #define trt05hd         12 /* Actuele rijtijd prio fase 05                          */
    #define tgb05hd         13 /* Groenbewaking prioriteit fase 05                      */
    #define tblk05hd        14 /* Blokkeertijd na prioriteitsingreep fase 05            */
    #define tbtovg08hd      15 /* Timer bezettijd prioriteit gehinderde rijtijd fase 08 */
    #define trt08hd         16 /* Actuele rijtijd prio fase 08                          */
    #define tgb08hd         17 /* Groenbewaking prioriteit fase 08                      */
    #define tblk08hd        18 /* Blokkeertijd na prioriteitsingreep fase 08            */
    #define tbtovg41bus     19 /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    #define trt41bus        20 /* Actuele rijtijd prio fase 41                          */
    #define tgb41bus        21 /* Groenbewaking prioriteit fase 41                      */
    #define tblk41bus       22 /* Blokkeertijd na prioriteitsingreep fase 41            */
    #define TMMAX1          23

/* teller elementen */
/* ---------------- */
    #define cvc02hd  0 /* Bijhouden prio inmeldingen fase 02 type Nood- en hulpdienst */
    #define cvc05hd  1 /* Bijhouden prio inmeldingen fase 05 type Nood- en hulpdienst */
    #define cvc08hd  2 /* Bijhouden prio inmeldingen fase 08 type Nood- en hulpdienst */
    #define cvc41bus 3 /* Bijhouden prio inmeldingen fase 41 type Bus                 */
    #define CTMAX1   4

/* schakelaars */
/* ----------- */
    #define schcheckdstype           0 /* Check type DSI bericht bij VECOM                                   */
    #define schprioin02hdkar         1 /* Inmelden 02 via Nood- en hulpdienst toestaan                       */
    #define schpriouit02hdkar        2 /* Uitmelden 02 via Nood- en hulpdienst toestaan                      */
    #define schchecksirene02hd       3 /* Bij HD meldingen bij 02 via DSI controleren op CIF_SIR             */
    #define schpriomr05hdmet02hd     4 /* Meerealisatie fase 05hd met hulpdienst ingreep Nood- en hulpdienst */
    #define schprioin05hdkar         5 /* Inmelden 05 via Nood- en hulpdienst toestaan                       */
    #define schprioin05hdopti        6 /* Inmelden 05 via Opticom toestaan                                   */
    #define schprioin05hdopti051DB   7 /* Inmelden 05 via Nood- en hulpdienst toestaan                       */
    #define schpriouit05hdkar        8 /* Uitmelden 05 via Nood- en hulpdienst toestaan                      */
    #define schpriouit05hdopti052DB  9 /* Uitmelden 05 via Nood- en hulpdienst toestaan                      */
    #define schpriomr02hdmet05hd    10 /* Meerealisatie fase 02hd met hulpdienst ingreep Nood- en hulpdienst */
    #define schprioin08hddet081SD   11 /* Inmelden 08 via Nood- en hulpdienst toestaan                       */
    #define schpriouit08hddet082ED  12 /* Uitmelden 08 via Nood- en hulpdienst toestaan                      */
    #define schprioin41buskar       13 /* Inmelden 41 via Bus toestaan                                       */
    #define schpriouit41buskar      14 /* Uitmelden 41 via Bus toestaan                                      */
    #define schaltg02               15 /* Alternatieve realisatie toestaan fase 02                           */
    #define schaltg05               16 /* Alternatieve realisatie toestaan fase 05                           */
    #define schaltg08               17 /* Alternatieve realisatie toestaan fase 08                           */
    #define schaltg41               18 /* Alternatieve realisatie toestaan fase 41                           */
    #define SCHMAX1                 19

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
    #define prmmk411        10 /* Meetkriterium type voor detector 411                                           */
    #define prmmk412        11 /* Meetkriterium type voor detector 412                                           */
    #define prmmwta         12 /* Maximale wachttijd autoverkeer                                                 */
    #define prmmwtfts       13 /* Maximale wachttijd fiets                                                       */
    #define prmmwtvtg       14 /* Maximale wachttijd voetgangers                                                 */
    #define prmpmgt02       15 /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                  */
    #define prmognt02       16 /* Minimale groentijd bij terugkomen fase 02                                      */
    #define prmnofm02       17 /* Aantal malen niet afkappen na OV ingreep fase 02                               */
    #define prmmgcov02      18 /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov02     19 /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep */
    #define prmohpmg02      20 /* Percentage ophogen groentijd na afkappen fase 02                               */
    #define prmpmgt05       21 /* Minimaal percentage groentijd primair tbv. terugkomen fase 05                  */
    #define prmognt05       22 /* Minimale groentijd bij terugkomen fase 05                                      */
    #define prmnofm05       23 /* Aantal malen niet afkappen na OV ingreep fase 05                               */
    #define prmmgcov05      24 /* Minimum groentijd waarna fase 05 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov05     25 /* Minimum percentage groentijd waarna fase 05 afgkapt mag worden door OV ingreep */
    #define prmohpmg05      26 /* Percentage ophogen groentijd na afkappen fase 05                               */
    #define prmpmgt08       27 /* Minimaal percentage groentijd primair tbv. terugkomen fase 08                  */
    #define prmognt08       28 /* Minimale groentijd bij terugkomen fase 08                                      */
    #define prmnofm08       29 /* Aantal malen niet afkappen na OV ingreep fase 08                               */
    #define prmmgcov08      30 /* Minimum groentijd waarna fase 08 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov08     31 /* Minimum percentage groentijd waarna fase 08 afgkapt mag worden door OV ingreep */
    #define prmohpmg08      32 /* Percentage ophogen groentijd na afkappen fase 08                               */
    #define prmpmgt41       33 /* Minimaal percentage groentijd primair tbv. terugkomen fase 41                  */
    #define prmognt41       34 /* Minimale groentijd bij terugkomen fase 41                                      */
    #define prmnofm41       35 /* Aantal malen niet afkappen na OV ingreep fase 41                               */
    #define prmmgcov41      36 /* Minimum groentijd waarna fase 41 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov41     37 /* Minimum percentage groentijd waarna fase 41 afgkapt mag worden door OV ingreep */
    #define prmohpmg41      38 /* Percentage ophogen groentijd na afkappen fase 41                               */
    #define prmrto02hd      39 /* Ongehinderde rijtijd prioriteit fase 02                                        */
    #define prmrtbg02hd     40 /* Beperkt gehinderde rijtijd prioriteit fase 02                                  */
    #define prmrtg02hd      41 /* Gehinderde rijtijd prioriteit fase 02                                          */
    #define prmomx02hd      42 /* Ondermaximum OV fase 02                                                        */
    #define prmupinagb02hd  43 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 02                  */
    #define prmvtgcat02hd   44 /* Voertuigcategorie DSI voor prio ingreep 02hd                                   */
    #define prmprio02hd     45 /* Prioriteitsinstelling OV fase 02                                               */
    #define prmrto05hd      46 /* Ongehinderde rijtijd prioriteit fase 05                                        */
    #define prmrtbg05hd     47 /* Beperkt gehinderde rijtijd prioriteit fase 05                                  */
    #define prmrtg05hd      48 /* Gehinderde rijtijd prioriteit fase 05                                          */
    #define prmomx05hd      49 /* Ondermaximum OV fase 05                                                        */
    #define prmupinagb05hd  50 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 05                  */
    #define prmvtgcat05hd   51 /* Voertuigcategorie DSI voor prio ingreep 05hd                                   */
    #define prmprio05hd     52 /* Prioriteitsinstelling OV fase 05                                               */
    #define prmrto08hd      53 /* Ongehinderde rijtijd prioriteit fase 08                                        */
    #define prmrtbg08hd     54 /* Beperkt gehinderde rijtijd prioriteit fase 08                                  */
    #define prmrtg08hd      55 /* Gehinderde rijtijd prioriteit fase 08                                          */
    #define prmomx08hd      56 /* Ondermaximum OV fase 08                                                        */
    #define prmupinagb08hd  57 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 08                  */
    #define prmprio08hd     58 /* Prioriteitsinstelling OV fase 08                                               */
    #define prmrto41bus     59 /* Ongehinderde rijtijd prioriteit fase 41                                        */
    #define prmrtbg41bus    60 /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    #define prmrtg41bus     61 /* Gehinderde rijtijd prioriteit fase 41                                          */
    #define prmomx41bus     62 /* Ondermaximum OV fase 41                                                        */
    #define prmupinagb41bus 63 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    #define prmvtgcat41bus  64 /* Voertuigcategorie DSI voor prio ingreep 41bus                                  */
    #define prmprio41bus    65 /* Prioriteitsinstelling OV fase 41                                               */
    #define prmstkp1        66 /* Start klokperiode ochtend                                                      */
    #define prmetkp1        67 /* Einde klokperiode ochtend                                                      */
    #define prmdckp1        68 /* Dagsoort klokperiode ochtend                                                   */
    #define prmmg1_02       69 /* Maximale groentijd MG1 02                                                      */
    #define prmmg1_05       70 /* Maximale groentijd MG1 05                                                      */
    #define prmmg1_08       71 /* Maximale groentijd MG1 08                                                      */
    #define prmmg1_41       72 /* Maximale groentijd MG1 41                                                      */
    #define prmmg2_02       73 /* Maximale groentijd MG2 02                                                      */
    #define prmmg2_05       74 /* Maximale groentijd MG2 05                                                      */
    #define prmmg2_08       75 /* Maximale groentijd MG2 08                                                      */
    #define prmmg2_41       76 /* Maximale groentijd MG2 41                                                      */
    #define prmmlfpr02      77 /* Maximaal aantal modules vooruit fase 02                                        */
    #define prmmlfpr05      78 /* Maximaal aantal modules vooruit fase 05                                        */
    #define prmmlfpr08      79 /* Maximaal aantal modules vooruit fase 08                                        */
    #define prmmlfpr41      80 /* Maximaal aantal modules vooruit fase 41                                        */
    #define prmaltg02       81 /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    #define prmaltp02       82 /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    #define prmaltg05       83 /* Minimale groentijd bij alternatieve realisatie fase 05                         */
    #define prmaltp05       84 /* Minimale ruimte tbv alternatieve realisatie fase 05                            */
    #define prmaltg08       85 /* Minimale groentijd bij alternatieve realisatie fase 08                         */
    #define prmaltp08       86 /* Minimale ruimte tbv alternatieve realisatie fase 08                            */
    #define prmaltg41       87 /* Minimale groentijd bij alternatieve realisatie fase 41                         */
    #define prmaltp41       88 /* Minimale ruimte tbv alternatieve realisatie fase 41                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define prmtestdsivert  89 /* Testen vertraging in DSI bericht in testomgeving                               */
    #define prmtestdsilyn   90 /* Testen lijnnummer DSI bericht in testomgeving                                  */
    #define prmtestdsicat   91 /* Testen ritcategorie DSI bericht in testomgeving                                */
    #define PRMMAX1         92
#else
    #define PRMMAX1         89
#endif

/* Selectieve detectie */
/* ------------------- */
    #define dsdummy 0 /* Dummy SD lus 0: tbv KAR */
    #define DSMAX    1

    #define prioFC02hd 0
    #define prioFC05hd 1
    #define prioFC08hd 2
    #define prioFC41bus 3
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

