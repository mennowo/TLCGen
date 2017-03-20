using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLDetectieStoringGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;

#pragma warning disable 0649
        private string _dvak;
#pragma warning restore 0649
        private string _prmda;
        
        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
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
                case CCOLRegCCodeTypeEnum.DetectieStoring:
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
                case CCOLRegCCodeTypeEnum.DetectieStoring:
                    
                    sb.AppendLine($"{ts}/* reset MK-bits vooraf, ivm onderlinge verwijzing. */");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}MK[fc] &= ~BIT5;");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Vaste aanvraag bij detectie storing */");
                    foreach (var fc in c.Fasen)
                    {
                        foreach (var d in fc.Detectoren)
                        {
                            if(d.AanvraagBijStoring == Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                            {
                                sb.AppendLine($"{ts}{ts}A[{_fcpf}{fc.Naam}] |= (CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING);");
                            }
                            else if(d.AanvraagBijStoring == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ||
                                    d.AanvraagBijStoring == Models.Enumerations.NooitAltijdAanUitEnum.SchUit)
                            {
                                sb.AppendLine($"{ts}{ts}A[{_fcpf}{fc.Naam}] |= SCH[{_schpf}{_dvak}{_dpf}{d.Naam}] && (CIF_IS[{_dpf}{d.Naam}] >= CIF_DET_STORING);");
                            }
                        }
                    }
                    sb.AppendLine($"{ts}");

                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _prmda = CCOLGeneratorSettingsProvider.Default.GetElementName("prmda");

            return base.SetSettings(settings);
        }
    }
}
