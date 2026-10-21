/* ALGEMENE APPLICATIEFILE */
/* ----------------------- */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMsys.h
      CCOL:   9.0
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
    #define fc22  2
    #define fc41  3
    #define fc42  4
    #define fc61  5
    #define FCMAX1 6 /* aantal fasecycli */

/* overige uitgangen */
/* ----------------- */
    #define ussegm1        (FCMAX +  0) /* Aansturing segmenten display               */
    #define ussegm2        (FCMAX +  1) /* Aansturing segmenten display               */
    #define ussegm3        (FCMAX +  2) /* Aansturing segmenten display               */
    #define ussegm4        (FCMAX +  3) /* Aansturing segmenten display               */
    #define ussegm5        (FCMAX +  4) /* Aansturing segmenten display               */
    #define ussegm6        (FCMAX +  5) /* Aansturing segmenten display               */
    #define ussegm7        (FCMAX +  6) /* Aansturing segmenten display               */
    #define usmaxwt        (FCMAX +  7) /* Verklikken maximale wachttijd overschreden */
    #define uskarmelding   (FCMAX +  8) /* Verklikken ontvangst melding KAR           */
    #define uskarog        (FCMAX +  9) /* Verklikken ondergedrag KAR                 */
    #define usovinm41tram  (FCMAX + 10) /* Verklikken inmelding OV fase 41            */
    #define usovinm41bus   (FCMAX + 11) /* Verklikken inmelding OV fase 41            */
    #define usovinm42bus   (FCMAX + 12) /* Verklikken inmelding OV fase 42            */
    #define usovinm22fiets (FCMAX + 13) /* Verklikken inmelding OV fase 22            */
    #define usovinm02vrw   (FCMAX + 14) /* Verklikken inmelding OV fase 02            */
    #define usovinm05auto  (FCMAX + 15) /* Verklikken inmelding OV fase 05            */
    #define usovinm05ng    (FCMAX + 16) /* Verklikken inmelding OV fase 05            */
    #define usovinm61hd    (FCMAX + 17) /* Verklikken inmelding OV fase 61            */
    #define usperdef       (FCMAX + 18) /* Default periode actief                     */
    #define usper1         (FCMAX + 19) /* Periode actief                             */
    #define USMAX1         (FCMAX + 20)

/* detectie */
/* -------- */
    #define d021                0
    #define d022                1
    #define d051                2
    #define d052                3
    #define d221                4
    #define d222                5
    #define dk22                6
    #define d411                7
    #define d412                8
    #define d421                9
    #define d422                10
    #define d611                11
    #define d612                12
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define ddummykarin41tram   13
    #define ddummykarin41bus    14
    #define ddummykarin42bus    15
    #define ddummykarin22fiets  16
    #define ddummykarin02vrw    17
    #define ddummykarin05auto   18
    #define ddummykarin05ng     19
    #define ddummykarin61hd     20
    #define ddummykaruit41tram  21
    #define ddummykaruit41bus   22
    #define ddummykaruit42bus   23
    #define ddummykaruit22fiets 24
    #define ddummykaruit02vrw   25
    #define ddummykaruit05auto  26
    #define ddummykaruit05ng    27
    #define ddummykaruit61hd    28
    #define DPMAX1              29 /* aantal detectoren testomgeving */
#else
    #define DPMAX1              13 /* aantal detectoren automaat omgeving */
#endif

/* overige ingangen */
/* ---------------- */
    #define ISMAX1 (DPMAX + 0)

