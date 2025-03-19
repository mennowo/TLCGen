using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class RatelTikkerViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        #region Fields

        private RatelTikkerModel _RatelTikker;

        #endregion // Fields

        #region Properties

        public RateltikkerTypeEnum Type
        {
            get => _RatelTikker.Type;
            set
            {
                _RatelTikker.Type = value;
                OnPropertyChanged(nameof(Type), broadcast: true);
                WeakReferenceMessenger.Default.Send(new RatelTikkerTypeChangedMessage(_RatelTikker));
            }
        }

        public int NaloopTijd
        {
            get => _RatelTikker.NaloopTijd;
            set
            {
                _RatelTikker.NaloopTijd = value;
                OnPropertyChanged(nameof(NaloopTijd), broadcast: true);
            }
        }

        public string FaseCyclus
        {
            get => _RatelTikker.FaseCyclus;
            set
            {
                _RatelTikker.FaseCyclus = value;
                OnPropertyChanged(nameof(FaseCyclus), broadcast: true);
            }
        }

        public int DimmingNiveauPeriodeDimmen
        {
            get => _RatelTikker.DimmingNiveauPeriodeDimmen;
            set
            {
                _RatelTikker.DimmingNiveauPeriodeDimmen = value;
                if (_RatelTikker.DimmingNiveauPeriodeDimmen < 0) _RatelTikker.DimmingNiveauPeriodeDimmen = 0;
                if (_RatelTikker.DimmingNiveauPeriodeDimmen > 10) _RatelTikker.DimmingNiveauPeriodeDimmen = 10;
                OnPropertyChanged(nameof(DimmingNiveauPeriodeDimmen), broadcast: true);
            }
        }
        
        public int DimmingNiveauPeriodeNietDimmen
        {
            get => _RatelTikker.DimmingNiveauPeriodeNietDimmen;
            set
            {
                _RatelTikker.DimmingNiveauPeriodeNietDimmen = value;
                if (_RatelTikker.DimmingNiveauPeriodeNietDimmen < 0) _RatelTikker.DimmingNiveauPeriodeNietDimmen = 0;
                if (_RatelTikker.DimmingNiveauPeriodeNietDimmen > 10) _RatelTikker.DimmingNiveauPeriodeNietDimmen = 10;
                OnPropertyChanged(nameof(DimmingNiveauPeriodeNietDimmen), broadcast: true);
            }
        }

        public RatelTikkerDetectorViewModel SelectedDetector
        {
            get => DetectorManager.SelectedItem;
            set
            {
                DetectorManager.SelectedItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollectionAroundList<RatelTikkerDetectorViewModel, RatelTikkerDetectorModel> Detectoren
        {
            get;
            private set;
        }


        private ItemsManagerViewModel<RatelTikkerDetectorViewModel, string> _DetectorManager;
        public ItemsManagerViewModel<RatelTikkerDetectorViewModel, string> DetectorManager
        {
            get
            {
                return _DetectorManager ??= new ItemsManagerViewModel<RatelTikkerDetectorViewModel, string>(
                    Detectoren,
                    ControllerAccessProvider.Default.AllDetectors.Where(x =>
                        x.Type == DetectorTypeEnum.Knop ||
                        x.Type == DetectorTypeEnum.KnopBinnen ||
                        x.Type == DetectorTypeEnum.KnopBuiten).Select(x => x.Naam),
                    x => new RatelTikkerDetectorViewModel(new RatelTikkerDetectorModel{Detector = x}),
                    x => Detectoren.All(y => y.Detector != x),
                    null,
                    () => OnPropertyChanged(nameof(SelectedDetector), broadcast: true),
                    () => OnPropertyChanged(nameof(SelectedDetector), broadcast: true));
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

        #region IViewModelWithItem  

        public object GetItem()
        {
            return _RatelTikker;
        }

        #endregion // IViewModelWithItem

        #region TLCGen messaging

        private void OnDetectorenChanged(object sender, DetectorenChangedMessage msg)
        {
            _DetectorManager?.Refresh();
        }

        private void OnNameChanged(object sender, NameChangedMessage msg)
        {
            if (msg.ObjectType != TLCGenObjectTypeEnum.Detector) return;
            _DetectorManager?.Refresh();
        }

        #endregion // TLCGen messaging

        #region IComparable

        public int CompareTo(object obj)
        {
            if (!(obj is RatelTikkerViewModel other))
                throw new InvalidCastException();
            return TLCGenIntegrityChecker.CompareSignalGroups(this.FaseCyclus, other.FaseCyclus);
        }
        
        #endregion // IComparable

        #region Constructor

        public RatelTikkerViewModel(RatelTikkerModel rateltikker)
        {
            _RatelTikker = rateltikker;
            Detectoren = new ObservableCollectionAroundList<RatelTikkerDetectorViewModel, RatelTikkerDetectorModel>(_RatelTikker.Detectoren);
            WeakReferenceMessenger.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            WeakReferenceMessenger.Default.Register<NameChangedMessage>(this, OnNameChanged);
        }

        #endregion // Constructor
    }
}
