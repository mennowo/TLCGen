using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class WaarschuwingsGroepViewModel : ViewModelBase, IViewModelWithItem
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
                RaisePropertyChanged<object>(nameof(Naam), broadcast: true);
            }
        }

        public bool Lichten
        {
            get => _WaarschuwingsGroep.Lichten;
            set
            {
                _WaarschuwingsGroep.Lichten = value;
                RaisePropertyChanged<object>(nameof(Lichten), broadcast: true);
            }
        }

        public bool Bellen
        {
            get => _WaarschuwingsGroep.Bellen;
            set
            {
                _WaarschuwingsGroep.Bellen = value;
                RaisePropertyChanged<object>(nameof(Bellen), broadcast: true);
                // cause a check, so rtbel will be hidden or shown in the bitmap tab
                MessengerInstance.Send(new Messaging.Messages.ModelManagerMessageBase());
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
                RaisePropertyChanged<object>(nameof(FaseCyclusVoorAansturing), broadcast: true);
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
