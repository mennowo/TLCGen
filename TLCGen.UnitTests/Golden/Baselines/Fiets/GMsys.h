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
    #define fc24  2
    #define fc31  3
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
    #define usovinm22fiets (FCMAX +  8) /* Verklikken inmelding OV fase 22            */
    #define usovinm24fiets (FCMAX +  9) /* Verklikken inmelding OV fase 24            */
    #define usperdef       (FCMAX + 10) /* Default periode actief                     */
    #define usper1         (FCMAX + 11) /* Periode actief                             */
    #define USMAX1         (FCMAX + 12)

/* detectie */
/* -------- */
    #define d021 0
    #define d022 1
    #define d221 2
    #define d222 3
    #define dk22 4
    #define d241 5
    #define d242 6
    #define dk24 7
    #define d311 8
    #define d312 9
    #define DPMAX1 10 /* aantal detectoren */

/* overige ingangen */
/* ---------------- */
    #define ISMAX1 (DPMAX + 0)

/* hulp elementen */
/* -------------- */
    #define hprio22fiets             0 /* Bijhouden actief zijn prioriteit fase 22 */
    #define hprioin22fiets           1 /* Prioriteit inmelding fase 22 Fiets       */
    #define hpriouit22fiets          2 /* Prioriteit uitmelding 22 Fiets           */
    #define hprioin22fietsfiets221   3 /* Prioriteit inmelding fase 22 Fiets       */
    #define hpriouit22fietsfiets221  4 /* Prioriteit uitmelding 22 Fiets           */
    #define hprio24fiets             5 /* Bijhouden actief zijn prioriteit fase 24 */
    #define hprioin24fiets           6 /* Prioriteit inmelding fase 24 Fiets       */
    #define hpriouit24fiets          7 /* Prioriteit uitmelding 24 Fiets           */
    #define hprioin24fietsfiets      8 /* Prioriteit inmelding fase 24 Fiets       */
    #define hpriouit24fietsdet241    9 /* Prioriteit uitmelding 24 Fiets           */
    #define HEMAX1                  10

/* geheugen elementen */
/* ------------------ */
    #define mperiod 0 /* Onthouden actieve periode   */
    #define mar02   1 /* Alternatieve ruimte fase 02 */
    #define mar22   2 /* Alternatieve ruimte fase 22 */
    #define mar24   3 /* Alternatieve ruimte fase 24 */
    #define mar31   4 /* Alternatieve ruimte fase 31 */
    #define MEMAX1  5

/* tijd elementen */
/* -------------- */
    #define tkm02          0 /* Kop maximum voor detector 02                          */
    #define tkm22          1 /* Kop maximum voor detector 22                          */
    #define tkm24          2 /* Kop maximum voor detector 24                          */
    #define tkm31          3 /* Kop maximum voor detector 31                          */
    #define tbtovg22fiets  4 /* Timer bezettijd prioriteit gehinderde rijtijd fase 22 */
    #define trt22fiets     5 /* Actuele rijtijd prio fase 22                          */
    #define tgb22fiets     6 /* Groenbewaking prioriteit fase 22                      */
    #define tblk22fiets    7 /* Blokkeertijd na prioriteitsingreep fase 22            */
    #define tbtovg24fiets  8 /* Timer bezettijd prioriteit gehinderde rijtijd fase 24 */
    #define trt24fiets     9 /* Actuele rijtijd prio fase 24                          */
    #define tgb24fiets    10 /* Groenbewaking prioriteit fase 24                      */
    #define tblk24fiets   11 /* Blokkeertijd na prioriteitsingreep fase 24            */
    #define TMMAX1        12

/* teller elementen */
/* ---------------- */
    #define cftscyc22fietsfiets 0 /* Bijhouden realisaties tbv peloton prio voor fase 22     */
    #define cftsvtg22fietsfiets 1 /* Bijhouden aantal fietsers tbv peloton prio voor fase 22 */
    #define cvc22fiets          2 /* Bijhouden prio inmeldingen fase 22 type Fiets           */
    #define cftscyc24fietsfiets 3 /* Bijhouden realisaties tbv peloton prio voor fase 24     */
    #define cvc24fiets          4 /* Bijhouden prio inmeldingen fase 24 type Fiets           */
    #define CTMAX1              5

