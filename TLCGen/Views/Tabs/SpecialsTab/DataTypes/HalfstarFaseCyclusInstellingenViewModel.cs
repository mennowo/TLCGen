using CommunityToolkit.Mvvm.ComponentModel;
using System;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class HalfstarFaseCyclusInstellingenViewModel : ObservableObjectEx, IViewModelWithItem, IComparable<HalfstarFaseCyclusInstellingenViewModel>, IComparable
    {
        #region Properties

        public HalfstarFaseCyclusInstellingenModel Model { get; set; }

        public string FaseCyclus => Model.FaseCyclus;

        public bool AlternatiefToestaan
        {
            get => Model.AlternatiefToestaan;
            set
            {
                Model.AlternatiefToestaan = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int AlternatieveRuimte
        {
            get => Model.AlternatieveRuimte;
            set
            {
                Model.AlternatieveRuimte = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool AanvraagOpTxB
        {
            get => Model.AanvraagOpTxB;
            set
            {
                Model.AanvraagOpTxB = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool PrivilegePeriodeOpzetten
        {
            get => Model.PrivilegePeriodeOpzetten;
            set
            {
                Model.PrivilegePeriodeOpzetten = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
            return FaseCyclus.CompareTo(((HalfstarFaseCyclusInstellingenViewModel)obj).FaseCyclus);
        }

        #endregion // IComparable

        #region IComparable<HalfstarFaseCyclusAlternatiefViewModel>

        public int CompareTo(HalfstarFaseCyclusInstellingenViewModel other)
        {
            return FaseCyclus.CompareTo(other.FaseCyclus);
        }

        #endregion // IComparable<HalfstarFaseCyclusInstellingenViewModel>

        #region IViewModelWithItem

        public object GetItem()
        {
            return Model;
        }
        #endregion // IViewModelWithItem

        #region Constructor

        public HalfstarFaseCyclusInstellingenViewModel(HalfstarFaseCyclusInstellingenModel model)
        {
            Model = model;
        }
        
        #endregion // Constructor
    }
}
