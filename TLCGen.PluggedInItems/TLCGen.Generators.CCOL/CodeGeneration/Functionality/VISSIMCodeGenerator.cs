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
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCTop:
                    return 40;
				case CCOLCodeTypeEnum.TabCControlParameters:
					return 10;
                default:
                    return 0;
            }
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
		            foreach (var d in c.Fasen.SelectMany(x => x.Detectoren))
		            {
			            if (!string.IsNullOrWhiteSpace(d.VissimNaam))
			            {
				            sb.AppendLine($"{ts}D_code[{_dpf}{d.Naam}] = \"{d.VissimNaam}\";");
			            }
		            }
		            foreach (var d in c.Detectoren)
		            {
			            if (!string.IsNullOrWhiteSpace(d.VissimNaam))
			            {
				            sb.AppendLine($"{ts}D_code[{_dpf}{d.Naam}] = \"{d.VissimNaam}\";");
			            }
		            }
                    foreach (var d in c.SelectieveDetectoren)
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
