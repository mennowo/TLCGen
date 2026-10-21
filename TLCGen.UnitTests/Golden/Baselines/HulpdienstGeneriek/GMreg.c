/* REGELPROGRAMMA */
/* -------------- */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMreg.c
      CCOL:   10.0
    TLCGEN:   
   CCOLGEN:   UnitTest
*/

/****************************** Versie commentaar ***********************************
 *
 * Versie   Datum   Ontwerper   Commentaar
 *
 ************************************************************************************/

#define REG (CIF_WPS[CIF_PROG_STATUS] == CIF_STAT_REG)
#define REALFUNC
#define PRIO_ADDFILE

/* include files */
/* ------------- */
    #include "GMsys.h"
    #include "stdfunc.h"  /* standaard functies                */
    #include "fcvar.c"    /* fasecycli                         */
    #include "kfvar.c"    /* conflicten                        */
    #include "usvar.c"    /* uitgangs elementen                */
    #include "dpvar.c"    /* detectie elementen                */
    #include "trg_min.c"  /* garantie-roodtijden               */
    #include "tgg_min.c"  /* garantie-groentijden              */
    #include "tgl_min.c"  /* garantie-geeltijden               */
    #include "isvar.c"    /* ingangs elementen                 */
    #include "dsivar.c"   /* selectieve detectie               */
    #include "hevar.c"    /* hulp elementen                    */
    #include "mevar.c"    /* geheugen elementen                */
    #include "tmvar.c"    /* tijd elementen                    */
    #include "ctvar.c"    /* teller elementen                  */
    #include "schvar.c"   /* software schakelaars              */
    #include "prmvar.c"   /* parameters                        */
    #include "lwmlvar.c"  /* langstwachtende modulen structuur */
#ifndef NO_PRIO
    #include "prio.h"       /* prio-afhandeling                  */
#endif /* NO_PRIO */
    #include "prsvar.c"   /* parameters parser                 */
    #include "control.c"  /* controller interface              */
    #include "rtappl.h"   /* applicatie routines               */
#ifndef NO_PRIO
    #include "extra_func_prio.c" /* extra standaard functies OV     */
#endif /* NO_PRIO */
    #include "extra_func.c" /* extra standaard functies        */

#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)
/*    #include "ccdump.inc" */
    #include "keysdef.c"     /* Definitie toetsenbord t.b.v. stuffkey  */
    #if !defined (_DEBUG)
        #include "xyprintf.h" /* Printen debuginfo                      */
    #endif
#endif

    #include "detectie.c"
    #include "ccolfunc.c"
    #include "realfunc.c"
#ifdef MIRMON
    #include "MirakelMonitor.h"
#endif /* MIRMON */

mulv TDH_old[DPMAX];
mulv DB_old[DPMAX];
mulv DVG[DPMAX]; /* T.b.v. veiligheidsgroen */

/* kruispuntnaam in VISSIM */
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM
    code SCJ_code[] = "";
#endif
mulv C_counter_old[CTMAX];

#if !defined AUTOMAAT && !defined AUTOMAAT_TEST
    extern boolv display;
#endif

#include "GMreg.add"

void PreApplication(void)
{
    TFB_max = PRM[prmfb];

    PreApplication_Add();

}

void DetectieStoring_Aanvraag(void)
{
    /* vaste/vertraagde aanvraag bij detectiestoring */


    DetectieStoring_Aanvraag_Add();
}

void DetectieStoring_Meetkriterium(void)
{
    int fc;

    /* reset MK-bits vooraf, ivm onderlinge verwijzing. */
    for (fc = 0; fc < FCMAX; ++fc)
        MK[fc] &= ~BIT5;

    /* hiaattijd op koplus bij defect lange lus */
    /* ---------------------------------------- */

    /* instellen MK bij defect alle kop/lange lussen */
    /* ---------------------------------------------- */


    DetectieStoring_Meetkriterium_Add();
}

void DetectieStoring_MaxGroen(void)
{
    /* percentage MG bij defect alle kop/lange lussen */
    /* ---------------------------------------------- */


    DetectieStoring_MaxGroen_Add();
}
void KlokPerioden(void)
{
    /* default klokperiode voor max.groen */
    /* ---------------------------------- */
    MM[mperiod] = 0;

    /* klokperiode:  */
    /* ------------- */
    if (klokperiode(PRM[prmstkp1], PRM[prmetkp1]) &&
        dagsoort(PRM[prmdckp1]))
        MM[mperiod] = 1;

    KlokPerioden_Add();
}

void Aanvragen(void)
{
    /* Vaste aanvragen */
    /* --------------- */

    /* Wachtstand groen aanvragen */
    /* -------------------------- */

    DetectieStoring_Aanvraag();
    Aanvragen_Add();
}

