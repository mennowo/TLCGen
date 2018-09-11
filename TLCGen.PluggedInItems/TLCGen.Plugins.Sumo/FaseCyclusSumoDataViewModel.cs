using GalaSoft.MvvmLight;
using System;
using TLCGen.Helpers;

namespace TLCGen.Plugins.Sumo
{
    public class FaseCyclusSumoDataViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        public FaseCyclusSumoDataModel FaseCyclus { get; }

        public string Naam
        {
            get => FaseCyclus.Naam;
            set
            {
                FaseCyclus.Naam = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string SumoIds
        {
            get => FaseCyclus.SumoIds;
            set
            {
                FaseCyclus.SumoIds = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public FaseCyclusSumoDataViewModel(FaseCyclusSumoDataModel faseCyclus)
        {
            FaseCyclus = faseCyclus;
        }

        public object GetItem()
        {
            return FaseCyclus;
        }

        public int CompareTo(object obj)
        {
            return this.Naam.CompareTo(((FaseCyclusSumoDataViewModel)obj).Naam);
        }
    }

    public class DetectorSumoDataViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        public DetectorSumoDataModel Detector { get; }

        public string Naam
        {
            get => Detector.Naam;
            set
            {
                Detector.Naam = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string SumoNaam1
        {
            get => Detector.SumoNaam1;
            set
            {
                Detector.SumoNaam1 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string SumoNaam2
        {
            get => Detector.SumoNaam2;
            set
            {
                Detector.SumoNaam2 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string SumoNaam3
        {
            get => Detector.SumoNaam3;
            set
            {
                Detector.SumoNaam3 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool Selectief
        {
            get => Detector.Selectief;
            set
            {
                Detector.Selectief = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public DetectorSumoDataViewModel(DetectorSumoDataModel detector)
        {
            Detector = detector;
        }

        public object GetItem()
        {
            return Detector;
        }

        public int CompareTo(object obj)
        {
            return this.Naam.CompareTo(((DetectorSumoDataViewModel)obj).Naam);
        }
    }
}
