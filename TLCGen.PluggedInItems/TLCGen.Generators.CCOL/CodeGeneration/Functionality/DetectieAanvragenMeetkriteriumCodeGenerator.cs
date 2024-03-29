﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
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
        private CCOLGeneratorCodeStringSettingModel _mmk;
        private CCOLGeneratorCodeStringSettingModel _tav;
#pragma warning restore 0169
#pragma warning restore 0649

        private int GetVerlengenSetting(FaseCyclusModel fc, DetectorModel dm)
        {
            var dmVerl = (int)dm.Verlengen;
            if (fc.ToepassenMK2)
            {
                switch (dm.Rijstrook)
                {
                    default:
                    case 1:
                        // leave as is
                        break;
                    case 2:
                        dmVerl += 4;
                        break;
                    case 3:
                        dmVerl += 8;
                        break;
                    case 4:
                        dmVerl += 12;
                        break;
                }
            }
            return dmVerl;
        }

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            var dets = c.Fasen.SelectMany(x => x.Detectoren).ToList();
            
            // Detectie aanvraag functie
            foreach (var dm in dets)
            {
                if (dm.Aanvraag == DetectorAanvraagTypeEnum.Geen)
                    continue;

                if (!dm.AanvraagHardOpStraat)
                {
                    var set = CCOLCodeHelper.GetAanvraagSetting(dm);
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmda}{dm.Naam}", set, CCOLElementTimeTypeEnum.None, _prmda, dm.Naam));
                }
                if (dm.ResetAanvraag)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tav}{dm.Naam}", dm.ResetAanvraagTijdsduur, CCOLElementTimeTypeEnum.TE_type, _tav, dm.Naam));
                }
            }

            // Detectie verlengkriterium
            foreach (var fc in c.Fasen)
            {
                foreach (var dm in fc.Detectoren)
                {
                    if (dm.Verlengen == DetectorVerlengenTypeEnum.Geen)
                        continue;
                    if (!dm.VerlengenHardOpStraat)
                    {
                        
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_prmmk}{dm.Naam}", GetVerlengenSetting(fc, dm), CCOLElementTimeTypeEnum.None, _prmmk, dm.Naam));
                    }
                }
            }

            // Collect Kopmax
            foreach (var fcm in c.Fasen.Where(x => x.Detectoren.Any(x2 => x2.Verlengen != DetectorVerlengenTypeEnum.Geen)))
            {
                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tkm}{fcm.Naam}",
                            fcm.Kopmax,
                            CCOLElementTimeTypeEnum.TE_type,
						    _tkm, fcm.Naam));
            }

            // Memory elems for meetkriterium2
            foreach(var fcm in c.Fasen.Where(x => x.ToepassenMK2))
            {
                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_mmk}{fcm.Naam}", _mmk, fcm.Naam));
            }
        }

        public override bool HasCCOLElements() => true;

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    return c.Fasen.Any(x => x.Detectoren?.Count > 0 && x.Detectoren.Any(x2 => x2.Aanvraag != DetectorAanvraagTypeEnum.Geen && x2.ResetAanvraag)) 
                        ? new List<CCOLLocalVariable> { new("int", "fc") } 
                        : base.GetFunctionLocalVariables(c, type);
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCAanvragen => new []{10},
                CCOLCodeTypeEnum.RegCMeetkriterium => new []{10},
                _ => null
            };
        }

        public override bool HasCodeForController(ControllerModel c, CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCAanvragen => c.Fasen.Any(x => x.Detectoren?.Any(x2 => x2.Aanvraag != DetectorAanvraagTypeEnum.Geen) == true),
                CCOLCodeTypeEnum.RegCMeetkriterium => c.Fasen.Any(x => x.Detectoren?.Any(x2 => x2.Verlengen != DetectorVerlengenTypeEnum.Geen) == true),
                _ => false
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    if (c.Fasen.Any(x => x.Detectoren?.Count > 0 && x.Detectoren.Any(x2 => x2.Aanvraag != DetectorAanvraagTypeEnum.Geen && x2.ResetAanvraag)))
                    {
                        sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                        sb.AppendLine($"{ts}{ts}RR[fc] &= ~BIT8;  /* reset BIT-sturing t.b.v. reset A */");
                        sb.AppendLine();
                    }
                    sb.AppendLine($"{ts}/* Detectie aanvragen */");
                    sb.AppendLine($"{ts}/* ------------------ */");
                    foreach (var fcm in c.Fasen)
                    {
                        if (fcm.Detectoren?.Count > 0 && fcm.Detectoren.Any(x => x.Aanvraag != DetectorAanvraagTypeEnum.Geen && !x.ResetAanvraag))
                        {
                            sb.AppendLine($"{ts}aanvraag_detectie_prm_va_arg((count) {_fcpf}{fcm.Naam}, ");
                            foreach (var dm in fcm.Detectoren)
                            {
                                if (!DetectorCanHaveAanvraag(dm)) continue;

                                if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen && !dm.ResetAanvraag)
                                {
                                    if(!dm.AanvraagHardOpStraat)
                                    {
                                        sb.AppendLine($"{ts}{ts}(va_count) {_dpf}{dm.Naam}, (va_mulv) PRM[{_prmpf}{_prmda}{dm.Naam}], ");
                                    }
                                    else if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen && dm.Aanvraag != DetectorAanvraagTypeEnum.Uit)
                                    {
                                        sb.AppendLine($"{ts}{ts}(va_count) {_dpf}{dm.Naam}, (va_mulv) {CCOLCodeHelper.GetAanvraagSetting(dm)}, ");
                                    }
                                }
                            }
                            sb.AppendLine($"{ts}{ts}(va_count) END);");
                        }
                        if (fcm.Detectoren?.Count > 0 && fcm.Detectoren.Any(x => x.Aanvraag != DetectorAanvraagTypeEnum.Geen && x.ResetAanvraag))
                        {
                            sb.AppendLine($"{ts}aanvraag_detectie_reset_prm_va_arg((count) {_fcpf}{fcm.Naam}, ");
                            foreach (var dm in fcm.Detectoren)
                            {
                                if (!DetectorCanHaveAanvraag(dm)) continue;

                                if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen && dm.ResetAanvraag)
                                {
                                    if (!dm.AanvraagHardOpStraat)
                                    {
                                        sb.AppendLine($"{ts}{ts}(va_count) {_dpf}{dm.Naam}, {_tpf}{_tav}{dm.Naam}, (va_mulv) PRM[{_prmpf}{_prmda}{dm.Naam}], ");
                                    }
                                    else if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen && dm.Aanvraag != DetectorAanvraagTypeEnum.Uit)
                                    {
                                        sb.AppendLine($"{ts}{ts}(va_count) {_dpf}{dm.Naam}, {_tpf}{_tav}{dm.Naam}, (va_mulv) {CCOLCodeHelper.GetAanvraagSetting(dm)}, ");
                                    }
                                }
                            }
                            sb.AppendLine($"{ts}{ts}(va_count) END);");
                        }
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    foreach (var fcm in c.Fasen)
                    {
                        var HasKopmax = false;
                        foreach (var dm in fcm.Detectoren)
                        {
                            if (dm.Verlengen != DetectorVerlengenTypeEnum.Geen)
                            {
                                HasKopmax = true;
                                break;
                            }
                        }
                        if (fcm.ToepassenMK2)
                        {
                            if (HasKopmax)
                                sb.AppendLine($"{ts}meetkriterium2_prm_va_arg((count){_fcpf}{fcm.Naam}, (count){_tpf}{_tkm}{fcm.Naam}, (count){_mpf}{_mmk}{fcm.Naam}, ");
                            else
                                sb.AppendLine($"{ts}meetkriterium2_prm_va_arg((count){_fcpf}{fcm.Naam}, NG, (count){_mpf}{_mmk}{fcm.Naam}, ");
                        }
                        else
                        {
                            if (HasKopmax)
                                sb.AppendLine($"{ts}meetkriterium_prm_va_arg((count){_fcpf}{fcm.Naam}, (count){_tpf}{_tkm}{fcm.Naam}, ");
                            else
                                sb.AppendLine($"{ts}meetkriterium_prm_va_arg((count){_fcpf}{fcm.Naam}, NG, ");
                        }
                        foreach (var dm in fcm.Detectoren)
                        {
                            if (!DetectorCanHaveVerlengen(dm)) continue;

                            if (dm.Verlengen != DetectorVerlengenTypeEnum.Geen)
                            {
                                sb.Append("".PadLeft($"{ts}meetkriterium_prm_va_arg(".Length));
                                if (!dm.VerlengenHardOpStraat)
                                {
                                    if (fcm.ToepassenMK2)
                                        sb.AppendLine($"({c.GetVaBoolV()})TDH[{dm.GetDefine()}], (va_mulv)PRM[{_prmpf}{_prmmk}{dm.Naam}],");
                                    else
                                        sb.AppendLine($"(va_count){dm.GetDefine()}, (va_mulv)PRM[{_prmpf}{_prmmk}{dm.Naam}],");
                                }
                                else
                                {
                                    if (fcm.ToepassenMK2)
                                        sb.AppendLine($"({c.GetVaBoolV()})TDH[{dm.GetDefine()}], (va_mulv){GetVerlengenSetting(fcm, dm)},");
                                    else
                                        sb.AppendLine($"(va_count){dm.GetDefine()}, (va_mulv){GetVerlengenSetting(fcm, dm)},");
                                }
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

        private bool DetectorCanHaveAanvraag(DetectorModel dm)
        {
            return dm.Type switch
            {
                DetectorTypeEnum.Kop => true,
                DetectorTypeEnum.Lang => true,
                DetectorTypeEnum.Verweg => true,
                DetectorTypeEnum.File => true,
                DetectorTypeEnum.Knop => true,
                DetectorTypeEnum.KnopBinnen => true,
                DetectorTypeEnum.KnopBuiten => true,
                DetectorTypeEnum.Radar => true,
                DetectorTypeEnum.WisselDetector => true,
                DetectorTypeEnum.WisselStroomKringDetector => true,
                DetectorTypeEnum.WisselStandDetector => true,
                DetectorTypeEnum.Overig => true,
                DetectorTypeEnum.VecomDetector => false,
                DetectorTypeEnum.OpticomIngang => false,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool DetectorCanHaveVerlengen(DetectorModel dm)
        {
            return dm.Type switch
            {
                DetectorTypeEnum.Kop => true,
                DetectorTypeEnum.Lang => true,
                DetectorTypeEnum.Verweg => true,
                DetectorTypeEnum.File => true,
                DetectorTypeEnum.Knop => true,
                DetectorTypeEnum.KnopBinnen => true,
                DetectorTypeEnum.KnopBuiten => true,
                DetectorTypeEnum.Radar => true,
                DetectorTypeEnum.WisselDetector => true,
                DetectorTypeEnum.WisselStroomKringDetector => true,
                DetectorTypeEnum.WisselStandDetector => true,
                DetectorTypeEnum.Overig => true,
                DetectorTypeEnum.VecomDetector => false,
                DetectorTypeEnum.OpticomIngang => false,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
