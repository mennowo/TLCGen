using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class NevenMeldingenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private string _hplact;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _prmtdbl;
        private CCOLGeneratorCodeStringSettingModel _prmtdbh;
        private CCOLGeneratorCodeStringSettingModel _hovss;
        private CCOLGeneratorCodeStringSettingModel _hneven;
#pragma warning restore 0649
        private string _prmrto;
        private string _hpriouit;
        private string _hprioin;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            
            var nevenMeldingen = CombineNevenMeldingenAndHeadDet(c);

            foreach (var nm in nevenMeldingen)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmtdbl}{nm.Item2}", nm.Item1.BezetTijdLaag, CCOLElementTimeTypeEnum.TE_type, _prmtdbl, nm.Item2));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmtdbh}{nm.Item2}", nm.Item1.BezetTijdHoog, CCOLElementTimeTypeEnum.TE_type, _prmtdbh, nm.Item2));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hovss}{nm.Item1.FaseCyclus1}", _hovss, nm.Item1.FaseCyclus1));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hovss}{nm.Item1.FaseCyclus2}", _hovss, nm.Item1.FaseCyclus2));
                if (nm.Item1.FaseCyclus3 != "NG") _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hovss}{nm.Item1.FaseCyclus3}", _hovss, nm.Item1.FaseCyclus3));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hneven}{nm.Item1.FaseCyclus1}", _hneven, nm.Item1.FaseCyclus1));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hneven}{nm.Item1.FaseCyclus2}", _hneven, nm.Item1.FaseCyclus2));
                if (nm.Item1.FaseCyclus3 != "NG") _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hneven}{nm.Item1.FaseCyclus3}", _hneven, nm.Item1.FaseCyclus3));
            }
        }

        public override bool HasCCOLElements() => true;
        
        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.PrioCInUitMelden:
                    return 30;
            }
            return 0;
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

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.PrioCInUitMelden:
                    var nevenMeldingen = CombineNevenMeldingenAndHeadDet(c);
                    sb.AppendLine($"{ts}/* Nevenmeldingen: OV bij stopstreep (koplus bezet raakt na voldoende rijtijd, tot hiaattijd op koplus valt) */");
                    foreach (var nm in nevenMeldingen)
                    {
                        sb.AppendLine($"{ts}IH[{_hpf}{_hovss}{nm.Item1.FaseCyclus1}] = SD[{_dpf}{nm.Item2}] && iAantalInmeldingen[prioFC{nm.Item1.FaseCyclus1}] && (iRijTimer[prioFC{nm.Item1.FaseCyclus1}] >= PRM[{_prmpf}{_prmrto}{nm.Item3}]) ? TRUE : IH[{_hpf}{_hovss}{nm.Item1.FaseCyclus1}] && TDH[{_dpf}{nm.Item2}];");
                        sb.AppendLine($"{ts}IH[{_hpf}{_hovss}{nm.Item1.FaseCyclus2}] = SD[{_dpf}{nm.Item2}] && iAantalInmeldingen[prioFC{nm.Item1.FaseCyclus2}] && (iRijTimer[prioFC{nm.Item1.FaseCyclus2}] >= PRM[{_prmpf}{_prmrto}{nm.Item3}]) ? TRUE : IH[{_hpf}{_hovss}{nm.Item1.FaseCyclus2}] && TDH[{_dpf}{nm.Item2}];");
                        if (nm.Item1.FaseCyclus3 != "NG") sb.AppendLine($"{ts}IH[{_hpf}{_hovss}{nm.Item1.FaseCyclus3}] = SD[{_dpf}{nm.Item2}] && iAantalInmeldingen[prioFC{nm.Item1.FaseCyclus3}] && (iRijTimer[prioFC{nm.Item1.FaseCyclus3}] >= PRM[{_prmpf}{_prmrto}{nm.Item3}]) ? TRUE : IH[{_hpf}{_hovss}{nm.Item1.FaseCyclus3}] && TDH[{_dpf}{nm.Item2}];");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Nevenmeldingen: in- en uitmelden */");
                    foreach (var nm in nevenMeldingen)
                    {
                        var prioFC3 = nm.Item1.FaseCyclus3 == "NG" ? "NG" : "prioFC" + nm.Item1.FaseCyclus3;
                        var hovss3 = nm.Item1.FaseCyclus3 == "NG" ? "NG" : _hpf + _hovss + nm.Item1.FaseCyclus3;
                        var hneven3 = nm.Item1.FaseCyclus3 == "NG" ? "NG" : _hpf + _hneven + nm.Item1.FaseCyclus3;
                        sb.AppendLine($"{ts}NevenMelding(prioFC{nm.Item1.FaseCyclus1}, prioFC{nm.Item1.FaseCyclus2}, {prioFC3}, {_dpf}{nm.Item2}, {_prmpf}{_prmtdbl}{nm.Item2}, {_prmpf}{_prmtdbh}{nm.Item2}, {_hpf}{_hovss}{nm.Item1.FaseCyclus1}, {_hpf}{_hovss}{nm.Item1.FaseCyclus2}, {hovss3}, {_hpf}{_hneven}{nm.Item1.FaseCyclus1}, {_hpf}{_hneven}{nm.Item1.FaseCyclus2}, {hneven3}, {_prmpf}{_prmrto}{nm.Item3});");
                        sb.AppendLine($"{ts}IH[{_hpf}{_hprioin}{nm.Item1.FaseCyclus1}] |= SH[{_hpf}{_hneven}{nm.Item1.FaseCyclus1}];");
                        sb.AppendLine($"{ts}IH[{_hpf}{_hprioin}{nm.Item1.FaseCyclus2}] |= SH[{_hpf}{_hneven}{nm.Item1.FaseCyclus2}];");
                        if (nm.Item1.FaseCyclus3 !="NG") sb.AppendLine($"{ts}IH[{_hpf}{_hprioin}{nm.Item1.FaseCyclus3}] |= SH[{_hpf}{_hneven}{nm.Item1.FaseCyclus3}];");
                        sb.AppendLine($"{ts}IH[{_hpf}{_hpriouit}{nm.Item1.FaseCyclus1}] |= EH[{_hpf}{_hneven}{nm.Item1.FaseCyclus1}];");
                        sb.AppendLine($"{ts}IH[{_hpf}{_hpriouit}{nm.Item1.FaseCyclus2}] |= EH[{_hpf}{_hneven}{nm.Item1.FaseCyclus2}];");
                        if (nm.Item1.FaseCyclus3 !="NG") sb.AppendLine($"{ts}IH[{_hpf}{_hpriouit}{nm.Item1.FaseCyclus3}] |= EH[{_hpf}{_hneven}{nm.Item1.FaseCyclus3}];");
                    }
                    return sb.ToString();
            }

            return sb.ToString();
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _prmrto = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrto");
            _hprioin = CCOLGeneratorSettingsProvider.Default.GetElementName("hprioin");
            _hpriouit = CCOLGeneratorSettingsProvider.Default.GetElementName("hpriouit");

            return base.SetSettings(settings);
        }
    }
}
