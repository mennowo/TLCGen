/* FILE: SYNCFUNC.C */
/* ---------------- */

#ifndef AUTOMAAT
  #if defined (_DEBUG)
    #include "xyprintf.h"   /* Printen debuginfo */
  #endif
#endif
#include <string.h>  /* declaration strcpy(), strcat()      */
#include <stdio.h>   /* declaration sprintf()               */

#include "fcvar.h"   /* fasecycli                           */
#include "kfvar.h"   /* conflicten                          */
#include "usvar.h"   /* uitgangs elementen                  */
#include "dpvar.h"   /* detectie elementen                  */
#include "isvar.h"   /* ingangs elementen                   */
#include "hevar.h"   /* hulp elementen                      */
#include "mevar.h"   /* geheugen elementen                  */
#include "tmvar.h"   /* tijd elementen                      */
#include "ctvar.h"   /* teller elementen                    */
#include "schvar.h"  /* software schakelaars                */
#include "prmvar.h"  /* parameters                          */
#include "lwmlvar.h" /* langstwachtende modulen structuur   */
#include "control.h" /* controller interface                */
#include "bericht.h" /* declaration mtoc(), btohc()         */
#include "syncvar.h" /* synchronisatie variabelen           */
#include "sysdef.c"

bool kcv_primair(count i); /* prototype */
bool kaa(count i);         /* prototype */
bool kcv(count i);         /* prototype */

#define max(a,b) (((a) > (b)) ? (a) : (b))      /* vergelijken van twee waarden -> return maximum     */

/* toedeling van de bits in KR[]
 * BIT0 - conflicterende realisatie bezig
 * BIT1 - voorstart
 * BIT2 - gelijkstart
 * BIT3 - fiets/voetganger
 * BIT4 - fictief ontruimen
 * BIT5 - reserve
 * BIT6 - eigen realisatie bezig (geel en garantierood)
 */

void init_realisation_timers(void)
{
    register count fc, i;

    for (fc = 0; fc < FC_MAX; ++fc)
    {
        R_timer[fc] = M_R_timer + (fc*FC_MAX); /* memory allocation    */
    }

    SYNCDUMP = 0;
    for (fc = 0; fc < FC_MAX; ++fc)
    {
        for (i = 0; i < FC_MAX; ++i)
        {
            R_timer[fc][i] = NG;
        }
        KR[fc] = FALSE;
    }
}

void control_realisation_timers(void)
{
    register count fc, i, k;

    for (fc = 0; fc < FC_MAX; ++fc)
    {
        /* set initial value of R_timer[][] to NG */
        for (i = 0; i < FC_MAX; ++i)
        {
            R_timer[fc][i] = NG;
        }
    }
    for (fc = 0; fc < FC_MAX; ++fc)
    {
        for (i = 0; i < KFC_MAX[fc]; ++i)
        {
#ifdef CCOLTIG
            k = KF_pointer[fc][i];
#else
            k = TO_pointer[fc][i];
#endif
#if defined CCOLTIG && !defined NO_TIGMAX
            if (CV[fc])
            {
                R_timer[fc][k] = TFG_max[fc] - TFG_timer[fc] + TVG_max[fc] - TVG_timer[fc] + TGL_max[fc] + TIG_max[fc][k];
            }
            else if (TIG[fc][k])
            {
                R_timer[fc][k] = TGL_max[fc] - TGL_timer[fc] + TIG_max[fc][k] - TIG_timer[fc];
            }
#else
            if (CV[fc])
            {
                R_timer[fc][k] = TFG_max[fc] - TFG_timer[fc] + TVG_max[fc] - TVG_timer[fc] + TGL_max[fc] + TO_max[fc][k];
            }
            else if (TO[fc][k])
            {
                R_timer[fc][k] = TGL_max[fc] - TGL_timer[fc] + TO_max[fc][k] - TO_timer[fc];
            }
#endif
            else
            {
                R_timer[fc][k] = 0;
            }

            KR[k] |= (R_timer[fc][k] > 0) ? BIT0 : 0;
        }
        if (GL[fc] || TRG[fc])
        {
            R_timer[fc][fc] = TGL_max[fc] - TGL_timer[fc] + TRG_max[fc] - TRG_timer[fc];
        }
        else
        {
            R_timer[fc][fc] = 0;
        }

        KR[fc] |= (R_timer[fc][fc] > 0) ? BIT6 : 0;
    }
}

void correction_realisation_timers(count fcv, count fcn, count tcorrection, bool bit)
{
    if (tcorrection > R_timer[fcv][fcn])
    {
        R_timer[fcv][fcn] = tcorrection;
        KR[fcn] |= (R_timer[fcv][fcn] > 0) ? bit : 0;
    }
}

