using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RoBuGroverHiaatDetectorViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private RoBuGroverHiaatDetectorModel _HiaatDetector;

        #endregion // Fields

        #region Properties

        public string Detector
        {
            get => _HiaatDetector.Detector;
            set
            {
                _HiaatDetector.Detector = value;
                RaisePropertyChanged<object>(nameof(Detector), broadcast: true);
            }
        }
        public int HiaatTijd
        {
            get => _HiaatDetector.HiaatTijd;
            set
            {
                _HiaatDetector.HiaatTijd = value;
                RaisePropertyChanged<object>(nameof(HiaatTijd), broadcast: true);
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
            return _HiaatDetector;
        }

        #endregion // IViewModelwithItem

        #region Constructor

        public RoBuGroverHiaatDetectorViewModel(RoBuGroverHiaatDetectorModel hiaatdetector)
        {
            _HiaatDetector = hiaatdetector;
        }

        #endregion // Constructor

    }
}
