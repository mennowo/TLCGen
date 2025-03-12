using System.Collections.Generic;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class FixatieCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _isfix;
        private CCOLGeneratorCodeStringSettingModel _schbmfix;
        private CCOLGeneratorCodeStringSettingModel _hfixatietegenh;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if (c.Data.FixatieData.FixatieMogelijk)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_schbmfix}",
                        c.Data.FixatieData.BijkomenTijdensFixatie ? 1 : 0,
                        CCOLElementTimeTypeEnum.SCH_type,
                        _schbmfix));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_isfix}",
                        _isfix, c.Data.FixatieData.FixatieBitmapData));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_hfixatietegenh}",
                        0,
                        CCOLElementTimeTypeEnum.None,
                        _hfixatietegenh));
            }
        }
        
        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.PrioCAfkappen:
                    if (!c.Data.FixatieData.FixatieMogelijk)
                        return base.GetFunctionLocalVariables(c, type);
                    return new List<CCOLLocalVariable>
                    {
                        new("int", "fc"),
                    };
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }
        
        public override bool HasCCOLElements() => true;

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPreApplication: return new []{25};
                case CCOLCodeTypeEnum.PrioCAfkappen: return new[] { 50 };
            }
            return null;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPreApplication:
                    if (!c.Data.FixatieData.FixatieMogelijk) return null;
                    sb.AppendLine($"{ts}/* Soms is het wenselijk om een fixatie ingreep te kunnen uitstellen, */");
                    sb.AppendLine($"{ts}/* bijvoorbeeld bij een AFT of WTV die laag staat; in dat geval kan   */");
                    sb.AppendLine($"{ts}/* hulpelement IH[{_hpf}{_hfixatietegenh}] hoog gemaakt worden.                */");
                    sb.AppendLine($"{ts}IH[{_hpf}{_hfixatietegenh}] = FALSE;");
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCAfkappen:
                    if (!c.Data.FixatieData.FixatieMogelijk) return null;
                    sb.AppendLine($"{ts}/* Niet afkappen tijdens fixeren */");
                    sb.AppendLine($"{ts}if (IS[{_ispf}{_isfix}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts} Z[fc] &= ~PRIO_Z_BIT;");
                    sb.AppendLine($"{ts}{ts}{ts}FM[fc] &= ~PRIO_FM_BIT;");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();
            }

            return null;
        }
    }
}
