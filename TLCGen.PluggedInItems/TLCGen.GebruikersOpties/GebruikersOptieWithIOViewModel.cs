using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;
using TLCGen.ViewModels;

namespace TLCGen.GebruikersOpties
{
    public class GebruikersOptieWithIOViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private GebruikersOptieWithIOModel _GebruikersOptieWithOI;

        #endregion // Fields

        #region Properties


        public string Naam
        {
            get { return _GebruikersOptieWithOI.Naam; }
            set
            {
                _GebruikersOptieWithOI.Naam = value;
                OnMonitoredPropertyChanged("Naam");
            }
        }
        public string Define
        {
            get { return _GebruikersOptieWithOI.Define; }
            set
            {
                _GebruikersOptieWithOI.Define = value;
                OnMonitoredPropertyChanged("Define");
            }
        }
        public string Commentaar
        {
            get { return _GebruikersOptieWithOI.Commentaar; }
            set
            {
                _GebruikersOptieWithOI.Commentaar = value;
                OnMonitoredPropertyChanged("Commentaar");
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
            return _GebruikersOptieWithOI;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public GebruikersOptieWithIOViewModel(GebruikersOptieWithIOModel gebruikersoptiewithoi)
        {
            _GebruikersOptieWithOI = gebruikersoptiewithoi;
        }

        #endregion // Constructor
    }
}
