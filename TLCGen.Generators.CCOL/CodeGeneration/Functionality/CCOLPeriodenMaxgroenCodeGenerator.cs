using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    [CCOLCodePieceGenerator]
    public class CCOLPeriodenMaxgroenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;

        private string _usperdef; // output default period name
        private string _usper;    // output period name
        private string _prmstkp;  // parameter start period name
        private string _prmetkp;  // parameter end period name
        private string _prmdckp;  // parameter day type period name
        private string _mperiod;  // period me name

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyBitmapOutputs = new List<CCOLIOElement>();

            // outputs
            _MyElements.Add(new CCOLElement(_usperdef, CCOLElementTypeEnum.Uitgang));
            _MyBitmapOutputs.Add(new CCOLIOElement(c.PeriodenData.DefaultPeriodeBitmapData as IOElementModel, $"{_uspf}{_usperdef}"));
            foreach (var item in c.PeriodenData.Perioden)
            {
                _MyElements.Add(new CCOLElement(_usper + item.Naam, CCOLElementTypeEnum.Uitgang));
                _MyBitmapOutputs.Add(new CCOLIOElement(item.BitmapData as IOElementModel, $"{_uspf}{_usper}{item.Naam}"));
            }

            // parameters
            int i = 1;
            foreach (var per in c.PeriodenData.Perioden)
            {
                var hours = per.StartTijd.Hours;
                if (per.StartTijd.Days == 1)
                {
                    hours = 24;
                }
                var inst = hours * 100 + per.StartTijd.Minutes;
                _MyElements.Add(new CCOLElement($"{_prmstkp}{i}", inst, CCOLElementTimeTypeEnum.TI_type, CCOLElementTypeEnum.Parameter));
                hours = per.EindTijd.Hours;
                if (per.EindTijd.Days == 1)
                {
                    hours = 24;
                }
                inst = hours * 100 + per.EindTijd.Minutes;
                _MyElements.Add(new CCOLElement($"{_prmetkp}{i}", inst, CCOLElementTimeTypeEnum.TI_type, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_prmdckp}{i}", (int)per.DagCode, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                ++i;
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
                case CCOLRegCCodeTypeEnum.KlokPerioden:
                case CCOLRegCCodeTypeEnum.Maxgroen:
                case CCOLRegCCodeTypeEnum.SystemApplication:
                    return true;
                default:
                    return false;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();
            int i;

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.KlokPerioden:
                    sb.AppendLine("void KlokPerioden(void)");
                    sb.AppendLine("{");
                    sb.AppendLine($"{ts}/* default klokperiode voor max.groen */");
                    sb.AppendLine($"{ts}/* ---------------------------------- */");
                    sb.AppendLine($"{ts}MM[{_mpf}{_mperiod}] = 0;");
                    sb.AppendLine();
                    i = 1;
                    foreach (PeriodeModel kpm in c.PeriodenData.Perioden)
                    {
                        string comm = kpm.Commentaar;
                        if (comm == null) comm = "";
                        sb.AppendLine($"{ts}/* klokperiode: {comm} */");
                        sb.AppendLine($"{ts}/* -------------{new string('-', comm.Length)} */");
                        sb.AppendLine($"{ts}if (klokperiode(PRM[{_prmpf}{_prmstkp}{i}], PRM[{_prmpf}{_prmetkp}{i}]) &&");
                        sb.AppendLine($"{ts}    dagsoort(PRM[{_prmpf}{_prmdckp}{i}]))");
                        sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mperiod}] = {i};");
                        sb.AppendLine();
                        ++i;
                    }
                    sb.AppendLine($"{ts}KlokPerioden_Add();");
                    sb.AppendLine("}");
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.Maxgroen:
                    sb.AppendLine();
                    // Maxgroen obv periode
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        // Check if the FaseCyclus has any maxgreen times set
                        bool HasMG = false;
                        foreach (GroentijdenSetModel mgsm in c.GroentijdenSets)
                        {
                            foreach (GroentijdModel mgm in mgsm.Groentijden)
                            {
                                if (mgm.FaseCyclus == fcm.Naam && mgm.Waarde.HasValue)
                                {
                                    HasMG = true;
                                }
                            }
                        }

                        if (HasMG)
                        {
                            sb.AppendLine($"{ts}max_star_groentijden_va_arg((count) {_fcpf}{fcm.Naam}, (mulv) FALSE, (mulv) FALSE,");
                            int mper = 1;
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
                                                if (mgm.FaseCyclus == fcm.Naam && mgm.Waarde.HasValue)
                                                {
                                                    sb.Append("".PadLeft(($"{ts}max_star_groentijden_va_arg(").Length));
                                                    sb.AppendLine(
                                                       ($"(va_mulv) PRM[{_prmpf}{per.GroentijdenSet.ToLower()}{fcm.Naam}], (va_mulv) NG, (va_mulv) (MM[mperiod] == {mper}),"));
                                                }
                                            }
                                        }
                                    }
                                    ++mper;
                                }
                            }
                            sb.Append("".PadLeft(($"{ts}max_star_groentijden_va_arg(").Length));
                            sb.AppendLine($"(va_mulv) PRM[{_prmpf}{c.PeriodenData.DefaultPeriodeGroentijdenSet.ToLower()}{fcm.Naam}], (va_mulv) NG, (va_count) END);");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}TVG_max[{_fcpf}{fcm.Naam}] = 0;");
                        }

                    }
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.SystemApplication:
                    sb.AppendLine("/* periode verklikking */");
                    sb.AppendLine("/* ------------------- */");
                    i = 0;
                    sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usperdef}] = (MM[{_mpf}{_mperiod}] == {i++});");
                    foreach (var per in c.PeriodenData.Perioden)
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{per.Naam}] = (MM[{_mpf}{_mperiod}] == {i++});");
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

        public override void SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            foreach (var s in settings.Settings)
            {
                switch(s.Default)
                {
                    case "perdef": _usperdef = s.Setting == null ? s.Default : s.Setting; break;
                    case "per": _usper = s.Setting == null ? s.Default : s.Setting; break;
                    case "stkp": _prmetkp = s.Setting == null ? s.Default : s.Setting; break;
                    case "etkp": _prmstkp = s.Setting == null ? s.Default : s.Setting; break;
                    case "dckp": _prmdckp = s.Setting == null ? s.Default : s.Setting; break;
                    case "period": _mperiod = s.Setting == null ? s.Default : s.Setting; break;
                }
            }

            base.SetSettings(settings);
        }
    }
}
