using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public interface ICCOLCodePieceGenerator
    {
        void CollectCCOLElements(ControllerModel c);
        bool HasCCOLElements();
        IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type);
        bool HasCCOLBitmapOutputs();
        IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs();
        bool HasCCOLBitmapInputs();
        IEnumerable<CCOLIOElement> GetCCOLBitmapInputs();
        bool HasCode(CCOLRegCCodeTypeEnum type);
        string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string tabspace);
        bool HasSettings();
        void SetSettings(CCOLGeneratorClassWithSettingsModel settings);
    }
}
