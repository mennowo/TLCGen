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

        public static string ReplaceHolders(string text, ControllerModel c, SpecificatorDataModel model)
        {
            var t = text;
            t = Regex.Replace(t, "__KR__", c.Data.Naam);
            t = Regex.Replace(t, "__STAD__", c.Data.Stad);
            t = Regex.Replace(t, "__STRAAT1__", c.Data.Straat1);
            t = Regex.Replace(t, "__STRAAT2__", c.Data.Straat2);
            return t;
        }

        public static void SetDirtyFlag(WordprocessingDocument doc)
        {
            DocumentSettingsPart settingsPart = doc.MainDocumentPart.GetPartsOfType<DocumentSettingsPart>().First();
            UpdateFieldsOnOpen updateFields = new UpdateFieldsOnOpen();
            updateFields.Val = new OnOffValue(true);
            settingsPart.Settings.PrependChild<UpdateFieldsOnOpen>(updateFields);
            settingsPart.Settings.Save();
        }

        public static void GenerateSpecification(string filename, ControllerModel c, SpecificatorDataModel model)
        {
            using (var doc = WordprocessingDocument.Open(filename, true))
            {
                // Add a main document part. 
                Body body = doc.MainDocumentPart.Document.Body;
                body.RemoveAllChildren<Paragraph>();

                //AddStylesToDocument(doc);
                AddHeaderTexts(doc, model, c.Data);
                AddFirstPage(doc, c.Data);
                AddVersionControl(doc, c.Data);

                AddChapterTitle(doc, $"{Texts["Ch1Intro"]}", 1);
                AddText(doc, ReplaceHolders((string)Texts["Ch1_Text"], c, model), stylename: "Body");

                AddChapterTitle(doc, $"{Texts["Ch2Functionality"]}", 1);
                AddChapterTitle(doc, $"{Texts["Ch2P1Intergreen"]}", 2);
            }
        }
    }
}
