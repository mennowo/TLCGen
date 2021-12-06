using GalaSoft.MvvmLight;
using System;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class TimingsFaseCyclusDataViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        #region Fields

        private readonly TimingsFaseCyclusDataModel _faseCyclus;

        #endregion // Fields

        #region Properties

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

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return _faseCyclus;
        }

        #endregion // IViewModelWithItem

        #region IComparable

        public int CompareTo(object obj)
        {
            return string.CompareOrdinal(FaseCyclus, ((TimingsFaseCyclusDataViewModel)obj).FaseCyclus);
        }

        #endregion // IComparable

        #region Constructor

        public TimingsFaseCyclusDataViewModel(TimingsFaseCyclusDataModel faseCyclus)
        {
            _faseCyclus = faseCyclus;
        }

        #endregion // Constructor
    }
}
