using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLFileIngrepenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _myElements;
        private List<CCOLIOElement> _myBitmapOutputs;

#pragma warning disable 0649
        private string _hfile;
        private string _usfile;
        private string _hafv;
        private string _tafv;
        private string _schafv;
        private string _trij;
        private string _tbz;
        private string _prmfperc;
        private string _prmfmeldmin;
        private string _scheerlijkdoseren;
#pragma warning restore 0649

        // read from other objects
        private string _mperiod;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myBitmapOutputs = new List<CCOLIOElement>();

            foreach (var fm in c.FileIngrepen)
            {
                _myBitmapOutputs.Add(new CCOLIOElement(fm.BitmapData, $"{_uspf}{_usfile}{fm.Naam}"));

                if(fm.EerlijkDoseren)
                {
                    _myElements.Add(
                        new CCOLElement(
                            $"{_scheerlijkdoseren}{fm.Naam}",
                            fm.EerlijkDoseren ? 1 : 0,
                            CCOLElementTimeTypeEnum.SCH_type,
                            CCOLElementTypeEnum.Schakelaar));
                }

                _myElements.Add(
                    new CCOLElement(
                        $"{_usfile}{fm.Naam}",
                        CCOLElementTypeEnum.Uitgang));
                _myElements.Add(
                    new CCOLElement(
                        $"{_hfile}{fm.Naam}",
                        CCOLElementTypeEnum.HulpElement));
                _myElements.Add(
                    new CCOLElement(
                        $"{_tafv}{fm.Naam}",
                        fm.AfvalVertraging,
                        CCOLElementTimeTypeEnum.TE_type,
                        CCOLElementTypeEnum.Timer));
                _myElements.Add(
                    new CCOLElement(
                        $"{_schafv}{fm.Naam}",
#warning Make this configurable via GUI (?)
                        1,
                        CCOLElementTimeTypeEnum.SCH_type,
                        CCOLElementTypeEnum.Schakelaar));
                _myElements.Add(
                    new CCOLElement(
                        $"{_prmfmeldmin}{fm.Naam}",
                        fm.MinimaalAantalMeldingen,
                        CCOLElementTimeTypeEnum.None,
                        CCOLElementTypeEnum.Parameter));
                foreach (var fd in fm.FileDetectoren)
                {
                    _myElements.Add(
                        new CCOLElement(
                            $"{_hafv}{fd.Detector}",
                            CCOLElementTypeEnum.HulpElement));
                    _myElements.Add(
                        new CCOLElement(
                            $"{_tafv}{fd.Detector}",
                            fd.AfvalVertraging,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Timer));
                    _myElements.Add(
                        new CCOLElement(
                            $"{_tbz}{fd.Detector}",
                            fd.BezetTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Timer));
                    _myElements.Add(
                        new CCOLElement(
                            $"{_trij}{fd.Detector}",
                            fd.RijTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Timer));
                    _myElements.Add(
                        new CCOLElement(
                            $"{_hfile}{fd.Detector}",
                            CCOLElementTypeEnum.HulpElement));
                }
                if (fm.EerlijkDoseren && fm.TeDoserenSignaalGroepen.Count > 0)
                {
                    _myElements.Add(
                        new CCOLElement(
                            $"{_prmfperc}{fm.Naam}",
                            fm.TeDoserenSignaalGroepen[0].DoseerPercentage,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                }
                else
                {
                    foreach (var ff in fm.TeDoserenSignaalGroepen)
                    {
                        _myElements.Add(
                            new CCOLElement(
                                $"{_prmfperc}{ff.FaseCyclus}",
                                ff.DoseerPercentage,
                                CCOLElementTimeTypeEnum.None,
                                CCOLElementTypeEnum.Parameter));
                    }
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

        public override bool HasCCOLBitmapOutputs()
        {
            return true;
        }

        public override IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs()
        {
            return _myBitmapOutputs;
        }

        public override int HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Top:
                    return 1;
                case CCOLRegCCodeTypeEnum.InitApplication:
                    return 1;
                case CCOLRegCCodeTypeEnum.SystemApplication:
                    return 1;
                case CCOLRegCCodeTypeEnum.FileVerwerking:
                    return 1;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Top:
                    if (c.FileIngrepen.Any(x => x.EerlijkDoseren))
                    {
                        sb.AppendLine("/* Variabelen eerlijke filedosering */");
                        sb.AppendLine("");

                        foreach (var fi in c.FileIngrepen)
                        {
                            if (fi.EerlijkDoseren)
                            {
                                sb.AppendLine($"/* File ingreep {fi.Naam} */");
                                sb.AppendLine($"#define filefcmax{fi.Naam} {fi.TeDoserenSignaalGroepen.Count} // Aantal fasen die worden gedoseerd");
                                sb.AppendLine($"static count filefc_{fi.Naam}[filefcmax{fi.Naam}]; // Opslag fasenummers");
                                if (c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden)
                                {
                                    sb.AppendLine($"static count filefcmg_{fi.Naam}[filefcmax{fi.Naam}][{c.PeriodenData.Perioden.Count(x => x.Type == PeriodeTypeEnum.Groentijden) + 1}]; // Opslag bij fasen behorende MG parameter nummers");
                                }
                                else
                                {
                                    sb.AppendLine($"static count filefcvg_{fi.Naam}[filefcmax{fi.Naam}][{c.PeriodenData.Perioden.Count(x => x.Type == PeriodeTypeEnum.Groentijden) + 1}]; // Opslag bij fasen behorende MG parameter nummers");
                                }
                                sb.AppendLine($"static int nogtedoseren_{fi.Naam}[filefcmax{fi.Naam}] = {{0}}; // Opslag nog te doseren actueel per fase");
                            }
                        }
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.InitApplication:
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

                case CCOLRegCCodeTypeEnum.FileVerwerking:
                    sb.AppendLine($"{ts}int i;");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* File afhandeling */");
                    sb.AppendLine($"{ts}/* ---------------- */");
                    sb.AppendLine();

                    foreach (var fm in c.FileIngrepen)
                    {
                        sb.AppendLine($"{ts}/* File ingreep {fm.Naam} */");
                        sb.AppendLine();

                        foreach (var fd in fm.FileDetectoren)
                        {
                            sb.AppendLine($"{ts}FileMelding({_dpf}{fd.Detector}, {_tpf}{_tbz}{fd.Detector}, {_tpf}{_trij}{fd.Detector}, {_tpf}{_hafv}{fd.Detector}, {_hpf}{_hfile}{fd.Detector}, {_hpf}{_hafv}{fd.Detector});");
                        }

                        sb.Append($"{ts}RT[{_tpf}{_tafv}{fm.Naam}] = ");
                        int i = 0;
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

                        sb.AppendLine($"{ts}if (!(T[{_tpf}{_tafv}{fm.Naam}] || RT[{_tpf}{_tafv}{fm.Naam}]) && SCH[{_schpf}{_schafv}{fm.Naam}])");
                        sb.AppendLine($"{ts}{{");
                        foreach (var fd in fm.FileDetectoren)
                        {
                            sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hfile}{fd.Detector}] = FALSE;");
                        }
                        sb.AppendLine($"{ts}}}");

                        sb.AppendLine($"{ts}i = 0;");
                        foreach (var fd in fm.FileDetectoren)
                        {
                            sb.AppendLine($"{ts}if (H[{_hpf}{_hfile}{fd.Detector}]) ++i;");
                        }
                        sb.AppendLine($"{ts}IH[{_hpf}{_hfile}{fm.Naam}] = (i >= PRM[{_prmpf}{_prmfmeldmin}{fm.Naam}]);");

                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* percentage MG bij filemelding */");
                        sb.AppendLine($"{ts}if (IH[{_hpf}{_hfile}{fm.Naam}])");
                        sb.AppendLine($"{ts}{{");
                        foreach (var ff in fm.TeDoserenSignaalGroepen)
                        {
                            string grfunc = "";
                            switch (c.Data.TypeGroentijden)
                            {
                                case GroentijdenTypeEnum.MaxGroentijden: grfunc = "PercentageMaxGroenTijden"; break;
                                case GroentijdenTypeEnum.VerlengGroentijden: grfunc = "PercentageVerlengGroenTijden"; break;
                            }
                            sb.AppendLine(fm.EerlijkDoseren
                                ? $"{ts}{ts}{grfunc}({_fcpf}{ff.FaseCyclus}, {_mpf}{_mperiod}, {_prmpf}{_prmfperc}{fm.Naam},"
                                : $"{ts}{ts}{grfunc}({_fcpf}{ff.FaseCyclus}, {_mpf}{_mperiod}, {_prmpf}{_prmfperc}{ff.FaseCyclus},");
                            sb.Append("".PadLeft($"{ts}{ts}{grfunc}(".Length));
                            var rest = "";
                            var irest = 1;
                            rest += $", {_prmpf}{c.PeriodenData.DefaultPeriodeGroentijdenSet.ToLower()}_{ff.FaseCyclus}";

                            foreach (var per in c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.Groentijden))
                            {
                                foreach (var mgsm in c.GroentijdenSets.Where(x => x.Naam == per.GroentijdenSet))
                                {
                                    foreach (var mgm in mgsm.Groentijden.Where(
                                        x => x.FaseCyclus == ff.FaseCyclus && x.Waarde.HasValue))
                                    {
                                        ++irest;
                                        rest += $", {_prmpf}{per.GroentijdenSet.ToLower()}_{ff.FaseCyclus}";
                                    }
                                }
                            }
                            sb.AppendLine($"{irest}{rest});");
                        }
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine();
                    }
                    if (c.FileIngrepen.Any(x => x.EerlijkDoseren))
                    {
                        sb.AppendLine($"{ts}/* Eerlijk doseren: deze functie compenseert zodanig, dat voor alle richtingen gelijk wordt gedoseerd. */");
                        foreach (var fm in c.FileIngrepen)
                        {
                            if (fm.EerlijkDoseren && fm.TeDoserenSignaalGroepen.Count > 0)
                            {
                                sb.AppendLine($"{ts}if(SCH[{_schpf}{_scheerlijkdoseren}{fm.Naam}])");
                                sb.AppendLine($"{ts}{{");
                                if (c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden)
                                {
                                    sb.AppendLine($"{ts}{ts}Eerlijk_doseren_V1({_hpf}{_hfile}{fm.Naam}, {_prmpf}{_prmfperc}{fm.Naam}, filefcmax{fm.Naam}, filefc_{fm.Naam}, filefcmg_{fm.Naam}, nogtedoseren_{fm.Naam});");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}{ts}Eerlijk_doseren_VerlengGroenTijden_V1({_hpf}{_hfile}{fm.Naam}, {_prmpf}{_prmfperc}{fm.Naam}, filefcmax{fm.Naam}, filefc_{fm.Naam}, filefcvg_{fm.Naam}, nogtedoseren_{fm.Naam});");
                                }
                                    
                                sb.AppendLine($"{ts}}}");
                            }
                            sb.AppendLine();
                        }
                    }
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.SystemApplication:
                    sb.AppendLine($"{ts}/* file verklikking */");
                    sb.AppendLine($"{ts}/* ---------------- */");
                    foreach (var f in c.FileIngrepen)
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usfile}{f.Naam}] = IH[{_hpf}{_hfile}{f.Naam}];");
                    }
                    sb.AppendLine();
                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _mperiod = CCOLGeneratorSettingsProvider.Default.GetElementName("mperiod");

            return base.SetSettings(settings);
        }
    }
}
