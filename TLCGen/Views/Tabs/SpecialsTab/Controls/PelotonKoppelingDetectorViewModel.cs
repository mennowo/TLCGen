using GalaSoft.MvvmLight;
using System;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class PelotonKoppelingDetectorViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        public PelotonKoppelingDetectorModel Detector { get; }

        public string DetectorNaam
        {
            get => Detector.DetectorNaam;
            set
            {
                Detector.DetectorNaam = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int KoppelSignaal
        {
            get => Detector.KoppelSignaal;
            set
            {
                Detector.KoppelSignaal = value;
                RaisePropertyChanged<object>(broadcast: true);
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
