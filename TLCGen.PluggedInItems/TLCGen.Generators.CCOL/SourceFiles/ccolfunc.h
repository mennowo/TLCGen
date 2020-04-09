#ifndef __CCOLFUNC_H__
#define __CCOLFUNC_H__
#endif
#ifdef NALOPEN
   #include "nalopen.h"
   #include "gkvar.h"    /* groengroenkonflikten              */
   #include "nlvar.h"    /* nalopen                           */
#endif

#ifdef STARTOPTIES
   #include "startopt.h"
#endif

#ifdef VECOM
   #include "vecom.h"
#endif

#include "uitstuur.h"

void aanvraag_detectie_reset_prm_va_arg(count fc, ...);
void aanvraag_richtinggevoelig_reset(count fc, count d1, count d2, count trga, count tav, mulv schrga);
#ifndef INSTRUCTION_BITS8
void mee_aanvraag_reset(count fcn, count fcv, bool expressie);
#endif
#if MLMAX
bool WStandRi(count fci, bool *prml[], count ml, count ml_max);
void WachtStand(bool *prml[], count ml, count ml_max);
#endif
mulv max_tar_to(count i);
mulv max_tar_ov(count i, ...);
bool AlternatieveRuimte(count fcalt, count fcprim, count paltg);
bool no_conflict(count fc1, count fc2);
bool testpri_gk_calw(count i);
bool set_PRIRLW(count i, bool period);
bool set_ARLW_bit6 (count i);
void langstwachtende_alternatief_bit6(void);