void BepaalRealisatieTijden(void)
{
    boolv wijziging = TRUE;
    int i;

    BepaalRealisatieTijden_Add();
}

void Maxgroen(void)
{
    maximumgroentijden_va_arg((count) fc02,
                              (va_mulv) PRM[prmmg2_02], (va_mulv) (MM[mperiod] == 1),
                              (va_mulv) PRM[prmmg1_02], (va_count) END);
    maximumgroentijden_va_arg((count) fc05,
                              (va_mulv) PRM[prmmg2_05], (va_mulv) (MM[mperiod] == 1),
                              (va_mulv) PRM[prmmg1_05], (va_count) END);
    maximumgroentijden_va_arg((count) fc08,
                              (va_mulv) PRM[prmmg2_08], (va_mulv) (MM[mperiod] == 1),
                              (va_mulv) PRM[prmmg1_08], (va_count) END);
    maximumgroentijden_va_arg((count) fc41,
                              (va_mulv) PRM[prmmg2_41], (va_mulv) (MM[mperiod] == 1),
                              (va_mulv) PRM[prmmg1_41], (va_count) END);

    DetectieStoring_MaxGroen();


    Maxgroen_Add();
}

void Wachtgroen(void)
{
    int fc;

    for (fc = 0; fc < FCMAX; ++fc)
        RW[fc] &= ~BIT4;  /* reset BIT-sturing */


    for (fc = 0; fc < FCMAX; ++fc)
    {
        WS[fc] &= ~BIT4;  /* reset BIT-sturing */
        WS[fc] |= (G[fc] && (RW[fc] & BIT4)) ? BIT4 : 0;
    }

    for (fc = 0; fc < FCMAX; ++fc)
        WS[fc] &= ~BIT1;  /* reset BIT-sturing */


    Wachtgroen_Add();
}
void Meetkriterium(void)
{
    meetkriterium_prm_va_arg((count)fc02, (count)tkm02, 
                             (va_count)d021, (va_mulv)PRM[prmmk021],
                             (va_count)d022, (va_mulv)PRM[prmmk022],
                             (va_count)END);
    meetkriterium_prm_va_arg((count)fc05, (count)tkm05, 
                             (va_count)d051, (va_mulv)PRM[prmmk051],
                             (va_count)d052, (va_mulv)PRM[prmmk052],
                             (va_count)END);
    meetkriterium_prm_va_arg((count)fc08, (count)tkm08, 
                             (va_count)d081, (va_mulv)PRM[prmmk081],
                             (va_count)d082, (va_mulv)PRM[prmmk082],
                             (va_count)END);
    meetkriterium_prm_va_arg((count)fc41, (count)tkm41, 
                             (va_count)d411, (va_mulv)PRM[prmmk411],
                             (va_count)d412, (va_mulv)PRM[prmmk412],
                             (va_count)END);


    DetectieStoring_Meetkriterium();
    Meetkriterium_Add();
}

void Meeverlengen(void)
{
    int fc;

    for (fc = 0; fc < FCMAX; ++fc)
    {
        YM[fc] &= ~BIT4;  /* reset BIT-sturing */
    }


    Meeverlengen_Add();
}
void Synchronisaties(void)
{
    Synchronisaties_Add();
}

