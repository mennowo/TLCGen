using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TLCGen.Extensions;
using TLCGen.Models;

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
        public static Dictionary<string, int> Tables { get; } = new Dictionary<string, int>();

        public static void ClearTables()
        {
            Tables.Clear();
        }

        private static void UpdateTables(string table)
        {
            NumberOfTables++;
            Tables.Add(table, NumberOfTables);
        }

        public static List<OpenXmlCompositeElement> GetTable_Detectie_Tijden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Detectoren_Tijden");

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Detectoren_Tijden"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));
            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Detector"],
                    "Fase",
                    "Bezettijd",
                    "Hiaattijd",
                    "Ondergedrag",
                    "Bovengedrag",
                    "Flutter tijd",
                    "Flutter counter"
                }
            };
            foreach (var fc in c.Fasen)
            {
                foreach (var d in fc.Detectoren)
                {
                    l.Add(new List<string>
                    {
                        CCOLGenHelper.Dpf + d.Naam,
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
                    CCOLGenHelper.Dpf + d.Naam,
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

        public static List<OpenXmlCompositeElement> GetTable_Perioden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Perioden");

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Perioden"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string> { "Periode", "Start", "Einde", "Dagtype", "Groentijden set" }
            };
            foreach (var p in c.PeriodenData.Perioden)
            {
                l.Add(new List<string> { p.Naam, p.StartTijd.ToString(@"hh\:mm"), p.EindTijd.ToString(@"hh\:mm"), p.DagCode.ToString(), p.GroentijdenSet });
            }
            items.Add(OpenXmlHelper.GetTable(l));

            return items;
        }


        public static List<OpenXmlCompositeElement> GetTable_ModuleStructuurInstellingen(WordprocessingDocument doc, ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            if (c.ModuleMolen.LangstWachtendeAlternatief)
            {
                items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_VooruitAltInst"], styleid: "Caption"));

                UpdateTables("Table_VooruitAltInst");

                var l = new List<List<string>>
                {
                    new List<string>
                    {
                        (string)Texts["Generic_Fase"],
                        "Aantal modulen vooruit",
                        "Alternatief toegestaan",
                        "Alternatieve ruimte",
                        "Alternatieve groentijd"
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
                items.Add(OpenXmlHelper.GetTable(l));
            }
            else if (c.ModuleMolen.Modules.Any(x => x.Fasen.Any(x2 => x2.Alternatieven.Any())))
            {
                items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Vooruit"], styleid: "Caption"));

                UpdateTables("Table_Vooruit");

                var lt = new List<string>
                {
                    (string)Texts["Generic_Fase"],
                    "Aantal modulen vooruit"
                };
                //foreach(var m in c.ModuleMolen.Modules)
                //{
                //    lt.Add("Alt. groentijd onder " + m.Naam);
                //}
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
                    //foreach (var m in c.ModuleMolen.Modules)
                    //{
                    //    var t = "-";
                    //    var mfc = m.Fasen.FirstOrDefault(x2 => x2.Alternatieven.Any(x3 => x3.FaseCyclus == x.FaseCyclus));
                    //    if(mfc != null)
                    //    {
                    //        var afc = mfc.Alternatieven.First(x2 => x2.FaseCyclus == x.FaseCyclus);
                    //        t = afc.AlternatieveGroenTijd.ToString();
                    //    }
                    //    nl.Add(t);
                    //}
                    l.Add(nl);
                });
                items.Add(OpenXmlHelper.GetTable(l));

                items.Add(OpenXmlHelper.GetTextParagraph($"De volgende {(string)Texts["Generic_fasen"]} kunnen alternatief realiseren onder dekking van een andere {(string)Texts["Generic_fase"]} in cyclisch verlenggroen (CV)."));

                var il = new List<Tuple<string, int>>();
                foreach (var m in c.ModuleMolen.Modules)
                {
                    foreach (var f in m.Fasen.Where(x2 => x2.Alternatieven.Any()))
                    {
                        foreach (var a in f.Alternatieven)
                        {
                            il.Add(new Tuple<string, int>($"In module {m.Naam}: {a.FaseCyclus} onder dekking van {f.FaseCyclus} (groentijd: {a.AlternatieveGroenTijd})", 0));
                        }
                    }
                }
                items.AddRange(OpenXmlHelper.GetBulletList(doc, il));
            }
            else
            {
                items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_VooruitAltInst"], styleid: "Caption"));

                UpdateTables("Table_VooruitAltInst");

                var l = new List<List<string>>
                {
                    new List<string>
                    {
                        (string)Texts["Generic_Fase"],
                        "Aantal modulen vooruit"
                    }
                };
                c.ModuleMolen.FasenModuleData.ForEach(x => l.Add(new List<string> { x.ModulenVooruit.ToString() }));
                items.Add(OpenXmlHelper.GetTable(l));
            }

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Ontruimingstijden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Ontruimingstijden"], styleid: "Caption"));

            UpdateTables("Table_Ontruimingstijden");

            var sl = new List<List<string>>();

            var top = new List<string> { "" };
            foreach(var fc in c.Fasen)
            {
                top.Add(fc.Naam);
            }
            sl.Add(top);

            foreach(var fcVan in c.Fasen)
            {
                var tijden = new List<string> { fcVan.Naam };
                foreach (var fcNaar in c.Fasen)
                {
                    if(ReferenceEquals(fcVan, fcNaar))
                    {
                        tijden.Add("X");
                    }
                    else
                    {
                        var ot = c.InterSignaalGroep.Conflicten.FirstOrDefault(x => x.FaseVan == fcVan.Naam && x.FaseNaar == fcNaar.Naam);
                        if(ot != null)
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

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_OntruimingstijdenGarantie"], styleid: "Caption"));

            UpdateTables("Table_OntruimingstijdenGarantie");

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

        public static List<OpenXmlCompositeElement> GetTable_Fasen_Groentijden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            OpenXmlCompositeElement tableTitle = null;
            switch (c.Data.TypeGroentijden)
            {
                case Models.Enumerations.GroentijdenTypeEnum.MaxGroentijden:
                UpdateTables("Table_MaxGroentijden");
                    tableTitle = OpenXmlHelper.GetTextParagraph((string)Texts["Table_MaxGroentijden"], styleid: "Caption");
                    break;
                case Models.Enumerations.GroentijdenTypeEnum.VerlengGroentijden:
                UpdateTables("Table_VerlGroentijden");
                    tableTitle = OpenXmlHelper.GetTextParagraph((string)Texts["Table_VerlGroentijden"], styleid: "Caption");
                    break;
            }
            items.Add(tableTitle);

            var l = new List<List<string>>();
            var l1 = new List<string> { (string)Texts["Fase"] };
            foreach (var set in c.GroentijdenSets)
            {
                l1.Add(set.Naam);
            }
            l.Add(l1);
            foreach (var fc in c.Fasen)
            {
                var l2 = new List<string> { fc.Naam };
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
                l.Add(l2);
            }
            items.Add(OpenXmlHelper.GetTable(l));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Detectie_Functies(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Detectoren_Functies");

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Detectoren_Functies"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));
            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Detector"],
                    "Fase",
                    "Type",
                    "Aanvraag",
                    "Verlengen",
                    "Aanvraag direct",
                    "Wachtlicht",
                    "Rijstrook",
                    "Aanvraag bij storing",
                    "Veiligheidsgroen"
                }
            };
            foreach (var fc in c.Fasen)
                foreach (var d in fc.Detectoren)
                {
                    l.Add(new List<string>
                {
                    CCOLGenHelper.Dpf + d.Naam,
                    fc.Naam,
                    d.Type.GetDescription(),
                    d.Aanvraag.GetDescription(),
                    d.Verlengen.GetDescription(),
                    d.AanvraagDirect.ToCustomString(),
                    d.Wachtlicht.ToCustomString(),
                    d.Rijstrook.ToString(),
                    d.AanvraagBijStoring.GetDescription(),
                    d.VeiligheidsGroen.GetDescription(),
                });
                }
            foreach (var d in c.Detectoren)
            {
                l.Add(new List<string>
                {
                    CCOLGenHelper.Dpf + d.Naam,
                    "-",
                    d.Type.GetDescription(),
                    d.Aanvraag.GetDescription(),
                    d.Verlengen.GetDescription(),
                    d.AanvraagDirect.ToCustomString(),
                    d.Wachtlicht.ToCustomString(),
                    d.Rijstrook.ToString(),
                    d.AanvraagBijStoring.GetDescription(),
                    d.VeiligheidsGroen.GetDescription()
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_FasenTijden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Fasen_Tijden");

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Fasen_Tijden"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));
            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"],
                    "Vastgroen",
                    "Garantiegroen",
                    "Minimum garantiegroen",
                    "Garantierood",
                    "Minimum garantierood",
                    "Geel",
                    "Minimum geel",
                    "Kopmax"
                }
            };
            // TODO: veiligheidsgroen
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

        public static List<OpenXmlCompositeElement> GetTable_FasenFuncties(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Fasen_Functies");

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Fasen_Functies"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"],
                    "Type",
                    "Rijstroken",
                    "Vaste aanvraag",
                    "Wachtgroen",
                    "Meeverlengen",
                    "Wachttijd voorspellers",
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
                    fc.Wachtgroen.GetDescription(),
                    fc.Meeverlengen.GetDescription(),
                    fc.WachttijdVoorspeller == true ? "X" : "-",
                    fc.Detectoren.Count.ToString()
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Modulen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Modulen");

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Modulen"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string> { (string)Texts["Generic_Modulen"], (string)Texts["Generic_Fasen"] }
            };
            c.ModuleMolen.Modules.ForEach(m => l.Add(new List<string> { m.Naam, m.Fasen.Select(x => x.FaseCyclus).Aggregate((y, z) => y + ", " + z) }));
            items.Add(OpenXmlHelper.GetTable(l));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Detectie_StoringMaatregelen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Detectoren_StoringMaatregelen");

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Detectoren_StoringMaatregelen"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"],
                    "Vaste aanvraag bij defect alle lussen",
                    "Vervangend hiaat koplus",
                    "Instelling vervangen hiaat",
                    "Percentage groentijd bij storing",
                    "Instelling percentage groentijd",
                }
            };
            foreach (var fc in c.Fasen)
            {
                l.Add(new List<string>
                {
                    fc.Naam,
                    fc.AanvraagBijDetectieStoring.ToCustomString(),
                    fc.HiaatKoplusBijDetectieStoring.ToCustomString(),
                    fc.HiaatKoplusBijDetectieStoring ? fc.VervangendHiaatKoplus.ToString() : "-",
                    fc.PercentageGroenBijDetectieStoring.ToCustomString(),
                    fc.PercentageGroenBijDetectieStoring ? fc.PercentageGroen.ToString() : "-"
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }
    }
}
