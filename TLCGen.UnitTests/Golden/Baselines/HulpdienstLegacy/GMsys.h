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
    #define fc05  1
    #define fc08  2
    #define fc41  3
    #define FCMAX1 4 /* aantal fasecycli */

/* overige uitgangen */
/* ----------------- */
    #define ussegm1      (FCMAX +  0) /* Aansturing segmenten display                            */
    #define ussegm2      (FCMAX +  1) /* Aansturing segmenten display                            */
    #define ussegm3      (FCMAX +  2) /* Aansturing segmenten display                            */
    #define ussegm4      (FCMAX +  3) /* Aansturing segmenten display                            */
    #define ussegm5      (FCMAX +  4) /* Aansturing segmenten display                            */
    #define ussegm6      (FCMAX +  5) /* Aansturing segmenten display                            */
    #define ussegm7      (FCMAX +  6) /* Aansturing segmenten display                            */
    #define usincontrol  (FCMAX +  7) /* Verklikken of applicatie daadwerkelijk de TLC aanstuurt */
    #define usnocontrol  (FCMAX +  8) /* Verklikken of applicatie niet in staat is te regelen    */
    #define usmaxwt      (FCMAX +  9) /* Verklikken maximale wachttijd overschreden              */
    #define uskarmelding (FCMAX + 10) /* Verklikken ontvangst melding KAR                        */
    #define uskarog      (FCMAX + 11) /* Verklikken ondergedrag KAR                              */
    #define usovinm41bus (FCMAX + 12) /* Verklikken inmelding OV fase 41                         */
    #define ushdinm02    (FCMAX + 13) /* Verklikken inmelding HD fase 02                         */
    #define ushdinm05    (FCMAX + 14) /* Verklikken inmelding HD fase 05                         */
    #define ushdinm08    (FCMAX + 15) /* Verklikken inmelding HD fase 08                         */
    #define usperdef     (FCMAX + 16) /* Default periode actief                                  */
    #define usper1       (FCMAX + 17) /* Periode actief                                          */
    #define USMAX1       (FCMAX + 18)

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
    #define ddummykarin41bus  8
    #define ddummykaruit41bus 9
    #define ddummyhdkarin02   10
    #define ddummyhdkaruit02  11
    #define ddummyhdkarin05   12
    #define ddummyhdkaruit05  13
    #define DPMAX1            14 /* aantal detectoren testomgeving */
#else
    #define DPMAX1            8 /* aantal detectoren automaat omgeving */
#endif

/* overige ingangen */
/* ---------------- */
    #define ISMAX1 (DPMAX + 0)

