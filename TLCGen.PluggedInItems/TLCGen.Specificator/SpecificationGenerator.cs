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

                
                // Chap 1: Introduction
                FunctionalityGenerator.GetIntroChapter(doc, c, model);

                // Chap 2: Structuur en afwikkeling
                body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_StructuurEnAfwikkeling"]}", 1));
                body.Append(FunctionalityGenerator.GetChapter_StructureIntroduction(c));
                body.Append(FunctionalityGenerator.GetChapter_Perioden(c));
                body.Append(FunctionalityGenerator.GetChapter_SignaalGroepAfhandeling(doc, c));
                body.Append(FunctionalityGenerator.GetChapter_SignaalGroepInstellingen(doc, c));
                body.Append(FunctionalityGenerator.GetChapter_Groentijden(c));
                body.Append(FunctionalityGenerator.GetChapter_Modulestructuur(doc, c));

                // Chap 3: Detectoren
                body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren"]}", 1));
                body.Append(FunctionalityGenerator.GetChapter_DetectieConfiguratie(doc, c));
                body.Append(FunctionalityGenerator.GetChapter_DetectieInstellingen(doc, c));
                body.Append(FunctionalityGenerator.GetChapter_DetectieRichtingGevoelig(c));
                body.Append(FunctionalityGenerator.GetChapter_DetectieStoring(doc, c));

                // Chap 4: Intersignaalgroep
                body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Intersignaalgroep"]}", 1));
                body.Append(FunctionalityGenerator.GetChapter_Ontruimingstijden(c));
                body.Append(FunctionalityGenerator.GetChapter_OntruimingstijdenGarantie(c));
                body.Append(FunctionalityGenerator.GetChapter_Synchronisaties(c, doc));

                if (c.OVData.OVIngreepType != Models.Enumerations.OVIngreepTypeEnum.Geen && c.HasPTorHD())
                {
                    if (c.HasPT() && c.HasHD())
                    {
                        body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OVHD"]}", 1));
                        body.Append(FunctionalityGenerator.GetChapter_OVHDIntro(c));
                    }
                    if (c.HasPT())
                    {
                        body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_OV"]}", c.HasHD() ? 2 : 1));
                        body.Append(FunctionalityGenerator.GetChapter_OV(c, doc, c.HasHD() ? 2 : 1));
                    }
                    if (c.HasHD())
                    {
                        body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_HD"]}", c.HasPT() ? 2 : 1));
                        body.Append(FunctionalityGenerator.GetChapter_HD(c, doc, c.HasHD() ? 2 : 1));
                    }
                }

                if (c.HalfstarData.IsHalfstar)
                {
                    body.Append(OpenXmlHelper.GetChapterTitleParagraph($"Title_Halfstar", 1));
                    body.Append((OpenXmlHelper.GetTextParagraph($"TODO: Hoofdstuk inzake halfstar regelen.", "TODO")));

                }
                body.Append(OpenXmlHelper.GetChapterTitleParagraph($"TODO", 1));
                body.Append((OpenXmlHelper.GetTextParagraph($"TODO: Hoofdstuk OV: details toevoegen, zoals: " +
                    $"lijnnummers, details rond in/uitmelden, inmelden koplus, .", "TODO")));
                body.Append((OpenXmlHelper.GetTextParagraph($"TODO: Overige punten, zoals: PTP, VA ontruimen, file ingrepen, rateltikkers, " +
                    $"ingangen, selectieve detectie, uitgestelde vaste aanvragen, hard meeverlengen, veiligheidsgroen, RoBuGrover, special van plugins zoals AFM, etc.", "TODO")));
                body.Append((OpenXmlHelper.GetTextParagraph($"TODO: Opnemen verwijzingen naar CCOL benaming van instellingen (?).", "TODO")));
                
                // TODO: signaalplannen

                // dan: afhandeling signaalgroepen (nalopen, gelijkstarten, etc)
                // tabel functies hiernaartoe verplaatsen (?)

                // dan: OV, HD

                // dan: overig: fixatie -> onderbrengen in H1 (intro) bij algemene instellingen

                //body.Append(FunctionalityGenerator.GetModulenChapter(doc, c));

                doc.Close();
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
