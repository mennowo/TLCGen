using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

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

            string _prmrgv = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrgv");
            string _prmmin_tcyclus = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmin_tcyclus");
            string _prmmax_tcyclus = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmax_tcyclus");
            string _prmmintvg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmintvg");
            string _prmmaxtvg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmmaxtvg");
            string _prmtvg_omhoog = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_omhoog");
            string _prmtvg_omlaag = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_omlaag");
            string _prmtvg_verschil = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_verschil");
            string _prmtvg_npr_omlaag = CCOLGeneratorSettingsProvider.Default.GetElementName("prmtvg_npr_omlaag");
            string _hprreal = CCOLGeneratorSettingsProvider.Default.GetElementName("hprreal");
            string _schrgv = CCOLGeneratorSettingsProvider.Default.GetElementName("schrgv");
            string _schrgv_snel = CCOLGeneratorSettingsProvider.Default.GetElementName("schrgv_snel");
            string _tfd = CCOLGeneratorSettingsProvider.Default.GetElementName("tfd");
            string _thd = CCOLGeneratorSettingsProvider.Default.GetElementName("thd");

            string _hfile = CCOLGeneratorSettingsProvider.Default.GetElementName("hfile");

            StringBuilder sb = new StringBuilder();
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
            sb.AppendLine("mulv TO_ontwerp[FCMAX][FCMAX];");
            sb.AppendLine("");
            sb.AppendLine("#if (!defined AUTOMAAT) || (defined VISSIM)");
            sb.AppendLine($"{ts}mulv TC_rgv[MAX_AANTAL_CONFLICTGROEPEN];");
            sb.AppendLine($"{ts}string TC_string$[MAX_AANTAL_CONFLICTGROEPEN];");
            sb.AppendLine("#endif");
            sb.AppendLine("");
            sb.AppendLine("/* Robugrover includes */");
            sb.AppendLine("#include \"rgvfunc.c\"");
            sb.AppendLine("#include \"rgv_overslag.c\"");
            sb.AppendLine("");
            if (c.RoBuGrover.RoBuGroverVenster)
            {
                sb.AppendLine("#if (!defined AUTOMAAT) || (defined VISSIM)");
                sb.AppendLine($"{ts}#include \"winmg.c\"");
                sb.AppendLine("#endif");
            }
            sb.AppendLine();
            sb.AppendLine("void rgv_add(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{ts}static bool DD[FCMAX];            /* Detectie storing (Detection Disabled) */");
            sb.AppendLine($"{ts}static bool MK1[FCMAX];           /* Meetkriterium op rijstrook 1 */");
            if(c.RoBuGrover.SignaalGroepInstellingen.Where(x => x.HiaatDetectoren.Count > 1).Any())
            {
                sb.AppendLine($"{ts}static bool MK2[FCMAX];           /* Meetkriterium op rijstrook 2 */");
            }
            if (c.RoBuGrover.SignaalGroepInstellingen.Where(x => x.HiaatDetectoren.Count > 2).Any())
            {
                sb.AppendLine($"{ts}static bool MK3[FCMAX];           /* Meetkriterium op rijstrook 3 */");
            }
            sb.AppendLine($"{ts}static mulv TVG_rgv_old[FCMAX];   /* Opslag 'old' TVG tijden */");
            sb.AppendLine($"{ts}static mulv TVG_rgv_older[FCMAX]; /* Opslag 'older' TVG tijden */");
            sb.AppendLine($"{ts}static mulv rgvinit = 1;          /* Onthouden initialisatie */");
            sb.AppendLine($"{ts}int teller = 0;");
            sb.AppendLine("#ifdef AUTOMAAT");
            sb.AppendLine($"{ts}int fc;");
            sb.AppendLine("#endif");
            sb.AppendLine($"{ts}int i, j;");
            sb.AppendLine();
            sb.AppendLine($"{ts}for(i = 0; i < FCMAX; ++i)");
            sb.AppendLine($"{ts}{{");
            sb.AppendLine($"{ts}{ts}for(j = 0; j < FCMAX; ++j) ");
            sb.AppendLine($"{ts}{ts}{{");
            sb.AppendLine($"{ts}{ts}{ts}TO_ontwerp[i][j]=TO_max[i][j];");
            sb.AppendLine($"{ts}{ts}}}");
            sb.AppendLine($"{ts}}}");
            sb.AppendLine();
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
            sb.AppendLine($"{ts}#if defined (AUTOMAAT)");
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
                int i = 0;
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
            foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.HiaatDetectoren.Count == 1)
                {
                    sb.AppendLine($"{ts}MK1[{_fcpf}{fc.FaseCyclus}] = SVG[{_fcpf}{fc.FaseCyclus}] || G[{_fcpf}{fc.FaseCyclus}] && MK1[{_fcpf}{fc.FaseCyclus}] && MK[{_fcpf}{fc.FaseCyclus}];");
                }
                else if (fc.HiaatDetectoren.Count > 1)
                {
                    foreach(var d in fc.HiaatDetectoren)
                    {
                        sb.AppendLine($"{ts}RT[{_tpf}{_thd}{_dpf}{d.Detector}] = D[{_dpf}{d.Detector}];");
                    }
                    int i = 1;
                    foreach (var d in fc.HiaatDetectoren)
                    {
                        sb.AppendLine($"{ts}MK{i}[{_fcpf}{fc.FaseCyclus}] = SVG[{_fcpf}{fc.FaseCyclus}] || G[{_fcpf}{fc.FaseCyclus}] && MK{i}[{_fcpf}{fc.FaseCyclus}] && (RT[{_tpf}{_thd}{_dpf}{d.Detector}] || T[{_tpf}{_thd}{_dpf}{d.Detector}]);");
                        ++i;
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
            foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.HiaatDetectoren.Count == 1)
                {
                    sb.AppendLine($"{ts}rgv_verlenggroentijd1({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmmintvg}_{fc.FaseCyclus}], PRM[{_prmpf}{_prmmaxtvg}_{fc.FaseCyclus}], PRM[{_prmpf}{_prmtvg_omhoog}], PRM[{_prmpf}{_prmtvg_omlaag}], PRM[{_prmpf}{_prmtvg_verschil}], TVG_max[{_fcpf}{fc.FaseCyclus}], (bool)!SCH[{_schpf}{_schrgv_snel}], (bool)DD[{_fcpf}{fc.FaseCyclus}], (bool)(MK1[{_fcpf}{fc.FaseCyclus}]));");
                }
                else if (fc.HiaatDetectoren.Count > 1)
                {
                    sb.Append($"{ts}rgv_verlenggroentijd2({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmmintvg}_{fc.FaseCyclus}], PRM[{_prmpf}{_prmmaxtvg}_{fc.FaseCyclus}], PRM[{_prmpf}{_prmtvg_omhoog}], PRM[{_prmpf}{_prmtvg_omlaag}], PRM[{_prmpf}{_prmtvg_verschil}], TVG_max[{_fcpf}{fc.FaseCyclus}], (bool)!SCH[{_schpf}{_schrgv_snel}], (bool)DD[{_fcpf}{fc.FaseCyclus}], ");
                    int i = 1;
                    sb.Append("(bool)(");
                    foreach (var d in fc.HiaatDetectoren)
                    {
                        sb.Append($"MK{i}[{_fcpf}{fc.FaseCyclus}]");
                        ++i;
                        if(i <= fc.HiaatDetectoren.Count)
                        {
                            sb.Append(" && ");
                        }
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
                sb.AppendLine($"{ts}rgv_niet_primair({_fcpf}{fc.FaseCyclus}, PRML, ML, ML_MAX, {_hpf}{_hprreal}{fc.FaseCyclus}, PRM[{_prmpf}{_prmmintvg}_{fc.FaseCyclus}], PRM[{_prmpf}{_prmtvg_npr_omlaag}], (bool)(DD[{_fcpf}{fc.FaseCyclus}]));");
            }
            sb.AppendLine();
            sb.AppendLine($"{ts}/* Opslaan 'oude' TVG tijd volgens RoBuGrover */");
            sb.AppendLine($"{ts}/* ------------------------------------------ */");
            sb.AppendLine($"{ts}for (i = 0; i < FCMAX; ++i) TVG_rgv_old[i] = TVG_rgv[i];");
            sb.AppendLine();
            sb.AppendLine($"{ts}/* correctie verlenggroentijden t.o.v. de maximum gewenste cyclustijd */");
            sb.AppendLine($"{ts}/* ------------------------------------------------------------------ */");

            StringBuilder sb2 = new StringBuilder();
            sb2.AppendLine($"{ts}#if (defined AUTOMAAT) && (!defined VISSIM)");
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
            sb.AppendLine($"{ts}#if (!defined AUTOMAAT) || (defined VISSIM)");
            sb.AppendLine($"{ts}{ts}teller = 0;");
            foreach (var gr in c.RoBuGrover.ConflictGroepen)
            {
                sb.Append($"{ts}{ts}TC_string$[teller++] = \"");
                int i = 0;
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
            sb.AppendLine($"{ts}#if !defined (AUTOMAAT) || (defined (VISSIM))");
            sb.AppendLine($"{ts}{ts}/* Toon de waarden in de tesstomgeving */");
            sb.AppendLine($"{ts}{ts}/* ----------------------------------- */ ");
            sb.AppendLine($"{ts}{ts}for (teller = 0; teller < MAX_AANTAL_CONFLICTGROEPEN; ++teller)");
            sb.AppendLine($"{ts}{ts}{{");
            sb.AppendLine($"{ts}{ts}{ts}xyprintf (30, teller + 1, \"%10s\",TC_string$[teller]);");
            sb.AppendLine($"{ts}{ts}{ts}xyprintf (41, teller + 1, \":%4d\", TC_rgv[teller]);");
            sb.AppendLine($"{ts}{ts}}}");
            foreach (var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                sb.AppendLine($"{ts}{ts}xyprintf (30, teller+2, \"TVG{fc.FaseCyclus}=%4d\", TVG_max[{_fcpf}{fc.FaseCyclus}]);");
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
