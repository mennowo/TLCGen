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
    public class FileIngrepenCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _hfile;
        private CCOLGeneratorCodeStringSettingModel _schfile;
        private CCOLGeneratorCodeStringSettingModel _usfile;
        private CCOLGeneratorCodeStringSettingModel _tafv;
        private CCOLGeneratorCodeStringSettingModel _trij;
        private CCOLGeneratorCodeStringSettingModel _tbz;
        private CCOLGeneratorCodeStringSettingModel _prmfperc;
        private CCOLGeneratorCodeStringSettingModel _schparlus;
        private CCOLGeneratorCodeStringSettingModel _schparstrook;
        private CCOLGeneratorCodeStringSettingModel _schfiledoseren;
        private CCOLGeneratorCodeStringSettingModel _schfilealtgset;
        private CCOLGeneratorCodeStringSettingModel _scheerlijkdoseren;
        private CCOLGeneratorCodeStringSettingModel _hafk;
        private CCOLGeneratorCodeStringSettingModel _tafkmingroen;
        private CCOLGeneratorCodeStringSettingModel _tminrood;
        private CCOLGeneratorCodeStringSettingModel _tmaxgroen;
#pragma warning restore 0649

        // read from other objects
        private string _mperiod;
        private string _hplact;
        private string _hhd;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var fm in c.FileIngrepen)
            {
                var name = CCOLCodeHelper.GetNameFromNameAndElementName(_usfile, fm.Naam);
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        name,
                        _usfile, fm.BitmapData, fm.Naam));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_hfile}{fm.Naam}",
                        _hfile, fm.Naam));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_schfile}{fm.Naam}",
                        fm.AanUit ? 1 : 0, 
                        CCOLElementTimeTypeEnum.SCH_type,
                        _schfile, fm.Naam));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tafv}{fm.Naam}",
                        fm.AfvalVertraging,
                        CCOLElementTimeTypeEnum.TE_type,
                        _tafv, fm.Naam));
                
                if(fm.EerlijkDoseren)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_scheerlijkdoseren}{fm.Naam}",
                            fm.EerlijkDoseren ? 1 : 0,
                            CCOLElementTimeTypeEnum.SCH_type,
                            _scheerlijkdoseren, fm.Naam));
                }

                if (fm.ToepassenDoseren != NooitAltijdAanUitEnum.Nooit && fm.ToepassenDoseren != NooitAltijdAanUitEnum.Altijd)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_schfiledoseren}{fm.Naam}",
                            fm.ToepassenDoseren == NooitAltijdAanUitEnum.SchAan ? 1 : 0,
                            CCOLElementTimeTypeEnum.SCH_type,
                            _schfiledoseren, fm.Naam));
                }

                if (fm.ToepassenAlternatieveGroentijdenSet != NooitAltijdAanUitEnum.Nooit && fm.ToepassenAlternatieveGroentijdenSet != NooitAltijdAanUitEnum.Altijd &&
                    !string.IsNullOrWhiteSpace(fm.AlternatieveGroentijdenSet) && fm.AlternatieveGroentijdenSet != "NG")
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_schfilealtgset}{fm.Naam}",
                            fm.ToepassenAlternatieveGroentijdenSet == NooitAltijdAanUitEnum.SchAan ? 1 : 0,
                            CCOLElementTimeTypeEnum.SCH_type,
                            _schfilealtgset, fm.Naam));
                }

                var detectorDict = new Dictionary<int, List<string>>();
                foreach (var fmd in fm.FileDetectoren)
                {
                    var d = c.Fasen.SelectMany(x => x.Detectoren).FirstOrDefault(x => x.Naam == fmd.Detector) ??
                            c.Detectoren.FirstOrDefault(x => x.Naam == fmd.Detector);
                    if (d?.Rijstrook == null) continue;
                    if (!detectorDict.ContainsKey(d.Rijstrook.Value))
                    {
                        detectorDict.Add(d.Rijstrook.Value, new List<string> { fmd.Detector });
                    }
                    else
                    {
                        detectorDict[d.Rijstrook.Value].Add(fmd.Detector);
                    }
                }
                var multiStrook = detectorDict.Count > 1;
                var multiLusPerStrook = detectorDict.Any(x => x.Value.Count > 1) ||
                                        detectorDict.Count == 0 && fm.FileDetectoren.Count > 1;

                if (fm.FileDetectoren.Count > 1)
                {
                    if (multiLusPerStrook)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_hfile}{fm.Naam}{_schparlus}",
                                fm.MetingPerLus ? 1 : 0,
                                CCOLElementTimeTypeEnum.SCH_type,
                                _schparlus, fm.Naam));
                    }
                    if(multiStrook)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_hfile}{fm.Naam}{_schparstrook}",
                                fm.MetingPerStrook ? 1 : 0,
                                CCOLElementTimeTypeEnum.SCH_type,
                                _schparstrook, fm.Naam));
                    }
                }
                foreach (var fd in fm.FileDetectoren)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tafv}{fd.Detector}",
                            fd.AfvalVertraging,
                            CCOLElementTimeTypeEnum.TE_type,
                            _tafv, fd.Detector));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tbz}{fd.Detector}",
                            fd.BezetTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            _tbz, fd.Detector));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_trij}{fd.Detector}",
                            fd.RijTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            _trij, fd.Detector));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_hfile}{fd.Detector}",
                            _hfile, fd.Detector));
                }
                if (fm.EerlijkDoseren && fm.TeDoserenSignaalGroepen.Count > 0)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmfperc}{fm.Naam}",
                            fm.TeDoserenSignaalGroepen[0].DoseerPercentage,
                            CCOLElementTimeTypeEnum.None,
                            _prmfperc, fm.Naam));
                }
                else
                {
                    foreach (var ff in fm.TeDoserenSignaalGroepen)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_prmfperc}{fm.Naam}{ff.FaseCyclus}",
                                ff.DoseerPercentage,
                                CCOLElementTimeTypeEnum.None,
                                _prmfperc, ff.FaseCyclus));
                    }
                }
                foreach (var ff in fm.TeDoserenSignaalGroepen.Where(x => x.AfkappenOpStartFile))
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tafkmingroen}{ff.FaseCyclus}{_hfile}{fm.Naam}",
                            ff.AfkappenOpStartFileMinGroentijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            _tafkmingroen, ff.FaseCyclus));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_hafk}{ff.FaseCyclus}{_hfile}{fm.Naam}",
                            _hafk, ff.FaseCyclus));
                }
                foreach (var ff in fm.TeDoserenSignaalGroepen.Where(x => x.MinimaleRoodtijd))
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tminrood}{ff.FaseCyclus}{_hfile}{fm.Naam}",
                            ff.MinimaleRoodtijdTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            _tminrood, ff.FaseCyclus));
                }
                foreach (var ff in fm.TeDoserenSignaalGroepen.Where(x => x.MaximaleGroentijd))
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tmaxgroen}{ff.FaseCyclus}{_hfile}{fm.Naam}",
                            ff.MaximaleGroentijdTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            _tmaxgroen, ff.FaseCyclus));
                }
            }
        }

        public override bool HasCCOLElements() => true;
        
        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCFileVerwerking:
                    if(!c.FileIngrepen.Any(x => x.TeDoserenSignaalGroepen.Any(x2 => x2.AfkappenOpStartFile || x2.MinimaleRoodtijd)))
                        return base.GetFunctionLocalVariables(c, type);
                    return new List<CCOLLocalVariable> { new CCOLLocalVariable("int", "fc") };
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCTop => new []{10},
                CCOLCodeTypeEnum.RegCAanvragen => new []{80}, 
                CCOLCodeTypeEnum.RegCInitApplication => new []{10},
                CCOLCodeTypeEnum.RegCSystemApplication => new []{10},
                CCOLCodeTypeEnum.RegCWachtgroen => new []{20},
                CCOLCodeTypeEnum.RegCFileVerwerking => new []{10},
                CCOLCodeTypeEnum.PrioCPARCorrecties => new []{20},
                CCOLCodeTypeEnum.PrioCPrioriteitsOpties => new []{30},
                CCOLCodeTypeEnum.PrioCPrioriteitsToekenning => new []{10},
                CCOLCodeTypeEnum.RegCVerlenggroen => new []{40},
                CCOLCodeTypeEnum.RegCMaxgroen => new []{40},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();
            var first = true;

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    foreach (var fi in c.FileIngrepen.Where(x => x.FileMetingLocatie == FileMetingLocatieEnum.NaStopstreep))
                    {
                        if (first)
                        {
                            sb.AppendLine($"{ts}/* Geen wachtstand aanvraag bij file stroomafwaarts */");
                            first = false;
                        }

                        foreach (var f in fi.TeDoserenSignaalGroepen)
                        {
                            sb.AppendLine($"{ts}if (IH[{_hpf}{_hfile}{fi.Naam}] && !G[{_fcpf}{f.FaseCyclus}]) A[{_fcpf}{f.FaseCyclus}] &= ~BIT2;");
                        }
                    }

                    first = true;
                    foreach (var fi in c.FileIngrepen.Where(x => x.FileMetingLocatie == FileMetingLocatieEnum.NaStopstreep))
                    {
                        if (first)
                        {
                            sb.AppendLine($"{ts}/* Geen mee aanvraag bij file stroomafwaarts */");
                            first = false;
                        }

                        foreach (var f in fi.TeDoserenSignaalGroepen)
                        {
                            sb.AppendLine($"{ts}if (IH[{_hpf}{_hfile}{fi.Naam}] && !G[{_fcpf}{f.FaseCyclus}]) A[{_fcpf}{f.FaseCyclus}] &= ~BIT4;");
                        }
                    }

                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    if (c.FileIngrepen.Any(x => x.ToepassenAlternatieveGroentijdenSet != NooitAltijdAanUitEnum.Nooit && 
                                                !string.IsNullOrWhiteSpace(x.AlternatieveGroentijdenSet) && 
                                                x.AlternatieveGroentijdenSet != "NG") && 
                        c.Fasen.Any())
                    {
                        foreach (var fi in c.FileIngrepen.Where(x => x.ToepassenAlternatieveGroentijdenSet != NooitAltijdAanUitEnum.Nooit &&
                                                                     !string.IsNullOrWhiteSpace(x.AlternatieveGroentijdenSet) &&
                                                                     x.AlternatieveGroentijdenSet != "NG"))
                        {
                            if (!first) sb.AppendLine();
                            first = false;

                            var set = c.GroentijdenSets.FirstOrDefault(x => x.Naam == fi.AlternatieveGroentijdenSet);
                            if (set == null)
                            {
                                sb.AppendLine($"{ts}{ts}!!! Groentijden set {fi.AlternatieveGroentijdenSet} niet gevonden (file ingreep {fi.Naam}) !!!");
                                continue;
                            }

                            sb.AppendLine($"{ts}/* Alternatieve groentijdenset tijdens file ingreep {fi.Naam} ({set.Naam}) */");
                            sb.Append($"{ts}if (IH[{_hpf}{_hfile}{fi.Naam}]");
                            if (fi.ToepassenAlternatieveGroentijdenSet != NooitAltijdAanUitEnum.Altijd)
                            {
                                sb.Append($" && SCH[{_schpf}{_schfilealtgset}{fi.Naam}]");
                            }
                            sb.AppendLine($")");
                            sb.AppendLine($"{ts}{{");
                            foreach (var g in set.Groentijden.Where(x => x.Waarde.HasValue))
                            {
                                sb.AppendLine($"{ts}{ts}max_star_groentijden_va_arg((count){_fcpf}{g.FaseCyclus}, (mulv)FALSE, (mulv)FALSE, (va_mulv)PRM[{_prmpf}{set.Naam.ToLower()}_{g.FaseCyclus}], (va_mulv)NG, (va_count)END);");

                            }
                            sb.AppendLine($"{ts}}}");
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCTop:
                    if (c.FileIngrepen.Any(x => x.EerlijkDoseren))
                    {
                        sb.AppendLine("/* Variabelen eerlijke filedosering */");
                        sb.AppendLine("");

                        foreach (var fi in c.FileIngrepen)
                        {
                            if (fi.EerlijkDoseren)
                            {
                                sb.AppendLine($"/* File ingreep {fi.Naam} */");
                                sb.AppendLine($"#define filefcmax{fi.Naam} {fi.TeDoserenSignaalGroepen.Count} /* Aantal fasen die worden gedoseerd */");
                                sb.AppendLine($"static count filefc_{fi.Naam}[filefcmax{fi.Naam}]; /* Opslag fasenummers */");
                                sb.AppendLine(c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden
                                    ? $"static count filefcmg_{fi.Naam}[filefcmax{fi.Naam}][{c.PeriodenData.Perioden.Count(x => x.Type == PeriodeTypeEnum.Groentijden) + 1}]; /* Opslag bij fasen behorende MG parameter nummers */"
                                    : $"static count filefcvg_{fi.Naam}[filefcmax{fi.Naam}][{c.PeriodenData.Perioden.Count(x => x.Type == PeriodeTypeEnum.Groentijden) + 1}]; /* Opslag bij fasen behorende MG parameter nummers */");
                                sb.AppendLine($"static int nogtedoseren_{fi.Naam}[filefcmax{fi.Naam}] = {{0}}; /* Opslag nog te doseren actueel per fase */");
                            }
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCInitApplication:
                    if (c.FileIngrepen.Any(x => x.EerlijkDoseren))
                    {
                        sb.AppendLine($"{ts}/* Initialiseren variabelen voor eerlijke filedosering */");

                        foreach (var fi in c.FileIngrepen)
                        {
                            if (!fi.EerlijkDoseren) continue;
                            var i = 0;
                            foreach(var fc in fi.TeDoserenSignaalGroepen)
                            {
                                sb.AppendLine($"{ts}filefc_{fi.Naam}[{i++}] = {_fcpf}{fc.FaseCyclus};");
                            }
                            i = 0;
                            var gtt = c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? "mg" : "vg";
                            foreach (var fc in fi.TeDoserenSignaalGroepen)
                            {
                                sb.Append($"{ts}");
                                var defmg = c.GroentijdenSets.FirstOrDefault(
                                    x => x.Naam == c.PeriodenData.DefaultPeriodeGroentijdenSet);
                                var defmgfc = defmg?.Groentijden.FirstOrDefault(x => x.FaseCyclus == fc.FaseCyclus);
                                if (defmgfc?.Waarde != null)
                                {
                                    sb.Append($"filefc{gtt}_{fi.Naam}[{i}][0] = {_prmpf}{c.PeriodenData.DefaultPeriodeGroentijdenSet.ToLower()}_{fc.FaseCyclus};");
                                }
                                var j = 1;
                                foreach (var per in c.PeriodenData.Perioden)
                                {

                                
                                    if (per.Type == PeriodeTypeEnum.Groentijden)
                                    {
                                        foreach (var mgsm in c.GroentijdenSets)
                                        {
                                            if (mgsm.Naam == per.GroentijdenSet)
                                            {
                                                foreach (var mgm in mgsm.Groentijden)
                                                {
                                                    if (mgm.FaseCyclus == fc.FaseCyclus && mgm.Waarde.HasValue)
                                                    {
                                                        sb.Append($" filefc{gtt}_{fi.Naam}[{i}][{j}] = {_prmpf}{mgsm.Naam.ToLower()}_{fc.FaseCyclus};");
                                                    }
                                                }
                                            }
                                        }
                                        ++j;
                                    }
                                }
                                ++i;
                                sb.AppendLine();
                            }
                        }
                        sb.AppendLine();
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCWachtgroen:
                    if (!c.FileIngrepen.Any()) return "";
                    sb.AppendLine($"{ts}/* Niet in wachtgroen vasthouden tijdens file */");
                    var tts = ts;
                    if (c.HalfstarData.IsHalfstar)
                    {
                        tts += ts;
                        sb.AppendLine($"{ts}if (!IH[{_hpf}{_hplact}])");
                        sb.AppendLine($"{ts}{{");
                    }
                    foreach (var fi in c.FileIngrepen)
                    {
                        foreach(var tdfc in fi.TeDoserenSignaalGroepen)
                        {
                            sb.AppendLine($"{tts}if (IH[{_hpf}{_hfile}{fi.Naam}]) RW[{_fcpf}{tdfc.FaseCyclus}] &= ~BIT4;");
                        }
                    }
                    if (c.HalfstarData.IsHalfstar)
                    {
                        sb.AppendLine($"{ts}}}");
                    }

                    if (c.FileIngrepen.Any(x => x.FileMetingLocatie == FileMetingLocatieEnum.NaStopstreep))
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* Geen wachtstand bij file stroomafwaarts */");
                        tts = ts;
                        if (c.HalfstarData.IsHalfstar)
                        {
                            tts += ts;
                            sb.AppendLine($"{ts}if (!IH[{_hpf}{_hplact}])");
                            sb.AppendLine($"{ts}{{");
                        }

                        foreach (var fi in c.FileIngrepen)
                        {
                            foreach (var tdfc in fi.TeDoserenSignaalGroepen)
                            {
                                sb.AppendLine($"{tts}if (IH[{_hpf}{_hfile}{fi.Naam}]) WS[{_fcpf}{tdfc.FaseCyclus}] &= ~BIT4;");
                            }
                        }

                        if (c.HalfstarData.IsHalfstar)
                        {
                            sb.AppendLine($"{ts}}}");
                        }
                    }

                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCFileVerwerking:
                    if (!c.FileIngrepen.Any()) return "";

                    sb.AppendLine($"{ts}/* File afhandeling */");
                    sb.AppendLine($"{ts}/* ---------------- */");
                    sb.AppendLine();

                    if (c.FileIngrepen.Any(x => x.TeDoserenSignaalGroepen.Any(x2 => x2.AfkappenOpStartFile || x2.MinimaleRoodtijd)))
                    {
                        sb.AppendLine($"{ts}/* reset bitsturing */");
                        sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                        sb.AppendLine($"{ts}{{");
                        if (c.FileIngrepen.Any(x => x.TeDoserenSignaalGroepen.Any(x2 => x2.AfkappenOpStartFile || x2.MaximaleGroentijd)))
                            sb.AppendLine($"{ts}{ts}Z[fc] &= ~BIT5;");
                        if (c.FileIngrepen.Any(x => x.TeDoserenSignaalGroepen.Any(x2 => x2.MinimaleRoodtijd)))
                            sb.AppendLine($"{ts}{ts}BL[fc] &= ~BIT5;");
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine();
                    }

                    foreach (var fm in c.FileIngrepen)
                    {
                        if (!first) sb.AppendLine();

                        sb.AppendLine($"{ts}/* File ingreep {fm.Naam} */");
                        sb.AppendLine();

                        foreach (var fd in fm.FileDetectoren)
                        {
                            sb.AppendLine($"{ts}FileMeldingV2({_dpf}{fd.Detector}, {_tpf}{_tbz}{fd.Detector}, {_tpf}{_trij}{fd.Detector}, {_tpf}{_tafv}{fd.Detector}, {_hpf}{_hfile}{fd.Detector});");
                        }

                        sb.Append($"{ts}RT[{_tpf}{_tafv}{fm.Naam}] = ");
                        var i = 0;
                        foreach (var fd in fm.FileDetectoren)
                        {
                            if (i != 0)
                            {
                                sb.Append(" || ");
                            }
                            sb.Append($"D[{_dpf}{fd.Detector}]");
                            ++i;
                        }
                        sb.AppendLine(";");

                        sb.AppendLine($"{ts}if (!(T[{_tpf}{_tafv}{fm.Naam}] || RT[{_tpf}{_tafv}{fm.Naam}]))");
                        sb.AppendLine($"{ts}{{");
                        foreach (var fd in fm.FileDetectoren)
                        {
                            sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hfile}{fd.Detector}] = FALSE;");
                        }
                        sb.AppendLine($"{ts}}}");
                        
                        var detectorDict = new Dictionary<int, List<string>>();
                        foreach (var fmd in fm.FileDetectoren)
                        {
                            var d = c.Fasen.SelectMany(x => x.Detectoren).FirstOrDefault(x => x.Naam == fmd.Detector) ?? 
                                    c.Detectoren.FirstOrDefault(x => x.Naam == fmd.Detector);
                            if (d?.Rijstrook == null) continue;
                            if (!detectorDict.ContainsKey(d.Rijstrook.Value))
                            {
                                detectorDict.Add(d.Rijstrook.Value, new List<string> {fmd.Detector});
                            }
                            else
                            {
                                detectorDict[d.Rijstrook.Value].Add(fmd.Detector);
                            }
                        }
                        var multiStrook = detectorDict.Count > 1;
                        var multiLusPerStrook = detectorDict.Any(x => x.Value.Count > 1) ||
                                                detectorDict.Count == 0 && fm.FileDetectoren.Count > 1;
                        if (multiStrook)
                        {
                            if (multiLusPerStrook)
                            {
                                sb.AppendLine($"{ts}if(SCH[{_schpf}{_hfile}{fm.Naam}{_schparstrook}])");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}/* strook en lus parallel: file via om het even welke lus */");
                                sb.AppendLine($"{ts}{ts}if(SCH[{_schpf}{_hfile}{fm.Naam}{_schparlus}])");
                                sb.AppendLine($"{ts}{ts}{{");
                                sb.Append($"{ts}{ts}{ts}IH[{_hpf}{_hfile}{fm.Naam}] = ");
                                var id = 0;
                                foreach (var fd in fm.FileDetectoren)
                                {
                                    if (id > 0) sb.Append(" || ");
                                    ++id;
                                    sb.Append($"IH[{_hpf}{_hfile}{fd.Detector}]");
                                }
                                sb.AppendLine($";");
                                sb.AppendLine($"{ts}{ts}}}");
                                sb.AppendLine($"{ts}{ts}/* alleen strook parallel: file via alle lussen op 1 strook */");
                                sb.AppendLine($"{ts}{ts}else");
                                sb.AppendLine($"{ts}{ts}{{");
                                sb.Append($"{ts}{ts}{ts}IH[{_hpf}{_hfile}{fm.Naam}] = ");
                                id = 0;
                                foreach (var dd in detectorDict)
                                {
                                    if (id > 0) sb.Append(" || ");
                                    ++id;
                                    sb.Append("(");
                                    var rid = 0;
                                    foreach (var rstrd in dd.Value)
                                    {
                                        if (rid > 0) sb.Append(" && ");
                                        ++rid;
                                        sb.Append($"IH[{_hpf}{_hfile}{rstrd}]");
                                    }
                                    sb.Append(")");
                                }
                                sb.AppendLine(";");
                                sb.AppendLine($"{ts}{ts}}}");
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"{ts}else");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}/* alleen parallel lus: file via minimaal 1 lus per strook */");
                                sb.AppendLine($"{ts}{ts}if(SCH[{_schpf}{_hfile}{fm.Naam}{_schparlus}])");
                                sb.AppendLine($"{ts}{ts}{{");
                                sb.Append($"{ts}{ts}{ts}IH[{_hpf}{_hfile}{fm.Naam}] = ");
                                id = 0;
                                foreach (var dd in detectorDict)
                                {
                                    if (id > 0) sb.Append(" && ");
                                    ++id;
                                    sb.Append("(");
                                    var rid = 0;
                                    foreach (var rstrd in dd.Value)
                                    {
                                        if (rid > 0) sb.Append(" || ");
                                        ++rid;
                                        sb.Append($"IH[{_hpf}{_hfile}{rstrd}]");
                                    }
                                    sb.Append(")");
                                }
                                sb.AppendLine(";");
                                sb.AppendLine($"{ts}{ts}}}");
                                sb.AppendLine($"{ts}{ts}/* niet parallel: file bij melding van alle lussen */");
                                sb.AppendLine($"{ts}{ts}else");
                                sb.AppendLine($"{ts}{ts}{{");
                                sb.Append($"{ts}{ts}{ts}IH[{_hpf}{_hfile}{fm.Naam}] = ");
                                id = 0;
                                foreach (var fd in fm.FileDetectoren)
                                {
                                    if (id > 0) sb.Append(" && ");
                                    ++id;
                                    sb.Append($"IH[{_hpf}{_hfile}{fd.Detector}]");
                                }
                                sb.AppendLine($";");
                                sb.AppendLine($"{ts}{ts}}}");
                                sb.AppendLine($"{ts}}}");
                            }
                            else
                            {
                                sb.AppendLine($"{ts}{ts}/* strook parallel: file bij melding op 1 strook */");
                                sb.AppendLine($"{ts}if(SCH[{_schpf}{_hfile}{fm.Naam}{_schparstrook}])");
                                sb.AppendLine($"{ts}{{");
                                sb.Append($"{ts}{ts}IH[{_hpf}{_hfile}{fm.Naam}] = ");
                                var id = 0;
                                foreach (var fd in fm.FileDetectoren)
                                {
                                    if (id > 0) sb.Append(" || ");
                                    ++id;
                                    sb.Append($"IH[{_hpf}{_hfile}{fd.Detector}]");
                                }
                                sb.AppendLine($";");
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"{ts}/* niet parallel: file bij melding van alle lussen */");
                                sb.AppendLine($"{ts}else");
                                sb.AppendLine($"{ts}{{");
                                sb.Append($"{ts}{ts}IH[{_hpf}{_hfile}{fm.Naam}] = ");
                                id = 0;
                                foreach (var fd in fm.FileDetectoren)
                                {
                                    if (id > 0) sb.Append(" && ");
                                    ++id;
                                    sb.Append($"IH[{_hpf}{_hfile}{fd.Detector}]");
                                }
                                sb.AppendLine($";");
                                sb.AppendLine($"{ts}}}");
                            }
                        }
                        else if (multiLusPerStrook)
                        {
                            sb.AppendLine($"{ts}/* lus parallel: file bij melding op 1 lus */");
                            sb.AppendLine($"{ts}if(SCH[{_schpf}{_hfile}{fm.Naam}{_schparlus}])");
                            sb.AppendLine($"{ts}{{");
                            sb.Append($"{ts}{ts}IH[{_hpf}{_hfile}{fm.Naam}] = ");
                            var id = 0;
                            foreach (var fd in fm.FileDetectoren)
                            {
                                if (id > 0) sb.Append(" || ");
                                ++id;
                                sb.Append($"IH[{_hpf}{_hfile}{fd.Detector}]");
                            }
                            sb.AppendLine(";");
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine($"{ts}/* niet parallel: file bij melding van alle lussen */");
                            sb.AppendLine($"{ts}else");
                            sb.AppendLine($"{ts}{{");
                            sb.Append($"{ts}{ts}IH[{_hpf}{_hfile}{fm.Naam}] = ");
                            id = 0;
                            foreach (var fd in fm.FileDetectoren)
                            {
                                if (id > 0) sb.Append(" && ");
                                ++id;
                                sb.Append($"IH[{_hpf}{_hfile}{fd.Detector}]");
                            }
                            sb.AppendLine($";");
                            sb.AppendLine($"{ts}}}");
                        }
                        else // 1 lus
                        {
                            sb.Append($"{ts}{ts}IH[{_hpf}{_hfile}{fm.Naam}] = ");
                            i = 0;
                            foreach (var fd in fm.FileDetectoren)
                            {
                                if (i != 0)
                                {
                                    sb.Append(" || ");
                                }
                                sb.Append($"IH[{_hpf}{_hfile}{fd.Detector}]");
                                ++i;
                            }
                            sb.AppendLine($";");
                        }
                        
                        sb.AppendLine();
                        tts = ts;
                        if (c.HalfstarData.IsHalfstar)
                        {
                            tts += ts;
                            sb.AppendLine($"{ts}if (!IH[{_hpf}{_hplact}])");
                            sb.AppendLine($"{ts}{{");
                        }

                        if (fm.ToepassenDoseren != NooitAltijdAanUitEnum.Nooit)
                        {
                            sb.AppendLine($"{tts}/* percentage MG bij filemelding */");
                            sb.Append($"{tts}if (IH[{_hpf}{_hfile}{fm.Naam}] && SCH[{_schpf}{_schfile}{fm.Naam}]");
                            if (fm.ToepassenDoseren != NooitAltijdAanUitEnum.Altijd)
                            {
                                sb.Append($" && SCH[{_schpf}{_schfiledoseren}{fm.Naam}]");
                            }
                            sb.AppendLine(")");
                            sb.AppendLine($"{tts}{{");
                            foreach (var ff in fm.TeDoserenSignaalGroepen)
                            {
                                var grfunc = c.Data.TypeGroentijden switch
                                {
                                    GroentijdenTypeEnum.MaxGroentijden => "PercentageMaxGroenTijden",
                                    GroentijdenTypeEnum.VerlengGroentijden => "PercentageVerlengGroenTijden",
                                    _ => ""
                                };
                                sb.AppendLine(fm.EerlijkDoseren
                                    ? $"{tts}{ts}{grfunc}({_fcpf}{ff.FaseCyclus}, {_mpf}{_mperiod}, PRM[{_prmpf}{_prmfperc}{fm.Naam}],"
                                    : $"{tts}{ts}{grfunc}({_fcpf}{ff.FaseCyclus}, {_mpf}{_mperiod}, PRM[{_prmpf}{_prmfperc}{fm.Naam}{ff.FaseCyclus}],");
                                sb.Append("".PadLeft($"{tts}{ts}{grfunc}(".Length));
                                var rest = "";
                                var irest = 1;
                                if (!c.Data.TVGAMaxAlsDefaultGroentijdSet)
                                {
                                    rest += $", PRM[{_prmpf}{c.PeriodenData.DefaultPeriodeGroentijdenSet?.ToLower() ?? "NG"}_{ff.FaseCyclus}]";
                                }
                                else
                                {
                                    rest += $", TVGA_max[{_fcpf}{ff.FaseCyclus}]";
                                }

                                foreach (var per in c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.Groentijden))
                                {
                                    var greentimeSet = c.GroentijdenSets.FirstOrDefault(x => x.Naam == per.GroentijdenSet);

                                    if (greentimeSet.Groentijden.Any(x => x.FaseCyclus == ff.FaseCyclus && x.Waarde.HasValue))
                                    {
                                        ++irest;
                                        rest += $", PRM[{_prmpf}{per.GroentijdenSet.ToLower()}_{ff.FaseCyclus}]";
                                    }
                                }

                                sb.AppendLine($"{irest}{rest});");
                            }
                            sb.AppendLine($"{tts}}}");
                        }

                        if (c.HalfstarData.IsHalfstar)
                        {
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine($"{ts}else");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{tts}/* percentage MG bij filemelding tijdens halfstar */");
                            sb.Append($"{tts}if (IH[{_hpf}{_hfile}{fm.Naam}] && SCH[{_schpf}{_schfile}{fm.Naam}]");
                            if (fm.ToepassenDoseren != NooitAltijdAanUitEnum.Altijd)
                            {
                                sb.Append($" && SCH[{_schpf}{_schfiledoseren}{fm.Naam}]");
                            }
                            sb.AppendLine(")");
                            sb.AppendLine($"{tts}{{");
                            foreach (var ff in fm.TeDoserenSignaalGroepen)
                            {
                                var grfunc = c.Data.TypeGroentijden switch
                                {
                                    GroentijdenTypeEnum.MaxGroentijden => "PercentageMaxGroenTijden_halfstar",
                                    GroentijdenTypeEnum.VerlengGroentijden => "PercentageVerlengGroenTijden_halfstar",
                                    _ => ""
                                };
                                sb.AppendLine(fm.EerlijkDoseren
                                    ? $"{tts}{ts}{grfunc}({_fcpf}{ff.FaseCyclus}, {_prmpf}{_prmfperc}{fm.Naam}, BIT3);"
                                    : $"{tts}{ts}{grfunc}({_fcpf}{ff.FaseCyclus}, {_prmpf}{_prmfperc}{fm.Naam}{ff.FaseCyclus}, BIT3);");
                            }
                            sb.AppendLine($"{tts}}}");
                            sb.AppendLine($"{ts}}}");
                        }

                        sb.AppendLine();

                        if (fm.TeDoserenSignaalGroepen.Any(x => x.AfkappenOpStartFile || x.MaximaleGroentijd))
                        {
                            sb.AppendLine($"{ts}/* Afkappen tijdens file ingreep {fm.Naam} */");
                            sb.AppendLine($"{ts}if (SCH[{_schpf}{_schfile}{fm.Naam}])");
                            sb.AppendLine($"{ts}{{");
                            foreach (var tdfc in fm.TeDoserenSignaalGroepen)
                            {
                                if (tdfc.AfkappenOpStartFile)
                                {
                                    sb.AppendLine($"{ts}{ts}/* Eenmalig afkappen fase {tdfc.FaseCyclus} op start file ingreep */");
                                    sb.AppendLine($"{ts}{ts}RT[{_tpf}{_tafkmingroen}{tdfc.FaseCyclus}{_hfile}{fm.Naam}] = " +
                                                  $"ER[{_fcpf}{tdfc.FaseCyclus}] && " +
                                                  $"T_max[{_tpf}{_tafkmingroen}{tdfc.FaseCyclus}{_hfile}{fm.Naam}];");
                                    sb.AppendLine($"{ts}{ts}if (SH[{_hpf}{_hfile}{fm.Naam}] && G[{_fcpf}{tdfc.FaseCyclus}]) IH[{_hpf}{_hafk}{tdfc.FaseCyclus}{_hfile}{fm.Naam}] = TRUE;");
                                    sb.AppendLine($"{ts}{ts}if (EG[{_fcpf}{tdfc.FaseCyclus}]) IH[{_hpf}{_hafk}{tdfc.FaseCyclus}{_hfile}{fm.Naam}] = FALSE;");
                                }
                                
                                if (tdfc.MaximaleGroentijd)
                                {
                                    sb.AppendLine($"{ts}{ts}/* Afkappen fase {tdfc.FaseCyclus} op max. groentijd tijdens file ingreep */");
                                    sb.AppendLine($"{ts}{ts}RT[{_tpf}{_tmaxgroen}{tdfc.FaseCyclus}{_hfile}{fm.Naam}] = SG[{_fcpf}{tdfc.FaseCyclus}] && T_max[{_tpf}{_tmaxgroen}{tdfc.FaseCyclus}{_hfile}{fm.Naam}];");
                                }

                                if (tdfc.AfkappenOpStartFile || tdfc.MaximaleGroentijd)
                                {
                                    sb.AppendLine($"{ts}{ts}if (G[{_fcpf}{tdfc.FaseCyclus}] && IH[{_hpf}{_hfile}{fm.Naam}])");
                                    sb.AppendLine($"{ts}{ts}{{");
                                    sb.Append($"{ts}{ts}{ts}if (");
                                }

                                var padding = $"{ts}{ts}{ts}if (".Length;
                                if (tdfc.AfkappenOpStartFile)
                                {
                                    sb.AppendLine($"IH[{_hpf}{_hafk}{tdfc.FaseCyclus}{_hfile}{fm.Naam}] && " +
                                                  $"T_max[{_tpf}{_tafkmingroen}{tdfc.FaseCyclus}{_hfile}{fm.Naam}] &&");
                                    sb.Append("".PadLeft(padding) +
                                        $"!RT[{_tpf}{_tafkmingroen}{tdfc.FaseCyclus}{_hfile}{fm.Naam}] && " +
                                                  $"!T[{_tpf}{_tafkmingroen}{tdfc.FaseCyclus}{_hfile}{fm.Naam}] && " +
                                                  $"!(MK[{_fcpf}{tdfc.FaseCyclus}] & PRIO_MK_BIT)");
                                }

                                if (tdfc.MaximaleGroentijd)
                                {
                                    if (tdfc.AfkappenOpStartFile)
                                    {
                                        sb.AppendLine(" || ");
                                        sb.Append("".PadLeft(padding));
                                    }
                                    sb.Append($"!RT[{_tpf}{_tmaxgroen}{tdfc.FaseCyclus}{_hfile}{fm.Naam}] && !T[{_tpf}{_tmaxgroen}{tdfc.FaseCyclus}{_hfile}{fm.Naam}]");
                                }

                                if (tdfc.AfkappenOpStartFile || tdfc.MaximaleGroentijd)
                                {
                                    sb.AppendLine($")");
                                    sb.AppendLine($"{ts}{ts}{ts}{{");
                                    sb.AppendLine($"{ts}{ts}{ts}{ts}Z[{_fcpf}{tdfc.FaseCyclus}] |= BIT5;");
                                    sb.AppendLine($"{ts}{ts}{ts}}}");

                                    sb.AppendLine($"{ts}{ts}}}");
                                }
                            }
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine();
                        }

                        if (fm.TeDoserenSignaalGroepen.Any(x => x.MinimaleRoodtijd))
                        {
                            sb.AppendLine($"{ts}/* Minimale roodtijden tijdens file ingreep {fm.Naam} */");
                            sb.AppendLine($"{ts}if (SCH[{_schpf}{_schfile}{fm.Naam}])");
                            sb.AppendLine($"{ts}{{");
                            foreach (var tdfc in fm.TeDoserenSignaalGroepen.Where(x => x.MinimaleRoodtijd))
                            {
                                sb.AppendLine($"{ts}{ts}/* Minimale roodtijd fase {tdfc.FaseCyclus} */");
                                sb.AppendLine($"{ts}{ts}RT[{_tpf}{_tminrood}{tdfc.FaseCyclus}{_hfile}{fm.Naam}] = EGL[{_fcpf}{tdfc.FaseCyclus}] && T_max[{_tpf}{_tminrood}{tdfc.FaseCyclus}{_hfile}{fm.Naam}];");
                                sb.AppendLine($"{ts}{ts}if (R[{_fcpf}{tdfc.FaseCyclus}] && T[{_tpf}{_tminrood}{tdfc.FaseCyclus}{_hfile}{fm.Naam}] && IH[{_hpf}{_hfile}{fm.Naam}])");
                                sb.AppendLine($"{ts}{ts}{{");
                                sb.AppendLine($"{ts}{ts}{ts}BL[{_fcpf}{tdfc.FaseCyclus}] |= BIT5;");
                                sb.AppendLine($"{ts}{ts}}}");
                            }
                            sb.AppendLine($"{ts}}}");
                        }

                        first = false;
                    }
                    if (c.FileIngrepen.Any(x => x.EerlijkDoseren))
                    {
                        tts = ts;
                        if (c.HalfstarData.IsHalfstar)
                        {
                            tts += ts;
                            sb.AppendLine($"{ts}if (!IH[{_hpf}{_hplact}])");
                            sb.AppendLine($"{ts}{{");
                        }

                        sb.Append($"{tts}/* Eerlijk doseren: deze functie compenseert zodanig, dat voor alle richtingen gelijk wordt gedoseerd. */");
                        foreach (var fm in c.FileIngrepen)
                        {
                            if (fm.EerlijkDoseren && fm.TeDoserenSignaalGroepen.Count > 0)
                            {
                                sb.AppendLine();
                                sb.AppendLine($"{tts}if (SCH[{_schpf}{_scheerlijkdoseren}{fm.Naam}] && SCH[{_schpf}{_schfile}{fm.Naam}])");
                                sb.AppendLine($"{tts}{{");
                                if (c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden)
                                {
                                    if (!c.Data.MultiModuleReeksen)
                                    {
                                        sb.AppendLine($"{tts}{ts}Eerlijk_doseren_V1({_hpf}{_hfile}{fm.Naam}, {_prmpf}{_prmfperc}{fm.Naam}, filefcmax{fm.Naam}, filefc_{fm.Naam}, filefcmg_{fm.Naam}, nogtedoseren_{fm.Naam}, PRML, ML, {_mpf}{_mperiod});");
                                    }
                                    else
                                    {
                                        var r = c.MultiModuleMolens.FirstOrDefault(x => x.Modules.Any(x2 => x2.Fasen.Any(x3 => fm.TeDoserenSignaalGroepen.Any(x4 => x3.FaseCyclus == x4.FaseCyclus))));
                                        if (r != null)
                                        {
                                            sb.AppendLine($"{tts}{ts}Eerlijk_doseren_V1({_hpf}{_hfile}{fm.Naam}, {_prmpf}{_prmfperc}{fm.Naam}, filefcmax{fm.Naam}, filefc_{fm.Naam}, filefcmg_{fm.Naam}, nogtedoseren_{fm.Naam}, PR{r.Reeks}, {r.Reeks}, {_mpf}{_mperiod});");
                                        }
                                    }
                                }
                                else
                                {
                                    if (!c.Data.MultiModuleReeksen)
                                    {
                                        sb.AppendLine($"{tts}{ts}Eerlijk_doseren_VerlengGroenTijden_V1({_hpf}{_hfile}{fm.Naam}, {_prmpf}{_prmfperc}{fm.Naam}, filefcmax{fm.Naam}, filefc_{fm.Naam}, filefcvg_{fm.Naam}, nogtedoseren_{fm.Naam}, PRML, ML, {_mpf}{_mperiod});");
                                    }
                                    else
                                    {
                                        var r = c.MultiModuleMolens.FirstOrDefault(x => x.Modules.Any(x2 => x2.Fasen.Any(x3 => fm.TeDoserenSignaalGroepen.Any(x4 => x3.FaseCyclus == x4.FaseCyclus))));
                                        if (r != null)
                                        {
                                            sb.AppendLine($"{tts}{ts}Eerlijk_doseren_VerlengGroenTijden_V1({_hpf}{_hfile}{fm.Naam}, {_prmpf}{_prmfperc}{fm.Naam}, filefcmax{fm.Naam}, filefc_{fm.Naam}, filefcvg_{fm.Naam}, nogtedoseren_{fm.Naam}, &PR{r.Reeks}, {r.Reeks}, {_mpf}{_mperiod});");
                                        }
                                    }
                                }
                                    
                                sb.AppendLine($"{tts}}}");
                            }
                        }

                        if (c.HalfstarData.IsHalfstar)
                        {
                            sb.AppendLine($"{ts}}}");
                        }
                    }

                    if (c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen)
                    {
                        var fcWithFileIngreep = c.FileIngrepen.SelectMany(x => x.TeDoserenSignaalGroepen).Distinct();
                        first = true;
                        foreach (var fc in fcWithFileIngreep)
                        {
                            if (first)
                            {
                                sb.AppendLine();
                                sb.AppendLine($"{ts}/* Als hulpdienst ingreep aktief is op kruispunt arm dan nooit uitstel of afbreken als gevolg van file stroomafwaarts */");
                                first = false;
                            }
                            if (c.PrioData.HDIngrepen.Any(x => x.FaseCyclus == fc.FaseCyclus))
                            {
                                sb.AppendLine($"{ts}if (IH[{_hpf}{_hhd}{fc.FaseCyclus}])");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}Z[{_fcpf}{fc.FaseCyclus}] &= ~BIT5;");
                                sb.AppendLine($"{ts}{ts}BL[{_fcpf}{fc.FaseCyclus}] &= ~BIT5;");
                                sb.AppendLine($"{ts}}}");
                            }
                        }
                    }
                    
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSystemApplication:
                    if (!c.FileIngrepen.Any()) return "";

                    sb.AppendLine($"{ts}/* file verklikking */");
                    sb.AppendLine($"{ts}/* ---------------- */");
                    foreach (var f in c.FileIngrepen)
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{f.BitmapData.ToString(_usfile)}] = IH[{_hpf}{_hfile}{f.Naam}];");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCPARCorrecties:
                    if (!c.FileIngrepen.Any()) return "";

                    var yes = false;
                    foreach (var fcm in c.Fasen)
                    {
                        foreach (var fm in c.FileIngrepen.Where(x => x.TeDoserenSignaalGroepen.Any(x2 => x2.FaseCyclus == fcm.Naam)))
                        {
                            if (fm is { FileMetingLocatie: FileMetingLocatieEnum.NaStopstreep })
                            {
                                if (!yes)
                                {
                                    yes = true;
                                    sb.AppendLine();
                                    sb.AppendLine($"{ts}/* Niet alternatief komen tijdens file (meting na ss) */");
                                }
                                sb.AppendLine(
                                    $"{ts}if (IH[{_hpf}{_hfile}{fm.Naam}]) PAR[{_fcpf}{fcm.Naam}] = FALSE;");
                            }
                        }
                    }

                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCPrioriteitsOpties:
                    foreach (var fi in c.FileIngrepen.Where(x => x.FileMetingLocatie == FileMetingLocatieEnum.NaStopstreep))
                    {
                        if (first)
                        {
                            sb.AppendLine($"{ts}/* Geen prioriteit bij file stroom afwaarts */");
                            first = false;
                        }

                        sb.AppendLine($"{ts}if (IH[{_hpf}{_hfile}{fi.Naam}])");
                        sb.AppendLine($"{ts}{{");
                        foreach (var f in fi.TeDoserenSignaalGroepen)
                        {
                            var prios = c.PrioData.PrioIngrepen.Where(x => x.FaseCyclus == f.FaseCyclus);
                            foreach (var p in prios)
                            {
                                sb.AppendLine($"{ts}{ts}iPrioriteitsOpties[prioFC{CCOLCodeHelper.GetPriorityName(c, p)}] = poAanvraag;");
                            }
                        }
                        sb.AppendLine($"{ts}}}");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCPrioriteitsToekenning:
                    foreach (var fi in c.FileIngrepen.Where(x => x.FileMetingLocatie == FileMetingLocatieEnum.NaStopstreep))
                    {
                        if (first)
                        {
                            sb.AppendLine($"{ts}/* Geen prioriteit bij file stroom afwaarts */");
                            first = false;
                        }

                        sb.AppendLine($"{ts}if (IH[{_hpf}{_hfile}{fi.Naam}])");
                        sb.AppendLine($"{ts}{{");
                        foreach (var f in fi.TeDoserenSignaalGroepen)
                        {
                            var prios = c.PrioData.PrioIngrepen.Where(x => x.FaseCyclus == f.FaseCyclus);
                            foreach (var p in prios)
                            {
                                sb.AppendLine($"{ts}{ts}iPrioriteit[prioFC{CCOLCodeHelper.GetPriorityName(c, p)}] = 0;");
                            }
                        }

                        sb.AppendLine($"{ts}}}");
                    }

                    return sb.ToString();
                
                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _mperiod = CCOLGeneratorSettingsProvider.Default.GetElementName("mperiod");
            _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");
            _hhd = CCOLGeneratorSettingsProvider.Default.GetElementName("hhd");

            return base.SetSettings(settings);
        }
    }
}
