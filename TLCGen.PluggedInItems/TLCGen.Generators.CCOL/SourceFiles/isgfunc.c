#include "isgfunc.h"

mulv TNL_type[FCMAX][FCMAX]; /* type naloop */
mulv FK_type[FCMAX][FCMAX]; /* type fictief conflict */

mulv TISG_PR[FCMAX][FCMAX];
mulv TVG_basis[FCMAX];
mulv TVG_AR[FCMAX];
mulv TISG_AR[FCMAX][FCMAX];
mulv TVG_PR[FCMAX];
mulv TVG_old[FCMAX];
mulv TVG_AR_old[FCMAX];
mulv REALISATIETIJD_max[FCMAX];
mulv TIGR[FCMAX][FCMAX];
mulv PRIOFC[FCMAX];

boolv NietGroentijdOphogen[FCMAX];
mulv twacht[FCMAX];
mulv twacht_afkap[FCMAX];
count REALISATIETIJD[FCMAX][FCMAX];

boolv Volgrichting[FCMAX];
boolv AfslaandDeelconflict[FCMAX] = { 0 };


void BepaalIntergroenTijden(void)
{
   count fc1, fc2;
   for (fc1 = 0; fc1 < FCMAX; ++fc1)
   {
      for (fc2 = 0; fc2 < FCMAX; ++fc2)
      {
         TIGR[fc1][fc2] = TIG_max[fc1][fc2];
      }
   }
}

void corrigeerTIGRvoorNalopen(count fc1, count fc2, mulv tnleg, mulv tnlegd, mulv tvgnaloop)
{
   /* Uitgangspunt is dat tnleg de maatgevende nalooptijd is */
   int fc3, n;
   for (n = 0; n < KFC_MAX[fc2]; ++n)
   {
      fc3 = KF_pointer[fc2][n];
      if (tnleg != NG) TIGR[fc1][fc3] = max(TIGR[fc1][fc3], T_max[tnleg] + T_max[tvgnaloop] + TIGR[fc2][fc3]);
      if (tnlegd != NG) TIGR[fc1][fc3] = max(TIGR[fc1][fc3], T_max[tnlegd] + TGL_max[fc1] + T_max[tvgnaloop] + TIGR[fc2][fc3]);
   }
}

void InitRealisatieTijden(void)
{
   count fc1, fc2;

   for (fc1 = 0; fc1 < FC_MAX; fc1++)
   {
      for (fc2 = 0; fc2 < FC_MAX; fc2++)
      {
         REALISATIETIJD[fc1][fc2] = NG;
      }
   }
}

void RealisatieTijden_VulHardeConflictenIn(void)
{
   count fc1, fc2, n;

   for (fc1 = 0; fc1 < FCMAX; ++fc1)
   {
      for (n = 0; n < KFC_MAX[fc1]; ++n)
      {
         fc2 = KF_pointer[fc1][n];
         if (TIG[fc1][fc2])
         {
            if (PR[fc1] || BR[fc1] /* @@ !AR kan in principe weg @@ */ && !AR[fc1])
            {
               REALISATIETIJD[fc1][fc2] =
                  (VS[fc1]) ? TFG_max[fc1] + TVG_max[fc1] + TIG_max[fc1][fc2] :
                  (FG[fc1]) ? TFG_max[fc1] - TFG_timer[fc1] + TVG_max[fc1] + TIG_max[fc1][fc2] :
                  (WG[fc1]) ? TVG_max[fc1] + TIG_max[fc1][fc2] :
                  (VG[fc1] && (TVG_max[fc1] > TVG_timer[fc1])) ? TVG_max[fc1] - TVG_timer[fc1] + TIG_max[fc1][fc2] :
                  (VG[fc1] && (TVG_max[fc1] <= TVG_timer[fc1])) ? TIG_max[fc1][fc2] :
                  (MG[fc1]) ? TIG_max[fc1][fc2] : TIG_max[fc1][fc2] - TIG_timer[fc1];
            }
            else /* AR */
            {
               REALISATIETIJD[fc1][fc2] =
                  (VS[fc1]) ? TFG_max[fc1] + TVG_AR[fc1] + TIG_max[fc1][fc2] :
                  (FG[fc1]) ? TFG_max[fc1] - TFG_timer[fc1] + TVG_AR[fc1] + TIG_max[fc1][fc2] :
                  (WG[fc1]) ? TVG_AR[fc1] + TIG_max[fc1][fc2] :
                  (VG[fc1] && (TVG_AR[fc1] > TVG_timer[fc1])) ? TVG_AR[fc1] - TVG_timer[fc1] + TIG_max[fc1][fc2] :
                  (VG[fc1] && (TVG_AR[fc1] <= TVG_timer[fc1])) ? TIG_max[fc1][fc2] :
                  (MG[fc1]) ? (TIG_max[fc1][fc2]) : TIG_max[fc1][fc2] - TIG_timer[fc1];
            }
         }
         else
         {
            REALISATIETIJD[fc1][fc2] = 0;
         }
      }
   }
}

