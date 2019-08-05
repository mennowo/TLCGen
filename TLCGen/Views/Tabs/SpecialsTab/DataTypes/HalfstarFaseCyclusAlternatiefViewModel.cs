using GalaSoft.MvvmLight;
using System;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class HalfstarFaseCyclusAlternatiefViewModel : ViewModelBase, IViewModelWithItem, IComparable<HalfstarFaseCyclusAlternatiefViewModel>, IComparable
    {
        #region Properties

        public HalfstarFaseCyclusAlternatiefModel Model { get; set; }

        public string FaseCyclus => Model.FaseCyclus;

        public bool AlternatiefToestaan
        {
            get => Model.AlternatiefToestaan;
            set
            {
                Model.AlternatiefToestaan = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int AlternatieveRuimte
        {
            get => Model.AlternatieveRuimte;
            set
            {
                Model.AlternatieveRuimte = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
            return FaseCyclus.CompareTo(((HalfstarFaseCyclusAlternatiefViewModel)obj).FaseCyclus);
        }

        #endregion // IComparable

        #region IComparable<HalfstarFaseCyclusAlternatiefViewModel>

        public int CompareTo(HalfstarFaseCyclusAlternatiefViewModel other)
        {
            return FaseCyclus.CompareTo(other.FaseCyclus);
        }

        #endregion // IComparable<HalfstarFaseCyclusAlternatiefViewModel>

        #region IViewModelWithItem

        public object GetItem()
        {
            return Model;
        }
        #endregion // IViewModelWithItem

        #region Constructor

        public HalfstarFaseCyclusAlternatiefViewModel(HalfstarFaseCyclusAlternatiefModel model)
        {
            Model = model;
        }
        
        #endregion // Constructor
    }
}
