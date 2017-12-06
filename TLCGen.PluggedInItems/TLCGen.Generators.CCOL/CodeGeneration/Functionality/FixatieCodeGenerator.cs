using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class FixatieCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapInputs;

#pragma warning disable 0649
        private string _isfix;
        private string _schbmfix;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyBitmapInputs = new List<CCOLIOElement>();

            if (c.Data.FixatieData.FixatieMogelijk)
            {
                _MyElements.Add(
                    new CCOLElement(
                        $"{_isfix}", 
                        CCOLElementTypeEnum.Ingang));
                _MyElements.Add(
                    new CCOLElement(
                        $"{_schbmfix}",
                        c.Data.FixatieData.BijkomenTijdensFixatie ? 1 : 0,
                        CCOLElementTimeTypeEnum.SCH_type,
                        CCOLElementTypeEnum.Schakelaar));
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

        public override int HasCode(CCOLRegCCodeTypeEnum type)
        {
            return 0;
        }
    }
}
