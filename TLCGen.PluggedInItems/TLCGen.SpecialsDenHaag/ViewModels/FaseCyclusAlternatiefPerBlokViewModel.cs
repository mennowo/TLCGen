using System;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;

namespace TLCGen.SpecialsDenHaag.Models
{
    public class FaseCyclusAlternatiefPerBlokViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        #region Fields

        private readonly FaseCyclusAlternatiefPerBlokModel _faseCyclus;
        
        #endregion // Fields

        #region Properties

        public string FaseCyclus
        {
            get { return _faseCyclus.FaseCyclus; }
            set { _faseCyclus.FaseCyclus = value; }
        }

        public int BitWiseBlokAlternatief
        {
            get { return _faseCyclus.BitWiseBlokAlternatief; }
            set
            {
                _faseCyclus.BitWiseBlokAlternatief = value; 
                RaisePropertyChanged<object>("BitWiseBlokAlternatief", true);
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
            var other = obj as FaseCyclusAlternatiefPerBlokViewModel;
            return other != null ? string.Compare(FaseCyclus, other.FaseCyclus, StringComparison.Ordinal) : 0;
        }
        
        #endregion // IComparable

        #region Constructor

        public FaseCyclusAlternatiefPerBlokViewModel(FaseCyclusAlternatiefPerBlokModel faseCyclus)
        {
            _faseCyclus = faseCyclus;
        }

        #endregion // Constructor
    }
}