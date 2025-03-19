using System;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class VersieViewModel : ObservableObjectEx
    {
        public VersieModel VersieEntry { get; }

        public string Versie
        {
            get => VersieEntry.Versie;
            set
            {
                VersieEntry.Versie = value;
                OnPropertyChanged(nameof(Versie), broadcast: true);
            }
        }

        public DateTime Datum
        {
            get => VersieEntry.Datum;
            set
            {
                VersieEntry.Datum = value;
                OnPropertyChanged(nameof(Datum), broadcast: true);
            }
        }

        public string Ontwerper
        {
            get => VersieEntry.Ontwerper;
            set
            {
                VersieEntry.Ontwerper = value;
                OnPropertyChanged(nameof(Ontwerper), broadcast: true);
            }
        }

        public string Commentaar
        {
            get => VersieEntry.Commentaar;
            set
            {
                VersieEntry.Commentaar = value;
                OnPropertyChanged(nameof(Commentaar), broadcast: true);
            }
        }

        public VersieViewModel(VersieModel vm)
        {
            VersieEntry = vm;
        }
    }
}
