﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private string _usperdef;
        private string _usper;
        private string _prmstkp;
        private string _prmetkp;
        private string _prmdckp;
        private string _mperiod;
        private string _hperiod;
        private string _prmperrt;
        private string _prmperrta;
        private string _prmperrtdim;
        private string _prmperbel;
        private string _prmperbeldim;
        private string _prmpertwl;
        private string _prmpero;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyBitmapOutputs = new List<CCOLIOElement>();

            int iper = 1;
            int ipero = 1;
            int iperrt = 1;
            int iperrta = 1;
            int iperrtdim = 1;
            int iperbel = 1;
            int iperbeldim = 1;
            int ipertwl = 1;

            // outputs
            _MyElements.Add(new CCOLElement(_usperdef, CCOLElementTypeEnum.Uitgang));
            _MyBitmapOutputs.Add(new CCOLIOElement(c.PeriodenData.DefaultPeriodeBitmapData as IOElementModel, $"{_uspf}{_usperdef}"));
            foreach (var per in c.PeriodenData.Perioden)
            {
                switch(per.Type)
                {
                    case PeriodeTypeEnum.Groentijden:
                        _MyElements.Add(new CCOLElement(_usper + iper, CCOLElementTypeEnum.Uitgang));
                        _MyBitmapOutputs.Add(new CCOLIOElement(per.BitmapData as IOElementModel, $"{_uspf}{_usper}{iper++}"));
                        break;
                    case PeriodeTypeEnum.Overig:
                        _MyElements.Add(new CCOLElement(_usper + _prmpero + ipero, CCOLElementTypeEnum.Uitgang));
                        _MyBitmapOutputs.Add(new CCOLIOElement(per.BitmapData as IOElementModel, $"{_uspf}{_usper + _prmpero}{ipero++}"));
                        break;
                }
            }
