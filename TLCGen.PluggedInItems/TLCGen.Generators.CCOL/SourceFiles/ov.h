#ifndef __OVH
#define __OVH

#define    OV_A_BIT      BIT6
#define    OV_Z_BIT      BIT6
#define   OV_FM_BIT      BIT6
#define   OV_RW_BIT      BIT6
#define   OV_RR_BIT      BIT6
#define   OV_YV_BIT      BIT6
#define   OV_MK_BIT      BIT6
#define   OV_PP_BIT      BIT6
#define  OV_PAR_BIT      BIT6
#define OV_RTFB_BIT      BIT6

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

/* Functie declaraties voor functies uit de applicatie */
void OVInitExtra(void);
void OVInstellingen(void);
void RijTijdScenario(void);
void InUitMelden(void);
void PrioriteitsOpties(void);
void PrioriteitsToekenningExtra(void);
void TegenhoudenConflictenExtra(void);
void PostAfhandelingOV(void);
void OVPARCorrecties(void);
void OnderMaximumExtra(void);
void AfkapGroenExtra(void);
void StartGroenMomentenExtra(void);
void OVAfkappenExtra(void);
void OVTerugkomGroenExtra(void);
void OVGroenVasthoudenExtra(void);
void OVMeetKriteriumExtra(void);

/* Functie declaraties voor functies uit ov.c */
void AfhandelingOV(void);
int BepaalPrioriteitsOpties(int prm_prio);
void OVInit(void);
void OVInmeldenID(int ov,
                  int iInmelding,
                  int iPN,         /* prioriteitsniveau       */
                  int iPO,         /* prioriteitsopties       */
                  int iRT,         /* rijtimer                */
                  int iGBT,        /* groenbewakingstimer     */
                  int iID);        /* identificatie inmelding */
void OVInmelden(int ov,
                int iInmelding,
                int iPN,         /* prioriteitsniveau   */
                int iPO,         /* prioriteitsopties   */
                int iRT,         /* rijtimer            */
                int iGBT);       /* groenbewakingstimer */
void OVUitmelden(int ov,
                 int iUitmelding);
void OVUitmeldenIndex(int ov,
                      int inm,
                      int iUitmelding,
                      bool bGeforceerd);
void OVUitmeldenID(int ov,
                   int iUitmelding,
                   int iID);
int OVAantalInmeldingenID(int ov,
                          int iID);
void OVRijTijdScenario(int ov,
                       int dkop,
                       int dlang,
                       int tbezet);
void OVCcolElementen(int ov, int tgb, int trt, int hprio, int cvc, int tblk);
void OVCcol(void);
void KonfliktTijden(void);
void SKVoorStarten(bool period, count fcvs, count fcls, count tvs, bool bit);
void SKGelijkStarten(bool period, count fc1, count fc2, bool bit, bool overslag_sg);
void SKFietsVoetganger(bool period, count fcfts, count fcvtg, bool bit);
void SKFictiefOntruimen(bool period, count fcv, count fcn, count tftofcvfcn, bool bit);
int StartGroenFC(int fc, int iGewenstStartGroen, int iPrioriteitsOptiesFC);
void TegenHoudenStartGroen(int fc, int iStartGroenFC);
void AfkappenStartGroen(int fc, int iStartGr);
void VLOG_mon5_buffer(void);

#ifdef OV_ADDFILE
extern int iStartGroenFC[];

void RijTijdScenario_Add(void);
void InUitMelden_Add(void);
void OVInstellingen_Add(void);
void WachtTijdBewaking_Add(void);
void KonfliktTijden_Add(void);
void OnderMaximum_Add(void);
void BlokkeringsTijd_Add(void);
void PrioriteitsOpties_Add(void);
void PrioriteitsNiveau_Add(void);
void PrioriteitsToekenning_Add(void);
void AfkapGroen_Add(void);
void StartGroenMomenten_Add(void);
void OVAanvragen_Add(void);
void RealisatieTijden_Add(void);
void OVTegenhouden_Add(void);
void OVAfkappen_Add(void);
void OVAlternatieven_Add(void);
void PostAfhandelingOV_Add(void);
#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM
void OVDebug_Add(void);
#endif
#endif


#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM
void OVDebug(int ov);
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
extern int iH_OVix[];
extern int iBlokkeringsTijd[];
extern int iBlokkeringsTimer[];
extern int iFC_OVix[];
extern int iOnderMaximum[];
extern int iOnderMaximumVerstreken[];
extern int iGroenBewakingsTijd[];
extern int iGroenBewakingsTimer[];
extern int iRijTijd[];
extern int iRijTimer[];
extern int iPrioriteit[];
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
extern int *iRealisatieTijd[];
extern int *iInPrioriteitsNiveau[];
extern int *iInPrioriteitsOpties[];
extern int *iInRijTimer[];
extern int *iInGroenBewakingsTimer[];
extern int *iInOnderMaximumVerstreken[];

extern int ovKFC_MAX[];
extern int *ovTO_pointer[];
extern int iLangstWachtendeAlternatief;

#endif // __OVH