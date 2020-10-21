#ifndef EXTRA_FUNC
#define EXTRA_FUNC

#include <stdio.h>
#include <stdlib.h>

#ifdef NALOPEN
	#include "gkvar.h"
	#include "nlvar.h"
#endif

bool ym_maxV1(count i, mulv to_verschil);
bool ym_max_prmV1(count i, count prm, mulv to_verschil);
bool ym_max_toV1(count i, mulv to_verschil);
bool ym_max_vtgV1(count i);
void AanvraagSnelV2(count fc1, count dp);
bool Rateltikkers(count fc,       /* fase */
	count has,      /* hulpelement rateltikkers voor deze fase */
	count has_aan_, /* hulpelement tikkers werking */
	count has_cont_,/* hulpelement tikkers continu */
	count tnlrt,    /* tijd na EG dat de tikkers nog moeten worden aangestuurd indien niet continu */
	bool bewaakt,  /* rateltikker van nieuwe (bewaakte) type?  */
	...);           /* drukknoppen */
bool Rateltikkers_Accross(count fc,       /* fase */
	count has,      /* hulpelement rateltikkers voor deze fase */
	count has_aan_, /* hulpelement tikkers werking */
	count has_cont_,/* hulpelement tikkers continu */
	...);           /* drukknoppen */
bool Rateltikkers_HoeflakeDimming(count fc,/* fase                                           */
                         count hperasdim,  /* hulpelement klokperiode gedimde uitsturing     */
                         count prmasndim,  /* dimnivo periode niet dimmen (0-10, 10 = tikker uit) of NG  */ 
                         count prmasdim);  /* dimnivo periode dimmen (0-10, 10 = tikker uit) of NG  */
void Eerlijk_doseren_V1(count hfile,            /* hulpelement wel/geen file */
	count _prmperc,         /* indexnummer parameter % doseren */
	count aantalfc,         /* aantal te doseren fasen */
	count fc[],             /* pointer naar array met fasenummers */
	count fcmg[][MPERIODMAX],        /* pointer naar array met mg parameter index nummers */
	int nogtedoseren[],     /* pointer naar array met nog te doseren waarden */
	bool *prml[],
	count ml,
	count _mperiod);
void Eerlijk_doseren_VerlengGroenTijden_V1(count hfile,            /* hulpelement wel/geen file */
    count _prmperc,         /* indexnummer parameter % doseren */
    count aantalfc,         /* aantal te doseren fasen */
    count fc[],             /* pointer naar array met fasenummers */
    count fcvg[][MPERIODMAX],        /* pointer naar array met mg parameter index nummers */
    int nogtedoseren[],     /* pointer naar array met nog te doseren waarden */
	bool *prml[], 
	count ml,
	count _mperiod);
void FileMeldingV2(count det,     /* filelus                                */
    count tbez,    /* bezettijd  als D langer bezet -> file  */
    count trij,    /* rijtijd    als D korter bezet -> !file */
    count tafval,  /* afvalvertraging filemelding            */
    count hfile);   /* hulpelement filemelding                */
void mee_aanvraag_prm(count i, count j, count prm, bool extra_condition);
void UpdateKnipperSignalen();
bool hf_wsg_nl(void);
bool hf_wsg_nl_fcfc(count fc1, count fc2);
void wachttijd_leds_knip(count fc, count mmwtv, count mmwtm, count RR_T_wacht, count fix);
bool set_FPRML_fk_gkl(count i, bool *prml[], count ml, count ml_max, bool period);
bool kcv_primair_fk_gkl(count i);
void veiligheidsgroen_V1(count fc, count tmaxvag4, ...);
bool proc_pel_in_V1(                       /* Dh20130124                                                    */
	count hfc,                            /* fasecyclus                                                   */
	count tmeet,                          /* T meetperiode                                                 */
	count tmaxth,                         /* T max.hiaat                                                   */
	count grens,                          /* PRM grenswaarde                                               */
	count mvtg,                           /* MM aantal vtg                                                 */
	count muit,                           /* MM uitsturing aktief                                          */
	...);                                 /* va arg list: inkomende signalen koplussen                     */
bool IsConflict(count fc1, count fc2);
void ModuleStructuurPRM(count prmfcml, count fcfirst, count fclast, count ml_max, bool *prml[], bool yml[], count *mlx, bool *sml);
void SeniorenGroen(count fc, count drk1, count drk1timer, count drk2, count drk2timer,
	count exgperc, count verlengen, count meergroen, ...);
void CyclustijdMeting(count tcyclus, count scyclus, count cond, count sreset, count mlcyclus);
void maximumgroentijden_va_arg(count fc, ...);

extern mulv FC_type[];
extern mulv DVG[];
extern int Knipper_1Hz;
extern int Knipper_2Hz;

#endif