void print_realisation_timers(void)
{
#ifndef AUTOMAAT
#if defined (_DEBUG)
    //   register count fc, i;
    //
    //   xyprintf(0, 1, "KR ");
    //   for (fc=0; fc<FC_MAX; fc++) {
    //      xyprintf(6+fc*5,    0, "%s", FC_code[fc]);   /* kolomkop     */
    //      xyprintf(3+fc*5,    1, "|%4X", KR[fc]);      /* rij met KR[] */
    //      xyprintf(     0, fc+2, "%s", FC_code[fc]);   /* rijkop       */
    //      for (i=0; i<FC_MAX; i++) {
    //         if (R_timer[fc][i]!=NG)
    //            xyprintf(3+5*i, fc+2, "|%04d", R_timer[fc][i]);
    //      }
    //   }

#endif
#endif
}

void dump_realisation_timers(void)
{
    char szDump[1024];
    char szTmp[32];
    int len;
    bool bbreak, bstop;
    register count fc, i;
    bstop = FALSE;

    switch (SYNCDUMP)
    {
    case 1: /* header */
        if (!waitterm((mulv)50))
        {
            uber_puts("\n");
            uber_puts("Realisationtimers\n");
            uber_puts("-----------------\n");
            SYNCDUMP++;
        }
        break;
    case 2: /* columntitle */
        sprintf(szDump, "   ");
        for (fc = 0; fc < FC_MAX; ++fc)
        {
            sprintf(szTmp, "%5s", FC_code[fc]);
            strcat(szDump, szTmp);
        }
        len = strlen(szDump) + 10;
        if (!waitterm((mulv)len))
        {
            uber_puts(szDump);
            uber_puts("\n");
            SYNCDUMP++;
        }
        break;
    case 3: /* row with KR[] */
        sprintf(szDump, "KR ");
        for (fc = 0; fc < FC_MAX; ++fc)
        {
            sprintf(szTmp, "|%4X", KR[fc]);
            strcat(szDump, szTmp);
        }
        len = strlen(szDump) + 10;
        if (!waitterm((mulv)len)) {
            uber_puts(szDump);
            uber_puts("\n");
            SYNCDUMP++;
        }
        break;
    default:
        for (fc = SYNCDUMP - 4; fc < FC_MAX; ++fc)
        {
            sprintf(szDump, "%s ", FC_code[fc]);
            for (i = 0; i < FC_MAX; ++i)
            {
                if (R_timer[fc][i] != NG)
                {
                    sprintf(szTmp, "|%04d", R_timer[fc][i]);
                }
                else
                {
                    sprintf(szTmp, "|    ");
                }
                strcat(szDump, szTmp);
            }
            len = strlen(szDump) + 10;
            if (!waitterm((mulv)len))
            {
                bbreak = FALSE;
                uber_puts(szDump);
                uber_puts("\n");
                SYNCDUMP++;
                if (SYNCDUMP == FC_MAX + 4)
                {
                    bstop = TRUE;
                }
            }
            else
            {
                bbreak = TRUE;
            }
            if (bbreak == TRUE)
            {
                break;
            }
        }
        if (bstop == TRUE)
        {
            SYNCDUMP = 0;
            break;
        }
        if (bbreak == TRUE)
        {
            break;
        }
    }
}

void FictiefOntruimen(bool period, count fcv, count fcn, count tftofcvfcn, bool bit)
{
    if (period)
    {
        /* Fictieve ontruimingstijd fcv naar fcn */
        RT[tftofcvfcn] = SR[fcv];

        /* Tegenhouden groenfase met RR totdat fictieve ontruimingstijd gaat lopen */
        RR[fcn] |= R[fcn] && (G[fcv] || ERA[fcn]) ? bit : 0;
        RW[fcv] &= (G[fcn] || !A[fcn]) ? ~0 : ~BIT4;
        YM[fcv] &= (G[fcn] || !A[fcn] || kcv_primair(fcn)) ? ~0 : ~BIT4;

#ifndef AUTOMAAT
#if defined (_DEBUG)
        if (SG[fcn] && (!R[fcv] || T[tftofcvfcn] && (T_max[tftofcvfcn] > 0)) && !SG[fcv])
        {
            xyprintf(1, /*2* */FC_MAX + fcn, "Fictief Ontruimen: Startgroen fc%s conflicteert met fc%s.", FC_code[fcn], FC_code[fcv]);
            /*         FB=TRUE; */
        }
#endif
#endif
    }
}

