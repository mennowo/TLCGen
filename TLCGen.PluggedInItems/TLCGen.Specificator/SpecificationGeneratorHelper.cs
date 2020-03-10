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
using System.Windows.Media.Imaging;
using System.IO;

namespace TLCGen.Specificator
{
    public static partial class OpenXmlHelper
    {
        public static string GetStyleIdFromStyleName(WordprocessingDocument doc, string styleName)
        {
            var stylePart = doc.MainDocumentPart.StyleDefinitionsPart;
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
            var pPr = p.Elements<ParagraphProperties>().First();

            pPr.ParagraphStyleId = new ParagraphStyleId() { Val = styleid };
        }

        public static NumberingDefinitionsPart GetOrCreateNumberingDefinitionsPart(WordprocessingDocument doc)
        {
            // Introduce bulleted numbering in case it will be needed at some point
            var numberingPart = doc.MainDocumentPart.NumberingDefinitionsPart;
            if (numberingPart == null)
            {
                numberingPart = doc.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>("numbDef001");
                var element = new Numbering();
                element.Save(numberingPart);
            }
            return numberingPart;
        }

        public static List<Paragraph> GetBulletList(WordprocessingDocument doc, List<Tuple<string, int>> list)
        {
            var items = new List<Paragraph>();

            var numberingPart = GetOrCreateNumberingDefinitionsPart(doc);

            // Insert an AbstractNum into the numbering part numbering list.  The order seems to matter or it will not pass the 
            // Open XML SDK Productity Tools validation test.  AbstractNum comes first and then NumberingInstance and we want to
            // insert this AFTER the last AbstractNum and BEFORE the first NumberingInstance or we will get a validation error.
            var abstractNumberId = numberingPart.Numbering.Elements<AbstractNum>().Count() + 1;
            var abstractLevel1 = new Level(new NumberingFormat() { Val = NumberFormatValues.Bullet }, new LevelText() { Val = "·" }) { LevelIndex = 0 };
            var abstractLevel2 = new Level(new NumberingFormat() { Val = NumberFormatValues.Bullet }, new LevelText() { Val = "o" }) { LevelIndex = 1 };
            var abstractLevel3 = new Level(new NumberingFormat() { Val = NumberFormatValues.Bullet }, new LevelText() { Val = "·" }) { LevelIndex = 2 };
            var abstractLevel4 = new Level(new NumberingFormat() { Val = NumberFormatValues.Bullet }, new LevelText() { Val = "o" }) { LevelIndex = 3 };
            var ml = new MultiLevelType() { Val = MultiLevelValues.HybridMultilevel };
            var abstractNum1 = new AbstractNum(ml)
            {
                AbstractNumberId = abstractNumberId
            };
            abstractNum1.Append(abstractLevel1);
            abstractNum1.Append(abstractLevel2);
            abstractNum1.Append(abstractLevel3);
            abstractNum1.Append(abstractLevel4);

            if (abstractNumberId == 1)
            {
                numberingPart.Numbering.Append(abstractNum1);
            }
            else
            {
                var lastAbstractNum = numberingPart.Numbering.Elements<AbstractNum>().Last();
                numberingPart.Numbering.InsertAfter(abstractNum1, lastAbstractNum);
            }

            // Insert an NumberingInstance into the numbering part numbering list.  The order seems to matter or it will not pass the 
            // Open XML SDK Productity Tools validation test.  AbstractNum comes first and then NumberingInstance and we want to
            // insert this AFTER the last NumberingInstance and AFTER all the AbstractNum entries or we will get a validation error.
            var numberId = numberingPart.Numbering.Elements<NumberingInstance>().Count() + 1;
            var numberingInstance1 = new NumberingInstance() { NumberID = numberId };
            var abstractNumId1 = new AbstractNumId() { Val = abstractNumberId };
            numberingInstance1.Append(abstractNumId1);

            if (numberId == 1)
            {
                numberingPart.Numbering.Append(numberingInstance1);
            }
            else
            {
                var lastNumberingInstance = numberingPart.Numbering.Elements<NumberingInstance>().Last();
                numberingPart.Numbering.InsertAfter(numberingInstance1, lastNumberingInstance);
            }

            var body = doc.MainDocumentPart.Document.Body;

            foreach (var item in list)
            {
                // Create items for paragraph properties
                var numberingProperties = new NumberingProperties(new NumberingLevelReference() { Val = item.Item2 }, new NumberingId() { Val = numberId });
                var spacingBetweenLines1 = new SpacingBetweenLines() { After = "0" };  // Get rid of space between bullets
                var indentation = new Indentation()
                {
                    Left = (720 + 360 * item.Item2).ToString(),
                    Hanging = (360).ToString(),
                };  // correct indentation 

                var paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
                if (item.Item2 % 2 == 0)
                {
                    var runFonts1 = new RunFonts() { Ascii = "Symbol", HighAnsi = "Symbol" };
                    paragraphMarkRunProperties1.Append(runFonts1);
                }
                else
                {
                    var runFonts1 = new RunFonts() { Ascii = "Courier New", HighAnsi = "Courier New" };
                    paragraphMarkRunProperties1.Append(runFonts1);
                }

                // create paragraph properties
                var paragraphProperties = new ParagraphProperties(numberingProperties, spacingBetweenLines1, indentation, paragraphMarkRunProperties1);

                // Create paragraph 
                var newPara = new Paragraph(paragraphProperties);
                
                // Add run to the paragraph
                newPara.AppendChild(new Run(new Text(item.Item1)));

                // Add one bullet item to the body
                items.Add(newPara);
            }

            return items;
        }


        public static void AddImageToBody(WordprocessingDocument doc, string file)
        {
            var imagePart = doc.MainDocumentPart.AddImagePart(ImagePartType.Bmp);

            var img = new BitmapImage();
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                img.BeginInit();
                img.StreamSource = fs;
                img.EndInit();
            }
            var widthPx = img.PixelWidth;
            var heightPx = img.PixelHeight;
            var horzRezDpi = img.DpiX;
            var vertRezDpi = img.DpiY;
            const int emusPerInch = 914400;
            const int emusPerCm = 360000;

            var widthEmus = (long)(widthPx / horzRezDpi * emusPerInch);
            var heightEmus = (long)(heightPx / vertRezDpi * emusPerInch);
            var maxWidthEmus = (long)(15.75 * emusPerCm);
            if (widthEmus > maxWidthEmus)
            {
                var ratio = (heightEmus * 1.0m) / widthEmus;
                widthEmus = maxWidthEmus;
                heightEmus = (long)(widthEmus * ratio);
            }

            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = widthEmus, Cy = heightEmus },
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
                                             Name = Path.GetFileName(file)
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                       Guid.NewGuid().ToString()
                                                 })
                                         )
                                         {
                                             Embed = doc.MainDocumentPart.GetIdOfPart(imagePart),
                                             CompressionState = A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = widthEmus, Cy = heightEmus }),
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


            using (var stream = new FileStream(file, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }

            doc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }

        public static TableRow GetTableRow(string[] data, int[] widths = null, bool header = false, bool verticalText = false)
        {
            var tr = new TableRow();
            for (var i = 0; i < data.Length; i++)
            {
                var d = data[i];
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

        public static Paragraph GetTextParagraph(string text, string styleid = "Normal", bool bold = false)
        {
            var par = new Paragraph();
            var run = par.AppendChild(new Run());
            if (bold)
            {
                var runProperties = run.AppendChild(new RunProperties());
                var b = new Bold();
                runProperties.AppendChild(b);
            }
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
