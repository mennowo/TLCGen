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

#define RHDHV_IRT(t)    ((bool)(T[t] || RT[t] || IT[t]))
#define RHDHV_BIT(v,t,m)  ( v = ((t) ? ((v) |= (m)) : ((v) &= ~(m))) )

bool RHDHV_COPY_2_TIG = FALSE;  /* nieuwe TX-tijden copieren naar TIG tabel             */

/* tbv harde koppelingen */
count geeltimer[FCMAX][FCMAX];
count groodtimer[FCMAX][FCMAX];

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
#define RHDHV_A_MR          BIT8  /* meerealisatie                              */
#define RHDHV_A_WS          BIT9  /* wachtstand                                 */

/* B[]                                                                          */

/* RR[]                                                                         */
#define RHDHV_RR_GELIJK     BIT8  /* gelijkstart                                */
#define RHDHV_RR_ALTCOR     BIT9  /* alternatieve correctie                     */
#define RHDHV_RR_VS         BIT10 /* voorstart tegenhouden                      */
#define RHDHV_RR_PL         BIT11 /* RR tbv. (half)star bedrijf                 */
#define RHDHV_RR_KOP        BIT12 /* RR tbv. harde koppeling                    */
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
#define RHDHV_YV_KOP        BIT8  /* YV tbv. VA koppeling                       */
#define RHDHV_YV_PL         BIT9  /* YV tbv. (half)star bedrijf                 */
#define RHDHV_YV_BRAND      BIT15 /* YV tbv. hulpdienstingreep                  */

/* YM[]                                                                         */
#define RHDHV_YM_PL         BIT8  /* meeverlengen tijdens halfstar              */
#define RHDHV_YM_KOP        BIT9  /* tbv koppeling                              */
#define RHDHV_YM_HD         BIT10 /* tbv hulpdienstingreep                      */
#define RHDHV_YM_COMBI      BIT11 /* tbv gecombineerde rijstroken bus/auto      */
#define RHDHV_YM_VAST       BIT12 /* vast meeverlengen met andere richting      */
#define RHDHV_YM_BRAND      BIT15 /* YV tbv. hulpdienstingreep                  */

/* X[]                                                                          */
#define RHDHV_X_GELIJK      BIT8  /* gelijkstart                                */
#define RHDHV_X_DEELC       BIT9  /* deelconflict                               */
#define RHDHV_X_VOOR        BIT10 /* voorstart                                  */
#define RHDHV_X_PL          BIT11 /* tbv. (half)star bedrijf                    */
#define RHDHV_X_AANV        BIT12 /* tbv. x_aanvoer                             */
#define RHDHV_X_BRAND       BIT15 /* tbv. brandweeringreep                      */

/* YS[]                                                                         */

/* RS[]                                                                         */
#define RHDHV_RS_PLE        BIT4  /* voorstartgroen                             */

/* RW[]                                                                         */
#define RHDHV_RW_KOP        BIT8  /* koppeling                                  */
#define RHDHV_RW_VAR_TXC    BIT9  /* RW voor gegarandeerd groen na TA           */
#define RHDHV_RW_VS         BIT10 /* tbv voorstartrichtingen deelconflicten     */
#define RHDHV_RW_VGNAMG     BIT11 /* RW tbv. terug naar verlengen tijdens MG    */ 
#define RHDHV_RW_WG         BIT12 /* RW wachtgroen tijdens halfstar             */ 

/* YW[]                                                                         */
#define RHDHV_YW_VAR_TXC    BIT8   /* YW voor gegarandeerd groen na SAS         */
#define RHDHV_YW_PL         BIT9   /* YW tbv. (half)star bedrijf                */
#define RHDHV_YW_WS         BIT10  /* tbv wachtstand tijdens PL                 */

/* FM[]                                                                         */
#define RHDHV_FM_PL         BIT8   /* FM tbv. (half)star bedrijf                */
#define OV_PLE_BIT          BIT14  /* FM tbv OV ingrepen tijdens PL             */

/* RTFB                                                                         */
#define RHDHV_RTFB_PLVA     BIT1   /* RTFB tbv omschakelen VA <-> PL            */
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


void rhdhv_reset_MLE(void);
void reset_fc_halfstar(void);
bool rhdhv_pl_gebied(mulv tx,
                   mulv s, 
                   mulv e);
void rhdhv_gelijkstart_va_arg(count h_x, count h_rr, bool overslag, ...);
void rhdhv_primcor_2kopp(count fc1, count fc2, bool voorwaarde, count h_mak1, count h_amk2);
void rhdhv_deelconflict_auto_1r(count fc1, count fc2, count t_vs, count t_vso);
bool rhdhv_yws_groen_fk(count i);
void rhdhv_MR(count i, count j, bool condition);
void rhdhv_reset_altreal(void);
void rhdhv_timing(void);
void rhdhv_signaltimers(void);
void rhdhv_fictief_ort(count fc1, count fc2, count t_ort);
void rhdhv_hd_real(count fc, bool condition);
void rhdhv_z_rr_fk(int fc, bool bitz, bool bitrr);
void rhdhv_wijzig_prm(count p, mulv newvalue);
void rhdhv_wijzig_timer(count t, mulv newvalue);
void rhdhv_wijzig_hiaattijd(count d, mulv newvalue);
void rhdhv_set_prmbit(count p, mulv bitje);
void rhdhv_reset_sch(count s);
void rhdhv_verhoog_hiaattijd(count d, mulv verhoging);
void rhdhv_verhoog_timer(count t, mulv verhoging);
void rhdhv_toggle_conflict(count fc1, count fc2, count to1_2, count to2_1, bool period);
void rhdhv_toggle_fictive_conflict(count fc1, count fc2, bool period);
void rhdhv_combi_rijstrook(count fcbus, count fcauto, count h_gsx, count hbuskoplus, count dkop, 
                           count tbezetkl, count schbezetkl, count cvc, mulv garantiegroen);
void rhdhv_inlopen(bool period, count fc1, count fc2, count tinl);
void rhdhv_timestamp(void);
void rhdhv_NaloopDet(count fc1, count fc2, bool condition, count tnl);
void rhdhv_harde_koppeling_va(count fcaan, count fcnal, count tnleg, count tnld, bool dcondition, mulv mtgln, bool hkvastnl, bool hkcondition);
void rhdhv_getrapte_fietser(count fc1, count fc2, count tnlevgfc1fc2, count tnlevgfc2fc1, count hmakfc1, count hmakfc2);
void rhdhv_inkop(count fc, bool det, count t_hiaat, count h_kop, count prm_lpel, count prm_hpel, count prm_priol, count prm_prioh, 
                 count prm_priokop, count c_pel, count t_nal, bool condition);
void rhdhv_meereal_hk(count fcaan, count fcnal, count tnal, bool hkcondition);
bool rhdhv_set_MRLW (count i, count j, bool period);
#endif
