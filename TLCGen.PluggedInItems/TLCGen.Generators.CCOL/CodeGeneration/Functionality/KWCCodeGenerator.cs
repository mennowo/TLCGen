using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class KWCCodeGenerator : CCOLCodePieceGeneratorBase
    {
        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCIncludes => new []{10},
                CCOLCodeTypeEnum.RegCKwcApplication => new []{10},
                CCOLCodeTypeEnum.RegCSystemApplication => new []{20},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:
                    if(c.Data.KWCType == Models.Enumerations.KWCTypeEnum.Vialis)
                    {
                        sb.AppendLine($"#if (!defined AUTOMAAT) && (!defined NO_KWC)");
                        sb.AppendLine($"{ts}#include \"pi_ccol.h\"");
                        sb.AppendLine($"#elif (!defined NO_KWC)");
                        sb.AppendLine($"{ts}#include \"pi_ccol.c\"");
                        sb.AppendLine($"#endif");
                    }
                    else if (c.Data.KWCType != Models.Enumerations.KWCTypeEnum.Geen)
                    {
                        sb.AppendLine($"#if (!defined AUTOMAAT) && (!defined NO_KWC)");
                        sb.AppendLine($"{ts}#include \"mv_ccol.h\"");
                        sb.AppendLine($"#elif (!defined NO_KWC)");
                        sb.AppendLine($"{ts}#include \"mv_ccol.c\"");
                        sb.AppendLine($"#endif");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSystemApplication:
                    if (c.Data.KWCType == Models.Enumerations.KWCTypeEnum.Vialis)
                    {
                        sb.AppendLine($"#if (defined AUTOMAAT || defined AUTOMAAT_TEST) && (!defined VISSIM) && (!defined NO_KWC)");
                        sb.AppendLine($"{ts}PI_save();");
                        sb.AppendLine($"#endif");
                    }
                    else if (c.Data.KWCType != Models.Enumerations.KWCTypeEnum.Geen)
                    {
                        sb.AppendLine($"#if (defined AUTOMAAT || defined AUTOMAAT_TEST) && (!defined VISSIM) && (!defined NO_KWC)");
                        sb.AppendLine($"{ts}MvSave();");
                        sb.AppendLine($"#endif");
                    }
                    return sb.ToString();

                default:
                    return null;
            }
        }
    }
}
