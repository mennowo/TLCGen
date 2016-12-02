using System;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 4)]
    public class ModulesTabViewModel : TLCGenTabItemViewModel
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

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Modulen";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        #endregion // TabItem Overrides

        #region Public Methods

        public void SetSelectedModule(ModuleViewModel mvm)
        {
            _FasenLijstVM.SelectedModule = mvm;
        }

        #endregion // Public Methods

        #region Collection Changed

        #endregion // Collection Changed

        #region TLCGen Message Handling

        private void OnFasenChanged(FasenChangedMessage message)
        {

        }

        #endregion // TLCGen Message Handling

        #region Constructor

        public ModulesTabViewModel(ControllerModel controller) : base(controller)
        {
            _ModuleMolenVM = new ModuleMolenViewModel(_Controller, this);
            _FasenLijstVM = new ModulesTabFasenLijstViewModel(this);

            MessageManager.Instance.Subscribe(this, new Action<FasenChangedMessage>(OnFasenChanged));

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

        #endregion // Constructor
    }
}
