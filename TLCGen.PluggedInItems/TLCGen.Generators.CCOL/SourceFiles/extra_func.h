#ifndef EXTRA_FUNC
#define EXTRA_FUNC

#include <stdio.h>
#include <stdlib.h>

#ifdef NALOPEN
	#include "gkvar.h"
	#include "nlvar.h"
#endif

boolv ym_maxV1(count i, mulv to_verschil);
boolv ym_max_prmV1(count i, count prm, mulv to_verschil);
boolv ym_max_toV1(count i, mulv to_verschil);
boolv ym_max_vtgV1(count i);
void AanvraagSnelV2(count fc1, count dp);
boolv Rateltikkers(count fc,       /* fase */
	count has,      /* hulpelement rateltikkers voor deze fase */
	count has_aan_, /* hulpelement tikkers werking */
	count has_cont_,/* hulpelement tikkers continu */
	count tnlrt,    /* tijd na EG dat de tikkers nog moeten worden aangestuurd indien niet continu */
	...);           /* drukknoppen */
boolv Rateltikkers_Accross(count fc,       /* fase */
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
	boolv *prml[],
	count ml);
void Eerlijk_doseren_VerlengGroenTijden_V1(count hfile,            /* hulpelement wel/geen file */
    count _prmperc,         /* indexnummer parameter % doseren */
    count aantalfc,         /* aantal te doseren fasen */
    count fc[],             /* pointer naar array met fasenummers */
    count fcvg[][MPERIODMAX],        /* pointer naar array met mg parameter index nummers */
    int nogtedoseren[],     /* pointer naar array met nog te doseren waarden */
	boolv *prml[], 
	count ml);
void FileMeldingV2(count det,     /* filelus                                */
    count tbez,    /* bezettijd  als D langer bezet -> file  */
    count trij,    /* rijtijd    als D korter bezet -> !file */
    count tafval,  /* afvalvertraging filemelding            */
    count hfile);   /* hulpelement filemelding                */
void mee_aanvraag_prm(count i, count j, count prm, boolv extra_condition);
void UpdateKnipperSignalen();
boolv hf_wsg_nl(void);
boolv hf_wsg_nl_fcfc(count fc1, count fc2);
void wachttijd_leds_knip(count fc, count mmwtv, count mmwtm, count RR_T_wacht, count fix);
boolv set_FPRML_fk_gkl(count i, boolv *prml[], count ml, count ml_max, boolv period);
boolv kcv_primair_fk_gkl(count i);
void veiligheidsgroen_V1(count fc, count tmaxvag4, ...);
boolv proc_pel_in_V1(                     /* Dh20130124                                                    */
	count hfc,                            /* fasecyclus                                                    */
	count tmeet,                          /* T meetperiode                                                 */
	count tmaxth,                         /* T max.hiaat                                                   */
	count grens,                          /* PRM grenswaarde                                               */
	count mvtg,                           /* MM aantal vtg                                                 */
	count muit,                           /* MM uitsturing aktief                                          */
	...);                                 /* va arg list: inkomende signalen koplussen                     */
boolv IsConflict(count fc1, count fc2);
void ModuleStructuurPRM(count prmfcml, count fcfirst, count fclast, count ml_max, boolv *prml[], boolv yml[], count *mlx, boolv *sml);

extern mulv FC_type[];
extern mulv DVG[];
extern int Knipper_1Hz;

#endif