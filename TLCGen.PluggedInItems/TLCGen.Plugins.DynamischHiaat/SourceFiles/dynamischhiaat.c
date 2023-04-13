/* 
   BESTAND:   dynamischhiaat.c

   ****************************** Versie commentaar **********************************************************
   *
   * Versie   Datum        Ontwerper   Commentaar
   * 1.0.0    25-01-2018   Kzw         Basisversie i.o.v. IVER
   * 2.0.0    05-12-2018   ddo         Diverse aanpassingen voor stedelijk gebruik
   * 2.1.0    24-12-2018   ddo         Diverse aanpassingen na Vissim simulatie
   * 2.2.0    01-02-2019   ddo         Correctie veiligheidsgroen verwijderd
   * 2.3.0    15-02-2019   ddo         Veiligstellen hiaattijden aangepast
   * 2.4.0    20-06-2019   ddo         Niet afzetten MK[fc] BIT3 tijdens MG[fc] tot er een conflictaanvraag is
   * 2.5.0    21-06-2019   ddo         Niet afzetten MK[fc] BIT3 tijdens WG[fc] en 'extra verlengen in WG' geselecteerd
   * 2.5.0a   20-09-2019   ddo         Commentaar toegevoegd mbt 'extra verlengen in WG' en SCH[schdynhiaat<fc>]
   * 2.6.0    21-10-2021   ddo         Fix voor 'nietToepassen' (else toegevoegd)
   * 3.0.0    23-10-2021   ddo         Toevoegen en gebruiken TDHA / TDHDYN ter voorkoming van stortvloed aan wijzigingen op
   *                                      de CVN-C interface, alsmede verwijderen van de wijzigingen onder 2.3.0 en 2.6.0
   * 3.0.1    27-10-2021   ddo         Fix bij niettoepassen (accolade verkeerd)
   * 3.1.0    08-11-2021   ddo         TDHA_max wijzigingen verwijderd; TDH_max berekenen via eigen timers (TDHDYN) zodat niet
   *                                      continue  op de CVN-C interface wordt geschreven maar maar toch de oorspronkelijke 
   *                                      (statische) TDH_max behouden blijft (deze wordt gebruikt in geval van detectiestoring).  
   * 3.2.0    05-02-2022   ddo         Bijwerken tdhdyn ook tijdens RV[fc] (resetten timers en TDHDYN[dp]); toevoegen 
   *                                      #define DYN_HIAAT ivm toepassing van custom VLOG berichten. 
   * 4.0.0    11-08-2022   ddo         Maatregel bij slechts 2 mvt op StartGroen, n.a.v. functionele analyse Rotterdam / Sweco (Peter Zondag).
   *                                      Het tweede voertuig staat dan nog voor de 1e verlenglus waardoor groen soms te snel beeindigd 
   *                                      wordt; in dat geval wordt de SG nu enige tijd vastgehouden in VOORSTART groen.
   * 4.1.0    03-04-2023   ddo         Aflopen detectoren die tot maxgroen moeten verlengen halteren tijdens wachtgroen (Rotterdam / Peter Snijders)
   *
   ***********************************************************************************************************

   Zowel bij de IVER detectieconfiguratie uit 2018 (IVER'18) als bij Groen Op Maat (GOM) wordt gebruik gemaakt
   van dynamische hiaattijden. Met onderstaande code kunnen beide detectieconfiguraties worden bediend.
   
   Dynamische hiaattijden zijn bedoeld om op effici?nte wijze groen te verlengen, waarbij aan de 'voorkant'
   minder vastgroen en geen koplusmaximum* nodig zijn, en aan de 'achterkant' gebruik gemaakt kan worden
   van een deel van de geeltijd. Daarbij wordt gebruik gemaakt van een specifieke detectie configuratie. 

   De methodiek is als GOM ontwikkeld door Luuk Misdom en IT&T (nu Vialis). Voor meer informatie, zie het 
   Handboek Verleerslichtenregelingen 2014 p. 294, Verkeerskunde nummer 06-09, of de website van IT&T:
   - http://www.it-t.nl/wp-content/uploads/Vk06_09-00-art-Groen-op-maat-LR.pdf
   - http://www.it-t.nl/wp-content/uploads/Groen-op-Maat-configuraties.pdf
   
   In augustus 2018 heeft de IVER een (nieuwe) detectieconfiguratie gepubliceerd die ook gebruik maakt van 
   dynamische hiattijden. Zie voor de rapportage "Onderzoek detectieconfiguratie en signaalgroepafhandeling" 
   van Goudappel Coffeng (in opdracht van IVER):
   - https://www.crow.nl/thema-s/verkeersmanagement/iver onder 'Downloads'.
   
   De implementatie van dynamische hiaattijden voor de TLCGen en de effecten ervan zijn nog NIET getest met 
   intergroen of met gekoppelde regelingen. De huidige implementatie is een eerste versie kan daarom nog 
   onverwachte effecten geven; zorgvuldig testen wordt aanbevolen.

   --

   Onderstaande functie 'hiaattijden_verlenging()' gaat ervan uit dat de functie 'meetkriterium2_prm_va_arg()'
   wordt gebruikt voor het bepalen van het meetkriterium. Via een hulpelement kan worden aangegeven of er 
   opgedrempeld mag worden.
   
   De argumenten met een enkel streepje zijn de originele argumenten uit de IVER'18 voorbeeldcode van 
   Goudappel Coffeng (Willem Kinzel). De argumenten met een dubbel streepje zijn toegevoegde argumenten 
   door Lex Trafico (Dick den Ouden) tbv het gemeentelijk wegennet en de GOM detectieconfiguratie. 
   In de code zijn de wijzigingen aangegeven met / *-* / bij de functie of bij de aanpassing.
   
   De code is vermoedelijk nog voor verbetering vatbaar; feedback wordt gewaardeerd op d.denouden@ll-t.nl 

   Een van de verbeteringen is dat sinds versie 3.0.0 geen wijzigingen in TDH_max[] worden aangebracht; het
   grote aantal wijzigingen veroorzaakte een veelheid aan parameterwijzigingen op de CVN-C interface.
   Om dit te bereiken wordt gebruik gemaakt van een (voor dit doel toegevoegde) TDHDYN[], TDHDYN_max[] en 
   TDHDYN_timer[] voor CCOL versies onder de 11, en TDHA voor CCOL versies vanaf 11.
   
   Beschrijving van de aanroep:
   -- 1e argument: boolean tbv bepaling functie wel/niet gebruiken voor deze fase (bijvoorbeeld niet bij 
                   brugopening), uitgevoerd als hulpelement IH[hgeendynhiaat<fc>] voor aansturing vanuit de applicatie.
                   Het hulpelement wordt bestuurd door een schakelaar: IH[hgeendynhiaat<fc>] = !SCH[schdynhiaat<fc>]; 
                   Indien HE waarde == TRUE wordt de functie niet doorlopen.
   -- 2e argument: boolean tbv detectie vrijkomen koplus (tellers starten op ED[koplus] ipv op SG[fc]), 
                   uitgevoerd als schakelaar
                   Indien waarde == TRUE worden de timers gestart bij het voor het eerst na SG[] afvallen van
                   de koplus.
   -- 3e argument: boolean t.b.v. wel/niet extra verlengen in WG. Wanneer in TLCGen onder tabblad
                   'Algemeen' -> 'Info & opties' de optie "extra meeverlengen in WG" is aangevinkt, wordt 
                   in de aanroep TRUE mee gegeven; anders FALSE. De waarde TRUE zorgt ervoor dat in WG[<fc>] 
                   de overgang naar VG[<fc>] pas plaats vindt na een conflicterende aanvraag.
   -- 4e argument: array-nummer memory element meetkriterium
   -- 5e argument: boolean tbv wel/niet opdrempelen, uitgevoerd als hulpelement (aan te sturen vanuit de
                   applicatie via schakelaar of hulpelement)
                   Indien waarde == TRUE mag er worden opgedrampeld; bij NIET opdrempelen geldt er een 
                   gescheiden hiaatmeting per rijstrook.
   -  6e argument: array-nummer signaalgroep 

   Dan per detectielus de volgende argumenten:
   -  rijstrook 1, 2, 3 of 4. Deze waarde wordt gebruikt voor het al dan niet opdrempelen. Als er wel 
                   opgedrempeld mag worden, dan via een schakelaar of in de regeling (bijvoorbeeld tbv comfort
                   in de dalperiode) het hulpelement H[hopdrempelen<fc>] hoog maken.
   -  array-nummer detectie-element
   -  array-nummer tijdelement - moment 1
   -  array-nummer tijdelement - moment 2
   -  array-nummer tijdelement - hiaattijd 1
   -  array-nummer tijdelement - hiaattijd 2
   -  array-nummer tijdelement - maximum groentijd (wanneer '0' wordt ingevuld, gebruikt de code de vigerende
                   maximum groentijd TFG_max[] + TVG_max[]     
   -- array-nummer parameter detectorvoorwaarden (zie hieronder)
   -- array-nummer hulpelement extra verlengvoorwaarde (zie hieronder)

   De parameter 'detectorvoorwaarden' omvat de volgende opties:
   -  springvoorwaarde  (op SG, als er geen hiaatmeting      is op         de stroomafwaartse lussen, 
      meteen naar de 2e/lagere hiaattijd overgaan)
   -  verlengvoorwaarde (op SG, als er geen hiaatmeting meer is op deze en de stroomafwaartse lussen, 
      de verlengfunctie UITschakelen)
   -- extra verlengvoorwaarde (bij TRUE altijd verlengen op deze lus; bijvoorbeeld bij permanente aanwezigheid
      deelconflict (G[fc11] && G[fc36]) )
   -- aftelvoorwaarde (tijdens groen, als er w?l hiaatmeting is op deze lus maar niet op de stroomafwaartse 
      lussen, meteen TDH_max[] gaan aftellen) 
   -- spring-tijdens-groen voorwaarde (wanneer tijdens G[] het hiaat valt, wordt de volgende detector 
      stroomopwaarts de aktieve verlenglus)
      De spring-tijdens-groen voorwaarde is voornamelijk bedoeld voor een GOM detectieveld, waar de eerste en 
      tweede lange lus dichter bij elkaar liggen dan bij een IVER'18 detectieveld.

	  NB: de verlengvoorwaarde is dus eigenlijk een NIETverleng voorwaarde!

   Het hulpelement 'extra verlengvoorwaarde' dient om vanuit de regelapplicatie "incidentele" verlenging op 
   de detector af te dwingen, bijvoorbeeld tijdens file, ontruiming van een spoorwegovergang, of bij een 
   niet-permanent deelconflict.

   In de parameter 'detectorvoorwaarden' worden de springvoorwaarde, verlengvoorwaarde, extra 
   verlengvoorwaarde, aftelvoorwaarde en spring-tijdens-groen voorwaarde bitsgewijs opgeslagen in 
   ??n parameter; de instellingen zijn daardoor ook op straat te wijzigen:
   BIT    dec.   betekenis                          optie in TLCGen 
   BIT0 =  1  -  springvoorwaarde                 - 'SpringStart' 
   BIT1 =  2  -  verlengvoorwaarde                - 'VerlengNiet'
   BIT2 =  4  -- extra verlengvoorwaarde          - 'VerlengExtra'
   BIT3 =  8  -- aftelvoorwaarde                  - 'DirectAftel'  
   BIT4 = 16  -- spring-tijdens-groen voorwaarde  - 'SpringGroen'  

   De waardes kunnen worden opgeteld, bijvoorbeeld verlengvoorwaarde ?n aftelvoorwaarde voor dezelfde detector
   maakt waarde 10.
   Het aktief worden extra verlengvoorwaarde kan tevens worden aangestuurd vanuit het regelprogramma 
   via een hulpelement (IH[] = G[fc1] && G[fc2]).
   
   Er wordt verlengd op de aktieve lus ?n op de lussen stroomopwaarts van de aktieve lus.
   
   De functie hiaattijden_verlenging wordt niet doorlopen (en dus de hiaattijden niet aangepast) wanneer een 
   detectiestoring is geconstateerd. In dat geval wordt de statische hiaattijd gebruikt en dient de gebruiker 
   een eigen detectiestoringsopvang te programmeren, of die uit de TLCGen te gebruiken.

   Voorbeeld voor 2 rijstroken:
                                    1e argument,      2e argument, 3e arg, 4e arg,        5e argument, 6e arg,
    hiaattijden_verlenging( IH[hgeendynhiaat05], SCH[schedkop_05],  FALSE,  mmk05, IH[hopdrempelen05],   fc05,
        1, d051, t051_1, t051_2, ttdh_051_1, ttdh_051_2, tmax_051, prmspringverleng_051, hverleng_051, 
        1, d053, t053_1, t053_2, ttdh_053_1, ttdh_053_2, tmax_053, prmspringverleng_053, hverleng_053, 
        1, d055, t055_1, t055_2, ttdh_055_1, ttdh_055_2, tmax_055, prmspringverleng_055, hverleng_055, 
        1, d057, t057_1, t057_2, ttdh_057_1, ttdh_057_2, tmax_057, prmspringverleng_057, hverleng_057, 
        2, d052, t052_1, t052_2, ttdh_052_1, ttdh_052_2, tmax_052, prmspringverleng_052, hverleng_052, 
        2, d054, t054_1, t054_2, ttdh_054_1, ttdh_054_2, tmax_054, prmspringverleng_054, hverleng_054, 
        2, d056, t056_1, t056_2, ttdh_056_1, ttdh_056_2, tmax_056, prmspringverleng_056, hverleng_056, 
        2, d058, t058_1, t058_2, ttdh_058_1, ttdh_058_2, tmax_058, prmspringverleng_058, hverleng_058, 
        END);

	De hulpelementen IH[hgeendynhiaat05] en IH[hopdrempelen05] worden met een schakelaar op- of afgezet onder
    PreApplication en zijn daarna in de regeling te manipuleren.
		
   ======================================================================================================== */

