using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class VeiligheidsGroenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

#pragma warning disable 0649
        private string _schvg; // schakelaar veiligheidsgroen
        private string _tvga; // veiligheidsgroen min. tijdsduur in MG
        private string _tvgb; // tijdsduur veiligheidsgroen
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            foreach (var fcm in c.Fasen)
            {
                if(fcm.Detectoren.Any(x => x.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit))
                {
                    foreach (var dm in fcm.Detectoren.Where(x => x.VeiligheidsGroen != NooitAltijdAanUitEnum.Altijd))
                    {
                        _MyElements.Add(
                            new CCOLElement(
                                $"{_schvg}{dm.Naam}",
                                dm.VeiligheidsGroen == NooitAltijdAanUitEnum.SchAan ? 1 : 0,
                                CCOLElementTimeTypeEnum.SCH_type,
                                CCOLElementTypeEnum.Schakelaar));
                    }
                    _MyElements.Add(
                            new CCOLElement(
                                $"{_tvga}{fcm.Naam}",
                                fcm.VeiligheidsGroenMinMG,
                                CCOLElementTimeTypeEnum.TE_type,
                                CCOLElementTypeEnum.Timer));
                        _MyElements.Add(
                            new CCOLElement(
                                $"{_tvgb}{fcm.Naam}",
                                fcm.VeiligheidsGroenTijdsduur,
                                CCOLElementTimeTypeEnum.TE_type,
                                CCOLElementTypeEnum.Timer));
                }
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
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
