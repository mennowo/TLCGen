using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using TLCGen.Plugins.DynamischHiaat.Models;
using TLCGen.Helpers;
using TLCGen.Integrity;

namespace TLCGen.Plugins.DynamischHiaat.ViewModels
{
    internal class DynamischHiaatDetectorViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        #region Fields
        #endregion // Fields

        #region Properties

        [Browsable(false)]
        public DynamischHiaatDetectorModel Detector { get; }

        [Browsable(false)]
        public string DetectorName => Detector.DetectorName;

        public int Moment1
        {
            get => Detector.Moment1;
            set
            {
                Detector.Moment1 = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int Moment2
        {
            get => Detector.Moment2;
            set
            {
                Detector.Moment2 = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int TDH1
        {
            get => Detector.TDH1;
            set
            {

                Detector.TDH1 = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int TDH2
        {
            get => Detector.TDH2;
            set
            {
                Detector.TDH2 = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int Maxtijd
        {
            get => Detector.Maxtijd;
            set
            {
                Detector.Maxtijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool SpringStart
        {
            get => Detector.SpringStart;
            set
            {
                Detector.SpringStart = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool VerlengNiet
        {
            get => Detector.VerlengNiet;
            set
            {
                Detector.VerlengNiet = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool VerlengExtra
        {
            get => Detector.VerlengExtra;
            set
            {
                Detector.VerlengExtra = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool DirectAftellen
        {
            get => Detector.DirectAftellen;
            set
            {
                Detector.DirectAftellen = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool SpringGroen
        {
            get => Detector.SpringGroen;
            set
            {
                Detector.SpringGroen = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return Detector;
        }

        #endregion // IViewModelWithItem

        #region IComparable

        public int CompareTo(object obj)
        {
            if (!(obj is DynamischHiaatDetectorViewModel d2)) throw new InvalidCastException();
            return TLCGenIntegrityChecker.CompareDetectors(DetectorName, d2.DetectorName, null, null);
        }

        #endregion // IComparable

        #region Constructor

        public DynamischHiaatDetectorViewModel(DynamischHiaatDetectorModel detector)
        {
            Detector = detector;
        }

        #endregion // Constructor
    }
}
