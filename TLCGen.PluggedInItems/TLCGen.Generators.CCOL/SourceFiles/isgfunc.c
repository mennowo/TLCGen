#include "isgfunc.h"

mulv TNL_type[FCMAX][FCMAX]; /* type naloop */
mulv FK_type[FCMAX][FCMAX]; /* type fictief conflict */

mulv TISG_PR[FCMAX][FCMAX]; /* interstartgroentijd primair    */
mulv TVG_basis[FCMAX];
mulv TVG_AR[FCMAX];
mulv TISG_AR[FCMAX][FCMAX]; /* interstartgorentijd altenatief */
mulv TISG_AR_los[FCMAX][FCMAX];
mulv TVG_PR[FCMAX];
mulv TVG_old[FCMAX];
mulv TVG_AR_old[FCMAX];
mulv REALISATIETIJD_max[FCMAX];
mulv REALISATIETIJD_max_wtv[FCMAX];
mulv TIGR[FCMAX][FCMAX];
mulv PRIOFC[FCMAX]; /* aanwezigheid prioriteitsaanvragen */

bool NietGroentijdOphogen[FCMAX];
mulv twacht[FCMAX];
mulv twacht_wtv[FCMAX];
mulv twacht_AR[FCMAX];
mulv twacht_AR_wtv[FCMAX];
mulv twacht_afkap[FCMAX];
mulv REALISATIETIJD[FCMAX][FCMAX];
mulv REALISATIETIJD_wtv[FCMAX][FCMAX];

bool Volgrichting[FCMAX];
bool AfslaandDeelconflict[FCMAX] = { 0 };

mulv TISG_rgv[FCMAX][FCMAX];
mulv TISG_basis[FCMAX][FCMAX];
mulv TVG_rgv[FCMAX];
mulv init_tvg;
mulv TISG_afkap[FCMAX][FCMAX];

bool PAR_los[FCMAX];

/* BEPAAL RESULTERENDE INTERGROENTIJDEN - TIGR[][] */
/* ----------------------------------------------- */
/* void BepaalIntergroenTijden(void) bepaalt de initiele waarden van de resulterende intergroentijden matrix TIGR[][] en
 * maakt hiervoor een kopie van de integroentijden matrix TIG[][].
 * de resulterende intergroentijden TIGR[][] dienen in de regelapplicatie te worden gecorrigeerd voor naloop richtingen met
 * de functie corrigeerTIGRvoorNalopen(count fc1, count fc2, mulv tnleg, mulv tnlegd, mulv tvgnaloop).
 * BepaalIntergroenTijden() wordt aangeroepen door de applicatiefunctie BepaalRealisatieTijden().
 * BepaalRealisatieTijden(void) wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 */

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

/* CORRIGEER TIGR[][] VOOR NALOPEN */
/* ------------------------------- */
/* void corrigeerTIGRvoorNalopen(count fc1, count fc2, count tnleg, count tnlegd, count tvgnaloop) kan in de regelapplicatie worden gebruikt
 * voor de correctie van de de resulterende intergroentijden TIGR[][] voor de conflicten van EG-naloop richtingen; na initialisatie van
 * de resulterende intergroentijden matrix TIGR[][].
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus van de voedende richting
 * fc2 - index fasecyclus van de nalooprichting
 *
 * tnleg     - index T_max[] - Naloop op einde groen van fc1 naar fc2; NG indien niet gebruikt.
 * tnlegd    - index T_max[] - Detectieafhankelijke naloop op einde groen van fc1 naar fc2; NG indien niet gebruikt.
 * tvgnaloop - index T_max[] - Maximale verlengtijd op eigen detectie na aflopen nalooptijden voor de naloop richting.
 *
 * voorbeeld: BepaalIntergroenTijden();      // startwaarden voor TIGR[][]
 *            corrigeerTIGRvoorNalopen(fc02, fc62, tnleg0262, tnlegd0262, tvgnaloop0262); // EG-naloop van fc02 naar fc62
 *            corrigeerTIGRvoorNalopen(fc08, fc68, tnleg0868, tnlegd0868, tvgnaloop0868); // EG-naloop van fc08 naar fc68
 *
 * corrigeerTIGRvoorNalopen() wordt aangeroepen vanuit de applicatiefunctie BepaalRealisatieTijden().
 *
 */

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

/* INITIALISATIE REALISATIETIJDEN */
/* ------------------------------ */
/* void InitRealisatieTijden(void) bepaalt de initiele waarden van de REALISATIETIJD[][] en zet alle waarden op NG (niet gebruikt).
 * de REALISATIETIJD[][]-en dienen in de regelapplicatie te worden bepaald en gecorrigeerd voor de synchronisaties o.a naloop richtingen.
 *
 * InitRealisatieTijden(void) wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void).
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door de applicatiefunctie application().
 */

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

/* REALISATIETIJDEN VUL HARDE CONFLICTEN IN */
/* ---------------------------------------- */
/* void RealisatieTijden_VulHardeConflictenIn(void) vult de REALISATIETIJD[][] in voor de aanwezige harde conflicten (TIG[fc1][fc2] is waar (TRUE)).
 *
 * RealisatieTijden_VulHardeConflictenIn(void) bepaalt de waarde van REALISATIETIJD[][] voor de harde conflicten van de gerealiseerde fasecycli.
 * RealisatieTijden_VulHardeConflictenIn(void) wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na InitRealisatieTijden().
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door de applicatiefunctie application().
 *
 *  //Realisatietijden
 *  InitRealisatieTijden();
 *  RealisatieTijden_VulHardeConflictenIn();
 *
 */

void RealisatieTijden_VulHardeConflictenIn(void)
{
    count fc1, fc2, n;

    for (fc1 = 0; fc1 < FCMAX; ++fc1)
    {
        for (n = 0; n < KFC_MAX[fc1]; ++n)
        {
            fc2 = KF_pointer[fc1][n]; /* bepaal de index van de conflicterende fasecyclus */
            if (TIG[fc1][fc2]) /* aanwezig conflict - lopende intergroentijd */
            {
                if (PR[fc1] || BR[fc1])
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

/* REALISATIETIJDEN VUL GROEN-GROEN CONFLICTEN IN */
/* ---------------------------------------------- */
/* void RealisatieTijden_VulGroenGroenConflictenIn(void) vult de REALISATIETIJD[][] in voor de aanwezige groen/groen conflicten (TIG[fc1][fc2] is waar (TRUE)).
 *
 * RealisatieTijden_VulGroenGroenConflictenIn(void) bepaalt de waarde van REALISATIETIJD[][] voor de groen/groen conflicten van de gerealiseerde fasecycli.
 * RealisatieTijden_VulHardeConflictenIn(void) wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na InitRealisatieTijden() en
 * RealisatieTijden_VulHardeConflictenIn().
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door de applicatiefunctie application().
 *
 *  //Realisatietijden
 *  InitRealisatieTijden();
 *  RealisatieTijden_VulHardeConflictenIn();
 *  RealisatieTijden_VulGroenGroenConflictenIn(); 
 */

void RealisatieTijden_VulGroenGroenConflictenIn(void)
{
    count fc1, fc2, n;

    for (fc1 = 0; fc1 < FCMAX; ++fc1)
    {
        for (n = GKFC_MAX[fc1]; n < FKFC_MAX[fc1]; ++n)
        {
            fc2 = KF_pointer[fc1][n]; /* bepaal de index van de conflicterende fasecyclus */
            if (TIG[fc1][fc2]) /* aanwezig conflict - lopende intergroentijd */
            {
                if (PR[fc1] || BR[fc1])
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

/* CORRIGEER REALISATIETIJDEN OP BASIS VAN GARANTIETIJDEN */
/* ------------------------------------------------------ */
/* void CorrigeerRealisatieTijdenObvGarantieTijden(void) corrigeert de REALISATIETIJD[][] indien de fasecyclus zelf wordt afgewikkeld (realisatietijd naar zichzelf).
 * een fasecyclus mag na groenrealisatie niet direct weer realiseren; eerst geel (GL[]) en garantierood (TRG[]).
 * de waarde wordt ingevuld in de realisatietijd naar de fasecylus zelf; REALISATIETIJD[fc1][fc1].
 *
 * CorrigeerRealisatieTijdenObvGarantieTijden(void) wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na InitRealisatieTijden(),
 * RealisatieTijden_VulHardeConflictenIn() en RealisatieTijden_VulGroenGroenConflictenIn().
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door de applicatiefunctie application().
 *
 *  //Realisatietijden
 *  InitRealisatieTijden();
 *  RealisatieTijden_VulHardeConflictenIn();
 *  RealisatieTijden_VulGroenGroenConflictenIn();  
 *  CorrigeerRealisatieTijdenObvGarantieTijden();  // een richting mag na groen niet direct weer realiseren (eerst GL en TRG)
 *
 */

void CorrigeerRealisatieTijdenObvGarantieTijden(void)
{
    count fc1;

    for (fc1 = 0; fc1 < FCMAX; ++fc1)
    {
        REALISATIETIJD[fc1][fc1] = GL[fc1] ? (TGL_max[fc1] - TGL_timer[fc1] + TRG_max[fc1]) : TRG[fc1] ? TRG_max[fc1] - TRG_timer[fc1] : 0;
        if ((REALISATIETIJD[fc1][fc1] <= 0) && RV[fc1]) REALISATIETIJD[fc1][fc1] = 1; // De waarde is 1 omdat de richting altijd nog door RA moet
    }
}

/* REALISATIETIJD NALOOP OP EINDE GROEN (NLEG) */
/* ------------------------------------------- */
/* void Realisatietijd_NLEG(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop) bij een naloop EG.
 * voor de harde en groen-groen conflicten van de volgrichting/nalooprichting worden de REALISATIETIJD[][]-en bepaald t.o.v. de voedende richting REALISATIETIJD[i][k[],
 * dit zijn vaak fictieve conflicten van elkaar.
 *
 *  i ---->    j --->       i: index-voedende richting en j: index-nalooprichting
 *                     ^
 *                     |
 *                     k  - index van conflict van de nalooprichting
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * i - index fasecyclus van de voedende richting
 * j - index fasecyclus van de nalooprichting
 * tnlfg -  index tijdelement - Vaste nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                              rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlfgd - index tijdelement - Detectie afhankelijke nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                              rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnleg -  index tijdelement - Vaste nalooptijd voor voertuig(en) tijdens groen (G[fc1]) van de voedende richting;
 *                              rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlegd - index tijdelement - Detectie afhankelijke nalooptijd voor voertuig(en) tijdens groen of geel (G[fc1] || GL[fc1]) van de voedende richting;
 *                              rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tvgnaloop - index T_max[] - Maximale verlengtijd op eigen detectie na aflopen nalooptijden voor de naloop richting.
 *
 * void Realisatietijd_NLEG() wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na de initialisatie en vullen van de REALISATIETIJD[][]-en.
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door de applicatiefunctie application().
 *
 *  //Realisatietijden
 *  InitRealisatieTijden();
 *  RealisatieTijden_VulHardeConflictenIn();
 *  RealisatieTijden_VulGroenGroenConflictenIn();  
 *  CorrigeerRealisatieTijdenObvGarantieTijden();  // een richting mag na groen niet direct weer realiseren (eerst GL en TRG)
 *
 *  //Pas Realisatietijden aan voor nalopenEG
 *  Realisatietijd_NLEG(fc02, fc62, tnlfg0262, tnlfgd0262, tnleg0262, tnlegd0262, tvgnaloop0262);
 *  Realisatietijd_NLEG(fc08, fc68, tnlfg0868, tnlfgd0868, tnleg0868, tnlegd0868, tvgnaloop0868);
 *  Realisatietijd_NLEG(fc11, fc68, tnlfg1168, tnlfgd1168, tnleg1168, tnlegd1168, tvgnaloop1168);
 *  Realisatietijd_NLEG(fc22, fc21, tnlfg2221, tnlfgd2221, tnleg2221, tnlegd2221, tvgnaloop2221);
 *  Realisatietijd_NLEG(fc82, fc81, tnlfg8281, tnlfgd8281, tnleg8281, tnlegd8281, tvgnaloop8281);
 *
 */


void Realisatietijd_NLEG(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++) /* doorloop de conflicten van de naloop richting */
    {
        k = KF_pointer[j][n]; /* bepaal de index van de conflicterende fasecyclus */
        if (!AR[i])
        {
            if (CV[i] && !RA[i])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfgd] - (FG[i] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnleg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + (((TVG_max[i] - TVG_timer[i]) > 0) ? (TVG_max[i] - TVG_timer[i]) : 0) + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlegd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + (((TVG_max[i] - TVG_timer[i]) > 0) ? (TVG_max[i] - TVG_timer[i]) : 0) + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
            }
            else if (TIG[j][k])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlfgd] - T_timer[tnlfgd] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnleg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnleg] - T_timer[tnleg] + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
                if (!(tnlegd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TGL_max[i] - TGL_timer[i] + T_max[tnlegd] - ((G[i] || GL[i]) ? 0 : T_timer[tnlegd]) + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
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
                if (!(tnlegd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TGL_max[i] - TGL_timer[i] + T_max[tnlegd] - ((G[i] || GL[i]) ? 0 : T_timer[tnlegd]) + T_max[tvgnaloop] - T_timer[tvgnaloop] + TIG_max[j][k] - TIG_timer[j]);
            }
        }
    }
    for (n = KFC_MAX[j]; n < GKFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n]; /* bepaal de index van de conflicterende fasecyclus */
        if (!AR[i])
        {
            if (CV[i] && !RA[i])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfgd] - (FG[i] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop]);
                if (!(tnleg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + (((TVG_max[i] - TVG_timer[i]) > 0) ? (TVG_max[i] - TVG_timer[i]) : 0) + T_max[tnleg] + T_max[tvgnaloop]);
                if (!(tnlegd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + (((TVG_max[i] - TVG_timer[i]) > 0) ? (TVG_max[i] - TVG_timer[i]) : 0) + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop]);
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
                if (!(tnlegd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TGL_max[i] - TGL_timer[i] + T_max[tnlegd] - ((G[i] || GL[i]) ? 0 : T_timer[tnlegd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);
            }
        }
    }
}

/* REALISATIETIJD NALOOP OP EINDE VERLENGGROEN (NLEVG) */
/* --------------------------------------------------- */
/* void Realisatietijd_NLEVG(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop) bepaalt de REALISATIETIJD[][] bij een NaloopEVG
 * voor de harde en groen-groen conflicten van de volgrichting/nalooprichting worden de REALISATIETIJD[][]-en bepaald t.o.v. de voedende richting REALISATIETIJD[i][k[],
 *  dit zijn vaak fictieve conflicten van elkaar.
 *
 *  i ---->    j --->        i: index-voedende richting en j: index-nalooprichting
 *                     ^
 *                     |
 *                     k  - index van conflict van de nalooprichting
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * i - index fasecyclus van de voedende richting
 * j - index fasecyclus van de nalooprichting
 *
 * tnlfg     - index tijdelement - Vaste nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlfgd    - index tijdelement - Detectie afhankelijke nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlevg    - index tijdelement - Vaste nalooptijd voor voertuig(en) tijdens groen en geen meeverlenggroen (G[fc1] && !MG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlevgd   - index tijdelement - Detectie afhankelijke nalooptijd voor voertuig(en) tijdens groen en geen meeverlenggroen (G[fc1] && !MG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tvgnaloop - index T_max[] - Maximale verlengtijd op eigen detectie na aflopen nalooptijden voor de naloop richting.
 *
 * void Realisatietijd_NLEVG() wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na de initialisatie en vullen van de REALISATIETIJD[][]-en
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door de applicatiefunctie application().
 *
 *  //Realisatietijden
 *  InitRealisatieTijden();
 *  RealisatieTijden_VulHardeConflictenIn();
 *  RealisatieTijden_VulGroenGroenConflictenIn(); 
 *  CorrigeerRealisatieTijdenObvGarantieTijden(); // een richting mag na groen niet direct weer realiseren (eerst GL en TRG)
 *
 *  //Pas Realisatietijden aan voor nalopenEG
 *  Realisatietijd_NLEG(fc02, fc62, tnlfg0262, tnlfgd0262, tnleg0262, tnlegd0262, tvgnaloop0262);  
 *  Realisatietijd_NLEG(fc08, fc68, tnlfg0868, tnlfgd0868, tnleg0868, tnlegd0868, tvgnaloop0868);
 *  Realisatietijd_NLEG(fc11, fc68, tnlfg1168, tnlfgd1168, tnleg1168, tnlegd1168, tvgnaloop1168);
 *  Realisatietijd_NLEG(fc22, fc21, tnlfg2221, tnlfgd2221, tnleg2221, tnlegd2221, tvgnaloop2221);
 *  Realisatietijd_NLEG(fc82, fc81, tnlfg8281, tnlfgd8281, tnleg8281, tnlegd8281, tvgnaloop8281);
 *
 */

void Realisatietijd_NLEVG(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++) /* doorloop de conflicten van de nalooprichting */
    {
        k = KF_pointer[j][n]; /* bepaal de index van de conflicterende fasecyclus */
        if (!AR[i])
        {
            if (CV[i] && !RA[i])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfgd] - (FG[i] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlevg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + (((TVG_max[i] - TVG_timer[i]) > 0) ? (TVG_max[i] - TVG_timer[i]) : 0) + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
                if (!(tnlevgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + (((TVG_max[i] - TVG_timer[i]) > 0) ? (TVG_max[i] - TVG_timer[i]) : 0) + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
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
    for (n = KFC_MAX[j]; n < GKFC_MAX[j]; n++)  /* doorloop de conflicten van de nalooprichting */
    {
        k = KF_pointer[j][n]; /* bepaal de index van de conflicterende fasecyclus */
        if (!AR[i])
        {
            if (CV[i] && !RA[i])
            {
                if (!(tnlfg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop]);
                if (!(tnlfgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + T_max[tnlfgd] - (FG[i] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop]);
                if (!(tnlevg == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + (((TVG_max[i] - TVG_timer[i]) > 0) ? (TVG_max[i] - TVG_timer[i]) : 0) + T_max[tnlevg] + T_max[tvgnaloop]);
                if (!(tnlevgd == NG)) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], TFG_max[i] - TFG_timer[i] + (((TVG_max[i] - TVG_timer[i]) > 0) ? (TVG_max[i] - TVG_timer[i]) : 0) + T_max[tnlevgd] + T_max[tvgnaloop]);
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

/* REALISATIETIJD NALOOP OP STARTGROEN (NLSG) */
/* ------------------------------------------ */
/* void Realisatietijd_NLSG(count i, count j, count tnlsg, count tnlsgd) bepaalt de REALISATIETIJD[][] bij een NaloopSG. 
 * voor de harde en groen-groen conflicten van de volgrichting/nalooprichting worden de REALISATIETIJD[][]-en bepaald t.o.v. de voedende richting REALISATIETIJD[i][k[] - dit zijn vaak fictieve conflicten van elkaar.
 *
 *  i ---->    j --->       i: index-voedende richting en j: index-nalooprichting
 *                     ^
 *                     |
 *                     k  - index van conflict van de nalooprichting
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * i - index fasecyclus van de voedende richting
 * j - index fasecyclus van de nalooprichting
 * tnlsg -  index tijdelement - vaste nalooptijd voor (eerste) voetganger vanaf startgroen (SG[fc1]) van de voedende richting; NG indien niet gebruikt.
 * tnlsgd - index tijdelement - detectie aanvraag afhankelijke nalooptijd voor (eerste) voetganger vanaf startgroen (SG[fc1]) van de voedende richting;
 *                              NG indien niet gebruikt
 *
 * void Realisatietijd_NLSG() wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na de initialisatie en vullen van de REALISATIETIJD[][]-en
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door de applicatiefunctie application().
 *
 *  //Realisatietijden
 *  InitRealisatieTijden();
 *  RealisatieTijden_VulHardeConflictenIn();
 *  RealisatieTijden_VulGroenGroenConflictenIn(); 
 *  CorrigeerRealisatieTijdenObvGarantieTijden(); // een richting mag na groen niet direct weer realiseren (eerst GL en TRG)
 *
 *  // Pas Realisatietijden aan voor nalopenEG
 *  Realisatietijd_NLEG(fc02, fc62, tnlfg0262, tnlfgd0262, tnleg0262, tnlegd0262, tvgnaloop0262);
 *  Realisatietijd_NLEG(fc08, fc68, tnlfg0868, tnlfgd0868, tnleg0868, tnlegd0868, tvgnaloop0868);
 *  Realisatietijd_NLEG(fc11, fc68, tnlfg1168, tnlfgd1168, tnleg1168, tnlegd1168, tvgnaloop1168);
 *  Realisatietijd_NLEG(fc22, fc21, tnlfg2221, tnlfgd2221, tnleg2221, tnlegd2221, tvgnaloop2221);
 *  Realisatietijd_NLEG(fc82, fc81, tnlfg8281, tnlfgd8281, tnleg8281, tnlegd8281, tvgnaloop8281);
 *
 *  // Pas Realisatietijden aan voor nalopenSG
 *  Realisatietijd_NLSG(fc31, fc32, tnlsg3132, tnlsgd3132);
 *  Realisatietijd_NLSG(fc32, fc31, tnlsg3231, tnlsgd3231);
 *  Realisatietijd_NLSG(fc33, fc34, NG,        tnlsgd3334);
 *  Realisatietijd_NLSG(fc34, fc33, NG,        tnlsgd3433);
 *
 */

void Realisatietijd_NLSG(count i, count j, count tnlsg, count tnlsgd)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++) /* doorloop de conflicten van de nalooprichting */
    {
        k = KF_pointer[j][n]; /* bepaal de index van de conflicterende fasecyclus */
        if (VS[i])
        {
            if (!(tnlsg == NG) && (RT[tnlsg] || T[tnlsg])) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlsg] + TIG_max[j][k]);
            if (!(tnlsgd == NG) && (RT[tnlsgd] || T[tnlsgd])) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlsgd] + TIG_max[j][k]);
        }
        else
        {
            if (!(tnlsg == NG) && T[tnlsg]) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlsg] - T_timer[tnlsg] + TIG_max[j][k]);
            if (!(tnlsgd == NG) && T[tnlsgd]) REALISATIETIJD[i][k] = max(REALISATIETIJD[i][k], T_max[tnlsgd] - T_timer[tnlsgd] + TIG_max[j][k]);
        }
    }
}

/* REALISATIETIJD BIJ HARD MEEVERLENGEN MET EEN DEELCONFLICT */
/* --------------------------------------------------------- */
/* void Realisatietijd_HardMeeverlengenDeelconflict(count fc1, count fc2)  bepaalt de REALISATIETIJD[][] bij toepassing van hard meeverlengen
 * van een deelconflict richting.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus van de richting die hard meeverlengt
 * fc2 - index fasecyclus van de richting
 *
 * void Realisatietijd_HardMeeverlengenDeelconflict() wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void)
 * na de initialisatie en vullen van de REALISATIETIJD[][]-en BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 *
 *  //Pas Realisatietijden aan voor hard meeverlengen deelconflict
 *  Realisatietijd_HardMeeverlengenDeelconflict(fc05, fc22);
 *  Realisatietijd_HardMeeverlengenDeelconflict(fc11, fc26);
 *  Realisatietijd_HardMeeverlengenDeelconflict(fc05, fc32);
 */


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
                    REALISATIETIJD[fc1][fc] = max(REALISATIETIJD[fc1][fc], TFG_max[fc1] - TFG_timer[fc1] + ((((TVG_max[fc1] - TVG_timer[fc1]) && !MG[fc1]) > 0) ? (TVG_max[fc1] - TVG_timer[fc1]) : 0) + TIGR[fc2][fc]);
                }
                else
                {
                    REALISATIETIJD[fc1][fc] = max(REALISATIETIJD[fc1][fc], TFG_max[fc1] - TFG_timer[fc1] + TVG_AR[fc1] - (((TVG_AR[fc1] > TVG_timer[fc1]) && !MG[fc1]) ? TVG_timer[fc1] : 0) + TIGR[fc2][fc]);
                }
            }
        }
    }
}

