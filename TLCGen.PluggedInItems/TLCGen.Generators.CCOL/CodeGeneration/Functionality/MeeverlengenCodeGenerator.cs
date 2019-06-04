using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private CCOLGeneratorCodeStringSettingModel _prmmv;
#pragma warning restore 0649
        private string _hfile;
        private string _hplact;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (FaseCyclusModel fcm in c.Fasen)
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
                    // anders indien meeverlengen niet hard aan
                    else if(fcm.Meeverlengen != NooitAltijdAanUitEnum.Altijd)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_schmv}{fcm.Naam}", 
                                (fcm.Meeverlengen == NooitAltijdAanUitEnum.SchAan ? 1 : 0), 
                                CCOLElementTimeTypeEnum.SCH_type, 
                                _schmv, fcm.Naam));
                    }

                }
            }
        }

        public override bool HasCCOLElements() => true;
        
        public override bool HasFunctionLocalVariables() => true;

        public override IEnumerable<Tuple<string, string, string>> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCMeeverlengen:
                    return new List<Tuple<string, string, string>> { new Tuple<string, string, string>("int", "fc", "") };
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCMeeverlengen:
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
                case CCOLCodeTypeEnum.RegCMeeverlengen:
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    if (c.Fasen.Any(x => x.HardMeeverlengenFaseCycli.Any()))
                        sb.AppendLine($"{ts}{ts}YM[fc] &= ~BIT1;  /* reset BIT-sturing */");
                    sb.AppendLine($"{ts}{ts}YM[fc] &= ~BIT4;  /* reset BIT-sturing */");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();

                    var totigfunc = c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && c.Data.Intergroen ? "ym_max_toV1" : "ym_max_toV1";
                    var totigfuncCCOL = c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && c.Data.Intergroen ? "ym_max_to" : "ym_max_tig";

                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (fcm.Meeverlengen != NooitAltijdAanUitEnum.Nooit)
                        {
                            sb.Append($"{ts}YM[{fcm.GetDefine()}] |= ");
                            if (!fcm.MeeverlengenTypeInstelbaarOpStraat && fcm.Meeverlengen != NooitAltijdAanUitEnum.Altijd)
                            {
                                sb.Append($"SCH[{_schpf}{_schmv}{fcm.Naam}] && ");
                            }
                            var verschil = fcm.MeeverlengenVerschil?.ToString() ?? "NG";
                            var hf_wsg = "hf_wsg";
                            var hf_wsg_args = "";
                            if (c.Data.MultiModuleReeksen)
                            {
                                hf_wsg = "hf_wsg_fcfc";
                                var reeks = c.MultiModuleMolens.FirstOrDefault(x => x.Modules.Any(x2 => x2.Fasen.Any(x3 => x3.FaseCyclus == fcm.Naam)));
                                if (reeks != null)
                                {
                                    var rfc1 = c.Fasen.FirstOrDefault(x => reeks.Modules.SelectMany(x2 => x2.Fasen).Any(x3 => x3.FaseCyclus == x.Naam));
                                    var rfc2 = c.Fasen.LastOrDefault(x => reeks.Modules.SelectMany(x2 => x2.Fasen).Any(x3 => x3.FaseCyclus == x.Naam));
                                    if (rfc1 == null || rfc2 == null)
                                    {
                                        hf_wsg_args = "0, FCMAX";
                                    }
                                    else
                                    {
                                        var id2 = c.Fasen.IndexOf(rfc2);
                                        ++id2;
                                        hf_wsg_args = $"{_fcpf}{rfc1.Naam}, {(id2 == c.Fasen.Count ? "FCMAX" : $"{_fcpf}{c.Fasen[id2].Naam}")}";
                                    }
                                }
                                else
                                {
                                    hf_wsg_args = "0, FCMAX";
                                }
                            }
                            if(c.InterSignaalGroep.Nalopen.Any())
                            {
                                var nl = c.InterSignaalGroep.Nalopen.FirstOrDefault(x => x.FaseVan == fcm.Naam);
                                if (nl != null && nl.Type == NaloopTypeEnum.EindeGroen)
                                {
                                    hf_wsg = "hf_wsg_nl";
                                    if (c.Data.MultiModuleReeksen)
                                    {
                                        hf_wsg = "hf_wsg_nl_fcfc";
                                    }
                                }
                            }
                            if (!fcm.MeeverlengenTypeInstelbaarOpStraat)
                            {
                                switch (fcm.MeeverlengenType)
                                {
                                    case MeeVerlengenTypeEnum.Default:
                                        sb.AppendLine($"ym_maxV1({fcm.GetDefine()}, {verschil}) && {hf_wsg}({hf_wsg_args}) ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.To:
                                        sb.AppendLine($"{totigfunc}({fcm.GetDefine()}, {verschil}) && {hf_wsg}({hf_wsg_args}) ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.MKTo:
                                        sb.AppendLine($"(ym_maxV1({fcm.GetDefine()}, {verschil}) || {totigfunc}({fcm.GetDefine()}, {verschil}) && MK[{fcm.GetDefine()}]) && {hf_wsg}({hf_wsg_args}) ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.Voetganger:
                                        sb.AppendLine($"ym_max_vtgV1({fcm.GetDefine()}) && {hf_wsg}({hf_wsg_args}) ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.DefaultCCOL:
                                        sb.AppendLine($"ym_maxV1({fcm.GetDefine()}, {verschil}) && {hf_wsg}({hf_wsg_args}) ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.ToCCOL:
                                        sb.AppendLine($"{totigfuncCCOL}({fcm.GetDefine()}, {verschil}) && {hf_wsg}({hf_wsg_args}) ? BIT4 : 0;");
                                        break;
                                    case MeeVerlengenTypeEnum.MKToCCOL:
                                        sb.AppendLine($"(ym_max({fcm.GetDefine()}, {verschil}) || {totigfuncCCOL}({fcm.GetDefine()}, {verschil}) && MK[{fcm.GetDefine()}]) && {hf_wsg}({hf_wsg_args}) ? BIT4 : 0;");
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                            else
                            {
                                sb.AppendLine($"ym_max_prmV1({fcm.GetDefine()}, {_prmpf}{_prmmv}{fcm.Naam}, {verschil}) && {hf_wsg}({hf_wsg_args}) ? BIT4 : 0;");
                            }
                        }
                    }
                    var file = false;
                    var tts = ts;
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (fcm.Meeverlengen != Models.Enumerations.NooitAltijdAanUitEnum.Nooit)
                        {
                            var fm = c.FileIngrepen.FirstOrDefault(x => x.TeDoserenSignaalGroepen.Any(x2 => x2.FaseCyclus == fcm.Naam));
                            if (fm != null)
                            {
                                if (!file)
                                {
                                    file = true;
                                    sb.AppendLine();
                                    sb.AppendLine($"{ts}/* Niet meeverlengen tijdens file */");
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
                    foreach (FaseCyclusModel fcm in c.Fasen)
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
                                sb.Append($"{ts}if (");
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
