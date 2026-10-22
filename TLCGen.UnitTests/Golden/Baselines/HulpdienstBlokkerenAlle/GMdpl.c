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
    X_us[fc08] = NG; Y_us[fc08] = NG;
    X_us[fc22] = NG; Y_us[fc22] = NG;
    X_us[fc31] = NG; Y_us[fc31] = NG;
    X_us[fc41] = NG; Y_us[fc41] = NG;

    /* detectie */
    /* -------- */
    X_is[d021] = NG; Y_is[d021] = NG;
    X_is[d022] = NG; Y_is[d022] = NG;
    X_is[d051] = NG; Y_is[d051] = NG;
    X_is[d052] = NG; Y_is[d052] = NG;
    X_is[d081] = NG; Y_is[d081] = NG;
    X_is[d082] = NG; Y_is[d082] = NG;
    X_is[d221] = NG; Y_is[d221] = NG;
    X_is[d222] = NG; Y_is[d222] = NG;
    X_is[dk22] = NG; Y_is[dk22] = NG;
    X_is[d311] = NG; Y_is[d311] = NG;
    X_is[d312] = NG; Y_is[d312] = NG;
    X_is[d411] = NG; Y_is[d411] = NG;
    X_is[d412] = NG; Y_is[d412] = NG;
#if (!defined AUTOMAAT_TEST)
    X_is[ddummykarin02hd] = NG; Y_is[ddummykarin02hd] = NG;
    X_is[ddummykarin22hd] = NG; Y_is[ddummykarin22hd] = NG;
    X_is[ddummykarin41bus] = NG; Y_is[ddummykarin41bus] = NG;
    X_is[ddummykaruit02hd] = NG; Y_is[ddummykaruit02hd] = NG;
    X_is[ddummykaruit22hd] = NG; Y_is[ddummykaruit22hd] = NG;
    X_is[ddummykaruit41bus] = NG; Y_is[ddummykaruit41bus] = NG;
    X_is[ddummyhdkarin08] = NG; Y_is[ddummyhdkarin08] = NG;
    X_is[ddummyhdkaruit08] = NG; Y_is[ddummyhdkaruit08] = NG;
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
    X_us[usovinm02hd] = NG; Y_us[usovinm02hd] = NG;
    X_us[usovinm22hd] = NG; Y_us[usovinm22hd] = NG;
    X_us[usovinm41bus] = NG; Y_us[usovinm41bus] = NG;
    X_us[ushdinm08] = NG; Y_us[ushdinm08] = NG;
    X_us[usperdef] = NG; Y_us[usperdef] = NG;
    X_us[usper1] = NG; Y_us[usper1] = NG;

    /* overige ingangen */
    /* ---------------- */

    /* Gebruikers toevoegingen file includen */
    /* ------------------------------------- */
    #include "GMdpl.add"

}
