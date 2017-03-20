using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.DataAccess
{
    public interface ITLCGenControllerDataProvider
    {
        ControllerModel Controller { get; }
        string ControllerFileName { get; }
        bool ControllerHasChanged { get; set; }

        bool NewController();
        bool OpenController(string controllername = null);
        bool SaveController();
        bool SaveControllerAs();
        bool CloseController();
        bool SetController(ControllerModel controller);
        bool CheckChanged();

#if DEBUG
        bool OpenDebug();
#endif
    }
}
