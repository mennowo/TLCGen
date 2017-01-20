using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public class CCOLRoBuGroverCodeGenerator : ICCOLCodePieceGenerator
    {
        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;

        public void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyBitmapOutputs = new List<CCOLIOElement>();


        }

        public IEnumerable<CCOLIOElement> GetCCOLBitmapInputs()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs()
        {
            return _MyBitmapOutputs;
        }

        public IEnumerable<CCOLElement> GetCCOLElements(CCOLElementType type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string tabspace)
        {
            if(c.RoBuGrover.ConflictGroepen?.Count == 0)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            switch(type)
            {
                case CCOLRegCCodeTypeEnum.Top:
                    sb.AppendLine($"{tabspace}/* Robuuste Groenverdeler */");
                    sb.AppendLine($"{tabspace}#include \"{c.Data.Naam}rgv.c\"");
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.Maxgroen:
                    sb.AppendLine($"{tabspace}/* AANROEP EN RAPPOTEREN ROBUGROVER */");
                    sb.AppendLine($"{tabspace}if (SCH[schrgv] != 0)");
                    sb.AppendLine($"{tabspace}{{");
                    sb.AppendLine($"{tabspace}{tabspace}int teller = 0;");
                    sb.AppendLine();
                    foreach(var cg in c.RoBuGrover.ConflictGroepen)
                    {
                        sb.Append($"{tabspace}{tabspace}TC[teller++] = berekencyclustijd_va_arg(");
                        foreach(var fc in cg.Fasen)
                        {
#warning Change so it uses prefix settings (also at the end!)
                            sb.Append($"fc{fc.FaseCyclus}, ");
                        }
                        sb.AppendLine($"END);");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{tabspace}{tabspace}TC_max = TC[0];");
                    sb.AppendLine();
                    sb.AppendLine($"{tabspace}{tabspace}for (teller = 1; teller < MAX_AANTAL_CONFLICTGROEPEN; ++teller)");
                    sb.AppendLine($"{tabspace}{tabspace}{{");
                    sb.AppendLine($"{tabspace}{tabspace}{tabspace}if (TC_max < TC[teller])");
                    sb.AppendLine($"{tabspace}{tabspace}{tabspace}{{");
                    sb.AppendLine($"{tabspace}{tabspace}{tabspace}{tabspace}TC_max = TC[teller];");
                    sb.AppendLine($"{tabspace}{tabspace}{tabspace}}}");
                    sb.AppendLine($"{tabspace}{tabspace}}}");
                    sb.AppendLine($"{tabspace}#if !defined AUTOMAAT");
                    sb.AppendLine($"{tabspace}{tabspace}for (teller = 0; teller < MAX_AANTAL_CONFLICTGROEPEN; ++teller)");
                    sb.AppendLine($"{tabspace}{tabspace}{{");
                    sb.AppendLine($"{tabspace}{tabspace}{tabspace}xyprintf(50, teller + 1, \"%4d\", TC[teller]);");
                    sb.AppendLine($"{tabspace}{tabspace}}}");
                    sb.AppendLine($"{tabspace}#endif");
                    sb.AppendLine();
                    sb.AppendLine($"{tabspace}{tabspace}/* AANROEP ROBUUSTE GROENTIJD VERDELER */");
                    sb.AppendLine($"{tabspace}{tabspace}/* ================================== */");
                    sb.AppendLine($"{tabspace}{tabspace}rgv_add();");
                    sb.AppendLine();
                    sb.AppendLine($"{tabspace}{tabspace}CIF_GUS[usrgv] = TRUE;");
                    sb.AppendLine($"{tabspace}}}");
                    sb.AppendLine($"{tabspace}else");
                    sb.AppendLine($"{tabspace}{{");
                    foreach (var fc in c.Fasen)
                    {
                        if(fc.Type == Models.Enumerations.FaseTypeEnum.Auto)
                        {
                            sb.AppendLine($"{tabspace}{tabspace}TVG_rgv[fc{fc.Naam}] = TVG_basis[fc{fc.Naam}];");
                        }
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{tabspace}{tabspace}CIF_GUS[usrgv] = FALSE;");
                    sb.AppendLine($"{tabspace}}}");

                    return sb.ToString();

                default:
                    return null;
            }
        }
    }
}
