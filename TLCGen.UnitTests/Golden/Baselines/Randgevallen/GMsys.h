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
    #define fc22  1
    #define fc41  2
    #define fc61a 3
    #define FCMAX1 4 /* aantal fasecycli */

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
    #define usovinm61abus  (FCMAX + 10) /* Verklikken inmelding OV fase 61a           */
    #define usovinm61ahd   (FCMAX + 11) /* Verklikken inmelding OV fase 61a           */
    #define usovinm22fiets (FCMAX + 12) /* Verklikken inmelding OV fase 22            */
    #define usovinm41bus   (FCMAX + 13) /* Verklikken inmelding OV fase 41            */
    #define usperdef       (FCMAX + 14) /* Default periode actief                     */
    #define usper1         (FCMAX + 15) /* Periode actief                             */
    #define USMAX1         (FCMAX + 16)

/* detectie */
/* -------- */
    #define d021               0
    #define d022               1
    #define d221               2
    #define d222               3
    #define dk22               4
    #define d411               5
    #define d412               6
    #define d61a1              7
    #define d61a2              8
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define ddummykarin61abus  9
    #define ddummykarin61ahd   10
    #define ddummykaruit61abus 11
    #define ddummykaruit61ahd  12
    #define DPMAX1             13 /* aantal detectoren testomgeving */
#else
    #define DPMAX1             9 /* aantal detectoren automaat omgeving */
#endif

/* overige ingangen */
/* ---------------- */
    #define ISMAX1 (DPMAX + 0)

/* hulp elementen */
/* -------------- */
    #define hprio61abus             0 /* Bijhouden actief zijn prioriteit fase 61a         */
    #define hprioin61abus           1 /* Prioriteit inmelding fase 61a Bus                 */
    #define hpriouit61abus          2 /* Prioriteit uitmelding 61a Bus                     */
    #define hprioin61abuskar        3 /* Prioriteit inmelding fase 61a Bus                 */
    #define hpriouit61abuskar       4 /* Prioriteit uitmelding 61a Bus                     */
    #define hprio61ahd              5 /* Bijhouden actief zijn prioriteit fase 61a         */
    #define hprioin61ahd            6 /* Prioriteit inmelding fase 61a Nood- en hulpdienst */
    #define hpriouit61ahd           7 /* Prioriteit uitmelding 61a Nood- en hulpdienst     */
    #define hprioin61ahdkar         8 /* Prioriteit inmelding fase 61a Nood- en hulpdienst */
    #define hpriouit61ahdkar        9 /* Prioriteit uitmelding 61a Nood- en hulpdienst     */
    #define hprio22fiets           10 /* Bijhouden actief zijn prioriteit fase 22          */
    #define hprioin22fiets         11 /* Prioriteit inmelding fase 22 Fiets                */
    #define hpriouit22fiets        12 /* Prioriteit uitmelding 22 Fiets                    */
    #define hprioin22fietsfiets221 13 /* Prioriteit inmelding fase 22 Fiets                */
    #define hpriouit22fietsfiets   14 /* Prioriteit uitmelding 22 Fiets                    */
    #define hprio41bus             15 /* Bijhouden actief zijn prioriteit fase 41          */
    #define hprioin41bus           16 /* Prioriteit inmelding fase 41 Bus                  */
    #define hpriouit41bus          17 /* Prioriteit uitmelding 41 Bus                      */
    #define hprioin41busdet411     18 /* Prioriteit inmelding fase 41 Bus                  */
    #define hprioin41businpov41in  19 /* Prioriteit inmelding fase 41 Bus                  */
    #define hprioin41busvecio414   20 /* Prioriteit inmelding fase 41 Bus                  */
    #define hpriouit41busdet412    21 /* Prioriteit uitmelding 41 Bus                      */
    #define HEMAX1                 22

/* geheugen elementen */
/* ------------------ */
    #define mperiod 0 /* Onthouden actieve periode    */
    #define mar02   1 /* Alternatieve ruimte fase 02  */
    #define mar22   2 /* Alternatieve ruimte fase 22  */
    #define mar41   3 /* Alternatieve ruimte fase 41  */
    #define mar61a  4 /* Alternatieve ruimte fase 61a */
    #define MEMAX1  5

