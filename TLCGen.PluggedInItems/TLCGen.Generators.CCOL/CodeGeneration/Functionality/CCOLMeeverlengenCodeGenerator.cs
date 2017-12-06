using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLMeeverlengenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

#pragma warning disable 0649
        private string _schmv; // schakelaar meeverlengen naam
#pragma warning restore 0649
        private string _hfile;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            foreach (FaseCyclusModel fcm in c.Fasen)
            {
                if (fcm.Meeverlengen != Models.Enumerations.NooitAltijdAanUitEnum.Nooit &&
                    fcm.Meeverlengen != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_schmv}{fcm.Naam}", 
                            (fcm.Meeverlengen == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ? 1 : 0), 
                            CCOLElementTimeTypeEnum.SCH_type, 
                            CCOLElementTypeEnum.Schakelaar));
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

        public override int HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Meeverlengen:
                    return 10;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Meeverlengen:
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}YM[fc] &= ~BIT4;  /* reset BIT-sturing */");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
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
                            switch (fcm.MeeverlengenType)
                            {
                                case MeeVerlengenTypeEnum.Default:
                                    sb.AppendLine($"ym_maxV1({fcm.GetDefine()}, {verschil}) && hf_wsg() ? BIT4 : 0;");
                                    break;
                                case MeeVerlengenTypeEnum.To:
                                    sb.AppendLine($"ym_max_to({fcm.GetDefine()}, {verschil}) && hf_wsg() ? BIT4 : 0;");
                                    break;
                                case MeeVerlengenTypeEnum.MKTo:
                                    sb.AppendLine($"(ym_max({fcm.GetDefine()}, {verschil}) || ym_max_to({fcm.GetDefine()}, {verschil}) && MK[{fcm.GetDefine()}]) && hf_wsg() ? BIT4 : 0;");
                                    break;
                                case MeeVerlengenTypeEnum.Voetganger:
                                    sb.AppendLine($"ym_max_vtgV1({fcm.GetDefine()}) && hf_wsg() ? BIT4 : 0;");
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
                    }
                    sb.AppendLine();
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
