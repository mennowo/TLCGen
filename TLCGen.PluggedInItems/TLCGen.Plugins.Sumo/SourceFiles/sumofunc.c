#ifdef SUMO
void set_detection_single(int i)
{
    SD[i] = ED[i] = FALSE;     /* reset start/end detectionsignals  */
    if (CIF_IS[i] & CIF_DET_BEZET)
    {
        if (!D[i])
        {
            SD[i] = D[i] = TDB[i] = TDH[i] = TOG[i] = TRUE;
            OG[i] = FALSE;
            TDH_timer[i] = TOG_timer[i] = 0;
            if (TDB_max[i] <= 0)
            {
                DB[i] = TRUE;
                TDB[i] = FALSE;
            }
        }
        else
        {
            if (TDB[i])
            {
                TDB_timer[i] += TE;
                if (TDB_timer[i] >= TDB_max[i])
                {
                    DB[i] = TRUE;
                    TDB[i] = FALSE;
                }
            }
            if (TBG_timer[i] < TBG_max[i]) TBG_timer[i] += TM;
            else
            {
                TBG[i] = FALSE;
                if (TBG_max[i] > 0) BG[i] = TRUE;
                else BG[i] = FALSE;
            }
        }
    }
    else
    {
        if (D[i])
        {
            ED[i] = TBG[i] = TRUE;
            D[i] = DB[i] = TDB[i] = BG[i] = FALSE;
            TDB_timer[i] = TBG_timer[i] = 0;
            if (TDH_max[i] <= 0) TDH[i] = FALSE;
        }
        else
        {
            if (TDH[i])
            {
                TDH_timer[i] += TE;
                if (TDH_timer[i] >= TDH_max[i]) TDH[i] = FALSE;
            }
            if (TOG_timer[i] < TOG_max[i])
            {
                if (DBOG) TOG_timer[i] += TM;
            }
            else
            {
                TOG[i] = FALSE;
                if (TOG_max[i] > 0) OG[i] = TRUE;
                else OG[i] = FALSE;
            }
        }
    }
# ifndef NO_DDFLUTTER
    if ((TFL_max[i] > 0) && (CFL_max[i] > 0))
    {
        if (SD[i])
        {
            if ((CFL_counter[i] < CFL_max[i]))
            {
                CFL_counter[i]++;
                if (CFL_counter[i] >= CFL_max[i])
                {
                    FL[i] = TRUE;
                    TFL_timer[i] = CFL_counter[i] = 0;
                }
            }
        }
        if (TS)
        {
            TFL[i] = TRUE;
            if (TFL_timer[i] < TFL_max[i])
            {
                TFL_timer[i] += TS;
            }
            else
            {
                FL[i] = FALSE;
                TFL_timer[i] = CFL_counter[i] = 0;
            }
        }
    }
    else
    {
        TFL[i] = FL[i] = FALSE;
        TFL_timer[i] = CFL_counter[i] = 0;
    }
#endif
}
#endif