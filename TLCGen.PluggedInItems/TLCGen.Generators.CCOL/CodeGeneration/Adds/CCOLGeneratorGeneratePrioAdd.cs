﻿using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GeneratePrioAddHeader(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.AppendLine(_beginGeneratedHeader);
            sb.AppendLine("/* PRIORITEIT BESTAND, GEBRUIKERS TOEVOEGINGEN         */");
            sb.AppendLine("/* --------------------------------------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(c.Data, "prio.add"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(c.Data));
            sb.AppendLine(_endGeneratedHeader);

            return sb.ToString();
        }

        private string GeneratePrioAdd(ControllerModel c)
        {
            var sb = new StringBuilder();

            sb.Append(GeneratePrioAddHeader(c));
            sb.AppendLine();
            sb.AppendLine("void RijTijdScenario_Add(void) {");
            sb.AppendLine("  /* -----------------------------------------------");
            sb.AppendLine("     Pas hier zonodig het RijTijdScenario aan, bijv:");
            sb.AppendLine("     iRijTijdScenario[prioFC02] = rtsOngehinderd;");
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
            sb.AppendLine("void PrioInstellingen_Add(void) {");
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
            sb.AppendLine("     iMaximumWachtTijdOverschreden[prioFC02] = FALSE;");
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
            sb.AppendLine("     iOnderMaximumVerstreken[prioFC02] = FALSE;");
            sb.AppendLine("     ------------------------------------------------------ */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void BlokkeringsTijd_Add(void) {");
            sb.AppendLine("  /* -----------------------------------------------");
            sb.AppendLine("     Pas hier zonodig de BlokkeringsTimer aan, bijv:");
            sb.AppendLine("     iBlokkeringsTimer[prioFC02] = MAX_INT;");
            sb.AppendLine("     ----------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void PrioriteitsOpties_Add(void) {");
            sb.AppendLine("  /* ------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig de PrioriteitsOpties aan, bijv:");
            sb.AppendLine("     iPrioriteitsOpties[prioFC02] = poGeenPrioriteit;");
            sb.AppendLine("     ------------------------------------------------ */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void PrioriteitsNiveau_Add(void) {");
            sb.AppendLine("  /* ---------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig het PrioriteitsNiveau aan, bijv:");
            sb.AppendLine("     iPrioriteitsNiveau[prioFC02] = 0;");
            sb.AppendLine("     ---------------------------------------------------");
            sb.AppendLine("     Houdt hier zonodig tijdelijk de prioteitstoekenning");
            sb.AppendLine("     tegen, door gebruik te maken van iXPrio[fc], bijv:");
            sb.AppendLine("     iXPrio[prioFC02] = TRUE;");
            sb.AppendLine("     --------------------------------------------------- */");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void PrioriteitsToekenning_Add(void) {");
            sb.AppendLine("  /* -----------------------------------------");
            sb.AppendLine("     Pas hier zonodig de Prioriteit aan, bijv:");
            sb.AppendLine("     iPrioriteit[prioFC02] = FALSE;");
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
            sb.AppendLine("     iStartGroen[prioFC02] = 0;");
            sb.AppendLine("     ------------------------------------------------ */");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void PrioAanvragen_Add(void) {");
            sb.AppendLine("  /* -------------------------------------------");
            sb.AppendLine("     Pas hier zonodig de aanvraag A[] aan, bijv:");
            sb.AppendLine("     A[fc02] |= TRUE;");
            sb.AppendLine("     ------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void RealisatieTijden_Add(void) {");
            sb.AppendLine("  /* ----------------------------------------------------");
            sb.AppendLine("     Een met de prio-richting konflikterende richting wordt");
            sb.AppendLine("     tegengehouden, als zijn realisatietijd groter is dan");
            sb.AppendLine("     het startgroenmoment van de prio-richting.");
            sb.AppendLine("     De realisatietijd van een richting is de hoeveelheid");
            sb.AppendLine("     tijd dat de prio-richting niet kan komen t.g.v. een");
            sb.AppendLine("     realisatie van de konflikterende richting.");
            sb.AppendLine("     Pas hier zonodig de RealisatieTijd aan.");
            sb.AppendLine("     ---------------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void PrioTegenhouden_Add(void) {");
            sb.AppendLine("  /* -----------------------------------------------------");
            sb.AppendLine("     Pas hier zonodig de instructievariabele RR aan, bijv:");
            sb.AppendLine("     if (RR[fc08] & PRIO_RR_BIT) {");
            sb.AppendLine("       RR[fc24]|= PRIO_RR_BIT;");
            sb.AppendLine("     }");
            sb.AppendLine("     ----------------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void PrioAfkappen_Add(void) {");
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
            sb.AppendLine("void PrioAlternatieven_Add(void) {");
            sb.AppendLine("    /* -----------------------------------------------------");
            sb.AppendLine("    Pas hier zonodig de PAR aan (bijvoorbeeld bij naloop,");
            sb.AppendLine("    deelconflict, voorstarten...), bijv:");
            sb.AppendLine("    PAR[fc31] = PAR[fc31] && PAR[fc32];");
            sb.AppendLine("    ----------------------------------------------------- */");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("void PostAfhandelingPrio_Add(void) {");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM");
            sb.AppendLine("void PrioDebug_Add(void) {");
            sb.AppendLine("  PrioDebug(NG);");
            sb.AppendLine("}");
            sb.AppendLine("#endif");

            return sb.ToString();
        }
    }
}
