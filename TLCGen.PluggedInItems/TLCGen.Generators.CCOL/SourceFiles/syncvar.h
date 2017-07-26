/* FILE: SYNCVAR.H */
/* ---------------- */

#ifndef __SYNCVAR_H__
#define __SYNCVAR_H__

#include "sysdef.c"

extern mulv M_R_timer[];          /* realisationtimers                   */
extern mulv *R_timer[];
extern bool A_old[], A_old_old[]; /* t.b.v. negeren start aanvraag       */
#ifdef INSTRUCTION_BITS8
   extern unsigned char KR[];     /* conflicting realisationtimer active */
#else
   extern unsigned short KR[];    /* conflicting realisationtimer active */
#endif
extern mulv SYNCDUMP;             /* dumpflag                            */

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
