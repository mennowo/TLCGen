using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class DetectieAanvragenMeetkriteriumCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
#pragma warning disable 0169
        private CCOLGeneratorCodeStringSettingModel _prmda;
		private CCOLGeneratorCodeStringSettingModel _prmmk;
        private CCOLGeneratorCodeStringSettingModel _tkm;
        private CCOLGeneratorCodeStringSettingModel _tav;
#pragma warning restore 0169
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            List<DetectorModel> dets = new List<DetectorModel>();
            dets.AddRange(c.Fasen.SelectMany(x => x.Detectoren));
            //dets.AddRange(c.Detectoren); not for loose detectors!

            // Detectie aanvraag functie
            foreach (DetectorModel dm in dets)
            {
                if (dm.Aanvraag == DetectorAanvraagTypeEnum.Geen)
                    continue;

                int set = 0;
                switch (dm.Aanvraag)
                {
                    case DetectorAanvraagTypeEnum.Uit:
                        set = 0;
                        break;
                    case DetectorAanvraagTypeEnum.RnietTRG:
                        set = 1;
                        break;
                    case DetectorAanvraagTypeEnum.Rood:
                        set = 2;
                        break;
                    case DetectorAanvraagTypeEnum.RoodGeel:
                        set = 3;
                        break;
                }
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmda}{dm.Naam}", set, CCOLElementTimeTypeEnum.None, _prmda, dm.Naam));
                if (dm.ResetAanvraag)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tav}{dm.Naam}", dm.ResetAanvraagTijdsduur, CCOLElementTimeTypeEnum.TE_type, _tav, dm.Naam));
                }
            }

            // Detectie verlengkriterium
            foreach (DetectorModel dm in dets)
            {
                if (dm.Verlengen == DetectorVerlengenTypeEnum.Geen)
                    continue;
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmmk}{dm.Naam}", (int)dm.Verlengen, CCOLElementTimeTypeEnum.TE_type, _prmmk, dm.Naam));
            }

            // Collect Kopmax
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
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tkm}{fcm.Naam}",
                            fcm.Kopmax,
                            CCOLElementTimeTypeEnum.TE_type,
						    _tkm, fcm.Naam));
                }
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _myElements.Where(x => x.Type == type);
        }

        public override bool HasFunctionLocalVariables()
        {
            return true;
        }

        public override IEnumerable<Tuple<string, string>> GetFunctionLocalVariables(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    return new List<Tuple<string, string>> { new Tuple<string, string>("int", "fc") };
                default:
                    return base.GetFunctionLocalVariables(type);
            }
        }

        public override bool HasFunctionLocalVariablesForController(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    return (c.Fasen.Any(x => x.Detectoren?.Count > 0 && x.Detectoren.Any(x2 => x2.Aanvraag != DetectorAanvraagTypeEnum.Geen && x2.ResetAanvraag)));
            }
            return base.HasFunctionLocalVariablesForController(c, type);
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    return 10;
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    return 10;
                default:
                    return 0;
            }
        }

        public override bool HasCodeForController(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    return c.Fasen.Any(x => x.Detectoren?.Any(x2 => x2.Aanvraag != DetectorAanvraagTypeEnum.Geen) == true);
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    return c.Fasen.Any(x => x.Detectoren?.Any(x2 => x2.Verlengen != DetectorVerlengenTypeEnum.Geen) == true);
                default:
                    return false;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    if (c.Fasen.Any(x => x.Detectoren?.Count > 0 && x.Detectoren.Any(x2 => x2.Aanvraag != DetectorAanvraagTypeEnum.Geen && x2.ResetAanvraag)))
                    {
                        sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                        sb.AppendLine($"{ts}{ts}RR[fc] &= ~BIT8;  /* reset BIT-sturing t.b.v. reset A */");
                    }
                    sb.AppendLine($"{ts}/* Detectie aanvragen */");
                    sb.AppendLine($"{ts}/* ------------------ */");
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (fcm.Detectoren?.Count > 0 && fcm.Detectoren.Any(x => x.Aanvraag != DetectorAanvraagTypeEnum.Geen && !x.ResetAanvraag))
                        {
                            sb.AppendLine($"{ts}aanvraag_detectie_prm_va_arg((count) {_fcpf}{fcm.Naam}, ");
                            foreach (DetectorModel dm in fcm.Detectoren)
                            {
                                if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen && !dm.ResetAanvraag)
                                    sb.AppendLine($"{ts}{ts}(va_count) {_dpf}{dm.Naam}, (va_mulv) PRM[{_prmpf}{_prmda}{dm.Naam}], ");
                            }
                            sb.AppendLine($"{ts}{ts}(va_count) END);");
                        }
                        if (fcm.Detectoren?.Count > 0 && fcm.Detectoren.Any(x => x.Aanvraag != DetectorAanvraagTypeEnum.Geen && x.ResetAanvraag))
                        {
                            sb.AppendLine($"{ts}aanvraag_detectie_reset_prm_va_arg((count) {_fcpf}{fcm.Naam}, ");
                            foreach (DetectorModel dm in fcm.Detectoren)
                            {
                                if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen && dm.ResetAanvraag)
                                    sb.AppendLine($"{ts}{ts}(va_count) {_dpf}{dm.Naam}, {_tpf}{_tav}{dm.Naam}, (va_mulv) PRM[{_prmpf}{_prmda}{dm.Naam}], ");
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
