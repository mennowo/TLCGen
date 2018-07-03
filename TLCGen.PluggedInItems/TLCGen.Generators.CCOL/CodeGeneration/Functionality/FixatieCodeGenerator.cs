using System.Collections.Generic;
using System.Linq;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class FixatieCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapInputs;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _isfix;
        private CCOLGeneratorCodeStringSettingModel _schbmfix;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyBitmapInputs = new List<CCOLIOElement>();

            if (c.Data.FixatieData.FixatieMogelijk)
            {
                _MyElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_schbmfix}",
                        c.Data.FixatieData.BijkomenTijdensFixatie ? 1 : 0,
                        CCOLElementTimeTypeEnum.SCH_type,
                        _schbmfix));
                _MyElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_isfix}",
                        _isfix));
                _MyBitmapInputs.Add(new CCOLIOElement(c.Data.FixatieData.FixatieBitmapData as IOElementModel, $"{_ispf}{_isfix}"));
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

        public override bool HasCCOLBitmapInputs()
        {
            return true;
        }

        public override IEnumerable<CCOLIOElement> GetCCOLBitmapInputs()
        {
            return _MyBitmapInputs;
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            return 0;
        }
    }
}