/* tijd elementen */
/* -------------- */
    #define tkm02          0 /* Kop maximum voor detector 02                           */
    #define tkm22          1 /* Kop maximum voor detector 22                           */
    #define tkm41          2 /* Kop maximum voor detector 41                           */
    #define tkm61a         3 /* Kop maximum voor detector 61a                          */
    #define tkarmelding    4 /* Duur verklikking ontvangst melding KAR                 */
    #define tkarog         5 /* Ondergedrag KAR                                        */
    #define tbtovg61abus   6 /* Timer bezettijd prioriteit gehinderde rijtijd fase 61a */
    #define trt61abus      7 /* Actuele rijtijd prio fase 61a                          */
    #define tgb61abus      8 /* Groenbewaking prioriteit fase 61a                      */
    #define tblk61abus     9 /* Blokkeertijd na prioriteitsingreep fase 61a            */
    #define tbtovg61ahd   10 /* Timer bezettijd prioriteit gehinderde rijtijd fase 61a */
    #define trt61ahd      11 /* Actuele rijtijd prio fase 61a                          */
    #define tgb61ahd      12 /* Groenbewaking prioriteit fase 61a                      */
    #define tblk61ahd     13 /* Blokkeertijd na prioriteitsingreep fase 61a            */
    #define tbtovg22fiets 14 /* Timer bezettijd prioriteit gehinderde rijtijd fase 22  */
    #define trt22fiets    15 /* Actuele rijtijd prio fase 22                           */
    #define tgb22fiets    16 /* Groenbewaking prioriteit fase 22                       */
    #define tblk22fiets   17 /* Blokkeertijd na prioriteitsingreep fase 22             */
    #define tbtovg41bus   18 /* Timer bezettijd prioriteit gehinderde rijtijd fase 41  */
    #define trt41bus      19 /* Actuele rijtijd prio fase 41                           */
    #define tgb41bus      20 /* Groenbewaking prioriteit fase 41                       */
    #define tblk41bus     21 /* Blokkeertijd na prioriteitsingreep fase 41             */
    #define TMMAX1        22

/* teller elementen */
/* ---------------- */
    #define cvc61abus           0 /* Bijhouden prio inmeldingen fase 61a type Bus                 */
    #define cvc61ahd            1 /* Bijhouden prio inmeldingen fase 61a type Nood- en hulpdienst */
    #define cftscyc22fietsfiets 2 /* Bijhouden realisaties tbv peloton prio voor fase 22          */
    #define cvc22fiets          3 /* Bijhouden prio inmeldingen fase 22 type Fiets                */
    #define cvc41bus            4 /* Bijhouden prio inmeldingen fase 41 type Bus                  */
    #define CTMAX1              5

/* schakelaars */
/* ----------- */
    #define schcheckdstype              0 /* Check type DSI bericht bij VECOM               */
    #define schprioin61abuskar          1 /* Inmelden 61a via Bus toestaan                  */
    #define schpriouit61abuskar         2 /* Uitmelden 61a via Bus toestaan                 */
    #define schprioin61ahdkar           3 /* Inmelden 61a via Nood- en hulpdienst toestaan  */
    #define schpriouit61ahdkar          4 /* Uitmelden 61a via Nood- en hulpdienst toestaan */
    #define schprioin22fietsfiets221    5 /* Inmelden 22 via Fiets toestaan                 */
    #define schpriouit22fietsfiets      6 /* Uitmelden 22 via Fiets toestaan                */
    #define schprioin41busdet411SD      7 /* Inmelden 41 via Bus toestaan                   */
    #define schprioin41businpov41inSIS  8 /* Inmelden 41 via Bus toestaan                   */
    #define schprioin41busvecio414SD    9 /* Inmelden 41 via Bus toestaan                   */
    #define schpriouit41busdet412ED    10 /* Uitmelden 41 via Bus toestaan                  */
    #define schaltg02                  11 /* Alternatieve realisatie toestaan fase 02       */
    #define schaltg22                  12 /* Alternatieve realisatie toestaan fase 22       */
    #define schaltg41                  13 /* Alternatieve realisatie toestaan fase 41       */
    #define schaltg61a                 14 /* Alternatieve realisatie toestaan fase 61a      */
    #define SCHMAX1                    15

