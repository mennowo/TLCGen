using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ModulesTabViewModel : ViewModelBase
    {
        #region Fields
        
        protected ControllerViewModel _ControllerVM;
        private ModuleMolenViewModel _ModuleMolenVM;
        private ModulesTabFasenLijstViewModel _FasenLijstVM;
        
        #endregion // Fields

        #region Properties

        public ModuleMolenViewModel ModuleMolenVM
        {
            get { return _ModuleMolenVM; }
        }

        public ModulesTabFasenLijstViewModel FasenLijstVM
        {
            get { return _FasenLijstVM; }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Public Methods

        public void SetSelectedModule(ModuleViewModel mvm)
        {
            _FasenLijstVM.SelectedModule = mvm;
        }

        #endregion // Public Methods

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public ModulesTabViewModel(ControllerViewModel controllervm)
        {
            _ControllerVM = controllervm;
            _ModuleMolenVM = new ModuleMolenViewModel(_ControllerVM, this, _ControllerVM.Controller.ModuleMolen);
            _FasenLijstVM = new ModulesTabFasenLijstViewModel(_ControllerVM, this);
        }

        #endregion // Constructor
    }
}
