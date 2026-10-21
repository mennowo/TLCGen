/* SIMULATIE APPLICATIE */
/* -------------------- */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMsim.c
      CCOL:   11.0
    TLCGEN:   
   CCOLGEN:   UnitTest
*/

/****************************** Versie commentaar ***********************************
 *
 * Versie   Datum   Ontwerper   Commentaar
 *
 ************************************************************************************/

#define LNKMAX1 14 /* aantal links */

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

    LNK_code[6] = "411";
    IS_nr[6] = d411;
    FC_nr[6] = NG;
    S_generator[6] = NG;
    S_stopline[6] = 0;
    Q1[6] = 0;
    Q2[6] = 0;
    Q3[6] = 0;
    Q4[6] = 0;

    LNK_code[7] = "412";
    IS_nr[7] = d412;
    FC_nr[7] = NG;
    S_generator[7] = NG;
    S_stopline[7] = 0;
    Q1[7] = 0;
    Q2[7] = 0;
    Q3[7] = 0;
    Q4[7] = 0;

#if (!defined AUTOMAAT_TEST)
    LNK_code[8] = "dummykarin41bus";
    IS_nr[8] = ddummykarin41bus;
    FC_nr[8] = NG;
    S_generator[8] = NG;
    S_stopline[8] = 0;
    Q1[8] = 0;
    Q2[8] = 0;
    Q3[8] = 0;
    Q4[8] = 0;

    LNK_code[9] = "dummykaruit41bus";
    IS_nr[9] = ddummykaruit41bus;
    FC_nr[9] = NG;
    S_generator[9] = NG;
    S_stopline[9] = 0;
    Q1[9] = 0;
    Q2[9] = 0;
    Q3[9] = 0;
    Q4[9] = 0;

    LNK_code[10] = "dummyhdkarin02";
    IS_nr[10] = ddummyhdkarin02;
    FC_nr[10] = NG;
    S_generator[10] = NG;
    S_stopline[10] = 0;
    Q1[10] = 0;
    Q2[10] = 0;
    Q3[10] = 0;
    Q4[10] = 0;

    LNK_code[11] = "dummyhdkaruit02";
    IS_nr[11] = ddummyhdkaruit02;
    FC_nr[11] = NG;
    S_generator[11] = NG;
    S_stopline[11] = 0;
    Q1[11] = 0;
    Q2[11] = 0;
    Q3[11] = 0;
    Q4[11] = 0;

    LNK_code[12] = "dummyhdkarin05";
    IS_nr[12] = ddummyhdkarin05;
    FC_nr[12] = NG;
    S_generator[12] = NG;
    S_stopline[12] = 0;
    Q1[12] = 0;
    Q2[12] = 0;
    Q3[12] = 0;
    Q4[12] = 0;

    LNK_code[13] = "dummyhdkaruit05";
    IS_nr[13] = ddummyhdkaruit05;
    FC_nr[13] = NG;
    S_generator[13] = NG;
    S_stopline[13] = 0;
    Q1[13] = 0;
    Q2[13] = 0;
    Q3[13] = 0;
    Q4[13] = 0;

#endif

    /* Gebruikers toevoegingen file includen */
    /* ------------------------------------- */
    #include "GMsim.add"

}
