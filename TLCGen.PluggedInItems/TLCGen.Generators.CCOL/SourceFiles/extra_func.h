#ifndef EXTRA_FUNC
#define EXTRA_FUNC
	#include "gkvar.h"
	#include "nlvar.h"

bool ym_maxV1(count i, mulv to_verschil);
bool ym_max_toV1(count i, mulv to_verschil);
bool ym_max_vtgV1(count i);
void AanvraagSnelV2(count fc1, count dp);
bool Rateltikkers(count fc,       /* fase */
	count has,      /* hulpelement rateltikkers voor deze fase */
	count has_aan_, /* hulpelement tikkers werking */
	count has_cont_,/* hulpelement tikkers continu */
	count tnlrt,    /* tijd na EG dat de tikkers nog moeten worden aangestuurd indien niet continu */
	...);           /* drukknoppen */
bool Rateltikkers_Accross(count fc,       /* fase */
	count has,      /* hulpelement rateltikkers voor deze fase */
	count has_aan_, /* hulpelement tikkers werking */
	count has_cont_,/* hulpelement tikkers continu */
	...);           /* drukknoppen */
void Eerlijk_doseren_V1(count hfile,            /* hulpelement wel/geen file */
	count _prmperc,         /* indexnummer parameter % doseren */
	count aantalfc,         /* aantal te doseren fasen */
	count fc[],             /* pointer naar array met fasenummers */
	count fcmg[][MPERIODMAX],        /* pointer naar array met mg parameter index nummers */
	int nogtedoseren[],     /* pointer naar array met nog te doseren waarden */
	bool *prml[],
	count ml);
void Eerlijk_doseren_VerlengGroenTijden_V1(count hfile,            /* hulpelement wel/geen file */
    count _prmperc,         /* indexnummer parameter % doseren */
    count aantalfc,         /* aantal te doseren fasen */
    count fc[],             /* pointer naar array met fasenummers */
    count fcvg[][MPERIODMAX],        /* pointer naar array met mg parameter index nummers */
    int nogtedoseren[],     /* pointer naar array met nog te doseren waarden */
	bool *prml[], 
	count ml);
void FileMeldingV2(count det,     /* filelus                                */
    count tbez,    /* bezettijd  als D langer bezet -> file  */
    count trij,    /* rijtijd    als D korter bezet -> !file */
    count tafval,  /* afvalvertraging filemelding            */
    count hfile);   /* hulpelement filemelding                */
void mee_aanvraag_prm(count i, count j, count prm, bool extra_condition);
void UpdateKnipperSignalen();
bool hf_wsg_nl(void);
void wachttijd_leds_knip(count fc, count mmwtv, count mmwtm, count RR_T_wacht, count fix);
bool set_FPRML_fk_gkl(count i, bool *prml[], count ml, count ml_max, bool period);
bool kcv_primair_fk_gkl(count i);
void veiligheidsgroen_V1(count fc, count tmaxvag4, ...);
bool proc_pel_uit_V1(count fc, count he1, count he2, count he3, count tmeet, count tmaxth, count grens, count mvtg, count muit);                          /* MM uitsturing aktief                                             */

extern mulv FC_type[];
extern mulv DVG[];
extern int Knipper_1Hz;

#endif