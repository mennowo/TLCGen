using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using TLCGen.Extensions;
using TLCGen.Models;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TLCGen.Models.Enumerations;

using TLCGen.Generators.CCOL.Settings;

namespace TLCGen.Specificator
{
    public static class FunctionalityGenerator
    {
        public static string ToCustomString(this bool value)
        {
            return value ? (string)Texts["Generic_True"] : (string)Texts["Generic_False"];
        }

        public static string ToCustomString2(this bool value)
        {
            return value ? (string)Texts["Special_True"] : (string)Texts["Special_False"];
        }

        public static string ToCustomStringJN(this bool value)
        {
            return value ? (string)Texts["Special_Ja"] : (string)Texts["Special_Nee"];
        }

        public static string ToCustomStringHL(this bool value)
        {
            return value ? (string)Texts["Special_Hoog"] : (string)Texts["Special_Laag"];
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
                    //var fpv = false;
                    foreach (var t in r.Descendants<Text>())
                    {
                        if (t.Text.Contains("FIRSTPAGEFOOTER"))
                        {
                            t.Text = "";
                            fp = true;
                        }
                        //if (t.Text.Contains("VERSION"))
                        //{
                        //    t.Text = "";
                        //    fpv = true;
                        //}
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
                            t = t + "  " + model.Stad;
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
                            r.AppendChild(new Break());
                        }
                        r.AppendChild(new Break());
                        r.AppendChild(new Text($"Regeling is gegenereerd met TLCGen versie { data.TLCGenVersie }."));
                    }
                    //if (fpv)
                    //{
                    //    r.AppendChild(new Text($"Regeling is gegenereerd met TLCGen versie { data.TLCGenVersie }."));
                    //}
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

        // TODO: tabel met ingangen (indien aanwezig)
        // TODO: tabel met selectieve detectie (indien aanwezig)

        public static string GetGroentijden(ControllerModel c, bool multiple = true)
        {
            if (multiple) return $"{(c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? "maximum" : "verleng")} groentijden";
            else return $"{(c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? "maximum" : "verleng")} groentijd";
        }

        public static List<OpenXmlCompositeElement> GetVersionControl(ControllerDataModel data)
        {
            var title = OpenXmlHelper.GetTextParagraph($"{Texts["Title_Versioning"]}"/*, "Subtitle"*/);

            var table = new Table();

            var props = new TableProperties(
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

        public static void GetIntroChapter(WordprocessingDocument doc, ControllerModel c, SpecificatorDataModel model)                 // hfdst 1   Inleiding en algemene instellingen  
        {
            var body = doc.MainDocumentPart.Document.Body;

            /* pagebreak toegevoegd */
            body.Append(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));

            body.Append(TableGenerator.Reset_Tables(c));

            body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Intro"]}", 1));

            var text = 
                $"Dit document beschrijft de functionele werking van de verkeersregelinstallatie (VRI) op het " +
                $"kruispunt " +
                GetKruisingNaam(c) +
                (c.Data.Stad != null && Regex.IsMatch(c.Data.Stad, @"^(g|G)emeente") ? $"in de {c.Data.Stad}" : $"te {c.Data.Stad ?? "Onbekend"} ({c.Data.Naam}).");
            body.Append(OpenXmlHelper.GetTextParagraph(text));

            var sb = new StringBuilder();
            
            if (!c.Data.NietGebruikenBitmap &&
                !string.IsNullOrWhiteSpace(c.Data.BitmapNaam) &&
                !string.IsNullOrEmpty(DataAccess.TLCGenControllerDataProvider.Default.ControllerFileName))
            {
                var path = Path.GetDirectoryName(DataAccess.TLCGenControllerDataProvider.Default.ControllerFileName);
                var krpfile = Path.Combine(path, (Regex.IsMatch(c.Data.BitmapNaam, @"\.(bmp|BMP)$") ? c.Data.BitmapNaam + "_krp": c.Data.BitmapNaam + "_krp.bmp"));
                var file = Path.Combine(path, (Regex.IsMatch(c.Data.BitmapNaam, @"\.(bmp|BMP)$") ? c.Data.BitmapNaam : c.Data.BitmapNaam + ".bmp"));
                if (File.Exists(krpfile))
                {
                    OpenXmlHelper.AddImageToBody(doc, krpfile);
                }
                else if (File.Exists(file))
                {
                    OpenXmlHelper.AddImageToBody(doc, file);
                }
                sb.Append($"Het kruispunt {GetKruisingNaam(c)} wordt schematisch weergegeven in de bovenstaande figuur. ");
            }

            var sl = new List<Tuple<string, int>>();
            if (c.InterSignaalGroep.Gelijkstarten.Any(x => x.DeelConflict) ||
                c.InterSignaalGroep.Voorstarten.Any())
            {
                sb.Append("De volgende richtingen woren in deelconflict afgehandeld: ");
                body.Append(OpenXmlHelper.GetTextParagraph(sb.ToString()));
                sb.Clear();

                foreach(var gs in c.InterSignaalGroep.Gelijkstarten.Where(x => x.DeelConflict))
                {
                    sl.Add(new Tuple<string, int>($"{(string)Texts["Generic_Fase"]} {gs.FaseVan} (gelijkstart met {(string)Texts["Generic_fase"]} {gs.FaseNaar})", 0));
                }
                foreach (var vs in c.InterSignaalGroep.Voorstarten)
                {
                    sl.Add(new Tuple<string, int>($"{(string)Texts["Generic_Fase"]} {vs.FaseVan} (voorstart op {(string)Texts["Generic_fase"]} {vs.FaseNaar})", 0));
                }
                body.Append(OpenXmlHelper.GetBulletList(doc, sl));
            }
            else
            {
                sb.Append("Alle bewegingen worden conflictvrij afgehandeld. ");
            }
            body.Append(OpenXmlHelper.GetTextParagraph(sb.ToString()));
            sb.Clear();

            body.Append(OpenXmlHelper.GetTextParagraph("Het regelprogramma is ontworpen om zo goed mogelijk de volgende doelen te bereiken: "));
            sl.Clear();
            sl.Add(new Tuple<string, int>("een veilige situatie op het kruispunt;", 0));
            sl.Add(new Tuple<string, int>("een goede en efficiënte verkeersafwikkeling;", 0));
            sl.Add(new Tuple<string, int>("een logische en acceptabele situatie voor alle weggebruikers.", 0));
            body.Append(OpenXmlHelper.GetBulletList(doc, sl));

            body.Append(OpenXmlHelper.GetTextParagraph("", "Footer"));

            body.Append(OpenXmlHelper.GetTextParagraph(
                "In het regelprogramma is in het commentaar een functionele " +
                "beschrijving van de parameters, tijden, schakelaars en dergelijke opgenomen."));

            body.Append(OpenXmlHelper.GetTextParagraph("", "Footer"));

            /* pagebreak toegevoegd */
            body.Append(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));

            body.Append(OpenXmlHelper.GetChapterTitleParagraph("Algemene instellingen", 2));

            body.Append(OpenXmlHelper.GetTextParagraph(
                "Onderstaande tabel geeft een overzicht van een aantal algemene instellingen van de regeling."));

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
            l.Add(new List<string> { (string)Texts["Generic_Intergroen"], c.Data.Intergroen.ToCustomString() });
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
            if (c.Fasen.Any(x => x.WachttijdVoorspeller))
            {
                l.Add(new List<string> { (string)Texts["Generic_WTVNietHalterenAlsMeerDanLeds"], c.Data.WachttijdvoorspellerNietHalterenMax.ToString() });
                l.Add(new List<string> { (string)Texts["Generic_WTVNietHalterenAlsMinderDanLeds"], c.Data.WachttijdvoorspellerNietHalterenMin.ToString() });
                l.Add(new List<string> { (string)Texts["Generic_WTVAansturenBusTijdensOV"], c.Data.WachttijdvoorspellerAansturenBus.ToCustomString() });
                l.Add(new List<string> { (string)Texts["Generic_WTVAansturenBusTijdensHD"], c.Data.WachttijdvoorspellerAansturenBusHD.ToCustomString() });
            }

            body.Append(OpenXmlHelper.GetTable(l));

            body.Append(OpenXmlHelper.GetTextParagraph(
                ""));

