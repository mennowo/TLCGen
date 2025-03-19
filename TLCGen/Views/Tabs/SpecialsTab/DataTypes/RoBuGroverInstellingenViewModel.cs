using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class RoBuGroverInstellingenViewModel : ObservableObjectEx
    {
        #region Fields

        private RoBuGroverModel _RoBuGrover;

        #endregion // Fields

        #region Properties

        [Browsable(false)]
        public bool RoBuGrover
        {
            get => _RoBuGrover.RoBuGrover;
            set
            {
                _RoBuGrover.RoBuGrover = value;
                OnPropertyChanged(nameof(RoBuGrover), broadcast: true);
            }
        }

        [Description("Minimale cyclustijd")]
        public int MinimaleCyclustijd
        {
            get => _RoBuGrover.MinimaleCyclustijd;
            set
            {
                _RoBuGrover.MinimaleCyclustijd = value;
                OnPropertyChanged(nameof(MinimaleCyclustijd), broadcast: true);
            }
        }

        [Description("Maximale cyclustijd")]
        public int MaximaleCyclustijd
        {
            get => _RoBuGrover.MaximaleCyclustijd;
            set
            {
                _RoBuGrover.MaximaleCyclustijd = value;
                OnPropertyChanged(nameof(MaximaleCyclustijd), broadcast: true);
            }
        }

        [Description("Groentijd verschil")]
        public int GroentijdVerschil
        {
            get => _RoBuGrover.GroentijdVerschil;
            set
            {
                _RoBuGrover.GroentijdVerschil = value;
                OnPropertyChanged(nameof(GroentijdVerschil), broadcast: true);
            }
        }

        [Description("Methode RobuGrover")]
        public RoBuGroverMethodeEnum MethodeRoBuGrover
        {
            get => _RoBuGrover.MethodeRoBuGrover;
            set
            {
                _RoBuGrover.MethodeRoBuGrover = value;
                OnPropertyChanged(nameof(MethodeRoBuGrover), broadcast: true);
            }
        }

        [Description("Mate ophogen groentijd")]
        public int GroenOphoogFactor
        {
            get => _RoBuGrover.GroenOphoogFactor;
            set
            {
                _RoBuGrover.GroenOphoogFactor = value;
                OnPropertyChanged(nameof(GroenOphoogFactor), broadcast: true);
            }
        }

        [Description("Mate verlagen groentijd")]
        public int GroenVerlaagFactor
        {
            get => _RoBuGrover.GroenVerlaagFactor;
            set
            {
                _RoBuGrover.GroenVerlaagFactor = value;
                OnPropertyChanged(nameof(GroenVerlaagFactor), broadcast: true);
            }
        }

        [Description("Mate verlagen bij overslag")]
        public int GroenVerlaagFactorNietPrimair
        {
            get => _RoBuGrover.GroenVerlaagFactorNietPrimair;
            set
            {
                _RoBuGrover.GroenVerlaagFactorNietPrimair = value;
                OnPropertyChanged(nameof(GroenVerlaagFactorNietPrimair), broadcast: true);
            }
        }

        [Description("Ophogen tijdens groen")]
        public bool OphogenTijdensGroen
        {
            get => _RoBuGrover.OphogenTijdensGroen;
            set
            {
                _RoBuGrover.OphogenTijdensGroen = value;
                OnPropertyChanged(nameof(OphogenTijdensGroen), broadcast: true);
            }
        }

        [Description("RoBuGrover venster in testomgeving")]
        public bool RoBuGroverVenster
        {
            get => _RoBuGrover.RoBuGroverVenster;
            set
            {
                _RoBuGrover.RoBuGroverVenster = value;
                OnPropertyChanged(nameof(RoBuGroverVenster), broadcast: true);
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _SetRoBuGroverDefaultsCommand;
        public ICommand SetRoBuGroverDefaultsCommand
        {
            get
            {
                if (_SetRoBuGroverDefaultsCommand == null)
                {
                    _SetRoBuGroverDefaultsCommand = new RelayCommand(SetRoBuGroverDefaultsCommand_Executed);
                }
                return _SetRoBuGroverDefaultsCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        public void SetRoBuGroverDefaultsCommand_Executed()
        {
            DefaultsProvider.Default.SetDefaultsOnModel(_RoBuGrover);
            OnPropertyChanged("");
            OnPropertyChanged(nameof(RoBuGrover), broadcast: true);
        }

        #endregion // Command Functionality

        #region Constructor

        public RoBuGroverInstellingenViewModel(RoBuGroverModel robugrover)
        {
            _RoBuGrover = robugrover;
        }

        #endregion // Constructor
    }
}
