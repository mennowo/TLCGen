using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using TLCGen.Dependencies.Providers;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
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

        private string ts => CCOLGeneratorSettingsProvider.Default.Settings.TabSpace ?? "";

        private string _beginGeneratedHeader = "/* BEGIN GEGENEREERDE HEADER */";
        private string _endGeneratedHeader = "/* EINDE GEGENEREERDE HEADER */";

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

        public static List<Tuple<ICCOLCodePieceGenerator, List<CCOLElement>>> GetAllGeneratorsWithElements(ControllerModel c)
        {
            var l = new List<Tuple<ICCOLCodePieceGenerator, List<CCOLElement>>>();
            foreach (var pgen in PieceGenerators)
            {
                pgen.CollectCCOLElements(c);
                var elems = pgen.GetCCOLElements();
                if(elems != null)
                {
                    l.Add(new Tuple<ICCOLCodePieceGenerator, List<CCOLElement>>(pgen, elems.ToList()));
                }
            }
            return l;
        }

        public static CCOLElemListData[] GetAllCCOLElements(ControllerModel c)
        {
            foreach (var pgen in PieceGenerators)
            {
                pgen.CollectCCOLElements(c);
            }
            return CCOLElementCollector.CollectAllCCOLElements(c, PieceGenerators);
        }

        public string GenerateSourceFiles(ControllerModel c, string sourcefilepath)
        {
            if (Directory.Exists(sourcefilepath))
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
                _cpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("c");
                _prmpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("prm");

                foreach (var pgen in PieceGenerators)
                {
                    pgen.CollectCCOLElements(c);
                }

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

                var CCOLElementLists = CCOLElementCollector.CollectAllCCOLElements(c, PieceGenerators);

                if (CCOLElementLists == null || CCOLElementLists.Length != 8)
                    throw new IndexOutOfRangeException("Error collecting CCOL elements from controller.");

                foreach (var pl in TLCGenPluginManager.Default.ApplicationPlugins)
                {
                    if ((pl.Item1 & TLCGenPluginElems.IOElementProvider) != TLCGenPluginElems.IOElementProvider) continue;

                    var elemprov = pl.Item2 as ITLCGenElementProvider;
                    var elems = elemprov?.GetAllItems();
                    if (elems == null) continue;
                    foreach (var elem in elems)
                    {
                        var ccolElement = elem as CCOLElement;
                        if (ccolElement == null) continue;
                        switch (ccolElement.Type)
                        {
                            case CCOLElementTypeEnum.Uitgang: CCOLElementLists[0].Elements.Add(ccolElement); break;
                            case CCOLElementTypeEnum.Ingang: CCOLElementLists[1].Elements.Add(ccolElement); break;
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

                foreach (var l in CCOLElementLists)
                {
                    l.SetMax();
                }

                CCOLElementCollector.AddAllMaxElements(CCOLElementLists);

                File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}reg.c"), GenerateRegC(c), Encoding.Default);
                if (!c.Data.NietGebruikenBitmap)
                {
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}dpl.c"), GenerateDplC(c), Encoding.Default);
                }
                File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}tab.c"), GenerateTabC(c), Encoding.Default);
                File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}sim.c"), GenerateSimC(c), Encoding.Default);
                File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}sys.h"), GenerateSysH(c), Encoding.Default);
                if(c.RoBuGrover.ConflictGroepen?.Count > 0)
                {
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}rgv.c"), GenerateRgvC(c), Encoding.Default);
                }
                if(c.PTPData.PTPKoppelingen?.Count > 0)
                {
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}ptp.c"), GeneratePtpC(c), Encoding.Default);
                }
                if (c.OVData.OVIngrepen.Any() ||
                    c.OVData.HDIngrepen.Any())
                {
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}ov.c"), GenerateOvC(c), Encoding.Default);
                }
	            if (c.HalfstarData.IsHalfstar)
	            {
		            File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}hst.c"), GenerateHstC(c), Encoding.Default);
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
                WriteAndReviseAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}tab.add"), c, GenerateTabAdd, GenerateTabAddHeader, Encoding.Default);
                if (!c.Data.NietGebruikenBitmap)
                {
                    WriteAndReviseAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}dpl.add"), c, GenerateDplAdd, GenerateDplAddHeader, Encoding.Default);
                }
                WriteAndReviseAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}sim.add"), c, GenerateSimAdd, GenerateSimAddHeader, Encoding.Default);
                WriteAndReviseAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}sys.add"), c, GenerateSysAdd, GenerateSysAddHeader, Encoding.Default);
                if (c.OVData.OVIngrepen.Count > 0 || c.OVData.HDIngrepen.Count > 0)
                {
                    WriteAndReviseAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}ov.add"), c, GenerateOvAdd, GenerateOvAddHeader, Encoding.Default);
                }
                if (c.HalfstarData.IsHalfstar)
                {
                    WriteAndReviseAdd(Path.Combine(sourcefilepath, $"{c.Data.Naam}hst.add"), c, GenerateHstAdd, GenerateHstAddHeader, Encoding.Default);
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

                if (c.OVData.OVIngrepen.Count > 0 || c.OVData.HDIngrepen.Count > 0)
                {
                    CopySourceIfNeeded(c, "extra_func_ov.c", sourcefilepath);
                    CopySourceIfNeeded(c, "extra_func_ov.h", sourcefilepath);
                }

                if(c.InterSignaalGroep.Nalopen.Any())
                {
                    CopySourceIfNeeded(c, "gkvar.c", sourcefilepath);
                    CopySourceIfNeeded(c, "gkvar.h", sourcefilepath);
                    CopySourceIfNeeded(c, "nlvar.c", sourcefilepath);
                    CopySourceIfNeeded(c, "nlvar.h", sourcefilepath);
                    CopySourceIfNeeded(c, "nalopen.c", sourcefilepath);
                    CopySourceIfNeeded(c, "nalopen.h", sourcefilepath);
                }

                if (c.InterSignaalGroep.Voorstarten.Any() || c.InterSignaalGroep.Gelijkstarten.Any())
                {
                    CopySourceIfNeeded(c, "syncfunc.c", sourcefilepath);
                    CopySourceIfNeeded(c, "syncvar.c", sourcefilepath);
                    CopySourceIfNeeded(c, "syncvar.h", sourcefilepath);
                }

                if (c.OVData.OVIngrepen.Any() || c.OVData.HDIngrepen.Any())
                {
                    CopySourceIfNeeded(c, "ov.c", sourcefilepath);
                    CopySourceIfNeeded(c, "ov.h", sourcefilepath);
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
		            CopySourceIfNeeded(c, "halfstar_ov.c", sourcefilepath);
					CopySourceIfNeeded(c, "halfstar_ov.h", sourcefilepath);
		            CopySourceIfNeeded(c, "halfstar_help.c", sourcefilepath);
					CopySourceIfNeeded(c, "halfstar_help.h", sourcefilepath);
	            }

                if (c.Fasen.Any(x => x.WachttijdVoorspeller))
                {
                    CopySourceIfNeeded(c, "wtv_testwin.c", sourcefilepath);
                }

                if (c.RISData.RISToepassen)
                {
                    CopySourceIfNeeded(c, "risappl.c", sourcefilepath);
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
                        foreach(var f in Directory.EnumerateFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SourceFilesToCopy\\")))
                        {
                            try
                            {
                                var lines = File.ReadAllLines(f);
                                if (lines != null && lines.Length > 0 && lines[0].StartsWith("CONDITION="))
                                {
                                    var copy = false;
                                    var cond = lines[0].Replace("CONDITION=", "");
                                    switch (cond)
                                    {
                                        case "ALWAYS":
                                            copy = true;
                                            break;
                                        case "OV":
                                            copy = (c.OVData.OVIngrepen.Count > 0 || c.OVData.HDIngrepen.Count > 0);
                                            break;
                                        case "SYNC":
                                            copy = (c.InterSignaalGroep.Gelijkstarten.Count > 0 ||
                                                    c.InterSignaalGroep.Voorstarten.Count > 0);
                                            break;
                                        case "FIXATIE":
                                            copy = (c.Data.FixatieData.FixatieMogelijk);
                                            break;
                                        case "NALOPEN":
                                            copy = (c.InterSignaalGroep.Nalopen.Count > 0);
                                            break;
                                        case "RGV":
                                            copy = (c.RoBuGrover.SignaalGroepInstellingen.Count > 0);
                                            break;
                                    }

                                    if (!copy || File.Exists(Path.Combine(sourcefilepath, Path.GetFileName(f)))) continue;

                                    var fileLines = new string[lines.Length - 1];
                                    Array.Copy(lines, 1, fileLines, 0, lines.Length - 1);
                                    File.WriteAllLines(Path.Combine(sourcefilepath, Path.GetFileName(f)), fileLines, Encoding.Default);
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

                return "CCOL source code gegenereerd";
            }
            return $"Map {sourcefilepath} niet gevonden. Niets gegenereerd.";
        }

        private void CopySourceIfNeeded(ControllerModel c, string filename, string sourcefilepath)
        {
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
                if(c.Data.CCOLVersie < Models.Enumerations.CCOLVersieEnum.CCOL100)
                {
                    File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SourceFiles\\" + filename), Path.Combine(sourcefilepath, filename));
                }
                else // CCOL 10.0
                {
                    var text = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SourceFiles\\" + filename));
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
            else if(CCOLGeneratorSettingsProvider.Default.Settings.AlterAddHeadersWhileGenerating)
            {
                try
                {
                    var addlines = File.ReadAllLines(filename);

                    var wtvAdd = c.Fasen.Any(x => x.WachttijdVoorspeller) && addlines.All(x => !x.Contains("WachtijdvoorspellersWachttijd_Add"));
                    var postSys2 = c.Data.CCOLVersie >= Models.Enumerations.CCOLVersieEnum.CCOL9 && addlines.All(x => !x.Contains("post_system_application2"));

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
                        else if(!header)
                        {
                            if (CCOLGeneratorSettingsProvider.Default.Settings.AlterAddFunctionsWhileGenerating && 
                                wtvAdd && l.Contains("pre_system_application"))
                            {
                                sb.AppendLine("void WachtijdvoorspellersWachttijd_Add()");
                                sb.AppendLine("{");
                                sb.AppendLine($"{ts}");
                                sb.AppendLine("}");
                                sb.AppendLine();
                            }
                            if (CCOLGeneratorSettingsProvider.Default.Settings.AlterAddFunctionsWhileGenerating &&
                                postSys2 && l.Contains("post_dump_application"))
                            {
                                sb.AppendLine("void WachtijdvoorspellersWachttijd_Add()");
                                sb.AppendLine("{");
                                sb.AppendLine("");
                                sb.AppendLine("}");
                                sb.AppendLine();
                            }
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
                    if (index > 0)
                    {
                        OrderedPieceGenerators[(CCOLCodeTypeEnum)codetype].Add(index, genPlugin);
                    }
                }
            }
        }

	    private void AddCodeTypeToStringBuilder(ControllerModel c, StringBuilder sb, CCOLCodeTypeEnum type, bool includevars, bool includecode, bool addnewlinebefore, bool addnewlineatend, List<string> varsBefore = null)
	    {
			if (OrderedPieceGenerators[type].Any())
			{
                if ((includevars || includecode) && addnewlinebefore) sb.AppendLine();
                if (includevars)
                {
                    var vars = new List<string>();
                    var added = false;
                    if (varsBefore != null) vars = varsBefore;
                    foreach (var gen in OrderedPieceGenerators[type])
                    {
                        var lv = gen.Value.GetFunctionLocalVariables(c, type);
                        if (lv.Any())
                        {
                            foreach (var i in lv)
                            {
                                if (!vars.Any(x => x == i.Item2))
                                {
                                    vars.Add(i.Item2);
                                    added = true;
                                    if (!string.IsNullOrWhiteSpace(i.Item3))
                                    {
                                        sb.AppendLine($"{ts}{i.Item1} {i.Item2} = {i.Item3};");
                                    }
                                    else
                                    {
                                        sb.AppendLine($"{ts}{i.Item1} {i.Item2};");
                                    }
                                }
                                else
                                {
                                    // ignore: local variable already exists
                                    //MessageBox.Show($"Function local variable with name {i.Item2} (now from {gen.Value.GetType().Name}) already exists!", "Error while generating function local variables");
                                }
                            }
                        }
                    }
                    if (added && addnewlineatend) sb.AppendLine();
                }
                if (includecode)
                {
                    foreach (var gen in OrderedPieceGenerators[type].Where(x => x.Value.HasCodeForController(c, type)))
                    {
                        var code = gen.Value.GetCode(c, type, ts);
                        if (!string.IsNullOrWhiteSpace(code))
                        {
                            sb.Append(code);
                            if (addnewlineatend) sb.AppendLine();
                        }
                    }
                }
			}
		}

        #endregion // Public Methods

        #region Private Methods

        /// <summary>
        /// Generates all "sys.h" lines for a given instance of CCOLElemListData.
        /// The function loops all elements in the Elements member.
        /// </summary>
        /// <param name="data">The instance of CCOLElemListData to use for generation</param>
        /// <param name="numberdefine">Optional: this string will be used as follows:
        /// - if it is null, lines are generated like this: #define ElemName #
        /// - if it is not null, it goes like this: #define ElemName (numberdefine + #)</param>
        /// <returns></returns>
        private string GetAllElementsSysHLines(CCOLElemListData data, string numberdefine = null, List<CCOLElement> extraElements = null)
        {
            var sb = new StringBuilder();

            var pad1 = data.DefineMaxWidth + $"{ts}#define  ".Length;
            var pad2 = data.Elements.Count.ToString().Length;
            int pad3 = data.CommentsMaxWidth;
            var index = 0;

            foreach (var elem in data.Elements)
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
                    sb.Append($" /* ");
                    sb.Append($"{elem.Commentaar}".PadRight(pad3));
                    sb.Append($" */");
                }
				sb.AppendLine();
                ++index;
            }
            var indexautom = index;

            if (data.Elements.Count > 0 && data.Elements.Any(x => x.Dummy))
            {
                sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM");
                foreach (var elem in data.Elements)
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
                        sb.Append($" /* ");
                        sb.Append($"{elem.Commentaar}".PadRight(pad3));
                        sb.Append($" */");
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

        private string GetAllElementsTabCLines(CCOLElemListData data)
        {
            StringBuilder sb = new StringBuilder();

            int pad1 = ts.Length + data.CCOLCodeWidth + 2 + data.DefineMaxWidth; // 3: [ ]
            int pad2 = data.NameMaxWidth + 6;  // 6: space = space " " ;
            int pad3 = data.CCOLSettingWidth + 3 + data.DefineMaxWidth; // 3: space [ ]
            int pad4 = data.SettingMaxWidth + 4;  // 4: space = space ;
            int pad5 = data.CCOLTTypeWidth + 3 + data.DefineMaxWidth; // 3: space [ ]
            int pad6 = data.CommentsMaxWidth;
            int pad7 = data.TTypeMaxWidth + 4; // 4: ' = ;'

            foreach (CCOLElement elem in data.Elements)
            {
                if (elem.Dummy)
                    continue;

                if (!string.IsNullOrWhiteSpace(elem.Naam))
                {
                    sb.Append($"{ts}{data.CCOLCode}[{elem.Define}]".PadRight(pad1));
                    sb.Append($" = \"{elem.Naam}\";".PadRight(pad2));
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
                        sb.Append($" /* ");
                        sb.Append($"{ elem.Commentaar}".PadRight(pad6));
                        sb.Append($" */");
					}
                    sb.AppendLine();
                }
            }

            if (data.Elements.Count > 0 && data.Elements.Any(x => x.Dummy))
            {
                sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)");
                foreach (CCOLElement delem in data.Elements)
                {
                    if (!delem.Dummy)
                        continue;
                    sb.Append($"{ts}{data.CCOLCode}[{delem.Define}]".PadRight(pad1));
                    sb.Append($" = \"{delem.Naam}\";".PadRight(pad2));
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
                        sb.Append($" /*");
                        sb.Append($"{ delem.Commentaar}".PadRight(pad6));
                        sb.Append($" */");
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

            Assembly ccolgen = typeof(CCOLGenerator).Assembly;
            foreach (Type type in ccolgen.GetTypes())
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
                        if (index > 0)
                        {
                            try
                            {
                                OrderedPieceGenerators[(CCOLCodeTypeEnum) codetype].Add(index, v);
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
