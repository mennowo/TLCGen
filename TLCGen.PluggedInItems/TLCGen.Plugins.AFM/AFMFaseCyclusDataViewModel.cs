using GalaSoft.MvvmLight;
using System;
using TLCGen.Helpers;
using TLCGen.Plugins.AFM.Models;

namespace TLCGen.Plugins.AFM
{
    public class AFMFaseCyclusDataViewModel : ViewModelBase, IViewModelWithItem
    {
        private AFMFaseCyclusDataModel _faseCyclus;

        public string FaseCyclus
        {

            get => _faseCyclus.FaseCyclus;
            set
            {
                _faseCyclus.FaseCyclus = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string DummyFaseCyclus
        {

            get => _faseCyclus.DummyFaseCyclus;
            set
            {
                _faseCyclus.DummyFaseCyclus = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int MinimaleGroentijd
        {
            get => _faseCyclus.MinimaleGroentijd;
            set
            {
                _faseCyclus.MinimaleGroentijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int MaximaleGroentijd
        {
            get => _faseCyclus.MaximaleGroentijd;
            set
            {
                _faseCyclus.MaximaleGroentijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public object GetItem()
        {
            return _faseCyclus;
        }

        public AFMFaseCyclusDataViewModel(AFMFaseCyclusDataModel faseCyclus)
        {
            _faseCyclus = faseCyclus;
        }
    }
}
