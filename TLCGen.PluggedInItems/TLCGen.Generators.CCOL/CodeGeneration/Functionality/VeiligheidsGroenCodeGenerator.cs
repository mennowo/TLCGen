using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class VeiligheidsGroenCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schvg;
        private CCOLGeneratorCodeStringSettingModel _tvgmax; 
        private CCOLGeneratorCodeStringSettingModel _tvgvolg; 
        private CCOLGeneratorCodeStringSettingModel _tvghiaat; 
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var fcm in c.Fasen.Where(x => x.Detectoren.Any(x2 => x2.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit)))
            {
                _myElements.Add(
                     CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tvgmax}{fcm.Naam}", fcm.VeiligheidsGroenMaximaal, CCOLElementTimeTypeEnum.TE_type, _tvgmax, fcm.Naam));
                foreach (var dm in fcm.Detectoren.Where(x => x.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit))
                {
                    if (dm.VeiligheidsGroen != NooitAltijdAanUitEnum.Altijd)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_schvg}{dm.Naam}", dm.VeiligheidsGroen == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schvg, dm.Naam, fcm.Naam));
                    }
                    _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tvgvolg}{dm.Naam}", dm.VeiligheidsGroenVolgtijd, CCOLElementTimeTypeEnum.TE_type, _tvgvolg, dm.Naam, fcm.Naam));
                    _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tvghiaat}{dm.Naam}", dm.VeiligheidsGroenHiaat, CCOLElementTimeTypeEnum.TE_type, _tvghiaat, dm.Naam, fcm.Naam));
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCMeeverlengen => new []{20},
                _ => null
            };
        }

        public override bool HasCodeForController(ControllerModel c, CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCMeeverlengen => c.Fasen.SelectMany(x => x.Detectoren).Any(x => x.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit),
                _ => false
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCMeeverlengen:
                    sb.AppendLine($"{ts}/* Veiligheidsgroen */");
                    sb.AppendLine($"{ts}/* ---------------- */");
                    foreach (var fcm in c.Fasen.Where(x => x.Detectoren.Any(x2 => x2.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit)))
                    {
                        sb.Append($"{ts}veiligheidsgroen_V1({_fcpf}{fcm.Naam}, {_tpf}{_tvgmax}{fcm.Naam}, ");
                        foreach (var dm in fcm.Detectoren.Where(x => x.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit))
                        {
                            if (dm.VeiligheidsGroen == NooitAltijdAanUitEnum.Altijd)
                            {
                                sb.Append($"{_dpf}{dm.Naam}, {_tpf}{_tvgvolg}{dm.Naam}, NG, {_tpf}{_tvghiaat}{dm.Naam}, ");
                            }
                            else
                            {
                                sb.Append($"{_dpf}{dm.Naam}, {_tpf}{_tvgvolg}{dm.Naam}, {_schpf}{_schvg}{dm.Naam}, {_tpf}{_tvghiaat}{dm.Naam}, ");
                            }
                        }
                        sb.AppendLine("END);");
                    }
                    return sb.ToString();
                default:
                    return null;
            }
        }
    }
}
