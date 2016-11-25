using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class NaloopViewModel : ViewModelBase
    {
        private NaloopModel _Naloop;

        public NaloopViewModel(NaloopModel nm)
        {
            _Naloop = nm;
        }
    }
}