#warning think about outputs for other periods: rt, bel, twl...

            // parameters
            iper = 1;
            ipero = 1;
            foreach (var per in c.PeriodenData.Perioden)
            {
                if (per.Type == PeriodeTypeEnum.Groentijden)
                {
                    var hours = per.StartTijd.Hours;
                    if (per.StartTijd.Days == 1)
                    {
                        hours = 24;
                    }
                    var inst = hours * 100 + per.StartTijd.Minutes;
                    _MyElements.Add(new CCOLElement($"{_prmstkp}{iper}", inst, CCOLElementTimeTypeEnum.TI_type, CCOLElementTypeEnum.Parameter));
                    hours = per.EindTijd.Hours;
                    if (per.EindTijd.Days == 1)
                    {
                        hours = 24;
                    }
                    inst = hours * 100 + per.EindTijd.Minutes;
                    _MyElements.Add(new CCOLElement($"{_prmetkp}{iper}", inst, CCOLElementTimeTypeEnum.TI_type, CCOLElementTypeEnum.Parameter));
                    _MyElements.Add(new CCOLElement($"{_prmdckp}{iper}", (int)per.DagCode, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    ++iper;
                }
            }

            foreach (var per in c.PeriodenData.Perioden)
            {
                if (per.Type == PeriodeTypeEnum.Groentijden)
                    continue;

                // get period params names
                string pertypeandnum = "";
                switch (per.Type)
                {
                    case PeriodeTypeEnum.RateltikkersAltijd: pertypeandnum = _prmperrt + iperrt.ToString(); iperrt++; break;
                    case PeriodeTypeEnum.RateltikkersAanvraag: pertypeandnum = _prmperrta + iperrta.ToString(); iperrta++; break;
                    case PeriodeTypeEnum.RateltikkersDimmen: pertypeandnum = _prmperrtdim + iperrtdim.ToString(); iperrtdim++; break;
                    case PeriodeTypeEnum.BellenActief: pertypeandnum = _prmperbel + iperbel.ToString(); iperbel++; break;
                    case PeriodeTypeEnum.BellenDimmen: pertypeandnum = _prmperbeldim + iperbeldim.ToString(); iperbeldim++; break;
                    case PeriodeTypeEnum.WaarschuwingsLichten: pertypeandnum = _prmpertwl + ipertwl.ToString(); ipertwl++; break;
                    case PeriodeTypeEnum.Overig: pertypeandnum = _prmpero + ipero.ToString(); break;
                }
                string stkp = _prmstkp + pertypeandnum;
                string etkp = _prmetkp + pertypeandnum;
                string dckp = _prmdckp + pertypeandnum;

                // period params
                var hours = per.StartTijd.Hours;
                if (per.StartTijd.Days == 1)
                {
                    hours = 24;
                }
                var inst = hours * 100 + per.StartTijd.Minutes;
                _MyElements.Add(new CCOLElement(stkp, inst, CCOLElementTimeTypeEnum.TI_type, CCOLElementTypeEnum.Parameter));
                hours = per.EindTijd.Hours;
                if (per.EindTijd.Days == 1)
                {
                    hours = 24;
                }
                inst = hours * 100 + per.EindTijd.Minutes;
                _MyElements.Add(new CCOLElement(etkp, inst, CCOLElementTimeTypeEnum.TI_type, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement(dckp, (int)per.DagCode, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));

                // free period helpelem
                if(per.Type == PeriodeTypeEnum.Overig)
                {
                    _MyElements.Add(new CCOLElement($"{_hperiod}{ipero++}", CCOLElementTypeEnum.HulpElement));
                }
            }
            if (iperrt > 1)     _MyElements.Add(new CCOLElement($"{_hperiod}{_prmperrt}", CCOLElementTypeEnum.HulpElement));
            if (iperrta > 1)    _MyElements.Add(new CCOLElement($"{_hperiod}{_prmperrta}", CCOLElementTypeEnum.HulpElement));
            if (iperrtdim > 1)  _MyElements.Add(new CCOLElement($"{_hperiod}{_prmperrtdim}", CCOLElementTypeEnum.HulpElement));
            if (iperbel > 1)    _MyElements.Add(new CCOLElement($"{_hperiod}{_prmperbel}", CCOLElementTypeEnum.HulpElement));
            if (iperbeldim > 1) _MyElements.Add(new CCOLElement($"{_hperiod}{_prmperbeldim}", CCOLElementTypeEnum.HulpElement));
            if (ipertwl > 1)    _MyElements.Add(new CCOLElement($"{_hperiod}{_prmpertwl}", CCOLElementTypeEnum.HulpElement));
            if (iperrt > 1)     _MyElements.Add(new CCOLElement($"{_usper}{_prmperrt}", CCOLElementTypeEnum.Uitgang));
            if (iperrta > 1)    _MyElements.Add(new CCOLElement($"{_usper}{_prmperrta}", CCOLElementTypeEnum.Uitgang));
            if (iperrtdim > 1)  _MyElements.Add(new CCOLElement($"{_usper}{_prmperrtdim}", CCOLElementTypeEnum.Uitgang));
            if (iperbel > 1)    _MyElements.Add(new CCOLElement($"{_usper}{_prmperbel}", CCOLElementTypeEnum.Uitgang));
            if (iperbeldim > 1) _MyElements.Add(new CCOLElement($"{_usper}{_prmperbeldim}", CCOLElementTypeEnum.Uitgang));
            if (ipertwl > 1)    _MyElements.Add(new CCOLElement($"{_usper}{_prmpertwl}", CCOLElementTypeEnum.Uitgang));
            if (iperrt > 1)     _MyBitmapOutputs.Add(new CCOLIOElement(c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersAltijd).First().BitmapData, $"{_uspf}{_usper}{_prmperrt}"));
            if (iperrta > 1)    _MyBitmapOutputs.Add(new CCOLIOElement(c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersAanvraag).First().BitmapData, $"{_uspf}{_usper}{_prmperrta}"));
            if (iperrtdim > 1)  _MyBitmapOutputs.Add(new CCOLIOElement(c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersDimmen).First().BitmapData, $"{_uspf}{_usper}{_prmperrtdim}"));
            if (iperbel > 1)    _MyBitmapOutputs.Add(new CCOLIOElement(c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.BellenActief).First().BitmapData, $"{_uspf}{_usper}{_prmperbel}"));
            if (iperbeldim > 1) _MyBitmapOutputs.Add(new CCOLIOElement(c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.BellenDimmen).First().BitmapData, $"{_uspf}{_usper}{_prmperbeldim}"));
            if (ipertwl > 1)    _MyBitmapOutputs.Add(new CCOLIOElement(c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.WaarschuwingsLichten).First().BitmapData, $"{_uspf}{_usper}{_prmpertwl}"));

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
            int iper;
            int iperrt;
            int iperrta;
            int iperrtdim;
            int iperbel;
            int iperbeldim;
            int ipertwl;
            int ipero;

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.KlokPerioden:
                    sb.AppendLine("void KlokPerioden(void)");
                    sb.AppendLine("{");
                    sb.AppendLine($"{ts}/* default klokperiode voor max.groen */");
                    sb.AppendLine($"{ts}/* ---------------------------------- */");
                    sb.AppendLine($"{ts}MM[{_mpf}{_mperiod}] = 0;");
                    sb.AppendLine();
                    iper = 1;
                    foreach (PeriodeModel kpm in c.PeriodenData.Perioden)
                    {
                        if (kpm.Type == PeriodeTypeEnum.Groentijden)
                        {
                            string comm = kpm.Commentaar;
                            if (comm == null) comm = "";
                            sb.AppendLine($"{ts}/* klokperiode: {comm} */");
                            sb.AppendLine($"{ts}/* -------------{new string('-', comm.Length)} */");
                            sb.AppendLine($"{ts}if (klokperiode(PRM[{_prmpf}{_prmstkp}{iper}], PRM[{_prmpf}{_prmetkp}{iper}]) &&");
                            sb.AppendLine($"{ts}    dagsoort(PRM[{_prmpf}{_prmdckp}{iper}]));");
                            sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mperiod}] = {iper};");
                            sb.AppendLine();
                            ++iper;
                        }
                    }
                    iperrt = 1;
                    iperrta = 1;
                    iperrtdim = 1;
                    iperbel = 1;
                    iperbeldim = 1;
                    ipertwl = 1;
                    ipero = 1;
                    foreach (PeriodeModel kpm in c.PeriodenData.Perioden)
                    {
                        if (kpm.Type == PeriodeTypeEnum.Overig)
                        {
                            string comm = kpm.Commentaar;
                            if (comm == null) comm = "";
                            sb.AppendLine($"{ts}/* vrije klokperiode: {comm} */");
                            sb.AppendLine($"{ts}/* -------------------{new string('-', comm.Length)} */");
                            sb.AppendLine($"{ts}if (klokperiode(PRM[{_prmpf}{_prmstkp}{_prmpero}{ipero}], PRM[{_prmpf}{_prmetkp}{_prmpero}{ipero}]) &&");
                            sb.AppendLine($"{ts}    dagsoort(PRM[{_prmpf}{_prmdckp}{_prmpero}{ipero}]));");
                            sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hperiod}{ipero}] = TRUE;");
                            sb.AppendLine();
                            ++ipero;
                        }
                    }
                    if (c.PeriodenData.Perioden.Count > 0)
                    {
                        if(c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersAltijd).Any())
                        {
                            sb.AppendLine($"{ts}/* klokperiode rateltikkers altijd */");
                            sb.AppendLine($"{ts}/* ------------------------------- */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperrt}] = ");
                            sb.AppendLine($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperrt}{iperrt}], PRM[{_prmpf}{_prmetkp}{_prmperrt}{iperrt}]) &&");
                            sb.AppendLine($"{ts}{ts}{ts}dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperrt}{iperrt}]));");
                            sb.AppendLine();
                            ++iperrt;
                        }
                        if (c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersAanvraag).Any())
                        {
                            sb.AppendLine($"{ts}/* klokperiode rateltikker op aanvraag */");
                            sb.AppendLine($"{ts}/* ----------------------------------- */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperrta}] = ");
                            sb.AppendLine($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperrta}{iperrta}], PRM[{_prmpf}{_prmetkp}{_prmperrta}{iperrta}]) &&");
                            sb.AppendLine($"{ts}{ts}{ts}dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperrta}{iperrta}]));");
                            sb.AppendLine();
                            ++iperrta;
                        }
                        if (c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersDimmen).Any())
                        {
                            sb.AppendLine($"{ts}/* klokperiode rateltikker dimmen */");
                            sb.AppendLine($"{ts}/* ------------------------------ */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperrtdim}] = ");
                            sb.AppendLine($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperrtdim}{iperrtdim}], PRM[{_prmpf}{_prmetkp}{_prmperrtdim}{iperrtdim}]) &&");
                            sb.AppendLine($"{ts}{ts}{ts}dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperrtdim}{iperrtdim}]));");
                            sb.AppendLine();
                            ++iperrtdim;
                        }
                        if (c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.BellenActief).Any())
                        {
                            sb.AppendLine($"{ts}/* klokperiode bellen actief */");
                            sb.AppendLine($"{ts}/* ------------------------- */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperbel}] = ");
                            sb.AppendLine($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperbel}{iperbel}], PRM[{_prmpf}{_prmetkp}{_prmperbel}{iperbel}]) &&");
                            sb.AppendLine($"{ts}{ts}{ts}dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperbel}{iperbel}]));");
                            sb.AppendLine();
                            ++iperbel;
                        }
                        if (c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.BellenDimmen).Any())
                        {
                            sb.AppendLine($"{ts}/* klokperiode bellen dimmen */");
                            sb.AppendLine($"{ts}/* ------------------------- */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperbeldim}] = ");
                            sb.AppendLine($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperbeldim}{iperbeldim}], PRM[{_prmpf}{_prmetkp}{_prmperbeldim}{iperbeldim}]) &&");
                            sb.AppendLine($"{ts}{ts}{ts}dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperbeldim}{iperbeldim}]));");
                            sb.AppendLine();
                            ++iperbeldim;
                        }
                        if (c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.WaarschuwingsLichten).Any())
                        {
                            sb.AppendLine($"{ts}/* klokperiode twl's actief */");
                            sb.AppendLine($"{ts}/* ------------------------ */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmpertwl}] = ");
                            sb.AppendLine($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmpertwl}{ipertwl}], PRM[{_prmpf}{_prmetkp}{_prmpertwl}{ipertwl}]) &&");
                            sb.AppendLine($"{ts}{ts}{ts}dagsoort(PRM[{_prmpf}{_prmdckp}{_prmpertwl}{ipertwl}]));");
                            sb.AppendLine();
                            ++ipertwl;
                        }
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
                    iper = 0;
                    sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usperdef}] = (MM[{_mpf}{_mperiod}] == {iper++});");
                    foreach (var per in c.PeriodenData.Perioden)
                    {
                        if(per.Type == PeriodeTypeEnum.Groentijden)
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{iper}] = (MM[{_mpf}{_mperiod}] == {iper++});");
                        }
                    }
                    if(c.PeriodenData.Perioden.Count > 0)
                    {
                        if(c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersAltijd).Any())
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{_prmperrt}] = (IH[{_hpf}{_hperiod}{_prmperrt}] == TRUE);");
                        }
                        if (c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersAanvraag).Any())
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{_prmperrta}] = (IH[{_hpf}{_hperiod}{_prmperrta}] == TRUE);");
                        }
                        if (c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersDimmen).Any())
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{_prmperrtdim}] = (IH[{_hpf}{_hperiod}{_prmperrtdim}] == TRUE);");
                        }
                        if (c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.BellenActief).Any())
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{_prmperbel}] = (IH[{_hpf}{_hperiod}{_prmperbel}] == TRUE);");
                        }
                        if (c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.BellenDimmen).Any())
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{_prmperbeldim}] = (IH[{_hpf}{_hperiod}{_prmperbeldim}] == TRUE);");
                        }
                        if (c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.WaarschuwingsLichten).Any())
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{_prmpertwl}] = (IH[{_hpf}{_hperiod}{_prmpertwl}] == TRUE);");
                        }
                    }
                    ipero = 1;
                    foreach (var per in c.PeriodenData.Perioden)
                    {
                        if (per.Type == PeriodeTypeEnum.Overig)
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{_prmpero}{ipero}] = (IH[{_hpf}{_hperiod}{ipero}] == TRUE);");
                        }
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
                switch(s.Default)
                {
                    case "perdef": _usperdef = s.Setting == null ? s.Default : s.Setting; break;
                    case "per": _usper = s.Setting == null ? s.Default : s.Setting; break;
                    case "stkp": _prmetkp = s.Setting == null ? s.Default : s.Setting; break;
                    case "etkp": _prmstkp = s.Setting == null ? s.Default : s.Setting; break;
                    case "dckp": _prmdckp = s.Setting == null ? s.Default : s.Setting; break;
                    case "period":
                        switch (s.Type)
                        {
                            case CCOLGeneratorSettingTypeEnum.GeheugenElement: _mperiod = s.Setting == null ? s.Default : s.Setting; break;
                            case CCOLGeneratorSettingTypeEnum.HulpElement: _hperiod = s.Setting == null ? s.Default : s.Setting; break;
                        }
                        break;
                    case "perrt": _prmperrt = s.Setting == null ? s.Default : s.Setting; break;
                    case "perrta": _prmperrta = s.Setting == null ? s.Default : s.Setting; break;
                    case "perrtdim": _prmperrtdim = s.Setting == null ? s.Default : s.Setting; break;
                    case "perbel": _prmperbel = s.Setting == null ? s.Default : s.Setting; break;
                    case "perbeldim": _prmperbeldim = s.Setting == null ? s.Default : s.Setting; break;
                    case "pertwl": _prmpertwl = s.Setting == null ? s.Default : s.Setting; break;
                    case "pero": _prmpero = s.Setting == null ? s.Default : s.Setting; break;
                }
            }

            return base.SetSettings(settings);
        }
    }
}
