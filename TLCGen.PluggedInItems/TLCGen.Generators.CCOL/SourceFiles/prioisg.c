#define    PRIO_AA_BIT      BIT6

mulv TVG_BR[FCMAX];
mulv TVG_afkap[FCMAX];
mulv TISG_afkap[FCMAX][FCMAX];
mulv TISG_BR[FCMAX][FCMAX];
mulv TVG_max_voor_afkap[FCMAX];
mulv TVG_AR_voor_afkap[FCMAX];

boolv TVG_max_opgehoogd[FCMAX];
boolv TVG_AR_opgehoogd[FCMAX];
boolv RW_OV[FCMAX] = { 0 };
void BepaalInterStartGroenTijden_PRIO(void);
void InterStartGroenTijd_NLEG_PRIO(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop);
void InterStartGroenTijd_NLSG_PRIO(count i, count j, count tnlsg, count tnlsgd);
boolv Correctie_TISG_Voorstart_PRIO(count fcvs, count fcns, count tvs);
boolv Correctie_TISG_Gelijkstart_PRIO(count fc1, count fc2);
boolv Correctie_TISG_LateRelease_PRIO(count fclr, count fcvs, count tlr);
void BepaalTVG_BR(void);
void VerhoogTVG_maxDoorPrio(void);
void VerlaagTVG_maxDoorConfPrio(void);
void PrioMeetKriteriumISG(void);
void BepaalStartGroenMomentenPrioIngrepen(void);
void PasTVG_maxAanStartGroenMomentenPrioIngrepen(void);
void TegenHoudenStartGroenISG(int fc, int iStartGroenFC);
void PrioBijzonderRealiserenISG(void);
void PrioTegenhoudenISG(void);
void MeeverlengenUitDoorPrio(void);
void PasRealisatieTijdenAanVanwegeRRPrio(void);
void PrioriteitsToekenning_ISG(void);
void PrioInit_ISG(void);
void PrioriteitsToekenning_ISG_Add(void);
void InterStartGroenTijd_NLEG_PRIO(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if ((TISG_PR[i][k] < (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfg == NG))
            if ((TISG_afkap[i][k] < (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfg == NG))
            {
                TISG_afkap[i][k] = (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
            }
        if ((TISG_afkap[i][k] < (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfgd == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_afkap[i][k] < (TFG_max[i] + TVG_afkap[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnleg == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + TVG_afkap[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_afkap[i][k] < (TFG_max[i] + TVG_afkap[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlegd == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + TVG_afkap[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfg == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfgd == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + TVG_BR[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnleg == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + TVG_BR[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + TVG_BR[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlegd == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + TVG_BR[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
    }
    for (n = KFC_MAX[j]; n < GKFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if ((TISG_afkap[i][k] < (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfg == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_afkap[i][k] < (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfgd == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_afkap[i][k] < (TFG_max[i] + TVG_afkap[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnleg == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + TVG_afkap[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_afkap[i][k] < (TFG_max[i] + TVG_afkap[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlegd == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + TVG_afkap[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfg == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfgd == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + TVG_BR[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnleg == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + TVG_BR[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + TVG_BR[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlegd == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + TVG_BR[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
    }
}
void InterStartGroenTijd_NLEVG_PRIO(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if ((TISG_PR[i][k] < (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfg == NG))
            if ((TISG_afkap[i][k] < (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfg == NG))
            {
                TISG_afkap[i][k] = (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
            }
        if ((TISG_afkap[i][k] < (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfgd == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_afkap[i][k] < (TFG_max[i] + TVG_afkap[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlevg == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + TVG_afkap[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_afkap[i][k] < (TFG_max[i] + TVG_afkap[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlevgd == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + TVG_afkap[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfg == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfgd == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + TVG_BR[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlevg == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + TVG_BR[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + TVG_BR[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlevgd == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + TVG_BR[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
    }
    for (n = KFC_MAX[j]; n < GKFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if ((TISG_afkap[i][k] < (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfg == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_afkap[i][k] < (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfgd == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_afkap[i][k] < (TFG_max[i] + TVG_afkap[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlevg == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + TVG_afkap[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_afkap[i][k] < (TFG_max[i] + TVG_afkap[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlevgd == NG))
        {
            TISG_afkap[i][k] = (TFG_max[i] + TVG_afkap[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfg == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlfgd == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + TVG_BR[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlevg == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + TVG_BR[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
        if ((TISG_BR[i][k] < (TFG_max[i] + TVG_BR[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k])) && !(tnlevgd == NG))
        {
            TISG_BR[i][k] = (TFG_max[i] + TVG_BR[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        }
    }
}
void InterStartGroenTijd_NLSG_PRIO(count i, count j, count tnlsg, count tnlsgd)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if ((TISG_afkap[i][k] < (T_max[tnlsg] + TIG_max[j][k])) && !(tnlsg == NG))
        {
            TISG_afkap[i][k] = T_max[tnlsg] + TIG_max[j][k];
        }
        if ((TISG_afkap[i][k] < (T_max[tnlsgd] + TIG_max[j][k])) && !(tnlsgd == NG))
        {
            TISG_afkap[i][k] = T_max[tnlsgd] + TIG_max[j][k];
        }
        if ((TISG_BR[i][k] < (T_max[tnlsg] + TIG_max[j][k])) && !(tnlsg == NG))
        {
            TISG_BR[i][k] = T_max[tnlsg] + TIG_max[j][k];
        }
        if ((TISG_BR[i][k] < (T_max[tnlsgd] + TIG_max[j][k])) && !(tnlsgd == NG))
        {
            TISG_BR[i][k] = T_max[tnlsgd] + TIG_max[j][k];
        }
    }
}
boolv Correctie_TISG_Voorstart_PRIO(count fcvs, count fcns, count tvs)
{
    count n;
    boolv result;
    result = FALSE;
    for (n = 0; n < FCMAX; ++n)
    {
        if ((TISG_afkap[n][fcns] < TISG_afkap[n][fcvs] + T_max[tvs]) && (TISG_afkap[n][fcvs] > 0))
        {
            TISG_afkap[n][fcns] = TISG_afkap[n][fcvs] + T_max[tvs];
            result = TRUE;
        }
        if ((TISG_BR[n][fcns] < TISG_BR[n][fcvs] + T_max[tvs]) && (TISG_BR[n][fcvs] > 0))
        {
            TISG_BR[n][fcns] = TISG_BR[n][fcvs] + T_max[tvs];
            result = TRUE;
        }
    }
    return result;
}
boolv Correctie_TISG_Gelijkstart_PRIO(count fc1, count fc2)
{
    count n;
    boolv result;
    result = FALSE;
    for (n = 0; n < FCMAX; ++n)
    {
        if (TISG_afkap[n][fc1] < TISG_afkap[n][fc2])
        {
            TISG_afkap[n][fc1] = TISG_afkap[n][fc2];
            result = TRUE;
        }
        else
        {
            if (TISG_afkap[n][fc1] != TISG_afkap[n][fc2])
            {
                TISG_afkap[n][fc2] = TISG_afkap[n][fc1];
                result = TRUE;
            }
        }
        if (TISG_BR[n][fc1] < TISG_BR[n][fc2])
        {
            TISG_BR[n][fc1] = TISG_BR[n][fc2];
            result = TRUE;
        }
        else
        {
            if (TISG_BR[n][fc1] != TISG_BR[n][fc2])
            {
                TISG_BR[n][fc2] = TISG_BR[n][fc1];
                result = TRUE;
            }
        }
    }
    return result;
}
boolv Correctie_TISG_LateRelease_PRIO(count fclr, count fcvs, count tlr)
{
    count n;
    boolv result;
    result = FALSE;
    for (n = 0; n < FCMAX; ++n)
    {
        if (TISG_afkap[n][fcvs] < TISG_afkap[n][fclr] - T_max[tlr])
        {
            TISG_afkap[n][fcvs] = TISG_afkap[n][fclr] - T_max[tlr];
            result = TRUE;
        }
        if (TISG_BR[n][fcvs] < TISG_BR[n][fclr] - T_max[tlr])
        {
            TISG_BR[n][fcvs] = TISG_BR[n][fclr] - T_max[tlr];
            result = TRUE;
        }
    }
    return result;
}

void VerhoogTVG_maxDoorPrio(void)
{
    count prio, fc, RestGroen, TVG_max_voor_OV[FCMAX], TVG_AR_voor_OV[FCMAX];
    for (fc = 0; fc < FCMAX; ++fc)
    {
        TVG_max_voor_OV[fc] = TVG_max[fc];
        TVG_AR_voor_OV[fc] = TVG_AR[fc];
        RW_OV[fc] = RW_OV[fc] && G[fc];
    }
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        if (iPrioriteit[prio] &&
            iPrioriteitsOpties[prio] & poGroenVastHouden)
        {
            fc = iFC_PRIOix[prio];
            if (!NietGroentijdOphogen[fc] && (iGroenBewakingsTimer[prio] < iGroenBewakingsTijd[prio]))
            {
                RestGroen = iGroenBewakingsTijd[prio] - iGroenBewakingsTimer[prio];
                if (VS[fc])
                {
                    TVG_max[fc] = (RestGroen > (TFG_max[fc] + TVG_max[fc])) ? TVG_max[fc] = RestGroen - TFG_max[fc] : TVG_max[fc];
                    TVG_AR[fc] = (RestGroen > (TFG_max[fc] + TVG_AR[fc])) ? TVG_AR[fc] = RestGroen - TFG_max[fc] : TVG_AR[fc];
                }

                if (FG[fc])
                {
                    TVG_max[fc] = (RestGroen > (TFG_max[fc] - TFG_timer[fc] + TVG_max[fc])) ? TVG_max[fc] = RestGroen - TFG_max[fc] + TFG_timer[fc] : TVG_max[fc];
                    TVG_AR[fc] = (RestGroen > (TFG_max[fc] - TFG_timer[fc] + TVG_AR[fc])) ? TVG_AR[fc] = RestGroen - TFG_max[fc] + TFG_timer[fc] : TVG_AR[fc];
                }
                if (WG[fc])
                {
                    if (RW_OV[fc])
                    {
                        TVG_max[fc] = RestGroen;
                        TVG_AR[fc] = RestGroen;
                    }
                    else
                    {

                        TVG_max[fc] = (RestGroen > TVG_max[fc]) ? TVG_max[fc] = RestGroen : TVG_max[fc];
                        TVG_AR[fc] = (RestGroen > TVG_AR[fc]) ? TVG_AR[fc] = RestGroen : TVG_AR[fc];
                    }
                }
                if (VG[fc])
                {
                    if (RW_OV[fc])
                    {
                        TVG_max[fc] = RestGroen + TVG_timer[fc];
                        TVG_AR[fc] = RestGroen + TVG_timer[fc];
                    }
                    else
                    {
                        if (RestGroen > (TVG_max[fc] - TVG_timer[fc])) TVG_max[fc] = RestGroen + TVG_timer[fc];
                        if (RestGroen > (TVG_AR[fc] - TVG_timer[fc])) TVG_AR[fc] = RestGroen + TVG_timer[fc];
                    }

                }
                if (MG[fc])
                {
                    TVG_max[fc] = (RestGroen > TVG_max[fc]) ? TVG_max[fc] = RestGroen : TVG_max[fc];
                    TVG_AR[fc] = (RestGroen > TVG_AR[fc]) ? TVG_AR[fc] = RestGroen : TVG_AR[fc];
                    RW_OV[fc] = TRUE;
                }
            }
        }
    }
    for (fc = 0; fc < FCMAX; ++fc)
    {
        if (TVG_max[fc] > TVG_max_voor_OV[fc]) TVG_max_opgehoogd[fc] = TRUE; else TVG_max_opgehoogd[fc] = FALSE;
        if (TVG_AR[fc] > TVG_AR_voor_OV[fc]) TVG_AR_opgehoogd[fc] = TRUE; else TVG_AR_opgehoogd[fc] = FALSE;
    }

}
void VerlaagTVG_maxDoorConfPrio(void)
{
    count prio, fc, k, n;

    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        if (iPrioriteit[prio] &&
            (iPrioriteitsOpties[prio] & poAfkappenKonfliktRichtingen))
        {
            fc = iFC_PRIOix[prio];
            for (n = 0; n < FKFC_MAX[fc]; ++n)
            {
                k = KF_pointer[fc][n];
                if (G[k] && !MG[k] && (TVG_max[k] > TVG_afkap[k])) TVG_max[k] = TVG_afkap[k];

            }
        }
    }
}

void PrioMeetKriteriumISG(void)
{
    int prio, fc;

    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        if (G[fc])
        {
            if (iPrioriteit[prio] &&
                iPrioriteitsOpties[prio] & poGroenVastHouden)
            {
                MK[fc] |= PRIO_MK_BIT;
                if (MG[fc] && !NietGroentijdOphogen[fc])
                {
                    RW[fc] |= PRIO_RW_BIT;
                }

            }
        }
    }
    for (fc = 0; fc < FCMAX; ++fc)
    {
        if (BR[fc] && !PR[fc] && !AR[fc] && !Volgrichting[fc])
        {
            MK[fc] &= PRIO_MK_BIT;
        }
    }
}
int StartGroenFCISG(int fc, int iGewenstStartGroen, int iPrioriteitsOptiesFC)
{
    count iStartGroenFC;

    if (REALISATIETIJD_max[fc] < iGewenstStartGroen)
    {
        iStartGroenFC = iGewenstStartGroen;
    }
    else
    {
        iStartGroenFC = REALISATIETIJD_max[fc];
    }
    return iStartGroenFC;
}

void BepaalStartGroenMomentenPrioIngrepen(void)
{
    count fc, prio, iRestRijtijd;
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        iStartGroen[prio] = -1;
        iRestRijtijd = (iRijTijd[prio] >= iRijTimer[prio]) ? (iRijTijd[prio] - iRijTimer[prio]) : 0;
        if (iPrioriteit[prio] && (iPrioriteitsOpties[prio] & poNoodDienst))
        {
            iStartGroen[prio] = (REALISATIETIJD_max[fc] > iRestRijtijd) ? REALISATIETIJD_max[fc] : iRestRijtijd;
        }
        if (iPrioriteit[prio] && (iPrioriteitsOpties[prio] & poBijzonderRealiseren) && (iPrioriteitsOpties[prio] & poAfkappenKonfliktRichtingen))
        {
            iStartGroen[prio] = (REALISATIETIJD_max[fc] > iRestRijtijd) ? REALISATIETIJD_max[fc] : iRestRijtijd;
        }
        if (iPrioriteit[prio] && (iPrioriteitsOpties[prio] & poBijzonderRealiseren) && !(iPrioriteitsOpties[prio] & poAfkappenKonfliktRichtingen))
        {
            iStartGroen[prio] = (REALISATIETIJD_max[fc] > iRestRijtijd) ? REALISATIETIJD_max[fc] : iRestRijtijd;
        }
        if (iPrioriteit[prio] && !(iPrioriteitsOpties[prio] & poBijzonderRealiseren) && (iPrioriteitsOpties[prio] & poAfkappenKonfliktRichtingen))
        {
            iStartGroen[prio] = (twacht_afkap[fc] > iRestRijtijd) ? twacht_afkap[fc] : iRestRijtijd;
        }
        if (iPrioriteit[prio] && !(iPrioriteitsOpties[prio] & poBijzonderRealiseren) && !(iPrioriteitsOpties[prio] & poAfkappenKonfliktRichtingen))
        {
            iStartGroen[prio] = (twacht[fc] > iRestRijtijd) ? twacht[fc] : iRestRijtijd;
        }

    }

}
void PasTVG_maxAanStartGroenMomentenPrioIngrepen(void)
{
    mulv prio, fc, n, k;
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        if (iPrioriteit[prio] && ((iPrioriteitsOpties[prio] & poNoodDienst) || (iPrioriteitsOpties[prio] & poAfkappenKonfliktRichtingen) && (AAPR[fc] || (iPrioriteitsOpties[prio] & poBijzonderRealiseren))))
        {
            for (n = 0; n < FKFC_MAX[fc]; ++n)
            {
                k = KF_pointer[fc][n];
                if (!NietGroentijdOphogen[k] && G[k] && !MG[k] && !(FK_type[k][fc] == FK_SG) && !AfslaandDeelconflict[k]
                    && (TVG_max[k] < (TVG_afkap[k] + iStartGroen[prio] - REALISATIETIJD[k][fc]))) TVG_max[k] = min((TVG_afkap[k] + iStartGroen[prio] - REALISATIETIJD[k][fc]), TVG_max_voor_afkap[k]);
            }
        }

    }
}
void BepaalTVG_BR(void)
{
    mulv prio, fc;
    for (fc = 0; fc < FCMAX; ++fc)
    {
        TVG_BR[fc] = 0;
    }
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        if (iPrioriteit[prio] && ((iPrioriteitsOpties[prio] & poNoodDienst) || (iPrioriteitsOpties[prio] & poGroenVastHouden)))
        {
            if (iGroenBewakingsTijd[prio] > (TVG_BR[fc] + TFG_max[fc])) TVG_BR[fc] = iGroenBewakingsTijd[prio] - TFG_max[fc];
        }
    }
    for (fc = 0; fc < FCMAX; ++fc)
    {
        if (BR[fc]) TVG_max[fc] = TVG_BR[fc];
    }

}
/* -------------------------------------------------------
   OVTegenhouden zorgt voor het tegenhouden van konflikten
   van de OV-richtingen.
   Daartoe wordt het OV-bitje van de instructievariabele
   RR[fc] gebruikt.
   Bij een nooddienstinmelding wordt tevens het OV-bitje
   van de instructievariabele RTFB opgezet.
   Een konflikt wordt tegengehouden als een realisatie
   zou veroorzaken dat het startgroenmoment van een
   OV-richting niet meer haalbaar is.
   De te realiseren groentijd van de richting is
   afhankelijk van:
   - of de konflikterende OV-richting beschikt over de
     prioriteitsopties poBijzonderRealiseren, poNoodDienst
     en/of poAfkappenKonfliktRichtingen.
   ------------------------------------------------------- */
void PrioTegenhoudenISG(void)
{
    int prio, fc;

    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        if (iPrioriteit[prio] && iPrioriteitsOpties[prio] & poBijzonderRealiseren)
        {
            fc = iFC_PRIOix[prio];
            TegenHoudenStartGroenISG(fc, iStartGroen[prio]);
            if (iPrioriteitsOpties[prio] & poNoodDienst)
            {
                RTFB |= PRIO_RTFB_BIT;
            }
        }
    }
    TegenhoudenConflictenExtra();
}
void TegenHoudenStartGroenISG(int fc, int iStartGroenFC)
{
    int i, k;
    for (i = 0; i < FKFC_MAX[fc]; ++i)
    {
#if (CCOL_V >= 95)
        k = KF_pointer[fc][i];
#else
        k = TO_pointer[fc][i];
#endif
        if (iStartGroenFC <= (TISG_afkap[k][fc] + REALISATIETIJD_max[k]) || AfslaandDeelconflict[k])
        {
            RR[k] |= PRIO_RR_BIT;
        }
    }
}
void PrioBijzonderRealiserenISG(void)
{
    int prio, fc;
    for (fc = 0; fc < FCMAX; ++fc)
    {
        //       AA_set[fc] = FALSE;
        //       BR_set[fc] = FALSE;
    }


    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        if (iPrioriteit[prio] &&
            iStartGroen[prio] <= 1 && iStartGroen[prio] >= 0 &&
            iPrioriteitsOpties[prio] & poBijzonderRealiseren && !AAPR[fc])
        {
            iBijzonderRealiseren[prio] = 1;
            /* voorkeuraanvraag openbaar vervoer */
            BR[fc] |= BIT6;
            //           BR_set[fc] = TRUE;
            if (!kcv(fc))
            {
                AA[fc] |= BIT6;
                //               AA_set[fc] = TRUE;
            }
        }
        else
        {
            iBijzonderRealiseren[prio] = 0;
        }
    }
}
void MeeverlengenUitDoorPrio(void)
{
    count prio, fc, n, k;
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        if (iPrioriteit[prio])
        {
            fc = iFC_PRIOix[prio];
            for (n = 0; n < FKFC_MAX[fc]; ++n)
            {
                k = KF_pointer[fc][n];
                if ((REALISATIETIJD[k][fc] >= iStartGroen[prio])) YM[k] &= ~BIT4;
            }
        }

    }
}
void PasRealisatieTijdenAanVanwegeRRPrio(void)
{
    count prio, fc, n, k;
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        for (n = 0; n < FKFC_MAX[fc]; ++n)
        {
            k = KF_pointer[fc][n];
            if ((RR[k] & PRIO_RR_BIT) && AAPR[k] && R[k] && R[fc])
            {
                if (REALISATIETIJD[fc][k] < (iStartGroen[prio] + TISG_BR[fc][k])) REALISATIETIJD[fc][k] = iStartGroen[prio] + TISG_BR[fc][k];
            }
        }
    }
}

void PrioInit_ISG(void)
{
    int prio1, prio2, fc1, fc2, fc;

    /* default OV-instellingen */
    for (fc = 0; fc < FCMAX; ++fc)
    {
        iMaximumWachtTijd[fc] = DEFAULT_MAX_WACHTTIJD;
        iPRM_ALTP[fc] = TFG_max[fc];
        iSCH_ALTG[fc] = TRUE;
        iRealisatieTijd[fc] = iM_RealisatieTijd + (fc * FCMAX);

        /* Meerealisatie default uit (NG) */
        iPrioMeeRealisatie[fc] = iM_PrioMeeRealisatie + (fc * FCMAX);
        for (fc2 = 0; fc2 < FCMAX; ++fc2)
        {
            iPrioMeeRealisatie[fc][fc2] = NG;
        }
    }

    /* werkelijke OV-instellingen */
    PrioInstellingen();

    /* initialisatie overige OV-variabelen */
    for (prio1 = 0; prio1 < prioFCMAX; ++prio1)
    {
        fc1 = iFC_PRIOix[prio1];
        prioTO_pointer[prio1] = prioM_TO_pointer + (prio1 * prioFCMAX);
        prioKFC_MAX[prio1] = 0;
        for (prio2 = 0; prio2 < prioFCMAX; ++prio2)
        {
            fc2 = iFC_PRIOix[prio2];
#if (CCOL_V >= 95) && !defined NO_TIGMAX
            if (TIG_max[fc1][fc2] >= 0)
#else
            if (TO_max[fc1][fc2] >= 0)
#endif
            {
                prioTO_pointer[prio1][prioKFC_MAX[prio1]] = prio2;
                (prioKFC_MAX[prio1])++;
            }
        }
        iBlokkeringsTimer[prio1] = MAX_INT;
        iInPrioriteitsNiveau[prio1] = iM_InPrioriteitsNiveau + (prio1 * MAX_AANTAL_INMELDINGEN);
        iInPrioriteitsOpties[prio1] = iM_InPrioriteitsOpties + (prio1 * MAX_AANTAL_INMELDINGEN);
        iInRijTimer[prio1] = iM_InRijTimer + (prio1 * MAX_AANTAL_INMELDINGEN);
        iInGroenBewakingsTimer[prio1] = iM_InGroenBewakingsTimer + (prio1 * MAX_AANTAL_INMELDINGEN);
        iInOnderMaximumVerstreken[prio1] = iM_InOnderMaximumVerstreken + (prio1 * MAX_AANTAL_INMELDINGEN);
        iInMaxWachtTijdOverschreden[prio1] = iM_InMaxWachtTijdOverschreden + (prio1 * MAX_AANTAL_INMELDINGEN);/*@@@ DSC*/
        iInID[prio1] = iM_InID + (prio1 * MAX_AANTAL_INMELDINGEN);
        iPrioriteit[prio1] = FALSE;
        iAantalInmeldingen[prio1] = 0;
    }
    for (prio1 = 0; prio1 < prioFCMAX; ++prio1)
    {
        fc1 = iFC_PRIOix[prio1];
        prioGKFC_MAX[prio1] = prioKFC_MAX[prio1];
        for (prio2 = 0; prio2 < prioFCMAX; ++prio2)
        {
            fc2 = iFC_PRIOix[prio2];
#if (CCOL_V >= 95) && !defined NO_TIGMAX
            if (TIG_max[fc1][fc2] == GK || TIG_max[fc1][fc2] == GKL) /* toegevoegd Ane 25-04-2011, GKL */
#else
            if (TO_max[fc1][fc2] == GK || TO_max[fc1][fc2] == GKL)
#endif
            {
                prioTO_pointer[prio1][prioGKFC_MAX[prio1]] = prio2;
                (prioGKFC_MAX[prio1])++;
            }
        }
    }
    for (prio1 = 0; prio1 < prioFCMAX; ++prio1)
    {
        fc1 = iFC_PRIOix[prio1];
        prioFKFC_MAX[prio1] = prioGKFC_MAX[prio1];
        for (prio2 = 0; prio2 < prioFCMAX; ++prio2)
        {
            fc2 = iFC_PRIOix[prio2];
#if (CCOL_V >= 95) && !defined NO_TIGMAX
            if (TIG_max[fc1][fc2] == FK) /* toegevoegd Ane 25-04-2011, GKL */
#else
            if (TO_max[fc1][fc2] == FK)
#endif
            {
                prioTO_pointer[prio1][prioFKFC_MAX[prio1]] = prio2;
                (prioFKFC_MAX[prio1])++;
            }
        }
    }
}
void PrioriteitsToekenning_ISG(void)
{
    int prio, inm, i, kov, fc;

    /* wijz. Ane Max.Wachttijdoverschreden kijkt alleen naar de prio-conflicten! */
    /* Bepaal prioriteitsniveau */
    /* van alle OV-richtingen   */
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        iPrioriteitsNiveau[prio] = 0;
        iPrioriteitsOpties[prio] = 0;
        iAantalPrioriteitsInmeldingen[prio] = 0;
        iGroenBewakingsTimer[prio] = iGroenBewakingsTijd[prio];
        for (inm = 0; inm < iAantalInmeldingen[prio]; ++inm)
        {
            if (!G[fc] || kg(fc))
            {
                iInOnderMaximumVerstreken[prio][inm] = 0;
                iInMaxWachtTijdOverschreden[prio][inm] = 0;
            }
            iPrioriteitsOpties[prio] |= iInPrioriteitsOpties[prio][inm] & poAanvraag;
            if (!iSelDetFout[prio] && !iInOnderMaximumVerstreken[prio][inm] && !iInMaxWachtTijdOverschreden[prio][inm] ||/*@@@ DSC*/
                iInPrioriteitsOpties[prio][inm] & poNoodDienst)
            {
                (iAantalPrioriteitsInmeldingen[prio])++;
                if (iGroenBewakingsTimer[prio] > iInGroenBewakingsTimer[prio][inm])
                {
                    iGroenBewakingsTimer[prio] = iInGroenBewakingsTimer[prio][inm];
                }
                if (iPrioriteitsNiveau[prio] < iInPrioriteitsNiveau[prio][inm])
                {
                    iPrioriteitsNiveau[prio] = iInPrioriteitsNiveau[prio][inm];
                }
                iPrioriteitsOpties[prio] |= iInPrioriteitsOpties[prio][inm];
            }
        }
    }

    PrioriteitsOpties();

    /* Trek prioriteiten in */
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        fc = iFC_PRIOix[prio];
        if (BL[fc] ||
            iAantalPrioriteitsInmeldingen[prio] == 0
            /* Rotterdam - Modificatie 1								*/
            /* Aanpassing: niet terugnemen ingezette prioriteit indien	*/
            /* wachttijd tussentijds alsnog wordt overschreden			*/
            /* 19-01-2009 / Peter Snijders								*/
            /*|| !G[fc] && iMaximumWachtTijdOverschreden[prio] &&
                 !(iPrioriteitsOpties[prio] & poNoodDienst)*/
            )
        {
            iPrioriteit[prio] = 0;
        }
        iKOVPrio[prio] = -1;
        for (i = 0; i < prioFKFC_MAX[prio]; ++i)
        {
            kov = prioTO_pointer[prio][i];
            if (!(iPrioriteitNooitAfkappen[prio] && G[fc]) &&
                iPrioriteitsNiveau[kov] > iPrioriteitsNiveau[prio] && !iXPrio[kov] &&
                (!G[fc] || iPrioriteitsOpties[kov] & poAfkappenKonflikterendOV))
            {
                iPrioriteit[prio] = 0;
                iKOVPrio[prio] = kov;
            }
        }
    }
    /* Deel prioriteiten uit */
    for (prio = 0; prio < prioFCMAX; prio++)
    {
        fc = iFC_PRIOix[prio];
        if (!BL[fc] &&
            !iXPrio[prio] &&
            iKOVPrio[prio] == -1 &&
            iAantalInmeldingen[prio] > 0 &&
            !iPrioriteit[prio] &&
            (iPrioriteitsOpties[prio] & poNoodDienst ||
                !iSelDetFout[prio] &&
                !(!G[fc] && iMaximumWachtTijdOverschreden[prio]) &&
                (!G[fc] || !iOnderMaximumVerstreken[prio]) &&
                /* Werner : hieronder toegevoegd als beveiliging als ondermax niet is ingevuld */
                !(G[fc] && iMaximumWachtTijdOverschreden[prio] && (iOnderMaximum[prio] <= 0)) &&
                iBlokkeringsTimer[prio] >= iBlokkeringsTijd[prio] &&
                !iWachtOpKonflikt[prio]))
        {
            iPrioriteit[prio] = 1;
            for (i = 0; iPrioriteit[prio] && i < prioFKFC_MAX[prio]; ++i)
            {
                kov = prioTO_pointer[prio][i];
                if (iPrioriteit[kov])
                {
                    iPrioriteit[prio] = 0;
                }
            }
        }
    }


    PrioriteitsToekenningExtra();
}
void PrioriteitsToekenning_ISG_Add(void) {
    /* -----------------------------------------
       Pas hier zonodig de Prioriteit aan, bijv:
       iPrioriteit[prioFC02] = FALSE;
       ----------------------------------------- */
       /*    count prio, fc, k, i;
               for (prio = 0; prio < prioFCMAX; prio++)
               {
                   fc = iFC_PRIOix[prio];
                   for (i = KFC_MAX[fc]; i < FKFC_MAX[fc]; ++i)
                   {
                       k = KF_pointer[fc][i];
                       if (G[k] && G[fc] && (FK_type[k][fc] == FK_EG)) iXPrio[prio] = TRUE;  else iXPrio[prio] = FALSE;
                   }
               }
       */
}
boolv fkra(count i)
{
    register count n, k;

    for (n = KFC_MAX[i]; n < FKFC_MAX[i]; n++)
    {
        k = KF_pointer[i][n];
        if (((R[k] || GL[k]) && AA[k] || RA[k]) && (FK_type[i][k] == FK_EG))
            return (TRUE);
    }
    return (FALSE);
}


PrioMeerealisatieDeelconflictVoorstart(mulv fc1, mulv fc2, mulv tvs)
{
    count prio;
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        if (iPrioriteit[prio] && (iPrioriteitsOpties[prio] & poBijzonderRealiseren))
        {
            if (iFC_PRIOix[prio] == fc2)
            {
                A[fc1] |= PRIO_A_BIT;
                if ((iStartGroen[prio] >= 0) && (iStartGroen[prio] <= T_max[tvs] + 1))
                {
                    AA[fc1] |= PRIO_AA_BIT;

                }
                if ((iStartGroen[prio] >= 0) && !kcv(fc1) && !fkra(fc1)) YM[fc1] |= PRIO_YM_BIT;
            }
            BR[fc1] = BR[fc2];
        }
    }
}
PrioMeerealisatieDeelconflictLateRelease(mulv fc1, mulv fc2, mulv tlr)
{
    count prio;
    for (prio = 0; prio < prioFCMAX; ++prio)
    {
        if (iPrioriteit[prio] && (iPrioriteitsOpties[prio] & poBijzonderRealiseren))
        {
            if (iFC_PRIOix[prio] == fc2)
            {
                A[fc1] |= PRIO_A_BIT;
                if ((iStartGroen[prio] >= 0) && (iStartGroen[prio] <= 1))
                {
                    AA[fc1] |= PRIO_AA_BIT;
                    if (BR[fc2]) BR[fc1] = TRUE;
                }
                if ((iStartGroen[prio] >= 0) && !kcv(fc1) && !fkra(fc1)) YM[fc1] |= PRIO_YM_BIT;
            }
        }
    }
}
void InterStartGroentijd_MeeverlengenDeelconflict_PRIO(mulv fc1, mulv fc2)
{
    count fc;
    for (fc = 0; fc < FCMAX; fc++)
    {
        if (TIGR[fc2][fc] >= 0)
        {
            if (TISG_BR[fc1][fc] < TFG_max[fc1] + TVG_BR[fc1] + TIGR[fc2][fc])
                TISG_BR[fc1][fc] = TFG_max[fc1] + TVG_BR[fc1] + TIGR[fc2][fc];
            if (TISG_afkap[fc1][fc] < TFG_max[fc1] + TVG_afkap[fc1] + TIGR[fc2][fc])
                TISG_afkap[fc1][fc] = TFG_max[fc1] + TVG_afkap[fc1] + TIGR[fc2][fc];
        }
    }
}
void PasRealisatieTijdenAanVanwegeBRLateRelease(count fc)
{
    if ((RA[fc] || AA[fc]) && BR[fc])
    {
        int n, i;
        for (n = 0; n < FKFC_MAX[fc]; n++)
        {
            i = KF_pointer[fc][n];
            REALISATIETIJD[fc][i] = REALISATIETIJD_max[fc] + TISG_BR[fc][i];
        }
    }
}

void ResetIsgPrioVars()
{
    count i, j;

    /* zet alle interstartgroentijden op -1 */
    for (i = 0; i < FC_MAX; i++)
    {
        for (j = 0; j < FC_MAX; j++)
        {
            TISG_PR[i][j] = NG;
            TISG_AR[i][j] = NG;
            TISG_afkap[i][j] = NG;
            TISG_BR[i][j] = NG;
        }
    }
}

void VulHardEnGroenConflictenInPrioVars()
{
    count fc1, fc2, n;

    /* Bepalen realisatietijden */
    /* Vul harde conflicten in */
    for (fc1 = 0; fc1 < FCMAX; ++fc1)
    {
        for (n = 0; n < KFC_MAX[fc1]; ++n)
        {
            fc2 = KF_pointer[fc1][n];
            TISG_afkap[fc1][fc2] = TFG_max[fc1] + TVG_afkap[fc1] + TIG_max[fc1][fc2];
            TISG_BR[fc1][fc2] = TFG_max[fc1] + TVG_BR[fc1] + TIG_max[fc1][fc2];
        }
    }
    /* Vul groen-groenconflicten in */
    for (fc1 = 0; fc1 < FCMAX; ++fc1)
    {
        for (n = KFC_MAX[fc1]; n < GKFC_MAX[fc1]; ++n)
        {
            fc2 = KF_pointer[fc1][n];
            TISG_afkap[fc1][fc2] = TFG_max[fc1] + TVG_afkap[fc1];
            TISG_BR[fc1][fc2] = TFG_max[fc1] + TVG_BR[fc1];
        }
    }
}

void ResetNietGroentijdOphogen()
{
    int fc;
    for (fc = 0; fc < FCMAX; ++fc)
    {
        NietGroentijdOphogen[fc] = 0;
    }
}