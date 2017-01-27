using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration 
{
    [CCOLCodePieceGenerator]
    public class CCOLRichtingGevoeligCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            foreach (RichtingGevoeligeAanvraagModel rga in c.RichtingGevoeligeAanvragen)
            {
                _MyElements.Add(new CCOLElement($"rgad{rga.VanDetector}", rga.MaxTijdsVerschil, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                _MyElements.Add(new CCOLElement($"rgadd{rga.VanDetector}", 1, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
            }

            foreach (RichtingGevoeligVerlengModel rgv in c.RichtingGevoeligVerlengen)
            {
                _MyElements.Add(
                    new CCOLElement(
                        $"rgrd{rgv.VanDetector}_d{rgv.NaarDetector}", 
                        rgv.MaxTijdsVerschil, 
                        CCOLElementTimeTypeEnum.TE_type, 
                        CCOLElementTypeEnum.Timer));
                _MyElements.Add(
                    new CCOLElement(
                        $"rgvd{rgv.VanDetector}_d{rgv.NaarDetector}",
                        rgv.VerlengTijd,
                        CCOLElementTimeTypeEnum.TE_type,
                        CCOLElementTypeEnum.Timer));
                _MyElements.Add(
                    new CCOLElement(
                        $"rgvd{rgv.VanDetector}_d{rgv.NaarDetector}",
                        CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(
                    new CCOLElement(
                        $"mkrgd{rgv.VanDetector}",
                        (int)rgv.TypeVerlengen,
                        CCOLElementTimeTypeEnum.TE_type,
                        CCOLElementTypeEnum.Parameter));
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
                case CCOLRegCCodeTypeEnum.Meetkriterium:
                case CCOLRegCCodeTypeEnum.Aanvragen:
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
                case CCOLRegCCodeTypeEnum.Aanvragen:
                    sb.AppendLine($"{tabspace}/* Richtinggevoelige aanvragen */");
                    sb.AppendLine($"{tabspace}/* --------------------------= */");
                    foreach (RichtingGevoeligeAanvraagModel rga in c.RichtingGevoeligeAanvragen)
                    {
                        sb.AppendLine($"{tabspace}aanvraag_richtinggevoelig(fc{rga.FaseCyclus}, d{rga.NaarDetector}, d{rga.VanDetector}, trgad{rga.VanDetector}, SCH[schrgadd{rga.VanDetector}]);");
                    }
                    return sb.ToString();
                case CCOLRegCCodeTypeEnum.Meetkriterium:
                    sb.AppendLine($"{tabspace}/* Richtinggevoelig verlengen */");
                    sb.AppendLine($"{tabspace}/* -------------------------- */");
                    foreach (RichtingGevoeligVerlengModel rgv in c.RichtingGevoeligVerlengen)
                    {
                        sb.AppendLine($"{tabspace}MeetKriteriumRGprm((count) fc{rgv.FaseCyclus}, (count) tkm{rgv.FaseCyclus},");
                        sb.AppendLine($"{tabspace}{tabspace}(bool) RichtingVerlengen(fc{rgv.FaseCyclus}, d{rgv.VanDetector}, d{rgv.NaarDetector},");
                        sb.AppendLine($"{tabspace}{tabspace}                         trgrd{rgv.VanDetector}_d{rgv.NaarDetector}, trgvd{rgv.VanDetector}_d{rgv.NaarDetector},");
                        sb.AppendLine($"{tabspace}{tabspace}                         hrgvd{rgv.VanDetector}_d{rgv.NaarDetector}), (mulv)PRM[prmmkrgd{rgv.VanDetector}],");
                        sb.AppendLine($"{tabspace}{tabspace}(count)END);");
                    }
                    sb.AppendLine();
                    return sb.ToString();
                default:
                    return null;
            }
        }
    }
}
