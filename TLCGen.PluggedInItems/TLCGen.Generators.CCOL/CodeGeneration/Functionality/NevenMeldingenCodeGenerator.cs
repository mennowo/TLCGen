using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class NevenMeldingenCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _prmtdbl;
        private CCOLGeneratorCodeStringSettingModel _prmtdbh;
        private CCOLGeneratorCodeStringSettingModel _prmrtn;
        private CCOLGeneratorCodeStringSettingModel _hovss;
        private CCOLGeneratorCodeStringSettingModel _hneven;
#pragma warning restore 0649
        private string _hpriouit;
        private string _hprioin;
        private string _prmda;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            
            var nevenMeldingen = CombineNevenMeldingenAndHeadDet(c);

            foreach (var nm in nevenMeldingen)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmtdbl}{nm.Item2}", nm.Item1.BezetTijdLaag, CCOLElementTimeTypeEnum.TE_type, _prmtdbl, nm.Item2));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmtdbh}{nm.Item2}", nm.Item1.BezetTijdHoog, CCOLElementTimeTypeEnum.TE_type, _prmtdbh, nm.Item2));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrtn}{nm.Item3}", nm.Item1.Rijtijd, CCOLElementTimeTypeEnum.TE_type, _prmrtn, nm.Item3));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hovss}{nm.Item1.FaseCyclus1}", _hovss, nm.Item1.FaseCyclus1));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hovss}{nm.Item1.FaseCyclus2}", _hovss, nm.Item1.FaseCyclus2));
                if (nm.Item1.FaseCyclus3 != "NG") _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hovss}{nm.Item1.FaseCyclus3}", _hovss, nm.Item1.FaseCyclus3));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hneven}{nm.Item1.FaseCyclus1}", _hneven, nm.Item1.FaseCyclus1));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hneven}{nm.Item1.FaseCyclus2}", _hneven, nm.Item1.FaseCyclus2));
                if (nm.Item1.FaseCyclus3 != "NG") _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hneven}{nm.Item1.FaseCyclus3}", _hneven, nm.Item1.FaseCyclus3));
            }
        }

        public override bool HasCCOLElements() => true;
        
        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.PrioCInUitMelden => new []{30},
                CCOLCodeTypeEnum.RegCAanvragen => new []{90},
                _ => null
            };
        }

        private List<Tuple<NevenMeldingModel, string, string>> CombineNevenMeldingenAndHeadDet(ControllerModel c)
        {
            var result = new List<Tuple<NevenMeldingModel, string, string>>();

            foreach (var nm in c.PrioData.NevenMeldingen)
            {
                DetectorModel kl = null;
                FaseCyclusModel fc = null;
                var fc1 = c.GetFaseCyclus(nm.FaseCyclus1);
                var fc2 = c.GetFaseCyclus(nm.FaseCyclus2);
                var fc3 = c.GetFaseCyclus(nm.FaseCyclus3);
                if (fc1 != null) { kl = fc1.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.Kop); fc = fc1; }
                if (kl == null && fc2 != null) { kl = fc2.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.Kop); fc = fc2; }
                if (kl == null && fc3 != null) { kl = fc3.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.Kop); fc = fc3; }
                if (kl != null) result.Add(new Tuple<NevenMeldingModel, string, string>(nm, kl.Naam, fc.Naam));
            }

            return result;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            var nevenMeldingen = CombineNevenMeldingenAndHeadDet(c);

            if (!nevenMeldingen.Any()) return "";

            switch (type)
            {
                case CCOLCodeTypeEnum.PrioCInUitMelden:
                    sb.AppendLine($"{ts}/* Nevenmeldingen: OV bij stopstreep (koplus bezet raakt na voldoende rijtijd, tot hiaattijd op koplus valt) */");
                    foreach (var nm in nevenMeldingen)
                    {
                        var ov1 = c.PrioData.PrioIngrepen.FirstOrDefault(x => x.FaseCyclus == nm.Item1.FaseCyclus1);
                        var ov2 = c.PrioData.PrioIngrepen.FirstOrDefault(x => x.FaseCyclus == nm.Item1.FaseCyclus2);
                        var ov3 = c.PrioData.PrioIngrepen.FirstOrDefault(x => x.FaseCyclus == nm.Item1.FaseCyclus3);

                        sb.AppendLine($"{ts}IH[{_hpf}{_hovss}{nm.Item1.FaseCyclus1}] = SD[{_dpf}{nm.Item2}] && iAantalInmeldingen[prioFC{CCOLCodeHelper.GetPriorityName(c, ov1)}] && (iRijTimer[prioFC{CCOLCodeHelper.GetPriorityName(c, ov1)}] >= PRM[{_prmpf}{_prmrtn}{nm.Item3}]) ? TRUE : IH[{_hpf}{_hovss}{nm.Item1.FaseCyclus1}] && TDH[{_dpf}{nm.Item2}];");
                        sb.AppendLine($"{ts}IH[{_hpf}{_hovss}{nm.Item1.FaseCyclus2}] = SD[{_dpf}{nm.Item2}] && iAantalInmeldingen[prioFC{CCOLCodeHelper.GetPriorityName(c, ov2)}] && (iRijTimer[prioFC{CCOLCodeHelper.GetPriorityName(c, ov2)}] >= PRM[{_prmpf}{_prmrtn}{nm.Item3}]) ? TRUE : IH[{_hpf}{_hovss}{nm.Item1.FaseCyclus2}] && TDH[{_dpf}{nm.Item2}];");
                        if (nm.Item1.FaseCyclus3 != "NG") sb.AppendLine($"{ts}IH[{_hpf}{_hovss}{nm.Item1.FaseCyclus3}] = SD[{_dpf}{nm.Item2}] && iAantalInmeldingen[prioFC{CCOLCodeHelper.GetPriorityName(c, ov3)}] && (iRijTimer[prioFC{CCOLCodeHelper.GetPriorityName(c, ov3)}] >= PRM[{_prmpf}{_prmrtn}{nm.Item3}]) ? TRUE : IH[{_hpf}{_hovss}{nm.Item1.FaseCyclus3}] && TDH[{_dpf}{nm.Item2}];");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Nevenmeldingen: in- en uitmelden */");
                    foreach (var nm in nevenMeldingen)
                    {
                        var ov1 = c.PrioData.PrioIngrepen.FirstOrDefault(x => x.FaseCyclus == nm.Item1.FaseCyclus1);
                        var ov2 = c.PrioData.PrioIngrepen.FirstOrDefault(x => x.FaseCyclus == nm.Item1.FaseCyclus2);
                        var ov3 = c.PrioData.PrioIngrepen.FirstOrDefault(x => x.FaseCyclus == nm.Item1.FaseCyclus3);

                        var prioFC3 = nm.Item1.FaseCyclus3 == "NG" ? "NG" : "prioFC" + CCOLCodeHelper.GetPriorityName(c, ov3);
                        var hovss3 = nm.Item1.FaseCyclus3 == "NG" ? "NG" : _hpf + _hovss + nm.Item1.FaseCyclus3;
                        var hneven3 = nm.Item1.FaseCyclus3 == "NG" ? "NG" : _hpf + _hneven + nm.Item1.FaseCyclus3;
                        sb.AppendLine($"{ts}NevenMelding(prioFC{CCOLCodeHelper.GetPriorityName(c, ov1)}, prioFC{CCOLCodeHelper.GetPriorityName(c, ov2)}, {prioFC3}, {_dpf}{nm.Item2}, {_prmpf}{_prmtdbl}{nm.Item2}, {_prmpf}{_prmtdbh}{nm.Item2}, {_hpf}{_hovss}{nm.Item1.FaseCyclus1}, {_hpf}{_hovss}{nm.Item1.FaseCyclus2}, {hovss3}, {_hpf}{_hneven}{nm.Item1.FaseCyclus1}, {_hpf}{_hneven}{nm.Item1.FaseCyclus2}, {hneven3});");
                        sb.AppendLine($"{ts}IH[{_hpf}{_hprioin}{CCOLCodeHelper.GetPriorityName(c, ov1)}] |= SH[{_hpf}{_hneven}{nm.Item1.FaseCyclus1}];");
                        sb.AppendLine($"{ts}IH[{_hpf}{_hprioin}{CCOLCodeHelper.GetPriorityName(c, ov2)}] |= SH[{_hpf}{_hneven}{nm.Item1.FaseCyclus2}];");
                        if (nm.Item1.FaseCyclus3 !="NG") sb.AppendLine($"{ts}IH[{_hpf}{_hprioin}{CCOLCodeHelper.GetPriorityName(c, ov3)}] |= SH[{_hpf}{_hneven}{nm.Item1.FaseCyclus3}];");
                        sb.AppendLine($"{ts}IH[{_hpf}{_hpriouit}{CCOLCodeHelper.GetPriorityName(c, ov1)}] |= EH[{_hpf}{_hneven}{nm.Item1.FaseCyclus1}];");
                        sb.AppendLine($"{ts}IH[{_hpf}{_hpriouit}{CCOLCodeHelper.GetPriorityName(c, ov2)}] |= EH[{_hpf}{_hneven}{nm.Item1.FaseCyclus2}];");
                        if (nm.Item1.FaseCyclus3 !="NG") sb.AppendLine($"{ts}IH[{_hpf}{_hpriouit}{CCOLCodeHelper.GetPriorityName(c, ov3)}] |= EH[{_hpf}{_hneven}{nm.Item1.FaseCyclus3}];");
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCAanvragen:
                    sb.AppendLine($"{ts}/* Neven aanvragen */");
                    foreach (var nm in nevenMeldingen)
                    {
                        var dm = c.GetAllDetectors(x => x.Naam == nm.Item2).FirstOrDefault();
                        var fasen = new List<string> { nm.Item1.FaseCyclus1, nm.Item1.FaseCyclus2 };
                        if (nm.Item1.FaseCyclus3 != "NG") fasen.Add(nm.Item1.FaseCyclus3);
                        fasen.Remove(nm.Item3);
                        foreach (var f in fasen)
                        {
                            sb.AppendLine(
                                $"{ts}aanvraag_detectie_prm_va_arg((count) {_fcpf}{f}, ");

                            if (dm != null && dm.Aanvraag != DetectorAanvraagTypeEnum.Geen && !dm.ResetAanvraag)
                            {
                                if (!dm.AanvraagHardOpStraat)
                                {
                                    sb.AppendLine(
                                        $"{ts}{ts}(va_count) {_dpf}{dm.Naam}, (va_mulv) PRM[{_prmpf}{_prmda}{dm.Naam}], ");
                                }
                                else if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen &&
                                         dm.Aanvraag != DetectorAanvraagTypeEnum.Uit)
                                {
                                    sb.AppendLine(
                                        $"{ts}{ts}(va_count) {_dpf}{dm.Naam}, (va_mulv) {CCOLCodeHelper.GetAanvraagSetting(dm)}, ");
                                }
                            }
                            
                            sb.AppendLine($"{ts}{ts}(va_count) END);");
                        }

                    } 

                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Resetten neven aanvragen */");
                    foreach (var nm in nevenMeldingen)
                    {
                        if (nm.Item1.FaseCyclus3 == "NG") 
                        {
                            sb.AppendLine($"{ts}if(ED[{_dpf}{nm.Item2}] && (!R[{_fcpf}{nm.Item1.FaseCyclus2}] || TRG[{_fcpf}{nm.Item1.FaseCyclus2}]) && A[{_fcpf}{nm.Item1.FaseCyclus1}] & BIT0) A[{_fcpf}{nm.Item1.FaseCyclus1}] &= ~BIT0;");
                            sb.AppendLine($"{ts}if(ED[{_dpf}{nm.Item2}] && (!R[{_fcpf}{nm.Item1.FaseCyclus1}] || TRG[{_fcpf}{nm.Item1.FaseCyclus1}]) && A[{_fcpf}{nm.Item1.FaseCyclus2}] & BIT0) A[{_fcpf}{nm.Item1.FaseCyclus2}] &= ~BIT0;");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}if(ED[{_dpf}{nm.Item2}] && (!R[{_fcpf}{nm.Item1.FaseCyclus2}] || TRG[{_fcpf}{nm.Item1.FaseCyclus2}] || !R[{_fcpf}{nm.Item1.FaseCyclus3}] || TRG[{_fcpf}{nm.Item1.FaseCyclus3}]) && A[{_fcpf}{nm.Item1.FaseCyclus1}] & BIT0) A[{_fcpf}{nm.Item1.FaseCyclus1}] &= ~BIT0;");
                            sb.AppendLine($"{ts}if(ED[{_dpf}{nm.Item2}] && (!R[{_fcpf}{nm.Item1.FaseCyclus1}] || TRG[{_fcpf}{nm.Item1.FaseCyclus1}] || !R[{_fcpf}{nm.Item1.FaseCyclus3}] || TRG[{_fcpf}{nm.Item1.FaseCyclus3}]) && A[{_fcpf}{nm.Item1.FaseCyclus2}] & BIT0) A[{_fcpf}{nm.Item1.FaseCyclus2}] &= ~BIT0;");
                            sb.AppendLine($"{ts}if(ED[{_dpf}{nm.Item2}] && (!R[{_fcpf}{nm.Item1.FaseCyclus1}] || TRG[{_fcpf}{nm.Item1.FaseCyclus1}] || !R[{_fcpf}{nm.Item1.FaseCyclus2}] || TRG[{_fcpf}{nm.Item1.FaseCyclus2}]) && A[{_fcpf}{nm.Item1.FaseCyclus3}] & BIT0) A[{_fcpf}{nm.Item1.FaseCyclus3}] &= ~BIT0;");
                        }
                    }

                    foreach (var nm in nevenMeldingen)
                    {
                        sb.AppendLine($"{ts}RR[{_fcpf}{nm.Item1.FaseCyclus1}] &= ~BIT7; if(RA[{_fcpf}{nm.Item1.FaseCyclus1}] && !A[{_fcpf}{nm.Item1.FaseCyclus1}]) RR[{_fcpf}{nm.Item1.FaseCyclus1}] |= BIT7;");
                        sb.AppendLine($"{ts}RR[{_fcpf}{nm.Item1.FaseCyclus2}] &= ~BIT7; if(RA[{_fcpf}{nm.Item1.FaseCyclus2}] && !A[{_fcpf}{nm.Item1.FaseCyclus2}]) RR[{_fcpf}{nm.Item1.FaseCyclus2}] |= BIT7;");
                        if (nm.Item1.FaseCyclus3 != "NG") sb.AppendLine($"{ts}RR[{_fcpf}{nm.Item1.FaseCyclus3}] &= ~BIT7; if(RA[{_fcpf}{nm.Item1.FaseCyclus3}] && !A[{_fcpf}{nm.Item1.FaseCyclus3}]) RR[{_fcpf}{nm.Item1.FaseCyclus3}] |= BIT7;");
                    }
                    return sb.ToString();
            }

            return sb.ToString();
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _hprioin = CCOLGeneratorSettingsProvider.Default.GetElementName("hprioin");
            _hpriouit = CCOLGeneratorSettingsProvider.Default.GetElementName("hpriouit");
            _prmda = CCOLGeneratorSettingsProvider.Default.GetElementName("prmda");

            return base.SetSettings(settings);
        }
    }
}
