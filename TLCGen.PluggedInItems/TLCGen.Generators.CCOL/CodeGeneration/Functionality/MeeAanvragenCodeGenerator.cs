﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class MeeAanvragenCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schma;
        private CCOLGeneratorCodeStringSettingModel _hmad;
        private CCOLGeneratorCodeStringSettingModel _prmtypema;
        private CCOLGeneratorCodeStringSettingModel _tuitgestma;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var ma in c.InterSignaalGroep.Meeaanvragen)
            {
                if (ma.DetectieAfhankelijk)
                {
                    foreach(var dm in ma.Detectoren)
                    {
                        var elem =
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_hmad}{dm.MeeaanvraagDetector}",
                                _hmad, dm.MeeaanvraagDetector);
                        if (_myElements.Count == 0 || _myElements.All(x => x.Naam != elem.Naam))
                        {
                            _myElements.Add(elem);
                        }
                    }
                }

                if (ma.AanUit != AltijdAanUitEnum.Altijd)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_schma}{ma.FaseVan}{ma.FaseNaar}",
                            ma.AanUit == AltijdAanUitEnum.SchAan ? 1 : 0,
                            CCOLElementTimeTypeEnum.SCH_type,
                            _schma, ma.FaseVan, ma.FaseNaar));
                }

                if ((ma.Type == MeeaanvraagTypeEnum.Startgroen || ma.TypeInstelbaarOpStraat) &&
                    ma.Uitgesteld)
                {

                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}",
                            ma.UitgesteldTijdsduur,
                            CCOLElementTimeTypeEnum.TE_type,
                            _tuitgestma, ma.FaseVan, ma.FaseNaar));
                }
                
                if (ma.TypeInstelbaarOpStraat)
                {
                    var inst = 0;
                    switch (ma.Type)
                    {
                        case MeeaanvraagTypeEnum.Aanvraag:
                            inst = 1;
                            break;
                        case MeeaanvraagTypeEnum.RoodVoorAanvraag:
                            inst = 2;
                            break;
                        case MeeaanvraagTypeEnum.RoodVoorAanvraagGeenConflicten:
                            inst = 3;
                            break;
                        case MeeaanvraagTypeEnum.Startgroen:
                            inst = 4;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmtypema}{ma.FaseVan}{ma.FaseNaar}",
                            inst,
                            CCOLElementTimeTypeEnum.None,
                            _prmtypema, ma.FaseVan, ma.FaseNaar));
                }
            }
        }

        public override bool HasCCOLElements() => true;
        
        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    return 30;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    sb.AppendLine($"{ts}/* Meeaanvragen */");
                    sb.AppendLine($"{ts}/* ------------ */");
                    bool hasdetafh = false;
                    if(c.InterSignaalGroep.Meeaanvragen.Count > 0)
                    {
                        hasdetafh = c.InterSignaalGroep.Meeaanvragen.Any(x => x.DetectieAfhankelijk);
                    }
                    if(hasdetafh)
                    {
                        sb.AppendLine($"{ts}/* Bewaar meldingen van detectie voor het zetten van een meeaanvraag */");
                        var mads = new List<Tuple<string, string>>();
                        foreach (var ma in c.InterSignaalGroep.Meeaanvragen)
                        {
                            if (!ma.DetectieAfhankelijk) continue;
                            foreach(var d in ma.Detectoren)
                            {
                                if(!mads.Any(x => x.Item1 == ma.FaseVan && x.Item2 == d.MeeaanvraagDetector))
                                {
                                    mads.Add(new Tuple<string, string>(ma.FaseVan, d.MeeaanvraagDetector));
                                }
                            }
                        }
                        foreach (var mad in mads)
                        {
                            sb.AppendLine($"{ts}IH[{_hpf}{_hmad}{mad.Item2}] = SG[{_fcpf}{mad.Item1}] ? FALSE : IH[{_hpf}{_hmad}{mad.Item2}] || D[{_dpf}{mad.Item2}] && !G[{_fcpf}{mad.Item1}] && A[{_fcpf}{mad.Item1}];");
                        }
                    }
                    foreach (var ma in c.InterSignaalGroep.Meeaanvragen)
                    {
                        var tts = ts;
                        if (ma.AanUit != AltijdAanUitEnum.Altijd)
                        {
                            sb.AppendLine($"{ts}if (SCH[{_schpf}{_schma}{ma.FaseVan}{ma.FaseNaar}])");
                            sb.AppendLine($"{ts}{{");
                            tts = ts + ts;
                        }
                        if (ma.Uitgesteld)
                        {
                            sb.AppendLine($"{tts}/* Uitgestelde meeaanvraag op startgroen */");
                            
                            sb.Append($"{tts}RT[{_tpf}{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}] = SG[{_fcpf}{ma.FaseVan}]");
                            if (ma.DetectieAfhankelijk && ma.Detectoren.Any())
                            {
                                sb.Append(" && (");
                                var i = 0;
                                foreach (var dm in ma.Detectoren)
                                {
                                    if (i == 1)
                                    {
                                        sb.Append(" || ");
                                    }
                                    ++i;
                                    sb.Append($"H[{_hpf}{_hmad}{dm.MeeaanvraagDetector}]");
                                }
                                sb.Append(")");
                            }
                            sb.AppendLine(";");
                        }
                        if (ma.TypeInstelbaarOpStraat)
                        {
                            if (!ma.DetectieAfhankelijk || !ma.Detectoren.Any())
                            {
                                if (ma.Uitgesteld)
                                {
                                    sb.AppendLine($"{tts}if (PRM[{_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}] == 4)");
                                    sb.AppendLine($"{tts}{{");
                                    sb.AppendLine($"{tts}{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (boolv)(ET[{_tpf}{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}]));");
                                    sb.AppendLine($"{tts}}}");
                                    sb.AppendLine($"{tts}else");
                                    sb.AppendLine($"{tts}{{");
                                    sb.AppendLine($"{tts}{ts}mee_aanvraag_prm({_fcpf}{ma.FaseNaar}, {_fcpf}{ma.FaseVan}, {_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}, (boolv)(TRUE));");
                                    sb.AppendLine($"{tts}}}");
                                }
                                else
                                {
                                    sb.AppendLine($"{tts}mee_aanvraag_prm({_fcpf}{ma.FaseNaar}, {_fcpf}{ma.FaseVan}, {_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}, (boolv)(TRUE));");
                                }
                            }
                            else
                            {
                                if (ma.Uitgesteld)
                                {
                                    sb.AppendLine($"{tts}if (PRM[{_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}] == 4)");
                                    sb.AppendLine($"{tts}{{");
                                    sb.AppendLine($"{tts}{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (boolv)(ET[{_tpf}{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}]));");
                                    sb.AppendLine($"{tts}}}");
                                    sb.AppendLine($"{tts}else");
                                    sb.AppendLine($"{tts}{{");
                                }

                                var uts = ma.Uitgesteld ? ts + tts : tts;
                                sb.Append($"{uts}mee_aanvraag_prm({_fcpf}{ma.FaseNaar}, {_fcpf}{ma.FaseVan}, {_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}, (boolv)(");
                                var i = 0;
                                foreach (var dm in ma.Detectoren)
                                {
                                    if (i > 0)
                                    {
                                        sb.Append(" || ");
                                    }
                                    ++i;
                                    sb.Append($"H[{_hpf}{_hmad}{dm.MeeaanvraagDetector}]");
                                }
                                sb.AppendLine("));");
                                
                                if (ma.Uitgesteld)
                                {
                                    sb.AppendLine($"{tts}}}");
                                }
                            }
                        }
                        else if(!ma.DetectieAfhankelijk || !ma.Detectoren.Any())
                        {
                            switch(ma.Type)
                            {
                                case MeeaanvraagTypeEnum.Aanvraag:
                                    sb.AppendLine($"{tts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (boolv) (!G[{_fcpf}{ma.FaseVan}] && A[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraag:
                                    sb.AppendLine($"{tts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (boolv) (RA[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraagGeenConflicten:
                                    sb.AppendLine($"{tts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (boolv) (RA[{_fcpf}{ma.FaseVan}] && !K[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.Startgroen:
                                    sb.AppendLine(!ma.Uitgesteld
                                        ? $"{tts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (boolv) (SG[{_fcpf}{ma.FaseVan}]));"
                                        : $"{tts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (boolv) (ET[{_tpf}{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}]));");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else
                        {
                            sb.Append($"{tts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (boolv) (");
                            var i = 0;
                            if (!ma.Uitgesteld)
                            {
                                sb.Append("(");
                                foreach (var dm in ma.Detectoren)
                                {
                                    if (i >= 1)
                                    {
                                        sb.Append(" || ");
                                    }

                                    ++i;
                                    sb.Append($"H[{_hpf}{_hmad}{dm.MeeaanvraagDetector}]");
                                }
                                sb.Append(") ");
                            }

                            switch (ma.Type)
                            {
                                case MeeaanvraagTypeEnum.Aanvraag:
                                    sb.AppendLine($"&& !G[{_fcpf}{ma.FaseVan}] && A[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraag:
                                    sb.AppendLine($"&& RA[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraagGeenConflicten:
                                    sb.AppendLine($"&& RA[{_fcpf}{ma.FaseVan}] && !K[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.Startgroen:
                                    sb.AppendLine(!ma.Uitgesteld
                                        ? $"&& SG[{_fcpf}{ma.FaseVan}]));"
                                        : $"ET[{_tpf}{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}]));");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        if (ma.AanUit != AltijdAanUitEnum.Altijd)
                        {
                            sb.AppendLine($"{ts}}}");
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
    }
}
