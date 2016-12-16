using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;
using TLCGen.ViewModels;

namespace TLCGen.GebruikersOpties
{
    public class GebruikersOptieViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private GebruikersOptieModel _GebruikersOptie;

        #endregion // Fields

        #region Properties

        public string Naam
        {
            get { return _GebruikersOptie.Naam; }
            set
            {
                _GebruikersOptie.Naam = value;
                OnMonitoredPropertyChanged("Naam");
            }
        }
        public string Define
        {
            get { return _GebruikersOptie.Define; }
            set
            {
                _GebruikersOptie.Define = value;
                OnMonitoredPropertyChanged("Define");
            }
        }
        public CCOLElementTypeEnum Type
        {
            get { return _GebruikersOptie.Type; }
            set
            {
                _GebruikersOptie.Type = value;
                OnMonitoredPropertyChanged("Type");
            }
        }
        public int? Instelling
        {
            get { return _GebruikersOptie.Instelling; }
            set
            {
                _GebruikersOptie.Instelling = value;
                OnMonitoredPropertyChanged("Instelling");
            }
        }
        public string Commentaar
        {
            get { return _GebruikersOptie.Commentaar; }
            set
            {
                _GebruikersOptie.Commentaar = value;
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
            return _GebruikersOptie;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public GebruikersOptieViewModel(GebruikersOptieModel gebruikersoptie)
        {
            _GebruikersOptie = gebruikersoptie;
        }

        #endregion // Constructor
    }
}
