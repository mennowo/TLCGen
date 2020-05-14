using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateRgvC(ControllerModel c)
        {
            if(c.RoBuGrover.ConflictGroepen?.Count == 0)
            {
                return null;
            }

            var _prmrgv = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrgv");
            var _prmmin_tcyclus = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmin_tcyclus");
            var _prmmax_tcyclus = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmax_tcyclus");
            var _prmmintvg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmintvg");
            var _prmmaxtvg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmaxtvg");
            var _prmtvg_omhoog = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_omhoog");
            var _prmtvg_omlaag = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_omlaag");
            var _prmtvg_verschil = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_verschil");
            var _prmtvg_npr_omlaag = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_npr_omlaag");
            var _hprreal = CCOLGeneratorSettingsProvider.Default.GetElementName("hprreal");
            var _schrgv = CCOLGeneratorSettingsProvider.Default.GetElementName("schrgv");
            var _schrgv_snel = CCOLGeneratorSettingsProvider.Default.GetElementName("schrgv_snel");
            var _tfd = CCOLGeneratorSettingsProvider.Default.GetElementName("tfd");
            var _thd = CCOLGeneratorSettingsProvider.Default.GetElementName("thd");
            var _tnlcv = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcv");
            var _tnlcvd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcvd");
            var _tnleg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnleg");
            var _tnlegd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlegd");

            var _hfile = CCOLGeneratorSettingsProvider.Default.GetElementName("hfile");

            var fasenMetRgv = c.Fasen.Where(x => c.RoBuGrover.SignaalGroepInstellingen.Any(x2 => x2.FaseCyclus == x.Naam));

            var sb = new StringBuilder();
            sb.AppendLine("/* IMPLEMENTATIE ROBUGROVER */");
            sb.AppendLine("/* ------------------------ */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(c.Data, "rgv.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(c.Data));
            sb.AppendLine();
            sb.AppendLine("/* defines voor ROBUGROVER */");
            sb.AppendLine($"#define MAX_AANTAL_CONFLICTGROEPEN {c.RoBuGrover.ConflictGroepen.Count}");
            sb.AppendLine("mulv TC[MAX_AANTAL_CONFLICTGROEPEN];");
            sb.AppendLine("mulv TC_max, DD_anyfase;");
            if(c.Data.CCOLVersie >= Models.Enumerations.CCOLVersieEnum.CCOL9 && c.Data.Intergroen)
            {
                sb.AppendLine($"#if (CCOL_V >= 95) && !defined NO_TIGMAX");
                sb.AppendLine($"{ts}mulv TIG_ontwerp[FCMAX][FCMAX];");
                sb.AppendLine($"#else");
                sb.AppendLine($"{ts}mulv TO_ontwerp[FCMAX][FCMAX];");
                sb.AppendLine($"#endif");
            }
            else
            {
                sb.AppendLine("mulv TO_ontwerp[FCMAX][FCMAX];");
            }
            sb.AppendLine("");
            sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || (defined VISSIM)");
            sb.AppendLine($"{ts}mulv TC_rgv[MAX_AANTAL_CONFLICTGROEPEN];");
            sb.AppendLine($"{ts}char * TC_string$[MAX_AANTAL_CONFLICTGROEPEN];");
            sb.AppendLine("#endif");
            sb.AppendLine("");
            sb.AppendLine("/* Robugrover includes */");
            sb.AppendLine("#include \"rgvfunc.c\"");
            sb.AppendLine("#include \"rgv_overslag.c\"");
            sb.AppendLine("");
            if (c.RoBuGrover.RoBuGroverVenster)
            {
                sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || (defined VISSIM)");
                sb.AppendLine($"{ts}#include \"winmg.c\"");
                sb.AppendLine("#endif");
            }
            sb.AppendLine();
            sb.AppendLine("void rgv_add(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}static {c.GetBoolV()} DD[FCMAX];            /* Detectie storing (Detection Disabled) */");
            sb.AppendLine($"{ts}static {c.GetBoolV()} MK1[FCMAX];           /* Meetkriterium op rijstrook 1 */");
            if(c.RoBuGrover.SignaalGroepInstellingen.Any())
            {
                var rijstrookMax = fasenMetRgv.Max(x => x.AantalRijstroken);
                if(rijstrookMax > 1)
                {
                    sb.AppendLine($"{ts}static {c.GetBoolV()} MK2[FCMAX];           /* Meetkriterium op rijstrook 2 */");
                }
                if (rijstrookMax > 2)
                {
                    sb.AppendLine($"{ts}static {c.GetBoolV()} MK3[FCMAX];           /* Meetkriterium op rijstrook 3 */");
                }
                if (rijstrookMax > 3)
                {
                    sb.AppendLine($"{ts}static {c.GetBoolV()} MK4[FCMAX];           /* Meetkriterium op rijstrook 4 */");
                }
            }
            sb.AppendLine($"{ts}static mulv TVG_rgv_old[FCMAX];   /* Opslag 'old' TVG tijden */");
            sb.AppendLine($"{ts}static mulv TVG_rgv_older[FCMAX]; /* Opslag 'older' TVG tijden */");
            sb.AppendLine($"{ts}static mulv rgvinit = 1;          /* Onthouden initialisatie */");
            sb.AppendLine($"{ts}int teller = 0;");
            sb.AppendLine("#if (defined AUTOMAAT || defined AUTOMAAT_TEST)");
            sb.AppendLine($"{ts}int fc;");
            sb.AppendLine("#endif");
            sb.AppendLine($"{ts}int i, j;");
            sb.AppendLine();
            sb.AppendLine($"{ts}for(i = 0; i < FCMAX; ++i)");
            sb.AppendLine($"{ts}{{");
            sb.AppendLine($"{ts}{ts}for(j = 0; j < FCMAX; ++j) ");
            sb.AppendLine($"{ts}{ts}{{");
            if (c.Data.CCOLVersie >= Models.Enumerations.CCOLVersieEnum.CCOL9 && c.Data.Intergroen)
            {
                sb.AppendLine($"{ts}{ts}{ts}#if (CCOL_V >= 95) && !defined NO_TIGMAX");
                sb.AppendLine($"{ts}{ts}{ts}{ts}TIG_ontwerp[i][j] = TIG_max[i][j];");
                sb.AppendLine($"{ts}{ts}{ts}#else");
                sb.AppendLine($"{ts}{ts}{ts}{ts}TO_ontwerp[i][j] = TO_max[i][j];");
                sb.AppendLine($"{ts}{ts}{ts}#endif");
            }
            else
            {
                sb.AppendLine($"{ts}{ts}{ts}TO_ontwerp[i][j] = TO_max[i][j];");
            }
            sb.AppendLine($"{ts}{ts}}}");
            sb.AppendLine($"{ts}}}");
            sb.AppendLine();
            if(c.InterSignaalGroep.Nalopen.Any(x => x.Type == Models.Enumerations.NaloopTypeEnum.CyclischVerlengGroen || x.Type == Models.Enumerations.NaloopTypeEnum.EindeGroen))
            {
                sb.AppendLine($"{ts}/* Fictieve ontwerp ontruimingstijden obv nalooptijden");
                sb.AppendLine($"{ts}   - voor nalopen op CV of EG wordt TO_ontwerp gecorrigeerd */");
                foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == Models.Enumerations.NaloopTypeEnum.CyclischVerlengGroen || x.Type == Models.Enumerations.NaloopTypeEnum.EindeGroen))
                {
                    var tnl = _tnleg;
                    if (nl.Type == Models.Enumerations.NaloopTypeEnum.EindeGroen && nl.DetectieAfhankelijk) tnl = _tnlegd;
                    else if (nl.Type == Models.Enumerations.NaloopTypeEnum.CyclischVerlengGroen && nl.DetectieAfhankelijk) tnl = _tnlcvd;
                    else if (nl.Type == Models.Enumerations.NaloopTypeEnum.CyclischVerlengGroen) tnl = _tnlcv;
                    var nlfc = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);
                    if (nlfc == null) continue;
                    sb.AppendLine($"{ts}/* Fase {nl.FaseVan} en conflicten van naloop {nl.FaseNaar} */");
                    if (c.Data.CCOLVersie >= Models.Enumerations.CCOLVersieEnum.CCOL9 && c.Data.Intergroen)
                    {
                        sb.AppendLine($"{ts}#if (CCOL_V >= 95) && !defined NO_TIGMAX");
                        foreach (var conf in c.InterSignaalGroep.Conflicten.Where(x => x.FaseVan == nlfc.Naam))
                        {
                            sb.AppendLine($"{ts}{ts}TIG_ontwerp[{_fcpf}{nl.FaseVan}][{_fcpf}{conf.FaseNaar}] = TIG[{_fcpf}{conf.FaseVan}][{_fcpf}{conf.FaseNaar}] + T_max[{_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar}];");
                        }
                        sb.AppendLine($"{ts}#else");
                        foreach (var conf in c.InterSignaalGroep.Conflicten.Where(x => x.FaseVan == nlfc.Naam))
                        {
                            sb.AppendLine($"{ts}{ts}TO_ontwerp[{_fcpf}{nl.FaseVan}][{_fcpf}{conf.FaseNaar}] = TO[{_fcpf}{conf.FaseVan}][{_fcpf}{conf.FaseNaar}] + T_max[{_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar}];");
                        }
                        sb.AppendLine($"{ts}#endif");
                    }
                    else
                    {
                        foreach (var conf in c.InterSignaalGroep.Conflicten.Where(x => x.FaseVan == nlfc.Naam))
                        {
                            sb.AppendLine($"{ts}TO_ontwerp[{_fcpf}{nl.FaseVan}][{_fcpf}{conf.FaseNaar}] = TO[{_fcpf}{conf.FaseVan}][{_fcpf}{conf.FaseNaar}] + T_max[{_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar}];");
                        }
                    }
                    sb.AppendLine();
                }
            }
            sb.AppendLine($"{ts}/* intitieer waarden TGV_rgv */");
            sb.AppendLine($"{ts}/* ------------------------- */");
            sb.AppendLine($"{ts}if(rgvinit)");
            sb.AppendLine($"{ts}{{");
            foreach(var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                sb.AppendLine($"{ts}{ts}TVG_rgv[{_fcpf}{fc.FaseCyclus}] = TVG_max[{_fcpf}{fc.FaseCyclus}];");
            }
            sb.AppendLine($"{ts}{ts}rgvinit = 0;");
            sb.AppendLine($"{ts}}}");
            sb.AppendLine();
            sb.AppendLine($"{ts}/* kopieer de basis waarden van TVG_max */");
            sb.AppendLine($"{ts}/* ------------------------------------ */");
            foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                sb.AppendLine($"{ts}TVG_basis[{_fcpf}{fc.FaseCyclus}] = TVG_max[{_fcpf}{fc.FaseCyclus}] > 0 ? TVG_max[{_fcpf}{fc.FaseCyclus}] : 1;");
            }
            sb.AppendLine();
            sb.AppendLine($"{ts}/* detectiestoringen voor de fasecycli */");
            sb.AppendLine($"{ts}/* ----------------------------------- */");
            foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.FileDetectoren.Count > 0)
                {
                    foreach (var d in fc.FileDetectoren)
                    {
                        sb.AppendLine($"{ts}RT[{_tpf}{_tfd}{_dpf}{d.Detector}] = SD[{_dpf}{d.Detector}] || ED[{_dpf}{d.Detector}] || !VG[{_fcpf}{fc.FaseCyclus}]; ");
                    }
                }
            }
            sb.AppendLine();
            sb.AppendLine($"{ts}DD_anyfase = 0;");
            sb.AppendLine($"{ts}#if (defined AUTOMAAT || defined AUTOMAAT_TEST)");
            foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                if(fc.FileDetectoren.Count == 0 && fc.HiaatDetectoren.Count == 0)
                {
                    continue;
                }

                var s = $"{ts}{ts}DD[{_fcpf}{fc.FaseCyclus}] = ";
                var l = new string(' ', s.Length);
                sb.AppendLine(s);
                sb.Append(l);
                foreach(var d in fc.FileDetectoren)
                {
                    sb.Append($"(CIF_IS[{_dpf}{d.Detector}] >= CIF_DET_STORING) || ");
                }
                sb.AppendLine();
                sb.Append(l);
                foreach (var d in fc.HiaatDetectoren)
                {
                    sb.Append($"(CIF_IS[{_dpf}{d.Detector}] >= CIF_DET_STORING) || ");
                }
                sb.AppendLine();
                if(c.FileIngrepen.Count > 0)
                {
                    var any = false;
                    foreach(var fi in c.FileIngrepen)
                    {
                        if(fi.TeDoserenSignaalGroepen.Any(x => x.FaseCyclus == fc.FaseCyclus))
                        {
                            any = true;
                            sb.Append($"{l}(IH[{_hpf}{_hfile}{fi.Naam}]) ||");
                        }
                    }
                    if(any)
                    {
                        sb.AppendLine();
                    }
                }
                sb.Append($"{l}(");
                var i = 0;
                foreach (var d in fc.FileDetectoren)
                {
                    if(i > 0)
                    {
                        sb.Append(" && ");
                    }
                    ++i;
                    sb.Append($"!T[{_tpf}{_tfd}{_dpf}{d.Detector}]"); 
                }
                sb.AppendLine(");");
            }
            sb.AppendLine($"{ts}#else");
            foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                if(fc.FileDetectoren.Count == 0 && fc.HiaatDetectoren.Count == 0)
                {
                    continue;
                }
                if (c.FileIngrepen.Count > 0)
                {
                    var any = false;
                    foreach (var fi in c.FileIngrepen)
                    {
                        if (fi.TeDoserenSignaalGroepen.Any(x => x.FaseCyclus == fc.FaseCyclus))
                        {
                            any = true;
                            sb.AppendLine($"{ts}{ts}DD[{_fcpf}{fc.FaseCyclus}] = IH[{_hpf}{_hfile}{fi.Naam}] ? TRUE : FALSE;");
                        }
                    }
                    if (!any)
                    {
                        sb.AppendLine($"{ts}{ts}DD[{_fcpf}{fc.FaseCyclus}] = FALSE;");
                    }
                }
                else
                {
                    sb.AppendLine($"{ts}{ts}DD[{_fcpf}{fc.FaseCyclus}] = FALSE;");
                }
            }
            sb.AppendLine($"{ts}#endif");
            sb.AppendLine();
            foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.FileDetectoren.Count == 0 && fc.HiaatDetectoren.Count == 0)
                {
                    continue;
                } 
                sb.AppendLine($"{ts}DD_anyfase |= DD[{_fcpf}{fc.FaseCyclus}];");
            }
            sb.AppendLine();
            sb.AppendLine($"{ts}/* Meetkriterium MK */");
            sb.AppendLine($"{ts}/* ---------------- */");
            foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen.Where(x => x.HiaatDetectoren.Any()))
            {
                var faseMetRgv = fasenMetRgv.First(x => x.Naam == fc.FaseCyclus);
                if (faseMetRgv.AantalRijstroken == 1)
                {
                    sb.AppendLine($"{ts}MK1[{_fcpf}{fc.FaseCyclus}] = SVG[{_fcpf}{fc.FaseCyclus}] || G[{_fcpf}{fc.FaseCyclus}] && MK1[{_fcpf}{fc.FaseCyclus}] && MK[{_fcpf}{fc.FaseCyclus}];");
                }
                else if (faseMetRgv.AantalRijstroken > 1)
                {
                    foreach(var d in fc.HiaatDetectoren)
                    {
                        sb.AppendLine($"{ts}RT[{_tpf}{_thd}{_dpf}{d.Detector}] = D[{_dpf}{d.Detector}];");
                    }
                    for (var i = 1; i <= faseMetRgv.AantalRijstroken; ++i)
                    {
                        if (!faseMetRgv.Detectoren.Any(x => x.Rijstrook == i)) continue;
                        var first = true;
                        foreach(var d in faseMetRgv.Detectoren.Where(x => x.Rijstrook == i))
                        {
                            var hd = fc.HiaatDetectoren.FirstOrDefault(x => x.Detector == d.Naam);
                            if(hd != null)
                            {
                                if(first)
                                {
                                    sb.Append($"{ts}MK{i}[{_fcpf}{fc.FaseCyclus}] = SVG[{_fcpf}{fc.FaseCyclus}] || G[{_fcpf}{fc.FaseCyclus}] && MK{i}[{_fcpf}{fc.FaseCyclus}] && (");
                                }
                                else
                                {
                                    sb.Append(" || ");
                                }
                                sb.Append($"RT[{_tpf}{_thd}{_dpf}{d.Naam}] || T[{_tpf}{_thd}{_dpf}{d.Naam}]");
                                first = false;
                            }
                        }
                        if (!first) sb.AppendLine(");");
                    }
                }
            }
            sb.AppendLine();
            sb.AppendLine($"{ts}/* Opslaan 'oudste' TVG tijd volgens RoBuGrover */");
            sb.AppendLine($"{ts}/* -------------------------------------------- */");
            sb.AppendLine($"{ts}for (i = 0; i < FCMAX; ++i) TVG_rgv_older[i] = TVG_rgv[i];");
            sb.AppendLine();
            sb.AppendLine($"{ts}/* Aanpassen verlenggroentijden op einde verlenggroen */");
            sb.AppendLine($"{ts}/* -------------------------------------------------- */");
            foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen.Where(x => x.HiaatDetectoren.Any()))
            {
                var faseMetRgv = fasenMetRgv.First(x => x.Naam == fc.FaseCyclus);
                if (faseMetRgv.AantalRijstroken == 1)
                {
                    sb.AppendLine($"{ts}rgv_verlenggroentijd1({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmmintvg}_{fc.FaseCyclus}], PRM[{_prmpf}{_prmmaxtvg}_{fc.FaseCyclus}], PRM[{_prmpf}{_prmtvg_omhoog}], PRM[{_prmpf}{_prmtvg_omlaag}], PRM[{_prmpf}{_prmtvg_verschil}], TVG_max[{_fcpf}{fc.FaseCyclus}], ({c.GetBoolV()})!SCH[{_schpf}{_schrgv_snel}], ({c.GetBoolV()})DD[{_fcpf}{fc.FaseCyclus}], ({c.GetBoolV()})(MK1[{_fcpf}{fc.FaseCyclus}]));");
                }
                else if (faseMetRgv.AantalRijstroken > 1)
                {
                    sb.Append($"{ts}rgv_verlenggroentijd2({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmmintvg}_{fc.FaseCyclus}], PRM[{_prmpf}{_prmmaxtvg}_{fc.FaseCyclus}], PRM[{_prmpf}{_prmtvg_omhoog}], PRM[{_prmpf}{_prmtvg_omlaag}], PRM[{_prmpf}{_prmtvg_verschil}], TVG_max[{_fcpf}{fc.FaseCyclus}], ({c.GetBoolV()})!SCH[{_schpf}{_schrgv_snel}], ({c.GetBoolV()})DD[{_fcpf}{fc.FaseCyclus}], ");
                    sb.Append($"({c.GetBoolV()})(");
                    for (var i = 1; i <= faseMetRgv.AantalRijstroken; ++i)
                    {
                        if (i != 1) sb.Append(" && ");
                        sb.Append($"MK{i}[{_fcpf}{fc.FaseCyclus}]");
                    }
                    sb.AppendLine("));");
                }
            }
            sb.AppendLine();
            sb.AppendLine($"{ts}/* Verlaag de verlenggroentijd indien geen primaire realisatie in de cyclus */");
            sb.AppendLine($"{ts}/* ------------------------------------------------------------------------ */");
            foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.FileDetectoren.Count == 0 && fc.HiaatDetectoren.Count == 0)
                {
                    continue;
                }
                if (!c.Data.MultiModuleReeksen)
                {
                    sb.AppendLine($"{ts}rgv_niet_primair({_fcpf}{fc.FaseCyclus}, PRML, ML, SML, ML_MAX, {_hpf}{_hprreal}{fc.FaseCyclus}, PRM[{_prmpf}{_prmmintvg}_{fc.FaseCyclus}], PRM[{_prmpf}{_prmtvg_npr_omlaag}], ({c.GetBoolV()})(DD[{_fcpf}{fc.FaseCyclus}]));");
                }
                else
                {
                    var r = c.MultiModuleMolens.FirstOrDefault(x => x.Modules.Any(x2 => x2.Fasen.Any(x3 => x3.FaseCyclus == fc.FaseCyclus)));
                    if (r != null)
                    {
                        sb.AppendLine($"{ts}rgv_niet_primair({_fcpf}{fc.FaseCyclus}, PR{r.Reeks}, {r.Reeks}, S{r.Reeks}, {r.Reeks}_MAX, {_hpf}{_hprreal}{fc.FaseCyclus}, PRM[{_prmpf}{_prmmintvg}_{fc.FaseCyclus}], PRM[{_prmpf}{_prmtvg_npr_omlaag}], ({c.GetBoolV()})(DD[{_fcpf}{fc.FaseCyclus}]));");
                    }
                }
            }
            sb.AppendLine();
            sb.AppendLine($"{ts}/* Opslaan 'oude' TVG tijd volgens RoBuGrover */");
            sb.AppendLine($"{ts}/* ------------------------------------------ */");
            sb.AppendLine($"{ts}for (i = 0; i < FCMAX; ++i) TVG_rgv_old[i] = TVG_rgv[i];");
            sb.AppendLine();
            sb.AppendLine($"{ts}/* correctie verlenggroentijden t.o.v. de maximum gewenste cyclustijd */");
            sb.AppendLine($"{ts}/* ------------------------------------------------------------------ */");

            var sb2 = new StringBuilder();
            sb2.AppendLine($"{ts}#if (defined AUTOMAAT || defined AUTOMAAT_TEST) && (!defined VISSIM)");
            foreach(var gr in c.RoBuGrover.ConflictGroepen)
            {
                sb2.Append($"{ts}{ts}rgv_verlenggroentijd_correctie_va_arg(PRM[{_prmpf}{_prmrgv}], DD_anyfase, PRM[{_prmpf}min_tcyclus], PRM[{_prmpf}max_tcyclus], ");
                foreach(var fc in gr.Fasen)
                {
                    sb2.Append($"{_fcpf}{fc.FaseCyclus}, ");
                }
                sb2.AppendLine($"END);");
            }
            sb2.AppendLine($"{ts}#else");
            sb2.AppendLine($"{ts}{ts}for (teller = 0; teller < MAX_AANTAL_CONFLICTGROEPEN; ++teller) TC_rgv[teller] = 0;");
            sb2.AppendLine($"{ts}{ts}teller = 0;");
            foreach (var gr in c.RoBuGrover.ConflictGroepen)
            {
                sb2.Append($"{ts}{ts}TC_rgv[teller++] = rgv_verlenggroentijd_correctie_va_arg(PRM[{_prmpf}{_prmrgv}], DD_anyfase, PRM[{_prmpf}{_prmmin_tcyclus}], PRM[{_prmpf}{_prmmax_tcyclus}], ");
                foreach (var fc in gr.Fasen)
                {
                    sb2.Append($"{_fcpf}{fc.FaseCyclus}, ");
                }
                sb2.AppendLine("END);");
            }
            sb2.AppendLine($"{ts}#endif");
            sb.Append(sb2.ToString());

            sb.AppendLine();
            sb.AppendLine($"{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || (defined VISSIM)");
            sb.AppendLine($"{ts}{ts}teller = 0;");
            foreach (var gr in c.RoBuGrover.ConflictGroepen)
            {
                sb.Append($"{ts}{ts}TC_string$[teller++] = \"");
                var i = 0;
                foreach (var fc in gr.Fasen)
                {
                    sb.Append($"{fc.FaseCyclus}");
                    ++i;
                    if (i < gr.Fasen.Count)
                    {
                        sb.Append(" ");
                    }
                }
                sb.AppendLine("\";");
            }
            sb.AppendLine($"{ts}#endif");
            sb.AppendLine();
            sb.AppendLine($"{ts}/* corrigeer voor te veel verlaagde groentijden */");
            sb.AppendLine($"{ts}/* -------------------------------------------- */");
            sb.AppendLine($"{ts}/* (a.g.v. minder verhoogde groentijd dan waarop de verlaagde groentijd was aangepast) */");
            sb.AppendLine($"{ts}for(i = 0; i < FCMAX; ++i) ");
            sb.AppendLine($"{ts}{{");
            sb.AppendLine($"{ts}{ts}if ((TVG_rgv_old[i] > TVG_rgv_older[i]) && (TVG_rgv[i] < TVG_rgv_old[i]))");
            sb.AppendLine($"{ts}{ts}{{");
            sb.AppendLine($"{ts}{ts}{ts}for(j = 0; j < FCMAX; ++j)");
            sb.AppendLine($"{ts}{ts}{ts}{{");
            sb.AppendLine($"{ts}{ts}{ts}{ts}if(j != i)");
            sb.AppendLine($"{ts}{ts}{ts}{ts}{{");
            sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}TVG_rgv[j] = TVG_rgv_old[j]; /* minder verhoogde groentijd */");
            sb.AppendLine($"{ts}{ts}{ts}{ts}}}");
            sb.AppendLine($"{ts}{ts}{ts}}}");
            sb.Append(sb2.ToString());
            sb.AppendLine($"{ts}{ts}}}");
            sb.AppendLine($"{ts}}}");
            sb.AppendLine();
            sb.AppendLine($"{ts}/* Kopieer de rgv-waarden naar TVG_max */");
            sb.AppendLine($"{ts}/* ----------------------------------- */");
            foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.FileDetectoren.Count == 0 && fc.HiaatDetectoren.Count == 0)
                {
                    sb.AppendLine($"{ts}TVG_max[{_fcpf}{fc.FaseCyclus}] = TVG_basis[{_fcpf}{fc.FaseCyclus}];");
                }
                else
                {
                    sb.AppendLine($"{ts}TVG_max[{_fcpf}{fc.FaseCyclus}] = TVG_rgv[{_fcpf}{fc.FaseCyclus}];");
                }
            }
            sb.AppendLine();
            sb.AppendLine($"{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || (defined (VISSIM))");
            sb.AppendLine($"{ts}{ts}/* Toon de waarden in de tesstomgeving */");
            sb.AppendLine($"{ts}{ts}/* ----------------------------------- */ ");
            sb.AppendLine($"{ts}{ts}for (teller = 10; teller < MAX_AANTAL_CONFLICTGROEPEN; ++teller)");
            sb.AppendLine($"{ts}{ts}{{");
            sb.AppendLine($"{ts}{ts}{ts}xyprintf (32, teller + 1, \"%10s\",TC_string$[teller]);");
            sb.AppendLine($"{ts}{ts}{ts}xyprintf (43, teller + 1, \":%4d\", TC_rgv[teller]);");
            sb.AppendLine($"{ts}{ts}}}");
            var teller = 2;
            foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                sb.AppendLine($"{ts}{ts}xyprintf (32, teller + {teller}, \"TVG{fc.FaseCyclus}=%4d\", TVG_max[{_fcpf}{fc.FaseCyclus}]);");
                ++teller;
            }
            sb.AppendLine($"{ts}{ts}");
            if (c.RoBuGrover.RoBuGroverVenster)
            {
                sb.AppendLine($"{ts}{ts}#ifndef DUURTEST");
                sb.AppendLine($"{ts}{ts}{ts}MG_Bars_init(TVG_basis, TVG_rgv, 10, 750, 0, 0);");
                sb.Append($"{ts}{ts}{ts}MG_Fasen_Venster_init(SYSTEM, ");
                foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen)
                {
                    sb.Append($"{_fcpf}{fc.FaseCyclus}, ");
                }
                sb.AppendLine("END);");
                sb.AppendLine($"{ts}{ts}{ts}MG_Bars();");
                sb.AppendLine($"{ts}{ts}#endif");
            }
            sb.AppendLine($"{ts}#endif ");
            sb.AppendLine("}");
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
