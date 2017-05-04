using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class VAOntruimenNaarFaseViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private VAOntruimenNaarFaseModel _VAOntruimenNaarFase;
        private int _OntruimingsTijd;

        #endregion // Fields

        #region Properties

        public string FaseCyclus
        {
            get { return _VAOntruimenNaarFase.FaseCyclus; }
            set
            {
                _VAOntruimenNaarFase.FaseCyclus = value;
                RaisePropertyChanged<VAOntruimenNaarFaseViewModel>("FaseCyclus", broadcast: true);
            }
        }

        public int VAOntruimingsTijd
        {
            get { return _VAOntruimenNaarFase.VAOntruimingsTijd; }
            set
            {
                if (value >= 0)
                {
                    _VAOntruimenNaarFase.VAOntruimingsTijd = value;
                }
                RaisePropertyChanged<VAOntruimenNaarFaseViewModel>("VAOntruimingsTijd", broadcast: true);
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
            return _VAOntruimenNaarFase;
        }

        #endregion // IViewModelWithItem
        
        #region Constructor

        public VAOntruimenNaarFaseViewModel(VAOntruimenNaarFaseModel vaontruimennaarfase)
        {
            _VAOntruimenNaarFase = vaontruimennaarfase;
        }

        #endregion // Constructor
    }
}
