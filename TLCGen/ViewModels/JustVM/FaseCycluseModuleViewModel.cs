using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;

namespace TLCGen.ViewModels
{
    public class FaseCyclusModuleViewModel : ViewModelBase
    {
        #region Fields

        private FaseCyclusViewModel _FaseCyclusVM;
        private ModuleViewModel _ModuleVM;
        
        #endregion // Fields

        #region Properties

        public FaseCyclusViewModel FaseCyclusVM
        {
            get { return _FaseCyclusVM; }
        }

        public ModuleViewModel ModuleVM
        {
            get { return _ModuleVM; }
            set
            {
                _ModuleVM = value;
                OnPropertyChanged("ModuleVM");
                UpdateModuleInfo();
            }
        }

        public string Naam
        {
            get { return _FaseCyclusVM.Naam; }
        }

        public string Define
        {
            get { return _FaseCyclusVM.Define; }
        }

        public ObservableCollection<ConflictViewModel> Conflicten
        {
            get { return _FaseCyclusVM.Conflicten; }
        }

        public bool CanBeAddedToModule
        {
            get
            {
                if (_ModuleVM != null)
                {
                    foreach (ModuleFaseCyclusViewModel mfcvm in _ModuleVM.Fasen)
                    {
                        if (this.IsFaseConflicting(mfcvm))
                            return false;
                    }
                }
                return true;
            }
        }

        public bool IsInModule
        {
            get
            {
                if (_ModuleVM != null)
                {
                    foreach (ModuleFaseCyclusViewModel mfcvm in _ModuleVM.Fasen)
                    {
                        if (mfcvm.FaseCyclusDefine == this.Define)
                            return true;
                    }
                }
                return false;
            }
        }

        public bool NoModuleAvailable
        {
            get { return _ModuleVM == null; }
        }

        #endregion // Properties

        #region Collection Changed

        #endregion // Collection Changed

        #region Overrides

        public override string ToString()
        {
            return Naam;
        }

        #endregion // Overrides

        #region Public methods

        public bool IsFaseConflicting(FaseCyclusModuleViewModel fcmvm)
        {
            foreach(ConflictViewModel cvm in fcmvm.Conflicten)
            {
                if (cvm.FaseNaar == this.Define)
                    return true;
            }
            return false;
        }

        public bool IsFaseConflicting(ModuleFaseCyclusViewModel mfcvm)
        {
            foreach (ConflictViewModel cvm in this.Conflicten)
            {
                if (cvm.FaseNaar == mfcvm.FaseCyclusDefine)
                    return true;
            }
            return false;
        }

        public void UpdateModuleInfo()
        {
            OnPropertyChanged("CanBeAddedToModule");
            OnPropertyChanged("IsInModule");
            OnPropertyChanged("NoModuleAvailable"); 
        }

        #endregion // Public methods

        #region Constructor

        public FaseCyclusModuleViewModel(FaseCyclusViewModel fcvm, ModuleViewModel mvm)
        {
            _FaseCyclusVM = fcvm;
            _ModuleVM = mvm;
        }

        #endregion // Constructor
    }
}
