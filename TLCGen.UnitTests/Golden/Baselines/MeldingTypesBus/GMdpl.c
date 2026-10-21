/* DISPLAY APPLICATIE */
/* ------------------ */

/* KRUISPUNT: 
              GM
              
              

   BESTAND:   GMdpl.c
      CCOL:   10.0
    TLCGEN:   
   CCOLGEN:   UnitTest
*/

/****************************** Versie commentaar ***********************************
 *
 * Versie   Datum   Ontwerper   Commentaar
 *
 ************************************************************************************/

/* aantal ingangs-/uitgangs signalen */
/* --------------------------------- */

    #define USDPLMAX (USMAX + 0)
    #define ISDPLMAX (ISMAX + 0)

/* include files */
/* ------------- */
    #include "sysdef.c"
    #include "GMsys.h"
    #include "dplwvar.c"

void display_background(void)
{
    load_picture_bmp("");
}

void display_parameters(void)
{
    /* fasecycli */
    /* --------- */
    X_us[fc02] = NG; Y_us[fc02] = NG;
    X_us[fc05] = NG; Y_us[fc05] = NG;
    X_us[fc41] = NG; Y_us[fc41] = NG;
    X_us[fc42] = NG; Y_us[fc42] = NG;

    /* detectie */
    /* -------- */
    X_is[d021] = NG; Y_is[d021] = NG;
    X_is[d022] = NG; Y_is[d022] = NG;
    X_is[d051] = NG; Y_is[d051] = NG;
    X_is[d052] = NG; Y_is[d052] = NG;
    X_is[d411] = NG; Y_is[d411] = NG;
    X_is[d412] = NG; Y_is[d412] = NG;
    X_is[d413] = NG; Y_is[d413] = NG;
    X_is[d414] = NG; Y_is[d414] = NG;
    X_is[d421] = NG; Y_is[d421] = NG;
    X_is[d422] = NG; Y_is[d422] = NG;
    X_is[dsd41in] = NG; Y_is[dsd41in] = NG;
    X_is[dsd41uit] = NG; Y_is[dsd41uit] = NG;
#if (!defined AUTOMAAT_TEST)
    X_is[ddummykarin41bus] = NG; Y_is[ddummykarin41bus] = NG;
    X_is[ddummykaruit41bus] = NG; Y_is[ddummykaruit41bus] = NG;
#endif

    /* overige uitgangen */
    /* ----------------- */
    X_us[ussegm1] = NG; Y_us[ussegm1] = NG;
    X_us[ussegm2] = NG; Y_us[ussegm2] = NG;
    X_us[ussegm3] = NG; Y_us[ussegm3] = NG;
    X_us[ussegm4] = NG; Y_us[ussegm4] = NG;
    X_us[ussegm5] = NG; Y_us[ussegm5] = NG;
    X_us[ussegm6] = NG; Y_us[ussegm6] = NG;
    X_us[ussegm7] = NG; Y_us[ussegm7] = NG;
    X_us[usmaxwt] = NG; Y_us[usmaxwt] = NG;
    X_us[uskarmelding] = NG; Y_us[uskarmelding] = NG;
    X_us[uskarog] = NG; Y_us[uskarog] = NG;
    X_us[usovinm41bus] = NG; Y_us[usovinm41bus] = NG;
    X_us[usovinm42bus] = NG; Y_us[usovinm42bus] = NG;
    X_us[usperdef] = NG; Y_us[usperdef] = NG;
    X_us[usper1] = NG; Y_us[usper1] = NG;

    /* overige ingangen */
    /* ---------------- */
    X_is[isov41in] = NG; Y_is[isov41in] = NG;
    X_is[isov41uit] = NG; Y_is[isov41uit] = NG;

    /* Gebruikers toevoegingen file includen */
    /* ------------------------------------- */
    #include "GMdpl.add"

}
