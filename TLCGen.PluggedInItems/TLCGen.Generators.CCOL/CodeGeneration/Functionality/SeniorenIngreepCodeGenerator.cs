﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class SeniorenIngreepCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _tdbsiexgr;
        private CCOLGeneratorCodeStringSettingModel _hsiexgr;
        private CCOLGeneratorCodeStringSettingModel _hsiexgrd;
        private CCOLGeneratorCodeStringSettingModel _schsi;
        private CCOLGeneratorCodeStringSettingModel _prmsiexgrperc;
        private CCOLGeneratorCodeStringSettingModel _tsiexgr;
#pragma warning restore 0649
        private string _tnlsgd;
        private string _uswt;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var fc in c.Fasen.Where(x => x.SeniorenIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Nooit && 
                                                  x.Detectoren.Any(x2 => x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen ||
                                                                         x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten)))
            {
                if (fc.SeniorenIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_schsi}{fc.Naam}",
                            fc.SeniorenIngreep == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ? 1 : 0,
                            CCOLElementTimeTypeEnum.SCH_type,
                            _schsi,
                            fc.Naam));
                }
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmsiexgrperc}{fc.Naam}",
                        fc.SeniorenIngreepExtraGroenPercentage,
                        CCOLElementTimeTypeEnum.TE_type,
                        _prmsiexgrperc,
                        fc.Naam));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tsiexgr}{fc.Naam}",
                        999,
                        CCOLElementTimeTypeEnum.TE_type,
                        _tsiexgr,
                        fc.Naam));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_hsiexgr}{fc.Naam}",
                        _hsiexgr,
                        fc.Naam));
                foreach (var d in fc.Detectoren.Where(x => x.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen || x.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten))
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tdbsiexgr}{_dpf}{d.Naam}",
                            fc.SeniorenIngreepBezetTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            _tdbsiexgr,
                            d.Naam));
                    if (c.InterSignaalGroep.Meeaanvragen.Any(x => x.Detectoren.Any(x2 => x2.MeeaanvraagDetector == d.Naam)))
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_hsiexgrd}{_dpf}{d.Naam}",
                                _hsiexgrd,
                                fc.Naam,
                                d.Naam));
                    }
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override bool HasFunctionLocalVariables() => true;

        public override IEnumerable<Tuple<string, string, string>> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPreApplication:
                    return new List<Tuple<string, string, string>> { new Tuple<string, string, string>("int", "fc", "") };
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPreApplication:
                    return 80;
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    return 40;
            }
            return 0;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var fcs = c.Fasen.Where(x => x.SeniorenIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Nooit &&
                                         x.Detectoren.Any(x2 => x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen || x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten)).ToList();
            if (!fcs.Any()) return null;

            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPreApplication:
                    sb.AppendLine($"{ts}/* Reset BITs senioren ingreep */");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}    if (US_type[fc] & VTG_type) RW[fc] &= ~BIT7;");
                    sb.AppendLine($"{ts}}}");
                    break;
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    sb.AppendLine($"{ts}/* Seniorengroen (percentage van TFG extra als WG) */");
                    foreach(var fc in fcs)
                    {
                        var dks = fc.Detectoren.Where(x => x.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten || x.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen).ToList();
                        var dk1 = dks.FirstOrDefault();
                        DetectorModel dk2 = ((dks.Count() > 1) ? dks[1] : null);
                        var nl1 = dk1 == null ? false : c.InterSignaalGroep.Meeaanvragen.Any(x => x.Detectoren.Any(x2 => x2.MeeaanvraagDetector == dk1.Naam));
                        var nl2 = dk2 == null ? false : c.InterSignaalGroep.Meeaanvragen.Any(x => x.Detectoren.Any(x2 => x2.MeeaanvraagDetector == dk2.Naam));
                        var nl_extra = c.InterSignaalGroep.Meeaanvragen.Where(x => x.FaseNaar == fc.Naam && x.Detectoren.Any());
                        var extra_d = "";
                        sb.Append($"{ts}");
                        if (fc.SeniorenIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                        {
                            sb.Append($"if (SCH[{_schpf}{_schsi}{fc.Naam}]) ");
                        }
                        if (nl_extra.Any())
                        {
                            foreach(var nl in nl_extra)
                            {
                                var mafc = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseVan);
                                if (mafc == null || mafc.SeniorenIngreep == Models.Enumerations.NooitAltijdAanUitEnum.Nooit) continue;
                                foreach (var d in nl.Detectoren)
                                {
                                    if (fc.Detectoren.Any(x => x.Naam == d.MeeaanvraagDetector)) continue;
                                    extra_d += $"{_hpf}{_hsiexgrd}{_dpf}{d.MeeaanvraagDetector}, ";
                                }
                            }
                            extra_d += "END";
                        }
                        else
                        {
                            extra_d = "END";
                        }
                        sb.AppendLine($"SeniorenGroen({_fcpf}{fc.Naam}, " +
                            $"{(dk1 != null ? $"{_dpf}{dk1.Naam}" : "NG")}, " +
                            $"{(dk1 != null ? $"{_tpf}{_tdbsiexgr}{_dpf}{dk1.Naam}" : "NG")}, " +
                            $"{(nl1 ? $"{_hpf}{_hsiexgrd}{_dpf}{dk1.Naam}" : "NG")}, " +
                            $"{(dk2 != null ? $"{_dpf}{dk2.Naam}" : "NG")}, " +
                            $"{(dk2 != null ? $"{_tpf}{_tdbsiexgr}{_dpf}{dk2.Naam}" : "NG")}, " +
                            $"{(nl2 ? $"{_hpf}{_hsiexgrd}{_dpf}{dk2.Naam}" : "NG")}, " +
                            $"{_prmpf}{_prmsiexgrperc}{fc.Naam}, " + 
                            $"{_hpf}{_hsiexgr}{fc.Naam}, " +
                            $"{_tpf}{_tsiexgr}{fc.Naam}, " +
                            $"{extra_d});");
                    }
                    break;
            }

            return sb.ToString();
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _tnlsgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsgd");
            _uswt = CCOLGeneratorSettingsProvider.Default.GetElementName("uswt");

            return base.SetSettings(settings);
        }
    }
}
