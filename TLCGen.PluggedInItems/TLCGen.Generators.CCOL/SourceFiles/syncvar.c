/* FILE: SYNCVAR.C */
/* ---------------- */

#ifndef __SYNCVAR_C__
#define __SYNCVAR_C__

#include "sysdef.c"

mulv M_R_timer[FCMAX*FCMAX];          /* realisationtimers                   */
mulv *R_timer[FCMAX];
bool A_old[FCMAX], A_old_old[FCMAX];  /* t.b.v. negeren start aanvraag       */
#ifdef INSTRUCTION_BITS8
   unsigned char KR[FCMAX];           /* conflicting realisationtimer active */
#else
   unsigned short KR[FCMAX];          /* conflicting realisationtimer active */
#endif
mulv SYNCDUMP;                        /* dumpflag                            */

void init_realisation_timers(void);
void control_realisation_timers(void);
void correction_realisation_timers(count fcv, count fcn, count tcorrection, bool bit);
void print_realisation_timers(void);
void dump_realisation_timers(void);
void FictiefOntruimen(bool period, count fcv, count fcn, count tftofcvfcn, bool bit);
void FictiefOntruimen_correctionKR(bool period, count fcv, count fcn, count tftofcvfcn);
void VoorStarten_correctionKR(bool period, count fcvs, count fcls, count tvs);
void GelijkStarten_correctionKR(bool period, count fc1, count fc2);
void FietsVoetganger_correctionKR(bool period, count fcfts, count fcvtg);
void VoorStarten(bool period, count fcvs, count fcls, count tvs, bool bit);
void GelijkStarten(bool period, count fc1, count fc2, bool bit, bool overslag_sg);
void FietsVoetganger(bool period, count fcfts, count fcvtg, bool bit);
void realisation_timers(bool bit);

#endif
