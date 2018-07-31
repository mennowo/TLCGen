using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    class WachttijdVoorspellerCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _myElements;
        private List<CCOLIOElement> _myBitmapOutputs;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _uswtv;
        private CCOLGeneratorCodeStringSettingModel _twtv;
        private CCOLGeneratorCodeStringSettingModel _mwtv;
        private CCOLGeneratorCodeStringSettingModel _prmminwtv;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myBitmapOutputs = new List<CCOLIOElement>();

            if(c.Fasen.Any(x => x.WachttijdVoorspeller))
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmminwtv}", 2, CCOLElementTimeTypeEnum.None, _prmminwtv));
            }

            foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
            {
                _myBitmapOutputs.Add(new CCOLIOElement(fc.WachttijdVoorspellerBitmapData, $"{_uspf}{_uswtv}{fc.Naam}"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtv}{fc.Naam}", _uswtv, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mwtv}{fc.Naam}", _mwtv, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_twtv}{fc.Naam}", 999, CCOLElementTimeTypeEnum.TE_type, _twtv, fc.Naam));
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

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCInitApplication:
                    return 20;
                case CCOLCodeTypeEnum.RegCTop:
                    return 50;
                case CCOLCodeTypeEnum.RegCIncludes:
                    return 30;
                case CCOLCodeTypeEnum.RegCSystemApplication:
                    return 90;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:
                    sb.AppendLine("/* Include files wachttijdvoorspeller*/");
                    sb.AppendLine("#include \"wtvfunc.c\" /* berekening van de wachttijden voorspelling */");
                    sb.AppendLine("#include \"wtlleds.c\" /* aansturing van de wachttijdlantaarn met leds */");
                    sb.AppendLine("#if !defined AUTOMAAT && !defined NO_WTV_WIN");
                    sb.AppendLine($"{ts}#include \"wtv_testwin.c\"");
                    sb.AppendLine("#endif");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCTop:
                    sb.AppendLine("/* tijden t.b.v. wachttijdvoorspellers */");
                    sb.AppendLine("/* ----------------------------------- */");
                    sb.AppendLine("mulv t_wacht[FCMAX];	/* wachttijd'*/");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCInitApplication:
                    sb.AppendLine("#if !defined AUTOMAAT && !defined NO_WTV_WIN");
                    sb.AppendLine($"{ts}extrawin_init(SYSTEM);");
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}extrawin_add_fc({_fcpf}{fc.Naam}, NG, TYPE_LEDS);");
                    }
                    sb.AppendLine("#endif");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSystemApplication:
                    sb.AppendLine($"{ts}/* Wachttijdvoorspellers */");
                    sb.AppendLine($"{ts}/* bereken de primaire wachttijd van alle richtingen */");
                    sb.AppendLine($"{ts}max_wachttijd_modulen_primair(PRML, ML, ML_MAX, t_wacht);");
                    sb.AppendLine($"{ts}");
                    sb.AppendLine($"{ts}/* bereken de alternatieve wachttijd van de fietsrichtingen */");
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}max_wachttijd_alternatief({_fcpf}{fc.Naam}, t_wacht);");
                    }
                    sb.AppendLine($"{ts}");
                    sb.AppendLine($"{ts}/* aansturing wachttijd lantaarns */");
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}wachttijd_leds_mm({_fcpf}{fc.Naam}, {_mpf}{_mwtv}{fc.Naam}, {_tpf}{_twtv}{fc.Naam}, t_wacht[{_fcpf}{fc.Naam}], PRM[{_prmpf}{_prmminwtv}]);");
                    }
                    sb.AppendLine($"{ts}");
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}]= MM[{_mpf}{_mwtv}{fc.Naam}];");
                    }
                    sb.AppendLine($"{ts}");

                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        var gss = c.InterSignaalGroep.Gelijkstarten.Where(x => x.FaseVan == fc.Naam);
                        if(gss.Any())
                        {
                            foreach(var gs in gss)
                            {
                                sb.AppendLine($"{ts}wachttijd_correctie_gelijkstart({_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, t_wacht);");
                            }
                        }
                    }
                    sb.AppendLine($"{ts}");

                    sb.AppendLine("#if !defined AUTOMAAT && !defined NO_WTV_WIN");
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}extrawin_wtv({_fcpf}{fc.Naam}, {_mpf}{_mwtv}{fc.Naam});");
                    }
                    sb.AppendLine("#endif");
                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            //_mperiod = CCOLGeneratorSettingsProvider.Default.GetElementName("mperiod");

            return base.SetSettings(settings);
        }
    }
}
