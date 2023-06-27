#ifndef __ISGFUNC
#define __ISGFUNC

void Realisatietijd_NLEG(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop);
void Realisatietijd_NLEVG(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop);
void InterStartGroenTijd_NLEG(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop);
void InterStartGroenTijd_NLEVG(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop);
void Realisatietijd_NLSG(count i, count j, count tnlsg, count tnlsgd);
void InterStartGroenTijd_NLSG(count i, count j, count tnlsg, count tnlsgd);
void NaloopEG_TVG_Correctie(count fc1, count fc2, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop);
void NaloopEVG_TVG_Correctie(count fc1, count fc2, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop);
bool Correctie_REALISATIETIJD_Voorstart(count fcvs, count fcns, count tvs);
bool Correctie_REALISATIETIJD_Gelijkstart(count fc1, count fc2);
bool Correctie_REALISATIETIJD_LateRelease(count fclr, count fcvs, count tlr);
bool Correctie_TISG_Voorstart(count fcvs, count fcns, count tvs);
bool Correctie_TISG_Gelijkstart(count fc1, count fc2);
bool Correctie_TISG_LateRelease(count fclr, count fcvs, count tlr);
void NaloopVtg_TVG_Correctie(count fc1, count fc2, count tnlsg, count tnlsgd);
void Ontruiming_Deelconflict_Voorstart(count fcns, count fcvs, count tfo);
void Ontruiming_Deelconflict_Gelijkstart(count fc1, count fc2, count tfo12, count tfo21);
void Ontruiming_Deelconflict_LateRelease(count fcvs, count fclr, count tlr, count tfo);
void NaloopVtg(count fc1, count fc2, count dk, count hdk, bool hnlsg, count tnlsg, count tnlsgd);
void NaloopEG(count fc1, count fc2, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop, ...);
void NaloopEVG(count fc1, count fc2, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop, ...);
bool ym_max_tig_REALISATIETIJD(count i, count prmomx);
bool max_par(count fc);
bool max_par_los(fc);
void max_wachttijd_modulen_primair_ISG(bool* prml[], count ml, count ml_max, mulv twacht[]);
bool yml_cv_pr_nl_ISG(bool* prml[], count ml, count ml_max);
void Bepaal_Realisatietijd_per_richting(void);
void set_PG_Deelconflict_Voorstart(mulv fc1, mulv fc2);
set_PG_Deelconflict_LateRelease(mulv fc1, mulv fc2, mulv tlr);
void MeeverlengenUitDoorDeelconflictVoorstart(mulv fc1, mulv fc2);
void MeeverlengenUitDoorDeelconflictLateRelease(mulv fc1, mulv fc2, mulv tlr);
void PercentageVerlengGroenTijdenISG(count fc, mulv percentage);
bool hf_wsg_nlISG(void);
bool afsluiten_aanvraaggebied_prISG(bool* prml[], count ml);
void BepaalVolgrichtingen(void);
void Realisatietijd_MeeverlengenDeelconflict(mulv fc1, mulv fc2);
void InterStartGroentijd_MeeverlengenDeelconflict(mulv fc1, mulv fc2);
void corrigeerTIGRvoorNalopen(count fc1, count fc2, mulv tnleg, mulv tnlegd, mulv tvgnaloop);
void MeeverlengenUitDoorVoetgangerLos(count fcvtg, count hmadk);
void PrioAanwezig(void);
void BepaalIntergroenTijden(void);
void BepaalIntersignaalgroepTijden(void);
void RealisatieTijden_VulHaldeConflictenIn(void);
void RealisatieTijden_VulGroenGroenConflictenIn(void);
void InterStartGroenTijden_VulHaldeConflictenIn(void);
void InterStartGroenTijden_VulGroenGroenConflictenIn(void);
void CorrigeerIntersignaalgroepTijdObvGarantieTijden(void);
void InitInterStartGroenTijden();
mulv Real_Ruimte(count fc, count mar);
bool Naloop_OK(count fc1, count marfc2, count tnlsg);

extern mulv TNL_type[][FCMAX]; /* type naloop */
extern mulv FK_type[][FCMAX]; /* type fictief conflict */
extern boolv AfslaandDeelconflict[];
extern boolv TNL[FCMAX];

#define offsetAR    5
/* Type naloop */
#define TNL_NG	-1 /* Geen */
#define TNL_SG	 0 /* Naloop van StartGroen bv TNL_type[fc31][fc32] = TNL_SG*/
#define TNL_ECV	 1 /* Naloop vanaf Einde verlenggroen bv TNL_type[fc22][fc21] = TNL_ECV */
#define TNL_EG	 2 /* Naloop vanaf EindeGroen nv TNL_type[fc02][fc62] = TNL_EG*/ 

/* Type fictief conflict*/
#define FK_NG		-1 /* Geen */
#define FK_SG		0  /* bijv FK_type[fc32][fc02] = FK_SG  Starttijd 02 kan worden bepaald op basis  StartGroen van fc32  */
#define FK_EVG  	1  /* bv FK_type[fc22][fc02] = FK_EV  Starttijd 02 kan worden bepaald op basis van EindeVerlengGroen van fc22 */  
#define FK_EG		2  /* bv FK_type[fc02][fc05] = FK_EG Starttijd 05 kan worden bepaald op bais van Einde Groen fc02 */

#endif /* __ISGFUNC */
