using System.Collections.Generic;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public interface ICCOLCodePieceGenerator
    {
        int ElementGenerationOrder { get; }
        void CollectCCOLElements(ControllerModel c);
        IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type);
        bool HasCCOLElements();
        IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type);
        bool HasSimulationElements(ControllerModel c);
        IEnumerable<DetectorSimulatieModel> GetSimulationElements(ControllerModel c);
        int[] HasCode(CCOLCodeTypeEnum type);
        bool HasCodeForController(ControllerModel c, CCOLCodeTypeEnum type);
        string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order);
        bool HasSettings();
        bool SetSettings(CCOLGeneratorClassWithSettingsModel settings);
        List<string> GetSourcesToCopy();
    }
}
