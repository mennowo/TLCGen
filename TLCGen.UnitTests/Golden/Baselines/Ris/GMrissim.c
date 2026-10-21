/* APPLICATIE RIS SIMULATIEPROGRAMMA */
/* --------------------------------- */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMrissim.c
      CCOL:   11.0
    TLCGEN:   
   CCOLGEN:   UnitTest
*/

/****************************** Versie commentaar ***********************************
 *
 * Versie   Datum   Ontwerper   Commentaar
 *
 ************************************************************************************/

/* INCLUDE FILES */
/* ============= */
#include <stdio.h>

#include "GMsys.h"
#include "cif.inc"            /* declaratie CIF_IS[]           */
#include "isvar.h"            /* declaratie IS[]               */
#include "rissimvar.c"        /* ris-simulatie variabelen      */
#include "xyprintf.h"         /* declaratie xyprintf-functie   */
#include "prmvar.h"           /* declaratie PRM[]              */

#if !defined AUTOMAAT && !defined AUTOMAAT_TEST
    extern boolv display;
#endif

/* RIS-FI - ObjectID<Intersection, LaneID en ObjectID<SIgnalGroupID> */
/* ================================================================= */
/* The ID of the intersection can be retrieved from the ITF controlData section, 
 * element “name” in “controlledIntersection” (SYSTEM_ITF).
 * The LaneNr is a unique number within the intersection (LaneID).
 * The (TLC) ID of the signal group can be retrieved from the ITF controlData section, 
 * element “name” in “sg” (FC_code[]).
 */
/*RIS SIMULATIE PARAMETERS */
/*======================== */

void ris_simulation_parameters(void)
{
    #ifdef RISSIMULATIE
    #endif /* RISSIMULATIE */

}

/* RIS SIMULATIE APPLICATIE */
/* ======================== */

#define SIS(is)  (IS[is] && !IS_old[is])
void ris_simulation_application(void)
{
    #if (!defined AUTOMAAT_TEST)

    /* Display ris_lanes met ItsStations */
    /* --------------------------------- */
    if (display) {
    }

    /* Display aantal ItsStations en PrioRequests */
    /* ------------------------------------------ */  
    if (display) {
        xyprintf(32, 17, "ItsStation  =% -3d ItsStation - Ex =% -3d", RIS_ITSSTATION_AP_NUMBER,  RIS_ITSSTATION_EX_AP_NUMBER);
        xyprintf(32, 18, "PrioRequest =% -3d PrioRequest_Ex  =% -3d", RIS_PRIOREQUEST_AP_NUMBER, RIS_PRIOREQUEST_EX_AP_NUMBER);
    }
    #endif
}
