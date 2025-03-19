using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TLCGen.Extensions;
using TLCGen.Models;

using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models.Enumerations;


namespace TLCGen.Specificator
{
    public class TableGenerator 
    {
        private static ResourceDictionary _texts;
        public static ResourceDictionary Texts
        {
            get
            {
                if (_texts == null)
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

        public static int NumberOfTables = 0;
        public static int KlokperiodenTable = 0;  // tbv verwijzing verderop in spec
        public static Dictionary<string, int> Tables { get; } = new Dictionary<string, int>();

        public static void ClearTables()
        {
            Tables.Clear();
        }

        public static void UpdateTables(string table)  // DdO: private -> public ivm aanroep in SpecificationGenerator.cs tbv tabellen in plugins
        {
            NumberOfTables++;
            Tables.Add(table, NumberOfTables);
        }

        private static void ResetTables(string table)
        {
            NumberOfTables = 0;
            Tables.Add(table, NumberOfTables);
        }

        public static List<OpenXmlCompositeElement> Reset_Tables(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
            ResetTables("");
            return items;
        }

        static string TTalgTabel = "";

        public static List<OpenXmlCompositeElement> GetTable_FasenFuncties(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Fasen_Functies");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Fasen_Functies"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"] + " (##)",
                    "Type",
                    "Rijstroken",
                    "Vaste aanvraag                       SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schca") + "##",
                    "Uitgestelde vaste aanvraag ",
                    "Tijd uitgest.aanvr.[TE]              T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tuitgestca") + "##",
                    "Wachtgroen                           SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schwg") + "##",
                    "Meeverlengen                         SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schmv") + "##",
                    "Wachttijd- voorspeller",
                    "Aantal detectoren"
                }
            };
            foreach (var fc in c.Fasen)
            {
                l.Add(new List<string>
                {
                    fc.Naam,
                    fc.Type.GetDescription(),
                    fc.AantalRijstroken.ToString(),
                    fc.VasteAanvraag.GetDescription(),
                    fc.UitgesteldeVasteAanvraag.ToCustomString(),
                    fc.UitgesteldeVasteAanvraag ? fc.UitgesteldeVasteAanvraagTijdsduur.ToString()  : "-",
                    fc.Wachtgroen.GetDescription(),
                    fc.Meeverlengen.GetDescription(),
                    fc.WachttijdVoorspeller == true ? "X" : "-",
                    fc.Detectoren.Count.ToString()
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_FasenTijden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Fasen_Tijden");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Fasen_Tijden"], styleid: "Caption"));
            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"] + " (##)",
                    "Vastgroen                                   TFG ##",
                    "Garantiegroen                               TGG ##",
                    "Minimum garantiegroen",                     
                    "Garantierood                                TRG ##",
                    "Minimum garantierood",                      
                    "Geel                                        TGL ##",
                    "Minimum geel",
                    "Kopmax                                      T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tkm") + "##",
                }
            };
            foreach (var fc in c.Fasen)
            {
                l.Add(new List<string>
                {
                    fc.Naam,
                    fc.TFG.ToString(),
                    fc.TGG.ToString(),
                    fc.TGG_min.ToString(),
                    fc.TRG.ToString(),
                    fc.TRG_min.ToString(),
                    fc.TGL.ToString(),
                    fc.TGL_min.ToString(),
                    fc.Detectoren.Any(x => (x.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.Kopmax)) ? fc.Kopmax.ToString() : "-" });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Perioden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Perioden");

            KlokperiodenTable = NumberOfTables;

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Perioden"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string> { "Periode (###)",
                                   "Start klokperiode           PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmstkp") + "###",
                                   "Einde klokperiode           PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmetkp") + "###",
                                   "Dagtype                     PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmdckp") + "###",
                                   "Groentijden set" }
            };
            foreach (var p in c.PeriodenData.Perioden.Where(x => x.Type == Models.Enumerations.PeriodeTypeEnum.Groentijden))
            {
                l.Add(new List<string> { p.Naam, 
                    p.StartTijd.ToString(@"hh\:mm"), 
                    p.EindTijd.ToString(@"hh\:mm"),
                   (p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Uitgeschakeld ?  "0: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Maandag       ?  "1: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Dinsdag       ?  "2: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Woensdag      ?  "3: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Donderdag     ?  "4: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Vrijdag       ?  "5: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Zondag        ?  "6: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Zondag        ?  "7: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Werkdagen     ?  "8: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Weekeind      ?  "9: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.AlleDagen     ? "10: " : "NG") + p.DagCode.GetDescription(),
                    p.GroentijdenSet });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            if (!c.Data.TVGAMaxAlsDefaultGroentijdSet)
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"Wanneer geen van bovenstaande perioden actief is geldt de dalperiode (met groentijdenset {c.PeriodenData.DefaultPeriodeGroentijdenSet}).", "Footer"));

            items.Add(OpenXmlHelper.GetTextParagraph("Voor het dagtype geldt: 1 t/m 7 zijn de individuele dagen maandag t/m zondag, 8 = werkdagen,                                         " +
                                              "9 = weekend, 10 = alle dagen; 0 = uitgeschakeld.", "Footer"));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Fasen_Groentijden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            OpenXmlCompositeElement tableTitle = null;
            switch (c.Data.TypeGroentijden)
            {
                case Models.Enumerations.GroentijdenTypeEnum.MaxGroentijden:
                    UpdateTables("Table_MaxGroentijden");
                    tableTitle = OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_MaxGroentijden"], styleid: "Caption");
                    break;
                case Models.Enumerations.GroentijdenTypeEnum.VerlengGroentijden:
                    UpdateTables("Table_VerlGroentijden");
                    tableTitle = OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_VerlGroentijden"], styleid: "Caption");
                    break;
            }
            items.Add(tableTitle);

            var l = new List<List<string>>();
            var l1 = new List<string> { (string)Texts["Fase"] };
            if (c.Data.TVGAMaxAlsDefaultGroentijdSet)
            {
                foreach (var set in c.GroentijdenSets.Where(x => x.Naam != c.PeriodenData.DefaultPeriodeGroentijdenSet))
                {
                    l1.Add(set.Naam);
                }
            }
            else
            {
                foreach (var set in c.GroentijdenSets)
                {
                    l1.Add(set.Naam);
                }
            }
            l.Add(l1);
            foreach (var fc in c.Fasen)
            {
                var l2 = new List<string> { fc.Naam };
                if (c.Data.TVGAMaxAlsDefaultGroentijdSet)
                {
                    foreach (var set in c.GroentijdenSets.Where(x => x.Naam != c.PeriodenData.DefaultPeriodeGroentijdenSet))
                    {
                        var f = set.Groentijden.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                        if (f != null)
                        {
                            l2.Add(f.Waarde.HasValue ? f.Waarde.Value.ToString() : "-");
                        }
                        else
                        {
                            l2.Add("-");
                        }
                    }
                }
                else
                {
                    foreach (var set in c.GroentijdenSets)
                    {
                        var f = set.Groentijden.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                        if (f != null)
                        {
                            l2.Add(f.Waarde.HasValue ? f.Waarde.Value.ToString() : "-");
                        }
                        else
                        {
                            l2.Add("-");
                        }
                    }
                }

                l.Add(l2);
            }
            items.Add(OpenXmlHelper.GetTable(l));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Modulen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Modulen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Modulen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string> { (string)Texts["Generic_Module"], (string)Texts["Generic_Fasen"] }
            };
            foreach (var m in c.ModuleMolen.Modules)
            {
                if (m.Fasen.Any())
                {
                    l.Add(new List<string> { m.Naam, m.Fasen.Select(x => x.FaseCyclus).Aggregate((y, z) => y + ", " + z) });
                }
                else
                {
                    l.Add(new List<string> { m.Naam, "leeg" });
                }
            }
            items.Add(OpenXmlHelper.GetTable(l));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_ModuleStructuurInstellingen(WordprocessingDocument doc, ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            var altperblok = c.AlternatievenPerBlokData.ToepassenAlternatievenPerBlok;

            if (c.ModuleMolen.LangstWachtendeAlternatief)
            {
                UpdateTables("Table_VooruitAltInst");

                items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_VooruitAltInst"], styleid: "Caption"));

                if (altperblok)
                {
                    var l = new List<List<string>>
                    {
                        new List<string>
                        {
                            (string)Texts["Generic_Fase"] + " (##)",
                            "Aant. modulen vooruit                       PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmlfpr") + "##",
                            "Alt. toegestaan                             SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schaltg") + "##",
                            "Min. benodigde ruimte                       PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltp") + "##",
                            "Min. te maken groentijd                     PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltg") + "##",
                            "Toegestane alt. blokken                     PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltb") + "##",
                        }
                    };

                    foreach (var x in c.ModuleMolen.FasenModuleData)
                    {
                        var mls = "";
                        var altb = c.AlternatievenPerBlokData.AlternatievenPerBlok.FirstOrDefault(y => y.FaseCyclus == x.FaseCyclus).BitWiseBlokAlternatief;
                        var altbb = altb;

                        bool bit0 = false, bit1 = false, bit2 = false, bit3 = false, bit4 = false,
                             bit5 = false, bit6 = false, bit7 = false, bit8 = false, bit9 = false;

                        if (altbb >= 512) { bit9 = true; altbb %= 512; }
                        if (altbb >= 256) { bit8 = true; altbb %= 256; }
                        if (altbb >= 128) { bit7 = true; altbb %= 128; }
                        if (altbb >=  64) { bit6 = true; altbb %=  64; }
                        if (altbb >=  32) { bit5 = true; altbb %=  32; }
                        if (altbb >=  16) { bit4 = true; altbb %=  16; }
                        if (altbb >=   8) { bit3 = true; altbb %=   8; }
                        if (altbb >=   4) { bit2 = true; altbb %=   4; }
                        if (altbb >=   2) { bit1 = true; altbb %=   2; }
                        if (altbb >=   1) { bit0 = true; altbb %=   1; }

                        if (bit0) mls +=                         "ML1";
                        if (bit1) mls += (mls == "") ? "ML2" : ", ML2";
                        if (bit2) mls += (mls == "") ? "ML3" : ", ML3";
                        if (bit3) mls += (mls == "") ? "ML4" : ", ML4";
                        if (bit4) mls += (mls == "") ? "ML5" : ", ML5";
                        if (bit5) mls += (mls == "") ? "ML6" : ", ML6";
                        if (bit6) mls += (mls == "") ? "ML7" : ", ML7";
                        if (bit7) mls += (mls == "") ? "ML8" : ", ML8";
                        if (bit8) mls += (mls == "") ? "ML9" : ", ML9";

                        mls = (altb > 0) ? " (" + mls + ")" : "";

                        l.Add(new List<string>
                        {
                            x.FaseCyclus,
                            x.ModulenVooruit.ToString(),
                            x.AlternatiefToestaan.ToCustomString(),
                            x.AlternatieveRuimte.ToString(),
                            x.AlternatieveGroenTijd.ToString(),
                            altb + mls,
                        });
                    }
                    items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
                }
                else  // geen alternatieven per blok
                {
                    var l = new List<List<string>>
                    {
                        new List<string>
                        {
                            (string)Texts["Generic_Fase"] + " (##)",
                            "Aant. modulen vooruit                       PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmlfpr") + "##",
                            "Alt. toegestaan                             SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schaltg") + "##",
                            "Min. benodigde ruimte                       PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltp") + "##",
                            "Min. te maken groentijd                     PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltg") + "##",
                        }
                    };
                    c.ModuleMolen.FasenModuleData.ForEach(x => l.Add(new List<string>
                    {
                        x.FaseCyclus,
                        x.ModulenVooruit.ToString(),
                        x.AlternatiefToestaan.ToCustomString(),
                        x.AlternatieveRuimte.ToString(),
                        x.AlternatieveGroenTijd.ToString()
                    }));
                    items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
                }
            }
            else 
            {
                UpdateTables("Table_Vooruit");

                items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Vooruit"], styleid: "Caption"));

                var lt = new List<string>
                {
                    (string)Texts["Generic_Fase"] + " (##)",
                    "Aantal modulen vooruit                       PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmlfpr") + "##",
                };
                var l = new List<List<string>>
                {
                    lt
                };
                c.ModuleMolen.FasenModuleData.ForEach(x =>
                {
                    var nl = new List<string>
                    {
                        x.FaseCyclus,
                        x.ModulenVooruit.ToString()
                    };
                    
                    l.Add(nl);
                });

                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            }

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_AlternatievenOnderDekking(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            if (c.ModuleMolen.Modules.Any(x => x.Fasen.Any(x2 => x2.Alternatieven.Any())))
            {
                UpdateTables("Table_AlternatievenOnderDekking");

                items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_AlternatievenOnderDekking"], styleid: "Caption"));

                var l = new List<List<string>>
            {
                new List<string> { "Module (ML$)", "Richting (##)", "Onder dekking van",
                                   "Min. te maken groentijd                     PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltg") + "$##", }
            };
                foreach (var ml in c.ModuleMolen.Modules)
                {
                    foreach (var mlfc in ml.Fasen)
                    {
                        if (mlfc.Alternatieven.Any())
                        {
                            foreach (var alt in mlfc.Alternatieven)
                            {
                                l.Add(new List<string> { ml.Naam, alt.FaseCyclus, mlfc.FaseCyclus, alt.AlternatieveGroenTijd.ToString() });
                            }
                        }
                    }
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            }

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_VasteAanvragen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_VasteAanvragen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_VasteAanvragen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Fase (##)",
                    "Vaste aanvraag                                    SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schca")      + "##",
                    "Uitgestelde vaste aanvraag",
                    "Uitsteltijd in TE                                 T   " + CCOLGeneratorSettingsProvider.Default.GetElementName("tuitgestca") + "##",
                 }
            };
            foreach (var cfc in c.Fasen)
            {
                l.Add(new List<string>
                {
                    cfc.Naam,
                    cfc.VasteAanvraag.GetDescription(),
                    cfc.VasteAanvraag != NooitAltijdAanUitEnum.Nooit ?  cfc.UitgesteldeVasteAanvraag.ToCustomString() : "-",
                    cfc.VasteAanvraag != NooitAltijdAanUitEnum.Nooit ? (cfc.UitgesteldeVasteAanvraag ? cfc.UitgesteldeVasteAanvraagTijdsduur.ToString() : "-") : "-"
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Meeverlengen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Meeverlengen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Meeverlengen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Fase (##)",
                    "Meeverlengen                                       SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schmv") + "##",
                    "Type meeverlengen",
                    "Type instelbaar op straat                          PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmv")     + "##",
                    "Hard meeverlengen met ($$)",
                    "Hard mv toepassen                                  SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schhardmv") + "##$$",
                    "OT verschil"
                }
            };
            foreach (var mvfc in c.Fasen.Where(x => x.Meeverlengen != NooitAltijdAanUitEnum.Nooit || x.Meeverlengen == NooitAltijdAanUitEnum.Nooit))
            {
                var mvmet = "-";
                var schmvmet = "-";
                if (mvfc.HardMeeverlengenFaseCycli.Any())
                {
                    mvmet = "";
                    schmvmet = "";
                    var first = true;
                    foreach (var fc in mvfc.HardMeeverlengenFaseCycli)
                    {
                        if (!first)
                        {
                            mvmet    += ", ";
                            schmvmet += ", ";
                        }
                        first = false;
                        mvmet += fc.FaseCyclus + " (";
                        mvmet += fc.Type.GetDescription() + ")";
                        schmvmet += fc.FaseCyclus + " (Aan)";
                    }
                }

                l.Add(new List<string>
                {
                    mvfc.Naam,
                    mvfc.Meeverlengen.GetDescription(),

                   (mvfc.MeeverlengenType == Models.Enumerations.MeeVerlengenTypeEnum.Default         ? " 1: " :
                    mvfc.MeeverlengenType == Models.Enumerations.MeeVerlengenTypeEnum.To              ? " 2: " :
                    mvfc.MeeverlengenType == Models.Enumerations.MeeVerlengenTypeEnum.MKTo            ? " 3: " :
                    mvfc.MeeverlengenType == Models.Enumerations.MeeVerlengenTypeEnum.Voetganger      ? " 4: " :
                    mvfc.MeeverlengenType == Models.Enumerations.MeeVerlengenTypeEnum.DefaultCCOL     ? " 5: " :
                    mvfc.MeeverlengenType == Models.Enumerations.MeeVerlengenTypeEnum.ToCCOL          ? " 6: " :
                    mvfc.MeeverlengenType == Models.Enumerations.MeeVerlengenTypeEnum.MKToCCOL        ? " 7: " :
                    mvfc.MeeverlengenType == Models.Enumerations.MeeVerlengenTypeEnum.MaatgevendGroen ? " 8: " :
                    mvfc.MeeverlengenType == Models.Enumerations.MeeVerlengenTypeEnum.Default2        ? " 9: " :
                    mvfc.MeeverlengenType == Models.Enumerations.MeeVerlengenTypeEnum.To2             ? "10: " :
                    mvfc.MeeverlengenType == Models.Enumerations.MeeVerlengenTypeEnum.MKTo2           ? "11: " :
                    mvfc.MeeverlengenType == Models.Enumerations.MeeVerlengenTypeEnum.Voetganger2     ? "12: " :
                                                                                                        " 0: " ) + mvfc.MeeverlengenType.GetDescription(),
                    mvfc.MeeverlengenTypeInstelbaarOpStraat.ToCustomString(),
                    mvmet,
                    schmvmet,
                    mvfc.MeeverlengenVerschil != null ? mvfc.MeeverlengenVerschil.ToString() : "-",
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            
            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Veiligheidsgroen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Veiligheidsgroen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Veiligheidsgroen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Fase (##)",
                    "Detector ($$$)",
                    "Toepassen                              SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schvg")    + "$$$",
                    "Hiaat                                  T   " + CCOLGeneratorSettingsProvider.Default.GetElementName("tvghiaat") + "$$$",
                    "Volgtijd                               T   " + CCOLGeneratorSettingsProvider.Default.GetElementName("tvgvolg")  + "$$$",
                    "Max extra groen                        T   " + CCOLGeneratorSettingsProvider.Default.GetElementName("tvgmax")   + "##",
                }
            };
            foreach (var vgfc in c.Fasen.Where(x => x.Detectoren.Any(y => y.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit)))
            {
                foreach (var vgd in vgfc.Detectoren.Where(z => z.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit))
                {
                    l.Add(new List<string>
                    {
                        vgfc.Naam,
                        vgd.Naam,
                        vgd.VeiligheidsGroen.GetDescription(),
                        vgd.VeiligheidsGroenHiaat.ToString(),
                        vgd.VeiligheidsGroenVolgtijd.ToString(),
                        vgfc.VeiligheidsGroenMaximaal.ToString()
                    });
                }
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Senioreningreep(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Senioreningreep");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Senioreningreep"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Fase (##)",
                    "Toepassen                                    SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schsi") + "##",
                    "Perc. extra groen (t.o.v. TFG)               PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmsiexgrperc") + "##",
                    "Detector ($$$)",
                    "Vasthoudtijd drukknop [TE]                   T   " + CCOLGeneratorSettingsProvider.Default.GetElementName("tdbsiexgr") + "d" + "$$$",
                }
            };
            var oldfc = "";
            foreach (var si in c.Fasen.Where(x => x.SeniorenIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Nooit))
            {
                var fc = si.Naam;
                foreach (var d in si.Detectoren.Where(x => x.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen || x.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten))
                { 
                    l.Add(new List<string>
                    {
                        (fc != oldfc) ? si.Naam : "",
                        (fc != oldfc) ? si.SeniorenIngreep.GetDescription() : "",
                        (fc != oldfc) ? si.SeniorenIngreepExtraGroenPercentage.ToString() : "",
                        d.Naam,
                        si.SeniorenIngreepBezetTijd.ToString(),
                    });
                    oldfc = fc;
                }
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Schoolingreep(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Schoolingreep");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Schoolingreep"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Fase (##)",
                    "Toepassen                                    SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schschoolingreep") + "##",
                    "Max. duur groen (vanaf SG) [TE]              T   " + CCOLGeneratorSettingsProvider.Default.GetElementName("tschoolingreepmaxg")  + "##",
                    "Detector ($$$)",
                    "Vasthoudtijd drukknop [TE]                   T   " + CCOLGeneratorSettingsProvider.Default.GetElementName("tdbsi") + "d" + "$$$",
                    "Hiaattijd [TE]                               TDH $$$"
                }
            };
            var oldfc = "";
            foreach (var si in c.Fasen.Where(x => x.SchoolIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Nooit))
            {
                var fc = si.Naam;
                foreach (var d in si.Detectoren.Where(x => x.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen || x.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten))
                {
                    l.Add(new List<string>
                    {
                        (fc != oldfc) ? si.Naam : "",
                        (fc != oldfc) ? si.SchoolIngreep.GetDescription() : "",
                        (fc != oldfc) ? si.SchoolIngreepMaximumGroen.ToString() : "",
                        d.Naam,
                        si.SchoolIngreepBezetTijd.ToString(),
                        d.TDH.ToString(),
                    });
                    oldfc = fc;
                }
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Detectie_Functies(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Detectoren_Functies");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Detectoren_Functies"], styleid: "Caption"));

            if (!(c.Fasen.Any(x => x.Detectoren.Any(y => y.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit)))) // wanneer overal veiligheidsgroen op 'nooit' is ingesteld
            {
                var l = new List<List<string>>
                {
                    new List<string>
                    {
                        (string)Texts["Generic_Detector"] + " (###)",
                        "Fase",
                        "Type",
                        "Aanvraagfunctie                           PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmda") + "###",
                        "Verlengfunctie                            PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmk") + "###",
                        "Aanvraag direct",
                        "Wachtlicht",
                        "Rijstrook",
                        "Aanvraag bij storing                      SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schdvak") + "###",
                        "Veiligheidsgroen",
                    }
                };
                foreach (var fc in c.Fasen)
                {
                    foreach (var d in fc.Detectoren)
                    {
                        l.Add(new List<string>
                        {
                            d.Naam,
                            fc.Naam,
                            d.Type.GetDescription(),
                           (d.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.Uit      ? "0: " :
                            d.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.RnietTRG ? "1: " :
                            d.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.Rood     ? "2: " :
                            d.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.RoodGeel ? "3: " : "" ) + d.Aanvraag.GetDescription(),
                          //d.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.Geen     ? "4: " : "" ) + d.Aanvraag.GetDescription(),
                           (d.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.Uit    ? "0: " :
                            d.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.Kopmax ? "1: " :
                            d.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.MK1    ? "2: " :
                            d.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.MK2    ? "3: " : "" ) + d.Verlengen.GetDescription(),
                          //d.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.Geen   ? "4: " : "" ) + d.Verlengen.GetDescription(),
                            d.AanvraagDirect.ToCustomString(),
                            d.Wachtlicht.ToCustomString(),
                            d.Rijstrook.ToString(),
                            d.AanvraagBijStoring.GetDescription(),
                            d.VeiligheidsGroen.GetDescription(),
                        });
                    }
                }
                foreach (var d in c.Detectoren)
                {
                    l.Add(new List<string>
                    {
                        d.Naam,
                        "-",
                        d.Type.GetDescription(),
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                    });
                }
                foreach(var d in c.SelectieveDetectoren)
                {
                    l.Add(new List<string>
                    {
                        d.Naam,
                        "-",
                        "Selectieve detector", //d.Type.GetDescription(),
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-", 
                    });
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            }
            else
            {
                var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Detector"] + " (###)",
                    "Fase",
                    "Type",
                    "Aanvraagfunctie                           PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmda") + "###",
                    "Verlengfunctie                            PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmk") + "###",
                    "Aanvraag direct",
                    "Wachtlicht",
                    "Rijstrook",
                    "Aanvraag bij storing                      SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schdvak") + "###",
                  //"Veiligheidsgroen                          SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schvg")   + "###",
                }
            };
                foreach (var fc in c.Fasen)
                {
                    foreach (var d in fc.Detectoren)
                    {
                        l.Add(new List<string>
                        {
                             d.Naam,
                            fc.Naam,
                            d.Type.GetDescription(),
                           (d.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.Uit      ? "0: " :
                            d.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.RnietTRG ? "1: " :
                            d.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.Rood     ? "2: " :
                            d.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.RoodGeel ? "3: " : "" ) + d.Aanvraag.GetDescription(),
                          //d.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.Geen     ? "4: " : "" ) + d.Aanvraag.GetDescription(),
                           (d.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.Uit    ? "0: " :
                            d.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.Kopmax ? "1: " :
                            d.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.MK1    ? "2: " :
                            d.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.MK2    ? "3: " : "" ) + d.Verlengen.GetDescription(),
                          //d.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.Geen   ? "4: " : "" ) + d.Verlengen.GetDescription(),
                            d.AanvraagDirect.ToCustomString(),
                            d.Wachtlicht.ToCustomString(),
                            d.Rijstrook.ToString(),
                            d.AanvraagBijStoring.GetDescription(),
                          //d.VeiligheidsGroen.GetDescription(),
                        });
                    }
                }
                foreach (var d in c.Detectoren)
                {
                    l.Add(new List<string>
                    {
                        d.Naam,
                        "-",
                        d.Type.GetDescription(),
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                      //"-",
                    });
                }
                foreach (var d in c.SelectieveDetectoren)
                {
                    l.Add(new List<string>
                    {
                        d.Naam,
                        "-",
                        "Selectieve detector", //d.Type.GetDescription(),
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                      //"-",
                    });
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            }

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Detectie_Tijden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Detectoren_Tijden");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Detectoren_Tijden"], styleid: "Caption"));
            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Detector"] + " (###)",
                    "Fase",
                    "Bezettijd                       TDB ###, in TE",
                    "Hiaattijd                       TDH ###, in TE",
                    "Ondergedrag                     TOG ###, in TM",
                    "Bovengedrag                     TBG ###, in TM",
                    "Flutter tijd                    TFL ###, in TS",
                    "Flutter counter                 CFL"
                }
            };
            foreach (var fc in c.Fasen)
            {
                foreach (var d in fc.Detectoren)
                {
                    l.Add(new List<string>
                    {
                        d.Naam,
                        fc.Naam,
                        d.TDB.ToString(),
                        d.TDH.ToString(),
                        d.TOG.ToString(),
                        d.TBG.ToString(),
                        d.TFL.ToString(),
                        d.CFL.ToString(),
                    });
                }
            }
            foreach (var d in c.Detectoren)
            {
                l.Add(new List<string>
                {
                    d.Naam,
                    "-",
                    d.TDB.ToString(),
                    d.TDH.ToString(),
                    d.TOG.ToString(),
                    d.TBG.ToString(),
                    d.TFL.ToString(),
                    d.CFL.ToString(),
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Detectie_DynHiaat(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Detectoren_DynHiaat");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Detectoren_DynHiaat"], styleid: "Caption"));
            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"] + " (##)",
                    "Toepassen                            SCH dynhiaat##",
                    "Opdrempelen                          SCH opdrempelen##",
                    "Start op ED koplus                   SCH edkop_##",
                    (string)Texts["Generic_Detector"] + " ($$)",
                    "Rijstrook",
                    "Moment 1                             T $$_1",
                    "Moment 2                             T $$_2",
                    "TDH 1                                T tdh_$$_1",
                    "TDH 2                                T tdh_$$_2",
                    "Max. verlengen                       T max_$$",
                    "Detector instelling                  PRM springverleng_$$",
                }
            };
            l.Add(new List<string>
            {
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
            });
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Detectie_RGA(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Detectoren_RGA");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Detectoren_RGA"], styleid: "Caption"));
            var l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Fase ",
                        "Van (###)",
                        "Naar",
                        "Toepassen                                SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schrga") + "d###",
                        "Maximaal tijdsverschil                   T "   + CCOLGeneratorSettingsProvider.Default.GetElementName("trga")    + "d###",
                        "Reset aanvraag",
                        "Reset tijd                               T "   + CCOLGeneratorSettingsProvider.Default.GetElementName("trgav")   + "d###",
                        "Snelle variant                           SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schrgad") + "d###",
                    }
                };
            foreach (var rga in c.RichtingGevoeligeAanvragen)
            {
                l.Add(new List<string>
                    {
                            rga.FaseCyclus,
                            rga.VanDetector,
                            rga.NaarDetector,
                            rga.AltijdAanUit.GetDescription(),
                            rga.MaxTijdsVerschil.ToString(),
                            rga.ResetAanvraag.ToCustomString(),
                            rga.ResetAanvraag ? rga.ResetAanvraagTijdsduur.ToString() : "-",
                            "Aan",
                    });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            items.Add(OpenXmlHelper.GetTextParagraph("Er bestaat bij de richtinggevoelige aanvraag een 'snelle variant' en een 'zekere variant'. " +
                "Bij de snelle variant wordt de aanvraag gezet wanneer de tweede detector opkomt binnen een ingestelde tijd na de eerste " +
                "detector. Bij de zekere variant wordt de aanvraag opgezet wanneer de tweede detector opkomt terwijl de eerste nog op is. " +
                "Via een schakelaar kan in de regeling tussen beide varianten worden gekozen; standaard wordt in TLCGen de 'snelle variant' toegepast.", "Footer"));
            items.Add(OpenXmlHelper.GetTextParagraph("LET OP: er zit maar één letter verschil tussen de schakelaar waarmee het type variant " +
                "wordt gekozen, en de schakelaar waarmee de richtinggevoelige aanvraag wordt aan- of uitgezet.", "Caption"));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Detectie_RGV(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Detectoren_RGV");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Detectoren_RGV"], styleid: "Caption"));
            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Fase",
                    "Van (###)",
                    "Naar ($$$)",
                    "Toepassen                                    SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schrgvl") + "d###",
                    "Type verlengen                               PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmkrg") + "d###",
                    "Maximaal tijdsverschil                       T "   + CCOLGeneratorSettingsProvider.Default.GetElementName("trgr")    + "d###_d$$$",
                    "Verleng tijd                                 T "   + CCOLGeneratorSettingsProvider.Default.GetElementName("trgv")    + "d###_d$$$",
                }
            };
            foreach (var rgv in c.RichtingGevoeligVerlengen)
            {
                l.Add(new List<string>
                {
                    rgv.FaseCyclus,
                    rgv.VanDetector,
                    rgv.NaarDetector,
                    rgv.AltijdAanUit.GetDescription(),
                   (rgv.TypeVerlengen == Models.Enumerations.RichtingGevoeligVerlengenTypeEnum.Uit    ? "0: " :
                    rgv.TypeVerlengen == Models.Enumerations.RichtingGevoeligVerlengenTypeEnum.Kopmax ? "1: " :
                    rgv.TypeVerlengen == Models.Enumerations.RichtingGevoeligVerlengenTypeEnum.MK1    ? "2: " :
                    rgv.TypeVerlengen == Models.Enumerations.RichtingGevoeligVerlengenTypeEnum.MK2    ? "3: " : "" ) + rgv.TypeVerlengen.GetDescription(),
                  //rgv.TypeVerlengen == Models.Enumerations.RichtingGevoeligVerlengenTypeEnum.Geen   ? "4: " : "" ) + rgv.TypeVerlengen.GetDescription(),
                    rgv.MaxTijdsVerschil.ToString(),
                    rgv.VerlengTijd.ToString(),
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Detectie_StoringMaatregelen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Detectoren_StoringMaatregelen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Detectoren_StoringMaatregelen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"] + " (##)",
                    "Vaste aanvraag bij detectiestoring van:",
                    "Uitgestelde vaste aanvraag",
                    "Uitsteltijd in TE                                            T "   + CCOLGeneratorSettingsProvider.Default.GetElementName("tdstvert") + "##",
                    "Vervangend hiaat koplus",
                    "Instelling vervangen hiaat                                   T "   + CCOLGeneratorSettingsProvider.Default.GetElementName("thdv")     + "##<dp>",
                    "Percentage groentijd bij storing",
                    "Instelling percentage groentijd                              PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmperc")  + "##"
                }
            };
            foreach (var fc in c.Fasen)
            {
                l.Add(new List<string>
                {
                    fc.Naam,
                    fc.AanvraagBijDetectieStoringKoplusKnop ? "kop + knop"
                                                            : fc.AanvraagBijDetectieStoringKopLang ? "kop + lang"
                                                                                                   : fc.AanvraagBijDetectieStoring ? "alle aanvr. det."
                                                                                                                                   : "-",
                    fc.AanvraagBijDetectieStoringVertraagd.ToCustomString(),
                    fc.AanvraagBijDetectieStoringVertraagd ? fc.AanvraagBijDetectieStoringVertraging.ToString() : "-",
                    fc.HiaatKoplusBijDetectieStoring.ToCustomString(),
                    fc.HiaatKoplusBijDetectieStoring ? fc.VervangendHiaatKoplus.ToString() : "-",
                    fc.PercentageGroenBijDetectieStoring.ToCustomString(),
                    fc.PercentageGroenBijDetectieStoring ? fc.PercentageGroen.ToString() : "-"
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Ontruimingstijden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Ontruimingstijden");

            if (c.Data.Intergroen)
            {
                items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Intergroentijden"], styleid: "Caption"));
            }
            else
            {
                items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Ontruimingstijden"], styleid: "Caption"));
            }

            var sl = new List<List<string>>();

            var top = new List<string> { "" };
            foreach (var fc in c.Fasen)
            {
                top.Add(fc.Naam);
            }
            sl.Add(top);

            foreach (var fcVan in c.Fasen)
            {
                var tijden = new List<string> { fcVan.Naam };
                foreach (var fcNaar in c.Fasen)
                {
                    if (ReferenceEquals(fcVan, fcNaar))
                    {
                        tijden.Add("X");
                    }
                    else
                    {
                        var ot = c.InterSignaalGroep.Conflicten.FirstOrDefault(x => x.FaseVan == fcVan.Naam && x.FaseNaar == fcNaar.Naam);
                        if (ot != null)
                        {
                            tijden.Add(ot.Waarde.ToString());
                        }
                        else
                        {
                            tijden.Add("");
                        }
                    }
                }
                sl.Add(tijden);
            }

            items.Add(OpenXmlHelper.GetTable(sl));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_OntruimingstijdenGarantie(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_OntruimingstijdenGarantie");

            if (c.Data.Intergroen)
            {
                items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_IntergroentijdenGarantie"], styleid: "Caption"));
            }
            else
            {
                items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_OntruimingstijdenGarantie"], styleid: "Caption"));
            }

            var sl = new List<List<string>>();

            var top = new List<string> { "" };
            foreach (var fc in c.Fasen)
            {
                top.Add(fc.Naam);
            }
            sl.Add(top);

            foreach (var fcVan in c.Fasen)
            {
                var tijden = new List<string> { fcVan.Naam };
                foreach (var fcNaar in c.Fasen)
                {
                    if (ReferenceEquals(fcVan, fcNaar))
                    {
                        tijden.Add("X");
                    }
                    else
                    {
                        var ot = c.InterSignaalGroep.Conflicten.FirstOrDefault(x => x.FaseVan == fcVan.Naam && x.FaseNaar == fcNaar.Naam);
                        if (ot != null)
                        {
                            tijden.Add(ot.GarantieWaarde.HasValue ? ot.GarantieWaarde.Value.ToString() : "-");
                        }
                        else
                        {
                            tijden.Add("");
                        }
                    }
                }
                sl.Add(tijden);
            }

            items.Add(OpenXmlHelper.GetTable(sl));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Intersignaalgroep_Meeaanvragen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Intersignaalgroep_Meeaanvragen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Intersignaalgroep_Meeaanvragen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Van (##)",
                    "Naar ($$)",
                    "Type meeaanvraag                PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmtypema") + "##$$" ,
                    "Schakelbaar                     SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schma") + "##$$" ,
                    "Type instelbaar",
                    "Detectie afhankelijk",
                    "Uitgesteld                      T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tuitgestma") + "##$$" ,
                }
            };
            foreach (var ma in c.InterSignaalGroep.Meeaanvragen)
            {
                var da = "-";
                if (ma.DetectieAfhankelijk && ma.Detectoren.Any())
                {
                    da = "x (";
                    var first = true;
                    foreach (var d in ma.Detectoren)
                    {
                        if (!first) da += ", ";
                        first = false;
                        da += d.MeeaanvraagDetector;
                    }
                    da += ")";
                }
                l.Add(new List<string>
                {
                    ma.FaseVan,
                    ma.FaseNaar,
                   (ma.Type == Models.Enumerations.MeeaanvraagTypeEnum.Aanvraag                       ? "1: " :
                    ma.Type == Models.Enumerations.MeeaanvraagTypeEnum.RoodVoorAanvraag               ? "2: " :
                    ma.Type == Models.Enumerations.MeeaanvraagTypeEnum.RoodVoorAanvraagGeenConflicten ? "3: " :
                    ma.Type == Models.Enumerations.MeeaanvraagTypeEnum.Startgroen                     ? "4: " : "0" ) + ma.Type.GetDescription(),
                    ma.AanUit.GetDescription(),
                    ma.TypeInstelbaarOpStraat.ToCustomString(),
                    da,
                    ma.Type == Models.Enumerations.MeeaanvraagTypeEnum.Startgroen && ma.Uitgesteld ? "x (" + ma.UitgesteldTijdsduur.ToString() + ")" : "-"
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Intersignaalgroep_Nalopen_SG(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Intersignaalgroep_Nalopen_SG");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Intersignaalgroep_Nalopen_SG"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Van (##)",
                    "Naar ($$)",
                    "Type naloop",
                    "Detectie afhankelijk",
                    "Inlopen/inrijden bij groen",
                    "Vaste nalooptijd                             T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsg")   + "##$$,  ",
                    "Det.afh.nalooptijd                           T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsgd")  + "##$$,  ",
                    "Inlooptijd                                   T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tinl")    + "##$$,  ",
                    "Late release tijd                            T " + CCOLGeneratorSettingsProvider.Default.GetElementName("treallr") + "$$##,  ",
                }
            };
            foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == Models.Enumerations.NaloopTypeEnum.StartGroen))
            {
                var da = "-";
                if (nl.DetectieAfhankelijk && nl.Detectoren.Any())
                {
                    da = "";
                    var first = true;
                    foreach (var d in nl.Detectoren)
                    {
                        if (!first) da += ", ";
                        first = false;
                        da += d.Detector;
                    }
                }
                
                var t1 = "-";
                var t2 = "-";

                if (nl.Type == Models.Enumerations.NaloopTypeEnum.StartGroen)
                {
                    if (nl.VasteNaloop)
                    {
                        t1 = nl.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.StartGroen).Waarde.ToString();
                    }
                    if (nl.DetectieAfhankelijk)
                    {
                        t2 = nl.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.StartGroenDetectie).Waarde.ToString();
                    }
                }
                l.Add(new List<string>
                {
                    nl.FaseVan,
                    nl.FaseNaar,
                    nl.Type.GetDescription(),
                    da,
                    nl.InrijdenTijdensGroen.ToCustomString(),
                    t1, t2, 
                   (nl.Type == Models.Enumerations.NaloopTypeEnum.StartGroen) ? (nl.MaximaleVoorstart != null) ? nl.MaximaleVoorstart.Value.ToString() : "-" : "-",
                   (nl.Type != Models.Enumerations.NaloopTypeEnum.StartGroen) ? (nl.MaximaleVoorstart != null) ? nl.MaximaleVoorstart.Value.ToString() : "-" : "-",
                }); ;
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Intersignaalgroep_Nalopen_CV(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Intersignaalgroep_Nalopen_CV");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Intersignaalgroep_Nalopen_CV"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Van (##)",
                    "Naar ($$)",
                    "Type naloop",
                    "Detectie afhankelijk",
                    "Inlopen/inrijden bij groen",
                    "Vaste nalooptijd FG                            T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfg")   + "##$$,  ",
                    "Det.afh.nalooptijd FG                          T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfgd")  + "##$$,  ",
                    "Vaste nalooptijd CV                            T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcv")   + "##$$,  ",
                    "Det.afh.nalooptijd CV                          T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcvd")  + "##$$,  ",
                    "Inlooptijd                                     T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tinl")    + "##$$,  ",
                    "Late release tijd                              T " + CCOLGeneratorSettingsProvider.Default.GetElementName("treallr") + "$$##,  ",
                }
            };
            foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == Models.Enumerations.NaloopTypeEnum.CyclischVerlengGroen))
            {
                var da = "-";
                if (nl.DetectieAfhankelijk && nl.Detectoren.Any())
                {
                    da = "";
                    var first = true;
                    foreach (var d in nl.Detectoren)
                    {
                        if (!first) da += ", ";
                        first = false;
                        da += d.Detector;
                    }
                }
                var t1 = "-";
                var t2 = "-";
                var t3 = "-";
                var t4 = "-";
                if (nl.Type == Models.Enumerations.NaloopTypeEnum.CyclischVerlengGroen && nl.VasteNaloop)
                {
                    if (nl.VasteNaloop)
                    {
                        t1 = nl.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.VastGroen).Waarde.ToString();
                    }
                    if (nl.DetectieAfhankelijk)
                    {
                        t2 = nl.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.VastGroenDetectie).Waarde.ToString();
                    }
                }
                if (nl.Type == Models.Enumerations.NaloopTypeEnum.CyclischVerlengGroen && nl.DetectieAfhankelijk)
                {
                    if (nl.VasteNaloop)
                    {
                        t3 = nl.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.EindeVerlengGroen).Waarde.ToString();
                    }
                    if (nl.DetectieAfhankelijk)
                    {
                        t4 = nl.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.EindeVerlengGroenDetectie).Waarde.ToString();
                    }
                }
                l.Add(new List<string>
                {
                    nl.FaseVan,
                    nl.FaseNaar,
                    nl.Type.GetDescription(),
                    da,
                    nl.InrijdenTijdensGroen.ToCustomString(),
                    t1, t2, t3, t4,
                   (nl.Type == Models.Enumerations.NaloopTypeEnum.StartGroen) ? (nl.MaximaleVoorstart != null) ? nl.MaximaleVoorstart.Value.ToString() : "-" : "-",
                   (nl.Type != Models.Enumerations.NaloopTypeEnum.StartGroen) ? (nl.MaximaleVoorstart != null) ? nl.MaximaleVoorstart.Value.ToString() : "-" : "-",
                }); ;
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Intersignaalgroep_Nalopen_EG(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Intersignaalgroep_Nalopen_EG");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Intersignaalgroep_Nalopen_EG"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Van (##)",
                    "Naar ($$)",
                    "Type naloop",
                    "Detectie afhankelijk",
                    "Inlopen/inrijden bij groen",
                    "Vaste nalooptijd FG                            T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfg")   + "##$$,  ",
                    "Det.afh.nalooptijd FG                          T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfgd")  + "##$$,  ",
                    "Vaste nalooptijd EG                            T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tnleg")   + "##$$,  ",
                    "Det.afh.nalooptijd EG                          T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tnlegd")  + "##$$,  ",
                    "Inlooptijd                                     T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tinl")    + "##$$,  ",
                    "Late release tijd                              T " + CCOLGeneratorSettingsProvider.Default.GetElementName("treallr") + "$$##,  ",
                }
            };
            foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == Models.Enumerations.NaloopTypeEnum.EindeGroen))
            {
                var da = "-";
                if (nl.DetectieAfhankelijk && nl.Detectoren.Any())
                {
                    da = "";
                    var first = true;
                    foreach (var d in nl.Detectoren)
                    {
                        if (!first) da += ", ";
                        first = false;
                        da += d.Detector;
                    }
                }

                var t1 = "-";
                var t2 = "-";
                var t3 = "-";
                var t4 = "-";
                if (nl.Type == Models.Enumerations.NaloopTypeEnum.EindeGroen && nl.VasteNaloop)
                {
                    if (nl.VasteNaloop)
                    {
                        t1 = nl.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.VastGroen).Waarde.ToString();
                    }
                    if (nl.DetectieAfhankelijk)
                    {
                        t2 = nl.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.VastGroenDetectie).Waarde.ToString();
                    }
                }
                if (nl.Type == Models.Enumerations.NaloopTypeEnum.EindeGroen && nl.DetectieAfhankelijk)
                {
                    if (nl.VasteNaloop)
                    {
                        t3 = nl.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.EindeGroen).Waarde.ToString();
                    }
                    if (nl.DetectieAfhankelijk)
                    {
                        t4 = nl.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.EindeGroenDetectie).Waarde.ToString();
                    }
                }
                l.Add(new List<string>
                {
                    nl.FaseVan,
                    nl.FaseNaar,
                    nl.Type.GetDescription(),
                    da,
                    nl.InrijdenTijdensGroen.ToCustomString(),
                    t1, t2, t3, t4,
                   (nl.Type == Models.Enumerations.NaloopTypeEnum.StartGroen) ? (nl.MaximaleVoorstart != null) ? nl.MaximaleVoorstart.Value.ToString() : "-" : "-",
                   (nl.Type != Models.Enumerations.NaloopTypeEnum.StartGroen) ? (nl.MaximaleVoorstart != null) ? nl.MaximaleVoorstart.Value.ToString() : "-" : "-",
                }); ;
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Intersignaalgroep_Gelijkstarten(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Intersignaalgroep_Gelijkstarten");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Intersignaalgroep_Gelijkstarten"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Van (##)",
                    "Naar ($$)",
                    "Deelconflict",
                    "Schakelbaar                     SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schgs") + "##$$" ,
                    "Fictieve o.t.                   van > naar                             T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tfo") + "##$$" ,
                    "Fictieve o.t.                   naar > van                             T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tfo") + "$$##" 
                }
            };
            foreach (var ma in c.InterSignaalGroep.Gelijkstarten)
            {
                l.Add(new List<string>
                {
                    ma.FaseVan,
                    ma.FaseNaar,
                    ma.DeelConflict.ToCustomString(),
                    ma.Schakelbaar.GetDescription(),
                    ma.DeelConflict ? ma.GelijkstartOntruimingstijdFaseVan.ToString() : "-",
                    ma.DeelConflict ? ma.GelijkstartOntruimingstijdFaseNaar.ToString() : "-",
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Intersignaalgroep_Voorstarten(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Intersignaalgroep_Voorstarten");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Intersignaalgroep_Voorstarten"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Van (##)",
                    "Op ($$)",
                    "Voorstart tijd                                                         T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tvs") + "##$$",
                    "Fictieve o.t.                     op > van                             T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tfo") + "$$##"
                }
            };
            foreach (var ma in c.InterSignaalGroep.Voorstarten)
            {
                l.Add(new List<string>
                {
                    ma.FaseVan,
                    ma.FaseNaar,
                    ma.VoorstartTijd.ToString(),
                    ma.VoorstartOntruimingstijd.ToString()
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Intersignaalgroep_LateRelease(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Intersignaalgroep_LateRelease");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Intersignaalgroep_LateRelease"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Van (##)",
                    "Na ($$)",
                    "Late release tijd                                                         T " + CCOLGeneratorSettingsProvider.Default.GetElementName("treallr") + "##$$",
                    "Fictieve o.t.                        na > van                             T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tfo") + "$$##"
                }
            };
            foreach (var ma in c.InterSignaalGroep.LateReleases)
            {
                l.Add(new List<string>
                {
                    ma.FaseVan,
                    ma.FaseNaar,
                    ma.LateReleaseTijd.ToString(),
                    ma.LateReleaseOntruimingstijd.ToString()
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Prio_InUit(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            foreach (PrioIngreepVoertuigTypeEnum voertuigtype in Enum.GetValues(typeof(PrioIngreepVoertuigTypeEnum)))
            {
                if (c.PrioData.PrioIngrepen.Exists(x => x.Type == voertuigtype &&
                                              (x.MeldingenData. Inmeldingen.Any(y => y.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde) ||
                                               x.MeldingenData.Uitmeldingen.Any(y => y.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))))
                {
                    UpdateTables("In-uitmeldingen" + voertuigtype);
                    items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: Instellingen tbv in- en uitmeldingen voor voertuigtype '{voertuigtype}'", styleid: "Caption"));

                    if (voertuigtype == PrioIngreepVoertuigTypeEnum.Tram &&
                        c.PrioData.PrioIngrepen.Exists(x => x.Type == PrioIngreepVoertuigTypeEnum.Tram && x.MeldingenData.Inmeldingen.Any(y => y.KijkNaarWisselStand)))
                    {
                        var l = new List<List<string>>
                        {
                            new List<string>
                            {
                                "Fase (##)",
                                "In- of Uitmelding",
                                "Via",
                                "Melding trigger",
                                "Alleen indien rood",
                                "Alleen indien geen inmelding aanwezig",
                                "Afhankelijk van wisselstand",
                                "Ingang wissel 1",
                                "Ingang wissel 2",
                            }
                        };
                        var oldfc = "";
                        foreach (var prio in c.PrioData.PrioIngrepen.Where(x => x.Type == voertuigtype &&
                                                      (x.MeldingenData.Inmeldingen.Any(y => y.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde) ||
                                                       x.MeldingenData.Uitmeldingen.Any(y => y.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))))
                        {
                            var fc = prio.FaseCyclus;

                            foreach (var melding in prio.MeldingenData.Inmeldingen.Where(x => x.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))
                            {
                                var wisseldc1 = prio.MeldingenData.Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Detector ? prio.MeldingenData.Wissel1Detector :
                                                prio.MeldingenData.Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Ingang ? prio.MeldingenData.Wissel1Input : "--";
                                var wisseldc2 = prio.MeldingenData.Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Detector ? prio.MeldingenData.Wissel2Detector :
                                                prio.MeldingenData.Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Ingang ? prio.MeldingenData.Wissel2Input : "--";

                                l.Add(new List<string>
                                {
                                   (fc != oldfc) ? prio.FaseCyclus.ToString() : "",
                                   (melding.InUit.GetDescription() == "Inmelding") ? "In" : "Uit",
                                   (melding.Type.GetDescription() == "Detector(en)") ? "Detectie" : ((melding.Type.GetDescription() == "KAR DSI melding") ? "KAR" : melding.Type.GetDescription()),
                                   (melding.Type.GetDescription() == "Detector(en)") ? melding.RelatedInput1Type.GetDescription() + " " + melding.RelatedInput1
                                                      + (melding.TweedeInput ? " EN " + melding.RelatedInput2Type.GetDescription() + " " + melding.RelatedInput2 : "")
                                                                                       : (melding.Type.GetDescription() == "KAR DSI melding") ? "KAR DSI bericht" :
                                                                                       melding.Type.GetDescription() == "Selectieve detector" ? melding.RelatedInput1 : "-",
                                    melding.AlleenIndienRood.ToCustomString(),
                                    melding.AlleenIndienGeenInmelding.ToCustomString(),
                                    melding.KijkNaarWisselStand.ToCustomString(),
                                    melding.KijkNaarWisselStand && prio.MeldingenData.Wissel1 ? prio.MeldingenData.Wissel1InputVoorwaarde.ToCustomStringHL() + " (" + wisseldc1 + ")" : "-",
                                    melding.KijkNaarWisselStand && prio.MeldingenData.Wissel2 ? prio.MeldingenData.Wissel2InputVoorwaarde.ToCustomStringHL() + " (" + wisseldc2 + ")" : "-",
                                });
                                if (melding.OpvangStoring && melding.MeldingBijstoring != null)
                                {
                                    var st1 = melding.MeldingBijstoring;
                                    l.Add(new List<string>
                                    {
                                       "",
                                       "",
                                       "   - storingsopvang",
                                       (st1.Type.GetDescription() == "Detector(en)") ? st1.RelatedInput1Type.GetDescription() + " " + st1.RelatedInput1
                                                          + (st1.TweedeInput ? " EN " + st1.RelatedInput2Type.GetDescription() + " " + st1.RelatedInput2 : "")
                                                                                           : (st1.Type.GetDescription() == "KAR DSI melding") ? "KAR DSI bericht" : "-",
                                        st1.AlleenIndienRood.ToCustomString(),
                                        st1.AlleenIndienGeenInmelding.ToCustomString(),
                                        st1.KijkNaarWisselStand.ToCustomString(),
                                        st1.KijkNaarWisselStand && prio.MeldingenData.Wissel1 ? prio.MeldingenData.Wissel1InputVoorwaarde.ToCustomStringHL() + " (" + wisseldc1 + ")" : "-",
                                        st1.KijkNaarWisselStand && prio.MeldingenData.Wissel2 ? prio.MeldingenData.Wissel2InputVoorwaarde.ToCustomStringHL() + " (" + wisseldc2 + ")" : "-",
                                    });
                                    if (st1.OpvangStoring && st1.MeldingBijstoring != null)
                                    {
                                        var st2 = st1.MeldingBijstoring;
                                        l.Add(new List<string>
                                        {
                                           "",
                                           "",
                                           "   - storingsopvang",
                                           (st2.Type.GetDescription() == "Detector(en)") ? st2.RelatedInput1Type.GetDescription() + " " + st2.RelatedInput1
                                                              + (st2.TweedeInput ? " EN " + st2.RelatedInput2Type.GetDescription() + " " + st2.RelatedInput2 : "")
                                                                                               : (st2.Type.GetDescription() == "KAR DSI melding") ? "KAR DSI bericht" : "-",
                                            st2.AlleenIndienRood.ToCustomString(),
                                            st2.AlleenIndienGeenInmelding.ToCustomString(),
                                            st2.KijkNaarWisselStand.ToCustomString(),
                                            st2.KijkNaarWisselStand && prio.MeldingenData.Wissel1 ? prio.MeldingenData.Wissel1InputVoorwaarde.ToCustomStringHL() + " (" + wisseldc1 + ")" : "-",
                                            st2.KijkNaarWisselStand && prio.MeldingenData.Wissel2 ? prio.MeldingenData.Wissel2InputVoorwaarde.ToCustomStringHL() + " (" + wisseldc2 + ")" : "-",
                                        });

                                    }
                                }
                                oldfc = fc;
                            }

                            foreach (var melding in prio.MeldingenData.Uitmeldingen.Where(x => x.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))
                            {
                                var wisseldc1 = prio.MeldingenData.Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Detector ? prio.MeldingenData.Wissel1Detector :
                                                prio.MeldingenData.Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Ingang ? prio.MeldingenData.Wissel1Input : "--";
                                var wisseldc2 = prio.MeldingenData.Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Detector ? prio.MeldingenData.Wissel2Detector :
                                                prio.MeldingenData.Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Ingang ? prio.MeldingenData.Wissel2Input : "--";

                                l.Add(new List<string>
                                {
                                   (fc != oldfc) ? prio.FaseCyclus.ToString() : "",
                                   (melding.InUit.GetDescription() == "Inmelding") ? "In" : "Uit",
                                   (melding.Type.GetDescription() == "Detector(en)") ? "Detectie" : ((melding.Type.GetDescription() == "KAR DSI melding") ? "KAR" : melding.Type.GetDescription()),
                                   (melding.Type.GetDescription() == "Detector(en)") ? melding.RelatedInput1Type.GetDescription() + " " + melding.RelatedInput1
                                                      + (melding.TweedeInput ? " EN " + melding.RelatedInput2Type.GetDescription() + " " + melding.RelatedInput2 : "")
                                                                                       : (melding.Type.GetDescription() == "KAR DSI melding") ? "KAR DSI bericht" :
                                                                                       melding.Type.GetDescription() == "Selectieve detector" ? melding.RelatedInput1 : "-",
                                    melding.AlleenIndienRood.ToCustomString(),
                                    melding.AlleenIndienGeenInmelding.ToCustomString(),
                                    melding.KijkNaarWisselStand.ToCustomString(),
                                    melding.KijkNaarWisselStand && prio.MeldingenData.Wissel1 ? prio.MeldingenData.Wissel1InputVoorwaarde.ToCustomStringHL() + " (" + wisseldc1 + ")" : "-",
                                    melding.KijkNaarWisselStand && prio.MeldingenData.Wissel2 ? prio.MeldingenData.Wissel2InputVoorwaarde.ToCustomStringHL() + " (" + wisseldc2 + ")" : "-",
                                });
                                if (melding.OpvangStoring && melding.MeldingBijstoring != null)
                                {
                                    var st1 = melding.MeldingBijstoring;
                                    l.Add(new List<string>
                                    {
                                       "",
                                       "",
                                       "   - storingsopvang",
                                       (st1.Type.GetDescription() == "Detector(en)") ? st1.RelatedInput1Type.GetDescription() + " " + st1.RelatedInput1
                                                          + (st1.TweedeInput ? " EN " + st1.RelatedInput2Type.GetDescription() + " " + st1.RelatedInput2 : "")
                                                                                           : (st1.Type.GetDescription() == "KAR DSI melding") ? "KAR DSI bericht" : "-",
                                        st1.AlleenIndienRood.ToCustomString(),
                                        st1.AlleenIndienGeenInmelding.ToCustomString(),
                                        st1.KijkNaarWisselStand.ToCustomString(),
                                        st1.KijkNaarWisselStand && prio.MeldingenData.Wissel1 ? prio.MeldingenData.Wissel1InputVoorwaarde.ToCustomStringHL() + " (" + wisseldc1 + ")" : "-",
                                        st1.KijkNaarWisselStand && prio.MeldingenData.Wissel2 ? prio.MeldingenData.Wissel2InputVoorwaarde.ToCustomStringHL() + " (" + wisseldc2 + ")" : "-",
                                    });
                                    if (st1.OpvangStoring && st1.MeldingBijstoring != null)
                                    {
                                        var st2 = st1.MeldingBijstoring;
                                        l.Add(new List<string>
                                        {
                                           "",
                                           "",
                                           "   - storingsopvang",
                                           (st2.Type.GetDescription() == "Detector(en)") ? st2.RelatedInput1Type.GetDescription() + " " + st2.RelatedInput1
                                                              + (st2.TweedeInput ? " EN " + st2.RelatedInput2Type.GetDescription() + " " + st2.RelatedInput2 : "")
                                                                                               : (st2.Type.GetDescription() == "KAR DSI melding") ? "KAR DSI bericht" : "-",
                                            st2.AlleenIndienRood.ToCustomString(),
                                            st2.AlleenIndienGeenInmelding.ToCustomString(),
                                            st2.KijkNaarWisselStand.ToCustomString(),
                                            st2.KijkNaarWisselStand && prio.MeldingenData.Wissel1 ? prio.MeldingenData.Wissel1InputVoorwaarde.ToCustomStringHL() + " (" + wisseldc1 + ")" : "-",
                                            st2.KijkNaarWisselStand && prio.MeldingenData.Wissel2 ? prio.MeldingenData.Wissel2InputVoorwaarde.ToCustomStringHL() + " (" + wisseldc2 + ")" : "-",
                                        });
                                    }
                                }
                            }
                        }
                        items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
                        items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                    }
                    else
                    if (voertuigtype == PrioIngreepVoertuigTypeEnum.Fiets &&
                        c.PrioData.PrioIngrepen.Exists(x => x.MeldingenData.Inmeldingen.Any(y => y.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton)))
                    {
                        var l = new List<List<string>>
                        {
                            new List<string>
                            {
                                "Fase (##)",
                                "In- of Uitmelding",
                                "Max aantal per cyclus                       PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmftsmaxpercyc") + "##fietsfiets",
                                "Toegestaan blok (BITSgewijs)                PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmftsblok") + "##fietsfiets",
                                "Minimaal aantal fietsers                    PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmftsminvtg") + "##fietsfiets",
                                "Minimale wachttijd                          PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmftsminwt") + "##fietsfiets",
                                "Tel detector",
                            }
                        };
                        var oldfc = "";
                        foreach (var prio in c.PrioData.PrioIngrepen.Where(x => x.Type == voertuigtype &&
                                                      (x.MeldingenData.Inmeldingen.Any(y => y.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde) ||
                                                       x.MeldingenData.Uitmeldingen.Any(y => y.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))))
                        {
                            var fc = prio.FaseCyclus;

                            foreach (var melding in prio.MeldingenData.Inmeldingen.Where(x => x.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))
                            {
                                var mls = "";
                                var blk = melding.FietsPrioriteitBlok;
                                var blkk = blk;
                                
                                bool bit0 = false, bit1 = false, bit2 = false, bit3 = false, bit4 = false,
                                     bit5 = false, bit6 = false, bit7 = false, bit8 = false, bit9 = false;
                                
                                if (blkk >= 512) { bit9 = true; blkk %= 512; }
                                if (blkk >= 256) { bit8 = true; blkk %= 256; }
                                if (blkk >= 128) { bit7 = true; blkk %= 128; }
                                if (blkk >=  64) { bit6 = true; blkk %=  64; }
                                if (blkk >=  32) { bit5 = true; blkk %=  32; }
                                if (blkk >=  16) { bit4 = true; blkk %=  16; }
                                if (blkk >=   8) { bit3 = true; blkk %=   8; }
                                if (blkk >=   4) { bit2 = true; blkk %=   4; }
                                if (blkk >=   2) { bit1 = true; blkk %=   2; }
                                if (blkk >=   1) { bit0 = true; blkk %=   1; }
                                
                                if (bit0) mls +=                         "ML1";
                                if (bit1) mls += (mls == "") ? "ML2" : ", ML2";
                                if (bit2) mls += (mls == "") ? "ML3" : ", ML3";
                                if (bit3) mls += (mls == "") ? "ML4" : ", ML4";
                                if (bit4) mls += (mls == "") ? "ML5" : ", ML5";
                                if (bit5) mls += (mls == "") ? "ML6" : ", ML6";
                                if (bit6) mls += (mls == "") ? "ML7" : ", ML7";
                                if (bit7) mls += (mls == "") ? "ML8" : ", ML8";
                                if (bit8) mls += (mls == "") ? "ML9" : ", ML9";
                                
                                mls = (blk > 0) ? " (" + mls + ")" : "";

                                l.Add(new List<string>
                                {
                                   (fc != oldfc) ? prio.FaseCyclus.ToString() : "",
                                   (melding.InUit.GetDescription() == "Inmelding") ? "In" : "Uit",
                                   melding.FietsPrioriteitAantalKeerPerCyclus.ToString(),
                                   blk + mls,
                                   melding.FietsPrioriteitMinimumAantalVoertuigen.ToString(),
                                   melding.FietsPrioriteitMinimumWachttijdVoorPrioriteit.ToString(),
                                   melding.FietsPrioriteitGebruikLus ? melding.RelatedInput1 : "-",
                                });
                                oldfc = fc;
                            }
                        }
                        items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
                        items.Add(OpenXmlHelper.GetTextParagraph($"Uitmelding voor prioriteit {voertuigtype} vindt plaats op basis van groenbewaking.", "Footer"));
                        items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                    }
                    else
                    {
                        var l = new List<List<string>>
                        {
                            new List<string>
                            {
                                "Fase (##)",
                                "In- of Uitmelding",
                                "Via",
                                "Melding trigger",
                                "Alleen indien rood",
                                "Alleen indien geen inmelding aanwezig",
                            }
                        };
                        var oldfc = "";
                        foreach (var prio in c.PrioData.PrioIngrepen.Where(x => x.Type == voertuigtype &&
                                                      (x.MeldingenData.Inmeldingen.Any(y => y.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde) ||
                                                       x.MeldingenData.Uitmeldingen.Any(y => y.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))))
                        {
                            var fc = prio.FaseCyclus;

                            foreach (var melding in prio.MeldingenData.Inmeldingen.Where(x => x.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))
                            {
                                l.Add(new List<string>
                                {
                                   (fc != oldfc) ? prio.FaseCyclus.ToString() : "",
                                   (melding.InUit.GetDescription() == "Inmelding") ? "In" : "Uit",
                                   (melding.Type.GetDescription() == "Detector(en)") ? "Detectie" : ((melding.Type.GetDescription() == "KAR DSI melding") ? "KAR" : melding.Type.GetDescription()),
                                   (melding.Type.GetDescription() == "Detector(en)") ? melding.RelatedInput1Type.GetDescription() + " " + melding.RelatedInput1
                                                      + (melding.TweedeInput ? " EN " + melding.RelatedInput2Type.GetDescription() + " " + melding.RelatedInput2 : "")
                                                                                       : (melding.Type.GetDescription() == "KAR DSI melding") ? "KAR DSI bericht" : "-",
                                    melding.AlleenIndienRood.ToCustomString(),
                                    melding.AlleenIndienGeenInmelding.ToCustomString(),
                                });
                                if (melding.OpvangStoring && melding.MeldingBijstoring != null)
                                {
                                    var st1 = melding.MeldingBijstoring;
                                    l.Add(new List<string>
                                    {
                                       "",
                                       "",
                                       "   - storingsopvang",
                                       (st1.Type.GetDescription() == "Detector(en)") ? st1.RelatedInput1Type.GetDescription() + " " + st1.RelatedInput1
                                                          + (st1.TweedeInput ? " EN " + st1.RelatedInput2Type.GetDescription() + " " + st1.RelatedInput2 : "")
                                                                                           : (st1.Type.GetDescription() == "KAR DSI melding") ? "KAR DSI bericht" : "-",
                                        st1.AlleenIndienRood.ToCustomString(),
                                        st1.AlleenIndienGeenInmelding.ToCustomString(),
                                    });
                                    if (st1.OpvangStoring && st1.MeldingBijstoring != null)
                                    {
                                        var st2 = st1.MeldingBijstoring;
                                        l.Add(new List<string>
                                        {
                                           "",
                                           "",
                                           "   - storingsopvang",
                                           (st2.Type.GetDescription() == "Detector(en)") ? st2.RelatedInput1Type.GetDescription() + " " + st2.RelatedInput1
                                                              + (st2.TweedeInput ? " EN " + st2.RelatedInput2Type.GetDescription() + " " + st2.RelatedInput2 : "")
                                                                                               : (st2.Type.GetDescription() == "KAR DSI melding") ? "KAR DSI bericht" : "-",
                                            st2.AlleenIndienRood.ToCustomString(),
                                            st2.AlleenIndienGeenInmelding.ToCustomString(),
                                        });
                                    }
                                }
                                oldfc = fc;
                            }

                            foreach (var melding in prio.MeldingenData.Uitmeldingen.Where(x => x.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))
                            {
                                l.Add(new List<string>
                                {
                                   (fc != oldfc) ? prio.FaseCyclus.ToString() : "",
                                   (melding.InUit.GetDescription() == "Inmelding") ? "In" : "Uit",
                                   (melding.Type.GetDescription() == "Detector(en)") ? "Detectie" : ((melding.Type.GetDescription() == "KAR DSI melding") ? "KAR" : melding.Type.GetDescription()),
                                   (melding.Type.GetDescription() == "Detector(en)") ? melding.RelatedInput1Type.GetDescription() + " " + melding.RelatedInput1
                                                      + (melding.TweedeInput ? " EN " + melding.RelatedInput2Type.GetDescription() + " " + melding.RelatedInput2 : "")
                                                                                       : (melding.Type.GetDescription() == "KAR DSI melding") ? "KAR DSI bericht" : "-",
                                    melding.AlleenIndienRood.ToCustomString(),
                                    melding.AlleenIndienGeenInmelding.ToCustomString(),
                                });
                                if (melding.OpvangStoring && melding.MeldingBijstoring != null)
                                {
                                    var st1 = melding.MeldingBijstoring;
                                    l.Add(new List<string>
                                    {
                                       "",
                                       "",
                                       "   - storingsopvang",
                                       (st1.Type.GetDescription() == "Detector(en)") ? st1.RelatedInput1Type.GetDescription() + " " + st1.RelatedInput1
                                                          + (st1.TweedeInput ? " EN " + st1.RelatedInput2Type.GetDescription() + " " + st1.RelatedInput2 : "")
                                                                                           : (st1.Type.GetDescription() == "KAR DSI melding") ? "KAR DSI bericht" : "-",
                                        st1.AlleenIndienRood.ToCustomString(),
                                        st1.AlleenIndienGeenInmelding.ToCustomString(),
                                    });
                                    if (st1.OpvangStoring && st1.MeldingBijstoring != null)
                                    {
                                        var st2 = st1.MeldingBijstoring;
                                        l.Add(new List<string>
                                        {
                                           "",
                                           "",
                                           "   - storingsopvang",
                                           (st2.Type.GetDescription() == "Detector(en)") ? st2.RelatedInput1Type.GetDescription() + " " + st2.RelatedInput1
                                                              + (st2.TweedeInput ? " EN " + st2.RelatedInput2Type.GetDescription() + " " + st2.RelatedInput2 : "")
                                                                                               : (st2.Type.GetDescription() == "KAR DSI melding") ? "KAR DSI bericht" : "-",
                                            st2.AlleenIndienRood.ToCustomString(),
                                            st2.AlleenIndienGeenInmelding.ToCustomString(),
                                        });
                                    }
                                }
                            }
                        }
                        items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
                        items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                    }
                }
            }

            if (c.PrioData.PrioIngrepen.Exists(x => !x.MeldingenData.Inmeldingen.Any() && !x.MeldingenData.Uitmeldingen.Any()))
            {
                UpdateTables("Prio zonder in-uitmeldingen");
                items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: Overzicht van ingrepen zonder gestandaardiseerde in- of uitmelding", styleid: "Caption"));

                var l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Fase (##)",
                        "Voertuigtype",
                    }
                };
                foreach (var prio in c.PrioData.PrioIngrepen.Where(x => !x.MeldingenData.Inmeldingen.Any() && !x.MeldingenData.Uitmeldingen.Any()))
                {
                    l.Add(new List<string>
                    {
                        prio.FaseCyclus.ToString(),
                        prio.Type.GetDescription(),
                    });
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: false));
                items.Add(OpenXmlHelper.GetTextParagraph("Van deze ingrepen zijn mogelijk in de .add bestanden handmatig in- of uitmeldingen geconfigureerd.", "Footer"));
                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            }
            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_OV_Lijnnummers(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_OV_Lijnnummers");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_OV_Lijnnummers"], styleid: "Caption"));
            
            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Richting (##)",
                    "Toegestane Lijnnummers"
                }
            };
            foreach (var ln in c.PrioData.PrioIngrepen.Where(x => x.CheckLijnNummer))
            {
                var lnrsbestaan = ln.LijnNummers.Any(x => x.Nummer != "0");

                l.Add(new List<string>
                {
                    ln.FaseCyclus,
                    lnrsbestaan ? ln.LijnNummers.Select(x => x.Nummer).Where(p => !p.Equals("0")).Aggregate((y, z) => y + ", " + z) : "niet opgegeven",
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: false));

            items.Add(OpenXmlHelper.GetTextParagraph("De toegestane lijnnummers worden (per richting ##) opgeslagen in parameters: " +
                    "PRM lijn##bus_01 voor het eerste lijnnummer van richting ##, lijn##bus_02 voor het tweede lijnnummer, enz", "Footer"));
            items.Add(OpenXmlHelper.GetTextParagraph("Om het filter uit te schakelen kan per richting PRM allelijnen##bus op 1 gezet worden.", "Footer"));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_OV_PrioriteitsOpties(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_OV_PrioriteitsOpties");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_OV_PrioriteitsOpties"], styleid: "Caption"));

            var l = new List<List<string>> { };

            if (!c.HasPT())
            {
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Optie ",
                        "Toelichting"
                    }
                };
            }
            else
            {
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Optie (PRM prio##xyz)",
                        "Toelichting"
                    }
                };
            }
            l.Add(new List<string> { "0", "Geen prioriteit" });
            l.Add(new List<string> { "1", "Aanvragen en afkappen conflicterende richtingen" });
            l.Add(new List<string> { "2", "Aanvragen en vasthouden van het groen" });
            l.Add(new List<string> { "3", "Aanvragen en bijzonder realiseren" });
            l.Add(new List<string> { "4", "Aanvragen, afkappen conflicterende richtingen en afkappen conflicterende OV richtingen" });
            l.Add(new List<string> { "5", "Nooddienst: Omvat alle voorgaande prioriteitsopties en maakt die dus overbodig. Verder " +
                "wordt er niet gekeken naar het ondermaximum, overschrijding van de maximum wachttijd, of blokkeringstijd. Conflicten " +
                "mogen worden afgekapt na garantiegroen. Tijdens een nooddienstingreep worden de fasebewakingstijden gereset." });

            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: false));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_OV_PrioriteitsInstellingen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_OV_PrioriteitsInstellingen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_OV_PrioriteitsInstellingen"], styleid: "Caption"));

            List<List<string>> l;
            if (c.PrioData.PrioIngrepen.Any(x => x.GeconditioneerdePrioriteit != Models.Enumerations.NooitAltijdAanUitEnum.Nooit))
            {
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Signaalgroep (##)",
                        "Geconditioneerde prio",
                        "Gec. Te vroeg    PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmovstipttevroeg") + "##bus",
                        "Gec. Op tijd       PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmovstiptoptijd") + "##bus",
                        "Gec. Te laat       PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmovstipttelaat") + "##bus",
                        "Ongecond.                        PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmprio") + "##bus",
                        "Rijtijd ongeh.                   PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrto") + "##bus",
                        "Rijtijd bep. geh.                PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtbg") + "##bus",
                        "Rijtijd geh.                     PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtg") + "##bus",
                        "Bezettijd geh.                   T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tbtovg") + "##bus",
                        "Blokkeringstijd                  T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tblk") + "##bus",
                        "Groenbewaking                    T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tgb") + "##bus",
                        "Ondermaximum                     PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmomx") + "##bus"
                    }
                };
                foreach (var ov in c.PrioData.PrioIngrepen.Where(x => x.HasOVIngreepDSI() || x.HasOVIngreepVecom() ||
                                                                      x.HasOVIngreepVecomIO() || x.HasOVIngreepWissel() || x.HasPrioIngreepKAR() ))
                {
                    var cp = ov.GeconditioneerdePrioriteit != Models.Enumerations.NooitAltijdAanUitEnum.Nooit;
                    var opties = 0;
                    if (ov.AfkappenConflicten || ov.AfkappenConflictenPrio) opties += 100;
                    if (ov.AfkappenConflictenPrio) opties += 300;
                    if (ov.TussendoorRealiseren) opties += 3;
                    if (ov.VasthoudenGroen) opties += 20;
                    var sopties = opties == 0 ? "0" : opties.ToString().Replace("0", "");
                    l.Add(new List<string>
                    {
                        ov.FaseCyclus,
                        ov.GeconditioneerdePrioriteit.GetDescription(),
                        cp ? ov.GeconditioneerdePrioTeVroeg.ToString() : "-",
                        cp ? ov.GeconditioneerdePrioOpTijd.ToString() : "-",
                        cp ? ov.GeconditioneerdePrioTeLaat.ToString() : "-",
                        sopties,
                        ov.RijTijdOngehinderd.ToString(),
                        ov.RijTijdBeperktgehinderd.ToString(),
                        ov.RijTijdGehinderd.ToString(),
                        ov.BezettijdPrioGehinderd.ToString(),
                        ov.BlokkeertijdNaPrioIngreep.ToString(),
                        ov.GroenBewaking.ToString(),
                        ov.OnderMaximum.ToString()
                    });
                }
            }
            else
            {
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Signaalgroep",
                        "Prioriteitsopties",
                        "Ongehinderde rijtijd",
                        "Beperkt rijtijd",
                        "Gehinderde rijtijd",
                        "Bezettijd OV gehinderd",
                        "Blokkeringstijd",
                        "Groenbewaking",
                        "Ondermaximum"
                    }
                };
                foreach (var ov in c.PrioData.PrioIngrepen)
                {
                    var cp = ov.GeconditioneerdePrioriteit != Models.Enumerations.NooitAltijdAanUitEnum.Nooit;
                    var opties = 0;
                    if (ov.AfkappenConflicten || ov.AfkappenConflictenPrio) opties += 100;
                    if (ov.AfkappenConflictenPrio) opties += 300;
                    if (ov.TussendoorRealiseren) opties += 3;
                    if (ov.VasthoudenGroen) opties += 20;
                    var sopties = opties == 0 ? "0" : opties.ToString().Replace("0", "");
                    l.Add(new List<string>
                    {
                        ov.FaseCyclus,
                        sopties,
                        ov.RijTijdOngehinderd.ToString(),
                        ov.RijTijdBeperktgehinderd.ToString(),
                        ov.RijTijdGehinderd.ToString(),
                        ov.BezettijdPrioGehinderd.ToString(),
                        ov.BlokkeertijdNaPrioIngreep.ToString(),
                        ov.GroenBewaking.ToString(),
                        ov.OnderMaximum.ToString()
                    });
                }
            }

            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_OV_ConflictenInstellingen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_OV_ConflictenInstellingen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_OV_ConflictenInstellingen"], styleid: "Caption"));

            List<List<string>> l;
            l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Signaalgroep (##)",
                        "Afkapgarantie                              PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmgcov") + "##",
                        "Perc. maxgroen                             PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmpmgcov") + "##",
                        "Ophoogperc. maxgroen                       PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmohpmg") + "##",
                        "Aant. niet afbreken                        PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmnofm") + "##",
                        "Perc. groen voor terugkeren                PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmpmgt") + "##",
                        "Ondergrens voor terugkomen                 PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmognt") + "##",
                    }
                };
            foreach (var ovcf in c.PrioData.PrioIngreepSignaalGroepParameters)
            {
                l.Add(new List<string>
                {
                    ovcf.FaseCyclus,
                    ovcf.MinimumGroentijdConflictOVRealisatie.ToString(),
                    ovcf.PercMaxGroentijdConflictOVRealisatie.ToString(),
                    ovcf.OphoogpercentageNaAfkappen.ToString(),
                    ovcf.AantalKerenNietAfkappen.ToString(),
                    ovcf.PercMaxGroentijdVoorTerugkomen.ToString(),
                    ovcf.OndergrensNaTerugkomen.ToString()
                });
            }

            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_HD_Instellingen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_HD_Instellingen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_HD_Instellingen"], styleid: "Caption"));

            var l = new List<List<string>>();
            var ll = new List<string> { "Signaalgroep (##)" };
            if (c.HasHDKAR())
            {
                ll.Add("HD via KAR");
                ll.Add("KAR inmeld filtertijd                T " + CCOLGeneratorSettingsProvider.Default.GetElementName("thdin") + "##kar");
                ll.Add("KAR uitmeld filtertijd               T " + CCOLGeneratorSettingsProvider.Default.GetElementName("thduit") + "##kar");
                if (c.PrioData.HDIngrepen.Any(x => x.InmeldingOokDoorToepassen))
                {
                    ll.Add("Ook inmelden door KAR richting");
                }
            }
            ll.Add("Check sirene                             SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schchecksirene") + "##");
            ll.Add("Rijtijd ongehinderd                      PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrto") + "hd##");
            ll.Add("Rijtijd bep. gehinderd                   PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtbg") + "hd##");
            ll.Add("Rijtijd gehinderd                        PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrtg") + "hd##");
            ll.Add("Groenbewaking                            T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tgb") + "hd##");

            if (c.HasHDOpticom())
            {
                ll.Add("Opticom");
                ll.Add("Opticom inmeld filtertijd");
            }
            if (c.PrioData.HDIngrepen.Any(x => x.MeerealiserendeFaseCycli.Any()))
            {
                ll.Add("Meerealiserende fasen");
            }
            l.Add(ll);
            foreach (var ovcf in c.PrioData.HDIngrepen)
            {
                ll = new List<string> { ovcf.FaseCyclus };
                if (c.HasHDKAR())
                {
                    ll.Add(ovcf.KAR.ToCustomString());
                    ll.Add(ovcf.KARInmeldingFilterTijd.ToString());
                    ll.Add(ovcf.KARUitmeldingFilterTijd.ToString());
                    if (c.PrioData.HDIngrepen.Any(x => x.InmeldingOokDoorToepassen))
                    {
                        ll.Add(
                            (ovcf.InmeldingOokDoorToepassen && ovcf.KAR)?
                            string.Join(", ", ovcf.InmeldingOokDoorFase.ToString()) :
                            "-");
                    }
                }
                ll.Add(ovcf.Sirene.ToCustomString());
                ll.Add(ovcf.RijTijdOngehinderd.ToString());
                ll.Add(ovcf.RijTijdBeperktgehinderd.ToString());
                ll.Add(ovcf.RijTijdGehinderd.ToString());
                ll.Add(ovcf.GroenBewaking.ToString());
                if (c.HasHDOpticom())
                {
                    ll.Add(ovcf.Opticom.ToCustomString() + (ovcf.Opticom ? $" [{ovcf.OpticomRelatedInput}]" : ""));
                    ll.Add(ovcf.OpticomInmeldingFilterTijd.ToString());
                }
                if (c.PrioData.HDIngrepen.Any(x => x.MeerealiserendeFaseCycli.Any()))
                {
                    ll.Add(
                        ovcf.MeerealiserendeFaseCycli.Any() ?
                        string.Join(", ", ovcf.MeerealiserendeFaseCycli.Select(x => x.FaseCyclus)) :
                        "-");
                }
                l.Add(ll);
            }

            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Halfstar_configuratie(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_HS_Configuratie");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_HS_Configuratie"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Type regeling",
                    "Type VA regelen          (1=ML, 0=versneld PL)            SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schvaml"),
                    "Altijd VA regelen (deze regeling)                    SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schvar"),
                    //"Gehele streng VA regelen                           SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schvarstreng") + "###"
                    "Toestaan hoofdrichtingen alternatief                 SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("scharh"),
                    "TX tijden in parameters "
                }
            };
            l.Add(new List<string>
            {
                c.HalfstarData.Type.ToString(),
                c.HalfstarData.TypeVARegelen.ToString(),
                c.HalfstarData.VARegelen.ToCustomString2(),
                c.HalfstarData.AlternatievenVoorHoofdrichtingen.ToCustomString2(),
                c.HalfstarData.PlantijdenInParameters.ToCustomStringJN(),
            });
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Halfstar_hoofdrichtingen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_HS_Hoofdrichtingen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_HS_Hoofdrichtingen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Hoofdrichting (###)",
                    "Tegenhouden door OV                            SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schtegenov") + "###",
                    "Afkappen WG door OV                            SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schafkwgov") + "###",
                    "Afkappen VG door  OV                                   SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schafkvgov") + "###"
                }
            };
            foreach (var fc in c.HalfstarData.Hoofdrichtingen)
            {
                l.Add(new List<string>
                {
                    fc.FaseCyclus,
                    fc.Tegenhouden.ToCustomString2(),
                    fc.AfkappenWG.ToCustomString2(),
                    fc.AfkappenVG.ToCustomString2(),
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Halfstar_koppelingen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_HS_Koppelingen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_HS_Koppelingen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Gekoppelde kruising",
                    "Type regeling van gekoppelde kruising",
                    "Koppelwijze",
                    "Gebruikte koppeling"
                }
            };
            foreach (var k in c.HalfstarData.GekoppeldeKruisingen)
            {
                l.Add(new List<string>
                {
                    k.KruisingNaam.ToString(),
                    k.Type.ToString(),
                    k.KoppelWijze.ToString(),
                    k.PTPKruising.ToString(),
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Halfstar_perioden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_HS_PL");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_HS_PL"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Klokperiode conform tabel " + KlokperiodenTable.ToString() + " (###)",
                    "Toegewezen signaalplan",
                    "VA regelen actief                                                SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schpervar") + "###",
                    "Toestaan hoofdrichtingen alternatief                             SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schperarh") + "###",
                }
            };
            foreach (var p in c.HalfstarData.HalfstarPeriodenData)
            {
                l.Add(new List<string>
                {
                    p.Periode,
                    p.Signaalplan,
                    p.VARegelen.ToCustomString2(),
                    p.AlternatievenVoorHoofdrichtingen.ToCustomString2(),

                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Halfstar_SG(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_HS_SG");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_HS_SG"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Richting (###)",
                    "Alternatief toestaan                                                SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schaltghst") + "###",
                    "Alternatieve ruimte                                                T " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltphst") + "###",
                    "Aanvraag bij TXB moment",
                    "PP opzetten",
                }
            };
            foreach (var fc in c.HalfstarData.FaseCyclusInstellingen)
            {
                l.Add(new List<string>
                {
                    fc.FaseCyclus,
                    fc.AlternatiefToestaan.ToCustomString2(),
                    fc.AlternatieveRuimte.ToString(),
                    fc.AanvraagOpTxB.ToCustomString(),
                    fc.PrivilegePeriodeOpzetten.ToCustomString(),
                    
                    
                   
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Halfstar_Prio(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_HS_Prio");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_HS_Prio"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Richting  (###)",
                    "Prioriteit  (PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmpriohst") + "###)",
                    "Tijd na TXD  (PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmnatxdhst") + "###)",
                }
            };
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                l.Add(new List<string>
                {
                    prio.FaseCyclus,
                    prio.HalfstarIngreepData.Prioriteit.ToString(),
                    prio.HalfstarIngreepData.GroenNaTXDTijd.ToString(),
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: false));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_PTP(ControllerModel c) 
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_PTP");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_PTP"], styleid: "Caption"));

            var l = new List<List<string>> { };

            if (c.PTPData.PTPInstellingenInParameters)
            {
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Gekoppeld kruispunt (###)",
                        "Aantal inkomend",
                        "Aantal uitgaand",
                        "Multivalent       inkomend",
                        "Multivalent       uitgaand",
                        "Poort AUTOMAAT               PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmportnr") + "###",
                        "Source                       PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmsrc") + "###",
                        "Destination                  PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmdest") + "###",
                        "Poort testomgeving",
                    }
                };
            }
            else
            {
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Gekoppeld kruispunt",
                        "Aantal inkomend",
                        "Aantal uitgaand",
                        "Multivalent       inkomend",
                        "Multivalent       uitgaand",
                        "Poort AUTOMAAT",
                        "Source",
                        "Destination",
                        "Poort testomgeving"
                    }
                };
            }
            foreach (var k in c.PTPData.PTPKoppelingen)
            {
                l.Add(new List<string>
                {
                    k.TeKoppelenKruispunt.ToString(),
                    k.AantalsignalenIn.ToString(),
                    k.AantalsignalenUit.ToString(),
                    k.AantalsignalenMultivalentIn.ToString(),
                    k.AantalsignalenMultivalentUit.ToString(),
                    k.PortnummerAutomaatOmgeving.ToString(),
                    k.NummerSource.ToString(),
                    k.NummerDestination.ToString(),
                    k.PortnummerSimuatieOmgeving.ToString(),
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_FileIngreep(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
            bool eersterijgeweest = false;

            UpdateTables("Table_FileIngrepen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_FileIngrepen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "File ingreep (###)",
                    "Locatie",
                    "Filemelding                     SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schfile") + "###",
                    "Meting per lus                  SCH file###" + CCOLGeneratorSettingsProvider.Default.GetElementName("schparlus"),
                    "Meting per rijstrook            SCH file###" + CCOLGeneratorSettingsProvider.Default.GetElementName("schparstrook"),
                    "Afval vertraging                T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tafv") + "###",
                    "Eerl. doseren  SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("scheerlijkdoseren") + "###",
                    "Doseren                         SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schfiledoseren") + "###",
                    "Afw. groentijdenset             SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schfilealtgset") + "###",
                    "Groentijdenset bij file",
                    "File detectie",
                    "Te doseren richtingen"
                }
            };
            foreach (var fi in c.FileIngrepen)
            {
                l.Add(new List<string>
                {
                    fi.Naam,
                    fi.FileMetingLocatie.GetDescription(),
                    fi.AanUit.ToCustomString2(),
                    fi.MetingPerLus.ToCustomStringJN(),
                    fi.MetingPerStrook.ToCustomStringJN(),
                    fi.AfvalVertraging.ToString(),
                    fi.EerlijkDoseren ? fi.EerlijkDoseren.ToCustomString() : "-",
                    fi.ToepassenDoseren.GetDescription(),
                    fi.ToepassenAlternatieveGroentijdenSet.GetDescription(),
                   (fi.ToepassenAlternatieveGroentijdenSet.GetDescription() == "Nooit") ? "-" : fi.AlternatieveGroentijdenSet.Any() ? fi.AlternatieveGroentijdenSet.ToString() : "-",
                    fi.FileDetectoren.Select(x => x.Detector).Any() ? fi.FileDetectoren.Select(x => x.Detector).Aggregate((y, z) => y + ", " + z).ToString() : "-",
                    fi.TeDoserenSignaalGroepen.Select(x => x.FaseCyclus).Any() ? fi.TeDoserenSignaalGroepen.Select(x => x.FaseCyclus).Aggregate((y, z) => y + ", " + z).ToString() : "-",
               });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
                        
            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
          
            var doseerfc = 0;
            var m = new List<List<string>>
            {
                new List<string>
                {
                    "File ingreep",
                    "Beïnvloedde                  richting (##)",
                    "Doseer                       percentage                       PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmfperc") + "##",
                    "Afkappen op                  start file",
                    "Min.groen                    t.b.v. afkappen                  T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tafkmingroen") + "##",
                    "Min.rood                     bij file",
                    "Minimale                     roodtijd                         T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tminrood") + "##",
                    "Max.groen                    bij file",
                    "Maximale                     groentijd                        T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tmaxgroen") + "##",
                }
            };
            foreach (var fi in c.FileIngrepen)
            {
                doseerfc = 0;
                foreach (var fm in fi.TeDoserenSignaalGroepen)
                {
                    ++doseerfc;
                    if ((doseerfc == 1) && eersterijgeweest)
                    {
                        m.Add(new List<string> { "", "", "", "", "", "", "", "", "" });
                    }
                    m.Add(new List<string>
                    {
                        (doseerfc == 1) ? fi.Naam : "",
                        fm.FaseCyclus,
                        fm.DoseerPercentage.ToString(),
                        fm.AfkappenOpStartFile.ToCustomString(),
                        fm.AfkappenOpStartFile ? fm.AfkappenOpStartFileMinGroentijd.ToString() : "-",
                        fm.MinimaleRoodtijd.ToCustomString(),
                        fm.MinimaleRoodtijd ? fm.MinimaleRoodtijdTijd.ToString() : "-",
                        fm.MaximaleGroentijd.ToCustomString(),
                        fm.MaximaleGroentijd ? fm.MaximaleGroentijdTijd.ToString() : "-"
                    });
                    eersterijgeweest = true;
                }
            }
            items.Add(OpenXmlHelper.GetTable(m, firstRowVerticalText: true));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            eersterijgeweest = false;
            var n = new List<List<string>>
            {
                new List<string>
                {
                    "File ingreep",
                    "Detector (###)",
                    "Bezettijd                            T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tbz") + "###",
                    "Rijtijd                              T " + CCOLGeneratorSettingsProvider.Default.GetElementName("trij") + "###",
                    "AfvalVertraging                      T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tafv") + "###",
                }
            };
            foreach (var fi in c.FileIngrepen)
            {
                doseerfc = 0;
                foreach (var fd in fi.FileDetectoren)
                {
                    ++doseerfc;
                    if ((doseerfc == 1) && eersterijgeweest)
                    {
                        n.Add(new List<string> { "", "", "", "", "" });
                    }
                    n.Add(new List<string>
                    {
                        (doseerfc == 1) ? fi.Naam : "",
                        fd.Detector,
                        fd.BezetTijd.ToString(),
                        fd.RijTijd.ToString(),
                        fd.AfvalVertraging.ToString()
                    });
                    eersterijgeweest = true;
                }
            }
            items.Add(OpenXmlHelper.GetTable(n, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_VAontr(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
            bool eersterijgeweest = false;

            UpdateTables("Table_VAontruimen");

            var l = new List<List<string>> { };

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_VAontruimen"], styleid: "Caption"));

            if (c.VAOntruimenFasen.Any(x => x.KijkNaarWisselstand))
            {
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Bron " + (string)Texts["Generic_Fase"] + " (##)",
                        "Max VA Ontruimen                                          T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tvamax") + "##",
                        "Afh. van wisselstand",
                        "Detectie (***)",
                        "Beïnvoedde richtingen ($$)",
                        "Timers per fase                                           T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tva") + "##$$_*** "
                    }
                };
                foreach (var vao in c.VAOntruimenFasen)
                {
                    var detcount = 0;
                    var cfcount = 0;
                    var aantaldet = 0;
                    var aantalcf = 0;

                    if (vao.VADetectoren.Any())
                    {
                        aantaldet = vao.VADetectoren.Count;
                        foreach (var d in vao.VADetectoren)
                        {
                            ++detcount;
                            aantalcf = d.ConflicterendeFasen.Count;
                            cfcount = 0;
                            foreach (var f in d.ConflicterendeFasen)
                            {
                                ++cfcount;
                                if ((detcount == 1) && (cfcount == 1) && eersterijgeweest)
                                {
                                    l.Add(new List<string> { "", "", "", "", "", "" });
                                }
                                l.Add(new List<string>
                            {
                                ((detcount == 1) && (cfcount == 1))? vao.FaseCyclus.ToString() : "",
                                ((detcount == 1) && (cfcount == 1))? vao.VAOntrMax.ToString()  : "",
                                ((detcount == 1) && (cfcount == 1))? vao.KijkNaarWisselstand.ToCustomString() : "",
                                (cfcount == 1) ? d.Detector : "",
                                f.FaseCyclus,
                                f.VAOntruimingsTijd.ToString(),
                            });
                                eersterijgeweest = true;
                            }
                        }
                    }
                }
            }
            else
            {
                l = new List<List<string>>
            {
                new List<string>
                {
                    "Bron " + (string)Texts["Generic_Fase"] + " (##)",
                    "Max VA Ontruimen                                          T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tvamax") + "##",
                  //"Afh. van wisselstand",
                    "Detectie (***)",
                    "Beïnvloedde " + (string)Texts["Generic_Fase"] + "n ($$)",
                    "Timers per fase                                           T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tva") + "##$$_*** "
                }
            };
                foreach (var vao in c.VAOntruimenFasen)
                {
                    var detcount = 0;
                    var cfcount = 0;
                    var aantaldet = 0;
                    var aantalcf = 0;

                    if (vao.VADetectoren.Any())
                    {
                        aantaldet = vao.VADetectoren.Count;
                        foreach (var d in vao.VADetectoren)
                        {
                            ++detcount;
                            aantalcf = d.ConflicterendeFasen.Count;
                            cfcount = 0;
                            foreach (var f in d.ConflicterendeFasen)
                            {
                                ++cfcount;
                                if ((detcount == 1) && (cfcount == 1) && eersterijgeweest)
                                {
                                    l.Add(new List<string> { "", "", "", "", ""/*, ""*/ });
                                }
                                l.Add(new List<string>
                            {
                                ((detcount == 1) && (cfcount == 1))? vao.FaseCyclus.ToString() : "",
                                ((detcount == 1) && (cfcount == 1))? vao.VAOntrMax.ToString()  : "",
                              //((detcount == 1) && (cfcount == 1))? vao.KijkNaarWisselstand.ToCustomString() : "",
                                (cfcount == 1) ? d.Detector : "",
                                f.FaseCyclus,
                                f.VAOntruimingsTijd.ToString(),
                            });
                                eersterijgeweest = true;
                            }
                        }
                    }
                }
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Signalen_Perioden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Signalen_Perioden");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Signalen_Perioden"], styleid: "Caption"));

            bool prev = false;
            var l = new List<List<string>>
            {
                new List<string> { "Periode (###)",
                                   "Start klokperiode           PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmstkp") + "###",
                                   "Einde klokperiode           PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmetkp") + "###",
                                   "Dagtype                     PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmdckp") + "###",
                                   "Type periode" }
            };
            foreach (var p in c.PeriodenData.Perioden.Where(x => (x.Type == Models.Enumerations.PeriodeTypeEnum.RateltikkersAanvraag)
                                                              || (x.Type == Models.Enumerations.PeriodeTypeEnum.RateltikkersAltijd)
                                                              || (x.Type == Models.Enumerations.PeriodeTypeEnum.RateltikkersDimmen)
                                                            //|| (x.Type == Models.Enumerations.PeriodeTypeEnum.BellenActief)
                                                            //|| (x.Type == Models.Enumerations.PeriodeTypeEnum.BellenDimmen)
                                                              ))
            {
                l.Add(new List<string> { p.Naam, p.StartTijd.ToString(@"hh\:mm"), p.EindTijd.ToString(@"hh\:mm"), 
                   (p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Uitgeschakeld ?  "0: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Maandag       ?  "1: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Dinsdag       ?  "2: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Woensdag      ?  "3: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Donderdag     ?  "4: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Vrijdag       ?  "5: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Zaterdag      ?  "6: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Zondag        ?  "7: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Werkdagen     ?  "8: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Weekeind      ?  "9: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.AlleDagen     ? "10: " : "NG") + p.DagCode.GetDescription(),
                    p.Type.GetDescription() });
                prev = true;
            }

            if (prev && (c.PeriodenData.Perioden.Any(x => (x.Type == Models.Enumerations.PeriodeTypeEnum.BellenActief)
                                                       || (x.Type == Models.Enumerations.PeriodeTypeEnum.BellenDimmen)
                                                       )))
            {
                l.Add(new List<string> { "", "", "", "", "" });
            }

            foreach (var p in c.PeriodenData.Perioden.Where(x => (x.Type == Models.Enumerations.PeriodeTypeEnum.BellenActief)
                                                              || (x.Type == Models.Enumerations.PeriodeTypeEnum.BellenDimmen)
                                                              ))
            {
                l.Add(new List<string> { p.Naam, p.StartTijd.ToString(@"hh\:mm"), p.EindTijd.ToString(@"hh\:mm"),
                   (p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Uitgeschakeld ?  "0: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Maandag       ?  "1: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Dinsdag       ?  "2: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Woensdag      ?  "3: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Donderdag     ?  "4: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Vrijdag       ?  "5: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Zondag        ?  "6: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Zondag        ?  "7: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Werkdagen     ?  "8: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.Weekeind      ?  "9: " :
                    p.DagCode == Models.Enumerations.PeriodeDagCodeEnum.AlleDagen     ? "10: " : "NG") + p.DagCode.GetDescription(),
                    p.Type.GetDescription() });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            items.Add(OpenXmlHelper.GetTextParagraph("Voor het dagtype geldt: 1 t/m 7 zijn de individuele dagen maandag t/m zondag, 8 = weekdagen,                                         " +
                                              "9 = weekend, 10 = alle dagen; 0 = uitgeschakeld.", "Footer"));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Signalen_Rateltikkers(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
        
            UpdateTables("Table_Signalen_Rateltikkers");
        
            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Signalen_Rateltikkers"], styleid: "Caption"));

            var l = new List<List<string>> { };

            if (c.Signalen.DimmingNiveauVanuitApplicatie)
            {
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        (string)Texts["Generic_Fase"] + " (##)",
                        "Type",
                        "Narateltijd                           T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tnlrt") + "##",
                        "Drukknoppen",                        
                        "Dimnivo NIET dimmen                   PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmnivongedimd") + "##",
                        "Dimnivo WEL dimmen                    PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmnivgedimd") + "##",
                        "Dimmen per rateltikker uitgang"
                    }
                };
                foreach (var rt in c.Signalen.Rateltikkers)
                {
                    l.Add(new List<string>
                    {
                        rt.FaseCyclus.ToString(),
                        rt.Type.GetDescription(),
                        rt.NaloopTijd.ToString(),
                        rt.Detectoren.Select(x => x.Detector).Any() ? rt.Detectoren.Select(x => x.Detector).Aggregate((y, z) => y + ", " + z).ToString() : "-",
                        (rt.Type == Models.Enumerations.RateltikkerTypeEnum.HoeflakeBewaakt) ? rt.DimmingNiveauPeriodeNietDimmen.ToString() : "-",
                        (rt.Type == Models.Enumerations.RateltikkerTypeEnum.HoeflakeBewaakt) ? rt.DimmingNiveauPeriodeDimmen.ToString() : "-",
                        rt.DimmenPerUitgang.ToCustomString()
                    });
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            }
            else
            {
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        (string)Texts["Generic_Fase"] + " (##)",
                        "Type",
                        "Narateltijd                      T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tnlrt") + "##",
                        "Drukknoppen",
                        "Dimmen per rateltikker uitgang"
                    }
                };
                foreach (var rt in c.Signalen.Rateltikkers)
                {
                    l.Add(new List<string>
                    {
                        rt.FaseCyclus.ToString(),
                        rt.Type.GetDescription(),
                        rt.NaloopTijd.ToString(),
                        rt.Detectoren.Select(x => x.Detector).Any() ? rt.Detectoren.Select(x => x.Detector).Aggregate((y, z) => y + ", " + z).ToString() : "-",
                        rt.DimmenPerUitgang.ToCustomString()
                    });
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            }
            if (c.Signalen.Rateltikkers.Any(x => x.Type == Models.Enumerations.RateltikkerTypeEnum.HoeflakeBewaakt))
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    "Let op: Bewaakte rateltikkers worden geïnverteerd aangestuurd!", "Footer"));
            }

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Signalen_WaarschuwingsGroepen(ControllerModel c)    // nog niet gebruikt    
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Signalen_WaarschuwingsGroepen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Signalen_WaarschuwingsGroepen"], styleid: "Caption"));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_RobuGroVerKruising(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_RobuGroVerKruising");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_RobuGroVerKruising"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Hoofdschakelaar            SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schrgv"),
                    "Ophoogtype                 SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schrgv_snel"),
                    "RGV type                   PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrgv"),
                    "Min. cyclustijd            PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmin_tcyclus"),
                    "Max. cyclustijd            PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmax_tcyclus"),
                    "Ophogen tvg                PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_omhoog"),
                    "Verlagen tvg               PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_omlaag"),
                    "Min. verschil tvg          PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_verschil"),
                    "Verlagen bij OS            PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_npr_omlaag"), 
                }
            };
            l.Add(new List<string>
            {
                c.RoBuGrover.RoBuGrover.ToCustomString2(),
                c.RoBuGrover.OphogenTijdensGroen.ToCustomString2(),
               (c.RoBuGrover.MethodeRoBuGrover.ToString() == "IngesteldMaximum"    ? "0: " :
                c.RoBuGrover.MethodeRoBuGrover.ToString() == "AlleConflictGroepen" ? "1: " :
                c.RoBuGrover.MethodeRoBuGrover.ToString() == "EigenConflictGroep"  ? "2: " : "NG") + c.RoBuGrover.MethodeRoBuGrover.GetDescription(),
                c.RoBuGrover.MinimaleCyclustijd.ToString(),
                c.RoBuGrover.MaximaleCyclustijd.ToString(),
                c.RoBuGrover.GroenOphoogFactor.ToString(),
                c.RoBuGrover.GroenVerlaagFactor.ToString(),
                c.RoBuGrover.GroentijdVerschil.ToString(),
                c.RoBuGrover.GroenVerlaagFactorNietPrimair.ToString()
            });
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_RobuGroVerFasen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_RobuGroVerFasen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_RobuGroVerFasen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"] + " (##)",
                    "Min. verlenggroen             PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmintvg") + "##",
                    "Max. verlenggroen             PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmmaxtvg") + "##",
                    "Hiaat detectie",
                    "File detectie"
                }
            };
            foreach (var rgvfc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                l.Add(new List<string>
                {
                    rgvfc.FaseCyclus,
                    rgvfc.MinGroenTijd.ToString(),
                    rgvfc.MaxGroenTijd.ToString(),
                    (rgvfc.HiaatDetectoren.Select(x => x.Detector).Any()) ?
                      rgvfc.HiaatDetectoren.Select(x => x.Detector).Aggregate((y, z) => y + ", " + z).ToString() : "-",
                    (rgvfc.FileDetectoren.Select(x => x.Detector).Any()) ? 
                      rgvfc.FileDetectoren.Select(x => x.Detector).Aggregate((y, z) => y + ", " + z).ToString() : "-"
                 });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_RobuGroVerConflictgroepen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_RobuGroVerConflictgroepen");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_RobuGroVerConflictgroepen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string> { (string)Texts["Generic_Conflictgroep"], (string)Texts["Generic_Fasen"] }
            };
            int group = 0;
            foreach (var rgvfc in c.RoBuGrover.ConflictGroepen)
            {
                ++group;
                if (rgvfc.Fasen.Any())
                {
                    l.Add(new List<string> { group.ToString(), rgvfc.Fasen.Select(x => x.FaseCyclus).Aggregate((y, z) => y + ", " + z) });
                }

            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Pelotonkoppelingen(ControllerModel c)
        {

            var items = new List<OpenXmlCompositeElement>();
            var l = new List<List<string>> { };

            UpdateTables("Table_Pelotonkoppelingen_Overzicht");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Pelotonkoppelingen_Overzicht"], styleid: "Caption"));

            l = new List<List<string>>
            {
                new List<string>
                {
                    "Naam",
                    "Gekoppelde kruising",
                    "Inkomend / Uitgaand",
                    "Fase",
                    "Type",
                    "Intern",
                    "Gerelateerde koppeling (intern)"
                }
            };
            foreach (var kopp in c.PelotonKoppelingenData.PelotonKoppelingen)
            {
                l.Add(new List<string>
                {
                    kopp.KoppelingNaam.ToString(),
                    kopp.IsIntern ? "-" : kopp.PTPKruising.ToString(),
                    kopp.Richting.ToString(),
                    kopp.GekoppeldeSignaalGroep.ToString(),
                    kopp.Type.GetDescription(),
                    kopp.IsIntern.ToCustomString(),
                    kopp.IsIntern && (kopp.Richting == PelotonKoppelingRichtingEnum.Inkomend) ? kopp.GerelateerdePelotonKoppeling.ToString() : "-"
                 });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
           
            if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Type == Models.Enumerations.PelotonKoppelingTypeEnum.DenHaag))
            {
                UpdateTables("Table_Pelotonkoppelingen_Inkomend_DenHaag");

                items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Pelotonkoppelingen_Inkomend_DenHaag"], styleid: "Caption"));

                l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Naam (###)",
                        "Fase",
                        "Meetperiode                  T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tpelmeet") + "###",
                        "Max. hiaattijd               T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tpelmaxhiaat") + "###",
                        "Min. aantal mvt              PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmpelgrens") + "###",
                        "Toepassen A                  SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schpela") + "###",
                        "Tijd tot aanvraag            T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tpela") + "###",
                        "Toepassen RW                 SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schpelrw") + "###",
                        "Tijd tot RW                  T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tpelstartrw") + "###",
                        "Tijdsduur RW                 T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tpelrw") + "###",
                        "Max. RW na SG                T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tpelrwmax") + "###",
                        "Toepassen MK                 SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schpelmk") + "###",
                    }
                };
                foreach (var koppi in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => (x.Richting == Models.Enumerations.PelotonKoppelingRichtingEnum.Inkomend) && (x.Type == Models.Enumerations.PelotonKoppelingTypeEnum.DenHaag)))
                {
                    l.Add(new List<string>
                    {
                        koppi.KoppelingNaam.ToString(),
                        koppi.GekoppeldeSignaalGroep.ToString(),
                        koppi.Meetperiode.ToString(),
                        koppi.MaximaalHiaat.ToString(),
                        koppi.MinimaalAantalVoertuigen.ToString(),
                        koppi.ToepassenAanvraag.GetDescription(),
                        koppi.TijdTotAanvraag.ToString(),
                        koppi.ToepassenRetourWachtgroen.GetDescription(),
                        koppi.TijdTotRetourWachtgroen.ToString(),
                        koppi.TijdRetourWachtgroen.ToString(),
                        koppi.MaxTijdToepassenRetourWachtgroen.ToString(),
                        koppi.ToepassenMeetkriterium.GetDescription()
                    });
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            }

            if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Type == Models.Enumerations.PelotonKoppelingTypeEnum.RHDHV))
            {
                UpdateTables("Table_Pelotonkoppelingen_Inkomend_RHDHV");

                items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Pelotonkoppelingen_Inkomend_RHDHV"], styleid: "Caption"));

                l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Naam (###)",
                        "Fase",
                        "Verschuiving [s]             PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmpelverschuif") + "###",
                        "Min. duur peloton [s]        PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmpelgrens") + "###",
                        "Toepassen RW                 SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schpelrw") + "###",
                        "Nalooptijd [s]               T " + CCOLGeneratorSettingsProvider.Default.GetElementName("tpelnl") + "###",
                        "Toepassen A                  SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schpela") + "###",
                        "Toepassen MK                 SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schpelmk") + "###",
                    }
                };
                foreach (var koppi in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => (x.Richting == Models.Enumerations.PelotonKoppelingRichtingEnum.Inkomend) && (x.Type == Models.Enumerations.PelotonKoppelingTypeEnum.RHDHV)))
                {
                    l.Add(new List<string>
                    {
                        koppi.KoppelingNaam.ToString(),
                        koppi.GekoppeldeSignaalGroep.ToString(),
                        koppi.Verschuiving.ToString(),
                        koppi.MinimaalAantalVoertuigen.ToString(),
                        koppi.ToepassenRetourWachtgroen.GetDescription(),
                        koppi.TijdRetourWachtgroen.ToString(),
                        koppi.ToepassenAanvraag.GetDescription(),
                        koppi.ToepassenMeetkriterium.GetDescription(),
                    });
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            }

            if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == Models.Enumerations.PelotonKoppelingRichtingEnum.Uitgaand))
            {
                UpdateTables("Table_Pelotonkoppelingen_Uitgaand");

                items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_Pelotonkoppelingen_Uitgaand"], styleid: "Caption"));

                l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Naam (###)",
                        "Fase ($$)",
                        "Detectie",
                        "Schakelaar                   SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schpku") + "###$$"
                    }
                };
                foreach (var koppu in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == Models.Enumerations.PelotonKoppelingRichtingEnum.Uitgaand))
                {
                    l.Add(new List<string>
                    {
                        koppu.KoppelingNaam.ToString(),
                        koppu.GekoppeldeSignaalGroep.ToString(),
                        koppu.Detectoren.Select(x => x.DetectorNaam).Any() ? koppu.Detectoren.Select(x => x.DetectorNaam).Aggregate((y, z) => y + ", " + z).ToString() : "-",
                        "Aan"
                    });
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
                items.Add(OpenXmlHelper.GetTextParagraph("De schakelaar(s) voor de uitgaande peloton koppeling(en) zijn in TLCGen hard vastgelegd " +
                    "op 'Aan' maar zijn in de gegenereerde regeling wel te wijzigen.", "Footer"));

                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            }
            
            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_TT_Instellingen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
        
            UpdateTables("Table_TT_Instellingen");
        
            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_TT_Instellingen"], styleid: "Caption"));

            TTalgTabel = NumberOfTables.ToString();

            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"] + " (##)",
                    "Rijstrook ($)",
                    "Lane ID                                  PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrislaneid") + "##_$",
                    "Approach ID                              PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisapproachid") + "##",
                    "Crossing ID                             (SYSTEM_ITF)",
                }
            };
            foreach (var tt in c.RISData.RISFasen)
            {
                foreach (var lane in tt.LaneData)
                {
                    l.Add(new List<string> 
                    { 
                       (lane.RijstrookIndex.ToString() == "1") ? lane.SignalGroupName.ToString() : "",
                        lane.RijstrookIndex.ToString(),
                        lane.LaneID.ToString(),
                       (lane.RijstrookIndex.ToString() == "1") ? tt.ApproachID.ToString() : "",
                       (lane.RijstrookIndex.ToString() == "1") ? lane.SystemITF.ToString() : "",
                    });
                }
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

        public static List<OpenXmlCompositeElement> Table_TT_UC3_Prioriteren(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            var l = new List<List<string>> { };

            bool bit0  = false, bit1  = false, bit2  = false, bit3  = false, bit4  = false,
                 bit5  = false, bit6  = false, bit7  = false, bit8  = false, bit9  = false,
                 bit10 = false, bit11 = false, bit12 = false, bit13 = false, bit14 = false;

            if (c.PrioData.HDIngrepen.Any(x => x.RIS))
            {

                UpdateTables("Table_TT_UC3_HD");

                items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_TT_UC3_HD"], styleid: "Caption"));

                l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Fase (##)",
                        "Rijstrook ($)",
                        "Approach ID",
                      //"Crossing ID                             (SYSTEM_ITF)",
                        "Start afstand                            PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisstart")   + "##hd",
                        "Einde afstand                            PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisend")     + "##hd",
                        "RIS role                                 PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisrole")    + "##hd",
                        "RIS subrole                              PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrissubrole") + "##hd",
                        "Max. ETA                                 PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmriseta")     + "##hd",
                        "Prio niveau"
                    }
                };

                foreach (var hd in c.PrioData.HDIngrepen)
                {
                    var fcRis = c.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == hd.FaseCyclus);

                    foreach (var lane in fcRis.LaneData)
                    {
                        bit0  = bit1  = bit2  = bit3  = bit4  = 
                        bit5  = bit6  = bit7  = bit8  = bit9  = 
                        bit10 = bit11 = bit12 = bit13 = bit14 = false;
                        
                        var importance = "";
                        var impwaarde = (int)hd.RisImportance;
                        var impbit = impwaarde;
                        
                        if (impbit >= 16384) { bit14 = true; impbit %=  16384; }
                        if (impbit >=  8192) { bit13 = true; impbit %=   8192; }
                        if (impbit >=  4096) { bit12 = true; impbit %=   4096; }
                        if (impbit >=  2048) { bit11 = true; impbit %=   2048; }
                        if (impbit >=  1024) { bit10 = true; impbit %=   1024; }
                        if (impbit >=   512) { bit9 =  true; impbit %=    512; }
                        if (impbit >=   256) { bit8 =  true; impbit %=    256; }
                        if (impbit >=   128) { bit7 =  true; impbit %=    128; }
                        if (impbit >=    64) { bit6 =  true; impbit %=     64; }
                        if (impbit >=    32) { bit5 =  true; impbit %=     32; }
                        if (impbit >=    16) { bit4 =  true; impbit %=     16; }
                        if (impbit >=     8) { bit3 =  true; impbit %=      8; }
                        if (impbit >=     4) { bit2 =  true; impbit %=      4; }
                        if (impbit >=     2) { bit1 =  true; impbit %=      2; }
                        if (impbit >=     1) { bit0 =  true; impbit %=      1; }
                        
                        if (bit0)  importance += "0";
                        if (bit1)  importance += (importance == "") ?  "1" : ", 1";
                        if (bit2)  importance += (importance == "") ?  "2" : ", 2";
                        if (bit3)  importance += (importance == "") ?  "3" : ", 3";
                        if (bit4)  importance += (importance == "") ?  "4" : ", 4";
                        if (bit5)  importance += (importance == "") ?  "5" : ", 5";
                        if (bit6)  importance += (importance == "") ?  "6" : ", 6";
                        if (bit7)  importance += (importance == "") ?  "7" : ", 7";
                        if (bit8)  importance += (importance == "") ?  "8" : ", 8";
                        if (bit9)  importance += (importance == "") ?  "9" : ", 9";
                        if (bit10) importance += (importance == "") ? "10" : ", 10";
                        if (bit11) importance += (importance == "") ? "11" : ", 11";
                        if (bit12) importance += (importance == "") ? "12" : ", 12";
                        if (bit13) importance += (importance == "") ? "13" : ", 13";
                        if (bit14) importance += (importance == "") ? "14" : ", 14";

                        importance = (impwaarde > 0) ? importance : "";

                        l.Add(new List<string>
                        {
                            (lane.RijstrookIndex == 1) ? fcRis.FaseCyclus : "",
                            lane.RijstrookIndex.ToString(),
                            fcRis.ApproachID.ToString(),
                          //lane.SystemITF,
                            hd.RisStart.ToString(),
                            hd.RisEnd.ToString(),
                          //(int)hd.RisRole + ": " + hd.RisRole.ToString(),
                          //(int)hd.RisSubrole + ": " + hd.RisSubrole.ToString(),
                            "64: EMERGENCY",
                            "32: EMERGENCY",
                            hd.RisEta.HasValue ? hd.RisEta.ToString() : "-",
                            importance + " (" + impwaarde.ToString() + ")",
                        });
                    }
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
                items.Add(OpenXmlHelper.GetTextParagraph("Voor de samenhang tussen fasenummer, rijstrooknummer, Lane ID, Approach ID en Crossing ID: zie tabel " +
                    TTalgTabel + ".", "Footer"));
                items.Add(OpenXmlHelper.GetTextParagraph("Het HD prioriteitsniveau ('importance') via RIS is voor fc## bitsgewijs instelbaar " +
                    "via PRM risimportance##hd.", "Footer"));
                items.Add(OpenXmlHelper.GetTextParagraph("Het approach ID is voor fc## instelbaar " +
                    "via PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisapproachid") + "##hd.", "Footer"));
                items.Add(OpenXmlHelper.GetTextParagraph("In- en uitmeldingen zijn voor fc## schakelbaar via " +
                    "SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schhdin") + "##ris, respectievelijk " +
                    "SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schhduit") + "##ris.", "Footer"));
                items.Add(OpenXmlHelper.GetTextParagraph("De waarden voor RIS role (64) en RIS subrole (32) zijn in TLCGen hard vastgelegd " +
                    "maar zijn in de gegenereerde regeling wel te wijzigen via de aangegeven parameters.", "Footer"));

                items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
            }

            foreach (PrioIngreepVoertuigTypeEnum voertuigtype in Enum.GetValues(typeof(PrioIngreepVoertuigTypeEnum)))
            {
                var ingreepnaam = "";

                if (c.PrioData.PrioIngrepen.Exists(x => x.Type == voertuigtype &&
                                              x.MeldingenData.Inmeldingen.Any(y => y.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)))
                {
                    UpdateTables("UC3" + voertuigtype);

                    items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: Instellingen tbv prioriteren (UC3) voor voertuigtype '{voertuigtype}'", styleid: "Caption"));

                    foreach (var prio in c.PrioData.PrioIngrepen.Where(x => x.Type == voertuigtype &&
                                                 x.MeldingenData.Inmeldingen.Any(y => y.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)))
                    {
                        ingreepnaam = prio.Naam; // belabberde code, maar neemt per type ingreep de laatste prio.Naam uit het rijtje 
                    }

                    l = new List<List<string>>
                    {
                        new List<string>
                        {
                            "Fase (##)",
                            "Rijstrook",
                          //"Crossing ID                             (SYSTEM_ITF)",
                            "Start afstand                            PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisstart")      + "##" + ingreepnaam,
                            "Einde afstand                            PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisend")        + "##" + ingreepnaam,
                            "RIS role                                 PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisrole")       + "##" + ingreepnaam + "               (bitsgewijs instelbaar)",
                            "RIS subrole       PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrissubrole")    + "##" + ingreepnaam + "               (bitsgewijs instelbaar)",
                            "Max. ETA                                 PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmriseta")        + "##" + ingreepnaam,
                            "Prio niveau",
                        }
                    };

                    foreach (var prio in c.PrioData.PrioIngrepen.Where(x => x.Type == voertuigtype &&
                                                 x.MeldingenData.Inmeldingen.Any(y => y.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)))
                    {
                        var inRis = prio.MeldingenData.Inmeldingen.Where(x =>
                                    x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde);

                        if (inRis.Any())
                        {
                            var inmelding  = prio.MeldingenData.Inmeldingen.Where(x => x.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding);
                            var uitmelding = prio.MeldingenData.Inmeldingen.Where(x => x.InUit == PrioIngreepInUitMeldingTypeEnum.Uitmelding);

                            foreach (var inR in inRis)
                            {
                                bit0  = bit1  = bit2  = bit3  = bit4 =
                                bit5  = bit6  = bit7  = bit8  = bit9 =
                                bit10 = bit11 = bit12 = bit13 = bit14 = false;

                                var fcRis = c.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == prio.FaseCyclus);

                                var importance = "";
                                var impwaarde = (int)inR.RisImportance;
                                var impbit = impwaarde;

                                if (impbit >= 16384) { bit14 = true; impbit %= 16384; }
                                if (impbit >=  8192) { bit13 = true; impbit %=  8192; }
                                if (impbit >=  4096) { bit12 = true; impbit %=  4096; }
                                if (impbit >=  2048) { bit11 = true; impbit %=  2048; }
                                if (impbit >=  1024) { bit10 = true; impbit %=  1024; }
                                if (impbit >=   512) { bit9  = true; impbit %=   512; }
                                if (impbit >=   256) { bit8  = true; impbit %=   256; }
                                if (impbit >=   128) { bit7  = true; impbit %=   128; }
                                if (impbit >=    64) { bit6  = true; impbit %=    64; }
                                if (impbit >=    32) { bit5  = true; impbit %=    32; }
                                if (impbit >=    16) { bit4  = true; impbit %=    16; }
                                if (impbit >=     8) { bit3  = true; impbit %=     8; }
                                if (impbit >=     4) { bit2  = true; impbit %=     4; }
                                if (impbit >=     2) { bit1  = true; impbit %=     2; }
                                if (impbit >=     1) { bit0  = true; impbit %=     1; }

                                if (bit0)  importance += "0";
                                if (bit1)  importance += (importance == "") ? "1"  : ", 1";
                                if (bit2)  importance += (importance == "") ? "2"  : ", 2";
                                if (bit3)  importance += (importance == "") ? "3"  : ", 3";
                                if (bit4)  importance += (importance == "") ? "4"  : ", 4";
                                if (bit5)  importance += (importance == "") ? "5"  : ", 5";
                                if (bit6)  importance += (importance == "") ? "6"  : ", 6";
                                if (bit7)  importance += (importance == "") ? "7"  : ", 7";
                                if (bit8)  importance += (importance == "") ? "8"  : ", 8";
                                if (bit9)  importance += (importance == "") ? "9"  : ", 9";
                                if (bit10) importance += (importance == "") ? "10" : ", 10";
                                if (bit11) importance += (importance == "") ? "11" : ", 11";
                                if (bit12) importance += (importance == "") ? "12" : ", 12";
                                if (bit13) importance += (importance == "") ? "13" : ", 13";
                                if (bit14) importance += (importance == "") ? "14" : ", 14";

                                importance = (impwaarde > 0) ? importance : "-";

                                foreach (var lane in fcRis.LaneData)
                                {
                                    l.Add(new List<string>
                                    {
                                        (lane.RijstrookIndex == 1) ? prio.FaseCyclus : "",
                                        lane.RijstrookIndex.ToString(),
                                      //lane.SystemITF,
                                        inR.RisStart.ToString(),
                                        inR.RisEnd.ToString(),
                                        (int)inR.RisRole + ": " + inR.RisRole.ToString(),
                                        (int)inR.RisSubrole + ": " + inR.RisSubrole.ToString(),
                                        inR.RisEta.ToString(),
                                        importance + " (" + impwaarde.ToString() + ")",
                                    });
                                }
                            }
                        }
                    }

                    items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
                    items.Add(OpenXmlHelper.GetTextParagraph("Voor de samenhang tussen fasenummer, rijstrooknummer, Lane ID, Approach ID en Crossing ID: zie tabel " +
                        TTalgTabel + ".", "Footer"));
                    items.Add(OpenXmlHelper.GetTextParagraph("Alle approach ID's voor voertuigtype " + voertuigtype + " zijn op 999 (= niet gebruikt) ingesteld " +
                        "(PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisapproachid") + "##" + ingreepnaam + ").", "Footer"));
                    items.Add(OpenXmlHelper.GetTextParagraph("Het prioriteitsniveau ('importance') via RIS is voor fc## " +
                        "bitsgewijs instelbaar via PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisimportance") + "##" + ingreepnaam + ".", "Footer"));
                  //items.Add(OpenXmlHelper.GetTextParagraph("Het type voertuig ('stationtype') waavoor prioriteit moet worden verleend via RIS is voor fc## " +
                  //     "bitsgewijs instelbaar via PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisstationtype") + "##" + ingreepnaam + ".", "Footer"));
                    items.Add(OpenXmlHelper.GetTextParagraph("In- en uitmeldingen zijn voor fc## schakelbaar via " +
                        "SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schprioin") + "##" + ingreepnaam + "ris, respectievelijk " +
                        "SCH " + CCOLGeneratorSettingsProvider.Default.GetElementName("schpriouit") + "##" + ingreepnaam + "ris.", "Footer"));

                    items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));
                }
            }

            return items;
        }

        public static List<OpenXmlCompositeElement> Table_TT_UC5_Optimaliseren(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_TT_UC5_Optimaliseren");

            items.Add(OpenXmlHelper.GetTextParagraph($"Tabel {NumberOfTables.ToString()}: " + (string)Texts["Table_TT_UC5_Optimaliseren"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"] + " (##)",
                    "Rijstrook",
                    "Voertuigtype aanvraag (@@)",
                    "Aanvragen start [m]                         PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisastart") + "##@@$",
                    "Aanvragen einde [m]                         PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisaend") + "##@@$",
                    "Voertuigtype verlengen (@@)",
                    "Verlengen start [m]                         PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisvstart") + "##@@$",
                    "Verlengen einde [m]                         PRM " + CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisvend") + "##@@$",
                }
            };
            var sg_old = "";
            var ri_old = 0;
            foreach (var risa in c.RISData.RISRequestLanes) 
            {
                
                foreach (var risv in c.RISData.RISExtendLanes)
                {
                    if (risa.SignalGroupName.Equals(risv.SignalGroupName))
                    {
                        if (risa.SignalGroupName != sg_old) ri_old = 0;
                        if (risa.RijstrookIndex.Equals(risv.RijstrookIndex))
                        { 
                            if (risa.Type.Equals(risv.Type))
                            {
                                l.Add(new List<string>
                                {
                                    (risa.SignalGroupName != sg_old) ? risa.SignalGroupName.ToString() : "",
                                    (risa.RijstrookIndex  != ri_old) ? risa.RijstrookIndex.ToString() : "",
                                    risa.Type.ToString() + " (" + risa.Type.GetDescription() + ")",
                                    risa.AanvraagStart.ToString(),
                                    risa.AanvraagEnd.ToString(),
                                    risv.Type.ToString() + " (" + risv.Type.GetDescription() + ")",
                                    risv.VerlengenStart.ToString(),
                                    risv.VerlengenEnd.ToString(),
                                });
                                ri_old = risa.RijstrookIndex;
                                sg_old = risa.SignalGroupName;
                            }
                        }
                    }
                }
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            items.Add(OpenXmlHelper.GetTextParagraph("Voor de samenhang tussen fasenummer, rijstrooknummer, Lane ID, Approach ID en Crossing ID: zie tabel " +
                    TTalgTabel + ".", "Footer"));
            items.Add(OpenXmlHelper.GetTextParagraph("Wanneer zowel de start- als de eindeparameter '0' zijn wordt er niet via CAM aangevraagd resp. verlengd.", "Footer"));

            items.Add(OpenXmlHelper.GetTextParagraph("", "Footer"));

            return items;
        }

    }
}
