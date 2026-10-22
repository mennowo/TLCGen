/* SIMULATIE APPLICATIE */
/* -------------------- */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMsim.c
      CCOL:   10.0
    TLCGEN:   
   CCOLGEN:   UnitTest
*/

/****************************** Versie commentaar ***********************************
 *
 * Versie   Datum   Ontwerper   Commentaar
 *
 ************************************************************************************/

#define LNKMAX1 21 /* aantal links */

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

    LNK_code[4] = "081";
    IS_nr[4] = d081;
    FC_nr[4] = NG;
    S_generator[4] = NG;
    S_stopline[4] = 0;
    Q1[4] = 0;
    Q2[4] = 0;
    Q3[4] = 0;
    Q4[4] = 0;

    LNK_code[5] = "082";
    IS_nr[5] = d082;
    FC_nr[5] = NG;
    S_generator[5] = NG;
    S_stopline[5] = 0;
    Q1[5] = 0;
    Q2[5] = 0;
    Q3[5] = 0;
    Q4[5] = 0;

    LNK_code[6] = "221";
    IS_nr[6] = d221;
    FC_nr[6] = NG;
    S_generator[6] = NG;
    S_stopline[6] = 0;
    Q1[6] = 0;
    Q2[6] = 0;
    Q3[6] = 0;
    Q4[6] = 0;

    LNK_code[7] = "222";
    IS_nr[7] = d222;
    FC_nr[7] = NG;
    S_generator[7] = NG;
    S_stopline[7] = 0;
    Q1[7] = 0;
    Q2[7] = 0;
    Q3[7] = 0;
    Q4[7] = 0;

    LNK_code[8] = "k22";
    IS_nr[8] = dk22;
    FC_nr[8] = NG;
    S_generator[8] = NG;
    S_stopline[8] = 0;
    Q1[8] = 0;
    Q2[8] = 0;
    Q3[8] = 0;
    Q4[8] = 0;

    LNK_code[9] = "311";
    IS_nr[9] = d311;
    FC_nr[9] = NG;
    S_generator[9] = NG;
    S_stopline[9] = 0;
    Q1[9] = 0;
    Q2[9] = 0;
    Q3[9] = 0;
    Q4[9] = 0;

    LNK_code[10] = "312";
    IS_nr[10] = d312;
    FC_nr[10] = NG;
    S_generator[10] = NG;
    S_stopline[10] = 0;
    Q1[10] = 0;
    Q2[10] = 0;
    Q3[10] = 0;
    Q4[10] = 0;

    LNK_code[11] = "411";
    IS_nr[11] = d411;
    FC_nr[11] = NG;
    S_generator[11] = NG;
    S_stopline[11] = 0;
    Q1[11] = 0;
    Q2[11] = 0;
    Q3[11] = 0;
    Q4[11] = 0;

    LNK_code[12] = "412";
    IS_nr[12] = d412;
    FC_nr[12] = NG;
    S_generator[12] = NG;
    S_stopline[12] = 0;
    Q1[12] = 0;
    Q2[12] = 0;
    Q3[12] = 0;
    Q4[12] = 0;

#if (!defined AUTOMAAT_TEST)
    LNK_code[13] = "dummykarin02hd";
    IS_nr[13] = ddummykarin02hd;
    FC_nr[13] = NG;
    S_generator[13] = NG;
    S_stopline[13] = 0;
    Q1[13] = 0;
    Q2[13] = 0;
    Q3[13] = 0;
    Q4[13] = 0;

    LNK_code[14] = "dummykarin22hd";
    IS_nr[14] = ddummykarin22hd;
    FC_nr[14] = NG;
    S_generator[14] = NG;
    S_stopline[14] = 0;
    Q1[14] = 0;
    Q2[14] = 0;
    Q3[14] = 0;
    Q4[14] = 0;

    LNK_code[15] = "dummykarin41bus";
    IS_nr[15] = ddummykarin41bus;
    FC_nr[15] = NG;
    S_generator[15] = NG;
    S_stopline[15] = 0;
    Q1[15] = 0;
    Q2[15] = 0;
    Q3[15] = 0;
    Q4[15] = 0;

    LNK_code[16] = "dummykaruit02hd";
    IS_nr[16] = ddummykaruit02hd;
    FC_nr[16] = NG;
    S_generator[16] = NG;
    S_stopline[16] = 0;
    Q1[16] = 0;
    Q2[16] = 0;
    Q3[16] = 0;
    Q4[16] = 0;

    LNK_code[17] = "dummykaruit22hd";
    IS_nr[17] = ddummykaruit22hd;
    FC_nr[17] = NG;
    S_generator[17] = NG;
    S_stopline[17] = 0;
    Q1[17] = 0;
    Q2[17] = 0;
    Q3[17] = 0;
    Q4[17] = 0;

    LNK_code[18] = "dummykaruit41bus";
    IS_nr[18] = ddummykaruit41bus;
    FC_nr[18] = NG;
    S_generator[18] = NG;
    S_stopline[18] = 0;
    Q1[18] = 0;
    Q2[18] = 0;
    Q3[18] = 0;
    Q4[18] = 0;

    LNK_code[19] = "dummyhdkarin08";
    IS_nr[19] = ddummyhdkarin08;
    FC_nr[19] = NG;
    S_generator[19] = NG;
    S_stopline[19] = 0;
    Q1[19] = 0;
    Q2[19] = 0;
    Q3[19] = 0;
    Q4[19] = 0;

    LNK_code[20] = "dummyhdkaruit08";
    IS_nr[20] = ddummyhdkaruit08;
    FC_nr[20] = NG;
    S_generator[20] = NG;
    S_stopline[20] = 0;
    Q1[20] = 0;
    Q2[20] = 0;
    Q3[20] = 0;
    Q4[20] = 0;

#endif

    /* Gebruikers toevoegingen file includen */
    /* ------------------------------------- */
    #include "GMsim.add"

}