void FictiefOntruimen_correctionKR(bool period, count fcv, count fcn, count tftofcvfcn)
{
    if (period)
    {
        if (G[fcv])
        {
            correction_realisation_timers(fcv, fcn, TGL_max[fcv] + T_max[tftofcvfcn], BIT4);
        }
        else if (GL[fcv] || SR[fcv])
        {
            correction_realisation_timers(fcv, fcn, TGL_max[fcv] - TGL_timer[fcv] + T_max[tftofcvfcn], BIT4);
        }
        else
        {
            correction_realisation_timers(fcv, fcn, RT[tftofcvfcn] || T[tftofcvfcn] ? (T_max[tftofcvfcn] - T_timer[tftofcvfcn]) : 0, BIT4);
        }
    }
}

void VoorStarten_correctionKR(bool period, count fcvs, count fcls, count tvs)
{
    register count fc;

    if (period)
    {
        /* copy maximum realisation timers for phasecycles with late release */
        for (fc = 0; fc < FC_MAX; ++fc)
        {
            if (R_timer[fc][fcvs] != NG)
                R_timer[fc][fcls] = max(R_timer[fc][fcls], R_timer[fc][fcvs] + (T[tvs] ? T_max[tvs] - T_timer[tvs] : T_max[tvs]));
        }

        if ((A[fcvs] && !G[fcvs] || GL[fcvs] || TRG[fcvs]) && !BL[fcvs])
        {
            correction_realisation_timers(fcvs, fcls, GL[fcvs] || TRG[fcvs] ? TGL_max[fcvs] - TGL_timer[fcvs] + TRG_max[fcvs] - TRG_timer[fcvs] + T_max[tvs] :
                R[fcvs] ? R_timer[fcvs][fcls] + (T_max[tvs] > 0 ? T_max[tvs] : 1) :
                T[tvs] ? (T_max[tvs] - T_timer[tvs]) :
                R_timer[fcvs][fcls], BIT1);
        }

        KR[fcvs] |= GL[fcls] ? BIT1 : 0;
        KR[fcls] |= GL[fcvs] || TRG[fcvs] || T[tvs] ? BIT1 : 0;
    }
}

void GelijkStarten_correctionKR(bool period, count fc1, count fc2)
{
    register count fc;

    if (period && (A[fc1] && !G[fc1] || GL[fc1] || TRG[fc1]) && !BL[fc1] && (A[fc2] && !G[fc2] || GL[fc2] || TRG[fc2]) && !BL[fc2])
    {
        /* copy maximum realisation timers for both phasecycles */
        for (fc = 0; fc < FC_MAX; ++fc)
        {
            R_timer[fc][fc1] = R_timer[fc][fc2] = max(R_timer[fc][fc1], R_timer[fc][fc2]);
        }

        if (G[fc1] && R[fc2])
        {
            R_timer[fc1][fc2] = max(R_timer[fc1][fc2], TGL_max[fc1] + TRG_max[fc1]);
        }
        if (G[fc2] && R[fc1])
        {
            R_timer[fc2][fc1] = max(R_timer[fc2][fc1], TGL_max[fc2] + TRG_max[fc2]);
        }

        KR[fc1] |= KR[fc2] || GL[fc2] ? BIT2 : 0;
        KR[fc2] |= KR[fc1] || GL[fc1] ? BIT2 : 0;
    }
}

void FietsVoetganger_correctionKR(bool period, count fcfts, count fcvtg) 
{
    register count fc;

    if (period && A[fcfts] && !G[fcfts] && A[fcvtg])
    {
        /* copy maximum realisation timers for pedestrians */
        for (fc = 0; fc < FC_MAX; ++fc)
        {
            if (R_timer[fc][fcfts] != NG)
            {
                R_timer[fc][fcvtg] = max(R_timer[fc][fcfts], R_timer[fc][fcvtg]);
            }
        }
        KR[fcvtg] |= KR[fcfts] ? BIT3 : 0;
    }
}

void VoorStarten(bool period, count fcvs, count fcls, count tvs, bool bit)
{
    bool xpg;

    if (period)
    {
        /* Voorstarttijd */
        RT[tvs] = RA[fcvs];
        AT[tvs] = RV[fcvs];

        /* Vasthouden/terugzetten groenfase */
        YM[fcvs] |= G[fcvs] && (RA[fcls] || AAPR[fcls]) && !(KR[fcvs] & BIT1) && !kaa(fcvs) ? bit : 0;
        RW[fcvs] |= G[fcvs] && G[fcls] && (CG[fcls] < CG_WG) && !kaa(fcvs) ? bit : 0;

        RR[fcls] |= R[fcls] && ((kcv_primair(fcvs) || RR[fcvs] && !G[fcvs]) && A[fcvs] || ERA[fcls]) ? bit : 0;

        xpg = G[fcvs] && !VS[fcvs] && !FG[fcvs] && !G[fcls] && !PG[fcls] || !G[fcvs] && !G[fcls] && !PG[fcls] && A[fcls] && A_old[fcls] && A_old_old[fcls];

        /* Zet de PRIMAIR_OVERSLAG in PG voor gebruikte fasen */
        /* -------------------------------------------------- */
        PG[fcvs] |= !PG[fcvs] && !xpg && (!A[fcvs] || !A_old[fcvs] || !A_old_old[fcvs] || SGL[fcvs]) && PG[fcls] && G[fcls] ? PRIMAIR_OVERSLAG : 0;

#ifndef AUTOMAAT
#if defined (_DEBUG)
        if (SG[fcvs] && !R[fcls] && (T[tvs] && (T_max[tvs] > 0) || !SG[fcls] && (T_max[tvs] == 0))) {
            xyprintf(1, /*FC_MAX+ */fcvs, "Voorstart: Startgroen fc%s conflicteert met fc%s.", FC_code[fcvs], FC_code[fcls]);
            /*         FB=TRUE; */
        }
#endif
#endif
    }
}