/* REALISATIETIJD ONTRUIMING VOORSTART */
/* ----------------------------------- */
/* void Realisatietijd_Ontruiming_Voorstart(count fcns, count fcvs, count tfo) bepaalt de REALISATIETIJD[][] bij (fictief) ontruimende deelconflicten met een voorstart;
 * REALISATIETIJD[fcns][fcvs] wordt zonodig aangepast.
 * Realisatietijd_Ontruiming_Voorstart() herstart de fictieve ontruimingstijd.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fcns - index fasecyclus die als laatste start
 * fcvs - index fasecyclus die als eerste start
 * tfo - index tijdelement fictieve ontruimingstijd
 *
 * void Realisatietijd_Ontruiming_Voorstart() wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na de initialisatie van de REALISATIETIJD[][]-en.
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 *
 *  // Pas realisatietijden aan voor (fictieve) ontruiming deelconflicten met Voorstart
 *  Realisatietijd_Ontruiming_Voorstart(fc05, fc22, tfo0522);
 *  Realisatietijd_Ontruiming_Voorstart(fc05, fc32, tfo0532);
 */


void Realisatietijd_Ontruiming_Voorstart(count fcns, count fcvs, count tfo)
{
    RT[tfo] = G[fcns]; /* hertart de fictieve ontruimingstijd */
    if (T[tfo] && !G[fcvs])
    {
        REALISATIETIJD[fcns][fcvs] = TFG_max[fcns] - TFG_timer[fcns] + (((TVG_max[fcns] - TVG_timer[fcns]) > 0) ? (TVG_max[fcns] - TVG_timer[fcns]) : 0) + T_max[tfo] - T_timer[tfo];
    }
}

/* REALISATIETIJD ONTRUIMING GELIJKSTART */
/* ------------------------------------- */
/* void Realisatietijd_Ontruiming_Gelijkstart(count fc1, count fc2, count tfo12, count tfo21) bepaalt de REALISATIETIJD[][] bij (fictief) ontruimende deelconflicten
 * met een gelijkstart; REALISATIETIJD[fc1][fc2] en REALISATIETIJD[fc2][fc1] worden zonodig aangepast.
 * Realisatietijd_Ontruiming_Gelijkstart() herstart de fictieve ontruimingstijden.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus deelconflict
 * fc2 - index fasecyclus deelconflict
 * tfo12 - index tijdelement fictieve ontruimingstijd
 * tfo21 - index tijdelement fictieve ontruimingstijd
 *
 * void Realisatietijd_Ontruiming_Gelijkstart() wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na de initialisatie van de REALISATIETIJD[][]-en.
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 *
 *  //Pas realisatietijden aan voor (fictieve) ontruiming deelconflicten met Gelijkstart
 *  Realisatietijd_Ontruiming_Voorstart(fc05, fc11, tfo0511, tfo1105);
 */

void Realisatietijd_Ontruiming_Gelijkstart(count fc1, count fc2, count tfo12, count tfo21)
{
    RT[tfo12] = G[fc1]; /* hertart de fictieve ontruimingstijd */
    if (T[tfo12] && !G[fc2])
    {
        REALISATIETIJD[fc2][fc1] = TFG_max[fc1] - TFG_timer[fc1] + (((TVG_max[fc1] - TVG_timer[fc1]) > 0) ? (TVG_max[fc1] - TVG_timer[fc1]) : 0) + T_max[tfo12] - T_timer[tfo12];
    }
    RT[tfo21] = G[fc2]; /* hertart de fictieve ontruimingstijd */
    if (T[tfo21] && !G[fc1])
    {
        REALISATIETIJD[fc1][fc2] = TFG_max[fc2] - TFG_timer[fc2] + (((TVG_max[fc1] - TVG_timer[fc1]) > 0) ? (TVG_max[fc1] - TVG_timer[fc1]) : 0) + T_max[tfo21] - T_timer[tfo21];
    }
}

/* REALISATIETIJD ONTRUIMING LATERELEASE */
/* ------------------------------------- */
/* void Realisatietijd_Ontruiming_LateRelease(count fcvs, count fclr, count tlr, count tfo) bepaalt de REALISATIETIJD[][] bij (fictief) ontruimende deelconflicten
 * met een LateRelease; REALISATIETIJD[fclr][fcvs] wordt zonodig aangepast.
 * Realisatietijd_Ontruiming_LateRelease() herstart de fictieve ontruimingstijden.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fcvs  - index fasecyclus deelconflict die als eerste start
 * fclr  - index fasecyclus deelconflict die als laatste start met LateRelease
 * tlr - index tijdelement LateRelease
 * tfo - index tijdelement fictieve ontruimingstijd
 *
 * void Realisatietijd__Ontruiming_LateRelease() wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na de initialisatie van de REALISATIETIJD[][]-en.
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 *
 *  //Pas realisatietijden aan voor (fictieve) ontruiming deelconflicten met LateRelease
 *  Realisatietijd_Ontruiming_LateRelease(fc11, fc26, tlr2611, tfo2611);
 */

void Realisatietijd_Ontruiming_LateRelease(count fcvs, count fclr, count tlr, count tfo)
{
    RT[tfo] = G[fcvs] && (TG_timer[fcvs] > T_max[tlr]); /* hertart de fictieve ontruimingstijd */
    if (T[tfo] && !G[fclr])
    {
        REALISATIETIJD[fclr][fcvs] = TFG_max[fcvs] - TFG_timer[fcvs] + (((TVG_max[fcvs] - TVG_timer[fcvs]) > 0) ? (TVG_max[fcvs] - TVG_timer[fcvs]) : 0) + T_max[tfo] - T_timer[tfo];
    }
}


/* REALISATIETIJD VOORSTART CORRECTIE */
/* ---------------------------------- */
/* bool Realisatietijd_Voorstart_Correctie(count fcvs, count fcns, count tvs) corrigeert de REALISATIETIJD[][] bij deelconflicten met een Voorstart. //@PSN andere volgorde fcns en fcvs bij void Realisatietijd_Ontruiming_Voorstart()
 * Realisatietijd_Voorstart_Correctie() vergelijkt alle REALISATIETIJD[n][fcvs] met REALISATIETIJD[n][fcns] en past indien nodig REALISATIETIJD[n][fcns] aan,
 * en/of past REALISATIETIJD[fcvs][fcns] aan.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fcvs - index fasecyclus die als eerste start
 * fcns - index fasecyclus die als laatste start
 * tvs - index tijdelement voorstarttijd
 *
 * void Realisatietijd_Voorstart_Correctie() wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na de initialisatie van de REALISATIETIJD[][]-en.
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 *
 *  // Pas realisatietijden aan voor deelconflicten met Voorstart die nog groen moeten worden   //@PSN Deze correcties zouden toch ook in TIGR[][] kunnen worden geplaatst.
 *   do
 *   {
 *       wijziging = FALSE;
 *
 *       // Voorstart deelconflicten
 *        wijziging |= Realisatietijd_Voorstart_Correctie(fc22, fc05, tvs2205);
 *        wijziging |= Realisatietijd_Voorstart_Correctie(fc32, fc05, tvs3205);
 *
 *        wijziging |= CorrectieRealisatieTijd_Add();
 *
 *   } while (wijziging);  //@PSN Let op! functie zou in theorie oneindig kunnen doorgaan!!
 */

