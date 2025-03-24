using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RoBuGroverHiaatDetectorViewModel : ObservableObjectEx, IViewModelWithItem
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
                OnPropertyChanged(nameof(Detector), broadcast: true);
            }
        }
        public int HiaatTijd
        {
            get => _HiaatDetector.HiaatTijd;
            set
            {
                _HiaatDetector.HiaatTijd = value;
                OnPropertyChanged(nameof(HiaatTijd), broadcast: true);
            }
        }

        #endregion Properties

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
