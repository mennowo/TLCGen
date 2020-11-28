using System;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.DataAccess
{
    public interface ITLCGenControllerDataProvider
    {
        ControllerModel Controller { get; }
        ITLCGenGenerator CurrentGenerator { get; set; }
        string ControllerFileName { get; set; }
        bool ControllerHasChanged { get; set; }

        bool NewController();
        bool OpenController(string controllername = null);
        bool SaveController();
        bool SaveControllerAs();
        bool CloseController();
        bool SetController(ControllerModel controller);
        bool CheckChanged();

	    void InjectDefaultAction(Action<object> setDefaultsAction);

#if DEBUG
        bool OpenDebug();
#endif
    }
}
