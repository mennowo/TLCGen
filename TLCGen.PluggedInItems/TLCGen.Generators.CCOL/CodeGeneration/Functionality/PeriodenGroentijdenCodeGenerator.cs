using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    [CCOLCodePieceGenerator]
    public class PeriodenGroentijdenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _myElements;
        private List<CCOLIOElement> _myBitmapOutputs;

#pragma warning disable 0649
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
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myBitmapOutputs = new List<CCOLIOElement>();

            var iper = 1;
            var ipero = 1;
            var iperrt = 1;
            var iperrta = 1;
            var iperrtdim = 1;
            var iperbel = 1;
            var iperbeldim = 1;
            var ipertwl = 1;

            // outputs
            _myElements.Add(new CCOLElement(_usperdef, CCOLElementTypeEnum.Uitgang));
            _myBitmapOutputs.Add(new CCOLIOElement(c.PeriodenData.DefaultPeriodeBitmapData, $"{_uspf}{_usperdef}"));
            foreach (var per in c.PeriodenData.Perioden)
            {
                switch(per.Type)
                {
                    case PeriodeTypeEnum.Groentijden:
                        _myElements.Add(new CCOLElement(_usper + iper, CCOLElementTypeEnum.Uitgang));
                        _myBitmapOutputs.Add(new CCOLIOElement(per.BitmapData, $"{_uspf}{_usper}{iper++}"));
                        break;
                    case PeriodeTypeEnum.Overig:
                        _myElements.Add(new CCOLElement(_usper + _prmpero + ipero, CCOLElementTypeEnum.Uitgang));
                        _myBitmapOutputs.Add(new CCOLIOElement(per.BitmapData, $"{_uspf}{_usper + _prmpero}{ipero++}"));
                        break;
                }
            }

            // parameters
            iper = 1;
            ipero = 1;
            foreach (var per in c.PeriodenData.Perioden)
            {
                if (per.Type != PeriodeTypeEnum.Groentijden) continue;
                var hours = per.StartTijd.Hours;
                if (per.StartTijd.Days == 1)
                {
                    hours = 24;
                }
                var inst = hours * 100 + per.StartTijd.Minutes;
                _myElements.Add(new CCOLElement($"{_prmstkp}{iper}", inst, CCOLElementTimeTypeEnum.TI_type, CCOLElementTypeEnum.Parameter));
                hours = per.EindTijd.Hours;
                if (per.EindTijd.Days == 1)
                {
                    hours = 24;
                }
                inst = hours * 100 + per.EindTijd.Minutes;
                _myElements.Add(new CCOLElement($"{_prmetkp}{iper}", inst, CCOLElementTimeTypeEnum.TI_type, CCOLElementTypeEnum.Parameter));
                _myElements.Add(new CCOLElement($"{_prmdckp}{iper}", (int)per.DagCode, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                ++iper;
            }

            foreach (var per in c.PeriodenData.Perioden)
            {
                if (per.Type == PeriodeTypeEnum.Groentijden)
                    continue;

                // get period params names
                var pertypeandnum = "";
                switch (per.Type)
                {
                    case PeriodeTypeEnum.RateltikkersAltijd: pertypeandnum = _prmperrt + iperrt.ToString(); iperrt++; break;
                    case PeriodeTypeEnum.RateltikkersAanvraag: pertypeandnum = _prmperrta + iperrta.ToString(); iperrta++; break;
                    case PeriodeTypeEnum.RateltikkersDimmen: pertypeandnum = _prmperrtdim + iperrtdim.ToString(); iperrtdim++; break;
                    case PeriodeTypeEnum.BellenActief: pertypeandnum = _prmperbel + iperbel.ToString(); iperbel++; break;
                    case PeriodeTypeEnum.BellenDimmen: pertypeandnum = _prmperbeldim + iperbeldim.ToString(); iperbeldim++; break;
                    case PeriodeTypeEnum.WaarschuwingsLichten: pertypeandnum = _prmpertwl + ipertwl.ToString(); ipertwl++; break;
                    case PeriodeTypeEnum.Overig: pertypeandnum = _prmpero + ipero.ToString(); break;
                    case PeriodeTypeEnum.Groentijden:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                var stkp = _prmstkp + pertypeandnum;
                var etkp = _prmetkp + pertypeandnum;
                var dckp = _prmdckp + pertypeandnum;

                // period params
                var hours = per.StartTijd.Hours;
                if (per.StartTijd.Days == 1)
                {
                    hours = 24;
                }
                var inst = hours * 100 + per.StartTijd.Minutes;
                _myElements.Add(new CCOLElement(stkp, inst, CCOLElementTimeTypeEnum.TI_type, CCOLElementTypeEnum.Parameter));
                hours = per.EindTijd.Hours;
                if (per.EindTijd.Days == 1)
                {
                    hours = 24;
                }
                inst = hours * 100 + per.EindTijd.Minutes;
                _myElements.Add(new CCOLElement(etkp, inst, CCOLElementTimeTypeEnum.TI_type, CCOLElementTypeEnum.Parameter));
                _myElements.Add(new CCOLElement(dckp, (int)per.DagCode, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));

                // free period helpelem
                if(per.Type == PeriodeTypeEnum.Overig)
                {
                    _myElements.Add(new CCOLElement($"{_hperiod}{ipero++}", CCOLElementTypeEnum.HulpElement));
                }
            }
            if (iperrt > 1)     _myElements.Add(new CCOLElement($"{_hperiod}{_prmperrt}", CCOLElementTypeEnum.HulpElement));
            if (iperrta > 1)    _myElements.Add(new CCOLElement($"{_hperiod}{_prmperrta}", CCOLElementTypeEnum.HulpElement));
            if (iperrtdim > 1)  _myElements.Add(new CCOLElement($"{_hperiod}{_prmperrtdim}", CCOLElementTypeEnum.HulpElement));
            if (iperbel > 1)    _myElements.Add(new CCOLElement($"{_hperiod}{_prmperbel}", CCOLElementTypeEnum.HulpElement));
            if (iperbeldim > 1) _myElements.Add(new CCOLElement($"{_hperiod}{_prmperbeldim}", CCOLElementTypeEnum.HulpElement));
            if (ipertwl > 1)    _myElements.Add(new CCOLElement($"{_hperiod}{_prmpertwl}", CCOLElementTypeEnum.HulpElement));
            if (iperrt > 1)     _myElements.Add(new CCOLElement($"{_usper}{_prmperrt}", CCOLElementTypeEnum.Uitgang));
            if (iperrta > 1)    _myElements.Add(new CCOLElement($"{_usper}{_prmperrta}", CCOLElementTypeEnum.Uitgang));
            if (iperrtdim > 1)  _myElements.Add(new CCOLElement($"{_usper}{_prmperrtdim}", CCOLElementTypeEnum.Uitgang));
            if (iperbel > 1)    _myElements.Add(new CCOLElement($"{_usper}{_prmperbel}", CCOLElementTypeEnum.Uitgang));
            if (iperbeldim > 1) _myElements.Add(new CCOLElement($"{_usper}{_prmperbeldim}", CCOLElementTypeEnum.Uitgang));
            if (ipertwl > 1)    _myElements.Add(new CCOLElement($"{_usper}{_prmpertwl}", CCOLElementTypeEnum.Uitgang));
            if (iperrt > 1)     _myBitmapOutputs.Add(new CCOLIOElement(c.Signalen.RatelTikkerAltijdBitmapData, $"{_uspf}{_usper}{_prmperrt}"));
            if (iperrta > 1)    _myBitmapOutputs.Add(new CCOLIOElement(c.Signalen.RatelTikkerActiefBitmapData, $"{_uspf}{_usper}{_prmperrta}"));
            if (iperrtdim > 1)  _myBitmapOutputs.Add(new CCOLIOElement(c.Signalen.RatelTikkerDimmenBitmapData, $"{_uspf}{_usper}{_prmperrtdim}"));
            if (iperbel > 1)    _myBitmapOutputs.Add(new CCOLIOElement(c.Signalen.BellenActiefBitmapData, $"{_uspf}{_usper}{_prmperbel}"));
            if (iperbeldim > 1) _myBitmapOutputs.Add(new CCOLIOElement(c.Signalen.BellenDimmenBitmapData, $"{_uspf}{_usper}{_prmperbeldim}"));
            if (ipertwl > 1)    _myBitmapOutputs.Add(new CCOLIOElement(c.Signalen.WaarschuwingsLichtenActiefBitmapData, $"{_uspf}{_usper}{_prmpertwl}"));

            // groentijden
            foreach (var mgset in c.GroentijdenSets)
            {
                foreach (var mgm in mgset.Groentijden)
                {
                    if (!mgm.Waarde.HasValue)
                        continue;

                    var thisfcm = c.Fasen.FirstOrDefault(fcm => fcm.Naam == mgm.FaseCyclus);

                    if (thisfcm == null)
                        throw new NullReferenceException($"Maxgroentijd voor niet bestaande fase {mgm.FaseCyclus} opgegeven.");

                    _myElements.Add(new CCOLElement(
                        $"{mgset.Naam.ToLower()}_{thisfcm.Naam}",
                        mgm.Waarde.Value,
                        CCOLElementTimeTypeEnum.TE_type, 
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
                case CCOLCodeTypeEnum.RegCKlokPerioden:
                    return 10;
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    return 20;
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                    return 20;
                case CCOLCodeTypeEnum.RegCSystemApplication:
                    return 50;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();
            int iper;
            int ipero;

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCKlokPerioden:
                    sb.AppendLine($"{ts}/* default klokperiode voor max.groen */");
                    sb.AppendLine($"{ts}/* ---------------------------------- */");
                    sb.AppendLine($"{ts}MM[{_mpf}{_mperiod}] = 0;");
                    sb.AppendLine();
                    iper = 1;
                    foreach (var kpm in c.PeriodenData.Perioden)
                    {
                        if (kpm.Type != PeriodeTypeEnum.Groentijden) continue;
                        var comm = kpm.Commentaar ?? "";
                        sb.AppendLine($"{ts}/* klokperiode: {comm} */");
                        sb.AppendLine($"{ts}/* -------------{new string('-', comm.Length)} */");
                        sb.AppendLine($"{ts}if (klokperiode(PRM[{_prmpf}{_prmstkp}{iper}], PRM[{_prmpf}{_prmetkp}{iper}]) &&");
                        sb.AppendLine($"{ts}    dagsoort(PRM[{_prmpf}{_prmdckp}{iper}]))");
                        sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mperiod}] = {iper};");
                        sb.AppendLine();
                        ++iper;
                    }
                    var iperrt = 1;
                    var iperrta = 1;
                    var iperrtdim = 1;
                    var iperbel = 1;
                    var iperbeldim = 1;
                    var ipertwl = 1;
                    ipero = 1;
                    foreach (var kpm in c.PeriodenData.Perioden)
                    {
                        if (kpm.Type != PeriodeTypeEnum.Overig) continue;
                        var comm = kpm.Commentaar ?? "";
                        sb.AppendLine($"{ts}/* vrije klokperiode: {comm} */");
                        sb.AppendLine($"{ts}/* -------------------{new string('-', comm.Length)} */");
                        sb.AppendLine($"{ts}if (klokperiode(PRM[{_prmpf}{_prmstkp}{_prmpero}{ipero}], PRM[{_prmpf}{_prmetkp}{_prmpero}{ipero}]) &&");
                        sb.AppendLine($"{ts}    dagsoort(PRM[{_prmpf}{_prmdckp}{_prmpero}{ipero}]));");
                        sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hperiod}{ipero}] = TRUE;");
                        sb.AppendLine();
                        ++ipero;
                    }
                    if (c.PeriodenData.Perioden.Count > 0)
                    {
                        if(c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.RateltikkersAltijd))
                        {
                            sb.AppendLine($"{ts}/* klokperiode rateltikkers altijd */");
                            sb.AppendLine($"{ts}/* ------------------------------- */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperrt}] = ");
                            foreach(var per in c.PeriodenData.Perioden)
                            {
                                if(per.Type == PeriodeTypeEnum.RateltikkersAltijd)
                                {
                                    if(iperrt != 1)
                                    {
                                        sb.AppendLine(" || ");
                                    }
                                    sb.Append($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperrt}{iperrt}], ");
                                    sb.Append($"PRM[{_prmpf}{_prmetkp}{_prmperrt}{iperrt}]) && ");
                                    sb.Append($"dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperrt}{iperrt}]))");
                                    iperrt++;
                                }
                            }
                            sb.AppendLine(";");
                            sb.AppendLine();
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.RateltikkersAanvraag))
                        {
                            sb.AppendLine($"{ts}/* klokperiode rateltikker op aanvraag */");
                            sb.AppendLine($"{ts}/* ----------------------------------- */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperrta}] = ");
                            foreach (var per in c.PeriodenData.Perioden)
                            {
                                if (per.Type == PeriodeTypeEnum.RateltikkersAanvraag)
                                {
                                    if (iperrta != 1)
                                    {
                                        sb.AppendLine(" || ");
                                    }
                                    sb.Append($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperrta}{iperrta}], ");
                                    sb.Append($"PRM[{_prmpf}{_prmetkp}{_prmperrta}{iperrta}]) && ");
                                    sb.Append($"dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperrta}{iperrta}]))");
                                    iperrta++;
                                }
                            }
                            sb.AppendLine(";");
                            sb.AppendLine();
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.RateltikkersDimmen))
                        {
                            sb.AppendLine($"{ts}/* klokperiode rateltikker dimmen */");
                            sb.AppendLine($"{ts}/* ------------------------------ */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperrtdim}] = ");
                            foreach (var per in c.PeriodenData.Perioden)
                            {
                                if (per.Type != PeriodeTypeEnum.RateltikkersDimmen) continue;
                                if (iperrtdim != 1)
                                {
                                    sb.AppendLine(" || ");
                                }
                                sb.Append($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperrtdim}{iperrtdim}], ");
                                sb.Append($"PRM[{_prmpf}{_prmetkp}{_prmperrtdim}{iperrtdim}]) && ");
                                sb.Append($"dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperrtdim}{iperrtdim}]))");
                                iperrtdim++;
                            }
                            sb.AppendLine(";");
                            sb.AppendLine();
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.BellenActief))
                        {
                            sb.AppendLine($"{ts}/* klokperiode bellen actief */");
                            sb.AppendLine($"{ts}/* ------------------------- */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperbel}] = ");
                            foreach (var per in c.PeriodenData.Perioden)
                            {
                                if (per.Type != PeriodeTypeEnum.RateltikkersAanvraag) continue;
                                if (iperbel != 1)
                                {
                                    sb.AppendLine(" || ");
                                }
                                sb.Append($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperbel}{iperbel}], ");
                                sb.Append($"PRM[{_prmpf}{_prmetkp}{_prmperbel}{iperbel}]) && ");
                                sb.Append($"dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperbel}{iperbel}]))");
                                iperbel++;
                            }
                            sb.AppendLine(";");
                            sb.AppendLine();
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.BellenDimmen))
                        {
                            sb.AppendLine($"{ts}/* klokperiode bellen dimmen */");
                            sb.AppendLine($"{ts}/* ------------------------- */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperbeldim}] = ");
                            foreach (var per in c.PeriodenData.Perioden)
                            {
                                if (per.Type != PeriodeTypeEnum.RateltikkersAanvraag) continue;
                                if (iperbeldim != 1)
                                {
                                    sb.AppendLine(" || ");
                                }
                                sb.Append($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperbeldim}{iperbeldim}], ");
                                sb.Append($"PRM[{_prmpf}{_prmetkp}{_prmperbeldim}{iperbeldim}]) && ");
                                sb.Append($"dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperbeldim}{iperbeldim}]))");
                                iperbeldim++;
                            }
                            sb.AppendLine(";");
                            sb.AppendLine();
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.WaarschuwingsLichten))
                        {
                            sb.AppendLine($"{ts}/* klokperiode twl's actief */");
                            sb.AppendLine($"{ts}/* ------------------------ */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmpertwl}] = ");
                            foreach (var per in c.PeriodenData.Perioden)
                            {
                                if (per.Type != PeriodeTypeEnum.RateltikkersAanvraag) continue;
                                if (ipertwl != 1)
                                {
                                    sb.AppendLine(" || ");
                                }
                                sb.Append($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmpertwl}{ipertwl}], ");
                                sb.Append($"PRM[{_prmpf}{_prmetkp}{_prmpertwl}{ipertwl}]) && ");
                                sb.Append($"dagsoort(PRM[{_prmpf}{_prmdckp}{_prmpertwl}{ipertwl}]))");
                                ipertwl++;
                            }
                            sb.AppendLine(";");
                            sb.AppendLine();
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:

                    // ReSharper disable once TooWideLocalVariableScope
                    string grfunc;
                    switch(c.Data.TypeGroentijden)
                    {
                        case GroentijdenTypeEnum.MaxGroentijden: grfunc = "max_star_groentijden_va_arg"; break;
                        case GroentijdenTypeEnum.VerlengGroentijden: grfunc = "verleng_star_groentijden_va_arg"; break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    // Groentijden obv periode
                    foreach (var fcm in c.Fasen)
                    {
                        // Check if the FaseCyclus has any maxgreen times set
                        var hasMg = false;
                        foreach (var mgsm in c.GroentijdenSets)
                        {
                            foreach (var mgm in mgsm.Groentijden)
                            {
                                if (mgm.FaseCyclus == fcm.Naam && mgm.Waarde.HasValue)
                                {
                                    hasMg = true;
                                }
                            }
                        }
                        if (c.PeriodenData.DefaultPeriodeGroentijdenSet == null)
                        {
                            hasMg = false;
                        }

                        if (hasMg)
                        {
                            sb.AppendLine($"{ts}{grfunc}((count) {_fcpf}{fcm.Naam}, (mulv) FALSE, (mulv) FALSE,");
                            var mper = 1;
                            foreach (var per in c.PeriodenData.Perioden)
                            {
                                if (per.Type != PeriodeTypeEnum.Groentijden) continue;
                                foreach (var mgsm in c.GroentijdenSets)
                                {
                                    if (mgsm.Naam != per.GroentijdenSet) continue;
                                    foreach (var mgm in mgsm.Groentijden)
                                    {
                                        if (mgm.FaseCyclus != fcm.Naam || !mgm.Waarde.HasValue) continue;
                                        sb.Append("".PadLeft(($"{ts}{grfunc}(").Length));
                                        sb.AppendLine(
                                            ($"(va_mulv) PRM[{_prmpf}{per.GroentijdenSet.ToLower()}_{fcm.Naam}], (va_mulv) NG, (va_mulv) (MM[mperiod] == {mper}),"));
                                    }
                                }
                                ++mper;
                            }
                            sb.Append("".PadLeft(($"{ts}{grfunc}(").Length));
                            sb.AppendLine($"(va_mulv) PRM[{_prmpf}{c.PeriodenData.DefaultPeriodeGroentijdenSet.ToLower()}_{fcm.Naam}], (va_mulv) NG, (va_count) END);");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}TVG_max[{_fcpf}{fcm.Naam}] = 0;");
                        }

                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSystemApplication:
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
                        if(c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.RateltikkersAltijd))
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{_prmperrt}] = (IH[{_hpf}{_hperiod}{_prmperrt}] == TRUE);");
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.RateltikkersAanvraag))
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{_prmperrta}] = (IH[{_hpf}{_hperiod}{_prmperrta}] == TRUE);");
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.RateltikkersDimmen))
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{_prmperrtdim}] = (IH[{_hpf}{_hperiod}{_prmperrtdim}] == TRUE);");
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.BellenActief))
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{_prmperbel}] = (IH[{_hpf}{_hperiod}{_prmperbel}] == TRUE);");
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.BellenDimmen))
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{_prmperbeldim}] = (IH[{_hpf}{_hperiod}{_prmperbeldim}] == TRUE);");
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.WaarschuwingsLichten))
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
                    sb.AppendLine();
                    return sb.ToString();

                default:
                    return null;
            }
        }
    }
}
