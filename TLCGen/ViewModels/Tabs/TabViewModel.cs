using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.ViewModels
{
    public class TabViewModel : ViewModelBase
    {
        protected ControllerViewModel _ControllerVM;

        public TabViewModel(ControllerViewModel controllervm)
        {
            _ControllerVM = controllervm;
        }
    }
}
