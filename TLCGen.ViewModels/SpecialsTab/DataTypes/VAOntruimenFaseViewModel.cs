using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class VAOntruimenFaseViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private VAOntruimenFaseModel _VAOntruimenFase;

        #endregion // Fields

        #region Properties

        public string FaseCyclus
        {
            get { return _VAOntruimenFase.FaseCyclus; }
            set
            {
                _VAOntruimenFase.FaseCyclus = value;
                OnMonitoredPropertyChanged("FaseCyclus");
            }
        }
        public int MaximaleVAOntruimingsTijd
        {
            get { return _VAOntruimenFase.MaximaleVAOntruimingsTijd; }
            set
            {
                _VAOntruimenFase.MaximaleVAOntruimingsTijd = value;
                OnMonitoredPropertyChanged("MaximaleVAOntruimingsTijd");
            }
        }

        public ObservableCollectionAroundList<VAOntruimenNaarFaseViewModel, VAOntruimenNaarFaseModel> ConflicterendeFasen
        {
            get;
            private set;
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
            return _VAOntruimenFase;
        }
        
        #endregion // IViewModelWithItem

        #region Constructor

        public VAOntruimenFaseViewModel(VAOntruimenFaseModel vaontruimenfase)
        {
            _VAOntruimenFase = vaontruimenfase;

            ConflicterendeFasen = new ObservableCollectionAroundList<VAOntruimenNaarFaseViewModel, VAOntruimenNaarFaseModel>(_VAOntruimenFase.ConflicterendeFasen);
        }

        #endregion // Constructor
    }
}
