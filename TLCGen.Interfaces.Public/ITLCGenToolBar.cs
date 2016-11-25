using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TLCGen.Plugins
{
    public interface ITLCGenToolBar : ITLCGenPlugin
    {
        bool IsEnabled { get; set; }
        ToolBar ToolBarControl { get; }
    }
}
