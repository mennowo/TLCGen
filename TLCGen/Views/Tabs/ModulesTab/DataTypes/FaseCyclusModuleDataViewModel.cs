using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class FaseCyclusModuleDataViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private FaseCyclusModuleDataModel _ModuleFaseData;

        #endregion // Fields

        #region Properties

        public string FaseCyclus
        {
            get => _ModuleFaseData.FaseCyclus;
            set
            {
                _ModuleFaseData.FaseCyclus = value;
                RaisePropertyChanged<object>(nameof(FaseCyclus), broadcast: true);
            }
        }

        public int ModulenVooruit
        {
            get => _ModuleFaseData.ModulenVooruit;
            set
            {
                _ModuleFaseData.ModulenVooruit = value;
                RaisePropertyChanged<object>(nameof(ModulenVooruit), broadcast: true);
            }
        }

        public bool AlternatiefToestaan
        {
            get => _ModuleFaseData.AlternatiefToestaan;
            set
            {
                _ModuleFaseData.AlternatiefToestaan = value;
                RaisePropertyChanged<object>(nameof(AlternatiefToestaan), broadcast: true);
            }
        }

        public int AlternatieveRuimte
        {
            get => _ModuleFaseData.AlternatieveRuimte;
            set
            {
                _ModuleFaseData.AlternatieveRuimte = value;
                RaisePropertyChanged<object>(nameof(AlternatieveRuimte), broadcast: true);
            }
        }

        public int AlternatieveGroenTijd
        {
            get => _ModuleFaseData.AlternatieveGroenTijd;
            set
            {
                _ModuleFaseData.AlternatieveGroenTijd = value;
                RaisePropertyChanged<object>(nameof(AlternatieveGroenTijd), broadcast: true);
            }
        }

        #endregion // Properties

        #region Collection Changed

        #endregion // Collection Changed

        #region Public Methods

        #endregion // Public Methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return _ModuleFaseData;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public FaseCyclusModuleDataViewModel(FaseCyclusModuleDataModel fasedata)
        {
            _ModuleFaseData = fasedata;
        }

        #endregion // Constructor
    }
}