/* hulp elementen */
/* -------------- */
    #define hprio41bus        0 /* Bijhouden actief zijn prioriteit fase 41 */
    #define hprioin41bus      1 /* Prioriteit inmelding fase 41 Bus         */
    #define hpriouit41bus     2 /* Prioriteit uitmelding 41 Bus             */
    #define hprioin41buskar   3 /* Prioriteit inmelding fase 41 Bus         */
    #define hpriouit41buskar  4 /* Prioriteit uitmelding 41 Bus             */
    #define hhd02             5 /* Bijhouden aanwezigheid HD fase 02        */
    #define hhdin02           6 /* HD inmelding 02                          */
    #define hhduit02          7 /* HD uitmelding 02                         */
    #define hhdin02kar        8 /* HD inmelding 02                          */
    #define hhduit02kar       9 /* HD uitmelding 02                         */
    #define hhd05            10 /* Bijhouden aanwezigheid HD fase 05        */
    #define hhdin05          11 /* HD inmelding 05                          */
    #define hhduit05         12 /* HD uitmelding 05                         */
    #define hhdin05kar       13 /* HD inmelding 05                          */
    #define hhduit05kar      14 /* HD uitmelding 05                         */
    #define hhdin05opt       15 /* HD inmelding 05                          */
    #define hhduit05opt      16 /* HD uitmelding 05                         */
    #define hhd08            17 /* Bijhouden aanwezigheid HD fase 08        */
    #define hhdin08          18 /* HD inmelding 08                          */
    #define hhduit08         19 /* HD uitmelding 08                         */
    #define hhdin08ris       20 /* HD inmelding 08                          */
    #define hhduit08ris      21 /* HD uitmelding 08                         */
    #define HEMAX1           22

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
    #define tkm02        0 /* Kop maximum voor detector 02                          */
    #define tkm05        1 /* Kop maximum voor detector 05                          */
    #define tkm08        2 /* Kop maximum voor detector 08                          */
    #define tkm41        3 /* Kop maximum voor detector 41                          */
    #define tkarmelding  4 /* Duur verklikking ontvangst melding KAR                */
    #define tkarog       5 /* Ondergedrag KAR                                       */
    #define tbtovg41bus  6 /* Timer bezettijd prioriteit gehinderde rijtijd fase 41 */
    #define trt41bus     7 /* Actuele rijtijd prio fase 41                          */
    #define tgb41bus     8 /* Groenbewaking prioriteit fase 41                      */
    #define tblk41bus    9 /* Blokkeertijd na prioriteitsingreep fase 41            */
    #define tgbhd02     10 /* Groenbewaking HD fase 02                              */
    #define trthd02     11 /* Actuele rijtijd HD fase 02                            */
    #define tbtovg02hd  12 /* Timer bezettijd prioriteit gehinderde rijtijd fase 02 */
    #define thdin02kar  13 /* Anti jutter tijd inmelden HD 02 KAR                   */
    #define thduit02kar 14 /* Anti jutter tijd uitmelden HD 02                      */
    #define tgbhd05     15 /* Groenbewaking HD fase 05                              */
    #define trthd05     16 /* Actuele rijtijd HD fase 05                            */
    #define tbtovg05hd  17 /* Timer bezettijd prioriteit gehinderde rijtijd fase 05 */
    #define thdin05opt  18 /* Anti jutter tijd inmelden HD 05 Opticom               */
    #define tgbhd08     19 /* Groenbewaking HD fase 08                              */
    #define trthd08     20 /* Actuele rijtijd HD fase 08                            */
    #define tbtovg08hd  21 /* Timer bezettijd prioriteit gehinderde rijtijd fase 08 */
    #define TMMAX1      22

/* teller elementen */
/* ---------------- */
    #define cvc41bus 0 /* Bijhouden prio inmeldingen fase 41 type Bus */
    #define cvchd02  1 /* Bijhouden prio inmeldingen fase 02          */
    #define cvchd05  2 /* Bijhouden prio inmeldingen fase 05          */
    #define cvchd08  3 /* Bijhouden prio inmeldingen fase 08          */
    #define CTMAX1   4

