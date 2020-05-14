using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class VAOntruimenNaarFaseViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private VAOntruimenNaarFaseModel _VAOntruimenNaarFase;
        
        #endregion // Fields

        #region Properties

        public string FaseCyclus
        {
            get => _VAOntruimenNaarFase.FaseCyclus;
            set
            {
                _VAOntruimenNaarFase.FaseCyclus = value;
                RaisePropertyChanged<object>(nameof(FaseCyclus), broadcast: true);
            }
        }

        public int VAOntruimingsTijd
        {
            get => _VAOntruimenNaarFase.VAOntruimingsTijd;
            set
            {
                if (value >= 0)
                {
                    _VAOntruimenNaarFase.VAOntruimingsTijd = value;
                }
                RaisePropertyChanged<object>(nameof(VAOntruimingsTijd), broadcast: true);
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
