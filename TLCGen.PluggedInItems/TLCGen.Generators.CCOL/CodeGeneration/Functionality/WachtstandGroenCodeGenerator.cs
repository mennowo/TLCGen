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
        private List<CCOLElement> _MyElements;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schwg;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            foreach (FaseCyclusModel fcm in c.Fasen)
            {
                if (fcm.Wachtgroen != Models.Enumerations.NooitAltijdAanUitEnum.Nooit &&
                    fcm.Wachtgroen != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_schwg}{fcm.Naam}", 
                            fcm.Wachtgroen == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ? 1 : 0, 
                            CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
                }
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
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
                            sb.AppendLine($"{ts}aanvraag_wachtstand_exp({fcm.GetDefine()}, (bool) (SCH[{_schpf}{_schwg}{fcm.Naam}]));");
                        else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                            sb.AppendLine($"{ts}aanvraag_wachtstand_exp({fcm.GetDefine()}, TRUE);");
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCWachtgroen:
                    if(c.Data.ExtraMeeverlengenInWG)
                    {
                        sb.AppendLine($"{ts}/* Zet voor alle fasen het WS[] bitje. */");
                        sb.AppendLine($"{ts}WachtStand(PRML, ML, MLMAX);");
                        sb.AppendLine();
                    }
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
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                            fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                        {
                            sb.AppendLine($"{ts}WS[{fcm.GetDefine()}] = WG[{fcm.GetDefine()}] && SCH[schwg{fcm.Naam}] && yws_groen({fcm.GetDefine()});");
                        }
                        else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                        {
                            sb.AppendLine($"{ts}WS[{fcm.GetDefine()}] = WG[{fcm.GetDefine()}] && yws_groen({fcm.GetDefine()});");
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
