using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLVasteAanvraagCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

#pragma warning disable 0649
        private string _schca;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            foreach (FaseCyclusModel fcm in c.Fasen)
            {
                if (fcm.VasteAanvraag != Models.Enumerations.NooitAltijdAanUitEnum.Nooit &&
                    fcm.VasteAanvraag != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_schca}{fcm.Naam}",
                            fcm.VasteAanvraag == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ? 1 : 0,
                            CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
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

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string tabspace)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Aanvragen:
                    sb.AppendLine($"{tabspace}/* Vaste aanvragen */");
                    sb.AppendLine($"{tabspace}/* --------------- */");
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (fcm.VasteAanvraag == NooitAltijdAanUitEnum.SchAan ||
                            fcm.VasteAanvraag == NooitAltijdAanUitEnum.SchUit)
                            sb.AppendLine($"{tabspace}if (SCH[{_schpf}{_schca}{fcm.Naam}]) vaste_aanvraag({_fcpf}{fcm.Naam});");
                        else if (fcm.VasteAanvraag == NooitAltijdAanUitEnum.Altijd)
                            sb.AppendLine($"{tabspace}vaste_aanvraag({_fcpf}{fcm.Naam});");
                    }
                    sb.AppendLine();
                    return sb.ToString();
                default:
                    return null;
            }
        }
    }
}
