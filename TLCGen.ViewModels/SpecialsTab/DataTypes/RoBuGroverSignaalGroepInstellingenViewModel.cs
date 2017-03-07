using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class RoBuGroverSignaalGroepInstellingenViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        #region Fields

        private RoBuGroverFaseCyclusInstellingenModel _SignaalGroepInstellingen;

        private RoBuGroverFileDetectorViewModel _SelectedFileDetector;
        private RoBuGroverHiaatDetectorViewModel _SelectedHiaatDetector;

        #endregion // Fields

        #region Properties

        public RoBuGroverFileDetectorViewModel SelectedFileDetector
        {
            get { return _SelectedFileDetector; }
            set
            {
                _SelectedFileDetector = value;
                FileDetectorManager.SelectedDetector = value;
                OnMonitoredPropertyChanged("SelectedFileDetector");
            }
        }

        public RoBuGroverHiaatDetectorViewModel SelectedHiaatDetector
        {
            get { return _SelectedHiaatDetector; }
            set
            {
                _SelectedHiaatDetector = value;
                HiaatDetectorManager.SelectedDetector = value;
                OnMonitoredPropertyChanged("SelectedHiaatDetector");
            }
        }

        public string FaseCyclus
        {
            get { return _SignaalGroepInstellingen.FaseCyclus; }
            set
            {
                _SignaalGroepInstellingen.FaseCyclus = value;
                OnMonitoredPropertyChanged("FaseCyclus");
            }
        }
        public int MinGroenTijd
        {
            get { return _SignaalGroepInstellingen.MinGroenTijd; }
            set
            {
                _SignaalGroepInstellingen.MinGroenTijd = value;
                OnMonitoredPropertyChanged("MinGroenTijd");
            }
        }
        public int MaxGroenTijd
        {
            get { return _SignaalGroepInstellingen.MaxGroenTijd; }
            set
            {
                _SignaalGroepInstellingen.MaxGroenTijd = value;
                OnMonitoredPropertyChanged("MaxGroenTijd");
            }
        }

        public ObservableCollectionAroundList<RoBuGroverFileDetectorViewModel, RoBuGroverFileDetectorModel> FileDetectoren
        {
            get;
            private set;
        }

        public ObservableCollectionAroundList<RoBuGroverHiaatDetectorViewModel, RoBuGroverHiaatDetectorModel> HiaatDetectoren
        {
            get;
            private set;
        }

        private DetectorManagerViewModel<RoBuGroverFileDetectorViewModel, string> _FileDetectorManager;
        public DetectorManagerViewModel<RoBuGroverFileDetectorViewModel, string> FileDetectorManager
        {
            get
            {
                if (_FileDetectorManager == null)
                {
                    List<string> dets =
                        DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.
                            Where(x => x.Naam == FaseCyclus).
                            First().
                            Detectoren.
                            Select(x => x.Naam).
                            ToList();
                    _FileDetectorManager = new DetectorManagerViewModel<RoBuGroverFileDetectorViewModel, string>(
                        FileDetectoren,
                        dets,
                        (x) => 
                        {
                            RoBuGroverFileDetectorModel d = new RoBuGroverFileDetectorModel();
                            d.Detector = x;
                            DefaultsProvider.Default.SetDefaultsOnModel(d);
                            return new RoBuGroverFileDetectorViewModel(d);
                        },
                        (x) => { return !FileDetectoren.Where(y => y.Detector == x).Any(); }
                        );
                }
                return _FileDetectorManager;
            }
        }

        private DetectorManagerViewModel<RoBuGroverHiaatDetectorViewModel, string> _HiaatDetectorManager;
        public DetectorManagerViewModel<RoBuGroverHiaatDetectorViewModel, string> HiaatDetectorManager
        {
            get
            {
                if (_HiaatDetectorManager == null)
                {
                    List<string> dets =
                        DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.
                            Where(x => x.Naam == FaseCyclus).
                            First().
                            Detectoren.
                            Select(x => x.Naam).
                            ToList();
                    _HiaatDetectorManager = new DetectorManagerViewModel<RoBuGroverHiaatDetectorViewModel, string>(
                        HiaatDetectoren,
                        dets,
                        (x) =>
                        {
                            RoBuGroverHiaatDetectorModel d = new RoBuGroverHiaatDetectorModel();
                            d.Detector = x;
                            DefaultsProvider.Default.SetDefaultsOnModel(d);
                            return new RoBuGroverHiaatDetectorViewModel(d);
                        },
                        (x) => { return !HiaatDetectoren.Where(y => y.Detector == x).Any(); }
                        );
                }
                return _HiaatDetectorManager;
            }
        }

        #endregion Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public Methods

        #region TLCGen Events

        private void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            _FileDetectorManager = null;
            _HiaatDetectorManager = null;
            OnPropertyChanged("FileDetectorManager");
            OnPropertyChanged("HiaatDetectorManager");
        }

        #endregion // TLCGen Events

        #region IViewModelWithItem

        public object GetItem()
        {
            return _SignaalGroepInstellingen;
        }

        #endregion // IViewModelWithItem

        #region IComparable

        public int CompareTo(object obj)
        {
            if(obj is RoBuGroverSignaalGroepInstellingenViewModel)
            {
                return this.FaseCyclus.CompareTo(((RoBuGroverSignaalGroepInstellingenViewModel)obj).FaseCyclus);
            }
            return 0;
        }
        
        #endregion // IComparable

        #region Constructor

        public RoBuGroverSignaalGroepInstellingenViewModel(RoBuGroverFaseCyclusInstellingenModel signaalgroepinstellingen)
        {
            _SignaalGroepInstellingen = signaalgroepinstellingen;

            FileDetectoren = new ObservableCollectionAroundList<RoBuGroverFileDetectorViewModel, RoBuGroverFileDetectorModel>(_SignaalGroepInstellingen.FileDetectoren);
            HiaatDetectoren = new ObservableCollectionAroundList<RoBuGroverHiaatDetectorViewModel, RoBuGroverHiaatDetectorModel>(_SignaalGroepInstellingen.HiaatDetectoren);
        }

        #endregion // Constructor

    }
}
