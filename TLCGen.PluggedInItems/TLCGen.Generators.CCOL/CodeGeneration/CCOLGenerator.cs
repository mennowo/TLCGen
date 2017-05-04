using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        #region Fields

        private CCOLElemListData Uitgangen;
        private CCOLElemListData Ingangen;
        private CCOLElemListData HulpElementen;
        private CCOLElemListData GeheugenElementen;
        private CCOLElemListData Timers;
        private CCOLElemListData Counters;
        private CCOLElemListData Schakelaars;
        private CCOLElemListData Parameters;

        private List<DetectorModel> AlleDetectoren;

        private string _uspf;
        private string _ispf;
        private string _fcpf;
        private string _dpf;
        private string _tpf;
        private string _cpf;
        private string _schpf;
        private string _prmpf;
        private string _hpf;

        private bool _AnyOV;
        private bool _AnyHD;
        private bool _AnyOVorHD;

        public static List<ICCOLCodePieceGenerator> PieceGenerators
        {
            get { return _PieceGenerators; }
        }
        private static List<ICCOLCodePieceGenerator> _PieceGenerators;

        public string ts
        {
            get
            {
                if(CCOLGeneratorSettingsProvider.Default.Settings.TabSpace == null)
                {
                    return "";
                }
                return CCOLGeneratorSettingsProvider.Default.Settings.TabSpace;
            }
        }

        #endregion // Fields

        #region Properties

        #endregion // Properties

        #region Commands
        #endregion // Commands

        #region Command Functionality
        #endregion // Command Functionality

        #region Public Methods

        public string GenerateSourceFiles(ControllerModel c, string sourcefilepath)
        {
            if (Directory.Exists(sourcefilepath))
            {
                _uspf = CCOLGeneratorSettingsProvider.Default.GetPrefix("us");
                _ispf = CCOLGeneratorSettingsProvider.Default.GetPrefix("is");
                _fcpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("fc");
                _dpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("d");
                _tpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("t");
                _schpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("sch");
                _hpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("h");
                _cpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("c");
                _prmpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("prm");

                _AnyOV = c.OVData.OVIngrepen.Count > 0 && c.OVData.OVIngrepen.Where(x => x.KAR).Any();
                _AnyHD = c.OVData.HDIngrepen.Count > 0 && c.OVData.HDIngrepen.Where(x => x.KAR).Any();
                _AnyOVorHD = c.OVData.OVIngrepen.Count > 0 && c.OVData.OVIngrepen.Where(x => x.KAR).Any() ||
                             c.OVData.HDIngrepen.Count > 0 && c.OVData.HDIngrepen.Where(x => x.KAR).Any();

                foreach (var pgen in _PieceGenerators)
                {
                    pgen.CollectCCOLElements(c);
                }

                AlleDetectoren = new List<DetectorModel>();
                foreach (FaseCyclusModel fcm in c.Fasen)
                {
                    foreach (DetectorModel dm in fcm.Detectoren)
                        AlleDetectoren.Add(dm);
                }
                foreach (DetectorModel dm in c.Detectoren)
                    AlleDetectoren.Add(dm);

                var CCOLElementLists = CCOLElementCollector.CollectAllCCOLElements(c, _PieceGenerators);

                if (CCOLElementLists == null || CCOLElementLists.Length != 8)
                    throw new NotImplementedException("Error collection CCOL elements from controller.");

                foreach (var pl in TLCGen.Plugins.TLCGenPluginManager.Default.ApplicationPlugins)
                {
                    if ((pl.Item1 & TLCGenPluginElems.IOElementProvider) == TLCGenPluginElems.IOElementProvider)
                    {
                        var elemprov = pl.Item2 as ITLCGenElementProvider;
                        if (elemprov != null)
                        {
                            var elems = elemprov.GetAllItems() as List<object>;
                            if (elems != null)
                            {
                                foreach (var elem in elems)
                                {
                                    var _elem = elem as CCOLElement;
                                    if (_elem != null)
                                    {
                                        switch (_elem.Type)
                                        {
                                            case CCOLElementTypeEnum.Uitgang: CCOLElementLists[0].Elements.Add(_elem); break;
                                            case CCOLElementTypeEnum.Ingang: CCOLElementLists[1].Elements.Add(_elem); break;
                                            case CCOLElementTypeEnum.HulpElement: CCOLElementLists[2].Elements.Add(_elem); break;
                                            case CCOLElementTypeEnum.GeheugenElement: CCOLElementLists[3].Elements.Add(_elem); break;
                                            case CCOLElementTypeEnum.Timer: CCOLElementLists[4].Elements.Add(_elem); break;
                                            case CCOLElementTypeEnum.Counter: CCOLElementLists[5].Elements.Add(_elem); break;
                                            case CCOLElementTypeEnum.Schakelaar: CCOLElementLists[6].Elements.Add(_elem); break;
                                            case CCOLElementTypeEnum.Parameter: CCOLElementLists[7].Elements.Add(_elem); break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                CCOLElementCollector.AddAllMaxElements(CCOLElementLists);

                foreach (var l in CCOLElementLists)
                {
                    l.SetMax();
                }

                Uitgangen = CCOLElementLists[0];
                Ingangen = CCOLElementLists[1];
                HulpElementen = CCOLElementLists[2];
                GeheugenElementen = CCOLElementLists[3];
                Timers = CCOLElementLists[4];
                Counters = CCOLElementLists[5];
                Schakelaars = CCOLElementLists[6];
                Parameters = CCOLElementLists[7];

                File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}reg.c"), GenerateRegC(c));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}tab.c"), GenerateTabC(c));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}dpl.c"), GenerateDplC(c));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}sim.c"), GenerateSimC(c));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}sys.h"), GenerateSysH(c));
                if(c.RoBuGrover.ConflictGroepen?.Count > 0)
                {
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}rgv.c"), GenerateRgvC(c));
                }
                if(c.PTPData.PTPKoppelingen?.Count > 0)
                {
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}ptp.c"), GeneratePtpC(c));
                }
                if (c.OVData.OVIngrepen.Count > 0 ||
                    c.OVData.HDIngrepen.Count > 0)
                {
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}ov.c"), GenerateOvC(c));
                }

                if (!File.Exists(Path.Combine(sourcefilepath, $"{c.Data.Naam}reg.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}reg.add"), GenerateRegAdd(c));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{c.Data.Naam}tab.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}tab.add"), GenerateTabAdd(c));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{c.Data.Naam}dpl.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}dpl.add"), GenerateDplAdd(c));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{c.Data.Naam}sim.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}sim.add"), GenerateSimAdd(c));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{c.Data.Naam}sys.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}sys.add"), GenerateSysAdd(c));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{c.Data.Naam}ov.add")) &&
                    (c.OVData.OVIngrepen.Count > 0 || c.OVData.HDIngrepen.Count > 0))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}ov.add"), GenerateOvAdd(c));

                CopySourceIfNeeded("extra_func.c", sourcefilepath);
                CopySourceIfNeeded("extra_func.h", sourcefilepath);

                if(c.OVData.OVIngrepen.Count > 0 || c.OVData.HDIngrepen.Count > 0)
                {
                    CopySourceIfNeeded("extra_func_ov.c", sourcefilepath);
                    CopySourceIfNeeded("extra_func_ov.h", sourcefilepath);
                }

                if(c.InterSignaalGroep.Nalopen.Any())
                {
                    CopySourceIfNeeded("nalopen.c", sourcefilepath);
                    CopySourceIfNeeded("nalopen.h", sourcefilepath);
                }

                if(Directory.Exists(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SourceFilesToCopy\\")))
                {
                    try
                    {
                        foreach(var f in Directory.EnumerateFiles(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SourceFilesToCopy\\")))
                        {
                            try
                            {
                                var lines = File.ReadAllLines(f);
                                if(lines != null && lines.Length > 0)
                                {
                                    if(lines[0].StartsWith("CONDITION="))
                                    {
                                        bool copy = false;
                                        string cond = lines[0].Replace("CONDITION=", "");
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
                                        if(copy)
                                        {
                                            if (!File.Exists(Path.Combine(sourcefilepath, Path.GetFileName(f))))
                                            {
                                                string[] _lines = new string[lines.Length - 1];
                                                Array.Copy(lines, 1, _lines, 0, lines.Length - 1);
                                                File.WriteAllLines(Path.Combine(sourcefilepath, Path.GetFileName(f)), _lines);
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    catch
                    {

                    }
                }

                return "CCOL source code gegenereerd";
            }
            return $"Map {sourcefilepath} niet gevonden. Niets gegenereerd.";
        }

        private void CopySourceIfNeeded(string filename, string sourcefilepath)
        {
            if (!File.Exists(Path.Combine(sourcefilepath, filename)) && File.Exists(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SourceFiles\\" + filename)))
            {
                File.Copy(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SourceFiles\\" + filename), Path.Combine(sourcefilepath, filename));
            }
        }

        public void LoadSettings()
        {
            foreach(var v in _PieceGenerators)
            {
                if (v.HasSettings())
                {
                    var set = CCOLGeneratorSettingsProvider.Default.Settings.CodePieceGeneratorSettings.Find(x => x.Item1 == v.GetType().Name);
                    if (set != null)
                    {
                        if (!v.SetSettings(set.Item2))
                        {
                            System.Windows.MessageBox.Show($"Error with {v.GetType().Name}.\nCould not load settings; code generation will be faulty.", "Error loading CCOL code generator settings.");
                            return;
                        }
                    }
                    else
                    {
                        if (!v.SetSettings(null))
                        {
                            System.Windows.MessageBox.Show($"Error with {v.GetType().Name}.\nCould not load settings; code generation will be faulty.", "Error loading CCOL code generator settings.");
                            return;
                        }
                        v.SetSettings(null);
                    }
                }
            }

            // Check if any plugins provide code
            foreach (var pl in TLCGenPluginManager.Default.ApplicationPlugins)
            {
                Type type = pl.Item2.GetType();
                var attr = (CCOLCodePieceGeneratorAttribute)Attribute.GetCustomAttribute(type, typeof(CCOLCodePieceGeneratorAttribute));
                if (attr != null)
                {
                    var _pl = pl.Item2 as ICCOLCodePieceGenerator;
                    _PieceGenerators.Add(_pl);
                    if(_pl.HasSettings())
                        _pl.SetSettings(null);
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
        private string GetAllElementsSysHLines(CCOLElemListData data, string numberdefine = null)
        {
            StringBuilder sb = new StringBuilder();

            int pad1 = data.DefineMaxWidth + $"{ts}#define  ".Length;
            int pad2 = data.Elements.Count.ToString().Length;

            int index = 0;
            int indexautom = 0;

            foreach (CCOLElement elem in data.Elements)
            {
                if (elem.Dummy || (Regex.IsMatch(elem.Define, @"[A-Z]+MAX")))
                    continue;
                
                sb.Append($"{ts}#define {elem.Define} ".PadRight(pad1));
                if (string.IsNullOrWhiteSpace(numberdefine))
                {
                    sb.AppendLine($"{index.ToString()}".PadLeft(pad2));
                }
                else
                {
                    sb.Append($"({numberdefine} + ");
                    sb.Append($"{index.ToString()}".PadLeft(pad2));
                    sb.AppendLine(")");
                }
                ++index;
            }
            indexautom = index;

            if (data.Elements.Count > 0 && data.Elements.Where(x => x.Dummy).Any())
            {
                sb.AppendLine("#ifndef AUTOMAAT");
                foreach (CCOLElement elem in data.Elements)
                {
                    if (!elem.Dummy || (Regex.IsMatch(elem.Define, @"[A-Z]+MAX")))
                        continue;

                    sb.Append($"{ts}#define {elem.Define} ".PadRight(pad1));
                    if (string.IsNullOrWhiteSpace(numberdefine))
                    {
                        sb.AppendLine($"{indexautom.ToString()}".PadLeft(pad2));
                    }
                    else
                    {
                        sb.Append($"({numberdefine} + ");
                        sb.Append($"{indexautom.ToString()}".PadLeft(pad2));
                        sb.AppendLine($")");
                    }
                    ++indexautom;
                }
                sb.Append($"{ts}#define {data.Elements.Last().Define} ".PadRight(pad1));
                if (string.IsNullOrWhiteSpace(numberdefine))
                {
                    sb.AppendLine($"{indexautom.ToString()}".PadLeft(pad2));
                }
                else
                {
                    sb.Append($"({numberdefine} + ");
                    sb.Append($"{indexautom.ToString()}".PadLeft(pad2));
                    sb.AppendLine(")");
                }
                sb.AppendLine("#else");
                sb.Append($"{ts}#define {data.Elements.Last().Define} ".PadRight(pad1));
                if (string.IsNullOrWhiteSpace(numberdefine))
                {
                    sb.AppendLine($"{index.ToString()}".PadLeft(pad2));
                }
                else
                {
                    sb.Append($"({numberdefine} + ");
                    sb.Append($"{index.ToString()}".PadLeft(pad2));
                    sb.AppendLine(")");
                }
                sb.AppendLine("#endif");
            }
            else if(data.Elements.Count > 0)
            {
                sb.Append($"{ts}#define {data.Elements.Last().Define} ".PadRight(pad1));
                if (string.IsNullOrWhiteSpace(numberdefine))
                {
                    sb.AppendLine($"{index.ToString()}".PadLeft(pad2));
                }
                else
                {
                    sb.Append($"({numberdefine} + ");
                    sb.Append($"{index.ToString()}".PadLeft(pad2));
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
                        sb.Append($" = {elem.Instelling.ToString()};".PadRight(pad4));
                    }
                    if (!string.IsNullOrEmpty(data.CCOLTType) && elem.TType != CCOLElementTimeTypeEnum.None)
                    {
                        sb.Append($" {data.CCOLTType}[{elem.Define}]".PadRight(pad5));
                        sb.Append($" = {elem.TType};");
                    }
                    sb.AppendLine();
                }
            }

            if (data.Elements.Count > 0 && data.Elements.Where(x => x.Dummy).Any())
            {
                sb.AppendLine("#ifndef AUTOMAAT");
                foreach (CCOLElement delem in data.Elements)
                {
                    if (!delem.Dummy)
                        continue;
                    sb.Append($"{ts}{data.CCOLCode}[{delem.Define}]".PadRight(pad1));
                    sb.Append($" = \"{delem.Naam}\";".PadRight(pad2));
                    if (!string.IsNullOrEmpty(data.CCOLSetting) && delem.Instelling.HasValue)
                    {
                        sb.Append($" {data.CCOLSetting}[{delem.Define}]".PadRight(pad3));
                        sb.Append($" = {delem.Instelling.ToString()};".PadRight(pad4));
                    }
                    if (!string.IsNullOrEmpty(data.CCOLTType) && delem.TType != CCOLElementTimeTypeEnum.None)
                    {
                        sb.Append($" {data.CCOLTType}[{delem.Define}]".PadRight(pad5));
                        sb.Append($" = {delem.TType};");
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
            if (_PieceGenerators != null)
                throw new NotImplementedException();

            _PieceGenerators = new List<ICCOLCodePieceGenerator>();

            Assembly ccolgen = typeof(CCOLGenerator).Assembly;
            foreach (Type type in ccolgen.GetTypes())
            {
                // Find CCOLCodePieceGenerator attribute, and if found, continue
                var attr = (CCOLCodePieceGeneratorAttribute)Attribute.GetCustomAttribute(type, typeof(CCOLCodePieceGeneratorAttribute));
                if (attr != null)
                {
                    var v = Activator.CreateInstance(type) as ICCOLCodePieceGenerator;
                    _PieceGenerators.Add(v);
                }
            }
        }

        #endregion // Constructor
    }
}
