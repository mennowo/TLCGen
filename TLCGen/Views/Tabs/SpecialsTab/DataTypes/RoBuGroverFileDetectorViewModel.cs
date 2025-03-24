using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RoBuGroverFileDetectorViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        private RoBuGroverFileDetectorModel _FileDetector;

        #endregion // Fields

        #region Properties

        public string Detector
        {
            get => _FileDetector.Detector;
            set
            {
                _FileDetector.Detector = value;
                OnPropertyChanged(nameof(Detector), broadcast: true);
            }
        }
        public int FileTijd
        {
            get => _FileDetector.FileTijd;
            set
            {
                _FileDetector.FileTijd = value;
                OnPropertyChanged(nameof(FileTijd), broadcast: true);
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
            return _FileDetector;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public RoBuGroverFileDetectorViewModel(RoBuGroverFileDetectorModel filedetector)
        {
            _FileDetector = filedetector;
        }

        #endregion // Constructor

    }
}
