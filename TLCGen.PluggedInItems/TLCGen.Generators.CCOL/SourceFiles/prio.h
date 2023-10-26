#ifndef __PRIOH
#define __PRIOH

#define    PRIO_A_BIT      BIT6
#define    PRIO_Z_BIT      BIT6
#define   PRIO_FM_BIT      BIT6
#define   PRIO_RW_BIT      BIT6
#define   PRIO_RR_BIT      BIT6
#define   PRIO_YV_BIT      BIT6
#define   PRIO_YM_BIT      BIT6
#define   PRIO_MK_BIT      BIT11 /* Hier geen BIT6 wegens conflict met MeetKriteriumRGprm */
#define   PRIO_PP_BIT      BIT6
#define  PRIO_PAR_BIT      BIT6
#define PRIO_RTFB_BIT      BIT6

typedef enum
{
	poGeenPrioriteit = 0,
	poAanvraag = 1,
	poAfkappenKonfliktRichtingen = 2,
	poGroenVastHouden = 4,
	poBijzonderRealiseren = 8,
	poAfkappenKonflikterendOV = 16,
	poNoodDienst = 32,
	poPLGroenVastHoudenNaTXD = 64,
	poPLTegenhoudenHoofdrichting = 128,
	poPLAbsolutePrioriteit = 256,
	poPLNoodDienst = 512,
} TPrioriteitsOpties;

typedef enum
{
	rtsOngehinderd,
	rtsBeperktGehinderd,
	rtsGehinderd,
} TRijTijdScenario;

#if !defined VLOGMON5STRUCTURE
typedef struct {
	bool voorinov;
	bool inmov;
	bool uitmov;
	bool uitmbewov;
	bool foutuitmov;
	bool uituitmov;
	bool voorinhd;
	bool inmhd;
	bool uitmhd;
	bool uitmbewhd;
} VLOG_MON5_STRUCT;
extern VLOG_MON5_STRUCT VLOG_mon5[FCMAX];
#define VLOGMON5STRUCTURE
#endif

/* Functie declaraties voor functies uit de applicatie */
void PrioInitExtra(void);
void PrioInstellingen(void);
void RijTijdScenario(void);
void InUitMelden(void);
void PrioriteitsOpties(void);
void PrioriteitsToekenningExtra(void);
void TegenhoudenConflictenExtra(void);
void PostAfhandelingPrio(void);
void PrioPARCorrecties(void);
void OnderMaximumExtra(void);
void AfkapGroenExtra(void);
void StartGroenMomentenExtra(void);
void PrioAfkappenExtra(void);
void PrioTerugkomGroenExtra(void);
void PrioGroenVasthoudenExtra(void);
void PrioMeetKriteriumExtra(void);

/* Functie declaraties voor functies uit prio.c */
void AfhandelingPrio(void);
int BepaalPrioriteitsOpties(int prm_prio);
void PrioInit(void);
void PrioInmeldenID(int ov,
	int iInmelding,
	int iPN,         /* prioriteitsniveau       */
	int iPO,         /* prioriteitsopties       */
	int iRT,         /* rijtimer                */
	int iGBT,        /* groenbewakingstimer     */
	int iID);        /* identificatie inmelding */
void PrioInmelden(int ov,
	int iInmelding,
	int iPN,         /* prioriteitsniveau   */
	int iPO,         /* prioriteitsopties   */
	int iRT,         /* rijtimer            */
	int iGBT);       /* groenbewakingstimer */
void PrioUitmelden(int ov,
	int iUitmelding);
void PrioUitmeldenIndex(int ov,
	int inm,
	int iUitmelding,
	bool bGeforceerd);
void PrioUitmeldenID(int ov,
	int iUitmelding,
	int iID);
int PrioAantalInmeldingenID(int ov,
	int iID);
void PrioRijTijdScenario(int ov,
	int dkop,
	int dlang,
	int tbezet);
