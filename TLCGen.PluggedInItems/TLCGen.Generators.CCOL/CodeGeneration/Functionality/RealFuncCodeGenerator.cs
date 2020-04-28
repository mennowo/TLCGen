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
    public class RealFuncCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _hinl;
        private CCOLGeneratorCodeStringSettingModel _tinl;
        private CCOLGeneratorCodeStringSettingModel _treallr;
        private CCOLGeneratorCodeStringSettingModel _trealvs;
        private CCOLGeneratorCodeStringSettingModel _hlos;
        private CCOLGeneratorCodeStringSettingModel _schlos;
        private CCOLGeneratorCodeStringSettingModel _mrealtijd;
#pragma warning restore 0649
        private string _hmad;
        private GroenSyncDataModel _groenSyncData;
        private List<string> _fasenMetSync;

        #endregion // Fields

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.RealFunc) return;

            _groenSyncData = GroenSyncDataModel.ConvertSyncFuncToRealFunc(c);
            var (oneWay, twoWay, twoWayPedestrians) = GroenSyncDataModel.OrderSyncs(c, _groenSyncData);
            _fasenMetSync = _groenSyncData.GroenSyncFasen.Select(x => x.FaseVan)
                .Concat(_groenSyncData.GroenSyncFasen.Select(x => x.FaseNaar)).Distinct()
                .ToList();
            _fasenMetSync.Sort();

            foreach (var model in oneWay)
            {
                var max = model.Waarde < 0 ? _treallr : _trealvs;
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{max}{model.FaseVan}{model.FaseNaar}",
                        model.Waarde < 0 ? model.Waarde * -1 : model.Waarde,
                        CCOLElementTimeTypeEnum.TE_type,
                        max, model.FaseVan, model.FaseNaar));
            }

            var helps = new List<string>();

            foreach (var (m1, m2, gelijkstart) in twoWayPedestrians)
            {
                if (gelijkstart) continue;

                if (!helps.Contains($"h{_hinl}{m1.FaseVan}"))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hinl}{m1.FaseVan}", _hinl, m1.FaseVan));
                    helps.Add($"h{_hinl}{m1.FaseVan}");
                }
                if (!helps.Contains($"h{_hlos}{m1.FaseVan}"))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hlos}{m1.FaseVan}", _hlos, m1.FaseVan));
                    helps.Add($"h{_hlos}{m1.FaseVan}");
                }
                if (!helps.Contains($"h{_hinl}{m2.FaseVan}"))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hinl}{m2.FaseVan}", _hinl, m2.FaseVan));
                    helps.Add($"h{_hinl}{m2.FaseVan}");
                }
                if (!helps.Contains($"h{_hlos}{m2.FaseVan}"))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hlos}{m2.FaseVan}", _hlos, m2.FaseVan));
                    helps.Add($"h{_hlos}{m2.FaseVan}");
                }

                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tinl}{m1.FaseVan}{m1.FaseNaar}",
                        m1.Waarde < 0 ? m1.Waarde * -1 : m1.Waarde,
                        CCOLElementTimeTypeEnum.TE_type,
                        _tinl, m1.FaseVan, m1.FaseNaar));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tinl}{m2.FaseVan}{m2.FaseNaar}",
                        m2.Waarde < 0 ? m2.Waarde * -1 : m2.Waarde,
                        CCOLElementTimeTypeEnum.TE_type,
                        _tinl, m2.FaseVan, m2.FaseNaar));

                if (!helps.Contains($"s{_schlos}{m1.FaseVan}_1"))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schlos}{m1.FaseVan}_1", 0, CCOLElementTimeTypeEnum.SCH_type, _schlos, m1.FaseVan));
                    helps.Add($"s{_schlos}{m1.FaseVan}_1");
                }
                if (!helps.Contains($"s{_schlos}{m1.FaseVan}_2"))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schlos}{m1.FaseVan}_2", 0, CCOLElementTimeTypeEnum.SCH_type, _schlos, m1.FaseVan));
                    helps.Add($"s{_schlos}{m1.FaseVan}_2");
                }
                if (!helps.Contains($"s{_schlos}{m2.FaseVan}_1"))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schlos}{m2.FaseVan}_1", 0, CCOLElementTimeTypeEnum.SCH_type, _schlos, m2.FaseVan));
                    helps.Add($"s{_schlos}{m2.FaseVan}_1");
                }
                if (!helps.Contains($"s{_schlos}{m2.FaseVan}_2"))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schlos}{m2.FaseVan}_2", 0, CCOLElementTimeTypeEnum.SCH_type, _schlos, m2.FaseVan));
                    helps.Add($"s{_schlos}{m2.FaseVan}_2");
                }
                
            }

            foreach (var model in _fasenMetSync)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_mrealtijd}{model}",
                        _mrealtijd, model));
            }


        }

        public override bool HasCCOLElements() => true;

        public override bool HasFunctionLocalVariables() => true;
        
        public override IEnumerable<Tuple<string, string, string>> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.RealFunc 
                        || c.InterSignaalGroep?.Gelijkstarten?.Count == 0 && c.InterSignaalGroep?.Voorstarten?.Count == 0 && c.InterSignaalGroep?.LateReleases?.Count == 0)
                        return base.GetFunctionLocalVariables(c, type);
                    return new List<Tuple<string, string, string>> { new Tuple<string, string, string>("boolv", "wijziging", "TRUE") };
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.RealFunc 
                        || c.InterSignaalGroep?.Gelijkstarten?.Count == 0 && c.InterSignaalGroep?.Voorstarten?.Count == 0 && c.InterSignaalGroep?.LateReleases?.Count == 0)
                        return base.GetFunctionLocalVariables(c, type);
                    return new List<Tuple<string, string, string>> { new Tuple<string, string, string>("int", "fc", "") };
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }
        
        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:
                    return 50;
                case CCOLCodeTypeEnum.RegCAanvragen:
                    return 80;
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    return 30;
				default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            // return if no sync
            if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.RealFunc ||
                c.InterSignaalGroep?.Gelijkstarten?.Count == 0 
                && c.InterSignaalGroep?.Voorstarten?.Count == 0
                && c.InterSignaalGroep?.LateReleases?.Count == 0)
                return null;

            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:
                    sb.AppendLine($"{ts}#include \"realfunc.c\"");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCAanvragen:

                    // Order
                    var (oneWay, twoWay, twoWayPedestrians) = GroenSyncDataModel.OrderSyncs(c, _groenSyncData);

                    // TODO test and check logic
                    var threeWayPedestrians = GetThreeWayPedestirans(twoWayPedestrians);

                    // Two-way negative pedestrians
                    var startDuringRed = false;
                    foreach (var (m1, _, gelijkstart) in twoWayPedestrians)
                    {
                        if (gelijkstart) continue;

                        var fc1 = c.Fasen.FirstOrDefault(x => x.Naam == m1.FaseVan);
                        var fc2 = c.Fasen.FirstOrDefault(x => x.Naam == m1.FaseNaar);
                        if (fc1 == null || fc2 == null) continue;

                        var mdr1A = fc1.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.KnopBuiten);
                        var mdr1B = fc1.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.KnopBinnen);
                        var mdr2A = fc2.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.KnopBuiten);
                        var mdr2B = fc2.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.KnopBinnen);
                        var dr1A = mdr1A == null ? "NG" : _dpf + mdr1A.Naam;
                        var dr1B = mdr1B == null ? "NG" : _dpf + mdr1B.Naam;
                        var dr2A = mdr2A == null ? "NG" : _dpf + mdr2A.Naam;
                        var dr2B = mdr2B == null ? "NG" : _dpf + mdr2B.Naam;
                        var hdr1A = mdr1A == null ? "NG" : _hpf + _hmad + mdr1A.Naam;
                        var hdr1B = mdr1B == null ? "NG" : _hpf + _hmad + mdr1B.Naam;
                        var hdr2A = mdr2A == null ? "NG" : _hpf + _hmad + mdr2A.Naam;
                        var hdr2B = mdr2B == null ? "NG" : _hpf + _hmad + mdr2B.Naam;
                        sb.AppendLine($"{ts}Inlopen_Los2({_fcpf}{m1.FaseVan}, {_fcpf}{m1.FaseNaar}, {dr1A}, {dr1B}, {dr2B}, {dr2A}, {hdr1A}, {hdr1B}, {hdr2B}, {hdr2A}, {_hpf}{_hinl}{m1.FaseVan}, {_hpf}{_hinl}{m1.FaseNaar}, {_hpf}{_hlos}{m1.FaseVan}, {_hpf}{_hlos}{m1.FaseVan}, {_schpf}{_schlos}{m1.FaseVan}_1, {_schpf}{_schlos}{m1.FaseVan}_2, {_schpf}{_schlos}{m1.FaseNaar}_1, {_schpf}{_schlos}{m1.FaseNaar}_2);");
                        startDuringRed = true;
                    }

                    if (startDuringRed)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* Herstarten/afkappen inlooptijd */");
                        foreach (var (m1, _, gelijkstart) in twoWayPedestrians)
                        {
                            if (gelijkstart) continue;

                            sb.AppendLine($"{ts}RT[{_tpf}{_tinl}{m1.FaseVan}{m1.FaseNaar}] = SG[{_fcpf}{m1.FaseVan}] && H[{_hpf}{_hinl}{m1.FaseVan}]; AT[{_tpf}{_tinl}{m1.FaseVan}{m1.FaseNaar}] = G[{_fcpf}{m1.FaseNaar}];");
                            sb.AppendLine($"{ts}RT[{_tpf}{_tinl}{m1.FaseNaar}{m1.FaseVan}] = SG[{_fcpf}{m1.FaseNaar}] && H[{_hpf}{_hinl}{m1.FaseNaar}]; AT[{_tpf}{_tinl}{m1.FaseNaar}{m1.FaseVan}] = G[{_fcpf}{m1.FaseVan}];");
                        }
                    }

                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* correctie realisatietijd berekenen */");
                    sb.AppendLine($"{ts}while (wijziging)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}wijziging = FALSE;");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/* Gelijkstart / voorstart / late release */");
                    foreach (var model in oneWay)
                    {
                        var max = model.Waarde < 0 ? _treallr : _trealvs;
                        var dir = model.Waarde < 0 ? "TRUE" : "FALSE";
                        sb.AppendLine($"{ts}{ts}wijziging |= Corr_Min({_fcpf}{model.FaseVan}, {_fcpf}{model.FaseNaar}, T_max[{_tpf}{max}{model.FaseVan}{model.FaseNaar}], {dir});");
                    }
                    foreach (var (m1, m2, _) in twoWay)
                    {
                        if (m1.Waarde != 0 || m2.Waarde != 0) continue;
                        sb.AppendLine($"{ts}{ts}wijziging |= Corr_Min({_fcpf}{m1.FaseVan}, {_fcpf}{m1.FaseNaar}, 0, FALSE);");
                        sb.AppendLine($"{ts}{ts}wijziging |= Corr_Min({_fcpf}{m2.FaseVan}, {_fcpf}{m2.FaseNaar}, 0, FALSE);");
                    }
                    sb.AppendLine();
                    var first = true;
                    foreach (var (m1, _, gelijkstart) in twoWayPedestrians)
                    {
                        if (gelijkstart) continue;

                        if (first)
                        {
                            sb.AppendLine($"{ts}{ts}/* Inlopen */");
                            first = false;
                        }
                        sb.AppendLine($"{ts}{ts}wijziging |= VTG2_Real_Los({_fcpf}{m1.FaseVan}, {_fcpf}{m1.FaseNaar}, T_max[{_tpf}{_tinl}{m1.FaseVan}{m1.FaseNaar}], T_max[{_tpf}{_tinl}{m1.FaseNaar}{m1.FaseVan}], H[{_hpf}{_hinl}{m1.FaseVan}], H[{_hpf}{_hinl}{m1.FaseNaar}], H[{_hpf}{_hlos}{m1.FaseVan}], H[{_hpf}{_hlos}{m1.FaseNaar}]);");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/* Fictieve ontruiming */");
                    foreach (var fotModel in _groenSyncData.FictieveConflicten)
                    {
                        var ow = oneWay.FirstOrDefault(x => x.FaseVan == fotModel.FaseVan && x.FaseNaar == fotModel.FaseNaar);
                        var tw = twoWay.FirstOrDefault(x => x.m1.FaseVan == fotModel.FaseVan && x.m1.FaseNaar == fotModel.FaseNaar || x.m2.FaseVan == fotModel.FaseVan && x.m2.FaseNaar == fotModel.FaseNaar);
                        if (ow == null && tw.m1 == null || tw.m1 != null && (tw.m1.Waarde != 0 || tw.m2.Waarde != 0)) continue;
                        
                        var max = ow != null ? ow.Waarde < 0 ? _treallr : _trealvs : null;

                        sb.AppendLine($"{ts}{ts}wijziging |= Corr_FOT({_fcpf}{fotModel.FaseVan}, {_fcpf}{fotModel.FaseNaar}, {(max == null ? "0" : $"T_max[{_tpf}{max}{fotModel.FaseVan}{fotModel.FaseNaar}]")}, TGG_max[{_fcpf}{fotModel.FaseVan}]);");
                    }
                    sb.AppendLine($"{ts}}}");
                    
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Realisatie tijd naar geheugenelement */");
                    foreach (var model in _fasenMetSync)
                    {
                        sb.AppendLine($"{ts}Realisatietijd_MM({_fcpf}{model}, {_mpf}{_mrealtijd}{model});");
                    }
                    return sb.ToString();


                // TODO PARcorr, setMRLW

                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    sb.AppendLine($"{ts}/* Reset synchronisatie BITs */");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}RR[fc] &= ~(BIT1 | BIT2);");
                    sb.AppendLine($"{ts}{ts}RW[fc]&= ~(BIT1);");
                    sb.AppendLine($"{ts}{ts}YV[fc]&= ~(BIT1);");
                    sb.AppendLine($"{ts}{ts}YM[fc]&= ~(BIT3);");
                    sb.AppendLine($"{ts}{ts} X[fc]&= ~(BIT1|BIT2);");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Uitvoeren synchronisaties */");
                    sb.AppendLine($"{ts}Synchroniseer();");
                    return sb.ToString();
                default:
                    return null;
            }
        }

        private static List<GroenSyncModel[]> GetThreeWayPedestirans(List<(GroenSyncModel m1, GroenSyncModel m2, bool gelijkstart)> twoWayPedestrians)
        {
            var threeWayPedestrians = new List<GroenSyncModel[]>();
            foreach (var (fm1, fm2, _) in twoWayPedestrians)
            {
                if (threeWayPedestrians.Any(x => x.Any(x2 => x2.FaseVan == fm1.FaseVan && x2.FaseNaar == fm2.FaseNaar)))
                {
                    continue;
                }

                var (sm1, sm2, _) =
                    twoWayPedestrians.FirstOrDefault(x => x.m1.FaseVan == fm1.FaseVan && x.m1.FaseNaar != fm1.FaseNaar);
                var (tm1, tm2, _) =
                    twoWayPedestrians.FirstOrDefault(x => x.m1.FaseVan == fm2.FaseVan && x.m1.FaseNaar != fm2.FaseNaar);
                if (sm1 != null && tm1 != null)
                {
                    threeWayPedestrians.Add(new[] {fm1, fm2, sm1, sm2, tm1, tm2});
                }
            }

            return threeWayPedestrians;
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _hmad = CCOLGeneratorSettingsProvider.Default.GetElementName("hmad");
		    
            return base.SetSettings(settings);
        }
    }
}
