using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLFileIngrepenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;

        private string _hfile;
        private string _usfile;
        private string _hafv;
        private string _tafv;
        private string _schafv;
        private string _trij;
        private string _tbz;
        private string _prmfperc;
        private string _prmfmeldmin;

        // read from other objects
        private string _mperiod;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyBitmapOutputs = new List<CCOLIOElement>();

            foreach (var fm in c.FileIngrepen)
            {
                _MyBitmapOutputs.Add(new CCOLIOElement(fm.BitmapData as IOElementModel, $"{_uspf}{_usfile}{fm.Naam}"));

                _MyElements.Add(
                    new CCOLElement(
                        $"{_usfile}{fm.Naam}",
                        CCOLElementTypeEnum.Uitgang));
                _MyElements.Add(
                    new CCOLElement(
                        $"{_hfile}{fm.Naam}",
                        CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(
                    new CCOLElement(
                        $"{_tafv}{fm.Naam}",
#warning Make this configurable via GUI
                        // fm.AfvalVertraging
                        600,
                        CCOLElementTimeTypeEnum.TE_type,
                        CCOLElementTypeEnum.Timer));
                _MyElements.Add(
                    new CCOLElement(
                        $"{_schafv}{fm.Naam}",
#warning Make this configurable via GUI (?)
                        1,
                        CCOLElementTimeTypeEnum.SCH_type,
                        CCOLElementTypeEnum.Schakelaar));
                _MyElements.Add(
                    new CCOLElement(
                        $"{_prmfmeldmin}{fm.Naam}",
                        fm.MinimaalAantalMeldingen,
                        CCOLElementTimeTypeEnum.None,
                        CCOLElementTypeEnum.Parameter));
                foreach (var fd in fm.FileDetectoren)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_hafv}{fd.Detector}",
                            CCOLElementTypeEnum.HulpElement));
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_tafv}{fd.Detector}",
                            fd.AfvalVertraging,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Timer));
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_tbz}{fd.Detector}",
                            fd.BezetTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Timer));
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_trij}{fd.Detector}",
                            fd.RijTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Timer));
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_hfile}{fd.Detector}",
                            CCOLElementTypeEnum.HulpElement));
                }
                foreach(var ff in fm.TeDoserenSignaalGroepen)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_prmfperc}{ff.FaseCyclus}",
                            ff.DoseerPercentage,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                }
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public override bool HasCCOLBitmapOutputs()
        {
            return true;
        }

        public override IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs()
        {
            return _MyBitmapOutputs;
        }

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.SystemApplication:
                case CCOLRegCCodeTypeEnum.FileVerwerking:
                    return true;
                default:
                    return false;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
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
                        sb.AppendLine($";");

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
                            sb.AppendLine($"{ts}{ts}PercentageMaxGroenTijden({_fcpf}{ff.FaseCyclus}, {_mpf}{_mperiod}, {_prmpf}{_prmfperc}{ff.FaseCyclus},");
                            sb.Append("".PadLeft($"{ts}{ts}PercentageMaxGroenTijden(".Length));
                            string rest = "";
                            int irest = 1;
                            rest += $", {_prmpf}{c.PeriodenData.DefaultPeriodeGroentijdenSet.ToLower()}{ff.FaseCyclus}";
                            foreach (PeriodeModel per in c.PeriodenData.Perioden)
                            {
                                if (per.Type == PeriodeTypeEnum.Groentijden)
                                {
                                    foreach (GroentijdenSetModel mgsm in c.GroentijdenSets)
                                    {
                                        if (mgsm.Naam == per.GroentijdenSet)
                                        {
                                            foreach (GroentijdModel mgm in mgsm.Groentijden)
                                            {
                                                if (mgm.FaseCyclus == ff.FaseCyclus && mgm.Waarde.HasValue)
                                                {
                                                    ++irest;
                                                    rest += $", {_prmpf}{per.GroentijdenSet.ToLower()}{ff.FaseCyclus}";
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            sb.AppendLine($"{irest}{rest});");
                        }
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.SystemApplication:
                    sb.AppendLine($"{ts}/* file verklikking */");
                    sb.AppendLine($"{ts}/* ---------------- */");
                    foreach (var f in c.FileIngrepen)
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usfile}{f.Naam}] = IH[{_hpf}{_hfile}{f.Naam}];");
                    }
                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool HasSettings()
        {
            return true;
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            if (settings == null || settings.Settings == null)
            {
                return false;
            }

            foreach (var s in settings.Settings)
            {
                if (s.Default == "file")
                {
                    switch (s.Type)
                    {
                        case CCOLGeneratorSettingTypeEnum.Uitgang: _usfile = s.Setting == null ? s.Default : s.Setting; break;
                        case CCOLGeneratorSettingTypeEnum.HulpElement: _hfile = s.Setting == null ? s.Default : s.Setting; break;
                    }
                }
                if (s.Default == "bz") _tbz = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "rij") _trij = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "fperc") _prmfperc = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "fmeldmin") _prmfmeldmin = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "afv")
                {
                    switch (s.Type)
                    {
                        case CCOLGeneratorSettingTypeEnum.Timer: _tafv = s.Setting == null ? s.Default : s.Setting; break;
                        case CCOLGeneratorSettingTypeEnum.HulpElement: _hafv = s.Setting == null ? s.Default : s.Setting; break;
                        case CCOLGeneratorSettingTypeEnum.Schakelaar: _schafv = s.Setting == null ? s.Default : s.Setting; break;
                    }
                }
            }

            _mperiod = CCOLGeneratorSettingsProvider.Default.GetElementName("mperiod");

            return base.SetSettings(settings);
        }
    }
}
