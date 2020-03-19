#ifndef EXTRA_FUNC_OV
#define EXTRA_FUNC_OV

#define MAX_AANTAL_INMELDINGEN 10

extern mulv C_counter_old[CTMAX];

bool DSIMeldingPRIO_V1(count dslus, count vtgtype, bool checkfcnmr, count fcnmr, bool checktype, count meldingtype, bool extra);
bool DSIMeldingPRIO_LijnNummer_V1(count lijnparm, count lijnmax);
bool DSIMeldingPRIO_LijnNummerEnRitCategorie_V1(count lijnparm, count lijnmax);
bool DSIMelding_HD_V1(count dir, count meldingtype, bool check_sirene);
void TrackStiptObvTSTP(count hin, count huit, int * iAantInm, int iKARInSTP[], count cvc, int grensvroeg, int grenslaat);
void PRIO_teller(count cov, count scov);

rif_bool ris_inmelding_selectief(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_bool match_signalgroup);
rif_bool ris_uitmelding_selectief(count fc);
void ris_ym(int prioFcsrm, count tym, count tym_max);
void ris_verstuur_ssm(int prioFcsrm);

#ifdef CCOL_IS_SPECIAL
void reset_DSI_message(void);
void set_DSI_message(mulv ds, s_int16 vtg, s_int16 dir, count type, s_int16 stiptheid, s_int16 aantalsecvertr, s_int16 PRM_lijnnr, s_int16 PRM_ritcat, s_int16 prio);
#endif

#ifdef PRIO_CHECK_WAGENNMR
void WDNST_cleanup(void);
bool WDNST_check_in(count fc);
bool WDNST_check_uit(count fc);
#endif
#endif