#ifndef __NALOPENFUNC__
#define __NALOPENFUNC__

/**************************************************************************
 *  Functie  : NaloopVtg
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2.
 *    Nalooptijd geldt vanaf startgroen aanvoerrichting en duurt tnl lang.
 **************************************************************************/
void NaloopVtg(count fc1, count fc2, count tnl);

/**************************************************************************
 *  Functie  : NaloopVtgDet
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 afhankelijk van gebruikte dk
 *    Nalooptijd geldt vanaf startgroen aanvoerrichting en duurt tnl lang.
 **************************************************************************/
void NaloopVtgDet(count fc1, count fc2, count dk, count hdk, count tnl);

/**************************************************************************
 *  Functie  : NaloopEG
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 vanaf eindgroen fc1
 *    Nalooptijd wordt geherstart tijdens G aanvoerrichting
 **************************************************************************/
void NaloopEG(count fc1, count fc2, count tnl);

/**************************************************************************
 *  Functie  : NaloopEGDet
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 afhankelijk van detectie dp
 *    Nalooptijd geldt groen en geel van de aanvoerrichting en wordt gedurende
 *    het bezet zijn van detector dp geherstart.
 **************************************************************************/
void NaloopEGDet(count fc1, count fc2, count tnl, ...);

/**************************************************************************
 *  Functie  : NaloopEG
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 vanaf eindgroen fc1
 *    Nalooptijd wordt geherstart tijdens cyclisch verlenggroen van de aanvoerrichting
 **************************************************************************/
void NaloopCV(count fc1, count fc2, count tnl);

/**************************************************************************
 *  Functie  : NaloopEGDet
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 afhankelijk van detectie dp
 *    Nalooptijd geldt tijdens cyclisch verlenggroen van de aanvoerrichting en wordt gedurende
 *    het bezet zijn van detector dp geherstart.
 **************************************************************************/
void NaloopCVDet(count fc1, count fc2, count tnl, ...);
/**************************************************************************
 *  Functie  : NaloopFG
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 tijdens vastgroen fc1
 *    Nalooptijd wordt geherstart tijdens FG aanvoerrichting
 **************************************************************************/
void NaloopFG(count fc1, count fc2, count tnl);

/**************************************************************************
 *  Functie  : NaloopFGDET
 *
 *  Functionele omschrijving :
 *    Verzorgt een naloop van fc1 naar fc2 tijdens vastgroen fc1
 *    Nalooptijd wordt geherstart tijdens FG aanvoerrichting en wordt gedurende
 *    het bezet zijn van detector dp geherstart.
 **************************************************************************/

void NaloopFGDet(count fc1, count fc2, count tnl, ...);

/* YMLX[] - VASTHOUDEN MODULE AFWIKKELING */
/* ====================================== */
/* yml_cv_pr-nl() tests CV[] of the phasecycles, with PRIMARY specification
 * in the active module and no PRIMARY specification in the next module.
 * yml_cv_pr() returns TRUE if CV[] && !WS[] or !RW[fci] & BIT2 (fci is nalooprichting) is detected, otherwise FALSE.
 * yml_cv_pr() can be used in the function application() - specification
 * of YMLx[].
 */

#if !defined (CCOLFUNC) || defined (LWMLFUNC2)

boolv yml_cv_pr_nl(boolv *prml[], count ml, count ml_max);

#endif
mulv TVGTMP_max[FCMAX];  /* t.b.v. onthouden verlenggroentijd vanaf SVG[] */

void gk_InitGK(void);

void gk_ResetGK(void);

void gk_ControlGK(void);

void gk_NaloopTGK(count fc1, count fc2, count tnl, boolv per_herstart, count tgl_nl);

void gk_NaloopTNL(count fc1, count fc2, count tnl, boolv per_herstart);

void berekenTGK_max(count fc1, count fc2, count tnl_max);
void berekenTNL(count fc2, count tnl);

#endif /* #define __NALOPENFUNC__ */