/* hulp elementen */
/* -------------- */
    #define hprio41tram         0 /* Bijhouden actief zijn prioriteit fase 41         */
    #define hprioin41tram       1 /* Prioriteit inmelding fase 41 Tram                */
    #define hpriouit41tram      2 /* Prioriteit uitmelding 41 Tram                    */
    #define hprioin41tramkar    3 /* Prioriteit inmelding fase 41 Tram                */
    #define hpriouit41tramkar   4 /* Prioriteit uitmelding 41 Tram                    */
    #define hprio41bus          5 /* Bijhouden actief zijn prioriteit fase 41         */
    #define hprioin41bus        6 /* Prioriteit inmelding fase 41 Bus                 */
    #define hpriouit41bus       7 /* Prioriteit uitmelding 41 Bus                     */
    #define hprioin41buskar     8 /* Prioriteit inmelding fase 41 Bus                 */
    #define hpriouit41buskar    9 /* Prioriteit uitmelding 41 Bus                     */
    #define hprio42bus         10 /* Bijhouden actief zijn prioriteit fase 42         */
    #define hprioin42bus       11 /* Prioriteit inmelding fase 42 Bus                 */
    #define hpriouit42bus      12 /* Prioriteit uitmelding 42 Bus                     */
    #define hprioin42buskar    13 /* Prioriteit inmelding fase 42 Bus                 */
    #define hpriouit42buskar   14 /* Prioriteit uitmelding 42 Bus                     */
    #define hprio22fiets       15 /* Bijhouden actief zijn prioriteit fase 22         */
    #define hprioin22fiets     16 /* Prioriteit inmelding fase 22 Fiets               */
    #define hpriouit22fiets    17 /* Prioriteit uitmelding 22 Fiets                   */
    #define hprioin22fietskar  18 /* Prioriteit inmelding fase 22 Fiets               */
    #define hpriouit22fietskar 19 /* Prioriteit uitmelding 22 Fiets                   */
    #define hprio02vrw         20 /* Bijhouden actief zijn prioriteit fase 02         */
    #define hprioin02vrw       21 /* Prioriteit inmelding fase 02 Vrachtwagen         */
    #define hpriouit02vrw      22 /* Prioriteit uitmelding 02 Vrachtwagen             */
    #define hprioin02vrwkar    23 /* Prioriteit inmelding fase 02 Vrachtwagen         */
    #define hpriouit02vrwkar   24 /* Prioriteit uitmelding 02 Vrachtwagen             */
    #define hprio05auto        25 /* Bijhouden actief zijn prioriteit fase 05         */
    #define hprioin05auto      26 /* Prioriteit inmelding fase 05 Auto                */
    #define hpriouit05auto     27 /* Prioriteit uitmelding 05 Auto                    */
    #define hprioin05autokar   28 /* Prioriteit inmelding fase 05 Auto                */
    #define hpriouit05autokar  29 /* Prioriteit uitmelding 05 Auto                    */
    #define hprio05ng          30 /* Bijhouden actief zijn prioriteit fase 05         */
    #define hprioin05ng        31 /* Prioriteit inmelding fase 05 NG                  */
    #define hpriouit05ng       32 /* Prioriteit uitmelding 05 NG                      */
    #define hprioin05ngkar     33 /* Prioriteit inmelding fase 05 NG                  */
    #define hpriouit05ngkar    34 /* Prioriteit uitmelding 05 NG                      */
    #define hprio61hd          35 /* Bijhouden actief zijn prioriteit fase 61         */
    #define hprioin61hd        36 /* Prioriteit inmelding fase 61 Nood- en hulpdienst */
    #define hpriouit61hd       37 /* Prioriteit uitmelding 61 Nood- en hulpdienst     */
    #define hprioin61hdkar     38 /* Prioriteit inmelding fase 61 Nood- en hulpdienst */
    #define hpriouit61hdkar    39 /* Prioriteit uitmelding 61 Nood- en hulpdienst     */
    #define HEMAX1             40

/* geheugen elementen */
/* ------------------ */
    #define mperiod 0 /* Onthouden actieve periode   */
    #define mar02   1 /* Alternatieve ruimte fase 02 */
    #define mar05   2 /* Alternatieve ruimte fase 05 */
    #define mar22   3 /* Alternatieve ruimte fase 22 */
    #define mar41   4 /* Alternatieve ruimte fase 41 */
    #define mar42   5 /* Alternatieve ruimte fase 42 */
    #define mar61   6 /* Alternatieve ruimte fase 61 */
    #define MEMAX1  7