void RealisatieTijden_VulGroenGroenConflictenIn(void)
{
   count fc1, fc2, n;

   for (fc1 = 0; fc1 < FCMAX; ++fc1)
   {
      for (n = GKFC_MAX[fc1]; n < FKFC_MAX[fc1]; ++n)
      {
         fc2 = KF_pointer[fc1][n];
         if (TIG[fc1][fc2])
         {
            if (PR[fc1] || BR[fc1] /* @@ !AR kan in principe weg @@ */ && !AR[fc1])
            {
               REALISATIETIJD[fc1][fc2] =
                  (VS[fc1]) ? TFG_max[fc1] + TVG_max[fc1] :
                  (FG[fc1]) ? TFG_max[fc1] - TFG_timer[fc1] + TVG_max[fc1] :
                  (WG[fc1]) ? TVG_max[fc1] :
                  (VG[fc1] && (TVG_max[fc1] > TVG_timer[fc1])) ? TVG_max[fc1] - TVG_timer[fc1] :
                  (MG[fc1] || VG[fc1] && (TVG_max[fc1] <= TVG_timer[fc1])) ? 1 : 0;
            }
            else /* AR[] */
            {
               REALISATIETIJD[fc1][fc2] =
                  (VS[fc1]) ? TFG_max[fc1] + TVG_AR[fc1] :
                  (FG[fc1]) ? TFG_max[fc1] - TFG_timer[fc1] + TVG_AR[fc1] :
                  (WG[fc1]) ? TVG_AR[fc1] :
                  (VG[fc1] && (TVG_AR[fc1] > TVG_timer[fc1])) ? TVG_AR[fc1] - TVG_timer[fc1] :
                  (MG[fc1] || VG[fc1] && (TVG_AR[fc1] <= TVG_timer[fc1])) ? 1 : 0;
            }
         }
         else
         {
            REALISATIETIJD[fc1][fc2] = 0;
         }

      }
   }
}

void CorrigeerRealisatieTijdenObvGarantieTijden(void)
{
   count fc1;

   for (fc1 = 0; fc1 < FCMAX; ++fc1)
   {
      REALISATIETIJD[fc1][fc1] = GL[fc1] ? (TGL_max[fc1] - TGL_timer[fc1] + TRG_max[fc1]) : TRG[fc1] ? TRG_max[fc1] - TRG_timer[fc1] : 0;
      if ((REALISATIETIJD[fc1][fc1] <= 0) && RV[fc1]) REALISATIETIJD[fc1][fc1] = 1;
   }
}

void Realisatietijd_NLEG(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!AR[i])
        {
            if (CV[i] && !RA[i])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfgd] - (FG[i] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnleg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_max[i] - TVG_timer[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlegd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_max[i] - TVG_timer[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
            }
            else if (TIG[j][k])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfgd] - T_timer[tnlfgd] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnleg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnleg] - T_timer[tnleg] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnlegd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TGL_max[i] - TGL_timer[i] + T_max[tnlegd] - ((G[i]||GL[i])? 0 : T_timer[tnlegd]) + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
            }
        }
        else
        {
            if (CV[i] && !RA[i] && !(TVG_timer[i] > TVG_AR[i]))
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
                    if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfgd] - (FG[i] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop] + TIG_max[j][k]);
                    if (!(tnleg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_AR[i] - TVG_timer[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlegd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_AR[i] - TVG_timer[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
            }
            else if (TIG[j][k])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfgd] - T_timer[tnlfgd] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnleg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnleg] - T_timer[tnleg] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnlegd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TGL_max[i] - TGL_timer[i] + T_max[tnlegd] - ((G[i]||GL[i])? 0 : T_timer[tnlegd]) + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
            }
        }
    }
    for (n = KFC_MAX[j]; n < GKFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!AR[i])
        {
            if (CV[i] && !RA[i])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfgd] - (FG[i] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop]);
                if (!(tnleg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_max[i] - TVG_timer[i] + T_max[tnleg] + T_max[tvgnaloop]);
                if (!(tnlegd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_max[i] - TVG_timer[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop]);
            }
            else if (G[j])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfgd] - T_timer[tnlfgd] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnleg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnleg] - T_timer[tnleg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlegd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TGL_max[i] - TGL_timer[i] + T_max[tnlegd] - (GL[i] ? 0 : T_timer[tnlegd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);
            }
        }
        else
        {
            if (CV[i] && !RA[i] && !(TVG_timer[i] > TVG_AR[i]))
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfgd] - (FG[i] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop]);
                if (!(tnleg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_AR[i] - TVG_timer[i] + T_max[tnleg] + T_max[tvgnaloop]);
                if (!(tnlegd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_AR[i] - TVG_timer[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop]);
            }
            else if (G[j])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfgd] - T_timer[tnlfgd] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnleg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnleg] - T_timer[tnleg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlegd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TGL_max[i] - TGL_timer[i] + T_max[tnlegd] - ((G[i]||GL[i]) ? 0 : T_timer[tnlegd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);
            }
        }
    }
}

void Realisatietijd_NLEVG(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!AR[i])
        {
            if (CV[i] && !RA[i])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfgd] - (FG[i] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlevg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_max[i] - TVG_timer[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlevgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_max[i] - TVG_timer[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
            }
            else if (TIG[j][k])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfgd] - T_timer[tnlfgd] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnlevg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlevg] - T_timer[tnlevg] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnlevgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlevgd] - T_timer[tnlevgd] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
            }
        }
        else
        {
            if (CV[i] && !RA[i] && !(TVG_timer[i] > TVG_AR[i]))
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfgd] - (FG[i] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlevg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_AR[i] - TVG_timer[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlevgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_AR[i] - TVG_timer[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
            }
            else if (TIG[j][k])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfgd] - T_timer[tnlfgd] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnlevg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlevg] - T_timer[tnlevg] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnlevgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlevgd] - T_timer[tnlevgd] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
            }
        }
    }
    for (n = KFC_MAX[j]; n < GKFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!AR[i])
        {
            if (CV[i] && !RA[i])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfgd] - (FG[i] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop]);
                if (!(tnlevg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_max[i] - TVG_timer[i] + T_max[tnlevg] + T_max[tvgnaloop]);
                if (!(tnlevgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_max[i] - TVG_timer[i] + T_max[tnlevgd] + T_max[tvgnaloop]);
            }
            else if (G[j])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfgd] - T_timer[tnlfgd] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlevg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlevg] - T_timer[tnlevg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlevgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlevgd] - T_timer[tnlevgd] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
            }
        }
        else
        {
            if (CV[i] && !RA[i] && !(TVG_timer[i] > TVG_AR[i]))
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop]);
                    if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfgd] - (FG[i] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop]);
                    if (!(tnlevg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_AR[i] - TVG_timer[i] + T_max[tnlevg] + T_max[tvgnaloop]);
                if (!(tnlevgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + TVG_AR[i] - TVG_timer[i] + T_max[tnlevgd] + T_max[tvgnaloop]);
            }
            else if (G[j])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfgd] - T_timer[tnlfgd] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlevg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlevg] - T_timer[tnlevg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlevgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlevgd] - T_timer[tnlevgd] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
            }
        }
    }
}

