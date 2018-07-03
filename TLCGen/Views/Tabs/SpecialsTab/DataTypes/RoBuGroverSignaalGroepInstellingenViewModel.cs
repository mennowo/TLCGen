using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
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

        #endregion // Fields

        #region Properties

        public RoBuGroverFileDetectorViewModel SelectedFileDetector
        {
            get { return FileDetectorManager.SelectedItem; }
            set
            {
                FileDetectorManager.SelectedItem = value;
                RaisePropertyChanged("SelectedFileDetector");
            }
        }

        public RoBuGroverHiaatDetectorViewModel SelectedHiaatDetector
        {
            get { return HiaatDetectorManager.SelectedItem; }
            set
            {
                HiaatDetectorManager.SelectedItem = value;
                RaisePropertyChanged("SelectedHiaatDetector");
            }
        }

        public string FaseCyclus
        {
            get { return _SignaalGroepInstellingen.FaseCyclus; }
            set
            {
                _SignaalGroepInstellingen.FaseCyclus = value;
                RaisePropertyChanged("FaseCyclus");
            }
        }
        public int MinGroenTijd
        {
            get { return _SignaalGroepInstellingen.MinGroenTijd; }
            set
            {
                _SignaalGroepInstellingen.MinGroenTijd = value;
                RaisePropertyChanged("MinGroenTijd");
            }
        }
        public int MaxGroenTijd
        {
            get { return _SignaalGroepInstellingen.MaxGroenTijd; }
            set
            {
                _SignaalGroepInstellingen.MaxGroenTijd = value;
                RaisePropertyChanged("MaxGroenTijd");
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

        private ItemsManagerViewModel<RoBuGroverFileDetectorViewModel, string> _FileDetectorManager;
        public ItemsManagerViewModel<RoBuGroverFileDetectorViewModel, string> FileDetectorManager
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
                    _FileDetectorManager = new ItemsManagerViewModel<RoBuGroverFileDetectorViewModel, string>(
                        FileDetectoren,
                        dets,
                        (x) => 
                        {
                            var d = new RoBuGroverFileDetectorModel()
                            {
                                Detector = x
                            };
                            DefaultsProvider.Default.SetDefaultsOnModel(d);
                            return new RoBuGroverFileDetectorViewModel(d);
                        },
                        (x) => { return !FileDetectoren.Where(y => y.Detector == x).Any(); },
                        (x) => { return SelectedFileDetector; },
                        () => 
                        {
                            RaisePropertyChanged<object>(broadcast: true);
                        },
                        () => 
                        {
                            RaisePropertyChanged<object>(broadcast: true);
                        }
                        );
                }
                return _FileDetectorManager;
            }
        }

        private ItemsManagerViewModel<RoBuGroverHiaatDetectorViewModel, string> _HiaatDetectorManager;
        public ItemsManagerViewModel<RoBuGroverHiaatDetectorViewModel, string> HiaatDetectorManager
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
                    _HiaatDetectorManager = new ItemsManagerViewModel<RoBuGroverHiaatDetectorViewModel, string>(
                        HiaatDetectoren,
                        dets,
                        (x) =>
                        {
                            var d = new RoBuGroverHiaatDetectorModel()
                            {
                                Detector = x
                            };
                            DefaultsProvider.Default.SetDefaultsOnModel(d);
                            return new RoBuGroverHiaatDetectorViewModel(d);
                        },
                        (x) => { return !HiaatDetectoren.Where(y => y.Detector == x).Any(); },
                        (x) => { return SelectedHiaatDetector; },
                        () => 
                        {
                            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ControllerDataChangedMessage());
                            RaisePropertyChanged("SelectedHiaatDetector");
                        },
                        () => 
                        {
                            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ControllerDataChangedMessage());
                            RaisePropertyChanged("SelectedHiaatDetector");
                        }
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
            RaisePropertyChanged("FileDetectorManager");
            RaisePropertyChanged("HiaatDetectorManager");
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