bool Realisatietijd_Voorstart_Correctie(count fcvs, count fcns, count tvs)
{
    count n;
    bool result = FALSE;
    if ((A[fcvs] || !(PG[fcvs] & PRIMAIR_OVERSLAG) || TRUE) && !G[fcvs])  //@PSN || TRUE is altijd waar; //@@## warning C4127: conditional expression is constant
    {
        for (n = 0; n < FCMAX; ++n)
        {
            if ((REALISATIETIJD[n][fcns] < (REALISATIETIJD[n][fcvs] + T_max[tvs])) && REALISATIETIJD[n][fcvs] > 0)  
            {                                                                                                     /* @PSN is testen op REALISATIETIJD[n][fcvs] > 0 wel goed? >=0??  extra haakjes (REALISATIETIJD[n][fcvs] > 0) */
               REALISATIETIJD[n][fcns] = REALISATIETIJD[n][fcvs] + T_max[tvs];                                    /* @PSN bij REALISATIETIJD[n][fcvs] == 0 moet toch worden verhoogd met T_max[tvs]; */
                result = TRUE;
            }
        }
    }
    if (G[fcvs] || RA[fcvs] && (REALISATIETIJD_max[fcvs] <= 1)) //@PSN G[fcvs] er wordt toch ook tijdens groen aangepast!
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

/* REALISATIETIJD GELIJKSTART CORRECTIE */
/* ------------------------------------ */
/* bool Realisatietijd_Gelijkstart_Correctie(count fc1, count fc2) corrigeert zonodig de REALISATIETIJD[][] bij deelconflicten met een gelijkstart.
 * de realisatietijden REALISATIETIJD[n][fc1] en REALISATIETIJD[n][fc2] moeten gelijk aan elkaar zijn (grootste is maatgevend).
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index eerste fasecyclus
 * fc2 - index tweede fasecyclus
 *
 * void Realisatietijd_Gelijkstart_Correctie() wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na de initialisatie van de REALISATIETIJD[][]-en.
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 *
 *  // Pas realisatietijden aan voor deelconflicten met Gelijkstart die nog groen moeten worden
 *   do
 *   {
 *       wijziging = FALSE;
 *
 *       // Gelijkstart deelconflicten
 *       wijziging |= Realisatietijd_Gelijkstart_Correctie(fc05, fc11);
 *
 *       wijziging |= CorrectieRealisatieTijd_Add();
 *
 *   } while (wijziging);  //@PSN Let op! functie zou in theorie oneindig kunnen doorgaan!!
 */


bool Realisatietijd_Gelijkstart_Correctie(count fc1, count fc2)
{
    count n;
    bool result = FALSE;
    // @PSN TODO vergt uitwerking indien een van de twee gelijkstartende richtingen niet komt
    // if ((A[fc1] || !(PG[fc1] & PRIMAIR_OVERSLAG)) && (A[fc2] || !(PG[fc2] & PRIMAIR_OVERSLAG)) && !G[fc1] && !G[fc2])
    // {
    for (n = 0; n < FCMAX; ++n) 
    {
        if (REALISATIETIJD[n][fc1] < REALISATIETIJD[n][fc2])
        {
            REALISATIETIJD[n][fc1] = REALISATIETIJD[n][fc2];
            result = TRUE;
        }
        else
        {
            if (REALISATIETIJD[n][fc1] != REALISATIETIJD[n][fc2])   /* @PSN REALISATIETIJD[n][fc1] > REALISATIETIJD[n][fc2] */
            {
                REALISATIETIJD[n][fc2] = REALISATIETIJD[n][fc1];
                result = TRUE;
            }
            // }
        }
    }
    return result;
}

/* REALISATIETIJD LATERELEASE CORRECTIE */
/* ------------------------------------ */
/* bool Realisatietijd_LateRelease_Correctie(count fclr, count fcvs, count tlr) corrigeert de REALISATIETIJD[][] bij deelconflicten met een LateReleas. //@@## andere volgorde fclr en fcvs bij void Realisatietijd_Ontruiming_LateRelease()
 * Realisatietijd__LateRelease_Correctie() vergelijkt alle REALISATIETIJD[n][fcvs] met REALISATIETIJD[n][fclr] en past indien nodig REALISATIETIJD[n][fcvs] aan,
 * en/of past REALISATIETIJD[fcvs][fcns] aan.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fclr  - index fasecyclus deelconflict die als laatste start met LateRelease
 * fcvs  - index fasecyclus deelconflict die als eerste start
 * tlr - index tijdelement LateRelease
 *
 * bool Realisatietijd_LateRelease_Correctie() wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na de initialisatie van de REALISATIETIJD[][]-en.
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 *
 *  // Pas realisatietijden aan voor deelconflicten met LateRelease die nog groen moeten worden
 *   do
 *   {
 *       wijziging = FALSE;
 *
 *      // Inlopen / inrijden nalopen
 *      wijziging |= Realisatietijd_LateRelease_Correctie(fc62, fc02, txnl0262);
 *      wijziging |= Realisatietijd_LateRelease_Correctie(fc68, fc08, txnl0868);
 *      wijziging |= Realisatietijd_LateRelease_Correctie(fc68, fc11, txnl1168);
 *      wijziging |= Realisatietijd_LateRelease_Correctie(fc21, fc22, txnl2221);
 *      wijziging |= Realisatietijd_LateRelease_Correctie(fc32, fc31, txnl3132);
 *      wijziging |= Realisatietijd_LateRelease_Correctie(fc31, fc32, txnl3231);
 *      wijziging |= Realisatietijd_LateRelease_Correctie(fc34, fc33, txnl3334);
 *      wijziging |= Realisatietijd_LateRelease_Correctie(fc33, fc34, txnl3433);
 *      wijziging |= Realisatietijd_LateRelease_Correctie(fc81, fc82, txnl8281);
 *
 *      wijziging |= CorrectieRealisatieTijd_Add();
 *
 *   } while (wijziging);  //@PSN Let op! functie zou in theorie oneindig kunnen doorgaan!!
 */

bool Realisatietijd_LateRelease_Correctie(count fcvs, count fclr, count tlr)
{
    count n;
    bool result = FALSE;
    if (A[fclr] || !PG[fclr] || TRUE)   //@PSN || TRUE is altijd waar; warning C4127: conditional expression is constant
    {
        if (!G[fcvs])
        {
            for (n = 0; n < FCMAX; ++n)
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

/* Deze functie is gelijk aan Realisatietijd_LateRelease_Correctie (zie hierboven) maar kijkt naar de maximale tijd.
 */

bool Realisatietijd_LateRelease_Correctie_wtv(count fcvs, count fclr, count tlr)
{
    count n;
    bool result = FALSE;
    if (A[fclr] || !PG[fclr] || TRUE)   //@PSN || TRUE is altijd waar; warning C4127: conditional expression is constant
    {
        if (!G[fcvs])
        {
            for (n = 0; n < FCMAX; ++n)
            {
                if (REALISATIETIJD_wtv[n][fcvs] < REALISATIETIJD_wtv[n][fclr] - T_max[tlr])
                {
                    REALISATIETIJD_wtv[n][fcvs] = REALISATIETIJD_wtv[n][fclr] - T_max[tlr];
                    if (REALISATIETIJD_wtv[n][fcvs] < 0) REALISATIETIJD_wtv[n][fcvs] = 0;
                    result = TRUE;
                }
            }
        }
    }
    return result;
}

/* BEPAAL REALISATIETIJD VOOR RICHTING */
/* ----------------------------------- */
/* void Bepaal_Realisatietijd_voor_richting(count i) bepaalt de waarde van REALISATIETIJD_max[], de grootste Realisatietijd in REALISATIETIJD[][] is bepalend.
 *
 * void Bepaal_Realisatietijd_voor_richting(i) wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na de initialisatie en correctie
 * van de de REALISATIETIJD[][]-en.
 * 
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 */

void Bepaal_Realisatietijd_voor_richting(count i)
{
   int j;
   REALISATIETIJD_max[i] = 0;/* @PSN moet de intiele waarde niet NG zijn? */
   REALISATIETIJD_max_wtv[i] = 0;/* @PSN moet de intiele waarde niet NG zijn? */ 
   for (j = 0; j < FCMAX; ++j) /* zoek de hoogste waarde voor de realisatietijd */
   {
       if (REALISATIETIJD_max[i] < REALISATIETIJD[j][i]) REALISATIETIJD_max[i] = REALISATIETIJD[j][i]; 
       if (REALISATIETIJD_max_wtv[i] < REALISATIETIJD_wtv[j][i]) REALISATIETIJD_max_wtv[i] = REALISATIETIJD_wtv[j][i]; 
   }
}

/* BEPAAL REALISATIETIJD VOOR ALLE RICHTINGEN */
/* ------------------------------------------ */
/* void Bepaal_Realisatietijd_alle_richtingen() bepaalt de waarde van REALISATIETIJD_max[] voor alle richtingen.
 *
 * void Bepaal_Realisatietijd_alle_richtingen() wordt aangeroepen vanuit de applicatiefunctie void BepaalRealisatieTijden(void) na de initialisatie en correctie
 * van de de REALISATIETIJD[][]-en.
 * 
 * BepaalRealisatieTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 *
 */

void Bepaal_Realisatietijd_alle_richtingen()
{
   count i;
   for (i = 0; i < FCMAX; ++i)
   {
      Bepaal_Realisatietijd_voor_richting(i);
   }
}

/* MEEVERLENGEN - YM_MAX_TIG_REALISATIETIJD */
/* ---------------------------------------- */
/* bool ym_max_tig_Realisatietijd(count i, count prmomx) is een aangepaste ym_max_tig() voor de specificatie van het meeverlengen (YM[]) die gebruikt maakt
 * van de REALISATIETIJD[][] en REALISATIETIJD_max[].
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * i      - index fasecyclus voor meeverlengen
 * prmomx - index parameter voor maximaal ontruimingsverschil
 *
 * ym_max_tig_Realisatietijd() wordt aangeroepen vanuit de applicatiefunctie Meeverlengen().
 *
 * void Meeverlengen(void)
 * {
 *    int fc; //@PSN int wijzigen in count.
 *
 *    for (fc = 0; fc < FC_MAX; ++fc)
 *    {
 *       YM[fc] &= ~BIT1;  // reset BIT-sturing
 *       YM[fc] &= ~BIT4;  // reset BIT-sturing
 *    }
 *
 *    YM[fc02] |= SCH[schmv02] && ym_max_tig_Realisatietijd(fc02, prmmv02) && hf_wsg_nlISG() ? BIT4 : 0;
 *    YM[fc03] |= SCH[schmv03] && ym_max_tig_Realisatietijd(fc03, prmmv03) && hf_wsg_nlISG() ? BIT4 : 0;
 *    YM[fc05] |= SCH[schmv05] && ym_max_tig_Realisatietijd(fc05, prmmv05) && hf_wsg_nlISG() ? BIT4 : 0;
 *    YM[fc08] |= SCH[schmv08] && ym_max_tig_Realisatietijd(fc08, prmmv08) && hf_wsg_nlISG() ? BIT4 : 0;
 *    YM[fc09] |= SCH[schmv09] && ym_max_tig_Realisatietijd(fc09, prmmv09) && hf_wsg_nlISG() ? BIT4 : 0;
 *    YM[fc11] |= SCH[schmv11] && ym_max_tig_Realisatietijd(fc11, prmmv11) && hf_wsg_nlISG() ? BIT4 : 0;
 * }
 *
 */

bool ym_max_tig_Realisatietijd(count i, count prmomx) /* @PSN: todo fc22 moet met 5 meeverlengen als 11 groen is. */
{
    register count n, j, k, m;
    bool ym;
    if (prmomx == NG) prmomx = 0;
    if (MG[i])
    {     /* let op! i.v.m. snelheid alleen in MG[] behandeld	*/
        ym = TRUE;
        for (n = 0; n < FKFC_MAX[i]; ++n)
        {
            k = KF_pointer[i][n]; /* bepaal de index van de conflicterende fasecyclus */
            if ((RA[k] || AAPR[k]) && !(FK_type[i][k] == FK_SG) && !(FK_type[i][k] == FK_EVG) && (REALISATIETIJD_max[k] >= 0))
            {
                ym = FALSE;
                for (j = 0; j < FKFC_MAX[k]; j++)
                {
                    m = KF_pointer[k][j]; /* bepaal de index van de fictief conflicterende fasecyclus */
                    if ((REALISATIETIJD[m][k] > REALISATIETIJD[i][k] + prmomx) && !(TNL_type[m][i] == TNL_EG))
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

/* TEGENHOUDEN DOOR REALISATIETIJDEN */
/* --------------------------------- */
/* void TegenhoudenDoorRealisatietijden(void) wordt gebruikt voor het tegenhouden van de groenrealisatie van de richtingen m.b.v de instructies
 * 'tegenhouden groenrealisatie' (X[]-BIT1) en 'herstart rood voor aanvraag' (RR[]-BIT1)
 * op basis van de waarde(n) van de REALISATIETIJD[][].
 *
 * TegenhoudenDoorRealisatietijden() wordt aangeroepen vanuit de applicatiefunctie Synchronisaties().
 * Synchronisaties(d) wordt aangeroepen vanuit de applicatiefunctie application()
 *
 * void Synchronisaties(void)
 *  {
 *     TegenhoudenDoorRealisatietijden();
 *
 *     Synchronisaties_Add();
 * }
 */


void TegenhoudenDoorRealisatietijden()
{
    count i, j;

    for (i = 0; i < FCMAX; ++i)
    {
        X[i] &= ~BIT1;   /* reset instructie BIT1 van X[i]   */
        RR[i] &= ~BIT1;  /* reset instructie BIT1 van RR[i]  */
    }
    for (i = 0; i < FCMAX; ++i)
    {
        for (j = 0; j < FCMAX; ++j)
        {
            if (REALISATIETIJD[i][j] > 0) X[j] |= BIT1; /* Als er een realisatietijd loopt van (fictief) conflict i, wordt richting j nog tegengehouden */
            if (REALISATIETIJD[i][j] > 150) RR[j] |= BIT1; /* @PSN 150 tijdelijk moet afhankelijk gemaakt worden van de tijd die een richting eerder mag starten dan de volgrichting */
        }
    }
}


/* INITIALISATIE INTERSTARTGROENTIJDEN */
/* ----------------------------------- */
/* void InitInterStartGroenTijden(void) bepaalt de initiele waarden van de InterStartGroenTijden matrix TISG_PR[][] en TISG_AR[][] en zet alle waarden op NG (niet gebruikt).
 * de TISG_PR[][] en TISG_AR[][] dienen in de regelapplicatie te worden bepaald en gecorrigeerd voor o.a naloop richtingen //@@@@ verder aanvullen
 *
 * In een TLCGen-regelapplicatie wordt InitInterStartGroenTijden(void) aangeroepen vanuit de applicatiefunctie void BepaalInterStartGroenTijden(void).
 * BepaalInterStartGroenTijden() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door application().
 */

void InitInterStartGroenTijden(void)
{
    count i, j;
    for (i = 0; i < FC_MAX; i++)  /* zet alle GK en GKL conflicten om in FK */
    {
        for (j = 0; j < FC_MAX; j++)
        {
            TISG_PR[i][j] = NG;
            TISG_AR[i][j] = NG;
            TISG_AR_los[i][j] = NG;
        }
    }
}

/* INTERSTARTGROENTIJDEN VUL HARDE CONFLICTEN IN */
/* --------------------------------------------- */
/* void InterStartGroenTijden_VulHardeConflictenIn(void) vult de TISG_PR[fc1][fc2] en TISG_AR[fc1][fc2] in voor alle harde conflicten met de vastgroentijden, verlengroentijden
 * en intergroentijden.
 *
 * InterStartGroenTijden_VulHardeConflictenIn(void(void) wordt aangeroepen vanuit de applicatiefunctie void BepaalInterStartGroenTijden(void).
 * na de initialisatie van TISG_PR[][] en TISG_AR[][].
 * BepaalInterStartGroenTijden(void) wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door application().
 *
 * // Bepaal InterStartGroenTijden
 * InitInterStartGroenTijden();
 * InterStartGroenTijden_VulHardeConflictenIn();
 * InterStartGroenTijden_VulGroenGroenConflictenIn();
 *
 */


void InterStartGroenTijden_VulHardeConflictenIn(void)
{
    count fc1, fc2, n;
    for (fc1 = 0; fc1 < FCMAX; ++fc1)
    {
        for (n = 0; n < KFC_MAX[fc1]; ++n)
        {
            fc2 = KF_pointer[fc1][n]; /* bepaal de index van de conflicterende fasecyclus */
            TISG_PR[fc1][fc2] = TFG_max[fc1] + TVG_PR[fc1] + TIG_max[fc1][fc2];
            TISG_AR[fc1][fc2] = TFG_max[fc1] + TVG_AR[fc1] + TIG_max[fc1][fc2];
            TISG_AR_los[fc1][fc2] = TFG_max[fc1] + TVG_AR[fc1] + TIG_max[fc1][fc2];
        }
    }
}

/* INTERSTARTGROENTIJDEN VUL GROEN-GROEN CONFLICTEN IN */
/* --------------------------------------------------- */
/* void InterStartGroenTijden_VulGroenGroenConflictenIn(void) vult de TISG_PR[fc1][fc2] en TISG_AR[fc1][fc2] in voor alle Groen-Groen conflicten met de vastgroentijden
 * en verlengroentijden.
 *
 * InterStartGroenTijden_VulGroenGroenConflictenIn(void) wordt aangeroepen vanuit de applicatiefunctie void BepaalInterStartGroenTijden(void) na
 * de initialisatie van TISG_PR[][] en TISG_AR[][].
 * BepaalInterStartGroenTijden(void) wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door application().
 *
 * //Bepaal InterStartGroenTijden
 * InitInterStartGroenTijden();
 * InterStartGroenTijden_VulHardeConflictenIn();
 * InterStartGroenTijden_VulGroenGroenConflictenIn();
 *
 * //Pas interstartgroentijden aan voor naloopEG
 * InterStartGroenTijd_NLEG(fc02, fc62, tnlfg0262, tnlfgd0262, tnleg0262, tnlegd0262, tvgnaloop0262);
 * InterStartGroenTijd_NLEG(fc08, fc68, tnlfg0868, tnlfgd0868, tnleg0868, tnlegd0868, tvgnaloop0868);
 * InterStartGroenTijd_NLEG(fc11, fc68, tnlfg1168, tnlfgd1168, tnleg1168, tnlegd1168, tvgnaloop1168);
 * InterStartGroenTijd_NLEG(fc22, fc21, tnlfg2221, tnlfgd2221, tnleg2221, tnlegd2221, tvgnaloop2221);
 * InterStartGroenTijd_NLEG(fc82, fc81, tnlfg8281, tnlfgd8281, tnleg8281, tnlegd8281, tvgnaloop8281);
 */


void InterStartGroenTijden_VulGroenGroenConflictenIn(void)
{
    count fc1, fc2, n;
    for (fc1 = 0; fc1 < FCMAX; ++fc1)
    {
        for (n = KFC_MAX[fc1]; n < GKFC_MAX[fc1]; ++n)
        {
            fc2 = KF_pointer[fc1][n]; /* bepaal de index van de conflicterende fasecyclus */
            TISG_PR[fc1][fc2] = TFG_max[fc1] + TVG_PR[fc1];
            TISG_AR[fc1][fc2] = TFG_max[fc1] + TVG_AR[fc1];
            TISG_AR_los[fc1][fc2] = TFG_max[fc1] + TVG_AR[fc1];
        }
    }
}

/* INTERSTARTGROENTIJD NALOOP OP EINDE GROEN (NLEG) */
/* ------------------------------------------------ */
/* void InterStartGroenTijd_NLEG(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop) bepaalt TISG_PR[fc1][fc2] en TISG_AR[fc1][fc2]
 * bij een NaloopEG voor de harde en groen-groen conflicten van de volgrichting worden TISG_PR[fc1][fc2] en TISG_AR[fc1][fc2] bepaald t.o.v. de voedende richting.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus van de voedende richting
 * fc2 - index fasecyclus van de nalooprichting
 *
 * tnlfg     -  index tijdelement    - Vaste nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlfgd    - index tijdelement    - Detectie afhankelijke nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnleg     -  index tijdelement    - Vaste nalooptijd voor voertuig(en) tijdens groen (G[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlegd    - index tijdelement    - Detectie afhankelijke nalooptijd voor voertuig(en) tijdens groen of geel (G[fc1] || GL[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tvgnaloop - index T_max[]        - Maximale verlengtijd op eigen detectie na aflopen nalooptijden voor de nalooprichting.
 *
 * void InterStartGroenTijd_NLEG() wordt aangeroepen vanuit de applicatiefunctie void BepaalInterStartGroenTijden(void) na de initialisatie van TISG_PR[][] en TISG_AR[][].
 * BepaalInterStartGroenTijden(void) wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door application().
 *
 * //Bepaal InterStartGroenTijden
 * InitInterStartGroenTijden();
 * InterStartGroenTijden_VulHardeConflictenIn();
 *  InterStartGroenTijden_VulGroenGroenConflictenIn();
 *
 * //Pas interstartgroentijden aan voor naloopEG
 * InterStartGroenTijd_NLEG(fc02, fc62, tnlfg0262, tnlfgd0262, tnleg0262, tnlegd0262, tvgnaloop0262);
 * InterStartGroenTijd_NLEG(fc08, fc68, tnlfg0868, tnlfgd0868, tnleg0868, tnlegd0868, tvgnaloop0868);
 * InterStartGroenTijd_NLEG(fc11, fc68, tnlfg1168, tnlfgd1168, tnleg1168, tnlegd1168, tvgnaloop1168);
 * InterStartGroenTijd_NLEG(fc22, fc21, tnlfg2221, tnlfgd2221, tnleg2221, tnlegd2221, tvgnaloop2221);
 * InterStartGroenTijd_NLEG(fc82, fc81, tnlfg8281, tnlfgd8281, tnleg8281, tnlegd8281, tvgnaloop8281);
 *
 */

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

/* INTERSTARTGROENTIJD NALOOP OP EINDE VERLENGGROEN (NLEVG) */
/* -------------------------------------------------------- */
/* InterStartGroenTijd_NLEVG(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop) bepaalt TISG_PR[fc1][fc2] en TISG_AR[fc1][fc2]
 * bij een NaloopEVG voor de harde en groen-groen conflicten van de volgrichting worden  TISG_PR[fc1][fc2] en TISG_AR[fc1][fc2] bepaald t.o.v. de voedende richting.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus van de voedende richting
 * fc2 - index fasecyclus van de nalooprichting
 *
 * tnlfg     - index tijdelement - Vaste nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlfgd    - index tijdelement - Detectie afhankelijke nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlevg    - index tijdelement - Vaste nalooptijd voor voertuig(en) tijdens groen en geen meeverlenggroen (G[fc1] && !MG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlevgd   - index tijdelement - Detectie afhankelijke nalooptijd voor voertuig(en) tijdens groen en geen meeverlenggroen (G[fc1] && !MG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tvgnaloop - index T_max[] - Maximale verlengtijd op eigen detectie na aflopen nalooptijden voor de naloop richting.
 *
 * void InterStartGroenTijd_NLEVG() wordt aangeroepen vanuit de applicatiefunctie void BepaalInterStartGroenTijden(void) na de initialisatie van TISG_PR[][] en TISG_AR[][].
 * BepaalInterStartGroenTijden(void) wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door application().
 *
 * //Bepaal InterStartGroenTijden
 * InitInterStartGroenTijden();
 * InterStartGroenTijden_VulHardeConflictenIn();
 * InterStartGroenTijden_VulGroenGroenConflictenIn();
 *
 * //Pas interstartgroentijden aan voor naloopEG
 * InterStartGroenTijd_NLEG(fc02, fc62, tnlfg0262, tnlfgd0262, tnleg0262, tnlegd0262, tvgnaloop0262);
 * InterStartGroenTijd_NLEG(fc08, fc68, tnlfg0868, tnlfgd0868, tnleg0868, tnlegd0868, tvgnaloop0868);
 * InterStartGroenTijd_NLEG(fc11, fc68, tnlfg1168, tnlfgd1168, tnleg1168, tnlegd1168, tvgnaloop1168);
 * InterStartGroenTijd_NLEG(fc22, fc21, tnlfg2221, tnlfgd2221, tnleg2221, tnlegd2221, tvgnaloop2221);
 * InterStartGroenTijd_NLEG(fc82, fc81, tnlfg8281, tnlfgd8281, tnleg8281, tnlegd8281, tvgnaloop8281);
 *
 */

void InterStartGroenTijd_NLEVG(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];  /* bepaal de index van de conflicterende fasecyclus */
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
        k = KF_pointer[j][n]; /* bepaal de index van de conflicterende fasecyclus */
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

/* INTERSTARTGROENTIJD NALOOP OP STARTGROEN (NLSG) */
/* ------------------------------------------------ */
/* void InterStartGroenTijd_NLSG(count i, count j, count tnlsg, count tnlsgd) bepaalt de TISG_PR[fc1][fc2] en TISG_AR[fc1][fc2] bij een NaloopSG. 
 * voor de harde en groen-groen conflicten van de volgrichting worden TISG_PR[fc1][fc2] en TISG_AR[fc1][fc2] bepaald t.o.v. de voedende richting.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus van de voedende richting
 * fc2 - index fasecyclus van de nalooprichting
 *
 * tnlsg  - index tijdelement - vaste nalooptijd voor (eerste) voetganger vanaf startgroen (SG[fc1]) van de voedende richting; NG indien niet gebruikt.
 * tnlsgd - index tijdelement - detectie aanvraag afhankelijke nalooptijd voor (eerste) voetganger vanaf startgroen (SG[fc1]) van de voedende richting; NG indien niet gebruikt.
 *
 * void InterStartGroenTijd_NLSGG() wordt aangeroepen vanuit de applicatiefunctie void BepaalInterStartGroenTijden(void) na de initialisatie van TISG_PR[][] en TISG_AR[][].
 * BepaalInterStartGroenTijden(void) wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door application().
 *
 * //Bepaal InterStartGroenTijden
 * InitInterStartGroenTijden();
 * InterStartGroenTijden_VulHardeConflictenIn();
 * InterStartGroenTijden_VulGroenGroenConflictenIn();
 *
 * //Pas interstartgroentijden aan voor naloopEG
 * InterStartGroenTijd_NLEG(fc02, fc62, tnlfg0262, tnlfgd0262, tnleg0262, tnlegd0262, tvgnaloop0262);
 * InterStartGroenTijd_NLEG(fc08, fc68, tnlfg0868, tnlfgd0868, tnleg0868, tnlegd0868, tvgnaloop0868);
 * InterStartGroenTijd_NLEG(fc11, fc68, tnlfg1168, tnlfgd1168, tnleg1168, tnlegd1168, tvgnaloop1168);
 * InterStartGroenTijd_NLEG(fc22, fc21, tnlfg2221, tnlfgd2221, tnleg2221, tnlegd2221, tvgnaloop2221);
 * InterStartGroenTijd_NLEG(fc82, fc81, tnlfg8281, tnlfgd8281, tnleg8281, tnlegd8281, tvgnaloop8281);
 *
 * //Pas interstartgroentijden aan voor naloopSG
 * InterStartGroenTijd_NLSG(fc31, fc32, tnlsg3132, tnlsgd3132);
 * InterStartGroenTijd_NLSG(fc32, fc31, tnlsg3231, tnlsgd3231);
 * InterStartGroenTijd_NLSG(fc33, fc34, NG, tnlsgd3334);
 * InterStartGroenTijd_NLSG(fc34, fc33, NG, tnlsgd3433);
 */

void InterStartGroenTijd_NLSG(count i, count j, count tnlsg, count tnlsgd)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n]; /* bepaal de index van de conflicterende fasecyclus */
        if (!(tnlsg == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], T_max[tnlsg] + TIG_max[j][k]);
        if (!(tnlsgd == NG)) TISG_PR[i][k] = max(TISG_PR[i][k], T_max[tnlsgd] + TIG_max[j][k]);
        if (!(tnlsg == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], T_max[tnlsg] + TIG_max[j][k]);
        if (!(tnlsgd == NG)) TISG_AR[i][k] = max(TISG_AR[i][k], T_max[tnlsgd] + TIG_max[j][k]);
    }
}

/* INTERSTARTGROENTIJD HARD MEEVERLENGEN MET DEELCONFLICT */
/* ------------------------------------------------------ */
/* void InterStartGroentijd_HardMeeverlengenDeelconflict(mulv fc1, mulv fc2) bepaalt TISG_PR[fc1][fc2] en TISG_AR[fc1][fc2] bij toepassing van hard meeverlengen
 * van een deelconflict richting.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus van de richting die hard meeverlengt
 * fc2 - index fasecyclus van de richting
 *
 * void InterStartGroentijd_HardMeeverlengenDeelconflict) wordt aangeroepen vanuit de applicatiefunctie void BepaalInterStartGroenTijden(void) na
 * de initialisatie van TISG_PR[][] en TISG_AR[][].
 * BepaalInterStartGroenTijden(void) wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door application().
 *
 * //Pas InterstartGroentTijden aan voor hard meeverlengen deelconflict
 * InterStartGroentijd_HardMeeverlengenDeelconflict(fc05, fc22);
 * InterStartGroentijd_HardMeeverlengenDeelconflict(fc11, fc26);
 * InterStartGroentijd_HardMeeverlengenDeelconflict(fc05, fc32);
 */


void InterStartGroentijd_HardMeeverlengenDeelconflict(mulv fc1, mulv fc2)
{
    count fc;
    for (fc = 0; fc < FCMAX; fc++)
    {
        if (TIGR[fc2][fc] >= 0)
        {
            TISG_PR[fc1][fc] = max(TISG_PR[fc1][fc], TFG_max[fc1] + TVG_PR[fc1] + TIGR[fc2][fc]);
            TISG_AR[fc1][fc] = max(TISG_AR[fc1][fc], TFG_max[fc1] + TVG_AR[fc1] + TIGR[fc2][fc]);
            TISG_AR_los[fc1][fc] = max(TISG_AR_los[fc1][fc], TFG_max[fc1] + TVG_AR[fc1] + TIGR[fc2][fc]);
        }
    }
}

/* INTERSTARTGROENTIJD VOORSTART CORRECTIE */
/* --------------------------------------- */
/* bool InterStartGroenTijd_Voorstart_Correctie(count fcvs, count fcns, count tvs) corrigeert TISG_PR[fc1][fc2] en TISG_AR[fc1][fc2] bij deelconflicten met een Voorstart.
 *                                                                                  
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fcvs - index fasecyclus die als eerste start
 * fcns - index fasecyclus die als laatste start
 * tvs  - index tijdelement voorstarttijd
 *
 * InterStartGroenTijd_Voorstart_Correctie() wordt aangeroepen vanuit de applicatiefunctie BepaalInterStartGroenTijden() na de initialisatie van TISG_PR[][] en TISG_AR[][].
 * BepaalInterStartGroenTijden(void) wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door application().
 *
 *  // Pas InterStartGroentijden aan voor deelconflicten met Voorstart die nog groen moeten worden
 *
 *   do
 *   {
 *      wijziging = FALSE;
 *
 *      // Voorstart
 *      wijziging |= InterStartGroenTijd_Voorstart_Correctie(fc22, fc05, tvs2205);
 *      wijziging |= InterStartGroenTijd_Voorstart_Correctie(fc32, fc05, tvs3205);
 *
 *      wijziging |= Correctie_TISG_add();
 *
 *   } while (wijziging);
 */


bool InterStartGroenTijd_Voorstart_Correctie(count fcvs, count fcns, count tvs)
{
    count n;
    bool result;
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
        if ((TISG_AR_los[n][fcns] < TISG_AR_los[n][fcvs] + T_max[tvs]) && (TISG_AR_los[n][fcvs] > 0))
        {
            TISG_AR_los[n][fcns] = TISG_AR_los[n][fcvs] + T_max[tvs];
            result = TRUE;
        }
    }
    return result;
}


/* INTERSTARTGROENTIJD GELIJKSTART CORRECTIE */
/* ----------------------------------------- */
/* bool InterStartGroenTijd_Gelijkstart_Correctie(count fc1, count fc2) corrigeert TISG_PR[fc1][fc2] en TISG_AR[fc1][fc2] bij deelconflicten met een gelijkstart.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index eerste fasecyclus
 * fc2 - index tweede fasecyclus
 *
 * InterStartGroenTijd_Gelijstart_Correctie() wordt aangeroepen vanuit de applicatiefunctie BepaalInterStartGroenTijden() na de initialisatie van TISG_PR[][] en TISG_AR[][].
 * BepaalInterStartGroenTijden(void) wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door application().
 *
 *  // Pas InterStartGroentijden aan voor deelconflicten met Voorstart
 *
 *   do
 *   {
 *      wijziging = FALSE;
 *
 *      // Gelijkstart / voorstart
 *      if (SCH[schgs2484]) wijziging |= InterStartGroenTijd_Gelijkstart_Correctie(fc84, fc24);
 *      wijziging |= InterStartGroenTijd_Voorstart_Correctie(fc22, fc05, tvs2205);
 *      wijziging |= InterStartGroenTijd_Voorstart_Correctie(fc32, fc05, tvs3205);
 *
 *      wijziging |= Correctie_TISG_add();
 *
 *   } while (wijziging);
 */

bool InterStartGroenTijd_Gelijkstart_Correctie(count fc1, count fc2)
{
    count n;
    bool result;
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
        if (TISG_AR_los[n][fc1] < TISG_AR_los[n][fc2])
        {
            TISG_AR_los[n][fc1] = TISG_AR_los[n][fc2];
            result = TRUE;
        }
        else
        {
            if (TISG_AR_los[n][fc1] != TISG_AR_los[n][fc2])
            {
                TISG_AR_los[n][fc2] = TISG_AR_los[n][fc1];
                result = TRUE;
            }
        }
    }
    return result;
}

/* INTERSTARTGROENTIJD LATERELEASE CORRECTIE */
/* ----------------------------------------- */
/* bool InterStartGroenTijd_LateRelease_Correctie(count fclr, count fcvs, count tlr) corrigeert  TISG_PR[fc1][fc2] en TISG_AR[fc1][fc2] voor LateRelease.
 *                                                                              
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fclr  - index fasecyclus deelconflict die als laatste start met LateRelease
 * fcvs  - index fasecyclus deelconflict die als eerste start
 * tlr   - index tijdelement LateRelease
 *
 * InterStartGroenTijd_LateReLease_Correctie() wordt aangeroepen vanuit de applicatiefunctie BepaalInterStartGroenTijden() na de initialisatie van TISG_PR[][] en TISG_AR[][].
 * BepaalInterStartGroenTijden(void) wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door application().
 *
 *  // Pas InterStartGroentijden aan voor deelconflicten met LateRelease
 *
 *  do
 *  {
 *      wijziging = FALSE;
 *
 *      // Gelijkstart / voorstart / late release
 *      if (SCH[schgs2484]) wijziging |= InterStartGroenTijd_Gelijkstart_Correctie(fc84, fc24);
 *      wijziging |= InterStartGroenTijd_Voorstart_Correctie(fc22, fc05, tvs2205);
 *      wijziging |= InterStartGroenTijd_Voorstart_Correctie(fc32, fc05, tvs3205);
 *      wijziging |= InterStartGroenTijd_LateRelease_Correctie(fc26, fc11, tlr2611);
 *
 *      // Inlopen / inrijden
 *      wijziging |= InterStartGroenTijd_LateRelease_Correctie(fc62, fc02, txnl0262);
 *      wijziging |= InterStartGroenTijd_LateRelease_Correctie(fc68, fc08, txnl0868);
 *      wijziging |= InterStartGroenTijd_LateRelease_Correctie(fc68, fc11, txnl1168);
 *      wijziging |= InterStartGroenTijd_LateRelease_Correctie(fc21, fc22, txnl2221);
 *      wijziging |= InterStartGroenTijd_LateRelease_Correctie(fc32, fc31, txnl3132);
 *      wijziging |= InterStartGroenTijd_LateRelease_Correctie(fc31, fc32, txnl3231);
 *      wijziging |= InterStartGroenTijd_LateRelease_Correctie(fc34, fc33, txnl3334);
 *      wijziging |= InterStartGroenTijd_LateRelease_Correctie(fc33, fc34, txnl3433);
 *      wijziging |= InterStartGroenTijd_LateRelease_Correctie(fc81, fc82, txnl8281);
 *
 *      wijziging |= Correctie_TISG_add();
 *
 *   } while (wijziging);  //@PSN Let op! functie zou in theorie oneindig kunnen doorgaan!!
 */

bool InterStartGroenTijd_LateRelease_Correctie(count fclr, count fcvs, count tlr)
{
    count n;
    bool result = FALSE;
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



/* TVG_max[] - CORRECTIE MAXIMUM VERLENGGROENTIJDEN VOOR NALOOP RICHTINGEN */ 
/* ======================================================================= */

/* Voor de naloop richtingen moet de verlenggroentijd groot genoeg zijn om de gehele naloop te kunnen afwikkelingen.
 * TVG_max[] van de nalooprichting moet worden opgehoogd als naloop niet past.
 *
 * TVG_max[] voor de nalooprichting ophogen als naloop niet past, met de functies:
 * -------------------------------------------------------------------------------
 * void NaloopEG_TVG_Correctie(count fc1, count fc2, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop);     - bij een NaloopEG
 * void NaloopEVG_TVG_Correctie(count fc1, count fc2, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop);  - bij een NaloopEVG
 * void NaloopVtg_TVG_Correctie(count fc1, count fc2, count tnlsg, count tnlsgd);                                                - bij een NaloopSG
 *
 */


 /* TVG_max[] CORRECTIE NALOOP EINDE GROEN (NLEG) */  
 /* --------------------------------------------- */
 /* void NaloopEG_TVG_NaloopEG_Correctie(count fc1, count fc2, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop)
  * wordt in de regelapplicatie gebruikt voor corrigeren/aanpassen van de verlenggroentijd (TVG_max[]) van de naloop/volgrichting voor een NaloopEG.
  *
  * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
  * fc1 - index fasecyclus van de voedende richting
  * fc2 - index fasecyclus van de nalooprichting
  *
  * tnlfg     - index tijdelement - Vaste nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
  *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
  * tnlfgd    - index tijdelement - Detectie afhankelijke nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
  *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
  * tnleg     - index tijdelement - Vaste nalooptijd voor voertuig(en) tijdens groen (G[fc1]) van de voedende richting;
  *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
  * tnlegd    - index tijdelement - Detectie afhankelijke nalooptijd voor voertuig(en) tijdens groen of geel (G[fc1] || GL[fc1]) van de voedende richting;
  *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
  * tvgnaloop - index T_max[] - Maximale verlengtijd op eigen detectie na aflopen nalooptijden voor de naloop richting.
  *
  * De functie NaloopEG_Correctie() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
  *
  * Voorbeelden: NaloopEG_TVG_Correctie(fc02, fc62, tnlfg0262, tnlfgd0262, tnleg0262, tnlegd0262, tvgnaloop0262
  *              NaloopEG_TVG_Correctie(fc08, fc68, tnlfg0868, tnlfgd0868, tnleg0868, tnlegd0868, tvgnaloop0868);
  *              NaloopEG_TVG_Correctie(fc11, fc68, tnlfg1168, tnlfgd1168, tnleg1168, tnlegd1168, tvgnaloop1168);
  *              NaloopEG_TVG_Correctie(fc22, fc21, tnlfg2221, tnlfgd2221, tnleg2221, tnlegd2221, tvgnaloop2221);
  */

void NaloopEG_TVG_Correctie(count fc1, count fc2, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop)
{
    if (G[fc1] || GL[fc1] || T[tvgnaloop])
    {
        if (G[fc2])
        {
            if (AR[fc1])
            {
                if (!(tnlfg == NG)) TVG_max[fc2] = max(TVG_max[fc2], TFG_max[fc1] - TFG_timer[fc1] - TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlfgd == NG)) TVG_max[fc2] = max(TVG_max[fc2], TFG_max[fc1] - TFG_timer[fc1] - TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + T_max[tnlfg] - (FG[fc1] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnleg == NG)) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + TFG_max[fc1] - TFG_timer[fc1] + ((CV[fc1] && (TVG_AR[fc1] > TVG_timer[fc1])) ? (TVG_AR[fc1] - TVG_timer[fc1]) : 0) + T_max[tnleg] - T_timer[tnleg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlegd == NG)) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + TFG_max[fc1] - TFG_timer[fc1] + ((CV[fc1] && (TVG_AR[fc1] > TVG_timer[fc1])) ? (TVG_AR[fc1] - TVG_timer[fc1]) : 0) + TGL_max[fc1] - TGL_timer[fc1] + T_max[tnlegd] - ((G[fc1] || GL[fc1]) ? 0 : T_timer[tnlegd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);
            }
            else
            {
                if (!(tnlfg == NG)) TVG_max[fc2] = max(TVG_max[fc2], TFG_max[fc1] - TFG_timer[fc1] - TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlfgd == NG)) TVG_max[fc2] = max(TVG_max[fc2], TFG_max[fc1] - TFG_timer[fc1] - TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + T_max[tnlfg] - (FG[fc1] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnleg == NG)) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + TFG_max[fc1] - TFG_timer[fc1] + ((CV[fc1]) ? (TVG_max[fc1] - TVG_timer[fc1]) : 0) + T_max[tnleg] - T_timer[tnleg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlegd == NG)) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + TFG_max[fc1] - TFG_timer[fc1] + ((CV[fc1]) ? (TVG_max[fc1] - TVG_timer[fc1]) : 0) + TGL_max[fc1] - TGL_timer[fc1] + T_max[tnlegd] - ((G[fc1] || GL[fc1]) ? 0 : T_timer[tnlegd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);

            }
        }
    }
    if ((TVG_max[fc2] < TVG_AR[fc2]) && AR[fc2]) TVG_max[fc2] = TVG_AR[fc2];
}                                                                           

/* TVG_max[] CORRECTIE NALOOP EINDE VERLENGGROEN (NLEVG) */ 
/* ----------------------------------------------------- */
/* void NaloopEVG_TVG_Correctie(count fc1, count fc2, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop)
 * wordt in de regelapplicatie gebruikt voor corrigeren/aanpassen van de verlenggroentijd (TVG_max[]) van de naloop/volgrichting voor een NaloopEVG.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus van de voedende richting
 * fc2 - index fasecyclus van de nalooprichting
 *
 * tnlfg     - index tijdelement - Vaste nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlfgd    - index tijdelement - Detectie afhankelijke nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlevg    - index tijdelement - Vaste nalooptijd voor voertuig(en) tijdens groen en geen meeverlenggroen (G[fc1] && !MG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlevgd   - index tijdelement - Detectie afhankelijke nalooptijd voor voertuig(en) tijdens groen en geen meeverlenggroen (G[fc1] && !MG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tvgnaloop - index T_max[] - Maximale verlengtijd op eigen detectie na aflopen nalooptijden voor de naloop richting.
 *
 * De functie NaloopEVG_Correctie() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 *
 * Voorbeelden: NaloopEG_TVG_Correctie(fc02, fc62, tnlfg0262, tnlfgd0262, tnleg0262, tnlegd0262, tvgnaloop0262
 *              NaloopEG_TVG_Correctie(fc08, fc68, tnlfg0868, tnlfgd0868, tnleg0868, tnlegd0868, tvgnaloop0868);
 *              NaloopEG_TVG_Correctie(fc11, fc68, tnlfg1168, tnlfgd1168, tnleg1168, tnlegd1168, tvgnaloop1168);
 *              NaloopEG_TVG_Correctie(fc22, fc21, tnlfg2221, tnlfgd2221, tnleg2221, tnlegd2221, tvgnaloop2221);
 */

void NaloopEVG_TVG_Correctie(count fc1, count fc2, count tnlfg, count tnlfgd, count tnlcv, count tnlcvd, count tvgnaloop)
{
    if (G[fc1] || GL[fc1] || T[tvgnaloop])
    {
        if (G[fc2])
        {
            if (AR[fc1])
            {
                if (!(tnlfg == NG)) TVG_max[fc2] = max(TVG_max[fc2], TFG_max[fc1] - TFG_timer[fc1] - TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlfgd == NG)) TVG_max[fc2] = max(TVG_max[fc2], TFG_max[fc1] - TFG_timer[fc1] - TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + T_max[tnlfg] - (FG[fc1] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlcv == NG)) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + TFG_max[fc1] - TFG_timer[fc1] + ((CV[fc1] && (TVG_AR[fc1] > TVG_timer[fc1])) ? (TVG_AR[fc1] - TVG_timer[fc1]) : 0) + T_max[tnlcv] - T_timer[tnlcv] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlcvd == NG)) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + TFG_max[fc1] - TFG_timer[fc1] + ((CV[fc1] && (TVG_AR[fc1] > TVG_timer[fc1])) ? (TVG_AR[fc1] - TVG_timer[fc1]) : 0) + T_max[tnlcvd] - ((G[fc1] && !MG[fc1]) ? 0 : T_timer[tnlcvd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);
            }
            else
            {
                if (!(tnlfg == NG)) TVG_max[fc2] = max(TVG_max[fc2], TFG_max[fc1] - TFG_timer[fc1] - TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + T_max[tnlfg] - T_timer[tnlfg] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlfgd == NG)) TVG_max[fc2] = max(TVG_max[fc2], TFG_max[fc1] - TFG_timer[fc1] - TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + T_max[tnlfg] - (FG[fc1] ? 0 : T_timer[tnlfgd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlcv == NG)) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + TFG_max[fc1] - TFG_timer[fc1] + ((CV[fc1]) ? (TVG_max[fc1] - TVG_timer[fc1]) : 0) + T_max[tnlcv] - T_timer[tnlcv] + T_max[tvgnaloop] - T_timer[tvgnaloop]);
                if (!(tnlcvd == NG)) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc2] + TVG_timer[fc2] + TFG_max[fc1] - TFG_timer[fc1] + ((CV[fc1]) ? (TVG_max[fc1] - TVG_timer[fc1]) : 0) + T_max[tnlcvd] - ((G[fc1] && !MG[fc1]) ? 0 : T_timer[tnlcvd]) + T_max[tvgnaloop] - T_timer[tvgnaloop]);

            }
        }
    }
    if ((TVG_max[fc2] < TVG_AR[fc2]) && AR[fc2]) TVG_max[fc2] = TVG_AR[fc2];
}

