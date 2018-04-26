using System;
using TLCGen.Messaging;
using TLCGen.Models;

namespace TLCGen.ModelManagement
{
    public interface ITLCGenModelManager
    {
        ControllerModel Controller { get; set; }

        bool IsElementIdentifierUnique(TLCGenObjectTypeEnum objectType, string identifier, bool vissim = false);
        void InjectDefaultAction(Action<object, string> setDefaultsAction);
        bool CheckVersionOrder(ControllerModel controller);
        void CorrectModelByVersion(ControllerModel controller);
        void ChangeNameOnObject(object obj, string oldName, string newName);
    }
}
