using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TLCGen.Models;

namespace TLCGen.Plugins
{
    public interface ITLCGenToolBar : ITLCGenPlugin
    {
        bool IsVisible { get; set; }
        bool IsEnabled { get; set; }
        UserControl ToolBarView { get; }
    }
}
