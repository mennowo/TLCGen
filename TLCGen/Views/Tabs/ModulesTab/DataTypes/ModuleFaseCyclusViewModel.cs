using System;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ModuleFaseCyclusViewModel : ViewModelBase, IComparable
    {
        #region Fields

        private ModuleFaseCyclusModel _ModuleFaseCyclus;
        
        #endregion // Fields

        #region Properties

        public ModuleFaseCyclusModel ModuleFaseCyclus => _ModuleFaseCyclus;

        public string FaseCyclusNaam => _ModuleFaseCyclus.FaseCyclus;

        public ObservableCollectionAroundList<ModuleFaseCyclusAlternatiefViewModel, ModuleFaseCyclusAlternatiefModel> Alternatieven
        {
            get;
        }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
	        if (!(obj is ModuleFaseCyclusViewModel fcvm))
                throw new InvalidCastException();
	        var myName = FaseCyclusNaam;
	        var hisName = fcvm.FaseCyclusNaam;
	        if (myName.Length < hisName.Length) myName = myName.PadLeft(hisName.Length, '0');
	        else if (hisName.Length < myName.Length) hisName = hisName.PadLeft(myName.Length, '0');
	        return string.Compare(myName, hisName, StringComparison.Ordinal);
        }

        #endregion // IComparable

        #region Public Methods

        #endregion // Public Methods

        public override string ToString()
        {
            return FaseCyclusNaam;
        }

        #region Constructor

        public ModuleFaseCyclusViewModel(ModuleFaseCyclusModel mfcm)
        {
            _ModuleFaseCyclus = mfcm;
            Alternatieven = new ObservableCollectionAroundList<ModuleFaseCyclusAlternatiefViewModel, ModuleFaseCyclusAlternatiefModel>(mfcm.Alternatieven);
        }

        #endregion // Constructor
    }
}
