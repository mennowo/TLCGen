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
    public class CCOLWaitsignalenCodeGenerator : ICCOLCodePieceGenerator
    {
        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;

        public void CollectCCOLElements(ControllerModel c)
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
                        _MyElements.Add(new CCOLElement() { Define = d.WachtlichtBitmapData.GetBitmapCoordinaatOutputDefine("wt" + d.Naam), Naam = "wt" + d.Naam });
                        _MyBitmapOutputs.Add(new CCOLIOElement(d.WachtlichtBitmapData as IOElementModel, "uswt" + d.Naam));
                    }
                }
            }
        }

        public IEnumerable<CCOLElement> GetCCOLElements(CCOLElementType type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public IEnumerable<CCOLIOElement> GetCCOLBitmapInputs()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs()
        {
            return _MyBitmapOutputs;
        }

        public string GetCode(ControllerModel c, CCOLCodeType type, string tabspace)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeType.SystemApplication:
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
