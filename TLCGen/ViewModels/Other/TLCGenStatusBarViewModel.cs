using GalaSoft.MvvmLight;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel for the statusbar of the application.
    /// The statusbar is held in a seperate View.
    /// </summary>
    public class TLCGenStatusBarViewModel : ViewModelBase
    {
        #region Fields

        private string _statusText;
        private string _alertText;

        #endregion // Fields

        #region Properties

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                RaisePropertyChanged();
            }
        }

        public string AlertText
        {
            get { return _alertText; }
            set
            {
                _alertText = value;
                RaisePropertyChanged();
            }
        }

        #endregion // Properties
    }
}
