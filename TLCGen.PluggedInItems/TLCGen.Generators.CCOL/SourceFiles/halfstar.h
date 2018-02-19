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

#ifndef __RHDHV_PLE__
#define __RHDHV_PLE__

#define h_plact hplact
mulv TVGA_timer[FCMAX];
char HalfstarOmschakelenToegestaan = FALSE;

void rhdhv_yv_ov_pl(count fc, bool bit, bool condition);
bool txb_gemist(count i, int marge);
void rhdhv_altcor_kop_pl(count fc_aan, count fc_af, count t_nl);
void naloopSG_halfstar(count fc1, count fc2, bool a_bui_fc1, count tkopfc1fc2);
void rhdhv_altcor_parftsvtg_pl(count fc1, count fc2, bool voorwaarde);
void rhdhv_altcor_parfts_pl(count fc1, count fc2, bool voorwaarde);
void altcor_naloopSG_halfstar(count fc1, count fc2, bool a_bui_fc1, count tnlsg12, bool voorwaarde);
void rhdhv_alternatief_pl(count fc, mulv altp, bool condition);
bool rhdhv_yv_ar_max_pl(count i, mulv koppeltijd);
bool rhdhv_ym_max_pl(count i, mulv koppeltijd);
void rhdhv_zachtekoppeling_pl(bool period, count fc1, count fc2, count tvs, count tnldet, count tnl);
void naloopEG_CV_halfstar(bool period, count fc1, count fc2, count tvs, count tnldet, count tnl);
void set_ym_pl_halfstar(count fc, bool condition);
void rhdhv_mgcor_pl(count fcaan, count fcnal, count t_nal);
void rhdhv_mgcor_pl_deelc(count fc1, count fc2);
void Verlengroen_na_Meeverlenggroen_PL(count fc, count prmvgmg);
void wachtstand_halfstar(count fc, bool condition_a, bool condition_ws);
bool rhdhv_tussen_txa_en_txb(count fc);
bool rhdhv_tussen_txb_en_txc(count fc);
bool rhdhv_tussen_txb_en_txd(count fc);
void rhdhv_tweederealisatie(count fc_1, count fc_2);
mulv rhdhv_tar_max_ple(count i);
void rhdhv_set_pp(count fc, bool condition, count value);
void rhdhv_var_txc(count fc, bool condition);
void rhdhv_pl_fc(count fc_a,  count fc_e);
void rhdhv_reset_PLE(void);
void rhdhv_detstor_ple(count fc, count dkop, count dlang1, count dlang2, count prm_perc);
void sync_pg(void);
void rhdhv_set_yspl(count fc);
void rhdhv_vs_ple(count fc, count prmtotxa, bool condition);
void rhdhv_wg_ple(count fc, bool condition);
void SetPlanTijden2R(count fc, mulv plan, mulv ta  , mulv tb  , mulv tc  , mulv td  , mulv te  , 
                     count fc_2,          mulv ta_2, mulv tb_2, mulv tc_2, mulv td_2, mulv te_2);
bool rhdhv_pl_gebied(mulv tx, mulv s, mulv e);
void reset_realisation_timers(void);
void tvga_timer_halfstar(void);
void PercentageMaxGroenTijdenSP(count fc, count percentage);

#ifndef AUTOMAAT
  bool rhdhv_txboverslag(count fc, bool  condition);
  
  #ifdef PRINTTIG
    void rhdhv_print_tig(void);
  #endif
#endif

#endif