#define DYN_HIAAT

mulv TUSSEND_TELLER[FCMAX] = { 0 };
mulv TDBEZET_KOPLUS[FCMAX] = { 0 };
mulv GROEN_TIJD[FCMAX]     = { 0 };
mulv mindynhgroen          = 60;      /* toepassen 'maatregel bij slechts 2 mvt op StartGroen' wanneer TFG lager is ingesteld dan deze waarde */
mulv mindynhkopbezet       = 15;      /* minimale bezettijd koplus voor toepassen 'maatregel bij slechts 2 mvt op StartGroen'                 */

#define DYNH_RS_BIT  BIT2  /* gebruikt BIT om de SG in VOORSTARTgroen vast te houden bij toepassen 'maatregel bij slechts 2 mvt op StartGroen' */

#if (CCOL_V >= 110 /* && !defined TDHAMAX */) || (CCOL_V < 110)

   /* TDHDYN variabelen */
   bool TDHDYN[DPMAX];        /* dynamische hiaattijd - logische waarde */
   mulv  TDHDYN_timer0[DPMAX]; /* dynamische hiaattijd - hulp waarde     */
   mulv  TDHDYN_timer[DPMAX];  /* dynamische hiaattijd - actuele waarde  */
   mulv  TDHDYN_max[DPMAX];    /* maximum waarde dynamische hiaattijd    */


   /* TDHDYN detectiefuncties */
