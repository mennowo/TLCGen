using GalaSoft.MvvmLight.Messaging;
using System;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.ModulesTab)]
    public class ModulesDetailsTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

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

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Modules";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
            
        }

        public override ControllerModel Controller
        {
            get
            {
                return base.Controller;
            }

            set
            {
                base.Controller = value;
                ModuleMolenVM.Controller = value;
                FasenLijstVM.Controller = value;
                if (ModuleMolenVM.Modules.Count > 0)
                {
                    ModuleMolenVM.SelectedModule = ModuleMolenVM.Modules[0];
                    FasenLijstVM.SelectedModule = ModuleMolenVM.Modules[0];
                }

                if (FasenLijstVM.Fasen.Count > 0)
                {
                    FasenLijstVM.SelectedFaseCyclus = FasenLijstVM.Fasen[0];
                }
            }
        }

        #endregion // TabItem Overrides

        #region Public Methods

        public void SetSelectedModule(ModuleViewModel mvm)
        {
            _FasenLijstVM.SelectedModule = mvm;
        }

        public void SetSelectedModuleFase(ModuleFaseCyclusViewModel mvm)
        {
            _FasenLijstVM.SelectedModuleFase = mvm;
        }

        #endregion // Public Methods

        #region TLCGen Message Handling

        #endregion // TLCGen Message Handling

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public ModulesDetailsTabViewModel() : base()
        {
            _ModuleMolenVM = new ModuleMolenViewModel(this);
            _FasenLijstVM = new ModulesTabFasenLijstViewModel();
        }

        #endregion // Constructor
    }
}
