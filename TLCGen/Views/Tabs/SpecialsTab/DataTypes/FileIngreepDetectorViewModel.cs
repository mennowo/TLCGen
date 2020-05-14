using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class FileIngreepDetectorViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private FileIngreepDetectorModel _Detector;

        #endregion // Fields

        #region Properties

        public string Detector
        {
            get => _Detector.Detector;
            set
            {
                _Detector.Detector = value;
                RaisePropertyChanged("Detector");
            }
        }

        public int BezetTijd
        {
            get => _Detector.BezetTijd;
            set
            {
                _Detector.BezetTijd = value;
                RaisePropertyChanged<object>(nameof(BezetTijd), broadcast: true);
            }
        }

        public int RijTijd
        {
            get => _Detector.RijTijd;
            set
            {
                _Detector.RijTijd = value;
                RaisePropertyChanged<object>(nameof(RijTijd), broadcast: true);
            }
        }

        public int AfvalVertraging
        {
            get => _Detector.AfvalVertraging;
            set
            {
                _Detector.AfvalVertraging = value;
                RaisePropertyChanged<object>(nameof(AfvalVertraging), broadcast: true);
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return _Detector;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public FileIngreepDetectorViewModel(FileIngreepDetectorModel detector)
        {
            _Detector = detector;
        }

        #endregion // Constructor
    }
}