void init_tdhdyn(void) /* aanroepen onder init_application() of post_init_application() en !SAPPLPROG */
{
  register count dp;

  for (dp = 0; dp < DP_MAX; dp++) {
    TDHDYN[dp]        = FALSE;
    TDHDYN_timer [dp] = 0;
    TDHDYN_timer0[dp] = 0;
    if (TDHDYN_max[dp] >= 0) TDHDYN[dp] = TRUE;
  }
}

void tdhdyn(count dp, count fc) /* verwerk TDHDYN per detector */
{
  if (RV[fc]) {
    TDHDYN_timer[dp]  = 0;
    TDHDYN_timer0[dp] = 0;
    TDHDYN[dp]        = FALSE;
  }
  
  if (CIF_IS[dp] & CIF_DET_BEZET) {
    if (D[dp]) {
      TDHDYN_timer [dp] = 0;
      TDHDYN_timer0[dp] = 0;
      if (TDHDYN_max[dp] >= 0)  TDHDYN[dp] = TRUE;        /* detectiemeting dynamische hiaattijd is ingeschakeld */
    }
  }
  else {
    if (D[dp]) {
      if (TDHDYN_max[dp] <= 0)  TDHDYN[dp] = FALSE;
    }
    else {
      if (TDHDYN[dp]) {
        TDHDYN_timer [dp]  = TDHDYN_timer0[dp];
        TDHDYN_timer0[dp] += TE;
        if (TDHDYN_timer [dp] >= TDHDYN_max[dp])  TDHDYN[dp] = FALSE;
        
      }
    }
  }
}

