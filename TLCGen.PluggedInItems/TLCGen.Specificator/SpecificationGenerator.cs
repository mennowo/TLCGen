using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Models;
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

        //public static void AddSettingsTable(WordprocessingDocument doc, List<CCOLElement> elements)
        //{
        //    Table table = new Table();
        //
        //    TableProperties props = new TableProperties(
        //        new TableBorders(
        //        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
        //        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
        //        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
        //        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
        //        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
        //        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 }),
        //        new TableWidth() { Type = TableWidthUnitValues.Pct, Width = $"{100 * 50}" });
        //
        //    table.AppendChild(props);
        //
        //    AddRowToTable(
        //            table,
        //            new[] { "Type", "Naam", "Instelling", "Commentaar" },
        //            new[] { 14, 14, 14, 58 });
        //
        //    foreach (var e in elements)
        //    {
        //        AddRowToTable(
        //            table,
        //            new[] { e.Type.ToString(), e.Naam, e.Instelling.ToString(), e.Commentaar },
        //            new[] { 14, 14, 14, 58 });
        //    }
        //
        //    doc.MainDocumentPart.Document.Body.Append(new Paragraph(new Run(table)));
        //}

        
        

        

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

                //var gensWithElems = CCOLGenerator.GetAllGeneratorsWithElements(c);

                // Headers, title page, versioning
                FunctionalityGenerator.AddHeaderTextsToDocument(doc, model, c.Data);
                body.Append(FunctionalityGenerator.GetFirstPage(c.Data));
                body.Append(FunctionalityGenerator.GetVersionControl(c.Data));

                // Introduction
                
                // Content
                body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Functionality"]}", 1));

                body.Append(FunctionalityGenerator.GetIntroChapter(doc, c, model));

                body.Append(FunctionalityGenerator.GetFasenChapter(c));
                body.Append(FunctionalityGenerator.GetDetectorenChapter(c));
                body.Append(FunctionalityGenerator.GetRichtingGevoeligChapter(c));
                body.Append(FunctionalityGenerator.GetPeriodenChapter(c));
                body.Append(FunctionalityGenerator.GetGroentijdenChapter(c));
                
                
                //foreach (var g in gensWithElems)
                //{
                //    if (g.Item2.Any())
                //    {
                //        AddChapterTitle(doc, $"{g.Item1.GetType().Name.ToString()}", 2);
                //        AddSettingsTable(doc, g.Item2);
                //    }
                //}
            }
        }
    }
}
