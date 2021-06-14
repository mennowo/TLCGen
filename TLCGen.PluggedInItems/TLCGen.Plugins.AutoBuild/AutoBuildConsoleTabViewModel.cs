using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace TLCGen.Plugins.AutoBuild
{
    public class AutoBuildConsoleTabViewModel : ViewModelBase
    {
        public ObservableQueue<LogMessage> BuildAndRunConsoleOutput => Logger.ConsoleOutput;
    }
}
