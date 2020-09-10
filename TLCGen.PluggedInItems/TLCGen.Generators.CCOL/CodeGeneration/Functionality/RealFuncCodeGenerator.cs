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
        private CCOLGeneratorCodeStringSettingModel _tfo;
        private CCOLGeneratorCodeStringSettingModel _treallr;
        private CCOLGeneratorCodeStringSettingModel _trealvs;
        private CCOLGeneratorCodeStringSettingModel _trealil;
        private CCOLGeneratorCodeStringSettingModel _hlos;
        private CCOLGeneratorCodeStringSettingModel _schlos;
        private CCOLGeneratorCodeStringSettingModel _mrealtijd;
#pragma warning restore 0649
        private string _hmad;
        private GroenSyncDataModel _groenSyncData;
        private List<string> _fasenMetSync;
        private (List<GroenSyncModel> oneWay, List<(GroenSyncModel m1, GroenSyncModel m2, bool gelijkstart)> twoWay,
            List<(GroenSyncModel m1, GroenSyncModel m2, bool gelijkstart)> twoWayPedestrians) _sortedSyncs;

        #endregion // Fields

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.RealFunc) return;

            _groenSyncData = GroenSyncDataModel.ConvertSyncFuncToRealFunc(c);
            _sortedSyncs = GroenSyncDataModel.OrderSyncs(c, _groenSyncData);
            _fasenMetSync = _groenSyncData.GroenSyncFasen.Select(x => x.FaseVan)
                .Concat(_groenSyncData.GroenSyncFasen.Select(x => x.FaseNaar)).Distinct()
                .ToList();
            _fasenMetSync.Sort();

            foreach (var grsync in _sortedSyncs.oneWay)
            {
                var max = grsync.Waarde <= 0 ? _trealvs : _treallr;
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{max}{grsync}",
                        grsync.Waarde < 0 ? grsync.Waarde * -1 : grsync.Waarde,
                        CCOLElementTimeTypeEnum.TE_type,
                        max, grsync.FaseVan, grsync.FaseNaar));
            }

            var helps = new List<string>();

            foreach (var (m1, m2, gelijkstart) in _sortedSyncs.twoWayPedestrians)
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

            foreach (var fc in c.Fasen)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_mrealtijd}{fc}",
                        _mrealtijd, fc.Naam));
            }

            foreach (var fot in _groenSyncData.FictieveConflicten)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tfo}{fot}",
                        fot.FictieveOntruimingsTijd,
                        CCOLElementTimeTypeEnum.TE_type,
                        _tfo, fot.FaseVan, fot.FaseNaar));
            }

        }

        public override bool HasCCOLElements() => true;

        public override bool HasFunctionLocalVariables() => true;
        
        public override IEnumerable<Tuple<string, string, string>> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCBepaalRealisatieTijden:
                    if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.RealFunc 
                        || c.InterSignaalGroep?.Gelijkstarten?.Count == 0 && c.InterSignaalGroep?.Voorstarten?.Count == 0 && c.InterSignaalGroep?.LateReleases?.Count == 0)
                        return base.GetFunctionLocalVariables(c, type);
                    return new List<Tuple<string, string, string>>
                    {
                        new Tuple<string, string, string>(c.GetBoolV(), "wijziging", "TRUE"),
                        new Tuple<string, string, string>("int", "i", ""),
                    };
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
            return type switch
            {
                CCOLCodeTypeEnum.RegCIncludes => 50,
                CCOLCodeTypeEnum.RegCBepaalRealisatieTijden => 10,
                CCOLCodeTypeEnum.RegCSynchronisaties => 30,
                CCOLCodeTypeEnum.RegCRealisatieAfhandelingVoorModules => 10,
                CCOLCodeTypeEnum.RegCMaxgroen => 70,
                CCOLCodeTypeEnum.RegCVerlenggroen => 70,
                _ => 0
            };
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
            var first = true;

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:
                    sb.AppendLine($"{ts}#include \"realfunc.c\"");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCBepaalRealisatieTijden:

                    // TODO test and check logic
                    var threeWayPedestrians = GetThreeWayPedestirans(_sortedSyncs.twoWayPedestrians);

                    // Two-way negative pedestrians
                    var startDuringRed = false;
                    foreach (var (grsync, _, gelijkstart) in _sortedSyncs.twoWayPedestrians)
                    {
                        if (gelijkstart) continue;

                        var fc1 = c.Fasen.FirstOrDefault(x => x.Naam == grsync.FaseVan);
                        var fc2 = c.Fasen.FirstOrDefault(x => x.Naam == grsync.FaseNaar);
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
                        sb.AppendLine($"{ts}Inlopen_Los2({_fcpf}{grsync:van}, {_fcpf}{grsync:naar}, {dr1A}, {dr1B}, {dr2B}, {dr2A}, {hdr1A}, {hdr1B}, {hdr2B}, {hdr2A}, {_hpf}{_hinl}{grsync:van}, {_hpf}{_hinl}{grsync.FaseNaar}, {_hpf}{_hlos}{grsync:van}, {_hpf}{_hlos}{grsync:van}, {_schpf}{_schlos}{grsync:van}_1, {_schpf}{_schlos}{grsync:van}_2, {_schpf}{_schlos}{grsync:naar}_1, {_schpf}{_schlos}{grsync:naar}_2);");
                        startDuringRed = true;
                    }

                    if (startDuringRed)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* Herstarten/afkappen inlooptijd */");
                        foreach (var (grsync, _, gelijkstart) in _sortedSyncs.twoWayPedestrians)
                        {
                            if (gelijkstart) continue;

                            sb.AppendLine($"{ts}RT[{_tpf}{_tinl}{grsync}] = SG[{_fcpf}{grsync:van}] && H[{_hpf}{_hinl}{grsync:van}]; AT[{_tpf}{_tinl}{grsync}] = G[{_fcpf}{grsync:naar}];");
                            sb.AppendLine($"{ts}RT[{_tpf}{_tinl}{grsync:naarvan}] = SG[{_fcpf}{grsync:naar}] && H[{_hpf}{_hinl}{grsync:naar}]; AT[{_tpf}{_tinl}{grsync:naarvan}] = G[{_fcpf}{grsync:van}];");
                        }
                    }

                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* correctie realisatietijd berekenen (max. 100 iteraties) */");
                    sb.AppendLine($"{ts}for (i = 0; i < 100; ++i)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}wijziging = FALSE;");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/* Gelijkstart / voorstart / late release */");
                    foreach (var grsync in _sortedSyncs.oneWay)
                    {
                        var max = grsync.Waarde <= 0 ? _trealvs : _treallr;
                        // voorstart
                        if (grsync.Waarde <= 0)
                        {
                            sb.AppendLine($"{ts}{ts}wijziging |= Corr_Pls({_fcpf}{grsync:van}, {_fcpf}{grsync:naar}, T_max[{_tpf}{max}{grsync}], TRUE);");
                        }
                        // late release of inlopen
                        else
                        {
                            // bij voetgangers waarbij éénzijdig ingelopen mag worden moet de correctie omgekeerd: de synchronisatie (naloop) is van a naar b,
                            // maar de groentijdcorrectie van b naar a
                            var fc1 = c.Fasen.FirstOrDefault(x => x.Naam == grsync.FaseVan);
                            var fc2 = c.Fasen.FirstOrDefault(x => x.Naam == grsync.FaseNaar);
                            if (fc1?.Type == FaseTypeEnum.Voetganger && fc2?.Type == FaseTypeEnum.Voetganger)
                            {
                                max = _trealil;
                                sb.AppendLine($"{ts}{ts}wijziging |= Corr_Min({_fcpf}{grsync:naar}, {_fcpf}{grsync:van}, T_max[{_tpf}{max}{grsync}], TRUE);");
                            }
                            else
                            {
                                sb.AppendLine($"{ts}{ts}wijziging |= Corr_Min({_fcpf}{grsync:van}, {_fcpf}{grsync:naar}, T_max[{_tpf}{max}{grsync}], TRUE);");
                            }
                        }
                    }
                    foreach (var (grsync1, grsync2, _) in _sortedSyncs.twoWay)
                    {
                        if (grsync1.Waarde != 0 || grsync2.Waarde != 0) continue;
                        sb.AppendLine($"{ts}{ts}wijziging |= Corr_Gel({_fcpf}{grsync1:van}, {_fcpf}{grsync1:naar}, TRUE);");
                    }
                    sb.AppendLine();
                    foreach (var (grsync, _, gelijkstart) in _sortedSyncs.twoWayPedestrians)
                    {
                        if (gelijkstart) continue;

                        if (first)
                        {
                            sb.AppendLine($"{ts}{ts}/* Inlopen */");
                            first = false;
                        }
                        sb.AppendLine($"{ts}{ts}wijziging |= VTG2_Real_Los({_fcpf}{grsync:van}, {_fcpf}{grsync:naar}, T_max[{_tpf}{_tinl}{grsync}], T_max[{_tpf}{_tinl}{grsync:naarvan}], H[{_hpf}{_hinl}{grsync:van}], H[{_hpf}{_hinl}{grsync:naar}], H[{_hpf}{_hlos}{grsync:van}], H[{_hpf}{_hlos}{grsync:naar}], TRUE);");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/* Fictieve ontruiming */");
                    foreach (var fot in _groenSyncData.FictieveConflicten)
                    {
                        var ow = _sortedSyncs.oneWay.FirstOrDefault(x => x.FaseVan == fot.FaseNaar && x.FaseNaar == fot.FaseVan);
                        var (m1, m2, _) = _sortedSyncs.twoWay.FirstOrDefault(x => x.m1.FaseVan == fot.FaseVan && x.m1.FaseNaar == fot.FaseNaar || x.m2.FaseVan == fot.FaseVan && x.m2.FaseNaar == fot.FaseNaar);
                        if (ow == null && m1 == null || m1 != null && (m1.Waarde != 0 || m2.Waarde != 0)) continue;

                        var lr = ow != null && ow.Waarde > 0;
                        sb.AppendLine($"{ts}{ts}wijziging |= Corr_FOT({_fcpf}{fot:naar}, {_fcpf}{fot:van}, T_max[{_tpf}{_tfo}{fot}], {(lr ? $"T_max[{_tpf}{_treallr}{fot:naarvan}]" : "0")});");
                    }
                    if (_groenSyncData.FictieveConflicten.Count > 0) sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}if (!wijziging) break;");
                    sb.AppendLine($"{ts}}}");
                    
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Realisatie tijd naar geheugenelement */");
                    foreach (var fc in c.Fasen)
                    {
                        sb.AppendLine($"{ts}Realisatietijd_MM({_fcpf}{fc}, {_mpf}{_mrealtijd}{fc});");
                    }
                    return sb.ToString();

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
                    if (c.HalfstarData.IsHalfstar)
                        sb.AppendLine($"{ts}Synchroniseer_SP(); /* synchronisatie intrekken tbv SP */");
                    sb.AppendLine($"{ts}Synchroniseer_SG(); /* synchronisatie obv realtijd (startgroenmomenten) */");
                    sb.AppendLine($"{ts}Synchroniseer_FO(); /* synchronisatie obv fictieve ontruiming */");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCRealisatieAfhandelingVoorModules:
                    sb.AppendLine($"{ts}Synchroniseer_PG(); /* synchronisatie PG's */");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCMaxgroen:
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                    foreach (var (m1, m2, gelijkstart) in _sortedSyncs.twoWayPedestrians)
                    {
                        if (gelijkstart) continue;
                        
                        if (first)
                        {
                            sb.AppendLine($"{ts}/* Bij inlopen, inlopende richting in WG houden t.b.v. eventuele aanvraag naloop in tegenrichting */");
                            first = false;
                        }
                        sb.AppendLine($"{ts}RW[{_fcpf}{m1.FaseVan}] |= T[{_tpf}{_tinl}{m1.FaseVan}{m1.FaseNaar}] ? BIT2 : 0;");
                        sb.AppendLine($"{ts}RW[{_fcpf}{m2.FaseVan}] |= T[{_tpf}{_tinl}{m2.FaseVan}{m2.FaseNaar}] ? BIT2 : 0;");
                    }
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