void RealisatieAfhandeling(void)
{
    int fc;
    boolv wijziging = TRUE;
    mulv PAR_old[FCMAX];
    mulv PAR_los_old[FCMAX] = {0};

    /* versnelde primaire realisaties */
    /* ------------------------------ */
    /* periode versnelde primaire realisatie - aantal modulen vooruit */
    /* -------------------------------------------------------------- */
    PFPR[fc02] = ml_fpr(fc02, PRM[prmmlfpr02], PRML, ML, MLMAX);
    PFPR[fc05] = ml_fpr(fc05, PRM[prmmlfpr05], PRML, ML, MLMAX);
    PFPR[fc08] = ml_fpr(fc08, PRM[prmmlfpr08], PRML, ML, MLMAX);
    PFPR[fc41] = ml_fpr(fc41, PRM[prmmlfpr41], PRML, ML, MLMAX);



    VersneldPrimair_Add();

    for (fc = 0; fc < FCMAX; ++fc)
    {
        set_FPRML_fk_gkl(fc, PRML, ML, MLMAX, (boolv)PFPR[fc]);
    }

    /* langstwachtende alternatieve realisatie */
    /* --------------------------------------- */

    afsluiten_aanvraaggebied_pr(PRML, ML);

    for (fc=0; fc<FCMAX; fc++)
    {
        RR[fc] &= ~BIT5;
        FM[fc] &= ~BIT5;
    }

    /* zet richtingen die alternatief gaan realiseren         */
    /* terug naar RV als er geen alternatieve ruimte meer is. */
    RR[fc02] |= R[fc02] && AR[fc02] && (!PAR[fc02] || ERA[fc02]) ? BIT5 : 0;
    RR[fc05] |= R[fc05] && AR[fc05] && (!PAR[fc05] || ERA[fc05]) ? BIT5 : 0;
    RR[fc08] |= R[fc08] && AR[fc08] && (!PAR[fc08] || ERA[fc08]) ? BIT5 : 0;
    RR[fc41] |= R[fc41] && AR[fc41] && (!PAR[fc41] || ERA[fc41]) ? BIT5 : 0;

    FM[fc02] |= (fm_ar_kpr(fc02, PRM[prmaltg02])) ? BIT5 : 0;
    FM[fc05] |= (fm_ar_kpr(fc05, PRM[prmaltg05])) ? BIT5 : 0;
    FM[fc08] |= (fm_ar_kpr(fc08, PRM[prmaltg08])) ? BIT5 : 0;
    FM[fc41] |= (fm_ar_kpr(fc41, PRM[prmaltg41])) ? BIT5 : 0;

    PAR[fc02] = (max_tar_tig(fc02) >= PRM[prmaltp02]) && SCH[schaltg02];
    PAR[fc05] = (max_tar_tig(fc05) >= PRM[prmaltp05]) && SCH[schaltg05];
    PAR[fc08] = (max_tar_tig(fc08) >= PRM[prmaltp08]) && SCH[schaltg08];
    PAR[fc41] = (max_tar_tig(fc41) >= PRM[prmaltp41]) && SCH[schaltg41];








    Alternatief_Add();

    langstwachtende_alternatief_modulen(PRML, ML, ML_MAX);

    YML[ML] = yml_cv_pr(PRML, ML, ML_MAX);

    YML[ML1] |= yml_wml(PRML, ML_MAX);
    YML[ML2] |= FALSE;
    YML[ML3] |= FALSE;
    YML[ML4] |= FALSE;

    Modules_Add();

    SML = modules(ML_MAX, PRML, YML, &ML);

    for (fc = 0; fc < FCMAX; ++fc)
    {
        YM[fc] &= ~BIT5;
        YM[fc] |= SML && PG[fc] ? BIT5 : FALSE;
    }

    RealisatieAfhandeling_Add();
}

void FileVerwerking(void)
{
    FileVerwerking_Add();
}

void init_application(void)
{
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) && !defined VISSIM
    if (!SAPPLPROG)
        stuffkey(CTRLF4KEY);
#endif

    post_init_application();
}

void PostApplication(void)
{
    int i = 0;
    for (i = 0; i < DPMAX; ++i)
    {
        TDH_old[i] = TDH[i];
        DB_old[i] = DB[i];
    }

    PostApplication_Add();
}
void application(void)
{
    PreApplication();

    KlokPerioden();
    Aanvragen();
    Maxgroen();
    BepaalRealisatieTijden();
    Wachtgroen();
    Meetkriterium();
    Meeverlengen();
    Synchronisaties();
    RealisatieAfhandeling();
    FileVerwerking();
#if !defined NO_PRIO && !defined INTERFUNC
    AfhandelingPrio();
#endif /* NO_PRIO */

    PostApplication();
}

void system_application(void)
{
    int ov = 0;

    pre_system_application();

    /* PRIO verklikking */
    /* ---------------- */
    CIF_GUS[usovinm02hd] = C[cvc02hd];
    CIF_GUS[usovinm05hd] = C[cvc05hd];
    CIF_GUS[usovinm08hd] = C[cvc08hd];
    CIF_GUS[usovinm41bus] = C[cvc41bus];

    /* Verklikken melding en ondergedrag KAR */
    CIF_GUS[uskarmelding] = T[tkarmelding];
    CIF_GUS[uskarog] = !T[tkarog];

    /* Verklikken overschreiding maximale wachttijd */
    CIF_GUS[usmaxwt] = FALSE;
    for (ov = 0; ov < prioFCMAX; ++ov)
        CIF_GUS[usmaxwt] |= iMaximumWachtTijdOverschreden[ov];

    /* periode verklikking */
    /* ------------------- */
    CIF_GUS[usperdef] = (MM[mperiod] == 0);
    CIF_GUS[usper1] = (MM[mperiod] == 1);

    check_tgg_min();
    check_tgl_min();
    check_trg_min();

    SegmentSturing(ML+1, ussegm1, ussegm2, ussegm3, ussegm4, ussegm5, ussegm6, ussegm7);


    post_system_application();
}

void system_application2(void)
{

    post_system_application2();
}

#define ENDDUMP 21

void dump_application(void)
{

    post_dump_application();
}

#ifdef CCOL_IS_SPECIAL
void PrioSpecialSignals();
void is_special_signals(void)
{
    PrioSpecialSignals();
    SpecialSignals_Add();
}
#endif

