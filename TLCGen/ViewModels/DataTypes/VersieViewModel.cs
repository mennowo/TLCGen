using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class VersieViewModel : ViewModelBase
    {
        private ControllerViewModel _ControllerVM;
        private VersieModel _VersieEntry;
        public VersieModel VersieEntry
        {
            get { return _VersieEntry; }
        }

        public string Versie
        {
            get { return _VersieEntry.Versie; }
            set
            {
                _VersieEntry.Versie = value;
                OnMonitoredPropertyChanged("Versie", _ControllerVM);
            }
        }

        public DateTime Datum
        {
            get { return _VersieEntry.Datum; }
            set
            {
                _VersieEntry.Datum = value;
                OnMonitoredPropertyChanged("Datum", _ControllerVM);
            }
        }

        public string Ontwerper
        {
            get { return _VersieEntry.Ontwerper; }
            set
            {
                _VersieEntry.Ontwerper = value;
                OnMonitoredPropertyChanged("Ontwerper", _ControllerVM);
            }
        }

        public string Commentaar
        {
            get { return _VersieEntry.Commentaar; }
            set
            {
                _VersieEntry.Commentaar = value;
                OnMonitoredPropertyChanged("Commentaar", _ControllerVM);
            }
        }

        public VersieViewModel(ControllerViewModel cvm, VersieModel vm)
        {
            _VersieEntry = vm;
            _ControllerVM = cvm;
        }
    }
}