void Realisatietijd_NLSG(count i, count j, count tnlsg, count tnlsgd)
{
   int k, n;
   for (n = 0; n < KFC_MAX[j]; n++)
   {
      k = KF_pointer[j][n];
      if (VS[i] && (RT[tnlsg] || T[tnlsg] || RT[tnlsgd] || T[tnlsgd]))
      {
         if (!(tnlsg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlsg] + TIG_max[j][k]);
         if (!(tnlsgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlsgd] + TIG_max[j][k]);
      }
      else if (T[tnlsg] || T[tnlsgd])
      {
         if (!(tnlsg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlsg] - T_timer[tnlsg] + TIG_max[j][k]);
         if (!(tnlsgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlsgd] - T_timer[tnlsgd] + TIG_max[j][k]);
      }
   }
}

void Realisatietijd_HardMeeverlengenDeelconflict(mulv fc1, mulv fc2)
{
   count fc;
   for (fc = 0; fc < FCMAX; fc++)
   {
      if (TIGR[fc2][fc] >= 0)
      {
         if (G[fc1])
         {
            if (!AR[fc1])
            {
               REALISATIETIJD[fc1][fc] = max(REALISATIETIJD[fc1][fc], TFG_max[fc1] - TFG_timer[fc1] + TVG_max[fc1] - TVG_timer[fc1] + TIGR[fc2][fc]);
            }
            else
            {
               REALISATIETIJD[fc1][fc] = max(REALISATIETIJD[fc1][fc], TFG_max[fc1] - TFG_timer[fc1] + TVG_AR[fc1] - ((TVG_AR[fc1] > TVG_timer[fc1]) ? TVG_timer[fc1] : 0) + TIGR[fc2][fc]);
            }
         }
      }
   }
}

void Realisatietijd_Ontruiming_Voorstart(count fcns, count fcvs, count tfo)
{
   RT[tfo] = G[fcns];
   if (T[tfo] && !G[fcvs])
   {
      REALISATIETIJD[fcns][fcvs] = TFG_max[fcns] - TFG_timer[fcns] + TVG_max[fcns] - TVG_timer[fcns] + T_max[tfo] - T_timer[tfo];
   }
}

void Realisatietijd_Ontruiming_Gelijkstart(count fc1, count fc2, count tfo12, count tfo21)
{
   RT[tfo12] = G[fc1];
   if (T[tfo12] && !G[fc2])
   {
      REALISATIETIJD[fc2][fc1] = TFG_max[fc1] - TFG_timer[fc1] + TVG_max[fc1] - TVG_timer[fc1] + T_max[tfo12] - T_timer[tfo12];
   }
   RT[tfo21] = G[fc2];
   if (T[tfo21] && !G[fc1])
   {
      REALISATIETIJD[fc1][fc2] = TFG_max[fc2] - TFG_timer[fc2] + TVG_max[fc2] - TVG_timer[fc2] + T_max[tfo21] - T_timer[tfo21];
   }
}

void Realisatietijd_Ontruiming_LateRelease(count fcvs, count fclr, count tlr, count tfo)
{
   RT[tfo] = G[fcvs] && (TG_timer[fcvs] > T_max[tlr]);
   if (T[tfo] && !G[fclr])
   {
      REALISATIETIJD[fclr][fcvs] = TFG_max[fcvs] - TFG_timer[fcvs] + TVG_max[fcvs] - TVG_timer[fcvs] + T_max[tfo] - T_timer[tfo];
   }
}

boolv Realisatietijd_Voorstart_Correctie(count fcvs, count fcns, count tvs)
{
   count n;
   boolv result;
   result = FALSE;
   if ((A[fcvs] || !(PG[fcvs] & PRIMAIR_OVERSLAG) || TRUE) && !G[fcvs])
   {
      for (n = 0; n < FCMAX; ++n)
      {
         if ((REALISATIETIJD[n][fcns] < REALISATIETIJD[n][fcvs] + T_max[tvs]) && REALISATIETIJD[n][fcvs] > 0)
         {
            REALISATIETIJD[n][fcns] = REALISATIETIJD[n][fcvs] + T_max[tvs];
            result = TRUE;
         }
      }
   }
   if (G[fcvs] || RA[fcvs] && (REALISATIETIJD_max[fcvs] <= 1))
   {
      if (TG[fcvs])
      {
         if (REALISATIETIJD[fcvs][fcns] < (T_max[tvs] - TG_timer[fcvs]))
         {
            REALISATIETIJD[fcvs][fcns] = T_max[tvs] - TG_timer[fcvs];
            result = TRUE;
         }
      }
      else
      {
         if (REALISATIETIJD[fcvs][fcns] < T_max[tvs])
         {
            REALISATIETIJD[fcvs][fcns] = T_max[tvs];
            result = TRUE;
         }
      }
   }
   return result;
}

