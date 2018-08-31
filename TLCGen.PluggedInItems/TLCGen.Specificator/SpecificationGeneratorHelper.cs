using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using Style = DocumentFormat.OpenXml.Wordprocessing.Style;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

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

        public static void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 990000L, Cy = 792000L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            // Append the reference to body, the element should be in a Run.
            wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
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
