/* SIMULATIE APPLICATIE */
/* -------------------- */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMsim.c
      CCOL:   9.0
    TLCGEN:   
   CCOLGEN:   UnitTest
*/

/****************************** Versie commentaar ***********************************
 *
 * Versie   Datum   Ontwerper   Commentaar
 *
 ************************************************************************************/

#define LNKMAX1 29 /* aantal links */

/* include files */
/* ------------- */
    #include "GMsys.h"
    #include "simvar.c" /* simulatie variabelen */

/* Default waarden */
/* --------------- */
void simulation_defaults(void)
{
    S_defgenerator = NG;
    S_defstopline  = 1800;
    Q1_def         = 25;
    Q2_def         = 50;
    Q3_def         = 100;
    Q4_def         = 200;
}

void simulation_parameters(void)
{
 T_subrun_max  = 3600; /* subrun tijd                 */
 RANDGEN_start = 123;  /* startwaarde randomgenerator */

    /* Link parameters */
    /* --------------- */
    LNK_code[0] = "021";
    IS_nr[0] = d021;
    FC_nr[0] = NG;
    S_generator[0] = NG;
    S_stopline[0] = 0;
    Q1[0] = 0;
    Q2[0] = 0;
    Q3[0] = 0;
    Q4[0] = 0;

    LNK_code[1] = "022";
    IS_nr[1] = d022;
    FC_nr[1] = NG;
    S_generator[1] = NG;
    S_stopline[1] = 0;
    Q1[1] = 0;
    Q2[1] = 0;
    Q3[1] = 0;
    Q4[1] = 0;

    LNK_code[2] = "051";
    IS_nr[2] = d051;
    FC_nr[2] = NG;
    S_generator[2] = NG;
    S_stopline[2] = 0;
    Q1[2] = 0;
    Q2[2] = 0;
    Q3[2] = 0;
    Q4[2] = 0;

    LNK_code[3] = "052";
    IS_nr[3] = d052;
    FC_nr[3] = NG;
    S_generator[3] = NG;
    S_stopline[3] = 0;
    Q1[3] = 0;
    Q2[3] = 0;
    Q3[3] = 0;
    Q4[3] = 0;

    LNK_code[4] = "221";
    IS_nr[4] = d221;
    FC_nr[4] = NG;
    S_generator[4] = NG;
    S_stopline[4] = 0;
    Q1[4] = 0;
    Q2[4] = 0;
    Q3[4] = 0;
    Q4[4] = 0;

    LNK_code[5] = "222";
    IS_nr[5] = d222;
    FC_nr[5] = NG;
    S_generator[5] = NG;
    S_stopline[5] = 0;
    Q1[5] = 0;
    Q2[5] = 0;
    Q3[5] = 0;
    Q4[5] = 0;

    LNK_code[6] = "k22";
    IS_nr[6] = dk22;
    FC_nr[6] = NG;
    S_generator[6] = NG;
    S_stopline[6] = 0;
    Q1[6] = 0;
    Q2[6] = 0;
    Q3[6] = 0;
    Q4[6] = 0;

    LNK_code[7] = "411";
    IS_nr[7] = d411;
    FC_nr[7] = NG;
    S_generator[7] = NG;
    S_stopline[7] = 0;
    Q1[7] = 0;
    Q2[7] = 0;
    Q3[7] = 0;
    Q4[7] = 0;

    LNK_code[8] = "412";
    IS_nr[8] = d412;
    FC_nr[8] = NG;
    S_generator[8] = NG;
    S_stopline[8] = 0;
    Q1[8] = 0;
    Q2[8] = 0;
    Q3[8] = 0;
    Q4[8] = 0;

    LNK_code[9] = "421";
    IS_nr[9] = d421;
    FC_nr[9] = NG;
    S_generator[9] = NG;
    S_stopline[9] = 0;
    Q1[9] = 0;
    Q2[9] = 0;
    Q3[9] = 0;
    Q4[9] = 0;

    LNK_code[10] = "422";
    IS_nr[10] = d422;
    FC_nr[10] = NG;
    S_generator[10] = NG;
    S_stopline[10] = 0;
    Q1[10] = 0;
    Q2[10] = 0;
    Q3[10] = 0;
    Q4[10] = 0;

    LNK_code[11] = "611";
    IS_nr[11] = d611;
    FC_nr[11] = NG;
    S_generator[11] = NG;
    S_stopline[11] = 0;
    Q1[11] = 0;
    Q2[11] = 0;
    Q3[11] = 0;
    Q4[11] = 0;

    LNK_code[12] = "612";
    IS_nr[12] = d612;
    FC_nr[12] = NG;
    S_generator[12] = NG;
    S_stopline[12] = 0;
    Q1[12] = 0;
    Q2[12] = 0;
    Q3[12] = 0;
    Q4[12] = 0;

#if (!defined AUTOMAAT_TEST)
    LNK_code[13] = "dummykarin41tram";
    IS_nr[13] = ddummykarin41tram;
    FC_nr[13] = NG;
    S_generator[13] = NG;
    S_stopline[13] = 0;
    Q1[13] = 0;
    Q2[13] = 0;
    Q3[13] = 0;
    Q4[13] = 0;

    LNK_code[14] = "dummykarin41bus";
    IS_nr[14] = ddummykarin41bus;
    FC_nr[14] = NG;
    S_generator[14] = NG;
    S_stopline[14] = 0;
    Q1[14] = 0;
    Q2[14] = 0;
    Q3[14] = 0;
    Q4[14] = 0;

    LNK_code[15] = "dummykarin42bus";
    IS_nr[15] = ddummykarin42bus;
    FC_nr[15] = NG;
    S_generator[15] = NG;
    S_stopline[15] = 0;
    Q1[15] = 0;
    Q2[15] = 0;
    Q3[15] = 0;
    Q4[15] = 0;

    LNK_code[16] = "dummykarin22fiets";
    IS_nr[16] = ddummykarin22fiets;
    FC_nr[16] = NG;
    S_generator[16] = NG;
    S_stopline[16] = 0;
    Q1[16] = 0;
    Q2[16] = 0;
    Q3[16] = 0;
    Q4[16] = 0;

    LNK_code[17] = "dummykarin02vrw";
    IS_nr[17] = ddummykarin02vrw;
    FC_nr[17] = NG;
    S_generator[17] = NG;
    S_stopline[17] = 0;
    Q1[17] = 0;
    Q2[17] = 0;
    Q3[17] = 0;
    Q4[17] = 0;

    LNK_code[18] = "dummykarin05auto";
    IS_nr[18] = ddummykarin05auto;
    FC_nr[18] = NG;
    S_generator[18] = NG;
    S_stopline[18] = 0;
    Q1[18] = 0;
    Q2[18] = 0;
    Q3[18] = 0;
    Q4[18] = 0;

    LNK_code[19] = "dummykarin05ng";
    IS_nr[19] = ddummykarin05ng;
    FC_nr[19] = NG;
    S_generator[19] = NG;
    S_stopline[19] = 0;
    Q1[19] = 0;
    Q2[19] = 0;
    Q3[19] = 0;
    Q4[19] = 0;

    LNK_code[20] = "dummykarin61hd";
    IS_nr[20] = ddummykarin61hd;
    FC_nr[20] = NG;
    S_generator[20] = NG;
    S_stopline[20] = 0;
    Q1[20] = 0;
    Q2[20] = 0;
    Q3[20] = 0;
    Q4[20] = 0;

    LNK_code[21] = "dummykaruit41tram";
    IS_nr[21] = ddummykaruit41tram;
    FC_nr[21] = NG;
    S_generator[21] = NG;
    S_stopline[21] = 0;
    Q1[21] = 0;
    Q2[21] = 0;
    Q3[21] = 0;
    Q4[21] = 0;

    LNK_code[22] = "dummykaruit41bus";
    IS_nr[22] = ddummykaruit41bus;
    FC_nr[22] = NG;
    S_generator[22] = NG;
    S_stopline[22] = 0;
    Q1[22] = 0;
    Q2[22] = 0;
    Q3[22] = 0;
    Q4[22] = 0;

    LNK_code[23] = "dummykaruit42bus";
    IS_nr[23] = ddummykaruit42bus;
    FC_nr[23] = NG;
    S_generator[23] = NG;
    S_stopline[23] = 0;
    Q1[23] = 0;
    Q2[23] = 0;
    Q3[23] = 0;
    Q4[23] = 0;

    LNK_code[24] = "dummykaruit22fiets";
    IS_nr[24] = ddummykaruit22fiets;
    FC_nr[24] = NG;
    S_generator[24] = NG;
    S_stopline[24] = 0;
    Q1[24] = 0;
    Q2[24] = 0;
    Q3[24] = 0;
    Q4[24] = 0;

    LNK_code[25] = "dummykaruit02vrw";
    IS_nr[25] = ddummykaruit02vrw;
    FC_nr[25] = NG;
    S_generator[25] = NG;
    S_stopline[25] = 0;
    Q1[25] = 0;
    Q2[25] = 0;
    Q3[25] = 0;
    Q4[25] = 0;

    LNK_code[26] = "dummykaruit05auto";
    IS_nr[26] = ddummykaruit05auto;
    FC_nr[26] = NG;
    S_generator[26] = NG;
    S_stopline[26] = 0;
    Q1[26] = 0;
    Q2[26] = 0;
    Q3[26] = 0;
    Q4[26] = 0;

    LNK_code[27] = "dummykaruit05ng";
    IS_nr[27] = ddummykaruit05ng;
    FC_nr[27] = NG;
    S_generator[27] = NG;
    S_stopline[27] = 0;
    Q1[27] = 0;
    Q2[27] = 0;
    Q3[27] = 0;
    Q4[27] = 0;

    LNK_code[28] = "dummykaruit61hd";
    IS_nr[28] = ddummykaruit61hd;
    FC_nr[28] = NG;
    S_generator[28] = NG;
    S_stopline[28] = 0;
    Q1[28] = 0;
    Q2[28] = 0;
    Q3[28] = 0;
    Q4[28] = 0;

#endif

    /* Gebruikers toevoegingen file includen */
    /* ------------------------------------- */
    #include "GMsim.add"

}
