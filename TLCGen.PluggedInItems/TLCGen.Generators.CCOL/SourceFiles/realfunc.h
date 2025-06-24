/*
BESTAND:   realfunc.h
*/

/****************************** Versie commentaar ***********************************
*
* TLCGen  Datum       Naam          Commentaar
*
* 1.0     05-08-2020  OK Geregeld:  Functies t.b.v. realisatietijd en correcties
* 2.0     07-03-2021  Cyril         Geschikt gemaakt voor UC4
* 3.0     14-04-2021                Aanpassingen Steven van Oostendorp verwerkt
* 3.1     16-10-2021                Corr_Min_nl gemaakt waarniet naar de aanwezigheid voor A voor de naloop wordt gekeken
* 3.2     09-11-2021                MG && TGG toegevoegd in berekeningen
* 3.3     06-12-2021                _temp interne variabelen wederom toegevoegd
* 3.4     09-03-2023                MLNLTEST toegevoegd ivm onterechte PG (optioneel zelf te activeren)
* 3.5     07-04-2023                MLNLTEST register count toegevoegd
* 3.6     04-03-2023                Aanpassing Synchroniseer_FO met een controle dat beide fc's moeten in dezelfde module moeten zitten
************************************************************************************/

#ifndef __REALFUNC_H__
#define __REALFUNC_H__

#include "sysdef.c"

/* Extern variables */
extern mulv REALTIJD[FCMAX];
extern mulv REALTIJD_uncorrected[FCMAX];
extern mulv REALTIJD_max[FCMAX];
extern mulv REALTIJD_min[FCMAX];
extern bool REAL_SYN[FCMAX][FCMAX];  /* Vlag tbv synchronisatie      obv REALTIJD */
                                     /* BIT1 TRUE/FALSE                           */
                                     /* BIT2 correctie gelijk (extra info)        */
                                     /* BIT3 correctie plus   (extra info)        */
                                     /* BIT4 correctie min    (extra info)        */
extern bool REAL_FOT[FCMAX][FCMAX];  /* Vlag tbv fictieve ontruiming obv REALTIJD */
extern mulv TIME_FOT[FCMAX][FCMAX];   /* Timer tbv fictieve ontruiming, FOT loopt, resterende tijd */

/* Function prototypes */

/* Realisatietijd berekening functies */
void Realisatietijd(count fc, count hsignaalplan, mulv correctie_sp);
void Realisatietijd_min(count fc, count hsignaalplan, mulv correctie_sp);
void Realisatietijd_MM(count fc, count mrt);

/* Correctie functies */
bool Corr_Real(count fc1, count fc2, mulv t1_t2, bool period);
bool Corr_Real_nl(count fc1, count fc2, mulv t1_t2, bool period);
bool Corr_Gel(count fc1, count fc2, bool period);
bool Corr_Pls(count fc1, count fc2, mulv t1_t2, bool period);
bool Corr_Min(count fc1, count fc2, mulv t1_t2, bool period);
bool Corr_Min_nl(count fc1, count fc2, mulv t1_t2, bool period);
bool Corr_FOT(count fc1, count fc2, count fot1_2, mulv gg1, bool period);

/* Voetgangers en inloop functies */
bool VTG2_Real(count fc1, count fc2, mulv t1_t2, mulv t2_t1, count hdk1_bu, count hdk2_bu, bool gelijk);
bool VTG2_Real_Los(count fc1, count fc2, mulv t1_t2, mulv t2_t1, count hinl1, count hinl2, count hlos1, count hlos2, bool gelijk);
bool VTG3_Real_Los(count fc1, count fc9, count fc2, mulv t1_t2, mulv t2_t1, mulv t1_t9, mulv t9_t1, mulv t2_t9, mulv t9_t2,
                   count hinl1, count hinl9_1, count hinl9_2, count hinl2, count hlos1, count hlos9, count hlos2,
                   bool gelijk1_2, bool gelijk1_9, bool gelijk2_9);
bool Real_Los(count fc1, count fc2, mulv t2_t1, count hlos2, bool gelijk);

/* Inloop hulpfuncties */
void Inlopen_Los3(count fc1, count fc9, count fc2, count hdk1bu, count hdk1bi, count hdk9a, count hdk9b, count hdk2bi, count hdk2bu,
                 count hinl1, count hinl9_1, count hinl9_2, count hinl2, count hlos1, count hlos9, count hlos2,
                 bool sch1_1, bool sch1_2, bool sch2_1, bool sch2_2, bool sch9_1a, bool sch9_1b, bool sch9_2a, bool sch9_2b,
                 bool sch9_3a, bool sch9_3b, bool sch9_4a, bool sch9_4b);
void Inlopen_Los2(count fc1, count fc2, count hdk1bu, count hdk1bi, count hdk2bi, count hdk2bu,
                 count hinl1, count hinl2, count hlos1, count hlos2,
                 bool sch1_1, bool sch1_2, bool sch2_1, bool sch2_2);

/* Naloop en ruimte functies */
bool Naloop_OK(count fc1, count marfc2, count tnlsg);
mulv Real_Ruimte(count fc, count mar);

/* Synchronisatie functies */
void Synchroniseer_SG(void);
void Synchroniseer_SG1_2(count fc1, count fc2);
void Synchroniseer_FO(void);
void Synchroniseer_FO1_2(count fc1, count fc2);
void Synchroniseer_PG(void);
void Synchroniseer_PG1_2(count fc1, count fc2);

#if PLMAX
void Synchroniseer_SP(bool period);
#endif

/* Andere hulpfuncties */
bool Maatgevend_Groen(count fc);

#endif /* __REALFUNC_H__ */