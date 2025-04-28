using CommunityToolkit.Mvvm.ComponentModel;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel for the statusbar of the application.
    /// The statusbar is held in a seperate View.
    /// </summary>
    public class TLCGenStatusBarViewModel : ObservableObject
    {
        #region Fields

        private string _statusText;
        private string _alertText;

        #endregion // Fields

        #region Properties

        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }

        public string AlertText
        {
            get => _alertText;
            set
            {
                _alertText = value;
                OnPropertyChanged();
            }
        }

        #endregion // Properties
    }
}