boolv Realisatietijd_Gelijkstart_Correctie(count fc1, count fc2)
{
   count n;
   boolv result;
   result = FALSE;
   for (n = 0; n < FCMAX; ++n)
   {
      if ((A[fc1] || !(PG[fc1] & PRIMAIR_OVERSLAG)) && (A[fc2] || !(PG[fc2] & PRIMAIR_OVERSLAG)) && !G[fc1] && !G[fc2])
      {
         {
            if (REALISATIETIJD[n][fc1] < REALISATIETIJD[n][fc2])
            {
               REALISATIETIJD[n][fc1] = REALISATIETIJD[n][fc2];
               result = TRUE;
            }
            else
            {
               if (REALISATIETIJD[n][fc1] != REALISATIETIJD[n][fc2])
               {
                  REALISATIETIJD[n][fc2] = REALISATIETIJD[n][fc1];
                  result = TRUE;
               }
            }
         }
      }
   }
   return result;
}

boolv Realisatietijd_LateRelease_Correctie(count fclr, count fcvs, count tlr)
{
   count n;
   boolv result;
   result = FALSE;
   for (n = 0; n < FCMAX; ++n)
   {
      if (A[fclr] || !PG[fclr] || TRUE)
      {
         if (!G[fcvs])
         {
            if (REALISATIETIJD[n][fcvs] < REALISATIETIJD[n][fclr] - T_max[tlr])
            {
               REALISATIETIJD[n][fcvs] = REALISATIETIJD[n][fclr] - T_max[tlr];
               if (REALISATIETIJD[n][fcvs] < 0) REALISATIETIJD[n][fcvs] = 0;
               result = TRUE;
            }
         }
      }
   }
   return result;
}

void Bepaal_Realisatietijd_per_richting(void)
{
   int i, j;

   for (i = 0; i < FCMAX; ++i)
   {
      REALISATIETIJD_max[i] = 0;
      for (j = 0; j < FCMAX; ++j)
      {
         if (REALISATIETIJD_max[i] < REALISATIETIJD[j][i]) REALISATIETIJD_max[i] = REALISATIETIJD[j][i];
      }
   }
}

boolv ym_max_tig_Realisatietijd(count i, count prmomx) /* todo fc22 moet met 5 meeverlengen als 11 groen is. */
{
   register count n, j, k, m;
   boolv ym;

   if (MG[i])
   {     /* let op! i.v.m. snelheid alleen in MG[] behandeld	*/
      ym = TRUE;
      for (n = 0; n < FKFC_MAX[i]; ++n)
      {
         k = KF_pointer[i][n];
         if ((RA[k] || AAPR[k]) && !(FK_type[i][k] == FK_SG) && !(FK_type[i][k] == FK_EVG) && (REALISATIETIJD_max[k] >= 0))
         {
            ym = FALSE;
            for (j = 0; j < FKFC_MAX[k]; j++)
            {
               m = KF_pointer[k][j];
               if ((REALISATIETIJD[m][k] > REALISATIETIJD[i][k] + PRM[prmomx]) && !(TNL_type[m][i] == TNL_EG))
               {
                  ym = TRUE;
                  break;
               }
            }
            if (!ym) break;
         }
      }
   }
   else
   {
      ym = FALSE;
   }
   return ym;
}

void TegenhoudenDoorRealisatietijden()
{
   count i, j;

   for (i = 0; i < FCMAX; ++i)
   {
      X[i] &= ~BIT1;
      RR[i] &= ~BIT1;
   }
   for (i = 0; i < FCMAX; ++i)
   {
      for (j = 0; j < FCMAX; ++j)
      {
         if (REALISATIETIJD[i][j] > 0) X[j] |= BIT1; /* Als er een realisatietijd loopt van (fictief) conflict i, wordt richting j nog tegengehouden */
         if (REALISATIETIJD[i][j] > 150) RR[j] |= BIT1; /*  150 tijdelijk moet afhankleijk gemaakt wordt van de tijd de een richting eerder mag starten dan de volgrichting */
      }
   }
}

void InitInterStartGroenTijden()
{
   count i, j;
   for (i = 0; i < FC_MAX; i++)  /* zet alle GK en GKL conflicten om in FK */
   {
      for (j = 0; j < FC_MAX; j++)
      {
         if (TIG_max[i][j] < FK) TIG_max[i][j] = FK;
         TISG_PR[i][j] = NG;
         TISG_AR[i][j] = NG;
      }
   }
   pointer_conflicts();
}

void InterStartGroenTijden_VulHardeConflictenIn(void)
{
   count fc1, fc2, n;
   for (fc1 = 0; fc1 < FCMAX; ++fc1)
   {
      for (n = 0; n < KFC_MAX[fc1]; ++n)
      {
         fc2 = KF_pointer[fc1][n];
         TISG_PR[fc1][fc2] = TFG_max[fc1] + TVG_PR[fc1] + TIG_max[fc1][fc2];
         TISG_AR[fc1][fc2] = TFG_max[fc1] + TVG_AR[fc1] + TIG_max[fc1][fc2];
      }
   }
}

void InterStartGroenTijden_VulGroenGroenConflictenIn(void)
{
   count fc1, fc2, n;
   for (fc1 = 0; fc1 < FCMAX; ++fc1)
   {
      for (n = KFC_MAX[fc1]; n < GKFC_MAX[fc1]; ++n)
      {
         fc2 = KF_pointer[fc1][n];
         TISG_PR[fc1][fc2] = TFG_max[fc1] + TVG_PR[fc1];
         TISG_AR[fc1][fc2] = TFG_max[fc1] + TVG_AR[fc1];
      }
   }
}

