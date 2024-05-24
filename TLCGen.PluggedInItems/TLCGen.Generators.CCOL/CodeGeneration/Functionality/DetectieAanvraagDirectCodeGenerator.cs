using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class DetectieAanvraagDirectCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schsnel;
#pragma warning restore 0649
        private string _prmda;
     
        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var fc in c.Fasen)
            {
                foreach (var d in fc.Detectoren.Where(x => x.AanvraagDirectSch != Models.Enumerations.NooitAltijdAanUitEnum.Nooit))
                {
                    if (d.AanvraagDirectSch != Models.Enumerations.NooitAltijdAanUitEnum.Altijd &&
                        d.AanvraagDirectSch != Models.Enumerations.NooitAltijdAanUitEnum.Nooit &&
                        d.Aanvraag != Models.Enumerations.DetectorAanvraagTypeEnum.Geen)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_schsnel}{_dpf}{d.Naam}",
                                d.AanvraagDirectSch == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ? 1 : 0,
                                CCOLElementTimeTypeEnum.SCH_type,
                                _schsnel, d.Naam));
                    }
                }
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCAanvragen => new []{20},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    var i = 0;
                    foreach(var fc in c.Fasen)
                    {
                        foreach (var d in fc.Detectoren.Where(x => x.AanvraagDirectSch != Models.Enumerations.NooitAltijdAanUitEnum.Nooit))
                        {
                            if (i == 0)
                            {
                                sb.AppendLine($"{ts}/* Direct groen in geval van !K voor een richting */");
                                ++i;
                                foreach(var fc2 in c.Fasen.Where(x => x.Detectoren.Any(x2 => x2.AanvraagDirectSch != Models.Enumerations.NooitAltijdAanUitEnum.Nooit)))
                                {
                                    sb.AppendLine($"{ts}if (!RA[{_fcpf}{fc2.Naam}] && !G[{_fcpf}{fc2.Naam}]) A[{_fcpf}{fc2.Naam}] &= ~BIT5;");
                                }
                                sb.AppendLine();
                            }

                            if (d.Aanvraag != Models.Enumerations.DetectorAanvraagTypeEnum.Geen)
                            {
                                if (d.AanvraagHardOpStraat)
                                {
                                    if (d.AanvraagDirectSch == Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                                        sb.AppendLine($"{ts}AanvraagSnelV3({_fcpf}{fc.Naam}, {_dpf}{d.Naam});");
                                    else
                                        sb.AppendLine($"{ts}if (SCH[{_schpf}{_schsnel}{_dpf}{d.Naam}]) AanvraagSnelV3({_fcpf}{fc.Naam}, {_dpf}{d.Naam});");
                                }
                                else
                                {
                                if (d.AanvraagDirectSch == Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                                    sb.AppendLine($"{ts}if (PRM[{_prmpf}{_prmda}{d.Naam}] != 0) AanvraagSnelV3({_fcpf}{fc.Naam}, {_dpf}{d.Naam});");
                                else
                                    sb.AppendLine($"{ts}if (PRM[{_prmpf}{_prmda}{d.Naam}] != 0 && SCH[{_schpf}{_schsnel}{_dpf}{d.Naam}]) AanvraagSnelV3({_fcpf}{fc.Naam}, {_dpf}{d.Naam});");
                                }
                            }
                        }
                    }
                    return sb.ToString();
                default:
                    return null;
            }
        }
        
        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _prmda = CCOLGeneratorSettingsProvider.Default.GetElementName("prmda");

            return base.SetSettings(settings);
        }
    }
}
