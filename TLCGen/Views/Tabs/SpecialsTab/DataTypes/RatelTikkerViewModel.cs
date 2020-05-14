using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class RatelTikkerViewModel : ViewModelBase, IViewModelWithItem
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
                RaisePropertyChanged<object>(nameof(Type), broadcast: true);
            }
        }

        public int NaloopTijd
        {
            get => _RatelTikker.NaloopTijd;
            set
            {
                _RatelTikker.NaloopTijd = value;
                RaisePropertyChanged<object>(nameof(NaloopTijd), broadcast: true);
            }
        }

        public string FaseCyclus
        {
            get => _RatelTikker.FaseCyclus;
            set
            {
                _RatelTikker.FaseCyclus = value;
                RaisePropertyChanged<object>(nameof(FaseCyclus), broadcast: true);
            }
        }

        public RatelTikkerDetectorViewModel SelectedDetector
        {
            get => DetectorManager.SelectedItem;
            set
            {
                DetectorManager.SelectedItem = value;
                RaisePropertyChanged();
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
                return _DetectorManager ?? (_DetectorManager =
                           new ItemsManagerViewModel<RatelTikkerDetectorViewModel, string>(
                               Detectoren,
                               ControllerAccessProvider.Default.AllDetectors.Where(x =>
                                   x.Type == DetectorTypeEnum.Knop ||
                                   x.Type == DetectorTypeEnum.KnopBinnen ||
                                   x.Type == DetectorTypeEnum.KnopBuiten).Select(x => x.Naam),
                               x => new RatelTikkerDetectorViewModel(new RatelTikkerDetectorModel{Detector = x}),
                               x => Detectoren.All(y => y.Detector != x),
                               null,
                               () => RaisePropertyChanged<object>(nameof(SelectedDetector), broadcast: true),
                               () => RaisePropertyChanged<object>(nameof(SelectedDetector), broadcast: true)));
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

        private void OnDetectorenChanged(DetectorenChangedMessage msg)
        {
            _DetectorManager?.Refresh();
        }

        private void OnNameChanged(NameChangedMessage msg)
        {
            if (msg.ObjectType != TLCGenObjectTypeEnum.Detector) return;
            _DetectorManager?.Refresh();
        }

        #endregion // TLCGen messaging

        #region Constructor

        public RatelTikkerViewModel(RatelTikkerModel rateltikker)
        {
            _RatelTikker = rateltikker;
            Detectoren = new ObservableCollectionAroundList<RatelTikkerDetectorViewModel, RatelTikkerDetectorModel>(_RatelTikker.Detectoren);
            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
        }

        #endregion // Constructor
    }
}
