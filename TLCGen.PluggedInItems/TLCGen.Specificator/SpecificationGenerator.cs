using System;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TLCGen.Models;
using TLCGen.Plugins;

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
        public static void SetDirtyFlag(WordprocessingDocument doc)
        {
            var settingsPart = doc.MainDocumentPart.GetPartsOfType<DocumentSettingsPart>().First();
            var updateFields = new UpdateFieldsOnOpen();
            updateFields.Val = new OnOffValue(true);
            settingsPart.Settings.PrependChild<UpdateFieldsOnOpen>(updateFields);
            settingsPart.Settings.Save();
        }

        private static void AppendGenericSpecificationData(WordprocessingDocument doc, IEnumerable<SpecificationData> pluginData)
        {
            var body = doc.MainDocumentPart.Document.Body;

            foreach (var specData in pluginData.Where(x => (x.Subject == SpecificationSubject.DynHiaat)
                                                        || (x.Subject == SpecificationSubject.AFM)
                                                        || (x.Subject == SpecificationSubject.GebruikersPlugin1)
                                                        || (x.Subject == SpecificationSubject.GebruikersPlugin2)
                                                        ))
            {
                // verwerken spec data uit plugins
                foreach (var element in specData.Elements)
                {
                    switch (element)
                    {
                        case SpecificationParagraph paragraph:
                            switch (paragraph.Type)
                            {
                                case SpecificationParagraphType.Body:
                                    body.Append(OpenXmlHelper.GetTextParagraph($"{paragraph.Text}"));
                                    break;
                                case SpecificationParagraphType.Header1:
                                    body.Append(OpenXmlHelper.GetTextParagraph($"{paragraph.Text}", "Heading1", true));
                                    break;
                                case SpecificationParagraphType.Header2:
                                    body.Append(OpenXmlHelper.GetTextParagraph($"{paragraph.Text}", "Heading2", true));
                                    break;
                                case SpecificationParagraphType.TableHeader:
                                    TableGenerator.UpdateTables($"{paragraph.Text}");
                                    body.Append(OpenXmlHelper.GetTextParagraph($"Tabel {TableGenerator.NumberOfTables.ToString()}: {paragraph.Text}", styleid: "Caption"));
                                    break;
                                case SpecificationParagraphType.Spacer:
                                    body.Append(OpenXmlHelper.GetTextParagraph($"{paragraph.Text}", styleid: "Footer"));
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;

                        case SpecificationTable table:
                            var docTable = OpenXmlHelper.GetTable(table.TableData, firstRowVerticalText: true);
                            body.Append(docTable);
                            break;

                        case SpecificationBulletList bulletList:
                            var docBulletList = OpenXmlHelper.GetBulletList(doc, bulletList.BulletData);
                            body.Append(docBulletList);
                            break;
                    }
                }
            }
        }

        public static void GenerateSpecification(string filename, ControllerModel c, SpecificatorDataModel model)
        {
            var pluginData = new List<SpecificationData>();

            // loopen van alle plugins
            foreach (var plugin in TLCGenPluginManager.Default.ApplicationPlugins)
            {
                // check of de plugin specificatie data heeft
                if (plugin.Item2 is ITLCGenHasSpecification specGen)
                {
                    var d = specGen.GetSpecificationData(c);
                    if (d != null) pluginData.Add(d);
                }
            }

            using (var doc = WordprocessingDocument.Open(filename, true))
            {
                // Add a main document part. 
                var body = doc.MainDocumentPart.Document.Body;

                // Headers, title page, versioning
                FunctionalityGenerator.AddHeaderTextsToDocument(doc, model, c.Data);
                body.Append(FunctionalityGenerator.GetFirstPage(c.Data));
                body.Append(FunctionalityGenerator.GetVersionControl(c.Data));
                
                // Chap 1: Introduction
                FunctionalityGenerator.GetIntroChapter(doc, c, model);
                AppendGenericSpecificationData(doc, pluginData.Where(x => x.Subject == SpecificationSubject.Intro));

                // Chap 2: Structuur en afwikkeling
                body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_StructuurEnAfwikkeling"]}", 1));
                body.Append(FunctionalityGenerator.GetChapter_StructureIntroduction(c));
                body.Append(FunctionalityGenerator.GetChapter_SignaalGroepAfhandeling(doc, c));
                body.Append(FunctionalityGenerator.GetChapter_SignaalGroepInstellingen(doc, c));
                body.Append(FunctionalityGenerator.GetChapter_Perioden(c));
                body.Append(FunctionalityGenerator.GetChapter_Groentijden(c));
                body.Append(FunctionalityGenerator.GetChapter_Modulestructuur(doc, c));
                body.Append(FunctionalityGenerator.GetChapter_VasteAanvragen(c));
                body.Append(FunctionalityGenerator.GetChapter_Meeverlengen(c));
                body.Append(FunctionalityGenerator.GetChapter_Wachtgroen(c));
                body.Append(FunctionalityGenerator.GetChapter_Veiligheidsgroen(c));
                body.Append(FunctionalityGenerator.GetChapter_Senioreningreep(c));
                body.Append(FunctionalityGenerator.GetChapter_Schoolingreep(c));

                // Chap 3: Detectoren
                body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren"]}", 1));
                body.Append(FunctionalityGenerator.GetChapter_DetectieConfiguratie(doc, c));
                body.Append(FunctionalityGenerator.GetChapter_DetectieInstellingen(doc, c));

                AppendGenericSpecificationData(doc, pluginData.Where(x => x.Subject == SpecificationSubject.DynHiaat));

                body.Append(FunctionalityGenerator.GetChapter_DetectieRichtingGevoelig(c));
                body.Append(FunctionalityGenerator.GetChapter_DetectieStoring(doc, c));

                // Chap 4: Intersignaalgroep
                body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Intersignaalgroep"]}", 1));
                body.Append(FunctionalityGenerator.GetChapter_Ontruimingstijden(c));
                body.Append(FunctionalityGenerator.GetChapter_OntruimingstijdenGarantie(c));
                body.Append(FunctionalityGenerator.GetChapter_Synchronisaties(c, doc));

                // Chap 5: Prio ingrepen 
                if (c.PrioData.PrioIngreepType != Models.Enumerations.PrioIngreepTypeEnum.Geen && c.HasPTorHD())
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

                // Chap 6: Halfstar
                if (c.HalfstarData.IsHalfstar)
                {
                    body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Halfstar"]}", 1));
                    body.Append(FunctionalityGenerator.GetChapter_HalfstarIntro(c, doc));
                }

                // Chap 7: Gegenererde specials
                body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_TLCGen_Specials"]}", 1));
                body.Append(OpenXmlHelper.GetTextParagraph("", "Footer"));
                body.Append(FunctionalityGenerator.GetChapter_PTP(c));
                body.Append(FunctionalityGenerator.GetChapter_File(c, doc));
                body.Append(FunctionalityGenerator.GetChapter_VAontr(c, doc));
                body.Append(FunctionalityGenerator.GetChapter_Signalen(c));
                body.Append(FunctionalityGenerator.GetChapter_RobuGroVer(c, doc));
                body.Append(FunctionalityGenerator.GetChapter_PelotonKoppeling(c, doc));

                if (model.SpecialsParagrafen.Any())
                {
                    body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Specials"]}", 1));
                    foreach(var par in model.SpecialsParagrafen)
                    {
                        body.Append(OpenXmlHelper.GetChapterTitleParagraph(par.Titel, 2));
                        foreach(var parpar in par.Text.Split('\n'))
                        {
                            if(!string.IsNullOrWhiteSpace(parpar))
                                body.Append(OpenXmlHelper.GetTextParagraph(parpar));
                        }
                    }
                }

                // Chap 8: AFM
                AppendGenericSpecificationData(doc, pluginData.Where(x => x.Subject == SpecificationSubject.AFM));

                // Chap 9: TalkingTraffic
                if (c.RISData.RISToepassen)
                {
                    body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_TalkingTraffic"]}", 1));
                    body.Append(OpenXmlHelper.GetTextParagraph("", "Footer"));
                    body.Append(FunctionalityGenerator.GetChapter_TT_Algemeen(c, doc));
                    body.Append(FunctionalityGenerator.GetChapter_TT_Instellingen(c));
                    body.Append(FunctionalityGenerator.GetChapter_TT_UC3_Prioriteren(c, doc));
                    body.Append(FunctionalityGenerator.GetChapter_TT_UC4_Informeren(c, doc));
                    body.Append(FunctionalityGenerator.GetChapter_TT_UC5_Optimaliseren(c));
                }

                // Chaps gebruikers plugins
                AppendGenericSpecificationData(doc, pluginData.Where(x => x.Subject == SpecificationSubject.GebruikersPlugin1));

                AppendGenericSpecificationData(doc, pluginData.Where(x => x.Subject == SpecificationSubject.GebruikersPlugin2));

                //ToDo: - OV nadere details (conditionele prio, inmelden koplus, prio nivo, inmelden/aanvragen koplus,
                //                           check wagennummer, anti-jutter, klokperiode als voorwaarde, rit categorie).
                //      - Uitschrijven / verklaren meeverleng opties (ym_maxV1, ym_max_toV2, etc)).

            }
        }
    }
}