void PrioCcolElementen(int prio, int tgb, int trt, int hprio, int cvc, int tblk);
void PrioCcol(void);
void KonfliktTijden(void);
void SKVoorStarten(bool period, count fcvs, count fcls, count tvs, bool bit);
void SKGelijkStarten(bool period, count fc1, count fc2, bool bit, bool overslag_sg);
void SKFietsVoetganger(bool period, count fcfts, count fcvtg, bool bit);
void SKFictiefOntruimen(bool period, count fcv, count fcn, count tftofcvfcn, bool bit);
int StartGroenFC(int fc, int iGewenstStartGroen, int iPrioriteitsOptiesFC);
void TegenHoudenStartGroen(int fc, int iStartGroenFC);
void AfkappenStartGroen(int fc, int iStartGr);
void VLOG_mon5_buffer(void);

#ifdef PRIO_ADDFILE

void RijTijdScenario_Add(void);
void InUitMelden_Add(void);
void PrioInstellingen_Add(void);
void WachtTijdBewaking_Add(void);
void KonfliktTijden_Add(void);
void OnderMaximum_Add(void);
void BlokkeringsTijd_Add(void);
void PrioriteitsOpties_Add(void);
void PrioriteitsNiveau_Add(void);
void PrioriteitsToekenning_Add(void);
void AfkapGroen_Add(void);
void StartGroenMomenten_Add(void);
void PrioAanvragen_Add(void);
void RealisatieTijden_Add(void);
void PrioTegenhouden_Add(void);
void PrioAfkappen_Add(void);
void PrioAlternatieven_Add(void);
void PostAfhandelingPrio_Add(void);
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM
void PrioDebug_Add(void);
#endif
#endif


#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM
void PrioDebug(int ov);
#endif

extern int iMaximumWachtTijdOverschreden[];
extern int iMaximumWachtTijd[];
extern int iVerstrekenGroenTijd[];
extern int iAfkapGroen[];
extern int iAfkapGroenTijd[];
extern int iPercGroenTijd[];
extern int iKonfliktTijd[];
extern int iTerugKomGroenTijd[];
extern int iTerugKomen[];
extern int iInstPercMaxGroenTijdTerugKomen[];
extern int iMaxGroenTijdTerugKomen[];
extern int iInstMinTerugKomGroenTijd[];
extern int iAantalMalenNietAfkappen[];
extern int iInstAantalMalenNietAfkappen[];
extern int iNietAfkappen[];
extern int iMaxGroen[];
extern int iPRM_ALTP[];
extern int iSCH_ALTG[];
extern int iInstAfkapGroenAlt[];
extern int iInstOphoogPercentageMG[];
extern int iOphoogPercentageMG[];
extern int iT_GBix[];
extern int iH_PRIOix[];
extern int iBlokkeringsTijd[];
extern int iBlokkeringsTimer[];
extern int iFC_PRIOix[];
extern int iOnderMaximum[];
extern int iOnderMaximumVerstreken[];
extern int iGroenBewakingsTijd[];
extern int iGroenBewakingsTimer[];
extern int iRijTijd[];
extern int iRijTimer[];
extern int iPrioriteit[];
extern int iPrioriteitNooitAfkappen[];
extern int iKOVPrio[];
extern int iInstPrioriteitsNiveau[];
extern int iInstPrioriteitsOpties[];
extern int iPrioriteitsNiveau[];
extern int iPrioriteitsOpties[];
extern int iStartGroen[];
extern int iBijzonderRealiseren[];
extern int iWachtOpKonflikt[];
extern int iAantalPrioriteitsInmeldingen[];
extern int iRijTijdScenario[];
extern int iRTSOngehinderd[];
extern int iRTSBeperktGehinderd[];
extern int iRTSGehinderd[];
extern int iSelDetFoutNaGB[];
extern int iSelDetFout[];
extern int iAantalInmeldingen[];
extern int iXPrio[];

extern int* iRealisatieTijd[];
extern int* iInPrioriteitsNiveau[];
extern int* iInPrioriteitsOpties[];
extern int* iInRijTimer[];
extern int* iInGroenBewakingsTimer[];
extern int* iInOnderMaximumVerstreken[];
extern int* iPrioMeeRealisatie[];

extern int prioKFC_MAX[];
extern int prioFKFC_MAX[];
extern int* prioTO_pointer[];
extern int iLangstWachtendeAlternatief;

extern bool bHDAanwezig;

#endif // __PRIOH