void GelijkStarten(bool period, count fc1, count fc2, bool bit, bool overslag_sg)
{
    bool xpg1, xpg2;

    if (period && A[fc1] && A[fc2])
    {
        RR[fc1] |= R[fc1] && ((TRG[fc2] || GL[fc2] || kcv_primair(fc2) || RR[fc2] && !G[fc2]) || ERA[fc1]) ? bit : 0;
        RR[fc2] |= R[fc2] && ((TRG[fc1] || GL[fc1] || kcv_primair(fc1) || RR[fc1] && !G[fc1]) || ERA[fc2]) ? bit : 0;

        X[fc1] |= RV[fc1] || RV[fc2] ? BIT2 : 0;
        X[fc2] |= RV[fc1] || RV[fc2] ? BIT2 : 0;
    }
    if (period) 
    {
        xpg1 = G[fc1] && !VS[fc1] && !FG[fc1] && !G[fc2] && !PG[fc2] || !G[fc1] && !G[fc2] && !PG[fc2] && A[fc2] && A_old[fc2] && A_old_old[fc2];
        xpg2 = G[fc2] && !VS[fc2] && !FG[fc2] && !G[fc1] && !PG[fc1] || !G[fc2] && !G[fc1] && !PG[fc1] && A[fc1] && A_old[fc1] && A_old_old[fc1];

        /* Zet de PRIMAIR_OVERSLAG in PG voor gebruikte fasen */
        /* -------------------------------------------------- */
        if (overslag_sg) 
        {
            PG[fc1] |= !PG[fc1] && !xpg1 && (!A[fc1] || !A_old[fc1] || !A_old_old[fc1] || SGL[fc1]) && PG[fc2] ? PRIMAIR_OVERSLAG : 0;
            PG[fc2] |= !PG[fc2] && !xpg2 && (!A[fc2] || !A_old[fc2] || !A_old_old[fc2] || SGL[fc2]) && PG[fc1] ? PRIMAIR_OVERSLAG : 0;
        }
        else
        {
            PG[fc1] |= !PG[fc1] && !xpg1 && (!A[fc1] || !A_old[fc1] || !A_old_old[fc1]) && !kcv(fc1) && SGL[fc2] && PG[fc2] ? PRIMAIR_OVERSLAG : 0;
            PG[fc2] |= !PG[fc2] && !xpg2 && (!A[fc2] || !A_old[fc2] || !A_old_old[fc2]) && !kcv(fc2) && SGL[fc1] && PG[fc1] ? PRIMAIR_OVERSLAG : 0;
        }
    }
}

void FietsVoetganger(bool period, count fcfts, count fcvtg, bool bit)
{
    if (period && A[fcfts] && A[fcvtg])
    {
        RR[fcvtg] |= R[fcvtg] && (kcv_primair(fcfts) || ERA[fcvtg]) ? bit : 0;
    }
    if (period)
    {
        RR[fcvtg] |= R[fcvtg] && (TRG[fcfts] || GL[fcfts] || ERA[fcvtg]) ? bit : 0;

        /* Zet de PRIMAIR_OVERSLAG in PG voor gebruikte fasen */
        /* -------------------------------------------------- */
        PG[fcvtg] |= !PG[fcvtg] && (!A[fcvtg] || !A_old[fcvtg] || !A_old_old[fcvtg] || SGL[fcvtg]) && PG[fcfts] && (!G[fcfts] || kcv(fcfts)) ? PRIMAIR_OVERSLAG : 0;
    }
}

void realisation_timers(bool bit)
{
    register count fc;
    for (fc = 0; fc < FC_MAX; ++fc)
    {
        X[fc] &= ~bit;
        X[fc] |= R[fc] && KR[fc] ? bit : 0;
        A_old_old[fc] = A_old[fc];
        A_old[fc] = A[fc];
    }
}