            body.Append(OpenXmlHelper.GetChapterTitleParagraph("Prioriteit instellingen", 2));
            body.Append(OpenXmlHelper.GetTextParagraph(
                "Onderstaande tabel geeft een overzicht van een aantal algemene instellingen rond prioriteitsverlening."));
            l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Optie"],
                    (string)Texts["Generic_Instelling"]
                }
            };
            l.Add(new List<string> { (string)Texts["Prio_CheckDSin"], c.PrioData.CheckOpDSIN.ToCustomString() });
            l.Add(new List<string> { (string)Texts["Prio_MaxWtMvt"], c.PrioData.MaxWachttijdAuto.ToString() });
            l.Add(new List<string> { (string)Texts["Prio_MaxWtFts"], c.PrioData.MaxWachttijdFiets.ToString() });
            l.Add(new List<string> { (string)Texts["Prio_MaxWtVtg"], c.PrioData.MaxWachttijdVoetganger.ToString() });
            l.Add(new List<string> { (string)Texts["Prio_GecPrioTV"], c.PrioData.GeconditioneerdePrioGrensTeVroeg.ToString() });
            l.Add(new List<string> { (string)Texts["Prio_GecPrioTL"], c.PrioData.GeconditioneerdePrioGrensTeLaat.ToString() });
            l.Add(new List<string> { (string)Texts["Prio_HD_BL_NC"], c.PrioData.BlokkeerNietConflictenBijHDIngreep.ToCustomString() });
            if (c.PrioData.BlokkeerNietConflictenBijHDIngreep)
                l.Add(new List<string> { (string)Texts["Prio_HD_BL_NC_LV"], c.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer.ToCustomString() });
            l.Add(new List<string> { (string)Texts["Prio_PrioUber"], c.PrioData.VerklikkenPrioTellerUber.GetDescription() });
            l.Add(new List<string> { (string)Texts["Prio_VerlaagSG"], c.PrioData.VerlaagHogeSignaalGroepNummers.ToCustomString() });
            l.Add(new List<string> { (string)Texts["Prio_KARinPRM"], c.PrioData.KARSignaalGroepNummersInParameters.ToCustomString() });


            body.Append(OpenXmlHelper.GetTable(l));
        }

        public static List<OpenXmlCompositeElement> GetChapter_StructureIntroduction(ControllerModel c)                                // hfdst 2.0 Regelstructuur en afhandeling  
        {
            var items = new List<OpenXmlCompositeElement>();
            var t = "";
            if (c.HalfstarData.IsHalfstar)
            {
                t = $"De kruising wordt zowel halfstar (PL) als voertuigafhankelijk (VA) geregeld. ";
            }
            else
            {
                t = $"De kruising wordt voertuigafhankelijk (VA) geregeld. ";
            }
            t +=  "VA regelen houdt in dat de signaalgroepen binnen de regeling groen krijgen toegedeeld " +
                 $"op basis van de aanwezigheid van voertuigen. Hierbij gelden periode gebonden {GetGroentijden(c)}. " +
                 $"In dit hoofdstuk worden de voertuigafhankelijke signaalgroepafhandeling en de modulenstructuur " +
                 $"beschreven.";
            if (c.HalfstarData.IsHalfstar)
            {
                t += $" De functionaliteit van en instellingen omtrent halfstar regelen komen " +
                     $"in het hoofdstuk inzake halfstar regelen aan de orde. ";
            }
            
            items.Add(OpenXmlHelper.GetTextParagraph(t));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_SignaalGroepAfhandeling(WordprocessingDocument doc, ControllerModel c)  // hfdst 2.1 Signaalgroepafhandeling  
        {
            var items = new List<OpenXmlCompositeElement>
            {
                OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_SignaalgroepAfhandeling"]}", 2),
                OpenXmlHelper.GetTextParagraph(
                    "De signaalgroepen worden volgens een vaste procedure afgehandeld (standaard CCOL afhandeling). " +
                    "De signaalgroepafhandeling is verdeeld in een aantal toestanden die alle doorlopen (moeten) worden. " +
                    "Een aantal van deze toestanden " +
                    "is tijdsafhankelijk. In het navolgende worden de belangrijkste signaalgroeptoestanden en " +
                    "signaalgroeptijden in chronologische volgorde besproken. "),
                OpenXmlHelper.GetTextParagraph("Garantierood(tijd)", bold: true),
                OpenXmlHelper.GetTextParagraph(
                    "De garantieroodtijd is de tijd dat een signaalgroep minimaal rood is. Deze tijd is bedoeld om " +
                    "een onrustig beeld voor weggebruikers te voorkomen (‘flitsrood’)."),
                OpenXmlHelper.GetTextParagraph("Garantiegroen(tijd)", bold: true),
                OpenXmlHelper.GetTextParagraph(
                    "Garantiegroentijden hebben een relatie met de veiligheid van de verkeerslichtenregeling. Na begin groen " +
                    "kijken weggebruikers vaak een aantal seconden niet naar het verkeerslicht. Bij te korte " +
                    "garantiegroentijden bestaat het gevaar dat weggebruikers ongemerkt door rood rijden."),
                OpenXmlHelper.GetTextParagraph("Vastgroen(tijd)", bold: true),
                OpenXmlHelper.GetTextParagraph(
                    "De vastgroentijd is afhankelijk van het detectieveld. De vastgroentijd moet voldoende zijn, " +
                    "om ervoor te zorgen dat (auto)verkeer dat tussen de stopstreep en de eerste velenglus staat, tijdens de " +
                    "vastgroentijd weg kan rijden. Bij het gebruik van een (beperkte) verlengfunctie op de koplus " +
                    "hoeft hier geen rekening mee gehouden te worden en kan de vastgroentijd gelijk zijn aan de " +
                    "garantiegroentijd."),
                OpenXmlHelper.GetTextParagraph(
                    "Bij fiets- en voetgangersrichtingen wordt vaak een vastgroentijd aangehouden die hoger ligt dan de garantiegroentijd."),
                OpenXmlHelper.GetTextParagraph("Wachtgroen", bold: true),
                OpenXmlHelper.GetTextParagraph(
                    "Wanneer er op een signaalgroep verkeer aanwezig is en er geen conflictaanvragen zijn, kan de " +
                    "betreffende signaalgroep in wachtgroen worden vastgehouden. Dit is een instelling die vast in TLCCGen moet worden opgegeven." +
                    "In dat geval gaat een signaalgroep pas na een conflictaanvraag " +
                    "daadwerkelijk verlengen.Op deze manier wordt voorkomen dat het groen van een drukke signaalgroep " +
                    "plotseling wordt beëindigd door één voertuig op een rustige conflicterende signaalgroep. " +
                    "Ook wordt in dat geval een signaalgroep die aan het meeverlengen is weer teruggezet naar wachtgroen op het " +
                    "moment dat er weer verkeer in het detectieveld aanwezig is en er geen conflictaanvragen zijn."),
                OpenXmlHelper.GetTextParagraph(
                    "Daarnaast kan de toestand wachtgroen worden gebruikt om een richting in wachtstand groen te houden of om een richting " +
                    "groen te houden die een naalooprichting is van een voedende richting die groen (of bijna groen) is."),
                OpenXmlHelper.GetTextParagraph("Verlenggroen", bold: true),
                OpenXmlHelper.GetTextParagraph(
                    "Gedurende de verlenggroenfase kan een signaalgroep bij continu verkeersaanbod (d.w.z. zolang het meetkriterium op staat) groen blijven tot " +
                    "het primaire groengebied is verstreken. De primaire groengebieden kunnen afhankelijk van de " +
                    $"klokperiode variëren. Er zijn in deze regeling {c.GroentijdenSets.Count.ToString()} verschillende klokperioden opgenomen."),
                OpenXmlHelper.GetTextParagraph("Meeverlenggroen", bold: true),
                OpenXmlHelper.GetTextParagraph(
                    "Signaalgroepen die groen zijn en dit kunnen blijven zonder dat hierdoor andere signaalgroepen " +
                    "worden tegengehouden, kunnen meeverlenggroen krijgen na het verlopen van hun hiaattijden of maximum " +
                    "groentijd. Door het gebruik van meeverlenggroen wordt de restruimte in de verkeerslichtenregeling benut " +
                    "en verbetert de logica en acceptatie van de regeling. Hierbij wordt rekening gehouden " +
                    "met het verschil in geel- en ontruimingstijden of intergroentijden."),
                OpenXmlHelper.GetTextParagraph("Garantiegeel(tijd)", bold: true),
                OpenXmlHelper.GetTextParagraph(
                    "De garantiegeeltijd is de tijd dat een signaalgroep minimaal geel is. Deze tijd is bedoeld om een " +
                    "onrustig beeld voor weggebruikers te voorkomen (‘flitsgeel’) en dient tevens om weggebruikers te 'garanderen' dat zij " +
                    "tijdig tot stilstand te komen. De garantiegeeltijd is daarmee mede afhankelijk van de maximale snelheid op de richitng."),
                OpenXmlHelper.GetTextParagraph("Geel(tijd)", bold: true),
                OpenXmlHelper.GetTextParagraph(
                    "De geeltijd moet niet te laag en niet te hoog worden ingesteld. Een te lage waarde kan voertuigen " +
                    "die niet meer tijdig kunnen stoppen in onveilige situaties brengen. Bij te hoge waarden blijven " +
                    "bestuurders, die eigenlijk hadden kunnen stoppen, doorrijden."),
                OpenXmlHelper.GetTextParagraph("", "Footer")
            };
            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_SignaalGroepInstellingen(WordprocessingDocument doc, ControllerModel c) // hfdst 2.2 Signaalgroepinstellingen  
        {
            var items = new List<OpenXmlCompositeElement>();
            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_SignaalgroepInstellingen"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Hieronder worden de instellingen voor de {Texts["Generic_fasen"]} weergegeven. Eerst volgt een " +
                $"tabel met de kenmerken van voor de verschillende fasen en de instelde functie(s). Daarna volgt een tabel met de ingestelde tijden."));
            items.AddRange(TableGenerator.GetTable_FasenFuncties(c));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            items.AddRange(TableGenerator.GetTable_FasenTijden(c));
            items.Add(OpenXmlHelper.GetTextParagraph(""));
            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_Perioden(ControllerModel c)                                             // hfdst 2.3 Klokperioden ToDo uitleg stkp  
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Perioden"]}", 2));

            if (c.HalfstarData.IsHalfstar)
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Op het moment dat de regeling voertuigafhankelijk regelt, gelden de volgende " +
                    $"klokperioden ten behoeve van de {GetGroentijden(c)}:"));
            }
            else
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Voor voertuigafhankelijk regelen gelden de volgende " +
                    $"klokperioden ten behoeve van de {GetGroentijden(c)}:"));
            }

            items.AddRange(TableGenerator.GetTable_Perioden(c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_Groentijden(ControllerModel c)                                          // hfdst 2.4 Groentijden ToDo uitleg PRM MG##_$  
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Groentijden"]}", 2));

            var gtijd = "";
            switch (c.Data.TypeGroentijden)
            {
                case Models.Enumerations.GroentijdenTypeEnum.MaxGroentijden:
                    gtijd = "maximum groentijd";
                    break;
                case Models.Enumerations.GroentijdenTypeEnum.VerlengGroentijden:
                    gtijd = "verlenggroentijd";
                    break;
            }

            var t = $"Voor {Texts["Generic_fasen"]} kan per klokperiode de {gtijd} worden ingesteld. ";
            switch (c.Data.TypeGroentijden)
            {
                case GroentijdenTypeEnum.MaxGroentijden:
                    t += $"De {gtijd} is de tijd dat een {Texts["Generic_fasen"]} maximaal kan groen kan krijgen op basis van de " +
                        $"aanwezigheid van verkeer. Hierbij wordt gekeken naar de totale tijd van vastgroen en verlenggroen.";
                    break;
                case GroentijdenTypeEnum.VerlengGroentijden:
                    t += $"De {gtijd} is de tijd dat het groen voor een {Texts["Generic_fasen"]} maximaal verlengen op basis van de " +
                        $"aanwezigheid van verkeer.";
                    break;
            }
            items.Add(OpenXmlHelper.GetTextParagraph(t));

            items.AddRange(TableGenerator.GetTable_Fasen_Groentijden(c));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_Modulestructuur(WordprocessingDocument doc, ControllerModel c)          // hfdst 2.5 Modulestructuur  
        {
            var items = new List<OpenXmlCompositeElement>();

            var tableModulen = TableGenerator.GetTable_Modulen(c);

            items.Add(OpenXmlHelper.GetTextParagraph($" "));
            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Modulestructuur"]}", 2));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Tijdens voertuigafhankelijk regelen wordt gebruik gemaakt van de modulestructuur. De " +
                "modulestructuur is opgebouwd uit primaire, alternatieve en bijzondere realisaties."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Aan het eind van deze paragraaf worden in tabelvorm de diverse instellingen weergegeven."));
            items.Add(OpenXmlHelper.GetTextParagraph("Primaire realisatie", bold: true));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "De modulestructuur bepaalt in welke vaste volgorde de verschillende signaalgroepen met een aanvraag " +
                "groen worden (primaire realisaties). Als geen enkele signaalgroep een aanvraag heeft dan wacht de " +
                $"modulestructuur in module {c.ModuleMolen.WachtModule}. Een moduleovergang vindt plaats op het moment dat alle primair " +
                "toegedeelde signaalgroepen met een aanvraag van het actieve blok cyclisch verlenggroen hebben " +
                "verlaten."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Alle signaalgroepen die 'op straat' een lichtbeeld tonen, zijn toegedeeld aan minimaal één module (zie tabel {TableGenerator.Tables["Table_Modulen"]}). Een signaalgroep die " +
                "een aanvraag heeft gekregen (in het aanvraaggebied) wordt tijdens het actief zijn van de module " +
                "waaraan deze is toegedeeld, eenmalig primair gerealiseerd. De groenduur van een primaire " +
                "realisatie wordt beperkt volgens de ingestelde maximumgroentijd (zie tabel " +
                $"{(c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? TableGenerator.Tables["Table_MaxGroentijden"] : TableGenerator.Tables["Table_VerlGroentijden"])}) per klokperiode " +
                $"(zie tabel {TableGenerator.Tables["Table_Perioden"]})."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                 "Het aanvraaggebied voor primair toegedeelde richtingen van een module wordt afgesloten nadat " +
                 "de desbetreffende module actief is geworden en er een aanvraag is gezet door een signaalgroep " +
                 "uit een volgende module. Bij de moduleovergang wordt het aanvraaggebied van de richtingen van " +
                 "de module die verlaten is, weer opengesteld."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                  "Door het gebruik van versnelde primaire realisaties worden richtingen onder voorwaarden (bijvoorbeeld afwezigheid van aanvragen " +
                  "op eerder in de regelstructuur te realiseren conflictrichtingen) eerder groen. Versnelde realisaties geven, met name in rustige perioden, " +
                  "een grote flexibiliteit aan de verkeersregeling waardoor de acceptatie en geloofwaardigheid van de verkeersregeling hoger wordt. " +
                  "Met behulp van een parameter is per richting in te stellen hoeveel module er vooruit mag worden gerealiseerd. " + 
                  "Default mogen alle richtingen één module vooruit realiseren."));

            items.Add(OpenXmlHelper.GetTextParagraph("Alternatieve realisaties", bold: true));
            if (c.ModuleMolen.LangstWachtendeAlternatief)
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    "Met alternatieve realisaties kunnen extra realisatiemogelijkheden worden toegekend. Deze alternatieve " +
                    "realisaties worden volgens het langstwachtende principe afgewikkeld, dat wil zeggen dat wanneer " +
                    "er op een bepaald moment meerdere richtingen alternatief kunnen realiseren, de richting met de " +
                    "hoogste wachttijd als eerste groen krijgt. De alternatieve realisaties zijn dusdanig opgenomen " +
                    "dat ze de regeling zo min mogelijk ophouden."));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    "Per richting is een schakelaar opgenomen om alternatieve realisaties toe te staan. " +
                    "Daarnaast is per richting een referentie opgeven hoeveel ruimte er in de " +
                    "regeling beschikbaar moet zijn om alternatief te mogen realiseren. Deze ruimte wordt berekend op basis van " +
                    $"de vigerende {GetGroentijden(c, false)} van een " +
                    "niet-conflict dat op dat moment in cyclisch verlenggroen staat, of op het punt staat om primair " +
                    "dan wel bijzonder te realiseren. Hierbij wordt er voor richtingen met een naloop rekening mee gehouden " +
                    "dat ook die moet passen. Voor iedere richting is opgegeven wat de maximale groentijd voor " +
                    "alternatief realiseren is, als de voorwaarde voor alternatief realiseren is vervallen."));
            }
            else
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    "Alternatieve relatisaties zijn binnen de regeling uitsluitend mogelijk voor richtingen waarvoor expliciet is " +
                    "ingesteld dat deze onder dekking van een of meer andere richtingen alternatief mogen komen. Dit is ingesteld " +
                    "per richting per blok."
                    ));
                
            }

            items.Add(OpenXmlHelper.GetTextParagraph("Bijzondere realisaties", bold: true));
            
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Naast de primaire realisaties kunnen signaalgroepen ook bijzonder realiseren. Bij een bijzondere " +
                "realisatie wordt een signaalgroep buiten de modulestructuur om naar groen gestuurd. Bijzondere " +
                "realisaties kunnen worden gebruikt om prioriteit te geven aan bussen of hulpdiensten, of om een groensturing " +
                "af te dwingen (bijvoorbeeld wanneer een wachttijdvoorspeller onder een bepaalde waarde komt, om te vookomen dat deze blijft 'hangen')."));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            
            items.Add(OpenXmlHelper.GetTextParagraph("Tabellen modulestructuur", bold: true));

            items.AddRange(tableModulen);

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.AddRange(TableGenerator.GetTable_ModuleStructuurInstellingen(doc, c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            if (!c.ModuleMolen.LangstWachtendeAlternatief)
            {
                items.AddRange(TableGenerator.GetTable_AlternatievenOnderDekking(c));
            }

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;

        }

        public static List<OpenXmlCompositeElement> GetChapter_VasteAanvragen(ControllerModel c)                                       // hfdst 2.6 vaste aanvragen  
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_VasteAanvragen"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph($"Richtingen kunnen een vaste (cyclische) aanvraag krijgen, eventueel met een instelbare vertraging. " +
                $"Hieronder worden de ingestelde cyclische aanvragen weergegeven, alsmede de eventuele instellingen voor een vertraging:"));

            items.AddRange(TableGenerator.GetTable_VasteAanvragen(c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_Meeverlengen(ControllerModel c)                                         // hfdst 2.7 Meeverlengen  
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Meeverlengen"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph($"Voor de {Texts["Generic_fasen"]} kan meeverlengen worden ingesteld, hetzij algemeen hetzij specifiek met een richting. " +
                $"De algemene instelling per fase staat weeergegeven in tabel {TableGenerator.Tables["Table_Fasen_Functies"]}; Hieronder wordt het type meeverlengen per fase " +
                $"en eventuele instellingen van hard meeverlengen weergegeven:"));

          items.AddRange(TableGenerator.GetTable_Meeverlengen(c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_Wachtgroen(ControllerModel c)                                         // hfdst 2.7 Meeverlengen  
        {
            var items = new List<OpenXmlCompositeElement>
            {
                OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Wachtgroen"]}", 2),
                OpenXmlHelper.GetTextParagraph(
                    $"Voor de {Texts["Generic_fasen"]} kan het type wachtstand worden ingesteld per richting. " +
                    $"De algemene instelling per fase staat weergegeven in tabel {TableGenerator.Tables["Table_Fasen_Functies"]}; " +
                    $"Hieronder wordt het type wachtstand per fase weergegeven:")
            };

            items.AddRange(TableGenerator.GetTable_Wachtgroen(c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_Veiligheidsgroen(ControllerModel c)                                     // hfdst 2.8 Veiligheidsgroen  
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!(c.Fasen.Any(x => x.Detectoren.Any(y => y.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit)))) return items;

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Veiligheidsgroen"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph($"Voor de onderstaande {Texts["Generic_fasen"]} en detectoren kan veiligheidsgroen worden ingesteld: "));

            items.AddRange(TableGenerator.GetTable_Veiligheidsgroen(c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_Senioreningreep(ControllerModel c)                                      // hfdst 2.9 Senioreningreep  
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!(c.Fasen.Any(x => x.SeniorenIngreep != NooitAltijdAanUitEnum.Nooit))) return items;

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Senioreningreep"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph("Een senioreningreep houdt een (voetgangers)richting na het vastgroen een " +
                "instelbaar percentage van dat vastgroen vast in wachtgroen, zodat voetgangers die moeilijk ter been zijn meer " +
                "gelegenheid hebben om de oversteek te maken. Zij dienen daartoe een instelbaar aantal seconden achtereen " +
                "de aanvraagknop ingedrukt te houden. Er kunnen drie opeenvolgende oversteken bediend worden. Eventuele " +
                "volgoversteken hebben ieder hun eigen instelling voor extra groen."));
            items.Add(OpenXmlHelper.GetTextParagraph("Wanneer het seniorengroen aktief is, worden eventuele nalopen pas " +
                "gestart op het einde van de verlengde groentijd van de voedende richting (in plaats van op start groen), " +
                "zodat men ook gelegenheid heeft om in trager tempo de naloop over te steken."));
            
            items.AddRange(TableGenerator.GetTable_Senioreningreep(c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_Schoolingreep(ControllerModel c)                                        // hfdst 2.10 Schoolingreep  
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!(c.Fasen.Any(x => x.SchoolIngreep != NooitAltijdAanUitEnum.Nooit))) return items;

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Schoolingreep"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph("Bij een schoolingreep blijft een voetgangersoversteek " +
                "langer groen (tot een instelbaar maximum vanaf start groen) zolang de drukknop ingedrukt blijft. " +
                "In deze tijd kan dan een schoolklas in zijn geheel oversteken. Als terugkoppeling naar de " +
                "begeleid(st)er gaat het waitsignaal van de drukknop knipperen zolang de ingreep actief is."));
            items.Add(OpenXmlHelper.GetTextParagraph("De knop moet worden ingedrukt tijdens rood of onmiddellijk " +
                "na het groen worden, en ingedrukt blijven tot de laatste persoon aan de oversteek is begonnen."));
            items.Add(OpenXmlHelper.GetTextParagraph("Nadat de knop is losgelaten blijft de richting nog gedurende " +
                "een instelbare hiaattijd (TDH) groen."));

            items.AddRange(TableGenerator.GetTable_Schoolingreep(c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetModulenChapter(WordprocessingDocument doc, ControllerModel c)                   // hfdst 2.5      ongebruikt ??????????  
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

            return items;
        }

        internal static List<OpenXmlCompositeElement> GetChapter_DetectieConfiguratie(WordprocessingDocument doc, ControllerModel c)   // hfdst 3.1 Configuratie detectoren -> hernoemen naar 'Inleiding' ???    
        {
            var body = doc.MainDocumentPart.Document.Body;
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren_Configuratie"]}", 2));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"De belangrijkste doelen van het detectieveld zijn:"));

            var sl = new List<Tuple<string, int>>();

            sl.Add(new Tuple<string, int>($"Het aanvragen van signaalgroepen (bij lussen pas na het verstrijken " +
                $"van de bezettijd). Per lus of drukknop is met een parameter instelbaar of en wanneer " +
                $"de aanvraag gezet mag worden:", 0));
            sl.Add(new Tuple<string, int>($"0 = Aanvraagfunctie uitgeschakeld;", 1));
            sl.Add(new Tuple<string, int>($"1 = Aanvragen na garantierood;", 1));
            sl.Add(new Tuple<string, int>($"2 = Aanvragen tijdens de gehele roodfase;", 1));
            sl.Add(new Tuple<string, int>($"3 = Aanvragen tijdens geel en rood.", 1));
            sl.Add(new Tuple<string, int>($"Het op een veilige manier verlengen en beëindigen van signaalgroepen " +
                $"(bij lussen pas na het verstrijken van de hiaattijd). Per lus is met een parameter instelbaar " +
                $"of en volgens welk principe er verlengd mag worden: ", 0));
            sl.Add(new Tuple<string, int>($"0 = Verlengfunctie uitgeschakeld;", 1));
            sl.Add(new Tuple<string, int>($"1 = Koplusmaximum: beperkt gedeelte van maximum groentijd. Bedoeld " +
                $"voor koplussen, om te voorkomen dat, wanneer het eerste voertuig niet oplet, de groenfase na " +
                $"de vastgroentijd wordt beëindigd. Op deze manier kunnen de voertuigen tussen de koplus en de " +
                $"lange lus toch worden weggewerkt. Op koplussen wordt in principe niet doorverlengd: voertuigen " +
                $"die bij de koplus arriveren, zijn de dilemmazone al gepasseerd en rijden altijd door;", 1));
            sl.Add(new Tuple<string, int>($"2 = Eerste meetkriterium: zolang de hiaattijd van een lus met het " +
                $"eerste meetkriterium loopt, wordt er verlengd mits het primaire groengebied nog niet is " +
                $"verstreken. Na het verstrijken van de hiaattijd van een lus met het eerste meetkriterium, " +
                $"wordt er op deze lus niet meer verlengd als er ook een lus met het tweede meetkriterium is. " +
                $"Als er alleen lussen zijn met het eerste meetkriterium mogen ze elkaar opdrempelen;", 1));
            sl.Add(new Tuple<string, int>($"3 - Tweede meetkriterium: zolang de hiaattijd van een lus met het " +
                $"tweede meetkriterium loopt, wordt er verlengd mits het primaire groengebied nog niet is " +
                $"verstreken. Ook al is de hiaattijd van een lus een keer verstreken, deze lus mag altijd " +
                $"opnieuw meedoen bij het bepalen van het meetkriterium als er verlengd is op een andere " +
                $"lus. Lussen met het tweede meetkriterium mogen elkaar opdrempelen.", 1));

            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        internal static List<OpenXmlCompositeElement> GetChapter_DetectieInstellingen(WordprocessingDocument doc, ControllerModel c)   // hfdst 3.2 Instellingen detectoren ToDo: check flutter eenheden   
        {
            var body = doc.MainDocumentPart.Document.Body;
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren_Instellingen"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Hieronder worden eerst de functionele instellingen voor de detectoren op de kruising weergegeven. Vervolgens worden " +
                $"de tijdsinstellingen voor de detectoren weergegeven."));

            items.AddRange(TableGenerator.GetTable_Detectie_Functies(c));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            items.AddRange(TableGenerator.GetTable_Detectie_Tijden(c));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        internal static List<OpenXmlCompositeElement> GetChapter_Dynamischiaat(WordprocessingDocument doc, ControllerModel c)          // hfdst 3.3 Dynamische hiaattijden ToDo: check flutter eenheden   
        {
            var body = doc.MainDocumentPart.Document.Body;
            var items = new List<OpenXmlCompositeElement>();
            var sl = new List<Tuple<string, int>>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Dynamische_Hiaattijden"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                "De regeling is voorzien van dynamische hiaattijden. Hiermee kan - afhankelijk van de ingestelde waarden - zowel " +
                "een Groen op Maat (GOM) detectieveld als een detectieveld conform het IVER rapport uit 2018 (IVER'18) worden bediend. " +
                "Dynamische hiaattijden zijn bedoeld om op efficiënte wijze groen te verlengen, waarbij aan de 'voorkant' " +
                "minder vastgroen en geen koplusmaximum nodig zijn, en aan de 'achterkant' gebruik gemaakt kan worden " +
                "van een deel van de geeltijd." +
                ""));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "De methodiek is als GOM ontwikkeld door Luuk Misdom en IT&T (nu Vialis) en in augustus 2018 in aangepaste vorm " +
                "overgenomen door IVER. Zie voor de rapportage 'Onderzoek detectieconfiguratie en signaalgroepafhandeling' " +
                "van Goudappel Coffeng (in opdracht van IVER): https://www.crow.nl/thema-s/verkeersmanagement/iver onder 'Downloads'." +
                ""));

            items.Add(OpenXmlHelper.GetTextParagraph(
                "Bij dynamische hiaattijden volgens GOM of IVER'18 wordt de ingestelde maximum hiaattijd op de verschillende detectielussen " +
                "na een instelbare tijd lager naarmate de (groen)tijd vordert. Het aflopen van de maximum hiaattijd begint op 'moment 1' " +
                "en eindigt op 'moment 2'. Vanaf start groen tot aan moment 1 geldt TDH 1, vanaf moment 2 tot aan einde groen geldt TDH 2. " +
                "Tussen moment 1 en moment 2 loopt de maximum hiaattijd lineair af van TDH 1 tot TDH 2. " +
                "In de tabel aan het eind van deze paragraaf wordt een overzicht gegeven van de gebruikte instellingen, die eerst worden verklaard: " +
                ""));

            sl.Add(new Tuple<string, int>("SCH edkop_##:                                                                                                                                   " +
                "Betreft keuze voor moment start aftellen t.b.v. moment 1 en moment 2. AAN is beginnen op einde detectie koplus, UIT is beginnen op start groen.", 0));
            sl.Add(new Tuple<string, int>("SCH opdrempelen##:                                                                                                                              " +
                "Keuze wel / niet opdrempelen; bij AAN mag er worden opgedrempeld, bij UIT geldt gescheiden hiaatmeting per rijstrook (aleen bij meerdere rijstroken per signaalgroep).", 0));
            sl.Add(new Tuple<string, int>("PRM springverleng_$$:                                                                                                                           " +
                "BITsgewijze instelling van het gewenste gedrag van de detectoren onder verschillende omstandigheden, vaak bedoeld om onderscheid " +
                "te kunnen maken in detectiegedrag tussen verkeer op snelheid en vertragend of optrekkend verkeer. Daarbij geldt:", 0));
            sl.Add(new Tuple<string, int>(" 1 = SpringStart:                                                                                                                               " +
                "op start groen, als er geen hiaatmeting is op de stroomafwaartse lussen, meteen naar de 2e / lagere hiaattijd overgaan: ", 1));
            sl.Add(new Tuple<string, int>(" 2 = VerlengNiet:                                                                                                                               " +
                "op start groen, als er geen hiaatmeting (meer) aktief is op deze en de stroomafwaartse lussen, de verlengfunctie UITschakelen", 1));
            sl.Add(new Tuple<string, int>(" 4 = VerlengExtra:                                                                                                                              " +
                "altijd verlengen op deze lus; bijvoorbeeld bij permanente aanwezigheid deelconflict", 1));
            sl.Add(new Tuple<string, int>(" 8 = DirectAftel:                                                                                                                               " +
                "tijdens groen, als er wél hiaatmeting is op deze lus maar niet op de stroomafwaartse lussen, meteen TDH_max[] gaan aftellen", 1));
            sl.Add(new Tuple<string, int>("16 = SpringGroen:                                                                                                                               " +
                "wanneer tijdens groen het hiaat valt, wordt de volgende detector stroomopwaarts de aktieve verlenglus", 1));
            sl.Add(new Tuple<string, int>("T $$_1: moment 1 (tijd na start groen)", 0));
            sl.Add(new Tuple<string, int>("T $$_2: moment 2 (tijd na start groen)", 0));
            sl.Add(new Tuple<string, int>("T tdh_$$_1: TDH van start groen tot aan moment 1", 0));
            sl.Add(new Tuple<string, int>("T tdh_$$_2: TDH van moment 2 tot aan einde groen", 0));
            sl.Add(new Tuple<string, int>("T max_$$: maximale tijd na start groen dat de detector mag verlengen (0 = de vigerende verlenggroentijd aanhouden)", 0));

            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.AddRange(TableGenerator.GetTable_Detectie_DynHiaat(c));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
 
            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_DetectieRichtingGevoelig(ControllerModel c)                             // hfdst 3.4 Richtinggevoelige detectie ToDo check met M2, G5 
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!c.RichtingGevoeligeAanvragen.Any() && !c.RichtingGevoeligVerlengen.Any()) return items;

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren_Richtinggevoelig"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Detectoren kunnen in paren benut worden voor het richtinggevoelig aanvragen en/of richtinggevoelig verlengen van {Texts["Generic_fasen"]}. " +
                $"Hieronder wordt een overzicht gegeven van deze functionaliteit."));

            if (c.RichtingGevoeligeAanvragen.Any())
            {
                items.AddRange(TableGenerator.GetTable_Detectie_RGA(c));
            }

            if (c.RichtingGevoeligVerlengen.Any())
            {
                items.AddRange(TableGenerator.GetTable_Detectie_RGV(c));
            }

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        internal static List<OpenXmlCompositeElement> GetChapter_DetectieStoring(WordprocessingDocument doc, ControllerModel c)        // hfdst 3.5 Detectiestoring  ToDo: kop+knop / kop+1e lang   
        {
            var body = doc.MainDocumentPart.Document.Body;
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren_Storing"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"De detectie dient in de TLC bewaakt te worden op boven- en ondergedrag. De tijden die hierbij gelden zijn weergegeven in de tabel " +
                $"met tijdsinstellingen voor detectoren (tabel {TableGenerator.Tables["Table_Detectoren_Tijden"]})."));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Het is mogelijk maatregelen te nemen wanneer een of meer detectoren voor een richting in storing komen. Sowieso geldt: " +
                $"van een lus in storing worden in de regelapplicatie alle functies uitgeschakeld. Daarnaast zijn de volgende maatregelen mogelijk:"));

            var sl = new List<Tuple<string, int>>();
            sl.Add(new Tuple<string, int>($"Per lus (of knop) is instelbaar dat een storing zorgt voor een vaste aanvraag. Deze instelling is opgenomen " +
                $"in de tabel met detectie functies (tabel {TableGenerator.Tables["Table_Detectoren_Functies"]}).", 0));
            sl.Add(new Tuple<string, int>($"Bij storing van de lange lus kan de koplus de verlengfunctie van de lange lus (deels) overnemen met een extra " +
                $"per koplus instelbare hiaattijd.", 0));
            sl.Add(new Tuple<string, int>($"Als van een {Texts["Generic_fase"]} alle aanvraag detectie in storing is, kan worden ingesteld dat " +
                $"(schakelbaar) een vaste aanvraag wordt gezet, eventueel met een instelbare tijdvertraging.", 0));
            sl.Add(new Tuple<string, int>($"Optioneel kan deze functionaliteit beperkt worden tot een storing van alleen de koplus en de (eerste) verlenglus.", 1));
            sl.Add(new Tuple<string, int>($"Als van een {Texts["Generic_fase"]} alle verlengdetectie in storing is, kan worden ingesteld dat een instelbaar " +
                $"percentage van de actuele maximum of verlenggroentijd wordt gemaakt.", 0));
            sl.Add(new Tuple<string, int>($"Optioneel kan worden ingesteld dat bij aanwezigheid van een koplus en een drukknop, de storingsopvang " +
                $"alleen actief wordt wanneer die beide in storing staan (en dus niet wanneer slechts één van beide defect is).", 0));
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));

            items.Add(OpenXmlHelper.GetTextParagraph(""));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Hieronder worden de instellingen voor detectiestoring per {Texts["Generic_fase"]} weergegeven."));

            items.AddRange(TableGenerator.GetTable_Detectie_StoringMaatregelen(c));

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_Ontruimingstijden(ControllerModel c)                                    // hfdst 4.1 Ontruimingstijden  
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            var otTable = TableGenerator.GetTable_Ontruimingstijden(c);

            if (c.Data.Intergroen)
            {
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Intergroentijden"]}", 2));
                       
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"De intergroenen zijn opgenomen in tabel {TableGenerator.Tables["Table_Ontruimingstijden"]}. Deze tijden " +
                    $"zijn weergegeven in tienden van een seconde, van startgeel tot begin groen van signaalgroepen welke worden bewaakt volgens " +
                    $"NEN 3384 (2017) en CROW richtlijn 321. Daar waar geen waarde is ingevuld zijn signaalgroepen onderling niet bewaakt."));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    "De intergroentijden kunnen worden aangepast via de CCOL parser middels 'TIG <FCvan> <FCnaar>' doch niet lager worden ingesteld dan de garantie intergroentijden."));
            } 
            else
            {
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Ontruimingstijden"]}", 2));

                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"De ontruimingstijden zijn opgenomen in tabel {TableGenerator.Tables["Table_Ontruimingstijden"]}. Deze tijden " +
                    $"zijn weergegeven in tienden van een seconde, van eindgeel tot begin groen van signaalgroepen welke worden bewaakt volgens " +
                    $"NEN 3384 en de relevante CROW richtlijn. Daar waar geen waarde is ingevuld zijn signaalgroepen onderling niet bewaakt."));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    "De ontruimingstijden kunnen worden aangepast via de CCOL parser middels 'TO <FCvan> <FCnaar>' doch niet lager worden ingesteld dan de garantie ontruimingstijden."));
            }

            items.AddRange(otTable);

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_OntruimingstijdenGarantie(ControllerModel c)                            // hfdst 4.2 Garantie ontruimingstijden  
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!c.Data.GarantieOntruimingsTijden) return items;

            items.Add(OpenXmlHelper.GetTextParagraph(""));

            var otTable = TableGenerator.GetTable_OntruimingstijdenGarantie(c);

            if (c.Data.Intergroen)
            {
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_IntergroentijdenGarantie"]}", 2));

                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"De garantie intergroentijden zijn opgenomen in tabel {TableGenerator.Tables["Table_OntruimingstijdenGarantie"]}. De " +
                    $"intergroentijden kunnen niet lager worden ingesteld dan de garantie intergroentijden. De garantie " +
                    $"intergroentijden zelf zijn Read Only en niet in te stellen via CCOL."));
            }
            else
            {
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OntruimingstijdenGarantie"]}", 2));

                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"De garantie ontruimingstijden zijn opgenomen in tabel {TableGenerator.Tables["Table_OntruimingstijdenGarantie"]}. De " +
                    $"ontruimingstijden kunnen niet lager worden ingesteld dan de garantie ontruimingstijden. De garantie " +
                    $"ontruimingstijden zelf zijn Read Only en niet in te stellen via CCOL."));
            }
            items.AddRange(otTable);

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_Synchronisaties(ControllerModel c, WordprocessingDocument doc)          // hfdst 4.3 Synchronisaties
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetTextParagraph(""));

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Synchronisaties"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Dit hoofdstuk geeft een overzicht van de synchronisaties tussen signaalgroepen binnen de regeling."));

            if (c.InterSignaalGroep.Meeaanvragen.Any())
            {
                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Meeaanvragen"]}", 3));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Een meeaanvraag tussen twee signaalgroepen zorgt ervoor dat een aanvraag bij de ene signaalgroep " +
                    $"onder bepaalde voorwaarden zorgt dat ook de andere signaalgroep een aanvraag krijgt, ongeacht de " +
                    $"aanwezigheid van verkeer bij die tweede signaalgroep. Hierbij zijn de volgende opties mogelijk:"));
                var sl = new List<Tuple<string, int>>();
                sl.Add(new Tuple<string, int>("0 = Meeaanvraag uitgeschakeld.", 0));
                sl.Add(new Tuple<string, int>("1 = Meeaanvraag op aanvraag.", 0));
                sl.Add(new Tuple<string, int>("2 = Meeaanvraag op rood voor aanvraag (RA).", 0));
                sl.Add(new Tuple<string, int>("3 = Meeaanvraag op RA en geen conflicten", 0));
                sl.Add(new Tuple<string, int>("4 = Meeaanvraag op start groen", 0));
                items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Een meeaanvraag kan vaak als detectie-afhankelijke meeaanvraag worden ingesteld, zodat enkel bij een melding " +
                    $"op een bepaalde detector, de meeaanvraag wordt gezet. Optioneel is het mogelijk het " +
                    $"type meaanvraag instelbaar te maken op straat. Voor meeaanvragen op startgroen kan worden gekozen voor een " +
                    $"(instelbare) uitgestelde meeaanvraag. Voor meeaanvragen gelden de volgende instellingen:"));
                items.AddRange(TableGenerator.GetTable_Intersignaalgroep_Meeaanvragen(c));
            }

            if (c.InterSignaalGroep.Nalopen.Any())
            {
                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Nalopen"]}", 3));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Een naloop tussen twee signaalgroepen zorgt ervoor dat het groen op de volgrichting " +
                    $"onder bepaalde voorwaarden wordt vastgehouden wanneer de voedende richting groen is (geweest), ongeacht de " +
                    $"aanwezigheid van verkeer bij de volgrichting. Hierbij zijn de volgende voorwaarden mogelijk:"));
                var sl = new List<Tuple<string, int>>();
                sl.Add(new Tuple<string, int>("Naloop op startgroen:                            T nlsg(d)<FCvan><FCnaar>.", 0));
                sl.Add(new Tuple<string, int>("Naloop tijdens vastgroen:                     T nlfg(d)<FCvan><FCnaar>.", 0));
                sl.Add(new Tuple<string, int>("Naloop tijdens cyclisch verlengroen:  T nlcv(d)<FCvan><FCnaar>.", 0));
                sl.Add(new Tuple<string, int>("Naloop op einde groen:                         T nleg(d)<FCvan><FCnaar>.", 0));
                items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
                items.Add(OpenXmlHelper.GetTextParagraph(
                        "Daarnaast is het mogelijk om verkeer al te laten inrijden/inlopen tijdens het rood van de nalooprichting. Voor inlopen geldt daarbij " +
                        "T inl<FCvan><FCnaar>; voor inrijden wordt hierbij de late release toegepast: T lr<FCnaar><FCvan> (waarbij de fasen dus zijn verwisseld)."));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Een naloop kan als detectie afhankelijke naloop worden geconfigureerd, zodat enkel bij een melding " +
                    $"op een bepaalde detector, de naloop actief wordt. Bij detectieafhankelijke nalopen kan ervoor worden " +
                    $"gekozen geen vaste naloop aan te houden. Voor nalopen gelden de volgende instellingen:"));

                if (c.InterSignaalGroep.Nalopen.Any(x => x.Type == Models.Enumerations.NaloopTypeEnum.StartGroen))
                {
                    items.AddRange(TableGenerator.GetTable_Intersignaalgroep_Nalopen_SG(c));
                    items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                }

                if (c.InterSignaalGroep.Nalopen.Any(x => x.Type == Models.Enumerations.NaloopTypeEnum.CyclischVerlengGroen))
                {
                    items.AddRange(TableGenerator.GetTable_Intersignaalgroep_Nalopen_CV(c));
                    items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                }

                if (c.InterSignaalGroep.Nalopen.Any(x => x.Type == Models.Enumerations.NaloopTypeEnum.EindeGroen))
                {
                    items.AddRange(TableGenerator.GetTable_Intersignaalgroep_Nalopen_EG(c));
                    items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                }

            }

            if (c.InterSignaalGroep.Gelijkstarten.Any())
            {
                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Gelijkstarten"]}", 3));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Wanneer tussen twee signaalgroepen een gelijktart geldt, wordt ervoor gezorgd dat de betreffende " +
                    $"richtingen gelijktijdig groen krijgen wanneer beide een aanvraag hebben. Is er sprake van een deelconflict, " +
                    $"dan gelden aanvullend fictieve ontruimingstijden tussen beide richtingen, en mag een richting niet bijkomen " +
                    $"wanneer de andere richting reeds groen is."));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Voor gelijkstarten gelden de volgende instellingen:"));
                items.AddRange(TableGenerator.GetTable_Intersignaalgroep_Gelijkstarten(c));
            }

            if (c.InterSignaalGroep.Voorstarten.Any())
            {
                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Voorstarten"]}", 3));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Bij toepassen van een voorstart krijgt de ene richting altijd minimaal een instelbare tijd eerder groen dan het start groen " +
                    $"van de andere richting. Die tweede richting wordt hiertoe tijdelijk tegengehouden. Er geldt een fictieve " +
                    $"ontruimingstijd van de tweede richting naar de eerste: pas na het aflopen van die tijd kan de voorstartende " +
                    $"richting weer groen worden."));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Voor voorstarten gelden de volgende instellingen:"));
                items.AddRange(TableGenerator.GetTable_Intersignaalgroep_Voorstarten(c));
            }

            if (c.InterSignaalGroep.LateReleases.Any())
            {
                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_LateRelease"]}", 3));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Bij toepassen van een late release krijgt de ene richting altijd maximaal een instelbare tijd later groen dan het start groen " +
                    $"van de andere richting. Die tweede richting wordt hiertoe tijdelijk tegengehouden. Er geldt een fictieve " +
                    $"ontruimingstijd van de tweede richting naar de eerste; pas na het aflopen van die tijd kan de late release " +
                    $"richting weer groen worden."));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Voor late release gelden de volgende instellingen:"));
                items.AddRange(TableGenerator.GetTable_Intersignaalgroep_LateRelease(c));
            }

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_OVHDIntro(ControllerModel c)                                            // hfdst 5.0 Prioriteitsingrepen  
        {
            var items = new List<OpenXmlCompositeElement>();
            items.Add(OpenXmlHelper.GetTextParagraph(
                $"De regeling heeft zowel ingrepen voor de afhandeling van openbaar vervoer, als voor de afhandeling van hulpdiensten. " +
                $"De navolgende paragrafen geven een overzicht van de werking en instellingen voor deze ingrepen."));

            if (c.RISData.RISToepassen) 
            { 
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"In dit hoofdstuk worden voornamelijk de instellingen rond de traditionele afhandeling van prioriteitsaanvragen behandeld (KAR, Vecom, etc.). " +
                    $"Voor de instellingen rond prioriteits- aanvragen via het RIS wordt verwezen naar het hoofdstuk {Texts["Title_TalkingTraffic"]} verderop in deze specificatie."));
            }
            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_OV(ControllerModel c, WordprocessingDocument doc, int startLevel)       // hfdst 5.1 OV en logistiek  
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetTextParagraph("Het openbaar vervoer kan in de regeling op volgende manieren in- en/of uitmelden:"));

            var sl = new List<Tuple<string, int>>();
            if (c.HasKAR())
            {
                sl.Add(new Tuple<string, int>("KAR berichten", 0));
            }
            if (c.HasVecom())
            {
                sl.Add(new Tuple<string, int>("VECOM detectie", 0));
            }
            if (c.HasVecomIO())
            {
                sl.Add(new Tuple<string, int>("Externe VECOM detectie via ingangen", 0));
            }
            if (c.RISData.RISToepassen &&
                c.PrioData.PrioIngrepen.Exists(x => x.Type == PrioIngreepVoertuigTypeEnum.Bus &&
                                              x.MeldingenData.Inmeldingen.Any(y => y.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)))
            {
                sl.Add(new Tuple<string, int>("SRM berichten (zie hiervoor het hoofdstuk TT Prioriteren / UC3)", 0));
            }
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            if (c.PrioData.PrioIngrepen.Any())
            {
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVIngreepInUitmelding"]}", startLevel + 1));

                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Prioriteits aanvragen ('inmeldingen') voor verschillende voertuigtypen kunnen op verschillende (en soms ook meerdere) wijzen worden gedaan. " +
                    $"Hieronder een overzicht van de in deze regeling geconfigureerde in- en uitmeldingen per voertuigtype."));

                items.AddRange(TableGenerator.GetTable_Prio_InUit(c));

                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            }

            if (c.HasKAR() && c.PrioData.PrioIngrepen.Any() && (c.PrioData.KARSignaalGroepNummersInParameters || c.PrioData.VerlaagHogeSignaalGroepNummers))
            {
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVRichtingBijzonderheden"]}", startLevel + 1));

                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"In de regeling wordt een afwijkende, dan wel instelbare, afhandeling van richtingnummers uit de KAR berichten toegepast. " +
                    $"Dit wordt hieronder nader toegelicht."));

                if (c.PrioData.KARSignaalGroepNummersInParameters)
                {
                    items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVRichtingInPRM"]}", startLevel + 2)); 
                    
                    items.Add(OpenXmlHelper.GetTextParagraph(
                    $"De richtingnummers in het KAR bericht waar de regeling op reageert, zijn per signaalgroep opgenomen in een parameter. " +
                    $"Dit maakt het mogelijk om, bijvoorbeeld in geval van een omleiding, prioriteit aan te vragen op een andere signaalgroep " +
                    $"(bijvoorbeeld een bus die normaal op fc03 inmeldt, kan als richting 03 is afgesloten, inmelden op fc02 zonder dat er een aanpassing in de boordcomputer nodig is). " +
                    $"Per signaalgroep ## is er een parameter PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmkarsg") + "## waarin " +
                    "het KAR richtingnummer is opgegeven, zonder voorloopnul, waarop voor de betreffende signaalgroep wordt ingemeld."));
                }

                if (c.PrioData.VerlaagHogeSignaalGroepNummers)
                {
                    items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVRichtingHogeNummers"]}", startLevel + 2));

                    items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Volgens het KAR protocol kan de waarde van het richtingnummer maximaal 255 zijn, waarbij 201, 202 en 203 een speciale " +
                    $"(afwijkende) betekenis hebben. Hierdoor kunnen signaalgroepen hoger dan 200 niet goed met KAR worden bediend. Om dit te verhelpen " +
                    $"wordt van deze richtingen de waarde 200 afgetrokken. Een bus op richting 202 dient dan een KAR bericht te sturen met daarin een " +
                    $"inmelding op richting 2, een bus op richting 207 een KAR bericht met daarin richting 7, enzovoorts."));
                }

                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            }

            if (c.HasDSI() && c.PrioData.PrioIngrepen.Any(x => x.CheckLijnNummer))
            {
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVIngreepLijnnummers"]}", startLevel + 1));
            
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"OV (in)meldingen via KAR of Vecom worden voor de volgende signaalgroepen gefilterd op lijnnummer, waarbij " +
                    $"(in)meldingen van voertuigen met andere lijnnummers dan opgegeven, worden genegeerd. "));

                items.AddRange(TableGenerator.GetTable_OV_Lijnnummers(c));

                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            }

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVIngreep"]}", startLevel + 1));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Op basis van de parameters wordt bepaald welke mate van prioriteit een ingemeld voertuig krijgt met daarbij " +
                $"behorende prioriteitsopties voor de ingreep. Deze paragraaf geeft een funcioneel overzicht van de werking van de " +
                $"OV ingreep. In de volgende paragraaf wordt dit nader uitgewerkt, en zijn de instellingen voor de OV ingrepen " +
                $"in deze regeling opgenomen."));

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVIngreepAfkappen"]}", startLevel + 2));
            items.Add(OpenXmlHelper.GetTextParagraph("Wanneer een richting mag afkappen wordt wel rekening gehouden met de " +
                "instellingen van de parameters:"));
            sl = new List<Tuple<string, int>>
            {
                 new Tuple<string, int>("Afkapgarantie bij conflicterend OV", 0),
                 new Tuple<string, int>("Percentage maximum groentijd bij conflicterend OV", 0),
                 new Tuple<string, int>("Aantal malen niet afbreken bij conflicterend OV", 0),
            };
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            items.Add(OpenXmlHelper.GetTextParagraph("Verder hoeft de conflictrichting, die als laatste groen heeft voor de " +
                "OV-richting, pas afgekapt te worden op het moment dat de resterende rijtijd kleiner of gelijk is aan de geeltijd + ontruimingstijd. Wanneer " +
                "de OV-richting door omstandigheden niet op het gewenste moment groen kan worden, bijvoorbeeld doordat een " +
                "conflictrichting niet afgekapt mag worden, hoeven (overige) conflictrichtingen niet onnodig vroeg te worden " +
                "afgekapt. Er moet dan bepaald worden op welk moment de OV-richting op zijn vroegst groen kan worden. Aan " +
                "de hand van dat moment moet (opnieuw) bepaald worden wanneer conflictrichtingen afgekapt moeten worden."));

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVIngreepBijzonderRealiseren"]}", startLevel + 2));
            items.Add(OpenXmlHelper.GetTextParagraph("Als een richting bijzonder mag realiseren betekent dat normaal gesproken dat de realisatie " +
                "buiten de modulestructuur om plaats vindt. De OV-richting wordt op deze manier eigenlijk tussen de modulen " +
                "door gerealiseerd. In de priomodule worden OV richtingen doorgaans wel tussendoor gerealiseerd, echter niet als" +
                "bijzondere realisatie maar als extra primaire of alternatieve realisatie."));
            items.Add(OpenXmlHelper.GetTextParagraph("Tijdens een realisatie ten behoeve van een OV-ingreep wordt het groen " +
                "vastgehouden zolang het voertuig zich niet heeft uitgemeld en de groenbewaking niet is verstreken. Tijdens " +
                "een bijzondere realisatie mogen richtingen die niet conflicterend zijn met de OV-richting alternatief " +
                "realiseren. Hiervoor gelden dezelfde voorwaarde die gelden voor het regulier alternatief realiseren."));

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVIngreepNaOVIngreep"]}", startLevel + 2));
            items.Add(OpenXmlHelper.GetTextParagraph("Na afloop van de OV-ingreep gaat de regeling verder op het punt waar " +
                "de regeling was voor de OV-ingreep. Afhankelijk van de instelling van de parameter \"Percentage groentijd " +
                "t.b.v. terugkeren\" en de gerealiseerde groentijden van de afgekapte richtingen, worden deze richtingen al " +
                "dan niet opnieuw groen. Deze richtingen mogen hun resterende deel van maximum groentijd maken, of in ieder " +
                "geval uitverlengen tot en met de \"Ondergrens na terugkomen\". Is er geen sprake van richtingen die " +
                "terugkeren, dan worden de richtingen groen die volgens de regelstructuur als eerstvolgende aan de " +
                "beurt zijn."));

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVIngreepDefecteUitmelding"]}", startLevel + 2));
            items.Add(OpenXmlHelper.GetTextParagraph("Wanneer er voor het aflopen van de groenbewaking geen goede uitmelding " +
                "is ontvangen, kan (schakelbaar, default uit) de prioriteitsingreep worden uitgeschakeld. Er wordt dan vanuit gegaan " +
                "dat de uitmelding niet goed binnenkomt. De prioriteitsingreep wordt pas weer geactiveerd als er na een " +
                "correcte inmelding ook een correcte uitmelding is ontvangen."));
            items.Add(OpenXmlHelper.GetTextParagraph("De groenbewakingstijd loop altijd tijdens groen als een OV-voertuig " +
                "is ingemeld, alleen wordt het groen van een OV-richting zonder prioriteit niet vastgehouden. Dit zal tot " +
                "gevolg hebben dat na het constateren van een defecte uitmelding de aanvraag op de betreffende richting " +
                "blijft staan totdat de richting langer dan de groenbewakingstijd groen is geweest."));
            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVParameters"]}", startLevel + 1));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Met behulp van een parameter zijn voor elke signaalgroep met OV de verschillende prioriteitsopties in te stellen. " +
                $"Het honderdtal, het tiental en de eenheid van de parameter kunnen hiervoor worden gebruikt. Er kunnen dus maximaal " +
                $"3 prioriteitsopties worden gekozen. Met de waarde 123 krijgt de OV-richting de opties aanvragen, afkappen, groen " +
                $"vasthouden en bijzonder realiseren(\"volledige prioriteit\"). Mag de OV-richting ook conflicterende OV-richtingen " +
                $"afkappen dan wordt de waarde 234 toegekend; optie 4 maakt optie 1 overbodig. Met behulp van de waarde 500 wordt " +
                $"een nooddienst toegekend; optie 5 maakt opties 1, 2, 3 en 4 overbodig."));
            if (c.PrioData.PrioIngrepen.Any(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Binnen deze regeling is voorzien in de mogelijk de toe te kennen prioriteit afhankelijk te maken van de stiptheid" +
                    $"van het voertuig. Hiertoe wordt bij de inmelding (indien deze verloopt middels KAR of VECOM) gekeken naar de " +
                    $"afwijking ten opzichte van de dienstregeling. Hierbij gelden de volgende instellingen:"));
                sl = new List<Tuple<string, int>>();
                sl.Add(new Tuple<string, int>($"Meer dan {c.PrioData.GeconditioneerdePrioGrensTeVroeg} voor op dienstregeling: te vroeg", 0));
                sl.Add(new Tuple<string, int>($"Meer dan {c.PrioData.GeconditioneerdePrioGrensTeLaat} achter op dienstregeling: te laat", 0));
                sl.Add(new Tuple<string, int>($"Tussen deze twee waarden: op tijd", 0));
                items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            }
            items.Add(OpenXmlHelper.GetTextParagraph(
                $"De beschikbare opties worden toegelicht in onderstaande tabel:"));
            items.AddRange(TableGenerator.GetTable_OV_PrioriteitsOpties(c));

            items.Add(OpenXmlHelper.GetTextParagraph("Met dezelfde parameter zijn 10 prioriteitsniveaus instelbaar. Hiervoor is het " +
                "duizendtal van de parameter gereserveerd. Het prioriteitsniveau kan dus variëren van 0 t / m 9. Een richting met een " +
                "hoger prioriteitsniveau dan een conflictrichting, mag de prioriteitsafhandeling van die conflictrichting intrekken " +
                "en afhankelijk van de prioriteitsopties afkappen. Voor prioriteitsingrepen met hetzelfde prioriteitsniveau geldt " +
                "het “first in first out” principe."));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Wanneer een voertuig zich heeft ingemeld en de prioriteit niet is ingesteld op 0, wordt op basis van parameter " +
                $"instellingen bepaald of, gegeven de actuele status van de regeling, prioriteit mag worden verleend. Hiervoor zijn " +
                $"de volgende instellingen beschikbaar:"));

            sl = new List<Tuple<string, int>>
            {
                new Tuple<string, int>("Rijtijd: Per signaalgroep wordt voor maximaal 10 voertuigen, vanaf het moment dat een " +
                    "voertuig zich inmeldt, de rijtijd bijgehouden. Deze rijtijd wordt afgezet tegen de tijd die het voertuig nodig heeft " +
                    "om vanaf het inmeldpunt tot aan het punt voor de stopstreep te komen waarop het licht groen moet zijn. Er worden " +
                    "drie parameters gebruikt:", 0),
                new Tuple<string, int>("Ongehinderde rijtijd: alle lussen zijn onbezet.", 1),
                new Tuple<string, int>("Beperkt gehinderde rijtijd: koplus of lange lus is bezet.", 1),
                new Tuple<string, int>("Gehinderde rijtijd: koplus en lange lus tegelijkertijd gedurende een instelbare tijd bezet.", 1),
                new Tuple<string, int>("Blokkeringstijd: Ten behoeve van het niet honoreren van een prioriteitsaanvraag, zolang de " +
                    "blokkeringtijd loopt. Deze tijd wordt per OV-richting bijgehouden en wordt gestart op startrood en afgekapt als " +
                    "er geen (fictief) conflicterende aanvragen zijn. Deze instelling speelt geen rol bij prioriteits-inmeldingen met " +
                    "prioriteitsoptie 'nooddienst'.", 0),
                new Tuple<string, int>("Groenbewaking: Geeft aan hoelang een OV-richting maximaal groen krijgt ten behoeve van een " +
                    "OV-voertuig. Dit wordt per OV-voertuig bijgehouden. Er wordt daarbij vanuit gegaan dat er per OV-traject niet meer " +
                    "dan 10 OV-voertuigen tegelijkertijd ingemeld kunnen zijn. Na afloop van de groenbewaking, zonder een uitmelding, " +
                    "wordt het OV-voertuig uitgemeld.", 0),
                new Tuple<string, int>("Ondermaximum: Hiermee wordt aangegeven tot welk moment voor het bereiken van de maximum " +
                    "groentijd een OV-voertuig nog prioriteit mag krijgen. Deze instelling speelt geen rol bij prioriteits-inmeldingen met " +
                    "prioriteitsoptie 'nooddienst'.", 0)
            };
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));

            items.Add(OpenXmlHelper.GetTextParagraph(""));
            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Onderstaande tabel geeft een overzicht van de prioriteitsinstellingen voor richtingen met prioriteitsingrepen in deze regeling:"));

            items.AddRange(TableGenerator.GetTable_OV_PrioriteitsInstellingen(c));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Naast bovenstaande parameters is een aantal parameters beschikbaar om de mate van verstoring door een OV-ingreep " +
                $"te beperken:"));

            sl = new List<Tuple<string, int>>
            {

                new Tuple<string, int>("Wachttijdcriterium: Indien één of meer fasecycli een hogere wachttijd hebben dan de waarde " +
                    "van het wachttijdcriterium, mag er niet bijzonder gerealiseerd worden. Conflicten mogen wel afgebroken worden om " +
                    "zodoende een versnelde cyclus te kunnen maken. De waarde van het wachttijdcriterium is voor autorichtingen (default " +
                    "120 seconden), fietsrichtingen (default 90 seconden) en voetgangersrichtingen (default 90 seconden) apart in te stellen. Deze instelling speelt geen rol bij prioriteitsingrepen met prioriteitsoptie 'nooddienst'.", 0),
                new Tuple<string, int>("Afkapgarantie bij conflicterend OV: Per richting is middels een parameter aan te geven hoelang " +
                    "die richting groen mag zijn, voordat deze kan worden afgekapt door een conflicterende OV-ingreep. Deze instelling speelt geen rol bij prioriteitsingrepen met prioriteitsoptie 'nooddienst'.", 0),
                new Tuple<string, int>("Percentage maximum groentijd bij conflicterend OV: Per richting is middels een percentage aan " +
                    "te geven hoelang die richting groen moet (kunnen) zijn, voordat deze kan worden afgekapt door een conflicterende prioriteitsingreep. " +
                    "Deze instelling speelt geen rol bij prioriteitsingrepen met prioriteitsoptie 'nooddienst'.", 0),
                new Tuple<string, int>("Ophoogpercentage maximum groentijd bij conflicterend OV: Per richting is een percentage aan te " +
                    "geven waarmee het percentage maximum groentijd bij een conflicterende prioriteitsingreep wordt opgehoogd nadat die richting is afgebroken. " +
                    "Staat het percentage maximum groentijd bij een conflicterende prioriteitsingreep bijvoorbeeld op 50% en het ophoogpercentage op 25%, dan mag " +
                    "een afgekapte richting de volgende cyclus 75% van zijn groen maken, voordat hij wordt afgekapt. Wordt de richting dan " +
                    "weer afgekapt dan mag de cyclus daarna 100% worden gemaakt. Dit mag er overigens nooit toe leiden dat het percentage " +
                    "maximum groen bij een conflicterende prioriteitsingreep groter wordt dan 100%. Wanneer een richting op hiaat wordt beëindigd of een keer " +
                    "100% van zijn maximum groen maakt, wordt het percentage maximum groentijd bij een conflicterende prioriteitsingreep gereset naar de " +
                    "ingestelde waarde.", 0),
                new Tuple<string, int>("Aantal malen niet afbreken bij een conflicterende prioriteitsingreep: Geeft aan hoe vaak een conflicterende richting " +
                    "niet mag worden afgebroken, nadat deze richting afgekapt is ten behoeve van een ingreep.", 0),
                new Tuple<string, int>("Percentage groentijd t.b.v. terugkeren: Per richting is een percentage van de groentijd aan te " +
                    "geven waarvoor geldt dat als de primaire realisatie van een richting wordt afgekapt door een conflicterende prioriteitsingreep " +
                    "en dit percentage van de groentijd nog niet is bereikt, dat die richting dan na afloop van de conflicterende prioriteitsingreep " +
                    "opnieuw wordt gerealiseerd.", 0),
                new Tuple<string, int>("Ondergrens na terugkomen: Als na afloop van een prioriteitsingreep een afgekapte conflicterende " +
                    "richting opnieuw mag realiseren dan mag deze richting het restant van zijn maximum groentijd maken. Als dit restant " +
                    "lager is dan de ondergrens na terugkomen dan dient de ondergrens gehanteerd te worden.", 0)
            };
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));

            items.Add(OpenXmlHelper.GetTextParagraph(""));
            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Onderstaande tabel geeft een overzicht van de instellingen voor met een prioriteitsingreep conflicteren richtingen:"));

            items.AddRange(TableGenerator.GetTable_OV_ConflictenInstellingen(c));

            items.Add(OpenXmlHelper.GetTextParagraph(""));

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_HD(ControllerModel c, WordprocessingDocument doc, int startLevel)       // hfdst 5.2 Hulpdiensten  
        {
            var items = new List<OpenXmlCompositeElement>();
            var text = "De regeling is uitgerust met hulpdienstingrepen op basis van ";
            if (c.HasHDKAR() && c.HasHDOpticom()) text += "KAR en Opticom.";
            else if (c.HasHDKAR()) text += "KAR.";
            else if (c.HasHDOpticom()) text += "Opticom.";

            if (!c.HasPT())
            {
                text +=
                    $" Een hulpdienstingreep kent de hoogste vorm van prioriteit (zie onderstaande tabel). Deze ingreep kapt af (na " +
                    $"verstrijken van de garantiegroentijd) en blokkeert alle signaalgroepen die conflicteren met de hulpdienst.";
            }
            else
            {
                text +=
                    $" Een hulpdienstingreep kent de hoogste vorm van prioriteit: (zie tabel {TableGenerator.Tables["Table_OV_PrioriteitsOpties"]}: " +
                    $"{Texts["Table_OV_PrioriteitsOpties"]}). Deze ingreep kapt af (na verstrijken van de garantiegroentijd) en blokkeert " +
                    $"alle signaalgroepen die conflicteren met de hulpdienst.";
            }

            if (c.PrioData.BlokkeerNietConflictenBijHDIngreep)
            {
                if (c.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer)
                    text += $" Van niet-conflicten worden alleen richtingen met langzaam verkeer afgekapt en geblokkeerd.";
                else
                    text += $" Ook niet-conflicten worden afgekapt, voor zover ze niet ook een actieve hulpdienst ingreep hebben.";
            }
            text += " Tijdens een hulpdienstingreep wordt de fasebewakingstimer herstart.";
            items.Add(OpenXmlHelper.GetTextParagraph(text));

            if (!c.HasPT())
            {
                items.AddRange(TableGenerator.GetTable_OV_PrioriteitsOpties(c));
                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            }

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"De instellingen en opties voor hulpdienstingrepen worden weergegeven in onderstaande tabel:"));
            items.AddRange(TableGenerator.GetTable_HD_Instellingen(c));

            items.Add(OpenXmlHelper.GetTextParagraph(""));

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_HalfstarIntro(ControllerModel c, WordprocessingDocument doc)           // hfdst 6  Halfstar  
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!c.HalfstarData.IsHalfstar) return items;

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Halfstar_Intro"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                 "In de regeling is voorzien in een halfstarre regelstructuur via één of meer signaalplannen (PL). Bij een halfstarre structuur " +
                 "kan een regeling de rol hebben van Master, Fallback-master of Slave. Een Fallback master gedraagt zich in normale omstandigheden " +
                 "als Slave maar neemt de rol van Master aan, wanneer en voor zolang de communicatie met de regeling met de Master rol, wegvalt. " +
                 "Er kan binnen het netwerk maar één regeling de (actieve) Master zijn."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                 "Vanuit de Master bezien worden de andere regelingen geconfigureerd als Slave. Die andere regelingen kunnen zelf wel als Fallback-master " +
                 "geconfigureerd zijn, waarbij zij zich richting de 'echte' master gedragen als Slave en richting hun Slave(s) als Master. "));
            items.Add(OpenXmlHelper.GetTextParagraph(
                 "Doorgaans verdient het aanbeveling om de Master centraal in het netwerk te plaatsen, de Slaves aan het eind van de streng(en) " +
                 "en de tussenliggende regelingen als Fallback-master te configureren. Op deze wijze blijft bij uitval van een regeling of " +
                 "communicatieverbinding een zo groot mogelijk deel van de regelingen gecoördineerd doordraaien."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                 "De regelingen geven onderling de benodigde synchronisatie- en plankeuze-pulsen door, wanneer de regelingen die zich tussen de " +
                 "Master en het eind van de streng(en) bevinden als Fallback-master geconfigureerd zijn. De Master hoeft daarom niet met alle " +
                 "regelingen individueel een verbinding te hebben. "));

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Halfstar_Config"]}", 2));
            items.Add(OpenXmlHelper.GetTextParagraph(
                 "De configuratie van de regeling is als volgt: "));
            
            items.AddRange(TableGenerator.GetTable_Halfstar_configuratie(c));
            
            if (c.HalfstarData.Type.ToString() != "Slave")
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    "Voor een regeling van type 'Master' (of 'Fallback master' die de rol van Master heeft aangenomen), kan voor de hele streng " +
                    "VA regelen worden ingesteld via de schakelaar '" + CCOLGeneratorSettingsProvider.Default.GetElementName("schvarstreng") + "'.", "Footer"
                    ));
            }

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.AddRange(TableGenerator.GetTable_Halfstar_hoofdrichtingen(c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.AddRange(TableGenerator.GetTable_Halfstar_koppelingen(c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Halfstar_PL"]}", 2));

            var sl = new List<Tuple<string, int>>();

            var def = c.HalfstarData;

            items.Add(OpenXmlHelper.GetTextParagraph(

                "Voor de default periode (dalperiode) geldt:"));

                sl = new List<Tuple<string, int>>();
            {
                    int dpsl = def.DefaultPeriodeSignaalplan.Length;

                    if (dpsl > 2)
                        sl.Add(new Tuple<string, int>($"Actief signaalplan                                                : " + def.DefaultPeriodeSignaalplan, 0));
                    else
                        sl.Add(new Tuple<string, int>($"Actief signaalplan                                                : Geen", 0));
                
                sl.Add(new Tuple<string, int>($"VA regelen actief (SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schpervar") + "def)                    : " + def.DefaultPeriodeVARegelen.ToCustomString2(), 0));
                sl.Add(new Tuple<string, int>($"Hoofdrichtingen alternatief (SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schperarh") + "def) : " + def.DefaultPeriodeAlternatievenVoorHoofdrichtingen.ToCustomString2(), 0));
            }
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.Add(OpenXmlHelper.GetTextParagraph(
                 "De koppeling van halfstarre perioden " + 
                 //"(conform tabel " + KlokperiodenTable.ToString() + ") " +
                 "aan signaalplannen is als volgt: "));

            items.AddRange(TableGenerator.GetTable_Halfstar_perioden(c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Halfstar_SG"]}", 2));

            items.AddRange(TableGenerator.GetTable_Halfstar_SG(c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            /* pagebreak toegevoegd */
            items.Add(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Halfstar_Signaalplannen"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                "De regling kent " + c.HalfstarData.SignaalPlannen.Count() + " signaalplannen; deze worden hieronder weergegeven."
                ));

            var l = new List<List<string>> { };

            foreach (var pl in c.HalfstarData.SignaalPlannen)
            {
                items.Add(OpenXmlHelper.GetTextParagraph($"Plan naam: " + pl.Naam, "Footer"));
                items.Add(OpenXmlHelper.GetTextParagraph($"Cyclustijd: " + pl.Cyclustijd.ToString() + " sec", "Footer"));
                items.Add(OpenXmlHelper.GetTextParagraph($"Start moment: " + pl.StartMoment.ToString(), "Footer"));
                items.Add(OpenXmlHelper.GetTextParagraph($"Omschakel moment: " + pl.SwitchMoment.ToString(), "Footer"));
                items.Add(OpenXmlHelper.GetTextParagraph($"Beschrijving: " + pl.Commentaar, "Footer"));
                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                items.Add(OpenXmlHelper.GetTextParagraph($"TX momenten " + pl.Naam + ":", "Footer"));

                l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Fase (##)",
                        "A1",
                        "B1",
                        "C1",
                        "D1",
                        "E1",
                        "A2",
                        "B2",
                        "C2",
                        "D2",
                        "E2"
                    }
                };

                foreach (var plfc in pl.Fasen)
                {
                    l.Add(new List<string>
                    {
                        plfc.FaseCyclus,
                        plfc.A1.ToString(),
                        plfc.B1.ToString(),
                        plfc.C1.ToString(),
                        plfc.D1.ToString(),
                        plfc.E1.ToString(),
                        plfc.A2.ToString(),
                        plfc.B2.ToString(),
                        plfc.C2.ToString(),
                        plfc.D2.ToString(),
                        plfc.E2.ToString(),
                    });
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: false));

                /* pagebreak toegevoegd */
                items.Add(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
            }

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Halfstar_Prio"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                 "Wanneer tijdens halfstar bedrijf een inmelding van nood- en hulpdiensten wordt ontvangen, schakelt de regeling " +
                 "tijdelijk over naar voertuigafhankelijk bedrijf. Voor andere prio meldingen gelden de volgende instellingen:"));

            items.Add(OpenXmlHelper.GetTextParagraph($"Prio aanvragen tijdens halfstar mogelijk (SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schovpriople") + "): " + c.HalfstarData.OVPrioriteitPL.ToCustomString2(), "Footer"));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.AddRange(TableGenerator.GetTable_Halfstar_Prio(c));

            items.Add(OpenXmlHelper.GetTextParagraph(""));

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_PTP(ControllerModel c)                                                  // hfdst 7.1 PTP    
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!c.PTPData.PTPKoppelingen.Any()) return items; 

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_PTP"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                "De regeling is voorzien van één of meer PTP koppelingen(en) met buurkruispunt(en). In onderstaande tabel worden " +
                "de gegevens van de PTP verbinding(en) weergegeven:"));

            if (c.PTPData.PTPInstellingenInParameters)
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    "De PTP verbingingsgegevens zijn instelbaar via parameters."));
            }

            items.AddRange(TableGenerator.GetTable_PTP(c));

            items.Add(OpenXmlHelper.GetTextParagraph(""));
            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_File(ControllerModel c, WordprocessingDocument doc)                     // hfdst 7.2 Filemeldingen  ToDo: tabel  
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!c.FileIngrepen.Any()) return items;  

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_File"]}", 2));

            var sl = new List<Tuple<string, int>>();
            items.Add(OpenXmlHelper.GetTextParagraph(
                 "Op de aan- of afvoerende strook van een richting kan gebruik worden gemaakt van filedetectie. Dit is doorgaans een (al dan niet virtuele) lus " +
                 "met een lengte van 5 à 7 meter. Bij het bereiken van een bezettijd op deze lus wordt de betreffende file-ingreep waar. Wanneer " +
                 "een ingestelde hiaattijd is verstreken, of wanneer de afvalvertraging optreedt, vervalt de ingreep (de afvalvertraging grijpt in wanneer " +
                 "de filedetectie voor langere tijd bezet is zonder dat er wordt gemeten dat er voertuigen rijden)."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                 "Een (traditionele) fileingreep bestaat uit: "));

                 sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>($"het op één of meerdere richtingen doseren van de groentijd op basis van een percentage van de actuele verlenggroentijd;", 0));
                sl.Add(new Tuple<string, int>($"het beëindigen gvan het roen na een minimale groentijd bij start filemeting;", 0));
                sl.Add(new Tuple<string, int>($"een minimale roodtijd;", 0));
                sl.Add(new Tuple<string, int>($"de keuze van een andere set maximum groentijden;", 0));
            }
                 items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            items.Add(OpenXmlHelper.GetTextParagraph(
                 "waarbij alle ingrepen optioneel zijn."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                 "Naast de traditionele fileingreep is het ook mogelijk om een fileingreep van het type ‘eerlijk te doseren’ toe te " +
                 "passen. Als eerlijk doseren wordt toegepast, wordt tijdens filemeting op alle richtingen naar het filemeetpunt toe de " +
                 "groentijd gedoseerd. Pas wanneer de file is afgevallen en alle richtingen hun gedoseerde groentijd hebben gekregen, " +
                 "word de fileingreep uitgeschakeld. Door het gebruik van eerlijk doseren wordt voorkomen dat bij filemeetpunten die " +
                 "vaak opkomen, steeds dezelfde richting wordt gedoseerd. "));
            items.Add(OpenXmlHelper.GetTextParagraph(
                 "Wanneer eerlijk doseren wordt toegepast kan er slechts 1 doseerpercentage worden gebruikt dat op de actuele " +
                 "verlenggroentijd van alle toevoerende richting van toepassing is."));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Onderstaande tabel geeft een overzicht van de instellingen voor de file ingre(e)p(en):"));

            items.AddRange(TableGenerator.GetTable_FileIngreep(c));

            items.Add(OpenXmlHelper.GetTextParagraph(""));

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_VAontr(ControllerModel c, WordprocessingDocument doc)                   // hfdst 7.3 VA ontruimen  ToDo: tabel  
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!c.VAOntruimenFasen.Any()) return items;

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_VAontr"]}", 2));

            var sl = new List<Tuple<string, int>>();
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Op basis van een massadetector kan er voertuigafhankelijk worden ontruimd. " +
                "Bij voertuigafhankelijk ontruimen worden de conflicten van een richting tegengehouden gedurende:"));
            sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>($"een maximale tijd nadat de richting heeft gerealiseerd (groen/geel);", 0));
                sl.Add(new Tuple<string, int>($"een tijdsduur per conflict welke wordt geactiveerd als de ontruimingsdetector bezet is; "+
                                              "deze tijd wordt alleen geactiveerd als de maximale VA ontruimingstijd nog actief is.", 0));
            }
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));

            items.Add(OpenXmlHelper.GetTextParagraph(
                "In onderstaande tabel is weergegeven voor welke richting(en) op basis van welke detector(en) VA ontruimd wordt."));

            items.AddRange(TableGenerator.GetTable_VAontr(c));

            items.Add(OpenXmlHelper.GetTextParagraph(""));
            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_Signalen(ControllerModel c)                                             // hfdst 7.4 ToDo TWL Signalen  
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!c.Signalen.WaarschuwingsGroepen.Any() && !c.Signalen.Rateltikkers.Any()) return items;

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Signalen"]}", 2));

            if (c.Signalen.Rateltikkers.Any() && !c.Signalen.WaarschuwingsGroepen.Any())
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    "In de regeling zijn rateltikkers opgenomen. In onderstaande tabel worden de relevante klokperioden weergegeven, daarna de instellingen voor de signalen."));
            }
            if (!c.Signalen.Rateltikkers.Any() && c.Signalen.WaarschuwingsGroepen.Any())
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    "In de regeling zijn waarschuwingsgroepen voor TWL's opgenomen. In onderstaande tabel worden de relevante klokperioden weergegeven, daarna de instellingen voor de signalen."));
            }
            if (c.Signalen.Rateltikkers.Any() && c.Signalen.WaarschuwingsGroepen.Any())
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    "In de regeling zijn rateltikkers en waarschuwingsgroepen voor TWL's opgenomen. In onderstaande tabel worden de relevante klokperioden weergegeven, daarna de instellingen voor de signalen."));
            }

            
            items.AddRange(TableGenerator.GetTable_Signalen_Perioden(c));
            
            items.Add(OpenXmlHelper.GetTextParagraph(""));

            if (c.Signalen.Rateltikkers.Any())
            {
                items.AddRange(TableGenerator.GetTable_Signalen_Rateltikkers(c));
               
                items.Add(OpenXmlHelper.GetTextParagraph(""));
            }
          //if (c.Signalen.WaarschuwingsGroepen.Any())  // vooralsnog lijken er geen specifieke instellingen voor TWL's te zijn anders dan de klokperioden
          //{
          //    items.AddRange(TableGenerator.GetTable_Signalen_WaarschuwingsGroepen(c));
          //    items.Add(OpenXmlHelper.GetTextParagraph(""));
          //}

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_RobuGroVer(ControllerModel c, WordprocessingDocument doc)               // hfdst 7.5 RoBuGrover  
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!c.RoBuGrover.ConflictGroepen.Any()) return items;

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_RobuGroVer"]}", 2));

            var sl = new List<Tuple<string, int>>();
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Voor de bepaling van de maximum(verleng)groentijden wordt gebruik gemaakt van de Robuuste GroentijdVerdeler (RobuGroVer, RGV). Voor een goede " +
                "verkeersafwikkeling op een geregeld kruispunt is het belangrijk dat de maximum(verleng)groentijden van de " +
                "verschillende fasecycli goed zijn ingesteld. Omdat het verkeersaanbod sterk kan fluctueren zijn de maximale groentijden niet altijd " +
                "accuraat. De RobuGroVer is een groentijdverdeler die reageert op het actuele verkeersaanbod en rekening houdt met " +
                "een maximum gewenste cyclustijd van meerdere conflictgroepen: "));
            sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>("De maximum (verleng)groentijd wordt gewijzigd afhankelijk van de benutting van het verlenggroen van de primaire richting;" +
                                              "dit wordt bepaald via het meetkriterium.", 0));
                sl.Add(new Tuple<string, int>("Er wordt (dus) geen verkeer geteld omdat het tellen via lussen met verkeerslichten vaak onbetrouwbaar is. " +
                                              "Bij de RobuGroVer wordt uitgegaan van lussen die de groentijd verlengen; deze zijn vrijwel altijd beschikbaar " +
                                              "en de groentijdverdeler is daarmee zeer robuust. ", 0));
                sl.Add(new Tuple<string, int>("Om de groentijd te bepalen worden er maximaal 9 conflictgroepen doorgerekend en wordt bepaald of een richting " +
                                              "meer of minder groen kan krijgen.", 0));
            }
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            items.Add(OpenXmlHelper.GetTextParagraph("Alternatieve realisaties en prioriteitsingrepen worden door de RobuGroVer buiten beschouwing gelaten, " +
                "alsmede richtingen met detectiestoringen; bij een detectie storing op een kop- of lange lus wordt voor de betreffende richting de RobuGroVer " +
                "uitgeschakeld en wordt de basis maximum verlenggroentijd volgens de klok gehanteerd."));
            items.Add(OpenXmlHelper.GetTextParagraph("De RobuGroVer kijkt voor een fasecyclus waarvoor een maximum verlenggroentijd is gedefinieerd op einde verlenggroen " +
                "van een primaire realisatie naar het verschil tussen de ingestelde maximum verlenggroentijd en de actuele (gerealiseerde) verlenggroentijd."));
            items.Add(OpenXmlHelper.GetTextParagraph("Daarbij kunnen zich de volgende situaties zich voor doen:"));
            items.Add(OpenXmlHelper.GetTextParagraph("1) Er is geen verlenggroentijd over (er is maximaal uitverlengd). In dat geval wordt de maximum verlenggroentijd " +
                "verhoogd met PRM tvg_omhoog. Indien de berekende maximum verlenggroentijd daarna groter is dan PRM maxtvg_##, wordt de maximum verlenggroentijd gelijk aan PRM maxtvg_##. "));
            items.Add(OpenXmlHelper.GetTextParagraph("2) Er is veel verlenggroentijd over, het verschil is groter dan PRM tvg_verschil en de fasecyclus is niet afgebroken " +
                "(FM[]) of afgekapt (Z[]). In dat geval wordt de maximum verlenggroentijd verlaagd met PRM tvg_omlaag. Indien de berekende maximum verlenggroentijd kleiner wordt dan " +
                "PRM mintvg_##, wordt de maximum verlenggroentijd gelijk aan PRM mintvg_##."));
            items.Add(OpenXmlHelper.GetTextParagraph("3) Er is weinig verlenggroentijd over, het verschil is kleiner of gelijk aan PRM tvg_verschil. In dat  geval wordt " + 
                "de maximum verlenggroentijd niet gewijzigd."));
            items.Add(OpenXmlHelper.GetTextParagraph("4) Indien er een detectiestoring aanwezig is op een signaalgroep die betreffende signaalgroep in de procedure genegeerd."));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.Add(OpenXmlHelper.GetTextParagraph("Op kruispuntniveau gelden de volgende instellingen:"));
            sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>("SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schrgv") + ": schakelaar RobuGroVer aan/uit", 0));
                sl.Add(new Tuple<string, int>("SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schrgv_snel") + ": bepaalt het moment dat maxgroen mag worden verhoogd of verlaagd", 0));
                sl.Add(new Tuple<string, int>("0: groentijd mag pas worden aangepast bij de volgende groenfase", 1));
                sl.Add(new Tuple<string, int>("1: groentijd mag al worden aangepast tijdens een lopende groenfase; " +
                                                  "dit kan nadelig kan uitpakken voor de betrouwbaarheid van (wachttijd)voorspellers", 1));
                sl.Add(new Tuple<string, int>("PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrgv") + ": type groentijdberekening", 0));
                sl.Add(new Tuple<string, int>("0: de correctie vindt alleen plaats door de opgegeven minima en maxima", 1));
                sl.Add(new Tuple<string, int>("1: de correctie vindt plaats door de maximaal mogelijke cyclustijd uit alle opgegeven conflictgroepen (en mag minima en maxima niet verbreken) ", 1));
                sl.Add(new Tuple<string, int>("2: de correctie vindt plaats door de maximaal mogelijke cyclustijd voor alléén de eigen conflictgroep (en mag minima en maxima niet verbreken)", 1));
                sl.Add(new Tuple<string, int>("PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmin_tcyclus") + ": ondergrens van de cyclustijd die is toegestaan", 0));
                sl.Add(new Tuple<string, int>("PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmax_tcyclus") + ": bovengrens van de cyclustijd die is toegestaan", 0));
                sl.Add(new Tuple<string, int>("PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_omhoog") + ": waarde waarmee maximum verlenggroentijd wordt verhoogd", 0));
                sl.Add(new Tuple<string, int>("PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_omlaag") + ": waarde waarmee maximum verlenggroentijd wordt verlaagd", 0));
                sl.Add(new Tuple<string, int>("PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_verschil") + ": waarde van minimaal verschil tussen groenbehoefte en gerealiseerd groen", 0));
                sl.Add(new Tuple<string, int>("PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_npr_omlaag") + ": waarde waarmee maximum verlenggroentijd wordt verlaagd in geval van overslag", 0));
                sl.Add(new Tuple<string, int>("Per deelnemende fasecyclus zijn er parameters die de de groentijd begrenzen:", 0));
                sl.Add(new Tuple<string, int>("PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmintvg") + "_##: ondergrens aan verlenggroentijd voor richting ##", 1));
                sl.Add(new Tuple<string, int>("PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmaxtvg") + "_##: bovengrens aan verlenggroentijd voor richting ##", 1));
            }
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            
            items.AddRange(TableGenerator.GetTable_RobuGroVerKruising(c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.AddRange(TableGenerator.GetTable_RobuGroVerFasen(c));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.AddRange(TableGenerator.GetTable_RobuGroVerConflictgroepen(c));

            items.Add(OpenXmlHelper.GetTextParagraph(""));

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_PelotonKoppeling(ControllerModel c, WordprocessingDocument doc)         // hfdst 7.6 ToDo PelotonKoppeling  
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!c.PelotonKoppelingenData.PelotonKoppelingen.Any()) return items;

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_PelotonKoppeling"]}", 2));
            var sl = new List<Tuple<string, int>>();

            items.Add(OpenXmlHelper.GetTextParagraph(
                "Op basis van een inkomend koppelsignaal kan een pelotonkoppeling voor een richting worden toegepast. " +
                "Op basis van dit signaal wordt berekend op welke moment het peloton in de buurt van de stopstreep is. " +
                "Vervolgens worden bij een koppeling van 'Type 1', op de gekoppelde richting de volgende acties ondernomen: "));
            sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>("een aanvraag gezet;", 0));
                sl.Add(new Tuple<string, int>("de richting vastgehouden in verlengroen;", 0));
                sl.Add(new Tuple<string, int>("de richting teruggezet naar wachtgroen.", 0));
            }
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Op startgroen van de voedende richting wordt een uitgaand koppelsignaal verstuurd als in een periode " +
                "vanaf startgroen een (via parameter " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmpelgrens") + "### instelbaar) aantal vertrekkende voertuigen wordt gemeten.  "));

            items.Add(OpenXmlHelper.GetTextParagraph(
                "De optie 'Type 2' is een uitgebreidere code waarin rekening wordt gehouden met meerdere pelotons. Deze optie " +
                "wordt dan ook vooral toegepast bij kruisingen die op grotere afstand van elkaar liggen. Bij deze optie is " +
                "de (peloton)verschuiving de tijd die berekend wordt als (afstand tussen de kruisingen - lengte peloton) / snelheid in m/s. "));
            
            items.AddRange(TableGenerator.GetTable_Pelotonkoppelingen(c));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_TT_Algemeen(ControllerModel c, WordprocessingDocument doc)              // hfdst 8.1 ToDo Talking Traffic  
        {
            var items = new List<OpenXmlCompositeElement>();
 
            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_TT_Algemeen"]}", 2));
            var sl = new List<Tuple<string, int>>();

            items.Add(OpenXmlHelper.GetTextParagraph(
                "Voor een goede communicatie en data overdracht tussen de (C)ITS applicatie en het RIS is een aantal "+
                "instellingen noodzakelijk; deze bevinden zich deels in de ITSapp. Hierbij zijn het Approach ID en het " +
                "Lane ID opgeslagen in parameters zodat de instellingen in de ITSapp kunnen worden aangepast wanneer " +
                "het ITF bestand bij het maken van de regeling nog niet volledig beschikbaar is, of naderhand wordt aangepast."));

            items.Add(OpenXmlHelper.GetTextParagraph(
                "Om in geval van mis-communicatie binnen de Talking Traffic keten die een negatieve impact heeft op de verkeersafwikkeling, " +
                "snel functionaliteit te kunnen uitzetten, is voorzien in een aantal schakelaars:"));
            sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>("SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schrisaanvraag") + ": afzetten van RIS aanvragen (via CAM) voor alle richtingen", 0));
                sl.Add(new Tuple<string, int>("SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schrisverlengen") + ": afzetten van RIS verlengen (via CAM) voor alle richtingen", 0));
            }
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));            

            items.Add(OpenXmlHelper.GetTextParagraph("Voor de instellingen en functionele werking van prio ingrepen, " +
                $"zie het eerdere hoofdstuk \"{Texts["Title_OVHD"]}\"."));
            

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_TT_Instellingen(ControllerModel c)                                      // hfdst 8.2 ToDo Talking Traffic  
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_TT_Instellingen"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                "In deze paragraaf wordt een overzicht gegeven van de koppeling tussen signaalgroepen in de ITSapp en " +
                "de Aproach en Lane ID's in het ITF bestand. " +
                "Hierbij moet er rekening mee worden gehouden dat er voor de voetgangers geen vaste relatie is tussen " + 
                "het enerzijds eerste en tweede Lane ID en anderszijds een binnen- of buitendrukknop; dit moet handmatig " +
                "worden geverifiëerd bij het configureren van de ITSapp."));

            items.AddRange(TableGenerator.GetTable_TT_Instellingen(c));

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_TT_UC3_Prioriteren(ControllerModel c, WordprocessingDocument doc)       // hfdst 8.3 ToDo UC3
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_TT_UC3_Prioriteren"]}", 2));
            var sl = new List<Tuple<string, int>>();

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Ten begoeve van prioriteit via het RIS kunnen voertuigen zich in- en uitmelden via SRM berichten. " +
                "In TLCGen is via parameters een aantal mogelijkheden en opties opgenomen:"));

            sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>("Koppelen aan een signaalgroepnummer:", 0));
                sl.Add(new Tuple<string, int>("Inmelden op basis van een signaalgroepnummer in het SRM-bericht (openbaar vervoer, vrachtverkeer). " +
                    "Hiervoor hoeft niets ingesteld te worden. Wel dient ingesteld te worden dat er niet op apporachid wordt ingemeld zodat er " +
                    "geen dubbele inmelding ontstaat; hiertoe dient de betreffende parameter op '999' ingesteld te worden (dit is voor RIS " +
                    "inmeldingen - anders dan hulpdiensten - ook de standaard waarde in TLCGen). " +
                    "Per type aanvrager (bus, vrachtauto, etc) is er hiertoe een parameter, bijvoorbeeld", 1));
                sl.Add(new Tuple<string, int>(CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisapproachid") + "##busris voor bussen", 2));
                sl.Add(new Tuple<string, int>(CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisapproachid") + "##vrwris voor vrachtverkeer", 2));
            //  sl.Add(new Tuple<string, int>(CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisapproachid") + "##hd voor hulpdiensten", 2));
                sl.Add(new Tuple<string, int>("Inmelden op appraoch ID (voor nood- en hulpdiensten). Hiervoor dient het approachid ingesteld te worden " +
                    "(PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisapproachid") + "##hd is daarbij gelijk aan het approach ID " +
                   $"in het ITF bestand, zie hiervoor tabel {TableGenerator.Tables["Table_TT_Instellingen"]}).", 1));
                sl.Add(new Tuple<string, int>("Inmelden op routenaam / lijnnummer. Hoewel dit wel in de specificatie van UC3 is opgenomen, is in de loop der tijd " +
                    "besloten dat dit niet meer wordt gebruikt. TLCGen voorziet daarom niet in deze optie.", 1));
                sl.Add(new Tuple<string, int>("De locatie dat de daadwerkelijke inmelding start in de TLCGen (met gebruik van de rijtijden gehinderd, " +
                    "beperkt gehinderd en ongehinderd): ", 0));
                sl.Add(new Tuple<string, int>("Inmelden op basis van de ETA (Estimated Time of Arrival) uit het SRM-bericht. Als de parameter " + 
                    "PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmriseta") + "##busris of -vrwris groter is dan 0, wordt ingemeld " +
                    "op basis van de ETA. De waarde van deze ETA parameter bepaalt vanaf welk moment de inmelding plaatsvindt.", 1));
                sl.Add(new Tuple<string, int>("Inmelden op basis van het gebied waarin het voertuig zich bevindt op basis van de GPS-positie(s) uit het CAM-bericht.", 1));
            }
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            items.Add(OpenXmlHelper.GetTextParagraph("In onderstaande tabellen zijn de instellingen voor prioriteitsaanvragen opgenomen:"));

            items.AddRange(TableGenerator.Table_TT_UC3_Prioriteren(c));

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_TT_UC4_Informeren(ControllerModel c, WordprocessingDocument doc)        // hfdst 8.4 ToDo UC4
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_TT_UC4_Informeren"]}", 2));
            var sl = new List<Tuple<string, int>>();

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Van alle richtingen wordt door de ITSapp informatie aangeboden op de CVN-interface, waarmee de TLC een SPAT bericht kan maken. " +
                "De aangeboden informatie betreft:"));
            sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>("EventState: de status van een signaalgroep.                                                                                                                                                             " +
                    "Naast rood, geel of groen betreft dit ook de status gedoofd, " +
                    "alles rood en geelknipperen; ook wordt de aanwezigheid van een deelconflict aangeboden.", 0));
            }
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>("StartTime:                                                                                                                                                                                           " +
                    "p.m.", 0));
            }
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>("minEndTime:                                                                                                                                                                                           " +
                    "p.m.", 0));
            }
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>("maxEndTime:                                                                                                                                                                                           " +
                    "p.m.", 0));
            }
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>("likelyTime:                                                                                                                                                                                           " +
                    "p.m.", 0));
            }
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>("confidence:                                                                                                                                                                                           " +
                    "p.m.", 0));
            }
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
            sl = new List<Tuple<string, int>>();
            {
                sl.Add(new Tuple<string, int>("nextime:                                                                                                                                                                                           " +
                    "p.m.", 0));
            }
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_TT_UC5_Optimaliseren(ControllerModel c)                                 // hfdst 8.5 ToDo UC5
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_TT_UC5_Optimaliseren"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Voertuigen kunnen via CAM berichten aanvragen zetten en verlengen op een rijstrook (lane)."));

            items.Add(OpenXmlHelper.GetTextParagraph(
                "In verband met de onnauwkeurige plaatsbepaling van de voertuigen (GPS-positie) dient goed overwogen te worden of " +
                "aanvragen en verlengen via CAM niet te verstorend werkt wanneer er meerdere lane ID's naast elkaar liggen op dezelfde arm."));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"In onderstaand overzicht zijn de afstanden opgenomen waarop voertuigen via CAM berichten kunnen aanvragen en verlengen, " +
                "wanneer de betreffende schakelaars SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schrisaanvraag") + 
                " en SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schrisverlengen") + " 'AAN' staan."));

            items.Add(OpenXmlHelper.GetTextParagraph("Wanneer niet op alle rijstroken dezelfde voertuigtypes zijn ingesteld voor zowel " +
                "aanvragen als verlengen, dient onderstaand overzicht handmatig gecorrigeerd te worden.", "TODO"));

            items.AddRange(TableGenerator.Table_TT_UC5_Optimaliseren(c));

            return items;
        }
    }
}
