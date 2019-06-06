/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* CCOL - OV Prioriteit signaalplanstructuur                                              */
/*                                                                                        */
/*                                                                                        */
/* (C) Copyright 2017 Royal HaskoningDHV b.v. All rights reserved.                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* Versie :  2.0                                                                          */
/*           Integratie met uitgebreide OV module CCOL Generator                          */
/* Naam   :  ov_ple.h                                                                     */
/* Datum  :  14-06-2017                                                                   */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */

#ifndef __OV_PLE__
#define __OV_PLE__

extern mulv TXB_PL[FCMAX], TXD_PL[FCMAX];
mulv TVGPL_max[FCMAX];

boolv HoofdRichting[FCMAX];             /* Array met hoofdrichtingen                       */
boolv HoofdRichtingTegenhouden[FCMAX];  /* Tegenhouden hoofdrichting (TXC of minimum groen)*/
boolv HoofdRichtingAfkappenYWPL[FCMAX]; /* Afkappen YW_PL hoofdrichting (na minimum groen) */
boolv HoofdRichtingAfkappenYVPL[FCMAX]; /* Afkappen YV_PL hoofdrichting (na minimum groen) */
int iExtraGroenNaTXD[ovOVMAX];
int iMinimumGroenUitgesteldeHoofdrichting[FCMAX];

/* -------------------------------------------------------------------------------------- */
/* Gereserveerde bitwaarde tbv OV ingrepen tijdens signaalplan                            */
/* -------------------------------------------------------------------------------------- */
#define OV_PLE_BIT          BIT14  /* FM tbv OV ingrepen tijdens PL             */

void BepaalHoofdrichtingOpties(void);
int  TijdTotLaatsteRealisatieMomentConflict(int, int, int);
boolv StartGroenConflictenUitstellen(count, int);
void set_pg_primair_fc_ov_ple(void);
void signaalplan_primair_ov_ple(void);

void OVHalfstarBepaalHoofdrichtingOpties(int, ...);
int  OVHalfstarBepaalPrioriteitsOpties(int);

void OVHalfstarInit(void);
void OVHalfstarSettings(void);
void OVHalfstarOnderMaximum(void);
void OVHalfstarAfkapGroen(void);
void OVHalfstarStartGroenMomenten(void);
void OVHalfstarAfkappen(void);
void OVHalfstarTerugkomGroen(void);
void OVHalfstarGroenVasthouden(void);
void OVHalfstarMeetKriterium(void);

#endif
