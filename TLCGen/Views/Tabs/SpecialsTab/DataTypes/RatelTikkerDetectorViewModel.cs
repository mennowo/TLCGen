using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RatelTikkerDetectorViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        private RatelTikkerDetectorModel _Detector;

        #endregion // Fields

        #region Properties

        public string Detector
        {
            get => _Detector.Detector;
            set
            {
                _Detector.Detector = value;
                OnPropertyChanged(nameof(Detector), broadcast: true);
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
            return _Detector;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public RatelTikkerDetectorViewModel(RatelTikkerDetectorModel rtdetector)
        {
            _Detector = rtdetector;
        }

        #endregion // Constructor
    }
}
