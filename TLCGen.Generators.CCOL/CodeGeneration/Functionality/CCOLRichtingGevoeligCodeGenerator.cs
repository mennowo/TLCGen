using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration 
{
    [CCOLCodePieceGenerator]
    public class CCOLRichtingGevoeligCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

        private string _trgr;
        private string _trga;
        private string _trgv;
        private string _hrgv;
        private string _prmmkrg;
        private string _schrgad;

        private string _tkm; // read from settings provider, comes from other code gen object

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            foreach (RichtingGevoeligeAanvraagModel rga in c.RichtingGevoeligeAanvragen)
            {
                _MyElements.Add(
                    new CCOLElement(
                        $"{_trga}{_dpf}{rga.VanDetector}", 
                        rga.MaxTijdsVerschil, 
                        CCOLElementTimeTypeEnum.TE_type, 
                        CCOLElementTypeEnum.Timer));
                _MyElements.Add(
                    new CCOLElement(
                        $"{_schrgad}{_dpf}{rga.VanDetector}", 
                        1, 
                        CCOLElementTimeTypeEnum.SCH_type, 
                        CCOLElementTypeEnum.Schakelaar));
            }

            foreach (RichtingGevoeligVerlengModel rgv in c.RichtingGevoeligVerlengen)
            {
                _MyElements.Add(
                    new CCOLElement(
                        $"{_trgr}{_dpf}{rgv.VanDetector}_{_dpf}{rgv.NaarDetector}", 
                        rgv.MaxTijdsVerschil, 
                        CCOLElementTimeTypeEnum.TE_type, 
                        CCOLElementTypeEnum.Timer));
                _MyElements.Add(
                    new CCOLElement(
                        $"{_trgv}{_dpf}{rgv.VanDetector}_{_dpf}{rgv.NaarDetector}",
                        rgv.VerlengTijd,
                        CCOLElementTimeTypeEnum.TE_type,
                        CCOLElementTypeEnum.Timer));
                _MyElements.Add(
                    new CCOLElement(
                        $"{_hrgv}{_dpf}{rgv.VanDetector}_{_dpf}{rgv.NaarDetector}",
                        CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(
                    new CCOLElement(
                        $"{_prmmkrg}{_dpf}{rgv.VanDetector}",
                        (int)rgv.TypeVerlengen,
                        CCOLElementTimeTypeEnum.TE_type,
                        CCOLElementTypeEnum.Parameter));
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
                case CCOLRegCCodeTypeEnum.Meetkriterium:
                case CCOLRegCCodeTypeEnum.Aanvragen:
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
                case CCOLRegCCodeTypeEnum.Aanvragen:
                    sb.AppendLine($"{ts}/* Richtinggevoelige aanvragen */");
                    sb.AppendLine($"{ts}/* --------------------------= */");
                    foreach (RichtingGevoeligeAanvraagModel rga in c.RichtingGevoeligeAanvragen)
                    {
                        sb.AppendLine($"{ts}aanvraag_richtinggevoelig({_fcpf}{rga.FaseCyclus}, {_dpf}{rga.NaarDetector}, {_dpf}{rga.VanDetector}, {_tpf}{_trga}{_dpf}{rga.VanDetector}, SCH[{_schpf}{_schrgad}{_dpf}{rga.VanDetector}]);");
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.Meetkriterium:
                    sb.AppendLine($"{ts}/* Richtinggevoelig verlengen */");
                    sb.AppendLine($"{ts}/* -------------------------- */");
                    foreach (RichtingGevoeligVerlengModel rgv in c.RichtingGevoeligVerlengen)
                    {
                        sb.AppendLine($"{ts}MeetKriteriumRGprm((count) {_fcpf}{rgv.FaseCyclus}, (count) {_tpf}{_tkm}{rgv.FaseCyclus},");
                        sb.AppendLine($"{ts}{ts}(bool) RichtingVerlengen({_fcpf}{rgv.FaseCyclus}, {_dpf}{rgv.VanDetector}, {_dpf}{rgv.NaarDetector},");
                        sb.AppendLine($"{ts}{ts}                         {_tpf}{_trgr}{_dpf}{rgv.VanDetector}_{_dpf}{rgv.NaarDetector}, {_tpf}{_trgv}{_dpf}{rgv.VanDetector}_{_dpf}{rgv.NaarDetector},");
                        sb.AppendLine($"{ts}{ts}                         {_hpf}{_hrgv}{_dpf}{rgv.VanDetector}_{_dpf}{rgv.NaarDetector}), (mulv)PRM[{_prmpf}{_prmmkrg}{_dpf}{rgv.VanDetector}],");
                        sb.AppendLine($"{ts}{ts}(count)END);");
                    }
                    sb.AppendLine();
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _tkm = CCOLGeneratorSettingsProvider.Default.GetElementName("tkm");

            return base.SetSettings(settings);
        }
    }
}
