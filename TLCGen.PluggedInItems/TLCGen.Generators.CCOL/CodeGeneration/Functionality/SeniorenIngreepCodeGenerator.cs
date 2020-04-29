using System;
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
        private CCOLGeneratorCodeStringSettingModel _schsi;
        private CCOLGeneratorCodeStringSettingModel _prmsiexgrperc;
        private CCOLGeneratorCodeStringSettingModel _tsiexgr;
#pragma warning restore 0649
        private string _tnlsg;
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
                    var fcs = c.Fasen.Where(x => x.SeniorenIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Nooit &&
                                         x.Detectoren.Any(x2 => x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen || x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten)).ToList();
                    return !fcs.Any() 
                        ? base.GetFunctionLocalVariables(c, type) 
                        : new List<Tuple<string, string, string>> { new Tuple<string, string, string>("int", "fc", "") };
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
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    return 50;
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
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    sb.AppendLine($"{ts}/* Seniorengroen (percentage van TFG extra als WG) */");
                    foreach(var fc in fcs)
                    {
                        var dks = fc.Detectoren.Where(x => x.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten || x.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen).ToList();
                        var dk1 = dks.FirstOrDefault();
                        var dk2 = dks.Count > 1 ? dks[1] : null;
                        var nlExtra = c.InterSignaalGroep.Nalopen.Where(x => x.Type == Models.Enumerations.NaloopTypeEnum.StartGroen && x.FaseVan == fc.Naam);
                        var extraD = "";
                        sb.Append($"{ts}");
                        if (fc.SeniorenIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                        {
                            sb.Append($"if (SCH[{_schpf}{_schsi}{fc.Naam}]) ");
                        }
                        if (nlExtra.Any())
                        {
                            foreach(var nl in nlExtra)
                            {
                                var tnl = _tnlsg;
                                if (nl.DetectieAfhankelijk)
                                {
                                    tnl = _tnlsgd;
                                }
                                extraD += _tpf + tnl + nl.FaseVan + nl.FaseNaar + ", ";
                            }
                            extraD += "END";
                        }
                        else
                        {
                            extraD = "END";
                        }
                        sb.AppendLine($"SeniorenGroen({_fcpf}{fc.Naam}, " +
                            $"{(dk1 != null ? $"{_dpf}{dk1.Naam}" : "NG")}, " +
                            $"{(dk1 != null ? $"{_tpf}{_tdbsiexgr}{_dpf}{dk1.Naam}" : "NG")}, " +
                            $"{(dk2 != null ? $"{_dpf}{dk2.Naam}" : "NG")}, " +
                            $"{(dk2 != null ? $"{_tpf}{_tdbsiexgr}{_dpf}{dk2.Naam}" : "NG")}, " +
                            $"{_prmpf}{_prmsiexgrperc}{fc.Naam}, " + 
                            $"{_hpf}{_hsiexgr}{fc.Naam}, " +
                            $"{_tpf}{_tsiexgr}{fc.Naam}, " +
                            $"{extraD});");
                    }
                    break;
            }

            return sb.ToString();
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _tnlsg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsg");
            _tnlsgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsgd");
            _uswt = CCOLGeneratorSettingsProvider.Default.GetElementName("uswt");

            return base.SetSettings(settings);
        }
    }
}
