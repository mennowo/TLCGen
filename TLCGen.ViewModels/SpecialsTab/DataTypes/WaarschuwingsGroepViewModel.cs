using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            get { return _WaarschuwingsGroep.Naam; }
            set
            {
                _WaarschuwingsGroep.Naam = value;
                OnMonitoredPropertyChanged("Naam");
            }
        }

        public bool Lichten
        {
            get { return _WaarschuwingsGroep.Lichten; }
            set
            {
                _WaarschuwingsGroep.Lichten = value;
                OnMonitoredPropertyChanged("Lichten");
            }
        }

        public bool Bellen
        {
            get { return _WaarschuwingsGroep.Bellen; }
            set
            {
                _WaarschuwingsGroep.Bellen = value;
                OnMonitoredPropertyChanged("Bellen");
            }
        }

        public string FaseCyclusVoorAansturing
        {
            get { return _WaarschuwingsGroep.FaseCyclusVoorAansturing; }
            set
            {
                _WaarschuwingsGroep.FaseCyclusVoorAansturing = value;
                OnMonitoredPropertyChanged("FaseCyclusVoorAansturing");
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
