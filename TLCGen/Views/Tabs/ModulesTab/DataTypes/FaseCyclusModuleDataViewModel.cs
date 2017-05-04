using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            get { return _ModuleFaseData.FaseCyclus; }
            set
            {
                _ModuleFaseData.FaseCyclus = value;
                RaisePropertyChanged<object>("FaseCyclus", broadcast: true);
            }
        }

        public int ModulenVooruit
        {
            get { return _ModuleFaseData.ModulenVooruit; }
            set
            {
                _ModuleFaseData.ModulenVooruit = value;
                RaisePropertyChanged<object>("ModulenVooruit", broadcast: true);
            }
        }

        public bool AlternatiefToestaan
        {
            get { return _ModuleFaseData.AlternatiefToestaan; }
            set
            {
                _ModuleFaseData.AlternatiefToestaan = value;
                RaisePropertyChanged<object>("AlternatiefToestaan", broadcast: true);
            }
        }

        public int AlternatieveRuimte
        {
            get { return _ModuleFaseData.AlternatieveRuimte; }
            set
            {
                _ModuleFaseData.AlternatieveRuimte = value;
                RaisePropertyChanged<object>("AlternatieveRuimte", broadcast: true);
            }
        }

        public int AlternatieveGroenTijd
        {
            get { return _ModuleFaseData.AlternatieveGroenTijd; }
            set
            {
                _ModuleFaseData.AlternatieveGroenTijd = value;
                RaisePropertyChanged<object>("AlternatieveGroenTijd", broadcast: true);
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
