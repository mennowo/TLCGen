using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Integrity;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    class WachttijdVoorspellerCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _prmwtvnhaltmax;
        private CCOLGeneratorCodeStringSettingModel _prmwtvnhaltmin;
        private CCOLGeneratorCodeStringSettingModel _uswtv;
        private CCOLGeneratorCodeStringSettingModel _uswtvbus;
        private CCOLGeneratorCodeStringSettingModel _schwtv;
        private CCOLGeneratorCodeStringSettingModel _hwtv;
        private CCOLGeneratorCodeStringSettingModel _twtv;
        private CCOLGeneratorCodeStringSettingModel _mwtv;
        private CCOLGeneratorCodeStringSettingModel _mwtvm;
        private CCOLGeneratorCodeStringSettingModel _prmminwtv;
#pragma warning restore 0649
        private string _isfix;
        private string _tnlsg;
        private string _tnlfg;
        private string _tnlcv;
        private string _tnleg;
        private string _tnlsgd;
        private string _tnlfgd;
        private string _tnlcvd;
        private string _tnlegd;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myBitmapOutputs = new List<CCOLIOElement>();

            if (!c.Fasen.Any(x => x.WachttijdVoorspeller)) return;

            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmminwtv}", 2, CCOLElementTimeTypeEnum.None, _prmminwtv));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmwtvnhaltmax}", c.Data.WachttijdvoorspellerNietHalterenMax, CCOLElementTimeTypeEnum.None, _prmwtvnhaltmax));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmwtvnhaltmin}", c.Data.WachttijdvoorspellerNietHalterenMin, CCOLElementTimeTypeEnum.None, _prmwtvnhaltmin));

            foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
            {
                _myBitmapOutputs.Add(new CCOLIOElement(fc.WachttijdVoorspellerBitmapData, $"{_uspf}{_uswtv}{fc.Naam}"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtv}{fc.Naam}", _uswtv, fc.Naam));
                if (c.Data.WachttijdvoorspellerAansturenBus)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtvbus}{fc.Naam}", _uswtvbus, fc.Naam));
                }
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtv}{fc.Naam}0", _uswtv, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtv}{fc.Naam}1", _uswtv, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtv}{fc.Naam}2", _uswtv, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtv}{fc.Naam}3", _uswtv, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtv}{fc.Naam}4", _uswtv, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schwtv}{fc.Naam}", 1, CCOLElementTimeTypeEnum.SCH_type, _schwtv, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hwtv}{fc.Naam}", _hwtv, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mwtv}{fc.Naam}", _mwtv, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mwtvm}{fc.Naam}", _mwtvm, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_twtv}{fc.Naam}", 999, CCOLElementTimeTypeEnum.TE_type, _twtv, fc.Naam));
            }
        }

        public override bool HasCCOLElements() => true;

        public override bool HasCCOLBitmapOutputs() => true;

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
                case CCOLCodeTypeEnum.OvCTegenhoudenConflicten:
                    return 10;
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
                    if (!c.Fasen.Any(x => x.WachttijdVoorspeller)) return "";
                    sb.AppendLine("/* Include files wachttijdvoorspeller*/");
                    sb.AppendLine("#include \"wtvfunc.c\" /* berekening van de wachttijden voorspelling */");
                    sb.AppendLine("#include \"wtlleds.c\" /* aansturing van de wachttijdlantaarn met leds */");
                    sb.AppendLine("#if !defined AUTOMAAT && !defined NO_WTV_WIN");
                    sb.AppendLine($"{ts}#include \"wtv_testwin.c\"");
                    sb.AppendLine("#endif");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCTop:
                    if (!c.Fasen.Any(x => x.WachttijdVoorspeller)) return "";
                    sb.AppendLine("/* tijden t.b.v. wachttijdvoorspellers */");
                    sb.AppendLine("/* ----------------------------------- */");
                    sb.AppendLine("mulv t_wacht[FCMAX]; /* berekende wachttijd */");
                    sb.AppendLine("mulv rr_twacht[FCMAX]; /* halteren wachttijd */");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCInitApplication:
                    if (!c.Fasen.Any(x => x.WachttijdVoorspeller)) return "";
                    sb.AppendLine("#if !defined AUTOMAAT && !defined NO_WTV_WIN");
                    sb.AppendLine($"{ts}extrawin_init(SYSTEM);");
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        if (c.Data.WachttijdvoorspellerAansturenBus)
                        {
                            sb.AppendLine($"{ts}extrawin_add_fc({_fcpf}{fc.Naam}, {_uspf}{_uswtvbus}{fc.Naam}, TYPE_LEDS);");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}extrawin_add_fc({_fcpf}{fc.Naam}, NG, TYPE_LEDS);");
                        }
                    }
                    sb.AppendLine("#endif");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Aansturing hulpelement aansturing wachttijdvoorspellers */");
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}IH[{_hpf}{_hwtv}{fc.Naam}] = SCH[{_schpf}{_schwtv}{fc.Naam}];");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSystemApplication:
                    if (!c.Fasen.Any(x => x.WachttijdVoorspeller)) return "";
                    sb.AppendLine($"{ts}/* Wachttijdvoorspellers */");
                    sb.AppendLine();

                    #region verlenggroentijd gekoppelde richtingen
                    sb.AppendLine($"{ts}/* verlenggroentijd gekoppelde richtingen */");
                    foreach(var nl in c.InterSignaalGroep.Nalopen)
                    {
                        #region Get naloop type timer
                        var tnl = "";
                        if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.VastGroenDetectie))
                        {
                            tnl = _tnlfgd;
                        }
                        else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.StartGroenDetectie))
                        {
                            tnl = _tnlsgd;
                        }
                        else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.EindeGroenDetectie))
                        {
                            tnl = _tnlegd;
                        }
                        else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.EindeVerlengGroenDetectie))
                        {
                            tnl = _tnlcvd;
                        }
                        else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.VastGroen))
                        {
                            tnl = _tnlfg;
                        }
                        else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.StartGroen))
                        {
                            tnl = _tnlsg;
                        }
                        else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.EindeGroen))
                        {
                            tnl = _tnleg;
                        }
                        else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.EindeVerlengGroen))
                        {
                            tnl = _tnlcv;
                        }
                        #endregion
                        sb.AppendLine($"{ts}TVG_max[{_fcpf}{nl.FaseNaar}] = T_max[{_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar}] > TVG_max[{_fcpf}{nl.FaseNaar}] ? T_max[{_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar}] : TVG_max[{_fcpf}{nl.FaseNaar}];");
                    }
                    sb.AppendLine();
                    #endregion

                    #region bereken de primaire wachttijd van alle richtingen
                    sb.AppendLine($"{ts}/* bereken de primaire wachttijd van alle richtingen */");
                    sb.AppendLine($"{ts}max_wachttijd_modulen_primair(PRML, ML, ML_MAX, t_wacht);");
                    sb.AppendLine();
                    #endregion

                    #region bereken de alternatieve wachttijd van de richtingen met wachttijdvoorspeller
                    sb.AppendLine($"{ts}/* bereken de alternatieve wachttijd van de richtingen met wachttijdvoorspeller */");
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}max_wachttijd_alternatief({_fcpf}{fc.Naam}, t_wacht);");
                    }
                    sb.AppendLine();
                    #endregion

                    #region corrigeer waarde i.v.m. gelijkstart fietsers
                    sb.AppendLine($"{ts}/* corrigeer waarde i.v.m. gelijkstart fietsers */");
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        var gss = c.InterSignaalGroep.Gelijkstarten.Where(x => x.FaseVan == fc.Naam);
                        if (gss.Any())
                        {
                            foreach (var gs in gss)
                            {
                                sb.AppendLine($"{ts}wachttijd_correctie_gelijkstart({_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, t_wacht);");
                            }
                        }
                    }
                    sb.AppendLine();
                    #endregion

                    #region Eventuele correctie op berekende wachttijd door gebruiker
                    sb.AppendLine($"{ts}/* Eventuele correctie op berekende wachttijd door gebruiker */");
                    sb.AppendLine($"{ts}WachtijdvoorspellersWachttijd_Add();");
                    sb.AppendLine();
                    #endregion

                    #region check of richting wordt tegengehouden door OV/HD
                    sb.AppendLine($"{ts}/* check of richting wordt tegengehouden door OV/HD */");
                    sb.AppendLine($"{ts}rr_modulen_primair(PRML, ML, ML_MAX, rr_twacht);");
                    sb.AppendLine();
                    #endregion

                    #region aansturing wachttijd lantaarns
                    sb.AppendLine($"{ts}/* aansturing wachttijd lantaarns (niet tijdens fixatie of prio ingreep) */");
                    var tts = ts;
                    if (c.Data.FixatieMogelijk)
                    {
                        sb.AppendLine($"{ts}if (!CIF_IS[{_ispf}{_isfix}])");
                        sb.AppendLine($"{ts}{{");
                        tts = ts + ts;
                    }
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{tts}if (!MM[{_mpf}{_mwtv}{fc.Naam}] || MM[{_mpf}{_mwtv}{fc.Naam}] >= PRM[{_prmpf}{_prmwtvnhaltmax}] || MM[{_mpf}{_mwtv}{fc.Naam}] <= PRM[{_prmpf}{_prmwtvnhaltmin}]) rr_twacht[{_fcpf}{fc.Naam}] = 0;");
                    }
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{tts}if (rr_twacht[{_fcpf}{fc.Naam}] < 1) wachttijd_leds_mm({_fcpf}{fc.Naam}, {_mpf}{_mwtv}{fc.Naam}, {_tpf}{_twtv}{fc.Naam}, t_wacht[{_fcpf}{fc.Naam}], PRM[{_prmpf}{_prmminwtv}]);");
                    }
                    if (c.Data.FixatieMogelijk)
                    {
                        sb.AppendLine($"{ts}}}");
                    }
                    sb.AppendLine();

                    sb.AppendLine($"{ts}/* laatste ledje laten knipperen bij ov/hd-ingreep of fixatie */");
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}wachttijd_leds_knip({_fcpf}{fc.Naam}, {_mpf}{_mwtv}{fc.Naam}, {_mpf}{_mwtvm}{fc.Naam}, rr_twacht[{_fcpf}{fc.Naam}]);");
                    }
                    sb.AppendLine();

                    sb.AppendLine($"{ts}/* beveiliging op afzetten tijdens bedrijf */");
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}if (G[{_fcpf}{fc.Naam}])  IH[{_hpf}{_hwtv}{fc.Naam}] = SCH[{_schpf}{_schwtv}{fc.Naam}];");
                    }
                    sb.AppendLine();

                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}/* Aansturen wachttijdlantaarn fase {fc.Naam} */");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}]= MM[{_mpf}{_mwtvm}{fc.Naam}];");
                        if (c.Data.WachttijdvoorspellerAansturenBus)
                        {
                            if (!c.Data.WachttijdvoorspellerAansturenBusHD)
                            {
                                sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtvbus}{fc.Naam}]= CIF_GUS[{_uspf}{_uswtv}{fc.Naam}] && (RR[{_fcpf}{fc.Naam}] & BIT6) && rr_twacht[{_fcpf}{fc.Naam}] && !(RTFB & OV_RTFB_BIT);");
                            }
                            else
                            {
                                sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtvbus}{fc.Naam}]= CIF_GUS[{_uspf}{_uswtv}{fc.Naam}] && (RR[{_fcpf}{fc.Naam}] & BIT6) && rr_twacht[{_fcpf}{fc.Naam}];");
                            }
                        }
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}0]= (MM[{_mpf}{_mwtvm}{fc.Naam}] & BIT0) && IH[{_hpf}{_hwtv}{fc.Naam}] ? TRUE : FALSE;");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}1]= (MM[{_mpf}{_mwtvm}{fc.Naam}] & BIT1) && IH[{_hpf}{_hwtv}{fc.Naam}] ? TRUE : FALSE;");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}2]= (MM[{_mpf}{_mwtvm}{fc.Naam}] & BIT2) && IH[{_hpf}{_hwtv}{fc.Naam}] ? TRUE : FALSE;");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}3]= (MM[{_mpf}{_mwtvm}{fc.Naam}] & BIT3) && IH[{_hpf}{_hwtv}{fc.Naam}] ? TRUE : FALSE;");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}4]= (MM[{_mpf}{_mwtvm}{fc.Naam}] & BIT4) && IH[{_hpf}{_hwtv}{fc.Naam}] ? TRUE : FALSE;");
                        sb.AppendLine();
                    }

                    #endregion

                    #region aansturen test window
                    sb.AppendLine("#if !defined AUTOMAAT && !defined NO_WTV_WIN");
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}extrawin_wtv({_fcpf}{fc.Naam}, {_mpf}{_mwtvm}{fc.Naam});");
                    }
                    sb.AppendLine("#endif");
                    #endregion

                    return sb.ToString();

                case CCOLCodeTypeEnum.OvCTegenhoudenConflicten:
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}if (MM[{_mpf}{_mwtvm}{fc.Naam}] && MM[{_mpf}{_mwtvm}{fc.Naam}] <= PRM[{_prmpf}{_prmwtvnhaltmin}])");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}RR[{_fcpf}{fc.Naam}] &= ~BIT6;");
                        foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.FaseVan == fc.Naam))
                        {
                            var nlfc = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);
                            if (nlfc != null)
                            {
                                sb.AppendLine($"{ts}{ts}RR[{_fcpf}{nlfc.Naam}] &= ~BIT6;");
                            }
                        }
                        foreach (var nl in c.InterSignaalGroep.Gelijkstarten.Where(x => x.FaseVan == fc.Naam))
                        {
                            var gsfc = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);
                            if (gsfc != null)
                            {
                                sb.AppendLine($"{ts}{ts}RR[{_fcpf}{gsfc.Naam}] &= ~BIT6;");
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
            _isfix = CCOLGeneratorSettingsProvider.Default.GetElementName("isfix");
            _tnlsg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsg");
            _tnlfg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfg");
            _tnleg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnleg");
            _tnlcv = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcv");
            _tnlsgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsgd");
            _tnlfgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfgd");
            _tnlegd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlegd");
            _tnlcvd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcvd");

            return base.SetSettings(settings);
        }
    }
}
