using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
using Style = DocumentFormat.OpenXml.Wordprocessing.Style;

namespace TLCGen.Specificator
{
    public static partial class SpecificationGenerator
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

    }
}
