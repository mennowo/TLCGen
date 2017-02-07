using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLKWCCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

        private string _prmmaxtvg;
        private string _prmmaxtfb;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            if (c.Data.KWCType != Models.Enumerations.KWCTypeEnum.Geen)
            {
                //
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

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Includes:
                case CCOLRegCCodeTypeEnum.KwcApplication:
                case CCOLRegCCodeTypeEnum.SystemApplication:
                    return true;
                default:
                    return false;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Includes:
                    if(c.Data.KWCType == Models.Enumerations.KWCTypeEnum.Vialis)
                    {
                        sb.AppendLine($"#ifndef AUTOMAAT");
                        sb.AppendLine($"{ts}#include \"pi_ccol.h\"");
                        sb.AppendLine($"#else");
                        sb.AppendLine($"{ts}#include \"pi_ccol.c\"");
                        sb.AppendLine($"#endif");
                    }
                    else if (c.Data.KWCType != Models.Enumerations.KWCTypeEnum.Geen)
                    {
                        sb.AppendLine($"#ifndef AUTOMAAT");
                        sb.AppendLine($"{ts}#include \"mv_ccol.h\"");
                        sb.AppendLine($"#else");
                        sb.AppendLine($"{ts}#include \"mv_ccol.c\"");
                        sb.AppendLine($"#endif");
                    }
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.SystemApplication:
                    if (c.Data.KWCType == Models.Enumerations.KWCTypeEnum.Vialis)
                    {
                        sb.AppendLine($"#if (!defined AUTOMAAT) && (!defined VISSIM)");
                        sb.AppendLine($"{ts}PI_save();");
                        sb.AppendLine($"#endif");
                    }
                    else if (c.Data.KWCType != Models.Enumerations.KWCTypeEnum.Geen)
                    {
                        sb.AppendLine($"#if (!defined AUTOMAAT) && (!defined VISSIM)");
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
