#ifndef ISGFUNC_PRIO_H
#define ISGFUNC_PRIO_H

#include "isgfunc.h"

/* Macro definitions */
#define PRIO_AA_BIT BIT6

/* Global variables */
extern mulv TVG_BR[FCMAX];
extern mulv TVG_afkap[FCMAX];
extern mulv TISG_afkap[FCMAX][FCMAX];
extern mulv TISG_BR[FCMAX][FCMAX];
extern mulv TVG_max_voor_afkap[FCMAX];
extern mulv TVG_AR_voor_afkap[FCMAX];

extern boolv TVG_max_opgehoogd[FCMAX];
extern boolv TVG_AR_opgehoogd[FCMAX];
extern boolv RW_OV[FCMAX];

/* Function prototypes */
void BepaalInterStartGroenTijden_PRIO(void);

void InterStartGroenTijd_NLEG_PRIO(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop);
void InterStartGroenTijd_NLEVG_PRIO(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop);
void InterStartGroenTijd_NLSG_PRIO(count i, count j, count tnlsg, count tnlsgd);

boolv TISG_Voorstart_PRIO_Correctie(count fcvs, count fcns, count tvs);
boolv TISG_Gelijkstart_PRIO_Correctie(count fc1, count fc2);
boolv TISG_LateRelease_PRIO_Correctie(count fclr, count fcvs, count tlr);

void BepaalTVG_BR(void);
void VerhoogTVG_maxDoorPrio(void);
void VerlaagTVG_maxDoorConfPrio(void);
void PrioMeetKriteriumISG(void);
int StartGroenFCISG(int fc, int iGewenstStartGroen, int iPrioriteitsOptiesFC);
void BepaalStartGroenMomentenPrioIngrepen(void);
void PasTVG_maxAanStartGroenMomentenPrioIngrepen(void);
void TegenHoudenStartGroenISG(int fc, int iStartGroenFC, boolv Afkappen);
void PrioBijzonderRealiserenISG(void);
void PrioTegenhoudenISG(void);
void MeeverlengenUitDoorPrio(void);
void PasRealisatieTijdenAanVanwegeRRPrio(void);
void PrioriteitsToekenning_ISG(void);
void PrioInit_ISG(void);
void PrioriteitsToekenning_ISG_Add(void);

boolv fkra(count i);

void PrioMeerealisatieDeelconflictVoorstart(mulv fc1, mulv fc2, mulv tvs);
void PrioMeerealisatieDeelconflictLateRelease(mulv fc1, mulv fc2, mulv tlr);
void InterStartGroentijd_MeeverlengenDeelconflict_PRIO(mulv fc1, mulv fc2);
void PasRealisatieTijdenAanVanwegeBRLateRelease(count fc);

void ResetIsgVars(void);
void VulHardEnGroenConflictenInPrioVars(void);
void ResetNietGroentijdOphogen(void);
void VerhoogGroentijdNietTijdensInrijden(count fc1, count fc2, count txnlfc1fc2);

#endif /* ISGFUNC_PRIO_H */