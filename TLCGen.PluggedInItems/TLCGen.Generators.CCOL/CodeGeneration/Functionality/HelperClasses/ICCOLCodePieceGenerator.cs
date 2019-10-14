using System;
using System.Collections.Generic;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public interface ICCOLCodePieceGenerator
    {
        void CollectCCOLElements(ControllerModel c);
        bool HasFunctionLocalVariables();
        IEnumerable<Tuple<string,string, string>> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type);
        bool HasCCOLElements();
        IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type);
        IEnumerable<CCOLElement> GetCCOLElements();
        bool HasCCOLBitmapOutputs();
        IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs();
        bool HasCCOLBitmapInputs();
        IEnumerable<CCOLIOElement> GetCCOLBitmapInputs();
        bool HasSimulationElements(ControllerModel c);
        IEnumerable<DetectorSimulatieModel> GetSimulationElements(ControllerModel c);
        int HasCode(CCOLCodeTypeEnum type);
        bool HasCodeForController(ControllerModel c, CCOLCodeTypeEnum type);
        string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts);
        bool HasSettings();
        bool SetSettings(CCOLGeneratorClassWithSettingsModel settings);
        List<string> GetSourcesToCopy();
    }
}
