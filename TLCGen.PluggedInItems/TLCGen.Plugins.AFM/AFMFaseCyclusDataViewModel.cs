using CommunityToolkit.Mvvm.ComponentModel;
using System;
using TLCGen.Helpers;
using TLCGen.Plugins.AFM.Models;

namespace TLCGen.Plugins.AFM
{
    public class AFMFaseCyclusDataViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        private AFMFaseCyclusDataModel _faseCyclus;

        public string FaseCyclus
        {

            get => _faseCyclus.FaseCyclus;
            set
            {
                _faseCyclus.FaseCyclus = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string DummyFaseCyclus
        {

            get => _faseCyclus.DummyFaseCyclus;
            set
            {
                _faseCyclus.DummyFaseCyclus = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int MinimaleGroentijd
        {
            get => _faseCyclus.MinimaleGroentijd;
            set
            {
                _faseCyclus.MinimaleGroentijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int MaximaleGroentijd
        {
            get => _faseCyclus.MaximaleGroentijd;
            set
            {
                _faseCyclus.MaximaleGroentijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public object GetItem()
        {
            return _faseCyclus;
        }

        public int CompareTo(object obj)
        {
            return string.CompareOrdinal(FaseCyclus, ((AFMFaseCyclusDataViewModel)obj).FaseCyclus);
        }

        public AFMFaseCyclusDataViewModel(AFMFaseCyclusDataModel faseCyclus)
        {
            _faseCyclus = faseCyclus;
        }
    }
}
