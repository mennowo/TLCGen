/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* CCOL - OV Prioriteit signaalplanstructuur                                              */
/*                                                                                        */
/*                                                                                        */
/* (C) Copyright 2015 RHDHV b.v. All rights reserved.                                       */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* Versie :  1.1                                                                          */
/* Naam   :  rhdhv_ov_ple.h                                                               */
/* Datum  :  12-10-2010                                                                   */
/*           07-12-2015 kla: punctualiteit toegevoegd                                     */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */

#ifndef __OV_PLE__
#define __OV_PLE__

extern mulv TXB_PL[FCMAX], TXD_PL[FCMAX];

/* -------------------------------------------------------------------------------------- */
/* Gereserveerde bitwaarde tbv OV ingrepen tijdens signaalplan                            */
/* -------------------------------------------------------------------------------------- */

#define OV_PLE_BIT          BIT14  /* FM tbv OV ingrepen tijdens PL             */

void OV_ple_init(void);
void OVSettingsHalfstar(void);
void AlternatievePlannen(int, ...);
bool PasSignaalplanToe(bool);
void HoofdrichtingOpties(int, ...);
void BijhoudenWachtTijd(void);
bool WachttijdOverschrijding(count, count);
void minimum_groentijden_ovprio_va_arg(count, ...);
void BijhoudenMinimumGroenTijden(void);
int  BepaalRealisatieRuimte(count);
int  TijdTotLaatsteRealisatieMomentConflict(int, int);
mulv BepaalGrootsteTO(count);
bool StartGroenConflictenUitstellen(count);
bool StartGroenHoofdRichtingenUitstellen(count);
bool HoofdRichtingGroen(void);
int  TOTXB_AlternatiefSignaalplan(count);
int  TOTXB_Hoofdrichting(int, int);
void set_pg_primair_fc_ov_ple(void);
void signaalplan_primair_ov_ple(void);

void OV_CCOL_Elementen_ple(count fc,      /* OV richting                                  */
                           bool  inm,     /* inmeldvoorwaarde (SH[], SD[])                */
                           bool  inm2,    /* tweede inmeldvoorwaarde (SH[], SD[] of NG)   */
                           bool  uitm,    /* uitmeldvoorwaarde (SH[], SD[])               */
                           bool  uitm2,   /* tweede uitmeldvoorwaarde (SH[], SD[] of NG)  */
                           count cvc,     /* ov teller                                    */
                           count cvb,     /* ov buffer teller                             */
                           count tiv,     /* inmeldingsvertraging                         */
                           count tib,     /* inmeldingsbewaking                           */
                           count tgb,     /* groenbewaking                                */
                           count tblk,    /* blokkeringstijd                              */
                           count textra,  /* extra groentijd na TXD                       */
                           count hprio,   /* hulpelement busprioriteit                    */
                           mulv  prio,    /* waarde prioriteitstype                       */
                           mulv  omax,    /* waarde ondermaximum                          */
                           count pmwt,    /* parameter maximum wachttijd (1e fc)          */
                           bool  ov_mag); /* extra voorwaarde voor toestaan OV prio       */

void OVIngreep_ple(count fc,              /* fc met prioriteit                            */
                   bool  inm,             /* inmeldvoorwaarde (SH[], SD[])                */
                   bool  inm2,            /* tweede inmeldvoorwaarde (SH[], SD[] of NG)   */
                   bool  uitm,            /* uitmeldvoorwaarde (SH[], SD[])               */
                   bool  uitm2,           /* tweede uitmeldvoorwaarde (SH[], SD[] of NG)  */
                   count cvc,             /* ov teller                                    */
                   count cvb,             /* ov buffer teller                             */
                   count tiv,             /* inmeldingsvertraging                         */
                   count tib,             /* inmeldingsbewaking                           */
                   count tgb,             /* groenbewaking                                */
                   count tblk,            /* blokkeringstijd                              */
                   mulv  textra,          /* extra groentijd na TXD                       */
                   count hprio,           /* ov prioriteit                                */
                   count rijtijd,         /* ongehinderde rijtijd tot stopstreep          */
                   mulv  prio,            /* prioriteitstype                              */
                   mulv  omax,            /* waarde ondermaximum                          */
                   count pmwt,            /* parameter maximum wachttijd (1e fc)          */
                   bool  ov_mag);         /* extra voorwaarde voor toestaan OV prio       */



#endif
