using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Linq;
using TLCGen.Extensions;
using TLCGen.Models;
using Style = DocumentFormat.OpenXml.Wordprocessing.Style;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace TLCGen.Specificator
{
    public static partial class SpecificationGenerator
    {
        private static ResourceDictionary _texts;
        public static ResourceDictionary Texts
        {
            get
            {
                if(_texts == null)
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

        public static void AddFirstPage(WordprocessingDocument doc, ControllerDataModel data)
        {
            var par = doc.MainDocumentPart.Document.Body.AppendChild(new Paragraph());
            var run = par.AppendChild(new Run());

            run.AppendChild(new Text($"{Texts["Title"]} {data.Naam}"));
            ApplyStyleToParagraph(doc, par, "Title");

            par = doc.MainDocumentPart.Document.Body.AppendChild(new Paragraph());
            run = par.AppendChild(new Run());

            run.AppendChild(new Text($"{data.Straat1}{(!string.IsNullOrWhiteSpace(data.Straat2) ? " - " + data.Straat2 : "")}"));
            ApplyStyleToParagraph(doc, par, "Subtitle");

            par = doc.MainDocumentPart.Document.Body.AppendChild(new Paragraph());
            run = par.AppendChild(new Run());

            run.AppendChild(new Text($"{data.Stad}"));
            ApplyStyleToParagraph(doc, par, "Subtitle");
        }

        public static void AddRowToTable(WordprocessingDocument doc, Table table, string[] data, int[] widths = null)
        {
            var tr = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
            for (int i = 0; i < data.Length; i++)
            {
                string d = data[i];
                var tc = new DocumentFormat.OpenXml.Wordprocessing.TableCell();
                var par = new Paragraph(new Run(new Text(d)));
                ApplyStyleToParagraph(doc, par, "TableContents");
                tc.Append(par);
                if(widths != null && widths.Length == data.Length)
                {
                    tc.Append(
                        new TableCellProperties(
                            new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = $"{widths[i] * 50 }" },
                            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center } ));
                }
                else
                {
                    tc.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                }
                tr.Append(tc);
            }
            table.Append(tr);
        }

        public static void AddVersionControl(WordprocessingDocument doc, ControllerDataModel data)
        {
            AddText(doc, $"{Texts["Ch0Versioning"]}", "Subtitle");

            Table table = new Table();

            TableProperties props = new TableProperties(
                new TableBorders(
                new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 }), 
                new TableWidth() { Type = TableWidthUnitValues.Pct, Width = $"{100 * 50}" } );

            table.AppendChild(props);

            foreach(var v in data.Versies)
            {
                AddRowToTable(
                    doc,
                    table, 
                    new[]{ v.Datum.ToShortDateString(), v.Versie, v.Ontwerper, v.Commentaar },
                    new[]{ 14, 14, 14, 58 });
            }

            doc.MainDocumentPart.Document.Body.Append(new Paragraph(new Run(table)));
        }

        public static void AddHeaderTexts(WordprocessingDocument doc, SpecificatorDataModel model, ControllerDataModel data)
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
                        if (!string.IsNullOrWhiteSpace(model.Postcode))
                        {
                            r.AppendChild(new Text($"{model.Postcode} "));
                        }
                        if (!string.IsNullOrWhiteSpace(model.Stad))
                        {
                            r.AppendChild(new Text(model.Stad));
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

        public static void AddChapterTitle(WordprocessingDocument doc, string title, int headingLevel)
        {
            Paragraph par = doc.MainDocumentPart.Document.Body.AppendChild(new Paragraph());
            Run run = par.AppendChild(new Run());
            run.AppendChild(new Text(title));
            ApplyStyleToParagraph(doc, par, styleid: $"Heading{headingLevel}");
        }

        public static Paragraph AddText(WordprocessingDocument doc, string text, string stylename)
        {
            Paragraph par = doc.MainDocumentPart.Document.Body.AppendChild(new Paragraph());
            Run run = par.AppendChild(new Run());
            run.AppendChild(new Text(text));
            ApplyStyleToParagraph(doc, par, stylename: stylename);
            return par;
        }

        public static void AddStylesToDocument(WordprocessingDocument doc)
        {
            // Get the Styles part for this document.
            StyleDefinitionsPart part =
                doc.MainDocumentPart.StyleDefinitionsPart;

            // If the Styles part does not exist, add it and then add the style.
            if (part == null)
            {
                part = AddStylesPartToPackage(doc);
            }

            CreateAndAddParagraphStyle(part, "Body", "Body", 12);
            CreateAndAddParagraphStyle(part, "Title", "Title", 24, justification: JustificationValues.Center);
            CreateAndAddParagraphStyle(part, "Subtitle", "Subtitle", 18, justification: JustificationValues.Center);
            CreateAndAddParagraphStyle(part, "Heading", "Heading", 12, bold: true);
            CreateAndAddParagraphStyle(part, "Heading1", "Heading 1", 16, numId: 1);
            CreateAndAddParagraphStyle(part, "Heading2", "Heading 2", 14, numId: 2);
            CreateAndAddParagraphStyle(part, "Heading3", "Heading 3", 12, numId: 3);
            CreateAndAddParagraphStyle(part, "Heading4", "Heading 4", 12, italic: true, bold: false, numId: 4);
            CreateAndAddParagraphStyle(part, "TableContents", "Table Contents", 12);
        }

        public static void CreateAndAddParagraphStyle(
            StyleDefinitionsPart styleDefinitionsPart,
            string styleid, 
            string stylename, 
            int fontSize,
            string basedOn = "Normal",
            bool bold = false,
            bool italic = false,
            JustificationValues justification = JustificationValues.Left,
            string nextPar = "Body",
            string aliases = "",
            int numId = 0)
        {
            // Access the root element of the styles part.
            Styles styles = styleDefinitionsPart.Styles;
            if (styles == null)
            {
                styleDefinitionsPart.Styles = new Styles();
                styleDefinitionsPart.Styles.Save();
            }

            // Create a new paragraph style element and specify some of the attributes.
            Style style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = styleid,
                CustomStyle = true,
                Default = false
            };

            // Create and add the child elements (properties of the style).
            if (!string.IsNullOrWhiteSpace(aliases)) style.Append(new Aliases() { Val = aliases });
            if (!string.IsNullOrWhiteSpace(basedOn)) style.Append(new BasedOn() { Val = basedOn });
            style.Append(new NextParagraphStyle() { Val = nextPar });
            style.Append(new AutoRedefine() { Val = OnOffOnlyValues.Off });
            //style.Append(new LinkedStyle() { Val = "OverdueAmountChar" });
            style.Append(new Locked() { Val = OnOffOnlyValues.Off });
            style.Append(new PrimaryStyle() { Val = OnOffOnlyValues.On });
            style.Append(new StyleHidden() { Val = OnOffOnlyValues.Off });
            style.Append(new SemiHidden() { Val = OnOffOnlyValues.Off });
            style.Append(new StyleName() { Val = stylename });
            style.Append(new UIPriority() { Val = 1 });
            style.Append(new UnhideWhenUsed() { Val = OnOffOnlyValues.On });

            // Create the StyleRunProperties object and specify some of the run properties.
            var styleRunProperties1 = new StyleRunProperties();
            if (bold) styleRunProperties1.Append(new Bold());
            //styleRunProperties1.Append(new Color() { ThemeColor = ThemeColorValues.Accent2 });
            styleRunProperties1.Append(new RunFonts() { Ascii = "Open Sans" });
            styleRunProperties1.Append(new FontSize() { Val = (fontSize * 2).ToString() });
            if (italic) styleRunProperties1.Append(new Italic());

            var parProps1 = new ParagraphProperties();
            parProps1.Append(new Justification { Val = justification });
            style.Append(parProps1);

            if (numId != 0)
            {
                var numbProps1 = new NumberingProperties();
                numbProps1.Append(new NumberingId() { Val = numId });
                style.Append(numbProps1);
            }

            // Add the run properties to the style.
            style.Append(styleRunProperties1);

            // Add the style to the styles part.
            styles.Append(style);
        }

        // Add a StylesDefinitionsPart to the document.  Returns a reference to it.
        public static StyleDefinitionsPart AddStylesPartToPackage(WordprocessingDocument doc)
        {
            StyleDefinitionsPart part;
            part = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
            Styles root = new Styles();
            root.Save(part);
            return part;
        }

        public static void GenerateSpecification(string filename, ControllerModel c, SpecificatorDataModel model)
        {
            using (var doc = WordprocessingDocument.Open(filename, true))
            {
                // Add a main document part. 
                Body body = doc.MainDocumentPart.Document.Body;
                body.RemoveAllChildren();

                //AddStylesToDocument(doc);
                AddHeaderTexts(doc, model, c.Data);
                AddFirstPage(doc, c.Data);
                AddVersionControl(doc, c.Data);

                AddChapterTitle(doc, $"{Texts["Ch1Intro"]}", 1);
                var ch1t = Regex.Replace((string)Texts["Ch1_Text"], "__KR__", c.Data.Naam);
                AddText(doc, $"{ch1t}", stylename: "Body");
                AddChapterTitle(doc, $"{Texts["Ch2Functionality"]}", 1);
                AddChapterTitle(doc, $"{Texts["Ch2P1Intergreen"]}", 2);

                //DocumentSettingsPart settingsPart = doc.MainDocumentPart.GetPartsOfType<DocumentSettingsPart>().First();
                //UpdateFieldsOnOpen updateFields = new UpdateFieldsOnOpen();
                //updateFields.Val = new DocumentFormat.OpenXml.OnOffValue(true);
                //settingsPart.Settings.PrependChild<UpdateFieldsOnOpen>(updateFields);
                //settingsPart.Settings.Save();
            }

            // Create a new document.
            //Paragraph par;
            //Paragraph par1;
            //Table table;
            //
            //using (var doc = Xceed.Words.NET.DocX.Load(filename))
            //{
            //    var par1 = doc.Paragraphs.First(x => x.StyleName == "Heading1");
            //    var toc = doc.InsertTableOfContents(par1, "Inhoudsopgave",
            //        Xceed.Words.NET.TableOfContentsSwitches.O |
            //        Xceed.Words.NET.TableOfContentsSwitches.U |
            //        Xceed.Words.NET.TableOfContentsSwitches.Z |
            //        Xceed.Words.NET.TableOfContentsSwitches.H);
            //    doc.Save();
            //}
            //    // Header
            //    doc.AddHeaders();
            //    doc.DifferentFirstPage = true;
            //    par = doc.Headers.Odd.InsertParagraph($"Functionele specificatie {c.Data.Naam} ({c.Data.Straat1}{(!string.IsNullOrWhiteSpace(c.Data.Straat2) ? " - " + c.Data.Straat2 : "")}, {c.Data.Stad})");
            //
            //    // Footer
            //    doc.AddFooters();
            //    par = doc.Footers.First.InsertParagraph();
            //    if (!string.IsNullOrWhiteSpace(model.Organisatie)) par.AppendLine($"{model.Organisatie}");
            //    if (!string.IsNullOrWhiteSpace(model.Straat)) par.AppendLine($"{model.Straat}");
            //    if (!string.IsNullOrWhiteSpace(model.Postcode)) par.Append($"{model.Postcode} ");
            //    if (!string.IsNullOrWhiteSpace(model.Stad)) par.Append($"{model.Stad}");
            //    if (!string.IsNullOrWhiteSpace(model.TelefoonNummer)) par.AppendLine($"Telefoon: {model.TelefoonNummer}");
            //    if (!string.IsNullOrWhiteSpace(model.EMail)) par.AppendLine($"E-mail: {model.EMail}");
            //
            //    // Front page
            //    par = doc.InsertParagraph($"Functionele specificatie {c.Data.Naam}");
            //    par.StyleName = "Title";
            //    par.Alignment = Alignment.center;
            //    par = doc.InsertParagraph($"{c.Data.Straat1}{(!string.IsNullOrWhiteSpace(c.Data.Straat2) ? " - " + c.Data.Straat2 : "")}");
            //    par.Alignment = Alignment.center;
            //    par = doc.InsertParagraph($"{c.Data.Stad})");
            //    par.Alignment = Alignment.center;
            //    par.InsertPageBreakAfterSelf();
            //
            //    // Version page
            //    par1 = doc.InsertParagraph($"Versie beheer");
            //    par1.StyleName = "Heading1";
            //    table = doc.AddTable(c.Data.Versies.Count + 1, 4);
            //    table.Rows[0].Cells[0].Paragraphs.First().Append("Versie");
            //    table.Rows[0].Cells[1].Paragraphs.First().Append("Datum");
            //    table.Rows[0].Cells[2].Paragraphs.First().Append("Ontwerper");
            //    table.Rows[0].Cells[3].Paragraphs.First().Append("Commentaar");
            //    for (var i = 0; i < c.Data.Versies.Count; ++i)
            //    {
            //        table.Rows[i + 1].Cells[0].Paragraphs.First().Append(c.Data.Versies[i].Versie);
            //        table.Rows[i + 1].Cells[1].Paragraphs.First().Append(c.Data.Versies[i].Datum.ToShortDateString());
            //        table.Rows[i + 1].Cells[2].Paragraphs.First().Append(c.Data.Versies[i].Ontwerper);
            //        table.Rows[i + 1].Cells[3].Paragraphs.First().Append(c.Data.Versies[i].Commentaar);
            //    }
            //    table.Design = TableDesign.LightGrid;
            //    doc.InsertTable(table);
            //    doc.InsertParagraph().InsertPageBreakAfterSelf();
            //
            //    // Fasen
            //    doc.InsertParagraph("Fasen").StyleName = "Heading1";
            //    table = doc.AddTable(c.Fasen.Count + 1, 2);
            //    table.Rows[0].Cells[0].Paragraphs.First().Append("Naam");
            //    table.Rows[0].Cells[1].Paragraphs.First().Append("Type");
            //    for (var i = 0; i < c.Fasen.Count; ++i)
            //    {
            //        table.Rows[i + 1].Cells[0].Paragraphs.First().Append(c.Fasen[i].Naam);
            //        table.Rows[i + 1].Cells[1].Paragraphs.First().Append(c.Fasen[i].Type.GetDescription());
            //    }
            //    table.Design = TableDesign.LightGrid;
            //    doc.InsertTable(table);
            //
            //    // TOC page
            //    var toc = doc.InsertTableOfContents(par1, "Inhoudsopgave", 
            //        TableOfContentsSwitches.O | TableOfContentsSwitches.U | TableOfContentsSwitches.Z | TableOfContentsSwitches.H);
            //    
            //    doc.Save();
            //}
        }
    }
}
