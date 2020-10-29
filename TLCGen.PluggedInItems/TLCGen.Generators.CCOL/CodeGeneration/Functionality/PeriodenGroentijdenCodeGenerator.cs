using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    [CCOLCodePieceGenerator]
    public class PeriodenGroentijdenCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _usperdef;
        private CCOLGeneratorCodeStringSettingModel _usper;
        private CCOLGeneratorCodeStringSettingModel _prmstkp;
        private CCOLGeneratorCodeStringSettingModel _prmetkp;
        private CCOLGeneratorCodeStringSettingModel _prmdckp;
        private CCOLGeneratorCodeStringSettingModel _mperiod;
        private CCOLGeneratorCodeStringSettingModel _mperiodstar;
        private CCOLGeneratorCodeStringSettingModel _hperiod;
        private CCOLGeneratorCodeStringSettingModel _prmperrt;
        private CCOLGeneratorCodeStringSettingModel _prmperrta;
        private CCOLGeneratorCodeStringSettingModel _prmperrtdim;
        private CCOLGeneratorCodeStringSettingModel _prmperbel;
        private CCOLGeneratorCodeStringSettingModel _prmperbeldim;
        private CCOLGeneratorCodeStringSettingModel _prmpero;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myBitmapOutputs = new List<CCOLIOElement>();

            var iper = 1;
            var iperrt = 1;
            var iperrta = 1;
            var iperrtdim = 1;
            var iperbel = 1;
            var iperbeldim = 1;

            if (c.StarData.ToepassenStar && c.StarData.ProgrammaSturingViaKlok)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mperiodstar}", _mperiodstar));
            }

            // outputs
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_usperdef.Setting, _usperdef));
            _myBitmapOutputs.Add(new CCOLIOElement(c.PeriodenData.DefaultPeriodeBitmapData, $"{_uspf}{_usperdef}"));
            foreach (var per in c.PeriodenData.Perioden)
            {
                switch(per.Type)
                {
                    case PeriodeTypeEnum.Groentijden:
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usper}{(c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iper.ToString())}", _usper, per.Commentaar));
                        _myBitmapOutputs.Add(new CCOLIOElement(per.BitmapData, $"{_uspf}{_usper}{(c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iper.ToString())}"));
                        ++iper;
                        break;
                    case PeriodeTypeEnum.StarRegelen:
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usper}{per.Naam}", _usper, per.Commentaar));
                        _myBitmapOutputs.Add(new CCOLIOElement(per.BitmapData, $"{_uspf}{_usper}{per.Naam}"));
                        break;
                    case PeriodeTypeEnum.Overig:
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usper}{_prmpero}{per.Naam}", _usper, per.Commentaar));
                        _myBitmapOutputs.Add(new CCOLIOElement(per.BitmapData, $"{_uspf}{_usper}{_prmpero}{per.Naam}"));
                        break;
                }
            }

            // parameters
            iper = 1;
            foreach (var per in c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.Groentijden))
            {
                var hours = per.StartTijd.Hours;
                if (per.StartTijd.Days == 1)
                {
                    hours = 24;
                }
                var inst = hours * 100 + per.StartTijd.Minutes;
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstkp}{(c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iper.ToString())}", inst, CCOLElementTimeTypeEnum.TI_type, _prmstkp, per.Naam));
                hours = per.EindTijd.Hours;
                if (per.EindTijd.Days == 1)
                {
                    hours = 24;
                }
                inst = hours * 100 + per.EindTijd.Minutes;
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmetkp}{(c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iper.ToString())}", inst, CCOLElementTimeTypeEnum.TI_type, _prmetkp, per.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmdckp}{(c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iper.ToString())}", (int)per.DagCode, CCOLElementTimeTypeEnum.None, _prmdckp, per.Naam));
                ++iper;
            }

            foreach (var per in c.PeriodenData.Perioden.Where(x => x.Type != PeriodeTypeEnum.Groentijden))
            {
                // get period params names
                var pertypeandnum = "";
                switch (per.Type)
                {
                    case PeriodeTypeEnum.RateltikkersAltijd: pertypeandnum = _prmperrt + (c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iperrt.ToString()); iperrt++; break;
                    case PeriodeTypeEnum.RateltikkersAanvraag: pertypeandnum = _prmperrta + (c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iperrta.ToString()); iperrta++; break;
                    case PeriodeTypeEnum.RateltikkersDimmen: pertypeandnum = _prmperrtdim + (c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iperrtdim.ToString()); iperrtdim++; break;
                    case PeriodeTypeEnum.BellenActief: pertypeandnum = _prmperbel + (c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iperbel.ToString()); iperbel++; break;
                    case PeriodeTypeEnum.BellenDimmen: pertypeandnum = _prmperbeldim + (c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iperbeldim.ToString()); iperbeldim++; break;
                    case PeriodeTypeEnum.StarRegelen: pertypeandnum = per.Naam; break;
                    case PeriodeTypeEnum.Overig: pertypeandnum = _prmpero + per.Naam; break;
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
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(stkp, inst, CCOLElementTimeTypeEnum.TI_type, _prmstkp, per.Naam));
                hours = per.EindTijd.Hours;
                if (per.EindTijd.Days == 1)
                {
                    hours = 24;
                }
                inst = hours * 100 + per.EindTijd.Minutes;
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(etkp, inst, CCOLElementTimeTypeEnum.TI_type, _prmetkp, per.Naam));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(dckp, (int)per.DagCode, CCOLElementTimeTypeEnum.None, _prmdckp, per.Naam));

                // free period helpelem
                if(per.Type == PeriodeTypeEnum.Overig)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hperiod}{per.Naam}", _hperiod, per.Naam));
                }
            }
            if (iperrt > 1)     _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hperiod}{_prmperrt}", CCOLElementTypeEnum.HulpElement, _prmperrt.Description));
            if (iperrta > 1)    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hperiod}{_prmperrta}", CCOLElementTypeEnum.HulpElement, _prmperrta.Description));
            if (iperrtdim > 1)  _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hperiod}{_prmperrtdim}", CCOLElementTypeEnum.HulpElement, _prmperrtdim.Description));
            if (iperbel > 1)    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hperiod}{_prmperbel}", CCOLElementTypeEnum.HulpElement, _prmperbel.Description));
            if (iperbeldim > 1) _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hperiod}{_prmperbeldim}", CCOLElementTypeEnum.HulpElement, _prmperbeldim.Description));
            if (iperrt > 1)     _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usper}{_prmperrt}", CCOLElementTypeEnum.Uitgang, _prmperrt.Description));
            if (iperrta > 1)    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usper}{_prmperrta}", CCOLElementTypeEnum.Uitgang, _prmperrta.Description));
            if (iperrtdim > 1)  _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usper}{_prmperrtdim}", CCOLElementTypeEnum.Uitgang, _prmperrtdim.Description));
            if (iperbel > 1)    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usper}{_prmperbel}", CCOLElementTypeEnum.Uitgang, _prmperbel.Description));
            if (iperbeldim > 1) _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usper}{_prmperbeldim}", CCOLElementTypeEnum.Uitgang, _prmperbeldim.Description));
            if (iperrt > 1)     _myBitmapOutputs.Add(new CCOLIOElement(c.Signalen.RatelTikkerAltijdBitmapData, $"{_uspf}{_usper}{_prmperrt}"));
            if (iperrta > 1)    _myBitmapOutputs.Add(new CCOLIOElement(c.Signalen.RatelTikkerActiefBitmapData, $"{_uspf}{_usper}{_prmperrta}"));
            if (iperrtdim > 1)  _myBitmapOutputs.Add(new CCOLIOElement(c.Signalen.RatelTikkerDimmenBitmapData, $"{_uspf}{_usper}{_prmperrtdim}"));
            if (iperbel > 1)    _myBitmapOutputs.Add(new CCOLIOElement(c.Signalen.BellenActiefBitmapData, $"{_uspf}{_usper}{_prmperbel}"));
            if (iperbeldim > 1) _myBitmapOutputs.Add(new CCOLIOElement(c.Signalen.BellenDimmenBitmapData, $"{_uspf}{_usper}{_prmperbeldim}"));

            // groentijden
            var mg = c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? "Maximale groentijd" : "Verlenggroentijd";
            foreach (var mgset in c.GroentijdenSets)
            {
                foreach (var mgm in mgset.Groentijden)
                {
                    if (!mgm.Waarde.HasValue)
                        continue;

                    var thisfcm = c.Fasen.FirstOrDefault(fcm => fcm.Naam == mgm.FaseCyclus);

                    if (thisfcm == null)
                        throw new NullReferenceException($"Maxgroentijd voor niet bestaande fase {mgm.FaseCyclus} opgegeven.");

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{mgset.Naam.ToLower()}_{thisfcm.Naam}",
                        mgm.Waarde.Value,
                        CCOLElementTimeTypeEnum.TE_type,
                        CCOLElementTypeEnum.Parameter,
                        $"{mg} {mgset.Naam} {thisfcm.Naam}"));
                }
            }
        }

        public override bool HasCCOLElements() => true;
    
        public override bool HasCCOLBitmapOutputs() => true;
    
        public override int HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCKlokPerioden => 10,
                CCOLCodeTypeEnum.RegCMaxgroen => 10,
                CCOLCodeTypeEnum.RegCVerlenggroen => 20,
                CCOLCodeTypeEnum.RegCSystemApplication => 50,
                _ => 0
            };
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
                    iper = 1;
                    foreach (var kpm in c.PeriodenData.Perioden)
                    {
                        if (kpm.Type != PeriodeTypeEnum.Groentijden) continue;
                        var comm = kpm.Commentaar ?? "";
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* klokperiode: {comm} */");
                        sb.AppendLine($"{ts}/* -------------{new string('-', comm.Length)} */");
                        var perN = c.PeriodenData.GebruikPeriodenNamen ? kpm.Naam : iper.ToString();
                        sb.AppendLine($"{ts}if (klokperiode(PRM[{_prmpf}{_prmstkp}{perN}], PRM[{_prmpf}{_prmetkp}{perN}]) &&");
                        sb.AppendLine($"{ts}    dagsoort(PRM[{_prmpf}{_prmdckp}{perN}]))");
                        sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mperiod}] = {iper};");
                        ++iper;
                    }

                    iper = 1; // start at 1 for star, so 0 can be 'not star'
                    foreach (var kpm in c.PeriodenData.Perioden)
                    {
                        if (kpm.Type != PeriodeTypeEnum.StarRegelen) continue;
                        var comm = kpm.Commentaar ?? "";
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* klokperiode star regelen: {comm} */");
                        sb.AppendLine($"{ts}/* --------------------------{new string('-', comm.Length)} */");
                        sb.AppendLine($"{ts}if (klokperiode(PRM[{_prmpf}{_prmstkp}{kpm.Naam}], PRM[{_prmpf}{_prmetkp}{kpm.Naam}]) &&");
                        sb.AppendLine($"{ts}    dagsoort(PRM[{_prmpf}{_prmdckp}{kpm.Naam}]))");
                        sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mperiodstar}] = {iper};");
                        ++iper;
                    }

                    var iperrt = 1;
                    var iperrta = 1;
                    var iperrtdim = 1;
                    var iperbel = 1;
                    var iperbeldim = 1;
                    foreach (var kpm in c.PeriodenData.Perioden)
                    {
                        if (kpm.Type != PeriodeTypeEnum.Overig) continue;
                        var comm = kpm.Commentaar ?? "";
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* vrije klokperiode: {comm} */");
                        sb.AppendLine($"{ts}/* -------------------{new string('-', comm.Length)} */");
                        sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{kpm.Naam}] = (klokperiode(PRM[{_prmpf}{_prmstkp}{_prmpero}{kpm.Naam}], PRM[{_prmpf}{_prmetkp}{_prmpero}{kpm.Naam}]) && dagsoort(PRM[{_prmpf}{_prmdckp}{_prmpero}{kpm.Naam}]));");
                    }
                    if (c.PeriodenData.Perioden.Count > 0)
                    {
                        if(c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.RateltikkersAltijd))
                        {
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* klokperiode rateltikkers altijd */");
                            sb.AppendLine($"{ts}/* ------------------------------- */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperrt}] = ");
                            foreach (var per in c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersAltijd))
                            {
                                if (iperrt != 1)
                                {
                                    sb.AppendLine(" || ");
                                }

                                var perN = c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iperrt.ToString();
                                sb.Append($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperrt}{perN}], ");
                                sb.Append($"PRM[{_prmpf}{_prmetkp}{_prmperrt}{perN}]) && ");
                                sb.Append($"dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperrt}{perN}]))");
                                iperrt++;
                            }
                            sb.AppendLine(";");
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.RateltikkersAanvraag))
                        {
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* klokperiode rateltikker op aanvraag */");
                            sb.AppendLine($"{ts}/* ----------------------------------- */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperrta}] = ");
                            foreach (var per in c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersAanvraag))
                            {
                                if (iperrta != 1)
                                {
                                    sb.AppendLine(" || ");
                                }
                                var perN = c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iperrta.ToString();
                                sb.Append($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperrta}{perN}], ");
                                sb.Append($"PRM[{_prmpf}{_prmetkp}{_prmperrta}{perN}]) && ");
                                sb.Append($"dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperrta}{perN}]))");
                                iperrta++;
                            }
                            sb.AppendLine(";");
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.RateltikkersDimmen))
                        {
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* klokperiode rateltikker dimmen */");
                            sb.AppendLine($"{ts}/* ------------------------------ */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperrtdim}] = ");
                            foreach (var per in c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersDimmen))
                            {
                                if (iperrtdim != 1)
                                {
                                    sb.AppendLine(" || ");
                                }
                                var perN = c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iperrtdim.ToString();
                                sb.Append($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperrtdim}{perN}], ");
                                sb.Append($"PRM[{_prmpf}{_prmetkp}{_prmperrtdim}{perN}]) && ");
                                sb.Append($"dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperrtdim}{perN}]))");
                                iperrtdim++;
                            }
                            sb.AppendLine(";");
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.BellenActief))
                        {
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* klokperiode bellen actief */");
                            sb.AppendLine($"{ts}/* ------------------------- */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperbel}] = ");
                            foreach (var per in c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.BellenActief))
                            {
                                if (iperbel != 1)
                                {
                                    sb.AppendLine(" || ");
                                }
                                var perN = c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iperbel.ToString();
                                sb.Append($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperbel}{perN}], ");
                                sb.Append($"PRM[{_prmpf}{_prmetkp}{_prmperbel}{perN}]) && ");
                                sb.Append($"dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperbel}{perN}]))");
                                iperbel++;
                            }
                            sb.AppendLine(";");
                        }
                        if (c.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.BellenDimmen))
                        {
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* klokperiode bellen dimmen */");
                            sb.AppendLine($"{ts}/* ------------------------- */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hperiod}{_prmperbeldim}] = ");
                            foreach (var per in c.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.BellenDimmen))
                            {
                                if (iperbeldim != 1)
                                {
                                    sb.AppendLine(" || ");
                                }
                                var perN = c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iperbeldim.ToString();
                                sb.Append($"{ts}{ts}(klokperiode(PRM[{_prmpf}{_prmstkp}{_prmperbeldim}{perN}], ");
                                sb.Append($"PRM[{_prmpf}{_prmetkp}{_prmperbeldim}{perN}]) && ");
                                sb.Append($"dagsoort(PRM[{_prmpf}{_prmdckp}{_prmperbeldim}{perN}]))");
                                iperbeldim++;
                            }
                            sb.AppendLine(";");
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    var grfunc = c.Data.TypeGroentijden switch
                    {
                        GroentijdenTypeEnum.MaxGroentijden => "maximumgroentijden_va_arg",
                        GroentijdenTypeEnum.VerlengGroentijden => "verlenggroentijden_va_arg",
                        _ => throw new ArgumentOutOfRangeException()
                    };

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
                            sb.AppendLine($"{ts}{grfunc}((count) {_fcpf}{fcm.Naam},");
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
                                            ($"(va_mulv) PRM[{_prmpf}{per.GroentijdenSet.ToLower()}_{fcm.Naam}], (va_mulv) (MM[{_mpf}{_mperiod}] == {mper}),"));
                                    }
                                }
                                ++mper;
                            }
                            sb.Append("".PadLeft($"{ts}{grfunc}(".Length));
                            if (c.Data.TVGAMaxAlsDefaultGroentijdSet)
                            {
                                sb.AppendLine($"(va_mulv) TVGA_max[{_fcpf}{fcm.Naam}], (va_count) END);");
                            }
                            else
                            {
                                sb.AppendLine($"(va_mulv) PRM[{_prmpf}{c.PeriodenData.DefaultPeriodeGroentijdenSet.ToLower()}_{fcm.Naam}], (va_count) END);");
                            }
                        }
                        else
                        {
                            sb.AppendLine($"{ts}TVG_max[{_fcpf}{fcm.Naam}] = 0;");
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSystemApplication:
                    sb.AppendLine($"{ts}/* periode verklikking */");
                    sb.AppendLine($"{ts}/* ------------------- */");
                    iper = 0;
                    sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usperdef}] = (MM[{_mpf}{_mperiod}] == {iper++});");
                    foreach (var per in c.PeriodenData.Perioden.Where(per => per.Type == PeriodeTypeEnum.Groentijden))
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{(c.PeriodenData.GebruikPeriodenNamen ? per.Naam : iper.ToString())}] = (MM[{_mpf}{_mperiod}] == {iper++});");
                    }
                    iper = 1;
                    foreach (var per in c.PeriodenData.Perioden.Where(per => per.Type == PeriodeTypeEnum.StarRegelen))
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{per.Naam}] = (MM[{_mpf}{_mperiodstar}] == {iper++});");
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
                    }
                    foreach (var per in c.PeriodenData.Perioden.Where(per => per.Type == PeriodeTypeEnum.Overig))
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usper}{_prmpero}{per.Naam}] = (IH[{_hpf}{_hperiod}{per.Naam}] == TRUE);");
                    }
                    return sb.ToString();

                default:
                    return null;
            }
        }
    }
}
