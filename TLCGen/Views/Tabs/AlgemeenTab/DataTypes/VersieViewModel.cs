using System;
using GalaSoft.MvvmLight;
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
                RaisePropertyChanged<object>("Versie", broadcast: true);
            }
        }

        public DateTime Datum
        {
            get { return _VersieEntry.Datum; }
            set
            {
                _VersieEntry.Datum = value;
                RaisePropertyChanged<object>("Datum", broadcast: true);
            }
        }

        public string Ontwerper
        {
            get { return _VersieEntry.Ontwerper; }
            set
            {
                _VersieEntry.Ontwerper = value;
                RaisePropertyChanged<object>("Ontwerper", broadcast: true);
            }
        }

        public string Commentaar
        {
            get { return _VersieEntry.Commentaar; }
            set
            {
                _VersieEntry.Commentaar = value;
                RaisePropertyChanged<object>("Commentaar", broadcast: true);
            }
        }

        public VersieViewModel(VersieModel vm)
        {
            _VersieEntry = vm;
        }
    }
}
