using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using TLCGen.Dependencies.Models.Enumerations;
using TLCGen.Dependencies.Providers;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.ProjectGeneration;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        #region Fields

        private CCOLElemListData _uitgangen;
        private CCOLElemListData _ingangen;
        private CCOLElemListData _hulpElementen;
        private CCOLElemListData _geheugenElementen;
        private CCOLElemListData _timers;
        private CCOLElemListData _counters;
        private CCOLElemListData _schakelaars;
        private CCOLElemListData _parameters;

        private readonly List<string> _allFiles = new List<string>();

        private List<DetectorModel> _alleDetectoren;

        private string _uspf;
        private string _ispf;
        private string _fcpf;
        private string _dpf;
        private string _tpf;
        private string _cpf;
        private string _schpf;
        private string _prmpf;
        private string _hpf;
        private string _mpf;

        private string ts => CCOLGeneratorSettingsProvider.Default.Settings.TabSpace ?? "";

        private readonly string _beginGeneratedHeader = "/* BEGIN GEGENEREERDE HEADER */";
        private readonly string _endGeneratedHeader = "/* EINDE GEGENEREERDE HEADER */";

        #endregion // Fields

        #region Properties

        public static List<ICCOLCodePieceGenerator> PieceGenerators { get; private set; }
        public static Dictionary<CCOLCodeTypeEnum, SortedDictionary<int, ICCOLCodePieceGenerator>> OrderedPieceGenerators { get; private set; }

        #endregion // Properties

        #region Commands
        #endregion // Commands

        #region Command Functionality
        #endregion // Command Functionality

        #region Public Methods

        private void PrepareGeneration(ControllerModel c)
        {
            CCOLGeneratorSettingsProvider.Default.Reset();
            CCOLElementCollector.Reset();

            _uspf = CCOLGeneratorSettingsProvider.Default.GetPrefix("us");
            _ispf = CCOLGeneratorSettingsProvider.Default.GetPrefix("is");
            _fcpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("fc");
            _dpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("d");
            _tpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("t");
            _schpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("sch");
            _hpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("h");
            _mpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("m");
            _cpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("c");
            _prmpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("prm");

            foreach (var pgen in PieceGenerators)
            {
                pgen.CollectCCOLElements(c);
            }

            var CCOLElementLists = CCOLElementCollector.CollectAllCCOLElements(c, PieceGenerators.OrderBy(x => x.ElementGenerationOrder).ToList());

            if (CCOLElementLists == null || CCOLElementLists.Length != 8)
                throw new IndexOutOfRangeException("Error collecting CCOL elements from controller.");

            foreach (var (pluginItems, plugin) in TLCGenPluginManager.Default.ApplicationPlugins)
            {
                if ((pluginItems & TLCGenPluginElems.IOElementProvider) != TLCGenPluginElems.IOElementProvider) continue;

                var elemprov = plugin as ITLCGenElementProvider;
                var elems = elemprov?.GetAllItems();
                if (elems == null) continue;
                foreach (var elem in elems)
                {
                    var ccolElement = elem as CCOLElement;
                    if (ccolElement == null) continue;
                    switch (ccolElement.Type)
                    {
                        case CCOLElementTypeEnum.Uitgang: 
                            ccolElement.IOElementData.ElementType = IOElementTypeEnum.Output; 
                            CCOLElementLists[0].Elements.Add(ccolElement); 
                            break;
                        case CCOLElementTypeEnum.Ingang: 
                            ccolElement.IOElementData.ElementType = IOElementTypeEnum.Input; 
                            CCOLElementLists[1].Elements.Add(ccolElement); 
                            break;
                        case CCOLElementTypeEnum.HulpElement: CCOLElementLists[2].Elements.Add(ccolElement); break;
                        case CCOLElementTypeEnum.GeheugenElement: CCOLElementLists[3].Elements.Add(ccolElement); break;
                        case CCOLElementTypeEnum.Timer: CCOLElementLists[4].Elements.Add(ccolElement); break;
                        case CCOLElementTypeEnum.Counter: CCOLElementLists[5].Elements.Add(ccolElement); break;
                        case CCOLElementTypeEnum.Schakelaar: CCOLElementLists[6].Elements.Add(ccolElement); break;
                        case CCOLElementTypeEnum.Parameter: CCOLElementLists[7].Elements.Add(ccolElement); break;
                    }
                }
            }

            _uitgangen = CCOLElementLists[0];
            _ingangen = CCOLElementLists[1];
            _hulpElementen = CCOLElementLists[2];
            _geheugenElementen = CCOLElementLists[3];
            _timers = CCOLElementLists[4];
            _counters = CCOLElementLists[5];
            _schakelaars = CCOLElementLists[6];
            _parameters = CCOLElementLists[7];

            if (c.Data.RangeerData.RangerenIngangen)
            {
                foreach (var ccolelem in _ingangen.Elements)
                {
                    var model = c.Data.RangeerData.RangeerIngangen.FirstOrDefault(x => x.Naam == ccolelem.Naam);
                    if (model != null) ccolelem.RangeerIndex = model.RangeerIndex;
                }
            }

            if (c.Data.RangeerData.RangerenUitgangen)
            {
                foreach (var ccolelem in _uitgangen.Elements)
                {
                    var model = c.Data.RangeerData.RangeerUitgangen.FirstOrDefault(x => x.Naam == ccolelem.Naam);
                    if (model != null) ccolelem.RangeerIndex = model.RangeerIndex;
                }
            }
            
            foreach (var l in CCOLElementLists)
            {
                l.SetMax();
            }

            CCOLElementCollector.AddAllMaxElements(CCOLElementLists);
        }

        public string GenerateSourceFiles(ControllerModel c, string sourcefilepath)
        {
            if (Directory.Exists(sourcefilepath))
            {
                try
                {
                    PrepareGeneration(c);

                    CollectAllIO(c);

                    _alleDetectoren = new List<DetectorModel>();
                    foreach (var fcm in c.Fasen)
                    {
                        foreach (var dm in fcm.Detectoren)
                            _alleDetectoren.Add(dm);
                    }
                    foreach (var dm in c.Detectoren)
                        _alleDetectoren.Add(dm);
                    foreach (var dm in c.SelectieveDetectoren)
                        _alleDetectoren.Add(dm);

                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}reg.c"), GenerateRegC(c), Encoding.Default);
                    _allFiles.Add($"{c.Data.Naam}reg.c");
                    if (!c.Data.NietGebruikenBitmap)
                    {
                        File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}dpl.c"), GenerateDplC(c), Encoding.Default);
                    }
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}tab.c"), GenerateTabC(c), Encoding.Default);
                    _allFiles.Add($"{c.Data.Naam}tab.c");
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}sim.c"), GenerateSimC(c), Encoding.Default);
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}sys.h"), GenerateSysH(c), Encoding.Default);
                    _allFiles.Add($"{c.Data.Naam}sys.h");
                    if (c.RoBuGrover.ConflictGroepen?.Count > 0)
                    {
                        File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}rgv.c"), GenerateRgvC(c), Encoding.Default);
                        _allFiles.Add($"{c.Data.Naam}rgv.c");
                    }
                    if (c.PTPData.PTPKoppelingen?.Count > 0)
                    {
                        File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}ptp.c"), GeneratePtpC(c), Encoding.Default);
                        _allFiles.Add($"{c.Data.Naam}ptp.c");
                    }
                    if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.GeneriekePrioriteit &&
                        (c.PrioData.PrioIngrepen.Any() ||
                         c.PrioData.HDIngrepen.Any()))
                    {
                        File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}prio.c"), GeneratePrioC(c), Encoding.Default);
                        _allFiles.Add($"{c.Data.Naam}prio.c");
                    }
                    if (c.HalfstarData.IsHalfstar)
                    {
                        File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}hst.c"), GenerateHstC(c), Encoding.Default);
                        _allFiles.Add($"{c.Data.Naam}hst.c");
                    }

                    if (c.TimingsData.TimingsToepassen)
                    {
                        File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}fctimings.c"), GenerateFcTimingsC(c), Encoding.Default);
                        CopySourceIfNeeded(c, "timingsvar.c", sourcefilepath);
                        CopySourceIfNeeded(c, "timingsfunc.c", sourcefilepath);
                        if (c.TimingsData.TimingsUsePredictions)
                        {
                            CopySourceIfNeeded(c, "timings_uc4.c", sourcefilepath);
                        }
                    }
                    if (c.Data.GenererenEnkelCompilatieBestand)
                    {
                        File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}regeling.c"), GenerateRegelingC(c), Encoding.Default);
                        _allFiles.Add($"{c.Data.Naam}regeling.c");
                    }
                    if (c.Data.PracticeOmgeving)
                    {
                        File.WriteAllText(Path.Combine(sourcefilepath, "ccolreg.txt"), GeneratePraticeCcolReg(c), Encoding.Default);
                        File.WriteAllText(Path.Combine(sourcefilepath, "ccolreg2.txt"), GeneratePraticeCcolReg2(c), Encoding.Default);
                    }
                    if (c.RISData.RISToepassen)
                    {
                        File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}rissim.c"), GenerateRisSimC(c), Encoding.Default);
                    }

                    WriteAndReviseAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}reg.add"), c, GenerateRegAdd, GenerateRegAddHeader, Encoding.Default);
                    _allFiles.Add($"{c.Data.Naam}reg.add");
                    ReviseRegAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}reg.add"), c, Encoding.Default);
                    WriteAndReviseAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}tab.add"), c, GenerateTabAdd, GenerateTabAddHeader, Encoding.Default);
                    _allFiles.Add($"{c.Data.Naam}tab.add");
                    if (!c.Data.NietGebruikenBitmap)
                    {
                        WriteAndReviseAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}dpl.add"), c, GenerateDplAdd, GenerateDplAddHeader, Encoding.Default);
                    }
                    WriteAndReviseAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}sim.add"), c, GenerateSimAdd, GenerateSimAddHeader, Encoding.Default);
                    WriteAndReviseAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}sys.add"), c, GenerateSysAdd, GenerateSysAddHeader, Encoding.Default);
                    _allFiles.Add($"{c.Data.Naam}sys.add");
                    ReviseSysAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}sys.add"), c, Encoding.Default);
                    if (c.PrioData.PrioIngrepen.Count > 0 || c.PrioData.HDIngrepen.Count > 0)
                    {
                        if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.GeneriekePrioriteit)
                        {
                            WriteAndReviseAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}prio.add"), c, GeneratePrioAdd, GeneratePrioAddHeader, Encoding.Default);
                            _allFiles.Add($"{c.Data.Naam}prio.add");
                        }
                    }
                    if (c.HalfstarData.IsHalfstar)
                    {
                        WriteAndReviseAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}hst.add"), c, GenerateHstAdd, GenerateHstAddHeader, Encoding.Default);
                        _allFiles.Add($"{c.Data.Naam}hst.add");
                    }

                    CopySourceIfNeeded(c, "extra_func.c", sourcefilepath);
                    CopySourceIfNeeded(c, "extra_func.h", sourcefilepath);
                    CopySourceIfNeeded(c, "ccolfunc.c", sourcefilepath);
                    CopySourceIfNeeded(c, "ccolfunc.h", sourcefilepath);
                    CopySourceIfNeeded(c, "detectie.c", sourcefilepath);
                    CopySourceIfNeeded(c, "uitstuur.c", sourcefilepath);
                    CopySourceIfNeeded(c, "uitstuur.h", sourcefilepath);

                    if (c.Data.FixatieData.FixatieMogelijk)
                    {
                        CopySourceIfNeeded(c, "fixatie.c", sourcefilepath);
                        CopySourceIfNeeded(c, "fixatie.h", sourcefilepath);
                    }

                    if (c.PrioData.PrioIngrepen.Count > 0 || c.PrioData.HDIngrepen.Count > 0)
                    {
                        CopySourceIfNeeded(c, "extra_func_prio.c", sourcefilepath);
                        CopySourceIfNeeded(c, "extra_func_prio.h", sourcefilepath);
                    }

                    if (c.InterSignaalGroep.Nalopen.Any())
                    {
                        CopySourceIfNeeded(c, "gkvar.c", sourcefilepath);
                        CopySourceIfNeeded(c, "gkvar.h", sourcefilepath);
                        CopySourceIfNeeded(c, "nlvar.c", sourcefilepath);
                        CopySourceIfNeeded(c, "nlvar.h", sourcefilepath);
                        CopySourceIfNeeded(c, "nalopen.c", sourcefilepath);
                        CopySourceIfNeeded(c, "nalopen.h", sourcefilepath);
                    }

                    switch (c.Data.SynchronisatiesType)
                    {
                        case SynchronisatiesTypeEnum.SyncFunc when (c.InterSignaalGroep.Voorstarten.Any() || c.InterSignaalGroep.Gelijkstarten.Any()):
                            CopySourceIfNeeded(c, "syncfunc.c", sourcefilepath);
                            CopySourceIfNeeded(c, "syncvar.c", sourcefilepath);
                            CopySourceIfNeeded(c, "syncvar.h", sourcefilepath);
                            break;
                        case SynchronisatiesTypeEnum.RealFunc when (c.InterSignaalGroep.Voorstarten.Any() 
                                                                    || c.InterSignaalGroep.Gelijkstarten.Any()
                                                                    || c.InterSignaalGroep.Nalopen.Any(x => x.MaximaleVoorstart.HasValue)
                                                                    || c.InterSignaalGroep.LateReleases.Any()
                                                                    || c.Data.RealFuncBepaalRealisatieTijdenAltijd
                                                                    || c.TimingsData.TimingsUsePredictions):
                            CopySourceIfNeeded(c, "realfunc.c", sourcefilepath);
                            break;
                    }

                    if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.GeneriekePrioriteit &&
                        (c.PrioData.PrioIngrepen.Any() || c.PrioData.HDIngrepen.Any()))
                    {
                        CopySourceIfNeeded(c, "prio.c", sourcefilepath);
                        CopySourceIfNeeded(c, "prio.h", sourcefilepath);
                    }

                    if (c.RoBuGrover.ConflictGroepen.Any())
                    {
                        CopySourceIfNeeded(c, "rgv_overslag.c", sourcefilepath);
                        CopySourceIfNeeded(c, "rgvfunc.c", sourcefilepath);
                        CopySourceIfNeeded(c, "rgvvar.c", sourcefilepath);
                        CopySourceIfNeeded(c, "winmg.c", sourcefilepath);
                        CopySourceIfNeeded(c, "winmg.h", sourcefilepath);
                    }

                    if (c.HalfstarData.IsHalfstar)
                    {
                        CopySourceIfNeeded(c, "halfstar.c", sourcefilepath);
                        CopySourceIfNeeded(c, "halfstar.h", sourcefilepath);
                        CopySourceIfNeeded(c, "halfstar_prio.c", sourcefilepath);
                        CopySourceIfNeeded(c, "halfstar_prio.h", sourcefilepath);
                        if (c.Fasen.Any(x => x.WachttijdVoorspeller))
                        {
                            CopySourceIfNeeded(c, "halfstar_wtv.c", sourcefilepath);
                            CopySourceIfNeeded(c, "halfstar_wtv.h", sourcefilepath);
                        }
                    }

                    if (c.Fasen.Any(x => x.WachttijdVoorspeller))
                    {
                        CopySourceIfNeeded(c, "wtv_testwin.c", sourcefilepath);
                    }

                    if (c.RISData.RISToepassen)
                    {
                        CopySourceIfNeeded(c, "risappl.c", sourcefilepath);
                        CopySourceIfNeeded(c, "extra_func_ris.c", sourcefilepath);
                        CopySourceIfNeeded(c, "extra_func_ris.h", sourcefilepath);
                    }

                    if (c.StarData.ToepassenStar)
                    {
                        CopySourceIfNeeded(c, "starfunc.c", sourcefilepath);
                        CopySourceIfNeeded(c, "starfunc.h", sourcefilepath);
                        CopySourceIfNeeded(c, "starvar.c", sourcefilepath);
                        CopySourceIfNeeded(c, "starvar.h", sourcefilepath);
                    }

                    foreach (var pl in PieceGenerators)
                    {
                        var fs = pl.GetSourcesToCopy();
                        if (fs != null)
                        {
                            foreach (var f in fs)
                            {
                                CopySourceIfNeeded(c, f, sourcefilepath);
                            }
                        }
                    }

                    if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SourceFilesToCopy\\")))
                    {
                        try
                        {
                            foreach (var f in Directory.EnumerateFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SourceFilesToCopy\\")))
                            {
                                try
                                {
                                    var lines = File.ReadAllLines(f);
                                    if (lines != null && lines.Length > 0 && lines[0].StartsWith("CONDITION="))
                                    {
                                        var copy = false;
                                        var cond = lines[0].Replace("CONDITION=", "");
                                        copy = cond switch
                                        {
                                            "ALWAYS" => true,
                                            "OV" => (c.PrioData.PrioIngrepen.Count > 0 || c.PrioData.HDIngrepen.Count > 0),
                                            "SYNC" => (c.InterSignaalGroep.Gelijkstarten.Count > 0 || c.InterSignaalGroep.Voorstarten.Count > 0),
                                            "FIXATIE" => (c.Data.FixatieData.FixatieMogelijk),
                                            "NALOPEN" => (c.InterSignaalGroep.Nalopen.Count > 0),
                                            "RGV" => (c.RoBuGrover.SignaalGroepInstellingen.Count > 0),
                                            _ => copy
                                        };

                                        if (!copy || File.Exists(Path.Combine(sourcefilepath, Path.GetFileName(f)))) continue;

                                        var fileLines = new string[lines.Length - 1];
                                        Array.Copy(lines, 1, fileLines, 0, lines.Length - 1);
                                        File.WriteAllLines(Path.Combine(sourcefilepath, Path.GetFileName(f)), fileLines, Encoding.Default);
                                        _allFiles.Add(Path.GetFileName(f));
                                    }
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    if (c.Data.GenererenIncludesLijst)
                    {
                        GenerateIncludesList(c, Path.Combine(sourcefilepath, $"{c.Data.Naam}_sources_list.txt"));
                    }
                }
                catch (Exception e)
                {
                    TLCGenDialogProvider.Default.ShowMessageBox(
                        "Er is een fout opgetreden tijdens genereren. " +
                        $"Controlleer of alle te genereren bestanden overschreven kunnen worden.\n\nOorspronkelijke foutmelding:\n{e.Message}", "Fout tijdens genereren", MessageBoxButton.OK);
                }

                return "CCOL source code gegenereerd";
            }
            return $"Map {sourcefilepath} niet gevonden. Niets gegenereerd.";
        }

        private void GenerateIncludesList(ControllerModel c, string filename)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Overzicht TLCGen bestanden regeling: {c.Data.Naam}");
            sb.AppendLine("  > excl. dpl, sim");
            sb.AppendLine("  > let op: diverse files zijn reeds opgenomen middels #include");
            foreach (var file in _allFiles)
            {
                sb.AppendLine($"- {file}");
            }
            sb.AppendLine();
            sb.AppendLine($"Overzicht benodigde CCOL libraries regeling: {c.Data.Naam}");
            foreach (var lib in CCOLVisualProjectGenerator.GetNeededCCOLLibraries(c, true, 0))
            {
                sb.AppendLine($"- {lib}");
            }
            File.WriteAllText(filename, sb.ToString());
        }

        private void CopySourceIfNeeded(ControllerModel c, string filename, string sourcefilepath)
        {
            _allFiles.Add(filename);
            if ((!File.Exists(Path.Combine(sourcefilepath, filename)) || CCOLGeneratorSettingsProvider.Default.Settings.AlwaysOverwriteSources)
                && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SourceFiles\\" + filename)))
            {
                if (File.Exists(Path.Combine(sourcefilepath, filename)))
                {
                    try
                    {
                        File.Delete(Path.Combine(sourcefilepath, filename));
                    }
                    catch
                    {
                        TLCGenDialogProvider.Default.ShowMessageBox($"Bestand {filename} kan niet worden overschreven. Staat het nog ergens open?", "Fout bij overschrijven bestand", MessageBoxButton.OK);
                        return;
                    }
                }
                var text = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SourceFiles\\" + filename));
                text = $"/* {Path.GetFileName(filename)} - gegenereerd met TLCGen {c.Data.TLCGenVersie} */" + Environment.NewLine + Environment.NewLine + text;
                if(c.Data.CCOLVersie < CCOLVersieEnum.CCOL100)
                {
                    var rtext = Regex.Replace(text, @"(^|\s|\W)va_boolv", "$1va_bool");
                    rtext = Regex.Replace(rtext, @"(^|\W)boolv(\W)", "$1bool$2");
                    File.WriteAllText(Path.Combine(sourcefilepath, filename), rtext, Encoding.Default);
                }
                else // CCOL 10.0
                {
                    var rtext = Regex.Replace(text, @"(^|\s|\W)va_bool", "$1va_boolv");
                    rtext = Regex.Replace(rtext, @"(^|\W)bool(\W)", "$1boolv$2");
                    File.WriteAllText(Path.Combine(sourcefilepath, filename), rtext, Encoding.Default);
                }
            }
        }

        private void WriteAndReviseAdd(string filename, ControllerModel c, Func<ControllerModel, string> generateFunc,
            Func<ControllerModel, string> generateHeaderFunc, Encoding encoding)
        {
            if (!File.Exists(filename))
            {
                File.WriteAllText(filename, generateFunc(c), encoding);
            }
            else if (CCOLGeneratorSettingsProvider.Default.Settings.AlterAddHeadersWhileGenerating)
            {
                try
                {
                    var addlines = File.ReadAllLines(filename);
                    var sb = new StringBuilder();

                    var header = false;
                    foreach (var l in addlines)
                    {
                        if (l == _endGeneratedHeader)
                        {
                            header = false;
                        }
                        else if (l == _beginGeneratedHeader)
                        {
                            header = true;
                            sb.Append(generateHeaderFunc(c));
                        }
                        else if (!header)
                        {
                            sb.AppendLine(l);
                        }
                    }
                    File.Delete(filename);
                    File.WriteAllText(filename, sb.ToString(), encoding);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void ReviseRegAdd(string filename, ControllerModel c, Encoding encoding)
        {
            if(CCOLGeneratorSettingsProvider.Default.Settings.AlterAddFunctionsWhileGenerating)
            {
                try
                {
                    var addlines = File.ReadAllLines(filename);

                    var wtvAdd = c.Fasen.Any(x => x.WachttijdVoorspeller) && addlines.All(x => !x.Contains("WachttijdvoorspellersWachttijd_Add"));
                    // check of regel met spelfout erin zit
                    var wtvAddOld = c.Fasen.Any(x => x.WachttijdVoorspeller) && addlines.Any(x => x.Contains("WachtijdvoorspellersWachttijd_Add"));
                    var realAdd = c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc && addlines.All(x => !x.Contains("BepaalRealisatieTijden_Add"));
                    var postSys2 = c.Data.CCOLVersie >= CCOLVersieEnum.CCOL9 && addlines.All(x => !x.Contains("post_system_application2"));
                    var corrrealAdd = c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110 && c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc && addlines.All(x => !x.Contains("CorrectieRealisatieTijd_Add"));
                    var preMsgFcTimingAdd = c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110 && c.TimingsData.TimingsToepassen && addlines.All(x => !x.Contains("pre_msg_fctiming"));

                    var sb = new StringBuilder();

                    foreach (var l in addlines)
                    {
                        if (wtvAddOld && l.Contains("WachtijdvoorspellersWachttijd_Add"))
                        {
                            var l2 = l.Replace("WachtijdvoorspellersWachttijd_Add", "WachttijdvoorspellersWachttijd_Add");
                            sb.AppendLine(l2);
                            continue;
                        }
                        if (wtvAdd && !wtvAddOld && l.Contains("pre_system_application"))
                        {
                            sb.AppendLine("void WachttijdvoorspellersWachttijd_Add()");
                            sb.AppendLine("{");
                            sb.AppendLine($"{ts}");
                            sb.AppendLine("}");
                            sb.AppendLine();
                        }
                        if (corrrealAdd && l.Contains("Maxgroen_Add"))
                        {
                            sb.AppendLine($"{c.GetBoolV()} CorrectieRealisatieTijd_Add(void)");
                            sb.AppendLine("{");
                            sb.AppendLine($"{ts}/* let op! deze functie wordt in een loop aangeroepen (max. 100 iteraties). */");
                            sb.AppendLine($"{ts}{c.GetBoolV()} aanpassing = FALSE;");
                            sb.AppendLine($"{ts}");
                            sb.AppendLine($"{ts}/* Voeg hier zonodig eigen code toe, bijv:"); 
                            sb.AppendLine($"{ts} * aanpassing |= VTG2_Real_Los(fc32, fc31, T_max[tinl3231], T_max[tinl3132], hinl32, hinl31, hlos32, hlos31, (IH[hdrtk311] && IH[hdrtk321]));");
                            sb.AppendLine($"{ts} * aanpassing |= VTG2_Real_Los(fc31, fc32, T_max[tinl3132], T_max[tinl3231], hinl31, hinl32, hlos31, hlos32, (IH[hdrtk311] && IH[hdrtk321]));");
                            sb.AppendLine($"{ts} */");
                            sb.AppendLine($"{ts}");
                            sb.AppendLine($"{ts}return aanpassing;");
                            sb.AppendLine("}");
                            sb.AppendLine();
                        }
                        if (realAdd && l.Contains("Maxgroen_Add"))
                        {
                            sb.AppendLine("void BepaalRealisatieTijden_Add()");
                            sb.AppendLine("{");
                            sb.AppendLine();
                            sb.AppendLine("}");
                            sb.AppendLine();
                        }
                        if (preMsgFcTimingAdd && l.Contains("post_dump_application"))
                        {
                            sb.AppendLine("void pre_msg_fctiming()");
                            sb.AppendLine("{");
                            sb.AppendLine();
                            sb.AppendLine("}");
                            sb.AppendLine();
                        }
                        if (postSys2 && l.Contains("post_dump_application"))
                        {
                            sb.AppendLine("void post_system_application2()");
                            sb.AppendLine("{");
                            sb.AppendLine();
                            sb.AppendLine("}");
                            sb.AppendLine();
                        }
                        sb.AppendLine(l);
                    }
                    File.Delete(filename);
                    File.WriteAllText(filename, sb.ToString(), encoding);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void ReviseSysAdd(string filename, ControllerModel c, Encoding encoding)
        {
            if (CCOLGeneratorSettingsProvider.Default.Settings.AlterAddFunctionsWhileGenerating)
            {
                try
                {
                    var addlines = File.ReadAllLines(filename);

                    var addLnkMax = addlines.All(x => !x.Contains("LNKMAX"));
                    var addPlMax = c.HalfstarData.IsHalfstar && addlines.All(x => !x.Contains("PLMAX"));
                    
                    var sb = new StringBuilder();

                    foreach (var l in addlines)
                    {
                        sb.AppendLine(l);
                    }

                    if (CCOLGeneratorSettingsProvider.Default.Settings.AlterAddFunctionsWhileGenerating)
                    {
                        if (addLnkMax)
                        {
                            sb.AppendLine("#define LNKMAX (LNKMAX1+0) /* Totaal aantal gebruikte simulatie elementen */");
                        }
                        if (addPlMax)
                        {
                            sb.AppendLine("#define PLMAX (PLMAX1+0) /* Totaal aantal gebruikte signaalplannen */");
                        }
                    }

                    File.Delete(filename);
                    File.WriteAllText(filename, sb.ToString(), encoding);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public void LoadSettings()
        {
            foreach(var v in PieceGenerators)
            {
                if (v.HasSettings())
                {
                    var set = CCOLGeneratorSettingsProvider.Default.Settings.CodePieceGeneratorSettings.Find(x => x.Item1 == v.GetType().Name);
                    if (set != null)
                    {
                        if (!v.SetSettings(set.Item2))
                        {
                            MessageBox.Show($"Error with {v.GetType().Name}.\nCould not load settings; code generation will be faulty.", "Error loading CCOL code generator settings.");
                            return;
                        }
                    }
                    else
                    {
                        if (!v.SetSettings(null))
                        {
                            MessageBox.Show($"Error with {v.GetType().Name}.\nCould not load settings; code generation will be faulty.", "Error loading CCOL code generator settings.");
                            return;
                        }
                        v.SetSettings(null);
                    }
                }
            }

            // Check if any plugins provide code
            foreach (var pl in TLCGenPluginManager.Default.ApplicationPlugins)
            {
                var type = pl.Item2.GetType();
                var attr = (CCOLCodePieceGeneratorAttribute)Attribute.GetCustomAttribute(type, typeof(CCOLCodePieceGeneratorAttribute));

                if (attr == null) continue;

                var genPlugin = pl.Item2 as ICCOLCodePieceGenerator;

                if (genPlugin == null) continue;

                PieceGenerators.Add(genPlugin);
                var set = CCOLGeneratorSettingsProvider.Default.Settings.CodePieceGeneratorSettings.Find(x => x.Item1 == type.Name);
                if (genPlugin.HasSettings()) genPlugin.SetSettings(set?.Item2);

                var codetypes = Enum.GetValues(typeof(CCOLCodeTypeEnum));
                foreach (var codetype in codetypes)
                {
                    var index = genPlugin.HasCode((CCOLCodeTypeEnum)codetype);
                    if (index != null)
                    {
                        foreach (var i in index)
                        {
                            OrderedPieceGenerators[(CCOLCodeTypeEnum)codetype].Add(i, genPlugin);
                        }
                    }
                }
            }
        }

	    private void AddCodeTypeToStringBuilder(ControllerModel c, StringBuilder sb, CCOLCodeTypeEnum type, bool includevars, bool includecode, bool addnewlinebefore, bool addnewlineatend, List<CCOLLocalVariable> varsBefore = null)
	    {
			if (OrderedPieceGenerators[type].Any())
			{
                if ((includevars || includecode) && addnewlinebefore) sb.AppendLine();
                if (includevars)
                {
                    var vars = new List<CCOLLocalVariable>();
                    var added = false;
                    foreach (var gen in OrderedPieceGenerators[type])
                    {
                        var lv = gen.Value.GetFunctionLocalVariables(c, type).ToArray();
                        if (lv.Any())
                        {
                            foreach (var i in lv)
                            {
                                // check if variable already exists
                                var existing = varsBefore?.FirstOrDefault(x => x.Name == i.Name) ?? vars.FirstOrDefault(x => x.Name == i.Name);
                                if (existing != null)
                                {
                                    // set initial value if needed
                                    if (string.IsNullOrEmpty(existing.InitialValue) &&
                                        !string.IsNullOrEmpty(i.InitialValue))
                                    {
                                        existing.InitialValue = i.InitialValue;
                                    }

                                    // warn if initial value is already set
                                    if (!string.IsNullOrEmpty(existing.InitialValue) &&
                                        !string.IsNullOrEmpty(i.InitialValue) &&
                                        existing.InitialValue != i.InitialValue)
                                    {
                                        MessageBox.Show($"Initial value of variable {existing.Name} " +
                                                        $"in function type {type} is being set twice, " +
                                                        "with different values", "Warning: initial variable value inconsistency");
                                    }

                                    // reset define condition if not set here, cause it means
                                    // the variable is needed no matter what for this code gen
                                    if (!string.IsNullOrEmpty(existing.DefineCondition) &&
                                        string.IsNullOrEmpty(i.DefineCondition))
                                    {
                                        existing.DefineCondition = "";
                                    }
                                    // otherwise, if different, use both conditions
                                    else if (!string.IsNullOrEmpty(existing.DefineCondition) &&
                                             !string.IsNullOrEmpty(i.DefineCondition) &&
                                             existing.DefineCondition != i.DefineCondition)
                                    {
                                        existing.DefineCondition = existing.DefineCondition + " || " + i.DefineCondition;
                                    }
                                }
                                else
                                {
                                    varsBefore?.Add(i);
                                    vars.Add(i);
                                    added = true;
                                }
                            }
                        }
                    }

                    if (added)
                    {
                        foreach (var variable in vars)
                        {
                            if (!string.IsNullOrEmpty(variable.DefineCondition))
                            {
                                sb.AppendLine($"#if {variable.DefineCondition}");
                            }

                            sb.AppendLine(!string.IsNullOrWhiteSpace(variable.InitialValue) ? $"{ts}{variable.Type} {variable.Name} = {variable.InitialValue};" : $"{ts}{variable.Type} {variable.Name};");
                            if (!string.IsNullOrEmpty(variable.DefineCondition))
                            {
                                sb.AppendLine("#endif");
                            }
                        }
                    }

                    if (added && addnewlineatend) sb.AppendLine();
                }
                if (includecode)
                {
                    foreach (var gen in OrderedPieceGenerators[type].Where(x => x.Value.HasCodeForController(c, type)))
                    {
                        var code = gen.Value.GetCode(c, type, ts, gen.Key);
                        
                        if (string.IsNullOrWhiteSpace(code)) continue;

                        sb.Append(code);

                        if (addnewlineatend) sb.AppendLine();
                    }
                }
			}
		}

        #endregion // Public Methods

        #region Private Methods

        public List<IOElementModel> CollectAllIO(ControllerModel c, bool prepare = false)
        {
            if (prepare) PrepareGeneration(c);

            var rest = new List<IOElementModel>();
            foreach (var fc in c.Fasen)
            {
                fc.ElementType = IOElementTypeEnum.FaseCyclus;
                rest.Add(fc);
            }
            foreach (var d in c.GetAllRegularDetectors())
            {
                d.ElementType = IOElementTypeEnum.Detector;
                rest.Add(d);
            }
            foreach (var d in c.SelectieveDetectoren)
            {
                d.ElementType = IOElementTypeEnum.SelectiveDetector;
                rest.Add(d);
            }

            var ioResult = _uitgangen.Elements.Take(_uitgangen.Elements.Count - 1).OrderBy(x => x.IOElementData.RangeerIndex).Concat(_ingangen.Elements.Take(_ingangen.Elements.Count - 1).OrderBy(x => x.IOElementData.RangeerIndex)).ToList();

            var totalResult = rest.Concat(ioResult.Select(x => x.IOElementData)).ToList();

            return totalResult;
        }

        /// <summary>
        /// Generates all "sys.h" lines for a given instance of CCOLElemListData.
        /// The function loops all elements in the Elements member.
        /// </summary>
        /// <param name="data">The instance of CCOLElemListData to use for generation</param>
        /// <param name="numberdefine">Optional: this string will be used as follows:
        /// - if it is null, lines are generated like this: #define ElemName #
        /// - if it is not null, it goes like this: #define ElemName (numberdefine + #)</param>
        /// <returns></returns>
        private string GetAllElementsSysHLines(CCOLElemListData data, string numberdefine = null, List<CCOLElement> extraElements = null, bool useRangering = false)
        {
            var sb = new StringBuilder();

            var pad1 = data.DefineMaxWidth + $"{ts}#define  ".Length;
            var pad2 = data.Elements.Count.ToString().Length;
            var pad3 = data.CommentsMaxWidth;
            var index = 0;

            var elements = (useRangering ? data.Elements.OrderBy(x => x.RangeerIndex).ToList() : data.Elements).ToList();

            foreach (var elem in elements)
            {
                if (elem.Dummy || Regex.IsMatch(elem.Define, @"[A-Z]+MAX"))
                    continue;
                
                sb.Append($"{ts}#define {elem.Define} ".PadRight(pad1));
                if (string.IsNullOrWhiteSpace(numberdefine))
                {
                    sb.Append($"{index}".PadLeft(pad2));
                }
                else
                {
                    sb.Append($"({numberdefine} + ");
                    sb.Append($"{index}".PadLeft(pad2));
                    sb.Append(")");
                }
				if (!string.IsNullOrWhiteSpace(elem.Commentaar))
				{
                    sb.Append(" /* ");
                    sb.Append($"{elem.Commentaar}".PadRight(pad3));
                    sb.Append(" */");
                }
				sb.AppendLine();
                ++index;
            }
            var indexautom = index;

            if (data.Elements.Count > 0 && data.Elements.Any(x => x.Dummy))
            {
                sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST");
                foreach (var elem in elements)
                {
                    if (!elem.Dummy || Regex.IsMatch(elem.Define, @"[A-Z]+MAX"))
                        continue;

                    sb.Append($"{ts}#define {elem.Define} ".PadRight(pad1));
                    if (string.IsNullOrWhiteSpace(numberdefine))
                    {
                        sb.Append($"{indexautom}".PadLeft(pad2));
                    }
                    else
                    {
                        sb.Append($"({numberdefine} + ");
                        sb.Append($"{indexautom}".PadLeft(pad2));
                        sb.Append(")");
                    }
					if (!string.IsNullOrWhiteSpace(elem.Commentaar))
					{
                        sb.Append(" /* ");
                        sb.Append($"{elem.Commentaar}".PadRight(pad3));
                        sb.Append(" */");
                    }
					sb.AppendLine();
                    ++indexautom;
                }
                sb.Append($"{ts}#define {data.Elements.Last().Define} ".PadRight(pad1));
                if (string.IsNullOrWhiteSpace(numberdefine))
                {
                    sb.AppendLine($"{indexautom}".PadLeft(pad2));
                }
                else
                {
                    sb.Append($"({numberdefine} + ");
                    sb.Append($"{indexautom}".PadLeft(pad2));
                    sb.AppendLine(")");
                }
                sb.AppendLine("#else");
                sb.Append($"{ts}#define {data.Elements.Last().Define} ".PadRight(pad1));
                if (string.IsNullOrWhiteSpace(numberdefine))
                {
                    sb.AppendLine($"{index}".PadLeft(pad2));
                }
                else
                {
                    sb.Append($"({numberdefine} + ");
                    sb.Append($"{index}".PadLeft(pad2));
                    sb.AppendLine(")");
                }
                sb.AppendLine("#endif");
            }
            else if(data.Elements.Count > 0)
            {
                sb.Append($"{ts}#define {data.Elements.Last().Define} ".PadRight(pad1));
                if (string.IsNullOrWhiteSpace(numberdefine))
                {
                    sb.AppendLine($"{index}".PadLeft(pad2));
                }
                else
                {
                    sb.Append($"({numberdefine} + ");
                    sb.Append($"{index}".PadLeft(pad2));
                    sb.AppendLine(")");
                }
            }

            return sb.ToString();
        }

        private string GetAllElementsTabCLines(ControllerModel c, CCOLElemListData data)
        {
            if (data.Elements.Count == 0) return "";

            var sb = new StringBuilder();

            var pad1 = ts.Length + data.CCOLCodeWidth + 2 + data.DefineMaxWidth; // 3: [ ]
            var pad2 = data.NameMaxWidth + 6;  // 6: space = space " " ;
            var pad3 = data.CCOLSettingWidth + 3 + data.DefineMaxWidth; // 3: space [ ]
            var pad4 = data.SettingMaxWidth + 4;  // 4: space = space ;
            var pad5 = data.CCOLTTypeWidth + 3 + data.DefineMaxWidth; // 3: space [ ]
            var pad6 = data.CommentsMaxWidth;
            var pad7 = data.TTypeMaxWidth + 4; // 4: ' = ;'

            var loopElements =
                c.Data.RangeerData.RangerenIngangen && data.Elements[0].Type == CCOLElementTypeEnum.Ingang ||
                c.Data.RangeerData.RangerenUitgangen && data.Elements[0].Type == CCOLElementTypeEnum.Uitgang
                ? data.Elements
                    .Where(x => !string.IsNullOrWhiteSpace(x.Naam))
                    .OrderBy(x => x.IOElementData.RangeerIndex)
                : data.Elements
                    .Where(x => !string.IsNullOrWhiteSpace(x.Naam));

            foreach (var elem in loopElements)
            {
                if (elem.Dummy)
                    continue;

                sb.Append($"{ts}{data.CCOLCode}[{elem.Define}]".PadRight(pad1));
                var name = c.Data.CCOLCodeCase switch
                {
                    CCOLCodeCaseEnum.LowerCase => elem.Naam.ToLower(),
                    CCOLCodeCaseEnum.UpperCase => elem.Naam.ToUpper(),
                    _ => elem.Naam
                };
                if (!string.IsNullOrWhiteSpace(elem.IOElementData?.ManualNaam))
                {
                    name = c.Data.CCOLCodeCase switch
                    {
                        CCOLCodeCaseEnum.LowerCase => elem.IOElementData.ManualNaam.ToLower(),
                        CCOLCodeCaseEnum.UpperCase => elem.IOElementData.ManualNaam.ToUpper(),
                        _ => elem.IOElementData.ManualNaam
                    };
                }
                sb.Append($" = \"{name}\";".PadRight(pad2));
                if (!string.IsNullOrEmpty(data.CCOLSetting) && elem.Instelling.HasValue)
                {
                    sb.Append($" {data.CCOLSetting}[{elem.Define}]".PadRight(pad3));
                    sb.Append($" = {elem.Instelling};".PadRight(pad4));
                }
                if (!string.IsNullOrEmpty(data.CCOLTType) && elem.TType != CCOLElementTimeTypeEnum.None)
                {
                    sb.Append($" {data.CCOLTType}[{elem.Define}]".PadRight(pad5));
                    sb.Append($" = {elem.TType};");
                }
                else
                {
                    sb.Append("".PadRight(pad5 + pad7));
                }
                if (!string.IsNullOrWhiteSpace(elem.Commentaar))
                {
                    sb.Append(" /* ");
                    sb.Append($"{elem.Commentaar}".PadRight(pad6));
                    sb.Append(" */");
                }
                sb.AppendLine();
            }

            if (data.Elements.Count > 0 && data.Elements.Any(x => x.Dummy))
            {
                sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM || defined PRACTICE_TEST");
                foreach (var delem in data.Elements)
                {
                    if (!delem.Dummy) continue;
                    
                    sb.Append($"{ts}{data.CCOLCode}[{delem.Define}]".PadRight(pad1));
                    var name = c.Data.CCOLCodeCase switch
                    {
                        CCOLCodeCaseEnum.LowerCase => delem.Naam.ToLower(),
                        CCOLCodeCaseEnum.UpperCase => delem.Naam.ToUpper(),
                        _ => delem.Naam
                    };
                    if (!string.IsNullOrWhiteSpace(delem.IOElementData?.ManualNaam))
                    {
                        name = c.Data.CCOLCodeCase switch
                        {
                            CCOLCodeCaseEnum.LowerCase => delem.IOElementData.ManualNaam.ToLower(),
                            CCOLCodeCaseEnum.UpperCase => delem.IOElementData.ManualNaam.ToUpper(),
                            _ => delem.IOElementData.ManualNaam
                        };  
                    }
                    sb.Append($" = \"{name}\";".PadRight(pad2));
                    if (!string.IsNullOrEmpty(data.CCOLSetting) && delem.Instelling.HasValue)
                    {
                        sb.Append($" {data.CCOLSetting}[{delem.Define}]".PadRight(pad3));
                        sb.Append($" = {delem.Instelling};".PadRight(pad4));
                    }
                    if (!string.IsNullOrEmpty(data.CCOLTType) && delem.TType != CCOLElementTimeTypeEnum.None)
                    {
                        sb.Append($" {data.CCOLTType}[{delem.Define}]".PadRight(pad5));
                        sb.Append($" = {delem.TType};");
                    }
                    else
                    {
                        sb.Append("".PadRight(pad5));
                    }
                    if (!string.IsNullOrWhiteSpace(delem.Commentaar))
                    {
                        sb.Append(" /* ");
                        sb.Append($"{delem.Commentaar}".PadRight(pad6));
                        sb.Append(" */");
                    }
                    sb.AppendLine();
                }
                sb.AppendLine("#endif");
            }

            return sb.ToString();
        }

        #endregion // Private Methods

        #region Constructor

        public CCOLGenerator()
        {
            if (PieceGenerators != null)
                throw new NullReferenceException();

            PieceGenerators = new List<ICCOLCodePieceGenerator>();
            OrderedPieceGenerators = new Dictionary<CCOLCodeTypeEnum, SortedDictionary<int, ICCOLCodePieceGenerator>>();
            var codetypes = Enum.GetValues(typeof(CCOLCodeTypeEnum));
            foreach (var type in codetypes)
            {
                OrderedPieceGenerators.Add((CCOLCodeTypeEnum)type, new SortedDictionary<int, ICCOLCodePieceGenerator>());
            }

            var ccolgen = typeof(CCOLGenerator).Assembly;
            foreach (var type in ccolgen.GetTypes())
            {
                // Find CCOLCodePieceGenerator attribute, and if found, continue
                var attr = (CCOLCodePieceGeneratorAttribute)Attribute.GetCustomAttribute(type, typeof(CCOLCodePieceGeneratorAttribute));
                if (attr != null)
                {
                    var v = Activator.CreateInstance(type) as ICCOLCodePieceGenerator;
                    PieceGenerators.Add(v);
                    foreach (var codetype in codetypes)
                    {
                        var index = v.HasCode((CCOLCodeTypeEnum) codetype);
                        if (index != null)
                        {
                            try
                            {
                                foreach (var i in index)
                                    OrderedPieceGenerators[(CCOLCodeTypeEnum) codetype].Add(i, v);
                            }
                            catch
                            {
                                MessageBox.Show($"Error while loading code generator {v.GetType().FullName}.\n" +
                                                $"Is the index number for {codetype} unique?",
                                    "Error loading code generator");
                            }
                        }
                    }
                }
            }
        }

        #endregion // Constructor
    }
}