//@PSN Algemeen: functienaam met TVG laten beginnen. TVG_NaloopSG_Correctie() etc. zoals ook bij realisatie en interstartgroentijd TVG_NaloopEG_Correctie().

/* TVG_max[] CORRECTIE NALOOP START GROEN (NLVTG) */ 
/* ---------------------------------------------- */
/* void NaloopVtg_TVG_Correctie(count fc1, count fc2, count tnlsg, count tnlsgd)    //@PSN  'fc1' : unreferenced formal parameter
 * wordt in de regelapplicatie gebruikt voor corrigeren/aanpassen van de verlenggroentijd (TVG_max[]) van de volgrichting voor een NaloopVtg.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus van de voedende richting                                  //@PSN  'fc1' : unreferenced formal parameter
 * fc2 - index fasecyclus van de nalooprichting
 *
 * hnlsg  - hulpwaarde naloop wel/niet toegestaan op basis van primair+alternatief gecoordineerd 
 * tnlsg  - vaste nalooptijd voor (eerste) voetganger vanaf startgroen (SG[fc1]) van de voedende richting; NG indien niet gebruikt.
 * tnlsgd - detectie aanvraag afhankelijke nalooptijd voor (eerste) voetganger vanaf startgroen (SG[fc1]) van de voedende richting; NG indien niet gebruikt
 *
 * De functie NaloopVtg_RVG_Correctie() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 *
 * Voorbeelden: NaloopVtg_TVG_Correctie(fc31, fc32, hnlsg3132, tnlsg3132, tnlsgd3132);
 *              NaloopVtg_TVG_Correctie(fc32, fc31, hnlsg3231, tnlsg3231, tnlsgd3231);
 *              NaloopVtg_TVG_Correctie(fc33, fc34, hnlsg3334, NG, tnlsgd3334);
 *              NaloopVtg_TVG_Correctie(fc34, fc33, hnlsg4133, NG, tnlsgd3433);
 */

