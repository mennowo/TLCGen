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

bool HoofdRichting[FCMAX];             /* Array met hoofdrichtingen                       */
bool HoofdRichtingTegenhouden[FCMAX];  /* Tegenhouden hoofdrichting (TXC of minimum groen)*/
bool HoofdRichtingAfkappenYWPL[FCMAX]; /* Afkappen YW_PL hoofdrichting (na minimum groen) */
bool HoofdRichtingAfkappenYVPL[FCMAX]; /* Afkappen YV_PL hoofdrichting (na minimum groen) */

/* -------------------------------------------------------------------------------------- */
/* Gereserveerde bitwaarde tbv OV ingrepen tijdens signaalplan                            */
/* -------------------------------------------------------------------------------------- */
#define OV_PLE_BIT   BIT8

void OV_ple_init(void);
void OV_ple_settings(void);
int  OV_ple_BepaalPrioriteitsOpties(int);
void OV_ple_BepaalHoofdrichtingOpties(int, ...);
void BepaalHoofdrichtingOpties(void);
int  TijdTotLaatsteRealisatieMomentConflict(int, int, int);
bool StartGroenConflictenUitstellen(count, int);
void set_pg_primair_fc_ov_ple(void);
void signaalplan_primair_ov_ple(void);

#endif
