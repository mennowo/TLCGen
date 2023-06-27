#ifndef EXTRA_FUNC_OV
#define EXTRA_FUNC_OV

#define MAX_AANTAL_INMELDINGEN 10

extern mulv C_counter_old[CTMAX];
extern mulv C_counter_old[CTMAX];
#ifndef NO_PRIO
    extern boolv vertraag_kar_uitm[prioFCMAX];
#endif 

bool DSIMeldingPRIO_V1(count dslus, count vtgtype, bool checkfcnmr, count fcnmr, bool checktype, count meldingtype, bool extra);
bool DSIMeldingPRIO_V2(count fc, count prio_fc, count dslus, count vtgtype, bool checkfcnmr, count fcnmr, bool checktype, count meldingtype, bool extra);
bool DSIMeldingPRIO_LijnNummer_V1(count lijnparm, count lijnmax);
bool DSIMeldingPRIO_LijnNummerEnRitCategorie_V1(count lijnparm, count lijnmax);
bool DSIMelding_HD_V1(count dir, count meldingtype, bool check_sirene);
void TrackStiptObvTSTP(count hin, count huit, int * iAantInm, int iKARInSTP[], count cvc, int grensvroeg, int grenslaat);
void PRIO_teller(count cov, count scov);

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

void NevenMelding(count ov1, count ov2, count ov3, count d, count prmrtbl, count prmrtbh, count hovss1, count hovss2, count hovss3, count hneven1, count hneven2, count hneven3);
bool fietsprio_inmelding(count fc, count dvw, count c_priocount, count c_priocyc, count prm_prioblok, count prm_priocyc, count prm_priocount, count prm_priowt, bool prioin, count ml, count me_priocount, count prm_priocountris);
void fietsprio_update(count fc, count dvw, count c_priocount, count c_priocyc, bool prioin, count ml);  
