using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using TLCGen.Extensions;
using TLCGen.Models;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using TLCGen.Models.Enumerations;

namespace TLCGen.Specificator
{
    public static class FunctionalityGenerator
    {
        public static int TableCount;

        public static void ResetCounters()
        {
            TableCount = 0;
        }

        public static string ToCustomString(this bool value)
        {
            return value ? (string)Texts["Generic_True"] : (string)Texts["Generic_False"];
        }

        public static string GetKruisingNaam(ControllerModel c)
        {
            return $"{c.Data.Straat1}" +
                (string.IsNullOrWhiteSpace(c.Data.Straat2) ? "" : $" - {c.Data.Straat2} ");
        }

        private static ResourceDictionary _texts;
        public static ResourceDictionary Texts
        {
            get
            {
                if (_texts == null)
                {
                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    using (var stream = assembly.GetManifestResourceStream("TLCGen.Specificator.Resources.SpecificatorTexts.xaml"))
                    {
                        _texts = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(stream);
                    }
                }
                return _texts;
            }
        }

        public static void AddHeaderTextsToDocument(WordprocessingDocument doc, SpecificatorDataModel model, ControllerDataModel data)
        {
            foreach (var h in doc.MainDocumentPart.HeaderParts)
            {
                foreach (var t in h.RootElement.Descendants<Paragraph>().SelectMany(x => x.Descendants<Run>()).SelectMany(x => x.Descendants<Text>()))
                {
                    if (t.Text.Contains("FIRSTPAGEHEADER")) t.Text = "";
                    if (t.Text.Contains("HEADER")) t.Text = $"Functionele specificatie {data.Naam} ({data.Straat1}{(!string.IsNullOrWhiteSpace(data.Straat2) ? " - " + data.Straat2 : "")}, {data.Stad})";
                }
            }
            foreach (var f in doc.MainDocumentPart.FooterParts)
            {
                foreach (var r in f.RootElement.Descendants<Run>())
                {
                    var fp = false;
                    foreach (var t in r.Descendants<Text>())
                    {
                        if (t.Text.Contains("FIRSTPAGEFOOTER"))
                        {
                            t.Text = "";
                            fp = true;
                        }
                    }
                    if (fp)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Organisatie))
                        {
                            r.AppendChild(new Text(model.Organisatie));
                            r.AppendChild(new Break());
                        }
                        if (!string.IsNullOrWhiteSpace(model.Straat))
                        {
                            r.AppendChild(new Text(model.Straat));
                            r.AppendChild(new Break());
                        }
                        var t = "";
                        if (!string.IsNullOrWhiteSpace(model.Postcode))
                        {
                            t = model.Postcode;
                        }
                        if (!string.IsNullOrWhiteSpace(model.Stad))
                        {
                            t = t + " " + model.Stad;
                        }
                        if (t != "")
                        {
                            r.AppendChild(new Text(t));
                            r.AppendChild(new Break());
                        }
                        if (!string.IsNullOrWhiteSpace(model.TelefoonNummer))
                        {
                            r.AppendChild(new Text($"Telefoon: {model.TelefoonNummer}"));
                            r.AppendChild(new Break());
                        }
                        if (!string.IsNullOrWhiteSpace(model.EMail))
                        {
                            r.AppendChild(new Text($"E-mail: {model.EMail}"));
                        }
                    }
                }
            }
        }

        public static string ReplaceHolders(string text, ControllerModel c, SpecificatorDataModel model)
        {
            var t = text;
            t = Regex.Replace(t, "__KR__", c.Data.Naam);
            t = Regex.Replace(t, "__STAD__", c.Data.Stad);
            t = Regex.Replace(t, "__STRAAT1__", c.Data.Straat1);
            t = Regex.Replace(t, "__STRAAT2__", c.Data.Straat2);
            return t;
        }

        public static List<OpenXmlCompositeElement> GetFirstPage(ControllerDataModel data)
        {
            var par1 = new Paragraph();
            var run = par1.AppendChild(new Run());
            run.AppendChild(new Text($"{Texts["Title_Document"]} {data.Naam}"));
            OpenXmlHelper.ApplyStyleToParagraph(par1, "Title");

            var par2 = new Paragraph();
            run = par2.AppendChild(new Run());
            run.AppendChild(new Text($"{data.Straat1}{(!string.IsNullOrWhiteSpace(data.Straat2) ? " - " + data.Straat2 : "")}"));
            OpenXmlHelper.ApplyStyleToParagraph(par2, "Subtitle");

            var par3 = new Paragraph();
            run = par3.AppendChild(new Run());
            run.AppendChild(new Text($"{data.Stad}"));
            OpenXmlHelper.ApplyStyleToParagraph(par3, "Subtitle");

            return new List<OpenXmlCompositeElement> { par1, par2, par3 };
        }

        public static List<OpenXmlCompositeElement> GetVersionControl(ControllerDataModel data)
        {
            var title = OpenXmlHelper.GetTextParagraph($"{Texts["Title_Versioning"]}", "Subtitle");

            var table = new Table();

            TableProperties props = new TableProperties(
                new TableBorders(
                new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 }),
                new TableWidth() { Type = TableWidthUnitValues.Pct, Width = $"{100 * 50}" });

            table.AppendChild(props);

            foreach (var v in data.Versies)
            {
                table.Append(OpenXmlHelper.GetTableRow(
                    new[] { v.Datum.ToShortDateString(), v.Versie, v.Ontwerper, v.Commentaar },
                    new[] { 14, 14, 14, 58 }));
            }

            return new List<OpenXmlCompositeElement> { title, table };
        }

        public static void GetIntroChapter(WordprocessingDocument doc, ControllerModel c, SpecificatorDataModel model)
        {
            var body = doc.MainDocumentPart.Document.Body;

            body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Intro"]}", 1));

            var text = 
                $"Dit document beschrijft de functionele eisen aangaande de verkeersregelinstallatie (VRI) op het " +
                $"kruispunt " +
                GetKruisingNaam(c) +
                (Regex.IsMatch(c.Data.Stad, @"^(g|G)emeente") ? $"in de {c.Data.Stad}" : $"te {c.Data.Stad} ({c.Data.Naam}).");
            body.Append(OpenXmlHelper.GetTextParagraph(text));

            if (!string.IsNullOrEmpty(DataAccess.TLCGenControllerDataProvider.Default.ControllerFileName))
            {
                var path = Path.GetDirectoryName(DataAccess.TLCGenControllerDataProvider.Default.ControllerFileName);
                var file = Path.Combine(path, (Regex.IsMatch(c.Data.BitmapNaam, @"\.(bmp|BMP)$") ? c.Data.BitmapNaam : c.Data.BitmapNaam + ".bmp"));
                if (File.Exists(file))
                {
                    OpenXmlHelper.AddImageToBody(doc, file);
                }
            }

            var sb = new StringBuilder();

            sb.Append($"Het kruispunt {GetKruisingNaam(c)} wordt schematisch weergegeven in de bovenstaande figuur. ");

            var sl = new List<string>();
            if (c.InterSignaalGroep.Gelijkstarten.Any(x => x.DeelConflict) ||
                c.InterSignaalGroep.Voorstarten.Any())
            {
                sb.Append("De volgende richtingen woren in deelconflict afgehandeld: ");
                body.Append(OpenXmlHelper.GetTextParagraph(sb.ToString()));
                sb.Clear();

                foreach(var gs in c.InterSignaalGroep.Gelijkstarten.Where(x => x.DeelConflict))
                {
                    sl.Add($"{(string)Texts["Generic_Fase"]} {gs.FaseVan} (gelijkstart met {(string)Texts["Generic_fase"]} {gs.FaseNaar})");
                }
                foreach (var vs in c.InterSignaalGroep.Voorstarten)
                {
                    sl.Add($"{(string)Texts["Generic_Fase"]} {vs.FaseVan} (voorstart op {(string)Texts["Generic_fase"]} {vs.FaseNaar})");
                }
                body.Append(OpenXmlHelper.GetBulletList(doc, sl));
            }
            else
            {
                sb.Append("Alle bewegingen worden conflictvrij afgehandeld. ");
            }
            body.Append(OpenXmlHelper.GetTextParagraph(sb.ToString()));
            sb.Clear();

            body.Append(OpenXmlHelper.GetTextParagraph("De verkeersregeling dient aan de volgende eisen te voldoen: "));
            sl.Clear();
            sl.Add("Veilige situatie op het kruispunt.");
            sl.Add("Goede en efficiënte verkeersafwikkeling.");
            sl.Add("Logische en acceptabele situatie voor weggebruikers.");
            body.Append(OpenXmlHelper.GetBulletList(doc, sl));

            body.Append(OpenXmlHelper.GetTextParagraph(
                "In het regelprogramma dienen in het commentaar alle functionele" +
                " beschrijvingen van alle parameters, tijden, schakelaars en dergelijke opgenomen te worden."));
            
            body.Append(OpenXmlHelper.GetChapterTitleParagraph("Algemene instellingen", 2));

            body.Append(OpenXmlHelper.GetTextParagraph(
                "Onderstaande tabel geeft een overzicht van een aantal algemene instellingen voor de regeling."));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Optie"],
                    (string)Texts["Generic_Instelling"]
                }
            };
            l.Add(new List<string> { (string)Texts["Generic_Fasebewaking"], c.Data.Fasebewaking.ToString() });
            l.Add(new List<string> { (string)Texts["Generic_CCOLVersie"], c.Data.CCOLVersie.GetDescription() });
            if (c.Data.CCOLVersie <= Models.Enumerations.CCOLVersieEnum.CCOL8)
            {
                l.Add(new List<string> { (string)Texts["Generic_TypeVLOG"], c.Data.VLOGType.GetDescription() });
            }
            else
            {
                l.Add(new List<string> { (string)Texts["Generic_VLOGToepassen"], c.Data.VLOGSettings.VLOGToepassen.ToCustomString() });
                l.Add(new List<string> { (string)Texts["Generic_VLOGVersie"], c.Data.VLOGSettings.VLOGVersie.GetDescription() });
                l.Add(new List<string> { (string)Texts["Generic_VLOGLoggingMode"], c.Data.VLOGSettings.LOGPRM_VLOGMODE.GetDescription() });
                l.Add(new List<string> { (string)Texts["Generic_VLOGMonitoringMode"], c.Data.VLOGSettings.MONPRM_VLOGMODE.GetDescription() });
            }
            l.Add(new List<string> { (string)Texts["Generic_TypeKWC"], c.Data.KWCType.GetDescription() });
            l.Add(new List<string> { (string)Texts["Generic_ExtraMeevInWG"], c.Data.ExtraMeeverlengenInWG.ToCustomString() });
            l.Add(new List<string> { (string)Texts["Generic_AanstWaitsig"], c.Data.AansturingWaitsignalen.GetDescription() });
            l.Add(new List<string> { (string)Texts["Generic_SegmDisplay"], c.Data.SegmentDisplayType.GetDescription() });
            l.Add(new List<string> { (string)Texts["Generic_UsPerML"], c.Data.UitgangPerModule.ToCustomString() });
            l.Add(new List<string> { (string)Texts["Generic_FixatieMogelijk"], c.Data.FixatieMogelijk.ToCustomString() });
            l.Add(new List<string> { (string)Texts["Generic_BijkomenFixatie"], c.Data.BijkomenTijdensFixatie.ToCustomString() });
            l.Add(new List<string> { (string)Texts["Generic_TypeGroentijden"], c.Data.TypeGroentijden.GetDescription() });
            body.Append(OpenXmlHelper.GetTable(l));
        }

        
        public static List<OpenXmlCompositeElement> GetDetectorenChapter(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Detectoren_Functies"], styleid: "Caption"));
            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Detector"],
                    "Fase",
                    "Type",
                    "Aanvraag",
                    "Verlengen",
                    "Aanvraag direct",
                    "Wachtlicht",
                    "Rijstrook",
                    "Aanvraag bij storing",
                    "Veiligheidsgroen"
                }
            };
            foreach (var fc in c.Fasen)
            foreach (var d in fc.Detectoren)
            {
                l.Add(new List<string>
                {
                    d.Naam,
                    fc.Naam,
                    d.Type.GetDescription(),
                    d.Aanvraag.GetDescription(),
                    d.Verlengen.GetDescription(),
                    d.AanvraagDirect.ToString(),
                    d.Wachtlicht.ToCustomString(),
                    d.Rijstrook.ToString(),
                    d.AanvraagBijStoring.GetDescription(),
                    d.VeiligheidsGroen.GetDescription(),
                });
            }
            foreach (var d in c.Detectoren)
            {
                l.Add(new List<string>
                {
                    d.Naam,
                    "-",
                    d.Type.GetDescription(),
                    d.Aanvraag.GetDescription(),
                    d.Verlengen.GetDescription(),
                    d.AanvraagDirect.ToCustomString(),
                    d.Wachtlicht.ToCustomString(),
                    d.Rijstrook.ToString(),
                    d.AanvraagBijStoring.GetDescription(),
                    d.VeiligheidsGroen.GetDescription()
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Detectoren_Tijden"], styleid: "Caption"));
            l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Detector"],
                    "Fase",
                    "Bezettijd",
                    "Hiaattijd",
                    "Ondergedrag",
                    "Bovengedrag",
                    "Flutter tijd",
                    "Flutter counter"
                }
            };
            foreach (var fc in c.Fasen)
            foreach (var d in fc.Detectoren)
            {
                l.Add(new List<string>
                {
                    d.Naam,
                    fc.Naam,
                    d.TDB.ToString(),
                    d.TDH.ToString(),
                    d.TOG.ToString(),
                    d.TBG.ToString(),
                    d.TFL.ToString(),
                    d.CFL.ToString(),
                });
            }
            foreach (var d in c.Detectoren)
            {
                l.Add(new List<string>
                {
                    d.Naam,
                    "-",
                    d.TDB.ToString(),
                    d.TDH.ToString(),
                    d.TOG.ToString(),
                    d.TBG.ToString(),
                    d.TFL.ToString(),
                    d.CFL.ToString(),
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        // TODO: tabel met ingangen (indien aanwezig)
        // TODO: tabel met selectieve detectie (indien aanwezig)
        // TODO: hoofdstuk met omgang met detectiestoring (opm: de optie 'aanvraag bij storing' per detector zit in tabel met detectoren) 

        public static List<OpenXmlCompositeElement> GetRichtingGevoeligChapter(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren"]}", 2));

            if (c.RichtingGevoeligeAanvragen.Any())
            {
                items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Richtinggevoelig_Aanvragen"], styleid: "Caption"));
                var l = new List<List<string>>
            {
                new List<string>
                {
                    "Fase",
                    "Van",
                    "Naar",
                    "Maximaal tijdsverschil",
                    "Reset aanvraag",
                    "Reset tijd"
                }
            };
                foreach (var rga in c.RichtingGevoeligeAanvragen)
                {
                    l.Add(new List<string>
                {
                    rga.FaseCyclus,
                    rga.VanDetector,
                    rga.NaarDetector,
                    rga.MaxTijdsVerschil.ToString(),
                    rga.ResetAanvraag.ToCustomString(),
                    rga.ResetAanvraag ? rga.ResetAanvraagTijdsduur.ToString() : "-"
                });
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            }

            if (c.RichtingGevoeligVerlengen.Any())
            {
                items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Richtinggevoelig_Verlengen"], styleid: "Caption"));
                var l = new List<List<string>>
            {
                new List<string>
                {
                    "Fase",
                    "Van",
                    "Naar",
                    "Type verlengen",
                    "Maximaal tijdsverschil",
                    "Verleng tijd"
                }
            };
                foreach (var rgv in c.RichtingGevoeligVerlengen)
                {
                    l.Add(new List<string>
                {
                    rgv.FaseCyclus,
                    rgv.VanDetector,
                    rgv.NaarDetector,
                    rgv.TypeVerlengen.ToString(),
                    rgv.MaxTijdsVerschil.ToString(),
                    rgv.VerlengTijd.ToString(),
                });
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            }

            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_Perioden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Perioden"]}", 2));

            if (c.HalfstarData.IsHalfstar)
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Op het moment dat de regeling voertuigafhankelijk regelt, gelden de volgende " +
                    $"klokperiodes ten behoeve van de {(c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? "maximum" : "verleng")} groentijden"));
            }
            else
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Voor voertuigafhankelijk regelen gelden de volgende " +
                    $"klokperiodes ten behoeve van de {(c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? "maximum" : "verleng")} groentijden"));
            }

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Perioden"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string> { "Periode", "Start", "Einde", "Dagtype" }
            };
            foreach (var p in c.PeriodenData.Perioden)
            {
                l.Add(new List<string> { p.Naam, p.StartTijd.ToString(@"hh\:mm"), p.EindTijd.ToString(@"hh\:mm"), p.DagCode.ToString() });
            }
            items.Add(OpenXmlHelper.GetTable(l));

            items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Wanneer geen van bovenstaande perioden actief is geldt de dalperiode."));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_SignaalGroepAfhandeling(WordprocessingDocument doc, ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_SignaalgroepAfhandeling"]}", 2));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "De signaalgroepen worden volgens een vaste procedure afgehandeld (standaard CCOLafhandeling). " +
                "De signaalgroepafhandeling is verdeeld in een aantal toestanden. Een aantal van deze toestanden " +
                "is tijdsafhankelijk. In het navolgende worden de belangrijkste signaalgroeptoestanden en " +
                "signaalgroeptijden in chronologische volgorde besproken. De instellingen zijn opgenomen in " +
                "tabel 2."));
            items.Add(OpenXmlHelper.GetTextParagraph("Garantieroodtijd", bold: true));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "De garantieroodtijd is de tijd dat een signaalgroep minimaal rood is. Deze tijd is bedoeld om " +
                "een onrustig beeld voor weggebruikers te voorkomen (‘flitsrood’)."));
            items.Add(OpenXmlHelper.GetTextParagraph("Garantiegroentijd", bold: true));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Garantiegroentijden hebben een relatie met de veiligheid van de verkeersregeling. Na begin groen " +
                "kijken weggebruikers een aantal seconden niet naar het verkeerslicht. Bij te korte " +
                "garantiegroentijden bestaat het gevaar dat weggebruikers ongemerkt door rood rijden."));
            items.Add(OpenXmlHelper.GetTextParagraph("Vastgroentijd", bold: true));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "De vastgroentijd is afhankelijk van het detectieveld. De vastgroentijd moet voldoende zijn, " +
                "om ervoor te zorgen dat verkeer dat tussen de stopstreep en de eerste lus staat tijdens de " +
                "vastgroentijd weg kan rijden. Bij het gebruik van een (beperkte) verlengfunctie op de koplus " +
                "hoeft hier geen rekening mee gehouden te worden en kan de vastgroentijd gelijk zijn aan de " +
                "garantiegroentijd."));
            items.Add(OpenXmlHelper.GetTextParagraph("Wachtgroen", bold: true));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Wanneer er op een signaalgroep verkeer aanwezig is en er geen conflictaanvragen zijn, wordt " +
                "de betreffende signaalgroep in wachtgroen vastgehouden. Pas na een conflictaanvraag gaat een " +
                "signaalgroep daadwerkelijk verlengen.Op deze manier wordt voorkomen dat een drukke signaalgroep " +
                "plotsklaps kan worden beëindigd door één voertuig op een rustige conflicterende signaalgroep. " +
                "Ook wordt een signaalgroep die aan het meeverlengen is weer teruggezet naar wachtgroen op het " +
                "moment dat er weer verkeer in het detectieveld aanwezig is en er geen conflictaanvragen zijn."));
            items.Add(OpenXmlHelper.GetTextParagraph("Verlenggroen", bold: true));
            // TODO: dit gaat over signaalplannen!
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Gedurende de verlenggroenfase kan een signaalgroep bij continu verkeersaanbod groen blijven tot " +
                "het primaire groengebied is verstreken. De primaire groengebieden kunnen afhankelijk van het " +
                $"signaalplan variëren. Er zijn in deze regeling {c.GroentijdenSets.Count.ToString()} verschillende signaalplannen opgenomen."));
            items.Add(OpenXmlHelper.GetTextParagraph("Meeverlenggroen", bold: true));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Signaalgroepen die groen zijn en dit kunnen blijven zonder dat hierdoor andere signaalgroepen " +
                "worden tegengehouden, krijgen meeverlenggroen na het verlopen van hun hiaattijden of maximum " +
                "groentijd. Door het gebruik van meeverlenggroen wordt de restruimte in de verkeersregeling benut " +
                "en verbetert de logica en acceptatie van de verkeersregeling. Hierbij wordt rekening gehouden " +
                "met het verschil in geel- en ontruimingstijden."));
            items.Add(OpenXmlHelper.GetTextParagraph("Garantiegeeltijd", bold: true));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "De garantiegeeltijd is de tijd dat een signaalgroep minimaal geel is.Deze tijd is bedoeld om een " +
                "onrustig beeld voor weggebruikers te voorkomen (‘flitsgeel’)."));
            items.Add(OpenXmlHelper.GetTextParagraph("Geeltijd", bold: true));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "De geeltijd moet niet te laag en niet te hoog worden ingesteld.Een te lage waarde kan voertuigen " +
                "die niet meer tijdig kunnen stoppen in onveilige situaties brengen. Bij te hoge waarden blijven " +
                "bestuurders, die eigenlijk hadden kunnen stoppen, doorrijden."));
            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_SignaalGroepInstellingen(WordprocessingDocument doc, ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_SignaalgroepInstellingen"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Hieronder worden de instellingen voor de {Texts["Generic_fasen"]} weergegeven. Eerst volgt een " +
                $"tabel met de voor de fasen instelde functie. Daarna volgt een table met ingestelde tijden."));
            items.AddRange(TableGenerator.GetTable_FasenFuncties(c));
            items.AddRange(TableGenerator.GetTable_FasenTijden(c));
            return items;
        }


        public static List<OpenXmlCompositeElement> GetChapter_Modulestructuur(WordprocessingDocument doc, ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Modulestructuur"]}", 2));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Tijdens voertuigafhankelijk bedrijf wordt gebruik gemaakt van de modulestructuur. De " +
                "modulestructuur is opgebouwd uit primaire, bijzondere en alternatieve realisaties."));
            items.Add(OpenXmlHelper.GetTextParagraph("Primaire realisatie", bold: true));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "De modulestructuur bepaalt in welke vaste volgorde de verschillende signaalgroepen met een aanvraag " +
                "groen worden (primaire realisaties). Als geen enkele signaalgroep een aanvraag heeft dan wacht de " +
                "modulestructuur in module 1. Een moduleovergang vindt plaats op het moment dat alle primair " +
                "toegedeelde signaalgroepen met een aanvraag van het actieve blok cyclisch verlenggroen hebben " +
                "verlaten."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Alle signaalgroepen zijn toegedeeld aan minimaal één module (zie tabel 3). Een signaalgroep die " +
                "een aanvraag heeft gekregen (in het aanvraaggebied) wordt tijdens het actief zijn van de module " +
                "waaraan deze is toegedeeld, eenmalig primair gerealiseerd. De groenduur van een primaire " +
                "realisatie wordt beperkt volgens de ingestelde maximumgroentijd (zie tabel 2) per klokperiode " +
                "(zie tabel 1)."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                 "Het aanvraaggebied voor primair toegedeelde richtingen van een module wordt afgesloten nadat " +
                 "de desbetreffende module actief is geworden en er een aanvraag is gezet door een signaalgroep " +
                 "uit een volgende module. Bij de moduleovergang wordt het aanvraaggebied van de richtingen van " +
                 "de module die verlaten wordt weer opengesteld."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                  "Door het gebruik van versnelde primaire realisaties worden richtingen onder voorwaarden (bijvoorbeeld afwezigheid aanvragen van eerder in de regelstructuur te realiseren conflictrichtingen) eerder groen. Versnelde realisaties geven, met name in rustige perioden, een grote flexibiliteit aan de verkeersregeling waardoor de acceptatie en geloofwaardigheid van de verkeersregeling hoger wordt. Met behulp van een parameter is per richting in te stellen hoeveel module er vooruit mag worden gerealiseerd. Default mogen alle richtingen één module vooruit realiseren."));
            
            items.Add(OpenXmlHelper.GetTextParagraph("Bijzondere realisaties", bold: true));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Naast de primaire realisaties kunnen signaalgroepen ook bijzonder realiseren. Bij een bijzondere " +
                "realisatie wordt een signaalgroep buiten de modulestructuur om naar groen gestuurd. Bijzondere " +
                "realisaties worden gebruikt om prioriteit te geven aan bussen of hulpdiensten (zie hoofdstuk 6 " +
                "en hoofdstuk 7)."));

            items.Add(OpenXmlHelper.GetTextParagraph("Alternatieve realisaties", bold: true));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Extra realisatiemogelijkheden zijn met alternatieve realisaties toegekend. Deze alternatieve " +
                "realisaties worden volgens het langstwachtende principe afgewikkeld. Dat wil zeggen dat wanneer " +
                "er op een bepaald moment meerdere richtingen alternatief kunnen realiseren, de richting met de " +
                "hoogste wachttijd als eerste groen krijgt. Bij alternatieve toedeling krijgen fietsrichtingen " +
                "voorrang ten opzichte van autorichtingen, hiertoe wordt de wachttijd voor alternatieve toedeling " +
                "voor fietsers met een instelbare waarde opgehoogd (default 1000 sec.). De alternatieve realisaties " +
                "zijn dusdanig opgenomen dat ze de regeling zo min mogelijk ophouden."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Per richting is een schakelaar opgenomen om alternatieve realisaties toe te staan (default voor " +
                "alle richtingen aan). Daarnaast is per richting een referentie opgeven hoeveel ruimte er in de " +
                "regeling beschikbaar moet zijn om alternatief te realiseren (voor alle autorichtingen 8,0 sec, " +
                "voor fietsers 5,0 sec. en voor voetgangers gelijk aan de vastgroentijd eventueel rekening houdend " +
                "met de naloop). Deze ruimte wordt berekend op basis van de vigerende maximum groentijd van een " +
                "niet conflict dat op dat moment in cyclisch verlenggroen staat, of op het punt staat om primair " +
                "dan wel bijzonder te realiseren. Voor ieder richting is opgegeven wat de maximale groentijd voor " +
                "alternatief realiseren is, als de voorwaarde voor alternatief realiseren is vervallen (voor alle " +
                "autorichtingen 8,0 voor fietsrichtingen 5,0 en voor voetgangers de vastgroentijd)."));

            items.AddRange(TableGenerator.GetTable_Modulen(c));

            return items;
        }



        public static List<OpenXmlCompositeElement> GetModulenChapter(WordprocessingDocument doc, ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Modulen"]}", 2));
            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Modulen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string> { (string)Texts["Generic_Modulen"], (string)Texts["Generic_Fasen"] }
            };
            c.ModuleMolen.Modules.ForEach(m => l.Add(new List<string> { m.Naam, m.Fasen.Select(x => x.FaseCyclus).Aggregate((y, z) => y + ", " + z) }));
            items.Add(OpenXmlHelper.GetTable(l));

            items.Add(OpenXmlHelper.GetTextParagraph($"Wanneer er geen aanvragen zijn wacht de regeling in module {c.ModuleMolen.WachtModule}."));

            if (c.ModuleMolen.LangstWachtendeAlternatief)
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"De regeling is voorzien van langswachtende alternatief. " +
                    $"Per richting is instelbaar of die alternatief mag realiseren, " +
                    $"welke ruimte er minimaal nog moet om dat toe te staan, en welke groentijd minimaal wordt gegeven bij een alternatieve realisatie."));
            }
            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Wanneer daar ruimte voor is, kunnen richtingen primair vooruit realiseren. " +
                $"Het aantal {Texts["Generic_modulen"]} dat vooruit gerealiseerd kan worden is instelbaar." +
                $"Vooruit realiseren gaat voor op alternatief realiseren: wanneer beide mogelijk zijn zal een richting vooruit komen."));

            if (c.ModuleMolen.LangstWachtendeAlternatief)
            {
                items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_VooruitAltInst"], styleid: "Caption"));
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        (string)Texts["Generic_Fase"],
                        "Aantal modulen vooruit",
                        "Alternatief toegestaan",
                        "Alternatieve ruimte",
                        "Alternatieve groentijd"
                    }
                };
                c.ModuleMolen.FasenModuleData.ForEach(x => l.Add(new List<string>
                {
                    x.FaseCyclus,
                    x.ModulenVooruit.ToString(),
                    x.AlternatiefToestaan.ToCustomString(),
                    x.AlternatieveRuimte.ToString(),
                    x.AlternatieveGroenTijd.ToString()
                }));
                items.Add(OpenXmlHelper.GetTable(l));
            }
            else if (c.ModuleMolen.Modules.Any(x => x.Fasen.Any(x2 => x2.Alternatieven.Any())))
            {
                items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Vooruit"], styleid: "Caption"));
                var lt = new List<string>
                {
                    (string)Texts["Generic_Fase"],
                    "Aantal modulen vooruit"
                };
                //foreach(var m in c.ModuleMolen.Modules)
                //{
                //    lt.Add("Alt. groentijd onder " + m.Naam);
                //}
                l = new List<List<string>>
                {
                    lt
                };
                c.ModuleMolen.FasenModuleData.ForEach(x =>
                {
                    var nl = new List<string>
                    {
                        x.FaseCyclus,
                        x.ModulenVooruit.ToString()
                    };
                    //foreach (var m in c.ModuleMolen.Modules)
                    //{
                    //    var t = "-";
                    //    var mfc = m.Fasen.FirstOrDefault(x2 => x2.Alternatieven.Any(x3 => x3.FaseCyclus == x.FaseCyclus));
                    //    if(mfc != null)
                    //    {
                    //        var afc = mfc.Alternatieven.First(x2 => x2.FaseCyclus == x.FaseCyclus);
                    //        t = afc.AlternatieveGroenTijd.ToString();
                    //    }
                    //    nl.Add(t);
                    //}
                    l.Add(nl);
                });
                items.Add(OpenXmlHelper.GetTable(l));

                items.Add(OpenXmlHelper.GetTextParagraph($"De volgende {(string)Texts["Generic_fasen"]} kunnen alternatief realiseren onder dekking van een andere {(string)Texts["Generic_fase"]} in cyclisch verlenggroen (CV)."));

                var il = new List<string>();
                foreach (var m in c.ModuleMolen.Modules)
                { foreach (var f in m.Fasen.Where(x2 => x2.Alternatieven.Any()))
                    {
                        foreach (var a in f.Alternatieven)
                        {
                            il.Add($"In module {m.Naam}: {a.FaseCyclus} onder dekking van {f.FaseCyclus} (groentijd: {a.AlternatieveGroenTijd})");
                        }
                    }
                }
                items.AddRange(OpenXmlHelper.GetBulletList(doc, il));
            }
            else
            {
                items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_VooruitAltInst"], styleid: "Caption"));
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        (string)Texts["Generic_Fase"],
                        "Aantal modulen vooruit"
                    }
                };
                c.ModuleMolen.FasenModuleData.ForEach(x => l.Add(new List<string>{ x.ModulenVooruit.ToString() }));
                items.Add(OpenXmlHelper.GetTable(l));
            }

            return items;
        }

        public static List<OpenXmlCompositeElement> GetGroentijdenChapter(ControllerModel c)
        {
            var title = OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Groentijden"]}", 2);

            OpenXmlCompositeElement tableTitle = null;
            switch (c.Data.TypeGroentijden)
            {
                case Models.Enumerations.GroentijdenTypeEnum.MaxGroentijden:
                    tableTitle = OpenXmlHelper.GetTextParagraph((string)Texts["Table_MaxGroentijden"], styleid: "Caption");
                    break;
                case Models.Enumerations.GroentijdenTypeEnum.VerlengGroentijden:
                    tableTitle = OpenXmlHelper.GetTextParagraph((string)Texts["Table_VerlGroentijden"], styleid: "Caption");
                    break;
            }

            var l = new List<List<string>>();
            var l1 = new List<string> { (string)Texts["Fase"] };
            foreach (var set in c.GroentijdenSets)
            {
                l1.Add(set.Naam);
            }
            l.Add(l1);
            foreach (var fc in c.Fasen)
            {
                var l2 = new List<string> { fc.Naam };
                foreach (var set in c.GroentijdenSets)
                {
                    var f = set.Groentijden.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                    if (f != null)
                    {
                        l2.Add(f.Waarde.HasValue ? f.Waarde.Value.ToString() : "-");
                    }
                    else
                    {
                        l2.Add("-");
                    }
                }
                l.Add(l2);
            }
            var table = OpenXmlHelper.GetTable(l);

            return new List<OpenXmlCompositeElement> { title, tableTitle, table };
        }


    }
}
