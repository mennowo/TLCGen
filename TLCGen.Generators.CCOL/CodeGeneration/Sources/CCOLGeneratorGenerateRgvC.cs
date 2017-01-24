using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateRgvC(ControllerModel controller)
        {
            if(controller.RoBuGrover.ConflictGroepen?.Count == 0)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* IMPLEMENTATIE ROBUGROVER */");
            sb.AppendLine("/* ------------------------ */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "rgv.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.AppendLine("/* defines voor ROBUGROVER */");
            sb.AppendLine($"#define MAX_AANTAL_CONFLICTGROEPEN {controller.RoBuGrover.ConflictGroepen.Count}");
            sb.AppendLine("mulv TC[MAX_AANTAL_CONFLICTGROEPEN];");
            sb.AppendLine("mulv TC_max, DD_anyfase;");
            sb.AppendLine("mulv TO_ontwerp[FCMAX][FCMAX];");
            sb.AppendLine("");
            sb.AppendLine("#if (!defined AUTOMAAT) || (defined VISSIM)");
            sb.AppendLine($"{tabspace}mulv TC_rgv[MAX_AANTAL_CONFLICTGROEPEN];");
            sb.AppendLine($"{tabspace}string TC_string$[MAX_AANTAL_CONFLICTGROEPEN];");
            sb.AppendLine("#endif");
            sb.AppendLine("");
            sb.AppendLine("/* Robugrover includes */");
            sb.AppendLine("#include \"rgvfunc.c\"");
            sb.AppendLine("#include \"rgv_overslag.c\"");
            sb.AppendLine("");
            sb.AppendLine("#if (!defined AUTOMAAT) || (defined VISSIM)");
            sb.AppendLine($"{tabspace}#include \"winmg.c\"");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("void rgv_add(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{tabspace}static bool DD[FCMAX];            /* Detectie storing (Detection Disabled) */");
            sb.AppendLine($"{tabspace}static bool MK1[FCMAX];           /* Meetkriterium op rijstrook 1 */");
            if(controller.RoBuGrover.SignaalGroepInstellingen.Where(x => x.HiaatDetectoren.Count > 1).Any())
            {
                sb.AppendLine($"{tabspace}static bool MK2[FCMAX];           /* Meetkriterium op rijstrook 2 */");
            }
            if (controller.RoBuGrover.SignaalGroepInstellingen.Where(x => x.HiaatDetectoren.Count > 2).Any())
            {
                sb.AppendLine($"{tabspace}static bool MK3[FCMAX];           /* Meetkriterium op rijstrook 3 */");
            }
            sb.AppendLine($"{tabspace}static mulv TVG_rgv_old[FCMAX];   /* Opslag 'old' TVG tijden */");
            sb.AppendLine($"{tabspace}static mulv TVG_rgv_older[FCMAX]; /* Opslag 'older' TVG tijden */");
            sb.AppendLine($"{tabspace}static mulv rgvinit = 1;          /* Onthouden initialisatie */");
            sb.AppendLine($"{tabspace}int teller = 0;");
            sb.AppendLine("#ifdef AUTOMAAT");
            sb.AppendLine($"{tabspace}int fc;");
            sb.AppendLine("#endif");
            sb.AppendLine($"{tabspace}int i, j;");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}for(i = 0; i < FCMAX; ++i)");
            sb.AppendLine($"{tabspace}{{");
            sb.AppendLine($"{tabspace}{tabspace}for(j = 0; j < FCMAX; ++j) ");
            sb.AppendLine($"{tabspace}{tabspace}{{");
            sb.AppendLine($"{tabspace}{tabspace}{tabspace}TO_ontwerp[i][j]=TO_max[i][j];");
            sb.AppendLine($"{tabspace}{tabspace}}}");
            sb.AppendLine($"{tabspace}}}");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}/* intitieer waarden TGV_rgv */");
            sb.AppendLine($"{tabspace}/* ------------------------- */");
            sb.AppendLine($"{tabspace}if(rgvinit)");
            sb.AppendLine($"{tabspace}{{");
            foreach(var fc in controller.RoBuGrover.SignaalGroepInstellingen)
            {
#warning Change so it uses prefix settings (also everywhere here!) also other stuff like timers, names of timers, etc.
                sb.AppendLine($"{tabspace}{tabspace}TVG_rgv[fc{fc.FaseCyclus}] = TVG_max[fc{fc.FaseCyclus}];");
            }
            sb.AppendLine($"{tabspace}{tabspace}rgvinit = 0;");
            sb.AppendLine($"{tabspace}}}");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}/* kopieer de basis waarden van TVG_max */");
            sb.AppendLine($"{tabspace}/* ------------------------------------ */");
            foreach (var fc in controller.RoBuGrover.SignaalGroepInstellingen)
            {
                sb.AppendLine($"{tabspace}TVG_basis[fc{fc.FaseCyclus}] = TVG_max[fc{fc.FaseCyclus}] > 0 ? TVG_max[fc{fc.FaseCyclus}] : 1;");
            }
            sb.AppendLine();
            sb.AppendLine($"{tabspace}/* detectiestoringen voor de fasecycli */");
            sb.AppendLine($"{tabspace}/* ----------------------------------- */");
            foreach (var fc in controller.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.FileDetectoren.Count > 0)
                {
                    foreach (var d in fc.FileDetectoren)
                    {
                        sb.AppendLine($"{tabspace}RT[tfd{d.Detector}] = SD[d{d.Detector}] || ED[d{d.Detector}] || !VG[fc{fc.FaseCyclus}]; ");
                    }
                }
            }
            sb.AppendLine();
            sb.AppendLine($"{tabspace}DD_anyfase = 0;");
            sb.AppendLine($"{tabspace}#if defined (AUTOMAAT)");
            foreach (var fc in controller.RoBuGrover.SignaalGroepInstellingen)
            {
                if(fc.FileDetectoren.Count == 0 && fc.HiaatDetectoren.Count == 0)
                {
                    continue;
                }

                var s = $"{tabspace}{tabspace}DD[fc{fc.FaseCyclus}] = ";
                var l = new string(' ', s.Length);
                sb.AppendLine(s);
                sb.Append(l);
                foreach(var d in fc.FileDetectoren)
                {
                    sb.Append($"(CIF_IS[d{d.Detector}] >= CIF_DET_STORING) || ");
                }
                sb.AppendLine();
                sb.Append(l);
                foreach (var d in fc.HiaatDetectoren)
                {
                    sb.Append($"(CIF_IS[d{d.Detector}] >= CIF_DET_STORING) || ");
                }
                sb.AppendLine();
                sb.Append($"{l}(");
                foreach (var d in fc.FileDetectoren)
                {
                    sb.Append($"!T[tf{d.Detector}] &&"); 
                }
                sb.AppendLine(");");
            }
            sb.AppendLine($"{tabspace}#else");
            foreach (var fc in controller.RoBuGrover.SignaalGroepInstellingen)
            {
                if(fc.FileDetectoren.Count == 0 && fc.HiaatDetectoren.Count == 0)
                {
                    continue;
                }
                sb.AppendLine($"{tabspace}{tabspace}DD[fc{fc.FaseCyclus}] = FALSE;");
            }
            sb.AppendLine($"{tabspace}#endif");
            sb.AppendLine();
            foreach (var fc in controller.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.FileDetectoren.Count == 0 && fc.HiaatDetectoren.Count == 0)
                {
                    continue;
                } 
                sb.AppendLine($"{tabspace}DD_anyfase |= DD[fc{fc.FaseCyclus}];");
            }
            sb.AppendLine();
            sb.AppendLine($"{tabspace}/* Meetkriterium MK */");
            sb.AppendLine($"{tabspace}/* ---------------- */");
            foreach (var fc in controller.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.HiaatDetectoren.Count == 1)
                {
                    sb.AppendLine($"{tabspace}MK1[fc{fc.FaseCyclus}] = SVG[fc{fc.FaseCyclus}] || G[fc{fc.FaseCyclus}] && MK1[fc{fc.FaseCyclus}] && MK[fc{fc.FaseCyclus}];");
                }
                else if (fc.HiaatDetectoren.Count > 1)
                {
                    foreach(var d in fc.HiaatDetectoren)
                    {
                        sb.AppendLine($"{tabspace}RT[tdh{d.Detector}] = D[d{d.Detector}];");
                    }
                    int i = 1;
                    foreach (var d in fc.HiaatDetectoren)
                    {
                        sb.AppendLine($"{tabspace}MK{i}[fc{fc.FaseCyclus}] = SVG[fc{fc.FaseCyclus}] || G[fc{fc.FaseCyclus}] && MK{i}[fc{fc.FaseCyclus}] && (RT[tdh{d.Detector}] || T[tdh{d.Detector}]);");
                        ++i;
                    }
                }
            }
            sb.AppendLine();
            sb.AppendLine($"{tabspace}/* Opslaan 'oudste' TVG tijd volgens RoBuGrover */");
            sb.AppendLine($"{tabspace}/* -------------------------------------------- */");
            sb.AppendLine($"{tabspace}for (i = 0; i < FCMAX; ++i) TVG_rgv_older[i] = TVG_rgv[i];");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}/* Aanpassen verlenggroentijden op einde verlenggroen */");
            sb.AppendLine($"{tabspace}/* -------------------------------------------------- */");
            foreach (var fc in controller.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.HiaatDetectoren.Count == 1)
                {
                    sb.AppendLine($"{tabspace}rgv_verlenggroentijd2(fc{fc.FaseCyclus}, PRM[prmmintvg_{fc.FaseCyclus}], PRM[prmmaxtvg_{fc.FaseCyclus}], PRM[prmtvg_omhoog], PRM[prmtvg_omlaag], PRM[prmtvg_verschil], TVG_max[fc{fc.FaseCyclus}], (bool)!SCH[schrgvsnel], (bool)DD[fc{fc.FaseCyclus}], (bool)(MK1[fc{fc.FaseCyclus}]));");
                }
                else if (fc.HiaatDetectoren.Count > 1)
                {
                    sb.Append($"{tabspace}rgv_verlenggroentijd2(fc{fc.FaseCyclus}, PRM[prmmintvg_{fc.FaseCyclus}], PRM[prmmaxtvg_{fc.FaseCyclus}], PRM[prmtvg_omhoog], PRM[prmtvg_omlaag], PRM[prmtvg_verschil], TVG_max[fc{fc.FaseCyclus}], (bool)!SCH[schrgvsnel], (bool)DD[fc{fc.FaseCyclus}], ");
                    int i = 1;
                    sb.Append("(bool)(");
                    foreach (var d in fc.HiaatDetectoren)
                    {
                        sb.Append($"MK{i}[fc{fc.FaseCyclus}]");
                        ++i;
                        if(i <= fc.HiaatDetectoren.Count)
                        {
                            sb.Append(" || ");
                        }
                    }
                    sb.AppendLine("));");
                }
            }
            sb.AppendLine();
            sb.AppendLine($"{tabspace}/* Verlaag de verlenggroentijd indien geen primaire realisatie in de cyclus */");
            sb.AppendLine($"{tabspace}/* ------------------------------------------------------------------------ */");
            foreach (var fc in controller.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.FileDetectoren.Count == 0 && fc.HiaatDetectoren.Count == 0)
                {
                    continue;
                }
                sb.AppendLine($"{tabspace}rgv_niet_primair(fc{fc.FaseCyclus}, PRML, ML, ML_MAX, hprreal{fc.FaseCyclus}, PRM[prmmintvg_{fc.FaseCyclus}], PRM[prmtvg_npr_omlaag], (bool)(DD[fc{fc.FaseCyclus}]));");
            }
            sb.AppendLine();
            sb.AppendLine($"{tabspace}/* Opslaan 'oude' TVG tijd volgens RoBuGrover */");
            sb.AppendLine($"{tabspace}/* ------------------------------------------ */");
            sb.AppendLine($"{tabspace}for (i = 0; i < FCMAX; ++i) TVG_rgv_old[i] = TVG_rgv[i];");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}/* correctie verlenggroentijden t.o.v. de maximum gewenste cyclustijd */");
            sb.AppendLine($"{tabspace}/* ------------------------------------------------------------------ */");

            StringBuilder sb2 = new StringBuilder();
            sb2.AppendLine($"{tabspace}#if (defined AUTOMAAT) && (!defined VISSIM)");
            foreach(var gr in controller.RoBuGrover.ConflictGroepen)
            {
                sb2.Append($"{tabspace}{tabspace}rgv_verlenggroentijd_correctie_va_arg(PRM[prmrgv], DD_anyfase, PRM[prmmin_tcyclus], PRM[prmmax_tcyclus], ");
                foreach(var fc in gr.Fasen)
                {
                    sb2.Append($"fc{fc.FaseCyclus}, ");
                }
                sb2.AppendLine($"END);");
            }
            sb2.AppendLine($"{tabspace}#else");
            sb2.AppendLine($"{tabspace}{tabspace}for (teller = 0; teller < MAX_AANTAL_CONFLICTGROEPEN; ++teller) TC_rgv[teller] = 0;");
            sb2.AppendLine($"{tabspace}{tabspace}teller = 0;");
            foreach (var gr in controller.RoBuGrover.ConflictGroepen)
            {
                sb2.Append($"{tabspace}{tabspace}TC_rgv[teller++] = rgv_verlenggroentijd_correctie_va_arg(PRM[prmrgv], DD_anyfase, PRM[prmmin_tcyclus], PRM[prmmax_tcyclus], ");
                foreach (var fc in gr.Fasen)
                {
                    sb2.Append($"fc{fc.FaseCyclus}, ");
                }
                sb2.AppendLine("END);");
            }
            sb2.AppendLine($"{tabspace}#endif");
            sb.Append(sb2.ToString());

            sb.AppendLine();
            sb.AppendLine($"{tabspace}#if (!defined AUTOMAAT) || (defined VISSIM)");
            sb.AppendLine($"{tabspace}{tabspace}teller = 0;");
            foreach (var gr in controller.RoBuGrover.ConflictGroepen)
            {
                sb.Append($"{tabspace}{tabspace}TC_string$[teller++] = \"");
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
            sb.AppendLine($"{tabspace}#endif");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}/* corrigeer voor te veel verlaagde groentijden */");
            sb.AppendLine($"{tabspace}/* -------------------------------------------- */");
            sb.AppendLine($"{tabspace}/* (a.g.v. minder verhoogde groentijd dan waarop de verlaagde groentijd was aangepast) */");
            sb.AppendLine($"{tabspace}for(i = 0; i < FCMAX; ++i) ");
            sb.AppendLine($"{tabspace}{{");
            sb.AppendLine($"{tabspace}{tabspace}if ((TVG_rgv_old[i] > TVG_rgv_older[i]) && (TVG_rgv[i] < TVG_rgv_old[i]))");
            sb.AppendLine($"{tabspace}{tabspace}{{");
            sb.AppendLine($"{tabspace}{tabspace}{tabspace}for(j = 0; j < FCMAX; ++j)");
            sb.AppendLine($"{tabspace}{tabspace}{tabspace}{{");
            sb.AppendLine($"{tabspace}{tabspace}{tabspace}{tabspace}if(j != i)");
            sb.AppendLine($"{tabspace}{tabspace}{tabspace}{tabspace}{{");
            sb.AppendLine($"{tabspace}{tabspace}{tabspace}{tabspace}{tabspace}TVG_rgv[j] = TVG_rgv_old[j]; /* minder verhoogde groentijd */");
            sb.AppendLine($"{tabspace}{tabspace}{tabspace}{tabspace}}}");
            sb.AppendLine($"{tabspace}{tabspace}{tabspace}}}");
            sb.Append(sb2.ToString());
            sb.AppendLine($"{tabspace}{tabspace}}}");
            sb.AppendLine($"{tabspace}}}");
            sb.AppendLine();
            sb.AppendLine($"{tabspace}/* Kopieer de rgv-waarden naar TVG_max */");
            sb.AppendLine($"{tabspace}/* ----------------------------------- */");
            foreach (var fc in controller.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.FileDetectoren.Count == 0 && fc.HiaatDetectoren.Count == 0)
                {
                    sb.AppendLine($"{tabspace}TVG_max[fc{fc.FaseCyclus}] = TVG_basis[fc{fc.FaseCyclus}];");
                }
                else
                {
                    sb.AppendLine($"{tabspace}TVG_max[fc{fc.FaseCyclus}] = TVG_rgv[fc{fc.FaseCyclus}];");
                }
            }
            sb.AppendLine();
            sb.AppendLine($"{tabspace}#if !defined (AUTOMAAT) || (defined (VISSIM))");
            sb.AppendLine($"{tabspace}{tabspace}/* Toon de waarden in de tesstomgeving */");
            sb.AppendLine($"{tabspace}{tabspace}/* ----------------------------------- */ ");
            sb.AppendLine($"{tabspace}{tabspace}for (teller = 0; teller < MAX_AANTAL_CONFLICTGROEPEN; ++teller)");
            sb.AppendLine($"{tabspace}{tabspace}{{");
            sb.AppendLine($"{tabspace}{tabspace}{tabspace}xyprintf (30, teller + 1, \"%10s\",TC_string$[teller]);");
            sb.AppendLine($"{tabspace}{tabspace}{tabspace}xyprintf (41, teller + 1, \":%4d\", TC_rgv[teller]);");
            sb.AppendLine($"{tabspace}{tabspace}}}");
            foreach (var fc in controller.RoBuGrover.SignaalGroepInstellingen)
            {
                sb.AppendLine($"{tabspace}{tabspace}xyprintf (30, teller+2, \"TVG{fc.FaseCyclus}=%4d\", TVG_max[fc{fc.FaseCyclus}]);");
            }
            sb.AppendLine($"{tabspace}{tabspace}");
            sb.AppendLine($"{tabspace}{tabspace}#ifndef DUURTEST");
            sb.AppendLine($"{tabspace}{tabspace}{tabspace}MG_Bars_init(TVG_basis, TVG_rgv, 10, 750, 0, 0);");
            sb.Append($"{tabspace}{tabspace}{tabspace}MG_Fasen_Venster_init(SYSTEM, ");
            foreach (var fc in controller.RoBuGrover.SignaalGroepInstellingen)
            {
                sb.Append($"fc{fc.FaseCyclus}, ");
            }
            sb.AppendLine("END);");
            sb.AppendLine($"{tabspace}{tabspace}{tabspace}MG_Bars();");
            sb.AppendLine($"{tabspace}{tabspace}#endif");
            sb.AppendLine($"{tabspace}#endif ");
            sb.AppendLine("}");
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
