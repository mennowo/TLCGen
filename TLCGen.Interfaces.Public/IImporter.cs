using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TLCGen.Models;

namespace TLCGen.Interfaces.Public
{
    public interface IImporter : ITLCGenAddin
    {
        bool ImportsIntoExisting { get; }
        //MenuItem ImporterMenuItem { get; }

        ControllerModel ImportController(ControllerModel c = null);
    }
}