/* schakelaars */
/* ----------- */
    #define schcheckdstype       0 /* Check type DSI bericht bij VECOM                            */
    #define schprioin41buskar    1 /* Inmelden 41 via Bus toestaan                                */
    #define schpriouit41buskar   2 /* Uitmelden 41 via Bus toestaan                               */
    #define schhdin02kar         3 /* Inmelden 02 via KAR HD toestaan                             */
    #define schhduit02kar        4 /* Uitmelden 02 via KAR HD toestaan                            */
    #define schchecksirene02     5 /* Bij HD meldingen bij 02 via DSI controleren op CIF_SIR      */
    #define schhdin05kar         6 /* Inmelden 05 via KAR HD toestaan                             */
    #define schhduit05kar        7 /* Uitmelden 05 via KAR HD toestaan                            */
    #define schchecksirene05     8 /* Bij HD meldingen bij 05 via DSI controleren op CIF_SIR      */
    #define schhdinuit05opt      9 /* In- en uitmelden 05 via Opticom HD toestaan                 */
    #define schhdin08ris        10 /* Inmelden 08 via RIS HD toestaan                             */
    #define schhduit08ris       11 /* Uitmelden 08 via RIS HD toestaan                            */
    #define schrisgeencheckopsg 12 /* Niet checken op signaalgroep bij RIS aanvragen en verlengen */
    #define schrisaanvraag      13 /* Globaal in of uitschakelen aanvragen via RIS                */
    #define schrisverlengen     14 /* Globaal in of uitschakelen verlengen via RIS                */
    #define schaltg02           15 /* Alternatieve realisatie toestaan fase 02                    */
    #define schaltg05           16 /* Alternatieve realisatie toestaan fase 05                    */
    #define schaltg08           17 /* Alternatieve realisatie toestaan fase 08                    */
    #define schaltg41           18 /* Alternatieve realisatie toestaan fase 41                    */
    #define SCHMAX1             19

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
    #define prmmk081                 8 /* Meetkriterium type voor detector 081                                           */
    #define prmmk082                 9 /* Meetkriterium type voor detector 082                                           */
    #define prmmk411                10 /* Meetkriterium type voor detector 411                                           */
    #define prmmk412                11 /* Meetkriterium type voor detector 412                                           */
    #define prmmwta                 12 /* Maximale wachttijd autoverkeer                                                 */
    #define prmmwtfts               13 /* Maximale wachttijd fiets                                                       */
    #define prmmwtvtg               14 /* Maximale wachttijd voetgangers                                                 */
    #define prmpmgt02               15 /* Minimaal percentage groentijd primair tbv. terugkomen fase 02                  */
    #define prmognt02               16 /* Minimale groentijd bij terugkomen fase 02                                      */
    #define prmnofm02               17 /* Aantal malen niet afkappen na OV ingreep fase 02                               */
    #define prmmgcov02              18 /* Minimum groentijd waarna fase 02 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov02             19 /* Minimum percentage groentijd waarna fase 02 afgkapt mag worden door OV ingreep */
    #define prmohpmg02              20 /* Percentage ophogen groentijd na afkappen fase 02                               */
    #define prmpmgt05               21 /* Minimaal percentage groentijd primair tbv. terugkomen fase 05                  */
    #define prmognt05               22 /* Minimale groentijd bij terugkomen fase 05                                      */
    #define prmnofm05               23 /* Aantal malen niet afkappen na OV ingreep fase 05                               */
    #define prmmgcov05              24 /* Minimum groentijd waarna fase 05 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov05             25 /* Minimum percentage groentijd waarna fase 05 afgkapt mag worden door OV ingreep */
    #define prmohpmg05              26 /* Percentage ophogen groentijd na afkappen fase 05                               */
    #define prmpmgt08               27 /* Minimaal percentage groentijd primair tbv. terugkomen fase 08                  */
    #define prmognt08               28 /* Minimale groentijd bij terugkomen fase 08                                      */
    #define prmnofm08               29 /* Aantal malen niet afkappen na OV ingreep fase 08                               */
    #define prmmgcov08              30 /* Minimum groentijd waarna fase 08 afgkapt mag worden door OV ingreep            */
    #define prmpmgcov08             31 /* Minimum percentage groentijd waarna fase 08 afgkapt mag worden door OV ingreep */
    #define prmohpmg08              32 /* Percentage ophogen groentijd na afkappen fase 08                               */
    #define prmrto41bus             33 /* Ongehinderde rijtijd prioriteit fase 41                                        */
    #define prmrtbg41bus            34 /* Beperkt gehinderde rijtijd prioriteit fase 41                                  */
    #define prmrtg41bus             35 /* Gehinderde rijtijd prioriteit fase 41                                          */
    #define prmomx41bus             36 /* Ondermaximum OV fase 41                                                        */
    #define prmupinagb41bus         37 /* Selectieve detectie onbetrouwbaar na groenbewaking OV fase 41                  */
    #define prmvtgcat41bus          38 /* Voertuigcategorie DSI voor prio ingreep 41bus                                  */
    #define prmprio41bus            39 /* Prioriteitsinstelling OV fase 41                                               */
    #define prmpriohd02             40 /* Prioriteitsinstelling HD fase 02                                               */
    #define prmrtohd02              41 /* Ongehinderde rijtijd HD fase 02                                                */
    #define prmrtbghd02             42 /* Beperkt gehinderde rijtijd HD fase 02                                          */
    #define prmrtghd02              43 /* Gehinderde rijtijd HD fase 02                                                  */
    #define prmupinagbhd02          44 /* Selectieve detectie onbetrouwbaar na groenbewaking HD fase 02                  */
    #define prmpriohd05             45 /* Prioriteitsinstelling HD fase 05                                               */
    #define prmrtohd05              46 /* Ongehinderde rijtijd HD fase 05                                                */
    #define prmrtbghd05             47 /* Beperkt gehinderde rijtijd HD fase 05                                          */
    #define prmrtghd05              48 /* Gehinderde rijtijd HD fase 05                                                  */
    #define prmupinagbhd05          49 /* Selectieve detectie onbetrouwbaar na groenbewaking HD fase 05                  */
    #define prmpriohd08             50 /* Prioriteitsinstelling HD fase 08                                               */
    #define prmrtohd08              51 /* Ongehinderde rijtijd HD fase 08                                                */
    #define prmrtbghd08             52 /* Beperkt gehinderde rijtijd HD fase 08                                          */
    #define prmrtghd08              53 /* Gehinderde rijtijd HD fase 08                                                  */
    #define prmupinagbhd08          54 /* Selectieve detectie onbetrouwbaar na groenbewaking HD fase 08                  */
    #define prmrisstart08hd         55
    #define prmrisend08hd           56
    #define prmrisrole08hd          57
    #define prmrissubrole08hd       58
    #define prmrisimportance08hd    59
    #define prmrisstationtype08hd   60
    #define prmrisgrenspriotype08hd 61 /* Greenswaarde type prioriteit RIS voor fase 08                                  */
    #define prmrisapproachid08hd    62
    #define prmrislaneid08hd_1      63
    #define prmrislaneid08hd_2      64
    #define prmstkp1                65 /* Start klokperiode ochtend                                                      */
    #define prmetkp1                66 /* Einde klokperiode ochtend                                                      */
    #define prmdckp1                67 /* Dagsoort klokperiode ochtend                                                   */
    #define prmmg1_02               68 /* Maximale groentijd MG1 02                                                      */
    #define prmmg1_05               69 /* Maximale groentijd MG1 05                                                      */
    #define prmmg1_08               70 /* Maximale groentijd MG1 08                                                      */
    #define prmmg1_41               71 /* Maximale groentijd MG1 41                                                      */
    #define prmmg2_02               72 /* Maximale groentijd MG2 02                                                      */
    #define prmmg2_05               73 /* Maximale groentijd MG2 05                                                      */
    #define prmmg2_08               74 /* Maximale groentijd MG2 08                                                      */
    #define prmmg2_41               75 /* Maximale groentijd MG2 41                                                      */
    #define prmrisapproachid08      76 /* Approach id voor fase 08                                                       */
    #define prmrislaneid08_1        77 /* Lane id voor lane met index 1 bij fase 08                                      */
    #define prmrislaneid08_2        78 /* Lane id voor lane met index 2 bij fase 08                                      */
    #define prmmlfpr02              79 /* Maximaal aantal modules vooruit fase 02                                        */
    #define prmmlfpr05              80 /* Maximaal aantal modules vooruit fase 05                                        */
    #define prmmlfpr08              81 /* Maximaal aantal modules vooruit fase 08                                        */
    #define prmmlfpr41              82 /* Maximaal aantal modules vooruit fase 41                                        */
    #define prmaltg02               83 /* Minimale groentijd bij alternatieve realisatie fase 02                         */
    #define prmaltp02               84 /* Minimale ruimte tbv alternatieve realisatie fase 02                            */
    #define prmaltg05               85 /* Minimale groentijd bij alternatieve realisatie fase 05                         */
    #define prmaltp05               86 /* Minimale ruimte tbv alternatieve realisatie fase 05                            */
    #define prmaltg08               87 /* Minimale groentijd bij alternatieve realisatie fase 08                         */
    #define prmaltp08               88 /* Minimale ruimte tbv alternatieve realisatie fase 08                            */
    #define prmaltg41               89 /* Minimale groentijd bij alternatieve realisatie fase 41                         */
    #define prmaltp41               90 /* Minimale ruimte tbv alternatieve realisatie fase 41                            */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST
    #define prmtestdsivert          91 /* Testen vertraging in DSI bericht in testomgeving                               */
    #define prmtestdsilyn           92 /* Testen lijnnummer DSI bericht in testomgeving                                  */
    #define prmtestdsicat           93 /* Testen ritcategorie DSI bericht in testomgeving                                */
    #define PRMMAX1                 94
#else
    #define PRMMAX1                 91
#endif

/* Selectieve detectie */
/* ------------------- */
    #define dsdummy 0 /* Dummy SD lus 0: tbv KAR */
    #define DSMAX    1

    #define prioFC41bus 0
    #define hdFC02 1
    #define hdFC05 2
    #define hdFC08 3
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

