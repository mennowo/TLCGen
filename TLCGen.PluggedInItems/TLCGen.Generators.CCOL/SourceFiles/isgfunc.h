#ifndef __ISGFUNC
#define __ISGFUNC


/* Type naloop */
#define TNL_NG	-1 /* Geen */
#define TNL_SG	 0 /* Naloop van StartGroen bv TNL_type[fc31][fc32] = TNL_SG*/
#define TNL_ECV	 1 /* Naloop vanaf Einde verlenggroen bv TNL_type[fc22][fc21] = TNL_ECV */
#define TNL_EG	 2 /* Naloop vanaf EindeGroen nv TNL_type[fc02][fc62] = TNL_EG*/ 

/* Type fictief conflict*/
#define FK_NG	       -1 /* Geen */
#define FK_SG		0  /* bijv FK_type[fc32][fc02] = FK_SG  Starttijd 02 kan worden bepaald op basis  StartGroen van fc32  */
#define FK_EVG  	1  /* bv FK_type[fc22][fc02] = FK_EV  Starttijd 02 kan worden bepaald op basis van EindeVerlengGroen van fc22 */  
#define FK_EG		2  /* bv FK_type[fc02][fc05] = FK_EG Starttijd 05 kan worden bepaald op bais van Einde Groen fc02 */

#define offsetAR    5

extern mulv TNL_type[][FCMAX]; /* type naloop */
extern mulv FK_type[][FCMAX]; /* type fictief conflict */

extern boolv AfslaandDeelconflict[];

void BepaalIntergroenTijden(void);
void corrigeerTIGRvoorNalopen(count fc1, count fc2, mulv tnleg, mulv tnlegd, mulv tvgnaloop);

void InitRealisatieTijden(void);
void RealisatieTijden_VulHardeConflictenIn(void);
void RealisatieTijden_VulGroenGroenConflictenIn(void);
void CorrigeerRealisatieTijdenObvGarantieTijden(void);
void Realisatietijd_NLEG(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop);
void Realisatietijd_NLEVG(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop);
void Realisatietijd_NLSG(count i, count j, count tnlsg, count tnlsgd);
void Realisatietijd_HardMeeverlengenDeelconflict(mulv fc1, mulv fc2);
void Realisatietijd_Ontruiming_Voorstart(count fcns, count fcvs, count tfo);
void Realisatietijd_Ontruiming_Gelijkstart(count fc1, count fc2, count tfo12, count tfo21);
void Realisatietijd_Ontruiming_LateRelease(count fcvs, count fclr, count tlr, count tfo);
boolv Realisatietijd_Voorstart_Correctie(count fcvs, count fcns, count tvs);
boolv Realisatietijd_Gelijkstart_Correctie(count fc1, count fc2);
boolv Realisatietijd_LateRelease_Correctie(count fclr, count fcvs, count tlr);
void Bepaal_Realisatietijd_per_richting(void);
boolv ym_max_tig_Realisatietijd(count i, count prmomx);
void TegenhoudenDoorRealisatietijden();

void InitInterStartGroenTijden();
void InterStartGroenTijden_VulHardeConflictenIn(void);
void InterStartGroenTijden_VulGroenGroenConflictenIn(void);
void InterStartGroenTijd_NLEG(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop);
void InterStartGroenTijd_NLEVG(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop);
void InterStartGroenTijd_NLSG(count i, count j, count tnlsg, count tnlsgd);
void InterStartGroentijd_HardMeeverlengenDeelconflict(mulv fc1, mulv fc2);
boolv InterStartGroenTijd_Voorstart_Correctie(count fcvs, count fcns, count tvs);
boolv InterStartGroenTijd_Gelijkstart_Correctie(count fc1, count fc2);
boolv InterStartGroenTijd_LateRelease_Correctie(count fclr, count fcvs, count tlr);

void NaloopEG_TVG_Correctie(count fc1, count fc2, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop);
void NaloopEVG_TVG_Correctie(count fc1, count fc2, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop);
void NaloopVtg_TVG_Correctie(count fc1, count fc2, count tnlsg, count tnlsgd);

void NaloopVtg(count fc1, count fc2, count dk, count hdk, boolv hnlsg, count tnlsg, count tnlsgd);
void NaloopEG(count fc1, count fc2, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop, ...);
void NaloopEVG(count fc1, count fc2, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop, ...);

boolv max_par(count fc);
boolv max_par_los(fc);
void max_wachttijd_modulen_primair_ISG(boolv* prml[], count ml, count ml_max, mulv twacht[]);
boolv yml_cv_pr_nl_ISG(boolv* prml[], count ml, count ml_max);
void set_PG_Deelconflict_Voorstart(mulv fc1, mulv fc2);
set_PG_Deelconflict_LateRelease(mulv fc1, mulv fc2, mulv tlr);
void MeeverlengenUitDoorDeelconflictVoorstart(mulv fc1, mulv fc2);
void MeeverlengenUitDoorDeelconflictLateRelease(mulv fc1, mulv fc2, mulv tlr);
void MeeverlengenUitDoorVoetgangerLos(count fcvtg, count hmadk);
void PercentageVerlengGroenTijdenISG(count fc, mulv percentage);
boolv hf_wsg_nlISG(void);
boolv afsluiten_aanvraaggebied_prISG(boolv* prml[], count ml);
void BepaalVolgrichtingen(void);
void PrioAanwezig(void);

void InitInterfunc();
void IsgDebug();

void IsgCorrectieTvgPrTvgMax();
void IsgCorrectieTvgTimerTvgMax();

void InitInterStartGroenTijden_rgv();
void InterStartGroenTijden_VulHaldeConflictenIn_rgv(void);
void InterStartGroenTijden_VulGroenGroenConflictenIn_rgv(void);
void InterStartGroenTijd_NLEG_rgv(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop);
void InterStartGroenTijd_NLEVG_rgv(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop);
void InterStartGroenTijd_NLSG_rgv(count i, count j, count tnlsg, count tnlsgd);
void InterStartGroentijd_MeeverlengenDeelconflict_rgv(mulv fc1, mulv fc2);
bool Correctie_TISG_Voorstart_rgv(count fcvs, count fcns, count tvs);
bool Correctie_TISG_Gelijkstart_rgv(count fc1, count fc2);
bool Correctie_TISG_LateRelease_rgv(count fclr, count fcvs, count prmlr);

#endif /* __ISGFUNC */