#else

  #define TDHDYN        TDH       /* dynamische hiaattijd - logische waarde */
  #define TDHDYN_timer  TDH_timer /* dynamische hiaattijd - actuele waarde  */
  #define TDHDYN_max    TDH_max   /* maximum waarde dynamische hiaattijd    */

#endif
/* ======================================================================================================== */

static int eavl[FCMAX][5];
static int detstor[FCMAX];

void hiaattijden_verlenging(bool nietToepassen, bool vrijkomkop, bool extra_in_wg, count mmk, bool opdr, count fc, ...)
{
  va_list argpt;                                    /* variabele argumentenlijst                                 */
  count dpnr;                                       /* arraynummer detectie-element                              */
  count t1, t2, tdh1, tdh2, tmax;                   /* arraynummers tijdelementen                                */
  count prmdetvw;                                   /* arraynummer parameter detectorvoorwaarden                 */
  count hevlvw;                                     /* arraynummer hulpelement extra verlengvoorwaarde           */
  count rijstrook_old = -1;                         /* vorige rijstrooknummer                                    */
  count rijstrook;                                  /* rijstrooknummer                                           */
  count max_rijstrook = 1;                          /* hoogste rijstrooknummer                                   */
  bool svw, vvw, evlvw, daft, svwG, hulp_bit3, verlengen[5], tdh_saw[5];
  count dp_teller=0;                                /* telt aantal lussen vanaf stopstreep op bepaalde rijstrook */

  /* initialisatie */
  for (rijstrook=0; rijstrook<5; rijstrook++)
  {
    verlengen[rijstrook] = FALSE;
    tdh_saw[rijstrook]   = FALSE;                 /* hiaattijd stroomafwaarts aktief                           */
    eavl[fc][rijstrook]  = 0;                     /* eerste aktieve verlenglus (1 = eerste lus)                */
  }
  
  if (TRG[fc])  detstor[fc] = FALSE;              /* reset aanwezigheid detectiestoring voor deze fc bij TRG[] */ /*-*/

  /* maatregel bij slechts 2 mvt op StartGroen */
  RS[fc] &= ~DYNH_RS_BIT;
  if (FG[fc] && TS) {
      TUSSEND_TELLER[fc] = 0;
      TDBEZET_KOPLUS[fc] = 0;
      GROEN_TIJD[fc] = 0;
  }
  
  /* maatregel bij slechts 2 mvt op StartGroen */
  if (!G[fc] && TE && !detstor[fc]) {
    va_start(argpt, fc);                          /* start var. argumentenlijst                                */
    do {
      rijstrook = va_arg(argpt, va_count);        /* lees rijstrooknummer                                      */
      if (rijstrook>=0) {																	            
        dpnr       = va_arg(argpt, va_count);     /* lees array-nummer detectie                                */
        t1         = va_arg(argpt, va_count);     /* ongebruikt                                                */
        t2         = va_arg(argpt, va_count);     /* ongebruikt                                                */
        tdh1       = va_arg(argpt, va_count);     /* ongebruikt                                                */
        tdh2       = va_arg(argpt, va_count);     /* ongebruikt                                                */
        tmax       = va_arg(argpt, va_count);     /* ongebruikt                                                */
        prmdetvw   = va_arg(argpt, va_count);     /* ongebruikt                                                */
        hevlvw     = va_arg(argpt, va_count);     /* ongebruikt                                                */
    
        max_rijstrook = rijstrook;                    /* onthoud hoogste rijstrooknummer                                  */
        if (rijstrook != rijstrook_old) {
            eavl[fc][rijstrook] = 0;
            dp_teller = 0;
        }
        dp_teller++;
        rijstrook_old = rijstrook;
    
        if (dp_teller == 1) {
            if (!D[dpnr])            TDBEZET_KOPLUS[fc] = 0;
            if (!G[fc] && D[dpnr]) ++TDBEZET_KOPLUS[fc];
        }
    
        if (dp_teller == 2) {
            if ((TDBEZET_KOPLUS[fc] > mindynhkopbezet) && ED[dpnr]) ++TUSSEND_TELLER[fc];  /* TUSSEND_TELLER is dus 1 wanneer er 2 voertuigen staan! */
        }                                                                                  /* (die andere staat dan nl al op de koplus)          */
      }
    } while (rijstrook >= 0);
    va_end(argpt);                     /* maak var. arg-lijst leeg */
  }

  
  /* vaststellen detectiestoring, alleen tijdens RV[], eens per seconde ivm beperken rekentijd */ /*-*/
  if (RV[fc] && TS) {
    va_start(argpt, fc);                          /* start var. argumentenlijst                                */
    do {
      rijstrook = va_arg(argpt, va_count);        /* lees rijstrooknummer                                      */
      if (rijstrook>=0) {																	            
        dpnr       = va_arg(argpt, va_count);     /* lees array-nummer detectie                                */
        t1         = va_arg(argpt, va_count);     /* ongebruikt tijdens R[]                                    */
        t2         = va_arg(argpt, va_count);     /* ongebruikt tijdens R[]                                    */
        tdh1       = va_arg(argpt, va_count);     /* ongebruikt tijdens R[]                                    */
        tdh2       = va_arg(argpt, va_count);     /* ongebruikt tijdens R[]                                    */
        tmax       = va_arg(argpt, va_count);     /* ongebruikt tijdens R[]                                    */
        prmdetvw   = va_arg(argpt, va_count);     /* ongebruikt tijdens R[]                                    */
        hevlvw     = va_arg(argpt, va_count);     /* ongebruikt tijdens R[]                                    */
  
#if (CCOL_V >= 110 /*&& !defined TDHAMAX*/) || (CCOL_V < 110)
        /* bijwerken TDHDYN[dpnr] en TDHDYN_timer[dpnr] */
        tdhdyn(dpnr, fc);
#endif
  
        if (!TRG[fc] && !ERV[fc]) {
  #if defined (DL_type) && !defined (NO_DDFLUTTER) /* CCOL7 of hoger */  
          if (CIF_IS[dpnr] >= CIF_DET_STORING /*|| OG[dpnr]*/ || BG[dpnr] || FL[dpnr])   detstor[fc] |= TRUE;
  #else
          if (CIF_IS[dpnr] >= CIF_DET_STORING /*|| OG[dpnr]*/ || BG[dpnr])               detstor[fc] |= TRUE;
  #endif
         }
      }
    } while (rijstrook>=0);
    va_end(argpt);                     /* maak var. arg-lijst leeg */
  }
  
  if (RA[fc] && !SRA[fc] || G[fc] || GL[fc]) {        /*-*/ 
    va_start(argpt, fc);                              /* start var. argumentenlijst                              */
    do {
      rijstrook = va_arg(argpt, va_count);            /* lees rijstrooknummer                                    */
      if (rijstrook>=0 && (detstor[fc] != TRUE)) {	/*-*/
        dpnr       = va_arg(argpt, va_count);         /* lees array-nummer detectie                              */
        t1         = va_arg(argpt, va_count);         /* lees array-nummer tijdelement - moment 1                */
        t2         = va_arg(argpt, va_count);         /* lees array-nummer tijdelement - moment 2                */
        tdh1       = va_arg(argpt, va_count);         /* lees array-nummer tijdelement - hiaattijd 1             */
        tdh2       = va_arg(argpt, va_count);         /* lees array-nummer tijdelement - hiaattijd 2             */
        tmax       = va_arg(argpt, va_count);         /* lees array-nummer tijdelement - maximum groentijd       */
        prmdetvw   = va_arg(argpt, va_count);         /* lees array-nummer parameter   - detectorvoorwaarden     */
        hevlvw     = va_arg(argpt, va_count);         /* lees array-nummer hulpelement - extra verlengvoorwaarde */
        
        /* omzetten parameter verlengvoorwaarden naar booleanse criteria */           /*-*/
        svw   =  PRM[prmdetvw]&BIT0;                  /* springvoorwaarde  (op SG[])     */
        vvw   =  PRM[prmdetvw]&BIT1;                  /* verlengvoorwaarde (op SG[])     */
        evlvw = (PRM[prmdetvw]&BIT2 || IH[hevlvw]);   /* extra verlengvoorwaarde         */
        daft  =  PRM[prmdetvw]&BIT3;                  /* direct aftelen voorwaarde       */
        svwG  =  PRM[prmdetvw]&BIT4;                  /* spring-tijdens-groen voorwaarde */
       
        /* afbreken lopende hiaattimers op start geel */
        AT[t1]=AT[t2]=AT[tmax] = SGL[fc];              
        
#if (CCOL_V >= 110 && !defined TDHAMAX) || (CCOL_V < 110)
        /* bijwerken TDHDYN[dpnr] en TDHDYN_timer[dpnr] */  /*--*/
        tdhdyn(dpnr, fc);
#endif

        max_rijstrook = rijstrook;                    /* onthoud hoogste rijstrooknummer                                  */
        if (rijstrook != rijstrook_old) {
          eavl[fc][rijstrook] = 0;
          dp_teller = 0;
        }
        dp_teller++;
        rijstrook_old = rijstrook;

        /* maatregel bij slechts 2 mvt op StartGroen */
        if ((dp_teller == 1) && TE)  {
        //RS[fc] &= ~DYNH_RS_BIT;
          RS[fc] |= (G[fc] && (TUSSEND_TELLER[fc] == 1) && (TFG_max[fc] < mindynhgroen) && D[dpnr] && (GROEN_TIJD[fc] < (mindynhgroen - TFG_max[fc]))) ? DYNH_RS_BIT : 0;
          if (RS[fc]&DYNH_RS_BIT)  ++GROEN_TIJD[fc];
        }
        

        if (T_max[tmax]==0)       T_max[tmax] = (TFG_max[fc]+TVG_max[fc]);          /*-*/ /* overnemen max groentijd      */ 
        
        /* actuele hiaattijd bepalen */
        RT[t1]   =
        RT[t2]   =
        RT[tmax] = SG[fc] 
                   || (vrijkomkop && eavl[fc][rijstrook]==1 && (T_timer[tmax]==0)); /*-*/ 

        if (!nietToepassen) {
          if (ERA[fc])             TDHDYN_max[dpnr] = T_max[tdh1];                       /*-*/ /* ERA[] ipv !G[]               */
          if (G[fc] && ET[t2])  TDHDYN_max[dpnr] = T_max[tdh2];
          if (G[fc] && !RT[t1] && !T[t1] && T[t2]) {
            /* hiaattijd wijzigt tussen t1 en t2 lineair, van tdh1 naar tdh2 */
            /* -------y------- = -------------x----------- * -----------------richtingsco?ffici?nt=a-------------- + ----b------ */
            TDHDYN_max[dpnr] = (T_timer[t2] - T_max[t1]) * (T_max[tdh2] - T_max[tdh1]) / (T_max[t2] - T_max[t1]) + T_max[tdh1];
          }

          /* bepalen of er stroomafwaarts van een lus hiaattijden lopen */  /*-*/ /* locatie aangepast (v??r bepaling of meteen naar 2e hiaattijd gesprongen wordt) */
          if (TDHDYN[dpnr]) {
             tdh_saw[rijstrook] = TRUE;
          }

          /* Als er geen verkeer is op de lus en de lussen ervoor bij start groen, meteen naar de 2e hiaattijd springen */
          if (SG[fc] && !tdh_saw[rijstrook] && dp_teller > 1 && svw) {
            RT[t1] =
              RT[t2] = FALSE;
            TDHDYN_max[dpnr] = T_max[tdh2];
            if (vvw) {
              RT[tmax] = FALSE;
            }
          }

          /* Er mag verlengd worden op deze lus tot de timer is afgelopen     */
          /* of wanneer extra verlengvoorwaarde evlvw aanwezig is             */
          if ((RT[tmax] || T[tmax] || evlvw) && G[fc] && TDHDYN[dpnr]) {                                /*-*/ /* evlvw toegevoegd            */
             verlengen[rijstrook] = TRUE;
          }

          /* Bepaal eerste actieve verlenglus, vanaf de stopstreep gerekend   */
          if ((RT[tmax] || T[tmax]            /* maximum tijd loopt nog voor deze detector                         */
            || evlvw) &&     /* of extra verlengvoorwaarde is aktief                              */ /*-*/
            G[fc] &&     /* signaalgroep is groen                                             */
  /*        TDHDYN[dpnr]             && */  /* hiaattijd van de detector loopt                                   */ /* uitgecommentaard in Goudappel code */
            eavl[fc][rijstrook] == 0) {  /* eerste actieve verlenglus is op deze rijstrook nog niet ingesteld */
            eavl[fc][rijstrook] = dp_teller;
          }

          /* afkappen maxtimer t.b.v. springen naar eerstvolgende lus stroomopwaarts*/ /*-*/
          if (G[fc] &&
            ((dp_teller == 1) && vrijkomkop && !TDHDYN[dpnr])  /* voor koplus bij niet (meer) aanwezig zijn van hiaat                    */
            ||
            ((dp_teller != 1) && svwG && !TDHDYN[dpnr] &&      /* voor overige lussen indien spring-tijdens-groen voor deze lus waar is, */
               (eavl[fc][rijstrook] == dp_teller))) {     /* en de lus aktief is, en het hiaat voor de eerste keer gevallen is      */
            RT[tmax] = FALSE;
            AT[tmax] = TRUE;
          }
	  	 
          /* Tijdens groen, als deze lus is de aktieve verlenglus, en stroomafwaarts lopen geen hiaattijden, en de t1 timer */ /*-*/
          /* loopt nog, dan meteen hiaattijd laten aftellen door timers t1 en t2 gelijk te maken aan T_max[t1]              */
          /* Aftellen gebeurt via eerdere formule y = x * a + b                                                             */
          if (daft && G[fc] && tdh_saw[rijstrook] && (eavl[fc][rijstrook] == dp_teller) && (T_timer[t1] < T_max[t1]) && TDHDYN[dpnr]) { /*-*/
             T_timer[t1] = T_max[t1];
             T_timer[t2] = T_max[t1];
          }

          /* Correctie bij wachtgroen */
          HT[tmax] = T[tmax] && WG[fc] && (T_max[tmax] == (TFG_max[fc] + TVG_max[fc])) && (T_timer[t2] == T_max[t2]);    /*--*/

          /* Correctie MM[mmk] bij opdrempelen toegestaan; andere aanroep van meetkriterium2 niet nodig */   /*-*/
          if (opdr /* && !nietToepassen*/) {
            switch (rijstrook) {
            case 1:
              if ((eavl[fc][rijstrook] == dp_teller) && TDHDYN[dpnr])      MM[mmk] |= BIT2;
              break;
            case 2:
              if ((eavl[fc][rijstrook] == dp_teller) && TDHDYN[dpnr])      MM[mmk] |= BIT5;
              break;
            case 3:
              if ((eavl[fc][rijstrook] == dp_teller) && TDHDYN[dpnr])      MM[mmk] |= BIT7;
              break;
            case 4:
              if ((eavl[fc][rijstrook] == dp_teller) && TDHDYN[dpnr])      MM[mmk] |= BIT9;
              break;
            }
          }
        }
      }
    } while (rijstrook>=0);
    va_end(argpt);                     /* maak var. arg-lijst leeg */
  }

  if (!nietToepassen) {
    hulp_bit3 = FALSE;
    for (rijstrook=1; rijstrook<=max_rijstrook; rijstrook++)   /* voor alle rijstroken van de betreffende signaalgroep */
    {
      if ((verlengen[rijstrook] || detstor[fc])      /* zolang er verlengd wordt, of bij een aanwezige detectiestoring */
         || (!(verlengen[rijstrook] || detstor[fc])  /*       of                                                       */
  		   && !fka(fc) && (MG[fc] ||                   /* bij geen (fictieve) conflictaanvraag en MG[]                   */
  		   WG[fc] && extra_in_wg))) {                  /* danwel bij geen fict.confl.aanvr en WG[] en meeverlengen in WG */
        hulp_bit3 = TRUE;                            /* blijft hulp_bit 3 waar en wordt dus MK[] BIT3 niet af gezet    */
      }
      else {
        switch (rijstrook) {
          case 1 :
            MM[mmk] &= ~(BIT1|BIT2);    /* reset meetkriterium rijstrook 1 */  /*-*/ /* MM[mmk] ipv MK[fc] */
            break;
          case 2 :
            MM[mmk] &= ~(BIT4|BIT5);    /* reset meetkriterium rijstrook 2 */  /*-*/
            break;
          case 3 :
            MM[mmk] &= ~(BIT6|BIT7);    /* reset meetkriterium rijstrook 3 */  /*-*/
            break;
          case 4 :
            MM[mmk] &= ~(BIT8|BIT9);    /* reset meetkriterium rijstrook 4 */  /*-*/
            break;
        }
      }
    }
    
    if (!hulp_bit3) {                  /* als er geen enkele strook is waarop verlengd mag worden, ook BIT3 resetten */
      MK[fc] &= ~BIT3;                 /* reset meetkriterium BIT 3 */
    }
  } 
}
