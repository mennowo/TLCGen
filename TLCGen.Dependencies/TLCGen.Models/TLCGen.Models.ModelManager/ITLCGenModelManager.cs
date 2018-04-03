using System;
using TLCGen.Models;

namespace TLCGen.ModelManagement
{
    public interface ITLCGenModelManager
    {
        ControllerModel Controller { get; set; }

	    void InjectDefaultAction(Action<object> setDefaultsAction);
        bool CheckVersionOrder(ControllerModel controller);
        void CorrectModelByVersion(ControllerModel controller);
    }
}
