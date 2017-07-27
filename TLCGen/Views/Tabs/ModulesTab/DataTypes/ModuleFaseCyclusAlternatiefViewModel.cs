using System;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ModuleFaseCyclusAlternatiefViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public ModuleFaseCyclusAlternatiefModel Alternatief { get; }

        public string FaseCyclus
        {
            get { return Alternatief.FaseCyclus; }
        }

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
            var fcvm = obj as ModuleFaseCyclusAlternatiefViewModel;
            if (fcvm == null)
                throw new NotImplementedException();
            else
            {
                var myName = FaseCyclus;
                var hisName = fcvm.FaseCyclus;
                if (myName.Length < hisName.Length) myName = myName.PadLeft(hisName.Length, '0');
                else if (hisName.Length < myName.Length) hisName = hisName.PadLeft(myName.Length, '0');
                return string.Compare(myName, hisName, StringComparison.Ordinal);
            }
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