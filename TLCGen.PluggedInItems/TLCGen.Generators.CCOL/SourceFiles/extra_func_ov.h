#ifndef EXTRA_FUNC_OV
#define EXTRA_FUNC_OV


/* Structs tbv bijhouden laatste DSI berichten per richting in/uit
tbv voorkomen dubbele in/uit meldingen */
struct prevovkar
{
	count prevlijn, prevvtg, prevtype, prevdir;
};
typedef struct prevovkar prevOVkarstruct;

bool DSIMeldingOV_V1(
	count dslus,
	count vtgtype,
	count fcnmr,
	bool checktype,
	count meldingtype,
	bool checklijn,
	count lijnparm,
	count lijnmax,
	bool extra);

bool OVmelding_KAR_V2(count vtgtype,  /*  1. voertuigtype (CIF_BUS CIF_TRAM CIF_BRA) etc  */
	count dir,                  /*  2. fc nummer of richtingnummer (201, 202, 203)  */
	s_int16 stp,                /*  3. stiptheidsklasse                             */
	count meldingtype,          /*  4. type melding (CIF_DSIN / CIF_DSUIT)          */
	bool cprio,                 /*  5. alleen inmelden bij geconditioneerde prio    */
	s_int16 laatcrit,           /*  6. aantal seconden waarna bus te laat is        */
	count lijnparm,             /*  7. eerste lijnparameter (prmov##_allelijnen)    */
	count bufmax,               /*  8. max aantal lijnen in buffer                  */
struct prevovkar * prevOV,  /*  9. opslag data laatste DSI bericht              */
	count tdh);                  /* 10. hiaat timer tbv voorkomen dubbele melding    */
bool HDmelding_KAR_V1(count vtgtype,  /*  1. voertuigtype (CIF_BUS CIF_TRAM CIF_BRA) etc  */
	count prio,                 /*  2. Voert voertuig SIRENE */
	count dir,                  /*  3. fc nummer of richtingnummer (201, 202, 203)  */
	count meldingtype,          /*  5. type melding (CIF_DSIN / CIF_DSUIT)          */
struct prevovkar * prevOV,  /* 10. opslag data laatste DSI bericht              */
	count tdh);                  /* 11. hiaat timer tbv voorkomen dubbele melding    */

bool OVmelding_DSI_TRAM(
	count seldet1,              /* eerste selectieve detectielus            */
	count seldet2,              /* tweede selectieve detectielus (of NG)    */
	count seldet3,              /* derde selectieve detectielus (of NG)     */
	count lijnparm,             /* eerste lijnparameter (prmov##_allelijnen)*/
	count bufmax);               /* max aantal lijnen in buffer              */
bool OVmelding_DSI_BUS(
	count seldet1,              /* eerste selectieve detectielus            */
	count seldet2,              /* tweede selectieve detectielus (of NG)    */
	count seldet3,              /* derde selectieve detectielus (of NG)     */
	count lijnparm,             /* eerste lijnparameter (prmov##_allelijnen)*/
	count bufmax);               /* max aantal lijnen in buffer              */
#ifdef CCOL_IS_SPECIAL
void reset_DSI_message(void);
void set_DSI_message_KAR(s_int16 vtg, s_int16 dir, count type, s_int16 stiptheid, s_int16 aantalsecvertr, s_int16 PRM_lijnnr, s_int16 prio);
void set_DSI_message(mulv ds, mulv vtg, mulv melding, mulv PRM_lijnnr, mulv stiptheid);
#endif

#endif