/* tijd elementen */
/* -------------- */
    #define tkm02          0 /* Kop maximum voor detector 02                          */
    #define tkm05          1 /* Kop maximum voor detector 05                          */
    #define tkm22          2 /* Kop maximum voor detector 22                          */
    #define tkm41          3 /* Kop maximum voor detector 41                          */
    #define tkm42          4 /* Kop maximum voor detector 42                          */
    #define tkm61          5 /* Kop maximum voor detector 61                          */
    #define tkarmelding    6 /* Duur verklikking ontvangst melding KAR                */
    #define tkarog         7 /* Ondergedrag KAR                                       */
    #define tbtovg41tram   8 /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    #define trt41tram      9 /* Actuele rijtijd prio fase 41                          */
    #define tgb41tram     10 /* Groenbewaking prioriteit fase 41                      */
    #define tblk41tram    11 /* Blokkeertijd na prioriteitsingreep fase 41            */
    #define tbtovg41bus   12 /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    #define trt41bus      13 /* Actuele rijtijd prio fase 41                          */
    #define tgb41bus      14 /* Groenbewaking prioriteit fase 41                      */
    #define tblk41bus     15 /* Blokkeertijd na prioriteitsingreep fase 41            */
    #define tbtovg42bus   16 /* Timer bezettijd prioriteit gehinderde rijtijd fase 42 */
    #define trt42bus      17 /* Actuele rijtijd prio fase 42                          */
    #define tgb42bus      18 /* Groenbewaking prioriteit fase 42                      */
    #define tblk42bus     19 /* Blokkeertijd na prioriteitsingreep fase 42            */
    #define tbtovg22fiets 20 /* Timer bezettijd prioriteit gehinderde rijtijd fase 22 */
    #define trt22fiets    21 /* Actuele rijtijd prio fase 22                          */
    #define tgb22fiets    22 /* Groenbewaking prioriteit fase 22                      */
    #define tblk22fiets   23 /* Blokkeertijd na prioriteitsingreep fase 22            */
    #define tbtovg02vrw   24 /* Timer bezettijd prioriteit gehinderde rijtijd fase 02 */
    #define trt02vrw      25 /* Actuele rijtijd prio fase 02                          */
    #define tgb02vrw      26 /* Groenbewaking prioriteit fase 02                      */
    #define tblk02vrw     27 /* Blokkeertijd na prioriteitsingreep fase 02            */
    #define tbtovg05auto  28 /* Timer bezettijd prioriteit gehinderde rijtijd fase 05 */
    #define trt05auto     29 /* Actuele rijtijd prio fase 05                          */
    #define tgb05auto     30 /* Groenbewaking prioriteit fase 05                      */
    #define tblk05auto    31 /* Blokkeertijd na prioriteitsingreep fase 05            */
    #define tbtovg05ng    32 /* Timer bezettijd prioriteit gehinderde rijtijd fase 05 */
    #define trt05ng       33 /* Actuele rijtijd prio fase 05                          */
    #define tgb05ng       34 /* Groenbewaking prioriteit fase 05                      */
    #define tblk05ng      35 /* Blokkeertijd na prioriteitsingreep fase 05            */
    #define tbtovg61hd    36 /* Timer bezettijd prioriteit gehinderde rijtijd fase 61 */
    #define trt61hd       37 /* Actuele rijtijd prio fase 61                          */
    #define tgb61hd       38 /* Groenbewaking prioriteit fase 61                      */
    #define tblk61hd      39 /* Blokkeertijd na prioriteitsingreep fase 61            */
    #define TMMAX1        40

/* teller elementen */
/* ---------------- */
    #define cvc41tram  0 /* Bijhouden prio inmeldingen fase 41 type Tram                */
    #define cvc41bus   1 /* Bijhouden prio inmeldingen fase 41 type Bus                 */
    #define cvc42bus   2 /* Bijhouden prio inmeldingen fase 42 type Bus                 */
    #define cvc22fiets 3 /* Bijhouden prio inmeldingen fase 22 type Fiets               */
    #define cvc02vrw   4 /* Bijhouden prio inmeldingen fase 02 type Vrachtwagen         */
    #define cvc05auto  5 /* Bijhouden prio inmeldingen fase 05 type Auto                */
    #define cvc05ng    6 /* Bijhouden prio inmeldingen fase 05 type NG                  */
    #define cvc61hd    7 /* Bijhouden prio inmeldingen fase 61 type Nood- en hulpdienst */
    #define CTMAX1     8

