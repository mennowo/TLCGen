using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class PrioIngreepMeerealiserendeIngreepViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        private PrioIngreepMeerealiserendeIngreepModel _Ingreep;

        #endregion // Fields

        #region Properties

        public PrioIngreepMeerealiserendeIngreepModel Ingreep
        {
            get => _Ingreep;
            set
            {
                _Ingreep = value;
                OnPropertyChanged(nameof(_Ingreep), broadcast: true);
            }
        }

        public string PrioIngreep
        {
            get => _Ingreep.PrioIngreep;
            set
            {
                _Ingreep.PrioIngreep = value;
                OnPropertyChanged(nameof(PrioIngreep), broadcast: true);
            }
        }

        public string FaseCyclus
        {
            get => _Ingreep.FaseCyclus;
            set
            {
                _Ingreep.FaseCyclus = value;
                OnPropertyChanged(nameof(FaseCyclus), broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return _Ingreep;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public PrioIngreepMeerealiserendeIngreepViewModel(PrioIngreepMeerealiserendeIngreepModel fase)
        {
            _Ingreep = fase;
        }

        #endregion // Constructor
    }
}
