using System;
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
        private CCOLGeneratorCodeStringSettingModel _uswtvbit;
        private CCOLGeneratorCodeStringSettingModel _uswtvbus;
        private CCOLGeneratorCodeStringSettingModel _schwtv;
        private CCOLGeneratorCodeStringSettingModel _hwtv;
        private CCOLGeneratorCodeStringSettingModel _twtv;
        private CCOLGeneratorCodeStringSettingModel _mwtv;
        private CCOLGeneratorCodeStringSettingModel _mwtvm;
        private CCOLGeneratorCodeStringSettingModel _prmminwtv;
        private CCOLGeneratorCodeStringSettingModel _schwtvbusbijhd;
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
        private string _hplact;
        private string _hpeltegenh;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if (!c.Fasen.Any(x => x.WachttijdVoorspeller)) return;

            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmminwtv}", 2, CCOLElementTimeTypeEnum.TE_type, _prmminwtv));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmwtvnhaltmax}", c.Data.WachttijdvoorspellerNietHalterenMax, CCOLElementTimeTypeEnum.None, _prmwtvnhaltmax));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmwtvnhaltmin}", c.Data.WachttijdvoorspellerNietHalterenMin, CCOLElementTimeTypeEnum.None, _prmwtvnhaltmin));

            foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
            {
                var wtvElem = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtv}{fc.Naam}", _uswtv, fc.WachttijdVoorspellerBitmapData, fc.Naam);
                wtvElem.IOMultivalent = true;
                _myElements.Add(wtvElem);
                
                if (c.Data.WachttijdvoorspellerAansturenBus && 
                    c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtvbus}{fc.Naam}", _uswtvbus, fc.WachttijdVoorspellerBusBitmapData, fc.Naam));
                }

                if (c.Data.CCOLVersie <= CCOLVersieEnum.CCOL110)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtv}{fc.Naam}0", _uswtvbit, fc.WachttijdVoorspellerBitmapData0, fc.Naam));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtv}{fc.Naam}1", _uswtvbit, fc.WachttijdVoorspellerBitmapData1, fc.Naam));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtv}{fc.Naam}2", _uswtvbit, fc.WachttijdVoorspellerBitmapData2, fc.Naam));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtv}{fc.Naam}3", _uswtvbit, fc.WachttijdVoorspellerBitmapData3, fc.Naam));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uswtv}{fc.Naam}4", _uswtvbit, fc.WachttijdVoorspellerBitmapData4, fc.Naam));
                }

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schwtv}{fc.Naam}", 1, CCOLElementTimeTypeEnum.SCH_type, _schwtv, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hwtv}{fc.Naam}", _hwtv, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mwtv}{fc.Naam}", _mwtv, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mwtvm}{fc.Naam}", _mwtvm, fc.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_twtv}{fc.Naam}", 999, CCOLElementTimeTypeEnum.TE_type, _twtv, fc.Naam));
            }
            
            if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL120)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schwtvbusbijhd}", c.Data.WachttijdvoorspellerAansturenBusHD ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schwtvbusbijhd));
            }
        }

        public override bool HasCCOLElements() => true;

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCInitApplication => new []{20},
                CCOLCodeTypeEnum.RegCPreApplication => new []{90},
                CCOLCodeTypeEnum.RegCTop => new []{50},
                CCOLCodeTypeEnum.RegCIncludes => new []{30},
                CCOLCodeTypeEnum.RegCSystemApplication => new []{90, 190},
                CCOLCodeTypeEnum.PrioCTegenhoudenConflicten => new []{30},
                _ => null
            };
        }

        private string GetFaseReeks(ControllerModel c, string fc)
        {
            if (!c.Data.MultiModuleReeksen) return "";
            var reeks = "";
            var mlr = c.MultiModuleMolens.FirstOrDefault(x => x.Modules.Any(x2 => x2.Fasen.Any(x3 => x3.FaseCyclus == fc)));
            if (mlr != null) reeks = "_" + mlr.Reeks;
            return reeks;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:
                    if (!c.Fasen.Any(x => x.WachttijdVoorspeller)) return "";
                    
                    sb.AppendLine("/* Include files wachttijdvoorspeller*/");
                    sb.AppendLine("#include \"wtvfunc.c\" /* berekening van de wachttijden voorspelling */");
                    sb.AppendLine("#include \"wtlleds.c\" /* aansturing van de wachttijdlantaarn met leds */");
                    
                    if (c.Data.WachttijdvoorspellerVensterTestomgeving)
                    {
                        sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) && !defined NO_WTV_WIN");
                        sb.AppendLine($"{ts}#include \"wtv_testwin.c\"");
                        sb.AppendLine("#endif");
                    }

                    if (c.HalfstarData.IsHalfstar)
                    {
                        sb.AppendLine($"#include \"halfstar_wtv.c\"");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPreApplication:

                    #region check de combinatie van de wtv met peloton koppelingen
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any())
                    {
                        var com = false;
                        foreach (var sg in c.Fasen.Where(x => x.WachttijdVoorspeller))
                        {
                            foreach (var sgpl in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                            {
                                if (TLCGenIntegrityChecker.IsFasenConflicting(c, sg.Naam, sgpl.GekoppeldeSignaalGroep))
                                {
                                    if (!com)
                                    {
                                        sb.AppendLine($"{ts}/* tegenhouden aansturing RW voor pelotonkoppelingen bij minimaal aantal leds */");
                                        com = true;
                                    }
                                    sb.AppendLine($"{ts}{ts}if (MM[{_mpf}{_mwtvm}{sg.Naam}] && MM[{_mpf}{_mwtvm}{sg.Naam}] <= PRM[{_prmpf}{_prmwtvnhaltmin}]) IH[{_hpf}{_hpeltegenh}{sgpl.KoppelingNaam}] = TRUE;");
                                }
                            }
                        }
                    }
                    #endregion
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCTop:
                    if (!c.Fasen.Any(x => x.WachttijdVoorspeller)) return "";
                    sb.AppendLine("/* tijden t.b.v. wachttijdvoorspellers */");
                    sb.AppendLine("/* ----------------------------------- */");
                    if (c.Data.MultiModuleReeksen)
                    {
                        foreach (var r in c.MultiModuleMolens.Where(x => x.Modules.Any(x2 => x2.Fasen.Any())))
                        {
                            sb.AppendLine($"mulv t_wacht_{r.Reeks}[FCMAX] = {{ 0 }}; /* berekende wachttijd {r.Reeks} */");
                            sb.AppendLine($"mulv t_wacht_old_{r.Reeks}[FCMAX] = {{ 0 }}; /* vorige berekende wachttijd {r.Reeks} */");
                            sb.AppendLine($"mulv t_wacht_halt_{r.Reeks}[FCMAX] = {{ 0 }}; /* berekende wachttijd {r.Reeks} */");
                        }
                    }
                    else
                    {
                        sb.AppendLine("mulv t_wacht[FCMAX] = { 0 }; /* berekende wachttijd */");
                        sb.AppendLine("mulv t_wacht_old[FCMAX] = { 0 }; /* vorige berekende wachttijd */");
                        sb.AppendLine("mulv t_wacht_halt[FCMAX] = { 0 }; /* gehalteerde berekende wachttijd */");
                    }
                    if (c.Data.MultiModuleReeksen)
                    {
                        foreach(var r in c.MultiModuleMolens.Where(x => x.Modules.Any(x2 => x2.Fasen.Any())))
                        {
                            sb.AppendLine($"mulv rr_twacht_{r.Reeks}[FCMAX]; /* halteren wachttijd {r.Reeks} */");
                        }
                    }
                    else
                    {
                        sb.AppendLine("mulv rr_twacht[FCMAX]; /* halteren wachttijd */");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCInitApplication:
                    if (!c.Fasen.Any(x => x.WachttijdVoorspeller)) return "";
                    if (c.Data.WachttijdvoorspellerVensterTestomgeving)
                    {
                        sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) && !defined NO_WTV_WIN");
                        sb.AppendLine($"{ts}extrawin_init(SYSTEM);");
                        foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                        {
                            if (c.Data.WachttijdvoorspellerAansturenBus && c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen)
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
                    }
                    sb.AppendLine($"{ts}/* Aansturing hulpelement aansturing wachttijdvoorspellers */");
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}IH[{_hpf}{_hwtv}{fc.Naam}] = SCH[{_schpf}{_schwtv}{fc.Naam}];");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSystemApplication:

                    if (!c.Fasen.Any(x => x.WachttijdVoorspeller)) return "";

                    if (order == 90)
                    {
                        sb.AppendLine($"{ts}/* Wachttijdvoorspellers */");
                        sb.AppendLine();

                        if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.InterFunc)
                        {

                            #region t_wacht_old

                            foreach (var sgWt in c.Fasen.Where(x => x.WachttijdVoorspeller))
                            {
                                sb.AppendLine($"{ts}t_wacht_old[{_fcpf}{sgWt.Naam}] = t_wacht[{_fcpf}{sgWt.Naam}];");
                            }

                            #endregion

                            #region t_wacht_AR

                            foreach (var sgWt in c.Fasen.Where(x => x.WachttijdVoorspeller))
                            {
                                sb.AppendLine(
                                    $"{ts}t_wacht[{_fcpf}{sgWt.Naam}] = (AR[{_fcpf}{sgWt.Naam}] && (twacht_AR[{_fcpf}{sgWt.Naam}] < twacht[{_fcpf}{sgWt.Naam}]) || (twacht[{_fcpf}{sgWt.Naam}] < 0)) ? twacht_AR[{_fcpf}{sgWt.Naam}] : twacht[{_fcpf}{sgWt.Naam}];");
                            }

                            #endregion

                            #region

                            foreach (var sgWt in c.Fasen.Where(x => x.WachttijdVoorspeller))
                            {
                                sb.AppendLine(
                                    $"{ts}if ((t_wacht_old[{_fcpf}{sgWt.Naam}] < t_wacht[{_fcpf}{sgWt.Naam}]) && CIF_GUS[{_uspf}{_uswtv}{sgWt.Naam}] && (t_wacht_old[{_fcpf}{sgWt.Naam}] > 0))");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine(
                                    $"{ts}{ts}t_wacht_halt[{_fcpf}{sgWt.Naam}] = t_wacht_old[{_fcpf}{sgWt.Naam}];");
                                sb.AppendLine($"{ts}{ts}rr_twacht[{_fcpf}{sgWt.Naam}] = TRUE;");
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"{ts}else");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine(
                                    $"{ts}{ts}if (t_wacht[{_fcpf}{sgWt.Naam}] <= t_wacht_halt[{_fcpf}{sgWt.Naam}])");
                                sb.AppendLine($"{ts}{ts}{{");
                                sb.AppendLine($"{ts}{ts}{ts}rr_twacht[{_fcpf}{sgWt.Naam}] = FALSE;");
                                sb.AppendLine($"{ts}{ts}}}");
                                sb.AppendLine($"{ts}}}");
                            }

                            #endregion

                            #region

                            foreach (var sgWt in c.Fasen.Where(x => x.WachttijdVoorspeller))
                            {
                                sb.AppendLine($"{ts}if (rr_twacht[{_fcpf}{sgWt.Naam}])");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine(
                                    $"{ts}{ts}wachttijd_leds_mm({_fcpf}{sgWt.Naam}, {_mpf}{_mwtv}{sgWt.Naam}, {_tpf}{_twtv}{sgWt.Naam}, t_wacht_halt[{_fcpf}{sgWt.Naam}], PRM[{_prmpf}{_prmminwtv}]);");
                                sb.AppendLine($"{ts}{ts}RT[{_tpf}{_twtv}{sgWt.Naam}] = TRUE;");
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"{ts}else");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine(
                                    $"{ts}{ts}wachttijd_leds_mm({_fcpf}{sgWt.Naam}, {_mpf}{_mwtv}{sgWt.Naam}, {_tpf}{_twtv}{sgWt.Naam}, t_wacht[{_fcpf}{sgWt.Naam}], PRM[{_prmpf}{_prmminwtv}]);");
                                sb.AppendLine($"{ts}}}");
                            }

                            #endregion

                            #region

                            sb.AppendLine($"{ts}/* laatste ledje laten knipperen bij ov/hd-ingreep of fixatie */");
                            foreach (var sgWt in c.Fasen.Where(x => x.WachttijdVoorspeller))
                            {
                                sb.AppendLine(
                                    $"{ts}wachttijd_leds_knip({_fcpf}{sgWt.Naam}, {_mpf}{_mwtv}{sgWt.Naam}, {_mpf}{_mwtvm}{sgWt.Naam}, rr_twacht[{_fcpf}{sgWt.Naam}], {_ispf}{_isfix});");
                            }

                            #endregion

                            #region

                            sb.AppendLine($"{ts}/* beveiliging op afzetten tijdens bedrijf */");
                            foreach (var sgWt in c.Fasen.Where(x => x.WachttijdVoorspeller))
                            {
                                sb.AppendLine(
                                    $"{ts}if (G[{_fcpf}{sgWt.Naam}]) IH[{_hpf}{_hwtv}{sgWt.Naam}] = SCH[{_schpf}{_schwtv}{sgWt.Naam}];");
                            }

                            #endregion

                            #region

                            foreach (var sgWt in c.Fasen.Where(x => x.WachttijdVoorspeller))
                            {
                                sb.AppendLine($"{ts}/* Aansturen wachttijdlantaarn fase {_fcpf}{sgWt.Naam} */");
                                sb.AppendLine($"{ts}if (IH[{_hpf}{_hwtv}{sgWt.Naam}] && R[{_fcpf}{sgWt.Naam}])");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_uswtv}{sgWt.Naam}] = MM[{_mpf}{_mwtvm}{sgWt.Naam}];");
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"{ts}else");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_uswtv}{sgWt.Naam}] = 0;");
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{sgWt.Naam}] &= ~BIT8;");
                                sb.AppendLine($"{ts}if (CIF_GUS[{_uspf}{_uswtv}{sgWt.Naam}] && rr_twacht[{_fcpf}{sgWt.Naam}] && IH[{_hpf}{_hwtv}{sgWt.Naam}] && (SCH[{_schpf}{_schwtvbusbijhd}] || !(RTFB & PRIO_RTFB_BIT)))");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_uswtv}{sgWt.Naam}] |= BIT8;");
                                sb.AppendLine($"{ts}}}");
                            }

                            #endregion
                        }

                        if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.InterFunc)
                        {

                            #region verlenggroentijd gekoppelde richtingen

                            sb.AppendLine($"{ts}/* verlenggroentijd gekoppelde richtingen */");
                            foreach (var nl in c.InterSignaalGroep.Nalopen)
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

                                sb.AppendLine(
                                    $"{ts}TVG_max[{_fcpf}{nl.FaseNaar}] = T_max[{_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar}] > TVG_max[{_fcpf}{nl.FaseNaar}] ? T_max[{_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar}] : TVG_max[{_fcpf}{nl.FaseNaar}];");
                            }

                            sb.AppendLine();

                            #endregion

                            #region bereken de primaire wachttijd van alle richtingen

                            sb.AppendLine($"{ts}/* bereken de primaire wachttijd van alle richtingen */");
                            if (!c.Data.MultiModuleReeksen)
                            {
                                sb.AppendLine($"{ts}max_wachttijd_modulen_primair(PRML, ML, ML_MAX, t_wacht);");
                            }
                            else
                            {
                                foreach (var r in c.MultiModuleMolens.Where(x => x.Modules.Any(x2 => x2.Fasen.Any())))
                                {
                                    sb.AppendLine(
                                        $"{ts}max_wachttijd_modulen_primair(PR{r.Reeks}, {r.Reeks}, {r.Reeks}_MAX, t_wacht_{r.Reeks});");
                                }
                            }

                            sb.AppendLine();

                            #endregion

                            #region bereken de alternatieve wachttijd van de richtingen met wachttijdvoorspeller

                            sb.AppendLine(
                                $"{ts}/* bereken de alternatieve wachttijd van de richtingen met wachttijdvoorspeller */");
                            foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                            {
                                sb.AppendLine(
                                    $"{ts}max_wachttijd_alternatief({_fcpf}{fc.Naam}, t_wacht{GetFaseReeks(c, fc.Naam)});");
                            }

                            sb.AppendLine();

                            #endregion

                            #region Wachttijdvoorspeller aansturing tijdens halfstar regelen

                            if (c.HalfstarData.IsHalfstar)
                            {
                                sb.AppendLine($"{ts}/* Berekening wachttijd tijdens halfstar regelen */");
                                if (c.Data.MultiModuleReeksen)
                                {
                                    foreach (var r in c.MultiModuleMolens.Where(
                                                 x => x.Modules.Any(x2 => x2.Fasen.Any())))
                                    {
                                        sb.AppendLine(
                                            $"{ts}max_wachttijd_halfstar(t_wacht_{r.Reeks}, {_hpf}{_hplact}, PL);");
                                    }
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}max_wachttijd_halfstar(t_wacht, {_hpf}{_hplact}, PL);");
                                }

                                sb.AppendLine();
                            }

                            #endregion

                            #region corrigeer waarde i.v.m. gelijkstart fietsers

                            var start = false;
                            foreach (var fc in c.Fasen)
                            {
                                var gss = c.InterSignaalGroep.Gelijkstarten.Where(x => x.FaseVan == fc.Naam);
                                if (gss.Any())
                                {
                                    if (!start)
                                    {
                                        start = true;
                                        sb.AppendLine($"{ts}/* corrigeer waarde i.v.m. gelijkstart fietsers */");
                                    }

                                    foreach (var gs in gss)
                                    {
                                        sb.AppendLine(
                                            $"{ts}wachttijd_correctie_gelijkstart({_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, t_wacht{GetFaseReeks(c, fc.Naam)});");
                                    }
                                }
                            }

                            sb.AppendLine();

                            #endregion

                            #region check of richting wordt tegengehouden door OV/HD

                            sb.AppendLine($"{ts}/* check of richting wordt tegengehouden door OV/HD */");
                            if (!c.Data.MultiModuleReeksen)
                            {
                                sb.AppendLine($"{ts}rr_modulen_primair(PRML, ML, ML_MAX, rr_twacht);");
                            }
                            else
                            {
                                foreach (var r in c.MultiModuleMolens.Where(x => x.Modules.Any(x2 => x2.Fasen.Any())))
                                {
                                    sb.AppendLine(
                                        $"{ts}rr_modulen_primair(PR{r.Reeks}, {r.Reeks}, {r.Reeks}_MAX, rr_twacht_{r.Reeks});");
                                }
                            }

                            sb.AppendLine();

                            #endregion

                            #region check de combinatie van de wtv met peloton koppelingen

                            if (c.PelotonKoppelingenData.PelotonKoppelingen.Any())
                            {
                                var com = false;
                                foreach (var sg in c.Fasen.Where(x => x.WachttijdVoorspeller))
                                {
                                    foreach (var sgpl in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x =>
                                                 x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                                    {
                                        if (TLCGenIntegrityChecker.IsFasenConflicting(c, sg.Naam,
                                                sgpl.GekoppeldeSignaalGroep))
                                        {
                                            if (!com)
                                            {
                                                sb.AppendLine(
                                                    $"{ts}/* halteren wachttijdvoorspellers tijdens RW BIT12 bij conflicten (peloton koppeling) */");
                                                com = true;
                                            }

                                            sb.AppendLine(
                                                $"{ts}if (RW[{_fcpf}{sgpl.GekoppeldeSignaalGroep}] & BIT12) rr_twacht{GetFaseReeks(c, sg.Naam)}[{_fcpf}{sg.Naam}] = TRUE;");
                                        }
                                    }
                                }

                                if (com) sb.AppendLine();
                            }

                            #endregion

                            return sb.ToString();
                        }
                    }
                    else if (order == 190)
                    {
                        #region Eventuele correctie op berekende wachttijd door gebruiker
                        sb.AppendLine($"{ts}/* Eventuele correctie op berekende wachttijd door gebruiker */");
                        sb.AppendLine($"{ts}WachttijdvoorspellersWachttijd_Add();");
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
                            sb.AppendLine($"{tts}if (!MM[{_mpf}{_mwtv}{fc.Naam}] || MM[{_mpf}{_mwtv}{fc.Naam}] >= PRM[{_prmpf}{_prmwtvnhaltmax}] || MM[{_mpf}{_mwtv}{fc.Naam}] <= PRM[{_prmpf}{_prmwtvnhaltmin}]) rr_twacht{GetFaseReeks(c, fc.Naam)}[{_fcpf}{fc.Naam}] = 0;");
                        }
                        foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                        {
                            sb.AppendLine($"{tts}if (rr_twacht{GetFaseReeks(c, fc.Naam)}[{_fcpf}{fc.Naam}] < 1 || G[{_fcpf}{fc.Naam}]) wachttijd_leds_mm({_fcpf}{fc.Naam}, {_mpf}{_mwtv}{fc.Naam}, {_tpf}{_twtv}{fc.Naam}, t_wacht{GetFaseReeks(c, fc.Naam)}[{_fcpf}{fc.Naam}], PRM[{_prmpf}{_prmminwtv}]);");
                        }
                        if (c.Data.FixatieMogelijk)
                        {
                            sb.AppendLine($"{ts}}}");
                        }
                        sb.AppendLine();

                        sb.AppendLine($"{ts}/* laatste ledje laten knipperen bij ov/hd-ingreep of fixatie */");
                        foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                        {
                            sb.Append($"{ts}wachttijd_leds_knip({_mpf}{_mwtv}{fc.Naam}, {_mpf}{_mwtvm}{fc.Naam}, rr_twacht{GetFaseReeks(c, fc.Naam)}[{_fcpf}{fc.Naam}], ");
                            if (c.Data.FixatieData.FixatieMogelijk) sb.Append($"{_ispf}{_isfix}");
                            else sb.Append("NG");
                            sb.AppendLine($");");
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
                            var reeks = GetFaseReeks(c, fc.Naam);
                            sb.AppendLine($"{ts}/* Aansturen wachttijdlantaarn fase {fc.Naam} */");
                            if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL120)
                            {
                                sb.AppendLine($"{ts}if (IH[{_hpf}{_hwtv}{fc.Naam}] && R[{_fcpf}{fc.Naam}])");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}] = MM[{_mpf}{_mwtvm}{fc.Naam}];");
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"{ts}else");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}] = 0;");
                                sb.AppendLine($"{ts}}}");

                                if (c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen)
                                {
                                    sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}] &= ~BIT8;");
                                    sb.AppendLine(
                                        $"{ts}if (CIF_GUS[{_uspf}{_uswtv}{fc.Naam}] && {(c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.InterFunc ? $"(RR[{_fcpf}{fc.Naam}] & BIT6) && " : "")}" +
                                        $"rr_twacht{reeks}[{_fcpf}{fc.Naam}] && IH[{_hpf}{_hwtv}{fc.Naam}] && " +
                                        $"(SCH[{_schpf}{_schwtvbusbijhd}] || !(bHDAanwezig)))");
                                    sb.AppendLine($"{ts}{{");
                                    sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}] |= BIT8;");
                                    sb.AppendLine($"{ts}}}");
                                }
                            }
                            else
                            {
                                sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}] = MM[{_mpf}{_mwtvm}{fc.Naam}];");
                                if (c.Data.WachttijdvoorspellerAansturenBus && c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen)
                                {
                                    if (!c.Data.WachttijdvoorspellerAansturenBusHD)
                                    {
                                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtvbus}{fc.Naam}] = CIF_GUS[{_uspf}{_uswtv}{fc.Naam}] && (RR[{_fcpf}{fc.Naam}] & BIT6) && rr_twacht{reeks}[{_fcpf}{fc.Naam}] && !(bHDAanwezig);");
                                    }
                                    else
                                    {
                                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtvbus}{fc.Naam}] = CIF_GUS[{_uspf}{_uswtv}{fc.Naam}] && (RR[{_fcpf}{fc.Naam}] & BIT6) && rr_twacht{reeks}[{_fcpf}{fc.Naam}];");
                                    }
                                }

                                sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}0] = (MM[{_mpf}{_mwtvm}{fc.Naam}] & BIT0) && IH[{_hpf}{_hwtv}{fc.Naam}] ? TRUE : FALSE;");
                                sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}1] = (MM[{_mpf}{_mwtvm}{fc.Naam}] & BIT1) && IH[{_hpf}{_hwtv}{fc.Naam}] ? TRUE : FALSE;");
                                sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}2] = (MM[{_mpf}{_mwtvm}{fc.Naam}] & BIT2) && IH[{_hpf}{_hwtv}{fc.Naam}] ? TRUE : FALSE;");
                                sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}3] = (MM[{_mpf}{_mwtvm}{fc.Naam}] & BIT3) && IH[{_hpf}{_hwtv}{fc.Naam}] ? TRUE : FALSE;");
                                sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uswtv}{fc.Naam}4] = (MM[{_mpf}{_mwtvm}{fc.Naam}] & BIT4) && IH[{_hpf}{_hwtv}{fc.Naam}] ? TRUE : FALSE;");
                            }

                            sb.AppendLine();
                        }

                        #endregion

                        #region aansturen test window
                        if (c.Data.WachttijdvoorspellerVensterTestomgeving)
                        {
                            sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST) && !defined NO_WTV_WIN");
                            foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                            {
                                sb.AppendLine($"{ts}extrawin_wtv({_fcpf}{fc.Naam}, {_mpf}{_mwtvm}{fc.Naam});");
                            }
                            sb.AppendLine("#endif");
                        }
                        #endregion

                        return sb.ToString();
                    }
                    return null;

                case CCOLCodeTypeEnum.PrioCTegenhoudenConflicten:
                    foreach (var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}if (MM[{_mpf}{_mwtvm}{fc.Naam}] && MM[{_mpf}{_mwtvm}{fc.Naam}] <= PRM[{_prmpf}{_prmwtvnhaltmin}])");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}RR[{_fcpf}{fc.Naam}] &= ~PRIO_RR_BIT;");
                        var items = new List<string>{ fc.Naam };
                        foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.FaseVan == fc.Naam))
                        {
                            var nlfc = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);
                            if (nlfc != null)
                            {
                                sb.AppendLine($"{ts}{ts}RR[{_fcpf}{nlfc.Naam}] &= ~PRIO_RR_BIT;");
                                items.Add(nlfc.Naam);
                            }
                        }
                        foreach (var nl in c.InterSignaalGroep.Gelijkstarten.Where(x => x.FaseVan == fc.Naam || x.FaseNaar == fc.Naam))
                        {
                            var gsNaar = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);
                            if (gsNaar != null)
                            {
                                if (!items.Contains(gsNaar.Naam))
                                {
                                    sb.AppendLine($"{ts}{ts}RR[{_fcpf}{gsNaar.Naam}] &= ~PRIO_RR_BIT;");
                                    items.Add(gsNaar.Naam);
                                }
                            }
                            var gsVan = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseVan);
                            if (gsVan != null)
                            {
                                if (!items.Contains(gsVan.Naam))
                                {
                                    sb.AppendLine($"{ts}{ts}RR[{_fcpf}{gsVan.Naam}] &= ~PRIO_RR_BIT;");
                                    items.Add(gsVan.Naam);
                                }
                            }
                        }
                        sb.AppendLine($"{ts}}}");
                    }

                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        sb.AppendLine($"{ts}if (RR[{_fcpf}{gs:van}] & PRIO_RR_BIT) RR[{_fcpf}{gs:naar}] |= PRIO_RR_BIT;");
                        sb.AppendLine($"{ts}if (RR[{_fcpf}{gs:naar}] & PRIO_RR_BIT) RR[{_fcpf}{gs:van}] |= PRIO_RR_BIT;");
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
            _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");
            _hpeltegenh = CCOLGeneratorSettingsProvider.Default.GetElementName("hpeltegenh");

            return base.SetSettings(settings);
        }
    }
}
