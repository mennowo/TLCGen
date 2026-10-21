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
    X_us[fc22] = NG; Y_us[fc22] = NG;
    X_us[fc24] = NG; Y_us[fc24] = NG;
    X_us[fc31] = NG; Y_us[fc31] = NG;

    /* detectie */
    /* -------- */
    X_is[d021] = NG; Y_is[d021] = NG;
    X_is[d022] = NG; Y_is[d022] = NG;
    X_is[d221] = NG; Y_is[d221] = NG;
    X_is[d222] = NG; Y_is[d222] = NG;
    X_is[dk22] = NG; Y_is[dk22] = NG;
    X_is[d241] = NG; Y_is[d241] = NG;
    X_is[d242] = NG; Y_is[d242] = NG;
    X_is[dk24] = NG; Y_is[dk24] = NG;
    X_is[d311] = NG; Y_is[d311] = NG;
    X_is[d312] = NG; Y_is[d312] = NG;

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
    X_us[usovinm22fiets] = NG; Y_us[usovinm22fiets] = NG;
    X_us[usovinm24fiets] = NG; Y_us[usovinm24fiets] = NG;
    X_us[usperdef] = NG; Y_us[usperdef] = NG;
    X_us[usper1] = NG; Y_us[usper1] = NG;

    /* overige ingangen */
    /* ---------------- */

    /* Gebruikers toevoegingen file includen */
    /* ------------------------------------- */
    #include "GMdpl.add"

}
