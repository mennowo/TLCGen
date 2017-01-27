using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    [CCOLCodePieceGenerator]
    public class CCOLWaitsignalenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyBitmapOutputs = new List<CCOLIOElement>();

            // outputs
            foreach (var fc in c.Fasen)
            {
                foreach (var d in fc.Detectoren)
                {
                    if(d.Wachtlicht)
                    {
                        _MyElements.Add(new CCOLElement("wt" + d.Naam, CCOLElementTypeEnum.Uitgang));
                        _MyBitmapOutputs.Add(new CCOLIOElement(d.WachtlichtBitmapData as IOElementModel, "uswt" + d.Naam));
                    }
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

        public override bool HasCCOLBitmapOutputs()
        {
            return true;
        }

        public override IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs()
        {
            return _MyBitmapOutputs;
        }

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.SystemApplication:
                    return true;
                default:
                    return false;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string tabspace)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.SystemApplication:
                    sb.AppendLine("/* wachtlicht uitsturing */");
                    sb.AppendLine("/* --------------------- */");
                    foreach (var fc in c.Fasen)
                    {
                        foreach (var d in fc.Detectoren)
                        {
                            if (d.Wachtlicht)
                            {
                                string wtdef = d.WachtlichtBitmapData.GetBitmapCoordinaatOutputDefine("wt" + d.Naam);
                                string ddef = d.GetDefine();
                                string fcdef = fc.GetDefine();
                                if (c.Data.AansturingWaitsignalen == AansturingWaitsignalenEnum.DrukknopGebruik)
                                {
                                    sb.AppendLine($"{tabspace}CIF_GUS[{wtdef}] = (D[{ddef}] && !SD[{ddef}] || ED[{ddef}]) && A[{fcdef}] && !G[{fcdef}] && REG ? TRUE : CIF_GUS[{wtdef}] && !G[{fcdef}] && REG;");
                                }
                                else if (c.Data.AansturingWaitsignalen == AansturingWaitsignalenEnum.AanvraagGezet)
                                {
                                    sb.AppendLine($"{tabspace}CIF_GUS[{wtdef}] = !G[{fcdef}] && A[{fcdef}] && REG;");
                                }
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