/* parameters */
/* ---------- */
    #define prmfb                        0 /* Instelling fasebewaking                                                         */
    #define prmxx                        1 /* Versiebeheer xx                                                                 */
    #define prmyy                        2 /* Versiebeheer yy                                                                 */
    #define prmzz                        3 /* Versiebeheer zz                                                                 */
    #define prmmk021                     4 /* Meetkriterium type voor detector 021                                            */
    #define prmmk022                     5 /* Meetkriterium type voor detector 022                                            */
    #define prmmk221                     6 /* Meetkriterium type voor detector 221                                            */
    #define prmmk222                     7 /* Meetkriterium type voor detector 222                                            */
    #define prmmkk22                     8 /* Meetkriterium type voor detector k22                                            */
    #define prmmk411                     9 /* Meetkriterium type voor detector 411                                            */
    #define prmmk412                    10 /* Meetkriterium type voor detector 412                                            */
    #define prmmk61a1                   11 /* Meetkriterium type voor detector 61a1                                           */
    #define prmmk61a2                   12 /* Meetkriterium type voor detector 61a2                                           */
    #define prmmwta                     13 /* Maximale wachttijd autoverkeer                                                  */
    #define prmmwtfts                   14 /* Maximale wachttijd fiets                                                        */
    #define prmmwtvtg                   15 /* Maximale wachttijd voetgangers                                                  */
    #define prmpmgt02                   16 /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                   */
    #define prmognt02                   17 /* Minimale groentijd bij terugkomen fase 02                                       */
    #define prmnofm02                   18 /* Aantal malen niet afkappen na OV ingreep fase 02                                */
    #define prmmgcov02                  19 /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep             */
    #define prmpmgcov02                 20 /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep  */
    #define prmohpmg02                  21 /* Percentage ophogen groentijd na afkappen fase 02                                */
    #define prmpmgt22                   22 /* Minimaal percentage groentijd primair tbv. terugkomen fase 22                   */
    #define prmognt22                   23 /* Minimale groentijd bij terugkomen fase 22                                       */
    #define prmnofm22                   24 /* Aantal malen niet afkappen na OV ingreep fase 22                                */
    #define prmmgcov22                  25 /* Minimum groentijd waarna fase 22 afgkapt mag worden door OV ingreep             */
    #define prmpmgcov22                 26 /* Minimum percentage groentijd waarna fase 22 afgkapt mag worden door OV ingreep  */
    #define prmohpmg22                  27 /* Percentage ophogen groentijd na afkappen fase 22                                */
    #define prmpmgt41                   28 /* Minimaal percentage groentijd primair tbv. terugkomen fase 41                   */
    #define prmognt41                   29 /* Minimale groentijd bij terugkomen fase 41                                       */
    #define prmnofm41                   30 /* Aantal malen niet afkappen na OV ingreep fase 41                                */
    #define prmmgcov41                  31 /* Minimum groentijd waarna fase 41 afgkapt mag worden door OV ingreep             */
    #define prmpmgcov41                 32 /* Minimum percentage groentijd waarna fase 41 afgkapt mag worden door OV ingreep  */
    #define prmohpmg41                  33 /* Percentage ophogen groentijd na afkappen fase 41                                */
    #define prmpmgt61a                  34 /* Minimaal percentage groentijd primair tbv. terugkomen fase 61a                  */
    #define prmognt61a                  35 /* Minimale groentijd bij terugkomen fase 61a                                      */
    #define prmnofm61a                  36 /* Aantal malen niet afkappen na OV ingreep fase 61a                               */
    #define prmmgcov61a                 37 /* Minimum groentijd waarna fase 61a afgkapt mag worden door OV ingreep            */
    #define prmpmgcov61a                38 /* Minimum percentage groentijd waarna fase 61a afgkapt mag worden door OV ingreep */
    #define prmohpmg61a                 39 /* Percentage ophogen groentijd na afkappen fase 61a                               */
    #define prmrto61abus                40 /* Ongehinderde rijtijd prioriteit fase 61a                                        */
    #define prmrtbg61abus               41 /* Beperkt gehinderde rijtijd prioriteit fase 61a                                  */
    #define prmrtg61abus                42 /* Gehinderde rijtijd prioriteit fase 61a                                          */
    #define prmomx61abus                43 /* Ondermaximum OV fase 61a                                                        */
    #define prmupinagb61abus            44 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 61a                  */
    #define prmvtgcat61abus             45 /* Voertuigcategorie DSI voor prio ingreep 61abus                                  */
    #define prmprio61abus               46 /* Prioriteitsinstelling OV fase 61a                                               */
    #define prmrto61ahd                 47 /* Ongehinderde rijtijd prioriteit fase 61a                                        */
    #define prmrtbg61ahd                48 /* Beperkt gehinderde rijtijd prioriteit fase 61a                                  */
    #define prmrtg61ahd                 49 /* Gehinderde rijtijd prioriteit fase 61a                                          */
    #define prmomx61ahd                 50 /* Ondermaximum OV fase 61a                                                        */
    #define prmupinagb61ahd             51 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 61a                  */
    #define prmvtgcat61ahd              52 /* Voertuigcategorie DSI voor prio ingreep 61ahd                                   */
    #define prmprio61ahd                53 /* Prioriteitsinstelling OV fase 61a                                               */
    #define prmftsblok22fietsfiets      54 /* Blokken waarin fiets peloton prio actief mag zijn voor fase 22                  */
    #define prmftsmaxpercyc22fietsfiets 55 /* Maximaal aantal keer fiets peloton prio per cyclus voor fase 22                 */
    #define prmftsminwt22fietsfiets     56 /* Minimale wachttijd tbv peloton prio voor fase 22                                */
    #define prmrto22fiets               57 /* Ongehinderde rijtijd prioriteit fase 22                                         */
    #define prmrtbg22fiets              58 /* Beperkt gehinderde rijtijd prioriteit fase 22                                   */
    #define prmrtg22fiets               59 /* Gehinderde rijtijd prioriteit fase 22                                           */
    #define prmomx22fiets               60 /* Ondermaximum OV fase 22                                                         */
    #define prmupinagb22fiets           61 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 22                   */
    #define prmprio22fiets              62 /* Prioriteitsinstelling OV fase 22                                                */
    #define prmrto41bus                 63 /* Ongehinderde rijtijd prioriteit fase 41                                         */
    #define prmrtbg41bus                64 /* Beperkt gehinderde rijtijd prioriteit fase 41                                   */
    #define prmrtg41bus                 65 /* Gehinderde rijtijd prioriteit fase 41                                           */
    #define prmomx41bus                 66 /* Ondermaximum OV fase 41                                                         */
    #define prmupinagb41bus             67 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                   */
    #define prmprio41bus                68 /* Prioriteitsinstelling OV fase 41                                                */
    #define prmstkp1                    69 /* Start klokperiode ochtend                                                       */
    #define prmetkp1                    70 /* Einde klokperiode ochtend                                                       */
    #define prmdckp1                    71 /* Dagsoort klokperiode ochtend                                                    */
    #define prmmg1_02                   72 /* Maximale groentijd MG1 02                                                       */
    #define prmmg1_22                   73 /* Maximale groentijd MG1 22                                                       */
    #define prmmg1_41                   74 /* Maximale groentijd MG1 41                                                       */
    #define prmmg1_61a                  75 /* Maximale groentijd MG1 61a                                                      */
    #define prmmg2_02                   76 /* Maximale groentijd MG2 02                                                       */
    #define prmmg2_22                   77 /* Maximale groentijd MG2 22                                                       */
    #define prmmg2_41                   78 /* Maximale groentijd MG2 41                                                       */
    #define prmmg2_61a                  79 /* Maximale groentijd MG2 61a                                                      */
    #define prmmlfpr02                  80 /* Maximaal aantal modules vooruit fase 02                                         */
    #define prmmlfpr22                  81 /* Maximaal aantal modules vooruit fase 22                                         */
    #define prmmlfpr41                  82 /* Maximaal aantal modules vooruit fase 41                                         */
    #define prmmlfpr61a                 83 /* Maximaal aantal modules vooruit fase 61a                                        */
    #define prmaltg02                   84 /* Minimale groentijd bij alternatieve realisatie fase 02                          */
    #define prmaltp02                   85 /* Minimale ruimte tbv alternatieve realisatie fase 02                             */
    #define prmaltg22                   86 /* Minimale groentijd bij alternatieve realisatie fase 22                          */
    #define prmaltp22                   87 /* Minimale ruimte tbv alternatieve realisatie fase 22                             */
    #define prmaltg41                   88 /* Minimale groentijd bij alternatieve realisatie fase 41                          */
    #define prmaltp41                   89 /* Minimale ruimte tbv alternatieve realisatie fase 41                             */
    #define prmaltg61a                  90 /* Minimale groentijd bij alternatieve realisatie fase 61a                         */
    #define prmaltp61a                  91 /* Minimale ruimte tbv alternatieve realisatie fase 61a                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define prmtestdsivert              92 /* Testen vertraging in DSI bericht in testomgeving                                */
    #define prmtestdsilyn               93 /* Testen lijnnummer DSI bericht in testomgeving                                   */
    #define prmtestdsicat               94 /* Testen ritcategorie DSI bericht in testomgeving                                 */
    #define PRMMAX1                     95
#else
    #define PRMMAX1                     92
#endif

/* Selectieve detectie */
/* ------------------- */
    #define dsdummy 0 /* Dummy SD lus 0: tbv KAR */
    #define DSMAX    1

    #define prioFC61abus 0
    #define prioFC61ahd 1
    #define prioFC22fiets 2
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

