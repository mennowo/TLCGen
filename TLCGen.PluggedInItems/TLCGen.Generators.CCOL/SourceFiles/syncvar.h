/* FILE: SYNCVAR.H */
/* ---------------- */

#ifndef __SYNCVAR_H__
#define __SYNCVAR_H__

#include "sysdef.c"
#include "booldef.h"

extern mulv M_R_timer[];          /* realisationtimers                   */
extern mulv *R_timer[];
extern boolv A_old[], A_old_old[]; /* t.b.v. negeren start aanvraag       */
#ifdef INSTRUCTION_BITS8
   extern unsigned char KR[];     /* conflicting realisationtimer active */
#else
   extern unsigned short KR[];    /* conflicting realisationtimer active */
#endif
extern mulv SYNCDUMP;             /* dumpflag                            */

void init_realisation_timers(void);
void control_realisation_timers(void);
void correction_realisation_timers(count fcv, count fcn, count tcorrection, boolv bit);
void print_realisation_timers(void);
void dump_realisation_timers(void);
void FictiefOntruimen(boolv period, count fcv, count fcn, count tftofcvfcn, boolv bit);
void FictiefOntruimen_correctionKR(boolv period, count fcv, count fcn, count tftofcvfcn);
void VoorStarten_correctionKR(boolv period, count fcvs, count fcls, count tvs);
void GelijkStarten_correctionKR(boolv period, count fc1, count fc2);
void FietsVoetganger_correctionKR(boolv period, count fcfts, count fcvtg);
void VoorStarten(boolv period, count fcvs, count fcls, count tvs, boolv bit);
void GelijkStarten(boolv period, count fc1, count fc2, boolv bit, boolv overslag_sg);
void FietsVoetganger(boolv period, count fcfts, count fcvtg, boolv bit);
void realisation_timers(boolv bit);

#endif
