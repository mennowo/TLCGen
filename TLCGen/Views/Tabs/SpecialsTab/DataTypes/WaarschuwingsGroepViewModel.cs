using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class WaarschuwingsGroepViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        private WaarschuwingsGroepModel _WaarschuwingsGroep;

        #endregion // Fields

        #region Properties

        public string Naam
        {
            get => _WaarschuwingsGroep.Naam;
            set
            {
                _WaarschuwingsGroep.Naam = value;
                OnPropertyChanged(nameof(Naam), broadcast: true);
            }
        }

        public bool Lichten
        {
            get => _WaarschuwingsGroep.Lichten;
            set
            {
                _WaarschuwingsGroep.Lichten = value;
                OnPropertyChanged(nameof(Lichten), broadcast: true);
            }
        }

        public bool Bellen
        {
            get => _WaarschuwingsGroep.Bellen;
            set
            {
                _WaarschuwingsGroep.Bellen = value;
                OnPropertyChanged(nameof(Bellen), broadcast: true);
                // cause a check, so rtbel will be hidden or shown in the bitmap tab
                WeakReferenceMessenger.Default.Send(new Messaging.Messages.ModelManagerMessageBase());
            }
        }

        public string FaseCyclusVoorAansturing
        {
            get => _WaarschuwingsGroep.FaseCyclusVoorAansturing;
            set
            {
                if(value != null)
                {
                    _WaarschuwingsGroep.FaseCyclusVoorAansturing = value;
                }
                OnPropertyChanged(nameof(FaseCyclusVoorAansturing), broadcast: true);
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
            return _WaarschuwingsGroep;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public WaarschuwingsGroepViewModel(WaarschuwingsGroepModel waarschuwingsgroep)
        {
            _WaarschuwingsGroep = waarschuwingsgroep;

        }

        #endregion // Constructor

    }
}
