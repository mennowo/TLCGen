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
    public class CCOLDetectieAanvragenMeetkriteriumCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

        private string _prmd;  // parameter request type name
        private string _prmmk; // parameter measurement type name
        private string _tkm;   // kopmax timer type name

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            List<DetectorModel> dets = new List<DetectorModel>();
            dets.AddRange(c.Fasen.SelectMany(x => x.Detectoren));
            dets.AddRange(c.Detectoren);

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
                _MyElements.Add(new CCOLElement($"{_prmd}{dm.Naam}", set, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
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
                case CCOLRegCCodeTypeEnum.Meetkriterium:
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
                    sb.AppendLine($"{ts}/* Detectie aanvragen */");
                    sb.AppendLine($"{ts}/* ------------------ */");
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        bool HasA = false;
                        if (fcm.Detectoren?.Count > 0)
                        {
                            foreach (DetectorModel dm in fcm.Detectoren)
                            {
                                if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen)
                                {
                                    HasA = true;
                                    break;
                                }
                            }
                        }
                        if (HasA)
                        {
                            sb.AppendLine($"{ts}aanvraag_detectie_prm_va_arg((count) {_fcpf}{fcm.Naam}, ");
                            foreach (DetectorModel dm in fcm.Detectoren)
                            {
                                if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen)
                                    sb.AppendLine($"{ts}{ts}(va_count) {_dpf}{dm.Naam}, (va_mulv) PRM[{_prmpf}{_prmd}{dm.Naam}], ");
                            }
                            sb.AppendLine($"{ts}{ts}(va_count) END);");
                        }
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.Meetkriterium:
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        bool HasKopmax = false;
                        foreach (DetectorModel dm in fcm.Detectoren)
                        {
                            if (dm.Verlengen == DetectorVerlengenTypeEnum.Kopmax)
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
                                sb.AppendLine($"(va_count){dm.GetDefine()}, (va_mulv)PRM[{_prmpf}{_prmmk}{dm.GetDefine()}],");
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

        public override bool HasSettings()
        {
            return true;
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            if (settings == null || settings.Settings == null)
            {
                return false;
            }

            foreach (var s in settings.Settings)
            {
                if (s.Default == "da") _prmd = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "mk") _prmmk = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "km") _tkm = s.Setting == null ? s.Default : s.Setting;
            }

            return base.SetSettings(settings);
        }
    }
}
