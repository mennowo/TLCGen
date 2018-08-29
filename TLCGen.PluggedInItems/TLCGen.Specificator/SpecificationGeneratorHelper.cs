using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using Style = DocumentFormat.OpenXml.Wordprocessing.Style;

namespace TLCGen.Specificator
{
    public static partial class OpenXmlHelper
    {
        public static string GetStyleIdFromStyleName(WordprocessingDocument doc, string styleName)
        {
            StyleDefinitionsPart stylePart = doc.MainDocumentPart.StyleDefinitionsPart;
            string styleId = stylePart.Styles.Descendants<StyleName>()
                .Where(s => s.Val.Value.Equals(styleName) &&
                    (((Style)s.Parent).Type == StyleValues.Paragraph))
                .Select(n => ((Style)n.Parent).StyleId).FirstOrDefault();
            return styleId;
        }

        public static void ApplyStyleToParagraph(Paragraph p, string styleid)
        {
            // If the paragraph has no ParagraphProperties object, create one.
            if (p.Elements<ParagraphProperties>().Count() == 0)
            {
                p.PrependChild(new ParagraphProperties());
            }

            // Get the paragraph properties element of the paragraph.
            ParagraphProperties pPr = p.Elements<ParagraphProperties>().First();

            pPr.ParagraphStyleId = new ParagraphStyleId() { Val = styleid };
        }

        public static TableRow GetTableRow(string[] data, int[] widths = null, bool header = false, bool verticalText = false)
        {
            var tr = new TableRow();
            for (int i = 0; i < data.Length; i++)
            {
                string d = data[i];
                var tc = new TableCell();
                var run = new Run(new Text(d));
                if (header) run.RunProperties = new RunProperties(new Bold());
                var par = new Paragraph(run);
                
                ApplyStyleToParagraph(par, "TableContents");
                if (widths != null && widths.Length == data.Length)
                {
                    var tabProps = 
                        new TableCellProperties(
                            new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = $"{widths[i] * 50 }" },
                            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
                    if (verticalText)
                    {
                        tabProps.AppendChild(new TextDirection() { Val = TextDirectionValues.BottomToTopLeftToRight });
                    }
                    tc.Append(tabProps);
                }
                else
                {
                    var tabProps = new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto });
                    if (verticalText)
                    {
                        tabProps.AppendChild(new TextDirection() { Val = TextDirectionValues.BottomToTopLeftToRight });
                        tabProps.AppendChild(new TableRowHeight() { Val = Convert.ToUInt32("1500") });
                    }
                    tc.Append(tabProps);
                }
                tc.Append(par);
                tr.Append(tc);
            }
            return tr;
        }

        public static Paragraph GetTextParagraph(string text, string styleid = "Normal")
        {
            var par = new Paragraph();
            var run = par.AppendChild(new Run());
            run.AppendChild(new Text(text));
            ApplyStyleToParagraph(par, styleid: styleid);
            return par;
        }

        public static Paragraph GetChapterTitleParagraph(string title, int headingLevel)
        {
            var par = new Paragraph();
            var run = par.AppendChild(new Run());
            run.AppendChild(new Text(title));
            ApplyStyleToParagraph(par, $"Heading{headingLevel}");
            return par;
        }

        public static Table GetTable(List<List<string>> elements, bool firstRowVerticalText = false)
        {
            Table table = new Table();

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

            var first = true;
            foreach (var e1 in elements)
            {
                var items = new List<string>();
                foreach (var e2 in e1)
                {
                    items.Add(e2);
                }
                var row = GetTableRow(items.ToArray(), header: first, verticalText: firstRowVerticalText && first);
                if (first)
                {
                    if(row.TableRowProperties == null)
                    {
                        row.TableRowProperties = new TableRowProperties();
                    }
                    row.TableRowProperties.AppendChild(new TableHeader());
                }
                table.Append(row);
                first = false;
            }

            return table;
        }
    }
}
