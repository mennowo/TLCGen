#ifndef __FIXATIE_H__
#define __FIXATIE_H__

/*************************************************************************
 * Fixatie
 *************************************************************************
 * Zorgt voor fixatie van het groenbeeld afhankelijk van ingang isFix
 *
 * Parameters:
 *    isFix        = index ingangssignaal fixatie keuze
 *    first, last  = (opeenvolgende) indici van richtingen die gefixeerd
 *                   dienen te worden
 *    bijkomen     = aanduiding of richtingen nog groen mogen worden
 *    tegenhouden  = aanduiding of fixatie (kort) tegengehouden moet worden
 *    prml         = primair of alternatieve toedeling van betreffende modulereeks
 *    ml           = huidige module in betreffende modulereeks
 *
 * Voorbeeld:
 *    Fixatie(isFix, 0, FCMAX-1, SCH[schbmfix], IH[hfixatietegenh], PRML, ML);
 *************************************************************************/
#if defined MLMAX || defined MLAMAX
void Fixatie(count isFix, count first, count last, bool bijkomen, bool tegenhouden, bool *prml[], count ml);
#else
void Fixatie(count isFix, count first, count last, bool bijkomen, bool tegenhouden);
#endif

#endif
