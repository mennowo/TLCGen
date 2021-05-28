using System.Linq;
using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class VISSIMCodeGenerator : CCOLCodePieceGeneratorBase
    {
        public override int HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCTop => 40,
                CCOLCodeTypeEnum.TabCControlParameters => 10,
                _ => 0
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCTop:
					sb.AppendLine($"/* kruispuntnaam in VISSIM */");
					sb.AppendLine($"#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) || defined VISSIM");
					sb.AppendLine($"{ts}code SCJ_code[] = \"{c.Data.VissimNaam}\";");
					sb.AppendLine($"#endif");
                    return sb.ToString();

	            case CCOLCodeTypeEnum.TabCControlParameters:
		            sb.AppendLine("#ifdef VISSIM");
		            var ovdummydets = c.PrioData.GetAllDummyDetectors();
		            var alldets = c.GetAllDetectors().Concat(ovdummydets);
		            foreach (var d in alldets)
		            {
			            if (!string.IsNullOrWhiteSpace(d.VissimNaam))
			            {
				            sb.AppendLine($"{ts}D_code[{_dpf}{d.Naam}] = \"{d.VissimNaam}\";");
			            }
		            }
					sb.AppendLine("#endif");
					return sb.ToString();

				default:
                    return null;
            }
        }
    }
}
