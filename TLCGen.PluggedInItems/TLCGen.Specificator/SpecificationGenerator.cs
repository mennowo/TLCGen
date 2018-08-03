using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TLCGen.Extensions;
using TLCGen.Models;

namespace TLCGen.Specificator
{
    public static class SpecificationGenerator
    {


        // Return styleid that matches the styleName, or null when there's no match.
        public static string GetStyleIdFromStyleName(WordprocessingDocument doc, string styleName)
        {
            StyleDefinitionsPart stylePart = doc.MainDocumentPart.StyleDefinitionsPart;
            string styleId = stylePart.Styles.Descendants<StyleName>()
                .Where(s => s.Val.Value.Equals(styleName) &&
                    (((Style)s.Parent).Type == StyleValues.Paragraph))
                .Select(n => ((Style)n.Parent).StyleId).FirstOrDefault();
            return styleId;
        }

        // Apply a style to a paragraph.
        public static void ApplyStyleToParagraph(WordprocessingDocument doc, Paragraph p, string stylename)
        {
            // If the paragraph has no ParagraphProperties object, create one.
            if (p.Elements<ParagraphProperties>().Count() == 0)
            {
                p.PrependChild<ParagraphProperties>(new ParagraphProperties());
            }

            // Get the paragraph properties element of the paragraph.
            ParagraphProperties pPr = p.Elements<ParagraphProperties>().First();

            // Set the style of the paragraph.
            pPr.ParagraphStyleId = new ParagraphStyleId() { Val = GetStyleIdFromStyleName(doc, stylename) };
        }

        public static void GenerateSpecification(string filename, ControllerModel c, SpecificatorDataModel model)
        {
            // Open a WordprocessingDocument for editing using the filepath.
            using (var doc = WordprocessingDocument.Open(filename, true))
            {
                // Assign a reference to the existing document body.
                Body body = doc.MainDocumentPart.Document.Body;
                body.RemoveAllChildren<Paragraph>();

                // Front page
                Paragraph par = body.AppendChild(new Paragraph());
                Run run = par.AppendChild(new Run());
                run.AppendChild(new Text($"Functionele specificatie {c.Data.Naam}"));
                ApplyStyleToParagraph(doc, par, "Title");

                par = body.AppendChild(new Paragraph());
                run = par.AppendChild(new Run());
                run.AppendChild(new Text($"{c.Data.Straat1}{(!string.IsNullOrWhiteSpace(c.Data.Straat2) ? " - " + c.Data.Straat2 : "")}"));
                ApplyStyleToParagraph(doc, par, "Subtitle");

                par = body.AppendChild(new Paragraph());
                run = par.AppendChild(new Run());
                run.AppendChild(new Text($"{c.Data.Stad})"));
                ApplyStyleToParagraph(doc, par, "Subtitle");

                foreach (var h in doc.MainDocumentPart.HeaderParts)
                {
                    foreach (var t in h.RootElement.Descendants<Paragraph>().SelectMany(x => x.Descendants<Run>()).SelectMany(x => x.Descendants<Text>()))
                    {
                        if (t.Text.Contains("FIRSTPAGEHEADER")) t.Text = "";
                        if (t.Text.Contains("HEADER")) t.Text = $"Functionele specificatie {c.Data.Naam} ({c.Data.Straat1}{(!string.IsNullOrWhiteSpace(c.Data.Straat2) ? " - " + c.Data.Straat2 : "")}, {c.Data.Stad})";
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

                Paragraph parToc2 = body.AppendChild(new Paragraph());
                Run runTco2 = parToc2.AppendChild(new Run());
                runTco2.AppendChild(new Text($"Hoofdstuk 1"));
                ApplyStyleToParagraph(doc, parToc2, "Heading 1");

                Paragraph parToc = body.AppendChild(new Paragraph());
                Run runTco = parToc.AppendChild(new Run());
                runTco.AppendChild(new Text($"Hoofdstuk 2"));
                ApplyStyleToParagraph(doc, parToc, "Heading 1");

                // DOES NOT WORK!!!
                XElement firstPara = doc
                    .MainDocumentPart
                    .GetXDocument()
                    .Descendants(W.p)
                    .FirstOrDefault();
                TocAdder.AddToc(doc, firstPara, @"TOC \o '1-3' \h \z \u", "Test", null);
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
