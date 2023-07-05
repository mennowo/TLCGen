using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class MeeverlengenCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schmv;
        private CCOLGeneratorCodeStringSettingModel _schhardmv;
        private CCOLGeneratorCodeStringSettingModel _prmmv;
#pragma warning restore 0649
        private string _hfile;
        private string _hplact;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var fcm in c.Fasen)
            {
                // meeverlengen niet hard uit
                if (fcm.Meeverlengen != NooitAltijdAanUitEnum.Nooit)
                {
                    // type instelbaar op straat
                    if (fcm.MeeverlengenTypeInstelbaarOpStraat)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_prmmv}{fcm.Naam}",
                                ((int)fcm.MeeverlengenType) + 1,
                                CCOLElementTimeTypeEnum.None,
                                _prmmv, fcm.Naam));
                    }
                    // indien meeverlengen niet hard aan
                    if(fcm.Meeverlengen != NooitAltijdAanUitEnum.Altijd)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_schmv}{fcm.Naam}", 
                                fcm.Meeverlengen == NooitAltijdAanUitEnum.SchAan ? 1 : 0, 
                                CCOLElementTimeTypeEnum.SCH_type, 
                                _schmv, fcm.Naam));
                    }

                    if (fcm.HardMeeverlengenFaseCycli.Any())
                    {
                        foreach (var mvfc in fcm.HardMeeverlengenFaseCycli)
                        {
                            _myElements.Add(
                                CCOLGeneratorSettingsProvider.Default.CreateElement(
                                    $"{_schhardmv}{fcm.Naam}{mvfc.FaseCyclus}", 
                                    mvfc.AanUit ? 1 : 0, 
                                    CCOLElementTimeTypeEnum.SCH_type, 
                                    _schhardmv, fcm.Naam, mvfc.FaseCyclus));    
                        }
                    }
                }
            }
        }

        public override bool HasCCOLElements() => true;
        
        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCMeeverlengen => new List<CCOLLocalVariable>{new CCOLLocalVariable("int", "fc")},
                CCOLCodeTypeEnum.PrioCAfkappen => c.Fasen.Any(x => x.HardMeeverlengenFaseCycli.Any()) 
                    ? new List<CCOLLocalVariable>{new CCOLLocalVariable("int", "fc")}
                    : base.GetFunctionLocalVariables(c, type),
                _ => base.GetFunctionLocalVariables(c, type)
            };
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCMeeverlengen => new []{10},
                CCOLCodeTypeEnum.PrioCAfkappen => new []{60},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCMeeverlengen:
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    if (c.Fasen.Any(x => x.HardMeeverlengenFaseCycli.Any()))
                        sb.AppendLine($"{ts}{ts}YM[fc] &= ~BIT1;  /* reset BIT-sturing */");
                    sb.AppendLine($"{ts}{ts}YM[fc] &= ~BIT4;  /* reset BIT-sturing */");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();

                    // let op: ym_max_toV1 werkt ook voor intergroen!
                    var totigfunc = c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && c.Data.Intergroen ? "ym_max_toV1" : "ym_max_toV1";
                    var totigfunc2 = c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && c.Data.Intergroen ? "ym_max_toV2" : "ym_max_toV2";
                    var totigfuncCCOL = c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && c.Data.Intergroen ? "ym_max_tig" : "ym_max_to";

                    foreach (var fcm in c.Fasen)
                    {
                        if (fcm.Meeverlengen != NooitAltijdAanUitEnum.Nooit)
                        {
                            sb.Append($"{ts}YM[{fcm.GetDefine()}] |= ");
                            if (fcm.Meeverlengen != NooitAltijdAanUitEnum.Altijd)
                            {
                                sb.Append($"SCH[{_schpf}{_schmv}{fcm.Naam}] && ");
                            }
                            var verschil = fcm.MeeverlengenVerschil?.ToString() ?? "NG";
                            var hfWsgArgs = "";
                            var extraConditions = "hf_wsg()";
                            if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.InterFunc)
                            {
                                extraConditions = "hf_wsg_nlISG()";
                            }
                            if (c.Data.MultiModuleReeksen)
                            {
                                var hfWsg = "hf_wsg_fcfc";
                                var reeks = c.MultiModuleMolens.FirstOrDefault(x => x.Modules.Any(x2 => x2.Fasen.Any(x3 => x3.FaseCyclus == fcm.Naam)));
                                if (reeks != null)
                                {
                                    var rfc1 = c.Fasen.FirstOrDefault(x => reeks.Modules.SelectMany(x2 => x2.Fasen).Any(x3 => x3.FaseCyclus == x.Naam));
                                    var rfc2 = c.Fasen.LastOrDefault(x => reeks.Modules.SelectMany(x2 => x2.Fasen).Any(x3 => x3.FaseCyclus == x.Naam));
                                    if (rfc1 == null || rfc2 == null)
                                    {
                                        hfWsgArgs = "0, FCMAX";
                                    }
                                    else
                                    {
                                        var id2 = c.Fasen.IndexOf(rfc2);
                                        ++id2;
                                        hfWsgArgs = $"{_fcpf}{rfc1.Naam}, {(id2 == c.Fasen.Count ? "FCMAX" : $"{_fcpf}{c.Fasen[id2].Naam}")}";
                                    }
                                }
                                else
                                {
                                    hfWsgArgs = "0, FCMAX";
                                }

                                extraConditions = $"{hfWsg}({hfWsgArgs})";
                            }
                            if(c.InterSignaalGroep.Nalopen.Any())
                            {
                                var nl = c.InterSignaalGroep.Nalopen.FirstOrDefault(x => x.FaseVan == fcm.Naam);
                                if (nl is {Type: NaloopTypeEnum.EindeGroen})
                                {
                                    var hfWsg = "hf_wsg_nl";
                                    if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.InterFunc)
                                    {
                                        hfWsg = "hf_wsg_nlISG";
                                    }
                                    else if (c.Data.MultiModuleReeksen)
                                    {
                                        hfWsg = "hf_wsg_nl_fcfc";
                                    }
                                    
                                    extraConditions = $"{hfWsg}({hfWsgArgs})";
                                }
                                if (nl is { Type: NaloopTypeEnum.StartGroen })
                                {
                                    var sg1 = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseVan);
                                    var sg2 = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);
                                    if (sg1?.Type == FaseTypeEnum.Voetganger &&
                                        sg2?.Type == FaseTypeEnum.Voetganger)
                                    {
                                        extraConditions += $" && !kcv({_fcpf}{nl.FaseNaar})";
                                    }
                                }
                            }
                            if (!fcm.MeeverlengenTypeInstelbaarOpStraat)
                            {
                                switch (fcm.MeeverlengenType)
                                {
                                    case MeeVerlengenTypeEnum.Default:
                                        sb.AppendLine($"ym_maxV1({fcm.GetDefine()}, {verschil}) && {extraConditions} ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.To:
                                        sb.AppendLine($"{totigfunc}({fcm.GetDefine()}, {verschil}) && {extraConditions} ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.MKTo:
                                        sb.AppendLine($"(ym_maxV1({fcm.GetDefine()}, {verschil}) || {totigfunc}({fcm.GetDefine()}, {verschil}) && MK[{fcm.GetDefine()}]) && {extraConditions} ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.Voetganger:
                                        sb.AppendLine($"ym_max_vtgV1({fcm.GetDefine()}) && {extraConditions} ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.DefaultCCOL:
                                        sb.AppendLine($"ym_maxV1({fcm.GetDefine()}, {verschil}) && {extraConditions} ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.ToCCOL:
                                        sb.AppendLine($"{totigfuncCCOL}({fcm.GetDefine()}, {verschil}) && {extraConditions} ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.MKToCCOL:
                                        sb.AppendLine($"(ym_max({fcm.GetDefine()}, {verschil}) || {totigfuncCCOL}({fcm.GetDefine()}, {verschil}) && MK[{fcm.GetDefine()}]) && {extraConditions} ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.MaatgevendGroen:
                                        sb.AppendLine($"!Maatgevend_Groen({fcm.GetDefine()}) && {extraConditions} ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.Default2:
                                        sb.AppendLine($"ym_maxV2({fcm.GetDefine()}, {verschil}) && {extraConditions} ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.To2:
                                        sb.AppendLine($"{totigfunc2}({fcm.GetDefine()}, {verschil}) && {extraConditions} ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.MKTo2:
                                        sb.AppendLine($"(ym_maxV2({fcm.GetDefine()}, {verschil}) || {totigfunc}({fcm.GetDefine()}, {verschil}) && MK[{fcm.GetDefine()}]) && {extraConditions} ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.Voetganger2:
                                        sb.AppendLine($"ym_max_vtgV2({fcm.GetDefine()}) && {extraConditions} ? BIT4 : 0;");
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                            else
                            {
                                if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.InterFunc)
                                {
                                    sb.AppendLine($"ym_max_tig_REALISATIETIJD({fcm.GetDefine()}, {_prmpf}{_prmmv}{fcm.Naam}) && {extraConditions} ? BIT4 : 0;");
                                }
                                else
                                {
                                    sb.AppendLine($"ym_max_prmV1({fcm.GetDefine()}, {_prmpf}{_prmmv}{fcm.Naam}, {verschil}) && {extraConditions} ? BIT4 : 0;");
                                }
                            }
                        }
                    }
                    var file = false;
                    var tts = ts;
                    foreach (var fcm in c.Fasen)
                    {
                        if (fcm.Meeverlengen != NooitAltijdAanUitEnum.Nooit)
                        {
                            foreach (var fm in c.FileIngrepen.Where(x =>
                                x.FileMetingLocatie == FileMetingLocatieEnum.NaStopstreep &&
                                x.TeDoserenSignaalGroepen.Any(x2 => x2.FaseCyclus == fcm.Naam)))
                            {
                                if (!file)
                                {
                                    file = true;
                                    sb.AppendLine();
                                    sb.AppendLine($"{ts}/* Niet meeverlengen tijdens file (meting na ss) */");
                                    if (c.HalfstarData.IsHalfstar)
                                    {
                                        tts = ts;
                                        if (c.HalfstarData.IsHalfstar)
                                        {
                                            tts += ts;
                                            sb.AppendLine($"{ts}if (!IH[{_hpf}{_hplact}])");
                                            sb.AppendLine($"{ts}{{");
                                        }
                                    }
                                }

                                sb.AppendLine($"{tts}if (IH[{_hpf}{_hfile}{fm.Naam}]) YM[{_fcpf}{fcm.Naam}] &= ~BIT4;");
                            }
                        }
                    }
                    if (file && c.HalfstarData.IsHalfstar)
                    {
                        sb.AppendLine($"{ts}}}");
                    }

                    var hard = false;
                    foreach (var fcm in c.Fasen)
                    {
                        if (fcm.HardMeeverlengenFaseCycli.Any())
                        {
                            if (!hard)
                            {
                                hard = true;
                                sb.AppendLine();
                                sb.AppendLine($"{ts}/* Hard meeverlengen */");
                            }
                            foreach(var mvfc in fcm.HardMeeverlengenFaseCycli)
                            {
                                sb.Append($"{ts}if (SCH[{_schpf}{_schhardmv}{fcm.Naam}{mvfc.FaseCyclus}] && ");
                                switch (mvfc.Type)
                                {
                                    case HardMeevelengenTypeEnum.Groen:
                                        sb.Append($"G[{_fcpf}{mvfc.FaseCyclus}]");
                                        break;
                                    case HardMeevelengenTypeEnum.CyclischVerlengGroen:
                                        sb.Append($"CV[{_fcpf}{mvfc.FaseCyclus}]");
                                        break;
                                    case HardMeevelengenTypeEnum.CyclischVerlengGroenEnGroen:
                                        sb.Append($"(CV[{_fcpf}{mvfc.FaseCyclus}] || G[{_fcpf}{mvfc.FaseCyclus}])");
                                        break;
                                }
                                sb.AppendLine($" && !kcv({_fcpf}{fcm.Naam})) YM[{_fcpf}{fcm.Naam}] |= BIT1;");
                            }
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCAfkappen:
                    if (!c.Fasen.Any(x => x.HardMeeverlengenFaseCycli.Any())) return null;

                    sb.AppendLine($"{ts}/* Niet afkappen hard meeverlengen */");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}if (YM[fc] & BIT1)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}Z[fc] &= ~PRIO_Z_BIT;");
                    sb.AppendLine($"{ts}{ts}{ts}FM[fc] &= ~PRIO_FM_BIT;");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _hfile = CCOLGeneratorSettingsProvider.Default.GetElementName("hfile");
            _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");

            return base.SetSettings(settings);
        }
    }
}
