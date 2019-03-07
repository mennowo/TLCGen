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
        private CCOLGeneratorCodeStringSettingModel _schmv; // schakelaar meeverlengen naam
#pragma warning restore 0649
        private string _hfile;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (FaseCyclusModel fcm in c.Fasen)
            {
                if (fcm.Meeverlengen != Models.Enumerations.NooitAltijdAanUitEnum.Nooit &&
                    fcm.Meeverlengen != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_schmv}{fcm.Naam}", 
                            (fcm.Meeverlengen == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ? 1 : 0), 
                            CCOLElementTimeTypeEnum.SCH_type, 
                            _schmv, fcm.Naam));
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

                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (fcm.Meeverlengen != Models.Enumerations.NooitAltijdAanUitEnum.Nooit)
                        {
                            sb.Append($"{ts}YM[{fcm.GetDefine()}] |= ");
                            if (fcm.Meeverlengen != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                            {
                                sb.Append($"SCH[{_schpf}{_schmv}{fcm.Naam}] && ");
                            }
                            var verschil = fcm.MeeverlengenVerschil?.ToString() ?? "NG";
                            var hf_wsg = "hf_wsg";
                            if(c.InterSignaalGroep.Nalopen.Any())
                            {
                                var nl = c.InterSignaalGroep.Nalopen.FirstOrDefault(x => x.FaseVan == fcm.Naam);
                                if (nl != null && nl.Type == NaloopTypeEnum.EindeGroen)
                                {
                                    hf_wsg = "hf_wsg_nl";
                                }
                            }
                            switch (fcm.MeeverlengenType)
                            {
                                case MeeVerlengenTypeEnum.Default:
                                    sb.AppendLine($"ym_maxV1({fcm.GetDefine()}, {verschil}) && {hf_wsg}() ? BIT4 : 0;");
                                    break;
                                case MeeVerlengenTypeEnum.To:
                                    sb.AppendLine($"{totigfunc}({fcm.GetDefine()}, {verschil}) && {hf_wsg}() ? BIT4 : 0;");
                                    break;
                                case MeeVerlengenTypeEnum.MKTo:
                                    sb.AppendLine($"(ym_maxV1({fcm.GetDefine()}, {verschil}) || {totigfunc}({fcm.GetDefine()}, {verschil}) && MK[{fcm.GetDefine()}]) && {hf_wsg}() ? BIT4 : 0;");
                                    break;
                                case MeeVerlengenTypeEnum.Voetganger:
                                    sb.AppendLine($"ym_max_vtgV1({fcm.GetDefine()}) && {hf_wsg}() ? BIT4 : 0;");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                    var file = false;
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
                                }
                                sb.AppendLine($"{ts}if (IH[{_hpf}{_hfile}{fm.Naam}]) YM[{_fcpf}{fcm.Naam}] &= ~BIT4;");
                            }
                        }
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

            return base.SetSettings(settings);
        }
    }
}
