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

        private string _schpf; // schakelaar prefix
        private string _schmv; // schakelaar meeverlengen naam

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

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Meeverlengen:
                    return true;
                default:
                    return false;
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
                            string verschil = fcm.MeeverlengenVerschil.HasValue ? fcm.MeeverlengenVerschil.Value.ToString() : "NG";
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
                            }
                        }
                    }
                    sb.AppendLine();
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool HasSettings()
        {
            return true;
        }

        public override void SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            foreach (var s in settings.Settings)
            {
                if (s.Default == "mv") _schmv = s.Setting == null ? s.Default : s.Setting;
            }

            _schpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("sch");
        }
    }
}
