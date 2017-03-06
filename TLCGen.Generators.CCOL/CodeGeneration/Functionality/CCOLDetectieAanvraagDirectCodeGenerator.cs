using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLDetectieAanvraagDirectCodeGenerator : CCOLCodePieceGeneratorBase
    {
        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Aanvragen:
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
                case CCOLRegCCodeTypeEnum.Aanvragen:
                    int i = 0;
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
                    sb.AppendLine();
                    return sb.ToString();
                default:
                    return null;
            }
        }
    }
}