void NaloopVtg_TVG_Correctie(count fc1, count fc2, count hnlsg, count tnlsg, count tnlsgd)
{
   fc1 = 0; // fc1 wordt niet gebruikt in deze functie. door deze toevoeging wordt een compileer warming voorkomen 
   if (!(tnlsg == NG) && H[hnlsg]) TVG_max[fc2] = max(TVG_max[fc2], T_max[tnlsg] - T_timer[tnlsg] + TVG_timer[fc2]);
   if (!(tnlsgd == NG) && H[hnlsg]) TVG_max[fc2] = max(TVG_max[fc2], -TFG_max[fc2] + TFG_timer[fc2] + T_max[tnlsgd] - T_timer[tnlsgd] + TVG_timer[fc2]);
}

/* VASTHOUDEN NALOOP START GROEN/VOETGANGER (NLSG/NLVTG) */ 
/* ----------------------------------------------------- */
/* void NaloopVtg(count fc1, count fc2, count dk, count hdk, bool hnlsg, count tnlsg, count tnlsgd) //@PSN  hnlsg is een index dus count ip.v. bool.
 * wordt in de regelapplicatie gebruikt voor het in groen vasthouden van de volgrichting voor voetgangers van de voedende richting die op start groen vertrekken.
 * volgrichting in MeeverlengGroen (MG[fc2]) wordt teruggezet naar wachtgroen (WG{[fc2]) tijdens Rood Na Aanvraag (RA[fc1]) en StartGroen (SG(fc1) van de voedende richting
 * mey de instructie Retour Wachtgroen (RW[] - BIT2). @PSN waarom geen BIT12 gebruik zoals ook MK?
 * volgrichting wordt tijdens verlengroen vastgehouden met de instructie meetkriterium (MK[] - BIT12).
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus van de voedende richting
 * fc2 - index fasecyclus van de nalooprichting
 * hdk    - index hulpelement - voor onthouden van de detectieaanvraag van de voedende richting; NG indien niet gebruikt. @@ DA[[d**$] kan ook worden gebruikt i.p.v een hulpelement?
 * hnlsg  -  index hulpelement - naloop voetgangers wel/niet toegestaan; status van dit hulpelement wordt buiten deze functie bepaald. //@@##  is dit wel gewenst??
 * tnlsg  -  index tijdelement - vaste nalooptijd voor (eerste) voetganger vanaf startgroen (SG[fc1]) van de voedende richting; NG indien niet gebruikt.
 * tnlsgd - index tijdelement - detectie aanvraag afhankelijke nalooptijd voor (eerste) voetganger vanaf startgroen (SG[fc1]) van de voedende richting; NG indien niet gebruikt.
 *
 * De functie NaloopVtg() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 *
 * Voorbeelden: NaloopVtg(fc31, fc32, dk31a, hmadk31a, hnlsg3132, tnlsg3132, tnlsgd3132);
 *              NaloopVtg(fc32, fc31, dk32a, hmadk32a, hnlsg3231, tnlsg3231, tnlsgd3231);
 *              NaloopVtg(fc33, fc34, dk33a, hmadk33a, hnlsg3334, NG, tnlsgd3334);
 *              NaloopVtg(fc34, fc33, dk34a, hmadk34a, hnlsg3433, NG, tnlsgd3433);
 *
 */

