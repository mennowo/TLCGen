using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ModuleFaseCyclusAlternatiefViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public ModuleFaseCyclusAlternatiefModel Alternatief { get; }

        public string FaseCyclus => Alternatief.FaseCyclus;

        public int AlternatieveGroenTijd
        {
            get => Alternatief.AlternatieveGroenTijd;
            set
            {
                Alternatief.AlternatieveGroenTijd = value; 
                OnPropertyChanged(nameof(AlternatieveGroenTijd), true);
                foreach (var o in Others)
                {
                    o.AlternatieveGroenTijd = value;
                }
            }
        }

        public List<ModuleFaseCyclusAlternatiefModel> Others { get; set; }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return Alternatief;
        }

        #endregion // IViewModelWithItem

        #region Overrides

        public override string ToString()
        {
            return FaseCyclus;
        }

        #endregion // Overrides

        #region IComparable

        public int CompareTo(object obj)
        {
	        if (!(obj is ModuleFaseCyclusAlternatiefViewModel fcvm))
                throw new InvalidCastException();
	        var myName = FaseCyclus;
	        var hisName = fcvm.FaseCyclus;
	        if (myName.Length < hisName.Length) myName = myName.PadLeft(hisName.Length, '0');
	        else if (hisName.Length < myName.Length) hisName = hisName.PadLeft(myName.Length, '0');
	        return string.Compare(myName, hisName, StringComparison.Ordinal);
        }

        #endregion // IComparable


        #region Constructor

        public ModuleFaseCyclusAlternatiefViewModel(ModuleFaseCyclusAlternatiefModel model)
        {
            Alternatief = model;
        }

        #endregion // Constructor
    }
}