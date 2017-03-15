using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class RoBuGroverViewModel : ViewModelBase
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
                OnMonitoredPropertyChanged("RoBuGrover");
            }
        }

        [Description("Minimale cyclustijd")]
        public int MinimaleCyclustijd
        {
            get { return _RoBuGrover.MinimaleCyclustijd; }
            set
            {
                _RoBuGrover.MinimaleCyclustijd = value;
                OnMonitoredPropertyChanged("MinimaleCyclustijd");
            }
        }

        [Description("Maximale cyclustijd")]
        public int MaximaleCyclustijd
        {
            get { return _RoBuGrover.MaximaleCyclustijd; }
            set
            {
                _RoBuGrover.MaximaleCyclustijd = value;
                OnMonitoredPropertyChanged("MaximaleCyclustijd");
            }
        }

        [Description("Groentijd verschil")]
        public int GroentijdVerschil
        {
            get { return _RoBuGrover.GroentijdVerschil; }
            set
            {
                _RoBuGrover.GroentijdVerschil = value;
                OnMonitoredPropertyChanged("GroentijdVerschil");
            }
        }

        [Description("Methode RobuGrover")]
        public RoBuGroverMethodeEnum MethodeRoBuGrover
        {
            get { return _RoBuGrover.MethodeRoBuGrover; }
            set
            {
                _RoBuGrover.MethodeRoBuGrover = value;
                OnMonitoredPropertyChanged("MethodeRoBuGrover");
            }
        }

        [Description("Mate ophogen groentijd")]
        public int GroenOphoogFactor
        {
            get { return _RoBuGrover.GroenOphoogFactor; }
            set
            {
                _RoBuGrover.GroenOphoogFactor = value;
                OnMonitoredPropertyChanged("GroenOphoogFactor");
            }
        }

        [Description("Mate verlagen groentijd")]
        public int GroenVerlaagFactor
        {
            get { return _RoBuGrover.GroenVerlaagFactor; }
            set
            {
                _RoBuGrover.GroenVerlaagFactor = value;
                OnMonitoredPropertyChanged("GroenVerlaagFactor");
            }
        }

        [Description("Mate verlagen bij overslag")]
        public int GroenVerlaagFactorNietPrimair
        {
            get { return _RoBuGrover.GroenVerlaagFactorNietPrimair; }
            set
            {
                _RoBuGrover.GroenVerlaagFactorNietPrimair = value;
                OnMonitoredPropertyChanged("GroenVerlaagFactorNietPrimair");
            }
        }

        [Description("Ophogen tijdens groen")]
        public bool OphogenTijdensGroen
        {
            get { return _RoBuGrover.OphogenTijdensGroen; }
            set
            {
                _RoBuGrover.OphogenTijdensGroen = value;
                OnMonitoredPropertyChanged("OphogenTijdensGroen");
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
        }

        #endregion // Command Functionality

        #region Constructor

        public RoBuGroverViewModel(RoBuGroverModel robugrover)
        {
            _RoBuGrover = robugrover;
        }

        #endregion // Constructor
    }
}
