using System.Collections.Generic;
using System.Linq;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class FixatieCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _isfix;
        private CCOLGeneratorCodeStringSettingModel _schbmfix;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myBitmapInputs = new List<CCOLIOElement>();

            if (c.Data.FixatieData.FixatieMogelijk)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_schbmfix}",
                        c.Data.FixatieData.BijkomenTijdensFixatie ? 1 : 0,
                        CCOLElementTimeTypeEnum.SCH_type,
                        _schbmfix));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_isfix}",
                        _isfix));
                _myBitmapInputs.Add(new CCOLIOElement(c.Data.FixatieData.FixatieBitmapData as IOElementModel, $"{_ispf}{_isfix}"));
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _myElements.Where(x => x.Type == type);
        }

        public override bool HasCCOLBitmapInputs()
        {
            return true;
        }

        public override IEnumerable<CCOLIOElement> GetCCOLBitmapInputs()
        {
            return _myBitmapInputs;
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            return 0;
        }
    }
}
