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

namespace TLCGen.Specificator
{
    public static class FunctionalityGenerator
    {
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

        internal static List<OpenXmlCompositeElement> GetChapter_DetectieConfiguratie(WordprocessingDocument doc, ControllerModel c)
        {
            var body = doc.MainDocumentPart.Document.Body;
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren_Configuratie"]}", 2));
            items.Add(OpenXmlHelper.GetTextParagraph(
                $"De belangrijkste doelen van het detectieveld zijn:"));

            var sl = new List<Tuple<string, int>>();

            sl.Add(new Tuple<string, int>($"Het aanvragen van signaalgroepen (bij lussen pas na het verstrijken " +
                $"van de bezettijd). Per lus of drukknop dient met een parameter instelbaar te zijn of en wanneer " +
                $"de aanvraag gezet mag worden:", 0));
            sl.Add(new Tuple<string, int>($"0 = Aanvraagfunctie uitgeschakeld;", 1));
            sl.Add(new Tuple<string, int>($"1 = Aanvragen na garantierood;", 1));
            sl.Add(new Tuple<string, int>($"2 = Aanvragen tijdens de gehele roodfase;", 1));
            sl.Add(new Tuple<string, int>($"3 = Aanvragen tijdens geel en rood.", 1));
            sl.Add(new Tuple<string, int>($"Het op een veilige manier verlengen en beëindigen van signaalgroepen " +
                $"(bij lussen pas na het verstrijken van de hiaattijd). Per lus dient met een parameter instelbaar " +
                $"te zijn of en volgens welk principe er verlengd mag worden: ", 0));
            sl.Add(new Tuple<string, int>($"0 = Verlengfunctie uitgeschakeld;", 1));
            sl.Add(new Tuple<string, int>($"1 = Koplusmaximum: beperkt gedeelte van maximum groentijd. Bedoeld " +
                $"voor koplussen, om te voorkomen dat, wanneer het eerste voertuig niet oplet, de groenfase na " +
                $"de vastgroentijd wordt beëindigd.Op deze manier kunnen de voertuigen tussen de koplus en de " +
                $"lange lus toch worden weggewerkt.Op koplussen wordt in principe niet doorverlengd: voertuigen " +
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

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_Ontruimingstijden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Ontruimingstijden"]}", 2));

