using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

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
        
        private List<ICCOLCodePieceGenerator> _PieceGenerators;

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

        public string GenerateSourceFiles(ControllerModel controller, string sourcefilepath)
        {
            if (Directory.Exists(sourcefilepath))
            {
                foreach (var pgen in _PieceGenerators)
                {
                    pgen.CollectCCOLElements(controller);
                }

                AlleDetectoren = new List<DetectorModel>();
                foreach (FaseCyclusModel fcm in controller.Fasen)
                {
                    foreach (DetectorModel dm in fcm.Detectoren)
                        AlleDetectoren.Add(dm);
                }
                foreach (DetectorModel dm in controller.Detectoren)
                    AlleDetectoren.Add(dm);

                var CCOLElementLists = CCOLElementCollector.CollectAllCCOLElements(controller, _PieceGenerators);
                if (CCOLElementLists == null || CCOLElementLists.Length != 8)
                    throw new NotImplementedException("Error collection CCOL elements from controller.");

                foreach(var l in CCOLElementLists)
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

                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}reg.c"), GenerateRegC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}tab.c"), GenerateTabC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}dpl.c"), GenerateDplC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sim.c"), GenerateSimC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sys.h"), GenerateSysH(controller));
                if(controller.RoBuGrover.ConflictGroepen?.Count > 0)
                {
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}rgv.c"), GenerateRgvC(controller));
                }
                if(controller.PTPData.PTPKoppelingen?.Count > 0)
                {
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}ptp.c"), GeneratePtpC(controller));
                }

                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}reg.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}reg.add"), GenerateRegAdd(controller));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}tab.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}tab.add"), GenerateTabAdd(controller));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}dpl.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}dpl.add"), GenerateDplAdd(controller));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sim.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sim.add"), GenerateSimAdd(controller));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sys.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sys.add"), GenerateSysAdd(controller));
                return "CCOL source code gegenereerd";
            }
            return $"Map {sourcefilepath} niet gevonden. Niets gegenereerd.";
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
            foreach (CCOLElement elem in data.Elements)
            {
                sb.Append($"{ts}#define {elem.Define} ".PadRight(pad1));
                if (string.IsNullOrWhiteSpace(numberdefine))
                {
                    sb.AppendLine($"{index.ToString()}".PadLeft(pad2));
                }
                else
                {
                    sb.Append($"({numberdefine} + ");
                    sb.Append($"{index.ToString()}".PadLeft(pad2));
                    sb.AppendLine($")");
                }
                ++index;
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

            foreach (CCOLElement ce in data.Elements)
            {
                if (!string.IsNullOrWhiteSpace(ce.Naam))
                {
                    sb.Append($"{ts}{data.CCOLCode}[{ce.Define}]".PadRight(pad1));
                    sb.Append($" = \"{ce.Naam}\";".PadRight(pad2));
                    if (!string.IsNullOrEmpty(data.CCOLSetting) && ce.Instelling.HasValue)
                    {
                        sb.Append($" {data.CCOLSetting}[{ce.Define}]".PadRight(pad3));
                        sb.Append($" = {ce.Instelling.ToString()};".PadRight(pad4));
                    }
                    if (!string.IsNullOrEmpty(data.CCOLTType) && ce.TType != CCOLElementTimeTypeEnum.None)
                    {
                        sb.Append($" {data.CCOLTType}[{ce.Define}]".PadRight(pad5));
                        sb.Append($" = {ce.TType};");
                    }
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        #endregion // Private Methods

        #region Constructor

        public CCOLGenerator()
        {
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