void InterStartGroenTijd_NLEG(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!(tnlfg == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfgd == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnleg == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + TVG_PR[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlegd == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + TVG_PR[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfg == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfgd == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnleg == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + TVG_AR[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlegd == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + TVG_AR[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
    }
    for (n = KFC_MAX[j]; n < GKFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!(tnlfg == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop]);
        if (!(tnlfgd == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnleg == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + TVG_PR[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlegd == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + TVG_PR[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfg == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfgd == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnleg == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + TVG_AR[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlegd == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + TVG_AR[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
    }
}

void InterStartGroenTijd_NLEVG(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!(tnlfg == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfgd == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevg == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + TVG_PR[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevgd == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + TVG_PR[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfg == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfgd == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevg == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + TVG_AR[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevgd == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + TVG_AR[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
    }
    for (n = KFC_MAX[j]; n < GKFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!(tnlfg == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfgd == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevg == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + TVG_PR[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevgd == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], TFG_max[i] + TVG_PR[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfg == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfgd == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevg == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + TVG_AR[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevgd == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], TFG_max[i] + TVG_AR[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
    }
}

void InterStartGroenTijd_NLSG(count i, count j, count tnlsg, count tnlsgd)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!(tnlsg == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], T_max[tnlsg] + TIG_max[j][k]);
        if (!(tnlsgd == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], T_max[tnlsgd] + TIG_max[j][k]);
        if (!(tnlsg == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], T_max[tnlsg] + TIG_max[j][k]);
        if (!(tnlsgd == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], T_max[tnlsgd] + TIG_max[j][k]);
    }
}

void InterStartGroentijd_HardMeeverlengenDeelconflict(mulv fc1, mulv fc2)
{
   count fc;
   for (fc = 0; fc < FCMAX; fc++)
   {
      if (TIGR[fc2][fc] >= 0)
      {
         TISG_PR[fc1][fc] = max(TISG_PR[fc1][fc], TFG_max[fc1] + TVG_PR[fc1] + TIGR[fc2][fc]);
         TISG_AR[fc1][fc] = max(TISG_AR[fc1][fc], TFG_max[fc1] + TVG_PR[fc1] + TIGR[fc2][fc]);
      }
   }
}

boolv InterStartGroenTijd_Voorstart_Correctie(count fcvs, count fcns, count tvs)
{
    count n;
    boolv result;
    result = FALSE;
    for (n = 0; n < FCMAX; ++n)
    {
        if ((TISG_PR[n][fcns] < TISG_PR[n][fcvs] + T_max[tvs]) && (TISG_PR[n][fcvs] > 0))
        {
            TISG_PR[n][fcns] = TISG_PR[n][fcvs] + T_max[tvs];
            result = TRUE;
        }
        if ((TISG_AR[n][fcns] < TISG_AR[n][fcvs] + T_max[tvs]) && (TISG_AR[n][fcvs] > 0))
        {
            TISG_AR[n][fcns] = TISG_AR[n][fcvs] + T_max[tvs];
            result = TRUE;
        }
    }
    return result;
}

boolv InterStartGroenTijd_Gelijkstart_Correctie(count fc1, count fc2)
{
    count n;
    boolv result;
    result = FALSE;
    for (n = 0; n < FCMAX; ++n)
    {
        if (TISG_PR[n][fc1] < TISG_PR[n][fc2])
        {
            TISG_PR[n][fc1] = TISG_PR[n][fc2];
            result = TRUE;
        }
        else
        {
            if (TISG_PR[n][fc1] != TISG_PR[n][fc2])
            {
                TISG_PR[n][fc2] = TISG_PR[n][fc1];
                result = TRUE;
            }
        }
        if (TISG_AR[n][fc1] < TISG_AR[n][fc2])
        {
            TISG_AR[n][fc1] = TISG_AR[n][fc2];
            result = TRUE;
        }
        else
        {
            if (TISG_AR[n][fc1] != TISG_AR[n][fc2])
            {
                TISG_AR[n][fc2] = TISG_AR[n][fc1];
                result = TRUE;
            }
        }
    }
    return result;
}

boolv InterStartGroenTijd_LateRelease_Correctie(count fclr, count fcvs, count tlr)
{
    count n;
    boolv result;
    result = FALSE;
    for (n = 0; n < FCMAX; ++n)
    {
        if (TISG_PR[n][fcvs] < TISG_PR[n][fclr] - T_max[tlr])
        {
            TISG_PR[n][fcvs] = TISG_PR[n][fclr] - T_max[tlr];
            result = TRUE;
        }
        if (TISG_AR[n][fcvs] < TISG_AR[n][fclr] - T_max[tlr])
        {
            TISG_AR[n][fcvs] = TISG_AR[n][fclr] - T_max[tlr];
            result = TRUE;
        }
    }
    return result;
}


void NaloopEG_TVG_Correctie(count fc1, count fc2, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop)
{
   if (G[fc1] || GL[fc1] || T[tvgnaloop])
   {
      if (G[fc2])
      {
         if (!(tnlfg == NG)) TVG_max[fc2] = max(TVG_max[fc2], TFG_max[fc1] - TFG_timer[fc1] - TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
         if (!(tnlfgd == NG)) TVG_max[fc2] = max(TVG_max[fc2], TFG_max[fc1] - TFG_timer[fc1] - TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + T_max[tnlfg] - (FG[fc1] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);
         if (!(tnleg == NG)) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc1] + TVG_timer[fc2] + T_max[tnleg] - T_timer[tnleg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
         if (!(tnlegd == NG)) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + TGL_max[fc1] - TGL_timer[fc1] + T_max[tnlegd] - (GL[fc1] ? 0 : T_timer[tnlegd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);
      }
   }
   if ((TVG_max[fc2] < TVG_AR[fc2]) && AR[fc2]) TVG_max[fc2] = TVG_AR[fc2];
}

void NaloopEVG_TVG_Correctie(count fc1, count fc2, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop)
{
   //@@Peter past deze functie nog aan
   if (G[fc1] || GL[fc1] || T[tvgnaloop])
   {
      if (G[fc2])
      {
         if (!(tnlfg == NG)) TVG_max[fc2] = max(TVG_max[fc2], TFG_max[fc1] - TFG_timer[fc1] - TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
         if (!(tnlfgd == NG)) TVG_max[fc2] = max(TVG_max[fc2], TFG_max[fc1] - TFG_timer[fc1] - TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + T_max[tnlfg] - (FG[fc1] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);
         if (!(tnleg == NG)) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc1] + TVG_timer[fc2] + T_max[tnleg] - T_timer[tnleg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
         if (!(tnlegd == NG)) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + TGL_max[fc1] - TGL_timer[fc1] + T_max[tnlegd] - (GL[fc1] ? 0 : T_timer[tnlegd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);
      }
   }
   if ((TVG_max[fc2] < TVG_AR[fc2]) && AR[fc2]) TVG_max[fc2] = TVG_AR[fc2];
}

void NaloopVtg_TVG_Correctie(count fc1, count fc2, count tnlsg, count tnlsgd)
{
   if (!(tnlsg == NG)) TVG_max[fc2] = max(TVG_max[fc2], T_max[tnlsg] - T_timer[tnlsg] + TVG_timer[fc2]);
   if (!(tnlsgd == NG)) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc2] + T_max[tnlsgd] - T_timer[tnlsgd] + TVG_timer[fc2]);
}


void NaloopVtg(count fc1, count fc2, count dk, count hdk, boolv hnlsg, count tnlsg, count tnlsgd)
{
   if (tnlsg != NG)
   {
      RT[tnlsg] = SG[fc1] && H[hnlsg];
      if ((RA[fc1] || SG[fc1]) && MG[fc2] && H[hnlsg]) RW[fc2] |= BIT2;
      if (RT[tnlsg] || T[tnlsg])   MK[fc2] |= BIT12;
   }
   if ((dk != NG) && (tnlsgd != NG))
   {
      if (SG[fc1]) IH[hdk] = FALSE;
      IH[hdk] |= D[dk] && !G[fc1] && A[fc1];
      RT[tnlsgd] = SG[fc1] && H[hdk] && H[hnlsg];
      if ((RA[fc1] || SG[fc1]) && MG[fc2] && H[hnlsg]) RW[fc2] |= BIT2;
      if (RT[tnlsgd] || T[tnlsgd])   MK[fc2] |= BIT12;
   }
}

void NaloopEG(count fc1, count fc2, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop, ...)
{
   va_list argpt;
   count dp;
   RT[tvgnaloop] = FALSE;
   if ((RA[fc1] || SG[fc1]) && MG[fc2])             RW[fc2] |= BIT2;
   if (tnlfg != NG)
   {
      RT[tnlfg] = VS[fc1] || FG[fc1];
      if (RA[fc1] || RT[tnlfg] || T[tnlfg])   MK[fc2] |= BIT12;
      RT[tvgnaloop] |= G[fc1] || T[tnlfg];
   }
   if (tnlfgd != NG)
   {
      va_start(argpt, tvgnaloop);
      RT[tnlfgd] = FALSE;
      while ((dp = va_arg(argpt, va_count)) != END)
      {
         RT[tnlfgd] |= D[dp] && FG[fc1];
      }
      va_end(argpt);
      if (RA[fc1] || RT[tnlfgd] || T[tnlfgd] || FG[fc1])   MK[fc2] |= BIT12;
      RT[tvgnaloop] |= G[fc1] || GL[fc1] || T[tnlfgd];
   }
   if (tnleg != NG)
   {
      RT[tnleg] = G[fc1];
      if (RA[fc1] || RT[tnleg] || T[tnleg])   MK[fc2] |= BIT12;
      RT[tvgnaloop] |= G[fc1] || T[tnleg];
   }
   if (tnlegd != NG)
   {
      va_start(argpt, tvgnaloop);
      RT[tnlegd] = FALSE;
      while ((dp = va_arg(argpt, va_count)) != END)
      {
         RT[tnlegd] |= D[dp] && (G[fc1] || GL[fc1]);
      }
      va_end(argpt);
      if (RA[fc1] && MG[fc2])             RW[fc2] |= BIT2;
      if (RA[fc1] || RT[tnlegd] || T[tnlegd] || G[fc1] || GL[fc1])   MK[fc2] |= BIT12;
      RT[tvgnaloop] |= G[fc1] || GL[fc1] || T[tnlegd];
   }
   if (EVG[fc2]) AT[tvgnaloop] = TRUE; else AT[tvgnaloop] = FALSE;
}

void NaloopEVG(count fc1, count fc2, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop, ...)
{
   va_list argpt;
   count dp;
   RT[tvgnaloop] = FALSE;
   if ((RA[fc1] || SG[fc1]) && MG[fc2])             RW[fc2] |= BIT2;
   if (tnlfg != NG)
   {
      RT[tnlfg] = VS[fc1] || FG[fc1];
      if (RA[fc1] || RT[tnlfg] || T[tnlfg])   MK[fc2] |= BIT12;
      RT[tvgnaloop] |= G[fc1] && !MG[fc1] || T[tnlfg];
   }
   if (tnlfgd != NG)
   {
      va_start(argpt, tvgnaloop);
      RT[tnlfgd] = FALSE;
      while ((dp = va_arg(argpt, va_count)) != END)
      {
         RT[tnlfgd] |= D[dp] && FG[fc1];
      }
      va_end(argpt);
      if (RA[fc1] || RT[tnlfgd] || T[tnlfgd] || FG[fc1])   MK[fc2] |= BIT12;
      RT[tvgnaloop] |= G[fc1] && !MG[fc1] || T[tnlfgd];
   }
   if (tnlevg != NG)
   {
      RT[tnlevg] = G[fc1] && !MG[fc1];
      if (RA[fc1] || RT[tnlevg] || T[tnlevg])   MK[fc2] |= BIT12;
      RT[tvgnaloop] |= G[fc1] && !MG[fc1] || T[tnlevg];
   }
   if (tnlevgd != NG)
   {
      va_start(argpt, tvgnaloop);
      RT[tnlevgd] = FALSE;
      while ((dp = va_arg(argpt, va_count)) != END)
      {
         RT[tnlevgd] |= D[dp] && G[fc1] && !MG[fc1];
      }
      va_end(argpt);
      if (RA[fc1] && MG[fc2])             RW[fc2] |= BIT2;
      if (RA[fc1] || RT[tnlevgd] || T[tnlevgd] || G[fc1] && !MG[fc1])   MK[fc2] |= BIT12;
      RT[tvgnaloop] |= G[fc1] && !MG[fc1] || T[tnlevgd];
   }
   if (EVG[fc2]) AT[tvgnaloop] = TRUE;
}


boolv max_par(count fc)
{
    int k, n;
    for (n = 0; n < FKFC_MAX[fc]; ++n)
    {
        k = KF_pointer[fc][n];
        if (AAPR[k] && ((REALISATIETIJD_max[k] - REALISATIETIJD_max[fc] + offsetAR) < TISG_AR[fc][k]) || CV[k] ||
            (A[k] || PRIOFC[k]) && ((twacht[k] - REALISATIETIJD_max[fc] + offsetAR) < TISG_AR[fc][k]))
        {
            return FALSE;
        }

    }
    return TRUE;
}

boolv max_par_los(fc)
{
    int k, n;
    if (kcv(fc)) return FALSE;
    for (n = 0; n < FKFC_MAX[fc]; ++n)
    {
        k = KF_pointer[fc][n];
        if (AAPR[k] && !((TIG_max[fc][k] == FK) && (FK_type[fc][k] == FK_SG)) && ((REALISATIETIJD_max[k] - REALISATIETIJD_max[fc] + offsetAR) < TISG_AR[fc][k])) return FALSE;
    }
    return TRUE;
}

void max_wachttijd_modulen_primair_ISG(boolv* prml[], count ml, count ml_max, mulv twacht[])
{
    register count i, m, n, hml;
    mulv twacht_tmp = NG;


    /* reset wachttijden van alle fasecycli */
    /* ------------------------------------ */
    for (i = 0; i < FC_MAX; i++) twacht[i] = NG;

    /* bereken wachttijden van de primaire fasecycli van de actieve module */
    /* ------------------------------------------------------------------- */
    for (i = 0; i < FC_MAX; i++) {
        if ((prml[ml][i] & PRIMAIR_VERSNELD) && !PG[i] && R[i])
            twacht[i] = REALISATIETIJD_max[i];
    }


    /* bereken wachttijden van de primaire fasecycli van de volgende modulen */
    /* --------------------------------------------------------------------- */
    hml = ml + 1;
    if (hml >= ml_max)  hml = ML1;
    for (m = 1; m < ml_max; m++) {
        for (i = 0; i < FC_MAX; i++) {

            if ((prml[hml][i] & PRIMAIR_VERSNELD) && !PG[i] && (twacht[i] < 0)) {
                twacht[i] = REALISATIETIJD_max[i];
                for (n = 0; n < FC_MAX; n++)
                {
                    if (TISG_PR[i][n] >= 0 && i != n)
                        if (twacht[n] >= 0)
                        {
                            twacht_tmp = twacht[n] + TISG_PR[n][i];
                            if (twacht_tmp > twacht[i])  twacht[i] = twacht_tmp;
                        }
                }
            }
        }

        /* wachttijden voor overgeslagen fasecycli */
        /* -------------------------------------- */
        for (i = 0; i < FC_MAX; i++) {
            if (PG[i] && !G[i]) {
                twacht[i] = -3;
            }
        }

        /* volgende module */
        /* --------------- */
        hml++;
        if (hml >= ml_max)  hml = ML1;

    }
}

boolv yml_cv_pr_nl_ISG(boolv* prml[], count ml, count ml_max)
{
    register count i;
    register count hml = ml + 1;		/* next module			*/

    if (hml >= ml_max)  hml = ML1;		/* first module			*/
    for (i = 0; i < FC_MAX; ++i)
    {
        if ((prml[ml][i] & PRIMAIR_VERSNELD) && !prml[hml][i] &&
            (PR[i] && CV[i] && !(WG[i] && WS[i] && (RW[i] || YW[i])) && !(VG[i] && (MK[i] & BIT12))))
            return (TRUE);
    }
    return (FALSE);
}

void set_PG_Deelconflict_Voorstart(mulv fc1, mulv fc2)
{
    if (G[fc2] && !G[fc1] && !PG[fc1])
    {
        PG[fc1] |= PRIMAIR_OVERSLAG;
        RR[fc1] |= BIT2; /* fc22 mag niet alternatief realiseren */
    }
    else
    {
        RR[fc1] &= ~BIT2;
    }
}

set_PG_Deelconflict_LateRelease(mulv fc1, mulv fc2, mulv tlr)
{
    if (G[fc2] && !G[fc1] && (TG_timer[fc2] > T_max[tlr]) && !PG[fc1])
    {
        PG[fc1] |= PRIMAIR_OVERSLAG;
        RR[fc1] |= BIT2;
    }
    else
    {
        RR[fc1] &= ~BIT2;
    }
}

void MeeverlengenUitDoorDeelconflictVoorstart(mulv fc1, mulv fc2)
{
    if (RA[fc1] || AAPR[fc1]) YM[fc2] &= ~BIT4;
}

void MeeverlengenUitDoorDeelconflictLateRelease(mulv fc1, mulv fc2, mulv tlr)
{
    if ((RA[fc1] || AAPR[fc1]) && (TG_timer[fc1] >= T_max[tlr])) YM[fc2] &= ~BIT4;
}

void MeeverlengenUitDoorVoetgangerLos(count fcvtg, count hmadk)
{
   count n, fc;
   if ((AAPR[fcvtg] || RA[fcvtg]) && IH[hmadk])

   {
      for (n = 0; n < KFC_MAX[fcvtg]; ++n)
      {
         fc = KF_pointer[fcvtg][n];
         if ((REALISATIETIJD[fc][fcvtg] > 0) && MG[fc]) YM[fc] = FALSE;
      }
      for (n = KFC_MAX[fcvtg]; n < FKFC_MAX[fcvtg]; ++n)
      {
         fc = KF_pointer[fcvtg][n];
         if ((REALISATIETIJD[fc][fcvtg] > 0) && MG[fc] && (FK_type[fc][fcvtg] == FK_EG)) YM[fc] = FALSE;
      }
   }
}

void PercentageVerlengGroenTijdenISG(count fc, mulv percentage)
{
    TVG_max[fc] = (mulv)(((long)PRM[percentage] * (long)TVG_max[fc]) / 100);
}

boolv hf_wsg_nlISG(void)
{
    register count i;

    for (i = 0; i < FC_MAX; i++) {
        if (G[i] && !MG[i] && !WS[i] && !(WG[i] && (RW[i] & BIT2)) && !(MK[i] & BIT12) || G[i] && (MK[i] & ~BIT12) || GL[i] || TRG[i]
            || R[i] && A[i] && !BL[i] 
#ifndef NO_PRIO
           || PRIOFC[i]
#endif
           )
            return (TRUE);
    }
    return (FALSE);
}

boolv afsluiten_aanvraaggebied_prISG(boolv* prml[], count ml)
{
    register count i;
    for (i = 0; i < FC_MAX; i++) {
        if ((prml[ml][i] & PRIMAIR) && !PG[i] && !A[i] && fka(i)) /* was && fkaa(i) */
            PG[i] |= PRIMAIR_OVERSLAG;
    }
    return (TRUE);
}

void BepaalVolgrichtingen(void)
{
    count fc1, fc2;
    for (fc1 = 0; fc1 < FCMAX; ++fc1)
    {
        Volgrichting[fc1] = FALSE;
        for (fc2 = 0; fc2 < FCMAX; ++fc2)
        {
            if (TNL_type[fc2][fc1] == TNL_EG)
            {
                Volgrichting[fc1] = TRUE;
                break;
            }
        }

    }
}

void PrioAanwezig(void)
{
    count fc;
#ifndef NO_PRIO
    count prio;
#endif
    for (fc = 0; fc < FCMAX; ++fc) PRIOFC[fc] = FALSE;
#ifndef NO_PRIO
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        if (iPrioriteit[prio]) PRIOFC[iFC_PRIOix[prio]] = TRUE;
    }
#endif
}

void InitInterfunc()
{
    count i, j;
    for (i = 0; i < FCMAX; i++)  /* initialisatie TNL-type en FK_type */
    {
        for (j = 0; j < FCMAX; j++)
        {
            TNL_type[i][j] = TNL_NG;  /* defaults nalooptypen */
            FK_type[i][j] = FK_NG; /* defaultls FK_type */
        }
    }
}

void IsgDebug()
{
#ifndef AUTOMAAT
   count x, y;
   xyprintf(30, 1, "Realisatietijd");
   xyprintf(38 + 4 * FCMAX, 1, "InterStartGroentijd");
   xyprintf(46 + 8 * FCMAX, 1, "PAR");
   for (y = 0; y < FCMAX; ++y)
   {
      xyprintf(30, y + 4, "%2s", FC_code[y]);
      for (x = 0; x < FCMAX; ++x)
      {
         xyprintf(34 + 4 * x, y + 4, "%4d", REALISATIETIJD[y][x]);
      }
   }
   for (x = 0; x < FCMAX; ++x)
   {
      xyprintf(34 + 4 * x, 3, "%4s", FC_code[x]);
   }
   for (x = 0; x < FCMAX; ++x)
   {
      xyprintf(34 + 4 * x, 4 + FCMAX, "%4d", REALISATIETIJD_max[x]);
   }
   for (x = 0; x < FCMAX; ++x)
   {
      xyprintf(42 + 4 * (x + FCMAX), 3, "%4s", FC_code[x]);
   }
   for (y = 0; y < FCMAX; ++y)
   {
      xyprintf(38 + 4 * FCMAX, y + 4, "%2s", FC_code[y]);
      for (x = 0; x < FCMAX; ++x)
      {
         xyprintf(42 + 4 * (x + FCMAX), y + 4, "%4d", TISG_PR[y][x]);
      }
   }
   for (x = 0; x < FCMAX; ++x)
   {
      xyprintf(42 + 4 * (x + FCMAX), 4 + FCMAX, "%4d", twacht[x]);
   }
   xyprintf(36 + 4 * FCMAX, 4 + FCMAX, "twacht");
   for (y = 0; y < FCMAX; ++y)
   {
      xyprintf(46 + 8 * FCMAX, y + 4, "%2s", FC_code[y]);
      xyprintf(50 + 8 * FCMAX, y + 4, "%4d", max_par(y));
   }
#endif
}