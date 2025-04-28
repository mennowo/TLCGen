using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TLCGen.Plugins.AutoBuild
{
    public class AutoBuildConsoleTabViewModel : ObservableObject
    {
        public ObservableQueue<LogMessage> BuildAndRunConsoleOutput => Logger.ConsoleOutput;
    }
}