void NaloopVtg(count fc1, count fc2, count dk, count hdk, bool hnlsg, count tnlsg, count tnlsgd)
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

/* VASTHOUDEN NALOOP EINDE GROEN (NLEG) */
/* ------------------------------------ */
/* void NaloopEG(count fc1, count fc2, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop, ...)
 * wordt in de regelapplicatie gebruikt voor het in groen vasthouden van de volgrichting voor het verkeer van de voedende richting.
 * volgrichting in MeeverlengGroen (MG[fc2]) wordt teruggezet naar wachtgroen (WG[fc2]) tijdens Rood Na Aanvraag (RA[fc1]) en StartGroen (SG[fc1])
 * van de voeddende richting met de instructie Retour Wachtgroen (RW[] - BIT2). @PSN waarom geen BIT12 gebruik zoals ook MK?
 * volgrichting wordt tijdens verlengroen vastgehouden met de instructie meetkriterium (MK[] - BIT12).
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus van de voedende richting
 * fc2 - index fasecyclus van de nalooprichting
 * tnlfg     - index tijdelement - Vaste nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlfgd    - index tijdelement - Detectie afhankelijke nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnleg     - index tijdelement - Vaste nalooptijd voor voertuig(en) tijdens groen (G[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlegd    - index tijdelement - Detectie afhankelijke nalooptijd voor voertuig(en) tijdens groen of geel (G[fc1] || GL[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tvgnaloop - index T_max[] - Maximale verlengtijd op eigen detectie na aflopen nalooptijden voor de naloop richting.
 * ...       - index detectie    - index d**$ van de detectie (koplussen) van de voedende richting voor detectieafhankelijke naloop.
 *
 *
 * De functie NaloopEG() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 *
 * Voorbeelden: NaloopEG(fc02, fc62, tnlfg0262, tnlfgd0262, tnleg0262, tnlegd0262, tvgnaloop0262, d02_1a, d02_1b, END);   //naloop van fc02 -> fc62
 *              NaloopEG(fc08, fc68, tnlfg0868, tnlfgd0868, tnleg0868, tnlegd0868, tvgnaloop0868, d08_1a, d08_1b, END);   //naloop van fc08 -> fc68
 *              NaloopEG(fc11, fc68, tnlfg1168, tnlfgd1168, tnleg1168, tnlegd1168, tvgnaloop1168, d11_1, END);
 *              NaloopEG(fc22, fc21, tnlfg2221, tnlfgd2221, tnleg2221, tnlegd2221, tvgnaloop2221, d22_1, END);
 *              NaloopEG(fc82, fc81, tnlfg8281, tnlfgd8281, tnleg8281, tnlegd8281, tvgnaloop8281, d82_1, END);
 *
 */


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

/* VASTHOUDEN NALOOP EINDE VERLENGGROEN (NLEVG) */
/* -------------------------------------------- */
/* void NaloopEVG(count fc1, count fc2, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop, ...)
 * wordt in de regelapplicatie gebruikt voor het in groen vasthouden van de volgrichting voor het verkeer van de voedende richting.
 * volgrichting in MeeverlengGroen (MG[fc2]) wordt teruggezet naar wachtgroen (WG{[fc2]) tijdens Rood Na Aanvraag (RA[fc1]) en StartGroen (SG(fc1)
 * van de voedende richting met de instructie Retour Wachtgroen (RW[] - BIT2). @PSN waarom geen BIT12 gebruik zoals ook MK?
 * volgrichting wordt tijdens verlengroen vastgehouden met de instructie meetkriterium (MK[] - BIT12).
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus van de voedende richting
 * fc2 - index fasecyclus van de nalooprichting
 * tnlfg     - index tijdelement - Vaste nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlfgd    - index tijdelement - Detectie afhankelijke nalooptijd voor (eerste) voertuig(en) tijdens vastgroen (FG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlevg    - index tijdelement - Vaste nalooptijd voor voertuig(en) tijdens groen en geen meeverlenggroen (G[fc1] && !MG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tnlevgd   - index tijdelement - Detectie afhankelijke nalooptijd voor voertuig(en) tijdens groen en geen meeverlenggroen (G[fc1] && !MG[fc1]) van de voedende richting;
 *                                 rijtijd tot de detectie van de volgrichting; NG indien niet gebruikt.
 * tvgnaloop - index T_max[] - Maximale verlengtijd op eigen detectie na aflopen nalooptijden voor de naloop richting.
 * ...       - index detecie     - index d**$ van de detectie (koplussen) van de voedende richting voor detectieafhankelijke naloop.
 *
 * De functie NaloopEG() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden().
 *
 * Voorbeelden:  NaloopEVG(fc02, fc62, tnlfg0262, tnlfgd0262, tnleg0262, tnlegd0262, tvgnaloop0262, d02_1a, d02_1b, END);   //naloop van fc02 -> fc62
 *               NaloopEVG(fc08, fc68, tnlfg0868, tnlfgd0868, tnleg0868, tnlegd0868, tvgnaloop0868, d08_1a, d08_1b, END);   //naloop van fc08 -> fc68
 *
 */

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


/* MAX PAR */
/* ------- */
/* bool max_par(count fc, mulv t_wacht[])
 * wordt gebruikt bij het bepalen van de status van de Periode Alternatieve Realisatie (PAR[fc##]) van een fasecylus.
 * max_par() maakt gebruik van de maximale waarde van de realisatietijd REALISATIETIJD_max[fc] en de Interstartgroentijd Alternatief (TISG_AR[fc][k]).
 * TISG_AR[fc][k] is de minimale benodigde tijd voor de alternatieve realisatie van de fasecyclus.
 *
 * max_par() geeft als return-waarde waar (TRUE), als de Periode Alternatieve Realisatie mag worden opgezet, ander niet waar (FALSE).
 *
 * De functie max_par() wordt aangeroepen vanuit de applicatiefunctie RealisatieAfhandeling().
 *
 * voorbeelden: PAR[fc02] = max_par(fc02, PRML, ML) && SCH[schaltg02];
 *              PAR[fc03] = max_par(fc03, PRML, ML) && SCH[schaltg03];
 *              PAR[fc05] = max_par(fc05, PRML, ML) && SCH[schaltg05];
 *              PAR[fc08] = max_par(fc08, PRML, ML) && SCH[schaltg08];
 */


bool max_par(count fc, mulv t_wacht[])   
{
    int k, n;
    if (kcv(fc)) return FALSE;
    for (n = 0; n < FKFC_MAX[fc]; ++n)
    {
        k = KF_pointer[fc][n];
        if ((t_wacht[k] > 0) && ((t_wacht[k] - REALISATIETIJD_max[fc] + offsetAR) < TISG_AR[fc][k]))
        {
            return FALSE;
        }

    }
    return TRUE;
}

/* MAX PAR_LOS */
/* ----------- */
/* bool max_par_los(count fc, mulv t_wacht[])
 * wordt gebruikt bij het bepalen van de status van de Periode Alternatieve Realisatie (PAR[fc##]) van een fasecylus.
 * max_par_los() maakt gebruik van de maximale waarde van de realisatietijd REALISATIETIJD_max[fc] en de Interstartgroentijd Alternatief (TISG_AR[fc][k]).
 * TISG_AR[fc][k] is de minimale benodigde tijd voor de alternatieve realisatie van de fasecyclus.
 *
 * PAR_los bepaalt of een gecoordineerde richting (schakelbaar) los mag realiseren.
 * Bij eventuele parallelle deelconflicten mag de riching alleen komen als zijn meerealiserende richting ook alternatief mag komen.
 * Als bepaald is dat een richting moet komen mag hij niet meer worden teruggenomen.
 *
 * max_par_los() geeft als return-waarde waar (TRUE), als de Periode Alternatieve Realisatie mag worden opgezet, ander niet waar (FALSE).
 *
 * De functie max_par_los() wordt aangeroepen vanuit de applicatiefunctie RealisatieAfhandeling().
 *
 * voorbeelden: PAR_los[fc31] = max_par_los(fc31, twacht) && SCH[schlos3132] && (!IH[hmadk31a] || SCH[schgeennla3132])              || RA[fc31] && PAR_los[fc31];
 *              PAR_los[fc32] = max_par_los(fc32, twacht) && SCH[schlos3231] && (!IH[hmadk32a] || SCH[schgeennla3231] && PAR[fc22]) || RA[fc32] && PAR_los[fc32]; // voorstart 
 */

bool max_par_los(count fc, mulv t_wacht[])
{
    int k, n;
    if (kcv(fc)) return FALSE;
    for (n = 0; n < FKFC_MAX[fc]; ++n)
    {
        k = KF_pointer[fc][n];
        if ((t_wacht[k] > 0) && !((TIG_max[fc][k] == FK) && (FK_type[fc][k] == FK_SG)) && ((REALISATIETIJD_max[k] - REALISATIETIJD_max[fc] + offsetAR) < TISG_AR_los[fc][k])) return FALSE;
    }
    return TRUE;
}

/* MAXIMALE WACHTTIJD VAN ALLE PRIMAIRE FASECYCLI */
/* ============================================== */

/* void max_wachttijd_modulen_primair_ISG(bool* prml[], count ml, count ml_max, mulv t_wacht[])
 * berekent de wachttijden van de primaire fasecycli van de modulenreeks. eerst worden de wachttijden berekent van de primaire
 * fasecycli van de actieve module. daarna van de primaire fasecycli van de daarna volgende modulen.
 * de functie max_wachttijd_modulen_primair_ISG() berekent de wachttijden in een systeemronde.
 * max_wachttijd_modulen_primair_ISG() is gebaseerd op de werking van de functie max_wachttijd_modulen_primair() die is gedefinieerd in wtvfunc.c
 * max_wachttijd_modulen_primair_ISG() gebruikt bij de berekening van de primaire wachttijden de waarden van REALSATIETIJD_max[].
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * prml[]   - pointer naar de primaire moduletoedeling
 * ml       - waarde van de actieve module van de modulereeks
 * ml_max   - maximum aantal moduluen van de modulereeks
 *
 * De functie max_wachttijd_modulen_primair_ISG() wordt aangeroepen vanuit de applicatiefunctie application(), na de aanroep van
 * de functie Synchronisaties(), waar REALISATIETIJD_max[] wordt bepaald.
 *
 * Voorbeeld: max_wachttijd_modulen_primair_ISG(PRML, ML, MLMAX, twacht);
 */


