using CommunityToolkit.Mvvm.ComponentModel;
using System;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class PelotonKoppelingDetectorViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        public PelotonKoppelingDetectorModel Detector { get; }

        public string DetectorNaam
        {
            get => Detector.DetectorNaam;
            set
            {
                Detector.DetectorNaam = value;
                OnPropertyChanged(broadcast: true);
            }
        }
        
        public PelotonKoppelingDetectorViewModel(PelotonKoppelingDetectorModel detector)
        {
            Detector = detector;
        }

        public object GetItem()
        {
            return Detector;
        }

        public int CompareTo(object obj)
        {
            return this.DetectorNaam.CompareTo(((PelotonKoppelingDetectorViewModel)obj).DetectorNaam);
        }
    }
}
