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
            get => VersieEntry.Versie;
            set
            {
                VersieEntry.Versie = value;
                RaisePropertyChanged<object>(nameof(Versie), broadcast: true);
            }
        }

        public DateTime Datum
        {
            get => VersieEntry.Datum;
            set
            {
                VersieEntry.Datum = value;
                RaisePropertyChanged<object>(nameof(Datum), broadcast: true);
            }
        }

        public string Ontwerper
        {
            get => VersieEntry.Ontwerper;
            set
            {
                VersieEntry.Ontwerper = value;
                RaisePropertyChanged<object>(nameof(Ontwerper), broadcast: true);
            }
        }

        public string Commentaar
        {
            get => VersieEntry.Commentaar;
            set
            {
                VersieEntry.Commentaar = value;
                RaisePropertyChanged<object>(nameof(Commentaar), broadcast: true);
            }
        }

        public VersieViewModel(VersieModel vm)
        {
            VersieEntry = vm;
        }
    }
}
