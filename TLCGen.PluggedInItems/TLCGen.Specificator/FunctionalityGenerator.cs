using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TLCGen.Extensions;
using TLCGen.Models;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace TLCGen.Specificator
{
    public static class FunctionalityGenerator
    {
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

        public static List<OpenXmlCompositeElement> GetFasenChapter(ControllerModel c)
        {
            var title = OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Fasen"]}", 2);
            var tableTitleFunties = OpenXmlHelper.GetTextParagraph((string)Texts["Table_Fasen_Functies"], styleid: "Caption");

            var tableTitleTijden = OpenXmlHelper.GetTextParagraph((string)Texts["Table_Fasen_Tijden"], styleid: "Caption");

            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"],
                    "Type",
                    "Rijstroken",
                    "Vaste aanvraag",
                    "Wachtgroen",
                    "Meeverlengen",
                    "Wachttijd voorspellers",
                    "Aantal detectoren"
                }
            };
            foreach (var fc in c.Fasen)
            {
                l.Add(new List<string>
                {
                    fc.Naam,
                    fc.Type.GetDescription(),
                    fc.AantalRijstroken.ToString(),
                    fc.VasteAanvraag.GetDescription(),
                    fc.Wachtgroen.GetDescription(),
                    fc.Meeverlengen.GetDescription(),
                    fc.WachttijdVoorspeller == true ? "X" : "-",
                    fc.Detectoren.Count.ToString()
                });
            }
            var tableFuncties = OpenXmlHelper.GetTable(l, firstRowVerticalText: true);

            l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"],
                    "Vastgroen",
                    "Garantiegroen",
                    "Minimum garantiegroen",
                    "Garantierood",
                    "Minimum garantierood",
                    "Geel",
                    "Minimum geel",
                    "Kopmax"
                }
            };
            // TODO: veiligheidsgroen
            foreach (var fc in c.Fasen)
            {
                l.Add(new List<string>
                {
                    fc.Naam,
                    fc.TFG.ToString(),
                    fc.TGG.ToString(),
                    fc.TGG_min.ToString(),
                    fc.TRG.ToString(),
                    fc.TRG_min.ToString(),
                    fc.TGL.ToString(),
                    fc.TGL_min.ToString(),
                    fc.Detectoren.Any(x => (x.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.Kopmax)) ? fc.Kopmax.ToString() : "-" });
            }
            var tableTijden = OpenXmlHelper.GetTable(l, firstRowVerticalText: true);

            return new List<OpenXmlCompositeElement> { title, tableTitleFunties, tableFuncties, tableTitleTijden, tableTijden };
        }

        public static List<OpenXmlCompositeElement> GetPeriodenChapter(ControllerModel c)
        {
            var title = OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Perioden"]}", 2);
            var tabelTitle = OpenXmlHelper.GetTextParagraph((string)Texts["Table_Perioden"], styleid: "Caption");

            var l = new List<List<string>>
            {
                new List<string> { "Periode", "Start", "Einde", "Dagtype" }
            };
            foreach (var p in c.PeriodenData.Perioden)
            {
                l.Add(new List<string> { p.Naam, p.StartTijd.ToString(@"hh\:mm"), p.EindTijd.ToString(@"hh\:mm"), p.DagCode.ToString() });
            }
            var table = OpenXmlHelper.GetTable(l);

            return new List<OpenXmlCompositeElement> { title, tabelTitle, table };
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
