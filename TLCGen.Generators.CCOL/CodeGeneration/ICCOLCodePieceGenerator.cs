using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public interface ICCOLCodePieceGenerator
    {
        void CollectCCOLElements(ControllerModel c);
        IEnumerable<CCOLElement> GetCCOLElements(CCOLElementType type);
        IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs();
        IEnumerable<CCOLIOElement> GetCCOLBitmapInputs();
        string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string tabspace);
    }
}
