using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            get { return _RatelTikker.Type; }
            set
            {
                _RatelTikker.Type = value;
                RaisePropertyChanged<object>("Type", broadcast: true);
            }
        }

        public int NaloopTijd
        {
            get { return _RatelTikker.NaloopTijd; }
            set
            {
                _RatelTikker.NaloopTijd = value;
                RaisePropertyChanged<object>("NaloopTijd", broadcast: true);
            }
        }

        public string FaseCyclus
        {
            get { return _RatelTikker.FaseCyclus; }
            set
            {
                _RatelTikker.FaseCyclus = value;
                RaisePropertyChanged<object>("FaseCyclus", broadcast: true);
            }
        }

        public RatelTikkerDetectorViewModel SelectedDetector
        {
            get { return DetectorManager.SelectedItem; }
            set
            {
                DetectorManager.SelectedItem = value;
                RaisePropertyChanged("SelectedDetector");
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
                if (_DetectorManager == null)
                {
                    List<string> dets =
                        DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen
                            .SelectMany(x => x.Detectoren)
                            .Where(x => x.Type == DetectorTypeEnum.Knop ||
                                        x.Type == DetectorTypeEnum.KnopBinnen ||
                                        x.Type == DetectorTypeEnum.KnopBuiten)
                            .Select(x => x.Naam).
                            ToList();
                    _DetectorManager = new ItemsManagerViewModel<RatelTikkerDetectorViewModel, string>(
                        Detectoren as ObservableCollection<RatelTikkerDetectorViewModel>,
                        dets,
                        (x) => { var rtd = new RatelTikkerDetectorViewModel(new RatelTikkerDetectorModel { Detector = x }); return rtd; },
                        (x) => { return !Detectoren.Where(y => y.Detector == x).Any(); },
                        null,
                        () => { RaisePropertyChanged<object>("SelectedDetector", broadcast: true); },
                        () => { RaisePropertyChanged<object>("SelectedDetector", broadcast: true); }
                        );
                }
                return _DetectorManager;
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
            _DetectorManager = null;
        }

        private void OnNameChanged(NameChangedMessage msg)
        {
            _DetectorManager = null;
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
