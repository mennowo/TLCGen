using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    [CCOLCodePieceGenerator]
    public class RichtingGevoeligCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _trgr;
        private CCOLGeneratorCodeStringSettingModel _trga;
        private CCOLGeneratorCodeStringSettingModel _trgav;
        private CCOLGeneratorCodeStringSettingModel _trgv;
        private CCOLGeneratorCodeStringSettingModel _hrgv;
        private CCOLGeneratorCodeStringSettingModel _prmmkrg;
        private CCOLGeneratorCodeStringSettingModel _schrgad;
        private CCOLGeneratorCodeStringSettingModel _schrga;
        private CCOLGeneratorCodeStringSettingModel _schrgvl;
#pragma warning restore 0649
        private string _tkm; // read from settings provider, comes from other code gen object

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var rga in c.RichtingGevoeligeAanvragen)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_trga}{_dpf}{rga.VanDetector}", rga.MaxTijdsVerschil, CCOLElementTimeTypeEnum.TE_type, _trga, rga.FaseCyclus, rga.VanDetector, rga.NaarDetector));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_schrgad}{_dpf}{rga.VanDetector}", 1, CCOLElementTimeTypeEnum.SCH_type, _schrgad, rga.FaseCyclus, rga.VanDetector, rga.NaarDetector));
                if (rga.ResetAanvraag)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_trgav}{_dpf}{rga.VanDetector}", rga.ResetAanvraagTijdsduur, CCOLElementTimeTypeEnum.TE_type, _trgav, rga.FaseCyclus, rga.VanDetector, rga.NaarDetector));
                }
                if (rga.AltijdAanUit != Models.Enumerations.AltijdAanUitEnum.Altijd)
                {
                    _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_schrga}{_dpf}{rga.VanDetector}", rga.AltijdAanUit == Models.Enumerations.AltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schrga, rga.FaseCyclus, rga.VanDetector, rga.NaarDetector));
                }
            }

            foreach (var rgv in c.RichtingGevoeligVerlengen)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_trgr}{_dpf}{rgv.VanDetector}_{_dpf}{rgv.NaarDetector}", rgv.MaxTijdsVerschil, CCOLElementTimeTypeEnum.TE_type, _trgr, rgv.FaseCyclus, rgv.VanDetector, rgv.NaarDetector));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_trgv}{_dpf}{rgv.VanDetector}_{_dpf}{rgv.NaarDetector}", rgv.VerlengTijd, CCOLElementTimeTypeEnum.TE_type, _trgv, rgv.FaseCyclus, rgv.VanDetector, rgv.NaarDetector));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_hrgv}{_dpf}{rgv.VanDetector}_{_dpf}{rgv.NaarDetector}", _hrgv, rgv.FaseCyclus, rgv.VanDetector));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmmkrg}{_dpf}{rgv.VanDetector}", (int)rgv.TypeVerlengen, CCOLElementTimeTypeEnum.None, _prmmkrg, rgv.FaseCyclus));

                if (rgv.AltijdAanUit != Models.Enumerations.AltijdAanUitEnum.Altijd)
                {
                    _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                    $"{_schrgvl}{_dpf}{rgv.VanDetector}", rgv.AltijdAanUit == Models.Enumerations.AltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schrgvl, rgv.FaseCyclus, rgv.VanDetector, rgv.NaarDetector));
                }
            }
        }
        public override bool HasCCOLElements() => true;
        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    if (c?.RichtingGevoeligeAanvragen.Any(x => x.ResetAanvraag) == true)
                        return new List<CCOLLocalVariable> { new("int", "fc") };
                    return base.GetFunctionLocalVariables(c, type);
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCMeetkriterium => new []{20},
                CCOLCodeTypeEnum.RegCAanvragen => new []{40},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    if (!c.RichtingGevoeligeAanvragen.Any()) return "";

                    sb.AppendLine($"{ts}/* Richtinggevoelige aanvragen */");
                    sb.AppendLine($"{ts}/* --------------------------- */");

                    if (c.RichtingGevoeligeAanvragen.Any(x => x.ResetAanvraag))
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc) RR[fc] &= ~BIT9;  /* reset BIT-sturing t.b.v. reset A */");
                        sb.AppendLine();
                    }

                    foreach (var rga in c.RichtingGevoeligeAanvragen)
                    {
                        sb.Append(ts);
                        if (rga.AltijdAanUit != Models.Enumerations.AltijdAanUitEnum.Altijd)
                        {
                            sb.Append($"if (SCH[{_schpf}{_schrga}{_dpf}{rga.VanDetector}]) ");
                        }
                        if (!rga.ResetAanvraag)
                        {
                            sb.AppendLine($"aanvraag_richtinggevoeligV1({_fcpf}{rga.FaseCyclus}, {_dpf}{rga.NaarDetector}, {_dpf}{rga.VanDetector}, {_tpf}{_trga}{_dpf}{rga.VanDetector}, SCH[{_schpf}{_schrgad}{_dpf}{rga.VanDetector}]);");
                        }
                        else
                        {
                            sb.AppendLine($"aanvraag_richtinggevoelig_reset({_fcpf}{rga.FaseCyclus}, {_dpf}{rga.NaarDetector}, {_dpf}{rga.VanDetector}, {_tpf}{_trga}{_dpf}{rga.VanDetector}, {_tpf}{_trgav}{_dpf}{rga.VanDetector}, SCH[{_schpf}{_schrgad}{_dpf}{rga.VanDetector}]);");
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    if (!c.RichtingGevoeligVerlengen.Any()) return "";

                    sb.AppendLine($"{ts}/* Richtinggevoelig verlengen */");
                    sb.AppendLine($"{ts}/* -------------------------- */");
                    var rgvGroups = c.RichtingGevoeligVerlengen.GroupBy(x => x.FaseCyclus);
                    foreach(var gr in rgvGroups)
                    {
                        var first = gr.First();
                        sb.AppendLine($"{ts}MeetKriteriumRGprm((count) {_fcpf}{first.FaseCyclus}, (count) {_tpf}{_tkm}{first.FaseCyclus},");
                        foreach (var rgv in gr)
                        {
                            if (first.AltijdAanUit != Models.Enumerations.AltijdAanUitEnum.Altijd)
                            {
                                sb.AppendLine($"{ts}{ts}SCH[{_schpf}{_schrgvl}{_dpf}{rgv.VanDetector}] &&");
                            }
                            sb.AppendLine($"{ts}{ts}({c.GetBoolV()}) RichtingVerlengen({_fcpf}{rgv.FaseCyclus}, {_dpf}{rgv.VanDetector}, {_dpf}{rgv.NaarDetector},");
                            sb.AppendLine($"{ts}{ts}                         {_tpf}{_trgr}{_dpf}{rgv.VanDetector}_{_dpf}{rgv.NaarDetector}, {_tpf}{_trgv}{_dpf}{rgv.VanDetector}_{_dpf}{rgv.NaarDetector},");
                            sb.AppendLine($"{ts}{ts}                         {_hpf}{_hrgv}{_dpf}{rgv.VanDetector}_{_dpf}{rgv.NaarDetector}), (mulv)PRM[{_prmpf}{_prmmkrg}{_dpf}{rgv.VanDetector}],");
                        }
                        sb.AppendLine($"{ts}{ts}                         (count)END);");
                    }
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _tkm = CCOLGeneratorSettingsProvider.Default.GetElementName("tkm");

            return base.SetSettings(settings);
        }
    }
}
