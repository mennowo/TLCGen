using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
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
        private CCOLGeneratorCodeStringSettingModel _prmtypema;
        private CCOLGeneratorCodeStringSettingModel _tuitgestma;
#pragma warning restore 0649
        private string _hmad;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var ma in c.InterSignaalGroep.Meeaanvragen)
            {
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
                    var inst = ma.Type switch
                    {
                        MeeaanvraagTypeEnum.Aanvraag => 1,
                        MeeaanvraagTypeEnum.RoodVoorAanvraag => 2,
                        MeeaanvraagTypeEnum.RoodVoorAanvraagGeenConflicten => 3,
                        MeeaanvraagTypeEnum.Startgroen => 4,
                        _ => throw new ArgumentOutOfRangeException()
                    };
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
        
        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    if (!c.InterSignaalGroep.Meeaanvragen.Any()) return base.GetFunctionLocalVariables(c, type);
                    return new List<CCOLLocalVariable> {new("int", "fc")};
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }
        
        public override int HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCAanvragen => 30,
                _ => 0
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    if (!c.InterSignaalGroep.Meeaanvragen.Any()) return "";
                    
                    sb.AppendLine($"{ts}/* Meeaanvragen */");
                    sb.AppendLine($"{ts}/* ------------ */");

                    sb.AppendLine("");
                    
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
                                    sb.AppendLine($"{tts}{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, ({c.GetBoolV()})(ET[{_tpf}{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}]));");
                                    sb.AppendLine($"{tts}}}");
                                    sb.AppendLine($"{tts}else");
                                    sb.AppendLine($"{tts}{{");
                                    sb.AppendLine($"{tts}{ts}mee_aanvraag_prm({_fcpf}{ma.FaseNaar}, {_fcpf}{ma.FaseVan}, {_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}, ({c.GetBoolV()})(TRUE));");
                                    sb.AppendLine($"{tts}}}");
                                }
                                else
                                {
                                    sb.AppendLine($"{tts}mee_aanvraag_prm({_fcpf}{ma.FaseNaar}, {_fcpf}{ma.FaseVan}, {_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}, ({c.GetBoolV()})(TRUE));");
                                }
                            }
                            else
                            {
                                if (ma.Uitgesteld)
                                {
                                    sb.AppendLine($"{tts}if (PRM[{_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}] == 4)");
                                    sb.AppendLine($"{tts}{{");
                                    sb.AppendLine($"{tts}{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, ({c.GetBoolV()})(ET[{_tpf}{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}]));");
                                    sb.AppendLine($"{tts}}}");
                                    sb.AppendLine($"{tts}else");
                                    sb.AppendLine($"{tts}{{");
                                }

                                var uts = ma.Uitgesteld ? ts + tts : tts;
                                sb.Append($"{uts}mee_aanvraag_prm({_fcpf}{ma.FaseNaar}, {_fcpf}{ma.FaseVan}, {_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}, ({c.GetBoolV()})(");
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
                                    sb.AppendLine($"{tts}mee_aanvraag({_fcpf}{ma.FaseNaar}, ({c.GetBoolV()}) (R[{_fcpf}{ma.FaseVan}] && !TRG[{_fcpf}{ma.FaseVan}] && A[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraag:
                                    sb.AppendLine($"{tts}mee_aanvraag({_fcpf}{ma.FaseNaar}, ({c.GetBoolV()}) (RA[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraagGeenConflicten:
                                    sb.AppendLine($"{tts}mee_aanvraag({_fcpf}{ma.FaseNaar}, ({c.GetBoolV()}) (RA[{_fcpf}{ma.FaseVan}] && !K[{_fcpf}{ma.FaseVan}] || SG[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.Startgroen:
                                    sb.AppendLine(!ma.Uitgesteld
                                        ? $"{tts}mee_aanvraag({_fcpf}{ma.FaseNaar}, ({c.GetBoolV()}) (SG[{_fcpf}{ma.FaseVan}]));"
                                        : $"{tts}mee_aanvraag({_fcpf}{ma.FaseNaar}, ({c.GetBoolV()}) (ET[{_tpf}{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}]));");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else
                        {
                            sb.Append($"{tts}mee_aanvraag({_fcpf}{ma.FaseNaar}, ({c.GetBoolV()}) (");
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
                                    sb.AppendLine($"&& RA[{_fcpf}{ma.FaseVan}] && !K[{_fcpf}{ma.FaseVan}] || SG[{_fcpf}{ma.FaseVan}]));");
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

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _hmad = CCOLGeneratorSettingsProvider.Default.GetElementName("hmad");

            return base.SetSettings(settings);
        }
    }
}