            var otTable = TableGenerator.GetTable_Ontruimingstijden(c);

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"De ontruimingstijden zijn opgenomen in tabel {TableGenerator.Tables["Table_Ontruimingstijden"]}. Deze tijden " +
                $"zijn weergegeven in seconden van eindgeel tot begin groen van signaalgroepen welke worden beveiligd volgens " +
                $"NEN 3384 en de veiligheidseisen.Daar waar geen waarde is ingevuld zijn signaalgroepen onderling niet beveiligd."));

            items.AddRange(otTable);

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_OntruimingstijdenGarantie(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!c.Data.GarantieOntruimingsTijden) return items;

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OntruimingstijdenGarantie"]}", 2));

            var otTable = TableGenerator.GetTable_OntruimingstijdenGarantie(c);

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"De garantie ontruimingstijden zijn opgenomen in tabel {TableGenerator.Tables["Table_OntruimingstijdenGarantie"]}. De " +
                $"ontruimingstijden kunnen niet lager worden ingesteld dan de garantie ontruimingstijden. De garantie " +
                $"ontruimingstijden zelf zijn allen-lezen."));

            items.AddRange(otTable);

            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_OVHDIntro(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
            items.Add(OpenXmlHelper.GetTextParagraph(
                $"De regeling heeft zowel ingrepen voor afhandeling van openbaar vervoer, als voor afhandeling van hulpdiensten. " +
                $"De navolgende paragrafen geven een overzicht van de werking en instellingen voor deze ingrepen."));
            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_HD(ControllerModel c, WordprocessingDocument doc, int startLevel)
        {
            var items = new List<OpenXmlCompositeElement>();
            var text = "De regeling is uitgerust met hulpdienstingrepen op basis van ";
            if (c.HasHDKAR() && c.HasHDOpticom()) text += "KAR en Opticom.";
            else if (c.HasHDKAR()) text += "KAR.";
            else if (c.HasHDOpticom()) text += "Opticom.";
            text +=
                $" Een hulpdienstingreep kent de hoogste vorm van prioriteit (zie tabel {TableGenerator.Tables["Table_OV_PrioriteitsOpties"]}: " +
                $"{Texts["Table_OV_PrioriteitsOpties"]}). Deze ingreep kapt af (na verstrijken van de garantiegroentijd) en blokkeert " +
                $"alle signaalgroepen die conflicteren met de hulpdienst.";
            if (c.PrioData.BlokkeerNietConflictenBijHDIngreep)
            {
                if (c.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer)
                    text += $" Van niet-conflicten worden alleen richtingen met langzaam verkeer afgekapt en geblokkeerd.";
                else
                    text += $" Ook niet-conflicten worden afgekapt, voor zover ze niet ook een actieve hulpdienst ingreep hebben.";
            }
            text += " Tijdens een hulpdienstingreep wordt de fasebewakingstimer herstart.";
            items.Add(OpenXmlHelper.GetTextParagraph(text));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"De instellingen en opties voor hulpdienstingrepen worden weergegeven in onderstaande tabel:"));
            items.AddRange(TableGenerator.GetTable_HD_Instellingen(c));
            
            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_OV(ControllerModel c, WordprocessingDocument doc, int startLevel)
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
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));

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
                "OV-richting, pas afgekapt te worden op het moment dat de geeltijd +ontruimingstijd  resterende rijtijd. Wanneer " +
                "de OV-richting door omstandigheden niet op het gewenste moment groen kan worden, bijvoorbeeld doordat een " +
                "conflictrichting niet afgekapt mag worden, hoeven (overige) conflictrichtingen niet onnodig vroeg te worden " +
                "afgekapt. Er moet dan bepaald worden op welk moment de OV-richting op zijn vroegst groen kan worden. Aan " +
                "de hand van dat moment moet (opnieuw) bepaald worden wanneer conflictrichtingen afgekapt moeten worden."));

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVIngreepBijzonderRealiseren"]}", startLevel + 2));
            items.Add(OpenXmlHelper.GetTextParagraph("Als een richting bijzonder mag realiseren betekent dat de realisatie " +
                "buiten de modulestructuur om plaats vindt. De OV-richting wordt op deze manier eigenlijk tussen de modulen " +
                "door gerealiseerd."));
            items.Add(OpenXmlHelper.GetTextParagraph("Tijdens een realisatie ten behoeve van een OV-ingreep wordt het groen " +
                "vastgehouden zolang het voertuig zich niet heeft uitgemeld en de groenbewaking niet is verstreken.Tijdens " +
                "een bijzondere realisatie mogen richtingen die niet conflicterend zijn met de OV - richting alternatief " +
                "realiseren. Hiervoor gelden dezelfde voorwaarde die gelden voor alternatief realiseren."));

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVIngreepNaOVIngreep"]}", startLevel + 2));
            items.Add(OpenXmlHelper.GetTextParagraph("Na afloop van de OV-ingreep gaat de regeling verder op het punt waar " +
                "de regeling was voor de OV-ingreep.Afhankelijk van de instelling van de parameter \"Percentage groentijd " +
                "t.b.v. terugkeren\" en de gerealiseerde groentijden van de afgekapte richtingen, worden deze richtingen al " +
                "dan niet opnieuw groen.Deze richtingen mogen hun resterende deel van maximum groentijd maken, of in ieder " +
                "geval uitverlengen tot en met de \"Ondergrens na terugkomen\". Is er geen sprake van richtingen die " +
                "terugkeren, dan worden de richtingen groen die volgens de regelstructuur als eerstvolgende aan de " +
                "beurt zijn."));

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVIngreepDefecteUitmelding"]}", startLevel + 2));
            items.Add(OpenXmlHelper.GetTextParagraph("Wanneer er voor het aflopen van de groenbewaking geen goede uitmelding " +
                "is ontvangen, wordt schakelbaar(default uit) de prioriteitsingreep uitgeschakeld. Er wordt dan vanuit gegaan " +
                "dat de uitmelding niet goed binnenkomt. De prioriteitsingreep wordt pas weer geactiveerd als er na een " +
                "correcte inmelding ook een correcte uitmelding is ontvangen."));
            items.Add(OpenXmlHelper.GetTextParagraph("De groenbewakingstijd loop altijd tijdens groen als een OV-voertuig " +
                "is ingemeld, alleen wordt het groen van een OV-richting zonder prioriteit niet vastgehouden. Dit zal tot " +
                "gevolg hebben dat na het constateren van een defecte uitmelding de aanvraag op de betreffende richting " +
                "blijft staan totdat de richting langer dan de groenbewakingstijd groen is geweest."));
            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVParameters"]}", startLevel + 1));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Met behulp van een parameter zijn voor elke signaalgroep met OV de verschillende prioriteitsopties in te stellen. " +
                $"Het honderdtal, het tiental en de eenheid van de parameter kunnen hiervoor worden gebruikt.Er kunnen dus maximaal " +
                $"3 prioriteitsopties worden gekozen.Met de waarde 123 krijgt de OV-richting de opties aanvragen, afkappen, groen " +
                $"vasthouden en bijzonder realiseren(\"volledige prioriteit\"). Mag de OV-richting ook conflicterende OV-richtingen " +
                $"afkappen dan wordt de waarde 234 toegekend; optie 4 maakt optie 1 overbodig. Met behulp van de waarde 500 wordt " +
                $"een nooddienst toegekend; optie 5 maakt opties 1, 2, 3 en 4 overbodig."));
            if(c.PrioData.PrioIngrepen.Any(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
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
                "duizendtal van de parameter gereserveerd.Het prioriteitsniveau kan dus variëren van 0 t / m 9.Een richting met een " +
                "hoger prioriteitsniveau dan een conflictrichting, mag de prioriteitsafhandeling van die conflictrichting intrekken " +
                "en afhankelijk van de prioriteitsopties afkappen. Voor prioriteitsingrepen met hetzelfde prioriteitsniveau geldt " +
                "het “first in first out” principe."));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Wanneer een voertuig zich heeft ingemeld en de prioriteit niet is ingesteld op 0, wordt op basis van parameter " +
                $"instellingen bepaald of, gegeven de actuele status van de regeling, prioriteit mag worden verleend. Hiervoor zijn " +
                $"de volgende instellingen beschikbaar:"));

            sl = new List<Tuple<string, int>>
            {
                new Tuple<string, int>("Rijtijd: per signaalgroep wordt voor maximaal 10 voertuigen vanaf het moment dat een " +
                    "voertuig zich inmeldt de rijtijd bijgehouden.Deze rijtijd wordt afgezet tegen de tijd die het voertuig nodig heeft " +
                    "om vanaf het inmeldpunt tot aan het punt voor de stopstreep te komen waarop het licht groen moet zijn. Er worden " +
                    "drie parameters gebruikt:", 0),
                new Tuple<string, int>("Ongehinderde rijtijd. Alle lussen zijn onbezet.", 1),
                new Tuple<string, int>("Beperkt gehinderde rijtijd. Koplus of lange lus is bezet.", 1),
                new Tuple<string, int>("Gehinderde rijtijd. Koplus en lange lus tegelijkertijd gedurende een instelbare tijd bezet.", 1),
                new Tuple<string, int>("Blokkeringstijd: Ten behoeve van het niet honoreren van een prioriteitsaanvraag, zolang de " +
                    "blokkeringtijd loopt. Deze tijd wordt per OV-richting bijgehouden en wordt gestart op startrood en afgekapt als " +
                    "er geen (fictief) conflicterende aanvragen zijn. Deze instelling speelt geen rol bij OV-inmeldingen met " +
                    "prioriteitsoptie nooddienst.", 0),
                new Tuple<string, int>("Groenbewaking: Geeft aan hoelang een OV-richting maximaal groen krijgt ten behoeve van een " +
                    "OV-voertuig. Dit wordt per OV-voertuig bijgehouden. Er wordt daarbij vanuit gegaan dat er per OV-traject niet meer " +
                    "dan 10 OV-voertuigen tegelijkertijd ingemeld kunnen zijn. Na afloop van de groenbewaking, zonder een uitmelding, " +
                    "wordt het OV-voertuig uitgemeld.", 0),
                new Tuple<string, int>("Ondermaximum: Hiermee wordt aangegeven tot welk moment voor het bereiken van de maximum " +
                    "groentijd een OV-voertuig nog prioriteit mag krijgen. Deze instelling speelt geen rol bij OV-inmeldingen met " +
                    "prioriteitsoptie nooddienst.", 0)
            };
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Onderstaande tabel geeft een overzicht van de prioriteitsinstellingen voor richtingen met OV in deze regeling:"));

            items.AddRange(TableGenerator.GetTable_OV_PrioriteitsInstellingen(c));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Naast bovenstaande parameters zijn een aantal parameters beschikbaar om de mate van verstoring door een OV-ingreep " +
                $"te beperken:"));

            sl = new List<Tuple<string, int>>
            {
                
                new Tuple<string, int>("Wachttijdcriterium: Indien één of meer fasecycli een hogere wachttijd hebben dan de waarde " +
                    "van het wachttijdcriterium, mag er niet bijzonder gerealiseerd worden. Conflicten mogen wel afgebroken worden om " +
                    "zodoende een versnelde cyclus te kunnen maken. De waarde van het wachttijdcriterium is voor autorichtingen (default " +
                    "120 seconden), fietsrichtingen (default 90 seconden) en voetgangersrichtingen (default 90 seconden) apart in te stellen. Deze instelling speelt geen rol bij OV-inmeldingen met prioriteitsoptie nooddienst.", 0),
                new Tuple<string, int>("Afkapgarantie bij conflicterend OV: Per richting is middels een parameter aan te geven hoelang " +
                    "die richting groen mag zijn, voordat deze kan worden afgekapt door een conflicterende OV-ingreep. Deze instelling speelt geen rol bij OV-inmeldingen met prioriteitsoptie nooddienst.", 0),
                new Tuple<string, int>("Percentage maximum groentijd bij conflicterend OV: Per richting is middels een percentage aan " +
                    "te geven hoelang die richting groen mag zijn, voordat deze kan worden afgekapt door een conflicterende OV-ingreep. " +
                    "Deze instelling speelt geen rol bij OV-inmeldingen met prioriteitsoptie nooddienst.", 0),
                new Tuple<string, int>("Ophoogpercentage maximum groentijd bij conflicterend OV: Per richting is een percentage aan te " +
                    "geven waarmee het percentage maximum groentijd bij conflicterend OV wordt opgehoogd nadat die richting is afgebroken. " +
                    "Staat het percentage maximum groentijd bij conflicterend OV bijvoorbeeld op 50% en het ophoogpercentage op 25%, dan mag " +
                    "een afgekapte richting de volgende cyclus 75% van zijn groen maken, voordat hij wordt afgekapt. Wordt de richting dan " +
                    "weer afgekapt dan mag de cyclus daarna 100% worden gemaakt. Dit mag er overigens nooit toe leiden dat het percentage " +
                    "maximum groen bij conflicterend OV groter wordt dan 100%. Wanneer een richting op hiaat wordt beëindigd of een keer " +
                    "100% van zijn maximum groen maakt, wordt het percentage maximum groentijd bij conflicterend OV gereset naar de " +
                    "ingestelde waarde.", 0),
                new Tuple<string, int>("Aantal malen niet afbreken bij conflicterend OV: Geeft aan hoe vaak een conflicterende richting " +
                    "niet mag worden afgebroken, nadat deze richting afgekapt is ten behoeve van een ingreep.", 0),
                new Tuple<string, int>("Percentage groentijd t.b.v. terugkeren: Per richting is een percentage van de groentijd aan te " +
                    "geven waarvoor geldt dat als de primaire realisatie van een richting wordt afgekapt door een conflicterende OV-ingreep " +
                    "en dit percentage van de groentijd nog niet is bereikt, dat die richting dan na afloop van de conflicterende OV-ingreep " +
                    "opnieuw wordt gerealiseerd.", 0),
                new Tuple<string, int>("Ondergrens na terugkomen: Als na afloop van een OV-ingreep een afgekapte conflicterende " +
                    "richting opnieuw mag realiseren dan mag deze richting het restant van zijn maximum groentijd maken. Als dit restant " +
                    "lager is dan de ondergrens na terugkomen dan dient de ondergrens gehanteerd te worden.", 0)
            };
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Onderstaande tabel geeft een overzicht van de instellingen voor met het OV conflicteren richtingen:"));

            items.AddRange(TableGenerator.GetTable_OV_ConflictenInstellingen(c));


            return items;
        }

        internal static IEnumerable<OpenXmlElement> GetChapter_Synchronisaties(ControllerModel c, WordprocessingDocument doc)
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!c.Data.GarantieOntruimingsTijden) return items;

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Synchronisaties"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Dit hoofdstuk geeft een overzicht van de synchronisaties tussen signaalgroepen binnen de regeling."));

            if (c.InterSignaalGroep.Meeaanvragen.Any())
            {
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Meeaanvragen"]}", 3));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Een meeaanvraag tussen twee signaalgroepen zorgt ervoor dat een aanvraag bij de ene signaalgroep " +
                    $"onder bepaalde voorwaarden zorgt dat ook de andere signaalgroep een aanvraag krijgt, ongeacht de " +
                    $"aanwezigheid van verkeer bij die tweede signaalgroep. Hierbij zijn de volgende voorwaarden mogelijk:"));
                var sl = new List<Tuple<string, int>>();
                sl.Add(new Tuple<string, int>("Meeaanvraag op aanvraag.", 0));
                sl.Add(new Tuple<string, int>("Meeaanvraag op rood voor aanvraag (RA).", 0));
                sl.Add(new Tuple<string, int>("Meeaanvraag op RA en geen conflicten", 0));
                sl.Add(new Tuple<string, int>("Meeaanvraag op start groen", 0));
                items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Elke meeaanvraag kan als detectie afhankelijke meeaanvraag worden ingesteld, zodat enkel bij een melding " +
                    $"op een bepaalde detector, de meeaanvraag wordt gezet. Optioneel is het mogelijk het type meeaanvraag het " +
                    $"type meaanvraag instelbaar te maken op straat. Voor meeaanvragen op startgroen kan worden gekozen voor een " +
                    $"(instelbare) uitgestelde meeaanvraag. Voor meeaanvragen gelden de volgende instellingen:"));
                items.AddRange(TableGenerator.GetTable_Intersignaalgroep_Meeaanvragen(c));
            }
            if (c.InterSignaalGroep.Nalopen.Any())
            {
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Nalopen"]}", 3));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Een naloop tussen twee signaalgroepen zorgt ervoor dat het groen op de volgrichting " +
                    $"onder bepaalde voorwaarden wordt vastgehouden wanneer de voedende richting groen is (geweest), ongeacht de " +
                    $"aanwezigheid van verkeer bij die tweede signaalgroep. Hierbij zijn de volgende voorwaarden mogelijk:"));
                var sl = new List<Tuple<string, int>>();
                sl.Add(new Tuple<string, int>("Naloop op startgroen.", 0));
                sl.Add(new Tuple<string, int>("Naloop tijdens cyclisch verlengroen.", 0));
                sl.Add(new Tuple<string, int>("Naloop op einde groen", 0));
                items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Elke naloop kan als detectie afhankelijke naloop worden ingesteld, zodat enkel bij een melding " +
                    $"op een bepaalde detector, de naloopo actief wordt. Bij detectieafhankelijke nalopen kan ervoor worden " +
                    $"gekozen geen vaste naloop aan te houden. Voor nalopen gelden de volgende instellingen:"));
                items.Add(OpenXmlHelper.GetTextParagraph("TODO: bovenstaande tekst moet de instelling \"inlopen/inrijden " +
                    "tijdens groen\" nader toelichten."));
                items.AddRange(TableGenerator.GetTable_Intersignaalgroep_Nalopen(c));

            }
            if (c.InterSignaalGroep.Gelijkstarten.Any())
            {
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Gelijkstarten"]}", 3));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Wanneer tussen twee signaalgroepen een gelijktart geldt, wordt ervoor gezorgd dat de betreffende " +
                    $"richtingen gelijktijdig groen krijgen wanneer beide een aanvraag hebben. Is er sprake van een deelconflict, " +
                    $"dan gelden aanvullend fictieve ontruimingstijden tussen beide richtingen, en mag een richting niet bij komen " +
                    $"wanneer de andere richting reeds groen is."));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Voor gelijkstarten gelden de volgende instellingen:"));
                items.AddRange(TableGenerator.GetTable_Intersignaalgroep_Gelijkstarten(c));
            }
            if (c.InterSignaalGroep.Voorstarten.Any())
            {
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Voorstarten"]}", 3));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Bij toepassen van een voorstart krijgt de ene richting altijd groen een instelbare tijd vooraf aan start groen " +
                    $"van de andere richting. Die tweede richting wordt hiertoe tijdelijk tegengehouden. Er geldt een fictieve " +
                    $"ontruimingstijd van de tweede richting naar de eerste: pas na het aflopen van die tijd kan de voorstartende " +
                    $"richting weer groen worden."));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Voor voorstarten gelden de volgende instellingen:"));
                items.AddRange(TableGenerator.GetTable_Intersignaalgroep_Voorstarten(c));
            }
            if (c.InterSignaalGroep.LateReleases.Any())
            {
                items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_LateRelease"]}", 3));
                items.Add(OpenXmlHelper.GetTextParagraph("TODO: deze tekst moet uitleg geven over de functionele werking van een late release, " +
                    "inclusief het verschil met een reguliere voorstart.", "TODO"));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Bij toepassen van een late release ..."));
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Voor late release gelden de volgende instellingen:"));
                items.AddRange(TableGenerator.GetTable_Intersignaalgroep_LateRelease(c));
            }

            return items;
        }


        internal static List<OpenXmlCompositeElement> GetChapter_DetectieInstellingen(WordprocessingDocument doc, ControllerModel c)
        {
            var body = doc.MainDocumentPart.Document.Body;
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren_Instellingen"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Hieronder worden eerst de functionele instellingen voor de detectoren op de kruising weergegeven. Vervolgens worden " +
                $"de tijdsinstellingen voor de detectoren weergegeven."));

            items.AddRange(TableGenerator.GetTable_Detectie_Functies(c));
            items.AddRange(TableGenerator.GetTable_Detectie_Tijden(c));

            return items;
        }

        internal static List<OpenXmlCompositeElement> GetChapter_DetectieStoring(WordprocessingDocument doc, ControllerModel c)
        {
            var body = doc.MainDocumentPart.Document.Body;
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren_Storing"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"De detectie dient bewaakt te worden op boven- en ondergedrag. De tijden die hierbij gelden zijn weergegeven in de tabel " +
                $"met tijdsinstellingen voor detectoren (tabel {TableGenerator.Tables["Table_Detectoren_Tijden"]})."));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Het is mogelijk maatregelen te nemen wanneer een of meer detectoren voor een richting in storing komen. Sowieso geldt: " +
                $"van een lus in storing worden in de regelapplicatie alle functies uitgeschakeld. Daarnaast zijn de volgende maatregelen mogelijk:"));

            var sl = new List<Tuple<string, int>>();
            sl.Add(new Tuple<string, int>($"Per lus (of knop) is instelbaar dat een storing zorgt voor een vaste aanvraag. Deze instelling is opgenomen in de tabel met detectie functies (tabel {TableGenerator.Tables["Table_Detectoren_Functies"]}).", 0));
            sl.Add(new Tuple<string, int>($"Bij storing van de lange lus kan de koplus de verlengfunctie van de lange lus overnemen met een extra per koplus instelbare hiaattijd.", 0));
            sl.Add(new Tuple<string, int>($"Als van een {Texts["Generic_fase"]} alle aanvraag detectie in storing is, kan worden ingesteld dat een (schakelbaar) een vaste aanvraag wordt gezet.", 0));
            sl.Add(new Tuple<string, int>($"Als van een {Texts["Generic_fase"]} alle verlengdetectie in storing is, kan worden ingesteld dat een instelbaar percentage van de actuele maximum of verlenggroentijd wordt gemaakt.", 0));
            items.AddRange(OpenXmlHelper.GetBulletList(doc, sl));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Hieronder worden de instellingen voor detectiestoring per {Texts["Generic_fase"]} weergegeven."));

            items.AddRange(TableGenerator.GetTable_Detectie_StoringMaatregelen(c));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_DetectieRichtingGevoelig(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            if (!c.RichtingGevoeligeAanvragen.Any() && !c.RichtingGevoeligVerlengen.Any()) return items;

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren_Richtinggevoelig"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Detectoren kunnen in paren benut worden voor het richtinggevoelig aanvragen en/of verlengen van {Texts["Generic_fasen"]}. " +
                $"Hieronder wordt een overzicht gegeven van deze functionaliteit."));

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
                    CCOLGenHelper.Dpf + rga.VanDetector,
                    CCOLGenHelper.Dpf + rga.NaarDetector,
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

        public static void GetIntroChapter(WordprocessingDocument doc, ControllerModel c, SpecificatorDataModel model)
        {
            var body = doc.MainDocumentPart.Document.Body;

            body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Intro"]}", 1));

            var text = 
                $"Dit document beschrijft de functionele eisen aangaande de verkeersregelinstallatie (VRI) op het " +
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
                var file = Path.Combine(path, (Regex.IsMatch(c.Data.BitmapNaam, @"\.(bmp|BMP)$") ? c.Data.BitmapNaam : c.Data.BitmapNaam + ".bmp"));
                if (File.Exists(file))
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

            body.Append(OpenXmlHelper.GetTextParagraph("De verkeersregeling dient aan de volgende eisen te voldoen: "));
            sl.Clear();
            sl.Add(new Tuple<string, int>("Veilige situatie op het kruispunt.", 0));
            sl.Add(new Tuple<string, int>("Goede en efficiënte verkeersafwikkeling.", 0));
            sl.Add(new Tuple<string, int>("Logische en acceptabele situatie voor weggebruikers.", 0));
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
            if (c.Fasen.Any(x => x.WachttijdVoorspeller))
            {
                l.Add(new List<string> { (string)Texts["Generic_WTVNietHalterenAlsMeerDanLeds"], c.Data.WachttijdvoorspellerNietHalterenMax.ToString() });
                l.Add(new List<string> { (string)Texts["Generic_WTVNietHalterenAlsMinderDanLeds"], c.Data.WachttijdvoorspellerNietHalterenMin.ToString() });
                l.Add(new List<string> { (string)Texts["Generic_WTVAansturenBusTijdensOV"], c.Data.WachttijdvoorspellerNietHalterenMin.ToString() });
            }
            body.Append(OpenXmlHelper.GetTable(l));
        }

        // TODO: tabel met ingangen (indien aanwezig)
        // TODO: tabel met selectieve detectie (indien aanwezig)

        public static string GetGroentijden(ControllerModel c, bool multiple = true)
        {
            if(multiple) return $"{(c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? "maximum" : "verleng")} groentijden";
            else return $"{(c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? "maximum" : "verleng")} groentijd";
        }

        public static List<OpenXmlCompositeElement> GetChapter_StructureIntroduction(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
            var t = "";
            if (c.HalfstarData.IsHalfstar)
            {
                t = $"De kruising wordt zowel voertuigafhankelijk (VA) als halfstar geregeld. ";
            }
            else
            {
                t = $"De kruising wordt voertuigafhankelijk (VA) geregeld. ";
            }
            t +=  "VA regelen houdt in de signaalgroepen binnen de regeling groen krijgen toegedeeld " +
                 $"op basis van de aanwezigheid van voertuigen. Hierbij gelden periode gebonden {GetGroentijden(c)}. " +
                 $"In dit hoofdstuk worden de voertuigafhankelijke signaalgroepafhandeling en de modulenstructuur " +
                 $"nader toegelicht.";
            if (c.HalfstarData.IsHalfstar)
            {
                t += $"De functionaliteit van en instellingen omtrent halfstar regelen komen " +
                     $"in hoofdstuk inzake halfstar regelen aan de orde. ";
            }

            items.Add(OpenXmlHelper.GetTextParagraph(t));

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
                    $"klokperiodes ten behoeve van de {GetGroentijden(c)}"));
            }
            else
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Voor voertuigafhankelijk regelen gelden de volgende " +
                    $"klokperiodes ten behoeve van de {GetGroentijden(c)}"));
            }

            items.AddRange(TableGenerator.GetTable_Perioden(c));

            items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Wanneer geen van bovenstaande perioden actief is geldt de dalperiode (met groentijdenset {c.PeriodenData.DefaultPeriodeGroentijdenSet})."));

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

            var tableModulen = TableGenerator.GetTable_Modulen(c);

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Modulestructuur"]}", 2));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Tijdens voertuigafhankelijk regelen wordt gebruik gemaakt van de modulestructuur. De " +
                "modulestructuur is opgebouwd uit primaire, bijzondere en alternatieve realisaties."));
            items.Add(OpenXmlHelper.GetTextParagraph("Primaire realisatie", bold: true));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "De modulestructuur bepaalt in welke vaste volgorde de verschillende signaalgroepen met een aanvraag " +
                "groen worden (primaire realisaties). Als geen enkele signaalgroep een aanvraag heeft dan wacht de " +
                $"modulestructuur in module {c.ModuleMolen.WachtModule}. Een moduleovergang vindt plaats op het moment dat alle primair " +
                "toegedeelde signaalgroepen met een aanvraag van het actieve blok cyclisch verlenggroen hebben " +
                "verlaten."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Alle signaalgroepen zijn toegedeeld aan minimaal één module (zie tabel {TableGenerator.Tables["Table_Modulen"]}). Een signaalgroep die " +
                "een aanvraag heeft gekregen (in het aanvraaggebied) wordt tijdens het actief zijn van de module " +
                "waaraan deze is toegedeeld, eenmalig primair gerealiseerd. De groenduur van een primaire " +
                "realisatie wordt beperkt volgens de ingestelde maximumgroentijd (zie tabel " +
                $"{(c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? TableGenerator.Tables["Table_MaxGroentijden"] : TableGenerator.Tables["Table_VerlGroentijden"])}) per klokperiode " +
                $"(zie tabel {TableGenerator.Tables["Table_Perioden"]})."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                 "Het aanvraaggebied voor primair toegedeelde richtingen van een module wordt afgesloten nadat " +
                 "de desbetreffende module actief is geworden en er een aanvraag is gezet door een signaalgroep " +
                 "uit een volgende module. Bij de moduleovergang wordt het aanvraaggebied van de richtingen van " +
                 "de module die verlaten wordt weer opengesteld."));
            items.Add(OpenXmlHelper.GetTextParagraph(
                  "Door het gebruik van versnelde primaire realisaties worden richtingen onder voorwaarden (bijvoorbeeld afwezigheid aanvragen van eerder in de regelstructuur te realiseren conflictrichtingen) eerder groen. Versnelde realisaties geven, met name in rustige perioden, een grote flexibiliteit aan de verkeersregeling waardoor de acceptatie en geloofwaardigheid van de verkeersregeling hoger wordt. Met behulp van een parameter is per richting in te stellen hoeveel module er vooruit mag worden gerealiseerd. Default mogen alle richtingen één module vooruit realiseren."));
            
            items.Add(OpenXmlHelper.GetTextParagraph("Bijzondere realisaties", bold: true));
            items.Add(OpenXmlHelper.GetTextParagraph("TODO: de tekst hieronder moet later verwijzen naar de betreffende hoofdstukken OV en HD.", "TODO"));
            items.Add(OpenXmlHelper.GetTextParagraph(
                "Naast de primaire realisaties kunnen signaalgroepen ook bijzonder realiseren. Bij een bijzondere " +
                "realisatie wordt een signaalgroep buiten de modulestructuur om naar groen gestuurd. Bijzondere " +
                "realisaties worden gebruikt om prioriteit te geven aan bussen of hulpdiensten."));

            items.Add(OpenXmlHelper.GetTextParagraph("Alternatieve realisaties", bold: true));
            if (c.ModuleMolen.LangstWachtendeAlternatief)
            {
                items.Add(OpenXmlHelper.GetTextParagraph("TODO: klopt onderstaande tekst vwb default prio voor fietsers?", "TODO"));
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
                    "regeling beschikbaar moet zijn om alternatief te mogen realiseren. Deze ruimte wordt berekend op basis van " +
                    $"de vigerende {GetGroentijden(c, false)} van een " +
                    "niet conflict dat op dat moment in cyclisch verlenggroen staat, of op het punt staat om primair " +
                    "dan wel bijzonder te realiseren. Hierbij wordt voor richtingen met een naloop rekening mee gehouden " +
                    "dat ook die moet passen. Voor iedere richting is opgegeven wat de maximale groentijd voor " +
                    "alternatief realiseren is, als de voorwaarde voor alternatief realiseren is vervallen."));
            }
            else
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    "Alternatieve relatisaties zijn binnen de regeling uitsluitend mogelijk voor richtingen waarvoor expliciet is " +
                    "ingesteld data deze onder dekking van een of meer andere richtingen alternatief mogen komen. Dit is ingesteld " +
                    "richting per blok. Hiervoor gelden de volgende instellingen:"
                    ));
                items.AddRange(TableGenerator.GetTable_AlternatievenOnderDekking(c));
            }

            items.AddRange(tableModulen);

            items.Add(OpenXmlHelper.GetTextParagraph("De instellingen voor vooruit realiseren en alternatieve realisaties worden hieronder weergegeven."));

            items.AddRange(TableGenerator.GetTable_ModuleStructuurInstellingen(doc, c));

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

            

            return items;
        }

        public static List<OpenXmlCompositeElement> GetChapter_Groentijden(ControllerModel c)
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


    }
}
