using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using TLCGen.DataAccess;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class RoBuGroverSignaalGroepInstellingenViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        #region Fields

        private readonly RoBuGroverFaseCyclusInstellingenModel _signaalGroepInstellingen;
        private ItemsManagerViewModel<RoBuGroverFileDetectorViewModel, string> _fileDetectorManager;
        private ItemsManagerViewModel<RoBuGroverHiaatDetectorViewModel, string> _hiaatDetectorManager;

        #endregion // Fields

        #region Properties

        public RoBuGroverFileDetectorViewModel SelectedFileDetector
        {
            get => FileDetectorManager.SelectedItem;
            set
            {
                FileDetectorManager.SelectedItem = value;
                RaisePropertyChanged();
            }
        }

        public RoBuGroverHiaatDetectorViewModel SelectedHiaatDetector
        {
            get => HiaatDetectorManager.SelectedItem;
            set
            {
                HiaatDetectorManager.SelectedItem = value;
                RaisePropertyChanged();
            }
        }

        public string FaseCyclus
        {
            get => _signaalGroepInstellingen.FaseCyclus;
            set
            {
                _signaalGroepInstellingen.FaseCyclus = value;
                RaisePropertyChanged();
            }
        }
        
        public int MinGroenTijd
        {
            get => _signaalGroepInstellingen.MinGroenTijd;
            set
            {
                _signaalGroepInstellingen.MinGroenTijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int MaxGroenTijd
        {
            get => _signaalGroepInstellingen.MaxGroenTijd;
            set
            {
                _signaalGroepInstellingen.MaxGroenTijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public ObservableCollectionAroundList<RoBuGroverFileDetectorViewModel, RoBuGroverFileDetectorModel> FileDetectoren
        {
            get;
        }

        public ObservableCollectionAroundList<RoBuGroverHiaatDetectorViewModel, RoBuGroverHiaatDetectorModel> HiaatDetectoren
        {
            get;
        }

        public ItemsManagerViewModel<RoBuGroverFileDetectorViewModel, string> FileDetectorManager =>
            _fileDetectorManager ?? (_fileDetectorManager =
                new ItemsManagerViewModel<RoBuGroverFileDetectorViewModel, string>(
                    FileDetectoren,
                    ControllerAccessProvider.Default.AllSignalGroups.First(x => x.Naam == FaseCyclus)
                        .FaseCyclus.Detectoren.Select(x => x.Naam).ToList(),
                    x =>
                    {
                        var d = new RoBuGroverFileDetectorModel()
                        {
                            Detector = x
                        };
                        DefaultsProvider.Default.SetDefaultsOnModel(d);
                        return new RoBuGroverFileDetectorViewModel(d);
                    },
                    x => FileDetectoren.All(y => y.Detector != x),
                    x => SelectedFileDetector,
                    () => RaisePropertyChanged<object>(broadcast: true),
                    () => RaisePropertyChanged<object>(broadcast: true)));
        
        public ItemsManagerViewModel<RoBuGroverHiaatDetectorViewModel, string> HiaatDetectorManager =>
            _hiaatDetectorManager ?? (_hiaatDetectorManager =
                new ItemsManagerViewModel<RoBuGroverHiaatDetectorViewModel, string>(
                    HiaatDetectoren,
                    ControllerAccessProvider.Default.AllSignalGroups.First(x => x.Naam == FaseCyclus)
                        .FaseCyclus.Detectoren.Select(x => x.Naam).ToList(),
                    x =>
                    {
                        var d = new RoBuGroverHiaatDetectorModel()
                        {
                            Detector = x
                        };
                        DefaultsProvider.Default.SetDefaultsOnModel(d);
                        return new RoBuGroverHiaatDetectorViewModel(d);
                    },
                    x => HiaatDetectoren.All(y => y.Detector != x),
                    x => SelectedHiaatDetector,
                    OnHiaatDetectorListChanged,
                    OnHiaatDetectorListChanged));

        #endregion Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        private void OnHiaatDetectorListChanged()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ControllerDataChangedMessage());
            RaisePropertyChanged(nameof(SelectedHiaatDetector));
        }

        #endregion // Private methods

        #region Public methods

        #endregion // Public Methods

        #region TLCGen Events

        private void OnDetectorenChanged(DetectorenChangedMessage msg)
        {
            _fileDetectorManager?.Refresh();
            _hiaatDetectorManager?.Refresh();
        }

        private void OnNameChanged(NameChangedMessage msg)
        {
            if (msg.ObjectType != TLCGenObjectTypeEnum.Detector) return;
            _fileDetectorManager?.Refresh();
            _hiaatDetectorManager?.Refresh();
        }

        #endregion // TLCGen Events

        #region IViewModelWithItem

        public object GetItem()
        {
            return _signaalGroepInstellingen;
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
            _signaalGroepInstellingen = signaalgroepinstellingen;

            FileDetectoren = new ObservableCollectionAroundList<RoBuGroverFileDetectorViewModel, RoBuGroverFileDetectorModel>(_signaalGroepInstellingen.FileDetectoren);
            HiaatDetectoren = new ObservableCollectionAroundList<RoBuGroverHiaatDetectorViewModel, RoBuGroverHiaatDetectorModel>(_signaalGroepInstellingen.HiaatDetectoren);

            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
        }

        #endregion // Constructor

    }
}
