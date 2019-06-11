using System;
using GalaSoft.MvvmLight;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class VersieViewModel : ViewModelBase
    {
        public VersieModel VersieEntry { get; }

        public string Versie
        {
            get { return VersieEntry.Versie; }
            set
            {
                VersieEntry.Versie = value;
                RaisePropertyChanged<object>("Versie", broadcast: true);
            }
        }

        public DateTime Datum
        {
            get { return VersieEntry.Datum; }
            set
            {
                VersieEntry.Datum = value;
                RaisePropertyChanged<object>("Datum", broadcast: true);
            }
        }

        public string Ontwerper
        {
            get { return VersieEntry.Ontwerper; }
            set
            {
                VersieEntry.Ontwerper = value;
                RaisePropertyChanged<object>("Ontwerper", broadcast: true);
            }
        }

        public string Commentaar
        {
            get { return VersieEntry.Commentaar; }
            set
            {
                VersieEntry.Commentaar = value;
                RaisePropertyChanged<object>("Commentaar", broadcast: true);
            }
        }

        public VersieViewModel(VersieModel vm)
        {
            VersieEntry = vm;
        }
    }
}
