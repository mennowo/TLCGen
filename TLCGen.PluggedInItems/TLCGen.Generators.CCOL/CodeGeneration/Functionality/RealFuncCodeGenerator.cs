﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
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
        private CCOLGeneratorCodeStringSettingModel _tfo;
        private CCOLGeneratorCodeStringSettingModel _treallr;
        private CCOLGeneratorCodeStringSettingModel _trealvs;
        private CCOLGeneratorCodeStringSettingModel _trealil;
        private CCOLGeneratorCodeStringSettingModel _hlos;
        private CCOLGeneratorCodeStringSettingModel _schlos;
        private CCOLGeneratorCodeStringSettingModel _schrealgs;
        private CCOLGeneratorCodeStringSettingModel _mar;
        private CCOLGeneratorCodeStringSettingModel _mrealtijd;
        private CCOLGeneratorCodeStringSettingModel _mrealtijdmin;
        private CCOLGeneratorCodeStringSettingModel _mrealtijdmax;
#pragma warning restore 0649
        private string _hmad;
        private string _hplact;
        private string _cvchd;
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
                var max = grsync.Richting == -1 ? _trealvs : _treallr;
                var grsyncElemName = grsync.ToString();
                // inlopen voetgangers (eenzijdig)
                var fc1 = c.Fasen.FirstOrDefault(x => x.Naam == grsync.FaseVan);
                var fc2 = c.Fasen.FirstOrDefault(x => x.Naam == grsync.FaseNaar);
                bool vtgEenz = false;
                if (fc1?.Type == FaseTypeEnum.Voetganger && fc2?.Type == FaseTypeEnum.Voetganger)
                {
                    max = _trealil;
                    grsyncElemName = grsync.FaseNaar + grsync.FaseVan;
                    vtgEenz = true;
                }
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{max}{grsyncElemName}",
                        grsync.Waarde < 0 ? grsync.Waarde * -1 : grsync.Waarde,
                        CCOLElementTimeTypeEnum.TE_type,
                        max, vtgEenz ? grsync.FaseNaar : grsync.FaseVan, vtgEenz ? grsync.FaseVan : grsync.FaseNaar));
            }

            var helps = new List<string>();

            foreach (var (gs1, _, _) in _sortedSyncs.twoWay)
            {
                if (gs1.AanUit != AltijdAanUitEnum.Altijd)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_schrealgs}{gs1}",
                            gs1.AanUit == AltijdAanUitEnum.SchAan ? 1 : 0,
                            CCOLElementTimeTypeEnum.SCH_type,
                            _schrealgs, gs1.FaseNaar, gs1.FaseVan));
                }
            }

            foreach (var (m1, m2, gelijkstart) in _sortedSyncs.twoWayPedestrians)
            {
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
                        $"{_trealil}{m1.FaseVan}{m1.FaseNaar}",
                        m2.Waarde < 0 ? m2.Waarde * -1 : m2.Waarde,
                        CCOLElementTimeTypeEnum.TE_type,
                        _trealil, m1.FaseVan, m1.FaseNaar));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_trealil}{m2.FaseVan}{m2.FaseNaar}",
                        m1.Waarde < 0 ? m1.Waarde * -1 : m1.Waarde,
                        CCOLElementTimeTypeEnum.TE_type,
                        _trealil, m2.FaseVan, m2.FaseNaar));

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

            if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc &&
                (c.Data.RealFuncBepaalRealisatieTijdenAltijd
                 || c.TimingsData.TimingsUsePredictions
                 || c.InterSignaalGroep?.Gelijkstarten?.Count > 0
                 || c.InterSignaalGroep?.Voorstarten?.Count > 0
                 || c.InterSignaalGroep?.LateReleases?.Count > 0))
            {
                foreach (var fc in c.Fasen)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_mrealtijd}{fc}",
                            _mrealtijd, fc.Naam));
                }

                if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                {
                    foreach (var fc in c.Fasen)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_mrealtijdmin}{fc}",
                                _mrealtijdmin, fc.Naam));
                    }

                    foreach (var fc in c.Fasen)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_mrealtijdmax}{fc}",
                                _mrealtijdmax, fc.Naam));
                    }
                }
            }
            
            if (_sortedSyncs.oneWay.Any())
            {
                foreach (var grsync in _sortedSyncs.oneWay)
                {
                    var fc1 = c.Fasen.FirstOrDefault(x => x.Naam == grsync.FaseVan);
                    var fc2 = c.Fasen.FirstOrDefault(x => x.Naam == grsync.FaseNaar);

                    if (fc1 == null || fc2 == null ||
                        fc1.Type == FaseTypeEnum.Voetganger && fc2.Type == FaseTypeEnum.Voetganger ||
                        c.InterSignaalGroep != null &&
                        !c.InterSignaalGroep.Nalopen.Any(x => fc1.Naam == x.FaseNaar && fc2.Naam == x.FaseVan))
                    {
                        continue;
                    }

                    if (!helps.Contains($"h{_hlos}{grsync.FaseVan}"))
                    {
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hlos}{grsync.FaseVan}", _hlos, grsync.FaseVan));
                        helps.Add($"h{_hlos}{grsync.FaseVan}");
                    }
                }
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
            
            foreach (var fc in c.Fasen)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_mar}{fc.Naam}", _mar, fc.Naam));
            }

        }

        public override bool HasCCOLElements() => true;

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCBepaalRealisatieTijden:
                    if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.RealFunc)
                        return base.GetFunctionLocalVariables(c, type);
                    return new List<CCOLLocalVariable>
                    {
                        new(c.GetBoolV(), "wijziging", "TRUE"),
                        new("int", "i"),
                    };
                case CCOLCodeTypeEnum.RegCAlternatieven:
                    if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.RealFunc)
                        return base.GetFunctionLocalVariables(c, type);
                    return new List<CCOLLocalVariable> { new("int", "fc") };
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.RealFunc 
                        || 
                        c.InterSignaalGroep?.Gelijkstarten?.Count == 0 
                        && c.InterSignaalGroep?.Voorstarten?.Count == 0 
                        && c.InterSignaalGroep?.LateReleases?.Count == 0
                        && c.InterSignaalGroep?.Nalopen?.Count == 0)
                        return base.GetFunctionLocalVariables(c, type);
                    return new List<CCOLLocalVariable> { new("int", "fc") };
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }
        
        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCBepaalRealisatieTijden => new []{20},
                CCOLCodeTypeEnum.RegCSynchronisaties => new []{30},
                CCOLCodeTypeEnum.RegCRealisatieAfhandelingVoorModules => new []{10},
                CCOLCodeTypeEnum.RegCMaxgroen => new []{70},
                CCOLCodeTypeEnum.RegCVerlenggroen => new []{70},
                CCOLCodeTypeEnum.RegCAlternatieven => new []{90},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            // return if no sync
            if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.RealFunc ||
                !c.Data.RealFuncBepaalRealisatieTijdenAltijd &&
                !c.TimingsData.TimingsUsePredictions &&
                c.InterSignaalGroep?.Gelijkstarten?.Count == 0
                && c.InterSignaalGroep?.Voorstarten?.Count == 0
                && c.InterSignaalGroep?.LateReleases?.Count == 0)
                return null;

            var sb = new StringBuilder();

            string firstFcName = null;
            if (c.Data.RangeerData.RangerenFasen)
            {
                firstFcName = c.Data.RangeerData.RangeerFasen.OrderBy(x => x.RangeerIndex).First()?.Naam;
            }
            firstFcName ??= c.Fasen.First().Naam;
            
            var first = true;

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAlternatieven:
                    var arTypes = new HashSet<AlternatieveRuimteTypeEnum>();
                    foreach (var f in c.Fasen)
                    {
                        arTypes.Add(f.AlternatieveRuimteType);
                    }

                    if (arTypes.All(x => x == AlternatieveRuimteTypeEnum.RealRuimte)) return "";
                    
                    sb.AppendLine($"{ts}/* Alternatieve ruimte in memory element schrijven */");
                    var maxtartotig = c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && c.Data.Intergroen ? "max_tar_tig" : "max_tar_to";
                    
                    
                    if (arTypes.Count == 1 && arTypes.First() != AlternatieveRuimteTypeEnum.RealRuimte)
                    {
                        var tts = ts;
                        if (c.HalfstarData.IsHalfstar)
                        {
                            sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}])");
                            sb.AppendLine($"{ts}{{");
                            if (c.Data.RangeerData.RangerenFasen)
                            {
                                foreach (var fc in c.Data.RangeerData.RangeerFasen.OrderBy(x => x.RangeerIndex))
                                {
                                    sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mar}{fc.Naam}] = tar_max_ple({_fcpf}{fc.Naam});");
                                }
                            }
                            else
                            {
                                sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc) MM[{_mpf}{_mar}{firstFcName} + fc] = tar_max_ple(fc);");
                            }
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine($"{ts}else");
                            sb.AppendLine($"{ts}{{");
                            tts += ts;
                        }

                        switch (arTypes.First())
                        {
                            case AlternatieveRuimteTypeEnum.MaxTarToTig:
                                if (c.Data.RangeerData.RangerenFasen)
                                {
                                    foreach (var fc in c.Data.RangeerData.RangeerFasen.OrderBy(x => x.RangeerIndex))
                                    {
                                        sb.AppendLine($"{tts}MM[{_mpf}{_mar}{fc.Naam}] = {maxtartotig}({_fcpf}{fc.Naam});");
                                    }
                                }
                                else
                                {
                                    sb.AppendLine($"{tts}for (fc = 0; fc < FCMAX; ++fc) MM[{_mpf}{_mar}{firstFcName} + fc] = {maxtartotig}(fc);");
                                }
                                break;
                            case AlternatieveRuimteTypeEnum.MaxTar:
                                if (c.Data.RangeerData.RangerenFasen)
                                {
                                    foreach (var fc in c.Data.RangeerData.RangeerFasen.OrderBy(x => x.RangeerIndex))
                                    {
                                        sb.AppendLine($"{tts}MM[{_mpf}{_mar}{fc.Naam}] = max_tar({_fcpf}{fc.Naam});");
                                    }
                                }
                                else
                                {
                                    sb.AppendLine($"{tts}for (fc = 0; fc < FCMAX; ++fc) MM[{_mpf}{_mar}{firstFcName} + fc] = max_tar(fc);");
                                }
                                break;
                            case AlternatieveRuimteTypeEnum.RealRuimte:
                                // niets nodig, is reeds gebeurt
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        
                        if (c.HalfstarData.IsHalfstar)
                        {
                            sb.AppendLine($"{ts}}}");
                        }
                    }
                    else if (arTypes.Any(x => x != AlternatieveRuimteTypeEnum.RealRuimte))
                    {
                        var tts = ts;
                        if (c.HalfstarData.IsHalfstar)
                        {
                            sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}])");
                            sb.AppendLine($"{ts}{{");
                            if (c.Data.RangeerData.RangerenFasen)
                            {
                                foreach (var fc in c.Data.RangeerData.RangeerFasen.OrderBy(x => x.RangeerIndex))
                                {
                                    sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mar}{fc.Naam}] = tar_max_ple({_fcpf}{fc.Naam});");
                                }
                            }
                            else
                            {
                                sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc) MM[{_mpf}{_mar}{firstFcName} + fc] = tar_max_ple(fc);");
                            }
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine($"{ts}else");
                            sb.AppendLine($"{ts}{{");
                        }

                        foreach (var f in c.Fasen)
                        {
                            switch (f.AlternatieveRuimteType)
                            {
                                case AlternatieveRuimteTypeEnum.MaxTarToTig:
                                    sb.AppendLine($"{tts}MM[{_mpf}{_mar}{f.Naam}] = {maxtartotig}({_fcpf}{f.Naam});");
                                    break;
                                case AlternatieveRuimteTypeEnum.MaxTar:
                                    sb.AppendLine($"{tts}MM[{_mpf}{_mar}{f.Naam}] = max_tar({_fcpf}{f.Naam});");
                                    break;
                                case AlternatieveRuimteTypeEnum.RealRuimte:
                                    // niets nodig, is reeds gebeurt
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        if (c.HalfstarData.IsHalfstar)
                        {
                            sb.AppendLine($"{ts}}}");
                        }
                    }

                    return sb.ToString();
                    
                case CCOLCodeTypeEnum.RegCBepaalRealisatieTijden:

                    sb.AppendLine($"{ts}/* Bepalen realisatietijden */");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Reset */");
                    sb.AppendLine($"{ts}for (i = 0; i < FCMAX; ++i)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}Realisatietijd(i, NG, NG);");
                    sb.AppendLine($"{ts}{ts}Realisatietijd_min(i, NG, NG);");
                    sb.AppendLine($"{ts}{ts}REALTIJD_max[i] = REALTIJD_uncorrected[i];");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();

                    // TODO test and check logic
                    //var threeWayPedestrians = GetThreeWayPedestirans(_sortedSyncs.twoWayPedestrians);

                    // Two-way negative pedestrians
                    var startDuringRed = false;
                    if (_sortedSyncs.twoWayPedestrians.Any(x => !x.gelijkstart))
                    {
                        sb.AppendLine($"{ts}/* Inlopen voetgangers */");
                        foreach (var (grsync, _, gelijkstart) in _sortedSyncs.twoWayPedestrians)
                        {
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
                            sb.AppendLine($"{ts}Inlopen_Los2({_fcpf}{grsync:van}, {_fcpf}{grsync:naar}, {hdr1A}, {hdr1B}, {hdr2B}, {hdr2A}, {_hpf}{_hinl}{grsync:van}, {_hpf}{_hinl}{grsync.FaseNaar}, {_hpf}{_hlos}{grsync:van}, {_hpf}{_hlos}{grsync:naar}, SCH[{_schpf}{_schlos}{grsync:van}_1], SCH[{_schpf}{_schlos}{grsync:van}_2], SCH[{_schpf}{_schlos}{grsync:naar}_1], SCH[{_schpf}{_schlos}{grsync:naar}_2]);");
                            startDuringRed = true;
                        }
                        sb.AppendLine();
                    }

                    if (startDuringRed && 
                        _sortedSyncs.twoWayPedestrians.Any(x => !x.gelijkstart) ||
                        _sortedSyncs.oneWay.Any(x => x.Richting == 1))
                    {
                        sb.AppendLine($"{ts}/* Herstarten/afkappen inlooptijd/inrijtijd */");
                        foreach (var (grsync, _, gelijkstart) in _sortedSyncs.twoWayPedestrians)
                        {
                            sb.AppendLine($"{ts}RT[{_tpf}{_trealil}{grsync}] = SG[{_fcpf}{grsync:van}] && H[{_hpf}{_hinl}{grsync:van}]; AT[{_tpf}{_trealil}{grsync}] = G[{_fcpf}{grsync:naar}];");
                            sb.AppendLine($"{ts}RT[{_tpf}{_trealil}{grsync:naarvan}] = SG[{_fcpf}{grsync:naar}] && H[{_hpf}{_hinl}{grsync:naar}]; AT[{_tpf}{_trealil}{grsync:naarvan}] = G[{_fcpf}{grsync:van}];");
                        }

                        foreach (var grsync in _sortedSyncs.oneWay)
                        {
                            if (grsync.Richting == 1)
                            {
                                var fc1 = c.Fasen.FirstOrDefault(x => grsync.FaseVan == x.Naam);
                                var fc2 = c.Fasen.FirstOrDefault(x => grsync.FaseNaar == x.Naam);
                                var eenzVtg = false;
                                if (fc1.Type == FaseTypeEnum.Voetganger && fc2.Type == FaseTypeEnum.Voetganger)
                                {
                                    var nl2 = c.InterSignaalGroep.Nalopen
                                                .FirstOrDefault(x =>
                                                    x.Type == NaloopTypeEnum.StartGroen &&
                                                    x.FaseVan == grsync.FaseVan &&
                                                    x.FaseNaar == grsync.FaseNaar);
                                    eenzVtg = nl2 == null;
                                }
                                if (eenzVtg)
                                {
                                    sb.AppendLine($"{ts}RT[{_tpf}{_trealil}{grsync:naarvan}] = SG[{_fcpf}{grsync:naar}]; AT[{_tpf}{_trealil}{grsync:naarvan}] = G[{_fcpf}{grsync:van}];");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}RT[{_tpf}{_treallr}{grsync}] = SG[{_fcpf}{grsync:naar}]; AT[{_tpf}{_treallr}{grsync}] = G[{_fcpf}{grsync:van}];");
                                }
                            }
                        }
                        
                        sb.AppendLine();
                    }

                    sb.AppendLine($"{ts}/* correctie realisatietijd berekenen (max. 100 iteraties) */");
                    sb.AppendLine($"{ts}for (i = 0; i < 100; ++i)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}wijziging = FALSE;");
                    sb.AppendLine();

                    if (_sortedSyncs.oneWay.Any() || _sortedSyncs.twoWay.Any() || _groenSyncData.FictieveConflicten.Any())
                    {
                        sb.AppendLine($"{ts}{ts}/* Gelijkstart / voorstart / late release */");
                    }

                    if (_sortedSyncs.oneWay.Any())
                    {
                        foreach (var grsync in _sortedSyncs.oneWay)
                        {
                            var fc1 = c.Fasen.FirstOrDefault(x => grsync.FaseVan == x.Naam);
                            var fc2 = c.Fasen.FirstOrDefault(x => grsync.FaseNaar == x.Naam);
                            var max = grsync.Richting == -1 ? _trealvs : _treallr;
                            // voorstart
                            if (grsync.Richting == -1)
                            {
                                var condition = "TRUE";
                                if (c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen)
                                {
                                    var hd = c.PrioData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == grsync.FaseNaar);
                                    if (c.PrioData.BlokkeerNietConflictenBijHDIngreep &&
                                        (!c.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer ||
                                         fc1.Type == FaseTypeEnum.Fiets ||
                                         fc1.Type == FaseTypeEnum.Voetganger ||
                                         fc2.Type == FaseTypeEnum.Fiets ||
                                         fc2.Type == FaseTypeEnum.Voetganger) &&
                                        hd != null)
                                    {
                                        condition = $"!C[{_ctpf}{_cvchd}{hd.FaseCyclus}]";
                                    }
                                }
                                sb.AppendLine($"{ts}{ts}wijziging |= Corr_Pls({_fcpf}{grsync:van}, {_fcpf}{grsync:naar}, T_max[{_tpf}{max}{grsync}], {condition});");
                            }
                            // late release of inlopen
                            else
                            {
                                if (fc1?.Type == FaseTypeEnum.Voetganger && fc2?.Type == FaseTypeEnum.Voetganger)
                                {
                                    max = _trealil;
                                }
                                
                                var hd = c.PrioData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == grsync.FaseNaar);
                                var condition = "TRUE";
                                if (c.PrioData.BlokkeerNietConflictenBijHDIngreep &&
                                    (!c.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer ||
                                     fc1.Type == FaseTypeEnum.Fiets ||
                                     fc1.Type == FaseTypeEnum.Voetganger ||
                                     fc2.Type == FaseTypeEnum.Fiets ||
                                     fc2.Type == FaseTypeEnum.Voetganger) && 
                                    hd != null)
                                {
                                    condition = $"!C[{_ctpf}{_cvchd}{hd.FaseCyclus}]";
                                }

                                var nl = c.InterSignaalGroep.Nalopen
                                    .FirstOrDefault(x => 
                                        x.Type == NaloopTypeEnum.EindeGroen && 
                                        x.FaseVan == grsync.FaseNaar &&
                                        x.FaseNaar == grsync.FaseVan);
                                
                                if (nl == null)
                                {
                                    var nl2 = c.InterSignaalGroep.Nalopen
                                        .FirstOrDefault(x =>
                                            x.Type == NaloopTypeEnum.StartGroen &&
                                            x.FaseVan == grsync.FaseVan &&
                                            x.FaseNaar == grsync.FaseNaar);
                                    var eenzVtg = nl?.Type == NaloopTypeEnum.StartGroen && nl2 == null;
                                    if (eenzVtg)
                                    {
                                        sb.AppendLine($"{ts}{ts}wijziging |= Corr_Min({_fcpf}{grsync:naar}, {_fcpf}{grsync:van}, T_max[{_tpf}{max}{grsync:naarvan}], {condition});");
                                    }
                                    else
                                    {
                                        sb.AppendLine($"{ts}{ts}wijziging |= Corr_Min({_fcpf}{grsync:van}, {_fcpf}{grsync:naar}, T_max[{_tpf}{max}{grsync}], {condition});");
                                    }
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}{ts}wijziging |= Corr_Min_nl({_fcpf}{grsync:van}, {_fcpf}{grsync:naar}, T_max[{_tpf}{max}{grsync}], TRUE);");
                                }
                            }
                        }
                    }

                    if (_sortedSyncs.twoWay.Any())
                    {
                        foreach (var (grsync1, grsync2, gs) in _sortedSyncs.twoWay)
                        {
                            if (grsync1.Waarde != 0 || grsync2.Waarde != 0) continue;
                            if (gs && grsync1.AanUit != AltijdAanUitEnum.Altijd)
                            {
                                sb.Append($"{ts}{ts}if (SCH[{_schpf}{_schrealgs}{grsync1}]) ");
                            }
                            else sb.Append($"{ts}{ts}");

                            var fc1 = c.Fasen.FirstOrDefault(x => grsync1.FaseVan == x.Naam);
                            var fc2 = c.Fasen.FirstOrDefault(x => grsync1.FaseNaar == x.Naam);

                            var condition = "TRUE";
                            if (c.PrioData.BlokkeerNietConflictenBijHDIngreep &&
                                (!c.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer ||
                                 fc1.Type == FaseTypeEnum.Fiets ||
                                 fc1.Type == FaseTypeEnum.Voetganger ||
                                 fc2.Type == FaseTypeEnum.Fiets ||
                                 fc2.Type == FaseTypeEnum.Voetganger))
                            {
                                var hd1 = c.PrioData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == grsync1.FaseVan);
                                if (hd1 != null) condition = $"!C[{_ctpf}{_cvchd}{hd1.FaseCyclus}]";
                                var hd2 = c.PrioData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == grsync1.FaseNaar);
                                if (hd2 != null)
                                    condition = condition == "TRUE"
                                        ? $"!C[{_ctpf}{_cvchd}{hd2.FaseCyclus}]"
                                        : condition + $" && !C[{_ctpf}{_cvchd}{hd2.FaseCyclus}]";
                            }

                            sb.AppendLine($"wijziging |= Corr_Gel({_fcpf}{grsync1:van}, {_fcpf}{grsync1:naar}, {condition});");
                        }
                        sb.AppendLine();
                    }

                    if (_sortedSyncs.twoWayPedestrians.Any())
                    {
                        foreach (var (grsync, _, gelijkstart) in _sortedSyncs.twoWayPedestrians)
                        {
                            if (first)
                            {
                                sb.AppendLine($"{ts}{ts}/* Inlopen */");
                                first = false;
                            }

                            sb.AppendLine($"{ts}{ts}wijziging |= VTG2_Real_Los({_fcpf}{grsync:van}, {_fcpf}{grsync:naar}, T_max[{_tpf}{_trealil}{grsync}], T_max[{_tpf}{_trealil}{grsync:naarvan}], {_hpf}{_hinl}{grsync:van}, {_hpf}{_hinl}{grsync:naar}, {_hpf}{_hlos}{grsync:van}, {_hpf}{_hlos}{grsync:naar}, " +
                                          $"{(grsync.AanUit != AltijdAanUitEnum.Altijd ? $"SCH[{_schpf}{_schrealgs}{grsync}]" : "FALSE")});");
                        }
                        sb.AppendLine();
                    }

                    if (_sortedSyncs.oneWay.Any())
                    {
                        first = true;
                        foreach (var grsync in _sortedSyncs.oneWay)
                        {
                            var nl = c.InterSignaalGroep.Nalopen.FirstOrDefault(x => x.Type == NaloopTypeEnum.EindeGroen && x.FaseVan == grsync.FaseNaar);
                            if (nl != null) continue;
                            
                            var fc1 = c.Fasen.FirstOrDefault(x => x.Naam == grsync.FaseVan);
                            var fc2 = c.Fasen.FirstOrDefault(x => x.Naam == grsync.FaseNaar);

                            if (fc1 == null || fc2 == null ||
                                fc1.Type == FaseTypeEnum.Voetganger && fc2.Type == FaseTypeEnum.Voetganger ||
                                c.InterSignaalGroep != null &&
                                !c.InterSignaalGroep.Nalopen.Any(x => fc1.Naam == x.FaseNaar && fc2.Naam == x.FaseVan))
                            {
                                continue;
                            }
                            
                            if (first)
                            {
                                sb.AppendLine($"{ts}{ts}/* Inrijden */");
                                first = false;
                            }

                            sb.AppendLine($"{ts}{ts}wijziging |= Real_Los({_fcpf}{grsync:naar}, {_fcpf}{grsync:van}, 0, {_hpf}{_hlos}{grsync:van}, FALSE);");
                        }
                    }

                    if (_groenSyncData.FictieveConflicten.Any())
                    {
                        sb.AppendLine($"{ts}{ts}/* Fictieve ontruiming */");
                        foreach (var fot in _groenSyncData.FictieveConflicten)
                        {
                            var ow = _sortedSyncs.oneWay.FirstOrDefault(x => x.FaseVan == fot.FaseNaar && x.FaseNaar == fot.FaseVan);
                            var (m1, m2, gs) = _sortedSyncs.twoWay.FirstOrDefault(x => x.m1.FaseVan == fot.FaseVan && x.m1.FaseNaar == fot.FaseNaar || x.m2.FaseVan == fot.FaseVan && x.m2.FaseNaar == fot.FaseNaar);
                            if (ow == null && m1 == null || m1 != null && (m1.Waarde != 0 || m2.Waarde != 0)) continue;

                            var max = ow?.Richting == -1 ? _trealvs : _treallr;
                            
                            var lr = ow is {Waarde: >= 0};
                            if (gs && m1.AanUit != AltijdAanUitEnum.Altijd)
                            {
                                sb.Append($"{ts}{ts}if (SCH[{_schpf}{_schrealgs}{m1}]) ");
                            }
                            else sb.Append($"{ts}{ts}");

                            sb.AppendLine($"wijziging |= Corr_FOT({_fcpf}{fot:naar}, {_fcpf}{fot:van}, {_tpf}{_tfo}{fot}, {(lr ? $"T_max[{_tpf}{max}{fot:naarvan}]" : "0")}, TRUE);");
                        }

                        if (_groenSyncData.FictieveConflicten.Count > 0) sb.AppendLine();
                    }

                    // if (c.TimingsData.TimingsToepassen && c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                    // {
                    //     var syncGroups = CCOLCodeHelper.GetSyncGroupsForController(c);
                    //     
                    //     sb.AppendLine($"{ts}#ifndef NO_TIMETOX");
                    //     sb.AppendLine($"{ts}{ts}/* Hoogste realtijd_max voor alle richtingen uit een synchronisatiegroep */");
                    //     foreach (var sg in syncGroups)
                    //     {
                    //         sb.AppendLine($"{ts}{ts}/* Syncgroep: {string.Join(";", sg)} */");
                    //         for (var i = 1; i < sg.Count; i++)
                    //         {
                    //             sb.AppendLine($"{ts}{ts}REALTIJD_max[{_fcpf}{sg[0]}] = max(REALTIJD_max[{_fcpf}{sg[0]}], REALTIJD_max[{_fcpf}{sg[i]}]);");
                    //         }
                    //
                    //         sb.Append($"{ts}{ts}");
                    //         for (var i = 1; i < sg.Count; i++)
                    //         {
                    //             sb.Append($"REALTIJD_max[{_fcpf}{sg[i]}] = ");
                    //         }
                    //         sb.AppendLine($"REALTIJD_max[{_fcpf}{sg[0]}];");
                    //         sb.AppendLine();
                    //     }
                    //     sb.AppendLine($"{ts}#endif");
                    // }

                    sb.AppendLine($"{ts}{ts}wijziging |= CorrectieRealisatieTijd_Add();");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}if (!wijziging) break;");
                    sb.AppendLine($"{ts}}}");
                    
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Realisatie tijd naar geheugenelement */");
                    if (c.Data.RangeerData.RangerenFasen)
                    {
                        foreach (var fc in c.Data.RangeerData.RangeerFasen.OrderBy(x => x.RangeerIndex))
                        {
                            sb.AppendLine($"{ts}Realisatietijd_MM({_fcpf}{fc.Naam}, {_mpf}{_mrealtijd}{fc.Naam});");
                            if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                            {
                                sb.AppendLine($"{ts}MM[{_mpf}{_mrealtijdmin}{fc.Naam}] = REALTIJD_min[{_fcpf}{fc.Naam}];");
                                sb.AppendLine($"{ts}MM[{_mpf}{_mrealtijdmax}{fc.Naam}] = REALTIJD_max[{_fcpf}{fc.Naam}];");
                            }
                        }
                    }
                    else
                    {
                        sb.AppendLine($"{ts}for (i = 0; i < FCMAX; ++i)");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}Realisatietijd_MM({_fcpf}{firstFcName} + i, {_mpf}{_mrealtijd}{firstFcName} + i);");
                        if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                        {
                            sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mrealtijdmin}{firstFcName} + i] = REALTIJD_min[{_fcpf}{firstFcName} + i];");
                            sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mrealtijdmax}{firstFcName} + i] = REALTIJD_max[{_fcpf}{firstFcName} + i];");
                        }

                        sb.AppendLine($"{ts}}}");
                    }

                    sb.AppendLine($"{ts}#if (!defined (AUTOMAAT) && !defined AUTOMAAT_TEST || defined (VISSIM)) && !defined NO_PRINT_REALTIJD");
                    sb.AppendLine($"{ts}if (display) {{");
                    if (c.Data.RangeerData.RangerenFasen)
                    {
                        sb.AppendLine($"{ts}{ts}int i = 0;");
                    }
                    sb.AppendLine($"{ts}{ts}xyprintf(92, 6, \"REALtijden  min  max \");");
                    if (c.Data.RangeerData.RangerenFasen)
                    {
                        foreach (var fc in c.Data.RangeerData.RangeerFasen.OrderBy(x => x.RangeerIndex))
                        {
                            sb.AppendLine($"{ts}{ts}xyprintf( 92, 7 + i, \"fc%3s %4d\", FC_code[{_fcpf}{fc.Naam}], MM[{_mpf}{_mrealtijd}{fc.Naam}]);");
                            if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                            {
                                sb.AppendLine($"{ts}{ts}xyprintf(103, 7 + i, \"%4d\",                   MM[{_mpf}{_mrealtijdmin}{fc.Naam}]);");
                                sb.AppendLine($"{ts}{ts}xyprintf(108, 7 + i++, \"%4d\",                 MM[{_mpf}{_mrealtijdmax}{fc.Naam}]);");
                            }
                        }
                    }
                    else
                    {
                        sb.AppendLine($"{ts}{ts}for (i = 0; i < FC_MAX; ++i)");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}xyprintf( 92, 7 + i, \"fc%3s %4d\", FC_code[i], MM[{_mpf}{_mrealtijd}{firstFcName} + i]);");
                        if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                        {
                            sb.AppendLine($"{ts}{ts}{ts}xyprintf(103, 7 + i, \"%4d\",                   MM[{_mpf}{_mrealtijdmin}{firstFcName} + i]);");
                            sb.AppendLine($"{ts}{ts}{ts}xyprintf(108, 7 + i, \"%4d\",                   MM[{_mpf}{_mrealtijdmax}{firstFcName} + i]);");
                        }

                        sb.AppendLine($"{ts}{ts}}}");
                    }
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}#endif");
                    
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    if (c.InterSignaalGroep?.Gelijkstarten?.Count == 0
                        && c.InterSignaalGroep?.Voorstarten?.Count == 0
                        && c.InterSignaalGroep?.LateReleases?.Count == 0
                        && c.InterSignaalGroep?.Nalopen?.Count == 0) return null;
                    
                    sb.AppendLine($"{ts}/* Reset synchronisatie BITs */");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}RR[fc] &= ~(BIT1|BIT2);");
                    sb.AppendLine($"{ts}{ts}RW[fc] &= ~(BIT1);");
                    sb.AppendLine($"{ts}{ts}YV[fc] &= ~(BIT1);");
                    sb.AppendLine($"{ts}{ts}YM[fc] &= ~(BIT3);");
                    sb.AppendLine($"{ts}{ts} X[fc] &= ~(BIT1|BIT2);");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Uitvoeren synchronisaties */");
                    if (c.HalfstarData.IsHalfstar)
                    {
                        sb.AppendLine($"{ts}Synchroniseer_SP(IH[{_hpf}{_hplact}]); /* synchronisatie intrekken tbv SP */");
                    }
                    sb.AppendLine($"{ts}Synchroniseer_SG(); /* synchronisatie obv realtijd (startgroenmomenten) */");
                    if (c.HasDeelConflict())
                    {
                        sb.AppendLine($"{ts}Synchroniseer_FO(); /* synchronisatie obv fictieve ontruiming */");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCRealisatieAfhandelingVoorModules:
                    if (c.InterSignaalGroep?.Gelijkstarten?.Count == 0
                        && c.InterSignaalGroep?.Voorstarten?.Count == 0
                        && c.InterSignaalGroep?.LateReleases?.Count == 0
                        && c.InterSignaalGroep?.Nalopen?.Count == 0) return null;
                    
                    sb.AppendLine($"{ts}Synchroniseer_PG(); /* synchronisatie PG's */");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCMaxgroen:
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                    foreach (var (m1, m2, gelijkstart) in _sortedSyncs.twoWayPedestrians)
                    {
                        if (first)
                        {
                            sb.AppendLine($"{ts}/* Bij inlopen, inlopende richting in WG houden t.b.v. eventuele aanvraag naloop in tegenrichting */");
                            first = false;
                        }
                        sb.AppendLine($"{ts}RW[{_fcpf}{m1.FaseVan}] |= T[{_tpf}{_trealil}{m1.FaseVan}{m1.FaseNaar}] ? BIT2 : 0;");
                        sb.AppendLine($"{ts}RW[{_fcpf}{m2.FaseVan}] |= T[{_tpf}{_trealil}{m2.FaseVan}{m2.FaseNaar}] ? BIT2 : 0;");
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
            _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");
            _cvchd = CCOLGeneratorSettingsProvider.Default.GetElementName("cvchd");
		    
            return base.SetSettings(settings);
        }
    }
}
