/* FILE: SYNCVAR.C */
/* ---------------- */

#ifndef __SYNCVAR_C__
#define __SYNCVAR_C__

#include "sysdef.c"

mulv M_R_timer[FCMAX*FCMAX];          /* realisationtimers                   */
mulv *R_timer[FCMAX];
boolv A_old[FCMAX], A_old_old[FCMAX]; /* t.b.v. negeren start aanvraag       */
#ifdef INSTRUCTION_BITS8
   unsigned char KR[FCMAX];           /* conflicting realisationtimer active */
#else
   unsigned short KR[FCMAX];          /* conflicting realisationtimer active */
#endif
mulv SYNCDUMP;                        /* dumpflag                            */

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
