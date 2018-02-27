/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* CCOL - standaard bibliotheek Royal HaskoningDHV voor TPA generator V3.40               */
/*                                                                                        */
/*                                                                                        */
/* (C) Copyright 2015 Royal HaskoningDHV b.v. All rights reserved.                        */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
/*                                                                                        */
/* Versie :  1.0                                                                          */
/*           Eerste straatversie                                                          */
/* Naam   :  rhdhv_gen.h                                                                  */
/* Datum  :  07-12-2015                                                                   */
/*                                                                                        */
/* -------------------------------------------------------------------------------------- */
#ifndef __RHDHV_GEN__
#define __RHDHV_GEN__

/*
bit   hex     dec
BIT0  0x1     1
BIT1  0x2     2
BIT2  0x4     4
BIT3  0x8     8
BIT4  0x10    16
BIT5  0x20    32
BIT6  0x40    64
BIT7  0x80    128
BIT8  0x100   256
BIT9  0x200   512
BIT10 0x400   1024
BIT11 0x800   2048
BIT12 0x1000  4096
BIT13 0x2000  8192
BIT14 0x4000  16384
BIT15 0x8000  32768
*/





/* maximum waarde int16 */
#define MAX_VALUE_INT     32767  

bool H_WSG[FCMAX];          /* t.b.v. gekoppelde aanvoerrichting niet meeverlengen met naloop */

count RHDHV_MAX_WT[FCMAX];  /* bijhouden van de maximale wachttijd per richting */

/* tbv interne timers */
bool RHDHV_SSEC;                /* puls op start seconde                        */
bool RHDHV_SMIN;                /* puls op start minuut                         */
bool RHDHV_SUUR;                /* puls op start uur                            */
bool RHDHV_KNIPP1;              /* knipperend signaal 1 Hz.                     */
bool RHDHV_KNIPP2;              /* knipperend signaal 2 Hz.                     */
bool RHDHV_KNIPP5;              /* knipperend signaal 5 Hz.                     */
 int RHDHV_TSEC;                /* tiende seconde                               */

/* BR[]                                                                         */
#define RHDHV_BR_HD         BIT7  /*  bijzondere realisatie voor hulpdiensten   */

/* AA[]                                                                         */
#define RHDHV_AA_HD         BIT7  /*  bijzondere realisatie voor hulpdiensten   */

/* A[]                                                                          */

/* B[]                                                                          */

/* RR[]                                                                         */
#define RHDHV_RR_GELIJK     BIT8  /* gelijkstart                                */
#define RHDHV_RR_PL         BIT11 /* RR tbv. (half)star bedrijf                 */
#define RHDHV_RR_HD         BIT13 /* RR tbv. hulpdienstingreep                  */

/* Z[]                                                                          */
#define RHDHV_Z_PL          BIT9  /* tbv. (half)star bedrijf (gedeelde bit)     */
#define RHDHV_Z_HD          BIT10 /* Z tbv. hulpdienstingreep                   */
#define RHDHV_Z_GELIJK      BIT11 /* tbv gelijkstart combirichtingen            */
#define RHDHV_Z_FILE        BIT12 /* tbv file ingrepen                          */

/* BL */
#define RHDHV_BL_FILE       BIT9  /* tbv. fileingreep                           */
#define RHDHV_BL_BRAND      BIT10 /* tbv. hulpdienstingreep                     */

/* MK[]                                                                         */

/* YV[]                                                                         */
#define RHDHV_YV_PL         BIT9  /* YV tbv. (half)star bedrijf                 */
#define RHDHV_YV_BRAND      BIT15 /* YV tbv. hulpdienstingreep                  */

/* YM[]                                                                         */
#define RHDHV_YM_HD         BIT10 /* tbv hulpdienstingreep                      */
#define RHDHV_YM_COMBI      BIT11 /* tbv gecombineerde rijstroken bus/auto      */
#define RHDHV_YM_VAST       BIT12 /* vast meeverlengen met andere richting      */
#define RHDHV_YM_BRAND      BIT15 /* YV tbv. hulpdienstingreep                  */

/* X[]                                                                          */
#define X_GELIJK_HALFSTAR   BIT8  /* gelijkstart                                */
#define X_DEELC_HALFSTAR    BIT9  /* deelconflict                               */
#define X_VOOR_HALFSTAR     BIT10 /* voorstart                                  */
#define RHDHV_X_PL          BIT11 /* tbv. (half)star bedrijf                    */
#define RHDHV_X_AANV        BIT12 /* tbv. x_aanvoer                             */
#define RHDHV_X_BRAND       BIT15 /* tbv. brandweeringreep                      */

/* YS[]                                                                         */

/* RS[]                                                                         */

/* RW[]                                                                         */
#define RHDHV_RW_VS         BIT10 /* tbv voorstartrichtingen deelconflicten     */

/* YW[]                                                                         */


/* FM[]                                                                         */
#define RHDHV_FM_PL         BIT8   /* FM tbv. (half)star bedrijf                */
#define OV_PLE_BIT          BIT14  /* FM tbv OV ingrepen tijdens PL             */

/* RTFB                                                                         */
#define RHDHV_RTFB_HD       BIT2   /* RTFB tbv hulpdienstingrepen tijdens PL    */

/* HTX */

int  rhdhv_trood[FCMAX];           /* roodtijd                                  */
int  rhdhv_twacht[FCMAX];          /* wachttijd                                 */
int  rhdhv_tgroen[FCMAX];          /* groentijd                                 */
int  rhdhv_tgroen_ws[FCMAX];       /* groentijd exclusief wachtstand            */
int  rhdhv_tgeel[FCMAX];           /* geeltijd                                  */

/* tx-momenten wissel */
#define xa  0
#define xb  1
#define xc  2
#define xd  3
#define xe  4

/* GLOBALE VARIALEN */
/* ================ */
extern bool RHDHV_SSEC;            /* puls op start seconde                     */
extern bool RHDHV_SMIN;            /* puls op start minuut                      */
extern bool RHDHV_SUUR;            /* puls op start uur                         */
extern bool RHDHV_KNIPP1;          /* knipperend signaal 1 Hz.                  */
extern bool RHDHV_KNIPP2;          /* knipperend signaal 2 Hz.                  */
extern bool RHDHV_KNIPP5;          /* knipperend signaal 5 Hz.                  */
extern mulv RHDHV_APL_wens;        /* signaalplanwens PL/VA                     */
extern char RHDHV_TIJD[];          /* timestamp                                 */
extern bool RHDHV_PL_COPY_TO_TIG;  /* nieuwe TX-tijden copieren naar TIG tabel  */

/* OV punctualiteit */
/* per richting worden 3 punctualiteitsniveau's bijgehouden              */
#define OVTEVROEG       0
#define OVOPTIJD        1
#define OVTELAAT        2
#define PUNCTMAX        3
int RHDHV_PUNCTOV[FCMAX][PUNCTMAX];        /* weergave punctualiteit     */
iAantalPrioriteitsInmeldingen_old[FCMAX];  /* tbv weergave punctualiteit */

void rhdhv_hd_real(count fc, bool condition);
void wijzig_prm(count p, mulv newvalue);
void wijzig_timer(count t, mulv newvalue);
void wijzig_hiaattijd(count d, mulv newvalue);
void set_prmbit(count p, mulv bitje);
void reset_sch(count s);
void verhoog_hiaattijd(count d, mulv verhoging);
void verhoog_timer(count t, mulv verhoging);
#endif
