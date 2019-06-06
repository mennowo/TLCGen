#ifndef EXTRA_FUNC_OV
#define EXTRA_FUNC_OV

#define MAX_AANTAL_INMELDINGEN 10

extern mulv C_counter_old[CTMAX];

boolv DSIMeldingOV_V1(count dslus, count vtgtype, boolv checkfcnmr, count fcnmr, boolv checktype, count meldingtype, boolv extra);
boolv DSIMeldingOV_LijnNummer_V1(count lijnparm, count lijnmax);
boolv DSIMeldingOV_LijnNummerEnRitCategorie_V1(count lijnparm, count lijnmax);
boolv DSIMelding_HD_V1(count dir, count meldingtype, boolv check_sirene);
void TrackStiptObvTSTP(count hin, count huit, int * iAantInm, int iKARInSTP[], count cvc, int grensvroeg, int grenslaat);
void OV_teller(count cov, count scov);

#ifdef CCOL_IS_SPECIAL
void reset_DSI_message(void);
void set_DSI_message(mulv ds, s_int16 vtg, s_int16 dir, count type, s_int16 stiptheid, s_int16 aantalsecvertr, s_int16 PRM_lijnnr, s_int16 PRM_ritcat, s_int16 prio);
#endif

#endif