/* schakelaars */
/* ----------- */
    #define schcheckdstype        0 /* Check type DSI bericht bij VECOM              */
    #define schprioin41tramkar    1 /* Inmelden 41 via Tram toestaan                 */
    #define schpriouit41tramkar   2 /* Uitmelden 41 via Tram toestaan                */
    #define schprioin41buskar     3 /* Inmelden 41 via Bus toestaan                  */
    #define schpriouit41buskar    4 /* Uitmelden 41 via Bus toestaan                 */
    #define schprioin42buskar     5 /* Inmelden 42 via Bus toestaan                  */
    #define schpriouit42buskar    6 /* Uitmelden 42 via Bus toestaan                 */
    #define schprioin22fietskar   7 /* Inmelden 22 via Fiets toestaan                */
    #define schpriouit22fietskar  8 /* Uitmelden 22 via Fiets toestaan               */
    #define schprioin02vrwkar     9 /* Inmelden 02 via Vrachtwagen toestaan          */
    #define schpriouit02vrwkar   10 /* Uitmelden 02 via Vrachtwagen toestaan         */
    #define schprioin05autokar   11 /* Inmelden 05 via Auto toestaan                 */
    #define schpriouit05autokar  12 /* Uitmelden 05 via Auto toestaan                */
    #define schprioin05ngkar     13 /* Inmelden 05 via NG toestaan                   */
    #define schpriouit05ngkar    14 /* Uitmelden 05 via NG toestaan                  */
    #define schprioin61hdkar     15 /* Inmelden 61 via Nood- en hulpdienst toestaan  */
    #define schpriouit61hdkar    16 /* Uitmelden 61 via Nood- en hulpdienst toestaan */
    #define schaltg02            17 /* Alternatieve realisatie toestaan fase 02      */
    #define schaltg05            18 /* Alternatieve realisatie toestaan fase 05      */
    #define schaltg22            19 /* Alternatieve realisatie toestaan fase 22      */
    #define schaltg41            20 /* Alternatieve realisatie toestaan fase 41      */
    #define schaltg42            21 /* Alternatieve realisatie toestaan fase 42      */
    #define schaltg61            22 /* Alternatieve realisatie toestaan fase 61      */
    #define SCHMAX1              23

