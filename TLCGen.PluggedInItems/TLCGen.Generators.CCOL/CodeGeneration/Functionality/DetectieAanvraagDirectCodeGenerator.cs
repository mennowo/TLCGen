using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class DetectieAanvraagDirectCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private string _prmda;
     
        public override int HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCAanvragen => 20,
                _ => 0
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    var i = 0;
                    foreach(var fc in c.Fasen)
                    {
                        foreach(var d in fc.Detectoren)
                        {
                            if(d.AanvraagDirect)
                            {
                                if(i == 0)
                                {
                                    sb.AppendLine($"{ts}/* Direct groen in geval van !K voor een richting */");
                                    ++i;
                                }
                                if (d.Aanvraag != Models.Enumerations.DetectorAanvraagTypeEnum.Geen)
                                {
                                    if (d.AanvraagHardOpStraat)
                                    {
                                        sb.AppendLine($"{ts}AanvraagSnelV2({_fcpf}{fc.Naam}, {_dpf}{d.Naam});");
                                    }
                                    else
                                    {
                                        sb.AppendLine($"{ts}if (PRM[{_prmpf}{_prmda}{d.Naam}] != 0) AanvraagSnelV2({_fcpf}{fc.Naam}, {_dpf}{d.Naam});");
                                    }
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
