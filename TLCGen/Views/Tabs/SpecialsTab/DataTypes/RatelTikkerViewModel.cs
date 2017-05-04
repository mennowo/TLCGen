using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
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
                RaisePropertyChanged<RatelTikkerViewModel>("Type", broadcast: true);
            }
        }

        public int NaloopTijd
        {
            get { return _RatelTikker.NaloopTijd; }
            set
            {
                _RatelTikker.NaloopTijd = value;
                RaisePropertyChanged<RatelTikkerViewModel>("NaloopTijd", broadcast: true);
            }
        }

        public string FaseCyclus
        {
            get { return _RatelTikker.FaseCyclus; }
            set
            {
                _RatelTikker.FaseCyclus = value;
                RaisePropertyChanged<RatelTikkerViewModel>("FaseCyclus", broadcast: true);
            }
        }

        public RatelTikkerDetectorViewModel SelectedDetector
        {
            get { return DetectorManager.SelectedDetector; }
            set
            {
                DetectorManager.SelectedDetector = value;
                RaisePropertyChanged("SelectedDetector");
            }
        }

        public ObservableCollectionAroundList<RatelTikkerDetectorViewModel, RatelTikkerDetectorModel> Detectoren
        {
            get;
            private set;
        }


        private DetectorManagerViewModel<RatelTikkerDetectorViewModel, string> _DetectorManager;
        public DetectorManagerViewModel<RatelTikkerDetectorViewModel, string> DetectorManager
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
                    _DetectorManager = new DetectorManagerViewModel<RatelTikkerDetectorViewModel, string>(
                        Detectoren as ObservableCollection<RatelTikkerDetectorViewModel>,
                        dets,
                        (x) => { var rtd = new RatelTikkerDetectorViewModel(new RatelTikkerDetectorModel { Detector = x }); return rtd; },
                        (x) => { return !Detectoren.Where(y => y.Detector == x).Any(); },
                        null,
                        () => { RaisePropertyChanged<RatelTikkerViewModel>("SelectedDetector", broadcast: true); },
                        () => { RaisePropertyChanged<RatelTikkerViewModel>("SelectedDetector", broadcast: true); }
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

        #region Constructor

        public RatelTikkerViewModel(RatelTikkerModel rateltikker)
        {
            _RatelTikker = rateltikker;
            Detectoren = new ObservableCollectionAroundList<RatelTikkerDetectorViewModel, RatelTikkerDetectorModel>(_RatelTikker.Detectoren);
        }

        #endregion // Constructor

    }
}
