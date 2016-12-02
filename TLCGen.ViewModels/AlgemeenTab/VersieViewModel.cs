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
                OnMonitoredPropertyChanged("Versie");
            }
        }

        public DateTime Datum
        {
            get { return _VersieEntry.Datum; }
            set
            {
                _VersieEntry.Datum = value;
                OnMonitoredPropertyChanged("Datum");
            }
        }

        public string Ontwerper
        {
            get { return _VersieEntry.Ontwerper; }
            set
            {
                _VersieEntry.Ontwerper = value;
                OnMonitoredPropertyChanged("Ontwerper");
            }
        }

        public string Commentaar
        {
            get { return _VersieEntry.Commentaar; }
            set
            {
                _VersieEntry.Commentaar = value;
                OnMonitoredPropertyChanged("Commentaar");
            }
        }

        public VersieViewModel(VersieModel vm)
        {
            _VersieEntry = vm;
        }
    }
}
