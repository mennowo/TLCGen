using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class PrioIngreepMeerealiserendeFaseCyclusViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        private PrioIngreepMeerealiserendeFaseCyclusModel _FaseCyclus;

        #endregion // Fields

        #region Properties

        public PrioIngreepMeerealiserendeFaseCyclusModel FaseCyclus
        {
            get => _FaseCyclus;
            set
            {
                _FaseCyclus = value;
                OnPropertyChanged(nameof(FaseCyclus), broadcast: true);
            }
        }

        public string Fase
        {
            get => _FaseCyclus.FaseCyclus;
            set
            {
                _FaseCyclus.FaseCyclus = value;
                OnPropertyChanged(nameof(Fase), broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return _FaseCyclus;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public PrioIngreepMeerealiserendeFaseCyclusViewModel(PrioIngreepMeerealiserendeFaseCyclusModel fase)
        {
            _FaseCyclus = fase;
        }

        #endregion // Constructor
    }
}
