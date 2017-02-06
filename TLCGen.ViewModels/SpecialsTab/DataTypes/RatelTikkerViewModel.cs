using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class RatelTikkerViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private RatelTikkerModel _RatelTikker;

        private RatelTikkerDetectorViewModel _SelectedDetector;

        #endregion // Fields

        #region Properties

        public RateltikkerTypeEnum Type
        {
            get { return _RatelTikker.Type; }
            set
            {
                _RatelTikker.Type = value;
                OnMonitoredPropertyChanged("Type");
            }
        }

        public int NaloopTijd
        {
            get { return _RatelTikker.NaloopTijd; }
            set
            {
                _RatelTikker.NaloopTijd = value;
                OnMonitoredPropertyChanged("NaloopTijd");
            }
        }

        public string FaseCyclus
        {
            get { return _RatelTikker.FaseCyclus; }
            set
            {
                _RatelTikker.FaseCyclus = value;
                OnMonitoredPropertyChanged("FaseCyclus");
            }
        }

        public RatelTikkerDetectorViewModel SelectedDetector
        {
            get { return _SelectedDetector; }
            set
            {
                _SelectedDetector = value;
                OnPropertyChanged("SelectedDetector");
            }
        }

        public ObservableCollectionAroundList<RatelTikkerDetectorViewModel, RatelTikkerDetectorModel> Detectoren
        {
            get;
            private set;
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
