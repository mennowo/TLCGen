using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TLCGen.Generators.TLCCC.CodeGeneration.HelperClasses;
using TLCGen.Generators.TLCCC.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.TLCCC.CodeGeneration
{
    public partial class TLCCCGenerator
    {
        #region Fields

        private string _sgpf = TLCCCGeneratorSettingsProvider.Default.GetPrefix(TLCCCElementTypeEnum.SignalGroup);
        private string _dpf = TLCCCGeneratorSettingsProvider.Default.GetPrefix(TLCCCElementTypeEnum.Detector);
        private string _ospf = TLCCCGeneratorSettingsProvider.Default.GetPrefix(TLCCCElementTypeEnum.Output);
        private string _ispf = TLCCCGeneratorSettingsProvider.Default.GetPrefix(TLCCCElementTypeEnum.Input);
        private string _schpf = TLCCCGeneratorSettingsProvider.Default.GetPrefix(TLCCCElementTypeEnum.Switch);
        private string _tpf = TLCCCGeneratorSettingsProvider.Default.GetPrefix(TLCCCElementTypeEnum.Timer);
        private string _prmpf = TLCCCGeneratorSettingsProvider.Default.GetPrefix(TLCCCElementTypeEnum.Parameter);
        private string ts = TLCCCGeneratorSettingsProvider.Default.Settings.TabSpace;

        private TLCCCElemListData _outputs;
        private TLCCCElemListData _inputs;
        private TLCCCElemListData _timers;
        private TLCCCElemListData _switches;
        private TLCCCElemListData _parameters;

        #endregion // Fields

        #region Public Methods

        public void GenerateSourceFiles(ControllerModel c, string sourcefilepath)
        {
            if (Directory.Exists(sourcefilepath))
            {
                var elementLists = TLCCCElementCollector.CollectAllCCOLElements(c);
                TLCCCElementCollector.AddAllMaxElements(elementLists);

                _outputs = elementLists[0];
                _inputs = elementLists[1];
                _timers= elementLists[2];
                _switches = elementLists[3];
                _parameters = elementLists[4];

                File.WriteAllText(Path.Combine(sourcefilepath, $"{c.Data.Naam}sys.h"), GenerateSysH(c));
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
        private string GetAllElementsSysHLines(TLCCCElemListData data, string numberdefine = null)
        {
            var sb = new StringBuilder();

            var pad1 = data.DefineMaxWidth + $"#define  ".Length;
            var pad2 = data.Elements.Count.ToString().Length;
            var index = 0;

            foreach (var elem in data.Elements)
            {
                if (elem.Dummy || Regex.IsMatch(elem.Define, @"[A-Z]+MAX"))
                    continue;

                sb.Append($"#define {elem.Define} ".PadRight(pad1));
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
                ++index;
            }
            var indexautom = index;

            if (data.Elements.Count > 0 && data.Elements.Any(x => x.Dummy))
            {
                sb.AppendLine("#ifndef AUTOMAAT");
                foreach (var elem in data.Elements)
                {
                    if (!elem.Dummy || Regex.IsMatch(elem.Define, @"[A-Z]+MAX"))
                        continue;

                    sb.Append($"#define {elem.Define} ".PadRight(pad1));
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
                    ++indexautom;
                }
                sb.Append($"#define {data.Elements.Last().Define} ".PadRight(pad1));
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
                sb.Append($"#define {data.Elements.Last().Define} ".PadRight(pad1));
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
            else if (data.Elements.Count > 0)
            {
                sb.Append($"#define {data.Elements.Last().Define} ".PadRight(pad1));
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

        #endregion // Private Methods
    }
}