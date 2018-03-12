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
    public class DetectieAanvragenMeetkriteriumCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

#pragma warning disable 0649
#pragma warning disable 0169
        private string _prmda;
		private string _prmmk;
        private string _tkm;
		private string _prmda_D;
		private string _prmmk_D;
        private string _tkm_D;
#pragma warning restore 0169
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            List<DetectorModel> dets = new List<DetectorModel>();
            dets.AddRange(c.Fasen.SelectMany(x => x.Detectoren));
            //dets.AddRange(c.Detectoren); not for loose detectors!

            // Detectie aanvraag functie
            foreach (DetectorModel dm in dets)
            {
                if (dm.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.Geen)
                    continue;

                int set = 0;
                switch (dm.Aanvraag)
                {
                    case Models.Enumerations.DetectorAanvraagTypeEnum.Uit:
                        set = 0;
                        break;
                    case Models.Enumerations.DetectorAanvraagTypeEnum.RnietTRG:
                        set = 1;
                        break;
                    case Models.Enumerations.DetectorAanvraagTypeEnum.Rood:
                        set = 2;
                        break;
                    case Models.Enumerations.DetectorAanvraagTypeEnum.RoodGeel:
                        set = 3;
                        break;
                }
                _MyElements.Add(new CCOLElement($"{_prmda}{dm.Naam}", set, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter,  
					CCOLGeneratorSettingsProvider.Default.GetElementDescription(_prmda_D, dm.Naam)));
            }

            // Detectie verlengkriterium
            foreach (DetectorModel dm in dets)
            {
                if (dm.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.Geen)
                    continue;
                
                _MyElements.Add(
                    new CCOLElement(
                        $"{_prmmk}{dm.Naam}", 
                        (int)dm.Verlengen, 
                        CCOLElementTimeTypeEnum.TE_type,
                        CCOLElementTypeEnum.Parameter,
						CCOLGeneratorSettingsProvider.Default.GetElementDescription(_prmmk_D, dm.Naam)));
            }

            // Collect Kopmax
            foreach (FaseCyclusModel fcm in c.Fasen)
            {
                bool HasKopmax = false;
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    if (dm.Verlengen != Models.Enumerations.DetectorVerlengenTypeEnum.Geen)
                    {
                        HasKopmax = true;
                        break;
                    }
                }
                if (HasKopmax)
                {
                    _MyElements.Add(new CCOLElement(
                        $"{_tkm}{fcm.Naam}",
                        fcm.Kopmax,
                        CCOLElementTimeTypeEnum.TE_type,
                        CCOLElementTypeEnum.Timer,
						CCOLGeneratorSettingsProvider.Default.GetElementDescription(_tkm_D, fcm.Naam)));
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

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    return 20;
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    return 10;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    sb.AppendLine($"{ts}/* Detectie aanvragen */");
                    sb.AppendLine($"{ts}/* ------------------ */");
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (fcm.Detectoren?.Count > 0 && fcm.Detectoren.Where(x => x.Aanvraag != DetectorAanvraagTypeEnum.Geen).Any())
                        {
                            sb.AppendLine($"{ts}aanvraag_detectie_prm_va_arg((count) {_fcpf}{fcm.Naam}, ");
                            foreach (DetectorModel dm in fcm.Detectoren)
                            {
                                if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen)
                                    sb.AppendLine($"{ts}{ts}(va_count) {_dpf}{dm.Naam}, (va_mulv) PRM[{_prmpf}{_prmda}{dm.Naam}], ");
                            }
                            sb.AppendLine($"{ts}{ts}(va_count) END);");
                        }
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        bool HasKopmax = false;
                        foreach (DetectorModel dm in fcm.Detectoren)
                        {
                            if (dm.Verlengen != DetectorVerlengenTypeEnum.Geen)
                            {
                                HasKopmax = true;
                                break;
                            }
                        }
                        if (HasKopmax)
                            sb.AppendLine($"{ts}meetkriterium_prm_va_arg((count){_fcpf}{fcm.Naam}, (count){_tpf}{_tkm}{fcm.Naam}, ");
                        else
                            sb.AppendLine($"{ts}meetkriterium_prm_va_arg((count){_fcpf}{fcm.Naam}, NG, ");
                        foreach (DetectorModel dm in fcm.Detectoren)
                        {
                            if (dm.Verlengen != DetectorVerlengenTypeEnum.Geen)
                            {
                                sb.Append("".PadLeft($"{ts}meetkriterium_prm_va_arg(".Length));
                                sb.AppendLine($"(va_count){dm.GetDefine()}, (va_mulv)PRM[{_prmpf}{_prmmk}{dm.Naam}],");
                            }
                        }
                        sb.Append("".PadLeft($"{ts}meetkriterium_prm_va_arg(".Length));
                        sb.AppendLine($"(va_count)END);");
                    }
                    sb.AppendLine();
                    return sb.ToString();

                default:
                    return null;
            }
        }
    }
}
