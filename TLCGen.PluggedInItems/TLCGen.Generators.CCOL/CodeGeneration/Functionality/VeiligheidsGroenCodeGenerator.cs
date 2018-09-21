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
        private CCOLGeneratorCodeStringSettingModel _tvga; 
        private CCOLGeneratorCodeStringSettingModel _tvgb; 
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var fcm in c.Fasen)
            {
                if(fcm.Detectoren.Any(x => x.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit))
                {
                    foreach (var dm in fcm.Detectoren.Where(x => x.VeiligheidsGroen != NooitAltijdAanUitEnum.Altijd))
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_schvg}{dm.Naam}", dm.VeiligheidsGroen == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schvg, dm.Naam, fcm.Naam));
                    }
                    _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_tvga}{fcm.Naam}", fcm.VeiligheidsGroenMinMG, CCOLElementTimeTypeEnum.TE_type, _tvga, fcm.Naam));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tvgb}{fcm.Naam}", fcm.VeiligheidsGroenTijdsduur, CCOLElementTimeTypeEnum.TE_type, _tvgb, fcm.Naam));
                }
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _myElements.Where(x => x.Type == type);
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCMeeverlengen:
                    return 20;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCMeeverlengen:
                    if (c.Fasen.SelectMany(x => x.Detectoren).Any(x => x.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.AppendLine($"{ts}/* Veiligheidsgroen */");
                        sb.AppendLine($"{ts}/* ---------------- */");
                        foreach (var fcm in c.Fasen)
                        {
                            foreach (var dm in fcm.Detectoren)
                            {
                                if (dm.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit)
                                {
                                    if (dm.VeiligheidsGroen == NooitAltijdAanUitEnum.Altijd)
                                    {
                                        sb.AppendLine($"{ts}veiligheidsgroen({_fcpf}{fcm.Naam}, {_tpf}{_tvga}{fcm.Naam}, {_tpf}{_tvgb}{fcm.Naam}, (bool)(TDH[{_dpf}{dm.Naam}]));");
                                    }
                                    else
                                    {
                                        sb.AppendLine($"{ts}veiligheidsgroen({_fcpf}{fcm.Naam}, {_tpf}{_tvga}{fcm.Naam}, {_tpf}{_tvgb}{fcm.Naam}, (bool)(SCH[{_schpf}{_schvg}{dm.Naam}] && TDH[{_dpf}{dm.Naam}]));");
                                    }
                                }
                            }
                        }
                        sb.AppendLine();
                    }
                    return sb.ToString();
                default:
                    return null;
            }
        }
    }
}
