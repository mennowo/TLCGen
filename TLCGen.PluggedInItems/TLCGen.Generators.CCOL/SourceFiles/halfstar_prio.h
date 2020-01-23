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
/* Naam   :  PRIO_ple.h                                                                     */
/* Datum  :  14-06-2017                                                                   */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */

#ifndef __PRIO_PLE__
#define __PRIO_PLE__

extern mulv TXB_PL[FCMAX], TXD_PL[FCMAX];
mulv TVGPL_max[FCMAX];

bool HoofdRichting[FCMAX];             /* Array met hoofdrichtingen                       */
bool HoofdRichtingTegenhouden[FCMAX];  /* Tegenhouden hoofdrichting (TXC of minimum groen)*/
bool HoofdRichtingAfkappenYWPL[FCMAX]; /* Afkappen YW_PL hoofdrichting (na minimum groen) */
bool HoofdRichtingAfkappenYVPL[FCMAX]; /* Afkappen YV_PL hoofdrichting (na minimum groen) */
int iExtraGroenNaTXD[prioFCMAX];
int iMinimumGroenUitgesteldeHoofdrichting[FCMAX];

/* -------------------------------------------------------------------------------------- */
/* Gereserveerde bitwaarde tbv OV ingrepen tijdens signaalplan                            */
/* -------------------------------------------------------------------------------------- */
#define PRIO_PLE_BIT          BIT14  /* FM tbv OV ingrepen tijdens PL             */

void BepaalHoofdrichtingOpties(void);
int  TijdTotLaatsteRealisatieMomentConflict(int, int, int);
bool StartGroenConflictenUitstellen(count, count, int);
void set_pg_primair_fc_PRIO_ple(void);
void signaalplan_primair_PRIO_ple(void);

void PrioHalfstarBepaalHoofdrichtingOpties(int, ...);
int  PrioHalfstarBepaalPrioriteitsOpties(int);

void PrioHalfstarInit(void);
void PrioHalfstarSettings(void);
void PrioHalfstarOnderMaximum(void);
void PrioHalfstarAfkapGroen(void);
void PrioHalfstarStartGroenMomenten(void);
void PrioHalfstarTegenhouden(void);
void PrioHalfstarAfkappen(void);
void PrioHalfstarTerugkomGroen(void);
void PrioHalfstarGroenVasthouden(void);
void PrioHalfstarMeetKriterium(void);

#endif
