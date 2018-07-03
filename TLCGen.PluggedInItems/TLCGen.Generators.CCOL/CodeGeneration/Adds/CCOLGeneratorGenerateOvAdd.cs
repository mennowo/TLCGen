﻿using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateOvAddHeader(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine(_beginGeneratedHeader);
            sb.AppendLine("/* OV BESTAND, GEBRUIKERS TOEVOEGINGEN                 */");
            sb.AppendLine("/* --------------------------------------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(c.Data, "ov.add"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(c.Data));
            sb.AppendLine(_endGeneratedHeader);

            return sb.ToString();
        }

        private string GenerateOvAdd(ControllerModel c)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GenerateDplAddHeader(c));
            sb.AppendLine();
            sb.AppendLine("void RijTijdScenario_Add(void) {");
            sb.AppendLine("  /* -----------------------------------------------");
            sb.AppendLine("     Pas hier zonodig het RijTijdScenario aan, bijv:");
            sb.AppendLine("     iRijTijdScenario[ovFC02] = rtsOngehinderd;");
            sb.AppendLine("     ----------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void InUitMelden_Add(void) {");
            sb.AppendLine("  /* -----------------------------------------------------");
            sb.AppendLine("     Pas hier zo nodig de instructie voor de hulpelementen");
            sb.AppendLine("     aan t.b.v. de in- en uitmeldingen.");
            sb.AppendLine("     ----------------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void OVInstellingen_Add(void) {");
            sb.AppendLine("  /* -------------------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig de beschikbare alternatieve ruimte of de");
            sb.AppendLine("     toestemming om alternatief te realiseren door niet-konflikten");
            sb.AppendLine("     tijdens een ingreep aan, bijv:");
            sb.AppendLine("     iPRM_ALTP[fc02] = TFG_max[fc02];");
            sb.AppendLine("     iSCH_ALTG[fc02] = TRUE;");
            sb.AppendLine("     ------------------------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void WachtTijdBewaking_Add(void) {");
            sb.AppendLine("  /* --------------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig MaximumWachtTijdOverschreden aan, bijv:");
            sb.AppendLine("     iMaximumWachtTijdOverschreden = FALSE;");
            sb.AppendLine("     -------------------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void KonfliktTijden_Add(void) {");
            sb.AppendLine("  /* ----------------------------------------------");
            sb.AppendLine("     Pas hier zonodig iKonfliktTijd aan, bijv:");
            sb.AppendLine("     iKonfliktTijd[fc31] = iKonfliktTijd[fc32]=");
            sb.AppendLine("       iKonfliktTijd[fc31] >= iKonfliktTijd[fc32] ?");
            sb.AppendLine("       iKonfliktTijd[fc31] : iKonfliktTijd[fc32];");
            sb.AppendLine("     ---------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void OnderMaximum_Add(void) {");
            sb.AppendLine("  /* ------------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig het OnderMaximumVerstreken aan, bijv:");
            sb.AppendLine("     iOnderMaximumVerstreken[ovFC02] = FALSE;");
            sb.AppendLine("     ------------------------------------------------------ */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void BlokkeringsTijd_Add(void) {");
            sb.AppendLine("  /* -----------------------------------------------");
            sb.AppendLine("     Pas hier zonodig de BlokkeringsTimer aan, bijv:");
            sb.AppendLine("     iBlokkeringsTimer[ovFC02] = MAX_INT;");
            sb.AppendLine("     ----------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void PrioriteitsOpties_Add(void) {");
            sb.AppendLine("  /* ------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig de PrioriteitsOpties aan, bijv:");
            sb.AppendLine("     iPrioriteitsOpties[ovFC02] = poGeenPrioriteit;");
            sb.AppendLine("     ------------------------------------------------ */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void PrioriteitsNiveau_Add(void) {");
            sb.AppendLine("  /* ---------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig het PrioriteitsNiveau aan, bijv:");
            sb.AppendLine("     iPrioriteitsNiveau[ovFC02] = 0;");
            sb.AppendLine("     ---------------------------------------------------");
            sb.AppendLine("     Houdt hier zonodig tijdelijk de prioteitstoekenning");
            sb.AppendLine("     tegen, door gebruik te maken van iXPrio[ov], bijv:");
            sb.AppendLine("     iXPrio[ovFC02] = TRUE;");
            sb.AppendLine("     --------------------------------------------------- */");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void PrioriteitsToekenning_Add(void) {");
            sb.AppendLine("  /* -----------------------------------------");
            sb.AppendLine("     Pas hier zonodig de Prioriteit aan, bijv:");
            sb.AppendLine("     iPrioriteit[ovFC02] = FALSE;");
            sb.AppendLine("     ----------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void AfkapGroen_Add(void) {");
            sb.AppendLine("  /* ------------------------------------------");
            sb.AppendLine("     Pas hier zonodig het AfkapGroen aan, bijv:");
            sb.AppendLine("     iAfkapGroen[fc02] = TFG_max[fc02];");
            sb.AppendLine("     ------------------------------------------ */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void StartGroenMomenten_Add(void) {");
            sb.AppendLine("  /* ------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig het StartGroenMoment aan, bijv:");
            sb.AppendLine("     iStartGroen[ovFC02] = 0;");
            sb.AppendLine("     ------------------------------------------------ */");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void OVAanvragen_Add(void) {");
            sb.AppendLine("  /* -------------------------------------------");
            sb.AppendLine("     Pas hier zonodig de aanvraag A[] aan, bijv:");
            sb.AppendLine("     A[fc02] |= TRUE;");
            sb.AppendLine("     ------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void RealisatieTijden_Add(void) {");
            sb.AppendLine("  /* ----------------------------------------------------");
            sb.AppendLine("     Een met de ov-richting konflikterende richting wordt");
            sb.AppendLine("     tegengehouden, als zijn realisatietijd groter is dan");
            sb.AppendLine("     het startgroenmoment van de ov-richting.");
            sb.AppendLine("     De realisatietijd van een richting is de hoeveelheid");
            sb.AppendLine("     tijd dat de ov-richting niet kan komen t.g.v. een");
            sb.AppendLine("     realisatie van de konflikterende richting.");
            sb.AppendLine("     Pas hier zonodig de RealisatieTijd aan.");
            sb.AppendLine("     ---------------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void OVTegenhouden_Add(void) {");
            sb.AppendLine("  /* -----------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig de instructievariabele RR aan, bijv:");
            sb.AppendLine("     if (RR[fc08] & OV_RR_BIT) {");
            sb.AppendLine("       RR[fc24]|= OV_RR_BIT;");
            sb.AppendLine("     }");
            sb.AppendLine("     ----------------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void OVAfkappen_Add(void) {");
            sb.AppendLine("  /* --------------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig het TerugKomen aan, bijv:");
            sb.AppendLine("     iTerugKomen[fc02] = FALSE;");
            sb.AppendLine("     --------------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig de TerugKomGroenTijd aan, bijv:");
            sb.AppendLine("     iTerugKomGroenTijd[[fc02] = 0;");
            sb.AppendLine("     --------------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig het NietAfkappen aan, bijv:");
            sb.AppendLine("     iNietAfkappen[fc02]=FALSE;");
            sb.AppendLine("     -------------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig het AantalMalenNietAfkappen aan, bijv:");
            sb.AppendLine("     iAantalMalenNietAfkappen[fc02] = 3;");
            sb.AppendLine("     ------------------------------------------------------- */");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void OVAlternatieven_Add(void) {");
            sb.AppendLine("    /* -----------------------------------------------------");
            sb.AppendLine("    Pas hier zonodig de PAR aan (bijvoorbeeld bij naloop,");
            sb.AppendLine("    deelconflict, voorstarten...), bijv:");
            sb.AppendLine("    PAR[fc31] = PAR[fc31] && PAR[fc32];");
            sb.AppendLine("    ----------------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void PostAfhandelingOV_Add(void) {");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("#ifndef AUTOMAAT");
            sb.AppendLine("void OVDebug_Add(void) {");
            sb.AppendLine("  OVDebug(NG);");
            sb.AppendLine("}");
            sb.AppendLine("#endif");

            return sb.ToString();
        }
    }
}
