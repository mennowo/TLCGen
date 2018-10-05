using GalaSoft.MvvmLight;
using System;
using System.Linq;
using TLCGen.Helpers;
using TLCGen.Plugins.Timings.Models;

namespace TLCGen.Plugins.Timings
{
    public class TimingsFaseCyclusDataViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        private TimingsFaseCyclusDataModel _faseCyclus;

        public string FaseCyclus
        {

            get => _faseCyclus.FaseCyclus;
            set
            {
                _faseCyclus.FaseCyclus = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public TimingsFaseCyclusTypeEnum ConflictType
        {
            get => _faseCyclus.ConflictType;
            set
            {
                _faseCyclus.ConflictType = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public object GetItem()
        {
            return _faseCyclus;
        }

        public int CompareTo(object obj)
        {
            return string.CompareOrdinal(FaseCyclus, ((TimingsFaseCyclusDataViewModel)obj).FaseCyclus);
        }

        public TimingsFaseCyclusDataViewModel(TimingsFaseCyclusDataModel faseCyclus)
        {
            _faseCyclus = faseCyclus;
        }
    }
}