void max_wachttijd_modulen_primair_ISG(bool* prml[], count ml, count ml_max)  
{
    register count i, j, m, n, hml;
    mulv twacht_tmp = NG;


    /* reset wachttijden van alle fasecycli */
    /* ------------------------------------ */
    for (i = 0; i < FC_MAX; i++)
    {
        twacht[i] = NG; 
        twacht_wtv[i] = NG; 
        twacht_AR[i] = NG;
        twacht_AR_wtv[i] = NG; 
        twacht_afkap[i] = NG; 
    }
    /* bereken wachttijden van de primaire fasecycli van de actieve module */
    /* ------------------------------------------------------------------- */
    for (i = 0; i < FC_MAX; i++) {
        if ((prml[ml][i] & PRIMAIR_VERSNELD) && !PG[i] && R[i])
        {
            twacht[i] = REALISATIETIJD_max[i]; 
            twacht_wtv[i] = REALISATIETIJD_max_wtv[i]; 
            for (j = 0; j < FC_MAX; j++)
            {
                if (RA[j] && AR[j])
                    /* @PSN if ((RA[j] && prml[ml][j] & ALTERNATIEF_VERSNELD)) */
                {
                    if (PAR_los[j])
                    {
                        if (TISG_AR_los[j][i] >= 0)
                        {
                            twacht[i] = max(twacht[i], REALISATIETIJD_max[j] + TISG_AR_los[j][i]);
                            twacht_wtv[i] = max(twacht_wtv[i], REALISATIETIJD_max_wtv[j] + TISG_AR_los[j][i]);
                        }
                    }
                    else
                    {
                        if (TISG_AR[j][i] >= 0)
                        {
                            twacht[i] = max(twacht[i], REALISATIETIJD_max[j] + TISG_AR[j][i]); //@@is ISG aanpassing
                            twacht_wtv[i] = max(twacht_wtv[i], REALISATIETIJD_max_wtv[j] + TISG_AR[j][i]); //@@is ISG aanpassing
                        }
                    }

                    if (TISG_afkap[j][i] >= 0)
                    {
                        twacht_afkap[i] = max(twacht_afkap[i], REALISATIETIJD_max_wtv[j] + TISG_afkap[j][i]); //@@is ISG aanpassing
                    }
                }
            }
        }
    }
    /* bereken wachttijden van de primaire fasecycli van de volgende modulen */
    /* --------------------------------------------------------------------- */
    hml = ml + 1;
    if (hml >= ml_max)  hml = ML1;
    for (m = 1; m < ml_max; m++) {
        for (i = 0; i < FC_MAX; i++)
        {
            if ((prml[hml][i] & PRIMAIR_VERSNELD) && !PG[i] && (twacht[i] < 0))
            {
                twacht[i] = REALISATIETIJD_max[i];
                twacht_wtv[i] = REALISATIETIJD_max_wtv[i]; //@@is ISG aanpassing
                for (j = 0; j < FC_MAX; j++)
                {

                    if (RA[j] && AR[j] && !RR[j]) //@@is ISG aanpassing
                        /* @PSN                   if ((RA[j] && prml[ml][j] & ALTERNATIEF_VERSNELD)) */
                    {
                        if (PAR_los[j])
                        {
                            if (TISG_AR_los[j][i] >= 0)
                            {
                                twacht[i] = max(twacht[i], REALISATIETIJD_max[j] + TISG_AR_los[j][i]);
                            }
                        }
                        else
                        {
                            if (TISG_AR[j][i] >= 0)
                            {
                                twacht[i] = max(twacht[i], REALISATIETIJD_max[j] + TISG_AR[j][i]);
                                twacht_wtv[i] = max(twacht_wtv[i], REALISATIETIJD_max_wtv[j] + TISG_AR[j][i]);
                            }
                        }

                        if (TISG_afkap[j][i] >= 0)
                        {
                            twacht_afkap[i] = max(twacht_afkap[i], REALISATIETIJD_max[j] + TISG_afkap[j][i]);
                        }
                    }
                }

                for (n = 0; n < FC_MAX; n++)
                {
                    if (TISG_PR[n][i] >= 0 && i != n)
                    {
                        if (twacht[n] >= 0)
                        {
                            twacht_tmp = twacht[n] + TISG_PR[n][i];
                            if (twacht_tmp > twacht[i])  twacht[i] = twacht_tmp;
                            twacht_tmp = twacht_wtv[n] + TISG_PR[n][i];
                            if (twacht_tmp > twacht_wtv[i])  twacht_wtv[i] = twacht_tmp;
                        }
                    }
                    if (TISG_afkap[n][i] >= 0 && i != n)
                    {
                        if (twacht_afkap[n] >= 0)
                        {
                            twacht_tmp = twacht_afkap[n] + TISG_afkap[n][i];
                            if (twacht_tmp > twacht_afkap[i])  twacht_afkap[i] = twacht_tmp;
                        }
                    }
                }
            }
        }

        /* wachttijden voor overgeslagen fasecycli */
        /* -------------------------------------- */
        for (i = 0; i < FC_MAX; i++) {
            if (PG[i] && !G[i]) {
                twacht[i] = -3;
                twacht_wtv[i] = -3;
            }
        }

        /* volgende module */
        /* --------------- */
        hml++;
        if (hml >= ml_max)  hml = ML1;

    }
    for (i = 0; i < FC_MAX; i++) {
        if (RA[i] && AR[i]) {
            twacht_AR[i] = REALISATIETIJD_max[i];
            twacht_AR_wtv[i] = REALISATIETIJD_max_wtv[i];
        }

    }
}

/* VASTHOUDEN MODULE */
/* ================= */

/* bool yml_cv_pr_nl_ISG(bool* prml[], count ml, count ml_max) 
 * wordt gebruikt voor het vasthouden van de module-afwikkeling en houdt de actieve module ook vast voor de afwikkeling van nalopen.
 *
 * yml_cv_pr_nl_ISG() is gebaseerd op de werking van de functie bool yml_pg_kcv(), die is gedefinieerd in mlefunc.c.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * prml[]   - pointer naar de primaire moduletoedeling
 * ml       - waarde van de actieve module van de modulereeks
 * ml_max   - maximum aantal moduluen van de modulereeks
 *
 * De functie  yml_cv_pr_nl_ISG() wordt aangeroepen vanuit de applicatiefunctie RealisatieAfhandeling().
 *
 * Voorbeeld:  YML[ML] = yml_cv_pr_nl_ISG(PRML, ML, ML_MAX);
 */


bool yml_cv_pr_nl_ISG(bool* prml[], count ml, count ml_max)
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

/* SET PG[] DEELCONFLICT VOORSTART */
/* ------------------------------- */
/* @PSN Deze functie wordt nu nog niet gebruikt in de hudige release
 * void set_PG_Deelconflict_Voorstart(mulv fc1, mulv fc2)
 * set_PG_Deelconflict_Voorstart() wordt gebruikt bij deelconflciten voor het overslaan en tegenhouden een fasecyclus bij een voorstart.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus die wordt overgeslagen
 * fc2 - index fasecyclus die groen toont, eerst startende fasecyclus.
 *
 * De functie  set_PG_Deelconflict_Voorstart() wordt aangeroepen vanuit de applicatiefunctie RealisatieAfhandeling()
 *
 * Voorbeeld:  set_PG_Deelconflict_Voorstart(fc02, fc22);
 */


void set_PG_Deelconflict_Voorstart(mulv fc1, mulv fc2)
{
    if (G[fc2] && !G[fc1] && !PG[fc1])
    {
        PG[fc1] |= PRIMAIR_OVERSLAG;
        RR[fc1] |= BIT2; /* fc1 mag niet alternatief realiseren */
    }
    else
    {
        RR[fc1] &= ~BIT2;
    }
}

/* SET PG[] DEELCONFLICT LATERELEASE */
/* --------------------------------- */
/* @PSN wordt deze functie gebruikt in TLCGen?
 * void set_PG_Deelconflict_LateTelease(mulv fc1, mulv fc2)
 * set_PG_Deelconflict_LateRelease() wordt gebruikt bij deelconflicten voor het overslaan en tegenhouden een fasecyclus bij een LateRelease.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus die wordt overgeslagen
 * fc2 - index fasecyclus die groen toont, eerst startende fasecyclus.
 * tlr - index van de instelling (T_max[]) LateRelease tijd.
 *
 * De functie  set_PG_Deelconflict_LateRelease() wordt aangeroepen vanuit de applicatiefunctie RealisatieAfhandeling()
 *
 * Voorbeeld:  set_PG_Deelconflict_Voorstart( fc02, fc22);
 */

void set_PG_Deelconflict_LateRelease(mulv fc1, mulv fc2, mulv tlr)
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

/* MEEVERLENGEN UIT DOOR DEELCONFLICT VOORSTART */
/* -------------------------------------------- */
/* @PSN wordt deze functie gebruikt in TLCGen?
 * void MeeverlengenUitDoorDeelconflictVoorstart(mulv fc1, mulv fc2)
 * MeeverlengenUitDoorDeelconflictVoorstart() wordt gebruikt bij deelconflicten voor uitschakeling van het meeverlengen van een fasecyclus YM[] &= BIT4~; bij een voorstart.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc1 - index fasecyclus die als eerste start
 * fc2 - index fasecyclus waarvan meeverlengen wordt uitgeschakeld
 *
 * De functie MeeverlengenUitDoorDeelconflictVoorstart() wordt aangeroepen vanuit de applicatiefunctie RealisatieAfhandeling()
 *
 * Voorbeeld: MeeverlengenUitDoorDeelconflictVoorstart(fc02, fc22);
 */

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

/* PERCENTAGEVERLENGGROENTIJDEN ISG */
/* -------------------------------- */
/* @PSN gebruiken wij deze ? 
 * void PercentageVerlengGroenTijdenISG(count fc, count percentage)   //@@ in de naam zit ISG, maar daar maakt de functie geen gebruik van; is ISG weglaten. @@Tijden is onjuist, betreft een tijd.
 * PercentageVerlengGroenTijdenISG() wordt gebruikt voor de berekening van de instelling van de verlenggroentijd van een fasecyclus op basis van een opgegeven percentage.
 *
 * bij aanroep van de functie dienen de volgende argumenten te worden meegegeven:
 * fc        - index fasecyclus
 * percntage - index van de parameter van het percentage
 *
 * De functie PercentageVerlengGroenTijdenISG() wordt aangeroepen vanuit de applicatiefunctie ???.
 *
 * Voorbeeld: PercentageVerlengGroenTijdenISG(fc03, prmperc03);    //@@## Let op! in de functie PercentageVerlengGroenTijden(fc03, mperiod, PRM[prmperc03].....,
 *                                                                 //@@##         wordt de waarde van het percentage en niet de index meegegeven!!!
 */

void PercentageVerlengGroenTijdenISG(count fc, mulv percentage)
{
    TVG_max[fc] = (mulv)(((long)PRM[percentage] * (long)TVG_max[fc]) / 100);
}

/* HULPFUNCTIE T.B.V. MAXIMAAL MEEVERLENGEN BIJ WACHTSTAND GROEN */
/* ============================================================ */

/* hf_wsg_nlISG() is een hulpfunctie, die wordt gebruikt bij de specificatie van maximaal meeverlengen bij wachtstand groen.
 * hf_wsg_nl_ISG() geeft aan of er nog een fasecyclus wordt "afgewikkeld".
 * hf_wsg_nlISG() geeft als return-waarde waar (TRUE) indien er nog een fasecyclus wordt "afgewikkeld", anders niet waar (FALSE).
 * hf_wsg_nlISG() is gebaseerd op de werking van de functie bool hf_wsg(void), die is gedefinieerd in stdfunc.c
 *
 * De functie hf_wsg_nlISG() wordt aangeroepen vanuit de applicatiefunctie Meeverlengen().
 *
 * voorbeeld gebruik:
 * ------------------
 *  YM[fc02] |= SCH[schmv02] && ym_max_tig_Realisatietijd(fc02, prmmv02) && hf_wsg_nlISG() ? BIT4 : 0;
 *  YM[fc03] |= SCH[schmv03] && ym_max_tig_Realisatietijd(fc03, prmmv03) && hf_wsg_nlISG() ? BIT4 : 0;
 *
 */

bool hf_wsg_nlISG(void)
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

/* AFSLUITEN PRIMAIRE AANVRAAGGEBIED */
/* ================================= */

/* bool afsluiten_aanvraaggebied_prISG(bool* prml[], count ml)
 * kan worden gebruikt voor het afsluiten van het aanvraaggebied van de primaire fasecycli van de actieve module.
 * de PG[] van deze fasecycli worden gezet op het moment dat de laatste primaire fasecyclus met een aanvraag en die niet wordt geblokkeerd is gerealiseerd.
 * afsluiten_aanvraaggebied_prISG() geeft als return waarde waar (TRUE). //@PSN geeft geen return waarde FALSE; vreemd??
 * bool afsluiten_aanvraaggebied_prISG(bool* prml[], count ml) is afgeleid van de functie bool afsluiten_aanvraaggebied_pr(bool *prml[], count ml) in lwmlfunc.c.
 *
 * de functie afsluiten_aanvraaggebied_prISG() wordt aangeroepen in de functie RealisatieAfhandeling()

 * voorbeeld gebruik:
 * ------------------
 * afsluiten_aanvraaggebied_prISG(PRML, ML);*
 *
 *
 *
 */


void afsluiten_aanvraaggebied_prISG(bool* prml[], count ml)
{
    register count i;
    for (i = 0; i < FC_MAX; i++) {
        if ((prml[ml][i] & PRIMAIR) && !PG[i] && !A[i] && fka(i)) /* was && fkaa(i) */
            PG[i] |= PRIMAIR_OVERSLAG;
    }

}

/* BEPAAL VOLGRICHTINGEN */
/* --------------------- */
/* void BepaalVolgrichtingen(void)
 * bepaalt voor alle fasecycli op basis van de gedefinieerde nalopen (TNL_type[][]) of een fasecyclus ook een volgrichting is, en vult tabel Volgrichting[i].
 * de tabel Volgrichting[i] is gedefineerd in de file ISGFUNC.C.
 *
 * BepaalVolgrichtingen() wordt aangeroepen in applicatiefunctie de Meetkriterium().
 */

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

/* PRIO AANWEZIGHEID */
/* ----------------- */
/* void PrioAanwezig(void)
 * bepaalt voor alle fasecycli op basis van de gedefinieerde prioriteiten (iPrioriteit[]) of voor een fasecyclus prioriteit is opgegeven en vult tabel PRIOiFC[].
 * de tabel PRIOFC[] is gedefineerd in de file ISGFUNC.C.
 *
 * PrioAanwezig() wordt aangeroepen in de in applicatiefunctie de Wachtgroen.
 */

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

/* INIT INTERFUNC */
/* -------------- */
/* void InitInterfunc(void)
 * initialiseert voor alle fasecycli de nalooptypen (TNLP_type[][]) en de typen fictieve conflicten (FK_type[][]).
 *
 * InitInterfunc() wordt aangeroepen in de applicatiefunctie void control_parameters() voor de definitie van de typen nalopen en fictieve conflciten.
 *
 * voorbeeld:  InitInterfunc();
 *             TNL_type[fc02][fc62] = TNL_EG;
 *             TNL_type[fc22][fc21] = TNL_EVG;
 *             TNL_type[fc31][fc32] = TNL_SG;
 *
 *             FK_type[fc02][fc09] =  FK_EG; Het eindegroenmoment van richting 2 bepaalt het startgroenmoment van richting 9  
 *             FK_type[fc22][fc02] =  FK_EVG; Het eindeverlenggroenmoment van richting 22 bepaalt het startgroenmoment van richting 2. 
 *                                            Dit is als we 22 wel willen laten meeverlengen tijdens 2 bij bijv. een lange middenberm 
 *             FK_type[fc32][fc02] =  FK_SG; Het startgroenmoment van richting 32 bepaalt het startgroenmoment van richtng 2  
 */


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

/* ISGDEBUG */
/* -------- */
/* void IsgDebug (void)
 * schrijf in de testomgeving debug informatie over Realisatietijd, InterStartGroentijd en PAR naar het XY-Printf scherm.
 */

void IsgDebug()
{
#ifndef AUTOMAAT
    count x, y;
    xyprintf(30, 1, "Realisatietijd");
    //    xyprintf(38 + 4 * FCMAX, 1, "InterStartGroentijd");
    //    xyprintf(46 + 8 * FCMAX, 1, "PAR");
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
        xyprintf(34 + 4 * x, 5 + FCMAX, "%4d", twacht[x]);
    }
    for (x = 0; x < FCMAX; ++x)
    {
        xyprintf(34 + 4 * x, 6 + FCMAX, "%4d", twacht_wtv[x]);
    }
    xyprintf(36 + 4 * FCMAX, 4 + FCMAX, "twacht");
    for (y = 0; y < FCMAX; ++y)
    {
        xyprintf(46 + 8 * FCMAX, y + 4, "%2s", FC_code[y]);
        xyprintf(50 + 8 * FCMAX, y + 4, "%4d", max_par(y, twacht));
    }
#endif
}

/* OPHOGING TVG_max */
/* ---------------- */
/* Deze functie zorgt er voor dat de TVG_max van een richting aleen wordt aangepast op einde verlenggroen van de primaire realisatie en bij het starten van de regeling. 
 * Op deze manier wordt de maximale wachttijd van een conflicterende richting niet tussentijds verhoogd bij een wijziging van de primaire verlenggroentijd.     
 */

void IsgCorrectieTvgPrTvgMax()
{
    count fc;

    /* TVG_max alleen aanpassen op EVG van de primaire richting of tijdens initialisatie */
    for (fc = 0; fc < FCMAX; ++fc)
    {
        if (EVG[fc] && PR[fc] || init_tvg)
        {
            TVG_PR[fc] = TVG_max[fc];
        }
        else
        {
            TVG_max[fc] = TVG_PR[fc];
        }
    }
}

/* CORRIGEER TVG_max */
/* ----------------- */
/* Corrigeer TVG_timer[] i.r.t. TVG_max[] 
 */

void IsgCorrectieTvgTimerTvgMax()
{
    count fc;
    for (fc = 0; fc < FCMAX; ++fc)
    {
        if (MG[fc])  TVG_timer[fc] = TVG_max[fc];
        if (TVG_timer[fc] > TVG_max[fc]) TVG_timer[fc] = TVG_max[fc];
    }
}