/* parameters */
/* ---------- */
    #define prmfb               0 /* Instelling fasebewaking                                                        */
    #define prmxx               1 /* Versiebeheer xx                                                                */
    #define prmyy               2 /* Versiebeheer yy                                                                */
    #define prmzz               3 /* Versiebeheer zz                                                                */
    #define prmmk021            4 /* Meetkriterium type voor detector 021                                           */
    #define prmmk022            5 /* Meetkriterium type voor detector 022                                           */
    #define prmmk051            6 /* Meetkriterium type voor detector 051                                           */
    #define prmmk052            7 /* Meetkriterium type voor detector 052                                           */
    #define prmmk221            8 /* Meetkriterium type voor detector 221                                           */
    #define prmmk222            9 /* Meetkriterium type voor detector 222                                           */
    #define prmmkk22           10 /* Meetkriterium type voor detector k22                                           */
    #define prmmk411           11 /* Meetkriterium type voor detector 411                                           */
    #define prmmk412           12 /* Meetkriterium type voor detector 412                                           */
    #define prmmk421           13 /* Meetkriterium type voor detector 421                                           */
    #define prmmk422           14 /* Meetkriterium type voor detector 422                                           */
    #define prmmk611           15 /* Meetkriterium type voor detector 611                                           */
    #define prmmk612           16 /* Meetkriterium type voor detector 612                                           */
    #define prmmwta            17 /* Maximale wachttijd autoverkeer                                                 */
    #define prmmwtfts          18 /* Maximale wachttijd fiets                                                       */
    #define prmmwtvtg          19 /* Maximale wachttijd voetgangers                                                 */
    #define prmpmgt02          20 /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                  */
    #define prmognt02          21 /* Minimale groentijd bij terugkomen fase 02                                      */
    #define prmnofm02          22 /* Aantal malen niet afkappen na OV ingreep fase 02                               */
    #define prmmgcov02         23 /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov02        24 /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep */
    #define prmohpmg02         25 /* Percentage ophogen groentijd na afkappen fase 02                               */
    #define prmpmgt05          26 /* Minimaal percentage groentijd primair tbv. terugkomen fase 05                  */
    #define prmognt05          27 /* Minimale groentijd bij terugkomen fase 05                                      */
    #define prmnofm05          28 /* Aantal malen niet afkappen na OV ingreep fase 05                               */
    #define prmmgcov05         29 /* Minimum groentijd waarna fase 05 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov05        30 /* Minimum percentage groentijd waarna fase 05 afgkapt mag worden door OV ingreep */
    #define prmohpmg05         31 /* Percentage ophogen groentijd na afkappen fase 05                               */
    #define prmpmgt22          32 /* Minimaal percentage groentijd primair tbv. terugkomen fase 22                  */
    #define prmognt22          33 /* Minimale groentijd bij terugkomen fase 22                                      */
    #define prmnofm22          34 /* Aantal malen niet afkappen na OV ingreep fase 22                               */
    #define prmmgcov22         35 /* Minimum groentijd waarna fase 22 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov22        36 /* Minimum percentage groentijd waarna fase 22 afgkapt mag worden door OV ingreep */
    #define prmohpmg22         37 /* Percentage ophogen groentijd na afkappen fase 22                               */
    #define prmpmgt41          38 /* Minimaal percentage groentijd primair tbv. terugkomen fase 41                  */
    #define prmognt41          39 /* Minimale groentijd bij terugkomen fase 41                                      */
    #define prmnofm41          40 /* Aantal malen niet afkappen na OV ingreep fase 41                               */
    #define prmmgcov41         41 /* Minimum groentijd waarna fase 41 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov41        42 /* Minimum percentage groentijd waarna fase 41 afgkapt mag worden door OV ingreep */
    #define prmohpmg41         43 /* Percentage ophogen groentijd na afkappen fase 41                               */
    #define prmpmgt42          44 /* Minimaal percentage groentijd primair tbv. terugkomen fase 42                  */
    #define prmognt42          45 /* Minimale groentijd bij terugkomen fase 42                                      */
    #define prmnofm42          46 /* Aantal malen niet afkappen na OV ingreep fase 42                               */
    #define prmmgcov42         47 /* Minimum groentijd waarna fase 42 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov42        48 /* Minimum percentage groentijd waarna fase 42 afgkapt mag worden door OV ingreep */
    #define prmohpmg42         49 /* Percentage ophogen groentijd na afkappen fase 42                               */
    #define prmpmgt61          50 /* Minimaal percentage groentijd primair tbv. terugkomen fase 61                  */
    #define prmognt61          51 /* Minimale groentijd bij terugkomen fase 61                                      */
    #define prmnofm61          52 /* Aantal malen niet afkappen na OV ingreep fase 61                               */
    #define prmmgcov61         53 /* Minimum groentijd waarna fase 61 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov61        54 /* Minimum percentage groentijd waarna fase 61 afgkapt mag worden door OV ingreep */
    #define prmohpmg61         55 /* Percentage ophogen groentijd na afkappen fase 61                               */
    #define prmrto41tram       56 /* Ongehinderde rijtijd prioriteit fase 41                                        */
    #define prmrtbg41tram      57 /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    #define prmrtg41tram       58 /* Gehinderde rijtijd prioriteit fase 41                                          */
    #define prmomx41tram       59 /* Ondermaximum OV fase 41                                                        */
    #define prmupinagb41tram   60 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    #define prmvtgcat41tram    61 /* Voertuigcategorie DSI voor prio ingreep 41tram                                 */
    #define prmprio41tram      62 /* Prioriteitsinstelling OV fase 41                                               */
    #define prmrto41bus        63 /* Ongehinderde rijtijd prioriteit fase 41                                        */
    #define prmrtbg41bus       64 /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    #define prmrtg41bus        65 /* Gehinderde rijtijd prioriteit fase 41                                          */
    #define prmomx41bus        66 /* Ondermaximum OV fase 41                                                        */
    #define prmupinagb41bus    67 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    #define prmvtgcat41bus     68 /* Voertuigcategorie DSI voor prio ingreep 41bus                                  */
    #define prmprio41bus       69 /* Prioriteitsinstelling OV fase 41                                               */
    #define prmrto42bus        70 /* Ongehinderde rijtijd prioriteit fase 42                                        */
    #define prmrtbg42bus       71 /* Beperkt gehinderde rijtijd prioriteit fase 42                                  */
    #define prmrtg42bus        72 /* Gehinderde rijtijd prioriteit fase 42                                          */
    #define prmomx42bus        73 /* Ondermaximum OV fase 42                                                        */
    #define prmupinagb42bus    74 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 42                  */
    #define prmvtgcat42bus     75 /* Voertuigcategorie DSI voor prio ingreep 42bus                                  */
    #define prmprio42bus       76 /* Prioriteitsinstelling OV fase 42                                               */
    #define prmrto22fiets      77 /* Ongehinderde rijtijd prioriteit fase 22                                        */
    #define prmrtbg22fiets     78 /* Beperkt gehinderde rijtijd prioriteit fase 22                                  */
    #define prmrtg22fiets      79 /* Gehinderde rijtijd prioriteit fase 22                                          */
    #define prmomx22fiets      80 /* Ondermaximum OV fase 22                                                        */
    #define prmupinagb22fiets  81 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 22                  */
    #define prmvtgcat22fiets   82 /* Voertuigcategorie DSI voor prio ingreep 22fiets                                */
    #define prmprio22fiets     83 /* Prioriteitsinstelling OV fase 22                                               */
    #define prmrto02vrw        84 /* Ongehinderde rijtijd prioriteit fase 02                                        */
    #define prmrtbg02vrw       85 /* Beperkt gehinderde rijtijd prioriteit fase 02                                  */
    #define prmrtg02vrw        86 /* Gehinderde rijtijd prioriteit fase 02                                          */
    #define prmomx02vrw        87 /* Ondermaximum OV fase 02                                                        */
    #define prmupinagb02vrw    88 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 02                  */
    #define prmvtgcat02vrw     89 /* Voertuigcategorie DSI voor prio ingreep 02vrw                                  */
    #define prmprio02vrw       90 /* Prioriteitsinstelling OV fase 02                                               */
    #define prmrto05auto       91 /* Ongehinderde rijtijd prioriteit fase 05                                        */
    #define prmrtbg05auto      92 /* Beperkt gehinderde rijtijd prioriteit fase 05                                  */
    #define prmrtg05auto       93 /* Gehinderde rijtijd prioriteit fase 05                                          */
    #define prmomx05auto       94 /* Ondermaximum OV fase 05                                                        */
    #define prmupinagb05auto   95 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 05                  */
    #define prmvtgcat05auto    96 /* Voertuigcategorie DSI voor prio ingreep 05auto                                 */
    #define prmprio05auto      97 /* Prioriteitsinstelling OV fase 05                                               */
    #define prmrto05ng         98 /* Ongehinderde rijtijd prioriteit fase 05                                        */
    #define prmrtbg05ng        99 /* Beperkt gehinderde rijtijd prioriteit fase 05                                  */
    #define prmrtg05ng        100 /* Gehinderde rijtijd prioriteit fase 05                                          */
    #define prmomx05ng        101 /* Ondermaximum OV fase 05                                                        */
    #define prmupinagb05ng    102 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 05                  */
    #define prmvtgcat05ng     103 /* Voertuigcategorie DSI voor prio ingreep 05ng                                   */
    #define prmprio05ng       104 /* Prioriteitsinstelling OV fase 05                                               */
    #define prmrto61hd        105 /* Ongehinderde rijtijd prioriteit fase 61                                        */
    #define prmrtbg61hd       106 /* Beperkt gehinderde rijtijd prioriteit fase 61                                  */
    #define prmrtg61hd        107 /* Gehinderde rijtijd prioriteit fase 61                                          */
    #define prmomx61hd        108 /* Ondermaximum OV fase 61                                                        */
    #define prmupinagb61hd    109 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 61                  */
    #define prmvtgcat61hd     110 /* Voertuigcategorie DSI voor prio ingreep 61hd                                   */
    #define prmprio61hd       111 /* Prioriteitsinstelling OV fase 61                                               */
    #define prmstkp1          112 /* Start klokperiode ochtend                                                      */
    #define prmetkp1          113 /* Einde klokperiode ochtend                                                      */
    #define prmdckp1          114 /* Dagsoort klokperiode ochtend                                                   */
    #define prmmg1_02         115 /* Maximale groentijd MG1 02                                                      */
    #define prmmg1_05         116 /* Maximale groentijd MG1 05                                                      */
    #define prmmg1_22         117 /* Maximale groentijd MG1 22                                                      */
    #define prmmg1_41         118 /* Maximale groentijd MG1 41                                                      */
    #define prmmg1_42         119 /* Maximale groentijd MG1 42                                                      */
    #define prmmg1_61         120 /* Maximale groentijd MG1 61                                                      */
    #define prmmg2_02         121 /* Maximale groentijd MG2 02                                                      */
    #define prmmg2_05         122 /* Maximale groentijd MG2 05                                                      */
    #define prmmg2_22         123 /* Maximale groentijd MG2 22                                                      */
    #define prmmg2_41         124 /* Maximale groentijd MG2 41                                                      */
    #define prmmg2_42         125 /* Maximale groentijd MG2 42                                                      */
    #define prmmg2_61         126 /* Maximale groentijd MG2 61                                                      */
    #define prmmlfpr02        127 /* Maximaal aantal modules vooruit fase 02                                        */
    #define prmmlfpr05        128 /* Maximaal aantal modules vooruit fase 05                                        */
    #define prmmlfpr22        129 /* Maximaal aantal modules vooruit fase 22                                        */
    #define prmmlfpr41        130 /* Maximaal aantal modules vooruit fase 41                                        */
    #define prmmlfpr42        131 /* Maximaal aantal modules vooruit fase 42                                        */
    #define prmmlfpr61        132 /* Maximaal aantal modules vooruit fase 61                                        */
    #define prmaltg02         133 /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    #define prmaltp02         134 /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    #define prmaltg05         135 /* Minimale groentijd bij alternatieve realisatie fase 05                         */
    #define prmaltp05         136 /* Minimale ruimte tbv alternatieve realisatie fase 05                            */
    #define prmaltg22         137 /* Minimale groentijd bij alternatieve realisatie fase 22                         */
    #define prmaltp22         138 /* Minimale ruimte tbv alternatieve realisatie fase 22                            */
    #define prmaltg41         139 /* Minimale groentijd bij alternatieve realisatie fase 41                         */
    #define prmaltp41         140 /* Minimale ruimte tbv alternatieve realisatie fase 41                            */
    #define prmaltg42         141 /* Minimale groentijd bij alternatieve realisatie fase 42                         */
    #define prmaltp42         142 /* Minimale ruimte tbv alternatieve realisatie fase 42                            */
    #define prmaltg61         143 /* Minimale groentijd bij alternatieve realisatie fase 61                         */
    #define prmaltp61         144 /* Minimale ruimte tbv alternatieve realisatie fase 61                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define prmtestdsivert    145 /* Testen vertraging in DSI bericht in testomgeving                               */
    #define prmtestdsilyn     146 /* Testen lijnnummer DSI bericht in testomgeving                                  */
    #define prmtestdsicat     147 /* Testen ritcategorie DSI bericht in testomgeving                                */
    #define PRMMAX1           148
#else
    #define PRMMAX1           145
#endif

/* Selectieve detectie */
/* ------------------- */
    #define dsdummy 0 /* Dummy SD lus 0: tbv KAR */
    #define DSMAX    1

    #define prioFC41tram 0
    #define prioFC41bus 1
    #define prioFC42bus 2
    #define prioFC22fiets 3
    #define prioFC02vrw 4
    #define prioFC05auto 5
    #define prioFC05ng 6
    #define prioFC61hd 7
    #define prioFCMAX 8

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

