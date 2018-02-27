/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* CCOL - signaalplanstructuur bibliotheek Royal HaskoningDHV voor TPA generator V3.40    */
/*                                                                                        */
/*                                                                                        */
/* (C) Copyright 2015 Royal HaskoningDHV b.v. All rights reserved.                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* Versie :  1.0                                                                          */
/* Naam   :  rhdhv_ple.h                                                                  */
/* Datum  :  07-12-2015 kla: eerste versie                                                */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */

#ifndef __HALFSTARH__
#define __HALFSTARH__

#define RR_VS_HALFSTAR      BIT10 /* voorstart tegenhouden                      */
#define RR_KOP_HALFSTAR     BIT12 /* RR tbv. harde koppeling                    */
#define YV_KOP_HALFSTAR     BIT8  /* YV tbv. VA koppeling                       */
#define YM_KOP_HALFSTAR     BIT9  /* tbv koppeling                              */
#define X_GELIJK_HALFSTAR   BIT8  /* gelijkstart                                */
#define X_DEELC_HALFSTAR    BIT9  /* deelconflict                               */
#define X_VOOR_HALFSTAR     BIT10 /* voorstart                                  */
#define RW_KOP_HALFSTAR     BIT8  /* koppeling                                  */
#define YW_VAR_TXC          BIT8  /* YW voor gegarandeerd groen na SAS          */
#define RW_VAR_TXC          BIT9  /* RW voor gegarandeerd groen na TA           */
#define RW_WG_HALFSTAR      BIT12 /* RW wachtgroen tijdens halfstar             */ 
#define YW_PL_HALFSTAR      BIT9  /* YW tbv. (half)star bedrijf                 */
#define YM_HALFSTAR         BIT8  /* meeverlengen tijdens halfstar              */
#define RR_ALTCOR_HALFSTAR  BIT9  /* alternatieve correctie                     */
#define RS_HALFSTAR         BIT4  /* voorstartgroen                             */
#define RTFB_PLVA_HALFSTAR  BIT1  /* RTFB tbv omschakelen VA <-> PL             */
#define A_WS_HALFSTAR       BIT9  /* wachtstand                                 */
#define RW_VGNAMG_HALFSTAR  BIT11 /* RW tbv. terug naar verlengen tijdens MG    */ 
#define YW_WS_HALFSTAR      BIT10  /* tbv wachtstand tijdens PL                 */
#define A_MR_HALFSTAR       BIT8  /* meerealisatie                              */

#define h_plact hplact

#define TIMER_ACTIVE(t)    ((bool)(T[t] || RT[t] || IT[t]))
#define BIT_ACTIVE(v,t,m)  (v = ((t) ? ((v) |= (m)) : ((v) &= ~(m))))

mulv TVGA_timer[FCMAX];
extern char HalfstarOmschakelenToegestaan;

/* tbv harde koppelingen */
count geeltimer[FCMAX][FCMAX];
count groodtimer[FCMAX][FCMAX];

bool yws_groen_fk(count i);
void set_special_MR(count i, count j, bool condition);
void SetPlanTijden(count fc, mulv plan, mulv ta, mulv tb, mulv tc, mulv td, mulv te);
void gelijkstart_va_arg_halfstar(count h_x, count h_rr, bool  overslag, ...);
void reset_fc_halfstar(void);
void yv_ov_pl_halfstar(count fc, bool bit, bool condition);
bool txb_gemist(count i, int marge);
void altcor_kop_halfstar(count fc_aan, count fc_af, count t_nl);
void naloopSG_halfstar(count fc1, count fc2, bool a_bui_fc1, count tkopfc1fc2);
void altcor_parftsvtg_pl_halfstar(count fc1, count fc2, bool voorwaarde);
void altcor_parfts_pl_halfstar(count fc1, count fc2, bool voorwaarde);
void altcor_naloopSG_halfstar(count fc1, count fc2, bool a_bui_fc1, count tnlsg12, bool voorwaarde);
void alternatief_halfstar(count fc, mulv altp, bool condition);
bool yv_ar_max_halfstar(count i, mulv koppeltijd);
bool ym_max_halfstar(count i, mulv koppeltijd);
void zachtekoppeling_halfstar(bool period, count fc1, count fc2, count tvs, count tnldet, count tnl);
void naloopEG_CV_halfstar(bool period, count fc1, count fc2, count tvs, count tnldet, count tnl);
void set_ym_pl_halfstar(count fc, bool condition);
void mgcor_halfstar(count fcaan, count fcnal, count t_nal);
void mgcor_halfstar_deelc(count fc1, count fc2);
void Verlengroen_na_Meeverlenggroen_PL(count fc, count prmvgmg);
void wachtstand_halfstar(count fc, bool condition_a, bool condition_ws);
bool tussen_txa_en_txb(count fc);
bool tussen_txb_en_txc(count fc);
bool tussen_txb_en_txd(count fc);
void tweederealisatie_halfstar(count fc_1, count fc_2);
void set_pp_halfstar(count fc, bool condition, count value);
void var_txc(count fc, bool condition);
void sync_pg(void);
void set_yspl(count fc);
void vs_ple(count fc, count prmtotxa, bool condition);
void wg_ple(count fc, bool condition);
void SetPlanTijden2R(count fc, mulv plan, mulv ta  , mulv tb  , mulv tc  , mulv td  , mulv te  , 
                     count fc_2,          mulv ta_2, mulv tb_2, mulv tc_2, mulv td_2, mulv te_2);
bool pl_gebied(mulv tx, mulv s, mulv e);
void reset_realisation_timers(void);
void tvga_timer_halfstar(void);
void PercentageMaxGroenTijdenSP(count fc, count percentage);
void reset_altreal_halfstar(void);
void getrapte_fietser_halfstar(count fc1, count fc2, bool  a_bui_fc1, bool  a_bui_fc2, count tkopfc1fc2, count tkopfc2fc1, count voorstartfc1fc2,	count voorstartfc2fc1);

#ifndef AUTOMAAT
  bool txboverslag(count fc, bool  condition);
  
  #ifdef PRINTTIG
    void rhdhv_print_tig(void);
  #endif
#endif

#endif // __HALFSTARH__

