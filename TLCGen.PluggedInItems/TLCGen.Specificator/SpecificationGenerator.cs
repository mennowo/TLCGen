using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using TLCGen.Extensions;
using TLCGen.Models;
using Style = DocumentFormat.OpenXml.Wordprocessing.Style;

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
            var par = (Paragraph)doc.MainDocumentPart.Document.Body.ChildElements.First(x => x.InnerText.StartsWith("Title"));
            par.RemoveAllChildren();
            var run = par.AppendChild(new Run());

            run.AppendChild(new Text($"{Texts["Title"]} {data.Naam}"));
            ApplyStyleToParagraph(doc, par, "Title");

            par = (Paragraph)doc.MainDocumentPart.Document.Body.ChildElements.First(x => x.InnerText.StartsWith("Subtitle"));
            par.RemoveAllChildren();
            run = par.AppendChild(new Run());
            run.AppendChild(new Text($"{data.Straat1}{(!string.IsNullOrWhiteSpace(data.Straat2) ? " - " + data.Straat2 : "")}"));
            ApplyStyleToParagraph(doc, par, "Subtitle");

            var par2 = par.InsertAfterSelf(new Paragraph());
            run = par2.AppendChild(new Run());
            run.AppendChild(new Text($"{data.Stad})"));
            ApplyStyleToParagraph(doc, par2, "Subtitle");
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
            ApplyStyleToParagraph(doc, par, $"Heading {headingLevel}");
        }

        public static void GenerateSpecification(string filename, ControllerModel c, SpecificatorDataModel model)
        {
            using (var doc = WordprocessingDocument.Open(filename, true))
            {
                Body body = doc.MainDocumentPart.Document.Body;

                var remPars = new List<Paragraph>();
                foreach(var ch in body.ChildElements)
                {
                    if(ch is Paragraph p)
                    {
                        if (p.InnerText.StartsWith("__")) remPars.Add(p);
                    }
                }

                foreach(var p in remPars)
                {
                    body.RemoveChild(p);
                }

                AddHeaderTexts(doc, model, c.Data);
                AddFirstPage(doc, c.Data);

                AddChapterTitle(doc, $"{Texts["Ch1Title"]}", 1);
                AddChapterTitle(doc, $"{Texts["Ch2Functionality"]}", 1);
                AddChapterTitle(doc, $"{Texts["Ch2P1Intergreen"]}", 2);

                DocumentSettingsPart settingsPart = doc.MainDocumentPart.GetPartsOfType<DocumentSettingsPart>().First();
                UpdateFieldsOnOpen updateFields = new UpdateFieldsOnOpen();
                updateFields.Val = new DocumentFormat.OpenXml.OnOffValue(true);
                settingsPart.Settings.PrependChild<UpdateFieldsOnOpen>(updateFields);
                settingsPart.Settings.Save();
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
