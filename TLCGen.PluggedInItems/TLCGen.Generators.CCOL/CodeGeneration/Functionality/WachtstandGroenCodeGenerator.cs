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
    public class WachtstandGroenCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schwg;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (FaseCyclusModel fcm in c.Fasen)
            {
                if (fcm.Wachtgroen != NooitAltijdAanUitEnum.Nooit &&
                    fcm.Wachtgroen != NooitAltijdAanUitEnum.Altijd)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_schwg}{fcm.Naam}", fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schwg, fcm.Naam));
                }
            }
        }

        public override bool HasCCOLElements() => true;
        
        public override bool HasFunctionLocalVariables() => true;

        public override IEnumerable<Tuple<string, string, string>> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCWachtgroen:
                    return new List<Tuple<string, string, string>> { new Tuple<string, string, string>("int", "fc", "") };
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCWachtgroen:
                    return 10;
                case CCOLCodeTypeEnum.RegCAanvragen:
                    return 60;
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
                    sb.AppendLine($"{ts}/* Wachtstand groen aanvragen */");
                    sb.AppendLine($"{ts}/* -------------------------- */");
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                            fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                            sb.AppendLine($"{ts}aanvraag_wachtstand_exp({fcm.GetDefine()}, ({c.GetBoolV()}) (SCH[{_schpf}{_schwg}{fcm.Naam}]));");
                        else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                            sb.AppendLine($"{ts}aanvraag_wachtstand_exp({fcm.GetDefine()}, TRUE);");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCWachtgroen:
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}RW[fc] &= ~BIT4;  /* reset BIT-sturing */");
                    sb.AppendLine();
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                            fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                        {
                            sb.Append($"{ts}RW[{fcm.GetDefine()}] |= (");
                            if(c.Data.ExtraMeeverlengenInWG && fcm.Type != FaseTypeEnum.Voetganger)
                            {
                                sb.AppendLine($"(MK[{fcm.GetDefine()}] & ~BIT5) ||");
                                sb.Append("".PadLeft($"{ts}RW[{fcm.GetDefine()}] |= (".Length));
                            }
                            sb.AppendLine($"SCH[{_schpf}{_schwg}{fcm.Naam}] && yws_groen({fcm.GetDefine()})) && !fka({fcm.GetDefine()}) ? BIT4 : 0;");
                        }
                        else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                        {
                            sb.AppendLine($"{ts}RW[{fcm.GetDefine()}] |= (yws_groen({fcm.GetDefine()})) && !fka({fcm.GetDefine()}) ? BIT4 : 0;");
                        }
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}WS[fc] &= ~BIT4;  /* reset BIT-sturing */");
                    sb.AppendLine($"{ts}{ts}WS[fc] |= (RW[fc] & BIT4) ? BIT4 : 0;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    if (c.Data.ExtraMeeverlengenInWG)
                    {
                        sb.AppendLine($"{ts}/* Zet voor alle fasen het WS[] bitje. */");
                        if (!c.Data.MultiModuleReeksen)
                        {
                            sb.AppendLine($"{ts}WachtStand(PRML, ML, MLMAX);");
                        }
                        else
                        {
                            foreach (var r in c.MultiModuleMolens.Where(x => x.Modules.Any(x2 => x2.Fasen.Any())))
                            {
                                sb.AppendLine($"{ts}WachtStand(PR{r.Reeks}, {r.Reeks}, {r.Reeks}MAX);");
                            }
                        }
                        sb.AppendLine();
                    }
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}WS[fc] &= ~BIT1;  /* reset BIT-sturing */");
                    sb.AppendLine();

                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                            fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                        {
                            sb.AppendLine($"{ts}WS[{fcm.GetDefine()}] |= (WG[{fcm.GetDefine()}] && SCH[{_schpf}{_schwg}{fcm.Naam}] && yws_groen({fcm.GetDefine()})) ? BIT1 : 0;");
                        }
                        else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                        {
                            sb.AppendLine($"{ts}WS[{fcm.GetDefine()}] |= (WG[{fcm.GetDefine()}] && yws_groen({fcm.GetDefine()})) ? BIT1 : 0;");
                        }
                    }
                    return sb.ToString();

                default:
                    return null;
            }
        }
    }
}