/* INITIALISATIE INTERSTARTGROENTIJDEN t.b.v. de Robuuste Groentijdverdeler */
/* ------------------------------------------------------------------------ */
/* void InitInterStartGroenTijden_rgv(void) bepaalt de initiele waarden van de InterStartGroenTijden matrix TISG_rgv en zet alle waarden op NG (niet gebruikt).
 * de TISG_rgv dienen in de regelapplicatie te worden bepaald en gecorrigeerd voor o.a naloop richtingen, gelijkstarts, etc.
 *
 * In een TLCGen-regelapplicatie wordt InitInterStartGroenTijden_rgv(void) aangeroepen vanuit de applicatiefunctie void BepaalInterStartGroenTijden_rgv(void).
 * BepaalInterStartGroenTijden_rgv() wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door application().
 */

void InitInterStartGroenTijden_rgv()
{
    count i, j;
    for (i = 0; i < FC_MAX; i++) 
    {
        for (j = 0; j < FC_MAX; j++)
        {
            TISG_rgv[i][j] = NG;
            TISG_basis[i][j] = NG;
        }
    }
}

/*INTERSTARTGROENTIJDEN VUL HARDE CONFLICTEN IN t.b.v. de Robuuste Groentijdverdeler */
/* --------------------------------------------------------------------------------- */
/* void InterStartGroenTijden_VulHardeConflictenIn(void) vult de TISG_rgv[fc1][fc2] in voor alle harde conflicten met de vastgroentijden, verlengroentijden
 *
 * InterStartGroenTijden_VulHardeConflictenIn_rgv(void) wordt aangeroepen vanuit de applicatiefunctie void BepaalInterStartGroenTijden_rgv(void).
 * na de initialisatie van TISG_rgv.
 * BepaalInterStartGroenTijden_rgv(void) wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door application().
 *
 * // Bepaal InterStartGroenTijden
 * InitInterStartGroenTijden_rgv();
 * InterStartGroenTijden_VulHardeConflictenIn_rgv();
 * InterStartGroenTijden_VulGroenGroenConflictenIn_rgv();
 *
 */

void InterStartGroenTijden_VulHardeConflictenIn_rgv(void)
{
    count fc1, fc2, n;
    for (fc1 = 0; fc1 < FCMAX; ++fc1)
    {
        for (n = 0; n < KFC_MAX[fc1]; ++n)
        {
            fc2 = KF_pointer[fc1][n];
            TISG_rgv[fc1][fc2] = TFG_max[fc1] + TVG_rgv[fc1] + TIG_max[fc1][fc2];
            TISG_basis[fc1][fc2] = TFG_max[fc1] + TVG_basis[fc1] + TIG_max[fc1][fc2];
        }
    }
}

/* INTERSTARTGROENTIJDEN VUL GROEN-GROEN CONFLICTEN IN t.b.v. de Robuuste Groentijdverdeler */
/* ---------------------------------------------------------------------------------------- */
/* void InterStartGroenTijden_VulGroenGroenConflictenIn_rgv(void) vult de TISG_rgc[fc1][fc2] en TISG_AR[fc1][fc2] in voor alle Groen-Groen conflicten met de vastgroentijden
 * en verlengroentijden.
 *
 * InterStartGroenTijden_VulGroenGroenConflictenIn(void) wordt aangeroepen vanuit de applicatiefunctie void BepaalInterStartGroenTijden(void) na
 * de initialisatie van TISG_rgv[][] ie
 * BepaalInterStartGroenTijden(void) wordt aangeroepen vanuit de applicatiefunctie Verlenggroentijden(), die wordt aangeroepen door application().
 *
 * //Bepaal InterStartGroenTijden
 * InitInterStartGroenTijden();
 * InterStartGroenTijden_VulHardeConflictenIn();
 * InterStartGroenTijden_VulGroenGroenConflictenIn();
 */


void InterStartGroenTijden_VulGroenGroenConflictenIn_rgv(void)
{
    count fc1, fc2, n;
    for (fc1 = 0; fc1 < FCMAX; ++fc1)
    {
        for (n = KFC_MAX[fc1]; n < GKFC_MAX[fc1]; ++n)
        {
            fc2 = KF_pointer[fc1][n];
            TISG_rgv[fc1][fc2] = TFG_max[fc1] + TVG_rgv[fc1];
            TISG_basis[fc1][fc2] = TFG_max[fc1] + TVG_basis[fc1];
        }
    }
}

/* Zie InterStartGroenTijd_NLEG() maar dan als correcte t.b.v. TISG_rgv -matrix
 */

void InterStartGroenTijd_NLEG_rgv(count i, count j, count tnlfg, count tnlfgd, count tnleg, count tnlegd, count tvgnaloop)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!(tnlfg == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfgd == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnleg == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + TVG_rgv[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlegd == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + TVG_rgv[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfg == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfgd == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnleg == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + TVG_basis[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlegd == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + TVG_basis[i] + TGL_max[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
    }
    for (n = KFC_MAX[j]; n < GKFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!(tnlfg == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop]);
        if (!(tnlfgd == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnleg == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + TVG_rgv[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlegd == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + TVG_rgv[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfg == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop]);
        if (!(tnlfgd == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnleg == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + TVG_basis[i] + T_max[tnleg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlegd == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + TVG_basis[i] + T_max[tnlegd] + T_max[tvgnaloop] + TIG_max[j][k]);
    }
}

/* Zie InterStartGroenTijd_NLEVG() maar dan als correcte t.b.v. TISG_rgv-matrix
 */

void InterStartGroenTijd_NLEVG_rgv(count i, count j, count tnlfg, count tnlfgd, count tnlevg, count tnlevgd, count tvgnaloop)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!(tnlfg == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfgd == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevg == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + TVG_rgv[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevgd == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + TVG_rgv[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfg == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfgd == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevg == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + TVG_basis[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevgd == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + TVG_basis[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
    }
    for (n = KFC_MAX[j]; n < GKFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!(tnlfg == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfgd == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevg == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + TVG_rgv[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevgd == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], TFG_max[i] + TVG_rgv[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfg == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + T_max[tnlfg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlfgd == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + T_max[tnlfgd] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevg == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + TVG_basis[i] + T_max[tnlevg] + T_max[tvgnaloop] + TIG_max[j][k]);
        if (!(tnlevgd == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], TFG_max[i] + TVG_basis[i] + T_max[tnlevgd] + T_max[tvgnaloop] + TIG_max[j][k]);
    }
}

/* Zie InterStartGroenTijd_NLSG() maar dan als correcte t.b.v. TISG_rgv-matrix
 */

void InterStartGroenTijd_NLSG_rgv(count i, count j, count tnlsg, count tnlsgd)
{
    int k, n;
    for (n = 0; n < KFC_MAX[j]; n++)
    {
        k = KF_pointer[j][n];
        if (!(tnlsg == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], T_max[tnlsg] + TIG_max[j][k]);
        if (!(tnlsgd == NG)) TISG_rgv[i][k] = max(TISG_rgv[i][k], T_max[tnlsgd] + TIG_max[j][k]);
        if (!(tnlsg == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], T_max[tnlsg] + TIG_max[j][k]);
        if (!(tnlsgd == NG)) TISG_basis[i][k] = max(TISG_basis[i][k], T_max[tnlsgd] + TIG_max[j][k]);
    }
}

/* Zie InterStartGroentijd_MeeverlengenDeelconflict() maar dan als correcte t.b.v. TISG_rgv-matrix
 */

void InterStartGroentijd_MeeverlengenDeelconflict_rgv(mulv fc1, mulv fc2)
{
    count fc;
    for (fc = 0; fc < FCMAX; fc++)
    {
        if (TIGR[fc2][fc] >= 0)
        {
            TISG_rgv[fc1][fc] = max(TISG_rgv[fc1][fc], TFG_max[fc1] + TVG_rgv[fc1] + TIGR[fc2][fc]);
            TISG_basis[fc1][fc] = max(TISG_basis[fc1][fc], TFG_max[fc1] + TVG_basis[fc1] + TIGR[fc2][fc]);
        }
    }
}

/* Zie Correctie_TISG_Voorstart() maar dan als correcte t.b.v. TISG_rgv-matrix
 */

bool Correctie_TISG_Voorstart_rgv(count fcvs, count fcns, count tvs)
{
    count n;
    bool result = FALSE;
    for (n = 0; n < FCMAX; ++n)
    {
        if ((TISG_rgv[n][fcns] < TISG_rgv[n][fcvs] + T_max[tvs]) && (TISG_rgv[n][fcvs] > 0))
        {
            TISG_rgv[n][fcns] = TISG_rgv[n][fcvs] + T_max[tvs];
            result = TRUE;
        }
        if ((TISG_basis[n][fcns] < TISG_basis[n][fcvs] + T_max[tvs]) && (TISG_basis[n][fcvs] > 0))
        {
            TISG_basis[n][fcns] = TISG_basis[n][fcvs] + T_max[tvs];
            result = TRUE;
        }
    }
    return result;
}

/* Zie Correctie_TISG_Gelijkstart() maar dan als correcte t.b.v. TISG_rgv-matrix
 */

bool Correctie_TISG_Gelijkstart_rgv(count fc1, count fc2)
{
    count n;
    bool result = FALSE;
    for (n = 0; n < FCMAX; ++n)
    {
        if (TISG_rgv[n][fc1] < TISG_rgv[n][fc2])
        {
            TISG_rgv[n][fc1] = TISG_rgv[n][fc2];
            result = TRUE;
        }
        else
        {
            if (TISG_rgv[n][fc1] != TISG_rgv[n][fc2])
            {
                TISG_rgv[n][fc2] = TISG_rgv[n][fc1];
                result = TRUE;
            }
        }
        if (TISG_basis[n][fc1] < TISG_basis[n][fc2])
        {
            TISG_basis[n][fc1] = TISG_basis[n][fc2];
            result = TRUE;
        }
        else
        {
            if (TISG_basis[n][fc1] != TISG_basis[n][fc2])
            {
                TISG_basis[n][fc2] = TISG_basis[n][fc1];
                result = TRUE;
            }
        }
    }
    return result;
}

/* Zie Correctie_TISG_LateReleas() maar dan als correcte t.b.v. TISG_rgv-matrix
 */

bool Correctie_TISG_LateRelease_rgv(count fclr, count fcvs, count txnl)
{
    count n;
    bool result = FALSE;
    for (n = 0; n < FCMAX; ++n)
    {
        if (TISG_rgv[n][fcvs] < TISG_rgv[n][fclr] - T_max[txnl])
        {
            TISG_rgv[n][fcvs] = TISG_rgv[n][fclr] - T_max[txnl];
            result = TRUE;
        }
        if (TISG_basis[n][fcvs] < TISG_basis[n][fclr] - T_max[txnl])
        {
            TISG_basis[n][fcvs] = TISG_basis[n][fclr] - T_max[txnl];
            result = TRUE;
        }
    }
    return result;
}

/* Resetten van de naloopbits
 */

void ResetNaloopBits() 
{
    int fc;
    for (fc = 0; fc < FCMAX; ++fc)
    {
        RW[fc] &= ~BIT2;
        YV[fc] &= ~BIT2;
        YM[fc] &= ~BIT2;
    }
}



/* Realisatie fasecyclus mag niet meer worden tegengehouden doordat het aantal uitgestuurde ledjes een minimaal aantal bereikt heeft.
 * Daardoor mag de tijd tot conflictende groen van de conlflictrictingen (fc) al opgehoogd wroden met de tijd tot groen van deze richting i 
 */

void Realisatietijd_wtv_correctie(count i, count mwtv, count prmwtvhaltmin)
{
   count fc, prio, n;
   if ((MM[mwtv] < PRM[prmwtvhaltmin]) && (MM[mwtv] > 0))
   {
      Bepaal_Realisatietijd_voor_richting(i); /* bepaal de maximale realisatietijd voor deze richting */
      for (n = 0; n < FKFC_MAX[i]; ++n)
      {
         fc = KF_pointer[i][n];
         if (BR[i]) REALISATIETIJD[i][fc] = ((REALISATIETIJD_max[i] + TISG_BR[i][fc]) > REALISATIETIJD[i][fc]) ? (REALISATIETIJD_max[i] + TISG_BR[i][fc]) : REALISATIETIJD[i][fc];
         if (AR[i]) REALISATIETIJD[i][fc] = ((REALISATIETIJD_max[i] + TISG_AR[i][fc]) > REALISATIETIJD[i][fc]) ? (REALISATIETIJD_max[i] + TISG_AR[i][fc]) : REALISATIETIJD[i][fc];
         if (PR[i]) REALISATIETIJD[i][fc] = ((REALISATIETIJD_max[i] + TISG_PR[i][fc]) > REALISATIETIJD[i][fc]) ? (REALISATIETIJD_max[i] + TISG_PR[i][fc]) : REALISATIETIJD[i][fc];
         for (prio = 0; prio < prioFCMAX; ++prio)
         {
            if (iPrioriteit[prio] && (iPrioriteitsOpties[prio] & poAfkappenKonfliktRichtingen) && (fc == iFC_PRIOix[prio]))
            {
               REALISATIETIJD[i][fc] = ((REALISATIETIJD_max[i] + TISG_afkap[i][fc]) > REALISATIETIJD[i][fc]) ? (REALISATIETIJD_max[i] + TISG_afkap[i][fc]) : REALISATIETIJD[i][fc];
            }
         }
      }
   }
}

/* @PSN Commentaar
 */
bool Realisatietijd_Lokgroen_Correctie(count fc1, count fc2)
{
    count n;
    bool result = FALSE;
    for (n = 0; n < FCMAX; ++n)
    {
        if (REALISATIETIJD[n][fc1] < REALISATIETIJD[n][fc2])
        {
            REALISATIETIJD[n][fc1] = REALISATIETIJD[n][fc2];
            result = TRUE;
        }
    }
    return result;
}

/* @PSN Commentaar
 */
bool Realisatietijd_Lokgroen_Correctie_wtv(count fc1, count fc2)
{
    count n;
    bool result = FALSE;
    for (n = 0; n < FCMAX; ++n)
    {
        if (REALISATIETIJD_wtv[n][fc1] < REALISATIETIJD_wtv[n][fc2])
        {
            REALISATIETIJD_wtv[n][fc1] = REALISATIETIJD_wtv[n][fc2];
            result = TRUE;
        }
    }
    return result;
}

/* @PSN Commentaar
 */
bool TISG_Lokgroen_Correctie(count fc1, count fc2)
{
    count n;
    bool result = FALSE;
    for (n = 0; n < FCMAX; ++n)
    {
        if (TISG_PR[n][fc1] < TISG_PR[n][fc2])
        {
            TISG_PR[n][fc1] = TISG_PR[n][fc2];
            result = TRUE;
        }
    }
    for (n = 0; n < FCMAX; ++n)
    {
        if (TISG_AR[n][fc1] < TISG_AR[n][fc2])
        {
            TISG_AR[n][fc1] = TISG_AR[n][fc2];
            result = TRUE;
        }
    }
    return result;
}

/* @PSN Commentaar
 */
bool TISG_Lokgroen_Correctie_rgv(count fc1, count fc2)
{
    count n;
    bool result = FALSE;
    for (n = 0; n < FCMAX; ++n)
    {
        if (TISG_rgv[n][fc1] < TISG_rgv[n][fc2])
        {
            TISG_rgv[n][fc1] = TISG_rgv[n][fc2];
            result = TRUE;
        }
    }
    for (n = 0; n < FCMAX; ++n)
    {
        if (TISG_basis[n][fc1] < TISG_basis[n][fc2])
        {
            TISG_basis[n][fc1] = TISG_basis[n][fc2];
            result = TRUE;
        }
    }
    return result;
}

/* Kopieer de realisatijd in een nieuwe matrix */
void InitRealisatieTijdenWtv(void) {
    count i, j;
    for (i = 0; i < FCMAX; ++i)
    {
        for (j = 0; j < FCMAX; ++j)
        {
            REALISATIETIJD_wtv[i][j] = REALISATIETIJD[i][j];
        }
    }
}