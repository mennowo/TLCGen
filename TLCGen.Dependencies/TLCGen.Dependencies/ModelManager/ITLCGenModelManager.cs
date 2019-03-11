using System;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ModelManagement
{
    public interface ITLCGenModelManager
    {
        ControllerModel Controller { get; set; }

        bool IsElementIdentifierUnique(TLCGenObjectTypeEnum objectType, string identifier, bool vissim = false);
        void InjectDefaultAction(Action<object, string> setDefaultsAction);
        bool CheckVersionOrder(ControllerModel controller);
        void CorrectModelByVersion(ControllerModel controller, string filename);
        void PrepareModelForUI(ControllerModel controller);
        void ChangeNameOnObject(object obj, string oldName, string newName, TLCGenObjectTypeEnum objectType);
    }
}
