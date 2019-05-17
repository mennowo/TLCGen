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
            foreach (var p in c.PeriodenData.Perioden.Where(x => x.Type == Models.Enumerations.PeriodeTypeEnum.Groentijden))
            {
                l.Add(new List<string> { p.Naam, p.StartTijd.ToString(@"hh\:mm"), p.EindTijd.ToString(@"hh\:mm"), p.DagCode.ToString(), p.GroentijdenSet });
            }
            items.Add(OpenXmlHelper.GetTable(l));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_AlternatievenOnderDekking(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_AlternatievenOnderDekking");

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_AlternatievenOnderDekking"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string> { "Module", "Richting", "Onder dekking van", "Groentijd" }
            };
            foreach (var ml in c.ModuleMolen.Modules)
            {
                foreach(var mlfc in ml.Fasen)
                {
                    if (mlfc.Alternatieven.Any())
                    {
                        foreach(var alt in mlfc.Alternatieven)
                        {
                            l.Add(new List<string> { ml.Naam, alt.FaseCyclus, mlfc.FaseCyclus, alt.AlternatieveGroenTijd.ToString() });
                        }
                    }
                }
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

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Intersignaalgroep_Meeaanvragen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Intersignaalgroep_Meeaanvragen");

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Intersignaalgroep_Meeaanvragen"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Van",
                    "Naar",
                    "Type meeaanvraag",
                    "Schakelbaar",
                    "Type instelbaar",
                    "Detectie afhankelijk",
                    "Uitgesteld"
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
                    ma.Type.GetDescription(),
                    ma.AanUit.GetDescription(),
                    ma.TypeInstelbaarOpStraat.ToCustomString(),
                    da,
                    ma.Type == Models.Enumerations.MeeaanvraagTypeEnum.Startgroen && ma.Uitgesteld ? "x (" + ma.UitgesteldTijdsduur.ToString() + ")" : "-"
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Intersignaalgroep_Nalopen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Intersignaalgroep_Nalopen");

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Intersignaalgroep_Nalopen"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Van",
                    "Naar",
                    "Type naloop",
                    "Vaste naloop",
                    "Detectie afhankelijk",
                    "Inlopen/rijden bij groen",
                    "Max.voorstart tijd",
                    "Tijd SG/FG",
                    "Det.afh.tijd SG/FG",
                    "Tijd CV/EG",
                    "Det.afh.tijd CV/EG"
                }
            };
            foreach (var ma in c.InterSignaalGroep.Nalopen)
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
                        da += d.Detector;
                    }
                    da += ")";
                }
                var t1 = "-";
                var t2 = "-";
                var t3 = "-";
                var t4 = "-";
                if(ma.Type == Models.Enumerations.NaloopTypeEnum.StartGroen)
                {
                    if (ma.VasteNaloop)
                    {
                        t1 = ma.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.StartGroen).Waarde.ToString();
                    }
                    if (ma.DetectieAfhankelijk)
                    {
                        t2 = ma.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.StartGroenDetectie).Waarde.ToString();
                    }
                }
                if (ma.Type == Models.Enumerations.NaloopTypeEnum.EindeGroen && ma.VasteNaloop)
                {
                    if (ma.VasteNaloop)
                    {
                        t1 = ma.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.VastGroen).Waarde.ToString();
                    }
                    if (ma.DetectieAfhankelijk)
                    {
                        t2 = ma.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.VastGroenDetectie).Waarde.ToString();
                    }
                }
                if (ma.Type == Models.Enumerations.NaloopTypeEnum.EindeGroen && ma.DetectieAfhankelijk)
                {
                    if (ma.VasteNaloop)
                    {
                        t3 = ma.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.EindeGroen).Waarde.ToString();
                    }
                    if (ma.DetectieAfhankelijk)
                    {
                        t4 = ma.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.EindeGroenDetectie).Waarde.ToString();
                    }
                }
                if (ma.Type == Models.Enumerations.NaloopTypeEnum.CyclischVerlengGroen && ma.VasteNaloop)
                {
                    if (ma.VasteNaloop)
                    {
                        t1 = ma.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.VastGroen).Waarde.ToString();
                    }
                    if (ma.DetectieAfhankelijk)
                    {
                        t2 = ma.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.VastGroenDetectie).Waarde.ToString();
                    }
                }
                if (ma.Type == Models.Enumerations.NaloopTypeEnum.CyclischVerlengGroen && ma.DetectieAfhankelijk)
                {
                    if (ma.VasteNaloop)
                    {
                        t3 = ma.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.EindeVerlengGroen).Waarde.ToString();
                    }
                    if (ma.DetectieAfhankelijk)
                    {
                        t4 = ma.Tijden.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTijdTypeEnum.EindeVerlengGroenDetectie).Waarde.ToString();
                    }
                }
                l.Add(new List<string>
                {
                    ma.FaseVan,
                    ma.FaseNaar,
                    ma.Type.GetDescription(),
                    ma.VasteNaloop.ToCustomString(),
                    da,
                    ma.InrijdenTijdensGroen.ToCustomString(),
                    ma.MaximaleVoorstart == null ? "-" : ma.MaximaleVoorstart.Value.ToString(),
                    t1, t2, t3, t4
                    
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_Intersignaalgroep_Gelijkstarten(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_Intersignaalgroep_Gelijkstarten");

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Intersignaalgroep_Gelijkstarten"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Van",
                    "Naar",
                    "Deelconflict",
                    "Schakelbaar",
                    "Fictieve o.t. van > naar",
                    "Fictieve o.t. naar > van"
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

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Intersignaalgroep_Voorstarten"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Van",
                    "Naar",
                    "Voorstart tijd",
                    "Fictieve o.t. naar > van"
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

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Intersignaalgroep_LateRelease"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Van",
                    "Naar",
                    "Late release tijd",
                    "Fictieve o.t. naar > van"
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

        internal static IEnumerable<OpenXmlCompositeElement> GetTable_OV_PrioriteitsOpties(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            UpdateTables("Table_OV_PrioriteitsOpties");

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_OV_PrioriteitsOpties"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    "Optie",
                    "Toelichting"
                }
            };
            l.Add(new List<string> { "0", "Geen opties" });
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

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_OV_PrioriteitsInstellingen"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));

            List<List<string>> l;
            if (c.OVData.OVIngrepen.Any(x => x.GeconditioneerdePrioriteit != Models.Enumerations.NooitAltijdAanUitEnum.Nooit))
            {
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Signaalgroep",
                        "Prioriteit geconditioneerd",
                        "Prioriteitsopties te vroeg",
                        "Prioriteitsopties op tijd",
                        "Prioriteitsopties te laat",
                        "Prioriteitsopties geen",
                        "Ongehinderde rijtijd",
                        "Beperkt rijtijd",
                        "Gehinderde rijtijd",
                        "Bezettijd OV gehinderd",
                        "Blokkeringstijd",
                        "Groenbewaking",
                        "Ondermaximum"
                    }
                };
                foreach(var ov in c.OVData.OVIngrepen)
                {
                    var cp = ov.GeconditioneerdePrioriteit != Models.Enumerations.NooitAltijdAanUitEnum.Nooit;
                    var opties = 0;
                    if (ov.AfkappenConflicten || ov.AfkappenConflictenOV) opties += 100;
                    if (ov.AfkappenConflictenOV) opties += 300;
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
                        ov.BezettijdOVGehinderd.ToString(),
                        ov.BlokkeertijdNaOVIngreep.ToString(),
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
                foreach (var ov in c.OVData.OVIngrepen)
                {
                    var cp = ov.GeconditioneerdePrioriteit != Models.Enumerations.NooitAltijdAanUitEnum.Nooit;
                    var opties = 0;
                    if (ov.AfkappenConflicten || ov.AfkappenConflictenOV) opties += 100;
                    if (ov.AfkappenConflictenOV) opties += 300;
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
                        ov.BezettijdOVGehinderd.ToString(),
                        ov.BlokkeertijdNaOVIngreep.ToString(),
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

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_OV_ConflictenInstellingen"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));

            List<List<string>> l;
            l = new List<List<string>>
                {
                    new List<string>
                    {
                        "Signaalgroep",
                        "Afkapgarantie",
                        "Percentage maximum groentijd",
                        "Ophoog percentage maximumgroentijd",
                        "Aantal malen niet afbreken",
                        "Percentage groentijd t.b.v. terugkeren",
                        "Ondergrens na terugkomen"
                    }
                };
            foreach (var ovcf in c.OVData.OVIngreepSignaalGroepParameters)
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

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_HD_Instellingen"] + $" (tabel {NumberOfTables.ToString()})", styleid: "Caption"));

            var l = new List<List<string>>();
            var ll = new List<string> { "Signaalgroep" };
            if (c.HasHDKAR())
            {
                ll.Add("KAR");
                ll.Add("KAR inmeld filtertijd");
                ll.Add("KAR uitmeld filtertijd");
            }
            ll.Add("Check sirene");
            ll.Add("Rijtijd ongehinderd");
            ll.Add("Rijtijd beperkt gehinderd");
            ll.Add("Rijtijd gehinderd");
            ll.Add("Groenbewaking");
            if (c.HasHDOpticom())
            {
                ll.Add("Opticom");
                ll.Add("Opticom inmeld filtertijd");
            }
            if(c.OVData.HDIngrepen.Any(x => x.MeerealiserendeFaseCycli.Any()))
            {
                ll.Add("Meerealiserende fasen");
            }
            l.Add(ll);
            foreach (var ovcf in c.OVData.HDIngrepen)
            {
                ll.Clear();
                ll.Add(ovcf.FaseCyclus);
                if (c.HasHDKAR())
                {
                    ll.Add(ovcf.KAR.ToCustomString());
                    ll.Add(ovcf.KARInmeldingFilterTijd.ToString());
                    ll.Add(ovcf.KARUitmeldingFilterTijd.ToString());
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
                if (c.OVData.HDIngrepen.Any(x => x.MeerealiserendeFaseCycli.Any()))
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
    }
}
