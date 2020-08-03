using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class DetectieAanvraagDirectCodeGenerator : CCOLCodePieceGeneratorBase
    {
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
                                sb.AppendLine($"{ts}AanvraagSnelV2({_fcpf}{fc.Naam}, {_dpf}{d.Naam});");
                            }
                        }
                    }
                    return sb.ToString();
                default:
                    return null;
            }
        }
    }
}
