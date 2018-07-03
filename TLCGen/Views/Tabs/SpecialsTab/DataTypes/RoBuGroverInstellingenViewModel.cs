using GalaSoft.MvvmLight.CommandWpf;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class RoBuGroverInstellingenViewModel : ViewModelBase
    {
        #region Fields

        private RoBuGroverModel _RoBuGrover;

        #endregion // Fields

        #region Properties

        [Browsable(false)]
        public bool RoBuGrover
        {
            get { return _RoBuGrover.RoBuGrover; }
            set
            {
                _RoBuGrover.RoBuGrover = value;
                RaisePropertyChanged<object>("RoBuGrover", broadcast: true);
            }
        }

        [Description("Minimale cyclustijd")]
        public int MinimaleCyclustijd
        {
            get { return _RoBuGrover.MinimaleCyclustijd; }
            set
            {
                _RoBuGrover.MinimaleCyclustijd = value;
                RaisePropertyChanged<object>("MinimaleCyclustijd", broadcast: true);
            }
        }

        [Description("Maximale cyclustijd")]
        public int MaximaleCyclustijd
        {
            get { return _RoBuGrover.MaximaleCyclustijd; }
            set
            {
                _RoBuGrover.MaximaleCyclustijd = value;
                RaisePropertyChanged<object>("MaximaleCyclustijd", broadcast: true);
            }
        }

        [Description("Groentijd verschil")]
        public int GroentijdVerschil
        {
            get { return _RoBuGrover.GroentijdVerschil; }
            set
            {
                _RoBuGrover.GroentijdVerschil = value;
                RaisePropertyChanged<object>("GroentijdVerschil", broadcast: true);
            }
        }

        [Description("Methode RobuGrover")]
        public RoBuGroverMethodeEnum MethodeRoBuGrover
        {
            get { return _RoBuGrover.MethodeRoBuGrover; }
            set
            {
                _RoBuGrover.MethodeRoBuGrover = value;
                RaisePropertyChanged<object>("MethodeRoBuGrover", broadcast: true);
            }
        }

        [Description("Mate ophogen groentijd")]
        public int GroenOphoogFactor
        {
            get { return _RoBuGrover.GroenOphoogFactor; }
            set
            {
                _RoBuGrover.GroenOphoogFactor = value;
                RaisePropertyChanged<object>("GroenOphoogFactor", broadcast: true);
            }
        }

        [Description("Mate verlagen groentijd")]
        public int GroenVerlaagFactor
        {
            get { return _RoBuGrover.GroenVerlaagFactor; }
            set
            {
                _RoBuGrover.GroenVerlaagFactor = value;
                RaisePropertyChanged<object>("GroenVerlaagFactor", broadcast: true);
            }
        }

        [Description("Mate verlagen bij overslag")]
        public int GroenVerlaagFactorNietPrimair
        {
            get { return _RoBuGrover.GroenVerlaagFactorNietPrimair; }
            set
            {
                _RoBuGrover.GroenVerlaagFactorNietPrimair = value;
                RaisePropertyChanged<object>("GroenVerlaagFactorNietPrimair", broadcast: true);
            }
        }

        [Description("Ophogen tijdens groen")]
        public bool OphogenTijdensGroen
        {
            get { return _RoBuGrover.OphogenTijdensGroen; }
            set
            {
                _RoBuGrover.OphogenTijdensGroen = value;
                RaisePropertyChanged<object>("OphogenTijdensGroen", broadcast: true);
            }
        }

        [Description("RoBuGrover venster in testomgeving")]
        public bool RoBuGroverVenster
        {
            get { return _RoBuGrover.RoBuGroverVenster; }
            set
            {
                _RoBuGrover.RoBuGroverVenster = value;
                RaisePropertyChanged<object>("RoBuGroverVenster", broadcast: true);
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
            RaisePropertyChanged("");
            RaisePropertyChanged<object>(nameof(RoBuGrover), broadcast: true);
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
