using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ModelManagement
{
    public interface ITLCGenModelManager
    {
        ControllerModel Controller { get; set; }

	    void InjectDefaultAction(Action<object> setDefaultsAction);
        void CorrectModelByVersion(ControllerModel controller);
    }
}
