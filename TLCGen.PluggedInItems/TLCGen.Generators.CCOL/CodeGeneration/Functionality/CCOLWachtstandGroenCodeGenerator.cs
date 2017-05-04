using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLWachtstandGroenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

#pragma warning disable 0649
        private string _schwg;
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

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Wachtgroen:
                case CCOLRegCCodeTypeEnum.Aanvragen:
                    return true;
                default:
                    return false;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string tabspace)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Aanvragen:
                    sb.AppendLine($"{tabspace}/* Wachtstand groen aanvragen */");
                    sb.AppendLine($"{tabspace}/* -------------------------- */");
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                            fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                            sb.AppendLine($"{tabspace}aanvraag_wachtstand_exp({fcm.GetDefine()}, (bool) (SCH[{_schpf}{_schwg}{fcm.Naam}]));");
                        else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                            sb.AppendLine($"{tabspace}aanvraag_wachtstand_exp({fcm.GetDefine()}, TRUE);");
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.Wachtgroen:
                    if(c.Data.ExtraMeeverlengenInWG)
                    {
                        sb.AppendLine($"{tabspace}/* Zet voor alle fasen het WS[] bitje. */");
                        sb.AppendLine($"{tabspace}WachtStand(PRML, ML, MLMAX);");
                        sb.AppendLine();
                    }
                    sb.AppendLine($"{tabspace}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{tabspace}{tabspace}RW[fc] &= ~BIT4;  /* reset BIT-sturing */");
                    sb.AppendLine();
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                            fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                        {
                            sb.Append($"{tabspace}RW[{fcm.GetDefine()}] |= (");
                            if(c.Data.ExtraMeeverlengenInWG && fcm.Type != FaseTypeEnum.Voetganger)
                            {
                                sb.AppendLine($"(MK[{fcm.GetDefine()}] & ~BIT5) ||");
                                sb.Append("".PadLeft($"{tabspace}RW[{fcm.GetDefine()}] |= (".Length));
                            }
                            sb.AppendLine($"SCH[{_schpf}{_schwg}{fcm.Naam}] && yws_groen({fcm.GetDefine()})) && !fka({fcm.GetDefine()}) ? BIT4 : 0;");
                        }
                        else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                        {
                            sb.AppendLine($"{tabspace}RW[{fcm.GetDefine()}] |= (yws_groen({fcm.GetDefine()})) && !fka({fcm.GetDefine()}) ? BIT4 : 0;");
                        }
                    }
                    sb.AppendLine();
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        if (fcm.Wachtgroen == NooitAltijdAanUitEnum.SchAan ||
                            fcm.Wachtgroen == NooitAltijdAanUitEnum.SchUit)
                        {
                            sb.AppendLine($"{tabspace}WS[{fcm.GetDefine()}] = WG[{fcm.GetDefine()}] && SCH[schwg{fcm.Naam}] && yws_groen({fcm.GetDefine()});");
                        }
                        else if (fcm.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
                        {
                            sb.AppendLine($"{tabspace}WS[{fcm.GetDefine()}] = WG[{fcm.GetDefine()}] && yws_groen({fcm.GetDefine()});");
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