/* schakelaars */
/* ----------- */
    #define schcheckdstype             0 /* Check type DSI bericht bij VECOM         */
    #define schprioin22fietsfiets221   1 /* Inmelden 22 via Fiets toestaan           */
    #define schpriouit22fietsfiets221  2 /* Uitmelden 22 via Fiets toestaan          */
    #define schprioin24fietsfiets      3 /* Inmelden 24 via Fiets toestaan           */
    #define schpriouit24fietsdet241ED  4 /* Uitmelden 24 via Fiets toestaan          */
    #define schaltg02                  5 /* Alternatieve realisatie toestaan fase 02 */
    #define schaltg22                  6 /* Alternatieve realisatie toestaan fase 22 */
    #define schaltg24                  7 /* Alternatieve realisatie toestaan fase 24 */
    #define schaltg31                  8 /* Alternatieve realisatie toestaan fase 31 */
    #define SCHMAX1                    9

/* parameters */
/* ---------- */
    #define prmfb                        0 /* Instelling fasebewaking                                                        */
    #define prmxx                        1 /* Versiebeheer xx                                                                */
    #define prmyy                        2 /* Versiebeheer yy                                                                */
    #define prmzz                        3 /* Versiebeheer zz                                                                */
    #define prmmk021                     4 /* Meetkriterium type voor detector 021                                           */
    #define prmmk022                     5 /* Meetkriterium type voor detector 022                                           */
    #define prmmk221                     6 /* Meetkriterium type voor detector 221                                           */
    #define prmmk222                     7 /* Meetkriterium type voor detector 222                                           */
    #define prmmkk22                     8 /* Meetkriterium type voor detector k22                                           */
    #define prmmk241                     9 /* Meetkriterium type voor detector 241                                           */
    #define prmmk242                    10 /* Meetkriterium type voor detector 242                                           */
    #define prmmkk24                    11 /* Meetkriterium type voor detector k24                                           */
    #define prmmk311                    12 /* Meetkriterium type voor detector 311                                           */
    #define prmmk312                    13 /* Meetkriterium type voor detector 312                                           */
    #define prmmwta                     14 /* Maximale wachttijd autoverkeer                                                 */
    #define prmmwtfts                   15 /* Maximale wachttijd fiets                                                       */
    #define prmmwtvtg                   16 /* Maximale wachttijd voetgangers                                                 */
    #define prmpmgt02                   17 /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                  */
    #define prmognt02                   18 /* Minimale groentijd bij terugkomen fase 02                                      */
    #define prmnofm02                   19 /* Aantal malen niet afkappen na OV ingreep fase 02                               */
    #define prmmgcov02                  20 /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov02                 21 /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep */
    #define prmohpmg02                  22 /* Percentage ophogen groentijd na afkappen fase 02                               */
    #define prmpmgt22                   23 /* Minimaal percentage groentijd primair tbv. terugkomen fase 22                  */
    #define prmognt22                   24 /* Minimale groentijd bij terugkomen fase 22                                      */
    #define prmnofm22                   25 /* Aantal malen niet afkappen na OV ingreep fase 22                               */
    #define prmmgcov22                  26 /* Minimum groentijd waarna fase 22 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov22                 27 /* Minimum percentage groentijd waarna fase 22 afgkapt mag worden door OV ingreep */
    #define prmohpmg22                  28 /* Percentage ophogen groentijd na afkappen fase 22                               */
    #define prmpmgt24                   29 /* Minimaal percentage groentijd primair tbv. terugkomen fase 24                  */
    #define prmognt24                   30 /* Minimale groentijd bij terugkomen fase 24                                      */
    #define prmnofm24                   31 /* Aantal malen niet afkappen na OV ingreep fase 24                               */
    #define prmmgcov24                  32 /* Minimum groentijd waarna fase 24 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov24                 33 /* Minimum percentage groentijd waarna fase 24 afgkapt mag worden door OV ingreep */
    #define prmohpmg24                  34 /* Percentage ophogen groentijd na afkappen fase 24                               */
    #define prmpmgt31                   35 /* Minimaal percentage groentijd primair tbv. terugkomen fase 31                  */
    #define prmognt31                   36 /* Minimale groentijd bij terugkomen fase 31                                      */
    #define prmftsblok22fietsfiets      37 /* Blokken waarin fiets peloton prio actief mag zijn voor fase 22                 */
    #define prmftsmaxpercyc22fietsfiets 38 /* Maximaal aantal keer fiets peloton prio per cyclus voor fase 22                */
    #define prmftsminvtg22fietsfiets    39 /* Minimaal aantal gemeten fietsers tbv peloton prio voor fase 22                 */
    #define prmftsminwt22fietsfiets     40 /* Minimale wachttijd tbv peloton prio voor fase 22                               */
    #define prmrto22fiets               41 /* Ongehinderde rijtijd prioriteit fase 22                                        */
    #define prmrtbg22fiets              42 /* Beperkt gehinderde rijtijd prioriteit fase 22                                  */
    #define prmrtg22fiets               43 /* Gehinderde rijtijd prioriteit fase 22                                          */
    #define prmomx22fiets               44 /* Ondermaximum OV fase 22                                                        */
    #define prmupinagb22fiets           45 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 22                  */
    #define prmprio22fiets              46 /* Prioriteitsinstelling OV fase 22                                               */
    #define prmftsblok24fietsfiets      47 /* Blokken waarin fiets peloton prio actief mag zijn voor fase 24                 */
    #define prmftsmaxpercyc24fietsfiets 48 /* Maximaal aantal keer fiets peloton prio per cyclus voor fase 24                */
    #define prmftsminwt24fietsfiets     49 /* Minimale wachttijd tbv peloton prio voor fase 24                               */
    #define prmrto24fiets               50 /* Ongehinderde rijtijd prioriteit fase 24                                        */
    #define prmrtbg24fiets              51 /* Beperkt gehinderde rijtijd prioriteit fase 24                                  */
    #define prmrtg24fiets               52 /* Gehinderde rijtijd prioriteit fase 24                                          */
    #define prmomx24fiets               53 /* Ondermaximum OV fase 24                                                        */
    #define prmupinagb24fiets           54 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 24                  */
    #define prmprio24fiets              55 /* Prioriteitsinstelling OV fase 24                                               */
    #define prmstkp1                    56 /* Start klokperiode ochtend                                                      */
    #define prmetkp1                    57 /* Einde klokperiode ochtend                                                      */
    #define prmdckp1                    58 /* Dagsoort klokperiode ochtend                                                   */
    #define prmmg1_02                   59 /* Maximale groentijd MG1 02                                                      */
    #define prmmg1_22                   60 /* Maximale groentijd MG1 22                                                      */
    #define prmmg1_24                   61 /* Maximale groentijd MG1 24                                                      */
    #define prmmg1_31                   62 /* Maximale groentijd MG1 31                                                      */
    #define prmmg2_02                   63 /* Maximale groentijd MG2 02                                                      */
    #define prmmg2_22                   64 /* Maximale groentijd MG2 22                                                      */
    #define prmmg2_24                   65 /* Maximale groentijd MG2 24                                                      */
    #define prmmg2_31                   66 /* Maximale groentijd MG2 31                                                      */
    #define prmmlfpr02                  67 /* Maximaal aantal modules vooruit fase 02                                        */
    #define prmmlfpr22                  68 /* Maximaal aantal modules vooruit fase 22                                        */
    #define prmmlfpr24                  69 /* Maximaal aantal modules vooruit fase 24                                        */
    #define prmmlfpr31                  70 /* Maximaal aantal modules vooruit fase 31                                        */
    #define prmaltg02                   71 /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    #define prmaltp02                   72 /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    #define prmaltg22                   73 /* Minimale groentijd bij alternatieve realisatie fase 22                         */
    #define prmaltp22                   74 /* Minimale ruimte tbv alternatieve realisatie fase 22                            */
    #define prmaltg24                   75 /* Minimale groentijd bij alternatieve realisatie fase 24                         */
    #define prmaltp24                   76 /* Minimale ruimte tbv alternatieve realisatie fase 24                            */
    #define prmaltg31                   77 /* Minimale groentijd bij alternatieve realisatie fase 31                         */
    #define prmaltp31                   78 /* Minimale ruimte tbv alternatieve realisatie fase 31                            */
    #define PRMMAX1                     79

    #define prioFC22fiets 0
    #define prioFC24fiets 1
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

