using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class IsgFuncCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

#pragma warning disable 0649
        //private CCOLGeneratorCodeStringSettingModel _mtest;
#pragma warning restore 0649
        private string _prmaltg;
        private string _hfile;
        private string _prmfperc;

        #endregion // Fields

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
        }

        public override bool HasCCOLElements() => true;

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    return new List<CCOLLocalVariable>
                    {
                        new("int", "fc"),
                    };
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCIncludes => new[] { 140 },
                CCOLCodeTypeEnum.RegCTop=> new[] { 140 },
                CCOLCodeTypeEnum.RegCVerlenggroen => new[] { 90 },
                CCOLCodeTypeEnum.RegCMaxgroen => new[] { 90 },
                CCOLCodeTypeEnum.RegCInitApplication => new[] { 140 },
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:
                    sb.AppendLine("#ifndef NO_ISG");
                    sb.AppendLine($"{ts}#include \"isgfunc.c\" /* Interstartgroenfuncties */");
                    sb.AppendLine("#endif /* NO_ISG */");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCTop:
                    sb.AppendLine("#ifndef NO_ISG");
                    sb.AppendLine($"{ts}{c.GetBoolV()} init_tvg;");
                    sb.AppendLine("#endif /* NO_ISG */");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    sb.AppendLine($"#ifndef NO_ISG");
                    sb.AppendLine($"{ts}if (EVG[fc] && PR[fc] || init_tvg)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}TVG_PR[fc] = TVG_max[fc];");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}else");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}TVG_max[fc] = TVG_PR[fc];");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}init_tvg = TRUE;");
                    sb.AppendLine($"{ts}/* Bepaal de minimale maximale verlengroentijd bij alternatieve realisaties */");
                    foreach (var fc in c.Fasen)
                    {
                        sb.AppendLine($"{ts}TVG_AR[{_fcpf}{fc.Naam}] = ((PRM[{_prmpf}{_prmaltg}{fc.Naam}] - TFG_max[{_fcpf}{fc.Naam}]) >= 0) ? PRM[{_prmpf}{_prmaltg}{fc.Naam}] - TFG_max[{_fcpf}{fc.Naam}] : NG;");
                    }
                    sb.AppendLine($"#endif /* NO_ISG */");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCInitApplication:
                    sb.AppendLine($"#ifndef NO_ISG");
                    sb.AppendLine($"{ts}init_tvg = FALSE;");
                    sb.AppendLine($"#endif /* NO_ISG */");
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _prmaltg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltg");
            _hfile = CCOLGeneratorSettingsProvider.Default.GetElementName("hfile");
            _prmfperc = CCOLGeneratorSettingsProvider.Default.GetElementName("prmfperc");

            return base.SetSettings(settings);
        }
    }

}
