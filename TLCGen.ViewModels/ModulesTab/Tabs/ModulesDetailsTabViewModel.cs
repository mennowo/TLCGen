using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

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

        #endregion // TabItem Overrides

        #region Public Methods

        public void SetSelectedModule(ModuleViewModel mvm)
        {
            _FasenLijstVM.SelectedModule = mvm;
        }

        #endregion // Public Methods

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public ModulesDetailsTabViewModel(ControllerModel controller) : base(controller)
        {
            _ModuleMolenVM = new ModuleMolenViewModel(_Controller, this);
            _FasenLijstVM = new ModulesTabFasenLijstViewModel(this);

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
